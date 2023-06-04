using Luminus.Chat.Models;
using Luminus.Domain;
using Luminus.Domain.Entities;
using Luminus.Domain.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Luminus.Chat.Services
{
    public class ClientWebManager
    {
        public bool Connected => _webSocket.State == WebSocketState.Open;
        public delegate void MessageHandler(Message message);
        public event MessageHandler OnMessage;
        public User User { get; private set; }
        private ClientWebSocket _webSocket;

        public async Task Connect()
        {
            if (User != null)
            {
                _webSocket = new ClientWebSocket();
                var request = new Request { Data = JObject.Parse(JsonConvert.SerializeObject(User)), Type = RequestType.Connect };
                await _webSocket.ConnectAsync(new Uri("ws://localhost:5272/api/chat/echo"), CancellationToken.None);
                await _webSocket.SendAsync(new ArraySegment<byte>(Serialize(request)), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }


        public async void Disconnect()
        {
            try
            {
                var request = new Request { Data = JObject.Parse(JsonConvert.SerializeObject(User)), Type = RequestType.Disconnect };
                await _webSocket.SendAsync(new ArraySegment<byte>(Serialize(request)), WebSocketMessageType.Text, true, CancellationToken.None);
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "500", CancellationToken.None);
                User = null;
            }
            catch
            {

            }
        }

        public async Task<bool> Authorize(User user)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsJsonAsync("http://localhost:5272/api/account/auth", user);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                User = JsonConvert.DeserializeObject<User>(content);
                return true;
            }
            return false;
        }

        public async Task SendMessage(Message message)
        {
            if (User != null)
            {
                message.User = User;
                message.Created = DateTime.Now;
                var data = Serialize(new Request { Data = JObject.Parse(JsonConvert.SerializeObject(message)), Type = RequestType.SendMessage });
                await _webSocket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None);
                using var httpClient = new HttpClient();
                await httpClient.PostAsJsonAsync("http://localhost:5272/api/chat/save-message", message);
            }
        }
        public async Task SendFile(Stream stream, string fileName)
        {
            var requestContent = new MultipartFormDataContent();
            using var httpClient = new HttpClient();
            using var reader = new StreamContent(stream);
            var byteContent = new ByteArrayContent(await reader.ReadAsByteArrayAsync());
            requestContent.Add(byteContent, "file", fileName);
            var response = await httpClient.PostAsync($"http://localhost:5272/api/file/send", requestContent);

        }
        public async Task<Stream> GetFile(string fileName)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"http://localhost:5272/api/file/{fileName}");
            if (response.IsSuccessStatusCode)
                return response.Content.ReadAsStream();
            return null;
        }
        public async Task<List<Message>> GetMessages()
        {
            if (User != null)
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.PostAsJsonAsync("http://localhost:5272/api/chat/get-messages", User);
                if(response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Message>>(content);
                }
            }
            return null;
        }
        public async Task<bool> Register(User user)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsJsonAsync("http://localhost:5272/api/account/register", user);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                User = JsonConvert.DeserializeObject<User>(content);
                return true;
            }
            return false;
        }

        public async Task EchoMessage()
        {
            var buffer = new byte[1024 * 8];
            try
            {
                while (true)
                {
                    var receive = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (receive.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Deserialize<Message>(buffer, receive.Count);
                        OnMessage.Invoke(message);
                    }
                    else if (receive.MessageType == WebSocketMessageType.Close)
                    {
                        Disconnect();
                        return;
                    }
                }
            }
            catch
            {

            }
        }

        private byte[] Serialize (object data)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
            return bytes;
        }

        private T Deserialize <T>(byte[] data, int count)
        {
            var obj = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data,0, count));
            return obj;
        }

        public async Task<IEnumerable<UserInfoModel>> GetActiveUsers()
        {
            using var httpClient = new HttpClient();
            return await httpClient.GetFromJsonAsync<List<UserInfoModel>>("http://localhost:5272/api/account/get-active-users");
        }
    }
}
