using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Luminus.Server
{
    public class ServerManager : IDisposable
    {
        public List<Client> Clients { get; set; }
        private readonly TcpListener _listener;
        public ServerManager()
        {
            Clients = new List<Client>();
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);
        }

        public async Task Echo()
        {
            _listener.Start();
            while (true)
            {
                var client = await _listener.AcceptTcpClientAsync();
                var newClient = new Client(client, this);
                Clients.Add(newClient);
                _ = Task.Run(newClient.ProcessAsync);
            }
        }
        public async Task BroadcastMessage(Message message)
        {
            foreach(var client in Clients)
            {
                if(message.User.Password != client.User.Password && message.User.Name != client.User.Name)
                {
                    message.User.Messages.Clear();
                    await client.SendMessage(message);
                }
            }
        }

        public void Disconnect(Client client)
        {
            Clients.Remove(client);
        }

        public void Dispose()
        {
            Clients.Clear();
            _listener.Stop();
        }
    }
}
