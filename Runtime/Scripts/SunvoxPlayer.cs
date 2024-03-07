using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SunVoxIntegration
{
    [RequireComponent(typeof(AudioSource))]
    public class SunVoxPlayer : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        SunVoxLib.InitFlags initSettings;
        [SerializeField]
        [HideInInspector]
        private SunVoxProject sunvoxProject;
        [SerializeField]
        [HideInInspector]
        private int sunvoxSlot;
        [SerializeField]
        [HideInInspector]
        private bool beginPlaybackOnStart, beginPlaybackFromFirstLine;
        [SerializeField]
        [HideInInspector]
        bool enableDebugOutput;

        //pattern looping settings
        [SerializeField]
        [HideInInspector]
        private bool loopMode;
        [SerializeField]
        [HideInInspector]
        private string loopPatternLabel;
        [SerializeField]
        [HideInInspector]
        private int currentLoopPatternID, queuedLoopPatternID;

        //TODO make dictionaries serializable
        private Dictionary<string, int> loopPatterns;
        private int currentSunVoxVersion, lastLine, bpm;
        private bool outputToUnity;

        public int CurrentLoopPatternID { get { return currentLoopPatternID; } }
        public int QueuedLoopPatternID { get => queuedLoopPatternID; set => queuedLoopPatternID = value; }
        public bool LoopMode { get => loopMode; set => loopMode = value; }
        public bool BeginPlaybackOnStart { get => beginPlaybackOnStart; set => beginPlaybackOnStart = value; }
        public bool BeginPlaybackFromFirstLine { get => beginPlaybackFromFirstLine; set => beginPlaybackFromFirstLine = value; }
        public int SunVoxSlot { get { return sunvoxSlot; } }
        public int Bpm
        {
            get
            {
                bpm = SunVoxLib.sv_get_song_bpm(sunvoxSlot);
                return bpm; 
            }
            set
            {
                if (value != bpm && value > 0)
                {
                    bpm = value;
                    SunVoxUtility.SetBpm(sunvoxSlot, bpm);
                }
            }
        }

#if UNITY_EDITOR
        PauseState editorPaused;
#endif

        void Start()
        {
            loopPatterns = new Dictionary<string, int>();
            outputToUnity = initSettings.HasFlag(SunVoxLib.InitFlags.SV_INIT_FLAG_USER_AUDIO_CALLBACK);

            //When routing audio into unity, enforce output as float32 to prevent blowing up speakers
            if (outputToUnity)
                initSettings |= SunVoxLib.InitFlags.SV_INIT_FLAG_AUDIO_FLOAT32;

            try
            {
                currentSunVoxVersion = SunVoxLib.sv_init("", AudioSettings.outputSampleRate, (int)AudioSettings.speakerMode, (uint)initSettings);

                PrintDebugInfo($"Initialization done, running SunVox version {SunVoxUtility.VersionAsString(currentSunVoxVersion)}, unity integration last tested with {SunVoxUtility.integrationTestedWithSunVoxVersion} \n" +
                    $"unity samplerate is {AudioSettings.outputSampleRate}, SunVox samplerate is {SunVoxLib.sv_get_sample_rate()}" +
                    $", number of audio channels is {AudioSettings.speakerMode}, init flags are: {initSettings}");

                PrintDebugInfo($"Opening SunVox slot {sunvoxSlot}");
                if (sunvoxSlot < 0 || sunvoxSlot > 15)
                {
                    PrintDebugInfo("Tried opening a SunVox slot outside the range 0-15, aborting setup");
                    return;
                }
                else
                    SunVoxLib.sv_open_slot(sunvoxSlot);

                LoadProject(sunvoxProject);

#if UNITY_EDITOR
                EditorApplication.pauseStateChanged += EditorPausePlayer;
                editorPaused = PauseState.Unpaused;
#endif
            }
            catch (System.Exception)
            {
                PrintDebugInfo("SunVox initialization failed");
                throw;
            }
        }

        private void Update()
        {
            if (LoopMode)
            {
                CheckPatternQueueJump();
            }
        }

#if UNITY_EDITOR
        void EditorPausePlayer(PauseState state) => editorPaused = state;
#endif

        private void OnAudioFilterRead(float[] data, int channels)
        {
#if UNITY_EDITOR
            if (editorPaused == PauseState.Paused)
                return;
#endif

            if (outputToUnity)
                SunVoxLib.sv_audio_callback(data, data.Length / channels, 0, SunVoxLib.sv_get_ticks());
        }

        private void OnDestroy()
        {
            if (!enabled) return;

#if UNITY_EDITOR
            EditorApplication.pauseStateChanged -= EditorPausePlayer;
#endif

            SunVoxLib.sv_close_slot(sunvoxSlot);
            SunVoxLib.sv_deinit();
        }

        public void LoadProject(SunVoxProject newProject)
        {
            if (newProject != null)
            {
                sunvoxProject = newProject;
                PrintDebugInfo($"Loading SunVox project: {sunvoxProject}");
                SunVoxLib.sv_load_from_memory(sunvoxSlot, sunvoxProject.projectContents, (uint)sunvoxProject.projectContents.Length);
            }
            else
            {
                PrintDebugInfo("SunVox project is null, aborting setup");
                return;
            }
            PrintDebugInfo(SunVoxUtility.SongInfo(sunvoxSlot));

            if (loopPatternLabel != string.Empty)
                SunVoxUtility.FindLoopPatterns(sunvoxSlot, loopPatternLabel, loopPatterns);
            else
                PrintDebugInfo("Loop pattern label is empty, skipping setup");

            if (BeginPlaybackOnStart)
            {
                if (BeginPlaybackFromFirstLine)
                    SunVoxLib.sv_play_from_beginning(sunvoxSlot);
                else
                    SunVoxLib.sv_play(sunvoxSlot);
            }
            Bpm = SunVoxLib.sv_get_song_bpm(SunVoxSlot);

        }

        public void ToggleStartPlayer()
        {
            if (SunVoxLib.sv_end_of_song(sunvoxSlot) == 1)
                SunVoxLib.sv_play(sunvoxSlot);
            else
                SunVoxLib.sv_stop(sunvoxSlot);
        }

        public void StopPlayer()
        {
            SunVoxLib.sv_stop(sunvoxSlot);
            SunVoxLib.sv_rewind(sunvoxSlot, 0);
        }

        void CheckPatternQueueJump()
        {
            if (lastLine != SunVoxLib.sv_get_current_line(sunvoxSlot))//this is checked in case the update loop runs fast enough that a new line has not yet been moved. Note that when output to unity is not used, looping might not be as seamless
            {
                if (SunVoxLib.sv_get_current_line(sunvoxSlot) == SunVoxLib.sv_get_pattern_x(sunvoxSlot, currentLoopPatternID) + SunVoxLib.sv_get_pattern_lines(sunvoxSlot, currentLoopPatternID) - 1)
                {
                    SunVoxUtility.JumpImmediateSync(sunvoxSlot, SunVoxLib.sv_get_pattern_x(sunvoxSlot, QueuedLoopPatternID));
                    currentLoopPatternID = QueuedLoopPatternID;
                }
                lastLine = SunVoxLib.sv_get_current_line(sunvoxSlot);
            }
        }

        public string GetCurrentTimeStamp(int sunvoxSlot)
        {
            if (currentSunVoxVersion <= 0) //check to prevent crash when sunvox is not yet initialized
                return "";

            uint frame;
            return string.Format("{0}:{1} / {2}:{3}"
                , Mathf.Abs((frame = GetCurrentFrame(sunvoxSlot)) / AudioSettings.outputSampleRate / 60)
                , ((frame / SunVoxLib.sv_get_sample_rate()) % 60).ToString("D2")
                , Mathf.Abs(SunVoxLib.sv_get_song_length_frames(sunvoxSlot) / AudioSettings.outputSampleRate / 60)
                , ((SunVoxLib.sv_get_song_length_frames(sunvoxSlot) / SunVoxLib.sv_get_sample_rate()) % 60).ToString("D2"));
        }

        uint GetCurrentFrame(int sunvoxSlot)
        {
            uint[] data = new uint[SunVoxLib.sv_get_current_line(sunvoxSlot) + 1]; // we start at line 0 so we add 1 to at least have a single element in the array
            SunVoxLib.sv_get_time_map(sunvoxSlot, 0, data.Length, data, 1);
            return data[data.Length - 1];
        }

        void PrintDebugInfo(string message)
        {
#if UNITY_EDITOR
            if (enableDebugOutput)
                Debug.Log(message);
#endif
        }
    }
}
