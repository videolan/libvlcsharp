using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using LibVLCSharp;
using LibVLCSharp.Helpers;

// AOT Compatibility test for LibVLCSharp core.
//
// Build-time trim analysis (no native toolchain needed):
//   dotnet build
//
// Full NativeAOT publish (requires MSVC on Windows, clang on Linux/macOS):
//   dotnet publish -r win-x64 -c Release
//   dotnet publish -r linux-x64 -c Release

// Reference all public types so the linker includes them in analysis.
_ = typeof(LibVLC);
_ = typeof(MediaPlayer);
_ = typeof(Media);
_ = typeof(MediaList);
_ = typeof(MediaDiscoverer);
_ = typeof(RendererDiscoverer);
_ = typeof(Equalizer);
_ = typeof(MediaInput);
_ = typeof(StreamMediaInput);

AssertNativeCallbacksAreAotCompatible(typeof(MediaPlayerCallbacks));
AssertNativeCallbacksAreAotCompatible(typeof(MediaDiscovererCallbacks));
AssertNativeCallbacksAreAotCompatible(typeof(RendererDiscovererCallbacks));
AssertNativeCallbacksAreAotCompatible(typeof(MediaParser.ParserCallbacks));
AssertNativeCallbacksAreAotCompatible(typeof(MediaParser.ThumbnailerCallbacks));

Console.WriteLine("LibVLCSharp AOT compatibility OK");

static void AssertNativeCallbacksAreAotCompatible(
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicFields)] Type callbackContainer)
{
    var callbackFields = callbackContainer
        .GetFields(BindingFlags.NonPublic | BindingFlags.Static)
        .Where(field => typeof(Delegate).IsAssignableFrom(field.FieldType))
        .ToArray();

    if (callbackFields.Length == 0)
        throw new InvalidOperationException($"{callbackContainer.Name} does not root any native callback delegates");

    var errors = new List<string>();
    foreach (var field in callbackFields)
    {
        var callback = (Delegate)field.GetValue(null)!;
        var attribute = callback.Method.GetCustomAttribute<MonoPInvokeCallbackAttribute>();

        if (attribute == null)
        {
            errors.Add($"{callbackContainer.Name}.{callback.Method.Name} is missing " +
                $"{nameof(MonoPInvokeCallbackAttribute)} for {field.FieldType.Name}");
        }
        else if (attribute.Type != field.FieldType)
        {
            errors.Add($"{callbackContainer.Name}.{callback.Method.Name} declares " +
                $"{attribute.Type.Name}, expected {field.FieldType.Name}");
        }
    }

    if (errors.Count != 0)
    {
        throw new InvalidOperationException(
            $"{callbackContainer.Name} contains callbacks that cannot be marshalled by AOT runtimes:" +
            Environment.NewLine + string.Join(Environment.NewLine, errors));
    }
}
