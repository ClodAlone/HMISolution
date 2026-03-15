using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using FanucCNC.Focas_Library;

namespace FanucCNC
{
    public class FanucCNCDynTag_cnc_rdalmmsg2 : FanucCNCDynTag_BaseFunction
    {
        const String AlarmMessageLayoutParameter = "LAYOUT";

        public FanucCNCDynTag_cnc_rdalmmsg2(FanucCNCDynTagSettings main, string settings) : base(main, settings)
        {
            StringLength = FanucCNCProtocol.MAX_STRING_SIZE_PARAMETER;
            CNCPathSupported = true;
        }

        public override void SplitSettings(string settings)
        {
            _AlarmMessageLayout = GetPartByName(AlarmMessageLayoutParameter).Trim();
            if (string.IsNullOrEmpty(_AlarmMessageLayout))
                _AlarmMessageLayout = "{alm_no} - {alm_msg}";

            IsValid = true;

            CalculateInternalParameters();
        }

        public override void CalculateInternalParameters(List<Tag> tagList = null, int offsetValue = 0, bool aggregation = false)
        {
            if (Main.ArrayDimension != 0)
                _NrAlarms = (short)Main.ArrayDimension;
            else
                _NrAlarms = 1;

            // all alarms
            _AlarmType = -1;
        }

        public override string CreateSettings()
        {
            var parameters = new StringBuilder(); // base.ToString());            
            if (!string.IsNullOrWhiteSpace(_AlarmMessageLayout))
                AddParameter(ref parameters, AlarmMessageLayoutParameter, _AlarmMessageLayout.Trim());
            return parameters.ToString();
        }

        public DriverErrorCodes ParseReadData(Focas1.ODBALMMSG2_data_custom[] dataRead, short nrElements, out byte[] data)
        {
            data = new byte[_NrAlarms * FanucCNCProtocol.MAX_STRING_SIZE_PARAMETER];

            if (nrElements > _NrAlarms)
                nrElements = _NrAlarms;

            for (int n = 0; n < nrElements; n++) {
                string msg = _AlarmMessageLayout.Replace("{alm_no}", dataRead[n].alm_no.ToString()).Replace("{type}", dataRead[n].type.ToString()).Replace("{axis}", dataRead[n].axis.ToString()).Replace("{alm_msg}", dataRead[n].alm_msg);

                if (msg.Length > FanucCNCProtocol.MAX_STRING_SIZE_PARAMETER)
                    msg = msg.Substring(0, FanucCNCProtocol.MAX_STRING_SIZE_PARAMETER - 1);

                byte[] d = Encoding.ASCII.GetBytes(msg);

                Array.Copy(d, 0, data, (n * StringLength), d.Length);
            }

            return DriverErrorCodes.ErrorNoError;
        }

        public DriverErrorCodes PrepareReadRequest(out Focas1.ODBALMMSG2_data_custom[] dataToWrite, out short cnc_NumAlm)
        {
            dataToWrite = new Focas1.ODBALMMSG2_data_custom[_NrAlarms];

            cnc_NumAlm = _NrAlarms;

            return DriverErrorCodes.ErrorNoError;
        }

        private bool ContainAlarmFields(string alarmLayout)
        {
            return (alarmLayout.Contains("{alm_no}") || alarmLayout.Contains("{type}") || alarmLayout.Contains("{axis}") || alarmLayout.Contains("{alm_msg}"));
        }

        #region IDataErrorInfo Members
        public override String PerformValidation(String propertyName)
        {
            switch (propertyName)
            {
                case "FunctionCode":
                    if (Main.VarType != UFUAModel.DataType.String)
                        return Properties.Resources.ErrorOnlyStringDataTypeAllowed;                    
                    break;

                case "AlarmMessageLayout":
                    //if (Main.VarType != UFUAModel.DataType.String)
                    //    return Properties.Resources.ErrorOnlyStringDataTypeAllowed;

                    if (!ContainAlarmFields(_AlarmMessageLayout))
                        return Properties.Resources.ErrorAlarmLayoutInvalid;
                    break;
                
                case "TagLinkType":
                    if (Main.TagLinkType != (int)LinkType.Input)
                        return Properties.Resources.ErrorLinkTypeInvalid;
                    break;
            }

            return null;
        }
        #endregion

        #region Properties
        private short _NrAlarms;

        public short _AlarmType;
        public short AlarmType
        {
            get { return _AlarmType; }
        }

        private string _AlarmMessageLayout;
        public string AlarmMessageLayout
        {
            get { return _AlarmMessageLayout; }
            set { _AlarmMessageLayout = value; }
        }
        #endregion

        #region INotifyPropertyChanged Members
        public override void OnPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case "DataType":
                    OnPropertyChanged(new PropertyChangedEventArgs("AlarmMessageLayout"));
                    break;
            }
        } 
        #endregion
    }
}
