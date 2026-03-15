////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	IEC60870_5_104Protocol.cs
//
// summary:	Implements the driver IEC60870_5_104 protocol class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;

namespace IEC60870_5_104
{
    public class ReceiveItem : ICloneable
    {
        public APDU Apdu;
        public bool isValid { set; get; }
        public IEC60870_5_104ErrorCodes Error { set; get; }
        public ReceiveItem()
        {
            Error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorNoError;
            Apdu = new APDU();
            isValid = false;
        }

        public void Reset()
        {
            Error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorNoError;
            Apdu = new APDU();
            isValid = false;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    interface IPackable
    {
        byte[] Pack();
    }
    interface IExpandable
    {
        void Expand(ref byte[] DataArray, ref int Offset);
    }
    public abstract class infObj : Object, IPackable, IExpandable
    {
        public abstract void initJobData(IEC60870_5_104CommJob job);
        public abstract byte[] Pack();
        public abstract void Expand(ref byte[] DataArray, ref int Offset);
        public abstract ASDUTypes Type();
        public abstract ASDUTypes basicType();
        public abstract byte[] getValue() ;
        public abstract void setValue(byte[] Value);
        public abstract DateTime getTimestamp();
        public abstract byte getQuality();
        public abstract void setQuality(byte qos);
        public abstract void setSelect(bool select);
    }

    /// <summary>   Error codes of IEC60870_5_104. </summary>
    public enum IEC60870_5_104ErrorCodes : int
    {
        ErrorNoError = 0,
        ErrorTxWrite = 1000,
        unknown_type_identification,
        unknown_cause_of_transmission,
        unknown_common_address_of_ASDU,
        unknown_information_object_address,
        ErrorRxRead,
        ErrorConnectionBroken,
        ErrorWriteException,
        cause_of_transmission_pn,
        ErrorFileTransferException,
        ErrorFileTransferMalformedPacket,
        ErrorFileTransferFileReadyNegativeConfirm,
        ErrorFileTransferSectionReadyNegativeConfirm,
        ErrorFileTransferChecksum,
        ErrorFileTransferSave,
        ErrorDirectoryException,
        ErrorReadingDirectoryContents,
        ErrorDirectoryMalformedPacket
    }

    /// <summary>  part of ASDUs defined in IEC 60870-5-101. </summary>
    public enum ASDUSelectableTypes : byte
    {
        //NotDefined = 0,
        SinglePoint = ASDUTypes.M_SP_NA_1,						//may be input or output job, requires CQ
        DoublePoint = ASDUTypes.M_DP_NA_1,						//may be input or output job, requires CQ
        StepPosition = ASDUTypes.M_ST_NA_1,						//may be input job
        BitString = ASDUTypes.M_BO_NA_1,							//may be input or output job
        NormalizedMeasurand = ASDUTypes.M_ME_NA_1,				//may be input or output job
        ScaledMeasurand = ASDUTypes.M_ME_NB_1,					//may be input or output job
        FloatingPointMeasurand = ASDUTypes.M_ME_NC_1,				//may be input or output job
        IntegratedTotals = ASDUTypes.M_IT_NA_1,					//may be input job
        ParamNormalizedMeasurand = ASDUTypes.P_ME_NA_1,			//may be output job, requires PQ
        ParamScaledMeasurand = ASDUTypes.P_ME_NB_1,				//may be output job, requires PQ
        ParamFloatingPointMeasurand = ASDUTypes.P_ME_NC_1,		//may be output job, requires PQ
        RegulatingStep = ASDUTypes.C_RC_NA_1,					//may be output job, requires CQ and CA
        ClockSynchronization = ASDUTypes.C_CS_NA_1,					//may be output job
        GeneralInterrogation = ASDUTypes.C_IC_NA_1,					//may be output job
        UploadFile = ASDUTypes.F_SC_NA_1,                        // Output job, even if the file is read
        ReadDirectoryContents = ASDUTypes.F_DR_TA_1              // Input job
    }
    public enum CommandQualifiers : byte
    {
        NotUsedCQ,
        ShortPulse,
        LongPulse,
        PersistentOutput,
    }
    public enum CommandType : UInt16
    {        
        Operate = 0,
        SelectExecuteValue = 1,
        Select = 2,
        DeactivateSelect = 3,
        DeactivateOperate = 4
    }
    public enum SelectBeforeOperateStep : UInt16
    {
        Select,
        Operate
    }
    public enum ParamQualifiers : byte
    {
        NotUsedPQ,
        ThresholdValue,
        SmoothingFactor,
        LowLimit,
        HighLimit,
    }
    public enum CommandActions : byte
    {
        NotUsedCA,
        StepDown,
        StepUp,
        OutputValue,
    }
    public enum Control : byte
    {
        SUPERVISORY = 0x01,
        STARTDTACT = 0x07,
        STARTDTCON = 0x0B,
        STOPDTACT = 0x13,
        STOPDTCON = 0x23,
        TESTFRACT = 0x43,
        TESTFRCON = 0x83,
        INTERROGATION = 0x64,
        START = 0x68,
        RESET = 0x69,
    }
    public enum CausesOfTrasmission : byte
    {
        invalid = 0,
        periodic_cyclic = 1,
        background_scan,
        spontaneous,
        initialized,
        request_or_requested,
        activation,
        activation_confirmation,
        deactivation,
        deactivation_confirmation,
        activation_termination,
        return_information_caused_by_a_remote_command,
        return_information_caused_by_a_local_command,
        file_transfer,
        interrogated_by_station_interrogation = 20,
        interrogated_by_group_1_interrogation,
        interrogated_by_group_2_interrogation,
        interrogated_by_group_3_interrogation,
        interrogated_by_group_4_interrogation,
        interrogated_by_group_5_interrogation,
        interrogated_by_group_6_interrogation,
        interrogated_by_group_7_interrogation,
        interrogated_by_group_8_interrogation,
        interrogated_by_group_9_interrogation,
        interrogated_by_group_10_interrogation,
        interrogated_by_group_11_interrogation,
        interrogated_by_group_12_interrogation,
        interrogated_by_group_13_interrogation,
        interrogated_by_group_14_interrogation,
        interrogated_by_group_15_interrogation,
        interrogated_by_group_16_interrogation,
        requested_by_general_counter_request,
        requested_by_group_1_counter_request,
        requested_by_group_2_counter_request,
        requested_by_group_3_counter_request,
        requested_by_group_4_counter_request,
        unknown_type_identification = 44,
        unknown_cause_of_transmission,
        unknown_common_address_of_ASDU,
        unknown_information_object_address,
    }
    public enum ASDUTypes : byte
    {
        invalid = 0,    
        M_SP_NA_1 = 1, // single-point information   
        M_SP_TA_1 = 2, // single-point information with time tag  
        M_DP_NA_1 = 3, // double-point information 
        M_DP_TA_1 = 4, // double-point information with time tag 
        M_ST_NA_1 = 5, // step position information   
        M_ST_TA_1 = 6, // step position information with time tag   
        M_BO_NA_1 = 7, // bitstring of 32 bits    
        M_BO_TA_1 = 8, // bitstring of 32 bits with time tag  
        M_ME_NA_1 = 9, // normalized value     
        M_ME_TA_1 = 10, // normalized value with time tag    
        M_ME_NB_1 = 11, // scaled value        
        M_ME_TB_1 = 12, // scaled value with time tag       
        M_ME_NC_1 = 13, // floating point        
        M_ME_TC_1 = 14, // floating point with time tag       
        M_IT_NA_1 = 15, // integrated totals       
        M_IT_NC_1 = 16, // integrated totals with time tag     
        M_SP_TB_1 = 30, // single-point information with CP56Time2a time tag        
        M_DP_TB_1 = 31, // double-point information with CP56Time2a time tag        
        M_ST_TB_1 = 32, // step position information with CP56Time2a time tag 
        M_BO_TB_1 = 33, // bitstring of 32 bits with CP56Time2a time tag 
        M_ME_TD_1 = 34, // normalized value CP56Time2a with time tag   
        M_ME_TE_1 = 35, // scaled value with CP56Time2a time tag  
        M_ME_TF_1 = 36, // floating point with CP56Time2a time tag
        M_IT_TB_1 = 37, // integrated totals with CP56Time2a time tag  
        M_EP_TD_1 = 38,
        M_EP_TE_1 = 39,
        M_EP_TF_1 = 40,   

        C_SC_NA_1 = 45, // single command 
        C_DC_NA_1 = 46, // double command 
        C_RC_NA_1 = 47, // regulating step command 
        C_SE_NA_1 = 48, // set point command, normalized value
        C_SE_NB_1 = 49, // set point command, scaled value
        C_SE_NC_1 = 50, // set point command, short floating point number

        C_BO_NA_1 = 51, // bitstring of 32 bits,
        C_SC_TA_1 = 58, // single command with time tag
        C_DC_TA_1 = 59, // double command with time tag 
        C_RC_TA_1 = 60, // regulating step command with time tag 
        C_SE_TA_1 = 61,
        C_SE_TB_1 = 62,
        C_SE_TC_1 = 63,
        C_BO_TA_1 = 64,   

        
        M_EI_NA_1 = 70, // end of initialization 
        C_IC_NA_1 = 100, // general interrogation (GI)  
        C_CI_NA_1 = 101, // counter interrogation  
        C_CS_NA_1 = 103, // clock synchronization command  
        C_RP_NA_1 = 105, // reset process command  
        C_TS_TA_1 = 107, // test command with time tag CP56Time2a 
        P_ME_NA_1 = 110, // parameter of measured value, normalized value
        P_ME_NB_1 = 111, // parameter of measured value, scaled value
        P_ME_NC_1 = 112, // parameter of measured value, short floating point number
        P_AC_NA_1 = 113, // parameter activation

        F_FR_NA_1 = 120, // File Transfer: File Ready
        F_SR_NA_1 = 121, // File Transfer: Section Ready
        F_SC_NA_1 = 122, // File Transfer: Call directory, select file, call file, call section
        F_LS_NA_1 = 123, // File Transfer: Last Section / Segment
        F_AF_NA_1 = 124, // File Transfer: Ack File / Section
        F_SG_NA_1 = 125, // File Transfer: Segment
        F_DR_TA_1 = 126 // File Transfer: Directory
    }
    public enum CFFormats
    {
        I,      // numbered information transfer
        S,      // numbered supervisory functions
        U = 3,  // unnumbered control functions
    }

    public enum QCCRequests : byte
    {
        noCounterRequested,
        requestCounteGroup1,
        requestCounteGroup2,
        requestCounteGroup3,
        requestCounteGroup4,
        generalRequestCounter,
    }
    public enum QCCFreezes : byte
    {
        read,
        counterFreezeWithoutReset,
        counterFreezeWithReset,
        counterReset,
    }

    public enum SelectAndCallQualifier : byte
    {
        defaultValue = 0,
        selectFile = 1,
        requestFile = 2,
        deactivateFile = 3,
        deleteFile = 4,
        selectSection = 5,
        requestSection = 6,
        deactivateSection = 7
    }

    public enum LastSectionSegmentQualifier : byte
    {
        notUsed = 0,
        fileTransferWithoutDeactivation = 1,
        fileTransferWithDeactivation = 2,
        sectionTransferWithoutDeactivation = 3,
        sectionTransferWithDeactivation = 4
    }

    public enum SegmentType : byte
    {
        notUsed,
        dataSegment,
        lastSegment,
        lastSection
    }

    public enum AcknowledgeFileQualifier : byte
    {
        notUsed = 0,
        positiveAcknowledgeOfFileTransfer = 1,
        negativeAcknowledgeOfFileTransfer = 2,
        positiveAcknowledgeOfSectionTransfer = 3,
        negativeAcknowledgeOfSectionTransfer = 4
    }

    public static class BufferExpand
    {
        public static byte toByte(ref byte[] DataArray, ref int Offset)
        {
            checkSize(DataArray, Offset, 1);
            byte outval = DataArray[Offset];
            Offset += 1;
            return outval;
        }
        public static Int16 toInt16(byte[] DataArray)
        {
            int Offset = 0;
            return toInt16(ref DataArray, ref Offset);

        }
        public static Int16 toInt16(ref byte[] DataArray, ref int Offset)
        {
            checkSize(DataArray, Offset, 2);
            Int16 outval = BitConverter.ToInt16(DataArray, Offset);
            Offset += 2;
            return outval;
        }
        public static UInt16 toUInt16(ref byte[] DataArray, ref int Offset)
        {
            checkSize(DataArray, Offset, 2);
            UInt16 outval = BitConverter.ToUInt16(DataArray, Offset);
            Offset += 2;
            return outval;
        }

        public static Int32 toInt32(byte[] DataArray)
        {
            int Offset = 0;
            return toInt32(ref DataArray, ref Offset);

        }
        public static Int32 toInt32(ref byte[] DataArray, ref int Offset)
        {
            checkSize(DataArray, Offset, 4);
            Int32 outval = BitConverter.ToInt32(DataArray, Offset);
            Offset += 4;
            return outval;
        }
        public static UInt32 toUInt32(byte[] DataArray)
        {
            UInt32 rt = 0;
            for (int i = 0; i < DataArray.Length && i < 4; i++)
                rt += (UInt32)(DataArray[i] << (i * 8));
            return rt;

        }

        public static float toFloat(ref byte[] DataArray, ref int Offset)
        {
            checkSize(DataArray, Offset, 4);
            float outval = BitConverter.ToSingle(DataArray, Offset);
            Offset += 4;
            return outval;
        }

        public static float toFloat(byte[] DataArray)
        {
            checkSize(DataArray, 0, 4);
            float outval = BitConverter.ToSingle(DataArray, 0);
            return outval;
        }
        public static double toDouble(byte[] DataArray)
        {
            checkSize(DataArray, 0, 8);
            double outval = BitConverter.ToDouble(DataArray, 0);
            return outval;
        }
        public static string toString(byte[] DataArray)
        {
            return Encoding.UTF8.GetString(DataArray, 1, DataArray.Length - 1); ;
        }
        public static byte[] toArray(ref byte[] DataArray, ref int Offset, UInt32 lenght)
        {
            checkSize(DataArray, Offset, lenght);
            byte[] outBuffer = new byte[lenght];
            if (lenght != 0)
                for (UInt16 i = 0; i < lenght; i++)
                    outBuffer[i] = DataArray[Offset++];
            return outBuffer;
        }
        public static byte[] toArray(Int16 inValue)
        {
            return BitConverter.GetBytes(inValue);
        }
        public static byte[] toArray(UInt16 inValue)
        {
            return BitConverter.GetBytes(inValue);
        }
        public static byte[] toArray(Int32 inValue)
        {
            return BitConverter.GetBytes(inValue);
        }
        public static byte[] toArray(UInt32 inValue)
        {
            return BitConverter.GetBytes(inValue);
        }
        public static byte[] toArray(float value)
        {
            return BitConverter.GetBytes(value);
        }
         public static byte[] swapArray(byte[] inBuffer)
        {
            byte[] outBuffer = new byte[inBuffer.Length];
            if (inBuffer.Length != 0)
                for (UInt16 i = 0; i < inBuffer.Length; i++)
                    outBuffer[i] = inBuffer[inBuffer.Length - 1 - i];
            return outBuffer;
        }

        public static void checkSize(byte[] DataArray, int Offset, UInt32 lenght)
        {
            if (Offset + lenght > DataArray.Count())
                throw new NotImplementedException();
        }
    }

    // CP24Time2a timestamp
    public class CP24Time2a : IPackable, IExpandable
    {
        public CP24Time2a()
        {
            init(DateTime.Now.TimeOfDay);
        }
        public CP24Time2a(TimeSpan Time)
        {
            init(Time);
        }

        #region members
        private ushort _msec;
        private byte _byte1;
        #endregion

        #region Properties
        public TimeSpan Value
        {
            get
            {
                return new TimeSpan(0,0,min, msec / 1000, msec % 1000);
            }
            set { init(value); }
        }
        public ushort msec { get { return _msec; } set { _msec = value; } }
        public byte min
        {
            get { return (byte)(_byte1 & 0x3f); }
            set { _byte1 = (byte)((_byte1 & 0xc0) | (value & 0x3f)); }
        }
        public bool res1
        {
            get
            {
                return (_byte1 & (1 << 6)) == (1 << 6);
            }
            set
            {
                _byte1 = (byte)((Convert.ToByte(value) << 6) + (_byte1 & ((1 << 6) ^ 0xFF)));
            }
        }
        public bool iv
        {
            get
            {
                return (_byte1 & (1 << 7)) == (1 << 7);
            }
            set
            {
                _byte1 = (byte)((Convert.ToByte(value) << 7) + (_byte1 & ((1 << 7) ^ 0xFF)));
            }
        }
        #endregion

        #region Methods
        private void init(TimeSpan Time)
        {
            min = (byte)Time.Minutes;
            res1 = false;
            iv = false;
            msec = (ushort)(Time.Milliseconds + Time.Seconds * 1000);

        }

        public byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(BitConverter.GetBytes(_msec));
            retVal.Add(_byte1);
            return retVal.ToArray();
        }

        public void Expand(ref byte[] DataArray, ref int Offset)
        {
            _msec = (ushort)(BufferExpand.toUInt16(ref DataArray, ref Offset));
            _byte1 = BufferExpand.toByte(ref DataArray, ref Offset);
        }
        #endregion
    }
    // CP56Time2a timestamp
    public class CP56Time2a : IPackable, IExpandable
    {
        public CP56Time2a()
        {
            init(DateTime.Now);
        }
        public CP56Time2a(DateTime DateTime)
        {
            init(DateTime);
        }

        #region members
        private ushort _msec;
        private byte _byte1;
        private byte _byte2;
        private byte _byte3;
        private byte _byte4;
        private byte _byte5;
        #endregion

        #region Properties
        public DateTime Value
        {
            get 
            {
                return new DateTime(year + 2000, month, mday, hour, min, msec / 1000, msec % 1000);
            }
            set { init(value); }
        }
        public ushort msec { get { return _msec; } set { _msec = value; } }
        public byte min
        {
            get { return (byte)(_byte1 & 0x3f); }
            set { _byte1 = (byte)((_byte1 & 0xc0) | (value & 0x3f)); }
        }
        public bool res1
        {
            get
            {
                return (_byte1 & (1 << 6)) == (1 << 6);
            }
            set
            {
                _byte1 = (byte)((Convert.ToByte(value) << 6) + (_byte1 & ((1 << 6) ^ 0xFF)));
            }
        }
        public bool iv
        {
            get
            {
                return (_byte1 & (1 << 7)) == (1 << 7);
            }
            set
            {
                _byte1 = (byte)((Convert.ToByte(value) << 7) + (_byte1 & ((1 << 7) ^ 0xFF)));
            }
        }
        public byte hour
        {
            get { return (byte)(_byte2 & 0x1f); }
            set { _byte2 = (byte)((_byte2 & 0xe0) | (value & 0x1f)); }
        }
        public bool su
        {
            get
            {
                return (_byte2 & (1 << 7)) == (1 << 7);
            }
            set
            {
                _byte2 = (byte)((Convert.ToByte(value) << 7) + (_byte2 & ((1 << 7) ^ 0xFF)));
            }
        }
        public byte mday
        {
            get { return (byte)(_byte3 & 0x1f); }
            set { _byte3 = (byte)((_byte3 & 0xe0) | (value & 0x1f)); }
        }
        public byte wday
        {
            get { return (byte)((_byte3 & 0xe0) >> 5); }
            set { _byte3 = (byte)((_byte3 & 0x1f) | ((value << 5) & 0xe0)); }
        }
        public byte month
        {
            get { return (byte)(_byte4 & 0x0f); }
            set { _byte4 = (byte)((_byte4 & 0xf0) | (value & 0x0f)); }
        }

        public byte year
        {
            get { return (byte)(_byte5 & 0x7f); }
            set { _byte5 = (byte)((_byte5 & 0x80) | (value & 0x7f)); }
        }
        public bool res4
        {
            get
            {
                return (_byte5 & (1 << 7)) == (1 << 7);
            }
            set
            {
                _byte5 = (byte)((Convert.ToByte(value) << 7) + (_byte5 & ((1 << 7) ^ 0xFF)));
            }
        }
        #endregion

        #region Methods
        private void init(DateTime DateTime)
        {
            min = (byte)DateTime.Minute;
            res1 = false;
            iv = false;
            su = false;
            hour = (byte)DateTime.Hour;
            mday = (byte)DateTime.Day;
            wday = (byte)(((byte)DateTime.DayOfWeek + 6) % 7 + 1);
            month = (byte)DateTime.Month;
            year = (byte)(DateTime.Year - 2000);
            msec = (ushort)(DateTime.Millisecond + DateTime.Second * 1000);

        }

        public byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(BitConverter.GetBytes(_msec));
            retVal.Add(_byte1);
            retVal.Add(_byte2);
            retVal.Add(_byte3);
            retVal.Add(_byte4);
            retVal.Add(_byte5);
            return retVal.ToArray();
        }

        public void Expand(ref byte[] DataArray, ref int Offset)
        {
            _msec = (ushort)(BufferExpand.toUInt16(ref DataArray, ref Offset));
            _byte1 = BufferExpand.toByte(ref DataArray, ref Offset);
            _byte2 = BufferExpand.toByte(ref DataArray, ref Offset);
            _byte3 = BufferExpand.toByte(ref DataArray, ref Offset);
            _byte4 = BufferExpand.toByte(ref DataArray, ref Offset);
            _byte5 = BufferExpand.toByte(ref DataArray, ref Offset);
        }
        #endregion
    }

    public class C_CS_NA_1_infObj : infObj
    {
        public C_CS_NA_1_infObj()
        {
            _timestamp = new CP56Time2a(DateTime.Now);
        }

        #region members
        CP56Time2a _timestamp;
        #endregion

        #region Properties
        public CP56Time2a Timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }
        #endregion

        #region Methods
        public override ASDUTypes Type()
        {
            return ASDUTypes.C_CS_NA_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.C_CS_NA_1;
        }
        public override byte[] getValue()
        {
            return _timestamp.Pack();
        }
        public override void setValue(byte[] DataArray)
        {
            int Offset = 0;
            _timestamp.Expand(ref DataArray, ref Offset);
        }
        public override DateTime getTimestamp()
        {
            return _timestamp.Value;
        }
        public override byte getQuality()
        {
            return 0;
        }

        public override void setQuality(byte qos)
        {
        }
        public override void setSelect(bool select)
        {

        }
        public override byte[] Pack()
        {
            return _timestamp.Pack();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _timestamp.Expand(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            _timestamp = new CP56Time2a(DateTime.Now);
        }
        #endregion
    }
    public class C_IC_NA_1_infObj : infObj
    {
        public C_IC_NA_1_infObj()
        {
            _QOI = (byte)CausesOfTrasmission.interrogated_by_station_interrogation;
        }
        public C_IC_NA_1_infObj(CausesOfTrasmission QOI)
        {
            _QOI = (byte)QOI;
        }

        #region members
        byte _QOI;
        #endregion

        #region Properties
        public CausesOfTrasmission QOI
        {
            get { return (CausesOfTrasmission)_QOI; }
            set { _QOI = (byte)value ; }
        }

        #endregion

        #region Methods
        public override ASDUTypes Type()
        {
            return ASDUTypes.C_IC_NA_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.C_IC_NA_1;
        }

        public override byte[] getValue()
        {
            return new byte[0];
        }
        public override void setValue(byte[] DataArray)
        {
        }
        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }
        public override byte getQuality()
        {
            return 0;
        }
        public override void setQuality(byte qos)
        {
        }
        public override void setSelect(bool select)
        {

        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.Add(_QOI);
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _QOI = BufferExpand.toByte(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
        }
        #endregion
    }
    public class C_CI_NA_1_infObj : infObj
    {
        public C_CI_NA_1_infObj()
        {
            _QCC = 0;
        }
        public C_CI_NA_1_infObj(QCCRequests Request, QCCFreezes Freeze)
        {
            RQT = Request;
            FRZ = Freeze;
        }

        #region members
        byte _QCC;
        #endregion

        #region Properties
        public QCCRequests RQT
        {
            get { return (QCCRequests)(_QCC & 0x3f); }
            set { _QCC = (byte)((_QCC & 0xc0) | ((byte)value & 0x3f)); }
        }
        public QCCFreezes FRZ
        {
            get { return (QCCFreezes)((_QCC & 0xc0) >> 6); }
            set { _QCC = (byte)((_QCC & 0x3f) | (((byte)value << 6) & 0xc0)); }
        }

        #endregion

        #region Methods
        public override ASDUTypes Type()
        {
            return ASDUTypes.C_CI_NA_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.C_CI_NA_1;
        }

        public override byte[] getValue()
        {
            return new byte[0];
        }
        public override void setValue(byte[] DataArray)
        {
        }
        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }
        public override byte getQuality()
        {
            return 0;
        }
        public override void setQuality(byte qos)
        {
        }
        public override void setSelect(bool select)
        {

        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.Add(_QCC);
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _QCC = BufferExpand.toByte(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
        }
        #endregion
    }
    // M_SP_NA_1 - single point information with quality description
    public class M_SP_NA_1_infObj : infObj
    {
        public M_SP_NA_1_infObj()
        {
            Init();
        }

        #region members
        private byte _SIQ;
        #endregion

        #region Properties
        public bool SPI
        {
            get { return (_SIQ & 1) == 1; }
            set { _SIQ = (byte)(Convert.ToByte(value) + (_SIQ & 0xFE)); }
        } // single point information
        public byte RES
        {
            get { return (byte)(_SIQ & 0x0e); }
            set { _SIQ = (byte)((_SIQ & 0xf1) | (value & 0x0e)); }
        }
        public bool BL
        {
            get { return (_SIQ & (1 << 4)) == (1 << 4); }
            set { _SIQ = (byte)((Convert.ToByte(value) << 4) + (_SIQ & ((1 << 4) ^ 0xFF))); }
        }// blocked/not blocked
        public bool SB
        {
            get { return (_SIQ & (1 << 5)) == (1 << 5); }
            set { _SIQ = (byte)((Convert.ToByte(value) << 5) + (_SIQ & ((1 << 5) ^ 0xFF))); }
        }// substituted/not substituted
        public bool NT
        {
            get { return (_SIQ & (1 << 6)) == (1 << 6); }
            set { _SIQ = (byte)((Convert.ToByte(value) << 6) + (_SIQ & ((1 << 6) ^ 0xFF))); }
        }// not topical/topical
        public bool IV
        {
            get { return (_SIQ & (1 << 7)) == (1 << 7); }
            set { _SIQ = (byte)((Convert.ToByte(value) << 7) + (_SIQ & ((1 << 7) ^ 0xFF))); }
        }// valid/invalid
        public CommandQualifiers QU
        {
            set { _SIQ = (byte)((_SIQ & 0x83) | (((byte)value << 2) & 0x7c)); }
        }
        #endregion

        #region Methods
        public void Init()
        {
        }
        public override ASDUTypes Type()
        {
            return ASDUTypes.M_SP_NA_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.M_SP_NA_1;
        }
        public override byte[] getValue()
        {
            byte[]rt = new byte[1];
            rt[0] = Convert.ToByte(SPI);
            return rt;
        }
        public override void setValue(byte[] DataArray)
        {
            SPI = DataArray[0] != 0;
        }
        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }
        public override byte getQuality()
        {
            return (byte)(_SIQ & 0xfe);
        }
        public override void setQuality(byte qos)
        {
        }
        public override void setSelect(bool select)
        {
            if (select)
                _SIQ |= 0x80;
        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.Add(_SIQ);
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _SIQ = BufferExpand.toByte(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
            QU = job.CmdQualifier;
        }
        #endregion
    };
    // M_SP_TA_1 - single point information with quality description and CP24Time2a timestamp
    public class M_SP_TA_1_infObj : infObj
    {
        public M_SP_TA_1_infObj()
        {
            Init();
        }

        public M_SP_TA_1_infObj(TimeSpan timestamp)
        {
            _timestamp = new CP24Time2a(timestamp);
        }

        #region members
        private byte _SIQ;
        CP24Time2a _timestamp;
        #endregion

        #region Properties
        public TimeSpan Timestamp
        {
            get { return _timestamp.Value; }
            set { _timestamp.Value = value; }
        }
        public bool SPI
        {
            get { return (_SIQ & 1) == 1; }
            set { _SIQ = (byte)(Convert.ToByte(value) + (_SIQ & 0xFE)); }
        } // single point information
        public byte RES
        {
            get { return (byte)(_SIQ & 0x0e); }
            set { _SIQ = (byte)((_SIQ & 0xf1) | (value & 0x0e)); }
        }
        public bool BL
        {
            get { return (_SIQ & (1 << 4)) == (1 << 4); }
            set { _SIQ = (byte)((Convert.ToByte(value) << 4) + (_SIQ & ((1 << 4) ^ 0xFF))); }
        }// blocked/not blocked
        public bool SB
        {
            get { return (_SIQ & (1 << 5)) == (1 << 5); }
            set { _SIQ = (byte)((Convert.ToByte(value) << 5) + (_SIQ & ((1 << 5) ^ 0xFF))); }
        }// substituted/not substituted
        public bool NT
        {
            get { return (_SIQ & (1 << 6)) == (1 << 6); }
            set { _SIQ = (byte)((Convert.ToByte(value) << 6) + (_SIQ & ((1 << 6) ^ 0xFF))); }
        }// not topical/topical
        public bool IV
        {
            get { return (_SIQ & (1 << 7)) == (1 << 7); }
            set { _SIQ = (byte)((Convert.ToByte(value) << 7) + (_SIQ & ((1 << 7) ^ 0xFF))); }
        }// valid/invalid
        public CommandQualifiers QU
        {
            set { _SIQ = (byte)((_SIQ & 0x83) | (((byte)value << 2) & 0x7c)); }
        }
        #endregion

        #region Methods
        public void Init()
        {
            _timestamp = new CP24Time2a(DateTime.Now.TimeOfDay);
        }
        public override ASDUTypes Type()
        {
            return ASDUTypes.M_SP_TA_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.M_SP_NA_1;
        }
        public override byte[] getValue()
        {
            byte[] rt = new byte[1];
            rt[0] = Convert.ToByte(SPI);
            return rt;
        }
        public override void setValue(byte[] DataArray)
        {
            SPI = DataArray[0] != 0;
        }
        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }
        public override byte getQuality()
        {
            return (byte)(_SIQ & 0xfe);
        }
        public override void setQuality(byte qos)
        {
        }
        public override void setSelect(bool select)
        {
            if (select)
                _SIQ |= 0x80;
        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.Add(_SIQ);
            retVal.AddRange(_timestamp.Pack());
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _SIQ = BufferExpand.toByte(ref DataArray, ref Offset);
            _timestamp.Expand(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
            QU = job.CmdQualifier;
            _timestamp = new CP24Time2a(DateTime.Now.TimeOfDay);
        }
        #endregion
    };
    // M_SP_TB_1 - single point information with quality description and CP56Time2a timestamp
    public class M_SP_TB_1_infObj : infObj
    {
        public M_SP_TB_1_infObj()
        {
            Init();
        }

        public M_SP_TB_1_infObj(DateTime timestamp)
        {
            _timestamp = new CP56Time2a(timestamp);
        }

        #region members
        private byte _SIQ;
        CP56Time2a _timestamp;
        #endregion

        #region Properties
        public DateTime Timestamp
        {
            get { return _timestamp.Value; }
            set { _timestamp.Value = value; }
        }
        public bool SPI
        {
            get { return (_SIQ & 1) == 1; }
            set { _SIQ = (byte)(Convert.ToByte(value) + (_SIQ & 0xFE)); }
        } // single point information
        public byte RES
        {
            get { return (byte)(_SIQ & 0x0e); }
            set { _SIQ = (byte)((_SIQ & 0xf1) | (value & 0x0e)); }
        }
        public bool BL
        {
            get { return (_SIQ & (1 << 4)) == (1 << 4); }
            set { _SIQ = (byte)((Convert.ToByte(value) << 4) + (_SIQ & ((1 << 4) ^ 0xFF))); }
        }// blocked/not blocked
        public bool SB
        {
            get { return (_SIQ & (1 << 5)) == (1 << 5); }
            set { _SIQ = (byte)((Convert.ToByte(value) << 5) + (_SIQ & ((1 << 5) ^ 0xFF))); }
        }// substituted/not substituted
        public bool NT
        {
            get { return (_SIQ & (1 << 6)) == (1 << 6); }
            set { _SIQ = (byte)((Convert.ToByte(value) << 6) + (_SIQ & ((1 << 6) ^ 0xFF))); }
        }// not topical/topical
        public bool IV
        {
            get { return (_SIQ & (1 << 7)) == (1 << 7); }
            set { _SIQ = (byte)((Convert.ToByte(value) << 7) + (_SIQ & ((1 << 7) ^ 0xFF))); }
        }// valid/invalid
        public CommandQualifiers QU
        {
            set { _SIQ = (byte)((_SIQ & 0x83) | (((byte)value << 2) & 0x7c)); }
        }
        #endregion

        #region Methods
        public void Init()
        {
            _timestamp = new CP56Time2a(DateTime.Now);
        }
        public override ASDUTypes Type()
        {
            return ASDUTypes.M_SP_TB_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.M_SP_NA_1;
        }
        public override byte[] getValue()
        {
            byte[] rt = new byte[1];
            rt[0] = Convert.ToByte(SPI);
            return rt;
        }
        public override void setValue(byte[] DataArray)
        {
            SPI = DataArray[0] != 0;
        }
        public override DateTime getTimestamp()
        {
            return _timestamp.Value;
        }
        public override byte getQuality()
        {
            return (byte)(_SIQ & 0xfe);
        }
        public override void setQuality(byte qos)
        {
        }
        public override void setSelect(bool select)
        {
            if (select)
                _SIQ |= 0x80;
        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.Add(_SIQ);
            retVal.AddRange(_timestamp.Pack());
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _SIQ = BufferExpand.toByte(ref DataArray, ref Offset);
            _timestamp.Expand(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
            QU = job.CmdQualifier;
            _timestamp = new CP56Time2a(DateTime.Now);
        }
        #endregion
    };
    // M_DP_NA_1 - double point information with quality description
    public class M_DP_NA_1_infObj : infObj
    {
        public M_DP_NA_1_infObj()
        {
            Init();
        }

        #region members
        private byte _DIQ;
        #endregion

        #region Properties
        public byte DPI
        {
            get { return (byte)(_DIQ & 0x03); }
            set { _DIQ = (byte)((_DIQ & 0xfc) | (value & 0x03)); }
        } // double point information
        public byte RES
        {
            get { return (byte)(_DIQ & 0x0e); }
            set { _DIQ = (byte)((_DIQ & 0xf1) | (value & 0x0e)); }
        }
        public bool BL
        {
            get { return (_DIQ & (1 << 4)) == (1 << 4); }
            set { _DIQ = (byte)((Convert.ToByte(value) << 4) + (_DIQ & ((1 << 4) ^ 0xFF))); }
        }// blocked/not blocked
        public bool SB
        {
            get { return (_DIQ & (1 << 5)) == (1 << 5); }
            set { _DIQ = (byte)((Convert.ToByte(value) << 5) + (_DIQ & ((1 << 5) ^ 0xFF))); }
        }// substituted/not substituted
        public bool NT
        {
            get { return (_DIQ & (1 << 6)) == (1 << 6); }
            set { _DIQ = (byte)((Convert.ToByte(value) << 6) + (_DIQ & ((1 << 6) ^ 0xFF))); }
        }// not topical/topical
        public bool IV
        {
            get { return (_DIQ & (1 << 7)) == (1 << 7); }
            set { _DIQ = (byte)((Convert.ToByte(value) << 7) + (_DIQ & ((1 << 7) ^ 0xFF))); }
        }// valid/invalid
        public CommandQualifiers QU
        {
            set { _DIQ = (byte)((_DIQ & 0x83) | (((byte)value << 2) & 0x7c)); }
        }
        #endregion

        #region Methods
        public void Init()
        {
        }
        public override ASDUTypes Type()
        {
            return ASDUTypes.M_DP_NA_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.M_DP_NA_1;
        }
        public override byte[] getValue()
        {
            return BufferExpand.toArray(DPI);
        }
        public override void setValue(byte[] DataArray)
        {
            DPI = DataArray[0];
        }
        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }
        public override byte getQuality()
        {
            return (byte)(_DIQ & 0xfc);
        }
        public override void setQuality(byte qos)
        {
        }
        public override void setSelect(bool select)
        {
            if (select)
                _DIQ |= 0x80;
        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.Add(_DIQ);
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _DIQ = BufferExpand.toByte(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
            QU = job.CmdQualifier;
        }
        #endregion
    };
    // M_DP_TA_1 - double point information with quality description and CP24Time2a timestamp
    public class M_DP_TA_1_infObj : infObj
    {
        public M_DP_TA_1_infObj()
        {
            Init();
        }
        public M_DP_TA_1_infObj(TimeSpan timestamp)
        {
            _timestamp = new CP24Time2a(timestamp);
        }

        #region members
        private byte _DIQ;
        CP24Time2a _timestamp;
        #endregion

        #region Properties
        public TimeSpan Timestamp
        {
            get { return _timestamp.Value; }
            set { _timestamp.Value = value; }
        }
        public byte DPI
        {
            get { return (byte)(_DIQ & 0x03); }
            set { _DIQ = (byte)((_DIQ & 0xfc) | (value & 0x03)); }
        } // double point information
        public byte RES
        {
            get { return (byte)(_DIQ & 0x0e); }
            set { _DIQ = (byte)((_DIQ & 0xf1) | (value & 0x0e)); }
        }
        public bool BL
        {
            get { return (_DIQ & (1 << 4)) == (1 << 4); }
            set { _DIQ = (byte)((Convert.ToByte(value) << 4) + (_DIQ & ((1 << 4) ^ 0xFF))); }
        }// blocked/not blocked
        public bool SB
        {
            get { return (_DIQ & (1 << 5)) == (1 << 5); }
            set { _DIQ = (byte)((Convert.ToByte(value) << 5) + (_DIQ & ((1 << 5) ^ 0xFF))); }
        }// substituted/not substituted
        public bool NT
        {
            get { return (_DIQ & (1 << 6)) == (1 << 6); }
            set { _DIQ = (byte)((Convert.ToByte(value) << 6) + (_DIQ & ((1 << 6) ^ 0xFF))); }
        }// not topical/topical
        public bool IV
        {
            get { return (_DIQ & (1 << 7)) == (1 << 7); }
            set { _DIQ = (byte)((Convert.ToByte(value) << 7) + (_DIQ & ((1 << 7) ^ 0xFF))); }
        }// valid/invalid
        public CommandQualifiers QU
        {
            set { _DIQ = (byte)((_DIQ & 0x83) | (((byte)value << 2) & 0x7c)); }
        }
        #endregion

        #region Methods
        public void Init()
        {
            _timestamp = new CP24Time2a(DateTime.Now.TimeOfDay);
        }
        public override ASDUTypes Type()
        {
            return ASDUTypes.M_DP_TA_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.M_DP_NA_1;
        }
        public override byte[] getValue()
        {
            return BufferExpand.toArray(DPI);
        }
        public override void setValue(byte[] DataArray)
        {
            DPI = DataArray[0];
        }
        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }
        public override byte getQuality()
        {
            return (byte)(_DIQ & 0xfc);
        }
        public override void setQuality(byte qos)
        {
        }
        public override void setSelect(bool select)
        {
            if (select)
                _DIQ |= 0x80;
        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.Add(_DIQ);
            retVal.AddRange(_timestamp.Pack());
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _DIQ = BufferExpand.toByte(ref DataArray, ref Offset);
            _timestamp.Expand(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
            QU = job.CmdQualifier;
            _timestamp = new CP24Time2a(DateTime.Now.TimeOfDay);
        }
        #endregion
    };
    // M_DP_TB_1 - double point information with quality description and CP56Time2a timestamp
    public class M_DP_TB_1_infObj : infObj
    {
        public M_DP_TB_1_infObj()
        {
            Init();
        }
        public M_DP_TB_1_infObj(DateTime timestamp)
        {
            _timestamp = new CP56Time2a(timestamp);
        }

        #region members
        private byte _DIQ;
        CP56Time2a _timestamp;
        #endregion

        #region Properties
        public DateTime Timestamp
        {
            get { return _timestamp.Value; }
            set { _timestamp.Value = value; }
        }
        public byte DPI
        {
            get { return (byte)(_DIQ & 0x03); }
            set { _DIQ = (byte)((_DIQ & 0xfc) | (value & 0x03)); }
        } // double point information
        public byte RES
        {
            get { return (byte)(_DIQ & 0x0e); }
            set { _DIQ = (byte)((_DIQ & 0xf1) | (value & 0x0e)); }
        }
        public bool BL
        {
            get { return (_DIQ & (1 << 4)) == (1 << 4); }
            set { _DIQ = (byte)((Convert.ToByte(value) << 4) + (_DIQ & ((1 << 4) ^ 0xFF))); }
        }// blocked/not blocked
        public bool SB
        {
            get { return (_DIQ & (1 << 5)) == (1 << 5); }
            set { _DIQ = (byte)((Convert.ToByte(value) << 5) + (_DIQ & ((1 << 5) ^ 0xFF))); }
        }// substituted/not substituted
        public bool NT
        {
            get { return (_DIQ & (1 << 6)) == (1 << 6); }
            set { _DIQ = (byte)((Convert.ToByte(value) << 6) + (_DIQ & ((1 << 6) ^ 0xFF))); }
        }// not topical/topical
        public bool IV
        {
            get { return (_DIQ & (1 << 7)) == (1 << 7); }
            set { _DIQ = (byte)((Convert.ToByte(value) << 7) + (_DIQ & ((1 << 7) ^ 0xFF))); }
        }// valid/invalid
        public CommandQualifiers QU
        {
            set { _DIQ = (byte)((_DIQ & 0x83) | (((byte)value << 2) & 0x7c)); }
        }
        #endregion

        #region Methods
        public void Init()
        {
            _timestamp = new CP56Time2a(DateTime.Now);
        }
        public override ASDUTypes Type()
        {
            return ASDUTypes.M_DP_TB_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.M_DP_NA_1;
        }
        public override byte[] getValue()
        {
            return BufferExpand.toArray(DPI);
        }
        public override void setValue(byte[] DataArray)
        {
            DPI = DataArray[0];
        }
        public override DateTime getTimestamp()
        {
            return _timestamp.Value;
        }
        public override byte getQuality()
        {
            return (byte)(_DIQ & 0xfc);
        }
        public override void setQuality(byte qos)
        {
        }
        public override void setSelect(bool select)
        {
            if (select)
                _DIQ |= 0x80;
        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.Add(_DIQ);
            retVal.AddRange(_timestamp.Pack());
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _DIQ = BufferExpand.toByte(ref DataArray, ref Offset);
            _timestamp.Expand(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
            QU = job.CmdQualifier;
            _timestamp = new CP56Time2a(DateTime.Now);
        }
        #endregion
    };
    // M_ST_NA_1 - step position
    public class M_ST_NA_1_infObj : infObj
    {
        public M_ST_NA_1_infObj()
        {
            Init();
        }

        #region members
        private byte _VTI;
        private byte _QDS;
        #endregion

        #region Properties
        public byte Value
        {
            get { return (byte)(_VTI & 0x7f); }
            set { _VTI = (byte)((_VTI & 0x80) | (value & 0x7f)); }
        }
        public bool T
        {
            get { return (_VTI & (1 << 7)) == (1 << 7); }
            set { _VTI = (byte)((Convert.ToByte(value) << 7) + (_VTI & ((1 << 7) ^ 0xFF))); }
        }// valid/invalid
        public bool OV
        {
            get { return (_QDS & 1) == 1; }
            set { _QDS = (byte)((Convert.ToByte(value) + (_QDS & (1 ^ 0xFF)))); }
        } // overflow/no overflow
        public byte RES
        {
            get { return (byte)(_QDS & 0x0e); }
            set { _QDS = (byte)((_QDS & 0xf1) | (value & 0x0e)); }
        }
        public bool BL
        {
            get { return (_QDS & (1 << 4)) == (1 << 4); }
            set { _QDS = (byte)((Convert.ToByte(value) << 4) + (_QDS & ((1 << 4) ^ 0xFF))); }
        }// blocked/not blocked
        public bool SB
        {
            get { return (_QDS & (1 << 5)) == (1 << 5); }
            set { _QDS = (byte)((Convert.ToByte(value) << 5) + (_QDS & ((1 << 5) ^ 0xFF))); }
        }// substituted/not substituted
        public bool NT
        {
            get { return (_QDS & (1 << 6)) == (1 << 6); }
            set { _QDS = (byte)((Convert.ToByte(value) << 6) + (_QDS & ((1 << 6) ^ 0xFF))); }
        }// not topical/topical
        public bool IV
        {
            get { return (_QDS & (1 << 7)) == (1 << 7); }
            set { _QDS = (byte)((Convert.ToByte(value) << 7) + (_QDS & ((1 << 7) ^ 0xFF))); }
        }// valid/invalid
        #endregion

        #region Methods
        public void Init()
        {
        }
        public override ASDUTypes Type()
        {
            return ASDUTypes.M_ST_NA_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.M_ST_NA_1;
        }
        public override byte[] getValue()
        {
            return BufferExpand.toArray(Value);
        }
        public override void setValue(byte[] DataArray)
        {
            Value = DataArray[0];
        }
        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }
        public override byte getQuality()
        {
            return _QDS;
        }
        public override void setQuality(byte qos)
        {
            _QDS = qos;
        }
        public override void setSelect(bool select)
        {
            if (select)
                _QDS |= 0x80;
        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.Add(_VTI);
            retVal.Add(_QDS);
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _VTI = BufferExpand.toByte(ref DataArray, ref Offset);
            _QDS = BufferExpand.toByte(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
        }
        #endregion
    };
    // M_ST_TA_1 - step position and CP24Time2a timestamp
    public class M_ST_TA_1_infObj : infObj
    {
        public M_ST_TA_1_infObj()
        {
            Init();
        }
        public M_ST_TA_1_infObj(TimeSpan timestamp)
        {
            _timestamp = new CP24Time2a(timestamp);
        }

        #region members
        private byte _VTI;
        private byte _QDS;
        CP24Time2a _timestamp;
        #endregion

        #region Properties
        public TimeSpan Timestamp
        {
            get { return _timestamp.Value; }
            set { _timestamp.Value = value; }
        }
        public byte Value
        {
            get { return (byte)(_VTI & 0x7f); }
            set { _VTI = (byte)((_VTI & 0x80) | (value & 0x7f)); }
        }
        public bool T
        {
            get { return (_VTI & (1 << 7)) == (1 << 7); }
            set { _VTI = (byte)((Convert.ToByte(value) << 7) + (_VTI & ((1 << 7) ^ 0xFF))); }
        }// valid/invalid
        public bool OV
        {
            get { return (_QDS & 1) == 1; }
            set { _QDS = (byte)((Convert.ToByte(value) + (_QDS & (1 ^ 0xFF)))); }
        } // overflow/no overflow
        public byte RES
        {
            get { return (byte)(_QDS & 0x0e); }
            set { _QDS = (byte)((_QDS & 0xf1) | (value & 0x0e)); }
        }
        public bool BL
        {
            get { return (_QDS & (1 << 4)) == (1 << 4); }
            set { _QDS = (byte)((Convert.ToByte(value) << 4) + (_QDS & ((1 << 4) ^ 0xFF))); }
        }// blocked/not blocked
        public bool SB
        {
            get { return (_QDS & (1 << 5)) == (1 << 5); }
            set { _QDS = (byte)((Convert.ToByte(value) << 5) + (_QDS & ((1 << 5) ^ 0xFF))); }
        }// substituted/not substituted
        public bool NT
        {
            get { return (_QDS & (1 << 6)) == (1 << 6); }
            set { _QDS = (byte)((Convert.ToByte(value) << 6) + (_QDS & ((1 << 6) ^ 0xFF))); }
        }// not topical/topical
        public bool IV
        {
            get { return (_QDS & (1 << 7)) == (1 << 7); }
            set { _QDS = (byte)((Convert.ToByte(value) << 7) + (_QDS & ((1 << 7) ^ 0xFF))); }
        }// valid/invalid
        #endregion

        #region Methods
        public void Init()
        {
            _timestamp = new CP24Time2a(DateTime.Now.TimeOfDay);
        }
        public override ASDUTypes Type()
        {
            return ASDUTypes.M_ST_TA_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.M_ST_NA_1;
        }
        public override byte[] getValue()
        {
            byte[] rt = new byte[1];
            rt[0] = Value;
            return rt;
        }
        public override void setValue(byte[] DataArray)
        {
            Value = DataArray[0];
        }
        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }
        public override byte getQuality()
        {
            return _QDS;
        }
        public override void setQuality(byte qos)
        {
            _QDS = qos;
        }
        public override void setSelect(bool select)
        {
            if (select)
                _QDS |= 0x80;
        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.Add(_VTI);
            retVal.Add(_QDS);
            retVal.AddRange(_timestamp.Pack());
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _VTI = BufferExpand.toByte(ref DataArray, ref Offset);
            _QDS = BufferExpand.toByte(ref DataArray, ref Offset);
            _timestamp.Expand(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
            _timestamp = new CP24Time2a(DateTime.Now.TimeOfDay);
        }
        #endregion
    };
    // M_ST_TB_1 - step position and CP56Time2a timestamp
    public class M_ST_TB_1_infObj : infObj
    {
        public M_ST_TB_1_infObj()
        {
            Init();
        }
        public M_ST_TB_1_infObj(DateTime timestamp)
        {
            _timestamp = new CP56Time2a(timestamp);
        }

        #region members
        private byte _VTI;
        private byte _QDS;
        CP56Time2a _timestamp;
        #endregion

        #region Properties
        public DateTime Timestamp
        {
            get { return _timestamp.Value; }
            set { _timestamp.Value = value; }
        }
        public byte Value
        {
            get { return (byte)(_VTI & 0x7f); }
            set { _VTI = (byte)((_VTI & 0x80) | (value & 0x7f)); }
        }
        public bool T
        {
            get { return (_VTI & (1 << 7)) == (1 << 7); }
            set { _VTI = (byte)((Convert.ToByte(value) << 7) + (_VTI & ((1 << 7) ^ 0xFF))); }
        }// valid/invalid
        public bool OV
        {
            get { return (_QDS & 1) == 1; }
            set { _QDS = (byte)((Convert.ToByte(value) + (_QDS & (1 ^ 0xFF)))); }
        } // overflow/no overflow
        public byte RES
        {
            get { return (byte)(_QDS & 0x0e); }
            set { _QDS = (byte)((_QDS & 0xf1) | (value & 0x0e)); }
        }
        public bool BL
        {
            get { return (_QDS & (1 << 4)) == (1 << 4); }
            set { _QDS = (byte)((Convert.ToByte(value) << 4) + (_QDS & ((1 << 4) ^ 0xFF))); }
        }// blocked/not blocked
        public bool SB
        {
            get { return (_QDS & (1 << 5)) == (1 << 5); }
            set { _QDS = (byte)((Convert.ToByte(value) << 5) + (_QDS & ((1 << 5) ^ 0xFF))); }
        }// substituted/not substituted
        public bool NT
        {
            get { return (_QDS & (1 << 6)) == (1 << 6); }
            set { _QDS = (byte)((Convert.ToByte(value) << 6) + (_QDS & ((1 << 6) ^ 0xFF))); }
        }// not topical/topical
        public bool IV
        {
            get { return (_QDS & (1 << 7)) == (1 << 7); }
            set { _QDS = (byte)((Convert.ToByte(value) << 7) + (_QDS & ((1 << 7) ^ 0xFF))); }
        }// valid/invalid
        #endregion

        #region Methods
        public void Init()
        {
            _timestamp = new CP56Time2a(DateTime.Now);
        }
        public override ASDUTypes Type()
        {
            return ASDUTypes.M_ST_TB_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.M_ST_NA_1;
        }
        public override byte[] getValue()
        {
            byte[] rt = new byte[1];
            rt[0] = Value;
            return rt;
        }
        public override void setValue(byte[] DataArray)
        {
            Value = DataArray[0];
        }
        public override DateTime getTimestamp()
        {
            return _timestamp.Value;
        }
        public override byte getQuality()
        {
            return _QDS;
        }
        public override void setQuality(byte qos)
        {
            _QDS = qos;
        }
        public override void setSelect(bool select)
        {
            if (select)
                _QDS |= 0x80;
        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.Add(_VTI);
            retVal.Add(_QDS);
            retVal.AddRange(_timestamp.Pack());
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _VTI = BufferExpand.toByte(ref DataArray, ref Offset);
            _QDS = BufferExpand.toByte(ref DataArray, ref Offset);
            _timestamp.Expand(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
            _timestamp = new CP56Time2a(DateTime.Now);
        }
        #endregion
    };
    // M_BO_NA_1 - state and change information bit string
    public class M_BO_NA_1_infObj : infObj
    {
        public M_BO_NA_1_infObj()
        {
            Init();
        }

        #region members
        private byte[] _Value;
        private byte _QDS;
        #endregion

        #region Properties
        public byte[] Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
        public bool OV
        {
            get { return (_QDS & 1) == 1; }
            set { _QDS = (byte)((Convert.ToByte(value) + (_QDS & (1 ^ 0xFF)))); }
        } // overflow/no overflow
        public byte RES
        {
            get { return (byte)(_QDS & 0x0e); }
            set { _QDS = (byte)((_QDS & 0xf1) | (value & 0x0e)); }
        }
        public bool BL
        {
            get { return (_QDS & (1 << 4)) == (1 << 4); }
            set { _QDS = (byte)((Convert.ToByte(value) << 4) + (_QDS & ((1 << 4) ^ 0xFF))); }
        }// blocked/not blocked
        public bool SB
        {
            get { return (_QDS & (1 << 5)) == (1 << 5); }
            set { _QDS = (byte)((Convert.ToByte(value) << 5) + (_QDS & ((1 << 5) ^ 0xFF))); }
        }// substituted/not substituted
        public bool NT
        {
            get { return (_QDS & (1 << 6)) == (1 << 6); }
            set { _QDS = (byte)((Convert.ToByte(value) << 6) + (_QDS & ((1 << 6) ^ 0xFF))); }
        }// not topical/topical
        public bool IV
        {
            get { return (_QDS & (1 << 7)) == (1 << 7); }
            set { _QDS = (byte)((Convert.ToByte(value) << 7) + (_QDS & ((1 << 7) ^ 0xFF))); }
        }// valid/invalid
        #endregion

        #region Methods
        public void Init()
        {
            _Value = new byte[4];
        }
        public override ASDUTypes Type()
        {
            return ASDUTypes.M_BO_NA_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.M_BO_NA_1;
        }
        public override byte[] getValue()
        {
            return Value;
        }
        public override void setValue(byte[] DataArray)
        {
            Value = DataArray;
        }
        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }
        public override byte getQuality()
        {
            return _QDS;
        }
        public override void setQuality(byte qos)
        {
            _QDS = qos;
        }
        public override void setSelect(bool select)
        {
            if (select)
                _QDS |= 0x80;
        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(_Value);
            retVal.Add(_QDS);
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _Value = BufferExpand.toArray(ref DataArray, ref Offset,4);
            _QDS = BufferExpand.toByte(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
        }
        #endregion
    };
    // M_BO_TA_1 - state and change information bit string and CP24Time2a timestamp
    public class M_BO_TA_1_infObj : infObj
    {
        public M_BO_TA_1_infObj()
        {
            Init();
        }
        public M_BO_TA_1_infObj(TimeSpan timestamp)
        {
            _timestamp = new CP24Time2a(timestamp);
        }

        #region members
        private byte[] _Value;
        private byte _QDS;
        CP24Time2a _timestamp;
        #endregion

        #region Properties
        public TimeSpan Timestamp
        {
            get { return _timestamp.Value; }
            set { _timestamp.Value = value; }
        }
        public byte[] Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
        public bool OV
        {
            get { return (_QDS & 1) == 1; }
            set { _QDS = (byte)((Convert.ToByte(value) + (_QDS & (1 ^ 0xFF)))); }
        } // overflow/no overflow
        public byte RES
        {
            get { return (byte)(_QDS & 0x0e); }
            set { _QDS = (byte)((_QDS & 0xf1) | (value & 0x0e)); }
        }
        public bool BL
        {
            get { return (_QDS & (1 << 4)) == (1 << 4); }
            set { _QDS = (byte)((Convert.ToByte(value) << 4) + (_QDS & ((1 << 4) ^ 0xFF))); }
        }// blocked/not blocked
        public bool SB
        {
            get { return (_QDS & (1 << 5)) == (1 << 5); }
            set { _QDS = (byte)((Convert.ToByte(value) << 5) + (_QDS & ((1 << 5) ^ 0xFF))); }
        }// substituted/not substituted
        public bool NT
        {
            get { return (_QDS & (1 << 6)) == (1 << 6); }
            set { _QDS = (byte)((Convert.ToByte(value) << 6) + (_QDS & ((1 << 6) ^ 0xFF))); }
        }// not topical/topical
        public bool IV
        {
            get { return (_QDS & (1 << 7)) == (1 << 7); }
            set { _QDS = (byte)((Convert.ToByte(value) << 7) + (_QDS & ((1 << 7) ^ 0xFF))); }
        }// valid/invalid
        #endregion

        #region Methods
        public void Init()
        {
            _Value = new byte[4];
            _timestamp = new CP24Time2a(DateTime.Now.TimeOfDay);
        }
        public override ASDUTypes Type()
        {
            return ASDUTypes.M_BO_TA_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.M_BO_NA_1;
        }
        public override byte[] getValue()
        {
            return Value;
        }
        public override void setValue(byte[] DataArray)
        {
            Value = DataArray;
        }
        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }
        public override byte getQuality()
        {
            return _QDS;
        }
        public override void setQuality(byte qos)
        {
            _QDS = qos;
        }
        public override void setSelect(bool select)
        {
            if (select)
                _QDS |= 0x80;
        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(_Value);
            retVal.Add(_QDS);
            retVal.AddRange(_timestamp.Pack());
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _Value = BufferExpand.toArray(ref DataArray, ref Offset, 4);
            _QDS = BufferExpand.toByte(ref DataArray, ref Offset);
            _timestamp.Expand(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
            _timestamp = new CP24Time2a(DateTime.Now.TimeOfDay);
        }
        #endregion
    };
    // M_BO_TB_1 - state and change information bit string and CP56Time2a timestamp
    public class M_BO_TB_1_infObj : infObj
    {
        public M_BO_TB_1_infObj()
        {
            Init();
        }
        public M_BO_TB_1_infObj(DateTime timestamp)
        {
            _timestamp = new CP56Time2a(timestamp);
        }

        #region members
        private byte[] _Value;
        private byte _QDS;
        CP56Time2a _timestamp;
        #endregion

        #region Properties
        public DateTime Timestamp
        {
            get { return _timestamp.Value; }
            set { _timestamp.Value = value; }
        }
        public byte[] Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
        public bool OV
        {
            get { return (_QDS & 1) == 1; }
            set { _QDS = (byte)((Convert.ToByte(value) + (_QDS & (1 ^ 0xFF)))); }
        } // overflow/no overflow
        public byte RES
        {
            get { return (byte)(_QDS & 0x0e); }
            set { _QDS = (byte)((_QDS & 0xf1) | (value & 0x0e)); }
        }
        public bool BL
        {
            get { return (_QDS & (1 << 4)) == (1 << 4); }
            set { _QDS = (byte)((Convert.ToByte(value) << 4) + (_QDS & ((1 << 4) ^ 0xFF))); }
        }// blocked/not blocked
        public bool SB
        {
            get { return (_QDS & (1 << 5)) == (1 << 5); }
            set { _QDS = (byte)((Convert.ToByte(value) << 5) + (_QDS & ((1 << 5) ^ 0xFF))); }
        }// substituted/not substituted
        public bool NT
        {
            get { return (_QDS & (1 << 6)) == (1 << 6); }
            set { _QDS = (byte)((Convert.ToByte(value) << 6) + (_QDS & ((1 << 6) ^ 0xFF))); }
        }// not topical/topical
        public bool IV
        {
            get { return (_QDS & (1 << 7)) == (1 << 7); }
            set { _QDS = (byte)((Convert.ToByte(value) << 7) + (_QDS & ((1 << 7) ^ 0xFF))); }
        }// valid/invalid
        #endregion

        #region Methods
        public void Init()
        {
            _Value = new byte[4];
            _timestamp = new CP56Time2a(DateTime.Now);
        }
        public override ASDUTypes Type()
        {
            return ASDUTypes.M_BO_TB_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.M_BO_NA_1;
        }
        public override byte[] getValue()
        {
            return Value;
        }
        public override void setValue(byte[] DataArray)
        {
            Value = DataArray;
        }
        public override DateTime getTimestamp()
        {
            return _timestamp.Value;
        }
        public override byte getQuality()
        {
            return _QDS;
        }
        public override void setQuality(byte qos)
        {
            _QDS = qos;
        }
        public override void setSelect(bool select)
        {
            if (select)
                _QDS |= 0x80;
        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(_Value);
            retVal.Add(_QDS);
            retVal.AddRange(_timestamp.Pack());
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _Value = BufferExpand.toArray(ref DataArray, ref Offset, 4);
            _QDS = BufferExpand.toByte(ref DataArray, ref Offset);
            _timestamp.Expand(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
            _timestamp = new CP56Time2a(DateTime.Now);
        }
        #endregion
    };
    // M_ME_NA_1 - normalized measured value
    public class M_ME_NA_1_infObj : infObj
    {
        public M_ME_NA_1_infObj()
        {
            Init();
        }

        #region members
        private short _NVA;
        private byte _QDS;
        #endregion

        #region Properties
        public short Value
        {
            get { return _NVA; }
            set { _NVA = value; }
        }
        public bool OV
        {
            get { return (_QDS & 1) == 1; }
            set { _QDS = (byte)((Convert.ToByte(value)  + (_QDS & (1 ^ 0xFF)))); }
        } // overflow/no overflow
        public byte RES
        {
            get { return (byte)(_QDS & 0x0e); }
            set { _QDS = (byte)((_QDS & 0xf1) | (value & 0x0e)); }
        }
        public bool BL
        {
            get { return (_QDS & (1 << 4)) == (1 << 4); }
            set { _QDS = (byte)((Convert.ToByte(value) << 4) + (_QDS & ((1 << 4) ^ 0xFF))); }
        }// blocked/not blocked
        public bool SB
        {
            get { return (_QDS & (1 << 5)) == (1 << 5); }
            set { _QDS = (byte)((Convert.ToByte(value) << 5) + (_QDS & ((1 << 5) ^ 0xFF))); }
        }// substituted/not substituted
        public bool NT
        {
            get { return (_QDS & (1 << 6)) == (1 << 6); }
            set { _QDS = (byte)((Convert.ToByte(value) << 6) + (_QDS & ((1 << 6) ^ 0xFF))); }
        }// not topical/topical
        public bool IV
        {
            get { return (_QDS & (1 << 7)) == (1 << 7); }
            set { _QDS = (byte)((Convert.ToByte(value) << 7) + (_QDS & ((1 << 7) ^ 0xFF))); }
        }// valid/invalid
        #endregion

        #region Methods
        public void Init()
        {
        }
        public override ASDUTypes Type()
        {
            return ASDUTypes.M_ME_NA_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.M_ME_NA_1;
        }
        public override byte[] getValue()
        {
            return BufferExpand.toArray(Value);
        }
        public override void setValue(byte[] DataArray)
        {
            Value = BufferExpand.toInt16(DataArray);
        }
        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }
        public override byte getQuality()
        {
            return _QDS;
        }
        public override void setQuality(byte qos)
        {
            _QDS = qos;
        }
        public override void setSelect(bool select)
        {
            if (select)
                _QDS |= 0x80;
        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(BufferExpand.toArray(_NVA));
            retVal.Add(_QDS);
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _NVA = BufferExpand.toInt16(ref DataArray, ref Offset);
            _QDS = BufferExpand.toByte(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
        }
        #endregion
    };
    // M_ME_TA_1 - normalized measured value and CP24Time2a timestamp
    public class M_ME_TA_1_infObj : infObj
    {
        public M_ME_TA_1_infObj()
        {
            Init();
        }
        public M_ME_TA_1_infObj(TimeSpan timestamp)
        {
            _timestamp = new CP24Time2a(timestamp);
        }

        #region members
        private short _NVA;
        private byte _QDS;
        CP24Time2a _timestamp;
        #endregion

        #region Properties
        public TimeSpan Timestamp
        {
            get { return _timestamp.Value; }
            set { _timestamp.Value = value; }
        }
        public short Value
        {
            get { return _NVA; }
            set { _NVA = value; }
        }
        public bool OV
        {
            get { return (_QDS & 1) == 1; }
            set { _QDS = (byte)((Convert.ToByte(value) + (_QDS & (1 ^ 0xFF)))); }
        } // overflow/no overflow
        public byte RES
        {
            get { return (byte)(_QDS & 0x0e); }
            set { _QDS = (byte)((_QDS & 0xf1) | (value & 0x0e)); }
        }
        public bool BL
        {
            get { return (_QDS & (1 << 4)) == (1 << 4); }
            set { _QDS = (byte)((Convert.ToByte(value) << 4) + (_QDS & ((1 << 4) ^ 0xFF))); }
        }// blocked/not blocked
        public bool SB
        {
            get { return (_QDS & (1 << 5)) == (1 << 5); }
            set { _QDS = (byte)((Convert.ToByte(value) << 5) + (_QDS & ((1 << 5) ^ 0xFF))); }
        }// substituted/not substituted
        public bool NT
        {
            get { return (_QDS & (1 << 6)) == (1 << 6); }
            set { _QDS = (byte)((Convert.ToByte(value) << 6) + (_QDS & ((1 << 6) ^ 0xFF))); }
        }// not topical/topical
        public bool IV
        {
            get { return (_QDS & (1 << 7)) == (1 << 7); }
            set { _QDS = (byte)((Convert.ToByte(value) << 7) + (_QDS & ((1 << 7) ^ 0xFF))); }
        }// valid/invalid
        #endregion

        #region Methods
        public void Init()
        {
            _timestamp = new CP24Time2a(DateTime.Now.TimeOfDay);
        }
        public override ASDUTypes Type()
        {
            return ASDUTypes.M_ME_TA_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.M_ME_NA_1;
        }
        public override byte[] getValue()
        {
            return BufferExpand.toArray(Value);
        }
        public override void setValue(byte[] DataArray)
        {
            Value = BufferExpand.toInt16(DataArray);
        }
        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }
        public override byte getQuality()
        {
            return _QDS;
        }
        public override void setQuality(byte qos)
        {
            _QDS = qos;
        }
        public override void setSelect(bool select)
        {
            if (select)
                _QDS |= 0x80;
        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(BufferExpand.toArray(_NVA));
            retVal.Add(_QDS);
            retVal.AddRange(_timestamp.Pack());
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _NVA = BufferExpand.toInt16(ref DataArray, ref Offset);
            _QDS = BufferExpand.toByte(ref DataArray, ref Offset);
            _timestamp.Expand(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
        }
        #endregion
    };
    // M_ME_NB_1 - scaled measured value
    public class M_ME_NB_1_infObj : infObj
    {
        public M_ME_NB_1_infObj()
        {
            Init();
        }

        #region members
        private short _SVA;
        private byte _QDS;
        #endregion

        #region Properties
        public short Value
        {
            get { return _SVA; }
            set { _SVA = value; }
        }
        public bool OV
        {
            get { return (_QDS & 1) == 1; }
            set { _QDS = (byte)((Convert.ToByte(value) + (_QDS & (1 ^ 0xFF)))); }
        } // overflow/no overflow
        public byte RES
        {
            get { return (byte)(_QDS & 0x0e); }
            set { _QDS = (byte)((_QDS & 0xf1) | (value & 0x0e)); }
        }
        public bool BL
        {
            get { return (_QDS & (1 << 4)) == (1 << 4); }
            set { _QDS = (byte)((Convert.ToByte(value) << 4) + (_QDS & ((1 << 4) ^ 0xFF))); }
        }// blocked/not blocked
        public bool SB
        {
            get { return (_QDS & (1 << 5)) == (1 << 5); }
            set { _QDS = (byte)((Convert.ToByte(value) << 5) + (_QDS & ((1 << 5) ^ 0xFF))); }
        }// substituted/not substituted
        public bool NT
        {
            get { return (_QDS & (1 << 6)) == (1 << 6); }
            set { _QDS = (byte)((Convert.ToByte(value) << 6) + (_QDS & ((1 << 6) ^ 0xFF))); }
        }// not topical/topical
        public bool IV
        {
            get { return (_QDS & (1 << 7)) == (1 << 7); }
            set { _QDS = (byte)((Convert.ToByte(value) << 7) + (_QDS & ((1 << 7) ^ 0xFF))); }
        }// valid/invalid
        #endregion

        #region Methods
        public void Init()
        {
        }
        public override ASDUTypes Type()
        {
            return ASDUTypes.M_ME_NB_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.M_ME_NB_1;
        }
        public override byte[] getValue()
        {
            return BufferExpand.toArray(Value);
        }
        public override void setValue(byte[] DataArray)
        {
            Value = BufferExpand.toInt16(DataArray);
        }
        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }
        public override byte getQuality()
        {
            return _QDS;
        }
        public override void setQuality(byte qos)
        {
            _QDS = qos;
        }
        public override void setSelect(bool select)
        {
            if (select)
                _QDS |= 0x80;
        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(BufferExpand.toArray(_SVA));
            retVal.Add(_QDS);
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _SVA = BufferExpand.toInt16(ref DataArray, ref Offset);
            _QDS = BufferExpand.toByte(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
        }
        #endregion
    };
    // M_ME_TB_1 - scaled measured value and CP24Time2a timestamp
    public class M_ME_TB_1_infObj : infObj
    {
        public M_ME_TB_1_infObj()
        {
            Init();
        }
        public M_ME_TB_1_infObj(TimeSpan timestamp)
        {
            _timestamp = new CP24Time2a(timestamp);
        }

        #region members
        private short _SVA;
        private byte _QDS;
        CP24Time2a _timestamp;
        #endregion

        #region Properties
        public TimeSpan Timestamp
        {
            get { return _timestamp.Value; }
            set { _timestamp.Value = value; }
        }
        public short Value
        {
            get { return _SVA; }
            set { _SVA = value; }
        }
        public bool OV
        {
            get { return (_QDS & 1) == 1; }
            set { _QDS = (byte)((Convert.ToByte(value) + (_QDS & (1 ^ 0xFF)))); }
        } // overflow/no overflow
        public byte RES
        {
            get { return (byte)(_QDS & 0x0e); }
            set { _QDS = (byte)((_QDS & 0xf1) | (value & 0x0e)); }
        }
        public bool BL
        {
            get { return (_QDS & (1 << 4)) == (1 << 4); }
            set { _QDS = (byte)((Convert.ToByte(value) << 4) + (_QDS & ((1 << 4) ^ 0xFF))); }
        }// blocked/not blocked
        public bool SB
        {
            get { return (_QDS & (1 << 5)) == (1 << 5); }
            set { _QDS = (byte)((Convert.ToByte(value) << 5) + (_QDS & ((1 << 5) ^ 0xFF))); }
        }// substituted/not substituted
        public bool NT
        {
            get { return (_QDS & (1 << 6)) == (1 << 6); }
            set { _QDS = (byte)((Convert.ToByte(value) << 6) + (_QDS & ((1 << 6) ^ 0xFF))); }
        }// not topical/topical
        public bool IV
        {
            get { return (_QDS & (1 << 7)) == (1 << 7); }
            set { _QDS = (byte)((Convert.ToByte(value) << 7) + (_QDS & ((1 << 7) ^ 0xFF))); }
        }// valid/invalid
        #endregion

        #region Methods
        public void Init()
        {
            _timestamp = new CP24Time2a(DateTime.Now.TimeOfDay);
        }
        public override ASDUTypes Type()
        {
            return ASDUTypes.M_ME_TB_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.M_ME_NB_1;
        }
        public override byte[] getValue()
        {
            return BufferExpand.toArray(Value);
        }
        public override void setValue(byte[] DataArray)
        {
            Value = BufferExpand.toInt16(DataArray);
        }
        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }
        public override byte getQuality()
        {
            return _QDS;
        }
        public override void setQuality(byte qos)
        {
            _QDS = qos;
        }
        public override void setSelect(bool select)
        {
            if (select)
                _QDS |= 0x80;
        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(BufferExpand.toArray(_SVA));
            retVal.Add(_QDS);
            retVal.AddRange(_timestamp.Pack());
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _SVA = BufferExpand.toInt16(ref DataArray, ref Offset);
            _QDS = BufferExpand.toByte(ref DataArray, ref Offset);
            _timestamp.Expand(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
        }
        #endregion
    };
    // M_ME_NC_1 - short floating point measured value
    public class M_ME_NC_1_infObj : infObj
    {
        public M_ME_NC_1_infObj()
        {
            Init();
        }

        #region members
        private float _Value;
        private byte _QDS;
        #endregion

        #region Properties
        public float Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
        public bool OV
        {
            get { return (_QDS & 1) == 1; }
            set { _QDS = (byte)((Convert.ToByte(value)  + (_QDS & (1 ^ 0xFF)))); }
        } // overflow/no overflow
        public byte RES
        {
            get { return (byte)(_QDS & 0x0e); }
            set { _QDS = (byte)((_QDS & 0xf1) | (value & 0x0e)); }
        }
        public bool BL
        {
            get { return (_QDS & (1 << 4)) == (1 << 4); }
            set { _QDS = (byte)((Convert.ToByte(value) << 4) + (_QDS & ((1 << 4) ^ 0xFF))); }
        }// blocked/not blocked
        public bool SB
        {
            get { return (_QDS & (1 << 5)) == (1 << 5); }
            set { _QDS = (byte)((Convert.ToByte(value) << 5) + (_QDS & ((1 << 5) ^ 0xFF))); }
        }// substituted/not substituted
        public bool NT
        {
            get { return (_QDS & (1 << 6)) == (1 << 6); }
            set { _QDS = (byte)((Convert.ToByte(value) << 6) + (_QDS & ((1 << 6) ^ 0xFF))); }
        }// not topical/topical
        public bool IV
        {
            get { return (_QDS & (1 << 7)) == (1 << 7); }
            set { _QDS = (byte)((Convert.ToByte(value) << 7) + (_QDS & ((1 << 7) ^ 0xFF))); }
        }// valid/invalid
        #endregion

        #region Methods
        public void Init()
        {
        }
        public override ASDUTypes Type()
        {
            return ASDUTypes.M_ME_NC_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.M_ME_NC_1;
        }
        public override byte[] getValue()
        {
            return BufferExpand.toArray(_Value);
        }
        public override void setValue(byte[] DataArray)
        {
            Value = BufferExpand.toFloat(DataArray);
        }
        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }
        public override byte getQuality()
        {
            return _QDS;
        }
        public override void setQuality(byte qos)
        {
            _QDS = qos;
        }
        public override void setSelect(bool select)
        {
            if (select)
                _QDS |= 0x80;
        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(BufferExpand.toArray(_Value));
            retVal.Add(_QDS);
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _Value = BufferExpand.toFloat(ref DataArray, ref Offset);
            _QDS = BufferExpand.toByte(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
        }
        #endregion
    };
    // M_ME_TC_1 - short floating point measured value and CP24Time2a timestamp
    public class M_ME_TC_1_infObj : infObj
    {
        public M_ME_TC_1_infObj()
        {
            Init();
        }
        public M_ME_TC_1_infObj(TimeSpan timestamp)
        {
            _timestamp = new CP24Time2a(timestamp);
        }

        #region members
        private float _Value;
        private byte _QDS;
        CP24Time2a _timestamp;
        #endregion

        #region Properties
        public TimeSpan Timestamp
        {
            get { return _timestamp.Value; }
            set { _timestamp.Value = value; }
        }
        public float Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
        public bool OV
        {
            get { return (_QDS & 1) == 1; }
            set { _QDS = (byte)((Convert.ToByte(value) + (_QDS & (1 ^ 0xFF)))); }
        } // overflow/no overflow
        public byte RES
        {
            get { return (byte)(_QDS & 0x0e); }
            set { _QDS = (byte)((_QDS & 0xf1) | (value & 0x0e)); }
        }
        public bool BL
        {
            get { return (_QDS & (1 << 4)) == (1 << 4); }
            set { _QDS = (byte)((Convert.ToByte(value) << 4) + (_QDS & ((1 << 4) ^ 0xFF))); }
        }// blocked/not blocked
        public bool SB
        {
            get { return (_QDS & (1 << 5)) == (1 << 5); }
            set { _QDS = (byte)((Convert.ToByte(value) << 5) + (_QDS & ((1 << 5) ^ 0xFF))); }
        }// substituted/not substituted
        public bool NT
        {
            get { return (_QDS & (1 << 6)) == (1 << 6); }
            set { _QDS = (byte)((Convert.ToByte(value) << 6) + (_QDS & ((1 << 6) ^ 0xFF))); }
        }// not topical/topical
        public bool IV
        {
            get { return (_QDS & (1 << 7)) == (1 << 7); }
            set { _QDS = (byte)((Convert.ToByte(value) << 7) + (_QDS & ((1 << 7) ^ 0xFF))); }
        }// valid/invalid
        #endregion

        #region Methods
        public void Init()
        {
            _timestamp = new CP24Time2a(DateTime.Now.TimeOfDay);
        }
        public override ASDUTypes Type()
        {
            return ASDUTypes.M_ME_TC_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.M_ME_NC_1;
        }
        public override byte[] getValue()
        {
            return BufferExpand.toArray(_Value);
        }
        public override void setValue(byte[] DataArray)
        {
            Value = BufferExpand.toFloat(DataArray);
        }
        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }
        public override byte getQuality()
        {
            return _QDS;
        }
        public override void setQuality(byte qos)
        {
            _QDS = qos;
        }
        public override void setSelect(bool select)
        {
            if (select)
                _QDS |= 0x80;
        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(BufferExpand.toArray(_Value));
            retVal.Add(_QDS);
            retVal.AddRange(_timestamp.Pack());
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _Value = BufferExpand.toFloat(ref DataArray, ref Offset);
            _QDS = BufferExpand.toByte(ref DataArray, ref Offset);
            _timestamp.Expand(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
        }
        #endregion
    };
    // M_IT_NA_1 - integrated totals
    public class M_IT_NA_1_infObj : infObj
    {
        public M_IT_NA_1_infObj()
        {
            Init();
        }

        #region members
        private Int32 _Value;
        private byte _Sequence;
        #endregion

        #region Properties
        public Int32 Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
        public byte SQ
        {
            get { return (byte)(_Sequence & 0x1f); }
            set { _Sequence = (byte)((_Sequence & 0xe0) | (value & 0x1f)); }
        }
        public bool CY
        {
            get { return (_Sequence & (1 << 5)) == (1 << 5); }
            set { _Sequence = (byte)((Convert.ToByte(value) << 5) + (_Sequence & ((1 << 5) ^ 0xFF))); }
        } //no overflow/overflow
        public bool CA
        {
            get { return (_Sequence & (1 << 6)) == (1 << 6); }
            set { _Sequence = (byte)((Convert.ToByte(value) << 6) + (_Sequence & ((1 << 6) ^ 0xFF))); }
        }// not adjusted/adjusted
        public bool IV
        {
            get { return (_Sequence & (1 << 7)) == (1 << 7); }
            set { _Sequence = (byte)((Convert.ToByte(value) << 7) + (_Sequence & ((1 << 7) ^ 0xFF))); }
        }// valid/invalid
        #endregion

        #region Methods
        public void Init()
        {
        }
        public override ASDUTypes Type()
        {
            return ASDUTypes.M_IT_NA_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.M_IT_NA_1;
        }
        public override byte[] getValue()
        {
            return BufferExpand.toArray(Value);
        }
        public override void setValue(byte[] DataArray)
        {
            Value = BufferExpand.toInt32(DataArray);
        }
        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }
        public override byte getQuality()
        {
            return 0;
        }
        public override void setQuality(byte qos)
        {
        }
        public override void setSelect(bool select)
        {
            
        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(BufferExpand.toArray(_Value));
            retVal.Add(_Sequence);
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _Value = BufferExpand.toInt32(ref DataArray, ref Offset);
            _Sequence = BufferExpand.toByte(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
        }
        #endregion
    };
    // M_IT_NC_1 - integrated totals and CP24Time2a timestamp
    public class M_IT_NC_1_infObj : infObj
    {
        public M_IT_NC_1_infObj()
        {
            Init();
        }
        public M_IT_NC_1_infObj(TimeSpan timestamp)
        {
            _timestamp = new CP24Time2a(timestamp);
        }

        #region members
        private Int32 _Value;
        private byte _Sequence;
        CP24Time2a _timestamp;
        #endregion

        #region Properties
        public TimeSpan Timestamp
        {
            get { return _timestamp.Value; }
            set { _timestamp.Value = value; }
        }
        public Int32 Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
        public byte SQ
        {
            get { return (byte)(_Sequence & 0x1f); }
            set { _Sequence = (byte)((_Sequence & 0xe0) | (value & 0x1f)); }
        }
        public bool CY
        {
            get { return (_Sequence & (1 << 5)) == (1 << 5); }
            set { _Sequence = (byte)((Convert.ToByte(value) << 5) + (_Sequence & ((1 << 5) ^ 0xFF))); }
        } //no overflow/overflow
        public bool CA
        {
            get { return (_Sequence & (1 << 6)) == (1 << 6); }
            set { _Sequence = (byte)((Convert.ToByte(value) << 6) + (_Sequence & ((1 << 6) ^ 0xFF))); }
        }// not adjusted/adjusted
        public bool IV
        {
            get { return (_Sequence & (1 << 7)) == (1 << 7); }
            set { _Sequence = (byte)((Convert.ToByte(value) << 7) + (_Sequence & ((1 << 7) ^ 0xFF))); }
        }// valid/invalid
        #endregion

        #region Methods
        public void Init()
        {
            _timestamp = new CP24Time2a(DateTime.Now.TimeOfDay);
        }
        public override ASDUTypes Type()
        {
            return ASDUTypes.M_IT_NC_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.M_IT_NA_1;
        }
        public override byte[] getValue()
        {
            return BufferExpand.toArray(Value);
        }
        public override void setValue(byte[] DataArray)
        {
            Value = BufferExpand.toInt32(DataArray);
        }
        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }
        public override byte getQuality()
        {
            return 0;
        }
        public override void setQuality(byte qos)
        {            
        }
        public override void setSelect(bool select)
        {            
        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(BufferExpand.toArray(_Value));
            retVal.Add(_Sequence);
            retVal.AddRange(_timestamp.Pack());
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _Value = BufferExpand.toInt32(ref DataArray, ref Offset);
            _Sequence = BufferExpand.toByte(ref DataArray, ref Offset);
            _timestamp.Expand(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
        }
        #endregion
    };
    // M_ME_TD_1 - normalized measured value with time tag
    public class M_ME_TD_1_infObj : infObj
    {
        public M_ME_TD_1_infObj()
        {
            Init();
        }
        public M_ME_TD_1_infObj(DateTime DateTime)
        {
            _timestamp = new CP56Time2a(DateTime);
        }

        #region members
        private short _NVA;
        private byte _QDS;
        CP56Time2a _timestamp;
        #endregion

        #region Properties
        public DateTime Timestamp
        {
            get { return _timestamp.Value; }
            set { _timestamp.Value = value; }
        }
        public short Value
        {
            get { return _NVA; }
            set { _NVA = value; }
        }
        public bool OV
        {
            get { return (_QDS & 1) == 1; }
            set { _QDS = (byte)((Convert.ToByte(value) + (_QDS & (1 ^ 0xFF)))); }
        } // overflow/no overflow
        public byte RES
        {
            get { return (byte)(_QDS & 0x0e); }
            set { _QDS = (byte)((_QDS & 0xf1) | (value & 0x0e)); }
        }
        public bool BL
        {
            get { return (_QDS & (1 << 4)) == (1 << 4); }
            set { _QDS = (byte)((Convert.ToByte(value) << 4) + (_QDS & ((1 << 4) ^ 0xFF))); }
        }// blocked/not blocked
        public bool SB
        {
            get { return (_QDS & (1 << 5)) == (1 << 5); }
            set { _QDS = (byte)((Convert.ToByte(value) << 5) + (_QDS & ((1 << 5) ^ 0xFF))); }
        }// substituted/not substituted
        public bool NT
        {
            get { return (_QDS & (1 << 6)) == (1 << 6); }
            set { _QDS = (byte)((Convert.ToByte(value) << 6) + (_QDS & ((1 << 6) ^ 0xFF))); }
        }// not topical/topical
        public bool IV
        {
            get { return (_QDS & (1 << 7)) == (1 << 7); }
            set { _QDS = (byte)((Convert.ToByte(value) << 7) + (_QDS & ((1 << 7) ^ 0xFF))); }
        }// valid/invalid
        #endregion

        #region Methods
        public void Init()
        {
            _timestamp = new CP56Time2a(DateTime.Now);
        }
        public override byte[] getValue()
        {
            return BufferExpand.toArray(Value);
        }
        public override void setValue(byte[] DataArray)
        {
            Value = BufferExpand.toInt16(DataArray);
        }
        public override DateTime getTimestamp()
        {
            return _timestamp.Value;
        }
        public override byte getQuality()
        {
            return _QDS;
        }
        public override void setQuality(byte qos)
        {
            _QDS = qos;
        }
        public override void setSelect(bool select)
        {
            if (select)
                _QDS |= 0x80;
        }
        public override ASDUTypes Type()
        {
            return ASDUTypes.M_ME_TD_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.M_ME_NA_1;
        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(BufferExpand.toArray(_NVA));
            retVal.Add(_QDS);
            retVal.AddRange(_timestamp.Pack());
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _NVA = BufferExpand.toInt16(ref DataArray, ref Offset);
            _QDS = BufferExpand.toByte(ref DataArray, ref Offset);
            _timestamp.Expand(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
        }
        #endregion
    };
    // M_ME_TE_1 - scaled measured value with time tag
    public class M_ME_TE_1_infObj : infObj
    {
        public M_ME_TE_1_infObj()
        {
            Init();
        }
        public M_ME_TE_1_infObj(DateTime DateTime)
        {
            _timestamp = new CP56Time2a(DateTime);
        }

        #region members
        private short _SVA;
        private byte _QDS;
        CP56Time2a _timestamp;
        #endregion

        #region Properties
        public DateTime Timestamp
        {
            get { return _timestamp.Value; }
            set { _timestamp.Value = value; }
        }
        public short Value
        {
            get { return _SVA; }
            set { _SVA = value; }
        }
        public bool OV
        {
            get { return (_QDS & 1) == 1; }
            set { _QDS = (byte)((Convert.ToByte(value) + (_QDS & (1 ^ 0xFF)))); }
        } // overflow/no overflow
        public byte RES
        {
            get { return (byte)(_QDS & 0x0e); }
            set { _QDS = (byte)((_QDS & 0xf1) | (value & 0x0e)); }
        }
        public bool BL
        {
            get { return (_QDS & (1 << 4)) == (1 << 4); }
            set { _QDS = (byte)((Convert.ToByte(value) << 4) + (_QDS & ((1 << 4) ^ 0xFF))); }
        }// blocked/not blocked
        public bool SB
        {
            get { return (_QDS & (1 << 5)) == (1 << 5); }
            set { _QDS = (byte)((Convert.ToByte(value) << 5) + (_QDS & ((1 << 5) ^ 0xFF))); }
        }// substituted/not substituted
        public bool NT
        {
            get { return (_QDS & (1 << 6)) == (1 << 6); }
            set { _QDS = (byte)((Convert.ToByte(value) << 6) + (_QDS & ((1 << 6) ^ 0xFF))); }
        }// not topical/topical
        public bool IV
        {
            get { return (_QDS & (1 << 7)) == (1 << 7); }
            set { _QDS = (byte)((Convert.ToByte(value) << 7) + (_QDS & ((1 << 7) ^ 0xFF))); }
        }// valid/invalid
        #endregion

        #region Methods
        public void Init()
        {
            _timestamp = new CP56Time2a(DateTime.Now);
        }
        public override ASDUTypes Type()
        {
            return ASDUTypes.M_ME_TE_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.M_ME_NB_1;
        }
        public override byte[] getValue()
        {
            return BufferExpand.toArray(Value);
        }
        public override void setValue(byte[] DataArray)
        {
            Value = BufferExpand.toInt16(DataArray);
        }
        public override DateTime getTimestamp()
        {
            return _timestamp.Value;
        }
        public override byte getQuality()
        {
            return _QDS;
        }
        public override void setQuality(byte qos)
        {
            _QDS = qos;
        }
        public override void setSelect(bool select)
        {
            if (select)
                _QDS |= 0x80;
        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(BufferExpand.toArray(_SVA));
            retVal.Add(_QDS);
            retVal.AddRange(_timestamp.Pack());
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _SVA = BufferExpand.toInt16(ref DataArray, ref Offset);
            _QDS = BufferExpand.toByte(ref DataArray, ref Offset);
            _timestamp.Expand(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
        }
        #endregion
    };
    // M_ME_TF_1 - short floating point measurement value and time tag
    public class M_ME_TF_1_infObj : infObj
    {
        public M_ME_TF_1_infObj()
        {
            Init();
        }
        public M_ME_TF_1_infObj(DateTime DateTime)
        {
            _timestamp = new CP56Time2a(DateTime);
        }

        #region members
        private float _Value;
        private byte _QDS;
        CP56Time2a _timestamp;
        #endregion

        #region Properties
        public DateTime Timestamp
        {
            get { return _timestamp.Value; }
            set { _timestamp.Value = value; }
        }
        public float Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
        public bool OV
        {
            get { return (_QDS & 1) == 1; }
            set { _QDS = (byte)((Convert.ToByte(value) + (_QDS & (1 ^ 0xFF)))); }
        } // overflow/no overflow
        public byte RES
        {
            get { return (byte)(_QDS & 0x0e); }
            set { _QDS = (byte)((_QDS & 0xf1) | (value & 0x0e)); }
        }
        public bool BL
        {
            get { return (_QDS & (1 << 4)) == (1 << 4); }
            set { _QDS = (byte)((Convert.ToByte(value) << 4) + (_QDS & ((1 << 4) ^ 0xFF))); }
        }// blocked/not blocked
        public bool SB
        {
            get { return (_QDS & (1 << 5)) == (1 << 5); }
            set { _QDS = (byte)((Convert.ToByte(value) << 5) + (_QDS & ((1 << 5) ^ 0xFF))); }
        }// substituted/not substituted
        public bool NT
        {
            get { return (_QDS & (1 << 6)) == (1 << 6); }
            set { _QDS = (byte)((Convert.ToByte(value) << 6) + (_QDS & ((1 << 6) ^ 0xFF))); }
        }// not topical/topical
        public bool IV
        {
            get { return (_QDS & (1 << 7)) == (1 << 7); }
            set { _QDS = (byte)((Convert.ToByte(value) << 7) + (_QDS & ((1 << 7) ^ 0xFF))); }
        }// valid/invalid
        #endregion

        #region Methods
        public void Init()
        {
            _timestamp = new CP56Time2a(DateTime.Now);
        }
        public override ASDUTypes Type()
        {
            return ASDUTypes.M_ME_TF_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.M_ME_NC_1;
        }
        public override byte[] getValue()
        {
            return BufferExpand.toArray(_Value);
        }
        public override void setValue(byte[] DataArray)
        {
            Value = BufferExpand.toFloat(DataArray);
        }
        public override DateTime getTimestamp()
        {
            return _timestamp.Value;
        }
        public override byte getQuality()
        {
            return _QDS;
        }
        public override void setQuality(byte qos)
        {
            _QDS = qos;
        }
        public override void setSelect(bool select)
        {
            if (select)
                _QDS |= 0x80;
        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(BufferExpand.toArray(_Value));
            retVal.Add(_QDS);
            retVal.AddRange(_timestamp.Pack());
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _Value = BufferExpand.toFloat(ref DataArray, ref Offset);
            _QDS = BufferExpand.toByte(ref DataArray, ref Offset);
            _timestamp.Expand(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
        }
        #endregion
    };
    // M_IT_TB_1 - integrated totals and time tag
    public class M_IT_TB_1_infObj : infObj
    {
        public M_IT_TB_1_infObj()
        {
            Init();
        }
        public M_IT_TB_1_infObj(DateTime DateTime)
        {
            _timestamp = new CP56Time2a(DateTime);
        }

        #region members
        private Int32 _Value;
        private byte _Sequence;
        CP56Time2a _timestamp;
        #endregion

        #region Properties
        public DateTime Timestamp
        {
            get { return _timestamp.Value; }
            set { _timestamp.Value = value; }
        }
        public Int32 Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
        public byte SQ
        {
            get { return (byte)(_Sequence & 0x1f); }
            set { _Sequence = (byte)((_Sequence & 0xe0) | (value & 0x1f)); }
        }
        public bool CY
        {
            get { return (_Sequence & (1 << 5)) == (1 << 5); }
            set { _Sequence = (byte)((Convert.ToByte(value) << 5) + (_Sequence & ((1 << 5) ^ 0xFF))); }
        } //no overflow/overflow
        public bool CA
        {
            get { return (_Sequence & (1 << 6)) == (1 << 6); }
            set { _Sequence = (byte)((Convert.ToByte(value) << 6) + (_Sequence & ((1 << 6) ^ 0xFF))); }
        }// not adjusted/adjusted
        public bool IV
        {
            get { return (_Sequence & (1 << 7)) == (1 << 7); }
            set { _Sequence = (byte)((Convert.ToByte(value) << 7) + (_Sequence & ((1 << 7) ^ 0xFF))); }
        }// valid/invalid
        #endregion

        #region Methods
        public void Init()
        {
            _timestamp = new CP56Time2a(DateTime.Now);
        }
        public override ASDUTypes Type()
        {
            return ASDUTypes.M_IT_TB_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.M_IT_NA_1;
        }
        public override byte[] getValue()
        {
            return BufferExpand.toArray(Value);
        }
        public override void setValue(byte[] DataArray)
        {
            Value = BufferExpand.toInt32(DataArray);
        }
        public override DateTime getTimestamp()
        {
            return _timestamp.Value;
        }
        public override byte getQuality()
        {
            return 0;
        }
        public override void setQuality(byte qos)
        {            
        }
        public override void setSelect(bool select)
        {            
        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(BufferExpand.toArray(_Value));
            retVal.Add(_Sequence);
            retVal.AddRange(_timestamp.Pack());
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _Value = BufferExpand.toInt32(ref DataArray, ref Offset);
            _Sequence = BufferExpand.toByte(ref DataArray, ref Offset);
            _timestamp.Expand(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
        }
        #endregion
    };

    public class SelectFile_infObj : infObj
    {
        public SelectFile_infObj()
        {
            Init();
        }

        #region methods

        public void Init()
        {
        }

        public override void initJobData(IEC60870_5_104CommJob job)
        {
            UInt16 fileNameValue = 0; // The file name is a numeric value (UINT 16bits)
            job.getFileNameVariable(ref fileNameValue);
            _FileName = fileNameValue;
            _SectionName = job.FileSection;
            // Select and Call Qualifier
            _SCQ = job.JobSelectAndCallQualifier;
            if((_SCQ == (byte)SelectAndCallQualifier.selectFile) && (job.TagsListToWrite.Count > 0))
            {
                // Call job.GetJobData for moving the job tag from job.TagsListToWrite to job.TagListOnWriting,
                // (actually the value of the tag it's not used)
                object jobData = null;
                job.GetJobData(ref jobData);
            }
        }

        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(BitConverter.GetBytes(_FileName));
            retVal.Add(_SectionName);
            retVal.Add(_SCQ);
            return retVal.ToArray();
        }

        // Expand not implemented, because this ASDU is used only for requests 
        public override void Expand(ref byte[] DataArray, ref int Offset)
        {

        }

        public override ASDUTypes Type()
        {
            return ASDUTypes.F_SC_NA_1;
        }

        public override ASDUTypes basicType()
        {
            return ASDUTypes.F_SC_NA_1;
        }

        public override byte[] getValue()
        {
            return new byte[0];
        }

        public override void setValue(byte[] Value)
        {

        }

        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }

        public override byte getQuality()
        {
            return 0;
        }
        public override void setQuality(byte qos)
        {
        }
        public override void setSelect(bool select)
        {            
        }
        #endregion

        #region Properties

        private UInt16 _FileName;
        public UInt16 FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }

        private byte _SectionName;
        public byte SectionName
        {
            get { return _SectionName; }
            set { _SectionName = value; }
        }

        // Select and Call Qualifier
        private byte _SCQ;
        public byte SCQ
        {
            get { return _SCQ; }
            set { _SCQ = value; }
        }

        #endregion
    };

    public class AckFile_infObj : infObj
    {
        public AckFile_infObj()
        {
            Init();
        }

        #region methods

        public void Init()
        {
        }

        public override void initJobData(IEC60870_5_104CommJob job)
        {
            UInt16 fileNameValue = 0; // The file name is a numeric value (UINT 16bits)
            job.getFileNameVariable(ref fileNameValue);
            _FileName = fileNameValue;
            _SectionName = job.FileSection;
            // Acknowledge file or section qualifier
            _AFQ = job.JobAcknowledgeFileQualifier;
        }

        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(BitConverter.GetBytes(_FileName));
            retVal.Add(_SectionName);
            retVal.Add(_AFQ);
            return retVal.ToArray();
        }

        // Expand not implemented, because this ASDU is used only for requests 
        public override void Expand(ref byte[] DataArray, ref int Offset)
        {

        }

        public override ASDUTypes Type()
        {
            return ASDUTypes.F_AF_NA_1;
        }

        public override ASDUTypes basicType()
        {
            return ASDUTypes.F_AF_NA_1;
        }

        public override byte[] getValue()
        {
            return new byte[0];
        }

        public override void setValue(byte[] Value)
        {

        }

        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }

        public override byte getQuality()
        {
            return 0;
        }
        public override void setQuality(byte qos)
        {
        }
        public override void setSelect(bool select)
        {
        }
        #endregion

        #region Properties

        private UInt16 _FileName;
        public UInt16 FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }

        private byte _SectionName;
        public byte SectionName
        {
            get { return _SectionName; }
            set { _SectionName = value; }
        }

        // Acknowledge file or section qualifier
        private byte _AFQ;
        public byte AFQ
        {
            get { return _AFQ; }
            set { _AFQ = value; }
        }

        #endregion
    };

    public class FileReady_infObj : infObj
    {
        public FileReady_infObj()
        {
            Init();
        }

        #region methods

        public void Init()
        {
        }

        public override void initJobData(IEC60870_5_104CommJob job)
        {
            UInt16 fileNameValue = 0; // The file name is a numeric value (UINT 16bits)
            job.getFileNameVariable(ref fileNameValue);
            _FileName = fileNameValue;
            _FileLength = 0;
            // File Ready Qualifier
            _FRQ = 0; // Positive confirm of select
        }

        // Pack not implemented for the moment, because this ASDU is used only for replies 
        public override byte[] Pack()
        {
            return new byte[0];
        }

        // Parse the data of the received ASDU 
        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            // The file name is an integer, 2 bytes long
            _FileName = BufferExpand.toUInt16(ref DataArray, ref Offset);
            // The file length is an integer, 3 bytes long
            byte[] fileLength = new byte[3];
            fileLength = BufferExpand.toArray(ref DataArray, ref Offset, 3);
            _FileLength = BufferExpand.toUInt32(fileLength);
            // File Ready Qualifier
            _FRQ = BufferExpand.toByte(ref DataArray, ref Offset);
        }

        public override ASDUTypes Type()
        {
            return ASDUTypes.F_FR_NA_1;
        }

        public override ASDUTypes basicType()
        {
            return ASDUTypes.F_FR_NA_1;
        }

        public override byte[] getValue()
        {
            return new byte[0];
        }

        public override void setValue(byte[] Value)
        {

        }

        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }

        public override byte getQuality()
        {
            return 0;
        }
        public override void setQuality(byte qos)
        {
        }
        public override void setSelect(bool select)
        {
        }
        #endregion

        #region Properties

        private UInt16 _FileName;
        public UInt16 FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }

        private uint _FileLength;
        public uint FileLength
        {
            get { return _FileLength; }
            set { _FileLength = value; }
        }

        // File Ready Qualifier
        private byte _FRQ;
        public byte FRQ
        {
            get { return _FRQ; }
            set { _FRQ = value; }
        }

        #endregion
    };

    public class SectionReady_infObj : infObj
    {
        public SectionReady_infObj()
        {
            Init();
        }

        #region methods

        public void Init()
        {
        }

        public override void initJobData(IEC60870_5_104CommJob job)
        {
            UInt16 fileNameValue = 0; // The file name is a numeric value (UINT 16bits)
            job.getFileNameVariable(ref fileNameValue);
            _FileName = fileNameValue;
            _SectionName = 0; // Default value
            _SectionLength = 0;
            // File Ready Qualifier
            _SRQ = 0; // Section ready to load
        }

        // Pack not implemented for the moment, because this ASDU is used only for replies 
        public override byte[] Pack()
        {
            return new byte[0];
        }

        // Parse the data of the received ASDU 
        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            // The file name is an integer, 2 bytes long
            _FileName = BufferExpand.toUInt16(ref DataArray, ref Offset);
            // The section name is an integer, 1 byte long
            _SectionName = BufferExpand.toByte(ref DataArray, ref Offset);
            // The section length is an integer, 3 bytes long
            byte[] sectionLength = new byte[3];
            sectionLength = BufferExpand.toArray(ref DataArray, ref Offset, 3);
            _SectionLength = BufferExpand.toUInt32(sectionLength);
            // Section Ready Qualifier
            _SRQ = BufferExpand.toByte(ref DataArray, ref Offset);
        }

        public override ASDUTypes Type()
        {
            return ASDUTypes.F_SR_NA_1;
        }

        public override ASDUTypes basicType()
        {
            return ASDUTypes.F_SR_NA_1;
        }

        public override byte[] getValue()
        {
            return new byte[0];
        }

        public override void setValue(byte[] Value)
        {

        }

        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }

        public override byte getQuality()
        {
            return 0;
        }
        public override void setQuality(byte qos)
        {
        }
        public override void setSelect(bool select)
        {
        }
        #endregion

        #region Properties

        private UInt16 _FileName;
        public UInt16 FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }

        private byte _SectionName;
        public byte SectionName
        {
            get { return _SectionName; }
            set { _SectionName = value; }
        }

        private uint _SectionLength;
        public uint SectionLength
        {
            get { return _SectionLength; }
            set { _SectionLength = value; }
        }

        // Section Ready Qualifier
        private byte _SRQ;
        public byte SRQ
        {
            get { return _SRQ; }
            set { _SRQ = value; }
        }

        #endregion
    };

    public class Segment_infObj : infObj
    {
        public Segment_infObj()
        {
            Init();
        }

        #region methods

        public void Init()
        {
        }

        public override void initJobData(IEC60870_5_104CommJob job)
        {
            UInt16 fileNameValue = 0; // The file name is a numeric value (UINT 16bits)
            job.getFileNameVariable(ref fileNameValue);
            _FileName = fileNameValue;
            _SectionName = job.FileSection;
            _SegmentLength = 0;
            SegmentBytes = null;
        }

        // Pack not implemented for the moment, because this ASDU is used only for replies 
        public override byte[] Pack()
        {
            return new byte[0];
        }

        // Parse the data of the received ASDU 
        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            // The file name is an integer, 2 bytes long
            _FileName = BufferExpand.toUInt16(ref DataArray, ref Offset);
            // The section name is an integer, 1 byte long
            _SectionName = BufferExpand.toByte(ref DataArray, ref Offset);
            // The segment length is an integer, 1 byte long
            _SegmentLength = BufferExpand.toByte(ref DataArray, ref Offset);
            // Segment bytes
            SegmentBytes = new byte[_SegmentLength];
            SegmentBytes = BufferExpand.toArray(ref DataArray, ref Offset, (uint)_SegmentLength);
        }

        public override ASDUTypes Type()
        {
            return ASDUTypes.F_SG_NA_1;
        }

        public override ASDUTypes basicType()
        {
            return ASDUTypes.F_SG_NA_1;
        }

        public override byte[] getValue()
        {
            return new byte[0];
        }

        public override void setValue(byte[] Value)
        {

        }

        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }

        public override byte getQuality()
        {
            return 0;
        }
        public override void setQuality(byte qos)
        {
        }
        public override void setSelect(bool select)
        {
        }
        #endregion

        #region Properties

        private UInt16 _FileName;
        public UInt16 FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }

        private byte _SectionName;
        public byte SectionName
        {
            get { return _SectionName; }
            set { _SectionName = value; }
        }

        private byte _SegmentLength;
        public byte SegmentLength
        {
            get { return _SegmentLength; }
            set { _SegmentLength = value; }
        }

        public byte[] SegmentBytes;

        #endregion
    };

    public class LastSegment_infObj : infObj
    {
        public LastSegment_infObj()
        {
            Init();
        }

        #region methods

        public void Init()
        {
        }

        public override void initJobData(IEC60870_5_104CommJob job)
        {
            UInt16 fileNameValue = 0; // The file name is a numeric value (UINT 16bits)
            job.getFileNameVariable(ref fileNameValue);
            _FileName = fileNameValue;
            _SectionName = job.FileSection;
            _LSQ = (byte)LastSectionSegmentQualifier.notUsed; // Default value
            _Checksum = 0; // Default value
        }

        // Pack not implemented for the moment, because this ASDU is used only for replies 
        public override byte[] Pack()
        {
            return new byte[0];
        }

        // Parse the data of the received ASDU 
        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            // The file name is an integer, 2 bytes long
            _FileName = BufferExpand.toUInt16(ref DataArray, ref Offset);
            // The section name is an integer, 1 byte long
            _SectionName = BufferExpand.toByte(ref DataArray, ref Offset);
            // The Last Segment Qualifier is an integer, 1 byte long
            _LSQ = BufferExpand.toByte(ref DataArray, ref Offset);
            _Checksum = BufferExpand.toByte(ref DataArray, ref Offset);
        }

        public override ASDUTypes Type()
        {
            return ASDUTypes.F_SG_NA_1;
        }

        public override ASDUTypes basicType()
        {
            return ASDUTypes.F_SG_NA_1;
        }

        public override byte[] getValue()
        {
            return new byte[0];
        }

        public override void setValue(byte[] Value)
        {

        }

        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }

        public override byte getQuality()
        {
            return 0;
        }
        public override void setQuality(byte qos)
        {
        }
        public override void setSelect(bool select)
        {
        }
        #endregion

        #region Properties

        private UInt16 _FileName;
        public UInt16 FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }

        private byte _SectionName;
        public byte SectionName
        {
            get { return _SectionName; }
            set { _SectionName = value; }
        }

        private byte _LSQ;
        public byte LSQ
        {
            get { return _LSQ; }
            set { _LSQ = value; }
        }

        private byte _Checksum;
        public byte Checksum
        {
            get { return _Checksum; }
            set { _Checksum = value; }
        }
        #endregion
    };

    public class Directory_infObj : infObj
    {
        public const int FileInfoLength = 13;
        public Directory_infObj()
        {
            Init();
        }

        #region methods

        public void Init()
        {
        }

        public override void initJobData(IEC60870_5_104CommJob job)
        {
            _DirectoryLength = 0;
            _DirectoryNumberOfElements = 0;
            _IsDirectoryComplete = false;
            DirectoryContents = null;
        }

        // Pack not implemented for the moment, because this ASDU is used only for replies 
        public override byte[] Pack()
        {
            return new byte[0];
        }

        // Parse the data of the received ASDU 
        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            // Length of the directory contents
            _DirectoryLength = (uint)(DataArray.Count() - Offset);
            // Number of data elements (file info)
            _DirectoryNumberOfElements = _DirectoryLength / FileInfoLength;
            // Additional file of the directory follows? 
            if(_DirectoryNumberOfElements > 0)
            {

            }
            DirectoryContents = new byte[DirectoryLength];
            DirectoryContents = BufferExpand.toArray(ref DataArray, ref Offset, DirectoryLength);
            // Additional file of the directory follows? 
            if (_DirectoryNumberOfElements > 0)
            {
                byte statusOfLastFile = DirectoryContents[(_DirectoryNumberOfElements - 1) * FileInfoLength + 5];
                if((statusOfLastFile & 0x20) > 0)
                {
                    _IsDirectoryComplete = true;
                }
            }
        }

        public override ASDUTypes Type()
        {
            return ASDUTypes.F_DR_TA_1;
        }

        public override ASDUTypes basicType()
        {
            return ASDUTypes.F_DR_TA_1;
        }

        public override byte[] getValue()
        {
            return new byte[0];
        }

        public override void setValue(byte[] Value)
        {

        }

        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }

        public override byte getQuality()
        {
            return 0;
        }
        public override void setQuality(byte qos)
        {
        }
        public override void setSelect(bool select)
        {
        }
        #endregion

        #region Properties

        private uint _DirectoryLength;
        public uint DirectoryLength
        {
            get { return _DirectoryLength; }
            set { _DirectoryLength = value; }
        }

        private uint _DirectoryNumberOfElements;
        public uint DirectoryNumberOfElements
        {
            get { return _DirectoryNumberOfElements; }
            set { _DirectoryNumberOfElements = value; }
        }

        private bool _IsDirectoryComplete;
        public bool IsDirectoryComplete
        {
            get { return _IsDirectoryComplete; }
            set { _IsDirectoryComplete = value; }
        }

        public byte[] DirectoryContents;

        #endregion
    };

    // C_RC_NA_1 - Regulating step command
    public class C_RC_NA_1_infObj : infObj
    {
        public C_RC_NA_1_infObj()
        {
            Init();
        }

        #region members
        private byte _RCO;
        #endregion

        #region Properties
        public CommandActions RCS
        {
            get { return (CommandActions)(_RCO & 0x03); }
            set { _RCO = (byte)((_RCO & 0xfc) | ((byte)value & 0x03)); }
        } // Regulating step command state
        public CommandQualifiers QU
        {
            get { return (CommandQualifiers)((_RCO >> 2) & 0x1f); }
            set { _RCO = (byte)((_RCO & 0x83) | (((byte)value & 0x1f)<<2)); }
        } // Qualifier of command
        public bool SE
        {
            get { return (_RCO & (1 << 7)) == (1 << 7); }
            set { _RCO = (byte)((Convert.ToByte(value) << 7) + (_RCO & ((1 << 7) ^ 0xFF))); }
        }// execute/select
        #endregion

        #region Methods
        public void Init()
        {
        }
        public override byte[] getValue()
        {
            return BufferExpand.toArray(_RCO);
        }
        public override void setValue(byte[] DataArray)
        {
            _RCO = DataArray[0];
        }
        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }
        public override byte getQuality()
        {
            return 0;
        }
        public override void setQuality(byte qos)
        {
        }
        public override void setSelect(bool select)
        {
        }
        public override ASDUTypes Type()
        {
            return ASDUTypes.C_RC_NA_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.C_RC_NA_1;
        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.Add(_RCO);
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _RCO = BufferExpand.toByte(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
            QU = job.CmdQualifier;
            RCS = job.CmdAction;
        }
        #endregion
    };
    // P_ME_NA_1 - Parameter of measured values,normalized measured value
    public class P_ME_NA_1_infObj : infObj
    {
        public P_ME_NA_1_infObj()
        {
            Init();
        }

        #region members
        private short _NVA;
        private byte _QPM;
        #endregion

        #region Properties
        public short Value
        {
            get { return _NVA; }
            set { _NVA = value; }
        }
        public ParamQualifiers KPA
        {
            get { return (ParamQualifiers)(_QPM & 0x3f); }
            set { _QPM = (byte)((_QPM & 0xc0) | ((byte)value & 0x3f)); }
        }
        public bool LPC
        {
            get { return (_QPM & (1 << 6)) == (1 << 6); }
            set { _QPM = (byte)((Convert.ToByte(value) << 6) + (_QPM & ((1 << 6) ^ 0xFF))); }
        } //no change/change
        public bool POP
        {
            get { return (_QPM & (1 << 7)) == (1 << 7); }
            set { _QPM = (byte)((Convert.ToByte(value) << 7) + (_QPM & ((1 << 7) ^ 0xFF))); }
        }// in operation/not in operation
        #endregion

        #region Methods
        public void Init()
        {
        }
        public override ASDUTypes Type()
        {
            return ASDUTypes.P_ME_NA_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.P_ME_NA_1;
        }
        public override byte[] getValue()
        {
            return BufferExpand.toArray(Value);
        }
        public override void setValue(byte[] DataArray)
        {
            Value = BufferExpand.toInt16(DataArray);
        }
        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }
        public override byte getQuality()
        {
            return 0;
        }
        public override void setQuality(byte qos)
        {
        }
        public override void setSelect(bool select)
        {
        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(BufferExpand.toArray(_NVA));
            retVal.Add(_QPM);
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _NVA = BufferExpand.toInt16(ref DataArray, ref Offset);
            _QPM = BufferExpand.toByte(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);

            KPA = job.ParamQualifier;
        }
        #endregion
    };
    // P_ME_NB_1 - Parameter of measured values,scaled value
    public class P_ME_NB_1_infObj : infObj
    {
        public P_ME_NB_1_infObj()
        {
            Init();
        }

        #region members
        private short _NVA;
        private byte _QPM;
        #endregion

        #region Properties
        public short Value
        {
            get { return _NVA; }
            set { _NVA = value; }
        }
        public ParamQualifiers KPA
        {
            get { return (ParamQualifiers)(_QPM & 0x3f); }
            set { _QPM = (byte)((_QPM & 0xc0) | ((byte)value & 0x3f)); }
        }
        public bool LPC
        {
            get { return (_QPM & (1 << 6)) == (1 << 6); }
            set { _QPM = (byte)((Convert.ToByte(value) << 6) + (_QPM & ((1 << 6) ^ 0xFF))); }
        } //no change/change
        public bool POP
        {
            get { return (_QPM & (1 << 7)) == (1 << 7); }
            set { _QPM = (byte)((Convert.ToByte(value) << 7) + (_QPM & ((1 << 7) ^ 0xFF))); }
        }// in operation/not in operation
        #endregion

        #region Methods
        public void Init()
        {
        }
        public override ASDUTypes Type()
        {
            return ASDUTypes.P_ME_NB_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.P_ME_NB_1;
        }
        public override byte[] getValue()
        {
            return BufferExpand.toArray(Value);
        }
        public override void setValue(byte[] DataArray)
        {
            Value = BufferExpand.toInt16(DataArray);
        }
        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }
        public override byte getQuality()
        {
            return 0;
        }
        public override void setQuality(byte qos)
        {
        }
        public override void setSelect(bool select)
        {
        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(BufferExpand.toArray(_NVA));
            retVal.Add(_QPM);
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _NVA = BufferExpand.toInt16(ref DataArray, ref Offset);
            _QPM = BufferExpand.toByte(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
            KPA = job.ParamQualifier;
        }
        #endregion
    };
    // P_ME_NC_1 - Parameter of measured values,short floating point number
    public class P_ME_NC_1_infObj : infObj
    {
        public P_ME_NC_1_infObj()
        {
            Init();
        }

        #region members
        private float _NVA;
        private byte _QPM;
        #endregion

        #region Properties
        public float Value
        {
            get { return _NVA; }
            set { _NVA = value; }
        }
        public ParamQualifiers KPA
        {
            get { return (ParamQualifiers)(_QPM & 0x3f); }
            set { _QPM = (byte)((_QPM & 0xc0) | ((byte)value & 0x3f)); }
        }
        public bool LPC
        {
            get { return (_QPM & (1 << 6)) == (1 << 6); }
            set { _QPM = (byte)((Convert.ToByte(value) << 6) + (_QPM & ((1 << 6) ^ 0xFF))); }
        } //no change/change
        public bool POP
        {
            get { return (_QPM & (1 << 7)) == (1 << 7); }
            set { _QPM = (byte)((Convert.ToByte(value) << 7) + (_QPM & ((1 << 7) ^ 0xFF))); }
        }// in operation/not in operation
        #endregion

        #region Methods
        public void Init()
        {
        }
        public override ASDUTypes Type()
        {
            return ASDUTypes.P_ME_NC_1;
        }
        public override ASDUTypes basicType()
        {
            return ASDUTypes.P_ME_NC_1;
        }
        public override byte[] getValue()
        {
            return BufferExpand.toArray(Value);
        }
        public override void setValue(byte[] DataArray)
        {
            Value = BufferExpand.toFloat(DataArray);
        }
        public override DateTime getTimestamp()
        {
            return default(DateTime);
        }
        public override byte getQuality()
        {
            return 0;
        }
        public override void setQuality(byte qos)
        {
        }
        public override void setSelect(bool select)
        {
        }
        public override byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(BufferExpand.toArray(_NVA));
            retVal.Add(_QPM);
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            _NVA = BufferExpand.toFloat(ref DataArray, ref Offset);
            _QPM = BufferExpand.toByte(ref DataArray, ref Offset);
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            object jobData = null;
            job.GetJobData(ref jobData);
            setValue((byte[])jobData);
            KPA = job.ParamQualifier;
        }
        #endregion
    };

    public class UnitId : IPackable, IExpandable
    {

        public UnitId()
        {
            Init();
        }

        #region member
        private ASDUTypes _TypeId;
        private byte _Qualifier;
        private byte _Cause;
        private byte _OriginatorAddress;
        private ushort _CommonAddress;
        private bool _timestamp;
        private byte _QOS;
        #endregion

        #region Properties
        public ASDUTypes TypeIdBase { get { return _TypeId; } set { _TypeId = value; } }
        public bool Timestamp { get { return _timestamp; } set { _timestamp = value; } }

        public ASDUTypes ControlDirectionId
        {
            get
            {
                return ControlDirection(_TypeId, _timestamp);

            }
        }
        public ASDUTypes TypeId
        {
            get
            {
                return TypeIdComp(_TypeId, _timestamp);

            }
        }

        //(ASDUTypes)job.ASDUType

        //public static ASDUTypes ControlDirection(IEC60870_5_104CommJob job)
        //{
        //}
        //public static ASDUTypes ControlDirection(IEC60870_5_104CommJob job)
        //{
        //}
        public static ASDUTypes TypeIdComp(ASDUTypes Type, bool timestamp) 
        {
            if (!timestamp)
                return Type;
            switch (Type)
            {
                case ASDUTypes.M_SP_NA_1:
                    return ASDUTypes.M_SP_TB_1;
                case ASDUTypes.M_DP_NA_1:
                    return ASDUTypes.M_DP_TB_1;
                case ASDUTypes.M_ST_NA_1:
                    return ASDUTypes.M_ST_TB_1;
                case ASDUTypes.M_BO_NA_1:
                    return ASDUTypes.M_BO_TB_1;
                case ASDUTypes.M_ME_NA_1:
                    return ASDUTypes.M_ME_TD_1;
                case ASDUTypes.M_ME_NB_1:
                    return ASDUTypes.M_ME_TE_1;
                case ASDUTypes.M_ME_NC_1:
                    return ASDUTypes.M_ME_TF_1;
                case ASDUTypes.M_IT_NA_1:
                    return ASDUTypes.M_IT_TB_1;

                default:
                    return Type;
            }
        }
        public static ASDUTypes ControlDirection(ASDUTypes Type, bool timestamp)
        {
            switch (TypeIdComp(Type, timestamp))
            {
                case ASDUTypes.M_SP_NA_1:
                    return ASDUTypes.C_SC_NA_1;
                case ASDUTypes.M_DP_NA_1:
                    return ASDUTypes.C_DC_NA_1;
                case ASDUTypes.M_ST_NA_1:
                    return ASDUTypes.C_RC_NA_1;
                case ASDUTypes.M_BO_NA_1:
                    return ASDUTypes.C_BO_NA_1;
                case ASDUTypes.M_ME_NA_1:
                    return ASDUTypes.C_SE_NA_1;
                case ASDUTypes.M_ME_NB_1:
                    return ASDUTypes.C_SE_NB_1;
                case ASDUTypes.M_ME_NC_1:
                    return ASDUTypes.C_SE_NC_1;

                case ASDUTypes.M_SP_TB_1:
                    return ASDUTypes.C_SC_TA_1;
                case ASDUTypes.M_DP_TB_1:
                    return ASDUTypes.C_DC_TA_1;
                case ASDUTypes.M_ST_TB_1:
                    return ASDUTypes.C_RC_TA_1;
                case ASDUTypes.M_ME_TD_1:
                    return ASDUTypes.C_SE_TA_1;
                case ASDUTypes.M_ME_TE_1:
                    return ASDUTypes.C_SE_TB_1;
                case ASDUTypes.M_ME_TF_1:
                    return ASDUTypes.C_SE_TC_1;
                case ASDUTypes.M_BO_TB_1:
                    return ASDUTypes.C_BO_TA_1;

                default:
                    return Type;
            }
        }
        public static ASDUTypes converTypeIdBase(ASDUTypes type)
        {
            switch (type)
            {
                case ASDUTypes.M_SP_NA_1:
                case ASDUTypes.M_SP_TA_1:
                case ASDUTypes.M_SP_TB_1:
                    return ASDUTypes.M_SP_NA_1;
                case ASDUTypes.M_DP_NA_1:
                case ASDUTypes.M_DP_TA_1:
                case ASDUTypes.M_DP_TB_1:
                    return ASDUTypes.M_DP_NA_1;
                case ASDUTypes.M_ST_NA_1:
                case ASDUTypes.M_ST_TA_1:
                case ASDUTypes.M_ST_TB_1:
                    return ASDUTypes.M_ST_NA_1;
                case ASDUTypes.M_BO_NA_1:
                case ASDUTypes.M_BO_TA_1:
                case ASDUTypes.M_BO_TB_1:
                    return ASDUTypes.M_BO_NA_1;
                case ASDUTypes.M_ME_NA_1:
                case ASDUTypes.M_ME_TA_1:
                case ASDUTypes.M_ME_TD_1:
                    return ASDUTypes.M_ME_NA_1;
                case ASDUTypes.M_ME_NB_1:
                case ASDUTypes.M_ME_TB_1:
                case ASDUTypes.M_ME_TE_1:
                    return ASDUTypes.M_ME_NB_1;
                case ASDUTypes.M_ME_NC_1:
                case ASDUTypes.M_ME_TC_1:
                case ASDUTypes.M_ME_TF_1:
                    return ASDUTypes.M_ME_NC_1;
                case ASDUTypes.M_IT_NA_1:
                case ASDUTypes.M_IT_NC_1:
                case ASDUTypes.M_IT_TB_1:
                    return ASDUTypes.M_IT_NA_1;
                default:
                    return type;
            }
        }

        public byte Number { get { return (byte)(_Qualifier & 0x7f); } set { _Qualifier = (byte)((value & 0x7f) | (_Qualifier & 0x80)); } }
        public bool SQ
        {
            get
            {
                return (_Qualifier & (1 << 7)) == (1 << 7);
            }
            set
            {
                _Qualifier = (byte)((Convert.ToByte(value) << 7) + (_Qualifier & ((1 << 7) ^ 0xFF)));
            }
        }// Single/sequence
        public CausesOfTrasmission Cause { get { return (CausesOfTrasmission)((byte)_Cause & 0x3f); } set { _Cause = (byte)(((byte)value & 0x3f) | (_Cause & 0xc0)); } }

        public byte CauseRaw { get { return _Cause; }}

        public bool PN
        {
            get
            {
                return (_Cause & (1 << 6)) == (1 << 6);
            }
            set
            {
                _Cause = (byte)((Convert.ToByte(value) << 6) + (_Cause & ((1 << 6) ^ 0xFF)));
            }
        }//  positive/negative confirm
        public bool T
        {
            get
            {
                return (_Cause & (1 << 7)) == (1 << 7);
            }
            set
            {
                _Cause = (byte)((Convert.ToByte(value) << 7) + (_Cause & ((1 << 7) ^ 0xFF)));
            }
        }// no test / test
        public byte OrigAdd { get { return _OriginatorAddress; } set { _OriginatorAddress = value; } }
        public ushort CommAdd { get { return _CommonAddress; } set { _CommonAddress = value; } }

        #endregion

        #region Methods
        public void Init()
        {
            _TypeId = ASDUTypes.invalid;
            _Qualifier = 0;
            _Cause = 0;
            _OriginatorAddress = 0;
            _CommonAddress = 0;
            _timestamp = false;

        }
        public byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.Add((byte)ControlDirectionId);
            retVal.Add(_Qualifier);
            retVal.Add(_Cause);
            retVal.Add(_OriginatorAddress);
            retVal.AddRange(BitConverter.GetBytes(_CommonAddress));
            return retVal.ToArray();
        }


        public void Expand(ref byte[] DataArray, ref int Offset)
        {
            ASDUTypes tmpTypeId = (ASDUTypes)BufferExpand.toByte(ref DataArray, ref Offset);
            _TypeId = converTypeIdBase(tmpTypeId);
            if (tmpTypeId == _TypeId)
                _timestamp = false;
            else
                _timestamp = true;
            _Qualifier = BufferExpand.toByte(ref DataArray, ref Offset);
            _Cause = BufferExpand.toByte(ref DataArray, ref Offset);
            _OriginatorAddress = BufferExpand.toByte(ref DataArray, ref Offset);
            _CommonAddress = (ushort)(BufferExpand.toUInt16(ref DataArray, ref Offset));
        }
        #endregion
    }
    public class iecInfoObj : infObj
    {
        public iecInfoObj()
        {
            Init();
        }
        public iecInfoObj(infObj obj)
        {
            objData = obj;
        }
        public iecInfoObj(ASDUTypes type)
        {
            Init(type);
        }
        public iecInfoObj(ASDUTypes type, TimeSpan TimeTag)
        {
            Init(type, TimeTag);
        }

        public iecInfoObj(ASDUTypes type, DateTime TimeTag)
        {
            Init(type, TimeTag);
        }
        public iecInfoObj(IEC60870_5_104CommJob job, bool sendSelect = false)
        {
            if (job.WriteTimeStamp)
                Init((ASDUTypes)job.ASDUType, DateTime.Now);
            else
                Init((ASDUTypes)job.ASDUType);
            objData.initJobData(job);
            setSelect(sendSelect);
        }

        public iecInfoObj(IEC60870_5_104CommJob job, ASDUTypes type)
        {
            Init(type);
            objData.initJobData(job);
        }

        #region Members
        public infObj objData;
        byte[] Address = new byte[3];
        #endregion

        #region Properties
        public uint informationAddress 
        { 
            get 
            {
                return BufferExpand.toUInt32(Address);
            }
            set 
            {
                Address[0] = (byte)(value & 0xFF);
                Address[1] = (byte)((value >> 8) & 0xFF);
                Address[2] = (byte)((value >> 16) & 0xFF);
            }
        }
        public bool SQ { get; set; }
        #endregion

        #region Methods
        public void Init()
        {
            objData = null;
        }
        public void Init(ASDUTypes type)
        {
            switch(type)
            {
                case ASDUTypes.C_CS_NA_1:
                    objData = new C_CS_NA_1_infObj();
                    break;
                case ASDUTypes.C_IC_NA_1:
                    objData = new C_IC_NA_1_infObj();
                    break;
                case ASDUTypes.C_CI_NA_1:
                    objData = new C_CI_NA_1_infObj();
                    break;
                case ASDUTypes.M_SP_NA_1:
                    objData = new M_SP_NA_1_infObj();
                    break;
                case ASDUTypes.M_SP_TA_1:
                    objData = new M_SP_TA_1_infObj();
                    break;
                case ASDUTypes.M_DP_NA_1:
                    objData = new M_DP_NA_1_infObj();
                    break;
                case ASDUTypes.M_DP_TA_1:
                    objData = new M_DP_TA_1_infObj();
                    break;
                case ASDUTypes.M_ST_NA_1:
                    objData = new M_ST_NA_1_infObj();
                    break;
                case ASDUTypes.M_ST_TA_1:
                    objData = new M_ST_TA_1_infObj();
                    break;
                case ASDUTypes.M_BO_NA_1:
                    objData = new M_BO_NA_1_infObj();
                    break;
                case ASDUTypes.M_BO_TA_1:
                    objData = new M_BO_TA_1_infObj();
                    break;
                case ASDUTypes.M_ME_NA_1:
                    objData = new M_ME_NA_1_infObj();
                    break;
                case ASDUTypes.M_ME_TA_1:
                    objData = new M_ME_TA_1_infObj();
                    break;
                case ASDUTypes.P_ME_NA_1:
                    objData = new P_ME_NA_1_infObj();
                    break;
                case ASDUTypes.M_ME_NB_1:
                    objData = new M_ME_NB_1_infObj();
                    break;
                case ASDUTypes.M_ME_TB_1:
                    objData = new M_ME_TB_1_infObj();
                    break;
                case ASDUTypes.P_ME_NB_1:
                    objData = new P_ME_NB_1_infObj();
                    break;
                case ASDUTypes.M_ME_NC_1:
                    objData = new M_ME_NC_1_infObj();
                    break;
                case ASDUTypes.M_ME_TC_1:
                    objData = new M_ME_TC_1_infObj();
                    break;
                case ASDUTypes.P_ME_NC_1:
                    objData = new P_ME_NC_1_infObj();
                    break;
                case ASDUTypes.M_IT_NA_1:
                    objData = new M_IT_NA_1_infObj();
                    break;
                case ASDUTypes.M_IT_NC_1:
                    objData = new M_IT_NC_1_infObj();
                    break;
                case ASDUTypes.M_ME_TD_1:
                    objData = new M_ME_TD_1_infObj();
                    break;
                case ASDUTypes.M_ME_TE_1:
                    objData = new M_ME_TE_1_infObj();
                    break;
                case ASDUTypes.M_ME_TF_1:
                    objData = new M_ME_TF_1_infObj();
                    break;
                case ASDUTypes.M_IT_TB_1:
                    objData = new M_IT_TB_1_infObj();
                    break;
                case ASDUTypes.C_RC_NA_1:
                    objData = new C_RC_NA_1_infObj();
                    break;
                case ASDUTypes.M_SP_TB_1:
                    objData = new M_SP_TB_1_infObj();
                    break;
                case ASDUTypes.M_DP_TB_1:
                    objData = new M_DP_TB_1_infObj();
                    break;
                case ASDUTypes.M_ST_TB_1:
                    objData = new M_ST_TB_1_infObj();
                    break;
                case ASDUTypes.M_BO_TB_1:
                    objData = new M_BO_TB_1_infObj();
                    break;
                case ASDUTypes.F_SC_NA_1:
                    objData = new SelectFile_infObj();
                    break;
                case ASDUTypes.F_FR_NA_1:
                    objData = new FileReady_infObj();
                    break;
                case ASDUTypes.F_SR_NA_1:
                    objData = new SectionReady_infObj();
                    break;
                case ASDUTypes.F_LS_NA_1:
                    objData = new LastSegment_infObj();
                    break;
                case ASDUTypes.F_AF_NA_1:
                    objData = new AckFile_infObj();
                    break;
                case ASDUTypes.F_SG_NA_1:
                    objData = new Segment_infObj();
                    break;
                case ASDUTypes.F_DR_TA_1:
                    objData = new Directory_infObj();
                    break;
                default:
                    Init();
                    break;
            }

        }

        public void Init(ASDUTypes type, TimeSpan TimeTag)
        {
            switch (type)
            {
                case ASDUTypes.M_SP_NA_1:
                case ASDUTypes.M_SP_TA_1:
                case ASDUTypes.M_SP_TB_1:
                    objData = new M_SP_TA_1_infObj(TimeTag);
                    break;
                case ASDUTypes.M_DP_NA_1:
                case ASDUTypes.M_DP_TA_1:
                case ASDUTypes.M_DP_TB_1:
                    objData = new M_DP_TA_1_infObj(TimeTag);
                    break;
                case ASDUTypes.M_ST_NA_1:
                case ASDUTypes.M_ST_TA_1:
                case ASDUTypes.M_ST_TB_1:
                    objData = new M_ST_TA_1_infObj(TimeTag);
                    break;
                case ASDUTypes.M_BO_NA_1:
                case ASDUTypes.M_BO_TA_1:
                case ASDUTypes.M_BO_TB_1:
                    objData = new M_BO_TA_1_infObj(TimeTag);
                    break;
                case ASDUTypes.M_ME_NA_1:
                case ASDUTypes.M_ME_TA_1:
                case ASDUTypes.M_ME_TD_1:
                    objData = new M_ME_TA_1_infObj(TimeTag);
                    break;
                case ASDUTypes.M_ME_NB_1:
                case ASDUTypes.M_ME_TB_1:
                case ASDUTypes.M_ME_TE_1:
                    objData = new M_ME_TB_1_infObj(TimeTag);
                    break;
                case ASDUTypes.M_ME_NC_1:
                case ASDUTypes.M_ME_TC_1:
                case ASDUTypes.M_ME_TF_1:
                    objData = new M_ME_TC_1_infObj(TimeTag);
                    break;
                case ASDUTypes.M_IT_NA_1:
                case ASDUTypes.M_IT_NC_1:
                case ASDUTypes.M_IT_TB_1:
                    objData = new M_IT_NC_1_infObj(TimeTag);
                    break;
                default:
                    Init(type);
                    break;
            }
        }
        public void Init(ASDUTypes type, DateTime TimeTag)
        {
            switch (type)
            {
                case ASDUTypes.M_ME_NC_1:
                case ASDUTypes.M_ME_TC_1:
                case ASDUTypes.M_ME_TF_1:
                    objData = new M_ME_TF_1_infObj(TimeTag);
                    break;
                case ASDUTypes.M_IT_NA_1:
                case ASDUTypes.M_IT_NC_1:
                case ASDUTypes.M_IT_TB_1:
                    objData = new M_IT_TB_1_infObj(TimeTag);
                    break;
                case ASDUTypes.M_ME_NA_1:
                case ASDUTypes.M_ME_TA_1:
                case ASDUTypes.M_ME_TD_1:
                    objData = new M_ME_TD_1_infObj(TimeTag);
                    break;
                case ASDUTypes.M_ME_NB_1:
                case ASDUTypes.M_ME_TB_1:
                case ASDUTypes.M_ME_TE_1:
                    objData = new M_ME_TE_1_infObj(TimeTag);
                    break;

                case ASDUTypes.M_SP_NA_1:
                case ASDUTypes.M_SP_TA_1:
                case ASDUTypes.M_SP_TB_1:
                    objData = new M_SP_TB_1_infObj();
                    break;
                case ASDUTypes.M_DP_NA_1:
                case ASDUTypes.M_DP_TA_1:
                case ASDUTypes.M_DP_TB_1:
                    objData = new M_DP_TB_1_infObj();
                    break;
                case ASDUTypes.M_ST_NA_1:
                case ASDUTypes.M_ST_TA_1:
                case ASDUTypes.M_ST_TB_1:
                    objData = new M_ST_TB_1_infObj();
                    break;
                case ASDUTypes.M_BO_NA_1:
                case ASDUTypes.M_BO_TA_1:
                case ASDUTypes.M_BO_TB_1:
                    objData = new M_BO_TB_1_infObj();
                    break;

                default:
                    Init(type);
                    break;
            }
        }

        public override ASDUTypes Type()
        {
            if (objData == null)
                return ASDUTypes.invalid;
            return objData.Type();
        }
        public override ASDUTypes basicType()
        {
            if (objData == null)
                return ASDUTypes.invalid;
            return objData.basicType();
        }
        public override byte[] getValue()
        {
            if (objData == null)
                return new byte[0];
            return objData.getValue();
        }
        public override void setValue(byte[] DataArray)
        {
            objData.setValue(DataArray);
        }
        public override DateTime getTimestamp()
        {
            if (objData == null)
                return default(DateTime);
            return objData.getTimestamp();
        }
        public override byte getQuality()
        {
            if (objData == null)
                return 0;
            return objData.getQuality();
        }
        public override void setQuality(byte qos)
        {
            if (objData != null)
                objData.setQuality(qos);
        }
        public override void setSelect(bool select)
        {
            if (objData != null)
                objData.setSelect(select);
        }
        public override byte[] Pack()
        {
            if (objData == null)
                return new byte[0];
            List<byte> retVal = new List<byte>();
            if (!SQ)
                retVal.AddRange(Address);
            retVal.AddRange(objData.Pack());
            return retVal.ToArray();
        }

        public override void Expand(ref byte[] DataArray, ref int Offset)
        {
            if (objData != null)
            {
                if (!SQ)
                    Address = BufferExpand.toArray(ref DataArray, ref Offset, 3);
                objData.Expand(ref DataArray, ref Offset);

            }
        }
        public override void initJobData(IEC60870_5_104CommJob job)
        {
            objData.initJobData(job);
        }
        #endregion
    }
    public class APCI : IPackable, IExpandable
    {
        public const byte START = 0x68;
        public const byte Size = 6;
        private const byte SizeInAsdu = 4;
        public APCI()
        {
            Init();
        }
        public APCI(ref byte[] DataArray, ref int Offset)
        {
            Expand(ref DataArray, ref  Offset);
        }
        #region member
 
        private byte _start;
        private byte _length;
        private ushort _NS;
        private ushort _NR;
        
        #endregion

        #region Properties
        public byte start { get { return _start; } }
        public byte AsduLength { get { return (byte)(_length - SizeInAsdu); } set { _length = (byte)(value + SizeInAsdu); } }
        public ushort SendSequenceNumber { get { return (ushort)(((_NS & 1) == 0) ? _NS >> 1 : _NS >> 2); } set { _NS = (ushort)(value << 1); } }
        public ushort ReceiveSequenceNumber { get { return (ushort)(_NR >> 1); } set { _NR = (ushort)(value << 1); } }
        public Control CONTROL { get { return (Control)_NS; } set { _NS = (ushort)value; } }
        public CFFormats CFFormat
        {
            get
            {
                if ((_NS & 1) == 0)
                    return CFFormats.I;
                return (CFFormats)(_NS & 3);
            }
            set
            {
                if (((byte)value & 1) == 0)
                    _NS &= 0xfe;
                else
                    _NS = (byte)((_NS & 0xfc) + (byte)value);
            }
        }
        public bool TESTFR_con
        {
            get
            {
                return (_NS & (1 << 7)) == (1 << 7);
            }
            set
            {
                _NS = (byte)((Convert.ToByte(value) << 7) + (_NS & ((1 << 7) ^ 0xFF)));
            }
        }
        public bool TESTFR_act
        {
            get
            {
                return (_NS & (1 << 6)) == (1 << 6);
            }
            set
            {
                _NS = (byte)((Convert.ToByte(value) << 6) + (_NS & ((1 << 6) ^ 0xFF)));
            }
        }
        public bool STOPDT_con
        {
            get
            {
                return (_NS & (1 << 5)) == (1 << 5);
            }
            set
            {
                _NS = (ushort)((Convert.ToByte(value) << 5) + (_NS & ((1 << 5) ^ 0xFF)));
            }
        }
        public bool STOPDT_act
        {
            get
            {
                return (_NS & (1 << 4)) == (1 << 4);
            }
            set
            {
                _NS = (ushort)((Convert.ToByte(value) << 4) + (_NS & ((1 << 4) ^ 0xFF)));
            }
        }
        public bool STARTDT_con
        {
            get
            {
                return (_NS & (1 << 3)) == (1 << 3);
            }
            set
            {
                _NS = (ushort)((Convert.ToByte(value) << 3) + (_NS & ((1 << 3) ^ 0xFF)));
            }
        }
        public bool STARTDT_act
        {
            get
            {
                return (_NS & (1 << 2)) == (1 << 2);
            }
            set
            {
                _NS = (ushort)((Convert.ToByte(value) << 2) + (_NS & ((1 << 2) ^ 0xFF)));
            }
        }
        #endregion

        #region Methods
        public void Init()
        {
            _start = START;
            _length = SizeInAsdu;
            _NS = 0;
            _NR = 0;
        }
        public byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.Add(_start);
            retVal.Add(_length);
            retVal.AddRange(BitConverter.GetBytes(_NS));
            retVal.AddRange(BitConverter.GetBytes(_NR));

            return retVal.ToArray();
        }

        public void Expand(ref byte[] DataArray, ref int Offset)
        {
            _start = BufferExpand.toByte(ref DataArray, ref Offset);
            _length = BufferExpand.toByte(ref DataArray, ref Offset);
            _NS = BufferExpand.toUInt16(ref DataArray, ref Offset);
            _NR = BufferExpand.toUInt16(ref DataArray, ref Offset);
        }
        #endregion
    }
    public class ASDU : IPackable, IExpandable
    {
        public ASDU()
        {
            Init();
        }
        public ASDU(ASDUTypes type, CausesOfTrasmission cause, bool SQ, bool timestamp)
        {
            Init();
            _UnitId.SQ = SQ;
            _UnitId.Cause = cause;
            _UnitId.TypeIdBase = type;
            _UnitId.Timestamp = timestamp;
        }
        public ASDU(ref byte[] DataArray, ref int Offset)
        {
            Expand(ref DataArray, ref  Offset);
        }

        #region member
        private UnitId _UnitId;
        private List<iecInfoObj> _ObjList;
        byte[] Address = new byte[3];
        #endregion

        #region Properties
        public UnitId UnitId { get { return _UnitId; } set { _UnitId = value; } }
        public uint informationAddress
        {
            get
            {
             if (_UnitId.SQ)
                 return BufferExpand.toUInt32(Address);
             else if (_ObjList.Count() >0)
                 return _ObjList[0].informationAddress;
             else
                 return 0;
            }
            set
            {
                Address[0] = (byte)(value & 0xFF);
                Address[1] = (byte)((value >> 8) & 0xFF);
                Address[2] = (byte)((value >> 16) & 0xFF);
            }
        }

        public bool CauseOfTransmissionPN { get { return ((_UnitId.CauseRaw & (0x40)) == (0x40) ? true : false); } }
       
        public List<iecInfoObj> ObjList { get { return _ObjList; }  }
         #endregion

        #region Methods
        public void Add(iecInfoObj Obj)
        {
            _ObjList.Add(Obj);
            _UnitId.Number++;
        }
        public void Init()
        {
            _UnitId = new UnitId();
            _ObjList = new List<iecInfoObj>();
        }
        public byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(_UnitId.Pack());
            if (_UnitId.SQ)
                retVal.AddRange(Address);
            foreach (iecInfoObj obj in _ObjList)
            {
                obj.SQ = _UnitId.SQ;
                retVal.AddRange(obj.Pack());
            }

            return retVal.ToArray();
        }

        public void Expand(ref byte[] DataArray,ref int Offset)
        {
            _UnitId.Expand(ref DataArray, ref Offset);
            uint listAddress = 0;
            if (_UnitId.SQ)
            {
                Address = BufferExpand.toArray(ref DataArray, ref Offset, 3);
                listAddress = BufferExpand.toUInt32(Address);

            }
            for (byte index = 0; index < _UnitId.Number; index++)
            {
                iecInfoObj obj = new iecInfoObj(_UnitId.TypeId);
                obj.SQ = _UnitId.SQ;
                obj.Expand(ref DataArray, ref Offset);
                if (_UnitId.SQ)
                    obj.informationAddress = listAddress++;
                _ObjList.Add(obj);
            }
        }
        #endregion
    }
    public class APDU : IPackable, IExpandable
    {
        public APDU()
        {
            Init();
        }
        public APDU(Control cause)
        {
            Init();
            _Apci.CONTROL = cause;
        }
        public APDU(ASDUTypes type, CausesOfTrasmission cause, bool SQ, bool timestamp = false)
        {
            Init();
            _Asdu = new ASDU(type, cause, SQ , timestamp);
        }
        public APDU(ref byte[] DataArray, ref int Offset)
        {
            Expand(ref DataArray, ref  Offset);
        }

        #region member
        private APCI _Apci;
        private ASDU _Asdu;
        #endregion

        #region Properties
        public APCI Apci { get { return _Apci; } set { _Apci = value; } }
        public ASDU Asdu { get { return _Asdu; } set { _Asdu = value; } }
        #endregion

        #region Methods
        public void Init()
        {
            _Apci = new APCI();
        }
        public byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            if (_Asdu != null)
            {
                byte[] AsduBuffer = _Asdu.Pack();
                _Apci.AsduLength = (byte)AsduBuffer.Length;
                retVal.AddRange(_Apci.Pack());
                retVal.AddRange(AsduBuffer);
            }
            else
                retVal.AddRange(_Apci.Pack());

            return retVal.ToArray();
        }

        public void Expand(ref byte[] DataArray, ref int Offset)
        {
            Init();
            _Apci.Expand(ref DataArray, ref Offset);
            if (_Apci.CFFormat == CFFormats.I)
            {
                _Asdu = new ASDU();
                _Asdu.Expand(ref DataArray, ref Offset);
            }

        }
        #endregion
    }


    public static class APDUFactory
    {
        public static APDU StartDT()
        {
            return new APDU(Control.STARTDTACT);
        }
        public static APDU StartDTConf()
        {
            return new APDU(Control.STARTDTCON);
        }
        public static APDU Test()
        {
            return new APDU(Control.TESTFRACT);
        }
        public static APDU TestConf()
        {
            return new APDU(Control.TESTFRCON);
        }
        public static APDU StopDT()
        {
            return new APDU(Control.STOPDTACT);
        }
        public static APDU StopDTconf()
        {
            return new APDU(Control.STOPDTCON);
        }
        public static APDU Supervisory()
        {
            return new APDU(Control.SUPERVISORY);
        }
        public static APDU SType()
        {
            APDU Apdu = new APDU();
            Apdu.Apci.CFFormat = CFFormats.S;
            return Apdu;
        }

 
        public static APDU ClockSync()
        {
            APDU Apdu = new APDU(ASDUTypes.C_CS_NA_1, CausesOfTrasmission.activation, false);
            iecInfoObj Obj = new iecInfoObj(new C_CS_NA_1_infObj());
            Apdu.Asdu.Add(Obj);
            return Apdu;
        }
        public static APDU GeneralInterrogation(CausesOfTrasmission QOI)
        {
            APDU Apdu = new APDU(ASDUTypes.C_IC_NA_1, CausesOfTrasmission.activation, false);
            iecInfoObj Obj = new iecInfoObj(new C_IC_NA_1_infObj(QOI));
            Apdu.Asdu.Add(Obj);
            return Apdu;
        }
        public static APDU CounterInterrogation(QCCRequests Request, QCCFreezes Freeze)
        {
            APDU Apdu = new APDU(ASDUTypes.C_CI_NA_1, CausesOfTrasmission.activation, false);
            iecInfoObj Obj = new iecInfoObj(new C_CI_NA_1_infObj(Request, Freeze));
            Apdu.Asdu.Add(Obj);
            return Apdu;
        }
        public static APDU confirmedWriteJob(IEC60870_5_104CommJob job, bool sendSelect = false)
        {
            APDU Apdu = new APDU((ASDUTypes)job.ASDUType , CausesOfTrasmission.activation, false, job.WriteTimeStamp);
            Apdu.Asdu.informationAddress = job.StartAddCtrl;
            iecInfoObj OBJ = new iecInfoObj(job, sendSelect);
            OBJ.informationAddress = job.StartAddCtrl;
            Apdu.Asdu.Add(OBJ);

            return Apdu;
        }

        public static APDU SelectFileJob(IEC60870_5_104CommJob job)
        {
            // ASDU parameters for selecting a file
            ASDUTypes asduType = (ASDUTypes)job.ASDUType;
            CausesOfTrasmission causeOfTransmission = CausesOfTrasmission.file_transfer;
            if (job.ASDUType == ASDUSelectableTypes.ReadDirectoryContents)
            {
                // Set the parameters of the ASDU for reading directory contents
                asduType = ASDUTypes.F_SC_NA_1;
                causeOfTransmission = CausesOfTrasmission.request_or_requested;
            }
            APDU Apdu = new APDU(asduType, causeOfTransmission, false, false);
            if (job.ASDUType != ASDUSelectableTypes.ReadDirectoryContents)
            {
                UInt16 fileName = 0;
                job.getFileNameVariable(ref fileName);
                Apdu.Asdu.informationAddress = (uint)fileName;
                iecInfoObj OBJ = new iecInfoObj(job);
                OBJ.informationAddress = (uint)fileName;
                Apdu.Asdu.Add(OBJ);
            }
            else
            {
                UInt16 fileName = 0;
                job.getFileNameVariable(ref fileName);
                Apdu.Asdu.informationAddress = (uint)fileName;
                iecInfoObj OBJ = new iecInfoObj(job, ASDUTypes.F_SC_NA_1);
                OBJ.informationAddress = (uint)fileName;
                Apdu.Asdu.Add(OBJ);
            }

            return Apdu;
        }

        public static APDU AcknowledgeFile(IEC60870_5_104CommJob job)
        {
            APDU Apdu = new APDU(ASDUTypes.F_AF_NA_1, CausesOfTrasmission.file_transfer, false, false);
            UInt16 fileName = 0;
            job.getFileNameVariable(ref fileName);
            Apdu.Asdu.informationAddress = (uint)fileName;
            iecInfoObj OBJ = new iecInfoObj(job, ASDUTypes.F_AF_NA_1);
            OBJ.informationAddress = (uint)fileName;
            Apdu.Asdu.Add(OBJ);

            return Apdu;
        }
    }
    public static class APDUValidator
    {
        public static bool StartDTConf(ref APDU pdu)
        {
            return (pdu.Apci.CONTROL == Control.STARTDTCON);
        }
        public static bool StartDTAct(ref APDU pdu)
        {
            return (pdu.Apci.CONTROL == Control.STARTDTACT);
        }
        public static bool TestConf(ref APDU pdu)
        {
            return (pdu.Apci.CONTROL == Control.TESTFRCON);
        }
        public static bool TestAct(ref APDU pdu)
        {
            return (pdu.Apci.CONTROL == Control.TESTFRACT);
        }
        public static bool StopDTAct(ref APDU pdu)
        {
            return (pdu.Apci.CONTROL == Control.STOPDTACT);
        }
        public static bool SType(ref APDU pdu)
        {
            return (pdu.Apci.CFFormat == CFFormats.S);
        }
        public static bool isConfirmation(ref ReceiveItem receiveItem)
        {
            return (receiveItem.Apdu.Asdu != null && (receiveItem.Apdu.Asdu.UnitId.Cause == CausesOfTrasmission.activation_confirmation || receiveItem.Apdu.Asdu.UnitId.Cause == CausesOfTrasmission.deactivation_confirmation));
        }
        public static bool isEnd(ref ReceiveItem receiveItem)
        {
            return (receiveItem.Apdu.Asdu != null && receiveItem.Apdu.Asdu.UnitId.Cause == CausesOfTrasmission.activation_termination);
        }
        public static bool ClockSyncConf(ref ReceiveItem receiveItem)
        {
            return (receiveItem.Apdu.Asdu != null && receiveItem.Apdu.Asdu.UnitId.TypeIdBase == ASDUTypes.C_CS_NA_1 && isConfirmation(ref receiveItem));
        }
        public static bool GeneralInterrogationConf(ref ReceiveItem receiveItem)
        {
            return (receiveItem.Apdu.Asdu != null && receiveItem.Apdu.Asdu.UnitId.TypeIdBase == ASDUTypes.C_IC_NA_1 && isConfirmation(ref receiveItem));
        }
        public static bool GeneralInterrogationEnd(ref ReceiveItem receiveItem)
        {
            return (receiveItem.Apdu.Asdu != null && receiveItem.Apdu.Asdu.UnitId.TypeIdBase == ASDUTypes.C_IC_NA_1 && isEnd(ref receiveItem));
        }
        public static bool CounterInterrogationConf(ref ReceiveItem receiveItem)
        {
            return (receiveItem.Apdu.Asdu != null && receiveItem.Apdu.Asdu.UnitId.TypeIdBase == ASDUTypes.C_CI_NA_1 && isConfirmation(ref receiveItem));
        }
        public static bool CounterInterrogationEnd(ref ReceiveItem receiveItem)
        {
            return (receiveItem.Apdu.Asdu != null && receiveItem.Apdu.Asdu.UnitId.TypeIdBase == ASDUTypes.C_CI_NA_1 && isEnd(ref receiveItem));
        }
        public static bool ASDUInputTypes(ref APDU pdu, out List<iecInfoObj> InfoObjList)
        {
            InfoObjList = new List<iecInfoObj>();
            if (pdu.Asdu == null)
                return false;
            switch (pdu.Asdu.UnitId.TypeIdBase)
            {
                case ASDUTypes.M_SP_NA_1:
                case ASDUTypes.M_DP_NA_1:
                case ASDUTypes.M_ST_NA_1:
                case ASDUTypes.M_BO_NA_1:
                case ASDUTypes.M_ME_NA_1:
                case ASDUTypes.M_ME_NB_1:
                case ASDUTypes.M_ME_NC_1:
                case ASDUTypes.M_IT_NA_1:
                    InfoObjList.AddRange(pdu.Asdu.ObjList);
                    return InfoObjList.Count() != 0;
                default:
                    return false;
            }
        }

        public static bool ASDUSpontaneousInputTypes(ref APDU pdu, out List<iecInfoObj> InfoObjList)
        {
            if (pdu.Asdu == null ||
                (
                pdu.Asdu.UnitId.Cause != CausesOfTrasmission.spontaneous &&
                pdu.Asdu.UnitId.Cause != CausesOfTrasmission.periodic_cyclic &&
                pdu.Asdu.UnitId.Cause != CausesOfTrasmission.requested_by_general_counter_request &&
                pdu.Asdu.UnitId.Cause != CausesOfTrasmission.interrogated_by_station_interrogation &&
                pdu.Asdu.UnitId.Cause != CausesOfTrasmission.background_scan &&
                pdu.Asdu.UnitId.Cause != CausesOfTrasmission.return_information_caused_by_a_remote_command &&
                pdu.Asdu.UnitId.Cause != CausesOfTrasmission.return_information_caused_by_a_local_command)
                )
            {
                InfoObjList = new List<iecInfoObj>();
                return false;
            }
            else
                return ASDUInputTypes(ref pdu, out InfoObjList);
        }

        public static bool isWriteResponse(ref ReceiveItem receiveItem, IEC60870_5_104CommJob job)
        {
            return (receiveItem.Apdu.Asdu != null && receiveItem.Apdu.Asdu.UnitId.TypeIdBase == UnitId.ControlDirection((ASDUTypes)job.ASDUType,job.WriteTimeStamp) && isConfirmation(ref receiveItem));
        }

        public static bool isFileSelectResponse(ref ReceiveItem receiveItem, IEC60870_5_104CommJob job)
        {
            return (receiveItem.Apdu.Asdu != null && (receiveItem.Apdu.Asdu.UnitId.TypeIdBase == ASDUTypes.F_FR_NA_1));
        }

        public static bool isFileRequestResponse(ref ReceiveItem receiveItem, IEC60870_5_104CommJob job)
        {
            return (receiveItem.Apdu.Asdu != null && (receiveItem.Apdu.Asdu.UnitId.TypeIdBase == ASDUTypes.F_SR_NA_1));
        }

        public static bool isSectionRequestResponse(ref ReceiveItem receiveItem, IEC60870_5_104CommJob job)
        {
            return (receiveItem.Apdu.Asdu != null && ((receiveItem.Apdu.Asdu.UnitId.TypeIdBase == ASDUTypes.F_SG_NA_1) || (receiveItem.Apdu.Asdu.UnitId.TypeIdBase == ASDUTypes.F_LS_NA_1)));
        }

        public static bool isDirectoryResponse(ref ReceiveItem receiveItem, IEC60870_5_104CommJob job)
        {
            return (receiveItem.Apdu.Asdu != null && (receiveItem.Apdu.Asdu.UnitId.TypeIdBase == ASDUTypes.F_DR_TA_1));
        }

        public static bool GetError(ref ReceiveItem receiveItem, out IEC60870_5_104ErrorCodes error)
        {
            error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorNoError;
            if (receiveItem.Apdu.Asdu == null)
                return false;

            switch (receiveItem.Apdu.Asdu.UnitId.Cause)
            {
                case CausesOfTrasmission.unknown_type_identification:
                    error = IEC60870_5_104ErrorCodes.unknown_type_identification;
                    break;
                case CausesOfTrasmission.unknown_cause_of_transmission:
                    error = IEC60870_5_104ErrorCodes.unknown_cause_of_transmission;
                    break;
                case CausesOfTrasmission.unknown_common_address_of_ASDU:
                    error = IEC60870_5_104ErrorCodes.unknown_common_address_of_ASDU;
                    break;
                case CausesOfTrasmission.unknown_information_object_address:
                    error = IEC60870_5_104ErrorCodes.unknown_information_object_address;
                    break;
                default:
                    if (receiveItem.Apdu.Asdu.CauseOfTransmissionPN)
                    {
                        error = IEC60870_5_104ErrorCodes.cause_of_transmission_pn;
                    }
                    break;
            }

            return error !=(IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorNoError;
        }
        public static bool TestCommonAddress(ref ReceiveItem receiveItem, ushort CommonAddress)
        {
            if (receiveItem.Apdu.Asdu == null)
            {
                System.Diagnostics.Debug.WriteLine("TestCommonAddress - receiveItem.Apdu.Asdu is null");
                return false;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(String.Format("TestCommonAddress - CommonAddress: {0} - receiveItem.Apdu.Asdu.UnitId.CommAdd: {1}", CommonAddress, receiveItem.Apdu.Asdu.UnitId.CommAdd));
                return CommonAddress == receiveItem.Apdu.Asdu.UnitId.CommAdd;
            }
        }

        public static bool FileReadyQualifier(ref ReceiveItem receiveItem, out IEC60870_5_104ErrorCodes error, out uint fileLength)
        {
            error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorNoError;
            fileLength = 0;
            if ((receiveItem.Apdu == null) || (receiveItem.Apdu.Asdu == null))
            {
                error = IEC60870_5_104ErrorCodes.ErrorFileTransferMalformedPacket;
                return (false);
            }
            List<iecInfoObj> infoObjList = new List<iecInfoObj>();
            infoObjList.AddRange(receiveItem.Apdu.Asdu.ObjList);
            if(infoObjList.Count < 1)
            {
                error = IEC60870_5_104ErrorCodes.ErrorFileTransferMalformedPacket;
                return (false);
            }
            FileReady_infObj receivedInformationObject = (FileReady_infObj)infoObjList[0].objData;
            if((receivedInformationObject.FRQ & 0x80) > 0)
            {
                error = IEC60870_5_104ErrorCodes.ErrorFileTransferFileReadyNegativeConfirm;
                return (false);
            }
            fileLength = receivedInformationObject.FileLength;
            return (true);
        }

        public static bool SectionReadyQualifier(ref ReceiveItem receiveItem, out IEC60870_5_104ErrorCodes error, out uint sectionLength)
        {
            error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorNoError;
            sectionLength = 0;
            if ((receiveItem.Apdu == null) || (receiveItem.Apdu.Asdu == null))
            {
                error = IEC60870_5_104ErrorCodes.ErrorFileTransferMalformedPacket;
                return (false);
            }
            List<iecInfoObj> infoObjList = new List<iecInfoObj>();
            infoObjList.AddRange(receiveItem.Apdu.Asdu.ObjList);
            if (infoObjList.Count < 1)
            {
                error = IEC60870_5_104ErrorCodes.ErrorFileTransferMalformedPacket;
                return (false);
            }
            SectionReady_infObj receivedInformationObject = (SectionReady_infObj)infoObjList[0].objData;
            if ((receivedInformationObject.SRQ & 0x80) > 0)
            {
                error = IEC60870_5_104ErrorCodes.ErrorFileTransferSectionReadyNegativeConfirm;
                return (false);
            }
            sectionLength = receivedInformationObject.SectionLength;
            return (true);
        }

        public static bool Segment(ref ReceiveItem receiveItem, out IEC60870_5_104ErrorCodes error, out byte segmentType, ref uint segmentLength, ref byte[] segmentData, ref byte checksum)
        {
            // Initializations
            error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorNoError;
            segmentLength = 0;
            checksum = 0;
            segmentData = null;
            segmentType = (byte)SegmentType.notUsed;

            // General validation
            if ((receiveItem.Apdu == null) || (receiveItem.Apdu.Asdu == null))
            {
                error = IEC60870_5_104ErrorCodes.ErrorFileTransferMalformedPacket;
                return (false);
            }
            List<iecInfoObj> infoObjList = new List<iecInfoObj>();
            infoObjList.AddRange(receiveItem.Apdu.Asdu.ObjList);
            if (infoObjList.Count < 1)
            {
                error = IEC60870_5_104ErrorCodes.ErrorFileTransferMalformedPacket;
                return (false);
            }

            // Check the message type
            switch(receiveItem.Apdu.Asdu.UnitId.TypeIdBase)
            {
                // Data of a segment of a section
                case ASDUTypes.F_SG_NA_1:
                    segmentType = (byte)SegmentType.dataSegment;
                    break;

                // Last segment of a section or last section of a file
                case ASDUTypes.F_LS_NA_1:
                    {
                        LastSegment_infObj receivedInformationObject = (LastSegment_infObj)infoObjList[0].objData;
                        switch(receivedInformationObject.LSQ)
                        {
                            case (byte)LastSectionSegmentQualifier.fileTransferWithoutDeactivation:
                            case (byte)LastSectionSegmentQualifier.fileTransferWithDeactivation:
                                segmentType = (byte)SegmentType.lastSection;
                                checksum = receivedInformationObject.Checksum;
                                break;
                            case (byte)LastSectionSegmentQualifier.sectionTransferWithoutDeactivation:
                            case (byte)LastSectionSegmentQualifier.sectionTransferWithDeactivation:
                                segmentType = (byte)SegmentType.lastSegment;
                                checksum = receivedInformationObject.Checksum;
                                break;
                        }
                    }
                    break;
            }

            // Parse data
            switch(segmentType)
            {
                case (byte)SegmentType.dataSegment:
                    return (DataSegment(ref receiveItem, out error, ref segmentLength, ref segmentData));
                case (byte)SegmentType.lastSection:
                case (byte)SegmentType.lastSegment:
                    return (LastSegment(ref receiveItem, out error, ref checksum));
                default:
                    error = IEC60870_5_104ErrorCodes.ErrorFileTransferMalformedPacket;
                    return (false);
            }
        }

        public static bool DataSegment(ref ReceiveItem receiveItem, out IEC60870_5_104ErrorCodes error, ref uint segmentLength, ref byte[] segmentData)
        {
            // Initializations
            error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorNoError;
            segmentLength = 0;
            segmentData = null;

            // General validation
            List<iecInfoObj> infoObjList = new List<iecInfoObj>();
            infoObjList.AddRange(receiveItem.Apdu.Asdu.ObjList);
            if (infoObjList.Count < 1)
            {
                error = IEC60870_5_104ErrorCodes.ErrorFileTransferMalformedPacket;
                return (false);
            }

            // Get segment data
            Segment_infObj receivedInformationObject = (Segment_infObj)infoObjList[0].objData;
            segmentLength = receivedInformationObject.SegmentLength;
            if(segmentLength == 0)
            {
                error = IEC60870_5_104ErrorCodes.ErrorFileTransferMalformedPacket;
                return (false);
            }
            segmentData = new byte[segmentLength];
            receivedInformationObject.SegmentBytes.CopyTo(segmentData, 0);

            return (true);
        }

        public static bool LastSegment(ref ReceiveItem receiveItem, out IEC60870_5_104ErrorCodes error, ref byte checksum)
        {
            // Initializations
            error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorNoError;
            checksum = 0;

            // General validation
            List<iecInfoObj> infoObjList = new List<iecInfoObj>();
            infoObjList.AddRange(receiveItem.Apdu.Asdu.ObjList);
            if (infoObjList.Count < 1)
            {
                error = IEC60870_5_104ErrorCodes.ErrorFileTransferMalformedPacket;
                return (false);
            }

            // Get section/file checksum
            LastSegment_infObj receivedInformationObject = (LastSegment_infObj)infoObjList[0].objData;
            checksum = receivedInformationObject.Checksum;

            return (true);
        }

        public static bool Directory(ref ReceiveItem receiveItem, out IEC60870_5_104ErrorCodes error, ref uint directoryLength, ref byte[] directoryData, ref uint directoryNumberOfElements, ref bool directoryComplete)
        {
            // Initializations
            error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorNoError;
            directoryLength = 0;
            directoryData = null;
            directoryNumberOfElements = 0;
            directoryComplete = false;

            // General validation
            List<iecInfoObj> infoObjList = new List<iecInfoObj>();
            infoObjList.AddRange(receiveItem.Apdu.Asdu.ObjList);
            if (infoObjList.Count < 1)
            {
                error = IEC60870_5_104ErrorCodes.ErrorDirectoryMalformedPacket;
                return (false);
            }

            // Get directory data
            Directory_infObj receivedInformationObject = (Directory_infObj)infoObjList[0].objData;
            directoryLength = receivedInformationObject.DirectoryLength;
            if(directoryLength < 1)
            {
                error = IEC60870_5_104ErrorCodes.ErrorDirectoryMalformedPacket;
                return (false);
            }
            directoryNumberOfElements = receivedInformationObject.DirectoryNumberOfElements;
            if (directoryNumberOfElements < 1)
            {
                error = IEC60870_5_104ErrorCodes.ErrorDirectoryMalformedPacket;
                return (false);
            }
            directoryData = new byte[directoryLength];
            receivedInformationObject.DirectoryContents.CopyTo(directoryData, 0);
            directoryComplete = receivedInformationObject.IsDirectoryComplete;

            return (true);
        }
    }

    /// <summary>   Communication protocol of IEC60870_5_104 driver. </summary>
    public class IEC60870_5_104Protocol
    {
        #region const

        public const int MAX_DATA_BYTES = 0x400;
        public const int MAX_INFORMATION_OBJECT_ADDRESS = 16777215;
        public const int MIN_INFORMATION_OBJECT_ADDRESS = 0;
        public const char FILE_INFO_SEPARATOR = ',';

        public static bool isChecksumCorrect(List<byte> dataBytes, byte checksum)
        {
            byte dataChecksum = 0;
            int dataByteNumber = dataBytes.Count();
            for (int i=0; i<dataByteNumber; i++)
            {
                dataChecksum += dataBytes[i];
            }
            return (dataChecksum == checksum);
        }

        public static bool isASDUTypesWritable(IEC60870_5_104DynTagSettings TagSettings)
        {
            return isASDUTypesWritable(TagSettings.ASDUType);
        }

        public static bool isASDUTypesWritable(ASDUSelectableTypes ASDUType)
        {
            switch(ASDUType)
            {
                case ASDUSelectableTypes.SinglePoint:
                case ASDUSelectableTypes.DoublePoint:
                case ASDUSelectableTypes.BitString:
                case ASDUSelectableTypes.NormalizedMeasurand:
                case ASDUSelectableTypes.ScaledMeasurand:
                case ASDUSelectableTypes.FloatingPointMeasurand:
                case ASDUSelectableTypes.ParamNormalizedMeasurand:
                case ASDUSelectableTypes.ParamScaledMeasurand:
                case ASDUSelectableTypes.ParamFloatingPointMeasurand:
                case ASDUSelectableTypes.RegulatingStep:
                case ASDUSelectableTypes.ClockSynchronization:
                case ASDUSelectableTypes.GeneralInterrogation:
                case ASDUSelectableTypes.UploadFile:
                    return true;
                default:
                    return false;
            }
        }
        public static bool isASDUTypesReadable(IEC60870_5_104DynTagSettings TagSettings)
        {
            return isASDUTypesReadable(TagSettings.ASDUType);
        }
        public static bool isASDUTypesReadable(ASDUSelectableTypes ASDUType)
        {
            switch (ASDUType)
            {
                case ASDUSelectableTypes.SinglePoint:
                case ASDUSelectableTypes.DoublePoint:
                case ASDUSelectableTypes.StepPosition:
                case ASDUSelectableTypes.BitString:
                case ASDUSelectableTypes.NormalizedMeasurand:
                case ASDUSelectableTypes.ScaledMeasurand:
                case ASDUSelectableTypes.FloatingPointMeasurand:
                case ASDUSelectableTypes.IntegratedTotals:
                case ASDUSelectableTypes.ReadDirectoryContents:
                    return true;
                default:
                    return false;
            }
        }

        public static bool ASDUTypesRequiresFileName(IEC60870_5_104DynTagSettings TagSettings)
        {
            switch (TagSettings.ASDUType)
            {
                case ASDUSelectableTypes.UploadFile:
                case ASDUSelectableTypes.ReadDirectoryContents:
                    return true;
                default:
                    return false;
            }
        }
        public static bool ASDUTypesRequiresFileName(IEC60870_5_104CommJob job)
        {
            switch (job.ASDUType)
            {
                case ASDUSelectableTypes.UploadFile:
                case ASDUSelectableTypes.ReadDirectoryContents:
                    return true;
                default:
                    return false;
            }
        }

        public static bool ASDUTypesRequiresCQ(IEC60870_5_104DynTagSettings TagSettings)
        {
            switch (TagSettings.ASDUType)
            {
                case ASDUSelectableTypes.SinglePoint:
                case ASDUSelectableTypes.DoublePoint:
                    if (TagSettings.TagLinkType != (int)LinkType.Input)
                        return true;
                    else
                        return false;
                case ASDUSelectableTypes.RegulatingStep:
                    return true;
                default:
                    return false;
            }
        }
        public static bool ASDUTypesRequiresCQ(IEC60870_5_104CommJob job)
        {
            switch (job.ASDUType)
            {
                case ASDUSelectableTypes.SinglePoint:
                case ASDUSelectableTypes.DoublePoint:
                    if (job.Type != LinkType.Input)
                        return true;
                    else
                        return false;
                case ASDUSelectableTypes.RegulatingStep:
                    return true;
                default:
                    return false;
            }
        }
        public static bool ASDUTypesRequiresPQ(IEC60870_5_104DynTagSettings TagSettings)
        {
            switch (TagSettings.ASDUType)
            {
                case ASDUSelectableTypes.ParamNormalizedMeasurand:
                case ASDUSelectableTypes.ParamScaledMeasurand:
                case ASDUSelectableTypes.ParamFloatingPointMeasurand:
                    return true;
                default:
                    return false;
            }
        }
        public static bool ASDUTypesRequiresPQ(IEC60870_5_104CommJob job)
        {
            switch (job.ASDUType)
            {
                case ASDUSelectableTypes.ParamNormalizedMeasurand:
                case ASDUSelectableTypes.ParamScaledMeasurand:
                case ASDUSelectableTypes.ParamFloatingPointMeasurand:
                    return true;
                default:
                    return false;
            }
        }
        public static bool ASDUTypesRequiresCA(IEC60870_5_104DynTagSettings TagSettings)
        {
            switch (TagSettings.ASDUType)
            {
                case ASDUSelectableTypes.RegulatingStep:
                    return true;
                default:
                    return false;
            }
        }
        public static bool ASDUTypesRequiresCA(IEC60870_5_104CommJob job)
        {
            switch (job.ASDUType)
            {
                case ASDUSelectableTypes.RegulatingStep:
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        #region methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return maximum memory size of IEC60870_5_104CommJob objects. </summary>
        ///
        /// <returns>   The maximum job size. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static uint GetMaxJobSize()
        {
            return IEC60870_5_104Protocol.MAX_DATA_BYTES;
        }

        public static UFUAModel.DataType DataType(ASDUSelectableTypes ASDUType)
        {
            switch (ASDUType)
            {
                case ASDUSelectableTypes.RegulatingStep:
                case ASDUSelectableTypes.ClockSynchronization:
                case ASDUSelectableTypes.SinglePoint:
                case ASDUSelectableTypes.GeneralInterrogation:
                case ASDUSelectableTypes.UploadFile:
                    return UFUAModel.DataType.Boolean;
                case ASDUSelectableTypes.DoublePoint:
                case ASDUSelectableTypes.StepPosition:
                    return UFUAModel.DataType.Byte;
                case ASDUSelectableTypes.BitString:
                    return UFUAModel.DataType.UInt32;
                case ASDUSelectableTypes.NormalizedMeasurand:
                case ASDUSelectableTypes.ScaledMeasurand:
                case ASDUSelectableTypes.ParamNormalizedMeasurand:
                case ASDUSelectableTypes.ParamScaledMeasurand:
                    return UFUAModel.DataType.Int16;
                case ASDUSelectableTypes.FloatingPointMeasurand:
                case ASDUSelectableTypes.ParamFloatingPointMeasurand:
                    return UFUAModel.DataType.Float;
                case ASDUSelectableTypes.IntegratedTotals:
                    return UFUAModel.DataType.Int32;
                case ASDUSelectableTypes.ReadDirectoryContents:
                    return UFUAModel.DataType.String;
                default:
                    return UFUAModel.DataType.UInt16;
            }

        }
        public static uint BuiltInDataType(ASDUSelectableTypes ASDUType)
        {
            switch (ASDUType)
            {
                case ASDUSelectableTypes.RegulatingStep:
                case ASDUSelectableTypes.ClockSynchronization:
                case ASDUSelectableTypes.SinglePoint:
                case ASDUSelectableTypes.GeneralInterrogation:
                case ASDUSelectableTypes.UploadFile:
                    return (uint)BuiltInType.Boolean;
                case ASDUSelectableTypes.DoublePoint:
                case ASDUSelectableTypes.StepPosition:
                    return (uint)BuiltInType.Byte;
                case ASDUSelectableTypes.BitString:
                    return (uint)BuiltInType.UInt32;
                case ASDUSelectableTypes.NormalizedMeasurand:
                case ASDUSelectableTypes.ScaledMeasurand:
                case ASDUSelectableTypes.ParamNormalizedMeasurand:
                case ASDUSelectableTypes.ParamScaledMeasurand:
                    return (uint)BuiltInType.Int16;
                case ASDUSelectableTypes.FloatingPointMeasurand:
                case ASDUSelectableTypes.ParamFloatingPointMeasurand:
                    return (uint)BuiltInType.Float;
                case ASDUSelectableTypes.IntegratedTotals:
                    return (uint)BuiltInType.Int32;
                default:
                    return (uint)BuiltInType.UInt16;
            }

        }
        public static bool DataTypeIncompatible(ASDUSelectableTypes ASDUType, UFUAModel.DataType VarType)
        {
            if ((uint)VarType == unchecked((uint)(-1)))
                return false;

            switch (ASDUType)
            {
                case ASDUSelectableTypes.IntegratedTotals:
                    return VarType != UFUAModel.DataType.Int32 && VarType != UFUAModel.DataType.UInt32;
                case ASDUSelectableTypes.UploadFile:
                    return false;
                default:
                    return VarType != DataType(ASDUType);
            }
        }
        public static bool InvalidAreaTypeLinkType(ASDUSelectableTypes ASDUType, LinkType LinkType)
        {
            return ((!isASDUTypesWritable(ASDUType) && (LinkType != LinkType.Input)) ||
                (!isASDUTypesReadable(ASDUType) && (LinkType == LinkType.Input || LinkType == LinkType.InputOutput)));
        }
        public static bool InvalidAreaTypeInput(ASDUSelectableTypes ASDUType, LinkType LinkType)
        {
            return ((!isASDUTypesWritable(ASDUType) && (LinkType != LinkType.Input)));
        }
        public static bool InvalidAreaTypeOutput(ASDUSelectableTypes ASDUType, LinkType LinkType)
        {
            return ((!isASDUTypesReadable(ASDUType) && (LinkType == LinkType.Input || LinkType == LinkType.InputOutput)));
        }

        public static void ParseDirectoryData(ref byte[] dataArray, ref string dataString)
        {
            dataString = String.Empty;
            // Calculate the number of files contained in the directory
            uint numberOfDirectoryFiles = (uint)(dataArray.Count() / Directory_infObj.FileInfoLength);
            if (numberOfDirectoryFiles < 1)
            {
                return;
            }

            int parsingOffset = 0;
            uint numberOfParsedFile = 0;
            while ((numberOfParsedFile < numberOfDirectoryFiles) && ((parsingOffset + Directory_infObj.FileInfoLength) <= dataArray.Count()))
            { 
                ParseFileData(ref dataArray, ref dataString, ref parsingOffset);
                numberOfParsedFile++;
            }
        }

        static void ParseFileData(ref byte[] dataArray, ref string dataString, ref int parsingOffset)
        {
            // The file name is an integer, 2 bytes long
            UInt16 fileName = BufferExpand.toUInt16(ref dataArray, ref parsingOffset);
            if(!String.IsNullOrEmpty(dataString))
            {
                dataString += FILE_INFO_SEPARATOR;
            }
            dataString += fileName.ToString();
            // The file length is an integer, 3 bytes long
            byte[] fileLengthBuffer = BufferExpand.toArray(ref dataArray, ref parsingOffset, 3);
            uint fileLength = BufferExpand.toUInt32(fileLengthBuffer);
            dataString += FILE_INFO_SEPARATOR;
            dataString += fileLength.ToString();
            // The status of file is a byte
            byte sof = BufferExpand.toByte(ref dataArray, ref parsingOffset);
            // The file timestamp in CP56Time2a format is 7 bytes long
            CP56Time2a auxTimestamp = new CP56Time2a(DateTime.Now);
            auxTimestamp.Expand(ref dataArray, ref parsingOffset);
            dataString += FILE_INFO_SEPARATOR;
            dataString += auxTimestamp.Value.ToString();
        }
        #endregion

        #region methods override

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Validation of the response data type input. If correct copy data in job. </summary>
        ///
        /// <param name="receivebuffer">                    . </param>
        /// <param name="job">                              [in,out]. </param>
        /// <param name="items" type="ref List<object>">    [in,out]. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool ParseData(byte[] receivedbuffer, ref IEC60870_5_104CommJob job, ref List<object> items)
        {
            List<Tag> changed = new List<Tag>();
            uint receivedData = (uint)receivedbuffer.Length;

            if (job.isProtocolBool() &&
                (job.TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean || job.ElementNumber > 0))
                receivedData *= 8;
            if (job.TotalJobSize < receivedData)
            {
                receivedData = job.TotalJobSize;
            }

            byte[] tempBuffer = new byte[receivedData];
            

            if (job.isProtocolBool() &&
                (job.TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean || job.ElementNumber > 0))
                Array.Copy(receivedbuffer, tempBuffer, receivedData);
            else
                for (ushort Index = 0; Index < receivedData; Index++)
                    tempBuffer[Index % 8] += (byte)(receivedbuffer[Index] << (Index % 8));

            if (job.isProtocolBool() && job.TagsList[0].TagNode.DataType != Opc.Ua.DataTypes.Boolean && job.ElementNumber == 0)
                for (ushort Index = 0; Index < receivedData; Index++)
                    tempBuffer[Index % 8] += (byte)(receivedbuffer[Index] << (Index % 8));
            else
                Array.Copy(receivedbuffer, tempBuffer, receivedData);

            job.SetJobData(tempBuffer, ref changed);
            items.AddRange(changed);

            return true;
        }
        #endregion

    }
}
