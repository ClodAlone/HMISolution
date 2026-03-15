using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentManager.ComponentService;
using OPCUAViewModel;
using UFInterfaces;
using ViewModelLib;
using Utilities;

namespace ReportParameters
{
    public class ParameterCollection : List<Parameter>, IDisposable
    {
        #region Declarations
        Object lockObject = new Object();
        Dictionary<String, ReportParameters.Parameter> mapResolvedVariables;
        List<PropertyObserver<OPCUAEntityReference>> listPropertyObserverEntityReferenceResolveVariable;
        List<PropertyObserver<MonitoredItemViewModel>> listPropertyObserverMonitoredItemResolveVariable;
        List<String> listWaitingFirstParameterValue;

        bool bExecuted;
        #endregion 

        #region Constructors
        // Summary:
        //     Initializes a new instance of the System.Collections.Generic.List<T> class
        //     that is empty and has the default initial capacity.
        public ParameterCollection() { }

        //
        // Summary:
        //     Initializes a new instance of the System.Collections.Generic.List<T> class
        //     that contains elements copied from the specified collection and has sufficient
        //     capacity to accommodate the number of elements copied.
        //
        // Parameters:
        //   collection:
        //     The collection whose elements are copied to the new list.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     collection is null.
        public ParameterCollection(IEnumerable<Parameter> collection) : base(collection) { }

        //
        // Summary:
        //     Initializes a new instance of the System.Collections.Generic.List<T> class
        //     that is empty and has the specified initial capacity.
        //
        // Parameters:
        //   capacity:
        //     The number of elements that the new list can initially store.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     capacity is less than 0.

        public ParameterCollection(int capacity) : base(capacity) { }
        #endregion

        #region Events
        public event EventHandler Ready;
        void OnReady()
        {
            Ready?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        //#region Public Properties
        ///// <summary>
        ///// Session name to use for connecting the OPCUAEntityReference.
        ///// </summary>
        //public string sessionString;
        //public string SessionString
        //{
        //    get 
        //    {
        //        return sessionString;
        //    }
        //    set 
        //    {
        //        if (sessionString == value)
        //            return;

        //        if (!String.IsNullOrEmpty(sessionString))
        //            TerminateExecution();

        //        sessionString = value;
        //    }
        //}

        ///// <summary>
        ///// Entity to use for connecting the OPCUAEntityReference.
        ///// </summary>
        //IEntityReference entityReference;
        //public IEntityReference EntityReference
        //{
        //    get 
        //    {
        //        return entityReference;
        //    }
        //    set 
        //    {
        //        if (entityReference == value)
        //            return;

        //        if (entityReference != null)
        //            TerminateExecution();

        //        entityReference = value;
        //    }
        //}
        //#endregion

        #region Public Methods
        /// <summary>
        /// Refresh all the OPCUAEntityReference objects linked to the list.
        /// Note: 
        /// A valid EntityReference and SessionString must be set before call PrepareExecution method
        /// if TerminateExecution has just been called
        /// </summary>
        void RefreshAllEntityReferences()
        {
            this.ForEach(p =>
            {
                if(p.TagRef != null)
                {
                    var xml = p.TagRef.ToXml();
                    p.TagRef = xml.FromXml<OPCUAEntityReference>();
                }
            });
        }

        /// <summary>
        /// Return 'true' if there is at least one relative entity reference.
        /// </summary>
        /// <returns></returns>
        public bool ContainsRelativeEntityReferences()
        {
            return (from c in this 
                    where c.TagRef != null && c.TagRef.IsRelative && !c.TagRef.ResolvedItem
                    select c).ToList().Count > 0;
        }

        /// <summary>
        /// Prepare the execution of the OPCUAEntityReference object linked to the list.
        /// Note: 
        /// A valid EntityReference and SessionString must be set before call this method.
        /// </summary>
        public void PrepareExecution(string SessionString, IEntityReference EntityReference, IDocument parent)
        {
            if (bExecuted)
                return;
            bExecuted = true;

            if (String.IsNullOrEmpty(SessionString))
                throw new InvalidOperationException("SessionString cannot be null or empty.");

            if (EntityReference == null)
                throw new InvalidOperationException("EntityReference cannot be null.");
            
            RefreshAllEntityReferences();
            isReady = false;

            var parameters = (from c in this where c.TagRef != null && (
#if !NET_STANDARD
                              c.TagRef.IsValid &&
#endif
                              !c.TagRef.IsRelative || (c.TagRef.IsRelative && c.TagRef.ResolvedItem)) select c).ToList();
            if (parameters.Count > 0)
            {
                if (mapResolvedVariables == null)
                    mapResolvedVariables = new Dictionary<String, ReportParameters.Parameter>();
                if (listWaitingFirstParameterValue == null)
                    listWaitingFirstParameterValue = new List<string>();
                parameters.ForEach((par) =>
                {
                    mapResolvedVariables.Add(par.Name, par);
                    listWaitingFirstParameterValue.Add(par.Name);
                });
            }

            if (mapResolvedVariables != null && mapResolvedVariables.Count > 0)
            {
                foreach (var key in mapResolvedVariables.Keys)
                {
                    var observer = new PropertyObserver<OPCUAEntityReference>(mapResolvedVariables[key].TagRef);
                    lock (lockObject)
                    {
                        if (listPropertyObserverEntityReferenceResolveVariable == null)
                            listPropertyObserverEntityReferenceResolveVariable = new List<PropertyObserver<OPCUAEntityReference>>();
                        listPropertyObserverEntityReferenceResolveVariable.Add(observer);
                    }
                    observer.RegisterHandler(n => n.MonitoredItemViewModel, n =>
                    {
                        observer.UnregisterHandler(p => p.MonitoredItemViewModel);

                        var observerMonitoredModel = new PropertyObserver<MonitoredItemViewModel>(n.MonitoredItemViewModel);
                        lock (lockObject)
                        {
                            listPropertyObserverEntityReferenceResolveVariable.Remove(observer);
                            observer.Dispose();
                            if (listPropertyObserverMonitoredItemResolveVariable == null)
                                listPropertyObserverMonitoredItemResolveVariable = new List<PropertyObserver<MonitoredItemViewModel>>();
                            listPropertyObserverMonitoredItemResolveVariable.Add(observerMonitoredModel);
                        }
                        observerMonitoredModel.RegisterHandler(m => m.DataValue, m =>
                        {
                            if (m.DataValue != null && Opc.Ua.StatusCode.IsGood(m.DataValue.StatusCode))
                            {
                                mapResolvedVariables[key].Value = m.DataValue.Value;
                                lock (lockObject)
                                {
                                    if (listWaitingFirstParameterValue.Contains(key))
                                    {
                                        listWaitingFirstParameterValue.Remove(key);
                                        IsReady = listWaitingFirstParameterValue.Count == 0;
                                    }
                                }
                            }
                        });

                        if (n.MonitoredItemViewModel != null && n.MonitoredItemViewModel.DataValue != null &&
                            Opc.Ua.StatusCode.IsGood(n.MonitoredItemViewModel.DataValue.StatusCode))
                        {
                            mapResolvedVariables[key].Value = n.MonitoredItemViewModel.DataValue.Value;
                            lock (lockObject)
                            {
                                if (listWaitingFirstParameterValue.Contains(key))
                                {
                                    listWaitingFirstParameterValue.Remove(key);
                                    IsReady = listWaitingFirstParameterValue.Count == 0;
                                }
                            }
                        }
                    });

                    mapResolvedVariables[key].TagRef.Resolve(SessionString, parent);
                    mapResolvedVariables[key].TagRef.SetInUse(EntityReference, true);
                }
            }
            else
                IsReady = true;
        }

        /// <summary>
        /// Terminate the execution of the OPCUAEntityReference object linked to the list.
        /// Note: 
        /// A valid EntityReference and SessionString must be set before to call this function.
        /// This method is called when the this reference object is disposed.
        /// </summary>
        public void TerminateExecution(IEntityReference EntityReference)
        {
            if (!bExecuted)
                return;
            bExecuted = false;

            if (EntityReference == null)
                throw new InvalidOperationException("EntityReference cannot be null.");

            lock (lockObject)
            {
                if (listPropertyObserverMonitoredItemResolveVariable != null)
                {
                    listPropertyObserverMonitoredItemResolveVariable.ToList().ForEach(item =>
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
                    foreach (var key in mapResolvedVariables.Keys)
                        mapResolvedVariables[key].TagRef.SetInUse(EntityReference, false);
                    mapResolvedVariables.Clear();
                }

                if (listWaitingFirstParameterValue != null)
                    listWaitingFirstParameterValue.Clear();
            }
        }

        bool isReady;
        /// <summary>
        /// Check if all parameter values have been valorized.
        /// </summary>
        /// <returns></returns>
        public bool IsReady
        {
            get
            {
                return isReady;
            }
            private set
            {
                if (isReady == value)
                    return;

                isReady = value;
                if (isReady)
                    System.Threading.ThreadPool.QueueUserWorkItem((o) => OnReady());
            }
        }

        /// <summary>
        /// Add a new value in the Collection with the default values.
        /// </summary>
        /// <param name="basename"></param>
        public void AddNewDefault(string basename = null)
        {
            if (basename == null)
                basename = Properties.Resources.ReportNewParameterName;

            long count = 0;
            do
            {
                basename = string.Format("{0}{1}", Properties.Resources.ReportNewParameterName, ++count);
            } while ((from c in this where c.Name == basename select c.Name).ToList().Count > 0);

            this.Add(new ReportParameters.Parameter() { Name = basename, Type = ReportParameters.ParameterType.String });
        }

        public List<Parameter> GetParameterListStartingWith(string prefix)
        {
            return (from c in this.AsParallel() where c.Name.StartsWith(prefix) select c).ToList();
        }

        //
        // Summary:
        //     Determines whether the Collection
        //     contains an element with the specified key name.
        //
        // Parameters:
        //   key:
        //     The key name to locate in the Collection
        //
        // Returns:
        //     true if the Collection contains
        //     an element with the key; otherwise, false.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     key is null.
        public bool ContainsKey(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return (from c in this where c.Name == key select c.Name).ToList().Count > 0;
        }

        // Summary:
        //     Gets the element with the specified key name.
        //
        // Parameters:
        //   key:
        //     The key name of the element to get.
        //
        // Returns:
        //     The element with the specified key.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     key is null.
        //
        //   System.Collections.Generic.KeyNotFoundException:
        //     The property is retrieved and key is not found.
        public Parameter this[String key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException("key");

                return (from c in this where c.Name == key select c).First();
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Dispose this class references.
        /// </summary>
        public void Dispose()
        {
            foreach (var item in this)
                item.Dispose();
        }

        #endregion
    }
}
