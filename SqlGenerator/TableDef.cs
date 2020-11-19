using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGenerator
{
    public class TableDef
    {
        public string TableName { get; set; }
        public List<FieldDef> Fields { get; set; }
        public List<string> Keys { get; set; }

    }
}
