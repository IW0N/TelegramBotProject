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
    using static PostChecker;
    class RemovePostManager : Manager
    {

        public RemovePostManager(string run) : base(run)
        {

            input_requests = new string[] { "Введите имя удаляемой статьи" };
         
            succesfull_message = $"Статья удалена!";
            cancelation_message = "Удаление статьи отменено";
            minRightLevel = 0;
        }

        protected override async void EndManage(ITelegramBotClient botClient, Update update)
        {
            string post_name = post_fields[0];

            using (PostDbContext post_db = new())
            {
                var post = post_db.GetPostByName(post_name);
               
                post_db.posts.Remove(post);
                var rm_post=new Entities.RemovedPost(post, DateTime.Now);
                rm_post.Id = post_db.removedPosts.Count()+1;
                post_db.removedPosts.Add(rm_post);
                post_db.SaveChanges();
            }
            notificationOptions = new()
            {
                MessageToOther = $"Статья '<code>{post_fields[0]}</code>' удалена. Её можно восстановить командой /recover в течении 24 часов",
                MessageToTarget = "",
                SenderId = update.GetSpecUserId(),
                TargetId = -1,
                MinRightLevel = 0
            };
            base.EndManage(botClient, update);
        }
        protected override string? CheckField(int step, Message message) => Check(message.Text, step);
    }
}
