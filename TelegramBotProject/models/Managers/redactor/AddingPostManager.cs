
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotProject.models.contexts;
using TelegramBotProject.models.Entities;
using WordConverter2.NodeEnvironment;
using TelegraphPublisher;
using TelegramBotProject.models.options;

namespace TelegramBotProject.models.Managers.redactor
{
    internal class AddingPostManager : Manager
    {
        Stream doc_stream;
        string PostName { get => post_fields[0]; }
        string PostDescription { get => post_fields[1]; }
        string postLink;
        Account senderAccount { get => Program.account; }
        public AddingPostManager(string run_command) :
            base(run_command)
        {
            input_requests = new string[] { "Введите название статьи", "Введите описание", "Прикрепите docx-файл с текстом статьи" };
            cancelation_message = "Добавление статьи в бота отменено!";
            succesfull_message = "Статья добавлена!";
            
            minRightLevel = 0;
        }
       
        protected override void EndManage(ITelegramBotClient botClient, Update update)
        {
            notificationOptions = new()
            {
                MessageToOther = $"Добавлена статья <a href='{postLink}'>{PostName}</a>",
                SenderId = update.GetSpecUserId(),
                TargetId = -1,
                MessageToTarget = "",
                MinRightLevel = 0
            };
            Node node=Node.BuildFromDocx(doc_stream);
            postLink=Publisher.Publish(node, PostName,senderAccount).Result;
      
            UpdatePostDb(new Post(PostName, postLink,"no description" ));
            doc_stream.Dispose();
            base.EndManage(botClient, update);
        }
        static Stream GetDocumentStream(string docId)
        {
            var file=Program.bot.GetFileAsync(docId).Result;
            var botOptions = Program.botOptions;
            string path = file.FilePath;
            string fileUrl="https://api.telegram.org/file/bot"+botOptions.Token+'/'+path;
            var client = Publisher.client;
            var bytes= client.GetByteArrayAsync(fileUrl).Result;
            MemoryStream stream = new(bytes);
            return stream;
        }
        protected override string? CheckField(int step, Message message)
        {
            if (step == 1)
            {
                bool exists = PostDbContext.PostExists(message.Text);
                if (exists)
                    return "Статья уже существует!";
            }

            else if (step == 3)
            {
                var doc = message.Document;
                if(doc==null)
                    return "Ссылка не ведёт на https://telegra.ph/ или статьи не существует";
                else
                {
                    const string acceptableFormat = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    string fileType = doc.MimeType;
                   
                    if (fileType != acceptableFormat)
                        return "Загружен файл недопустимого формата! Вы можете использовать только файлы с типа docx";
                    doc_stream = GetDocumentStream(message.Document.FileId);
                    
                }
                
            }
            
            return null;
        }
        static async void UpdatePostDb(Post post)
        {
            await Task.Run(() =>
            {
                using PostDbContext db = new();
                db.posts.Add(post);
                db.SaveChanges();
            });
        }


    }
}
