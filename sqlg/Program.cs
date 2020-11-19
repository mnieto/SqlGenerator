using System;
using System.IO;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            //Configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("settings.json");
            if (configFile != null) {
                builder.AddJsonFile(configFile);
            }
            var config = builder.Build();


            //Dependency Injection
            IServiceCollection services = new ServiceCollection();
            services.AddOptions();
            if (configFile != null) {
                services.Configure<Specification>(config.GetSection("Specification"));
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