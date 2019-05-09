using System.Threading.Tasks;

namespace NextDoor.Core.Common
{
    public interface IInitializer
    {
         Task InitializeAsync();
    }
}