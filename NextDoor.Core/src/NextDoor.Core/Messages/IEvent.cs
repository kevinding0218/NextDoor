namespace NextDoor.Core.Messages
{
    // Event – can be subscribed & processed by one or more consumers, may product another Command (saga and that sort of workflows).
    public interface IEvent : IMessage
    {
         
    }
}