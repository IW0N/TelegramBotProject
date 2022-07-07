using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotProject.models.Managers.ordinary
{
    using models.contexts;
    using static MessageProccesor;
    internal class HelpManager:Manager
    {
       
        static string[] rolesInfo = new string[4];
        public HelpManager(string run) : base(run)
        {
            FillRolesInfo();
            cancelation_message = "Отменено!";
            minRightLevel = -1;
        }
        private static void FillRolesInfo()
        {
            string ordinaryComms_info = "\n<b>Блок обычного пользователя</b>\n\n" + System.IO.File.ReadAllText("information/commands/Ordinary.txt") + '\n';
            string redactorComms_info = "\n<b>Блок редактора</b>\n\n" + System.IO.File.ReadAllText("information/commands/Redactor.txt") + '\n';
            string adminComms_info = "\n<b>Блок администратора</b>\n\n" + System.IO.File.ReadAllText("information/commands/Admin.txt") + '\n';
            string ownerComms_info = "\n<b>Блок владельца</b>\n\n" + System.IO.File.ReadAllText("information/commands/Owner.txt") + '\n';
            rolesInfo[0] = ordinaryComms_info;
            rolesInfo[1] = redactorComms_info;
            rolesInfo[2] = adminComms_info;
            rolesInfo[3] = ownerComms_info;
        }
        protected override string? CheckField(int step, Message message) => null;
        static string GethelpfullMessageByRole(int role)
        {
            string str = "";
            int i_max = role + 1;
            for (int i = 0; i <= i_max; i++)
            {
                str += rolesInfo[i];
            }
            return str;
        }
        static int GetMyRole(int senderId)
        {
            int myRole;
            using (PostDbContext db = new())
            {
                var spec_user = db.special_users.Find(senderId);
                myRole = spec_user == null ? -1 : spec_user.rightLevel;
            }
            return myRole;
        }
        string GetCommandsInfo(long self_id)
        {
            int self_right_level;

            using (var context = PostDbContext.GetContext())
            {
                var user = context.GetUser(self_id);
                self_right_level = user == null ? -1 : user.rightLevel;
            }

            string output_message = $"Вы {GetRoleName(self_right_level)}\nВам доступны следущие команды\n";
            output_message += GethelpfullMessageByRole(self_right_level);
            return output_message;
        }
        protected override void EndManage(ITelegramBotClient botClient, Update update)
        {
            succesfull_message = GetCommandsInfo(update.Message.Chat.Id);
            base.EndManage(botClient, update);
        }
    }
}
