using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGenerator.Discover
{
    internal class DiscoverFieldDefFactory : IDiscoverFieldDefFactory
    {

        private IServiceProvider Services { get; set; }
        public DiscoverFieldDefFactory(IServiceProvider serviceProvider) {
            Services = serviceProvider;
        }
        public IFieldDefStrategy GetDiscoverStrategy(DiscoverStrategy strategy) =>
            strategy switch {
                DiscoverStrategy.Auto =>
                    (IFieldDefStrategy)Services.GetService(typeof(FieldDefParserStrategy)),
                DiscoverStrategy.UseTemplate =>
                    throw new InvalidOperationException($"{DiscoverStrategy.UseTemplate} strategy do not use discover fieldDef"),
                DiscoverStrategy.ConnectToDatabase =>
                   throw new NotImplementedException(),
                DiscoverStrategy.FieldDefDescriptor =>
                    (IFieldDefStrategy)Services.GetService(typeof(FieldDefParserStrategy)),
                DiscoverStrategy.GuessDataType =>
                    (IFieldDefStrategy)Services.GetService(typeof(SamplingStrategy)),

                _ => throw new NotImplementedException()
            };

    }
}
