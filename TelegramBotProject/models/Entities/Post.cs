using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotProject.models.Entities
{
    
    //public record class Post(string? Name, string? Url, string? Description, int Id = 0);
    public class Post
    {
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
        public int Id { get; internal set; } = 0;
        public int Views { get; internal set; } = 0;
        public Post(string? Name, string? Url, string? Description, int Id = 0)
        {
            this.Name = Name;
            this.Url = Url;
            this.Description = Description;
            this.Id = Id;
        }
        public Post() { }
        public static implicit operator Post(RemovedPost removedPost)
        {
            return new Post()
            {
                Name = removedPost.Name,
                Url=removedPost.Url,
                Description=removedPost.Description,
                Views=removedPost.Views
            };
        }
        
    }

}
