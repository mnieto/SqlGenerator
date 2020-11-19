using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGenerator
{
    public partial class FieldDef
    {
        public string Name { get; set; }
        public int MaxLength { get; set; }
        public bool IsNullable { get; set; }
        public FieldType FieldType { get; set; }
    }
}
