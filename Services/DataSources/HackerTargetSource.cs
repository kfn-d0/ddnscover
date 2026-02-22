using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DDNScover.Models;

namespace DDNScover.Services.DataSources;

public class HackerTargetSource : IDataSource
{
    public string Name => "HackerTarget";
    private readonly HttpClient _httpClient;

    public HackerTargetSource(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<DiscoveredHost>> GetSubdomainsAsync(string domain)
    {
        try
        {
            var url = $"https://api.hackertarget.com/hostsearch/?q={domain}";
            var response = await _httpClient.GetStringAsync(url);
            
            var results = new List<DiscoveredHost>();
            if (response.Contains("error") || response.Contains("API count exceeded")) return results;

            var lines = response.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length > 0)
                {
                    var hostname = parts[0];
                     results.Add(new DiscoveredHost { Hostname = hostname.Trim(), Source = Name });
                }
            }
            return results;
        }
        catch
        {
            return new List<DiscoveredHost>();
        }
    }
}
