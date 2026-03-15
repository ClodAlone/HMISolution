// <copyright file="TreeViewColumnHeader.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

#region file using

using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Syncfusion.Licensing;

#endregion file using

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the class for TreeView Column header
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TreeViewColumnHeader : ButtonBase
    {
        #region Constants

        /// <summary>
        /// Presents template name
        /// </summary>
        private const string C_headerGripperTemplateName = "PART_HeaderGripper";

        #endregion Constants

        #region Members

        /// <summary>
        /// Presents headerGripper
        /// </summary>
        private Thumb m_headerGripper;

        /// <summary>
        /// Presents originalWidth
        /// </summary>
        private double m_originalWidth;

        /// <summary>
        /// Presents previousHeader
        /// </summary>
        private TreeViewColumnHeader m_previousHeader;

        /// <summary>
        /// Presents splitCursorCach
        /// </summary>
        private static Cursor m_splitCursorCache = null;

        /// <summary>
        /// Presents plitOpenCursorCach
        /// </summary>
        private static Cursor m_splitOpenCursorCache = null;

        internal ContentPresenter headerContent;

        internal FrameworkElement Arrowpath;
       
        #endregion Members

        #region Dependency property

        /// <summary>
        /// Represents the IsPressedHeader Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsPressedHeaderProperty = DependencyProperty.Register("IsPressedHeader", typeof(bool), typeof(TreeViewColumnHeader), new UIPropertyMetadata(false));

        /// <summary>
        ///  Represents the ColumnPropertyKey Dependency Property
        /// </summary>
        internal static readonly DependencyPropertyKey ColumnPropertyKey = DependencyProperty.RegisterReadOnly("Column", typeof(TreeViewColumn), typeof(TreeViewColumnHeader), null);

        /// <summary>
        /// Represents the Column Dependency Property
        /// </summary>
        public static readonly DependencyProperty ColumnProperty = ColumnPropertyKey.DependencyProperty;

        /// <summary>
        /// Represents the RolePropertyKey Dependency Property
        /// </summary>
        internal static readonly DependencyPropertyKey RolePropertyKey = DependencyProperty.RegisterReadOnly("Role", typeof(TreeViewColumnHeaderRole), typeof(TreeViewColumnHeader), new FrameworkPropertyMetadata(TreeViewColumnHeaderRole.Normal));

        /// <summary>
        /// Represents the Role Dependency Property
        /// </summary>
        public static readonly DependencyProperty RoleProperty = RolePropertyKey.DependencyProperty;

        /// <summary>
        /// Represents sort direction
        /// </summary>
        internal static readonly DependencyProperty SortDirectionProperty = DependencyProperty.Register("SortDirection", typeof(SortDirection), typeof(TreeViewColumnHeader), new FrameworkPropertyMetadata(SortDirection.None));

        #endregion Dependency property

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating sort direction of the corresponding column.
        /// </summary>
        /// <value>
        /// Specifies the direction of a sort operation. The default value is None.
        /// </value>
        public SortDirection SortDirection
        {
            get
            {
                return (SortDirection)GetValue(SortDirectionProperty);
            }
            set
            {
                SetValue(SortDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is pressed header.
        /// </summary>
        /// <value>
        /// true if this instance is pressed header; otherwise, false.
        /// </value>
        public bool IsPressedHeader
        {
            get
            {
                return (bool)GetValue(IsPressedHeaderProperty);
            }

            set
            {
                SetValue(IsPressedHeaderProperty, value);
            }
        }

        /// <summary>
        /// Gets the column.
        /// </summary>
        /// <value>The column.</value>
        public TreeViewColumn Column
        {
            get
            {
                return (TreeViewColumn)base.GetValue(ColumnProperty);
            }
        }

        /// <summary>
        /// Gets the role.
        /// </summary>
        /// <value>The role of header.</value>
        public TreeViewColumnHeaderRole Role
        {
            get
            {
                return (TreeViewColumnHeaderRole)base.GetValue(RoleProperty);
            }
        }

        /// <summary>
        /// Gets or sets the previous visual header.
        /// </summary>
        /// <value>The previous visual header.</value>
        internal TreeViewColumnHeader PreviousVisualHeader
        {
            get
            {
                return m_previousHeader;
            }

            set
            {
                m_previousHeader = value;
            }
        }

        /// <summary>
        /// Gets the actual width of the column.
        /// </summary>
        /// <value>The actual width of the column.</value>
        private double ColumnActualWidth
        {
            get
            {
                if (Column == null)
                {
                    return ActualWidth;
                }

                return Column.ActualWidth;
            }
        }

        /// <summary>
        /// Gets the split cursor.
        /// </summary>
        /// <value>The split cursor.</value>
        private Cursor SplitCursor
        {
            get
            {
                if (m_splitCursorCache == null)
                {
                    m_splitCursorCache = GetCursor(false);
                }

                return m_splitCursorCache;
            }
        }

        /// <summary>
        /// Gets the split open cursor.
        /// </summary>
        /// <value>The split open cursor.</value>
        private Cursor SplitOpenCursor
        {
            get
            {
                if (m_splitOpenCursorCache == null)
                {
                    m_splitOpenCursorCache = GetCursor(true);
                }

                return m_splitOpenCursorCache;
            }
        }

        /// <summary>
        /// Gets the logical parent.
        /// </summary>
        /// <value>The logical parent.</value>
        private TreeViewHeaderRowPresenter LogicalParent
        {
            get
            {
                return Parent as TreeViewHeaderRowPresenter;
            }
        }

        #endregion Properties

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="TreeViewColumnHeader"/> class.
        /// </summary>
        static TreeViewColumnHeader()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeViewColumnHeader), new FrameworkPropertyMetadata(typeof(TreeViewColumnHeader)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewColumnHeader"/> class.
        /// </summary>
        public TreeViewColumnHeader()
        {
           
        }

        #endregion Initialization

        #region Implementation

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            switch (Role)
            {
                case TreeViewColumnHeaderRole.Normal:
                    {
                        HookupGripperEvents();
                        break;
                    }
            }
            this.MouseLeftButtonDown += new MouseButtonEventHandler(TreeViewColumnHeader_MouseLeftButtonDown);
            headerContent = this.GetTemplateChild("HeaderContent") as ContentPresenter;
            Arrowpath = this.GetTemplateChild("arrow") as FrameworkElement;
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the TreeViewColumnHeader control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void TreeViewColumnHeader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// Gets the parent tree view.
        /// </summary>
        /// <value>The parent tree view.</value>
        internal TreeViewAdv ParentTreeView
        {
            get
            {
                return TreeViewAdv.GetTreeViewFromChildren(this);
            }
        }

        

        /// <summary>
        /// Called when an element loses keyboard focus.
        /// </summary>
        /// <param name="e">The event data for the <see cref="E:System.Windows.IInputElement.LostKeyboardFocus"/> event.</param>
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);

            if ((base.ClickMode == ClickMode.Hover) && base.IsMouseCaptured)
            {
                base.ReleaseMouseCapture();
            }
        }

        /// <summary>
        /// Provides class handling for the <see cref="P:System.Windows.Controls.Primitives.ButtonBase.ClickMode"/> routed event that occurs when the mouse enters this control.
        /// </summary>
        /// <param name="e">The event data for the <see cref="E:System.Windows.Input.Mouse.MouseEnter"/> event.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (HandleIsMouseOverChanged())
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Provides class handling for the <see cref="E:System.Windows.UIElement.MouseLeave"/> routed event that occurs when the mouse leaves an element.
        /// </summary>
        /// <param name="e">The event data for the <see cref="E:System.Windows.Input.Mouse.MouseLeave"/> event.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (HandleIsMouseOverChanged())
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Provides class handling for the <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> routed event that occurs when the left mouse button is pressed while the mouse pointer is over this control.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            e.Handled = false;

            if ((base.ClickMode == ClickMode.Hover) && (e.ButtonState == MouseButtonState.Pressed))
            {
                base.CaptureMouse();
            }
        }

        /// <summary>
        /// Provides class handling for the <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/> routed event that occurs when the left mouse button is released while the mouse pointer is over this control.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            e.Handled = false;

            if ((base.ClickMode == ClickMode.Hover) && base.IsMouseCaptured)
            {
                base.ReleaseMouseCapture();
            }

            IsPressedHeader = false;
        }

        /// <summary>
        /// Provides class handling for the <see cref="E:System.Windows.UIElement.MouseMove"/> routed event that occurs when the mouse pointer moves while over this element.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (((base.ClickMode != ClickMode.Hover) && base.IsMouseCaptured) && (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed))
            {
                IsPressedHeader = true;
            }

            e.Handled = false;
        }

        /// <summary>
        /// Called when the rendered size of a control changes.
        /// </summary>
        /// <param name="sizeInfo">Specifies the size changes.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            CheckWidthForPreviousHeaderGripper();
        }

        /// <summary>
        /// Checks the width for previous header gripper.
        /// </summary>
        internal void CheckWidthForPreviousHeaderGripper()
        {
            bool hide = false;

            if (m_headerGripper != null)
            {
                hide = DoubleUtil.LessThan(base.ActualWidth, m_headerGripper.Width);
            }

            if (m_previousHeader != null)
            {
                m_previousHeader.HideGripperRightHalf(hide);
            }

            UpdateGripperCursor();
        }

        /// <summary>
        /// Called when [column header key down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        internal void OnColumnHeaderKeyDown(object sender, KeyEventArgs e)
        {
            if (((e.Key == Key.Escape) && (m_headerGripper != null)) && m_headerGripper.IsDragging)
            {
                m_headerGripper.CancelDrag();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Hookups the gripper events.
        /// </summary>
        private void HookupGripperEvents()
        {
            UnhookGripperEvents();
            m_headerGripper = base.GetTemplateChild(C_headerGripperTemplateName) as Thumb;

            if (m_headerGripper != null)
            {
                m_headerGripper.DragStarted += new DragStartedEventHandler(OnGripperDragStarted);
                m_headerGripper.DragDelta += new DragDeltaEventHandler(OnGripperDragDelta);
                m_headerGripper.DragCompleted += new DragCompletedEventHandler(OnGripperDragCompleted);
                m_headerGripper.MouseEnter += new MouseEventHandler(OnGripperMouseEnterLeave);
                m_headerGripper.MouseLeave += new MouseEventHandler(OnGripperMouseEnterLeave);
                m_headerGripper.MouseDoubleClick += new MouseButtonEventHandler(OnGripperDoubleClick);
                m_headerGripper.Cursor = SplitCursor;
            }
        }

        private void OnGripperDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ParentTreeView.AllowDynamicResizing)
            {
                ParentTreeView.Flag_Width = true;
                foreach (int key in ParentTreeView.cell_width.Keys)
                {
                    if (key == Column.ActualIndex)
                    {
                        UpdateColumnHeaderWidth(ParentTreeView.cell_width[key]);
                    }
                }
            }
        }

        /// <summary>
        /// Unhooks the gripper events.
        /// </summary>
        private void UnhookGripperEvents()
        {
            if (m_headerGripper != null)
            {
                m_headerGripper.DragStarted -= new DragStartedEventHandler(OnGripperDragStarted);
                m_headerGripper.DragDelta -= new DragDeltaEventHandler(OnGripperDragDelta);
                m_headerGripper.DragCompleted -= new DragCompletedEventHandler(OnGripperDragCompleted);
                m_headerGripper.MouseEnter -= new MouseEventHandler(OnGripperMouseEnterLeave);
                m_headerGripper.MouseLeave -= new MouseEventHandler(OnGripperMouseEnterLeave);
                m_headerGripper.MouseDoubleClick -= new MouseButtonEventHandler(OnGripperDoubleClick);
                m_headerGripper = null;
            }
        }

        /// <summary>
        /// Gets the cursor.
        /// </summary>
        /// <param name="isOpen">if set to <c>true</c> [is open].</param>
        /// <returns>Cursor type </returns>
        private Cursor GetCursor(bool isOpen)
        {
            Cursor cursor = null;
            Assembly ass = Assembly.GetExecutingAssembly();
            Stream stream = null;

            if (EnvironmentTest.IsSecurityGranted)
            {
                stream = isOpen ? ass.GetManifestResourceStream("Syncfusion.Tools.WPF.Controls.TreeViewAdv.Resources.OpenColumn.cur")
                           : stream = ass.GetManifestResourceStream("Syncfusion.Tools.WPF.Controls.TreeViewAdv.Resources.ResizeColumn.cur");
            }

            cursor = (stream != null) ? new Cursor(stream) : Cursors.SizeWE;
            return cursor;
        }

        /// <summary>
        /// Updates the gripper cursor.
        /// </summary>
        private void UpdateGripperCursor()
        {
            if ((m_headerGripper != null) && !m_headerGripper.IsDragging)
            {
                Cursor splitOpenCursor;

                if (DoubleUtil.IsZero(base.ActualWidth))
                {
                    splitOpenCursor = SplitOpenCursor;
                }
                else
                {
                    splitOpenCursor = SplitCursor;
                }

                if (splitOpenCursor != null)
                {
                    m_headerGripper.Cursor = splitOpenCursor;
                }
            }
        }

        /// <summary>
        /// Hides the gripper right half.
        /// </summary>
        /// <param name="hide">if set to <c>true</c> [hide].</param>
        private void HideGripperRightHalf(bool hide)
        {
            if (m_headerGripper != null)
            {
                FrameworkElement parent = m_headerGripper.Parent as FrameworkElement;

                if (parent != null)
                {
                    parent.ClipToBounds = hide;
                }
            }
        }

        /// <summary>
        /// Called when [gripper drag completed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragCompletedEventArgs"/> instance containing the event data.</param>
        private void OnGripperDragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (ParentTreeView != null)
            {
                ParentTreeView.AllowUpdate = true;
                ParentTreeView.allowArrange = false;
            }
            if (e.Canceled)
            {
                UpdateColumnHeaderWidth(m_originalWidth);
            }

            UpdateGripperCursor();
            ParentTreeView.Flag_Width = true;
            e.Handled = true;
        }

        /// <summary>
        /// Called when [gripper drag started].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragStartedEventArgs"/> instance containing the event data.</param>
        private void OnGripperDragStarted(object sender, DragStartedEventArgs e)
        {
            if (ParentTreeView != null)
            {
                ParentTreeView.AllowUpdate = true;
                ParentTreeView.allowArrange = false;
            }
            MakeParentGotFocus();
            m_originalWidth = ColumnActualWidth;
            ParentTreeView.Flag_Width = true;
            e.Handled = true;
        }

        /// <summary>
        /// Called when [gripper drag delta].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        private void OnGripperDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (ParentTreeView != null)
            {
                ParentTreeView.AllowUpdate = true;
                ParentTreeView.allowArrange = false;
            }
            double num = ActualWidth + e.HorizontalChange;
            if (DoubleUtil.LessThanOrClose(num, 0.0))
            {
                num = 0.0;
            }

            UpdateColumnHeaderWidth(num);
            ParentTreeView.Flag_Width = true;
            e.Handled = true;
        }

        /// <summary>
        /// Updates the width of the column header.
        /// </summary>
        /// <param name="width">The width.</param>
        internal void UpdateColumnHeaderWidth(double width)
        {
            if (Column != null)
            {
                double correctWidth = Math.Max(width, Column.MinWidth);
                Column.Width = new GridLength(correctWidth);
            }
            else
            {
                base.Width = width;
            }
        }

        /// <summary>
        /// Makes the parent got focus.
        /// </summary>
        private void MakeParentGotFocus()
        {
            TreeViewHeaderRowPresenter parent = base.Parent as TreeViewHeaderRowPresenter;

            if (parent != null)
            {
                parent.MakeParentItemsControlGotFocus();
            }
        }

        /// <summary>
        /// Called when [gripper mouse enter leave].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void OnGripperMouseEnterLeave(object sender, MouseEventArgs e)
        {
            HandleIsMouseOverChanged();
        }

        /// <summary>
        /// Handles the is mouse over changed.
        /// </summary>
        /// <returns> bool type when hover</returns>
        private bool HandleIsMouseOverChanged()
        {
            if (base.ClickMode != ClickMode.Hover)
            {
                return false;
            }

            if (base.IsMouseOver && ((m_headerGripper == null) || !m_headerGripper.IsMouseOver))
            {
                IsPressedHeader = true;
                OnClick();
            }
            else
            {
                IsPressedHeader = false;
            }

            return true;
        }

        #endregion Implementation
    }
}