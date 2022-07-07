using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotProject.models.contexts;
using TelegramBotProject.models.Entities;

namespace TelegramBotProject.models.Managers.redactor
{
    using static MessageProccesor;
    class EditPostUrlManager : Manager
    {
        public EditPostUrlManager(string run) : base(run)
        {
            input_requests = new string[] { "Введите название статьи", "Новый url" };
            cancelation_message = "Изменение url отменено";
            succesfull_message = "Url изменен!";
            minRightLevel = 0;
        }
        
        protected override string? CheckField(int step, Message message) 
        {
            string input_text = message.Text;
            if (step == 1)
            {
                bool exists = PostDbContext.PostExists(input_text);
                if (!exists)
                    return $"Статьи с названием '{input_text}' не существует!";
               
            }
            else if (step == 2 && !LinkIsCorrect(post_fields[1]).Result)
                return "Ссылка не ведёт на telepgra.ph или такой статьи не существует";

            return null;
        }
        protected override void EndManage(ITelegramBotClient botClient, Update update)
        {
            string lastUrl = "";
            using (var post_db = PostDbContext.GetContext())
            {
                Post? post = post_db.GetPostByName(post_fields[0]);
                lastUrl = post.Url;
                post.Url = post_fields[1];
                post_db.SaveChanges();
            }
            notificationOptions = new()
            {
                MessageToOther = $"Url статьи {post_fields[0]} изменен с <code>{lastUrl}</code> (можно скопировать, нажав на текст) на '{post_fields[1]}'",
                MessageToTarget = "",
                SenderId = update.GetSpecUserId(),
                TargetId = -1,
                MinRightLevel = 0
            };
            base.EndManage(botClient, update);
        }
    }

}
