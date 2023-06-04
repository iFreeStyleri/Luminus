using Luminus.Chat.Models;
using Luminus.Chat.Services;
using Luminus.Domain.Entities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
namespace Luminus.Chat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private User User { get; set; }
        private ClientWebManager manager => App.ClientManager;

        public MainWindow(User user)
        {
            User = user;
            InitializeComponent();
            manager.OnMessage += Manager_AddMessage;
            
            _ = manager.EchoMessage();
            selectedUser.Text = User.Name;
            _ = CheckConnection();
        }
        private async Task CheckConnection()
        {
            await Task.Run(() =>
            {
                App.Current.Dispatcher.Invoke(async () =>
                {
                    while (true)
                    {
                        if (manager.Connected)
                            statusText.Foreground = Brushes.Green;
                        else
                            statusText.Foreground = Brushes.Red;
                        await Task.Delay(100);
                    }
                });

            });
        }
        private void Manager_AddMessage(Message message)
        {
            list.Items.Add(new MessageModel(message, User));
        }

        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            var text = new TextRange(richBox.Document.ContentStart, richBox.Document.ContentEnd).Text.TrimStart().TrimEnd();
            if (!string.IsNullOrEmpty(text))
            {
                try
                {
                    var message = new Message { User = User, Text = text, Created = DateTime.Now };
                    if (manager.Connected)
                    {
                        await manager.SendMessage(message);
                        list.Items.Add(new MessageModel(message, User));
                        richBox.Document.Blocks.Clear();
                    }
                }
                catch
                {
                    MessageBox.Show("Проблемы с сервером");
                }
            }
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
            manager.Disconnect();
            var auth = new AuthWindow();
            auth.Show();
            Close();
        }
        private void Minimal_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var message = await manager.GetMessages();
            message.ForEach(f => list.Items.Add(new MessageModel(f, User)));
        }

        private async void addFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            if(fileDialog.ShowDialog() == true)
            {
                var text = new TextRange(richBox.Document.ContentStart, richBox.Document.ContentEnd).Text;
                await manager.SendFile(fileDialog.OpenFile(), fileDialog.SafeFileName);
                var message = new Message
                {
                    User = manager.User,
                    Created = DateTime.Now,
                    File = new List<MessageFile>
                    {
                        new MessageFile
                        {
                            FileName = fileDialog.SafeFileName
                        }
                    },
                    Text = text
                };
                await manager.SendMessage(message);
                list.Items.Add(new MessageModel(message, manager.User));
            }
        }

        private async void getFile_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var openDialog = new SaveFileDialog();
            openDialog.FileName = (string)button.Content;
            if (openDialog.ShowDialog() == true)
            {
                var stream = await manager.GetFile((string)button.Content);
                using var fileStream = new FileStream(openDialog.FileName, FileMode.CreateNew);
                await stream.CopyToAsync(fileStream);
            }
        }

        private void Users_Click(object sender, RoutedEventArgs e)
        {
            var window = new UsersWindow(manager);
            window.Show();
        }
    }
}
