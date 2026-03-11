using System.Net;
using System.Net.Sockets;
using System.Text;

namespace David_CSharp_Client;

public class GopherClient(Uri address)
{
    public Uri BaseAddress { get; set; } = address;
    
    public async Task<List<string>> GetLinesAsync(string selector)
    {
        var hostEntry = await Dns.GetHostEntryAsync(BaseAddress.DnsSafeHost);
        var ipAddress = hostEntry.AddressList[0];

        IPEndPoint ipEndPoint = new(ipAddress, 70);

        using var client = new Socket(
            ipEndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);

        await client.ConnectAsync(ipEndPoint);
        
        var messageBytes = Encoding.UTF8.GetBytes(selector + System.Environment.NewLine);
        _ = await client.SendAsync(messageBytes, SocketFlags.None);

        var buffer = new byte[1_024];
        int received;
        var all = "";
        do
        {
            received = await client.ReceiveAsync(buffer, SocketFlags.None);
            if (received > 0)
            {
                var response = Encoding.UTF8.GetString(buffer, 0, received);
                all += response;
            }
        } while (received > 0 );

        
        return all.Split(System.Environment.NewLine).ToList();
    }
}