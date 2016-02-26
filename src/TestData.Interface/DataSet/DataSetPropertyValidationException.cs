using System;

namespace TestData.DataSet
{
    public class DataSetPropertyValidationException : Exception
    {
        public DataSetPropertyValidationException(string message) :
            base(message)
        {
            
        }
    }
}