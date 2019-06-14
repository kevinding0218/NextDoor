using System;
using System.Collections.Generic;
using System.Text;

namespace NextDoor.Core.MSSQL
{
    public class MsSqlDbOptions
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public bool InMemory { get; set; }
        public bool Seed { get; set; }
    }
}
