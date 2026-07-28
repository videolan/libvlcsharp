using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using LibVLCSharp;
using LibVLCSharp.Helpers;

// AOT compatibility test for every managed callback that can cross into native code.
//
// Build-time trim analysis (no native toolchain needed):
//   dotnet build
//
// Full NativeAOT publish (requires MSVC on Windows, clang on Linux/macOS):
//   dotnet publish -r win-x64 -c Release
//   dotnet publish -r linux-x64 -c Release

var errors = AuditNativeCallbacks(
    typeof(LibVLC).Assembly,
    out var rootedCallbackCount,
    out var pinvokeCallbackParameterCount);

if (errors.Count != 0)
{
    throw new InvalidOperationException(
        "LibVLCSharp contains callbacks that cannot be marshalled by AOT runtimes:" +
        Environment.NewLine + string.Join(Environment.NewLine, errors));
}

Console.WriteLine(
    $"LibVLCSharp AOT compatibility OK " +
    $"({rootedCallbackCount} rooted callbacks, " +
    $"{pinvokeCallbackParameterCount} P/Invoke callback parameters)");

[UnconditionalSuppressMessage(
    "Trimming",
    "IL2026",
    Justification = "The test app preserves and audits the complete LibVLCSharp assembly.")]
[UnconditionalSuppressMessage(
    "Trimming",
    "IL2065",
    Justification = "The test app preserves and audits the complete LibVLCSharp assembly.")]
[UnconditionalSuppressMessage(
    "Trimming",
    "IL2075",
    Justification = "The test app preserves and audits the complete LibVLCSharp assembly.")]
static List<string> AuditNativeCallbacks(
    Assembly assembly,
    out int rootedCallbackCount,
    out int pinvokeCallbackParameterCount)
{
    var errors = new List<string>();
    var validWrappers = new HashSet<Type>();
    var types = assembly.GetTypes();

    rootedCallbackCount = 0;
    foreach (var type in types)
    {
        foreach (var field in type.GetFields(
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Static |
            BindingFlags.DeclaredOnly))
        {
            if (!IsUnmanagedDelegate(field.FieldType))
                continue;

            rootedCallbackCount++;
            var displayName = $"{type.FullName}.{field.Name}";

            if (!field.IsInitOnly)
            {
                errors.Add(
                    $"{displayName} must be readonly so its native callback remains rooted");
                continue;
            }

            if (field.GetValue(null) is not Delegate callback)
            {
                errors.Add($"{displayName} does not contain a callback delegate");
                continue;
            }

            if (!callback.Method.IsStatic)
            {
                errors.Add(
                    $"{displayName} targets non-static method " +
                    $"{callback.Method.DeclaringType?.FullName}.{callback.Method.Name}");
                continue;
            }

            var attribute =
                callback.Method.GetCustomAttribute<MonoPInvokeCallbackAttribute>();
            if (attribute == null)
            {
                errors.Add(
                    $"{callback.Method.DeclaringType?.FullName}.{callback.Method.Name} " +
                    $"is missing {nameof(MonoPInvokeCallbackAttribute)} " +
                    $"for {field.FieldType.Name}");
                continue;
            }

            if (attribute.Type != field.FieldType)
            {
                errors.Add(
                    $"{callback.Method.DeclaringType?.FullName}.{callback.Method.Name} " +
                    $"declares {attribute.Type.Name}, expected {field.FieldType.Name}");
                continue;
            }

            validWrappers.Add(field.FieldType);
        }
    }

    pinvokeCallbackParameterCount = 0;
    foreach (var type in types)
    {
        foreach (var method in type.GetMethods(
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Static |
            BindingFlags.DeclaredOnly))
        {
            if (method.GetCustomAttribute<DllImportAttribute>() == null)
                continue;

            foreach (var parameter in method.GetParameters())
            {
                if (!IsUnmanagedDelegate(parameter.ParameterType))
                    continue;

                pinvokeCallbackParameterCount++;
                if (!validWrappers.Contains(parameter.ParameterType))
                {
                    errors.Add(
                        $"{type.FullName}.{method.Name} parameter '{parameter.Name}' uses " +
                        $"{parameter.ParameterType.Name}, but no rooted, correctly annotated " +
                        "wrapper exists");
                }
            }
        }
    }

    return errors;
}

static bool IsUnmanagedDelegate(Type type) =>
    typeof(Delegate).IsAssignableFrom(type) &&
    type.GetCustomAttribute<UnmanagedFunctionPointerAttribute>() != null;
