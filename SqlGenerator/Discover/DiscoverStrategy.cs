using SqlGenerator.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGenerator.Discover
{
    /// <summary>
    /// Which strategy will follow the application to guess field name, data types and other field properties. Default <see cref="DiscoverStrategy.FieldDefDescriptor"/>
    /// </summary>
    public enum DiscoverStrategy
    {
        /// <summary>
        /// Try to do the best to guess the information
        /// </summary>
        /// <remarks>
        /// If there is a connections string it will use it to get the scheme information.
        /// If there is not a connection string, try to parse field names as field descriptors. If it encounter errors,
        /// then it try the get only field names and guess data types
        /// </remarks>
        Auto = 0,

        /// <summary>
        /// Uses then <see cref="Template.ConnectionString"/> and retrieves the table schema
        /// </summary>
        /// <remarks>
        /// If <see cref="Template.ConnectionString"/> is empty an exception is thrown in the <see cref="ISource.Load(string)"/> method call
        /// </remarks>
        ConnectToDatabase = 1,

        /// <summary>
        /// Fields have a specific descriptor that allow to infer the neccesary information about field name, data type and properties.
        /// </summary>
        /// <seealso cref="FieldDefParserStrategy"/>
        FieldDefDescriptor = 2,

        /// <summary>
        /// Get a sample of data of max <see cref="Template.RowsToScan"/> rows and, for each column try to guess the data type and other properties
        /// </summary>
        GuessDataType = 3
    }
}
