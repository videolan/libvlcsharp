# To run the iOS sample

dotnet build -t:Run -f net8.0-ios -p:RuntimeIdentifier=ios-arm64 -p:_DeviceName=xxx

> a few ways to get the device name

- instruments -s devices
- xcrun simctl list
- From Xcode: Window -> Devices and Simulators -> Simulators. The Identifier value is the UDID.