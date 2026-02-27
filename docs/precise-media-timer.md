# PreciseMediaTimer

## Overview
`PreciseMediaTimer` is a helper class designed to provide smoother and more precise playback position updates for `MediaPlayer`.

In LibVLC, `TimeChanged` events are tied to decoded frames and may be emitted at irregular intervals. This can result in visible jumps in progress bars, especially at low frame rates or near the end of playback.

This helper interpolates the playback time between `TimeChanged` events using LibVLC’s monotonic clock (`LibVLC.Clock`), producing stable and high-frequency position updates suitable for UI scenarios.

---

## Problem

`MediaPlayer.TimeChanged` alone is often insufficient for smooth progress bars because:

- Events may be infrequent depending on the media frame rate
- Reported timestamps can slightly lag behind the actual playback position
- Near the end of playback, LibVLC may emit delayed timestamps that appear to go backwards

This can cause UI issues such as:
- Jittery progress bars
- Small backward jumps near the end (e.g. stuck around ~90–95%)

---

## Solution

`PreciseMediaTimer` addresses these issues by:

1. Listening to `MediaPlayer.TimeChanged` events
2. Storing the last known playback timestamp
3. Using `LibVLC.Clock` to interpolate the current playback time between events
4. Ignoring late backward timestamps near the end of the media (common VLC artefact)
5. Emitting normalized position updates (`0.0` to `1.0`) at a steady interval

This results in a smooth and stable progress bar without modifying LibVLC core behavior.

---

## Key Features

- Smooth interpolation between VLC time updates
- Handles delayed/backward timestamps near end of playback
- Supports user seeks correctly
- Lightweight and UI-framework agnostic
- Emits normalized position values (0.0 → 1.0)

---

## Usage

### 1. Create the timer

```csharp
var preciseTimer = new PreciseMediaTimer(mediaPlayer, libVLC);
```

### 2. Subscribe to position updates
```csharp
preciseTimer.PrecisePositionChanged += position =>
{
    // position is normalized between 0.0 and 1.0
    progressBar.Value = position;
};
```

### 3. Start the timer
```csharp
preciseTimer.Start();
```

### 4. Stop when not needed
```csharp
preciseTimer.Stop();
```

### 5. Dispose when finished
```csharp
preciseTimer.Dispose();
```


## Full Example

```csharp
var preciseTimer = new PreciseMediaTimer(mediaPlayer, libVLC);

preciseTimer.PrecisePositionChanged += pos =>
{
    // Marshal to UI thread if required (WPF / WinUI / Avalonia, etc.)
    Dispatcher.Invoke(() =>
    {
        progressBar.Value = pos;
    });
};

// Start playback
mediaPlayer.Play(media);
preciseTimer.Start();

// When closing the view / stopping playback
void OnClose()
{
    preciseTimer?.Stop();
    preciseTimer?.Dispose();
    preciseTimer = null;

    mediaPlayer?.Stop();
    mediaPlayer?.Dispose();

    libVLC?.Dispose();
}
```

## Threading Notes
`PrecisePositionChanged` is raised from a background timer thread.  
UI frameworks (WPF, WinUI, Avalonia, etc.) should marshal updates to the UI thread:

```csharp
preciseTimer.PrecisePositionChanged += pos =>
{
    Dispatcher.Invoke(() =>
    {
        progressBar.Value = pos;
    });
};
```

## Lifecycle Notes

`PreciseMediaTimer` should be stopped and disposed when the associated `MediaPlayer` or view is closed.

Since the timer runs on a background thread and remains subscribed to `MediaPlayer.TimeChanged`, failing to dispose it may result in callbacks after the player or UI has been destroyed.

Example:
```csharp
preciseTimer.Stop();
preciseTimer.Dispose();
```

## When to Use
Use `PreciseMediaTimer` when you need:

- Smooth progress bar updates
- High-frequency position interpolation
- Stable UI feedback during playback

It is especially useful for:

- Media players
- Custom timeline controls
- Visual playback synchronization

## When Not to Use
If coarse position updates are sufficient and UI smoothness is not critical,  
`MediaPlayer.TimeChanged` alone may be enough.
