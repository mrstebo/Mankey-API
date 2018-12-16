using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TorrentBackend
{
    public class Program
    {
        public static dynamic config;
        public static void Main(string[] args)
        {
            Console.WriteLine("Storing Configuration Variables.");
            config = Newtonsoft.Json.Linq.JObject.Parse(System.IO.File.ReadAllText("config.json"));
            #if DEBUG
            Console.WriteLine(config);
            #endif
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
		        .UseUrls(urls: "http://192.168.0.23:5000")
                .Build();
    }
}
