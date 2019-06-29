using RestEase;

namespace NextDoor.ApiGateway.Services
{
    [SerializationMethods(Query = QuerySerializationMethod.Serialized)]
    public interface IIdentityService
    {
    }
}
