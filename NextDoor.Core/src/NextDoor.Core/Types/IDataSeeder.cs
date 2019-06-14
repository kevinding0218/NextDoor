using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NextDoor.Core.Types
{
    public interface IDataSeeder
    {
        Task SeedAsync();
    }
}
