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
            Version = "v0.1";
            BannerImage = @"img\banner_ets2.jpg";
            IconImage = @"img\icon_ets2.png";
            TelemetryUpdateFrequency = 30;
        }

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
            LogDebug("Starting ETS2TelemetryProvider");
            _scsSdkTelemetry = new SCSSdkTelemetry();
        }

        private void _scsSdkTelemetry_Data(SCSTelemetry scsTelemetryData, bool newTimestamp)
        {
            try
            {
                if (_lastScsTelemetry == null || _lastTelemetryData== null || (_lastScsTelemetry.Timestamp != scsTelemetryData.Timestamp))
                {
                    IsConnected = true;
                    IsRunning = true;

                    var tmpETS2TelemetryData = new ETS2Data();
                    tmpETS2TelemetryData.Timestamp = scsTelemetryData.Timestamp;

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