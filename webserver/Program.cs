using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;

class MiniWebServer
{
    static void Main(string[] args)
    {
        IPEndPoint ep = new IPEndPoint(IPAddress.Any, 9000);
        Socket srv = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        srv.Bind(ep);
        srv.Listen();

        System.Console.WriteLine(
            "Webserver ready: open URL http://127.0.0.1:9000");

        while (true) {
            // Wait for an other client
            Socket cl = srv.Accept();
            byte[] buf = new byte[1024];
            int n = cl.Receive(buf);
            string mime = "text/html";
            string filename = "index.html";
            byte[] content = File.ReadAllBytes(filename);
            string header = $"HTTP/1.0 200 OK\r\nContent-Type: {mime}\r\nContent-Length: {content.Length}\r\n\r\n";
            cl.Send(Encoding.UTF8.GetBytes(header));
            cl.Send(content);
            cl.Close();
        }
    }
}