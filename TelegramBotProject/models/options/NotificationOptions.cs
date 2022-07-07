using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotProject.models.options
{
    internal record struct NotificationOptions(string MessageToOther, string MessageToTarget, int TargetId, int MinRightLevel, int SenderId)
    {
        public (string messageToOther,string messageToTarget,int targetId,int minRightLevel,int senderId) GetVaribles()
            =>(MessageToOther,MessageToTarget,TargetId,MinRightLevel,SenderId);
        
    }
}
