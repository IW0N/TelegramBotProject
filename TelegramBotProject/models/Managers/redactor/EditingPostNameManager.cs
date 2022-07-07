using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotProject.models.contexts;

namespace TelegramBotProject.models.Managers.redactor
{
    //using static PostChecker;
    class EditingPostNameManager : Manager
    {
        public EditingPostNameManager(string run_com) : base(run_com)
        {
            minRightLevel = 0;
            input_requests = new string[] { "Название статьи", "Новое название" };
            succesfull_message = "Статья успешно переименована!";
            cancelation_message = "Переименование отменено";
        }
        protected override string? CheckField(int step, Message message)
        => PostChecker.Check(message.Text, step);
        protected override void EndManage(ITelegramBotClient botClient, Update update)
        {
            using (var post_db = PostDbContext.GetContext())
            {
                var post = post_db.GetPostByName(post_fields[0]);  
                post.Name = post_fields[1];
                post_db.SaveChanges();

            }
            notificationOptions = new()
            {
                MessageToOther = $"Имя статьи '<code>{post_fields[0]}</code>'(можно скопировать, нажав на текст) изменено на '{post_fields[1]}'",
                MessageToTarget = "",
                SenderId = update.GetSpecUserId(),
                TargetId = -1,
                MinRightLevel=0
            };
            base.EndManage(botClient, update);
        }
    }
}
