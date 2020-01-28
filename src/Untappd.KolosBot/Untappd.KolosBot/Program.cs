using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace Untappd.KolosBot
{
    public class Program
    {
        private static ITelegramBotClient _botClient;
        public static async Task Main(string[] args)
        {
            try
            {
                var token = Environment.GetEnvironmentVariable("BOT_TOKEN");
                _botClient = new TelegramBotClient(token);

                var host = CreateHostBuilder(args).Build();
                //TODO move to startup
                //var webhook = await _botClient.GetWebhookInfoAsync();
                await _botClient.SetWebhookAsync($"https://zkolos-bot.herokuapp.com/webhook");
                

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
                    webBuilder.UseStartup<Startup>().ConfigureServices(services =>
                            {
                                services.AddSingleton(_botClient);
                            })
                    .UseUrls("http://*:" + port);
                });
        }


    }
}
