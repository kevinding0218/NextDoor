using NextDoor.Core.Types.Domain;
using System;

namespace NextDoor.Core.Types.ChangeLog
{
    public class ChangeLog : IEntity
    {
        public ChangeLog()
        {
        }

        public int? ID { get; set; }
        public string ClassName { get; set; }
        public string PropertyName { get; set; }
        public string Key { get; set; }
        public string OriginalValue { get; set; }
        public string CurrentValue { get; set; }
        public int? Uid { get; set; }
        public DateTime? ChangeDate { get; set; }
    }
}
