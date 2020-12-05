using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGenerator.Sources
{
    /// <summary>
    /// Iterface for all data sources
    /// </summary>
    public interface ISource
    {
        
        /// <summary>
        /// Name of the table
        /// </summary>
        string TableName { get; }

        
        /// <summary>
        /// Table schema (or definition). See <see cref="TableDef"/>
        /// </summary>
        TableDef TableDef { get; }

        /// <summary>
        /// Load the information from de source file
        /// </summary>
        /// <param name="fileName">File name with the data</param>
        void Load(string fileName);

    }
}
