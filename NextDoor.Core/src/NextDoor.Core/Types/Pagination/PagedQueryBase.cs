namespace NextDoor.Core.Types.Pagination
{
    /// The PagedQueryBase is a base class from which derive the specialized query classes 
    /// (e.g. BrowseUsers) that may have their own specific properties
    public abstract class PagedQueryBase : IPagedQuery
    {
        public int Page { get; set; }
        public int Results { get; set; }
        public string OrderBy { get; set; }
        public string SortOrder { get; set; }
    }
}