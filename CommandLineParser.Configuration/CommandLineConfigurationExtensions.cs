using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommandLineParser.Configuration
{
    public static class CommandLineConfigurationExtensions
    {
        public static IConfigurationBuilder AddCommandLineConfiguration<TCmdOptions, TConfiguration>(
            this IConfigurationBuilder builder,
            TCmdOptions options) 
        {
            builder.Add(new CommandLineConfigurationSource<TCmdOptions, TConfiguration>(options));
            return builder;
        }
    }
}
