using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace LibVLCSharp.Tests
{
    internal static class NativeBindingAssertions
    {
        internal static MethodInfo NativeMethod(Type owner, string name)
        {
            var method = NativeMethods(owner).SingleOrDefault(m => m.Name == name);
            Assert.NotNull(method, $"{owner.Name}.Native.{name} was not found");
            return method!;
        }

        internal static MethodInfo[] NativeMethods(Type owner)
        {
            var nativeType = owner.GetNestedType("Native", BindingFlags.NonPublic);
            Assert.NotNull(nativeType, $"{owner.Name}.Native was not found");
            return nativeType!.GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
        }

        internal static void HasDllImport(Type owner, string methodName, string entryPoint) =>
            HasDllImport(NativeMethod(owner, methodName), entryPoint);

        internal static void HasDllImport(MethodInfo method, string entryPoint) =>
            Assert.AreEqual(entryPoint, DllImportEntryPoint(method));

        internal static void HasParameterTypes(MethodInfo method, params Type[] parameterTypes) =>
            CollectionAssert.AreEqual(parameterTypes, method.GetParameters().Select(p => p.ParameterType).ToArray());

        internal static string DllImportEntryPoint(MethodInfo method) =>
            method.GetCustomAttribute<DllImportAttribute>()?.EntryPoint;
    }
}
