using SqlGenerator.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGenerator
{
    /// <summary>
    /// Defines reader methods to get data from a <see cref="ISource"/>
    /// </summary>
    public interface IReader : IRecord
    {
        /// <summary>
        /// Read next record of data
        /// </summary>
        /// <returns><c>True</c> if there is more data. <c>False</c> if there is no more rows to read</returns>
        bool Read();

    }
}
