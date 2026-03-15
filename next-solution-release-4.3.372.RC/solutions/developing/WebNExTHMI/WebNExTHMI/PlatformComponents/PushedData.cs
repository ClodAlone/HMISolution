using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace WebNExTHMI.PlatformComponents
{
    public class PushedData
    {
        public PushedData()
        {
        }

        public PushedData(DataValue dv, int referenceid, BuiltInType? dataType)
        {
            id = referenceid;
            isGood = false;

            if (dv == null)
                return;

            value = String.Format("{0}", dv.WrappedValue);
            invariantValue = String.Format(CultureInfo.InvariantCulture, "{0}", dv.WrappedValue);
            statusCode = String.Format("{0}", dv.StatusCode);
            sourceTimestamp = String.Format("{0}", dv.SourceTimestamp);
            sourcePicoseconds = dv.SourcePicoseconds;
            serverTimestamp = String.Format("{0}", dv.ServerTimestamp);
            serverPicoseconds = dv.ServerPicoseconds;
            bIsArray = dv.Value is Array;
            bIsInteger = false;
            bIsNumeric = false;
            bIsBoolean = false;
            bIsString = false;
            if (dv.WrappedValue.TypeInfo != null)
            {
                dataType = dataType ?? dv.WrappedValue.TypeInfo.BuiltInType;
                bIsInteger = dataType >= BuiltInType.SByte && dataType <= BuiltInType.UInt64;
                bIsNumeric = dataType != null && TypeInfo.IsNumericType((BuiltInType)dataType);
                bIsBoolean = dataType == BuiltInType.Boolean;
                bIsString = dataType == BuiltInType.String;
            }
            isGood = Opc.Ua.StatusCode.IsGood(dv.StatusCode) || dv.StatusCode == Opc.Ua.StatusCodes.UncertainLastUsableValue;
            if (isGood)
                lastMessage = String.Empty;
            else
                lastMessage = String.Format("{0}", dv.StatusCode); 
        }

        public String value;
        public String invariantValue;
        public String statusCode;
        public String sourceTimestamp;
        public ushort sourcePicoseconds;
        public String serverTimestamp;
        public ushort serverPicoseconds;

        public int id;
        public bool isGood;
        public String lastMessage;

        public bool isUserReadable;
        public bool isReadable;
        public bool isUserWritable;
        public bool isWritable;

        public double rangeLow;
        public double rangeHigh;
        public String displayName;
        public String description;
        public List<String> enumStrings;
        public bool bIsInteger;
        public bool bIsNumeric;
        public bool bIsBoolean;
        public bool bIsString;
        public bool bIsArray;
        public string digitalTrueValue;
        public string digitalFalseValue;
        public BuiltInType? originalType;
    }
}
