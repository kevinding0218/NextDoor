using System.Threading.Tasks;
using NextDoor.Core.Messages;

namespace NextDoor.Core.Dispatcher
{
    public interface ICommandDispatcher
    {
         Task SendAsync<T>(T command) where T : ICommand;
    }
}