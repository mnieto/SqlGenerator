﻿using Microsoft.Extensions.DependencyInjection;
using SqlGenerator.Discover;
using SqlGenerator.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("SqlGenerator.Test")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace SqlGenerator
{
    /// <summary>
    /// Dependency injection extensions for SqlGenerator
    /// </summary>
    public static class SqlGeneratorExtensions
    {
        /// <summary>
        /// Register the neccesary DI in the services colection
        /// </summary>
        public static void AddSqlGenerator(this IServiceCollection services) {
            services.AddTransient<SourceFactory>();
            services.AddTransient<ExcelSource>();
            services.AddTransient<ISource, ExcelSource>(s => s.GetService<ExcelSource>());
            services.AddTransient<IDiscoverFieldDefFactory, DiscoverFieldDefFactory>();
            services.AddTransient<SamplingStrategy>();
            services.AddTransient<FieldDefParserStrategy>();
            services.AddTransient<IFieldDefStrategy, SamplingStrategy>(s => s.GetService<SamplingStrategy>());
            services.AddTransient<IFieldDefStrategy, FieldDefParserStrategy>(s => s.GetService<FieldDefParserStrategy>());
            
            services.AddTransient<IReader, ExcelSource>(s => s.GetService<ExcelSource>());
            
            services.AddTransient<IGeneratorFactory, GeneratorFactory>();
            services.AddTransient<IGenerator, InsertGenerator>();
        }
    }
}
