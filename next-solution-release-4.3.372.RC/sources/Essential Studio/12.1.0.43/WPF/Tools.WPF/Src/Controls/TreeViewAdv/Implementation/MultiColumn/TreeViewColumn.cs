// <copyright file="TreeViewColumn.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

#region file using

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

#endregion file using

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Class represents the TreeView Column
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    public class TreeViewColumn : DependencyObject, INotifyPropertyChanged
    {
        #region Constants

        /// <summary>
        /// Presents actual width name
        /// </summary>
        internal const string C_actualWidthName = "ActualWidth";

        /// <summary>
        /// Presents member binding
        /// </summary>
        internal const string C_displayMemberBindingName = "DisplayMemberBinding";

        #endregion Constants

        #region Members

        /// <summary>
        /// Presents actual index
        /// </summary>
        private int m_actualIndex;

        public event PropertyChangedCallback SortByChanged;

        public event PropertyChangedCallback ContentAlignmentChanged;

        public event PropertyChangedCallback HeaderContentAlignmentChanged;

        public event PropertyChangedCallback StateChanged;

        /// <summary>
        /// Presents actual width
        /// </summary>
        private double m_actualWidth;

        /// <summary>
        /// Presents desired width
        /// </summary>
        private double m_desiredWidth;

        /// <summary>
        /// Presents member binding
        /// </summary>
        private BindingBase m_displayMemberBinding;

        /// <summary>
        /// Presents state
        /// </summary>
        private ColumnMeasureState m_state;

        #endregion Members

        #region Dependency property

        /// <summary>
        /// Identifies CellTemplate dependency property of the <see cref="TreeViewColumn"/>.
        /// </summary>
        public static readonly DependencyProperty CellTemplateProperty = DependencyProperty.Register("CellTemplate", typeof(DataTemplate), typeof(TreeViewColumn), new PropertyMetadata(new PropertyChangedCallback(TreeViewColumn.OnCellTemplateChanged)));

        /// <summary>
        /// Identifies CellTemplateSelector dependency property of the <see cref="TreeViewColumn"/>.
        /// </summary>
        public static readonly DependencyProperty CellTemplateSelectorProperty = DependencyProperty.Register("CellTemplateSelector", typeof(DataTemplateSelector), typeof(TreeViewColumn), new PropertyMetadata(new PropertyChangedCallback(TreeViewColumn.OnCellTemplateSelectorChanged)));

        /// <summary>
        /// Identifies CellTemplate dependency property of the <see cref="TreeViewColumn"/>.
        /// </summary>
        public static readonly DependencyProperty ColumnHeaderTemplateProperty = DependencyProperty.Register("ColumnHeaderTemplate", typeof(DataTemplate), typeof(TreeViewColumn), new PropertyMetadata(new PropertyChangedCallback(TreeViewColumn.OnColumnHeaderTemplateChanged)));

        /// <summary>
        /// Identifies CellTemplateSelector dependency property of the <see cref="TreeViewColumn"/>.
        /// </summary>
        public static readonly DependencyProperty ColumnHeaderTemplateSelectorProperty = DependencyProperty.Register("ColumnHeaderTemplateSelector", typeof(DataTemplateSelector), typeof(TreeViewColumn), new PropertyMetadata(new PropertyChangedCallback(TreeViewColumn.OnColumnHeaderTemplateSelectorChanged)));

        /// <summary>
        /// Field name have to give for sortby
        /// </summary>
        public string SortBy
        {
            get { return (string)GetValue(SortByProperty); }
            set { SetValue(SortByProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SortBy.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SortByProperty =
            DependencyProperty.Register("SortBy", typeof(string), typeof(TreeViewColumn), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(TreeViewColumn.OnSortbyChanged)));

        /// <summary>
        /// content of row in multicolumn treeview
        /// </summary>
        internal TreeViewRowPresenter RowPresenter
        {
            get { return (TreeViewRowPresenter)GetValue(RowPresenterProperty); }
            set
            {
                SetValue(RowPresenterProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for RowPresenter.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RowPresenterProperty =
            DependencyProperty.Register("RowPresenter", typeof(TreeViewRowPresenter), typeof(TreeViewColumn), new UIPropertyMetadata(null));

        /// <summary>
        /// Header for column
        /// </summary>
        internal TreeViewColumnHeader ColumnHeader
        {
            get { return (TreeViewColumnHeader)GetValue(columnHeaderProperty); }
            set
            {
                SetValue(columnHeaderProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for RowPresenter.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty columnHeaderProperty =
            DependencyProperty.Register("ColumnHeader", typeof(TreeViewColumnHeader), typeof(TreeViewColumn), new UIPropertyMetadata(null));

        /// <summary>
        /// Tree view content alignment
        /// </summary>
        public HorizontalAlignment ContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(ContentAlignmentProperty); }
            set { SetValue(ContentAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentAlignmentProperty =
            DependencyProperty.Register("ContentAlignment", typeof(HorizontalAlignment), typeof(TreeViewColumn), new FrameworkPropertyMetadata(HorizontalAlignment.Left, FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(TreeViewColumn.OnContentAlignmentChanged)));

        /// <summary>
        /// treeview column header content alignment
        /// </summary>
        public HorizontalAlignment HeaderContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(HeaderContentAlignmentProperty); }
            set { SetValue(HeaderContentAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderContentAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderContentAlignmentProperty =
            DependencyProperty.Register("HeaderContentAlignment", typeof(HorizontalAlignment), typeof(TreeViewColumn), new FrameworkPropertyMetadata(HorizontalAlignment.Center, FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(TreeViewColumn.OnHeaderContentAlignmentChanged)));

        /// <summary>
        /// Identifies Header dependency property of the <see cref="TreeViewColumn"/>.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(TreeViewColumn), new FrameworkPropertyMetadata(new PropertyChangedCallback(TreeViewColumn.OnHeaderChanged)));

        /// <summary>
        /// Identifies Width dependency property of the <see cref="TreeViewColumn"/>.
        /// </summary>
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(GridLength), typeof(TreeViewColumn), new PropertyMetadata(TreeViewColumn.OnWidthChanged));

        /// <summary>
        /// Identifies MinWidth dependency property of the <see cref="TreeViewColumn"/>.
        /// </summary>
        public static readonly DependencyProperty MinWidthProperty = DependencyProperty.Register("MinWidth", typeof(double), typeof(TreeViewColumn), new UIPropertyMetadata(0d, new PropertyChangedCallback(TreeViewColumn.OnMinWidthChanged)));

        /// <summary>
        /// Identifies IsSortingEnabledOnHeaderClick dependency property of the <see cref="TreeViewColumn"/>.
        /// </summary>
        public static readonly DependencyProperty IsSortingEnabledOnHeaderClickProperty = DependencyProperty.Register("IsSortingEnabledOnHeaderClick", typeof(bool), typeof(TreeViewColumn), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identifies IsColumnDragDataEnabled dependency property of the <see cref="TreeViewColumn"/>.
        /// </summary>
        public static readonly DependencyProperty IsColumnDragDataEnabledProperty =
            DependencyProperty.RegisterAttached("IsColumnDragDataEnabled", typeof(bool), typeof(TreeViewAdv), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnIsColumnDragDataEnabledChanged)));

        #endregion Dependency property

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is sorting enabled on header click.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is sorting enabled on header click; otherwise, <c>false</c>.
        /// </value>
        public bool IsSortingEnabledOnHeaderClick
        {
            get
            {
                return (bool)GetValue(IsSortingEnabledOnHeaderClickProperty);
            }

            set
            {
                SetValue(IsSortingEnabledOnHeaderClickProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is column drag data enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is column drag data enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsColumnDragDataEnabled
        {
            get
            {
                return (bool)GetValue(IsColumnDragDataEnabledProperty);
            }

            set
            {
                SetValue(IsColumnDragDataEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the cell template.
        /// </summary>
        /// <value>The cell template.</value>
        public DataTemplate CellTemplate
        {
            get
            {
                return (DataTemplate)base.GetValue(CellTemplateProperty);
            }

            set
            {
                base.SetValue(CellTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the cell template selector.
        /// </summary>
        /// <value>The cell template selector.</value>
        public DataTemplateSelector CellTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)base.GetValue(CellTemplateSelectorProperty);
            }

            set
            {
                base.SetValue(CellTemplateSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the column header template selector.
        /// </summary>
        /// <value>The column header template selector.</value>
        public DataTemplateSelector ColumnHeaderTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)base.GetValue(ColumnHeaderTemplateSelectorProperty);
            }

            set
            {
                base.SetValue(ColumnHeaderTemplateSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the column header template.
        /// </summary>
        /// <value>The column header template.</value>
        public DataTemplate ColumnHeaderTemplate
        {
            get
            {
                return (DataTemplate)base.GetValue(ColumnHeaderTemplateProperty);
            }

            set
            {
                base.SetValue(ColumnHeaderTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public GridLength Width
        {
            get
            {
                return (GridLength)base.GetValue(WidthProperty);
            }

            set
            {
                base.SetValue(WidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the min.
        /// </summary>
        /// <value>The width of the min.</value>
        public double MinWidth
        {
            get
            {
                return (double)GetValue(MinWidthProperty);
            }

            set
            {
                SetValue(MinWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public object Header
        {
            get
            {
                return base.GetValue(HeaderProperty);
            }

            set
            {
                base.SetValue(HeaderProperty, value);
            }
        }

        /// <summary>
        /// Gets the actual width.
        /// </summary>
        /// <value>The actual width.</value>
        public double ActualWidth
        {
            get
            {
                return m_actualWidth;
            }

            internal set
            {
                if (((!double.IsNaN(value) && !double.IsInfinity(value))
                    && (value >= 0.0)) && (m_actualWidth != value))
                {
                    m_actualWidth = value;
                    OnPropertyChanged(C_actualWidthName);
                }
            }
        }

        /// <summary>
        /// Gets or sets the display member binding.
        /// </summary>
        /// <value>The display member binding.</value>
        public BindingBase DisplayMemberBinding
        {
            get
            {
                return m_displayMemberBinding;
            }

            set
            {
                if (m_displayMemberBinding != value)
                {
                    m_displayMemberBinding = value;
                    OnDisplayMemberBindingChanged();
                }
            }
        }

        /// <summary>
        /// Gets the width of the desired.
        /// </summary>
        /// <value>The width of the desired.</value>
        internal double DesiredWidth
        {
            get
            {
                return m_desiredWidth;
            }

            private set
            {
                m_desiredWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the actual index.
        /// </summary>
        /// <value>The actual index.</value>
        internal int ActualIndex
        {
            get
            {
                return m_actualIndex;
            }

            set
            {
                m_actualIndex = value;
            }
        }

        public ColumnMeasureState State
        {
            get { return (ColumnMeasureState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for State.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(ColumnMeasureState), typeof(TreeViewColumn), new UIPropertyMetadata(ColumnMeasureState.Init, new PropertyChangedCallback(OnStateChanged)));

        #endregion Properties

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewColumn"/> class.
        /// </summary>
        public TreeViewColumn()
        {
            ResetPrivateData();
            if (m_state != ColumnMeasureState.Star || m_state != ColumnMeasureState.Auto)
                m_state = double.IsNaN(Width.Value) ? ColumnMeasureState.Init : ColumnMeasureState.SpecificWidth;
        }

        #endregion Initialization

        #region Implementation

        /// <summary>
        /// Gets the wrapper column collection.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>TreeViewColumn Collection</returns>
        internal static TreeViewColumnCollection GetWrapperColumnCollection(TreeViewColumnCollection source)
        {
            TreeViewColumnCollection columns = new TreeViewColumnCollection();

            using (IEnumerator<TreeViewColumn> enumerator = source.GetEnumerator())
            {
                int num = 0;

                while (enumerator.MoveNext())
                {
                    TreeViewColumn column2 = GetWrapperColumn(enumerator.Current);
                    columns.Add(column2);
                    num++;
                }
            }

            return columns;
        }

        /// <summary>
        /// Gets the wrapper column.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Tree ViewColumn </returns>
        internal static TreeViewColumn GetWrapperColumn(TreeViewColumn source)
        {
            TreeViewColumn wrapper = GetWrapper(source);
            SetBindinWrapper(wrapper, source, TreeViewColumn.HeaderProperty);
            SetBindinWrapper(wrapper, source, TreeViewColumn.WidthProperty);
            SetBindinWrapper(wrapper, source, TreeViewColumn.MinWidthProperty);
            return wrapper;
        }

        /// <summary>
        /// Gets the wrapper.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>TreeView Column</returns>
        internal static TreeViewColumn GetWrapper(TreeViewColumn source)
        {
            TreeViewColumn wrapColumn = new TreeViewColumn();
            SetBindinWrapper(wrapColumn, source, TreeViewColumn.CellTemplateProperty);
            SetBindinWrapper(wrapColumn, source, TreeViewColumn.CellTemplateSelectorProperty);
            SetBindinWrapper(wrapColumn, source, TreeViewColumn.ContentAlignmentProperty);
            SetBindinWrapper(wrapColumn, source, TreeViewColumn.HeaderContentAlignmentProperty);
            SetBindinWrapper(wrapColumn, source, TreeViewColumn.columnHeaderProperty);
            SetBindinWrapper(wrapColumn, source, TreeViewColumn.StateProperty);
            SetBindinWrapper(wrapColumn, source, TreeViewColumn.RowPresenterProperty);
            wrapColumn.DisplayMemberBinding = source.DisplayMemberBinding;
            return wrapColumn;
        }

        /// <summary>
        /// Sets the binding wrapper.
        /// </summary>
        /// <param name="wrapper">The wrapper.</param>
        /// <param name="source">The source.</param>
        /// <param name="property">The property.</param>
        private static void SetBindinWrapper(TreeViewColumn wrapper, TreeViewColumn source, DependencyProperty property)
        {
            SetBindinWrapper(wrapper, source, property, property);
        }

        /// <summary>
        /// Sets the binding wrapper.
        /// </summary>
        /// <param name="wrapper">The wrapper.</param>
        /// <param name="source">The source.</param>
        /// <param name="wrapperProperty">The wrapper property.</param>
        /// <param name="sourceProperty">The source property.</param>
        private static void SetBindinWrapper(TreeViewColumn wrapper, TreeViewColumn source, DependencyProperty wrapperProperty, DependencyProperty sourceProperty)
        {
            Binding binding = new Binding(sourceProperty.Name);
            binding.Source = source;
            BindingOperations.SetBinding(wrapper, wrapperProperty, binding);
        }

        /// <summary>
        /// Resets the private data.
        /// </summary>
        internal void ResetPrivateData()
        {
            m_actualIndex = -1;
            m_desiredWidth = 0.0;
        }

        /// <summary>
        /// Called when [cell template changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCellTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TreeViewColumn)d).OnPropertyChanged(CellTemplateProperty.Name);
        }

        /// <summary>
        /// Called when [Column Header template changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnColumnHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TreeViewColumn)d).OnPropertyChanged(ColumnHeaderTemplateProperty.Name);
        }

        /// <summary>
        /// Called when [cell template selector changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCellTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TreeViewColumn)d).OnPropertyChanged(CellTemplateSelectorProperty.Name);
        }

        /// <summary>
        /// Called when [Column Header template selector changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnColumnHeaderTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TreeViewColumn)d).OnPropertyChanged(ColumnHeaderTemplateSelectorProperty.Name);
        }

        /// <summary>
        /// Called when [display member binding changed].
        /// </summary>
        private void OnDisplayMemberBindingChanged()
        {
            OnPropertyChanged(C_displayMemberBindingName);
        }

        /// <summary>
        /// Called when [header changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TreeViewColumn)d).OnPropertyChanged(HeaderProperty.Name);
        }

        /// <summary>
        /// Called when [width changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewColumn column = (TreeViewColumn)d;

            double newValue = ((GridLength)e.NewValue).Value;
            if (column.State != ColumnMeasureState.Star && column.State != ColumnMeasureState.Auto)
                column.State = double.IsNaN(newValue) ? ColumnMeasureState.Init : ColumnMeasureState.SpecificWidth;

            column.OnPropertyChanged(WidthProperty.Name);
        }

        /// <summary>
        /// Called when [sortby changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSortbyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TreeViewColumn c = (TreeViewColumn)obj;
            if (c != null)
                c.OnSortByChanged(args);
        }

        /// <summary>
        /// sortybychanged method will called when sortby value changes
        /// </summary>
        /// <param name="args"></param>
        protected void OnSortByChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.SortByChanged != null)
            {
                this.SortByChanged(this, args);
            }
        }

        /// <summary>
        /// Called when [contentalignment changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnContentAlignmentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TreeViewColumn c = (TreeViewColumn)obj;
            if (c != null)
                c.OnContentAlignmentChanged(args);
        }

        /// <summary>
        /// contentalignment method will called when contentalignment value changes
        /// </summary>
        /// <param name="args"></param>
        protected void OnContentAlignmentChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.RowPresenter != null)
            {
                this.RowPresenter.ContentAlignment = (HorizontalAlignment)args.NewValue;
            }
            if (this.ContentAlignmentChanged != null)
            {
                this.ContentAlignmentChanged(this, args);
            }
        }

        /// <summary>
        /// Called when [headercontentalignment changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnHeaderContentAlignmentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TreeViewColumn c = (TreeViewColumn)obj;
            if (c != null)
                c.OnHeaderContentAlignmentChanged(args);
        }

        /// <summary>
        /// method will called when headercontentalignment value changes
        /// </summary>
        /// <param name="args"></param>
        protected void OnHeaderContentAlignmentChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.ColumnHeader != null && this.ColumnHeader.headerContent != null)
            {
                this.ColumnHeader.headerContent.HorizontalAlignment = (HorizontalAlignment)args.NewValue;
            }
            if (this.HeaderContentAlignmentChanged != null)
            {
                this.HeaderContentAlignmentChanged(this, args);
            }
        }

        /// Called when [state changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnStateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TreeViewColumn c = (TreeViewColumn)obj;
            if (c != null)
                c.OnStateChanged(args);
        }

        /// <summary>
        /// method will called when state value changes
        /// </summary>
        /// <param name="args"></param>
        protected void OnStateChanged(DependencyPropertyChangedEventArgs args)
        {
            if (m_state != (ColumnMeasureState)args.NewValue)
            {
                m_state = (ColumnMeasureState)args.NewValue;

                if ((ColumnMeasureState)args.NewValue != ColumnMeasureState.Init)
                {
                    UpdateActualWidth();
                }
                else
                {
                    DesiredWidth = 0.0;
                }
            }
            else if ((ColumnMeasureState)args.NewValue == ColumnMeasureState.SpecificWidth)
            {
                UpdateActualWidth();
            }
            if (this.StateChanged != null)
            {
                this.StateChanged(this, args);
            }
        }

        /// <summary>
        /// Called when [min width changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMinWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewColumn column = (TreeViewColumn)d;

            double newValue = (double)e.NewValue;
            if (column.State != ColumnMeasureState.Star && column.State != ColumnMeasureState.Auto)
                column.State = double.IsNaN(newValue) ? ColumnMeasureState.Init : ColumnMeasureState.SpecificWidth;

            column.OnPropertyChanged(MinWidthProperty.Name);
        }

        private static void OnIsColumnDragDataEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewColumn column = (TreeViewColumn)d;

            column.OnPropertyChanged(IsColumnDragDataEnabledProperty.Name);
        }

        /// <summary>
        /// Updates the actual width.
        /// </summary>
        private void UpdateActualWidth()
        {
            ActualWidth = (this.State == ColumnMeasureState.SpecificWidth) ? Width.Value : DesiredWidth;
        }

        #endregion Implementation

        #region Support INotifyPropertyChanged

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        #endregion Support INotifyPropertyChanged
    }
}