using System;
using System.IO;
using CsvHelper;

namespace TestData.Interface.DataSet
{
    public class FileDataSetInstance<T>
    {
        private readonly string _path;

        public FileDataSetInstance(string path)
        {
            _path = path;
        }

        public void ForEach(Action<T> action, IDataSet dataSet)
        {
            
            using (var csv = new CsvReader(new StreamReader(Path.Combine(Path.GetDirectoryName(new Uri(dataSet.GetType().Assembly.CodeBase).LocalPath), _path))))
            {
                foreach (var record in csv.GetRecords<T>())
                {
                    action(record);
                }
            }
        }
    }
}