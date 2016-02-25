namespace TestData
{
    public interface ISpecification<in T>
    {
        bool IsSatisfiedBy(T target);
    }
}