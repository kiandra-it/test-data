using System;

namespace TestData.Interface.DataSet
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DataSetDependencyAttribute : Attribute
    {
        public DataSetDependencyAttribute(Type dependencyType)
        {
            DependencyType = dependencyType;
        }

        public Type DependencyType { get; set; }
        public int Order { get; set; }
    }
}