using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGenerator.Sources
{
    /// <summary>
    /// Selects the class that will read the soruce data from the file extension
    /// </summary>
    public class SourceFactory
    {
        private readonly IServiceProvider serviceProvider;
        private ILogger<SourceFactory> Logger { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceProvider">DI service provider</param>
        /// <param name="logger">Logger</param>
        public SourceFactory(IServiceProvider serviceProvider, ILogger<SourceFactory> logger) {
            this.serviceProvider = serviceProvider;
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get the specific <see cref="ISource"/> from the file extension
        /// </summary>
        /// <param name="extension">file extension</param>
        /// <returns><see cref="ISource"/></returns>
        public ISource GetSource(string extension) {
            Logger.LogDebug($"Selecting source type by file extension: {extension}");
            switch (extension.ToLower()) {
                case "xlsx":
                    return (ISource)serviceProvider.GetService(typeof(ExcelSource));
                default:
                    throw new ArgumentException($"Unknown file type: '{extension}'.");
            }
        }
    }
}
