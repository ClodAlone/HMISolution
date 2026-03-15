#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.OlapSilverlight.Engine;
using System.Windows.Data;
using Syncfusion.Windows.Chart;
using System.Windows.Controls.Primitives;

namespace Syncfusion.Silverlight.Chart.Olap
{
    [TemplatePart(Name = "PART_InnerTextBlock", Type = typeof(TextBlock))]
    public class OlapLabelPresenter : Control
    {
        #region Members

        internal ContentControl m_contentControl = null;
        Border m_PathBorder;
        ToggleButton toggleButton;

        #endregion

        #region Event

        public event OlapLabelClick LabelClick;
        public event OlapLabelClick LabelCellClick;

        #endregion

        #region Initialization

        public OlapLabelPresenter()
        {
            DefaultStyleKey = typeof(OlapLabelPresenter);
        }

        #endregion

        #region Properties

        public PivotCellDescriptor CellDescriptor
        {
            get { return (PivotCellDescriptor)GetValue(CellDescriptorProperty); }
            set { SetValue(CellDescriptorProperty, value); }
        }

        internal double LabelRotationAngle
        {
            get { return (double)GetValue(LabelRotationAngleProperty); }
            set { SetValue(LabelRotationAngleProperty, value); }
        }

        internal Brush LabelForeground
        {
            get { return (Brush)GetValue(LabelForegroundProperty); }
            set { SetValue(LabelForegroundProperty, value); }
        }

        internal OlapChartVisualStyle ChartVisualStyle
        {
            get { return (OlapChartVisualStyle)GetValue(ChartVisualStyleProperty); }
            set { SetValue(ChartVisualStyleProperty, value); }
        }

        public Style ExpanderStyle
        {
            get { return (Style)GetValue(ExpanderStyleProperty); }
            set { SetValue(ExpanderStyleProperty, value); }
        }

        public Style InnerExpanderStyle
        {
            get { return (Style)GetValue(InnerExpanderStyleProperty); }
            set { SetValue(InnerExpanderStyleProperty, value); }
        }
        bool _IsExpanded;
        public bool IsExpanded
        {
            get
            {
                if (this.CellDescriptor != null)
                    return this.CellDescriptor.ExpandableState == ExpandableState.Expanded ? true : false;
                return false;
            }
            set
            {
                this._IsExpanded = value;
            }
        }

        public Brush LabelBackground
        {
            get { return (Brush)GetValue(LabelBackgroundProperty); }
            set { SetValue(LabelBackgroundProperty, value); }
        }

        public FontStyle LabelFontStyle
        {
            get { return (FontStyle)GetValue(LabelFontStyleProperty); }
            set { SetValue(LabelFontStyleProperty, value); }
        }

        public double LabelFontSize
        {
            get{return (double)GetValue(LabelFontSizeProperty);}
            set { SetValue(LabelFontSizeProperty, value); }
        }

        public Visibility ToolTipVisibility 
        {
            get { return (System.Windows.Visibility)GetValue(ToolTipVisibilityProperty); }
            set { SetValue(ToolTipVisibilityProperty, value); }
        }

        internal string PrimaryAxisLabelDateTimeFormat
        {
            get;
            set;
        }
        #endregion

        #region Dependency Proeperties

        public static readonly DependencyProperty CellDescriptorProperty =
            DependencyProperty.Register("CellDescriptor", typeof(PivotCellDescriptor), typeof(OlapLabelPresenter), new PropertyMetadata(null));

        public static readonly DependencyProperty LabelRotationAngleProperty =
            DependencyProperty.Register("LabelRotationAngle", typeof(double), typeof(OlapLabelPresenter), new PropertyMetadata(0d));

        public static readonly DependencyProperty LabelForegroundProperty =
            DependencyProperty.Register("LabelForeground", typeof(Brush), typeof(OlapLabelPresenter), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly DependencyProperty ChartVisualStyleProperty =
            DependencyProperty.Register("ChartVisualStyle", typeof(OlapChartVisualStyle), typeof(OlapLabelPresenter), new PropertyMetadata(OlapChartVisualStyle.Default, OnChartStyleChangedCallback));

        public static readonly DependencyProperty ExpanderStyleProperty =
            DependencyProperty.Register("ExpanderStyle", typeof(Style), typeof(OlapLabelPresenter), new PropertyMetadata(null));

        public static readonly DependencyProperty InnerExpanderStyleProperty =
            DependencyProperty.Register("InnerExpanderStyle", typeof(Style), typeof(OlapLabelPresenter), new PropertyMetadata(null));

        public static readonly DependencyProperty LabelBackgroundProperty =
            DependencyProperty.Register("LabelBackground", typeof(Brush), typeof(OlapLabelPresenter), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public static readonly DependencyProperty LabelFontStyleProperty =
           DependencyProperty.Register("LabelFontStyle", typeof(FontStyle), typeof(OlapLabelPresenter), new PropertyMetadata(FontStyles.Normal));

        public static readonly DependencyProperty LabelFontSizeProperty =
            DependencyProperty.Register("LabelFontSize", typeof(double), typeof(OlapLabelPresenter), new PropertyMetadata(13d));

        public static readonly DependencyProperty ToolTipVisibilityProperty =
            DependencyProperty.Register("ToolTipVisibility", typeof(Visibility), typeof(OlapLabelPresenter), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region Helper Events

        void toggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.LabelClick != null)
            {
                this.LabelClick(sender, new OlapLabelClickEvenArgs() { CellDescriptor = this.CellDescriptor });
                toggleButton.IsChecked = this.IsExpanded;
            }
        }

        void OlapLabelPresenter_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                if (this.LabelCellClick != null)
                {
                    this.LabelCellClick(sender, new OlapLabelClickEvenArgs() { CellDescriptor = this.CellDescriptor });
                }
            }
        }

        #endregion

        #region Overridden Methods
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            base.MouseLeftButtonDown += new MouseButtonEventHandler(OlapLabelPresenter_MouseLeftButtonDown);
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                toggleButton = GetTemplateChild("PART_ExpanderButton") as ToggleButton;

                if (toggleButton != null)
                {
                    toggleButton.Click += new RoutedEventHandler(toggleButton_Click);
                }
                TextBlock textBlock = GetTemplateChild("PART_ChartLabel") as TextBlock;
                if (!string.IsNullOrEmpty(this.PrimaryAxisLabelDateTimeFormat))
                {
                    textBlock.SetBinding(TextBlock.TextProperty, new Binding { Source = this, Converter = new LabelDateTimeConverter() });
                }
                else
                {
                    textBlock.SetBinding(TextBlock.TextProperty, new Binding { Source = this, Path = new PropertyPath("CellDescriptor.CellValue") });
                    if (LabelRotationAngle != 0 && LabelRotationAngle != 360)
                    {
                        textBlock.RenderTransform = new RotateTransform() { Angle = this.LabelRotationAngle, CenterX = textBlock.ActualWidth / 2, CenterY = textBlock.ActualHeight / 2 };
                    }

                }
            }            
        }

        #endregion

        #region Callbacks

        private static void OnChartStyleChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapLabelPresenter olapLablePresenter = dependencyObject as OlapLabelPresenter;

            if (olapLablePresenter != null)
            {
                var chartStyle = (OlapChartVisualStyle)e.NewValue;
                olapLablePresenter.ApplyExpanderStyle(chartStyle);
            }              
        }
        private void ApplyExpanderStyle(OlapChartVisualStyle chartStyle)
        {
            var selectedStyle = chartStyle.ToString().ToLower();

            //if (this.ExpanderStyle == null)
            //{
            //    if (selectedStyle.Contains("blend"))
            //    {
            //        this.InnerExpanderStyle = this.resource["Blend.ExpanderStyle"] as Style;
            //    }
            //    else if (selectedStyle.Contains("screen"))
            //    {
            //        this.InnerExpanderStyle = this.resource["Transparent.ExpanderStyle"] as Style;
            //    }
            //    else if (selectedStyle.Contains("silver"))
            //    {
            //        this.InnerExpanderStyle = this.resource["Office2007Silver.ExpanderStyle"] as Style;
            //    }
            //    else if (selectedStyle.Contains("office2007black"))
            //    {
            //        this.InnerExpanderStyle = this.resource["Office2007Black.ExpanderStyle"] as Style;
            //    }
            //    else if (selectedStyle.Contains("office2007blue"))
            //    {
            //        this.InnerExpanderStyle = this.resource["Office2007Blue.ExpanderStyle"] as Style;
            //    }
            //    else
            //    {
            //        this.InnerExpanderStyle = this.resource["Classic.ExpanderStyle"] as Style;
            //    }
            //}
            //else
            //{
                this.InnerExpanderStyle = this.ExpanderStyle;
            //}
        }
        #endregion
    }

    public class LabelDateTimeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            OlapLabelPresenter formatlabel = value as OlapLabelPresenter;
            if (formatlabel != null)
            {
                DateTime date;
                string actualformat = formatlabel.PrimaryAxisLabelDateTimeFormat;
                string actualvalue = formatlabel.CellDescriptor.CellValue;
                bool canDateConvrt = DateTime.TryParse(actualvalue, out date);

                if (canDateConvrt)
                {
                    actualvalue = date.ToString(actualformat);
                }

                return actualvalue;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Process the cell descriptor and return expander visibility 
    /// </summary>
    public class ExpanderVisibilityConvertor : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Process the cell descriptor and return expander visibility 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            PivotCellDescriptor cellDescriptor = value as PivotCellDescriptor;

            if (cellDescriptor != null)
            {
                if (!cellDescriptor.HasChildren)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Process the cell descriptor and returns the drawing path of the expander
    /// </summary>
    public class ExpanderPathConvertor : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Process the cell descriptor and returns the drawing path of the expander
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            PivotCellDescriptor cellDescriptor = value as PivotCellDescriptor;
            if (cellDescriptor != null)
            {
                if (cellDescriptor.HasChildren)
                {
                    if (cellDescriptor.ExpandableState == Syncfusion.OlapSilverlight.Engine.ExpandableState.Expanded)
                    {
                        //// (-) Symbol path
                        return "M 0 2 L 0 3 L 5 3 L 5 2 Z";
                    }
                    else if (cellDescriptor.ExpandableState == Syncfusion.OlapSilverlight.Engine.ExpandableState.Collapsed)
                    {
                        //// (+) Symbol path
                        return "M 0 2 L 0 3 L 2 3 L 2 5 L 3 5 L 3 3 L 5 3 L 5 2 L 3 2 L 3 0 L 2 0 L 2 2 Z";
                    }
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

   
}
