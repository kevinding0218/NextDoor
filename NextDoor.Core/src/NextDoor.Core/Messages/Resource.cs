namespace NextDoor.Core.Messages
{
    public class Resource
    {
        public string Service { get; }
        public string Endpoint { get; }

        protected Resource()
        {

        }

        protected Resource(string service, string endpoint)
        {
            this.Service = service.ToLowerInvariant();
            this.Endpoint = endpoint.ToLowerInvariant();
        }

        public static Resource Create(string service, string endpoint)
            => new Resource(service, endpoint);
    }
}