#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.Generic;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for LegendItem
    /// </summary>
    public class LegendItem : Control
    {
        /// <summary>
        /// Called when instance created for LegendItem
        /// </summary>
        public LegendItem()
        {
            DefaultStyleKey = typeof(LegendItem);
        }

        /// <summary>
        /// Identifies Checkbox Dependency Property
        /// </summary>
        public static readonly DependencyProperty CheckboxVisibilityProperty =
            DependencyProperty.Register("CheckboxVisibility", typeof(Visibility), typeof(LegendItem), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Identifies Icon Dependency Property
        /// </summary>
        public static readonly DependencyProperty IconVisibilityProperty =
            DependencyProperty.Register("IconVisibility", typeof(Visibility), typeof(LegendItem), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Gets or sets the CheckboxProperty. This is dependency property.
        /// </summary>
        /// <value>The Visibility.</value>
        public Visibility CheckboxVisibility
        {
            get { return (Visibility)GetValue(CheckboxVisibilityProperty); }
            set { SetValue(CheckboxVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the IconProperty. This is dependency property.
        /// </summary>
        /// <value>The Visibility.</value>
        public Visibility IconVisibility
        {
            get { return (Visibility)GetValue(IconVisibilityProperty); }
            set { SetValue(IconVisibilityProperty, value); }
        }
        /// <summary>
        ///  Identifies the Interior dependency property.
        /// </summary>
        public static readonly DependencyProperty InteriorProperty =
    DependencyProperty.Register("Interior", typeof(Brush), typeof(LegendItem), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));
        /// <summary>
        /// Get or Set Interior property
        /// </summary>
        public Brush Interior
        {
            get { return (Brush)GetValue(InteriorProperty); }
            set { SetValue(InteriorProperty, value); }
        }
        /// <summary>
        /// Get or Set label property
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
    DependencyProperty.Register("Label", typeof(string), typeof(LegendItem), new PropertyMetadata(string.Empty));
        /// <summary>
        /// Get or Set Label property
        /// </summary>
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }
        /// <summary>
        ///  Identifies the IconTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty IconTemplateProperty =
DependencyProperty.Register("IconTemplate", typeof(DataTemplate), typeof(LegendItem), new PropertyMetadata(null));
        /// <summary>
        /// Get or Set IconTemplate
        /// </summary>
        public DataTemplate IconTemplate
        {
            get { return (DataTemplate)GetValue(IconTemplateProperty); }
            set { SetValue(IconTemplateProperty, value); }
        }
        /// <summary>
        ///  Identifies the IconContent dependency property.
        /// </summary>
        public static readonly DependencyProperty IconContentProperty =
DependencyProperty.Register("IconContent", typeof(object), typeof(LegendItem), new PropertyMetadata(null));
        /// <summary>
        /// Get or Set IconContent
        /// </summary>
        public object IconContent
        {
            get { return (object)GetValue(IconContentProperty); }
            set { SetValue(IconContentProperty, value); }
        }
        /// <summary>
        ///  Identifies the IsChecked dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
DependencyProperty.Register("IsChecked", typeof(bool), typeof(LegendItem), new PropertyMetadata(true));
        /// <summary>
        /// Get or Set IsChecked property
        /// </summary>
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }
        /// <summary>
        ///  Identifies the LegendObject dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendObjectProperty =
DependencyProperty.Register("LegendObject", typeof(object), typeof(LegendItem), new PropertyMetadata(null));
        /// <summary>
        /// Get or Set LegendObject property
        /// </summary>
        public object LegendObject
        {
            get { return (object)GetValue(LegendObjectProperty); }
            set { SetValue(LegendObjectProperty, value); }
        }

        CheckBox legendCheckbox = null;

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            legendCheckbox = this.GetTemplateChild("legendCheckBox") as CheckBox;
            if (legendCheckbox != null)
            {
                legendCheckbox.Checked += new RoutedEventHandler(legendCheckbox_Checked);
                legendCheckbox.Unchecked += new RoutedEventHandler(legendCheckbox_Unchecked);
            }
        }

        void legendCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            List<object> items = new List<object>();
            items.Add(this);
            items.Add(this.DataContext);
            OnSeriesVisibilityChanged(items, new SeriesVisibilityEventArg(Visibility.Visible, Visibility.Collapsed));
        }

        void legendCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            List<object> items = new List<object>();
            items.Add(this);
            items.Add(this.DataContext);
            OnSeriesVisibilityChanged(items, new SeriesVisibilityEventArg(Visibility.Collapsed, Visibility.Visible));
        }

        /// <summary>
        /// Series Visibility Event Handler for Chart Legend.
        /// </summary>
        /// <param name="sender">Sender Object.  It may be either ChartSeries or Segment based on Chart Type</param>
        /// <param name="e">Series Visibility EventArgument</param>
        public delegate void SeriesVisibilityEventHandler(object sender, SeriesVisibilityEventArg e);

        /// <summary>
        /// Event Raise when Series Visibility changed by the chart legend
        /// </summary>
        public event SeriesVisibilityEventHandler SeriesVisibilityChanged;

        /// <summary>
        /// Calls when Series Visiblity Change by the Legend Checkbox
        /// </summary>
        /// <param name="obj">Sender Object.  It may be either ChartSeries or Segment</param>
        /// <param name="e">Event Argument</param>
        protected virtual void OnSeriesVisibilityChanged(object obj, SeriesVisibilityEventArg e)
        {
            if (this.SeriesVisibilityChanged != null)
            {
                SeriesVisibilityChanged(obj, e);
            }
        }
        
    }

 }
