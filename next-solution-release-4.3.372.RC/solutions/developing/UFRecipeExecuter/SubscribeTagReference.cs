using OPCUAViewModel;
using System;
using System.Threading;
using UFRecipeExecutionContext;
using ViewModelLib;

namespace UFRecipeExecuter
{
    internal class SubscribeTagReference : IDisposable
    {
        #region Declarations

        PropertyObserver<OPCUAEntityReference> observer;
        PropertyObserver<MonitoredItemViewModel> observerMonitoredModel;
        PropertyObserver<SessionViewModel> observerSessionModel;

        readonly UFRecipeExecuter recipeExecuter;
        readonly OPCUAEntityReference tagReference;
        readonly Action<MonitoredItemViewModel> action;
        readonly Action<MonitoredItemViewModel, RecipeExecutionContext> actionEx;
        readonly RecipeExecutionContext context;
        readonly bool monitor;
        readonly bool invokeActionOnSessionConnected;

        long subscriptionCounter;
        object lockObject = new object();
        #endregion

        #region Constructors
        public SubscribeTagReference(UFRecipeExecuter recipeExecuter, OPCUAEntityReference tagReference, Action<MonitoredItemViewModel> action, bool monitor)
        {
            this.recipeExecuter = recipeExecuter;
            this.tagReference = tagReference;
            this.action = action;
            this.monitor = monitor;
            this.invokeActionOnSessionConnected = !monitor;
        }

        public SubscribeTagReference(UFRecipeExecuter recipeExecuter, OPCUAEntityReference tagReference, Action<MonitoredItemViewModel, RecipeExecutionContext> action, RecipeExecutionContext context)
        {
            this.recipeExecuter = recipeExecuter;
            this.tagReference = tagReference;
            this.actionEx = action;
            this.context = context;
        }
        #endregion

        #region Properties
        public Opc.Ua.StatusCode Quality
        {
            get
            {
                var monitoredItemViewModel = tagReference.MonitoredItemViewModel;
                if (monitoredItemViewModel == null || monitoredItemViewModel.DataValue == null)
                    return Opc.Ua.StatusCodes.BadWaitingForInitialData;

                return monitoredItemViewModel.DataValue.StatusCode;
            }
        }
        #endregion

        #region Methods
        public void Subscribe()
        {
            if (Interlocked.Increment(ref subscriptionCounter) != 1)
                return;

            tagReference.Resolve(recipeExecuter.currentSessionName, recipeExecuter.RecipeDocument);

            if (tagReference.MonitoredItemViewModel != null)
            {
                tagReference.SetInUse(recipeExecuter.RecipeDocument.RecipeEntity, true);
                if (IsGoodDataValue(tagReference.MonitoredItemViewModel.DataValue))
                {
                    if (action != null)
                        action.Invoke(tagReference.MonitoredItemViewModel);
                    else if (actionEx != null)
                        actionEx.Invoke(tagReference.MonitoredItemViewModel, context);
                }
            }

            lock (lockObject)
            {
                observer = new PropertyObserver<OPCUAEntityReference>(tagReference);
                observer.RegisterHandler(n => n.MonitoredItemViewModel, n =>
                {
                    lock (lockObject)
                    {
                        if (observer != null)
                        {
                            if (observerMonitoredModel != null)
                                observerMonitoredModel.Dispose();
                            observerMonitoredModel = new PropertyObserver<MonitoredItemViewModel>(n.MonitoredItemViewModel);
                            if (monitor)
                                observerMonitoredModel.RegisterHandler(m => m.DataValue, action);
                            else
                            {
                                observer.UnregisterHandler(p => p.MonitoredItemViewModel);
                                observer.Dispose();

                                if (invokeActionOnSessionConnected && action != null)
                                {
                                    var subscription = tagReference.MonitoredItemViewModel.GetSubscriptionViewModelParent();
                                    if (subscription != null)
                                    {
                                        var session = subscription.GetSessionViewModelParent();
                                        if (session != null)
                                        {
                                            if(observerSessionModel != null)
                                                observerSessionModel.Dispose();
                                            observerSessionModel = new PropertyObserver<SessionViewModel>(session);
                                            observerSessionModel.RegisterHandler(s => s.Connected, s =>
                                            {
                                                if (s.Connected)
                                                    action.Invoke(tagReference.MonitoredItemViewModel);
                                            });
                                        }
                                    }
                                }

                                observerMonitoredModel.RegisterHandler(m => m.DataValue, m =>
                                {
                                    if (IsGoodDataValue(m.DataValue))
                                    {
                                        lock (lockObject)
                                        {
                                            if (observerMonitoredModel != null)
                                            {
                                                observerMonitoredModel.UnregisterHandler(q => q.DataValue);
                                                observerMonitoredModel.Dispose();
                                            }
                                        }

                                        if (action != null)
                                            action.Invoke(m);
                                        else if (actionEx != null)
                                            actionEx.Invoke(m, context);
                                    }
                                });
                            }
                        }
                    }

                    if (n.MonitoredItemViewModel != null && IsGoodDataValue(n.MonitoredItemViewModel.DataValue))
                    {
                        if (action != null)
                            action.Invoke(n.MonitoredItemViewModel);
                        else if (actionEx != null)
                            actionEx.Invoke(n.MonitoredItemViewModel, context);
                    }
                });
            }

            tagReference.SetInUse(recipeExecuter.RecipeDocument.RecipeEntity, true);
        }

        public void UnSubscribe()
        {
            if (Interlocked.Decrement(ref subscriptionCounter) != 0)
                return;
            
            tagReference.SetInUse(recipeExecuter.RecipeDocument.RecipeEntity, false);

            lock (lockObject)
            {
                if (observerSessionModel != null)
                {
                    observerSessionModel.UnregisterHandler(m => m.Connected);
                    observerSessionModel.Dispose();
                    observerSessionModel = null;
                }

                if (observerMonitoredModel != null)
                {
                    observerMonitoredModel.UnregisterHandler(m => m.DataValue);
                    observerMonitoredModel.Dispose();
                    observerMonitoredModel = null;
                }

                if (observer != null)
                {
                    observer.UnregisterHandler(p => p.MonitoredItemViewModel);
                    observer.Dispose();
                    observer = null;
                }
            }
        }

        bool IsGoodDataValue(Opc.Ua.DataValue dataValue)
        {
            return dataValue != null && (Opc.Ua.StatusCode.IsGood(dataValue.StatusCode) || dataValue.StatusCode == Opc.Ua.StatusCodes.UncertainLastUsableValue);
        }
        #endregion

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            UnSubscribe();
        }
        #endregion
    }
}
