![Logo](source/EyeTrackingVsix/Resources/logo-eye-code-175.png)

# vs-eye-tracking

Extension for Visual Studio using eye tracking

## Status

Branch | Status | Download
------ | ------ | --------
master |  [![Build Status](https://danielsa.visualstudio.com/vs-eye-tracking/_apis/build/status/vs-eye-tracking-CI?branchName=master)](https://danielsa.visualstudio.com/vs-eye-tracking/_build/latest?definitionId=14&branchName=master) | [CI Build](http://vsixgallery.com/extension/65bd244c-48a0-46af-953a-0fb433e5343d/)
develop | [![Build Status](https://danielsa.visualstudio.com/vs-eye-tracking/_apis/build/status/vs-eye-tracking-CI?branchName=develop)](https://danielsa.visualstudio.com/vs-eye-tracking/_build/latest?definitionId=14&branchName=develop)

## Features
 
 - [x] Basic scrolling (while holding down a keyboard key. Currently Right-Ctrl, but it might change)
 - [x] Basic way of moving the caret to where you look (Currently by tapping Right-Shift, but it will change)

## TODO / Ideas
 
 - [x] Improve handling of keyboard events to detect double tap, and double tap and hold
 - [x] Publish preview releases to wsixgallery.com
   - [x] Update texts in the vsix manifest
 - [ ] Some kind of error reporting in UI and maybe upload to remote service
 - [ ] Add configuration UI
   - [ ] Select what keyboard keys to use
   - [ ] Enabe/disable individual features
