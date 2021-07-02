using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LibVLCSharp.Forms.Shared
{
    /// <summary>
    /// Defines the <see cref="TrackViewModel" />.
    /// </summary>
    public class TrackViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Track description Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Track description
        /// </summary>
        public string Name { get; set; }
        
        private bool selected = false;
        /// <summary>
        /// The track is selected or not.
        /// </summary>
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// TrackViewModel constructor
        /// </summary>
        /// <param name="id">Track description Id</param>
        /// <param name="name">Track description</param>
        internal TrackViewModel(int id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Occurs when a property value changes
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
