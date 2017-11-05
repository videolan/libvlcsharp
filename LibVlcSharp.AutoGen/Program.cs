using System;
using CppSharp;

namespace libvlcsharp
{
    partial class Program
    {
        static void Main(string[] args)
        {
            ConsoleDriver.Run(new LibVLC());
            Console.ReadKey();
        }
    }
}