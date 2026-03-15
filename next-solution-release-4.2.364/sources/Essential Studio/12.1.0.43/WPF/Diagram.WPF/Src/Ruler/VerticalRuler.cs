// <copyright file="VerticalRuler.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents the vertical ruler.
    /// </summary>
    /// <example>
    /// <para/>The following example shows how to create a <see cref="VerticalRuler"/> in XAML.
    /// <code language="XAML">
    /// &lt;Window x:Class="RulersAndUnits.Window1"
    /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
    /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
    ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
    ///  Icon="Images/App.ico" &gt;
    ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
    ///                                IsSymbolPaletteEnabled="True"&gt;
    ///           &lt;syncfusion:DiagramControl.View&gt;
    ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
    ///                                        Background="LightGray"  
    ///                                        Bounds="0,0,12,12"  
    ///                                        ShowHorizontalGridLine="False" 
    ///                                        ShowVerticalGridLine="False"
    ///                                        Name="diagramView"  &gt;
    ///            &lt;syncfusion:DiagramView.VerticalRuler&gt;
    ///            &lt;syncfusion:VerticalRuler Name="verticalRuler" 
    ///                                RulerThickness="25" 
    ///                                MajorLinesStroke="Black" 
    ///                                MinorLinesStroke="Gray" 
    ///                                LabelFontColor="Black" /&gt;
    ///          &lt;/syncfusion:DiagramView.VerticalRuler&gt;
    ///       &lt;/syncfusion:DiagramView&gt;
    ///    &lt;/syncfusion:DiagramControl.View&gt;
    /// &lt;/syncfusion:DiagramControl&gt;
    /// &lt;/Window&gt;
    /// </code>
    /// <para/>The following example shows how to create a <see cref="VerticalRuler"/> in C#.
    /// <code language="C#">
    /// using Syncfusion.Windows.Diagram;
    /// namespace WpfApplication1
    /// {
    /// public partial class Window1 : Window
    /// {
    ///    public DiagramControl Control;
    ///    public DiagramModel Model;
    ///    public DiagramView View;
    ///    public Window1 ()
    ///    {
    ///        InitializeComponent ();
    ///        Control = new DiagramControl ();
    ///        Model = new DiagramModel ();
    ///        View = new DiagramView ();
    ///        Control.Model = Model;
    ///        Control.View = View;
    ///        VerticalRuler vruler = new VerticalRuler();
    ///        vruler.RulerThickness = 25d;
    ///        vruler.MajorLinesStroke = Brushes.Black;
    ///        vruler.MinorLinesStroke = Brushes.Gray;
    ///        vruler.LabelFontColor = Brushes.Black;
    ///       View.VerticalRuler = vruler;
    /// </code>
    /// </example>
#if !SyncfusionFramework3_5
    [DesignTimeVisible(false)]
#endif
    public class VerticalRuler : Control
    {
        #region local variables

        /// <summary>
        /// Used to store ruler thickness value.
        /// </summary>
        private double rulerThickness = 30d;

        /// <summary>
        /// Used to store marker thickness value.
        /// </summary>
        private double markerThickness = 1d;

        /// <summary>
        /// Used to store minor lines thickness value.
        /// </summary>
        private double minorThickness = .5d;

        /// <summary>
        /// Used to store major lines thickness value.
        /// </summary>
        private double majorThickness = 1d;

        #endregion

        #region Dependency Properties

        public bool ShowDateTime
        {
            get { return (bool)GetValue(ShowDateTimeProperty); }
            set { SetValue(ShowDateTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowDateTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowDateTimeProperty =
            DependencyProperty.Register("ShowDateTime", typeof(bool), typeof(VerticalRuler), new UIPropertyMetadata(false));

        /// <summary>
        /// Gets or sets the MajorLinesStroke dependency property.
        /// </summary>
        public static readonly DependencyProperty MajorLinesStrokeProperty = DependencyProperty.Register("MajorLinesStroke", typeof(Brush), typeof(VerticalRuler), new PropertyMetadata(Brushes.Black));

        /// <summary>
        /// Gets or sets the MinorLinesStroke dependency property.
        /// </summary>
        public static readonly DependencyProperty MinorLinesStrokeProperty = DependencyProperty.Register("MinorLinesStroke", typeof(Brush), typeof(VerticalRuler), new PropertyMetadata(Brushes.Black));

        /// <summary>
        /// Gets or sets the MarkerBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty MarkerBrushProperty = DependencyProperty.Register("MarkerBrush", typeof(Brush), typeof(VerticalRuler), new PropertyMetadata(Brushes.Red));

        /// <summary>
        /// Gets or sets the ShowMarker dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowMarkerProperty = DependencyProperty.Register("ShowMarker", typeof(bool), typeof(VerticalRuler), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the Orientation dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(VerticalRuler), new PropertyMetadata(Orientation.Vertical, new PropertyChangedCallback(OnOrientationChanged)));

        /// <summary>
        /// Gets or sets the Interval dependency property.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty = DependencyProperty.Register("Interval", typeof(double), typeof(VerticalRuler), new PropertyMetadata(50d));

        /// <summary>
        /// Gets or sets the MinorLineCountPerInterval dependency property.
        /// </summary>
        public static readonly DependencyProperty MinorLineCountPerIntervalProperty = DependencyProperty.Register("MinorLinesCountPerInterval", typeof(double), typeof(VerticalRuler), new PropertyMetadata(4d));

        /// <summary>
        /// Gets or sets the MarkerPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty MarkerPositionProperty = DependencyProperty.Register("MarkerPosition", typeof(double), typeof(VerticalRuler), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the TranslatePosition dependency property.
        /// </summary>
        public static readonly DependencyProperty TranslatePositionProperty = DependencyProperty.Register("TranslatePosition", typeof(double), typeof(VerticalRuler), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the LabelFontColor dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFontColorProperty = DependencyProperty.Register("LabelFontColor", typeof(Brush), typeof(VerticalRuler), new PropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnLabelFontColorChanged)));

        #endregion

        #region DP Getter and Setters

        /// <summary>
        /// Gets or sets the Label Font Color.
        /// </summary>
        /// <remarks>
        /// Default value is Black.
        /// </remarks>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True"&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///            &lt;syncfusion:DiagramView.VerticalRuler&gt;
        ///            &lt;syncfusion:VerticalRuler Name="verticalRuler" 
        ///                                LabelFontColor="Black" /&gt;
        ///          &lt;/syncfusion:DiagramView.VerticalRuler&gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///        InitializeComponent ();
        ///        Control = new DiagramControl ();
        ///        Model = new DiagramModel ();
        ///        View = new DiagramView ();
        ///        Control.Model = Model;
        ///        Control.View = View;
        ///        VerticalRuler vruler = new VerticalRuler();
        ///        vruler.LabelFontColor = Brushes.Black;
        ///       View.VerticalRuler = vruler;
        /// </code>
        /// </example>
        public Brush LabelFontColor
        {
            get
            {
                return (Brush)GetValue(LabelFontColorProperty);
            }

            set
            {
                SetValue(LabelFontColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Position at which to move.
        /// </summary>
        internal double TranslatePosition
        {
            get
            {
                return (double)GetValue(TranslatePositionProperty);
            }

            set
            {
                SetValue(TranslatePositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the MajorLinesStroke.
        /// </summary>
        /// <remarks>
        /// Default value is Black.
        /// </remarks>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True"&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///            &lt;syncfusion:DiagramView.VerticalRuler&gt;
        ///            &lt;syncfusion:VerticalRuler Name="verticalRuler" 
        ///                                MajorLinesStroke="Red" /&gt;
        ///          &lt;/syncfusion:DiagramView.VerticalRuler&gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        ///  </code>
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///        InitializeComponent ();
        ///        Control = new DiagramControl ();
        ///        Model = new DiagramModel ();
        ///        View = new DiagramView ();
        ///        Control.Model = Model;
        ///        Control.View = View;
        ///        VerticalRuler vruler = new VerticalRuler();
        ///        vruler.MajorLinesStroke = Brushes.Red;
        ///       View.VerticalRuler = vruler;
        /// </code>
        /// </example>
        public Brush MajorLinesStroke
        {
            get
            {
                return (Brush)GetValue(MajorLinesStrokeProperty);
            }

            set
            {
                SetValue(MajorLinesStrokeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the MinorLinesStroke.
        /// </summary>
        /// <remarks>
        /// Default value is Black.
        /// </remarks>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True"&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///            &lt;syncfusion:DiagramView.VerticalRuler&gt;
        ///            &lt;syncfusion:VerticalRuler Name="verticalRuler" 
        ///                                MinorLinesStroke="Red" /&gt;
        ///          &lt;/syncfusion:DiagramView.VerticalRuler&gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///        InitializeComponent ();
        ///        Control = new DiagramControl ();
        ///        Model = new DiagramModel ();
        ///        View = new DiagramView ();
        ///        Control.Model = Model;
        ///        Control.View = View;
        ///        VerticalRuler vruler = new VerticalRuler();
        ///        vruler.MinorLinesStroke = Brushes.Red;
        ///       View.VerticalRuler = vruler;
        ///  </code>
        /// </example>
        public Brush MinorLinesStroke
        {
            get
            {
                return (Brush)GetValue(MinorLinesStrokeProperty);
            }

            set
            {
                SetValue(MinorLinesStrokeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Marker Brush.
        /// </summary>
        /// <remarks>
        /// Default value is Red.
        /// </remarks>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True"&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///            &lt;syncfusion:DiagramView.VerticalRuler&gt;
        ///            &lt;syncfusion:VerticalRuler Name="verticalRuler" 
        ///                                MarkerBrush="Black" /&gt;
        ///          &lt;/syncfusion:DiagramView.VerticalRuler&gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        ///  </code>
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///        InitializeComponent ();
        ///        Control = new DiagramControl ();
        ///        Model = new DiagramModel ();
        ///        View = new DiagramView ();
        ///        Control.Model = Model;
        ///        Control.View = View;
        ///        VerticalRuler vruler = new VerticalRuler();
        ///        vruler.MarkerBrush = Brushes.Black;
        ///       View.VerticalRuler = vruler;
        /// </code>
        /// </example>
        public Brush MarkerBrush
        {
            get
            {
                return (Brush)GetValue(MarkerBrushProperty);
            }

            set
            {
                SetValue(MarkerBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the MarkerThickness.
        /// </summary>
        /// <remarks>
        /// Default value is 1.
        /// </remarks>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True"&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///            &lt;syncfusion:DiagramView.VerticalRuler&gt;
        ///            &lt;syncfusion:VerticalRuler Name="verticalRuler" 
        ///                                MarkerThickness="2" /&gt;
        ///          &lt;/syncfusion:DiagramView.VerticalRuler&gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        ///  </code>
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///        InitializeComponent ();
        ///        Control = new DiagramControl ();
        ///        Model = new DiagramModel ();
        ///        View = new DiagramView ();
        ///        Control.Model = Model;
        ///        Control.View = View;
        ///        VerticalRuler vruler = new VerticalRuler();
        ///        vruler.MarkerThickness = 2;
        ///       View.VerticalRuler = vruler;
        /// </code>
        /// </example>
        public double MarkerThickness
        {
            get
            {
                return markerThickness;
            }

            set
            {
                //value = MeasureUnitsConverter.ToPixels(value, DiagramPage.Munits);
                markerThickness = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show marker].
        /// </summary>
        /// <value><c>true</c> if [show marker]; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// Default value is True.
        /// </remarks>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow"
        /// xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        /// Icon="Images/App.ico" &gt;
        /// &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl"
        /// IsSymbolPaletteEnabled="True"&gt;
        /// &lt;syncfusion:DiagramControl.View&gt;
        /// &lt;syncfusion:DiagramView  IsPageEditable="True"
        /// Bounds="0,0,12,12"
        /// Name="diagramView"  &gt;
        /// &lt;syncfusion:DiagramView.VerticalRuler&gt;
        /// &lt;syncfusion:VerticalRuler Name="verticalRuler"
        /// ShowMarker="True" /&gt;
        /// &lt;/syncfusion:DiagramView.VerticalRuler&gt;
        /// &lt;/syncfusion:DiagramView&gt;
        /// &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public Window1 ()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.Model = Model;
        /// Control.View = View;
        /// VerticalRuler vruler = new VerticalRuler();
        /// vruler.ShowMarker = true;
        /// View.VerticalRuler = vruler;
        /// </code>
        /// </example>
        public bool ShowMarker
        {
            get
            {
                return (bool)GetValue(ShowMarkerProperty);
            }

            set
            {
                SetValue(ShowMarkerProperty, value);
            }
        }

        /// <summary>
        /// Gets the Orientation.
        /// </summary>
        internal Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }
        }

        /// <summary>
        /// Gets or sets the Interval.
        /// </summary>
        internal double Interval
        {
            get
            {
                return (double)GetValue(IntervalProperty);
            }

            set
            {
                SetValue(IntervalProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the MajorLinesThickness.
        /// </summary>
        /// <remarks>
        /// Default value is 1.
        /// </remarks>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True"&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///            &lt;syncfusion:DiagramView.VerticalRuler&gt;
        ///            &lt;syncfusion:VerticalRuler Name="verticalRuler" 
        ///                                MajorLinesThickness="2" /&gt;
        ///          &lt;/syncfusion:DiagramView.VerticalRuler&gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        ///  </code>
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///        InitializeComponent ();
        ///        Control = new DiagramControl ();
        ///        Model = new DiagramModel ();
        ///        View = new DiagramView ();
        ///        Control.Model = Model;
        ///        Control.View = View;
        ///        VerticalRuler vruler = new VerticalRuler();
        ///        vruler.MajorLinesThickness = 2;
        ///       View.VerticalRuler = vruler;
        /// </code>
        /// </example>
        public double MajorLinesThickness
        {
            get
            {
                return majorThickness;
            }

            set
            {
                //value = MeasureUnitsConverter.ToPixels(value, DiagramPage.Munits);
                majorThickness = value;
            }
        }

        /// <summary>
        /// Gets or sets the MinorLinesThickness.
        /// </summary>
        /// <remarks>
        /// Default value is .5.
        /// </remarks>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True"&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///            &lt;syncfusion:DiagramView.VerticalRuler&gt;
        ///            &lt;syncfusion:VerticalRuler Name="verticalRuler" 
        ///                                MinorLinesThickness="2" /&gt;
        ///          &lt;/syncfusion:DiagramView.VerticalRuler&gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        ///  </code>
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///        InitializeComponent ();
        ///        Control = new DiagramControl ();
        ///        Model = new DiagramModel ();
        ///        View = new DiagramView ();
        ///        Control.Model = Model;
        ///        Control.View = View;
        ///        VerticalRuler vruler = new VerticalRuler();
        ///        vruler.MinorLinesThickness = 2;
        ///       View.VerticalRuler = vruler;
        /// </code>
        /// </example>
        public double MinorLinesThickness
        {
            get
            {
                return minorThickness;
            }

            set
            {
                //value = MeasureUnitsConverter.ToPixels(value, DiagramPage.Munits);
                minorThickness = value;
            }
        }

        /// <summary>
        /// Gets or sets the MinorLinesPerInterval.
        /// </summary>
        internal double MinorLinesPerInterval
        {
            get
            {
                return (double)GetValue(MinorLineCountPerIntervalProperty);
            }

            set
            {
                SetValue(MinorLineCountPerIntervalProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the RulerThickness.
        /// </summary>
        /// <remarks>
        /// Default value is 30.
        /// </remarks>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True"&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///            &lt;syncfusion:DiagramView.VerticalRuler&gt;
        ///            &lt;syncfusion:VerticalRuler Name="verticalRuler" 
        ///                                RulerThickness="50" /&gt;
        ///          &lt;/syncfusion:DiagramView.VerticalRuler&gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        ///  </code>
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///        InitializeComponent ();
        ///        Control = new DiagramControl ();
        ///        Model = new DiagramModel ();
        ///        View = new DiagramView ();
        ///        Control.Model = Model;
        ///        Control.View = View;
        ///        VerticalRuler vruler = new VerticalRuler();
        ///        vruler.RulerThickness = 50;
        ///       View.VerticalRuler = vruler;
        /// </code>
        /// </example>
        public double RulerThickness
        {
            get
            {
                return rulerThickness;
            }

            set
            {
                //value = MeasureUnitsConverter.ToPixels(value, DiagramPage.Munits);
                rulerThickness = value;
            }
        }

        /// <summary>
        /// Gets the MarkerPosition.
        /// </summary>
        public double MarkerPosition
        {
            get
            {
                return (double)GetValue(MarkerPositionProperty);
            }

            internal set
            {
                SetValue(MarkerPositionProperty, value);
            }
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Called when [orientation changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VerticalRuler ruler = (VerticalRuler)d;
            ruler.InvalidateVisual();
        }

        /// <summary>
        /// Called when [label font color changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLabelFontColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VerticalRuler ruler = (VerticalRuler)d;
            ruler.InvalidateVisual();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="VerticalRuler"/> class.
        /// </summary>
        static VerticalRuler()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VerticalRuler), new FrameworkPropertyMetadata(typeof(VerticalRuler)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VerticalRuler"/> class.
        /// </summary>
        public VerticalRuler()
        {
            this.ClipToBounds = true;
        }

        #endregion
    }
}
