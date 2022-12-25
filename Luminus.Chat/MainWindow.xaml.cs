using Luminus.Chat.DAL;
using Luminus.Chat.Models;
using Luminus.Chat.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Luminus.Chat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private User User { get; set; }
        private readonly ClientManager manager;

        public MainWindow(User user)
        {
            User = user;
            InitializeComponent();
            manager = new ClientManager(user);
            manager.AddMessage += Manager_AddMessage;
            manager.OpenChat();
            CheckPerTimeConnect();
        }

        private void Manager_AddMessage(Message message)
        {
            list.Items.Add(message);
        }

        private void CheckConnect(bool connect)
        {
            if (connect)
            {
                onlineState.Text = "Online";
                onlineState.Foreground = Brushes.Green;
            }
            else
            {
                onlineState.Text = "Offline";
                onlineState.Foreground = Brushes.Red;
            }
        }

        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            var text = new TextRange(richBox.Document.ContentStart, richBox.Document.ContentEnd).Text.TrimStart().TrimEnd();
            if (!string.IsNullOrEmpty(text))
            {
                try
                {
                    var message = new Message { User = User, Text = text };
                    await manager.SendMessageAsync(message);
                    CheckConnect(manager.Connected);
                    list.Items.Add(message);
                    richBox.Document.Blocks.Clear();
                }
                catch
                {
                    MessageBox.Show("Проблемы с сервером");
                    CheckConnect(false);
                }
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            manager.Close();
            manager.Connect(User);
            manager.OpenChat();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch
            {
                return;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            manager.Close();
            Application.Current.Shutdown();
        }

        private void CheckPerTimeConnect()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await App.Current.Dispatcher.InvokeAsync(() => CheckConnect(manager.Connected));
                    await Task.Delay(200);
                }
            });
        }
    }
}
