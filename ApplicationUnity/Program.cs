using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Concurrent;

IPHostEntry ipEntry = await Dns.GetHostEntryAsync(Dns.GetHostName());
IPAddress ip = ipEntry.AddressList[0];
IPEndPoint ipEndpoint = new(ip, 1234);

using Socket server = new(
    ipEndpoint.AddressFamily,
    SocketType.Stream,
    ProtocolType.Tcp
);

server.Bind(ipEndpoint);
server.Listen();

Console.WriteLine("Bienvenue sur le serveur !");

var clients = new ConcurrentBag<Socket>();

while (true)
{
    var clientSocket = await server.AcceptAsync();
    clients.Add(clientSocket);
    Console.WriteLine($"Nouvelle connexion depuis {clientSocket.RemoteEndPoint}");

    _ = Task.Run(async () =>
    {
        try
        {
            await HandleClientAsync(clientSocket);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur avec le client {clientSocket.RemoteEndPoint}: {ex.Message}");
 