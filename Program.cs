using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Exceptions;
using System.IO;

class Program
{
    private static TelegramBotClient botClient;
    private static int clickCount = 0;

    private static long? chatId;
    private static int? messageId;
    private static System.Timers.Timer? refreshTimer;

    static async Task Main()
    {
        var token = "7883919198:AAFffl68ZWNRbehGPAM3HCtrZjGXx98gBTY"; // Replace this with your bot token
        botClient = new TelegramBotClient(token);

        var me = await botClient.GetMeAsync();
        Console.WriteLine($"Bot @{me.Username} is running...");

        using var cts = new CancellationTokenSource();

        botClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() },
            cts.Token
        );

        LoadClickCount();
        Console.WriteLine("Loaded click_count.txt");

        Console.WriteLine("Press Enter to exit");
        Console.ReadLine();

        
        cts.Cancel();
    }

    static async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken token)
    {
        try
        {
            if (update.Type == UpdateType.Message && update.Message?.Text == "/start")
            {
                var chatId = update.Message.Chat.Id;

                Console.WriteLine("Received /start command.");

                var markup = new InlineKeyboardMarkup(
                    InlineKeyboardButton.WithCallbackData("Ай Колінька маладца, під'єбав Настусю) ❤️", "click")
                );

                await bot.SendTextMessageAsync(
                    chatId,
                    $"Тігр лев, підйоби Настінькі): {clickCount} разів",
                    replyMarkup: markup,
                    cancellationToken: token
                );
            }
            else if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery?.Data == "click")
            {
                var callbackQuery = update.CallbackQuery;
                var chatId = callbackQuery.Message.Chat.Id;
                var messageId = callbackQuery.Message.MessageId;

                // Always respond to the callback query immediately
                await bot.AnswerCallbackQueryAsync(
                    callbackQuery.Id,
                    text: "Ти ж моя умнічка!",
                    cancellationToken: token
                );

                clickCount++;
                SaveClickCount();
                Console.WriteLine("Saved click_count.txt");
                Console.WriteLine($"Button clicked {clickCount} times.");

                await bot.EditMessageTextAsync(
                    chatId,
                    messageId,
                    $"Тігр лев, підйоби Настінькі): {clickCount} разів",
                    replyMarkup: new InlineKeyboardMarkup(
                        InlineKeyboardButton.WithCallbackData("Ай Колінька маладца, під'єбав Настусю) ❤️", "click")
                    ),
                    cancellationToken: token
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception in HandleUpdateAsync: " + ex.Message);
        }
    }

    static Task HandleErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken token)
    {
        Console.WriteLine("Bot error: " + ex.Message);
        return Task.CompletedTask;
    }

    const string SaveFile = "click_count.txt";

    static void SaveClickCount()
    {
        System.IO.File.WriteAllText(SaveFile, clickCount.ToString());
    }

    static void LoadClickCount()
    {
        if (System.IO.File.Exists(SaveFile))
        {
            var content = System.IO.File.ReadAllText(SaveFile);
            if (int.TryParse(content, out int count))
            {
                clickCount = count;
            }
        }
    }

}
