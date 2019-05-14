using System;
namespace NextDoor.Core.Messages
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageNamespaceAttribute : Attribute
    {
        public string Namespace { get; }
        // used reserved words as variable names
        public MessageNamespaceAttribute(string @namespace)
        {
            this.Namespace = @namespace?.ToLowerInvariant();
        }
    }
}