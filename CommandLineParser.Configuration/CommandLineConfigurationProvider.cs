using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommandLineParser.Configuration
{
    /// <summary>
    /// Configration provider to translate legible command line options to standarized configuraiton format
    /// </summary>
    /// <typeparam name="TCmdOptions">A class with the binded the command line options</typeparam>
    /// <typeparam name="TConfiguration">A class with the binded configuration options</typeparam>
    public class CommandLineConfigurationProvider<TCmdOptions, TConfiguration> : ConfigurationProvider
    {
        private readonly TCmdOptions _options;

        public CommandLineConfigurationProvider(TCmdOptions options) {
            _options = options;
        }

        public override void Load() {
            BindProperties(_options);
        }

        public override string ToString() {
            return nameof(CommandLineConfigurationProvider<TCmdOptions, TConfiguration>);
        }

        private void BindProperties(TCmdOptions options) {

            foreach (var pi in options.GetType().GetProperties()) {
                var cmdAttribute = pi.GetCustomAttribute<CmdLineOptionAttribute>();
                if (cmdAttribute != null) {
                    if (!IsDefaultValue(pi, pi.GetValue(options))) {
                        string path = cmdAttribute.GetPropertyPath();
                        Set(path, pi.GetValue(options).ToString());
                    }
                }
            }
        }

        private bool IsDefaultValue(PropertyInfo pi, object value) {
            var att = pi.GetCustomAttribute<CommandLine.OptionAttribute>();
            if (att?.Default != null) {
                return value.Equals(att.Default);
            }
            Type type = pi.PropertyType;
            if (type.IsValueType) {
                return Activator.CreateInstance(type).Equals(value);
            }
            return value == null;
        }

    }
}
