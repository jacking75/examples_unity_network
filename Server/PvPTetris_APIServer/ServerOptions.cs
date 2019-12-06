using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TetrisApiServer
{
    public class ServerOptions
    {
        [Option("redisName", Required = true, HelpText = "Redis Server Name")]
        public string RedisName { get; set; }

        [Option("redisAddress", Required = true, HelpText = "Redis Server Address")]
        public string RedisAddress { get; set; }
        
    }
}
