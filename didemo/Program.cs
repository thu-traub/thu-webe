using Microsoft.Extensions.DependencyInjection;

class Program
{
    private static void Main(string[] args)
    {
        var srv = new ServiceCollection();
        srv.AddSingleton<IRun, App>();
        srv.AddSingleton<ITime, Time2>();
        var serviceProvider = srv.BuildServiceProvider();
        var app = serviceProvider.GetService<IRun>();
        if (app != null)
            app.Run();
    }
}

public interface IRun
{
    void Run();
}

public interface ITime
{
    string GetTime();
}

public class App : IRun
{
    private readonly ITime _time;

    public App(ITime time)
    {
        _time = time;
    }

    public void Run()
    {
        System.Console.WriteLine("Current Time: " + _time.GetTime());
    }
}

public class Time : ITime
{
    public string GetTime()
    {
        return DateTime.Now.ToString("hh:mm:ss");
    }
}

public class Time2 : ITime
{
    public string GetTime()
    {
        return "Zeit = "+DateTime.Now.ToString("hh:mm:ss");
    }
}