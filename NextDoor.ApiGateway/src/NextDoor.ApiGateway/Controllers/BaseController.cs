using Microsoft.AspNetCore.Mvc;
using NextDoor.Core.Messages;
using NextDoor.Core.RabbitMq;
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
        //private static readonly string PageLink = "page";
        private readonly IBusPublisher _busPublisher;
        //private readonly ITracer _tracer;

        protected BaseController(IBusPublisher busPublisher)
        {
            _busPublisher = busPublisher;
        }

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

        protected bool IsAdmin
            => User.IsInRole("admin");

        protected Guid UserId
            => string.IsNullOrWhiteSpace(User?.Identity?.Name) ?
                Guid.Empty :
                Guid.Parse(User.Identity.Name);

        protected string Culture
            => Request.Headers.ContainsKey(AcceptLanguageHeader) ?
                    Request.Headers[AcceptLanguageHeader].First().ToLowerInvariant() :
                    DefaultCulture;

    }
}