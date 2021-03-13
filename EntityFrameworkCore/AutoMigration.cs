using Infrastructure.Logging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EntityFrameworkCore
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Just because")]
    public class AutoMigration : IOperationReporter
    {
        private readonly DataContext _dataContext;
        private readonly ILogger _logger;

        public enum MigrationResult
        {
            Noop,
            Created,
            Migrated,
            AutoMigrated
        }

        public AutoMigration(DataContext dataContext, ILogger logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }

        public bool AllowDestructive { get; set; } = true;
        public bool MigrateNewDatabase { get; set; } = false;

        void IOperationReporter.WriteError(string message) => _logger.Log(message);
        void IOperationReporter.WriteInformation(string message) => _logger.Log(message);
        void IOperationReporter.WriteVerbose(string message) => _logger.Log(message);
        void IOperationReporter.WriteWarning(string message) => _logger.Log(message);

        private DbCommand GetNewCommand()
        {
            DbConnection connection = _dataContext.Database.GetDbConnection();
            DbCommand command = connection.CreateCommand();
            command.Transaction = _dataContext.Database.CurrentTransaction?.GetDbTransaction();
            
            return command;
        }

        private async Task<string> ReadSnapshotSource()
        {
            using DbCommand command = GetNewCommand();
            command.CommandText = "SELECT Snapshot FROM AutoMigrations";

            await _dataContext.Database.OpenConnectionAsync();

            try
            {
                using DbDataReader dataReader = command.ExecuteReader();

                if (!await dataReader.ReadAsync())
                {
                    return null;
                }

                using GZipStream stream = new GZipStream(dataReader.GetStream(0), CompressionMode.Decompress);
                
                return await new StreamReader(stream).ReadToEndAsync();
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                await _dataContext.Database.CloseConnectionAsync();
            }
        }

        private async Task WriteSnapshotSource(string source)
        {
            await _dataContext.Database.ExecuteSqlRawAsync("IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE name = 'AutoMigrations' AND xtype = 'U') CREATE TABLE AutoMigrations (Snapshot VARBINARY(MAX) NULL)");
            await _dataContext.Database.ExecuteSqlRawAsync("INSERT INTO AutoMigrations(Snapshot) SELECT NULL WHERE NOT EXISTS(SELECT 1 FROM AutoMigrations)");

            using MemoryStream dbStream = new MemoryStream();

            using (GZipStream blobStream = new GZipStream(dbStream, CompressionLevel.Fastest, true))
            {
                await blobStream.WriteAsync(Encoding.UTF8.GetBytes(source));
            }

            dbStream.Seek(0, SeekOrigin.Begin);

            await _dataContext.Database.ExecuteSqlInterpolatedAsync($"UPDATE AutoMigrations SET Snapshot = {dbStream.ToArray()}");
        }

        private T Compile<T>(string source, IEnumerable<Assembly> references) 
        {
            CSharpParseOptions options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);
            CSharpCompilationOptions compileOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithAssemblyIdentityComparer(DesktopAssemblyIdentityComparer.Default);
            CSharpCompilation compilation = CSharpCompilation.Create("Dynamic", new[] { SyntaxFactory.ParseSyntaxTree(source, options) }, references.Select(a => MetadataReference.CreateFromFile(a.Location)), compileOptions);
            using MemoryStream memoryStream = new MemoryStream();
            EmitResult emitResult = compilation.Emit(memoryStream);

            if (!emitResult.Success) 
            {
                throw new InvalidOperationException("Compilation failed.");
            }

            memoryStream.Seek(0, SeekOrigin.Begin);

            AssemblyLoadContext context = AssemblyLoadContext.Default;
            Assembly assembly = context.LoadFromStream(memoryStream);
            System.Reflection.TypeInfo modelType = assembly.DefinedTypes.Single(t => typeof(T).IsAssignableFrom(t));

            return (T)Activator.CreateInstance(modelType);
        }

        private ModelSnapshot CompileSnapshot(Assembly migrationAssembly, string source) =>           
            Compile<ModelSnapshot>(source, new HashSet<Assembly>() {
                AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "netstandard"),
                typeof(object).Assembly,
                typeof(DbContext).Assembly,
                migrationAssembly,
                _dataContext.GetType().Assembly,
                typeof(DbContextAttribute).Assembly,
                typeof(ModelSnapshot).Assembly,
                typeof(SqlServerValueGenerationStrategy).Assembly,
                typeof(AssemblyTargetedPatchBandAttribute).Assembly,
                AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "System.Runtime")
            });

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Just because")]
        private async Task<string> AutoMigrate(Assembly migrationAssembly, IModel oldModel, IModel newModel)
        {
            DesignTimeServicesBuilder builder = new DesignTimeServicesBuilder(migrationAssembly, Assembly.GetEntryAssembly(), this, null);
            IServiceProvider services = builder.Build(_dataContext);
            MigrationsScaffolderDependencies dependencies = services.GetRequiredService<MigrationsScaffolderDependencies>();
            string name = dependencies.MigrationsIdGenerator.GenerateId("Auto");
            string insert = dependencies.HistoryRepository.GetInsertScript(new HistoryRow(name, (string)newModel.FindAnnotation("ProductVersion")?.Value ?? "Unknown version"));

            if (oldModel == null)
            {
                await _dataContext.Database.EnsureCreatedAsync();
                await _dataContext.Database.ExecuteSqlRawAsync(dependencies.HistoryRepository.GetCreateScript());
                await _dataContext.Database.ExecuteSqlRawAsync(insert);
            }
            else
            {
                oldModel = dependencies.SnapshotModelProcessor.Process(oldModel);
                List<MigrationOperation> operations = dependencies.MigrationsModelDiffer.GetDifferences(oldModel.GetRelationalModel(), newModel.GetRelationalModel()).Where(o => !(o is UpdateDataOperation)).ToList();

                if (!operations.Any())
                {
                    return null;
                }

                if (!AllowDestructive && operations.Any(o => o.IsDestructiveChange))
                {
                    throw new InvalidOperationException("Automatic migration was not applied because it could result in data loss.");
                }

                operations.Add(new SqlOperation { Sql = insert });

                IMigrationsSqlGenerator sqlGenerator = _dataContext.GetService<IMigrationsSqlGenerator>();
                IReadOnlyList<MigrationCommand> commands = sqlGenerator.Generate(operations, _dataContext.Model);
                IMigrationCommandExecutor executor = _dataContext.GetService<IMigrationCommandExecutor>();

                await executor.ExecuteNonQueryAsync(commands, _dataContext.GetService<IRelationalConnection>());
            }

            IMigrationsCodeGenerator codeGen = dependencies.MigrationsCodeGeneratorSelector.Select(null);

            return codeGen.GenerateSnapshot("AutoMigrations", _dataContext.GetType(), $"Migration_{name}", newModel);
        }

        public async Task<MigrationResult> Migrate()
        {
            MigrationResult migrationResult = MigrationResult.Noop;
            IMigrationsAssembly migrationAssembly = _dataContext.GetService<IMigrationsAssembly>();
            IEnumerable<string> migrations = _dataContext.Database.GetMigrations();
            List<string> appliedMigrations = (await _dataContext.Database.GetAppliedMigrationsAsync()).ToList();
            bool migrateDatabase = MigrateNewDatabase || migrations.Intersect(appliedMigrations).Any();
            bool pendingMigrations = migrateDatabase && migrations.Except(appliedMigrations).Any();
            string devMigration = appliedMigrations.Except(migrations).LastOrDefault();
            ModelSnapshot modelSnapshot = null;

            if (devMigration != null)
            {
                if (pendingMigrations)
                {
                    throw new InvalidOperationException("An automatic migration has been run, but you've added new release migration(s).\nYou'll need to restore from a release database.");
                }

                string source = await ReadSnapshotSource();

                if (source == null || !source.Contains(devMigration))
                {
                    throw new InvalidOperationException($"Expected to find the source code of the {devMigration} ModelSnapshot stored in the database.");
                }

                modelSnapshot = CompileSnapshot(migrationAssembly.Assembly, source);
            }
            else
            {
                if (migrateDatabase)
                {
                    if (pendingMigrations)
                    {
                        await _dataContext.Database.MigrateAsync();

                        migrationResult = MigrationResult.Migrated;
                    }

                    modelSnapshot = migrationAssembly.ModelSnapshot;
                }
            }

            string newSnapshot = await AutoMigrate(migrationAssembly.Assembly, modelSnapshot?.Model, _dataContext.Model);
            
            if (newSnapshot != null)
            {
                migrationResult = appliedMigrations.Any() ? MigrationResult.AutoMigrated : MigrationResult.Created;
                
                await WriteSnapshotSource(newSnapshot);
            }

            return migrationResult;
        }
    }
}