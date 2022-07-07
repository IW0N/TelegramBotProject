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
    internal class AboutMeManager:Manager
    {
        static string[] rolesInformation = new string[3];
        static AboutMeManager()
        {
            rolesInformation[0] = System.IO.File.ReadAllText("information/roles/Redactor.txt");
            rolesInformation[1] = System.IO.File.ReadAllText("information/roles/Admin.txt");
            rolesInformation[2] = System.IO.File.ReadAllText("information/roles/Owner.txt");
        }
        public AboutMeManager(string run_com):base(run_com)
        {
            succesfull_message = "";
            minRightLevel = 0;
        }
        protected override string? CheckField(int step, Message message) => null;
        protected override void EndManage(ITelegramBotClient botClient, Update update)
        {
            succesfull_message = GetSpecialUserInfo(update);
            base.EndManage(botClient, update);
            succesfull_message = "";
        }
        
        static string GetSpecialUserInfo(Update update)
        {
            SpecialUser me;
            using (PostDbContext db=new())
            {
                int spec_user_id = update.GetSpecUserId();
                me = db.GetUser(spec_user_id);
            }
           
            string roleInfo = $"\n\n<i>О вашей роли</i>\n\n{rolesInformation[me.rightLevel]}";
            return $"<i>Информация о пользователе</i>\n\nРоль: {GetRoleName(me.rightLevel)}\nId: {me.Id}"+roleInfo;
        }
    }
}
