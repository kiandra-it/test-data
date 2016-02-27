using System.Collections.Generic;
using MediatR;

namespace TestData.Interface.MediatR
{
    

    public class DataSetRequest : IAsyncRequest<IEnumerable<string>>, IDataSetRequest
    {
        public string DataSet { get; set; }
        public IDictionary<string, IDictionary<string, string>> Properties { get; set; }
    }
}