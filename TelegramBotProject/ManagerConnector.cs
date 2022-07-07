using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotProject.models.Managers;
using TelegramBotProject.models.Managers.admin;
using TelegramBotProject.models.Managers.redactor;
using TelegramBotProject.models.Managers.ordinary;
using TelegramBotProject.models.Managers.owner;
using Newtonsoft.Json;
namespace TelegramBotProject
{
    static class ManagerConnector
    {
        public static void ConnectManagers()
        {
            var reqManager = new RequstInviteManager("/besuperuser");
            Manager.AddManagers
            (
                //Команды для обычных пользователей
                new MenuBuilderManager("/start"),
                new HelpManager("/help"),
                new GetViewStatManager("/getviews"),
                reqManager,
                new GetNSUA_StateManager("/state_of_besuperuser",reqManager),
                //Команды редакторов

                new AddingPostManager("/createPost"),
                new RemovePostManager("/removePost"),
                new EditDescriptionManager("/editDescription"),
                new EditingPostNameManager("/rename"),
                new EditPostUrlManager("/editUrl"),
                new GetSuperUsersManager("/getSuperList"),
                new ExitManager("/exit"),
                new RecoverPostManager("/recover"),
                new AboutMeManager("/aboutMe"),
                new SendMessageManager("/sendMessage"),
                
                //Команды админов

                new AddingUserManager("/addNewSuperUser"),
                new BanSuperUserManager("/ban"),
                new RecoverSuperUserManager("/recoverSuperUser",reqManager),

                //Команды владельцев бота

                new SwitchReqInviteActiveManager("/switchNSUA",reqManager),
                new ChangeRightsManager("/editRightLevel")


            );
            
        }
    }
}
