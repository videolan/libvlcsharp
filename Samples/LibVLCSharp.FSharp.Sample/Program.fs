open System
open LibVLCSharp.Shared

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    Core.Initialize()
    let libVLC = new LibVLC()
    let mp = new MediaPlayer(libVLC)
    mp.Play(new Media(libVLC, "https://download.blender.org/peach/bigbuckbunny_movies/BigBuckBunny_320x180.mp4", FromType.FromLocation)) |> ignore
    let result = Console.ReadKey()
    0