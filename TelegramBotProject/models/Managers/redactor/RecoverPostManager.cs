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
    internal class RecoverPostManager:Manager
    {
        RemovedPost? recoveringPost=null;
        public RecoverPostManager(string run) : base(run)
        {
            input_requests = new string[1] {"Введите название статьи, которую вы хотите восстановить"};
            cancelation_message = "Восстановление статьи отменено!";
            succesfull_message = "Статья восстановлена!";
            minRightLevel = 0;
        }
        protected override string? CheckField(int step, Message message)
        {
            if (step == 1)
            {
                using (PostDbContext db=new())
                {
                    foreach (RemovedPost rmPost in db.removedPosts)
                    {
                        if (rmPost.Name == post_fields[0])
                            recoveringPost = rmPost;
                    }
                }
                if (recoveringPost == null)
                    return $"Удаленной статьи с именем {post_fields[0]} не существует";
            }
            return null;
        }
        protected override void EndManage(ITelegramBotClient botClient, Update update)
        {
            using (PostDbContext db=new())
            {
                db.removedPosts.Remove(recoveringPost);
                db.posts.Add(recoveringPost);
                db.SaveChanges();
                
            }
            notificationOptions = new()
            {
                MessageToOther=$"Статья {recoveringPost.Name} восстановлена!",
                SenderId=update.GetSpecUserId(),
                MinRightLevel=0,
                TargetId=-1,
                MessageToTarget=""
            };
            base.EndManage(botClient, update);
            recoveringPost = null;
        }
    }
}
