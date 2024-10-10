using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MQTTnet.Client;
using MQTTnet;
using Tutorial.Infrastructure.Services;
using Tutorial.Infrastructure.Services.Interfaces;
using MQTTnet.Server;
using System.Text;
using System.Text.Json;
using Tutorial.Infrastructure.Facades.Common.HttpClients;
using Tutorial.Infrastructure.Facades.Common.HttpClients.Interfaces;
using System.Net.Http;

namespace Tutorial.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestMQTTController : ControllerBase
    {
        private readonly IMQTTService _mQTTService;
        private readonly IHttpClientSender _httpClientSender;

        public TestMQTTController(IMQTTService mQTTService, IHttpClientSender httpClientSender)
        {
            this._mQTTService = mQTTService;
            this._httpClientSender = httpClientSender;
        }

        [HttpGet("GetDataAsync")]
        public async Task<IActionResult> GetDataAsync()
        {
            await _mQTTService.ConnectAsync();

            await _mQTTService.CreateSubscribeOptionsBuilder("mype/test/command/1");

            return Ok(await _mQTTService.WaitForMessageAsync());
        }
    }
}
