using NextDoor.Services.Operations.Dto;
using System;
using System.Threading.Tasks;

namespace NextDoor.Services.Operations.Services
{
    public interface IOperationsStorage
    {
        Task<OperationDto> GetAsync(Guid id);

        Task SetAsync(Guid id, int userId, string name, OperationState state,
            string resource, string code = null, string reason = null);
    }
}
