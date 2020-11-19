using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGenerator.Sources
{
    class ExcelSource : ISource
    {
        public TableDef TableDef { get; }

        public void Load(string fileName) {
            throw new NotImplementedException();
        }
    }
}
