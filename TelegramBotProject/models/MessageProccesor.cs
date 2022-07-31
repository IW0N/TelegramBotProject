//#define LOCAL_DEBUG
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotProject.models.contexts;
using TelegramBotProject.models.options;
using Newtonsoft.Json;
using TelegramBotProject.models.Entities;
namespace TelegramBotProject.models
{
    using static Managers.ordinary.GetViewStatManager;
    internal static class MessageProccesor
    {
        public static async Task<bool> LinkIsCorrect(string link)
        {
            var parts=link.Split("https://");
            if (parts.Length==1)
            {
                parts = link.Split("http://");
                if (parts.Length == 1)
                    return false;

            }
            if (parts[1].Split('/')[0] == "telegra.ph")
            {
                
                string post_name = link.Replace("https://telegra.ph/", null);
                string command = "https://api.telegra.ph/getViews/" + post_name;
                var resp = await client.GetAsync(command);
                string str=await resp.Content.ReadAsStringAsync();
                dynamic json = JsonConvert.DeserializeObject(str);
                bool ok = json["ok"];
                return ok;
                //string str = resp.Content.ToString();
            }
            else
                return false;
            
        }
        internal static async void SendMessage(ITelegramBotClient botClient, ChatId chat, string message)
        {
            try
            {
               
                await botClient.SendTextMessageAsync(chat, message, ParseMode.Html,null,true);
            }
            catch { return; }
        }
        public static string GetRoleName(int role_code)
        {

            return role_code switch
            {
                0=>"редактор",
                1=>"администратор",
                2=>"владелец",
                _=>"обычный пользователь"
            };   
        }
        static int GetMyRole(int senderId)
        {
            int myRole;
            using (PostDbContext db = new())
            {
                var spec_user = db.special_users.Find(senderId);
                myRole = spec_user == null ? -1 : spec_user.rightLevel;
            }
            return myRole;
        }
        static void SendMessageWithSendModeSelection(ITelegramBotClient bot, Entities.SpecialUser superUser,NotificationOptions opts,int myRole)
        {
          
            var identity = superUser.ChatId;
            bool isOther = superUser.rightLevel >= opts.MinRightLevel && superUser.Id != opts.SenderId;
            bool isTarget = superUser.Id == opts.TargetId;
            string? message = isTarget ? opts.MessageToTarget : isOther ? opts.MessageToOther : null;
            if (message != null)
                SendMessage(bot, identity, $"Оповещение от {GetRoleName(myRole)} <b>{opts.SenderId}</b>\n" + message);
        }
        public static async Task Notify(ITelegramBotClient bot,NotificationOptions options)=> 
            await Task.Run
            ( 
                () => 
                {
                    var myRole = GetMyRole(options.SenderId);
                    using PostDbContext context = new();
                    foreach (var superUser in context.special_users)
                        SendMessageWithSendModeSelection(bot,superUser, options, myRole);   
                }
            );
        public static async Task<int> GetView(string link)
        {
            string post_name = link.Replace("https://telegra.ph/", null);
            string command = "https://api.telegra.ph/getViews/" + post_name;
            var resp = await client.GetAsync(command);
            string json = await resp.Content.ReadAsStringAsync();
            dynamic obj = JsonConvert.DeserializeObject(json);
            var res = (int)obj["result"]["views"];
            return res;
        }
        
        
    }
}
