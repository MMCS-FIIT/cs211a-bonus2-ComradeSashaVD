using System.Reflection.Metadata.Ecma335;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;


namespace SimpleTGBot;

using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

public class TelegramBot
{
    // Токен TG-бота. Можно получить у @BotFather
    private const string BotToken = "6849471329:AAH_8gdi5j9YpvKGvqTucEYj-PJEoEDH0NA";
    
    /// <summary>
    /// Инициализирует и обеспечивает работу бота до нажатия клавиши Esc
    /// </summary>
    public async Task Run()
    {
        // Если вам нужно хранить какие-то данные во время работы бота (массив информации, логи бота,
        // историю сообщений для каждого пользователя), то это всё надо инициализировать в этом методе.
        // TODO: Инициализация необходимых полей
        var path = "Цитаты.txt";
        string[] massiveQuote = System.IO.File.ReadAllLines(path);
        // Инициализируем наш клиент, передавая ему токен.
        var botClient = new TelegramBotClient(BotToken);
        
        // Служебные вещи для организации правильной работы с потоками
        using CancellationTokenSource cts = new CancellationTokenSource();
        
        // Разрешённые события, которые будет получать и обрабатывать наш бот.
        // Будем получать только сообщения. При желании можно поработать с другими событиями.
        ReceiverOptions receiverOptions = new ReceiverOptions()
        {
            AllowedUpdates = new [] { UpdateType.Message }
        };

        // Привязываем все обработчики и начинаем принимать сообщения для бота
        botClient.StartReceiving(
            updateHandler: OnMessageReceived,
            pollingErrorHandler: OnErrorOccured,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        // Проверяем что токен верный и получаем информацию о боте
        var me = await botClient.GetMeAsync(cancellationToken: cts.Token);
        Console.WriteLine($"Бот @{me.Username} запущен.\nДля остановки нажмите клавишу Esc...");
        
        // Ждём, пока будет нажата клавиша Esc, тогда завершаем работу бота
        while (Console.ReadKey().Key != ConsoleKey.Escape){}

        // Отправляем запрос для остановки работы клиента.
        cts.Cancel();
    }
    
    /// <summary>
    /// Обработчик события получения сообщения.
    /// </summary>
    /// <param name="botClient">Клиент, который получил сообщение</param>
    /// <param name="update">Событие, произошедшее в чате. Новое сообщение, голос в опросе, исключение из чата и т. д.</param>
    /// <param name="cancellationToken">Служебный токен для работы с многопоточностью</param>
    async Task OnMessageReceived(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Работаем только с сообщениями. Остальные события игнорируем
        var message = update.Message;
        if (message is null)
        {
            return;
        }
        // Будем обрабатывать только текстовые сообщения.
        // При желании можно обрабатывать стикеры, фото, голосовые и т. д.
        //
        // Обратите внимание на использованную конструкцию. Она эквивалентна проверке на null, приведённой выше.
        // Подробнее об этом синтаксисе: https://medium.com/@mattkenefick/snippets-in-c-more-ways-to-check-for-null-4eb735594c09
        if (message.Text is not { } messageText)
        {
            return;
        }

        // Получаем ID чата, в которое пришло сообщение. Полезно, чтобы отличать пользователей друг от друга.
        var chatId = message.Chat.Id;
        
        // Печатаем на консоль факт получения сообщения
        Console.WriteLine($"Получено сообщение в чате {chatId}: '{messageText}'");

        // TODO: Обработка пришедших сообщений

        if ((messageText == "моя цитата") || (messageText == "моя цитата.")  || (messageText == "моя цитата .") || (messageText == "моя цитата!") ||
            (messageText == "моя цитата !") || (messageText == "Моя цитата")  || (messageText == "Моя цитата.")  || (messageText == "Моя цитата .") ||
            (messageText == "Моя цитата!") || (messageText == "Моя цитата !") || 
            (messageText == "Бот, моя цитата!") || (messageText == "Бот, моя цитата") || (messageText == "Бот, моя цитата.") ||
            (messageText == "Бот моя цитата!") || (messageText == "Бот моя цитата") || (messageText == "Бот моя цитата.") ||
            (messageText == "бот, моя цитата!") || (messageText == "бот, моя цитата") || (messageText == "бот, моя цитата.") ||
            (messageText == "бот моя цитата!") || (messageText == "бот моя цитата") || (messageText == "бот моя цитата."))
        {
            var path = "Цитаты.txt";
            string[] massiveQuote = System.IO.File.ReadAllLines(path);
            Random rnd = new Random();
            int index = rnd.Next(0, massiveQuote.Length);
            WriteLine(massiveQuote[index]);


            // Отправляем сообщение с цитатой
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: massiveQuote[index],
                cancellationToken: cancellationToken);
        } 
        else
        {
            // Отправляем сообщение с просьбой о корректном вводе
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Извините, но я Вас не понимаю, пожалуйста, напишите \"бот, моя цитата\", или \"бот моя цитата\", или просто \"моя цитата\", если Вы хотите получить в ответ цитату из аниме \"Врата Штейна\". Спасибо за понимание! ",
                cancellationToken: cancellationToken);
        }
    }

    /// <summary>
    /// Обработчик исключений, возникших при работе бота
    /// </summary>
    /// <param name="botClient">Клиент, для которого возникло исключение</param>
    /// <param name="exception">Возникшее исключение</param>
    /// <param name="cancellationToken">Служебный токен для работы с многопоточностью</param>
    /// <returns></returns>
    Task OnErrorOccured(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        // В зависимости от типа исключения печатаем различные сообщения об ошибке
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        
        // Завершаем работу
        return Task.CompletedTask;
    }
}