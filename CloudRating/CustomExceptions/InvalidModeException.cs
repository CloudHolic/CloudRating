using System;
using System.Runtime.Serialization;

namespace CloudRating.CustomExceptions
{
    [Serializable]
    public class InvalidModeException : Exception, ISerializable
    {
        public InvalidModeException()
        {
        }

        public InvalidModeException(string message) : base(message)
        {
        }

        public InvalidModeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        //  This constructor is needed for serialization.
        protected InvalidModeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
