# Unity-SunVox Plugin

This package integrates the SunVox audio library into Unity.
It provides import and playback of SunVox projects out-of-the-box, as well as organized and annotated code to aid developers in building their own implementation.
Most XML documentation is taken directly from the SunVox library page with a few adjustments and clarifications, see [here](https://warmplace.ru/soft/sunvox/sunvox_lib.php) for full documentation.

## Pattern looping
The SunVoxPlayer component has a pattern looping function which provides dynamic playback capability.
When a loop pattern label is entered, any pattern with a unique name that contains the label will be considered a loop pattern. E.g. mylabel0, mylabel1, mylabelFooBar.
The project will then only play the section of the project that is within the bounds of the current loop pattern and the playhead will jump to the start of whichever loop pattern is queued next.
