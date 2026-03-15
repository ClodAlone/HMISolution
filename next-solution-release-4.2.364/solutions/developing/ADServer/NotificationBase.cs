using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPCUAViewModel;
using UFUAModel;
using UFUAAlarm;
using ViewModelLib;
using UFInterfaces;
using System.Threading;
using UFUAServerBase;
using Opc.Ua;
using System.Globalization;
using System.ComponentModel;
using Opc.Ua.Client;
using DevExpress.Xpo;
using XpoHelpers;
using ADModel;
using Utilities;
using Utilities.Logger;
using UFUserModel;

namespace ADServer
{
    public class NotificationBase : IDisposable, IEntityReference
    {

        public NodeId nodeId;
        public String name;
        public String persistenceName;
        public UFUAModel.AlarmType type;
        public TripCondition TripCondition;
        public ExceptionDeviationFormat ExceptionDeviationFormat;
        public double ActivationValue;
        public double?[] Limits;
        public double TimeUnit;
        public double DelayTimeOn;
        public double DelayTimeOff;
        public EventSeverity Severity;
        public Range range;
        public Range instrumentRange;
        public bool QualityGoodOnly;

        internal AlarmState state = AlarmState.Enabled;
        internal String Reason;
        internal String Comment;
        internal String UserName;
        internal DateTime EnableTime;
        internal DateTime ActiveTime;
        internal DateTime Time = DateTime.MinValue;
        internal DateTime lastTimeUdapted = DateTime.MinValue;
        internal double lastValue = 0.0;

        internal uint flMessageSent = 0;

        internal bool firstTime = true;

        internal bool svractive;
        Timer DelayTimer = null;

        Timer rateOfChangeTimer;
        string textMessage = string.Empty;
        string alarmMessage = null;

        DateTime lastTimeStateChanged = DateTime.UtcNow;
        public NotificationBase()
        {
        }

        public NotificationBase(ADModel.ADNotification n)
        {
            _Recipient = n.Recipient;
            RecipientID = n.RecipientID;
            MultiRecipientID = n.MultiRecipientID;
            _NotificationItem = n.NotificationItem;
            _PluginName = n.PluginName;
            _ADGroupMessage = n.ADGroupMessage;
            _PluginId = n.PluginID;
            textMessage = n.Message;
            Attachments = n.Attachments;

            _EnableON = n.EnableON.Value;
            _EnableOFF = n.EnableOFF;
            _EnableACK = n.EnableACK;
            _EnableCONFIRMED = n.EnableCONFIRMED;

            name = n.Name;
            persistenceName = n.Name;
            type = n.AlarmType.Value;
            nodeId = n.NodeId;
            TripCondition = (TripCondition)n.ConditionType;
            ActivationValue = n.ActivationValue.Value;
            ExceptionDeviationFormat = (ExceptionDeviationFormat)n.DeviationType;
            Limits = new double?[4];
            //range = analogVariable != null ? analogVariable.EURange.Value : null,
            //instrumentRange = analogVariable != null ? analogVariable.InstrumentRange.Value : null,
            TimeUnit = n.TimeUnit.TotalMilliseconds;
            DelayTimeOn = n.DelayTimeOn.TotalMilliseconds;
            DelayTimeOff = n.DelayTimeOff.TotalMilliseconds;

            Limits[0] = (n.EnableHighHighLimit ? n.HighHighLimit : null);
            Limits[1] = (n.EnableHighLimit ? n.HighLimit : null);
            Limits[2] = (n.EnableLowLimit ? n.LowLimit : null);
            Limits[3] = (n.EnableLowLowLimit ? n.LowLowLimit : null);
            NotificationType = n.NotificationType;
            AlarmGuid = n.AlarmNodeId;
            AlarmName = n.AlarmName;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (OPCItem != null)
                {
                    //if (OPCItem.MonitoredItemViewModel != null)
                    //    OPCItem.MonitoredItemViewModel.PropertyChanged -= MonitoredItemViewModel_PropertyChanged;

                    OPCItem.SetInUse(this, false);
                    OPCItem = null;
                }

                if (observer != null)
                {
                    observer.Dispose();
                    observer = null;
                }

                if (observerMonitoredModel != null)
                {
                    observerMonitoredModel.Dispose();
                    observerMonitoredModel = null;
                }

                if (dl != null)
                {
                    dl.Dispose();
                    dl = null;
                }

                if (objectsToDisposeOnDisconnect != null)
                {
                    foreach (IDisposable obj in objectsToDisposeOnDisconnect)
                        obj.Dispose();
                }
            }
        }
        ~NotificationBase()
        {
            Dispose(false);
        }

        private ADUAServer _ServerInstance;
        public ADUAServer ServerInstance
        {
            get
            {
                return _ServerInstance;
            }
            set
            {
                _ServerInstance = value;
            }
        }

        private string _PluginName;
        public string PluginName
        {
            get
            {
                return _PluginName;
            }
            set
            {
                _PluginName = value;
            }
        }

        private bool _ADGroupMessage;
        public bool ADGroupMessage
        {
            get
            {
                return _ADGroupMessage;
            }
            set
            {
                _ADGroupMessage = value;
            }
        }
        private string _Attachments;
        public string Attachments
        {
            get
            {
                return _Attachments;
            }
            set
            {
                _Attachments = value;
            }
        }
        private int _PriorityDelay;
        public int PriorityDelay
        {
            get
            {
                return _PriorityDelay;
            }
            set
            {
                _PriorityDelay = value;
            }
        }
        private NodeId _PluginId;
        public NodeId PluginId
        {
            get
            {
                return _PluginId;
            }
            set
            {
                _PluginId = value;
            }
        }
        private string _Recipient;
        public string Recipient
        {
            get
            {
                return _Recipient;
            }
            set
            {
                _Recipient = value;
            }
        }
        public NodeId RecipientID { get; set; }
        public string MultiRecipientID { get; set; }
        private UFUserModel.UFRole _ListOfRecpient;
        public UFUserModel.UFRole ListOfRecpient
        {
            get
            {
                return _ListOfRecpient;
            }
            set
            {
                _ListOfRecpient = value;
            }
        }

        private List<UFUserModel.UFUser> _MultiRecipients = new List<UFUserModel.UFUser>();
        public List<UFUserModel.UFUser> MultiRecipients
        {
            get { return _MultiRecipients; }
            set { _MultiRecipients = value; }
        }

        private UFUserModel.UFUser _SingleRecipient;
        public UFUserModel.UFUser SingleRecipient
        {
            get
            {
                return _SingleRecipient;
            }
            set
            {
                _SingleRecipient = value;
            }
        }

        private NotificationTypes _NotificationType;
        public NotificationTypes NotificationType
        {
            get { return _NotificationType; }
            set { _NotificationType = value; }
        }

        private bool _EnableON;
        public bool EnableON
        {
            get { return _EnableON; }
            set { _EnableON = value; }
        }

        private bool _EnableOFF;
        public bool EnableOFF
        {
            get { return _EnableOFF; }
            set { _EnableOFF = value; }
        }

        private bool _EnableACK;
        public bool EnableACK
        {
            get { return _EnableACK; }
            set { _EnableACK = value; }
        }

        private bool _EnableCONFIRMED;
        public bool EnableCONFIRMED
        {
            get { return _EnableCONFIRMED; }
            set { _EnableCONFIRMED = value; }
        }

        private String _ServerMessage;
        public String ServerMessage
        {
            get { return _ServerMessage; }
            set { _ServerMessage = value; }
        }

        private DateTime? _ServerTime;
        public DateTime? ServerTime
        {
            get { return _ServerTime; }
            set { _ServerTime = value; }
        }

        private String _ServerEnabledState;
        public String ServerEnabledState
        {
            get { return _ServerEnabledState; }
            set { _ServerEnabledState = value; }
        }

        private Guid _AlarmGuid;
        public Guid AlarmGuid
        {
            get { return _AlarmGuid; }
            set { _AlarmGuid = value; }
        }
        private NodeId _AlarmNodeId;
        public NodeId AlarmNodeId
        {
            get { return _AlarmNodeId; }
            set { _AlarmNodeId = value; }
        }

        private string _AlarmName;
        public string AlarmName
        {
            get
            {
                return _AlarmName;
            }
            set
            {
                _AlarmName = value;
            }
        }

        private string _ServerAlarmStringId;
        public string ServerAlarmStringId
        {
            get { return _ServerAlarmStringId; }
            set { _ServerAlarmStringId = value; }
        }
        public byte CurrentState { get; set; }
        public ushort CurrentWState { get; set; }
        public bool HasMessage { get; set; }

        private bool isInternal = true;
        #region OpcUaEntityReference
        private string _NotificationItem;
        public string NotificationItem
        {
            get
            {
                return _NotificationItem;
            }
            set
            {
                NotificationItem = _NotificationItem;
            }
        }

        public OPCUAEntityReference OPCItem = new OPCUAEntityReference();
        readonly Object lockObject = new Object();
        #endregion
        #region Methods
        PropertyObserver<OPCUAEntityReference> observer;
        PropertyObserver<MonitoredItemViewModel> observerMonitoredModel;
        public bool PrepareExecution(String sessionname)
        {
            isInternal = ServerInstance.BelongToThisServer(OPCItem.EndpointUrl);
            observer = new PropertyObserver<OPCUAEntityReference>(OPCItem);


            observer.RegisterHandler(n => n.MonitoredItemViewModel, n =>
            {
                observer.UnregisterHandler(p => p.MonitoredItemViewModel);

                try
                {
                    ReadValueIdCollection nodesToRead = new ReadValueIdCollection();
                    Browser browser = new Browser(n.GetSession(sessionname).Session);
                    browser.BrowseDirection = BrowseDirection.Forward;
                    browser.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                    browser.IncludeSubtypes = true;
                    browser.NodeClassMask = (int)NodeClass.Variable;
                    browser.ContinueUntilDone = true;

                    ReferenceDescriptionCollection references = browser.Browse(OPCItem.ResolvedNodeId);
                    foreach (ReferenceDescription reference in references)
                    {
                        ReadValueId valueId = new ReadValueId { NodeId = ExpandedNodeId.ToNodeId(reference.NodeId, browser.Session.NamespaceUris), AttributeId = Attributes.Value, IndexRange = null, DataEncoding = null };
#if DEBUG
                        System.Diagnostics.Trace.TraceInformation(
                            string.Format("refenence Name = {0}", reference.BrowseName.Name)
                            );
#endif
                        nodesToRead.Add(valueId);

                        DataValueCollection values;
                        DiagnosticInfoCollection diagnosticInfos;

                        browser.Session.Read(
                            null,
                            0,
                            TimestampsToReturn.Neither,
                            nodesToRead,
                            out values,
                            out diagnosticInfos);

#if DEBUG
                        System.Diagnostics.Trace.TraceInformation(
                            string.Format("After browse refenence Name = {0}", reference.BrowseName.Name)
                            );
#endif
                        ClientBase.ValidateResponse(values, nodesToRead);
                        ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);
                        if (reference.BrowseName.Name.Contains("InstrumentRange"))
                        {
                            if (values != null && values.Count > 0)
                            {
                                var eo = values[0].Value as ExtensionObject;
                                if (eo != null && eo.Body is Range)
                                {
                                    instrumentRange = eo.Body as Range;
                                }
                            }
                        }
                        else if (reference.BrowseName.Name.Contains("EURange"))
                        {
                            if (values != null && values.Count > 0)
                            {
                                var eo = values[0].Value as ExtensionObject;
                                if (eo != null && eo.Body is Range)
                                {
                                    range = eo.Body as Range;
                                }
                            }
                        }

                        nodesToRead.Clear();
                    }


                }
                catch (Exception e)
                { }

                if(NotificationType != NotificationTypes.Server)
                ServerInstance.AddToMapNodeIdToNotifications(this);
                if (n.MonitoredItemViewModel != null)
                {
                    observerMonitoredModel =
                        new PropertyObserver<MonitoredItemViewModel>(n.MonitoredItemViewModel);

                    if (NotificationType != NotificationTypes.Server)
                    {
                        observerMonitoredModel.RegisterHandler(m => m.DataValue, m =>
                        {

                            if (m.DataValue != null && DataValue.IsGood(m.DataValue))
                            {
                                ServerInstance.UpdateNotifications(OPCItem, m.DataValue);
                            }
                        });

                        observerMonitoredModel.RegisterHandler(m => m.referenceItem, m =>
                        {
                            var lst = (from p in m.referenceItem.ReadableAttributesList where p.Name.Contains("InstrumentRange") select p).ToList();
                        });
                    }

                }
            });

            try
            {
                OPCItem.Resolve(sessionname);
                OPCItem.SetInUse(this, true);
            }
            catch (Exception e)
            {
                ServerInstance.OServer.OnSystemEvent(ObjectIds.Server, string.Format(Properties.Resources.ErrorPrepareExecution, name, e.Message), EventSeverity.High, ObjectTypeIds.SystemStatusChangeEventType, null,
                        null, (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                return false;
            }

            return true;
        }

        public List<UFUser> GetUserWithTags()
        {
            List<UFUser> completeUserList = new List<UFUser>();
            if (ListOfRecpient != null && ListOfRecpient.UFUsers != null && ListOfRecpient.UFUsers.Count > 0)
                completeUserList.AddRange(ListOfRecpient.UFUsers);
            if (MultiRecipients != null && MultiRecipients.Count > 0)
                completeUserList.AddRange(MultiRecipients);
            if (SingleRecipient != null)
                completeUserList.Add(SingleRecipient);
            if(completeUserList.Count > 0)
            {
                var ul = (from u in completeUserList where !string.IsNullOrEmpty(u.TagPhoneNumber) select u).ToList();
                if (ul.Count > 0)
                    return ul;
            }
            
            return null;
        }

        public bool SetStateBits(AlarmState bits, bool isSet)
        {
            if (isSet)
            {
                bool currentlySet = ((state & bits) == bits);
                state |= bits;
                return !currentlySet;
            }

            bool currentlyCleared = ((state & ~bits) == state);
            state &= ~bits;
            return !currentlyCleared;
        }

        bool bTimerOn = false;
        bool bTimerOff = false;
        public bool UpdateNotificationStatus(DataValue value)
        {
#if DEBUG
            System.Diagnostics.Trace.TraceInformation(string.Format("UpdateNotificationStatus {0} value {1} lastvalue {2}", name, value, lastValue));
#endif
            if (value.Value == null)
                return false;
            string message = string.Empty;
            lock (lockObject)
            {
                bool bRateOfChange = false;
                if (TypeInfo.IsNumericType(value.WrappedValue.TypeInfo.BuiltInType) || value.WrappedValue.TypeInfo.BuiltInType == BuiltInType.Boolean)
                {
                    var doubleValue = Convert.ToDouble(value.Value);
                    if (lastValue == doubleValue)
                        return false;


                    if (rateOfChangeTimer != null)
                    {
                        bRateOfChange = true;
                        rateOfChangeTimer.Dispose();
                        rateOfChangeTimer = null;
                    }

                    var nowUtc = DateTime.UtcNow;
                    bool bActive = (state & AlarmState.Active) != 0;
                    if (StatusCode.IsGood(value.StatusCode))
                    {
                        bool bStateChanged = false;

                        switch (type)
                        {
                            //case UFUAModel.AlarmType.ExclusiveLimit:
                            //case UFUAModel.AlarmType.NonExclusiveLimit:
                            case UFUAModel.AlarmType.ExclusiveLevel:
                            case UFUAModel.AlarmType.NonExclusiveLevel:
                                {

                                    if (Limits[0].HasValue && doubleValue > Limits[0]) // HighHighLimit
                                    {
                                        bActive = true;
                                        bStateChanged |= SetStateBits(AlarmState.HighHigh, true);
                                        bStateChanged |= SetStateBits(AlarmState.High, (type != UFUAModel.AlarmType.ExclusiveLevel/* && type != UFUAModel.AlarmType.ExclusiveLimit*/));
                                        bStateChanged |= SetStateBits(AlarmState.Low, false);
                                        bStateChanged |= SetStateBits(AlarmState.LowLow, false);
                                        Reason = (type == AlarmType.ExclusiveLevel ?
                                            Properties.Resources.HHExclusive : Properties.Resources.HHNonExclusive);
                                    }
                                    else if (Limits[1].HasValue && doubleValue > Limits[1]) // HighLimit
                                    {
                                        bActive = true;
                                        bStateChanged |= SetStateBits(AlarmState.HighHigh, false);
                                        bStateChanged |= SetStateBits(AlarmState.High, true);
                                        bStateChanged |= SetStateBits(AlarmState.Low, false);
                                        bStateChanged |= SetStateBits(AlarmState.LowLow, false);
                                        Reason = (type == AlarmType.ExclusiveLevel ?
                                            Properties.Resources.HExclusive : Properties.Resources.HNonExclusive);
                                    }
                                    else if (Limits[3].HasValue && doubleValue < Limits[3]) // LowLowLimit
                                    {
                                        bActive = true;
                                        bStateChanged |= SetStateBits(AlarmState.HighHigh, false);
                                        bStateChanged |= SetStateBits(AlarmState.High, false);
                                        bStateChanged |= SetStateBits(AlarmState.Low, (type != UFUAModel.AlarmType.ExclusiveLevel/* && type != UFUAModel.AlarmType.ExclusiveLimit*/));
                                        bStateChanged |= SetStateBits(AlarmState.LowLow, true);
                                        Reason = (type == AlarmType.ExclusiveLevel ?
                                            Properties.Resources.LLExclusive : Properties.Resources.LLNonExclusive);
                                    }
                                    else if (Limits[2].HasValue && doubleValue < Limits[2]) // LowLimit
                                    {
                                        bActive = true;
                                        bStateChanged |= SetStateBits(AlarmState.HighHigh, false);
                                        bStateChanged |= SetStateBits(AlarmState.High, false);
                                        bStateChanged |= SetStateBits(AlarmState.Low, true);
                                        bStateChanged |= SetStateBits(AlarmState.LowLow, false);
                                        Reason = (type == AlarmType.ExclusiveLevel ?
                                            Properties.Resources.LExclusive : Properties.Resources.LNonExclusive);
                                    }
                                    else
                                    {
                                        bActive = false;
                                        bStateChanged |= SetStateBits(AlarmState.HighHigh, false);
                                        bStateChanged |= SetStateBits(AlarmState.High, false);
                                        bStateChanged |= SetStateBits(AlarmState.Low, false);
                                        bStateChanged |= SetStateBits(AlarmState.LowLow, false);
                                        Reason = Properties.Resources.Inactive;
                                    }

                                    if (SetStateBits(AlarmState.Active, bActive))
                                    {
                                        Time = DateTime.UtcNow;
                                        //Reason = "The alarm is " + (bActive ? "active" : "inactive.");
                                    }
                                    if (lastValue != doubleValue)
                                    {
                                        lastValue = doubleValue;
                                        lastTimeUdapted = DateTime.UtcNow;
                                    }
                                    break;
                                }
                            case UFUAModel.AlarmType.ExclusiveDeviation:
                            case UFUAModel.AlarmType.NonExclusiveDeviation:
                                {
                                    if (Limits[0].HasValue && CheckHysteresis(doubleValue, lastValue/*ActivationValue*/, Limits[0].Value))               // HighHighLimit
                                    {
                                        bActive = true;
                                        bStateChanged |= SetStateBits(AlarmState.HighHigh, true);
                                        bStateChanged |= SetStateBits(AlarmState.High, type != UFUAModel.AlarmType.ExclusiveDeviation);
                                        bStateChanged |= SetStateBits(AlarmState.Low, false);
                                        bStateChanged |= SetStateBits(AlarmState.LowLow, false);
                                        Reason = (type == AlarmType.ExclusiveDeviation ?
                                            Properties.Resources.HHExclusive : Properties.Resources.HHNonExclusive);
                                    }
                                    else if (Limits[1].HasValue && CheckHysteresis(doubleValue, lastValue/*ActivationValue*/, Limits[1].Value))          // HighLimit
                                    {
                                        bActive = true;
                                        bStateChanged |= SetStateBits(AlarmState.HighHigh, false);
                                        bStateChanged |= SetStateBits(AlarmState.High, true);
                                        bStateChanged |= SetStateBits(AlarmState.Low, false);
                                        bStateChanged |= SetStateBits(AlarmState.LowLow, false);
                                        Reason = (type == AlarmType.ExclusiveDeviation ?
                                            Properties.Resources.HExclusive : Properties.Resources.HNonExclusive);
                                    }
                                    else if (Limits[3].HasValue && CheckHysteresis(doubleValue, lastValue/*ActivationValue*/, Limits[3].Value))         // LowLowLimit
                                    {
                                        bActive = true;
                                        bStateChanged |= SetStateBits(AlarmState.HighHigh, false);
                                        bStateChanged |= SetStateBits(AlarmState.High, false);
                                        bStateChanged |= SetStateBits(AlarmState.Low, type != UFUAModel.AlarmType.ExclusiveDeviation);
                                        bStateChanged |= SetStateBits(AlarmState.LowLow, true);
                                        Reason = (type == AlarmType.ExclusiveDeviation ?
                                            Properties.Resources.LLExclusive : Properties.Resources.LLNonExclusive);
                                    }
                                    else if (Limits[2].HasValue && CheckHysteresis(doubleValue, lastValue/*ActivationValue*/, Limits[2].Value))         // LowLimit
                                    {
                                        bActive = true;
                                        bStateChanged |= SetStateBits(AlarmState.HighHigh, false);
                                        bStateChanged |= SetStateBits(AlarmState.High, false);
                                        bStateChanged |= SetStateBits(AlarmState.Low, true);
                                        bStateChanged |= SetStateBits(AlarmState.LowLow, false);
                                        Reason = (type == AlarmType.ExclusiveDeviation ?
                                            Properties.Resources.LExclusive : Properties.Resources.LNonExclusive);
                                    }
                                    else
                                    {
                                        bActive = false;
                                        bStateChanged |= SetStateBits(AlarmState.HighHigh, false);
                                        bStateChanged |= SetStateBits(AlarmState.High, false);
                                        bStateChanged |= SetStateBits(AlarmState.Low, false);
                                        bStateChanged |= SetStateBits(AlarmState.LowLow, false);
                                        Reason = Properties.Resources.Inactive;
                                    }

                                    if (SetStateBits(AlarmState.Active, bActive))
                                    {
                                        Time = DateTime.UtcNow;
                                        //Reason = "The alarm is " + (bActive ? "active" : "inactive.");
                                    }
                                    if (lastValue != doubleValue)
                                    {
                                        lastValue = doubleValue;
                                        lastTimeUdapted = DateTime.UtcNow;
                                    }
                                    break;
                                }
                            case UFUAModel.AlarmType.ExclusiveRateOfChange:
                            case UFUAModel.AlarmType.NonExclusiveRateOfChange:
                                {
                                    if (Limits[0].HasValue && CheckHysteresis(doubleValue, lastValue, Limits[0].Value))                 // HighHighLimit
                                    {
#if DEBUG
                                        System.Diagnostics.Trace.TraceInformation(string.Format("RateOfChange HighHighLimit val:{0} last:{1} limit:{2}", doubleValue, lastValue, Limits[0].Value));
#endif
                                        bActive = true;
                                        bStateChanged |= SetStateBits(AlarmState.HighHigh, true);
                                        bStateChanged |= SetStateBits(AlarmState.High, type != UFUAModel.AlarmType.ExclusiveRateOfChange);
                                        bStateChanged |= SetStateBits(AlarmState.Low, false);
                                        bStateChanged |= SetStateBits(AlarmState.LowLow, false);
                                        Reason = (type == AlarmType.ExclusiveRateOfChange ?
                                            Properties.Resources.HHExclusive : Properties.Resources.HHNonExclusive);
                                    }
                                    else if (Limits[1].HasValue && CheckHysteresis(doubleValue, lastValue, Limits[1].Value))            // HighLimit
                                    {
#if DEBUG
                                        System.Diagnostics.Trace.TraceInformation(string.Format("RateOfChange HighLimit val:{0} last:{1} limit:{2}", doubleValue, lastValue, Limits[1].Value));
#endif
                                        bActive = true;
                                        bStateChanged |= SetStateBits(AlarmState.HighHigh, false);
                                        bStateChanged |= SetStateBits(AlarmState.High, true);
                                        bStateChanged |= SetStateBits(AlarmState.Low, false);
                                        bStateChanged |= SetStateBits(AlarmState.LowLow, false);
                                        Reason = (type == AlarmType.ExclusiveRateOfChange ?
                                            Properties.Resources.HExclusive : Properties.Resources.HNonExclusive);
                                    }
                                    else if (Limits[3].HasValue && CheckHysteresis(doubleValue, lastValue, Limits[3].Value))           // LowLowLimit
                                    {
#if DEBUG
                                        System.Diagnostics.Trace.TraceInformation(string.Format("RateOfChange LowLowLimit val:{0} last:{1} limit:{2}", doubleValue, lastValue, Limits[3].Value));
#endif
                                        bActive = true;
                                        bStateChanged |= SetStateBits(AlarmState.HighHigh, false);
                                        bStateChanged |= SetStateBits(AlarmState.High, false);
                                        bStateChanged |= SetStateBits(AlarmState.Low, type != UFUAModel.AlarmType.ExclusiveRateOfChange);
                                        bStateChanged |= SetStateBits(AlarmState.LowLow, true);
                                        Reason = (type == AlarmType.ExclusiveRateOfChange ?
                                            Properties.Resources.LLExclusive : Properties.Resources.LLNonExclusive);
                                    }
                                    else if (Limits[2].HasValue && CheckHysteresis(doubleValue, lastValue, Limits[2].Value))           // LowLimit
                                    {
#if DEBUG
                                        System.Diagnostics.Trace.TraceInformation(string.Format("RateOfChange LowLimit val:{0} last:{1} limit:{2}", doubleValue, lastValue, Limits[2].Value));
#endif
                                        bActive = true;
                                        bStateChanged |= SetStateBits(AlarmState.HighHigh, false);
                                        bStateChanged |= SetStateBits(AlarmState.High, false);
                                        bStateChanged |= SetStateBits(AlarmState.Low, true);
                                        bStateChanged |= SetStateBits(AlarmState.LowLow, false);
                                        Reason = (type == AlarmType.ExclusiveRateOfChange ?
                                            Properties.Resources.LExclusive : Properties.Resources.LNonExclusive);
                                    }
                                    else
                                    {
#if DEBUG
                                        System.Diagnostics.Trace.TraceInformation(string.Format("RateOfChange Else val:{0} last:{1}", doubleValue, lastValue));
#endif
                                        bActive = false;
                                        bStateChanged |= SetStateBits(AlarmState.HighHigh, false);
                                        bStateChanged |= SetStateBits(AlarmState.High, false);
                                        bStateChanged |= SetStateBits(AlarmState.Low, false);
                                        bStateChanged |= SetStateBits(AlarmState.LowLow, false);
                                        Reason = Properties.Resources.Inactive;
                                    }

                                    if (((bActive && EnableON) || (!bActive && EnableOFF)) && bStateChanged)
                                        SendNotification();

                                    if (SetStateBits(AlarmState.Active, bActive))
                                    {
#if DEBUG
                                        System.Diagnostics.Trace.TraceInformation(string.Format("RateOfChange SetStateBits(AlarmState.Active, {0})", bActive));
#endif

                                        Time = DateTime.UtcNow;
                                        //Reason = "The alarm is " + (bActive ? "active" : "inactive.");
                                        //if (bActive)
                                        //    SendNotification();
                                    }
                                    if (bStateChanged || (lastTimeStateChanged + TimeSpan.FromMilliseconds(TimeUnit) <= DateTime.UtcNow))
                                    {
#if DEBUG
                                        System.Diagnostics.Trace.TraceInformation(string.Format("RateOfChange bStateChanbe:{0} lastTimeStateChanged:{1} TimeUnit:{2}",
                                            bStateChanged, lastTimeStateChanged, TimeUnit));
#endif

                                        lastTimeStateChanged = DateTime.UtcNow;

                                        if (bStateChanged || bRateOfChange)
                                        {
                                            lastValue = doubleValue;
                                            lastTimeUdapted = DateTime.UtcNow;
                                        }

                                        if (bActive)
                                        {

                                            TimeSpan dueTime = TimeSpan.FromMilliseconds(TimeUnit) - (DateTime.UtcNow - lastTimeStateChanged);
#if DEBUG
                                            System.Diagnostics.Trace.TraceInformation(string.Format("RateOfChange Timer dueTime:{0} value:{1}",
                                                dueTime, value));
#endif
                                            rateOfChangeTimer = new Timer((o) =>
                                            {
                                                UpdateNotificationStatus(value);
                                            },
                                            null,
                                            dueTime > TimeSpan.Zero ? dueTime : TimeSpan.Zero,
                                            TimeSpan.FromMilliseconds(-1));
                                        }
                                    }
                                    else if (TimeUnit > 0.0)
                                    {
                                        TimeSpan dueTime = TimeSpan.FromMilliseconds(TimeUnit) - (DateTime.UtcNow - lastTimeStateChanged);
#if DEBUG
                                        System.Diagnostics.Trace.TraceInformation(string.Format("RateOfChange ElseTimer dueTime:{0} value:{1}",
                                            dueTime, value));
#endif
                                        rateOfChangeTimer = new Timer((o) =>
                                        {
                                            UpdateNotificationStatus(value);
                                        },
                                        null,
                                        dueTime > TimeSpan.Zero ? dueTime : TimeSpan.Zero,
                                        TimeSpan.FromMilliseconds(-1));
                                    }

                                    break;
                                }
                            case UFUAModel.AlarmType.TripAlarm:
                                {
                                    switch (TripCondition)
                                    {
                                        case TripCondition.Equals: bActive = (doubleValue == ActivationValue); break;
                                        case TripCondition.GreaterThan: bActive = (doubleValue > ActivationValue); break;
                                        case TripCondition.GreaterThanOrEqual: bActive = (doubleValue >= ActivationValue); break;
                                        case TripCondition.LessThan: bActive = (doubleValue < ActivationValue); break;
                                        case TripCondition.LessThanOrEqual: bActive = (doubleValue <= ActivationValue); break;
                                        case TripCondition.NotEqual: bActive = (doubleValue != ActivationValue); break;
                                        default: bActive = (doubleValue == ActivationValue); break;
                                    }

                                    if (SetStateBits(AlarmState.Active, bActive))
                                    {
                                        bStateChanged = true;
                                        Time = DateTime.UtcNow;
                                        Reason = (bActive ? Properties.Resources.TripActive : Reason = Properties.Resources.Inactive);
                                    }
                                    if (doubleValue != lastValue)
                                    {
                                        lastValue = doubleValue;
                                        lastTimeUdapted = DateTime.UtcNow;
                                    }
                                    break;
                                }
                        }
                        bool bratechange = (type == UFUAModel.AlarmType.ExclusiveRateOfChange || type == UFUAModel.AlarmType.NonExclusiveRateOfChange);
                        if (!bratechange && bStateChanged)
                        {
                            lastTimeStateChanged = DateTime.UtcNow;
                            if (bActive)
                            {
                                if (DelayTimeOn > 0.0)
                                {
                                    if (bTimerOff)
                                        return false;
                                    TimeSpan dueTime = TimeSpan.Zero;
                                    dueTime = TimeSpan.FromMilliseconds(DelayTimeOn) - (DateTime.UtcNow - lastTimeStateChanged);
                                    if (DelayTimer != null)
                                    {
                                        DelayTimer.Dispose();
                                        DelayTimer = null;
                                    }
                                    bTimerOn = true;
                                    bTimerOff = false;
                                    DelayTimer = new Timer((o) =>
                                    {
#if DEBUG
                                        System.Diagnostics.Trace.TraceInformation(string.Format("Timer Send bActive:{0} bTimerOn:{1} bTimerOff:{2} DelayOn:{3} DelayOff:{4}", bActive, bTimerOn, bTimerOff, DelayTimeOn, DelayTimeOff));
#endif
                                        if (((bActive && EnableON) || (!bActive && EnableOFF)) && bStateChanged)
                                            SendNotification();
                                        bTimerOn = false;
                                        bTimerOff = false;
                                    }, null,
                                    dueTime > TimeSpan.Zero ? dueTime : TimeSpan.Zero,
                                    TimeSpan.FromMilliseconds(-1));

#if DEBUG
                                    System.Diagnostics.Trace.TraceInformation(string.Format("Start Timer Send bActive:{0} bTimerOn:{1} bTimerOff:{2} DelayOn:{3} DelayOff:{4}", bActive, bTimerOn, bTimerOff, DelayTimeOn, DelayTimeOff));
#endif

                                    return false;
                                }
                                else
                                {
                                    if (bTimerOff)
                                        return false;
                                    if (EnableON) //if (((/*bActive always true &&*/ EnableON) /*|| (!bActive && EnableOFF) always false*/) /*&& bStateChanged always true*/)
                                        SendNotification();
#if DEBUG
                                    System.Diagnostics.Trace.TraceInformation(string.Format("SendNotification bActive:{0} bTimerOn:{1} bTimerOff:{2} DelayOn:{3} DelayOff:{4}", bActive, bTimerOn, bTimerOff, DelayTimeOn, DelayTimeOff));
#endif

                                }
                            }
                            else
                            {
                                if (DelayTimeOff > 0.0)
                                {
                                    if (bTimerOn)
                                        return false;
                                    TimeSpan dueTime = TimeSpan.Zero;
                                    dueTime = TimeSpan.FromMilliseconds(DelayTimeOff) - (DateTime.UtcNow - lastTimeStateChanged);
                                    if (DelayTimer != null)
                                    {
                                        DelayTimer.Dispose();
                                        DelayTimer = null;
                                    }
                                    bTimerOn = false;
                                    bTimerOff = true;
                                    DelayTimer = new Timer((o) =>
                                    {
#if DEBUG
                                        System.Diagnostics.Trace.TraceInformation(string.Format("Timer Off bActive:{0} bTimerOn:{1} bTimerOff:{2} DelayOn:{3} DelayOff:{4}", bActive, bTimerOn, bTimerOff, DelayTimeOn, DelayTimeOff));
#endif
                                        if (((bActive && EnableON) || (!bActive && EnableOFF)) && bStateChanged)
                                            SendNotification();
                                        bTimerOn = false;
                                        bTimerOff = false;
                                    }, null,
                                    dueTime > TimeSpan.Zero ? dueTime : TimeSpan.Zero,
                                    TimeSpan.FromMilliseconds(-1));
#if DEBUG
                                    System.Diagnostics.Trace.TraceInformation(string.Format("Start Timer Off bActive:{0} bTimerOn:{1} bTimerOff:{2} DelayOn:{3} DelayOff:{4}", bActive, bTimerOn, bTimerOff, DelayTimeOn, DelayTimeOff));
#endif
                                    return false;
                                }
                                else
                                {
                                    if (bTimerOn)
                                    {

                                        return false;
                                    }
                                    else if (EnableOFF)//if ((/*(bActive always false && EnableON) ||*/ (/*!bActive always false &&*/ EnableOFF)) /*&& bStateChanged always true*/)
                                        SendNotification();
#if DEBUG
                                    System.Diagnostics.Trace.TraceInformation(string.Format("Break Timer Send bActive:{0} bTimerOn:{1} bTimerOff:{2} DelayOn:{3} DelayOff:{4}", bActive, bTimerOn, bTimerOff, DelayTimeOn, DelayTimeOff));
#endif

                                }
                            }

                        }
                        if (bStateChanged)
                            SaveNotificationStatus();
                    }
                }
            }
            return true;
        }
        private bool CheckHysteresis(double doubleValue, double doublePrevValue, double deadband)
        {
            if ((ExceptionDeviationFormat != ExceptionDeviationFormat.PercentOfRange || instrumentRange != null) &&
                (ExceptionDeviationFormat != ExceptionDeviationFormat.PercentOfEURange || range != null))
            {
                var deadBand = new Utilities.Maths.Deadband(doublePrevValue, doubleValue, deadband) { TreatEqualLikeFalse = true };
                switch (ExceptionDeviationFormat)
                {
                    case ExceptionDeviationFormat.AbsoluteValue:
                        return deadBand.IsExceeded();
                    case ExceptionDeviationFormat.PercentOfRange:
                        return deadBand.IsExceeded(instrumentRange.High - instrumentRange.Low);
                    case ExceptionDeviationFormat.PercentOfValue:
                        return deadBand.IsExceeded(doublePrevValue);
                    case ExceptionDeviationFormat.PercentOfEURange:
                        return deadBand.IsExceeded(range.High - range.Low);
                    default:
                        return false;
                }
            }

            return false;
        }

        public void serverSendNotification(string message)
        {
            alarmMessage = message;
            SendNotification();
        }

        void SendNotification()
        {
            if (isInternal && ServerInstance != null && !ServerInstance.IsServerInActiveState())
                return;
                
            if (ADGroupMessage)
            {
                if (ListOfRecpient != null && ListOfRecpient.UFUsers != null && ListOfRecpient.UFUsers.Count > 0)
                {
                    List<UFUser> list = new List<UFUser>();
                    list.AddRange(ListOfRecpient.UFUsers);
                    SendListADGrouped(list);
                }
                else if (MultiRecipients.Count > 0)
                {
                    SendListADGrouped(MultiRecipients);
                }
                else if (SingleRecipient != null)
                    SendSingleNotification(SingleRecipient, null, SingleRecipient.UFRoleAss.TelegramGroupChatID);
            }
            else
            {
                //single or multiple?
                if (ListOfRecpient != null && ListOfRecpient.UFUsers != null && ListOfRecpient.UFUsers.Count > 0)
                {
                    //multiple
                    string groupid = string.Format("{0}{1}", ListOfRecpient.Name, Guid.NewGuid().ToString());
                    foreach (var r in ListOfRecpient.UFUsers)
                        SendSingleNotification(r, groupid);
                    if (ServerInstance.PluginThreads.ContainsKey(PluginName))
                        ServerInstance.PluginThreads[PluginName].PostMessageGroupComplete();

                }
                else if (MultiRecipients.Count > 0)
                {
                    string groupid = string.Format("{0}{1}", MultiRecipients[0].Name, Guid.NewGuid().ToString());
                    foreach (var r in MultiRecipients)
                        SendSingleNotification(r, groupid);
                    if (ServerInstance.PluginThreads.ContainsKey(PluginName))
                        ServerInstance.PluginThreads[PluginName].PostMessageGroupComplete();
                }
                else if (SingleRecipient != null)
                {
                    //single
                    SendSingleNotification(SingleRecipient);
                }
            }

        }

        void SendListADGrouped(List<UFUser> list)
        {
            List<UFUserModel.UFUser> userList = new List<UFUserModel.UFUser>();
            List<UFUserModel.UFRole> roleList = new List<UFUserModel.UFRole>();
            foreach (var r in list)
            {
                if (r.TelegramChatID == string.Empty || r.TelegramChatID == null)
                {
                    if (!roleList.Contains(r.UFRoleAss))
                    {
                        roleList.Add(r.UFRoleAss);
                        userList.Add(r);
                    }
                }
                else
                {
                    SendSingleNotification(r);
                }

            }
            foreach (var r in userList)
                SendSingleNotification(r, null, r.UFRoleAss.TelegramGroupChatID);
            userList.Clear();
            roleList.Clear();
        }
        internal void CopyValues(NotificationBase from)
        {
            ServerInstance = from.ServerInstance;
            _Recipient = from.Recipient;
            RecipientID = from.RecipientID;
            _NotificationItem = from.NotificationItem;
            _PluginName = from.PluginName;
            _ADGroupMessage = from.ADGroupMessage;
            _PluginId = from.PluginId;
            textMessage = from.textMessage;
            Attachments = from.Attachments;
            _PriorityDelay = from.PriorityDelay;

            SingleRecipient = from.SingleRecipient;
            ListOfRecpient = from.ListOfRecpient;
            MultiRecipients.Clear();
            MultiRecipients.AddRange(from.MultiRecipients);

            _EnableON = from.EnableON;
            _EnableOFF = from.EnableOFF;
            _EnableACK = from.EnableACK;
            _EnableCONFIRMED = from.EnableCONFIRMED;

            name = from.name;
            persistenceName = from.name;
            type = from.type;
            nodeId = from.nodeId;
            TripCondition = from.TripCondition;
            ActivationValue = from.ActivationValue;
            ExceptionDeviationFormat = from.ExceptionDeviationFormat;

            //range = analogVariable != null ? analogVariable.EURange.Value : null,
            //instrumentRange = analogVariable != null ? analogVariable.InstrumentRange.Value : null,
            TimeUnit = from.TimeUnit;
            DelayTimeOn = from.DelayTimeOn;
            DelayTimeOff = from.DelayTimeOff;
            if (Limits == null)
                Limits = new double?[4];

            Limits[0] = from.Limits[0];
            Limits[1] = from.Limits[1];
            Limits[2] = from.Limits[2];
            Limits[3] = from.Limits[3];
            NotificationType = from.NotificationType;
            AlarmGuid = from.AlarmGuid;
            AlarmName = from.AlarmName;

        }

        void SendSingleNotification(UFUserModel.UFUser u, string groupid = null, string TelegramGroupChatID = null)
        {
            if (ServerInstance == null)
                return;

            var locale = CultureInfo.InvariantCulture.Name;
            if (u.CultureName?.Length > 0)
                locale = u.CultureName;
            else if (u.GlobalCultureName?.Length > 0)
                locale = u.GlobalCultureName;
            var listlocales = new List<string>();
            var txt = new LocalizedText(textMessage, "", textMessage);
            var mname = new LocalizedText(name, "", name);
            var reason = new LocalizedText(Reason, "", Reason);
            var alarm = new LocalizedText(alarmMessage, "", alarmMessage);
            StringBuilder serverEnabledStates = new StringBuilder();
            listlocales.Add(locale);

            CultureInfo ci = null;
            try
            {
                ci = new CultureInfo(locale);
            }
            catch (Exception ex)
            {
                //log error
                if (ServerInstance.OServer != null)
                    ServerInstance.OServer.OnSystemEvent(ObjectIds.Server,
                        string.Format(Properties.Resources.CultureInfoError, locale, u.Name, ex.ToString()),
                        EventSeverity.High, ObjectTypeIds.SystemStatusChangeEventType, null, null, (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
            }


            if (!string.IsNullOrEmpty(ServerEnabledState))
            {
                LocalizedText state;
                bool bInit = false;
                ServerEnabledState.Split('|').ToList().ForEach(o =>
                {
                    state = new LocalizedText(o.Trim(), "", o.Trim());
                    state = ServerInstance.ServerInstance.ResourceManager.Translate(listlocales, state);
                    serverEnabledStates.Append(bInit ? $" | {state}" : $"{state}");
                    bInit = true;
                });
            }

            txt = ServerInstance.ServerInstance.ResourceManager.Translate(listlocales, txt);
            mname = ServerInstance.ServerInstance.ResourceManager.Translate(listlocales, mname);
            reason = ServerInstance.ServerInstance.ResourceManager.Translate(listlocales, reason);
            alarm = ServerInstance.ServerInstance.ResourceManager.Translate(listlocales, alarm);

            String message = string.Empty;
            string textmessage = txt.Text;
            DateTime dt = DateTime.Now;
            if (ServerInstance.UseLocalDateTime)
                dt = (ServerTime.HasValue ? ServerTime.Value.ToLocalTime() : DateTime.Now.ToLocalTime());
            else
                dt = (ServerTime.HasValue ? ServerTime.Value : DateTime.UtcNow);

            if (!ServerInstance.CustomMessage || (!ServerInstance.AddAlarmState && !ServerInstance.AddDateTime &&
                !ServerInstance.AddNotificationText && !ServerInstance.AddServerText &&
                !ServerInstance.AddNotificationName))
            {
                string fmt = string.Empty;
                if (!string.IsNullOrEmpty(alarm.Text))
                {
                    fmt = (ServerInstance.UseLocalDateTime ? "{0} {1} - {2}" : "UTC: {0} {1} - {2}");
                    message = String.Format(fmt, dt.ToString(ci), serverEnabledStates.ToString(), alarm.Text);
                }
                if (!string.IsNullOrEmpty(txt.Text) && string.IsNullOrEmpty(alarm.Text))
                {
                    fmt = (ServerInstance.UseLocalDateTime ? "{0} - {1}" : "UTC: {0} - {1}");
                    textmessage = String.Format("{0} - {1}", dt.ToString(ci), txt.Text);
                }
            }
            else
            {
                //Custom message, build text
                StringBuilder sb = new StringBuilder();
                if (ServerInstance.AddDateTime)
                    sb.AppendFormat((ServerInstance.UseLocalDateTime ? "{0} " : "UTC: {0} "), dt.ToString(ci));

                if (ServerInstance.AddNotificationName)
                    sb.AppendFormat("{0} - ", mname.Text);

                if (ServerInstance.AddAlarmState)
                    sb.AppendFormat("{0} - ", (string.IsNullOrEmpty(reason.Text) ? serverEnabledStates.ToString() : reason.Text));

                if (ServerInstance.AddNotificationText)
                    sb.AppendFormat("{0} - ", txt.Text);

                if (ServerInstance.AddServerText)
                    sb.AppendFormat("{0}", alarm.Text);

                char[] ctotrim = { ' ', '-' };
                textmessage = sb.ToString().TrimEnd(ctotrim);
                
            }


            bool bCustomMessage = ServerInstance.CustomMessage && (ServerInstance.AddAlarmState || ServerInstance.AddDateTime ||
                ServerInstance.AddNotificationText || ServerInstance.AddServerText ||
                ServerInstance.AddNotificationName);
            bool updateable = false;

            var phoneNumber = u.PhoneNumber;
            if(!string.IsNullOrEmpty(u.TagPhoneNumber))
            {
                //maybe tag
                var realtimePhoneNumber = ServerInstance.GetRealPhoneNumber(u.Name);
                //if realtimePhoneNumber is null, ServerInstance.GetRealPhoneNumber already logged an error, use user.PhoneNumber if not empty...
                if (!string.IsNullOrEmpty(realtimePhoneNumber))
                    phoneNumber = realtimePhoneNumber;
                updateable = true;
            }
            
            ADPluginBase.Message m = new ADPluginBase.Message()
            {
                Email = u.Email,
                Recipient = u.Name,
                MobilePhoneNumber = u.MobilePhoneNumber,
                PhoneNumber = phoneNumber,
                PluginID = PluginId,
                ChatID = u.TelegramChatID,
                FCMTokenPath = u.FCMTokenPath,
                Textmessage = textmessage,
                Alarmmessage = message,
                Reason = (string.IsNullOrEmpty(reason.Text) ? ServerEnabledState : reason.Text),
                Name = mname.Text,
                TimeStamp = DateTime.UtcNow + TimeSpan.FromMinutes(PriorityDelay),
                NodeId = new NodeId(Guid.NewGuid()),
                PluginName = PluginName,
                ADGroupMessage = ADGroupMessage,
                CustomMessage = bCustomMessage,
                TagName = (NotificationType == NotificationTypes.Local ? OPCItem.HumanReadable : AlarmName),
                Attachments = Attachments,
                GroupId = groupid,
                ServerAlarmStringId = ServerAlarmStringId,
                CultureName = locale,
                UserId = (updateable ? u.NodeId : Guid.Empty)
            };
            if (String.IsNullOrEmpty(m.ChatID) && TelegramGroupChatID != null)
                m.ChatID = TelegramGroupChatID;
            //alarmMessage = string.Empty;
            /*Save message in persistence*/
            ServerInstance.SavePersistentMessage(m);
            if (ServerInstance.PluginThreads.ContainsKey(PluginName))
                ServerInstance.PluginThreads[PluginName].PostMessage(m);
        }
        #endregion

        #region IEntityReference Members

        [Browsable(false)]
        public System.Windows.Media.ImageSource CollapsedImageSource
        {
            get { throw new NotImplementedException(); }
        }

        [Browsable(false)]
        public System.Windows.Media.ImageSource ExpandedImageSource
        {
            get { throw new NotImplementedException(); }
        }

        [Browsable(false)]
        public System.Windows.Controls.ContextMenu contextMenu
        {
            get { throw new NotImplementedException(); }
        }

        [Browsable(false)]
        public object Tooltip
        {
            get { throw new NotImplementedException(); }
        }

        [Browsable(false)]
        public object ContainedObject
        {
            get { throw new NotImplementedException(); }
        }

        [Browsable(false)]
        public object EntityParent
        {
            get { throw new NotImplementedException(); }
        }

        [Browsable(false)]
        public string TypeDefinitionString
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Persistence
        IDataLayer dl;
        IDisposable[] objectsToDisposeOnDisconnect;
        public void CreateDataLayer(String settings)
        {
            try
            {
                string conn = XpoHelper.GetConnectionString(settings, Properties.Settings.Default.TypeLabel, persistenceName, Properties.Settings.Default.DefaultPersistenceFileExt);
                var dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
                var store = XpoDefault.GetConnectionProvider(conn, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, out objectsToDisposeOnDisconnect);
                dict.GetDataStoreSchema(typeof(NotificationPersistence).Assembly);
                dl = new ThreadSafeDataLayer(dict, store);
            }
            catch (Exception e)
            {
                Opc.Ua.Utils.Trace(e, "Unexpected error creating the persistence data layer for Alarm Dispatcher Notification '{0}'", this.persistenceName);
            }
        }

        //return true only if there was data to load
        public bool LoadNotificationStatus()
        {
            if (dl != null)
            {
                using (UnitOfWork ufw = new UnitOfWork(dl))
                {
                    var status = (from notif in new XPQuery<NotificationPersistence>(ufw)
                                  select notif).ToList();
                    if (status.Count > 0)
                    {
                        lastValue = status[0].LastValue;
                        state = status[0].State;
                        svractive = status[0].svrActive;
                        alarmMessage = status[0].Message;
                        CurrentState = status[0].CurrentState;
                        CurrentWState = status[0].CurrentWState;
                        flMessageSent = status[0].flMessageSent;
                        ServerTime = status[0].ServerTime;
                        return true;
                    }
                }
            }
            return false;
        }
        internal void SaveNotificationStatus()
        {
            if (dl != null)
            {
                using (UnitOfWork ufw = new UnitOfWork(dl))
                {
                    var status = (from sched in new XPQuery<NotificationPersistence>(ufw)
                                  select sched).ToList();
                    if (status.Count > 0)
                        status[0].Delete();

                    var newstatus = new NotificationPersistence(ufw)
                    {
                        LastValue = lastValue,
                        State = state,
                        svrActive = svractive,
                        Message = alarmMessage,
                        CurrentState = CurrentState,
                        CurrentWState = CurrentWState,
                        flMessageSent = flMessageSent,
                        ServerTime = ServerTime
                    };
                    ufw.CommitChangesAndDropIdentityMap();
                }
            }
        }
        #endregion
    }
}
