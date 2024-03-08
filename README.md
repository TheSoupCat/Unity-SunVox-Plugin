# Unity-SunVox Plugin

This package integrates the SunVox audio library into Unity.
It provides import and playback of SunVox projects out-of-the-box, as well as code and comments to help developers build their own implementation.
Most XML documentation is taken directly from the SunVox library page with a few adjustments and clarifications, see [here](https://warmplace.ru/soft/sunvox/sunvox_lib.php) for full documentation.

## Pattern looping
The SunVoxPlayer component has a pattern looping function which provides dynamic playback capability.
When a loop pattern label is entered, any pattern with a unique name that contains the label will be considered a loop pattern. E.g. mylabel0, mylabel1, mylabelFooBar.
During playback, the playhead will seamlessly loop between the bounds of the current loop pattern and will jump to the start of a newly queued loop pattern when reaching the end of the current one.