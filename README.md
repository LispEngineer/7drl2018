A Seven Day Roguelike entry for 2018. The actual game will not be built
until the 7DRL starts. For now, just building a UI framework.

# Credits & Dependencies

* Rider 2017.3
* Unity 2017.3.0f3 (previously 2018.1.0b3)
* Arcadia: `git submodule add https://github.com/arcadia-unity/Arcadia.git Arcadia`
  on commit `653f2bf8224e247f0b78d3ce962b5f76a0e1ff3b`
* TextMesh Pro
* [RLTiles](https://github.com/statico/rltiles)
* [Source Code Pro (font)](https://github.com/adobe-fonts/source-code-pro)

Clone from git with: `git clone --recursive ...` to get Arcadia too.

# Philosophy

Game logic in Clojure.
Display logic in C#.
Unity for game engine.

# Participants

* Douglas P. Fields, Jr.
  * Twitter: LispEngineer
  * GitHub: LispEngineer
  * Site: https://symbolics.lisp.engineer/
  * Email: symbolics@lisp.engineer


# How To Play

* UI Management:
  * Zoom the playfield: `Command-=`, `Command--` and `Command-0`
  * Zoom the UI: Add `Shift` to the above
  * (Use left modifiers for now. Windows not tested.)
  * Open/close message log: Backtick/Backquote
  * Scroll message log: Scroll wheel over message log window
* Movement:
  * VI Keys: `HJKL` for orthogonal, and `YUBN` for diagonal


# Current State

* Draws a series of (stacked) tiles in the playfield
* Moves the camera around with movement keys
* Zooms the playfield and UI independently


# Unity Notes

* If the standalone build's GUI looks all pink,
  [reset the project graphics settings](https://forum.unity.com/threads/everything-canvas-turns-pink-when-playing-windows-build.411603/).

* Unity 2018.1.0b3 is very unstable with Arcadia installed. I temporarily
  deleted it from my local copy and things run much faster and don't seem
  to crash. (macOS 10.12 w/ current 13" MBP) I should test it with 2017.3
  and see if that helps.
  
* `CanvasScaler` does not update when `Canvas.ForceUpdateCanvases()`
  is called, but only when its `protected Update()` method is called.
  So, this needs to be called manually (with `SendMessage("Update")`)
  when necessary.


# Copyright & License

Copyright 2018 Douglas P. Fields, Jr.

Current license: All Rights Reserved.

Planned license: AGPL
