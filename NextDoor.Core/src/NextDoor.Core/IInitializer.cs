using System.Threading.Tasks;

namespace NextDoor.Core
{
    public interface IInitializer
    {
         Task InitilizeAsync();
    }
}