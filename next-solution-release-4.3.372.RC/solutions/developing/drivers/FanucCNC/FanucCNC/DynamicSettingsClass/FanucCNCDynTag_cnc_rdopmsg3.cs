using DriverCodeBase;
using DriverCodeBase.Enumerators;
using FanucCNC.Focas_Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FanucCNC
{
    public class FanucCNCDynTag_cnc_rdopmsg3 : FanucCNCDynTag_BaseFunction
    {
        const String OperatorMessageLayoutParameter = "LAYOUT";

        //public enum ReadOperatorMsgType : short
        //{
        //    Unknown = -2,
        //    Message1st = 0,
        //    Message2nd = 1,
        //    Message3rd = 2,
        //    Message4th = 3,
        //    MacroMessage = 4,
        //    //5~16	:	5th - 16th message(30i, 0i-D/F, PMi-A olny)
        //    Message5th = 5, 
        //    Message6th = 6,
        //    Message7th = 7,
        //    Message8th = 8,
        //    Message9th = 9,
        //    Message10th = 10,
        //    Message11th = 11,
        //    Message12th = 12,
        //    Message13th = 13,
        //    Message14th = 14,
        //    Message15th = 15,
        //    Message16th = 16,
        //    //MessageAll = -1
        //}

        public FanucCNCDynTag_cnc_rdopmsg3(FanucCNCDynTagSettings main, string settings) : base(main, settings)
        {
            StringLength = FanucCNCProtocol.MAX_STRING_SIZE_PARAMETER;
            CNCPathSupported = true;
        }

        public override void SplitSettings(string settings)
        {
            //_MessageID = (ReadOperatorMsgType)(GetPartByName(OperatorMsgParameter, (int)ReadOperatorMsgType.Unknown));
            _OperatorMessageLayout = GetPartByName(OperatorMessageLayoutParameter);
            if (string.IsNullOrWhiteSpace(_OperatorMessageLayout))
                _OperatorMessageLayout = "{datano} - {data}";

            IsValid = true;

            CalculateInternalParameters();            
        }

        public override void CalculateInternalParameters(List<Tag> tagList = null, int offsetValue = 0, bool aggregation = false)
        {
            if (Main.ArrayDimension != 0)
                _NrMessages = (short)Main.ArrayDimension;
            else
                _NrMessages = 1;

            // all alarms
            _OperatorType = -1;
        }

        public override string CreateSettings()
        {
            var parameters = new StringBuilder(); // base.ToString());
            //AddParameter(ref parameters, OperatorMsgParameter, (int)_MessageID);
            if (!string.IsNullOrWhiteSpace(_OperatorMessageLayout))
                AddParameter(ref parameters, OperatorMessageLayoutParameter, _OperatorMessageLayout.Trim());
            return parameters.ToString();
        }


        public DriverErrorCodes ParseReadData(Focas1.OPMSG3_data_custom[] dataRead, short nrMessages, out byte[] data)
        {
            data = new byte[_NrMessages * StringLength];

            if (nrMessages > _NrMessages)
                nrMessages = _NrMessages;

            for (int n = 0; n < nrMessages; n++)
            {
                //dataRead[n].alm_no = n;
                //dataRead[n].alm_msg = "Alarm " + n.ToString();
                string msg = _OperatorMessageLayout.Replace("{datano}", dataRead[n].datano.ToString()).Replace("{type}", dataRead[n].type.ToString()).Replace("{data}", dataRead[n].data);
                if (msg.Length > StringLength)
                    msg = msg.Substring(0, (int)(StringLength - 1));

                byte[] d = Encoding.ASCII.GetBytes(msg);

                Array.Copy(d, 0, data, (n * StringLength), d.Length);
            }

            return DriverErrorCodes.ErrorNoError;
        }

        public DriverErrorCodes PrepareReadRequest(out Focas1.OPMSG3_data_custom[] dataToWrite, out short cnc_NumMsg)
        {
            dataToWrite = new Focas1.OPMSG3_data_custom[_NrMessages];

            cnc_NumMsg = _NrMessages;

            return DriverErrorCodes.ErrorNoError;
        }

        private bool ContainOperatorFields(string alarmLayout)
        {
            return (alarmLayout.Contains("{datano}") || alarmLayout.Contains("{type}") || alarmLayout.Contains("{data}"));
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

                case "OperatorMessageLayout":
                    if (!ContainOperatorFields(_OperatorMessageLayout))
                        return Properties.Resources.ErrorOperatorLayoutInvalid;

                    //if (Main.VarType != UFUAModel.DataType.String)
                    //    return Properties.Resources.ErrorOnlyStringDataTypeAllowed;
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
        private short _NrMessages;
       
        public short _OperatorType;
        public short OperatorType
        {
            get { return _OperatorType; }
        }

        private string _OperatorMessageLayout;
        public string OperatorMessageLayout
        {
            get { return _OperatorMessageLayout; }
            set { _OperatorMessageLayout = value; }
        }
        #endregion

        #region INotifyPropertyChanged Members
        public override void OnPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case "DataType":
                    OnPropertyChanged(new PropertyChangedEventArgs("OperatorMessageLayout"));
                    break;
            }
        }
        #endregion
    }
}
