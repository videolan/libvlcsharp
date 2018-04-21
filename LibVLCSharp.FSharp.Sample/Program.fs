// Learn more about F# at http://fsharp.org

open System
open LibVLCSharp.Shared
open LibVLCSharp

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    Core.Initialize()
    let instance = new Instance()
    let mp = new MediaPlayer(instance)
    let media = new Media(instance, "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", Media.FromType.FromLocation);
    mp.Media <- media
    mp.Play() |> ignore
    let result = Console.ReadKey()
    0 // return an integer exit code
