using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Windows;
using System.ComponentModel;
using Utilities.WPF;
using System.Windows.Controls;

namespace Utilities
{
    public static class DispatcherExtensions
    {
        private static Action EmptyDelegate = delegate() { };
        private static Dictionary<FrameworkElement, List<DispatcherOperation>> pendingOperations = new Dictionary<FrameworkElement, List<DispatcherOperation>>();

        private static Action<DispatcherOperation, DispatcherPriority, FrameworkElement> PendingDelegate = 
            delegate (DispatcherOperation dp, DispatcherPriority priority, FrameworkElement element) 
            {
                if (dp.Status != DispatcherOperationStatus.Pending)
                    return;

                if (element.IsVisible)
                {
                    dp.Priority = priority;
                }
                else
                {
                    var propDesc = DependencyPropertyDescriptor.FromProperty(UIElement.IsVisibleProperty, typeof(UIElement));
                    var notifier = new WPFUtilities.PropertyChangeNotifier(element, propDesc.Name);
                    notifier.ValueChanged += (s, e) =>
                    {
                        if (element.IsVisible)
                        {
                            notifier.Dispose();
                            lock (pendingOperations)
                                pendingOperations.Remove(element);
                            //element.Unloaded -= element_Unloaded;
                            dp.Priority = priority;
                        }
                    };
                    dp.Completed += (s, e) =>
                    {
                        notifier.Dispose();
                    };
                    dp.Aborted += (s, e) =>
                    {
                        dp.Dispatcher.BeginInvokeIfRequired(() =>
                        {
                            notifier.Dispose();
                            lock (pendingOperations)
                            {
                                if (pendingOperations.ContainsKey(element))
                                {
                                    pendingOperations[element].Remove(dp);
                                    if (pendingOperations[element].Count == 0)
                                        pendingOperations.Remove(element);
                                }
                            }
                        });
                    };
                    lock (pendingOperations)
                    {
                        if (!pendingOperations.ContainsKey(element))
                        {
                            pendingOperations.Add(element, new List<DispatcherOperation>());
                            //element.Unloaded += element_Unloaded;
                        }
                        pendingOperations[element].Add(dp);
                    }
                }
            };

        /*
        private static void element_Unloaded(object sender, RoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;

            List<DispatcherOperation> operations = null;
            lock (pendingOperations)
            {
                if (pendingOperations.ContainsKey(element))
                {
                    operations = pendingOperations[element].ToList();
                    pendingOperations.Remove(element);
                }
            }

            if (operations != null)
            {
                foreach (var dp in operations)
                {
                    if (dp.Status != DispatcherOperationStatus.Aborted &&
                        dp.Status != DispatcherOperationStatus.Completed &&
                        dp.Priority == DispatcherPriority.Inactive)
                        dp.Abort();
                }
            }
        }
        */

        public static void AbortPendingOperations(this Panel panel)
        {
            var operations = new List<DispatcherOperation>();
            lock (pendingOperations)
            {
                if (pendingOperations.Count == 0)
                    return;

                var elements = (from c in panel.GetChildrenOfType<FrameworkElement>() select c).ToList();
                foreach (var element in elements)
                {
                    if (pendingOperations.ContainsKey(element))
                    {
                        operations.AddRange(pendingOperations[element]);
                        pendingOperations.Remove(element);
                    }
                }
            }

            foreach (var dp in operations)
            {
                if (dp.Status != DispatcherOperationStatus.Aborted &&
                    dp.Status != DispatcherOperationStatus.Completed &&
                    dp.Priority == DispatcherPriority.Inactive)
                    dp.Abort();
            }
        }

        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }

        /// <summary>
        /// A simple threading extension method, to invoke a delegate
        /// on the correct thread if it is not currently on the correct thread
        /// which can be used with DispatcherObject types.
        /// </summary>
        /// <param name="dispatcher">The Dispatcher object on which to 
        /// perform the Invoke</param>
        /// <param name="action">The delegate to run</param>
        /// <param name="priority">The DispatcherPriority for the invoke.</param>
        public static void InvokeIfRequired(this Dispatcher dispatcher,
            Action action, DispatcherPriority priority)
        {
            if (!dispatcher.CheckAccess())
            {
                dispatcher.Invoke(priority, action);
            }
            else
            {
                action();
            }
        }
        public static void BeginInvokeIfRequired(this Dispatcher dispatcher,
            Action action, DispatcherPriority priority)
        {
            dispatcher.BeginInvoke(priority, action);
        }

        /// <summary>
        /// A simple threading extension method, to invoke a delegate
        /// on the correct thread if it is not currently on the correct thread
        /// which can be used with DispatcherObject types.
        /// </summary>
        /// <param name="dispatcher">The Dispatcher object on which to 
        /// perform the Invoke</param>
        /// <param name="action">The delegate to run</param>
        public static void InvokeIfRequired(this Dispatcher dispatcher, Action action)
        {
            if (!dispatcher.CheckAccess())
            {
                dispatcher.Invoke(DispatcherPriority.Normal, action);
            }
            else
            {
                action();
            }
        }
        public static void BeginInvokeIfRequired(this Dispatcher dispatcher, Action action)
        {
            if (!dispatcher.CheckAccess())
            {
                dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// A simple threading extension method, to invoke a delegate
        /// on the correct thread if it is not currently on the correct thread
        /// which can be used with DispatcherObject types.
        /// </summary>
        /// <param name="dispatcher">The Dispatcher object on which to 
        /// perform the Invoke</param>
        /// <param name="action">The delegate to run</param>
        public static void InvokeInBackgroundIfRequired(this Dispatcher dispatcher, Action action)
        {
            if (!dispatcher.CheckAccess())
            {
                dispatcher.Invoke(DispatcherPriority.Background, action);
            }
            else
            {
                action();
            }
        }
        public static void BeginInvokeInBackgroundIfRequired(this Dispatcher dispatcher, Action action)
        {
            if (!dispatcher.CheckAccess())
            {
                dispatcher.BeginInvoke(DispatcherPriority.Background, action);
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// A simple threading extension method, to invoke a delegate
        /// on the correct thread asynchronously if it is not currently on the correct thread
        /// which can be used with DispatcherObject types.
        /// </summary>
        /// <param name="dispatcher">The Dispatcher object on which to 
        /// perform the Invoke</param>
        /// <param name="action">The delegate to run</param>
        public static void InvokeAsynchronouslyInBackground(this Dispatcher dispatcher, Action action)
        {
            dispatcher.Invoke(DispatcherPriority.Background, action);
        }
        public static DispatcherOperation BeginInvokeAsynchronously(this Dispatcher dispatcher, FrameworkElement element, Action action)
        {
            var dp = dispatcher.BeginInvokeAsynchronously(action);
            if (element != null && !element.IsVisible)
            {
                dp.Priority = DispatcherPriority.Inactive;
                element.Dispatcher.BeginInvokeIfRequired(() => PendingDelegate(dp, DispatcherPriority.Normal, element));
            }
            return dp;
        }
        public static DispatcherOperation BeginInvokeAsynchronously(this Dispatcher dispatcher, Action action)
        {
            return dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
        }
        public static DispatcherOperation BeginInvokeAsynchronouslyInBackground(this Dispatcher dispatcher, FrameworkElement element, Action action)
        {
            var dp = dispatcher.BeginInvokeAsynchronouslyInBackground(action);
            if (element != null && !element.IsVisible)
            {
                dp.Priority = DispatcherPriority.Inactive;
                element.Dispatcher.BeginInvokeIfRequired(() => PendingDelegate(dp, DispatcherPriority.Background, element));
            }
            return dp;
        }
        public static DispatcherOperation BeginInvokeAsynchronouslyInBackground(this Dispatcher dispatcher, Action action)
        {
            return dispatcher.BeginInvoke(DispatcherPriority.Background, action);
        }
        public static DispatcherOperation BeginInvokeAsynchronouslyInInput(this Dispatcher dispatcher, FrameworkElement element, Action action)
        {
            var dp = dispatcher.BeginInvokeAsynchronouslyInInput(action);
            if (element != null && !element.IsVisible)
            {
                dp.Priority = DispatcherPriority.Inactive;
                element.Dispatcher.BeginInvokeIfRequired(() => PendingDelegate(dp, DispatcherPriority.Input, element));
            }
            return dp;
        }
        public static DispatcherOperation BeginInvokeAsynchronouslyInInput(this Dispatcher dispatcher, Action action)
        {
            return dispatcher.BeginInvoke(DispatcherPriority.Input, action);
        }
        public static DispatcherOperation BeginInvokeAsynchronouslyInApplicationIdle(this Dispatcher dispatcher, FrameworkElement element, Action action)
        {
            var dp = dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(action);
            if (element != null && !element.IsVisible)
            {
                dp.Priority = DispatcherPriority.Inactive;
                element.Dispatcher.BeginInvokeIfRequired(() => PendingDelegate(dp, DispatcherPriority.ApplicationIdle, element));
            }
            return dp;
        }
        public static DispatcherOperation BeginInvokeAsynchronouslyInApplicationIdle(this Dispatcher dispatcher, Action action)
        {
            return dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, action);
        }
        public static DispatcherOperation BeginInvokeAsynchronouslyInRender(this Dispatcher dispatcher, FrameworkElement element, Action action)
        {
            var dp = dispatcher.BeginInvokeAsynchronouslyInRender(action);
            if (element != null && !element.IsVisible)
            {
                dp.Priority = DispatcherPriority.Inactive;
                element.Dispatcher.BeginInvokeIfRequired(() => PendingDelegate(dp, DispatcherPriority.Render, element));
            }
            return dp;
        }
        public static DispatcherOperation BeginInvokeAsynchronouslyInRender(this Dispatcher dispatcher, Action action)
        {
            return dispatcher.BeginInvoke(DispatcherPriority.Render, action);
        }
    }
}
