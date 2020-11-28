using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGenerator.Sources
{
    public class SourceFactory
    {
        private readonly IServiceProvider serviceProvider;
        private ILogger<SourceFactory> Logger { get; set; }
        public SourceFactory(IServiceProvider serviceProvider, ILogger<SourceFactory> logger) {
            this.serviceProvider = serviceProvider;
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

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
