using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotProject.models.modes;

namespace TelegramBotProject.models
{
    internal class MessageArgs
    {
        public long Reciever { get; init; } 
        public MediaMode? Media_mode { get; set; }
    }
}
