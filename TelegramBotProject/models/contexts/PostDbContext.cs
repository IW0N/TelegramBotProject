using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TelegramBotProject.models.Entities;

namespace TelegramBotProject.models.contexts
{
    using static Program;
    internal class PostDbContext : DbContext
    {
        public DbSet<Post> posts => Set<Post>();
        public DbSet<RemovedPost> removedPosts => Set<RemovedPost>();
        public DbSet<SpecialUser> special_users => Set<SpecialUser>();
        public DbSet<BannedSpecialUser> bannedSpecialUsers => Set<BannedSpecialUser>();
        string connectionString;
        
        public PostDbContext()
        {
            connectionString = botOptions.PostsDbConnectionString;
           
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<RemovedPost>().HasData(new RemovedPost()
            {
                DeletionTime = DateTime.Now,
                Id = 1,
                Name = "no name",
                Description = "no descr",
                Url = "https://google.com",
                Views = 1
            });
            modelBuilder.Entity<BannedSpecialUser>().HasData(new BannedSpecialUser(11111,0,DateTime.Now,this,1));
            modelBuilder.Entity<Post>().HasIndex(p => p.Id).IsUnique();
            modelBuilder.Entity<RemovedPost>().HasIndex(p => p.Id).IsUnique();
            modelBuilder.Entity<SpecialUser>().HasIndex(user => user.Id).IsUnique();
            modelBuilder.Entity<BannedSpecialUser>().HasIndex(user => user.RecoveryId).IsUnique();
            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {

            builder.UseSqlServer(connectionString);
        }
        public Post? GetPostByName(string name)
        {
            foreach (var post in posts)
            {
                if (post.Name == name)
                    return post;

            }
            return null;
        }
        public static PostDbContext GetContext() => new();
        public SpecialUser? GetUser(long user_id)
        {
           
            foreach (var spec_user in special_users)
            {
                if (spec_user.ChatId == user_id)
                    return spec_user;

            }
            return null;
        }
        public SpecialUser? GetUser(int id) => special_users.Find(id);
        
       
        public static bool PostExists(string post_name)
        {
            bool exists;
            using (var db = GetContext())
                exists = db.GetPostByName(post_name) != null;
            return exists;
        }
        public static bool UserExists(int id)
        {
            bool exists;
            using (var db = GetContext())
                exists = db.special_users.Find(id) != null;
            return exists;
        }
        public static int GetFakeId(long truthId)
        {
            int fakeId = -1;
            using (var db = GetContext())
            {
                var user = db.GetUser(truthId);
                if (user != null)
                    fakeId = user.Id;
            }
            return fakeId;
        }
        public bool ExistsBannedUser(long recoveryId)
        {
            foreach (var bannedUser in bannedSpecialUsers)
            {
                if (bannedUser.RecoveryId == recoveryId)
                    return true;
            }
            return false;
        }
    }
}
