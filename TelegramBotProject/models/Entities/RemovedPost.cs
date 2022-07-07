using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace TelegramBotProject.models.Entities
{
    public class RemovedPost
    {
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
        public int Id { get; internal set; } = 0;
        public int Views { get; internal set; } = 0;
        
        public DateTime? DeletionTime { get; set; } = null;
        
        public RemovedPost(string? Name, string? Url, string? Description,DateTime dateTime, int Id = 0)
        {
            this.Name = Name;
            this.Url = Url;
            this.Description = Description;
            this.Id = Id;
            DeletionTime = dateTime;
            
        }
       
        
        
        public RemovedPost(Post post,DateTime deletionDate)
        {
            Name = post.Name;
            Description = post.Description;
            Url = post.Url;
            Views = 0;
            DeletionTime = deletionDate;
            Id = 1;
            
        }
        public RemovedPost() { }
        
        
    }
}
