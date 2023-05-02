using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Luminus.Chat.Services;
using Luminus.Domain.Entities;

namespace Luminus.Chat
{
    public partial class AuthWindow : Window
    {
        private ClientWebManager manager => App.ClientManager;
        public AuthWindow()
        {
            InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            btn.IsEnabled = false;
            var user = new User { Name = userText.Text, Password = passText.Text };
            if(await manager.Authorize(user))
            {
                await manager.Connect();
                new MainWindow(user).Show();
                Close();
            }
            else
            {
                MessageBox.Show("Такого пользователя не существует!");
            }
            btn.IsEnabled = true;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var window = new RegWindow();
            window.Owner = this;
            window.Show();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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
    }
}
