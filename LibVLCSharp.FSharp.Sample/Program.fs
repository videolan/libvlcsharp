open System
open LibVLCSharp.Shared
open LibVLCSharp

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    Core.Initialize()
    let instance = new Instance()
    let mp = new MediaPlayer(instance)
    mp.Play(new Media(instance, "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", Media.FromType.FromLocation)) |> ignore
    let result = Console.ReadKey()
    0