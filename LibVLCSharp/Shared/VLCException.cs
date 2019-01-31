using System;

namespace LibVLCSharp.Shared
{
    internal class VLCException : Exception
    {
        internal VLCException(string message = ""):base(message)
        {
        }
    }
}