using System;
using DriverCodeBase.Enumerators;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DriverCodeBase;
using Opc.Ua;
using System.Globalization;

namespace FanucCNC
{
    public class FanucCNCDynTag_BaseFunction: IDataErrorInfo, INotifyPropertyChanged
    {
        protected FanucCNCDynTagSettings Main;

        public const string StringLengthParameter = "STRLEN";
        public const char CharAssign = ':';
        public const char CharSep = ';';
        private Dictionary<string, string> _Parameters;

        public FanucCNCDynTag_BaseFunction(FanucCNCDynTagSettings main, string settings)
        {
            OffsetVariableSupported = false;
            CNCPathSupported = false;

            StringLength = 0;
            //HasSettings = false;
            InvalidReason = string.Empty;
            Main = main;
            
            SettingsParser(settings);

            SplitSettings(settings);
        }
        
        public void SetChannelStationInfo(FanucCNCProtocol.MachineSeries machineSerie, CommJob j = null)//, short cncPath)
        {
            MachineModel = machineSerie;
            //CNCPath = cncPath;

            if (j != null)
            {
                // is a struct ?
                if (j.TagsList.Count > 1)
                {
                    Main = new FanucCNCDynTagSettings
                    {
                        ArrayDimension = j.TagsList[0].TagNode.ArrayDimension,
                        TagLinkType = (int)j.Type,
                        VarType = GetStructType()
                    };
                }
                else
                {
                    Main = new FanucCNCDynTagSettings
                    {
                        ArrayDimension = j.TagsList[0].TagNode.ArrayDimension,
                        TagLinkType = (int)j.Type,
                        VarType = FanucCNCProtocol.GetDataType((BuiltInType)((uint)j.TagsList[0].TagNode.DataType.Identifier))
                    };
                }
                
                ReParseData();

                CalculateInternalParameters(j.TagsList);
            }
        }

        public virtual void CalculateInternalParameters(List<Tag> tagList = null, int offsetValue = 0, bool aggregation = false)
        {
        }

        public virtual void ReParseData()
        {            
        }

        public virtual void SplitSettings(string settings)
        {
            IsValid = true;
        }

        public virtual string CreateSettings()
        {
            return string.Empty;
        }

        public virtual String PerformValidation(String propertyName)
        {
            return string.Empty;
        }

        protected void AddParameter(ref StringBuilder parameters, string paramName,object paramValue)
        {
            parameters.AppendFormat("{0}{1}{2}{3}", paramName, CharAssign, paramValue, CharSep);
        }

        protected void SettingsParser(string settings)
        {
            Settings = settings;

            var parameters = settings.Split(CharSep);
            foreach (var parameter in parameters)
            {
                //var data = parameter.Split(CharAssign);
                int charSepPos = parameter.IndexOf(CharAssign, 0);
                if (charSepPos > 1)
                {
                    List<string> data = new List<string>();
                    data.Add(parameter.Substring(0, charSepPos));
                    data.Add(parameter.Substring(charSepPos + 1, parameter.Length-charSepPos-1));
                    if (_Parameters == null)
                        _Parameters = new Dictionary<string, string>();

                    _Parameters[data[0]] = data[1];
                }
            }
        }

        /// <summary>
        /// Force Tag's value to allow biggest value to simulate variable not found
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public DriverErrorCodes SetDefaultValueNotExistingVariable(out byte[] data)
        {
            data = null;

            switch (Main.VarType)
            {
                // not manage
                //case UFUAModel.DataType.Boolean:
                //    break;
                case UFUAModel.DataType.Byte:
                    data = BitConverter.GetBytes(Byte.MaxValue);
                    break;
                case UFUAModel.DataType.SByte:
                    data = BitConverter.GetBytes(SByte.MaxValue);
                    break;
                case UFUAModel.DataType.Int16:
                    data = BitConverter.GetBytes(Int16.MaxValue);
                    break;
                case UFUAModel.DataType.UInt16:
                    data = BitConverter.GetBytes(UInt16.MaxValue);
                    break;
                case UFUAModel.DataType.Int32:
                    data = BitConverter.GetBytes(Int32.MaxValue);
                    break;
                case UFUAModel.DataType.UInt32:
                    data = BitConverter.GetBytes(UInt32.MaxValue);
                    break;
                case UFUAModel.DataType.Int64:
                    data = BitConverter.GetBytes(Int64.MaxValue);
                    break;
                case UFUAModel.DataType.UInt64:
                    data = BitConverter.GetBytes(UInt64.MaxValue);
                    break;
                case UFUAModel.DataType.Float:
                    data = BitConverter.GetBytes(Single.MaxValue);
                    break;
                case UFUAModel.DataType.Double:
                    data = BitConverter.GetBytes(Double.MaxValue);
                    break;
                // not manage
                //case UFUAModel.DataType.String:
                //    break;
            }

            return DriverErrorCodes.ErrorNoError;
        }

        protected DriverErrorCodes ConvertFromMoviconDataType(byte[] jobdata, out byte result)
        {
            DriverErrorCodes ret = DriverErrorCodes.ErrorNoError;
            result = 0;

            try
            {
                switch (Main.VarType)
                {
                    case UFUAModel.DataType.Boolean:
                        result = jobdata[0];
                        break;
                    case UFUAModel.DataType.SByte:
                        result = (byte)jobdata[0];
                        break;
                    case UFUAModel.DataType.Byte:
                        result = (byte)jobdata[0];
                        break;
                    case UFUAModel.DataType.Int16:
                        result = (byte)Convert.ToInt16(jobdata[0]);
                        break;
                    case UFUAModel.DataType.UInt16:
                        result = (byte)Convert.ToUInt16(jobdata[0]);
                        break;
                    case UFUAModel.DataType.Int32:
                        result = (byte)Convert.ToInt32(jobdata[0]);
                        break;
                    case UFUAModel.DataType.UInt32:
                        result = (byte)Convert.ToUInt32(jobdata[0]);
                        break;
                    case UFUAModel.DataType.Int64:
                        result = (byte)Convert.ToInt64(jobdata[0]);
                        break;
                    case UFUAModel.DataType.UInt64:
                        result = (byte)Convert.ToUInt64(jobdata[0]);
                        break;
                    case UFUAModel.DataType.Float:
                        result = (byte)Convert.ToSingle(jobdata[0]);
                        break;
                    case UFUAModel.DataType.Double:
                        result = (byte)Convert.ToDouble(jobdata[0]);
                        break;
                    default:
                        ret = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorConvertingInOutData;
                        break;
                }
            }
            catch (Exception ex)
            {
                ret = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorConvertingInOutData;
            }
            return ret;
        }

        protected DriverErrorCodes ConvertFromMoviconDataType(byte[] jobdata, out short result)
        {
            DriverErrorCodes ret = DriverErrorCodes.ErrorNoError;
            result = 0;

            try
            {
                switch (Main.VarType)
                {
                    case UFUAModel.DataType.Boolean:
                        result = (short)jobdata[0];
                        break;
                    case UFUAModel.DataType.SByte:
                        result = (short)jobdata[0];
                        break;
                    case UFUAModel.DataType.Byte:
                        result = (short)jobdata[0];
                        break;
                    case UFUAModel.DataType.Int16:
                        result = (short)BitConverter.ToInt16(jobdata, 0);
                        break;
                    case UFUAModel.DataType.UInt16:
                        result = (short)BitConverter.ToUInt16(jobdata, 0);
                        break;
                    case UFUAModel.DataType.Int32:
                        result = (short)BitConverter.ToInt32(jobdata, 0);
                        break;
                    case UFUAModel.DataType.UInt32:
                        result = (short)BitConverter.ToUInt32(jobdata, 0);
                        break;
                    case UFUAModel.DataType.Int64:
                        result = (short)BitConverter.ToInt64(jobdata, 0);
                        break;
                    case UFUAModel.DataType.UInt64:
                        result = (short)BitConverter.ToUInt64(jobdata, 0);
                        break;
                    case UFUAModel.DataType.Float:
                        result = (short)BitConverter.ToSingle(jobdata, 0);
                        break;
                    case UFUAModel.DataType.Double:
                        result = (short)BitConverter.ToDouble(jobdata, 0);
                        break;
                    default:
                        ret = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorConvertingInOutData;
                        break;
                }
            }
            catch (Exception ex)
            {
                ret = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorConvertingInOutData;
            }
            return ret;
        }


        protected DriverErrorCodes ConvertFromMoviconDataType(byte[] jobdata, out int result)
        {
            DriverErrorCodes ret = DriverErrorCodes.ErrorNoError;
            result = 0;

            try
            {
                switch (Main.VarType)
                {
                    case UFUAModel.DataType.Boolean:
                        result = (int)jobdata[0];
                        break;
                    case UFUAModel.DataType.SByte:
                        result = (int)jobdata[0];
                        break;
                    case UFUAModel.DataType.Byte:
                        result = (int)jobdata[0];
                        break;
                    case UFUAModel.DataType.Int16:
                        result = (int)BitConverter.ToInt16(jobdata, 0);
                        break;
                    case UFUAModel.DataType.UInt16:
                        result = (int)BitConverter.ToUInt16(jobdata, 0);
                        break;
                    case UFUAModel.DataType.Int32:
                        result = (int)BitConverter.ToInt32(jobdata, 0);
                        break;
                    case UFUAModel.DataType.UInt32:
                        result = (int)BitConverter.ToUInt32(jobdata, 0);
                        break;
                    case UFUAModel.DataType.Int64:
                        result = (int)BitConverter.ToInt64(jobdata, 0);
                        break;
                    case UFUAModel.DataType.UInt64:
                        result = (int)BitConverter.ToUInt64(jobdata ,0);
                        break;
                    case UFUAModel.DataType.Float:
                        result = (int)BitConverter.ToSingle(jobdata, 0);
                        break;
                    case UFUAModel.DataType.Double:
                        result = (int)BitConverter.ToDouble(jobdata, 0);
                        break;
                    default:
                        ret = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorConvertingInOutData;
                        break;
                }
            }
            catch (Exception ex)
            {
                ret = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorConvertingInOutData;
            }
            return ret;
        }

        protected DriverErrorCodes ConvertToMoviconDataType(byte dataRead, out byte[] dataOut)
        {
            DriverErrorCodes ret = DriverErrorCodes.ErrorNoError;

            dataOut = null;

            try
            {
                switch (Main.VarType)
                {
                    case UFUAModel.DataType.Boolean:
                        dataOut = BitConverter.GetBytes(Convert.ToBoolean(dataRead));
                        break;
                    case UFUAModel.DataType.SByte:
                        dataOut = BitConverter.GetBytes(Convert.ToSByte(dataRead));
                        break;
                    case UFUAModel.DataType.Byte:
                        dataOut = BitConverter.GetBytes(Convert.ToByte(dataRead));
                        break;
                    case UFUAModel.DataType.Int16:
                        dataOut = BitConverter.GetBytes(Convert.ToInt16(dataRead));
                        break;
                    case UFUAModel.DataType.UInt16:
                        dataOut = BitConverter.GetBytes(Convert.ToUInt16(dataRead));
                        break;
                    case UFUAModel.DataType.Int32:
                        dataOut = BitConverter.GetBytes(Convert.ToInt32(dataRead));
                        break;
                    case UFUAModel.DataType.UInt32:
                        dataOut = BitConverter.GetBytes(Convert.ToUInt32(dataRead));
                        break;
                    case UFUAModel.DataType.Int64:
                        dataOut = BitConverter.GetBytes(Convert.ToInt64(dataRead));
                        break;
                    case UFUAModel.DataType.UInt64:
                        dataOut = BitConverter.GetBytes(Convert.ToUInt64(dataRead));
                        break;
                    case UFUAModel.DataType.Float:
                        dataOut = BitConverter.GetBytes(Convert.ToSingle(dataRead));
                        break;
                    case UFUAModel.DataType.Double:
                        dataOut = BitConverter.GetBytes(Convert.ToDouble(dataRead));
                        break;
                    default:
                        ret = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorConvertingInOutData;
                        break;
                }
            }
            catch (Exception ex)
            {
                ret = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorConvertingInOutData;
            }
            return ret;
        }

        protected DriverErrorCodes ConvertToMoviconDataType(short dataRead, out byte[] dataOut)
        {
            DriverErrorCodes ret = DriverErrorCodes.ErrorNoError;

            dataOut = null;

            try
            {
                switch (Main.VarType)
                {
                    case UFUAModel.DataType.Boolean:
                        dataOut = BitConverter.GetBytes(Convert.ToBoolean(dataRead));
                        break;
                    case UFUAModel.DataType.SByte:
                        dataOut = BitConverter.GetBytes(Convert.ToSByte(dataRead));
                        break;
                    case UFUAModel.DataType.Byte:
                        dataOut = BitConverter.GetBytes(Convert.ToByte(dataRead));
                        break;
                    case UFUAModel.DataType.Int16:
                        dataOut = BitConverter.GetBytes(Convert.ToInt16(dataRead));
                        break;
                    case UFUAModel.DataType.UInt16:
                        dataOut = BitConverter.GetBytes(Convert.ToUInt16(dataRead));
                        break;
                    case UFUAModel.DataType.Int32:
                        dataOut = BitConverter.GetBytes(Convert.ToInt32(dataRead));
                        break;
                    case UFUAModel.DataType.UInt32:
                        dataOut = BitConverter.GetBytes(Convert.ToUInt32(dataRead));
                        break;
                    case UFUAModel.DataType.Int64:
                        dataOut = BitConverter.GetBytes(Convert.ToInt64(dataRead));
                        break;
                    case UFUAModel.DataType.UInt64:
                        dataOut = BitConverter.GetBytes(Convert.ToUInt64(dataRead));
                        break;
                    case UFUAModel.DataType.Float:
                        dataOut = BitConverter.GetBytes(Convert.ToSingle(dataRead));
                        break;
                    case UFUAModel.DataType.Double:
                        dataOut = BitConverter.GetBytes(Convert.ToDouble(dataRead));
                        break;
                    default:
                        ret = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorConvertingInOutData;
                        break;
                }
            }
            catch (Exception ex)
            {
                ret = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorConvertingInOutData;
            }
            return ret;
        }

        protected DriverErrorCodes ConvertToMoviconDataType(int dataRead, out byte[] dataOut)
        {
            DriverErrorCodes ret = DriverErrorCodes.ErrorNoError;

            dataOut = null;

            try
            {
                switch (Main.VarType)
                {
                    case UFUAModel.DataType.Boolean:
                        dataOut = BitConverter.GetBytes(Convert.ToBoolean(dataRead));
                        break;
                    case UFUAModel.DataType.SByte:
                        dataOut = BitConverter.GetBytes(Convert.ToSByte(dataRead));
                        break;
                    case UFUAModel.DataType.Byte:
                        dataOut = BitConverter.GetBytes(Convert.ToByte(dataRead));
                        break;
                    case UFUAModel.DataType.Int16:
                        dataOut = BitConverter.GetBytes(Convert.ToInt16(dataRead));
                        break;
                    case UFUAModel.DataType.UInt16:
                        dataOut = BitConverter.GetBytes(Convert.ToUInt16(dataRead));
                        break;
                    case UFUAModel.DataType.Int32:
                        dataOut = BitConverter.GetBytes(Convert.ToInt32(dataRead));
                        break;
                    case UFUAModel.DataType.UInt32:
                        dataOut = BitConverter.GetBytes(Convert.ToUInt32(dataRead));
                        break;
                    case UFUAModel.DataType.Int64:
                        dataOut = BitConverter.GetBytes(Convert.ToInt64(dataRead));
                        break;
                    case UFUAModel.DataType.UInt64:
                        dataOut = BitConverter.GetBytes(Convert.ToUInt64(dataRead));
                        break;
                    case UFUAModel.DataType.Float:
                        dataOut = BitConverter.GetBytes(Convert.ToSingle(dataRead));
                        break;
                    case UFUAModel.DataType.Double:
                        dataOut = BitConverter.GetBytes(Convert.ToDouble(dataRead));
                        break;
                    default:
                        ret = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorConvertingInOutData;
                        break;
                }
            }
            catch (Exception ex)
            {
                ret = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorConvertingInOutData;
            }
            return ret;
        }

        protected DriverErrorCodes ConvertToMoviconDataType(Double dataRead, out byte[] dataOut)
        {
            DriverErrorCodes ret = DriverErrorCodes.ErrorNoError;

            dataOut = null;

            try
            {
                switch (Main.VarType)
                {
                    case UFUAModel.DataType.Boolean:
                        dataOut = BitConverter.GetBytes(Convert.ToBoolean(dataRead));
                        break;
                    case UFUAModel.DataType.SByte:
                        dataOut = BitConverter.GetBytes(Convert.ToSByte(dataRead));
                        break;
                    case UFUAModel.DataType.Byte:
                        dataOut = BitConverter.GetBytes(Convert.ToByte(dataRead));
                        break;
                    case UFUAModel.DataType.Int16:
                        dataOut = BitConverter.GetBytes(Convert.ToInt16(dataRead));
                        break;
                    case UFUAModel.DataType.UInt16:
                        dataOut = BitConverter.GetBytes(Convert.ToUInt16(dataRead));
                        break;
                    case UFUAModel.DataType.Int32:
                        dataOut = BitConverter.GetBytes(Convert.ToInt32(dataRead));
                        break;
                    case UFUAModel.DataType.UInt32:
                        dataOut = BitConverter.GetBytes(Convert.ToUInt32(dataRead));
                        break;
                    case UFUAModel.DataType.Int64:
                        dataOut = BitConverter.GetBytes(Convert.ToInt64(dataRead));
                        break;
                    case UFUAModel.DataType.UInt64:
                        dataOut = BitConverter.GetBytes(Convert.ToUInt64(dataRead));
                        break;
                    case UFUAModel.DataType.Float:
                        dataOut = BitConverter.GetBytes(Convert.ToSingle(dataRead));
                        break;
                    case UFUAModel.DataType.Double:
                        dataOut = BitConverter.GetBytes(Convert.ToDouble(dataRead));
                        break;
                    default:
                        ret = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorConvertingInOutData;
                        break;
                }
            }
            catch (Exception ex)
            {
                ret = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorConvertingInOutData;
            }
            return ret;
        }

        public virtual void OffsetVariableValueChanged(int newVaue, List<Tag> tagList)
        {

        }

        public bool IsStructType()
        {
            return (Main != null && (int)Main.VarType == -1);
        }

        public static bool IsStructType(UFUAModel.DataType varType)
        {
            return ((int)varType == -1);
        }

        public UFUAModel.DataType GetStructType()
        {
            return (UFUAModel.DataType)(-1);
        }

        #region PartName
        public string GetPartByName(string partName)
        {
            if (String.IsNullOrEmpty(partName))
                throw new ArgumentNullException("Argument cannot be null or empty");

            if (_Parameters != null && _Parameters.ContainsKey(partName))
                return _Parameters[partName];

            return String.Empty;
        }

        public bool CreateFloatingPoint(int valuePart, int decimalPart, out Single resultValue)
        {
            resultValue = 0;

            try
            {
                resultValue = (Single)valuePart / (Single)Math.Pow(10, decimalPart);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public bool CreateFloatingPoint(int valuePart, int decimalPart, out double resultValue)
        {
            resultValue = 0;

            try
            {
                resultValue = (double)valuePart / Math.Pow(10, decimalPart);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public bool SplitFloatingPointInto(Single value, out int valuePart, out int decimalPart)
        {
            valuePart = 0;
            decimalPart = 0;

            string stringValue = value.ToString(CultureInfo.InvariantCulture);
            var values = stringValue.ToString(CultureInfo.InvariantCulture).Split(Convert.ToChar(CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator));
            try
            {
                valuePart = int.Parse(values[0]);
                if (values.Length>1)
                    decimalPart = int.Parse(values[1]);                
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public bool SplitFloatingPointInto(double value, out int valuePart, out int decimalPart)
        {
            valuePart = 0;
            decimalPart = 0;

            string stringValue = value.ToString(CultureInfo.InvariantCulture);
            var values = stringValue.ToString(CultureInfo.InvariantCulture).Split(Convert.ToChar(CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator));
            try
            {
                valuePart = int.Parse(values[0]);
                if (values.Length > 1)
                    decimalPart = int.Parse(values[1]);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets part by name. </summary>
        ///
        /// <param name="partName" type="string">   Name of the part. </param>
        /// <param name="defvalue" type="Boolean">  . </param>
        ///
        /// <returns>   The part by name. </returns>
        ///
        /// ### <param name="defvalue" type="Boolean">    . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public Boolean GetPartByName(string partName, Boolean defvalue)
        {
            Boolean value = defvalue;
            if (!Boolean.TryParse(GetPartByName(partName), out value))
                return defvalue;

            return value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets part by name. </summary>
        ///
        /// <param name="partName" type="string">   Name of the part. </param>
        /// <param name="defvalue" type="Byte">    . </param>
        ///
        /// <returns>   The part by name. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public Byte GetPartByName(string partName, Byte defvalue)
        {
            Byte value = defvalue;
            if (!Byte.TryParse(GetPartByName(partName), out value))
                return defvalue;

            return value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets part by name. </summary>
        ///
        /// <param name="partName" type="string">   Name of the part. </param>
        /// <param name="defvalue" type="Int16">    . </param>
        ///
        /// <returns>   The part by name. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public Int16 GetPartByName(string partName, Int16 defvalue)
        {
            Int16 value = defvalue;
            if (!Int16.TryParse(GetPartByName(partName), out value))
                return defvalue;

            return value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets part by name. </summary>
        ///
        /// <param name="partName" type="string">   Name of the part. </param>
        /// <param name="defvalue" type="Int32">    . </param>
        ///
        /// <returns>   The part by name. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public Int32 GetPartByName(string partName, Int32 defvalue)
        {
            Int32 value = defvalue;
            if (!Int32.TryParse(GetPartByName(partName), out value))
                return defvalue;

            return value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets part by name. </summary>
        ///
        /// <param name="partName" type="string">   Name of the part. </param>
        /// <param name="defvalue" type="UInt16">   . </param>
        ///
        /// <returns>   The part by name. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public UInt16 GetPartByName(string partName, UInt16 defvalue)
        {
            UInt16 value = defvalue;
            if (!UInt16.TryParse(GetPartByName(partName), out value))
                return defvalue;

            return value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets part by name. </summary>
        ///
        /// <param name="partName" type="string">   Name of the part. </param>
        /// <param name="defvalue" type="UInt32">   The defvalue. </param>
        ///
        /// <returns>   The part by name. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public UInt32 GetPartByName(string partName, UInt32 defvalue)
        {
            UInt32 value = defvalue;
            if (!UInt32.TryParse(GetPartByName(partName), out value))
                return defvalue;

            return value;
        }
        #endregion

        public override string ToString()
        {
            return CreateSettings();
        }

        #region Properties
        public string InvalidReason { get; set; }
        public bool IsValid { get; set; }
        protected string Settings { get; set; }        
        public bool HasSettings { 
            get { return (!string.IsNullOrEmpty(this.ToString())); }
        }
        public FanucCNCProtocol.MachineSeries MachineModel { get; set; }
        //public short CNCPath { get; set; }

        public uint StringLength { get; set; }

        public bool OffsetVariableSupported { get; set; }

        public bool CNCPathSupported { get; set; }
        #endregion

        #region IDataErrorInfo Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets an error message indicating what is wrong with this object. </summary>
        ///
        /// <value>
        /// An error message indicating what is wrong with this object. The default is an empty string
        /// ("").
        /// </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string Error
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
        /// <summary>   Indexer to get items within this collection using array index syntax. </summary>
        ///
        /// <param name="propertyName" type="string">   Name of the property. </param>
        ///
        /// <returns>   The indexed item. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string this[string propertyName]
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
                var propertyInfo = GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(this, null);

                    return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Raises the property changed event. </summary>
        ///
        /// <param name="e" type="PropertyChangedEventArgs">    Event information to send to registered
        ///                                                     event handlers. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        /// <summary>   Occurs when a property value changes. </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion
    }
}
