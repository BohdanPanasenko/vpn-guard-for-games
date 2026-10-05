using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace VpnGuardForGames
{
    static class Vpn
    {
        static readonly string[] AdapterKeywords = { "NordLayer" };

        public static bool IsConnected()
        {
            return NetworkInterface.GetAllNetworkInterfaces().Any(adapter =>
                adapter.OperationalStatus == OperationalStatus.Up &&
                AdapterKeywords.Any(keyword => adapter.Description.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0));
        }
    }
}
