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

        public SourceFactory(IServiceProvider serviceProvider) {
            this.serviceProvider = serviceProvider;
        }

        public ISource GetSource(string extension) {
            switch (extension.ToLower()) {
                case "xlsx":
                    return (ISource)serviceProvider.GetService(typeof(ExcelSource));
                default:
                    throw new ArgumentException($"Unknown file exception: '{extension}'.");
            }
        }
    }
}
