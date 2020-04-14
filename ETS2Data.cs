using System;

namespace SimFeedback.telemetry
{
    public class ETS2Data
    {

        public uint Timestamp { get; set; }

        public double Heave { get; set; }
        public double HeaveVelocity { get; set; }
        public double Surge { get; set; }
        public double Sway { get; set; }
        public float CurrentEngineRpm { get; set; }
        public double Pitch { get; set; }
        public double Roll { get; set; }
    }
}