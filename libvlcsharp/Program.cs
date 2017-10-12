using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CppSharp;
using CppSharp.AST;
using CppSharp.AST.Extensions;
using CppSharp.Generators;
using CppSharp.Parser;
using CppSharp.Passes;
using CppSharp.Types;
using Type = CppSharp.AST.Type;

namespace libvlcsharp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ConsoleDriver.Run(new LibVLC());
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        class LibVLC : ILibrary
        {
            public void Preprocess(Driver driver, ASTContext ctx)
            {
                //ctx.IgnoreHeadersWithName("libvlc_events.h");
                ctx.FindClass("VaCopy").First().Ignore = true;
                ctx.TranslationUnits.FirstOrDefault(tu => tu.FileName == "libvlc_events.h").Ignore = true;
                //ctx.TranslationUnits.FirstOrDefault(tu => tu.FileName == "libvlc_events.h").ExplicitlyIgnore();
                //ctx.TranslationUnits.FirstOrDefault(tu => tu.FileName == "libvlc_events.h").GenerationKind = GenerationKind.None;

            }

            public void Postprocess(Driver driver, ASTContext ctx)
            {
                //ctx.SetClassAsValueType("VaCopy");
                var ctorsToFix = new List<IEnumerable<Method>>
                {
                    ctx.FindClass("MediaPlayer").First().Constructors,
                    ctx.FindClass("Media").First().Constructors,
                    ctx.FindClass("MediaList").First().Constructors,
                    ctx.FindClass("MediaListPlayer").First().Constructors,
                    ctx.FindClass("MediaLibrary").First().Constructors
                };

                foreach (var ctors in ctorsToFix)
                {
                    FixCtorParamNameHack(ctors);
                }
            }

            private void FixCtorParamNameHack(IEnumerable<Method> ctors)
            {
                foreach (var ctor in ctors)
                {
                    for (var i = 0; i < ctor.Parameters.Count; i++)
                    {
                        ctor.Parameters[i].Name = ctor.Parameters[i].Name + i;
                    }
                }
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
                    IncludeDirs = new List<string> {libvlcppHeaders},
                    LibraryDirs = new List<string> {libFolder},
                    LibraryFile = "libvlcpp",
                    //Verbose = true,
                    EnableRTTI = true,
                    Arguments = new List<string> {"-fcxx-exceptions"},
                    LanguageVersion = LanguageVersion.CPP14,
                    //SystemIncludeDirs = new List<string> { "C:\\Program Files (x86)\\Microsoft Visual Studio\\2017\\Community\\VC\\Tools\\MSVC\\14.11.25503" }
                };

                var options = driver.Options;
                options.GeneratorKind = GeneratorKind.CSharp;
                options.CompileCode = true;
                
                var module = options.AddModule("libvlcsharp");
                
                module.IncludeDirs.AddRange(new[] {libvlcppHeaders});
                module.Headers.Add("vlc.hpp");

                module.LibraryDirs.Add(libFolder);
                module.Libraries.AddRange(new[] {"libvlc.lib", "libvlccore.lib"});
            }

            public void SetupPasses(Driver driver)
            {
                //driver.Context.TranslationUnitPasses.AddPass(new ResolveIncompleteDeclsPass());
                //driver.Context.TranslationUnitPasses.AddPass(new MarshalPrimitivePointersAsRefTypePass());
                //driver.Context.TranslationUnitPasses.AddPass(new CleanInvalidDeclNamesPass());
                //driver.Context.TranslationUnitPasses.AddPass(new CheckIgnoredDeclsPass());
                //driver.Context.TranslationUnitPasses.RenameDeclsUpperCase(RenameTargets.Any);
                //driver.Context.TranslationUnitPasses.AddPass(new FunctionToInstanceMethodPass());
                //driver.Context.TranslationUnitPasses.AddPass(new CheckAmbiguousFunctions());
                //driver.Context.TranslationUnitPasses.AddPass(new StripUnusedSystemTypesPass());
                //driver.Context.TranslationUnitPasses.AddPass(new CheckDuplicatedNamesPass());
                //driver.Context.TranslationUnitPasses.AddPass(new FieldToPropertyPass());
            }
        }
    }
}