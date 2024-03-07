using System;
using System.Runtime.InteropServices;

namespace SunVoxIntegration
{
    public class SunVoxLib
    {

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID
        private const string libraryName = "sunvox";
#elif UNITY_IOS
        private const string libraryName = "__Internal";
#endif

        #region Constants

        //Note commands
        public const int NOTECMD_NOTE_OFF = 128;
        public const int NOTECMD_ALL_NOTES_OFF = 129; /* notes of all synths off */
        public const int NOTECMD_CLEAN_SYNTHS = 130; /* stop and clean all synths */
        public const int NOTECMD_STOP = 131;
        public const int NOTECMD_PLAY = 132;
        public const int NOTECMD_SET_PITCH = 133; /* set the pitch specified in column XXYY, where 0x0000 - highest possible pitch, 0x7800 - lowest pitch (note C0); one semitone = 0x100 */
        public const int NOTECMD_CLEAN_MODULE = 140;

        [Flags]
        public enum InitFlags //Flags for sv_init()
        {
            SV_INIT_FLAG_NO_DEBUG_OUTPUT = 1,
            SV_INIT_FLAG_USER_AUDIO_CALLBACK = (1 << 1), /*handle audio buffer data manually instead of using built-in audio output. use together with the INT16 or FLOAT32 flag. call sv_audio_callback() to get audio stream data*/
            //SV_INIT_FLAG_AUDIO_INT16 = (1 << 2), /*output of audio stream is int16 format. use together with the SV_INIT_FLAG_USER_AUDIO_CALLBACK flag. this is commented out because you will rarely need audio in this format in unity. uncomment if you know what you are doing*/
            SV_INIT_FLAG_AUDIO_FLOAT32 = (1 << 3), /*output of audio stream is float32 format. use together with the SV_INIT_FLAG_USER_AUDIO_CALLBACK flag*/
            SV_INIT_FLAG_ONE_THREAD = (1 << 4) /* Audio callback and song modification functions are in single thread */
        };

        //Flags for sv_get_time_map()
        public const int SV_TIME_MAP_SPEED = 0;
        public const int SV_TIME_MAP_FRAMECNT = 1;

        //Flags for sv_get_module_flags()
        public const int SV_MODULE_FLAG_EXISTS = 1;
        public const int SV_MODULE_FLAG_GENERATOR = (1 << 1);
        public const int SV_MODULE_FLAG_EFFECT = (1 << 2);
        public const int SV_MODULE_FLAG_MUTE = (1 << 3);
        public const int SV_MODULE_FLAG_SOLO = (1 << 4);
        public const int SV_MODULE_FLAG_BYPASS = (1 << 5);
        public const int SV_MODULE_INPUTS_OFF = 16;
        public const int SV_MODULE_INPUTS_MASK = (255 << (byte)SV_MODULE_INPUTS_OFF);
        public const int SV_MODULE_OUTPUTS_OFF = (16 + 8);
        public const int SV_MODULE_OUTPUTS_MASK = (255 << (byte)SV_MODULE_OUTPUTS_OFF);

        #endregion

        #region Main Functions
        //these functions are mentioned in the API reference, but they are not in the dll, they are in the .h file which we don't use
        //sv_load_dll()
        //sv_unload_dll()

        ///<include file = 'SunVoxLib.xml' path='Docs/MainFunctions/Method[@name="init"]/*'/>
        [DllImport(libraryName)] public static extern int sv_init(string config, int sample_rate, int channels, uint flags);

        ///<include file = 'SunVoxLib.xml' path='Docs/MainFunctions/Method[@name="deinit"]/*'/>
        [DllImport(libraryName)] public static extern int sv_deinit();

        ///<include file = 'SunVoxLib.xml' path='Docs/MainFunctions/Method[@name="get_sample_rate"]/*'/>
        [DllImport(libraryName)] public static extern int sv_get_sample_rate();

        ///<include file = 'SunVoxLib.xml' path='Docs/MainFunctions/Method[@name="update_input"]/*'/>
        [DllImport(libraryName)] public static extern int sv_update_input();

        ///<include file = 'SunVoxLib.xml' path='Docs/MainFunctions/Method[@name="audio_callbackFLOAT"]/*'/>
        [DllImport(libraryName)] public static extern int sv_audio_callback(float[] buf, int frames, int latency, int out_time);

        ///<include file = 'SunVoxLib.xml' path='Docs/MainFunctions/Method[@name="audio_callbackSHORT"]/*'/>
        [DllImport(libraryName)] public static extern int sv_audio_callback(short[] buf, int frames, int latency, int out_time);

        ///<include file = 'SunVoxLib.xml' path='Docs/MainFunctions/Method[@name="audio_callback2"]/*'/>
        [DllImport(libraryName)] public static extern int sv_audio_callback2(float[] buf, int frames, int latency, int out_time, int in_type, int in_channels, float[] in_buf);

        ///<include file = 'SunVoxLib.xml' path='Docs/MainFunctions/Method[@name="open_slot"]/*'/>
        [DllImport(libraryName)] public static extern int sv_open_slot(int slot);

        ///<include file = 'SunVoxLib.xml' path='Docs/MainFunctions/Method[@name="close_slot"]/*'/>
        [DllImport(libraryName)] public static extern int sv_close_slot(int slot);

        ///<include file = 'SunVoxLib.xml' path='Docs/MainFunctions/Method[@name="lock_slot"]/*'/>
        [DllImport(libraryName)] public static extern int sv_lock_slot(int slot);

        ///<include file = 'SunVoxLib.xml' path='Docs/MainFunctions/Method[@name="unlock_slot"]/*'/>
        [DllImport(libraryName)] public static extern int sv_unlock_slot(int slot);

        #endregion

        #region Project file

        ///<include file = 'SunVoxLib.xml' path='Docs/ProjectFile/Method[@name="load"]/*'/>        
        [DllImport(libraryName)] public static extern int sv_load(int slot, string name);

        ///<include file = 'SunVoxLib.xml' path='Docs/ProjectFile/Method[@name="load_from_memory"]/*'/>        
        [DllImport(libraryName)] public static extern int sv_load_from_memory(int slot, byte[] data, uint data_size);

        ///<include file = 'SunVoxLib.xml' path='Docs/ProjectFile/Method[@name="save"]/*'/>        
        [DllImport(libraryName)] public static extern int sv_save(int slot, string name);

        #endregion

        #region Project playback

        ///<include file = 'SunVoxLib.xml' path='Docs/ProjectPlayback/Method[@name="play"]/*'/>
        [DllImport(libraryName)] public static extern int sv_play(int slot);

        ///<include file = 'SunVoxLib.xml' path='Docs/ProjectPlayback/Method[@name="play_from_beginning"]/*'/>
        [DllImport(libraryName)] public static extern int sv_play_from_beginning(int slot);

        ///<include file = 'SunVoxLib.xml' path='Docs/ProjectPlayback/Method[@name="stop"]/*'/>
        [DllImport(libraryName)] public static extern int sv_stop(int slot);

        ///<include file = 'SunVoxLib.xml' path='Docs/ProjectPlayback/Method[@name="pause"]/*'/>
        [DllImport(libraryName)] public static extern int sv_pause(int slot);

        ///<include file = 'SunVoxLib.xml' path='Docs/ProjectPlayback/Method[@name="resume"]/*'/>
        [DllImport(libraryName)] public static extern int sv_resume(int slot);

        ///<include file = 'SunVoxLib.xml' path='Docs/ProjectPlayback/Method[@name="sync_resume"]/*'/>
        [DllImport(libraryName)] public static extern int sv_sync_resume(int slot);

        ///<include file = 'SunVoxLib.xml' path='Docs/ProjectPlayback/Method[@name="set_autostop"]/*'/>
        [DllImport(libraryName)] public static extern int sv_set_autostop(int slot, int autostop);

        ///<include file = 'SunVoxLib.xml' path='Docs/ProjectPlayback/Method[@name="get_autostop"]/*'/>
        [DllImport(libraryName)] public static extern int sv_get_autostop(int slot);

        ///<include file = 'SunVoxLib.xml' path='Docs/ProjectPlayback/Method[@name="end_of_song"]/*'/>
        [DllImport(libraryName)] public static extern int sv_end_of_song(int slot);

        ///<include file = 'SunVoxLib.xml' path='Docs/ProjectPlayback/Method[@name="rewind"]/*'/>
        [DllImport(libraryName)] public static extern int sv_rewind(int slot, int line_num);

        ///<include file = 'SunVoxLib.xml' path='Docs/ProjectPlayback/Method[@name="volume"]/*'/>
        [DllImport(libraryName)] public static extern int sv_volume(int slot, int vol);

        ///<include file = 'SunVoxLib.xml' path='Docs/ProjectPlayback/Method[@name="get_current_line"]/*'/>
        [DllImport(libraryName)] public static extern int sv_get_current_line(int slot);

        ///<include file = 'SunVoxLib.xml' path='Docs/ProjectPlayback/Method[@name="get_current_line2"]/*'/>
        [DllImport(libraryName)] public static extern int sv_get_current_line2(int slot);

        ///<include file = 'SunVoxLib.xml' path='Docs/ProjectPlayback/Method[@name="get_current_signal_level"]/*'/>
        [DllImport(libraryName)] public static extern int sv_get_current_signal_level(int slot, int channel);

        #endregion

        #region Project info

        ///<include file = 'SunVoxLib.xml' path='Docs/ProjectInfo/Method[@name="get_song_name"]/*'/>
        [DllImport(libraryName)] public static extern IntPtr sv_get_song_name(int slot);

        ///<include file = 'SunVoxLib.xml' path='Docs/ProjectInfo/Method[@name="set_song_name"]/*'/>
        [DllImport(libraryName)] public static extern int sv_set_song_name(int slot, string name);

        ///<include file = 'SunVoxLib.xml' path='Docs/ProjectInfo/Method[@name="get_song_bpm"]/*'/>
        [DllImport(libraryName)] public static extern int sv_get_song_bpm(int slot);

        ///<include file = 'SunVoxLib.xml' path='Docs/ProjectInfo/Method[@name="get_song_tpl"]/*'/>
        [DllImport(libraryName)] public static extern int sv_get_song_tpl(int slot);

        ///<include file = 'SunVoxLib.xml' path='Docs/ProjectInfo/Method[@name="get_song_length_frames"]/*'/>
        [DllImport(libraryName)] public static extern uint sv_get_song_length_frames(int slot);

        ///<include file = 'SunVoxLib.xml' path='Docs/ProjectInfo/Method[@name="get_song_length_lines"]/*'/>
        [DllImport(libraryName)] public static extern uint sv_get_song_length_lines(int slot);

        ///<include file = 'SunVoxLib.xml' path='Docs/ProjectInfo/Method[@name="get_time_map"]/*'/>
        [DllImport(libraryName)] public static extern int sv_get_time_map(int slot, int start_line, int len, uint[] dest, int flags);

        #endregion

        #region Events

        ///<include file = 'SunVoxLib.xml' path='Docs/Events/Method[@name="set_event_t"]/*'/>
        [DllImport(libraryName)] public static extern int sv_set_event_t(int slot, int set, int t);

        ///<include file = 'SunVoxLib.xml' path='Docs/Events/Method[@name="send_event"]/*'/>
        [DllImport(libraryName)] public static extern int sv_send_event(int slot, int track_num, int note, int vel, int module, int ctl, int ctl_val);

        #endregion

        #region Modules

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="new_module"]/*'/>
        [DllImport(libraryName)] public static extern int sv_new_module(int slot, string type, string name, int x, int y, int z);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="remove_module"]/*'/>
        [DllImport(libraryName)] public static extern int sv_remove_module(int slot, int mod_num);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="connect_module"]/*'/>
        [DllImport(libraryName)] public static extern int sv_connect_module(int slot, int source, int destination);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="disconnect_module"]/*'/>
        [DllImport(libraryName)] public static extern int sv_disconnect_module(int slot, int source, int destination);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="load_module"]/*'/>
        [DllImport(libraryName)] public static extern int sv_load_module(int slot, string fileName, int x, int y, int z);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="load_module_from_memory"]/*'/>
        [DllImport(libraryName)] public static extern int sv_load_module_from_memory(int slot, byte[] data, int data_size, int x, int y, int z);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="sampler_load"]/*'/>
        [DllImport(libraryName)] public static extern int sv_sampler_load(int slot, int mod_num, string fileName, int sample_slot);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="sampler_load_from_memory"]/*'/>
        [DllImport(libraryName)] public static extern int sv_sampler_load_from_memory(int slot, int mod_num, byte[] data, int data_size, int sample_slot);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="metamodule_load"]/*'/>
        [DllImport(libraryName)] public static extern int sv_metamodule_load(int slot, int mod_num, string fileName);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="metamodule_load_from_memory"]/*'/>
        [DllImport(libraryName)] public static extern int sv_metamodule_load_from_memory(int slot, int mod_num, byte[] data, uint data_size);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="vplayer_load"]/*'/>
        [DllImport(libraryName)] public static extern int sv_vplayer_load(int slot, int mod_num, string fileName);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="vplayer_load_from_memory"]/*'/>
        [DllImport(libraryName)] public static extern int sv_vplayer_load_from_memory(int slot, int mod_num, byte[] data, uint data_size);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="get_number_of_modules"]/*'/>
        [DllImport(libraryName)] public static extern int sv_get_number_of_modules(int slot);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="find_module"]/*'/>
        [DllImport(libraryName)] public static extern int sv_find_module(int slot, string name);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="get_module_flags"]/*'/>
        [DllImport(libraryName)] public static extern uint sv_get_module_flags(int slot, int mod_num);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="get_module_inputs"]/*'/>
        [DllImport(libraryName)] public static extern IntPtr sv_get_module_inputs(int slot, int mod_num);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="get_module_outputs"]/*'/>
        [DllImport(libraryName)] public static extern IntPtr sv_get_module_outputs(int slot, int mod_num);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="get_module_type"]/*'/>
        [DllImport(libraryName)] public static extern IntPtr sv_get_module_type(int slot, int mod_num);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="get_module_name"]/*'/>
        [DllImport(libraryName)] public static extern IntPtr sv_get_module_name(int slot, int mod_num);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="set_module_name"]/*'/>
        [DllImport(libraryName)] public static extern int sv_set_module_name(int slot, int mod_num, string name);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="get_module_xy"]/*'/>
        [DllImport(libraryName)] public static extern uint sv_get_module_xy(int slot, int mod_num);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="set_module_xy"]/*'/>
        [DllImport(libraryName)] public static extern int sv_set_module_xy(int slot, int mod_num, int x, int y);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="get_module_color"]/*'/>
        [DllImport(libraryName)] public static extern int sv_get_module_color(int slot, int mod_num);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="set_module_color"]/*'/>
        [DllImport(libraryName)] public static extern int sv_set_module_color(int slot, int mod_num, int color);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="get_module_finetune"]/*'/>
        [DllImport(libraryName)] public static extern uint sv_get_module_finetune(int slot, int mod_num);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="set_module_finetune"]/*'/>
        [DllImport(libraryName)] public static extern int sv_set_module_finetune(int slot, int mod_num, int finetune);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="set_module_relnote"]/*'/>
        [DllImport(libraryName)] public static extern int sv_set_module_relnote(int slot, int mod_num, int relative_note);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="get_module_scope2"]/*'/>
        [DllImport(libraryName)] public static extern uint sv_get_module_scope2(int slot, int mod_num, int channel, short[] dest_buf, uint samples_to_read);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="module_curve"]/*'/>
        [DllImport(libraryName)] public static extern int sv_module_curve(int slot, int mod_num, int curve_num, float[] data, int len, int w);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="get_number_of_module_ctls"]/*'/>
        [DllImport(libraryName)] public static extern int sv_get_number_of_module_ctls(int slot, int mod_num);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="get_module_ctl_name"]/*'/>
        [DllImport(libraryName)] public static extern IntPtr sv_get_module_ctl_name(int slot, int mod_num, int ctl_num);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="get_module_ctl_value"]/*'/>
        [DllImport(libraryName)] public static extern int sv_get_module_ctl_value(int slot, int mod_num, int ctl_num, int scaled);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="set_module_ctl_value"]/*'/>
        [DllImport(libraryName)] public static extern int sv_set_module_ctl_value(int slot, int mod_num, int ctl_num, int val, int scaled);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="get_module_ctl_min"]/*'/>
        [DllImport(libraryName)] public static extern int sv_get_module_ctl_min(int slot, int mod_num, int ctl_num, int scaled);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="get_module_ctl_max"]/*'/>
        [DllImport(libraryName)] public static extern int sv_get_module_ctl_max(int slot, int mod_num, int ctl_num, int scaled);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="get_module_ctl_offset"]/*'/>
        [DllImport(libraryName)] public static extern int sv_get_module_ctl_offset(int slot, int mod_num, int ctl_num);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="get_module_ctl_type"]/*'/>
        [DllImport(libraryName)] public static extern int sv_get_module_ctl_type(int slot, int mod_num, int ctl_num);

        ///<include file = 'SunVoxLib.xml' path='Docs/Modules/Method[@name="get_module_ctl_group"]/*'/>
        [DllImport(libraryName)] public static extern int sv_get_module_ctl_group(int slot, int mod_num, int ctl_num);

        #endregion

        #region Patterns

        ///<include file = 'SunVoxLib.xml' path='Docs/Patterns/Method[@name="new_pattern"]/*'/>
        [DllImport(libraryName)] public static extern int sv_new_pattern(int slot, int clone, int x, int y, int tracks, int lines, int icon_seed, string name);

        ///<include file = 'SunVoxLib.xml' path='Docs/Patterns/Method[@name="remove_pattern"]/*'/>
        [DllImport(libraryName)] public static extern int sv_remove_pattern(int slot, int pat_num);

        ///<include file = 'SunVoxLib.xml' path='Docs/Patterns/Method[@name="get_number_of_patterns"]/*'/>
        [DllImport(libraryName)] public static extern int sv_get_number_of_patterns(int slot);

        ///<include file = 'SunVoxLib.xml' path='Docs/Patterns/Method[@name="find_pattern"]/*'/>
        [DllImport(libraryName)] public static extern int sv_find_pattern(int slot, string name);

        ///<include file = 'SunVoxLib.xml' path='Docs/Patterns/Method[@name="get_pattern_x"]/*'/>
        [DllImport(libraryName)] public static extern int sv_get_pattern_x(int slot, int pat_num);

        ///<include file = 'SunVoxLib.xml' path='Docs/Patterns/Method[@name="get_pattern_y"]/*'/>
        [DllImport(libraryName)] public static extern int sv_get_pattern_y(int slot, int pat_num);

        ///<include file = 'SunVoxLib.xml' path='Docs/Patterns/Method[@name="set_pattern_xy"]/*'/>
        [DllImport(libraryName)] public static extern int sv_set_pattern_xy(int slot, int pat_num, int x, int y);

        ///<include file = 'SunVoxLib.xml' path='Docs/Patterns/Method[@name="get_pattern_tracks"]/*'/>
        [DllImport(libraryName)] public static extern int sv_get_pattern_tracks(int slot, int pat_num);

        ///<include file = 'SunVoxLib.xml' path='Docs/Patterns/Method[@name="get_pattern_lines"]/*'/>
        [DllImport(libraryName)] public static extern int sv_get_pattern_lines(int slot, int pat_num);

        ///<include file = 'SunVoxLib.xml' path='Docs/Patterns/Method[@name="set_pattern_size"]/*'/>
        [DllImport(libraryName)] public static extern int sv_set_pattern_size(int slot, int pat_num, int tracks, int lines);

        ///<include file = 'SunVoxLib.xml' path='Docs/Patterns/Method[@name="get_pattern_name"]/*'/>
        [DllImport(libraryName)] public static extern IntPtr sv_get_pattern_name(int slot, int pat_num);

        ///<include file = 'SunVoxLib.xml' path='Docs/Patterns/Method[@name="set_pattern_name"]/*'/>
        [DllImport(libraryName)] public static extern int sv_set_pattern_name(int slot, int pat_num, string name);

        ///<include file = 'SunVoxLib.xml' path='Docs/Patterns/Method[@name="get_pattern_data"]/*'/>
        [DllImport(libraryName)] public static extern IntPtr sv_get_pattern_data(int slot, int pat_num);

        ///<include file = 'SunVoxLib.xml' path='Docs/Patterns/Method[@name="set_pattern_event"]/*'/>
        [DllImport(libraryName)] public static extern int sv_set_pattern_event(int slot, int pat, int track, int line, int nn, int vv, int mm, int ccee, int xxyy);

        ///<include file = 'SunVoxLib.xml' path='Docs/Patterns/Method[@name="get_pattern_event"]/*'/>
        [DllImport(libraryName)] public static extern int sv_get_pattern_event(int slot, int pat, int track, int line, int column);

        ///<include file = 'SunVoxLib.xml' path='Docs/Patterns/Method[@name="pattern_mute"]/*'/>
        [DllImport(libraryName)] public static extern int sv_pattern_mute(int slot, int pat_num, int mute);

        #endregion

        #region Other

        ///<include file = 'SunVoxLib.xml' path='Docs/Other/Method[@name="get_ticks"]/*'/>
        [DllImport(libraryName)] public static extern int sv_get_ticks();

        ///<include file = 'SunVoxLib.xml' path='Docs/Other/Method[@name="get_ticks_per_second"]/*'/>
        [DllImport(libraryName)] public static extern int sv_get_ticks_per_second();

        ///<include file = 'SunVoxLib.xml' path='Docs/Other/Method[@name="get_log"]/*'/>
        [DllImport(libraryName)] public static extern IntPtr sv_get_log(int size);

        #endregion
    }
}
