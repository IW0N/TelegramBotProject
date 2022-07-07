using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotProject.models.contexts;
using Microsoft.EntityFrameworkCore;
namespace TelegramBotProject.models.Entities
{
    class Ticket
    {
        public long UserId { get; init; }
        public int Id { get; internal set; }
        public DateTime Date { get; init; }
        public int roleCode { get; init; }
        public Ticket(long UserId, int Id, DateTime Date, int roleCode)
        {
            this.UserId = UserId;
            this.Date = Date;
            this.roleCode = roleCode;
            
            this.Id = Id;
        }
    }
    

}
