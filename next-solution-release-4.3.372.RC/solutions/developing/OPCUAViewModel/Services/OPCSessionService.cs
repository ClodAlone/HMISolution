using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OPCUAViewModel.Services
{
    public class OPCSessionService : IOPCSessionService
    {
        public bool WriteValue(object value,
            MonitoredItemViewModel monitoredItemViewModel,
            object lockObject,
            int indexArray = -1,
            int indexBit = -1)
        {
            return WriteValues(new List<object> { value },
                new List<MonitoredItemViewModel> { monitoredItemViewModel },
                lockObject,
                indexArray,
                indexBit);
        }


        public IList<MonitoredItemValue> ReadValues(IList<MonitoredItemViewModel> monitoredItemViewModels)
        {
            var monitoredItemsBySession = GetViewModelsBySession(monitoredItemViewModels, out _);
            var dict = new List<MonitoredItemValue>();
            foreach (var session in monitoredItemsBySession.Keys)
            {
                var mItemViewModels = monitoredItemsBySession[session].Select(x => x.ViewModel).ToList();
                var nodeIdList = mItemViewModels.Select(x => x.monitoredItem.ResolvedNodeId).ToList();
                var nodeIdTypeList = mItemViewModels.Select(x => typeof(object)).ToList();
                session.ReadValues(nodeIdList, nodeIdTypeList, out var readValues, out var results);
                if (results != null && results.Any(x => StatusCode.IsNotGood(x.StatusCode)))
                {
                    var firstError = results.FirstOrDefault(x => StatusCode.IsNotGood(x.StatusCode));
                    throw new Exception(firstError?.AdditionalInfo ?? firstError?.InnerResult?.AdditionalInfo);
                }

                dict.AddRange(mItemViewModels.Select((x, i) =>
                    new MonitoredItemValue(mItemViewModels[i], new Variant(readValues[i]))));
            }

            return dict;
        }

        public void WriteValues(IList<MonitoredItemViewModel> monitoredItemViewModels,
            IList<object> values)
        {
            var monitoredItemsBySession = GetViewModelsBySession(monitoredItemViewModels,
                out var localTags,
                values);

            WriteLocalTags(localTags);
            WriteOPCTags(monitoredItemsBySession);
        }

        private static void WriteOPCTags(Dictionary<Session, IList<MonitoredItemValue>> monitoredItemsBySession)
        {
            foreach (var session in monitoredItemsBySession.Keys)
            {
                if (session == null)
                    throw new NullReferenceException("Session cannot be null while writing new values");

                var currentMonitoredItemValue = monitoredItemsBySession[session][0];
                var sessionViewModel = GetSessionViewModel(currentMonitoredItemValue.ViewModel);

                var valuesList = monitoredItemsBySession[session]
                    .Select(x => x.Value)
                    .ToArray();

                var opcMonitoredItemViewModels = monitoredItemsBySession[session]
                    .Select(x => x.ViewModel)
                    .ToList();

                WriteValues(valuesList, opcMonitoredItemViewModels, sessionViewModel.lockObject);
            }
        }

        private static void WriteLocalTags(IList<MonitoredItemValue> localTags)
        {
            if (!localTags.Any()) return;
            var localTagValues = localTags.Select(x => x.Value).ToArray();
            var localTagViewModels = localTags.Select(x => x.ViewModel).ToList();
            WriteValues(localTagValues, localTagViewModels, null);
        }


        private static bool WriteValues(IList<object> values,
            IList<MonitoredItemViewModel> monitoredItemViewModels,
            object lockObject,
            int indexArray = -1,
            int indexBit = -1)
        {
            try
            {
                return InternalWrite(values,
                    monitoredItemViewModels,
                    lockObject,
                    indexArray,
                    indexBit);
            }
            catch (ServiceResultException ex)
            {
                if ((indexArray >= 0 || indexBit >= 0) && ex.StatusCode == StatusCodes.BadWriteNotSupported)
                {
                    // opc ua server doesn't support write via IndexRange.
                    return InternalWrite(values,
                        monitoredItemViewModels,
                        lockObject,
                        -1,
                        indexBit,
                        false);
                }

                if (indexBit >= 0 && (ex.StatusCode == StatusCodes.BadTypeMismatch ||
                                      ex.StatusCode == StatusCodes.BadIndexRangeInvalid ||
                                      ex.StatusCode == StatusCodes.BadIndexRangeNoData))
                {
                    // opc ua server doesn't support write via IndexRange for bits.
                    return InternalWrite(values,
                        monitoredItemViewModels,
                        lockObject,
                        indexArray,
                        indexBit,
                        false);
                }

                throw;
            }
        }

        private static Dictionary<Session, IList<MonitoredItemValue>> GetViewModelsBySession(
            IList<MonitoredItemViewModel> monitoredItemViewModels,
            out IList<MonitoredItemValue> localTags,
            IList<object> values = null)
        {
            localTags = new List<MonitoredItemValue>();
            var viewModelsBySession = new Dictionary<Session, IList<MonitoredItemValue>>();
            for (var i = 0; i < monitoredItemViewModels.Count; i++)
            {
                var currentViewModel = monitoredItemViewModels[i];

                var monitoredItemValue = values != null && values.Count > i
                    ? new MonitoredItemValue(currentViewModel, values[i])
                    : new MonitoredItemValue(currentViewModel);

                if (currentViewModel.monitoredItem == null)
                {
                    localTags.Add(monitoredItemValue);
                }
                else
                {
                    var session = GetSession(currentViewModel);
                    if (!viewModelsBySession.ContainsKey(session))
                        viewModelsBySession.Add(session, new List<MonitoredItemValue>());
                    viewModelsBySession[session].Add(monitoredItemValue);
                }
            }

            return viewModelsBySession;
        }


        private static bool InternalWrite(IList<object> values,
            IList<MonitoredItemViewModel> monitoredItemViewModels,
            object lockObject,
            int indexArray = -1,
            int indexBit = -1,
            bool useIndexRange = true)
        {
            if (values == null ||
                monitoredItemViewModels == null ||
                !values.Any() ||
                !monitoredItemViewModels.Any() ||
                values.Count != monitoredItemViewModels.Count)
                throw new ArgumentException("Values list and monitored items must have same dimensions");

            InternalWriteLocalTags(values,
                monitoredItemViewModels,
                indexArray,
                indexBit);

            InternalWriteOnSessionTags(values,
                monitoredItemViewModels,
                lockObject,
                indexArray,
                indexBit,
                useIndexRange);

            return true;
        }


        private static void InternalWriteLocalTags(IList<object> values,
            IList<MonitoredItemViewModel> monitoredItemViewModels,
            int indexArray = -1,
            int indexBit = -1)
        {
            for (var i = 0; i < values.Count; i++)
            {
                var currentViewModel = monitoredItemViewModels[i];
                var monitoredItem = currentViewModel.monitoredItem;
                var v = values[i];
                if (monitoredItem != null) continue;

                lock (monitoredItemViewModels[i].lockObject)
                {
                    if (currentViewModel.IsReadOnly)
                        throw new Exception(
                            "This is a temporary readonly item, " +
                            "please wait for the real connected item to write");

                    if (currentViewModel.DataValue == null)
                        currentViewModel.DataValue = new DataValue(new Variant(v), StatusCodes.Good);
                    else if (v != null && currentViewModel.DataValue.Value != null)
                    {
                        uint arrayDimension = 0;
                        Type type;
                        if (currentViewModel.DataValue.Value is Array valueValue)
                        {
                            arrayDimension = (uint)valueValue.GetUpperBound(0) + 1;
                            type = valueValue.GetValue(0).GetType();
                        }
                        else
                            type = currentViewModel.DataValue.Value.GetType();

                        var builtinType = MonitoredItemViewModel.GetBuiltInType(type.Name);
                        if (builtinType == BuiltInType.Double || builtinType == BuiltInType.Float)
                        {
                            if (v is string s)
                            {
                                var culture =
                                    new CultureInfo(CultureInfo.CurrentCulture.Name, true);
                                v = s.Replace(culture.NumberFormat.NumberDecimalSeparator, ".");
                            }
                        }

                        try
                        {
                            try
                            {
                                v = ChangeTypeHelper.ChangeType(v, builtinType,
                                    arrayDimension); // pre-change type base on current thread localization first
                            }
                            catch (Exception)
                            {
                                if (v is string && builtinType != BuiltInType.String &&
                                    (builtinType == BuiltInType.Int64 || builtinType == BuiltInType.UInt64))
                                {
                                    var info =
                                        new NumberFormatInfo
                                        {
                                            NumberDecimalSeparator = ".",
                                            NumberGroupSeparator = ","
                                        };
                                    v = Convert.ToInt64(v, info);
                                }

                                string binary;
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
                                        binary = Convert.ToString((long)v, 2);
                                        // binary = binary.Substring(Math.Max(0, binary.Length - 63));
                                        v = Convert.ToInt64(binary, 2);
                                        break;
                                    case BuiltInType.UInt64:
                                    case BuiltInType.UInteger:
                                        binary = Convert.ToString((long)v, 2);
                                        // binary = binary.Substring(Math.Max(0, binary.Length - 63));
                                        v = Convert.ToUInt64(binary, 2);
                                        break;
                                }
                            }

                            if (arrayDimension == 0)
                            {
                                if (indexBit >= 0 && TypeInfo.IsNumericType(builtinType))
                                {
                                    v = GetNewValue(v, currentViewModel.DataValue.Value, indexBit, builtinType,
                                        out _);
                                    currentViewModel.DataValue = new DataValue(new Variant(v), StatusCodes.Good,
                                        DateTime.UtcNow);
                                }
                                else
                                    currentViewModel.DataValue = new DataValue(
                                        new Variant(TypeInfo.Cast(v, builtinType)),
                                        StatusCodes.Good, DateTime.UtcNow);
                            }
                            else if (v is Array array)
                            {
                                if (indexArray >= 0 && currentViewModel.DataValue.Value is Array dValue)
                                {
                                    var vValue = array.GetValue(indexArray);
                                    if (indexBit >= 0 && TypeInfo.IsNumericType(builtinType))
                                    {
                                        vValue = GetNewValue(vValue, dValue.GetValue(indexArray), indexBit,
                                            builtinType, out _);
                                    }

                                    dValue.SetValue(vValue, indexArray);
                                    currentViewModel.DataValue = new DataValue(
                                        new Variant(TypeInfo.CastArray(dValue, builtinType, builtinType,
                                            ChangeTypeHelper.CastArrayElement)), StatusCodes.Good,
                                        DateTime.UtcNow);
                                }
                                else
                                    currentViewModel.DataValue = new DataValue(
                                        new Variant(TypeInfo.CastArray(array, builtinType, builtinType,
                                            ChangeTypeHelper.CastArrayElement)), StatusCodes.Good,
                                        DateTime.UtcNow);
                            }
                        }
                        catch (Exception ex)
                        {
                            currentViewModel.LastMessage =
                                $"{currentViewModel.Title} - {Properties.Resource.OPCUAMonitoredItemWriteError}, " +
                                $"Value : {currentViewModel.Value}, Error : {ex.Message}";
                            Utils.Trace(ex, Properties.Resource.MonitoredTemporaryItemWriteError);
                            throw;
                        }
                    }
                }
            }
        }

        private static void InternalWriteOnSessionTags(IList<object> values,
            IList<MonitoredItemViewModel> monitoredItemViewModels,
            object lockObject,
            int indexArray = -1,
            int indexBit = -1,
            bool useIndexRange = true)
        {
            if (lockObject == null) return;

            lock (lockObject)
            {
                Session session = null;
                var valuesToWrite = new List<WriteValue>();
                var newValuesToWrite = new List<object>();

                for (var i = 0; i < values.Count; i++)
                {
                    var currentViewModel = monitoredItemViewModels[i];

                    var monitoredItem = currentViewModel.monitoredItem;
                    if (monitoredItem == null) continue;
                    if (!Properties.Settings.Default.UseIndexRangeForBitsOnWriting)
                        useIndexRange = false;

                    if (session == null)
                        session = GetSession(currentViewModel);

                    var writeValueObject = BuildWriteValue(session,
                        indexArray,
                        indexBit,
                        useIndexRange,
                        monitoredItem,
                        currentViewModel,
                        values[i]);

                    valuesToWrite.Add(writeValueObject.WriteValue);
                    newValuesToWrite.Add(writeValueObject.NewValue);
                }

                WriteOnSession(monitoredItemViewModels, valuesToWrite, session);

                for (var i = 0; i < valuesToWrite.Count; i++)
                {
                    var currentViewModel = monitoredItemViewModels[i];
                    var value = valuesToWrite[i];
                    if (currentViewModel.DataValue != null)
                    {
                        value.Value.ServerTimestamp = currentViewModel.DataValue.ServerTimestamp;
                        value.Value.SourcePicoseconds = currentViewModel.DataValue.SourcePicoseconds;
                        value.Value.SourceTimestamp = currentViewModel.DataValue.SourceTimestamp;
                        value.Value.SourcePicoseconds = currentViewModel.DataValue.SourcePicoseconds;
                    }

                    if (newValuesToWrite[i] != null)
                        value.Value.Value = newValuesToWrite[i];

                    //log.DebugFormat("WriteValue : old value = {0}, new value = {1}, Time = {2}", DataValue, value.Value, DateTime.UtcNow);
                    currentViewModel.DataValue = value.Value;
                }
            }
        }


        private static SessionViewModel GetSessionViewModel(MonitoredItemViewModel monitoredItemViewModel)
        {
            return monitoredItemViewModel?.GetSubscriptionViewModelParent()?.GetSessionViewModelParent();
        }

        private static Session GetSession(MonitoredItemViewModel monitoredItemViewModel)
        {
            var session = GetSessionViewModel(monitoredItemViewModel)?.Session;
            if (session == null)
                throw new NullReferenceException("Session cannot be null while writing a new value.");
            return session;
        }

        private static void WriteOnSession(IList<MonitoredItemViewModel> monitoredItemViewModels,
            IList<WriteValue> valuesToWrite,
            Session session)
        {
            try
            {
                if (valuesToWrite == null || !valuesToWrite.Any())
                    return;

                var writeCollection = new WriteValueCollection(valuesToWrite);

                var responseHeader = session.Write(
                    null,
                    writeCollection,
                    out var writeResults,
                    out var diagnosticInfos);

                ClientBase.ValidateResponse(writeResults, writeCollection);
                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, writeCollection);

                StatusCode? errorCode = null;
                for (var i = 0; i < writeResults.Count; i++)
                {
                    var sc = writeResults[i];
                    var currentViewModel = monitoredItemViewModels[i];

                    if (StatusCode.IsGood(sc))
                    {
                        // LastMessage = String.Format("{0} - {1}", Title, Properties.Resource.OPCUAMonitoredItemWriteSuccessful);
                        // AddOutputText(String.Format("Write Successfully to {0}", mi.DisplayName), false);
                        if (currentViewModel.DataValue != null &&
                            StatusCode.IsGood(currentViewModel.DataValue.StatusCode))
                            currentViewModel.LastMessage = string.Empty;
                    }
                    else
                    {
                        if (!errorCode.HasValue)
                            errorCode = sc;

                        currentViewModel.LastMessage =
                            $"{currentViewModel.Title} - {Properties.Resource.OPCUAMonitoredItemWriteFailure}, " +
                            $"Reason : {sc}";
                    }
                }

                if (errorCode.HasValue)
                    throw new ServiceResultException(new ServiceResult(errorCode.Value, null,
                        responseHeader.StringTable));
            }
            catch (Exception ex)
            {
                foreach (var currentViewModel in monitoredItemViewModels)
                {
                    currentViewModel.LastMessage =
                        $"{currentViewModel.Title} - {Properties.Resource.OPCUAMonitoredItemWriteError}, " +
                        $"Value : {currentViewModel.Value}, Error : {ex.Message}";
                }

                Utils.Trace(ex, Properties.Resource.OPCUAMonitoredItemWriteError);
                throw;
            }
        }

        private static (WriteValue WriteValue, object NewValue) BuildWriteValue(Session session,
            int indexArray,
            int indexBit,
            bool useIndexRange,
            MonitoredItem monitoredItem,
            MonitoredItemViewModel currentViewModel,
            object v)
        {
            var value = new WriteValue
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

            var datatypeId = Attributes.GetDataTypeId(monitoredItem.AttributeId);
            var valueRank = Attributes.GetValueRank(monitoredItem.AttributeId);
            uint arraySizeOneDimension = 0;

            if (monitoredItem.AttributeId == Attributes.Value)
            {
                if (session != null)
                {
                    if (session.NodeCache.Find(value.NodeId) is VariableNode vnode)
                    {
                        datatypeId = vnode.DataType;
                        valueRank = vnode.ValueRank;
                        arraySizeOneDimension =
                            (vnode.ArrayDimensions != null && vnode.ArrayDimensions.Count > 0)
                                ? vnode.ArrayDimensions[0]
                                : 0;
                    }
                }
            }

            var builtinType = TypeInfo.GetBuiltInType(datatypeId, session.TypeTree);
            object newValue = null;

            try
            {
                v = ChangeTypeHelper.ChangeType(v, builtinType,
                    arraySizeOneDimension); // prechange type base on currenthread localization first
                if (builtinType == BuiltInType.ExtensionObject)
                {
                    value.Value = (DataValue)v;
                }
                else if (arraySizeOneDimension == 0)
                {
                    if (indexBit >= 0 && TypeInfo.IsNumericType(builtinType))
                    {
                        if (useIndexRange)
                        {
                            newValue = GetNewValue(v, currentViewModel.DataValue?.Value, indexBit, builtinType,
                                out var bit);
                            value.IndexRange = Convert.ToString(indexBit, CultureInfo.InvariantCulture);
                            value.Value = new DataValue(new Variant(TypeInfo.Cast(bit, builtinType)));
                        }
                        else
                        {
                            var datavalue = session.ReadValue(monitoredItem.ResolvedNodeId);
                            v = GetNewValue(v, datavalue?.Value, indexBit, builtinType, out _);
                            value.Value = new DataValue(new Variant(v));
                        }
                    }
                    else
                        value.Value = new DataValue(new Variant(TypeInfo.Cast(v, builtinType)));
                }
                else if (v is Array array)
                {
                    if (indexArray >= 0)
                    {
                        var vValue = array.GetValue(indexArray);
                        string subrange = null;
                        if (indexBit >= 0 && TypeInfo.IsNumericType(builtinType))
                        {
                            if (useIndexRange)
                            {
                                object currentValue = null;
                                if (currentViewModel.DataValue != null &&
                                    currentViewModel.DataValue.Value is Array valueValue)
                                    currentValue = valueValue.GetValue(indexArray);
                                vValue = GetNewValue(vValue, currentValue, indexBit, builtinType, out var bit);
                                subrange = Convert.ToString(indexBit, CultureInfo.InvariantCulture);
                                array.SetValue(vValue, indexArray);
                                value.Value = new DataValue(new Variant(TypeInfo.Cast(bit, builtinType)));
                                if (currentViewModel.DataValue != null &&
                                    currentViewModel.DataValue.Value is Array dataValueValue)
                                {
                                    newValue = TypeInfo.CastArray(dataValueValue, builtinType,
                                        builtinType, ChangeTypeHelper.CastArrayElement);
                                    (newValue as Array).SetValue(vValue, indexArray);
                                }
                            }
                            else
                            {
                                object currentValue = null;
                                var datavalue = session.ReadValue(monitoredItem.ResolvedNodeId);
                                if (datavalue != null && datavalue.Value is Array datavalueValue)
                                    currentValue = datavalueValue.GetValue(indexArray);
                                vValue = GetNewValue(vValue, currentValue, indexBit, builtinType, out _);
                                array.SetValue(vValue, indexArray);
                                var element = TypeInfo.CreateArray(builtinType, 1);
                                element.SetValue(vValue, 0);
                                value.Value = new DataValue(new Variant(element));
                                if (datavalue != null && datavalue.Value is Array array1)
                                {
                                    newValue = TypeInfo.CastArray(array1, builtinType,
                                        builtinType, ChangeTypeHelper.CastArrayElement);
                                    (newValue as Array).SetValue(vValue, indexArray);
                                }
                            }
                        }
                        else
                        {
                            var element = TypeInfo.CreateArray(builtinType, 1);
                            element.SetValue(vValue, 0);
                            value.Value = new DataValue(new Variant(element));
                            if (currentViewModel.DataValue != null &&
                                currentViewModel.DataValue.Value is Array valueValue)
                            {
                                newValue = TypeInfo.CastArray(valueValue, builtinType,
                                    builtinType, ChangeTypeHelper.CastArrayElement);
                                (newValue as Array).SetValue(vValue, indexArray);
                            }
                        }

                        value.IndexRange = Convert.ToString(indexArray, CultureInfo.InvariantCulture);
                        if (subrange != null)
                            value.IndexRange = $"{value.IndexRange},{subrange}";
                        if (newValue == null)
                            newValue = TypeInfo.CastArray(array, builtinType, builtinType,
                                ChangeTypeHelper.CastArrayElement);
                    }
                    else
                        value.Value = new DataValue(new Variant(TypeInfo.CastArray(array, builtinType,
                            builtinType, ChangeTypeHelper.CastArrayElement)));
                }
            }
            catch (Exception exception)
            {
                try
                {
                    if (v is string && builtinType != BuiltInType.String &&
                        (builtinType == BuiltInType.Int64 || builtinType == BuiltInType.UInt64))
                    {
                        var info = new NumberFormatInfo
                        {
                            NumberDecimalSeparator = ".",
                            NumberGroupSeparator = ","
                        };
                        v = Convert.ToInt64(v, info);
                    }

                    string binary;
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
                            binary = Convert.ToString((long)v, 2);
                            // binary = binary.Substring(Math.Max(0, binary.Length - 63));
                            v = Convert.ToInt64(binary, 2);
                            break;
                        case BuiltInType.UInt64:
                        case BuiltInType.UInteger:
                            binary = Convert.ToString((long)v, 2);
                            // binary = binary.Substring(Math.Max(0, binary.Length - 63));
                            v = Convert.ToUInt64(binary, 2);
                            break;
                    }

                    if (arraySizeOneDimension == 0)
                    {
                        if (indexBit >= 0 && TypeInfo.IsNumericType(builtinType))
                        {
                            if (useIndexRange)
                            {
                                newValue = GetNewValue(v, currentViewModel.DataValue?.Value, indexBit,
                                    builtinType, out var bit);
                                value.IndexRange = Convert.ToString(indexBit, CultureInfo.InvariantCulture);
                                value.Value = new DataValue(new Variant(TypeInfo.Cast(bit, builtinType)));
                            }
                            else
                            {
                                var datavalue = session.ReadValue(monitoredItem.ResolvedNodeId);
                                v = GetNewValue(v, datavalue?.Value, indexBit, builtinType, out _);
                                value.Value = new DataValue(new Variant(v));
                            }
                        }
                        else
                            value.Value = new DataValue(new Variant(TypeInfo.Cast(v, builtinType)));
                    }
                    else if (v is Array array)
                    {
                        if (indexArray >= 0)
                        {
                            var vValue = array.GetValue(indexArray);
                            string subrange = null;
                            if (indexBit >= 0 && TypeInfo.IsNumericType(builtinType))
                            {
                                if (useIndexRange)
                                {
                                    object currentValue = null;
                                    if (currentViewModel.DataValue != null &&
                                        currentViewModel.DataValue.Value is Array valueValue)
                                        currentValue = valueValue.GetValue(indexArray);
                                    vValue = GetNewValue(vValue, currentValue, indexBit, builtinType,
                                        out var bit);
                                    subrange = Convert.ToString(indexBit, CultureInfo.InvariantCulture);
                                    array.SetValue(vValue, indexArray);
                                    value.Value =
                                        new DataValue(new Variant(TypeInfo.Cast(bit, builtinType)));
                                    if (currentViewModel.DataValue != null &&
                                        currentViewModel.DataValue.Value is Array dataValueValue)
                                    {
                                        newValue = TypeInfo.CastArray(dataValueValue, builtinType,
                                            builtinType, ChangeTypeHelper.CastArrayElement);
                                        (newValue as Array).SetValue(vValue, indexArray);
                                    }
                                }
                                else
                                {
                                    object currentValue = null;
                                    var datavalue = session.ReadValue(monitoredItem.ResolvedNodeId);
                                    if (datavalue != null && datavalue.Value is Array datavalueValue)
                                        currentValue = datavalueValue.GetValue(indexArray);
                                    vValue = GetNewValue(vValue, currentValue, indexBit, builtinType, out _);
                                    array.SetValue(vValue, indexArray);
                                    var element = TypeInfo.CreateArray(builtinType, 1);
                                    element.SetValue(vValue, 0);
                                    value.Value = new DataValue(new Variant(element));
                                    if (datavalue != null && datavalue.Value is Array array1)
                                    {
                                        newValue = TypeInfo.CastArray(array1, builtinType,
                                            builtinType, ChangeTypeHelper.CastArrayElement);
                                        (newValue as Array).SetValue(vValue, indexArray);
                                    }
                                }
                            }
                            else
                            {
                                var element = TypeInfo.CreateArray(builtinType, 1);
                                element.SetValue(vValue, 0);
                                value.Value = new DataValue(new Variant(element));
                                if (currentViewModel.DataValue != null &&
                                    currentViewModel.DataValue.Value is Array valueValue)
                                {
                                    newValue = TypeInfo.CastArray(valueValue, builtinType,
                                        builtinType, ChangeTypeHelper.CastArrayElement);
                                    (newValue as Array).SetValue(vValue, indexArray);
                                }
                            }

                            value.IndexRange = Convert.ToString(indexArray, CultureInfo.InvariantCulture);
                            if (subrange != null)
                                value.IndexRange = $"{value.IndexRange},{subrange}";
                            if (newValue == null)
                                newValue = TypeInfo.CastArray(array, builtinType, builtinType,
                                    ChangeTypeHelper.CastArrayElement);
                        }
                        else
                            value.Value = new DataValue(new Variant(TypeInfo.CastArray(array, builtinType,
                                builtinType, ChangeTypeHelper.CastArrayElement)));
                    }
                    else
                        throw;
                }
                catch (Exception ex)
                {
                    currentViewModel.LastMessage =
                        $"{currentViewModel.Title} - {Properties.Resource.OPCUAMonitoredItemWriteError}," +
                        $" Value : {currentViewModel.Value}, Error : {ex.Message}";
                    Utils.Trace(ex, Properties.Resource.OPCUAMonitoredItemWriteError);
                    throw;
                }
            }

            value.Value.StatusCode = StatusCodes.Good;
            value.Value.ServerTimestamp = DateTime.MinValue;
            value.Value.SourceTimestamp = DateTime.MinValue;

            return (value, newValue);
        }

        private static object GetNewValue(object v,
            object currentValue,
            int indexBit,
            BuiltInType builtinType,
            out bool bit)
        {
            switch (builtinType)
            {
                case BuiltInType.Byte:
                    {
                        if (indexBit > 7)
                            throw new IndexOutOfRangeException(
                                string.Format(Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        var lValue = Convert.ToByte(v);
                        byte shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = Convert.ToByte(currentValue);
                            if (bit)
                                lValue |= (byte)(1 << indexBit);
                            else
                                lValue &= (byte)(~(1 << indexBit));
                            return TypeInfo.Cast(lValue, builtinType);
                        }

                        return TypeInfo.Cast(v, builtinType);
                    }
                case BuiltInType.SByte:
                    {
                        if (indexBit > 7)
                            throw new IndexOutOfRangeException(
                                string.Format(Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        var lValue = Convert.ToSByte(v);
                        sbyte shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = Convert.ToSByte(currentValue);
                            if (bit)
                                lValue |= (sbyte)(1 << indexBit);
                            else
                                lValue &= (sbyte)(~(1 << indexBit));
                            return TypeInfo.Cast(lValue, builtinType);
                        }

                        return TypeInfo.Cast(v, builtinType);
                    }
                case BuiltInType.Int16:
                    {
                        if (indexBit > 15)
                            throw new IndexOutOfRangeException(
                                string.Format(Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        var lValue = Convert.ToInt16(v);
                        short shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = Convert.ToInt16(currentValue);
                            if (bit)
                                lValue |= (short)(1 << indexBit);
                            else
                                lValue &= (short)(~(1 << indexBit));
                            return TypeInfo.Cast(lValue, builtinType);
                        }

                        return TypeInfo.Cast(v, builtinType);
                    }
                case BuiltInType.UInt16:
                    {
                        if (indexBit > 15)
                            throw new IndexOutOfRangeException(
                                string.Format(Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        var lValue = Convert.ToUInt16(v);
                        ushort shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = Convert.ToUInt16(currentValue);
                            if (bit)
                                lValue |= (ushort)(1 << indexBit);
                            else
                                lValue &= (ushort)(~(1 << indexBit));
                            return TypeInfo.Cast(lValue, builtinType);
                        }

                        return TypeInfo.Cast(v, builtinType);
                    }
                case BuiltInType.Int32:
                    {
                        if (indexBit > 31)
                            throw new IndexOutOfRangeException(
                                string.Format(Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        var lValue = Convert.ToInt32(v);
                        var shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = Convert.ToInt32(currentValue);
                            if (bit)
                                lValue |= 1 << indexBit;
                            else
                                lValue &= ~(1 << indexBit);
                            return TypeInfo.Cast(lValue, builtinType);
                        }

                        return TypeInfo.Cast(v, builtinType);
                    }
                case BuiltInType.UInt32:
                    {
                        if (indexBit > 31)
                            throw new IndexOutOfRangeException(
                                string.Format(Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        var lValue = Convert.ToUInt32(v);
                        uint shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = Convert.ToUInt32(currentValue);
                            if (bit)
                                lValue |= (uint)1 << indexBit;
                            else
                                lValue &= ~((uint)1 << indexBit);
                            return TypeInfo.Cast(lValue, builtinType);
                        }

                        return TypeInfo.Cast(v, builtinType);
                    }
                case BuiltInType.Int64:
                case BuiltInType.Integer:
                    {
                        if (indexBit > 63)
                            throw new IndexOutOfRangeException(
                                string.Format(Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        var lValue = Convert.ToInt64(v);
                        long shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = Convert.ToInt64(currentValue);
                            if (bit)
                                lValue |= (long)1 << indexBit;
                            else
                                lValue &= ~((long)1 << indexBit);
                            return TypeInfo.Cast(lValue, builtinType);
                        }

                        return TypeInfo.Cast(v, builtinType);
                    }
                case BuiltInType.UInt64:
                case BuiltInType.UInteger:
                    {
                        if (indexBit > 63)
                            throw new IndexOutOfRangeException(
                                string.Format(Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        var lValue = Convert.ToUInt64(v);
                        ulong shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = Convert.ToUInt64(currentValue);
                            if (bit)
                                lValue |= (ulong)1 << indexBit;
                            else
                                lValue &= ~((ulong)1 << indexBit);
                            return TypeInfo.Cast(lValue, builtinType);
                        }

                        return TypeInfo.Cast(v, builtinType);
                    }
                case BuiltInType.Float:
                    {
                        if (indexBit > 31)
                            throw new IndexOutOfRangeException(
                                string.Format(Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        var lValue = (long)Convert.ToSingle(v);
                        long shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = (long)Convert.ToSingle(currentValue);
                            if (bit)
                                lValue |= (long)1 << indexBit;
                            else
                                lValue &= ~((long)1 << indexBit);
                            return TypeInfo.Cast(lValue, builtinType);
                        }

                        return TypeInfo.Cast(v, builtinType);
                    }
                case BuiltInType.Double:
                case BuiltInType.Number:
                    {
                        if (indexBit > 63)
                            throw new IndexOutOfRangeException(
                                string.Format(Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        var lValue = (long)Convert.ToDouble(v);
                        long shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = (long)Convert.ToDouble(currentValue);
                            if (bit)
                                lValue |= (long)1 << indexBit;
                            else
                                lValue &= ~((long)1 << indexBit);
                            return TypeInfo.Cast(lValue, builtinType);
                        }

                        return TypeInfo.Cast(v, builtinType);
                    }
                default:
                    {
                        bit = false;
                        return v;
                    }
            }
        }

        public bool WriteSynchValue(NodeId tag, object value,
            MonitoredItemViewModel monitoredItemViewModel)
        {
            var session = GetSession(monitoredItemViewModel);
            if (session != null)
            {
                var methNodeId = UFUAServerInfo.UFUAServerInfo.GetWriteTagSynchNodeId();

                CallMethodRequestCollection methodsToCall = new CallMethodRequestCollection();

                CallMethodRequest request = new CallMethodRequest();
                
                request.ObjectId = UFUAServerInfo.UFUAServerInfo.GetTagRootNodeId();
                request.MethodId = methNodeId;
                request.InputArguments.Add(new Variant(tag));
                var newVal = new Variant(value);
                request.InputArguments.Add(newVal);

                methodsToCall.Add(request);

                CallMethodResultCollection results = null;
                DiagnosticInfoCollection diagnosticInfos = null;


                var responseHeader = session.Call(
                    null,
                    methodsToCall,
                    out results,
                    out diagnosticInfos);

                ClientBase.ValidateResponse(results, methodsToCall);
                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, methodsToCall);

                results.ForEach(res =>
                {
                    if (StatusCode.IsBad(res.StatusCode))
                    {
                        throw new ServiceResultException(new ServiceResult(res.StatusCode, 0, diagnosticInfos, responseHeader.StringTable));
                    }
                });

                monitoredItemViewModel.DataValue = new DataValue(newVal);
            }
            return true;
        }
    }
}