namespace NewTask;

public static class ConsoleMessage
{
    public static string GetMessage(string[] args)
    {
        return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
    }
}