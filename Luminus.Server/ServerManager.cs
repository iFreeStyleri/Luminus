using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Luminus.Server
{
    public class ServerManager : IAsyncDisposable
    {
        TcpListener listener = new TcpListener(IPAddress.Any,8888);
        private List<UserObject> activeUsers = new List<UserObject>();
        public void RemoveConnection(int id)
        {
            var user = activeUsers.FirstOrDefault(f => f.User.Id == id);
            if(user != null)
                activeUsers.Remove(user);
        }
        public async Task ListenAsync()
        {
            try
            {
                listener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient tcpClient = await listener.AcceptTcpClientAsync();
                    UserObject userObject = new UserObject(tcpClient, this);
                    activeUsers.Add(userObject);
                    _ = Task.Run(userObject.ProcessAsync);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        public async Task BroadcastMessageAsync(Message message)
        {
            foreach (var client in activeUsers)
            {
                if (client.User.Id != message.User.Id)
                {
                    await client.Writer.WriteLineAsync(JsonConvert.SerializeObject(message));
                    await client.Writer.FlushAsync();
                }
            }
        }
        public void Disconnect()
        {
            foreach (var client in activeUsers)
            {
                client.Close(); 
            }
            listener.Stop();
        }

        public ValueTask DisposeAsync()
        {
            Disconnect();
            return ValueTask.CompletedTask;
        }
    }
}
