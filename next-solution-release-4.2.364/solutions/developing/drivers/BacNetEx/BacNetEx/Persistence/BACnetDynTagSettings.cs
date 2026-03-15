////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	BACnetDynTagSettings.cs
//
// summary:	Implements the driver BACnet dynamic tag settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Helpers;
using DriverCodeBaseEx.Enumerators;
using System.ComponentModel;
using DriverBaseInterfaces;
using Opc.Ua;
using DevExpress.Xpo;

namespace BACnet
{
    /// <summary>   Dynamic tag settings of the BACnet driver. </summary>
    public sealed class BACnetDynTagSettings : DynTagSettings
    {
        #region Constructors

        /// <summary>   Initializes the BACnetDynTagSettings. </summary>
        public BACnetDynTagSettings()
            : base()
        {
            _ObjectName = "";
            _BACnetObjectType = BACnetEnums.ObjectTypes.ANALOG_VALUE;
            _PropertyIdentifier = BACnetEnums.PropertyIdentifier.PRESENT_VALUE;
            _COVEnable = false;
            _DataSize = 0;
            _ArrayIndex = 1;
            _DataLogMode = DataLogModes.None;
            _PriorityLevel = BACnetEnums.DefaultPriorityLevel;
        }

        #endregion

        #region Static Members

        /// <summary>   The function code parameter. </summary>
        private static readonly String ObjectTypeParameter = "OBT";
        /// <summary>   The start address parameter. </summary>
        private static readonly String ObjectNameParameter = "OBN";
        /// <summary>   The property identifier parameter. </summary>
        private static readonly String PropertyIdentifierParameter = "PRI";
        /// <summary>   The COV Enable parameter. </summary>
        private static readonly String COVEnableParameter = "CE";
        /// <summary>   The Data Size Parameter. </summary>
        private static readonly String DataSizeParameter = "DS";
        /// <summary>   The Array Index Parameter. </summary>
        private static readonly String ArrayIndexParameter = "AI";
        /// <summary>   The Data Log Mode parameter. </summary>
        private static readonly String DataLogModeParameter = "DLM";
        /// <summary>   The Priority Level parameter. </summary>
        private static readonly String PriorityLevelParameter = "PL";
        /// <summary>   The start instance number. </summary>
        private static readonly String InstanceNumberParameter = "INN";

        #endregion

        #region Override Functions

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes the BACnetDynTagSettings from string dynamicSettings.
        /// </summary>
        ///
        /// <param name="dynamicSettings">  . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            BACnetObjectType = (BACnetEnums.ObjectTypes)(helper.GetPartByName(ObjectTypeParameter, (UInt16)BACnetEnums.ObjectTypes.ANALOG_VALUE));
            ObjectName = helper.GetPartByName(ObjectNameParameter);
            PropertyIdentifier = (BACnetEnums.PropertyIdentifier)(helper.GetPartByName(PropertyIdentifierParameter, (UInt16)BACnetEnums.PropertyIdentifier.PRESENT_VALUE));
            COVEnable = helper.GetPartByName(COVEnableParameter, false);
            DataSize = helper.GetPartByName(DataSizeParameter, (ushort)0);
            ArrayIndex = helper.GetPartByName(ArrayIndexParameter, (ushort)1);
            DataLogMode = (DataLogModes)(helper.GetPartByName(DataLogModeParameter, (UInt16)DataLogModes.None));
            PriorityLevel = (PriorityLevels)(helper.GetPartByName(PriorityLevelParameter, (UInt16)BACnetEnums.DefaultPriorityLevel));
            InstanceNumber = helper.GetPartByName(InstanceNumberParameter,-1);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes the BACnetDynTagSettings from string dynamicSettings if is possible.
        /// </summary>
        ///
        /// <param name="dynamicSettings">  . </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            // required parameter
            if (String.IsNullOrEmpty(helper.GetPartByName(ObjectTypeParameter)))
                return false;
            if (String.IsNullOrEmpty(helper.GetPartByName(ObjectNameParameter)))
                return false;
            BACnetObjectType = (BACnetEnums.ObjectTypes)(helper.GetPartByName(ObjectTypeParameter, (UInt16)BACnetEnums.ObjectTypes.DEVICE));
            ObjectName = helper.GetPartByName(ObjectNameParameter);

            PropertyIdentifier = (BACnetEnums.PropertyIdentifier)(helper.GetPartByName(PropertyIdentifierParameter, (UInt16)BACnetEnums.PropertyIdentifier.PRESENT_VALUE));
            COVEnable = helper.GetPartByName(COVEnableParameter, false);
            DataSize = helper.GetPartByName(DataSizeParameter, (ushort)0);
            ArrayIndex = helper.GetPartByName(ArrayIndexParameter, (ushort)1);
            DataLogMode = (DataLogModes)(helper.GetPartByName(DataLogModeParameter, (UInt16)DataLogModes.None));
            PriorityLevel = (PriorityLevels)(helper.GetPartByName(PriorityLevelParameter, (UInt16)BACnetEnums.DefaultPriorityLevel));
            InstanceNumber = helper.GetPartByName(InstanceNumberParameter,-1);

            // optional parameters

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Convert BACnetDynTagSettings to a string. </summary>
        ///
        /// <returns>   A string that represents this object. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());
			dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", ObjectTypeParameter, DynamicStringParser.CharAssign, (int)BACnetObjectType);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", ObjectNameParameter, DynamicStringParser.CharAssign, ObjectName);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", PropertyIdentifierParameter, DynamicStringParser.CharAssign, (int)PropertyIdentifier);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", COVEnableParameter, DynamicStringParser.CharAssign, COVEnable);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", DataSizeParameter, DynamicStringParser.CharAssign, DataSize);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", ArrayIndexParameter, DynamicStringParser.CharAssign, ArrayIndex);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", DataLogModeParameter, DynamicStringParser.CharAssign, (int)DataLogMode);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", PriorityLevelParameter, DynamicStringParser.CharAssign, (int)PriorityLevel);
            if (InstanceNumber >= 0)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", InstanceNumberParameter, DynamicStringParser.CharAssign, InstanceNumber);
            }

            return dynamicstring.ToString();
        }


        public override bool isTagByteSizeOk(uint ByteSize)
        {
            return (true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Return the string to link a tag after, in target device memory, the tag prevtagdefinition.
        /// </summary>
        ///
        /// <param name="prevtagdefinition">    . </param>
        /// <param name="thistagdefinition">    . </param>
        ///
        /// <returns>   The next dynamic setting. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //TODO
        //public override string GetNextDynSetting(TagDefinition prevtagdefinition, TagDefinition thistagdefinition)
        //{
        //    return ToString();
        //}
        
        public override UFUAModel.DataType getProtocolDataType()
        {
            switch (PropertyIdentifier)
            {
                case BACnetEnums.PropertyIdentifier.STATE_TEXT:
                case BACnetEnums.PropertyIdentifier.PRIORITY_ARRAY:
                    if (ArrayIndex == 0)
                        return UFUAModel.DataType.UInt32;
                    else
                        return BACnetEnums.ApplicationTagDataType[BACnetEnums.ObjectPropertyTypeDictionary[BACnetObjectType.ToString()][PropertyIdentifier]];
                default:
                    return BACnetEnums.ApplicationTagDataType[BACnetEnums.ObjectPropertyTypeDictionary[BACnetObjectType.ToString()][PropertyIdentifier]];
            }
        }

        #endregion

        #region Properties

        /// <summary>   Type of the tag link. </summary>
        //private int _TagLinkType;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Modes to access the tags. Override base property for simultaneous validation of
        /// "BACnetObjectType" property.
        /// </summary>
        ///
        /// <value> The type of the tag link. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //[Category("General")]
        //[Description("Link Type")]
        //public override int TagLinkType
        //{
        //    get { return _TagLinkType; }
        //    set
        //    {
        //        _TagLinkType = value;
        //        OnPropertyChanged(new PropertyChangedEventArgs("BACnetObjectType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ObjectName"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("PropertyIdentifier"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("OutputAtStartup"));
        //    }
        //}

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
        //        OnPropertyChanged(new PropertyChangedEventArgs("BACnetObjectType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ObjectName"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("PropertyIdentifier"));
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
        //        OnPropertyChanged(new PropertyChangedEventArgs("BACnetObjectType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ObjectName"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("PropertyIdentifier"));
        //    }
        //}

        ///// <summary>   Number of element to exchange. </summary>
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
        //        OnPropertyChanged(new PropertyChangedEventArgs("BACnetObjectType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ObjectName"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("PropertyIdentifier"));
        //    }
        //}

        /// <summary>   The Object Type. </summary>
        private BACnetEnums.ObjectTypes _BACnetObjectType;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Object Type. </summary>
        ///
        /// <value> The Object Type. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("BACnet Object Type")]
        public BACnetEnums.ObjectTypes BACnetObjectType
        {
            get { return _BACnetObjectType; }
            set
            {
                _BACnetObjectType = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TagLinkType"));
                OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                OnPropertyChanged(new PropertyChangedEventArgs("ObjectName"));
                OnPropertyChanged(new PropertyChangedEventArgs("PropertyIdentifier"));
            }
        }

        /// <summary>   The Object Name. </summary>
        private string _ObjectName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Object Name. </summary>
        ///
        /// <value> The Object Name. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Object Name")]
        [Size(SizeAttribute.Unlimited)]
        public string ObjectName
        {
            get { return _ObjectName; }
            set
            {
                _ObjectName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TagLinkType"));
                OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                OnPropertyChanged(new PropertyChangedEventArgs("BACnetObjectType"));
                OnPropertyChanged(new PropertyChangedEventArgs("PropertyIdentifier"));
                OnPropertyChanged(new PropertyChangedEventArgs("InstanceNumber"));
            }
        }

        /// <summary>   The Property Identifier. </summary>
        private BACnetEnums.PropertyIdentifier _PropertyIdentifier;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Property Identifier. </summary>
        ///
        /// <value> The Property Identifier. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Property Identifier")]
        public BACnetEnums.PropertyIdentifier PropertyIdentifier
        {
            get { return _PropertyIdentifier; }
            set
            {
                _PropertyIdentifier = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TagLinkType"));
                OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                OnPropertyChanged(new PropertyChangedEventArgs("BACnetObjectType"));
                OnPropertyChanged(new PropertyChangedEventArgs("ObjectName"));
            }
        }

        /// <summary>   The COV Enable. </summary>
        private bool _COVEnable;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   COV Enable. </summary>
        ///
        /// <value> The COV Enable. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("COV Enable")]
        public bool COVEnable
        {
            get { return _COVEnable; }
            set
            {
                _COVEnable = value;
            }
        }

        /// <summary>   The Data Size. </summary>
        private ushort _DataSize;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Data Size. </summary>
        ///
        /// <value> The Data Size. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Data Size")]
        public ushort DataSize
        {
            get { return _DataSize; }
            set
            {
                _DataSize = value;
            }
        }

        /// <summary>   The Array Index. </summary>
        private ushort _ArrayIndex;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Array Index. </summary>
        ///
        /// <value> The Array Index. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Array Index")]
        public ushort ArrayIndex
        {
            get { return _ArrayIndex; }
            set
            {
                _ArrayIndex = value;
            }
        }

        /// <summary>   The Data Log Mode. </summary>
        private DataLogModes _DataLogMode;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Data Log Mode. </summary>
        ///
        /// <value> The Data Log Mode. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Data Log Mode")]
        public DataLogModes DataLogMode
        {
            get { return _DataLogMode; }
            set
            {
                _DataLogMode = value;
            }
        }

        /// <summary>   The PriorityLevel. </summary>
        private PriorityLevels _PriorityLevel;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Priority Level. </summary>
        ///
        /// <value> The Priority Level. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Priority Level")]
        public PriorityLevels PriorityLevel
        {
            get { return _PriorityLevel; }
            set
            {
                _PriorityLevel = value;
            }
        }

        /// <summary>   The Instance Number. </summary>
        private Int32 _InstanceNumber = -1;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Instance Number. </summary>
        ///
        /// <value> The Instance Number. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Instance Number")]
        public Int32 InstanceNumber
        {
            get { return _InstanceNumber; }
            set
            {
                _InstanceNumber = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ObjectName"));
            }
        }

        #endregion

        #region IDataErrorInfo Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   BACnetDynTagSettings property validation. </summary>
        ///
        /// <param name="propertyName"> . </param>
        ///
        /// <returns>   A String. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "BACnetObjectType" ||
                propertyName == "PropertyIdentifier")
            {
                if ((uint)VarType != unchecked((uint)(-1)))
                    return ProtocolDataSizeValidation(VarType);

                if (!BACnetEnums.ObjectPropertyWritableDictionary[BACnetObjectType.ToString()][PropertyIdentifier] &&
                    (LinkType)base.TagLinkType != LinkType.Input)
                    return string.Format(Properties.Resources.AreaTypeRequireInput, DriverCodeBaseEx.Properties.Resources.LinkType_Input);
            }
            else if (propertyName == "TagLinkType")
            {
                if (!BACnetEnums.ObjectPropertyWritableDictionary[BACnetObjectType.ToString()][PropertyIdentifier] &&
                    (LinkType)base.TagLinkType != LinkType.Input)
                    return string.Format(Properties.Resources.AreaTypeRequireInput, DriverCodeBaseEx.Properties.Resources.LinkType_Input);

            }
            else if (propertyName == "DataSize")
            {
                if ((VarType == UFUAModel.DataType.String) && (DataSize <= 0))
                {
                    return Properties.Resources.ErrorTheStringCanNotHaveDataSizeZero;
                }
            }
            else if (propertyName == "ObjectName")
            {
                //TODO
                if (InstanceNumber == -1)
                {
                    if (string.IsNullOrWhiteSpace(ObjectName))
                        return Properties.Resources.ErrorObjectNameInvalid;
                }

            }
            else if (propertyName == "InstanceNumber")
            {
                if (string.IsNullOrWhiteSpace(ObjectName))
                {
                    //checked if the string is not empty 
                    if (InstanceNumber > BACnetProtocol.MAX_INSTANCE_NUMBER)
                    {
                        return Properties.Resources.ErrorOutOfRangeInstansNumber;
                    }

                    //checked if the string is not empty 
                    if (InstanceNumber == -1)
                    {
                        return Properties.Resources.ErrorOutOfRangeInstansNumber;
                    }
                }

            }
            else if (propertyName == "ArrayIndex")
            {
                if (_PropertyIdentifier == BACnetEnums.PropertyIdentifier.PRIORITY_ARRAY)
                {                    
                    { 
                        if (ArrayIndex < 1 || ArrayIndex > 16)
                        {
                            return Properties.Resources.ErrorOutOfRangeArraIndexPriorityArray;
                        }
                    }
                }
            }

            return null;
        }
        #endregion

        protected override void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

            switch (propertyName)
            {
                case "TagLinkType":
                    OnPropertyChanged(new PropertyChangedEventArgs("BACnetObjectType"));
                    OnPropertyChanged(new PropertyChangedEventArgs("ObjectName"));
                    OnPropertyChanged(new PropertyChangedEventArgs("PropertyIdentifier"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("OutputAtStartup"));
                    break;
                case "VarType":
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    OnPropertyChanged(new PropertyChangedEventArgs("BACnetObjectType"));
                    OnPropertyChanged(new PropertyChangedEventArgs("ObjectName"));
                    OnPropertyChanged(new PropertyChangedEventArgs("PropertyIdentifier"));
                    break;
                case "ArrayDimension":
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    OnPropertyChanged(new PropertyChangedEventArgs("BACnetObjectType"));
                    OnPropertyChanged(new PropertyChangedEventArgs("ObjectName"));
                    OnPropertyChanged(new PropertyChangedEventArgs("PropertyIdentifier"));
                    break;
                case "ElementNumber":
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    OnPropertyChanged(new PropertyChangedEventArgs("BACnetObjectType"));
                    OnPropertyChanged(new PropertyChangedEventArgs("ObjectName"));
                    OnPropertyChanged(new PropertyChangedEventArgs("PropertyIdentifier"));
                    break;
            }
        }
    }
}
