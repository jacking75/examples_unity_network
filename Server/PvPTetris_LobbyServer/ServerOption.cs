using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobbyServer
{
    public class ServerOption
    {
        [Option( "serverIndex", Required = true, HelpText = "Server Index")]
        public int Index { get; set; }

        [Option("name", Required = true, HelpText = "Server Name")]
        public string Name { get; set; }

        [Option("maxConnectionNumber", Required = true, HelpText = "MaxConnectionNumber")]
        public int MaxConnectionNumber { get; set; }

        [Option("port", Required = true, HelpText = "Port")]
        public int Port { get; set; }

        [Option("maxRequestLength", Required = true, HelpText = "maxRequestLength")]
        public int MaxRequestLength { get; set; }

        [Option("receiveBufferSize", Required = true, HelpText = "receiveBufferSize")]
        public int ReceiveBufferSize { get; set; }

        [Option("sendBufferSize", Required = true, HelpText = "sendBufferSize")]
        public int SendBufferSize { get; set; }

        [Option("lobbyMaxCount", Required = true, HelpText = "Max Lobby Count")]
        public int LobbyMaxCount { get; set; } = 0;

        [Option("lobbyMaxUserCount", Required = true, HelpText = "Max Lobby User Count")]
        public int LobbyMaxUserCount { get; set; } = 0;

        [Option("lobbyStartNumber", Required = true, HelpText = "Start Lobby Number")]
        public int LobbyStartNumber { get; set; } = 0;

    }    
}
