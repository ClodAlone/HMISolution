using System;
using System.Linq;
using ViewModelLib;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Helpers;
#if !WINDOWS_UWP && !NET_STANDARD
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
#endif
using UFInterfaces;
using Utilities;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Windows.Input;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Threading;
using System.Text;

namespace OPCUAViewModel
{
    public class MonitoredItemViewModel : TreeViewItemViewModel, IEntityReference
    {
#region Members
        public MonitoredItem monitoredItem { get; set; }
        public ReferenceDescriptionViewModel referenceItem { get; protected set; }

        public static SafeObservableCollection<MonitoredItemViewModel> listActiveMonitoredItems = new SafeObservableCollection<MonitoredItemViewModel>();
        static Dictionary<String, MonitoredItemViewModel> mapActiveMonitoredItems = new Dictionary<String, MonitoredItemViewModel>();

        private static Dictionary<NodeId, NodeId> eventTypeMappings = new Dictionary<NodeId, NodeId>();

        List<IEntityReference> listSubscribed = new List<IEntityReference>();

        static DataSinkInterface sysVariables;

        //static readonly log4net.ILog log = log4net.LogManager.GetLogger("MyDebug");

        /// <summary>
        /// The known event types which can be constructed by ConstructEvent()
        /// </summary>
        public static NodeId[] KnownEventTypes = new NodeId[] 
        {
            ObjectTypeIds.BaseEventType,
            ObjectTypeIds.ConditionType,
            ObjectTypeIds.DialogConditionType,
            ObjectTypeIds.AlarmConditionType,
            ObjectTypeIds.ExclusiveLevelAlarmType,
            ObjectTypeIds.ExclusiveLimitAlarmType,
            ObjectTypeIds.NonExclusiveLimitAlarmType,
            ObjectTypeIds.NonExclusiveLevelAlarmType,
            ObjectTypeIds.LimitAlarmType,
            ObjectTypeIds.ExclusiveRateOfChangeAlarmType,
            ObjectTypeIds.ExclusiveDeviationAlarmType,
            ObjectTypeIds.NonExclusiveRateOfChangeAlarmType,
            ObjectTypeIds.NonExclusiveDeviationAlarmType,
            ObjectTypeIds.DiscreteAlarmType,
            ObjectTypeIds.OffNormalAlarmType,
            ObjectTypeIds.TripAlarmType,

            ObjectTypeIds.AuditEventType,
            ObjectTypeIds.AuditUpdateMethodEventType,

            ObjectTypeIds.SystemEventType,

            ObjectTypeIds.ProgressEventType,
            ObjectTypeIds.DeviceFailureEventType
        };

        readonly bool bIsReadOnly;
        readonly MonitoredItemViewModel referenceViewModel;
#endregion

#region Constructor
        public MonitoredItemViewModel(MonitoredItem mi,
            ReferenceDescriptionViewModel r, 
            TreeViewItemViewModel parent, bool bCreateConditionList = false)
            :base(parent, false)
        {
            if (mi == null)
                throw new ArgumentNullException("MonitoredItem");

            monitoredItem = mi;
            referenceItem = r;
            Title = monitoredItem.DisplayName;
            CreateUniqueName();

            if (mi.Handle == null)
                mi.Handle = this;

            if (bCreateConditionList)
            {
                _conditionStateList = new SafeObservableCollection<ConditionStateViewModel>(lockObject);
                _auditEventStateList = new SafeObservableCollection<AuditEventStateViewModel>(lockObject);
                _systemEventStateList = new SafeObservableCollection<SystemEventStateViewModel>(lockObject);
            }

            lock (mapActiveMonitoredItems)
            {
                if (!listActiveMonitoredItems.Contains(this))
                {
                    if (sysVariables == null)
                    {
                        sysVariables = OPCUAEntityReference.GetDataSinkInterface(SysVariables.SysNames.dataSynkName);
                        if (sysVariables != null)
                        {
                            listActiveMonitoredItems.CollectionChanged += (s, e) =>
                            {
                                sysVariables.UpdateVariable(SysVariables.SysNames.TotalConnectedClientTags, new DataValue(new Variant(listActiveMonitoredItems.Count), StatusCodes.Good));
                            };
                        }
                    }

                    listActiveMonitoredItems.Add(this);
                }

                mapActiveMonitoredItems.Add(GetComposedTitle(), this);
            }
        }

        public MonitoredItemViewModel(MonitoredItemViewModel reference)
            : base(null, false)
        {
            referenceViewModel = reference;
        }

        public MonitoredItemViewModel(bool isReadOnly = false)
            : base(null, false)
        {
            bIsReadOnly = isReadOnly;
        }

#endregion

#region Methods

        public SubscriptionViewModel GetSubscriptionViewModelParent()
        {
            TreeViewItemViewModel parent = Parent;
            while (parent != null && !(parent is SubscriptionViewModel))
                parent = Parent.Parent;

            return parent as SubscriptionViewModel;
        }

        void CreateUniqueName()
        {
            lock (mapActiveMonitoredItems)
            {
                String originalTitle = monitoredItem.DisplayName;
                int i = 1;
                while (mapActiveMonitoredItems.ContainsKey(GetComposedTitle()))
                    Title = monitoredItem.DisplayName = String.Format("{0} {1}", originalTitle, i++);
            }
        }

        public String GetComposedTitle()
        {
            SubscriptionViewModel parent = GetSubscriptionViewModelParent();
            return String.Format("{0}.{1}", parent.GetComposedTitle(), Title);
        }

        public static MonitoredItemViewModel GetMonitoredItemViewModel(String name)
        {
            MonitoredItemViewModel ret = null;

            lock (mapActiveMonitoredItems)
            {
                mapActiveMonitoredItems.TryGetValue(name, out ret);
            }

            return ret;
        }

        public void AddEntityReferences(IList<IEntityReference> list)
        {
            bool hasChildren = HasChildren;

            lock (lockObject)
            {
                listSubscribed.AddRange(list);

                if (mapChildItems != null)
                {
                    foreach (var v in mapChildItems.Values)
                        v.AddEntityReferences(list);
                }

                if (hasChildren)
                {
                    foreach (var child in Children)
                    {
                        if (!(child is MonitoredItemViewModel))
                            continue;
                        (child as MonitoredItemViewModel).AddEntityReferences(list);
                    }
                }
            }
        }

        public void RemoveEntityReferences(IList<IEntityReference> list)
        {
            bool hasChildren = HasChildren;

            lock (lockObject)
            {
                foreach (var item in list)
                    listSubscribed.Remove(item);

                if (mapChildItems != null)
                {
                    foreach (var v in mapChildItems.Values)
                        v.RemoveEntityReferences(list);
                }

                if (hasChildren)
                {
                    foreach (var child in Children)
                    {
                        if (!(child is MonitoredItemViewModel))
                            continue;
                        (child as MonitoredItemViewModel).RemoveEntityReferences(list);
                    }
                }
            }
        }

        public IList<IEntityReference> GetEntityReferences()
        {
            lock (lockObject)
            {
                return listSubscribed.AsReadOnly();
            }
        }

        bool bPendingCallAcknowledgeAll;
        void CallAcknowledgeAll()
        {
            var list = new List<ConditionStateViewModel>();
            lock (lockObject)
            {
                if (_ConditionStateViewModel != null)
                {
                    _ConditionStateViewModel.CallAcknowledgeAll();
                    return;
                }
                if (ConditionStateList != null)
                {
                    list.AddRange(ConditionStateList);
                }
            }

            bPendingCallAcknowledgeAll = true;
            var task1 = System.Threading.Tasks.Task.Factory.StartNew(delegate
            {
                try
                {
                    list.ForEach(v =>
                    {
                        try
                        {
                            if (v.IsEnableCallAcknowledge)
                                v.CallAcknowledge(true);
                        }
                        catch (Exception ex)
                        {

                        }
                    });
                }
                catch (Exception ex)
                {
                
                }
                bPendingCallAcknowledgeAll = false;
            });
#if !WINDOWS_UWP && !NET_STANDARD
            if (System.Threading.SynchronizationContext.Current != null)
            {
                var task2 = task1.ContinueWith(ret =>
                {
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
            }
#endif
        }

        bool bPendingCallConfirmAll;
        void CallConfirmAll()
        {
            var list = new List<ConditionStateViewModel>();
            lock (lockObject)
            {
                if (_ConditionStateViewModel != null)
                {
                    _ConditionStateViewModel.CallConfirmAll();
                    return;
                }
                if (ConditionStateList != null)
                {
                    list.AddRange(ConditionStateList);
                }
            }

            bPendingCallConfirmAll = true;
            var task1 = System.Threading.Tasks.Task.Factory.StartNew(delegate
            {
                try
                {
                    list.ForEach(v =>
                    {
                        try
                        {
                            if (v.IsEnableCallConfirm)
                                v.CallConfirm(true);
                        }
                        catch (Exception ex)
                        {

                        }
                    });
                }
                catch (Exception ex)
                {

                }
                bPendingCallConfirmAll = false;
            });
#if !WINDOWS_UWP && !NET_STANDARD
            if (System.Threading.SynchronizationContext.Current != null)
            {
                var task2 = task1.ContinueWith(ret =>
                {
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
            }
#endif
        }

        public IList<ConditionStateViewModel> GetCurrentConditionStateList(bool bSortByTimeDescending = false)
        {
            var list = new List<ConditionStateViewModel>();
            lock (lockObject)
            {
                if (_ConditionStateViewModel != null)
                    list.AddRange(_ConditionStateViewModel.GetCurrentConditionStateList());
                else if (ConditionStateList != null)
                    list.AddRange(ConditionStateList);
            }

            return bSortByTimeDescending ? list.OrderByDescending(x => x.ActiveTransitionTime).ToList() : list;
        }

        bool bDisposed;
        protected override void OnDispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;
            base.OnDispose();

            var subscription = GetSubscriptionViewModelParent();
            if (subscription != null)
                subscription.RemoveMonitoredItem(this);

            lock (lockObject)
            {
                if (listSubscribed != null)
                    listSubscribed.Clear();

                if (_ConditionStateViewModel != null)
                {
                    Children.Remove(_ConditionStateViewModel);
                    _ConditionStateViewModel.Dispose();
                }

                if (_AuditEventStateViewModel != null)
                {
                    Children.Remove(_AuditEventStateViewModel);
                    _AuditEventStateViewModel.Dispose();
                }

                if (_SystemEventStateViewModel != null)
                {
                    Children.Remove(_SystemEventStateViewModel);
                    _SystemEventStateViewModel.Dispose();
                }

                if (_conditionStateList != null)
                {
                    foreach (var v in _conditionStateList)
                        v.Dispose();
                    // _conditionStateList.Clear();
                    // _conditionStateList = null;
                    // _conditionStateMap = null;
                }

                if (_conditionStateMap != null)
                    _conditionStateMap.Clear();

                if (_auditEventStateList != null)
                {
                    foreach (var v in _auditEventStateList)
                        v.Dispose();
                    // _auditEventStateList.Clear();
                    // _auditEventStateList = null;
                }

                if (_systemEventStateList != null)
                {
                    foreach (var v in _systemEventStateList)
                        v.Dispose();
                    // _systemEventStateList.Clear();
                    // _systemEventStateList = null;
                }

                if (mapChildItems != null)
                {
                    foreach (var v in mapChildItems.Values)
                        v.Dispose();
                    //mapChildItems.Clear();
                    mapChildItems = null;
                }

                if (historyReadModel != null)
                {
                    historyReadModel.Dispose();
                    historyReadModel = null;
                }

                if (monitoredItem != null)
                    monitoredItem.Handle = null;
            }

            lock (mapActiveMonitoredItems)
            {
                if (listActiveMonitoredItems.Contains(this))
                    listActiveMonitoredItems.Remove(this);

                var key = (from c in mapActiveMonitoredItems where c.Value == this select c.Key).ToList();
                if (key.Count > 0)
                    mapActiveMonitoredItems.Remove(key[0]);
            }

#if !WINDOWS_UWP
            lock (lockData)
            {
                if (dl != null)
                {
                    dl.Dispose();
                    dl = null;
                }
            }
#endif
            if (nodeIdModel != null)
            {
                nodeIdModel.Dispose();
                nodeIdModel = null;
            }
        }

        private String ResolveValue(DataValue datavalue)
        {
            try
            {
                if (EnumStrings != null)
                {
                    uint value = Convert.ToUInt32(datavalue.Value);
                    return EnumStrings[value].Text;
                }
                else if (NodeIdModel.TrueState != null && NodeIdModel.FalseState != null)
                {
                    bool value = Convert.ToBoolean(datavalue.Value);
                    return value ? NodeIdModel.TrueState.Text : NodeIdModel.FalseState.Text;
                }
                else
                    return String.Format("{0}", datavalue.WrappedValue);
            }
            catch (Exception ex)
            {
                return String.Format("{0}", datavalue.WrappedValue);
            }
        }

        private bool TryParseValue(String value, out Object result)
        {
            result = null;
            if (EnumStrings != null)
            {
                for (int ii = 0; ii < EnumStrings.Length; ii++)
                {
                    if (EnumStrings[ii].Text == value)
                    {
                        result = ii;
                        break;
                    }
                }
            }
            else if (NodeIdModel != null && NodeIdModel.TrueState != null && NodeIdModel.TrueState.Text == value)
                result = true;
            else if (NodeIdModel != null && NodeIdModel.FalseState != null && NodeIdModel.FalseState.Text == value)
                result = false;

            return result != null;
        }

        public static BuiltInType GetBuiltInType(string typeName)
        {
            switch (typeName)
            {
                case "Boolean": return BuiltInType.Boolean;
                case "SByte": return BuiltInType.SByte;
                case "Byte": return BuiltInType.Byte;
                case "Int16": return BuiltInType.Int16;
                case "UInt16": return BuiltInType.UInt16;
                case "Int32": return BuiltInType.Int32;
                case "UInt32": return BuiltInType.UInt32;
                case "Int64": return BuiltInType.Int64;
                case "UInt64": return BuiltInType.UInt64;
                case "Float": return BuiltInType.Float;
                case "Single": return BuiltInType.Float;
                case "Double": return BuiltInType.Double;
                case "String": return BuiltInType.String;
                case "DateTime": return BuiltInType.DateTime;
                case "Guid": return BuiltInType.Guid;
                case "Uuid": return BuiltInType.Guid;
                case "ByteString": return BuiltInType.ByteString;
                case "XmlElement": return BuiltInType.XmlElement;
                case "NodeId": return BuiltInType.NodeId;
                case "ExpandedNodeId": return BuiltInType.ExpandedNodeId;
                case "LocalizedText": return BuiltInType.LocalizedText;
                case "QualifiedName": return BuiltInType.QualifiedName;
                case "StatusCode": return BuiltInType.StatusCode;
                case "DiagnosticInfo": return BuiltInType.DiagnosticInfo;
                case "DataValue": return BuiltInType.DataValue;
                case "Variant": return BuiltInType.Variant;
                case "ExtensionObject": return BuiltInType.ExtensionObject;
                case "Object": return BuiltInType.Variant;
            }

            return BuiltInType.Null;
        }

        public bool WriteValue(Object v)
        {
            return WriteValue(v, -1, -1);
        }

        public bool WriteValue(Object v, int indexArray, int indexBit)
        {
            try
            {
                return WriteValue(v, indexArray, indexBit, useIndexRange: true);
            }
            catch (ServiceResultException ex)
            {
                if ((indexArray >= 0 || indexBit >= 0) && ex.StatusCode == StatusCodes.BadWriteNotSupported)
                {
                    // opc ua server doesn't support write via IndexRange.
                    return WriteValue(v, -1, indexBit, useIndexRange: false);
                }
                else if (indexBit >= 0 && (ex.StatusCode == StatusCodes.BadTypeMismatch ||
                    ex.StatusCode == StatusCodes.BadIndexRangeInvalid ||
                    ex.StatusCode == StatusCodes.BadIndexRangeNoData))
                {
                    // opc ua server doesn't support write via IndexRange for bits.
                    return WriteValue(v, indexArray, indexBit, useIndexRange: false);
                }
                throw;
            }
        }

        bool WriteValue(Object v, int indexArray, int indexBit, bool useIndexRange)
        {
            WriteValue value = null;
            lock (lockObject)
            {
                BuiltInType builtinType;
                if (monitoredItem == null)
                {
                    if (bIsReadOnly)
                        throw new Exception("This is a temporary reaonly item, please wait for the real connected item to write");

                    if (_dataValue == null)
                        DataValue = new DataValue(new Variant(v), StatusCodes.Good);
                    else if (v != null && _dataValue.Value != null)
                    {
                        try
                        {
                            uint arraydimension = 0;
                            Type type;
                            if (_dataValue.Value is Array)
                            {
                                arraydimension = (uint)(_dataValue.Value as Array).GetUpperBound(0) + 1;
                                type = (_dataValue.Value as Array).GetValue(0).GetType();
                            }
                            else
                                type = _dataValue.Value.GetType();

                            builtinType = GetBuiltInType(type.Name);
                            if (builtinType == BuiltInType.Double || builtinType == BuiltInType.Float)
                            {
                                if (v is string)
                                {
                                    CultureInfo culture = new CultureInfo(System.Globalization.CultureInfo.CurrentCulture.Name, true);
                                    v = (v as string).Replace(culture.NumberFormat.NumberDecimalSeparator, ".");
                                }
                            }
                            try
                            {
                                try
                                {
                                    v = ChangeTypeHelper.ChangeType(v, builtinType, arraydimension); // prechange type base on currenthread localization first
                                }
                                catch (Exception)
                                {
                                    if (v is String && builtinType != BuiltInType.String &&
                                        (builtinType == BuiltInType.Int64 || builtinType == BuiltInType.UInt64))
                                    {
                                        System.Globalization.NumberFormatInfo info = new System.Globalization.NumberFormatInfo();
                                        info.NumberDecimalSeparator = ".";
                                        info.NumberGroupSeparator = ",";
                                        v = Convert.ToInt64(v, info);
                                    }

                                    string binary = null;
                                    switch (builtinType)
                                    {
                                        case BuiltInType.Byte:
                                            binary = Convert.ToString((long)v, 2);
                                            binary = binary.Substring(Math.Max(0, binary.Length - 8));
                                            v = Convert.ToByte(binary, 2);
                                            break;
                                        case BuiltInType.SByte:
                                            binary = Convert.ToString((long)v, 2);
                                            binary = binary.Substring(Math.Max(0, binary.Length - 8));
                                            v = Convert.ToSByte(binary, 2);
                                            break;
                                        case BuiltInType.Int16:
                                            binary = Convert.ToString((long)v, 2);
                                            binary = binary.Substring(Math.Max(0, binary.Length - 16));
                                            v = Convert.ToInt16(binary, 2);
                                            break;
                                        case BuiltInType.UInt16:
                                            binary = Convert.ToString((long)v, 2);
                                            binary = binary.Substring(Math.Max(0, binary.Length - 16));
                                            v = Convert.ToUInt16(binary, 2);
                                            break;
                                        case BuiltInType.Int32:
                                            binary = Convert.ToString((long)v, 2);
                                            binary = binary.Substring(Math.Max(0, binary.Length - 32));
                                            v = Convert.ToInt32(binary, 2);
                                            break;
                                        case BuiltInType.UInt32:
                                            binary = Convert.ToString((long)v, 2);
                                            binary = binary.Substring(Math.Max(0, binary.Length - 32));
                                            v = Convert.ToUInt32(binary, 2);
                                            break;
                                        case BuiltInType.Int64:
                                        case BuiltInType.Integer:
                                            binary = Convert.ToString((Int64)v, 2);
                                            // binary = binary.Substring(Math.Max(0, binary.Length - 63));
                                            v = Convert.ToInt64(binary, 2);
                                            break;
                                        case BuiltInType.UInt64:
                                        case BuiltInType.UInteger:
                                            binary = Convert.ToString((Int64)v, 2);
                                            // binary = binary.Substring(Math.Max(0, binary.Length - 63));
                                            v = Convert.ToUInt64(binary, 2);
                                            break;
                                    }
                                }
                                if (arraydimension == 0)
                                {
                                    if (indexBit >= 0 && Opc.Ua.TypeInfo.IsNumericType(builtinType))
                                    {
                                        bool bit;
                                        v = GetNewValue(v, _dataValue.Value, indexBit, builtinType, out bit);
                                        DataValue = new DataValue(new Variant(v), StatusCodes.Good, DateTime.UtcNow);
                                    }
                                    else
                                        DataValue = new DataValue(new Variant(Opc.Ua.TypeInfo.Cast(v, builtinType)), StatusCodes.Good, DateTime.UtcNow);
                                }
                                else if (v is Array)
                                {
                                    var array = v as Array;
                                    if (indexArray >= 0 && _dataValue.Value is Array)
                                    {
                                        var dValue = _dataValue.Value as Array;
                                        var vValue = array.GetValue(indexArray);
                                        if (indexBit >= 0 && Opc.Ua.TypeInfo.IsNumericType(builtinType))
                                        {
                                            bool bit;
                                            vValue = GetNewValue(vValue, dValue.GetValue(indexArray), indexBit, builtinType, out bit);
                                        }
                                        dValue.SetValue(vValue, indexArray);
                                        DataValue = new DataValue(new Variant(Opc.Ua.TypeInfo.CastArray(dValue, builtinType, builtinType, ChangeTypeHelper.CastArrayElement)), StatusCodes.Good, DateTime.UtcNow);
                                    }
                                    else
                                        DataValue = new DataValue(new Variant(Opc.Ua.TypeInfo.CastArray(array, builtinType, builtinType, ChangeTypeHelper.CastArrayElement)), StatusCodes.Good, DateTime.UtcNow);
                                }
                            }
                            catch (Exception ex)
                            {
                                LastMessage = String.Format("{0} - {1}, Value : {2}, Error : {3}", Title, Properties.Resource.OPCUAMonitoredItemWriteError, Value, ex.Message);
                                Utils.Trace(ex, Properties.Resource.MonitoredTemporaryItemWriteError);
                                throw;
                            }
                        }
                        catch (Exception)
                        {
                            //throw new InvalidCastException(String.Format("Cannot cast the value '{0}' to type {1}", v, _dataValue.Value.GetType()));
                            throw;
                        }
                    }

                    return true;
                }

                //Node node = monitoredItem.Subscription.Session.NodeCache.Find(monitoredItem.ResolvedNodeId) as Node;

                //if (node == null)
                //    return false;

                if (!Properties.Settings.Default.UseIndexRangeForBitsOnWriting)
                    useIndexRange = false;

                value = new WriteValue
                {
                    NodeId = monitoredItem.ResolvedNodeId,
                    AttributeId = Attributes.Value,
                    IndexRange = null
                };

                // read the display name for non-variables.
                if ((monitoredItem.NodeClass & (NodeClass.Variable | NodeClass.VariableType)) == 0)
                {
                    value.AttributeId = Attributes.DisplayName;
                }

                NodeId datatypeId = Attributes.GetDataTypeId(monitoredItem.AttributeId);
                int valueRank = Attributes.GetValueRank(monitoredItem.AttributeId);
                uint arraySizeOneDimension = 0;

                Session session = null;
                var subscription = GetSubscriptionViewModelParent();
                if (subscription != null)
                    session = subscription.GetSessionViewModelParent()?.Session;
                if (session == null)
                    throw new NullReferenceException("session cannot be null while writing a new value.");

                if (monitoredItem.AttributeId == Attributes.Value)
                {
                    if (session != null)
                    {
                        var vnode = session.NodeCache.Find(value.NodeId) as VariableNode;
                        if (vnode != null)
                        {
                            datatypeId = vnode.DataType;
                            valueRank = vnode.ValueRank;
                            arraySizeOneDimension = (vnode.ArrayDimensions != null && vnode.ArrayDimensions.Count > 0) ? vnode.ArrayDimensions[0] : 0;
                        }
                    }
                }

                builtinType = Opc.Ua.TypeInfo.GetBuiltInType(datatypeId, session.TypeTree);
                object newValue = null;

                try
                {
                    v = ChangeTypeHelper.ChangeType(v, builtinType, arraySizeOneDimension); // prechange type base on currenthread localization first
                    if (builtinType == BuiltInType.ExtensionObject)
                    {
                        value.Value = (Opc.Ua.DataValue)v;
                    }
                    else if (arraySizeOneDimension == 0)
                    {
                        if (indexBit >= 0 && Opc.Ua.TypeInfo.IsNumericType(builtinType))
                        {
                            if (useIndexRange)
                            {
                                bool bit;
                                newValue = GetNewValue(v, DataValue?.Value, indexBit, builtinType, out bit);
                                value.IndexRange = Convert.ToString(indexBit, CultureInfo.InvariantCulture);
                                value.Value = new DataValue(new Variant(Opc.Ua.TypeInfo.Cast(bit, builtinType)));
                            }
                            else
                            {
                                bool bit;
                                var datavalue = session.ReadValue(monitoredItem.ResolvedNodeId);
                                v = GetNewValue(v, datavalue?.Value, indexBit, builtinType, out bit);
                                value.Value = new DataValue(new Variant(v));
                            }
                        }
                        else
                            value.Value = new DataValue(new Variant(Opc.Ua.TypeInfo.Cast(v, builtinType)));
                    }
                    else if (v is Array)
                    {
                        var array = v as Array;
                        if (indexArray >= 0)
                        {
                            var vValue = array.GetValue(indexArray);
                            string subrange = null;
                            if (indexBit >= 0 && Opc.Ua.TypeInfo.IsNumericType(builtinType))
                            {
                                if (useIndexRange)
                                {
                                    bool bit;
                                    object currentValue = null;
                                    if (DataValue != null && DataValue.Value is Array)
                                        currentValue = (DataValue.Value as Array).GetValue(indexArray);
                                    vValue = GetNewValue(vValue, currentValue, indexBit, builtinType, out bit);
                                    subrange = Convert.ToString(indexBit, CultureInfo.InvariantCulture);
                                    array.SetValue(vValue, indexArray);
                                    value.Value = new DataValue(new Variant(Opc.Ua.TypeInfo.Cast(bit, builtinType)));
                                    if (DataValue != null && DataValue.Value is Array)
                                    {
                                        newValue = Opc.Ua.TypeInfo.CastArray(DataValue.Value as Array, builtinType, builtinType, ChangeTypeHelper.CastArrayElement);
                                        (newValue as Array).SetValue(vValue, indexArray);
                                    }
                                }
                                else
                                {
                                    bool bit;
                                    object currentValue = null;
                                    var datavalue = session.ReadValue(monitoredItem.ResolvedNodeId);
                                    if (datavalue != null && datavalue.Value is Array)
                                        currentValue = (datavalue.Value as Array).GetValue(indexArray);
                                    vValue = GetNewValue(vValue, currentValue, indexBit, builtinType, out bit);
                                    array.SetValue(vValue, indexArray);
                                    var element = Opc.Ua.TypeInfo.CreateArray(builtinType, 1);
                                    element.SetValue(vValue, 0);
                                    value.Value = new DataValue(new Variant(element));
                                    if (datavalue != null && datavalue.Value is Array)
                                    {
                                        newValue = Opc.Ua.TypeInfo.CastArray(datavalue.Value as Array, builtinType, builtinType, ChangeTypeHelper.CastArrayElement);
                                        (newValue as Array).SetValue(vValue, indexArray);
                                    }
                                }
                            }
                            else
                            {
                                var element = Opc.Ua.TypeInfo.CreateArray(builtinType, 1);
                                element.SetValue(vValue, 0);
                                value.Value = new DataValue(new Variant(element));
                                if (DataValue != null && DataValue.Value is Array)
                                {
                                    newValue = Opc.Ua.TypeInfo.CastArray(DataValue.Value as Array, builtinType, builtinType, ChangeTypeHelper.CastArrayElement);
                                    (newValue as Array).SetValue(vValue, indexArray);
                                }
                            }
                            value.IndexRange = Convert.ToString(indexArray, CultureInfo.InvariantCulture);
                            if (subrange != null)
                                value.IndexRange = String.Format("{0},{1}", value.IndexRange, subrange);
                            if (newValue == null)
                                newValue = Opc.Ua.TypeInfo.CastArray(array, builtinType, builtinType, ChangeTypeHelper.CastArrayElement);
                        }
                        else
                            value.Value = new DataValue(new Variant(Opc.Ua.TypeInfo.CastArray(array, builtinType, builtinType, ChangeTypeHelper.CastArrayElement)));
                    }
                }
                catch (Exception exception)
                {
                    try
                    {
                        if (v is String && builtinType != BuiltInType.String && 
                            (builtinType == BuiltInType.Int64 || builtinType == BuiltInType.UInt64))
                        {
                            System.Globalization.NumberFormatInfo info = new System.Globalization.NumberFormatInfo();
                            info.NumberDecimalSeparator = ".";
                            info.NumberGroupSeparator = ",";
                            v = Convert.ToInt64(v, info);
                        }

                        string binary = null;
                        switch (builtinType)
                        {
                            case BuiltInType.Byte:
                                binary = Convert.ToString((long)v, 2);
                                binary = binary.Substring(Math.Max(0, binary.Length - 8));
                                v = Convert.ToByte(binary, 2); 
                                break;
                            case BuiltInType.SByte:
                                binary = Convert.ToString((long)v, 2);
                                binary = binary.Substring(Math.Max(0, binary.Length - 8));
                                v = Convert.ToSByte(binary, 2);
                                break;
                            case BuiltInType.Int16:
                                binary = Convert.ToString((long)v, 2);
                                binary = binary.Substring(Math.Max(0, binary.Length - 16));
                                v = Convert.ToInt16(binary, 2); 
                                break;
                            case BuiltInType.UInt16:
                                binary = Convert.ToString((long)v, 2);
                                binary = binary.Substring(Math.Max(0, binary.Length - 16));
                                v = Convert.ToUInt16(binary, 2); 
                                break;
                            case BuiltInType.Int32:
                                binary = Convert.ToString((long)v, 2);
                                binary = binary.Substring(Math.Max(0, binary.Length - 32));
                                v = Convert.ToInt32(binary, 2); 
                                break;
                            case BuiltInType.UInt32:
                                binary = Convert.ToString((long)v, 2);
                                binary = binary.Substring(Math.Max(0, binary.Length - 32));
                                v = Convert.ToUInt32(binary, 2); 
                                break;
                            case BuiltInType.Int64:
                            case BuiltInType.Integer:
                                binary = Convert.ToString((Int64)v, 2);
                                // binary = binary.Substring(Math.Max(0, binary.Length - 63));
                                v = Convert.ToInt64(binary, 2); 
                                break;
                            case BuiltInType.UInt64:
                            case BuiltInType.UInteger:
                                binary = Convert.ToString((Int64)v, 2);
                                // binary = binary.Substring(Math.Max(0, binary.Length - 63));
                                v = Convert.ToUInt64(binary, 2); 
                                break;
                        }
                        if (arraySizeOneDimension == 0)
                        {
                            if (indexBit >= 0 && Opc.Ua.TypeInfo.IsNumericType(builtinType))
                            {
                                if (useIndexRange)
                                {
                                    bool bit;
                                    newValue = GetNewValue(v, DataValue?.Value, indexBit, builtinType, out bit);
                                    value.IndexRange = Convert.ToString(indexBit, CultureInfo.InvariantCulture);
                                    value.Value = new DataValue(new Variant(Opc.Ua.TypeInfo.Cast(bit, builtinType)));
                                }
                                else
                                {
                                    bool bit;
                                    var datavalue = session.ReadValue(monitoredItem.ResolvedNodeId);
                                    v = GetNewValue(v, datavalue?.Value, indexBit, builtinType, out bit);
                                    value.Value = new DataValue(new Variant(v));
                                }
                            }
                            else
                                value.Value = new DataValue(new Variant(Opc.Ua.TypeInfo.Cast(v, builtinType)));
                        }
                        else if (v is Array)
                        {
                            var array = v as Array;
                            if (indexArray >= 0)
                            {
                                var vValue = array.GetValue(indexArray);
                                string subrange = null;
                                if (indexBit >= 0 && Opc.Ua.TypeInfo.IsNumericType(builtinType))
                                {
                                    if (useIndexRange)
                                    {
                                        bool bit;
                                        object currentValue = null;
                                        if (DataValue != null && DataValue.Value is Array)
                                            currentValue = (DataValue.Value as Array).GetValue(indexArray);
                                        vValue = GetNewValue(vValue, currentValue, indexBit, builtinType, out bit);
                                        subrange = Convert.ToString(indexBit, CultureInfo.InvariantCulture);
                                        array.SetValue(vValue, indexArray);
                                        value.Value = new DataValue(new Variant(Opc.Ua.TypeInfo.Cast(bit, builtinType)));
                                        if (DataValue != null && DataValue.Value is Array)
                                        {
                                            newValue = Opc.Ua.TypeInfo.CastArray(DataValue.Value as Array, builtinType, builtinType, ChangeTypeHelper.CastArrayElement);
                                            (newValue as Array).SetValue(vValue, indexArray);
                                        }
                                    }
                                    else
                                    {
                                        bool bit;
                                        object currentValue = null;
                                        var datavalue = session.ReadValue(monitoredItem.ResolvedNodeId);
                                        if (datavalue != null && datavalue.Value is Array)
                                            currentValue = (datavalue.Value as Array).GetValue(indexArray);
                                        vValue = GetNewValue(vValue, currentValue, indexBit, builtinType, out bit);
                                        array.SetValue(vValue, indexArray);
                                        var element = Opc.Ua.TypeInfo.CreateArray(builtinType, 1);
                                        element.SetValue(vValue, 0);
                                        value.Value = new DataValue(new Variant(element));
                                        if (datavalue != null && datavalue.Value is Array)
                                        {
                                            newValue = Opc.Ua.TypeInfo.CastArray(datavalue.Value as Array, builtinType, builtinType, ChangeTypeHelper.CastArrayElement);
                                            (newValue as Array).SetValue(vValue, indexArray);
                                        }
                                    }
                                }
                                else
                                {
                                    var element = Opc.Ua.TypeInfo.CreateArray(builtinType, 1);
                                    element.SetValue(vValue, 0);
                                    value.Value = new DataValue(new Variant(element));
                                    if (DataValue != null && DataValue.Value is Array)
                                    {
                                        newValue = Opc.Ua.TypeInfo.CastArray(DataValue.Value as Array, builtinType, builtinType, ChangeTypeHelper.CastArrayElement);
                                        (newValue as Array).SetValue(vValue, indexArray);
                                    }
                                }
                                value.IndexRange = Convert.ToString(indexArray, CultureInfo.InvariantCulture);
                                if (subrange != null)
                                    value.IndexRange = String.Format("{0},{1}", value.IndexRange, subrange);
                                if (newValue == null)
                                    newValue = Opc.Ua.TypeInfo.CastArray(array, builtinType, builtinType, ChangeTypeHelper.CastArrayElement);
                            }
                            else
                                value.Value = new DataValue(new Variant(Opc.Ua.TypeInfo.CastArray(array, builtinType, builtinType, ChangeTypeHelper.CastArrayElement)));
                        }
                        else
                            throw;
                    }
                    catch (Exception ex)
                    {
                        LastMessage = String.Format("{0} - {1}, Value : {2}, Error : {3}", Title, Properties.Resource.OPCUAMonitoredItemWriteError, Value, ex.Message);
                        Utils.Trace(ex, Properties.Resource.OPCUAMonitoredItemWriteError);
                        throw;
                    }
                }

                value.Value.StatusCode = StatusCodes.Good;
                value.Value.ServerTimestamp = DateTime.MinValue;
                value.Value.SourceTimestamp = DateTime.MinValue;

                WriteValueCollection values = new WriteValueCollection();
                values.Add(value);

                StatusCodeCollection results = null;
                DiagnosticInfoCollection diagnosticInfos = null;

                try
                {
                    ResponseHeader responseHeader = session.Write(
                        null,
                        values,
                        out results,
                        out diagnosticInfos);

                    ClientBase.ValidateResponse(results, values);
                    ClientBase.ValidateDiagnosticInfos(diagnosticInfos, values);

                    foreach (StatusCode sc in results)
                    {
                        if (StatusCode.IsGood(sc))
                        {
                            // LastMessage = String.Format("{0} - {1}", Title, Properties.Resource.OPCUAMonitoredItemWriteSuccessful);
                            // AddOutputText(String.Format("Write Successfully to {0}", mi.DisplayName), false);
                            if (_dataValue != null && StatusCode.IsGood(_dataValue.StatusCode))
                                LastMessage = String.Empty;
                        }
                        else
                        {
                            LastMessage = String.Format("{0} - {1}, Reason : {2}", Title, Properties.Resource.OPCUAMonitoredItemWriteFailure, sc);
                            throw new ServiceResultException(new ServiceResult(sc, null, responseHeader.StringTable));
                        }
                    }
                }
                catch (Exception ex)
                {
                    LastMessage = String.Format("{0} - {1}, Value : {2}, Error : {3}", Title, Properties.Resource.OPCUAMonitoredItemWriteError, Value, ex.Message);
                    Utils.Trace(ex, Properties.Resource.OPCUAMonitoredItemWriteError);
                    throw;
                }

                //if (_dataValue != null)
                //    _dataValue.Value = value.Value.Value;
                if (DataValue != null)
                {
                    value.Value.ServerTimestamp = DataValue.ServerTimestamp;
                    value.Value.SourcePicoseconds = DataValue.SourcePicoseconds;
                    value.Value.SourceTimestamp = DataValue.SourceTimestamp;
                    value.Value.SourcePicoseconds = DataValue.SourcePicoseconds;
                }

                if (newValue != null)
                    value.Value.Value = newValue;

                //log.DebugFormat("WriteValue : old value = {0}, new vaue = {1}, Time = {2}", DataValue, value.Value, DateTime.UtcNow);
                DataValue = value.Value;
            }
            return true;
        }

        object GetNewValue(object v, object currentValue, int indexBit, BuiltInType builtinType, out bool bit)
        {
            switch (builtinType)
            {
                case BuiltInType.Byte:
                    {
                        if (indexBit > 7)
                            throw new IndexOutOfRangeException(String.Format(Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        byte lValue = System.Convert.ToByte(v);
                        byte shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = System.Convert.ToByte(currentValue);
                            if (bit)
                                lValue |= (byte)((byte)1 << indexBit);
                            else
                                lValue &= (byte)(~((byte)1 << indexBit));
                            return Opc.Ua.TypeInfo.Cast(lValue, builtinType);
                        }
                        else
                            return Opc.Ua.TypeInfo.Cast(v, builtinType);
                    }
                case BuiltInType.SByte:
                    {
                        if (indexBit > 7)
                            throw new IndexOutOfRangeException(String.Format(Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        sbyte lValue = System.Convert.ToSByte(v);
                        sbyte shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = System.Convert.ToSByte(currentValue);
                            if (bit)
                                lValue |= (sbyte)((sbyte)1 << indexBit);
                            else
                                lValue &= (sbyte)(~((sbyte)1 << indexBit));
                            return Opc.Ua.TypeInfo.Cast(lValue, builtinType);
                        }
                        else
                            return Opc.Ua.TypeInfo.Cast(v, builtinType);
                    }
                case BuiltInType.Int16:
                    {
                        if (indexBit > 15)
                            throw new IndexOutOfRangeException(String.Format(Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        short lValue = System.Convert.ToInt16(v);
                        short shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = System.Convert.ToInt16(currentValue);
                            if (bit)
                                lValue |= (short)((short)1 << indexBit);
                            else
                                lValue &= (short)(~((short)1 << indexBit));
                            return Opc.Ua.TypeInfo.Cast(lValue, builtinType);
                        }
                        else
                            return Opc.Ua.TypeInfo.Cast(v, builtinType);
                    }
                case BuiltInType.UInt16:
                    {
                        if (indexBit > 15)
                            throw new IndexOutOfRangeException(String.Format(Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        ushort lValue = System.Convert.ToUInt16(v);
                        ushort shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = System.Convert.ToUInt16(currentValue);
                            if (bit)
                                lValue |= (ushort)((ushort)1 << indexBit);
                            else
                                lValue &= (ushort)(~((ushort)1 << indexBit));
                            return Opc.Ua.TypeInfo.Cast(lValue, builtinType);
                        }
                        else
                            return Opc.Ua.TypeInfo.Cast(v, builtinType);
                    }
                case BuiltInType.Int32:
                    {
                        if (indexBit > 31)
                            throw new IndexOutOfRangeException(String.Format(Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        int lValue = System.Convert.ToInt32(v);
                        int shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = System.Convert.ToInt32(currentValue);
                            if (bit)
                                lValue |= (int)((int)1 << indexBit);
                            else
                                lValue &= (int)(~((int)1 << indexBit));
                            return Opc.Ua.TypeInfo.Cast(lValue, builtinType);
                        }
                        else
                            return Opc.Ua.TypeInfo.Cast(v, builtinType);
                    }
                case BuiltInType.UInt32:
                    {
                        if (indexBit > 31)
                            throw new IndexOutOfRangeException(String.Format(Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        uint lValue = System.Convert.ToUInt32(v);
                        uint shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = System.Convert.ToUInt32(currentValue);
                            if (bit)
                                lValue |= (uint)((uint)1 << indexBit);
                            else
                                lValue &= (uint)(~((uint)1 << indexBit));
                            return Opc.Ua.TypeInfo.Cast(lValue, builtinType);
                        }
                        else
                            return Opc.Ua.TypeInfo.Cast(v, builtinType);
                    }
                case BuiltInType.Int64:
                case BuiltInType.Integer:
                    {
                        if (indexBit > 63)
                            throw new IndexOutOfRangeException(String.Format(Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        long lValue = System.Convert.ToInt64(v);
                        long shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = System.Convert.ToInt64(currentValue);
                            if (bit)
                                lValue |= (long)((long)1 << indexBit);
                            else
                                lValue &= (long)(~((long)1 << indexBit));
                            return Opc.Ua.TypeInfo.Cast(lValue, builtinType);
                        }
                        else
                            return Opc.Ua.TypeInfo.Cast(v, builtinType);
                    }
                case BuiltInType.UInt64:
                case BuiltInType.UInteger:
                    {
                        if (indexBit > 63)
                            throw new IndexOutOfRangeException(String.Format(Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        ulong lValue = System.Convert.ToUInt64(v);
                        ulong shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = System.Convert.ToUInt64(currentValue);
                            if (bit)
                                lValue |= (ulong)((ulong)1 << indexBit);
                            else
                                lValue &= (ulong)(~((ulong)1 << indexBit));
                            return Opc.Ua.TypeInfo.Cast(lValue, builtinType);
                        }
                        else
                            return Opc.Ua.TypeInfo.Cast(v, builtinType);
                    }
                case BuiltInType.Float:
                    {
                        if (indexBit > 31)
                            throw new IndexOutOfRangeException(String.Format(Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        long lValue = (long)System.Convert.ToSingle(v);
                        long shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = (long)System.Convert.ToSingle(currentValue);
                            if (bit)
                                lValue |= (long)((long)1 << indexBit);
                            else
                                lValue &= (long)(~((long)1 << indexBit));
                            return Opc.Ua.TypeInfo.Cast(lValue, builtinType);
                        }
                        else
                            return Opc.Ua.TypeInfo.Cast(v, builtinType);
                    }
                case BuiltInType.Double:
                case BuiltInType.Number:
                    {
                        if (indexBit > 63)
                            throw new IndexOutOfRangeException(String.Format(Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        long lValue = (long)System.Convert.ToDouble(v);
                        long shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = (long)System.Convert.ToDouble(currentValue);
                            if (bit)
                                lValue |= (long)((long)1 << indexBit);
                            else
                                lValue &= (long)(~((long)1 << indexBit));
                            return Opc.Ua.TypeInfo.Cast(lValue, builtinType);
                        }
                        else
                            return Opc.Ua.TypeInfo.Cast(v, builtinType);
                    }
                default:
                    {
                        bit = false;
                        return v;
                    }
            }
        }

        public NodeId FindEventType(EventFieldList notification)
        {
            EventFilter filter = monitoredItem.Status.Filter as EventFilter;

            if (filter != null)
            {
                for (int ii = 0; ii < filter.SelectClauses.Count; ii++)
                {
                    SimpleAttributeOperand clause = filter.SelectClauses[ii];

                    if (clause.BrowsePath.Count == 1 && clause.BrowsePath[0] == BrowseNames.EventType)
                    {
                        return notification.EventFields[ii].Value as NodeId;
                    }
                }
            }

            return null;
        }

        public BaseEventState ConstructEvent(EventFieldList notification)
        {
            // find the event type.
            NodeId eventTypeId = FindEventType(notification);

            if (eventTypeId == null)
                return null;

            // look up the known event type.
            NodeId knownTypeId = null;

            lock (lockObject)
            {
                if (!eventTypeMappings.TryGetValue(eventTypeId, out knownTypeId))
                {
                    // check for a known type
                    for (int jj = 0; jj < KnownEventTypes.Length; jj++)
                    {
                        if (KnownEventTypes[jj] != eventTypeId)
                            continue;

                        knownTypeId = eventTypeId;
                        eventTypeMappings.Add(eventTypeId, eventTypeId);
                        break;
                    }

                    // browse for the supertypes of the event type.
                    if (knownTypeId == null)
                    {
                        ReferenceDescriptionCollection supertypes = BrowserViewModel.BrowseSuperTypes(monitoredItem.Subscription.Session, eventTypeId, false);

                        // can't do anything with unknown types.
                        if (supertypes == null)
                            return null;

                        // find the first supertype that matches a known event type.
                        for (int ii = 0; ii < supertypes.Count; ii++)
                        {
                            for (int jj = 0; jj < KnownEventTypes.Length; jj++)
                            {
                                if (KnownEventTypes[jj] != supertypes[ii].NodeId)
                                    continue;

                                knownTypeId = ExpandedNodeId.ToNodeId(supertypes[ii].NodeId, monitoredItem.Subscription.Session.NamespaceUris);
                                eventTypeMappings.Add(eventTypeId, knownTypeId);
                                break;
                            }

                            if (knownTypeId != null)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            // all of the known event types have a UInt32 as identifier.
            uint? id = knownTypeId.Identifier as uint?;

            if (id == null)
            {
                return null;
            }

            // construct the event based on the known event type.
            BaseEventState e = null;

            switch (id.Value)
            {
                case ObjectTypes.ConditionType: { e = new ConditionState(null); break; }
                case ObjectTypes.DialogConditionType: { e = new DialogConditionState(null); break; }
                case ObjectTypes.AlarmConditionType: { e = new AlarmConditionState(null); break; }
                case ObjectTypes.NonExclusiveLimitAlarmType: { e = new NonExclusiveLimitAlarmState(null); break; }
                case ObjectTypes.NonExclusiveLevelAlarmType: { e = new NonExclusiveLevelAlarmState(null); break; }
                case ObjectTypes.LimitAlarmType: { e = new LimitAlarmState(null); break; }
                case ObjectTypes.ExclusiveLimitAlarmType: { e = new ExclusiveLimitAlarmState(null); break; }
                case ObjectTypes.ExclusiveLevelAlarmType: { e = new ExclusiveLevelAlarmState(null); break; }
                case ObjectTypes.ExclusiveRateOfChangeAlarmType: { e = new ExclusiveRateOfChangeAlarmState(null); break; }
                case ObjectTypes.ExclusiveDeviationAlarmType: { e = new ExclusiveLevelAlarmState(null); break; }
                case ObjectTypes.NonExclusiveRateOfChangeAlarmType: { e = new ExclusiveDeviationAlarmState(null); break; }
                case ObjectTypes.NonExclusiveDeviationAlarmType: { e = new NonExclusiveDeviationAlarmState(null); break; }
                case ObjectTypes.DiscreteAlarmType: { e = new DiscreteAlarmState(null); break; }
                case ObjectTypes.OffNormalAlarmType: { e = new OffNormalAlarmState(null); break; }
                case ObjectTypes.TripAlarmType: { e = new TripAlarmState(null); break; }

                case ObjectTypes.AuditEventType: { e = new AuditEventState(null); break; }
                case ObjectTypes.AuditUpdateMethodEventType: { e = new AuditUpdateMethodEventState(null); break; }

                case ObjectTypes.SystemEventType: { e = new SystemEventState(null); break; }

                default:
                    {
                        e = new BaseEventState(null);
                        break;
                    }
            }

            // get the filter which defines the contents of the notification.
            EventFilter filter = monitoredItem.Status.Filter as EventFilter;

            // initialize the event with the values in the notification.
            try
            {
                e.Update(monitoredItem.Subscription.Session.SystemContext, filter.SelectClauses, notification);
            }
            catch(Exception ex)
            {
                if (ex is ServiceResultException)
                {
                    if ((ex as ServiceResultException).StatusCode != StatusCodes.BadRefreshInProgress)
                        LastMessage = ex.Message;
                }
                else
                    LastMessage = ex.Message;
            }

            // save the orginal notification.
            e.Handle = notification;

            return e;
        }

        bool bListsPrepared;
        internal void PrepareListUpdates()
        {
            if (bListsPrepared)
                return;
            bListsPrepared = true;
            if (_conditionStateList != null)
                _conditionStateList.BeginUpdate();
            if (_auditEventStateList != null)
                _auditEventStateList.BeginUpdate();
            if (_systemEventStateList != null)
                _systemEventStateList.BeginUpdate();
        }

        internal void EndListUpdates()
        {
            if (!bListsPrepared)
                return;
            bListsPrepared = false;
            if (_conditionStateList != null)
                _conditionStateList.EndUpdate();
            if (_auditEventStateList != null)
                _auditEventStateList.EndUpdate();
            if (_systemEventStateList != null)
                _systemEventStateList.EndUpdate();
        }

        public void CheckAndUpdateEventList(EventFieldList change)
        {
            if (!DataValue.IsGood(DataValue))
                DataValue = new DataValue(new StatusCode(StatusCodes.Good));

            NodeId eventTypeId = FindEventType(change);

            // ignore unknown events.
            if (NodeId.IsNull(eventTypeId))
                return;

            // check for refresh start.
            if (eventTypeId == ObjectTypeIds.RefreshStartEventType)
            {
                lock (lockObject)
                {
                    if (_conditionStateList != null)
                    {
                        foreach (var v in _conditionStateList)
                            v.Dispose();
                        _conditionStateList.Clear();
                    }
                    if (_conditionStateMap != null)
                        _conditionStateMap.Clear();
                }
                return;
            }

            // check for refresh end.
            if (eventTypeId == ObjectTypeIds.RefreshEndEventType)
                return;

            // construct the condition object.
            BaseEventState condition = ConstructEvent(change) as BaseEventState;
            if (condition == null)
                return;
            if (condition is AuditEventState)
            {
                // look up the condition type metadata in the local cache.
                INode type = monitoredItem.Subscription.Session.NodeCache.Find(condition.TypeDefinitionId);
                var newVM = new AuditEventStateViewModel(condition as AuditEventState, this, type);
                if (_auditEventStateList != null)
                {
                    lock (lockObject)
                    {
                        _auditEventStateList.Add(newVM);
                        while (_auditEventStateList.Count > MaxAuditEvents)
                        {
                            var d = _auditEventStateList[0];
                            _auditEventStateList.RemoveAt(0);
                            d.Dispose();
                        }
                    }
                }
            }
            else if (condition is ConditionState)
            {
                var cs = condition as ConditionState;
                // look up the condition type metadata in the local cache.
                INode type = monitoredItem.Subscription.Session.NodeCache.Find(condition.TypeDefinitionId);
                String key = String.Format("{0}-{1}", condition.NodeId, BaseVariableState.GetValue(cs.BranchId));
                ConditionStateViewModel currentVM = null;
                lock (lockObject)
                {
                    if (_conditionStateMap == null)
                        _conditionStateMap = new Dictionary<String, ConditionStateViewModel>();
                    else
                    {
                        if (_conditionStateMap.TryGetValue(key, out currentVM))
                        {
                            ConditionState current = currentVM.conditionState;

                            if (current.NodeId == condition.NodeId && BaseVariableState.GetValue(current.BranchId) == BaseVariableState.GetValue(cs.BranchId))
                            {
                                // match found but watch out for out of order events (async processing can cause this to happen).
                                if (BaseVariableState.GetValue(current.Time) > BaseVariableState.GetValue(cs.Time))
                                    return;
                            }
                            else
                                currentVM = null;
                        }
                    }
                }

                if (currentVM != null && !cs.Retain.Value)
                {
                    currentVM.UpdateConditionState(cs, type);
                    lock (lockObject)
                    {
                        _conditionStateList.Remove(currentVM);
                        _conditionStateMap.Remove(key);
                    }
                    currentVM.Dispose();
                }
                else if (currentVM != null)
                {
                    //lock (lockObject)
                    //{
                    //    if (_conditionStateList.Contains(currentVM))
                    //    {
                    //        var insert = _conditionStateList.IndexOf(currentVM);
                    //        _conditionStateList.Remove(currentVM);
                    //        _conditionStateList.Insert(insert, currentVM);
                    //    }
                    //    else
                    //        _conditionStateList.Add(currentVM);
                    //}
                    currentVM.UpdateConditionState(cs, type);
                }
                else if (cs.Retain.Value)
                {
                    currentVM = new ConditionStateViewModel(cs, this, type);
                    lock (lockObject)
                    {
                        _conditionStateList.Add(currentVM);
                        _conditionStateMap.Add(key, currentVM);
                    }
                }
            }
            else
            {
                if (_systemEventStateList != null)
                {
                    // look up the condition type metadata in the local cache.
                    INode type = monitoredItem.Subscription.Session.NodeCache.Find(condition.TypeDefinitionId);
                    var newVM = new SystemEventStateViewModel(condition as BaseEventState, this, type);
                    lock (lockObject)
                    {
                        _systemEventStateList.Add(newVM);
                        while (_systemEventStateList.Count > MaxAuditEvents)
                        {
                            var d = _systemEventStateList[0];
                            _systemEventStateList.RemoveAt(0);
                            d.Dispose();
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Constructs the select clauses for a set of event types.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="eventTypeIds">The event type ids.</param>
        /// <returns>The select clauses for all fields discovered.</returns>
        /// <remarks>
        /// Each event type is an ObjectType in the address space. The fields supported by the
        /// server are defined as children of the ObjectType. Many of the fields are manadatory
        /// and are defined by the UA information model, however, indiviudual servers many not 
        /// support all of the optional fields.
        /// 
        /// This method browses the type model and 
        /// </remarks>
        public static SimpleAttributeOperandCollection ConstructSelectClauses(
#if !NET_STANDARD
            Session session, 
#else
            ISession session,
#endif
            params NodeId[] eventTypeIds)
        {
            // browse the type model in the server address space to find the fields available for the event type.
            SimpleAttributeOperandCollection selectClauses = new SimpleAttributeOperandCollection();

            // must always request the NodeId for the condition instances.
            // this can be done by specifying an operand with an empty browse path.
            SimpleAttributeOperand operand = new SimpleAttributeOperand
            {
                TypeDefinitionId = ObjectTypeIds.BaseEventType,
                AttributeId = Attributes.NodeId,
                BrowsePath = new QualifiedNameCollection()
            };

            selectClauses.Add(operand);

            // add the fields for the selected EventTypes.
            if (eventTypeIds != null && eventTypeIds.Length > 0)
            {
                List<NodeId> foundTypeIds = new List<NodeId>();
                for (int ii = 0; ii < eventTypeIds.Length; ii++)
                {
                    BrowserViewModel.CollectFields(session, eventTypeIds[ii], selectClauses, foundTypeIds);
                }
            }

            // use BaseEventType as the default if no EventTypes specified.
            else
            {
                BrowserViewModel.CollectFields(session, ObjectTypeIds.BaseEventType, selectClauses);
            }

            return selectClauses;
        }

        /// <summary>
        /// Constructs the event filter for the subscription.
        /// </summary>
        /// <returns>The event filter.</returns>
        public EventFilter ConstructFilter()
        {
            EventFilter filter = new EventFilter();

            // the select clauses specify the values returned with each event notification.
            filter.SelectClauses = SelectClauses;

            // the where clause restricts the events returned by the server.
            // it works a lot like the WHERE clause in a SQL statement and supports
            // arbitrary expession trees where the operands are literals or event fields.
            ContentFilter whereClause = new ContentFilter();

            // the code below constructs a filter that looks like this:
            // (Severity >= X OR LastSeverity >= X) AND (SuppressedOrShelved == False) AND (OfType(A) OR OfType(B))

            // add the severity.
            ContentFilterElement element1 = null;
            ContentFilterElement element2 = null;

            if (Severity > EventSeverity.Min)
            {
                // select the Severity property of the event.
                SimpleAttributeOperand operand1 = new SimpleAttributeOperand
                {
                    TypeDefinitionId = ObjectTypeIds.BaseEventType
                };
                operand1.BrowsePath.Add(BrowseNames.Severity);
                operand1.AttributeId = Attributes.Value;

                // specify the value to compare the Severity property with.
                LiteralOperand operand2 = new LiteralOperand
                {
                    Value = new Variant((ushort)Severity)
                };

                // specify that the Severity property must be GreaterThanOrEqual the value specified.
                element1 = whereClause.Push(FilterOperator.GreaterThanOrEqual, operand1, operand2);
            }

            // add the suppressed or shelved.
            if (!IgnoreSuppressedOrShelved)
            {
                // select the SuppressedOrShelved property of the event.
                SimpleAttributeOperand operand1 = new SimpleAttributeOperand
                {
                    TypeDefinitionId = ObjectTypeIds.BaseEventType
                };
                operand1.BrowsePath.Add(BrowseNames.SuppressedOrShelved);
                operand1.AttributeId = Attributes.Value;

                // specify the value to compare the Severity property with.
                LiteralOperand operand2 = new LiteralOperand
                {
                    Value = new Variant(false)
                };

                // specify that the Severity property must Equal the value specified.
                element2 = whereClause.Push(FilterOperator.Equals, operand1, operand2);

                // chain multiple elements together with an AND clause.
                if (element1 != null)
                {
                    element1 = whereClause.Push(FilterOperator.And, element1, element2);
                }
                else
                {
                    element1 = element2;
                }
            }

            // add the event types.
            if (EventTypes != null && EventTypes.Count > 0)
            {
                element2 = null;

                // save the last element.
                for (int ii = 0; ii < EventTypes.Count; ii++)
                {
                    // for this example uses the 'OfType' operator to limit events to thoses with specified event type. 
                    LiteralOperand operand1 = new LiteralOperand
                    {
                        Value = new Variant(EventTypes[ii])
                    };
                    ContentFilterElement element3 = whereClause.Push(FilterOperator.OfType, operand1);

                    // need to chain multiple types together with an OR clause.
                    if (element2 != null)
                    {
                        element2 = whereClause.Push(FilterOperator.Or, element2, element3);
                    }
                    else
                    {
                        element2 = element3;
                    }
                }

                // need to link the set of event types with the previous filters.
                if (element1 != null)
                {
                    whereClause.Push(FilterOperator.And, element1, element2);
                }
            }

            filter.WhereClause = whereClause;

            // return filter.
            return filter;
        }

        public void GenerateClone()
        {
#if !WINDOWS_UWP && !NET_STANDARD
            monitoredItem = monitoredItem.Clone() as MonitoredItem;
#endif
        }

        protected override void IdleExecution()
        {
#if !WINDOWS_UWP
            FlushDataItems();
#endif
        }

        public void ForceValuePropertyChanges(bool bValueInvalidate)
        {
            if(bValueInvalidate)
                _valueInvalidated = true;
            ForceValuePropertyChanges();
        }

        public void ForceValuePropertyChanges()
        {
            OnPropertyChanged("DataValue");
            OnPropertyChanged("Value");
            OnPropertyChanged("InvariantCultureValue");
            OnPropertyChanged("TimeStamp");
            OnPropertyChanged("Quality");
        }

        internal bool ReloadNodeProperties()
        {
            if (NodeIdModel == null)
                return false;

            NodeIdModel.ReloadNodeProperties();
            _EnumStrings = null;

            return NodeIdModel.NodeProperties.Count > 0;
        }

#endregion

#region Commands
        RelayCommand _applyCommand;
        public ICommand ApplyCommand
        {
            get
            {
                if (_applyCommand == null)
                {
                    _applyCommand = new RelayCommand(
                        param =>
                        {
                            try
                            {
                                monitoredItem.Subscription.ApplyChanges();

                                // Dirty the commands registered with CommandManager,
                                // such as our Save command, so that they are queried
                                // to see if they can execute now.
#if !WINDOWS_UWP && !NET_STANDARD
                                CommandManager.InvalidateRequerySuggested();
#endif
                            }
                            catch (Exception ex)
                            {
                                LastMessage = String.Format("{0} - {1}", Title, ex.Message);
                            }
                        },
                        param => CanApply
                        );
                }
                return _applyCommand;
            }
        }

        bool CanApply
        {
            get { return monitoredItem != null && monitoredItem.Subscription != null && monitoredItem.Subscription.Session.Connected && monitoredItem.Subscription.ChangesPending; }
        }

        RelayCommand _conditionRefreshCommand;
        public ICommand ConditionRefreshCommand
        {
            get
            {
                if (_conditionRefreshCommand == null)
                {
                    _conditionRefreshCommand = new RelayCommand(
                        param =>
                        {
                            try
                            {
                                _ConditionStateViewModel.monitoredItem.Subscription.ConditionRefresh();

                                // Dirty the commands registered with CommandManager,
                                // such as our Save command, so that they are queried
                                // to see if they can execute now.
                                // CommandManager.InvalidateRequerySuggested();
                            }
                            catch (Exception ex)
                            {
                                if (ex is ServiceResultException)
                                {
                                    if ((ex as ServiceResultException).StatusCode != StatusCodes.BadRefreshInProgress)
                                        LastMessage = ex.Message;
                                }
                                else
                                    LastMessage = ex.Message;
                            }
                        },
                        param => CanConditionRefresh
                        );
                }
                return _conditionRefreshCommand;
            }
        }

        bool CanConditionRefresh
        {
            get
            {
                return monitoredItem != null && monitoredItem.Subscription != null && monitoredItem.Subscription.Session != null &&
                    monitoredItem.Subscription.Session.Connected && NodeIdModel != null && NodeIdModel.IsEventNotifier && _ConditionStateViewModel != null;
            }
        }

        RelayCommand _auditEventRefreshCommand;
        public ICommand AuditEventRefreshCommand
        {
            get
            {
                if (_auditEventRefreshCommand == null)
                {
                    _auditEventRefreshCommand = new RelayCommand(
                        param =>
                        {
                            try
                            {
                                _AuditEventStateViewModel.monitoredItem.Subscription.ConditionRefresh();

                                // Dirty the commands registered with CommandManager,
                                // such as our Save command, so that they are queried
                                // to see if they can execute now.
                                // CommandManager.InvalidateRequerySuggested();
                            }
                            catch (Exception ex)
                            {
                                if (ex is ServiceResultException)
                                {
                                    if ((ex as ServiceResultException).StatusCode != StatusCodes.BadRefreshInProgress)
                                        LastMessage = ex.Message;
                                }
                                else
                                    LastMessage = ex.Message;
                            }
                        },
                        param => CanAuditEventRefresh
                        );
                }
                return _auditEventRefreshCommand;
            }
        }

        bool CanAuditEventRefresh
        {
            get
            {
                return monitoredItem != null && monitoredItem.Subscription != null && monitoredItem.Subscription.Session != null &&
                    monitoredItem.Subscription.Session.Connected && NodeIdModel != null && NodeIdModel.IsEventNotifier && _AuditEventStateViewModel != null;
            }
        }

        RelayCommand _conditionAcknowledgeAllCommand;
        public ICommand ConditionAcknowledgeAllCommand
        {
            get
            {
                if (_conditionAcknowledgeAllCommand == null)
                {
                    _conditionAcknowledgeAllCommand = new RelayCommand(
                        param => CallAcknowledgeAll(),
                        param => CanConditionAcknowledgeAllCommand
                        );
                }
                return _conditionAcknowledgeAllCommand;
            }
        }

        bool CanConditionAcknowledgeAllCommand
        {
            get
            {
                return monitoredItem != null && monitoredItem.Subscription != null && monitoredItem.Subscription.Session != null && !bPendingCallAcknowledgeAll &&
                    monitoredItem.Subscription.Session.Connected && NodeIdModel != null && NodeIdModel.IsEventNotifier && _ConditionStateViewModel != null &&
                    !_ConditionStateViewModel.bPendingCallAcknowledgeAll;
            }
        }

        RelayCommand _conditionConfirmAllCommand;
        public ICommand ConditionConfirmAllCommand
        {
            get
            {
                if (_conditionConfirmAllCommand == null)
                {
                    _conditionConfirmAllCommand = new RelayCommand(
                        param => CallConfirmAll(),
                        param => CanConditionConfirmAllCommand
                        );
                }
                return _conditionConfirmAllCommand;
            }
        }

        bool CanConditionConfirmAllCommand
        {
            get
            {
                return monitoredItem != null && monitoredItem.Subscription != null && monitoredItem.Subscription.Session != null && !bPendingCallConfirmAll &&
                    monitoredItem.Subscription.Session.Connected && NodeIdModel != null && NodeIdModel.IsEventNotifier && _ConditionStateViewModel != null &&
                    !_ConditionStateViewModel.bPendingCallConfirmAll;
            }
        }
#endregion

#region Properties

        public bool IsReadOnly
        {
            get
            {
                return bIsReadOnly;
            }
        }

        public MonitoredItemViewModel ReferenceViewModel
        {
            get
            {
                return referenceViewModel;
            }
        }

        public bool IsValid
        {
            get
            {
                if (monitoredItem == null)
                    return true;

                var subscription = GetSubscriptionViewModelParent();
                if (subscription == null)
                    return false;
                var session = subscription.GetSessionViewModelParent();
                if (session == null)
                    return false;
                return session.Connected;
                //return monitoredItem == null || monitoredItem.Subscription != null && monitoredItem.Subscription.Session != null &&
                //    monitoredItem.Subscription.Session.Connected;
            }
        }

#region Filter Properties
        /// <summary>
        /// The minimum severity for the events of interest.
        /// </summary>
        public EventSeverity Severity;
        /// <summary>
        /// The types for the events of interest.
        /// </summary>
        public IList<NodeId> EventTypes;
        /// <summary>
        /// Whether suppressed or shelved condition events are of interest.
        /// </summary>
        public bool IgnoreSuppressedOrShelved;
        /// <summary>
        /// The select clauses to use with the filter.
        /// </summary>
        public SimpleAttributeOperandCollection SelectClauses;
#endregion
        
        String endpointUrl;
        public String EndpointUrl
        {
            get
            {
                return endpointUrl;
            }
            set
            {
                if (endpointUrl == value)
                    return;
                endpointUrl = value;
            }
        }

        Dictionary<String, MonitoredItemViewModel> mapChildItems;
        public MonitoredItemViewModel GetChildItem(String relativePath)
        {
            lock (lockObject)
            {
                if (mapChildItems != null && mapChildItems.ContainsKey(relativePath))
                    return mapChildItems[relativePath];

                BrowsePathCollection browsePaths = new BrowsePathCollection();
                BrowsePath browsePath = new BrowsePath
                {
                    StartingNode = monitoredItem.StartNodeId
                };

                // parse the relative path.
                try
                {
                    browsePath.RelativePath = Opc.Ua.RelativePath.Parse(relativePath, monitoredItem.Subscription.Session.TypeTree);
                }
                catch (Exception e)
                {
                    monitoredItem.SetError(new ServiceResult(e));
                    return null;
                }
                browsePaths.Add(browsePath);

#if !NET_STANDARD
                if (monitoredItem.Subscription.MaxNodesPerTranslateBrowsePathsToNodeIds > 0 && browsePaths.Count > monitoredItem.Subscription.MaxNodesPerTranslateBrowsePathsToNodeIds)
                {
                    for (long i = 0; i < browsePaths.Count; i += monitoredItem.Subscription.MaxNodesPerTranslateBrowsePathsToNodeIds)
                    {
                        var browsePathsRange = new BrowsePathCollection(browsePaths.GetRange((int)i, Math.Min((int)monitoredItem.Subscription.MaxNodesPerTranslateBrowsePathsToNodeIds, (int)(browsePaths.Count - i))));

                        // translate browse paths.
                        BrowsePathResultCollection results;
                        DiagnosticInfoCollection diagnosticInfos;

                        ResponseHeader responseHeader = monitoredItem.Subscription.Session.TranslateBrowsePathsToNodeIds(
                            null,
                            browsePathsRange,
                            out results,
                            out diagnosticInfos);

                        ClientBase.ValidateResponse(results, browsePathsRange);
                        ClientBase.ValidateDiagnosticInfos(diagnosticInfos, browsePathsRange);

                        if (StatusCode.IsBad(results[0].StatusCode))
                        {
                            monitoredItem.SetError(ClientBase.GetResult(results[0].StatusCode, 0, diagnosticInfos, responseHeader));
                            return null;
                        }
                        else
                        {
                            // update the node id.
                            if (results[0].Targets.Count <= 0)
                                return null;

                            var ResolvedNodeId = ExpandedNodeId.ToNodeId(results[0].Targets[0].TargetId, monitoredItem.Subscription.Session.NamespaceUris);
                            ExpandedNodeIdCollection list = new ExpandedNodeIdCollection();
                            list.Add(ResolvedNodeId);

                            var subscription = GetSubscriptionViewModelParent();
                            if (subscription == null)
                                return null;

                            if (mapChildItems == null)
                                mapChildItems = new Dictionary<String, MonitoredItemViewModel>();

                            var map = subscription.AddMonitoredItem(list);
                            subscription.ApplyChanges();
                            foreach (var v in map.Keys)
                            {
                                if (v == ResolvedNodeId)
                                {
                                    mapChildItems[relativePath] = map[v];

                                    map[v].AddEntityReferences(GetEntityReferences());
                                    Children.Add(map[v]);
                                    map[v].MonitoringMode = MonitoringMode;
                                    map[v].SamplingInterval = SamplingInterval;
                                }
                            }
                        }
                    }
                }
                else
#endif
                {
                    // translate browse paths.
                    BrowsePathResultCollection results;
                    DiagnosticInfoCollection diagnosticInfos;

                    ResponseHeader responseHeader = monitoredItem.Subscription.Session.TranslateBrowsePathsToNodeIds(
                        null,
                        browsePaths,
                        out results,
                        out diagnosticInfos);

                    ClientBase.ValidateResponse(results, browsePaths);
                    ClientBase.ValidateDiagnosticInfos(diagnosticInfos, browsePaths);

                    if (StatusCode.IsBad(results[0].StatusCode))
                    {
                        monitoredItem.SetError(ClientBase.GetResult(results[0].StatusCode, 0, diagnosticInfos, responseHeader));
                        return null;
                    }
                    else
                    {
                        // update the node id.
                        if (results[0].Targets.Count <= 0)
                            return null;

                        var ResolvedNodeId = ExpandedNodeId.ToNodeId(results[0].Targets[0].TargetId, monitoredItem.Subscription.Session.NamespaceUris);
                        ExpandedNodeIdCollection list = new ExpandedNodeIdCollection();
                        list.Add(ResolvedNodeId);

                        var subscription = GetSubscriptionViewModelParent();
                        if (subscription == null)
                            return null;

                        if (mapChildItems == null)
                            mapChildItems = new Dictionary<String, MonitoredItemViewModel>();

                        var map = subscription.AddMonitoredItem(list);
                        subscription.ApplyChanges();
                        foreach (var v in map.Keys)
                        {
                            if (v == ResolvedNodeId)
                            {
                                mapChildItems[relativePath] = map[v];

                                map[v].AddEntityReferences(GetEntityReferences());
                                Children.Add(map[v]);
                                map[v].MonitoringMode = MonitoringMode;
                                map[v].SamplingInterval = SamplingInterval;
                            }
                        }
                    }
                }

                if (mapChildItems != null && mapChildItems.ContainsKey(relativePath))
                    return mapChildItems[relativePath];

            }

            return null;
        }

        HistoryReadViewModel historyReadModel;
        public HistoryReadViewModel HistoryReadModel
        {
            get
            {
                lock (lockObject)
                {
                    if (historyReadModel == null)
                    {
                        historyReadModel = new HistoryReadViewModel(GetSubscriptionViewModelParent().GetSessionViewModelParent(), monitoredItem.ResolvedNodeId);
                    }
                }

                return historyReadModel;
            }
        }

        HistoryEventReadViewModel historyEventReadModel;
        public HistoryEventReadViewModel HistoryEventReadModel
        {
            get
            {
                lock (lockObject)
                {
                    if (historyEventReadModel == null)
                    {
                        historyEventReadModel = new HistoryEventReadViewModel(GetSubscriptionViewModelParent().GetSessionViewModelParent(), monitoredItem.ResolvedNodeId);
                    }
                }

                return historyEventReadModel;
            }
        }

        //void CleanNotConnectedItemLists()
        //{
        //    if (monitoredItem == null)
        //        return;
        //    if (DataValue != null && StatusCode.IsGood(DataValue.StatusCode))
        //        return;
        //    //if (!NodeIdModel.IsEventNotifier)
        //    //    return;
        //
        //    lock (lockObject)
        //    {
        //        if (_ConditionStateViewModel != null)
        //        {
        //            Children.Remove(_ConditionStateViewModel);
        //            _ConditionStateViewModel.Dispose();
        //            _ConditionStateViewModel = null;
        //            _conditionStateList = null;
        //        }
        //
        //        if (_AuditEventStateViewModel != null)
        //        {
        //            Children.Remove(_AuditEventStateViewModel);
        //            _AuditEventStateViewModel.Dispose();
        //            _AuditEventStateViewModel = null;
        //            _auditEventStateList = null;
        //        }
        //
        //        if (_SystemEventStateViewModel != null)
        //        {
        //            Children.Remove(_SystemEventStateViewModel);
        //            _SystemEventStateViewModel.Dispose();
        //            _SystemEventStateViewModel = null;
        //            _systemEventStateList = null;
        //        }
        //    }
        //}

        Dictionary<String, ConditionStateViewModel> _conditionStateMap;
        SafeObservableCollection<ConditionStateViewModel> _conditionStateList;
        MonitoredItemViewModel _ConditionStateViewModel;
#if !WINDOWS_UWP
        Thread _ConditionStateViewModelThread;
#endif
        public SafeObservableCollection<ConditionStateViewModel> ConditionStateList
        {
            get
            {
                lock (lockObject)
                {
                    if (_conditionStateList != null)
                        return _conditionStateList;

                    if (_ConditionStateViewModel == null)
                    {
                        if (NodeIdModel == null || !NodeIdModel.IsEventNotifier)
                            return null;

#if !WINDOWS_UWP
                        var currentThread = Thread.CurrentThread;
                        if (_ConditionStateViewModelThread == null)
                            _ConditionStateViewModelThread = currentThread;
                        else if (_ConditionStateViewModelThread != currentThread)
                            throw new NotSupportedException("Cannot access ConditionList from different threads. Please use different session names");
#endif
                        if (GetSubscriptionViewModelParent().GetSessionViewModelParent().Connected)
                        {
                            DataValue dv = new DataValue(DataValue);
                            dv.StatusCode = StatusCodes.Good;
                            DataValue = dv;
                        }

                        var listCreateMonItem = new ExpandedNodeIdCollection();
                        listCreateMonItem.Add(NodeIdModel.nodeId);
                        var map = GetSubscriptionViewModelParent().AddAlarmEventMonitoredItem(listCreateMonItem);
                        if (map.ContainsKey(NodeIdModel.nodeId))
                        {
                            _ConditionStateViewModel = map[NodeIdModel.nodeId];

                            //if (mapChildItems == null)
                            //    mapChildItems = new Dictionary<String, MonitoredItemViewModel>();
                            //mapChildItems.Add("ConditionState", _ConditionStateViewModel);

                            _ConditionStateViewModel.AddEntityReferences(GetEntityReferences());
                            Children.Add(_ConditionStateViewModel);
                            // _ConditionStateViewModel.MonitoringMode = MonitoringMode;
                            // _ConditionStateViewModel.SamplingInterval = SamplingInterval;

                            GetSubscriptionViewModelParent().ApplyChanges();
                        }

                        ConditionRefreshCommand.Execute(null);
                    }

                    if (_ConditionStateViewModel != null)
                    {
                        // System.Windows.Data.BindingOperations.EnableCollectionSynchronization(_ConditionStateViewModel._conditionStateList, lockObject);
                        return _ConditionStateViewModel._conditionStateList;
                    }
                }

                return null;
            }
        }

        SafeObservableCollection<AuditEventStateViewModel> _auditEventStateList;
        MonitoredItemViewModel _AuditEventStateViewModel;
#if !WINDOWS_UWP
        Thread _AuditEventStateViewModelThread;
#endif
        public SafeObservableCollection<AuditEventStateViewModel> AuditEventStateList
        {
            get
            {
                lock (lockObject)
                {
                    if (_auditEventStateList != null)
                        return _auditEventStateList;

                    if (_AuditEventStateViewModel == null)
                    {
                        if (NodeIdModel == null || !NodeIdModel.IsEventNotifier)
                            return null;

#if !WINDOWS_UWP
                        var currentThread = Thread.CurrentThread;
                        if (_AuditEventStateViewModelThread == null)
                            _AuditEventStateViewModelThread = currentThread;
                        else if (_AuditEventStateViewModelThread != currentThread)
                            throw new NotSupportedException("Cannot access ConditionList from different threads. Please use different session names");
#endif
                        if (GetSubscriptionViewModelParent().GetSessionViewModelParent().Connected)
                        {
                            DataValue dv = new DataValue(DataValue);
                            dv.StatusCode = StatusCodes.Good;
                            DataValue = dv;
                        }

                        var listCreateMonItem = new ExpandedNodeIdCollection();
                        listCreateMonItem.Add(NodeIdModel.nodeId);
                        var map = GetSubscriptionViewModelParent().AddAuditingEventMonitoredItem(listCreateMonItem);
                        if (map.ContainsKey(NodeIdModel.nodeId))
                        {
                            _AuditEventStateViewModel = map[NodeIdModel.nodeId];
                            //if (mapChildItems == null)
                            //    mapChildItems = new Dictionary<String, MonitoredItemViewModel>();
                            //mapChildItems.Add("AuditEventState", _AuditEventStateViewModel);

                            _AuditEventStateViewModel.AddEntityReferences(GetEntityReferences());
                            Children.Add(_AuditEventStateViewModel);
                            // _AuditEventStateViewModel.MonitoringMode = MonitoringMode;
                            // _AuditEventStateViewModel.SamplingInterval = SamplingInterval;

                            GetSubscriptionViewModelParent().ApplyChanges();
                        }
                    }

                    if (_AuditEventStateViewModel != null)
                    {
                        // System.Windows.Data.BindingOperations.EnableCollectionSynchronization(_AuditEventStateViewModel._auditEventStateList, lockObject);
                        return _AuditEventStateViewModel._auditEventStateList;
                    }
                }

                return null;
            }
        }

        SafeObservableCollection<SystemEventStateViewModel> _systemEventStateList;
        MonitoredItemViewModel _SystemEventStateViewModel;
#if !WINDOWS_UWP
        Thread _SystemEventStateViewModelThread;
#endif
        public SafeObservableCollection<SystemEventStateViewModel> SystemEventStateList
        {
            get
            {
                lock (lockObject)
                {
                    if (_systemEventStateList != null)
                        return _systemEventStateList;

                    if (_SystemEventStateViewModel == null)
                    {
                        if (NodeIdModel == null || !NodeIdModel.IsEventNotifier)
                            return null;

#if !WINDOWS_UWP
                        var currentThread = Thread.CurrentThread;
                        if (_SystemEventStateViewModelThread == null)
                            _SystemEventStateViewModelThread = currentThread;
                        else if (_SystemEventStateViewModelThread != currentThread)
                            throw new NotSupportedException("Cannot access ConditionList from different threads. Please use different session names");
#endif
                        if (GetSubscriptionViewModelParent().GetSessionViewModelParent().Connected)
                        {
                            DataValue dv = new DataValue(DataValue);
                            dv.StatusCode = StatusCodes.Good;
                            DataValue = dv;
                        }

                        var listCreateMonItem = new ExpandedNodeIdCollection();
                        listCreateMonItem.Add(NodeIdModel.nodeId);
                        var map = GetSubscriptionViewModelParent().AddSystemEventMonitoredItem(listCreateMonItem);
                        if (map.ContainsKey(NodeIdModel.nodeId))
                        {
                            _SystemEventStateViewModel = map[NodeIdModel.nodeId];
                            //if (mapChildItems == null)
                            //    mapChildItems = new Dictionary<String, MonitoredItemViewModel>();
                            //mapChildItems.Add("SystemEventState", _SystemEventStateViewModel);

                            _SystemEventStateViewModel.AddEntityReferences(GetEntityReferences());
                            Children.Add(_SystemEventStateViewModel);
                            // _SystemEventStateViewModel.MonitoringMode = MonitoringMode;
                            // _SystemEventStateViewModel.SamplingInterval = SamplingInterval;

                            GetSubscriptionViewModelParent().ApplyChanges();
                        }
                    }

                    if (_SystemEventStateViewModel != null)
                    {
                        // System.Windows.Data.BindingOperations.EnableCollectionSynchronization(_SystemEventStateViewModel._systemEventStateList, lockObject);
                        return _SystemEventStateViewModel._systemEventStateList;
                    }
                }

                return null;
            }
        }

        int _maxAuditEvents = 100;
        public int MaxAuditEvents
        {
            get
            {
                return _maxAuditEvents;
            }
            set
            {
                if (value == _maxAuditEvents)
                    return;

                _maxAuditEvents = value;

                OnPropertyChanged("MaxAuditEvents");
            }
        }

        private DataValue _dataValue = new DataValue(StatusCodes.Uncertain);
        public DataValue DataValue
        {
            get
            {
                return _dataValue;
            }
            set
            {
                if (value == _dataValue)
                    return;

                _dataValue = value;
#if !WINDOWS_UWP
                AddNewDataItem();
#endif
                _qualityInvalidated = true;
                _timeStampInvalidated = true;
                _valueInvalidated = true;

                OnPropertyChanged("DataValue");
                OnPropertyChanged("Value");
                OnPropertyChanged("InvariantCultureValue");
                OnPropertyChanged("TimeStamp");
                OnPropertyChanged("Quality");

                if (_dataValue != null && _dataValue.Value is Array)
                    OnPropertyChanged("DataValueCollection");

                //CleanNotConnectedItemLists();
            }
        }

        SafeObservableCollection<DataValue> _dataValueHistory;
        public SafeObservableCollection<DataValue> DataValueHistory
        {
            get
            {
#if !WINDOWS_UWP
                lock (lockData)
                {
                    if (_dataValueHistory == null)
                        _dataValueHistory = new SafeObservableCollection<DataValue>();
                }
#endif
                return _dataValueHistory;
            }
        }

        public class DataObject
        {
            public Object Value { get; set; }
            public int Index { get; set; }
        }

        public List<DataObject> DataValueCollection
        {
            get
            {
                if (_dataValue != null && _dataValue.Value is Array)
                {
                    List<DataObject> dataValueCollection = new List<DataObject>();

                    Array a = (_dataValue.Value as Array);

                    int i = 0;
                    foreach (var el in a)
                        dataValueCollection.Add(new DataObject() { Value = el, Index = i++ });

                    return dataValueCollection;
                    // return new SafeObservableCollection<DataObject>(dataValueCollection);
                }

                return null;
            }
        }

        public List<DataObject> DataValueCollectionDouble
        {
            get
            {
                if (_dataValue != null && _dataValue.Value is Array)
                {
                    List<DataObject> dataValueCollection = new List<DataObject>();

                    Array a = (_dataValue.Value as Array);

                    int i = 0;
                    foreach (var el in a)
                    {
                        try
                        {
                            dataValueCollection.Add(new DataObject() { Value = Convert.ToDouble(el), Index = i++ });
                        }
                        catch (Exception ex)
                        {
                            return null;
                        }
                    }

                    return dataValueCollection;
                    // return new SafeObservableCollection<DataObject>(dataValueCollection);
                }

                return null;
            }
        }

        private bool _valueInvalidated;
        private string _valueCurrentCultureName = Thread.CurrentThread.CurrentCulture.Name;
        private String _value;

        public String NormalizedValue
        {
            get
            {
                if (_dataValue != null)
                    return ResolveValue(_dataValue);
                else
                    return String.Empty;
            }
        }

        public String InvariantCultureValue
        {
            get
            {
                if (_dataValue != null)
                    return String.Format(CultureInfo.InvariantCulture, "{0}", _dataValue.WrappedValue);
                else
                    return String.Empty;
            }
        }

        public bool IsValueEqual(String value)
        {
            String oldvalue = _dataValue != null ? Convert.ToString(_dataValue.Value) : _value;
            if (_dataValue.Value is Array)
                oldvalue = Value;

            String newvalue = value;

            Object parsevalue;
            if (TryParseValue(value, out parsevalue))
                newvalue = Convert.ToString(parsevalue);

            var ret = String.Compare(newvalue, oldvalue, false);
            return ret == 0;
        }

        int pendingWrites;
        List<String> pendingValues = new List<String>();
        Task<bool> pendingTask;
        public String Value
        {
            get
            {
                if (_valueInvalidated || _valueCurrentCultureName != Thread.CurrentThread.CurrentCulture.Name)
                {
                    _valueInvalidated = false;
                    _valueCurrentCultureName = Thread.CurrentThread.CurrentCulture.Name;
                    if (_dataValue != null)
                        _value = String.Format("{0}", _dataValue.WrappedValue);
                }
                return _value;
            }
            set
            {
                String oldvalue = _dataValue != null ? Convert.ToString(_dataValue.Value) : _value;
                String newvalue = value;

                var converterValue = Utilities.Converters.ConverterValue.Empty;
                try
                {
                    converterValue = value.FromXml<Utilities.Converters.ConverterValue>();
                    newvalue = converterValue.Value;
                }
                catch
                { }

                int indexArray = converterValue.IndexArray;
                int indexBit = converterValue.IndexBit;

                Object parsevalue;
                if (TryParseValue(newvalue, out parsevalue))
                    newvalue = Convert.ToString(parsevalue);
                
                if (String.Compare(newvalue, oldvalue, false) == 0)
                    return;

                TaskScheduler sc = null;

                if (monitoredItem != null)
                {
#if !WINDOWS_UWP
                    int nMaxPedingWrites = Properties.Settings.Default.MaxPendingWrites;
#else
                    int nMaxPedingWrites = 10;
#endif
                    try
                    {
                        sc = TaskScheduler.FromCurrentSynchronizationContext();
                    }
                    catch
                    { }

                    if (nMaxPedingWrites == 0 || sc == null)
                    {
                        WriteCastedValue(newvalue, indexArray, indexBit);
                        return;
                    }

                    if (pendingWrites >= nMaxPedingWrites)
                    {
                        // throw new ArgumentException(String.Format("Too many Pending Write command on this MonitoredItem {0}", Title));
                        lock (pendingValues)
                        {
                            pendingValues.Add(value);
                        }
                        return;
                    }

                    Interlocked.Increment(ref pendingWrites);

                    Task<bool> task1 = null;
                    lock (pendingValues)
                    {
                        if (pendingTask != null)
                        {
                            task1 = pendingTask.ContinueWith((T) =>
                            {
                                return WriteCastedValue(newvalue, indexArray, indexBit);
                            });
                        }
                        else
                        {
                            task1 = Task.Factory.StartNew(() =>
                            {
                                return WriteCastedValue(newvalue, indexArray, indexBit);
                            });
                        }

                        pendingTask = task1;
                    }

                    var ct = new CancellationToken();
                    var task2 = task1.ContinueWith(ret =>
                    {
                        var pending = Interlocked.Decrement(ref pendingWrites);

                        if (ret.Result == true)
                        {
                            _valueInvalidated = true;

                            OnPropertyChanged("Value");
                            OnPropertyChanged("InvariantCultureValue");
                        }

                        if (pending < (nMaxPedingWrites / 2))
                        {
                            String v = null;
                            lock (pendingValues)
                            {
                                if (pendingValues.Count > 0)
                                {
                                    v = pendingValues[0];
                                    pendingValues.RemoveAt(0);
                                }
                            }

                            if (v != null)
                                Value = v;
                        }
                    }, sc);
                    task1.ContinueWith((t) =>
                    {
                        Debug.WriteLine("I have observed a {0}",
                            t.Exception.InnerException.GetType().Name);
                        OnPropertyChanged("Value");
                        OnPropertyChanged("InvariantCultureValue");
                        throw t.Exception;
                    }, ct, TaskContinuationOptions.OnlyOnFaulted, sc);
                    task2.ContinueWith((t) =>
                    {
                        Debug.WriteLine("I have observed a {0}",
                            t.Exception.InnerException.GetType().Name);
                        OnPropertyChanged("Value");
                        OnPropertyChanged("InvariantCultureValue");
                        throw t.Exception;
                    }, ct, TaskContinuationOptions.OnlyOnFaulted, sc);
                }
                else
                {
                    //DataValue = new DataValue(new Variant(value), StatusCodes.Good);
                    WriteValue(newvalue, indexArray, indexBit);
                }
            }
        }

        bool WriteCastedValue(String newvalue, int indexArray, int indexBit)
        {
            WriteValue valueWrite = new WriteValue
            {
                NodeId = monitoredItem.ResolvedNodeId,
                AttributeId = Attributes.Value,
                IndexRange = null
            };

            // read the display name for non-variables.
            if ((monitoredItem.NodeClass & (NodeClass.Variable | NodeClass.VariableType)) == 0)
            {
                valueWrite.AttributeId = Attributes.DisplayName;
            }

            NodeId datatypeId = Attributes.GetDataTypeId(monitoredItem.AttributeId);
            int valueRank = Attributes.GetValueRank(monitoredItem.AttributeId);
            uint arraySizeOneDimension = 0;

            if (monitoredItem.AttributeId == Attributes.Value)
            {
                VariableNode vnode = monitoredItem.Subscription.Session.NodeCache.Find(valueWrite.NodeId) as VariableNode;

                if (vnode != null)
                {
                    datatypeId = vnode.DataType;
                    valueRank = vnode.ValueRank;
                    arraySizeOneDimension = (vnode.ArrayDimensions != null && vnode.ArrayDimensions.Count > 0) ? vnode.ArrayDimensions[0] : 0;
                }
            }
            var builtinType = Opc.Ua.TypeInfo.GetBuiltInType(datatypeId, monitoredItem.Subscription.Session.TypeTree);
            if (builtinType == BuiltInType.Double || builtinType == BuiltInType.Float)
            {
                CultureInfo culture = new CultureInfo(System.Globalization.CultureInfo.CurrentCulture.Name, true);
                newvalue = newvalue.Replace(culture.NumberFormat.NumberDecimalSeparator, ".");
            }

            return WriteValue(newvalue, indexArray, indexBit);
        }

        private bool _timeStampInvalidated;
        private String _timestamp;
        public String TimeStamp
        {
            get
            {
                if (_timeStampInvalidated)
                {
                    _timeStampInvalidated = false;
                    if (_dataValue != null)
                    {
                        DateTime time = _dataValue.SourceTimestamp;
                        _timestamp = time != null && time != DateTime.MinValue ? String.Format("{0:HH:mm:ss.fff}", time.ToLocalTime()) : String.Empty;
                    }
                    else
                        _timestamp = null;
                }
                return _timestamp;
            }
            set
            {
                if (String.Compare(value, _timestamp, false) == 0)
                    return;

                _timestamp = value;
                OnPropertyChanged("TimeStamp");
            }
        }

        private bool _qualityInvalidated;
        private String _quality;
        public String Quality
        {
            get
            {
                if (_qualityInvalidated)
                {
                    _qualityInvalidated = false;
                    if (_dataValue != null)
                        _quality = String.Format("{0}", _dataValue.StatusCode);
                    else
                        _quality = null;
                }

                return _quality;
            }
            set
            {
                if (String.Compare(value, _quality, false) == 0)
                    return;

                _quality = value;
                OnPropertyChanged("Quality");
            }
        }

        public uint AttributeId
        {
            get
            {
                return monitoredItem.AttributeId;
            }
            set
            {
                if (value == monitoredItem.AttributeId)
                    return;

                monitoredItem.AttributeId = value;
                OnPropertyChanged("AttributeId");
            }
        }

        public bool AttributesModified
        {
            get
            {
                return monitoredItem.AttributesModified;
            }
        }

        public int CacheQueueSize
        {
            get
            {
                return monitoredItem.CacheQueueSize;
            }
            set
            {
                if (value == monitoredItem.CacheQueueSize)
                    return;

                monitoredItem.CacheQueueSize = value;
                OnPropertyChanged("AttributeId");

                lock (lockObject)
                {
                    if (mapChildItems != null)
                    {
                        foreach (var v in mapChildItems.Values)
                            v.CacheQueueSize = value;
                    }
                }
            }
        }

        public bool DiscardOldest
        {
            get
            {
                return monitoredItem.DiscardOldest;
            }
            set
            {
                if (value == monitoredItem.DiscardOldest)
                    return;

                monitoredItem.DiscardOldest = value;
                OnPropertyChanged("DiscardOldest");

                lock (lockObject)
                {
                    if (mapChildItems != null)
                    {
                        foreach (var v in mapChildItems.Values)
                            v.DiscardOldest = value;
                    }
                }
            }
        }

        public string DisplayName
        {
            get
            {
                return monitoredItem.DisplayName;
            }
            set
            {
                if (String.Compare(value, monitoredItem.DisplayName, false) == 0)
                    return;

                lock (mapActiveMonitoredItems)
                {
                    var key = (from c in mapActiveMonitoredItems where c.Value == this select c.Key).ToList();
                    if (key.Count > 0)
                        mapActiveMonitoredItems.Remove(key[0]);
                }

                Title = monitoredItem.DisplayName = value;

                lock (mapActiveMonitoredItems)
                {
                    mapActiveMonitoredItems.Add(GetComposedTitle(), this);
                }

                OnPropertyChanged("DisplayName");
            }
        }

        public QualifiedName Encoding
        {
            get
            {
                return monitoredItem.Encoding;
            }
            set
            {
                if (value == monitoredItem.Encoding)
                    return;

                monitoredItem.Encoding = value;
                OnPropertyChanged("Encoding");

                lock (lockObject)
                {
                    if (mapChildItems != null)
                    {
                        foreach (var v in mapChildItems.Values)
                            v.Encoding = value;
                    }
                }
            }
        }

        public uint DeadbandFilterType
        {
            get
            {
                DataChangeFilter filter = Filter as DataChangeFilter;
                if (filter == null)
                    return (uint)DeadbandType.None;
                return filter.DeadbandType;
            }

            set
            {
                DataChangeFilter filter = Filter as DataChangeFilter;
                if (filter == null)
                {
                    filter = new DataChangeFilter();
                    filter.Trigger = DataChangeTrigger.StatusValue;
                    Filter = filter;
                }

                if (filter.DeadbandType == value)
                    return;
                filter.DeadbandType = value;
                OnPropertyChanged("DeadbandType");

                lock (lockObject)
                {
                    if (mapChildItems != null)
                    {
                        foreach (var v in mapChildItems.Values)
                            v.DeadbandFilterType = value;
                    }
                }
            }
        }

        public double DeadbandValue
        {
            get
            {
                DataChangeFilter filter = Filter as DataChangeFilter;
                if (filter == null)
                    return Double.NaN;
                return filter.DeadbandValue;
            }

            set
            {
                DataChangeFilter filter = Filter as DataChangeFilter;
                if (filter == null)
                {
                    filter = new DataChangeFilter();
                    filter.Trigger = DataChangeTrigger.StatusValue;
                    Filter = filter;
                }

                if (filter.DeadbandValue == value)
                    return;
                filter.DeadbandValue = value;
                OnPropertyChanged("DeadbandValue");

                lock (lockObject)
                {
                    if (mapChildItems != null)
                    {
                        foreach (var v in mapChildItems.Values)
                            v.DeadbandValue = value;
                    }
                }
            }
        }


        public MonitoringFilter Filter
        {
            get
            {
                return monitoredItem.Filter;
            }
            set
            {
                if (value == monitoredItem.Filter)
                    return;

                monitoredItem.Filter = value;
                OnPropertyChanged("Filter");

                lock (lockObject)
                {
                    if (mapChildItems != null)
                    {
                        foreach (var v in mapChildItems.Values)
                            v.Filter = value;
                    }
                }
            }
        }

        public string IndexRange
        {
            get
            {
                return monitoredItem.IndexRange;
            }
            set
            {
                if (String.Compare(value, monitoredItem.IndexRange, false) == 0)
                    return;

                monitoredItem.IndexRange = value;
                OnPropertyChanged("IndexRange");

                lock (lockObject)
                {
                    if (mapChildItems != null)
                    {
                        foreach (var v in mapChildItems.Values)
                            v.IndexRange = value;
                    }
                }
            }
        }

        DateTime monitoredItemChangedTime;
        public DateTime MonitoredItemChangedTime
        {
            get 
            {
                return monitoredItemChangedTime;
            }
        }

        public MonitoringMode MonitoringMode
        {
            get
            {
                return monitoredItem.MonitoringMode;
            }
            set
            {
                if (value == monitoredItem.MonitoringMode)
                    return;

                monitoredItem.MonitoringMode = value;
                monitoredItemChangedTime = DateTime.UtcNow;
                OnPropertyChanged("MonitoringMode");

                lock (lockObject)
                {
                    if (mapChildItems != null)
                    {
                        foreach (var v in mapChildItems.Values)
                            v.MonitoringMode = value;
                    }
                }
            }
        }

        public NodeClass NodeClass
        {
            get
            {
                return monitoredItem.NodeClass;
            }
            set
            {
                if (value == monitoredItem.NodeClass)
                    return;

                monitoredItem.NodeClass = value;
                OnPropertyChanged("NodeClass");
            }
        }

        public uint QueueSize
        {
            get
            {
                return monitoredItem.QueueSize;
            }
            set
            {
                if (value == monitoredItem.QueueSize)
                    return;

                monitoredItem.QueueSize = value;
                OnPropertyChanged("QueueSize");

                lock (lockObject)
                {
                    if (mapChildItems != null)
                    {
                        foreach (var v in mapChildItems.Values)
                            v.QueueSize = value;
                    }
                }
            }
        }

        public string RelativePath
        {
            get
            {
                return monitoredItem.RelativePath;
            }
            set
            {
                if (String.Compare(value, monitoredItem.RelativePath, false) == 0)
                    return;

                monitoredItem.RelativePath = value;
                OnPropertyChanged("RelativePath");
            }
        }

        public int SamplingInterval
        {
            get
            {
                return monitoredItem.SamplingInterval;
            }
            set
            {
                if (value == monitoredItem.SamplingInterval)
                    return;

                monitoredItem.SamplingInterval = value;
                monitoredItemChangedTime = DateTime.UtcNow;
                OnPropertyChanged("SamplingInterval");

                lock (lockObject)
                {
                    if (mapChildItems != null)
                    {
                        foreach (var v in mapChildItems.Values)
                            v.SamplingInterval = value;
                    }
                }
            }
        }

        public MonitoredItemStatus Status
        {
            get
            {
                return monitoredItem.Status;
            }
        }

        bool _auditingDataEnabled;
        public bool AuditingDataEnabled
        {
            get
            {
                return _auditingDataEnabled;
            }
            set
            {
                if (value == _auditingDataEnabled)
                    return;

#if !WINDOWS_UWP
                if (value == true)
                    CreateDataConnection();
#endif

                _auditingDataEnabled = value;
                OnPropertyChanged("AuditingDataEnabled");

                lock (lockObject)
                {
                    if (mapChildItems != null)
                    {
                        foreach (var v in mapChildItems.Values)
                            v.AuditingDataEnabled = value;
                    }
                }
            }
        }

        NodeIdViewModel nodeIdModel;
        public NodeIdViewModel NodeIdModel
        {
            get
            {
                if (monitoredItem == null)
                    return null;

                if (nodeIdModel == null)
                    nodeIdModel = new NodeIdViewModel(monitoredItem.ResolvedNodeId, GetSubscriptionViewModelParent().GetSessionViewModelParent());
                return nodeIdModel;
            }
        }

        public String SessionName
        {
            get
            {
                var model = GetSubscriptionViewModelParent().GetSessionViewModelParent();
                if (model == null)
                    return String.Empty;
                return model.GetComposedTitle().ToString();
            }
        }

        public bool HasRange
        {
            get
            {
                return NodeIdModel != null && NodeIdModel.Range != null;
            }
        }

        Range _range;
        public Range Range
        {
            get
            {
                if (_range == null)
                {
                    if (NodeIdModel != null)
                        _range = NodeIdModel.Range;
                    if (_range == null)
                        _range = new Range(Double.MinValue, Double.MaxValue);
                }
                return _range;
            }
        }

        EUInformation _EUInformation;
        public EUInformation EUInformation
        {
            get
            {
                if (_EUInformation == null)
                    _EUInformation = NodeIdModel?.EUInformation;
                return _EUInformation;
            }
        }

        LocalizedText[] _EnumStrings;
        public LocalizedText[] EnumStrings
        {
            get
            {
                if (_EnumStrings == null && NodeIdModel != null)
                    _EnumStrings = NodeIdModel.EnumStrings;
                return _EnumStrings;
            }
        }

        String _dataType;
        public String DataType
        {
            get
            {
                try
                {
                    if (_dataType == null && NodeIdModel != null)
                        _dataType = (from q in NodeIdModel.GetReadableAttributesList()
                                     where q.attributeId == Attributes.DataType
                                     select q.Value).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    
                }
                return _dataType;
            }
        }

        long arrayDimension;
        internal long ArrayDimension
        {
            get
            {
                if (arrayDimension > 0)
                    return arrayDimension;
                try
                {
                    if (monitoredItem != null && monitoredItem.AttributeId == Attributes.Value)
                    {
                        VariableNode vnode = monitoredItem.Subscription.Session.NodeCache.Find(monitoredItem.ResolvedNodeId) as VariableNode;

                        if (vnode != null)
                        {
                            arrayDimension = (long)((vnode.ArrayDimensions != null && vnode.ArrayDimensions.Count > 0) ? vnode.ArrayDimensions[0] : 1);
                            return arrayDimension;
                        }
                    }
                }
                catch { }

                arrayDimension = 1;
                return arrayDimension;
            }
        }
#endregion

#if !WINDOWS_UWP && !NET_STANDARD
#region Validations

        protected override String PerformValidation(String propertyName)
        {
            if (propertyName == "Value" || propertyName == "DataValue" || propertyName == "InvariantCultureValue")
            {
                if(!Opc.Ua.StatusCode.IsGood(DataValue.StatusCode))
                    return LastMessage;
            }

            return base.PerformValidation(propertyName);
        }

        public override string Error
        {
            get
            {
                return null;
            }
        }

        public override string this[string propertyName]
        {
            get
            {
                return PerformValidation(propertyName);
            }
        }
#endregion
#endif

#region IEntityReference Members

        public ImageSource CollapsedImageSource
        {
            get
            {
#if !WINDOWS_UWP && !NET_STANDARD
                //BitmapImage bm = new BitmapImage();
                //bm.BeginInit();
                //Assembly assembly = Assembly.GetExecutingAssembly();

                //String str = String.Format("pack://application:,,,/{0};component/Images/table_16x16.png",
                //    Path.GetFileNameWithoutExtension(assembly.Location));
                //bm.UriSource = new Uri(str);
                //bm.EndInit();
                var bm = SharedResources.Helpers.ResourceManager.GetCommonImage(Properties.Settings.Default.TypeLabel, $"OPCUAVMTable", false);

                return bm;
#else
                return null;
#endif
            }
        }

        public Object Tooltip
        {
            get
            {
#if !WINDOWS_UWP && !NET_STANDARD
                if (SelectClauses != null)
                    return null;
                else
                    return new OPCUAViewModel.UserControls.MonitoredItemViewModel();
#else
                return null;
#endif
            }
        }

        public ImageSource ExpandedImageSource { get { return null; } }
        public ContextMenu contextMenu { get { return null; } }

        public object ContainedObject
        {
            get
            {
                return monitoredItem;
            }
        }

        public object EntityParent
        {
            get 
            {
                return Parent;
            }
        }

        public String TypeDefinitionString
        {
            get { return null; }
        }

#endregion

#if !WINDOWS_UWP
#region Data Persistance

        DevExpress.Xpo.IDataLayer dl;
        readonly Object lockData = new Object();
        List<DataValue> listDataValues;
        void CreateDataConnection()
        {
            lock (lockData)
            {
                if (dl != null)
                    return;

                try
                {
                    string conn = DevExpress.Xpo.DB.MSSqlConnectionProvider.GetConnectionString("(local)", GetComposedTitle().Replace('\\', '.'));
                    DevExpress.Xpo.Metadata.XPDictionary dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
                    DevExpress.Xpo.DB.IDataStore store = DevExpress.Xpo.XpoDefault.GetConnectionProvider(conn, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);

                    dict.GetDataStoreSchema(typeof(Persistance.DataItem).Assembly);

                    dl = new DevExpress.Xpo.ThreadSafeDataLayer(dict, store);
                }
                catch (Exception ex)
                {
                    LastMessage = String.Format("{0} - {1}, Error : {2}", Title, Properties.Resource.OPCUAMonitoredItemOpeningDataConnection, ex);
                    Utils.Trace(ex, Properties.Resource.OPCUAMonitoredItemOpeningDataConnection);

                    if (dl != null)
                    {
                        dl.Dispose();
                        dl = null;
                    }
                }
            }
        }

        List<DataValue> bufferValueHistory;
        void AddNewDataItem()
        {
            lock (lockData)
            {
                if (_dataValueHistory != null)
                {
                    if (bufferValueHistory == null)
                        bufferValueHistory = new List<DataValue>();
                    bufferValueHistory.Add(DataValue);
                    if (bufferValueHistory.Count > MaxAuditEvents)
                        bufferValueHistory.RemoveRange(0, bufferValueHistory.Count - MaxAuditEvents);
                    PromoteIdleExecution(1000);
                }
            }

            if (!AuditingDataEnabled)
                return;

            lock (lockData)
            {
                if (listDataValues == null)
                    listDataValues = new List<DataValue>();
                listDataValues.Add(DataValue);
                PromoteIdleExecution(500);
            }
        }

        void FlushDataItems()
        {
            if (bufferValueHistory != null)
            {
                var v1 = new List<DataValue>();
                lock (lockData)
                {
                    v1.AddRange(bufferValueHistory);
                    bufferValueHistory.Clear();
                }

                //_dataValueHistory.AddRange(v1);
                //if (_dataValueHistory.Count > MaxAuditEvents)
                //    _dataValueHistory.RemoveRange(0, _dataValueHistory.Count - MaxAuditEvents);
                //OnPropertyChanged("DataValueHistory");
                using (var updater = new CollectionUpdater(_dataValueHistory))
                {
                    v1.ForEach(data =>
                        {
                            _dataValueHistory.Add(data);
                            while (_dataValueHistory.Count > MaxAuditEvents)
                                _dataValueHistory.RemoveAt(0);
                        });
                }
            }

            if (listDataValues == null)
                return;

            var v = new List<DataValue>();
            lock (lockData)
            {
                v.AddRange(listDataValues);
                listDataValues.Clear();
            }

            try
            {
                using (DevExpress.Xpo.UnitOfWork ufw = new DevExpress.Xpo.UnitOfWork(dl))
                {
                    v.ForEach(itemData =>
                                {
                                    Persistance.DataItem item = new Persistance.DataItem(ufw);

                                    item.SourceTimeStamp = itemData.SourceTimestamp.ToLocalTime();
                                    item.SourcePicoseconds = itemData.SourcePicoseconds;
                                    item.ServerTimeStamp = itemData.ServerTimestamp.ToLocalTime();
                                    item.ServerPicoseconds = itemData.ServerPicoseconds;

                                    item.Value = String.Format("{0}", itemData.WrappedValue);
                                    item.Status = String.Format("{0}", itemData.StatusCode);
                                });

                    ufw.CommitChanges();
                }

                using (DevExpress.Xpo.UnitOfWork ufw = new DevExpress.Xpo.UnitOfWork(dl))
                {
                    using (DevExpress.Xpo.XPCollection colDelete = new DevExpress.Xpo.XPCollection(typeof(Persistance.DataItem),
                                    new DevExpress.Data.Filtering.BinaryOperator(
                                    new DevExpress.Data.Filtering.OperandProperty("SourceTimeStamp"),
                                    new DevExpress.Data.Filtering.OperandValue(DateTime.Now - TimeSpan.FromDays(7)),
                                    DevExpress.Data.Filtering.BinaryOperatorType.Less)))
                    {
                        ufw.Delete(colDelete);
                        ufw.CommitChangesAndDropIdentityMap();
                    }
                }
            }
            catch (Exception ex)
            {
                LastMessage = String.Format("{0} - {1}, Error : {2}", Title, Properties.Resource.OPCUAMonitoredItemFlushingData, ex);
                Utils.Trace(ex, Properties.Resource.OPCUAMonitoredItemFlushingData);
            }
        }

#endregion
#endif
    }
}
