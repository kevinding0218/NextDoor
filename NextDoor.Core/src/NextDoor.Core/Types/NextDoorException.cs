using System;

namespace NextDoor.Core.Types
{
    public class NextDoorException : Exception
    {
        public string Code { get; }

        public NextDoorException()
        {
        }

        public NextDoorException(string code)
        {
            Code = code;
        }

        public NextDoorException(string message, params object[] args) 
            : this(string.Empty, message, args)
        {
        }

        public NextDoorException(string code, string message, params object[] args) 
            : this(null, code, message, args)
        {
        }

        public NextDoorException(Exception innerException, string message, params object[] args)
            : this(innerException, string.Empty, message, args)
        {
        }

        public NextDoorException(Exception innerException, string code, string message, params object[] args)
            : base(string.Format(message, args), innerException)
        {
            Code = code;
        }        
    }
}