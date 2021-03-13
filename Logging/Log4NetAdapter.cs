using System.IO;
using System.Reflection;
using System.Xml;
using log4net;

namespace Infrastructure.Logging
{
    public class Log4NetAdapter : ILogger
    {
        private readonly log4net.ILog _log = LogManager.GetLogger(typeof(Log4NetAdapter));

        public Log4NetAdapter()
        {
            XmlDocument log4netConfig = new XmlDocument();  

            log4netConfig.Load(File.OpenRead(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Logging", "log4net.config")));  
   
            var repo = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));  
   
            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);
        }

        public void Log(string message)
        {
            _log.Info(message);
        }
    }
}