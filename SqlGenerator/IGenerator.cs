using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGenerator
{
    /// <summary>
    /// Generate SQL sentences from the data source
    /// </summary>
    public interface IGenerator
    {
        /// <summary>
        /// Reads data from data soruce
        /// </summary>
        IReader Reader { get; }
        
        /// <summary>
        /// Contains filed information neccesary to format the SQL sentences
        /// </summary>
        TableDef TableDef { get; }

        /// <summary>
        /// Generates a SQL sentence from the curret <see cref="IReader"/> row.
        /// </summary>
        /// <returns></returns>
        string GenerateSQL();

        /// <summary>
        /// Generates the SQL sentences from the datasource and writes into an stream
        /// </summary>
        /// <param name="writer">Stream where SQL sentences are written to</param>
        void Generate(TextWriter writer);

    }
}
