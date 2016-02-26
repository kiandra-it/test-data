using System;

namespace TestData.DataSet
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FileDataSetAttribute : Attribute
    {
        public FileDataSetAttribute(string path)
        {
            Path = path;
        }

        public string Path { get; set; }
    }
}