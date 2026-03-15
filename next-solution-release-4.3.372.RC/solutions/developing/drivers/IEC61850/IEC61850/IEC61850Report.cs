using DriverCodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEC61850
{
    public class IEC61850Report
    {
        #region Constructor
        public IEC61850Report(string reportLogicalDeviceName, string mmsReportID, ReportTypes reportType, IEC61850Channel channel, IEC61850Station station)
        {
            m_szDeviceID = reportLogicalDeviceName;
            m_szMMSReportID = mmsReportID;
            m_nReportType = reportType;
            Channel = channel;
            Station = station;            
        }
        #endregion

        #region Properties
        string m_szDeviceID = String.Empty;
        string m_szMMSReportID = String.Empty;
        ReportTypes m_nReportType = ReportTypes.Unbuffered;
        bool m_bIsActive = false;
        IEC61850Station Station = null;
        IEC61850Channel Channel = null;
        Dictionary<uint, string> m_mapIndexToDataSetElement = new Dictionary<uint, string>();
        Dictionary<string, List<IEC61850CommJob>> m_mapActiveJobs = new Dictionary<string, List<IEC61850CommJob>>();
        Dictionary<string, string> m_mapStringIndexToDataSetElement = new Dictionary<string, string>();
        Dictionary<string, string> m_mapDataSetElementToStringIndex = new Dictionary<string, string>();
        String m_szRptID = String.Empty;
        bool m_bRptEna = false;
        bool m_bResv = false;
        String m_szDataSetDevice = String.Empty;
        String m_szDataSet = String.Empty;
        uint m_nConfRev = 0;
        UInt16 m_nOptFlds = 0;
        uint m_nBufTm = 0;
        uint m_nSqNum = 0;
        byte m_nTrgOps = 0;
        uint m_nIntgPd = 0;
        bool m_bGI = false;
        bool m_bPurgeBuf = false;
        String m_szEntryID = String.Empty;
        DateTime m_dtTimeOfEntry = DateTime.MinValue;
        uint m_nResvTms = 0;
        String m_szOwner = String.Empty;
        uint m_nLastReceivedItemIndex = 0;
        uint m_nLastUpdatedStructFieldIndex = 0;
        private DateTime m_nLastActivationAttemptTime = DateTime.MinValue;
        bool m_ForceRepIDIfEmpty = false;
        #endregion

        #region Methods
        //public bool AddJob(IEC61850CommJob iecJob)
        //{
        //    if (String.IsNullOrWhiteSpace(m_szDataSet))
        //    {
        //        iecJob.bAddedToReport = false;
        //        if (!ReadControlBlock())
        //            return (false);

        //        if (string.IsNullOrEmpty(m_szRptID))
        //            m_mapIndexToDataSetElement.Clear();
        //    }

        //    if (m_mapIndexToDataSetElement.Count == 0)
        //    {
        //        iecJob.bAddedToReport = false;
        //        if (!ReadDataSetDirectory())
        //        {
        //            m_mapIndexToDataSetElement.Clear();
        //            m_mapDataSetElementToStringIndex.Clear();
        //            m_mapStringIndexToDataSetElement.Clear();
        //            return (false);
        //        }

        //        if (!Channel.MapRptID.ContainsKey(m_szRptID))
        //            Channel.MapRptID[m_szRptID] = string.Format("{0}/{1}", m_szDeviceID, m_szMMSReportID);
        //    }

        //    if (!iecJob.bAddedToReport)
        //    {
        //        string szSearchKey = iecJob.MMSDataItemCompletePath;
        //        if (!m_mapDataSetElementToStringIndex.ContainsKey(szSearchKey))
        //        {
        //            iecJob.SetIsValid(false);
        //            return (false);
        //        }
        //        string szIndex = m_mapDataSetElementToStringIndex[szSearchKey];

        //        // Set the nesting level of the data associated to the job
        //        uint nDataNestingLevel = (uint)szIndex.Count(c => c == '.');
        //        if (nDataNestingLevel >= IEC61850Protocol.MAX_DATASET_NESTINGLEVEL)
        //        {
        //            iecJob.SetIsValid(false);
        //            return (false);
        //        }
        //        iecJob.nDataNestingLevel = nDataNestingLevel;
        //        if (szIndex == "0")
        //            iecJob.bIsFullReport = true;

        //        if (!m_mapActiveJobs.ContainsKey(szSearchKey))
        //            m_mapActiveJobs[szSearchKey] = new List<IEC61850CommJob>();
        //        if (m_mapActiveJobs[szSearchKey].Count(j => j == iecJob) == 0)
        //            m_mapActiveJobs[szSearchKey].Add(iecJob);

        //        iecJob.bAddedToReport = true;                
        //    }

        //    if (!m_bIsActive && Channel.ReportAutomaticActivation)
        //    {                                              
        //        if ((m_nLastActivationAttemptTime == DateTime.MinValue) || m_nLastActivationAttemptTime.Subtract(DateTime.UtcNow).TotalMilliseconds> Channel.Timeout) {
        //            if (!_ActivateReport(false))
        //            {
        //                iecJob.bAddedToReport = false;
        //                return (false);
        //            }
        //            if (!_ActivateReport(true))
        //            {
        //                iecJob.bAddedToReport = false;
        //                return (false);
        //            }
        //            m_nLastActivationAttemptTime = DateTime.UtcNow;
        //        }
        //    }

        //    return (true);
        //}

        public bool GetStructAndAssignJobs(IEC61850CommJob iecJob)
        {
            if (String.IsNullOrWhiteSpace(m_szDataSet))
            {
                if (!ReadControlBlock())
                    return (false);

                if (string.IsNullOrEmpty(m_szRptID))
                    m_mapIndexToDataSetElement.Clear();
            }

            if (m_mapIndexToDataSetElement.Count == 0)
            {
                if (!ReadDataSetDirectory())
                {
                    m_mapIndexToDataSetElement.Clear();
                    m_mapDataSetElementToStringIndex.Clear();
                    m_mapStringIndexToDataSetElement.Clear();
                    return (false);
                }

                //if (!Channel.MapRptID.ContainsKey(m_szRptID))
                //    Channel.MapRptID[m_szRptID] = string.Format("{0}/{1}", m_szDeviceID, m_szMMSReportID);
            }

            if (!iecJob.bAddedToReport)
            {
                string szSearchKey = iecJob.MMSDataItemCompletePath;
                if (!m_mapDataSetElementToStringIndex.ContainsKey(szSearchKey))
                {
                    iecJob.SetIsValid(false);
                    return (false);
                }
                string szIndex = m_mapDataSetElementToStringIndex[szSearchKey];

                // Set the nesting level of the data associated to the job
                uint nDataNestingLevel = (uint)szIndex.Count(c => c == '.');
                if (nDataNestingLevel >= IEC61850Protocol.MAX_DATASET_NESTINGLEVEL)
                {
                    iecJob.SetIsValid(false);
                    return (false);
                }
                iecJob.nDataNestingLevel = nDataNestingLevel;
                if (szIndex == "0")
                    iecJob.bIsFullReport = true;

                if (!m_mapActiveJobs.ContainsKey(szSearchKey))
                    m_mapActiveJobs[szSearchKey] = new List<IEC61850CommJob>();
                if (m_mapActiveJobs[szSearchKey].Count(j => j == iecJob) == 0)
                    m_mapActiveJobs[szSearchKey].Add(iecJob);

                iecJob.bAddedToReport = true;
            }

            return true;
        }

        public bool SetReportID()
        {
            // if Id report is empty, force driver id (station + ....); change is possible only when report is disabled
            if (string.IsNullOrEmpty(m_szRptID))
            {
                if (m_bRptEna)
                {
                    if (!_ActivateReport(false))
                        return (false);
                }

                string newRptID = string.Format("{0}_{1}", Station.Name, m_szMMSReportID.Replace("$", "_"));
                if (SetRepId(newRptID))
                {
                    m_szRptID = newRptID;
                    if (!Channel.MapRptID.ContainsKey(m_szRptID))
                        Channel.MapRptID[m_szRptID] = string.Format("{0}/{1}", m_szDeviceID, m_szMMSReportID);
                } else { 
                    return false;
                }

                if (m_bRptEna)
                {
                    if (!_ActivateReport(true))
                        return (false);
                }
            }
            else
            {
                if (!Channel.MapRptID.ContainsKey(m_szRptID))
                {
                    Channel.MapRptID[m_szRptID] = string.Format("{0}/{1}", m_szDeviceID, m_szMMSReportID);
                }
            }

            return true;
        }   

        public bool Activate() {

            if (!m_bIsActive && m_mapActiveJobs.Count>0)
            {
                if ((m_nLastActivationAttemptTime == DateTime.MinValue) || m_nLastActivationAttemptTime.Subtract(DateTime.UtcNow).TotalMilliseconds > Channel.Timeout)
                {
                    if (!_ActivateReport(false))
                        //jobiecJob.bAddedToReport = false;
                        return (false);
                    
                    if (!_ActivateReport(true))
                        //iecJob.bAddedToReport = false;
                        return (false);

                    m_bIsActive = true;

                    m_nLastActivationAttemptTime = DateTime.UtcNow;
                }
            }

            return (true);
        }

        public bool DeActivate()
        {
            return (_ActivateReport(false));
        }

        private bool SetRepId(string szRepID)
        {
            string szItemID = string.Format("{0}$RptID", m_szMMSReportID);
            byte[] pDataBuffer = ASCIIEncoding.ASCII.GetBytes(szRepID);
            return (Channel.WriteRCBItem(szItemID, m_szDeviceID, MMSDataTypes.VisibleString, pDataBuffer, 0));                
        }

        bool ReadDataSetDirectory()
        {
            if ((Station == null) || (Channel == null))
                return (false);

            byte[] DataBuffer = Channel.ReadDataSetDirectory(m_szDataSetDevice, m_szDataSet);
            if (DataBuffer == null)
                return (false);

            if (!ParseDataSetDirectory(DataBuffer))
                return (false);

            m_mapDataSetElementToStringIndex.Clear();
            m_mapStringIndexToDataSetElement.Clear();

            if (m_mapIndexToDataSetElement.Count() > 0)
            {
                // Read the access attributes of the components of the data set
                uint nMaxIndex = (uint)(m_mapIndexToDataSetElement.Count() - 1);
                for (uint nIndex = 0; nIndex <= nMaxIndex; nIndex++)
                {
                    if (!m_mapIndexToDataSetElement.ContainsKey(nIndex))
                    {
                        break;
                    }
                    string str = m_mapIndexToDataSetElement[nIndex];

                    string szStringIndex = nIndex.ToString();
                    // Skip the data set itself
                    if (nIndex == 0)
                    {
                        m_mapStringIndexToDataSetElement[szStringIndex] = str;
                        m_mapDataSetElementToStringIndex[str] = szStringIndex;
                        continue;
                    }

                    int nSearchIndex = str.IndexOf('/');
                    if ((nSearchIndex < 1) || (nSearchIndex == (str.Length - 1)))
                    {
                        continue;
                    }
                    string szDevice = str.Substring(0, nSearchIndex);
                    string szItem = str.Substring(nSearchIndex + 1, str.Length - nSearchIndex - 1);
                    DataBuffer = Channel.ReadVariableAccessAttributes(szDevice, szItem);
                    if (DataBuffer == null)
                    {
                        return (false);
                    }

                    ParseVariableAccessAttributes(DataBuffer, str, szStringIndex);
                }
            }

            return (true);
        }

        void ParseVariableAccessAttributes(byte[] dataBuffer, string szMMSVar, string szMapIndex)
        {
            string szBaseIndex = szMapIndex;
            uint nDataOffset = 0;
            uint nDataLength = (uint)dataBuffer.Length;
            while (nDataLength > (nDataOffset + 1))
            {
                if (!ParseVariableAccessAttribute(dataBuffer, nDataLength, ref nDataOffset, szMMSVar, szBaseIndex))
                    break;
            }
        }

        bool ParseVariableAccessAttribute(byte[] dataBuffer, uint nDataLength, ref uint nDataOffset, string szMMSVar, string szMapIndex)
        {
            bool bRetValue = true;
            IEC61850Protocol P = new IEC61850Protocol();

            if (nDataLength > (nDataOffset + 1))
            {
                uint nLengthSize = 0;
                uint nLength = 0;
                switch (dataBuffer[nDataOffset++])
                {
                    // Float
                    case 0x87:
                    // Boolean
                    case 0x83:
                    // Integer
                    case 0x85:
                    // Unsigned Integer
                    case 0x86:
                    // Octet String
                    case 0x89:
                    // Visible String
                    case 0x8a:
                    // MMS String
                    case 0x90:
                    // Bit String
                    case 0x84:
                    // UTC Time
                    case 0x91:
                    // Binary Time
                    case 0x8c:
                    // ??
                    case 0xa7:
                        m_mapStringIndexToDataSetElement[szMapIndex] = szMMSVar;
                        m_mapDataSetElementToStringIndex[szMMSVar] = szMapIndex;
                        nLengthSize = 0;
                        nLength = P.BerDecodeLength(dataBuffer, nDataOffset, nDataLength - nDataOffset, out nLengthSize);
                        nDataOffset += nLengthSize;
                        if (nLength > 0)
                            nDataOffset += nLength;
                        break;

                    // Structure
                    case 0xa2:
                        {
                            m_mapStringIndexToDataSetElement[szMapIndex] = szMMSVar;
                            m_mapDataSetElementToStringIndex[szMMSVar] = szMapIndex;
                            nLengthSize = 0;
                            nLength = P.BerDecodeLength(dataBuffer, nDataOffset, nDataLength - nDataOffset, out nLengthSize);
                            if (nLength > 0)
                            {
                                if (nDataLength >= (nDataOffset + nLengthSize + nLength))
                                {
                                    nDataOffset += nLengthSize;
                                    // Typedef specification?
                                    if (dataBuffer[nDataOffset++] != 0xa1)
                                    {
                                        bRetValue = false;
                                    }
                                    else
                                    {
                                        nLengthSize = 0;
                                        nLength = P.BerDecodeLength(dataBuffer, nDataOffset, nDataLength - nDataOffset, out nLengthSize);
                                        if (nLength == 0)
                                        {
                                            bRetValue = false;
                                        }
                                        else if (nDataLength >= (nDataOffset + nLengthSize + nLength))
                                        {
                                            nDataOffset += nLengthSize;
                                            uint nStructSize = nLength;
                                            uint nDataOffsetLimit = nDataOffset + nStructSize;
                                            uint nMemberIndex = 0;
                                            String szMemberIndex;
                                            String szMemberName;
                                            // Loop on structure's members
                                            for (nMemberIndex = 0; nDataOffset < nDataOffsetLimit; nMemberIndex++)
                                            {
                                                // Component item?
                                                if (dataBuffer[nDataOffset++] != 0x30)
                                                {
                                                    bRetValue = false;
                                                    break;
                                                }

                                                nLengthSize = 0;
                                                nLength = P.BerDecodeLength(dataBuffer, nDataOffset, nDataLength - nDataOffset, out nLengthSize);
                                                if (nLength == 0)
                                                {
                                                    bRetValue = false;
                                                    break;
                                                }
                                                if (nDataLength < (nDataOffset + nLengthSize + nLength))
                                                {
                                                    bRetValue = false;
                                                    break;
                                                }
                                                nDataOffset += nLengthSize;
                                                // Item name?
                                                if (dataBuffer[nDataOffset++] != 0x80)
                                                {
                                                    bRetValue = false;
                                                    break;
                                                }
                                                nLengthSize = 0;
                                                nLength = P.BerDecodeLength(dataBuffer, nDataOffset, nDataLength - nDataOffset, out nLengthSize);
                                                if (nLength == 0)
                                                {
                                                    bRetValue = false;
                                                    break;
                                                }
                                                if (nDataLength < (nDataOffset + nLengthSize + nLength))
                                                {
                                                    bRetValue = false;
                                                    break;
                                                }
                                                nDataOffset += nLengthSize;
                                                // Parse the item name
                                                szMemberName = string.Format("{0}$", szMMSVar);
                                                for (int i = 0; i < (int)nLength; i++)
                                                {
                                                    szMemberName += (char)dataBuffer[nDataOffset++];
                                                }

                                                // Typedef specification?
                                                if (dataBuffer[nDataOffset++] != 0xa1)
                                                {
                                                    bRetValue = false;
                                                    break;
                                                }
                                                nLengthSize = 0;
                                                nLength = P.BerDecodeLength(dataBuffer, nDataOffset, nDataLength - nDataOffset, out nLengthSize);
                                                if (nLength == 0)
                                                {
                                                    bRetValue = false;
                                                    break;
                                                }
                                                if (nDataLength <
                                                    (nDataOffset + nLengthSize + nLength))
                                                {
                                                    bRetValue = false;
                                                    break;
                                                }
                                                nDataOffset += nLengthSize;

                                                // Set the item search key
                                                szMemberIndex = string.Format("{0}.{1}", szMapIndex, nMemberIndex);

                                                // Recursive call
                                                if (!ParseVariableAccessAttribute(dataBuffer, nDataOffset + nLength, ref nDataOffset, szMemberName, szMemberIndex))
                                                {
                                                    bRetValue = false;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            bRetValue = false;
                                        }
                                    }
                                }
                                else
                                {
                                    bRetValue = false;
                                }
                            }
                            else
                            {
                                bRetValue = false;
                            }
                        }
                        break;

                    default:
                        bRetValue = false;
                        break;
                }
            
            }
            else
            {
                bRetValue = false;
            }

            return (bRetValue);
        }    

        bool ParseDataSetDirectory(byte[] dataBuffer)
        {
            // Empty the maps of the data set elements
            m_mapIndexToDataSetElement.Clear();

            // Add the data set to the maps as index 0
            m_mapIndexToDataSetElement[0] = string.Format("{0}/{1}", m_szDataSetDevice, m_szDataSet);

            // Parse the elements of the data set and add them to the maps
            bool bElementParsed = true;
            uint nDataOffset = 0;
            uint nDataLength = (uint)dataBuffer.Length;
            for (uint nElementIndex = 1; bElementParsed && (nDataOffset < nDataLength); nElementIndex++)
            {
                string szAux = string.Empty;
                bElementParsed = ParseDataSetElement(dataBuffer, nDataLength, ref nDataOffset, out szAux);
                if (bElementParsed)
                    m_mapIndexToDataSetElement[nElementIndex] = szAux;
            }

            return (bElementParsed);
        }

        bool ReadControlBlock()
        {
            if (Station == null || Channel == null)
                return (false);

            byte[] DataBuffer = Channel.ReadReportControlBlock(m_szDeviceID, m_szMMSReportID);
            if (DataBuffer == null)
                return (false);

            if (!ParseControlBlockData(DataBuffer))
                return (false);

            return (true);
        }

        bool IsLeave(string szSearchString, List<string> pNodeArray)
        {
            return (pNodeArray.Count(n => (n.IndexOf(szSearchString) == 0)) > 1);
        }

        uint CountLeaves(List<string> pNodeArray)
        {
            uint nLeavesNumber = 0;
            for (int i = 0; i < pNodeArray.Count; i++)
            {
                if (IsLeave(pNodeArray[i], pNodeArray))
                    nLeavesNumber++;
            }
            return (nLeavesNumber);
        }

        void SkipInformationReportElement(ref uint nCumulativeIndex, string szSearchString)
        {
            List<string> pKeyArray = new List<string>();            
            foreach (var szValue in m_mapDataSetElementToStringIndex.Values)
            {
                //if (pos.vm_mapDataSetElementToStringIndex.GetNextAssoc(pos, szKey, szValue);
                int nIndex = szValue.IndexOf(szSearchString);
                if (nIndex == 0)
                    pKeyArray.Add(szValue);
            }

            uint nLeavesNumber = CountLeaves(pKeyArray);
            if (nLeavesNumber > 0)
                nCumulativeIndex += nLeavesNumber;            
        }

        public void ParseInformationReport(byte[] dataBuffer, uint nDataLength, uint nDataOffset, out bool noActiveJobs)
        {
            noActiveJobs = (m_mapActiveJobs.Count == 0);

            if (noActiveJobs)
            {
                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReport no active jobs");
                return;
            }

            // OptFlds
            uint nValue = 0;
            bool bFieldFound = ParseBitString(dataBuffer, ref nDataOffset, ref nValue);
            if (!bFieldFound)
            {
                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReport error parsing OptFlds");
                return;
            }
            ushort nOptFlds = (ushort)nValue;

            // SeqNum
            if ((nOptFlds & (ushort)Mask_OptFlds.SequenceNumber) != 0)
            {
                if (nDataOffset >= nDataLength)
                {
                    m_nLastReceivedItemIndex = 0;
                    m_nLastUpdatedStructFieldIndex = 0;                    

                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReport error Rep:ParsInfRep return 4");
                    return;
                }
                bFieldFound = ParseUnsigned(dataBuffer, ref nDataOffset, ref m_nSqNum);
                if (!bFieldFound)
                {
                    m_nLastReceivedItemIndex = 0;
                    m_nLastUpdatedStructFieldIndex = 0;
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReport error Rep:ParsInfRep return 5");
                    return;
                }
            }

            // TimeOfEntry
            if ((nOptFlds & (ushort)Mask_OptFlds.ReportTimeStamp) != 0)
            {
                if (nDataOffset >= nDataLength)
                {
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReport error Rep:ParsInfRep return 6");
                    return;
                }
                bFieldFound = ParseBinaryTime(dataBuffer, ref nDataOffset, ref m_dtTimeOfEntry);
                if (!bFieldFound)
                {
                    m_nLastReceivedItemIndex = 0;
                    m_nLastUpdatedStructFieldIndex = 0;
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReport error Rep:ParsInfRep return 7");
                    return;
                }
            }
            else
            {
                m_dtTimeOfEntry = DateTime.UtcNow;
            }

            System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - {0} - ParseInformationReport - TimeOfEntry: {1}", DateTime.Now, m_dtTimeOfEntry);

            //// convert device "local time" into Movicon UTC Time
            //m_dtTimeOfEntry = Channel.ConvertTimeToUtc(m_dtTimeOfEntry);

            // DatSet
            if ((nOptFlds & (ushort)Mask_OptFlds.DataSetName) !=0)
            {
                if (nDataOffset >= nDataLength)
                {
                    m_nLastReceivedItemIndex = 0;
                    m_nLastUpdatedStructFieldIndex = 0;
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReport error Rep:ParsInfRep return 8");
                    return;
                }
                string szDataSet = string.Empty;
                bFieldFound = ParseVisibleString(dataBuffer, ref nDataOffset, ref szDataSet);
                if (!bFieldFound)
                {
                    m_nLastReceivedItemIndex = 0;
                    m_nLastUpdatedStructFieldIndex = 0;
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReport error Rep:ParsInfRep return 9");
                    return;
                }

                int nCharIndex = szDataSet.IndexOf('/');
                if ((nCharIndex <= 0) || (nCharIndex == szDataSet.Length))
                {
                    m_nLastReceivedItemIndex = 0;
                    m_nLastUpdatedStructFieldIndex = 0;
                    m_nLastUpdatedStructFieldIndex = 0;
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReport error Rep:ParsInfRep return 10");
                    return;
                }
                else
                {
                    // Check the data set name
                    string szDataSetDevice = szDataSet.Substring(0, nCharIndex);
                    string szDataSetID = szDataSet.Substring(nCharIndex + 1);
                    // changed from Mov11 --> exit if report don't match
                    if (m_szDataSetDevice != szDataSetDevice || m_szDataSet != szDataSetID)
                    {
                        m_nLastReceivedItemIndex = 0;
                        //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReport error Rep:ParsInfRep return 11");
                        return;
                    }
                }
            }

            // BufOvfl
            bool bBufferOverflow = false;
            if ((nOptFlds & (ushort)Mask_OptFlds.BufferOverflow)!=0)
            {
                if (nDataOffset >= nDataLength)
                {
                    m_nLastReceivedItemIndex = 0;
                    m_nLastUpdatedStructFieldIndex = 0;
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReport error Rep:ParsInfRep return 12");
                    return;
                }
                bFieldFound = ParseBoolean(dataBuffer, ref nDataOffset,ref bBufferOverflow);
                if (!bFieldFound)
                {
                    m_nLastReceivedItemIndex = 0;
                    m_nLastUpdatedStructFieldIndex = 0;
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReport error Rep:ParsInfRep return 13");
                    return;
                }
            }

            // EntryID
            if ((nOptFlds & (ushort)Mask_OptFlds.EntryID) !=0)
            {
                if (nDataOffset >= nDataLength)
                {
                    m_nLastReceivedItemIndex = 0;
                    m_nLastUpdatedStructFieldIndex = 0;
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReport error Rep:ParsInfRep return 14");
                    return;
                }
                bFieldFound = ParseEntryID(dataBuffer, ref nDataOffset, ref m_szEntryID);
                if (!bFieldFound)
                {
                    m_nLastReceivedItemIndex = 0;
                    m_nLastUpdatedStructFieldIndex = 0;
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReport error Rep:ParsInfRep return 15");
                    return;
                }

                System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - {0} - ParseInformationReport - EntryID = {1}", DateTime.Now, m_szEntryID);
            }

            // ConfRev
            if ((nOptFlds & (ushort)Mask_OptFlds.ConfRevision) !=0)
            {
                if (nDataOffset >= nDataLength)
                {
                    m_nLastReceivedItemIndex = 0;
                    m_nLastUpdatedStructFieldIndex = 0;
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReport error Rep:ParsInfRep return 16");
                    return;
                }
                bFieldFound = ParseUnsigned(dataBuffer, ref nDataOffset, ref m_nConfRev);
                if (!bFieldFound)
                {
                    m_nLastReceivedItemIndex = 0;
                    m_nLastUpdatedStructFieldIndex = 0;
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReport error Rep:ParsInfRep return 17");
                    return;
                }
            }

            // SubSeqNum
            uint nSubSeqNum = 0;
            if ((nOptFlds & (ushort)Mask_OptFlds.Segmentation) !=0)
            {
                if (nDataOffset >= nDataLength)
                {
                    m_nLastReceivedItemIndex = 0;
                    m_nLastUpdatedStructFieldIndex = 0;
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReport error Rep:ParsInfRep return 18");
                    return;
                }
                bFieldFound = ParseUnsigned(dataBuffer, ref nDataOffset, ref nSubSeqNum);
                if (!bFieldFound)
                {
                    m_nLastReceivedItemIndex = 0;
                    m_nLastUpdatedStructFieldIndex = 0;
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReport error Rep:ParsInfRep return 19");
                    return;
                }
            }

            // MoreSegmentsFollow
            bool bMoreSegmentsFollow = false;
            if ((nOptFlds & (ushort)Mask_OptFlds.Segmentation) != 0)
            {
                if (nDataOffset >= nDataLength)
                {
                    m_nLastReceivedItemIndex = 0;
                    m_nLastUpdatedStructFieldIndex = 0;
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReport error Rep:ParsInfRep return 20");
                    return;
                }
                bFieldFound = ParseBoolean(dataBuffer, ref nDataOffset, ref bMoreSegmentsFollow);
                if (!bFieldFound)
                {
                    m_nLastReceivedItemIndex = 0;
                    m_nLastUpdatedStructFieldIndex = 0;
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReport error Rep:ParsInfRep return 21");
                    return;
                }
            }

            // Inclusion bit string
            if (nDataOffset >= nDataLength)
            {
                m_nLastReceivedItemIndex = 0;
                m_nLastUpdatedStructFieldIndex = 0;
                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReport error Rep:ParsInfRep return 22");
                return;
            }
            string szInclusionBitString = string.Empty;
            uint nNumberOfPresentItems = 0;
            bFieldFound = ParseBitString(dataBuffer, ref nDataOffset,ref szInclusionBitString, ref nNumberOfPresentItems);
            if (!bFieldFound)
            {
                m_nLastReceivedItemIndex = 0;
                m_nLastUpdatedStructFieldIndex = 0;
                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReport error Rep:ParsInfRep return 23");
                return;
            }
            if (nNumberOfPresentItems == 0)
            {
                m_nLastReceivedItemIndex = 0;
                m_nLastUpdatedStructFieldIndex = 0;
                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReport error Rep:ParsInfRep return 24");
                return;
            }
            uint nNumberOfDataSetElements = (uint)szInclusionBitString.Length;

//#if DEBUG
//            {
//                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
//                System.Diagnostics.Debug.WriteLine(String.Format("IEC61850 DEBUG - {0} - ParseInformationReport - nNumberOfDataSetElements: {1} - nNumberOfPresentItems: {2}", currentTime, nNumberOfDataSetElements, nNumberOfPresentItems));
//            }
//#endif

            // Data-reference(s)
            if ((nOptFlds & (ushort)Mask_OptFlds.DataReference) !=0)
            {
                for (uint i = 0; i < nNumberOfPresentItems; i++)
                {
                    string szDataReference = string.Empty;
                    if (nDataOffset >= nDataLength)
                    {
                        m_nLastReceivedItemIndex = 0;
                        m_nLastUpdatedStructFieldIndex = 0;
                        //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReport error Rep:ParsInfRep return 25");
                        return;
                    }
                    bFieldFound = ParseVisibleString(dataBuffer, ref nDataOffset, ref szDataReference);
                    if (!bFieldFound)
                    {
                        m_nLastReceivedItemIndex = 0;
                        m_nLastUpdatedStructFieldIndex = 0;
                        //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReport error Rep:ParsInfRep return 26");
                        return;
                    }
                }
            }

            // Values
            // Check if the information about the data set are available 
            if (m_mapStringIndexToDataSetElement.Count == 0)
            {
                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReport error Rep:ParsInfRep return 27");
                return;
            }

            // Get the name of the data set            
            if (!m_mapStringIndexToDataSetElement.ContainsKey("0"))
            {
                //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReport error Rep:ParsInfRep return 28");
                return;
            }
            string szDataSetElement = m_mapStringIndexToDataSetElement["0"];

            //// Now we are sure that the report has been successfully activated
            //if (!m_bIsActive)
            //    m_bIsActive = true;

            // Get the list of jobs associated to the whole data set
            List<IEC61850CommJob> pFullDataSetJobList = null;
            if (m_mapActiveJobs.ContainsKey(szDataSetElement))
                pFullDataSetJobList = m_mapActiveJobs[szDataSetElement];

            List<IEC61850CommJob> pJobList = new List<IEC61850CommJob>();
            // Pass the data to jobs associated to single elements of the data set                        
            uint nDataSetIndex = 1;
            uint nCumulativeIndex = 1;
            uint[] IndexArray = new uint[IEC61850Protocol.MAX_DATASET_NESTINGLEVEL];            
            nCumulativeIndex = m_nLastReceivedItemIndex + 1;

#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - {0} - ParseInformationReport - SeqNum = {1}", currentTime, m_nSqNum));
            }
#endif

            for (nDataSetIndex = m_nLastReceivedItemIndex + 1; (nDataSetIndex <= nNumberOfDataSetElements) && (nDataOffset < nDataLength); nDataSetIndex++)
            {
                // Check if the information about the element of the data set are
                // available
                string szSearchKey = nDataSetIndex.ToString();
                //TRACE(_T("REPDBG - Rep:ParsInfRep searching %u elem\n"), nDataSetIndex);
                if (!m_mapStringIndexToDataSetElement.ContainsKey(szSearchKey))
                {
                    //System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - ParseInformationReport error Rep:ParsInfRep elem {0} not found\n", nDataSetIndex));
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReport error Rep:ParsInfRep return 29");
#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - {0} - ParseInformationReport - Rep:ParsInfRep elem {1} not found", currentTime, nDataSetIndex));
                    }
#endif
                    return;
                }

                szDataSetElement = m_mapStringIndexToDataSetElement[szSearchKey];

#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - {0} - ParseInformationReport - szSearchKey: {1} - szDataSetElement: {2}", currentTime, szSearchKey, szDataSetElement));
                }
#endif

                // Element present in the report?
                if (szInclusionBitString[(int)nDataSetIndex - 1] == '1')
                {
                    // Fill the list of jobs corresponding to the current element of the
                    // data set
                    //TRACE(_T("REPDBG - Rep:ParsInfRep searching jobs for %s elem\n"), szDataSetElement);
                    List<IEC61850CommJob> pDataSetElementJobList = null;
                    if (m_mapActiveJobs.ContainsKey(szDataSetElement))
                        pDataSetElementJobList = m_mapActiveJobs[szDataSetElement];
                    pJobList.Clear();
                    if (pFullDataSetJobList != null)
                        pJobList.AddRange(pFullDataSetJobList);
                    if (pDataSetElementJobList != null)
                        pJobList.AddRange(pDataSetElementJobList);
                    IncrementIndexOfNestingLevel(ref IndexArray, 0);
#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - {0} - ParseInformationReport calling ParseInformationReportElement for: {1}", currentTime, szDataSetElement));
                    }
#endif
                    ParseInformationReportElement(ref pJobList, ref IndexArray, ref nCumulativeIndex, 0, ref szSearchKey, dataBuffer, nDataLength, m_dtTimeOfEntry, ref nDataOffset);
                }
                // Skip the element
                else
                {
#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - {0} - ParseInformationReport calling SkipInformationReportElement for: {1}", currentTime, szDataSetElement));
                    }
#endif
                    SkipInformationReportElement(ref nCumulativeIndex, szSearchKey);
                }
            }
        }

        void IncrementIndexes(ref uint[] pIndexesArray,uint nNestingLevel)
        {
            for (uint i = 0; i <= nNestingLevel; i++)
                pIndexesArray[i] = pIndexesArray[i] + 1;
        }

        void IncrementIndexOfNestingLevel(ref uint[] pIndexesArray, uint nNestingLevel)
        {
            // Increment the index of the current nesting level
            pIndexesArray[nNestingLevel] += 1;
            // Reset the indexes of the inner nesting levels
            for (uint i = (nNestingLevel+1); i < pIndexesArray.Count(); i++)
                pIndexesArray[i] = 0;
        }

        public void CopyInformationReportData(IEC61850CommJob job, ref uint[] pIndexArray, uint nCumulativeIndex, uint nNestingLevel, DateTime dt, ref byte[] vt)//, byte nMmsType, byte nPadding)
        {
            //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - {0} - CopyInformationReportData called", DateTime.Now);
            // Is the job valid and in use?
            if (!job.IsValid)// || !job.InUse)
                return;
            //// Get the tag associated to the job
            //POSITION pos = m_TagMap.GetStartPosition();
            //if (pos == NULL)
            //{
            //    return;
            //}
            //CTag* pTag = m_TagMap.GetNextValue(pos);
            //if (!pTag)
            //{
            //    return;
            //}

            // System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - CopyInformationReportData - Local Time {0}, DeviceTime {1}, Updated Item {2}, Value={3}", DateTime.UtcNow, dt, job.MMSDataItemCompletePath, string.Join(",", vt.Select(b => b.ToString("X2")))));

            //// Check the nesting level
            //if (nNestingLevel < job.nDataNestingLevel)
            //    return;

            ExecutedJobArgs eJob = new ExecutedJobArgs();
            eJob.Values = vt;
            eJob.Timestamp = dt;
            eJob.Job = job;            
            eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
            Channel.OnJobExecutedPublic(eJob);

            //CString szTagName = pTag->m_Variable;
            //uint nType = 0;
            //uint nSizeTmp = 0;

            //if (job.TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.String)
            // Is the job is associated to a variable of type structure? 
            //if (pTag->m_nVarType == _DRV_VAR_TYPE_STRUCT)
            //if ((uint)TagsList[0].TagNode.DataType.Identifier == (uint)BuiltInType.)
            //if (TagsList[0].TagNode.DataType.IdType == IdType.Guid) {
            // Build the structure field name
            //string szName = string.Empty;
            //if (!m_bIsFullReport)
            //{
            //    szName = string.Format("{0}#{1}", pTag->m_Variable,pIndexArray[m_nDataNestingLevel]);
            //}
            //else
            //{
            //    szName.Format(_T("%s#%d"), pTag->m_Variable, nCumulativeIndex);
            //}
            //nSizeTmp = 250;
            //bool bFieldFound = AfxGetDriverApp()->GetVariableTypeSize(szName.GetBuffer(250), nType, nSizeTmp);
            //if (!bFieldFound)
            //    return;

            //// Build the complete name of the field
            //szTagName += ':';
            //szTagName += szName;
            //}

            //if (szTagName.IsEmpty())
            //    return;

            //// Check type and size of the tag associated to the job
            //nType = 0;
            //nSizeTmp = 0;
            //bool bTagFound = AfxGetDriverApp()->GetVariableTypeSize(szTagName.GetBuffer(),nType, nSizeTmp);
            //szTagName.ReleaseBuffer();
            //if (!bTagFound)
            //    return;

            //// Pass data to the supervisor
            //WORD wQuality = OPC_QUALITY_GOOD;
            //BOOL bDataCopied = FALSE;
            //switch (nMmsType)
            //{
            //    case MMSDataType_Float32:
            //    case MMSDataType_Float64:
            //    case MMSDataType_Boolean:
            //    case MMSDataType_Integer8:
            //    case MMSDataType_Integer16:
            //    case MMSDataType_Integer32:
            //    case MMSDataType_UnsignedInteger8:
            //    case MMSDataType_UnsignedInteger16:
            //    case MMSDataType_UnsignedInteger32:
            //    case MMSDataType_VisibleString:
            //    case MMSDataType_MMSString:
            //    case MMSDataType_UTCTime:
            //    case MMSDataType_BinaryTime:
            //        AfxGetIEC61850App()->PostNewData(this);
            //        pTag->m_bFirstTime = false;
            //        pTag->m_dtTimeStamp = dt;
            //        TRACE(_T("REPDBG - J:CopInfRpDdt call (1) SetVarVal %s\n"), szTagName);
            //        AfxGetIEC61850App()->SetVariableValue(szTagName, vt, dt, &wQuality);
            //        m_bFirstTime = false;
            //        if (pTag->m_nVarType == _DRV_VAR_TYPE_STRUCT)
            //        {
            //            // Update timestamp and quality of the structure
            //            AfxGetIEC61850App()->SetVariableTimeStamp(pTag->m_Variable, dt,
            //                                                      &wQuality);
            //        }
            //        bDataCopied = TRUE;
            //        break;

            //    case MMSDataType_OctetString:
            //        {
            //            if (nType != _DRV_VAR_TYPE_STRING)
            //            {
            //                AfxGetIEC61850App()->PostNewData(this);
            //                pTag->m_bFirstTime = false;
            //                pTag->m_dtTimeStamp = dt;
            //                TRACE(_T("REPDBG - J:CopInfRpDdt call (2) SetVarVal %s\n"), szTagName);
            //                AfxGetIEC61850App()->SetVariableValue(szTagName, vt, dt,
            //                                                      &wQuality);
            //                m_bFirstTime = false;
            //                if (pTag->m_nVarType == _DRV_VAR_TYPE_STRUCT)
            //                {
            //                    // Update timestamp and quality of the structure
            //                    AfxGetIEC61850App()->SetVariableTimeStamp(
            //                                                        pTag->m_Variable,
            //                                                        dt, &wQuality);
            //                }
            //                bDataCopied = TRUE;
            //            }
            //            else
            //            {
            //                // String
            //                if ((vt.vt & VT_ARRAY) == VT_ARRAY)
            //                {
            //                    SAFEARRAY* pArray = vt.parray;
            //                    LONG lBound = 0, uBound = 0;

            //            ::SafeArrayGetLBound(pArray, 1, &lBound);

            //            ::SafeArrayGetUBound(pArray, 1, &uBound);

            //                    LPBYTE pData;
            //                    HRESULT hr = ::SafeArrayAccessData(V_ARRAY(&vt),
            //                                                       (void**)&pData);
            //                    if (SUCCEEDED(hr))
            //                    {
            //                        CComVariant vtOctetString;
            //                        CopyOctetStringToASCIIString(uBound, pData,
            //                                                     vtOctetString);
            //                        hr = ::SafeArrayUnaccessData(V_ARRAY(&vt));

            //                        AfxGetIEC61850App()->PostNewData(this);
            //                        pTag->m_bFirstTime = false;
            //                        pTag->m_dtTimeStamp = dt;
            //                        TRACE(_T("REPDBG - J:CopInfRpDdt call (3) SetVarVal %s\n"), szTagName);
            //                        AfxGetIEC61850App()->SetVariableValue(szTagName,
            //                                                              vtOctetString,
            //                                                              dt,
            //                                                              &wQuality);
            //                        m_bFirstTime = false;
            //                        if (pTag->m_nVarType == _DRV_VAR_TYPE_STRUCT)
            //                        {
            //                            // Update timestamp and quality of the structure
            //                            AfxGetIEC61850App()->SetVariableTimeStamp(
            //                                                        pTag->m_Variable,
            //                                                        dt, &wQuality);
            //                        }
            //                        bDataCopied = TRUE;
            //                    }
            //                }
            //            }
            //        }
            //        break;

            //    case MMSDataType_BitString:
            //        if ((vt.vt & VT_ARRAY) == VT_ARRAY)
            //        {
            //            SAFEARRAY* pArray = vt.parray;
            //            LONG lBound = 0, uBound = 0;

            //    ::SafeArrayGetLBound(pArray, 1, &lBound);

            //    ::SafeArrayGetUBound(pArray, 1, &uBound);

            //            LPBYTE pData;
            //            HRESULT hr = ::SafeArrayAccessData(V_ARRAY(&vt),
            //                                               (void**)&pData);
            //            if (SUCCEEDED(hr))
            //            {
            //                CComVariant vtBitString;
            //                if (nType == _DRV_VAR_TYPE_STRING)
            //                {
            //                    CopyBitStringToASCIIString(uBound, pData, nPadding,
            //                                               vtBitString);
            //                }
            //                else if (nType == _DRV_VAR_TYPE_ARRAY)
            //                {
            //                    LPBYTE pBuffer = new BYTE[uBound];
            //                    if (!pBuffer)
            //                    {
            //                        return;
            //                    }
            //                    memcpy(pBuffer, pData, uBound);
            //                    BYTE nAux = pBuffer[uBound - 1];
            //                    nAux >>= nPadding;
            //                    pBuffer[uBound - 1] = nAux;
            //                    PrepareVariant(vtBitString, pBuffer,
            //                                   _DRV_VAR_TYPE_ARRAY, uBound);
            //                    delete[] pBuffer;
            //                }
            //                else
            //                {
            //                    if (uBound == 1)
            //                    {
            //                        BYTE nAux = pData[0];
            //                        nAux >>= nPadding;
            //                        vtBitString.vt = VT_UI1;
            //                        vtBitString.bVal = nAux;
            //                    }
            //                    else if (uBound == 2)
            //                    {
            //                        WORD nAux = pData[0];
            //                        nAux <<= 8;
            //                        nAux += pData[1];
            //                        nAux >>= nPadding;
            //                        vtBitString.vt = VT_UI2;
            //                        vtBitString.uiVal = nAux;
            //                    }
            //                    else
            //                    {
            //                        UINT nValue = 0;
            //                        LONG i = 0;
            //                        for (i = 0; (i < uBound) && (i < 4); i++)
            //                        {
            //                            nValue = (nValue << 8) | pData[i];
            //                        }
            //                        nValue >>= nPadding;
            //                        vtBitString.vt = VT_UI4;
            //                        vtBitString.ulVal = nValue;
            //                    }
            //                }
            //                hr = ::SafeArrayUnaccessData(V_ARRAY(&vt));

            //                AfxGetIEC61850App()->PostNewData(this);
            //                pTag->m_bFirstTime = false;
            //                pTag->m_dtTimeStamp = dt;
            //                TRACE(_T("REPDBG - J:CopInfRpDdt call (4) SetVarVal %s\n"), szTagName);
            //                AfxGetIEC61850App()->SetVariableValue(szTagName,
            //                                                      vtBitString, dt,
            //                                                      &wQuality);
            //                m_bFirstTime = false;
            //                if (pTag->m_nVarType == _DRV_VAR_TYPE_STRUCT)
            //                {
            //                    // Update timestamp and quality of the structure
            //                    AfxGetIEC61850App()->SetVariableTimeStamp(
            //                                                        pTag->m_Variable,
            //                                                        dt, &wQuality);
            //                }
            //                bDataCopied = TRUE;
            //            }
            //        }
            //        break;
            //}

            //if (bDataCopied)
            //{
            //    if (m_bIsInError)
            //    {
            //        m_bIsInError = FALSE;
            //        m_pStation->DecrementTasksInError();
            //    }
            //    SetInError(false, 0, NULL);
            //}
        }

        public void PrepareInformationReportData(IEC61850CommJob job, ref uint[] pIndexArray, uint nCumulativeIndex, uint nNestingLevel, DateTime dt, ref byte[] vt, ref List<ExecutedJobArgs> structJobList)
        {
            if (!job.IsValid)
                return;

            ExecutedJobArgs eJob = new ExecutedJobArgs();
            eJob.Values = vt;
            eJob.Timestamp = dt;
            eJob.Job = job;
            eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
            structJobList.Add(eJob);
        }

        public void NotifyStructElementsData(List<ExecutedJobArgs> structEJobList, DateTime reportDt, DateTime structElementsDt)
        {
            DateTime jobSourceTimestamp = reportDt;
            if(!structElementsDt.Equals(DateTime.MinValue))
            {
                jobSourceTimestamp = structElementsDt;
            }
            foreach(ExecutedJobArgs eJob in structEJobList)
            {
                eJob.Timestamp = jobSourceTimestamp;
                Channel.OnJobExecutedPublic(eJob);
            }
        }

        void PassInformationReportDataToJobs(ref List<IEC61850CommJob> pDataSetJobs,ref uint[] pIndexArray,uint nCumulativeIndex,uint nNestingLevel,DateTime dt,byte[] vt)//,byte nMmsType,byte nPadding /*= 0*/)
        {
            //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - {0} - PassInformationReportDataToJobs called - pDataSetJobs.Count: {1}", DateTime.Now, pDataSetJobs.Count);
            if (pDataSetJobs != null)
            {
                foreach (var job in pDataSetJobs)
                {
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - {0} - PassInformationReportDataToJobs - job.nDataNestingLevel: {1} - nNestingLevel: {2}", DateTime.Now, job.nDataNestingLevel, nNestingLevel);
                    if (!(job.nDataNestingLevel > nNestingLevel))
                        CopyInformationReportData(job, ref pIndexArray, nCumulativeIndex, nNestingLevel, dt, ref vt);//, nMmsType,nPadding);
                }
            }
        }

        void PassInformationReportDataToStructJobs(ref List<IEC61850CommJob> pDataSetJobs, ref uint[] pIndexArray, uint nCumulativeIndex, uint nNestingLevel, DateTime dt, byte[] vt, ref List<ExecutedJobArgs> structJobList)
        {
            if (pDataSetJobs != null)
            {
                foreach (var job in pDataSetJobs)
                {
                    if (!(job.nDataNestingLevel > nNestingLevel))
                        PrepareInformationReportData(job, ref pIndexArray, nCumulativeIndex, nNestingLevel, dt, ref vt, ref structJobList);
                }
            }
        }


        void RemoveAlreadyParsedJobs(ref List<IEC61850CommJob>listJobs, uint nNestingLevel)
        {
            List<IEC61850CommJob> copyList = listJobs.FindAll(job => !(job.nDataNestingLevel >= nNestingLevel));
            listJobs.Clear();
            if (copyList.Count > 0)
                listJobs.AddRange(copyList);
        }


        void ParseInformationReportElement(ref List<IEC61850CommJob> pDataSetJobs, ref uint[] pIndexArray, ref uint nCumulativeIndex, uint nNestingLevel,ref string szSearchString, byte[] dataBuffer, uint nDataLength, DateTime dt, ref uint nDataOffset) {
            //TRACE(_T("REPDBG - PIRE called with parameters: n. Jobs = %u n. Indexes = %u Nest. Level = %u SearchString = %s Length = %u Off. = %u\n"),
            //    pDataSetJobs.GetCount(), pIndexArray.GetCount(),
            //    nNestingLevel, szSearchString, nDataLength, nDataOffset);
            if (nDataLength <= nDataOffset)
                return;

            //uint nStructIndex = 0;
            //uint nSubElementIndex = 0;
                        
            bool bCompleted = false;
            while (!bCompleted && (nDataLength > (nDataOffset + 1)))
            {
                byte dataType = dataBuffer[nDataOffset];
                switch (dataType)
                {
                    // Float
                    case 0x87:
                    // Bit
                    case 0x83:
                    // Integer
                    case 0x85:
                    // Unsigned Integer
                    case 0x86:
                    // Octet String
                    case 0x89:
                    // Visible String Success
                    case 0x8a:
                    // MMS String Success
                    case 0x90:
                    // Bit String
                    case 0x84:
                    // UTC Time
                    case 0x91:
                    // Binary Time
                    case 0x8c:
                        if (nDataLength >= (nDataOffset + 2))
                        {
                            uint StartDataOffet = nDataOffset;

                            nDataOffset++;                            
                            // Check the data length size
                            uint nLengthSize = 0;
                            IEC61850Protocol P = new IEC61850Protocol();
                            uint nLength = P.BerDecodeLength(dataBuffer, nDataOffset, nDataLength - nDataOffset, out nLengthSize);
                            // Empty string is an acceptable case
                            if ((nLength != 0) || (dataType == 0x89) || (dataType == 0x8a) || (dataType == 0x90))
                            //if (nLength != 0)
                            {
                                nDataOffset += nLengthSize;
                                if (nDataLength >= (nDataOffset + nLength))
                                {
                                    //byte nPadding = 0;
                                    //// Bit String
                                    //if (dataBuffer[StartDataOffet] == 0x84)
                                    //    nPadding = dataBuffer[nDataOffset++];

                                    byte[] vt = new byte[nDataOffset - StartDataOffet + nLength];
                                    Array.Copy(dataBuffer, StartDataOffet, vt, 0, vt.Length);
                                    PassInformationReportDataToJobs(ref pDataSetJobs, ref pIndexArray, nCumulativeIndex, nNestingLevel, dt, vt);//, 0, nPadding);
                                    nCumulativeIndex++;
                                    nDataOffset += nLength;
                                    bCompleted = true;
                                }
                                else
                                {
                                    // Parsing operation terminated
                                    nDataOffset = nDataLength;
                                }
                            }
                            else
                            {
                                // Parsing operation terminated
                                nDataOffset = nDataLength;
                            }
                        }
                        else
                        {
                            // Parsing operation terminated
                            nDataOffset = nDataLength;
                        }
                        break;
                    //Structure
                    case 0xa2:
                        if (nDataLength >= (nDataOffset + 2))
                        {
                            nDataOffset++;
                            // Check the data length size
                            uint nLengthSize = 0;
                            IEC61850Protocol P = new IEC61850Protocol();
                            uint nLength = P.BerDecodeLength(dataBuffer, nDataOffset, nDataLength - nDataOffset, out nLengthSize);
                            if (nLength > 0)
                            {
                                nDataOffset += nLengthSize;
                                if (nDataLength >= (nDataOffset + nLength))
                                {
                                    // Recursive calls for parsing the structure
                                    // elements
                                    // Timestamp of the structure elements
                                    DateTime structDt = new DateTime(DateTime.MinValue.Ticks);
                                    // List of jobs associated to the structure data of the report
                                    List<ExecutedJobArgs> structEJobList = new List<ExecutedJobArgs>();
                                    uint nStructureDataLength = nDataOffset + nLength;
                                    if ((nNestingLevel + 1) < IEC61850Protocol.MAX_DATASET_NESTINGLEVEL)
                                    {
                                        while (nDataOffset < nStructureDataLength)
                                        {
                                            IncrementIndexOfNestingLevel(ref pIndexArray, nNestingLevel + 1);
                                            RemoveAlreadyParsedJobs(ref pDataSetJobs, nNestingLevel + 1);

                                            // Check if the information about the
                                            // element of the data set are available                                            
                                            string szNewSearchString = string.Format("{0}.{1}", szSearchString, pIndexArray[nNestingLevel + 1] - 1);
                                            if (!m_mapStringIndexToDataSetElement.ContainsKey(szNewSearchString))
                                            {
                                                // Parsing operation terminated
                                                nDataOffset = nDataLength;
                                                break;
                                            }

                                            string szNewElement = m_mapStringIndexToDataSetElement[szNewSearchString];

#if DEBUG
                                            {
                                                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                                                System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - {0} - ParseInformationReportElement - szSearchKey: {1} - szNewSearchString: {2} - szNewElement: {3}", currentTime, szSearchString, szNewSearchString, szNewElement));
                                            }
#endif
                                            // Fill the list of jobs corresponding to
                                            // the current element of the data set                                            
                                            if (m_mapActiveJobs.ContainsKey(szNewElement))                                                
                                                pDataSetJobs.AddRange(m_mapActiveJobs[szNewElement]);
                                                                                        
                                            // Parse the structure elements
                                            ParseInformationReportStructureElement(ref pDataSetJobs, ref pIndexArray, ref nCumulativeIndex, nNestingLevel + 1, ref szNewSearchString, dataBuffer, nStructureDataLength, dt, ref nDataOffset, ref structDt, ref structEJobList);
                                        }
                                        bCompleted = true;
                                    }
                                    else
                                    {
                                        // Parsing operation terminated
                                        nDataOffset = nDataLength;
                                    }
#if DEBUG
                                    {
                                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                                        System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - {0} - ParseInformationReportElement - structEJobList.Count: {1}", currentTime, structEJobList.Count));
                                    }
#endif

                                    // If available, set the true timestamps for the data of the structure (if not, use the report global timestamp) and notify the received data
                                    if (structEJobList.Count > 0)
                                    {
                                        NotifyStructElementsData(structEJobList, dt, structDt);
                                        structEJobList.Clear();
                                    }
                                }
                                else
                                {
                                    // Parsing operation terminated
                                    nDataOffset = nDataLength;
                                }
                            }
                            else
                            {
                                // Parsing operation terminated
                                nDataOffset = nDataLength;
                            }
                        }
                        else
                        {
                            // Parsing operation terminated
                            nDataOffset = nDataLength;
                        }
                        break;
                }
            }
        }

        void ParseInformationReportStructureElement(ref List<IEC61850CommJob> pDataSetJobs, ref uint[] pIndexArray, ref uint nCumulativeIndex, uint nNestingLevel, ref string szSearchString, byte[] dataBuffer, uint nDataLength, DateTime dt, ref uint nDataOffset, ref DateTime structDt, ref List<ExecutedJobArgs> structEJobs)
        {
            if (nDataLength <= nDataOffset)
                return;

            bool bCompleted = false;
            while (!bCompleted && (nDataLength > (nDataOffset + 1)))
            {
                byte dataType = dataBuffer[nDataOffset];
                switch (dataType)
                {
                    // Float
                    case 0x87:
                    // Bit
                    case 0x83:
                    // Integer
                    case 0x85:
                    // Unsigned Integer
                    case 0x86:
                    // Octet String
                    case 0x89:
                    // Visible String Success
                    case 0x8a:
                    // MMS String Success
                    case 0x90:
                    // Bit String
                    case 0x84:
                    // UTC Time
                    case 0x91:
                    // Binary Time
                    case 0x8c:
                        if (nDataLength >= (nDataOffset + 2))
                        {
                            uint StartDataOffset = nDataOffset;

                            nDataOffset++;
                            // Check the data length size
                            uint nLengthSize = 0;
                            IEC61850Protocol P = new IEC61850Protocol();
                            uint nLength = P.BerDecodeLength(dataBuffer, nDataOffset, nDataLength - nDataOffset, out nLengthSize);
                            // Empty string is an acceptable case
                            if ((nLength != 0) || (dataType == 0x89) || (dataType == 0x8a) || (dataType == 0x90))
                            {
                                nDataOffset += nLengthSize;
                                if (nDataLength >= (nDataOffset + nLength))
                                {
                                    byte[] vt = new byte[nDataOffset - StartDataOffset + nLength];
                                    Array.Copy(dataBuffer, StartDataOffset, vt, 0, vt.Length);

                                    if (dataType == 0x8c)
                                    {
                                        P.ConvertBinaryTimeToDateTime(vt, ref structDt);
                                    }
                                    else if (dataType == 0x91)
                                    {
                                        P.ConvertUTCTimeToDateTime(vt, ref structDt);
                                    }

#if DEBUG
                                    {
                                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                                        string reportTime = dt.ToString("dd/MM/yyyy HH:mm:ss.fff");
                                        string structTime = structDt.ToString("dd/MM/yyyy HH:mm:ss.fff");
                                        System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - {0} - ParseInformationReportStructureElement calling PassInformationReportDataToStructJobs - dataType: {1} - Report Time: {2} - Value Time: {3}", currentTime, dataType, reportTime, structTime));
                                    }
#endif

                                    PassInformationReportDataToStructJobs(ref pDataSetJobs, ref pIndexArray, nCumulativeIndex, nNestingLevel, dt, vt, ref structEJobs);//, 0, nPadding);
                                    nCumulativeIndex++;
                                    nDataOffset += nLength;
                                    bCompleted = true;
                                }
                                else
                                {
                                    // Parsing operation terminated
                                    nDataOffset = nDataLength;
                                }
                            }
                            else
                            {
                                // Parsing operation terminated
                                nDataOffset = nDataLength;
                            }
                        }
                        else
                        {
                            // Parsing operation terminated
                            nDataOffset = nDataLength;
                        }
                        break;
                    //Structure
                    case 0xa2:
                        if (nDataLength >= (nDataOffset + 2))
                        {
                            nDataOffset++;

                            // Check the data length size
                            uint nLengthSize = 0;
                            IEC61850Protocol P = new IEC61850Protocol();
                            uint nLength = P.BerDecodeLength(dataBuffer, nDataOffset, nDataLength - nDataOffset, out nLengthSize);
                            if (nLength > 0)
                            {
                                nDataOffset += nLengthSize;
                                if (nDataLength >= (nDataOffset + nLength))
                                {
                                    // Recursive calls for parsing the structure
                                    // elements
                                    uint nStructureDataLength = nDataOffset + nLength;
                                    // Timestamp of the structure elements
                                    DateTime innerStructDt = new DateTime(DateTime.MinValue.Ticks);
                                    // List of jobs associated to the structure data of the report
                                    List<ExecutedJobArgs> innerStructEJobList = new List<ExecutedJobArgs>();
                                    if ((nNestingLevel + 1) < IEC61850Protocol.MAX_DATASET_NESTINGLEVEL)
                                    {
                                        while (nDataOffset < nStructureDataLength)
                                        {
                                            IncrementIndexOfNestingLevel(ref pIndexArray, nNestingLevel + 1);
#if DEBUG
                                            {
                                                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                                                int numberOfJobs = 0;
                                                if (pDataSetJobs != null)
                                                {
                                                    numberOfJobs = pDataSetJobs.Count;
                                                }
                                                System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - {0} - ParseInformationReportStructureElement - szSearchKey: {1} - nNestingLevel: {2} - pDataSetJobs.Count before RemoveAlreadyParsedJobs: {3}", currentTime, szSearchString, nNestingLevel, numberOfJobs));
                                            }
#endif
                                            RemoveAlreadyParsedJobs(ref pDataSetJobs, nNestingLevel + 1);
#if DEBUG
                                            {
                                                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                                                int numberOfJobs = 0;
                                                if (pDataSetJobs != null)
                                                {
                                                    numberOfJobs = pDataSetJobs.Count;
                                                }
                                                System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - {0} - ParseInformationReportStructureElement - szSearchKey: {1} - nNestingLevel: {2} - pDataSetJobs.Count after RemoveAlreadyParsedJobs: {3}", currentTime, szSearchString, nNestingLevel, numberOfJobs));
                                            }
#endif
                                            // Check if the information about the
                                            // element of the data set are available                                            
                                            string szNewSearchString = string.Format("{0}.{1}", szSearchString, pIndexArray[nNestingLevel + 1] - 1);
                                            if (!m_mapStringIndexToDataSetElement.ContainsKey(szNewSearchString))
                                            {
                                                // Parsing operation terminated
                                                nDataOffset = nDataLength;
                                                break;
                                            }

                                            string szNewElement = m_mapStringIndexToDataSetElement[szNewSearchString];
                                            //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ParseInformationReportElement - {0} - szSearchString: {1} - szNewSearchString: {2} - szNewElement: {3}", DateTime.Now, szSearchString, szNewSearchString, szNewElement);

#if DEBUG
                                            {
                                                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                                                System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - {0} - ParseInformationReportStructureElement - szSearchKey: {1} - szNewSearchString: {2} - szNewElement: {3}", currentTime, szSearchString, szNewSearchString, szNewElement));
                                            }
#endif
                                            // Fill the list of jobs corresponding to
                                            // the current element of the data set                                            
                                            if (m_mapActiveJobs.ContainsKey(szNewElement))
                                                pDataSetJobs.AddRange(m_mapActiveJobs[szNewElement]);

                                            // Recursive call
                                            ParseInformationReportStructureElement(ref pDataSetJobs, ref pIndexArray, ref nCumulativeIndex, nNestingLevel + 1, ref szNewSearchString, dataBuffer, nStructureDataLength, dt, ref nDataOffset, ref innerStructDt, ref innerStructEJobList);
                                        }
                                        bCompleted = true;
                                    }
                                    else
                                    {
                                        // Parsing operation terminated
                                        nDataOffset = nDataLength;
                                    }
#if DEBUG
                                    {
                                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                                        System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - {0} - ParseInformationReportStructureElement - innerStructEJobList.Count: {1}", currentTime, innerStructEJobList.Count));
                                    }
#endif
                                    // Any parsed value?
                                    if (innerStructEJobList.Count > 0)
                                    {
                                        // If available, set the true timestamp for the data of the structure and notify the received data
                                        if (!innerStructDt.Equals(DateTime.MinValue))
                                        {
                                            NotifyStructElementsData(innerStructEJobList, dt, innerStructDt);
                                        }
                                        // else add the parsed values to the the value list of the parent structure 
                                        else
                                        {
                                            structEJobs.AddRange(innerStructEJobList);
                                        }
                                        innerStructEJobList.Clear();
                                    }
                                }
                                else
                                {
                                    // Parsing operation terminated
                                    nDataOffset = nDataLength;
                                }
                            }
                            else
                            {
                                // Parsing operation terminated
                                nDataOffset = nDataLength;
                            }
                        }
                        else
                        {
                            // Parsing operation terminated
                            nDataOffset = nDataLength;
                        }
                        break;
                }
            }
        }


        bool ParseDataSetElement(byte[] dataBuffer, uint nDataLength, ref uint nDataOffset, out string szMMSElementID)
        {
            szMMSElementID = String.Empty;

            // Check the data header
            if (dataBuffer[nDataOffset] != 0x30)
            {
                return (false);
            }
            if (nDataLength < (nDataOffset + 2))
            {
                return (false);
            }
            nDataOffset++;
            IEC61850Protocol P = new IEC61850Protocol();
            uint nLengthSize = 0;
            uint nLength = P.BerDecodeLength(dataBuffer, nDataOffset, nDataLength - nDataOffset, out nLengthSize);
            nDataOffset += nLengthSize;
            if (nDataLength < (nDataOffset + 2))
            {
                return (false);
            }
            if (dataBuffer[nDataOffset] != 0xa0)
            {
                return (false);
            }
            nDataOffset++;
            if (nDataLength < (nDataOffset + 2))
            {
                return (false);
            }
            nLengthSize = 0;
            nLength = P.BerDecodeLength(dataBuffer, nDataOffset, nDataLength - nDataOffset, out nLengthSize);
            nDataOffset += nLengthSize;
            if (nDataLength < (nDataOffset + 2))
            {
                return (false);
            }
            if (dataBuffer[nDataOffset] != 0xa1)
            {
                return (false);
            }
            nDataOffset++;
            if (nDataLength < (nDataOffset + 2))
            {
                return (false);
            }
            nLengthSize = 0;
            nLength = P.BerDecodeLength(dataBuffer, nDataOffset, nDataLength - nDataOffset, out nLengthSize);
            nDataOffset += nLengthSize;
            if (nDataLength < (nDataOffset + 2))
            {
                return (false);
            }

            // Parse the Device ID
            if (dataBuffer[nDataOffset] != 0x1a)
            {
                return (false);
            }
            nDataOffset++;
            if (nDataLength < (nDataOffset + 2))
            {
                return (false);
            }
            nLengthSize = 0;
            nLength = P.BerDecodeLength(dataBuffer, nDataOffset, nDataLength - nDataOffset, out nLengthSize); ;
            nDataOffset += nLengthSize;
            if ((nLength == 0) || (nDataLength < (nDataOffset + nLength)))
            {
                return (false);
            }
            string szAux = String.Empty;
            for (int i = 0; i < (int)nLength; i++)
            {
                szAux += (char)dataBuffer[nDataOffset++];
            }

            // Parse the Element ID
            if (nDataLength < (nDataOffset + 2))
            {
                return (false);
            }
            if (dataBuffer[nDataOffset] != 0x1a)
            {
                return (false);
            }
            nDataOffset++;
            if (nDataLength < (nDataOffset + 2))
            {
                return (false);
            }
            nLengthSize = 0;
            nLength = P.BerDecodeLength(dataBuffer, nDataOffset, nDataLength - nDataOffset, out nLengthSize);
            nDataOffset += nLengthSize;
            if ((nLength == 0) || (nDataLength < (nDataOffset + nLength)))
            {
                return (false);
            }
            string szAux2 = String.Empty;
            for (int i = 0; i < (int)nLength; i++)
            {
                szAux2 += (char)dataBuffer[nDataOffset++];
            }

            // Set the Data Set Element
            szMMSElementID = string.Format("{0}/{1}", szAux, szAux2);

            return (true);
        }

        private bool ParseControlBlockData(byte[] dataBuffer)
        {
            ResetDataMembers();

            switch (m_nReportType)
            {
                case ReportTypes.Unbuffered:
                    return ParseUnbufferedControlBlockData(dataBuffer);

                case ReportTypes.Buffered:
                    return ParseBufferedControlBlockData(dataBuffer);

                default:
                    return false;
            }
        }

        bool ParseUnbufferedControlBlockData(byte[] dataBuffer)
        {
            bool bFieldFound = true;
            uint nDataOffset = 0;
            uint nDataLength = (uint)dataBuffer.Length;
            
            for (int nFieldIndex = 0; bFieldFound && (nDataOffset < nDataLength) && (nFieldIndex < 12); nFieldIndex++) {
                uint nValue = 0;
                switch (nFieldIndex)
                {
                    // RptID
                    case 0:
                        bFieldFound = ParseVisibleString(dataBuffer, ref nDataOffset, ref m_szRptID);                        
                        break;

                    // RptEna
                    case 1:
                        bFieldFound = ParseBoolean(dataBuffer, ref nDataOffset, ref m_bRptEna);
                        break;

                    // Resv
                    case 2:
                        bFieldFound = ParseBoolean(dataBuffer, ref nDataOffset, ref m_bResv);
                        break;

                    // DataSet
                    case 3:
                        string szDataSet = String.Empty;
                        bFieldFound = ParseVisibleString(dataBuffer, ref nDataOffset, ref szDataSet);
                        if (bFieldFound)
                        {
                            // The data set identifier must be domain - specific
                            int nCharIndex = szDataSet.IndexOf('/');
                            if ((nCharIndex <= 0) || (nCharIndex == szDataSet.Length))
                            {
                                bFieldFound = false;
                            }
                            else
                            {
                                m_szDataSetDevice = szDataSet.Substring(0, nCharIndex);
                                m_szDataSet = szDataSet.Substring(nCharIndex + 1);
                            }
                        }
                        break;

                    // ConfRev
                    case 4:
                        bFieldFound = ParseUnsigned(dataBuffer, ref nDataOffset, ref m_nConfRev);
                        break;

                    // OptFlds
                    case 5:
                        bFieldFound = ParseBitString(dataBuffer, ref nDataOffset, ref nValue);
                        if (bFieldFound)
                            m_nOptFlds = (ushort)nValue;
                        break;                        

                    // BufTm
                    case 6:
                        bFieldFound = ParseUnsigned(dataBuffer, ref nDataOffset, ref m_nBufTm);
                        break;

                    // SqNum
                    case 7:
                        bFieldFound = ParseUnsigned(dataBuffer, ref nDataOffset, ref m_nSqNum);
                        break;

                    // TrgOps
                    case 8:
                        bFieldFound = ParseBitString(dataBuffer, ref nDataOffset, ref nValue);
                        if (bFieldFound)
                            m_nTrgOps = (byte)nValue;
                        break;

                    // SqNum
                    case 9:
                        bFieldFound = ParseUnsigned(dataBuffer, ref nDataOffset, ref m_nIntgPd);
                        break;

                    // GI
                    case 10:
                        bFieldFound = ParseBoolean(dataBuffer, ref nDataOffset, ref m_bGI);
                        break;

                    // Owner
                    case 11:
                        bFieldFound = ParseIpAddress(dataBuffer, ref nDataOffset, ref m_szOwner);
                        break;
                }
            }

            return (bFieldFound);
        }


        bool ParseBufferedControlBlockData(byte[] dataBuffer)
        {
            bool bFieldFound = true;
            int nDataLength = dataBuffer.Count();
            uint nDataOffset = 0;
            uint nValue = 0;
            for (int nFieldIndex = 0; bFieldFound && (nDataOffset < nDataLength) && (nFieldIndex < 15);nFieldIndex++) {
                switch (nFieldIndex)
                {
                    // RptID
                    case 0:
                        bFieldFound = ParseVisibleString(dataBuffer, ref nDataOffset, ref m_szRptID);                        
                        break;

                    // RptEna
                    case 1:
                        bFieldFound = ParseBoolean(dataBuffer, ref nDataOffset, ref m_bRptEna);
                        break;

                    // DataSet
                    case 2:
                        {
                            string szDataSet = string.Empty;
                            bFieldFound = ParseVisibleString(dataBuffer, ref nDataOffset, ref szDataSet);
                            if (bFieldFound)
                            {
                                // The data set identifier must be domain - specific
                                int nCharIndex = szDataSet.IndexOf('/');
                                if ((nCharIndex <= 0) || (nCharIndex == (int)szDataSet.Length)) {
                                    bFieldFound = false;
                                }
                                else
                                {
                                    m_szDataSetDevice = szDataSet.Substring(0, nCharIndex);
                                    m_szDataSet = szDataSet.Substring(nCharIndex + 1);
                                }
                            }
                        }
                        break;

                    // ConfRev
                    case 3:
                        bFieldFound = ParseUnsigned(dataBuffer, ref nDataOffset, ref m_nConfRev);
                        break;

                    // OptFlds
                    case 4:
                        nValue = 0;
                        bFieldFound = ParseBitString(dataBuffer, ref nDataOffset, ref nValue);
                        if (bFieldFound)
                            m_nOptFlds = (ushort)(nValue);                    
                        break;

                    // BufTm
                    case 5:
                        bFieldFound = ParseUnsigned(dataBuffer, ref nDataOffset, ref m_nBufTm);
                        break;

                    // SqNum
                    case 6:
                        bFieldFound = ParseUnsigned(dataBuffer, ref nDataOffset, ref m_nSqNum);
                        break;

                    // TrgOps
                    case 7:
                        nValue = 0;
                        bFieldFound = ParseBitString(dataBuffer, ref nDataOffset, ref nValue);
                        if (bFieldFound)
                            m_nTrgOps = (byte)(nValue);
                        break;

                    // IntgPd
                    case 8:
                        bFieldFound = ParseUnsigned(dataBuffer, ref nDataOffset, ref m_nIntgPd);
                        break;

                    // GI
                    case 9:
                        bFieldFound = ParseBoolean(dataBuffer, ref nDataOffset, ref m_bGI);
                        break;

                    // PurgeBuf
                    case 10:
                        bFieldFound = ParseBoolean(dataBuffer, ref nDataOffset, ref m_bPurgeBuf);
                        break;

                    // EntryID
                    case 11:
                        bFieldFound = ParseEntryID(dataBuffer, ref nDataOffset, ref m_szEntryID);
                        break;

                    // TimeOfEntry
                    case 12:
                        bFieldFound = ParseBinaryTime(dataBuffer, ref nDataOffset, ref m_dtTimeOfEntry);
                        break;

                    // ResvTms
                    case 13:
                        bFieldFound = ParseInteger(dataBuffer, ref  nDataOffset, ref m_nResvTms);
                        break;

                    // Owner
                    case 14:
                        bFieldFound = ParseIpAddress(dataBuffer, ref nDataOffset, ref m_szOwner);
                        break;
                }
            }

            return (bFieldFound);
        }

        bool ParseIpAddress(byte[] dataBuffer, ref uint nDataOffset, ref string szValue)
        {
            szValue = String.Empty;

            // The data type should be "Octet String" 
            if (dataBuffer[nDataOffset] != 0x89)
                return (false);

            uint nDataLength = (uint)dataBuffer.Length;
            if (nDataLength < (nDataOffset + 2))
                return (false);

            // Check the data length
            nDataOffset++;
            IEC61850Protocol P = new IEC61850Protocol();
            uint nLengthSize = 0;
            uint nLength = P.BerDecodeLength(dataBuffer, nDataOffset, nDataLength - nDataOffset, out nLengthSize);
            // Empty string is an acceptable case
            nDataOffset += nLengthSize;
            if ((nLength == 4) && (nDataLength >= (nDataOffset + nLength)))
            {
                // Copy the read value
                szValue = string.Format("{0}.{1}.{2}.{3}", (uint)dataBuffer[nDataOffset], (uint)dataBuffer[nDataOffset + 1], (uint)dataBuffer[nDataOffset + 2], (uint)dataBuffer[nDataOffset + 3]);
                nDataOffset += 4;
            }

            return (true);
        }

        bool TestBit(byte ptr, int bitnum)
        {
            return ((ptr & (1 << bitnum)) != 0);
        }


        bool ParseBitString(byte[] dataBuffer, ref uint nDataOffset, ref string szBitString, ref uint nNumberOfPresentItems)
        {
            szBitString = string.Empty;
            nNumberOfPresentItems = 0;

            // The data type should be "Bit String" 
            if (dataBuffer[nDataOffset] != 0x84)
            {
                return (false);
            }

            uint nDataLength = (uint)dataBuffer.Length;
            if (nDataLength < (nDataOffset + 2))
            {
                return (false);
            }

            // Check the data length
            nDataOffset++;
            IEC61850Protocol P = new IEC61850Protocol();
            uint nLengthSize = 0;
            uint nLength = P.BerDecodeLength(dataBuffer, nDataOffset, nDataLength - nDataOffset, out nLengthSize);
            nDataOffset += nLengthSize;
            if ((nLength < 2) || (nDataLength < (nDataOffset + nLength)))
            {
                return (false);
            }

            // Get the padding byte
            byte nPadding = dataBuffer[nDataOffset++];
            nLength--;

            // Copy the read value to a string            
            int j = 0;
            uint nBitNumber = 0;
            int nBitLimit = 0;
            for (uint i = 0; i < (int)nLength; i++)
            {
                if ((i == (int)(nLength - 1)) && (nPadding < 8))
                {
                    nBitLimit = (int)nPadding;
                }
                for (j = 7; j >= nBitLimit; j--)
                {
                    if (TestBit(dataBuffer[nDataOffset + nBitNumber / 8], j))
                    {
                        szBitString += '1';
                        nNumberOfPresentItems++;
                    }
                    else
                    {
                        szBitString += '0';
                    }
                    nBitNumber++;
                }
            }

            nDataOffset += nLength;

            return (true);
        }

        bool ParseBitString(byte[] dataBuffer, ref uint nDataOffset, ref uint nValue)
        {
            nValue = 0;

            // The data type should be "Bit String" 
            if (dataBuffer[nDataOffset] != 0x84)
            {
                return (false);
            }

            uint nDataLength = (uint)dataBuffer.Length;
            if (nDataLength < (nDataOffset + 2))
            {
                return (false);
            }

            // Check the data length
            nDataOffset++;
            IEC61850Protocol P = new IEC61850Protocol();
            uint nLengthSize = 0;
            uint nLength = P.BerDecodeLength(dataBuffer, nDataOffset, nDataLength - nDataOffset, out nLengthSize);
            nDataOffset += nLengthSize;
            if ((nLength < 2) || (nLength > 5) || (nDataLength < (nDataOffset + nLength)))
            {
                return (false);
            }

            // Skip the padding byte
            nDataOffset++;
            nLength--;

            // Copy the read value
            int i = 0;
            for (i = 0; i < (int)nLength; i++)
            {
                nValue <<= 8;
                nValue += dataBuffer[nDataOffset++];
            }

            return (true);
        }

        bool ParseEntryID(byte[] dataBuffer, ref uint nDataOffset, ref string szValue)
        {
            szValue = string.Empty;

            uint nDataLength = (uint)dataBuffer.Length;
            // The data type should be "Octet String" 
            if (dataBuffer[nDataOffset] != 0x89)
            {
                return (false);
            }

            if (nDataLength < (nDataOffset + 2))
            {
                return (false);
            }

            // Check the data length
            nDataOffset++;
            IEC61850Protocol P = new IEC61850Protocol();
            uint nLengthSize = 0;
            uint nLength = P.BerDecodeLength(dataBuffer, nDataOffset, nDataLength - nDataOffset, out nLengthSize);
            nDataOffset += nLengthSize;
            if ((nLength == 8) && (nDataLength >= (nDataOffset + nLength)))
            {
                // Copy the read value                
                for (int i = 0; i < (int)nLength; i++)
                {                    
                    szValue += dataBuffer[nDataOffset++].ToString();
                }
            }

            return (true);
        }

        bool ParseUnsigned(byte[] dataBuffer, ref uint nDataOffset, ref uint nValue)
        {
            nValue = 0;

            // The data type should be "Unsigned" 
            if (dataBuffer[nDataOffset] != 0x86)
            {
                return (false);
            }

            uint nDataLength = (uint)dataBuffer.Length;
            if (nDataLength < (nDataOffset + 2))
            {
                return (false);
            }
            
            // Check the data length
            nDataOffset++;
            IEC61850Protocol P = new IEC61850Protocol();
            uint nLengthSize = 0;
            uint nLength = P.BerDecodeLength(dataBuffer, nDataOffset, nDataLength - nDataOffset, out nLengthSize);
            nDataOffset += nLengthSize;
            if ((nLength == 0) || (nLength > 4) || (nDataLength < (nDataOffset + nLength)))
            {
                return (false);
            }

            // Copy the read value
            int i = 0;
            for (i = 0; i < (int)nLength; i++)
            {
                nValue <<= 8;
                nValue += dataBuffer[nDataOffset++];
            }

            return (true);
        }

        bool ParseInteger(byte[] dataBuffer, ref uint nDataOffset, ref uint nValue)
        {
            nValue = 0;

            // The data type should be "Integer" 
            if ((dataBuffer[nDataOffset] != 0x85) && (dataBuffer[nDataOffset] != 0x86))
            {
                return (false);
            }

            uint nDataLength = (uint)dataBuffer.Length;
            if (nDataLength < (nDataOffset + 2))
            {
                return (false);
            }

            // Check the data length
            nDataOffset++;
            IEC61850Protocol P = new IEC61850Protocol();
            uint nLengthSize = 0;
            uint nLength = P.BerDecodeLength(dataBuffer, nDataOffset, nDataLength - nDataOffset, out nLengthSize);
            nDataOffset += nLengthSize;
            if ((nLength == 0) || (nLength > 4) || (nDataLength < (nDataOffset + nLength)))
            {
                return (false);
            }

            // Copy the read value
            int i = 0;
            for (i = 0; i < (int)nLength; i++)
            {
                nValue <<= 8;
                nValue += dataBuffer[nDataOffset++];
            }

            return (true);
        }

        bool ParseBoolean(byte[] dataBuffer, ref uint nDataOffset, ref bool bValue)
        {
            bValue = false;

            // The data type should be "Boolean" 
            if (dataBuffer[nDataOffset] != 0x83)
            {
                return (false);
            }

            uint nDataLength = (uint)dataBuffer.Length;
            if (nDataLength < (nDataOffset + 2))
            {
                return (false);
            }

            // Check the data length
            nDataOffset++;
            IEC61850Protocol P = new IEC61850Protocol();
            uint nLengthSize = 0;
            uint nLength = P.BerDecodeLength(dataBuffer, nDataOffset, nDataLength - nDataOffset, out nLengthSize);
            nDataOffset += nLengthSize;
            if ((nLength == 0) || (nDataLength < (nDataOffset + nLength)))
            {
                return (false);
            }

            // Copy the read value
            if (dataBuffer[nDataOffset++] != 0)
            {
                bValue = true;
            }
            else
            {
                bValue = false;
            }

            return (true);
        }

        bool ParseVisibleString(byte[] dataBuffer, ref uint nDataOffset, ref string szValue)
        {
            szValue = String.Empty;

            // The data type should be "Visible String" 
            if (dataBuffer[nDataOffset] != 0x8a)
            {
                return (false);
            }
            uint nDataLength = (uint)dataBuffer.Length;
            if (nDataLength < (nDataOffset + 2))
            {
                return (false);
            }

            // Check the data length
            nDataOffset++;
            IEC61850Protocol P = new IEC61850Protocol();
            uint nLengthSize = 0;
            uint nLength = P.BerDecodeLength(dataBuffer, nDataOffset, nDataLength - nDataOffset, out nLengthSize);

            // Empty string is an acceptable case
            nDataOffset += nLengthSize;
            if ((nLength > 0) && (nDataLength >= (nDataOffset + nLength)))
            {
                // Copy the read value
                int i = 0;
                for (i = 0; i < (int)nLength; i++)
                {
                    szValue += (char)dataBuffer[nDataOffset++];
                }
            }

            return (true);
        }

        bool ParseBinaryTime(byte[] dataBuffer, ref uint nDataOffset, ref DateTime dtValue)
        {
            // The data type should be "Binary Time" 
            if (dataBuffer[nDataOffset] != 0x8c)
                return (false);

            uint nDataLength = (uint)dataBuffer.Length;
            if (nDataLength < (nDataOffset + 2))
                return (false);

            // Check the data length
            nDataOffset++;
            uint nLengthSize = 0;
            IEC61850Protocol P = new IEC61850Protocol();            
            uint nLength = P.BerDecodeLength(dataBuffer, nDataOffset, nDataLength - nDataOffset, out nLengthSize);
            nDataOffset += nLengthSize;
            if ((nLength != 6) || (nDataLength < (nDataOffset + nLength)))
            {
                return (false);
            }

            byte[] timeBuffer = new byte[nLength];
            Array.Copy(dataBuffer, nDataOffset, timeBuffer, 0, nLength);
            dtValue = P.ConvertBinaryTimeToDateTime(nLength, timeBuffer);

            nDataOffset += nLength;

            return (true);
        }
        
        void ResetDataMembers()
        {
            m_bIsActive = false;
            m_bRptEna = false;
            m_bResv = false;
            m_nConfRev = 0;
            m_nOptFlds = 0;
            m_nBufTm = 0;
            m_nSqNum = 0;
            m_nTrgOps = 0;
            m_nIntgPd = 0;
            m_bGI = false;
            m_bPurgeBuf = false;
            m_nResvTms = 0;
            m_nLastReceivedItemIndex = 0;
        }
                        

        bool _ActivateReport(bool bEnable)
        {
            // Set the RptEna Element of the RCB
            byte[] pDataBuffer = new byte[1];
            if (bEnable)
                pDataBuffer[0] = 0xff;
            else
                pDataBuffer[0] = 0;

            string szItemID = string.Format("{0}$RptEna", m_szMMSReportID);

            return (Channel.WriteRCBItem(szItemID, m_szDeviceID, MMSDataTypes.Boolean, pDataBuffer, 0));
        }
    }
    #endregion
}

