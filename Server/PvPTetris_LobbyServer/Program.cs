using System;

namespace LobbyServer
{
    class Program
    {
        //dotnet LobbyServer.dll --uniqueID 1 --roomMaxCount 16 --roomMaxUserCount 4 --roomStartNumber 1 --maxUserCount 100
        static void Main(string[] args)
        {
            System.AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(UnhandledException);
            var serverOption = ParseCommandLine(args);
            if(serverOption == null)
            {
                return;
            }

           
            var serverApp = new LobbyServer();
            serverApp.InitConfig(serverOption);

            serverApp.CreateStartServer();

            LobbyServer.MainLogger.Info("Press q to shut down the server");

            while (true)
            {
                System.Threading.Thread.Sleep(128);

                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.KeyChar == 'q')
                {
                    Console.WriteLine("Server Stop ~~~");
                    serverApp.StopServer();
                    break;
                }
                else
                {
                    Console.WriteLine($"Preessed key:{key.KeyChar}");
                }
            }

            Console.WriteLine("Server Terminate ~~~");
        }

        static ServerOption ParseCommandLine(string[] args)
        {
            var result = CommandLine.Parser.Default.ParseArguments<ServerOption>(args) as CommandLine.Parsed<ServerOption>;

            if (result == null)
            {
                System.Console.WriteLine("Failed Command Line");
                return null;
            }

            return result.Value;
        }                  


        static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var ex = (Exception)e.ExceptionObject;
                var exMsg = ExceptionHelper.ExtractException(ex, 2);
                Console.WriteLine($"[Exception] : {exMsg}");
            }
            finally
            {
                Environment.Exit(1);
            }
        }
    } // end Class


    public static class ExceptionHelper
    {
        public static string ExtractException(this Exception ex, int indent = 2)
        {
            var indentStr = new String(' ', indent);
            var traceLog = new System.Text.StringBuilder();
            var trace = new System.Diagnostics.StackTrace(ex, true);
            foreach (var frame in trace.GetFrames())
            {
                traceLog.AppendLine($"{indentStr}File Name : {frame.GetFileName()}");
                traceLog.AppendLine($"{indentStr}Class Name : {frame.GetMethod().ReflectedType.Name}");
                traceLog.AppendLine($"{indentStr}Method Name : {frame.GetMethod()}");
                traceLog.AppendLine($"{indentStr}Line Number : {frame.GetFileLineNumber()}");
                traceLog.AppendLine($"=======================================================");
            }

            return traceLog.ToString();
        }
    }
}
