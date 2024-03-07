using UnityEditor;
using UnityEngine;

namespace SunVoxIntegration
{
    [CustomEditor(typeof(SunVoxPlayer))]
    internal class SunVoxPlayerEditor : Editor
    {
        bool mouseStateToggle;
        int playheadPosition = 0;
        const float twoThirds = 2f / 3;
        SunVoxPlayer player;
        SerializedProperty beginPlaybackOnStart, beginPlaybackFromFirstLine, sunvoxProject, initSettings
            , sunvoxSlot, enableDebugOutput, loopMode, loopPatternLabel, currentLoopPatternID, queuedLoopPatternID;

        private void OnEnable()
        {
            player = (SunVoxPlayer)serializedObject.targetObject;
            sunvoxProject = serializedObject.FindProperty("sunvoxProject");
            initSettings = serializedObject.FindProperty("initSettings");
            sunvoxSlot = serializedObject.FindProperty("sunvoxSlot");
            enableDebugOutput = serializedObject.FindProperty("enableDebugOutput");
            beginPlaybackOnStart = serializedObject.FindProperty("beginPlaybackOnStart");
            beginPlaybackFromFirstLine = serializedObject.FindProperty("beginPlaybackFromFirstLine");
            loopMode = serializedObject.FindProperty("loopMode");
            loopPatternLabel = serializedObject.FindProperty("loopPatternLabel");
            currentLoopPatternID = serializedObject.FindProperty("currentLoopPatternID");
            queuedLoopPatternID = serializedObject.FindProperty("queuedLoopPatternID");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector(); //only calling this to draw the VU meter
            serializedObject.Update();
            //DrawPropertiesExcluding(serializedObject, new string[] { "m_Script" }); //this is an undocumented method. using this will hide the script property, but won't draw the VU meter during playmode, which i do want.
            //in v2022.3.11f the VU meter drawing behavior is broken, take a look at this again when it's fixed. until then use DrawDefaultInspector();
            //another curiosity about this method is that it can be called before serializedObject.ApplyModifiedProperties(); and wont lock up the controls like DrawDefaultInspector(); does.

            //Playback controls
            EditorGUILayout.LabelField("Playback controls", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUI.BeginDisabledGroup(!Application.isPlaying); // playback controls disabled during edit mode
            if (GUILayout.Button(new GUIContent("Start/Pause", "Playback controls are enabled during playmode"), GUILayout.Width(100)))
            {
                player.ToggleStartPlayer();
            }
            if (GUILayout.Button(new GUIContent("Stop", "Playback controls are enabled during playmode"), GUILayout.Width(100)))
            {
                player.StopPlayer();
            }
            EditorGUIUtility.labelWidth = 30; //BPM control uses the player property accessor directly instead of a SerializedProperty because we want to prevent setting a negative value
            player.Bpm = EditorGUILayout.IntField(new GUIContent("BPM", "Playback controls are enabled during playmode"), player.Bpm, GUILayout.Width(80), GUILayout.Height(18));
            EditorGUIUtility.labelWidth = 0;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (Event.current.type == EventType.MouseDown)
                mouseStateToggle = true;
            else if (Event.current.type == EventType.MouseUp)
                mouseStateToggle = false;

            //Playback progress bar
            Rect progressBar = EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (mouseStateToggle)
            {
                if (progressBar.Contains(Event.current.mousePosition))
                    playheadPosition = (int)GUILayout.HorizontalSlider(playheadPosition, 0, SunVoxLib.sv_get_song_length_lines(player.SunVoxSlot), GUILayout.Width(EditorGUIUtility.currentViewWidth * twoThirds));
                else
                    GUILayout.HorizontalSlider(SunVoxLib.sv_get_current_line(player.SunVoxSlot), 0, SunVoxLib.sv_get_song_length_lines(player.SunVoxSlot), GUILayout.Width(EditorGUIUtility.currentViewWidth * twoThirds));
            }
            if (Event.current.type == EventType.MouseUp && progressBar.Contains(Event.current.mousePosition))
                SunVoxLib.sv_rewind(player.SunVoxSlot, playheadPosition);//BUG when playback is paused, the slider will flicker when releasing mouse button, as far as i can tell no impact on playback            
            if (!mouseStateToggle)
                GUILayout.HorizontalSlider(SunVoxLib.sv_get_current_line(player.SunVoxSlot), 0, SunVoxLib.sv_get_song_length_lines(player.SunVoxSlot), GUILayout.Width(EditorGUIUtility.currentViewWidth * twoThirds));
            if (Application.isPlaying)
                EditorGUILayout.LabelField(player.GetCurrentTimeStamp(player.SunVoxSlot), GUILayout.MaxWidth(80));
            else
                EditorGUILayout.LabelField("0:00 / 0:00", GUILayout.MaxWidth(80));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();// playback controls disabled during edit mode

            //Project loading options
            EditorGUILayout.LabelField("Project loading", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(sunvoxProject);
            EditorGUILayout.PropertyField(initSettings);
            EditorGUI.BeginDisabledGroup(Application.isPlaying);//disabling this control during playmode to prevent deinitializing SunVox without closing all slots, which causes a crash
            EditorGUILayout.PropertyField(sunvoxSlot, GUILayout.Width(160), GUILayout.MinWidth(EditorGUIUtility.currentViewWidth / 2));
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(beginPlaybackOnStart);
            EditorGUI.BeginDisabledGroup(!beginPlaybackOnStart.boolValue);
            EditorGUIUtility.labelWidth = 185;
            EditorGUILayout.PropertyField(beginPlaybackFromFirstLine);
            EditorGUIUtility.labelWidth = 0;
            GUILayout.FlexibleSpace();
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(enableDebugOutput);

            //Pattern looping settings
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Pattern looping settings", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(loopMode);
            EditorGUIUtility.labelWidth = 120;
            EditorGUILayout.PropertyField(loopPatternLabel, GUILayout.Width(200));
            EditorGUIUtility.labelWidth = 0;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(currentLoopPatternID, GUILayout.Width(160), GUILayout.MinWidth(EditorGUIUtility.currentViewWidth / 2));
            EditorGUILayout.PropertyField(queuedLoopPatternID, GUILayout.Width(160), GUILayout.MinWidth(EditorGUIUtility.currentViewWidth / 2));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
