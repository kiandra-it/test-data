using System;

namespace TestData.Interface.DataSet
{
    public class DataSetPropertyValidationException : Exception
    {
        public DataSetPropertyValidationException(string message) :
            base(message)
        {
            
        }
    }
}