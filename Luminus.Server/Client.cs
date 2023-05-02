using Luminus.Server.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Luminus.Server
{
    public class Client : IDisposable
    {
        public User User { get; private set; }
        private readonly StreamWriter _writer;
        private readonly StreamReader _reader;
        private readonly ServerManager _serverManager;
        public Client(TcpClient client, ServerManager serverManager)
        {
            var networkStream = client.GetStream();
            _writer = new StreamWriter(networkStream);
            _reader = new StreamReader(networkStream);
            _serverManager = serverManager;
        }

        public async Task<bool> Authorize(User user)
        {
            using(var db = new ClientDbContext())
            {
                var result = await db.Users.FirstOrDefaultAsync(f => f.Name == user.Name && f.Password == user.Password);
                if (result == null) return false;
                User = result;
                User.Messages = null;
                return true;
            }
        }
        public async Task<bool> Register(User user)
        {
            using(var db = new ClientDbContext())
            {
                var result = await db.Users.FirstOrDefaultAsync(f => f.Name == user.Name && f.Password == user.Password);
                if(result == null)
                {
                    db.Users.Add(user);
                    await db.SaveChangesAsync();
                    User = await db.Users.LastAsync();
                    User.Messages = null;
                    return true;
                }
                return false;
            }
        }
        public async Task SendMessage(Message message)
        {
            await _writer.WriteLineAsync(JsonConvert.SerializeObject(message));
            await _writer.FlushAsync();
        }

        public async Task ProcessAsync()
        {
            while (true)
            {
                try
                {
                    var request = JsonConvert.DeserializeObject<BaseResponse>(await _reader.ReadLineAsync());
                    if (request != null)
                    {
                        switch (request.Type)
                        {
                            case ResponseType.Authorize:
                                if (await Authorize(request.Data.ToObject<User>()))
                                {
                                    await Console.Out.WriteLineAsync($"{User.Name} Authorize");
                                    await _writer.WriteLineAsync("Success");
                                    await _writer.FlushAsync();
                                    
                                }
                                else
                                {
                                    await _writer.WriteLineAsync("Unauthorized");
                                    await _writer.FlushAsync();
                                }
                                break;
                            case ResponseType.GetMessages:
                                if (User != null)
                                {
                                    using (var db = new ClientDbContext())
                                    {
                                        var messages = await db.Messages.Skip(Math.Max(0, db.Messages.Count() - 50)).ToListAsync();
                                        await Task.Delay(150);
                                        await _writer.WriteLineAsync(JsonConvert.SerializeObject(messages));
                                    }
                                }
                                break;
                            case ResponseType.SendMessage:
                                var message = request.Data.ToObject<Message>();
                                if (message.User != null && User.Name == message.User.Name && message.User.Password == User.Password)
                                {
                                    using (var db = new ClientDbContext())
                                    {
                                        var entity = db.Messages.Add(message);
                                        await db.SaveChangesAsync();
                                        await Console.Out.WriteLineAsync($"{User.Name}: {message.Text}");

                                    }
                                    await _serverManager.BroadcastMessage(message);
                                }
                                break;
                            case ResponseType.Register:
                                if (await Register(request.Data.ToObject<User>()))
                                {
                                    _writer.WriteLine("Success");
                                    await _writer.FlushAsync();
                                }
                                else
                                {
                                    _writer.WriteLine("Account was created!");
                                }
                                break;
                        }
                    }
                }catch
                {
                    _serverManager.Disconnect(this);
                    Console.WriteLine($"{User.Name} Disconnect!");
                    Dispose();
                    return;
                }
            }
        }

        public void Dispose()
        {
            _reader.Close();
            _writer.Close();
            _writer.Dispose();
            _reader.Dispose();
        }
    }
}
