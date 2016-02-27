using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestData.Interface
{
    public interface IDispatcher
    {
        Task<IEnumerable<string>> DispatchAsync(IDataSetRequest request);
    }
}