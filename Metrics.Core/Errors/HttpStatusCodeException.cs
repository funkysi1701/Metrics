using System.Net;
using System.Runtime.Serialization;

namespace Metrics.Core.Errors
{
    [Serializable]
    public class HttpStatusCodeException : Exception
    {
        public HttpStatusCode Status { get; private set; }

        public HttpStatusCodeException()
        {
        }

        public HttpStatusCodeException(string message) : base(message)
        {
        }

        public HttpStatusCodeException(HttpStatusCode status, string message) : base(message)
        {
            Status = status;
        }

        public HttpStatusCodeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HttpStatusCodeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
