using Luminus.Chat.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Luminus.Chat
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static ClientWebManager _clientManager;
        public static ClientWebManager ClientManager => _clientManager;
        public App()
        {
            _clientManager = new ClientWebManager();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _clientManager.Disconnect();
        }
    }
}
