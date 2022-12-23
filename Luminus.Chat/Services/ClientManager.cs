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
    public class ClientManager
    {
        public List<Message> Messages = new List<Message>();
        public delegate void MessageHandler(Message message);
        public event MessageHandler AddMessage;
        string host = "127.0.0.1";
        int port = 8888;
        private StreamReader reader;
        private StreamWriter writer;
        public ClientManager()
        {
            Connect();
        }

        private void Connect()
        {
            using var client = new TcpClient(host, port);
            try
            {
                client.Connect(host, port);
                reader = new StreamReader(client.GetStream());
                writer = new StreamWriter(client.GetStream());
                if (writer is null || reader is null) return;
                Task.Run(() => ReceiveMessageAsync(reader));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public async Task SendMessageAsync(Message message)
        {
            await writer.WriteLineAsync(JsonConvert.SerializeObject(message));
            await writer.FlushAsync();
        }

        async Task ReceiveMessageAsync(StreamReader reader)
        {
            while (true)
            {
                try
                {
                    var message = JsonConvert.DeserializeObject<Message>(await reader.ReadLineAsync());
                    if (message == null) continue;
                    Messages.Add(message);
                    AddMessage.Invoke(message);
                }
                catch
                {
                    break;
                }
            }
        }
    }
}
