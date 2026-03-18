
using David_CSharp_Client;
using Spectre.Console;

var server = "gopher://colorfield.space";
// var server = new Uri("gopher://tilde.club/1/~mz721");

// bombadillo gopher://colorfield.space 
  
await RenderPageAsync(server, "");

return;

async Task RenderPageAsync(string baseAddress, string selector)
{
    var client = new GopherClient(new Uri(baseAddress));
    
    var resourceNumber = 0;

    var links = new List<ILine>(); // off by one!

    foreach (var line in await client.GetLinesAsync(selector))
    {
        var parsedLine = GopherParser.Parse(line);

        var colour = parsedLine switch
        {
            Information => Color.Blue,
            TextFile => Color.Green,
            SubMenu => Color.Yellow,
            Search => Color.Orange1,
            _ => Color.Red,
        };

        var text = parsedLine switch
        {
            Information => parsedLine.ToString(),
            TextFile file => $"(TEXTFILE {++resourceNumber}) " + file.Text,
            SubMenu menu => $"(SUBMENU {++resourceNumber}) " + menu.Text,
            Search search => $"(SEARCH {++resourceNumber}) " + search.Text,
            CCSONameServer ns => $"(CCSONameServer {++resourceNumber}) " + ns.Text,
            _ => parsedLine.ToString(),
        };
    
        if (parsedLine is TextFile or SubMenu or Search or CCSONameServer)
            links.Add(parsedLine);
    
        var styled = new Text(text ?? string.Empty, new Style(foreground: colour));
        AnsiConsole.Write(styled);
        AnsiConsole.WriteLine();
    }
    
    var requestedResource = AnsiConsole.Ask<string>($"Navigate to: [[1-{resourceNumber}]]");
    var resource = links[int.Parse(requestedResource) - 1];

    if (resource is TextFile tf)
        await RenderTextFileAsync(tf.Host, tf.Selector);
    else if (resource is CCSONameServer ns)
        await RenderPageAsync(ns.Host, ns.Selector);        
    else if (resource is SubMenu sm)
        await RenderPageAsync(sm.Host, sm.Selector);    
    else if (resource is Search search)
        await RenderSearchAsync(search.Host, search.Selector);
    else
        Console.WriteLine(resource);
}


async Task RenderSearchAsync(string baseAddress, string selector)
{
    var searchTerm = AnsiConsole.Ask<string>("What would you like to search for ?");
    await RenderPageAsync(baseAddress, $"{selector}?{searchTerm}");
}

async Task RenderTextFileAsync(string baseAddress, string selector)
{
    var client = new GopherClient(new Uri(baseAddress));

    foreach (var line in await client.GetLinesAsync(selector))
    {
        var styled = new Text(line, new Style(foreground: Color.Yellow));
        AnsiConsole.Write(styled);
        AnsiConsole.WriteLine();
    }
}