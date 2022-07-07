using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotProject.models.contexts;

namespace TelegramBotProject.models.Managers.redactor
{
    static class PostChecker
    {
        public static string? Check(string input_text,int step)
        {
           
            if (step == 1)
            {
                bool exists = PostDbContext.PostExists(input_text);
                if (!exists)
                    return $"Статьи с названием '{input_text}' не существует!";
            }
            else if (step == 2)
            {
                bool exists = PostDbContext.PostExists(input_text);
                if (exists)
                    return $"Статья с названием '{input_text}' уже существует!";
            }
            return null;
        }
    }
}
