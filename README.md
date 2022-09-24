# PNGBuddy
A PNGTuber program similar to [veadotube mini](https://olmewe.itch.io/veadotube-mini?download) or Discord Reactive Images to give your PNG avatars a little bit more expression and life. Integrates directly into OBS using OBS-websockets.

## Requirements
- OBS (at least v28.0.0)
- .Net 6.0 Runtime (will be installed via PNGBuddy)
- Windows Only

## Features
- Multiple Poses
  - Idle (Open Eyes, Closed Mouth)
  - Speaking (Open Eyes, Open Mouth)
  - Blinking (Closed Eyes, Closed Mouth)
  - Blinking + Speaking (Closed Eyes, Open Mouth) 
- Transparency/Chroma Key
  - Completely transparent
  - Custom Color to apply chroma key filtering via OBS
- Configurable Blink duration and interval
- Configurable Mic Threshold (based on current OBS meters)
- Grayscale Idle picture (when OBS input is muted)
  - Can be toggled on or off
- Works with or without OBS Websocket Authentication
  - Enabled by default
  - Will also reconnect every few seconds if connection is interrupted or lost
  - Changes on OBS (names of inputs, muted state, etc.) are reflected in the application

## Installing
The installer for PNGBuddy uses ClickOnce and will install the .Net 6 Runtime and automatically check for update when launching. You can download it here: 
[PNGBuddy Setup](https://raw.githubusercontent.com/UnseenFaith/PNGBuddy/master/Installer/setup.exe)

In the future I may add portable downloads but for now this should suffice.

## To-Do / Suggestions
- Custom Animations
  - I have very little animation knowledge, but there could be room to add animations, things like growing avatar size when talking, shaking when too loud to give the impression of loud or anger, etc.
- Profiles
  - Multiple profiles that allows you to quick select different avatars or different files without having to select each one manually
- Revert Window Chrome
  - Custom window chromes are nice, but it messes with client area recognition of Windows. Will probably revert the window chrome and move/change the way you access the settings window in the future
- Make Application handle it's own scene
  - Right now the user has to add a scene for the application themselves. This could be extracted and done by the program automatically so that you don't have to worry about it. Almost no benefit from doing this other than being convenient, but it's possible.

> I'm open to any other suggestions or ideas


## FAQ
- The titlebar of the application is captured by OBS
  - Since I created a custom window chrome, OBS will capture the titlebar. You can easily crop it by clicking on the top of the scene, holding down alt, and cropping it out. I'll probably remove the custom window chrome in the future.









