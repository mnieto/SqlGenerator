using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandLineParser.Configuration
{

    [AttributeUsage(AttributeTargets.Property)]
    public class CmdLineOptionAttribute : Attribute
    {
        private readonly Type _rootType;
        private readonly IEnumerable<string> _properties;

        public CmdLineOptionAttribute(Type configType, string propertyName) : this(configType, new List<string>() { propertyName }) { }

        public CmdLineOptionAttribute(Type configType, params string[] properties) : this(configType, properties.ToList()) { }

        public CmdLineOptionAttribute(Type configType, IEnumerable<string> properties) {
            _rootType = configType;
            _properties = properties;
        }

        public string GetPropertyPath() {
            Type currentType = _rootType;
            string path = currentType.Name;
            foreach (string propertyName in _properties) {
                var pi = currentType.GetProperty(propertyName);
                if (pi == null) {
                    throw new ArgumentException($"{propertyName} is not a public property of {nameof(currentType.Name)}.");
                }
                path += ConfigurationPath.KeyDelimiter + propertyName;
                currentType = pi.PropertyType;
            }
            return path;
        }

    }
}