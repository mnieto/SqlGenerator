using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGenerator
{
    /// <summary>
    /// Field definition metadata
    /// </summary>
    public class FieldDef
    {
        /// <summary>
        /// Field name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Number of column where the field is defined
        /// </summary>
        public int OrdinalPosition { get; set; }

        /// <summary>
        /// MaxLenght. If type is other <see cref="FieldType.Text"/> this property is ignored
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// Field is nullable. Default <c>true</c>
        /// </summary>
        public bool IsNullable { get; set; } = true;

        /// <summary>
        /// Field is part of the primary key
        /// </summary>
        public bool IsKey { get; set; }

        /// <summary>
        /// Field data type. See types: <see cref="FieldType"/>
        /// </summary>
        public FieldType FieldType { get; set; }

        /// <summary>
        /// Format for <see cref="FieldType.Bool"/> and <see cref="FieldType.DateTime"/>. Ignored for other data types
        /// </summary>
        public string Format { get; set; }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append($"[{Name}]:{FieldType}, ");
            sb.Append(IsNullable ? "Nullable" : "not nullable");
            sb.Append($", PK={IsKey}");
            sb.Append($", MaxLength={MaxLength}");
            sb.Append($", Format={Format}");
            return sb.ToString();
        }
    }
}
