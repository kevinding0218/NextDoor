using NextDoor.Core.Messages;
using NextDoor.Core.RabbitMq;
using System.Threading.Tasks;

namespace NextDoor.Core.Handlers
{
    /// <typeparam name="TCommand">C# 7 the in  keyword specifies that you are passing a parameter by reference, but also that you will not modify the value inside a method.</typeparam>
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// Accept a command which is a message and then just consume it
        /// </summary>
        /// <param name="command">command message</param>
        /// <param name="context"></param>
        /// <returns></returns>
        Task HandleAsync(TCommand command, ICorrelationContext context);
    }
}