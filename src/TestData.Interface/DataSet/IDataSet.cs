using System.Threading.Tasks;

namespace TestData.Interface.DataSet
{
    /// <summary>
    /// Do not directly implement this interface
    /// </summary>
    public interface IDataSet
    {
        Task<string> Execute();
    }
}