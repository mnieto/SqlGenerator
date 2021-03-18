using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CommandLineParser.Configuration
{
    /// <summary>
    /// Maps a configuration properties to configuration properties
    /// </summary>
    /// <typeparam name="TCmdOptions">Type with commandline options</typeparam>
    /// <typeparam name="TConfiguration">Type with configuration settings</typeparam>
    public class CommandLineMapper<TCmdOptions, TConfiguration> where TCmdOptions : class 
                                                                where TConfiguration : class {

        private TCmdOptions _options;
        private Dictionary<string, MappedConfig> Data = new Dictionary<string, MappedConfig>();

        public CommandLineMapper(TCmdOptions options) {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }
        
        /// <summary>
        /// Maps a commandline option to a property in the configuration settings
        /// </summary>
        /// <typeparam name="T">Type of the commandline option</typeparam>
        /// <param name="optionsProperty">property in the options object to be mapped</param>
        /// <param name="configurationProperty">property in the settings to be mapped to</param>
        /// <Example>
        /// <code>
        /// configurator.Map(x => x.DeliveryStreet, x => x.DeliveryAddress.Street);
        /// </code>
        /// </Example>
        /// <remarks>
        /// The configuration property type must match with the type of the commandline option
        /// </remarks>
        public CommandLineMapper<TCmdOptions, TConfiguration> Map<T>(Expression<Func<TCmdOptions, T>> optionsProperty, 
                                                                     Expression<Func<TConfiguration, T>> configurationProperty) {

            string path = optionsProperty.Body.ToString();
            string optionName = path.Substring(path.IndexOf('.') + 1);

            path = configurationProperty.Body.ToString();
            path = configurationProperty.Parameters[0].Type.Name + path.Substring(path.IndexOf('.'));
            path = path.Replace(".", ConfigurationPath.KeyDelimiter);

            T value = optionsProperty.Compile().Invoke(_options);

            var pi = _options.GetType().GetProperty(optionName);

            Data.Add(optionName, new MappedConfig {
                OptionName = optionName,
                Path = path,
                Value = value,
                IsDefaultValue = pi.IsDefaultValue(value)
            });
            
            return this;

        }


        /// <summary>
        /// For test purposes. Returns the path to the setting
        /// </summary>
        internal string Path(string optionName) {
            return Data[optionName].Path;
        }

        /// <summary>
        /// For test purposes. Returns the value of the option set in the command line
        /// </summary>
        internal object Value(string optionName) {
            return Data[optionName].Value;
        }

        /// <summary>
        /// Returns a dictionarly of path, value. Required by the ConfigurationProvider
        /// </summary>
        internal Dictionary<string, string> GetSettings() {
            return Data.Values
                .Where(x => x.IsDefaultValue == false)
                .ToDictionary(x => x.Path, x => x.Value?.ToString());
        }

    }

    internal class MappedConfig
    {
        public string OptionName { get; set; }
        public string Path { get; set; }
        public object Value { get; set; }
        public bool IsDefaultValue { get; set; }
    }

}
