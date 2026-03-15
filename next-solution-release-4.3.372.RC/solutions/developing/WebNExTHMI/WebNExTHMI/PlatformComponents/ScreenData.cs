using DocumentManager.ComponentService;
using Microsoft.AspNetCore.SignalR;
using Opc.Ua;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using UFInterfaces;
using ViewModelLib;
using Utilities;
using System.Threading;
using ExpressionManager;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using System.ComponentModel;
using Utilities.Converters;
using Utilities.Enums;

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

        Dictionary<OPCUAEntityReference, ExpressionEntity> mapReferenceConverters =
            new Dictionary<OPCUAEntityReference, ExpressionEntity>();

        Dictionary<ExpressionEntity, PropertyObserver<MonitoredItemViewModel>> mapExpressionObservers =
            new Dictionary<ExpressionEntity, PropertyObserver<MonitoredItemViewModel>>();

        Dictionary<OPCUAEntityReference, int> mapReferenceIds =
            new Dictionary<OPCUAEntityReference, int>();
        Dictionary<String, String> mapCurrentParameteItems;

        Dictionary<OPCUAEntityReference, PushedData> pendingUpdates = new Dictionary<OPCUAEntityReference, PushedData>();
        public ScreenDataInfo screenDataInfo;

        OPCUAEntityReference resetStatisticsMethod;
        CancellationTokenSource cancellationTokenSource;
        Dictionary<Tuple<string, string>, string> lastExpressionEntityError = new Dictionary<Tuple<string, string>, string>();

        Dictionary<string, OPCUAEntityReference> childrenAlarmServerVariables = new Dictionary<string, OPCUAEntityReference>();

        Object lockObjectScreenData = new Object();
        static object lockAlarmserverSubscriptions = new Object();

        static readonly PropertyDescriptorCollection conditionStateViewModelDescriptor = TypeDescriptor.GetProperties(typeof(ConditionStateViewModel));

        IDocument parentDocument;
        ScreenSettings.ScreenDocument screenDocument;
        String SessionName;
        Session.Session session;
        bool bLoaded;
        bool inIdlePending;
        bool inIdleStatus;

        readonly IClientProxy caller;
        readonly String screenPath;
        readonly String screenId;
        public String parameterPath;
        internal Dictionary<string, ReferenceData> parametersMap = new Dictionary<string, ReferenceData>();
        OPCUAEntityReference alarmServerCommandVariable;
        MonitoredItemViewModel alarmCommandsMonitoredItemViewModel;
        string lastAlarmMonitoredItemQuality;
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

        public void SubscribeCanAckResetAll()
        {
            if (alarmServerCommandVariable == null)
            {
                var serverXML = PlatformComponents.UFUAEditorComponent.GetServerEntityReference(PlatformComponents.GetProjectDocument(), bCheckEmpty: true);
                if (serverXML != null)
                {
                    alarmServerCommandVariable = serverXML.FromXml<OPCUAEntityReference>();
                    var alarmCommandsObserverReference = new PropertyObserver<OPCUAEntityReference>(alarmServerCommandVariable);
                    alarmCommandsObserverReference.RegisterHandler(n => n.MonitoredItemViewModel, n =>
                    {
                        if (bDisposed)
                            return;

                        var bCanExecute = false;
                        if (n.MonitoredItemViewModel != null)
                        {
                            if (alarmCommandsMonitoredItemViewModel != null)
                                alarmCommandsMonitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;
                            alarmCommandsMonitoredItemViewModel = n.MonitoredItemViewModel;
                            
                            alarmCommandsMonitoredItemViewModel.PropertyChanged += monitoredItemViewModel_PropertyChanged;

                            bCanExecute = IsAlarmCommandExecutable();
                        }
                        caller.SendAsync("CanAckResetCommandUpdate", screenId, bCanExecute);
                    });
                    listObserverReferences.Add(alarmCommandsObserverReference);
                    
                    alarmServerCommandVariable.Resolve(session.GetAlarmCommandsSessionName(),
                        PlatformComponents.GetProjectDocument());
                    alarmServerCommandVariable.SetInUse(this, true);
                }
            }
        }

        bool IsAlarmCommandExecutable()
        {
            var conditionStateList = alarmServerCommandVariable?.MonitoredItemViewModel?.ConditionStateList; // initialize the alarm client subscription ("_ConditionStateViewModel")
            var isGoodQuality = alarmServerCommandVariable != null && (StatusCode.IsGood(alarmServerCommandVariable.MonitoredItemViewModel.DataValue.StatusCode) || StatusCode.IsUncertain(alarmServerCommandVariable.MonitoredItemViewModel.DataValue.StatusCode));

            return conditionStateList != null && isGoodQuality;
        }

        void monitoredItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (bDisposed)
                return;

            if (e.PropertyName == "Quality" && alarmCommandsMonitoredItemViewModel.Quality != lastAlarmMonitoredItemQuality)
            {
                lastAlarmMonitoredItemQuality = alarmCommandsMonitoredItemViewModel.Quality;
                caller.SendAsync("CanAckResetCommandUpdate", screenId, IsAlarmCommandExecutable());
            }
        }

        public void CommandAckResetAll(bool isReset)
        {
            session.AckReset(alarmServerCommandVariable, isReset);
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

        private void ExpressionEntity_ParserError(object sender, EventArgs e) {
            var expressionEntity = (ExpressionEntity)sender;
            var error = expressionEntity.GetParserError();
            var expressionErrorKey = new Tuple<string, string>(expressionEntity.Formula, expressionEntity.ReverseFormula);
            if (!String.IsNullOrEmpty(error) && (!lastExpressionEntityError.ContainsKey(expressionErrorKey) || lastExpressionEntityError[expressionErrorKey] != error))
            {
                lastExpressionEntityError[expressionErrorKey] = error;
                Console.Error.WriteLine(String.Format(Properties.Resources.ExpressionParserError, expressionEntity.Formula, error));
                caller.SendAsync("ExpressionParserError", expressionEntity.Formula, error);
            }
        }

        private void ExpressionEntity_ExecutionError(object sender, EventArgs e)
        {
            var expressionEntity = (ExpressionEntity)sender;
            var error = expressionEntity.GetExecutionError();
            if (error != null)
                Console.Error.WriteLine(String.Format(Properties.Resources.ExpressionExecutionError, expressionEntity.Formula, error.Message));
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

        public bool KeepAlwaysTagsAlive()
        {
            if (screenDocument == null)
                return true;
            return screenDocument.KeepAlwaysTagsAlive;
        }

        public bool HideLayoutScreens()
        {
            if (screenDocument == null)
                return false;
            return screenDocument.HideLayoutScreens;
        }

        public void SetValue(int idreference, String value, bool bForceSync)
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
                    if (bForceSync)
                        mapReferenceConverters[reference].WriteValue(value);
                    else
                        mapReferenceConverters[reference].TempVariable.WriteValue(value);
                    return;
                }
            }

            try
            {
                reference.MonitoredItemViewModel.WriteValue(value);
            }
            catch (Exception ex)
            {
                caller.SendAsync("WriteValueError", screenId, reference.HumanReadable, value, ex.Message);
                AddPendingValue(reference, reference.MonitoredItemViewModel?.DataValue);
            }
        }

        public IDictionary<int, String> GetReferenceNameList()
        {
            var ret = new Dictionary<int, String>();
            if (!bLoaded)
                return ret;

            var mapExpressions = new Dictionary<int?, String>();

            lock (lockObjectScreenData)
            {
                foreach (var values in mapReferenceConverters)
                {
                    if (mapReferenceIds.ContainsKey(values.Key))
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
            lock (lockObjectScreenData)
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

        public List<ConditionData> GetConditionStateList(int idreference, CancellationToken ct, bool bSortByTimeDescending = false, bool bNeedsRefresh = false, Dictionary<object, object> filters = null, bool bConnectChildAlarms = false)
        {
            OPCUAEntityReference mainReference = null;
            lock (lockObjectScreenData)
            {
                if (mapIdReferences.ContainsKey(idreference))
                    mainReference = mapIdReferences[idreference];
            }

            if (mainReference == null)
                throw new ArgumentException(Properties.Resources.CannotFindVariableToSetValue);

            if (mainReference.MonitoredItemViewModel == null)
                throw new ArgumentException(Properties.Resources.VariableToSetValueNotConnected);

            var ret = new List<ConditionData>();
            if (ct.IsCancellationRequested)
                return ret;

            //loop children's alarm server
            var alarmServerReferences = new List<OPCUAEntityReference>() { mainReference };
            if (bConnectChildAlarms)
                alarmServerReferences.AddRange(childrenAlarmServerVariables.Values);

            foreach (var reference in alarmServerReferences)
            {
                if (reference.MonitoredItemViewModel == null)
                    continue;

                if (session.EnableUserManager && !session.ConditionStateFilterInitialized)
                {
                    session.ConditionStateFilterInitialized = true;
                    reference.MonitoredItemViewModel.ApplyConditionStateFilter(session.CurrentUserReadAccessMask, session.CurrentUserAccessLevel);
                }

                if (bNeedsRefresh && reference.MonitoredItemViewModel != null && reference.MonitoredItemViewModel.ConditionStateList != null)
                    reference.MonitoredItemViewModel.ConditionRefreshCommand.Execute(null);

                var list = reference.MonitoredItemViewModel.GetCurrentConditionStateList(bSortByTimeDescending);

                bool bMaskOn = true;
                bool bMaskOnAck = true;
                bool bMaskOffAck = true;
                bool bMaskOff = true;
                ushort severityFilter = 0;
                int severityFilterConditions = 0;
                string alarmDescriptionFilter = null;
                if (filters != null)
                {
                    try
                    {
                        var alarmMaskFilter = int.Parse(filters["alarmMaskFilter"].ToString());
                        severityFilter = ushort.Parse(filters["severityFilter"].ToString());
                        severityFilterConditions = int.Parse(filters["severityFilterConditions"].ToString());
                        alarmDescriptionFilter = filters["alarmDescriptionFilter"].ToString();//.Trim();
                        var converter = new AlarmMaskConverter();
                        bMaskOn = ((bool)converter.Convert(alarmMaskFilter, null, AlarmMask.On, null));
                        bMaskOnAck = ((bool)converter.Convert(alarmMaskFilter, null, AlarmMask.OnAck, null));
                        bMaskOffAck = ((bool)converter.Convert(alarmMaskFilter, null, AlarmMask.OffAck, null));
                        bMaskOff = ((bool)converter.Convert(alarmMaskFilter, null, AlarmMask.Off, null));
                    }
                    catch
                    {
                        filters = null;
                    }
                }

                foreach (var item in list)
                {
                    if (ct.IsCancellationRequested)
                        return ret;
                    var cd = new ConditionData(item);

                    if (filters != null)
                    {
                        if (!bMaskOn && cd.enabledState.Contains(ConditionStateNames.Active) && cd.enabledState.Contains(ConditionStateNames.Unacknowledged))
                            continue;
                        if (!bMaskOnAck && cd.enabledState.Contains(ConditionStateNames.Active) && cd.enabledState.Contains(ConditionStateNames.Unconfirmed))
                            continue;
                        if (!bMaskOff && cd.enabledState.Contains(ConditionStateNames.Inactive) && cd.enabledState.Contains(ConditionStateNames.Unacknowledged))
                            continue;
                        if (!bMaskOffAck && cd.enabledState.Contains(ConditionStateNames.Inactive) && cd.enabledState.Contains(ConditionStateNames.Unconfirmed))
                            continue;
                        if (severityFilter != 0)
                        {
                            switch (severityFilterConditions)
                            {
                                case 1:
                                    if (cd.severity < severityFilter)
                                        continue;
                                    break;
                                case 2:
                                    if (cd.severity > severityFilter)
                                        continue;
                                    break;
                                case 0:
                                    if (cd.severity != severityFilter)
                                        continue;
                                    break;
                                default:
                                    if (cd.severity != severityFilter)
                                        continue;
                                    break;
                            }
                        }
                        if (!String.IsNullOrEmpty(alarmDescriptionFilter))
                        {
                            var criteria = @"[Message] Like '" + alarmDescriptionFilter + "'";
                            var criteriaOperator = CriteriaOperator.Parse(criteria);
                            var expEval = new ExpressionEvaluator(conditionStateViewModelDescriptor, criteriaOperator, false);
                            if (!(bool)expEval.Evaluate(item))
                                continue;
                        }
                    }
                    ret.Add(cd);
                }
            }

            return ret;
        }

        public void AckReset(int idreference, string[] list, bool isReset, bool bConnectChildAlarms = false)
        {
            OPCUAEntityReference mainReference = null;
            lock (lockObjectScreenData)
            {
                if (mapIdReferences.ContainsKey(idreference))
                    mainReference = mapIdReferences[idreference];
            }

            if (mainReference == null)
                throw new ArgumentException(Properties.Resources.CannotFindVariableToSetValue);

            if (mainReference.MonitoredItemViewModel == null)
                throw new ArgumentException(Properties.Resources.VariableToSetValueNotConnected);

            var ret = new List<ConditionData>();

            //loop children's alarm server
            var alarmServerReferences = new List<OPCUAEntityReference>() { mainReference };
            if (bConnectChildAlarms)
                alarmServerReferences.AddRange(childrenAlarmServerVariables.Values);

            foreach (var reference in alarmServerReferences)
            {
                if (reference.MonitoredItemViewModel == null)
                    continue;

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
        }

        protected override void IdleExecution()
        {
            if (bDisposed)
                return;

            base.IdleExecution();

            UpdateInIdleState();

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
                            foreach (var reference in listReferences) {
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

        internal void SetInIdle(bool newValue)
        {
            if (!bLoaded || bDisposed || inIdlePending == newValue || KeepAlwaysTagsAlive())
                return;
            inIdlePending = newValue;

            PromoteIdleExecution(PlatformComponents.CacheDelay);
        }

        void UpdateInIdleState()
        {
            if (inIdlePending == inIdleStatus)
                return;
            inIdleStatus = inIdlePending;

            List<OPCUAEntityReference> list = null;
            lock (lockObjectScreenData)
            {
                list = new List<OPCUAEntityReference>(listReferences);
            }

            if (list != null)
            {
                list.ForEach(reference =>
                {
                    reference.SetKeepAlive(inIdleStatus);
                    reference.SetInUse(this, !inIdleStatus);
                });
            }
        }

        void AddPendingValue(OPCUAEntityReference reference, DataValue dataValue, bool isAddingFromExpression = false, BuiltInType? originalType = null)
        {
            var value = dataValue;
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
                if (mapReferenceIds.ContainsKey(reference))
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
                            lock (lockObjectScreenData)
                            {
                                if (mapReferenceConverters.ContainsKey(n) && mapReferenceConverters[n] != null)
                                {
                                    mapReferenceConverters[n].ParserError -= ExpressionEntity_ParserError;
                                    mapReferenceConverters[n].ExecutionError -= ExpressionEntity_ExecutionError;
                                    mapReferenceConverters[n].Dispose();
                                    mapReferenceConverters[n] = null;
                                    mapReferenceConverters.Remove(n);
                                }
                            }

                            if (n.MonitoredItemViewModel != null)
                            {
                                var refData = (from rd in parametersMap where rd.Value != null && rd.Value.RelativePath == n.RelativePath select rd.Value).FirstOrDefault();
                                if (refData != null)
                                    refData.ResolvedNodeId = n.MonitoredItemViewModel.monitoredItem?.ResolvedNodeId?.ToString();
                                PropertyObserver<MonitoredItemViewModel> observerMonitoredModel;

                                MonitoredItemViewModel m = reference.MonitoredItemViewModel;
                                string expression = null;
                                string reverseExpression = null;
                                lock (lockObjectScreenData)
                                {
                                    if (screenDocument != null && !bDisposed && mapReferenceExpressions.ContainsKey(reference) && !String.IsNullOrEmpty(mapReferenceExpressions[reference].Item1))
                                    {
                                        expression = mapReferenceExpressions[reference].Item1;
                                        reverseExpression = mapReferenceExpressions[reference].Item2;
                                    }
                                }
                                if (expression != null)
                                    m = RegisterExpressions(reference, expression, reverseExpression);

                                observerMonitoredModel = new PropertyObserver<MonitoredItemViewModel>(m);
                                lock (lockObjectScreenData)
                                {
                                    if (mapObserverMonitoredModels.ContainsKey(reference))
                                        mapObserverMonitoredModels[reference].Dispose();
                                    mapObserverMonitoredModels[reference] = observerMonitoredModel;
                                }

                                try
                                {
                                    if (m.NodeIdModel != null &&
                                        !m.NodeIdModel.IsEventNotifier)
                                    {
                                        if (m.DataValue.Value != null)
                                            AddPendingValue(reference, m.DataValue);
                                    }
                                }
                                catch (Exception ex)
                                {

                                }

                                observerMonitoredModel.RegisterHandler(m => m.DataValue, m =>
                                {
                                    if (m.DataValue != null)
                                        AddPendingValue(reference, m.DataValue);
                                });

                                if (/*(m.NodeIdModel == null ||
                                m.NodeIdModel.IsVariable) &&*/
                                    (m.NodeIdModel != null || m.monitoredItem == null) &&
                                    m.DataValue != null &&
                                    (Opc.Ua.StatusCode.IsGood(m.DataValue.StatusCode) ||
                                    m.DataValue != null &&
                                    m.DataValue.StatusCode == Opc.Ua.StatusCodes.UncertainLastUsableValue))
                                    AddPendingValue(reference, m.DataValue);
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
                                var m = n.MonitoredItemViewModel;
                                lock (lockObjectScreenData)
                                {
                                    if (mapReferenceConverters.ContainsKey(n) && mapReferenceConverters[n] != null)
                                        m = mapReferenceConverters[n].TempVariable;
                                }
                                if (m.NodeIdModel != null && !m.NodeIdModel.IsEventNotifier)
                                {
                                    var value = String.Empty;
                                    if (m.DataValue.Value != null)
                                        AddPendingValue(reference, m.DataValue);
                                }
                            }
                            catch { };
                        }
                    });

                    listObserverReferences.Add(observer);

                    AddPendingValue(reference, new DataValue(StatusCodes.BadWaitingForInitialData));
                    reference.Resolve(SessionName, screenDocument);
                    reference.SetInUse(this, true);
                });
            }
        }

        MonitoredItemViewModel RegisterExpressions(OPCUAEntityReference reference, string expression, string reverseExpression)
        {
            var m = reference.MonitoredItemViewModel;
            var expressionEntity = ExpressionBucket.GetInstance(screenDocument).AddExpression(m, expression, reverseExpression, mapCurrentParameteItems);
            expressionEntity.ParserError += ExpressionEntity_ParserError;
            expressionEntity.ExecutionError += ExpressionEntity_ExecutionError;

            lock (lockObjectScreenData)
                mapReferenceConverters[reference] = expressionEntity;

            return expressionEntity.TempVariable;
        }

        public void UnsubscribeChildrenAlarmServer()
        {
            lock (childrenAlarmServerVariables)
            {
                foreach (var childServer in childrenAlarmServerVariables.Values)
                    childServer?.SetInUse(this, false);
                childrenAlarmServerVariables.Clear();
            }
        }

        public void SubscribeChildrenAlarmServer() 
        {
            var platformProject = PlatformComponents.GetProjectDocument() as UFProjectManager.UFProjectDocument;
            if (platformProject != null)
            {
                foreach (UFProjectManager.UFProjectDocument child in platformProject.Childs)
                    SubscribeChildrenAlarmServer(child);
            }
        }

        void SubscribeChildrenAlarmServer(UFProjectManager.UFProjectDocument platformProject)
        {
            var projectDocument = UFProjectManager.UFProjectDocument.FromFile(platformProject.ProjectPath, PlatformComponents.UFProjectManagerComponent);
            string serverXML;
            lock (lockAlarmserverSubscriptions)
            {
                serverXML = PlatformComponents.UFUAEditorComponent.GetServerEntityReference(projectDocument, bCheckEmpty: true);
            }
            if (serverXML != null)
            {
                var childAlarmServerVariable = serverXML.FromXml<OPCUAEntityReference>();
                childAlarmServerVariable.Resolve(PlatformComponents.GetSessionString(),
                    projectDocument);
                lock (childrenAlarmServerVariables)
                {
                    if (!childrenAlarmServerVariables.ContainsKey(platformProject.ProjectPath))
                    {
                        childrenAlarmServerVariables[platformProject.ProjectPath] = childAlarmServerVariable;
                        childAlarmServerVariable.SetInUse(this, true);
                    }
                }
            }

            foreach (UFProjectManager.UFProjectDocument child in platformProject.Childs)
                SubscribeChildrenAlarmServer(child);
        }

        void UnsubscribeReferences()
        {
            listObserverReferences.ForEach(observer => observer.Dispose());
            listObserverReferences.Clear();
            mapObserverMonitoredModels.Values.ToList().ForEach(ob => ob.Dispose());
            mapObserverMonitoredModels.Clear();

            foreach (var n in  mapReferenceConverters.Keys)
            {
                if (mapReferenceConverters[n] != null)
                {
                    mapReferenceConverters[n].ParserError -= ExpressionEntity_ParserError;
                    mapReferenceConverters[n].ExecutionError -= ExpressionEntity_ExecutionError;
                    mapReferenceConverters[n].Dispose();
                    mapReferenceConverters[n] = null;
                }
            }
            mapReferenceConverters.Clear();
            foreach (var e in mapExpressionObservers.Keys)
                mapExpressionObservers[e]?.Dispose();
            mapExpressionObservers.Clear();
            
            mapReferenceExpressions.Clear();
            mapIdReferences.Clear();
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

            if (alarmServerCommandVariable != null)
            {
                alarmServerCommandVariable?.SetInUse(this, false);
                alarmServerCommandVariable = null;
            }
            if (alarmCommandsMonitoredItemViewModel != null)
                alarmCommandsMonitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;

            List<OPCUAEntityReference> list = null;
            lock (lockObjectScreenData)
            {
                bDisposed = true;
//#if DEBUG
//                System.IO.File.AppendAllText("d:\\webhmilog.txt", String.Format("Unloaded Screen {0}\n", screenPath));
//#endif
                if (bLoaded)
                {
                    list = new List<OPCUAEntityReference>(listReferences);
                    listReferences.Clear();
                }

                CancelPendingExecution();
                UnsubscribeReferences();
                UnsubscribeChildrenAlarmServer();
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
