using NextDoor.ApiGateway.Messages.Queries.Identity;
using RestEase;
using System.Threading.Tasks;

namespace NextDoor.ApiGateway.Services
{
    [SerializationMethods(Body = BodySerializationMethod.Serialized)]
    public interface IIdentityService
    {
        /// <summary>
        /// called to localhost:5201/sign-in
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [AllowAnyStatusCode]
        [Post("sign-in")]
        //Task<JsonWebToken> SignInAsync([Body] SignInQuery query);
        Task<object> SignInAsync([Body] SignInQuery query);
    }
}
