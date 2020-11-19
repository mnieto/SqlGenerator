using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace sqlg
{
    internal class ApplicationService
    {

        private ILogger<ApplicationService> _logger;
        private Specification _specification;

        public ApplicationService(ILogger<ApplicationService> logger, IOptions<Specification> specification) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _specification = specification?.Value ?? throw new ArgumentNullException(nameof(specification));

        }
        public void Run(CommandLineOptions options) {
            _logger.LogInformation($"TableName: {_specification.TableName}");
            _logger.LogInformation($"Source: {options.Source}");
            _logger.LogInformation($"Target: {options.Target}");
        }
    }
}