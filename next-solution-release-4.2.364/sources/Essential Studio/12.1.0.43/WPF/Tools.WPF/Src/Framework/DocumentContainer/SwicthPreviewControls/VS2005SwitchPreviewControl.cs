// <copyright file="VS2005SwitchPreviewControl.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This control presents a like vista flip action.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class VS2005SwitchPreviewControl : SwicthPreviewControlBase
    {
        #region Constants
        /// <summary>
        /// This member presents a element's name from Template of control.
        /// </summary>
        private const string LIST_BOX_NAME = "PART_ListBox";
        
        /// <summary>
        /// This member presents a element's name from Template of control.
        /// </summary>
        private const string TOOLWINDOWS_LIST_NAME = "PART_ToolWindows";
        
        /// <summary>
        /// This member presents a element's name from Template of control.
        /// </summary>
        private const string PREVIEWBORDER_NAME = "PART_PreviewBorder";
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when ParentContainer property is changed.
        /// </summary>
        public event PropertyChangedCallback ParentContainerChanged;
        #endregion

        #region Private members
        /// <summary>
        /// This is documents collection of elements in current control.
        /// </summary>
        private readonly ObservableFrameworkElements m_DocumentsList = new ObservableFrameworkElements();
        
        /// <summary>
        /// This is collection of elements in current control.
        /// </summary>
        private readonly ObservableFrameworkElements m_Collections = new ObservableFrameworkElements();

        /// <summary>
        /// This is active collection (m_DocumentsList or m_Collections) of elements in current control.
        /// </summary>
        private ObservableFrameworkElements m_activeColection = null;
        
        /// <summary>
        /// This is index of selected item.
        /// </summary>
        private int m_index = 0;
        
        /// <summary>
        /// This member presents a element's name from Template of control.
        /// </summary>
        private ListBox m_listBox = null;
        
        /// <summary>
        /// This member presents a element's name from Template of control.
        /// </summary>
        private ListBox m_toolWindows = null;
        
        /// <summary>
        /// This member presents a element's name from Template of control.
        /// </summary>
        private PreviewBorder m_previewBorder = null;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="VS2005SwitchPreviewControl"/> class.
        /// </summary>
        static VS2005SwitchPreviewControl()
        {
            EnvironmentTest.ValidateLicense(typeof(VS2005SwitchPreviewControl));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VS2005SwitchPreviewControl), new FrameworkPropertyMetadata(typeof(VS2005SwitchPreviewControl)));
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public override object SelectedItem
        {
            get
            {
                if (null != m_activeColection && 0 < m_activeColection.Count)
                {
                    int index = m_index >= m_activeColection.Count ? m_activeColection.Count - 1 : m_index;

                    if (index < 0)
                    {
                        return null;
                    }
                    else
                    {
                        return m_activeColection[index].DataContext;
                    }
                }
                else
                {
                    return null;
                }
            }
        }
        
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public override IEnumerable Items
        {
            get
            {
                return m_Collections;
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether [used main collection].
        /// </summary>
        /// <value><c>true</c> if [used main collection]; otherwise, <c>false</c>.</value>
        public override bool UsedMainCollection
        {
            get
            {
                return m_activeColection == m_Collections;
            }
        }

        /// <summary>
        /// Gets or sets the value of the ParentContainer dependency property.
        /// </summary>
        public IVS2005FlipOwner ParentContainer
        {
            get
            {
                return (IVS2005FlipOwner)GetValue(ParentContainerProperty);
            }

            set
            {
                SetValue(ParentContainerProperty, value);
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// When overridden in a derived class, is invoked whenever 
        /// application code or internal processes call 
        /// <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            m_listBox = GetTemplateChild(LIST_BOX_NAME) as ListBox;
            m_toolWindows = GetTemplateChild(TOOLWINDOWS_LIST_NAME) as ListBox;
            m_previewBorder = GetTemplateChild(PREVIEWBORDER_NAME) as PreviewBorder;

            if (null == m_listBox || null == m_toolWindows || null == m_previewBorder)
            {
                throw new NotImplementedException("Incorrect template");
            }

            m_listBox.ItemsSource = m_Collections;
            SetIndex(m_index);

            m_toolWindows.ItemsSource = m_DocumentsList;
            m_activeColection = m_Collections;
        }
        
        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="item">The item Collections.</param>
        public override void AddItem(ContentControl item)
        {
            m_Collections.Add(item);
        }

        /// <summary>
        /// Clears the items.
        /// </summary>
        public override void ClearItems()
        {
            m_Collections.Clear();
            m_index = 0;
            m_activeColection = null;
            ClearDocumentsList();
            Visibility = Visibility.Collapsed;
        }
        
        /// <summary>
        /// Moves the item.
        /// </summary>
        /// <param name="isForward">if set to <c>true</c> [is forward].</param>
        /// <param name="switchDirection">The switch direction.</param>
        public override void MoveItem(bool isForward, SwitchDirection switchDirection)
        {
            if (SwitchDirection.Horizontal != switchDirection)
            {
                m_activeColection = m_activeColection ?? m_Collections;

                if (isForward)
                {
                    ChangeCollectionToMoveForward(switchDirection);
                    ForwardSwitchVisual();
                }
                else
                {
                    ChangeCollectionToMoveBackward(switchDirection);
                    BackforwardSwitchVisual();
                }
            }
            else if (0 < m_DocumentsList.Count)
            {
                m_activeColection = m_Collections == m_activeColection ? m_DocumentsList : m_Collections;
                int count = m_activeColection.Count - 1;
                int index = m_index;

                if (count < index)
                {
                    index = count;
                }

                SetIndex(index);
            }
        }
        
        /// <summary>
        /// Changes active collection when down key is pressed.
        /// </summary>
        /// <param name="switchDirection">The switch direction.</param>
        private void ChangeCollectionToMoveForward(SwitchDirection switchDirection)
        {
            if (switchDirection == SwitchDirection.Vertical && m_activeColection.Count - 1 <= m_index)
            {
                if (m_activeColection == m_Collections && m_DocumentsList.Count > 0)
                {
                    m_activeColection = m_DocumentsList;
                    m_index = m_DocumentsList.Count - 1;
                }
                else if (m_activeColection == m_DocumentsList && m_Collections.Count > 0)
                {
                    m_activeColection = m_Collections;
                    m_index = m_Collections.Count - 1;
                }
            }
        }
        
        /// <summary>
        /// Changes active collection when up key is pressed.
        /// </summary>
        /// <param name="switchDirection">The switch direction.</param>
        private void ChangeCollectionToMoveBackward(SwitchDirection switchDirection)
        {
            if (switchDirection == SwitchDirection.Vertical && m_index == 0)
            {
                if (m_activeColection == m_Collections && m_DocumentsList.Count > 0)
                {
                    m_activeColection = m_DocumentsList;
                    m_index = 0;
                }
                else if (m_activeColection == m_DocumentsList && m_Collections.Count > 0)
                {
                    m_activeColection = m_Collections;
                    m_index = 0;
                }
            }
        }
        
        /// <summary>
        /// Shows the selected item.
        /// </summary>
        public override void ShowSelectedItem()
        {
            ObservableFrameworkElements toolWindowsList = ParentContainer.ToolWindowsList;
            AddItems(toolWindowsList);
            m_activeColection = m_Collections;

            if (m_activeColection == null || m_activeColection.Count == 0)
            {
                m_activeColection = m_DocumentsList;
            }

            int index = (m_activeColection.Count == 1) ? 0 : 1;
            SetIndex(index);
        }
        #endregion

        #region Implemenation
        /// <summary>
        /// Gets the parent container.
        /// </summary>
        /// <param name="parentPopup">The parent popup.</param>
        /// <returns>IVS2005 FlipOwner</returns>
        protected virtual IVS2005FlipOwner GetParentContainer(Popup parentPopup)
        {
            IVS2005FlipOwner parentContainer = parentPopup.PlacementTarget as IVS2005FlipOwner
                ?? ((DockingManager)parentPopup.PlacementTarget).DocContainer as IVS2005FlipOwner;

            if (null == parentContainer)
            {
                //throw new NotImplementedException("ParentContainer is implemented incorrect.");
            }

            return parentContainer;
        }
        
        /// <summary>
        /// Raises ParentContainerChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnParentContainerChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != ParentContainerChanged)
            {
                ParentContainerChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. 
        /// This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            Popup parentPopup = (Popup)Parent;
            ParentContainer = GetParentContainer(parentPopup);
            base.OnInitialized(e);
        }

        /// <summary>
        /// Adds the items.
        /// </summary>
        /// <param name="items">The items.</param>
        private void AddItems(IList items)
        {
            if (items != null && items.Count > 0)
            {
                foreach (FrameworkElement element in items)
                {
                    ListBoxItem item = new ListBoxItem
                                        {
                                            DataContext = element
                                        };
                    item.AddHandler(Control.MouseUpEvent, new RoutedEventHandler(OnItemMouseUp), true);
                    m_DocumentsList.Add(item);
                }
            }
        }
        
        /// <summary>
        /// Clears the documents list.
        /// </summary>
        private void ClearDocumentsList()
        {
            while (0 < m_DocumentsList.Count)
            {
                FrameworkElement item = m_DocumentsList[0];
                item.RemoveHandler(Control.MouseUpEvent, new RoutedEventHandler(OnItemMouseUp));
                m_DocumentsList.Remove(item);
            }
        }

        /// <summary>
        /// Sets the index of collections.
        /// </summary>
        /// <param name="value">The value.</param>
        private void SetIndex(int value)
        {
            m_index = value;

            if (null != m_listBox)
            {
                ListBox list;

                if (m_Collections == m_activeColection)
                {
                    list = m_listBox;
                    m_toolWindows.SelectedItem = null;
                }
                else
                {
                    list = m_toolWindows;
                    m_listBox.SelectedItem = null;
                }

                int cnt = list.Items.Count;

                if (0 < cnt)
                {
#if DevCode
					if ( cnt < value )
					{
						Debugger.Break();
					}
#endif
                    list.SelectedIndex = value;
                    FrameworkElement item = (FrameworkElement)list.SelectedItem;
                    PrepareVisual(item, list);
                }
            }
        }
        
        /// <summary>
        /// Forwards the switch visual element.
        /// </summary>
        private void ForwardSwitchVisual()
        {
            int count = m_activeColection.Count - 1;
            SwitchQuickVisual(count, 0, 1);
        }
        
        /// <summary>
        /// Back forward the switch visual element.
        /// </summary>
        private void BackforwardSwitchVisual()
        {
            int count = m_activeColection.Count - 1;
            SwitchQuickVisual(0, count, -1);
        }
        
        /// <summary>
        /// Switches the visual element.
        /// </summary>
        /// <param name="bound">The bound SwitchQuickVisual.</param>
        /// <param name="newBound">The new bound.</param>
        /// <param name="inc">The inc SwitchQuickVisual.</param>
        private void SwitchQuickVisual(int bound, int newBound, int inc)
        {
            if (bound == m_index)
            {
                SetIndex(newBound);
            }
            else
            {
                SetIndex(m_index + inc);
            }
        }
        
        /// <summary>
        /// Prepares the visual element.
        /// </summary>
        /// <param name="item">The item dContext.</param>
        /// <param name="list">The list dContext.</param>
        private void PrepareVisual(FrameworkElement item, ListBox list)
        {
            try
            {
                if (item != null)
                {
                    list.ScrollIntoView(item);
                    FrameworkElement dContext = (FrameworkElement)item.DataContext;
                    DataContext = dContext;
                    SwicthPreviewControlBase.SetCustomBrush(m_previewBorder, dContext);
                }
            }
            catch
            {

            }
        }
        
        /// <summary>
        /// Called when [item mouse up].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnItemMouseUp(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)sender;
            ParentContainer.ClosePreview();
            ParentContainer.OnToolWindowsItemSelected(item.DataContext);
        }

        /// <summary>
        /// Calls OnParentContainerChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnParentContainerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VS2005SwitchPreviewControl instance = (VS2005SwitchPreviewControl)d;
            instance.OnParentContainerChanged(e);
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Presents TemplateParent that is DocContainer in our case.
        /// </summary>
        public static readonly DependencyProperty ParentContainerProperty = DependencyProperty.Register("ParentContainer", typeof(IVS2005FlipOwner), typeof(VS2005SwitchPreviewControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnParentContainerChanged)));
        #endregion
    }
}