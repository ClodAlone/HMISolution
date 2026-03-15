using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using DocumentManager.ComponentService;
using Opc.Ua;
using OPCUAViewModel;
using UFUAEditor.ComponentService;
using Utilities;
using ViewModelLib;
using System.Windows;
using UFUAModel.Extensions;
using UFRecipeSettings.UFRecipeModel;
using log4net.Repository.Hierarchy;
using Opc.Ua.Client;
using OPCUAViewModel.Services;
using UFInterfaces;
#if !NET_STANDARD
using System.Windows.Threading;
#endif

namespace UFRecipeSettings.Helpers
{
    public class ExecutionResult
    {
        public Boolean ResultState { get; set; }
        public Variant[] OutputValues { get; set; }
        public Exception ExceptionInfo { get; set; }
    }

    public class RecipeEntityHelper : IDisposable
    {
        #region Declaration

        readonly IDocument Parent;
        readonly IUFUAEditorManager UfuaEditorService;
        readonly UFRecipeModel.UFRecipeEntity RecipeEntity;
        readonly bool bWrite;
        private readonly IOPCSessionService _opcSessionService;

        readonly Object lockObject = new Object();

        bool bExecuted;
        Dictionary<String, OPCUAEntityReference> opcuaEntityReference;
        Dictionary<String, PropertyObserver<OPCUAEntityReference>> mapObserver;
        Dictionary<Guid, PropertyObserver<MonitoredItemViewModel>> mapObserverDataValue;
        Dictionary<String, Timer> mapTimer;
        ManualResetEvent canExecute;

        List<OPCUAEntityReference> listTagIOEntityReference;
        Dictionary<Guid, OPCUAEntityReference> mapTagIOEntityReference;
        Dictionary<Guid, MonitoredItemViewModel> mapMonitoredItemViewModel;
        Dictionary<Guid, UFDataValueEntity> mapInheritedRecipeEntity;
        #endregion

        #region Constructors

        public RecipeEntityHelper(IDocument parent, IUFUAEditorManager ufuaEditorService, UFRecipeModel.UFRecipeEntity recipeEntity, bool write)
        {
            Parent = parent;
            UfuaEditorService = ufuaEditorService;
            RecipeEntity = recipeEntity;
            bWrite = write;
            _opcSessionService = new OPCSessionService();

        }

        #endregion

        #region Properties
#if !NET_STANDARD
        public UIElement Control { get; set; }
#endif
        public int DefaultTimeout { get; set; }

        #endregion

        #region Methods
        public void PrepareExecution(String sessionname)
        {
            PrepareExecution(sessionname, TimeSpan.Zero);
        }

        public event EventHandler OnComplete;

        public void PrepareExecution(String sessionname, TimeSpan timeout)
        {
            var listSyncRead = new List<MonitoredItemViewModel>();

            lock (lockObject)
            {
                if (bExecuted)
                    return;
                bExecuted = true;

                PrepareEntityReferences();

                if (canExecute == null)
                    canExecute = new ManualResetEvent(false);

                foreach (var key in opcuaEntityReference.Keys)
                {
                    if (opcuaEntityReference[key] == null
#if !NET_STANDARD
                        || !opcuaEntityReference[key].IsValid
#endif
                        )
                        return;

                    if (mapObserver == null)
                        mapObserver = new Dictionary<String, PropertyObserver<OPCUAEntityReference>>();

                    if (mapTimer == null)
                        mapTimer = new Dictionary<String, Timer>();

                    mapObserver[key] = new PropertyObserver<OPCUAEntityReference>(opcuaEntityReference[key])
                            .RegisterHandler(n => n.NodeIdViewModel, n =>
                            {
                                lock (lockObject)
                                {
                                    if (bDisposed)
                                        return;

                                    if (mapTimer.ContainsKey(key))
                                        mapTimer[key].Dispose();
                                    mapTimer[key] = new Timer(timerCallback, key, 0, Timeout.Infinite);
                                }
                            });

                    opcuaEntityReference[key].Resolve(sessionname, Parent);
                    opcuaEntityReference[key].SetInUse(RecipeEntity, true);
                }

                foreach (var key in mapTagIOEntityReference.Keys)
                {
                    if (mapTagIOEntityReference[key] == null
#if !NET_STANDARD
                        || !mapTagIOEntityReference[key].IsValid
#endif
                        )
                        return;

                    if (mapObserver == null)
                        mapObserver = new Dictionary<String, PropertyObserver<OPCUAEntityReference>>();

                    if (mapObserverDataValue == null)
                        mapObserverDataValue = new Dictionary<Guid, PropertyObserver<MonitoredItemViewModel>>();

                    if (mapMonitoredItemViewModel == null)
                        mapMonitoredItemViewModel = new Dictionary<Guid, MonitoredItemViewModel>();

                    mapObserver[key.ToString()] = new PropertyObserver<OPCUAEntityReference>(mapTagIOEntityReference[key])
                            .RegisterHandler(n => n.MonitoredItemViewModel, n =>
                            {
                                Opc.Ua.Client.Session session = null;
                                lock (lockObject)
                                {
                                    if (bDisposed)
                                        return;

                                    if (mapMonitoredItemViewModel.ContainsKey(key))
                                        mapMonitoredItemViewModel.Remove(key);
                                    if (n.MonitoredItemViewModel != null)
                                    {
                                        if (mapObserverDataValue.ContainsKey(key))
                                            mapObserverDataValue[key].Dispose();
                                        mapObserverDataValue[key] = new PropertyObserver<MonitoredItemViewModel>(n.MonitoredItemViewModel)
                                            .RegisterHandler(m => m.DataValue, m =>
                                            {
                                                lock (lockObject)
                                                {
                                                    if (IsGoodDataValue(m.DataValue))
                                                    {
                                                        if (!mapMonitoredItemViewModel.ContainsKey(key))
                                                            mapMonitoredItemViewModel.Add(key, m);

                                                        if (canExecute != null && opcuaEntityReference.Count == 0 &&
                                                            mapMonitoredItemViewModel.Count == mapTagIOEntityReference.Count)
                                                        {
                                                            canExecute.Set();
                                                        }
                                                    }
                                                    else
                                                        canExecute.Reset();
                                                }
                                            });

                                        session = n.MonitoredItemViewModel.NodeIdModel?.session;
                                        if (session == null && IsGoodDataValue(n.MonitoredItemViewModel.DataValue))
                                        {
                                            if (!mapMonitoredItemViewModel.ContainsKey(key))
                                                mapMonitoredItemViewModel.Add(key, n.MonitoredItemViewModel);

                                            if (canExecute != null && opcuaEntityReference.Count == 0 &&
                                                mapMonitoredItemViewModel.Count == mapTagIOEntityReference.Count)
                                            {
                                                canExecute.Set();
                                            }
                                        }

                                        if (mapInheritedRecipeEntity.ContainsKey(key))
                                        {
                                            try
                                            {
                                                if (n.MonitoredItemViewModel.NodeIdModel != null)
                                                {
                                                    mapInheritedRecipeEntity[key].DataType = n.MonitoredItemViewModel.NodeIdModel.DataType.ToDataType();
                                                    mapInheritedRecipeEntity.Remove(key);
                                                }
                                                else if (n.MonitoredItemViewModel.DataValue != null)
                                                {
                                                    mapInheritedRecipeEntity[key].DataType = n.MonitoredItemViewModel.DataValue.WrappedValue.TypeInfo.BuiltInType.ToDataType();
                                                    mapInheritedRecipeEntity.Remove(key);
                                                }
                                                else
                                                {
                                                    var receipe = mapInheritedRecipeEntity[key].UFRecipeAss == null ?
                                                        mapInheritedRecipeEntity[key].UFGroupAss.UFRecipeAss.Name : mapInheritedRecipeEntity[key].UFRecipeAss.Name;
                                                    Utilities.Logger.Logger.WriteToEventLog(Utilities.Properties.Resources.RecipeService,
                                                        String.Format(Utilities.Properties.Resources.InheritDataTypeError, n.MonitoredItemViewModel.DisplayName,
                                                        mapInheritedRecipeEntity[key].DataValueName, receipe), EventLogEntryType.Error);
                                                }

                                            }
                                            catch
                                            {
                                                var receipe = mapInheritedRecipeEntity[key].UFRecipeAss == null ?
                                                        mapInheritedRecipeEntity[key].UFGroupAss.UFRecipeAss.Name : mapInheritedRecipeEntity[key].UFRecipeAss.Name;
                                                Utilities.Logger.Logger.WriteToEventLog(Utilities.Properties.Resources.RecipeService,
                                                    String.Format(Utilities.Properties.Resources.InheritDataTypeError, n.MonitoredItemViewModel.DisplayName,
                                                    mapInheritedRecipeEntity[key].DataValueName, receipe), EventLogEntryType.Error);
                                            }
                                            if (mapInheritedRecipeEntity.Count == 0) //Only in this case all the tags are subscribed
                                                ThreadPool.QueueUserWorkItem(new WaitCallback((object state) => OnComplete?.Invoke(this, EventArgs.Empty)));
                                        }
                                    }
                                }

                                if (session != null)
                                {
                                    try
                                    {
                                        n.MonitoredItemViewModel.DataValue = session.ReadValue(n.MonitoredItemViewModel.monitoredItem.ResolvedNodeId);
                                    }
                                    catch
                                    { }
                                }

#if !NET_STANDARD
                                InvalidateRequerySuggestedAsynchronously();
#endif
                            });

                    if (mapTagIOEntityReference[key].MonitoredItemViewModel != null)
                    {
                        if (mapObserverDataValue.ContainsKey(key))
                            mapObserverDataValue[key].Dispose();
                        mapObserverDataValue[key] = new PropertyObserver<MonitoredItemViewModel>(mapTagIOEntityReference[key].MonitoredItemViewModel)
                        .RegisterHandler(m => m.DataValue, m =>
                        {
                            lock (lockObject)
                            {
                                if (IsGoodDataValue(m.DataValue))
                                {
                                    if (!mapMonitoredItemViewModel.ContainsKey(key))
                                        mapMonitoredItemViewModel.Add(key, m);

                                    if (canExecute != null && opcuaEntityReference.Count == 0 &&
                                        mapMonitoredItemViewModel.Count == mapTagIOEntityReference.Count)
                                    {
                                        canExecute.Set();
                                    }
                                }
                            }
                        });

                        var n = mapTagIOEntityReference[key];
                        Opc.Ua.Client.Session session = n.MonitoredItemViewModel.NodeIdModel?.session;
                        if (session != null)
                        {
                            if (!listSyncRead.Contains(n.MonitoredItemViewModel))
                                listSyncRead.Add(n.MonitoredItemViewModel);
                        }
                        else if (IsGoodDataValue(mapTagIOEntityReference[key].MonitoredItemViewModel.DataValue))
                        {
                            if (!mapMonitoredItemViewModel.ContainsKey(key))
                                mapMonitoredItemViewModel.Add(key, mapTagIOEntityReference[key].MonitoredItemViewModel);

                            if (canExecute != null && opcuaEntityReference.Count == 0 &&
                                mapMonitoredItemViewModel.Count == mapTagIOEntityReference.Count)
                            {
                                canExecute.Set();
                            }
                        }
                    }

                    mapTagIOEntityReference[key].Resolve(sessionname, Parent);
                    mapTagIOEntityReference[key].SetInUse(RecipeEntity, true);
                }
            }

            if (listSyncRead.Count > 0)
            {
                foreach (var monitoredItemViewModel in listSyncRead)
                {
                    Opc.Ua.Client.Session session = monitoredItemViewModel.NodeIdModel?.session;
                    try
                    {
                        monitoredItemViewModel.DataValue = session.ReadValue(monitoredItemViewModel.monitoredItem.ResolvedNodeId);
                    }
                    catch
                    { }
                }
            }

            if (canExecute != null && timeout != TimeSpan.Zero)
                canExecute.WaitOne(timeout);
        }

        void PrepareEntityReferences()
        {
            if (opcuaEntityReference == null)
            {
                opcuaEntityReference = new Dictionary<String, OPCUAEntityReference>();

                if (!String.IsNullOrEmpty(RecipeEntity.StartingAddress))
                {
                    var key = GetDriverName(RecipeEntity.StartingAddress);
                    if (!String.IsNullOrEmpty(key) && !opcuaEntityReference.ContainsKey(key))
                    {
                        var entity = GetEntityReference(key);
                        if (entity != null)
                            opcuaEntityReference[key] = entity;
                    }
                }

                var validgroups = (from recipegroup in RecipeEntity.Groups.AsParallel()
                                   where !String.IsNullOrEmpty(recipegroup.StartingAddress)
                                   select recipegroup).ToList();

                validgroups.ForEach(group =>
                {
                    var key = GetDriverName(group.StartingAddress);
                    if (!String.IsNullOrEmpty(key) && !opcuaEntityReference.ContainsKey(key))
                    {
                        var entity = GetEntityReference(key);
                        if (entity != null)
                            opcuaEntityReference[key] = entity;
                    }
                });

                var validvalues = (from recipevalues in RecipeEntity.GetFlatDataValuesCollection().AsParallel()
                                   where recipevalues.UseInCommunication && !String.IsNullOrEmpty(recipevalues.StartingAddress)
                                   select recipevalues).ToList();

                validvalues.ForEach(value =>
                {
                    var key = GetDriverName(value.StartingAddress);
                    if (!String.IsNullOrEmpty(key) && !opcuaEntityReference.ContainsKey(key))
                    {
                        var entity = GetEntityReference(key);
                        if (entity != null)
                            opcuaEntityReference[key] = entity;
                    }
                });
            }

            if (listTagIOEntityReference == null && mapTagIOEntityReference == null)
            {
                var validDataValues = (from c in RecipeEntity.GetFlatDataValuesCollection()
                                       where c.IsTagIOReferenceValid()
                                       select c).ToList();

                listTagIOEntityReference = new List<OPCUAEntityReference>(validDataValues.Count);
                mapTagIOEntityReference = new Dictionary<Guid, OPCUAEntityReference>(validDataValues.Count);
                mapInheritedRecipeEntity = new Dictionary<Guid, UFDataValueEntity>();
                foreach (var dataValue in validDataValues)
                {
                    if (!mapTagIOEntityReference.ContainsKey(dataValue.NodeId))
                    {
                        var entityReference = new OPCUAEntityReference(dataValue.TagIODataValue);
                        listTagIOEntityReference.Add(entityReference);
                        mapTagIOEntityReference.Add(dataValue.NodeId, entityReference);
                        if (dataValue.InheritDataTypeFromTag && !mapInheritedRecipeEntity.ContainsKey(dataValue.NodeId))
                            mapInheritedRecipeEntity.Add(dataValue.NodeId, dataValue);
                    }
                }
            }
        }

        void timerCallback(Object state)
        {
            var key = state as String;

            var oldPriority = Thread.CurrentThread.Priority;
            try
            {
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                var bCan = opcuaEntityReference[key].NodeIdViewModel.IsMethodExecutable && (
                    mapTagIOEntityReference.Count == 0 || mapMonitoredItemViewModel == null ||
                    mapTagIOEntityReference.Count == mapMonitoredItemViewModel.Count);
                if (bCan != bCanExecute)
                {
                    bCanExecute = bCan;

                    if (canExecute != null)
                    {
                        if (bCanExecute)
                            canExecute.Set();
                        else
                            canExecute.Reset();
                    }
#if !NET_STANDARD
                    InvalidateRequerySuggestedAsynchronously();
#endif
                }
            }
            catch (Exception ex)
            {
                if (bCanExecute)
                {
                    bCanExecute = false;

                    if (canExecute != null)
                        canExecute.Reset();
#if !NET_STANDARD
                    InvalidateRequerySuggestedAsynchronously();
#endif
                }
            }
            Thread.CurrentThread.Priority = oldPriority;

            lock (lockObject)
            {
                if (bDisposed)
                    return;

                // renew timer
                if (mapTimer.ContainsKey(key))
                    mapTimer[key].Dispose();
                mapTimer[key] = new Timer(timerCallback, key, 1000, Timeout.Infinite);
            }
        }

        bool IsGoodDataValue(DataValue dataValue)
        {
            return dataValue != null && StatusCode.IsNotBad(dataValue.StatusCode);
        }

        String GetDriverName(String startAddress)
        {
            var driverName = String.Empty;
            int index = startAddress.IndexOf('.');
            if (index != -1)
                driverName = startAddress.Substring(0, index);

            return driverName;
        }

        OPCUAEntityReference GetEntityReference(String driverName)
        {
            String erString;
            if (bWrite)
                erString = UfuaEditorService.GetWriteValuesEntityReference(Parent, driverName);
            else
                erString = UfuaEditorService.GetReadValuesEntityReference(Parent, driverName);

            if (String.IsNullOrEmpty(erString))
                return null;
            return erString.FromXml<OPCUAEntityReference>();
        }

        public ExecutionResult Execute(Variant[] values)
        {
            var executionResult = new ExecutionResult { ResultState = false };
            if (!CanExecute()) return executionResult;

            var monitoredItemViewModels = listTagIOEntityReference
                .Where(x => x.MonitoredItemViewModel != null)
                .Select(entityRef => entityRef.MonitoredItemViewModel)
                .ToList();

            if (monitoredItemViewModels.Count != values.Length)
                return executionResult;

            try
            {
                bExecuting = true;
                executionResult.OutputValues = new Variant[monitoredItemViewModels.Count];
                if (bWrite)
                {
                    var valuesToWrite = values.Select(x => x.Value).ToList();
                    _opcSessionService.WriteValues(monitoredItemViewModels, valuesToWrite);
                }
                else
                {
                    var readValues = _opcSessionService.ReadValues(monitoredItemViewModels);
                    executionResult.OutputValues = readValues.Select(x => new Variant(x.Value)).ToArray();
                }
                executionResult.ResultState = true;
            }
            catch (Exception ex)
            {
                executionResult.ExceptionInfo = ex;
            }
            finally
            {
                bExecuting = false;
            }

            return executionResult;
        }

        
        public ExecutionResult Execute(String startAddress, Variant[] values, int timeout = 0)
        {
            var executionResult = new ExecutionResult() { ResultState = false };
            if (CanExecute())
            {
                if (timeout == 0)
                    timeout = DefaultTimeout;

                if (timeout > 0)
                {
                    DateTime dt = DateTime.UtcNow.Add(TimeSpan.FromMilliseconds(timeout));
                    int sleepTime = 100;
                    do
                    {
                        executionResult = Execute(startAddress, values);
                        if (!executionResult.ResultState)
                        {
                            Thread.Sleep(sleepTime);
                            if (sleepTime < 1000)
                                sleepTime *= 2;
                        }
                    } while (!executionResult.ResultState && dt >= DateTime.UtcNow);
                }
                else
                    executionResult = Execute(startAddress, values);
            }

            return executionResult;
        }

        bool bExecuting;
        ExecutionResult Execute(String startAddress, Variant[] values)
        {
#if DEBUG
            var watcher = new Stopwatch();
            watcher.Start();
#endif
            ExecutionResult ret = new ExecutionResult();
            try
            {
                bExecuting = true;
                var key = GetDriverName(startAddress);
                ret.OutputValues = opcuaEntityReference[key].NodeIdViewModel.CallMethod(startAddress, values).ToArray();
                ret.ResultState = true;
            }
            catch (Exception ex)
            {
                ret.ExceptionInfo = ex;
            }
            finally
            {
                bExecuting = false;
            }

#if DEBUG
            watcher.Stop();
            Debug.WriteLine(String.Format("RecipeEntityHelper CallMethod for start address {0}, executed in {1}", startAddress, watcher.Elapsed));
#endif

            return ret;
        }

        public bool HasReferences()
        {
            return opcuaEntityReference.Count != 0 || mapTagIOEntityReference.Count != 0;
        }

        public bool CanExecute(int millisecondsTimeout)
        {
            if (canExecute != null)
                return canExecute.WaitOne(millisecondsTimeout);
            else
                return false;
        }

        bool bCanExecute;
        public bool CanExecute()
        {
            if (bDisposed || bExecuting)
                return false;

            try
            {
                if (!HasReferences())
                    return false;
                else if (mapTagIOEntityReference.Count > 0)
                    return true;

                foreach (var key in opcuaEntityReference.Keys)
                {
                    if (opcuaEntityReference[key] == null ||
                        opcuaEntityReference[key].NodeIdViewModel == null/* || !OpcuaEntityReference[key].NodeIdViewModel.IsMethod*/)
                        return false;
                }

                return bCanExecute;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void TerminateExecution()
        {
            var entityReferences = new List<OPCUAEntityReference>();
            lock (lockObject)
            {
                if (!bExecuted)
                    return;
                bExecuted = false;

                if (mapObserver != null)
                {
                    foreach (var observer in mapObserver.Values)
                        observer.Dispose();
                    mapObserver.Clear();
                }

                if (mapObserverDataValue != null)
                {
                    foreach (var observer in mapObserverDataValue.Values)
                        observer.Dispose();
                    mapObserverDataValue.Clear();
                }

                if (opcuaEntityReference != null)
                {
                    foreach (var key in opcuaEntityReference.Keys)
                    {
                        if (opcuaEntityReference[key] != null
#if !NET_STANDARD
                            && opcuaEntityReference[key].IsValid
#endif
                            )
                            entityReferences.Add(opcuaEntityReference[key]);
                    }

                    opcuaEntityReference.Clear();
                    opcuaEntityReference = null;
                }

                if (mapTagIOEntityReference != null)
                {
                    foreach (var key in mapTagIOEntityReference.Keys)
                    {
                        if (mapTagIOEntityReference[key] != null
#if !NET_STANDARD
                            && mapTagIOEntityReference[key].IsValid
#endif
                            )
                            entityReferences.Add(mapTagIOEntityReference[key]);
                    }

                    mapTagIOEntityReference.Clear();
                    mapTagIOEntityReference = null;
                }

                if (listTagIOEntityReference != null)
                {
                    listTagIOEntityReference.Clear();
                    listTagIOEntityReference = null;
                }

                if (mapInheritedRecipeEntity != null)
                {
                    mapInheritedRecipeEntity.Clear();
                    mapInheritedRecipeEntity = null;
                }

                if (canExecute != null)
                {
                    canExecute.Dispose();
                    canExecute = null;
                }
            }

            entityReferences.ForEach((entity) => entity.SetInUse(RecipeEntity, false));
        }

#if !NET_STANDARD
        DispatcherOperation dpUpdate;
        void InvalidateRequerySuggestedAsynchronously()
        {
            if (Control == null)
                return;

            lock (lockObject)
            {
                if (dpUpdate == null ||
                    dpUpdate.Status == DispatcherOperationStatus.Completed ||
                    dpUpdate.Status == DispatcherOperationStatus.Aborted)
                {
                    dpUpdate = Control.Dispatcher.BeginInvokeAsynchronouslyInBackground(Control as FrameworkElement, () =>
                    {
                        System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                    });
                }
            }
        }
#endif
        #endregion

        #region IDisposable

        bool bDisposed;

        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            TerminateExecution();

            var notifyObjects = new List<WaitHandle>();
            lock (lockObject)
            {
                if (mapTimer != null)
                {
                    foreach (var timer in mapTimer.Values)
                    {
                        var notifyObject = new AutoResetEvent(false);
                        timer.Change(0, System.Threading.Timeout.Infinite);
                        timer.Dispose(notifyObject);
                        notifyObjects.Add(notifyObject);
                    }
                    mapTimer.Clear();
                }
#if !NET_STANDARD
                if (dpUpdate != null &&
                    dpUpdate.Status != DispatcherOperationStatus.Aborted &&
                    dpUpdate.Status != DispatcherOperationStatus.Completed)
                    dpUpdate.Abort();
#endif
            }

            if (notifyObjects.Count > 0)
            {
                WaitHandle.WaitAll(notifyObjects.ToArray());
                notifyObjects.ForEach((notify) => notify.Dispose());
            }
        }

        #endregion
    }
}
