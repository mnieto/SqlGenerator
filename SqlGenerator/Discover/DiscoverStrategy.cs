using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGenerator.Discover
{
    public enum DiscoverStrategy
    {
        Auto = 0,
        ConnectToDatabase = 1,
        FieldDefDescriptor = 2,
        FieldNamesOnly = 3,
        GuestDataType = 4
    }
}
