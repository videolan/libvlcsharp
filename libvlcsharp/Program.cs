using System;
using System.Collections.Generic;
using System.IO;
using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Parser;
using CppSharp.Passes;
using ClangParser = CppSharp.ClangParser;

namespace libvlcsharp
{
    class Program
    {
        //public static void Main(string[] args)
        //{
        //    //var rootPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent;
        //    ////        //var headerFolder = Path.Combine(rootPath.FullName, "include\\headers");
        //    //var path = Path.Combine(rootPath.FullName, "include\\headers\\plugins\\vlc_arrays.h");
        //    //var file = Path.GetFullPath(args[0]);

        //    var rootPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent;
        //    var path = Path.Combine(rootPath.FullName, "include\\libvlcpp\\common.hpp");

        //    ParseSourceFile(path);
        //}

        //public static bool ParseSourceFile(string file)
        //{
        //    // Lets setup the options for parsing the file.
        //    var parserOptions = new ParserOptions
        //    {
        //        LanguageVersion = LanguageVersion.CPP14,
        //        MicrosoftMode = false,
        //        LibraryDirs = new List<string> { Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "include\\lib") },
        //        IncludeDirs = new List<string>
        //        {
        //            Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "include\\libvlcpp"),
        //            Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "include\\libvlcpp\\vlc")
        //        },
                
        //        // Verbose here will make sure the parser outputs some extra debugging
        //        // information regarding include directories, which can be helpful when
        //        // tracking down parsing issues.
        //        Verbose = true
        //    };

        //    // This will setup the necessary system include paths and arguments for parsing.
        //    // It will probe into the registry (on Windows) and filesystem to find the paths
        //    // of the system toolchains and necessary include directories.
        //    parserOptions.Setup();

        //    // We create the Clang parser and parse the source code.
        //    var parser = new ClangParser();
        //    var parserResult = parser.ParseSourceFile(file, parserOptions);

        //    // If there was some kind of error parsing, then lets print some diagnostics.
        //    if (parserResult.Kind != ParserResultKind.Success)
        //    {
        //        if (parserResult.Kind == ParserResultKind.FileNotFound)
        //            Console.Error.WriteLine($"{file} was not found.");

        //        for (uint i = 0; i < parserResult.DiagnosticsCount; i++)
        //        {
        //            var diag = parserResult.GetDiagnostics(i);

        //            Console.WriteLine("{0}({1},{2}): {3}: {4}",
        //                diag.FileName, diag.LineNumber, diag.ColumnNumber,
        //                diag.Level.ToString().ToLower(), diag.Message);
        //        }

        //        parserResult.Dispose();
        //        return false;
        //    }

        //    // Now we can consume the output of the parser (syntax tree).

        //    // First we will convert the output, bindings for the native Clang AST,
        //    // to CppSharp's managed AST representation.
        //    var astContext = ClangParser.ConvertASTContext(parserResult.ASTContext);
            
        //    // After its converted, we can dispose of the native AST bindings.
        //    parserResult.Dispose();

        //    // Now we can finally do what we please with the syntax tree.
        //    //foreach (var sourceUnit in astContext.TranslationUnits)
        //    //    Console.WriteLine(sourceUnit.FileName);

        //    return true;
        //}


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
                    
                    SystemIncludeDirs = new List<string> { libvlcHeaders, libvlcPluginsHeaders},
                };

                var options = driver.Options;

                options.GeneratorKind = GeneratorKind.CSharp;

                var module = options.AddModule("libvlcpp");

               

                module.IncludeDirs.AddRange(new []{ libvlcppHeaders, libvlcHeaders, libvlcPluginsHeaders });
                module.Headers.AddRange(new[]
                {
                    // working:
                    "vlc.hpp",
                    "structures.hpp",
                    "common.hpp",
                    "Internal.hpp",
                    //"Equalizer.hpp", //lots of stuff ignored but no errors
                    "MediaListPlayer.hpp",

                    // not working:
                    //"MediaDiscoverer.hpp",
                    //"MediaLibrary.hpp",
                    //"EventManager.hpp"
                    //"MediaList.hpp",
                    //"Instance.hpp",
                    //"MediaPlayer.hpp",
                    //"Media.hpp",
                    //"Dialog.hpp",
                });

        
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
