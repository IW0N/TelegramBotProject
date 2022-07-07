using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotProject.models.Managers.ordinary
{
    internal class GetNSUA_StateManager:Manager
    {
        readonly RequstInviteManager reqManager;
        static readonly string activeOff = "Команда /besuperuser временно отключена. Нам пока хватает как редакторов, так и администраторов. Следите за новостями группы.";
        static readonly string activeOn = "Команда включена в данный момент. Можете подать заявку на администратора бота или на редактора статей.";
        public GetNSUA_StateManager(string run_com,RequstInviteManager reqManager) : base(run_com)
        {
            this.reqManager = reqManager;
            minRightLevel = -1;
        }
        protected override string? CheckField(int step, Message message) => null;
        protected override void EndManage(ITelegramBotClient botClient, Update update)
        {
            string state = reqManager.active?activeOn:activeOff;
            succesfull_message = state;
            base.EndManage(botClient, update);
        }
    }
}
