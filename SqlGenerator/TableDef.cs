using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGenerator
{
    /// <summary>
    /// Table definition schema
    /// </summary>
    public class TableDef
    {
        /// <summary>
        /// Name of the table
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Collection of <see cref="FieldDef"/>
        /// </summary>
        public List<FieldDef> Fields { get; } = new List<FieldDef>();

        /// <summary>
        /// Collection of field names that belong to the primary key. Used to generate Update SQL scripts
        /// </summary>
        public IEnumerable<string> Keys => Fields.Where(x => x.IsKey).Select(x => x.Name);

    }
}
