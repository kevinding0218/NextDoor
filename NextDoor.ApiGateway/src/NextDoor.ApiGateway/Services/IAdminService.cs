using NextDoor.ApiGateway.Dto.Admin;
using NextDoor.ApiGateway.Messages.Queries.Admin;
using NextDoor.Core.Types.Pagination;
using RestEase;
using System.Threading.Tasks;

namespace NextDoor.ApiGateway.Services
{
    [SerializationMethods(Query = QuerySerializationMethod.Serialized)]
    public interface IAdminService
    {
        /// <summary>
        /// called to localhost:5203/users
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [AllowAnyStatusCode]
        [Get("users")]
        Task<PagedResult<UserDto>> BrowseAsync([Query] BrowseUserQuery query);
    }
}
