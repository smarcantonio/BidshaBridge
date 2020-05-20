using System;

namespace BlueIrisClient
{
    public class BlueIrisClientException : Exception
    {
        public BlueIrisClientException(string? message) : base(message)
        {
        }
    }
}
