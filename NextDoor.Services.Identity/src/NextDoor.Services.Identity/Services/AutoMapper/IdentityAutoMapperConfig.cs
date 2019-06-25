using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Services.AutoMapper
{
    public class IdentityAutoMapperConfig
    {
        public static void Configure()
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile<IdentityMappings>();
            });
        }
    }
}
