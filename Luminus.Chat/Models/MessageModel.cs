using Luminus.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Luminus.Chat.Models
{
    public class MessageModel
    {
        public int Id { get; set; }
        public User User { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public string FileName { get; set; }
        public HorizontalAlignment Aligment { get; set; }
        public Visibility FileVisibility { get; set; }

        public MessageModel(Message message, User mainUser)
        {
            var isRight = message.User.Name == mainUser.Name;
            if (isRight)
                Aligment = HorizontalAlignment.Right;
            else
                Aligment = HorizontalAlignment.Left;
            User = message.User;
            Id = message.Id;
            Text = message.Text;
            Created = message.Created;
            if(message.File != null && message.File.Count != 0)
            {
                FileName = message.File.First().FileName;
                FileVisibility = Visibility.Visible;
            }
            FileVisibility = Visibility.Collapsed;
        }
    }
}
