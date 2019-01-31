using System;

namespace LibVLCSharp.Shared
{
    public class VLCException : Exception
    {
        public VLCException(string message = ""):base(message)
        {
        }
    }
}