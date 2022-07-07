using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using static TelegramBotProject.models.contexts.PostDbContext;
namespace TelegramBotProject.models.Managers.admin
{
    static class UserChecker
    {
        public static string? CheckId(string id_str)
        {
            bool id_isCorrect = int.TryParse(id_str, out int id), auth;
            if (id_isCorrect)
            {
                bool user_exists=UserExists(id);
                return user_exists ? null : $"Пользователь с id {id} не является супер-пользователем";
            }
            else
                return "Некорректный id";
        }
        public static string? CheckAdminRights(Message message,string failed_message)
        {
            long sender_id=message.From.Id;
            int other_id = int.Parse(message.Text);
            
            bool rights_enough;
            
            using(var post_db=GetContext())
            {
                var admin = post_db.GetUser(sender_id);
                var otherSuperUser = post_db.special_users.Find(other_id);
                rights_enough = admin.rightLevel>otherSuperUser.rightLevel-1;
            }
            return rights_enough ? null : failed_message;
        }
    }
}
