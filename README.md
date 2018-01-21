A Seven Day Roguelike entry for 2018. The actual game will not be built
until the 7DRL starts. For now, just building a UI framework.

# Dependencies

* Rider 2017.3
* Unity 2018.1.0b3
* Arcadia: git submodule add https://github.com/arcadia-unity/Arcadia.git Arcadia
  on commit 653f2bf8224e247f0b78d3ce962b5f76a0e1ff3b
* TextMesh Pro
* [RLTiles](https://github.com/statico/rltiles)

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
* Movement:
  * VI Keys: `HJKL` for orthogonal, and `YUBN` for diagonal


# Current State

* Draws a series of (stacked) tiles in the playfield
* Moves the camera around with movement keys
* Zooms the playfield and UI independently


# Unity Notes

* If the standalone build's GUI looks all pink,
  [reset the project graphics settings](https://forum.unity.com/threads/everything-canvas-turns-pink-when-playing-windows-build.411603/).



# Copyright & License

Copyright 2018 Douglas P. Fields, Jr.

Current license: All Rights Reserved.

Planned license: AGPL
