using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TelegramBotProject.models.contexts;
using TelegramBotProject.models.Entities;
namespace TelegramBotProject.models.options
{
    internal struct EntityDelOptions
    {
        public static EntityDelOptions<PostDbContext, RemovedPost> GetRemovedPostsOptions(TimeSpan deletionDelay)
        {
            return new()
            {
                GetTable = (db) => db.removedPosts,
                GetDateCreating = (info) => info.DeletionTime.Value,
                BuildAnswer = (info) => $"Статья {info.Name} удалена окончательно!",
                TimeConstraint = deletionDelay

            };
        }
        public static EntityDelOptions<InviteDbContext, Ticket> GetInvitesOptions(TimeSpan deletionDelay)
        {
            return new()
            {
                GetTable = (db) => db.tickets,
                GetDateCreating = (info) => info.Date,
                BuildAnswer = (info) => $"Запрос на статус супер-пользователя {info.Id} удален окончательно!",
                TimeConstraint = deletionDelay
            };
        }
        public static EntityDelOptions<PostDbContext, BannedSpecialUser> GetBannedSpecialUserOptions(TimeSpan deletionDelay)
        {
            return new()
            {
                GetTable = (db) => db.bannedSpecialUsers,
                GetDateCreating = (table) => table.BanDate,
                BuildAnswer = (entity) => $"Ещё один пользователь удалён окончательно!",
                TimeConstraint=deletionDelay
            };
        }
    }
    internal struct EntityDelOptions<DbContextType,TableType> where DbContextType:DbContext,new() where TableType:class
    {
        public Func<DbContextType, DbSet<TableType>> GetTable { get; init; }
        public Func<TableType, DateTime> GetDateCreating { get; init; }
        public Func<TableType, string> BuildAnswer { get; init; }
        public  TimeSpan TimeConstraint { get; init; }
        

    }
}
