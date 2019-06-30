using System;
namespace NextDoor.Core.Messages
{
    // Define a custom topic name for message in RabbitMQ
    [AttributeUsage(AttributeTargets.Class)]
    public class ExchangeNamespaceAttribute : Attribute
    {
        public string Namespace { get; }
        // used reserved words as variable names
        public ExchangeNamespaceAttribute(string @namespace)
        {
            this.Namespace = @namespace?.ToLowerInvariant();
        }
    }
}