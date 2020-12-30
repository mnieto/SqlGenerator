using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SqlGenerator
{

    /// <summary>
    /// Generator factory to create Insert, Update... generator
    /// </summary>
    public class GeneratorFactory : IGeneratorFactory
    {
        private IServiceProvider Services { get; set; }
        private ILogger<GeneratorFactory> logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceProvider">DI service</param>
        /// <param name="logger">Logger used in the <see cref="IGenerator"/> implementation</param>
        public GeneratorFactory(IServiceProvider serviceProvider, ILogger<GeneratorFactory> logger) {
            Services = serviceProvider;
            this.logger = logger;
        }

        /// <summary>
        /// Return the correct <see cref="IGenerator"/> implementation
        /// </summary>
        public IGenerator GetGenerator(IReader reader, TableDef tableDef, DataSourceInformation options) {
            return new InsertGenerator(reader, logger, tableDef, options);
        }

        /// <summary>
        /// Return the correct <see cref="IGenerator"/> implementation
        /// </summary>
        public IGenerator GetGenerator(IReader reader, TableDef tableDef, Action<DataSourceInformation> options = null) {
            return new InsertGenerator(reader, logger, tableDef, options);
        }
    }
}
