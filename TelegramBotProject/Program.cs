using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types.Enums;
using TelegramBotProject.models.Managers;
using TelegramBotProject.models.Entities;
using TelegramBotProject.models.contexts;
using Microsoft.EntityFrameworkCore;
using TelegramBotProject.models.options;
using static TelegramBotProject.models.MessageProccesor;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotProject.models.modes;
using  TelegramBotProject.models.Managers.ordinary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TelegraphPublisher;

namespace TelegramBotProject
{
    class Program
    {
        public static readonly BotOptions botOptions; 
        public static readonly ITelegramBotClient bot;
        internal static Account account;
        static readonly string config_path = "D:\\AdminInfo\\IT\\Программирование\\C#\\TelegramBotProject\\TelegramBotProject\\configs.json";
        static Program()
        {
            botOptions = new(config_path);
            bot = new TelegramBotClient(botOptions.Token);
            account = Account.GetFromConfig(config_path);
            
        }
        static void HandleMessage(ITelegramBotClient botClient, Update update)
        {
           
            if (update.Type == UpdateType.Message)//если тип обновления-сообщение
            {
                try
                {
                    var message = update.Message;
                    string? command = message.Text;

                    long userId = update.Message.Chat.Id;
                    var running_manager = Manager.GetRunningManagerFor(botClient, update, out CancelationMode canceled);
                    if (running_manager == null)
                    {
                        if (canceled == CancelationMode.No)
                            SendMessage(bot, update.Message.Chat, "Такой команды нет!");
                        else if (canceled == CancelationMode.Accidentally)
                            SendMessage(bot, update.Message.Chat, "Вы не вводите никакие данные!");
                    }
                    else if (canceled == CancelationMode.No)
                        running_manager.Process(botClient, update);

                }
                catch
                {
                    SendMessage(bot,update.Message.Chat,"Что-то пошло не так :(");
                }


            }
        }
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            HandleMessage(botClient, update);
            
        
        }
       
        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(exception);
           // System.IO.File.AppendAllText("logs.txt", DateTime.Now.ToString() + exception.ToString() + '\n');
        }
        static void HandleDeletion<DbContextType,TableType>(EntityDelOptions<DbContextType,TableType> options) 
            where DbContextType:DbContext,new()  where TableType:class
        {
            try
            {
                using (DbContextType dbContext = new())
                {
                    
                    var table = options.GetTable(dbContext);
                    List<TableType> removableRecords = new();
                    foreach (var rem_info in table)
                    {
                        var deltaTime = DateTime.Now - options.GetDateCreating(rem_info);
                        if (deltaTime >= options.TimeConstraint)
                            removableRecords.Add(rem_info);

                    }
                    if (removableRecords.Count>0)
                    {
                        foreach (var removableRecord in removableRecords)
                        {
                            table.Remove(removableRecord);
                            Console.WriteLine(options.BuildAnswer(removableRecord));
                        }
                        dbContext.SaveChanges();
                        
                    }
                }
            }
            catch { }
        }
        static async Task HandleUpdateStatisticsViews()
        {
            await Task.Run(() =>
            {
               
                while (true)
                {
                    //Console.WriteLine("Обновление статистики просмотров...");
                    using (PostDbContext postDb =new())
                    {
                        foreach (var post in postDb.posts)
                        {
                            int view=GetView(post.Url).Result;
                            post.Views = view;
                        }
                        postDb.SaveChanges();
                    }
                    Thread.Sleep(botOptions.ViewCheckDelay);
                }
            });
        }
        static async Task HandleTokenRevoking()
        {
            await Task.Run(async () =>
            {
                var client = Publisher.client;
                const string adress = "https://api.telegra.ph/revokeAccessToken?access_token=";
                while (true)
                {
                    string old_json_str = System.IO.File.ReadAllText(config_path);
                    JObject old_json = (JObject)JsonConvert.DeserializeObject(old_json_str);
                    
                    HttpRequestMessage request = new(HttpMethod.Get, adress + account.access_token);
                    var response = client.Send(request);
                    string jsonStr = await response.Content.ReadAsStringAsync();
                    JObject json = (JObject)JsonConvert.DeserializeObject(jsonStr);
                   
                    Console.WriteLine(json.GetType());
                    string newToken = (string)json["result"]["access_token"];
                    account.access_token = newToken;
                    old_json["account"]["access_token"] = newToken;
                    System.IO.File.WriteAllText(config_path, old_json.ToString());
                    Thread.Sleep(new TimeSpan(2, 0, 0));
                }
            });
        }
        static async Task HandleDeletionFromDbsAsync()
        {
            
            await Task.Run(() =>
            {
                Console.WriteLine("Задача запущена!");
              
                var rmPostsOptions = EntityDelOptions.GetRemovedPostsOptions
                (botOptions.DeletionOldPostsFinallyDelay);
                var rmInvitesOptions = EntityDelOptions.GetInvitesOptions(botOptions.RemoveInvitesDelay);
                var bannedUsersOptions = EntityDelOptions.GetBannedSpecialUserOptions
                (botOptions.BanSuperUsersFinallyDelay);
                
                while (true)
                {
                    HandleDeletion(rmInvitesOptions);
                    HandleDeletion(rmPostsOptions); 
                    HandleDeletion(bannedUsersOptions);
                    Thread.Sleep(60000);
                }
            });
        }
        static async Task Main(string[] args)
        {
            
            Console.WriteLine("Запущен бот " + (await bot.GetMeAsync()).FirstName);

            ManagerConnector.ConnectManagers();
            HandleDeletionFromDbsAsync();
            HandleUpdateStatisticsViews();
            //HandleTokenRevoking();
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new UpdateType[] { UpdateType.Message },

                // receive all update types
            };

            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Thread.Sleep(-1);


            //Console.ReadLine();
        }
    }
}