using System;
using System.Net.Http;
using System.Runtime.Serialization;

namespace SafeNet.Exceptions
{
    public class AuthException : SafeException
    {
        public AuthException(HttpResponseMessage launcherResponse) : base(launcherResponse)
        {
        }

        public AuthException(string message, HttpResponseMessage launcherResponse) : base(message, launcherResponse)
        {
        }

        public AuthException(string message, Exception innerException, HttpResponseMessage launcherResponse) : base(message, innerException, launcherResponse)
        {
        }

        public AuthException(SerializationInfo info, StreamingContext context, HttpResponseMessage launcherResponse) : base(info, context, launcherResponse)
        {
        }
    }
}