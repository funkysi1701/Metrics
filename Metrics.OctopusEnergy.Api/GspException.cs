using System;
using System.Runtime.Serialization;

namespace Metrics.OctopusEnergy.Api
{
    [Serializable]
    public class GspException : ApplicationException
    {
        protected GspException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public GspException()
        {
        }

        public GspException(string message) : base(message)
        {
        }

        public GspException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
