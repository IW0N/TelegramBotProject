using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotProject.models.contexts;
using Telegram.Bot.Types.InputFiles;
namespace TelegramBotProject.models.Managers.redactor
{
    using static MessageProccesor;
    using modes;
    using static Managers.ordinary.GetViewStatManager;
    internal class SendMessageManager:Manager
    {
        Dictionary<long, MessageArgs> corresponders = new();
        MessageArgs CurrentArgs { get => corresponders[current_worker]; }
        
        public SendMessageManager(string run_com) : base(run_com)
        {
            input_requests = new string[2] {"Id супер-пользователя","Сообщение"};
            succesfull_message = "Сообщение отправлено!";
            cancelation_message = "Отправка отменена";
            minRightLevel = 0;
        }
        protected override string? CheckField(int step, Message message)
        {
            
            if (step == 1)
            {
                bool idCorrected = int.TryParse(post_fields[0],out int specUserId);
                if (idCorrected)
                {
                    bool specUserExists = PostDbContext.UserExists(specUserId);
                    if (!specUserExists)
                        return "Такого супер-пользователя не существует!";
                    else
                    { 
                        long recieverId;
                        using (PostDbContext db=new())
                            recieverId=db.special_users.Find(specUserId).specialUser_Identity;
                        var messArgs = new MessageArgs() { Reciever=recieverId, Media_mode=null};
                        corresponders.Add(current_worker,messArgs);
                    }
                }
                else
                    return "Формат введённого id неверный!";
            }
            else if (step==2)
            {
                CurrentArgs.Media_mode = GetMediaMode(message);
                if (CurrentArgs.Media_mode != null)
                    post_fields[1] = message.Caption;
            }
            return null;
        }
        protected override void Cancel(ITelegramBotClient botClient, Update update, bool error = false)
        {
            corresponders.Remove(current_worker);
            base.Cancel(botClient, update, error);
        }
        static MediaMode? GetMediaMode(Message message)
        {
            if (message.Photo != null)
                return MediaMode.Photo;
            else if (message.Document != null)
                return MediaMode.Document;
            else
                return null;
        }
        
        private InputOnlineFile? GetMediaFile(Message message,MediaMode mediaMode)
        {
            if (mediaMode != null)
            {
                string fileId = mediaMode == MediaMode.Photo ? message.Photo[0].FileId : message.Document.FileId;
                return new InputOnlineFile(fileId);
            }
            return null;
            
        }
       
        private async Task SendMedia(MediaMode mode,InputOnlineFile file,ITelegramBotClient botClient)
        {
            
            if (mode == MediaMode.Photo)

                await botClient.SendPhotoAsync(CurrentArgs.Reciever, file);
            else
                await botClient.SendDocumentAsync(CurrentArgs.Reciever, file);
        }
        protected override async void EndManage(ITelegramBotClient botClient, Update update)
        {
            var mediaMode = GetMediaMode(update.Message);
            SendMessage(botClient,current_worker,"Отправка сообщения...");
            int mySpecUserId = PostDbContext.GetFakeId(current_worker);
            SendMessage(botClient, CurrentArgs.Reciever, $"Сообщение от <b>{mySpecUserId}</b>\n\n" + post_fields[1] ?? "");

            if (mediaMode.HasValue)
            {
                var file = GetMediaFile(update.Message,mediaMode.Value);
                SendMedia(mediaMode.Value,file, botClient);
            }
            base.EndManage(botClient, update);
            corresponders.Remove(current_worker);
            
            
        }
    }
}
