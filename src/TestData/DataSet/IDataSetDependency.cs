namespace TestData.DataSet
{
    public interface IDataSetDependency<TDataSet>
    {
        int Order { get; }
    }
}