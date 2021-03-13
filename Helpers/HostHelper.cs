using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Helpers
{
    public static class HostHelper
    {
        private const string _localHostIp = "127.0.0.1";
        private const string _localHostName = "localhost";

        public static string GetIpAddress()
        {
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());

                if (hostEntry != null && hostEntry.AddressList != null && hostEntry.AddressList.Length > 0)
                {
                    foreach (IPAddress ipAddress in hostEntry.AddressList)
                    {
                        if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return ipAddress.ToString();
                        }
                    }
                }

                return _localHostIp;
            }
            catch
            {
                return _localHostIp;
            }
        }

        public static string GetHostName(string ipAddress)
        {
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(ipAddress);

                if (hostEntry != null)
                {
                    return hostEntry.HostName;
                }

                return _localHostName;
            }
            catch
            {
                return _localHostName;
            }
        }

        public static string GetRemoteIpAddress(this HttpContext httpContext)
        {            
            try
            {                
                return httpContext?.Connection?.RemoteIpAddress?.ToString() ?? _localHostIp;
            }
            catch
            {
                return _localHostIp;
            }
        }
    }
}