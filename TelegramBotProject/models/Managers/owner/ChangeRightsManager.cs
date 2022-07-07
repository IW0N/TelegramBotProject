using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotProject.models.contexts;
using TelegramBotProject.models.Entities;
using TelegramBotProject.models.Managers.admin;
using TelegramBotProject.models.options;
using static TelegramBotProject.models.MessageProccesor;
namespace TelegramBotProject.models.Managers.owner
{
    class ChangeRightsManager : Manager
    {
        int last_rightLevel = -1;
        int current_right_level = -1;
        public ChangeRightsManager(string run_command) :
            base(run_command)
        {
            input_requests = new string[] { "Id пользователя", "Редактор(0), Администратор(1)" };
            cancelation_message = "Изменение прав супер-пользователя отменено!";
            succesfull_message = "Правовой статус пользователя изменен!";
            minRightLevel = 2;
        }
        protected override string? CheckField(int step, Message message)
        {
            string input_text = message.Text;
            string self_rightNotEnough = "Вы не можете изменить права этого пользователя!";
            if (step == 1)
            {
                string? id_info = UserChecker.CheckId(input_text);
                if (id_info == null)
                {
                    bool exists = PostDbContext.UserExists(int.Parse(input_text));
                    if (!exists)
                        return $"Пользователь с id {input_text} не наделён правами супер-пользователя!";
                    else
                        return UserChecker.CheckAdminRights(message, self_rightNotEnough);
                }
                else
                    return id_info;
            }
            return null;
        }
        protected override async void EndManage(ITelegramBotClient botClient, Update update)
        {
            using (var post_db = PostDbContext.GetContext())
            {
                var superUser = post_db.GetUser(int.Parse(post_fields[0]));
                last_rightLevel = superUser.rightLevel;
                superUser.rightLevel = int.Parse(post_fields[1]);
                current_right_level = superUser.rightLevel;
                post_db.SaveChanges();
            }
            int myFakeId = update.GetSpecUserId();
            string lastRoleName = GetRoleName(last_rightLevel);
            string currentRoleName = GetRoleName(current_right_level);
            string message = $"Пользователь <b>{myFakeId}</b> изменил права <b>{post_fields[0]}</b> с {lastRoleName}а на {currentRoleName}а";
            NotificationOptions options = new()
            {
                SenderId=myFakeId,
                TargetId = int.Parse(post_fields[0]),
                MessageToOther=message,
                MessageToTarget=$"Ваши правы изменены с '{lastRoleName}' на '{currentRoleName}'",
                MinRightLevel=0
            };
            
            Notify(botClient, options);
            
            base.EndManage(botClient, update);
        }

    }
}
