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
    class EditDescriptionManager : Manager
    {
        public EditDescriptionManager(string run) : base(run)
        {
            input_requests = new string[] { "Введите название статьи", "Новое описание" };
            cancelation_message = "Изменение статьи отменено";
            succesfull_message = "Описание статьи изменено!";
            
            minRightLevel = 0;
        }
        protected async override void EndManage(ITelegramBotClient botClient, Update update)
        {
            
            string last_descr = "";
            await Task.Run(() =>
            {
                using var post_db = PostDbContext.GetContext();

                var post = post_db.GetPostByName(post_fields[0]);
                last_descr = post.Description;
                post.Description = post_fields[1];
                post_db.SaveChanges();
            });
            notificationOptions = new()
            {
                MessageToOther = $"Изменено описание статьи {post_fields[0]} с '<code>{last_descr}</code>' (можно скопировать,нажав на текст) на {post_fields[1]}",
                MessageToTarget = "",
                SenderId = update.GetSpecUserId()
            };
            base.EndManage(botClient, update);
        }
        protected override string? CheckField(int step, Message message)
        => PostChecker.Check(message.Text, step);

    }
}
