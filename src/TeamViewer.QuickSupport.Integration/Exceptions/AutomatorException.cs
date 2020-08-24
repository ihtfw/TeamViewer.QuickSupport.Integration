using System;
using System.Runtime.Serialization;

namespace TeamViewer.QuickSupport.Integration.Exceptions
{
    public class AutomatorException : ApplicationException
    {
        public AutomatorException()
        {
        }

        protected AutomatorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public AutomatorException(string message) : base(message)
        {
        }

        public AutomatorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}