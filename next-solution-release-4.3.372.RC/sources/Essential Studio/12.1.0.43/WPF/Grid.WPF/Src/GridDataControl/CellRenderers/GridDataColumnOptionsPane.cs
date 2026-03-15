#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Controls;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Controls.Primitives;
    using Syncfusion.Windows.Shared;

#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class GridDataColumnOptionsPane : Control
    {
        private string[] dateTimeFormats = new string[] 
        {
          "(none)",
          "d",
          "D",
          "f",
          "dddd, dd MMMM yyyy",
          "t",
          "s"
        };

        private string[] textFormats = new string[] 
        {
          "(none)",
          "0.00",
          "C",
          "0.00;(0.00)",
          "###0.##%",
          "#0.#E+00",
          "10:##,##0.#"
        };

        static GridDataColumnOptionsPane()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridDataColumnOptionsPane), new FrameworkPropertyMetadata(typeof(GridDataColumnOptionsPane)));
        }

        public static readonly DependencyProperty ColumnOptionsButtonBackgroundProperty = DependencyProperty.Register(
            "ColumnOptionsButtonBackground",
            typeof(Brush),
            typeof(GridDataColumnOptionsPane));

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return (Brush)this.GetValue(GridDataColumnOptionsPane.ColumnOptionsButtonBackgroundProperty);
            }

            set
            {
                this.SetValue(GridDataColumnOptionsPane.ColumnOptionsButtonBackgroundProperty, value);
            }
        }



        public Brush ColumnOptionsButtonBorderBrush
        {
            get { return (Brush)this.GetValue(GridDataColumnOptionsPane.ColumnOptionsButtonBorderBrushProperty); }
            set { this.SetValue(GridDataColumnOptionsPane.ColumnOptionsButtonBorderBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnOptionsButtonBorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnOptionsButtonBorderBrushProperty =
            DependencyProperty.Register("ColumnOptionsButtonBorderBrush", typeof(Brush), typeof(GridDataColumnOptionsPane));

        

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(GridDataColumnOptionsPane));

        public string Text
        {
            get
            {
                return (string)this.GetValue(GridDataColumnOptionsPane.TextProperty);
            }

            set
            {
                this.SetValue(GridDataColumnOptionsPane.TextProperty, value);
            }
        }

        public static readonly DependencyProperty FilterButtonVisibilityProperty = DependencyProperty.Register(
            "FilterButtonVisibility",
            typeof(Visibility),
            typeof(GridDataColumnOptionsPane));

        public Visibility FilterButtonVisibility
        {
            get
            {
                return (Visibility)this.GetValue(GridDataColumnOptionsPane.FilterButtonVisibilityProperty);
            }

            set
            {
                this.SetValue(GridDataColumnOptionsPane.FilterButtonVisibilityProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for HAlignment.
        /// </summary>
        public static readonly DependencyProperty HAlignmentProperty = DependencyProperty.Register(
            "HAlignment",
            typeof(HorizontalAlignment),
            typeof(GridDataColumnOptionsPane),
            new FrameworkPropertyMetadata(HorizontalAlignment.Left, new PropertyChangedCallback(OnHAlignmentChanged)));

        private static void OnHAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataColumnOptionsPane columnOptionsPane = d as GridDataColumnOptionsPane;
            if (!columnOptionsPane.HeaderCell.IsInSuspend)
            {
                columnOptionsPane.VisibleColumn.ColumnStyle.HorizontalAlignment = (HorizontalAlignment)args.NewValue;
                columnOptionsPane.VisibleColumn.TableModel.InvalidateCell(GridRangeInfo.Col(columnOptionsPane.RenderStyle.ColumnIndex));
            }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment of the column text.
        /// </summary>
        public HorizontalAlignment HAlignment
        {
            get
            {
                return (HorizontalAlignment)this.GetValue(GridDataColumnOptionsPane.HAlignmentProperty);
            }

            set
            {
                this.SetValue(GridDataColumnOptionsPane.HAlignmentProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for HAlignment.
        /// </summary>
        public static readonly DependencyProperty VAlignmentProperty = DependencyProperty.Register(
            "VAlignment",
            typeof(VerticalAlignment),
            typeof(GridDataColumnOptionsPane),
            new FrameworkPropertyMetadata(VerticalAlignment.Center, new PropertyChangedCallback(OnVAlignmentChanged)));

        /// <summary>
        /// Gets or sets the vertical alignment of the column text.
        /// </summary>
        public VerticalAlignment VAlignment
        {
            get
            {
                return (VerticalAlignment)this.GetValue(GridDataColumnOptionsPane.VAlignmentProperty);
            }

            set
            {
                this.SetValue(GridDataColumnOptionsPane.VAlignmentProperty, value);
            }
        }

        private static void OnVAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataColumnOptionsPane columnOptionsPane = d as GridDataColumnOptionsPane;
            if (!columnOptionsPane.HeaderCell.IsInSuspend)
            {
                columnOptionsPane.VisibleColumn.ColumnStyle.VerticalAlignment = (VerticalAlignment)args.NewValue;
                columnOptionsPane.VisibleColumn.TableModel.InvalidateCell(GridRangeInfo.Col(columnOptionsPane.RenderStyle.ColumnIndex));
            }
        }

        
        public static readonly DependencyProperty SetFrozenColumnProperty = DependencyProperty.Register(
            "SetFrozenColumn",
            typeof(bool),
            typeof(GridDataColumnOptionsPane));//,
           // new FrameworkPropertyMetadata( new PropertyChangedCallback(OnSetFrozenColumn)));
        
        public bool SetFrozenColumn
        {
            get
            {
                return (bool)this.GetValue(GridDataColumnOptionsPane.SetFrozenColumnProperty);
            }

            set
            {
                this.SetValue(GridDataColumnOptionsPane.SetFrozenColumnProperty, value);
            }
        }       
        
      

        public event EventHandler TemplateApplied;

        public override void OnApplyTemplate()
        {
            if (this.ApplyButton != null)
            {
                this.ApplyButton.Click -= this.OkButton_Click;
            }

            if (this.ColumnFormatOptions != null)
            {
                this.ColumnFormatOptions.SelectionChanged -= this.OnColumnFormatOptionsSelectionChanged;
            }

            if (this.ChkAutoFit != null)
            {
                this.ChkAutoFit.Checked -= new RoutedEventHandler(ChkAutoFit_Checked);
                this.ChkAutoFit.Unchecked -= new RoutedEventHandler(ChkAutoFit_Unchecked);
            }

            base.OnApplyTemplate();

            this.ApplyButton = this.GetTemplateChild("PART_OkButton") as Button;
            this.ApplyButton.Click += this.OkButton_Click;
            this.WidthSetter = this.GetTemplateChild("PART_WidthSetter") as Slider;
            this.ChkAutoFit = this.GetTemplateChild("PART_ChkAutoFit") as CheckBox;
            this.ColumnFormatOptions = this.GetTemplateChild("PART_ColumnFormatOptions") as ComboBox;

            if (this.ChkAutoFit != null)
            {
                this.ChkAutoFit.Checked += new RoutedEventHandler(ChkAutoFit_Checked);
                this.ChkAutoFit.Unchecked += new RoutedEventHandler(ChkAutoFit_Unchecked);
            }

            if (this.VisibleColumn != null)
            {
                this.WidthSetter.Value = this.VisibleColumn.ActualWidth;//((GridDataControlBaseImpl)this.VisibleColumn.TableModel.Grid).GridDataColumnSizer.ApplyColumnSizer(this.VisibleColumn);
                this.ChkAutoFit.IsChecked = this.VisibleColumn.AutoFit;
                if (this.VisibleColumn.ColumnStyle == null)
                {
                    var tableModel = this.VisibleColumn.TableModel;
                    if (tableModel != null)
                    {
                        this.VisibleColumn.ColumnStyle = new GridDataColumnStyle()
                        {
                            VerticalAlignment = tableModel.TableStyle.VerticalAlignment,
                            HorizontalAlignment = tableModel.TableStyle.HorizontalAlignment
                        };

                    }
                }
              
                if (VisibleColumn.TableModel.Grid.Model.FrozenColumns == this.VisibleColumn.TableModel.CurrentCellState.ColumnIndex+1)
                    SetFrozenColumn = true;

               

                if (this.VisibleColumn.ColumnType != null)
                {
                    if (this.VisibleColumn.ColumnType == typeof(System.DateTime))
                    {
                        if (this.VisibleColumn.ColumnStyle.CellType == "DateTimeEdit")
                        {
                            var patterns = Enum.GetNames(typeof(DateTimePattern));
                            this.ColumnFormatOptions.ItemsSource = patterns;
                            var idx = -1;
                            var currentPattern = this.VisibleColumn.ColumnStyle.DateTimeEdit.DateTimePattern;
                            for (int i = 0; i < patterns.Length; i++)
                            {
                                if (patterns[i] == currentPattern.ToString())
                                {
                                    idx = i;
                                    break;
                                }
                            }
                            this.ColumnFormatOptions.SelectedIndex = idx;
                        }
                        else
                        {
                            this.ColumnFormatOptions.ItemsSource = this.dateTimeFormats;
                        }
                    }
                    else if (this.VisibleColumn.ColumnType == typeof(decimal) || this.VisibleColumn.ColumnType == typeof(double) || this.VisibleColumn.ColumnType == typeof(int))
                    {
                        this.ColumnFormatOptions.ItemsSource = this.textFormats;
                    }
                    else
                    {
                        this.ColumnFormatOptions.IsEnabled = false;
                    }

                    this.ColumnFormatOptions.SelectionChanged += this.OnColumnFormatOptionsSelectionChanged;
                }
            }
            if (this.TemplateApplied != null)
            {
                this.TemplateApplied(this, EventArgs.Empty);
            }
           
          this.LostFocus +=new RoutedEventHandler(GridDataColumnOptionsPane_LostFocus);
        }
        private void GridDataColumnOptionsPane_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!HeaderCell.IsInSuspend && VisibleColumn!=null)
            {
                if (SetFrozenColumn == true)
                {
                    int CurrentcolumnIndex = this.VisibleColumn.TableModel.ResolveVisibleColumnIndexToPosition(this.VisibleColumn.TableModel.TableProperties.VisibleColumns.IndexOf(this.VisibleColumn));
                    VisibleColumn.TableModel.Grid.Model.FrozenColumns = CurrentcolumnIndex + 1;
                }
                else
                {
                    VisibleColumn.TableModel.Grid.Model.FrozenColumns = 0;
                }

            }
        }

        void ChkAutoFit_Unchecked(object sender, RoutedEventArgs e)
        {
            if (this.WidthSetter != null)
                this.WidthSetter.IsEnabled = true;
        }

        void ChkAutoFit_Checked(object sender, RoutedEventArgs e)
        {
            if (this.WidthSetter != null)
                this.WidthSetter.IsEnabled = false;
        }
      
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.HeaderCell.CloseColumnOptionsDropDown();

            if (this.VisibleColumn != null)
            {
                if (this.ChkAutoFit.IsChecked.Value)
                {
                    this.VisibleColumn.AutoFit = this.ChkAutoFit.IsChecked.Value;
                }
                else
                {
                    // var value = (int)this.WidthSetter.Value; Unused local variable
                    this.VisibleColumn.IsInSuspend = true;
                    this.VisibleColumn.AutoFit = false;
                    this.VisibleColumn.IsInSuspend = false;
                    //this.VisibleColumn.Width =new GridLength( this.WidthSetter.Value, GridUnitType.Pixel);
                    this.VisibleColumn.ActualWidth = this.WidthSetter.Value;
                }
            }
        }

        public GridRenderStyleInfo RenderStyle
        {
            get;
            internal set;
        }

        public Button ApplyButton
        {
            get;
            private set;
        }

        public Slider WidthSetter
        {
            get;
            private set;
        }

        public ToggleButton CloseButton
        {
            get;
            private set;
        }

        public CheckBox ChkAutoFit
        {
            get;
            private set;
        }

        public ComboBox ColumnFormatOptions
        {
            get;
            private set;
        }

        internal void SetHeaderCellControl(GridDataHeaderCellControl headerCell)
        {
            this.HeaderCell = headerCell;
        }

        public GridDataHeaderCellControl HeaderCell
        {
            get;
            private set;
        }
        private GridDataVisibleColumn visibleColum;
        public GridDataVisibleColumn VisibleColumn
        {
            get
            {
                return this.visibleColum;
            }
            set
            {
                visibleColum = value;
            }
        }
        
        private void OnColumnFormatOptionsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ColumnFormatOptions.SelectedItem.ToString() == "(none)")
            {
                this.VisibleColumn.ColumnStyle.Format = string.Empty;
            }
            else
            {
                if (this.VisibleColumn.ColumnStyle.CellType == "DateTimeEdit" && this.VisibleColumn.ColumnStyle.DateTimeEdit != null)
                {
                    this.VisibleColumn.ColumnStyle.DateTimeEdit.DateTimePattern = (DateTimePattern)Enum.Parse(typeof(DateTimePattern), this.ColumnFormatOptions.SelectedItem.ToString());
                }
                else
                {
                    this.VisibleColumn.ColumnStyle.Format = this.ColumnFormatOptions.SelectedItem.ToString();
                }
            }

            var range = GridRangeInfo.Col(this.RenderStyle.ColumnIndex);
            range = this.VisibleColumn.TableModel.ExpandRange(range);
            this.VisibleColumn.TableModel.InvalidateCell(range);
            //this.VisibleColumn.TableModel.Data.Remove(range.ToCellSpan());
        }
    }
}
