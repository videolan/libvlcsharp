using System;

#if WINUI
using Microsoft.UI.Xaml.Markup;
#else
using Windows.UI.Xaml.Markup;
#endif

namespace LibVLCSharp.LibVLCSharp_XamlTypeInfo
{
    /// <summary>
    /// Exposes XAML metadata under the namespace expected from the LibVLCSharp assembly name.
    /// </summary>
    public sealed partial class XamlMetaDataProvider : IXamlMetadataProvider
    {
        readonly IXamlMetadataProvider _provider = (IXamlMetadataProvider)Activator.CreateInstance(
            Type.GetType("LibVLCSharp.LibVLCSharp_UWP_XamlTypeInfo.XamlMetaDataProvider, LibVLCSharp")
                ?? throw new InvalidOperationException("Could not find the generated UWP XAML metadata provider."))!;

        IXamlType IXamlMetadataProvider.GetXamlType(Type type) => _provider.GetXamlType(type);

        IXamlType IXamlMetadataProvider.GetXamlType(string fullName) => _provider.GetXamlType(fullName);

        XmlnsDefinition[] IXamlMetadataProvider.GetXmlnsDefinitions() => _provider.GetXmlnsDefinitions();
    }
}
