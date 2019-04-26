using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NextDoor.Core.Types.Pagination;

namespace NextDoor.Core.MSSQL
{
    /// https://piotrgankiewicz.com/2016/04/19/pagination-for-mssql-mongodb/
    public static class MSSQLPagination
    {
        #region Dealing with IOrderedQueryable
        /*
         * IOrderedQueryable<T> contains extra state to hold information about sorting
         */
        public static async Task<PagedResult<T>> PaginateAsync<T>(this IOrderedQueryable<T> collection, PagedQueryBase query)
            => await collection.PaginateAsync(query.Page, query.Results);
        
        public static async Task<PagedResult<T>> PaginateAsync<T>(this IOrderedQueryable<T> collection, 
        int page = 1, int resultsPerPage = 10)
            => await collection.AsQueryable().PaginateAsync(page, resultsPerPage);
        #endregion
        
        #region Dealing with IQueryable
        /* sdfs
        *  IQueryable<T> normally represents an operation that will be performed later (e.g. with LINQ to SQL). 
        * A separate interface is needed because the next operation might be another sort, which needs to be treated differently to the IOrderedQueryable<T>
         */
        public static async Task<PagedResult<T>> Paginate<T>(this IQueryable<T> collection, PagedQueryBase query)
            => await collection.PaginateAsync(query.Page, query.Results);
        public static async Task<PagedResult<T>> PaginateAsync<T>(this IQueryable<T> collection,
            int page = 1, int resultsPerPage = 10)
        {
            if (page <= 0)  page = 1;
            if (resultsPerPage <= 0) resultsPerPage = 10;

            var isEmpty = await collection.AnyAsync() == false;
            if (isEmpty)    return PagedResult<T>.Empty;

            var totalResults = await collection.CountAsync();
            var totalPages = (int)Math.Ceiling((decimal)totalResults / resultsPerPage);
            var data = await collection.Limit(page, resultsPerPage).ToListAsync();

            return PagedResult<T>.Create(data, page, resultsPerPage, totalPages, totalResults);
        }

        public static IQueryable<T> Limit<T>(this IQueryable<T> collection, PagedQueryBase query)
            => collection.Limit(query.Page, query.Results);

        public static IQueryable<T> Limit<T>(this IQueryable<T> collection,
            int page = 1, int resultsPerPage = 10)
        {
            if (page <= 0)
            {
                page = 1;
            }
            if (resultsPerPage <= 0)
            {
                resultsPerPage = 10;
            }
            var skip = (page - 1) * resultsPerPage;
            var data = collection.Skip(skip)
                .Take(resultsPerPage);

            return data;
        }
        #endregion
    }
}