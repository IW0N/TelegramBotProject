using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotProject.models.Entities;
using System.Globalization;
using TelegramBotProject.models.contexts;
namespace TelegramBotProject.models.Managers.admin
{
    using static UserChecker;
    using static MessageProccesor;
    class BanSuperUserManager:Manager
    {
        public BanSuperUserManager(string run_command) :
            base(run_command)
        {
            input_requests = new string[] { "Id пользователя","Причина" ,"Вы уверены?(да/нет)" };
         
            cancelation_message = "Удаление супер-пользователя отменено";
            
           
            minRightLevel = 1;
        }
        protected override string? CheckField(int step, Message message) 
        {
            string self_rightNotEnough = "Недостаточно прав для удаления!";
            if (step == 1)
                return CheckId(message.Text)??CheckAdminRights(message, self_rightNotEnough);   
            return null;
        }
        protected async override void EndManage(ITelegramBotClient botClient, Update update)
        {
            if (post_fields[2].ToLower() == "да")
            {
                using (var post_db = PostDbContext.GetContext())
                {
                    var superUser = post_db.special_users.Find(int.Parse(post_fields[0]));
                    var bannedSuperUser = BannedSpecialUser.ToBanned(superUser, post_db);
                    var me = post_db.GetUser(update.Message.Chat.Id);
                    succesfull_message = "Супер-пользователь стал обычным пользователем!\n";
                    string recoveryMessage = $"Код восстановления, если бан был выдан по ошибке <code>{bannedSuperUser.RecoveryId}</code>. \nДействует в течении 24 часов.";
                    succesfull_message += recoveryMessage;
                    notificationOptions = new()
                    {
                        SenderId = PostDbContext.GetFakeId(update.Message.Chat.Id),
                        TargetId = superUser.Id,
                        MessageToOther = $"{GetRoleName(superUser.rightLevel)} {superUser.Id} забанен\nПричина\n{post_fields[1]}\n" + recoveryMessage,
                        MessageToTarget = $"Вы исключены из списка супер-пользователей\nПричина\n{post_fields[1]}",
                        MinRightLevel = 1
                    };
                    base.EndManage(botClient, update);
                    post_db.special_users.Remove(superUser);
                    post_db.bannedSpecialUsers.Add(bannedSuperUser);
                    post_db.SaveChanges();

                }

            }
            else
            {
                string str = "";
                using (PostDbContext db=new())
                {
                    int id = int.Parse(post_fields[0]);
                    var specUser=db.special_users.Find(id);
                    string roleName = GetRoleName(specUser.rightLevel);
                    str += $"Блокировка {roleName}а <b>{id}</b>отменена";
                }
                    cancelation_message = str;
                Cancel(botClient, update);
            }
            
            
        }
    }
}
