using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using TestData.DataSet;
using TestData.Requests;

namespace TestData.Handler
{
    public class DataSetRequestHandler : IAsyncRequestHandler<DataSetRequest, IEnumerable<string>>
    {
        private readonly ILifetimeScope _lifetimeScope;
        private static readonly IEnumerable<Type> DataSetTypes;

        public DataSetRequestHandler(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        static DataSetRequestHandler()
        {
            DataSetTypes = DataSetContainer.DiscoverDataSets();
        }

        public async Task<IEnumerable<string>> Handle(DataSetRequest message)
        {
            var container = new DataSetContainer(_lifetimeScope, message.Properties);
            container.AddRange(DataSetTypes);

            var messages = await container.Execute(container.DatasetsByType.Keys.First(t => t.FullName == message.DataSet));
            return messages;
        }
    }
}