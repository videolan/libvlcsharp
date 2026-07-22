open System
open LibVLCSharp.Shared

[<EntryPoint>]
let main argv =
    let libVLC = new LibVLC(true)
    let mp = new MediaPlayer(libVLC)
    let media = new Media(libVLC, new Uri("https://streams.videolan.org/misc/unity-samples/BigBuckBunny.avi"))
    mp.Play(media) |> ignore
    media.Dispose()
    Console.ReadKey() |> ignore
    mp.Dispose()
    libVLC.Dispose()
    0
