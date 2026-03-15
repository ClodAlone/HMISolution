using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DriverCodeBase.Helpers;
using DriverCodeBase.Enumerators;
using System.ComponentModel;

namespace RMS621
{
    public sealed class RMS621DynTagSettings : DynTagSettings
    {
        #region Constructors

        public RMS621DynTagSettings()
            : base()
        {
            _Command = RMS621Protocol.Command.Read;
            _Process = RMS621Protocol.Process.HeatFlow;
            _ProcessNumber = 1;
            // force as default Input; InputOutput is not supported by device
            TagLinkType = (int)LinkType.Input;
        }

        #endregion
        
        #region Static Members

        private static readonly String CommandParameter = "CMD";
        private static readonly String CommandProcess= "PRO";
        private static readonly String CommandProcessNumber = "NUM";

        #endregion

        #region Properties

        //private UFUAModel.DataType _VarType;
        //[Category("General")]
        //[Description("Variable Type")]
        //public override UFUAModel.DataType VarType
        //{
        //    get
        //    {
        //        return _VarType;
        //    }
        //    set
        //    {
        //        _VarType = value;                
        //        OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
        //    }
        //}
        //private uint _ArrayDimension;
        //[Category("General")]
        //[Description("Array Dimension")]
        //public override uint ArrayDimension
        //{
        //    get
        //    {
        //        return _ArrayDimension;
        //    }
        //    set
        //    {
        //        _ArrayDimension = value;                
        //        OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
        //    }
        //}

        /// <summary>   Number of element to exchange. </summary>
        //private int _ElementNumber;
        //[Category("General")]
        //[Description("Element Number")]
        //public override int ElementNumber
        //{
        //    get { return _ElementNumber; }
        //    set
        //    {
        //        _ElementNumber = value;                
        //        OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
        //    }
        //}

        private RMS621Protocol.Command _Command;
        [Category("Device Data")]
        [Description("Command")]
        public RMS621Protocol.Command Command
        {
            get { return _Command; }
            set
            {
                _Command = value;
                OnPropertyChanged("Process");
                OnPropertyChanged("ProcessNumber");
                OnPropertyChanged("TagLinkType");
            }
        }

        private RMS621Protocol.Process _Process;
        [Category("Device Data")]
        [Description("Process")]
        public RMS621Protocol.Process Process
        {
            get { return _Process; }
            set
            {
                _Process = value;
                OnPropertyChanged("Command");
                OnPropertyChanged("ProcessNumber");
                OnPropertyChanged("TagLinkType");
            }
        }

        private uint _ProcessNumber;
        [Category("Device Data")]
        [Description("Process Number")]
        public uint ProcessNumber
        {
            get { return _ProcessNumber; }
            set
            {
                _ProcessNumber = value;
                OnPropertyChanged("Command");
                OnPropertyChanged("Process");
                OnPropertyChanged("TagLinkType");
            }
        }
        #endregion

        #region Override Functions

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);
            Command = (RMS621Protocol.Command)helper.GetPartByName(CommandParameter, (int)(RMS621Protocol.Command.Read));
            Process = (RMS621Protocol.Process)helper.GetPartByName(CommandProcess, (int)(RMS621Protocol.Process.None));
            ProcessNumber = (uint)helper.GetPartByName(CommandProcessNumber, 1);
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            Command = (RMS621Protocol.Command)helper.GetPartByName(CommandParameter, (int)(RMS621Protocol.Command.Read));
            Process = (RMS621Protocol.Process)helper.GetPartByName(CommandProcess, (int)(RMS621Protocol.Process.None));
            ProcessNumber = (uint)helper.GetPartByName(CommandProcessNumber,1);

            return true;
        }

        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", CommandParameter, DynamicStringParser.CharAssign, (int)Command);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", CommandProcess, DynamicStringParser.CharAssign, (int)Process);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", CommandProcessNumber, DynamicStringParser.CharAssign, ProcessNumber);

            return dynamicstring.ToString();
        }

        public override bool isTagByteSizeOk(uint ByteSize)
        {            
            return (RMS621Protocol.BUFFER_SIZE >= ByteSize);
        }
        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            switch (propertyName) 
            {
                case "TagLinkType":
                    switch ((LinkType)TagLinkType)
                    {
                        case LinkType.InputOutput:
                            return string.Format(Properties.Resources.ErrorLinkTypeNotSupported, (LinkType)TagLinkType);                            
                        case LinkType.Input:
                            if (_Command != RMS621Protocol.Command.Read)
                                return string.Format(Properties.Resources.ErrorLinkTypeNotSupportedForSelectedCommand, (LinkType)TagLinkType);
                            break;
                        case LinkType.ExceptionOutput:
                        case LinkType.UnconditionalOutput:
                            if (_Command == RMS621Protocol.Command.Read)
                                return string.Format(Properties.Resources.ErrorLinkTypeNotSupportedForSelectedCommand, (LinkType)TagLinkType);
                            break;
                    }
                    break;
                case "Command":
                    if (!Enum.IsDefined(typeof(RMS621Protocol.Command),_Command))
                        return Properties.Resources.ErrorInvalidCommand;
                    break;
                case "Process":
                    if (!Enum.IsDefined(typeof(RMS621Protocol.Process), _Process))
                        return Properties.Resources.ErrorInvalidProcess;
                    break;
                case "ProcessNumber":
                    if (_ProcessNumber < 1 || _ProcessNumber > 3)
                        return Properties.Resources.ErrorInvalidProcessNumber;
                    break;
                case "ArrayDimension":
                    if (ArrayDimension != 0)
                        return Properties.Resources.ErroorProtocolSetIncompatibleWithArrays;
                    break;
                case "VarType":
                    {
                        if ((VarType == UFUAModel.DataType.String) || (VarType == UFUAModel.DataType.Boolean))
                        {
                            return (UFUAModel.Properties.Resources.DataTypeInvalid);
                        }
                    }
                    break;
            }

            return null;
        }

        #endregion

        #region INotifyPropertyChanged Members

        //protected override void OnPropertyChanged(string propertyName)
        //{
        //    OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

        //    switch (propertyName)
        //    {
        //        //case "VarType":
        //        //    OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
        //        //    OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
        //        //    break;
        //        //case "ArrayDimension":
        //        //    OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
        //        //    OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
        //        //    break;
        //        //case "ElementNumber":
        //        //    OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
        //        //    OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
        //        //    break;
        //    }
        //}
        #endregion
    }
}
