using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotProject.models.contexts;
namespace TelegramBotProject.models.Entities
{
    internal class BannedSpecialUser
    {
        public long specialUser_Identity { get; init; }
        public int rightLevel { get; set; }
        [System.ComponentModel.DataAnnotations.Key]
        public long RecoveryId { get; init; }
        public DateTime BanDate { get; init; }
        public int BannedUserId { get; init; }
        public BannedSpecialUser(long specialUser_Identity, int rightLevel, DateTime banDate, PostDbContext db,int bannedId=0)
        {
            this.specialUser_Identity = specialUser_Identity;
            this.rightLevel = rightLevel;
            long recoveryId = new Random().NextInt64();
            try
            {
                while (db.ExistsBannedUser(recoveryId))
                {
                    recoveryId = new Random().NextInt64();
                }
            }
            catch { }
            this.RecoveryId = recoveryId;
            BanDate = banDate;
            BannedUserId = bannedId;
        }
        public static BannedSpecialUser ToBanned(SpecialUser specUser,PostDbContext db)
        {
            return new BannedSpecialUser(specUser.specialUser_Identity,specUser.rightLevel,DateTime.Now,db,specUser.Id);
        }
        public BannedSpecialUser()
        {

        }
    }
}
