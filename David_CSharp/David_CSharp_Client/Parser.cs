namespace David_CSharp_Client;

public interface ILine
{
}

public class Information : ILine
{
    public required string Text { get; init; }

    public override string ToString()
    {
        var parts = Text.Split('\t');
        return parts[0];
    }
}

public class TextFile : ILine
{
    public string Text { get; }
    public string Selector { get; }
    public string Host { get; }
    
    private readonly string _fullLine;

    /// <summary>
    /// 1CIA World Factbook     /Archives/mirrors/textfiles.com/politics/CIA    gopher.quux.org 70
    /// </summary>
    public TextFile(string fullLine)
    {
        _fullLine = fullLine;
        var parts = fullLine.Split('\t');

        Text = parts[0];
        Selector = parts[1];
        Host = "gopher://" + parts[2];
    }

    public override string ToString()
    {
        
        return _fullLine;
    }
}

public class SubMenu : ILine
{
    public string Text { get; }
    public string Selector { get; }
    public string Host { get; }
    
    private readonly string _fullLine;

    /// <summary>
    /// 1CIA World Factbook     /Archives/mirrors/textfiles.com/politics/CIA    gopher.quux.org 70
    /// </summary>
    public SubMenu(string fullLine)
    {
        _fullLine = fullLine;
        var parts = fullLine.Split('\t');

        Text = parts[0];
        Selector = parts[1];
        Host = "gopher://" +  parts[2];
    }

    public override string ToString()
    {
        
        return _fullLine;
    }
}

public class Search : ILine
{
    public string Text { get; }
    public string Selector { get; }
    public string Host { get; }
    
    private readonly string _fullLine;

    public Search(string fullLine)
    {
        _fullLine = fullLine;
        var parts = fullLine.Split('\t');

        Text = parts[0];
        Selector = parts[1];
        Host = "gopher://" + parts[2];
    }

    public override string ToString()
    {
        return _fullLine;
    }
}

public class CCSONameServer : ILine
{
    public string Text { get; }
    public string Selector { get; }
    public string Host { get; }
    
    private readonly string _fullLine;

    public CCSONameServer(string fullLine)
    {
        _fullLine = fullLine;
        var parts = fullLine.Split('\t');

        Text = parts[0];
        Selector = parts[1];
        Host = "gopher://" + parts[2];
    }

    public override string ToString()
    {
        return _fullLine;
    }
}

public class Unknown : ILine
{
    public required string Text { get; init; }

    public override string ToString()
    {
        return "Unknown -- " + Text;
    }
}

public static class GopherParser
{
    public static ILine Parse(string line)
    {
        if (line.StartsWith('i'))
            return new Information { Text = line[1..] };

        if (line.StartsWith('0'))
            return new TextFile(line[1..]);
        
        if (line.StartsWith('1'))
            return new SubMenu(line[1..]);
        
        if (line.StartsWith('2'))
            return new CCSONameServer(line[1..]);
        
        if (line.StartsWith('7'))
            return new Search(line[1..]);
        
        return new Unknown { Text = line };
    }
}