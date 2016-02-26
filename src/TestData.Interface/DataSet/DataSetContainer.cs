using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;

namespace TestData.DataSet
{
    public class DataSetContainer
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly IDictionary<string, DataSetDescriptor> _datasetsByName = new Dictionary<string, DataSetDescriptor>();

        public IDictionary<string, IDictionary<string, string>> Properties { get; set; }

        public IDictionary<Type, DataSetDescriptor> DatasetsByType { get; } = new Dictionary<Type, DataSetDescriptor>();

        public DataSetContainer(ILifetimeScope lifetimeScope, IDictionary<string, IDictionary<string, string>> properties = null)
        {
            _lifetimeScope = lifetimeScope;
            Properties = properties ?? new Dictionary<string, IDictionary<string, string>>();
        }

        public static IEnumerable<Type> DiscoverDataSets()
        {
            return DiscoverDataSets(AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.Contains(".Tests")));
        }

        public static IEnumerable<Type> DiscoverDataSets(IEnumerable<Assembly> assemblies)
        {
            return assemblies.SelectMany(a => a.GetTypes())
                .Where(t => t.GetCustomAttribute<DataSetAttribute>() != null);
        }

        public void AddRange(IEnumerable<Type> dataSetTypes)
        {
            foreach (var t in dataSetTypes)
            {
                Add(t);
            }
        }

        public void Add(Type dataSetType)
        {
            var descriptor = new DataSetDescriptor(dataSetType);
            _datasetsByName.Add(descriptor.Name, descriptor);
            DatasetsByType.Add(descriptor.Type, descriptor);
        }

        public async Task<IEnumerable<string>> Execute(string name)
        {
            return await Execute(_datasetsByName[name]);
        }

        public async Task<IEnumerable<string>> Execute(Type type)
        {
            return await Execute(DatasetsByType[type]);
        }

        private async Task<IEnumerable<string>> Execute(DataSetDescriptor descriptor)
        {
            var messages = new List<string>();
            var dataSet =_lifetimeScope.Resolve(descriptor.Type) as IDataSet;
            //determine dependencies
            foreach (var dependency in descriptor.Dependencies)
            {
                var dependencySetDescriptor = DatasetsByType[dependency];
                messages.AddRange(await Execute(dependencySetDescriptor));
            }

            foreach (var fileDependency in descriptor.FileDependencies)
            {
                fileDependency.MemberInfo.SetValue(dataSet, fileDependency.Instance);
            }

            foreach (var property in descriptor.Properties)
            {
                if (Properties.ContainsKey(descriptor.Type.FullName))
                {
                    var name = property.MemberInfo.Name;
                    if (Properties[descriptor.Type.FullName].ContainsKey(name) && Properties[descriptor.Type.FullName][name] != null)
                    {
                        var value = Convert.ChangeType(Properties[descriptor.Type.FullName][name], property.MemberInfo.PropertyType);
                        property.MemberInfo.SetValue(dataSet, value);
                    }
                }

                if (!new DataSetPropertyRequiredSpecification(property)
                    .IsSatisfiedBy(Properties.ContainsKey(descriptor.Type.FullName) ? Properties[descriptor.Type.FullName] : null))
                {
                    throw new DataSetPropertyValidationException($"The property {property.Property.Name} must have a value");
                }
            }

            messages.Add(await dataSet.Execute());
            return messages;
        }
    }
}