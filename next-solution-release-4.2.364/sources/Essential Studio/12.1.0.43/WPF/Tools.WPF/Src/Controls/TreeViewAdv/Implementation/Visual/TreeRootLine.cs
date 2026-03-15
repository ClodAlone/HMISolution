// <copyright file="TreeRootLine.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

#region file using

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

#endregion file using

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the RootLine for TreeViewAdv control
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TreeRootLine : FrameworkElement
    {
        #region Members

        /// <summary>
        /// Represents an ordered collection
        /// </summary>
        private static DoubleCollection dcollection;

        #endregion Members

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is vertical line.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is vertical line; otherwise, <c>false</c>.
        /// </value>
        public bool IsVerticalLine
        {
            get;
            set;
        }

        #endregion Properties

        #region DP Setters & getters

        /// <summary>
        /// Gets or sets a brush that describes the background of a node line.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// The brush that is used to fill the line's node.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set LineBrush property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             myTreeView.LineBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set LineBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" LineBrush="Red">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="Brush"/>
        public Brush LineBrush
        {
            get
            {
                return (Brush)GetValue(LineBrushProperty);
            }

            set
            {
                SetValue(LineBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a stroke thickness that describes the thickness of a node line.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// The double that is used to draw the node line.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set StrokeThickness property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             myTreeView.LineStrokeThickness = 2;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set StrokeThickness property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" LineStrokeThickness="2">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="double"/>
        public double LineStrokeThickness
        {
            get
            {
                return (double)GetValue(LineStrokeThicknessProperty);
            }

            set
            {
                SetValue(LineStrokeThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a collection of Double values that indicate the pattern of dashes and gaps that is used to outline node line.
        /// </summary>
        /// <value>
        /// Type: <see cref="System.Windows.Media.DoubleCollection" />
        /// A collection of Double values that specify the pattern of dashes and gaps.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set LineStrokeDashArray property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             DoubleCollection dcollection= new DoubleCollection();
        ///             dcollection.Add(2);
        ///             myTreeView.StrokeDashArray = dcollection;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set LineStrokeDashArray property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" LineStrokeDashArray="2">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="DoubleCollection"/>
        public DoubleCollection LineStrokeDashArray
        {
            get
            {
                return (DoubleCollection)GetValue(LineStrokeDashArrayProperty);
            }

            set
            {
                SetValue(LineStrokeDashArrayProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a Double that specifies the distance within the dash pattern where a dash begins in node line.
        /// </summary>
        /// <value>
        /// Type: <see cref="System.Windows.Media.DoubleCollection" />
        /// A collection of Double values that specify the pattern of dashes and gaps.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set LineStrokeDashOffset property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             myTreeView.LineStrokeDashOffset = 0.5d;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set LineStrokeDashOffset property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" LineStrokeDashOffset="0.5">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="DoubleCollection"/>
        public double LineStrokeDashOffset
        {
            get
            {
                return (double)GetValue(LineStrokeDashOffsetProperty);
            }

            set
            {
                SetValue(LineStrokeDashOffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a brush that describes the pen of a node line.
        /// </summary>
        /// <value>
        /// Type: <see cref="Pen"/>
        /// The pen that is used to fill the line's node.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set LinePen property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             myTreeView.LinePen = new Pen( Brushes.Red, 1 );
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set LinePen property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview">
        ///     <local:TreeViewAdv.LinePen>
        ///         <Pen Brush="Red" Thickness="2"/>
        ///     </local:TreeViewAdv.LinePen>
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="Pen"/>
        public Pen LinePen
        {
            get
            {
                return (Pen)GetValue(LinePenProperty);
            }

            set
            {
                SetValue(LinePenProperty, value);
            }
        }

        #endregion DP Setters & getters

        #region Dependency property

        /// <summary>
        /// Identifies TreeViewAdv. LineBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty LineBrushProperty =
            DependencyProperty.Register("LineBrush", typeof(Brush), typeof(TreeRootLine), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies TreeViewAdv. LinePen dependency property.
        /// </summary>
        public static readonly DependencyProperty LinePenProperty =
            DependencyProperty.Register("LinePen", typeof(Pen), typeof(TreeRootLine), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies TreeViewAdv. LineStrokeThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty LineStrokeThicknessProperty =
            DependencyProperty.Register("LineStrokeThickness", typeof(double), typeof(TreeRootLine), new FrameworkPropertyMetadata(1.0d, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies TreeViewAdv. LinePen dependency property.
        /// </summary>
        public static readonly DependencyProperty LineStrokeDashArrayProperty =
            DependencyProperty.Register("LineStrokeDashArray", typeof(DoubleCollection), typeof(TreeRootLine), new FrameworkPropertyMetadata(dcollection, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies TreeViewAdv. LinePen dependency property.
        /// </summary>
        public static readonly DependencyProperty LineStrokeDashOffsetProperty =
            DependencyProperty.Register("LineStrokeDashOffset", typeof(double), typeof(TreeRootLine), new FrameworkPropertyMetadata(0.5d, FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion Dependency property

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="TreeRootLine"/> class.
        /// </summary>
        static TreeRootLine()
        {
            List<double> values = new List<double>();
            values.Add(2);
            values.Add(2);
            dcollection = new DoubleCollection(values);
        }

        #endregion Initialization

        #region Implementation

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            GuidelineSet guidelines = new GuidelineSet();
            guidelines.GuidelinesX.Add(0.5);
            guidelines.GuidelinesY.Add(0.5);

            drawingContext.PushGuidelineSet(guidelines);

            if (LinePen == null)
            {
                LinePen = GetLinePen();
            }

            if (IsVerticalLine)
            {
                if (IsVista())
                {
                    drawingContext.DrawLine(LinePen, new Point(0, 0), new Point(0, this.RenderSize.Height));
                }
                else
                {
                    drawingContext.DrawLine(LinePen, new Point(0, 0), new Point(0, this.RenderSize.Height - 0.7d));
                }
            }
            else
            {
                drawingContext.DrawLine(LinePen, new Point(0, 0), new Point(this.RenderSize.Width, 0));
            }
        }

        /// <summary>
        /// Gets the line pen.
        /// </summary>
        /// <returns>Pen of linePen</returns>
        private Pen GetLinePen()
        {
            Pen linePen = new Pen(LineBrush, 1);
            linePen.DashStyle = DashStyles.Dot;
            linePen.DashCap = PenLineCap.Square;
            linePen.StartLineCap = PenLineCap.Square;
            linePen.EndLineCap = PenLineCap.Square;
            linePen.LineJoin = PenLineJoin.Miter;
            linePen.Thickness = LineStrokeThickness;
            return linePen;
        }

        /// <summary>
        /// Defines whether this is Vista OS.
        /// </summary>
        /// <returns>
        /// True if this is Vista OS; otherwise, false.
        /// </returns>
        /// <property name="flag" value="Finished"/>
        internal static bool IsVista()
        {
            return Environment.OSVersion.Version.Major >= 6;
        }

        #endregion Implementation
    }
}