using DotnetAPI.Data;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        public TestController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("TestConnection")]
        public DateTime GetPosts(int postId = 0, int userId = 0, string searchParam = "None")
        {
            return _dapper.LoadSingleData<DateTime>("SELECT GETDATE()");
        }

        [HttpGet]
        public string GetMyPosts()
        {
            return "Your application is up and runnning";
        }
    }
}