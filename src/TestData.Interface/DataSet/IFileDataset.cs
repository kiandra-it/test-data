namespace TestData.Interface.DataSet
{
    public interface IFileDataSet<TModel>
        where TModel : class, new()
    {
        string Path { get; }
    }
}