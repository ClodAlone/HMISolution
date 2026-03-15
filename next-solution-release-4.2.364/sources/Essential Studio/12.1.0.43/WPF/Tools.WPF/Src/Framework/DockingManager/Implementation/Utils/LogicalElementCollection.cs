// <copyright file="LogicalElementCollection.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Syncfusion.Windows.Tools.Controls.Resources;
using System.ComponentModel;
using Syncfusion.Windows.Shared;
using System.Reflection;
using System.Windows.Data;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Specify the LogicalElementCollection class.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class LogicalElementCollection : IList
    {
        #region Constants
        /// <summary>
        /// Prefix of added name.
        /// </summary>
        private const string AddedName = "AddedName";

        /// <summary>
        /// Contains floating menu item name.
        /// </summary>
        private const string FLOATING = "Floating";

        /// <summary>
        /// Contains dock able menu item name.
        /// </summary>
        private const string DOCKABLE = "Dockable";

        /// <summary>
        /// Contains document menu item name.
        /// </summary>
        private const string DOCUMENT = "Document";
        #endregion

        #region Readonly private member
        /// <summary>
        /// Logical parent.
        /// </summary>
        private readonly DockingManager m_LogicalParent;

        /// <summary>
        /// Represents children.
        /// </summary>
        private readonly List<FrameworkElement> m_Children;
        #endregion

        #region Private memeber
        /// <summary>
        /// Represent object reference of Resource wrapper class.
        /// </summary>
        static ResourceWrapper wrapper = new ResourceWrapper();

        /// <summary>
        /// Represents value for create different name.
        /// </summary>
        private static int m_nameCreator = 0;

        /// <summary>
        /// Represents bool value for items added and changed externally
        /// </summary>
        internal bool m_itemchangedexternally = false;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Capacity of the <see cref="LogicalElementCollection"/>.
        /// </summary>
        public virtual int Capacity
        {
            get
            {
                return m_Children.Capacity;
            }

            set
            {
                VerifyWriteAccess();
                m_Children.Capacity = value;
            } 
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="LogicalElementCollection"/>.
        /// </summary>
        public virtual int Count
        {
            get
            {
                return m_Children.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsSynchronized of the <see cref="LogicalElementCollection"/>.
        /// </summary>
        /// <value></value>
        /// <returns>true if access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe); otherwise, false.
        /// </returns>
        public virtual bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public virtual FrameworkElement this[int index]
        {
            get
            {
                return m_Children[index];
            }

            set
            {
                VerifyWriteAccess();
                //ValidateElement(value);
                if (value != null)
                {
                    if (m_Children[index] != value)
                    {
                        FrameworkElement element = m_Children[index];

                        if (element != null)
                        {
                            ClearLogicalParent(element);
                        }

                        m_Children[index] = value;
                        SetLogicalParent(value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets SyncRoot of the <see cref="LogicalElementCollection"/>.
        /// </summary>
        public virtual object SyncRoot
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> has a fixed size.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.IList"/> has a fixed size; otherwise, false.</returns>
        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.IList"/> is read-only; otherwise, false.</returns>
        bool IList.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <value>return list.</value>
        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                this[index] = value as FrameworkElement;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds an item to the <see cref="LogicalElementCollection"/> children.
        /// </summary>
        /// <param name="element">The object to add to the <see cref="LogicalElementCollection"/> children.</param>
        /// <returns>Index of the added item.</returns>
        public virtual int Add(FrameworkElement element)
        {
            int result = AddElement(element);
            if (result != -1)
            {
                if (m_LogicalParent != null)
                {
                    m_LogicalParent.m_loadflag = false;
                }
                PrepareElementForLogicalParent(element);
                AddContainerMenuItems(element);
            }
            return result;
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void ClearCollection()
        {
            foreach (FrameworkElement element in m_Children)
            {
                DockingManager.SetDockInfo(element, null);
            }
            m_Children.Clear();
        }

        /// <summary>
        /// Removes all items from the <see cref="LogicalElementCollection"/> children.
        /// </summary>
        public virtual void Clear()
        {
            VerifyWriteAccess();
            ClearInternal();
        }

        /// <summary>
        /// Removes range of items from the <see cref="LogicalElementCollection"/> children.
        /// </summary>
        /// <param name="index">The zero-based index from which removing starts.</param>
        /// <param name="count">The number of items to remove.</param>
        public virtual void RemoveRange(int index, int count)
        {
            VerifyWriteAccess();
            RemoveRangeInternal(index, count);
        }

        /// <summary>
        /// Determines whether the <see cref="LogicalElementCollection"/> children contains a specific value.
        /// </summary>
        /// <param name="element">The object to locate in the <see cref="LogicalElementCollection"/> children.</param>
        /// <returns>true if item is found in the <see cref="LogicalElementCollection"/> children; otherwise, false.</returns>
        public virtual bool Contains(FrameworkElement element)
        {
            return m_Children.Contains(element);
        }

        /// <summary>
        /// Copies the elements of the <see cref="LogicalElementCollection"/> children to an array, starting at a particular array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from <see cref="LogicalElementCollection"/> children. The array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        public virtual void CopyTo(FrameworkElement[] array, int index)
        {
            m_Children.CopyTo(array, index);
        }

        /// <summary>
        /// Copies the elements of the <see cref="LogicalElementCollection"/> children to an array, starting at a particular array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from <see cref="LogicalElementCollection"/> children. The array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        public virtual void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A list of items that can be used to iterate through the collection.</returns>
        public virtual IEnumerator GetEnumerator()
        {
            return m_Children.GetEnumerator();
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="LogicalElementCollection"/> children.
        /// </summary>
        /// <param name="element"> The object to locate in the <see cref="LogicalElementCollection"/> children.</param>
        /// <returns>The index of item if found in the list; otherwise, -1.</returns>
        public virtual int IndexOf(FrameworkElement element)
        {
            return m_Children.IndexOf(element);
        }

        /// <summary>
        /// Inserts an item to the <see cref="LogicalElementCollection"/> children at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="element">The object to insert into the <see cref="LogicalElementCollection"/> children.</param>
        public virtual void Insert(int index, FrameworkElement element)
        {
            VerifyWriteAccess();
            InsertInternal(index, element);
        }

        /// <summary>
        /// Clears the windows registered.
        /// </summary>
        /// <param name="element">The element.</param>
        private void ClearWindowsRegistered(FrameworkElement element)
        {
            if (m_LogicalParent.m_WindowsRegistered != null && m_LogicalParent.m_WindowsRegistered.Count > 0)
            {
                foreach (var window in m_LogicalParent.m_WindowsRegistered)
                {
                    if (window.PrimaryElement != null && window.PrimaryElement.Equals(element))
                    {
                        m_LogicalParent.m_WindowsRegistered.Remove(window);
                        m_LogicalParent.m_WindowOrder.Remove(window);
                        m_LogicalParent.m_VisibleWindows.Remove(window);
                        if (window is FloatWindow)
                        {
                            (window as FloatWindow).DisposeFloatWindow();
                            (window as FloatWindow).ClearDockingManager();
                        }
                        DockedElementTabbedHost host = (window.InternalDataContext as DockedElementTabbedHost);
                        host.InternalDataContext = null;
                        host.HostedElement = null;
                        window.InternalDataContext = null;
                        break;
                    }
                }
            }

            List<object> clearhost = new List<object>();
            foreach (DockedElementTabbedHost m_host in m_LogicalParent.m_Completehost)
            {
               if ((m_host.TabChildren.Count == 0 && m_host.HostedElement == null || (m_host.TabChildren.Count == 1 && m_host.TabChildren[0] == element)))
                    clearhost.Add(m_host);
            }
            foreach (DockedElementTabbedHost m_host in clearhost)
            {
                m_LogicalParent.m_Completehost.Remove(m_host);
                m_host.m_firstelement = null;
                m_host.HostedElement = null;
                m_host.TabChildren.Clear();
                m_host.Dispose();
            }
            clearhost.Clear();
            foreach (var window in m_LogicalParent.m_WindowsUnRegistered)
            {
                if(window.PrimaryElement==null || window.PrimaryElement.Equals(element))
                    clearhost.Add(window);
            }
            foreach (var window in clearhost)
            {
                m_LogicalParent.m_WindowsUnRegistered.Remove(window as IWindow);
                if (window is FloatWindow)
                {
                    (window as FloatWindow).DisposeFloatWindow();
                    (window as FloatWindow).ClearDockingManager();
                }
            }
        }

        /// <summary>
        /// Clears the host.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="element">The element.</param>
        private void ClearHost(DockInfoInternal info, DockedElementTabbedHost host, FrameworkElement element, DockState state)
        {
            if (host != null)
            {
                if (host.TabChildren.Count > 0)
                {
                    if (host.m_firstelement == element)
                        host.m_firstelement = null;
                    if (host.HostedElement == element)
                        host.HostedElement = null;
                }
                else
                {
                    if (m_LogicalParent.m_usedHosts != null && m_LogicalParent.m_usedHosts.Count > 0)
                        m_LogicalParent.m_usedHosts.Remove(host);
                    if (m_LogicalParent.m_primaryChild is MainHost && (m_LogicalParent.m_primaryChild as MainHost).Content is DockedElementsContainer
                        && ((m_LogicalParent.m_primaryChild as MainHost).Content as DockedElementsContainer).Children.Contains(host))
                    {
                        ((m_LogicalParent.m_primaryChild as MainHost).Content as DockedElementsContainer).Children.Remove(host);
                    }
                    host.Dispose();
                    if (state == DockState.Dock)
                        info.HostDock = null;
                    else
                        info.HostFloat = null;
                    info.FloatingWindow = null;
                }
            }
        }

        /// <summary>
        /// Clears the document.
        /// </summary>
        /// <param name="tabcontrol">The tabcontrol.</param>
        /// <param name="element">The element.</param>
        private void ClearDocument(DocumentTabControl tabcontrol, FrameworkElement element)
        {
            if (tabcontrol != null)
            {
                if (tabcontrol.TabPositionCache.Contains(element))
                {
                    tabcontrol.TabPositionCache.Remove(element);
                }
                if (tabcontrol.Items.Count == 0)
                {
                    if (tabcontrol.selectionStack.Count > 0)
                    {
                        tabcontrol.selectionStack.Clear();
                    }
                    LocalValueEnumerator locallySetProperties = tabcontrol.GetLocalValueEnumerator();
                    while (locallySetProperties.MoveNext())
                    {
                        DependencyProperty propertyToClear = locallySetProperties.Current.Property;
                        if (propertyToClear.Name == "SelectedItem")
                        {
                            tabcontrol.ClearValue(propertyToClear);
                        }
                    }
                    if (tabcontrol.previewselectedargs != null)
                    {
                        tabcontrol.previewselectedargs.OldSelectedItem = null;
                        tabcontrol.previewselectedargs.NewSelectedItem = null;
                    }
                }
                tabcontrol.lastSelected = null;
                tabcontrol.ActivatedItem = null;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="LogicalElementCollection"/> children.
        /// </summary>
        /// <param name="element">The object to remove from the <see cref="LogicalElementCollection"/> children.</param>
        public virtual void Remove(FrameworkElement element)
        {
            VerifyWriteAccess();
            ClearWindowsRegistered(element);

            m_Children.Remove(element);
            ClearLogicalParent(element);
            DockInfoInternal info = DockingManager.GetDockInfo(element);
            if (info != null && info.HostDock != null && info.HostDock.Parent != null && info.HostDock.Parent is DockedElementsContainer)
            {
                DockedElementsContainer container = info.HostDock.Parent as DockedElementsContainer;
                foreach (Splitter splt in container.m_Splitters)
                {
                    if (DockedElementsContainer.GetElementAfterSplitter(splt) == info.HostDock)
                    {
                        DockedElementsContainer.SetElementAfterSplitter(splt, null);
                    }
                    if (DockedElementsContainer.GetElementBeforeSplitter(splt) == info.HostDock)
                    {
                        DockedElementsContainer.SetElementBeforeSplitter(splt, null);
                    }
                }
            }
            m_LogicalParent.RemoveElementFromDocking(element);
            ClearDocument(DockingManager.GetTabControl(element as DependencyObject) as DocumentTabControl, element);

            if (info != null)
            {
                ClearHost(info, info.HostDock, element, DockState.Dock);
                ClearHost(info, info.HostFloat, element, DockState.Float);
                BindingOperations.ClearAllBindings(element);
                if (info.DockingManager != null && info.DockingManager.m_hwndHosts.Count > 0)
                {
                    info.DockingManager.m_hwndHosts.Clear();
                }
                if (info.DockingManager != null && info.DockingManager.lastacitive != null)
                {
                    if (info.DockingManager.lastacitive.Contains(element))
                        info.DockingManager.lastacitive.Remove(element);
                }
            }
            if (info.DockingManager.UseNativeFloatWindow)
            {
                NativeFloatWindow window = DockingManager.GetDockInfo(element).NativeWindow;
                if (window != null)
                {
                    window.Close();
                }
            }
        }

        private void ClearLocal1(DependencyObject obj, FrameworkElement element)
        {
            LocalValueEnumerator locallySetProperties = obj.GetLocalValueEnumerator();
            while (locallySetProperties.MoveNext())
            {
                DependencyProperty propertyToClear = locallySetProperties.Current.Property;
                if (locallySetProperties.Current.Value == element)
                {
                    obj.ClearValue(propertyToClear);
                }
            }
        }

        private void ClearLocal(DependencyObject obj,FrameworkElement element)
        {
            LocalValueEnumerator locallySetProperties = obj.GetLocalValueEnumerator();
            while (locallySetProperties.MoveNext())
            {
                DependencyProperty propertyToClear = locallySetProperties.Current.Property;
                if (!propertyToClear.ReadOnly && !(propertyToClear.DefaultMetadata.DefaultValue is bool) && propertyToClear.Name != "State" && locallySetProperties.Current.Value==element)
                {
                    obj.ClearValue(propertyToClear);
                }
            }
        }

        /// <summary>
        /// Removes the <see cref="LogicalElementCollection"/> children item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public virtual void RemoveAt(int index)
        {
            FrameworkElement element = m_Children[index];
            if (element != null)
            {
                Remove(element);
            }
        }

        /// <summary>
        /// Initializes static members of the <see cref="LogicalElementCollection"/> class.
        /// </summary>
        static LogicalElementCollection()
        {
            //langDictionary = new ResourceDictionary();
            //langDictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Themes/LangDictionary.xaml", UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogicalElementCollection"/> class.
        /// </summary>
        /// <param name="logicalParent">The logical parent.</param>
        public LogicalElementCollection(DockingManager logicalParent)
        {
            m_LogicalParent = logicalParent;
            m_Children = new List<FrameworkElement>();
        }

        /// <summary>
        /// Adds an item to the <see cref="System.Collections.IList"/>.
        /// </summary>
        /// <param name="value">The <see cref="System.Object"/> to add to the <see cref="System.Collections.IList"/>.</param>
        /// <returns>
        /// The position into which the new element was inserted.
        /// </returns>
        /// <exception cref="System.NotSupportedException">The <see cref="System.Collections.IList"/> is read-only.-or- The <see cref="System.Collections.IList"/> has a fixed size. </exception>
        int IList.Add(object value)
        {
            return Add(value as FrameworkElement);
        }

        /// <summary>
        /// Determines whether the <see cref="System.Collections.IList"/> contains a specific value.
        /// </summary>
        /// <param name="value">The <see cref="System.Object"/> to locate in the <see cref="System.Collections.IList"/>.</param>
        /// <returns>
        /// true if the <see cref="System.Object"/> is found in the <see cref="System.Collections.IList"/>; otherwise, false.
        /// </returns>
        bool IList.Contains(object value)
        {
            return Contains(value as FrameworkElement);
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="System.Collections.IList"/>.
        /// </summary>
        /// <param name="value">The <see cref="System.Object"/> to locate in the <see cref="System.Collections.IList"/>.</param>
        /// <returns>
        /// The index of <paramref name="value"/> if found in the list; otherwise, -1.
        /// </returns>
        int IList.IndexOf(object value)
        {
            return IndexOf(value as FrameworkElement);
        }

        /// <summary>
        /// Inserts an item to the <see cref="System.Collections.IList"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="value"/> should be inserted.</param>
        /// <param name="value">The <see cref="System.Object"/> to insert into the <see cref="System.Collections.IList"/>.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not a valid index in the <see cref="System.Collections.IList"/>. </exception>
        /// <exception cref="System.NotSupportedException">The <see cref="System.Collections.IList"/> is read-only.-or- The <see cref="System.Collections.IList"/> has a fixed size. </exception>
        /// <exception cref="System.NullReferenceException">
        /// <paramref name="value"/> is null reference in the <see cref="System.Collections.IList"/>.</exception>
        void IList.Insert(int index, object value)
        {
            Insert(index, value as FrameworkElement);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="System.Collections.IList"/>.
        /// </summary>
        /// <param name="value">The <see cref="System.Object"/> to remove from the <see cref="System.Collections.IList"/>.</param>
        /// <exception cref="System.NotSupportedException">The <see cref="System.Collections.IList"/> is read-only.-or- The <see cref="System.Collections.IList"/> has a fixed size. </exception>
        void IList.Remove(object value)
        {
            Remove(value as FrameworkElement);
        }

        /// <summary>
        /// For's the each.
        /// </summary>
        /// <param name="action">The action.</param>
        public void ForEach(Action<FrameworkElement> action)
        {
            for (int i = 0; i < m_Children.Count; i++)
            {
                action(m_Children[i]);
            }
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Removes the no verify.
        /// </summary>
        /// <param name="element">The element.</param>
        internal virtual void RemoveNoVerify(FrameworkElement element)
        {
            m_Children.Remove(element);
        }

        /// <summary>
        /// Internals the add.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>return index of the element.</returns>
        internal int InternalAdd(FrameworkElement element)
        {
            return AddElement(element);
        }

        /// <summary>
        /// Clears the internal.
        /// </summary>
        internal void ClearInternal()
        {
            //int count = m_Children.Count;

            //if (count > 0)
            //{
            //    FrameworkElement[] visualArray = new FrameworkElement[count];

            //    for (int i = 0; i < count; i++)
            //    {
            //        visualArray[i] = m_Children[i];
            //    }

            //    m_Children.Clear();

            //    for (int j = 0; j < count; j++)
            //    {
            //        FrameworkElement element = visualArray[j];

            //        if (element != null)
            //        {
            //            ClearLogicalParent(element);
            //        }
            //    }

            //    m_LogicalParent.ClearElements();
            //}
            while (Count > 0)
            {
                Remove(this[Count - 1]);
            }
        }

        /// <summary>
        /// Inserts the internal.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="element">The element.</param>
        internal void InsertInternal(int index, FrameworkElement element)
        {
            //ValidateElement(element);
            if (element != null)
            {
                SetLogicalParent(element);
                m_Children.Insert(index, element);
                PrepareElementForLogicalParent(element);
                AddContainerMenuItems(element);
            }
        }

        /// <summary>
        /// Removes the range internal.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        internal void RemoveRangeInternal(int index, int count)
        {
            int num = m_Children.Count;

            if (count > (num - index))
            {
                count = num - index;
            }

            if (count > 0)
            {
                FrameworkElement[] visualArray = new FrameworkElement[count];
                int num2 = index;

                for (int i = 0; i < count; i++)
                {
                    visualArray[i] = m_Children[num2];
                    num2++;
                }

                m_Children.RemoveRange(index, count);

                for (num2 = 0; num2 < count; num2++)
                {
                    FrameworkElement element = visualArray[num2];

                    if (element != null)
                    {
                        Remove(element);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the internal.
        /// </summary>
        /// <param name="index">The index value.</param>
        /// <param name="item">The framework element item.</param>
        internal void SetInternal(int index, FrameworkElement item)
        {
            //ValidateElement(item);

            if (item!=null && m_Children[index] != item)
            {
                m_Children[index] = item;
            }
        }

        /// <summary>
        /// Sets the index.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="index">The index.</param>
        internal void SetIndex(FrameworkElement element, int index)
        {
            m_Children.Remove(element);

            if (index < m_Children.Count)
            {
                m_Children.Insert(index, element);
            }
            else
            {
                m_Children.Add(element);
            }
        }

        /// <summary>
        /// Used for writing access verification in the <see cref="LogicalElementCollection"/>. 
        /// </summary>
        protected virtual void VerifyWriteAccess()
        {
        }

        /// <summary>
        /// Invokes remove method of the <see cref="LogicalElementCollection"/>.
        /// </summary>
        /// <param name="element">An argument for the invoked method.</param>
        private void ClearLogicalParent(FrameworkElement element)
        {
            if (m_LogicalParent != null)
            {
                m_LogicalParent.RemoveLogicalChildInternal(element);
            }
        }

        /// <summary>
        /// Invokes add method of the <see cref="LogicalElementCollection"/>.
        /// </summary>
        /// <param name="element">An argument for the invoked method.</param>
        private void SetLogicalParent(FrameworkElement element)
        {
            m_LogicalParent.AddLogicalChildInternal(element);
        }

        /// <summary>
        /// Adds the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>return index.</returns>
        private int AddElement(FrameworkElement element)
        {
            int num = -1;
            if (!m_Children.Contains(element))
            {
                VerifyWriteAccess();
                //ValidateElement(element);
                if (element != null)
                {
                    SetLogicalParent(element);
                    m_Children.Add(element);
                    num = m_Children.IndexOf(element);
                }
            }
            return num;
        }

        /// <summary>
        /// Prepares the element for logical parent.
        /// </summary>
        /// <param name="element">The element.</param>
        private void PrepareElementForLogicalParent(FrameworkElement element)
        {
            if (m_LogicalParent.IsInitialized)
            {
                if (string.IsNullOrEmpty(element.Name))
                {
                    element.Name = AddedName + (++m_nameCreator);
                }

                m_LogicalParent.AddElement(element);
                m_LogicalParent.InternalRemoveVisualChild(element);

                if (m_LogicalParent.IsLoaded)
                {
                    m_LogicalParent.AddElementToDocking(element);
                }
                if (!DesignerProperties.GetIsInDesignMode(m_LogicalParent))
                    m_LogicalParent.AddElementToDockingTree(element);

                if (!m_LogicalParent.IsLoaded)
                {
                    DockingManager.UpdateLayout(m_LogicalParent);
                }
            }
        }

        /// <summary>
        /// Validates the element.
        /// </summary>
        /// <param name="element">The element.</param>
        private static void ValidateElement(FrameworkElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
        }
        #endregion

        #region DockingIssue
        /// <summary>
        /// Adds the container menu items.
        /// </summary>
        /// <param name="element">The element.</param>
        private static void AddContainerMenuItems(UIElement element)
        {
            DocumentContextMenuItemsCollection items = new DocumentContextMenuItemsCollection
            {
                InitializeMenuItem(element,wrapper.MDIFloating, false, false, DockingManager.FloatingCommand),//InitializeMenuItem(element, langDictionary["MDIFloating"] as string, false, false, DockingManager.FloatingCommand),
                InitializeMenuItem(element,wrapper.MDIDockable, false, false, DockingManager.DockableCommand), //InitializeMenuItem(element, langDictionary["MDIDockable"] as string, false, false, DockingManager.DockableCommand),
                InitializeMenuItem(wrapper.MDIDocument, false, true) //InitializeMenuItem(langDictionary["MDIDocument"] as string, false, true)
            };
            DocumentContainer.SetMDIContextMenuItemsCollection(element, items);
        }

        /// <summary>
        /// Initializes the menu item.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="header">The header.</param>
        /// <param name="isCheckable">if set to <c>true</c> [is checkable].</param>
        /// <param name="isChecked">if set to <c>true</c> [is checked].</param>
        /// <param name="routedCommand">The routed command.</param>
        /// <returns>return menu item.</returns>
        private static DocMenuItem InitializeMenuItem(IInputElement element, string header, bool isCheckable, bool isChecked, ICommand routedCommand)
        {
            DocMenuItem item = new DocMenuItem
            {
                Header = header,
                CommandTarget = element,
                Command = routedCommand,
                CommandParameter = element,
                IsCheckable = isCheckable,
                IsChecked = isChecked
            };

            return item;
        }

        /// <summary>
        /// Creates and initializes menu item.
        /// </summary>
        /// <param name="header">Header of the item</param>
        /// <param name="isCheckable">True if item is checkable, otherwise - false.</param>
        /// <param name="isChecked">True if item is checked, otherwise - false.</param>
        /// <returns>Menu item created</returns>
        private static DocMenuItem InitializeMenuItem(string header, bool isCheckable, bool isChecked)
        {
            DocMenuItem item = new DocMenuItem
            {
                Header = header,
                IsCheckable = isCheckable,
                IsChecked = isChecked
            };

            return item;
        }
        #endregion
    }
}