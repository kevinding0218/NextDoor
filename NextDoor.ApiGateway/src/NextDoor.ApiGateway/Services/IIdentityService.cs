using Microsoft.IdentityModel.JsonWebTokens;
using NextDoor.ApiGateway.Messages.Queries.Identity;
using RestEase;
using System.Threading.Tasks;

namespace NextDoor.ApiGateway.Services
{
    [SerializationMethods(Query = QuerySerializationMethod.Serialized)]
    public interface IIdentityService
    {
        /// <summary>
        /// called to localhost:5203/users
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [AllowAnyStatusCode]
        [Post("sign-in")]
        Task<JsonWebToken> SignInAsync([Query] SignInQuery query);
    }
}
