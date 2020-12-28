using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGenerator
{
    public interface IGenerator
    {
        IReader Reader { get; }
        TableDef TableDef { get; }

        string GenerateSQL();

        void Generate(TextWriter writer);

    }
}
