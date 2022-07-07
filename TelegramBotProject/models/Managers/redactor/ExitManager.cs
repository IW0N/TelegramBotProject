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
    class ExitManager:Manager
    {
        string control_command = "Да. Я полностью уверен в своём решении";
        int exiterId = 0;
        string exitorRoleName = "";
        public ExitManager(string run) : base(run)
        {

            input_requests = new string[1] {$"Вы уверены?\n/cancel, чтобы отменить.\n Если да, введите '{control_command}' без кавычек" };
            succesfull_message = "Теперь вы обычный пользователь";
            cancelation_message = "Действие отменено";
            minRightLevel = 0;
        }
        protected override string? CheckField(int step, Message message)
        {
            if (step == 1)
                return message.Text == control_command ? null : cancelation_message;
            else
                return null;
        }
        protected void Exit(Chat chat)
        {
            using PostDbContext postDb = new();
            foreach (var spec_user in postDb.special_users)
            {
                Console.WriteLine(spec_user.specialUser_Identity);
                if (spec_user.specialUser_Identity == chat.Id)
                {
                    exiterId = spec_user.Id;
                    exitorRoleName = MessageProccesor.GetRoleName(spec_user.rightLevel);
                    postDb.special_users.Remove(spec_user);
                }
                
            }
            postDb.SaveChanges(); 
        }
        protected override void EndManage(ITelegramBotClient botClient, Update update)
        {
            Exit(update.Message.Chat);
            notificationOptions = new()
            {
                MessageToOther = $"{exitorRoleName} {exiterId} снял с себя полномочия",
                MessageToTarget = "",
                SenderId = exiterId,
                TargetId = -1,
                MinRightLevel = 0
            };
            base.EndManage(botClient, update);
        }

    }
}
