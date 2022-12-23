using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Luminus.Server
{
    class UserObject
    {
        protected internal User User = new User();
        protected internal StreamWriter Writer { get; }
        protected internal StreamReader Reader { get; }

        TcpClient client;
        ServerManager server; // объект сервера

        public UserObject(TcpClient tcpClient, ServerManager serverObject)
        {
            client = tcpClient;
            server = serverObject;
            // получаем NetworkStream для взаимодействия с сервером
            var stream = client.GetStream();
            // создаем StreamReader для чтения данных
            Reader = new StreamReader(stream);
            // создаем StreamWriter для отправки данных
            Writer = new StreamWriter(stream);
        }

        public async Task ProcessAsync()
        {
            try
            {
                var message = JsonConvert.DeserializeObject<Message>(await Reader.ReadLineAsync());
                
                await server.BroadcastMessageAsync(message);
                Console.WriteLine(message);
                while (true)
                {
                    try
                    {
                        message = JsonConvert.DeserializeObject<Message>(await Reader.ReadLineAsync());
                        if (message == null) continue;
                        Console.WriteLine(message.User.Id + $": {message.Text}" );
                        await server.BroadcastMessageAsync(message);
                    }
                    catch
                    {
                        Console.WriteLine($"{message.User.Name} покинул чат");
                        await server.BroadcastMessageAsync(message);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                server.RemoveConnection(User.Id);
            }
        }
        // закрытие подключения
        protected internal void Close()
        {
            Writer.Close();
            Reader.Close();
            client.Close();
        }
    }
}
