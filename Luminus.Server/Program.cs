using Luminus.Server;

using var server = new ServerManager();
await server.Echo();