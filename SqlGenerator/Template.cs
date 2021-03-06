﻿using SqlGenerator.Discover;
using SqlGenerator.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGenerator
{
    /// <summary>
    /// Specification to load data and generate the SQL scripts
    /// </summary>
    public class Template {
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
        /// Which stratgy will follow to guess field names and data types. Default <see cref="DiscoverStrategy.Auto"/>
        /// </summary>
        public DiscoverStrategy DiscoverStrategy { get; set; } = DiscoverStrategy.Auto;


        /// <summary>
        /// List of field definitions
        /// </summary>
        /// <remarks>
        /// <para>If not specified, fields are infered from the source file. Also, if no ordinal position specified for each 
        /// <see cref="FieldDef"/>, fields are assigned by creation order</para>
        /// </remarks>
        public List<FieldDef> Fields { get; private set; } = new List<FieldDef>();

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
