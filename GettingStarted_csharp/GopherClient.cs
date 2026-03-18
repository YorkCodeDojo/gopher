using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GettingStarted_csharp;

/// <summary>
/// Creates a new Gopher client with a specified host
/// <code>
/// var client = new GopherClient(new Uri("gopher://colorfield.space"));
/// </code>
/// </summary>
/// <param name="address">For example gopher://colorfield.space</param>
public class GopherClient(Uri address)
{
    private Uri BaseAddress { get; set; } = address;
    
    /// <summary>
    /// Returns the Gopher text resource as specified by the selector
    /// </summary>
    public async Task<List<string>> GetLinesAsync(string selector, CancellationToken cancellationToken = default)
    {
        var hostEntry = await Dns.GetHostEntryAsync(BaseAddress.DnsSafeHost, cancellationToken);
        var ipAddress = hostEntry.AddressList[0];

        IPEndPoint ipEndPoint = new(ipAddress, 70);

        using var client = new Socket(
            ipEndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);

        await client.ConnectAsync(ipEndPoint, cancellationToken);
        
        var messageBytes = Encoding.UTF8.GetBytes(selector + System.Environment.NewLine);
        _ = await client.SendAsync(messageBytes, SocketFlags.None, cancellationToken);

        var buffer = new byte[1_024];
        int received;
        var all = "";
        do
        {
           
            received = await client.ReceiveAsync(buffer, SocketFlags.None, cancellationToken);
            if (received > 0)
            {
                var response = Encoding.UTF8.GetString(buffer, 0, received);
                all += response;
            }
        } while (received > 0 );

        
        return all.Split(System.Environment.NewLine).ToList();
    }
}