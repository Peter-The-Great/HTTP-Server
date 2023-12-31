﻿using System.Net;
using System.Net.Sockets;
using System.Web;

namespace Pretpark
{
    class Program
    {
        static int globalCounter = 0;
        static Dictionary<string, int> userCounters = new Dictionary<string, int>();

        static void Main(string[] args)
        {
            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);
            server.Start();
            Console.WriteLine("Server started");
            Console.WriteLine("Server is located at:");
            Console.WriteLine("http://127.0.0.1:5000/");

            while (true)
            {
                using Socket connection = server.AcceptSocket();
                using NetworkStream stream = new NetworkStream(connection);
                using StreamReader reader = new StreamReader(stream);
                using StreamWriter writer = new StreamWriter(stream);

                string? requestLine = reader.ReadLine();
                if (string.IsNullOrEmpty(requestLine))
                    continue;

                string[] parts = requestLine.Split(' ');
                string method = parts[0];
                string url = parts[1];

                if (method == "GET")
                {
                    if (url == "/")
                    {
                        // Home Page
                        string response = "HTTP/1.0 200 OK\r\nContent-Type: text/html\r\n\r\n";
                        response += "<html><body>";
                        response += "<h1>Welkom bij de website van PretPark <a href='https://nl.wikipedia.org/wiki/Den_Haag'>Den Haag</a></h1>";    
                        response += "<a href='/contact'>Contact</a><br>";
                        response += "<a href='/teller'>Teller</a><br>";
                        response += "<a href='/add?a=3&b=7'>Add</a><br>";
                        response += "<a href='/mijnteller'>Mijn Teller</a><br>";
                        response += "</body></html>";
                        writer.Write(response);
                    }
                    else if (url == "/contact")
                    {
                        // Contact Page
                        string response = "HTTP/1.0 200 OK\r\nContent-Type: text/html\r\n\r\n";
                        response += "<html><body>";
                        response += "Dit is de contactpagina.";
                        response += "<a href='https://google.com'>Hier is de website naar google.com</a>";
                        response += "</body></html>";
                        writer.Write(response);
                    }
                    else if (url == "/teller")
                    {
                        // Teller Page
                        globalCounter++;
                        string response = "HTTP/1.0 200 OK\r\nContent-Type: text/html\r\n\r\n";
                        response += "<html><body>";
                        response += "De teller staat op: " + globalCounter;
                        response += "</body></html>";
                        writer.Write(response);
                    }
                    else if (url.StartsWith("/add"))
                    {
                        // Add Page
                        // Parse query parameters
                        string[] queryParams = url.Split('?');
                        if (queryParams.Length == 2)
                        {
                            string[] paramPairs = queryParams[1].Split('&');
                            int sum = 0;
                            int minus = 0;
                            int multible = 1;
                            int divide = 1;
                            foreach (string paramPair in paramPairs)
                            {
                                string[] keyValue = paramPair.Split('=');
                                if (keyValue.Length == 2)
                                {
                                    if (int.TryParse(keyValue[1], out int value))
                                    {
                                        sum += value;
                                        minus -= value;
                                        multible *= value;
                                        divide /= value;
                                    }
                                }
                            }
                            string response = "HTTP/1.0 200 OK\r\nContent-Type: text/html\r\n\r\n";
                            response += "<html><body>";
                            response += "De som is: " + sum + "<br>";
                            response += "De som is: " + minus + "<br>";
                            response += "De som is: " + multible + "<br>";
                            response += "De som is: " + divide + "<br>";
                            response += "</body></html>";
                            writer.Write(response);
                        }
                    }
                    else if (url == "/mijnteller")
                    {
                        // Mijn Teller Page (gebruiker specifiek)
                        // Teller met queryparameter (per gebruiker)
                        string[] queryParams = url.Split('?');
                        var query = HttpUtility.ParseQueryString(url);
                        int teller = 0; // Standaardwaarde als "teller" ontbreekt
                        if (query["teller"] != null)
                        {
                            teller = int.Parse(query["teller"]);
                        }
                        teller++; // Verhoog de teller

                        string link = $"<a href=\"/mijnteller?teller={teller}\">klik hier om te verhogen</a>";
                        string responseBody = $"<html><body>De teller staat op {teller}, {link}</body></html>";
                        writer.Write("HTTP/1.0 200 OK\r\nContent-Type: text/html\r\n\r\n" + responseBody);

                    }
                    else
                    {
                        // 404 Page
                        string response = "HTTP/1.0 404 Not Found\r\nContent-Type: text/html\r\n\r\n";
                        response += "<html><body>";
                        response += "404 - Pagina niet gevonden";
                        response += "</body></html>";
                        writer.Write(response);
                    }
                }
                else
                {
                    // Unsupported method
                    string response = "HTTP/1.0 405 Method Not Allowed\r\nContent-Type: text/html\r\n\r\n";
                    response += "<html><body>";
                    response += "405 - Methode niet toegestaan";
                    response += "</body></html>";
                    writer.Write(response);
                }

                writer.Flush();
            }
        }
    }
}
