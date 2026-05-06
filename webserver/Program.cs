using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Diagnostics;

class MiniWebServer
{
    const string WWW_ROOT = "wwwroot";
    const string HOME_PAGE = "index.html";

    static string dynamicContent()
    {
        //return DateTime.Now.ToString();
        ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/c dir")
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        Process? p = Process.Start(psi);
        if (p == null) return "Error starting process";
        string output = p.StandardOutput.ReadToEnd();
        p.WaitForExit();
        output = output.Replace("<", "&lt;").Replace(">", "&gt;");
        return output;
    }

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
            string request = Encoding.UTF8.GetString(buf, 0, n);
            string[] parts = request.Split(" ");
            if (parts.Length < 2) {
                cl.Close();
                continue;
            }
            string filename = parts[1].Substring(1);
            if (filename == "") filename = HOME_PAGE;
            filename = Path.Combine(WWW_ROOT, filename);
            Console.WriteLine(filename);
            string extension = Path.GetExtension(filename).ToLower();
            System.Console.WriteLine("Request for " + extension);

            string mime = extension switch
            {
                ".html" => "text/html",
                ".css" => "text/css",
                ".js" => "application/javascript",
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                _ => "application/octet-stream"
            };

            if (!File.Exists(filename)) {
                string headerne = "HTTP/1.0 404 Not Found\r\nContent-Type: text/html\r\n\r\n<h1>File not found</h1>\r\n";
                cl.Send(Encoding.UTF8.GetBytes(headerne));
                cl.Close();
                continue;
            }

            byte[] content = File.ReadAllBytes(filename);

            if (extension == ".html") {
                string html = Encoding.UTF8.GetString(content);
                html = html.Replace("$$", dynamicContent());
                content = Encoding.UTF8.GetBytes(html);
            }

            string header = $"HTTP/1.0 200 OK\r\nContent-Type: {mime}\r\nContent-Length: {content.Length}\r\n\r\n";
            cl.Send(Encoding.UTF8.GetBytes(header));
            cl.Send(content);
            cl.Close();
        }
    }
}