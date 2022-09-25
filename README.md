# PNGBuddy
A PNGTuber program similar to [veadotube mini](https://olmewe.itch.io/veadotube-mini?download) or Discord Reactive Images to give your PNG avatars a little bit more expression and life. Integrates directly into OBS using OBS-websockets.

## Requirements
- OBS (v28.0.0 or later)
- .NET 6.0 Runtime (Included in installation)
- Windows Only

## Features
- Multiple poses
  - Idle (open eyes, closed mouth)
  - Speaking (open eyes, open mouth)
  - Blinking (closed eyes, closed mouth)
  - Blinking + Speaking (closed eyes, open mouth) 
- Transparency/Chroma key
  - Completely transparent
  - Custom color to apply chroma key filtering via OBS
- Configurable blink duration and interval
- Configurable mic threshold (based on current OBS meters)
- Grayscale idle image (when OBS input is muted)
  - Can be toggled on or off
- Works with or without OBS Websocket Authentication
  - Enabled by default
  - Changes on OBS (names of inputs, muted state, etc.) are reflected in the application

## Installing
The installer for PNGBuddy uses ClickOnce and will install the .NET 6 Runtime and automatically check for update when launching. You can download it here: 
[PNGBuddy](https://raw.githubusercontent.com/UnseenFaith/PNGBuddy/master/Installer/setup.exe)

May add portable downloads in the future, but for now this should suffice.

## To-Do / Suggestions
- Custom Animations
  - Some examples: increasing avatar size when talking, or shaking when input volume is too loud to give the impression of being loud or angry
- Profiles
  - Ability to create multiple profiles that allows the user to quickly select different avatars without having to select each one manually.
- Revert Window Chrome
  - Custom window chromes are nice, but it messes with the client area recognition of Windows. I will probably revert the window chrome and change the way users access the settings window in the future.
- Make Application handle it's own scene
  - Currently the user has to add a scene manually for the application to work. This could be extracted and done by the program automatically so that the user doesn't have to worry about it. There's almost no benefit to implementing this other than convenience, but it is possible.

> I'm open to any other suggestions or ideas


## FAQ
- The titlebar of the application is captured by OBS
  - Since I created a custom window chrome, OBS will capture the titlebar. You can easily crop it by clicking on the top of the scene, holding down alt, and cropping it out. I'll probably remove the custom window chrome in the future.









