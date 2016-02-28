using System.Web.Http;

namespace demo.Controllers
{
    [RoutePrefix("api/test")]
    public class TestController: ApiController
    {
        public TestController()
        {
            
        }

        [Route]
        public IHttpActionResult Get()
        {
            return Ok("Hi!");
        }
    }
}