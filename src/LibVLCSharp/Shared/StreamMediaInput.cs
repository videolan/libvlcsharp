using System;
using System.IO;
using System.Runtime.InteropServices;

namespace LibVLCSharp.Shared
{
    /// <summary>
    /// A <see cref="MediaInput"/> implementation that reads from a .NET stream
    /// </summary>
    public class StreamMediaInput : MediaInput
    {
        private readonly Stream _stream;
        private readonly byte[] _readBuffer = new byte[0x4000];

        /// <summary>
        /// Initializes a new instance of <see cref="StreamMediaInput"/>, which reads from the given .NET stream.
        /// </summary>
        /// <remarks>You are still responsible to dispose the stream you give as input.</remarks>
        /// <param name="stream">The stream to be read from.</param>
        public StreamMediaInput(Stream stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        /// <summary>
        /// LibVLC calls this method when it wants to open the media
        /// </summary>
        /// <param name="size">This value must be filled with the length of the media (or ulong.MaxValue if unknown)</param>
        /// <returns><c>true</c> if the stream opened successfully</returns>
        public override bool Open(out ulong size)
        {
            try
            {
                try
                {
                    size = (ulong)_stream.Length;
                }
                catch (Exception)
                {
                    // byte length of the bitstream or UINT64_MAX if unknown
                    size = ulong.MaxValue;
                }

                if (_stream.CanSeek)
                {
                    _stream.Seek(0L, SeekOrigin.Begin);
                }

                return true;
            }
            catch (Exception)
            {
                size = 0UL;
                return false;
            }
        }

        /// <summary>
        /// LibVLC calls this method when it wants to read the media
        /// </summary>
        /// <param name="buf">The buffer where read data must be written</param>
        /// <param name="len">The buffer length</param>
        /// <returns>The number of bytes actually read, -1 on error</returns>
        public override int Read(IntPtr buf, uint len)
        {
            try
            {
                var read = _stream.Read(_readBuffer, 0, Math.Min((int)len, _readBuffer.Length));
                Marshal.Copy(_readBuffer, 0, buf, read);

                return read;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// LibVLC calls this method when it wants to seek to a specific position in the media
        /// </summary>
        /// <param name="offset">The offset, in bytes, since the beginning of the stream</param>
        /// <returns><c>true</c> if the seek succeeded, false otherwise</returns>
        public override bool Seek(ulong offset)
        {
            try
            {
                _stream.Seek((long)offset, SeekOrigin.Begin);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// LibVLC calls this method when it wants to close the media.
        /// </summary>
        public override void Close()
        {
            try
            {
                if (_stream.CanSeek)
                    _stream.Seek(0, SeekOrigin.Begin);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
