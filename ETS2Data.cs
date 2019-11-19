using System;

namespace SimFeedback.telemetry
{
    public class ETS2Data
    {
        private static float Rad2Deg(float v) { return (float)(v * 180 / Math.PI); }

        public uint Timestamp { get; set; }

        public float Heave { get; set; }
        public float Surge { get; set; }
        public float Sway { get; set; }

        public float CurrentEngineRpm { get; set; }

        private float _pitch;
        public float Pitch { get { return Rad2Deg(_pitch); } set { _pitch = value; } }

        private float _roll;
        public float Roll { get { return Rad2Deg(_roll); } set { _roll = value; } }
    }
}