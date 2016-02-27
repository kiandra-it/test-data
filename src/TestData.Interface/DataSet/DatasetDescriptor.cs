using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TestData.Interface.DataSet
{
    public class DataSetDescriptor
    {
        public class DataSetPropertyContainer
        {
            public PropertyInfo MemberInfo { get; set; }
            public DataSetPropertyAttribute Property { get; set; }
        }

        public class FileDataSetContainer
        {
            public PropertyInfo MemberInfo { get; set; }
            public object Instance { get; set; }
        }

        public DataSetDescriptor(Type datasetType)
        {
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

            FileDependencies = datasetType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.GetCustomAttribute<FileDataSetAttribute>() != null)
                .Select(p =>
                {
                    var dsa = p.GetCustomAttribute<FileDataSetAttribute>();

                    var instanceType = typeof (FileDataSetInstance<>).MakeGenericType(p.PropertyType.GetGenericArguments()[0]);
                    return new FileDataSetContainer()
                    {
                        Instance = Activator.CreateInstance(instanceType, dsa.Path),
                        MemberInfo = p
                    };
                });
                

            Type = datasetType;
        }

        public string Name { get; }
        public Type Type { get; }
        public IEnumerable<Type> Dependencies { get; }
        public IEnumerable<FileDataSetContainer> FileDependencies { get; }
        public IEnumerable<DataSetPropertyContainer> Properties { get; }
        public string Description { get; }
    }
}