using Microsoft.EntityFrameworkCore;
using NextDoor.Core.MsSql;
using NextDoor.Core.Types.Domain;
using NextDoor.Services.Identity.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Infrastructure.EF.Repositories
{
    public class UserRepository : MsSqlRepository<User>, IMsSqlRepository<User>
    {
        public UserRepository(NextDoorDbContext dbContext)
            : base(dbContext)
        {
        }

        public UserRepository(IUserInfo userInfo, NextDoorDbContext dbContext)
            : base(dbContext, userInfo)
        {
        }
    }
}
