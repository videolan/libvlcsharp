using Xamarin.Forms;

namespace LibVLCSharp.Forms.Shared.Effects
{
    /// <summary>
    /// Click effect.
    /// </summary>
    internal class ClickEffect : TriggerAction<VisualElement>
    {
        /// <summary>
        /// Apply a click effect.
        /// </summary>
        /// <param name="sender">The object on which to invoke the trigger action.</param>
        protected override async void Invoke(VisualElement sender)
        {
            await sender.ScaleTo(0.85, 100);
            await sender.ScaleTo(1, 50);
        }
    }
}
