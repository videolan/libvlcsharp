using System;

namespace LibVLCSharp.Shared
{
    public class VLCException : Exception
    {
        public VLCException(string message = ""):base(message)
        {
        }

        /// <summary>
        /// Creates a <see cref="VLCException" /> with a message and an inner exeption
        /// </summary>
        public VLCException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}