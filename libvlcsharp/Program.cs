using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Parser;
using CppSharp.Passes;
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
                var libnewF = ctx.FindFunction("libvlc_new").Single();
                var instance = ctx.FindClass("libvlc_instance_t").Single();

                var m = new Method(libnewF)
                {
                    Access = AccessSpecifier.Public,
                    Kind = CXXMethodKind.Constructor,
                    IsFinal = true
                };
                
                instance.Methods.Add(m);
            }

            public void Postprocess(Driver driver, ASTContext ctx)
            {
               // TODO: rename LibvlcCallbackT to VLCCallback
                RenameClasses(ctx);
                RenameEnums(ctx);

                ctx.SetClassAsValueType("Event");
            }

            private void RenameClasses(ASTContext ctx)
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
                Rename("LibvlcEventT", "Event");
                Rename("LibvlcDialogId", "DialogId");
                Rename("LibvlcDialogCbs", "DialogCallback");
                Rename("LibvlcLogIteratorT", "LogIterator");
                Rename("LibvlcLogMessageT", "LogMessage");
            }

            private void RenameEnums(ASTContext ctx)
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
                Rename("LibvlcEventE", "EventType", strip: "Libvlc");

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
                var rootPath = Directory.GetParent(Assembly.GetExecutingAssembly().Location).Parent.Parent.Parent.FullName;
                var h = Path.Combine(rootPath, "include");
                var libFolder = Path.Combine(rootPath, "include\\lib");

                driver.ParserOptions = new ParserOptions
                {
                    IncludeDirs = new List<string> { h },
                    //Verbose = true
                };
                
                var options = driver.Options;
                options.GeneratorKind = GeneratorKind.CSharp;
                options.CompileCode = true;
                options.OutputDir = Path.Combine(rootPath, "..\\Sample\\Generated\\");
                options.StripLibPrefix = false;
                options.GenerateSingleCSharpFile = false;
                //options.CheckSymbols = true;

                var module = options.AddModule("libvlcsharp.generated");
                module.SharedLibraryName = "libvlc";
                module.Headers.Add("vlc\\vlc.h");
                module.LibraryDirs.Add(libFolder);
            }

            public void SetupPasses(Driver driver)
            {
                driver.AddTranslationUnitPass(new MoveCustomFunctionToClassPass());
                driver.AddTranslationUnitPass(new RenameEventClasses());
            }

            private class MoveCustomFunctionToClassPass : TranslationUnitPass
            {
                public override bool VisitFunctionDecl(Function function)
                {
                    if (function.Name == "libvlc_new")
                    {
                        var instance = ASTContext.FindClass("libvlc_instance_t").Single();
                        MoveFunction(function, instance);
                    }
                    
                    return true;
                }

                private static void MoveFunction(Function function, Class @class)
                {
                    // TODO: Need to make a constructor which passes its params to the native libvlc_new call and store the returned pointer in field
                    // TODO: also add finalizer with libvlc_release and pass the pointer
                    var method = new Method(function)
                    {
                        Namespace = @class,
                        Kind = CXXMethodKind.Constructor,
                        
                    };
                    //function.ExplicitlyIgnore();

              
                    //@class.Methods.Add(method);
                    //@class.Functions.Add(function);

                    function.Namespace = @class;
                    function.OriginalNamespace = @class;                    
                    function.Access = AccessSpecifier.Internal;
                }
            }
        }
    }

    internal class RenameEventClasses : TranslationUnitPass
    {
        public override bool VisitClassDecl(Class @class)
        {
            if (@class.Name == string.Empty && @class.Fields.Count == 1)
            {
                @class.Name = @class.Fields.First().Name;
                return true;
            }
            return base.VisitClassDecl(@class);
        }
    }
}