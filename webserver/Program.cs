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
    const string WWW_ROOT = "wwwroot";
    const string HOME_PAGE = "index.html";
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

            string mime = "application/octet-stream";
            switch (extension) {
                case ".html": mime = "text/html"; break;
                case ".css": mime = "text/css"; break;
                case ".js": mime = "application/javascript"; break;
                case ".png": mime = "image/png"; break;
                case ".jpg":
                case ".jpeg": mime = "image/jpeg"; break;
            }

            if (!File.Exists(filename)) {
                string headerne = "HTTP/1.0 404 Not Found\r\nContent-Type: text/html\r\n\r\n<h1>File not found</h1>\r\n";
                cl.Send(Encoding.UTF8.GetBytes(headerne));
                cl.Close();
                continue;
            }

            byte[] content = File.ReadAllBytes(filename);
            string header = $"HTTP/1.0 200 OK\r\nContent-Type: {mime}\r\nContent-Length: {content.Length}\r\n\r\n";
            cl.Send(Encoding.UTF8.GetBytes(header));
            cl.Send(content);
            cl.Close();
        }
    }
}