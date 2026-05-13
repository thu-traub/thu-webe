public class MyTime : ITime
{
    private readonly ILogger<MyTime> logger;

    public MyTime(ILogger<MyTime> logger)
    {
        this.logger = logger;
    }
    public string GetTime()
    {
        logger.LogInformation("GetTime called at {Time}", DateTime.Now);
        return DateTime.Now.ToString("HH:mm:ss");
    }
}   