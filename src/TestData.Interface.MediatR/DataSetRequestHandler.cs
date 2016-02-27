using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using TestData.Interface.DataSet;

namespace TestData.Interface.MediatR
{
    public class DataSetRequestHandler : IAsyncRequestHandler<DataSetRequest, IEnumerable<string>>
    {
        private readonly Func<Type, IDataSet> _dataSetResolver;
        private static readonly IEnumerable<Type> DataSetTypes;

        public DataSetRequestHandler(Func<Type, IDataSet> dataSetResolver)
        {
            _dataSetResolver = dataSetResolver;
        }

        static DataSetRequestHandler()
        {
            DataSetTypes = DataSetContainer.DiscoverDataSets();
        }

        public async Task<IEnumerable<string>> Handle(DataSetRequest message)
        {
            var container = new DataSetContainer(_dataSetResolver, message.Properties);
            container.AddRange(DataSetTypes);

            var messages = await container.Execute(container.DatasetsByType.Keys.First(t => t.FullName == message.DataSet));
            return messages;
        }
    }
}