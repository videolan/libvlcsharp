using System;
using System.Collections.Generic;

namespace LibVLCSharp.Shared.Structures
{
    /// <summary>
    /// <para>Description for video, audio tracks and subtitles. It contains</para>
    /// <para>id, name (description string) and pointer to next record.</para>
    /// </summary>
    public struct TrackDescription 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IntPtr Next { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is TrackDescription))
            {
                return false;
            }

            var description = (TrackDescription)obj;
            return Id == description.Id &&
                   Name == description.Name &&
                   EqualityComparer<IntPtr>.Default.Equals(Next, description.Next);
        }

        public override int GetHashCode()
        {
            var hashCode = -485159682;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<IntPtr>.Default.GetHashCode(Next);
            return hashCode;
        }
    }
}