using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TestData.Interface.DataSet
{
    public interface IDescriptorExtension
    {
        void Scan(Type datasetType, IDictionary<string, IEnumerable<object>> containe);
        void Apply(IDataSet dataSet, IDictionary<string, IEnumerable<object>> container);
    }

    public class DataSetDescriptor
    {
        public class DataSetPropertyContainer
        {
            public PropertyInfo MemberInfo { get; set; }
            public DataSetPropertyAttribute Property { get; set; }
        }

        public DataSetDescriptor(Type datasetType)
        {
            ExtensionObjects = new Dictionary<Type, IDictionary<string, IEnumerable<object>>>();

            var ds = datasetType.GetCustomAttribute<DataSetAttribute>();
            if (ds == null)
            {
                throw new ArgumentException("Not a dataset", nameof(datasetType));
            }

            Name = ds.Name;
            Description = ds.Description;

            Properties = datasetType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(p => new DataSetPropertyContainer
                {
                    MemberInfo = p,
                    Property = p.GetCustomAttribute<DataSetPropertyAttribute>()
                })
                .Where(p => p.Property != null);

            Dependencies = datasetType.GetCustomAttributes<DataSetDependencyAttribute>()
                .OrderBy(a => a.Order)
                .Select(a => a.DependencyType);

            foreach (var e in DataSetContainer.DescriptorExtensions)
            {
                e.Scan(datasetType, ExtensionObjects[e.GetType()] = new Dictionary<string, IEnumerable<object>>());
            }

            Type = datasetType;
        }

        public string Name { get; }
        public Type Type { get; }
        public IEnumerable<Type> Dependencies { get; }
        public IDictionary<Type, IDictionary<string, IEnumerable<object>>> ExtensionObjects { get; }
        public IEnumerable<DataSetPropertyContainer> Properties { get; }
        public string Description { get; }
    }
}