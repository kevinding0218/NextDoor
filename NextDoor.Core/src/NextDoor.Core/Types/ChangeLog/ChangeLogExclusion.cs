using NextDoor.Core.Types.Domain;

namespace NextDoor.Core.Types.ChangeLog
{
    public class ChangeLogExclusion : IEntity
    {
        public ChangeLogExclusion()
        {
        }

        public int? ID { get; set; }
        public string EntityName { get; set; }
        public string PropertyName { get; set; }
    }
}
