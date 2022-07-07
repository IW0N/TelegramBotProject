using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotProject.models.contexts;
using TelegramBotProject.models.Entities;

namespace TelegramBotProject.models.Managers.redactor
{
    using static TelegramBotProject.models.MessageProccesor;
    internal class AddingPostManager : Manager
    {

        public AddingPostManager(string run_command) :
            base(run_command)
        {
            input_requests = new string[] { "Введите название статьи", "Введите описание", "Укажите ссылку на статью" };
            cancelation_message = "Добавление статьи в бота отменено!";
            succesfull_message = "Статья добавлена!";
            
            minRightLevel = 0;
        }

        protected override void EndManage(ITelegramBotClient botClient, Update update)
        {
            notificationOptions = new()
            {
                MessageToOther = $"Добавлена статья <a href='{post_fields[1]}'>{post_fields[0]}</a>",
                SenderId = update.GetSpecUserId(),
                TargetId = -1,
                MessageToTarget = "",
                MinRightLevel = 0
            };
            UpdatePostDb(new Post(post_fields[0], post_fields[2], post_fields[1]));
            
            base.EndManage(botClient, update);
        }
        protected override string? CheckField(int step, Message message)
        {
            if (step == 1)
            {
                bool exists=PostDbContext.PostExists(message.Text);
                if (exists)
                    return "Статья уже существует!";
            }
            else if (step==3&& !LinkIsCorrect(post_fields[2]).Result)
                return "Ссылка не ведёт на https://telegra.ph/ или статьи не существует";
            
            return null;
        }
        static async void UpdatePostDb(Post post)
        {
            await Task.Run(() =>
            {
                using PostDbContext db = new();
                db.posts.Add(post);
                db.SaveChanges();
            });
        }


    }
}
