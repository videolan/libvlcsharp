namespace LibVLCSharp
{
    /// <summary>A snapshot of bytes written by a download.</summary>
    public readonly struct DownloadProgress
    {
        internal DownloadProgress(ulong bytesWritten, ulong totalBytes)
        {
            BytesWritten = bytesWritten;
            TotalBytes = totalBytes;
        }

        /// <summary>Bytes successfully written to the destination by this download.</summary>
        public ulong BytesWritten { get; }

        /// <summary>Current source size in bytes, as reported by LibVLC. May change during downloading.</summary>
        public ulong TotalBytes { get; }
    }
}
