using System;

namespace TestData.Interface.DataSet
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DataSetAttribute : Attribute
    {
        public DataSetAttribute(string name, string description = null)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
    }
}