using System;
using System.IO;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CommandLineParser.Configuration;
using Serilog;
using SqlGenerator;

namespace sqlg {

    public class Program
    {

        public static void Main(string[] args) {

            //Command line
            string configFile = null;
            var arguments = Parser.Default.ParseArguments<CommandLineOptions>(args);
            arguments.WithParsed(opts => {
                if (!string.IsNullOrEmpty(opts.ConfigFile)) {
                    configFile = opts.ConfigFile;
                }
            });

            //Configuration, en this order
            //1st: application settins
            //2nd: template file
            //3rd: command line options
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("settings.json");
            if (configFile != null) {
                builder.AddJsonFile(configFile);
            }
            arguments.WithParsed(cmdOptions => {
                builder.AddCommandLineConfiguration<CommandLineOptions, Template>(cmdOptions, mapper => {
                    mapper
                        .Map(x => x.TableName, x => x.TableName)
                        .Map(x => x.WorkSheetName, x => x.WorkSheetName);
                });
            });
            var config = builder.Build();

            //Dependency Injection
            IServiceCollection services = new ServiceCollection();
            services.AddOptions();
            if (configFile != null || config.Get<Template>() != null) {
                services.Configure<Template>(config.GetSection("Template"));
            }
            services.AddSqlGenerator();
            services.AddSingleton<ApplicationService>();


            //Logging
            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();
            services.AddLogging(builder => builder.AddSerilog(logger));

            //Build services
            var appServices = services.BuildServiceProvider();


            //Run
            arguments.WithParsed(o => {
                var mainService = appServices.GetService<ApplicationService>();
                mainService.Run(o);
            });

        }
    }
}