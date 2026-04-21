open System
open LibVLCSharp.Shared

[<EntryPoint>]
let main argv =
    let libVLC = new LibVLC(true)
    let mp = new MediaPlayer(libVLC)
    let media = new Media(libVLC, new Uri("https://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_480p_stereo.avi"))
    mp.Play(media) |> ignore
    media.Dispose()
    Console.ReadKey() |> ignore
    mp.Dispose()
    libVLC.Dispose()
    0
