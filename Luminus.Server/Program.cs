using Luminus.Server;

await using var server = new ServerManager();
await server.ListenAsync();