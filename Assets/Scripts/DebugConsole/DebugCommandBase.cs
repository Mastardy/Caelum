public class DebugCommandBase
{
    public string CommandName { get; private set; }
    public string CommandDescription { get; private set; }
    public string CommandFormat { get; private set; }

    protected DebugCommandBase(string name, string description, string format)
    {
        CommandName = name;
        CommandDescription = description;
        CommandFormat = format;
    }
}