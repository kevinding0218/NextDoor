using Microsoft.AspNetCore.Mvc;
using NextDoor.Core.Messages;
using NextDoor.Core.RabbitMq;
using NextDoor.Core.Types.Pagination;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NextDoor.ApiGateway.Controllers
{
    [Route("[controller]")]
    [ApiController]
    // [JwtAuth]
    public abstract class BaseController : ControllerBase
    {
        private static readonly string AcceptLanguageHeader = "accept-language";
        private static readonly string OperationHeader = "X-Operation";
        private static readonly string ResourceHeader = "X-Resource";
        private static readonly string DefaultCulture = "en-us";
        private static readonly string PageLink = "page";
        private readonly IBusPublisher _busPublisher;
        //private readonly ITracer _tracer;

        protected BaseController(IBusPublisher busPublisher)
        {
            _busPublisher = busPublisher;
        }

        #region Identity Info
        protected bool IsAdmin
            => User.IsInRole("admin");

        protected Guid UserId
            => string.IsNullOrWhiteSpace(User?.Identity?.Name) ?
                Guid.Empty :
                Guid.Parse(User.Identity.Name);
        #endregion

        #region Message Broker
        /// <summary>
        /// Create some context, passes command which is our object coming to the API 
        /// and a context and it gets published to the bus and it returns the acceptor 202 status
        /// from our API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <param name="resourceId"></param>
        /// <param name="resource"></param>
        /// <returns></returns>
        protected async Task<IActionResult> SendAsync<T>(T command,
            Guid? resourceId = null, string resource = "") where T : ICommand
        {
            var context = GetContext<T>(resourceId, resource);
            await _busPublisher.SendAsync(command, context);

            return Accepted(context);
        }

        protected IActionResult Accepted(ICorrelationContext context)
        {
            Response.Headers.Add(OperationHeader, $"operations/{context.Id}");
            if (!string.IsNullOrWhiteSpace(context.Resource))
            {
                Response.Headers.Add(ResourceHeader, context.Resource);
            }

            return base.Accepted();
        }

        protected ICorrelationContext GetContext<T>(Guid? resourceId = null, string resource = "") where T : ICommand
        {
            if (!string.IsNullOrWhiteSpace(resource))
            {
                resource = $"{resource}/{resourceId}";
            }

            return CorrelationContext.Create<T>(Guid.NewGuid(), UserId, resourceId ?? Guid.Empty,
               HttpContext.TraceIdentifier, HttpContext.Connection.Id, string.Empty,
               Request.Path.ToString(), Culture, resource);
        }
        #endregion

        #region IActionResult Implementation
        protected IActionResult Single<T>(T model, Func<T, bool> criteria = null)
        {
            if (model == null)
            {
                return NotFound();
            }
            var isValid = criteria == null || criteria(model);
            if (isValid)
            {
                return Ok(model);
            }

            return NotFound();
        }

        protected IActionResult Collection<T>(PagedResult<T> pagedResult, Func<PagedResult<T>, bool> criteria = null)
        {
            if (pagedResult == null)
            {
                return NotFound();
            }
            var isValid = criteria == null || criteria(pagedResult);
            if (!isValid)
            {
                return NotFound();
            }
            if (pagedResult.IsEmpty)
            {
                return Ok(Enumerable.Empty<T>());
            }
            Response.Headers.Add("Link", GetLinkHeader(pagedResult));
            Response.Headers.Add("X-Total-Count", pagedResult.TotalResults.ToString());

            return Ok(pagedResult.Items);
        }
        #endregion

        #region Pagination Info
        private string GetLinkHeader(PagedResultBase result)
        {
            var first = GetPageLink(result.CurrentPage, 1);
            var last = GetPageLink(result.CurrentPage, result.TotalPages);
            var prev = string.Empty;
            var next = string.Empty;
            if (result.CurrentPage > 1 && result.CurrentPage <= result.TotalPages)
            {
                prev = GetPageLink(result.CurrentPage, result.CurrentPage - 1);
            }
            if (result.CurrentPage < result.TotalPages)
            {
                next = GetPageLink(result.CurrentPage, result.CurrentPage + 1);
            }

            return $"{FormatLink(next, "next")}{FormatLink(last, "last")}" +
                   $"{FormatLink(first, "first")}{FormatLink(prev, "prev")}";
        }

        private string GetPageLink(int currentPage, int page)
        {
            var path = Request.Path.HasValue ? Request.Path.ToString() : string.Empty;
            var queryString = Request.QueryString.HasValue ? Request.QueryString.ToString() : string.Empty;
            var conjunction = string.IsNullOrWhiteSpace(queryString) ? "?" : "&";
            var fullPath = $"{path}{queryString}";
            var pageArg = $"{PageLink}={page}";
            var link = fullPath.Contains($"{PageLink}=")
                ? fullPath.Replace($"{PageLink}={currentPage}", pageArg)
                : fullPath += $"{conjunction}{pageArg}";

            return link;
        }
        #endregion

        protected string Culture
            => Request.Headers.ContainsKey(AcceptLanguageHeader) ?
                    Request.Headers[AcceptLanguageHeader].First().ToLowerInvariant() :
                    DefaultCulture;

        private static string FormatLink(string path, string rel)
            => string.IsNullOrWhiteSpace(path) ? string.Empty : $"<{path}>; rel=\"{rel}\",";
    }
}