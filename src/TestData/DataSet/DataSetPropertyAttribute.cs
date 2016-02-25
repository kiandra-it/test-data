using System;

namespace TestData.DataSet
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DataSetPropertyAttribute : Attribute
    {
        public DataSetPropertyAttribute(string name, DataType dataType, string description = null)
        {
            Name = name;
            DataType = dataType;
            Description = description;
        }

        public string Name { get; private set; }
        public DataType DataType { get; private set; }
        public string Description { get; set; }
        public bool Required { get; set; }
    }
}