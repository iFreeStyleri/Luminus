using Luminus.Chat.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    /// <summary>
    /// Логика взаимодействия для UsersWindow.xaml
    /// </summary>
    public partial class UsersWindow : Window
    {
        private readonly ClientWebManager _manager;
        public UsersWindow(ClientWebManager manager)
        {
            _manager = manager;
            InitializeComponent();
            Init();
        }
        private async void Init()
        {
            UserList.ItemsSource = await _manager.GetActiveUsers();

        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Minimal_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try { DragMove(); } catch { }
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            UserList.ItemsSource = await _manager.GetActiveUsers();
        }
    }
}
