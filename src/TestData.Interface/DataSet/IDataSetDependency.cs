namespace TestData.Interface.DataSet
{
    public interface IDataSetDependency<TDataSet>
    {
        int Order { get; }
    }
}