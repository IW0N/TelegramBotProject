using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotProject.models.Entities
{
    //public record SpecialUser(long specialUser_Identity, int rightLevel, int Id = 1);
    public class SpecialUser
    {
        public long specialUser_Identity { get; init; }
        public int rightLevel { get; set; }
        public int Id { get; init; }
        public SpecialUser(long specialUser_Identity, int rightLevel, int Id = 0)
        {
            this.specialUser_Identity = specialUser_Identity;
            this.rightLevel = rightLevel;
            this.Id = Id;
        }
    }
}
