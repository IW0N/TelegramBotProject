using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotProject.models.options;

namespace TelegramBotProject.models.Managers
{
    internal abstract class ManagerOptions
    {
        protected int minRightLevel;
        string[] _input_requests;
        protected string[] input_requests
        {
            get => _input_requests;
            init
            {
                _input_requests = value;
                post_fields = new string[value.Length];
            }
        }
        protected string?[] post_fields { get; private init; }
        protected NotificationOptions? notificationOptions { get; set; }
        protected string Run_Command { get; init; }
        protected string? succesfull_message;
        protected string? cancelation_message;
        
        protected abstract int step { get; set; }
        protected bool mustNotify = false;
       
    }
}
