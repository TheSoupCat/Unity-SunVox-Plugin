using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SunVoxIntegration
{
    class SunVoxUtility
    {
        public const string integrationTestedWithSunVoxVersion = "2.1.1c";

        /// <summary>
        /// Converts the version code produced when initializing SunVox into a regular string
        /// </summary>
        /// <param name="version">return value of sv_init goes here</param>
        /// <returns>version formatted as string</returns>
        public static string VersionAsString(int version)
        {
            if (version >= 0)
                return $"{version >> 16 & 255}.{version >> 8 & 255}.{version & 255}";
            else
                return $"error in version number: {version}";
        }

        /// <summary>
        /// Get a summary of stats of loaded SunVox project
        /// </summary>
        /// <param name="sunvoxSlot">the SunVox slot which you want the project info from</param>
        /// <returns>Project information and stats</returns>
        public static string SongInfo(int sunvoxSlot)
        {
            return string.Format("SunVox project contains {0} patterns in {1} pattern slots, {2} modules in {3} module slots, estimated length of project is: {4}:{5}, bpm = {6}, ticks per line = {7}"
                , GetNumberOfPatterns(sunvoxSlot), SunVoxLib.sv_get_number_of_patterns(sunvoxSlot)
                , GetNumberOfModules(sunvoxSlot), SunVoxLib.sv_get_number_of_modules(sunvoxSlot)
                , Mathf.Abs(SunVoxLib.sv_get_song_length_frames(sunvoxSlot) / AudioSettings.outputSampleRate / 60)
                , ((SunVoxLib.sv_get_song_length_frames(sunvoxSlot) / SunVoxLib.sv_get_sample_rate()) % 60).ToString("D2")
                , SunVoxLib.sv_get_song_bpm(sunvoxSlot), SunVoxLib.sv_get_song_tpl(sunvoxSlot));

            //return $"SunVox project contains {GetNumberOfPatterns(sunvoxSlot)} patterns in {SunVoxLib.sv_get_number_of_patterns(sunvoxSlot)} pattern slots, " +
            //    $"{GetNumberOfModules(sunvoxSlot)} modules in {SunVoxLib.sv_get_number_of_modules(sunvoxSlot)} module slots, estimated length of project is: " +
            //    $"{Mathf.Abs(SunVoxLib.sv_get_song_length_frames(sunvoxSlot) / AudioSettings.outputSampleRate / 60)}:{((SunVoxLib.sv_get_song_length_frames(sunvoxSlot) / SunVoxLib.sv_get_sample_rate()) % 60)}" +
            //    $", bpm = {SunVoxLib.sv_get_song_bpm(sunvoxSlot)}, ticks per line = {SunVoxLib.sv_get_song_tpl(sunvoxSlot)}";
        }

        /// <summary>
        /// Get the number of patterns in a project, derived from the number of pattern slots which is the smallest multiple of 16 the number of patterns fits into
        /// </summary>
        /// <param name="sunvoxSlot">the SunVox slot containing the project which you want the number of patterns from</param>
        /// <returns>Number of patterns</returns>
        public static int GetNumberOfPatterns(int sunvoxSlot)
        {
            //check first if the given SunVox slot is valid, and if there is a project loaded in the given SunVox slot
            if (sunvoxSlot < 0 || sunvoxSlot > 15 || SunVoxLib.sv_get_number_of_modules(sunvoxSlot) <= 0)
            {
                Debug.Log("invalid or empty SunVox slot ID, returning 0");
                return 0;
            }

            //this works because patterns are rearranged into the smallest number of pattern slots, newly created patterns with a higher ID will be reset to a lower ID when lower ID patterns are removed
            //module(-slots) don't work quite the same, which is why they are handled differently
            int startingIndex = SunVoxLib.sv_get_number_of_patterns(sunvoxSlot) - 16;
            for (int i = 0; i < 16; i++)
            {
                if (SunVoxLib.sv_get_pattern_lines(sunvoxSlot, startingIndex + i) <= 0)
                {
                    return startingIndex + i;
                }
            }
            //if all slots are filled, just return number of pattern slots
            return SunVoxLib.sv_get_number_of_patterns(sunvoxSlot);
        }

        /// <summary>
        /// Get the number of modules in a project, derived from the number of module slots. occupied module slots are non-consecutive, unlike pattern slots.
        /// </summary>
        /// <param name="sunvoxSlot">the SunVox slot which you want the number of modules from</param>
        /// <returns>Number of modules</returns>
        public static int GetNumberOfModules(int sunvoxSlot)
        {
            //check first if the given SunVox slot is valid, and if there is a project loaded in the given SunVox slot
            if (sunvoxSlot < 0 || sunvoxSlot > 15 || SunVoxLib.sv_get_number_of_modules(sunvoxSlot) <= 0)
            {
                Debug.Log("invalid or empty SunVox slot ID, returning 0");
                return 0;
            }

            int occupiedModuleSlots = 0;
            int moduleSlots = SunVoxLib.sv_get_number_of_modules(sunvoxSlot);
            for (int i = 0; i < moduleSlots; i++)
            {
                if (SunVoxLib.sv_get_number_of_module_ctls(sunvoxSlot, i) > 0) //this logic doesn't count the output module, hence the + 1 at the return statement
                {
                    occupiedModuleSlots++;
                }
            }
            return occupiedModuleSlots + 1;
        }

        /// <summary>
        /// Jump to a specific line while staying in sync with the track. Calling this more than once before the jump occurs(like in an update loop) will retrigger the jump multiple times.
        /// </summary>
        /// <param name="instanceID">SunVox slot number</param>
        /// <param name="lineNumber">The line number that should be queued to jump to</param>
        public static void JumpImmediateSync(int instanceID, int lineNumber) => SunVoxLib.sv_send_event(instanceID, 0, 0, 0, 0, 49, lineNumber);

        /// <summary>
        /// Set the BPM of the track
        /// </summary>
        /// <param name="instanceID">SunVox slot number</param>
        /// <param name="bpm">The beats per minute to set</param>
        public static void SetBpm(int instanceID, int bpm) => SunVoxLib.sv_send_event(instanceID, 0, 0, 0, 0, 31, bpm);

        /// <summary>
        /// Sets up a list of patterns whose names match a given prefix that can be used as looping sections in a project
        /// </summary>
        /// <param name="sunvoxSlot">SunVox slot number</param>
        /// <param name="loopPatternLabel">the string used in the names of patterns that designate it as a loop pattern</param>
        /// <param name="loopPatterns">the list in which the loop patterns are put</param>
        public static void FindLoopPatterns(int sunvoxSlot, string loopPatternLabel, Dictionary<string, int> loopPatterns)
        {
            //check first if the given SunVox slot is valid, and if there is a project loaded in the given SunVox slot
            if (sunvoxSlot < 0 || sunvoxSlot > 15 || SunVoxLib.sv_get_number_of_modules(sunvoxSlot) <= 0)
            {
                Debug.Log("invalid or empty SunVox slot ID, returning");
                return;
            }

            string foundPatternIDs = string.Empty;
            string patternNameCache = string.Empty;
            int numberOfLoopPatterns = GetNumberOfPatterns(sunvoxSlot);
            for (int i = 0; i < numberOfLoopPatterns; i++)
            {
                patternNameCache = Marshal.PtrToStringAnsi(SunVoxLib.sv_get_pattern_name(sunvoxSlot, i));
                if (patternNameCache != null && patternNameCache.Contains(loopPatternLabel))
                {
                    foundPatternIDs = foundPatternIDs.Insert(foundPatternIDs.Length, i + ", ");
                    loopPatterns.Add(patternNameCache, i);
                }
            }
            Debug.Log($"Found {loopPatterns.Count} loop patterns with ID's: {foundPatternIDs}");
        }
    }
}
