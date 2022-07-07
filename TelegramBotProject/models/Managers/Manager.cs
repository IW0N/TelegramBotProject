using Telegram.Bot;
using Telegram.Bot.Types;
using static TelegramBotProject.models.MessageProccesor;
using TelegramBotProject.models.contexts;
using TelegramBotProject.models.modes;

namespace TelegramBotProject.models.Managers
{
    abstract class Manager:ManagerOptions
    {
        private static List<Manager> managers=new ();
        protected override int step { get => users_step[current_worker]; set => users_step[current_worker]=value; }
        protected long current_worker  { get; private set; }
        Dictionary<long, int> users_step = new();
        public Manager(string run_command)
        {
            Run_Command = run_command;
            
        }
        public static void AddManager(Manager manager) => managers.Add(manager);
        public static void AddManagers(params Manager[] managers) => Manager.managers = new(managers);
        bool RunningFor(long userId) => users_step.ContainsKey(userId);
        public static Manager? GetRunningManagerFor(ITelegramBotClient botClient, Update update,out CancelationMode canceled)
        {
            string? command = update.Message.Text;
            bool is_run_command,is_cancel_command;
            canceled=CancelationMode.No;
            long userId = update.Message.Chat.Id;
            bool runningForCurrent;
            foreach (Manager manager in managers)
            {
                is_run_command = command == manager.Run_Command;
                is_cancel_command = command == "/cancel";
                runningForCurrent = manager.RunningFor(userId);
                if ((runningForCurrent && !is_cancel_command) || is_run_command)
                {
                    if (is_run_command)
                        manager.users_step.Add(userId, 0);
                    return manager;
                }
                else if (is_cancel_command && runningForCurrent)
                {
                    manager.Cancel(botClient, update);
                    canceled = CancelationMode.Yes;
                    return null;
                }
                else if (is_cancel_command)
                    canceled = CancelationMode.Accidentally;
                
               
            }
            
            return null;
        }
        protected async virtual void Cancel(ITelegramBotClient botClient, Update update,bool error=false)
        {
            users_step.Remove(current_worker);
            if (!error)
               SendMessage(botClient,update.Message.Chat, cancelation_message);
        }
        private bool Authenticate(Chat chat)
        {
            bool auth;
            if (minRightLevel >= 0) 
            {
                using PostDbContext context = new();
                var super_user = context.GetUser(chat.Id);
                auth = super_user != null && super_user.rightLevel >= minRightLevel;
            }
            else
                auth = true;
            return auth;
        }
        protected abstract string? CheckField(int step,Message message);
        protected async void HandleStep(ITelegramBotClient botClient, Update update)
        {
            string? error=null;
            
            Message message = update.Message;
            if (step>0)
            {
                string message_text=message.Text;
                post_fields[step - 1] = message_text;
                error=CheckField(step, message);
            }
            string output = error ?? input_requests[step];
            SendMessage(botClient,message.Chat, output);
            if(error!=null)
                Cancel(botClient, update, true);

        }
        private void HandleIfReqListIsVoid(ITelegramBotClient botClient, Update update, string? error)
        {
            if (error == null)
                EndManage(botClient, update);
            else
            {
                SendMessage(botClient, update.Message.Chat, error);
                Cancel(botClient, update, true);
            }
        }
        private void HandleIfReqListIsNormal(ITelegramBotClient botClient, Update update,string? error)
        {
            if (error == null)
            {
                HandleStep(botClient, update);
                current_worker = update.Message.Chat.Id;
                step++;
            }
            else
            {
                SendMessage(botClient, update.Message.Chat, error);
                Cancel(botClient, update, true);
            }

        }
        private async void ManageWithAuthenticate(ITelegramBotClient botClient, Update update)
        { 
            bool authenticated = Authenticate(update.Message.Chat);
            if (authenticated)
            {
                string? error = CheckField(step, update.Message);
                if (input_requests!=null&&input_requests.Length >0)
                    HandleIfReqListIsNormal(botClient, update, error);
                else
                    HandleIfReqListIsVoid(botClient, update, error);
            }
            else
            {
                SendMessage(botClient, update.Message.Chat, "У вас нет прав на это действие!!!");
                Cancel(botClient, update, true);
            }
         
        }
        
        protected virtual async void EndManage(ITelegramBotClient botClient, Update update)
        {
            
            if (notificationOptions != null)
                Notify(botClient, notificationOptions.Value);
            SendMessage(botClient,update.Message.Chat,succesfull_message);
            users_step.Remove(current_worker);
        }
        
        public async void Process(ITelegramBotClient botClient,Update update)
        {
            current_worker = update.Message.Chat.Id;
            if (!users_step.ContainsKey(current_worker))
                users_step.Add(current_worker, 0);
            if (step==0)
                ManageWithAuthenticate(botClient, update);
            else
            {
                if (step < input_requests.Length)
                {
                    HandleStep(botClient, update);
                    if(users_step.ContainsKey(current_worker))
                        step++;
                }
                else
                {
                    post_fields[step - 1] = update.Message.Text;
                    string? error = CheckField(step, update.Message);
                    
                    if (error == null)
                        EndManage(botClient, update);
                    else
                    {
                        SendMessage(botClient,update.Message.Chat,error);
                        Cancel(botClient, update, true);
                    }
                }
                
            }
            

        }
        
    }

}
