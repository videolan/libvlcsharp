using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
            Console.ReadKey();
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

                // TODO: rename LibvlcCallbackT to VLCCallback

                CustomizeClasses(ctx);
                CustomizeEnums(ctx);
            }

            private void CustomizeClasses(ASTContext ctx)
            {
                void Rename(string originalName, string newName)
                {
                    ctx.FindClass(originalName).Single().Name = newName;
                }

                Rename("LibvlcInstanceT", "Instance");
                Rename("LibvlcEventManagerT", "EventManager");
                Rename("LibvlcModuleDescriptionT", "ModuleDescription");
                Rename("LibvlcMediaTrackT", "MediaTrack");
                Rename("LibvlcMediaPlayerT", "MediaPlayer");
                Rename("LibvlcEqualizerT", "Equalizer");
                Rename("LibvlcTrackDescriptionT", "TrackDescription");
                Rename("VlcLogT", "Log");
                Rename("LibvlcRendererItemT", "RendererItem");
                Rename("LibvlcRendererDiscovererT", "RendererDiscoverer");
                Rename("LibvlcRdDescriptionT", "RendererDiscovererDescription");
                Rename("LibvlcMediaT", "Media");
                Rename("LibvlcMediaListT", "MediaList");
                Rename("LibvlcMediaStatsT", "MediaStats");
                Rename("LibvlcMediaTrackInfoT", "MediaTrackInfo");
                Rename("LibvlcAudioTrackT", "AudioTrack");
                Rename("LibvlcVideoTrackT", "VideoTrack");
                Rename("LibvlcSubtitleTrackT", "SubtitleTrack");
                Rename("LibvlcMediaSlaveT", "MediaSlave");
                Rename("LibvlcTitleDescriptionT", "TitleDescription");
                Rename("LibvlcChapterDescriptionT", "ChapterDescription");
                Rename("LibvlcAudioOutputT", "AudioOutput");
                Rename("LibvlcAudioOutputDeviceT", "AudioOutputDevice");
                Rename("LibvlcVideoViewpointT", "VideoViewpoint");
                Rename("LibvlcMediaDiscovererT", "MediaDiscoverer");
                Rename("LibvlcMediaDiscovererDescriptionT", "MediaDiscovererDescription");
            }

            private void CustomizeEnums(ASTContext ctx)
            {
                Rename("LibvlcLogLevel", "LogLevel", true, "LIBVLC_");
                Rename("LibvlcStateT", "VLCState", strip: "Libvlc");
                Rename("LibvlcMediaOption", "MediaOption", strip: "LibvlcMediaOption");
                Rename("LibvlcTrackTypeT", "TrackType", strip: "LibvlcTrack");
                Rename("LibvlcVideoOrientT", "VideoOrientation", strip: "LibvlcVideoOrient");
                Rename("LibvlcVideoProjectionT", "VideoProjection", strip: "LibvlcVideoProjection");
                Rename("LibvlcMetaT", "MetadataType", strip: "LibvlcMeta");
                Rename("LibvlcMediaTypeT", "MediaType", strip: "LibvlcMediaType");
                Rename("LibvlcMediaParseFlagT", "MediaParseOptions", strip: "LibvlcMedia");
                Rename("LibvlcMediaParsedStatusT", "MediaParsedStatus", strip: "LibvlcMediaParsedStatus");
                Rename("LibvlcMediaSlaveTypeT", "MediaSlaveType", strip: "LibvlcMediaSlaveType");
                Rename("LibvlcTitle", "Title", strip: "LibvlcTitle");
                Rename("LibvlcVideoMarqueeOptionT", "VideoMarqueeOption", strip: "LibvlcMarquee");

                Rename("LibvlcNavigateModeT", "NavigationMode", strip: "LibvlcNavigate");
                Rename("LibvlcPositionT", "Position", strip: "LibvlcPosition");
                Rename("LibvlcTeletextKeyT", "TeletextKey", strip: "LibvlcTeletextKey");

                Rename("LibvlcVideoLogoOptionT", "VideoLogoOption", strip: "LibvlcLogo");
                Rename("LibvlcVideoAdjustOptionT", "VideoAdjustOption", strip: "LibvlcAdjust");
                Rename("LibvlcAudioOutputDeviceTypesT", "AudioOutputDeviceType", strip: "LibvlcAudioOutput");
                Rename("LibvlcAudioOutputChannelT", "AudioOutputChannel", strip: "LibvlcAudioChannel");
                Rename("LibvlcMediaPlayerRole", "MediaPlayerRole", strip: "LibvlcRole");
                Rename("LibvlcPlaybackModeT", "PlaybackMode", strip: "LibvlcPlaybackMode");
                Rename("LibvlcMediaDiscovererCategoryT", "MediaDiscovererCategory", strip: "LibvlcMediaDiscoverer");
                Rename("LibvlcDialogQuestionType", "DialogQuestionType", true, "LIBVLC_DIALOG_QUESTION_");


                void Rename(string originalName, string newName, bool formatValues = false, string strip = null)
                {
                    var match = ctx.FindEnum(originalName).Single();
                    match.Name = newName;
                    if (string.IsNullOrEmpty(strip)) return;
                    match.Items.ForEach(item =>
                    {       
                        var name = item.Name.Replace(strip, string.Empty);              
                        item.Name = formatValues
                            ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.ToLower())
                            : name;
                    });
                }
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
                    //Verbose = true
                };
                
                var options = driver.Options;
                options.GeneratorKind = GeneratorKind.CSharp;
                options.CompileCode = true;
                options.OutputDir = Path.Combine(rootPath, "..\\Sample\\");

                var module = options.AddModule("libvlcsharp");
                module.Headers.Add("vlc.h");
                module.LibraryDirs.Add(libFolder);
                module.Libraries.AddRange(new[] { "libvlc.lib", "libvlccore.lib" });
            }

            public void SetupPasses(Driver driver)
            {
            }
        }
    }
}