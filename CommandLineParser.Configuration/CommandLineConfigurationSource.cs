using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommandLineParser.Configuration
{
    public class CommandLineConfigurationSource<TCmdOptions, TConfiguration> : IConfigurationSource
    {
        TCmdOptions _options;
        public CommandLineConfigurationSource(TCmdOptions options) {
            _options = options;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder) {
            return new CommandLineConfigurationProvider<TCmdOptions, TConfiguration>(_options);
        }
    }
}
