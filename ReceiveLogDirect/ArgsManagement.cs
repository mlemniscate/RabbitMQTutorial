namespace ReceiveLogDirect;

public static class ArgsManagement
{
    public static void CheckArgsCount(string[] args)
    {
        if (args.Length >= 1) return;
        Console.Error.WriteLine($"Usage: {Environment.GetCommandLineArgs()[0]}" +
                                $"[info] [warning] [error]");
        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
        Environment.ExitCode = 1;
    }
}