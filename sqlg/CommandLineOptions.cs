using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace sqlg
{
    class CommandLineOptions
    {
        [Option('s', "source", HelpText = "Imput file with data")]
        public string Source { get; set; }

        [Option('t', "target",HelpText = "Output file with sql script")]
        public string Target { get; set; }

        [Option('c', "config", HelpText = "Configuration settings file")]
        public string ConfigFile { get; set; }
    }
}
