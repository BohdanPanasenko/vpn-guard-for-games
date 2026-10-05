using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace VpnGuardForGames
{
    static class Vpn
    {
        public static bool IsConnected(string[] adapterKeywords)
        {
            return NetworkInterface.GetAllNetworkInterfaces().Any(adapter =>
                adapter.OperationalStatus == OperationalStatus.Up &&
                adapterKeywords.Any(keyword => adapter.Description.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0));
        }
    }
}
