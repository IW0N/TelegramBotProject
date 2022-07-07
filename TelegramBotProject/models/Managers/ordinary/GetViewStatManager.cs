using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Newtonsoft.Json;
using TelegramBotProject.models.contexts;

namespace TelegramBotProject.models.Managers.ordinary
{
    internal class GetViewStatManager : Manager
    {
        public static readonly HttpClient client = new();
        public GetViewStatManager(string run) : base(run)
        {
            minRightLevel = -1;
        }
        protected override string? CheckField(int step, Message message) => null;
        async Task<int> GetView(string link)
        {
            string post_name = link.Replace("https://telegra.ph/", null);
            string command = "https://api.telegra.ph/getViews/" + post_name;
            var resp = await client.GetAsync(command);
            string json = await resp.Content.ReadAsStringAsync();
            dynamic obj = JsonConvert.DeserializeObject(json);
            var res = (int)obj["result"]["views"];
            return res;
        }
        protected async Task<string> GetViews()
        {

            string stat = "";
            using (PostDbContext postDb = new())
            {
                
                foreach (var post in postDb.posts)
                {
                    try
                    {
                        stat += $"\n<i><a href='{post.Url}'>{post.Name}</a></i> {post.Views}\n";
                    }
                    catch { return "Что-то пошло не так :("; }
                }
            }
            return stat;
        }
        protected override async void EndManage(ITelegramBotClient botClient, Update update)
        {
            succesfull_message = await GetViews();
            base.EndManage(botClient, update);
        }
    }

}
