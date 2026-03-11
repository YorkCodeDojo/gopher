
using David_CSharp_Client;
using Spectre.Console;

var server = new Uri("gopher://gopher.floodgap.com");
var client = await GopherClient.ConnectAsync(server);

await RenderPageAsync(client, "");

return;

async Task RenderPageAsync(GopherClient gopherClient, string selector)
{
    var resourceNumber = 0;

    var links = new List<ILine>(); // off by one!

    foreach (var line in await gopherClient.GetLinesAsync(selector))
    {
        var parsedLine = GopherParser.Parse(line);

        var colour = parsedLine switch
        {
            Information => Color.Blue,
            TextFile => Color.Green,
            SubMenu => Color.Yellow,
            _ => Color.Red,
        };

        var text = parsedLine switch
        {
            Information => parsedLine.ToString(),
            TextFile file => $"(TEXTFILE {++resourceNumber}) " + file.Text,
            SubMenu menu => $"(SUBMENU {++resourceNumber}) " + menu.ToString(),
            _ => parsedLine.ToString(),
        };
    
        if (parsedLine is TextFile or SubMenu)
            links.Add(parsedLine);
    
        var styled = new Text(text ?? string.Empty, new Style(foreground: colour));
        AnsiConsole.Write(styled);
        AnsiConsole.WriteLine();
    }
    
    var requestedResource = AnsiConsole.Ask<string>($"Navigate to: [[1-{resourceNumber}]]");
    var resource = links[int.Parse(requestedResource) - 1];

    if (resource is TextFile tf)
        await RenderPageAsync(gopherClient, tf.Selector);
    else
        Console.WriteLine(resource);
}

