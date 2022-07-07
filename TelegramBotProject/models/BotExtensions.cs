using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Extensions.Polling;
using TelegramBotProject.models.contexts;
namespace TelegramBotProject.models
{
    internal static class BotExtensions
    {
        public static int GetSpecUserId(this Update update) => PostDbContext.GetFakeId(update.Message.Chat.Id);
    }
}
