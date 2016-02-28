namespace TestData.Interface.Files
{
    public interface IFileDataSet<TModel>
        where TModel : class, new()
    {
        string Path { get; }
    }
}