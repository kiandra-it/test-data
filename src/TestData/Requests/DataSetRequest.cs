using System.Collections.Generic;
using MediatR;

namespace TestData.Requests
{
    public class DataSetRequest : IAsyncRequest<IEnumerable<string>>
    {
        public string DataSet { get; set; }
        public IDictionary<string, IDictionary<string, string>> Properties { get; set; }
    }
}