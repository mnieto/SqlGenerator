using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("CommandLineParser.Configuration.Test")]

namespace CommandLineParser.Configuration
{
    /// <summary>
    /// Configration provider to translate legible command line options to standarized configuraiton format
    /// </summary>
    /// <typeparam name="TCmdOptions">A class with the binded the command line options</typeparam>
    /// <typeparam name="TConfiguration">A class with the binded configuration options</typeparam>
    public class CommandLineConfigurationProvider<TCmdOptions, TConfiguration>
        : ConfigurationProvider
        where TCmdOptions : class
        where TConfiguration : class
    {
        private readonly TCmdOptions _options;
        private Action<CommandLineMapper<TCmdOptions, TConfiguration>> _mapperAction;

        public CommandLineConfigurationProvider(TCmdOptions options) : this(options, null) { }

        public CommandLineConfigurationProvider(TCmdOptions options, Action<CommandLineMapper<TCmdOptions, TConfiguration>> mapper) {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _mapperAction = mapper;
        }


        public override void Load() {
            if (_mapperAction == null) {
                BindProperties(_options);
            } else {
                var mapper = new CommandLineMapper<TCmdOptions, TConfiguration>(_options);
                _mapperAction(mapper);
                BindMappedProperties(mapper);
            }
        }

        public override string ToString() {
            return nameof(CommandLineConfigurationProvider<TCmdOptions, TConfiguration>);
        }

        private void BindProperties(TCmdOptions options) {

            foreach (var pi in options.GetType().GetProperties()) {
                var cmdAttribute = pi.GetCustomAttribute<CmdLineOptionAttribute>();
                if (cmdAttribute != null) {
                    if (!pi.IsDefaultValue(pi.GetValue(options))) {
                        string path = cmdAttribute.GetPropertyPath();
                        Set(path, pi.GetValue(options).ToString());
                    }
                }
            }
        }

        private void BindMappedProperties(CommandLineMapper<TCmdOptions, TConfiguration> mapper) {
            Data = mapper.GetSettings();
        }

    }
}
