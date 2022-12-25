using Luminus.Chat.Models;
using Luminus.Chat.DAL;
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
                using (var db = new ClientDbContext())
                {
                    var user = new User { Name = userText.Text, Password = passText.Text };
                    db.Users.Add(user);
                    await db.SaveChangesAsync();
                    var result = await db.Users.FirstOrDefaultAsync(f => f.Name == user.Name && f.Password == user.Password);
                    ((AuthWindow)Owner).User = result;
                    DialogResult = true;
                    Close();
                }
            }
            btn.IsEnabled = true;

        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
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
