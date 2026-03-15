// <copyright file="TDILayoutPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Syncfusion.Windows.Tools.Controls.Resources;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Reflection;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Presents panel for TDI support in DocumentContainer
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    
    public class TDILayoutPanel : PanelBase, ILayoutPanel
    {
        #region Constants
        /// <summary>
        /// Presents name prefix.
        /// </summary>
        private const string NAME_PREFIX = "InternalAutoGenerateName";

        /// <summary>
        /// Presents true bit.
        /// </summary>
        private const char TRUE_BIT = '1';

        /// <summary>
        /// Presents false bit.
        /// </summary>
        private const char FALSE_BIT = '0';

        /// <summary>
        /// Presents true bit.
        /// </summary>
        private const string TRUE_BIT_EX = "1";

        /// <summary>
        /// Presents false bit.
        /// </summary>
        private const string FALSE_BIT_EX = "0";

        /// <summary>
        /// Represents  CREATE
        /// </summary>
        private const string C_CREATE_NEWGROUP = "CREATE";

        /// <summary>
        /// Represents  MOVE NEXT
        /// </summary>
        private const string C_MOVE_TO_NEXT_GROUP = "MOVENEXT";


        /// <summary>
        /// Represents  MOVE PREVIOUS
        /// </summary>
        private const string C_MOVE_TO_PREVIOUS_GROUP = "MOVEPREVIOUS";

        /// <summary>
        /// Represents  CANCEL
        /// </summary>
        private const string C_CANCEL = "CANCEL";
        #endregion

        #region Internal type
        /// <summary>
        /// Presents way's map.
        /// </summary>
        private class WaysMap : Dictionary<List<bool>, GroupInformation>
        {
            #region Private members
            /// <summary>
            /// Presents keys sorted list.
            /// </summary>
            private readonly KeysList m_KeysList = new KeysList();
            #endregion

            #region Public methods
            /// <summary>
            /// Adds to way map.
            /// </summary>
            /// <param name="inputWay">The input way.</param>
            /// <param name="element">The element.</param>
            public void AddToWaysMap(List<bool> inputWay, UIElement element)
            {
                int cnt = inputWay.Count;
                bool isAdded = false;

                foreach (List<bool> way in Keys)
                {
                    if (cnt == way.Count)
                    {
                        bool equal = true;

                        for (int i = 0; i < cnt; ++i)
                        {
                            if (inputWay[i] != way[i])
                            {
                                equal = false;
                                break;
                            }
                        }

                        if (equal)
                        {
                            GroupInformation gInfo;
                            TryGetValue(way, out gInfo);
                            List<UIElement> elList = gInfo.ElementsList;
                            elList.Add(element);
                            isAdded = true;
                            break;
                        }
                    }
                }

                if (!isAdded)
                {
                    GroupInformation gInfo = new GroupInformation(element);
                    Add(inputWay, gInfo);
                    UpdateSortKeyList(inputWay);
                }
            }

            /// <summary>
            /// Gets the max ways list.
            /// </summary>
            /// <returns>KeysList Count</returns>
            public KeysList GetMaxWaysList()
            {
                int max = -1;

                foreach (List<bool> way in Keys)
                {
                    if (max < way.Count)
                    {
                        max = way.Count;
                    }
                }

                KeysList returnList = new KeysList();

                foreach (List<bool> way in m_KeysList)
                {
                    if (max == way.Count)
                    {
                        returnList.Add(way);
                    }
                }
#if DevCode
				if( 0 != returnList.Count % 2 )
				{
					Debugger.Break();
				}
#endif
                return returnList;
            }

            /// <summary>
            /// Removes the way.
            /// </summary>
            /// <param name="way">The way RemoveWay.</param>
            public void RemoveWay(List<bool> way)
            {
                Remove(way);
                m_KeysList.Remove(way);
            }
            #endregion

            #region Implementation
            /// <summary>
            /// Updates the sort key list.
            /// </summary>
            /// <param name="inputWay">The input way.</param>
            private void UpdateSortKeyList(List<bool> inputWay)
            {
                int inputPos = m_KeysList.Count;

                for (int i = 0, cnt = m_KeysList.Count; i < cnt; ++i)
                {
                    List<bool> way = m_KeysList[i];

                    if (Below(way, inputWay))
                    {
                        inputPos = i;
                        break;
                    }
                }

                m_KeysList.Insert(inputPos, inputWay);
            }

            /// <summary>
            /// Below the specified way.
            /// </summary>
            /// <param name="way">The way bool.</param>
            /// <param name="inputWay">The input way bool.</param>
            /// <returns>bool result result</returns>
            private static bool Below(IList<bool> way, IList<bool> inputWay)
            {
                bool result = false;

                for (int i = 0, cnt = way.Count; i < cnt; ++i)
                {
                    if (i < inputWay.Count && inputWay[i] != way[i])
                    {
                        result = inputWay[i];
                        break;
                    }
                }

                return result;
            }
            #endregion
        }

        /// <summary>
        /// Presents wrapper for key list.
        /// </summary>
        private class KeysList : List<List<bool>>
        {
        }

        /// <summary>
        /// Presents information about tab group.
        /// </summary>
        private struct GroupInformation
        {
            /// <summary>
            /// Presents orientation.
            /// </summary>
            public readonly Orientation Orientation;

            /// <summary>
            /// Presents list of elements.
            /// </summary>
            public readonly List<UIElement> ElementsList;

            /// <summary>
            /// Initializes a new instance of the <see cref="GroupInformation"/> struct.
            /// </summary>
            /// <param name="element">The element.</param>
            public GroupInformation(UIElement element)
            {
                Orientation = (Orientation)element.GetValue(TDILayoutPanel.TDIGroupOrientationProperty);
                ElementsList = new List<UIElement> { element };
            }
        }
        #endregion

        #region Private members

        ResourceWrapper wrapper = new ResourceWrapper();

        /// <summary>
        /// Presents empty point.
        /// </summary>
        private static readonly Point EMPTY_POINT = new Point(0, 0);

        /// <summary>
        /// Presents Name suffix
        /// </summary>
        public static int m_nameSufix = 0;

        /// <summary>
        /// Presents list of TabControls in all groups.
        /// </summary>
        internal readonly List<DocumentTabControl> m_TabList = new List<DocumentTabControl>();

        /// <summary>
        /// Presents the active tab control.
        /// </summary>
        private DocumentTabControl m_activeTabControl = null;

        /// <summary>
        /// Presents value that indicate or any TabCon is changing group now.
        /// </summary>
        private bool m_changingGroup = false;
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when Content property is changed.
        /// </summary>
        public event PropertyChangedCallback ContentChanged;
        
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="TDILayoutPanel"/> class.
        /// </summary>
        static TDILayoutPanel()
        {
            EnvironmentTest.ValidateLicense(typeof(TDILayoutPanel));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TDILayoutPanel"/> class.
        /// </summary>
        public TDILayoutPanel()
        {
            DocumentTabControl tabControl = new DocumentTabControl
            {
                tdipanel=this,
                Background = Brushes.Transparent,
                EnableLabelEdit = false,
                Name = string.Concat(NAME_PREFIX, ++m_nameSufix)
            };

            //SetPropertiesBinding(Container,tabControl,DocumentContainer.EditableTabHeaderStyleProperty, TabControlExt.EditHeaderItemStyleProperty);

            //BindingUtils.SetBinding(tabControl, Container, TabControlExt.HideHeaderOnSingleChildProperty, DocumentContainer.HideTDIHeaderOnSingleChildProperty,BindingMode.OneWay);
            SetPropertiesBinding(Container, tabControl, DocumentContainer.HideTDIHeaderOnSingleChildProperty, TabControlExt.HideHeaderOnSingleChildProperty);
            SetPropertiesBinding( this, tabControl, FrameworkElement.FlowDirectionProperty, TabControlExt.FlowDirectionProperty );
            ActiveTabControl = tabControl;
            m_TabList.Add(tabControl);
            SetCommnadBindings();
            Content = tabControl;
            LayoutUpdated += new EventHandler(OnLayoutUpdated);
            m_cm = new Popup { };
            Loaded += new RoutedEventHandler(TDILayoutPanel_Loaded);
            Unloaded += new RoutedEventHandler(TDILayoutPanel_Unloaded);
            if (m_TabList != null)
            {
                foreach (TabControlExt tabcontrolext in m_TabList)
                {
                    InitializeTabControl(tabcontrolext);
                }
            }
        }

        void TDILayoutPanel_Loaded(object sender, RoutedEventArgs e)
        {
           
            if (Container != null)
            {
                foreach (FrameworkElement element in Container.Items)
                {
                    SetTDIIndexForDocking(element);
                }

            }
            if (Container != null && m_TabList != null)
            {
                foreach (TabControlExt tabcontrolext in m_TabList)
                {
                    tabcontrolext.ItemsChanged += new NotifyCollectionChangedEventHandler(OnItemsChanged);
                    if (!Container.m_tabControlCollection.Contains(tabcontrolext as DocumentTabControl))
                        Container.m_tabControlCollection.Add(tabcontrolext as DocumentTabControl);
                }
            }
        }

        void TDILayoutPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            if (m_TabList != null)
            {
                foreach (TabControlExt tabcontrolext in m_TabList)
                {
                    tabcontrolext.ItemsChanged -= new NotifyCollectionChangedEventHandler(OnItemsChanged);
                    tabcontrolext.GotKeyboardFocus -= new KeyboardFocusChangedEventHandler(OnTabControlGotKeyboardFocus);
                    tabcontrolext.TakeDragItemEvent -= new TakeDragItemHandler(OnTabControlTakeDragItemEvent);
                    tabcontrolext.OnCloseAllTabs -= new OnCloseTabsEventHandler(OnTabControlOnCloseAllTabs);
                    tabcontrolext.OnCloseOtherTabs -= new OnCloseTabsEventHandler(OnTabControlOnCloseOtherTabs);
                    tabcontrolext.SelectedItemChangedEvent -= new SelectedItemChangedEventHandler(OnTabControlSelectedItemChanged);
                    tabcontrolext.TabClosing -= new CancelingRoutedEventHandler(OnTabControlTabClosing);
                }
            }
            
            if (m_cm != null)
            {
                m_cm.Closed -= new EventHandler(OnContextMenuClosed);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the value of the Content dependency property.
        /// </summary>
        public UIElement Content
        {
            get
            {
                return (UIElement)GetValue(ContentProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(ContentProperty, value);
#else
                SetValue(ContentProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the value of the Container dependency property.
        /// </summary>
        public DocumentContainer Container
        {
            get
            {
                return (DocumentContainer)GetValue(ContainerProperty);
            }

            protected internal set
            {
                SetValue(ContainerPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the active tab control.
        /// </summary>
        /// <value>The active tab control.</value>
        internal DocumentTabControl ActiveTabControl
        {
            get
            {
                return m_activeTabControl;
            }

            set
            {
                m_activeTabControl = value;
            }
        }

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        /// <value></value>
        /// <returns>The number of visual child elements for this element.</returns>
        protected override int VisualChildrenCount
        {
            get
            {
                return null == Content ? 0 : 1;
            }
        }
        #endregion

        #region ILayoutPanel Members
        /// <summary>
        /// Creates the child document params.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        /// <returns>ChildDocument Params isActive</returns>
        public ChildDocumentParams CreateChildDocumentParams(FrameworkElement element, bool isActive)
        {
            int index = -1;
            bool isSelected = false;

            foreach (DocumentTabControl tabCon in m_TabList)
            {
                if (tabCon.ContainsItem(element))
                {
                    index = tabCon.GetIndexOfElement(element);
                    if (TDILayoutPanel.GetTDIIndex(element) == -1)
                    {
                        tabCon.m_canupdateindex = false;
                        TDILayoutPanel.SetTDIIndex(element, index);
                        tabCon.m_canupdateindex = true;
                    }
                    else 
                    {
                        index = TDILayoutPanel.GetTDIIndex(element);
                    }
                    isSelected = index == tabCon.SelectedIndex;
                    TDILayoutPanel.SetIsSelected(element, isSelected);
                    break;
                }
            }
            return new ChildDocumentParams(element, PropertiesMode.Child, isActive);
        }

        /// <summary>
        /// Sets the TDI index for docking.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void SetTDIIndexForDocking(FrameworkElement element)
        {
            int index = 0;
            bool isSelected = false;
            DockingManager manager = DockingManager.ResolveManager(element);
            FrameworkElement m_previousselecteditem=null;
            if (manager != null)
            {
                foreach (DocumentTabControl tabCon in m_TabList)
                {
                    if (tabCon.ContainsItem(element))
                    {
                        index = tabCon.GetIndexOfElement(element);
                        if (TDILayoutPanel.GetTDIIndex(element) == -1)
                        {
                            tabCon.m_canupdateindex = false;
                            TDILayoutPanel.SetTDIIndex(element, index);
                            tabCon.m_canupdateindex = true;
                        }
                        else if (manager.canUpdateTDIindex && index != TDILayoutPanel.GetTDIIndex(element))
                        {
                            index = TDILayoutPanel.GetTDIIndex(element);
                            if (manager != null && manager.m_stateDefault.Equals(String.Empty))
                            {
                                m_previousselecteditem = tabCon.SelectedItem as FrameworkElement;
                                tabCon.UpdateIndex(index, element);
                                if (index != 0 && m_previousselecteditem != tabCon.SelectedItem)
                                    tabCon.SelectedItem = m_previousselecteditem;
                            }
                            else
                                tabCon.UpdateIndex(index, element);
                        }
                        isSelected = index == tabCon.SelectedIndex;
                        TDILayoutPanel.SetIsSelected(element, isSelected);
                        break;
                    }
                }
             }
        }


        /// <summary>
        /// Gets the ordered items.
        /// </summary>
        /// <returns>IList Control </returns>
        public IList<Control> GetOrderedItems()
        {
            List<Control> result = new List<Control>();
            result.AddRange(ActiveTabControl.GetOrderedItems());

            for (int i = 0; i < m_TabList.Count; ++i)
            {
                DocumentTabControl tabControl = m_TabList[i];

                if (ActiveTabControl != tabControl)
                {
                    result.AddRange(tabControl.GetOrderedItems());
                }
            }

            return result;
        }

        /// <summary>
        /// Sets the active item.
        /// </summary>
        /// <param name="activeItem">The active item.</param>
       internal FrameworkElement oldactive;
       internal bool changed = false;
        public void SetActiveItem(FrameworkElement activeItem)
        {
            for (int i = 0; i < m_TabList.Count; ++i)
            {
                DocumentTabControl tabControl = m_TabList[i];
                if (tabControl.ContainsItem(activeItem))
                {                   
                    DockingManager docmanager = DockingManager.ResolveManager(activeItem as UIElement);
                    if (docmanager != null)
                    {
                        if (docmanager.m_mousemoveonheaderpanel)
                        {
                            if (tabControl.Items.Count > docmanager.m_mouseonheaderpanelindex)
                            {
                                if (tabControl.m_mousemoveonitem != null || docmanager.m_mousemoveonTabpaneladv)
                                {
                                    if (docmanager.m_mouseonheaderpanelindex == TDILayoutPanel.GetTDIIndex(activeItem as DependencyObject))
                                    {
                                        TDILayoutPanel.SetTDIIndex(activeItem as DependencyObject, docmanager.m_mouseonheaderpanelindex);
                                        DocumentTabControl tabcontrol = (DockingManager.GetTabControl(activeItem as DependencyObject)) as DocumentTabControl;
                                        tabcontrol.UpdateIndex(docmanager.m_mouseonheaderpanelindex, activeItem);
                                        tabcontrol.UpdateIndexOrder();
                                    }
                                    else
                                    {
                                        TDILayoutPanel.SetTDIIndex(activeItem as DependencyObject, docmanager.m_mouseonheaderpanelindex);
                                    }
                                    
                                }
                            }
                        }
                    }
                    tabControl.SetActiveItem(activeItem);
                    ActiveTabControl = tabControl;
                    break;
                }
            }
        }

        /// <summary>
        /// Determines whether this instance can switch.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance can switch; otherwise, <c>false</c>.
        /// </returns>
        public bool CanSwitch()
        {
            return 1 < GetOrderedItems().Count;
        }

        /// <summary>
        /// Sets the focus.
        /// </summary>
        public void SetFocus()
        {
            Focus();
        }

        /// <summary>
        /// Gets the Content.
        /// </summary>
        /// <param name="wrapper">The wrapper.</param>
        /// <returns>UIElement  TabItemExt</returns>
        public UIElement GetContent(Control wrapper)
        {
            return DocumentTabControl.GetContent((TabItemExt)wrapper);
        }

        /// <summary>
        /// Forwards the switch immediate.
        /// </summary>
        /// <param name="firstTabulation">if set to <c>true</c> [first tabulation].</param>
        /// <param name="isKeepCircle">if set to <c>true</c> [is keep circle].</param>
        public void ForwardSwitchImmediate(bool firstTabulation, bool isKeepCircle)
        {
            if (null == ActiveTabControl)
            {
                throw new NotImplementedException();
            }

            ActiveTabControl.SwitchImmediate(1);
        }

        /// <summary>
        /// Back forward the switch immediate.
        /// </summary>
        public void BackforwardSwitchImmediate()
        {
            if (null == ActiveTabControl)
            {
                throw new NotImplementedException();
            }

            ActiveTabControl.SwitchImmediate(-1);
        }

        /// <summary>
        /// Sets the active window.
        /// </summary>
        /// <param name="wrapper">The wrapper.</param>
        public void SetActiveWindow(Control wrapper)
        {
            m_cm.IsOpen = false;
        }

        /// <summary>
        /// Sets the focus after persist load.
        /// </summary>
        public void UpdateAfterPersistLoad()
        {
            UIElement activeDocument = null != Container ? Container.ActiveDocument : null;
            SetContainer();
            BuildTabGroup();

            foreach (DocumentTabControl tabCon in m_TabList)
            {
                tabCon.UpdateAfterPersistLoad(activeDocument);
                tabCon.m_loadstate = true;
                if (DocumentContainer.GetDocumentTabControlStyle(Container) != null)
                {
                    tabCon.Style = DocumentContainer.GetDocumentTabControlStyle(Container) as Style;
                }
                else if (Container != null && (Container.IsInDockingManager
                                               && DockingManager.GetDocumentTabControlStyle(Container.FlipParent as DockingManager) != null))
                {
                    tabCon.Style=(Style)DockingManager.GetDocumentTabControlStyle(Container.FlipParent as DockingManager);
                }
            }
            SetTDIHide();
            if (Container != null)
            Container.LoadingPersistState = false;
        }
        #endregion

        #region PanelBase Methods
        /// <summary>
        /// Removes the element.
        /// </summary>
        /// <param name="element">The element.</param>
        public override void RemoveElement(UIElement element)
        {
            Content = null;
        }

        /// <summary>
        /// Adds the element.
        /// </summary>
        /// <param name="element">The element.</param>
        public override void AddElement(UIElement element)
        {
            Content = element;
            InvalidateMeasure();
            InvalidateArrange();
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            for (int i = m_TabList.Count - 1; i > -1; --i)
            {
                DisposeTabControl(m_TabList[i]);
            }
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Gets the TDI group orientation.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static Orientation GetTDIGroupOrientation(DependencyObject obj)
        {
            if (DockingManager.GetState(obj) == DockState.Document)
            {
                if (DockingManager.GetTabControl(obj) != null)
                {
                    return (Orientation)DockingManager.GetTabControl(obj).GetValue(TDILayoutPanel.TDIGroupOrientationProperty);
                }
            }
            return (Orientation)obj.GetValue(TDILayoutPanel.TDIGroupOrientationProperty);
        }

        /// <summary>
        /// Sets the TDI group orientation.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetTDIGroupOrientation(DependencyObject obj, Orientation value)
        {
            if (DockingManager.GetState(obj) == DockState.Document)
            {
                if (DockingManager.GetTabControl(obj) != null)
                {
                    DockingManager.GetTabControl(obj).SetValue(TDILayoutPanel.TDIGroupOrientationProperty, value);
                }
            }
        }

        /// <summary>
        /// Gets the TDI group orientation.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static int GetTDIIndex(DependencyObject obj)
        {
            return (int)obj.GetValue(TDILayoutPanel.TDIIndexProperty);
        }

        /// <summary>
        /// Gets the is selected.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetIsSelected(DependencyObject obj)
        {
            return (bool)obj.GetValue(TDILayoutPanel.IsSelectedProperty);
        }

        /// <summary>
        /// Sets the is selected.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        internal static void SetIsSelected(DependencyObject obj, bool value)
        {
            obj.SetValue(TDILayoutPanel.IsSelectedProperty, value);
        }

        /// <summary>
        /// Gets the way of TDI group.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static string GetWayOfTDIGroup(DependencyObject obj)
        {
            if (DockingManager.GetState(obj) == DockState.Document)
            {
                if ((obj as DocumentTabControl) != null)
                {
                    return (string)(obj as DocumentTabControl).GetValue(TDILayoutPanel.WayOfTDIGroupProperty);
                }
            }
            return  (string)obj.GetValue(TDILayoutPanel.WayOfTDIGroupProperty);
        }

        /// <summary>
        /// Sets the way of TDI group.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetWayOfTDIGroup(DependencyObject obj, string value)
        {
            if (DockingManager.GetState(obj) == DockState.Document)
            {
                TDILayoutPanel panel = (TDILayoutPanel)VisualUtils.FindAncestor((Visual)obj, typeof(TDILayoutPanel));
                if (panel != null && panel.m_TabList != null && panel.m_TabList.Count > 1)
                {
                    if ((obj as DocumentTabControl) != null)
                    {
                        (obj as DocumentTabControl).SetValue(TDILayoutPanel.WayOfTDIGroupProperty, value);
                        ItemsUpdate(obj as ItemsControl);
                    }
                    else
                    {
                        (DockingManager.GetTabControl(obj as DependencyObject) as DocumentTabControl).SetValue(TDILayoutPanel.WayOfTDIGroupProperty, value);
                        ItemsUpdate(DockingManager.GetTabControl(obj as DependencyObject) as ItemsControl);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the TDI group orientation.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetTDIIndex(DependencyObject obj, int value)
        {
             obj.SetValue(TDILayoutPanel.TDIIndexProperty, value);
        }


        public static double GetSplitPanelOffset(DependencyObject obj)
        {
            return (double)obj.GetValue(SplitPanelOffsetProperty);
        }

        public static void SetSplitPanelOffset(DependencyObject obj, double value)
        {
            obj.SetValue(SplitPanelOffsetProperty, value);
        }


        /// <summary>
        /// Lists the bool to string convert.
        /// </summary>
        /// <param name="list">The list result.</param>
        /// <returns>string result</returns>
        internal static string ListBoolToStringConvert(ICollection<bool> list)
        {
            StringBuilder result = new StringBuilder(list.Count);

            foreach (bool item in list)
            {
                char input = item ? TRUE_BIT : FALSE_BIT;
                result.Append(input);
            }

            return result.ToString();
        }

        /// <summary>
        /// Strings to bool list.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <returns>List result</returns>
        internal static List<bool> StringToBoolList(string inputString)
        {
            int cnt = inputString.Length;
            List<bool> result = new List<bool>(cnt);

            for (int i = 0; i < cnt; ++i)
            {
                result.Add(TRUE_BIT == inputString[i]);
            }

            return result;
        }

        /// <summary>
        /// Raises ContentChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnContentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != ContentChanged)
            {
                ContentChanged(this, e);
            }

            Visual oldContent = e.OldValue as Visual;

            if (null != oldContent)
            {
              //BindingOperations.ClearBinding(oldContent, SkinStorage.VisualStyleProperty);
                try
                {
                  RemoveVisualChild(oldContent);
                  RemoveLogicalChild(oldContent);
                }
                catch { }
     
            }

            Visual newContent = e.NewValue as Visual;

            if (null != newContent)
            {
              AddVisualChild(newContent);
              AddLogicalChild(newContent);
              BindingUtils.SetBinding(newContent, this, SkinStorage.VisualStyleProperty, SkinStorage.VisualStyleProperty);
            }
        }

        /// <summary>
        /// Updates property value cache and raises ContainerChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnContainerChanged(DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer container = (DocumentContainer)e.NewValue;
            DocumentContainer containerOld = (DocumentContainer)e.OldValue;

            if (containerOld != null)
            {
                DetachFromContainer(containerOld);
            }

            if (container != null)
            {
                AttachToContainer(container);

                Style tabStyle = null;

                tabStyle = (Style)DocumentContainer.GetDocumentTabControlStyle(ActiveTabControl) ??
                    (Style)DocumentContainer.GetDocumentTabControlStyle(container);

                UIElement item = ActiveTabControl as UIElement;

                if (container.IsInDockingManager)
                {
                    tabStyle = (Style)DockingManager.GetDocumentTabControlStyle(item) ??
                              (Style)DockingManager.GetDocumentTabControlStyle(container.FlipParent as DockingManager);
                }

                if (tabStyle != null)
                {
                    ActiveTabControl.Style = tabStyle;
                }
            }
        }

        /// <summary>
        /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)"/>, and returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <returns>
        /// The requested child element. This should not return null; if the provided index is out of range, an exception is thrown.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            return Content;
        }

        /// <summary>
        /// Gets an enumerator for logical child elements of this element.
        /// </summary>
        /// <value></value>
        /// <returns>An enumerator for logical child elements of this element.</returns>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                yield return Content;
            }
        }

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"/>-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            base.MeasureOverride(availableSize);

            if (null != Content)
            {
                if (double.IsInfinity(availableSize.Height))
                {
                    availableSize.Height = RenderSize.Height;
                }

                if (double.IsInfinity(availableSize.Width))
                {
                    availableSize.Width = RenderSize.Width;
                }

                Content.Measure(availableSize);
                return Content.DesiredSize;
            }



            return new Size(0, 0);
        }

        /// <summary>
        /// Enables the disable header edit.
        /// </summary>
        /// <param name="isedit">if set to <c>true</c> [isedit].</param>
        internal void EnableDisableHeaderEdit(bool isedit)
        {
            foreach (DocumentTabControl tab in m_TabList)
            {
                tab.EnableLabelEdit = isedit;
            }
        }

        internal void SetTDIHide()
        {
            if (Container != null)
            {
                foreach (DocumentTabControl tab in m_TabList)
                {
                    tab.HideHeaderOnSingleChild = Container.HideTDIHeaderOnSingleChild;
                }
            }
        }
        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (null != Content)
            {
                Content.Arrange(new Rect(EMPTY_POINT, finalSize));
            }

            return base.ArrangeOverride(finalSize);
        }

        private void SetDockingManagerFlag(bool flag,MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                if (Container != null && Container.IsInDockingManager && e.LeftButton == MouseButtonState.Pressed)
                {
                    DockingManager manager = VisualUtils.FindAncestor(Container, typeof(DockingManager)) as DockingManager;
                    if (manager != null)
                    {
                        manager.m_activeflag = flag;
                    }
                }
            }
        }

#if !SyncfusionFramework3_5
        private void SetDockingManagerFlag(bool flag, TouchEventArgs e)
        {
            if (Container != null && Container.IsInDockingManager && Container.m_documentContainerSystemGesture == SystemGesture.Drag)
            {
                DockingManager manager = VisualUtils.FindAncestor(Container, typeof(DockingManager)) as DockingManager;
                if (manager != null)
                {
                    manager.m_activeflag = flag;
                }
            }

        }
#endif

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                base.OnMouseLeftButtonUp(e);
                if (e.Source is TabItemExt)
                {
                    if (!Container.IsInDockingManager)
                    {
                        TabItemExt item = (TabItemExt)e.Source;
                        oldactive = null;
                        Container.ActiveDocument = GetContent(item);
                        ActiveTabControl = item.Parent as DocumentTabControl;

                    }

                }
            }
        }
        /// <summary>
        /// Invoked when an unhandled PreviewMouseDown attached routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that one or more mouse buttons were pressed.</param>
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                MouseButtonState state = e.LeftButton;
                MouseButtonState IsMouseRightState = e.RightButton;
                base.OnPreviewMouseDown(e);
                if (e.Source is TabItemExt)
                {
                    DockingManager manager = VisualUtils.FindAncestor(this, typeof(DockingManager)) as DockingManager;
                    if (Container.IsInDockingManager)
                    {
                        TabItemExt item = (TabItemExt)e.Source;
                        SetDockingManagerFlag(false, e);
                        Container.ActiveDocument = GetContent(item);
                        SetDockingManagerFlag(true, e);
                    }
                    else
                    {
                        TabItemExt item = (TabItemExt)e.Source;
                        SetDockingManagerFlag(false, e);
                        if (IsMouseRightState != MouseButtonState.Pressed && state != MouseButtonState.Pressed)
                            Container.ActiveDocument = GetContent(item);
                        SetDockingManagerFlag(true, e);
                    }
                }
                else if (e.Source is TabControlExt)
                {
                    TabControlExt tabCon = (TabControlExt)e.Source;

                    if (null != tabCon.SelectedItem)
                    {
                        Container.ActiveDocument = GetContent((Control)tabCon.SelectedItem);
                    }
                }
                else if (e.Source is ContentPresenter)
                {
                    TabItemExt item = (TabItemExt)(e.Source as ContentPresenter).Parent;
                    Container.ActiveDocument = GetContent(item);
                }
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            DockingManager docking = (DockingManager)VisualUtils.FindAncestor(this, typeof(DockingManager));
            if (docking != null)
            {
                docking.mousestartpoint = e.GetPosition(this);
            }
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                DockingManager docking = (DockingManager)VisualUtils.FindAncestor(this, typeof(DockingManager));
                if (docking != null)
                {
                    docking.OnMouseMoveOnTDILayoutPanel(this, e);
                }
                base.OnMouseMove(e);
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                DockingManager owner = (DockingManager)VisualUtils.FindAncestor(this, typeof(DockingManager));
                if (owner != null)
                {
                    owner.OnMouseLeaveOnTDILayoutPanel(this, e);
                }
                base.OnMouseLeave(e);
            }
        }

#if !SyncfusionFramework3_5
        protected override void OnTouchMove(TouchEventArgs e)
        {
            DockingManager owner = (DockingManager)VisualUtils.FindAncestor(this, typeof(DockingManager));
            if (owner != null)
            {
                if (owner.IsTouchEnabled)
                {
                    HeaderPanel headerpanel = VisualUtils.FindDescendant(this as Visual, typeof(HeaderPanel)) as HeaderPanel;
                    Point mousepoint = e.GetTouchPoint(headerpanel).Position;
                    HitTestResult testresult = (headerpanel != null) ? VisualTreeHelper.HitTest(headerpanel, mousepoint) : null;
                    if (headerpanel != null && mousepoint.Y > headerpanel.ActualHeight)
                        owner.OnTouchMoveOnTDILayoutPanel(this, e);
                }
            }
            base.OnTouchMove(e);
        }

        protected override void OnTouchLeave(TouchEventArgs e)
        {
            DockingManager owner = (DockingManager)VisualUtils.FindAncestor(this, typeof(DockingManager));
            if (owner != null)
            {
                if (owner.IsTouchEnabled)
                {
                    owner.OnTouchLeaveOnTDILayoutPanel(this, e);
                }
            }
            base.OnTouchLeave(e);
        }

        protected override void OnTouchUp(TouchEventArgs e)
        {
            if (Container != null && Container.IsTouchEnabled)
            {
                #region OnTouchLeftFingerUp
                if (Container.m_documentContainerSystemGesture == SystemGesture.Tap)
                {
                    OnTouchLeftFingerUp(e);
                }
                #endregion
            }
            base.OnTouchUp(e);
        }

        private void OnTouchLeftFingerUp(TouchEventArgs e)
        {
            if (e.Source is TabItemExt)
            {
                if (!Container.IsInDockingManager)
                {
                    TabItemExt item = (TabItemExt)e.Source;
                    oldactive = null;
                    Container.ActiveDocument = GetContent(item);
                    ActiveTabControl = item.Parent as DocumentTabControl;

                }

            }
        }

        protected override void OnPreviewTouchDown(TouchEventArgs e)
        {
            if (Container != null && Container.IsTouchEnabled)
            {
                base.OnPreviewTouchDown(e);


            }
        }

        protected override void OnPreviewTouchUp(TouchEventArgs e)
        {
            if (Container != null && Container.IsTouchEnabled)
            {
                if (e.Source is TabItemExt)
                {
                    DockingManager manager = VisualUtils.FindAncestor(this, typeof(DockingManager)) as DockingManager;
                    if (Container.IsInDockingManager)
                    {
                        TabItemExt item = (TabItemExt)e.Source;
                        SetDockingManagerFlag(false, e);
                        Container.ActiveDocument = GetContent(item);
                        SetDockingManagerFlag(true, e);
                    }
                    else
                    {
                        TabItemExt item = (TabItemExt)e.Source;
                        SetDockingManagerFlag(false, e);
                        if (Container.m_documentContainerSystemGesture != SystemGesture.Drag && Container.m_documentContainerSystemGesture != SystemGesture.RightDrag)
                            Container.ActiveDocument = GetContent(item);
                        SetDockingManagerFlag(true, e);
                    }
                }
                else if (e.Source is TabControlExt)
                {
                    TabControlExt tabCon = (TabControlExt)e.Source;

                    if (null != tabCon.SelectedItem)
                    {
                        Container.ActiveDocument = GetContent((Control)tabCon.SelectedItem);
                    }
                }
                else if (e.Source is ContentPresenter)
                {
                    TabItemExt item = (TabItemExt)(e.Source as ContentPresenter).Parent;
                    Container.ActiveDocument = GetContent(item);
                }
                base.OnPreviewTouchUp(e);
            }
        }
#endif

        /// <summary>
        /// Gets the tab item.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        internal TabItemExt GetTabItem(UIElement element)
        {
            foreach (TabControlExt tabcontrolext in m_TabList)
            {
                foreach (TabItemExt item in tabcontrolext.Items)
                {
                    ContentPresenter presenter = item.Content as ContentPresenter;
                    if (element == presenter.Content)
                    {
                        return item;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Sets the command bindings.
        /// </summary>
        private void SetCommnadBindings()
        {
            CommandBinding newHorizontalTabCommandBinding = new CommandBinding(DocumentTabControl.NewHorizontalTabGroupCommand, new ExecutedRoutedEventHandler(ExecuteNewHorizontalTabCommand), new CanExecuteRoutedEventHandler(CanExecuteTabCommand));

            CommandBinding newVerticalTabCommandBinding = new CommandBinding(DocumentTabControl.NewVerticalTabGroupCommand, new ExecutedRoutedEventHandler(ExecuteNewVerticalTabCommand), new CanExecuteRoutedEventHandler(CanExecuteTabCommand));

            CommandBinding moveToNextTabGroupCommandBinding = new CommandBinding(DocumentTabControl.MoveToNextTabGroupCommand, new ExecutedRoutedEventHandler(ExecuteMoveToNextTabGroupCommand), new CanExecuteRoutedEventHandler(CanExecuteMoveToNextTabGroupCommand));

            CommandBinding moveToPreviousTabGroupCommandBinding = new CommandBinding(DocumentTabControl.MoveToPreviousTabGroupCommand, new ExecutedRoutedEventHandler(ExecutemoveToPreviousTabGroupCommand), new CanExecuteRoutedEventHandler(CanExecuteMoveToPreviousTabGroupCommand));

            CommandBindings.Add(newHorizontalTabCommandBinding);
            CommandBindings.Add(newVerticalTabCommandBinding);
            CommandBindings.Add(moveToNextTabGroupCommandBinding);
            CommandBindings.Add(moveToPreviousTabGroupCommandBinding);
        }

        /// <summary>
        /// Executes the new horizontal tab command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ExecuteNewHorizontalTabCommand(object sender, ExecutedRoutedEventArgs e)
        {
            m_activeTabControl.m_Flag = false;
            ExecuteNewTabCommand(e, Orientation.Horizontal);
            m_activeTabControl.m_Flag = true;
        }

        /// <summary>
        /// Executes the new vertical tab command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ExecuteNewVerticalTabCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ExecuteNewTabCommand(e, Orientation.Vertical);
        }

        /// <summary>
        /// Executes the new tab command.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        /// <param name="orientation">The orientation.</param>
        private void ExecuteNewTabCommand(ExecutedRoutedEventArgs e, Orientation orientation)
        {

            UIElement element = e.Parameter as UIElement;
            DocumentTabControl tabControl = e.Source as DocumentTabControl;
#if DEBUG
            if (null == element || null == tabControl)
            {
                throw new ArgumentException();
            }
#endif

            Style tabStyle = null;

            tabStyle = (Style)DocumentContainer.GetDocumentTabControlStyle(ActiveTabControl) ??
                    (Style)DocumentContainer.GetDocumentTabControlStyle(Container);
            if (tabStyle != null)
            {
                tabControl.Style = tabStyle;
            }

            CreateNewTab(orientation, element, tabControl);
        }

        /// <summary>
        /// Creates the tab group.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="orientation">The orientation.</param>
        internal void CreateTabGroup(UIElement element, Orientation orientation)
        {
            if (m_TabList.Count > 0 && m_activeTabControl!=null)
            {
                DocumentTabControl tabControl = m_activeTabControl;
                if (!Container.Items.Contains(element))
                {
                    Container.Items.Add(element);
                    DragDropNewTab(element, tabControl, orientation);
                }
                else
                {
                    DocumentTabControl elementtab = VisualUtils.FindAncestor(element, typeof(DocumentTabControl)) as DocumentTabControl;
                    if (elementtab != null)
                    {
                        if (elementtab.Items.Count > 1)
                        {
                            DragDropNewTab(element, tabControl, orientation);
                        }
                    }
                    else
                    {
                        DragDropNewTab(element, tabControl, orientation);
                    }
                }

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="tabControl"></param>
        /// <param name="orientation"></param>
        internal void DragDropNewTab(UIElement element, DocumentTabControl tabControl, Orientation orientation)
        {
#if DEBUG
            if (null == element || null == tabControl)
            {
                throw new ArgumentException();
            }
#endif

            Style tabStyle = null;

            tabStyle = (Style)DocumentContainer.GetDocumentTabControlStyle(ActiveTabControl) ??
                    (Style)DocumentContainer.GetDocumentTabControlStyle(Container);
            if (tabStyle != null)
            {
                tabControl.Style = tabStyle;
            }

            CreateNewTab(orientation, element, tabControl);
        }

        /// <summary>
        /// Called when [items changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateActiveItems(e);
           UpdateVisualTree((DocumentTabControl)sender);
        }

        /// <summary>
        /// Updates the visual tree.
        /// </summary>
        /// <param name="tabControl">The tab control.</param>
        private void UpdateVisualTree(DocumentTabControl tabControl)
        {
            if (0 == tabControl.Items.Count)
            {
                PanelBase parent = (PanelBase)tabControl.Parent;

                if (parent is TDISplitPanel)
                {
                    PanelBase parentOfParent = (PanelBase)parent.Parent;
                    UIElement opposite = parent.GetOppositeElement(tabControl);
                    parent.RemoveElement(tabControl);
                    parent.RemoveElement(opposite);
                    parentOfParent.RemoveElement(parent);
                    parentOfParent.AddElement(opposite);
                    m_TabList.Remove(tabControl);
                    DisposeTabControl(tabControl);
                    foreach (DocumentTabControl tabcontrol in m_TabList)
                    {
                        ItemsUpdate(tabcontrol);
                        if (DocumentContainer.GetDocumentTabControlStyle(Container) != null)
                        {
                            tabcontrol.Style = DocumentContainer.GetDocumentTabControlStyle(Container) as Style;
                        }
                        else if (Container.IsInDockingManager
                            && DockingManager.GetDocumentTabControlStyle(Container.FlipParent as DockingManager)!=null)
                        {
                            tabcontrol.Style = (Style)DockingManager.GetDocumentTabControlStyle(Container.FlipParent as DockingManager);
                        }
                    }
                    if (tabControl == ActiveTabControl)
                    {
                        ActiveTabControl = m_TabList[0];
                    }

                    if (0 < ActiveTabControl.Items.Count)
                    {
                        TabItem item = null;
                        if (ActiveTabControl.SelectedItem != null)
                        {
                            item = ActiveTabControl.SelectedItem as TabItem;
                        }
                        else
                        {
                            item = (TabItem)ActiveTabControl.Items[0];
                        }
                        Container.ActiveDocument = GetContent(item);
                    }
                }
            }
        }

        /// <summary>
        /// Update active's tab items.
        /// </summary>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void UpdateActiveItems(NotifyCollectionChangedEventArgs e)
        {
            DragDropHelper helper = DragDropHelper.GetInstance();

            if (helper.IsDragging && e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (DocumentTabControl tabcontrol in m_TabList)
                {
                    ItemsUpdate(tabcontrol);
                    if (DocumentContainer.GetDocumentTabControlStyle(Container) != null)
                    {
                        tabcontrol.Style = DocumentContainer.GetDocumentTabControlStyle(Container) as Style;
                    }
                    else if (Container.IsInDockingManager
                        && DockingManager.GetDocumentTabControlStyle(Container.FlipParent as DockingManager)!=null)
                    {
                        tabcontrol.Style = (Style)DockingManager.GetDocumentTabControlStyle(Container.FlipParent as DockingManager);
                    }
                }
                ActiveTabControl.SelectedItem = helper.DraggedItem;
            }
        }

        /// <summary>
        /// Called when [tab control got keyboard focus].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyboardFocusChangedEventArgs"/> instance containing the event data.</param>
        private void OnTabControlGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender != ActiveTabControl)
            {
                ActiveTabControl = (DocumentTabControl)sender;
                Container.ActiveDocument = GetContent((Control)ActiveTabControl.SelectedItem);
            }

            SetActiveWindow(ActiveTabControl);
        }

        /// <summary>
        /// Initializes the tab control.
        /// </summary>
        /// <param name="tabControl">The tab control.</param>
        private void InitializeTabControl(TabControlExt tabControl)
        {

            tabControl.ItemsChanged += new NotifyCollectionChangedEventHandler(OnItemsChanged);
            tabControl.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnTabControlGotKeyboardFocus);
            tabControl.TakeDragItemEvent += new TakeDragItemHandler(OnTabControlTakeDragItemEvent);
            tabControl.OnCloseAllTabs += new OnCloseTabsEventHandler(OnTabControlOnCloseAllTabs);
            tabControl.OnCloseOtherTabs += new OnCloseTabsEventHandler(OnTabControlOnCloseOtherTabs);
            tabControl.SelectedItemChangedEvent += new SelectedItemChangedEventHandler(OnTabControlSelectedItemChanged);
            tabControl.TabClosing += new CancelingRoutedEventHandler(OnTabControlTabClosing);

        }

        /// <summary>
        /// Called when [tab control tab closing].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.Controls.CancelingRoutedEventArgs"/> instance containing the event data.</param>
        private void OnTabControlTabClosing(object sender, CancelingRoutedEventArgs e)
        {
            e.Cancel = DocumentContainer.CanceledClosed(this, GetContent((Control)e.OriginalSource));
        }

        /// <summary>
        /// Called when [tab control selected item changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.Controls.SelectedItemChangedEventArgs"/> instance containing the event data.</param>
        private void OnTabControlSelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            if (!Container.LoadingPersistState && e.NewSelectedItem != null && !m_changingGroup && IsLoaded)
            {
                TabItemExt item = (TabItemExt)e.NewSelectedItem;
                if (Container.ActiveDocument != null)
                {
                    Container.ActiveDocument.Focus();
                }
            }
        }

        /// <summary>
        /// Called when [tab control on close other tabs].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.Controls.CloseTabEventArgs"/> instance containing the event data.</param>
        private void OnTabControlOnCloseOtherTabs(object sender, CloseTabEventArgs e)
        {
            Container.RaiseCloseOtherTabs(sender, e);
        }

        public override void OnApplyTemplate()
        {
              
            base.OnApplyTemplate();

            if (SkinStorage.GetVisualStyle(this) == "Office2007Blue")
            {
                ResourceDictionary rd = new ResourceDictionary();
                rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2007BlueStyle.xaml", UriKind.RelativeOrAbsolute);
                TDILayoutPanel paneladv = VisualUtils.FindDescendant(this, typeof(TDILayoutPanel)) as TDILayoutPanel;

                if (paneladv != null)
                {
                    SkinStorage.SetVisualStyle(paneladv, "Office2007Blue");
                    paneladv.Style = rd["TabPanelAdvStyle"] as Style;
                }
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == FrameworkElement.FlowDirectionProperty)
            {
                if (ActiveTabControl != null)
                    ActiveTabControl.FlowDirection = (FlowDirection)e.NewValue;
            }
        }

        /// <summary>
        /// Called when [tab control on close all tabs].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.Controls.CloseTabEventArgs"/> instance containing the event data.</param>
        private void OnTabControlOnCloseAllTabs(object sender, CloseTabEventArgs e)
        {
            Container.RaiseCloseAllTabsEvent(sender, e);
        }

        /// <summary>
        /// Disposes the tab control.
        /// </summary>
        /// <param name="tabControl">The tab control.</param>
        private void DisposeTabControl(DocumentTabControl tabControl)
        {
            string way = (string)tabControl.GetValue(TDILayoutPanel.WayOfTDIGroupProperty);
            int point = way.Length - 1;

            if (-1 < point)
            {
                m_TabList.ForEach(delegate(DocumentTabControl item)
                {
                    string itemWay = (string)item.GetValue(TDILayoutPanel.WayOfTDIGroupProperty);
                    if (point < itemWay.Length)
                    {
                        bool result = ContainsWays(way, itemWay, point);

                        if (result)
                        {
                            itemWay = itemWay.Remove(point, 1);
                            item.SetValue(TDILayoutPanel.WayOfTDIGroupProperty, itemWay);
                        }
                    }
                });
            }

            tabControl.ItemsChanged -= new NotifyCollectionChangedEventHandler(OnItemsChanged);
            tabControl.GotKeyboardFocus -= new KeyboardFocusChangedEventHandler(OnTabControlGotKeyboardFocus);
            tabControl.TakeDragItemEvent -= new TakeDragItemHandler(OnTabControlTakeDragItemEvent);
            tabControl.OnCloseAllTabs -= new OnCloseTabsEventHandler(OnTabControlOnCloseAllTabs);
            tabControl.OnCloseOtherTabs -= new OnCloseTabsEventHandler(OnTabControlOnCloseOtherTabs);
            tabControl.SelectedItemChangedEvent -= new SelectedItemChangedEventHandler(OnTabControlSelectedItemChanged);
            tabControl.TabClosing -= new CancelingRoutedEventHandler(OnTabControlTabClosing);
            tabControl.Dispose();
        }

        /// <summary>
        /// Detaches from container.
        /// </summary>
        /// <param name="container">The container.</param>
        private void DetachFromContainer(IDocumentContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            INotifyCollectionChanged colNotifications = container.Items;

            if (colNotifications != null)
            {
                colNotifications.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnNotifyCollectionChanged);
            }

            for (int i = 0; i < m_TabList.Count; ++i)
            {
                DocumentTabControl tabCon = m_TabList[i];

                foreach (object item in tabCon.Items)
                {
                    tabCon.RemoveItem(item);
                }
            }
        }

        /// <summary>
        /// Attaches to container.
        /// </summary>
        /// <param name="container">The container.</param>
        private void AttachToContainer(IDocumentContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            INotifyCollectionChanged colNotifications = container.Items;

            if (colNotifications != null)
            {
                colNotifications.CollectionChanged += new NotifyCollectionChangedEventHandler(OnNotifyCollectionChanged);
            }

            DocumentCollection contItems = container.Items;
            int cnt = contItems.Count;
            IList items = new DocumentCollection();

            for (int i = cnt - 1; i > -1; --i)
            {
                if (contItems[i] is UIElement)
                {
                    ActiveTabControl.InsertItemExt((UIElement)contItems[i], 0);
                }
                else
                {
                    ContentControl control = new ContentControl();
                    control.DataContext = contItems[i];
                    ActiveTabControl.InsertItemExt((UIElement)control, 0);
                }
            }
        }

        /// <summary>
        /// Handles the CollectionChanged event of the notifyChange control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnNotifyCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IEnumerable enm = sender as IEnumerable;

            if (TemplatedParent == null)
            {
                return;
            }

            if (0 == m_TabList.Count)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    GenerateVisualsFromObject(e.NewItems);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    RemoveVisualsUsingObject(e.OldItems, enm);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    RemoveVisualsUsingObject(e.OldItems, enm);
                    foreach (object item in e.NewItems)
                    {
                        UIElement element = null;
                        if (item is UIElement)
                        {
                            element = item as UIElement;
                        }
                        else
                        {
                            element = new ContentControl();
                            (element as ContentControl).DataContext = item;
                        }
                        ActiveTabControl.InsertItemExt(element, 0);
                    }

                    break;

                case NotifyCollectionChangedAction.Reset:
                    if (e.NewItems != null)
                    {
                        ActiveTabControl.ResetItems(e.NewItems);
                    }
                    else
                    {
                        ActiveTabControl.ResetItems(enm);
                    }
                    break;
            }

            if (ActiveTabControl.Items.Count <= 0)
            {
                ActiveTabControl.IsAllTabsClosed = true;
                this.Visibility = Visibility.Collapsed;
            }
            else
            {
                ActiveTabControl.IsAllTabsClosed = false;
                this.Visibility = Visibility.Visible;
                if (Container!=null)
                {
                    bool canExecute = Container.IsInDockingManager ? Container.DockingManager.m_loadingState : Container.LoadingPersistState;
                    if (!canExecute)
                        ItemsUpdate(ActiveTabControl);
                }
            }
        }


        /// <summary>
        /// Removes the items.
        /// </summary>
        /// <param name="items">The items value.</param>
        /// <param name="itms">The item value.</param>
        private void RemoveItems(IList items, IEnumerable itms)
        {
            foreach (UIElement item in items)
            {
                for (int i = m_TabList.Count - 1; i > -1; --i)
                {
                    DocumentTabControl tabControl = m_TabList[i];

                    if (tabControl.ContainsItem(item))
                    {
                        tabControl.RemoveItem(item);
                    }
                }
            }
        }

        /// <summary>
        /// Generates the visuals from object.
        /// </summary>
        /// <param name="items">The items.</param>
        private void GenerateVisualsFromObject(IList items)
        {
            foreach (object item in items)
            {
                UIElement element = null;

                if (item is UIElement)
                {
                    element = item as UIElement;
                }
                else
                {
                    element = new ContentControl();
                    (element as ContentControl).DataContext = item;
                }

                if (Container != null && Container.AddTabDocumentAtLast)
                {
                    TabLayoutPanel.AddAtLast = true;
                    if (m_WayOfTDIFlag)
                    {
                        Orientation orientation = (Orientation)ActiveTabControl.GetValue(TDILayoutPanel.TDIGroupOrientationProperty);
                        string way = (string)ActiveTabControl.GetValue(TDILayoutPanel.WayOfTDIGroupProperty);
                        FrameworkElement tabitem = item as FrameworkElement;
                        tabitem.SetValue(TDILayoutPanel.TDIGroupOrientationProperty, orientation);
                        tabitem.SetValue(TDILayoutPanel.WayOfTDIGroupProperty, way);
                    }
                    else
                    {
                        Orientation orientation = (Orientation)ActiveTabControl.GetValue(TDILayoutPanel.TDIGroupOrientationProperty);
                        string way = (string)ActiveTabControl.GetValue(TDILayoutPanel.WayOfTDIGroupProperty);
                        int length = way.Length;
                        string newWay = way.Insert(length, TRUE_BIT_EX);
                        FrameworkElement tabitem = item as FrameworkElement;
                        if (!(newWay.Length > 1))
                        {
                            tabitem.SetValue(TDILayoutPanel.TDIGroupOrientationProperty, orientation);
                            tabitem.SetValue(TDILayoutPanel.WayOfTDIGroupProperty, newWay);
                        }
                        else
                        {
                            tabitem.SetValue(TDILayoutPanel.TDIGroupOrientationProperty, orientation);
                            tabitem.SetValue(TDILayoutPanel.WayOfTDIGroupProperty, way);
                        }
                    }
                    int insertindex = ActiveTabControl.Items.Count;
                    ActiveTabControl.InsertItemExt(element, insertindex);
                }
                else
                {
                    ActiveTabControl.InsertItemExt(element, 0);
                    ActiveTabControl.SelectedIndex = 0;
                }

                DockingManager docking = DockingManager.ResolveManager(element);
                if (docking != null)
                {
                    ActiveWindowChangingEventArgs args = new ActiveWindowChangingEventArgs();
                    args.NewValue = (FrameworkElement)element;
                    args.OldValue = docking.ActiveWindow;
                    if (args.OldValue != args.NewValue)
                    {
                        docking.FireActiveWindowChanging(args.NewValue, args);
                        if (!args.Cancel)
                        {
                            DockingManager.SetNewFocusedElement(element as FrameworkElement);
                        }
                    }
                }
            }
        }

        internal void RestoreTDI(UIElement element)
        {
            if (ActiveTabControl != null)
            {
                DockingManager.SetState(element, DockState.Document);
                ActiveTabControl.InsertItemExt(element, 0);
            }
        }

        /// <summary>
        /// Removes the visuals using object.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="itms">The itms.</param>
        private void RemoveVisualsUsingObject(IList items, IEnumerable itms)
        {
            bool _lflag = false;
            List<FrameworkElement> controllist = new List<FrameworkElement>();

            foreach (object obj in items)
            {
                if (obj is UIElement)
                {
                    RemoveItems(items, itms);
                    break;
                }
                else
                {
                    for (int i = m_TabList.Count - 1; i > -1; --i)
                    {
                        DocumentTabControl tabControl = m_TabList[i];

                        foreach (TabItemExt item in tabControl.Items)
                        {
                            if ((item.Content as FrameworkElement).DataContext == obj)
                            {
                                controllist.Add(item.Content as FrameworkElement);
                                _lflag = true;
                            }
                        }
                    }
                }
                if (_lflag)
                {
                    RemoveItems(controllist, itms);
                }

            }
        }


        /// <summary>
        /// Executes the move to next tab group command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ExecuteMoveToNextTabGroupCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ExecuteMoveToTabGroupCommand(e, true);
        }

        /// <summary>
        /// Execute moves to previous tab group command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ExecutemoveToPreviousTabGroupCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ExecuteMoveToTabGroupCommand(e, false);
        }

        /// <summary>
        /// Executes the move to tab group command.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        /// <param name="next">if set to <c>true</c> [next].</param>
        private void ExecuteMoveToTabGroupCommand(ExecutedRoutedEventArgs e, bool next)
        {
            UIElement element = e.Parameter as UIElement;
            DocumentTabControl tabControl = e.Source as DocumentTabControl;
#if DEBUG
            if (null == element || null == tabControl)
            {
                throw new ArgumentException();
            }
#endif
            PanelBase parent = (PanelBase)tabControl.Parent;
            DocumentTabControl nextTabControl = GetNextElement(tabControl, parent, next);

            if (null != nextTabControl)
            {
                m_changingGroup = true;
                tabControl.RemoveItem(element);
                nextTabControl.AddItemContent(element);
                foreach (DocumentTabControl tabcontrol in m_TabList)
                {
                    ItemsUpdate(tabcontrol);
                    if (DocumentContainer.GetDocumentTabControlStyle(Container) != null)
                    {
                        tabcontrol.Style = DocumentContainer.GetDocumentTabControlStyle(Container) as Style; ;
                    }
                    else if (Container.IsInDockingManager
                        && DockingManager.GetDocumentTabControlStyle(Container.FlipParent as DockingManager) != null)
                    {
                        tabcontrol.Style = (Style)DockingManager.GetDocumentTabControlStyle(Container.FlipParent as DockingManager);
                    }
                }
                if (DocumentContainer.GetDocumentTabControlStyle(Container) != null)
                {
                    nextTabControl.Style = DocumentContainer.GetDocumentTabControlStyle(Container) as Style;
                    tabControl.Style = DocumentContainer.GetDocumentTabControlStyle(Container) as Style;
                }
                else if (Container.IsInDockingManager
                    && DockingManager.GetDocumentTabControlStyle(Container.FlipParent as DockingManager)!=null)
                {
                    nextTabControl.Style = (Style)DockingManager.GetDocumentTabControlStyle(Container.FlipParent as DockingManager);
                    tabControl.Style = (Style)DockingManager.GetDocumentTabControlStyle(Container.FlipParent as DockingManager); 
                }
                ActiveTabControl = nextTabControl;
                m_changingGroup = false;

                TabGroupEventArgs args = new TabGroupEventArgs()
                {
                    Orientation = TDILayoutPanel.GetTDIGroupOrientation(nextTabControl),
                    PreviousTabGroup = (tabControl.Items.Count > 0) ? tabControl : null,
                    CurrentTabGroup = nextTabControl,
                    TargetItem = element
                };
                Container.FireMoveToOtherTabGroup(args);
            }
        }

        internal void ExecuteAddElementToTabGroup(DocumentTabControl targettabcontrol,DocumentTabControl oldtabcontrol, UIElement Element)
        {
            UIElement element = Element;
            DocumentTabControl tabControl = oldtabcontrol;

            PanelBase parent = (PanelBase)tabControl.Parent;
            DocumentTabControl nextTabControl = targettabcontrol;

            if (null != nextTabControl)
            {
                m_changingGroup = true;
                tabControl.RemoveItem(element);
                nextTabControl.AddItemContent(element);
                foreach (DocumentTabControl tabcontrol in m_TabList)
                {
                    ItemsUpdate(tabcontrol);
                    if (DocumentContainer.GetDocumentTabControlStyle(Container) != null)
                    {
                        tabcontrol.Style = DocumentContainer.GetDocumentTabControlStyle(Container) as Style; ;
                    }
                    else if (Container.IsInDockingManager
                        && DockingManager.GetDocumentTabControlStyle(Container.FlipParent as DockingManager) != null)
                    {
                        tabcontrol.Style = (Style)DockingManager.GetDocumentTabControlStyle(Container.FlipParent as DockingManager);
                    }
                }
                if (DocumentContainer.GetDocumentTabControlStyle(Container) != null)
                {
                    nextTabControl.Style = DocumentContainer.GetDocumentTabControlStyle(Container) as Style;
                    tabControl.Style = DocumentContainer.GetDocumentTabControlStyle(Container) as Style;
                }
                else if (Container.IsInDockingManager
                    && DockingManager.GetDocumentTabControlStyle(Container.FlipParent as DockingManager) != null)
                {
                    nextTabControl.Style = (Style)DockingManager.GetDocumentTabControlStyle(Container.FlipParent as DockingManager);
                    tabControl.Style = (Style)DockingManager.GetDocumentTabControlStyle(Container.FlipParent as DockingManager);
                }
                ActiveTabControl = nextTabControl;
                m_changingGroup = false;

                TabGroupEventArgs args = new TabGroupEventArgs()
                {
                    Orientation = TDILayoutPanel.GetTDIGroupOrientation(nextTabControl),
                    PreviousTabGroup = (tabControl == nextTabControl) ? null : tabControl,
                    CurrentTabGroup = nextTabControl,
                    TargetItem = element
                };
                Container.FireMoveToOtherTabGroup(args);
            }
        }

        private void PrepareTabPositionCache(UIElement element)
        {
            DocumentTabControl tabControl = element as DocumentTabControl;
            SortedList<int, UIElement> list = new SortedList<int, UIElement>();
            List _keylist = new List();
            if (tabControl != null)
            {
                DockingManager dockingmanager = Container.DockingManager;
                string _wayofTDIGroup = (string)tabControl.GetValue(TDILayoutPanel.WayOfTDIGroupProperty);
                if (dockingmanager != null)
                {
                    foreach (FrameworkElement child in dockingmanager.Children)
                    {
                        if (_wayofTDIGroup.Equals((string)child.GetValue(TDILayoutPanel.WayOfTDIGroupProperty)))
                        {
                            if (DockingManager.GetDocumentTabOrderIndex(child) != -1)
                            {
                                if (!list.ContainsKey(DockingManager.GetDocumentTabOrderIndex(child)))
                                    list.Add(DockingManager.GetDocumentTabOrderIndex(child), child);
                            }
                        }
                    }
                    if (list != null && list.Count > 0)
                    {
                        tabControl.TabPositionCache.Clear();
                        for (int i = 0; i < list.Keys.Count; i++)
                        {
                            tabControl.TabPositionCache.Add(list[list.Keys[i]]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Builds the tab group.
        /// </summary>
        private void BuildTabGroup()
        {
            WaysMap waysMap = CreateWaysMap();

            if (waysMap.Count == 0)
            {
                return;
            }
            KeysList keysList = waysMap.GetMaxWaysList();

            if (Container.m_Group)
            {
                ClearTabControls(m_TabList);
                BuildTabGroup(waysMap);
            }
            else if (keysList.Count > 1)
            {
                ClearTabControls(m_TabList);
                BuildTabGroup(waysMap);
            }
        }
        

        /// <summary>
        /// Builds the tab group.
        /// </summary>
        /// <param name="waysMap">The ways map.</param>
        private void BuildTabGroup(WaysMap waysMap)
        {
            while (1 != waysMap.Count)
            {
                KeysList keysList = waysMap.GetMaxWaysList();

                for (int i = 0, cnt = keysList.Count; i < cnt; i += 2)
                {
                    List<bool> firstWay = keysList[i];
                    Orientation groupOrientation;
                    UIElement fElement = GetItem(waysMap, firstWay, out groupOrientation);
                    PrepareTabPositionCache(fElement);
                    List<bool> secondWay = null;
                    Orientation orientation=Orientation.Horizontal;
                    UIElement sElement = null;
                    if (keysList.Count >= 1)
                    {
                        if (keysList.Count > (i + 1))
                        {
                            secondWay = keysList[i + 1];
                            sElement = GetItem(waysMap, secondWay, out orientation);
                            PrepareTabPositionCache(sElement);
                        }
                    }
                    if (fElement != null && sElement != null)
                    {
                        TDISplitPanel panel = new TDISplitPanel(fElement, sElement, orientation)
                        {
                            Name = string.Concat(NAME_PREFIX, ++m_nameSufix)
                        };
                        panel.SetValue(TDILayoutPanel.TDIGroupOrientationProperty, groupOrientation);
                        List<bool> nodeList = new List<bool>(firstWay);
                        nodeList.RemoveAt(firstWay.Count - 1);
                        panel.SetValue(TDILayoutPanel.WayOfTDIGroupProperty, ListBoolToStringConvert(nodeList));

                        waysMap.RemoveWay(firstWay);
                        if (secondWay != null)
                        {
                            waysMap.RemoveWay(secondWay);
                        }
                        waysMap.AddToWaysMap(nodeList, panel);
                    }
                }
            }

            foreach (List<bool> itemWay in waysMap.Keys)
            {
                List<bool> rootWay = itemWay;
                Orientation elOrientation;
                UIElement uiElement = GetItem(waysMap, rootWay, out elOrientation);
                PrepareTabPositionCache(uiElement);
                RemoveElement(Content);
                AddElement(uiElement);
            }
        }

        /// <summary>
        /// Creates the ways map.
        /// </summary>
        /// <returns>WaysMap waysMap</returns>
        private WaysMap CreateWaysMap()
        {
            WaysMap waysMap = new WaysMap();
            if (Container != null)
            {
                foreach (UIElement element in Container.Items)
                {
                    if (DockState.Hidden != DockingManager.GetState(element))
                    {
                        List<bool> way = StringToBoolList((string)element.GetValue(WayOfTDIGroupProperty));
                        waysMap.AddToWaysMap(way, element);
                    }
                }
            }

            return waysMap;
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="waysMap">The ways map.</param>
        /// <param name="way">The way result.</param>
        /// <param name="orientation">The orientation.</param>
        /// <returns>UIElement orientation</returns>
        private UIElement GetItem(WaysMap waysMap, List<bool> way, out Orientation orientation)
        {
            UIElement returnElement;
            GroupInformation gInfo;
            waysMap.TryGetValue(way, out gInfo);
            List<UIElement> uiList = gInfo.ElementsList;
            orientation = gInfo.Orientation;

            if (NeedConcatInTabGroup(uiList))
            {
                DocumentTabControl tabControl = new DocumentTabControl
                {
                    Background = Brushes.Transparent,
                    EnableLabelEdit = false,
                    Container = Container,
                    Name = string.Concat(NAME_PREFIX, ++m_nameSufix)
                };

                if (DocumentContainer.GetDocumentTabControlStyle(Container) != null)
                {
                    tabControl.Style = (Style)DocumentContainer.GetDocumentTabControlStyle(Container);
                }
                else if (Container.IsInDockingManager
                    && DockingManager.GetDocumentTabControlStyle(Container.FlipParent as DockingManager)!=null)
                {
                    tabControl.Style = (Style)DockingManager.GetDocumentTabControlStyle(Container.FlipParent as DockingManager);
                }
                tabControl.SetValue(TDILayoutPanel.TDIGroupOrientationProperty, orientation);
                tabControl.SetValue(TDILayoutPanel.WayOfTDIGroupProperty, ListBoolToStringConvert(way));

                for (int j = 0, uicnt = uiList.Count; j < uicnt; ++j)
                {
                    tabControl.AddItemContent(uiList[j]);
                }

                m_TabList.Add(tabControl);
                InitializeTabControl(tabControl);
                ActiveTabControl = tabControl;
                returnElement = tabControl;
            }
            else
            {
                returnElement = uiList[0];
            }

            return returnElement;
        }

        /// <summary>
        /// Sets the container.
        /// </summary>
        internal void SetContainer()
        {
            DocumentTabControl tabControl = Content as DocumentTabControl;
            DocumentContainer templParent = (DocumentContainer)TemplatedParent;

            if (null != tabControl)
            {
                tabControl.Container = templParent;
            }

            if (null == Container)
            {
                Container = (DocumentContainer)TemplatedParent;
            }

            if (ActiveTabControl.IsLoaded && null != ActiveTabControl.SelectedItem)
            {
                Container.ActiveDocument = GetContent((Control)ActiveTabControl.SelectedItem);
            }
            SetTDIHide();

            if (tabControl != null && Container != null)
            {
                if (!Container.m_tabControlCollection.Contains(tabControl))
                    Container.m_tabControlCollection.Add(tabControl);

                BindingUtils.SetBinding(this, Container, TDILayoutPanel.FocusVisualStyleProperty, DocumentContainer.FocusVisualStyleProperty, BindingMode.OneWay);
                BindingUtils.SetBinding(tabControl, Container, TabControlExt.FocusVisualStyleProperty, DocumentContainer.FocusVisualStyleProperty, BindingMode.OneWay);
                BindingUtils.SetBinding(tabControl, Container, TabControlExt.IsLazyLoadedProperty, DocumentContainer.IsLazyLoadedProperty,BindingMode.OneWay);

                if (Container.m_showtablistcontextmenuchanged)
                    BindingUtils.SetBinding(tabControl, Container, TabControlExt.ShowTabListContextMenuProperty, DocumentContainer.ShowTabListContextMenuProperty);
                BindingUtils.SetBinding(tabControl, Container, TabControlExt.TabListContextMenuItemsProperty, DocumentContainer.TabListContextMenuItemsProperty);
                if (Container.m_collapsedefaulttablistcontextmenuitemschanged)
                    BindingUtils.SetBinding(tabControl, Container, TabControlExt.CollapseDefaultTabListContextMenuItemsProperty, DocumentContainer.CollapseDefaultTabListContextMenuItemsProperty);
                if (Container.m_showtabitemcontextmenuchanged)
                    BindingUtils.SetBinding(tabControl, Container, TabControlExt.ShowTabItemContextMenuProperty, DocumentContainer.ShowTabItemContextMenuProperty);
                if (Container.m_tdiclosebuttontypechanged)
                    BindingUtils.SetBinding(tabControl, Container, TabControlExt.CloseButtonTypeProperty,DocumentContainer.TDICloseButtonTypeProperty);
                if(Container.m_tdifullscreenmodechanged)
                    BindingUtils.SetBinding(tabControl, Container, TabControlExt.FullScreenModeProperty, DocumentContainer.TDIFullScreenModeProperty, BindingMode.OneWay);
                if(Container.m_tditoolbartraychanged)
                    BindingUtils.SetBinding(tabControl, Container, TabControlExt.ToolBarTrayProperty, DocumentContainer.TDIToolBarTrayProperty, BindingMode.OneWay);
            }
            if (tabControl != null)
            {
                for (int i = 0; i < tabControl.Items.Count; i++)
                {
                    if (tabControl.Items[i] is TabItemExt && ActiveTabControl.ItemTemplate != null)
                    {
                        (tabControl.Items[i] as TabItemExt).HeaderTemplate = ActiveTabControl.ItemTemplate;
                        if ((tabControl.Items[i] as TabItemExt).m_headerelement != null && (tabControl.Items[i] as TabItemExt).m_headerelement.Content.ToString() == string.Empty)
                            (tabControl.Items[i] as TabItemExt).m_headerelement.Content = (tabControl.Items[i] as TabItemExt).DataContext;
                    }
                } 
            }
          
        }

        /// <summary>
        /// Sets the selected item font weight.
        /// </summary>
        /// <param name="fontWeight">The font weight.</param>
        private void SetSelectedItemFontWeight(FontWeight fontWeight)
        {
            if (null != m_activeTabControl)
            {
                m_activeTabControl.ClearValue(TabControlExt.SelectedItemFontWeightProperty);
                m_activeTabControl.SelectedItemFontWeight = fontWeight;
            }
        }

        /// <summary>
        /// Determines whether this instance [can execute move to next tab group command] the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void CanExecuteMoveToNextTabGroupCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            DocumentTabControl tabControl = e.Source as DocumentTabControl;

            if (null != tabControl)
            {
                PanelBase parent = (PanelBase)tabControl.Parent;
                e.CanExecute = HasNextElement(tabControl, parent, true);
            }
        }

        /// <summary>
        /// Called when [layout updated].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            if (null == Container)
            {
                SetContainer();
                this.InvalidateMeasure();
                this.InvalidateArrange();
            }
        }

        /// <summary>
        /// Determines whether this instance [can execute move to previous tab group command] the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void CanExecuteMoveToPreviousTabGroupCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            DocumentTabControl tabControl = e.Source as DocumentTabControl;

            if (null != tabControl)
            {
                PanelBase parent = (PanelBase)tabControl.Parent;
                e.CanExecute = HasNextElement(tabControl, parent, false);
            }
        }

        /// <summary>
        /// Determines whether [has next element] [the specified current].
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="next">if set to <c>true</c> [next].</param>
        /// <returns>
        /// <c>true</c> if [has next element] [the specified current]; otherwise, <c>false</c>.
        /// </returns>
        private static bool HasNextElement(UIElement current, PanelBase parent, bool next)
        {
            bool result = parent.HasElement(current, next);

            if (!result)
            {
                result = null != GetRootPanel(current, parent, next);
            }

            return result;
        }

        /// <summary>
        /// Gets the next element.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="next">if set to <c>true</c> [next].</param>
        /// <returns>DocumentTab Control </returns>
        private static DocumentTabControl GetNextElement(UIElement current, PanelBase parent, bool next)
        {
            UIElement result1;

            if (parent.HasElement(current, next))
            {
                result1 = GetElement(parent, !next);
            }
            else
            {
                PanelBase rootParent = GetRootPanel(current, parent, next);
#if DEBUG
                if (null == rootParent)
                {
                    throw new NotSupportedException("Incorrect implementation");
                }
#endif
                result1 = GetElement(rootParent, !next);
            }

            return (DocumentTabControl)result1;
        }

        /// <summary>
        /// Determines whether this instance [can execute tab command] the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void CanExecuteTabCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            DocumentTabControl tabControl = e.Source as DocumentTabControl;

            if (null != tabControl)
            {
                e.CanExecute = 1 < tabControl.Items.Count;
            }
        }

        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="next">if set to <c>true</c> [next].</param>
        /// <returns>UIElement result</returns>
        private static UIElement GetElement(PanelBase parent, bool next)
        {
            UIElement result = parent.GetElement(next);

            while (result is TDISplitPanel)
            {
                parent = (PanelBase)result;
                result = parent.GetElement(!next);
            }

            return result;
        }

        /// <summary>
        /// Gets the root panel.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="next">if set to <c>true</c> [next].</param>
        /// <returns>PanelBase parent</returns>
        private static PanelBase GetRootPanel(UIElement current, PanelBase parent, bool next)
        {
            if (null == current)
            {
                throw new ArgumentException("current element");
            }

            PanelBase rootParent = null;

            while (parent is TDISplitPanel)
            {
                current = parent;
                parent = (PanelBase)parent.Parent;

                if (parent.HasElement(current, next))
                {
                    rootParent = parent;
                    break;
                }
            }

            return rootParent;
        }



        /// <summary>
        /// Called when [TDI index changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTDIIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (DockingManager.GetState(d) == DockState.Document)
            {
                FrameworkElement element = (FrameworkElement)d;
                DockingManager docking = element.Parent as DockingManager;
                if (docking != null && !docking.canUpdateTDIindex && !docking.updateTDIindexInternal)
                    docking.canUpdateTDIindex = true;
                if (DockingManager.GetTabControl(d) != null)
                {
                    DocumentTabControl tabcontrol = (DockingManager.GetTabControl(d) as DocumentTabControl);
                    if(tabcontrol.m_canupdateindex)
                    {
                        tabcontrol.UpdateIndex((int)args.NewValue, d as FrameworkElement);
                        tabcontrol.UpdateIndexOrder();
                    }
                }
            }
        }

        /// <summary>
        /// Called when [is selected changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            FrameworkElement element = (FrameworkElement)d;
            DockingManager owner = DockingManager.ResolveManager(element as UIElement);
            if (owner != null)
            {
                IsSelectedChangedEventArgs e = new IsSelectedChangedEventArgs();
                if ((element as TabItemExt) != null)
                {
                    ContentPresenter presenter = (element as TabItemExt).Content as ContentPresenter;
                    if (presenter != null && presenter.Content != null)
                    {
                        e.TargetElement = presenter.Content as FrameworkElement;
                    }
                }
                else
                {
                    e.TargetElement = element;
                }
                e.NewValue = (bool)args.NewValue;
                e.OldValue = (bool)args.OldValue;
                owner.FireIsSelectedDocumentChanged(e);
            }
        }

        /// <summary>
        /// Calls OnContentChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TDILayoutPanel instance = (TDILayoutPanel)d;
            instance.OnContentChanged(e);
        }

        /// <summary>
        /// Called when [container changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnContainerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TDILayoutPanel instance = (TDILayoutPanel)d;
            instance.OnContainerChanged(e);
        }

        /// <summary>
        /// Determines whether the specified way contains way.
        /// </summary>
        /// <param name="baseWay">The base way baseWay.</param>
        /// <param name="way">The way baseWay.</param>
        /// <param name="count">The count baseWay.</param>
        /// <returns>
        /// <c>true</c> if the specified way contains way; otherwise, <c>false</c>.
        /// </returns>
        private static bool ContainsWays(string baseWay, string way, int count)
        {
            bool result = true;

            for (int i = 0; i < count; ++i)
            {
                if (baseWay[i] != way[i])
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Clears the tab controls.
        /// </summary>
        /// <param name="tabList">The tab list.</param>
        private static void ClearTabControls(IList<DocumentTabControl> tabList)
        {
            int count = tabList.Count;

            while (0 < count)
            {
                DocumentTabControl tabControl = tabList[tabList.Count - 1];

                while (tabControl != null && 0 < tabControl.Items.Count)
                {
                    ContentControl item = (ContentControl)tabControl.Items[0];
                    UIElement element = DocumentTabControl.GetContent(item);
                    tabControl.RemoveItem(element);
                }

                --count;
            }

            tabList.Clear();
        }

        /// <summary>
        /// Needs the concat in tab group.
        /// </summary>
        /// <param name="uiList">The UI elements list.</param>
        /// <returns>bool element value</returns>
        private static bool NeedConcatInTabGroup(IList<UIElement> uiList)
        {
            if (1 < uiList.Count)
            {
                return true;
            }

            FrameworkElement element = (FrameworkElement)uiList[0];
            return !element.Name.StartsWith(NAME_PREFIX);
        }

        /// <summary>
        /// Items the update.
        /// </summary>
        /// <param name="tabControl">The tab control.</param>
        /// <remarks>It is workaround. This should be removed after fix of MS</remarks>
        private static void ItemsUpdate(ItemsControl tabControl)
        {
            Orientation orientation = (Orientation)tabControl.GetValue(TDIGroupOrientationProperty);
            string way = tabControl.GetValue(WayOfTDIGroupProperty).ToString();

            foreach (ContentControl item in tabControl.Items)
            {
                DependencyObject element = DocumentTabControl.GetContent(item);
                element.SetValue(TDILayoutPanel.WayOfTDIGroupProperty, way);
                element.SetValue(TDILayoutPanel.TDIGroupOrientationProperty, orientation);
            }
        }

        /// <summary>
        /// Called when [tab control take drag item event].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="arg">The <see cref="Syncfusion.Windows.Tools.Controls.TakeDragItemEventArgs"/> instance containing the event data.</param>
        private static void OnTabControlTakeDragItemEvent(object sender, TakeDragItemEventArgs arg)
        {
            DocumentTabControl tabControl = (DocumentTabControl)sender;
            tabControl.UpdateMenuItems((TabItemExt)arg.DragedItem);
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Presents content of panel.
        /// </summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(UIElement), typeof(TDILayoutPanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContentChanged)));

        /// <summary>
        /// Presents counter for tab group.
        /// </summary>
        internal static readonly DependencyProperty WayOfTDIGroupProperty = DependencyProperty.RegisterAttached("WayOfTDIGroup", typeof(string), typeof(TDILayoutPanel), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Presents TDIGroupOrientation for tab group. 
        /// </summary>
        internal static readonly DependencyProperty TDIGroupOrientationProperty = DependencyProperty.RegisterAttached("TDIGroupOrientation", typeof(Orientation), typeof(TDILayoutPanel), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Presents index in internal TabCon.
        /// </summary>
        internal static readonly DependencyProperty TDIIndexProperty = DependencyProperty.RegisterAttached("TDIIndex", typeof(int), typeof(TDILayoutPanel), new UIPropertyMetadata(-1, new PropertyChangedCallback(OnTDIIndexChanged)));

        /// <summary>
        /// Presents selected tab control item.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.RegisterAttached("IsSelected", typeof(bool), typeof(TDILayoutPanel), new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged)));

        /// <summary>
        /// Represents the key for Container Property
        /// </summary>
        protected static readonly DependencyPropertyKey ContainerPropertyKey = DependencyProperty.RegisterReadOnly("Container", typeof(DocumentContainer), typeof(TDILayoutPanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange, new PropertyChangedCallback(OnContainerChanged)));

        /// <summary>
        /// Presents Container property.
        /// </summary>
        protected static readonly DependencyProperty ContainerProperty = ContainerPropertyKey.DependencyProperty;

        // Using a DependencyProperty as the backing store for SplitPanelOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SplitPanelOffsetProperty =
            DependencyProperty.RegisterAttached("SplitPanelOffset", typeof(double), typeof(TDILayoutPanel), new UIPropertyMetadata(0.0));

        #endregion

        /// <summary>
        /// Represents the pop up in the Tab layout panel
        /// </summary>
        internal Popup m_cm;

        /// <summary>
        /// Presents stack panel
        /// </summary>
        private StackPanel sp;

        /// <summary>
        /// Presents boo value
        /// </summary>
        private bool bm_cm;

        /// <summary>
        /// Presents Drag data
        /// </summary>
        private DragData m_dragData = DragData.Empty;

        /// <summary>
        /// Updates the tab controls.
        /// </summary>
        /// <param name="draggedItem">The dragged item.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        internal void UpdateTabControls(TabItemExt draggedItem, DocumentTabControl source, DocumentTabControl target)
        {
            m_dragData = new DragData(draggedItem, source, target);
            PanelBase parent = (PanelBase)source.Parent;
            if (HasNextElement(source, parent, true) || HasNextElement(source, parent, false))
            {
                bm_cm = false;
            }
            else
            {
                bm_cm = true;
            }

            m_cm = new Popup
            {
                Placement = PlacementMode.Mouse,
                PlacementTarget = this,
            };

            sp = new StackPanel()
            {
                Background = Brushes.LightGray,
            };
            if (bm_cm == true)
            {
                if (source != null)
                {
                    TDISplitPanel panel = parent as TDISplitPanel;
                    if (source.Items.Count > 1 || panel != null)
                    {
                        sp.Children.Add(CreateMenuItem(wrapper.NewTabgroup, C_CREATE_NEWGROUP, draggedItem));
                    }

                }
            }

                if (parent != null)
                {
                    TDISplitPanel panel = parent as TDISplitPanel;
                    if (panel != null)
                    {
                        if (source.Items.Count > 1)
                        {
                            sp.Children.Add(CreateMenuItem(wrapper.NewTabgroup, C_CREATE_NEWGROUP, draggedItem));
                        }
                        if (HasNextElement(source, parent, true) && HasNextElement(source, parent, false))
                        {
                            sp.Children.Add(CreateMenuItem(wrapper.MoveToNextTabGroup, C_MOVE_TO_NEXT_GROUP, draggedItem));
                            sp.Children.Add(CreateMenuItem(wrapper.MoveToPreviousTabGroup, C_MOVE_TO_PREVIOUS_GROUP, draggedItem));
                        }
                        else if (HasNextElement(source, parent, false))
                        {
                            sp.Children.Add(CreateMenuItem(wrapper.MoveToPreviousTabGroup, C_MOVE_TO_PREVIOUS_GROUP, draggedItem));
                        }
                        else
                        {
                            sp.Children.Add(CreateMenuItem(wrapper.MoveToNextTabGroup, C_MOVE_TO_NEXT_GROUP, draggedItem));
                        }
                    }
                }

            if (source != null)
            {
                TDISplitPanel panel = parent as TDISplitPanel;
                if (source.Items.Count > 1 || panel != null)
                {
                    sp.Children.Add(CreateMenuItem(wrapper.MenuItemCancel, C_CANCEL,draggedItem));
                }
            }
            m_cm.Child = sp;
            m_cm.Closed += new EventHandler(OnContextMenuClosed);
            m_cm.IsOpen = true;
        }

        /// <summary>
        /// Represents DragData
        /// </summary>
        private struct DragData
        {
            /// <summary>
            /// Represents DraggedItem
            /// </summary>
            public TabItemExt DraggedItem;

            /// <summary>
            /// Represents Source
            /// </summary>
            public DocumentTabControl Source;

            /// <summary>
            /// Represents Target
            /// </summary>
            public DocumentTabControl Target;

            /// <summary>
            /// Represents m_Empty
            /// </summary>
            private static DragData m_empty;

            /// <summary>
            /// Gets the empty.
            /// </summary>
            /// <value>The empty.</value>
            public static DragData Empty
            {
                get
                {
                    return m_empty;
                }
            }

            /// <summary>
            /// Initializes static members of the <see cref="DragData"/> struct.
            /// </summary>
            static DragData()
            {
                m_empty = new DragData(null, null, null);
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="DragData"/> struct.
            /// </summary>
            /// <param name="item">The item DraggedItem.</param>
            /// <param name="source">The source Source.</param>
            /// <param name="target">The target Target.</param>
            public DragData(TabItemExt item, DocumentTabControl source, DocumentTabControl target)
            {
                DraggedItem = item;
                Source = source;
                Target = target;
            }

            /// <summary>
            /// Updates the items.
            /// </summary>
            /// <param name="element">The element.</param>
            /// <param name="source">The source.</param>
            internal void UpdateItems(UIElement element, DocumentTabControl source,bool nexttabgroup)
            {
                PanelBase parent = (PanelBase)source.Parent;
                bool trgt = false;

                if (HasNextElement(source, parent, true))
                {
                    trgt = true;
                }
                else if (HasNextElement(source, parent, false))
                {
                    trgt = true;
                }
                else
                {
                    trgt = false;
                }

                if (trgt)
                {
                    if (HasNextElement(source, parent, nexttabgroup))
                    {
                        Target = GetNextElement(source, parent, nexttabgroup);
                    }
                }
                source.RemoveItem(element);
                Target.AddItemContent(element);
                Target.SelectedItem = DraggedItem;
                Target.UpdateMenuItems(DraggedItem);
            }
        }

        /// <summary>
        /// Called when [context menu closed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnContextMenuClosed(object sender, EventArgs e)
        {
            m_dragData = DragData.Empty;
        }

        /// <summary>
        /// Creates the menu item.
        /// </summary>
        /// <param name="header">The header result.</param>
        /// <param name="tag">The tag result.</param>
        /// <returns>MenuItem result</returns>
        private MenuItem CreateMenuItem(string header, string tag,TabItemExt draggedItem)
        {
            MenuItem result = new MenuItem
            {
                Header = header,
                Tag = tag,
            };
            if (draggedItem != null)
            {
                if (draggedItem.TabItemContextMenuItemTemplate != null)
                {
                    result.HeaderTemplate = draggedItem.TabItemContextMenuItemTemplate;
                }

                if (draggedItem.TabItemContextMenuItemStyle != null)
                {
                    result.Style = draggedItem.TabItemContextMenuItemStyle;
                }
            }
            result.Click += new RoutedEventHandler(OnMenuItemClick);

            return result;
        }

        /// <summary>
        /// Called when [menu item click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement menuItem = (FrameworkElement)sender;
            string tag = menuItem.Tag.ToString();
            UIElement element = DocumentTabControl.GetContent(m_dragData.DraggedItem);

            switch (tag)
            {
                case C_CREATE_NEWGROUP:
                    Orientation orientation = (Orientation)m_dragData.Source.GetValue(TDIGroupOrientationProperty);
                    CreateNewTab(orientation, element, m_dragData.Source);
                    break;
                case C_MOVE_TO_NEXT_GROUP:
                    m_dragData.UpdateItems(element, m_dragData.Source,true);
                    break;
                case C_MOVE_TO_PREVIOUS_GROUP:
                    m_dragData.UpdateItems(element, m_dragData.Source,false);
                    break;
                case C_CANCEL:
                    break;
                default:
                    throw new NotImplementedException(string.Format("This {0} behavior isn't implemented!", tag));
            }

            m_cm.IsOpen = false;
        }


        private bool m_WayOfTDIFlag = false;
        /// <summary>
        /// Creates the new tab.
        /// </summary>
        /// <param name="orientation">The orientation.</param>
        /// <param name="element">The element.</param>
        /// <param name="tabControl">The tab control.</param>

        private bool CheckPropertyList(PropertyInfo property)
        {
            List<string> PropertyList = new List<string>();
            PropertyList.Add("ActivatedItem");
            PropertyList.Add("SelectedContent");
            PropertyList.Add("SelectedIndex");
            PropertyList.Add("SelectedItem");
            PropertyList.Add("SelectedValue");
            PropertyList.Add("Name");
            PropertyList.Add("Uid");
            PropertyList.Add("ItemsSource");
           return PropertyList.Contains(property.Name);
           
        }

        private void CreateNewTab(Orientation orientation, UIElement element, DocumentTabControl tabControl)
        {

            m_changingGroup = true;
            PanelBase parent = (PanelBase)tabControl.Parent;
            parent.RemoveElement(tabControl);
            tabControl.RemoveItem(element);
            if (tabControl.SelectionStack.Count > 0 && tabControl.Items.Count != 0)
            {
                tabControl.SelectedItem = tabControl.Items[0];
                tabControl.IsTabGroupFocus = false;
                if ((tabControl.SelectedItem as TabItemExt) != null)
                {
                    ContentPresenter headpresenter = (tabControl.SelectedItem as TabItemExt).Template.FindName("Content", (tabControl.SelectedItem as TabItemExt)) as ContentPresenter;
                    if (headpresenter != null )
                    {
                        TextElement.SetFontWeight(headpresenter, FontWeights.Normal);
                    }
                    (tabControl.SelectedItem as TabItemExt).IsTabGroupFocus = false;
                }
            }
            DockingManager manager = VisualUtils.FindAncestor(this, typeof(DockingManager)) as DockingManager;
            bool isedit = manager != null ? manager.EnableDocumentTabHeaderEdit : false;
            DocumentTabControl newTabControl = new DocumentTabControl
            {
                Background = Brushes.Transparent,
                EnableLabelEdit = isedit,
                Style = tabControl.Style,
                Container = tabControl.Container,
                Name = string.Concat(NAME_PREFIX, ++m_nameSufix),
            };
            foreach (PropertyInfo property in tabControl.GetType().GetProperties())
            {
                foreach (PropertyInfo newTabProperty in newTabControl.GetType().GetProperties())
                {
                    if (property.Name == newTabProperty.Name && !CheckPropertyList(property) && newTabProperty.CanWrite && (property.DeclaringType.BaseType.Name == "TabControlExt"
                        || property.DeclaringType.BaseType.Name == "TabItemExt" || property.DeclaringType.BaseType.Name == "TabControl" || property.DeclaringType.BaseType.Name == "TabItem"))
                    {
                         newTabProperty.SetValue(newTabControl, property.GetValue(tabControl, null), null);
                    }
                }
            }
            newTabControl.SetValue(TDILayoutPanel.TDIGroupOrientationProperty, orientation);

            string way = (string)tabControl.GetValue(TDILayoutPanel.WayOfTDIGroupProperty);
            int lenght = way.Length;
            string newWay = way.Insert(lenght, FALSE_BIT_EX);
            newTabControl.SetValue(TDILayoutPanel.WayOfTDIGroupProperty, newWay);
            way = way.Insert(lenght, TRUE_BIT_EX);
            tabControl.SetValue(TDILayoutPanel.WayOfTDIGroupProperty, way);
            newTabControl.AddItemContent(element);
            TDILayoutPanel.SetIsSelected(element, true);
            m_changingGroup = false;

            ItemsUpdate(tabControl);
            ItemsUpdate(newTabControl);

            m_TabList.Add(newTabControl);
            InitializeTabControl(newTabControl);

            TDISplitPanel panel = new TDISplitPanel(tabControl, newTabControl, orientation)
            {
                Name = string.Concat(NAME_PREFIX, ++m_nameSufix)
            };
            parent.AddElement(panel);
            if (DocumentContainer.GetDocumentTabControlStyle(Container) != null)
            {
                newTabControl.Style = DocumentContainer.GetDocumentTabControlStyle(Container) as Style;
                tabControl.Style = DocumentContainer.GetDocumentTabControlStyle(Container) as Style;
            }
            else if (Container.IsInDockingManager
                && DockingManager.GetDocumentTabControlStyle(Container.FlipParent as DockingManager)!=null)
            {
                newTabControl.Style = (Style)DockingManager.GetDocumentTabControlStyle(Container.FlipParent as DockingManager);
                tabControl.Style = (Style)DockingManager.GetDocumentTabControlStyle(Container.FlipParent as DockingManager);
            }
            ActiveTabControl = newTabControl;
            m_WayOfTDIFlag = true;

            TabGroupEventArgs args = new TabGroupEventArgs() 
            { 
                Orientation = orientation, 
                PreviousTabGroup = tabControl, 
                CurrentTabGroup = newTabControl, 
                TargetItem = element 
            };
            Container.FireTabGroupCreated(args);
        }

        /// <summary>
        /// Sets the properties binding.
        /// </summary>
        /// <param name="sourceElement">The source element binding.</param>
        /// <param name="destElement">The dest element binding.</param>
        /// <param name="source">The source binding.</param>
        /// <param name="dest">The dest binding.</param>
        private void SetPropertiesBinding(UIElement sourceElement, FrameworkElement destElement, DependencyProperty source, DependencyProperty dest)
        {
            Binding binding = new Binding();
            binding.Source = sourceElement;
            binding.Mode = BindingMode.OneWay;
            binding.Path = new PropertyPath(source);
            destElement.SetBinding(dest, binding);
        }

        #region ILayoutPanel Members


        /// <summary>
        /// Resets the visible list.
        /// </summary>
        public void ResetVisibleList()
        {

        }

        /// <summary>
        /// Sets the active document.
        /// </summary>
        /// <param name="element">The element.</param>
        public void SetActiveDocument(UIElement element)
        {

        }

        #endregion
    }
}