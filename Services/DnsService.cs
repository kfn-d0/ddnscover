using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DDNScover.Models;
using DDNScover.Services.DataSources;

namespace DDNScover.Services;

public class DnsService
{
    private readonly List<IDataSource> _dataSources;
    private readonly HttpClient _httpClient;

    public DnsService()
    {
        _httpClient = new HttpClient();
        _httpClient.Timeout = TimeSpan.FromSeconds(30);

        _dataSources = new List<IDataSource>
        {
            new CrtShSource(_httpClient),
            new AlienVaultSource(_httpClient),
            new HackerTargetSource(_httpClient)
        };
    }

    public async Task<List<DiscoveredHost>> GetSubdomainsAsync(string domain)
    {
        var allResults = new List<DiscoveredHost>();

        var tasks = _dataSources.Select(async source => 
        {
            try
            {
                var hosts = await source.GetSubdomainsAsync(domain);
                return hosts;
            }
            catch
            {
                return new List<DiscoveredHost>();
            }
        });

        var results = await Task.WhenAll(tasks);

        var uniqueHostsMap = new Dictionary<string, DiscoveredHost>(StringComparer.OrdinalIgnoreCase);

        foreach (var sourceResults in results)
        {
            foreach (var host in sourceResults)
            {
                var cleanName = host.Hostname.Trim().TrimEnd('.');
                if (cleanName.StartsWith("*.")) cleanName = cleanName.Substring(2);

                if (!string.Equals(cleanName, domain, StringComparison.OrdinalIgnoreCase) && 
                     cleanName.EndsWith("." + domain, StringComparison.OrdinalIgnoreCase))
                {
                    if (uniqueHostsMap.TryGetValue(cleanName, out var existing))
                    {
                        if (!existing.Source.Contains(host.Source, StringComparison.OrdinalIgnoreCase))
                        {
                            existing.Source += $", {host.Source}";
                        }
                    }
                    else
                    {
                        host.Hostname = cleanName;
                        uniqueHostsMap[cleanName] = host;
                        allResults.Add(host);
                    }
                }
            }
        }

        return allResults.OrderBy(x => x.Hostname).ToList();
    }
}
