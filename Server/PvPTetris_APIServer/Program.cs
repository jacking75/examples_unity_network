using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TetrisApiServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var serverOption = ParseCommandLine(args);
            if (serverOption == null)
            {
                return;
            }

            APIServer.Init(serverOption);

            BuildWebHost(args).Run();
        }
        

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseUrls("http://*:19000/")
                .Build();


        static ServerOptions ParseCommandLine(string[] args)
        {
            var result = CommandLine.Parser.Default.ParseArguments<ServerOptions>(args) as CommandLine.Parsed<ServerOptions>;

            if (result == null)
            {
                System.Console.WriteLine("Failed Command Line");
                return null;
            }

            return result.Value;
        }
    }
}
