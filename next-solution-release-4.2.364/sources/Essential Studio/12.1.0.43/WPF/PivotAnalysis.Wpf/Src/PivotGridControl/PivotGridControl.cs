#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Collections;
using System.ComponentModel;
#if SILVERLIGHT
using Syncfusion.Silverlight.Controls.PivotSchemaDesigner;
using Syncfusion.PivotAnalysis.Base.Silverlight;
#endif
#if !SILVERLIGHT
using Syncfusion.Licensing;
using Syncfusion.PivotAnalysis.Base;
using Syncfusion.Windows.Controls.Grid;
using System.Windows.Controls.Primitives;
using Syncfusion.Windows.Shared;
using System.Xml.Serialization;
using System.Runtime.Serialization;
#if SyncfusionFramework4_0
using System.Xaml;
#endif
using System.Xml;
using System.Windows.Markup;
using Microsoft.Win32;
using Syncfusion.Windows.Controls.PivotSchemaDesigner;
using Syncfusion.Windows.Tools.Controls;
using System.Globalization;
using Syncfusion.Windows.Controls.PivotGrid;

namespace Syncfusion.Windows.Controls.PivotGrid
#else
using Syncfusion.Windows;
using Syncfusion.Windows.Controls.Grid;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using Syncfusion.Windows.Controls;
using System.Xml.Serialization;
using Syncfusion.Windows.Controls.Theming;
using Syncfusion.Windows.Controls.PivotGrid;
using Syncfusion.Windows.Tools.Controls;
namespace Syncfusion.Silverlight.Controls.PivotGrid
#endif
{
    /// <summary>
    /// PivotGridControl provides the functionality of a pivot table. A pivot table is a data summarization tool
    /// which automatically sorts, count and total the table data and represents in cross tabular format.
    /// </summary>
    /// <remarks>
    /// To use this control, you must set the <see cref="ItemSource"/> to some IList object.
    /// The object in the IList should have public property which you want to use it in the pivot table.
    /// To represent the data in cross tabular format you should specify the pivoting info in these
    /// properties <see cref="PivotRows"/>, <see cref="PivotColumns"/> and <see cref="PivotCalculations"/>
    /// </remarks>
#if !SILVERLIGHT
    [Serializable]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(PivotGridControl), XamlResource = "/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.Brushes.Blend.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(PivotGridControl), XamlResource = "/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.Brushes.Office2003.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(PivotGridControl), XamlResource = "/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.Brushes.Office2007Black.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
    Type = typeof(PivotGridControl), XamlResource = "/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.Brushes.Office2007Blue.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(PivotGridControl), XamlResource = "/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.Brushes.Office2007Silver.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
   Type = typeof(PivotGridControl), XamlResource = "/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.Brushes.Office2010Black.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
   Type = typeof(PivotGridControl), XamlResource = "/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.Brushes.Office2010Blue.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
   Type = typeof(PivotGridControl), XamlResource = "/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.Brushes.Office2010Silver.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
  Type = typeof(PivotGridControl), XamlResource = "/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.Brushes.Transparent.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(PivotGridControl), XamlResource = "/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.Brushes.Classic.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
   Type = typeof(PivotGridControl), XamlResource = "/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.Brushes.Metro.xaml")]
#else
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
       Type = typeof(PivotGridControl), XamlResource = "/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.Brushes.Office2010Silver.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
       Type = typeof(PivotGridControl), XamlResource = "/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.Brushes.Office2010Blue.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
       Type = typeof(PivotGridControl), XamlResource = "/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.Brushes.Office2010Black.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
       Type = typeof(PivotGridControl), XamlResource = "/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.Brushes.Office2007Blue.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
       Type = typeof(PivotGridControl), XamlResource = "/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.Brushes.Office2007Black.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
       Type = typeof(PivotGridControl), XamlResource = "/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.Brushes.Office2007Silver.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
       Type = typeof(PivotGridControl), XamlResource = "/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.Brushes.Blend.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
       Type = typeof(PivotGridControl), XamlResource = "/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.Brushes.Metro.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
      Type = typeof(PivotGridControl), XamlResource = "/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.Brushes.Transparent.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
       Type = typeof(PivotGridControl), XamlResource = "/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.xaml")]

#endif
    public class PivotGridControl : Control, IPivotControl
    {
        #region [ Private Variables]

        /// <summary>
        /// FieldInfo collection for default fields
        /// </summary>
        private List<FieldInfo> allowedFields;


        /// <summary>
        /// PivotItem collection for PivotFields
        /// </summary>
        ObservableCollection<PivotItem> _PivotFields;

        /// <summary>
        /// FilterItem collection for PivotFields
        /// </summary>
        internal ObservableCollection<FilterItemsCollection> FilterItems;

        private bool _IsExtenalEngine;

        private int m_AutoSizeColumnCount = 0;

        private bool grandTotalRowAlwaysVisible = false;
#if SILVERLIGHT
        private double m_verticalOffset = 0;

        internal double m_actualVerticalOffset = 0;
#endif

#if !SILVERLIGHT
        private int m_AutoSizeRowCount = 0;
#endif

        #endregion

        #region [ Initialize/Finalize ]

#if !SILVERLIGHT
        static PivotGridControl()
        {
            EnvironmentTest.ValidateLicense(typeof(PivotGridControl));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PivotGridControl), new FrameworkPropertyMetadata(typeof(PivotGridControl)));

            CommandBinding refreshCommandBinding = new CommandBinding(PivotGridCommands.Refresh, new ExecutedRoutedEventHandler(OnRefreshCommand));
            CommandBinding resetPivotDataCommandBinding = new CommandBinding(PivotGridCommands.ResetPivotData, new ExecutedRoutedEventHandler(OnResetPivotDataCommand));
            CommandBinding serializeCommandBinding = new CommandBinding(PivotGridCommands.Serialize, new ExecutedRoutedEventHandler(OnSerializeCommand));
            CommandBinding deserializeCommandBinding = new CommandBinding(PivotGridCommands.Deserialize, new ExecutedRoutedEventHandler(OnDeserializeCommand));
            CommandBinding populateDefaultFieldsCommandBinding = new CommandBinding(PivotGridCommands.PopulateDefaultPropertyFields, new ExecutedRoutedEventHandler(OnPopulateDefaultFieldsCommand));
            CommandBinding expandAllCommandBinding = new CommandBinding(PivotGridCommands.ExpandAll, new ExecutedRoutedEventHandler(OnExpandAllCommandExecuted));
            CommandBinding expandRowCommandBinding = new CommandBinding(PivotGridCommands.ExpandRow, new ExecutedRoutedEventHandler(OnExpandRowCommandExecuted));
            CommandBinding expandColumnCommandBinding = new CommandBinding(PivotGridCommands.ExpandColumn, new ExecutedRoutedEventHandler(OnExpandColumnCommandExecuted));
            CommandBinding collapseAllCommandBinding = new CommandBinding(PivotGridCommands.CollapseAll, new ExecutedRoutedEventHandler(OnCollapseAllCommandExecuted));
            CommandBinding collapseRowCommandBinding = new CommandBinding(PivotGridCommands.CollapseRow, new ExecutedRoutedEventHandler(OnCollapseRowCommandExecuted));
            CommandBinding collapseColumnCommandBinding = new CommandBinding(PivotGridCommands.CollapseColumn, new ExecutedRoutedEventHandler(OnCollapseColumnCommandExecuted));

            CommandManager.RegisterClassCommandBinding(typeof(PivotGridControl), refreshCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof(PivotGridControl), resetPivotDataCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof(PivotGridControl), serializeCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof(PivotGridControl), deserializeCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof(PivotGridControl), populateDefaultFieldsCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof(PivotGridControl), expandAllCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof(PivotGridControl), expandRowCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof(PivotGridControl), expandColumnCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof(PivotGridControl), collapseAllCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof(PivotGridControl), collapseRowCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof(PivotGridControl), collapseColumnCommandBinding);

        }

#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="PivotGridControl"/> class.
        /// </summary>
        public PivotGridControl()
        {
#if !SILVERLIGHT
            EnvironmentTest.ValidateLicense(typeof(PivotGridControl));
#endif

#if SILVERLIGHT
            DefaultStyleKey = typeof(PivotGridControl);
            this.ConditionalFormats = new ObservableCollection<PivotGridDataConditionalFormat>();
#endif
            //// Initializing the collection variables
            this._PivotFields = new ObservableCollection<PivotItem>();
            this.allowedFields = new List<FieldInfo>();
            this.PivotEngine = new PivotEngine();
            this.FilterItems = new ObservableCollection<FilterItemsCollection>();
            this.SizeChanged += new SizeChangedEventHandler(PivotGridControl_SizeChanged);

            //Clearing the previous instance value in order to use this in current instance
            this.PivotColumns = new ObservableCollection<PivotItem>();
            this.PivotRows = new ObservableCollection<PivotItem>();
            this.PivotCalculations = new ObservableCollection<PivotComputationInfo>();
            this.Filters = new ObservableCollection<FilterExpression>();
#if !SILVERLIGHT
            this.ConditionalFormats = new FreezableCollection<PivotGridDataConditionalFormat>();
        
#endif

#if SILVERLIGHT
            this.Unloaded += new RoutedEventHandler(PivotGridControl_Unloaded);
#endif
#if !SILVERLIGHT
            CommandBindings.Add(new CommandBinding(PivotGridCommands.ShowPivotValueChooser, ShowPivotValueChooserExecuted, ShowPivotValueChooserCanExecute));
#endif
        }
#if SILVERLIGHT
        void PivotGridControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.ShowFieldList)
                this.ShowFieldList = false;
        }
#endif

        void PivotGridControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        //  CheckScrollBarVisibility();
        }

        internal void CheckScrollBarVisibility()
        {
            if (this.FreezeHeaders && !this.ShowGroupingBar && this.InternalGrid != null && this.PivotEngine.PivotValues != null)
            {
                var m_headerHeightWidth = 0.0;
                for (int i = 0; i < this.PivotRows.Count; i++)
                    m_headerHeightWidth = m_headerHeightWidth + this.InternalGrid.ColumnWidths[i];
                if (this.ActualWidth <= m_headerHeightWidth || this.ActualWidth >= this.InternalGrid.ColumnWidths.TotalExtent)
                    this.GridScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                else
                    this.GridScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
              
                m_headerHeightWidth = 0.0;
                for (int i = 0; i < this.PivotColumns.Count; i++)
                    m_headerHeightWidth = m_headerHeightWidth + this.InternalGrid.RowHeights[i];
                if (this.ActualHeight <= m_headerHeightWidth || this.ActualHeight >= this.InternalGrid.RowHeights.TotalExtent)
                    this.GridScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                else
                    this.GridScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
            else if (this.FreezeHeaders && this.ShowGroupingBar && this.GroupingBar != null)
            {
                var m_headerHeightWidth = 0.0;
                if (this.GroupingBar.RowHeaderArea != null && this.ActualWidth <= this.GroupingBar.GetColumnHeaderWidth())
                    this.GridScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                else
                    this.GridScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

                for (int i = 0; i < this.PivotColumns.Count; i++)
                    m_headerHeightWidth = m_headerHeightWidth + this.InternalGrid.RowHeights[i];
                if (this.GroupingBar.ColumnHeaderArea != null && this.ActualHeight <= (m_headerHeightWidth + this.GroupingBar.ActualHeight))
                    this.GridScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                else
                    this.GridScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
        }

        internal void PivotGridReCalculateSize()
        {
            if (this.InternalGrid != null)
            {
                if (this.GroupingBar != null)
                    this.Height = this.InternalGrid.RowHeights.HeaderLineCount > 1 ? this.InternalGrid.RowHeights.TotalExtent + this.InternalGrid.RowHeights.DefaultLineSize + this.GroupingBar.ActualHeight :
                            this.InternalGrid.RowHeights.TotalExtent + this.GroupingBar.ActualHeight + 3.0;
                else
                    this.Height = this.InternalGrid.RowHeights.HeaderLineCount > 1 ? this.InternalGrid.RowHeights.TotalExtent + this.InternalGrid.RowHeights.DefaultLineSize :
                            this.InternalGrid.RowHeights.TotalExtent + 3.0;
                if (this.GroupingBar == null)
                    this.VerticalAlignment = VerticalAlignment.Top;
            }
        }

        #endregion

#if !SILVERLIGHT
        #region [CommandBinding Methods]
        /// <summary>
        /// Can Execute the PivotValueChooser visibility(show/hide) operation for the given command
        /// </summary>
        /// <param name="sender">The PivotValueChooser</param>
        /// <param name="e">The event argument</param>
        public void ShowPivotValueChooserCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }
        /// <summary>
        /// Executes the PivotValueChooser visibility(show/hide) operation for the given command
        /// </summary>
        /// <param name="sender">The PivotValueChooser</param>
        /// <param name="e">The event argument</param>
        public void ShowPivotValueChooserExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (!this.ShowPivotValueChooser)
                this.ShowPivotValueChooser = true;
        }

        
        #endregion


        #region Static Method
        /// <summary>
        /// Calls the Refresh method of PivotGridControl
        /// </summary>
        /// <param name="target">PivotGridControl</param>
        /// <param name="args">ExecutedRoutedEventArgs</param>
        static void OnRefreshCommand(object target, ExecutedRoutedEventArgs args)
        {
            PivotGridControl pivotGrid = target as PivotGridControl;
            if (pivotGrid != null)
            {
                pivotGrid.Refresh();
            }
        }

        /// <summary>
        /// Calls the ResetPivotData method of PivotGridControl
        /// </summary>
        /// <param name="target">PivotGridControl</param>
        /// <param name="args">ExecutedRoutedEventArgs</param>
        static void OnResetPivotDataCommand(object target, ExecutedRoutedEventArgs args)
        {
            PivotGridControl pivotGrid = target as PivotGridControl;
            if (pivotGrid != null)
            {
                pivotGrid.ResetPivotData();
            }
        }

        /// <summary>
        /// Calls the Serialize method of PivotGridControl
        /// </summary>
        /// <param name="target">PivotGridControl</param>
        /// <param name="args">ExecutedRoutedEventArgs</param>
        static void OnSerializeCommand(object target, ExecutedRoutedEventArgs args)
        {
            PivotGridControl pivotGrid = target as PivotGridControl;
            if (pivotGrid != null)
            {
                pivotGrid.Serialize();
            }
        }

        /// <summary>
        /// Calls the Deserialize method of PivotGridControl
        /// </summary>
        /// <param name="target">PivotGridControl</param>
        /// <param name="args">ExecutedRoutedEventArgs</param>
        static void OnDeserializeCommand(object target, ExecutedRoutedEventArgs args)
        {
            PivotGridControl pivotGrid = target as PivotGridControl;
            if (pivotGrid != null)
            {
                pivotGrid.Deserialize();
            }
        }

        /// <summary>
        /// Called when [populate default fields command is activated].
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        static void OnPopulateDefaultFieldsCommand(object target, ExecutedRoutedEventArgs args)
        {
            PivotGridControl pivotGrid = target as PivotGridControl;
            if (pivotGrid != null)
            {
                pivotGrid.PopulateDefaultPropertyFields();
            }
        }

        static void OnExpandAllCommandExecuted(object target, ExecutedRoutedEventArgs args)
        {
            PivotGridControl pivotGrid = target as PivotGridControl;
            if (pivotGrid != null)
            {
                pivotGrid.ExpandAllGroup();
            }
        }

        static void OnCollapseAllCommandExecuted(object target, ExecutedRoutedEventArgs args)
        {
            PivotGridControl pivotGrid = target as PivotGridControl;
            if (pivotGrid != null)
            {
                pivotGrid.CollapseAllGroup();
            }
        }

        

        static void OnCollapseRowCommandExecuted(object target, ExecutedRoutedEventArgs args)
        {
            PivotGridControl pivotGrid = target as PivotGridControl;
            if (pivotGrid != null)
            {
                if (args.Parameter is IList)
                {
                    pivotGrid.CollapseRow(args.Parameter as List<string>);
                }
                else if (args.Parameter is string)
                {
                    pivotGrid.CollapseRow(args.Parameter.ToString());
                }
            }
        }

        static void OnCollapseColumnCommandExecuted(object target, ExecutedRoutedEventArgs args)
        {
            PivotGridControl pivotGrid = target as PivotGridControl;
            if (pivotGrid != null)
            {
                if (args.Parameter is IList)
                {
                    pivotGrid.CollapseColumn(args.Parameter as List<string>);
                }
                else if (args.Parameter is string)
                {
                    pivotGrid.CollapseColumn(args.Parameter.ToString());
                }
            }
        }

        static void OnExpandColumnCommandExecuted(object target, ExecutedRoutedEventArgs args)
        {
            PivotGridControl pivotGrid = target as PivotGridControl;
            if (pivotGrid != null)
            {
                if (args.Parameter is IList)
                {
                    pivotGrid.ExpandColumn(args.Parameter as List<string>);
                }
                else if (args.Parameter is string)
                {
                    pivotGrid.ExpandColumn(args.Parameter.ToString());
                }
            }
        }

        static void OnExpandRowCommandExecuted(object target, ExecutedRoutedEventArgs args)
        {
            PivotGridControl pivotGrid = target as PivotGridControl;
            if (pivotGrid != null)
            {
                if (args.Parameter is IList)
                {
                    pivotGrid.ExpandRow(args.Parameter as List<string>);
                }
                else if (args.Parameter is string)
                {
                    pivotGrid.ExpandRow(args.Parameter.ToString());
                }
            }
        }

        #endregion
#endif

        #region [ Dependency Property declaration ]
#if SILVERLIGHT
        public static bool IsFocusIn(DependencyObject element)
        {
            DependencyObject focusedElement = FocusManager.GetFocusedElement() as DependencyObject;
            if (focusedElement != null)
            {
                return true;
            }
            return false;
        }
#endif
#if !SILVERLIGHT
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.FieldListBorderBrush"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.FieldListBorderBrush"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty FieldListBorderBrushProeprty = DependencyProperty.Register("FieldListBorderBrush", typeof(Brush), typeof(PivotGridControl), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.GroupingBarBackground"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.GroupingBarBackground"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty GroupingBarBackgroundProeprty = DependencyProperty.Register("GroupingBarBackground", typeof(Brush), typeof(PivotGridControl), new UIPropertyMetadata(null));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.GroupingButtonCheckedBackground"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.GroupingButtonCheckedBackground"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty GroupingButtonCheckedBackgroundProperty =
#if !SILVERLIGHT

            DependencyProperty.Register("GroupingButtonCheckedBackground", typeof(Brush), typeof(PivotGridControl), new UIPropertyMetadata(null));
#else

 DependencyProperty.Register("GroupingButtonCheckedBackground", typeof(Brush), typeof(PivotGridControl), new PropertyMetadata(null));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.GroupingButtonHoverForeground"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.GroupingButtonHoverForeground"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty GroupingButtonHoverForegroundProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("GroupingButtonHoverForeground", typeof(Brush), typeof(PivotGridControl), new UIPropertyMetadata(null));
#else
 DependencyProperty.Register("GroupingButtonHoverForeground", typeof(Brush), typeof(PivotGridControl), new PropertyMetadata(null));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.GroupingButtonCheckedForeground"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.GroupingButtonCheckedForeground"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty GroupingButtonCheckedForegroundProperty =

#if !SILVERLIGHT
            DependencyProperty.Register("GroupingButtonCheckedForeground", typeof(Brush), typeof(PivotGridControl), new UIPropertyMetadata(null));
#else
 DependencyProperty.Register("GroupingButtonCheckedForeground", typeof(Brush), typeof(PivotGridControl), new PropertyMetadata(null));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.GroupingButtonHoverBorderBrush"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.GroupingButtonHoverBorderBrush"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty GroupingButtonHoverBorderBrushProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("GroupingButtonHoverBorderBrush", typeof(Brush), typeof(PivotGridControl), new UIPropertyMetadata(null));

#else
 DependencyProperty.Register("GroupingButtonHoverBorderBrush", typeof(Brush), typeof(PivotGridControl), new PropertyMetadata(null));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.GroupingButtonHoverBackgroundBrush"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.GroupingButtonHoverBackgroundBrush"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty GroupingButtonHoverBackgroundBrushProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("GroupingButtonHoverBackgroundBrush", typeof(Brush), typeof(PivotGridControl), new UIPropertyMetadata(null));
#else

 DependencyProperty.Register("GroupingButtonHoverBackgroundBrush", typeof(Brush), typeof(PivotGridControl), new PropertyMetadata(null));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.GroupingBarItemBorderBrush"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.GroupingBarItemBorderBrush"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty GroupingBarItemBorderBrushProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("GroupingBarItemBorderBrush", typeof(Brush), typeof(PivotGridControl), new UIPropertyMetadata(null));
#else

 DependencyProperty.Register("GroupingBarItemBorderBrush", typeof(Brush), typeof(PivotGridControl), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.GroupingBarItemBackground"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.GroupingBarItemBackground"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty GroupingBarItemBackgroundProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("GroupingBarItemBackground", typeof(Brush), typeof(PivotGridControl), new UIPropertyMetadata(null));
#else
 DependencyProperty.Register("GroupingBarItemBackground", typeof(Brush), typeof(PivotGridControl), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.GroupingBarItemForeground"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.GroupingBarItemForeground"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty GroupingBarItemForegroundProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("GroupingBarItemForeground", typeof(Brush), typeof(PivotGridControl), new UIPropertyMetadata(null));
#else
 DependencyProperty.Register("GroupingBarItemForeground", typeof(Brush), typeof(PivotGridControl), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ItemSource"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ItemSource"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ItemSourceProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("ItemSource", typeof(object), typeof(PivotGridControl), new UIPropertyMetadata(null, OnItemSourceChanged));
#else
 DependencyProperty.Register("ItemSource", typeof(object), typeof(PivotGridControl), new PropertyMetadata(null, OnItemSourceChanged));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.EnableHyperlinkOnMouseHover"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.EnableHyperlinkOnMouseHover"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty HyperlinkBehaviorProperty =
#if !SILVERLIGHT
        DependencyProperty.Register("EnableHyperlinkOnMouseHover", typeof(bool), typeof(PivotGridControl), new UIPropertyMetadata(true, OnHyperlinkBehaviorChanged));
#else
        DependencyProperty.Register("EnableHyperlinkOnMouseHover", typeof(bool), typeof(PivotGridControl), new PropertyMetadata(true));
#endif




        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.CurrentCellBorder"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.CurrentCellBorder"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty CurrentCellBorderProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("CurrentCellBorder", typeof(Brush), typeof(PivotGridControl), new UIPropertyMetadata(new SolidColorBrush(Colors.Black)));
#else
 DependencyProperty.Register("CurrentCellBorder", typeof(Brush), typeof(PivotGridControl), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
#endif
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ShowCalculationsAsColumns"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ShowCalculationsAsColumns"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ShowCalculationsAsColumnsProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("ShowCalculationsAsColumns", typeof(bool), typeof(PivotGridControl), new UIPropertyMetadata(true, OnShowCalculationsAsColumnsChanged));
#else
 DependencyProperty.Register("ShowCalculationsAsColumns", typeof(bool), typeof(PivotGridControl), new PropertyMetadata(true, OnShowCalculationsAsColumnsChanged));
#endif
#if !SILVERLIGHT
        /// <summary>
        /// Update the data in Grid only when the user releases the thumb on scrolling.
        /// Hence it is used to improve the scrolling performance
        /// </summary>
        public bool EnableDefferedScrolling
        {
            get { return (bool)GetValue(EnableDefferedScrollingProperty); }
            set { SetValue(EnableDefferedScrollingProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.EnableDefferedScrolling"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.EnableDefferedScrolling"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty EnableDefferedScrollingProperty =
                 DependencyProperty.Register("EnableDefferedScrolling", typeof(bool), typeof(PivotGridControl), new UIPropertyMetadata(false,OnEnableDefferedScrollingChanged ));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ResizeToFit"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ResizeToFit"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ResizePivotGridToFitProperty =
#if !SILVERLIGHT         
 DependencyProperty.Register("ResizeToFit", typeof(bool), typeof(PivotGridControl), new UIPropertyMetadata(false));
#else
 DependencyProperty.Register("ResizeToFit", typeof(bool), typeof(PivotGridControl), new PropertyMetadata(false));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.AllowRowHeaderAreaAutoSizing"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.AllowRowHeaderAreaAutoSizing"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty AllowRowHeaderAreaAutoSizingProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("AllowRowHeaderAreaAutoSizing", typeof(bool), typeof(PivotGridControl), new UIPropertyMetadata(true, OnAllowRowHeaderAreaAutoSizingChanged));
#else
 DependencyProperty.Register("AllowRowHeaderAreaAutoSizing", typeof(bool), typeof(PivotGridControl), new PropertyMetadata(true, OnAllowRowHeaderAreaAutoSizingChanged));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ShowDisabledGroupBackground"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ShowDisabledGroupBackground"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ShowDisabledGroupBackgroundProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("ShowDisabledGroupBorder", typeof(bool), typeof(PivotGridControl), new UIPropertyMetadata(false));
#else
 DependencyProperty.Register("ShowDisabledGroupBackground", typeof(bool), typeof(PivotGridControl), new PropertyMetadata(false, OnShowDisabledGroupBackground));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.FreezeHeaders"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.FreezeHeaders"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty FreezeHeadersProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("FreezeHeaders", typeof(bool), typeof(PivotGridControl), new UIPropertyMetadata(true, OnFreezeHeadersChanged));
#else
 DependencyProperty.Register("FreezeHeaders", typeof(bool), typeof(PivotGridControl), new PropertyMetadata(true, OnFreezeHeadersChanged));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.DeferLayoutUpdate"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.DeferLayoutUpdate"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty DeferLayoutUpdateProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("DeferLayoutUpdate", typeof(bool), typeof(PivotGridControl), new UIPropertyMetadata(false));
#else
 DependencyProperty.Register("DeferLayoutUpdate", typeof(bool), typeof(PivotGridControl), new PropertyMetadata(false));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ExpanderStyle"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ExpanderStyle"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ExpanderStyleProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("ExpanderStyle", typeof(System.Windows.Style), typeof(PivotGridControl), new FrameworkPropertyMetadata(null, OnStyleChanged));
#else
 DependencyProperty.Register("ExpanderStyle", typeof(System.Windows.Style), typeof(PivotGridControl), new PropertyMetadata(null, OnStyleChanged));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.RowHeaderCellStyle"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.RowHeaderCellStyle"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty RowHeaderCellStyleProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("RowHeaderCellStyle", typeof(PivotGridCellStyle), typeof(PivotGridControl), new UIPropertyMetadata(null, OnStyleChanged));
#else
 DependencyProperty.Register("RowHeaderCellStyle", typeof(PivotGridCellStyle), typeof(PivotGridControl), new PropertyMetadata(null, OnStyleChanged));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ColumnHeaderCellStyle"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ColumnHeaderCellStyle"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ColumnHeaderCellStyleProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("ColumnHeaderCellStyle", typeof(PivotGridCellStyle), typeof(PivotGridControl), new UIPropertyMetadata(null, OnStyleChanged));
#else
 DependencyProperty.Register("ColumnHeaderCellStyle", typeof(PivotGridCellStyle), typeof(PivotGridControl), new PropertyMetadata(null, OnStyleChanged));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.SummaryHeaderStyle"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.SummaryHeaderStyle"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty SummaryHeaderStyleProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("SummaryHeaderStyle", typeof(PivotGridCellStyle), typeof(PivotGridControl), new UIPropertyMetadata(null, OnStyleChanged));
#else
 DependencyProperty.Register("SummaryHeaderStyle", typeof(PivotGridCellStyle), typeof(PivotGridControl), new PropertyMetadata(null, OnStyleChanged));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.SummaryCellStyle"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.SummaryCellStyle"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty SummaryCellStyleProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("SummaryCellStyle", typeof(PivotGridCellStyle), typeof(PivotGridControl), new UIPropertyMetadata(null, OnStyleChanged));
#else
 DependencyProperty.Register("SummaryCellStyle", typeof(PivotGridCellStyle), typeof(PivotGridControl), new PropertyMetadata(null, OnStyleChanged));
#endif


       

        // Using a DependencyProperty as the backing store for SortOption.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.SortOption"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.SortOption"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty SortOptionProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("SortOption", typeof(PivotSortOption), typeof(PivotGridControl), new UIPropertyMetadata(PivotSortOption.None));
#else
 DependencyProperty.Register("SortOption", typeof(PivotSortOption), typeof(PivotGridControl), new PropertyMetadata(PivotSortOption.None));

#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ValueCellStyle"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ValueCellStyle"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ValueCellStyleProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("ValueCellStyle", typeof(PivotGridCellStyle), typeof(PivotGridControl), new UIPropertyMetadata(null, OnStyleChanged));
#else
 DependencyProperty.Register("ValueCellStyle", typeof(PivotGridCellStyle), typeof(PivotGridControl), new PropertyMetadata(null, OnStyleChanged));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.PivotEngine"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.PivotEngine"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty PivotEngineProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("PivotEngine", typeof(PivotEngine), typeof(PivotGridControl), new UIPropertyMetadata(null, OnPivotEngineChanged));
#else
 DependencyProperty.Register("PivotEngine", typeof(PivotEngine), typeof(PivotGridControl), new PropertyMetadata(null, OnPivotEngineChanged));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ConditionalFormats"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ConditionalFormats"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ConditionalFormatsProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("ConditionalFormats", typeof(FreezableCollection<PivotGridDataConditionalFormat>), typeof(PivotGridControl));
#else
 DependencyProperty.Register("ConditionalFormats", typeof(ObservableCollection<PivotGridDataConditionalFormat>), typeof(PivotGridControl), new PropertyMetadata(null));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.GridLineStroke"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.GridLineStroke"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty GridLineStrokeProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("GridLineStroke", typeof(Brush), typeof(PivotGridControl), new UIPropertyMetadata(null, OnGridLineStrokeChanged));
#else
 DependencyProperty.Register("GridLineStroke", typeof(Brush), typeof(PivotGridControl), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnGridLineStrokeChanged));// Common.GetColorFromHexaDecimal("#CFCFCF")));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.GridOuterBorderBrush"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.GridOuterBorderBrush"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty GridOuterBorderBrushProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("GridOuterBorderBrush", typeof(Brush), typeof(PivotGridControl), new UIPropertyMetadata(new SolidColorBrush(Colors.Black)));
#else
 DependencyProperty.Register("GridOuterBorderBrush", typeof(Brush), typeof(PivotGridControl), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ShowGrandTotals"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ShowGrandTotals"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ShowGrandTotalsProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("ShowGrandTotals", typeof(bool), typeof(PivotGridControl), new UIPropertyMetadata(true, OnShowGrandTotalsChanged));
#else
 DependencyProperty.Register("ShowGrandTotals", typeof(bool), typeof(PivotGridControl), new PropertyMetadata(true, OnShowGrandTotalsChanged));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ShowSubTotals"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ShowSubTotals"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ShowSubTotalsProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("ShowSubTotals", typeof(bool), typeof(PivotGridControl), new UIPropertyMetadata(true, OnShowSubTotalsChanged));
#else
 DependencyProperty.Register("ShowSubTotals", typeof(bool), typeof(PivotGridControl), new PropertyMetadata(true, OnShowSubTotalsChanged));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.AllowSelection"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.AllowSelection"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty AllowSelectionProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("AllowSelection", typeof(bool), typeof(PivotGridControl), new UIPropertyMetadata(false, OnAllowSelectionChanged));
#else
 DependencyProperty.Register("AllowSelection", typeof(bool), typeof(PivotGridControl), new PropertyMetadata(false, OnAllowSelectionChanged));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.SelectedItems"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.SelectedItems"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty SelectedItemsProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("SelectedItems", typeof(SelectedItems), typeof(PivotGridControl), new UIPropertyMetadata(null, OnSelectedItemsChanged));
#else
 DependencyProperty.Register("SelectedItems", typeof(SelectedItems), typeof(PivotGridControl), new PropertyMetadata(null, OnSelectedItemsChanged));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.AllowResizeColumns"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.AllowResizeColumns"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty AllowResizeColumnsProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("AllowResizeColumns", typeof(bool), typeof(PivotGridControl), new UIPropertyMetadata(false));
#else
 DependencyProperty.Register("AllowResizeColumns", typeof(bool), typeof(PivotGridControl), new PropertyMetadata(false));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.AllowResizeRows"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.AllowResizeRows"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty AllowResizeRowsProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("AllowResizeRows", typeof(bool), typeof(PivotGridControl), new UIPropertyMetadata(false));
#else
 DependencyProperty.Register("AllowResizeRows", typeof(bool), typeof(PivotGridControl), new PropertyMetadata(false));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.IsDynamicData"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.IsDynamicData"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty IsDynamicDataProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("IsDynamicData", typeof(bool), typeof(PivotGridControl), new UIPropertyMetadata(false));
#else
 DependencyProperty.Register("IsDynamicData", typeof(bool), typeof(PivotGridControl), new PropertyMetadata(false));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.IsDataDynamic"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.IsDataDynamic"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty IsDataDynamicProperty =
#if !SILVERLIGHT
        DependencyProperty.Register("IsDataDynamic", typeof(bool), typeof(PivotGridControl), new UIPropertyMetadata(false, OnDataDynamicChanged));
#else
        DependencyProperty.Register("IsDataDynamic", typeof(bool), typeof(PivotGridControl), new PropertyMetadata(false, OnDataDynamicChanged));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ShowGroupingBar"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ShowGroupingBar"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ShowGroupingBarProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("ShowGroupingBar", typeof(bool), typeof(PivotGridControl), new UIPropertyMetadata(true, ShowGroupingBarPropertyChanged));
#else
 DependencyProperty.Register("ShowGroupingBar", typeof(bool), typeof(PivotGridControl), new PropertyMetadata(true, ShowGroupingBarPropertyChanged));
#endif

       
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.GridLayout"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.GridLayout"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty GridLayoutProperty =
            DependencyProperty.Register("GridLayout", typeof(GridLayout), typeof(PivotGridControl), new PropertyMetadata(GridLayout.Normal, OnGridLayoutChanged));

        
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.GroupingBar"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.GroupingBar"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty GroupingBarProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("GroupingBar", typeof(PivotGridGroupingBar), typeof(PivotGridControl), new UIPropertyMetadata(null));//, ShowGroupingBarPropertyChanged));
#else
 DependencyProperty.Register("GroupingBar", typeof(PivotGridGroupingBar), typeof(PivotGridControl), new PropertyMetadata(null));//, ShowGroupingBarPropertyChanged));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ShowFieldList"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ShowFieldList"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ShowFieldListProperty =
#if !SILVERLIGHT
             DependencyProperty.Register("ShowFieldList", typeof(bool), typeof(PivotGridControl), new UIPropertyMetadata(false, ShowFieldListPropertyChanged));//, ShowGroupingBarPropertyChanged));
#else
 DependencyProperty.Register("ShowFieldList", typeof(bool), typeof(PivotGridControl), new PropertyMetadata(new PropertyChangedCallback(
                (dependencyObject, args) =>
                {
                    PivotGridControl pivotGridControl = dependencyObject as PivotGridControl;
                    if (pivotGridControl != null && !DesignerProperties.GetIsInDesignMode(pivotGridControl))
                    {
                        if (pivotGridControl.GroupingBar != null)
                            pivotGridControl.GroupingBar.ShowFieldList = (bool)args.NewValue;
                    }
                })));
#endif
#if !SILVERLIGHT
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ResizeColumnsToFit"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ResizeColumnsToFit"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ResizeColumnsToFitProperty =
            DependencyProperty.Register("ResizeColumnsToFit", typeof(bool), typeof(PivotGridControl), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ResizeRowsToFit"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ResizeRowsToFit"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ResizeRowsToFitProperty =
            DependencyProperty.Register("ResizeRowsToFit", typeof(bool), typeof(PivotGridControl), new UIPropertyMetadata(true));
#endif
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.EnableValueEditing"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.EnableValueEditing"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty EnableValueEditingProperty =
             DependencyProperty.Register("EnableValueEditing", typeof(bool), typeof(PivotGridControl), new PropertyMetadata(false, EnableValueEditingPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.EnableSpecificColumnEditing"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.EnableSpecificColumnEditing"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty EnableSpecificColumnEditingProperty =
             DependencyProperty.Register("EnableSpecificColumnEditing", typeof(bool), typeof(PivotGridControl), new PropertyMetadata(false, EnableValueEditingPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.EnableUpdating"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.EnableUpdating"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty EnableUpdatingProperty =
             DependencyProperty.Register("EnableUpdating", typeof(bool), typeof(PivotGridControl), new PropertyMetadata(false, EnableUpdatingPropertyChanged));


        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.AutoSizeOption"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.AutoSizeOption"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty AutoSizeOptionProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("AutoSizeOption", typeof(GridAutoSizeOption), typeof(PivotGridControl), new UIPropertyMetadata(GridAutoSizeOption.All));
#else
 DependencyProperty.Register("AutoSizeOption", typeof(GridAutoSizeOption), typeof(PivotGridControl), new PropertyMetadata(GridAutoSizeOption.All));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ToolTipEnabled"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ToolTipEnabled"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ToolTipEnabledProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("ToolTipEnabled", typeof(bool), typeof(PivotGridControl), new UIPropertyMetadata(false,ToolTipEnabledPropertyChanged));
#else
 DependencyProperty.Register("ToolTipEnabled", typeof(bool), typeof(PivotGridControl), new PropertyMetadata(false, ToolTipEnabledPropertyChanged));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.LoadInBackground"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.LoadInBackground"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty LoadInBackgroundProperty =
#if !SILVERLIGHT
     DependencyProperty.Register("LoadInBackground", typeof(bool), typeof(PivotGridControl), new UIPropertyMetadata(false,OnLoadInBackgroundChanged));
#else
     DependencyProperty.Register("LoadInBackground", typeof(bool), typeof(PivotGridControl), new PropertyMetadata(false,OnLoadInBackgroundChanged));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.CustomToolTipTemplateKey"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.CustomToolTipTemplateKey"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty CustomToolTipTemplateKeyProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("CustomToolTipTemplateKey", typeof(string), typeof(PivotGridControl), new UIPropertyMetadata(null, CustomToolTipTemplateKeyPropertyChanged));
#else
 DependencyProperty.Register("CustomToolTipTemplateKey", typeof(string), typeof(PivotGridControl), new PropertyMetadata(null, CustomToolTipTemplateKeyPropertyChanged));
#endif

        /// <summary>
        /// Visual Style Dependency Property
        /// </summary>

        public static readonly DependencyProperty VisualStyleProperty =
#if SILVERLIGHT
 DependencyProperty.Register("VisualStyle", typeof(Syncfusion.Windows.Controls.Theming.VisualStyle), typeof(PivotGridControl), new PropertyMetadata(Syncfusion.Windows.Controls.Theming.VisualStyle.Default,
     (dependencyObject, args) =>
     {
         PivotGridControl pivotGrid = dependencyObject as PivotGridControl;
         ResourceDictionary resource = new ResourceDictionary();
         if (pivotGrid != null)
         {
             SkinManager.SetVisualStyle(dependencyObject,(Syncfusion.Windows.Controls.Theming.VisualStyle)args.NewValue);
         }
     }));
#else
 DependencyProperty.Register("VisualStyle", typeof(PivotGridVisualStyle), typeof(PivotGridControl), new UIPropertyMetadata(PivotGridVisualStyle.Default,
     (dependencyObject, args) =>
     {
         PivotGridControl pivotGrid = dependencyObject as PivotGridControl;
         if (pivotGrid != null)
         {
             SkinStorage.SetVisualStyle(dependencyObject, args.NewValue.ToString());
         }
     }));
#endif

        /// <summary>
        /// SelectedCell Dependency Property
        /// </summary>
        public static readonly DependencyProperty SelectedCellProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("SelectedCell", typeof(PivotCellInfo), typeof(PivotGridControl), new UIPropertyMetadata(null));
#else
 DependencyProperty.Register("SelectedCell", typeof(PivotCellInfo), typeof(PivotGridControl), new PropertyMetadata(null));
#endif


        /// <summary>
        /// Load with default properties which is available in items source.
        /// </summary>
        public static readonly DependencyProperty LoadWithDefaultPropertyFieldsProperty =
#if !SILVERLIGHT
                    DependencyProperty.Register("LoadWithDefaultPropertyFields", typeof(bool), typeof(PivotGridControl), new UIPropertyMetadata(false, OnPopulateDefaultPropertyFieldsChanged)); 
#else
                    DependencyProperty.Register("LoadWithDefaultPropertyFields", typeof(bool), typeof(PivotGridControl), new PropertyMetadata(false, OnPopulateDefaultPropertyFieldsChanged));
#endif

#if SILVERLIGHT
        /// <summary>
        /// Gets or sets the ShowSelectedFieldsOnly.
        /// </summary>
        public bool ShowSelectedFieldsOnly
        {
            get { return (bool)GetValue(ShowSelectedFieldsOnlyProperty); }
            set { SetValue(ShowSelectedFieldsOnlyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowSelectedFieldsOnly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowSelectedFieldsOnlyProperty =
            DependencyProperty.Register("ShowSelectedFieldsOnly", typeof(bool), typeof(PivotGridControl), new PropertyMetadata(
                (dependencyObject, args) => 
                {
                    PivotGridControl pivotGridControl = dependencyObject as PivotGridControl;
                    if (pivotGridControl != null)
                    {
                        if (pivotGridControl.GroupingBar != null)
                            pivotGridControl.GroupingBar.ShowSelectedFieldsOnly = (bool)args.NewValue;
                    }
                }));


        /// <summary>
        /// Gets or Sets the SelectedFields
        /// </summary>
        public IEnumerable SelectedFields
        {
            get { return (IEnumerable<string>)GetValue(SelectedFieldsProperty); }
            set { SetValue(SelectedFieldsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedFields.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedFieldsProperty =
            DependencyProperty.Register("SelectedFields", typeof(IEnumerable), typeof(PivotGridControl), new PropertyMetadata(null));
#endif
        
        #endregion

        #region [ Properties ]

        internal bool RefreshFromGroupingBar { get; set; }

        internal bool UpdateGridLayout { get; set; }
        /// <summary>
        /// Gets or sets whether can refresh the PivotGrid or not
        /// </summary>
        public bool IgnoreRefesh { get; set; }

        internal Border GridOuterBorder { get; set; }

        internal PivotGridSerializer GridSerializer { get; set; }

        /// <summary>
        /// Gets or sets whether the GrandTotal row is always visible at the bottom of the PivotGridControl. The default
        /// setting is false.
        /// </summary>
        public bool GrandTotalRowAlwaysVisible
        {
            get { return grandTotalRowAlwaysVisible; }
            set 
            { 
                grandTotalRowAlwaysVisible = value;
                if (this.PivotEngine.RowCount > 1 && this.InternalGrid != null
                    && this.InternalGrid.Model.RowCount > 1 && this.InternalGrid.Model.FooterRows == 0)
                {
                    this.InternalGrid.Model.FooterRows = 1;
                }
            }
        }
#if !SILVERLIGHT

        private bool rowPivotsOnly;
        /// <summary>
        /// Gets or sets whether this PivotGridControl can pivot both rows and columns, or only rows. 
        /// The default setting to allow pivoting both rows and columns. 
        /// <remarks>
        /// Setting this property to true will 
        ///   1) automatically set ShowGroupingBar to false. 
        ///   2) Replace the TopLeft covered cell that normally appears in a PivotGrid with individual
        ///      header cells that appear the same as Calculation header cells.
        /// </remarks>
        /// </summary>
        public bool RowPivotsOnly 
        { 
            get { return rowPivotsOnly; }
            set
            {
                rowPivotsOnly = value;
                if (rowPivotsOnly)
                {
                    this.SetValue(PivotGridControl.ShowGroupingBarProperty, false);
                    this.SetValue(PivotGridControl.HyperlinkBehaviorProperty, false);
                }
                if (PivotEngine != null)
                {
                    PivotEngine.RowPivotsOnly = rowPivotsOnly;
                }
            }
        }
#endif

        /// <summary>
        /// Gets or sets the value indicating the number of columns to be resized.        
        /// </summary>
        public int AutoSizeColumnCount
        {
            get
            {
                return m_AutoSizeColumnCount;
            }
            set
            {
                m_AutoSizeColumnCount = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the grid cells can have ToolTip
        /// </summary>
        public bool ToolTipEnabled
        {
            get { return (bool)GetValue(ToolTipEnabledProperty); }
            set { SetValue(ToolTipEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets Custom ToolTip template Key
        /// </summary>
        public string CustomToolTipTemplateKey
        {
            get { return (string)GetValue(CustomToolTipTemplateKeyProperty); }
            set { SetValue(CustomToolTipTemplateKeyProperty, value); }
        }


        /// <summary>
        /// Gets or Sets the Visual Style property of PivotGridControl
        /// </summary>
#if SILVERLIGHT
        public Syncfusion.Windows.Controls.Theming.VisualStyle VisualStyle
        {
            get { return (Syncfusion.Windows.Controls.Theming.VisualStyle)GetValue(VisualStyleProperty); }
            set { SetValue(VisualStyleProperty, value); }
        }
#else
        public PivotGridVisualStyle VisualStyle
        {
            get { return (PivotGridVisualStyle)GetValue(VisualStyleProperty); }
            set { SetValue(VisualStyleProperty, value); }
        }
#endif

        /// <summary>
        /// Gets or Sets the SelectedCell of PivotGridControl
        /// </summary>
        [Browsable(false)]
        public PivotCellInfo SelectedCell
        {
            get { return (PivotCellInfo)GetValue(SelectedCellProperty); }
            set { SetValue(SelectedCellProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show empty value cell if summary value has null value.
        /// By default true.
        /// </summary>    
        public bool ShowEmptyCells
        {
            get { return (bool)GetValue(ShowEmptyCellsProperty); }
            set { SetValue(ShowEmptyCellsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ShowEmptyCells"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ShowEmptyCells"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ShowEmptyCellsProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("ShowEmptyCells", typeof(bool), typeof(PivotGridControl), new UIPropertyMetadata(true, OnShowEmptyCellsPropertyChanged));
#else
 DependencyProperty.Register("ShowEmptyCells", typeof(bool), typeof(PivotGridControl), new PropertyMetadata(true, OnShowEmptyCellsPropertyChanged));
#endif

        private static void OnShowEmptyCellsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PivotGridControl pivotGrid = d as PivotGridControl;
            if (pivotGrid != null && pivotGrid.PivotEngine != null)
            {
                pivotGrid.PivotEngine.ShowEmptyCells = pivotGrid.ShowEmptyCells;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to maintain/show collapsed cells when pivot schema getting changed.
        /// </summary>        
        public bool StatePersistenceEnabled
        {
            get { return (bool)GetValue(StatePersistenceEnabledProperty); }
            set { SetValue(StatePersistenceEnabledProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.StatePersistenceEnabled"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.StatePersistenceEnabled"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty StatePersistenceEnabledProperty =
#if !SILVERLIGHT
          DependencyProperty.Register("StatePersistenceEnabled", typeof(bool), typeof(PivotGridControl), new UIPropertyMetadata(false,
                (sender, args) =>
                {
                    PivotGridControl pivotGrid = sender as PivotGridControl;
                    if (pivotGrid.InternalGrid != null)
                    {
                        pivotGrid.InternalGrid.StatePersistenceEnabled = (bool)args.NewValue;
                    }
                }));
#else
 DependencyProperty.Register("StatePersistenceEnabled", typeof(bool), typeof(PivotGridControl), new PropertyMetadata(false,
                (sender, args) =>
                {
                    PivotGridControl pivotGrid = sender as PivotGridControl;
                    if (pivotGrid.InternalGrid != null)
                    {
                        pivotGrid.InternalGrid.StatePersistenceEnabled = (bool)args.NewValue;
                    }
                }));
#endif

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the grid scroll viewer.
        /// </summary>
        /// <value>The grid scroll viewer.</value>
        internal ScrollViewer GridScrollViewer { get; set; }
        internal BusyIndicator BusyIndicator { get; set; }
#else
        internal ScrollableContentViewer GridScrollViewer { get; set; }
        internal BusyIndicator BusyIndicator { get; set; }
        internal Grid BusyIndicatorHolder { get; set; }

        BusyIndicator GetBusyIndicator()
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = Syncfusion.Silverlight.Controls.PivotGrid.Resources.SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "PivotGrid_Loading");
            textBlock.Height = 35;
            textBlock.TextAlignment = TextAlignment.Left;
            textBlock.Width = 280;
            textBlock.TextWrapping = TextWrapping.Wrap;
            return new BusyIndicator
            {
                IsEnabled = true,
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalContentAlignment = System.Windows.VerticalAlignment.Center,
                IsBusy = false,
                Header = "Pivot Grid",
                LoadingDescription = textBlock,
                CloseButtonVisibility = System.Windows.Visibility.Collapsed,
                CancelButtonVisibility = System.Windows.Visibility.Collapsed,
                IsIndeterminate = true,
                OverlayOpacity = 0.5
            };
        }
#endif
#if !SILVERLIGHT

        /// <summary>
        /// Gets or sets a collection of possible PivotCalculations that may appear in the PivotGridControl
        /// when RowPivotsOnly is true.
        /// </summary>
        public ObservableCollection<PivotComputationInfo> PossiblePivotCalculations
        {
            get { return (ObservableCollection<PivotComputationInfo>)GetValue(PossiblePivotCalculationsProperty); }
            set { SetValue(PossiblePivotCalculationsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PossiblePivotCalculations.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.PossiblePivotCalculations"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.PossiblePivotCalculations"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty PossiblePivotCalculationsProperty =
            DependencyProperty.Register("PossiblePivotCalculations", typeof(ObservableCollection<PivotComputationInfo>), typeof(PivotGridControl), new UIPropertyMetadata(new ObservableCollection<PivotComputationInfo>()));


        private ObservableCollection<PivotValueField> localPossibleCalculations = null;
        internal ObservableCollection<PivotValueField> LocalPossibleCalculations 
        { 
            get
            {
                if (localPossibleCalculations == null)
                {
                    localPossibleCalculations = new ObservableCollection<PivotValueField>();

                    PropertyDescriptorCollection propertyDescriptorCollection = ItemProperties;
                    PivotValueField pivotValueField = null;
                    foreach (PropertyDescriptor pd in propertyDescriptorCollection)
                    {
                        localPossibleCalculations.Add(pivotValueField = new PivotValueField
                        {
                            FieldName = pd.Name,
                            FieldHeader = pd.Name,
                            SummaryType = GetPivotTableSummaryType(pd.PropertyType),
                            CalculationType = CalculationType.NoCalculation
                        });
                        pivotValueField.PropertyChanged += new PropertyChangedEventHandler(pivotValueField_PropertyChanged);
                    }
                }
                return localPossibleCalculations;
            }
            set
            {
                localPossibleCalculations = value;
            }
        }

        void pivotValueField_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            PivotValueField pivotTableField = (sender as PivotValueField);
            if (this.LocalPossibleCalculations != null && pivotTableField != null)
            {
                this.InternalGrid.SetValueColumnVisibility(pivotTableField.FieldName, !pivotTableField.IsSelected);

                if (pivotTableField.IsSelected && ColumnFilterPopup.Exclusions != null && ColumnFilterPopup.Exclusions.Count > 0)
                {
                    this.InternalGrid.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.InternalGrid.PivotEngine.UpdateAllSummariesRespectingHiddenRowIndexes();
                        this.InvalidateCells();
                    }), System.Windows.Threading.DispatcherPriority.Render);
                }
                else
                    this.InvalidateCells();
            }
            this.Cursor = Cursors.Arrow;
        }


        internal PivotValueField GetPivotValueFields(PivotComputationInfo pivotComputationInfo, bool IsSelected)
        {
            return new PivotValueField
                            {
                                AllowFilter = pivotComputationInfo.AllowFilter,
                                AllowRunTimeGroupByField = pivotComputationInfo.AllowRunTimeGroupByField,
                                AllowSort = pivotComputationInfo.AllowSort,
                                BaseField = pivotComputationInfo.BaseField,
                                CalculationName = pivotComputationInfo.CalculationName,
                                CalculationType = pivotComputationInfo.CalculationType,
                                DefaultValue = pivotComputationInfo.DefaultValue,
                                Description = pivotComputationInfo.Description,
                                EnableHyperlinks = pivotComputationInfo.EnableHyperlinks,
                                FieldHeader = pivotComputationInfo.FieldHeader,
                                FieldName = pivotComputationInfo.FieldName,
                                Format = pivotComputationInfo.Format,
                                InnerMostComputationsOnly = pivotComputationInfo.InnerMostComputationsOnly,
                                IsSelected = IsSelected,
                                PadString = pivotComputationInfo.PadString,
                                Summary = pivotComputationInfo.Summary,
                                SummaryType = pivotComputationInfo.SummaryType
                            };
        }

        private double pivotValueChooserItemHeight;

        /// <summary>
        /// Gets or sets the height of the items listed in the PivotValueChooser window. The PivotValueChooser 
        /// is only available when RowPivotsOnly is true.
        /// </summary>
        public double PivotValueChooserItemHeight
        {
            get { return pivotValueChooserItemHeight; }
            set { pivotValueChooserItemHeight = value; }
        }

        private double pivotValueChooserItemFontSize = 11;

        /// <summary>
        /// Gets or sets the size of the font used in the PivotValueChooser window. The PivotValueChooser 
        /// is only available when RowPivotsOnly is true.
        /// </summary>
        public double PivotValueChooserItemFontSize
        {
            get { return pivotValueChooserItemFontSize; }
            set { pivotValueChooserItemFontSize = value; }
        }

        /// <summary>
        /// Hides and shows a Computation Value Column chooser dialog that allows you to hide/show/reorder 
        /// value calculations when RowPivotsOnly is true.
        /// The choices displayed in the chooser are obtained from <see cref="PossiblePivotCalculations"/>.
        /// </summary>
        public bool ShowPivotValueChooser
        {
            get { return (bool)GetValue(ShowPivotValueChooserProperty); }
            set { SetValue(ShowPivotValueChooserProperty, value); }
        }

        internal PivotValueChooser FieldChooser { get; set; }

        // Using a DependencyProperty as the backing store for ShowFieldChooser.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ShowPivotValueChooser"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.ShowPivotValueChooser"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ShowPivotValueChooserProperty =
            DependencyProperty.Register("ShowPivotValueChooser", typeof(bool), typeof(PivotGridControl), new UIPropertyMetadata(false, new PropertyChangedCallback((obj, args) =>
            {
                PivotGridControl pivotGrid = obj as PivotGridControl;
                if (pivotGrid != null)
                {
                    if ((bool)args.NewValue)
                    {
                        Window parentWindow = Utils.GetParentItem<Window>(pivotGrid);
                        pivotGrid.FieldChooser = new PivotValueChooser(pivotGrid);
                        pivotGrid.FieldChooser.FieldListItemsSource = pivotGrid.LocalPossibleCalculations;
                        pivotGrid.FieldChooser.Title = Syncfusion.Windows.Controls.PivotGrid.Resources.SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "Pivot_Value_Chooser");
                        pivotGrid.FieldChooser.ItemHeight = pivotGrid.PivotValueChooserItemHeight;
                        pivotGrid.FieldChooser.ItemFontSize = pivotGrid.PivotValueChooserItemFontSize;
                        if (pivotGrid.ValueChooserSize != null)
                        {
                            pivotGrid.FieldChooser.Height = pivotGrid.ValueChooserSize.Height;
                            pivotGrid.FieldChooser.Width = pivotGrid.ValueChooserSize.Width;
                        }
                        if (parentWindow != null)
                        {
                            pivotGrid.FieldChooser.Owner = parentWindow;
                        }
                        else if (Application.Current != null && Application.Current.MainWindow != null)
                        {
                            pivotGrid.FieldChooser.Owner = Application.Current.MainWindow;
                        }
                        Syncfusion.Windows.Shared.SkinStorage.SetVisualStyle(pivotGrid.FieldChooser, pivotGrid.VisualStyle == PivotGridVisualStyle.Default ? Syncfusion.Windows.Shared.SkinStorage.GetVisualStyle(pivotGrid.Parent) : pivotGrid.VisualStyle.ToString());
                        pivotGrid.FieldChooser.Closing += new CancelEventHandler(FieldChooser_Closing);
                        pivotGrid.FieldChooser.Show();
                    }
                    else
                    {
                        if (pivotGrid.FieldChooser != null)
                            pivotGrid.FieldChooser.Close();
                    }
                }
            })));

        static void FieldChooser_Closing(object sender, CancelEventArgs e)
        {
            PivotValueChooser valueChooser = (sender as PivotValueChooser);
            valueChooser.GridControl.ValueChooserSize = new Size(valueChooser.Width, valueChooser.Height);
        }

        private Size valueChooserSize;

        /// <summary>
        /// Gets or sets the size of the PivotValueChooser window which is only available when 
        /// RowPivotsOnly is true.
        /// </summary>
        public Size ValueChooserSize
        {
            get { return valueChooserSize; }
            set { valueChooserSize = value; }
        }

#endif
      
        /// <summary>
        /// Gets or sets a value indicating whether [show grouping bar].
        /// </summary>
        /// <value><c>true</c> if [show grouping bar]; otherwise, <c>false</c>.</value>
        public bool ShowGroupingBar
        {
            get { return (bool)GetValue(ShowGroupingBarProperty); }
            set 
            {
#if !SILVERLIGHT
                if (!PivotEngine.RowPivotsOnly) //ignore if rowPivotsOnly is true.
#endif
                {
                    SetValue(ShowGroupingBarProperty, value);
                }
            }
        }
#if !SILVERLIGHT
        internal bool PrintHeader = true;

        internal bool PrintFooter = true;
#endif
        /// <summary>
        /// Gets or Set a value indication whether [show field list].
        /// </summary>
        public bool ShowFieldList
        {
            get { return (bool)GetValue(ShowFieldListProperty); }
            set { SetValue(ShowFieldListProperty, value); }
        }
        /// <summary>
        /// Gets or sets a value indication whether [enable value edit].
        /// </summary>
        public bool EnableValueEditing
        {
            get { return (bool)GetValue(EnableValueEditingProperty); }
            set { SetValue(EnableValueEditingProperty, value); }
        }

        public bool EnableSpecificColumnEditing
        {
            get { return (bool)GetValue(EnableSpecificColumnEditingProperty); }
            set { SetValue(EnableSpecificColumnEditingProperty, value); }

        }

        /// <summary>
        /// Gets or sets a value indication whether [enable value update].
        /// </summary>
        public bool EnableUpdating
        {
            get { return (bool)GetValue(EnableUpdatingProperty); }
            set { SetValue(EnableUpdatingProperty, value); }
        }


        private PivotUpdatingManager updateManager = null;

        /// <summary>
        /// Gets or sets a reference to a class that facilitates the pivot automatically updating itself due to changes in the underlying data. To enable this support,
        /// set <see cref="EnableUpdating"/> to true;
        /// </summary>
        /// <remarks>
        /// In order for the PivotGridControl to automatically respond to the changes in the underlying data, the underlying data must be either:
        ///     A) a DataTable or DataView
        /// or
        ///     B) an IList&lt;T&gt; where T implements both INotifyPropertyChanging and INotifyPropertyChanged. Additionally, the IList must also
        ///     implement INotifyCollectionChanged or IBindingList."
        /// </remarks>
        public PivotUpdatingManager UpdateManager
        {
            get
            {
                if (updateManager == null)
                {
                    updateManager = new PivotUpdatingManager(this);
                }
                return updateManager;
            }

            set
            {
                updateManager = value;
            }

        }

        private PivotEditingManager editManager = null;

        /// <summary>
        /// Gets or sets a reference to a class that facilitates editing of value cells. To enable this support,
        /// set <see cref="EnableValueEditing"/> to true;
        /// </summary>
        /// <remarks>
        /// The EditManager handles direct editing of value cell contents. The <see cref="PivotEditingManager.PivotValueEdited"/>
        /// event to allow for controlling what happens as the cell as the user leaves the cell. You should handle this event if
        /// you want to do any actions when a cell is completed. The default behavior will adjust the edit cell value so the newly typed entry
        /// is displayed. Also, any display value that depends upon this edited cell will also be updated. If you set e.Handled = true
        /// when you handle the event, then EditManager will make no changes to the displayed information, and it would be up to you
        /// to make any adjustments you want to see.
        /// 
        /// The EditManager also has public properties <see cref="PivotEditingManager.AllowEditingOfTotalCells"/> and 
        /// <see cref="PivotEditingManager.HideExpanders"/> that control whether you can edit Total cells and whether the expander glyphs
        /// are visible.
        /// 
        /// The EditManager has two public methods <see cref="PivotEditingManager.GetRowColumnPivotValuesAt"/> and 
        /// <see cref="PivotEditingManager.GetRawItemsFor"/> that return lists of pivot values and raw data items for a specified
        /// row and column.
        /// </remarks>
        public PivotEditingManager EditManager
        {
            get
            {
                if (editManager == null)
                {
                    editManager = new PivotEditingManager(this);
                }
                return editManager;
            }

            set
            {
                editManager = value;
            }

        }


        /// <summary>
        /// Gets or sets the grouping bar.
        /// </summary>
        /// <value>The grouping bar.</value>
        public PivotGridGroupingBar GroupingBar
        {
            get { return (PivotGridGroupingBar)GetValue(GroupingBarProperty); }
            set
            {
                SetValue(GroupingBarProperty, value);
                if (value != null)
                {
                    this.GroupingBar.Loaded += new RoutedEventHandler(GroupingBar_Loaded);
                }
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the value indicating the number of rows to be resized.
        /// </summary>
        public int AutoSizeRowCount
        {
            get
            {

                return m_AutoSizeRowCount;
            }
            set
            {
                m_AutoSizeRowCount = value;
            }
        }

#endif


        /// <summary>
        /// Gets or sets the Auto size option based on which Grid rows will be resized.
        /// </summary>
        /// <value>The auto size option.</value>
        public GridAutoSizeOption AutoSizeOption
        {
            get
            {
                return (GridAutoSizeOption)GetValue(AutoSizeOptionProperty);
            }

            set
            {
                SetValue(AutoSizeOptionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets source of data for this pivot table. This object should be either 
        /// an IEnumerable list, or a DataTable.
        /// </summary>
        [Category("Pivot")]
        [Browsable(false)]
        public object ItemSource
        {
            get { return (object)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets whether to enable hyperlinks only while hovering the cell and disables 
        /// on leaving it. The default value is false indicating that hyperlinks are always visible.
        /// This property only has an effect if RowPivotsOnly is true.
        /// </summary>
        [Category("Pivot")]
        [Browsable(false)]
        public bool EnableHyperlinkOnMouseOver
        {
            get { return (bool)GetValue(HyperlinkBehaviorProperty); }
            set { SetValue(HyperlinkBehaviorProperty, value); }
        }
#endif

        /// <summary>
        /// Gets the collection of PivotItems of PivotRows
        /// </summary>
        [Category("Pivot")]
        public ObservableCollection<PivotItem> PivotRows
        {
            get 
            {
                return (ObservableCollection<PivotItem>)GetValue(PivotRowsProperty); 
            }
            set 
            { 
                SetValue(PivotRowsProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for PivotRows.  This enables animation, styling, binding, etc...
        // Using a DependencyProperty as the backing store for ShowFieldChooser.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.PivotRows"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.PivotRows"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty PivotRowsProperty =
#if SILVERLIGHT
 DependencyProperty.Register("PivotRows", typeof(ObservableCollection<PivotItem>), typeof(PivotGridControl), new PropertyMetadata(new ObservableCollection<PivotItem>(), new PropertyChangedCallback(
#else
            DependencyProperty.Register("PivotRows", typeof(ObservableCollection<PivotItem>), typeof(PivotGridControl), new UIPropertyMetadata(null,new PropertyChangedCallback(
#endif
                (obj,args)=>
                {
                    PivotGridControl pivotGrid = obj as PivotGridControl;
                    if (pivotGrid != null && pivotGrid.PivotEngine != null)
                    {
                        var oldCollection = args.OldValue as ObservableCollection<PivotItem>;
                        if (oldCollection != null)
                            oldCollection.CollectionChanged -= pivotGrid.OnPivotRowsCollectionChanged;

                        var newCollection = args.NewValue as ObservableCollection<PivotItem>;
                        if (newCollection != null)
                        {
                            newCollection.CollectionChanged += pivotGrid.OnPivotRowsCollectionChanged;
                            for (int i = 0; i < newCollection.Count; i++)
                            {
                                pivotGrid.PivotEngine.InsertRowPivot(i, newCollection[i]);
                            }
                        }
                    }
                })));


        /// <summary>
        /// Gets the collection of PivotItems of PivotColumns
        /// </summary>
        [Category("Pivot")]
        public ObservableCollection<PivotItem> PivotColumns
        {
            get 
            { 
                return (ObservableCollection<PivotItem>)GetValue(PivotColumnsProperty);
            }
            set 
            { 
                SetValue(PivotColumnsProperty, value); 
            }
        }

        // Using a DependencyProperty as the backing store for PivotRows.  This enables animation, styling, binding, etc...
        // Using a DependencyProperty as the backing store for ShowFieldChooser.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.PivotColumns"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.PivotColumns"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty PivotColumnsProperty =
#if SILVERLIGHT
 DependencyProperty.Register("PivotColumns", typeof(ObservableCollection<PivotItem>), typeof(PivotGridControl), new PropertyMetadata(new ObservableCollection<PivotItem>(), new PropertyChangedCallback(
#else
            DependencyProperty.Register("PivotColumns", typeof(ObservableCollection<PivotItem>), typeof(PivotGridControl), new UIPropertyMetadata(null, new PropertyChangedCallback(
#endif
                (obj, args) =>
                {
                    PivotGridControl pivotGrid = obj as PivotGridControl;
                    if (pivotGrid != null && pivotGrid.PivotEngine != null)
                    {
                        var oldCollection = args.OldValue as ObservableCollection<PivotItem>;
                        if (oldCollection != null)
                            oldCollection.CollectionChanged -= pivotGrid.OnPivotColumnsCollectionChanged;

                        var newCollection = args.NewValue as ObservableCollection<PivotItem>;
                        if (newCollection != null)
                        {
                            newCollection.CollectionChanged += pivotGrid.OnPivotColumnsCollectionChanged;
                            for (int i = 0; i < newCollection.Count; i++)
                            {
                                pivotGrid.PivotEngine.InsertColumnPivot(i, newCollection[i]);
                            }
                        }
                    }
                })));

        /// <summary>
        /// Gets the collection of FilterExpression
        /// </summary>
        [Category("Pivot")]


        public ObservableCollection<FilterExpression> Filters
        {
            get { return (ObservableCollection<FilterExpression>)GetValue(FiltersProperty); }
            set { SetValue(FiltersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Filters.  This enables animation, styling, binding, etc...
        // Using a DependencyProperty as the backing store for ShowFieldChooser.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.Filters"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.Filters"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty FiltersProperty =
#if SILVERLIGHT
            DependencyProperty.Register("Filters", typeof(ObservableCollection<FilterExpression>), typeof(PivotGridControl), new PropertyMetadata(new ObservableCollection<FilterExpression>(), new PropertyChangedCallback(
#else
                DependencyProperty.Register("Filters", typeof(ObservableCollection<FilterExpression>), typeof(PivotGridControl), new UIPropertyMetadata(null, new PropertyChangedCallback(
#endif
                (obj, args) => 
                {
                    PivotGridControl pivotGrid = obj as PivotGridControl;
                    if (pivotGrid != null && pivotGrid.PivotEngine != null)
                    {
                        var oldCollection = args.OldValue as ObservableCollection<FilterExpression>;
                        if (oldCollection != null)
                            oldCollection.CollectionChanged -= pivotGrid.OnFiltersCollectionChanged;

                        var newCollection = args.NewValue as ObservableCollection<FilterExpression>;
                        if (newCollection != null)
                        {
                            newCollection.CollectionChanged += pivotGrid.OnFiltersCollectionChanged;
                            for (int i = 0; i < newCollection.Count; i++)
                            {
                                pivotGrid.PivotEngine.InsertFilter(i, newCollection[i]);
                            }
                        }
                    }
                })));


        /// <summary>
        /// Gets the collection of PivotCalculations
        /// </summary>
        [Category("Pivot")]
        public ObservableCollection<PivotComputationInfo> PivotCalculations
        {
            get { return (ObservableCollection<PivotComputationInfo>)GetValue(PivotCalculationsProperty); }
            set { SetValue(PivotCalculationsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PivotCalculations.  This enables animation, styling, binding, etc...
        // Using a DependencyProperty as the backing store for ShowFieldChooser.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.PivotCalculations"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.PivotCalculations"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty PivotCalculationsProperty =
#if SILVERLIGHT
 DependencyProperty.Register("PivotCalculations", typeof(ObservableCollection<PivotComputationInfo>), typeof(PivotGridControl), new PropertyMetadata(new ObservableCollection<PivotComputationInfo>(), new PropertyChangedCallback(
#else
            DependencyProperty.Register("PivotCalculations", typeof(ObservableCollection<PivotComputationInfo>), typeof(PivotGridControl), new UIPropertyMetadata(null, new PropertyChangedCallback(
#endif
                (obj, args) => 
                {
                    PivotGridControl pivotGrid = obj as PivotGridControl;
                    if (pivotGrid != null && pivotGrid.PivotEngine != null)
                    {
                        var oldCollection = args.OldValue as ObservableCollection<PivotComputationInfo>;
                        if (oldCollection != null)
                            oldCollection.CollectionChanged -= pivotGrid.OnCalculationsCollectionChanged;

                        var newCollection = args.NewValue as ObservableCollection<PivotComputationInfo>;
                        if (newCollection != null)
                        {
                            newCollection.CollectionChanged += pivotGrid.OnCalculationsCollectionChanged;
                            for (int i = 0; i < newCollection.Count; i++)
                            {
                                pivotGrid.PivotEngine.InsertPivotCalculation(i, newCollection[i]);
                            }
                        }
                    }
                })));


        /// <summary>
        /// Gets the collection of PivotItems of Pivot FieldList
        /// </summary>
        [Category("Pivot")]
        public ObservableCollection<PivotItem> PivotFields
        {
            get
            {
                return _PivotFields;
            }
        }

        /// <summary>
        /// Gets or sets the Expander Style.
        /// </summary>
        /// <value>The expander style.</value>
        [Category("Style")]
        public System.Windows.Style ExpanderStyle
        {
            get { return (System.Windows.Style)GetValue(ExpanderStyleProperty); }
            set { SetValue(ExpanderStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Grid line stroke.
        /// </summary>
        /// <value>The grid line stroke.</value>
        [Category("Style")]
        public Brush GridLineStroke
        {
            get { return (Brush)GetValue(GridLineStrokeProperty); }
            set { SetValue(GridLineStrokeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Grid Outer Border stroke.
        /// </summary>
        /// <value>The grid outer border stroke.</value>
        [Category("Style")]
        public Brush GridOuterBorderBrush
        {
            get { return (Brush)GetValue(GridOuterBorderBrushProperty); }
            set { SetValue(GridOuterBorderBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style for row header cells
        /// </summary>
        [Category("Style")]
        public PivotGridCellStyle RowHeaderCellStyle
        {
            get { return (PivotGridCellStyle)GetValue(RowHeaderCellStyleProperty); }
            set { SetValue(RowHeaderCellStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style for column header cells
        /// </summary>
        [Category("Style")]
        public PivotGridCellStyle ColumnHeaderCellStyle
        {
            get { return (PivotGridCellStyle)GetValue(ColumnHeaderCellStyleProperty); }
            set { SetValue(ColumnHeaderCellStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style for summary header cells
        /// </summary>
        [Category("Style")]
        public PivotGridCellStyle SummaryHeaderStyle
        {
            get { return (PivotGridCellStyle)GetValue(SummaryHeaderStyleProperty); }
            set { SetValue(SummaryHeaderStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style for summary cells
        /// </summary>
        [Category("Style")]
        public PivotGridCellStyle SummaryCellStyle
        {
            get { return (PivotGridCellStyle)GetValue(SummaryCellStyleProperty); }
            set { SetValue(SummaryCellStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style for value cells
        /// </summary>
        [Category("Style")]
        public PivotGridCellStyle ValueCellStyle
        {
            get { return (PivotGridCellStyle)GetValue(ValueCellStyleProperty); }
            set { SetValue(ValueCellStyleProperty, value); }
        }
        // added these properties for customization

#if !SILVERLIGHT
         /// <summary>
        /// Gets or sets the FieldListBorderBrush for GroupingBar
        /// </summary>
        public Brush FieldListBorderBrush
        {
            get { return (Brush)GetValue(FieldListBorderBrushProeprty); }
            set { SetValue(FieldListBorderBrushProeprty, value); }
        }
        /// <summary>
        /// Gets or sets the Background for GroupingBar
        /// </summary>
        public Brush GroupingBarBackground
        {
            get { return (Brush)GetValue(GroupingBarBackgroundProeprty); }
            set { SetValue(GroupingBarBackgroundProeprty, value); }
        }
#endif
        /// <summary>
        /// Gets or sets the Background of items(Togglebuttons) in GroupingBar
        /// </summary>
        public Brush GroupingBarItemBackground
        {
            get { return (Brush)GetValue(GroupingBarItemBackgroundProperty); }
            set { SetValue(GroupingBarItemBackgroundProperty, value); }
        }
        /// <summary>
        /// Gets or sets the BorderBrush of items(Togglebuttons) in GroupingBar
        /// </summary>
        public Brush GroupingBarItemBorderBrush
        {
            get { return (Brush)GetValue(GroupingBarItemBorderBrushProperty); }
            set { SetValue(GroupingBarItemBorderBrushProperty, value); }
        }
        /// <summary>
        /// Gets or sets the foreground brush of items(Togglebuttons) in GroupingBar
        /// </summary>
        public Brush GroupingBarItemForeground
        {
            get { return (Brush)GetValue(GroupingBarItemForegroundProperty); }
            set { SetValue(GroupingBarItemForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the MouseHoverBackground of items(Togglebuttons) in GroupingBar
        /// </summary>
        public Brush GroupingButtonHoverBackgroundBrush
        {
            get { return (Brush)GetValue(GroupingButtonHoverBackgroundBrushProperty); }
            set { SetValue(GroupingButtonHoverBackgroundBrushProperty, value); }
        }
        /// <summary>
        /// Gets or sets the MouseHoverBorderBrush of items(Togglebuttons) in GroupingBar
        /// </summary>
        public Brush GroupingButtonHoverBorderBrush
        {
            get { return (Brush)GetValue(GroupingButtonHoverBorderBrushProperty); }
            set { SetValue(GroupingButtonHoverBorderBrushProperty, value); }
        }


        /// <summary>
        /// Gets or sets the MouseHoverForeground of items(Togglebuttons) in GroupingBar.
        /// </summary>      
        public Brush GroupingButtonHoverForeground
        {
            get { return (Brush)GetValue(GroupingButtonHoverForegroundProperty); }
            set { SetValue(GroupingButtonHoverForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets CheckedBackground of items(Togglebuttons) in GroupingBar.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Brush GroupingButtonCheckedBackground
        {
            get { return (Brush)GetValue(GroupingButtonCheckedBackgroundProperty); }
            set { SetValue(GroupingButtonCheckedBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets CheckedForeground of items(ToggleButtons) in GroupingBar.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Brush GroupingButtonCheckedForeground
        {
            get { return (Brush)GetValue(GroupingButtonCheckedForegroundProperty); }
            set { SetValue(GroupingButtonCheckedForegroundProperty, value); }
        }


        /// <summary>
        /// 
        /// </summary>
        public Brush CurrentCellBorder
        {
            get { return (Brush)GetValue(CurrentCellBorderProperty); }
            set { SetValue(CurrentCellBorderProperty, value); }
        }


        /// <summary>
        /// Gets or sets a value indicating whether the supplied Item Source is dynamic
        /// </summary>
        public bool IsDynamicData
        {
            get { return (bool)GetValue(IsDynamicDataProperty); }
            set { SetValue(IsDynamicDataProperty, value); }
        }
        /// <summary>
        /// Gets or sets whether the DataSource is a collection of dynamic objects supported in the .Net 4.0 framework.
        /// </summary>
        public bool IsDataDynamic
        {
            get { return (bool)GetValue(IsDataDynamicProperty); }
            set { SetValue(IsDataDynamicProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore expand/collapse state of cells on serialization.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [ignore expand collapse on serialization]; otherwise, <c>false</c>.
        /// </value>
        public bool IgnoreExpandCollapseOnSerialization { get; set; }

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the conditional formats to be applied for PivotGridControl.
        /// </summary>
        /// <value>The conditional formats.</value>
        [Category("Style")]
        public FreezableCollection<PivotGridDataConditionalFormat> ConditionalFormats
        {
            get
            {
                return (FreezableCollection<PivotGridDataConditionalFormat>)GetValue(PivotGridControl.ConditionalFormatsProperty);
            }
            set
            {
                SetValue(PivotGridControl.ConditionalFormatsProperty, value);
            }
        }
#else

        /// <summary>
        /// Gets or sets the conditional formats to be applied for PivotGridControl.
        /// </summary>
        /// <value>The conditional formats.</value>
        [Category("Style")]
        public ObservableCollection<PivotGridDataConditionalFormat> ConditionalFormats
        {
            get
            {
                return (ObservableCollection<PivotGridDataConditionalFormat>)GetValue(PivotGridControl.ConditionalFormatsProperty);
            }
            set
            {
                SetValue(PivotGridControl.ConditionalFormatsProperty, value);
            }
        }
#endif

        /// <summary>
        /// Gets or sets the PivotEngine
        /// </summary>
        [Browsable(false)]
        public PivotEngine PivotEngine
        {
            get { return (PivotEngine)GetValue(PivotEngineProperty); }
            set
            {
                SetValue(PivotEngineProperty, value);
                SynchronizeEngine();
            }
        }

        private void SynchronizeEngine()
        {
            if (!IsExternalEngine || PivotEngine == null) return;

            ItemSource = PivotEngine.DataSource;

            PivotEngine.PivotSchemaChanged += new PivotSchemaChangedEventHandler(PivotEngine_PivotSchemaChanged);

            foreach (var item in PivotEngine.PivotColumns.Where(item => !PivotColumns.Contains(item)))
            {
                PivotColumns.Add(item);
            }

            foreach (var item in PivotEngine.PivotRows.Where(item => !PivotRows.Contains(item)))
            {
                PivotRows.Add(item);
            }

            foreach (var item in PivotEngine.PivotCalculations.Where(item => !PivotCalculations.Contains(item)))
            {
                PivotCalculations.Add(item);
            }

            foreach (var item in PivotEngine.Filters.Where(item => !Filters.Contains(item)))
            {
                Filters.Add(item);
            }

            if (InternalGrid != null) InternalGrid.Refresh(true);
        }

        void PivotEngine_PivotSchemaChanged(object sender, PivotSchemaChangedArgs e)
        {
            var pivotGridControlBase = this.InternalGrid;
            if (pivotGridControlBase != null) pivotGridControlBase.SynchronizeGrid(e);
        }

        /// <summary>
        /// Gets a collection of Syncfusion.PivotAnalysis.Base.FieldInfo objects that
        /// hold field names that you want to be visible in the engine. The names can
        /// be either public property names of the underlying data objects, or they can
        /// be expression field.
        /// </summary>
        /// <remarks>
        /// If your data contains fields that you do not want exposed to the pivoting
        /// process, then add the names of the properties you want to include to this
        /// list. All other fields will be excluded. If you leave this collection empty,
        /// the default behavior will be to make all public properties available for
        /// use in the pivot table.  To add an expression field, set the FieldInfo.FieldType
        /// to FieldsType.Expression and set FieldInfo.Expression to be a string holding
        /// a well formed expression defining the value that should appear in this field.
        /// </remarks>
        [Browsable(false)]
        public List<FieldInfo> AllowedFields
        {
            get
            {
#if !SILVERLIGHT
                if (this.PivotEngine != null)
                    allowedFields = this.PivotEngine.AllowedFields; 
#endif
                return allowedFields;
            }
        }

        /// <summary>
        /// Enable/Disable Background color for the grouping disabled fields. Default false
        /// </summary>
        /// <value>The selected items.</value>
        [Browsable(false)]
        public bool ShowDisabledGroupBackground
        {
            get { return (bool)GetValue(ShowDisabledGroupBackgroundProperty); }
            set
            {
                SetValue(ShowDisabledGroupBackgroundProperty, value);
                OnShowDisabledGroupBackgroundPropertyChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the Selected items which is an IEnumerable collection of Columns, Rows and Value.
        /// </summary>
        /// <value>The selected items.</value>
        [Browsable(false)]
        public SelectedItems SelectedItems
        {
            get { return (SelectedItems)GetValue(SelectedItemsProperty); }
            internal set { SetValue(SelectedItemsProperty, value); }
        }

        /// <summary>
        /// Gets the Internal grid
        /// </summary>
        /// <remarks>
        /// This is for advance Appearance.
        /// </remarks>
        [Browsable(false)]
        public PivotGridControlBase InternalGrid
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets whether the engine of this control is internally created or 
        /// supplied from external source
        /// </summary>
        [Browsable(false)]
        public bool IsExternalEngine
        {
            get
            {
                return _IsExtenalEngine;
            }
            set
            {
                _IsExtenalEngine = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the calculations should appear as rows or columns. THe default behavior is 
        /// for the calculations to appear as columns.
        /// </summary>
        [Category("Customization")]
        public bool ShowCalculationsAsColumns
        {
            get { return (bool)GetValue(ShowCalculationsAsColumnsProperty); }
            set { SetValue(ShowCalculationsAsColumnsProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the PivotGrid resize to its changed size
        /// </summary>
        [Category("Customization")]
        public bool ResizePivotGridToFit
        {
            get { return (bool)GetValue(ResizePivotGridToFitProperty); }
            set { SetValue(ResizePivotGridToFitProperty, value); }
        }

        /// <summary>
        /// Gets or sets to restrict the RowHeaders stretching when too many items in the PivotGrid GroupingBar computationArea. 
        /// For Example- If we have more than two items in the PivotGridComputationArea, then an button(down Arrow) appears after the ComputationArea in the groupingBar. 
        /// Click event of the button results in opening a new PivotGridComputationList window with the items in the ComputationArea. 
        /// We can perform drag and drop operation of the items between the GroupingBar Area (Row/Column/Data/Filter Areas) and PivotGridComputationList Window.
        /// </summary>
        public bool AllowRowHeaderAreaAutoSizing
        {
            get { return (bool)GetValue(AllowRowHeaderAreaAutoSizingProperty); }
            set { SetValue(AllowRowHeaderAreaAutoSizingProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the column, row headers should freeze or not
        /// </summary>
        [Category("Customization")]
        public bool FreezeHeaders
        {
            get { return (bool)GetValue(FreezeHeadersProperty); }
            set { SetValue(FreezeHeadersProperty, value); }
        }
        /// <summary>
        /// Gets or Sets whether the PivotGrid need to update on background using unique UI thread 
        /// </summary>
        public bool LoadInBackground
        {
            get { return (bool)GetValue(LoadInBackgroundProperty); }
            set { SetValue(LoadInBackgroundProperty, value); }
        }
        /// <summary>
        /// Gets or sets whether the layout should be updated immediately after the 
        /// pivoting info update or it should wait for a Refresh() call.
        /// </summary>
        [Category("Customization")]
        public bool DeferLayoutUpdate
        {
            get { return (bool)GetValue(DeferLayoutUpdateProperty); }
            set { SetValue(DeferLayoutUpdateProperty, value); }
        }

        internal double DefaultComputationColumnSize { get; set; }

        /// <summary>
        /// Gets or sets whether grand total calculations should be computed by the engine.
        /// </summary>
        /// <remarks>
        /// The default value is true.
        /// </remarks>
        [Category("Customization")]
        public bool ShowGrandTotals
        {
            get { return (bool)GetValue(ShowGrandTotalsProperty); }
            set { SetValue(ShowGrandTotalsProperty, value); }
        }

        /// <summary>
        /// Gets or sets value indicating whether sub total calculations should be shown or hidden.
        /// </summary>
        /// <remarks>
        /// The default value is true.
        /// </remarks>
        [Category("Customization")]
        public bool ShowSubTotals
        {
            get { return (bool)GetValue(ShowSubTotalsProperty); }
            set { SetValue(ShowSubTotalsProperty, value); }
        }

        /// <summary>
        /// Gets or sets value indicating whether show sub total calculations should be shown on top or not.
        /// </summary>
        /// <remarks>
        /// The default value is Normal.
        /// </remarks>
        public GridLayout GridLayout
        {
            get { return (GridLayout)GetValue(GridLayoutProperty); }
            set { SetValue(GridLayoutProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to Allow Selection of Cells as Like in Excel
        /// </summary>
        /// <value><c>true</c> if [allow selection]; otherwise, <c>false</c>.</value>
        [Category("Customization")]
        public bool AllowSelection
        {
            get { return (bool)GetValue(AllowSelectionProperty); }
            set { SetValue(AllowSelectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to [allow resize columns].
        /// </summary>
        /// <value><c>true</c> if [allow resize columns]; otherwise, <c>false</c>.</value>
        [Category("Customization")]
        public bool AllowResizeColumns
        {
            get { return (bool)GetValue(AllowResizeColumnsProperty); }
            set { SetValue(AllowResizeColumnsProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow resize rows].
        /// </summary>
        /// <value><c>true</c> if [allow resize rows]; otherwise, <c>false</c>.</value>
        [Category("Customization")]
        public bool AllowResizeRows
        {
            get { return (bool)GetValue(AllowResizeRowsProperty); }
            set { SetValue(AllowResizeRowsProperty, value); }
        }
        /// <summary>
        /// Gets and sets the option for sorting in PivotGrid Control
        /// </summary>
        [Category("Customization")]
        public PivotSortOption SortOption
        {
            get { return (PivotSortOption)GetValue(SortOptionProperty); }
            set { SetValue(SortOptionProperty, value); }
        }
        private bool _ShowExpanderForSinglePivot = true;
        /// <summary>
        /// Gets or Sets whether expander symbol needs to show/hide
        /// </summary>
        public bool ShowExpanderForSinglePivot
        {
            get
            {
                return _ShowExpanderForSinglePivot;
            }
            set 
            { 
                _ShowExpanderForSinglePivot = value; 
            }
        }

        /// <summary>
        /// Gets or Sets List of Column names which needs to be in edit mode.
        /// </summary>
        public List<string> EditColumnList
        {
            get;
            set;
        }

        internal RowDefinition GroupingBarRowHeight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether grid control load with default property fields for expression support.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [load with default property fields]; otherwise, <c>false</c>.
        /// </value>
        public bool LoadWithDefaultPropertyFields
        {
            get { return (bool)GetValue(LoadWithDefaultPropertyFieldsProperty); }
            set { SetValue(LoadWithDefaultPropertyFieldsProperty, value); }
        }

        #endregion

        #region [ Events ]
        /// <summary>
        /// The Event SelectionChanged will fire whenever the PivotGrid selection is changed
        /// </summary>
        public virtual event SelectionChanged SelectionChanged;

        /// <summary>
        /// The Event ItemSourceChanged will fire whenever the item source of PivotGrid is changed
        /// </summary>
        public virtual event ItemSourceChanged ItemSourceChanged;

        /// <summary>
        /// An event that notifies before Expand action occurs.
        /// </summary>
        public virtual event Expanding Expanding;

        /// <summary>
        /// An event that notifies after Expand action occurs.
        /// </summary>
        public virtual event Expanded Expanded;

        /// <summary>
        /// An event that notifies before Collapse action occurs.
        /// </summary>
        public virtual event Collapsing Collapsing;

        /// <summary>
        /// An event that notifies after Collapse action occurs.
        /// </summary>
        public virtual event Collapsed Collapsed;

        /// <summary>
        /// An event that notifies on click action over hyperlink cell.
        /// </summary>
        public virtual event HyperlinkCellClick HyperlinkCellClick;

        /// <summary>
        /// An event that notifies before refreshing the data.
        /// </summary>
        public virtual event DataRefreshing DataRefreshing;

        /// <summary>
        /// An event that notifies after refreshing the data.
        /// </summary>
        public virtual event DataRefreshed DataRefreshed;

        /// <summary>
        /// An event that notifies on loading GroupingBar.
        /// </summary>
        public virtual event GroupingBarLoaded GroupingBarLoaded;

        /// <summary>
        /// An event that notifies whenever ShowDisabledGroupBackGround property value changes.
        /// </summary>
        public virtual event EventHandler ShowDisabledGroupBackgroundPropertyChanged;

        /// <summary>
        /// An event that notifies on completion of filter action.
        /// </summary>
        public virtual event FilterActionCompletedEventHandler FilterActionCompleted;
        /// <summary>
        /// Occurs when loadInBackGround operations completed
        /// </summary>
        public event EventHandler LoadInBackgroundCompleted;
        /// <summary>
        /// Occurs when EnableOnDemandCalculation operations completed
        /// </summary>
        public event EventHandler OnDemandCalculationsCompleted;


         #endregion

        #region [ PropertyChanged Events ]

        static void OnPopulateDefaultPropertyFieldsChanged(DependencyObject depObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridControl pivotGrid = depObject as PivotGridControl;
            if (args.NewValue is bool && (bool)args.NewValue)
            {
                pivotGrid.PopulateDefaultPropertyFields();
            }
            else
            {
                pivotGrid.AllowedFields.Clear();
            }

        }
        /// <summary>
        /// Calls when the value of the property "ShowDisabledGroupBackground" changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnShowDisabledGroupBackgroundPropertyChanged(object sender, EventArgs e)
        {
            if (this.ShowDisabledGroupBackgroundPropertyChanged != null)
            {
                this.ShowDisabledGroupBackgroundPropertyChanged(this, e);
            }

            if (this.GroupingBar != null)
            {
                this.GroupingBar.DisableControls();
            }
        }

        void OnPivotRowsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.PivotEngine != null)
            {
                SynchronizePivotItems(e, true);
            }
        }


        void OnPivotColumnsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.PivotEngine != null)
            {
                if (this.EnableUpdating)
                {
                    UpdateManager.UnwireEvents();
                    UpdateManager.WireEvents();
                }
                SynchronizePivotItems(e, false);
            }
        }

        void OnCalculationsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.PivotEngine != null)
            {
                SynchronizeCalculations(e);
            }
        }

        void OnFiltersCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.PivotEngine != null)
            {
                SynchronizeFilters(e);
            }
        }

        void OnLoadInBackgroundCompleted(EventArgs e)
        {
            if (LoadInBackgroundCompleted != null)
            {
                LoadInBackgroundCompleted(this, e);
            }
        }

        /// <summary>
        /// Raises the LoadInBackgroundCompleted event which is raised when the pivot 
        /// completes its construction when <see cref="LoadInBackground"/> is set true.
        /// </summary>
        public void RaiseLoadInBackgroundCompleted()
        {
            OnLoadInBackgroundCompleted(EventArgs.Empty);
        }

#if !SILVERLIGHT
        static void OnHyperlinkBehaviorChanged(DependencyObject dependencyObject,
                                               DependencyPropertyChangedEventArgs args)
        {
            PivotGridControl gridControl = (PivotGridControl)dependencyObject;
            gridControl.InvalidateCells();
        }

        private PropertyDescriptorCollection itemProperties = null;

        private Type ItemType { get; set; }

        internal PropertyDescriptorCollection ItemProperties
        {
            get
            {
                if (ItemType == null && this.ItemSource != null)
                {
                    var list = this.ItemSource as IList;
                    if (list != null)
                    {
                        foreach (object o in list)
                        {
                            ItemType = o.GetType();
                            break;
                        }
                    }
                }
                if (ItemType != null)
                {
                    itemProperties = TypeDescriptor.GetProperties(ItemType);
                }
                List<PropertyDescriptor> includes = new List<PropertyDescriptor>();
                if (itemProperties != null)
                {
                    foreach (PropertyDescriptor pd in itemProperties)
                    {
                        if (AllowedFields.Count == 0 || AllowedFields.IndexOf(new FieldInfo() { Name = pd.Name }) > -1)
                        {
                            includes.Add(pd);
                        }
                    }
                }

                itemProperties = new PropertyDescriptorCollection(includes.ToArray());
                ItemType = null;
                return itemProperties;
            }
        }
#endif
        static void OnItemSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridControl gridControl = (PivotGridControl)dependencyObject;
            if (gridControl.PivotEngine != null)
            {
#if !SILVERLIGHT
                gridControl.PivotEngine.RefreshItemProperties();
                if (gridControl.RowPivotsOnly && args.NewValue != null)
                {
                    if (gridControl.PossiblePivotCalculations == null ||
                        gridControl.PossiblePivotCalculations.Count == 0)
                    {
                        ObservableCollection<PivotComputationInfo> pivotCalculationCollection =
                            new ObservableCollection<PivotComputationInfo>();

                        PropertyDescriptorCollection propertyDescriptorCollection = gridControl.ItemProperties;

                        foreach (PivotComputationInfo pci in gridControl.PivotCalculations)
                        {
                            pivotCalculationCollection.Add(pci);
                        }
                        foreach (PropertyDescriptor pd in propertyDescriptorCollection)
                        {
                            var pivotComputationInfo = gridControl.PivotCalculations.FirstOrDefault(p => p.FieldName == pd.Name);
                            if (pivotComputationInfo == null)
                            {
                                pivotCalculationCollection.Add(new PivotComputationInfo
                                    {
                                        FieldName = pd.Name,
                                        FieldHeader = pd.Name,
                                        SummaryType = GetPivotTableSummaryType(pd.PropertyType),
                                        CalculationType = CalculationType.NoCalculation,
                                    });
                            }
                        }
                        gridControl.PossiblePivotCalculations = pivotCalculationCollection;
                    }
                    List<int> hidden = new List<int>();
                    gridControl.PivotEngine.columnIndexes = Enumerable.Range(0, gridControl.PossiblePivotCalculations.Count + gridControl.PivotRows.Count).ToList();
                    int k = gridControl.PivotCalculations.Count + gridControl.PivotRows.Count;
                    gridControl.LocalPossibleCalculations = new ObservableCollection<PivotValueField>();
                    foreach (var item in gridControl.PossiblePivotCalculations)
                    {
                        PivotComputationInfo pci =
                            gridControl.PivotCalculations.FirstOrDefault(c => c.FieldName == item.FieldName);
                        PivotValueField pivotValueFields = null;
                        if (pci != null)
                        {
                            gridControl.LocalPossibleCalculations.Add(pivotValueFields = gridControl.GetPivotValueFields(item, true));
                        }
                        else
                        {
                            gridControl.PivotCalculations.Add(item);
                            gridControl.LocalPossibleCalculations.Add(pivotValueFields = gridControl.GetPivotValueFields(item, false));
                            hidden.Add(k);
                            k++;
                           // gridControl.InternalGrid.SetValueColumnVisibility(k, true);
                        }
                        pivotValueFields.PropertyChanged += gridControl.pivotValueField_PropertyChanged;

                    }
                    if (gridControl.FieldChooser != null && gridControl.FieldChooser.IsVisible)
                        gridControl.FieldChooser.FieldListItemsSource = gridControl.LocalPossibleCalculations;
                    gridControl.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            foreach (int i in hidden)
                            {
                                gridControl.InternalGrid.SetValueColumnVisibility(i, true);

                            }
                        }), System.Windows.Threading.DispatcherPriority.DataBind);
                }
                if (gridControl.ItemSource == null)
                {
                    gridControl.PossiblePivotCalculations.Clear();
                }
#endif
                gridControl.PivotEngine.DataSource = gridControl.ItemSource;

                if (gridControl.LoadWithDefaultPropertyFields)
                {
                    gridControl.PopulateDefaultPropertyFields();

                }
                if (gridControl.ItemSourceChanged != null)
                {
                    gridControl.ItemSourceChanged(gridControl, new ItemsSourceChangedEventArgs { OldValue = args.OldValue, NewValue = args.NewValue });
                }

                if (gridControl.EnableUpdating)
                {
                    gridControl.UpdateManager.InitializeUpdatingManager(gridControl);
                }
            }
        }

        private static SummaryType GetPivotTableSummaryType(Type type)
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


        static void OnPivotEngineChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridControl gridControl = (PivotGridControl)dependencyObject;
        }

        static void OnShowCalculationsAsColumnsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridControl gridControl = (PivotGridControl)dependencyObject;
            if (gridControl.PivotEngine != null)
            {
                gridControl.PivotEngine.ShowCalculationsAsColumns = gridControl.ShowCalculationsAsColumns;
            }
        }
#if !SILVERLIGHT
        static void OnEnableDefferedScrollingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridControl gridControl = (PivotGridControl)dependencyObject;
            if (gridControl.InternalGrid != null && gridControl.GridScrollViewer != null)
            {
                gridControl.GridScrollViewer.IsDeferredScrollingEnabled = gridControl.EnableDefferedScrolling;
            }
        }
#endif
        static void OnDataDynamicChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridControl gridControl = (PivotGridControl)dependencyObject;
            if (gridControl.PivotEngine != null)
            {
                gridControl.PivotEngine.IsDataDynamic = gridControl.IsDataDynamic;
            }
        }

        static void OnAllowRowHeaderAreaAutoSizingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridControl gridControl = (PivotGridControl)dependencyObject;
            if (gridControl.GroupingBar != null)
            {
                gridControl.GroupingBar.CalculatePivotRowItemWidth();
            }
        }

#if SILVERLIGHT
        static void OnShowDisabledGroupBackground(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridControl gridControl = (PivotGridControl)dependencyObject;
            if (gridControl.GroupingBar != null)
            {
                gridControl.GroupingBar.InvalidateArrange();
            }
        }
#endif

        static void OnFreezeHeadersChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (!DesignerProperties.GetIsInDesignMode(dependencyObject))
            {
                PivotGridControl gridControl = (PivotGridControl)dependencyObject;

                if (gridControl.PivotEngine != null && gridControl.InternalGrid != null)
                {
                    int headerColumns = 0, headerRows = 0;
                    headerColumns = gridControl.PivotEngine.PivotRows.Count;
                    headerRows = gridControl.PivotEngine.PivotColumns.Count;
                    if (gridControl.PivotCalculations.Count > 1)
                    {
                        if (!gridControl.PivotEngine.ShowCalculationsAsColumns)
                            headerColumns++;
                        else
                            headerRows++;
                    }

                    //// freezing headers
                    if (gridControl.FreezeHeaders)
                    {
                        gridControl.InternalGrid.Model.FrozenRows = headerRows;
                        gridControl.InternalGrid.Model.FrozenColumns = headerColumns;
                    }
                    else
                    {
                        gridControl.InternalGrid.Model.FrozenRows = 0;
                        gridControl.InternalGrid.Model.FrozenColumns = 0;
                    }
                }
            }
        }
#if SILVERLIGHT
        private static Syncfusion.Windows.Controls.Theming.VisualStyle currentStyle = Windows.Controls.Theming.VisualStyle.Default;
#else
                private static string currentStyle = "Default";
#endif

       
        static void OnStyleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridControl gridControl = (PivotGridControl)dependencyObject;
            if (gridControl.InternalGrid != null)
            {
                gridControl.ColumnHeaderCellStyle.ToolTipEnabled = gridControl.RowHeaderCellStyle.ToolTipEnabled = gridControl.ToolTipEnabled;
                gridControl.SummaryHeaderStyle.ToolTipEnabled = gridControl.SummaryCellStyle.ToolTipEnabled = gridControl.ToolTipEnabled;
                gridControl.ValueCellStyle.ToolTipEnabled = gridControl.ToolTipEnabled;
#if SILVERLIGHT
                Syncfusion.Windows.Controls.Theming.VisualStyle visualStyle = Syncfusion.Windows.Controls.Theming.SkinManager.GetVisualStyle(gridControl);
#else
                string visualStyle = Syncfusion.Windows.Shared.SkinStorage.GetVisualStyle(gridControl);
#endif
                if (currentStyle != visualStyle)
                {
                    DataTemplate dt = gridControl.InternalGrid.GetToolTipTemplate(visualStyle);
                    if (gridControl.InternalGrid.Resources.Contains("pivotTooltipTemplate"))
                    {
                        gridControl.InternalGrid.Resources.Remove("pivotTooltipTemplate");
                    }
                    gridControl.InternalGrid.Resources.Add("pivotTooltipTemplate", dt);
#if SILVERLIGHT
                    if (gridControl.GroupingBar != null && gridControl.GroupingBar.ButtonComputation != null)
                    {
                        Style buttonStyle = gridControl.GetComputationButtonStyle(visualStyle);
                        gridControl.GroupingBar.ButtonComputation.Style = buttonStyle;
                    }
#endif
                    currentStyle = visualStyle;
                }
                gridControl.InternalGrid.InvalidateCells();
            }
        }

        static void OnLoadInBackgroundChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridControl gridControl = (PivotGridControl)dependencyObject;
            if (gridControl != null && gridControl.InternalGrid != null)
            {
                gridControl.InternalGrid.LoadInBackground = (bool)args.NewValue;
            }
        }

        static void ToolTipEnabledPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridControl gridControl = (PivotGridControl)dependencyObject;
            if (gridControl != null && !DesignerProperties.GetIsInDesignMode(gridControl))
            {
#if SILVERLIGHT
                gridControl.Dispatcher.BeginInvoke(() =>
                {
#endif
                    gridControl.RowHeaderCellStyle.ToolTipEnabled = gridControl.ColumnHeaderCellStyle.ToolTipEnabled = gridControl.ToolTipEnabled;
                    gridControl.SummaryCellStyle.ToolTipEnabled = gridControl.SummaryHeaderStyle.ToolTipEnabled = gridControl.ToolTipEnabled;
                    gridControl.ValueCellStyle.ToolTipEnabled = gridControl.ToolTipEnabled;
                    PivotGridTooltipService.SetShowTooltips(gridControl, gridControl.ToolTipEnabled);
                    gridControl.InvalidateCells();
#if SILVERLIGHT
                    });        
#endif
            }
        }

        static void CustomToolTipTemplateKeyPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridControl gridControl = (PivotGridControl)dependencyObject;
            if (gridControl != null && !DesignerProperties.GetIsInDesignMode(gridControl))
            {
                gridControl.InvalidateCells();
            }
        }
        static void OnShowGrandTotalsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridControl gridControl = (PivotGridControl)dependencyObject;
            if (gridControl.PivotEngine != null)
            {
                gridControl.PivotEngine.ShowGrandTotals = gridControl.ShowGrandTotals;
            }
        }

        static void OnShowSubTotalsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridControl gridControl = (PivotGridControl)dependencyObject;
            if (gridControl.InternalGrid != null)
            {
                gridControl.InternalGrid.ShowSubTotals = gridControl.ShowSubTotals;
                if (gridControl.InternalGrid.ShowSubTotals)
                {
                    foreach (PivotItem item in gridControl.PivotRows)
                    {
                        gridControl.InternalGrid.SubTotalVisibilityRenderer(item);
                    }
                    foreach (PivotItem item in gridControl.PivotColumns)
                    {
                        gridControl.InternalGrid.SubTotalVisibilityRenderer(item);
                    }
                }
            }

        }

        static void OnAllowSelectionChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridControl gridControl = (PivotGridControl)dependencyObject;
            if (gridControl.InternalGrid != null)
            {
                gridControl.InternalGrid.AllowSelection = gridControl.AllowSelection;
            }
        }

        static void OnSelectedItemsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridControl gridControl = (PivotGridControl)dependencyObject;
            if (gridControl.PivotEngine != null)
            {
                gridControl.SelectedItems = gridControl.InternalGrid.SelectedItems;
            }
        }
        static void OnGridLayoutChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridControl gridControl = (PivotGridControl)dependencyObject;
            if (gridControl != null)
            {
                if (args.NewValue.ToString() == "Normal")
                {
                    gridControl.PivotEngine.GridLayout = GridLayout.Normal;
                }
                else
                    gridControl.PivotEngine.GridLayout = GridLayout.TopSummary;

            }
        }

        static void ShowGroupingBarPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridControl gridControl = (PivotGridControl)dependencyObject;
            if (gridControl != null && !DesignerProperties.GetIsInDesignMode(gridControl))
            {
                if (gridControl.GroupingBarRowHeight != null)
                {
                    if (((bool)args.NewValue))
                    {

                        if (gridControl.GroupingBar == null)
                        {
                            gridControl.GroupingBar = gridControl.GetTemplateChild("PART_PivotGridGroupingBar") as PivotGridGroupingBar;
                            if (gridControl.GroupingBar != null)
                            {
                                gridControl.GroupingBar.GridControl = gridControl;
                                gridControl.GroupingBar.Engine = gridControl.PivotEngine;
                                gridControl.GroupingBar.OnApplyTemplate();
                            }
                        }

#if SILVERLIGHT
                        gridControl.GroupingBarRowHeight.Height = new GridLength(68);
                        gridControl.GridOuterBorder.BorderThickness = new Thickness(1);
#else
                        gridControl.GroupingBarRowHeight.Height = new GridLength(70);
#endif
                        if (gridControl.GroupingBar != null)
                        {
                            if (gridControl.GroupingBar.RowHeaderArea != null)
                            {
                                gridControl.GroupingBar.RowHeaderArea.Visibility = Visibility.Visible;
                                gridControl.InvalidateCells();
                            }
                            else
                                gridControl.InvalidateCells();
                        }

#if SILVERLIGHT
                        gridControl.GridOuterBorder.BorderThickness = new Thickness(1);
#endif
                    }
                    else
                    {
                        gridControl.GroupingBarRowHeight.Height = new GridLength(0);
                        if (gridControl.GroupingBar != null)
                        {
                            if (gridControl.GroupingBar.RowHeaderArea != null)
                            {
#if !SILVERLIGHT
                                gridControl.GroupingBar.RowHeaderArea.Visibility = Visibility.Hidden;
#else
                                gridControl.GroupingBar.RowHeaderArea.Visibility = Visibility.Collapsed;
#endif
                            }
                        }

#if SILVERLIGHT
                        //gridControl.GridOuterBorder.Visibility = Visibility.Collapsed;
                        gridControl.GridOuterBorder.BorderThickness = new Thickness(0);
                        gridControl.InvalidateCells();
#endif
                    }
                }
            }
        }

#if !SILVERLIGHT
        static void ShowFieldListPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridControl control = dependencyObject as PivotGridControl;
            if (control != null && !DesignerProperties.GetIsInDesignMode(control))
            {
                if (!((bool)args.NewValue))
                {
                    control.GroupingBar.FieldList.Close();
                }
                else
                {
                    if(control.GroupingBar == null)
                        control.GroupingBar = new PivotGridGroupingBar();
                    control.GroupingBar.ShowFieldListExecuted(control.GroupingBar, new RoutedEventArgs() as ExecutedRoutedEventArgs);
                }
            }
        }
#endif
        static void EnableValueEditingPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridControl control = dependencyObject as PivotGridControl;
            if (control != null && !DesignerProperties.GetIsInDesignMode(control))
            {
                if (!((bool)args.NewValue))
                {
                     //false
                    if (control.editManager != null)
                    {
                        control.editManager.Dispose();
                        control.editManager = null;
                    }
                }
                else
                {
                     //true
                    if (control.editManager == null)
                    {
                        control.editManager = new PivotEditingManager(control);
                    }
                }
            }
        }

        static void EnableUpdatingPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridControl control = dependencyObject as PivotGridControl;
            if (control != null && !DesignerProperties.GetIsInDesignMode(control))
            {
                if (!((bool)args.NewValue))
                {
                    //false
                    if (control.updateManager != null)
                    {
                        control.updateManager.Dispose();
                        control.updateManager = null;
                    }
                }
                else
                {
                    //true
                    if (control.updateManager == null)
                    {
                        control.updateManager = new PivotUpdatingManager(control);
                    }
                }
            }
        }
        static void OnGridLineStrokeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridControl pivotGrid = dependencyObject as PivotGridControl;
            pivotGrid.InvalidateCells();
        }
        #endregion

        #region [ Overrides ]

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (PivotRows != null)
                this.PivotRows.Cast<PivotItem>().Where(i => String.IsNullOrEmpty(i.FieldHeader)).Select(i => i.FieldHeader = i.FieldMappingName).ToList();
            if (PivotColumns != null)
                this.PivotColumns.Cast<PivotItem>().Where(i => String.IsNullOrEmpty(i.FieldHeader)).Select(i => i.FieldHeader = i.FieldMappingName).ToList();
            if (this.PivotFields != null)
                this.PivotFields.Cast<PivotItem>().Where(i => String.IsNullOrEmpty(i.FieldHeader)).Select(i => i.FieldHeader = i.FieldMappingName).ToList();
            if (this.PivotCalculations != null)
                this.PivotCalculations.Cast<PivotComputationInfo>().Where(i => String.IsNullOrEmpty(i.FieldHeader)).Select(i => i.FieldHeader = i.FieldName).ToList();
#if SILVERLIGHT
            this.InternalGrid = GetTemplateChild("PART_PivotGridControlBase") as PivotGridControlBase;
            if (this.InternalGrid != null) { this.InternalGrid.StatePersistenceEnabled = this.StatePersistenceEnabled; this.InternalGrid.LoadInBackground = this.LoadInBackground; }
#endif
#if SILVERLIGHT
            this.GridScrollViewer = GetTemplateChild("PART_GridScrollViewer") as ScrollableContentViewer;
            this.BusyIndicatorHolder = GetTemplateChild("Part_IndicatorGrid") as Grid;
            if (this.BusyIndicatorHolder != null && this.LoadInBackground)
            {
                this.BusyIndicator = GetBusyIndicator();
                this.BusyIndicatorHolder.Children.Add(this.BusyIndicator);
            }
            if(this.GridScrollViewer!=null)
               this.GridScrollViewer.MouseMove += new MouseEventHandler(GridScrollViewer_MouseMove);
#else            
            this.GridScrollViewer = GetTemplateChild("PART_GridScrollViewer") as ScrollViewer;
            this.BusyIndicator = GetTemplateChild("PART_BusyIndicator") as BusyIndicator;
            this.InternalGrid = new PivotGridControlBase { GridControl = this, StatePersistenceEnabled = this.StatePersistenceEnabled, LoadInBackground = this.LoadInBackground };
            if (this.GridScrollViewer != null)
            {
                this.GridScrollViewer.Content = this.InternalGrid;
                this.GridScrollViewer.IsDeferredScrollingEnabled = this.EnableDefferedScrolling;
                //this.GridScrollViewer.ScrollChanged += new ScrollChangedEventHandler(GridScrollViewer_ScrollChanged);
            }
#endif


            this.GridOuterBorder = GetTemplateChild("PART_GridOuterBorder") as Border;
            this.DefaultComputationColumnSize = 90.0;
            if (this.GridOuterBorder != null)
            {
                if (DesignerProperties.GetIsInDesignMode(this))
                {
                    this.GridOuterBorder.BorderThickness = new Thickness(1);
                }
            }

            this.GroupingBarRowHeight = GetTemplateChild("PART_GroupingBarRowDefinition") as RowDefinition;
            if (this.ShowGroupingBar && !DesignerProperties.GetIsInDesignMode(this) && this.GroupingBarRowHeight != null)
            {
#if SILVERLIGHT
                GroupingBarRowHeight.Height = new GridLength(68);
                this.GridOuterBorder.BorderThickness = new Thickness(1);
#else               
                GroupingBarRowHeight.Height = new GridLength(70);
#endif
            }
            else
            {
                GroupingBarRowHeight.Height = new GridLength(0);
#if SILVERLIGHT
                this.GridOuterBorder.BorderThickness = new Thickness(0);
#endif
            }

            if (!DesignerProperties.GetIsInDesignMode(this) && this.ShowGroupingBar)
            {
                this.GroupingBar = GetTemplateChild("PART_PivotGridGroupingBar") as PivotGridGroupingBar;
                this.GroupingBar.GridControl = this;
                this.GroupingBar.Engine = this.PivotEngine;
#if SILVERLIGHT
                this.Dispatcher.BeginInvoke(() =>
                {
                    this.GroupingBar.ShowFieldList = this.ShowFieldList;
                    this.GroupingBar.ShowSelectedFieldsOnly = this.ShowSelectedFieldsOnly;
                });
#endif
            }
            this.InternalGrid.AllowSelection = this.AllowSelection;
            if (this.RowHeaderCellStyle != null)
            {
                this.RowHeaderCellStyle.PropertyChanged += new PropertyChangedEventHandler(CellStyle_PropertyChanged);
            }

            if (this.ColumnHeaderCellStyle != null)
            {
                this.ColumnHeaderCellStyle.PropertyChanged += new PropertyChangedEventHandler(CellStyle_PropertyChanged);
            }

            if (this.SummaryHeaderStyle != null)
            {
                this.SummaryHeaderStyle.PropertyChanged += new PropertyChangedEventHandler(CellStyle_PropertyChanged);
            }

            if (this.SummaryCellStyle != null)
            {
                this.SummaryCellStyle.PropertyChanged += new PropertyChangedEventHandler(CellStyle_PropertyChanged);
            }

            if (this.ValueCellStyle != null)
            {
                this.ValueCellStyle.PropertyChanged += new PropertyChangedEventHandler(CellStyle_PropertyChanged);
            }

            if (this.InternalGrid != null)
            {
#if SILVERLIGHT
                this.InternalGrid.GridControl = this;
#endif
                //if (this.AllowSelection)
                {
                    this.InternalGrid.RaiseSelectionChangedEvent(new PivotGridSelectionChangedEventArgs(null, null, GridSelectionReason.Clear));
                }
            }
            if (this.PivotEngine != null)
            {
                if (this.PivotEngine.UseIndexedEngine && this.PivotEngine.EnableOnDemandCalculations &&
                    !(this.AutoSizeOption == GridAutoSizeOption.None ||
                      this.AutoSizeOption == GridAutoSizeOption.VisibleRange))
                {
                    this.AutoSizeOption = GridAutoSizeOption.None;
                }
                if (this.PivotEngine.DataSource == null && this.ItemSource != null)
                    this.PivotEngine.DataSource = this.ItemSource;
                this.PivotEngine.ShowEmptyCells = this.ShowEmptyCells;
            }
            this.InternalGrid.ShowSubTotals = this.ShowSubTotals;
        }

        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "ShowSubTotal" && this.ShowSubTotals)
            this.InternalGrid.SubTotalVisibilityRenderer(sender as PivotItem);
        }
       
#if SILVERLIGHT
private void GridScrollViewer_MouseMove(object sender, MouseEventArgs e)
         {
            if (this.GridScrollViewer.HorizontalOffset > 0.0 || this.GridScrollViewer.VerticalOffset > 0.0)
             {
                if (!this.FreezeHeaders && this.ShowGroupingBar)
                 {
                    Popup RowHeaderPopup = this.GroupingBar.RowHeaderArea.Parent as Popup;
                    RowHeaderPopup.HorizontalOffset = (this.GridScrollViewer.HorizontalOffset * -1.0);
                    if (this.GridScrollViewer.VerticalOffset >= this.m_verticalOffset && this.GridScrollViewer.VerticalOffset <= this.m_actualVerticalOffset)
                    {
                        this.m_verticalOffset = this.GridScrollViewer.VerticalOffset;
                       RowHeaderPopup.VerticalOffset = RowHeaderPopup.VerticalOffset <= 32 ? 32 : this.m_actualVerticalOffset - this.GridScrollViewer.VerticalOffset;
                    }
                    else if (this.GridScrollViewer.VerticalOffset <= this.m_actualVerticalOffset && this.GridScrollViewer.VerticalOffset <= this.m_verticalOffset)
                    {
                        this.m_verticalOffset = this.GridScrollViewer.VerticalOffset;
                        if (this.m_actualVerticalOffset - this.GridScrollViewer.VerticalOffset >= 32)
                            RowHeaderPopup.VerticalOffset = this.m_actualVerticalOffset - this.GridScrollViewer.VerticalOffset;
                    }
                    else if (this.GridScrollViewer.VerticalOffset > this.m_actualVerticalOffset)
                    {
                        this.m_verticalOffset = this.GridScrollViewer.VerticalOffset;
                        RowHeaderPopup.VerticalOffset = 32;
                    }
                 }
             }
         }
#else
        //void GridScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        //{
        //    if (this.InternalGrid != null && this.AutoSizeOption == GridAutoSizeOption.All && (e.HorizontalChange > 0.0 || e.VerticalChange > 0))
        //    {
        //        this.InternalGrid.Model.ResizeColumnsToFit(GridRangeInfo.Cells(this.InternalGrid.ScrollRows.GetVisibleLinesRange(1).Start, this.InternalGrid.ScrollColumns.LastBodyVisibleLineIndex, this.InternalGrid.ScrollRows.LastBodyVisibleLineIndex, this.InternalGrid.ScrollColumns.LastBodyVisibleLineIndex), GridResizeToFitOptions.None);
        //        this.InternalGrid.Model.ResizeRowsToFit(GridRangeInfo.Cells(this.InternalGrid.ScrollRows.LastBodyVisibleLineIndex, this.InternalGrid.ScrollColumns.GetVisibleLinesRange(1).Start, this.InternalGrid.ScrollRows.LastBodyVisibleLineIndex, this.InternalGrid.ScrollColumns.LastBodyVisibleLineIndex), GridResizeToFitOptions.None);
        //    }
        //}
#endif
        private void ApplyVisualStyle(string visualStyleName)
        {
            var prefix = "Classic.";
            var suffix = ".Blue";
            var resourceDictionary = GetResourceDictionaryFullyQualifiedName(visualStyleName, ref prefix, ref suffix);

            ResourceDictionary resource = new ResourceDictionary()
            {
                Source = new Uri(resourceDictionary, UriKind.RelativeOrAbsolute)
            };

            this.Background = resource[prefix + "GridBackground"] as Brush;
            this.GridLineStroke = resource[prefix + "GridLineStroke"] as Brush;
            this.SummaryCellStyle = resource[prefix + "SummaryCellStyle"] as PivotGridCellStyle;
            this.ColumnHeaderCellStyle = resource[prefix + "ColumnHeaderStyle"] as PivotGridCellStyle;
            this.RowHeaderCellStyle = resource[prefix + "RowHeaderStyle"] as PivotGridCellStyle;
            this.SummaryHeaderStyle = resource[prefix + "SummaryHeaderStyle"] as PivotGridCellStyle;
            this.SummaryCellStyle = resource[prefix + "SummaryCellStyle"] as PivotGridCellStyle;
            this.ValueCellStyle = resource[prefix + "ValueCellStyle"] as PivotGridCellStyle;
            this.ExpanderStyle = resource[prefix + "ExpanderStyle"] as Style;
#if !SILVERLIGHT

            if (this.GroupingBar != null)
            {
                this.GroupingBar.Background = resource[prefix + "GroupingBarBackground"] as Brush;
                this.GroupingBar.ItemsBackground = resource[prefix + "GroupingBarItemsBackground"] as Brush;
                this.GroupingBar.ItemsBorderBrush = resource[prefix + "GroupingBarItemsBorderBrush"] as Brush;
                this.GroupingBar.FieldListBorderBrush = resource[prefix + "WindowBorderBrush"] as Brush;
            }

#endif
        }

        internal Style computationButtonStyle = null;
        /// <summary>
        /// Gets the Style from the themes files
        /// </summary>
#if SILVERLIGHT
        /// <param name="vs">VisualStyle</param>
        /// <returns>The ComputationButton Style for the provided VisualStyle.</returns>     
        internal Style GetComputationButtonStyle(Syncfusion.Windows.Controls.Theming.VisualStyle visualstyle)
        {
            var rd = new ResourceDictionary();
            string source = string.Empty;
            switch (visualstyle)
            {
                case Windows.Controls.Theming.VisualStyle.Office2007Black:
                    source = "/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.Brushes.Office2007Black.xaml";
                    break;
                case Windows.Controls.Theming.VisualStyle.Office2007Blue:
                    source = "/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.Brushes.Office2007Blue.xaml";
                    break;
                case Windows.Controls.Theming.VisualStyle.Office2007Silver:
                    source = "/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.Brushes.Office2007Silver.xaml";
                    break;
                case Windows.Controls.Theming.VisualStyle.Metro:
                    source = "/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.Brushes.Metro.xaml";
                    break;
                case Windows.Controls.Theming.VisualStyle.Blend:
                    source = "/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.Brushes.Blend.xaml";
                    break;
                default:
                    source = "/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.xaml";
                    break;
            }
            rd = new ResourceDictionary()
            {
                Source = new Uri(source, UriKind.RelativeOrAbsolute)
            };
            this.computationButtonStyle = rd["ComputationButtonStyle"] as Style;
            return this.computationButtonStyle;
        }
#endif

        private static string GetResourceDictionaryFullyQualifiedName(string visualStyleName, ref string prefix, ref string suffix)
        {
            switch (visualStyleName)
            {
                case "Office2007Silver":
                    prefix = "Office2007Silver.";
                    suffix = ".Silver";
                    return "/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.Brushes.Office2007Silver.xaml";
                case "Office2007Blue":
                    prefix = "Office2007Blue.";
                    suffix = ".Blue";
                    return "/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.Brushes.Office2007Blue.xaml";
                case "Office2007Black":
                    prefix = "Office2007Black.";
                    suffix = ".Black";
                    return "/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.Brushes.Office2007Black.xaml";
                case "Office2003":
                    prefix = "Office2003Blue.";
                    suffix = ".Blue";
                    return "/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.Brushes.Office2003.xaml";
                case "Blend":
                    prefix = "Blend.";
                    suffix = ".Black";
                    return "/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.Brushes.Blend.xaml";
                default:
                    prefix = "Classic.";
                    suffix = ".Blue";
                    return "/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.Brushes.Classic.xaml";
            }
        }

        #endregion

        #region [ Public Methods ]

        bool onDemandCalculationsLoaded = false;

        internal bool OnDemandCalculationsLoaded
        {
            get { return onDemandCalculationsLoaded; }
            set
            {
                onDemandCalculationsLoaded = value;
                if (value && OnDemandCalculationsCompleted != null)
                {
                    OnDemandCalculationsCompleted(this, EventArgs.Empty);
                }
            }
        }


        /// <summary>
        /// This method triggers the lazy idle calculations of summaries when EnableLoadOnDemandCalculations is true.
        /// </summary>
        public void DoLazyCalculations()
        {
            if (( OnDemandCalculationsLoaded = PivotEngine.DoLazyCalculation()) == false)
            {
#if !SILVERLIGHT
                this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        DoLazyCalculations();
                    }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
#else
                DoLazyCalculations();
#endif
            }
        }
#if !SILVERLIGHT
        /// <summary>
        /// Shows the Print Preview window with empty template and default title
        /// </summary>
        /// <param name="Win">Gets the type of the Window</param>
        public void ShowPrintPreview(Window Win)
        {
            this.InternalGrid.PrintRange = GridRangeInfo.Table();
            PivotGridPrintDocument printDocument = new PivotGridPrintDocument(this);
            printDocument.Margin = new Thickness(20);
            PrintPreviewControl navigationControl = new PrintPreviewControl(printDocument);
            navigationControl.FooterTemplate = null;
            navigationControl.HeaderTemplate = null;
            if (Win.GetType().BaseType.Name.ToLower() == "chromelesswindow")
            {
                ChromelessWindow printWindow = new ChromelessWindow() { Title = "Print Preview", Content = navigationControl };
                printWindow.ShowDialog();
            }
            else
            {
                Window printWindow = new Window() { Title = "Print Preview", Content = navigationControl };
                printWindow.ShowDialog();
            }
        }

        /// <summary>
        /// Shows the Print Preview window with templates for header, footer and with default title.
        /// </summary>
        /// <param name="headerTemplate"></param>
        /// <param name="footerTemplate"></param>
        /// <param name="win"></param>
        public void ShowPrintPreview(DataTemplate headerTemplate, DataTemplate footerTemplate, Window win)
        {
            this.InternalGrid.PrintRange = GridRangeInfo.Table();
            PivotGridPrintDocument printDocument = new PivotGridPrintDocument(this);
            printDocument.Margin = new Thickness(20);
            PrintPreviewControl navigationControl = new PrintPreviewControl(printDocument);
            if (this.PrintHeader)
                navigationControl.HeaderTemplate = headerTemplate;
            else
                navigationControl.HeaderTemplate = null;
            if (this.PrintFooter)
                navigationControl.FooterTemplate = footerTemplate;
            else
                navigationControl.FooterTemplate = null;
            if (win.GetType().BaseType.Name.ToLower() == "chromelesswindow")
            {
                ChromelessWindow printWindow = new ChromelessWindow() { Title = "Print Preview", Content = navigationControl };
                printWindow.ShowDialog();
            }
            else
            {
                Window printWindow = new Window() { Title = "Print Preview", Content = navigationControl };
                printWindow.ShowDialog();
            }
        }

        /// <summary>
        /// Shows the Print Preview window with templates for header, footer and with user defined title.
        /// </summary>
        /// <param name="headerTemplate">DateTemplate for Header</param>
        /// <param name="footerTemplate">DataTemplate for Footer</param>
        /// <param name="title">Title for Print Preview Window</param>
        /// <param name="win">Type of Window</param>
        public void ShowPrintPreview(DataTemplate headerTemplate,DataTemplate footerTemplate,string title, Window win)
        {
            this.InternalGrid.PrintRange = GridRangeInfo.Table();
            PivotGridPrintDocument printDocument = new PivotGridPrintDocument(this);
            printDocument.Margin = new Thickness(20);
            PrintPreviewControl navigationControl = new PrintPreviewControl(printDocument);
            if (this.PrintHeader)
                navigationControl.HeaderTemplate = headerTemplate;
            else
                navigationControl.HeaderTemplate = null;
            if (this.PrintFooter)
                navigationControl.FooterTemplate = footerTemplate;
            else
                navigationControl.FooterTemplate = null;
            if (win.GetType().BaseType.Name.ToLower() == "chromelesswindow")
            {
                ChromelessWindow printWindow = new ChromelessWindow() { Title = title, Content = navigationControl };
                printWindow.ShowDialog();
            }
            else
            {
                Window printWindow = new Window() { Title = title, Content = navigationControl };
                printWindow.ShowDialog();
            }
        }
#endif
        /// <summary>
        /// Populates the default property fields.
        /// </summary>
        public void PopulateDefaultPropertyFields()
        {
            if (this.PivotEngine != null)
                this.PivotEngine.PopulateDefaultPropertyFields();
        }

        /// <summary>
        /// Resets the pivot data (resets the pivot engine, pivot rows, pivot columns, pivot fields, pivot calculations and items source).
        /// </summary>
        public void ResetPivotData()
        {
            this.PivotCalculations.Clear();
            this.PivotColumns.Clear();
            this.PivotFields.Clear();
            this.PivotRows.Clear();
            if (this.PivotEngine != null)
            {
                this.PivotEngine.Reset();
            }

        }

        /// <summary>
        /// Resets the covered cell ranges in the InternalGrid to match the covered cells in the PivotEngine.
        /// </summary>
        public void ResetCoveredCells()
        {
            this.InternalGrid.CoveredCells.Clear();

            foreach (CoveredCellRange range in this.PivotEngine.CoveredRanges)
            {
                if (range.Top <= range.Bottom && range.Left <= range.Right)
                {
                    InternalGrid.CoveredCells.Add(new Syncfusion.Windows.Controls.Cells.CoveredCellInfo(range.Top, range.Left, range.Bottom, range.Right));
                }
            }
            this.InternalGrid.InvalidateCells();

        }

        /// <summary>
        /// Refreshes the control with latest data and invalidates the grid
        /// </summary>
        public void Refresh()
        {
            if (this.PivotEngine != null && !DesignerProperties.GetIsInDesignMode(this))
            {
                if (!this.ShowGroupingBar)
                {
                    if (this.GridScrollViewer == null)
                        this.ApplyTemplate();
                    if (this.GridScrollViewer != null)
                    {
                        this.GridScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                        this.GridScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                    }
                }
                this.StatePersistenceEnabled = false;
                this.PivotEngine.RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
            }
        }

        /// <summary>
        /// Refresh the control without affecting persistence of expand/collapse states.
        /// </summary>
        internal void InternalRefresh()
        {
            if (this.PivotEngine != null && !DesignerProperties.GetIsInDesignMode(this))
            {
                this.PivotEngine.RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
            }
        }

        /// <summary>
        /// Expands all the group.
        /// </summary>
        public void ExpandAllGroup()
        {
            if (this.InternalGrid != null)
            {
                this.InternalGrid.ExpandAllGroup();
            }
            else
            {
                throw new PivotGridException("Invalid operation, Internal Grid is null");
            }
        }

        /// <summary>
        /// Collapses all the group
        /// </summary>
        public void CollapseAllGroup()
        {
            if (this.InternalGrid != null)
            {
                this.InternalGrid.CollapseAllGroup();
            }
            else
            {
                throw new PivotGridException("Invalid operation, Internal Grid is null");
            }
        }

        /// <summary>
        /// Expands the row for the specified unique text.
        /// </summary>
        /// <param name="uniqueText">Unique text of the Expander header.</param>
        public void ExpandRow(string uniqueText)
        {
            if (this.InternalGrid != null)
            {
                this.InternalGrid.ExpandRow(uniqueText);
            }
            else
            {
                throw new PivotGridException("Invalid operation, Internal Grid is null");
            }
        }

        /// <summary>
        /// Expand the rows for provided array of unique text.
        /// </summary>
        /// <param name="list">Array of Unique text of the Expander headers.</param>
        public void ExpandRow(List<string> list)
        {
            if (this.InternalGrid != null)
            {
                this.InternalGrid.ExpandRow(list);
            }
            else
            {
                throw new PivotGridException("Invalid operation, Internal Grid is null");
            }
        }

        /// <summary>
        /// Collapses the row for the specified unique text.
        /// </summary>
        /// <param name="uniqueText">Unique text of the Expander header.</param>
        public void CollapseRow(string uniqueText)
        {
            if (this.InternalGrid != null)
            {
                this.InternalGrid.CollapseRow(uniqueText);
            }
            else
            {
                throw new PivotGridException("Invalid operation, Internal Grid is null");
            }
        }

        /// <summary>
        /// Collapse the rows for the specified unique text.
        /// </summary>
        /// <param name="list">Array of Unique text of the Expander headers.</param>
        public void CollapseRow(List<string> list)
        {
            if (this.InternalGrid != null)
            {
                this.InternalGrid.CollapseRow(list);
            }
            else
            {
                throw new PivotGridException("Invalid operation, Internal Grid is null");
            }
        }

        /// <summary>
        /// Expands the column for the specified unique text.
        /// </summary>
        /// <param name="uniqueText">Unique text of the Expander header.</param>
        public void ExpandColumn(string uniqueText)
        {
            if (this.InternalGrid != null)
            {
                this.InternalGrid.ExpandColumn(uniqueText);
            }
            else
            {
                throw new PivotGridException("Invalid operation, Internal Grid is null");
            }
        }

        /// <summary>
        /// Expand the Columns for provided array of unique text.
        /// </summary>
        /// <param name="list">Array of Unique text of the Expander headers.</param>
        public void ExpandColumn(List<string> list)
        {
            if (this.InternalGrid != null)
            {
                this.InternalGrid.ExpandColumn(list);
            }
            else
            {
                throw new PivotGridException("Invalid operation, Internal Grid is null");
            }
        }

        /// <summary>
        /// Collapses the column for the specified unique text.
        /// </summary>
        /// <param name="uniqueText">Unique text of the Expander header.</param>
        public void CollapseColumn(string uniqueText)
        {
            if (this.InternalGrid != null)
            {
                this.InternalGrid.CollapseColumn(uniqueText);
            }
            else
            {
                throw new PivotGridException("Invalid operation, Internal Grid is null");
            }
        }

        /// <summary>
        /// Collapse the Columns for the specified unique text.
        /// </summary>
        /// <param name="list">Array of Unique text of the Expander headers.</param>
        public void CollapseColumn(List<string> list)
        {
            if (this.InternalGrid != null)
            {
                this.InternalGrid.CollapseColumn(list);
            }
            else
            {
                throw new PivotGridException("Invalid operation, Internal Grid is null");
            }
        }

        /// <summary>
        /// Invalidates the cells.
        /// </summary>
        public void InvalidateCells()
        {
            if (this.InternalGrid != null)
            {
                this.InternalGrid.InvalidateCells();
            }
        }
   #if !SILVERLIGHT
        /// <summary>
        /// When RowPivotsOnly is true, this method returns information on the currently sorted value fields.
        /// </summary>
        /// <param name="fieldNames">Returns a list of the sorted fieldnames in the order that they were sorted.</param>
        /// <param name="sortDirections">Returns a list of the ListSortDirections for each sorted value field in the order that the fields were sorted.</param>
        public void GetSortedValueFields(out List<string> fieldNames, out List<ListSortDirection> sortDirections)
        {
            fieldNames = null;
            sortDirections = null;

            if (RowPivotsOnly && InternalGrid.sortHeaderList != null && InternalGrid.sortHeaderList.Count > 0)
            {
                List<string> names = new List<string>();
                for (int i = 0; i < InternalGrid.sortHeaderList.Count; i++)
                {
                    names.Add(this.InternalGrid.GetNameAt(InternalGrid.sortHeaderList[i]));
                }
                fieldNames = names; 
                if (fieldNames != null && fieldNames.Count > 0)
                {
                    sortDirections = new List<ListSortDirection>();
                    foreach (string name in fieldNames)
                    {
                        sortDirections.Add(PivotEngine.GetSortDirection(InternalGrid.GetColumnIndexFromName(name)));
                    }
                }
            }
        }

        /// <summary>
        /// When RowPivotsOnly is true, this method returns the currently filtered value field names.
        /// </summary>
        /// <returns>A list of names for the currently filtered value columns.</returns>
        public List<string> GetFilteredValueFields()
        {
            List<string> list = null;
            if (RowPivotsOnly && ColumnFilterPopup.Exclusions != null && ColumnFilterPopup.Exclusions.Count > 0)
            {
                list = new List<string>(ColumnFilterPopup.Exclusions.Keys);
                list.Sort();
            }
            return list;
        }

        /// <summary>
        /// When RowPivotsOnly is true, returns a list of filtered values associated with a specidied value column.
        /// </summary>
        /// <param name="fieldName">The field name of the column whose filtered values should be returned.</param>
        /// <param name="returnExclusions">True if you want the excluded values returned, False is you want tothe included values returned.</param>
        /// <returns>The requested list of values.</returns>
        public List<string> GetFilteredFieldValues(string fieldName, bool returnExclusions)
        {
            List<string> list = null;
            if (RowPivotsOnly && ColumnFilterPopup.Exclusions != null && ColumnFilterPopup.Exclusions.Count > 0 && ColumnFilterPopup.Exclusions.ContainsKey(fieldName))
            {
                if (returnExclusions)
                {
                    list = new List<string>(ColumnFilterPopup.Exclusions[fieldName]);
                    list.Sort();
                }
                else
                {
                    int col = this.InternalGrid.GetColumnIndexFromName(fieldName);
                    if (col > -1)
                    {

                        HashSet<string> set = new HashSet<string>();

                        int c = PivotEngine.ResolveColumnIndex(col);
                        for (int i = 1; i < PivotEngine.RowCount - PivotEngine.PivotRows.Count - 1; ++i)
                        {
                            PivotCellInfo pci = this.PivotEngine.PivotValues[i, c];
                            if (pci != null)
                            {
                                bool isValue = pci.CellType == PivotCellType.ValueCell;
                                bool isRowPivot = pci.CellType == (PivotCellType.HeaderCell | PivotCellType.RowHeaderCell);
                                if (isValue || isRowPivot)
                                {
                                    if (!ColumnFilterPopup.Exclusions[fieldName].Contains(pci.FormattedText))
                                        set.Add(pci.FormattedText);
                                }
                            }
                        }
                        list = set.ToList();
                        list.Sort();
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// When RowPivots is true, this method expands/collapses all groups under a particular row pivot column.
        /// </summary>
        /// <param name="rowPivotFieldName">The mapping name for the row pivot field. If this string is empty or null, all nodes will be affected.</param>
        /// <param name="expand">True to expand all nodes, false to collapse them.</param>
        public void ExpandCollapseAtLevel(string rowPivotFieldName, bool expand)
        {
            if (!RowPivotsOnly)
                return;
           
            if (rowPivotFieldName == null || rowPivotFieldName == string.Empty)
            {
                if (expand)
                {
                    InternalGrid.ExpandAllGroup();
                }
                else
                {
                    InternalGrid.CollapseAllGroup();
                }

            }
            else
            {
                int loc = PivotRows.IndexOf(PivotRows.Where(pi => pi.FieldMappingName == rowPivotFieldName).FirstOrDefault());
                if (loc > -1)
                {

                    int i = 1;
                    while (i < PivotEngine.RowCount - PivotRows.Count)
                    {
                        PivotCellInfo pci = PivotEngine[i, loc];
                        if (pci != null && pci.CellType == (PivotCellType.ExpanderCell | PivotCellType.RowHeaderCell))
                        {
                            Expand(i, loc, pci, pci.UniqueText, expand);

                            i += pci.CellRange.Bottom - pci.CellRange.Top + 1;
                        }
                        else
                        {
                            i++;
                        }
                    }
                }
            }
            
        }
        private void Expand(int row, int col, PivotCellInfo cellInfo, string uniqueText, bool expand)
        {
            GridStyleInfo styleInfo = null;
            styleInfo = this.InternalGrid.Model[row, col];
            cellInfo.Tag = styleInfo;
            if (expand)
            {
                this.RaiseOnExpanding(new ExpandingEventArgs(cellInfo, uniqueText, styleInfo.CellRowColumnIndex), cellInfo);
            }
            else
            {
                this.RaiseOnCollapsing(new CollapsingEventArgs(cellInfo), styleInfo);
            }
                      
        }

        ///// <summary>
        ///// Exports to XLS.
        ///// </summary>
        ///// <param name="fileName">The Name of file</param>
        //public void ExportToXls(String fileName)
        //{
        //    try
        //    {

        //        GridExcelExport gridExcelExport = new GridExcelExport(this);
        //        gridExcelExport.Export(fileName);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new PivotGridException("Error occured in Exporting the PivotData", ex);
        //    }
        //}
#else
        ///// <summary>
        ///// Exports to XLS.
        ///// </summary>
        //public void ExportToXls()
        //{
        //    SaveFileDialog sfv = new SaveFileDialog { DefaultExt = ".xls", Filter = "(*.xls)|*.xls" };
        //    if (sfv.ShowDialog() == true)
        //    {
        //        Stream stream = sfv.OpenFile();
        //        GridExcelExport exportToXls = new GridExcelExport(this);
        //        exportToXls.Export(stream);
        //        stream.Close();
        //    }
        //}
#endif

#if !SILVERLIGHT
        ///// <summary>
        ///// Exports to PDF.
        ///// </summary>
        ///// <param name="fileName">The Name of file</param>
        //public void ExportToPdf(string fileName)
        //{
        //    GridPdfExport gridPdfExport = new GridPdfExport(this);
        //    gridPdfExport.Export(fileName);
        //}
#else
        ///// <summary>
        ///// Exports to PDF.
        ///// </summary>
        //public void ExportToPdf()
        //{
        //    SaveFileDialog sfv = new SaveFileDialog { DefaultExt = ".pdf", Filter = "(*.pdf)|*.pdf" };
        //    if (sfv.ShowDialog() == true)
        //    {
        //        Stream stream = sfv.OpenFile();
        //        GridPdfExport exporttopdf = new GridPdfExport(this);
        //        exporttopdf.Export(stream);
        //        stream.Close();
        //    }
        //}

#endif
#if !SILVERLIGHT

        ///// <summary>
        ///// Exports to doc.
        ///// </summary>
        ///// <param name="fileName">Name of the file.</param>
        //public void ExportToDoc(string fileName)
        //{
        //    GridWordExport gridWordExport = new GridWordExport(this);
        //    gridWordExport.Export(fileName);
        //}
#else
        ///// <summary>
        ///// Exports to doc.
        ///// </summary>
        //public void ExportToDoc()
        //{
        //    SaveFileDialog sfv = new SaveFileDialog { DefaultExt = ".doc", Filter = "(*.doc)|*.doc" };
        //    if (sfv.ShowDialog() == true)
        //    {
        //        Stream stream = sfv.OpenFile();
        //        GridWordExport gridWordExport = new GridWordExport(this);
        //        gridWordExport.Export(stream);
        //        stream.Close();
        //    }
        //}

#endif

        #endregion

        #region [ Helper Methods ]

        #region Code to support sorting of pivots by reaarranging the underly pivot information and not rebuilding pivot.

        /// <summary>
        /// Sorts a PivotItem specified by the index and isRowPivot settings.
        /// </summary>
        /// <param name="index">The index of the pivot item in its parent collection.</param>
        /// <param name="sortDir">The ListSortDirection for the sort.</param>
        /// <param name="isRowPivot">True if the pivot item is a row pivot and false if it is a column pivot.</param>
        public void SortPivotItem(int index, ListSortDirection sortDir, bool isRowPivot)
        {
            if (PivotEngine.EnableOnDemandCalculations && !PivotEngine.UseIndexedEngine)
            {
                //this call ensures summaries fully computed to avoid issue with summaries displaying empty after a sort
                PivotEngine.EnsureCalculationsLoaded();
            };
                
            int start = isRowPivot ? this.PivotEngine.PivotColumns.Count + (this.PivotEngine.PivotCalculations.Count > 1 ? 1 : 0) : this.PivotRows.Count;

            bool isLastPivot = isRowPivot ? index == PivotRows.Count - 1 : index == PivotColumns.Count - 1;
            int i = start;
            int end = isRowPivot ? PivotEngine.RowCount - 1 : PivotEngine.ColumnCount - this.PivotEngine.PivotCalculations.Count - 1;
            int sectionEnd = end;
            int sectionStart = 0;
            List<string> needToCollapse = new List<string>();
            for (int k = start; k < end; ++k)
            {
                int inc = 0;
                for (int mm = index; mm < (isRowPivot ? PivotRows.Count - 1 : PivotColumns.Count - 1); ++mm)
                {
                    PivotCellInfo info1 = isRowPivot ? PivotEngine.PivotValues[k][mm] : PivotEngine.PivotValues[mm][k];
                    PivotItem pi = isRowPivot ? PivotRows[mm] : PivotColumns[mm];

                    inc = 0;
                    if(mm == index && info1.CellRange != null)
                    {
                        inc = isRowPivot ? info1.CellRange.Bottom - info1.CellRange.Top : info1.CellRange.Right - info1.CellRange.Left;
                    }

                    if (InternalGrid.IsGroupCollapsed(info1, k, isRowPivot, pi))
                    {
                        int count = 0;
                        if (isRowPivot)
                        {
                            ExpandRow(info1.UniqueText);
                            if (PivotRows[mm].ShowSubTotal == false)
                            {
                                this.InternalGrid.Model.RowHeights.GetHidden(k, out count);
                                if (count <= info1.CellRange.Bottom - info1.CellRange.Top + 1)
                                {
                                    count = 0;
                                }
                            }
                        }
                        else
                        {
                            ExpandColumn(info1.UniqueText);
                            if (PivotColumns[mm].ShowSubTotal == false)
                            {
                                this.InternalGrid.Model.ColumnWidths.GetHidden(k, out count);
                                if (count <= info1.CellRange.Right - info1.CellRange.Left + 1 + PivotColumns.Count)
                                {
                                    count = 0;
                                }
                            }
                        }
                        
                        if (count == 0)
                        {
                            needToCollapse.Add(info1.UniqueText);
                        }
                    }
                 }
                 k += inc;
            }

            //flip this so sort works properly with hidden subtotals then flip it back at the end...
            List<PivotItem> hiddenSubTotals = new List<PivotItem>(isRowPivot ? this.PivotRows.Where(x => x.ShowSubTotal == false) : this.PivotColumns.Where(x => x.ShowSubTotal == false));
            if (hiddenSubTotals.Count > 0)
            {
                foreach (PivotItem pi in hiddenSubTotals)
                {
                    pi.ShowSubTotal = true;
                }
                this.InternalGrid.ClearHiddenSubtotals(isRowPivot);
            }
            
            if (!isLastPivot || (!isRowPivot && PivotEngine.PivotCalculations.Count > 1))
            {
                ClearCoveredCellsIn(start, end, index, isRowPivot);
            }

            while (i < end && sectionEnd <= end)
            {
                sectionStart = i;
                if (index > 0)
                {
                    PivotCellInfo info1 = isRowPivot ? PivotEngine.PivotValues[i][index - 1] : PivotEngine.PivotValues[index - 1][i];
                    if (info1.CellRange != null)
                    {
                        sectionEnd = isRowPivot ? sectionStart + info1.CellRange.Bottom - info1.CellRange.Top + 1
                            : sectionStart + info1.CellRange.Right - info1.CellRange.Left + 1;
                    }
                }

                SetIndexedPivotSort(sectionStart, sectionEnd, index, isRowPivot, sortDir);
                i = ++sectionEnd;
                while (i < end)
                {
                    PivotCellInfo info = isRowPivot ? PivotEngine.PivotValues[i][index - 1] : PivotEngine.PivotValues[index - 1][i]; //use minus one because that is where the expander is
                    if ((info.CellType & PivotCellType.ExpanderCell) != 0 && info.UniqueText != "x")
                    {
                        break;
                    }
                    i++;
                    sectionEnd++;
                }
            }

            if (!isLastPivot || (!isRowPivot && PivotEngine.PivotCalculations.Count > 1))
            {
                ResetCoveredCells();
            }

            if (hiddenSubTotals.Count > 0)
            {
                foreach (PivotItem pi in hiddenSubTotals)
                {
                    pi.ShowSubTotal = false;
                    if (this.ShowSubTotals)
                        this.InternalGrid.SubTotalVisibilityRenderer(pi);
                }
            }

            if (needToCollapse.Count > 0)
            {
                this.InternalGrid.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        foreach (string s in needToCollapse)
                        {
                            if (isRowPivot)
                                CollapseRow(s);
                            else
                                CollapseColumn(s);
                        }
                    }), null);
            }
            this.InternalGrid.InvalidateCells();
        }


        void SetIndexedPivotSort(int start, int limit, int index, bool isRowPivot, ListSortDirection dir)   
        {
            bool isLastPivot = index == (isRowPivot ? PivotRows.Count : PivotColumns.Count) - 1;
            int count = limit - start + 1;
            
            List<List<PivotCellInfo>> saveList = new List<List<PivotCellInfo>>(count);
            if (isRowPivot)
            {
                for (int i = start; i < limit; ++i)
                {
                    saveList.Add(new List<PivotCellInfo>(PivotEngine.PivotValues[i].ToArray<PivotCellInfo>()));
                }
            }
            else
            {
                for (int i = start; i < limit; ++i)
                {
                    List<PivotCellInfo> temp = new List<PivotCellInfo>();
                    for (int k1 = 0; k1 < PivotEngine.RowCount; ++k1)
                    {
                        temp.Add(PivotEngine[k1, i]);
                    }
                    saveList.Add(temp);
                }
            }

            List<PivotEngine.SortKeys> listTemp = new List<PivotEngine.SortKeys>();
            int k = start;

            while (k < limit - 1 || (k < limit && isLastPivot))
            {
                PivotCellInfo info = null;
                int size = 0;
                bool include = true;
                if (isRowPivot)
                {
                        info = PivotEngine[k, index];
                    if (isLastPivot && (info.CellType & PivotCellType.RowHeaderCell) != 0)
                    {
                        size = 0;
                    }
                    else if ((info.CellType & PivotCellType.ExpanderCell) == 0)
                    {
                        break;
                    }
                    else
                    {
                        size = info.CellRange.Bottom - info.CellRange.Top + 1;
                    }
                    listTemp.Add(new PivotEngine.SortKeys() { BlockSize = size, Keys = new IComparable[] { info.FormattedText }, Index = k - start });
                    k += size + 1;
                }
                else
                {
                        info = PivotEngine[index, k];
                    if (isLastPivot && (info.CellType & PivotCellType.ColumnHeaderCell) != 0)
                    {
                        size = 1;
                        if (PivotEngine.PivotCalculations.Count > 1)
                        {
                            size = (info.CellRange.Right - info.CellRange.Left) + 1;
                        }
                    }
                    else
                    {
                        if (info.CellRange == null)
                        {
                            size = Math.Max(1, PivotEngine.PivotCalculations.Count); 
                            include = false;
                        }
                        else
                        {
                                size = info.CellRange.Right - info.CellRange.Left + 1 + PivotEngine.PivotCalculations.Count;
                           // size = info.CellRange.Right - info.CellRange.Left + 1;
                        }
                    }
                    if (include)
                    {
                        listTemp.Add(new PivotEngine.SortKeys() { BlockSize = size, Keys = new IComparable[] { info.FormattedText }, Index = k - start });
                    }
                    k += size;
                }
            }

            List<ListSortDirection> dirs = new List<ListSortDirection>();
            dirs.Add(dir);
            PivotEngine.CalcSortComparer comparer = new PivotEngine.CalcSortComparer(dirs);

            listTemp.Sort(comparer);

            if (isRowPivot)
            {
                MoveRows(saveList, listTemp, start, limit, index);
            }
            else
            {
                MoveColumns(saveList, listTemp, start, limit, index);
            }
        }

        private void MoveColumns(List<List<PivotCellInfo>> saveList, List<PivotEngine.SortKeys> listTemp, int start, int limit, int index)
        {
            int i = start;
            int savedCount = saveList.Count;
            foreach (PivotEngine.SortKeys key in listTemp)
            {
                int j = key.Index;

                for (int k = 0; k < key.BlockSize; ++k)
                {
                    for (int k1 = index; k1 < PivotEngine.RowCount; ++k1)
                    {
                        PivotEngine.PivotValues[k1][i] = saveList[j][k1];
                    }
                    int ct = PivotColumns.Count - (PivotEngine.PivotCalculations.Count <= 1 ? 1 : 0);
                    for (int m = index; m < ct; ++m)
                    {
                        PivotCellInfo info = PivotEngine.PivotValues[m][i];
                        if (info.CellRange != null && info.UniqueText != "x")
                        {
                            int offset = i - info.CellRange.Left;
                            info.CellRange.Left += offset;
                            info.CellRange.Right += offset;
                            PivotEngine.CoveredRanges.Add(info.CellRange);
                        }
                    }
                    i++;
                    j++;
                }
            }
        }

        private void MoveRows(List<List<PivotCellInfo>> saveList, List<PivotEngine.SortKeys> listTemp, int start, int limit, int index)
        {
            int i = start;

            PivotCellInfo[] temp = new PivotCellInfo[index];
            int savedCount = saveList.Count;
            foreach (PivotEngine.SortKeys key in listTemp)
            {
                int j = key.Index;

                PivotEngine.PivotValues[i].CopyTo(0, temp, 0, index);

                for (int k = 0; k < index; ++k)
                {
                    saveList[j][k] = temp[k];
                }

                for (int k = 0; k <= key.BlockSize; ++k)
                {
                    PivotEngine.PivotValues[i] = saveList[j];
                    for (int m = index; m < PivotRows.Count - 1; ++m)
                    {
                        PivotCellInfo info = PivotEngine.PivotValues[i][m];
                        if (info.CellRange != null && info.UniqueText != "x")
                        {
                            int offset = i - info.CellRange.Top;
                            info.CellRange.Top += offset;
                            info.CellRange.Bottom += offset;

                            PivotEngine.CoveredRanges.Add(info.CellRange);
                        }
                    }
                    i++;
                    j++;
                }
            }
        }

        private void ClearCoveredCellsIn(int start, int limit, int index, bool isRowPivot)
        {
            List<CoveredCellRange> list = new List<CoveredCellRange>(); ;
            if (isRowPivot)
            {
                int bottomLimit = PivotEngine.RowCount - 2;
                foreach (CoveredCellRange x in PivotEngine.CoveredRanges)
                {
                    if (x.Left >= index && x.Top >= start && x.Bottom < bottomLimit)
                    {
                        list.Add(x);
                    }
                }
                foreach (CoveredCellRange x in list)
                {
                    PivotEngine.CoveredRanges.Remove(x);
                }
            }
            else
            {
                int rightLimit = PivotEngine.ColumnCount - PivotEngine.PivotCalculations.Count;
                foreach (CoveredCellRange x in PivotEngine.CoveredRanges)
                {
                    if (x.Top >= index && x.Left >= start && x.Right < rightLimit)
                    {
                        list.Add(x);
                    }
                }
                foreach (CoveredCellRange x in list)
                {
                    PivotEngine.CoveredRanges.Remove(x);
                }
            }
        }

        #endregion

        /// <summary>
        /// Builds a pivot using the current schema on a background thread so the UI thread is responsive.
        /// </summary>
        public void LoadPivotInBackground()
        {
            this.InternalGrid.LoadPivotInBackground();
        }


        void SynchronizePivotItems(System.Collections.Specialized.NotifyCollectionChangedEventArgs e, bool isRow)
        {
            if (this.IsExternalEngine) return;
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    {
                        foreach (PivotItem item in e.NewItems)
                        {
                            if (isRow)
                            {
                                this.PivotEngine.InsertRowPivot(e.NewStartingIndex, item);
                                item.PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);
                            }
                            else
                            {
                                this.PivotEngine.InsertColumnPivot(e.NewStartingIndex, item);
                                item.PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);
                            }
                        }
                        break;
                    }
#if !SILVERLIGHT
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    {
                        foreach (PivotItem item in e.OldItems)
                        {
                            if (isRow)
                                this.PivotEngine.RemoveRowPivot(item);
                            else
                                this.PivotEngine.RemoveColumnPivot(item);
                        }

                        foreach (PivotItem item in e.NewItems)
                        {
                            if (isRow)
                            {
                                //this.PivotEngine.AddRowPivot(item);
                                this.PivotEngine.InsertRowPivot(e.NewStartingIndex, item);
                            }
                            else
                            {
                                this.PivotEngine.InsertColumnPivot(e.NewStartingIndex, item);
                                //this.PivotEngine.AddColumnPivot(item);     
                            }

                        }
                    }
                    break;
#endif
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    {
                        foreach (PivotItem item in e.OldItems)
                        {
                            if (isRow)
                            {
                                this.PivotEngine.RemoveRowPivot(item);
                                item.PropertyChanged -= new PropertyChangedEventHandler(item_PropertyChanged);
                            }
                            else
                            {
                                this.PivotEngine.RemoveColumnPivot(item);
                                item.PropertyChanged -= new PropertyChangedEventHandler(item_PropertyChanged);
                            }
                        }
                        break;
                    }
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    {
                        if (isRow)
                            this.PivotEngine.PivotRows = this.PivotRows.ToList<PivotItem>();
                        else
                            this.PivotEngine.PivotColumns = this.PivotColumns.ToList<PivotItem>();
                        break;
                    }
                default:
                    break;
            }
        }

        void SynchronizeCalculations(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.IsExternalEngine) return;
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    {
                        foreach (PivotComputationInfo item in e.NewItems)
                        {
                            //this.PivotEngine.AddPivotCalculation(item);
                            if (String.IsNullOrEmpty(item.FieldHeader)) item.FieldHeader = item.FieldName;
                            this.PivotEngine.InsertPivotCalculation(e.NewStartingIndex, item);
                        }
                        break;
                    }
#if !SILVERLIGHT
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    {
                        foreach (PivotComputationInfo item in e.NewItems)
                        {
                            this.PivotEngine.RemovePivotCalculation(item);
                        }

                        foreach (PivotComputationInfo item in e.NewItems)
                        {
                            //this.PivotEngine.AddPivotCalculation(item);
                            this.PivotEngine.InsertPivotCalculation(e.NewStartingIndex, item);
                        }
                    }
                    break;
#endif

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    {
                        foreach (PivotComputationInfo item in e.OldItems)
                        {
                            this.PivotEngine.RemovePivotCalculation(item);
                        }
                        break;
                    }
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    {
                        this.PivotEngine.PivotCalculations = this.PivotCalculations.ToList<PivotComputationInfo>();
                        break;
                    }
                default:
                    break;
            }
        }
       /// <summary>
       ///  to update the filter item in Grouping Bar Area
       /// </summary>
       /// <param name="index">int</param>
       /// <param name="dimensionName">string</param>
        
        void UpdateGroupingBarFilter(int index,string dimensionName)
        {
            for (int i = 0; i < this.PivotColumns.Count; i++)
            {
                if (this.PivotColumns[i].FieldMappingName == dimensionName)
                {
                    this.PivotColumns.RemoveAt(i);
                    break;
                }
            }
            for (int i = 0; i < this.PivotRows.Count; i++)
            {
                if (this.PivotRows[i].FieldMappingName == dimensionName)
                {
                    this.PivotRows.RemoveAt(i);
                    break;
                }
            }
            for (int i = 0; i < this.PivotCalculations.Count; i++)
            {
                if (this.PivotCalculations[i].FieldName == dimensionName)
                {
                    this.PivotCalculations.RemoveAt(i);
                    break;
                }
            }
            if (this.GroupingBar == null)
                this.ApplyTemplate();
            if (this.GroupingBar != null)
            {
                itemCollection = PivotEngine.ItemCollection;
                if (!this.GroupingBar.Filters.Any(o => o.Name == itemCollection.Name) && index >= 0)
                {
                    this.GroupingBar.Filters.Insert(index, itemCollection);
                    if (this.GroupingBar.Filters[index].DisplayHeader == null)
                        this.GroupingBar.Filters[index].DisplayHeader = this.GroupingBar.Filters[index].Name;
                   if (this.GroupingBar.FilterHeaderArea != null && (this.GroupingBar.FilterHeaderArea.DataContext == null || this.GroupingBar.FilterHeaderArea.ItemsSource == null))
                    {
                        this.GroupingBar.FilterHeaderArea.Items.Clear();
                        this.GroupingBar.FilterHeaderArea.DataContext = this.GroupingBar.Filters;
                        this.GroupingBar.FilterHeaderArea.ItemsSource = this.GroupingBar.Filters;

                        ResourceDictionary resource = new ResourceDictionary()
                        {
#if !SILVERLIGHT
                            Source = new Uri("/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
#else
                            Source = new Uri("/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
#endif
                        };
                        this.GroupingBar.FilterHeaderArea.ItemTemplate = resource["PivotFilterItemTemplate"] as DataTemplate;

                    }
                }

            }
        }
        
        FilterItemsCollection itemCollection = null;
        /// <summary>
        /// Variable to maintain the current index of the filter
        /// </summary>
        public static int FilterIndex;
        int index = 0;

        void SynchronizeFilters(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.IsExternalEngine) return;
            switch (e.Action)
            {
               case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    { 
                       
                        string DimensionName=(e.NewItems[0] as FilterExpression).DimensionName;
                        string Expression=(e.NewItems[0] as FilterExpression).Expression;
                        if ((!(this.Filters.Any(o => o.DimensionName == DimensionName)) || !(this.Filters.Any(o=>o.Name == DimensionName))) && ((Expression==null)||(Expression=="")))
                        {
                            if ((DimensionName != "") && (DimensionName != null))
                            {
                                foreach (FilterExpression item in e.NewItems)
                                {
                                    if (this.GroupingBar != null && this.GroupingBar.Filters != null && e.NewStartingIndex != this.GroupingBar.Filters.Count)
                                        FilterIndex = this.GroupingBar.Filters.Count;
                                    else
                                        FilterIndex = e.NewStartingIndex - index;
                                    this.PivotEngine.AddFilter(item);
                                    if (FilterIndex >= e.NewStartingIndex)
                                        this.UpdateGroupingBarFilter(e.NewStartingIndex, DimensionName);
                                    else
                                        this.UpdateGroupingBarFilter(FilterIndex, DimensionName);
                                    if (string.IsNullOrEmpty(item.Expression) && this.PivotEngine.ItemCollection != null)
                                        item.Expression = this.PivotEngine.ItemCollection.GetFilterExpression(true);
#if !SILVERLIGHT
                                    if ((!this.PivotEngine.EnableOnDemandCalculations || this.PivotEngine.UseIndexedEngine) || (!this.PivotEngine.Filters.Any(x=>x.Name == item.DimensionHeader || x.Name == item.Name) && this.GroupingBar.Filters.Any(x => x.Name == item.DimensionHeader || x.Name == item.Name)))
#endif
                                    this.PivotEngine.InsertFilter(e.NewStartingIndex, item);
                                    this.PivotEngine.RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs() { ChangeHints = SchemaChangeHints.CalculationChanged });
                                }

                            }
                        }

                        else if (this.GroupingBar != null && !(this.GroupingBar.Filters.Any(o => o.Name == DimensionName)) && Expression != "")
                        {
                            foreach (FilterExpression item in e.NewItems)
                            {
                                if (item.Expression == null)
                                {
                                    this.PivotEngine.AddFilter(item);
                                    item.Expression = this.PivotEngine.ItemCollection.GetFilterExpression(true);
                                }
                                if (!(this.PivotEngine.Filters.Any(o => o.Expression == item.Expression)))
                                {
#if !SILVERLIGHT
                                    if ((!this.PivotEngine.EnableOnDemandCalculations || this.PivotEngine.UseIndexedEngine) || (!this.PivotEngine.Filters.Any(x => x.Name == item.DimensionHeader || x.Name == item.Name) && this.GroupingBar.Filters.Any(x => x.Name == item.DimensionHeader || x.Name == item.Name)))
#endif
                                    this.PivotEngine.InsertFilter(PivotEngine.Filters.Count, item);
                                }
                                if (this.GroupingBar != null && !(this.GroupingBar.Filters.Any(o => o.Name == item.DimensionName)) && (item.DimensionName != null) && (item.DimensionHeader != null))
                                {
                                    if (FilterIndex >= e.NewStartingIndex)
                                        this.UpdateGroupingBarFilter(e.NewStartingIndex, DimensionName);
                                    else
                                        this.UpdateGroupingBarFilter(FilterIndex, DimensionName);
                                }
                                else
                                    index++;
                            }
                        }

                        else if (Expression != "")
                        {
                            foreach (FilterExpression item in e.NewItems)
                            {
                                if (item.Expression == null)
                                {
                                    this.PivotEngine.AddFilter(item);
                                    item.Expression = this.PivotEngine.ItemCollection.GetFilterExpression(true);
                                }
                                if (!(this.PivotEngine.Filters.Any(o => o.Expression == item.Expression)))
                                {
                                    this.PivotEngine.InsertFilter(e.NewStartingIndex, item);
                                }
                            }
                        }


    
                        break;
                    }
#if !SILVERLIGHT
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
#endif

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    {

                        if((((e.OldItems[0] as FilterExpression).DimensionName != "")&& ((e.OldItems[0] as FilterExpression).DimensionName !=null))||((e.OldItems[0] as FilterExpression).DimensionHeader!=null))
                        {
                            foreach (FilterExpression item in e.OldItems)
                            {
                                if (this.GroupingBar != null)
                                {
                                    FilterItemsCollection itemCollection = this.GroupingBar.Filters.Where(p => p.Name == item.DimensionHeader).FirstOrDefault();
                                    if(itemCollection == null)
                                   itemCollection = this.GroupingBar.Filters.Where(p => p.Name == item.DimensionName).FirstOrDefault();
                                 
                                    if (itemCollection == null)
                                        itemCollection = this.GroupingBar.Filters.Where(p => p.Name == item.Name).FirstOrDefault();

                                    if (itemCollection != null)
                                    {
                                        this.GroupingBar.Filters.Remove(itemCollection);
                                        if (this.GroupingBar.Filters.Count == 0)
                                            this.GroupingBar.ApplyEmptyTemplate();

                                    }
                                }
                                    this.PivotEngine.RemoveFilter(item);
                                    this.Filters.Remove(item);
                            }
                           
                        }
                        else if ((e.OldItems[0] as FilterExpression).Expression != "")
                        {
                            foreach (FilterExpression item in e.OldItems)
                            {
                                this.PivotEngine.RemoveFilter(item);
                            }
                        }
                        break;
                    }
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    {
                            index = 0;
                            this.PivotEngine.Filters.Clear();
                            if (GroupingBar != null)
                            {
                                this.GroupingBar.Filters.Clear();
                                this.GroupingBar.ApplyEmptyTemplate();
                                FilterIndex = 0;
                            }
                            this.PivotEngine.RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs() { ChangeHints = SchemaChangeHints.CalculationChanged });
                     break;
                    }
                default:
                    break;
            }
            if (EnableUpdating)
            {
                UpdateManager.UnwireEvents();
                UpdateManager.WireEvents();
            }
        }

        /// <summary>
        /// Raises the Grid Selection event.
        /// </summary>
        /// <param name="SelectionChangingEventArgs">The <see cref="Syncfusion.Windows.Controls.PivotGrid.PivotGridSelectionChangedEventArgs"/> instance containing the event data.</param>
        internal void RaiseSelectionEvent(PivotGridSelectionChangedEventArgs SelectionChangingEventArgs)
        {
            if (SelectionChanged != null)
            {
                this.SelectedItems = SelectionChangingEventArgs.SelectedItems;
                SelectionChanged(this, SelectionChangingEventArgs);
            }
        }

        /// <summary>
        /// Raises the OnExpanding event.
        /// </summary>
        /// <param name="expandingEventArgs">The <see cref="Syncfusion.Windows.Controls.PivotGrid.ExpandingEventArgs"/> instance containing the event data.</param>
        /// <param name="OriginalCell">Holds pivot cell information</param>
        public void RaiseOnExpanding(ExpandingEventArgs expandingEventArgs, PivotCellInfo OriginalCell)
        {
            GridStyleInfo styleInfo = expandingEventArgs.PivotCellInfo.Tag as GridStyleInfo;
            if (Expanding != null)
            {
                Expanding(this, expandingEventArgs);
                if (!expandingEventArgs.Cancel)
                {
                    styleInfo = expandingEventArgs.PivotCellInfo.Tag as GridStyleInfo;
                    this.InternalGrid.ExpandGroup(expandingEventArgs.PivotCellInfo, styleInfo);
                    this.InternalGrid.InvalidateCells();
                    this.RaiseOnExpanded(new ExpandedEventArgs(OriginalCell));
                }
            }
            else
            {
                styleInfo = expandingEventArgs.PivotCellInfo.Tag as GridStyleInfo;
                this.InternalGrid.ExpandGroup(expandingEventArgs.PivotCellInfo, styleInfo);
                this.InternalGrid.InvalidateCells();
                this.RaiseOnExpanded(new ExpandedEventArgs(OriginalCell));
            }
            if (this.Filters.Count > 0)
            {
                this.InternalGrid.ApplyPivotRowFilters();
                this.InternalGrid.ApplyPivotColumnFilters();
            }
        }

        /// <summary>
        /// Raises the on OnExpanded Event.
        /// </summary>
        /// <param name="expandedEventArgs">The <see cref="Syncfusion.Windows.Controls.PivotGrid.ExpandedEventArgs"/> instance containing the event data.</param>
        internal void RaiseOnExpanded(ExpandedEventArgs expandedEventArgs)
        {
            if (Expanded != null)
            {
                Expanded(this, expandedEventArgs);
            }

            if (this.ResizePivotGridToFit)
                this.PivotGridReCalculateSize();

        }
        /// <summary>
        /// Raises the FilterActionCompleted Event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.PivotGrid.FilterActionCompletedEventArgs"/> instance containing the event data.</param>
        public void RaiseFilterActionCompleted(FilterActionCompletedEventArgs e)
        {
            if (FilterActionCompleted != null)
            {
                FilterActionCompleted(this, e);
            }
        }

        /// <summary>
        /// Raises the OnCollapsing Event.
        /// </summary>
        /// <param name="collapsingEventArgs">The <see cref="Syncfusion.Windows.Controls.PivotGrid.CollapsingEventArgs"/> instance containing the event data.</param>
        /// <param name="styleInfo">The style info.</param>
        public void RaiseOnCollapsing(CollapsingEventArgs collapsingEventArgs, GridStyleInfo styleInfo)
        {
            if (Collapsing != null)
            {
                Collapsing(this, collapsingEventArgs);
                if (!collapsingEventArgs.Cancel)
                {
                    this.InternalGrid.CollapseGroup(collapsingEventArgs.PivotCellInfo, styleInfo);
                    this.InternalGrid.InvalidateCells();
                    this.RaiseOnCollapsed(new CollapsedEventArgs(collapsingEventArgs.PivotCellInfo));
                }
            }
            else
            {
                this.InternalGrid.CollapseGroup(collapsingEventArgs.PivotCellInfo, styleInfo);
                this.InternalGrid.InvalidateCells();
                this.RaiseOnCollapsed(new CollapsedEventArgs(collapsingEventArgs.PivotCellInfo));
            }
        }

        /// <summary>
        /// Raises the OnCollapsed Event.
        /// </summary>
        /// <param name="collapsedEventArgs">The <see cref="Syncfusion.Windows.Controls.PivotGrid.CollapsedEventArgs"/> instance containing the event data.</param>
        internal void RaiseOnCollapsed(CollapsedEventArgs collapsedEventArgs)
        {
            if (Collapsed != null)
            {
                Collapsed(this, collapsedEventArgs);
            }

            if (this.ResizePivotGridToFit)
                this.PivotGridReCalculateSize();

        }

        /// <summary>
        /// Raises the hyperlink cell click.
        /// </summary>
        /// <param name="hyperlinkCellClickEventArgs">The <see cref="Syncfusion.Windows.Controls.PivotGrid.HyperlinkCellClickEventArgs"/> instance containing the event data.</param>
        internal void RaiseHyperlinkCellClick(HyperlinkCellClickEventArgs hyperlinkCellClickEventArgs)
        {
            if (HyperlinkCellClick != null)
            {
                this.SelectedCell = hyperlinkCellClickEventArgs.PivotCellInfo;
                HyperlinkCellClick(this, hyperlinkCellClickEventArgs);
            }
        }

        /// <summary>
        /// Handles the PropertyChanged event of the CellStyle control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        public void CellStyle_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.InvalidateCells();
        }

        internal void RaiseDataRefreshing(DataRefreshingArgs e)
        {
            if (DataRefreshing != null)
            {
                this.DataRefreshing(this, e);
            }
        }


        internal void RaiseDataRefreshed(DataRefreshedArgs e)
        {
            if (DataRefreshed != null)
            {
                this.DataRefreshed(this, e);
            }

            if (this.ResizePivotGridToFit)
                this.PivotGridReCalculateSize();
            if (this.InternalGrid != null && !this.ShowSubTotals)
                this.InternalGrid.SubTotalsRendering();

        }

        void GroupingBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (GroupingBarLoaded != null)
            {
                GroupingBarLoaded(this, new EventArgs());
            }
            if (this.Filters.Count > 0)
            {
                for (int i = 0; i < Filters.Count; i++)
                {
                    FilterExpression item = Filters[i];
                    this.InternalGrid.PivotEngine.AddFilter(item);
                    if (PivotEngine.ItemCollection == null)
                        return;
                    if (!this.GroupingBar.Filters.Any(x=>x.DisplayHeader== PivotEngine.ItemCollection.DisplayHeader))
                    this.GroupingBar.Filters.Insert(i, PivotEngine.ItemCollection);
                    if (this.GroupingBar.Filters[i].DisplayHeader == null)
                        this.GroupingBar.Filters[i].DisplayHeader = this.GroupingBar.Filters[i].Name;
                    if (this.GroupingBar.FilterHeaderArea != null && (this.GroupingBar.FilterHeaderArea.DataContext == null || this.GroupingBar.FilterHeaderArea.ItemsSource == null))
                    {
                        this.GroupingBar.FilterHeaderArea.Items.Clear();
                        this.GroupingBar.FilterHeaderArea.DataContext = this.GroupingBar.Filters;
                        this.GroupingBar.FilterHeaderArea.ItemsSource = this.GroupingBar.Filters;

                        ResourceDictionary resource = new ResourceDictionary()
                        {
#if !SILVERLIGHT
                            Source = new Uri("/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
#else
                            Source = new Uri("/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
#endif
                        };
                        this.GroupingBar.FilterHeaderArea.ItemTemplate = resource["PivotFilterItemTemplate"] as DataTemplate;

                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Serializes some specific properties in PivotGridControl
        /// </summary>
        public void Serialize()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PivotGridSerializer));
                SaveFileDialog sfv = new SaveFileDialog();
                sfv.DefaultExt = ".xml";
                sfv.Filter = "(*.xml)|*.xml";
                if (sfv.ShowDialog() == true)
                {
#if SILVERLIGHT
                    using (var sw = new StreamWriter(sfv.OpenFile()))
#else
                    using (var sw = new StreamWriter(sfv.FileName))
#endif
                    {
                        PivotGridSerializer gridSerializer = new PivotGridSerializer(this);
                        gridSerializer.Wrap();
                        serializer.Serialize(sw, gridSerializer);
                    }
                }
            }
            catch (Exception ex)
            {
#if !SILVERLIGHT
                MessageBox.Show(ex.Message, "Serialization Error", MessageBoxButton.OK, MessageBoxImage.Error); 
#else
                MessageBox.Show(ex.Message);
#endif
            }
        }

        /// <summary>
        /// Serializes some specific properties in PivotGridControl into string format
        /// </summary>
        public String SerializedXmlString()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PivotGridSerializer));
                StringWriter stringWriter = new StringWriter();
                PivotGridSerializer gridSerializer = new PivotGridSerializer(this);
                gridSerializer.Wrap();
                serializer.Serialize(stringWriter, gridSerializer);
                return stringWriter.ToString();
            }
            catch (Exception ex)
            {
#if !SILVERLIGHT
                MessageBox.Show(ex.Message, "Serialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
#else
                MessageBox.Show(ex.Message);
#endif
            }
            return null;
        }

#if !SILVERLIGHT

        /// <summary>
        /// Serializes the specific properties in PivotGridControl and saves it with the specified file name
        /// </summary>
        public void Serialize(string fileName)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PivotGridSerializer));
                using (var sw = new StreamWriter(fileName))
                {
                    PivotGridSerializer gridSerializer = new PivotGridSerializer(this);
                    gridSerializer.Wrap();
                    serializer.Serialize(sw, gridSerializer);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Serialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

#endif

        /// <summary>
        /// De-serializes the XML file format into PivotGridControl
        /// </summary>
        public void Deserialize()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PivotGridSerializer));
                OpenFileDialog ofd = new OpenFileDialog();
#if !SILVERLIGHT
                ofd.DefaultExt = ".xml";
#else
                ofd.Multiselect = false;
#endif
                ofd.Filter = "(*.xml)|*.xml";

                if (ofd.ShowDialog() == true)
                {
#if SILVERLIGHT
                    using (var sw = new StreamReader(ofd.File.OpenRead()))
#else
                    using (var sw = new StreamReader(ofd.FileName))
#endif
                    {
                        PivotGridSerializer gridSerializer = serializer.Deserialize(sw) as PivotGridSerializer;
                        gridSerializer.WrapGridControl(this);
                    }
                }
                //return null;
            }
            catch (Exception ex)
            {
#if !SILVERLIGHT
                MessageBox.Show(ex.Message, "De-Serialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
#else
                MessageBox.Show(ex.Message);
#endif
            }
        }

        /// <summary>
        /// De-serializes the XML string format into PivotGridControl
        /// </summary>
        public void DeserializeXmlString(string xmlString)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PivotGridSerializer));
                PivotGridSerializer gridSerializer = serializer.Deserialize(new StringReader(xmlString)) as PivotGridSerializer;
                gridSerializer.WrapGridControl(this);
                this.PivotEngine.RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
            }
            catch (Exception ex)
            {
#if !SILVERLIGHT
                MessageBox.Show(ex.Message, "De-Serialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
#else
                MessageBox.Show(ex.Message);
#endif
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// De-serializes the given file name into PivotGridControl
        /// </summary>
        public void Deserialize(string fileName)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PivotGridSerializer));
                using (var sw = new StreamReader(fileName))
                {                    
                    PivotGridSerializer gridSerializer = serializer.Deserialize(sw) as PivotGridSerializer;
                    gridSerializer.WrapGridControl(this);
                    //this.PivotEngine.RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "De-Serialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
#endif

    }
    /// <summary>
    /// Specifies the sorting options for PivotGrid Control
    /// </summary>
    public enum PivotSortOption
    {
        /// <summary>
        /// No sorting
        /// </summary>
        None,
        /// <summary>
        /// Sorting All columns
        /// </summary>
        All,
        /// <summary>
        /// Sorting at the columns (not total or grandtotal columns)
        /// </summary>
        ColumnSorting,
        /// <summary>
        /// Sorting at Total cells columns
        /// </summary>
        TotalSorting,
        /// <summary>
        /// Sorting at Grand total cells columns
        /// </summary>
        GrandTotalSorting
        
        
    }
    /// <summary>
    /// Specifies the Visual Style for PivotGridControl
    /// </summary>
    public enum PivotGridVisualStyle
    {
        /// <summary>
        /// Provide Blend Style for PivotGridControl
        /// </summary>
        Blend,
        /// <summary>
        /// Provide Default Style for PivotGridControl
        /// </summary>
        Default,
        /// <summary>
        /// Provide Metro Style for PivotGridControl
        /// </summary>
        Metro,
#if !SILVERLIGHT
        /// <summary>
        /// Provide Office2003 Style for PivotGridControl
        /// </summary>
        Office2003,
#endif
        /// <summary>
        /// Provide Office2007Black Style for PivotGridControl
        /// </summary>
        Office2007Black,
        /// <summary>
        /// Provide Office2007Blue Style for PivotGridControl
        /// </summary>
        Office2007Blue,
        /// <summary>
        /// Provide Office2007Silver Style for PivotGridControl
        /// </summary>
        Office2007Silver,
        /// <summary>
        /// Provide Office2010Black style for PivotGridControl
        /// </summary>
        Office2010Black,
        /// <summary>
        /// Provide Office2010Blue style for PivotGridControl
        /// </summary>
        Office2010Blue,
        /// <summary>
        /// Provide Office2010Silver style for PivotGridControl
        /// </summary>
        Office2010Silver,
        /// <summary>
        /// Provides transparent style for PivotGridControl
        /// </summary>
        Transparent
    }
}