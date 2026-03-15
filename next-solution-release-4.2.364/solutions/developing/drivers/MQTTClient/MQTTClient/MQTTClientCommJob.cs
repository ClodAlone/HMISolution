using DriverCodeBase;
using DriverCodeBase.Enumerators;
//using MQTTClientMessaging.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Utilities;

namespace MQTTClient
{
    public enum MQTTClientCommJobStatus : byte
    {
        Idle,
        PublishRequestPending,
        PublishReplyReceived,
        PublishError
    }

    public enum MQTTClientJsonAddTimestamp : sbyte
    {
        AddTimestamp_Undefined = -1,
        AddTimestamp_False = 0,
        AddTimestamp_True = 1
    }

    public class MQTTClientCommJob : CommJob
    {
        #region Constructors
        public MQTTClientCommJob(Station station, MQTTClientCommJobSettings settings)
            : base(station, settings)
        {
            _TagName = settings.TagName;
            _Retained = settings.Retained;
            _QualityOfServiceLevel = settings.QualityOfServiceLevel;
            _JsonMessageFormat = settings.JsonMessageFormat;
            _JsonMessageTimestampField = settings.JsonMessageTimestampField;
            _JsonTimestampFormat = settings.JsonTimestampFormat;
            _JsonUseLocalTime = settings.JsonUseLocalTime;
            _HysteresisThreshold = settings.HysteresisThreshold;
            jsonTimeStampMustBeAdded = MQTTClientJsonAddTimestamp.AddTimestamp_Undefined;
            CheckJobValid();
        }

        public MQTTClientCommJob(Station station, MQTTClientTag defTag)
            : base(station, defTag)
        {
            _TagName = defTag.MQTTClientDynSettings.TagName;
            _Retained = defTag.MQTTClientDynSettings.Retained;
            _QualityOfServiceLevel = defTag.MQTTClientDynSettings.QualityOfServiceLevel;
            _JsonMessageFormat = defTag.MQTTClientDynSettings.JsonMessageFormat;
            _JsonMessageTimestampField = defTag.MQTTClientDynSettings.JsonMessageTimestampField;
            _JsonTimestampFormat = defTag.MQTTClientDynSettings.JsonTimestampFormat;
            _JsonUseLocalTime = defTag.MQTTClientDynSettings.JsonUseLocalTime;
            _HysteresisThreshold = defTag.MQTTClientDynSettings.HysteresisThreshold;
            _Status = MQTTClientCommJobStatus.Idle;
            jsonTimeStampMustBeAdded = MQTTClientJsonAddTimestamp.AddTimestamp_False;
            CheckJobValid();
            if (IsValid)
            {
                // If needed, set the field of the JSon message corresponding to each tag of the job
                MQTTClientStation mqttStation = (MQTTClientStation)station;
                if(mqttStation.MQTTClientMessageFormat == MQTTClientMessageFormats.JSON)
                {
                    // Special case: unsplitted structure
                    if (defTag.TagNode.DataType.IdType == IdType.Guid)
                    {
                        jsonTimeStampMustBeAdded = MQTTClientJsonAddTimestamp.AddTimestamp_Undefined;
                        bool jsonTimestampFound = false;
                        // Each tag of the job corresponds to an element of the structure and to a field of the JSon message
                        foreach (MQTTClientTag tag in TagsList)
                        {
                            string fieldName = MQTTClientDynTagSettings.GetNodeTree(tag.TagNode.NodeId, tag.TagNode.Name);
                            if(!String.IsNullOrWhiteSpace(fieldName))
                            {
                                if(!String.IsNullOrWhiteSpace(_JsonMessageFormat))
                                {
                                    tag.JsonMessageFormat = _JsonMessageFormat + ("." + fieldName.Replace('/', '.'));
                                }
                                else
                                {
                                    tag.JsonMessageFormat = fieldName.Replace('/', '.');
                                }
                                tag.JsonMessageTimestampField = _JsonMessageTimestampField;
                                tag.JsonTimestampFormat = _JsonTimestampFormat;
                                if(!String.IsNullOrWhiteSpace(_JsonMessageTimestampField) && (_JsonMessageTimestampField == tag.JsonMessageFormat))
                                {
                                    jsonTimestampFound = true;
                                }
                            }
                        }

                        // Set if the timestamp must be explicitely added to a published JSon message
                        if (!String.IsNullOrWhiteSpace(_JsonMessageTimestampField) && (jsonTimestampFound == false))
                        {
                            jsonTimeStampMustBeAdded = MQTTClientJsonAddTimestamp.AddTimestamp_True;
                        }
                    }
                    else
                    {
                        foreach (MQTTClientTag tag in TagsList)
                        {
                            tag.JsonMessageFormat = _JsonMessageFormat;
                            tag.JsonMessageTimestampField = _JsonMessageTimestampField;
                            tag.JsonTimestampFormat = _JsonTimestampFormat;
                        }

                        // Set if the timestamp must be explicitely added to a published JSon message
                        if (String.IsNullOrWhiteSpace(_JsonMessageFormat) || String.IsNullOrWhiteSpace(_JsonMessageTimestampField) || (_JsonMessageFormat == _JsonMessageTimestampField))
                        {
                            jsonTimeStampMustBeAdded = MQTTClientJsonAddTimestamp.AddTimestamp_False;
                        }
                        else
                        {
                            jsonTimeStampMustBeAdded = MQTTClientJsonAddTimestamp.AddTimestamp_True;
                        }
                    }
                }
                else
                {
                    jsonTimeStampMustBeAdded = MQTTClientJsonAddTimestamp.AddTimestamp_False;
                }
            }
        }

        public MQTTClientCommJob(Station station)
            : base(station)
        {
            _TagName = String.Empty;
            _Retained = true;
            _QualityOfServiceLevel = QualityOfServiceLevels.AtMostOnce_0;
            _JsonMessageFormat = String.Empty;
            _JsonMessageTimestampField = String.Empty;
            _JsonTimestampFormat = JSonTimestampFormats.tf_ISO;
            _JsonUseLocalTime = false;
            _HysteresisThreshold = 0.0;
            _Status = MQTTClientCommJobStatus.Idle;
            jsonTimeStampMustBeAdded = MQTTClientJsonAddTimestamp.AddTimestamp_Undefined;

            CheckJobValid();
        }

        protected MQTTClientCommJob()
        {
            _TagName = String.Empty;
            _Retained = true;
            _QualityOfServiceLevel = QualityOfServiceLevels.AtMostOnce_0;
            _JsonMessageFormat = String.Empty;
            _JsonMessageTimestampField = String.Empty;
            _JsonTimestampFormat = JSonTimestampFormats.tf_ISO;
            _JsonUseLocalTime = false;
            _HysteresisThreshold = 0.0;
            _Status = MQTTClientCommJobStatus.Idle;
            jsonTimeStampMustBeAdded = MQTTClientJsonAddTimestamp.AddTimestamp_Undefined;

            CheckJobValid();
        }
        #endregion

        #region data Member
        private MQTTClientJsonAddTimestamp jsonTimeStampMustBeAdded = MQTTClientJsonAddTimestamp.AddTimestamp_Undefined;
        #endregion

        #region Static methods
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

        #region override Methods

        #endregion

        #region Methods

        public void SetError(DriverErrorCodes errorCode)
        {
            uint quality = StatusCodes.BadCommunicationError;
            string error = String.Empty;
            Station.GetCommDriver().GetDriverErrorInfo((int)errorCode, out quality, out error);
            if((errorCode == (DriverErrorCodes)MQTTClientErrorCodes.ErrorCodeErrorSubscriptionFailed) ||
               (errorCode == (DriverErrorCodes)MQTTClientErrorCodes.ErrorCodeErrorSubscriptionTimeout) ||
               (errorCode == (DriverErrorCodes)MQTTClientErrorCodes.ErrorCodeErrorPublishTimeout))
            {
                string errorFormat = error;
                error = String.Format(errorFormat, TagName, Station.GetChannel().Name);
            }
            SetErrorState(quality, error);

            if (errorCode != DriverErrorCodes.ErrorNoError)
            {
#if DEBUG
                System.Diagnostics.Trace.TraceInformation("Station {0} - Channel {1} - Error {2}", Station.Name, Station.GetChannel().Name, error);
#endif
                if (Station.LastErrorCode == DriverErrorCodes.ErrorNoError)
                {
                    Station.LastErrorTime = DateTime.UtcNow;
                    Station.LastErrorCode = errorCode;
                }
                lock (((MQTTClientStation)Station).getLockBool())
                {
                    if (InUse)
                        Station.InErrorState = true;
                    if (((MQTTClientStation)Station).GetChannelBase() != null)
                        ((MQTTClientStation)Station).GetChannelBase().ChangeStateJob(this, CommJobState.PollingInError);

                }

            }
            else if (Station.InErrorState)
            {
                var listJob = Station.GetChannel().GetJobStateList(CommJobState.PollingInError);
                bool resetError = true;
                foreach (var commjob in listJob)
                {
                    if (commjob.InErrorState && commjob.InUse)
                        resetError = false;
                }
                if (resetError)
                {
#if DEBUG
                    System.Diagnostics.Trace.TraceInformation("Station {0} ResetErrors - Channel {1} ResetErrors", Station.Name, Station.GetChannel().Name);
#endif
                    Station.InErrorState = false;
                    Station.LastErrorTime = DateTime.MinValue;
                    Station.LastErrorCode = DriverErrorCodes.ErrorNoError;
                }
            }

            //lock (((MQTTClientStation)Station).getLockBool())
            //{
                if (!InErrorState && Station.FirstTime)
                {
                    Station.FirstTime = false;
                    Station.GetCommDriver().OnSystemEvent(null, String.Format(DriverCodeBase.Properties.Resources.StationResumeError, Station.GetCommDriver().DriverName/*DriverInfo.GetDriverName()*/, Station.Name), EventSeverity.Low);
                }
            //}
        }

        public bool MustPublish()
        {
            bool returnValue = false;
            if ((Type != LinkType.Input) &&
                ((TagsListToWrite.Count > 0) || (Type == LinkType.UnconditionalOutput)))
            {
                returnValue = true;
            }

            return (returnValue);
        }

        public bool MustSubscribe()
        {
            bool returnValue = false;

            if ((Type == LinkType.Input) || (Type == LinkType.InputOutput))
            {
                MQTTClientChannel mqttChannel = (MQTTClientChannel)Station.GetChannel();
                if(mqttChannel.GetSubscriptionState(this) == (byte)MQTTClientSubscriptionStates.Todo)
                {
                    returnValue = true;
                }
            }

            return (returnValue);
        }

        private T GetTagValue<T>(MQTTClientTag mqttTag, T defaultValue)
        {
            T returnedValue = defaultValue;
            try
            {
                returnedValue = (T)mqttTag.Value.Value;
            }
            catch (Exception e)
            {
                returnedValue = defaultValue;
            }
            return (returnedValue);
        }

        private void AddTagValueToJSonMessage(JsonWriter writer, MQTTClientTag mqttTag)
        {
           if (mqttTag.TagNode.ArrayDimension != 0)
            {
                return;
            }
            if (mqttTag.TagNode.DataType.IdType != IdType.Numeric)
            {
                return;
            }

            // Add the value of the tag to the JSon message
            uint nType = (uint)mqttTag.TagNode.DataType.Identifier;
            switch (nType)
            {
                case (uint)Opc.Ua.DataTypes.String:
                    // Add Value to the message
                    writer.WriteValue(mqttTag.Value.Value.ToString());
                    break;

                case (uint)Opc.Ua.DataTypes.Boolean:
                    {
                        bool auxBool = GetTagValue(mqttTag, false);
                        // Add Value to the message
                        writer.WriteValue(auxBool);
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.SByte:
                    {
                        sbyte auxSByte = GetTagValue(mqttTag, (sbyte)0);
                        // Add Value to the message
                        writer.WriteValue(auxSByte);
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.Byte:
                    {
                        byte auxByte = GetTagValue(mqttTag, (byte)0);
                        // Add Value to the message
                        writer.WriteValue(auxByte);
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.Int16:
                    {
                        Int16 auxInt16 = GetTagValue(mqttTag, (Int16)0);
                        // Add Value to the message
                        writer.WriteValue(auxInt16);
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.UInt16:
                    {
                        UInt16 auxUInt16 = GetTagValue(mqttTag, (UInt16)0);
                        // Add Value to the message
                        writer.WriteValue(auxUInt16);
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.Int32:
                    {
                        Int32 auxInt32 = GetTagValue(mqttTag, (Int32)0);
                        // Add Value to the message
                        writer.WriteValue(auxInt32);
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.UInt32:
                    {
                        UInt32 auxUInt32 = GetTagValue(mqttTag, (UInt32)0);
                        // Add Value to the message
                        writer.WriteValue(auxUInt32);
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.Float:
                    {
                        float auxFloat = GetTagValue(mqttTag, 0.0F);
                        // Add Value to the message
                        writer.WriteValue(auxFloat);
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.Double:
                    {
                        double auxDouble = GetTagValue(mqttTag, 0.0);
                        // Add Value to the message
                        writer.WriteValue(auxDouble);
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.Int64:
                    {
                        Int64 auxInt64 = GetTagValue(mqttTag, (Int64)0);
                        // Add Value to the message
                        writer.WriteValue(auxInt64);
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.UInt64:
                    {
                        UInt64 auxUInt64 = GetTagValue(mqttTag, (UInt64)0);
                        // Add Value to the message
                        writer.WriteValue(auxUInt64);
                    }
                    break;
            }
        }

        private void AddArrayItemsToJSonMessage(JsonWriter writer, MQTTClientTag mqttTag)
        {
            if (mqttTag.TagNode.ArrayDimension == 0)
            {
                return;
            }
            if (mqttTag.TagNode.DataType.IdType != IdType.Numeric)
            {
                return;
            }
            Array tagValueArray = mqttTag.Value.Value as Array;
            if ((tagValueArray == null) || (tagValueArray.GetLength(0) != mqttTag.TagNode.ArrayDimension))
            {
                return;
            }

            // Open square bracket
            writer.WriteStartArray();

            // Add the values of the array elements to the JSon message
            uint nType = (uint)mqttTag.TagNode.DataType.Identifier;
            switch (nType)
            {
                case (uint)Opc.Ua.DataTypes.String:
                    {
                        string auxString = String.Empty;

                        for (uint i = 0; i < mqttTag.TagNode.ArrayDimension; i++)
                        {
                            try
                            {
                                auxString = Convert.ToString(tagValueArray.GetValue(i));
                            }
                            catch
                            {
                                auxString = String.Empty;
                            }
                            // Add Value to the message
                            writer.WriteValue(auxString);
                        }
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.Boolean:
                    {
                        bool auxBool = false;
                        for (uint i = 0; i < mqttTag.TagNode.ArrayDimension; i++)
                        {
                            try
                            {
                                auxBool = Convert.ToBoolean(tagValueArray.GetValue(i));
                            }
                            catch
                            {
                                auxBool = false;
                            }
                            // Add Value to the message
                            writer.WriteValue(auxBool);
                        }
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.SByte:
                    {
                        sbyte auxSByte = 0;
                        for (uint i = 0; i < mqttTag.TagNode.ArrayDimension; i++)
                        {
                            try
                            {
                                auxSByte = Convert.ToSByte(tagValueArray.GetValue(i));
                            }
                            catch
                            {
                                auxSByte = 0;
                            }
                            // Add Value to the message
                            writer.WriteValue(auxSByte);
                        }
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.Byte:
                    {
                        byte auxByte = 0;
                        for (uint i = 0; i < mqttTag.TagNode.ArrayDimension; i++)
                        {
                            try
                            {
                                auxByte = Convert.ToByte(tagValueArray.GetValue(i));
                            }
                            catch
                            {
                                auxByte = 0;
                            }
                            // Add Value to the message
                            writer.WriteValue(auxByte);
                        }
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.Int16:
                    {
                        Int16 auxInt16 = 0;
                        for (uint i = 0; i < mqttTag.TagNode.ArrayDimension; i++)
                        {
                            try
                            {
                                auxInt16 = Convert.ToInt16(tagValueArray.GetValue(i));
                            }
                            catch
                            {
                                auxInt16 = 0;
                            }
                            // Add Value to the message
                            writer.WriteValue(auxInt16);
                        }
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.UInt16:
                    {
                        UInt16 auxUInt16 = 0;
                        for (uint i = 0; i < mqttTag.TagNode.ArrayDimension; i++)
                        {
                            try
                            {
                                auxUInt16 = Convert.ToUInt16(tagValueArray.GetValue(i));
                            }
                            catch
                            {
                                auxUInt16 = 0;
                            }
                            // Add Value to the message
                            writer.WriteValue(auxUInt16);
                        }
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.Int32:
                    {
                        Int32 auxInt32 = 0;
                        for (uint i = 0; i < mqttTag.TagNode.ArrayDimension; i++)
                        {
                            try
                            {
                                auxInt32 = Convert.ToInt32(tagValueArray.GetValue(i));
                            }
                            catch
                            {
                                auxInt32 = 0;
                            }
                            // Add Value to the message
                            writer.WriteValue(auxInt32);
                        }
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.UInt32:
                    {
                        UInt32 auxUInt32 = 0;
                        for (uint i = 0; i < mqttTag.TagNode.ArrayDimension; i++)
                        {
                            try
                            {
                                auxUInt32 = Convert.ToUInt32(tagValueArray.GetValue(i));
                            }
                            catch
                            {
                                auxUInt32 = 0;
                            }
                            // Add Value to the message
                            writer.WriteValue(auxUInt32);
                        }
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.Float:
                    {
                        float auxFloat = 0;
                        for (uint i = 0; i < mqttTag.TagNode.ArrayDimension; i++)
                        {
                            try
                            {
                                auxFloat = Convert.ToSingle(tagValueArray.GetValue(i));
                            }
                            catch
                            {
                                auxFloat = 0.0F;
                            }
                            // Add Value to the message
                            writer.WriteValue(auxFloat);
                        }
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.Double:
                    {
                        double auxDouble = 0;
                        for (uint i = 0; i < mqttTag.TagNode.ArrayDimension; i++)
                        {
                            try
                            {
                                auxDouble = Convert.ToDouble(tagValueArray.GetValue(i));
                            }
                            catch
                            {
                                auxDouble = 0.0F;
                            }
                            // Add Value to the message
                           writer.WriteValue(auxDouble);
                        }
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.Int64:
                    {
                        Int64 auxInt64 = 0;
                        for (uint i = 0; i < mqttTag.TagNode.ArrayDimension; i++)
                        {
                            try
                            {
                                auxInt64 = Convert.ToInt64(tagValueArray.GetValue(i));
                            }
                            catch
                            {
                                auxInt64 = 0;
                            }
                            // Add Value to the message
                            writer.WriteValue(auxInt64);
                        }
                    }
                    break;

                case (uint)Opc.Ua.DataTypes.UInt64:
                    {
                        UInt64 auxUInt64 = 0;
                        for (uint i = 0; i < mqttTag.TagNode.ArrayDimension; i++)
                        {
                            try
                            {
                                auxUInt64 = Convert.ToUInt64(tagValueArray.GetValue(i));
                            }
                            catch
                            {
                                auxUInt64 = 0;
                            }
                            // Add Value to the message
                            writer.WriteValue(auxUInt64);
                        }
                    }
                    break;
            }

            // Close square bracket
            writer.WriteEnd();
        }

        MQTTClientJsonAddTimestamp TimestampMustBeAdded()
        {
            MQTTClientJsonAddTimestamp timestampMustBeAdded = MQTTClientJsonAddTimestamp.AddTimestamp_False;
            bool jsonTimestampFound = false;

            lock (lockListObject)
            {
                // Each tag of the job corresponds to an element of the structure and to a field of the JSon message
                foreach (MQTTClientTag tag in TagsList)
                {
                   if (!String.IsNullOrWhiteSpace(_JsonMessageTimestampField) && (_JsonMessageTimestampField == tag.JsonMessageFormat))
                   {
                      jsonTimestampFound = true;
                   }
                }
            }

            // Set if the timestamp must be explicitely added to a published JSon message
            if (!String.IsNullOrWhiteSpace(_JsonMessageTimestampField) && (jsonTimestampFound == false))
            {
                jsonTimeStampMustBeAdded = MQTTClientJsonAddTimestamp.AddTimestamp_True;
            }

            return (timestampMustBeAdded);
        }

        string GetJsonWriteTimestamp()
        {
            string jsonTimestamp = String.Empty;
            DateTime writeTimestamp = DateTime.UtcNow;
            if (JsonUseLocalTime && (JsonTimestampFormat != JSonTimestampFormats.tf_EPOCH))
            {
                writeTimestamp = DateTime.Now;
            }

            // Build the string of the timestamp in the selected format
            switch(JsonTimestampFormat)
            {
                case JSonTimestampFormats.tf_ISO:
                    jsonTimestamp = GetISOTimestamp(writeTimestamp);
                    break;

                case JSonTimestampFormats.tf_en_US:
                    jsonTimestamp = GetEnUSTimestamp(writeTimestamp);
                    break;

                case JSonTimestampFormats.tf_it_IT:
                    jsonTimestamp = GetItITTimestamp(writeTimestamp);
                    break;

                case JSonTimestampFormats.tf_EPOCH:
                    jsonTimestamp = GetEpochTimestamp(writeTimestamp);
                    break;

            }
            return (jsonTimestamp);
        }

        // Count the number of matching strings in two string arrays
        int GetMatchingStringNumber(string[] stringArray1, string[] stringArray2)
        {
            int matchingStringNumber = 0;
            int numberOfComparisons = stringArray1.GetLength(0);
            if(stringArray2.GetLength(0) < numberOfComparisons)
            {
                numberOfComparisons = stringArray2.GetLength(0);
            }
            for (int i= 0; i<numberOfComparisons; i++)
            {
                if(stringArray1[i] == stringArray2[i])
                {
                    matchingStringNumber++;
                }
            }

            return(matchingStringNumber);
        }

        // Prepare the payload in JSon format for the publish message (timestamp included)
        public string GetPublishJSonMessagePayloadWithTimestamp()
        {
            string messagePayload = String.Empty;

            // Get the timestamp to be added to the message;
            string jsonTimestamp = GetJsonWriteTimestamp();

            bool timestampAdded = false;

            // Split the JSon path of the timestamp
            string[] timestampItems = JsonMessageTimestampField.Split('.');
            int timestampNumberOfLevels = timestampItems.Count();
            if (timestampNumberOfLevels == 0)
            {
                // Do not add the timestamp to the message (something wrong in the name of the message field reserved to the timestamp)
                timestampAdded = true;
            }
            // Check if the timestamp value is correct
            if(String.IsNullOrWhiteSpace(jsonTimestamp))
            {
                // Do not add the timestamp to the message (something wrong in the selected format of the timestamp)
                timestampAdded = true;
            }

            int maxNumberOfMatchingLevels = 0;

            // Build the JSon message
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            // List of the items of the complete JSon message
            List<string> messageJsonItems = new List<string>();
            int countOfMessageLevels = 0;
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                // Write the message indented
                writer.Formatting = Formatting.Indented;

                lock (lockListObject)
                {
                    foreach (Tag tag in TagsListOnWriting)
                    {
                        MQTTClientTag mqttTag = (MQTTClientTag)tag;
                        if (String.IsNullOrWhiteSpace(mqttTag.JsonMessageFormat))
                        {
                            continue;
                        }
                        // Split the JSon path of the tag
                        string[] tagJsonItems = mqttTag.JsonMessageFormat.Split('.');
                        // Add the tag value to the messagge level by level
                        int tagNumberOfLevels = tagJsonItems.Count();
                        if (tagNumberOfLevels == 0)
                        {
                            continue;
                        }
                        // Skip null values
                        if (tag.Value.Value == null)
                        {
                            continue;
                        }

                        // Check the field to be added to message: tag value or timestamp?
                        bool addTimestamp = false;
                        if(!timestampAdded)
                        {
                            if(mqttTag.JsonMessageFormat != JsonMessageTimestampField)
                            {
                                // Get the number of parts of the tag field matching the name of the timestamp field
                                int numberOfMatchingLevels = GetMatchingStringNumber(tagJsonItems, timestampItems);
                                if(numberOfMatchingLevels < maxNumberOfMatchingLevels)
                                {
                                    addTimestamp = true;
                                }
                                else
                                {
                                    maxNumberOfMatchingLevels = numberOfMatchingLevels;
                                }
                            }
                            else
                            {
                                // Special case: do not add the timestamp if the value of the tag is the timestamp itself
                                timestampAdded = true;
                            }
                        }

                        // Add the timestamp to the Json message
                        if(addTimestamp)
                        {
                            for (int i = 0; i < timestampNumberOfLevels; i++)
                            {
                                countOfMessageLevels = messageJsonItems.Count;
                                if (i == countOfMessageLevels)
                                {
                                    // Open a new level in the message: Open bracket
                                    writer.WriteStartObject();
                                    messageJsonItems.Add(String.Empty);
                                    countOfMessageLevels++;
                                }
                                if (timestampItems[i] != messageJsonItems[i])
                                {
                                    // Close successive levels of the message
                                    if ((countOfMessageLevels - 1) > i)
                                    {
                                        for (int j = (countOfMessageLevels - 1); j > i; j--)
                                        {
                                            // Close bracket
                                            writer.WriteEndObject();
                                            messageJsonItems.RemoveAt(j);
                                        }
                                    }

                                    // Timestamp field name
                                    messageJsonItems[i] = timestampItems[i];
                                    writer.WritePropertyName(timestampItems[i]);
                                    if (i == (timestampNumberOfLevels - 1))
                                    {
                                       // Timestamp value
                                       writer.WriteValue(jsonTimestamp);
                                    }
                                }
                            }
                            timestampAdded = true;
                        }

                        // Add the tag value to the message
                        for (int i = 0; i < tagNumberOfLevels; i++)
                        {
                            countOfMessageLevels = messageJsonItems.Count;
                            if (i == countOfMessageLevels)
                            {
                                // Open a new level in the message: Open bracket
                                writer.WriteStartObject();
                                messageJsonItems.Add(String.Empty);
                                countOfMessageLevels++;
                            }
                            if (tagJsonItems[i] != messageJsonItems[i])
                            {
                                // Close successive levels of the message
                                if ((countOfMessageLevels - 1) > i)
                                {
                                    for (int j = (countOfMessageLevels - 1); j > i; j--)
                                    {
                                        // Close bracket
                                        writer.WriteEndObject();
                                        messageJsonItems.RemoveAt(j);
                                    }
                                }

                                // Property Name
                                messageJsonItems[i] = tagJsonItems[i];
                                writer.WritePropertyName(tagJsonItems[i]);
                                if (i == (tagNumberOfLevels - 1))
                                {
                                    if (mqttTag.TagNode.ArrayDimension == 0)
                                    {
                                        // Property Value
                                        //writer.WriteValue(mqttTag.Value.Value.ToString());
                                        AddTagValueToJSonMessage(writer, mqttTag);
                                    }
                                    // Special case: arrays
                                    else
                                    {
                                        AddArrayItemsToJSonMessage(writer, mqttTag);
                                    }
                                    mqttTag.LastValue = mqttTag.Value.Value;
                                }
                            }
                        }
                    }
                }

                // Timestamp not added? Add it at to end of the message
                if(!timestampAdded)
                {
                    for (int i = 0; i < timestampNumberOfLevels; i++)
                    {
                        countOfMessageLevels = messageJsonItems.Count;
                        if (i == countOfMessageLevels)
                        {
                            // Open a new level in the message: Open bracket
                            writer.WriteStartObject();
                            messageJsonItems.Add(String.Empty);
                            countOfMessageLevels++;
                        }
                        if (timestampItems[i] != messageJsonItems[i])
                        {
                            // Close successive levels of the message
                            if ((countOfMessageLevels - 1) > i)
                            {
                                for (int j = (countOfMessageLevels - 1); j > i; j--)
                                {
                                    // Close bracket
                                    writer.WriteEndObject();
                                    messageJsonItems.RemoveAt(j);
                                }
                            }

                            // Timestamp field name
                            messageJsonItems[i] = timestampItems[i];
                            writer.WritePropertyName(timestampItems[i]);
                            if (i == (timestampNumberOfLevels - 1))
                            {
                                // Timestamp value
                                writer.WriteValue(jsonTimestamp);
                            }
                        }
                    }
                }

                // Complete the Json message
                countOfMessageLevels = messageJsonItems.Count;
                // Close brackets
                for (int i = 0; i < countOfMessageLevels; i++)
                {
                    // Close bracket
                    writer.WriteEndObject();
                }
            }
            messagePayload = sb.ToString();

            System.Diagnostics.Debug.WriteLine(String.Format("JSon Write DBG - GetPublishJSonMessagePayloadWithTimestamp - Message Payload = {0}", messagePayload));

            return (messagePayload);
        }

        // Prepare the payload in RAW format (just the value) for the publish message
        public string GetPublishRawMessagePayload()
        {

            string messagePayload = String.Empty;
            Tag cand;

            lock (lockListObject)
            {
                if (TagsListOnWriting.Count == 0)
                {
                    return (messagePayload);
                }
                cand = TagsListOnWriting[0];
            }

            if (cand == null)
            {
                return (messagePayload);
            }

            if (cand.DynSettings.MethodID != -1)
            {
                return (messagePayload);
            }

            cand.LastValue = cand.Value.Value;
            DataValue valueTobeSent = new DataValue(cand.Value);
            messagePayload = valueTobeSent.Value.ToString();

            return (messagePayload);
        }

        // Prepare the payload in JSon format for the publish message
        public string GetPublishJSonMessagePayload()
        {
            string messagePayload = String.Empty;

            // Check if at least one tag must be written
            lock (lockListObject)
            {
                if (TagsListOnWriting.Count == 0)
                {
                    return (messagePayload);
                }
            }

            // Check if the timestamp must be explicitely added to the message
            if(jsonTimeStampMustBeAdded == MQTTClientJsonAddTimestamp.AddTimestamp_Undefined)
            {
                jsonTimeStampMustBeAdded = TimestampMustBeAdded();
            }
            if(jsonTimeStampMustBeAdded == MQTTClientJsonAddTimestamp.AddTimestamp_True)
            {
                return (GetPublishJSonMessagePayloadWithTimestamp());
            }

            // Build the JSon message
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            // Array of the items of the complete JSon message
            List<string> messageJsonItems = new List<string>();
            int countOfMessageLevels = 0;
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                // Write the message indented
                writer.Formatting = Formatting.Indented;

                lock (lockListObject)
                {
                    foreach (Tag tag in TagsListOnWriting)
                    {
                        MQTTClientTag mqttTag = (MQTTClientTag)tag;
                        if (String.IsNullOrWhiteSpace(mqttTag.JsonMessageFormat))
                        {
                            continue;
                        }
                        // Split the JSon path of the tag
                        string[] tagJsonItems = mqttTag.JsonMessageFormat.Split('.');
                        // Add the tag value to the messagge level by level
                        int tagNumberOfLevels = tagJsonItems.Count();
                        if (tagNumberOfLevels == 0)
                        {
                            continue;
                        }
                        // Skip null values
                        if(tag.Value.Value == null)
                        {
                            continue;
                        }

                        for (int i = 0; i < tagNumberOfLevels; i++)
                        {
                            countOfMessageLevels = messageJsonItems.Count;
                            if (i == countOfMessageLevels)
                            {
                                // Open a new level in the message: Open bracket
                                writer.WriteStartObject();
                                messageJsonItems.Add(String.Empty);
                                countOfMessageLevels++;
                            }
                            if(tagJsonItems[i] != messageJsonItems[i])
                            {
                                // Close successive levels of the message
                                if((countOfMessageLevels - 1) > i)
                                {
                                    for(int j = (countOfMessageLevels - 1); j > i; j--)
                                    {
                                        // Close bracket
                                        writer.WriteEndObject();
                                        messageJsonItems.RemoveAt(j);
                                    }
                                }

                                // Property Name
                                messageJsonItems[i] = tagJsonItems[i];
                                writer.WritePropertyName(tagJsonItems[i]);
                                if (i == (tagNumberOfLevels - 1))
                                {
                                    if(mqttTag.TagNode.ArrayDimension == 0)
                                    {
                                        // Property Value
                                        //writer.WriteValue(mqttTag.Value.Value.ToString());
                                        AddTagValueToJSonMessage(writer, mqttTag);
                                    }
                                    // Special case: arrays
                                    else
                                    {
                                        AddArrayItemsToJSonMessage(writer, mqttTag);
                                    }
                                    mqttTag.LastValue = mqttTag.Value.Value;
                                }
                            }
                        }
                    }
                }
                countOfMessageLevels = messageJsonItems.Count;
                // Close brackets
                for (int i = 0; i < countOfMessageLevels; i++)
                {
                    // Close bracket
                    writer.WriteEndObject();
                }
            }
            messagePayload = sb.ToString();

            System.Diagnostics.Debug.WriteLine(String.Format("JSon Write DBG - GetPublishJSonMessagePayload - Message Payload = {0}", messagePayload));

            return (messagePayload);
        }

        // Prepare the payload for the publish message
        public string GetPublishMessagePayload()
        {
            MQTTClientStation mqttStation = (MQTTClientStation)Station;
            if (mqttStation.MQTTClientMessageFormat == MQTTClientMessageFormats.JSON)
            {
                return (GetPublishJSonMessagePayload());
            }
            else if (mqttStation.MQTTClientMessageFormat == MQTTClientMessageFormats.RAW)
            {
                return (GetPublishRawMessagePayload());
            }

            string messagePayload = String.Empty;
            Tag cand;

            lock (lockListObject)
            {
                if (TagsListOnWriting.Count == 0)
                {
                    return(messagePayload);
                }
                cand = TagsListOnWriting[0];
            }

            if (cand == null)
            {
                return (messagePayload);
            }

            if (cand.DynSettings.MethodID != -1)
            {
                return (messagePayload);
            }

            cand.Value.SourceTimestamp = DateTime.UtcNow;
            cand.Value.ServerTimestamp = DateTime.UtcNow;
            cand.LastValue = cand.Value.Value;
            DataValue valueTobeSent = new DataValue(cand.Value);
            valueTobeSent.StatusCode = StatusCodes.Good;
            messagePayload = valueTobeSent.ToXml();

            return (messagePayload);
        }

        private void CheckJobValid()
        {
            string tagnamelist = string.Empty;

            List<DriverBaseInterfaces.TagDefinition> tempList = new List<DriverBaseInterfaces.TagDefinition>();
            foreach (var t in TagsList)
            {
                if (!IsTypeAdmitted(t.TagNode.DataType) && t.DynSettings.MethodID == -1)
                {
                    IsValid = false;
                    InvalidReason = string.Format("The Tag type is invalid for its Data Type. (Tag: {0} Data Type: {1})", t.TagNode.NodeId.ToString(), t.TagNode.DataType.Identifier.ToString());
                    return;
                }
                tagnamelist += (tagnamelist.Length > 0 ? ", " : string.Empty) + t.TagNode.NodeId.ToString();
                tempList.AddRange(GetSimpleTagList(t.TagNode));
            }

            string errorDesc;
            if (!ProtocolDataSizeIsValid(out errorDesc))
            {
                IsValid = false;
                InvalidReason = string.Format("The {0} is invalid for the Function Code. (Tags: {1})", errorDesc, tagnamelist);
                return;
            }
          
            if (TotalJobSize > GetMaxJobSize())
            {
                IsValid = false;
                InvalidReason = string.Format("Job exceed the maximum size (Tags: {0})", tagnamelist);
                return;
            }

            IsValid = true;
            InvalidReason = string.Empty;
        }

        public override uint GetMaxJobSize()
        {
            //return MQTTClientProtocol.GetMaxJobSize(FunctionCode, Type);
            return 2048;
        }

        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            ExtraBytes = 0;
            return JobAggregationType.JobAggregImpossible;
        }
        
        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            return false;
        }

        public override void GetJobData(ref object jobData)
        {
            var listToWrite = new List<Tag>();
            var listOnWriting = new List<Tag>();

            lock (lockListObject)
            {
                listToWrite.AddRange(TagsListToWrite);
                TagsListToWrite.Clear();
            }

            List<byte> outData = new List<byte>();
            //prepare a write request
            listToWrite.Sort(CompareTagByOffset);
            Tag cand = null;
            byte[] jobdata;
            UInt16 nData = 0;
            do
            {
                if (cand != null)
                {
                    if ((cand.ByteOffset + cand.Size) != listToWrite[0].ByteOffset)
                        break;
                }
                cand = listToWrite[0];
                listToWrite.Remove(cand);
                if (!listOnWriting.Contains(cand))
                    listOnWriting.Add(cand);
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
                outData.AddRange(jobdata);

            } while (listToWrite.Count > 0);

            lock (lockListObject)
            {
                listOnWriting.ForEach((tag) =>
                {
                    if (!TagsListOnWriting.Contains(tag))
                        TagsListOnWriting.Add(tag);
                });
                listOnWriting.Clear();

                if(listToWrite.Count > 0)
                {
                    var tempListToWrite = new List<Tag>();
                    tempListToWrite.AddRange(TagsListToWrite);
                    TagsListToWrite.Clear();
                    listToWrite.ForEach((tag) =>
                    {
                        if (!TagsListToWrite.Contains(tag))
                        {
                            TagsListToWrite.Add(tag);
                        }
                    });
                    listToWrite.Clear();
                    tempListToWrite.ForEach((tag) =>
                    {
                        if (!TagsListToWrite.Contains(tag))
                        {
                           TagsListToWrite.Add(tag);
                        }
                    });
                    tempListToWrite.Clear();
                }
            }

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

        public bool SetJobValue(string receivedValue, ref List<Tag> changed)
        {
            MQTTClientStation mqttStation = (MQTTClientStation)Station;
            if (mqttStation.MQTTClientMessageFormat == MQTTClientMessageFormats.XML)
            {
                DataValue receivedData = receivedValue.FromXml<DataValue>();
                lock (lockListObject)
                {
                    if (TagsList.Count > 0)
                    {
                        if (!receivedData.Equals(TagsList[0].Value))
                        {
#if DEBUG
                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                            System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} SetJobValue: received new value for var {1}",
                                                               currentTime, TagName));
                            System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} old value = {1}, old stat = {2}, old sourcet = {3}, old servert = {4}",
                                                               currentTime, TagsList[0].Value.Value, TagsList[0].Value.StatusCode, TagsList[0].Value.SourceTimestamp.ToString("HH:mm:ss.fff"), TagsList[0].Value.ServerTimestamp.ToString("HH:mm:ss.fff")));
                            System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} new value = {1}, new stat = {2}, new sourcet = {3}, new servert = {4}",
                                                               currentTime, receivedData.Value, receivedData.StatusCode, receivedData.SourceTimestamp.ToString("HH:mm:ss.fff"), receivedData.ServerTimestamp.ToString("HH:mm:ss.fff")));
#endif
                            TagsList[0].Value.Value = Utils.Clone(receivedData.Value);
                            TagsList[0].Value.StatusCode = receivedData.StatusCode;
                            TagsList[0].Value.SourceTimestamp = receivedData.SourceTimestamp;
                            TagsList[0].Value.ServerTimestamp = receivedData.ServerTimestamp;
                            changed.Add(TagsList[0]);
                        }
#if DEBUG
                        else
                        {
                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                            System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} SetJobValue: discarded value for var {1}, value = {2}",
                                                               currentTime, TagName, receivedData.Value));

                        }
#endif
                    }
                }
                return (true);
            }
            else if (mqttStation.MQTTClientMessageFormat == MQTTClientMessageFormats.JSON)
            {
                lock (lockListObject)
                {
                    DateTime parsedTimestamp = DateTime.MinValue;
                    if(!String.IsNullOrWhiteSpace(JsonMessageTimestampField))
                    {
                        parsedTimestamp = ParseJsonMessageTimestamp(receivedValue, JsonMessageTimestampField, JsonTimestampFormat, JsonUseLocalTime);
                    }
                    for (int i=0; i<TagsList.Count; i++)
                    {
                        DataValue receivedData = ParseJsonMessage(receivedValue, TagsList[i]);
                        if (receivedData != null)
                        {
                            if (!receivedData.Equals(TagsList[i].Value.Value) ||
                                ((parsedTimestamp != DateTime.MinValue) && (parsedTimestamp != TagsList[i].Value.SourceTimestamp)))
                            {
                                TagsList[i].Value.Value = Utils.Clone(receivedData.Value);
                                TagsList[i].Value.StatusCode = receivedData.StatusCode;
                                if(parsedTimestamp == DateTime.MinValue)
                                {
                                    TagsList[i].Value.SourceTimestamp = receivedData.SourceTimestamp;
                                    TagsList[i].Value.ServerTimestamp = receivedData.ServerTimestamp;
                                }
                                else
                                {
                                    TagsList[i].Value.SourceTimestamp = parsedTimestamp;
                                    TagsList[i].Value.ServerTimestamp = parsedTimestamp;
                                }
                                changed.Add(TagsList[i]);
#if DEBUG
                                MQTTClientTag mqttTag = (MQTTClientTag)TagsList[i];
                                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                                System.Diagnostics.Debug.WriteLine(String.Format("JSON DBG - {0} SetJobValue: received new value {1} for var {2} (tag index: {3}/{4})", currentTime, mqttTag.Value.Value.ToString(), mqttTag.JsonMessageFormat, i, TagsList.Count));
#endif
                            }
#if DEBUG
                            else
                            {
                                MQTTClientTag mqttTag = (MQTTClientTag)TagsList[i];
                                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                                System.Diagnostics.Debug.WriteLine(String.Format("JSON DBG - {0} SetJobValue: discarded value {1} for var {2} (tag index {3}/{4})", currentTime, mqttTag.Value.Value.ToString(), mqttTag.JsonMessageFormat, i, TagsList.Count));
                            }
#endif
                        }
                    }
                }

                return (true);
            }

            // Raw Format
            else
            {
                lock (lockListObject)
                {
                    object receivedObject = receivedValue;
                    if (TagsList.Count > 0)
                    {
                        if ((TagsList[0].Value.Value == null) || (TagsList[0].Value.Value != receivedObject))
                        {
                            TagsList[0].Value.Value = Utils.Clone(receivedObject);
                            TagsList[0].Value.SourceTimestamp = DateTime.UtcNow;
                            TagsList[0].Value.ServerTimestamp = TagsList[0].Value.SourceTimestamp;
                            changed.Add(TagsList[0]);
#if DEBUG
                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                            System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} SetJobValue - RAW Format - Received new value {1} for var {2}",
                                                               currentTime, receivedValue, TagName));
#endif
                        }
                        else
                        {
                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                            System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} SetJobValue - RAW Format - Discarded value {1} for var {2}",
                                                                currentTime, receivedValue, TagName));
                        }
                    }
                }
                return (true);
            }
        }

        public bool HysteresysCanBeApplied()
        {
            // The Hysteresis threshold does meaning only for exception output jobs
            // and tags of numeric type (no structure, array, boolean or string tags)
            if (Type != LinkType.ExceptionOutput)
            {
                System.Diagnostics.Debug.WriteLine("HysteresysCanBeApplied - return (false) - Type != LinkType.ExceptionOutput");
                return (false);
            }
            if ((TagsList.Count != 1) || (TagsList[0].TagNode.DataType.IdType != IdType.Numeric))
            {
                System.Diagnostics.Debug.WriteLine("HysteresysCanBeApplied - return (false) - (TagsList.Count != 1) || (TagsList[0].TagNode.DataType.IdType != IdType.Numeric)");
                return (false);
            }
            if (TagsList[0].TagNode.ArrayDimension > 0)
            {
                System.Diagnostics.Debug.WriteLine("HysteresysCanBeApplied - return (false) - TagsList[0].TagNode.ArrayDimension > 0");
                return (false);
            }
            uint tagType = (uint)TagsList[0].TagNode.DataType.Identifier;
            if((tagType == (uint)Opc.Ua.DataTypes.Boolean) || (tagType == (uint)Opc.Ua.DataTypes.String))
            {
                System.Diagnostics.Debug.WriteLine("HysteresysCanBeApplied - return (false) - (tagType == (uint)Opc.Ua.DataTypes.Boolean) || (tagType == (uint)Opc.Ua.DataTypes.String)");
                return (false);
            }

            System.Diagnostics.Debug.WriteLine("HysteresysCanBeApplied - return (true)");
            return (true);
        }

        // Check if the difference between a value and the last published value exceeds the hysteresys threshold
        public bool CheckHysteresis(ref object value)
        {
            // The Hysteresis threshold does meaning only for exception output jobs
            // and tags of numeric type (no structure, array, boolean or string tags)
            if(!HysteresysCanBeApplied())
            {
                System.Diagnostics.Debug.WriteLine("CheckHysteresis - return (true) - HysteresysCanBeApplied returned false");
                return (true);
            }

            uint tagType = (uint)TagsList[0].TagNode.DataType.Identifier;

            MQTTClientTag mqttTag = (MQTTClientTag)TagsList[0];
            // Check if the tag value has been already published at least one time
            if (mqttTag.LastPublishedValue == null)
            {
                System.Diagnostics.Debug.WriteLine("CheckHysteresis - return (true) - mqttTag.LastPublishedValue == null");
                return (true);
            }

            object auxObject = Utils.Clone(value);

            try
            {
                switch (tagType)
                {
                    case (uint)Opc.Ua.DataTypes.SByte:
                        {
                            if ((HysteresisThreshold > 0.0) && ((double)Math.Abs((SByte)mqttTag.LastPublishedValue - (SByte)auxObject) < _HysteresisThreshold))
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("CheckHysteresis - return (false) - case (uint)Opc.Ua.DataTypes.SByte - mqttTag.LastPublishedValue = {0} - value = {1} - _HysteresisThreshold = {2}",
                                                                                 (SByte)mqttTag.LastPublishedValue, (SByte)auxObject, _HysteresisThreshold));
                                return (false);
                            }
#if DEBUG
                            else
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("CheckHysteresis - OK - case (uint)Opc.Ua.DataTypes.SByte - mqttTag.LastPublishedValue = {0} - value = {1} - _HysteresisThreshold = {2}",
                                                                                 (SByte)mqttTag.LastPublishedValue, (SByte)auxObject, _HysteresisThreshold));
                            }
#endif
                        }
                        break;

                    case (uint)Opc.Ua.DataTypes.Byte:
                        {
                            if ((HysteresisThreshold > 0.0) && ((double)Math.Abs((Byte)mqttTag.LastPublishedValue - (Byte)auxObject) < _HysteresisThreshold))
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("CheckHysteresis - return (false) - case (uint)Opc.Ua.DataTypes.Byte - mqttTag.LastPublishedValue = {0} - value = {1} - _HysteresisThreshold = {2}",
                                                                                 (Byte)mqttTag.LastPublishedValue, (Byte)auxObject, _HysteresisThreshold));
                                return (false);
                            }
#if DEBUG
                            else
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("CheckHysteresis - OK - case (uint)Opc.Ua.DataTypes.Byte - mqttTag.LastPublishedValue = {0} - value = {1} - _HysteresisThreshold = {2}",
                                                                                 (Byte)mqttTag.LastPublishedValue, (Byte)auxObject, _HysteresisThreshold));
                            }
#endif
                        }
                        break;

                    case (uint)Opc.Ua.DataTypes.Int16:
                        {
                            if ((HysteresisThreshold > 0.0) && ((double)Math.Abs((Int16)mqttTag.LastPublishedValue - (Int16)auxObject) < _HysteresisThreshold))
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("CheckHysteresis - return (false) - case (uint)Opc.Ua.DataTypes.Int16 - mqttTag.LastPublishedValue = {0} - value = {1} - _HysteresisThreshold = {2}",
                                                                                 (Int16)mqttTag.LastPublishedValue, (Int16)auxObject, _HysteresisThreshold));
                                return (false);
                            }
#if DEBUG
                            else
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("CheckHysteresis - OK - case (uint)Opc.Ua.DataTypes.Int16 - mqttTag.LastPublishedValue = {0} - value = {1} - _HysteresisThreshold = {2}",
                                                                                 (Int16)mqttTag.LastPublishedValue, (Int16)auxObject, _HysteresisThreshold));
                            }
#endif
                        }
                        break;

                    case (uint)Opc.Ua.DataTypes.UInt16:
                        {
                            if ((HysteresisThreshold > 0.0) && ((double)Math.Abs((UInt16)mqttTag.LastPublishedValue - (UInt16)auxObject) < _HysteresisThreshold))
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("CheckHysteresis - return (false) - case (uint)Opc.Ua.DataTypes.UInt16 - mqttTag.LastPublishedValue = {0} - value = {1} - _HysteresisThreshold = {2}",
                                                                                 (UInt16)mqttTag.LastPublishedValue, (UInt16)auxObject, _HysteresisThreshold));
                                return (false);
                            }
#if DEBUG
                            else
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("CheckHysteresis - OK - case (uint)Opc.Ua.DataTypes.UInt16 - mqttTag.LastPublishedValue = {0} - value = {1} - _HysteresisThreshold = {2}",
                                                                                 (UInt16)mqttTag.LastPublishedValue, (UInt16)auxObject, _HysteresisThreshold));
                            }
#endif
                        }
                        break;

                    case (uint)Opc.Ua.DataTypes.Int32:
                        {
                            if ((HysteresisThreshold > 0.0) && ((double)Math.Abs((Int32)mqttTag.LastPublishedValue - (Int32)auxObject) < _HysteresisThreshold))
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("CheckHysteresis - return (false) - case (uint)Opc.Ua.DataTypes.Int32 - mqttTag.LastPublishedValue = {0} - value = {1} - _HysteresisThreshold = {2}",
                                                                                 (Int32)mqttTag.LastPublishedValue, (Int32)auxObject, _HysteresisThreshold));
                                return (false);
                            }
#if DEBUG
                            else
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("CheckHysteresis - OK - case (uint)Opc.Ua.DataTypes.Int32 - mqttTag.LastPublishedValue = {0} - value = {1} - _HysteresisThreshold = {2}",
                                                                                 (Int32)mqttTag.LastPublishedValue, (Int32)auxObject, _HysteresisThreshold));
                            }
#endif
                        }
                        break;

                    case (uint)Opc.Ua.DataTypes.UInt32:
                        {
                            if ((HysteresisThreshold > 0.0) && ((double)Math.Abs((UInt32)mqttTag.LastPublishedValue - (UInt32)auxObject) < _HysteresisThreshold))
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("CheckHysteresis - return (false) - case (uint)Opc.Ua.DataTypes.UInt32 - mqttTag.LastPublishedValue = {0} - value = {1} - _HysteresisThreshold = {2}",
                                                                                 (UInt32)mqttTag.LastPublishedValue, (UInt32)auxObject, _HysteresisThreshold));
                                return (false);
                            }
#if DEBUG
                            else
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("CheckHysteresis - OK - case (uint)Opc.Ua.DataTypes.UInt32 - mqttTag.LastPublishedValue = {0} - value = {1} - _HysteresisThreshold = {2}",
                                                                                 (UInt32)mqttTag.LastPublishedValue, (UInt32)auxObject, _HysteresisThreshold));
                            }
#endif
                        }
                        break;

                    case (uint)Opc.Ua.DataTypes.Int64:
                        {
                            if ((HysteresisThreshold > 0.0) && ((double)Math.Abs((Int64)mqttTag.LastPublishedValue - (Int64)auxObject) < _HysteresisThreshold))
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("CheckHysteresis - return (false) - case (uint)Opc.Ua.DataTypes.Int64 - mqttTag.LastPublishedValue = {0} - value = {1} - _HysteresisThreshold = {2}",
                                                                                (Int64)mqttTag.LastPublishedValue, (Int64)auxObject, _HysteresisThreshold));
                                return (false);
                            }
#if DEBUG
                            else
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("CheckHysteresis - OK - case (uint)Opc.Ua.DataTypes.Int64 - mqttTag.LastPublishedValue = {0} - value = {1} - _HysteresisThreshold = {2}",
                                                                                 (Int64)mqttTag.LastPublishedValue, (Int64)auxObject, _HysteresisThreshold));
                            }
#endif
                        }
                        break;

                    case (uint)Opc.Ua.DataTypes.UInt64:
                        {
                            if ((HysteresisThreshold > 0.0) && ((double)Math.Abs((Int64)((UInt64)mqttTag.LastPublishedValue - (UInt64)auxObject)) < _HysteresisThreshold))
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("CheckHysteresis - return (false) - case (uint)Opc.Ua.DataTypes.UInt64 - mqttTag.LastPublishedValue = {0} - value = {1} - _HysteresisThreshold = {2}",
                                                                                 (UInt64)mqttTag.LastPublishedValue, (UInt64)auxObject, _HysteresisThreshold));
                                return (false);
                            }
#if DEBUG
                            else
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("CheckHysteresis - OK - case (uint)Opc.Ua.DataTypes.UInt64 - mqttTag.LastPublishedValue = {0} - value = {1} - _HysteresisThreshold = {2}",
                                                                                 (UInt64)mqttTag.LastPublishedValue, (UInt64)auxObject, _HysteresisThreshold));
                            }
#endif
                        }
                        break;

                    case (uint)Opc.Ua.DataTypes.Float:
                        {
                            if ((HysteresisThreshold > 0.0) && ((double)Math.Abs((float)mqttTag.LastPublishedValue - (float)auxObject) < _HysteresisThreshold))
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("CheckHysteresis - return (false) - case (uint)Opc.Ua.DataTypes.Float - mqttTag.LastPublishedValue = {0} - value = {1} - _HysteresisThreshold = {2}",
                                                                                 (float)mqttTag.LastPublishedValue, (float)auxObject, _HysteresisThreshold));
                                return (false);
                            }
#if DEBUG
                            else
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("CheckHysteresis - OK - case (uint)Opc.Ua.DataTypes.Float - mqttTag.LastPublishedValue = {0} - value = {1} - _HysteresisThreshold = {2}",
                                                                                 (float)mqttTag.LastPublishedValue, (float)auxObject, _HysteresisThreshold));
                            }
#endif
                        }
                        break;

                    case (uint)Opc.Ua.DataTypes.Double:
                        {
                            if ((HysteresisThreshold > 0.0) && (Math.Abs((double)mqttTag.LastPublishedValue - (double)auxObject) < _HysteresisThreshold))
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("CheckHysteresis - return (false) - case (uint)Opc.Ua.DataTypes.Double - mqttTag.LastPublishedValue = {0} - value = {1} - _HysteresisThreshold = {2}",
                                                                                 (double)mqttTag.LastPublishedValue, (double)auxObject, _HysteresisThreshold));
                                return (false);
                            }
#if DEBUG
                            else
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("CheckHysteresis - OK - case (uint)Opc.Ua.DataTypes.Double - mqttTag.LastPublishedValue = {0} - value = {1} - _HysteresisThreshold = {2}",
                                                                                 (double)mqttTag.LastPublishedValue, (double)auxObject, _HysteresisThreshold));
                            }
#endif
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("CheckHysteresis - return (true) - Exception");
                return (true);
            }

            System.Diagnostics.Debug.WriteLine("CheckHysteresis - return (true) - End");
            return (true);
        }

        // Returns the value of an element of a JSon message already tokenized
        JToken GetJTokenFromJObject(ref JObject parsedObject, string tokenKey)
        {
            JToken token = null;
            if((parsedObject != null) && !String.IsNullOrWhiteSpace(tokenKey))
            {
                try
                {
                    // tokenKey could be a JPath expression (a regular expression)
                    token = parsedObject.SelectToken(tokenKey);
                }
                catch(Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Exception (1) in GetJTokenFromJObject: {0}", e.Message);
                    token = null;
                }

                // If SelectToken fails, try to match exactly the tokenKey
                if (token == null)
                {
                    string[] tokenArray = tokenKey.Split('.');
                    int numberOfTokens = tokenArray.Length;
                    JToken auxToken = null;
                    JToken lastToken = parsedObject;

                    for (int i=0; i<numberOfTokens; i++)
                    {
                        try
                        {
                            // tokenKey could be a JPath expression (a regular expression)
                            auxToken = lastToken.SelectToken(tokenArray[i]);
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine("Exception (2) in GetJTokenFromJObject: {0}", e.Message);
                            auxToken = null;
                        }
                        // If SelectToken fails, try to match exactly the tokenKey
                        if (auxToken == null)
                        {
                            auxToken = lastToken[tokenArray[i]];
                        }
                        if (auxToken == null)
                        {
                            break;
                        }
                        lastToken = auxToken;
                    }
                    token = auxToken;
                }
            }
            return (token);
        }

        DataValue ParseJsonMessage(string receivedValue)
        {
            if(String.IsNullOrWhiteSpace(JsonMessageFormat))
            {
                return (null);
            }

            DataValue receivedData = null;
            try
            {
                lock (lockListObject)
                {
                    if (TagsList.Count > 0)
                    {
                        if (TagsList[0].TagNode.DataType.IdType == IdType.Numeric)
                        {
                            string jsonMessage = receivedValue;
                            JObject parsedObject = JObject.Parse(jsonMessage);
                            uint nType = (uint)TagsList[0].TagNode.DataType.Identifier;
                            switch (nType)
                            {
                                case (uint)Opc.Ua.DataTypes.String:
                                    {
                                        receivedData = new DataValue();
                                        if(TagsList[0].TagNode.ArrayDimension == 0)
                                        {
                                            JToken token = GetJTokenFromJObject(ref parsedObject, JsonMessageFormat);
                                            if (token == null)
                                            {
                                                return (null);
                                            }
                                            receivedData.Value = (string)token;
                                        }
                                        else
                                        {
                                            string[] auxValue = new string[TagsList[0].TagNode.ArrayDimension];
                                            for(uint i=0; i<TagsList[0].TagNode.ArrayDimension; i++)
                                            {
                                                string tokenIndexString = String.Format("{0}[{1}]", JsonMessageFormat, i); 
                                                JToken token = GetJTokenFromJObject(ref parsedObject, tokenIndexString);
                                                if (token == null)
                                                {
                                                    return (null);
                                                }
                                                auxValue[i] = (string)token;
                                            }
                                            receivedData.Value = auxValue;
                                        }
                                        receivedData.StatusCode = StatusCodes.Good;
                                        receivedData.SourceTimestamp = DateTime.UtcNow;
                                        receivedData.ServerTimestamp = receivedData.SourceTimestamp;
                                    }
                                    break;
                                case (uint)Opc.Ua.DataTypes.Boolean:
                                    {
                                        receivedData = new DataValue();
                                        if (TagsList[0].TagNode.ArrayDimension == 0)
                                        {
                                            JToken token = GetJTokenFromJObject(ref parsedObject, JsonMessageFormat);
                                            if (token == null)
                                            {
                                                return (null);
                                            }
                                            receivedData.Value = (bool)token;
                                        }
                                        else
                                        {
                                            bool[] auxValue = new bool[TagsList[0].TagNode.ArrayDimension];
                                            for (uint i = 0; i < TagsList[0].TagNode.ArrayDimension; i++)
                                            {
                                                string tokenIndexString = String.Format("{0}[{1}]", JsonMessageFormat, i);
                                                JToken token = GetJTokenFromJObject(ref parsedObject, tokenIndexString);
                                                if (token == null)
                                                {
                                                    return (null);
                                                }
                                                auxValue[i] = (bool)token;
                                            }
                                            receivedData.Value = auxValue;
                                        }
                                        receivedData.StatusCode = StatusCodes.Good;
                                        receivedData.SourceTimestamp = DateTime.UtcNow;
                                        receivedData.ServerTimestamp = receivedData.SourceTimestamp;
                                    }
                                    break;
                                case (uint)Opc.Ua.DataTypes.SByte:
                                    {
                                        receivedData = new DataValue();
                                        if (TagsList[0].TagNode.ArrayDimension == 0)
                                        {
                                            JToken token = GetJTokenFromJObject(ref parsedObject, JsonMessageFormat);
                                            if (token == null)
                                            {
                                                return (null);
                                            }
                                            receivedData.Value = (sbyte)token;
                                        }
                                        else
                                        {
                                            sbyte[] auxValue = new sbyte[TagsList[0].TagNode.ArrayDimension];
                                            for (uint i = 0; i < TagsList[0].TagNode.ArrayDimension; i++)
                                            {
                                                string tokenIndexString = String.Format("{0}[{1}]", JsonMessageFormat, i);
                                                JToken token = GetJTokenFromJObject(ref parsedObject, tokenIndexString);
                                                if (token == null)
                                                {
                                                    return (null);
                                                }
                                                auxValue[i] = (sbyte)token;
                                            }
                                            receivedData.Value = auxValue;
                                        }
                                        receivedData.StatusCode = StatusCodes.Good;
                                        receivedData.SourceTimestamp = DateTime.UtcNow;
                                        receivedData.ServerTimestamp = receivedData.SourceTimestamp;
                                    }
                                    break;
                                case (uint)Opc.Ua.DataTypes.Byte:
                                    {
                                        receivedData = new DataValue();
                                        if (TagsList[0].TagNode.ArrayDimension == 0)
                                        {
                                            JToken token = GetJTokenFromJObject(ref parsedObject, JsonMessageFormat);
                                            if (token == null)
                                            {
                                                return (null);
                                            }
                                            receivedData.Value = (byte)token;
                                        }
                                        else
                                        {
                                            byte[] auxValue = new byte[TagsList[0].TagNode.ArrayDimension];
                                            for (uint i = 0; i < TagsList[0].TagNode.ArrayDimension; i++)
                                            {
                                                string tokenIndexString = String.Format("{0}[{1}]", JsonMessageFormat, i);
                                                JToken token = GetJTokenFromJObject(ref parsedObject, tokenIndexString);
                                                if (token == null)
                                                {
                                                    return (null);
                                                }
                                                auxValue[i] = (byte)token;
                                            }
                                            receivedData.Value = auxValue;
                                        }
                                        receivedData.StatusCode = StatusCodes.Good;
                                        receivedData.SourceTimestamp = DateTime.UtcNow;
                                        receivedData.ServerTimestamp = receivedData.SourceTimestamp;
                                    }
                                    break;
                                case (uint)Opc.Ua.DataTypes.Int16:
                                    {
                                        receivedData = new DataValue();
                                        if (TagsList[0].TagNode.ArrayDimension == 0)
                                        {
                                            JToken token = GetJTokenFromJObject(ref parsedObject, JsonMessageFormat);
                                            if (token == null)
                                            {
                                                return (null);
                                            }
                                            receivedData.Value = (short)token;
                                        }
                                        else
                                        {
                                            Int16[] auxValue = new Int16[TagsList[0].TagNode.ArrayDimension];
                                            for (uint i = 0; i < TagsList[0].TagNode.ArrayDimension; i++)
                                            {
                                                string tokenIndexString = String.Format("{0}[{1}]", JsonMessageFormat, i);
                                                JToken token = GetJTokenFromJObject(ref parsedObject, tokenIndexString);
                                                if (token == null)
                                                {
                                                    return (null);
                                                }
                                                auxValue[i] = (short)token;
                                            }
                                            receivedData.Value = auxValue;
                                        }
                                        receivedData.StatusCode = StatusCodes.Good;
                                        receivedData.SourceTimestamp = DateTime.UtcNow;
                                        receivedData.ServerTimestamp = receivedData.SourceTimestamp;
                                    }
                                    break;
                                case (uint)Opc.Ua.DataTypes.UInt16:
                                    {
                                        receivedData = new DataValue();
                                        if (TagsList[0].TagNode.ArrayDimension == 0)
                                        {
                                            JToken token = GetJTokenFromJObject(ref parsedObject, JsonMessageFormat);
                                            if (token == null)
                                            {
                                                return (null);
                                            }
                                            receivedData.Value = (ushort)token;
                                        }
                                        else
                                        {
                                            UInt16[] auxValue = new UInt16[TagsList[0].TagNode.ArrayDimension];
                                            for (uint i = 0; i < TagsList[0].TagNode.ArrayDimension; i++)
                                            {
                                                string tokenIndexString = String.Format("{0}[{1}]", JsonMessageFormat, i);
                                                JToken token = GetJTokenFromJObject(ref parsedObject, tokenIndexString);
                                                if (token == null)
                                                {
                                                    return (null);
                                                }
                                                auxValue[i] = (ushort)token;
                                            }
                                            receivedData.Value = auxValue;
                                        }
                                        receivedData.StatusCode = StatusCodes.Good;
                                        receivedData.SourceTimestamp = DateTime.UtcNow;
                                        receivedData.ServerTimestamp = receivedData.SourceTimestamp;
                                    }
                                    break;
                                case (uint)Opc.Ua.DataTypes.Int32:
                                    {
                                        receivedData = new DataValue();
                                        if (TagsList[0].TagNode.ArrayDimension == 0)
                                        {
                                            JToken token = GetJTokenFromJObject(ref parsedObject, JsonMessageFormat);
                                            if (token == null)
                                            {
                                                return (null);
                                            }
                                            receivedData.Value = (int)token;
                                        }
                                        else
                                        {
                                            Int32[] auxValue = new Int32[TagsList[0].TagNode.ArrayDimension];
                                            for (uint i = 0; i < TagsList[0].TagNode.ArrayDimension; i++)
                                            {
                                                string tokenIndexString = String.Format("{0}[{1}]", JsonMessageFormat, i);
                                                JToken token = GetJTokenFromJObject(ref parsedObject, tokenIndexString);
                                                if (token == null)
                                                {
                                                    return (null);
                                                }
                                                auxValue[i] = (int)token;
                                            }
                                            receivedData.Value = auxValue;
                                        }
                                        receivedData.StatusCode = StatusCodes.Good;
                                        receivedData.SourceTimestamp = DateTime.UtcNow;
                                        receivedData.ServerTimestamp = receivedData.SourceTimestamp;
                                    }
                                    break;
                                case (uint)Opc.Ua.DataTypes.UInt32:
                                    {
                                        receivedData = new DataValue();
                                        if (TagsList[0].TagNode.ArrayDimension == 0)
                                        {
                                            JToken token = GetJTokenFromJObject(ref parsedObject, JsonMessageFormat);
                                            if (token == null)
                                            {
                                                return (null);
                                            }
                                            receivedData.Value = (uint)token;
                                        }
                                        else
                                        {
                                            UInt32[] auxValue = new UInt32[TagsList[0].TagNode.ArrayDimension];
                                            for (uint i = 0; i < TagsList[0].TagNode.ArrayDimension; i++)
                                            {
                                                string tokenIndexString = String.Format("{0}[{1}]", JsonMessageFormat, i);
                                                JToken token = GetJTokenFromJObject(ref parsedObject, tokenIndexString);
                                                if (token == null)
                                                {
                                                    return (null);
                                                }
                                                auxValue[i] = (uint)token;
                                            }
                                            receivedData.Value = auxValue;
                                        }
                                        receivedData.StatusCode = StatusCodes.Good;
                                        receivedData.SourceTimestamp = DateTime.UtcNow;
                                        receivedData.ServerTimestamp = receivedData.SourceTimestamp;
                                    }
                                    break;
                                case (uint)Opc.Ua.DataTypes.Int64:
                                    {
                                        receivedData = new DataValue();
                                        if (TagsList[0].TagNode.ArrayDimension == 0)
                                        {
                                            JToken token = GetJTokenFromJObject(ref parsedObject, JsonMessageFormat);
                                            if (token == null)
                                            {
                                                return (null);
                                            }
                                            receivedData.Value = (long)token;
                                        }
                                        else
                                        {
                                            Int64[] auxValue = new Int64[TagsList[0].TagNode.ArrayDimension];
                                            for (uint i = 0; i < TagsList[0].TagNode.ArrayDimension; i++)
                                            {
                                                string tokenIndexString = String.Format("{0}[{1}]", JsonMessageFormat, i);
                                                JToken token = GetJTokenFromJObject(ref parsedObject, tokenIndexString);
                                                if (token == null)
                                                {
                                                    return (null);
                                                }
                                                auxValue[i] = (long)token;
                                            }
                                            receivedData.Value = auxValue;
                                        }
                                        receivedData.StatusCode = StatusCodes.Good;
                                        receivedData.SourceTimestamp = DateTime.UtcNow;
                                        receivedData.ServerTimestamp = receivedData.SourceTimestamp;
                                    }
                                    break;
                                case (uint)Opc.Ua.DataTypes.UInt64:
                                    {
                                        receivedData = new DataValue();
                                        if (TagsList[0].TagNode.ArrayDimension == 0)
                                        {
                                            JToken token = GetJTokenFromJObject(ref parsedObject, JsonMessageFormat);
                                            if (token == null)
                                            {
                                                return (null);
                                            }
                                            receivedData.Value = (ulong)token;
                                        }
                                        else
                                        {
                                            UInt64[] auxValue = new UInt64[TagsList[0].TagNode.ArrayDimension];
                                            for (uint i = 0; i < TagsList[0].TagNode.ArrayDimension; i++)
                                            {
                                                string tokenIndexString = String.Format("{0}[{1}]", JsonMessageFormat, i);
                                                JToken token = GetJTokenFromJObject(ref parsedObject, tokenIndexString);
                                                if (token == null)
                                                {
                                                    return (null);
                                                }
                                                auxValue[i] = (ulong)token;
                                            }
                                            receivedData.Value = auxValue;
                                        }
                                        receivedData.StatusCode = StatusCodes.Good;
                                        receivedData.SourceTimestamp = DateTime.UtcNow;
                                        receivedData.ServerTimestamp = receivedData.SourceTimestamp;
                                    }
                                    break;
                                case (uint)Opc.Ua.DataTypes.Float:
                                    {
                                        receivedData = new DataValue();
                                        if (TagsList[0].TagNode.ArrayDimension == 0)
                                        {
                                            JToken token = GetJTokenFromJObject(ref parsedObject, JsonMessageFormat);
                                            if (token == null)
                                            {
                                                return (null);
                                            }
                                            receivedData.Value = (float)token;
                                        }
                                        else
                                        {
                                            float[] auxValue = new float[TagsList[0].TagNode.ArrayDimension];
                                            for (uint i = 0; i < TagsList[0].TagNode.ArrayDimension; i++)
                                            {
                                                string tokenIndexString = String.Format("{0}[{1}]", JsonMessageFormat, i);
                                                JToken token = GetJTokenFromJObject(ref parsedObject, tokenIndexString);
                                                if (token == null)
                                                {
                                                    return (null);
                                                }
                                                auxValue[i] = (float)token;
                                            }
                                            receivedData.Value = auxValue;
                                        }
                                        receivedData.StatusCode = StatusCodes.Good;
                                        receivedData.SourceTimestamp = DateTime.UtcNow;
                                        receivedData.ServerTimestamp = receivedData.SourceTimestamp;
                                    }
                                    break;
                                case (uint)Opc.Ua.DataTypes.Double:
                                    {
                                        receivedData = new DataValue();
                                        if (TagsList[0].TagNode.ArrayDimension == 0)
                                        {
                                            JToken token = GetJTokenFromJObject(ref parsedObject, JsonMessageFormat);
                                            if (token == null)
                                            {
                                                return (null);
                                            }
                                            receivedData.Value = (double)token;
                                        }
                                        else
                                        {
                                            double[] auxValue = new double[TagsList[0].TagNode.ArrayDimension];
                                            for (uint i = 0; i < TagsList[0].TagNode.ArrayDimension; i++)
                                            {
                                                string tokenIndexString = String.Format("{0}[{1}]", JsonMessageFormat, i);
                                                JToken token = GetJTokenFromJObject(ref parsedObject, tokenIndexString);
                                                if (token == null)
                                                {
                                                    return (null);
                                                }
                                                auxValue[i] = (double)token;
                                            }
                                            receivedData.Value = auxValue;
                                        }
                                        receivedData.StatusCode = StatusCodes.Good;
                                        receivedData.SourceTimestamp = DateTime.UtcNow;
                                        receivedData.ServerTimestamp = receivedData.SourceTimestamp;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exeception in ParseJsonMessage: {0}", e.Message);
                receivedData = null;
            }

            return (receivedData);
        }

        string GetEpochTimestamp(DateTime timestamp)
        {
            // Get the EPOCH time: the number of seconds that have elapsed since January 1, 1970 (midnight UTC/GMT),
            // not counting leap seconds (in ISO 8601: 1970-01-01T00:00:00Z)
            Int64 epochTime = (Int64)(timestamp - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            string timestampString = epochTime.ToString();
            return (timestampString);
        }

        DateTime GetEpochTimestamp(JToken token)
        {
            DateTime messageTimestamp = DateTime.MinValue;

            // Get the EPOCH time: the number of seconds that have elapsed since January 1, 1970 (midnight UTC/GMT),
            // not counting leap seconds (in ISO 8601: 1970-01-01T00:00:00Z)
            string auxString = token.ToString();
            Int64 epochTime = 0;
            if (!Int64.TryParse(auxString, out epochTime))
            {
                return (messageTimestamp);
            }

            try
            {
                // Convert the EPOCH time in a DateTime object
                messageTimestamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(epochTime);
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("Exception in GetEpochTimestamp (case 1): {0}", e.Message));
                messageTimestamp = DateTime.MinValue;
            }

            return (messageTimestamp);
        }

        string GetItITTimestamp(DateTime timestamp)
        {
            string timestampString = String.Empty;
            if (!JsonUseLocalTime)
            {
                // UTC time: the string format must be "DD-MM-YYYYTHH:MM:SS.mmmZ".
                timestampString = timestamp.ToString("dd-MM-yyyyTHH:mm:ss.fffZ");
            }
            else
            {
                // Local time: the string format must be "DD-MM-YYYYTHH:MM:SS.mmm".
                timestampString = timestamp.ToString("dd-MM-yyyyTHH:mm:ss.fff");
            }
            return (timestampString);
        }

        DateTime GetItITTimestamp(JToken token, bool useLocalTime)
        {
            DateTime messageTimestamp = DateTime.MinValue;
            if (token.Type == JTokenType.Date)
            {
                try
                {
                    // If the message element is a Date, convert it to a DateTime object
                    messageTimestamp = (DateTime)token;
                    if (messageTimestamp == null)
                    {
                        messageTimestamp = DateTime.MinValue;
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Exception in GetItITTimestamp (case 1): {0}", e.Message));
                    messageTimestamp = DateTime.MinValue;
                }
            }
            else if (token.Type == JTokenType.String)
            {
                // The string format should be: "DD-MM-YYYYTHH:MM:SS.mmmZ" (UTC time) or "DD-MM-YYYYTHH:MM:SS.mmm" (local time).
                // Parse it to set the value of the DateTime object
                string auxString = token.ToString();
                string[] timestampElements = auxString.Split(new char[] { '-', 'T', ':', '.', 'Z' });
                if (timestampElements.GetLength(0) < 7)
                {
                    return (messageTimestamp);
                }

                // Parse the year
                int year = 0;
                if (!int.TryParse(timestampElements[2], out year))
                {
                    return (messageTimestamp);
                }

                // Parse the month
                int month = 0;
                if (!int.TryParse(timestampElements[1], out month))
                {
                    return (messageTimestamp);
                }

                // Parse the day
                int day = 0;
                if (!int.TryParse(timestampElements[0], out day))
                {
                    return (messageTimestamp);
                }

                // Parse the hour
                int hour = 0;
                if (!int.TryParse(timestampElements[3], out hour))
                {
                    return (messageTimestamp);
                }

                // Parse the minute
                int minute = 0;
                if (!int.TryParse(timestampElements[4], out minute))
                {
                    return (messageTimestamp);
                }

                // Parse the second
                int second = 0;
                if (!int.TryParse(timestampElements[5], out second))
                {
                    return (messageTimestamp);
                }

                // Parse the millisecond
                int millisecond = 0;
                if (!int.TryParse(timestampElements[6], out millisecond))
                {
                    return (messageTimestamp);
                }

                // Set the parsed timestamp
                try
                {
                    if (auxString.Contains('Z'))
                    {
                        messageTimestamp = new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc);
                    }
                    else
                    {
                        messageTimestamp = new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Local);
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Exception in GetItITTimestamp (case 2): {0}", e.Message));
                    messageTimestamp = DateTime.MinValue;
                }
            }

            return (messageTimestamp);
        }

        string GetEnUSTimestamp(DateTime timestamp)
        {
            string timestampString = String.Empty;
            if (!JsonUseLocalTime)
            {
                // UTC time: the string format must be "MM-DD-YYYYTHH:MM:SS.mmmZ".
                timestampString = timestamp.ToString("MM-yyyy-ddTHH:mm:ss.fffZ");
            }
            else
            {
                // Local time: the string format must be "MM-DD-YYYYTHH:MM:SS.mmm".
                timestampString = timestamp.ToString("MM-yyyy-ddTHH:mm:ss.fff");
            }
            return (timestampString);
        }

        DateTime GetEnUSTimestamp(JToken token, bool useLocalTime)
        {
            DateTime messageTimestamp = DateTime.MinValue;
            if (token.Type == JTokenType.Date)
            {
                try
                {
                    // If the message element is a Date, convert it to a DateTime object
                    messageTimestamp = (DateTime)token;
                    if (messageTimestamp == null)
                    {
                        messageTimestamp = DateTime.MinValue;
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Exception in GetEnUSTimestamp (case 1): {0}", e.Message));
                    messageTimestamp = DateTime.MinValue;
                }
            }
            else if (token.Type == JTokenType.String)
            {
                // The string format should be: "MM-DD-YYYYTHH:MM:SS.mmmZ" (UTC time) or "MM-DD-YYYYTHH:MM:SS.mmm" (local time).
                // Parse it to set the value of the DateTime object
                string auxString = token.ToString();
                string[] timestampElements = auxString.Split(new char[] { '-', 'T', ':', '.', 'Z' });
                if (timestampElements.GetLength(0) < 7)
                {
                    return (messageTimestamp);
                }

                // Parse the year
                int year = 0;
                if (!int.TryParse(timestampElements[2], out year))
                {
                    return (messageTimestamp);
                }

                // Parse the month
                int month = 0;
                if (!int.TryParse(timestampElements[0], out month))
                {
                    return (messageTimestamp);
                }

                // Parse the day
                int day = 0;
                if (!int.TryParse(timestampElements[1], out day))
                {
                    return (messageTimestamp);
                }

                // Parse the hour
                int hour = 0;
                if (!int.TryParse(timestampElements[3], out hour))
                {
                    return (messageTimestamp);
                }

                // Parse the minute
                int minute = 0;
                if (!int.TryParse(timestampElements[4], out minute))
                {
                    return (messageTimestamp);
                }

                // Parse the second
                int second = 0;
                if (!int.TryParse(timestampElements[5], out second))
                {
                    return (messageTimestamp);
                }

                // Parse the milliscondsecond
                int millisecond = 0;
                if (!int.TryParse(timestampElements[6], out millisecond))
                {
                    return (messageTimestamp);
                }

                // Set the parsed timestamp
                try
                {
                    if(auxString.Contains('Z'))
                    {
                        messageTimestamp = new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc);
                    }
                    else
                    {
                        messageTimestamp = new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Local);
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Exception in GetEnUSTimestamp (case 2): {0}", e.Message));
                    messageTimestamp = DateTime.MinValue;
                }
            }

            return (messageTimestamp);
        }

        string GetISOTimestamp(DateTime timestamp)
        {
            string timestampString = String.Empty;
            if(!JsonUseLocalTime)
            {
                // UTC time: the string format must be "YYYY-MM-DDTHH:MM:SS.mmmZ".
                timestampString = timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            }
            else
            {
                // Local time: the string format must be "YYYY-MM-DDTHH:MM:SS.mmm".
                timestampString = timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fff");
            }
            return (timestampString);
        }

        DateTime GetISOTimestamp(JToken token)
        {
            DateTime messageTimestamp = DateTime.MinValue;
            if(token.Type == JTokenType.Date)
            {
                try
                {
                    // If the message element is a Date, convert it to a DateTime object
                    messageTimestamp = (DateTime)token;
                    if (messageTimestamp == null)
                    {
                        messageTimestamp = DateTime.MinValue;
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Exception in GetISOTimestamp (case 1): {0}", e.Message));
                    messageTimestamp = DateTime.MinValue;
                }
            }
            else if(token.Type == JTokenType.String)
            {
                // The string format should be: "YYYY-MM-DDTHH:MM:SS.mmmZ" (UTC time) or "YYYY-MM-DDTHH:MM:SS.mmm" (local time).
                // Parse it to set the value of the DateTime object
                string auxString = token.ToString();
                string[] timestampElements = auxString.Split(new char[] {'-', 'T', ':', '.', 'Z'});
                if(timestampElements.GetLength(0) < 7)
                {
                    return (messageTimestamp);
                }

                // Parse the year
                int year = 0;
                if(!int.TryParse(timestampElements[0], out year))
                {
                    return (messageTimestamp);
                }

                // Parse the month
                int month = 0;
                if (!int.TryParse(timestampElements[1], out month))
                {
                    return (messageTimestamp);
                }

                // Parse the day
                int day = 0;
                if (!int.TryParse(timestampElements[2], out day))
                {
                    return (messageTimestamp);
                }

                // Parse the hour
                int hour = 0;
                if (!int.TryParse(timestampElements[3], out hour))
                {
                    return (messageTimestamp);
                }

                // Parse the minute
                int minute = 0;
                if (!int.TryParse(timestampElements[4], out minute))
                {
                    return (messageTimestamp);
                }

                // Parse the second
                int second = 0;
                if (!int.TryParse(timestampElements[5], out second))
                {
                    return (messageTimestamp);
                }

                // Parse the milliscondsecond
                int millisecond = 0;
                if (!int.TryParse(timestampElements[6], out millisecond))
                {
                    return (messageTimestamp);
                }

                // Set the parsed timestamp
                try
                {
                    if(auxString.Contains('Z'))
                    {
                        messageTimestamp = new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc);
                    }
                    else
                    {
                        messageTimestamp = new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Local);
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Exception in GetISOTimestamp (case 2): {0}", e.Message));
                    messageTimestamp = DateTime.MinValue;
                }
            }

            return (messageTimestamp);
        }

        DateTime ParseJsonMessageTimestamp(string receivedValue, string timestampField, JSonTimestampFormats timestampFormat, bool useLocalTime)
        {
            DateTime messageTimestamp = DateTime.MinValue;
            // Check the parameters
            if(String.IsNullOrWhiteSpace(receivedValue) || String.IsNullOrWhiteSpace(timestampField))
            {
                return (messageTimestamp);
            }
            try
            {
                string jsonMessage = receivedValue;
                JObject parsedObject = JObject.Parse(jsonMessage);
                JToken token = GetJTokenFromJObject(ref parsedObject, timestampField);
                if (token == null)
                {
                    System.Diagnostics.Debug.WriteLine("ParseJsonMessageTimestamp - GetJTokenFromJObject retuned null");
                    messageTimestamp = DateTime.MinValue;
                    return (messageTimestamp);
                }

                switch(timestampFormat)
                {
                    case JSonTimestampFormats.tf_ISO:
                        messageTimestamp = GetISOTimestamp(token);
                        break;
                    case JSonTimestampFormats.tf_en_US:
                        messageTimestamp = GetEnUSTimestamp(token, useLocalTime);
                        break;
                    case JSonTimestampFormats.tf_it_IT:
                        messageTimestamp = GetItITTimestamp(token, useLocalTime);
                        break;
                    case JSonTimestampFormats.tf_EPOCH:
                        messageTimestamp = GetEpochTimestamp(token);
                        break;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("Exception in ParseJsonMessageTimestamp: {0}", e.Message));
                messageTimestamp = DateTime.MinValue;
            }

            return (messageTimestamp);
        }

        DataValue ParseJsonMessage(string receivedValue, Tag searchTag)
        {
            // Check the tag object
            MQTTClientTag mqttTag = (MQTTClientTag)searchTag;
            if((mqttTag == null) || (mqttTag.TagNode.DataType.IdType != IdType.Numeric))
            {
                System.Diagnostics.Debug.WriteLine("JSON DBG - ParseJsonMessage Return 1");
                return (null);
            }

            // Set the message field to be parsed (stored in the tag or in the job)
            string jsonMessageSearchField = mqttTag.JsonMessageFormat;
            if (String.IsNullOrWhiteSpace(jsonMessageSearchField))
            {
                jsonMessageSearchField = _JsonMessageFormat;
                if (String.IsNullOrWhiteSpace(jsonMessageSearchField))
                {
                    System.Diagnostics.Debug.WriteLine("JSON DBG - ParseJsonMessage Return 2");
                    return (null);
                }
            }

            DataValue receivedData = null;
            try
            {
                string jsonMessage = receivedValue;
                JObject parsedObject = JObject.Parse(jsonMessage);
                uint nType = (uint)mqttTag.TagNode.DataType.Identifier;
                switch (nType)
                {
                    case (uint)Opc.Ua.DataTypes.String:
                        {
                            receivedData = new DataValue();
                            if (mqttTag.TagNode.ArrayDimension == 0)
                            {
                                JToken token = GetJTokenFromJObject(ref parsedObject, jsonMessageSearchField);
                                if (token == null)
                                {
                                    System.Diagnostics.Debug.WriteLine("JSON DBG - ParseJsonMessage Return 3");
                                    return (null);
                                }
                                receivedData.Value = (string)token;
                            }
                            else
                            {
                                string[] auxValue = new string[mqttTag.TagNode.ArrayDimension];
                                for (uint i = 0; i < mqttTag.TagNode.ArrayDimension; i++)
                                {
                                    string tokenIndexString = String.Format("{0}[{1}]", jsonMessageSearchField, i);
                                    JToken token = GetJTokenFromJObject(ref parsedObject, tokenIndexString);
                                    if (token == null)
                                    {
                                        return (null);
                                    }
                                    auxValue[i] = (string)token;
                                }
                                receivedData.Value = auxValue;
                            }
                            receivedData.StatusCode = StatusCodes.Good;
                            receivedData.SourceTimestamp = DateTime.UtcNow;
                            receivedData.ServerTimestamp = receivedData.SourceTimestamp;
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.Boolean:
                        {
                            receivedData = new DataValue();
                            if (mqttTag.TagNode.ArrayDimension == 0)
                            {
                                JToken token = GetJTokenFromJObject(ref parsedObject, jsonMessageSearchField);
                                if (token == null)
                                {
                                    return (null);
                                }
                                receivedData.Value = (bool)token;
                            }
                            else
                            {
                                bool[] auxValue = new bool[mqttTag.TagNode.ArrayDimension];
                                for (uint i = 0; i < mqttTag.TagNode.ArrayDimension; i++)
                                {
                                    string tokenIndexString = String.Format("{0}[{1}]", jsonMessageSearchField, i);
                                    JToken token = GetJTokenFromJObject(ref parsedObject, tokenIndexString);
                                    if (token == null)
                                    {
                                        return (null);
                                    }
                                    auxValue[i] = (bool)token;
                                }
                                receivedData.Value = auxValue;
                            }
                            receivedData.StatusCode = StatusCodes.Good;
                            receivedData.SourceTimestamp = DateTime.UtcNow;
                            receivedData.ServerTimestamp = receivedData.SourceTimestamp;
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.SByte:
                        {
                            receivedData = new DataValue();
                            if (mqttTag.TagNode.ArrayDimension == 0)
                            {
                                JToken token = GetJTokenFromJObject(ref parsedObject, jsonMessageSearchField);
                                if (token == null)
                                {
                                    return (null);
                                }
                                receivedData.Value = (sbyte)token;
                            }
                            else
                            {
                                sbyte[] auxValue = new sbyte[mqttTag.TagNode.ArrayDimension];
                                for (uint i = 0; i < mqttTag.TagNode.ArrayDimension; i++)
                                {
                                    string tokenIndexString = String.Format("{0}[{1}]", jsonMessageSearchField, i);
                                    JToken token = GetJTokenFromJObject(ref parsedObject, tokenIndexString);
                                    if (token == null)
                                    {
                                        return (null);
                                    }
                                    auxValue[i] = (sbyte)token;
                                }
                                receivedData.Value = auxValue;
                            }
                            receivedData.StatusCode = StatusCodes.Good;
                            receivedData.SourceTimestamp = DateTime.UtcNow;
                            receivedData.ServerTimestamp = receivedData.SourceTimestamp;
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.Byte:
                        {
                            receivedData = new DataValue();
                            if (mqttTag.TagNode.ArrayDimension == 0)
                            {
                                JToken token = GetJTokenFromJObject(ref parsedObject, jsonMessageSearchField);
                                if (token == null)
                                {
                                    return (null);
                                }
                                receivedData.Value = (byte)token;
                            }
                            else
                            {
                                byte[] auxValue = new byte[mqttTag.TagNode.ArrayDimension];
                                for (uint i = 0; i < mqttTag.TagNode.ArrayDimension; i++)
                                {
                                    string tokenIndexString = String.Format("{0}[{1}]", jsonMessageSearchField, i);
                                    JToken token = GetJTokenFromJObject(ref parsedObject, tokenIndexString);
                                    if (token == null)
                                    {
                                        return (null);
                                    }
                                    auxValue[i] = (byte)token;
                                }
                                receivedData.Value = auxValue;
                            }
                            receivedData.StatusCode = StatusCodes.Good;
                            receivedData.SourceTimestamp = DateTime.UtcNow;
                            receivedData.ServerTimestamp = receivedData.SourceTimestamp;
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.Int16:
                        {
                            receivedData = new DataValue();
                            if (mqttTag.TagNode.ArrayDimension == 0)
                            {
                                JToken token = GetJTokenFromJObject(ref parsedObject, jsonMessageSearchField);
                                if (token == null)
                                {
                                    return (null);
                                }
                                receivedData.Value = (short)token;
                            }
                            else
                            {
                                Int16[] auxValue = new Int16[mqttTag.TagNode.ArrayDimension];
                                for (uint i = 0; i < mqttTag.TagNode.ArrayDimension; i++)
                                {
                                    string tokenIndexString = String.Format("{0}[{1}]", jsonMessageSearchField, i);
                                    JToken token = GetJTokenFromJObject(ref parsedObject, tokenIndexString);
                                    if (token == null)
                                    {
                                        return (null);
                                    }
                                    auxValue[i] = (short)token;
                                }
                                receivedData.Value = auxValue;
                            }
                            receivedData.StatusCode = StatusCodes.Good;
                            receivedData.SourceTimestamp = DateTime.UtcNow;
                            receivedData.ServerTimestamp = receivedData.SourceTimestamp;
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.UInt16:
                        {
                            receivedData = new DataValue();
                            if (mqttTag.TagNode.ArrayDimension == 0)
                            {
                                JToken token = GetJTokenFromJObject(ref parsedObject, jsonMessageSearchField);
                                if (token == null)
                                {
                                    return (null);
                                }
                                receivedData.Value = (ushort)token;
                            }
                            else
                            {
                                UInt16[] auxValue = new UInt16[mqttTag.TagNode.ArrayDimension];
                                for (uint i = 0; i < mqttTag.TagNode.ArrayDimension; i++)
                                {
                                    string tokenIndexString = String.Format("{0}[{1}]", jsonMessageSearchField, i);
                                    JToken token = GetJTokenFromJObject(ref parsedObject, tokenIndexString);
                                    if (token == null)
                                    {
                                        return (null);
                                    }
                                    auxValue[i] = (ushort)token;
                                }
                                receivedData.Value = auxValue;
                            }
                            receivedData.StatusCode = StatusCodes.Good;
                            receivedData.SourceTimestamp = DateTime.UtcNow;
                            receivedData.ServerTimestamp = receivedData.SourceTimestamp;
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.Int32:
                        {
                            receivedData = new DataValue();
                            if (mqttTag.TagNode.ArrayDimension == 0)
                            {
                                JToken token = GetJTokenFromJObject(ref parsedObject, jsonMessageSearchField);
                                if (token == null)
                                {
                                    return (null);
                                }
                                receivedData.Value = (int)token;
                            }
                            else
                            {
                                Int32[] auxValue = new Int32[mqttTag.TagNode.ArrayDimension];
                                for (uint i = 0; i < mqttTag.TagNode.ArrayDimension; i++)
                                {
                                    string tokenIndexString = String.Format("{0}[{1}]", jsonMessageSearchField, i);
                                    JToken token = GetJTokenFromJObject(ref parsedObject, tokenIndexString);
                                    if (token == null)
                                    {
                                        return (null);
                                    }
                                    auxValue[i] = (int)token;
                                }
                                receivedData.Value = auxValue;
                            }
                            receivedData.StatusCode = StatusCodes.Good;
                            receivedData.SourceTimestamp = DateTime.UtcNow;
                            receivedData.ServerTimestamp = receivedData.SourceTimestamp;
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.UInt32:
                        {
                            receivedData = new DataValue();
                            if (mqttTag.TagNode.ArrayDimension == 0)
                            {
                                JToken token = GetJTokenFromJObject(ref parsedObject, jsonMessageSearchField);
                                if (token == null)
                                {
                                    return (null);
                                }
                                receivedData.Value = (uint)token;
                            }
                            else
                            {
                                UInt32[] auxValue = new UInt32[mqttTag.TagNode.ArrayDimension];
                                for (uint i = 0; i < mqttTag.TagNode.ArrayDimension; i++)
                                {
                                    string tokenIndexString = String.Format("{0}[{1}]", jsonMessageSearchField, i);
                                    JToken token = GetJTokenFromJObject(ref parsedObject, tokenIndexString);
                                    if (token == null)
                                    {
                                        return (null);
                                    }
                                    auxValue[i] = (uint)token;
                                }
                                receivedData.Value = auxValue;
                            }
                            receivedData.StatusCode = StatusCodes.Good;
                            receivedData.SourceTimestamp = DateTime.UtcNow;
                            receivedData.ServerTimestamp = receivedData.SourceTimestamp;
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.Int64:
                        {
                            receivedData = new DataValue();
                            if (mqttTag.TagNode.ArrayDimension == 0)
                            {
                                JToken token = GetJTokenFromJObject(ref parsedObject, jsonMessageSearchField);
                                if (token == null)
                                {
                                    return (null);
                                }
                                receivedData.Value = (long)token;
                            }
                            else
                            {
                                Int64[] auxValue = new Int64[mqttTag.TagNode.ArrayDimension];
                                for (uint i = 0; i < mqttTag.TagNode.ArrayDimension; i++)
                                {
                                    string tokenIndexString = String.Format("{0}[{1}]", jsonMessageSearchField, i);
                                    JToken token = GetJTokenFromJObject(ref parsedObject, tokenIndexString);
                                    if (token == null)
                                    {
                                        return (null);
                                    }
                                    auxValue[i] = (long)token;
                                }
                                receivedData.Value = auxValue;
                            }
                            receivedData.StatusCode = StatusCodes.Good;
                            receivedData.SourceTimestamp = DateTime.UtcNow;
                            receivedData.ServerTimestamp = receivedData.SourceTimestamp;
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.UInt64:
                        {
                            receivedData = new DataValue();
                            if (mqttTag.TagNode.ArrayDimension == 0)
                            {
                                JToken token = GetJTokenFromJObject(ref parsedObject, jsonMessageSearchField);
                                if (token == null)
                                {
                                    return (null);
                                }
                                receivedData.Value = (ulong)token;
                            }
                            else
                            {
                                UInt64[] auxValue = new UInt64[mqttTag.TagNode.ArrayDimension];
                                for (uint i = 0; i < mqttTag.TagNode.ArrayDimension; i++)
                                {
                                    string tokenIndexString = String.Format("{0}[{1}]", jsonMessageSearchField, i);
                                    JToken token = GetJTokenFromJObject(ref parsedObject, tokenIndexString);
                                    if (token == null)
                                    {
                                        return (null);
                                    }
                                    auxValue[i] = (ulong)token;
                                }
                                receivedData.Value = auxValue;
                            }
                            receivedData.StatusCode = StatusCodes.Good;
                            receivedData.SourceTimestamp = DateTime.UtcNow;
                            receivedData.ServerTimestamp = receivedData.SourceTimestamp;
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.Float:
                        {
                            receivedData = new DataValue();
                            if (mqttTag.TagNode.ArrayDimension == 0)
                            {
                                JToken token = GetJTokenFromJObject(ref parsedObject, jsonMessageSearchField);
                                if (token == null)
                                {
                                    return (null);
                                }
                                receivedData.Value = (float)token;
                            }
                            else
                            {
                                float[] auxValue = new float[mqttTag.TagNode.ArrayDimension];
                                for (uint i = 0; i < mqttTag.TagNode.ArrayDimension; i++)
                                {
                                    string tokenIndexString = String.Format("{0}[{1}]", jsonMessageSearchField, i);
                                    JToken token = GetJTokenFromJObject(ref parsedObject, tokenIndexString);
                                    if (token == null)
                                    {
                                        return (null);
                                    }
                                    auxValue[i] = (float)token;
                                }
                                receivedData.Value = auxValue;
                            }
                            receivedData.StatusCode = StatusCodes.Good;
                            receivedData.SourceTimestamp = DateTime.UtcNow;
                            receivedData.ServerTimestamp = receivedData.SourceTimestamp;
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.Double:
                        {
                            receivedData = new DataValue();
                            if (mqttTag.TagNode.ArrayDimension == 0)
                            {
                                JToken token = GetJTokenFromJObject(ref parsedObject, jsonMessageSearchField);
                                if (token == null)
                                {
                                    return (null);
                                }
                                receivedData.Value = (double)token;
                            }
                            else
                            {
                                double[] auxValue = new double[mqttTag.TagNode.ArrayDimension];
                                for (uint i = 0; i < mqttTag.TagNode.ArrayDimension; i++)
                                {
                                    string tokenIndexString = String.Format("{0}[{1}]", jsonMessageSearchField, i);
                                    JToken token = GetJTokenFromJObject(ref parsedObject, tokenIndexString);
                                    if (token == null)
                                    {
                                        return (null);
                                    }
                                    auxValue[i] = (double)token;
                                }
                                receivedData.Value = auxValue;
                            }
                            receivedData.StatusCode = StatusCodes.Good;
                            receivedData.SourceTimestamp = DateTime.UtcNow;
                            receivedData.ServerTimestamp = receivedData.SourceTimestamp;
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("Exception in ParseJsonMessage: {0}", e.Message));
                receivedData = null;
            }

            return (receivedData);
        }

        public override void SetJobData(object jobData, ref List<Tag> changed)
        {
            byte[] rec = jobData as byte[];
            if (rec == null)
                return;

            base.SetJobData(rec, ref changed);
            lock (lockListObject)
            {
                UInt16 sizeProtocolData = (UInt16)GetProtocolDataByteSize();
                for (int TagIndex = 0; TagIndex < TagsList.Count; TagIndex++)
                {
                    MQTTClientTag MQTTClientTag = (MQTTClientTag)TagsList[TagIndex];
                    if (MQTTClientTag.TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                    {
                        if (isProtocolBool())
                        {
                            if (MQTTClientTag.SetTagValue(ref rec, (int)MQTTClientTag.ByteOffset))
                                changed.Add(MQTTClientTag);
                        }
                        else
                        {
                            uint ArraySize = MQTTClientTag.TagNode.ArrayDimension;
                            if (ArraySize == 0)
                                ArraySize = 1;
                            byte[] tmpData = new byte[ArraySize];
                            MQTTClientTag.setMemRW(rec, (int)MQTTClientTag.ByteOffset, (int)(sizeProtocolData * ArraySize));
                            for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                            {
                                bool valBool = MQTTClientTag.getBoolValueFromMemRW((int)MQTTClientTag.ByteOffset + sizeProtocolData * ArrayIndex, ElementNumber);
                                tmpData[ArrayIndex] = (byte)(valBool ? 1 : 0);
                            }

                            if (MQTTClientTag.SetTagValue(ref tmpData, 0))
                                changed.Add(MQTTClientTag);
                        }
                    }
                    else
                    {
                        if (isProtocolBool())
                        {
                            if (MQTTClientTag.SetTagValue(ref rec, (int)MQTTClientTag.ByteOffset, (uint)(ElementNumber > 0 ? 1 : 0)))
                                changed.Add(MQTTClientTag);
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
                                if (MQTTClientTag.SetTagValue(ref rec, (int)MQTTClientTag.ByteOffset, elemsize))
                                    changed.Add(MQTTClientTag);
                            }
                            else
                            {
                                UInt16 sizeTmpData = (UInt16)GetDataTypeByteSize((uint)MQTTClientTag.TagNode.DataType.Identifier);
                                uint ArraySize = MQTTClientTag.TagNode.ArrayDimension;
                                if (ArraySize == 0)
                                    ArraySize = 1;
                                byte[] tmpData = new byte[sizeTmpData * ArraySize];
                                int indexTmpData = 0;
                                MQTTClientTag.setMemRW(rec, (int)MQTTClientTag.ByteOffset, (int)(sizeProtocolData * ArraySize));
                                for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                                {
                                    Array.Copy(rec, MQTTClientTag.ByteOffset + sizeProtocolData * ArrayIndex + ElementNumber * sizeTmpData, tmpData, indexTmpData, sizeTmpData);
                                    indexTmpData += sizeTmpData;
                                }

                                if (MQTTClientTag.SetTagValue(ref tmpData, 0))
                                    changed.Add(MQTTClientTag);
                            }
                        }
                    }
                }

                FirstTime = false;
            }
        }

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

        #endregion

        #region Properties

        /// <summary>
        /// Topic
        /// </summary>
        private string _TagName;
        public string TagName
        {
            get
            {
                return _TagName;
            }

            set
            {
                _TagName = value;
            }
        }

        /// <summary> Retain flag for published values. </summary>
        private bool _Retained;
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a value indicating whether the broker must retain the last value of the published tag. </summary>
        ///
        /// <value> true if the value must be retained by the broker, false if not. </value>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool Retained
        {
            get { return _Retained; }
            set
            {
                _Retained = value;
            }
        }

        /// <summary> Quality Of Service (QOS) level for PUBLISH and SUBSCRIBE messages. </summary>
        private QualityOfServiceLevels _QualityOfServiceLevel;
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a value indicating whether the broker must retain the last value of the published tag. </summary>
        ///
        /// <value> true if the value must be retained by the broker, false if not. </value>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public QualityOfServiceLevels QualityOfServiceLevel
        {
            get { return _QualityOfServiceLevel; }
            set
            {
                _QualityOfServiceLevel = value;
            }
        }

        /// <summary>
        /// Status
        /// </summary>
        private MQTTClientCommJobStatus _Status;
        public MQTTClientCommJobStatus Status
        {
            get
            {
                return _Status;
            }

            set
            {
                _Status = value;
            }
        }

        /// <summary>
        /// Format of Json messages
        /// </summary>
        private string _JsonMessageFormat;
        public string JsonMessageFormat
        {
            get { return _JsonMessageFormat; }
            set { _JsonMessageFormat = value; }
        }

        /// <summary>
        /// Timestamp field in the Json message
        /// </summary>
        private string _JsonMessageTimestampField;
        public string JsonMessageTimestampField
        {
            get { return _JsonMessageTimestampField; }
            set { _JsonMessageTimestampField = value; }
        }

        /// <summary> Time format for the timestamp of the JSon message. </summary>
        private JSonTimestampFormats _JsonTimestampFormat;
        public JSonTimestampFormats JsonTimestampFormat
        {
            get { return _JsonTimestampFormat; }
            set
            {
                _JsonTimestampFormat = value;
            }
        }

        /// <summary> "Use Local Time" flag for the timestamp information of a Json message. </summary>
        private bool _JsonUseLocalTime;
        public bool JsonUseLocalTime
        {
            get { return _JsonUseLocalTime; }
            set
            {
                _JsonUseLocalTime = value;
            }
        }

        /// <summary>
        /// Hysteresis Threshold
        /// </summary>
        private double _HysteresisThreshold;
        public double HysteresisThreshold
        {
            get { return _HysteresisThreshold; }
            set
            {
                _HysteresisThreshold = value;
            }
        }

        #endregion
    }
}
