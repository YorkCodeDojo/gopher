using System.Net;
using System.Net.Sockets;
using System.Text;

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

static async Task HandleClient(Socket socket)
{
    var buffer = new byte[1_024];
    var received = await socket.ReceiveAsync(buffer, SocketFlags.None);
    var request = Encoding.UTF8.GetString(buffer, 0, received).Trim();
    Console.WriteLine("Request " + request);

    var cells = new char[9] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I' };

    // The user is X and plays first
    var isX = true;
    foreach (var cell in request)
    {
        if (cell != '/')
        {
            var pos = cell - 'A';
            if (isX)
                cells[pos] = 'X';
            else
                cells[pos] = 'O';
            isX = !isX;
        }
    }
    
    // The computer is O and plays at random
    if (!string.IsNullOrWhiteSpace(request))
    {
        var available = cells.Where(c => c != 'X' && c != 'O').ToList();
        var random = new Random();
        var picked = random.Next(available.Count);
        var pickedCell = available[picked];
        cells[pickedCell - 'A'] = 'O';
        request += pickedCell;
    }

    await socket.SendAsync(Encoding.UTF8.GetBytes($"i{cells[0]} | {cells[1]} | {cells[2]}\txxx\txxxx" + System.Environment.NewLine), 0);
    await socket.SendAsync(Encoding.UTF8.GetBytes($"i{cells[3]} | {cells[4]} | {cells[5]}\txxx\txxxx" + System.Environment.NewLine), 0);
    await socket.SendAsync(Encoding.UTF8.GetBytes($"i{cells[6]} | {cells[7]} | {cells[8]}\txxx\txxxx" + System.Environment.NewLine), 0);
    await socket.SendAsync(Encoding.UTF8.GetBytes($"i\txxx\txxxx" + System.Environment.NewLine), 0);

    var remaining = cells.Where(c => c != 'X' && c != 'O').ToList();
    foreach (var possible in remaining)
    {
        await socket.SendAsync(Encoding.UTF8.GetBytes($"1{possible}\t{request+possible}\tlocalhost\t70" + System.Environment.NewLine), 0);  
    }
    
    Console.WriteLine("Done");
    socket.Close();
}