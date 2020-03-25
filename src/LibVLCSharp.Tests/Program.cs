using NUnitLite;
using System.Reflection;

namespace LibVLCSharp.Tests
{
    public static class Program
    {
        public static int Main(string[] args) => new AutoRun(Assembly.GetEntryAssembly()).Execute(args);
    }
}