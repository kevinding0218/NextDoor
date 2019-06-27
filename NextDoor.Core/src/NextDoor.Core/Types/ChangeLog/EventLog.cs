using NextDoor.Core.Types.Domain;
using System;

namespace NextDoor.Core.Types.ChangeLog
{
    public class EventLog : IEntity
    {
        public EventLog()
        {
        }

        public int? ID { get; set; }
        public int? EventType { get; set; }
        public string Key { get; set; }
        public string Message { get; set; }
        public DateTime? EntryDate { get; set; }
    }
}
