using System.Net;
using System.Net.Sockets;
using System.Text;

var hostName = Dns.GetHostName();
var localhost = await Dns.GetHostEntryAsync(hostName);
var localIpAddress = localhost.AddressList[0];

IPEndPoint ipEndPoint = new(localIpAddress, 70);

using var listener = new Socket(
    SocketType.Stream,
    ProtocolType.Tcp);

listener.Bind(ipEndPoint);
listener.Listen(100);

var handler = await listener.AcceptAsync();
while (true)
{
    // Receive message.
    var buffer = new byte[1_024];
    var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
    var request = Encoding.UTF8.GetString(buffer, 0, received);
    Console.WriteLine(request);
    if (string.IsNullOrWhiteSpace(request)) continue;
    if (!request.StartsWith('/')) continue;
    if (request.Contains("..")) continue;
    
    // Paths are relative to /Users/davidbetteridge
    // ie. /pictures means /Users/davidbetteridge/pictures
    var fullPath = Path.Combine("/Users/davidbetteridge", request);
    Console.WriteLine(fullPath);
    
    if (!Directory.Exists(fullPath)) continue;

    foreach (var subfolder in Directory.GetDirectories(fullPath))
    {
        var folderName = Path.GetFileNameWithoutExtension(subfolder);
        var ackMessage = $"1{folderName}\t/{folderName}\tlocalhost\t70";
        var echoBytes = Encoding.UTF8.GetBytes(ackMessage);
        await handler.SendAsync(echoBytes, 0);        
    }
    
    // var eom = "<|EOM|>";
    // if (response.IndexOf(eom) > -1 /* is end of message */)
    // {
    //     Console.WriteLine(
    //         $"Socket server received message: \"{response.Replace(eom, "")}\"");
    //
    //     var ackMessage = "<|ACK|>";
    //     var echoBytes = Encoding.UTF8.GetBytes(ackMessage);
    //     await handler.SendAsync(echoBytes, 0);
    //     Console.WriteLine(
    //         $"Socket server sent acknowledgment: \"{ackMessage}\"");
    //
    //     break;
    // }
    // // Sample output:
    //    Socket server received message: "Hi friends 👋!"
    //    Socket server sent acknowledgment: "<|ACK|>"
}