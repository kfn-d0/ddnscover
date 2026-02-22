using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DDNScover.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DDNScover.Services.DataSources;

public class AlienVaultSource : IDataSource
{
    public string Name => "AlienVault OTX";
    private readonly HttpClient _httpClient;

    public AlienVaultSource(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<DiscoveredHost>> GetSubdomainsAsync(string domain)
    {
        try
        {
            var url = $"https://otx.alienvault.com/api/v1/indicators/domain/{domain}/passive_dns";
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
            
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return new List<DiscoveredHost>();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<JObject>(json);
            
            var results = new List<DiscoveredHost>();
            var passiveDns = data?["passive_dns"] as JArray;
            
            if (passiveDns != null)
            {
                foreach (var item in passiveDns)
                {
                    var hostname = item["hostname"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(hostname))
                    {
                         results.Add(new DiscoveredHost { Hostname = hostname.Trim(), Source = Name });
                    }
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
