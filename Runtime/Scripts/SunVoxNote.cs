namespace SunVoxIntegration
{
    public struct SunVoxNote
    {
        public byte note; /* NN: 0 - nothing; 1..127 - note num; 128 - note off; 129, 130... - see NOTECMD_* defines */
        public byte vel; /* VV: Velocity 1..129; 0 - default */
        public ushort module; /* MM: 0 - nothing; 1..65535 - module number + 1 */
        public ushort ctl; /* 0xCCEE: CC: 1..127 - controller number + 1; EE - effect */
        public ushort ctl_val; /* 0xXXYY: controller value or effect parameter */
    }
}
