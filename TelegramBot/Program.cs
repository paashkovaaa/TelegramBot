using Telegram.Bot;
using Telegram.Bot.Polling;


namespace BookShelfBot
{
    public class ProgramBot
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Запущен бот " + TelegramBot._bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            TelegramBot._bot.StartReceiving(
                TelegramBot.HandleUpdateAsync,
                TelegramBot.HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );

            Console.ReadLine();
        }

    }
}