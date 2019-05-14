using System.Threading.Tasks;
using NextDoor.Core.Messages;
using NextDoor.Core.RabbitMq;

namespace NextDoor.Core.Handlers
{
    // C# 7 the in  keyword specifies that you are passing a parameter by reference, but also that you will not modify the value inside a method.
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
         Task HandleAsync(TCommand command, ICorrelationContext context);
    }
}