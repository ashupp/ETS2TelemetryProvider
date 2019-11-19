namespace SimFeedback.telemetry
{
    public class ETS2TelemetryValue : AbstractTelemetryValue
    {
        public ETS2TelemetryValue(string name, object value) : base()
        {
            Name = name;
            Value = value;
        }

        public override object Value { get; set; }
    }
}