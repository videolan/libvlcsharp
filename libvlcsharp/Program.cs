using System;
using System.Collections.Generic;
using System.IO;
using CppSharp;
using CppSharp.Generators;
using CppSharp.Parser;
using ASTContext = CppSharp.AST.ASTContext;

namespace libvlcsharp
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleDriver.Run(new LibVLC());
            //Console.ReadKey();
        }

        class LibVLC : ILibrary
        {
            public void Preprocess(Driver driver, ASTContext ctx)
            {

            }

            public void Postprocess(Driver driver, ASTContext ctx)
            {
                // FIXME
                ctx.IgnoreHeadersWithName("libvlc_events.h");
            }

            public void Setup(Driver driver)
            {
                var rootPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
                var libvlcHeaders = Path.Combine(rootPath, "include\\vlc");
                var libvlcPluginsHeaders = Path.Combine(rootPath, "include\\vlc\\plugins");
                var h = Path.Combine(rootPath, "include");
                var libFolder = Path.Combine(rootPath, "include\\lib");

                driver.ParserOptions = new ParserOptions
                {
                    IncludeDirs = new List<string> { libvlcHeaders, libvlcPluginsHeaders, h },
                    Verbose = true
                };
                
                var options = driver.Options;
                options.GeneratorKind = GeneratorKind.CSharp;
                options.CompileCode = true;
                options.CheckSymbols = true;
                options.OutputDir = Path.Combine(rootPath, "..\\Sample\\");
                

                var module = options.AddModule("libvlc");
                module.Headers.Add("vlc.h");

                //module.Libraries.Add("libvlc.dll");

                //module.SharedLibraryName = "libvlc";
                //module.SymbolsLibraryName = "libvlc";
                //module.LibraryName = "libvlc";

                //module.LibraryDirs.Add(Path.Combine(rootPath, "..\\Sample\\bin\\Debug\\"));

                module.LibraryDirs.Add(libFolder);
                module.Libraries.Add("libvlc.dll");
                module.Libraries.AddRange(new[] { "libvlc.lib", "libvlccore.lib" });
            }

            public void SetupPasses(Driver driver)
            {
            }
        }
    }
}