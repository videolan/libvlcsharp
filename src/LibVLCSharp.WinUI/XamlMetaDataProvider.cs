using System;
using Microsoft.UI.Xaml.Markup;

namespace LibVLCSharp.LibVLCSharp_XamlTypeInfo
{
    /// <summary>
    /// Exposes XAML metadata under the namespace expected from the LibVLCSharp assembly name.
    /// </summary>
    public sealed partial class XamlMetaDataProvider : IXamlMetadataProvider
    {
        readonly IXamlMetadataProvider _provider = (IXamlMetadataProvider)Activator.CreateInstance(
            Type.GetType("LibVLCSharp.LibVLCSharp_WinUI_XamlTypeInfo.XamlMetaDataProvider, LibVLCSharp")
                ?? throw new InvalidOperationException("Could not find the generated WinUI XAML metadata provider."))!;

        IXamlType IXamlMetadataProvider.GetXamlType(Type type) => _provider.GetXamlType(type);

        IXamlType IXamlMetadataProvider.GetXamlType(string fullName) => _provider.GetXamlType(fullName);

        XmlnsDefinition[] IXamlMetadataProvider.GetXmlnsDefinitions() => _provider.GetXmlnsDefinitions();
    }
}
