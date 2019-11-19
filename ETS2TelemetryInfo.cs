using System;
using System.Reflection;
using SCSSdkClient.Object;

namespace SimFeedback.telemetry
{
    public class ETS2TelemetryInfo : EventArgs, TelemetryInfo
    {
        private ETS2Data _telemetryData;

        public ETS2TelemetryInfo(ETS2Data telemetryData, ETS2Data lastTelemetryData)
        {
            _telemetryData = telemetryData;
        }

        public TelemetryValue TelemetryValueByName(string name)
        {
            ETS2TelemetryValue tv;
            switch (name)
            {
                default:
                    object data;
                    Type eleDataType = typeof(ETS2Data);
                    PropertyInfo propertyInfo;
                    FieldInfo fieldInfo = eleDataType.GetField(name);
                    if (fieldInfo != null)
                    {
                        data = fieldInfo.GetValue(_telemetryData);
                    }
                    else if ((propertyInfo = eleDataType.GetProperty(name)) != null)
                    {
                        data = propertyInfo.GetValue(_telemetryData, null);
                    }
                    else
                    {
                        throw new UnknownTelemetryValueException(name);
                    }
                    tv = new ETS2TelemetryValue(name, data);
                    object value = tv.Value;
                    if (value == null)
                    {
                        throw new UnknownTelemetryValueException(name);
                    }

                    break;
            }

            return tv;
        }
    }
}