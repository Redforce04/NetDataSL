namespace NetDataSL;

public static class Log
{
    public static void Debug(string x)
    {
        if(false)
            Console.WriteLine($"[Debug] {x}");
    }
    public static void Error(string x)
    {
        if(false)
            Console.WriteLine($"[Error] {x}");
    }

    public static void Line(string x) => Console.WriteLine(x);

}