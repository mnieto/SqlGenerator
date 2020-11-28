using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using SqlGenerator.Sources;
using SqlGenerator;

namespace sqlg
{
    internal class ApplicationService
    {

        private ILogger<ApplicationService> _logger;
        private Specification _specification;
        private SourceFactory SourceFactory { get; set; }

        public ApplicationService(ILogger<ApplicationService> logger, IOptions<Specification> specification, SourceFactory sourceFactory) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _specification = specification?.Value ?? throw new ArgumentNullException(nameof(specification));
            SourceFactory = sourceFactory ?? throw new ArgumentNullException(nameof(sourceFactory));

        }
        public void Run(CommandLineOptions options) {

            if (!File.Exists(options.Source)) {
                _logger.LogError($"File not found: {options.Source}");
                return;
            }
            ISource source = SourceFactory.GetSource(Path.GetExtension(options.Source).Substring(1));

            _logger.LogInformation($"TableName: {_specification.TableName}");
            _logger.LogInformation($"Source: {options.Source}");
            _logger.LogInformation($"Target: {options.Target}");
        }
    }
}