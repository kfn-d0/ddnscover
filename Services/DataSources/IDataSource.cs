using System.Collections.Generic;
using System.Threading.Tasks;
using DDNScover.Models;

namespace DDNScover.Services.DataSources;

public interface IDataSource
{
    string Name { get; }
    Task<List<DiscoveredHost>> GetSubdomainsAsync(string domain);
}
