using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotProject.models.contexts;
using TelegramBotProject.models.Entities;
namespace TelegramBotProject.models.Managers.ordinary
{
    using static MessageProccesor;
    internal class RequstInviteManager:Manager
    {
        public bool active = true;
        BannedSpecialUser? banned;
        string oldSuccefullmsg;
        public RequstInviteManager(string run) : base(run)
        {
            input_requests = new string[] {"Введите код требуемой роли или приглашения\n0-Редактор, 1-Администратор"};
            succesfull_message = "Ваш запрос отправлен. Ожидайте ответа";
            cancelation_message = "Запрос на роль супер-пользователя отменён";
            minRightLevel = -1;   
        }
        protected override string? CheckField(int step, Message message)
        {
            if (step == 0)
            {
                if (!active)
                    return "Команда временно отключена. Нам пока хватает как редакторов, так и администраторов. Следите за новостями группы.";
                else
                {
                    bool isSuperUser = false;
                    using (PostDbContext postDb = new())
                        isSuperUser = postDb.GetUser(message.Chat.Id) != null;
                    if (isSuperUser)
                        return "Вы уже являетесь супер-пользователем!";
                    else
                        return null;
                }
            }
            else if (step == 1)
            {
                string textRole = message.Text;
                bool isNumber = long.TryParse(textRole,out long inviteCode);
                if (isNumber && inviteCode > 2)
                {
                    
                    using (PostDbContext db = new())
                        banned=db.bannedSpecialUsers.Find(inviteCode);
                    
                    return banned == null ?"Такого кода восстановления не существует!":null;
                }
                return textRole == "0" || textRole == "1" ? null : "Неверное значение. Введите <b>0</b>, чтобы отправить запрос на <b>редактора</b>, <b>1</b>-на роль <b>администратора</b>";
            }
            return null;
        }
        void AnswerOffer(ITelegramBotClient botClient, Update update)
        {
            SpecialUser specUser;
            using (PostDbContext db=new())
            {
                db.bannedSpecialUsers.Remove(banned);
                specUser = new SpecialUser(banned.ChatId, banned.rightLevel);
                db.special_users.Add(specUser);
                db.SaveChanges();
            }
            oldSuccefullmsg = succesfull_message;
            string roleName =$"{GetRoleName(specUser.rightLevel)}а";
            succesfull_message = $"Мои поздравления! Вы восстановлены в роли {roleName}!";
            notificationOptions = new()
            {
                MessageToOther = $"Пользователь со старым id <b>{banned.BannedUserId}</b> восстановлен в роли {roleName}!",
                MessageToTarget="",
                MinRightLevel=0,
                SenderId=specUser.Id,
                TargetId=-1
            };
            
        }
        void SendNewSuperUserOffer(ITelegramBotClient botClient, Update update)
        {
            var role_code = int.Parse(post_fields[0]);
            int inviteCode = new Random().Next(3, int.MaxValue);
            string role_name = MessageProccesor.GetRoleName(role_code);
            string message = $"Пользователь с ником {update.Message.From.FirstName} желает стать {role_name}ом\n invite_id <code>{inviteCode}</code> (нажмите на код, чтобы скопировать). Id будет действительным в течении 15 часов";
            long chatId = update.Message.Chat.Id;

            using (InviteDbContext db = new())
            {
                var ticket = new Entities.Ticket(chatId, inviteCode, DateTime.Now, role_code);
                bool exitsId = true;
                while (exitsId)
                {
                    foreach (var ticket_db in db.tickets)
                    {
                        if (ticket_db.Id == ticket.Id)
                        {
                            int id = ticket.Id;
                            while (id == ticket_db.Id)
                            {
                                id = new Random().Next(3, int.MaxValue);
                            }
                            ticket.Id = id;
                            exitsId = false;
                        }
                    }
                    exitsId = false;
                }
                db.tickets.Add(ticket);
                db.SaveChanges();
            }

            notificationOptions = new()
            {
                SenderId = update.GetSpecUserId(),
                TargetId = -1,
                MessageToOther = message,
                MessageToTarget = "",
                MinRightLevel = 1
            };
        }
        protected override async void EndManage(ITelegramBotClient botClient, Update update)
        {
            if (banned==null)
                SendNewSuperUserOffer(botClient, update);
            else
                AnswerOffer(botClient, update);
            base.EndManage(botClient, update);
            if (banned != null)
            {
                succesfull_message = oldSuccefullmsg;
                banned = null;
            }
        }
    }
}
