using Luminus.Chat.Models;
using Luminus.Chat.Services;
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
        public MainWindow()
        {
            InitializeComponent();
            manager = new ClientManager();
            manager.AddMessage += Manager_AddMessage;
        }

        private void Manager_AddMessage(Message message)
        {
            
        }

        private async void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                var textBox = (TextBox)sender;
                await manager.SendMessageAsync(new Message { User = User, Text = textBox.Text});
                textBox.Text = "";
            }
        }
    }
}
