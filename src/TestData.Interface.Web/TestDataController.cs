using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using TestData.Interface.DataSet;

namespace TestData.Interface.Web
{
    [RoutePrefix("api/testdata")]
    public class TestDataController : ApiController
    {
        private readonly IDispatcher _dispatcher;
        private static readonly IEnumerable<Type> DataSetTypes;

        public TestDataController(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        static TestDataController()
        {
            DataSetTypes = DataSetContainer.DiscoverDataSets();
        }

        [HttpGet]
        [Route]
        public IHttpActionResult Get()
        {
            var descriptors = DataSetTypes.Select(dst => new DataSetDescriptor(dst));

            return Ok(descriptors.Select(d => new
            {
                Dependencies = d.Dependencies.Select(t => t.FullName),
                d.Name,
                d.Description,
                d.Type.FullName,
                TypeName = d.Type.Name,
                Properties = d.Properties.Select(p => new
                {
                    FieldName = p.MemberInfo.Name,
                    p.Property.Name,
                    p.Property.Description,
                    p.Property.DataType,
                    p.Property.Required
                })
            }));
        }

        [HttpPost]
        [Route]
        public async Task<IHttpActionResult> Post([FromBody] DataSetRequest request)
        {
            return Ok(await _dispatcher.DispatchAsync(request));
        }
    }
}
