using Microsoft.Extensions.DependencyInjection;
using SqlGenerator.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGenerator
{
    public static class SqlGeneratorExtensions
    {
        public static void AddSqlGenerator(this IServiceCollection services) {
            services.AddTransient<SourceFactory>();
            services.AddTransient<ISource, ExcelSource>(s => s.GetService<ExcelSource>());
        }
    }
}
