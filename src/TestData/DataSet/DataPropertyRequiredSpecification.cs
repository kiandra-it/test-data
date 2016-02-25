using System.Collections.Generic;

namespace TestData.DataSet
{
    public class DataSetPropertyRequiredSpecification : ISpecification<IDictionary<string, string>>
    {
        private readonly DataSetDescriptor.DataSetPropertyContainer _container;

        public DataSetPropertyRequiredSpecification(DataSetDescriptor.DataSetPropertyContainer container)
        {
            _container = container;
        }

        public bool IsSatisfiedBy(IDictionary<string, string> target)
        {
            if (!_container.Property.Required)
            {
                return true;
            }

            if (target == null) return false;

            if (target.ContainsKey(_container.MemberInfo.Name) && target[_container.MemberInfo.Name] != null)
            {
                return true;
            }

            return false;
        }
    }
}
