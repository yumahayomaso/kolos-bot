using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Untappd.KolosBot.Infrastucture;

namespace Untappd.KolosBot.Controllers
{
    [ApiController]
    public class TelegramController : ControllerBase
    {
        private readonly ITelegramBotClient _botClient;

        public TelegramController(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        [HttpGet("check")]
        public async Task<IActionResult> MessageGet()
        {
            return Ok("Zalupa");
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> MessageUpdate(Update update)
        {
            var message = update.Message;
            try
            {
                if (message.Text == "/pio")
                {
                    var parser = new UntappdParser();
                    var beers = await parser.GetBeers();

                    foreach (var beer in beers)
                    {
                        await _botClient.SendPhotoAsync(message.Chat.Id, new InputOnlineFile(new Uri(beer.ImageUrl)));
                        await _botClient.SendTextMessageAsync(
                            chatId: message.Chat,
                            text: beer.ToString(),
                            ParseMode.Markdown
                        );
                    }
                }


                if (update.Type == UpdateType.Message && message.Type == MessageType.Text && message.Text.Contains("/pidor"))
                {
                    await Task.Delay(5000);
                    await _botClient.SendTextMessageAsync(
                        chatId: message.Chat,
                        text: "Оййй та бля пацани"
                    );
                    await _botClient.SendTextMessageAsync(
                        chatId: message.Chat,
                        text: "І так панятно що"
                    );
                    await _botClient.SendTextMessageAsync(
                        chatId: message.Chat,
                        text: "ЙОНЗА ПЕДРИЛО"
                    );
                }

                if (message.Text != null && yosaList.Any(x => message.Text.Contains(x, StringComparison.InvariantCultureIgnoreCase)))
                {
                    var rand = new Random();
                    var randIndex = rand.Next(0, yosaJokeList.Count - 1);
                    await _botClient.SendTextMessageAsync(
                        chatId: message.Chat,
                        text: yosaJokeList[randIndex]
                    );
                }
            }
            catch (Exception exception)
            {
                // ignored
            }

            return Ok();
        }

        private static List<string> yosaList = new List<string>()
        {
            "Йоса",
            "Йонза",
            "Йондза",
            "Йондзолик",
            "Йонзолик",
            "Йосиф",
            "Йосип",
            "Йося",
            "Йосу",
            "Йосю",
            "Йонз",
            "Йондз",
            "Йос",
        };

        private static List<string> yosaJokeList = new List<string>
        {
            "Йоса педик",
            "Йоса як там тренер?",
            "Педрило шо ти?",
            "Мене взаламали, вірус, Йоса Педик",
            "Йоса петук",
            "Педрило як твоє очко?",
            "ЙОООСА ПЕДДДИК",
            "Шо там по педрилам, Йоса?",
            "Йоса, та ладно ми і так знаємо шо ти педрило",
            "За попередніми даними Йондза педик",
            "АЛО, ПРІЙОМ, ПЕДРИЛО Йос",
            "Була би срака, а Йоса появиться",
            "ТСН(НОВИНИ): В Червонограді згвалтували хлопчика в очко всі думають на Йодзолика",
            "HOW DARE YOU?>>>yosyf pedrilo",
        };
    }
}
