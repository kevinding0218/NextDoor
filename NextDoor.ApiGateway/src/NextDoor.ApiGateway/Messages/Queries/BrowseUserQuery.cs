using NextDoor.Core.Types.Pagination;

namespace NextDoor.ApiGateway.Messages.Queries
{
    public class BrowseUserQuery : PagedQueryBase
    {
        public string EmailDomain { get; set; }
    }
}
