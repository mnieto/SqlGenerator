using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CommandLineParser.Configuration
{
    public static class CommandLineConfigurationExtensions
    {

        /// <summary>
        /// Adds the command line configuration source to the .NET configuration system, using <see cref="CmdLineOptionAttribute"/>
        /// to build the mapping between the parsed command line arguments and the configuration object
        /// </summary>
        /// <typeparam name="TCmdOptions">Class that CommandLineParser will map args to</typeparam>
        /// <typeparam name="TConfiguration">Configuration entity</typeparam>
        /// <param name="builder">Configuration builder</param>
        /// <param name="options">object with the parsed arguments</param>
        /// <returns></returns>
        public static IConfigurationBuilder AddCommandLineConfiguration<TCmdOptions, TConfiguration>(
            this IConfigurationBuilder builder,
            TCmdOptions options) 
        where TCmdOptions : class where TConfiguration : class
        {
            builder.Add(new CommandLineConfigurationSource<TCmdOptions, TConfiguration>(options));
            return builder;
        }


        /// <summary>
        /// Adds the command line configuration source to the .NET configuration system, 
        /// using fluent <see cref="CommandLineMapper{TCmdOptions, TConfiguration}"/> to configure the mapping 
        /// between the parsed command line arguments and the configuration object
        /// </summary>
        /// <typeparam name="TCmdOptions"></typeparam>
        /// <typeparam name="TConfiguration"></typeparam>
        /// <param name="builder">Configuration builder</param>
        /// <param name="options">object with the parsed arguments</param>
        /// <param name="mapper">Lambda action that configures the mappings</param>
        public static IConfigurationBuilder AddCommandLineConfiguration<TCmdOptions, TConfiguration>(
            this IConfigurationBuilder builder,
            TCmdOptions options,
            Action<CommandLineMapper<TCmdOptions, TConfiguration>> mapper) 
        where TCmdOptions : class where TConfiguration : class
        {
            builder.Add(new CommandLineConfigurationSource<TCmdOptions, TConfiguration>(options, mapper));
            return builder;
        }

        internal static bool IsDefaultValue(this PropertyInfo pi, object value) {
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
