using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using TestData.DataSet;
using TestData.Requests;

namespace TestData.Web
{
    [RoutePrefix("api/testdata")]
    public class TestDataController : ApiController
    {
        private readonly IMediator _mediator;
        private static readonly IEnumerable<Type> DataSetTypes;

        public TestDataController(IMediator mediator)
        {
            _mediator = mediator;
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
            return Ok(await _mediator.SendAsync(request));
        }
    }
}
