using Microsoft.Extensions.DependencyInjection;
using SqlGenerator.Discover;
using SqlGenerator.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("SqlGenerator.Test")]
namespace SqlGenerator
{
    public static class SqlGeneratorExtensions
    {
        public static void AddSqlGenerator(this IServiceCollection services) {
            services.AddTransient<SourceFactory>();
            services.AddTransient<ISource, ExcelSource>(s => s.GetService<ExcelSource>());
            services.AddTransient<IDiscoverFieldDefFactory, DiscoverFieldDefFactory>();
            services.AddTransient<IFieldDefStrategy, SamplingStrategy>(s => s.GetService<SamplingStrategy>());
            services.AddTransient<IFieldDefStrategy, FieldDefParserStrategy>(s => s.GetService<FieldDefParserStrategy>());
        }
    }
}
