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
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection;
using System.Security.Permissions;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Licensing;


namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class represents Cursor in SyncInteractivecursor.
    /// </summary>
    public class SyncInteractiveCursor : DependencyObject, IChartSerializer
    {

        /// <summary>
        /// Identifies the BindWithMoseMoveOnSegment, It is a Dependency Property.
        /// </summary>
        public static readonly DependencyProperty BindWithMouseMoveOnSegmentProperty = DependencyProperty.Register("BindWithMouseMoveOnSegment", typeof(bool), typeof(SyncInteractiveCursor), new PropertyMetadata(true));
        /// <summary>
        /// Gets or sets a value indicating whether [bind with mose move on segment].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [bind with mose move on segment]; otherwise, <c>false</c>.
        /// </value>
        public bool BindWithMouseMoveOnSegment
        {
            set { SetValue(BindWithMouseMoveOnSegmentProperty, value); }
            get { return (bool)GetValue(BindWithMouseMoveOnSegmentProperty); }
        }

        /// <summary>
        ///  Identifies the IsBindWithSegment, It is a Dependency Property.
        /// </summary>
        public static readonly DependencyProperty IsBindWithSegmentProperty = DependencyProperty.Register("IsBindWithSegment", typeof(bool), typeof(SyncInteractiveCursor), new PropertyMetadata(true));
        /// <summary>
        /// Gets or sets a value indicating whether this instance is bind with segment.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is bind with segment; otherwise, <c>false</c>.
        /// </value>
        public bool IsBindWithSegment
        {
            set { SetValue(IsBindWithSegmentProperty, value); }
            get { return (bool)GetValue(IsBindWithSegmentProperty); }
        }

        /// <summary>
        ///  Identifies the CursorVisibility, It is a Dependency Property.
        /// </summary>
        public static readonly DependencyProperty CursorVisibilityProperty = DependencyProperty.Register("CursorVisibility", typeof(Visibility), typeof(SyncInteractiveCursor), new PropertyMetadata(Visibility.Visible));
        /// <summary>
        /// Gets or sets the cursor visibility.
        /// </summary>
        /// <value>The cursor visibility.</value>
        public Visibility CursorVisibility
        {
            get { return (Visibility)GetValue(CursorVisibilityProperty); }
            set { SetValue(CursorVisibilityProperty, value); }
        }

        /// <summary>
        ///  Identifies the CursorStrokeThickness, It is a Dependency Property.
        /// </summary>
        public static readonly DependencyProperty CursorStrokeThicknessProperty = DependencyProperty.Register("CursorStrokeThickness", typeof(double), typeof(SyncInteractiveCursor), new PropertyMetadata(1.0));
        /// <summary>
        /// Gets or sets the cursor stroke thickness.
        /// </summary>
        /// <value>The cursor stroke thickness.</value>
        public double CursorStrokeThickness
        {
            get { return (double)GetValue(CursorStrokeThicknessProperty); }
            set { SetValue(CursorStrokeThicknessProperty, value); }
        }

        /// <summary>
        ///  Identifies the VerticalCursorStroke, It is a Dependency Property.
        /// </summary>
        public static readonly DependencyProperty VerticalCursorStrokeProperty = DependencyProperty.Register("VerticalCursorStroke", typeof(Brush), typeof(SyncInteractiveCursor), new PropertyMetadata(Brushes.BlueViolet));
        /// <summary>
        /// Gets or sets the vertical cursor stroke.
        /// </summary>
        /// <value>The vertical cursor stroke.</value>
        public Brush VerticalCursorStroke
        {
            get { return (Brush)GetValue(VerticalCursorStrokeProperty); }
            set { SetValue(VerticalCursorStrokeProperty, value); }
        }

        /// <summary>
        ///  Identifies the HorizontalCursorStroke, It is a Dependency Property.
        /// </summary>
        public static readonly DependencyProperty HorizontalCursorStrokeProperty = DependencyProperty.Register("HorizontalCursorStroke", typeof(Brush), typeof(SyncInteractiveCursor), new PropertyMetadata(Brushes.BlueViolet));
        /// <summary>
        /// Gets or sets the horizontal cursor stroke.
        /// </summary>
        /// <value>The horizontal cursor stroke.</value>
        public Brush HorizontalCursorStroke
        {
            get { return (Brush)GetValue(HorizontalCursorStrokeProperty); }
            set { SetValue(HorizontalCursorStrokeProperty, value); }
        }

        /// <summary>
        ///  Identifies the LabelVisibility, It is a Dependency Property.
        /// </summary>
        public static readonly DependencyProperty LabelVisibilityProperty = DependencyProperty.Register("LabelVisibility", typeof(Visibility), typeof(SyncInteractiveCursor), new PropertyMetadata(Visibility.Visible));
        /// <summary>
        /// Gets or sets the label visibility.
        /// </summary>
        /// <value>The label visibility.</value>
        public Visibility LabelVisibility
        {
            get { return (Visibility)GetValue(LabelVisibilityProperty); }
            set { SetValue(LabelVisibilityProperty, value); }
        }

        /// <summary>
        ///  Identifies the LabelBackground, It is a Dependency Property.
        /// </summary>
        public static readonly DependencyProperty LabelBackgroundProperty = DependencyProperty.Register("LabelBackground", typeof(Brush), typeof(SyncInteractiveCursor), new PropertyMetadata(Brushes.Transparent));
        /// <summary>
        /// Gets or sets the label background.
        /// </summary>
        /// <value>The label background.</value>
        public Brush LabelBackground
        {
            get { return (Brush)GetValue(LabelBackgroundProperty); }
            set { SetValue(LabelBackgroundProperty, value); }
        }

        /// <summary>
        ///  Identifies the LabelForeground, It is a Dependency Property.
        /// </summary>
        public static readonly DependencyProperty LabelForegroundProperty = DependencyProperty.Register("LabelForeground", typeof(Brush), typeof(SyncInteractiveCursor), new PropertyMetadata(Brushes.Black));
        /// <summary>
        /// Gets or sets the label foreground.
        /// </summary>
        /// <value>The label foreground.</value>
        public Brush LabelForeground
        {
            get { return (Brush)GetValue(LabelForegroundProperty); }
            set { SetValue(LabelForegroundProperty, value); }
        }

        /// <summary>
        ///  Identifies the VerticalCursorLabelContent, It is a Dependency Property.
        /// </summary>
        internal static readonly DependencyProperty VerticalCursorLabelContentProperty = DependencyProperty.Register("VerticalCursorLabelContent", typeof(object), typeof(SyncInteractiveCursor), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the content of the vertical cursor label.
        /// </summary>
        /// <value>The content of the vertical cursor label.</value>
        internal object VerticalCursorLabelContent
        {
            get { return (object)GetValue(VerticalCursorLabelContentProperty); }
            set { SetValue(VerticalCursorLabelContentProperty, value); }
        }

        /// <summary>
        ///  Identifies the OffsetY, It is a Dependency Property.
        /// </summary>
        public static readonly DependencyProperty OffsetYProperty = DependencyProperty.Register("OffsetY", typeof(double), typeof(SyncInteractiveCursor), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the offset Y.
        /// </summary>
        /// <value>The offset Y.</value>
        public double OffsetY
        {
            get { return (double)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }

        /// <summary>
        ///  Identifies the OffsetX, It is a Dependency Property.
        /// </summary>
        public static readonly DependencyProperty OffsetXProperty = DependencyProperty.Register("OffsetX", typeof(double), typeof(SyncInteractiveCursor), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the offset X.
        /// </summary>
        /// <value>The offset X.</value>
        public double OffsetX
        {
            get { return (double)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }

        /// <summary>
        ///  Identifies the XValue, It is a Dependency Property.
        /// </summary>
        internal static readonly DependencyProperty XValueProperty = DependencyProperty.Register("XValue", typeof(double), typeof(SyncInteractiveCursor), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the X value.
        /// </summary>
        /// <value>The X value.</value>
        internal double XValue
        {
            get { return (double)GetValue(XValueProperty); }
            set { SetValue(XValueProperty, value); }
        }

        /// <summary>
        ///  Identifies the YValue, It is a Dependency Property.
        /// </summary>
        internal static readonly DependencyProperty YValueProperty = DependencyProperty.Register("YValue", typeof(double), typeof(SyncInteractiveCursor), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the Y value.
        /// </summary>
        /// <value>The Y value.</value>
        internal double YValue
        {
            get { return (double)GetValue(YValueProperty); }
            set { SetValue(YValueProperty, value); }
        }
       
        /// <summary>
        ///  Identifies the ChartSeries.
        /// </summary>
        private ChartSeries series;
        /// <summary>
        /// Gets or sets the chartseries.
        /// </summary>
        /// <value>The chartseries.</value>
        internal ChartSeries chartseries
        {
            get { return series; }
            set { series = value; }
        }

        /// <summary>
        ///  Identifies the ChartArea.
        /// </summary>
        private ChartArea area;
        /// <summary>
        /// Gets or sets the chartarea.
        /// </summary>
        /// <value>The chartarea.</value>
        internal ChartArea chartarea
        {
            get { return area; }
            set { area = value; }
        }

        /// <summary>
        ///  Identifies the InteractiveCuraor Collections
        /// </summary>
        internal InteractiveCursorCollection m_interactivecursor = new InteractiveCursorCollection();
        /// <summary>
        /// Gets the interactivecursor.
        /// </summary>
        /// <value>The interactivecursor.</value>
        internal InteractiveCursorCollection interactivecursor
        {
            get { return m_interactivecursor; }
        }
        //public SyncInteractiveCursor()
        //{
            
        //}

        #region IChartSerializer Members

        /// <summary>
        /// Method declaration for Serialize
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            string _xamlString;
            _xamlString = XamlWriter.Save(this);
            return _xamlString;
        }

        /// <summary>
        /// Method declaration for DeSerialize
        /// </summary>
        /// <param name="xamlString"></param>
        /// <returns></returns>
        public object Deserialize(string xamlString)
        {
            return XamlReader.Parse(xamlString);
        }

        #endregion
    }
}
