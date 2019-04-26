namespace NextDoor.Core.Types.Pagination
{
    /// specified as a separate abstract class, mostly due to the fact that it’s sometimes useful to be able to 
    /// have access to its properties without taking into consideration the Items.
    public abstract class PagedResultBase
    {
        public int CurrentPage { get; }
        public int ResultsPerPage { get; }
        public int TotalPages { get; }
        public long TotalResults { get; }
        
        //Prevent client code from directly instantiating the class object but still be able to instantiate through a subclass
        protected PagedResultBase()
        {            
        }

        //Prevent client code from directly instantiating the class object but still be able to instantiate through a subclass
        protected PagedResultBase(int currentPage, int resultsPerPage, int totalPages, long totalResults)
        {
            this.CurrentPage = currentPage >= totalPages ? totalPages : currentPage;
            this.ResultsPerPage = resultsPerPage;
            this.TotalPages = totalPages;
            this.TotalResults = totalResults;
        }
    }
}