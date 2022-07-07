using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotProject.models.Entities;
using TelegramBotProject.models.contexts;
using static TelegramBotProject.models.MessageProccesor;
using TelegramBotProject.models.options;

namespace TelegramBotProject.models.Managers.admin
{
    class AddingUserManager : Manager
    {
        Ticket? ticket;
        int targetUserId;
        public AddingUserManager(string run) : base(run)
        {
           
            minRightLevel = 1;
            input_requests = new string[] {"Введите id запроса"};
            succesfull_message = "Добавлен новый супер-пользователь!";
            cancelation_message = "Добавление нового пользователя отменено";
        }
        private async Task WriteUserToDbAsync()
        {
            var removeTicketTask = Task.Run(() =>
            {
                using (InviteDbContext inviteDb = new())
                {
                    inviteDb.tickets.Remove(ticket);
                    inviteDb.SaveChanges();
                }
            });
            var appendNewSuperUserTask = Task.Run(() => 
            {
                using PostDbContext db = new();
                var user = new SpecialUser(ticket.UserId, ticket.roleCode);
                db.special_users.Add(user);
                db.SaveChanges();
                targetUserId = user.Id;

            });
            await removeTicketTask;
            await appendNewSuperUserTask;
            
        }
        protected async override void EndManage(ITelegramBotClient botClient, Update update)
        {
            await WriteUserToDbAsync();
            
            var userId = update.Message.Chat.Id;
            int userFakeId = PostDbContext.GetFakeId(userId);
            
            string roleName = GetRoleName(ticket.roleCode);
            string messageToTarget = $"Вы приняты на роль {roleName}а бота\nПосмотрите, какие команды вам доступны, набрав /help";
            notificationOptions = new()
            {
                MessageToOther = $"Принят новый {roleName} с id {targetUserId}",
                MessageToTarget = messageToTarget,
                MinRightLevel = 1,
                SenderId = userFakeId,
                TargetId = targetUserId
                
            };
            ticket = null;
            base.EndManage(botClient, update);
        }
        protected override string? CheckField(int step, Message message)
        {

            if (step == 1)
            {
                
                bool isNum = int.TryParse(post_fields[0],out int inviteId);

                if (isNum)
                {
                   
                    using(var inviteDb=new InviteDbContext())
                        ticket=inviteDb.tickets.Find(inviteId);
                    if (ticket == null)
                        return "Такого id не существует";
                    else
                    {
                        int reqRole=ticket.roleCode;
                        long selfId = message.Chat.Id;
                        SpecialUser? me;
                        try
                        {
                            using (PostDbContext db = new())
                                me = db.GetUser(selfId);
                            if (me == null || me.rightLevel <= reqRole)

                                return $"Недостаточно прав на принятие этого пользователя";

                            else
                                return null;
                        }
                        catch { return "Что-то пошло не так"; }
                    }
                }
                else
                    return "Введённый id не соответствует требованиям";
            }
            return null;
            
        }
    }
}
