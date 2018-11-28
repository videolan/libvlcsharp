Imports LibVLCSharp.Shared

Module Program
    Sub Main(args As String())
        Core.Initialize()

        Using libVLC = New LibVLC()
            Dim video = New Media(libVLC, "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4",
                                  Media.FromType.FromLocation)

            Using mp = New MediaPlayer(video)
                video.Dispose()
                mp.Play()
                Console.ReadKey()
            End Using
        End Using
    End Sub
End Module