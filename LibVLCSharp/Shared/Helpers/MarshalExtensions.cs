using LibVLCSharp.Shared.Structures;

namespace LibVLCSharp.Shared.Helpers
{
    internal static class MarshalExtensions
    {
        /// <summary>
        /// Helper method that creates a managed type from the internal interop structure.
        /// </summary>
        /// <param name="s">AudioOutputDescriptionStructure from interop</param>
        /// <returns>public AudioOutputDescription to be consumed by the user</returns>
        internal static AudioOutputDescription Build(this AudioOutputDescriptionStructure s) => new AudioOutputDescription
        {
            Name = Utf8StringMarshaler.GetInstance().MarshalNativeToManaged(s.Name) as string,
            Description = Utf8StringMarshaler.GetInstance().MarshalNativeToManaged(s.Description) as string,
        };

        /// <summary>
        /// Helper method that creates a managed type from the internal interop structure.
        /// </summary>
        /// <param name="s">AudioOutputDeviceStructure from interop</param>
        /// <returns>public AudioOutputDevice to be consumed by the user</returns>
        internal static AudioOutputDevice Build(this AudioOutputDeviceStructure s) => new AudioOutputDevice
        {
            DeviceIdentifier = Utf8StringMarshaler.GetInstance().MarshalNativeToManaged(s.DeviceIdentifier) as string,
            Description = Utf8StringMarshaler.GetInstance().MarshalNativeToManaged(s.Description) as string,
        };
    }
}