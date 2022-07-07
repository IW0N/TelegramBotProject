using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotProject.models.Entities;
using TelegramBotProject.models.contexts;
using TelegramBotProject.models.Managers.ordinary;
using TelegramBotProject.models.options;
using Telegram.Bot;

namespace TelegramBotProject.models.Managers.admin
{
    using static MessageProccesor;
    internal class RecoverSuperUserManager:Manager
    {
        RequstInviteManager inviteManager;
        public RecoverSuperUserManager(string runCommand,RequstInviteManager reqManager) : base(runCommand)
        {
            input_requests = new string[2] {"Введите код восстановления","Введите сообщение, которое получит пользователь, объясняющее ситуацию"};
            succesfull_message = "Заявка на восстановление успешно отправлена!";
            cancelation_message = "Отправление заявки пользователю на восстановление отменено";
            minRightLevel = 1;
            inviteManager = reqManager;
        }
        NotificationOptions GetOptionsForSendingReqToOwner(int senderId)
        {
            SpecialUser sender;
            using (PostDbContext db=new())
            {
                sender=db.special_users.Find(senderId);
            }
                return new()
                {
                    MessageToOther = $"Запрос на включения команды /besuperuser от <b>{GetRoleName(sender.rightLevel)} {sender.Id}</b>",
                    MinRightLevel = 2,
                    MessageToTarget = "",
                    TargetId = -1,
                    SenderId = senderId
                };
        }
        string? CheckStepZero()
        {
            if (!inviteManager.active)
            {
                long id = current_worker;
                SpecialUser? specUser;
                using (PostDbContext db = new())
                {
                    specUser = db.GetUser(id);
                }
                string adding_message = specUser.rightLevel == 1 ? "\nЗаявка на включение команды отправлена владельцу" : "Включите команду, чтобы вернуть его";
                if (specUser.rightLevel == 1)
                {
                    var notOptions = GetOptionsForSendingReqToOwner(specUser.Id);
                    Notify(Program.bot,notOptions);
                }
                
                return "Команда /besuperuser отключена в данный момент, без неё исключенный супер-пользователь не сможет вернуть свои права." + adding_message;
            }
            else
                return null;
        }
        string? CheckStepOne(Message message)
        {
            bool bannedSpecUserExists = false;

            using (PostDbContext db = new())
            {
                bool recoveryIdCorrected = long.TryParse(post_fields[0], out long recoveryId);
                if (recoveryIdCorrected)
                {
                    BannedSpecialUser? bannedUser=db.bannedSpecialUsers.Find(recoveryId);
                    bannedSpecUserExists = bannedUser != null;
                    if (bannedSpecUserExists)
                    {
                        int rightLevel = bannedUser.rightLevel;
                        var me=db.GetUser(message.Chat.Id);
                        return me.rightLevel < rightLevel ? "Недостаточно прав на восстановление!" :null;
                    }
                }

                else
                    return "Введённый код восстановления не соответствует требованиям!";
            }
            return bannedSpecUserExists ? null : "Такого кода восстановления не существует!";
        }
        protected override void EndManage(ITelegramBotClient botClient, Update update)
        {
            using (PostDbContext db=new())
            {
                long recoveryId = long.Parse(post_fields[0]);
                BannedSpecialUser banned = db.bannedSpecialUsers.Find(recoveryId);
                string addingMessage = post_fields[1]+$"\nЕсли согласны, наберите команду /besuperuser и введите это <code>{post_fields[0]}</code>\n" +
                    $"PS Вы можете просто нажать на код, скопировав его";
                SendMessage(botClient, banned.specialUser_Identity, $"Вам предлагают вернуться на роль {GetRoleName(banned.rightLevel)}а. Извините, что забанили вас по ошибке. Надеюсь, такого больше не повторится. Вот, что об этом пишет отправитель\n" + addingMessage);
            }
                base.EndManage(botClient, update);
        }
        protected override string? CheckField(int step, Message message)
        {
            return step switch
            {
                0=>CheckStepZero(),
                1=>CheckStepOne(message),
                _=>null
            };
        }
    }
}
