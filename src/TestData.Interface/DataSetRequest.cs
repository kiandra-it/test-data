using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestData.Interface
{
    public class DataSetRequest: IDataSetRequest
    {
        public string DataSet { get; set; }
        public IDictionary<string, IDictionary<string, string>> Properties { get; set; }
    }
}
