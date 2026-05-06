using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Globalization;

class Program
{
    static string? getText(int a)
    {
        if (a == 0) return null;
        return "hello";
    }
    static void Main(string[] args)
    {
        string? t = getText(0);
        if (t == null)
        {
            Console.WriteLine("t is null");
            return;
        }   
        string m = t;
        Console.WriteLine(m);
    }
}