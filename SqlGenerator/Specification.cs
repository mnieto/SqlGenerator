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
        /// Depending on the value of <see cref="DiscoverStrategy"/>
        /// <list type="bullet">
        /// <item><term><see cref="DiscoverStrategy.GuestDataType"/></term>
        /// <description>This field is mandatory/></description>
        /// </item>
        /// <item><term><see cref="DiscoverStrategy.FieldNamesOnly"/></term>
        /// <description>
        /// If this field has no value or it is 0, <see cref="FieldType"/> will be <see cref="FieldType.Auto"/> 
        /// and data type will be guessed in each occurrence. If has a value greather than 0, field data types will be
        /// guessed scaning up to the specified row
        /// </description>
        /// </item>
        /// </list>
        /// </remarks>
        public int? RowsToScan { get; set; }

        /// <summary>
        /// For Excel files, thw worksheet that contains the source data. If not specified, the first worksheet (by index) is taken
        /// </summary>
        public string WorkSheetName { get; set; }
    }
}
