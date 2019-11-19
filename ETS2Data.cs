using System;

namespace SimFeedback.telemetry
{
    public class ETS2Data
    {
        private static float G = 9.81f;
        private static float Rad2Deg(float v) { return (float)(v * 180 / Math.PI); }
        private float _pitch;
        public float Pitch { get { return Rad2Deg(_pitch); } set { _pitch = value; } }
        private float _roll;
        public float Roll { get { return Rad2Deg(_roll); } set { _roll = value; } }
        public bool Paused { get; set; }
        public uint Timestamp { get; set; }
        public float EngineMaxRpm { get; set; }
        public float EngineRpm { get; set; }
        public float EngineRpmMax { get; set; }
        public float Speed { get; set; }
        public float SpeedKmH { get; set; }
        public float SpeedMpH { get; set; }
        public float Gear { get; set; }
    }
}