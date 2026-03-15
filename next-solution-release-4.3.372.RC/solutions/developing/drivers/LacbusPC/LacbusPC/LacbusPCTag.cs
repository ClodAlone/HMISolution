using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverCodeBase;
using DriverBaseInterfaces;
using Opc.Ua;

namespace LacbusPC
{
    public enum DatumTypes
    {
        DigitalInput = 0,
        DigitalOutput = 1,
        AnalogInput = 2,
        AnalogOutput = 3,
        CountInput = 4,
        Alarm = 10,
        RTUPollRequest = 5,
        SetDateTime = 6,
        ModbusCoilSetpoints = 7,
        ModbusRegisterSetpoints = 8,
        ShutdownFR1000FrontEnd = 9
     }

    public enum DatumFormats
    {
        Logical,
        Double,
        Float
    }

    public enum DatumCategories
    {
        Instantaneous,
        Historical,
        ReportAverageMeasurement,
        ReportMinimumMeasurement,
        ReportMaximumMeasurement,
        ReportCountIndex,
        ReportCountTimeBand1,
        ReportCountTimeBand2,
        ReportCountTimeBand3,
        ReportDIEventCount,
        ReportDIActiveStateTimeCount
    }

    public sealed class LacbusPCTag : Tag
    {
        #region Constructors

        public LacbusPCTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        public LacbusPCTag(TagDefinition tag)
            : base(tag)
        {

        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            bool res = LacbusPCDynSettings.TryParse(dynamicSettings);

            return res;
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)LacbusPCDynSettings; }
        }

        #endregion

        #region Properties

        private readonly LacbusPCDynTagSettings _LacbusPCDynSettings = new LacbusPCDynTagSettings();
        public LacbusPCDynTagSettings LacbusPCDynSettings
        {
            get { return _LacbusPCDynSettings; }
        }

        #endregion
    }
}
