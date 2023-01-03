using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace TelegramBotProject.models.options
{
    internal struct BotOptions
    {
        public string Token { get; init; }
        public string InvitesDbConnectionString { get; init; }
        public string PostsDbConnectionString { get; init; }
        public TimeSpan DeletionOldPostsFinallyDelay { get; init; }
        public TimeSpan BanSuperUsersFinallyDelay { get; init; }
        public TimeSpan RemoveInvitesDelay { get; init; }
        public TimeSpan ViewCheckDelay { get; init; }
        public BotOptions(string bot_configs_path)
        {
            string json_text =  File.ReadAllText(bot_configs_path);
            dynamic json = JsonConvert.DeserializeObject(json_text);
            dynamic bot = json["bot"];
            Token = (string)bot["token"];
            InvitesDbConnectionString = (string)bot["invites_db_connection_string"];
            PostsDbConnectionString = (string)bot["posts_db_connection_string"];
            DeletionOldPostsFinallyDelay = TimeSpan.Parse((string)bot["deletion_old_posts_finally_delay"]);
            BanSuperUsersFinallyDelay = TimeSpan.Parse((string)bot["ban_super_users_finally_delay"]);
            RemoveInvitesDelay = TimeSpan.Parse((string)bot["remove_invites_delay"]);
            ViewCheckDelay = TimeSpan.Parse((string)bot["view_check_delay"]);
        }
    }
}
