using System;
using System.Net.Http;
using System.Runtime.Serialization;

namespace SafeNet.Exceptions
{
    public abstract class SafeException : Exception
    {
        public HttpResponseMessage LauncherResponse { get; }

        protected SafeException(HttpResponseMessage launcherResponse)
        {
            LauncherResponse = launcherResponse;
        }

        protected SafeException(string message, HttpResponseMessage launcherResponse) : base(message)
        {
            LauncherResponse = launcherResponse;
        }

        protected SafeException(string message, Exception innerException, HttpResponseMessage launcherResponse) : base(message, innerException)
        {
            LauncherResponse = launcherResponse;
        }

        protected SafeException(SerializationInfo info, StreamingContext context, HttpResponseMessage launcherResponse) : base(info, context)
        {
            LauncherResponse = launcherResponse;
        }
    }
}