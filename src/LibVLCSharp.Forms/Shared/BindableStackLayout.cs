using System.Collections;
using Xamarin.Forms;

namespace LibVLCSharp.Forms.Shared
{
    /// <summary>
    /// Defines the <see cref="BindableStackLayout" />.
    /// </summary>
    public class BindableStackLayout : StackLayout
    {
        /// <summary>
        /// Gets or sets the ItemsSource.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Defines the ItemsSourceProperty.
        /// </summary>
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(BindableStackLayout),
                                    propertyChanged: (bindable, oldValue, newValue) => ((BindableStackLayout)bindable).PopulateItems());

        /// <summary>
        /// Gets or sets the ItemDataTemplate.
        /// </summary>
        public DataTemplate ItemDataTemplate
        {
            get { return (DataTemplate)GetValue(ItemDataTemplateProperty); }
            set { SetValue(ItemDataTemplateProperty, value); }
        }

        /// <summary>
        /// Defines the ItemDataTemplateProperty.
        /// </summary>
        public static readonly BindableProperty ItemDataTemplateProperty =
            BindableProperty.Create(nameof(ItemDataTemplate), typeof(DataTemplate), typeof(BindableStackLayout));

        internal void PopulateItems()
        {
            if (ItemsSource == null)
            {
                return;
            }
            if (Children.Count > 1)
            {
                Children.Clear();
            }
            foreach (var item in ItemsSource)
            {
                if (ItemDataTemplate.CreateContent() is View itemTemplate)
                {
                    itemTemplate.BindingContext = item;
                    Children.Add(itemTemplate);
                }
            }
        }
    }
}
