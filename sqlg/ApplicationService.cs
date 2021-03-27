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
    internal class ApplicationService {

        private ILogger<ApplicationService> _logger;
        private Template _template;
        private IGeneratorFactory GeneratorFactory {get; set; }
        private SourceFactory SourceFactory { get; set; }

        public ApplicationService(ILogger<ApplicationService> logger, IOptions<Template> template, IGeneratorFactory generatorFactory, SourceFactory sourceFactory) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _template = template?.Value ?? throw new ArgumentNullException(nameof(template));
            GeneratorFactory = generatorFactory ?? throw new ArgumentNullException(nameof(generatorFactory));
            SourceFactory = sourceFactory ?? throw new ArgumentNullException(nameof(sourceFactory));
        }

        public void Run(CommandLineOptions options) {

            if (!File.Exists(options.Source)) {
                _logger.LogError($"File not found: {options.Source}");
                return;
            }
            ISource source = SourceFactory.GetSource(Path.GetExtension(options.Source).Substring(1));

            _logger.LogInformation($"TableName: {_template.TableName}");
            _logger.LogInformation($"Source: {options.Source}");
            _logger.LogInformation($"Target: {options.Target}");
            _logger.LogInformation($"WorksheetName: {options.WorkSheetName}");

            source.Load(options.Source);

            var generator = GeneratorFactory.GetGenerator(source.GetReader(), source.TableDef);
            TextWriter output;
            if (string.IsNullOrEmpty(options.Target))
                output = Console.Out;
            else
                output = new StreamWriter(options.Target);
            generator.Generate(output);

        }
    }
}