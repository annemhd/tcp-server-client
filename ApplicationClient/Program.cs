using System.Net;
using System.Net.Sockets;
using System.Text;

IPHostEntry ipEntry = await Dns.GetHostEntryAsync(Dns.GetHostName());
IPAddress ip = ipEntry.AddressList[0];
IPEndPoint ipEndpoint = new(ip, 1234);

using Socket client = new(
    ipEndpoint.AddressFamily,
    SocketType.Stream,
    ProtocolType.Tcp
);

await client.ConnectAsync(ipEndpoint);

Console.Write("Entrer un nom d'utilisateur : ");
string username = Console.ReadLine();

if (string.IsNullOrWhiteSpace(username))
{
    Console.WriteLine("Un nom d'utilisateur est obligatoire");
    return;
}

while (true)
{
    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

    Console.WriteLine("Ecris ton message");
    var message = Console.ReadLine();
    var fullMessage = $"[{timestamp}] {username} : {message}";
    var messageBytes = Encoding.UTF8.GetBytes(fullMessage);

    await client.SendAsync(messageBytes, SocketFlags.None);

    var buffer = new byte[1_024];
    var received = await client.ReceiveAsync(buffer, SocketFlags.None);
    var messageString = Encoding.UTF8.GetString(buffer, 0, received);

    Console.WriteLine($"{messageString}");
}
