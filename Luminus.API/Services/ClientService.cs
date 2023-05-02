using Luminus.API.DAL;
using Luminus.API.Models;
using Luminus.Domain;
using Luminus.Domain.Entities;
using Luminus.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace Luminus.API.Services
{
    public class ClientService
    {
        private static List<ClientModel> _clients;
        public static List<ClientModel> Clients
        {
            get
            {
                if (_clients == null)
                    _clients = new List<ClientModel>();
                return _clients;
            }
        }
        private readonly ClientDbContext _context;
        public ClientService(ClientDbContext context)
        {
            _context = context;
        }
        private bool UserIsConnect(User user)
            => Clients.FirstOrDefault(f => f.User.Id == user.Id) != null;

        public async Task Echo(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 8];

            while (true)
            {
                var receive = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (receive.MessageType == WebSocketMessageType.Text)
                {
                    var request = ConvertToJson(buffer, receive.Count);
                    if (request != null)
                    {
                        switch (request.Type)
                        {
                            case RequestType.Connect:
                                var user = request.Data.ToObject<User>();
                                if (user != null && !UserIsConnect(user))
                                {
                                    var model = new ClientModel(_context, webSocket, this);
                                    if (!await model.Connect(user))
                                        return;
                                    Clients.Add(model);
                                }
                                break;
                            case RequestType.SendMessage:
                                var message = request.Data.ToObject<Message>();
                                if (message != null && message.User.Name != null && UserIsConnect(message.User))
                                {
                                    await BroadcastMessage(message);
                                }
                                else { return; }
                                break;
                        }
                    }
                }
            }
        }
        public async Task BroadcastMessage(Message message)
        {
            foreach(var client in Clients)
            {
                if(message.User.Name != client.User.Name && message.User.Password != client.User.Name)
                {
                    message.User.Password = "";
                    try
                    {
                        await client.SendMessage(message);
                    }
                    catch
                    {
                        Clients.Remove(client);
                    }
                }
            }
        }
        private Request? ConvertToJson(byte[] bytes, int count)
        {
            var message = Encoding.UTF8.GetString(bytes, 0, count );
            return JsonConvert.DeserializeObject<Request>(message);
        }

        public void Disconnect(ClientModel client)
        {
            Clients.Remove(client);
        }
    }
}
