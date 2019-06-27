using NextDoor.Core.Types;
using System;
using System.Threading.Tasks;

namespace NextDoor.Core.Handlers
{
    public interface IHandler
    {
        IHandler Handle(Func<Task> handle);
        IHandler OnSuccess(Func<Task> onSuccess);
        IHandler OnError(Func<Exception, Task> onError, bool rethrow = false);
        IHandler OnCustomError(Func<NextDoorException, Task> onCustomError, bool rethrow = false);
        IHandler Always(Func<Task> always);
        Task ExecuteAsync();
    }
}
