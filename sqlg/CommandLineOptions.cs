using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLineParser.Configuration;
using SqlGenerator;

namespace sqlg
{
    class CommandLineOptions
    {
        [Option('s', "source", HelpText = "Imput file with data")]
        public string Source { get; set; }

        [Option('t', "target",HelpText = "Output file with sql script")]
        public string Target { get; set; }

        [Option('c', "config", HelpText = "Template file with configuration settings")]
        public string ConfigFile { get; set; }

        [Option("table-name", HelpText = "Output table name")]
        [CmdLineOption(typeof(Template), "TableName")]
        public string TableName { get; set; }

        [Option("ws-name", HelpText = "Worksheet name. If not specified, first worksheet is taken")]
        [CmdLineOption(typeof(Template), "WorkSheetName")]
        public string WorkSheetName { get; set; }

    }
}
