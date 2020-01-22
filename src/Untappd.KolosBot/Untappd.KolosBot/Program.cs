﻿using System;
using System.Collections.Generic;
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
            var host = CreateHostBuilder(args).Build();

            //_dbContext = (KolosDbContext)host.Services.GetService(typeof(KolosDbContext));

            var token = Environment.GetEnvironmentVariable("BOT_TOKEN");
            _botClient = new TelegramBotClient(token);
            //var me = _botClient.GetMeAsync().Result;
            _botClient.OnMessage += Bot_OnMessage;
            
            _botClient.StartReceiving();

            host.Run();
            //_botClient.SendTextMessageAsync(new ChatId(352541299), "Daun it's me'");
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            //if (!await _dbContext.Users.AnyAsync(x => x.Id == e.Message.Chat.Id))
            //{
            //    _dbContext.Users.Add(new User {Id = e.Message.Chat.Id});
            //}

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


            if (e.Message.Text == "/pidor")
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

            if (yosaList.Any(x => e.Message.Text.Contains(x)))
            {
                var rand = new Random();
                var randIndex = rand.Next(0, yosaJokeList.Count - 1);
                await _botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: yosaJokeList[randIndex]
                );
            }
            //if (e.Message.Text != null)
            //{
            //    await _botClient.SendTextMessageAsync(
            //        chatId: e.Message.Chat,
            //        text: "You said:\n" + e.Message.Text
            //    );
            //}
        }

        private static List<string> yosaList = new List<string>()
        {
            "Йоса",
            "Йонза",
            "Йондза",
            "Йондзолик",
            "Йонзолик",
            "Йосиф",
            "Йосип"
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
            "Йоса вот скажи ти педик?",
        };
    }
}
