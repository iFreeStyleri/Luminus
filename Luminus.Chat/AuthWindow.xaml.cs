using Luminus.Chat.Models;
using Luminus.Chat.DAL;
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

namespace Luminus.Chat
{
    public partial class AuthWindow : Window
    {
        public User User { get; set; }
        public AuthWindow()
        {
            InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            btn.IsEnabled = false;
            if (!string.IsNullOrEmpty(userText.Text) && !string.IsNullOrEmpty(userText.Text))
            {
                using(var db = new ClientDbContext())
                {
                    var result = await db.Users.FirstOrDefaultAsync(f => f.Name == userText.Text && f.Password == passText.Text);
                    if (result == null)
                    {
                        MessageBox.Show("Такого пользователя не существует");
                        btn.IsEnabled = true;
                        return;
                    }
                    new MainWindow(result).Show();
                    Close();
                }
            }
            btn.IsEnabled = true;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var window = new RegWindow();
            window.Owner = this;
            if(window.ShowDialog() == true)
            {
                new MainWindow(User).Show();
                Close();
            }
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
