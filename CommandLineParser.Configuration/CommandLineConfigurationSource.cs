using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommandLineParser.Configuration
{
    /// <summary>
    /// Represents a source of configuration key/values for command line options
    /// </summary>
    /// <typeparam name="TCmdOptions">Class that CommandLineParser will map args to</typeparam>
    /// <typeparam name="TConfiguration">Configuration entity</typeparam>
    public class CommandLineConfigurationSource<TCmdOptions, TConfiguration> : IConfigurationSource
        where TCmdOptions : class
        where TConfiguration : class
    {
        private TCmdOptions _options;
        private Action<CommandLineMapper<TCmdOptions, TConfiguration>> _mapperAction;

        /// <summary>
        /// Constructor that will use <see cref="CmdLineOptionAttribute"/> attribute to map command line options with configuration object
        /// </summary>
        /// <param name="options">Object with parsed command line arguments</param>
        public CommandLineConfigurationSource(TCmdOptions options) : this (options, null) { }

        /// <summary>
        /// Constructor that will use fluent <see cref="CommandLineMapper{TCmdOptions, TConfiguration}"/> to configure the mapping 
        /// between the parsed command line arguments and the configuration object
        /// </summary>
        /// <param name="options">Object with parsed command line arguments</param>
        /// <param name="mapper">Lambda action that configures the mappings</param>
        public CommandLineConfigurationSource(TCmdOptions options, Action<CommandLineMapper<TCmdOptions, TConfiguration>> mapper) {
            _options = options;
            _mapperAction = mapper;
        }

        /// <summary>
        /// Builds the Microsoft.Extensions.Configuration.IConfigurationProvider for this source
        /// </summary>
        public IConfigurationProvider Build(IConfigurationBuilder builder) {
            return new CommandLineConfigurationProvider<TCmdOptions, TConfiguration>(_options, _mapperAction);
        }
    }
}
