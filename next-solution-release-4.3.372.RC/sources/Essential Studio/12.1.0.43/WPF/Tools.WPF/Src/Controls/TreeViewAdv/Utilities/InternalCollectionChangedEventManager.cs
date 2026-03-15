// <copyright file="InternalCollectionChangedEventManager.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

#region file using

using System;
using System.Collections.Specialized;
using System.Windows;

#endregion file using

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents InternalCollectionChangedEventManager class
    /// </summary>
    internal class InternalCollectionChangedEventManager : WeakEventManager
    {
        #region Properties

        /// <summary>
        /// Gets the current manager.
        /// </summary>
        /// <value>The current manager.</value>
        private static InternalCollectionChangedEventManager CurrentManager
        {
            get
            {
                Type managerType = typeof(InternalCollectionChangedEventManager);
                InternalCollectionChangedEventManager currentManager = (InternalCollectionChangedEventManager)WeakEventManager.GetCurrentManager(managerType);

                if (currentManager == null)
                {
                    currentManager = new InternalCollectionChangedEventManager();
                    WeakEventManager.SetCurrentManager(managerType, currentManager);
                }

                return currentManager;
            }
        }

        #endregion Properties

        #region Initialization

        /// <summary>
        /// Prevents a default instance of the <see cref="InternalCollectionChangedEventManager"/> class from being created.
        /// </summary>
        private InternalCollectionChangedEventManager()
        {
        }

        #endregion Initialization

        #region Implemntation

        /// <summary>
        /// Adds the listener.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="listener">The listener.</param>
        public static void AddListener(TreeViewColumnCollection source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedAddListener(source, listener);
        }

        /// <summary>
        /// Called when [collection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            base.DeliverEvent(sender, args);
        }

        /// <summary>
        /// Removes the listener.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="listener">The listener.</param>
        public static void RemoveListener(TreeViewColumnCollection source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedRemoveListener(source, listener);
        }

        /// <summary>
        /// When overridden in a derived class, starts listening for the event being managed. After <see cref="M:System.Windows.WeakEventManager.StartListening(System.Object)"/>  is first called, the manager should be in the state of calling <see cref="M:System.Windows.WeakEventManager.DeliverEvent(System.Object,System.EventArgs)"/> or <see cref="M:System.Windows.WeakEventManager.DeliverEventToList(System.Object,System.EventArgs,System.Windows.WeakEventManager.ListenerList)"/> whenever the relevant event from the provided source is handled.
        /// </summary>
        /// <param name="source">The source to begin listening on.</param>
        protected override void StartListening(object source)
        {
            TreeViewColumnCollection columns = (TreeViewColumnCollection)source;
            columns.InternalCollectionChanged += new NotifyCollectionChangedEventHandler(this.OnCollectionChanged);
        }

        /// <summary>
        /// When overridden in a derived class, stops listening on the provided source for the event being managed.
        /// </summary>
        /// <param name="source">The source to stop listening on.</param>
        protected override void StopListening(object source)
        {
            TreeViewColumnCollection columns = (TreeViewColumnCollection)source;
            columns.InternalCollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnCollectionChanged);
        }

        #endregion Implemntation
    }
}