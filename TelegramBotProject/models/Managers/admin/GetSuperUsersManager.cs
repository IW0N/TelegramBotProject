using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotProject.models.contexts;

namespace TelegramBotProject.models.Managers.admin
{
    internal class GetSuperUsersManager:Manager
    {
        public GetSuperUsersManager(string run):base(run)
        {
            input_requests = new string[0];
            succesfull_message ="Супер-пользователи\n";
            minRightLevel = 0;
        }
        protected override string? CheckField(int step, Message message) => null;
        protected override void EndManage(ITelegramBotClient botClient, Update update)
        {
           
            using (PostDbContext db=new())
            {
                var super_users = db.special_users.ToList();
                foreach (var super_user in super_users)
                {
                    succesfull_message += $"id:{super_user.Id} статус:{MessageProccesor.GetRoleName(super_user.rightLevel)}\n";
                }
            }
            base.EndManage(botClient, update);
            succesfull_message = "";
        }
    }
}
