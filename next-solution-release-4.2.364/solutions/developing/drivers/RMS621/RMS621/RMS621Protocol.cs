using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using Opc.Ua;
using System.Globalization;

namespace RMS621
{
    public class RMS621Protocol
    {
        public static int BUFFER_SIZE = 255;
        public static int RMS_MAX_ADMITTED = 128;

        private static byte SOH = 0x01;
        private static byte STX = 0x02;
        private static byte ETX = 0x03;

        const byte SOH_LENGTH = 1;
        const byte UnitNumber_LENGTH = 2;
        const byte STX_LENGTH = 1;
        const byte Data_LENGTH = 7;
        const byte BCC_LENGTH = 1;

        public enum RMS621ErrorCodes : int {            
            BadCheckSum = 602,
            BadFormatAnswer = 603,
            FromDevice = 604,
            DataMissing = 605,            
            DataFormatError = 606,
            JobDataFormat = 607,
            BadRequestCommand = 608,
            TooMuchCharsReceived = 609,
            WriteCommandInvalid = 610,
            WarningFromDevice = 611,
            EndWarningFromDevice = 612,
            //DRIVERLICCODE = 613,
            //RMS621GROUP = 614,
            //RMS621PASSWORD = 615            
        }

        #region Constructors
        public RMS621Protocol()
        {
        }
        #endregion

        #region Properties
        public enum Command : int
        {
            Read = -1,
            ResetAllCounters = 0,
            ResetEventList = 1
        }

        public enum Process : int
        {
            None = -1,
            HeatFlow = 0,
            HeatSum = 1,
            MassFlow = 2,
            MassSum = 3,
            FlowRate = 4,
            Pressure = 5,
            Temperature = 6,
            TemperatureDifference = 7,
            FlowSum = 8,
            Density = 9,
            SpecificEnthalpy = 10,
            TotHeatSum = 11,
            TotMassSum = 12,
            TotFlowRate = 13,
            NegativeHeatSum = 14,
            NegativeMassSum = 15,
            TotNegativeHeatSum = 16, 
            TotNegativeMassSum = 17
        }

        private enum GetRequestCmd : int {
            Proc_HFLOW,
            Proc_H_SUM,
            Proc_MFLOW,
            Proc_M_SUM,
            Proc_FLOW_,
            Proc_PRESS,
            Proc_TEMP_,
            Proc_TDIFF,
            Proc_QSUM_,
            Proc_DENSI,
            Proc_ENTAL,
            Proc_THSUM,
            Proc_TMSUM,
            Proc_TQSUM,
            Proc_HNSUM,
            Proc_MNSUM,
            Proc_TNHSU,
            Proc_TNMSU,
            Proc_MANZ
        };

        private enum GetWriteCmd : int
        {
            Cmd_None = -1,
            Cmd_Reset = 0,
            Cmd_ResetEvents = 1
        };
        #endregion

        #region Method
        public static byte CalculateBCC(byte[] buf, int nStart, int nByte)
        {
            byte b = 0;
            for (int i = nStart; i < (int)(nStart + nByte); i++)
            {
                b ^= buf[i];
            }
            return b;
        }

        public static uint GetRequestCommandMinLength()
        {
            // SOH + Unit Number + STX + Data + STX + BCC
            return (SOH_LENGTH + UnitNumber_LENGTH + STX_LENGTH + Data_LENGTH + STX_LENGTH + BCC_LENGTH);
        }

        private static uint GetRequestCommand(out byte[] szBuffer, int procNum)
        {
            string Cmd = null;

            switch ((GetRequestCmd)procNum)
            {
                case GetRequestCmd.Proc_HFLOW:
                    Cmd = "M0HFLOW";
                    break;
                case GetRequestCmd.Proc_H_SUM:
                    Cmd = "M0H_SUM";
                    break;
                case GetRequestCmd.Proc_MFLOW:
                    Cmd = "M0MFLOW";
                    break;
                case GetRequestCmd.Proc_M_SUM:
                    Cmd = "M0M_SUM";
                    break;
                case GetRequestCmd.Proc_FLOW_:
                    Cmd = "M0FLOW_";
                    break;
                case GetRequestCmd.Proc_PRESS:
                    Cmd = "M0PRESS";
                    break;
                case GetRequestCmd.Proc_TEMP_:
                    Cmd = "M0TEMP_";
                    break;
                case GetRequestCmd.Proc_TDIFF:
                    Cmd = "M0TDIFF";
                    break;
                case GetRequestCmd.Proc_QSUM_:
                    Cmd = "M0QSUM_";
                    break;
                case GetRequestCmd.Proc_DENSI:
                    Cmd = "M0DENSI";
                    break;
                case GetRequestCmd.Proc_ENTAL:
                    Cmd = "M0ENTAL";
                    break;
                case GetRequestCmd.Proc_THSUM:
                    Cmd = "M0THSUM";
                    break;
                case GetRequestCmd.Proc_TMSUM:
                    Cmd = "M0TMSUM";
                    break;
                case GetRequestCmd.Proc_TQSUM:
                    Cmd = "M0TQSUM";
                    break;
                case GetRequestCmd.Proc_HNSUM:
                    Cmd = "M0HNSUM";
                    break;
                case GetRequestCmd.Proc_MNSUM:
                    Cmd = "M0MNSUM";
                    break;
                case GetRequestCmd.Proc_TNHSU:
                    Cmd = "M0TNHSU";
                    break;
                case GetRequestCmd.Proc_TNMSU:
                    Cmd = "M0TNMSU";
                    break;
                case GetRequestCmd.Proc_MANZ:
                    Cmd = "MANZ";
                    break;
                default:
                    Cmd = string.Empty;
                    break;
            }

            if (!string.IsNullOrEmpty(Cmd))
                szBuffer = Encoding.ASCII.GetBytes(Cmd);
            else 
                szBuffer = new byte[0];

            return (uint)szBuffer.Length;
        }

        public static uint GetWriteCommandMinLength()
        {
            // SOH + Unit Number + STX + Data + STX + BCC
            return (1 + 2 + 1 + 7 + 1 + 1);
        }
                        
        public static uint DoReadData(ref byte[] buffer, int process, uint processNumber, byte unitNumber)
        {
            int nByteCount = 0;
            byte[] WriteBuffer = new byte[BUFFER_SIZE];
            byte[] szBuffer;

            //Start of header
            WriteBuffer[nByteCount++] = SOH;
            
            //Device address            
            szBuffer = Encoding.ASCII.GetBytes(unitNumber.ToString("00"));
            WriteBuffer[nByteCount++] = szBuffer[0];
            WriteBuffer[nByteCount++] = szBuffer[1];
            //Start of data
            WriteBuffer[nByteCount++] = STX;
            //Data
            if (GetRequestCommand(out szBuffer, process) == 0)
            {
                ////Invalid Command
                //UINT nLastError = BADREQUESTCOMMAND;
                //CString szBuffer = CMoviconDriverApp::fmtString(nLastError, szJobName);
                //SetInError(true, nLastError, szBuffer);
                //PurgeRx();
                //PurgeTx();
                return (0);
            }
            for (int i = 0; i < szBuffer.Length; i++)
            {
                if ((szBuffer[i] > 0 && szBuffer[i] < 0x16) ||
                    (szBuffer[i] == 0xFF))
                {
                    WriteBuffer[nByteCount++] = 0xFF;
                    WriteBuffer[nByteCount++] = (byte)(szBuffer[i] | (byte)0x80);
                }
                else
                    WriteBuffer[nByteCount++] = szBuffer[i];
            }
            //Process number
            if ((GetRequestCmd)process != GetRequestCmd.Proc_MANZ)
            {
                WriteBuffer[nByteCount++] = (byte)(0x2F + processNumber);
            }
            //End of data
            WriteBuffer[nByteCount++] = ETX;
            //BCC
            WriteBuffer[nByteCount++] = CalculateBCC(WriteBuffer, 4, nByteCount - 4);

            if (nByteCount>0)
            {
                Array.Resize(ref buffer, nByteCount);
                Array.Copy(WriteBuffer,buffer, nByteCount);
            }

            return (uint)nByteCount;
        }

        private static bool IsNumeric(byte byteValue)
        {
            return (byteValue >= 0x30 && byteValue <= 0x39);
        }

        public static int ParseFrame(List<byte> readBuffer, int minSize, out int sOHIdx, out int nIdx)
        {
            sOHIdx = 0;
            nIdx = 0;

            if (readBuffer == null || readBuffer.Count <= minSize) //7)
                return (int)RMS621Protocol.RMS621ErrorCodes.DataMissing;

            if (readBuffer.Count > RMS_MAX_ADMITTED)
                return (int)RMS621Protocol.RMS621ErrorCodes.TooMuchCharsReceived;

            sOHIdx = readBuffer.IndexOf(SOH);

            // end of message data
            nIdx = readBuffer.IndexOf(ETX);
            if (nIdx == -1)
                return (int)RMS621Protocol.RMS621ErrorCodes.DataMissing;

            // check if crc byte is present            
            if (nIdx + 1 + 1 > readBuffer.Count)
                    return (int)RMS621Protocol.RMS621ErrorCodes.DataMissing;


            for (int i = 0; i < readBuffer.Count; i++)
            {
                if (readBuffer[i] == 0xFF)
                    readBuffer[i] = (byte)(readBuffer[i] & 0x7F);
            }

            //do
            //{
            //    bRead = readBuffer[nIdx];
            //    //if (!DeviceRead(&bRead, 1))
            //    //{
            //    //    PurgeRx();
            //    //    PurgeTx();
            //    //    return (FALSE);
            //    //}

            //    if (bRead == ETX)
            //    {
            //        //m_ReadBuffer[nIdx] = bRead;
            //        nIdx++;
            //        break;
            //    }

            //    if (bRead == 0xFF)
            //    {
            //        ////this is a padding, read next...
            //        //if (!DeviceRead(&bRead, 1))
            //        //{
            //        //    PurgeRx();
            //        //    PurgeTx();
            //        //    return (FALSE);
            //        //}
            //        bRead = (byte)(bRead & 0x7F);
            //    }
            //    if (nIdx > RMS_MAX_ADMITTED)
            //    {
            //        //too much chars read
            //        //UINT nLastError = TOOMUCHCHARSRECEIVED;
            //        //CString szBuffer = CMoviconDriverApp::fmtString(nLastError, szJobName);
            //        //SetInError(true, nLastError, szBuffer);
            //        //PurgeRx();
            //        //PurgeTx();
            //        return (int)RMS621Protocol.RMS621Errors.TOOMUCHCHARSRECEIVED;
            //    }
            //    readBuffer[nIdx] = bRead;
            //    nIdx++;
            //} while (TRUE);

            ////Read BCC
            //if (!DeviceRead(&m_ReadBuffer[nIdx], 1))
            //{
            //    PurgeRx();
            //    PurgeTx();
            //    return (FALSE);
            //}

            //Check answer format
            if (readBuffer[0] != SOH || readBuffer[3] != STX || readBuffer[nIdx] != ETX)
                return (int)RMS621Protocol.RMS621ErrorCodes.BadFormatAnswer;

            //Check return code
            if (readBuffer[sOHIdx + 4] == 0x39)
                return (int)RMS621Protocol.RMS621ErrorCodes.FromDevice;

            //Check BCC
            if (readBuffer[nIdx + 1] != CalculateBCC(readBuffer.ToArray(), sOHIdx + 4, nIdx - 4 - sOHIdx + 1))
                return (int)RMS621Protocol.RMS621ErrorCodes.BadCheckSum;

            //if ((readBuffer[4] != 0x30) && (*bWarning == FALSE))
            //{
            //    //log a warning.
            //    CString szErr;
            //    szErr.Format(IDS_WARNINGFROMDEVICE, m_ReadBuffer[4] - 0x30, szJobName);
            //    *bWarning = TRUE;
            //    AfxGetRMS621App()->LogToMovicon(szErr);
            //}

            //if ((m_ReadBuffer[4] == 0x30) && (*bWarning == TRUE))
            //{
            //    //recover from warning
            //    CString szErr;
            //    szErr.Format(IDS_ENDWARNINGFROMDEVICE, szJobName);
            //    *bWarning = FALSE;
            //    AfxGetRMS621App()->LogToMovicon(szErr);
            //}

            nIdx++;

            return (int)DriverErrorCodes.ErrorNoError;
        }

        public static int ParseReadData(List<byte> readBuffer, out byte[] resultData, out int messageEnd)
        {            
            messageEnd = 0;
            resultData = new byte[0];

            int SOHIdx;
            int nIdx;
            int Ret = ParseFrame(readBuffer, 7, out SOHIdx, out nIdx);
            if (Ret != 0)
                return Ret;

            //Take data
            int nDati = nIdx - 2 - (SOHIdx+5) + 1;
            if (nDati < 1)
                return (int)RMS621Protocol.RMS621ErrorCodes.DataMissing;

            int nStartIdx = (SOHIdx + 5);
            for (int i = 0; i < nDati - 1; i++)
            {
                if (readBuffer[(SOHIdx + 5) + i] == (byte)0x2C)
                {
                    readBuffer[(SOHIdx + 5) + i] = 0x2E;
                }

                if (IsNumeric(readBuffer[(SOHIdx + 5) + i]) || (readBuffer[(SOHIdx + 5) + i] == 0x2E))
                {
                    continue;
                }
                if (readBuffer[(SOHIdx + 5) + i] == 0x2D)
                {
                    //check next...
                    if ((readBuffer[(SOHIdx + 5) + i + 1] == 0x2E) || (readBuffer[(SOHIdx + 5) + i + 1] == 0x2C) || IsNumeric(readBuffer[(SOHIdx + 5) + i + 1]))
                    {
                        if ((readBuffer[(SOHIdx + 5) + i + 1] == 0x2C))
                            readBuffer[(SOHIdx + 5) + i + 1] = 0x2E;

                        nStartIdx = ((SOHIdx + 5) + i);
                        break;
                    }
                }
            }

            // covert device data (number is in ascii format) into C# variable
            string DeviceValue = System.Text.Encoding.ASCII.GetString(readBuffer.Skip(nStartIdx).Take(nIdx - nStartIdx -1).ToArray());

            if (DeviceValue.IndexOf(",") > 0)
                DeviceValue = DeviceValue.Replace(",", ".");

            double dummy = 0.0;
            NumberStyles style = NumberStyles.Any;
            CultureInfo provider = CultureInfo.InvariantCulture;
            if (!double.TryParse(DeviceValue, style, provider, out dummy))
                return (int)RMS621Protocol.RMS621ErrorCodes.BadFormatAnswer;


            resultData = BitConverter.GetBytes(dummy);
            messageEnd = nIdx;

            return (int)DriverErrorCodes.ErrorNoError;
        }
        
        private static int GetWriteCommand(out byte[] szBuffer, int command)
        {
            string Cmd = null;

            switch ((GetWriteCmd)command)
            {
                case GetWriteCmd.Cmd_Reset:
                    Cmd = "KPR2";
                    break;
                case GetWriteCmd.Cmd_ResetEvents:
                    Cmd = "KPR3";
                    break;
                default:
                    Cmd = string.Empty;
                    break;
            }

            if (!string.IsNullOrEmpty(Cmd))
                szBuffer = Encoding.ASCII.GetBytes(Cmd);
            else
                szBuffer = new byte[0];

            return szBuffer.Length;
        }

        public static uint DoWriteData(ref byte[] buffer, byte[] jobdata, int process, uint processNumber, byte unitNumber)
        {
            int nByteCount = 0;
            byte[] WriteBuffer = new byte[BUFFER_SIZE];
            byte[] szBuffer;

            //Start of header
            WriteBuffer[nByteCount++] = SOH;

            //Device address            
            szBuffer = Encoding.ASCII.GetBytes(unitNumber.ToString("00"));
            WriteBuffer[nByteCount++] = szBuffer[0];
            WriteBuffer[nByteCount++] = szBuffer[1];
            //Start of data
            WriteBuffer[nByteCount++] = STX;
            //Data
            if (GetWriteCommand(out szBuffer, process) == 0)
            {
                ////Invalid Command
                //UINT nLastError = BADREQUESTCOMMAND;
                //CString szBuffer = CMoviconDriverApp::fmtString(nLastError, szJobName);
                //SetInError(true, nLastError, szBuffer);
                //PurgeRx();
                //PurgeTx();
                return (0);
            }
            for (int i = 0; i < szBuffer.Length; i++)
            {
                if ((szBuffer[i] > 0 && szBuffer[i] < 0x16) ||
                    (szBuffer[i] == 0xFF))
                {
                    WriteBuffer[nByteCount++] = 0xFF;
                    WriteBuffer[nByteCount++] = (byte)(szBuffer[i] | (byte)0x80);
                }
                else
                    WriteBuffer[nByteCount++] = szBuffer[i];
            }                        
            //End of data
            WriteBuffer[nByteCount++] = ETX;
            //BCC
            WriteBuffer[nByteCount++] = CalculateBCC(WriteBuffer, 4, nByteCount - 4);

            if (nByteCount > 0)
            {
                Array.Resize(ref buffer, nByteCount);
                Array.Copy(WriteBuffer, buffer, nByteCount);
            }

            return (uint)nByteCount;
        }

        public static int ParseWritedData(List<byte> readBuffer, out byte[] resultData, out int messageEnd)
        {
            messageEnd = 0;
            resultData = new byte[0];

            int SOHIdx;
            return ParseFrame(readBuffer, 6, out SOHIdx, out messageEnd);            
        }

        public static bool ParseData(byte[] receivebuffer, ref RMS621CommJob job, ref List<object> items)
        {
            List<Tag> changed = new List<Tag>();
            bool areArguments = (items.Count > 0);
            if (!areArguments && (receivebuffer == null))
            {
                return (false);
            }
            else if (receivebuffer == null)
            {
                BuiltInType bt = job.Station.GetBuiltInType(items[0].GetType());
                if (bt != BuiltInType.Byte && bt != BuiltInType.Double &&
                    bt != BuiltInType.Float && bt != BuiltInType.Int16 &&
                    bt != BuiltInType.Int32 && bt != BuiltInType.Int64 &&
                    bt != BuiltInType.Integer && bt != BuiltInType.Number &&
                    bt != BuiltInType.SByte && bt != BuiltInType.UInt16 &&
                    bt != BuiltInType.UInt32 && bt != BuiltInType.UInt64 &&
                    bt != BuiltInType.UInteger)
                {
                    return false;
                }

                items[0] = DriverErrorCodes.ErrorNoError;
                return (true);
            }

            if (areArguments)
            {
                items[0] = DriverErrorCodes.ErrorNoError;
            }

            byte[] tempBuffer = new byte[receivebuffer.Length];

            Array.Copy(receivebuffer, tempBuffer, receivebuffer.Length);

            job.SetJobData(tempBuffer, ref changed);

            if (areArguments)
            {
                if (job.TagsList.Count == items.Count - 1)
                {
                    for (int k = 0; k < job.TagsList.Count; k++)
                    {
                        items[k + 1] = job.TagsList[k].Value.Value;
                    }
                }
            }
            else
            {
                items.AddRange(changed);
            }

            return true;
        }

        #endregion

    }
}
