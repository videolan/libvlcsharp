using LibVLCSharp;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class LibVLCAPICoverage
    {
        const string LibVLCSymURL = "https://raw.githubusercontent.com/videolan/vlc/master/lib/libvlc.sym";
        const string LibVLCDeprecatedSymUrl = "https://raw.githubusercontent.com/videolan/vlc/master/include/vlc/deprecated.h";

        [Test]
        public async Task CheckLibVLCCoverage()
        {
            string[] libvlcSymbols;
            string[] libvlcdeprecatedSym;

            using (var httpClient = new HttpClient())
            {
                libvlcSymbols = (await httpClient.GetStringAsync(LibVLCSymURL)).Split(new[] { '\r', '\n' }).Where(s => !string.IsNullOrEmpty(s)).ToArray();
                libvlcdeprecatedSym = (await httpClient.GetStringAsync(LibVLCDeprecatedSymUrl)).Split(new[] { '\r', '\n' }).Where(s => !string.IsNullOrEmpty(s)).ToArray();
            }

            List<string> dllImports = new List<string>();

            // retrieving EventManager using reflection because the type is internal
            var eventManager = typeof(MediaPlayer).GetRuntimeProperties()
                .First(n => n.Name.Equals("EventManager"))
                .PropertyType
                .BaseType;

            var libvlcTypes = new List<Type>
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
                typeof(Picture),
                eventManager
            };

            var deprecatedSymbolsLine = new List<string>();

            for (var i = 0; i < libvlcdeprecatedSym.Count(); i++)
            {
                var currentLine = libvlcdeprecatedSym[i];
                if(currentLine.StartsWith("LIBVLC_DEPRECATED"))
                {
                    deprecatedSymbolsLine.Add(libvlcdeprecatedSym[i + 1]);
                }
            }

            var deprecatedSymbols = new List<string>();

            foreach (var symLine in deprecatedSymbolsLine)
            {
                var libvlcIndexStart = symLine.IndexOf("libvlc");
                var sym1 = symLine.Substring(libvlcIndexStart);
                var finalSymbol = new string(sym1.TakeWhile(c => c != '(').ToArray());

                if (finalSymbol.Contains('*'))
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

            // not implemented symbols for lack of use case or user interest
            List<string> notImplementedOnPurpose = new List<string>
            {
                "libvlc_printerr", "libvlc_vprinterr", "libvlc_clock", "libvlc_dialog_get_context", "libvlc_dialog_set_context",
                "libvlc_event_type_name", "libvlc_log_get_object", "libvlc_vlm", "libvlc_media_list_player", "libvlc_media_library"
            };

            List<string> exclude = new List<string>();
            exclude.AddRange(implementedButHidden);
            exclude.AddRange(notImplementedOnPurpose);

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

            var unusedDllImports = dllImports.Except(libvlcSymbols);

            var missingApis = libvlcSymbols
                .Where(symbol => !exclude.Any(excludeSymbol => symbol.StartsWith(excludeSymbol))) // Filters out excluded symbols
                .Except(dllImports)
                .Except(deprecatedSymbols);

            var missingApisCount = missingApis.Count();

            Debug.WriteLine($"we have {dllImports.Count} dll import statements");
            Debug.WriteLine($"{missingApisCount} missing APIs implementation");

            foreach (var miss in missingApis)
            {
                Debug.WriteLine(miss);
            }

            var unusedDllImportsCount = unusedDllImports.Count();
            Debug.WriteLine($"{unusedDllImportsCount} unused DllImports implementation");
            foreach (var unused in unusedDllImports)
            {
                Debug.WriteLine(unused);
            }

            Assert.Zero(missingApisCount);
            Assert.Zero(unusedDllImportsCount);
        }
    }
}
