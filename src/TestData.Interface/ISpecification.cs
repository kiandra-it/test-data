namespace TestData.Interface
{
    public interface ISpecification<in T>
    {
        bool IsSatisfiedBy(T target);
    }
}