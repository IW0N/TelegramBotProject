using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.SqlServer;
using TelegramBotProject.models.Entities;
namespace TelegramBotProject.models.contexts
{
   
    internal class InviteDbContext:DbContext
    {
        public DbSet<Ticket> tickets => Set<Ticket>();
        string connectionString;
        public InviteDbContext(string db_name)
        {
            
            
            Database.EnsureCreated();
        }
        public InviteDbContext()
        {
            connectionString = Program.botOptions.InvitesDbConnectionString;

            Database.EnsureCreated();
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticket>().HasIndex(t => t.Id).IsUnique();
            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {

            builder.UseSqlServer(connectionString);
        }
        public static bool ExistsTicket(int recoveryId, InviteDbContext inviteDb)
        {
            foreach (var ticket in inviteDb.tickets)
            {
                if (ticket.Id == recoveryId)
                    return true;
            }
            return false;
        }
    }
}
