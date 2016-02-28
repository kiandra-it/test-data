using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestData.Interface.DataSet;

namespace TestData.Interface.Files
{
    public class FileDescriptorExtension: IDescriptorExtension
    {
        public class FileDataSetContainer
        {
            public PropertyInfo MemberInfo { get; set; }
            public object Instance { get; set; }
        }

        public void Scan(Type datasetType, IDictionary<string, IEnumerable<object>> container)
        {
            container["files"] = 
            datasetType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.GetCustomAttribute<FileDataSetAttribute>() != null)
                .Select(p =>
                {
                    var dsa = p.GetCustomAttribute<FileDataSetAttribute>();

                    var instanceType = typeof(FileDataSetInstance<>).MakeGenericType(p.PropertyType.GetGenericArguments()[0]);
                    return new FileDataSetContainer()
                    {
                        Instance = Activator.CreateInstance(instanceType, dsa.Path),
                        MemberInfo = p
                    };
                });
        }

        public void Apply(IDataSet dataSet, IDictionary<string, IEnumerable<object>> container)
        {
            var fileDependencies = container["files"].Cast<FileDataSetContainer>();
            foreach (var fileDependency in fileDependencies)
            {
                fileDependency.MemberInfo.SetValue(dataSet, fileDependency.Instance);
            }
        }
    }
}
