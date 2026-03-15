using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;

namespace BACnet
{
    public partial class BACnetChannel : BacknetUdpChannel
    {
        public const int MAX_RETRY_REQUEST = 3;
        public const int APDU_TYPE_PDU_FLAG = 6;
        public const int SERVICE_CHOICE = 11;
        public const int WINDOWS_SIZE = 9;

        public bool RequestObjectList = false;

        private System.Timers.Timer aTimer;
        private void SetTimer()
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(this.Timeout);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;

        }

        private  void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            message item = new message();
            item.command = BACnetStateRequestImport.TimeOut;
            AddedQueueOfCommandsToExecute(item);
        }
        private void BACnetChannel_ObjectListImport(object sender, NewDataReceivedArgs e)
        {
            e.SetNewDataEvent = false;
            
            lock (lockThreadObject)
            {
                message item = new message();

                BACnetProtocol.BACnetParseDataImportVariable(e.Sender, e.RxBytes, e.Timestamp, AllowedIp, out ReceiveItem receiveItem);            
                
                if (receiveItem == null || !receiveItem.isValid) //  && !IPFrameValidator.isErrorFrame(receiveItem.Frame)
                {
                    item.command = BACnetStateRequestImport.End;
                    AddedQueueOfCommandsToExecute(item);
                    return;
                }

                #region Cov Notification Management
                if (IPFrameValidator.isUnconfirmedCovNotification(receiveItem.Frame))
                {
                    return;
                }
                #endregion

                #region special case
                if (deviceServer != null)
                {
                    //Device Server --> answer to WhoIs message with IAM (driver answer like a BACNet device)
                    if (IPFrameValidator.isUnConfirmedWhoIsRequest(receiveItem))
                    {
                        // Is WhoIs direct to local PC station ?
                        if (!receiveItem.ComingFromEndPoint(deviceServer.LocalEndPoint))
                        {
                            // manage here unconfirmed cov notification
                            if (!deviceServer.SendiAm(receiveItem.RemoteEndPoint))
                                CommDriver.OnSystemEvent(ObjectIds.Server, Properties.Resources.ErrorSendiAm, Opc.Ua.EventSeverity.Low);
                            item.command = BACnetStateRequestImport.End;
                            AddedQueueOfCommandsToExecute(item);
                            return;
                        }
                    }
                }
                #endregion

                if (RequestObjectList)
                {
                    if (IPFrameValidator.IAm(receiveItem.Frame, out IAmAnswer iAmAnswer))
                    {
                        BACnetDriver.Addinfo("Instance found : " + iAmAnswer.DeviceIdentifier.instance);
                    }
                }

                if (!receiveItem.ComingFromEndPoint((strionforImportVariable).RemoteEndPoint) && !receiveItem.ComingFromEndPoint((strionforImportVariable).BBMDEndPoint))
                    return;

                if (IPFrameValidator.isErrorFrame(receiveItem.Frame) || IPFrameValidator.isRejectFrame(receiveItem.Frame))
                {
                    item.command = BACnetStateRequestImport.End;
                    AddedQueueOfCommandsToExecute(item);
                    //UnSetActiveJob(receiveItem);
                    e.SetNewDataEvent = true;
                    return;
                }
                                
                item.InvokeId = receiveItem.Frame.Npdu.Apdu.InvokeId;
                item.SequenceNumber = receiveItem.Frame.Npdu.Apdu.SequenceNumber;
                item.WindowSize = receiveItem.Frame.Npdu.Apdu.WindowSize;
                switch (BacnetStateLocal)
                {
                    case BACnetStateRequestImport.None:
                        // discard data
                        break;

                    //case BACnetState.BBMDWhoIs:
                    //    break;
                    case BACnetStateRequestImport.BBMDInitialization:
                        switch ((strionforImportVariable).BBDMDevice.BacnetState)
                        {

                            case BACnetBBMDDevice.BACnetState.RegisterAsForeignDevice:
                            case BACnetBBMDDevice.BACnetState.RefreshRegistration:
                                if (IPFrameValidator.isconfirmedForeignDeviceRegistration(receiveItem))
                                {
                                    //UnSetActiveJob(receiveItem);
                                    e.SetNewDataEvent = true;
                                }
                                aTimer.Stop();
                                break;
                        }                        
                        break;

                    case BACnetStateRequestImport.WhoIs:

                        if (IPFrameValidator.IAm(receiveItem.Frame, out IAmAnswer iAmAnswer))
                        {
                            // sender match to job's station ?
                            if (((strionforImportVariable).GetDeviceInstance() == iAmAnswer.DeviceIdentifier.instance || (strionforImportVariable).GetDeviceInstance() < 0) && receiveItem.ComingFromEndPoint((strionforImportVariable).RemoteEndPoint))
                            {
                                //UnSetActiveJob(receiveItem);
                                aTimer.Stop();
                                e.SetNewDataEvent = true;
                                strionforImportVariable.BACnetInitDone = true;
                                (strionforImportVariable).RunTimeDeviceInstance = (int)iAmAnswer.DeviceIdentifier.instance;
                                InitStationWithWhoIsData(strionforImportVariable, iAmAnswer.DeviceIdentifier, iAmAnswer.MaxAPDULength, iAmAnswer.VendorIdentifier, iAmAnswer.DestinationSpecifierPresent, iAmAnswer.DNET, iAmAnswer.DLEN, iAmAnswer.DADR);
                                CheckBacNetStateCh(strionforImportVariable);
                                item.command = BacnetStateLocal;
                                AddedQueueOfCommandsToExecute(item);
                                
                            }
                        }
                        break;
                    case BACnetStateRequestImport.WhoHas:

                        if (IPFrameValidator.IHave(receiveItem.Frame, strionforImportVariable.DeviceIdentifier, activeJob.ObjectName, out BACnetObjectIdentifier objectID))
                        {
                            aTimer.Stop();
                            e.SetNewDataEvent = true;
                            CheckBacNetStateCh(strionforImportVariable);
                            item.command = BacnetStateLocal;
                            AddedQueueOfCommandsToExecute(item);
                            
                        }
                        break;
                    case BACnetStateRequestImport.CheckDeviceInfo:
                        {
                            if (((byte)receiveItem.Frame.Npdu.Apdu.ConfirmedServiceChoice == ((byte)BACnetEnums.ConfirmedService.READ_PROP_MULTIPLE)) &&
                                    receiveItem.Frame.Npdu.Apdu.ServiceRequest.List.Count > 1 && receiveItem.Frame.Npdu.Apdu.ServiceRequest.List[1].isTagList)
                            {
                                e.SetNewDataEvent = true;
                                aTimer.Stop();
                            }
                        }
                        break;
                    case BACnetStateRequestImport.GetObjectList:
                        {
                            if ((((byte)receiveItem.Frame.Npdu.Apdu.ConfirmedServiceChoice == ((byte)BACnetEnums.ConfirmedService.READ_PROP_MULTIPLE)) &&
                                    receiveItem.Frame.Npdu.Apdu.ServiceRequest.List.Count > 1 && receiveItem.Frame.Npdu.Apdu.ServiceRequest.List[1].isTagList) ||
                                    receiveItem.Frame.Npdu.Apdu.PduType == BACnetEnums.BACnetPDU.ABORT ||
                                    receiveItem.Frame.Npdu.Apdu.PduType == BACnetEnums.BACnetPDU.REJECT)
                            {
                                aTimer.Stop();
                                item.command = BACnetStateRequestImport.TimeOut;
                                AddedQueueOfCommandsToExecute(item);
                            }
                            else if (receiveItem.Frame.Npdu.Apdu.PduType == BACnetEnums.BACnetPDU.COMPLEX_ACK)
                            {
                                arraysStreamByteImportTags.Add(receiveItem.Frame.Npdu.Apdu.Data.ToArray());                                

                                // all data are contained in 1 message or is the last fragmented message
                                if ((!receiveItem.Frame.Npdu.Apdu.SEG && !receiveItem.Frame.Npdu.Apdu.MORESEG) || (receiveItem.Frame.Npdu.Apdu.SEG && !receiveItem.Frame.Npdu.Apdu.MORESEG))
                                {                                    
                                    item.TagClose = true;
                                    aTimer.Stop();
                                    item.command = BACnetStateRequestImport.ReadProperties;
                                    AddedQueueOfCommandsToExecute(item);
                                }
                                else if (receiveItem.Frame.Npdu.Apdu.SequenceNumber == 0 || receiveItem.Frame.Npdu.Apdu.SequenceNumber == receiveItem.Frame.Npdu.Apdu.WindowSize)
                                {
                                    aTimer.Stop();
                                    item.command = BACnetStateRequestImport.Segment_ACK;
                                    AddedQueueOfCommandsToExecute(item);
                                }                               
                            }
                        }
                        break;
                    case BACnetStateRequestImport.Segment_ACK:
                        {
                            aTimer.Stop();
                            item.command = BACnetStateRequestImport.Object_Name;
                            AddedQueueOfCommandsToExecute(item);
                        }
                        break;
                    case BACnetStateRequestImport.Object_Name:
                        {
                            if (ObjectListRequestQueue != null)
                            {
                                aTimer.Stop();
                                item.command = BACnetStateRequestImport.End;
                                AddedQueueOfCommandsToExecute(item);
                            }
                            if (PropertyListRequestQueue != null)
                            {
                                PropertyListRequestQueueRawData.AddRange(receiveItem.Frame.Npdu.Apdu.Data);
                                if (receiveItem.Frame.Npdu.Apdu.SEG) 
                                {
                                    if (receiveItem.Frame.Npdu.Apdu.SequenceNumber == 0 || receiveItem.Frame.Npdu.Apdu.SequenceNumber == receiveItem.Frame.Npdu.Apdu.WindowSize)
                                    {
                                        aTimer.Stop();
                                        item.command = BACnetStateRequestImport.Object_Name_Segment_ACK;
                                        AddedQueueOfCommandsToExecute(item);
                                        if (receiveItem.Frame.Npdu.Apdu.MORESEG)
                                            break;
                                    }

                                    if (!receiveItem.Frame.Npdu.Apdu.MORESEG)
                                    {
                                        aTimer.Stop();
                                        item.command = BACnetStateRequestImport.Object_Name_Segment_ACK;
                                        AddedQueueOfCommandsToExecute(item);

                                        if (DecodeReadPropertyMultiple(PropertyListRequestQueueRawData.ToArray()) < 0)
                                        {
                                            PropertyListRequestQueueRawData.Clear();
                                            aTimer.Stop();
                                            item.command = BACnetStateRequestImport.End;
                                            AddedQueueOfCommandsToExecute(item);
                                        }
                                        else
                                        {
                                            PropertyListRequestQueueRawData.Clear();
                                            aTimer.Stop();
                                            item.command = BACnetStateRequestImport.Next;
                                            AddedQueueOfCommandsToExecute(item);
                                        }
                                    }

                                    break;
                                }
                                    
                                if (DecodeReadPropertyMultiple(PropertyListRequestQueueRawData.ToArray()) < 0)
                                {
                                    PropertyListRequestQueueRawData.Clear();

                                    aTimer.Stop();
                                    item.command = BACnetStateRequestImport.End;
                                    AddedQueueOfCommandsToExecute(item);
                                }
                                else
                                {
                                    PropertyListRequestQueueRawData.Clear();

                                    aTimer.Stop();
                                    item.command = BACnetStateRequestImport.Next;
                                    AddedQueueOfCommandsToExecute(item);
                                }
                            }
                            else
                            {
                                PropertyListRequestQueueRawData.Clear();

                                aTimer.Stop();
                                item.command = BACnetStateRequestImport.End;
                                AddedQueueOfCommandsToExecute(item);
                            }
                        }
                        break;

                    case BACnetStateRequestImport.TimeOut:
                    default:
                        aTimer.Stop();
                        item.command = BACnetStateRequestImport.End;
                        AddedQueueOfCommandsToExecute(item);
                        break;
                }
            }
        }

        public void AddedQueueOfCommandsToExecute(message command)
        {
            if (ObjectListRequestQueue != null)
                ObjectListRequestQueue.Add(command);
            if (PropertyListRequestQueue != null)
            {
                message item = new message();
                item.command = command.command;
                item.InvokeId = command.InvokeId;
                item.SequenceNumber = command.SequenceNumber;
                item.WindowSize = command.WindowSize;
                PropertyListRequestQueue.Add(item);
            }
        }

        #region Import Tags from Device


        public enum BACnetStateRequestImport : int
        {
            None = 0,
            BBMDInitialization,
            WhoIs,
            WhoHas,
            Cov,
            CovSubscribedPolling,
            CovSubscribed,
            Polling,
            CheckDeviceInfo,
            GetObjectList,
            Segment_ACK,            
            End_Segment_ACK,
            ReadProperties,
            Object_Name,
            Object_Name_Segment_ACK,
            Next,
            End,
            TimeOut, // variable don't exist into device

        }

        public BACnetStateRequestImport BacnetStateLocal = BACnetStateRequestImport.None;
        public class message
        {
            public BACnetStateRequestImport command;
            public byte InvokeId = 0;
            public byte SequenceNumber = 0;
            public byte WindowSize = 0;
            public bool TagClose = false;
            public message()
            {
                command = BACnetStateRequestImport.None;

            }
        };
        public class ParserMessageRespnce
        {
            public byte version = 0;
            public byte Control = 0;
            public byte complexACK = 0;
            public byte apduType = 0;
            public bool SEGmentRequest = false;
            public bool MOReSegmentFollow = false;
            public byte invokeId = 0;
            public byte sequenceNumber = 0;
            public byte ProposedWindowSize = 0;
            public byte readProperty = 0;
            public int lenghtStream = 0;
            public byte[] stream = null;
            public byte value = 0;
            public byte tagClass = 0;
            public byte contextTagnumber = 0;
            public byte nameTag = 0;
            public ParserMessageRespnce() { }

        }
        byte InvokeId = 0;
        public BACnetStation strionforImportVariable = null;

        public List<byte[]> arraysStreamByteImportTags = new List<byte[]>();
        public bool IS_CLOSING_TAG(byte x)
        {
            return ((x & 0x07) == 7);
        }
        List<List<BufferExpand.BacnetReadAccessResult>> itemList = new List<List<BufferExpand.BacnetReadAccessResult>>();        
        private BlockingCollection<message> ObjectListRequestQueue = null;
        private BlockingCollection<message> PropertyListRequestQueue = null;
        private List<byte> PropertyListRequestQueueRawData = null;

        public int GetObjectList(ref List<List<BufferExpand.BacnetReadAccessResult>> ListObjectType, BACnetStation Station, string conn)
        {
            SetTimer();
            if (arraysStreamByteImportTags != null)
            {
                arraysStreamByteImportTags.Clear();
            }
            int error = 0;
            message item = new message();
            ObjectListRequestQueue = new BlockingCollection<message>();
            strionforImportVariable = Station;
            this.RequestObjectList = true;
            bool isStart = this.Startup();
            if (!isStart)
                return (3);


            PropertyListRequestQueueRawData = new List<byte>();

            var connDev = CheckDevice(null, this);

            CheckBacNetStateCh(Station);
            item.command = BacnetStateLocal;
            ObjectListRequestQueue.Add(item);
            bool fine = false;
            while (error == 0 &&  !fine)
            {
                //Looking forward to the next assignment
                item = ObjectListRequestQueue.Take();
                switch (item.command)
                {
                    case BACnetStateRequestImport.BBMDInitialization:
                        {

                            this.InitStationWithNoWhoIsData(Station);
                            CheckBacNetStateCh(Station);
                            ObjectListRequestQueue.Add(item);

                        }
                        break;
                    case BACnetStateRequestImport.WhoIs:
                        {
                            TryToAddGoodStationToAllowedIP(Station);
                            if (!SendMessage(IPFrameFactory.WhoIs(Station), Station))
                            {
                                error = (int)(DriverErrorCodes)BACnetErrorCodes.ErrorTxWrite;
                                break;
                            }
                            CheckBacNetStateCh(Station);
                            aTimer.Start();
                        }
                        break;
                    case BACnetStateRequestImport.WhoHas:
                        {
                            if (!SendMessage(IPFrameFactory.WhoHas("0", Station), Station))
                            {
                                error = (int)(DriverErrorCodes)BACnetErrorCodes.ErrorTxWrite;
                                break;
                            }
                            CheckBacNetStateCh(Station);
                            aTimer.Start();
                        }
                        break;
                    case BACnetStateRequestImport.GetObjectList:
                        {
                            //Prepare the list of requests
                            ServiceTagList inTagList = new ServiceTagList();
                            inTagList.Add(new ServiceTag((uint)BACnetEnums.PropertyIdentifier.OBJECT_LIST, 1));
                            //Componed the request
                            BACnetIPFrame Frame = PrepareRequestListObject(ref inTagList, Station);

                            Frame.Npdu.Apdu.InvokeId = item.InvokeId;
                            BacnetStateLocal = BACnetStateRequestImport.GetObjectList;
                            if (!SendMessage(Frame, Station))
                            {
                                error = (int)(DriverErrorCodes)BACnetErrorCodes.ErrorTxWrite;
                                break;
                            }
                        }
                        break;

                    case BACnetStateRequestImport.Segment_ACK:
                        {
                            BACnetIPFrame BACnetIPFrame = new BACnetIPFrame(BACnetEnums.ConfirmedService.READ_PROPERTY);
                            BACnetIPFrame.Npdu.Apdu.PduType = BACnetEnums.BACnetPDU.SEGMENT_ACK;
                            BACnetIPFrame.Npdu.DataExpectingReply = false;
                            BACnetIPFrame.Npdu.Apdu.InvokeId = item.InvokeId;
                            BACnetIPFrame.Npdu.Apdu.SequenceNumber = item.SequenceNumber;
                            BACnetIPFrame.Npdu.Apdu.WindowSize = item.WindowSize;
                            if (!SendMessage(BACnetIPFrame, Station))
                            {
                                error = (int)(DriverErrorCodes)BACnetErrorCodes.ErrorTxWrite;
                                break;
                            }
                            if (!item.TagClose)
                                BacnetStateLocal = BACnetStateRequestImport.GetObjectList;
                            else
                            {
                                item = new message();
                                item.command = BACnetStateRequestImport.End;
                                ObjectListRequestQueue.Add(item);
                            }

                        }
                        break;

                    case BACnetStateRequestImport.ReadProperties:
                        {
                            BACnetIPFrame BACnetIPFrame = new BACnetIPFrame(BACnetEnums.ConfirmedService.READ_PROPERTY);
                            BACnetIPFrame.Npdu.Apdu.PduType = BACnetEnums.BACnetPDU.SEGMENT_ACK;
                            BACnetIPFrame.Npdu.DataExpectingReply = false;
                            BACnetIPFrame.Npdu.Apdu.InvokeId = item.InvokeId;
                            BACnetIPFrame.Npdu.Apdu.SequenceNumber = item.SequenceNumber;
                            BACnetIPFrame.Npdu.Apdu.WindowSize = item.WindowSize;
                            if (!SendMessage(BACnetIPFrame, Station))
                            {
                                error = (int)(DriverErrorCodes)BACnetErrorCodes.ErrorTxWrite;
                                break;
                            }
                            //The timer is started when the SendMessage function is called. In this particular case,
                            //no response is sent from the device, therefore the time stops immediately
                            aTimer.Stop();
                            item = new message();
                            item.command = BACnetStateRequestImport.Object_Name;
                            ObjectListRequestQueue.Add(item);
                        }
                        break;
                        
                    case BACnetStateRequestImport.Object_Name:
                        {
                            // Prepare the list of requests
                            ServiceTagList inTagList = new ServiceTagList();
                            inTagList.List.Clear();
                            inTagList.Add(new ServiceTag((uint)BACnetEnums.PropertyIdentifier.OBJECT_NAME, 1));
                            //Componed the request
                            BACnetIPFrame Frame = PrepareRequestListObject(ref inTagList, Station);
                            if (item.InvokeId != 255)
                                Frame.Npdu.Apdu.InvokeId = (byte)(item.InvokeId++);
                            else
                                Frame.Npdu.Apdu.InvokeId = 0x00;
                            InvokeId = Frame.Npdu.Apdu.InvokeId;
                            BacnetStateLocal = BACnetStateRequestImport.Object_Name;
                            if (!SendMessage(Frame, Station))
                            {
                                error = (int)(DriverErrorCodes)BACnetErrorCodes.ErrorTxWrite;
                                break;
                            }
                        }
                        break;

                    case BACnetStateRequestImport.End:
                        fine = true;
                        break;
                    case BACnetStateRequestImport.TimeOut:
                        error = (int)DriverErrorCodes.ErrorTimeOut;
                        break;

                    default:
                        {
                            BACnetDriver.Addinfo("Error: It was not possible to retrieve the device information!");
                            error = (int)(DriverErrorCodes)BACnetErrorCodes.ErrorInvalidDataType;
                        }
                        break;
                }
            }

            ObjectListRequestQueue.Dispose();
            ObjectListRequestQueue = null;
            if(error != 0)
            {
                arraysStreamByteImportTags.Clear();
                ListObjectType = itemList;
                this.RequestObjectList = false;
                return error;
            }
            //Second step, after receiving the list of objects, request their properties
            if (arraysStreamByteImportTags.Count > 0)
            {
                BufferExpand.StructContextTag structContextTag = new BufferExpand.StructContextTag();
                BufferExpand.StructObjectIdentifier structObjectIdentifier = null;
                List<byte> bytes = new List<byte>();
                //Merge the various byte frames that have been recieved
                foreach (byte[] buffer in arraysStreamByteImportTags)
                {
                    bytes.AddRange(buffer);
                }
                List<BufferExpand.StructObjectIdentifier> _listObjectTypeIstance = new List<BufferExpand.StructObjectIdentifier>();
                //Running the parser, of the bytes that have been sent
                for (int offset = 0; offset < bytes.Count;)
                {
                    //Parser Context tag
                    structContextTag = BufferExpand.GetContextTag(bytes.ToArray(), ref offset);
                    if  ((offset + structContextTag.Length)> bytes.Count())
                        break;
                    //insert all the variables in the list
                    if (structContextTag.Length == 4 && structContextTag.ContexTagNumber == 12)
                    {
                        structObjectIdentifier = new BufferExpand.StructObjectIdentifier();
                        structObjectIdentifier = BufferExpand.GetObjectIdentifier(bytes.ToArray(), ref offset);
                        _listObjectTypeIstance.Add(structObjectIdentifier);
#if DEBUG
                        System.Diagnostics.Debug.WriteLine($" ___@ ObjectType {structObjectIdentifier.ObjectType} IstanceIde {structObjectIdentifier.IstanceNumber}");
#endif
                    }
                    else
                    {
                        offset += structContextTag.Length;
                    }
                }
                bytes.Clear();

                //Now read property for single tag
                error = ReadElementPropertyOfListTag(ref _listObjectTypeIstance, Station, conn);
                _listObjectTypeIstance.Clear();
                ListObjectType = itemList;

            }
            ListObjectType = itemList;
            arraysStreamByteImportTags.Clear();
            this.RequestObjectList = false;
            return error;
        }
        public BACnetIPFrame PrepareRequestListObject(ref ServiceTagList inTagList, BACnetStation Station, BACnetObjectIdentifier identifier = null)
        {
            BACnetIPFrame BACnetIPFrame = null;
            if (inTagList == null || inTagList.Count() == 0)
            {
                return null;
            }

            if ((inTagList.Count()) == 1 && (identifier == null))
            {
                BACnetIPFrame = new BACnetIPFrame(BACnetEnums.ConfirmedService.READ_PROPERTY);
                if (Station.DestinationSpecifierPresent)
                {
                    BACnetIPFrame.Npdu.DestinationSpecifierPresent = Station.DestinationSpecifierPresent;
                    BACnetIPFrame.Npdu.DNET = Station.DNET;
                    BACnetIPFrame.Npdu.DLEN = Station.DLEN;
                    BACnetIPFrame.Npdu.DADR = Station.DADR;
                }
                BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag(Station.DeviceIdentifier, 0));
                BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag(((ServiceTag)inTagList.List[0]).UInt, 1));
                BACnetIPFrame.Npdu.Apdu.SA = true;
                BACnetIPFrame.Npdu.Apdu.MaxSegs = (byte)BACnetEnums.BacnetMaxSegments.MAX_SEG65;
                BACnetIPFrame.Npdu.Apdu.MaxResp = (byte)BACnetEnums.BacnetMaxAdpu.MAX_APDU1476;

            }
            else
            {
                BACnetIPFrame = new BACnetIPFrame(BACnetEnums.ConfirmedService.READ_PROP_MULTIPLE);
                if (Station.DestinationSpecifierPresent)
                {
                    BACnetIPFrame.Npdu.DestinationSpecifierPresent = Station.DestinationSpecifierPresent;
                    BACnetIPFrame.Npdu.DNET = Station.DNET;
                    BACnetIPFrame.Npdu.DLEN = Station.DLEN;
                    BACnetIPFrame.Npdu.DADR = Station.DADR;
                }
                if (identifier == null)
                    BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag(((BACnetStation)Station).DeviceIdentifier, 0));
                else
                    BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag(identifier, 0));
                //add the list to the service tag 
                ServiceTag tag = new ServiceTag((ServiceTagList)inTagList, 1);
                //Added at Service request
                BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(tag);
            }
            return (BACnetIPFrame);
        }

        public bool SendMessage(BACnetIPFrame Frame, BACnetStation s)
        {
            bool error = true;
            if (!DeviceWrite(Frame, s.RemoteEndPoint))
            {
                error = false;
            }
            else
            {
                //
                aTimer.Start();
            }
            return error;
        }

        public void CheckBacNetStateCh(BACnetStation station)
        {
            if (BacnetStateLocal == BACnetStateRequestImport.None)
            {
                if (station.HasBBMDDevice())
                    BacnetStateLocal = BACnetStateRequestImport.BBMDInitialization;
                else
                    BacnetStateLocal = BACnetStateRequestImport.WhoIs;
            }

            if (BacnetStateLocal == BACnetStateRequestImport.BBMDInitialization)
            {
                if (station.BBDMDevice.IsRegistred)
                    BacnetStateLocal = BACnetStateRequestImport.WhoIs;
                else
                    return;  // send BBMD registrazion command
            }

            if (BacnetStateLocal == BACnetStateRequestImport.WhoIs)
            {
                if (station.WhoIsDisabled)
                {
                    if (!station.BACnetInitDone)
                        this.InitStationWithNoWhoIsData(station);

                    //request Object list from device
                    else if (this.RequestObjectList)
                    {
                        BacnetStateLocal = BACnetStateRequestImport.GetObjectList;

                    }
                    else
                    {
                        BacnetStateLocal = BACnetStateRequestImport.WhoHas;
                    }
                }
                else
                {
                    if (station.BACnetInitDone && station.HasDeviceInstance())
                    {
                        //Request object list from device
                        if (this.RequestObjectList)
                        {
                            BacnetStateLocal = BACnetStateRequestImport.GetObjectList;
                        }

                        else
                            BacnetStateLocal = BACnetStateRequestImport.WhoHas;
                    }
                    else
                    {
                        return; // send WhoIs request
                    }
                }
            }
        }

        public int ReadElementPropertyOfListTag(ref List<BufferExpand.StructObjectIdentifier> ListObjectType, BACnetStation Station, string conn)
        {
            int error = 0;
            PropertyListRequestQueue = new BlockingCollection<message>();
            message messag;
            BacnetStateLocal = BACnetStateRequestImport.Object_Name;
            bool exitListObjectType = false;
            bool exitObjectType = false;

            //Loop to read all object information
            foreach (BufferExpand.StructObjectIdentifier item in ListObjectType)
            {
                if (!Enum.IsDefined(typeof(BACnetEnums.ObjectTypes), (int)item.ObjectType))
                {
                    continue;
                }
                error = (int)DriverErrorCodes.ErrorNoError;
                //Prepare the frame to be sent with the object information reques
                ServiceTagList inTagList = new ServiceTagList();
                inTagList.Add(new ServiceTag((uint)BACnetEnums.PropertyIdentifier.ALL, 0));
                BACnetIPFrame Frame = PrepareRequestListObject(ref inTagList, Station, item.Object);

                Frame.Npdu.Apdu.PduType = BACnetEnums.BACnetPDU.CONFIRMED_REQUEST;
                Frame.Npdu.Apdu.InvokeId = ++InvokeId;
                Frame.Npdu.Apdu.SA = true;
                Frame.Npdu.Apdu.MaxSegs = (byte)BACnetEnums.BacnetMaxSegments.MAX_SEG65;
                Frame.Npdu.Apdu.MaxResp = (byte)BACnetEnums.BacnetMaxAdpu.MAX_APDU1476;

                if (!SendMessage(Frame, Station))
                {
                    error = (int)(DriverErrorCodes)BACnetErrorCodes.ErrorTxWrite;
                    break;
                }
#if DEBUG
                System.Diagnostics.Debug.WriteLine($" __@@ Istance number {item.IstanceNumber}");
#endif

                exitObjectType = false;
                while (!exitListObjectType && !exitObjectType)
                { 
                //Await the answer
                    messag = PropertyListRequestQueue.Take();
                    if (messag.command == BACnetStateRequestImport.Next)
                    {
                        aTimer.Stop();
                        exitObjectType = true;
                    }
                    else if (messag.command == BACnetStateRequestImport.End)
                    {
                        //out of the foreeach
                        exitListObjectType = true;
                        break;
                    }
                    else if (messag.command == BACnetStateRequestImport.TimeOut)
                    {
                        // if the request fails, I try again three times before moving on to the next tag.
                        int tryAgain = 0;
                        while (tryAgain < MAX_RETRY_REQUEST)
                        {
                            if (!SendMessage(Frame, Station))
                            {
                                error = (int)(DriverErrorCodes)BACnetErrorCodes.ErrorTxWrite;
                                tryAgain = 3;
                                exitListObjectType = true;
                            }
                            //Await the answer
                            messag = PropertyListRequestQueue.Take();
                            if (messag.command == BACnetStateRequestImport.End)
                            {
                                //out of the foreeach
                                tryAgain = MAX_RETRY_REQUEST;
                                exitListObjectType = true;
                            }
                            else if (messag.command == BACnetStateRequestImport.TimeOut)
                            {
                                tryAgain++;
                            }
                            if (messag.command == BACnetStateRequestImport.Next)
                            {
                                break;
                            }
                        }
                        //Inert messagge of error
                        if (tryAgain >= MAX_RETRY_REQUEST)
                        {
                            //Out of foreach 
                            error = (int)(DriverErrorCodes)BACnetErrorCodes.ErrorBACnetTimeOut;
                            exitListObjectType = true;
                            break;
                        }
                        //break;
                    }
                    // send ack message when fragmented message arrived
                    else if (messag.command == BACnetStateRequestImport.Object_Name_Segment_ACK)
                    {
                        BACnetIPFrame BACnetIPFrame = new BACnetIPFrame(BACnetEnums.ConfirmedService.READ_PROPERTY);
                        BACnetIPFrame.Npdu.Apdu.PduType = BACnetEnums.BACnetPDU.SEGMENT_ACK;
                        BACnetIPFrame.Npdu.DataExpectingReply = false;
                        BACnetIPFrame.Npdu.Apdu.InvokeId = messag.InvokeId;
                        BACnetIPFrame.Npdu.Apdu.SequenceNumber = messag.SequenceNumber;
                        BACnetIPFrame.Npdu.Apdu.WindowSize = messag.WindowSize;
                        if (!SendMessage(BACnetIPFrame, Station))
                        {
                            error = (int)(DriverErrorCodes)BACnetErrorCodes.ErrorTxWrite;
                            exitListObjectType = true;
                        }                        
                    }
                }
                if (exitListObjectType)
                    break;
            }
            PropertyListRequestQueue.Dispose();
            PropertyListRequestQueue = null;            

            return error;
        }
        public int DecodeReadPropertyMultiple(byte[] buffer)
        {
            if (PropertyListRequestQueue == null)
            {
                return 0;
            }
            if (BufferExpand.DecodeReadPropertyMultipleAcknowledge(buffer, 0, buffer.Length, ref itemList) < 0)
            {
                return -1;
            }
            return 0;
        }


        #endregion

    }
}
