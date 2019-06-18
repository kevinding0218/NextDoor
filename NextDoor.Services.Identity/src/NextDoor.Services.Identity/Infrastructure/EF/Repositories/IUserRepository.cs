using NextDoor.Core.MsSql;
using NextDoor.Services.Identity.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Infrastructure.EF.Repositories
{
    public interface IUserRepository : IMsSqlRepository<User>
    {
    }
}
