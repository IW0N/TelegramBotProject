using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotProject.models.Managers.ordinary
{
    using static MessageProccesor;
    using models.contexts;
    internal class MenuBuilderManager:Manager
    {
        public MenuBuilderManager(string run) : base(run)
        {
            cancelation_message = "Отменено";
            minRightLevel = -1;
        }
        protected override string? CheckField(int step, Message message) => null;
        protected override void EndManage(ITelegramBotClient botClient, Update update)
        {
            BuildMenuMessage(botClient,update);
            base.EndManage(botClient, update);
        }
        static async void BuildMenuMessage(ITelegramBotClient botClient, Update update)
        {

            string menu = "<i>Доступные темы</i>\n";
            await Task.Run(() =>
            {
                using PostDbContext post_db = new();
                foreach (var post in post_db.posts)
                {

                    menu += $"\n<a href='{post.Url}'>{post.Name}</a> {post.Description ?? string.Empty}\n";
                }
            });
            SendMessage(botClient, update.Message.Chat, menu);

        }
    }
}
