# Fall Guys Sharp
A tool that adds easy mode to Fall Guys.

This project shows how to inject Unity modules during runtime into il2cpp Unity games.

## Features
 - WinRace - teleports to the finish line on applicable stages
![Win Race](demo/winrace.gif)
 - Freeze Position - freezes current position to prevent falling
![Freeze Position](demo/freezepos.gif)
 - LevelHelper - removes fake doors/tiles
![Level Helper](demo/levelhelper.gif)
 
## Todo / current issues
 - Occasionally crashes on injection
 - Deployment / packaging requries lots of DLL's, would be nice to have a single exe. Fody doesn't work as is.
 - Dumper needs to be manually run on game update
 - Most of the logic of Gunfire Reborn is done outside of Unity in a native dll / python.
 - App that does the injection is just a simple winform - need to clean this up.

## Credits
https://github.com/Perfare/Il2CppDumper - dumps out dummy DLLs from Unity il2cpp game

https://github.com/knah/Il2CppAssemblyUnhollower - converts dummy DLLs from Il2CppDumper into native calls. knah also helped me debug along the way

https://github.com/warbler/SharpMonoInjector - calls mono functions after mono is loaded (forked to inject Mono)

