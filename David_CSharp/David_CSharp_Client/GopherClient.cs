using System.Net;
using System.Net.Sockets;
using System.Text;

namespace David_CSharp_Client;

public class GopherClient : IDisposable
{
    private readonly Socket _socket;

    private GopherClient(Socket socket)
    {
        _socket = socket;
    }

    public static async Task<GopherClient> ConnectAsync(Uri address)
    {
        var hostEntry = await Dns.GetHostEntryAsync(address.DnsSafeHost);
        var ipAddress = hostEntry.AddressList[0];

        IPEndPoint ipEndPoint = new(ipAddress, 70);

        Socket client = new(
            ipEndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);

        await client.ConnectAsync(ipEndPoint);

        return new GopherClient(client);
    }


    public async Task<List<string>> GetLinesAsync(string selector)
    {
        var messageBytes = Encoding.UTF8.GetBytes(selector + System.Environment.NewLine);
        _ = await _socket.SendAsync(messageBytes, SocketFlags.None);

        var buffer = new byte[1_024];
        int received;
        var all = "";
        do
        {
            received = await _socket.ReceiveAsync(buffer, SocketFlags.None);
            if (received > 0)
            {
                var response = Encoding.UTF8.GetString(buffer, 0, received);
                if (response != ".")
                {
                    all += response;
                }
            }
        } while (received > 0 );

        
        return all.Split(System.Environment.NewLine).ToList();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _socket.Close();
        _socket.Dispose();
    }
}