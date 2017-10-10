using System;
using System.Collections.Generic;
using System.IO;
using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Parser;
using CppSharp.Passes;

namespace libvlcsharp
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleDriver.Run(new LibVLC());
            Console.ReadKey();
        }

        class LibVLC : ILibrary
        {
            public void Preprocess(Driver driver, ASTContext ctx)
            {         
            }

            public void Postprocess(Driver driver, ASTContext ctx)
            {
            }

            public void Setup(Driver driver)
            {
                var rootPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
                var libvlcppHeaders = Path.Combine(rootPath, "include\\libvlcpp");
                var libvlcHeaders = Path.Combine(rootPath, "include\\libvlcpp\\vlc");
                var libvlcPluginsHeaders = Path.Combine(rootPath, "include\\libvlcpp\\vlc\\plugins");
                var libFolder = Path.Combine(rootPath, "include\\lib");

                driver.ParserOptions = new ParserOptions
                { 
                    IncludeDirs = new List<string> { libvlcppHeaders, libvlcHeaders, libvlcPluginsHeaders },
                    LibraryDirs = new List<string> { libFolder },
                    Verbose = true,
                    EnableRTTI = true,
                    Arguments = new List<string> { "-fcxx-exceptions" },
                    LanguageVersion = LanguageVersion.CPP14,                    
                };

                var options = driver.Options;
                options.GeneratorKind = GeneratorKind.CSharp;
                
                var module = options.AddModule("libvlcpp");
                
                module.IncludeDirs.AddRange(new []{ libvlcppHeaders, libvlcHeaders, libvlcPluginsHeaders });
                module.Headers.Add("vlc.hpp");
            
                module.LibraryDirs.Add(libFolder);
                module.Libraries.AddRange(new[] { "libvlc.lib", "libvlccore.lib" });
            }

            public void SetupPasses(Driver driver)
            {
                driver.Context.TranslationUnitPasses.RenameDeclsUpperCase(RenameTargets.Any);
                driver.Context.TranslationUnitPasses.AddPass(new FunctionToInstanceMethodPass());
            }
        }
    }
}