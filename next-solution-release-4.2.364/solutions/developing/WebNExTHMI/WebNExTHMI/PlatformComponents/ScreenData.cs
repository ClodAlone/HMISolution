using DocumentManager.ComponentService;
using Microsoft.AspNetCore.SignalR;
using Opc.Ua;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UFInterfaces;
using Utilities.Converters;
using ViewModelLib;
using Utilities;
using DevExpress.Xpo;
using System.Threading;

namespace WebNExTHMI.PlatformComponents
{
    public class ScreenDataInfo 
    {
        public ScreenSettings.WindowState WindowState { get; set; }
        public ScreenSettings.WindowStyle WindowStyle { get; set; }
        public bool FitInWindow { get; set; }
        public string ScreenTitle { get; set; }

        public ScreenDataInfo()
        {

        }
    }

    public class ScreenData : ViewModelBase, IEntityReference
    {
        List<OPCUAEntityReference> listReferences = new List<OPCUAEntityReference>();
        Dictionary<OPCUAEntityReference, Tuple<String, String>> mapReferenceExpressions =
            new Dictionary<OPCUAEntityReference, Tuple<String, String>>();
        Dictionary<int, OPCUAEntityReference> mapIdReferences =
            new Dictionary<int, OPCUAEntityReference>();

        List<PropertyObserver<OPCUAEntityReference>> listObserverReferences = new List<PropertyObserver<OPCUAEntityReference>>();

        Dictionary<OPCUAEntityReference, PropertyObserver<MonitoredItemViewModel>> mapObserverMonitoredModels = 
            new Dictionary<OPCUAEntityReference, PropertyObserver<MonitoredItemViewModel>>();

        Dictionary<OPCUAEntityReference, ExpressionValueConverter> mapReferenceConverters = 
            new Dictionary<OPCUAEntityReference, ExpressionValueConverter>();

        Dictionary<OPCUAEntityReference, int> mapReferenceIds =
            new Dictionary<OPCUAEntityReference, int>();
        Dictionary<string, object> mapDynamics = new Dictionary<string, object>();
        Dictionary<String, String> mapCurrentParameteItems;

        Dictionary<OPCUAEntityReference, PushedData> pendingUpdates = new Dictionary<OPCUAEntityReference, PushedData>();
        List<OPCUAEntityReference> mappedExpressionReferences = new List<OPCUAEntityReference>();
        List<OPCUAEntityReference> inuseVariableValueReferencesExpression = new List<OPCUAEntityReference>();
        public ScreenDataInfo screenDataInfo;

        OPCUAEntityReference resetStatisticsMethod;
        CancellationTokenSource cancellationTokenSource;

        Object lockObjectScreenData = new Object();

        IDocument parentDocument;
        ScreenSettings.ScreenDocument screenDocument;
        String SessionName;
        Session.Session session;
        bool bLocalExpressionSubscribed;
        bool bGlobalExpressionSubscribed;
        bool bLoaded;

        readonly IClientProxy caller;
        readonly String screenPath;
        readonly String screenId;
        public String parameterPath;
        internal Dictionary<string, ReferenceData> parametersMap = new Dictionary<string, ReferenceData>();
        readonly String clientId;
        readonly String callerScreenId;

        internal class ReferenceData
        {
            public string RelativePath { get; }
            public string ResolvedNodeId { get; set; }
            public ReferenceData(string relativePath)
            {
                RelativePath = relativePath;
            }
        }

        public ScreenData(String screenid, String name, String parameter, IClientProxy c, IDocument parent, string cId, string cScreenId)
        {
            caller = c;
            screenPath = name;
            screenId = screenid;
            parameterPath = parameter;
            parentDocument = parent;
            clientId = cId;
            session = Session.Session.GetSession(clientId);
            callerScreenId = cScreenId;

            LoadScreenData();

            PromoteIdleExecution(1);
        }

        public OPCUAEntityReference GetReference(int idreference)
        {
            OPCUAEntityReference reference = null;
            lock (lockObjectScreenData)
            {
                if (mapIdReferences.ContainsKey(idreference))
                    reference = mapIdReferences[idreference];
            }
            return reference;
        }

        public void InvalidateReferenceAttributes()
        {
            lock (lockObjectScreenData)
            {
                listReferences.ForEach(n =>
                {
                    var nodeIdViewModel = n?.NodeIdViewModel;
                    if (nodeIdViewModel != null && !nodeIdViewModel.IsEventNotifier)
                        nodeIdViewModel.InvalidateAttributes();
                });
            }
        }

        private void ExpressionBucket_OnParserError(object sender, ExpressionEvent e) {
            caller.SendAsync("ExpressionParserError", e.formula, e.error);
        }

        private void ExpressionBucket_OnParsedFormula(object sender, ExpressionEvent e)
        {
            var listVariables = e.converter.GetAllParsedVariables();
            if (listVariables.Count > 0)
            {
                lock (lockObjectScreenData)
                {
                    listVariables.ForEach(variable =>
                    {
                        var erString = PlatformComponents.UFUAEditorComponent.GetTagEntityReference(screenDocument, variable, null, inExecution: true);
                        OPCUAEntityReference reference = null;
                        if (!String.IsNullOrEmpty(erString))
                            reference = erString.FromXml<OPCUAEntityReference>();

                        if (reference == null)
                        {
                            e.error = String.Format(Properties.Resources.ExpressionTagMissing, variable);
                            return;
                        }

                        if (reference != null && !inuseVariableValueReferencesExpression.Contains(reference))
                        {
                            var observer = new PropertyObserver<OPCUAEntityReference>(reference)
                                .RegisterHandler(n => n.MonitoredItemViewModel, n =>
                                {
                                    if (n.MonitoredItemViewModel == null)
                                        return;

                                    try
                                    {
                                        if (n.MonitoredItemViewModel.NodeIdModel != null &&
                                                !n.MonitoredItemViewModel.NodeIdModel.IsEventNotifier)
                                        {
                                            var datavalue = n.MonitoredItemViewModel.DataValue;
                                            if (datavalue?.Value != null)
                                            {
                                                UpdateMapDynamics(reference);
                                                AddPendingValue(e.reference, n.MonitoredItemViewModel.DataValue);
                                            }
                                        }
                                    }
                                    catch { }

                                    var observerMonitoredModel = new PropertyObserver<MonitoredItemViewModel>(n.MonitoredItemViewModel);
                                    lock (lockObjectScreenData)
                                    {
                                        if (mapObserverMonitoredModels.ContainsKey(reference))
                                        {
                                            mapObserverMonitoredModels[reference].Dispose();
                                            mapObserverMonitoredModels.Remove(reference);
                                        }
                                        mapObserverMonitoredModels.Add(reference, observerMonitoredModel);
                                    }

                                    observerMonitoredModel.RegisterHandler(m => m.DataValue, m =>
                                    {
                                        if (m.DataValue != null)
                                        {
                                            UpdateMapDynamics(reference);
                                            AddPendingValue(e.reference, m.DataValue);
                                        }
                                    });
                                });

                            observer.RegisterHandler(n => reference.NodeIdViewModel, n =>
                            {
                                var nodeIdViewModel = reference.NodeIdViewModel;
                                if (nodeIdViewModel != null && !nodeIdViewModel.IsEventNotifier)
                                {
                                    try
                                    {
                                        //browseName = nodeIdViewModel.BrowseName.Name;
                                        //completePath = nodeIdViewModel.CompletePath;
                                        //relativePath = nodeIdViewModel.RelativePath;
                                        if (n.MonitoredItemViewModel.NodeIdModel != null && !n.MonitoredItemViewModel.NodeIdModel.IsEventNotifier)
                                        {
                                            var value = String.Empty;
                                            if (n.MonitoredItemViewModel.DataValue.Value != null)
                                            {
                                                UpdateMapDynamics(reference);
                                                AddPendingValue(e.reference, n.MonitoredItemViewModel.DataValue);
                                            }
                                        }
                                    }
                                    catch { };
                                }
                            });

                            listObserverReferences.Add(observer);

                            reference.Resolve(screenDocument.SessionString, screenDocument);
                            reference.SetInUse(this, true);
                            inuseVariableValueReferencesExpression.Add(reference);
                        }
                    });
                }
            }
        }

        private void UpdateMapDynamics(OPCUAEntityReference reference)
        {
            if (IsGoodDataValue(reference.MonitoredItemViewModel.DataValue))
            {
                var value = reference.MonitoredItemViewModel.InvariantCultureValue;
                lock (mapDynamics)
                {
                    if (reference.NodeIdViewModel != null)
                    {
                        mapDynamics[reference.NodeIdViewModel.BrowseName.Name] = value;
                        mapDynamics[reference.NodeIdViewModel.CompletePath] = value;
                    }
                    else if (!String.IsNullOrEmpty(reference.RelativePath))
                        mapDynamics[reference.RelativePath] = value;
                    mapDynamics[reference.HumanReadableNoProject] = value;
                }
            }
        }

        private void ExpressionBucket_ExpressionEvaluated(object sender, ExpressionEvent e)
        {
            if (e.convertBack)
            {
                if (e.value is System.ComponentModel.DataAnnotations.ValidationResult)
                {
                    var result = (e.value as System.ComponentModel.DataAnnotations.ValidationResult);
                    caller.SendAsync("WriteValueError", screenId, e.reference.HumanReadable, String.Empty, result.ErrorMessage);
                    return;
                }

                try
                {
                    int indexArray = -1;
                    int indexBit = -1;
                    if (!String.IsNullOrEmpty(e.converter.Formula))
                    {
                        int arrayIndex;
                        int bitNumber;
                        var type = ExpressionValueConverter.GetFormulaType(e.converter.Formula, out arrayIndex, out bitNumber);
                        if (type == ExpressionType.Bit)
                            indexBit = bitNumber;
                        else if (type == ExpressionType.Array || type == ExpressionType.ArrayPlusBit)
                        {
                            indexArray = arrayIndex;
                            indexBit = bitNumber;
                        }
                    }
                    e.reference.MonitoredItemViewModel.WriteValue(e.value, indexArray, indexBit);
                }
                catch(Exception ex)
                {
                    caller.SendAsync("WriteValueError", screenId, e.reference.HumanReadable, e.value, ex.Message);
                }
            }
            else
            {
                AddPendingValue(e.reference, e.value as DataValue, true, e.originalType);
            }
        }

        ExpressionBucket GetReferenceBucket(OPCUAEntityReference reference)
        {
            return reference.IsLocalVariable ? session.LocalExpressionBucket : Session.Session.GlobalExpressionBucket;
        }

        DateTime lastTimeAlive;
        static readonly TimeSpan aliveTimeout = TimeSpan.FromSeconds(PlatformComponents.ScreenAliveTimeoutSecs);
        public void KeepAlive()
        {
            lastTimeAlive = DateTime.UtcNow;
        }

        public bool IsAlive()
        {
            if (lastTimeAlive == null)
                return false;

            return (lastTimeAlive - DateTime.UtcNow < aliveTimeout);
        }

        public bool RequireUserLogin()
        {
            if (screenDocument == null)
                return false;
            return screenDocument.RequireUserLogin;
        }

        public String GetFullPath()
        {
            if (screenDocument == null)
                return null;
            return screenDocument.FullPath;
        }

        public long DelayUnloadSecs()
        {
            if (screenDocument == null)
                return -1;
            return screenDocument.DelayUnloadSecs;
        }

        public bool KeepAlwaysInMemory()
        {
            if (screenDocument == null)
                return false;
            return screenDocument.KeepAlwaysInMemory;
        }

        public bool HideLayoutScreens()
        {
            if (screenDocument == null)
                return false;
            return screenDocument.HideLayoutScreens;
        }

        public void SetValue(int idreference, String value)
        {
            OPCUAEntityReference reference = null;

            lock (lockObjectScreenData)
            {
                if (mapIdReferences.ContainsKey(idreference))
                    reference = mapIdReferences[idreference];
                if (reference == null)
                    throw new ArgumentException(Properties.Resources.CannotFindVariableToSetValue);
                if (reference.MonitoredItemViewModel == null)
                    throw new ArgumentException(Properties.Resources.VariableToSetValueNotConnected);

                if (mapReferenceConverters.ContainsKey(reference))
                {
                    GetReferenceBucket(reference).CalculateExpression(reference, mapReferenceConverters[reference], value, mapDynamics, true);
                    return;
                }
            }

            try
            { 
                reference.MonitoredItemViewModel.WriteValue(value);
            }
            catch (Exception ex)
            {
                caller.SendAsync("WriteValueError", reference.HumanReadable, value, ex.Message);
            }
        }

        public IDictionary<int, String> GetReferenceNameList()
        {
            var ret = new Dictionary<int, String>();
            if (!bLoaded)
                return ret;

            var mapExpressions = new Dictionary<int?, String>();

            lock(lockObjectScreenData)
            {
                foreach (var values in mapReferenceConverters)
                {
                    if(mapReferenceIds.ContainsKey(values.Key))
                    {
                        var error = values.Value.GetParserError();
                        if (!String.IsNullOrEmpty(error))
                            mapExpressions.Add(mapReferenceIds[values.Key], String.Format(" ({0} ({1}))", values.Value.Formula, error));
                        else
                            mapExpressions.Add(mapReferenceIds[values.Key], String.Format(" ({0})", values.Value.Formula));
                    }
                }

                foreach (var values in mapIdReferences)
                {
                    if (mapExpressions.ContainsKey(values.Key))
                        ret.Add(values.Key, values.Value.HumanReadable + mapExpressions[values.Key]);
                    else
                        ret.Add(values.Key, values.Value.HumanReadable);
                }
            }

            return ret;
        }

        public void ResetStatistics(int idreference)
        {
            OPCUAEntityReference reference = null;
            lock(lockObjectScreenData)
            {
                if (mapIdReferences.ContainsKey(idreference))
                    reference = mapIdReferences[idreference];
            }

            if (reference == null)
                throw new ArgumentException(Properties.Resources.CannotFindVariableToSetValue);
            if (reference.MonitoredItemViewModel == null)
                throw new ArgumentException(Properties.Resources.VariableToSetValueNotConnected);
            if (resetStatisticsMethod == null || resetStatisticsMethod.NodeIdViewModel == null)
                throw new ArgumentException(Properties.Resources.ResetStatisticServerMethodNotAvailable);

            var inputParameters = new VariantCollection();
            inputParameters.Add(new Variant(reference.MonitoredItemViewModel.monitoredItem.ResolvedNodeId));
            resetStatisticsMethod.NodeIdViewModel.CallMethod(inputParameters.ToArray());
        }

        public List<ConditionData> GetConditionStateList(int idreference, CancellationToken ct, bool bSortByTimeDescending = false, bool bNeedsRefresh = false)
        {
            OPCUAEntityReference reference = null;
            lock(lockObjectScreenData)
            {
                if (mapIdReferences.ContainsKey(idreference))
                    reference = mapIdReferences[idreference];
            }

            if (reference == null)
                throw new ArgumentException(Properties.Resources.CannotFindVariableToSetValue);
            if (reference.MonitoredItemViewModel == null)
                throw new ArgumentException(Properties.Resources.VariableToSetValueNotConnected);

            var ret = new List<ConditionData>();
            if (ct.IsCancellationRequested)
                return ret;

            if (bNeedsRefresh && reference.MonitoredItemViewModel != null && reference.MonitoredItemViewModel.ConditionStateList != null)
                reference.MonitoredItemViewModel.ConditionRefreshCommand.Execute(null);

            var list = reference.MonitoredItemViewModel.GetCurrentConditionStateList(bSortByTimeDescending);
            foreach (var item in list)
            {
                if (ct.IsCancellationRequested)
                    return ret;
                ret.Add(new ConditionData(item));
            }

            return ret;
        }

        public void AckReset(int idreference, string[] list, bool isReset)
        {
            OPCUAEntityReference reference = null;
            lock(lockObjectScreenData)
            {
                if (mapIdReferences.ContainsKey(idreference))
                    reference = mapIdReferences[idreference];
            }

            if (reference == null)
                throw new ArgumentException(Properties.Resources.CannotFindVariableToSetValue);
            if (reference.MonitoredItemViewModel == null)
                throw new ArgumentException(Properties.Resources.VariableToSetValueNotConnected);

            var ret = new List<ConditionData>();

            var listCondition = reference.MonitoredItemViewModel.GetCurrentConditionStateList();
            var listNodes = new List<String>(list);
            (from c in listCondition.AsParallel() where listNodes.Contains(c.NodeIdString) select c).
                ToList().ForEach(condition =>
                {
                    if (isReset)
                    {
                        if (condition.IsEnableCallConfirm)
                            condition.CallConfirm(true);
                    }
                    else
                    {
                        if (condition.IsEnableCallAcknowledge)
                            condition.CallAcknowledge(true);
                    }
                });
        }

        protected override void IdleExecution()
        {
            if (bDisposed)
                return;

            base.IdleExecution();

            var tempData = new List<PushedData>();
            lock (pendingUpdates)
            {
                foreach (var pair in pendingUpdates)
                    tempData.Add(pair.Value);
                pendingUpdates.Clear();
            }

            if (tempData.Count > 0)
                caller.SendAsync("ReferenceUpdate", screenId, tempData);
        }

        void LoadScreenData()
        {
            lock (lockObjectScreenData)
            {
                if (bLoaded || bDisposed)
                    return;

                if (screenDocument == null)
                    screenDocument = ScreenSettings.ScreenDocument.FromFile(screenPath, parentDocument);
                if (screenDocument == null)
                {
                    string screenBaseName = String.Empty;
                    try
                    {
                        screenBaseName = System.IO.Path.GetFileName(screenPath);
                    }
                    catch { }
                    caller.SendAsync("ErrorLoadingScreen", screenId, screenBaseName);
                }
                else
                {
                    screenDocument.Parent = parentDocument;

                    screenDataInfo = new ScreenDataInfo() {
                        WindowState = screenDocument.WindowState,
                        WindowStyle = screenDocument.WindowStyle,
                        FitInWindow = screenDocument.FitInWindow,
                        ScreenTitle = screenDocument.Title
                    };

                    listReferences.Clear();
                    var list = SVGHelper.DocSVGHelper.ImportSVGTagUsed(screenDocument);

                    foreach (var value in list)
                    {
                        listReferences.Add(value.Tag);
                        if (mapIdReferences.ContainsKey(value.SVGReferenceId))
                            mapIdReferences.Remove(value.SVGReferenceId);
                        mapIdReferences.Add(value.SVGReferenceId, value.Tag);

                        if (mapReferenceIds.ContainsKey(value.Tag))
                            mapReferenceIds[value.Tag] = value.SVGReferenceId;
                        mapReferenceIds.Add(value.Tag, value.SVGReferenceId);

                        if (!String.IsNullOrEmpty(value.Expression))
                        {
                            if (mapReferenceExpressions.ContainsKey(value.Tag))
                                mapReferenceExpressions.Remove(value.Tag);
                            mapReferenceExpressions.Add(value.Tag, new Tuple<string, string>(value.Expression, value.ReverseExpression));
                        }
                    }

                    if (!String.IsNullOrEmpty(parameterPath))
                    {
                        var resolvedParameterPath = parameterPath == "*" ? screenDocument.GetFullParameterFilePath(session.GetParameterPath(callerScreenId)) : screenDocument.GetFullParameterFilePath(parameterPath);
                        mapCurrentParameteItems = screenDocument.LoadParameterFile(resolvedParameterPath);
                        if (mapCurrentParameteItems != null)
                        {
                            foreach (var k in mapCurrentParameteItems.Keys)
                                parametersMap.Add(k, null); //k = alias' relativepath
                            foreach(var reference in listReferences) {
                                var oldRelPath = reference.RelativePath;
                                var bRet = reference.UpdatePathFromMap(mapCurrentParameteItems);
                                if (bRet)
                                    parametersMap[oldRelPath] = new ReferenceData(reference.RelativePath);
                            }
                        }
                    }

                    SessionName = screenDocument.SessionString;
                    screenDocument.UpdateSessionSettings();

                    resetStatisticsMethod = PlatformComponents.GetResetStatisticsNodeId().FromXml<OPCUAEntityReference>();
                    resetStatisticsMethod.Resolve(SessionName, screenDocument);
                    resetStatisticsMethod.SetInUse(this, true);

                    //#if DEBUG
                    //                    System.IO.File.AppendAllText("d:\\webhmilog.txt", String.Format("Loaded Screen {0}\n", screenPath));
                    //#endif
                    SubscribeReferences();
                }

                bLoaded = true;
            }
        }

        internal string GetSessionSettings()
        {
            return SessionName;
        }

        internal CancellationToken GetCancellationToken()
        {
            lock (lockObjectScreenData)
            {
                if (cancellationTokenSource == null)
                    cancellationTokenSource = new CancellationTokenSource();
                return cancellationTokenSource.Token;
            }
        }

        internal void CancelPendingExecution()
        {
            lock (lockObjectScreenData)
            {
                if (cancellationTokenSource != null)
                {
                    cancellationTokenSource.Cancel();
                    cancellationTokenSource.Dispose();
                    cancellationTokenSource = null;
                }
            }
        }

        bool IsGoodDataValue(DataValue value)
        {
            return value != null && (StatusCode.IsGood(value.StatusCode) || value.StatusCode == Opc.Ua.StatusCodes.UncertainLastUsableValue);
        }

        void AddPendingValue(OPCUAEntityReference reference, DataValue dataValue, bool isAddingFromExpression = false, BuiltInType? originalType = null)
        {
            var value = dataValue;
            if (!isAddingFromExpression)
            {
                if (value != null)
                {
                    if (IsGoodDataValue(value))
                    {
                        lock (lockObjectScreenData)
                        {
                            if (mapReferenceConverters.ContainsKey(reference) && IsGoodDataValue(reference.MonitoredItemViewModel?.DataValue))
                            {
                                value = new DataValue(reference.MonitoredItemViewModel.DataValue);
                                GetReferenceBucket(reference).CalculateExpression(reference, mapReferenceConverters[reference], value, mapDynamics);
                                return;
                            }
                        }
                    }
                }
            }

            bool isUserReadable = true;
            bool isReadable = true;
            bool isUserWritable = true;
            bool isWritable = true;
            Opc.Ua.Range range = null;
            EUInformation euinformation = null;
            List<string> enumStrings = null;

            var nodeIdModel = reference.MonitoredItemViewModel?.NodeIdModel;
            string digitalTrueValue = null;
            string digitalFalseValue = null;
            if (nodeIdModel != null)
            {
                if (nodeIdModel.EnumStrings != null)
                    enumStrings = nodeIdModel.EnumStrings.Select(x => x.Text).ToList();
                if (nodeIdModel.TrueState != null && nodeIdModel.FalseState != null) {
                    digitalTrueValue = nodeIdModel.TrueState.Text;
                    digitalFalseValue = nodeIdModel.FalseState.Text;
                }
            }

            var nodeidViewmodel = reference.NodeIdViewModel;
            if (nodeidViewmodel != null)
            {
                isUserReadable = nodeidViewmodel.IsUserReadable;
                isReadable = nodeidViewmodel.IsReadable;
                isUserWritable = nodeidViewmodel.IsUserWritable;
                isWritable = nodeidViewmodel.IsWritable;
                range = nodeidViewmodel.Range;
                euinformation = nodeidViewmodel.EUInformation;
            }

            lock (pendingUpdates)
            {
                if (pendingUpdates.ContainsKey(reference))
                    pendingUpdates.Remove(reference);
                if(mapReferenceIds.ContainsKey(reference))
                    pendingUpdates.Add(reference, new PushedData(value, mapReferenceIds[reference], originalType)
                    {
                        isUserReadable = isUserReadable,
                        isReadable = isReadable,
                        isUserWritable = isUserWritable,
                        isWritable = isWritable,
                        rangeLow = range != null ? range.Low : Double.NaN,
                        rangeHigh = range != null ? range.High : Double.NaN,
                        displayName = euinformation != null ? euinformation.DisplayName.ToString() : null,
                        description = euinformation != null ? euinformation.Description.ToString() : null,
                        enumStrings = enumStrings,
                        digitalTrueValue = digitalTrueValue,
                        digitalFalseValue = digitalFalseValue
                    });
            }

            PromoteIdleExecution(PlatformComponents.CacheDelay);
        }

        void SubscribeReferences()
        {
            lock (lockObjectScreenData)
            {
                listReferences.ForEach(reference =>
                {
                    String browseName = String.Empty;
                    String completePath = String.Empty;
                    String relativePath = String.Empty;

                    var observer = new PropertyObserver<OPCUAEntityReference>(reference)
                        .RegisterHandler(n => n.MonitoredItemViewModel, n =>
                        {
                            if (n.MonitoredItemViewModel != null)
                            {
                                var refData = (from rd in parametersMap where rd.Value != null && rd.Value.RelativePath == n.RelativePath select rd.Value).FirstOrDefault();
                                if (refData != null)
                                    refData.ResolvedNodeId = n.MonitoredItemViewModel.monitoredItem?.ResolvedNodeId?.ToString();
                                var observerMonitoredModel = new PropertyObserver<MonitoredItemViewModel>(n.MonitoredItemViewModel);
                                lock (lockObjectScreenData)
                                {
                                    if (mapObserverMonitoredModels.ContainsKey(reference))
                                    { 
                                        mapObserverMonitoredModels[reference].Dispose();
                                        mapObserverMonitoredModels.Remove(reference);
                                    }
                                    mapObserverMonitoredModels.Add(reference, observerMonitoredModel);
                                }
                                RegisterExpressions(reference);

                                try
                                {
                                    if (n.MonitoredItemViewModel.NodeIdModel != null &&
                                        !n.MonitoredItemViewModel.NodeIdModel.IsEventNotifier)
                                    {
                                        var value = String.Empty;
                                        if (n.MonitoredItemViewModel.DataValue.Value != null)
                                        {
                                            AddPendingValue(reference, n.MonitoredItemViewModel.DataValue);
                                            value = n.MonitoredItemViewModel.DataValue.Value.ToString();
                                        }
                                        lock (mapDynamics)
                                        {
                                            if (n.NodeIdViewModel != null)
                                            {
                                                mapDynamics[browseName] = value;
                                                mapDynamics[completePath] = value;
                                            }
                                            else if (!String.IsNullOrEmpty(relativePath))
                                                mapDynamics[relativePath] = value;
                                            mapDynamics[reference.HumanReadableNoProject] = value;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {

                                }

                                observerMonitoredModel.RegisterHandler(m => m.DataValue, m =>
                                {
                                    if (m.DataValue != null)
                                    {
                                        AddPendingValue(reference, m.DataValue);

                                        if (IsGoodDataValue(m.DataValue))
                                        {
                                            if (m.DataValue.Value != null)
                                            {
                                                try
                                                {
                                                    if (n.MonitoredItemViewModel.NodeIdModel != null &&
                                                        !n.MonitoredItemViewModel.NodeIdModel.IsEventNotifier)
                                                    {
                                                        var datavalue = m.DataValue.Value.ToString();
                                                        lock (mapDynamics)
                                                        {
                                                            if (n.NodeIdViewModel != null)
                                                            {
                                                                mapDynamics[browseName] = datavalue;
                                                                mapDynamics[completePath] = datavalue;
                                                            }
                                                            else if (!String.IsNullOrEmpty(relativePath))
                                                                mapDynamics[relativePath] = datavalue;
                                                            mapDynamics[reference.HumanReadableNoProject] = datavalue;
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {

                                                }
                                            }
                                        }
                                    }
                                });

                                if (/*(n.MonitoredItemViewModel.NodeIdModel == null ||
                                n.MonitoredItemViewModel.NodeIdModel.IsVariable) &&*/
                                    (n.MonitoredItemViewModel.NodeIdModel != null || n.MonitoredItemViewModel.monitoredItem == null) &&
                                    n.MonitoredItemViewModel.DataValue != null &&
                                    (Opc.Ua.StatusCode.IsGood(n.MonitoredItemViewModel.DataValue.StatusCode) ||
                                    n.MonitoredItemViewModel.DataValue != null &&
                                    n.MonitoredItemViewModel.DataValue.StatusCode == Opc.Ua.StatusCodes.UncertainLastUsableValue))
                                    AddPendingValue(reference, n.MonitoredItemViewModel.DataValue);
                            }
                        });

                    observer.RegisterHandler(n => n.NodeIdViewModel, n =>
                    {
                        var nodeIdViewModel = n.NodeIdViewModel;
                        if (nodeIdViewModel != null && !nodeIdViewModel.IsEventNotifier)
                        {
                            try
                            {
                                browseName = nodeIdViewModel.BrowseName.Name;
                                completePath = nodeIdViewModel.CompletePath;
                                relativePath = nodeIdViewModel.RelativePath;
                                if (n.MonitoredItemViewModel.NodeIdModel != null && !n.MonitoredItemViewModel.NodeIdModel.IsEventNotifier)
                                {
                                    var value = String.Empty;
                                    if (n.MonitoredItemViewModel.DataValue.Value != null)
                                    {
                                        UpdateMapDynamics(reference);
                                        AddPendingValue(reference, n.MonitoredItemViewModel.DataValue);
                                    }
                                }
                            }
                            catch { };
                        }
                    });

                    listObserverReferences.Add(observer);

                    reference.Resolve(SessionName, screenDocument);
                    reference.SetInUse(this, true);

                    if (reference.MonitoredItemViewModel != null && reference.MonitoredItemViewModel.DataValue != null)
                        AddPendingValue(reference, reference.MonitoredItemViewModel.DataValue);
                    else
                        AddPendingValue(reference, new DataValue(StatusCodes.BadWaitingForInitialData));
                });
            }
        }

        void RegisterExpressions(OPCUAEntityReference n)
        {
            if (mapReferenceExpressions.ContainsKey(n) && !mappedExpressionReferences.Contains(n) && !bDisposed)
            {
                mappedExpressionReferences.Add(n);
                var converter = new ExpressionValueConverter()
                {
                    Formula = mapReferenceExpressions[n].Item1,
                    ReverseFormula = mapReferenceExpressions[n].Item2,
                    MapCurrentParameteItems = mapCurrentParameteItems
                };

                if (n.IsLocalVariable)
                {
                    session.LocalExpressionBucket.AddExpression(n, converter);
                    if (!bLocalExpressionSubscribed)
                    {
                        bLocalExpressionSubscribed = true;
                        session.LocalExpressionBucket.ExpressionEvaluated += ExpressionBucket_ExpressionEvaluated;
                        session.LocalExpressionBucket.ParsedFormula += ExpressionBucket_OnParsedFormula;
                        session.LocalExpressionBucket.ParserErrorEvent += ExpressionBucket_OnParserError;
                    }
                }
                else
                {
                    Session.Session.GlobalExpressionBucket.AddExpression(n, converter);
                    if (!bGlobalExpressionSubscribed)
                    {
                        bGlobalExpressionSubscribed = true;
                        Session.Session.GlobalExpressionBucket.ExpressionEvaluated += ExpressionBucket_ExpressionEvaluated;
                        Session.Session.GlobalExpressionBucket.ParsedFormula += ExpressionBucket_OnParsedFormula;
                        Session.Session.GlobalExpressionBucket.ParserErrorEvent += ExpressionBucket_OnParserError;
                    }
                }

                lock (lockObjectScreenData)
                {
                    if (mapReferenceConverters.ContainsKey(n))
                        mapReferenceConverters.Remove(n);
                    mapReferenceConverters.Add(n, converter);
                }
            }
        }

        void UnsubscribeReferences()
        {
            listObserverReferences.ForEach(observer => observer.Dispose());
            listObserverReferences.Clear();
            mapObserverMonitoredModels.Values.ToList().ForEach(ob => ob.Dispose());
            mapObserverMonitoredModels.Clear();

            foreach (var pair in  mapReferenceConverters)
            {
                GetReferenceBucket(pair.Key).RemoveExpression(pair.Key, pair.Value);
            }
            mapReferenceConverters.Clear();

            if (bLocalExpressionSubscribed)
            {
                session.LocalExpressionBucket.ExpressionEvaluated -= ExpressionBucket_ExpressionEvaluated;
                session.LocalExpressionBucket.ParsedFormula -= ExpressionBucket_OnParsedFormula;
                session.LocalExpressionBucket.ParserErrorEvent -= ExpressionBucket_OnParserError;
            }
            if (bGlobalExpressionSubscribed)
            {
                Session.Session.GlobalExpressionBucket.ExpressionEvaluated -= ExpressionBucket_ExpressionEvaluated;
                Session.Session.GlobalExpressionBucket.ParsedFormula -= ExpressionBucket_OnParsedFormula;
                Session.Session.GlobalExpressionBucket.ParserErrorEvent -= ExpressionBucket_OnParserError;
            }

            mapReferenceExpressions.Clear();
            mapIdReferences.Clear();
            mappedExpressionReferences.Clear();

            // mapDynamics.Clear();
        }

        #region IEntityReference Members

        public ImageSource CollapsedImageSource
        {
            get
            {
                return null;
            }
        }

        public ImageSource ExpandedImageSource
        {
            get
            {
                return null;
            }
        }

        public ContextMenu contextMenu
        {
            get
            {
                return null;
            }
        }

        public object Tooltip
        {
            get
            {
                return null;
            }
        }

        public object ContainedObject
        {
            get
            {
                return null;
            }
        }

        public object EntityParent
        {
            get
            {
                return null;
            }
        }

        public string TypeDefinitionString
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region Dispose
        bool bDisposed;
        protected override void OnDispose()
        {
            base.OnDispose();

            List<OPCUAEntityReference> list = null;
            lock (lockObjectScreenData)
            {
                bDisposed = true;
//#if DEBUG
//                System.IO.File.AppendAllText("d:\\webhmilog.txt", String.Format("Unloaded Screen {0}\n", screenPath));
//#endif
                if (bLoaded)
                {
                    list = new List<OPCUAEntityReference>(listReferences.Concat(inuseVariableValueReferencesExpression));
                    listReferences.Clear();
                    inuseVariableValueReferencesExpression.Clear();
                }

                CancelPendingExecution();
                UnsubscribeReferences();
            }

            if (list != null)
            {
                list.ForEach(reference =>
                {
                    reference.SetInUse(this, false);
                });
            }

            resetStatisticsMethod?.SetInUse(this, false);
            screenDocument?.Dispose();
        }
        #endregion
    }
}
