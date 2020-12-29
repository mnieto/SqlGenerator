using SqlGenerator.Discover;
using SqlGenerator.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGenerator
{
    /// <summary>
    /// Indications to load data and generate the SQL scripts
    /// </summary>
    public class Specification {
        private int _rowsToScan = 50;

        /// <summary>
        /// Table name used to generate the SQL scripts. If ommited, can be infered by the <see cref="IFieldDefStrategy"/>
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Connection string to get the table scheme from. This property is mandatory if <see cref="DiscoverStrategy.ConnectToDatabase"/> strategy is used
        /// </summary>
        public string ConnectionString { get; set; }


        /// <summary>
        /// Which stratgy will follow to guess field names and data types. Default <see cref="DiscoverStrategy.FieldDefDescriptor"/>
        /// </summary>
        public DiscoverStrategy DiscoverStrategy { get; set; } = DiscoverStrategy.FieldDefDescriptor;

        /// <summary>
        /// Number of rows to scan to try to guess the fields data type
        /// </summary>
        /// <remarks>
        /// If the value of <see cref="DiscoverStrategy"/> is <see cref="DiscoverStrategy.ConnectToDatabase"/> or <see cref="DiscoverStrategy.FieldDefDescriptor"/> 
        /// this value is ignored. The value must be grater than 0. Default value is 50.
        /// </remarks>
        public int RowsToScan {
            get { return _rowsToScan; }
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Value must be greater than 0.");
                _rowsToScan = value;
            }
        }

        /// <summary>
        /// For Excel files, thw worksheet that contains the source data. If not specified, the first worksheet (by index) is taken
        /// </summary>
        public string WorkSheetName { get; set; }
    }
}
