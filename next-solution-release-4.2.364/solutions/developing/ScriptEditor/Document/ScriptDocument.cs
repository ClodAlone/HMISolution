using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using UFInterfaces.Constants;
using ViewModelLib;
using System.IO;
using System.ComponentModel;
using System.Xml;
using DocumentManager.ComponentService;
using System.Windows.Controls;
using VFS;
using ScriptVariableValues;
using OPCUAViewModel;
using UFInterfaces;
using System.Windows.Media;
using Utilities;
using log4net;
using System.Dynamic;
using System.Threading;
using System.Windows;
using WinWrap.Basic;
using System.Reflection;
using UFUAEditor.ComponentService;
using System.Threading.Tasks;
using System.Globalization;
using UFInterfaces.PropertyControl;
using UIMsgBoxAlertService.ComponentService;
using DocumentManager.ComponentService.Helpers;
using Utilities.Converters;

namespace ScriptManager.Document
{
    [Flags]
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum ScriptStatus : int
    {
        None = 0,
        Error = 1,
        DoEventing = 16,
        Starting = 32,
        Stopping = 64,
        Running = 128
    }

    public class LocalizedEnumConverter : ResourceEnumConverter
    {
        public LocalizedEnumConverter(Type type)
            : base(type, Properties.Resources.ResourceManager)
        {

        }
    }

    [DataContract(Name = "ScriptDocument", Namespace = Namespaces.UriProgea)]
    public class ScriptDocument : ViewModelBase, ICloneable, IDocument, INotifyPropertyVisibilityChanged, IEntityReference
    {
        #region Declarations

        static string TempFilenamePreface = "default";
        static int TempFilenameCount = 0;
        #endregion

        #region Constructors

        public ScriptDocument()
        {
        }

        #endregion

        #region Persistance

        [DataMember]
        String sCode;
        [DataMember]
        int nStartSel;
        [DataMember]
        int nSelLength;
        [DataMember]
        int nSleepTime = 50;
        [DataMember]
        int nMaxExecutionRestarts = 0;
        [DataMember]
        bool bEnableLog;
        [DataMember]
        bool bEnableSysLog;
        [DataMember]
        int[] breakpoints;
        [DataMember]
        string sessionName;
        [DataMember]
        int removeDisabledItemAfterSecs = 30;
        [DataMember]
        int maxCleanCount = 2;
        [DataMember]
        bool useAlwaysSecureConnections = false;
        [DataMember]
        int slowSamplingInterval = 5000;
        [DataMember]
        bool disableWhenNotUsed = true;
        [DataMember]
        int publishingInterval = 1000;
        [DataMember]
        int fastSamplingInterval = 500;
        [DataMember]
        int stopCommandTimeout = 2000;
        [DataMember]
        int writeTimeout = 0;
        [DataMember]
        bool forceWritingOnServer = false;
        [DataMember]
        ThreadPriority threadPriority = ThreadPriority.Normal;
        [DataMember]
        List<String> listVariableUsed;
        [DataMember]
        OPCUAEntityReference cycleTimeTag;
        [DataMember]
        OPCUAEntityReference currentStatusTag;
        [DataMember]
        Guid id;

        #endregion

        #region Methods

        bool IsBelongFromParent(IDocument parent)
        {
            return id == Guid.Empty || id == parent.Id;
        }

        internal void SubscribeSystemVariables()
        {
            if (_runtimeCycleTimeTag == null && 
                CycleTimeTag != null && CycleTimeTag.IsValid)
            {
                var tagxml = CycleTimeTag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    _runtimeCycleTimeTag = tagxml.FromXml<OPCUAEntityReference>();

                if (_runtimeCycleTimeTag != null)
                {
                    _runtimeCycleTimeTag.Resolve(SessionString, parent);
                    _runtimeCycleTimeTag.SetInUse(this, true);
                }
            }
            if (_runtimeCurrentStatusTag == null && 
                CurrentStatusTag != null && CurrentStatusTag.IsValid)
            {
                var tagxml = CurrentStatusTag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    _runtimeCurrentStatusTag = tagxml.FromXml<OPCUAEntityReference>();

                if (_runtimeCurrentStatusTag != null)
                {
                    _runtimeCurrentStatusTag.Resolve(SessionString, parent);
                    _runtimeCurrentStatusTag.SetInUse(this, true);
                }
            }
        }

        internal void UnsubscribeSystemVariables()
        {
            lock (lockObject)
            {
                if (_runtimeCycleTimeTag != null && _runtimeCycleTimeTag.IsValid)
                {
                    _runtimeCycleTimeTag.SetInUse(this, false);
                    _runtimeCycleTimeTag = null;
                }
                if (cycleTimeObserver != null)
                {
                    cycleTimeObserver.Dispose();
                    cycleTimeObserver = null;
                }

                if (_runtimeCurrentStatusTag != null && _runtimeCurrentStatusTag.IsValid)
                {
                    _runtimeCurrentStatusTag.SetInUse(this, false);
                    _runtimeCurrentStatusTag = null;
                }
                if (currentStatusObserver != null)
                {
                    currentStatusObserver.Dispose();
                    currentStatusObserver = null;
                }
            }
        }

        PropertyObserver<OPCUAEntityReference> cycleTimeObserver;
        internal void ChangeCycleTime(TimeSpan time)
        {
            lock (lockObject)
            {
                CycleTime = time;

                if (_runtimeCycleTimeTag != null)
                {
                    if (_runtimeCycleTimeTag.MonitoredItemViewModel == null)
                    {
                        if (cycleTimeObserver == null)
                        {
                            cycleTimeObserver = new PropertyObserver<OPCUAEntityReference>(_runtimeCycleTimeTag);
                            cycleTimeObserver.RegisterHandler(n => n.MonitoredItemViewModel, n =>
                            {
                                if (n.MonitoredItemViewModel != null)
                                {
                                    lock (lockObject)
                                    {
                                        if (cycleTimeObserver != null)
                                        {
                                            cycleTimeObserver.Dispose();
                                            cycleTimeObserver = null;
                                        }

                                        ChangeCycleTime(CycleTime);
                                    }
                                }
                            });
                        }
                    }
                    else
                    {
                        try
                        {
                            _runtimeCycleTimeTag.MonitoredItemViewModel.WriteValue(CycleTime.TotalMilliseconds);
                        }
                        catch (Exception ex)
                        {
                            var syslog = LogManager.GetLogger(Title);
                            syslog.Error(String.Format(Properties.Resources.WriteVariableException, _runtimeCycleTimeTag.HumanReadable), ex);
                        }
                    }
                }
            }
        }

        PropertyObserver<OPCUAEntityReference> currentStatusObserver;
        internal void ChangeStatus(ScriptStatus add, ScriptStatus remove, String error = null)
        {
            lock (lockObject)
            {
                CurrentStatus |= add;
                CurrentStatus &= ~(remove);
                if (String.IsNullOrEmpty(error) ||
                    (CurrentStatus & ScriptStatus.Error) != 0)
                    currentError = error;

                if (_runtimeCurrentStatusTag != null)
                {
                    if (_runtimeCurrentStatusTag.MonitoredItemViewModel == null)
                    {
                        if (currentStatusObserver == null)
                        {
                            currentStatusObserver = new PropertyObserver<OPCUAEntityReference>(_runtimeCurrentStatusTag);
                            currentStatusObserver.RegisterHandler(n => n.MonitoredItemViewModel, n =>
                            {
                                if (n.MonitoredItemViewModel != null)
                                {
                                    lock (lockObject)
                                    {
                                        if (currentStatusObserver != null)
                                        {
                                            currentStatusObserver.Dispose();
                                            currentStatusObserver = null;
                                        }

                                        ChangeStatus(CurrentStatus, ScriptStatus.None, CurrentError);
                                    }
                                }
                            });
                        }
                    }
                    else
                    {
                        try
                        {
                            _runtimeCurrentStatusTag.MonitoredItemViewModel.WriteValue((int)CurrentStatus);
                        }
                        catch (Exception ex)
                        {
                            var syslog = LogManager.GetLogger(Title);
                            syslog.Error(String.Format(Properties.Resources.WriteVariableException, _runtimeCurrentStatusTag.HumanReadable), ex);
                        }
                    }
                }
            }
        }

        internal void UpdateSessionSettings()
        {
            if (String.IsNullOrEmpty(SessionName))
                return;

            String[] serverUriArray = null;
#if !WINDOWS_UWP
            var ufuaEditor = GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (ufuaEditor != null)
                serverUriArray = ufuaEditor.GetServerUriArray(this);
#endif
            RealTimeConnectionManagerViewModel.AddSessionSettings(SessionString, new SessionSettings()
            {
                ParentTitle = Parent.Title,
                RemoveDisabledItemAfterSecs = this.RemoveDisabledItemAfterSecs,
                MaxCleanCount = this.MaxCleanCount,
                UseAlwaysSecureConnections = this.UseAlwaysSecureConnections,
                SlowSamplingInterval = this.SlowSamplingInterval,
                DisableWhenNotUsed = this.DisableWhenNotUsed,
                PublishingInterval = this.PublishingInterval,
                ServerArray = serverUriArray,
                FastSamplingInterval = this.FastSamplingInterval
            });
        }

        List<VariableValues> variableValues;
        List<VariableValues> variableValuesRuntime;
        Dictionary<String, VariableValues> mapvariableValuesRuntime;
        List<OPCUAEntityReference> inuseVariableValueReferences;
        List<PropertyObserver<OPCUAEntityReference>> listPropertyObserverEntityReferenceResolveVariable;
        Dictionary<String, PropertyObserver<MonitoredItemViewModel>> listPropertyObserverMonitoredItemResolveVariable;
        Dictionary<String, OPCUAEntityReference> mapResolvedVariables;
        internal void TerminateVariableValues()
        {
            lock (lockObject)
            {
                if (inuseVariableValueReferences != null)
                {
                    inuseVariableValueReferences.ToList().ForEach(item =>
                    {
                        item.SetInUse(this, false);
                    });
                    inuseVariableValueReferences.Clear();
                }
                if (listPropertyObserverMonitoredItemResolveVariable != null)
                {
                    listPropertyObserverMonitoredItemResolveVariable.Values.ToList().ForEach(item =>
                    {
                        if (item != null)
                            item.Dispose();
                    });
                    listPropertyObserverMonitoredItemResolveVariable.Clear();
                }
                if (listPropertyObserverEntityReferenceResolveVariable != null)
                {
                    listPropertyObserverEntityReferenceResolveVariable.ToList().ForEach(item =>
                    {
                        item.Dispose();
                    });
                    listPropertyObserverEntityReferenceResolveVariable.Clear();
                }
                if (mapResolvedVariables != null)
                {
                    mapResolvedVariables.Clear();
                    mapResolvedVariables = null;
                }
                if (variableValuesRuntime != null)
                {
                    var list = variableValuesRuntime.ToList();
                    variableValuesRuntime.Clear();
                    mapvariableValuesRuntime.Clear();
                    list.ForEach(value =>
                        {
                            value.TerminateResolvedItem();
                        });
                    variableValuesRuntime = null;
                }
            }
        }

        static readonly String qualityTag = "{0}Quality";
        static readonly String timestampTag = "{0}Timestamp";
        static readonly String qualityTagName = "Quality";
        static readonly String timestampTagName = "Timestamp";

        VariableValues SubscribeVariableValuesChanged(String instance = null, bool isDataService = false)
        {
            VariableValues varvalues = null;
            if (String.IsNullOrEmpty(instance))
                varvalues = new VariableValues();
            else
                varvalues = new VariableValues(instance);
            varvalues.isDataService = isDataService;

            varvalues.WriteVariable += (o, e) =>
            {
                if (bPendingVariableChangedEvent)
                    throw new StackOverflowException("Cannot set a variable value inside a variable value changed event");

                var instanceName = e.Name;
                if (!String.IsNullOrEmpty(instance))
                    instanceName = String.Format("{0}-{1}", instance, e.Name);

                try
                {
                    bool bFound = false;
                    var dateTime = DateTime.Now;
                    int timetoWait = WriteTimeout < 5000 ? 5000 : WriteTimeout;
                    var timeTo = dateTime.AddMilliseconds(timetoWait);
                    do
                    {
                        lock (lockObject)
                        {
                            if (mapResolvedVariables != null &&
                                mapResolvedVariables.ContainsKey(instanceName) &&
                                mapResolvedVariables[instanceName].MonitoredItemViewModel != null)
                            {
                                bFound = true;
                                break;
                            }
                        }

                        // WaitForPriority.DoEvents();
                        System.Threading.Thread.Sleep(100);
                    } while (timetoWait > 0 && timeTo > DateTime.Now);

                    if (bFound)
                    {
                        try
                        {
                            if (!ForceWritingOnServer && e.Value != null)
                            {
                                var v = Convert.ToString(e.Value);
                                if (e.Value is Array)
                                {
                                    var variant = new Opc.Ua.Variant(e.Value);
                                    v = String.Format(CultureInfo.InvariantCulture, "{0}", variant);
                                }
                                if (mapResolvedVariables[instanceName].MonitoredItemViewModel.IsValueEqual(v))
                                    return;
                            }
                        }
                        catch { }

                        dateTime = DateTime.Now;
                        timeTo = dateTime.AddMilliseconds(WriteTimeout > 0 ? WriteTimeout : 0);
                        Exception ex = null;
                        do
                        {
                            try
                            {
                                mapResolvedVariables[instanceName].MonitoredItemViewModel.WriteValue(e.Value);
                                ex = null;
                                break;
                            }
                            catch (Exception exception)
                            {
                                ex = exception;
                                if (WriteTimeout > 0)
                                    Thread.Sleep(500);
                                else
                                    break;
                            }
                        } while (WriteTimeout > 0 && timeTo > DateTime.Now);

                        if (ex != null)
                            throw ex;

                        varvalues.SetVariableValue(e.Name, e.Value);

                        OnVariableChanged(new VariableChangedEventArgs(instanceName, mapResolvedVariables[instanceName].MonitoredItemViewModel.DataValue));
                    }
                    else
                        throw new Exception(String.Format("Cannot find the variable {0} for writing the value {1}", instanceName, e.Value));
                }
                catch (Exception ex)
                {
                    var syslog = LogManager.GetLogger(Title);
                    syslog.Error(String.Format(Properties.Resources.WriteVariableException, instanceName), ex);
                    throw ex;
                }
            };

            var mapLocalResolvedVariables = new Dictionary<String, OPCUAEntityReference>();
            varvalues.ResolveVariable += (o, e) =>
            {
                var varName = e.Name;
                var instanceName = e.Name;
                if (!String.IsNullOrEmpty(instance))
                    instanceName = String.Format("{0}-{1}", instance, e.Name);

                lock (lockObject)
                {
                    if (mapResolvedVariables == null)
                        mapResolvedVariables = new Dictionary<String, OPCUAEntityReference>();
                    if (!mapResolvedVariables.ContainsKey(instanceName))
                    {
                        if (mapLocalResolvedVariables.ContainsKey(instanceName))
                            mapResolvedVariables.Add(instanceName, mapLocalResolvedVariables[instanceName]);
                        else
                        {
                            if (isDataService)
                            {
                                var data = OPCUAEntityReference.GetDataSinkInterface(instance);
                                if (data != null)
                                {
                                    string name = e.Name.Replace('\\', '&');
                                    var entity = data.GetReference(name);
                                    if (entity != null)
                                        mapResolvedVariables.Add(instanceName, entity);
                                    else
                                        throw new Exception("Tag not found !");
                                }
                            }
                            else
                            {
                                var original = e.Name;
                                //var originalCounter = -1;
                                //var secondloop = false;
                                var nameToCheck = e.Name;
                                // while (true)
                                // {
                                var eaEntity = new GetTagEntityReference(nameToCheck, instance);
                                OnGetTagEntityReference(eaEntity);
                                if (eaEntity.entityReference != null)
                                {
                                    // varName = nameToCheck;
                                    mapResolvedVariables.Add(instanceName, eaEntity.entityReference);
                                    // break;
                                }
                                else
                                {
                                    if (nameToCheck.EndsWith(qualityTagName))
                                    {
                                        var nameNoQuality = nameToCheck.Substring(0, nameToCheck.Length - qualityTagName.Length);
                                        eaEntity = new GetTagEntityReference(nameNoQuality, instance);
                                        OnGetTagEntityReference(eaEntity);
                                        if (eaEntity.entityReference != null)
                                        {
                                        // varName = nameNoQuality;
                                            varName = instanceName = nameNoQuality;
                                            mapResolvedVariables.Add(nameNoQuality, eaEntity.entityReference);
                                            // break;
                                        }
                                        else
                                            throw new Exception("Tag not found !");
                                    }
                                    else if (nameToCheck.EndsWith(timestampTagName))
                                    {
                                        var nameNoTimestamp = e.Name.Substring(0, nameToCheck.Length - timestampTagName.Length);
                                        eaEntity = new GetTagEntityReference(nameNoTimestamp, instance);
                                        OnGetTagEntityReference(eaEntity);
                                        if (eaEntity.entityReference != null)
                                        {
                                        // varName = nameNoTimestamp;
                                            varName = instanceName = nameNoTimestamp;
                                            mapResolvedVariables.Add(nameNoTimestamp, eaEntity.entityReference);
                                            // break;
                                        }
                                        else
                                            throw new Exception("Tag not found !");
                                    }
                                    else
                                        throw new Exception("Tag not found !");

                                    //                                    var replace = RegexExt.ReplaceFirst(nameToCheck, "_", "\\");
                                    //                                    if (replace == nameToCheck || secondloop)
                                    //                                    {
                                    //restartLoop:
                                    //                                        if (originalCounter == -1)
                                    //                                            originalCounter = original.Split(new char[] { '_' }).Length;
                                    //                                        if (originalCounter < 2)
                                    //                                        {
                                    //                                            if (secondloop)
                                    //                                                throw new Exception("Tag not found !");
                                    //                                            else
                                    //                                            {
                                    //                                                secondloop = true;
                                    //                                                originalCounter = -1;
                                    //                                                nameToCheck = original;
                                    //                                                goto restartLoop;
                                    //                                            }
                                    //                                        }

                                    //                                        var indexof = RegexExt.IndexOfNth(original, "_", 0, --originalCounter);
                                    //                                        if (indexof == -1)
                                    //                                        {
                                    //                                            if (secondloop)
                                    //                                                throw new Exception("Tag not found !");
                                    //                                            else
                                    //                                            {
                                    //                                                secondloop = true;
                                    //                                                originalCounter = -1;
                                    //                                                nameToCheck = original;
                                    //                                                goto restartLoop;
                                    //                                            }
                                    //                                        }

                                    //                                        var sb = new StringBuilder(secondloop ? nameToCheck : original);
                                    //                                        sb[indexof] = '\\';
                                    //                                        nameToCheck = sb.ToString();
                                    //                                    }
                                    //                                    else
                                    //                                        nameToCheck = replace;
                                    //                                }
                                }
                            }
                        }
                    }

                    if (mapResolvedVariables.ContainsKey(instanceName))
                    {
                        if (inuseVariableValueReferences == null)
                            inuseVariableValueReferences = new List<OPCUAEntityReference>();
                        if (!inuseVariableValueReferences.Contains(mapResolvedVariables[instanceName]))
                        {
                            varvalues.AddVariable(varName, false);
                            varvalues.AddVariable(String.Format(qualityTag, varName), false, true);
                            varvalues.AddVariable(String.Format(timestampTag, varName), false, true);

                            var observer = new PropertyObserver<OPCUAEntityReference>(mapResolvedVariables[instanceName]);
                            lock (lockObject)
                            {
                                if (listPropertyObserverEntityReferenceResolveVariable == null)
                                    listPropertyObserverEntityReferenceResolveVariable = new List<PropertyObserver<OPCUAEntityReference>>();
                                listPropertyObserverEntityReferenceResolveVariable.Add(observer);
                            }
                            observer.RegisterHandler(n => n.MonitoredItemViewModel, n =>
                            {
                                // observer.UnregisterHandler(p => p.MonitoredItemViewModel);

                                var observerMonitoredModel = new PropertyObserver<MonitoredItemViewModel>(n.MonitoredItemViewModel);
                                lock (lockObject)
                                {
                                    //listPropertyObserverEntityReferenceResolveVariable.Remove(observer);
                                    //observer.Dispose();
                                    if (listPropertyObserverMonitoredItemResolveVariable == null)
                                        listPropertyObserverMonitoredItemResolveVariable = new Dictionary<String, PropertyObserver<MonitoredItemViewModel>>();
                                    else if (listPropertyObserverMonitoredItemResolveVariable.ContainsKey(instanceName))
                                    {
                                        listPropertyObserverMonitoredItemResolveVariable[instanceName].Dispose();
                                        listPropertyObserverMonitoredItemResolveVariable.Remove(instanceName);
                                    }
                                    listPropertyObserverMonitoredItemResolveVariable.Add(instanceName, observerMonitoredModel);
                                }
                                observerMonitoredModel.RegisterHandler(m => m.DataValue, m =>
                                {
                                    varvalues.SetVariableValue(varName, m.DataValue.Value);
                                    varvalues.SetVariableValue(String.Format(qualityTag, varName), m.DataValue.StatusCode.ToString());
                                    varvalues.SetVariableValue(String.Format(timestampTag, varName), m.DataValue.SourceTimestamp);

                                    OnVariableChanged(new VariableChangedEventArgs(instanceName, m.DataValue));
                                });

                                if (n.MonitoredItemViewModel != null && n.MonitoredItemViewModel.DataValue != null)
                                {
                                    varvalues.SetVariableValue(varName, n.MonitoredItemViewModel.DataValue.Value);
                                    varvalues.SetVariableValue(String.Format(qualityTag, varName), n.MonitoredItemViewModel.DataValue.StatusCode.ToString());
                                    varvalues.SetVariableValue(String.Format(timestampTag, varName), n.MonitoredItemViewModel.DataValue.SourceTimestamp);

                                    OnVariableChanged(new VariableChangedEventArgs(instanceName, n.MonitoredItemViewModel.DataValue));
                                }
                            });

                            mapResolvedVariables[instanceName].Resolve(SessionString, parent);
                            mapResolvedVariables[instanceName].SetInUse(this, true);
                            inuseVariableValueReferences.Add(mapResolvedVariables[instanceName]);

                            if (mapResolvedVariables[instanceName].MonitoredItemViewModel != null &&
                                mapResolvedVariables[instanceName].MonitoredItemViewModel.DataValue != null)
                            {
                                varvalues.SetVariableValue(varName, mapResolvedVariables[instanceName].MonitoredItemViewModel.DataValue.Value);
                                varvalues.SetVariableValue(String.Format(qualityTag, varName), mapResolvedVariables[instanceName].MonitoredItemViewModel.DataValue.StatusCode.ToString());
                                varvalues.SetVariableValue(String.Format(timestampTag, varName), mapResolvedVariables[instanceName].MonitoredItemViewModel.DataValue.SourceTimestamp);

                                OnVariableChanged(new VariableChangedEventArgs(instanceName, mapResolvedVariables[instanceName].MonitoredItemViewModel.DataValue));
                            }
                            else
                            {
                                varvalues.SetVariableValue(String.Format(qualityTag, varName), VariableValues.initialQuality);
                                varvalues.SetVariableValue(String.Format(timestampTag, varName), DateTime.MinValue);

                                OnVariableChanged(new VariableChangedEventArgs(instanceName, null));
                            }
                        }
                    }
                }
            };

            if (listVariableUsed != null)
            {
                var found = (from c in listVariableUsed.AsParallel() where (String.IsNullOrEmpty(instance) && !c.Contains("-") ||
                                  c.StartsWith(String.Format("{0}-", instance)))
                                  select c).ToList();

                found.ForEach(var =>
                {
                    var instanceName = var;
                    if (!String.IsNullOrEmpty(instance))
                        var = var.Replace(String.Format("{0}-", instance), "");

                    if (!mapLocalResolvedVariables.ContainsKey(instanceName))
                    {
                        if (isDataService)
                        {
                            var data = OPCUAEntityReference.GetDataSinkInterface(instance);
                            if (data != null)
                            {
                                string name = var.Replace('\\', '&');
                                var entity = data.GetReference(name);
                                if (entity != null)
                                    mapLocalResolvedVariables.Add(instanceName, entity);
                            }
                        }
                        else
                        {
                            var original = var;
                            var nameToCheck = var;
                            var eaEntity = new GetTagEntityReference(nameToCheck, instance);
                            OnGetTagEntityReference(eaEntity);
                            if (eaEntity.entityReference != null)
                                mapLocalResolvedVariables.Add(instanceName, eaEntity.entityReference);
                            else
                            {
                                if (nameToCheck.EndsWith(qualityTagName))
                                {
                                    var nameNoQuality = nameToCheck.Substring(0, nameToCheck.Length - qualityTagName.Length);
                                    if (!mapLocalResolvedVariables.ContainsKey(nameNoQuality))
                                    {
                                        eaEntity = new GetTagEntityReference(nameNoQuality, instance);
                                        OnGetTagEntityReference(eaEntity);
                                        if (eaEntity.entityReference != null)
                                            mapLocalResolvedVariables.Add(nameNoQuality, eaEntity.entityReference);
                                    }
                                }
                                else if (nameToCheck.EndsWith(timestampTagName))
                                {
                                    var nameNoTimestamp = var.Substring(0, nameToCheck.Length - timestampTagName.Length);
                                    if (!mapLocalResolvedVariables.ContainsKey(nameNoTimestamp))
                                    {
                                        eaEntity = new GetTagEntityReference(nameNoTimestamp, instance);
                                        OnGetTagEntityReference(eaEntity);
                                        if (eaEntity.entityReference != null)
                                            mapLocalResolvedVariables.Add(nameNoTimestamp, eaEntity.entityReference);
                                    }
                                }
                            }
                        }
                    }
                });

                if (mapLocalResolvedVariables.Count > 0)
                {
                    foreach (var var in mapLocalResolvedVariables.Keys)
                    {
                        if (String.IsNullOrEmpty(instance))
                            varvalues.OnResolveVariable(new ResolveVariableEventArgs(var));
                        else
                            varvalues.OnResolveVariable(new ResolveVariableEventArgs(var.Replace(String.Format("{0}-", instance), "")));
                    }
                    mapLocalResolvedVariables.Clear();
                }
            }

            return varvalues;
        }

        internal List<VariableValues> GetVariableObjectDispatcher(bool bRuntime = false)
        {
            var ret = new List<VariableValues>();

            lock (lockObject)
            {
                if (bRuntime)
                {
                    if (variableValuesRuntime == null)
                    {
                        variableValuesRuntime = new List<VariableValues>();
                        mapvariableValuesRuntime = new Dictionary<String, VariableValues>();

                        var varvalues = SubscribeVariableValuesChanged();
                        variableValuesRuntime.Add(varvalues);

                        ret.Add(varvalues);

                        //varvalues.FoundVariableNameSuspect += (o, e) =>
                        //{
                        //    var ea = new GetTagListEventArgs();
                        //    OnGetTagList(ea);
                        //    varvalues.PrepareAddNewVariable();
                        //    if (ea.list != null)
                        //    {
                        //        foreach (var tag in ea.list)
                        //        {
                        //            varvalues.AddVariable(tag);
                        //            varvalues.AddVariable(String.Format(qualityTag, tag), false);
                        //            varvalues.AddVariable(String.Format(timestampTag, tag), false);
                        //        }
                        //    }
                        //    varvalues.EndAddNewVariable();
                        //};
                        /*
                        var ea = new GetTagListEventArgs();
                        OnGetTagList(ea);
                        */
                        var eap = new GetPrototypeListEventArgs();
                        OnGetPrototypeList(eap);
                        /*
                        varvalues.PrepareAddNewVariable();
                        if (ea.list != null)
                        {
                            foreach (var tag in ea.list)
                            {
                                varvalues.AddVariable(tag);
                                varvalues.AddVariable(String.Format(qualityTag, tag), false);
                                varvalues.AddVariable(String.Format(timestampTag, tag), false);
                            }
                        }
                        varvalues.EndAddNewVariable();
                        */

                        if (eap.mapPrototypes != null && eap.mapDefinitions != null)
                        {
                            foreach (var instance in eap.mapPrototypes.Keys)
                            {
                                if (eap.mapPrototypes[instance] == null || !eap.mapDefinitions.ContainsKey(eap.mapPrototypes[instance]))
                                    continue;
                                var prototypesValues = SubscribeVariableValuesChanged(instance);
                                variableValuesRuntime.Add(prototypesValues);
                                mapvariableValuesRuntime.Add(instance, prototypesValues);
                                /*
                                prototypesValues.PrepareAddNewVariable();

                                foreach (var member in eap.mapDefinitions[eap.mapPrototypes[instance]])
                                {
                                    prototypesValues.AddVariable(member);
                                    prototypesValues.AddVariable(String.Format(qualityTag, member), false);
                                    prototypesValues.AddVariable(String.Format(timestampTag, member), false);
                                }
                                prototypesValues.EndAddNewVariable();
                                */
                                ret.Add(prototypesValues);
                            }
                        }

                        var listDataSinkInterfaces = OPCUAEntityReference.GetDataSinkInterfaces();
                        listDataSinkInterfaces.ForEach(dataInterface =>
                            {
                                var prototypesValues = SubscribeVariableValuesChanged(dataInterface, true);
                                variableValuesRuntime.Add(prototypesValues);
                                mapvariableValuesRuntime.Add(dataInterface, prototypesValues);
                                prototypesValues.PrepareAddNewVariable();

                                var data = OPCUAEntityReference.GetDataSinkInterface(dataInterface);
                                foreach (var member in data.GetVariables(this))
                                {
                                    var _member = member.Replace('&', '\\');

                                    prototypesValues.AddVariable(_member);
                                    prototypesValues.AddVariable(String.Format(qualityTag, _member), false, true);
                                    prototypesValues.AddVariable(String.Format(timestampTag, _member), false, true);
                                }
                                prototypesValues.EndAddNewVariable();
                                prototypesValues.isDataService = true;
                                ret.Add(prototypesValues);
                            });
                    }
                    else
                        ret.AddRange(variableValuesRuntime); 
                }
                else
                {
                    if (variableValues == null)
                        variableValues = new List<VariableValues>();
                    variableValues.ForEach(value =>
                        {
                            value.Dispose();
                        });
                    variableValues.Clear();
                    var varvalue = new VariableValues(false);
                    variableValues.Add(varvalue);

                    var ea = new GetTagListEventArgs();
                    OnGetTagList(ea);
                    var eap = new GetPrototypeListEventArgs();
                    OnGetPrototypeList(eap);
                    varvalue.PrepareAddNewVariable();
                    if (ea.list != null)
                    {
                        var list = ea.list.ToList();
                        //bool bProcess = true;
                        //if (list.Count > Properties.Settings.Default.MaxVariableIntellisense)
                        //{
                        //    bProcess = MessageBox.Show(String.Format(Properties.Resources.NumberOfVariableCanSlowDown, list.Count),
                        //                            Title, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK;
                        //}

                        // if (bProcess)
                        // Parallel.ForEach(list, tag =>
                        varvalue.AddList(list);
                            //list.ForEach(tag =>
                            //{
                            //    // lock (varvalue)
                            //        {
                            //            varvalue.AddVariable(tag);
                            //            varvalue.AddVariable(String.Format(qualityTag, tag), false, true);
                            //            varvalue.AddVariable(String.Format(timestampTag, tag), false, true);
                            //        }
                            //    });
                    }
                    varvalue.EndAddNewVariable();
                    ret.Add(varvalue);

                    if (eap.mapPrototypes != null && eap.mapDefinitions != null)
                    {
                        foreach (var instance in eap.mapPrototypes.Keys)
                        {
                            if (eap.mapPrototypes[instance] == null || !eap.mapDefinitions.ContainsKey(eap.mapPrototypes[instance]))
                                continue;
                            var prototypesValues = new VariableValues(instance, false);
                            variableValues.Add(prototypesValues);
                            prototypesValues.PrepareAddNewVariable();

                            foreach (var member in eap.mapDefinitions[eap.mapPrototypes[instance]])
                            {
                                prototypesValues.AddVariable(member);
                                prototypesValues.AddVariable(String.Format(qualityTag, member), false, true);
                                prototypesValues.AddVariable(String.Format(timestampTag, member), false, true);
                            }
                            prototypesValues.EndAddNewVariable();
                            ret.Add(prototypesValues);
                        }
                    }

                    var listDataSinkInterfaces = OPCUAEntityReference.GetDataSinkInterfaces();
                    listDataSinkInterfaces.ForEach(dataInterface =>
                    {
                        var prototypesValues = new VariableValues(dataInterface, false);
                        variableValues.Add(prototypesValues);
                        prototypesValues.PrepareAddNewVariable();

                        var data = OPCUAEntityReference.GetDataSinkInterface(dataInterface);
                        foreach (var member in data.GetVariables(this))
                        {
                            var _member = member.Replace('&', '\\');

                            prototypesValues.AddVariable(_member);
                            prototypesValues.AddVariable(String.Format(qualityTag, _member), false, true);
                            prototypesValues.AddVariable(String.Format(timestampTag, _member), false, true);
                        }
                        prototypesValues.EndAddNewVariable();
                        prototypesValues.isDataService = true;
                        ret.Add(prototypesValues);
                    });
                }
            }

            return ret;
        }

        VariableValues GetInstanceVariableValues(String name)
        {
            lock (lockObject)
            {
                if (variableValuesRuntime == null || variableValuesRuntime.Count < 1)
                    return null;

                var names = name.Replace(':', '.').Split('.');
                if (names.Length < 2)
                    return variableValuesRuntime[0];
                if (mapvariableValuesRuntime.ContainsKey(names[0]))
                    return mapvariableValuesRuntime[names[0]];
                return null;
            }
        }

        String GetInstanceVariableName(String name)
        {
            lock (lockObject)
            {
                if (variableValuesRuntime == null || variableValuesRuntime.Count < 1)
                    return name;

                var names = name.Replace(':', '.').Split('.');
                if (names.Length < 2)
                    return name;
                return names[1];
            }
        }

        public Object GetVariableValue(String Name)
        {
            var variableValue = GetInstanceVariableValues(Name);
            if (variableValue == null)
                return null;
            var name = GetInstanceVariableName(Name);
            // Name = Name.Replace('_', '\\');
            variableValue.VerifyResolvedVariable(name);
            Object ret = variableValue.GetVariableValue(name);
            var timeout = DateTime.UtcNow + TimeSpan.FromSeconds(5);
            while (ret == null && timeout > DateTime.UtcNow)
            {
                var quality = variableValue.GetVariableValue(String.Format(qualityTag, name)) as String;
                if (!String.IsNullOrEmpty(quality) && quality.StartsWith("Good"))
                    break;

                Thread.Sleep(200);
                ret = variableValue.GetVariableValue(name);
            }

            return ret;
        }

        public Object GetVariableQuality(String Name)
        {
            var variableValue = GetInstanceVariableValues(Name);
            if (variableValue == null)
                return null;
            var name = GetInstanceVariableName(Name);
            // Name = Name.Replace('_', '\\');
            variableValue.VerifyResolvedVariable(name);
            return variableValue.GetVariableValue(String.Format(qualityTag, name));
        }

        internal void RenameReferences(UFInterfaces.Editors.CrossReferenceModel model)
        {
            using (var cursor = new WaitCursor())
            {
                IUFUAEditorManager uFUAEditorManager = GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                string defaultlocalendpoint = uFUAEditorManager?.GetDefaultLocalEndpoint(this, true);
                var endpointslist = uFUAEditorManager?.GetEndpoints(this);
                string aplicationName = uFUAEditorManager?.GetAplicationName(this, true);

                List<OPCUAViewModel.OPCUAEntityReference> list = new List<OPCUAViewModel.OPCUAEntityReference>();
                bool bStatusTag = false;
                bool bCycleTag = false;
                string statusTagNId = string.Empty;
                string cycleTagNId = string.Empty;
                if (CurrentStatusTag != null && CurrentStatusTag.ResolvedNodeId != null)
                {
                    bStatusTag = true;
                    statusTagNId = CurrentStatusTag.ResolvedNodeId.ToString();
                    list.Add(CurrentStatusTag);
                }
                if (CycleTimeTag != null && CycleTimeTag.ResolvedNodeId != null)
                {
                    bCycleTag = true;
                    cycleTagNId = CycleTimeTag.ResolvedNodeId.ToString();
                    list.Add(CycleTimeTag);
                }

                if (list.Count > 0 && uFUAEditorManager != null)
                {
                    var nodelist = (from c in list
                                    select c.ResolvedNodeId.ToString()).Distinct().ToList();

                    var mapNodes = uFUAEditorManager.GetListNodeNames(this, nodelist);
                    if (mapNodes == null)
                        return;
                    bool bDirty = false;
                    foreach (var node in mapNodes.Keys)
                    {
                        if (model.QuitEvent.IsCancellationRequested)
                            return;

                        var found = (from c in list
                                     where c.ResolvedNodeId != null &&
                                     c.ResolvedNodeId.ToString() == node
                                     select c).ToList();
                        found.ForEach(item =>
                        {
                            if (model.QuitEvent.IsCancellationRequested)
                                return;
                            bool changeEndpoint = !endpointslist.Contains(item.EndpointUrl);
                            if (changeEndpoint)
                            {
                                item.EndpointUrl = item.EndpointUrl.Replace(item.AppName, aplicationName);
                                if (!endpointslist.Contains(item.EndpointUrl))
                                    item.EndpointUrl = defaultlocalendpoint;
                            }
                            item.AppName = aplicationName;
                            var shortname = mapNodes[node];
                            var newName = String.Format("{0} ({1})", shortname, item.AppName);
                            if (item.HumanReadable != newName || changeEndpoint)
                            {
                                item.HumanReadable = newName;
                                item.ReadablePath = CrossReferenceHelper.Helper.GetNewPath(shortname, item.ReadablePath);
                                item.RelativePath = CrossReferenceHelper.Helper.GetNewPath(shortname, item.RelativePath);
                                bDirty = true;

                                if (bStatusTag && statusTagNId == node)
                                    CurrentStatusTag = item;
                                if (bCycleTag && cycleTagNId == node)
                                    CycleTimeTag = item;
                            }
                        });
                    }
                    if (bDirty)
                        try
                        {
                            SaveToFile();
                        }
                        catch (Exception ex)
                        {
                        }
                }
            }
        }

        public bool IsVariableQualityGood(String Name)
        {
            var variableValue = GetInstanceVariableValues(Name);
            if (variableValue == null)
                return false;
            var name = GetInstanceVariableName(Name);
            // Name = Name.Replace('_', '\\');

            variableValue.VerifyResolvedVariable(name);
            try
            {
                var quality = variableValue.GetVariableValue(String.Format(qualityTag, name));
                var iQuality = Convert.ToUInt32(quality);
                var statusCode = new Opc.Ua.StatusCode(iQuality);
                return Opc.Ua.StatusCode.IsGood(statusCode);
            }
            catch (Exception ex)
            {
                var quality = variableValue.GetVariableValue(String.Format(qualityTag, name));
                if (quality is String)
                {
                    var strQuality = quality as String;
                    var statusCode = new Opc.Ua.StatusCode(Opc.Ua.StatusCodes.Good);
                    var compare = statusCode.ToString();
                    if (strQuality == compare)
                        return true;
                }

                return false;
            }
        }

        public Object GetVariableTimestamp(String Name)
        {
            var variableValue = GetInstanceVariableValues(Name);
            if (variableValue == null)
                return null;
            var name = GetInstanceVariableName(Name);
            // Name = Name.Replace('_', '\\');

            variableValue.VerifyResolvedVariable(name);
            return variableValue.GetVariableValue(String.Format(timestampTag, name));
        }

        public Object SetVariableValue(String Name, Object value)
        {
            var variableValue = GetInstanceVariableValues(Name);
            if (variableValue == null)
                return null;
            var name = GetInstanceVariableName(Name);
            // Name = Name.Replace('_', '\\');

            variableValue.VerifyResolvedVariable(name);
            variableValue.OnWriteVariable(new WriteVariableEventArgs(name, value));
            return variableValue.GetVariableValue(name);
        }

        public Object CallMethod(String Instance, String Name, params object[] args)
        {
            var instanceName = Name;
            if (!String.IsNullOrEmpty(Instance))
                instanceName = String.Format("Method {0}-{1}", Instance, Name);

            NodeIdViewModel model = null;
            lock (lockObject)
            {
                if (mapResolvedVariables == null)
                    mapResolvedVariables = new Dictionary<String, OPCUAEntityReference>();
                if (!mapResolvedVariables.ContainsKey(instanceName))
                {
                    var eaEntity = new GetTagEntityReference(Name, Instance);
                    OnGetTagEntityReference(eaEntity);
                    if (eaEntity.entityReference != null)
                    {
                        if (!String.IsNullOrEmpty(Instance))
                        {
                            var nodeidStr = eaEntity.entityReference.ResolvedNodeId.ToString();
                            // parse the namespace index if present.
                            ushort namespaceIndex = 0;
                            if (nodeidStr.StartsWith("ns=", StringComparison.Ordinal))
                            {
                                int index = nodeidStr.IndexOf(';');

                                if (index != -1)
                                {
                                    namespaceIndex = Convert.ToUInt16(nodeidStr.Substring(3, index - 3), CultureInfo.InvariantCulture);
                                    nodeidStr = nodeidStr.Substring(index + 1);
                                }
                            }
                            if (nodeidStr.StartsWith("g=", StringComparison.Ordinal))
                                nodeidStr = nodeidStr.Substring(2);
                            var newNodeId = String.Format("{0}?{1}", nodeidStr, Name);
                            eaEntity.entityReference.ResolvedNodeId = new Opc.Ua.NodeId(newNodeId, namespaceIndex);
                        }
                        mapResolvedVariables.Add(instanceName, eaEntity.entityReference);
                    }
                }

                if (mapResolvedVariables.ContainsKey(instanceName))
                {
                    if (mapResolvedVariables[instanceName].NodeIdViewModel == null)
                    {
                        if (inuseVariableValueReferences == null)
                            inuseVariableValueReferences = new List<OPCUAEntityReference>();
                        if (!inuseVariableValueReferences.Contains(mapResolvedVariables[instanceName]))
                        {
                            mapResolvedVariables[instanceName].Resolve(SessionString, parent);
                            mapResolvedVariables[instanceName].SetInUse(this, true);
                            inuseVariableValueReferences.Add(mapResolvedVariables[instanceName]);

                            var now = DateTime.Now + TimeSpan.FromSeconds(5);
                            while (mapResolvedVariables[instanceName].NodeIdViewModel == null && (now > DateTime.Now))
                                System.Threading.Thread.Sleep(250);
                            model = mapResolvedVariables[instanceName].NodeIdViewModel;
                        }
                    }
                    else
                        model = mapResolvedVariables[instanceName].NodeIdViewModel;
                }
            }

            if (model != null)
            {
                var parameters = new Opc.Ua.VariantCollection();
                foreach (var arg in args)
                    if(arg != null)
                        parameters.Add(new Opc.Ua.Variant(arg));
                
                var ret = model.CallMethod(parameters.ToArray());
                return ret;
            }

            return null;
        }

        internal void UpdateListVariableUsed()
        {
            var map = GetListUsedVariables(skipDataService: true);
            if (map == null)
                listVariableUsed = null;
            else
                listVariableUsed = map.Keys.ToList();
        }
        internal IEnumerable<dynamic> GetListUsedVariablesFromPersistence(UFInterfaces.Editors.CrossReferenceModel model)
        {
            List<string> listDataSinkInterfaces = OPCUAEntityReference.GetDataSinkInterfaces();
            IUFUAEditorManager service = GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            bool getTags = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Tags);

            if (getTags && service != null && listVariableUsed != null)
            {
                for (int i = 0; i < listVariableUsed.Count(); i++)
                {
                    if (model.QuitEvent.IsCancellationRequested)
                        break;

                    var reference = listVariableUsed[i];
                    bool bExist = false;
                    OPCUAEntityReference tag = null;
                    lock (model.ScriptTagMap)
                    {
                        bExist = model.ScriptTagMap.ContainsKey(reference);
                        if (bExist)
                            tag = (OPCUAEntityReference)model.ScriptTagMap[reference];
                    }
                    if (bExist)
                    {
                        yield return tag;
                    }
                    else
                    {
                        var split = reference.Split('-');
                        var instance = split[0];
                        var name = split[0];
                        if (split.Length > 1)
                            name = split[1];
                        else
                            instance = null;
                        if (listDataSinkInterfaces.Contains(instance))
                        {
                            var datasync = OPCUAEntityReference.GetDataSinkInterface(instance);
                            tag = datasync?.GetReference(name);
                        }
                        else
                        {
                            try
                            {
                                var xml = service.GetTagEntityReference(this, name, instance, useCachedUow: true);
                                if (!String.IsNullOrEmpty(xml))
                                    tag = xml.FromXml<OPCUAEntityReference>();
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        if (tag != null)
                        {
                            lock (model.ScriptTagMap)
                            {
                                if (!model.ScriptTagMap.ContainsKey(reference))
                                    model.ScriptTagMap.Add(reference, tag);
                            }
                            yield return tag;
                        }
                    }
                }
            }
        }

        internal Dictionary<String, OPCUAEntityReference> GetListUsedVariables(bool skipDataService = false)
        {
            var service = GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (service == null)
                return null;

            var mapResolvedVariables = new Dictionary<String, OPCUAEntityReference>();
            var basicCtl = new BasicNoUIObj();
#if DEBUG
            if (System.IO.Directory.Exists(@"C:\Program Files (x86)\Polar Engineering\WinWrap Basic\Certificates"))
#endif
            basicCtl.Secret = new Guid(Properties.Settings.Default.SecretKey);

            basicCtl.Initialize();
            basicCtl.EventMode = true;
            basicCtl.LargeIcon = null;
            basicCtl.SmallIcon = null;
            basicCtl.Caption = Path.GetFileNameWithoutExtension(FullPath);
            basicCtl.TaskbarIconMode = WinWrap.Basic.TaskbarIconModeConstants.IconNoneSysmenuNone;

            var opcua = typeof(Opc.Ua.DataValue).Assembly;
            basicCtl.AddExtension("#", opcua);

            basicCtl.AddExtension("$Feature ExtensionCache False", null);
            var listVariables = GetVariableObjectDispatcher(false);

            listVariables.ForEach(variableValue =>
            {
                variableValue.ForceResolveVariables();

                var name = variableValue.GetName();
                if (String.IsNullOrEmpty(name))
                    basicCtl.AddExtension("%", variableValue);
                else
                    basicCtl.AddExtension(String.Format("%{0}.", name), variableValue);

                variableValue.ResolveVariable += (o, e) =>
                {
                    if (skipDataService && variableValue.isDataService)
                        return;

                    var instanceName = e.Name;
                    if (!String.IsNullOrEmpty(variableValue.GetName()))
                        instanceName = String.Format("{0}-{1}", variableValue.GetName(), e.Name);

                    if (!mapResolvedVariables.ContainsKey(instanceName))
                    {
                        if (variableValue.isDataService)
                        {
                            var data = OPCUAEntityReference.GetDataSinkInterface(variableValue.GetName());
                            if (data != null)
                            {
                                string _name = e.Name.Replace('\\', '&');
                                var entity = data.GetReference(_name);
                                if (entity != null)
                                    mapResolvedVariables.Add(instanceName, entity);
                            }
                        }
                        else
                        {
                            var nameToCheck = e.Name;
                            // while (true)
                            {
                                var erString = service.GetTagEntityReference(this, nameToCheck, variableValue.GetName(), useCachedUow: true);
                                if (erString != null)
                                {
                                    var entityReference = erString.FromXml<OPCUAEntityReference>();
                                    if (entityReference != null)
                                        mapResolvedVariables.Add(instanceName, entityReference);
                                    // break;
                                }

                                //var replaced = RegexExt.ReplaceFirst(nameToCheck, "_", "\\");
                                //if (replaced == nameToCheck)
                                //    break;
                                //nameToCheck = replaced;
                            }
                        }
                    }
                };
            });

            var listReferences = new List<Object>();
            listReferences.Add(this);
            listReferences.Add(parent);
            listReferences.Add(new StartupContext());

            listReferences.ForEach(reference =>
            {
                Assembly referenceGetTypeAssembly = reference.GetType().Assembly;
                basicCtl.AddExtension("#", referenceGetTypeAssembly);
                basicCtl.AddExtensionObjectWithEvents(reference.GetType().Name, reference);
            });
            basicCtl.FileTools = false;
            basicCtl.Code = Code;

            var error = String.Empty;
            basicCtl.ErrorAlert += (o, e) =>
            {
                error = String.Format(Properties.Resources.ScriptError, Title, basicCtl.Error.Description);
            };

            basicCtl.ReadMacro += (o, e) =>
            {
                if (e.FileName.StartsWith("*"))
                {
                    var filename = e.FileName.Replace("*", "");
                    var uri = MakeAbosoluteUri(new Uri(FullPath, UriKind.RelativeOrAbsolute));
                    var path = System.IO.Path.GetDirectoryName(uri.GetPathString());
                    var fileToRead = String.Format("{0}\\{1}{2}", path, filename, Properties.Settings.Default.DefaultFileExt);
                    var docMacro = ScriptDocument.FromFile(fileToRead, Parent);
                    if (docMacro != null)
                    {
                        e.Code = docMacro.Code;
                        docMacro.Dispose();
                        e.Changed = true;
                        e.Cancel = false;
                    };
                }
            };
            var handler = basicCtl.CreateHandler(Properties.Settings.Default.EntryPointSub);

            var check = basicCtl.SyntaxCheck();

            while (basicCtl.Shutdown() < 0)
                WaitForPriority.DoEvents();

            handler.Dispose();
            basicCtl.Disconnect();

            if (!check || !String.IsNullOrEmpty(error))
                return null;

            return mapResolvedVariables;
        }

        [OnDeserializing]
        private void PreInitialize(StreamingContext context)
        {
            nSleepTime = 50;
            nMaxExecutionRestarts = 0;
            removeDisabledItemAfterSecs = 30;
            maxCleanCount = 2;
            useAlwaysSecureConnections = false;
            slowSamplingInterval = 5000;
            disableWhenNotUsed = true;
            publishingInterval = 250;
            fastSamplingInterval = 500;
            stopCommandTimeout = 2000;
            writeTimeout = 0;
            forceWritingOnServer = false;
            threadPriority = ThreadPriority.Normal;
        }

        [OnDeserialized]
        private void PostInitialize(StreamingContext context)
        {
            if (sCode != null && !sCode.Contains('\r'))
                sCode = sCode.Replace("\n", Environment.NewLine);
        }

        public bool SaveCurrentDocument()
        {
            if (!NeedsSave)
                return true;

            using (var cursor = new WaitCursor())
            {
                UpdateListVariableUsed();
                return SaveToFile();
            }
        }

        private bool WriteProjectDataStream(Stream ostrm)
        {
            var settings = new XmlWriterSettings
            {
                Encoding = System.Text.Encoding.UTF8,
                Indent = true,
                CloseOutput = true
            };

            using (var writer = XmlDictionaryWriter.Create(ostrm, settings))
            {
                bool bRet = false;
                try
                {
                    var serializer = new DataContractSerializer(typeof(ScriptDocument));
                    serializer.WriteObject(writer, this);
                    NeedsSave = false;
                    bRet = true;
                }
                finally
                {
                    writer.Close();
                }

                return bRet;
            }
        }

        internal bool SaveToFile(bool forceEncryption = false)
        {
            try
            {
                if (fileSystemProviderBase != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        if (!WriteProjectDataStream(memoryStream))
                            return false;

                        fileSystemProviderBase.UploadFile(null, FullPath, memoryStream.ToArray());
                        return true;
                    }
                }

                Directory.CreateDirectory(Path.GetDirectoryName(FullPath));
                String settingsFileName = FullPath;
                if (forceEncryption || Protected)
                {
                    id = Id;
                    using (var memoryStream = new MemoryStream())
                    {
                        if (!WriteProjectDataStream(memoryStream))
                            return false;

                        var str = Convert.ToBase64String(memoryStream.ToArray());
                        var toWrite = WPFUtilities.CryptString.CryptString.EncryptString(str);
                        File.WriteAllText(settingsFileName, toWrite);
                    }
                }
                else
                {
                    id = Guid.Empty;
                    using (var ostrm = File.Open(settingsFileName, FileMode.Create, FileAccess.ReadWrite))
                    {
                        return WriteProjectDataStream(ostrm);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(Properties.Resources.ErrorSavingDocument, ex.Message),
                                        Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        #endregion

        #region Static Methods

        static ScriptDocument ReadFromStream(Stream stream)
        {
            try
            {
                var formatter = new DataContractSerializer(typeof(ScriptDocument));
                var document = formatter.ReadObject(stream) as ScriptDocument;
                return document;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        internal static bool ExistFile(String fullPath, FileSystemProviderBase fileSystemProvider = null)
        {
            if (fileSystemProvider != null)
                return fileSystemProvider.Exists(new FileManagerFile(fileSystemProvider, fullPath));

            return File.Exists(fullPath);
        }

        public static ScriptDocument FromFile(String fullPath, IDocument parent)
        {
            try
            {
                ScriptDocument document = null;
                FileSystemProviderBase fileSystemProvider = parent.fileSystemProviderBase;
                if (fileSystemProvider != null)
                {
                    if (fileSystemProvider.Exists(new FileManagerFile(fileSystemProvider, fullPath)))
                    {
                        var data = fileSystemProvider.ReadFile(new FileManagerFile(fileSystemProvider, fullPath));
                        using (var memoryStream = new MemoryStream(data))
                        {
                            document = ReadFromStream(memoryStream);
                            if (document == null)
                            {
                                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                if (uiMsgBox != null)
                                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, fullPath));
                            }
                            else
                                document.FullPath = fullPath;
                        }
                    }

                    if (document == null)
                    {
                        document = new ScriptDocument()
                        {
                            FullPath = fullPath
                        };
                    }

                    return document;
                }

                if (File.Exists(fullPath))
                {
                    if (parent.Protected || !Utilities.IO.FileSystem.IsXmlFile(fullPath))
                    {
                        var data = WPFUtilities.CryptString.CryptString.DecryptString(File.ReadAllText(fullPath));
                        using (var reader = new MemoryStream(Convert.FromBase64String(data)))
                        {
                            document = ReadFromStream(reader);
                            if (document == null)
                            {
                                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                if (uiMsgBox != null)
                                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, fullPath));
                            }
                            else if (!document.IsBelongFromParent(parent))
                            {
                                document.Dispose();
                                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                if (uiMsgBox != null)
                                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorValidatingDocument, fullPath));
                                return null;
                            }
                            else
                                document.FullPath = fullPath;
                        }
                    }
                    else
                    {
                        using (var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                        {
                            document = ReadFromStream(fileStream);
                            if (document == null)
                            {
                                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                if (uiMsgBox != null)
                                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, fullPath));
                            }
                            else
                                document.FullPath = fullPath;
                        }
                    }
                }

                if (document == null)
                {
                    document = new ScriptDocument()
                    {
                        FullPath = fullPath
                    };
                }

                return document;
            }
            catch
            {
                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (uiMsgBox != null)
                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, fullPath));
                return null;
            }
        }

        internal static void RemoveFile(string fullPath, FileSystemProviderBase fileSystemProvider = null)
        {
            if (fileSystemProvider != null)
            {
                var fileManagerFile = new FileManagerFile(fileSystemProvider, fullPath);
                if (fileSystemProvider.Exists(fileManagerFile))
                {
                    fileSystemProvider.DeleteFile(fileManagerFile);
                }
            }
            else
            {
                if (File.Exists(fullPath))
                    File.Delete(fullPath);
            }
        }

        internal static String RenameFile(String fullPath, String oldName, String newName, FileSystemProviderBase fileSystemProvider = null)
        {
            var folder = Path.GetDirectoryName(fullPath);
            var newPathName = String.Format("{0}\\{1}{2}", folder, newName, Path.GetExtension(fullPath));

            if (fileSystemProvider != null)
            {
                var fileManagerFile = new FileManagerFile(fileSystemProvider, fullPath);
                if (fileSystemProvider.Exists(fileManagerFile))
                {
                    fileSystemProvider.RenameFile(fileManagerFile, String.Format("{0}{1}", newName, fileManagerFile.Extension));
                }
            }
            else
            {
                if (File.Exists(fullPath))
                    File.Move(fullPath, newPathName);
            }

            return newPathName;
        }

        internal static void CopyFile(String fullPath, String newPath, bool bCopy, IDocument parent)
        {
            FileSystemProviderBase fileSystemProvider = parent.fileSystemProviderBase;
            FileSystemProviderBase targetVFS = null;
            bool bDisposeTargetVFS = true;
            bool bTargetDataSource = XpoHelpers.XpoHelper.IsDataSource(newPath);
            if (bTargetDataSource)
            {
                targetVFS = new DataSourceFileSystemProvider("")
                {
                    ConnectionString = newPath
                };
            }
            else
            {
                if (!Path.IsPathRooted(newPath))
                {
                    targetVFS = fileSystemProvider;
                    bDisposeTargetVFS = false;
                }
            }

            try
            {
                if (fileSystemProvider != null)
                {
                    var fileManagerFile = new FileManagerFile(fileSystemProvider, fullPath);
                    if (fileSystemProvider.Exists(fileManagerFile))
                    {
                        var data = fileSystemProvider.ReadFile(fileManagerFile);
                        if (targetVFS != null)
                        {
                            if (bDisposeTargetVFS)
                                targetVFS.UploadFile(null, fullPath, data);
                            else
                                targetVFS.UploadFile(null, newPath, data);
                        }
                        else
                            File.WriteAllBytes(newPath, data);

                        if (!bCopy)
                            fileSystemProvider.DeleteFile(fileManagerFile);
                    }
                }
                else
                {
                    if (File.Exists(fullPath))
                    {
                        if (targetVFS != null)
                        {
                            targetVFS.UploadFile(null, fullPath.Replace(parent.rootBase, parent.rootBaseDB), 
                                File.ReadAllBytes(fullPath));
                        }
                        else
                            File.Copy(fullPath, newPath, true);
                    }

                    if (!bCopy)
                        RemoveFile(fullPath);
                }
            }
            finally
            {
                if (bDisposeTargetVFS && targetVFS != null && targetVFS is DataSourceFileSystemProvider)
                    (targetVFS as DataSourceFileSystemProvider).Dispose();
            }
        }
        #endregion

        #region Properties

        OPCUAEntityReference _runtimeCycleTimeTag;
        public OPCUAEntityReference CycleTimeTag
        {
            get
            {
                return cycleTimeTag;
            }
            set
            {
                if (cycleTimeTag == value)
                    return;
                cycleTimeTag = value;
                OnPropertyChanged("CycleTimeTag");
                NeedsSave = true;
            }
        }

        OPCUAEntityReference _runtimeCurrentStatusTag;
        public OPCUAEntityReference CurrentStatusTag
        {
            get
            {
                return currentStatusTag;
            }
            set
            {
                if (currentStatusTag == value)
                    return;
                currentStatusTag = value;
                OnPropertyChanged("CurrentStatusTag");
                NeedsSave = true;
            }
        }

        TimeSpan cycleTime;
        [Browsable(false)]
        public TimeSpan CycleTime
        {
            get
            {
                return cycleTime;
            }

            internal set
            {
                if (cycleTime == value)
                    return;
                cycleTime = value;
                // OnPropertyChanged("CycleTime");
            }
        }

        ScriptStatus currentStatus;
        [Browsable(false)]
        public ScriptStatus CurrentStatus
        {
            get
            {
                return currentStatus;
            }

            internal set
            {
                if (currentStatus == value)
                    return;
                currentStatus = value;
                // OnPropertyChanged("CurrentStatus");
            }
        }

        String currentError;
        [Browsable(false)]
        public String CurrentError
        {
            get
            {
                return currentError;
            }

            internal set
            {
                if (currentError == value)
                    return;
                currentError = value;
                // OnPropertyChanged("CurrentError");
            }
        }

        [Browsable(false)]
        public string SessionString
        {
            get
            {
                if (!String.IsNullOrEmpty(sessionName))
                {
                    return sessionName;
                }

                var docParent = Parent;
                if (Parent != null)
                    docParent = DocumentHelper.GetRootParent(Parent, traverse: false);
                return docParent != null ? docParent.Title : Title;
            }
        }

        internal bool _IsInStoppingMode;
        [Browsable(false)]
        public bool IsInStoppingMode
        {
            get { return _IsInStoppingMode; }
        }

        public int StopCommandTimeout
        {
            get { return stopCommandTimeout; }
            set
            {
                if (value == stopCommandTimeout)
                    return;
                stopCommandTimeout = value;
                OnPropertyChanged("StopCommandTimeout");
                NeedsSave = true;
            }
        }

        public int WriteTimeout
        {
            get { return writeTimeout; }
            set
            {
                if (value == writeTimeout)
                    return;
                writeTimeout = value;
                OnPropertyChanged("WriteTimeout");
                NeedsSave = true;
            }
        }

        public ThreadPriority ThreadPriority
        {
            get
            {
                return threadPriority;
            }
            set
            {
                if (threadPriority == value)
                    return;
                threadPriority = value;
                OnPropertyChanged("ThreadPriority");
                NeedsSave = true;
            }
        }
        
        public string SessionName
        {
            get { return sessionName; }
            set
            {
                if (value == sessionName)
                    return;
                sessionName = value;
                NeedsSave = true;
                UpdateSessionSettings();
                OnPropertyChanged("SessionName");
                OnPropertyVisiblityChanged("SessionName");
            }
        }

        public int RemoveDisabledItemAfterSecs
        {
            get { return removeDisabledItemAfterSecs; }
            set
            {
                if (removeDisabledItemAfterSecs == value)
                    return;
                removeDisabledItemAfterSecs = value;
                OnPropertyChanged("RemoveDisabledItemAfterSecs");
                NeedsSave = true;

                UpdateSessionSettings();
            }
        }

        public bool ForceWritingOnServer
        {
            get { return forceWritingOnServer; }
            set
            {
                if (forceWritingOnServer == value)
                    return;
                forceWritingOnServer = value;
                OnPropertyChanged("ForceWritingOnServer");
                NeedsSave = true;
            }
        }

        public int MaxCleanCount
        {
            get { return maxCleanCount; }
            set
            {
                if (maxCleanCount == value)
                    return;
                maxCleanCount = value;
                OnPropertyChanged("MaxCleanCount");
                NeedsSave = true;

                UpdateSessionSettings();
            }
        }

        public bool UseAlwaysSecureConnections
        {
            get { return useAlwaysSecureConnections; }
            set
            {
                if (useAlwaysSecureConnections == value)
                    return;
                useAlwaysSecureConnections = value;
                OnPropertyChanged("UseAlwaysSecureConnections");
                NeedsSave = true;

                UpdateSessionSettings();
            }
        }

        public int FastSamplingInterval
        {
            get
            {
                return fastSamplingInterval;
            }
            set
            {
                if (fastSamplingInterval == value)
                    return;
                fastSamplingInterval = value;
                OnPropertyChanged("FastSamplingInterval");
                NeedsSave = true;

                UpdateSessionSettings();
            }
        }

        public int SlowSamplingInterval
        {
            get
            {
                return slowSamplingInterval;
            }
            set
            {
                if (slowSamplingInterval == value)
                    return;
                slowSamplingInterval = value;
                OnPropertyChanged("SlowSamplingInterval");
                NeedsSave = true;

                UpdateSessionSettings();
            }
        }

        public bool DisableWhenNotUsed
        {
            get { return disableWhenNotUsed; }
            set
            {
                if (disableWhenNotUsed == value)
                    return;
                disableWhenNotUsed = value;
                OnPropertyChanged("DisableWhenNotUsed");
                NeedsSave = true;

                UpdateSessionSettings();
            }
        }

        public int PublishingInterval
        {
            get
            {
                return publishingInterval;
            }
            set
            {
                if (publishingInterval == value)
                    return;
                publishingInterval = value;
                OnPropertyChanged("PublishingInterval");
                NeedsSave = true;

                UpdateSessionSettings();
            }
        }

        [Browsable(false)]
        public String Code
        {
            get
            {
                return sCode; 
            }
            internal set
            {
                if (Code == value)
                    return;
                sCode = value;
                PostInitialize(new StreamingContext());
                OnPropertyChanged("Code");
                NeedsSave = true;
            }
        }

        [Browsable(false)]
        public int SelStart
        {
            get
            {
                return nStartSel;
            }
            internal set
            {
                if (nStartSel == value)
                    return;
                nStartSel = value;
                OnPropertyChanged("SelStart");
            }
        }

        [Browsable(false)]
        public int SelLength
        {
            get
            {
                return nSelLength;
            }
            internal set
            {
                if (nSelLength == value)
                    return;
                nSelLength = value;
                OnPropertyChanged("SelLength");
            }
        }

        [Browsable(false)]
        public int[] Breakpoints
        {
            get
            {
                return breakpoints;
            }
            internal set
            {
                var arraysAreEqual = breakpoints != null && Enumerable.SequenceEqual(value, breakpoints);
                if (arraysAreEqual)
                    return;
                breakpoints = value;
                OnPropertyChanged("Breakpoints");
                NeedsSave = true;
            }
        }

        [Browsable(false)]
        public bool CanReadMacro
        {
            get
            {
                return true;
            }
        }

        public int SleepTime
        {
            get
            {
                return nSleepTime;
            }
            set
            {
                if (nSleepTime == value)
                    return;
                nSleepTime = value;
                OnPropertyChanged("SleepTime");
                NeedsSave = true;
            }
        }

        public int MaxExecutionRestarts
        {
            get
            {
                return nMaxExecutionRestarts;
            }
            set
            {
                if (nMaxExecutionRestarts == value)
                    return;
                nMaxExecutionRestarts = value;
                OnPropertyChanged("MaxExecutionRestarts");
                NeedsSave = true;
            }
        }
        

        public bool EnableLog
        {
            get
            {
                return bEnableLog;
            }
            set
            {
                if (bEnableLog == value)
                    return;
                bEnableLog = value;
                OnPropertyChanged("EnableLog");
                NeedsSave = true;
            }
        }

        public bool EnableSysLog
        {
            get
            {
                return bEnableSysLog;
            }
            set
            {
                if (bEnableSysLog == value)
                    return;
                bEnableSysLog = value;
                OnPropertyChanged("EnableSysLog");
                NeedsSave = true;
            }
        }

        private bool _NeedsSave = false;
        [Browsable(false)]
        public bool NeedsSave
        {
            get { return _NeedsSave; }
            set
            {
                if (_NeedsSave == value)
                    return;

                _NeedsSave = value;
                OnPropertyChanged("NeedsSave");
            }
        }

        [ReadOnly(true)]
        public string FullPath
        {
            get
            {
                if (String.IsNullOrEmpty(Filename))
                {
                    return Path.Combine(Folder, TemporaryFilename);
                }
                else
                {
                    return Path.Combine(Folder, Filename);
                }
            }
            internal set
            {
                Folder = Path.GetDirectoryName(value);
                Filename = Path.GetFileName(value);
            }
        }

        private string _Folder = "";
        [Browsable(false)]
        public string Folder
        {
            get
            {
                return _Folder;
            }
            internal set
            {
                if (_Folder != value)
                {
                    _Folder = value;
                    OnPropertyChanged("Folder");
                    OnPropertyChanged("FullPath");
                }
            }
        }

        private string _Filename;
        [Browsable(false)]
        public string Filename
        {
            get
            {
                if (String.IsNullOrEmpty(_Filename))
                {
                    return TemporaryFilename;
                }
                else
                {
                    return _Filename;
                }
            }
            internal set
            {
                if (_Filename != value)
                {
                    _Filename = value;
                    OnPropertyChanged("Filename");
                    OnPropertyChanged("FullPath");
                }
            }
        }

        string _TemporaryFilename = "";
        [Browsable(false)]
        public string TemporaryFilename
        {
            get
            {
                if (string.IsNullOrEmpty(_TemporaryFilename))
                {
                    string temp = "";
                    /*
                    if (TempFilenameCount == 0)
                    {
                        temp = TempFilenamePreface + ".xaml";
                    }
                    else
                    {
                        temp = TempFilenamePreface + TempFilenameCount + ".xaml";
                    }
                    */
                    _TemporaryFilename = temp;
                    TempFilenameCount++;
                }
                return _TemporaryFilename;
            }
        }

        [Browsable(false)]
        public bool InExecution { get; internal set; }
       
        #endregion

        #region Events
        public event EventHandler<GetTagListEventArgs> GetTagList;
        public virtual void OnGetTagList(GetTagListEventArgs ea)
        {
            if (GetTagList != null)
                GetTagList(null/*this*/, ea);
        }

        public event EventHandler<GetPrototypeListEventArgs> GetPrototypeList;
        public virtual void OnGetPrototypeList(GetPrototypeListEventArgs ea)
        {
            if (GetPrototypeList != null)
                GetPrototypeList(null/*this*/, ea);
        }

        public event EventHandler<GetTagEntityReference> GetTagEntityReference;
        public virtual void OnGetTagEntityReference(GetTagEntityReference ea)
        {
            if (GetTagEntityReference != null)
                GetTagEntityReference(null/*this*/, ea);
        }

        public event EventHandler<VariableChangedEventArgs> VariableChanged;
        bool bPendingVariableChangedEvent;
        public virtual void OnVariableChanged(VariableChangedEventArgs ea)
        {
            if (VariableChanged != null)
            {
                try
                {
                    bPendingVariableChangedEvent = true;
                    VariableChanged(null/*this*/, ea);
                }
                finally
                {
                    bPendingVariableChangedEvent = false;
                }
            }
        }
        #endregion

        #region ICloneable Members

        ScriptDocument(ScriptDocument template)
        {
            if (template == null)
                return;

            Code = template.Code;
        }

        public object Clone()
        {
            return new ScriptDocument(this);
        }

        #endregion

        #region Validations

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
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

        #region IDocument Members

        public event EventHandler Disposing;
        virtual public void OnDisposing(Object sender)
        {
            EventHandler temp = Disposing;
            if (temp != null)
                temp(sender, EventArgs.Empty);
        }

        UserControl activeView;
        [Browsable(false)]
        public UserControl ActiveView
        {
            get 
            { 
                return activeView; 
            }
            set 
            { 
                activeView = value; 
            }
        }

        [Browsable(false)]
        public UserControl View
        {
            get
            {
                return activeView;
            }
        }

        IDocument parent;
        [Browsable(false)]
        public IDocument Parent
        {
            get 
            { 
                return parent; 
            }
            set
            {
                parent = value;
            }
        }

        [Browsable(false)]
        public IList<IDocument> Childs
        {
            get
            {
                return null;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String ProjectType
        {
            get
            {
                if (Parent != null)
                    return Parent.ProjectType;
                return String.Empty;
            }
        }

        [Browsable(false)]
        public String Theme
        {
            get
            {
                if (Parent != null)
                    return Parent.Theme;
                return String.Empty;
            }
        }

        public Uri GetSpecialFolder(SpecialFolders specialFolder)
        {
            if (Parent != null)
                return Parent.GetSpecialFolder(specialFolder);
            return null;
        }

        public Uri MakeAbosoluteUri(Uri relative)
        {
            if (relative == null)
                return null;

            if (Parent != null)
                return Parent.MakeAbosoluteUri(relative);
            if (relative.IsAbsoluteUri)
                return relative;

            return new Uri(new Uri(Path.GetDirectoryName(FullPath) + "\\"), relative);
        }

        public IDocument UpdateParentFromUri(Uri relative)
        {
            if (Parent != null)
                return Parent.UpdateParentFromUri(relative);
            return this;
        }

        public Uri MakeRelativeUri(Uri absolute)
        {
            if (absolute == null)
                return null;

            if (Parent != null)
                return Parent.MakeRelativeUri(absolute);
            if (!absolute.IsAbsoluteUri)
                return absolute;

            return new Uri(Path.GetDirectoryName(FullPath) + "\\").MakeRelativeUri(absolute);
        }

        [Browsable(false)]
        public FileSystemProviderBase fileSystemProviderBase
        {
            get
            {
                if (Parent != null)
                    return Parent.fileSystemProviderBase;
                return null;
            }
        }

        [Browsable(false)]
        public String rootBase
        {
            get
            {
                if (Parent != null)
                    return Parent.rootBase;
                return Path.GetDirectoryName(FullPath);
            }
        }

        [Browsable(false)]
        public String rootBaseDB
        {
            get
            {
                if (Parent != null)
                    return Parent.rootBaseDB;
                return String.Empty;
            }
        }

        [Browsable(false)]
        public bool Protected
        {
            get
            {
                if (Parent != null)
                    return Parent.Protected;
                return false;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid Id
        {
            get
            {
                if (Parent != null)
                    return Parent.Id;
                return Guid.Empty;
            }
        }

        public String Title
        {
            get
            {
                return Path.GetFileNameWithoutExtension(FullPath);
            }
        }

        [Browsable(false)]
        public String FilePath
        {
            get
            {
                return FullPath;
            }
        }

        [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                return String.IsNullOrEmpty(Code);
            }
        }

        [Browsable(false)]
        public bool IsRoot
        {
            get
            {
                return false;
            }
        }

        public Object GetService(Type type)
        {
            if (Parent != null)
                return Parent.GetService(type);
            return null;
        }

        #endregion

        #region IEntityReference Members

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ImageSource CollapsedImageSource
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ImageSource ExpandedImageSource
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public System.Windows.Controls.ContextMenu contextMenu
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object Tooltip
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object ContainedObject
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object EntityParent
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string TypeDefinitionString
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region INotifyPropertyVisibilityChanged Members

        /// <summary>
        /// Gets the visibility state for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The property name that you want konw the current visibility state.</param>
        /// <returns></returns>
        bool INotifyPropertyVisibilityChanged.this[string propertyName]
        {
            get
            {
                if (propertyName == "RemoveDisabledItemAfterSecs" ||
                    propertyName == "MaxCleanCount" ||
                    propertyName == "UseAlwaysSecureConnections" ||
                    propertyName == "SlowSamplingInterval" ||
                    propertyName == "DisableWhenNotUsed" ||
                    propertyName == "PublishingInterval" ||
                    propertyName == "FastSamplingInterval")
                {
                    return !String.IsNullOrEmpty(SessionName);
                }

                return true;
            }
        }

        /// <summary>
        /// Raised when a property visibility state on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyVisiblityChanged;

        /// <summary>
        /// Raises this object's PropertyVisiblityChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has changed his value and has triggered the change of visibility.</param>
        void OnPropertyVisiblityChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyVisiblityChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion

        #region IDisposable Members

        internal bool IsDisposed
        {
            get
            {
                return bDisposed;
            }
        }

        bool bDisposed;
        protected override void OnDispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            OnDisposing(this);

            base.OnDispose();

            lock (lockObject)
            {
                if (variableValues != null)
                {
                    variableValues.ForEach(value =>
                        {
                            value.Dispose();
                        });
                    variableValues.Clear();
                    variableValues = null;
                }

                if (variableValuesRuntime != null)
                {
                    var list = variableValuesRuntime.ToList();
                    variableValuesRuntime.Clear();
                    mapvariableValuesRuntime.Clear();
                    variableValuesRuntime = null;
                    list.ForEach(value =>
                    {
                        value.Dispose();
                    });
                }
            }
        }

        #endregion
    }
}
