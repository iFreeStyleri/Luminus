using Luminus.Chat.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Luminus.Chat.Services
{
    public class ClientManager : IDisposable
    {
        private TcpClient Client { get; set; }
        public bool Connected => Client.Connected;
        public List<Message> Messages = new List<Message>();
        public delegate void MessageHandler(Message message);
        public event MessageHandler AddMessage;
        string host = "127.0.0.1";
        int port = 8888;
        private StreamReader reader;
        private StreamWriter writer;
        public ClientManager(User user)
        {
            Connect(user);
        }

        public async void Connect(User user)
        {
            Client = new TcpClient();
            try
            {
                await Client.ConnectAsync(host, port);
                if (Client.Connected)
                {
                    reader = new StreamReader(Client.GetStream());
                    writer = new StreamWriter(Client.GetStream());
                    await writer.WriteLineAsync(JsonConvert.SerializeObject(user));
                    await writer.FlushAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public async void OpenChat()
        {
            await Task.Run(ReceiveMessageAsync);
        }
        public async Task SendMessageAsync(Message message)
        {
            if (Connected)
            {
                await writer.WriteLineAsync(JsonConvert.SerializeObject(message));
                await writer.FlushAsync();
            }
        }

        public async void ReceiveMessageAsync()
        {
            while (true)
            {
                try
                {
                    if (Connected)
                    {
                        var message = JsonConvert.DeserializeObject<Message>(await reader.ReadLineAsync());
                        if (message == null) continue;
                        Messages.Add(message);
                        App.Current.Dispatcher.Invoke(() => AddMessage.Invoke(message));
                    }
                }
                catch
                {
                    continue;
                }
            }
        }

        public void Dispose()
        {
            Client.Close();
            Client.Dispose();
        }

        public void Close()
        {
            Client.Close();
            writer?.Dispose();
            reader?.Dispose();
        }
    }
}
