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
        const string LibVLCSymURL = "https://code.videolan.org/videolan/vlc/-/raw/master/lib/libvlc.sym";

        [Test]
        public async Task CheckLibVLCCoverage()
        {
            string[] libvlcSymbols;

            using (var httpClient = new HttpClient())
            {
                libvlcSymbols = (await httpClient.GetStringAsync(LibVLCSymURL)).Split(new[] { '\r', '\n' }).Where(s => !string.IsNullOrEmpty(s)).ToArray();
                
            }

            var dllImports = new List<string>();

            // retrieving EventManager using reflection because the type is internal
            var eventManager = typeof(MediaPlayer).GetRuntimeProperties()
                .First(n => n.Name.Equals("EventManager"))
                .PropertyType
                .BaseType!;

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
                typeof(PictureList),
                typeof(MediaTrack),
                typeof(MediaTrackList),
                typeof(ProgramList),
                eventManager
            };

            var implementedButHidden = new List<string>
            {
                "libvlc_media_player_set_android_context", // android build only
                "libvlc_free" // hidden in internal type
            };

            // not implemented symbols for lack of use case or user interest
            var notImplementedOnPurpose = new List<string>
            {
               "libvlc_media_list_player", "libvlc_dialog_get_context", "libvlc_dialog_set_context", "libvlc_log_get_object", "libvlc_media_player_lock", "libvlc_media_player_signal", "libvlc_media_player_unlock", "libvlc_media_player_wait"
            };

            var exclude = new List<string>();
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
                            if (arg == default) continue;

                            var sym = (string)arg.TypedValue.Value!;

                            dllImports.Add(sym);
                        }
                    }
                }
            }

            var unusedDllImports = dllImports.Except(libvlcSymbols);

            var missingApis = libvlcSymbols
                .Where(symbol => !exclude.Any(excludeSymbol => symbol.StartsWith(excludeSymbol))) // Filters out excluded symbols
                .Except(dllImports);

            var missingApisCount = missingApis.Count();

            Debug.WriteLine($"we have {dllImports.Count} dll import statements");

            Assert.Zero(missingApis.Count(), string.Concat("missing APIs are: ", string.Join(", ", missingApis)));

            Assert.Zero(unusedDllImports.Count(), string.Concat("unused dll imports are: ", string.Join(", ", unusedDllImports)));
        }
    }
}
