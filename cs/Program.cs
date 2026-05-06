using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    async static Task work()
    {
        Console.WriteLine("begin work");
        await Task.Delay(3000);
        Console.WriteLine("end work");
    }

    async static Task<String> read()
    {
        Console.WriteLine("begin work");
        await Task.Delay(3000);
        Console.WriteLine("end work");
        return "text";
    }

    static async void run1()
    {
        await work();
        await work();
    }

    static async void run2()
    {
        String r = await read();
        Console.WriteLine(r);
    }
    
    static void Main(string[] args)
    {
        run2();
        Console.WriteLine("back in main");
        Thread.Sleep(8000);
    }
}