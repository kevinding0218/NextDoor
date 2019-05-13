namespace NextDoor.Core.Messages
{
    public interface IRejectedEvent : IEvent
    {
         // readonly
         string Reason { get; }
         // readonly
         string Code { get; }
    }
}