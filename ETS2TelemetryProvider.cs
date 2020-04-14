using System;
using System.Threading;
using SCSSdkClient;
using SCSSdkClient.Object;
using SimFeedback.log;

namespace SimFeedback.telemetry
{
    public class ETS2TelemetryProvider : AbstractTelemetryProvider
    {
        private SCSSdkTelemetry _scsSdkTelemetry;
        private bool isStopped = true;
        private SCSTelemetry _lastScsTelemetry;
        private ETS2Data _lastTelemetryData;

        public ETS2TelemetryProvider()
        {
            Author = "ashupp / ashnet GmbH";
            Version = "0.0.2.0";
            BannerImage = @"img\banner_ets2.jpg";
            IconImage = @"img\icon_ets2.png";
            TelemetryUpdateFrequency = 30;
        }

        private static float Rad2Deg(float v) { return (float)(v * 180 / Math.PI); }

        public override string Name => "ets2";

        public override void Init(ILogger logger)
        {
            base.Init(logger);
            Log("Initializing ETS2TelemetryProvider");
        }


        public override string[] GetValueList()
        {
            return GetValueListByReflection(typeof(ETS2Data));
        }

        public override void Stop()
        {
            if (isStopped) return;
            LogDebug("Stopping ETS2TelemetryProvider");
            _scsSdkTelemetry.Data -= _scsSdkTelemetry_Data;
            _scsSdkTelemetry.Dispose();
            IsConnected = false;
            isStopped = true;
        }

        public override void Start()
        {
            if (!isStopped) return;
            isStopped = false;
            LogDebug("Starting ETS2TelemetryProvider");
            _scsSdkTelemetry = new SCSSdkTelemetry();
            _scsSdkTelemetry.Data += _scsSdkTelemetry_Data;
        }


        public static double ConvertRange(
            double originalMin, double originalMax,
            double newMin, double newMax,
            double value)
        {
            var scale = (newMax - newMin) / (originalMax - originalMin);
            return (newMin + ((value - originalMin) * scale));
        }

        private void _scsSdkTelemetry_Data(SCSTelemetry scsTelemetryData, bool newTimestamp)
        {
            try
            {
                if ((_lastScsTelemetry == null || _lastTelemetryData== null) || (_lastScsTelemetry.Timestamp != scsTelemetryData.Timestamp))
                {
                    IsConnected = true;
                    IsRunning = true;


                    // Units:

                    /* Vector types:
                     *
                     * In local space the X points to right, Y up and Z backwards.
                     * In world space the X points to east, Y up and Z south.
                     */

                    /* Heading
                     *
                     * Stored in unit range where <0,1) corresponds to <0,360).
                     *
                     * The angle is measured counterclockwise in horizontal plane when looking
                     * from top where 0 corresponds to forward (north), 0.25 to left (west),
                     * 0.5 to backward (south) and 0.75 to right (east).
                     */

                    /* Pitch
                     *
                     * Stored in unit range where <-0.25,0.25> corresponds to <-90,90>.
                     *
                     * The pitch angle is zero when in horizontal direction,
                     * with positive values pointing up (0.25 directly to zenith),
                     * and negative values pointing down (-0.25 directly to nadir).
                     */

                    /* Roll
                     *
                     * Stored in unit range where <-0.5,0.5> corresponds to <-180,180>.
                     *
                     * The angle is measured in counterclockwise when looking in direction of
                     * the roll axis.
                     */


                    // Linear velocity: meters per second.
                    // Linear acceleration: in meters per second^2

                    // Angular velocity: rotations per second.
                    // Angular acceleration: in rotations per second^2

                    var tmpETS2TelemetryData = new ETS2Data();

                    tmpETS2TelemetryData.Timestamp = scsTelemetryData.Timestamp;

                    tmpETS2TelemetryData.Heave = scsTelemetryData.TruckValues.CurrentValues.AccelerationValues.LinearAcceleration.Y;
                    tmpETS2TelemetryData.HeaveVelocity = scsTelemetryData.TruckValues.CurrentValues.AccelerationValues.AngularVelocity.Z;
                    tmpETS2TelemetryData.Surge = scsTelemetryData.TruckValues.CurrentValues.AccelerationValues.LinearAcceleration.Z;
                    tmpETS2TelemetryData.Sway = scsTelemetryData.TruckValues.CurrentValues.AccelerationValues.LinearAcceleration.X;
                    tmpETS2TelemetryData.Pitch = ConvertRange(-0.25, 0.25, -90, 90, scsTelemetryData.TruckValues.CurrentValues.PositionValue.Orientation.Pitch);
                    tmpETS2TelemetryData.Roll = ConvertRange(-0.5, 0.5, -180, 180, scsTelemetryData.TruckValues.CurrentValues.PositionValue.Orientation.Roll);
                    tmpETS2TelemetryData.CurrentEngineRpm = scsTelemetryData.TruckValues.CurrentValues.DashboardValues.RPM;


                    var args = new TelemetryEventArgs(new ETS2TelemetryInfo(tmpETS2TelemetryData, _lastTelemetryData));
                    RaiseEvent(OnTelemetryUpdate, args);
                    _lastTelemetryData = tmpETS2TelemetryData;
                    _lastScsTelemetry = scsTelemetryData;
                }
                else
                {
                    IsRunning = false;
                }
            }
            catch (Exception e)
            {
                LogError("ETS2TelemetryProvider Exception while processing data", e);
                IsConnected = false;
                IsRunning = false;
                Thread.Sleep(1000);
                Stop();
            }
        }
    }
}