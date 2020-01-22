using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Untappd.KolosBot.Controllers
{
    [ApiController]
    [Route("hello")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var botClient = new TelegramBotClient("926979704:AAGGACK5icmibmSyrVpIwWS8uFl2DTsBqjo");
            var me = await botClient.GetMeAsync();
            return Ok($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");
        }
    }
}
