# STACK Engine

The STACK Engine is a free and open .NET framework dedicated to create 2D point & click graphic adventure games for desktop PCs. It is used to develop [Session Seven](https://github.com/advdotnet/Session-Seven) which is also open source and has been released for free.

## Build Status

[![Build status](https://ci.appveyor.com/api/projects/status/77gd7mq3v1332ade?svg=true)](https://ci.appveyor.com/project/advdotnet/stack-engine)

## Features

The engine's features were chosen carefully to make development of point & click adventures as easy and comfortable as possible while not getting in the way of the developer. Any game logic is written in C# enabling the broad range of .NET's capabilities. Great value was placed on having a testable architecture which allows to write tests that solve your whole game within just a fraction of a second and giving you immediate feedback if something doesn't work as expected.

* Entity Component System
* Testable architecture
* Walkable areas using a triangular mesh
* Pathfinding over game scenes
* Spine integration for skeletal animations
* Supported platforms: Windows, Linux, Mac OS
* Serializable game logic spanning multiple updates via .NET's IEnumerators

## License

STACK Engine is released under the Microsoft Public License. See LICENSE for details.
STACK Engine uses FNA, released under the Microsoft Public License. See LICENSE for details.
STACK Engine uses Neoforce Controls, released under the GNU Lesser Generic Public License. 

## Built With

* [Spine](https://github.com/EsotericSoftware/spine-runtimes) - 2D skeletal animation toolkit
* [Neoforce Controls](https://github.com/NeoforceControls/Neoforce-Mono) - UI library for XNA
* [FNA](https://github.com/FNA-XNA/FNA/) - Accuracy-focused XNA4 reimplementation for open platforms