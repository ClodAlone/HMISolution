////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	IEC60870_5_104CommJob.cs
//
// summary:	Implements the driver IEC60870_5_104 communications job class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using DriverCodeBase;
using Opc.Ua;
using DriverCodeBase.Enumerators;
using DriverBaseInterfaces;

namespace IEC60870_5_104
{
    /// <summary>   Protocol's task of the IEC60870_5_104 driver. </summary>
    public class IEC60870_5_104CommJob : CommJob
    {
        #region Constructors
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the IEC60870_5_104CommJob. </summary>
        ///
        /// <param name="station">  assigned to an object of type Station. </param>
        /// <param name="settings"> set with an object of type IEC60870_5_104CommJobSettings. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public IEC60870_5_104CommJob(Station station, IEC60870_5_104CommJobSettings settings)
            : base(station, settings)
        {
            _ASDUType = settings.ASDUType;
            _StartAddMon = settings.StartAddMon;
            _StartAddCtrl = settings.StartAddCtrl;
            _CmdQualifier = settings.CmdQualifier;
            _ParamQualifier = settings.ParamQualifier;
            _CmdAction = settings.CmdAction;
            _CotVariableName = settings.CotVariableName;
            _CotVariableId = settings.CotVariableId;
            _QualityVariableName = settings.QualityVariableName;
            _QualityVariableId = settings.QualityVariableId;
            _UpdateTimeStamp = settings.UpdateTimeStamp;
            _WriteTimeStamp = settings.WriteTimeStamp;
            _FileNameVariableName = settings.FileNameVariableName;
            _FileNameVariableId = settings.FileNameVariableId;
            _JobSelectAndCallQualifier = (byte)SelectAndCallQualifier.defaultValue;
            _FileSection = 0; // Default value
            _FileLength = 0; // Default value
            _CurrentFileLength = 0; // Default value
            _JobAcknowledgeFileQualifier = (byte)AcknowledgeFileQualifier.notUsed;
            _FileChecksum = 0;
            _SectionChecksum = 0;
            if (!String.IsNullOrEmpty(_CotVariableName) && !String.IsNullOrEmpty(_CotVariableId))
            {
                jobCotVariable = new StateCommandVariable(_CotVariableName, _CotVariableId);
                CotVariableHasBeenSet = true;
            }
            else
            {
                CotVariableHasBeenSet = false;
            }
            if (!String.IsNullOrEmpty(_QualityVariableName) && !String.IsNullOrEmpty(_QualityVariableId))
            {
                jobQualityVariable = new StateCommandVariable(_QualityVariableName, _QualityVariableId);
                QualityVariableHasBeenSet = true;
            }
            else
            {
                QualityVariableHasBeenSet = false;
            }
            if (!String.IsNullOrEmpty(_FileNameVariableName) && !String.IsNullOrEmpty(_FileNameVariableId))
            {
                jobFileNameVariable = new StateCommandVariable(_FileNameVariableName, _FileNameVariableId);
                FileNameVariableHasBeenSet = true;
            }
            else
            {
                FileNameVariableHasBeenSet = false;
            }
            _CommandWriteType = CommandType.Operate;
            CheckJobValid();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the IEC60870_5_104CommJob. </summary>
        ///
        /// <param name="station">  assigned to an object of type Station. </param>
        /// <param name="defTag">   set with an object of type IEC60870_5_104Tag. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public IEC60870_5_104CommJob(Station station, IEC60870_5_104Tag defTag)
            : base(station, defTag)
        {
            _ASDUType = defTag.IEC60870_5_104DynSettings.ASDUType;
            _StartAddMon = defTag.IEC60870_5_104DynSettings.StartAddMon;
            _StartAddCtrl = defTag.IEC60870_5_104DynSettings.StartAddCtrl;
            _CmdQualifier = defTag.IEC60870_5_104DynSettings.CmdQualifier;
            _ParamQualifier = defTag.IEC60870_5_104DynSettings.ParamQualifier;
            _CmdAction = defTag.IEC60870_5_104DynSettings.CmdAction;
            _CotVariableName = defTag.IEC60870_5_104DynSettings.CotVariableName;
            _CotVariableId = defTag.IEC60870_5_104DynSettings.CotVariableId;
            _QualityVariableName = defTag.IEC60870_5_104DynSettings.QualityVariableName;
            _QualityVariableId = defTag.IEC60870_5_104DynSettings.QualityVariableId;
            _UpdateTimeStamp = defTag.IEC60870_5_104DynSettings.UpdateTimeStamp;
            _WriteTimeStamp = defTag.IEC60870_5_104DynSettings.WriteTimeStamp;
            _FileNameVariableName = defTag.IEC60870_5_104DynSettings.FileNameVariableName;
            _FileNameVariableId = defTag.IEC60870_5_104DynSettings.FileNameVariableId;
            _JobSelectAndCallQualifier = (byte)SelectAndCallQualifier.defaultValue;
            _JobAcknowledgeFileQualifier = (byte)AcknowledgeFileQualifier.notUsed;
            _FileSection = 0; // Default value
            _FileLength = 0; // Default value
            _CurrentFileLength = 0; // Default value
            _FileChecksum = 0;
            _SectionChecksum = 0;

            if ((ElementNumber > 0 && !isProtocolBool()) || ProtocolDataSizeBig())
            {
                if (defTag.TagNode.ArrayDimension == 0)
                    TotalJobSize = GetProtocolDataByteSize();
                else
                    TotalJobSize = GetProtocolDataByteSize() * defTag.TagNode.ArrayDimension;
            }

            if (!String.IsNullOrEmpty(_CotVariableName) && !String.IsNullOrEmpty(_CotVariableId))
            {
                jobCotVariable = new StateCommandVariable(_CotVariableName, _CotVariableId);
                CotVariableHasBeenSet = true;
            }
            else
            {
                CotVariableHasBeenSet = false;
            }
            if (!String.IsNullOrEmpty(_QualityVariableName) && !String.IsNullOrEmpty(_QualityVariableId))
            {
                jobQualityVariable = new StateCommandVariable(_QualityVariableName, _QualityVariableId);
                QualityVariableHasBeenSet = true;
            }
            else
            {
                QualityVariableHasBeenSet = false;
            }
            if (!String.IsNullOrEmpty(_FileNameVariableName) && !String.IsNullOrEmpty(_FileNameVariableId))
            {
                jobFileNameVariable = new StateCommandVariable(_FileNameVariableName, _FileNameVariableId);
                FileNameVariableHasBeenSet = true;
            }
            else
            {
                FileNameVariableHasBeenSet = false;
            }
            _CommandWriteType = CommandType.Operate;
            CheckJobValid();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the IEC60870_5_104CommJob. </summary>
        ///
        /// <param name="station">  assigned to an object of type Station. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public IEC60870_5_104CommJob(Station station)
            : base(station)
        {
            _ASDUType = ASDUSelectableTypes.SinglePoint;
            _StartAddMon = 1;
            _StartAddCtrl = 1;
            _CmdQualifier = CommandQualifiers.NotUsedCQ;
            _ParamQualifier = ParamQualifiers.NotUsedPQ;
            _CmdAction = CommandActions.NotUsedCA;
            _CotVariableId = string.Empty;
            _CotVariableName = string.Empty;
            _QualityVariableId = string.Empty;
            _QualityVariableName = string.Empty;
            _UpdateTimeStamp = false;
            _WriteTimeStamp = false;
            CotVariableHasBeenSet = false;
            QualityVariableHasBeenSet = false;
            _FileNameVariableId = string.Empty;
            _FileNameVariableName = string.Empty;
            FileNameVariableHasBeenSet = false;
            _JobSelectAndCallQualifier = (byte)SelectAndCallQualifier.defaultValue;
            _JobAcknowledgeFileQualifier = (byte)AcknowledgeFileQualifier.notUsed;
            _FileSection = 0; // Default value
            _FileLength = 0; // Default value
            _CurrentFileLength = 0; // Default value
            _FileChecksum = 0;
            _SectionChecksum = 0;
            CheckJobValid();
        }

        /// <summary>   Initializes the IEC60870_5_104CommJob. </summary>
        protected IEC60870_5_104CommJob()
        {
            CheckJobValid();
        }
        #endregion

        #region data Member
        /// <summary>  The Conditional variable associated to the job. </summary>
        protected StateCommandVariable jobCotVariable;
        /// <summary>  Flag that signal if a Conditional Variable has been associated to the job. </summary>
        bool CotVariableHasBeenSet;
        /// <summary>  The Conditional variable associated to the job. </summary>
        protected StateCommandVariable jobQualityVariable;
        /// <summary>  Flag that signal if a Conditional Variable has been associated to the job. </summary>
        bool QualityVariableHasBeenSet;
        /// <summary>  The Conditional variable associated to the job. </summary>
        protected StateCommandVariable jobFileNameVariable;
        /// <summary>  Flag that signal if the Variable of the File Name has been associated to the job. </summary>
        public bool FileNameVariableHasBeenSet;

        public List<byte> SectionBytes;
        public List<byte> FileBytes;
        public List<byte> DirectoryBytes;
        #endregion

        #region Static methods
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Test if NodeId type is admitted. </summary>
        ///
        /// <param name="type"> . </param>
        ///
        /// <returns>   true if type admitted, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsTypeAdmitted(NodeId type)
        {
            if (type.IdType == IdType.Numeric)
            {
                uint nType = (uint)type.Identifier;
                if (nType == (uint)BuiltInType.Boolean ||
                nType == (uint)BuiltInType.Byte ||
                nType == (uint)BuiltInType.Double ||
                nType == (uint)BuiltInType.Float ||
                nType == (uint)BuiltInType.Int16 ||
                nType == (uint)BuiltInType.Int32 ||
                nType == (uint)BuiltInType.Int64 ||
                nType == (uint)BuiltInType.Integer ||
                nType == (uint)BuiltInType.SByte ||
                nType == (uint)BuiltInType.UInt16 ||
                nType == (uint)BuiltInType.UInt32 ||
                nType == (uint)BuiltInType.UInt64 ||
                nType == (uint)BuiltInType.UInteger ||
                nType == (uint)BuiltInType.String
                )
                    return true;
                return false;
            }
            
            return false;
        }
        #endregion

        #region Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the list of node ids to be observed for this job. </summary>
        ///
        /// <returns>   Gets the list of node ids to be observed for this job. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override List<NodeId> GetObservingNodes()
        {
            List<NodeId> nodeIDs = base.GetObservingNodes();
            if (FileNameVariableHasBeenSet == true)
            {
                nodeIDs.Add(jobFileNameVariable.varNodeId);
            }

            return nodeIDs;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Get the File Name. </summary>
        ///
        /// <param name="fileName" type="ref byte">   The value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool getFileNameVariable(ref string fileName)
        {
            if(fileName == null)
            {
                return (false);
            }
            if (FileNameVariableHasBeenSet == false)
            {
                return (false);
            }
            fileName = String.Empty;

            object value = jobFileNameVariable.varValue.Value;
            if (value == null)
            {
                return (false);
            }
            if (value is Array)
            {
                return (false);
            }
            string tmpValue = String.Empty;
            try
            {
                // The names of the files are integer numbers, so also variables of numeric type can be used
                // for the file name
                switch (Station.GetBuiltInType(value.GetType()))
                {
                    case BuiltInType.String:
                        tmpValue = (string)value;
                        break;

                    case BuiltInType.Boolean:
                        {

                            bool boolValue = (bool)value;
                            if (boolValue == true)
                            {
                                tmpValue = "1";
                            }
                            else
                            {
                                tmpValue = "0";
                            }
                        }
                        break;
                    case BuiltInType.SByte:
                        {
                            sbyte sbyteValue = (sbyte)value;
                            tmpValue = sbyteValue.ToString();
                        }
                        break;

                    case BuiltInType.Byte:
                        byte byteValue = (byte)value;
                        tmpValue = byteValue.ToString();
                        break;

                    case BuiltInType.Int16:
                        {
                            short shortValue = (short)value;
                            tmpValue = shortValue.ToString();
                        }
                        break;

                    case BuiltInType.UInt16:
                        {
                            ushort ushortValue = (ushort)value;
                            tmpValue = ushortValue.ToString();
                        }
                        break;

                    case BuiltInType.Int32:
                        {
                            int intValue = (int)value;
                            tmpValue = intValue.ToString();
                        }
                        break;

                    case BuiltInType.UInt32:
                        {
                            uint uintValue = (uint)value;
                            tmpValue = uintValue.ToString();
                        }
                        break;

                    case BuiltInType.Int64:
                        {
                            Int64 int64Value = (int)value;
                            tmpValue = int64Value.ToString();
                        }
                        break;

                    case BuiltInType.UInt64:
                        {
                            UInt64 uint64Value = (uint)value;
                            tmpValue = uint64Value.ToString();
                        }
                        break;
                    default:
                        return (false);
                }
            }
            catch
            {
                return (false);
            }
            fileName = tmpValue;
            return (true);
        }

        public bool getFileNameVariable(ref UInt16 fileName)
        {
            fileName = 0;
            if (FileNameVariableHasBeenSet == false)
            {
                return (false);
            }

            object value = jobFileNameVariable.varValue.Value;
            if (value == null)
            {
                return (false);
            }
            if (value is Array)
            {
                return (false);
            }
            UInt16 tmpValue = 0;
            try
            {
                // The names of the files are integer numbers, so also variables of numeric type can be used
                // for the file name
                switch (Station.GetBuiltInType(value.GetType()))
                {
                    case BuiltInType.String:
                        tmpValue = Convert.ToUInt16((string)value, System.Globalization.CultureInfo.InvariantCulture);
                        break;

                    case BuiltInType.Boolean:
                        {

                            bool boolValue = (bool)value;
                            if (boolValue == true)
                            {
                                tmpValue = 1;
                            }
                            else
                            {
                                tmpValue = 0;
                            }
                        }
                        break;
                    case BuiltInType.SByte:
                        {
                            sbyte sbyteValue = (sbyte)value;
                            tmpValue = (UInt16)sbyteValue;
                        }
                        break;

                    case BuiltInType.Byte:
                        byte byteValue = (byte)value;
                        tmpValue = (UInt16)byteValue;
                        break;

                    case BuiltInType.Int16:
                        {
                            short shortValue = (short)value;
                            tmpValue = (UInt16)shortValue;
                        }
                        break;

                    case BuiltInType.UInt16:
                        {
                            ushort ushortValue = (ushort)value;
                            tmpValue = ushortValue;
                        }
                        break;

                    case BuiltInType.Int32:
                        {
                            int intValue = (int)value;
                            tmpValue = (UInt16)intValue;
                        }
                        break;

                    case BuiltInType.UInt32:
                        {
                            uint uintValue = (uint)value;
                            tmpValue = (UInt16)uintValue;
                        }
                        break;

                    case BuiltInType.Int64:
                        {
                            Int64 int64Value = (int)value;
                            tmpValue = (UInt16)int64Value;
                        }
                        break;

                    case BuiltInType.UInt64:
                        {
                            UInt64 uint64Value = (uint)value;
                            tmpValue = (UInt16)uint64Value;
                        }
                        break;
                    default:
                        return (false);
                }
            }
            catch
            {
                return (false);
            }
            fileName = tmpValue;
            return (true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets the value of the conditional variable associated to the job. </summary>
        ///
        /// <returns>   void. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ManageUpdatedValueForTheFileNameVariable(DataValue value)
        {
            if (FileNameVariableHasBeenSet)
            {
                jobFileNameVariable.varValue = value;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Get a bit of the Quality variable of the station. </summary>
        ///
        /// <param name="byteValue" type="ref byte">   The value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool getQualityVariable(ref byte byteValue)
        {
            if (QualityVariableHasBeenSet == false)
            {
                return (false);
            }

            object value = jobQualityVariable.varValue.Value;
            if (value == null)
            {
                return (false);
            }
            if (value is Array)
            {
                return (false);
            }
            byte tmpValue = 0;
            try
            {
                switch (Station.GetBuiltInType(value.GetType()))
                {
                    case BuiltInType.Boolean:
                        {

                            bool boolValue = (bool)value;
                            if (boolValue == true)
                            {
                                tmpValue = 1;
                            }
                        }
                        break;
                    case BuiltInType.SByte:
                        {

                            sbyte sbyteValue = (sbyte)value;
                            tmpValue = (byte)sbyteValue;
                        }
                        break;

                    case BuiltInType.Byte:
                        tmpValue = (byte)value;
                        break;

                    case BuiltInType.Int16:
                        {
                            short shortValue = (short)value;
                            tmpValue = (byte)shortValue;
                        }
                        break;

                    case BuiltInType.UInt16:
                        {
                            ushort ushortValue = (ushort)value;
                            tmpValue = (byte)ushortValue;
                        }
                        break;

                    case BuiltInType.Int32:
                        {
                            int intValue = (int)value;
                            tmpValue = (byte)intValue;
                        }
                        break;

                    case BuiltInType.UInt32:
                        {
                            uint uintValue = (uint)value;
                            tmpValue = (byte)uintValue;
                        }
                        break;

                    case BuiltInType.Int64:
                        {
                            Int64 int64Value = (int)value;
                            tmpValue = (byte)int64Value;
                        }
                        break;

                    case BuiltInType.UInt64:
                        {
                            UInt64 uint64Value = (uint)value;
                            tmpValue = (byte)uint64Value;
                        }
                        break;
                    default:
                        return (false);
                }
            }
            catch
            {
                return (false);
            }
            byteValue = tmpValue;
            return (true);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Set a bit of the Quality variable of the station. </summary>
        ///
        /// <param name="quality" type="byte">   The quality new value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool setQualityVariable(byte quality)
        {
            byte byteCurrentValue = 0;
            if (getQualityVariable(ref byteCurrentValue) == false)
            {
                return (false);
            }
            if (byteCurrentValue == quality)
            {
                return (true);
            }

            jobQualityVariable.varValue = null;
            jobQualityVariable.varValue = new DataValue(new Variant(quality));

            Station.GetCommDriver().OnTagChanged(jobQualityVariable.varNodeId, jobQualityVariable.varValue);

            return (true);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Get a bit of the Cot variable of the station. </summary>
        ///
        /// <param name="Cot" type="ref byte">   The value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool getCotVariable(ref CausesOfTrasmission Cot)
        {
            if (CotVariableHasBeenSet == false)
            {
                return (false);
            }

            object value = jobCotVariable.varValue.Value;
            if (value == null)
            {
                return (false);
            }
            if (value is Array)
            {
                return (false);
            }
            byte tmpValue = 0;
            try
            {
                switch (Station.GetBuiltInType(value.GetType()))
                {
                    case BuiltInType.Boolean:
                        {

                            bool boolValue = (bool)value;
                            if (boolValue == true)
                            {
                                tmpValue = 1;
                            }
                        }
                        break;
                    case BuiltInType.SByte:
                        {

                            sbyte sbyteValue = (sbyte)value;
                            tmpValue = (byte)sbyteValue;
                        }
                        break;

                    case BuiltInType.Byte:
                        tmpValue = (byte)value;
                        break;

                    case BuiltInType.Int16:
                        {
                            short shortValue = (short)value;
                            tmpValue = (byte)shortValue;
                        }
                        break;

                    case BuiltInType.UInt16:
                        {
                            ushort ushortValue = (ushort)value;
                            tmpValue = (byte)ushortValue;
                        }
                        break;

                    case BuiltInType.Int32:
                        {
                            int intValue = (int)value;
                            tmpValue = (byte)intValue;
                        }
                        break;

                    case BuiltInType.UInt32:
                        {
                            uint uintValue = (uint)value;
                            tmpValue = (byte)uintValue;
                        }
                        break;

                    case BuiltInType.Int64:
                        {
                            Int64 int64Value = (int)value;
                            tmpValue = (byte)int64Value;
                        }
                        break;

                    case BuiltInType.UInt64:
                        {
                            UInt64 uint64Value = (uint)value;
                            tmpValue = (byte)uint64Value;
                        }
                        break;
                    default:
                        return (false);
                }
            }
            catch
            {
                return (false);
            }
            Cot = (CausesOfTrasmission)tmpValue;
            return (true);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Set a bit of the Cot variable of the station. </summary>
        ///
        /// <param name="Cot" type="byte">   The Cot new value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool setCotVariable(CausesOfTrasmission Cot)
        {
            CausesOfTrasmission CurrentValue = 0;
            if (getCotVariable(ref CurrentValue) == false)
            {
                return (false);
            }
            if (CurrentValue == Cot)
            {
                return (true);
            }

            jobCotVariable.varValue = null;
            jobCotVariable.varValue = new DataValue(new Variant((byte)Cot));

            Station.GetCommDriver().OnTagChanged(jobCotVariable.varNodeId, jobCotVariable.varValue);

            return (true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Save a file to disk. </summary>
        ///
        /// <returns>   Error code. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public IEC60870_5_104ErrorCodes SaveFile()
        {
            IEC60870_5_104ErrorCodes errorCode = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorNoError;
            try
            {
                IEC60870_5_104Station iecStation = (IEC60870_5_104Station)Station;

                // Get the file path
                string filePath = iecStation.FileTransferDirectory;
                if (String.IsNullOrWhiteSpace(filePath))
                {
                    filePath = String.Empty;

                    // The default path is the folder of the project resources
                    IEC60870_5_104Driver commDriver = (IEC60870_5_104Driver)iecStation.GetCommDriver();
                    string connString = XpoHelpers.XpoHelper.GetDataSourceFilePath(CommunicationDriver.GetConnectionString(commDriver.StrConnectionString, null, commDriver.DriverName, null));
                    if (!String.IsNullOrWhiteSpace(connString))
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("SaveFile 1 - Connection String: {0}", connString));
                        filePath = Path.GetDirectoryName(connString);
                        if (!String.IsNullOrWhiteSpace(filePath))
                        {
                            System.Diagnostics.Debug.WriteLine(String.Format("SaveFile 2 - File Path: {0}", filePath));
                        }
                        else
                        {
                            filePath = String.Empty;
                        }
                    }
                }
                int pathLength = filePath.Length;
                if (pathLength > 0)
                {
                    // If the directory where the file must be saved does not exists, create it
                    if(!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    // Eventually, add the last '\' character to the path
                    if (filePath[pathLength - 1] != Path.DirectorySeparatorChar)
                    {
                        filePath += Path.DirectorySeparatorChar;
                    }
                }
                string fileName = String.Empty;
                if (!getFileNameVariable(ref fileName))
                {
                    errorCode = IEC60870_5_104ErrorCodes.ErrorFileTransferSave;
                    return (errorCode);
                }

                filePath += fileName;
                FileStream fileStream = File.Create(filePath);
                fileStream.Write(FileBytes.ToArray(), 0, FileBytes.Count);
            }
            catch(Exception ex)
            {
                errorCode = IEC60870_5_104ErrorCodes.ErrorFileTransferSave;
            }

            return (errorCode);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   return tag object memory size. </summary>
        ///
        /// <param name="t">    . </param>
        ///
        /// <returns>   The tag size. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        uint GetTagSize(DriverBaseInterfaces.TagDefinition t)
        {
            if (t.DataType.IdType == IdType.Numeric)
            {
                switch((uint)t.DataType.Identifier)
                {
                    case (uint)BuiltInType.Boolean:
                    case (uint)BuiltInType.SByte:
                    case (uint)BuiltInType.Byte:
                        return 1;
                    case (uint)BuiltInType.Int16:
                    case (uint)BuiltInType.UInt16:
                        return 2;
                    case (uint)BuiltInType.Float:
                    case (uint)BuiltInType.UInt32:
                    case (uint)BuiltInType.Int32:
                        return 4;
                    case (uint)BuiltInType.UInt64:
                    case (uint)BuiltInType.Int64:
                    case (uint)BuiltInType.Double:
                        return 8;
                    default:
                        return 0;
                }            
            }
            return 0;
        }

        /// <summary>   Test if IEC60870_5_104CommJob object is valid. </summary>
        private void CheckJobValid()
        {
                
            string tagnamelist = string.Empty;

            List<DriverBaseInterfaces.TagDefinition> tempList = new List<DriverBaseInterfaces.TagDefinition>();
            tagnamelist = TagsList[0].TagNode.NodeId.ToString();
            tempList.AddRange(GetSimpleTagList(TagsList[0].TagNode));

            foreach (var t in tempList)
            {
                if (!IsTypeAdmitted(t.DataType))
                {
                    IsValid = false;
                    InvalidReason = string.Format("The Tag type is invalid for its Data Type. (Tag: {0} Data Type: {1})", t.NodeId.ToString(), t.DataType.Identifier.ToString());
                    return;
                }

                if (IEC60870_5_104Protocol.InvalidAreaTypeLinkType(_ASDUType, Type))
                {
                    IsValid = false;
                    InvalidReason = string.Format(Properties.Resources.AreaTypeRequireInput, DriverCodeBase.Properties.Resources.LinkType_Input);
                    return;
                }

            }


            string errorDesc;
            if (!ProtocolDataSizeIsValid(out errorDesc))
            {
                IsValid = false;
                InvalidReason = string.Format("The {0} is invalid for the ASDUType. (Tags: {1} F.Code: {2})", errorDesc, tagnamelist, ASDUType);
                return;
            }

            IsValid = true;
            InvalidReason = string.Empty;
        }

        private bool IsSelectBeforeOperateJob()
        {
            return (base.Type != LinkType.Input && 
                    (_ASDUType == ASDUSelectableTypes.SinglePoint
                    || _ASDUType == ASDUSelectableTypes.DoublePoint
                    //|| _ASDUType == ASDUSelectableTypes.StepPosition
                    || _ASDUType == ASDUSelectableTypes.BitString
                    || _ASDUType == ASDUSelectableTypes.NormalizedMeasurand
                    || _ASDUType == ASDUSelectableTypes.ScaledMeasurand
                    || _ASDUType == ASDUSelectableTypes.FloatingPointMeasurand
                    //|| _ASDUType == ASDUSelectableTypes.IntegratedTotals
                    || _ASDUType == ASDUSelectableTypes.ParamNormalizedMeasurand
                    || _ASDUType == ASDUSelectableTypes.ParamScaledMeasurand
                    || _ASDUType == ASDUSelectableTypes.ParamFloatingPointMeasurand
                    || _ASDUType == ASDUSelectableTypes.RegulatingStep)
                    );
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Return maximum memory size for aggregation of IEC60870_5_104CommJob objects.
        /// </summary>
        ///
        /// <returns>   The aggregate maximum job size. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint GetAggregateMaxJobSize()
        {
            uint AggregLimit = Station.GetCommDriver().AggregationLimit;
            uint JobMaxSize = GetMaxJobSize();
            if ((AggregLimit != 0) && (AggregLimit < JobMaxSize))
            {
                return AggregLimit;
            }
            else
            {
                return JobMaxSize;
            }
        }

        internal bool CheckSendSelect(IEC60870_5_104Channel.ReadWriteJobStates readWriteJobState)
        {
            return (_CommandWriteType == CommandType.Select || (_CommandWriteType == CommandType.SelectExecuteValue && readWriteJobState == IEC60870_5_104Channel.ReadWriteJobStates.WRITE_SEND) || _CommandWriteType == CommandType.DeactivateSelect);
        }


        public override uint getProtocolDataType()
        {
            return (IEC60870_5_104Protocol.BuiltInDataType(ASDUType));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return maximum memory size of IEC60870_5_104CommJob objects. </summary>
        ///
        /// <returns>   The maximum job size. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override uint GetMaxJobSize()
        {
            return IEC60870_5_104Protocol.GetMaxJobSize();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Test if it can be aggregated candJob. </summary>
        ///
        /// <param name="candJob">      . </param>
        /// <param name="ExtraBytes">   [out] additional byte size for the aggregation. </param>
        ///
        /// <returns>   A JobAggregationType. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            ExtraBytes = 0;
            return JobAggregationType.JobAggregImpossible;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Aggregate candJob as specified by AggType and manage ExtraBytes. </summary>
        ///
        /// <param name="candJob">      . </param>
        /// <param name="AggType">      . </param>
        /// <param name="ExtraBytes">   . </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            return false;
        }

        public override void GetJobData(ref object jobData)
        {
            List<byte> outData = new List<byte>();
            //prepare a write request
            Tag cand = null;
            byte[] jobdata;
            UInt16 nData = 0;
            lock (lockListObject)
            {
                cand = TagsListToWrite[0];
                TagsListToWrite.Remove(cand);
                if (!TagsListOnWriting.Contains(cand))
                    TagsListOnWriting.Add(cand);
            }
            if ((uint)cand.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
                nData = (UInt16)((cand.Size + 7) / 8);
            else if (ElementNumber > 0 && !ProtocolDataSizeBig())
            {
                if (cand.TagNode.ArrayDimension == 0)
                    nData = (ushort)(GetProtocolDataByteSize());
                else
                    nData = (ushort)(GetProtocolDataByteSize() * cand.TagNode.ArrayDimension);
            }
            else
                nData = (UInt16)cand.Size;
            lock (lockListObject)
            {
                cand.LastValue = cand.Value.Value;
                jobdata = new byte[nData];
                cand.GetTagBuffer(ref jobdata, false, 0, (ElementNumber > 0 && !ProtocolDataSizeBig() ? GetProtocolDataByteSize() : 0));
            }
            uint ArraySize = cand.TagNode.ArrayDimension;
            if (ArraySize == 0)
                ArraySize = 1;
            if (ProtocolDataSizeBig())
            {
                List<byte> correctData = new List<byte>();
                UInt16 sizeDataType = (UInt16)GetDataTypeByteSize((uint)cand.TagNode.DataType.Identifier);
                UInt16 sizeProtocolData = (UInt16)GetProtocolDataByteSize();
                for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                {
                    byte[] tmpdata = new byte[sizeDataType];
                    if ((uint)cand.TagNode.DataType.Identifier != (uint)BuiltInType.Boolean)
                        Array.Copy(jobdata, ArrayIndex * sizeDataType, tmpdata, 0, sizeDataType);
                    else
                    {
                        if ((1 << (ArrayIndex % 8) & jobdata[ArrayIndex / 8]) == 0)
                            tmpdata[0] = 0;
                        else
                            tmpdata[0] = 1;
                    }
                    cand.getWriteValueFromMemRW(ref tmpdata, sizeDataType, sizeProtocolData, ElementNumber, ArrayIndex);
                    correctData.AddRange(tmpdata);
                }
                jobdata = correctData.ToArray();
            }
            else if (isProtocolBool())
            {
                if (ElementNumber == 0 || (uint)cand.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
                {
                    byte[] tmpData = new byte[ArraySize * GetDataTypeBitSize((uint)cand.TagNode.DataType.Identifier)];
                    for (int ArrayIndex = 0; ArrayIndex < tmpData.Length; ArrayIndex++)
                    {
                        if ((jobdata[ArrayIndex / 8] & (1 << (ArrayIndex % 8))) != 0)
                            tmpData[ArrayIndex] = 1;
                    }
                    jobdata = tmpData;
                }
            }
            if (!isProtocolBool())
            {
                if (SwapBytes)
                {
                    SwapByteBuffer(ref jobdata);
                }

                if (SwapWords)
                {
                    SwapWordBuffer(ref jobdata);
                }
            }
            outData.AddRange(jobdata);

            if (isProtocolBool())
            {
                byte[] tmpData = new byte[(outData.Count() + 7) / 8];
                for (int ArrayIndex = 0; ArrayIndex < outData.Count(); ArrayIndex++)
                {
                    if (outData[ArrayIndex] != 0)
                        tmpData[ArrayIndex / 8] |= (byte)(1 << (ArrayIndex % 8));
                }
                jobData = tmpData;
            }
            else
                jobData = outData.ToArray();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   write the IEC60870_5_104CommJob's tags with JobData. </summary>
        ///
        /// <param name="jobData">  data buffer to write. </param>
        /// <param name="changed">  [in,out] out list of changed tags. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void SetJobData(object jobData, ref List<Tag> changed)
        {
            byte[] rec = jobData as byte[];
            if (rec == null)
                return;

            base.SetJobData(rec, ref changed);
            lock (lockListObject)
            {
                UInt16 sizeProtocolData = (UInt16)GetProtocolDataByteSize();
                if (TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                {
                    if (isProtocolBool())
                    {
                        if (TagsList[0].SetTagValue(ref rec, (int)TagsList[0].ByteOffset, UpdateTimeStamp ))
                            changed.Add(TagsList[0]);
                    }
                    else
                    {
                        uint ArraySize = TagsList[0].TagNode.ArrayDimension;
                        if (ArraySize == 0)
                            ArraySize = 1;
                        byte[] tmpData = new byte[ArraySize];
                        TagsList[0].setMemRW(rec, (int)TagsList[0].ByteOffset, (int)(sizeProtocolData * ArraySize));
                        for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                        {
                            bool valBool = TagsList[0].getBoolValueFromMemRW((int)TagsList[0].ByteOffset + sizeProtocolData * ArrayIndex, ElementNumber);
                            tmpData[ArrayIndex] = (byte)(valBool ? 1 : 0);
                        }

                        if (TagsList[0].SetTagValue(ref tmpData, 0, UpdateTimeStamp))
                            changed.Add(TagsList[0]);
                    }
                }
                else
                {
                    if (isProtocolBool())
                    {
                        if (TagsList[0].SetTagValue(ref rec, (int)TagsList[0].ByteOffset, UpdateTimeStamp, (uint)(ElementNumber > 0 ? 1 : 0)))
                            changed.Add(TagsList[0]);
                    }
                    else
                    {
                        if (!ProtocolDataSizeBig())
                        {
                            uint elemsize = 0;
                            if (ElementNumber > 0)
                            {
                                elemsize = GetProtocolDataByteSize();
                            }
                            if (TagsList[0].SetTagValue(ref rec, (int)TagsList[0].ByteOffset, UpdateTimeStamp, elemsize))
                                changed.Add(TagsList[0]);
                        }
                        else
                        {
                            UInt16 sizeTmpData = (UInt16)GetDataTypeByteSize((uint)TagsList[0].TagNode.DataType.Identifier);
                            uint ArraySize = TagsList[0].TagNode.ArrayDimension;
                            if (ArraySize == 0)
                                ArraySize = 1;
                            byte[] tmpData = new byte[sizeTmpData * ArraySize];
                            int indexTmpData = 0;
                            TagsList[0].setMemRW(rec, (int)TagsList[0].ByteOffset, (int)(sizeProtocolData * ArraySize));
                            for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                            {
                                Array.Copy(rec, TagsList[0].ByteOffset + sizeProtocolData * ArrayIndex + ElementNumber * sizeTmpData, tmpData, indexTmpData, sizeTmpData);
                                indexTmpData += sizeTmpData;
                            }

                            if (TagsList[0].SetTagValue(ref tmpData, 0, UpdateTimeStamp))
                                changed.Add(TagsList[0]);
                        }
                    }
                }
                FirstTime = false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Compare tags by offset. Return 0 if x = y , 1 if x &gt; y and -1 if x &lt; y.
        /// </summary>
        ///
        /// <param name="x">    . </param>
        /// <param name="y">    . </param>
        ///
        /// <returns>   An int. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static int CompareTagByOffset(Tag x, Tag y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0; //==
                else
                    return -1;// x < y
            }
            else
            {
                //x!= null
                if (y == null)
                    return 1; //x > y
                else
                {
                    if (x.ByteOffset > y.ByteOffset)
                        return 1;
                    else if (x.ByteOffset == y.ByteOffset)
                        return 0;
                    else
                        return -1;

                }
            }
        }

        /// <summary>
        /// Tag management associated to a conditional variable 
        /// </summary>
        /// <param name="value">Conditional var value</param>
        public override void ManageUpdatedValueForTheConditionalVariable(DataValue value)
        {
            base.ManageUpdatedValueForTheConditionalVariable(value);
            
            if (IsSelectBeforeOperateJob())
            {
                bool bitValue = false;
                byte bitCount = 0;
                CommandType newCommandWriteType = CommandType.Select;
                if (jobConditionalVariable.GetStateCommandVariableBit(ref bitValue, (UInt16)CommandType.Select))
                {
                    if (bitValue)
                    {
                        newCommandWriteType = CommandType.Select;
                        bitCount++;
                    }
                }
                if (jobConditionalVariable.GetStateCommandVariableBit(ref bitValue, (UInt16)CommandType.Operate))
                {
                    if (bitValue)
                    {
                        newCommandWriteType = CommandType.Operate;
                        bitCount++;
                    }
                }
                if (jobConditionalVariable.GetStateCommandVariableBit(ref bitValue, (UInt16)CommandType.SelectExecuteValue))
                {
                    if (bitValue)
                    {
                        newCommandWriteType = CommandType.SelectExecuteValue;
                        bitCount++;
                    }
                }
                if (jobConditionalVariable.GetStateCommandVariableBit(ref bitValue, (UInt16)CommandType.DeactivateSelect))
                {
                    if (bitValue)
                    {
                        newCommandWriteType = CommandType.DeactivateSelect;
                        bitCount++;
                    }
                }
                if (jobConditionalVariable.GetStateCommandVariableBit(ref bitValue, (UInt16)CommandType.DeactivateOperate))
                {
                    if (bitValue)
                    {
                        newCommandWriteType = CommandType.DeactivateOperate;
                        bitCount++;
                    }
                }

                if (bitCount == 1)
                {
                    _CommandWriteType = newCommandWriteType;
                    lock (lockListObject)
                    {
                        if (TagsListToWrite.Count == 0)
                            TagsListToWrite.Add(TagsList[0]);
                    }
                } else if (bitCount > 1)
                {
                    Station.GetCommDriver().OnSystemEvent(null, String.Format(Properties.Resources.ErrorTooManyBitActivatedConditionalVariable, base.ConditionalVariableName), EventSeverity.Min);
                    lock (lockListObject)
                    {
                        TagsListToWrite.Clear();
                    }
                    ResetConditionalVariable();
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>   The Area Type. </summary>
        private ASDUSelectableTypes _ASDUType;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Memory area Property. </summary>
        ///
        /// <value> The Area Type. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ASDUSelectableTypes ASDUType
        {
            get { return _ASDUType; }
            set
            {
                _ASDUType = value;
            }
        }

        /// <summary>   The start address. </summary>
        private UInt32 _StartAddMon;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Start address of memory area Property. </summary>
        ///
        /// <value> The start address. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public UInt32 StartAddMon
        {
            get { return _StartAddMon; }
            set
            {
                _StartAddMon = value;
            }
        }

        /// <summary>   Control IOA. </summary>
        private UInt32 _StartAddCtrl;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Information Object Address for Control Direction (leave 0 if only monitoring). </summary>
        ///
        /// <value> Control IOA. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public UInt32 StartAddCtrl
        {
            get
            {
                return _StartAddCtrl;
            }
            set
            {
                _StartAddCtrl = value;
            }
        }

        /// <summary>   Command Qualifier. </summary>
        private CommandQualifiers _CmdQualifier;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Command Qualifier. </summary>
        ///
        /// <value> Command Qualifier. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public CommandQualifiers CmdQualifier
        {
            get
            {
                return _CmdQualifier;
            }
            set
            {
                _CmdQualifier = value;
            }
        }

        /// <summary>   Parameter Qualifier. </summary>
        private ParamQualifiers _ParamQualifier;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Parameter Qualifier. </summary>
        ///
        /// <value> Parameter Qualifier. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ParamQualifiers ParamQualifier
        {
            get
            {
                return _ParamQualifier;
            }
            set
            {
                _ParamQualifier = value;
            }
        }

        /// <summary>   Command Action. </summary>
        private CommandActions _CmdAction;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Command Action. </summary>
        ///
        /// <value> Command Action. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public CommandActions CmdAction
        {
            get
            {
                return _CmdAction;
            }
            set
            {
                _CmdAction = value;
            }
        }

        /// <summary>   COT Variable name. </summary>
        private string _CotVariableName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Optional parameter that specifies the name of the variable where the driver will store 
        ///             the ""Cause Of Transmission"" of the received data. </summary>
        ///
        /// <value> COT Variable Name. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string CotVariableName
        {
            get
            {
                return _CotVariableName;
            }
            set
            {
                _CotVariableName = value;
            }
        }

        /// <summary>   COT Variable Id. </summary>
        private string _CotVariableId;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Optional parameter that specifies the Id of the variable where the driver will store 
        ///             the ""Cause Of Transmission"" of the received data. </summary>
        ///
        /// <value> COT Variable Id. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string CotVariableId
        {
            get
            {
                return _CotVariableId;
            }
            set
            {
                _CotVariableId = value;
            }
        }

        /// <summary>   Quality Variable Name. </summary>
        private string _QualityVariableName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Optional parameter that specifies the name of the variable where the driver will store 
        ///             the ""Quality Descriptor"" of the received data. </summary>
        ///
        /// <value> Quality Variable Name. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string QualityVariableName
        {
            get
            {
                return _QualityVariableName;
            }
            set
            {
                _QualityVariableName = value;
            }
        }

        /// <summary>   Quality Variable Id. </summary>
        private string _QualityVariableId;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Optional parameter that specifies the Id of the variable where the driver will store 
        ///             the ""Quality Descriptor"" of the received data. </summary>
        ///
        /// <value> Quality Variable Id. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string QualityVariableId
        {
            get
            {
                return _QualityVariableId;
            }
            set
            {
                _QualityVariableId = value;
            }
        }

        /// <summary> Always update TimeStamp. </summary>
        private bool _UpdateTimeStamp;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Select if the driver have to update timestamp at every read. </summary>
        ///
        /// <value> Always update TimeStamp. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool UpdateTimeStamp
        {
            get
            {
                return _UpdateTimeStamp;
            }
            set
            {
                _UpdateTimeStamp = value;
            }
        }

        /// <summary> Write TimeStamp. </summary>
        private bool _WriteTimeStamp;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Add local TimeStamp to writes on the device. </summary>
        ///
        /// <value> Write TimeStamp. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool WriteTimeStamp
        {
            get
            {
                return _WriteTimeStamp;
            }
            set
            {
                _WriteTimeStamp = value;
            }
        }

        /// <summary>   File Name Variable Name. </summary>
        private string _FileNameVariableName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Optional parameter that specifies the name of the variable that contains 
        ///             the name of the file to be uploaded. </summary>
        ///
        /// <value> File Name Variable Name. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string FileNameVariableName
        {
            get
            {
                return _FileNameVariableName;
            }
            set
            {
                _FileNameVariableName = value;
            }
        }

        /// <summary>   File Name Variable Id. </summary>
        private string _FileNameVariableId;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Optional parameter that specifies the Id of the variable that contains 
        ///             the name of the file to be uploaded. </summary>
        ///
        /// <value> File Name Variable Id. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string FileNameVariableId
        {
            get
            {
                return _FileNameVariableId;
            }
            set
            {
                _FileNameVariableId = value;
            }
        }

        /// <summary>   Write offset Data. </summary>
        private ushort _offset;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   offset for write data. </summary>
        ///
        /// <value> The write data offset. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ushort writeOffset
        {
            get { return _offset; }
            set
            {
                _offset = value;
            }
        }

        /// <summary>   Write offset Data. </summary>
        private bool _onWrite = false;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   offset for write data. </summary>
        ///
        /// <value> The write data offset. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool onWrite
        {
            get { return _onWrite; }
            set
            {
                _onWrite = value;
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   String for grouping the IEC60870_5_104CommJobs Property. </summary>
        ///
        /// <value> The group string. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string GroupString
        {
            get
            {
                if (Station == null)
                    return string.Empty;
                string ret = base.GroupString;
                return string.Format("{0}AT{1:00}", ret, (uint)ASDUType);
            }
        }

        private byte _JobSelectAndCallQualifier;
        public byte JobSelectAndCallQualifier
        {
            get { return _JobSelectAndCallQualifier; }
            set
            {
                _JobSelectAndCallQualifier = value;
            }
        }

        private byte _JobAcknowledgeFileQualifier;
        public byte JobAcknowledgeFileQualifier
        {
            get { return _JobAcknowledgeFileQualifier; }
            set
            {
                _JobAcknowledgeFileQualifier = value;
            }
        }

        private byte _FileSection;
        public byte FileSection
        {
            get { return _FileSection; }
            set
            {
                _FileSection = value;
            }
        }

        private uint _FileLength;
        public uint FileLength
        {
            get { return _FileLength; }
            set
            {
                _FileLength = value;
            }
        }

        private uint _CurrentFileLength;
        public uint CurrentFileLength
        {
            get { return _CurrentFileLength; }
            set
            {
                _CurrentFileLength = value;
            }
        }

        private uint _SectionLength;
        public uint SectionLength
        {
            get { return _SectionLength; }
            set
            {
                _SectionLength = value;
            }
        }

        private byte _FileChecksum;
        public byte FileChecksum
        {
            get { return _FileChecksum; }
            set
            {
                _FileChecksum = value;
            }
        }

        private byte _SectionChecksum;
        public byte SectionChecksum
        {
            get { return _SectionChecksum; }
            set
            {
                _SectionChecksum = value;
            }
        }

        public void SetJobNotInErrorState()
        {
            InErrorState = false;
        }

        private CommandType _CommandWriteType;
        public CommandType CommandWriteType
        {
            get { return _CommandWriteType; }
            set { _CommandWriteType = value; }
        }
        #endregion
    }
}
