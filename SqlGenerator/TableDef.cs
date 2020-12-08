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
        /// Return <see cref="FieldDef"/> by position
        /// </summary>
        /// <param name="i">Field position</param>
        public FieldDef this[int i] => Fields[i];

        /// <summary>
        /// Return <see cref="FieldDef"/> by name
        /// </summary>
        /// <param name="name">name of the matching field</param>
        public FieldDef this[string name] => Fields.First(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));

        /// <summary>
        /// Collection of field names that belong to the primary key. Used to generate Update SQL scripts
        /// </summary>
        public IEnumerable<string> Keys => Fields.Where(x => x.IsKey).Select(x => x.Name);

    }
}
