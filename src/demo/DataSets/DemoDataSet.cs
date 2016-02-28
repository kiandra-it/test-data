using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TestData.Interface.DataSet;

namespace demo.DataSets
{
    [DataSet("Demo Data Set", "Demonstrates how to use datasets")]
    public class DemoDataSet: IDataSet
    {
        [DataSetProperty("Name", DataType.String, "Your Name", Required = true)]
        public string Name { get; set; }

        public async Task<string> Execute()
        {
            return $"Your Awesome {Name}!";
        }
    }
}