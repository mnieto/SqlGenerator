using System.Collections.Generic;

namespace SqlGenerator.Discover
{
    internal interface IFieldDefStrategy
    {
        ParseResult GetFieldDef(string fieldName, IEnumerable<string> data = null);
    }
}