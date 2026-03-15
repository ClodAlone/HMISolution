#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections;
using System.Windows.Input;

using System.Windows.Media;

#if !SILVERLIGHT
using Syncfusion.Licensing;
using Syncfusion.PivotAnalysis.Base;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Controls.PivotGrid;
using System.Reflection;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Controls.PivotSchemaDesigner
#else
using Syncfusion.Silverlight.Controls.PivotGrid;
using System.Reflection;
using Syncfusion.PivotAnalysis.Base.Silverlight;
using Syncfusion.Windows.Shared.Controls;
using Syncfusion.Windows.Tools.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using Syncfusion.Windows.Controls.Theming;
namespace Syncfusion.Silverlight.Controls.PivotSchemaDesigner
#endif

{
    /// <summary>
    /// This control enables Excel like environment to the pivot the data
    /// </summary>
#if SILVERLIGHT
    public class PivotSchemaDesigner : WindowControl
#else
     [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(PivotSchemaDesigner), XamlResource = "/Syncfusion.Shared.Wpf;component/SkinManager/BlendStyle.xaml")]
     [SkinType(SkinVisualStyle = Skin.Metro,
    Type = typeof(PivotSchemaDesigner), XamlResource = "/Syncfusion.Shared.Wpf;component/SkinManager/MetroStyle.xaml")]
     [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(PivotSchemaDesigner), XamlResource = "/Syncfusion.Shared.Wpf;component/SkinManager/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
    Type = typeof(PivotSchemaDesigner), XamlResource = "/Syncfusion.Shared.Wpf;component/SkinManager/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(PivotSchemaDesigner), XamlResource = "/Syncfusion.Shared.Wpf;component/SkinManager/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(PivotSchemaDesigner), XamlResource = "/Syncfusion.Shared.Wpf;component/SkinManager/Office2007BlackStyle.xaml")]
     [SkinType(SkinVisualStyle = Skin.Office2010Blue,
    Type = typeof(PivotSchemaDesigner), XamlResource = "/Syncfusion.Shared.Wpf;component/SkinManager/Office2010BlueStyle.xaml")]
     [SkinType(SkinVisualStyle = Skin.Office2010Silver,
     Type = typeof(PivotSchemaDesigner), XamlResource = "/Syncfusion.Shared.Wpf;component/SkinManager/Office2010SilverStyle.xaml")]
     [SkinType(SkinVisualStyle = Skin.Office2010Black,
     Type = typeof(PivotSchemaDesigner), XamlResource = "/Syncfusion.Shared.Wpf;component/SkinManager/Office2010BlackStyle.xaml")]
     [SkinType(SkinVisualStyle = Skin.Transparent,
     Type = typeof(PivotSchemaDesigner), XamlResource = "/Syncfusion.Shared.Wpf;component/SkinManager/TransparentStyle.xaml")]
    public class PivotSchemaDesigner : Control
#endif
    {
        #region [ Private Variables ]

        ListBox dragSource = null;
        bool resumeProcessing = false;
        private bool isHooked = true;
        #endregion

        #region [ Initilize / Finalize ]

        /// <summary>
        /// Initializes the <see cref="PivotSchemaDesigner"/> class.
        /// </summary>
        static PivotSchemaDesigner()
        {
#if !SILVERLIGHT
            EnvironmentTest.ValidateLicense(typeof(PivotGridControl));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PivotSchemaDesigner), new FrameworkPropertyMetadata(typeof(PivotSchemaDesigner)));
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PivotSchemaDesigner"/> class.
        /// </summary>
        public PivotSchemaDesigner()
        {
#if !SILVERLIGHT
            EnvironmentTest.ValidateLicense(typeof(PivotGridControl));

            // Adding the command binding to CommandBindings collection

            // Adding delete pivot item command
            CommandBindings.Add(new CommandBinding(PivotCommands.DeletePivotItem, DeletePivotItemExecuted, DeletePivotItemCanExecute));
            //// Adding delete filter item command
            CommandBindings.Add(new CommandBinding(PivotCommands.DeleteFilter, DeleteFilterExecuted, DeleteFilterCanExecute));
            //// Adding show filter popup command
            CommandBindings.Add(new CommandBinding(PivotCommands.ShowFilter, ShowFilterExecuted, ShowFilterCanExecute));
#else
            DefaultStyleKey = typeof(PivotSchemaDesigner);
#endif
            this.DataContext = this;

            //// Initializing the collections
            this.PivotTableFields = new ObservableCollection<PivotTableField>();
            this.Filters = new ObservableCollection<FilterItemsCollection>();
            this.Loaded += new RoutedEventHandler(PivotSchemaDesigner_Loaded);
        }

        void PivotSchemaDesigner_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.PivotControl != null)
            {
#if SILVERLIGHT
                if (this.PivotControl is PivotGridControl)
                {
                    (this.PivotControl as PivotGridControl).ApplyTemplate();
                }
#endif
                this.PivotControl.Filters.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Filters_CollectionChanged);
                this.PivotControl.Filters.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Filters_CollectionChanged);
                this.PivotControl.PivotCalculations.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(PivotCalculations_CollectionChanged);
                this.PivotControl.PivotCalculations.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(PivotCalculations_CollectionChanged);
                this.PivotControl.PivotColumns.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(PivotItem_CollectionChanged);
                this.PivotControl.PivotColumns.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(PivotItem_CollectionChanged);
                this.PivotControl.PivotRows.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(PivotItem_CollectionChanged);
                this.PivotControl.PivotRows.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(PivotItem_CollectionChanged);
                this.PivotControl.PivotFields.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(PivotItem_CollectionChanged);
                this.PivotControl.PivotFields.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(PivotItem_CollectionChanged);
                this.PivotControl.ShowDisabledGroupBackgroundPropertyChanged += new EventHandler(PivotControl_ShowDisabledGroupBackgroundPropertyChanged);
                if (this.PivotControl.PivotEngine.LoadInBackground)
                    this.IsEnabled = false;
                if (this.PivotControl is PivotGridControl)
                {
                    PivotGridControl gridControl = (this.PivotControl as PivotGridControl);
                    this.PivotControl.PivotEngine.PropertyChanged += new PropertyChangedEventHandler(PivotEngine_PropertyChanged);
                    if (gridControl.GroupingBar != null)
                    {
                        gridControl.GroupingBar.OnFilterItemChanged += new EventHandler(GroupingBar_OnFilterItemChanged);
                    }
#if !SILVERLIGHT
                    if (gridControl.RowPivotsOnly)
                    {
                        this.PivotColumnList.IsEnabled = this.FilterList.IsEnabled = this.ShowCalculationsAsColumnCheckBox.IsEnabled = false;
                    }
#endif
                }
            }
        }

        void PivotEngine_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LoadInBackground")
            {
                if ((sender as PivotEngine).LoadInBackground)
                    this.IsEnabled = false;
                else
                    this.IsEnabled = true;
            }
        }

        void GroupingBar_OnFilterItemChanged(object sender, EventArgs e)
        {

            PivotGridGroupingBar pivotGridGroupingBar = (sender as PivotGridGroupingBar);
            if (this.PivotTableFieldList != null)
            {
                ObservableCollection<PivotTableField> sourceFields = this.PivotTableFieldList.ItemsSource as ObservableCollection<PivotTableField>;
                if (sourceFields != null)
                {
                   
#if!SILVERLIGHT
                    for (int j = 0; j < pivotGridGroupingBar.Filters.Count; j++)
                    {
                        PivotTableField field = sourceFields.Where(i => i.FieldName == pivotGridGroupingBar.Filters[j].DisplayHeader).FirstOrDefault();
                        if (field != null && pivotGridGroupingBar.Filters.Count == 0)
                        {
                            if (field.IsSelected)
                                field.IsSelected = false;
                        }
                    }
            
#endif

                        if (pivotGridGroupingBar != null)
                        {
                            if (pivotGridGroupingBar.Filters.Count > 0 && pivotGridGroupingBar.Filters.Count > this.Filters.Count)
                            {
                                foreach (var _filterItem in pivotGridGroupingBar.Filters)
                                {
                                    if (!this.Filters.Any(i => i.DisplayHeader == _filterItem.DisplayHeader))
                                    {
                                        if (_filterItem != null && this.Filters.Count != pivotGridGroupingBar.Filters.Count)
                                        {
                                            this.Filters.Insert(pivotGridGroupingBar.Filters.IndexOf(_filterItem), _filterItem);
                                            UpdatePivotTableFieldItemSelection(_filterItem.DisplayHeader);
                                            PivotGridControl.FilterIndex++;
                                        }
                                    }
                                }
                            }
                            else if (this.Filters.Count > 0 && pivotGridGroupingBar.Filters.Count < this.Filters.Count)
                            {
                                PivotGridControl.FilterIndex--;
                                this.Filters.Clear();
                                foreach (var filter in pivotGridGroupingBar.Filters)
                                    this.Filters.Add(filter);
                            }
                        }
                    if (pivotGridGroupingBar.Filters.Count == 0)
                    {
                        this.Filters.Clear();
                        PivotGridControl.FilterIndex = 0;
                        
                    } 
#if SILVERLIGHT
                    //RegisterAllCommands();
#endif
                }
            }

            else if (pivotGridGroupingBar != null)
            {
                if (pivotGridGroupingBar.Filters.Count > 0 && pivotGridGroupingBar.Filters.Count > this.Filters.Count)
                {
                    foreach (var _filterItem in pivotGridGroupingBar.Filters)
                    {
                        if (!this.Filters.Any(i => i.DisplayHeader == _filterItem.DisplayHeader))
                        {
                            if (_filterItem != null && this.Filters.Count != pivotGridGroupingBar.Filters.Count)
                            {
                                this.Filters.Insert(pivotGridGroupingBar.Filters.IndexOf(_filterItem), _filterItem);
                                UpdatePivotTableFieldItemSelection(_filterItem.DisplayHeader);
                                PivotGridControl.FilterIndex++;
                            }
                        }
                    }
                }
            }
        }

        void PivotControl_ShowDisabledGroupBackgroundPropertyChanged(object sender, EventArgs e)
        {
            DisableControls();
        }

        private void DisableControls()
        {
            PivotGridControl grid = this.PivotControl as PivotGridControl;
            {
                if (PivotCalculationList != null)
                    for (var i = 0; i < PivotCalculationList.Items.Count; i++)
                    {
                        ListBoxItem item = this.PivotCalculationList.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                        PivotGridGroupingBar.SetDisabled(item, grid);
                    }
                if (PivotRowList != null)
                    for (int i = 0; i < this.PivotRowList.Items.Count; i++)
                    {
                        ListBoxItem item = this.PivotRowList.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                        PivotGridGroupingBar.SetDisabled(item, grid);
                    }
                if (PivotColumnList != null)
                    for (int i = 0; i < this.PivotColumnList.Items.Count; i++)
                    {
                        ListBoxItem item = this.PivotColumnList.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                        PivotGridGroupingBar.SetDisabled(item, grid);
                    }
            }
        }

        void PivotItem_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null)
            {
                if (e.OldItems != null)
                {
                    foreach (PivotItem item in e.OldItems)
                    {
                        UpdatePivotTableFieldItemSelection(item.FieldMappingName);
                    }
                }
            }
            else
            {
                foreach (PivotItem item in e.NewItems)
                {
                    UpdatePivotTableFieldItemSelection(item.FieldMappingName);
                }
            }
#if !SILVERLIGHT
            this.InvalidateVisual();
#endif
        }

        void PivotCalculations_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null)
            {
                if (e.OldItems != null)
                {
                    foreach (PivotComputationInfo item in e.OldItems)
                    {
                        UpdatePivotTableFieldItemSelection(item.FieldName);
                    }
                }
            }
            else
            {
                foreach (PivotComputationInfo compInfo in e.NewItems)
                {
                    UpdatePivotTableFieldItemSelection(compInfo.FieldName);
                }
            }
#if !SILVERLIGHT
            this.InvalidateVisual();
#endif
        }

        void Filters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PivotGridControl grid = this.PivotControl as PivotGridControl;
            if (grid != null && this.PivotTableFieldList != null)
            {
                PivotGridGroupingBar _pivotGridGroupingBar = grid.GroupingBar;
                ObservableCollection<PivotTableField> sourceFields = this.PivotTableFieldList.ItemsSource as ObservableCollection<PivotTableField>;
                if (_pivotGridGroupingBar != null && _pivotGridGroupingBar.Filters.Count > 0 && _pivotGridGroupingBar.Filters.Count > this.Filters.Count)
                {
                    this.Filters.Clear();
                    foreach (var _filterItem in _pivotGridGroupingBar.Filters)
                    {
                        if (!this.Filters.Any(i=>i.DisplayHeader == _filterItem.DisplayHeader))
                        {
                            if (_filterItem != null && this.Filters.Count != _pivotGridGroupingBar.Filters.Count)
                            {
                                this.Filters.Add(_filterItem);
                                UpdatePivotTableFieldItemSelection(_filterItem.DisplayHeader);
                                PivotGridControl.FilterIndex++;
                            }
                        }
                    }
                }
                
                else if (_pivotGridGroupingBar != null && this.Filters.Count > 0 && _pivotGridGroupingBar.Filters.Count < this.Filters.Count)
                {
                    if (sourceFields != null && e.OldItems != null)
                    {
#if !SILVERLIGHT
                        var field = sourceFields.Where(i => i.FieldName == (e.OldItems[0] as Syncfusion.PivotAnalysis.Base.FilterExpression).DimensionHeader).FirstOrDefault();
#else
                        var field = sourceFields.Where(i => i.FieldName == (e.OldItems[0] as Syncfusion.PivotAnalysis.Base.Silverlight.FilterExpression).DimensionHeader).FirstOrDefault();
#endif
                        if (field != null)
                        {
                            field.IsSelected = false;
                        }
                    }

                    this.Filters.Clear();
                    foreach (var _filter in _pivotGridGroupingBar.Filters)
                    {
                        this.Filters.Add(_filter);

                    }

                }

                else
                {
                    if (sourceFields != null && e.OldItems != null)
                    {
#if !SILVERLIGHT
                        var field = sourceFields.Where(i => i.FieldName == (e.OldItems[0] as Syncfusion.PivotAnalysis.Base.FilterExpression).DimensionHeader).FirstOrDefault();
#else
                        var field = sourceFields.Where(i => i.FieldName == (e.OldItems[0] as Syncfusion.PivotAnalysis.Base.Silverlight.FilterExpression).DimensionHeader).FirstOrDefault();

#endif
                        if (field != null)
                        {
                            field.IsSelected = false;
                        }
                    }

                }
            }
        }
    
        private bool FilterContains(string p)
        {
            foreach (var item in this.Filters)
            {
                if (item.Name == p)
                    return true;
            }

            return false;
        }

        #endregion

        #region [ Dependency property declaration ]

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotSchemaDesigner.PivotSchemaDesigner.PivotControl"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotSchemaDesigner.PivotSchemaDesigner.PivotControl"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty PivotControlProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("PivotControl", typeof(IPivotControl), typeof(PivotSchemaDesigner), new UIPropertyMetadata(null));
#else

 DependencyProperty.Register("PivotControl", typeof(IPivotControl), typeof(PivotSchemaDesigner), new PropertyMetadata((s, e) =>
 {
     PivotSchemaDesigner schemaDesigner = s as PivotSchemaDesigner;
     schemaDesigner.OnApplyTemplate();
     schemaDesigner.PivotSchemaDesigner_Loaded(schemaDesigner, new RoutedEventArgs());
 }));
#endif
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotSchemaDesigner.PivotSchemaDesigner.ShowDisplayFieldsOnly"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotSchemaDesigner.PivotSchemaDesigner.ShowDisplayFieldsOnly"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ShowDisplayFieldsOnlyProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("ShowDisplayFieldsOnly", typeof(bool), typeof(PivotSchemaDesigner), new UIPropertyMetadata(false,
#else
 DependencyProperty.Register("ShowDisplayFieldsOnly", typeof(bool), typeof(PivotSchemaDesigner), new PropertyMetadata(false,
#endif
 (s, e) =>
 {
     PivotSchemaDesigner schemaDesigner = s as PivotSchemaDesigner;
     schemaDesigner.PivotTableFields.Clear();
#if !SILVERLIGHT
                    schemaDesigner.PivotTableFieldList.ClearValue(ListBox.ItemsSourceProperty);
#endif
     schemaDesigner.InitilizePivotTableList();
 }));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotSchemaDesigner.PivotSchemaDesigner.CustomSummaryBaseCollection"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotSchemaDesigner.PivotSchemaDesigner.CustomSummaryBaseCollection"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty CustomSummaryBaseCollectionProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("CustomSummaryBaseCollection", typeof(ObservableCollection<SummaryBase>), typeof(PivotSchemaDesigner), new UIPropertyMetadata(null));
#else
 DependencyProperty.Register("CustomSummaryBaseCollection", typeof(ObservableCollection<SummaryBase>), typeof(PivotSchemaDesigner), new PropertyMetadata(null));
#endif


#if SILVERLIGHT
        /// <summary>
        /// Gets or sets the visual style for PivotSchemaDesigner.
        /// </summary>
        public new Syncfusion.Windows.Controls.Theming.VisualStyle VisualStyle
        {
            get { return (Syncfusion.Windows.Controls.Theming.VisualStyle)GetValue(VisualStyleProperty); }
            set { SetValue(VisualStyleProperty, value); }
        }
#else
        /// <summary>
        /// Gets or sets the visual style for PivotSchemaDesigner.
        /// </summary>
        public PivotGridVisualStyle VisualStyle
        {
            get { return (PivotGridVisualStyle)GetValue(VisualStyleProperty); }
            set { SetValue(VisualStyleProperty, value); }
        }
#endif

        // Using a DependencyProperty as the backing store for VisualStyle.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotSchemaDesigner.PivotSchemaDesigner.VisualStyle"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotSchemaDesigner.PivotSchemaDesigner.VisualStyle"/> dependency property.
        /// </returns>
       
#if SILVERLIGHT
        public new static readonly DependencyProperty VisualStyleProperty =
 DependencyProperty.Register("VisualStyle", typeof(Syncfusion.Windows.Controls.Theming.VisualStyle), typeof(PivotSchemaDesigner), new PropertyMetadata(Syncfusion.Windows.Controls.Theming.VisualStyle.Default, new PropertyChangedCallback((dependencyObject, args) =>
            {
                PivotSchemaDesigner pivotSchemaDesigner = dependencyObject as PivotSchemaDesigner;
                if (pivotSchemaDesigner != null)
                {
                    pivotSchemaDesigner.ApplyVisualStyle(args.NewValue.ToString());
                }
            })));
#else
         public static readonly DependencyProperty VisualStyleProperty =
 DependencyProperty.Register("VisualStyle", typeof(PivotGridVisualStyle), typeof(PivotSchemaDesigner), new PropertyMetadata(PivotGridVisualStyle.Default,new PropertyChangedCallback((dependencyObject, args) =>
            {
                PivotSchemaDesigner pivotSchemaDesigner = dependencyObject as PivotSchemaDesigner;
                if (pivotSchemaDesigner != null)
                {
                    pivotSchemaDesigner.ApplyVisualStyle(args.NewValue.ToString());
                }
            })));
#endif

        #endregion

        #region [ Properties ]


        /// <summary>
        /// Gets or sets the PivotAnalysis(IPivotControl) control
        /// </summary>
        [Browsable(false)]
        public IPivotControl PivotControl
        {
            get { return (IPivotControl)GetValue(PivotControlProperty); }
            set { SetValue(PivotControlProperty, value); }
        }

#if SILVERLIGHT
        internal Popup Indicator { get; set; }
#endif
        /// <summary>
        /// Gets or sets a value indicating whether to show only DisplayFields.
        /// </summary>
        [Browsable(false)]
        public bool ShowDisplayFieldsOnly
        {
            get { return (bool)GetValue(ShowDisplayFieldsOnlyProperty); }
            set { SetValue(ShowDisplayFieldsOnlyProperty, value); }
        }

        /// <summary>
        /// Gets or sets the PivotTableField ListBox
        /// </summary>
        [Browsable(false)]
        public ListBox PivotTableFieldList { get; set; }

        /// <summary>
        /// Gets or sets the PivotCalculation ListBox
        /// </summary>
        [Browsable(false)]
        public ListBox PivotCalculationList { get; set; }

        /// <summary>
        /// Gets or sets the PivotColumn ListBox
        /// </summary>
        [Browsable(false)]
        public ListBox PivotColumnList { get; set; }

#if !SILVERLIGHT
        private ObservableCollection<PivotComputationInfo> PossiblePivotCalculations { get; set; }
#endif

        /// <summary>
        /// Gets or sets the PivotRow ListBox
        /// </summary>
        [Browsable(false)]
        public ListBox PivotRowList { get; set; }

        /// <summary>
        /// Gets or sets the Filters Listbox
        /// </summary>
        [Browsable(false)]
        public ListBox FilterList { get; set; }

        /// <summary>
        /// Gets or sets the ShowCalculationAsColumn CheckBox
        /// </summary>
        [Browsable(false)]
        public CheckBox ShowCalculationsAsColumnCheckBox { get; set; }

        /// <summary>
        /// Gets or sets the DeferLayoutUpdate check box
        /// </summary>
        [Browsable(false)]
        public CheckBox DeferLayoutUpdateCheckBox { get; set; }

        /// <summary>
        /// Gets or sets the DeferLayoutUpdate button
        /// </summary>
        [Browsable(false)]
        public Button DeferLayoutUpdateButton { get; set; }

        /// <summary>
        /// Gets or sets the Custom SummaryBase Collection to use in Pivot Computation Information dialog.
        /// </summary>
        [Browsable(false)]
        public ObservableCollection<SummaryBase> CustomSummaryBaseCollection
        {
            get { return (ObservableCollection<SummaryBase>)GetValue(CustomSummaryBaseCollectionProperty); }
            set { SetValue(CustomSummaryBaseCollectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the filters collection
        /// </summary>
        [Browsable(false)]
#if SILVERLIGHT
         public ObservableCollection<Syncfusion.PivotAnalysis.Base.Silverlight.FilterItemsCollection> Filters { get; set; }
#else
         public ObservableCollection<Syncfusion.PivotAnalysis.Base.FilterItemsCollection> Filters { get; set; }

#endif
        /// <summary>
        /// Gets or sets the PivotTableFields
        /// </summary>
        [Browsable(false)]
        public ObservableCollection<PivotTableField> PivotTableFields { get; set; }

        #endregion

        #region [ Overrides ]
        /// <summary>
        /// An overridden method that applies template, visual style to PivotSchemaDesigner.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PivotTableFieldList = GetTemplateChild("PART_PivotTableList") as ListBox;
            this.FilterList = GetTemplateChild("PART_FilterList") as ListBox;
            this.PivotColumnList = GetTemplateChild("PART_PivotColumnList") as ListBox;
            this.PivotRowList = GetTemplateChild("PART_PivotRowList") as ListBox;
            this.PivotCalculationList = GetTemplateChild("PART_PivotCalculationList") as ListBox;
            this.ShowCalculationsAsColumnCheckBox = GetTemplateChild("PART_ShowCalculationsAsColumn") as CheckBox;
            this.DeferLayoutUpdateCheckBox = GetTemplateChild("PART_DeferLayoutUpdate") as CheckBox;
            this.DeferLayoutUpdateButton = GetTemplateChild("PART_DeferLayoutUpdateButton") as Button;
#if !SILVERLIGHT
            if (this.PivotCalculationList != null)
            {
                if ((this.PivotControl != null ) && (this.PivotControl as PivotGridControl)!=null && (this.PivotControl as PivotGridControl).RowPivotsOnly)
                {
                    this.PossiblePivotCalculations = new ObservableCollection<PivotComputationInfo>();
                    foreach (PivotComputationInfo pi in (this.PivotControl as PivotGridControl).PossiblePivotCalculations)
                    {
                        if (!(this.PivotControl.PivotRows.Any(i => i.FieldMappingName == pi.FieldName)))
                        {
                            this.PossiblePivotCalculations.Add(pi);
                        }
                        PivotValueField pv = (this.PivotControl as PivotGridControl).LocalPossibleCalculations.Where(p=>p.FieldName == pi.FieldName).FirstOrDefault();
                        if (pv.FieldName == pi.FieldName)
                            pv.IsSelected = true;
                    }
                    this.PivotCalculationList.ItemsSource = this.PossiblePivotCalculations;
                }
                else
                {
                    if (this.PivotControl != null && (this.PivotControl as PivotGridControl) != null && this.PivotControl.PivotCalculations != null)
                        this.PivotCalculationList.ItemsSource = this.PivotControl.PivotCalculations;
                }

            }
            ApplyVisualStyle(this.VisualStyle.ToString());
#else
            ApplyVisualStyle(this.VisualStyle.ToString());
            this.Indicator = GetTemplateChild("PART_Indicator") as Popup;
#endif

            //// Wiring events
            this.WireEvents();

            //// Binding the data in PivotTable field list
            this.InitilizePivotTableList();
        }

        /// <summary>
        /// An overridden method to arrange and size the content of a <see cref="T:Syncfusion.Windows.Controls.PivotSchemaDesigner.PivotSchemaDesigner"/> object.
        /// </summary>
        /// <param name="arrangeBounds">computed size that is used to arrange the content.</param>
        /// <returns>Size of the control.</returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
#if SILVERLIGHT
            RegisterCommands(this.PivotCalculationList);
            RegisterCommands(this.PivotColumnList);
            RegisterCommands(this.PivotRowList);
            RegisterFilterCommands(this.FilterList);
#endif
            DisableControls();
            return base.ArrangeOverride(arrangeBounds);
        }

#if SILVERLIGHT


#endif
        #endregion

        #region [ Methods ]

        /// <summary>
        /// Binds the PivotControl ItemSource property descriptor to the PivotTableList
        /// </summary>
#if !SILVERLIGHT
        public void InitilizePivotTableList()
        {
            if (this.PivotTableFieldList != null && this.PivotControl != null)
            {
                if (this.PivotTableFields.Count > 0)
                    this.PivotTableFields.Clear();

                PropertyDescriptorCollection propertyDescriptorCollection = null;
                if (this.PivotControl.PivotEngine != null && this.PivotControl.PivotEngine.ItemProperties.Count > 0)
                {
                    propertyDescriptorCollection = this.PivotControl.PivotEngine.ItemProperties;
                }
                else
                {
                    //// Gets the first item from the collection and then getting the
                    //// property descriptor from collection
                    IEnumerable source = null;
                    if (this.PivotControl.ItemSource is System.Data.DataTable)
                        source = (this.PivotControl.ItemSource as System.Data.DataTable).DefaultView;
                    else
                        source = this.PivotControl.ItemSource as IEnumerable;
                    if (source != null)
                    {
                        object firstItem = null;
                        foreach (object item in source)
                        {
                            firstItem = item;
                            break;
                        }
                        if (firstItem != null)
                        {
                            propertyDescriptorCollection = TypeDescriptor.GetProperties(firstItem);
                        }
                    }
                }
                if (propertyDescriptorCollection != null)
                {
                    PivotTableField sp;
                    if (ShowDisplayFieldsOnly)
                    {
                        foreach (var item in this.PivotControl.PivotCalculations)
                        {
                            this.PivotTableFields.Add(sp = new PivotTableField
                            {
                                FieldHeader = GetPivotTableFieldHeader(item.FieldName),
                                Format = GetPivotTableFormat(item.FieldName, propertyDescriptorCollection[item.FieldName].PropertyType),
                                FieldPropertyDescriptor = propertyDescriptorCollection[item.FieldName],
                                IsSelected = GetPivotTableFieldIsSelected(item.FieldName),
                                SummaryType = item.SummaryType,
                                Summary = item.Summary,
                                TotalHeader = GetPivotTableFieldTotalHeader(item.FieldName),
                                AllowRunTimeGroupByField = item.AllowRunTimeGroupByField
                            });
                            InvokePivotTableFieldSettings(sp);
                        }

                        foreach (var item in this.PivotControl.PivotColumns)
                        {
                            sp = AddPivotItemToTableFields(propertyDescriptorCollection, item);
                            InvokePivotTableFieldSettings(sp);
                        }

                        foreach (var item in this.PivotControl.PivotRows)
                        {
                            sp = AddPivotItemToTableFields(propertyDescriptorCollection, item);
                            InvokePivotTableFieldSettings(sp);
                        }

                        foreach (var item in this.PivotControl.PivotFields)
                        {
                            sp = AddPivotItemToTableFields(propertyDescriptorCollection, item);
                            InvokePivotTableFieldSettings(sp);
                        }
                    }
                    else
                    {
                        foreach (PropertyDescriptor pd in propertyDescriptorCollection)
                        {
                            var pivotComputationInfo = this.PivotControl.PivotCalculations.Where(p => p.FieldName == pd.Name).FirstOrDefault();
                            this.PivotTableFields.Add(sp = new PivotTableField
                            {
                                FieldHeader = GetPivotTableFieldHeader(pd.Name),
                                Format = GetPivotTableFormat(pd.Name, pd.PropertyType),
                                FieldPropertyDescriptor = pd,
                                Summary = pivotComputationInfo is PivotComputationInfo ? pivotComputationInfo.Summary : null,
                                IsSelected = GetPivotTableFieldIsSelected(pd.Name),
                                SummaryType = pivotComputationInfo is PivotComputationInfo ? pivotComputationInfo.SummaryType : GetPivotTableSummaryType(pd.PropertyType),
                                TotalHeader = GetPivotTableFieldTotalHeader(pd.Name),
                                AllowRunTimeGroupByField = GetPivotTableAllowGrouping(pd.Name)
                            });
                            InvokePivotTableFieldSettings(sp);
                        }
                    }
                }
            }
            if (this.PivotControl is PivotGridControl && this.PivotControl.Filters.Count > 0 && this.Filters.Count == 0)
            {
                for (int i = 0; i < this.PivotControl.Filters.Count; i++)
                {
                    this.PivotControl.PivotEngine.AddFilter(this.PivotControl.Filters[i]);
                    this.Filters.Insert(i, this.PivotControl.PivotEngine.ItemCollection);
                    if (this.Filters[i].DisplayHeader == null)
                        this.Filters[i].DisplayHeader = this.PivotControl.Filters[i].DimensionName;
                    UpdatePivotTableFieldItemSelection(this.PivotControl.Filters[i].DimensionName);
                    PivotGridControl.FilterIndex++;
                }
            }  
        }
#else
        private ListBox DropTarget { get; set; }

        private void DragAndDropManager_DragStarted_SchemaDesigner(object sender, DragDropEventArgs args)
        {
            if (!(args.DragSource is ListBox))
            {
                if (sender is ListBox)
                {
                    dragSource = (sender as ListBox);
                    args.PayLoad = dragSource.SelectedItem;
                }
                else
                    return;
            }
            else
            {
                ListBox source = (ListBox)args.DragSource;
                dragSource = source;
            }
        }

        private void DragAndDropManager_Drag_SchemaDesigner(object sender, DragDropEventArgs args)
        {
            if (this.Indicator == null) return;
            if (this.Indicator.IsOpen)
                this.Indicator.IsOpen = false;
            if (args.DragSource != null && args.DragSource is PivotSchemaDesigner)
            {
                this.Indicator.IsOpen = false;
                this.ItemIndex = -2;
                return;
            }

            args.DropDescription = null;
            ListBoxItem newitem = args.DropTarget as ListBoxItem;
            if (args.PayLoad == null && newitem != null)
                args.PayLoad = newitem.Content;
            Point dragPoint = args.MouseEventArgs.GetPosition(Application.Current.RootVisual as UIElement);
            var items = VisualTreeHelper.FindElementsInHostCoordinates(dragPoint, Application.Current.RootVisual as UIElement);
            if (items.Count() == 0)
            {
                var popupCollection = VisualTreeHelper.GetOpenPopups();
                if (popupCollection != null)
                {
                    foreach (var popups in popupCollection)
                    {
                        if (popups.Child is ChildWindow)
                        {
                            items = VisualTreeHelper.FindElementsInHostCoordinates(dragPoint, Application.Current.RootVisual as ChildWindow);
                        }
                    }
                }
            }
            if (items.Count() > 0)
            {
                ToggleButton item = items.Where(i => i.GetType() == typeof(ToggleButton)).FirstOrDefault() as ToggleButton;
                if (item != null && item.Tag != args.PayLoad)
                {
                    Point buttonRelativePosition = args.MouseEventArgs.GetPosition(item);
                    (this.Indicator.Child as Line).X2 = item.ActualWidth;
                    int currentItemIndex = -5;
                    ListBox ctrl = items.Where(j => j.GetType() == typeof(ListBox)).FirstOrDefault() as ListBox;
                    this.ItemIndex = GetItemIndex(item, ctrl, args.PayLoad, out currentItemIndex);
                    if (item.ActualHeight > 0 && ((item.ActualHeight / 2) > buttonRelativePosition.Y))
                    {
                        GeneralTransform transform = item.TransformToVisual(this as UIElement);
                        this.Indicator.IsOpen = false;
                        this.Indicator.HorizontalOffset = transform.Transform(new Point(0, 0)).X;
                        this.Indicator.VerticalOffset = transform.Transform(new Point(0, 0)).Y;
                        if (ctrl != null)
                            this.ItemIndex = ctrl.Items.IndexOf(item.Tag);
                    }
                    else
                    {
                        if (ItemIndex + 1 == currentItemIndex)
                        {
                            this.Indicator.IsOpen = false;
                            this.ItemIndex = -2;
                            return;
                        }

                        GeneralTransform transform = item.TransformToVisual(this as UIElement);
                        this.Indicator.IsOpen = true;
                        this.Indicator.HorizontalOffset = transform.Transform(new Point(0, 0)).X;
                        this.Indicator.VerticalOffset = transform.Transform(new Point(0, 0)).Y;

                        if (ctrl != null)
                            this.ItemIndex = ctrl.Items.IndexOf(item.Tag) + 1;
                    }

                    this.DropTarget = ctrl;
                }
                else
                {
                    if (item == null)
                    {
                        ListBox ctrl = items.Where(j => j.GetType() == typeof(ListBox)).FirstOrDefault() as ListBox;
                        if (ctrl != null && ((IList)ctrl.ItemsSource).Count > 0)
                        {
                            ListBoxItem lstItem = (ListBoxItem)ctrl.ItemContainerGenerator.ContainerFromIndex(((IList)ctrl.ItemsSource).Count - 1);
                            if (lstItem != null)
                            {
                                (this.Indicator.Child as Line).X2 = lstItem.ActualWidth;
                                double height = lstItem.ActualHeight;
                                GeneralTransform transform = lstItem.TransformToVisual(this as UIElement);
                                this.Indicator.IsOpen = true;
                                this.Indicator.HorizontalOffset = (transform.Transform(new Point(0, 0)).X);
                                this.Indicator.VerticalOffset = transform.Transform(new Point(0, 0)).Y + height;
                            }
                        }

                        else
                        {
                            this.DropTarget = null;
                            this.ItemIndex = -2;
                            this.Indicator.IsOpen = false;
                        }
                    }
                    else
                    {
                        this.DropTarget = null;
                        this.ItemIndex = -2;
                        this.Indicator.IsOpen = false;
                    }
                }
            }
            else
            {
                this.ItemIndex = -2;
                this.Indicator.IsOpen = false;
            }
        }
        private int GetItemIndex(ToggleButton item, ListBox pivotGroupingItemsControl, object currItem, out int currentItemIndex)
        {
            if (pivotGroupingItemsControl != null)
            {
                currentItemIndex = ((IList)pivotGroupingItemsControl.ItemsSource).IndexOf(currItem);
                return ((IList)pivotGroupingItemsControl.ItemsSource).IndexOf(item.Tag);
            }
            else
                currentItemIndex = -3;

            return -1;
        }

        private int m_ItemIndex = -2;
        internal int ItemIndex
        {
            get
            {
                return m_ItemIndex;
            }
            set
            {
                m_ItemIndex = value;
            }
        }

        private void DragAndDropManager_Drop_SchemaDesigner(object sender, DragDropEventArgs args)
        {
            if (this.Indicator == null) return;
            if (this.Indicator.IsOpen)
                this.Indicator.IsOpen = false;
            PivotGridControl grid = this.PivotControl as PivotGridControl;
            ListBox dropTarget = null;
            ListBoxItem newitem = args.DropTarget as ListBoxItem;
            if (args.PayLoad == null && newitem != null)
                args.PayLoad = newitem.Content;
            if (args.DropTarget is ListBoxItem)
            {
                DependencyObject depObject = (DependencyObject)args.OriginalSource;
                while (depObject != null)
                {
                    dropTarget = depObject as ListBox;
                    if (dropTarget != null)
                    {
                        break;
                    }
                    depObject = VisualTreeHelper.GetParent(depObject);
                }
            }
            else
                dropTarget = (ListBox)args.DropTarget;
            if (dropTarget == null) return;
            if (dropTarget is PivotGroupingItemsControl) return;
            if (dropTarget.Name == "PART_FilterList") { FilterDrop(sender, args); return; }

            if (dropTarget == this.PivotColumnList && ((IList)dropTarget.ItemsSource).Count == 0)
                ((IList)dropTarget.ItemsSource).Clear();
            if (args.PayLoad == null) return;
            if (args.PayLoad.GetType().Equals(typeof(PivotTableField)))
            {
                PivotTableField sourceProperties = (PivotTableField)args.PayLoad;
                if (!sourceProperties.AllowRunTimeGroupByField)
                {
                    if (grid.GroupingBar != null)
                        grid.GroupingBar.shouldShowBouncingArrows = true;
                    return;
                }
                if (dropTarget == this.PivotCalculationList)
                {
                    this.AddItemToValues(sourceProperties);
                }
                else
                {
                    ObservableCollection<PivotItem> pivotItems = dropTarget.ItemsSource as ObservableCollection<PivotItem>;
                    if (pivotItems != null)
                    {
                        PivotItem item = pivotItems.Where(p => p.FieldMappingName == sourceProperties.FieldName).FirstOrDefault();
                        if (item == null)
                        {
                            this.PivotControl.PivotColumns.Remove(this.PivotControl.PivotColumns.Where(f => f.FieldMappingName == sourceProperties.FieldName).FirstOrDefault());
                            this.PivotControl.PivotRows.Remove(this.PivotControl.PivotRows.Where(f => f.FieldMappingName == sourceProperties.FieldName).FirstOrDefault());
                            this.PivotControl.Filters.Remove(this.PivotControl.Filters.Where(f => f.Name == sourceProperties.FieldName).FirstOrDefault());
                            if (ItemIndex >= 0)
                                ((IList)dropTarget.ItemsSource).Insert(ItemIndex, GetPivotItem(sourceProperties));
                            else
                                ((IList)dropTarget.ItemsSource).Add(GetPivotItem(sourceProperties));
                        }
                    }
                }
                this.PivotControl.PivotFields.Remove(this.PivotControl.PivotFields.Where(f => f.FieldMappingName == sourceProperties.FieldName).FirstOrDefault());
                this.UpdatePivotTableFieldItemSelection(sourceProperties.FieldName);
            }
            else if (args.PayLoad.GetType().Equals(typeof(PivotItem)))
            {
                object data = args.PayLoad;
                PivotItem item = (PivotItem)data;
                if (!item.AllowRunTimeGroupByField)
                {
                    if (grid.GroupingBar != null)
                        grid.GroupingBar.shouldShowBouncingArrows = true;
                    return;
                }
                if (!this.PivotControl.DeferLayoutUpdate)
                {
                    //// Suspending refresh
                    this.SuspendRefresh();
                    ((IList)dragSource.ItemsSource).Remove(data);

                    //// Resuming refresh
                    this.ResumeRefresh();
                }
                else
                {
                    ((IList)dragSource.ItemsSource).Remove(data);
                }

                if (dropTarget == this.PivotCalculationList)
                {
                    PivotItem pivotItem = (PivotItem)data;
                    if (this.PivotControl is PivotGridControl)
                        this.AddItemToValues(this.ItemIndex, pivotItem);
                    else
                        this.AddItemToValues(this.PivotControl.PivotCalculations.Count, pivotItem);
                }
                else if (dropTarget == this.PivotColumnList || dropTarget == this.PivotRowList)
                {
                    if (ItemIndex >= 0 && ((IList)dropTarget.ItemsSource).Count > ItemIndex)
                        ((IList)dropTarget.ItemsSource).Insert(ItemIndex, data);
                    else
                        ((IList)dropTarget.ItemsSource).Add(data);
                }
                else
                {
                    PivotItem pivotItem = (PivotItem)data;
                    if (this.PivotControl.PivotFields.Where(i => i.FieldMappingName == pivotItem.FieldMappingName).Count() == 0)
                    {
                        this.PivotControl.PivotFields.Add(pivotItem);
                        this.PivotControl.PivotEngine.RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
                    }
                }
                if ((this.PivotControl as PivotGridControl).GroupingBar != null)
                    (this.PivotControl as PivotGridControl).GroupingBar.AddEmptyItem(dragSource);
            }
            else if (args.PayLoad.GetType().Equals(typeof(PivotComputationInfo)))
            {
                object data = args.PayLoad;
                PivotComputationInfo compInfo = (PivotComputationInfo)data;
                if (!compInfo.AllowRunTimeGroupByField)
                {
                    if (grid.GroupingBar != null)
                        grid.GroupingBar.shouldShowBouncingArrows = true;
                    return;
                }
                ObservableCollection<PivotItem> pivotItems = dropTarget.ItemsSource as ObservableCollection<PivotItem>;
                PivotItem item = null;
                if (pivotItems != null)
                {
                    item = pivotItems.Where(p => p.FieldMappingName == compInfo.FieldName).FirstOrDefault();
                }
                if (item == null)
                {
                    if (this.PivotTableFields != null && this.PivotTableFields.Count > 0)
                        compInfo.Format = this.PivotTableFields.Where(i => i.FieldName == compInfo.FieldName).FirstOrDefault().Format;

                    if (!this.PivotControl.DeferLayoutUpdate)
                    {
                        this.SuspendRefresh();
                        ((IList)dragSource.ItemsSource).Remove(data);
                        this.ResumeRefresh();
                    }
                    else
                    {
                        ((IList)dragSource.ItemsSource).Remove(data);
                    }
                    if (dropTarget == this.PivotCalculationList)
                    {
                        if (this.ItemIndex >= 0 && this.ItemIndex < ((IList)dropTarget.ItemsSource).Count)
                            ((IList)dropTarget.ItemsSource).Insert(ItemIndex, compInfo);
                        else
                            ((IList)dropTarget.ItemsSource).Add(compInfo);
                    }
                    else
                    {
                        this.PivotControl.PivotColumns.Remove(this.PivotControl.PivotColumns.Where(f => f.FieldMappingName == compInfo.FieldName).FirstOrDefault());
                        this.PivotControl.PivotRows.Remove(this.PivotControl.PivotRows.Where(f => f.FieldMappingName == compInfo.FieldName).FirstOrDefault());
                        this.PivotControl.Filters.Remove(this.PivotControl.Filters.Where(f => f.Name == compInfo.FieldName).FirstOrDefault());
                        if (((IList)dropTarget.ItemsSource).GetType().Equals(typeof(ObservableCollection<PivotItem>)))
                        {
                            if (ItemIndex >= 0)
                                ((IList)dropTarget.ItemsSource).Insert(ItemIndex, new PivotItem { FieldHeader = compInfo.FieldHeader, FieldMappingName = compInfo.FieldName, Format = compInfo.Format, TotalHeader = "Total", AllowRunTimeGroupByField = compInfo.AllowRunTimeGroupByField });
                            else
                                ((IList)dropTarget.ItemsSource).Add(new PivotItem { FieldHeader = compInfo.FieldHeader, FieldMappingName = compInfo.FieldName, Format = compInfo.Format, TotalHeader = "Total", AllowRunTimeGroupByField = compInfo.AllowRunTimeGroupByField });
                        }
                        else
                        {
                            if (this.PivotControl.PivotFields.Where(i => i.FieldMappingName == compInfo.FieldName).Count() == 0)
                            {
                                this.PivotControl.PivotFields.Add(new PivotItem { FieldHeader = compInfo.FieldHeader, FieldMappingName = compInfo.FieldName, Format = compInfo.Format, TotalHeader = "Total", AllowRunTimeGroupByField = compInfo.AllowRunTimeGroupByField });
                                this.PivotControl.PivotEngine.RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
                            }
                        }
                    }
                }
            }
            else if (args.PayLoad.GetType().Equals(typeof(FilterItemsCollection)))
            {
                FilterItemsCollection filterItem = (FilterItemsCollection)args.PayLoad;


                //// Removing the filter item entry from the filters collection
                RemoveFilterItem(filterItem);
                //// Generating a PivotItem based on FilterItemsCollection
                PivotItem pivotItem = GetPivotItem(filterItem);
                pivotItem.ShowSubTotal = filterItem.ShowSubTotal;
                if (!pivotItem.AllowRunTimeGroupByField)
                {
                    if (grid.GroupingBar != null)
                        grid.GroupingBar.shouldShowBouncingArrows = true;
                    return;
                }

                if (!this.PivotControl.DeferLayoutUpdate)
                {
                    //// Suspending refresh
                    this.SuspendRefresh();
                    grid.GroupingBar.Filters.Remove(filterItem);

                    //// Resuming refresh
                    this.ResumeRefresh();
                }
                else if(grid.GroupingBar != null)
                {
                    grid.GroupingBar.Filters.Remove(filterItem);
                }

                if (dropTarget == this.PivotCalculationList)
                {
                    //// Adding the PivotItem to droped target.

                    if (this.PivotControl is PivotGridControl)
                        this.AddItemToValues(this.ItemIndex, pivotItem);
                    else
                        this.AddItemToValues(this.PivotControl.PivotCalculations.Count, pivotItem);
                }
                else if (dropTarget == this.PivotRowList || dropTarget == this.PivotColumnList)
                {
                    if (ItemIndex >= 0 && ItemIndex < ((IList)dropTarget.ItemsSource).Count)
                        ((IList)dropTarget.ItemsSource).Insert(ItemIndex,pivotItem);
                    else
                    ((IList)dropTarget.ItemsSource).Add(pivotItem);
                }
                else
                {
                    if (this.PivotControl.PivotFields.Where(i => i.FieldMappingName == pivotItem.FieldMappingName).Count() == 0)
                    {
                        this.PivotControl.PivotFields.Add(pivotItem);
                        this.PivotControl.PivotEngine.RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
                    }
                }
                //// Updating the selection status
                UpdatePivotTableFieldItemSelection(pivotItem.FieldMappingName);
                if (this.FilterList.Items.Count == 0 && (this.PivotControl as PivotGridControl).GroupingBar != null)
                {
                    (this.PivotControl as PivotGridControl).GroupingBar.ApplyEmptyTemplate();
                }
            }
            RegisterAllCommands();
        }


        /// <summary>
        /// Registers all commands.
        /// </summary>
        private void RegisterAllCommands()
        {
            this.UpdateLayout();
            RegisterCommands(this.PivotCalculationList);
            RegisterCommands(this.PivotColumnList);
            RegisterCommands(this.PivotRowList);
            RegisterFilterCommands(this.FilterList);
        }

        private void FilterDrop(object sender, DragDropEventArgs args)
        {
            PivotGridControl grid = this.PivotControl as PivotGridControl;
            if (args.PayLoad == null) return;
            if (args.PayLoad.GetType().Equals(typeof(PivotTableField)))
            {
                PivotTableField sourceField = (PivotTableField)args.PayLoad;
                if (!sourceField.AllowRunTimeGroupByField)
                {
                    if (grid.GroupingBar != null)
                        grid.GroupingBar.shouldShowBouncingArrows = true;
                    return;
                }
                if (this.Filters.Where(f => f.Name == sourceField.FieldName).FirstOrDefault() == null)
                {

                    this.PivotControl.PivotFields.Remove(this.PivotControl.PivotFields.Where(f => f.FieldMappingName == sourceField.FieldName).FirstOrDefault());
                    this.PivotControl.PivotRows.Remove(this.PivotControl.PivotRows.Where(f => f.FieldMappingName == sourceField.FieldName).FirstOrDefault());
                    FilterItemsCollection filterList = GetFilterItem(sourceField.FieldPropertyDescriptor);
                    filterList.DisplayHeader = GetPivotTableFieldHeader(filterList.Name);
                    filterList.Format = GetPivotTableFormat(filterList.Name, filterList.GetType());
                    if (this.ItemIndex >= 0)
                        this.Filters.Insert(ItemIndex, filterList);
                    else
                        this.Filters.Add(filterList);
                    this.UpdateFilters(filterList);
                    if ((this.PivotControl as PivotGridControl).GroupingBar != null)
                    {
                        (this.PivotControl as PivotGridControl).GroupingBar.Filters = this.Filters;
                        (this.PivotControl as PivotGridControl).GroupingBar.ApplyTemplateToGroupingControl((this.PivotControl as PivotGridControl).GroupingBar.FilterHeaderArea, null);
                    }
                }
                this.PivotControl.PivotColumns.Remove(this.PivotControl.PivotColumns.Where(f => f.FieldMappingName == sourceField.FieldName).FirstOrDefault());
                UpdatePivotTableFieldItemSelection(sourceField.FieldName);
            }
            else if (args.PayLoad.GetType().Equals(typeof(PivotItem)))
            {
                PivotItem pivotItem = (PivotItem)args.PayLoad;
                if (!pivotItem.AllowRunTimeGroupByField)
                {
                    if (grid.GroupingBar != null)
                        grid.GroupingBar.shouldShowBouncingArrows = true;
                    return;
                }
                if (this.Filters.Where(f => f.Name == pivotItem.FieldHeader).FirstOrDefault() == null)
                {
                    PropertyInfo descriptor = GetPropertyDescriptor(pivotItem.FieldMappingName);
                    if (descriptor != null)
                    {
                        if (!this.PivotControl.DeferLayoutUpdate)
                        {
                            //// Suspending refresh
                            this.SuspendRefresh();
                            ((IList)dragSource.ItemsSource).Remove(pivotItem);

                            //// Resuming refresh
                            this.ResumeRefresh();
                        }
                        else
                        {
                            ((IList)dragSource.ItemsSource).Remove(pivotItem);
                        }

                        FilterItemsCollection filterList = GetFilterItem(descriptor);
                        FilterExpression exp = null;
                        if (descriptor is PropertyInfo)
                        {
                            exp = grid.Filters.SingleOrDefault(x => x.Name == (descriptor as PropertyInfo).Name);
                            if (exp != null)
                                filterList = exp.Tag as FilterItemsCollection;
                            else
                                filterList = GetFilterItem(descriptor as PropertyInfo);
                        }                        filterList.DisplayHeader = pivotItem.FieldHeader;
                        filterList.Format = pivotItem.Format;
                        filterList.ShowSubTotal = pivotItem.ShowSubTotal;
                                          
                        if (this.ItemIndex >= 0)
                            this.Filters.Insert(ItemIndex, filterList);
                        else
                            this.Filters.Add(filterList);
                        this.UpdateFilters(filterList);
                        if ((this.PivotControl as PivotGridControl).GroupingBar != null)
                        {
                            (this.PivotControl as PivotGridControl).GroupingBar.Filters = this.Filters;
                            (this.PivotControl as PivotGridControl).GroupingBar.ApplyTemplateToGroupingControl((this.PivotControl as PivotGridControl).GroupingBar.FilterHeaderArea, null);
                        }
                    }
                }
                if ((this.PivotControl as PivotGridControl).GroupingBar != null)
                    (this.PivotControl as PivotGridControl).GroupingBar.AddEmptyItem(dragSource);
                UpdatePivotTableFieldItemSelection(pivotItem.FieldMappingName);
            }
            else if (args.PayLoad.GetType().Equals(typeof(PivotComputationInfo)))
            {
                PivotComputationInfo pivotComputationInfo = (PivotComputationInfo)args.PayLoad;
                if (!pivotComputationInfo.AllowRunTimeGroupByField)
                {
                    if (grid.GroupingBar != null)
                        grid.GroupingBar.shouldShowBouncingArrows = true;
                    return;
                }
                if (this.Filters.Where(f => f.Name == pivotComputationInfo.FieldName).FirstOrDefault() == null)
                {
                    PropertyInfo descriptor = GetPropertyDescriptor(pivotComputationInfo.FieldName);
                    if (descriptor != null)
                    {
                        this.PivotControl.PivotRows.Remove(this.PivotControl.PivotRows.Where(f => f.FieldMappingName == pivotComputationInfo.FieldName).FirstOrDefault());
                        this.PivotControl.PivotColumns.Remove(this.PivotControl.PivotColumns.Where(f => f.FieldMappingName == pivotComputationInfo.FieldName).FirstOrDefault());
                        ((IList)dragSource.ItemsSource).Remove(pivotComputationInfo);
                        FilterItemsCollection filterList = GetFilterItem(descriptor);
                        filterList.DisplayHeader = pivotComputationInfo.FieldHeader;
                        filterList.Format = pivotComputationInfo.Format;
                        if (this.ItemIndex >= 0)
                            this.Filters.Insert(ItemIndex, filterList);
                        else
                            this.Filters.Add(filterList);
                        this.UpdateFilters(filterList);
                        if ((this.PivotControl as PivotGridControl).GroupingBar != null)
                        {
                            (this.PivotControl as PivotGridControl).GroupingBar.Filters = this.Filters;
                            (this.PivotControl as PivotGridControl).GroupingBar.ApplyTemplateToGroupingControl((this.PivotControl as PivotGridControl).GroupingBar.FilterHeaderArea, null);
                        }
                    }
                }
                UpdatePivotTableFieldItemSelection(pivotComputationInfo.FieldName);
            }
            else if (args.PayLoad.GetType().Equals(typeof(FilterItemsCollection)))
            {
                FilterItemsCollection filterItem = (FilterItemsCollection)args.PayLoad;
                //// Removing the filter item entry from the filters collection
                RemoveFilterItem(filterItem);
                //// Generating a PivotItem based on FilterItemsCollection
                if (!this.PivotControl.DeferLayoutUpdate)
                {
                    //// Suspending refresh
                    this.SuspendRefresh();
                    grid.GroupingBar.Filters.Remove(filterItem);

                    //// Resuming refresh
                    this.ResumeRefresh();
                }
                else if (grid.GroupingBar != null)
                {
                    grid.GroupingBar.Filters.Remove(filterItem);
                }
                if (this.ItemIndex >= 0)
                    this.Filters.Insert(ItemIndex, filterItem);
                else
                    this.Filters.Add(filterItem);
                this.UpdateFilters(filterItem);
                if ((this.PivotControl as PivotGridControl).GroupingBar != null)
                {
                    (this.PivotControl as PivotGridControl).GroupingBar.Filters = this.Filters;
                    (this.PivotControl as PivotGridControl).GroupingBar.ApplyTemplateToGroupingControl((this.PivotControl as PivotGridControl).GroupingBar.FilterHeaderArea, null);
                }
                UpdatePivotTableFieldItemSelection(filterItem.Name);
            }
            RegisterAllCommands();
        }


        private void UpdateFilters(FilterItemsCollection filterList)
        {
            if (this.PivotControl != null)
            {
                filterList.AcceptChanges();
                FilterExpression filterExpression = this.PivotControl.Filters.Where(x => x.Name == filterList.Name).FirstOrDefault();
                if (filterExpression == null)
                {
                    if (PivotControl.ItemSource is IEnumerable)
                    {
                        this.PivotControl.Filters.Add(new FilterExpression { Name = filterList.Name, Expression = filterList.GetFilterExpression(true), DimensionHeader=filterList.Name, Tag = filterList });
                    }
                }
                else
                {
                    if (PivotControl.ItemSource is IEnumerable)
                    {
                        filterExpression.Expression = filterList.GetFilterExpression(true);
                    }
                    this.PivotControl.PivotEngine.RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
                }
            }
        }

        /// <summary>
        /// Initializes the pivot table list.
        /// </summary>
        public void InitilizePivotTableList()
        {
            if (this.PivotTableFieldList != null && this.PivotControl != null)
            {
                if (this.PivotTableFields.Count > 0)
                    this.PivotTableFields.Clear();

                PropertyInfo[] propertyinfoCollection = null;
                object firstItem = null;
                if (this.PivotControl.PivotEngine != null && this.PivotControl.PivotEngine.ItemProperties.Count > 0
                    && this.PivotControl.PivotEngine.ItemProperties.Values.Count > 0)
                {
                    propertyinfoCollection = new PropertyInfo[this.PivotControl.PivotEngine.ItemProperties.Values.Count];
                    int entryCount = 0;
                    foreach (DictionaryEntry entry in this.PivotControl.PivotEngine.ItemProperties)
                    {
                        propertyinfoCollection[entryCount++] = entry.Value as PropertyInfo;
                    }
                }
                else
                {
                    IEnumerable source = null;
                    Type Itemtype = null;
                    source = this.PivotControl.ItemSource as IEnumerable;
                    if (source != null)
                    {
                        if ((source as IList).Count == 0)
                        {
                            Itemtype = source.GetType().GetProperties()[1].PropertyType;
                            if (Itemtype != null)
                            {
                                propertyinfoCollection = Itemtype.GetProperties();
                            }
                        }
                        else
                        {
                            foreach (object item in source)
                            {
                                firstItem = item;
                                propertyinfoCollection = item.GetType().GetProperties();
                                break;
                            }
                        }
                        if (firstItem != null)
                        {
                            propertyinfoCollection[0] = firstItem.GetType().GetProperty(firstItem.ToString());
                        }
                    }
                }
                PivotTableField sp;
                if (ShowDisplayFieldsOnly)
                {
                    foreach (var item in this.PivotControl.PivotCalculations)
                    {
                        this.PivotTableFields.Add(sp = new PivotTableField
                            {
                                FieldHeader = item.FieldHeader,
                                Format = item.Format,
                                FieldPropertyDescriptor = item.GetType().GetProperty(item.FieldName),
                                IsSelected = GetPivotTableFieldIsSelected(item.FieldName),
                                SummaryType = item.SummaryType,
                                TotalHeader = GetPivotTableFieldTotalHeader(item.FieldName),
                                AllowRunTimeGroupByField = item.AllowRunTimeGroupByField
                            });

                        InvokePivotTableFieldSettings(sp);
                    }

                    foreach (var item in this.PivotControl.PivotColumns)
                    {
                        sp = AddPivotItemToTableFields(propertyinfoCollection, item);
                        InvokePivotTableFieldSettings(sp);
                    }

                    foreach (var item in this.PivotControl.PivotRows)
                    {
                        sp = AddPivotItemToTableFields(propertyinfoCollection, item);
                        InvokePivotTableFieldSettings(sp);
                    }

                    foreach (var item in this.PivotControl.PivotFields)
                    {
                        sp = AddPivotItemToTableFields(propertyinfoCollection, item);
                        InvokePivotTableFieldSettings(sp);
                    }
                }
                else
                {
                    if (propertyinfoCollection != null)
                    {
                        foreach (PropertyInfo pi in propertyinfoCollection)
                        {
                            var pivotComputationInfo =
                                this.PivotControl.PivotCalculations.FirstOrDefault(p => p.FieldName == pi.Name);
                            this.PivotTableFields.Add(sp = new PivotTableField
                                {
                                    FieldHeader = GetPivotTableFieldHeader(pi.Name),
                                    Format = GetPivotTableFormat(pi.Name, pi.PropertyType),
                                    FieldPropertyDescriptor = pi,
                                    IsSelected = GetPivotTableFieldIsSelected(pi.Name),
                                    SummaryType =
                                        pivotComputationInfo == null
                                            ? GetPivotTableSummaryType(pi.PropertyType)
                                            : pivotComputationInfo.SummaryType,
                                    TotalHeader = GetPivotTableFieldTotalHeader(pi.Name),
                                    AllowRunTimeGroupByField = GetPivotTableAllowGrouping(pi.Name)
                                });

                            InvokePivotTableFieldSettings(sp);
                        }
                    }
                }
            }
             if (this.PivotControl is PivotGridControl && this.PivotControl.Filters.Count > 0 && this.Filters.Count == 0)
            {
                for (int i = 0; i < this.PivotControl.Filters.Count; i++)
                {
                    if (this.PivotControl.Filters[i].DimensionName != null)
                    {
                        this.PivotControl.PivotEngine.AddFilter(this.PivotControl.Filters[i]);
                        this.Filters.Insert(i, this.PivotControl.PivotEngine.ItemCollection);
                        if (this.Filters[i].DisplayHeader == null)
                            this.Filters[i].DisplayHeader = this.PivotControl.Filters[i].DimensionName;
                        UpdatePivotTableFieldItemSelection(this.PivotControl.Filters[i].DimensionName);
                        PivotGridControl.FilterIndex++;
                    }
                }
            }  
        }
#endif

#if !SILVERLIGHT
         private PivotTableField AddPivotItemToTableFields(PropertyDescriptorCollection propertyDescriptorCollection, PivotItem item)
#else
        private PivotTableField AddPivotItemToTableFields(PropertyInfo[] propertyDescriptorCollection, PivotItem item)
#endif
        {
            PivotTableField sp;
            this.PivotTableFields.Add(sp = new PivotTableField
            {
                FieldHeader = GetPivotTableFieldHeader(item.FieldMappingName),
#if !SILVERLIGHT
                Format = GetPivotTableFormat(item.FieldMappingName, propertyDescriptorCollection[item.FieldMappingName].PropertyType),
                FieldPropertyDescriptor = propertyDescriptorCollection[item.FieldMappingName],
                SummaryType = GetPivotTableSummaryType(propertyDescriptorCollection[item.FieldMappingName].PropertyType),
#else
                Format = item.Format,
                FieldPropertyDescriptor = item.GetType().GetProperty(item.FieldMappingName),
                SummaryType = GetPivotTableSummaryType(item.GetType()),
#endif
                IsSelected = GetPivotTableFieldIsSelected(item.FieldMappingName),
                TotalHeader = GetPivotTableFieldTotalHeader(item.FieldMappingName),
                AllowRunTimeGroupByField = item.AllowRunTimeGroupByField
            });
            return sp;
        }
        
        private void InvokePivotTableFieldSettings(PivotTableField sp)
        {
            sp.PropertyChanged += (pSender, pargs) =>
            {
                if (pargs.PropertyName.Equals("IsSelected"))
                {
                    if (!resumeProcessing)
                    {
                        PivotTableField pivotTableField = (PivotTableField)pSender;
                        
#if !SILVERLIGHT
                        if ((this.PivotControl as PivotGridControl).RowPivotsOnly == false)
                        {
#endif
                            if (pivotTableField.IsSelected)
                            {

#if SILVERLIGHT
                            object propInfo = GetPropertyDescriptor(pivotTableField.FieldName);
                            var dataType = (propInfo as PropertyInfo).PropertyType;
                            if(dataType == typeof(int) ||
                                dataType == typeof(float) ||
                                dataType == typeof(double) ||
                                dataType == typeof(decimal) ||
                                dataType == typeof(short) ||
                                dataType == typeof(long))
#else
                                if (pivotTableField.DataType == typeof(int) ||
                                    pivotTableField.DataType == typeof(float) ||
                                    pivotTableField.DataType == typeof(double) ||
                                    pivotTableField.DataType == typeof(decimal) ||
                                    pivotTableField.DataType == typeof(short) ||
                                    pivotTableField.DataType == typeof(long))
#endif
                                {
                                    if (this.PivotTableFields != null)
                                    {
                                        PivotTableField _field = this.PivotTableFields.Where(i => i.FieldName == pivotTableField.FieldName).FirstOrDefault();
                                        this.PivotControl.PivotCalculations.Add(new PivotComputationInfo { FieldHeader = _field.FieldHeader, FieldName = _field.FieldName, Format = _field.Format, SummaryType = _field.SummaryType, AllowRunTimeGroupByField = _field.AllowRunTimeGroupByField });
                                    }
                                    else
                                    {
                                        this.PivotControl.PivotCalculations.Add(new PivotComputationInfo { FieldHeader = pivotTableField.FieldHeader, FieldName = pivotTableField.FieldName, SummaryType = SummaryType.DoubleTotalSum, AllowRunTimeGroupByField = pivotTableField.AllowRunTimeGroupByField });
                                    }
                                }
                                else
                                    this.PivotControl.PivotRows.Add(GetPivotItem(pivotTableField));

                                PivotItem pivotitem = this.PivotControl.PivotFields.Where(p => p.FieldMappingName == pivotTableField.FieldName).FirstOrDefault();
                                if (pivotitem != null)
                                {
                                    this.PivotControl.PivotFields.Remove(pivotitem);
                                }
                            }
                            else
                            {
                                RemoveItem(pivotTableField);

                                PivotItem pivotitem = this.PivotControl.PivotFields.Where(p => p.FieldMappingName == pivotTableField.FieldName).FirstOrDefault();
                                if (pivotitem == null)
                                {
                                    this.PivotControl.PivotFields.Add(GetPivotItem(pivotTableField));
                                }
                            }
#if !SILVERLIGHT
                        }
                        else
                        {
                            if (pivotTableField.IsSelected)
                            {
                                (this.PivotControl as PivotGridControl).InternalGrid.SetValueColumnVisibility(pivotTableField.FieldName, false);
                                if (!(this.PossiblePivotCalculations.Any(s => s.FieldName == pivotTableField.FieldName)))
                                {
                                    this.PossiblePivotCalculations.Add(new PivotComputationInfo { FieldName = pivotTableField.FieldName, FieldHeader = pivotTableField.FieldHeader, SummaryType = pivotTableField.SummaryType, Summary = pivotTableField.Summary, Format = pivotTableField.Format });
                                }
                                // (this.PivotControl as PivotGridControl).LocalPossibleCalculations.Add(new PivotValueField { IsSelected = true, FieldName = pivotTableField.FieldName, FieldHeader = pivotTableField.FieldHeader, AllowRunTimeGroupByField = pivotTableField.AllowRunTimeGroupByField, Summary = pivotTableField.Summary, SummaryType = pivotTableField.SummaryType });
                            }
                            else
                            {
                                int value = this.PossiblePivotCalculations.Where(k => k.FieldName == pivotTableField.FieldName).Count();
                                for (int i = 0; i < value; i++)
                                {
                                    PivotComputationInfo calcInfo = this.PossiblePivotCalculations.Where(j => j.FieldName == pivotTableField.FieldName).FirstOrDefault();
                                    if (calcInfo != null)
                                    {
                                        (this.PivotControl as PivotGridControl).InternalGrid.SetValueColumnVisibility(calcInfo.FieldName, true);
                                        this.PossiblePivotCalculations.Remove(calcInfo);
                                    }
                                }
                            }
                        }
#endif
#if SILVERLIGHT
                        if ((this.PivotControl as PivotGridControl).GroupingBar != null)
                        {
                            (this.PivotControl as PivotGridControl).GroupingBar.ApplyEmptyTemplate();
                        }
#endif
                    }
                }
            };
        }

        /// <summary>
        /// Checks is PivotTableField exist in any of the PivotInfo collection(FilterList,
        /// ColumnPivot, RowPivot or in Calculation
        /// </summary>
        /// <param name="fieldName">ItemSource fieldname</param>
        /// <returns>bool</returns>
        private bool GetPivotTableFieldIsSelected(string fieldName)
        {
            bool isSelected = false;
#if !SILVERLIGHT
            if ((this.PivotControl as PivotGridControl).RowPivotsOnly == false)
            {
#endif
                if (!isSelected)
                    isSelected = this.PivotControl.PivotColumns.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() != null;
                if (!isSelected)
                    isSelected = this.PivotControl.PivotRows.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() != null;
                if (!isSelected)
                    isSelected = this.PivotControl.PivotCalculations.Where(p => p.FieldName == fieldName).FirstOrDefault() != null;
                if (!isSelected)
                    isSelected = this.Filters.Where(f => f.Name == fieldName).FirstOrDefault() != null;
                if (!isSelected)
                    isSelected = (this.PivotControl as PivotGridControl).FilterItems.Where(f => f.Name == fieldName).FirstOrDefault() != null;
#if !SILVERLIGHT
            }
            else
            {
                PivotValueField p = (this.PivotControl as PivotGridControl).LocalPossibleCalculations.Where(pi => pi.FieldName == fieldName).FirstOrDefault();
                isSelected = p.IsSelected;
            }
#endif
            return isSelected;
        }

        /// <summary>
        /// Checks for the FieldHeader name
        /// </summary>
        /// <param name="fieldName">ItemSource fieldname</param>
        /// <returns>string</returns>
        private string GetPivotTableFieldHeader(string fieldName)
        {
            if (this.PivotControl.PivotColumns.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() != null)
                return (this.PivotControl.PivotColumns.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() as PivotItem).FieldHeader;
            if (this.PivotControl.PivotRows.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() != null)
                return (this.PivotControl.PivotRows.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() as PivotItem).FieldHeader;
            if (this.PivotControl.PivotCalculations.Where(p => p.FieldName == fieldName).FirstOrDefault() != null)
                return (this.PivotControl.PivotCalculations.Where(p => p.FieldName == fieldName).FirstOrDefault() as PivotComputationInfo).FieldHeader;
            if (this.PivotControl.PivotFields.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() != null)
                return (this.PivotControl.PivotFields.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() as PivotItem).FieldHeader;
            if (this.PivotTableFields.Where(p => p.FieldName == fieldName).FirstOrDefault() != null)
                return (this.PivotTableFields.Where(p => p.FieldName == fieldName).FirstOrDefault() as PivotTableField).FieldHeader;
            return fieldName;
        }

        /// <summary>
        /// Checks for the FieldHeader name
        /// </summary>
        /// <param name="fieldName">ItemSource fieldname</param>
        /// <returns>string</returns>
        private string GetPivotTableFieldTotalHeader(string fieldName)
        {
            if (this.PivotControl.PivotColumns.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() != null)
                return (this.PivotControl.PivotColumns.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() as PivotItem).TotalHeader;
            if (this.PivotControl.PivotRows.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() != null)
                return (this.PivotControl.PivotRows.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() as PivotItem).TotalHeader;
            if (this.PivotControl.PivotCalculations.Where(p => p.FieldName == fieldName).FirstOrDefault() != null)
                return PivotGridConstants.TotalString;
            if (this.PivotControl.PivotFields.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() != null)
                return (this.PivotControl.PivotFields.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() as PivotItem).TotalHeader;
            if (this.PivotTableFields.Where(p => p.FieldName == fieldName).FirstOrDefault() != null)
                return (this.PivotTableFields.Where(p => p.FieldName == fieldName).FirstOrDefault() as PivotTableField).TotalHeader;
            return fieldName;
        }

        private string GetPivotTableFormat(string fieldName, Type fieldType)
        {
            if (this.PivotControl.PivotColumns.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() != null)
                return (this.PivotControl.PivotColumns.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() as PivotItem).Format;
            if (this.PivotControl.PivotRows.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() != null)
                return (this.PivotControl.PivotRows.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() as PivotItem).Format;
            if (this.PivotControl.PivotCalculations.Where(p => p.FieldName == fieldName).FirstOrDefault() != null)
                return (this.PivotControl.PivotCalculations.Where(p => p.FieldName == fieldName).FirstOrDefault() as PivotComputationInfo).Format;
            if (this.PivotControl.PivotFields.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() != null)
                return (this.PivotControl.PivotFields.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() as PivotItem).Format;
            if (this.PivotTableFields.Where(p => p.FieldName == fieldName).FirstOrDefault() != null)
                return (this.PivotTableFields.Where(p => p.FieldName == fieldName).FirstOrDefault() as PivotTableField).Format;
            if (fieldType == typeof(int) || fieldType == typeof(float) || fieldType == typeof(double) || fieldType == typeof(decimal))
                return "#.##"; // default format for computational info objects.
            else
                return null; //default format for pivot item objects
        }

        private bool GetPivotTableAllowGrouping(string fieldName)
        {
            if (this.PivotControl.PivotColumns.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() != null)
                return (this.PivotControl.PivotColumns.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() as PivotItem).AllowRunTimeGroupByField;
            if (this.PivotControl.PivotRows.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() != null)
                return (this.PivotControl.PivotRows.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() as PivotItem).AllowRunTimeGroupByField;
            if (this.PivotControl.PivotCalculations.Where(p => p.FieldName == fieldName).FirstOrDefault() != null)
                return (this.PivotControl.PivotCalculations.Where(p => p.FieldName == fieldName).FirstOrDefault() as PivotComputationInfo).AllowRunTimeGroupByField;
            if (this.PivotControl.PivotFields.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() != null)
                return (this.PivotControl.PivotFields.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() as PivotItem).AllowRunTimeGroupByField;
            if (this.PivotTableFields.Where(p => p.FieldName == fieldName).FirstOrDefault() != null)
                return (this.PivotTableFields.Where(p => p.FieldName == fieldName).FirstOrDefault() as PivotTableField).AllowRunTimeGroupByField;
            return true;
        }

        private SummaryType GetPivotTableSummaryType(Type type)
        {
            if (type == typeof(int))
            {
                return SummaryType.IntTotalSum;
            }
            if (type == typeof(decimal))
            {
                return SummaryType.DecimalTotalSum;
            }
            if (type == typeof(double) || type == typeof(float))
            {
                return SummaryType.DoubleTotalSum;
            }
            return SummaryType.Count;
        }

        /// <summary>
        /// Generates a PivotItem based on supplies PivotTableField 
        /// </summary>
        /// <param name="pivotTableField">PivotItemField</param>
        /// <returns>Pivotitem</returns>
        private PivotItem GetPivotItem(PivotTableField pivotTableField)
        {
            return new PivotItem { FieldHeader = pivotTableField.FieldHeader, FieldMappingName = pivotTableField.FieldName, TotalHeader = pivotTableField.TotalHeader, Format = pivotTableField.Format, AllowRunTimeGroupByField = pivotTableField.AllowRunTimeGroupByField };
        }

        /// <summary>
        /// Generates a PivotItem based on the supplied FilterItemsCollection
        /// </summary>
        /// <param name="filterItem"></param>
        /// <returns>Pivotitem</returns>
        private PivotItem GetPivotItem(FilterItemsCollection filterItem)
        {
            return GetPivotItem(filterItem.DisplayHeader, filterItem.Name, filterItem.Format, filterItem.AllowRunTimeGroupByField);
        }

        /// <summary>
        /// Generates a PivotItem based on given FieldName
        /// </summary>
        /// <param name="fieldHeaderName">Header text of field</param>
        /// <param name="fieldName">Mapping name of field</param>
        /// <param name="allowRunTimeGroupByField">To enable or disable group by field at runtime</param>
        /// <returns>Pivot Item</returns>
        private PivotItem GetPivotItem(string fieldHeaderName, string fieldName, bool allowRunTimeGroupByField)
        {
            return new PivotItem { FieldHeader = fieldHeaderName, FieldMappingName = fieldName, TotalHeader = PivotGridConstants.TotalString, AllowRunTimeGroupByField = allowRunTimeGroupByField };
        }

        private PivotItem GetPivotItem(string fieldHeaderName, string fieldName, bool allowRunTimeGroupByField, string format)
        {
            return new PivotItem { FieldHeader = fieldHeaderName, FieldMappingName = fieldName, TotalHeader = PivotGridConstants.TotalString, AllowRunTimeGroupByField = allowRunTimeGroupByField, Format = format };
        }

        private PivotItem GetPivotItem(string fieldHeaderName, string fieldName, string formatString, bool allowRunTimeGroupByField)
        {
            return new PivotItem { FieldHeader = fieldHeaderName, FieldMappingName = fieldName, Format = formatString, TotalHeader = PivotGridConstants.TotalString, AllowRunTimeGroupByField = allowRunTimeGroupByField };
        }

        /// <summary>
        /// Removed the filter from current filter collection and from the PivotControl
        /// </summary>
        /// <param name="filterItem">FilterItemsCollection</param>
        private void RemoveFilterItem(FilterItemsCollection filterItem)
        {
            FilterExpression filterExpression = null, filterExpressionExp = null;
            if (filterItem != null)
               this.Filters.Remove(filterItem);
            filterExpression = this.PivotControl.Filters.Where(f => f.DimensionName == filterItem.Name).FirstOrDefault();
            if (filterExpression == null)
                filterExpressionExp = this.PivotControl.Filters.Where(f => f.Name == filterItem.Name).FirstOrDefault();
            if (filterExpression != null)
                this.PivotControl.Filters.Remove(this.PivotControl.Filters.Where(i=>i.DimensionName == filterExpression.DimensionName).FirstOrDefault());
            // this.PivotControl.Filters.Remove(filterExpression);
            else if (filterExpressionExp != null)
                this.PivotControl.Filters.Remove(filterExpressionExp);
            //// Updating the PivotTableField selection
            UpdatePivotTableFieldItemSelection(filterItem.Name);
        }
        
        /// <summary>
        /// Update the PivotTableField selection status
        /// </summary>
        /// <param name="fieldName"></param>
        private void UpdatePivotTableFieldItemSelection(string fieldName)
        {
            if (this.PivotTableFieldList == null) return;
            ObservableCollection<PivotTableField> sourceFields = this.PivotTableFieldList.ItemsSource as ObservableCollection<PivotTableField>;
            if (sourceFields != null)
            {
                var field = sourceFields.Where(i => i.FieldName == fieldName).FirstOrDefault();
                if (field != null)
                {
                    resumeProcessing = true;
                    field.IsSelected = GetPivotTableFieldIsSelected(fieldName);
                    resumeProcessing = false;
                }
            }
        }

        /// <summary>
        /// Removes item PivotInfo based on the PivotTable 
        /// </summary>
        /// <param name="pivotTableField"></param>
        private void RemoveItem(PivotTableField pivotTableField)
        {
            //// Remeving item if exist in PivotRows
            PivotItem pivotItem = this.PivotControl.PivotRows.Where(i => i.FieldMappingName == pivotTableField.FieldName).FirstOrDefault();
            if (pivotItem != null)
                this.PivotControl.PivotRows.Remove(pivotItem);

            //// Removing item if exist in PivotColumns
            pivotItem = this.PivotControl.PivotColumns.Where(i => i.FieldMappingName == pivotTableField.FieldName).FirstOrDefault();
            if (pivotItem != null)
                this.PivotControl.PivotColumns.Remove(pivotItem);

            //// Removing item if exist in PivotValues
            int value;
#if !SILVERLIGHT
            if ((this.PivotControl as PivotGridControl).RowPivotsOnly == false)
            {
#endif
                value = this.PivotControl.PivotCalculations.Where(k => k.FieldName == pivotTableField.FieldName).Count();
                for (int i = 0; i < value; i++)
                {
                    PivotComputationInfo calcInfo = this.PivotControl.PivotCalculations.Where(j => j.FieldName == pivotTableField.FieldName).FirstOrDefault();
                    if (calcInfo != null)
                        this.PivotControl.PivotCalculations.Remove(calcInfo);
                }
#if !SILVERLIGHT
            }
            else
            {
                value = (this.PivotControl as PivotGridControl).PossiblePivotCalculations.Where(k => k.FieldName == pivotTableField.FieldName).Count();
                for (int i = 0; i < value; i++)
                {
                    PivotComputationInfo calcInfo = (this.PivotControl as PivotGridControl).PossiblePivotCalculations.Where(j => j.FieldName == pivotTableField.FieldName).FirstOrDefault();
                    if (calcInfo != null)
                        (this.PivotControl as PivotGridControl).InternalGrid.SetValueColumnVisibility(calcInfo.FieldName, true);
                }
            }
#endif

            //// Removing item if exist in Filters
            FilterItemsCollection filterItem = this.Filters.Where(f => f.Name == pivotTableField.FieldName).FirstOrDefault();
            if (filterItem != null)
            {
                this.Filters.Remove(filterItem);
                PivotGridControl.FilterIndex--;
                if (this.PivotControl is PivotGridControl && (this.PivotControl as PivotGridControl).GroupingBar != null)
                {
                    (this.PivotControl as PivotGridControl).GroupingBar.Filters.Remove(filterItem);
                    if ((this.PivotControl as PivotGridControl).GroupingBar.Filters.Count == 0)
                    {
                        (this.PivotControl as PivotGridControl).GroupingBar.ApplyEmptyTemplate(); 
                    }
                }
            }
            FilterExpression filterExpression = this.PivotControl.Filters.Where(f => f.Name == pivotTableField.FieldName).FirstOrDefault();
            if (filterExpression != null)
            {
                this.PivotControl.Filters.Remove(filterExpression);
            }
        }

        /// <summary>
        /// Wiring the control events
        /// </summary>
        private void WireEvents()
        {
#if !SILVERLIGHT
            ////PivotTableFieldList listbox events
            if (this.PivotTableFieldList != null)
            {

                this.PivotTableFieldList.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(PreviewMouseLeftButtonDown);
                this.PivotTableFieldList.PreviewMouseMove += new MouseEventHandler(PreviewMouseMove);
                this.PivotTableFieldList.PreviewDrop += (sender, e) =>
                    {
                        object data = null;
                        if (e.Data.GetDataPresent(typeof(FilterItemsCollection)))
                            data = e.Data.GetData(typeof(FilterItemsCollection));
                        else if (e.Data.GetDataPresent(typeof(PivotItem)))
                            data = e.Data.GetData(typeof(PivotItem));
                        else if (e.Data.GetDataPresent(typeof(PivotComputationInfo)))
                            data = e.Data.GetData(typeof(PivotComputationInfo));

                        if (data != null)
                            ((IList)dragSource.ItemsSource).Remove(data);
                    };
            }

            //// Row ListBox Events
            if (this.PivotRowList != null)
            {
                this.PivotRowList.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(PreviewMouseLeftButtonDown);
                this.PivotRowList.PreviewMouseMove += new MouseEventHandler(PreviewMouseMove);
                this.PivotRowList.PreviewDrop += new DragEventHandler(PreviewDrop);
            }

            //// Column ListBox Events
            if (this.PivotColumnList != null)
            {
                this.PivotColumnList.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(PreviewMouseLeftButtonDown);
                this.PivotColumnList.PreviewMouseMove += new MouseEventHandler(PreviewMouseMove);
                this.PivotColumnList.PreviewDrop += new DragEventHandler(PreviewDrop);
            }

            //// Value ListBox Events
            if (this.PivotCalculationList != null)
            {
                this.PivotCalculationList.PreviewMouseDoubleClick += new MouseButtonEventHandler(Calculations_PreviewMouseDoubleClick);
                this.PivotCalculationList.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(PreviewMouseLeftButtonDown);
                this.PivotCalculationList.PreviewMouseMove += new MouseEventHandler(PreviewMouseMove);
                this.PivotCalculationList.PreviewDrop += new DragEventHandler(PreviewDrop);
            }

            //// Filter ListBox Events
            if (this.FilterList != null)
            {
                this.FilterList.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(PreviewMouseLeftButtonDown);
                this.FilterList.PreviewMouseMove += new MouseEventHandler(PreviewMouseMove);
                this.FilterList.PreviewDrop += new DragEventHandler(ReportFilter_PreviewDrop);
            }
#else
            DragAndDropManager.Drag -= new DragDropEventHandler(DragAndDropManager_Drag_SchemaDesigner);
            DragAndDropManager.Drag += new DragDropEventHandler(DragAndDropManager_Drag_SchemaDesigner);
            DragAndDropManager.DragStarted -= new DragDropEventHandler(DragAndDropManager_DragStarted_SchemaDesigner);
            DragAndDropManager.DragStarted += new DragDropEventHandler(DragAndDropManager_DragStarted_SchemaDesigner);
            DragAndDropManager.Drop -= new DragDropEventHandler(DragAndDropManager_Drop_SchemaDesigner);
            DragAndDropManager.Drop += new DragDropEventHandler(DragAndDropManager_Drop_SchemaDesigner);
            DragAndDropManager.SetDropDescriptionVisibility(this, System.Windows.Visibility.Collapsed);
            Unloaded += (s, e) =>
            {
                DragAndDropManager.Drag -= new DragDropEventHandler(DragAndDropManager_Drag_SchemaDesigner);
                DragAndDropManager.DragStarted -= new DragDropEventHandler(DragAndDropManager_DragStarted_SchemaDesigner);
                DragAndDropManager.Drop -= new DragDropEventHandler(DragAndDropManager_Drop_SchemaDesigner);
            };
#endif
            if (this.DeferLayoutUpdateButton != null)
            {
                this.DeferLayoutUpdateButton.Click += (sender, args) =>
                    {
                        if (this.PivotControl != null)
                        {
                            this.PivotControl.PivotEngine.RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs { OverrideDeferLayoutUpdate = true });
                        }
                    };
            }
            if (this.PivotControl is PivotGridControl)
                (this.PivotControl as PivotGridControl).ItemSourceChanged += new ItemSourceChanged(PivotSchemaDesigner_ItemSourceChanged);
        }     
        /// <summary>
        /// Finds the visual child.
        /// </summary>
        /// <typeparam name="childItem">The type of the child item.</typeparam>
        /// <param name="obj">The obj.</param>
        /// <returns>child item</returns>
        public static childItem FindVisualChild<childItem>(DependencyObject obj)
            where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        void PivotSchemaDesigner_ItemSourceChanged(object sender, ItemsSourceChangedEventArgs e)
        { 
                InitilizePivotTableList();  
        }

        /// <summary>
        /// Creates a pivot calculation and add's to the calculation list
        /// </summary>
        /// <param name="pivotTableField"></param>
        private void AddItemToValues(PivotTableField pivotTableField)
        {
            if (pivotTableField.DataType == typeof(int) ||
                pivotTableField.DataType == typeof(float) ||
                pivotTableField.DataType == typeof(double) ||
                pivotTableField.DataType == typeof(decimal))
            this.PivotControl.PivotCalculations.Add(new PivotComputationInfo { FieldHeader = pivotTableField.FieldHeader, FieldName = pivotTableField.FieldName, Format = pivotTableField.Format, SummaryType = GetPivotTableSummaryType(pivotTableField.DataType) });
#if !SILVERLIGHT
            else if (pivotTableField.SummaryType==SummaryType.Custom)
                this.PivotControl.PivotCalculations.Add(new PivotComputationInfo { FieldHeader = pivotTableField.FieldHeader, FieldName = pivotTableField.FieldName, Format = pivotTableField.Format, SummaryType =SummaryType.Custom, Summary=pivotTableField.Summary });
#endif
            else
                this.PivotControl.PivotCalculations.Add(new PivotComputationInfo { FieldHeader = pivotTableField.FieldHeader, FieldName = pivotTableField.FieldName, Format = pivotTableField.Format, SummaryType = SummaryType.Count });
        }

        /// <summary>
        /// Creates a pivot calculation and add's to the calculation list
        /// </summary>
        /// <param name="pivotItem"></param>
        private void AddItemToValues(int index, PivotItem pivotItem)
        {
            PivotComputationInfo compInfo = new PivotComputationInfo();
            compInfo.FieldName = pivotItem.FieldMappingName;
            compInfo.FieldHeader = pivotItem.FieldHeader;
            compInfo.Format = pivotItem.Format;
            compInfo.AllowRunTimeGroupByField = pivotItem.AllowRunTimeGroupByField;
#if !SILVERLIGHT
            PropertyDescriptor propDescriptor =  GetPropertyDescriptor(pivotItem.FieldMappingName);
#else
            PropertyInfo propDescriptor =   GetPropertyDescriptor(pivotItem.FieldMappingName);
#endif
            if (propDescriptor != null)
            {
                if (propDescriptor.PropertyType == typeof(int) ||
                    propDescriptor.PropertyType == typeof(double) ||
                    propDescriptor.PropertyType == typeof(float) ||
                    propDescriptor.PropertyType == typeof(decimal) ||
                    propDescriptor.PropertyType == typeof(short) ||
                    propDescriptor.PropertyType == typeof(long))
                {
                    if (this.PivotTableFields != null)
                    {
                        PivotTableField _field = this.PivotTableFields.Where(i => i.FieldName == pivotItem.FieldMappingName).FirstOrDefault();
                        if (index >= 0)
                            this.PivotControl.PivotCalculations.Insert(index, new PivotComputationInfo { FieldHeader = _field.FieldHeader, FieldName = _field.FieldName, Format = _field.Format, SummaryType = _field.SummaryType, AllowRunTimeGroupByField = _field.AllowRunTimeGroupByField });

                        else
                            this.PivotControl.PivotCalculations.Add(new PivotComputationInfo { FieldHeader = _field.FieldHeader, FieldName = _field.FieldName, Format = _field.Format, SummaryType = _field.SummaryType, AllowRunTimeGroupByField = _field.AllowRunTimeGroupByField });
                    }
                    else
                    {
                        if (index >= 0)
                            this.PivotControl.PivotCalculations.Insert(index, new PivotComputationInfo { FieldHeader = pivotItem.FieldHeader, FieldName = pivotItem.FieldMappingName, SummaryType = SummaryType.DoubleTotalSum, AllowRunTimeGroupByField = pivotItem.AllowRunTimeGroupByField });
                        else
                            this.PivotControl.PivotCalculations.Add(new PivotComputationInfo { FieldHeader = pivotItem.FieldHeader, FieldName = pivotItem.FieldMappingName, SummaryType = SummaryType.DoubleTotalSum, AllowRunTimeGroupByField = pivotItem.AllowRunTimeGroupByField });
                    }
                }
                else
                {
                    var pivotField = this.PivotTableFields.FirstOrDefault(i => i.FieldName == pivotItem.FieldMappingName);
                    if (pivotField != null)
                    {
                        compInfo.Format = pivotField.Format;
#if !SILVERLIGHT
                        if (pivotField.Summary != null)
                            compInfo.Summary = pivotField.Summary;
#endif
                        if (pivotField.SummaryType != SummaryType.Count)
                            compInfo.SummaryType = pivotField.SummaryType;

                    }
                    else
                        compInfo.SummaryType = SummaryType.Count;

#if !SILVERLIGHT
                    if ((this.PivotControl as PivotGridControl).RowPivotsOnly == false)
                    {
#endif
                        if (index >= 0 && this.PivotControl.PivotCalculations.Count > index)
                            this.PivotControl.PivotCalculations.Insert(index, compInfo);
                        else
                            this.PivotControl.PivotCalculations.Add(compInfo);
#if !SILVERLIGHT
                    }
                    else
                    {
                        if (index >= 0 && (this.PivotControl as PivotGridControl).PossiblePivotCalculations.Count > index)
                        {
                            this.PossiblePivotCalculations.Insert(index, compInfo);
                            (this.PivotControl as PivotGridControl).InternalGrid.SetValueColumnVisibility(compInfo.FieldName, false);
                        }
                        else
                        {
                            this.PossiblePivotCalculations.Add(compInfo);
                            (this.PivotControl as PivotGridControl).InternalGrid.SetValueColumnVisibility(compInfo.FieldName, false);
                        }
                    }
#endif

                }
            }
        }

        /// <summary>
        /// Creates a filterItem based on PropertyDesciptor supplied
        /// </summary>
        /// <param name="propertyDescriptor"></param>
        /// <returns></returns>
#if SILVERLIGHT
        private FilterItemsCollection GetFilterItem(PropertyInfo propertyDescriptor)
#else
        private FilterItemsCollection GetFilterItem(PropertyDescriptor propertyDescriptor)
#endif
        {
            FilterItemsCollection filterList = new FilterItemsCollection();
            IEnumerable itemSource = null;
#if !SILVERLIGHT
            if (this.PivotControl.ItemSource is System.Data.DataTable)
                itemSource = (this.PivotControl.ItemSource as System.Data.DataTable).DefaultView;
            else
#endif
            itemSource = this.PivotControl.ItemSource as IEnumerable;
            foreach (var item in itemSource)
            {
                filterList.FilterProperty = propertyDescriptor;
                if (propertyDescriptor.PropertyType == typeof(int) ||
                    propertyDescriptor.PropertyType == typeof(double) ||
                    propertyDescriptor.PropertyType == typeof(float) ||
                    propertyDescriptor.PropertyType == typeof(decimal) ||
                    propertyDescriptor.PropertyType == typeof(short) ||
                    propertyDescriptor.PropertyType == typeof(long) ||
                    propertyDescriptor.PropertyType == typeof(DateTime))
#if !SILVERLIGHT
                   filterList.AddIfUnique(new FilterItemElement { Key = propertyDescriptor.GetValue(item).ToString() });
                else
                    filterList.AddIfUnique(new FilterItemElement { Key = propertyDescriptor.GetValue(item) as string });
#else
                     filterList.AddIfUnique(new FilterItemElement { Key = propertyDescriptor.GetValue(item, new object[] { }).ToString() });
                else
                    filterList.AddIfUnique(new FilterItemElement { Key = propertyDescriptor.GetValue(item, new object[] { }) as string });
#endif
            }
            filterList.Reverse();
            return filterList;
        }

        /// <summary>
        /// Gets the filter item from the current filter collection
        /// </summary>
        /// <param name="filterName"></param>
        /// <returns></returns>
        private FilterItemsCollection GetFilterItem(string filterName)
        {
            return this.Filters.Where(f => f.Name == filterName).FirstOrDefault();
        }

        /// <summary>
        /// Gets the property descriptor based on FieldName
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>

#if !SILVERLIGHT
        private PropertyDescriptor GetPropertyDescriptor(string fieldName)
#else
        private PropertyInfo GetPropertyDescriptor(string fieldName)
#endif
        {
            IEnumerable source = null;
#if !SILVERLIGHT
            if (this.PivotControl.ItemSource is System.Data.DataTable)
                source = (this.PivotControl.ItemSource as System.Data.DataTable).DefaultView;
            else
#endif
            source = this.PivotControl.ItemSource as IEnumerable;
            if (source != null)
            {
                object firstItem = null;
                foreach (object item in source)
                {
                    firstItem = item;
                    break;
                }
                if (firstItem != null)
                {
#if !SILVERLIGHT
                    var pdCollection = TypeDescriptor.GetProperties(firstItem);
                    return pdCollection.Cast<PropertyDescriptor>().Where(p => p.Name == fieldName).FirstOrDefault();
#else
                    var pdCollection = firstItem.GetType().GetProperties();
                    return pdCollection.Cast<PropertyInfo>().Where(p => p.Name == fieldName).FirstOrDefault();
#endif
                }
                else if (firstItem == null && fieldName != null)
                {
#if SILVERLIGHT
                    PropertyInfo[] propertyinfoCollection;
#endif
                    Type Itemtype = null;
                    Itemtype = source.GetType().GetProperties()[1].PropertyType;
                    if (Itemtype != null)
                    {
#if SILVERLIGHT
                        propertyinfoCollection = Itemtype.GetProperties();
                        foreach (var pdCollection in propertyinfoCollection)
                        {
                            if (pdCollection.Name == fieldName)
                            {
                                return pdCollection;
                            }
                        }
#endif

                    }

                }
            }
            
            return null;
        }

        /// <summary>
        /// Gets the list item based on co-ordinates
        /// </summary>
        /// <param name="source">Source list box</param>
        /// <param name="point">co-ordinates</param>
        /// <returns>object on co-ordinate</returns>
        internal static object GetDataFromListBox(ListBox source, Point point)
        {
            if (source != null)
            {
#if !SILVERLIGHT
                UIElement element = source.InputHitTest(point) as UIElement;
#else
                UIElement element = source;
#endif
                if (element != null)
                {
                    object data = DependencyProperty.UnsetValue;
                    while (data == DependencyProperty.UnsetValue)
                    {
                        data = source.ItemContainerGenerator.ItemFromContainer(element);
                        if (data == DependencyProperty.UnsetValue)
                        {
                            element = VisualTreeHelper.GetParent(element) as UIElement;
                        }
                        if (element == source)
                        {
                            return null;
                        }
                    }
                    if (data != DependencyProperty.UnsetValue)
                    {
                        return data;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Suspends the update in control
        /// </summary>
        void SuspendRefresh()
        {
            this.PivotControl.DeferLayoutUpdate = true;
        }

        /// <summary>
        /// Resuming the control update
        /// </summary>
        void ResumeRefresh()
        {
            this.PivotControl.DeferLayoutUpdate = false;
        }

#if !SILVERLIGHT
         /// <summary>
         /// Apply the visual style to pivotschemadesigner
         /// </summary>
         /// <param name="themeName"></param>
        void ApplyVisualStyle(string themeName)
        {
            ResourceDictionary themeResouce = new ResourceDictionary();
            SkinStorage.SetVisualStyle(this, themeName);
            ResourceDictionary resource = new ResourceDictionary
            {
                Source = new Uri("/Syncfusion.PivotAnalysis.WPF;component/PivotSchemaDesigner/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
            };
            this.Background = resource[themeName + ".PivotSchemaBackground"] as Brush;
            try
            {
                if (themeName.Contains("Office2010"))
                    themeResouce.Source = new Uri("/Syncfusion.PivotAnalysis.WPF;component/PivotSchemaDesigner/Themes/Office2010Style.xaml", UriKind.RelativeOrAbsolute);
                else if (themeName.ToString().Contains("Office2007"))
                    themeResouce.Source = new Uri("/Syncfusion.PivotAnalysis.WPF;component/PivotSchemaDesigner/Themes/Office2007Style.xaml", UriKind.RelativeOrAbsolute);
                else
                    themeResouce.Source = new Uri("/Syncfusion.PivotAnalysis.WPF;component/PivotSchemaDesigner/Themes/" + themeName + "Style.xaml", UriKind.RelativeOrAbsolute);
                //pivotSchemaDesigner.Resources = themeResouce;
                if (PivotCalculationList != null)
                {
                    this.PivotCalculationList.ItemTemplate = themeResouce["lstPivotCalculationTemplate"] as DataTemplate;
                    this.PivotColumnList.ItemTemplate = themeResouce["lstPivotItemTemplate"] as DataTemplate;
                    this.PivotRowList.ItemTemplate = themeResouce["lstPivotItemTemplate"] as DataTemplate;
                    this.FilterList.ItemTemplate = themeResouce["lstPivotFilterTemplate"] as DataTemplate;
                }
            }
            catch { }
        }
#else
        void ApplyVisualStyle(string themeName)
        {
            if (themeName != "Default")
            {
                ResourceDictionary resDictionary = new ResourceDictionary();
                SkinManager.SetVisualStyle(this, (VisualStyle)Enum.Parse(typeof(VisualStyle), themeName, true));
                ResourceDictionary resource = new ResourceDictionary() { Source = new Uri("/Syncfusion.PivotAnalysis.Silverlight;component/PivotSchemaDesigner/Themes/Generic.xaml", UriKind.RelativeOrAbsolute) };
                this.Background = resource[themeName + ".PivotSchemaBackground"] as Brush;
                if (themeName.Contains("Office2010"))
                {
                    resDictionary.Source = new Uri("/Syncfusion.PivotAnalysis.Silverlight;component/PivotSchemaDesigner/Themes/Office2010Style.xaml", UriKind.RelativeOrAbsolute);
                }
                else if (themeName.Contains("Office2007"))
                {
                    resDictionary.Source = new Uri("/Syncfusion.PivotAnalysis.Silverlight;component/PivotSchemaDesigner/Themes/Office2007Style.xaml", UriKind.RelativeOrAbsolute);
                }
                else
                    resDictionary.Source = new Uri("/Syncfusion.PivotAnalysis.Silverlight;component/PivotSchemaDesigner/Themes/" + themeName + "Style.xaml", UriKind.RelativeOrAbsolute);
                if (this.PivotCalculationList != null)
                {
                    this.PivotCalculationList.ItemTemplate = resDictionary["lstPivotCalculationTemplate"] as DataTemplate;
                    this.PivotColumnList.ItemTemplate = resDictionary["lstPivotItemTemplate"] as DataTemplate;
                    this.PivotRowList.ItemTemplate = resDictionary["lstPivotItemTemplate"] as DataTemplate;
                    this.FilterList.ItemTemplate = resDictionary["lstPivotFilterItemTemplate"] as DataTemplate;
                }
            }
        }
#endif

        #region [ Event methods ]
#if !SILVERLIGHT
        Point startPoint;
        new void PreviewMouseMove(object sender, MouseEventArgs e)
        {
            ListBox source = (ListBox)sender;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point position = e.GetPosition((IInputElement)source);
                if (Math.Abs(position.X - startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    object _draggedData = GetDataFromListBox(dragSource, startPoint);
                    if (_draggedData != null)
                    {
                        PivotGridControl grid = this.PivotControl as PivotGridControl;
                        if (_draggedData is PivotComputationInfo)
                        {
                            PivotComputationInfo item = _draggedData as PivotComputationInfo;
                            if (!item.AllowRunTimeGroupByField)
                            {
                                if (grid.GroupingBar != null)
                                    grid.GroupingBar.shouldShowBouncingArrows = false;
                            }
                        }
                        else if (_draggedData is PivotItem)
                        {
                            PivotItem item = _draggedData as PivotItem;
                            if (!item.AllowRunTimeGroupByField)
                            {
                                if (grid.GroupingBar != null)
                                    grid.GroupingBar.shouldShowBouncingArrows = false;
                            }
                        }
                        else if (_draggedData is PivotTableField)
                        {
                            PivotTableField item = _draggedData as PivotTableField;
                            if (!item.AllowRunTimeGroupByField)
                            {
                                if (grid.GroupingBar != null)
                                    grid.GroupingBar.shouldShowBouncingArrows = false;
                            }
                        }
                        Console.WriteLine("Dropped");
                        isHooked = false;
                        DragDrop.DoDragDrop(source, _draggedData, DragDropEffects.Move);
                    }
                }
            }
         }
        /// <summary>
        /// Gets the parent element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="child">The child.</param>
        /// <returns>parent element</returns>
        T GetParentElement<T>(DependencyObject child) where T : DependencyObject
        {
            if (child != null)
            {
                DependencyObject dependencyObject = VisualTreeHelper.GetParent(child);
                T parent = dependencyObject as T;
                if (parent != null)
                {
                    return parent;
                }
                else
                {
                    return GetParentElement<T>(dependencyObject);
                }
            }
            else
            {
                return null;
            }
        }

        new void PreviewDrop(object sender, DragEventArgs e)
        {
            PivotGridControl grid = this.PivotControl as PivotGridControl;
            ListBox dropTarget = (ListBox)sender;
            Point hitPoint = e.GetPosition(dropTarget);
            if (VisualTreeHelper.HitTest(dropTarget, hitPoint).VisualHit is Rectangle)
            {
                hitPoint.Y += 2;
            }
            HitTestResult result = VisualTreeHelper.HitTest(dropTarget, hitPoint);
            ToggleButton button = result != null ? GetParentElement<ToggleButton>(result.VisualHit) : null;
            ObservableCollection<PivotItem> pivotItems = dropTarget.ItemsSource as ObservableCollection<PivotItem>;
            PivotItem item = null;

            if (dropTarget == this.PivotColumnList && ((IList)dropTarget.ItemsSource).Count == 0)
                ((IList)dropTarget.ItemsSource).Clear();
            if (e.Data.GetDataPresent(typeof(PivotTableField)))
            {
                PivotTableField sourceProperties = (PivotTableField)e.Data.GetData(typeof(PivotTableField));
                if (!sourceProperties.AllowRunTimeGroupByField)
                {
                    if (grid.GroupingBar != null)
                        grid.GroupingBar.shouldShowBouncingArrows = true;
                    return;
                }
                if (dropTarget == this.PivotCalculationList)
                {
                    this.AddItemToValues(sourceProperties);
                }
                else
                {
                    if (pivotItems != null)
                    {
                        item = pivotItems.Where(p => p.FieldMappingName == sourceProperties.FieldName).FirstOrDefault();
                        if (item == null)
                        {
                            this.PivotControl.PivotColumns.Remove(this.PivotControl.PivotColumns.Where(f => f.FieldMappingName == sourceProperties.FieldName).FirstOrDefault());
                            this.PivotControl.PivotRows.Remove(this.PivotControl.PivotRows.Where(f => f.FieldMappingName == sourceProperties.FieldName).FirstOrDefault());
                            this.PivotControl.Filters.Remove(this.PivotControl.Filters.Where(f => f.Name == sourceProperties.FieldName).FirstOrDefault());
                            ((IList)dropTarget.ItemsSource).Add(GetPivotItem(sourceProperties));
                        }
                    }
                }
                this.PivotControl.PivotFields.Remove(this.PivotControl.PivotFields.Where(f => f.FieldMappingName == sourceProperties.FieldName).FirstOrDefault());
                this.UpdatePivotTableFieldItemSelection(sourceProperties.FieldName);
            }
            else if (e.Data.GetDataPresent(typeof(PivotItem)))
            {
                object data = e.Data.GetData(typeof(PivotItem));
                item = (PivotItem)data;
                if (!item.AllowRunTimeGroupByField)
                {
                    if (grid.GroupingBar != null)
                        grid.GroupingBar.shouldShowBouncingArrows = true;
                    return;
                }
                if (!this.PivotControl.DeferLayoutUpdate && !(this.PivotControl as PivotGridControl).RowPivotsOnly)
                {
                    //// Suspending refresh
                    this.SuspendRefresh();
                    ((IList)dragSource.ItemsSource).Remove(data);

                    //// Resuming refresh
                    this.ResumeRefresh();
                }
                else
                {
                    ((IList)dragSource.ItemsSource).Remove(data);
                }

                if (dropTarget == this.PivotCalculationList)
                {
                    if (!isHooked)
                    {
                        isHooked = true;
                        PivotItem pivotItem = (PivotItem)data;
                        if (button != null)
                        {
                            int insertIndex = ((IList)dropTarget.ItemsSource).IndexOf(button.Tag);
                            Point buttonRelativePosition = e.GetPosition(button);
                            if (!(button.ActualWidth > 0 && (button.ActualWidth / 2) > buttonRelativePosition.Y))
                            {
                                insertIndex++;
                            }
                            if (insertIndex >= 0)// && insertIndex < ((IList)dropTarget.ItemsSource).Count)
                            {
                                this.AddItemToValues(insertIndex, pivotItem);
                            }
                            else if (insertIndex < 0)
                            {
                                this.AddItemToValues(0, pivotItem);
                            }
                        }
                        else if ((this.PivotControl as PivotGridControl).RowPivotsOnly == false)
                            this.AddItemToValues(this.PivotControl.PivotCalculations.Count, pivotItem);
                        else
                            this.AddItemToValues(this.PossiblePivotCalculations.Count, pivotItem);
                    }

                }
                else
                {
                    item = pivotItems.Where(p => p.FieldMappingName == item.FieldMappingName).FirstOrDefault();
                    if (button != null)
                    {
                        int insertIndex = ((IList)dropTarget.ItemsSource).IndexOf(button.Tag);
                        Point buttonRelativePosition = e.GetPosition(button);
                        if (!(button.ActualWidth > 0 && (button.ActualWidth / 2) > buttonRelativePosition.Y))
                        {
                            insertIndex++;
                        }
                        if (insertIndex >= 0)// && insertIndex < ((IList)dropTarget.ItemsSource).Count)
                        {
                            if (item == null)
                                ((IList)dropTarget.ItemsSource).Insert(insertIndex, data); //.Add(data);
                        }
                        else if (insertIndex < 0)
                        {
                            if (item == null)
                                ((IList)dropTarget.ItemsSource).Insert(0, data);
                        }
                    }
                    else if (item == null)
                    {
                        ((IList)dropTarget.ItemsSource).Add(data);
                    }
                }

                if ((this.PivotControl as PivotGridControl).GroupingBar != null)
                    (this.PivotControl as PivotGridControl).GroupingBar.AddEmptyItem(dragSource);
            }
            else if (e.Data.GetDataPresent(typeof(PivotComputationInfo)))
            {
                object data = e.Data.GetData(typeof(PivotComputationInfo));
                PivotComputationInfo compInfo = (PivotComputationInfo)data;
                if (!compInfo.AllowRunTimeGroupByField)
                {
                    if (grid.GroupingBar != null)
                        grid.GroupingBar.shouldShowBouncingArrows = true;
                    return;
                }
                if (pivotItems != null)
                {
                    item = pivotItems.Where(p => p.FieldMappingName == compInfo.FieldName).FirstOrDefault();
                }
                if (item == null)
                {
                    if ((this.PivotTableFields != null && this.PivotTableFields.Count > 0))
                        compInfo.Format = this.PivotTableFields.Where(i => i.FieldName == compInfo.FieldName).FirstOrDefault().Format;
                    if (!this.PivotControl.DeferLayoutUpdate)
                    {
                        this.SuspendRefresh();
                        ((IList)dragSource.ItemsSource).Remove(data);
                        this.ResumeRefresh();
                    }
                    else
                    {
                        ((IList)dragSource.ItemsSource).Remove(data);
                    }
                    if (dropTarget == this.PivotCalculationList)
                    {
                        if (button != null)
                        {
                            int insertIndex = ((IList)dropTarget.ItemsSource).IndexOf(button.Tag);
                            Point buttonRelativePosition = e.GetPosition(button);
                            if (!(button.ActualWidth > 0 && (button.ActualWidth / 2) > buttonRelativePosition.Y))
                            {
                                insertIndex++;
                            }
                            if (insertIndex >= 0)// && insertIndex < ((IList)dropTarget.ItemsSource).Count)
                            {
                                ((IList)dropTarget.ItemsSource).Insert(insertIndex, compInfo);
                            }
                            else if (insertIndex < 0)
                            {
                                ((IList)dropTarget.ItemsSource).Insert(0, compInfo);
                            }
                        }
                        else
                        {
                            ((IList)dropTarget.ItemsSource).Add(compInfo);
                        }
                    }
                    else
                    {
                        this.PivotControl.PivotColumns.Remove(this.PivotControl.PivotColumns.Where(f => f.FieldMappingName == compInfo.FieldName).FirstOrDefault());
                        this.PivotControl.PivotRows.Remove(this.PivotControl.PivotRows.Where(f => f.FieldMappingName == compInfo.FieldName).FirstOrDefault());
                        this.PivotControl.Filters.Remove(this.PivotControl.Filters.Where(f => f.Name == compInfo.FieldName).FirstOrDefault());
                        ((IList)dropTarget.ItemsSource).Add(GetPivotItem(compInfo.FieldHeader, compInfo.FieldName, compInfo.Format, compInfo.AllowRunTimeGroupByField));
                    }

                }
                else
                {
                    this.PivotControl.PivotColumns.Remove(this.PivotControl.PivotColumns.Where(f => f.FieldMappingName == compInfo.FieldName).FirstOrDefault());
                    this.PivotControl.PivotRows.Remove(this.PivotControl.PivotRows.Where(f => f.FieldMappingName == compInfo.FieldName).FirstOrDefault());
                    this.PivotControl.Filters.Remove(this.PivotControl.Filters.Where(f => f.Name == compInfo.FieldName).FirstOrDefault());
                    if (button != null)
                    {
                        int insertIndex = ((IList)dropTarget.ItemsSource).IndexOf(button.Tag);
                        Point buttonRelativePosition = e.GetPosition(button);
                        if (!(button.ActualWidth > 0 && (button.ActualWidth / 2) > buttonRelativePosition.Y))
                        {
                            insertIndex++;
                        }
                        if (insertIndex >= 0)// && insertIndex < ((IList)dropTarget.ItemsSource).Count)
                        {
                            ((IList)dropTarget.ItemsSource).Insert(insertIndex, GetPivotItem(compInfo.FieldHeader, compInfo.FieldName, compInfo.Format, compInfo.AllowRunTimeGroupByField));
                        }
                        else if (insertIndex < 0)
                        {
                            ((IList)dropTarget.ItemsSource).Insert(0, GetPivotItem(compInfo.FieldHeader, compInfo.FieldName, compInfo.Format, compInfo.AllowRunTimeGroupByField));
                        }
                    }
                    else
                    {
                        ((IList)dropTarget.ItemsSource).Add(GetPivotItem(compInfo.FieldHeader, compInfo.FieldName, compInfo.Format, compInfo.AllowRunTimeGroupByField));
                    }
                }
            }
            else if (e.Data.GetDataPresent(typeof(FilterItemsCollection)))
            {
                FilterItemsCollection filterItem = (FilterItemsCollection)e.Data.GetData(typeof(FilterItemsCollection));
                //// Removing the filter item entry from the filters collection
                RemoveFilterItem(filterItem);
                //// Generating a PivotItem based on FilterItemsCollection
                PivotItem pivotItem = GetPivotItem(filterItem);
                pivotItem.ShowSubTotal = filterItem.ShowSubTotal;
                if (!pivotItem.AllowRunTimeGroupByField)
                {
                    if (grid.GroupingBar != null)
                        grid.GroupingBar.shouldShowBouncingArrows = true;
                    return;
                }
                if (pivotItems != null)
                {
                    item = pivotItems.Where(p => p.FieldMappingName == pivotItem.FieldMappingName).FirstOrDefault();
                }
                if (item == null)
                {
                    if (dropTarget == this.PivotCalculationList)
                    {
                        if (!isHooked)
                        {
                            isHooked = true;
                            if (button != null)
                            {
                                int insertIndex = ((IList)dropTarget.ItemsSource).IndexOf(button.Tag);
                                Point buttonRelativePosition = e.GetPosition(button);
                                if (!(button.ActualWidth > 0 && (button.ActualWidth / 2) > buttonRelativePosition.Y))
                                {
                                    insertIndex++;
                                }
                                if (insertIndex >= 0)// && insertIndex < ((IList)dropTarget.ItemsSource).Count)
                                {
                                    this.AddItemToValues(insertIndex, pivotItem);
                                }
                                else if (insertIndex < 0)
                                {
                                    this.AddItemToValues(0, pivotItem);
                                }
                            }
                            else
                                this.AddItemToValues(this.PivotControl.PivotCalculations.Count, pivotItem);
                        }
                    }
                    else
                    {
                        //// Adding the PivotItem to droped target.
                        if (button != null)
                        {
                            int insertIndex = ((IList)dropTarget.ItemsSource).IndexOf(button.Tag);
                            Point buttonRelativePosition = e.GetPosition(button);
                            if (!(button.ActualWidth > 0 && (button.ActualWidth / 2) > buttonRelativePosition.Y))
                            {
                                insertIndex++;
                            }
                            item = pivotItems.Where(p => p.FieldMappingName == pivotItem.FieldMappingName).FirstOrDefault();

                            if (insertIndex >= 0)// && insertIndex < ((IList)dropTarget.ItemsSource).Count)
                            {
                                if (item == null)
                                    ((IList)dropTarget.ItemsSource).Insert(insertIndex, pivotItem);
                            }
                            else if (insertIndex < 0)
                            {
                                if (item == null)
                                    ((IList)dropTarget.ItemsSource).Insert(0, pivotItem);
                            }
                        }
                        else
                            ((IList)dropTarget.ItemsSource).Add(pivotItem);
                    }
                    //// Updating the selection status
                    UpdatePivotTableFieldItemSelection(pivotItem.FieldMappingName);
                    if (this.FilterList.Items.Count == 0 && (this.PivotControl as PivotGridControl).GroupingBar != null)
                    {
                        (this.PivotControl as PivotGridControl).GroupingBar.ApplyEmptyTemplate();
                    }
                }
            }
        }

        void ReportFilter_PreviewDrop(object sender, DragEventArgs e)
        {
            PivotGridControl grid = this.PivotControl as PivotGridControl;
            ListBox dropTarget = (ListBox)sender;
            Point hitPoint = e.GetPosition(dropTarget);
            if (VisualTreeHelper.HitTest(dropTarget, hitPoint).VisualHit is Rectangle)
            {
                hitPoint.Y += 2;
            }            HitTestResult result = VisualTreeHelper.HitTest(dropTarget, hitPoint);
            ToggleButton button = result != null ? GetParentElement<ToggleButton>(result.VisualHit) : null;
            if (e.Data.GetDataPresent(typeof(PivotTableField)))
            {
                PivotTableField sourceField = (PivotTableField)e.Data.GetData(typeof(PivotTableField));
                if (!sourceField.AllowRunTimeGroupByField)
                {
                    if (grid.GroupingBar != null)
                        grid.GroupingBar.shouldShowBouncingArrows = true;
                    return;
                }
                if (this.Filters.Where(f => f.Name == sourceField.FieldName).FirstOrDefault() == null)
                {

                    this.PivotControl.PivotRows.Remove(this.PivotControl.PivotRows.Where(f => f.FieldMappingName == sourceField.FieldName).FirstOrDefault());
                    this.PivotControl.PivotColumns.Remove(this.PivotControl.PivotColumns.Where(f => f.FieldMappingName == sourceField.FieldName).FirstOrDefault());
                    FilterItemsCollection filterList = GetFilterItem(sourceField.FieldPropertyDescriptor);
                    filterList.DisplayHeader = GetPivotTableFieldHeader(filterList.Name);
                    filterList.Format = GetPivotTableFormat(filterList.Name, filterList.GetType());
                    this.Filters.Add(filterList);
                    this.UpdateFilters(filterList);
                    if ((this.PivotControl as PivotGridControl).GroupingBar != null)
                    {
                        (this.PivotControl as PivotGridControl).GroupingBar.Filters = this.Filters;
                        (this.PivotControl as PivotGridControl).GroupingBar.ApplyTemplateToGroupingControl((this.PivotControl as PivotGridControl).GroupingBar.FilterHeaderArea, null);
                    }
                }
                this.PivotControl.PivotFields.Remove(this.PivotControl.PivotFields.Where(f => f.FieldMappingName == sourceField.FieldName).FirstOrDefault());
                UpdatePivotTableFieldItemSelection(sourceField.FieldName);
            }
            else if (e.Data.GetDataPresent(typeof(PivotItem)))
            {
                PivotItem pivotItem = (PivotItem)e.Data.GetData(typeof(PivotItem));
                if (!pivotItem.AllowRunTimeGroupByField)
                {
                    if (grid.GroupingBar != null)
                        grid.GroupingBar.shouldShowBouncingArrows = true;
                    return;
                }
                if (this.Filters.Where(f => f.Name == pivotItem.FieldMappingName).FirstOrDefault() == null)
                {
                    PropertyDescriptor descriptor = GetPropertyDescriptor(pivotItem.FieldMappingName);
                    if (descriptor != null)
                    {
                        if (!this.PivotControl.DeferLayoutUpdate)
                        {
                            //// Suspending refresh
                            this.SuspendRefresh();
                            ((IList)dragSource.ItemsSource).Remove(pivotItem);

                            //// Resuming refresh
                            this.ResumeRefresh();
                        }
                        else
                        {
                            ((IList)dragSource.ItemsSource).Remove(pivotItem);
                        }
                        FilterItemsCollection filterList = GetFilterItem(descriptor);
                        filterList.DisplayHeader = pivotItem.FieldHeader;
                        filterList.Format = pivotItem.Format;
                        filterList.AllowRunTimeGroupByField = pivotItem.AllowRunTimeGroupByField;
                        filterList.ShowSubTotal = pivotItem.ShowSubTotal;

                        if (button != null)
                        {
                            int insertIndex = ((IList)dropTarget.ItemsSource).IndexOf(button.Tag);
                            Point buttonRelativePosition = e.GetPosition(button);
                            if (!(button.ActualWidth > 0 && (button.ActualWidth / 2) > buttonRelativePosition.Y))
                            {
                                insertIndex++;
                            }
                            if (insertIndex >= 0)// && insertIndex < ((IList)dropTarget.ItemsSource).Count)
                            {
                                this.Filters.Insert(insertIndex, filterList);
                            }
                            else if (insertIndex < 0)
                            {
                                this.Filters.Insert(0, filterList);
                            }
                        }
                        else
                            this.Filters.Add(filterList);
                        if ((this.PivotControl as PivotGridControl).GroupingBar != null)
                        {
                            (this.PivotControl as PivotGridControl).GroupingBar.Filters = this.Filters;
                            (this.PivotControl as PivotGridControl).GroupingBar.ApplyTemplateToGroupingControl((this.PivotControl as PivotGridControl).GroupingBar.FilterHeaderArea, null);
                        }
                        this.UpdateFilters(filterList);
                        
                    }
                }
                if ((this.PivotControl as PivotGridControl).GroupingBar != null)
                    (this.PivotControl as PivotGridControl).GroupingBar.AddEmptyItem(dragSource);
                UpdatePivotTableFieldItemSelection(pivotItem.FieldMappingName);
            }
            else if (e.Data.GetDataPresent(typeof(PivotComputationInfo)))
            {
                PivotComputationInfo pivotComputationInfo = (PivotComputationInfo)e.Data.GetData(typeof(PivotComputationInfo));
                if (!pivotComputationInfo.AllowRunTimeGroupByField)
                {
                    if (grid.GroupingBar != null)
                        grid.GroupingBar.shouldShowBouncingArrows = true;
                    return;
                }
                if (this.Filters.Where(f => f.Name == pivotComputationInfo.FieldName).FirstOrDefault() == null)
                {
                    PropertyDescriptor descriptor = GetPropertyDescriptor(pivotComputationInfo.FieldName);
                    if (descriptor != null)
                    {
                        this.PivotControl.PivotRows.Remove(this.PivotControl.PivotRows.Where(f => f.FieldMappingName == pivotComputationInfo.FieldName).FirstOrDefault());
                        this.PivotControl.PivotColumns.Remove(this.PivotControl.PivotColumns.Where(f => f.FieldMappingName == pivotComputationInfo.FieldName).FirstOrDefault());
                        ((IList)dragSource.ItemsSource).Remove(pivotComputationInfo);
                        FilterItemsCollection filterList = GetFilterItem(descriptor);
                        filterList.DisplayHeader = pivotComputationInfo.FieldHeader;
                        filterList.Format = pivotComputationInfo.Format;
                        if (button != null)
                        {
                            int insertIndex = ((IList)dropTarget.ItemsSource).IndexOf(button.Tag);
                            Point buttonRelativePosition = e.GetPosition(button);
                            if (!(button.ActualWidth > 0 && (button.ActualWidth / 2) > buttonRelativePosition.Y))
                            {
                                insertIndex++;
                            }
                            if (insertIndex >= 0)// && insertIndex < ((IList)dropTarget.ItemsSource).Count)
                            {
                                this.Filters.Insert(insertIndex, filterList);
                            }
                            else if (insertIndex < 0)
                            {
                                this.Filters.Insert(0, filterList);
                            }
                        }
                        else
                            this.Filters.Add(filterList);
                        this.UpdateFilters(filterList);
                        if ((this.PivotControl as PivotGridControl).GroupingBar != null)
                        {
                            (this.PivotControl as PivotGridControl).GroupingBar.Filters = this.Filters;
                            (this.PivotControl as PivotGridControl).GroupingBar.ApplyTemplateToGroupingControl((this.PivotControl as PivotGridControl).GroupingBar.FilterHeaderArea, null);
                        }
                    }
                }
                UpdatePivotTableFieldItemSelection(pivotComputationInfo.FieldName);
            }
            else if (e.Data.GetDataPresent(typeof(FilterItemsCollection)))
            {
                FilterItemsCollection filterItem = (FilterItemsCollection)e.Data.GetData(typeof(FilterItemsCollection));
                //// Removing the filter item entry from the filters collection
                RemoveFilterItem(filterItem);
                if (button != null)
                {
                    int insertIndex = ((IList)dropTarget.ItemsSource).IndexOf(button.Tag);
                    Point buttonRelativePosition = e.GetPosition(button);
                    if (!(button.ActualWidth > 0 && (button.ActualWidth / 2) > buttonRelativePosition.Y))
                    {
                        insertIndex++;
                    }
                    if (insertIndex >= 0)// && insertIndex < ((IList)dropTarget.ItemsSource).Count)
                    {
                        this.Filters.Insert(insertIndex, filterItem);
                    }
                    else if (insertIndex < 0)
                    {
                        this.Filters.Insert(0, filterItem);
                    }
                }
                else
                    this.Filters.Add(filterItem);
                this.UpdateFilters(filterItem);
                if ((this.PivotControl as PivotGridControl).GroupingBar != null)
                {
                    (this.PivotControl as PivotGridControl).GroupingBar.Filters = this.Filters;
                    (this.PivotControl as PivotGridControl).GroupingBar.ApplyTemplateToGroupingControl((this.PivotControl as PivotGridControl).GroupingBar.FilterHeaderArea, null);
                }
                UpdatePivotTableFieldItemSelection(filterItem.Name);
            }
        }

        private void UpdateFilters(FilterItemsCollection filterList)
        {
            if (this.PivotControl != null)
            {
                filterList.AcceptChanges();
                FilterExpression filterExpression = this.PivotControl.Filters.Where(x => x.Name == filterList.Name).FirstOrDefault();
                if (filterExpression == null)
                {
                    if (PivotControl.ItemSource is System.Data.DataView || PivotControl.ItemSource is System.Data.DataTable)
                    {
                        PivotControl.Filters.Add(new FilterExpression { Name = this.FilterList.Name, Expression = filterList.GetFilterExpressionForDataView(), DimensionHeader=filterList.Name, Tag = this.FilterList });
                    }
                    else if (PivotControl.ItemSource is IEnumerable)
                    {
                        this.PivotControl.Filters.Add(new FilterExpression { Name = filterList.Name, Expression = filterList.GetFilterExpression(true), DimensionHeader=filterList.Name, Tag = filterList });
                    }
                    else if (PivotControl.ItemSource is IListSource)
                    {
                        this.PivotControl.Filters.Add(new FilterExpression { Name = filterList.Name, Expression = filterList.GetFilterExpression(false), DimensionHeader=filterList.Name, Tag = filterList });
                    }
                }
                else
                {
                    if (PivotControl.ItemSource is System.Data.DataView)
                    {
                        filterExpression.Expression = filterList.GetFilterExpressionForDataView();
                    }
                    else if (PivotControl.ItemSource is IEnumerable)
                    {
                        filterExpression.Expression = filterList.GetFilterExpression(true);
                    }
                    else if (PivotControl.ItemSource is IListSource)
                    {
                        filterExpression.Expression = filterList.GetFilterExpression(false);
                    }
                    ////On FilterExpression change raise PivotSchemaDesigner changed event.
                    this.PivotControl.PivotEngine.RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
                }
            }
        }


        new void PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBox source = (ListBox)sender;
            dragSource = source;
            startPoint = e.GetPosition(dragSource);
        }

        void Calculations_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBox source = (ListBox)sender;
            object data = GetDataFromListBox(source, e.GetPosition(source));
            if (data != null)
            {
                string[] pivotRowColumnNames = new string[this.PivotControl.PivotRows.Count + this.PivotControl.PivotColumns.Count];
                int indx=0;
                foreach (var item in this.PivotControl.PivotRows)
                {
                    pivotRowColumnNames[indx++] = item.FieldMappingName;
                }
                foreach (var item in this.PivotControl.PivotColumns)
                {
                    pivotRowColumnNames[indx++] = item.FieldMappingName;
                }
                ComputationInfoWindow compWindow = new ComputationInfoWindow(data, pivotRowColumnNames);
                if (this.PivotControl is PivotGridControl)
                {
                    Syncfusion.Windows.Shared.SkinStorage.SetVisualStyle(compWindow, Syncfusion.Windows.Shared.SkinStorage.GetVisualStyle(this));
                }
                foreach (PivotComputationInfo info in this.PivotControl.PivotCalculations)
                {
                    if (info.SummaryType == SummaryType.Custom)
                    {
                        if (info.Summary.GetType().Name.Equals("CountSummary"))
                        {
                            compWindow.cmbSummaryType.SelectedIndex = 6;
                            continue;
                        }
                        else if (this.CustomSummaryBaseCollection == null)
                        {
                            this.CustomSummaryBaseCollection = new ObservableCollection<SummaryBase>();
                            this.CustomSummaryBaseCollection.Add(info.Summary);
                        }
                        else if (!this.CustomSummaryBaseCollection.Contains(info.Summary))
                        {
                            this.CustomSummaryBaseCollection.Add(info.Summary);
                        }
                    }
                }

                if (this.CustomSummaryBaseCollection != null && this.CustomSummaryBaseCollection.Count > 0 && compWindow.cmbSummaryType.Items.Contains(SummaryType.Custom))
                {
                    compWindow.cmbSummaryType.Items.Remove(SummaryType.Custom);
                    foreach (SummaryBase summaryBase in this.CustomSummaryBaseCollection)
                        compWindow.cmbSummaryType.Items.Add(summaryBase.GetType().Name);
                }
                compWindow.cmbSummaryType.SelectedItem = compWindow.ComputationInfo.Summary.GetType().Name;

                compWindow.Owner = Utils.GetParentItem<Window>(this);
                if (compWindow.ShowDialog() == true)
                {
                    if (compWindow.IsDirty)
                    {
                        if (compWindow.ComputationInfoClone.SummaryType == SummaryType.Custom && this.CustomSummaryBaseCollection != null && this.CustomSummaryBaseCollection.Count > 0)
                        {
                            object selectedItem = compWindow.cmbSummaryType.SelectedItem;
                            foreach (SummaryBase sum in this.CustomSummaryBaseCollection)
                            {
                                if (sum.GetType().Name.Equals(selectedItem))
                                {
                                    compWindow.ComputationInfo.Summary = sum;
                                    break;
                                }
                            }
                        }
                        this.PivotControl.PivotEngine.RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs()); 
                    }
                    else if (compWindow.IsCalculationChanged)
                    {
                        this.PivotControl.PivotEngine.RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs { ChangeHints = SchemaChangeHints.CalculationChanged });
                    }
                    else if (compWindow.IsHeadersChanged)
                    {
                        this.PivotControl.PivotEngine.RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs { ChangeHints = SchemaChangeHints.HeadersChanged });
                    }
                }
            }
        }
#endif
        #endregion

        #region [ Command Methods ]

#if !SILVERLIGHT
        /// <summary>
        /// A method that determines whether to execute deletion of pivot item or not.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event data.</param>
        public void DeletePivotItemCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        /// <summary>
        /// A method that executes on deleting a pivot item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event data.</param>
        public void DeletePivotItemExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DeletePivots(e.OriginalSource as Button);
        }
#endif
        /// <summary>
        /// Deletes the pivot item executed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void DeletePivotItemExecuted(object sender)
        {
            DeletePivots(sender as Button);
        }

        private void DeletePivots(Button button)
        {
            if (button != null && button.Tag != null)
            {
                ListBox parentListBox = Utils.GetParentItem<ListBox>(button);
                if (parentListBox != null)
                {
                    if (button.Tag is PivotItem)
                    {
                        PivotItem pivotItem = (PivotItem)button.Tag;
                        ((IList)parentListBox.ItemsSource).Remove(pivotItem);
                        this.PivotControl.PivotFields.Add(GetPivotItem(pivotItem.FieldHeader, pivotItem.FieldMappingName, pivotItem.AllowRunTimeGroupByField, pivotItem.Format));
                        UpdatePivotTableFieldItemSelection(pivotItem.FieldHeader);
                    }
                    else if (button.Tag is PivotComputationInfo)
                    {
                        PivotComputationInfo computationInfo = (PivotComputationInfo)button.Tag;
                        ((IList)parentListBox.ItemsSource).Remove(computationInfo);
#if !SILVERLIGHT
                        if ((this.PivotControl as PivotGridControl).RowPivotsOnly == false)
                        {
#endif
                            PivotItem pivotItem = GetPivotItem(computationInfo.FieldHeader, computationInfo.FieldName, computationInfo.AllowRunTimeGroupByField, computationInfo.Format);
                            pivotItem.SummaryType = computationInfo.SummaryType;
                            pivotItem.Summary = computationInfo.Summary;
                            this.PivotControl.PivotFields.Add(pivotItem);
#if !SILVERLIGHT
                        }
                        else
                        {
                            (this.PivotControl as PivotGridControl).InternalGrid.SetValueColumnVisibility(computationInfo.FieldName, true);
                            PivotValueField pivotValueField = (this.PivotControl as PivotGridControl).LocalPossibleCalculations.Where(j => j.FieldName == computationInfo.FieldName).FirstOrDefault();
                            if (pivotValueField != null)
                                pivotValueField.IsSelected = false;
                        }

                        UpdatePivotTableFieldItemSelection(computationInfo.FieldName);
#endif
                    }
                    if ((this.PivotControl as PivotGridControl).GroupingBar != null)
                        (this.PivotControl as PivotGridControl).GroupingBar.AddEmptyItem(parentListBox);
                }
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// A method that determines whether to execute the removal of filter item or not.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event data.</param>
        public void DeleteFilterCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        /// <summary>
        /// A method that determines whether to execute the filter popup window to show or not.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event data.</param>
        public void ShowFilterCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        /// <summary>
        /// A method that executes the filter popup window to show.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event data.</param>
        public void ShowFilterExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Button button = e.OriginalSource as Button;
            if (button != null && button.Tag != null)
            {
                object data = button.Tag;
                int index = this.Filters.IndexOf(data as FilterItemsCollection);
                FilterItemsCollection filterList = data as FilterItemsCollection;
                if (index >= 0 && filterList != null)
                {
                    index++;
                    Window parentWindow = Utils.GetParentItem<Window>(this);
                    GeneralTransform transformation = this.FilterList.TransformToVisual(this);

                    Point windowPosition = transformation.Transform(new Point(0, 0));
                    windowPosition = this.PointToScreen(windowPosition);
                    PopupWindow popupWindow = new PopupWindow(this.PivotControl, filterList);
                    if (this.PivotControl is PivotGridControl)
                    {
                        Syncfusion.Windows.Shared.SkinStorage.SetVisualStyle(popupWindow, Syncfusion.Windows.Shared.SkinStorage.GetVisualStyle(this));
                        popupWindow.Background = (this.PivotControl as PivotGridControl).Background;
                    }
                    popupWindow.Owner = parentWindow;
                    popupWindow.Left = windowPosition.X;
                    popupWindow.Top = windowPosition.Y + 6 + (index * 18.96);
                    popupWindow.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Deletes the filter executed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event data.</param>
        public void DeleteFilterExecuted(object sender, ExecutedRoutedEventArgs e)
#endif
#if SILVERLIGHT
        /// <summary>
        /// Deletes the filter executed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void DeleteFilterExecuted(object sender)
#endif
        {
#if !SILVERLIGHT
            Button button = e.OriginalSource as Button;
#else
            Button button = sender as Button;
#endif
            if (button != null && button.Tag != null)
            {
                //// Removing item if exist in Filters
                FilterItemsCollection filterItem = button.Tag as FilterItemsCollection;
                RemoveFilterItem(filterItem);
                this.PivotControl.PivotFields.Add(GetPivotItem(filterItem.DisplayHeader, filterItem.Name, filterItem.AllowRunTimeGroupByField));
            }
        }

#if SILVERLIGHT

        private void RegisterCommands(ListBox listBox)
        {
            if (listBox == null) return;
            for (int i = 0; i < listBox.Items.Count; i++)
            {
                ListBoxItem item = listBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;

                Button btn = Common.FindVisualChildWithName<Button>(item, "PART_PivotItemButton") as Button;
                if (btn != null)
                {
                    btn.Click += new RoutedEventHandler(btn_Click);
                    if (btn.Command != null)
                    {
                        btn.Command = new DeletePivotItemCommand();
                        btn.Command.CanExecuteChanged += new EventHandler(Command_CanExecuteDelete);
                    }
                }

                Button filterBtn = Common.FindVisualChildWithName<Button>(item, "PART_ShowCalcButton") as Button;
                if (filterBtn != null)
                {
                    filterBtn.Click += new RoutedEventHandler(btn_Click);
                    if (filterBtn.Command != null)
                    {
                        filterBtn.Command = new ShowCalculationCommand();
                        filterBtn.Command.CanExecuteChanged += new EventHandler(Command_ShowCalculationCanExecute);
                    }
                }
            }
        }

        private void RegisterFilterCommands(ListBox listBox)
        {
            if (listBox == null) return;
            for (int i = 0; i < listBox.Items.Count; i++)
            {
                ListBoxItem item = listBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                if (item == null) return;
                Button btn = Common.FindVisualChild<Button>(item);
                if (btn != null)
                {
                    btn.Click += new RoutedEventHandler(btn_Click);
                    if (btn.Command != null)
                    {
                        btn.Command = new DeleteFilterCommand();
                        btn.Command.CanExecuteChanged += new EventHandler(Command_CanExecuteDeleteFilter);
                    }
                }

                Button filterBtn = Common.FindVisualChildWithName<Button>(item, "PART_FilterBtn") as Button;
                if (filterBtn != null)
                {
                    filterBtn.Click += new RoutedEventHandler(btn_Click);
                    if (filterBtn.Command != null)
                    {
                        filterBtn.Command = new ShowFilterCommand();
                        filterBtn.Command.CanExecuteChanged += new EventHandler(Command_ShowFilterCanExecute);
                    }
                }
            }
        }

        internal Button FilterButton { get; set; }
        public event CommandExecuteChanged CommandExecute;

        void btn_Click(object sender, RoutedEventArgs e)
        {
            FilterButton = sender as Button;
        }

        void Command_CanExecuteDelete(object sender, EventArgs e)
        {
            if (this.CommandExecute != null)
            {
                CommandExecute(this, new CommandEventArgs(sender, false, FilterButton));
            }
            DeletePivots(FilterButton);
        }

        void Command_CanExecuteDeleteFilter(object sender, EventArgs e)
        {
            if (this.CommandExecute != null)
            {
                CommandExecute(this, new CommandEventArgs(sender, false, FilterButton));
            }
            DeleteFilterExecuted(FilterButton);
        }

        void Command_ShowCalculationCanExecute(object sender, EventArgs e)
        {
            if (this.CommandExecute != null)
            {
                CommandExecute(this, new CommandEventArgs(sender, false, FilterButton));
            }
            ShowCalculationOnClick(FilterButton);
        }

        public void Command_ShowFilterCanExecute(object sender, EventArgs e)
        {
            if (this.CommandExecute != null)
            {
                CommandExecute(this, new CommandEventArgs(sender, false, FilterButton));
            }
            ShowFilterOnClick(FilterButton);
        }

        private void ShowFilterOnClick(object sender)
        {
            Button button = sender as Button;
            if (button != null && button.Tag != null)
            {
                object data = button.Tag;
                int index = this.Filters.IndexOf(data as FilterItemsCollection);
                FilterItemsCollection filterList = data as FilterItemsCollection;
                if (index >= 0 && filterList != null)
                {
                    index++;
                    GeneralTransform transformation = this.FilterList.TransformToVisual(this);
                    Point windowPosition = transformation.Transform(new Point(0, 0));
                    PopupWindow popupWindow = new PopupWindow(this.PivotControl, filterList);
                    if (this.PivotControl is PivotGridControl)
                    {
                        popupWindow.VisualStyle = (Syncfusion.Windows.Shared.VisualStyle)Enum.Parse(typeof(Syncfusion.Windows.Shared.VisualStyle), (this.PivotControl as PivotGridControl).VisualStyle.ToString(), true);
                    }
                    popupWindow.Owner = this;
                    popupWindow.Left = windowPosition.X;
                    popupWindow.Top = windowPosition.Y + 6 + (index * 18.96);
                    popupWindow.ShowDialog();
                }
            }
        }

        ComputationInfoWindow compWindow;
        private void ShowCalculationOnClick(object sender)
        {
            Button button = sender as Button;
            if (button != null && button.Tag != null)
            {
                object data = button.Tag;
                string[] pivotRowColumnNames = new string[this.PivotControl.PivotRows.Count + this.PivotControl.PivotColumns.Count];
                int indx = 0;
                foreach (var item in this.PivotControl.PivotRows)
                {
                    pivotRowColumnNames[indx++] = item.FieldMappingName;
                }
                foreach (var item in this.PivotControl.PivotColumns)
                {
                    pivotRowColumnNames[indx++] = item.FieldMappingName;
                }
                compWindow = new ComputationInfoWindow(data, pivotRowColumnNames);
                if (this.PivotControl is PivotGridControl)
                {
                    compWindow.VisualStyle = (Syncfusion.Windows.Shared.VisualStyle)Enum.Parse(typeof(Syncfusion.Windows.Shared.VisualStyle), (this.PivotControl as PivotGridControl).VisualStyle.ToString(),true);
                    SkinManager.SetVisualStyle(compWindow, (Windows.Controls.Theming.VisualStyle)Enum.Parse(typeof(Windows.Controls.Theming.VisualStyle), (this.PivotControl as PivotGridControl).VisualStyle.ToString(), true));
                    if(compWindow.VisualStyle != Windows.Shared.VisualStyle.Metro && compWindow.VisualStyle != Windows.Shared.VisualStyle.Transparent)
                        compWindow.Background = (this.PivotControl as PivotGridControl).Background;
                }

                foreach (PivotComputationInfo info in this.PivotControl.PivotCalculations)
                {
                    if (info.SummaryType == SummaryType.Custom)
                    {
                        if (info.Summary.GetType().Name.Equals("CountSummary"))
                        {
                            compWindow.cmbSummaryType.SelectedIndex = 6;
                            continue;
                        }
                        else if (this.CustomSummaryBaseCollection == null)
                        {
                            this.CustomSummaryBaseCollection = new ObservableCollection<SummaryBase>();
                            this.CustomSummaryBaseCollection.Add(info.Summary);
                        }
                        else if (!this.CustomSummaryBaseCollection.Contains(info.Summary))
                        {
                            this.CustomSummaryBaseCollection.Add(info.Summary);
                        }
                    }
                }

                if (this.CustomSummaryBaseCollection != null && this.CustomSummaryBaseCollection.Count > 0 && compWindow.cmbSummaryType.Items.Contains(SummaryType.Custom))
                {
                    compWindow.cmbSummaryType.Items.Remove(SummaryType.Custom);
                    foreach (SummaryBase summaryBase in this.CustomSummaryBaseCollection)
                        compWindow.cmbSummaryType.Items.Add(summaryBase.GetType().Name);
                }
                compWindow.cmbSummaryType.SelectedItem = compWindow.ComputationInfo.Summary.GetType().Name;

                compWindow.Owner = this;
                compWindow.ShowDialog();
                compWindow.Closed += new ClosedEventHandler(compWindow_Closed);
            }
        }

        void compWindow_Closed(object sender, ClosedEventArgs e)
        {
            if (compWindow.IsDirty)
            {
                if (compWindow.ComputationInfoClone.SummaryType == SummaryType.Custom && this.CustomSummaryBaseCollection != null && this.CustomSummaryBaseCollection.Count > 0)
                {
                    object selectedItem = compWindow.cmbSummaryType.SelectedItem;
                    foreach (SummaryBase sum in this.CustomSummaryBaseCollection)
                    {
                        if (sum.GetType().Name.Equals(selectedItem))
                        {
                            compWindow.ComputationInfo.Summary = sum;
                            break;
                        }
                    }
                }
                this.PivotControl.PivotEngine.RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
            }
            else if (compWindow.IsCalculationChanged)
            {
                this.PivotControl.PivotEngine.RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs { ChangeHints = SchemaChangeHints.CalculationChanged });
            }
            else if (compWindow.IsHeadersChanged)
            {
                this.PivotControl.PivotEngine.RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs { ChangeHints = SchemaChangeHints.HeadersChanged });
            }
        }
#endif

        #endregion
        #endregion
    }
}