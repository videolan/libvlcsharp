# Android

This page is about the Android specifics in LibVLCSharp and LibVLC.

## How do I use R8?

- Create a new file in your Xamarin.Android app root named "r8.cfg".
- In this file properties, set the Build Action to "ProguardConfiguration".

In this file, add the following lines:

```
-keep class org.videolan.** { *; }
-dontwarn org.videolan.**
```

https://code.videolan.org/videolan/LibVLCSharp/issues/255#note_55737