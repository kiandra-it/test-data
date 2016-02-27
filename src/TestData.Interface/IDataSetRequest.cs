using System.Collections.Generic;

namespace TestData.Interface
{
    public interface IDataSetRequest
    {
        string DataSet { get; }
        IDictionary<string, IDictionary<string, string>> Properties { get; }
    }
}