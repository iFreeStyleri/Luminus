using Luminus.API.DAL;
using Luminus.API.Services;
using Luminus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace Luminus.API.Models
{
    public class ClientModel : IDisposable
    {
        public User User { get; private set; }

        private readonly WebSocket _webSocket;
        private readonly ClientDbContext _clientContext;
        private readonly ClientService _clientService;
        public ClientModel(ClientDbContext clientContext, WebSocket webSocket, ClientService clientService)
        {
            _clientContext = clientContext;
            _webSocket = webSocket;
            _clientService = clientService;
        }

        public async Task<bool> Connect(User user)
        {
            var result = await _clientContext.Users.AsNoTracking().FirstOrDefaultAsync(f => f.Name == user.Name && f.Password == user.Password);
            if (result == null)
            {
                Dispose();
                return false;
            }
            User = user;
            return true;
        }
        public async Task SendMessage(Message message)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, false, CancellationToken.None);
        }
        public async void Dispose()
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, _webSocket.CloseStatusDescription, CancellationToken.None);
            _webSocket.Dispose();
            _clientService.Disconnect(this);
        }

        public async Task SendActive(UserInfo userInfo)
        {
        }
    }
}
