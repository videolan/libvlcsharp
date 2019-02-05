using LibVLCSharp.Shared;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class LibVLCAPICoverage
    {
        const string LibVLCSym3URL = "https://raw.githubusercontent.com/videolan/vlc-3.0/3.0.0/lib/libvlc.sym";
        const string LibVLCDeprecatedSymUrl = "https://raw.githubusercontent.com/videolan/vlc-3.0/master/include/vlc/deprecated.h";

        [Test]
        public async Task CheckLibVLCCoverage()
        {
            var symPath = Path.GetTempFileName();
            var deprecatedSymPath = Path.GetTempFileName();

            using (var client = new WebClient())
            {                
                client.DownloadFile(LibVLCSym3URL, symPath);
                client.DownloadFile(LibVLCDeprecatedSymUrl, deprecatedSymPath);
            }
            
            List<string> dllImports = new List<string>();

            var eventManager = typeof(MediaPlayer).GetRuntimeProperties()
                .FirstOrDefault(n => n.Name.EndsWith("EventManager"))
                .PropertyType
                .BaseType;

            List<Type> libvlcTypes = new List<Type>
            {
                typeof(LibVLC),
                typeof(MediaPlayer),
                typeof(Media),
                typeof(MediaDiscoverer),
                typeof(RendererDiscoverer),
                typeof(RendererItem),
                typeof(Dialog),
                typeof(MediaList),
                typeof(Equalizer),
                eventManager
            };

            var deprecatedSymbolsLine = new List<string>();

            var deprecatedSymFileLines = await File.ReadAllLinesAsync(deprecatedSymPath);
            for (var i = 0; i < deprecatedSymFileLines.Count(); i++)
            {
                var currentLine = deprecatedSymFileLines[i];
                if(currentLine.StartsWith("LIBVLC_DEPRECATED"))
                {
                    deprecatedSymbolsLine.Add(deprecatedSymFileLines[i + 1]);
                }
            }

            var deprecatedSymbols = new List<string>();

            foreach (var symLine in deprecatedSymbolsLine)
            {
                var libvlcIndexStart = symLine.IndexOf("libvlc");
                var sym1 = symLine.Substring(libvlcIndexStart);
                var finalSymbol = new string(sym1.TakeWhile(c => c != '(').ToArray());
                if(finalSymbol.Contains('*'))
                {
                    finalSymbol = finalSymbol.Substring(finalSymbol.IndexOf('*') + 1);
                }
                
                deprecatedSymbols.Add(finalSymbol.Trim());
            }

            List<string> implementedButHidden = new List<string>
            {
                "libvlc_media_player_set_android_context", // android build only
                "libvlc_free" // hidden in internal type
            };

            List<string> internalSymbolsThatShouldNotBeThere = new List<string>
            {
                "libvlc_get_input_thread", "libvlc_media_new_from_input_item", "libvlc_media_set_state"
            };

            List<string> NotGonnaImplementYetUntilActualUserDemandAndIfItMakesSense = new List<string>
            {
                "libvlc_printerr", "libvlc_vprinterr", "libvlc_clock", "libvlc_dialog_get_context", "libvlc_dialog_set_context",
                "libvlc_event_type_name", "libvlc_log_get_object", "libvlc_vlm", "libvlc_media_list_player", "libvlc_media_library"
            };

            List<string> exclude = new List<string>();
            exclude.AddRange(implementedButHidden);
            exclude.AddRange(internalSymbolsThatShouldNotBeThere);
            exclude.AddRange(NotGonnaImplementYetUntilActualUserDemandAndIfItMakesSense);

            foreach (var libvlcType in libvlcTypes)
            {
                var r = libvlcType.GetNestedType("Native", BindingFlags.NonPublic);
                if (r == null) continue;

                foreach (var method in r.GetRuntimeMethods())
                {
                    foreach (var attr in method.CustomAttributes)
                    {
                        if (attr.AttributeType.Name.Equals("DllImportAttribute"))
                        {
                            var arg = attr.NamedArguments.FirstOrDefault(a => a.MemberName.Equals("EntryPoint"));
                            if (arg == null) continue;

                            var sym = (string)arg.TypedValue.Value;

                            dllImports.Add(sym);
                        }
                    }
                }
            }

            bool ShouldExclude(string symbol)
            {
                foreach(var excludeSymbol in exclude)
                {
                    if (symbol.StartsWith(excludeSymbol))
                        return true;
                }
                return false;
            }

            Debug.WriteLine($"we have {dllImports.Count} dll import statements");

            List<string> libvlcSymFile = new List<string>();
            foreach(var sym in File.ReadLines(symPath))
            {
                if (ShouldExclude(sym)) continue;

                libvlcSymFile.Add(sym);
            }

            var missingApis = libvlcSymFile
                .Except(dllImports)
                .Except(deprecatedSymbols)
                .ToList();

            Debug.WriteLine($"{missingApis.Count} missing APIs implementation");
            foreach(var miss in missingApis)
            {
                Debug.WriteLine(miss);
            }

            Assert.Zero(missingApis.Count);
        }
    }
}