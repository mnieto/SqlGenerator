using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGenerator.Sources
{
    public interface ISource
    {
        string TableName { get; }

        TableDef TableDef { get; }
        void Load(string fileName);

    }
}
