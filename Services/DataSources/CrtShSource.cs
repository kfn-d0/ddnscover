using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DDNScover.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DDNScover.Services.DataSources;

public class CrtShSource : IDataSource
{
    public string Name => "crt.sh";
    private readonly HttpClient _httpClient;

    public CrtShSource(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<DiscoveredHost>> GetSubdomainsAsync(string domain)
    {
        try
        {
            var url = $"https://crt.sh/?q=%.{domain}&output=json";
            var response = await _httpClient.GetStringAsync(url);
            
            var results = new List<DiscoveredHost>();
            var jsonArray = JsonConvert.DeserializeObject<JArray>(response);
            if (jsonArray == null) return results;

            foreach (var item in jsonArray)
            {
                var nameValue = item["name_value"]?.ToString();
                if (string.IsNullOrWhiteSpace(nameValue)) continue;

                var names = nameValue.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var name in names)
                {
                   results.Add(new DiscoveredHost { Hostname = name.Trim(), Source = Name });
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
