using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Untappd.KolosBot.Infrastucture;

namespace Untappd.KolosBot
{
    public class Program
    {
        private static ITelegramBotClient _botClient;
        private static KolosDbContext _dbContext;

        public static void Main(string[] args)
        {
            try
            {
                var host = CreateHostBuilder(args).Build();
                var token =  Environment.GetEnvironmentVariable("BOT_TOKEN");
                _botClient = new TelegramBotClient(token);
                _botClient.OnMessage += Bot_OnMessage;
                _botClient.StartReceiving();

                host.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);

                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var port = Environment.GetEnvironmentVariable("PORT");

            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                    .UseUrls("http://*:" + port);
                });
        }

        static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            try
            {
                if (e.Message.Text == "/pio")
                {
                    var parser = new UntappdParser();
                    var beers = await parser.GetBeers();

                    foreach (var beer in beers)
                    {
                        await _botClient.SendPhotoAsync(e.Message.Chat.Id, new InputOnlineFile(new Uri(beer.ImageUrl)));
                        await _botClient.SendTextMessageAsync(
                            chatId: e.Message.Chat,
                            text: beer.ToString(),
                            ParseMode.Markdown
                        );
                    }
                }


                if (e.Message.Type == MessageType.Text && e.Message.Text.Contains("/pidor"))
                {
                    await Task.Delay(5000);
                    await _botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: "Оййй та бля пацани"
                    );
                    await _botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: "І так панятно що"
                    );
                    await _botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: "ЙОНЗА ПЕДРИЛО"
                    );
                }

                if (e.Message.Text != null && yosaList.Any(x => e.Message.Text.Contains(x)))
                {
                    var rand = new Random();
                    var randIndex = rand.Next(0, yosaJokeList.Count - 1);
                    await _botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: yosaJokeList[randIndex]
                    );
                }
            }
            catch (Exception exception)
            {
                // ignored
            }
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
            "Йося"
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
