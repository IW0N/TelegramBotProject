using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Telegram.Bot.Types;
namespace TelegramBotProject.models.Entities
{
    //public record SpecialUser(long specialUser_Identity, int rightLevel, int Id = 1);
    public class SpecialUser
    {
        public long ChatId { get; init; }
        public int rightLevel { get; set; }
        public int Id { get; init; }
        public SpecialUser(long chatId, int rightLevel, int Id = 0)
        {
            ChatId = chatId;
            this.rightLevel = rightLevel;
            this.Id = Id;


        }       
        
    }
}
