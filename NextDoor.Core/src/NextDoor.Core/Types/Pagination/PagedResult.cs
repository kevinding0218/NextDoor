using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace NextDoor.Core.Types.Pagination
{
    /// why the PagedResult is immutable while PagedQueryBase is not?
    /// Well, because e.g. in an HTTP API I would parse the incoming request directly into 
    /// the derived type of PagedQueryBase and the setters come in handy in this occasion.
    public class PagedResult<T> : PagedResultBase
    {
        public IEnumerable<T> Items { get; }
        public bool IsEmpty => Items == null || !Items.Any();
        public bool IsNotEmpty => !IsEmpty;

        protected PagedResult()
        {
            Items = Enumerable.Empty<T>();
        }

        // To specify that a constructor should be used to create a class during deserialization
        [JsonConstructor]
        protected PagedResult(IEnumerable<T> items, int currentPage, int resultsPerPage, int totalPages, long totalResults)
         : base(currentPage, resultsPerPage, totalPages, totalResults)
        {
            Items = items;
        }

        public static PagedResult<T> Create(IEnumerable<T> items, int currentPage, int resultsPerPage, int totalPages, long totalResults)
            => new PagedResult<T>(items, currentPage, resultsPerPage, totalPages, totalResults);

        public static PagedResult<T> From(PagedResultBase result, IEnumerable<T> items)
            => new PagedResult<T>(items, result.CurrentPage, result.ResultsPerPage, result.TotalPages, result.TotalResults);

        public static PagedResult<T> Empty => new PagedResult<T>();
    }
}