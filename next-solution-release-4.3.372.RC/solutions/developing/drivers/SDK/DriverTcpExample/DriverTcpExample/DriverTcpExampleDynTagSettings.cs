////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	DriverTcpExampleDynTagSettings.cs
//
// summary:	Implements the driver TCP example dynamic tag settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DriverCodeBase.Helpers;
using System.ComponentModel;
using DriverBaseInterfaces;
using Opc.Ua;

namespace DriverTcpExample
{
    /// <summary>   Dynamic tag settings of the DriverTcpExample driver. </summary>
    public sealed class DriverTcpExampleDynTagSettings : DynTagSettings
    {
        #region Constructors

        /// <summary>   Initializes the DriverTcpExampleDynTagSettings. </summary>
        public DriverTcpExampleDynTagSettings()
            : base()
        {
            FunctionCode = FunctionCodes.MultipleRegisters;
            StartAddress = 0;
        }

        #endregion
        
        #region Static Members

        /// <summary>   The function code parameter. </summary>
        private static readonly String FunctionCodeParameter = "FC";
        /// <summary>   The start address parameter. </summary>
        private static readonly String StartAddressParameter = "SA";
        
        
        #endregion

        #region Override Functions

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes the DriverTcpExampleDynTagSettings from string dynamicSettings.
        /// </summary>
        ///
        /// <param name="dynamicSettings">  . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            FunctionCode = (FunctionCodes)(helper.GetPartByName(FunctionCodeParameter, (UInt16)FunctionCodes.MultipleRegisters));
            StartAddress = helper.GetPartByName(StartAddressParameter, (UInt16)0);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes the DriverTcpExampleDynTagSettings from string dynamicSettings if is possible.
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
            if (String.IsNullOrEmpty(helper.GetPartByName(FunctionCodeParameter)))
                return false;

            // optional parameters
            FunctionCode = (FunctionCodes)(helper.GetPartByName(FunctionCodeParameter, (UInt16)FunctionCodes.MultipleRegisters));
            StartAddress = helper.GetPartByName(StartAddressParameter, (UInt16)0);

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Convert DriverTcpExampleDynTagSettings to a string. </summary>
        ///
        /// <returns>   A string that represents this object. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());
			dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", FunctionCodeParameter, DynamicStringParser.CharAssign, (int)FunctionCode);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", StartAddressParameter, DynamicStringParser.CharAssign, StartAddress);
            return dynamicstring.ToString();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the DriverTcpExampleDynTagSettings from string address. </summary>
        ///
        /// <param name="address">  . </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ParseAddress(string address)
        {
            try
            {
                if (address.Length > 2)
                {
                    FunctionCodes fc = FunctionCodes.MultipleRegisters;
                    UInt16 sa = Convert.ToUInt16(address.Substring(2));
                    switch (address.Substring(0, 2))
                    {
                        case "CS":
                            fc = FunctionCodes.Coils;
                            break;
                        case "HR":
                            fc = FunctionCodes.MultipleRegisters;
                            break;
                    }
                    FunctionCode = fc;
                    StartAddress = sa;
                    return true;
                }
            }
            catch (Exception e)
            { }
            //dynamicaddress = "";
            return false;
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
        public override string GetNextDynSetting(TagDefinition prevtagdefinition, TagDefinition thistagdefinition)
        {
            bool bits = (FunctionCode == FunctionCodes.Coils);

            if (prevtagdefinition.DataType.IdType == IdType.Numeric)
            {
                switch ((uint)prevtagdefinition.DataType.Identifier)
                {
                    case (uint)BuiltInType.Boolean:
                        if(bits)
                            StartAddress += (prevtagdefinition.ArrayDimension > 0 ? (UInt16)prevtagdefinition.ArrayDimension : (UInt16)1);
                        else
                            StartAddress += (prevtagdefinition.ArrayDimension > 0 ? (UInt16)(prevtagdefinition.ArrayDimension / 16 + (prevtagdefinition.ArrayDimension % 16 > 0 ? 1 : 0)) : (UInt16)1);
                        break;
                    case (uint)BuiltInType.SByte:
                    case (uint)BuiltInType.Byte:
                        if (bits)
                            StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? prevtagdefinition.ArrayDimension : 1) * 8);
                        else
                            StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension / 2 + (prevtagdefinition.ArrayDimension % 2 > 0 ? 1: 0)) : 1));
                        break;
                    case (uint)BuiltInType.Int16:
                    case (uint)BuiltInType.UInt16:
                        if (bits)
                            StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? prevtagdefinition.ArrayDimension : 1) * 2 * 8);
                        else
                            StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension) : 1));
                        break;
                    case (uint)BuiltInType.Float:
                    case (uint)BuiltInType.UInt32:
                    case (uint)BuiltInType.Int32:
                    case (uint)BuiltInType.UInt64:
                    case (uint)BuiltInType.Int64:
                    case (uint)BuiltInType.Double:
                        if (bits)
                            StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? prevtagdefinition.ArrayDimension : 1) * 4 * 8);
                        else
                            StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension * 2) : 2));
                        break;
                }
            }

            return ToString();
        }
        #endregion

        #region Properties

        /// <summary>   The function code. </summary>
        private FunctionCodes _FunctionCode;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   DriverTcpExample memory area Property. </summary>
        ///
        /// <value> The function code. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Data Area")]
        public FunctionCodes FunctionCode
        {
            get { return _FunctionCode; }
            set
            {
                _FunctionCode = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TagLinkType"));
            }
        }

        /// <summary>   The start address. </summary>
        private UInt16 _StartAddress;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   DriverTcpExample start address memory Property. </summary>
        ///
        /// <value> The start address. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Start Address")]
        public UInt16 StartAddress
        {
            get { return _StartAddress; }
            set
            {
                _StartAddress = value;
            }
        }

        #endregion
        #region IDataErrorInfo Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Do not modify. </summary>
        ///
        /// <value> The error. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public new string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Do not modify. </summary>
        ///
        /// <param name="propertyName" type="string">   Name of the property. </param>
        ///
        /// <returns>   The indexed item. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public new string this[string propertyName]
        {
            get
            {
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var value = GetType().GetProperty(propertyName).GetValue(this, null);

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   DriverTcpExampleDynTagSettings property validation. </summary>
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

            if (propertyName == "FunctionCode")
            {
                if (InvalidFunctionCodeLinkType())
                    return Properties.Resources.FunctionCodeRequireInput;

                if (DataTypeIncompatible())
                    return UFUAModel.Properties.Resources.DataTypeIncompatible;
            }
            else if (propertyName == "TagLinkType")
            {
                if (InvalidFunctionCodeLinkType())
                    return Properties.Resources.FunctionCodeRequireInput;
            }

            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Compatibility test between FunctionCode and VarType. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool DataTypeIncompatible()
        {
            return ((FunctionCode == FunctionCodes.MultipleRegisters && VarType == UFUAModel.DataType.Boolean) ||
               (FunctionCode == FunctionCodes.Coils && VarType != UFUAModel.DataType.Boolean));
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Compatibility test between FunctionCode and TagLinkType. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool InvalidFunctionCodeLinkType()
        {
            return false;
        }

        #endregion



        
    }
}
