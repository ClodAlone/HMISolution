// <copyright file="HorizontalRuler.cs" company="Syncfusion">
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
    /// Represents the horizontal ruler.
    /// </summary>
    /// <remarks>
    /// Rulers display the coordinates of elements on the <see cref="DiagramPage"/>. Negative label values get displayed on the ruler in case the page is panned to the right side.
    /// On Zooming, the ruler values get adjusted accordingly, to match with the current Zoom level. At any point, the ruler value always indicates the exact coordinates of the page and its elements. 
    /// So when the page is zoomed, the interval values get halved or doubled depending upon the zoom level.
    /// </remarks>
    /// <example>
    /// <para/>The following example shows how to create a <see cref="HorizontalRuler"/> in XAML.
    /// <code language="XAML">
    /// &lt;Window x:Class="RulersAndUnits.Window1"
    /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
    /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
    ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
    ///  Icon="Images/App.ico" &gt;
    ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
    ///                                IsSymbolPaletteEnabled="True" 
    ///                                Background="WhiteSmoke"&gt;
    ///           &lt;syncfusion:DiagramControl.View&gt;
    ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
    ///                                        Background="LightGray"  
    ///                                        Bounds="0,0,12,12"  
    ///                                        ShowHorizontalGridLine="False" 
    ///                                        ShowVerticalGridLine="False"
    ///                                        Name="diagramView"  &gt;
    ///            &lt;syncfusion:DiagramView.HorizontalRuler&gt;
    ///            &lt;syncfusion:HorizontalRuler Name="horizontalRuler" 
    ///                                RulerThickness="25" 
    ///                                MajorLinesStroke="Black" 
    ///                                MinorLinesStroke="Gray" 
    ///                                LabelFontColor="Black" /&gt;
    ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
    ///       &lt;/syncfusion:DiagramView&gt;
    ///    &lt;/syncfusion:DiagramControl.View&gt;
    /// &lt;/syncfusion:DiagramControl&gt;
    /// &lt;/Window&gt;
    ///   </code>
    ///   <para/>The following example shows how to create a <see cref="HorizontalRuler"/> in C#.
    ///  <code language="C#">
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
    ///        HorizontalRuler hruler = new HorizontalRuler();
    ///        hruler.RulerThickness = 25d;
    ///        hruler.MajorLinesStroke = Brushes.Black;
    ///        hruler.MinorLinesStroke = Brushes.Gray;
    ///        hruler.LabelFontColor = Brushes.Black;
    ///        View.HorizontalRuler = hruler;
    /// </code>
    /// </example>
#if !SyncfusionFramework3_5
    [DesignTimeVisible(false)]
#endif
    public class HorizontalRuler : Control
    {
        #region local variables
        /// <summary>
        /// Used to store ruler thickness value.
        /// </summary>
        private double rulerThickness = 25d;
      
        /// <summary>
        /// Used to store marker thickness value.
        /// </summary>
        private double markerThickness = 1d;
       
        /// <summary>
        /// Used to store View instance.
        /// </summary>
        private DiagramView view;
       
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
            DependencyProperty.Register("ShowDateTime", typeof(bool), typeof(HorizontalRuler), new UIPropertyMetadata(false));
              
        /// <summary>
        /// Gets or sets the MajorLinesStroke dependency property.
        /// </summary>
        public static readonly DependencyProperty MajorLinesStrokeProperty = DependencyProperty.Register("MajorLinesStroke", typeof(Brush), typeof(HorizontalRuler), new PropertyMetadata(Brushes.Black));
      
        /// <summary>
        /// Gets or sets the MinorLinesStroke dependency property.
        /// </summary>
        public static readonly DependencyProperty MinorLinesStrokeProperty = DependencyProperty.Register("MinorLinesStroke", typeof(Brush), typeof(HorizontalRuler), new PropertyMetadata(Brushes.Black));
      
        /// <summary>
        /// Gets or sets the MarkerBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty MarkerBrushProperty = DependencyProperty.Register("MarkerBrush", typeof(Brush), typeof(HorizontalRuler), new PropertyMetadata(Brushes.Red));
      
        /// <summary>
        /// Gets or sets the ShowMarker dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowMarkerProperty = DependencyProperty.Register("ShowMarker", typeof(bool), typeof(HorizontalRuler), new PropertyMetadata(true));
      
        /// <summary>
        /// Gets or sets the Orientation dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(HorizontalRuler), new PropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(OnOrientationChanged)));
       
        /// <summary>
        /// Gets or sets the Interval dependency property.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty = DependencyProperty.Register("Interval", typeof(double), typeof(HorizontalRuler), new PropertyMetadata(50d));
       
        /// <summary>
        /// Gets or sets the MinorLineCountPerInterval dependency property.
        /// </summary>
        public static readonly DependencyProperty MinorLineCountPerIntervalProperty = DependencyProperty.Register("MinorLinesCountPerInterval", typeof(double), typeof(HorizontalRuler), new PropertyMetadata(4d));
      
        /// <summary>
        /// Gets or sets the MarkerPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty MarkerPositionProperty = DependencyProperty.Register("MarkerPosition", typeof(double), typeof(HorizontalRuler), new PropertyMetadata(0d));
      
        /// <summary>
        /// Gets or sets the LabelPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelPositionProperty = DependencyProperty.Register("LabelPosition", typeof(double), typeof(HorizontalRuler), new PropertyMetadata(100d, new PropertyChangedCallback(OnLabelPositionChanged)));
      
        /// <summary>
        /// Gets or sets the LabelFontColor dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFontColorProperty = DependencyProperty.Register("LabelFontColor", typeof(Brush), typeof(HorizontalRuler), new PropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnLabelFontColorChanged)));
       
        #endregion

        #region DP Getter and Setters
       
        /// <summary>
        /// Gets or sets the LabelFontColor.
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
        ///                                IsSymbolPaletteEnabled="True" 
        ///                                Background="WhiteSmoke"&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///            &lt;syncfusion:DiagramView.HorizontalRuler&gt;
        ///            &lt;syncfusion:HorizontalRuler Name="horizontalRuler" 
        ///                                          LabelFontColor="Black" /&gt;
        ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        ///   </code>
        ///  <code language="C#">
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
        ///        HorizontalRuler hruler = new HorizontalRuler();
        ///        hruler.LabelFontColor = Brushes.Black;
        ///        View.HorizontalRuler = hruler;
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
        ///                                IsSymbolPaletteEnabled="True" &gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///            &lt;syncfusion:DiagramView.HorizontalRuler&gt;
        ///            &lt;syncfusion:HorizontalRuler Name="horizontalRuler" 
        ///                                          MajorLinesThickness="2" /&gt;
        ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        ///   </code>
        ///  <code language="C#">
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
        ///        HorizontalRuler hruler = new HorizontalRuler();
        ///        hruler.MajorLinesThickness = 2;
        ///        View.HorizontalRuler = hruler;
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
        /// Default value is .5d.
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
        ///                                IsSymbolPaletteEnabled="True" &gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///            &lt;syncfusion:DiagramView.HorizontalRuler&gt;
        ///            &lt;syncfusion:HorizontalRuler Name="horizontalRuler" 
        ///                                          MinorLinesThickness="2" /&gt;
        ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        ///   </code>
        ///  <code language="C#">
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
        ///        HorizontalRuler hruler = new HorizontalRuler();
        ///        hruler.MinorLinesThickness = 2;
        ///        View.HorizontalRuler = hruler;
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
        /// Gets or sets the LabelPosition.
        /// </summary>
        internal double LabelPosition
        {
            get
            {
                return (double)GetValue(LabelPositionProperty);
            }

            set
            {
                SetValue(LabelPositionProperty, value);
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
        ///                                IsSymbolPaletteEnabled="True" &gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///            &lt;syncfusion:DiagramView.HorizontalRuler&gt;
        ///            &lt;syncfusion:HorizontalRuler Name="horizontalRuler" 
        ///                                          MajorLinesStroke="Orange" /&gt;
        ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        ///   </code>
        ///  <code language="C#">
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
        ///        HorizontalRuler hruler = new HorizontalRuler();
        ///        hruler.MajorLinesStroke = Brushes.Orange;
        ///        View.HorizontalRuler = hruler;
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
        ///                                IsSymbolPaletteEnabled="True" &gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///            &lt;syncfusion:DiagramView.HorizontalRuler&gt;
        ///            &lt;syncfusion:HorizontalRuler Name="horizontalRuler" 
        ///                                          MinorLinesStroke="Orange" /&gt;
        ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        ///   </code>
        ///  <code language="C#">
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
        ///        HorizontalRuler hruler = new HorizontalRuler();
        ///        hruler.MinorLinesStroke = Brushes.Orange;
        ///        View.HorizontalRuler = hruler;
        /// </code>
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
        /// Gets or sets the MarkerBrush.
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
        ///                                IsSymbolPaletteEnabled="True" &gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///            &lt;syncfusion:DiagramView.HorizontalRuler&gt;
        ///            &lt;syncfusion:HorizontalRuler Name="horizontalRuler" 
        ///                                          MarkerBrush="Red" /&gt;
        ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        ///   </code>
        ///  <code language="C#">
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
        ///        HorizontalRuler hruler = new HorizontalRuler();
        ///        hruler.MarkerBrush = Brushes.Red;
        ///        View.HorizontalRuler = hruler;
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
        /// Gets or sets the MarkerBrush.
        /// </summary>
        /// <remarks>
        /// Default value is 1d.
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
        ///                                IsSymbolPaletteEnabled="True" &gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///            &lt;syncfusion:DiagramView.HorizontalRuler&gt;
        ///            &lt;syncfusion:HorizontalRuler Name="horizontalRuler" 
        ///                                          MarkerThickness="2" /&gt;
        ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        ///   </code>
        ///  <code language="C#">
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
        ///        HorizontalRuler hruler = new HorizontalRuler();
        ///        hruler.MarkerThickness = 2;
        ///        View.HorizontalRuler = hruler;
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
        /// Default value is true.
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
        ///                                IsSymbolPaletteEnabled="True" &gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///            &lt;syncfusion:DiagramView.HorizontalRuler&gt;
        ///            &lt;syncfusion:HorizontalRuler Name="horizontalRuler" 
        ///                                          ShowMarker="False" /&gt;
        ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        ///   </code>
        ///  <code language="C#">
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
        ///        HorizontalRuler hruler = new HorizontalRuler();
        ///        hruler.ShowMarker = false;
        ///        View.HorizontalRuler = hruler;
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
        public Orientation Orientation
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
        /// Default value is 25d.
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
        ///                                IsSymbolPaletteEnabled="True" &gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///            &lt;syncfusion:DiagramView.HorizontalRuler&gt;
        ///            &lt;syncfusion:HorizontalRuler Name="horizontalRuler" 
        ///                                          RulerThickness="50" /&gt;
        ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        ///   </code>
        ///  <code language="C#">
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
        ///        HorizontalRuler hruler = new HorizontalRuler();
        ///        hruler.RulerThickness = 50;
        ///        View.HorizontalRuler = hruler;
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
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            TickBar tick = GetTemplateChild("PART_TickBar") as TickBar;
            view = Node.GetDiagramView(this);
        }

        /// <summary>
        /// Called when [orientation changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when [label position changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLabelPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when [label font color changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLabelFontColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        #endregion

        /// <summary>
        /// Initializes static members of the <see cref="HorizontalRuler"/> class.
        /// </summary>
        static HorizontalRuler()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HorizontalRuler), new FrameworkPropertyMetadata(typeof(HorizontalRuler)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HorizontalRuler"/> class.
        /// </summary>
        public HorizontalRuler()
        {
            this.ClipToBounds = true;
        }
    }
}
