using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotProject.models.Managers.ordinary;
namespace TelegramBotProject.models.Managers.owner
{
    internal class SwitchReqInviteActiveManager:Manager
    {
        RequstInviteManager manager;
        public SwitchReqInviteActiveManager(string run,RequstInviteManager manager) : base(run)
        {
            input_requests = new string[0];
            
            minRightLevel = 2;
            this.manager = manager;
        }
        protected override string? CheckField(int step, Message message) => null;
        protected override void EndManage(ITelegramBotClient botClient, Update update)
        {
            manager.active = !manager.active;
            string state = manager.active ? "включена" : "отключена";
            succesfull_message = $"Команда /besuperuser {state}";
            base.EndManage(botClient, update);
        }
    }
}
