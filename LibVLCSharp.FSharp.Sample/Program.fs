open System
open LibVLCSharp.Shared

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    Core.Initialize()
    let libVLC = new LibVLC()
    let mp = new MediaPlayer(libVLC)
    mp.Play(new Media(libVLC, "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", Media.FromType.FromLocation)) |> ignore
    let result = Console.ReadKey()
    0