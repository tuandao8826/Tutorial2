using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tutorial.Infrastructure.Services.Interfaces;

namespace Tutorial.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestDIController : ControllerBase
    {
        ITransientService _transientService1;
        ITransientService _transientService2;

        IScopedService _scopedService1;
        IScopedService _scopedService2;

        ISingletonService _singletonService1;
        ISingletonService _singletonService2;
        private readonly ISingletonService _cdService;

        public TestDIController(ITransientService transientService1,
                          ITransientService transientService2,
                          IScopedService scopedService1,
                          IScopedService scopedService2,
                          ISingletonService singletonService1,
                          ISingletonService singletonService2,
                          ISingletonService cdService)
        {

            _transientService1 = transientService1;
            _transientService2 = transientService2;

            _scopedService1 = scopedService1;
            _scopedService2 = scopedService2;

            _singletonService1 = singletonService1;
            _singletonService2 = singletonService2;
            this._cdService = cdService;
        }

        [HttpGet("Index1")]
        public async Task<IActionResult> Index()
        {
            var result = new
            {
                message1 = "First Instance " + _transientService1.GetID().ToString(),
                message2 = "Second Instance " + _transientService2.GetID().ToString(),

                message3 = "First Instance " + _scopedService1.GetID().ToString(),
                message4 = "Second Instance " + _scopedService2.GetID().ToString(),

                message5 = "First Instance " + _singletonService1.GetID().ToString(),
                message6 = "Second Instance " + _singletonService2.GetID().ToString()
            };

            return Ok(result);
        }

        [HttpGet("Index2")]
        public async Task<IActionResult> Index2()
        {
            var result = new
            {
                message3 = "First Instance " + _scopedService1.GetID().ToString(),
                message4 = "Second Instance " + _scopedService2.GetID().ToString(),

                message5 = "First Instance " + _singletonService1.GetID().ToString(),
                message6 = "Second Instance " + _singletonService2.GetID().ToString()
            };

            return Ok(result);
        }

        [HttpGet("Index3")]
        public async Task<IActionResult> Index3()
        {
            var result = new
            {
                message6 = "Second Instance " + _cdService.GetID().ToString()
            };

            return Ok(result);
        }
    }
}
