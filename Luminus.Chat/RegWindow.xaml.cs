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
using Luminus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Luminus.Chat
{
    /// <summary>
    /// Логика взаимодействия для RegWindow.xaml
    /// </summary>
    public partial class RegWindow : Window
    {
        public RegWindow()
        {
            InitializeComponent();
        }

        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            btn.IsEnabled = false;
            if (!string.IsNullOrEmpty(userText.Text) && !string.IsNullOrEmpty(passText.Text))
            {
                var user = new User { Name = userText.Text, Password = passText.Text };
                if (await App.ClientManager.Register(user))
                {
                    await App.ClientManager.Connect();
                    new MainWindow(user).Show();
                    Close();
                    Owner.Close();
                }
                else
                    MessageBox.Show("Аккаунт существует!");
            }
            btn.IsEnabled = true;

        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
