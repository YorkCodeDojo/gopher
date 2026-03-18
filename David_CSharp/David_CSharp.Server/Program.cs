using System.Net;
using System.Net.Sockets;
using System.Text;

const string BaseFolder = "/Users/davidbetteridge/gopher/David_CSharp/David_CSharp.Server";

var hostName = Dns.GetHostName();
var localhost = await Dns.GetHostEntryAsync(hostName);
var localIpAddress = localhost.AddressList[0];

Console.WriteLine("IP Address " + localIpAddress);

IPEndPoint ipEndPoint = new(localIpAddress, 70);

using var listener = new Socket(
    SocketType.Stream,
    ProtocolType.Tcp);

listener.Bind(ipEndPoint);
listener.Listen(100);

Console.WriteLine("Bound to port");

while (true) {
    var socket = listener.Accept();
    await Task.Run(async () =>
    {
        await HandleClient(socket);
    });
}

return;

static async Task HandleClient(Socket socket)
{
    var buffer = new byte[1_024];
    var received = await socket.ReceiveAsync(buffer, SocketFlags.None);
    var request = Encoding.UTF8.GetString(buffer, 0, received).Trim();
    Console.WriteLine("Request " + request);

    if (request.StartsWith("/0") || request == "")
    {
        Console.WriteLine("Submenu Request");
        if (request.StartsWith("/0"))
            request = request[2..];
        var fullPath = BaseFolder + request;
        
        foreach (var subfolder in Directory.GetDirectories(fullPath))
        {
            var folderName = Path.GetFileNameWithoutExtension(subfolder);
            var selector = "/0" + subfolder[(BaseFolder.Length)..];
            var ackMessage = $"1{folderName}\t{selector}\tlocalhost\t70" + System.Environment.NewLine;
            var echoBytes = Encoding.UTF8.GetBytes(ackMessage);
            await socket.SendAsync(echoBytes, 0);
        }

        foreach (var subfolder in Directory.GetFiles(fullPath))
        {
            var folderName = Path.GetFileNameWithoutExtension(subfolder);
            var selector = "/1" + subfolder[(BaseFolder.Length)..];
            var ackMessage = $"0{folderName}\t{selector}\tlocalhost\t70" + System.Environment.NewLine;
            var echoBytes = Encoding.UTF8.GetBytes(ackMessage);
            await socket.SendAsync(echoBytes, 0);
        }
        
        await socket.SendAsync(Encoding.UTF8.GetBytes("." + System.Environment.NewLine), 0);
    }

    if (request.StartsWith("/1"))
    {
        Console.WriteLine("Text File Request");
        request = request[2..];
        var fullPath = BaseFolder + request;

        var lines = await File.ReadAllLinesAsync(fullPath);
        foreach (var line in lines)
        {
            var echoBytes = Encoding.UTF8.GetBytes(line + System.Environment.NewLine);
            await socket.SendAsync(echoBytes, 0);
        }
    }
    
    Console.WriteLine("Done");
    socket.Close();
}