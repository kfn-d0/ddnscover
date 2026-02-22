using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Linq;

namespace DDNScover.Services;

public class PingService
{
    public async Task<bool> PingHostAsync(string host)
    {
        try
        {
            var addresses = await Dns.GetHostAddressesAsync(host);
            var ipv4Address = addresses.FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

            if (ipv4Address == null) return false;

            using var pinger = new Ping();
            int successCount = 0;
            
            for (int i = 0; i < 3; i++)
            {
                try 
                {
                    var reply = await pinger.SendPingAsync(ipv4Address, 2000);
                    if (reply.Status == IPStatus.Success)
                    {
                        successCount++;
                    }
                }
                catch 
                {
                }
            }

            return successCount > 0;
        }
        catch
        {
            return false;
        }
    }
}
