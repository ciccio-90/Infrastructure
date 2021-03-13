using System.Globalization;
using System.Threading;

namespace Infrastructure.Helpers
{
    public static class PriceHelper
    {
        public static string FormatMoney(this decimal price)
        {
            try
            {                
                var regionInfo = new RegionInfo(Thread.CurrentThread?.CurrentCulture?.LCID ?? 2057);

                return $"{regionInfo?.CurrencySymbol ?? "£"}{price}";
            }
            catch
            {
                return $"£{price}";
            }
        }
    }
}