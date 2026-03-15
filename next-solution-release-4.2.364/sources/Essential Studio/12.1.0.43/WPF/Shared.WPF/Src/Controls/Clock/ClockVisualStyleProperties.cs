// <copyright file="ClockVisualStyleProperties.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Clock class that provides us the visual style properties of clock control
    /// </summary>
    public partial class Clock
    {
        #region Constants
        /// <summary>
        /// Contains default value for some Brush type properties.
        /// </summary>
        private static readonly Brush c_defaultBrushValue = Brushes.Transparent;

        /// <summary>
        /// Contains default value for some Color type properties.
        /// </summary>
        private static readonly Color c_defaultColorValue = Colors.Transparent;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets date and time of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="DateTime"/>
        /// Provides DateTime value for the <see cref="Clock"/>. The default value of the DateTime property is DateTime.Now.
        /// </value>
        /// <remarks>
        /// DateTime dependency property is used for getting or setting date or time of the <see cref="Clock"/>. 
        /// Every tick of the clock updates DateTime property. Also DateTime property is updated when you drag 
        /// hands of the clock of do mouse wheel operations with clock hands.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set DateTime property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.DateTime = new DateTime( 2006, 1, 1, 1, 1, 1 );
        ///         }
        ///     }
        /// }
        /// </code> 
        /// <para/>
        /// <para/>This example shows how to set DateTime property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" DateTime="1/1/2005 2:20" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="DateTime"/>
        public DateTime DateTime
        {
            get
            {
                return (DateTime)GetValue(DateTimeProperty);
            }

            set
            {
                SetValue(DateTimeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets CornerRadius value of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="CornerRadius"/>
        /// Provides ClockCornerRadius value for the <see cref="Clock"/>. The default value of the ClockCornerRadius property is 90.
        /// </value>
        /// <remarks>
        /// ClockCornerRadius dependency property defines corner radius of the <see cref="Clock"/>. 
        /// If you set low ClockCornerRadius, the clock will be square. 
        /// If you set high ClockCornerRadius, the clock will be round. 
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set ClockCornerRadius property in C#.
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
        ///             customControlClock.ClockCornerRadius = new CornerRadius( 30 );
        ///         }
        ///     }
        /// }
        /// </code> 
        /// <para/>
        /// <para/>This example shows how to set ClockCornerRadius property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" ClockCornerRadius="10,30,10,30" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="CornerRadius"/>
        public CornerRadius ClockCornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(ClockCornerRadiusProperty);
            }

            set
            {
                SetValue(ClockCornerRadiusProperty, value);            
            }
        }

        /// <summary>
        /// Gets or sets border thickness value of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// Provides BorderThickness value for the <see cref="Clock"/>. The default value of the BorderThickness property is 2.
        /// </value>
        /// <remarks>
        /// BorderThickness dependency property defines border thickness of the <see cref="Clock"/>. 
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set BorderThickness property in C#.
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
        ///             customControlClock.BorderThickness = new Thickness( 2 );
        ///         }
        ///     }
        /// }
        /// </code> 
        /// <para/>
        /// <para/>This example shows how to set BorderThickness property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" BorderThickness="2" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="InnerBorderThickness"/>
        /// <seealso cref="DialBorderThickness"/>
        /// <seealso cref="Thickness"/>
        public new Thickness BorderThickness
        {
            get
            {              
                return (Thickness)GetValue(BorderThicknessProperty);
            }

            set
            {
                SetValue(BorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets second hand thickness value of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Provides SecondHandThickness value for the <see cref="Clock"/>. The default value of the SecondHandThickness property is 2.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set SecondHandThickness property in C#.
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
        ///             customControlClock.SecondHandThickness = new Thickness( 3 );
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>
        /// <para/>This example shows how to set SecondHandThickness property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" SecondHandThickness="3" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="double"/>
        public double SecondHandThickness
        {
            get
            {
                return (double)GetValue(SecondHandThicknessProperty);
            }

            set
            {
                SetValue(SecondHandThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets inner border thickness value of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// Provides InnerBorderThickness value for the <see cref="Clock"/>. The default value of the InnerBorderThickness property is 2.
        /// </value>
        /// <remarks>
        /// InnerBorderThickness dependency property defines inner border thickness of the <see cref="Clock"/>. 
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set InnerBorderThickness property in C#.
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
        ///             customControlClock.InnerBorderThickness = new Thickness( 3 );
        ///         }
        ///     }
        /// }
        /// </code>       
        /// <para/>This example shows how to set InnerBorderThickness property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" InnerBorderThickness="3" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// <para/>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="BorderThickness"/>
        /// <seealso cref="DialBorderThickness"/>
        /// <seealso cref="Thickness"/>
        public Thickness InnerBorderThickness
        {
            get
            {
                return (Thickness)GetValue(InnerBorderThicknessProperty);
            }

            set
            {
                SetValue(InnerBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets dial border thickness value of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// Provides DialBorderThickness value for the <see cref="Clock"/>. The default value of the InnerBorderThickness property is 25.
        /// </value>
        /// <remarks>
        /// DialBorderThickness dependency property defines dial border thickness of the <see cref="Clock"/>. 
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set DialBorderThickness property in C#.
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
        ///             customControlClock.DialBorderThickness = new Thickness( 3 );
        ///         }
        ///     }
        /// }
        /// </code>   
        /// <para/>
        /// <para/>This example shows how to set DialBorderThickness property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" DialBorderThickness="3" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="BorderThickness"/>
        /// <seealso cref="InnerBorderThickness"/>
        /// <seealso cref="Thickness"/>
        public Thickness DialBorderThickness
        {
            get
            {
                return (Thickness)GetValue(DialBorderThicknessProperty);
            }

            set
            {
                SetValue(DialBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets AMPMSelectorPosition of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Position"/>
        /// Provides AMPMSelectorPosition value for the <see cref="Clock"/>. The default value of the AMPMSelectorPosition property is Bottom.
        /// </value>
        /// <remarks>
        /// You can set one of four values for AMPMSelectorPosition dependency property (Top, Left, Right, Bottom). 
        /// To see the result of setting AMPMSelectorPosition dependency property you should set <see cref="IsInsideAmPmVisible"/>   
        /// dependency property value to True.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set AMPMSelectorPosition property in C#.
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
        ///             customControlClock.IsInsideAmPmVisible = true;
        ///             customControlClock.AMPMSelectorPosition = Clock.Position.Top;
        ///         }
        ///     }
        /// }
        /// </code>  
        /// <para/>
        /// <para/>This example shows how to set AMPMSelectorPosition property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" IsInsideAmPmVisible="True" AMPMSelectorPosition="3" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Position"/>
        public Position AMPMSelectorPosition
        {
            get
            {
                return (Position)GetValue(AMPMSelectorPositionProperty);
            }

            set
            {
                SetValue(AMPMSelectorPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets InnerBorderBrush of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides InnerBorderBrush value for the <see cref="Clock"/>. The default value of the InnerBorderBrush property is Transparent.
        /// </value>
        /// <remarks>
        /// InnerBorderBrush dependency property defines inner border brush of the <see cref="Clock"/>. 
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set InnerBorderBrush property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.InnerBorderBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>  
        /// <para/>
        /// <para/>This example shows how to set InnerBorderBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" InnerBorderBrush="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush InnerBorderBrush
        {
            get
            {
                return (Brush)GetValue(InnerBorderBrushProperty);
            }

            set
            {
                SetValue(InnerBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets DialBackground of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides DialBackground value for the <see cref="Clock"/>. The default value of the DialBackground property is Transparent.
        /// </value>
        /// <remarks>
        /// DialBackground property sets background to the place near inner border.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set DialBackground property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.DialBackground = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code> 
        /// <para/>
        /// <para/>This example shows how to set DialBackground property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" DialBackground="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush DialBackground
        {
            get
            {
                return (Brush)GetValue(DialBackgroundProperty);
            }

            set
            {
                SetValue(DialBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets DialCenterBackground of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides DialCenterBackground value for the <see cref="Clock"/>. The default value of the DialCenterBackground property is Transparent.
        /// </value>
        /// <remarks>
        /// DialCenterBackground property sets background to the place near center of the clock.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set DialCenterBackground property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.DialCenterBackground = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>  
        /// <para/>
        /// <para/>This example shows how to set DialCenterBackground property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" DialCenterBackground="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush DialCenterBackground
        {
            get
            {
                return (Brush)GetValue(DialCenterBackgroundProperty);
            }

            set
            {
                SetValue(DialCenterBackgroundProperty, value);           
            }
        }

        /// <summary>
        /// Gets or sets BorderBrush of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides BorderBrush value for the <see cref="Clock"/>. The default value of the BorderBrush property is Transparent.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set BorderBrush property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.BorderBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>  
        /// <para/>
        /// <para/>This example shows how to set BorderBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" BorderBrush="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public new Brush BorderBrush
        {
            get
            {
                return (Brush)GetValue(BorderBrushProperty);
            }

            set
            {
                SetValue(BorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets FrameBrush of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides ClockFrameBrush value for the <see cref="Clock"/>. The default value of the ClockFrameBrush property is Transparent.
        /// </value>
        /// <remarks>
        /// ClockFrameBrush property sets background to the frame of the <see cref="Clock"/>.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set ClockFrameBrush property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.ClockFrameBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>  
        /// <para/>
        /// <para/>This example shows how to set ClockFrameBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" ClockFrameBrush="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush ClockFrameBrush
        {
            get
            {
                return (Brush)GetValue(ClockFrameBrushProperty);
            }

            set
            {
                SetValue(ClockFrameBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets FrameBorderThickness of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// Provides FrameBorderThickness value for the <see cref="Clock"/>. The default value of the FrameBorderThickness property is 1.
        /// </value>
        /// <remarks>
        /// FrameBorderThickness property sets thickness to the frame of the <see cref="Clock"/>.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set FrameBorderThickness property in C#.
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
        ///             customControlClock.FrameBorderThickness = new Thickness( 3 );
        ///         }
        ///     }
        /// }
        /// </code> 
        /// <para/>
        /// <para/>This example shows how to set FrameBorderThickness property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" FrameBorderThickness="3" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Thickness"/>
        public Thickness FrameBorderThickness
        {
            get
            {
                return (Thickness)GetValue(FrameBorderThicknessProperty);
            }

            set
            {
                SetValue(FrameBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets FrameInnerBorderThickness of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// Provides FrameInnerBorderThickness value for the <see cref="Clock"/>. The default value of the FrameInnerBorderThickness property is "1,1,1,0".
        /// </value>
        /// <remarks>
        /// FrameInnerBorderThickness property sets thickness to the frame that is near the main frame of the <see cref="Clock"/>.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set FrameInnerBorderThickness property in C#.
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
        ///             customControlClock.FrameInnerBorderThickness = new Thickness( 3 );
        ///         }
        ///     }
        /// }
        /// </code>   
        /// <para/>
        /// <para/>This example shows how to set FrameInnerBorderThickness property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" FrameInnerBorderThickness="3" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Thickness"/>
        public Thickness FrameInnerBorderThickness
        {
            get
            {
                return (Thickness)GetValue(FrameInnerBorderThicknessProperty);
            }

            set
            {
                SetValue(FrameInnerBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets FrameBorderBrush of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides FrameBorderBrush value for the <see cref="Clock"/>. The default value of the FrameBorderBrush property is Transparent.
        /// </value>
        /// <remarks>
        /// FrameBorderBrush property sets brush to the frame of the <see cref="Clock"/>.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set FrameBorderBrush property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.FrameBorderBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code> 
        /// <para/>
        /// <para/>This example shows how to set FrameBorderBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" FrameBorderBrush="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush FrameBorderBrush
        {
            get
            {
                return (Brush)GetValue(FrameBorderBrushProperty);
            }

            set
            {
                SetValue(FrameBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets FrameInnerBorderBrush of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides FrameInnerBorderBrush value for the <see cref="Clock"/>. The default value of the FrameInnerBorderBrush property is Transparent.
        /// </value>
        /// <remarks>
        /// FrameInnerBorderBrush property sets brush to the frame that is near the main frame of the <see cref="Clock"/>.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set FrameInnerBorderBrush property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.FrameInnerBorderBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code> 
        /// <para/>
        /// <para/>This example shows how to set FrameInnerBorderBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" FrameInnerBorderBrush="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush FrameInnerBorderBrush
        {
            get
            {
                return (Brush)GetValue(FrameInnerBorderBrushProperty);
            }

            set
            {
                SetValue(FrameInnerBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets FrameBackground of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides FrameBackground value for the <see cref="Clock"/>. The default value of the FrameBackground property is Transparent.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set FrameBackground property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.FrameBackground = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code> 
        /// <para/>
        /// <para/>This example shows how to set FrameBackground property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" FrameBackground="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush FrameBackground
        {
            get
            {
                return (Brush)GetValue(FrameBackgroundProperty);
            }

            set
            {
                SetValue(FrameBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets FrameCornerRadius value of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="CornerRadius"/>
        /// Provides FrameCornerRadius value for the <see cref="Clock"/>. The default value of the FrameCornerRadius property is 0.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set FrameCornerRadius property in C#.
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
        ///             customControlClock.FrameCornerRadius = new CornerRadius( 30 );
        ///         }
        ///     }
        /// }
        /// </code>   
        /// <para/>
        /// <para/>This example shows how to set FrameCornerRadius property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" FrameCornerRadius="10,30,10,30" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="CornerRadius"/>
        public CornerRadius FrameCornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(FrameCornerRadiusProperty);
            }

            set
            {
                SetValue(FrameCornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets AMPMSelectorBorderThickness of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// Provides AMPMSelectorBorderThickness value for the <see cref="Clock"/>. The default value of the AMPMSelectorBorderThickness property is 1.
        /// </value>
        /// <remarks>
        /// AMPMSelectorBorderThickness sets border thickness to both inside am/pm selector and non-editable digital clock.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set AMPMSelectorBorderThickness property in C#.
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
        ///             customControlClock.AMPMSelectorBorderThickness = new Thickness( 3 );
        ///         }
        ///     }
        /// }
        /// </code> 
        /// <para/>
        /// <para/>This example shows how to set AMPMSelectorBorderThickness property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" AMPMSelectorBorderThickness="3" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Thickness"/>
        public Thickness AMPMSelectorBorderThickness
        {
            get
            {
                return (Thickness)GetValue(AMPMSelectorBorderThicknessProperty);
            }

            set
            {
                SetValue(AMPMSelectorBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets AMPMSelectorBorderBrush of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides AMPMSelectorBorderBrush value for the <see cref="Clock"/>. The default value of the AMPMSelectorBorderBrush property is Transparent.
        /// </value>
        /// <remarks>
        /// AMPMSelectorBorderBrush sets border brush to both inside am/pm selector and non-editable digital clock.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set AMPMSelectorBorderBrush property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.AMPMSelectorBorderBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>    
        /// <para/>
        /// <para/>This example shows how to set AMPMSelectorBorderBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" AMPMSelectorBorderBrush="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush AMPMSelectorBorderBrush
        {
            get
            {
                return (Brush)GetValue(AMPMSelectorBorderBrushProperty);
            }

            set
            {
                SetValue(AMPMSelectorBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets AMPMSelectorBackground of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides AMPMSelectorBackground value for the <see cref="Clock"/>. The default value of the AMPMSelectorBackground property is Transparent.
        /// </value>
        /// <remarks>
        /// AMPMSelectorBackground sets background to both inside am/pm selector and non-editable digital clock.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set AMPMSelectorBackground property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.AMPMSelectorBackground = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>   
        /// <para/>
        /// <para/>This example shows how to set AMPMSelectorBackground property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" AMPMSelectorBackground="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush AMPMSelectorBackground
        {
            get
            {
                return (Brush)GetValue(AMPMSelectorBackgroundProperty);
            }

            set
            {
                SetValue(AMPMSelectorBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets AMPMSelectorForeground of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides AMPMSelectorForeground value for the <see cref="Clock"/>. The default value of the AMPMSelectorForeground property is Transparent.
        /// </value>
        /// <remarks>
        /// AMPMSelectorForeground sets foreground to both inside am/pm selector and non-editable digital clock.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set AMPMSelectorForeground property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.AMPMSelectorForeground = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code> 
        /// <para/>
        /// <para/>This example shows how to set AMPMSelectorForeground property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" AMPMSelectorForeground="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush AMPMSelectorForeground
        {
            get
            {
                return (Brush)GetValue(AMPMSelectorForegroundProperty);
            }

            set
            {
                SetValue(AMPMSelectorForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets AMPMSelectorButtonsArrowBrush of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides AMPMSelectorButtonsArrowBrush value for the <see cref="Clock"/>. The default value of the AMPMSelectorButtonsArrowBrush property is Transparent.
        /// </value>
        /// <remarks>
        /// AMPMSelectorButtonsArrowBrush sets buttons arrow brush to both inside am/pm selector and non-editable digital clock.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set AMPMSelectorButtonsArrowBrush property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.AMPMSelectorButtonsArrowBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>   
        /// <para/>
        /// <para/>This example shows how to set AMPMSelectorButtonsArrowBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" AMPMSelectorButtonsArrowBrush="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush AMPMSelectorButtonsArrowBrush
        {
            get
            {
                return (Brush)GetValue(AMPMSelectorButtonsArrowBrushProperty);
            }

            set
            {
                SetValue(AMPMSelectorButtonsArrowBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets AMPMSelectorButtonsBackground of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides AMPMSelectorButtonsBackground value for the <see cref="Clock"/>. The default value of the AMPMSelectorButtonsBackground property is Transparent.
        /// </value>
        /// <remarks>
        /// AMPMSelectorButtonsBackground sets buttons background to both inside am/pm selector and non-editable digital clock.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set AMPMSelectorButtonsBackground property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.AMPMSelectorButtonsBackground = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>    
        /// <para/>
        /// <para/>This example shows how to set AMPMSelectorButtonsBackground property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" AMPMSelectorButtonsBackground="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush AMPMSelectorButtonsBackground
        {
            get
            {
                return (Brush)GetValue(AMPMSelectorButtonsBackgroundProperty);
            }

            set
            {
                SetValue(AMPMSelectorButtonsBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets AMPMSelectorCornerRadius value of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="CornerRadius"/>
        /// Provides AMPMSelectorCornerRadius value for the <see cref="Clock"/>. The default value of the AMPMSelectorCornerRadius property is 0.
        /// </value>
        /// <remarks>
        /// AMPMSelectorCornerRadius sets corner radius to both inside am/pm selector and non-editable digital clock.
        /// </remarks>
        /// <remarks>
        /// Default value of the AMPMSelectorCornerRadius property is 0.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set AMPMSelectorCornerRadius property in C#.
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
        ///             customControlClock.AMPMSelectorCornerRadius = new CornerRadius( 2 );
        ///         }
        ///     }
        /// }
        /// </code> 
        /// <para/>
        /// <para/>This example shows how to set AMPMSelectorCornerRadius property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" AMPMSelectorCornerRadius="2" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="CornerRadius"/>
        public CornerRadius AMPMSelectorCornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(AMPMSelectorCornerRadiusProperty);
            }

            set
            {
                SetValue(AMPMSelectorCornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets AMPMSelectorButtonsBorderBrush of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides AMPMSelectorButtonsBorderBrush value for the <see cref="Clock"/>. The default value of the AMPMSelectorButtonsBorderBrush property is Transparent.
        /// </value>
        /// <remarks>
        /// AMPMSelectorButtonsBorderBrush sets buttons border brush to both inside am/pm selector and non-editable digital clock.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set AMPMSelectorButtonsBorderBrush property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.AMPMSelectorButtonsBorderBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>  
        /// <para/>
        /// <para/>This example shows how to set AMPMSelectorButtonsBorderBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" AMPMSelectorButtonsBorderBrush="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush AMPMSelectorButtonsBorderBrush
        {
            get
            {
                return (Brush)GetValue(AMPMSelectorButtonsBorderBrushProperty);
            }

            set
            {
                SetValue(AMPMSelectorButtonsBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets AMPMMouseOverButtonsBorderBrush of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides AMPMMouseOverButtonsBorderBrush value for the <see cref="Clock"/>. The default value of the AMPMMouseOverButtonsBorderBrush property is Transparent.
        /// </value>
        /// <remarks>
        /// AMPMMouseOverButtonsBorderBrush sets buttons border brush to both inside am/pm selector and non-editable digital clock when mouse over the buttons.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set AMPMMouseOverButtonsBorderBrush property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.AMPMMouseOverButtonsBorderBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>  
        /// <para/>
        /// <para/>This example shows how to set AMPMMouseOverButtonsBorderBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" AMPMMouseOverButtonsBorderBrush="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush AMPMMouseOverButtonsBorderBrush
        {
            get
            {
                return (Brush)GetValue(AMPMMouseOverButtonsBorderBrushProperty);
            }

            set
            {
                SetValue(AMPMMouseOverButtonsBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets AMPMMouseOverButtonsArrowBrush of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides AMPMMouseOverButtonsArrowBrush value for the <see cref="Clock"/>. The default value of the AMPMMouseOverButtonsArrowBrush property is Transparent.
        /// </value>
        /// <remarks>
        /// AMPMMouseOverButtonsArrowBrush sets buttons arrow brush to both inside am/pm selector and non-editable digital clock when mouse over the buttons.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set AMPMMouseOverButtonsArrowBrush property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.AMPMMouseOverButtonsArrowBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>  
        /// <para/>
        /// <para/>This example shows how to set AMPMMouseOverButtonsArrowBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" AMPMMouseOverButtonsArrowBrush="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush AMPMMouseOverButtonsArrowBrush
        {
            get
            {
                return (Brush)GetValue(AMPMMouseOverButtonsArrowBrushProperty);
            }

            set
            {
                SetValue(AMPMMouseOverButtonsArrowBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets AMPMMouseOverButtonsBackground of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides AMPMMouseOverButtonsBackground value for the <see cref="Clock"/>. The default value of the AMPMMouseOverButtonsBackground property is Transparent.
        /// </value>
        /// <remarks>
        /// AMPMMouseOverButtonsBackground sets buttons background to both inside am/pm selector and non-editable digital clock when mouse over the buttons.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set AMPMMouseOverButtonsBackground property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.AMPMMouseOverButtonsBackground = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>  
        /// <para/>
        /// <para/>This example shows how to set AMPMMouseOverButtonsBackground property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" AMPMMouseOverButtonsBackground="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush AMPMMouseOverButtonsBackground
        {
            get
            {
                return (Brush)GetValue(AMPMMouseOverButtonsBackgroundProperty);
            }

            set
            {
                SetValue(AMPMMouseOverButtonsBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets PointColor of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides ClockPointBrush value for the <see cref="Clock"/>. The default value of the ClockPointBrush property is Transparent.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set ClockPointBrush property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.ClockPointBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>  
        /// <para/>
        /// <para/>This example shows how to set ClockPointBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" ClockPointBrush="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush ClockPointBrush
        {
            get
            {
                return (Brush)GetValue(ClockPointBrushProperty);
            }

            set
            {
                SetValue(ClockPointBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets CenterCircleBrush of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides CenterCircleBrush value for the <see cref="Clock"/>. The default value of the CenterCircleBrush property is Transparent.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set CenterCircleBrush property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.CenterCircleBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code> 
        /// <para/>
        /// <para/>This example shows how to set CenterCircleBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" CenterCircleBrush="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush CenterCircleBrush
        {
            get
            {
                return (Brush)GetValue(CenterCircleBrushProperty);
            }

            set
            {
                SetValue(CenterCircleBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets SecondHandBrush of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides SecondHandBrush value for the <see cref="Clock"/>. The default value of the SecondHandBrush property is Transparent.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set SecondHandBrush property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.SecondHandBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>  
        /// <para/>
        /// <para/>This example shows how to set SecondHandBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" SecondHandBrush="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush SecondHandBrush
        {
            get
            {
                return (Brush)GetValue(SecondHandBrushProperty);
            }

            set
            {
                SetValue(SecondHandBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets SecondHandMouseOverBrush of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides SecondHandMouseOverBrush value for the <see cref="Clock"/>. The default value of the SecondHandMouseOverBrush property is Transparent.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set SecondHandMouseOverBrush property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.SecondHandMouseOverBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>  
        /// <para/>
        /// <para/>This example shows how to set SecondHandMouseOverBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" SecondHandMouseOverBrush="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush SecondHandMouseOverBrush
        {
            get
            {
                return (Brush)GetValue(SecondHandMouseOverBrushProperty);
            }

            set
            {
                SetValue(SecondHandMouseOverBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets MinuteHandBrush of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides MinuteHandBrush value for the <see cref="Clock"/>. The default value of the MinuteHandBrush property is Transparent.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set MinuteHandBrush property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.MinuteHandBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code> 
        /// <para/>
        /// <para/>This example shows how to set MinuteHandBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" MinuteHandBrush="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush MinuteHandBrush
        {
            get
            {
                return (Brush)GetValue(MinuteHandBrushProperty);
            }

            set
            {
                SetValue(MinuteHandBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets MinuteHandBorderBrush of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides MinuteHandBorderBrush value for the <see cref="Clock"/>. The default value of the MinuteHandBorderBrush property is Transparent.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set MinuteHandBorderBrush property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.MinuteHandBorderBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>  
        /// <para/>
        /// <para/>This example shows how to set MinuteHandBorderBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" MinuteHandBorderBrush="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush MinuteHandBorderBrush
        {
            get
            {
                return (Brush)GetValue(MinuteHandBorderBrushProperty);
            }

            set
            {
                SetValue(MinuteHandBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets MinuteHandMouseOverBrush of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides MinuteHandMouseOverBrush value for the <see cref="Clock"/>. The default value of the MinuteHandMouseOverBrush property is Transparent.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set MinuteHandMouseOverBrush property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.MinuteHandMouseOverBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>  
        /// <para/>
        /// <para/>This example shows how to set MinuteHandMouseOverBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" MinuteHandMouseOverBrush="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush MinuteHandMouseOverBrush
        {
            get
            {
                return (Brush)GetValue(MinuteHandMouseOverBrushProperty);
            }

            set
            {
                SetValue(MinuteHandMouseOverBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets MinuteHandMouseOverBorderBrush of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides MinuteHandMouseOverBorderBrush value for the <see cref="Clock"/>. The default value of the MinuteHandMouseOverBorderBrush property is Transparent.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set MinuteHandMouseOverBorderBrush property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.MinuteHandMouseOverBorderBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>   
        /// <para/>
        /// <para/>This example shows how to set MinuteHandMouseOverBorderBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" MinuteHandMouseOverBorderBrush="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush MinuteHandMouseOverBorderBrush
        {
            get
            {
                return (Brush)GetValue(MinuteHandMouseOverBorderBrushProperty);
            }

            set
            {
                SetValue(MinuteHandMouseOverBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets HourHandBrush of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides HourHandBrush value for the <see cref="Clock"/>. The default value of the HourHandBrush property is Transparent.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set HourHandBrush property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.HourHandBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>  
        /// <para/>
        /// <para/>This example shows how to set HourHandBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" HourHandBrush="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush HourHandBrush
        {
            get
            {
                return (Brush)GetValue(HourHandBrushProperty);
            }

            set
            {
                SetValue(HourHandBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets HourHandBorderBrush of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides HourHandBorderBrush value for the <see cref="Clock"/>. The default value of the HourHandBorderBrush property is Transparent.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set HourHandBorderBrush property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.HourHandBorderBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>    
        /// <para/>
        /// <para/>This example shows how to set HourHandBorderBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" HourHandBorderBrush="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush HourHandBorderBrush
        {
            get
            {
                return (Brush)GetValue(HourHandBorderBrushProperty);
            }

            set
            {
                SetValue(HourHandBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets HourHandMouseOverBrush of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides HourHandMouseOverBrush value for the <see cref="Clock"/>. The default value of the HourHandMouseOverBrush property is Transparent.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set HourHandMouseOverBrush property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.HourHandMouseOverBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>   
        /// <para/>
        /// <para/>This example shows how to set HourHandMouseOverBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" HourHandMouseOverBrush="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush HourHandMouseOverBrush
        {
            get
            {
                return (Brush)GetValue(HourHandMouseOverBrushProperty);
            }

            set
            {
                SetValue(HourHandMouseOverBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets HourHandMouseOverBorderBrush of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides HourHandMouseOverBorderBrush value for the <see cref="Clock"/>. The default value of the HourHandMouseOverBorderBrush property is Transparent.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set HourHandMouseOverBorderBrush property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.HourHandMouseOverBorderBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code> 
        /// <para/>
        /// <para/>This example shows how to set HourHandMouseOverBorderBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" HourHandMouseOverBorderBrush="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush HourHandMouseOverBorderBrush
        {
            get
            {
                return (Brush)GetValue(HourHandMouseOverBorderBrushProperty);
            }

            set
            {
                SetValue(HourHandMouseOverBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets HourHandPressedBrush of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides HourHandPressedBrush value for the <see cref="Clock"/>. The default value of the HourHandPressedBrush property is Transparent.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set HourHandPressedBrush property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.HourHandPressedBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>   
        /// <para/>
        /// <para/>This example shows how to set HourHandPressedBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" HourHandPressedBrush="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush HourHandPressedBrush
        {
            get
            {
                return (Brush)GetValue(HourHandPressedBrushProperty);
            }

            set
            {
                SetValue(HourHandPressedBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets MinuteHandPressedBrush of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides MinuteHandPressedBrush value for the <see cref="Clock"/>. The default value of the MinuteHandPressedBrush property is Transparent.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set MinuteHandPressedBrush property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.MinuteHandPressedBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code> 
        /// <para/>
        /// <para/>This example shows how to set MinuteHandPressedBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" MinuteHandPressedBrush="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush MinuteHandPressedBrush
        {
            get
            {
                return (Brush)GetValue(MinuteHandPressedBrushProperty);
            }

            set
            {
                SetValue(MinuteHandPressedBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets SecondHandPressedBrush of the <see cref="Clock"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides SecondHandPressedBrush value for the <see cref="Clock"/>. The default value of the SecondHandPressedBrush property is Transparent.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set SecondHandPressedBrush property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             customControlClock.SecondHandPressedBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code> 
        /// <para/>
        /// <para/>This example shows how to set SecondHandPressedBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <local:Clock Name="customControlClock" SecondHandPressedBrush="Red" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="Brush"/>
        public Brush SecondHandPressedBrush
        {
            get
            {
                return (Brush)GetValue(SecondHandPressedBrushProperty);
            }

            set
            {
                SetValue(SecondHandPressedBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is inside am pm visible.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Provides IsInsideAmPmVisible value for the <see cref="Clock"/>. The default value of the IsInsideAmPmVisible property is False.
        /// </value>
        /// <remarks>
        /// If you set IsInsideAmPmVisible to True, it will be visible inside the clock.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set IsInsideAmPmVisible property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        /// public partial class Window1 : Window
        /// {
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// customControlClock.IsInsideAmPmVisible = True;
        /// }
        /// }
        /// }
        /// </code>
        /// <para/>
        /// <para/>This example shows how to set IsInsideAmPmVisible property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        /// <local:Clock Name="customControlClock" IsInsideAmPmVisible="True" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="bool"/>
        public bool IsInsideAmPmVisible
        {
            get
            {
                return (bool)GetValue(IsInsideAmPmVisibleProperty);
            }

            set
            {
                SetValue(IsInsideAmPmVisibleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is digital am pm visible.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Provides IsDigitalAmPmVisible value for the <see cref="Clock"/>. The default value of the IsDigitalAmPmVisible property is True.
        /// </value>
        /// <remarks>
        /// If you set IsDigitalAmPmVisible to False, you`ll not be able to change am/pm value of the <see cref="Clock"/> from the digital clock.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set IsDigitalAmPmVisible property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// using System.Windows.Media;
        /// namespace Sample1
        /// {
        /// public partial class Window1 : Window
        /// {
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// customControlClock.IsDigitalAmPmVisible = True;
        /// }
        /// }
        /// }
        /// </code>
        /// <para/>
        /// <para/>This example shows how to set IsDigitalAmPmVisible property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        /// <local:Clock Name="customControlClock" IsDigitalAmPmVisible="True" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Clock"/>
        /// <seealso cref="bool"/>
        public bool IsDigitalAmPmVisible
        {
            get
            {
                return (bool)GetValue(IsDigitalAmPmVisibleProperty);
            }

            set
            {
                SetValue(IsDigitalAmPmVisibleProperty, value);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when ClockCornerRadius property is changed.
        /// </summary>
        public event PropertyChangedCallback ClockCornerRadiusChanged;

        /// <summary>
        /// Event that is raised when BorderThickness property is changed.
        /// </summary>
        public event PropertyChangedCallback BorderThicknessChanged;

        /// <summary>
        /// Event that is raised when SecondHandThickness property is changed.
        /// </summary>
        public event PropertyChangedCallback SecondHandThicknessChanged;

        /// <summary>
        /// Event that is raised when inner BorderThickness property is changed.
        /// </summary>
        public event PropertyChangedCallback InnerBorderThicknessChanged;

        /// <summary>
        /// Event that is raised when dial BorderThickness property is changed.
        /// </summary>
        public event PropertyChangedCallback DialBorderThicknessChanged;

        /// <summary>
        /// Event that is raised when AMPMSelectorPosition property is changed.
        /// </summary>
        public event PropertyChangedCallback AMPMSelectorPositionChanged;

        /// <summary>
        /// Event that is raised when DateTime property is changed.
        /// </summary>
        public event PropertyChangedCallback DateTimeChanged;

        /// <summary>
        /// Event that is raised when BorderBrush property is changed.
        /// </summary>       
        public event PropertyChangedCallback BorderBrushChanged;

        /// <summary>
        /// Event that is raised when ClockFrameBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback ClockFrameBrushChanged;

        /// <summary>
        /// Event that is raised when DialBackground property is changed.
        /// </summary>
        public event PropertyChangedCallback DialBackgroundChanged;

        /// <summary>
        /// Event that is raised when DialCenterBackground property is changed.
        /// </summary>
        public event PropertyChangedCallback DialCenterBackgroundChanged;

        /// <summary>
        /// Event that is raised when InnerBorderBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback InnerBorderBrushChanged;

        /// <summary>
        /// Event that is raised when FrameBorderThickness property is changed.
        /// </summary>
        public event PropertyChangedCallback FrameBorderThicknessChanged;

        /// <summary>
        /// Event that is raised when FrameInnerBorderThickness property is changed.
        /// </summary>
        public event PropertyChangedCallback FrameInnerBorderThicknessChanged;

        /// <summary>
        /// Event that is raised when FrameBorderBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback FrameBorderBrushChanged;

        /// <summary>
        /// Event that is raised when FrameInnerBorderBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback FrameInnerBorderBrushChanged;

        /// <summary>
        /// Event that is raised when FrameBackground property is changed.
        /// </summary>
        public event PropertyChangedCallback FrameBackgroundChanged;

        /// <summary>
        /// Event that is raised when FrameCornerRadius property is changed.
        /// </summary>
        public event PropertyChangedCallback FrameCornerRadiusChanged;

        /// <summary>
        /// Event that is raised when AMPMSelectorBorderThickness property is changed.
        /// </summary>
        public event PropertyChangedCallback AMPMSelectorBorderThicknessChanged;

        /// <summary>
        /// Event that is raised when AMPMSelectorBorderBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback AMPMSelectorBorderBrushChanged;

        /// <summary>
        /// Event that is raised when AMPMSelectorBackground property is changed.
        /// </summary>
        public event PropertyChangedCallback AMPMSelectorBackgroundChanged;

        /// <summary>
        /// Event that is raised when AMPMSelectorForeground property is changed.
        /// </summary>
        public event PropertyChangedCallback AMPMSelectorForegroundChanged;

        /// <summary>
        /// Event that is raised when AMPMSelectorButtonsArrowBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback AMPMSelectorButtonsArrowBrushChanged;

        /// <summary>
        /// Event that is raised when AMPMSelectorButtonsBackground property is changed.
        /// </summary>
        public event PropertyChangedCallback AMPMSelectorButtonsBackgroundChanged;

        /// <summary>
        /// Event that is raised when AMPMSelectorCornerRadius property is changed.
        /// </summary>
        public event PropertyChangedCallback AMPMSelectorCornerRadiusChanged;

        /// <summary>
        /// Event that is raised when AMPMSelectorButtonsBorderBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback AMPMSelectorButtonsBorderBrushChanged;

        /// <summary>
        /// Event that is raised when AMPMMouseOverButtonsBorderBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback AMPMMouseOverButtonsBorderBrushChanged;

        /// <summary>
        /// Event that is raised when AMPMMouseOverButtonsArrowBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback AMPMMouseOverButtonsArrowBrushChanged;

        /// <summary>
        /// Event that is raised when AMPMMouseOverButtonsBackground property is changed.
        /// </summary>
        public event PropertyChangedCallback AMPMMouseOverButtonsBackgroundChanged;

        /// <summary>
        /// Event that is raised when ClockPointBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback ClockPointBrushChanged;

        /// <summary>
        /// Event that is raised when CenterCircleBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback CenterCircleBrushChanged;

        /// <summary>
        /// Event that is raised when SecondHandBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback SecondHandBrushChanged;

        /// <summary>
        /// Event that is raised when SecondHandMouseOverBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback SecondHandMouseOverBrushChanged;

        /// <summary>
        /// Event that is raised when MinuteHandBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback MinuteHandBrushChanged;

        /// <summary>
        /// Event that is raised when MinuteHandShadowBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback MinuteHandBorderBrushChanged;

        /// <summary>
        /// Event that is raised when MinuteHandMouseOverBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback MinuteHandMouseOverBrushChanged;

        /// <summary>
        /// Event that is raised when MinuteHandMouseOverBorderBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback MinuteHandMouseOverBorderBrushChanged;

        /// <summary>
        /// Event that is raised when HourHandBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback HourHandBrushChanged;

        /// <summary>
        /// Event that is raised when HourHandShadowBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback HourHandBorderBrushChanged;

        /// <summary>
        /// Event that is raised when HourHandMouseOverBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback HourHandMouseOverBrushChanged;

        /// <summary>
        /// Event that is raised when HourHandMouseOverBorderBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback HourHandMouseOverBorderBrushChanged;

        /// <summary>
        /// Event that is raised when HourHandPressedBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback HourHandPressedBrushChanged;

        /// <summary>
        /// Event that is raised when MinuteHandPressedBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback MinuteHandPressedBrushChanged;

        /// <summary>
        /// Event that is raised when SecondHandPressedBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback SecondHandPressedBrushChanged;

        /// <summary>
        /// Event that is raised when IsInsideAmPmVisible property is changed.
        /// </summary>
        public event PropertyChangedCallback IsInsideAmPmVisibleChanged;

        /// <summary>
        /// Event that is raised when IsDigitalAmPmVisible property is changed.
        /// </summary>
        public event PropertyChangedCallback IsDigitalAmPmVisibleChanged;
        #endregion

        #region Implementation
        /// <summary>
        /// Raises ClockCornerRadiusChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnClockCornerRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ClockCornerRadiusChanged != null)
            {
                ClockCornerRadiusChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnClockCornerRadiusChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnClockCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnClockCornerRadiusChanged(e);
        }

        /// <summary>
        /// Raises BorderThicknessChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnBorderThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (BorderThicknessChanged != null)
            {
                BorderThicknessChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnBorderThicknessChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnBorderThicknessChanged(e);
        }

        /// <summary>
        /// Raises SecondHandThicknessChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnSecondHandThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SecondHandThicknessChanged != null)
            {
                SecondHandThicknessChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnSecondHandThicknessChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSecondHandThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnSecondHandThicknessChanged(e);
        }

        /// <summary>
        /// Raises InnerBorderThicknessChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnInnerBorderThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (InnerBorderThicknessChanged != null)
            {
                InnerBorderThicknessChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnInnerBorderThicknessChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnInnerBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnInnerBorderThicknessChanged(e);
        }

        /// <summary>
        /// Raises DialBorderThicknessChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnDialBorderThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DialBorderThicknessChanged != null)
            {
                DialBorderThicknessChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnDialBorderThicknessChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDialBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnDialBorderThicknessChanged(e);
        }

        /// <summary>
        /// Raises AMPMSelectorPositionChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnAMPMSelectorPositionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AMPMSelectorPositionChanged != null)
            {
                AMPMSelectorPositionChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnAMPMSelectorPositionChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAMPMSelectorPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnAMPMSelectorPositionChanged(e);
        }

        /// <summary>
        /// Raises InnerBorderBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnInnerBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (InnerBorderBrushChanged != null)
            {
                InnerBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnInnerBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnInnerBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnInnerBorderBrushChanged(e);
        }

        /// <summary>
        /// Raises DialBackgroundChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnDialBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DialBackgroundChanged != null)
            {
                DialBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnDialBackgroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDialBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnDialBackgroundChanged(e);
        }

        /// <summary>
        /// Raises DialCenterBackgroundChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnDialCenterBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DialCenterBackgroundChanged != null)
            {
                DialCenterBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnDialCenterBackgroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDialCenterBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnDialCenterBackgroundChanged(e);
        }

        /// <summary>
        /// Raises BorderBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (BorderBrushChanged != null)
            {
                BorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnBorderBrushChanged(e);
        }

        /// <summary>
        /// Raises ClockFrameBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnClockFrameBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ClockFrameBrushChanged != null)
            {
                ClockFrameBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnClockFrameBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnClockFrameBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnClockFrameBrushChanged(e);
        }

        /// <summary>
        /// Raises FrameBorderThicknessChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnFrameBorderThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FrameBorderThicknessChanged != null)
            {
                FrameBorderThicknessChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnFrameBorderThicknessChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFrameBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnFrameBorderThicknessChanged(e);
        }

        /// <summary>
        /// Raises FrameInnerBorderThicknessChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnFrameInnerBorderThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FrameInnerBorderThicknessChanged != null)
            {
                FrameInnerBorderThicknessChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnFrameInnerBorderThicknessChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFrameInnerBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnFrameInnerBorderThicknessChanged(e);
        }

        /// <summary>
        /// Raises FrameBorderBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnFrameBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FrameBorderBrushChanged != null)
            {
                FrameBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnFrameBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFrameBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnFrameBorderBrushChanged(e);
        }

        /// <summary>
        /// Raises FrameInnerBorderBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnFrameInnerBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FrameInnerBorderBrushChanged != null)
            {
                FrameInnerBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnFrameInnerBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFrameInnerBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnFrameInnerBorderBrushChanged(e);
        }

        /// <summary>
        /// Raises FrameFrameBackgroundChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnFrameBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FrameBackgroundChanged != null)
            {
                FrameBackgroundChanged(this, e);            
            }
        }

        /// <summary>
        /// Calls OnFrameBackgroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFrameBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnFrameBackgroundChanged(e);
        }

        /// <summary>
        /// Raises FrameCornerRadiusChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnFrameCornerRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FrameCornerRadiusChanged != null)
            {
                FrameCornerRadiusChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnFrameCornerRadiusChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFrameCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnFrameCornerRadiusChanged(e);
        }

        /// <summary>
        /// Raises AMPMSelectorBorderThicknessChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnAMPMSelectorBorderThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AMPMSelectorBorderThicknessChanged != null)
            {
                AMPMSelectorBorderThicknessChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnAMPMSelectorBorderThicknessChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAMPMSelectorBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnAMPMSelectorBorderThicknessChanged(e);
        }

        /// <summary>
        /// Raises AMPMSelectorBorderBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnAMPMSelectorBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AMPMSelectorBorderBrushChanged != null)
            {
                AMPMSelectorBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnAMPMSelectorBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAMPMSelectorBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnAMPMSelectorBorderBrushChanged(e);
        }

        /// <summary>
        /// Raises AMPMSelectorBackgroundChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnAMPMSelectorBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AMPMSelectorBackgroundChanged != null)
            {
                AMPMSelectorBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnAMPMSelectorBackgroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAMPMSelectorBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnAMPMSelectorBackgroundChanged(e);
        }

        /// <summary>
        /// Raises AMPMSelectorForegroundChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnAMPMSelectorForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AMPMSelectorForegroundChanged != null)
            {
                AMPMSelectorForegroundChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnAMPMSelectorForegroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAMPMSelectorForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnAMPMSelectorForegroundChanged(e);
        }

        /// <summary>
        /// Raises AMPMSelectorButtonsArrowBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnAMPMSelectorButtonsArrowBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AMPMSelectorButtonsArrowBrushChanged != null)
            {
                AMPMSelectorButtonsArrowBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnAMPMSelectorButtonsArrowBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAMPMSelectorButtonsArrowBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnAMPMSelectorButtonsArrowBrushChanged(e);
        }

        /// <summary>
        /// Raises AMPMSelectorButtonsBackgroundChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnAMPMSelectorButtonsBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AMPMSelectorButtonsBackgroundChanged != null)
            {
                AMPMSelectorButtonsBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnAMPMSelectorButtonsBackgroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAMPMSelectorButtonsBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnAMPMSelectorButtonsBackgroundChanged(e);
        }

        /// <summary>
        /// Raises AMPMSelectorCornerRadiusChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnAMPMSelectorCornerRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AMPMSelectorCornerRadiusChanged != null)
            {
                AMPMSelectorCornerRadiusChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnAMPMSelectorCornerRadiusChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAMPMSelectorCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnAMPMSelectorCornerRadiusChanged(e);
        }

        /// <summary>
        /// Raises AMPMSelectorButtonsBorderBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnAMPMSelectorButtonsBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AMPMSelectorButtonsBorderBrushChanged != null)
            {
                AMPMSelectorButtonsBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnAMPMSelectorButtonsBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAMPMSelectorButtonsBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnAMPMSelectorButtonsBorderBrushChanged(e);
        }

        /// <summary>
        /// Raises AMPMMouseOverButtonsBorderBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnAMPMMouseOverButtonsBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AMPMMouseOverButtonsBorderBrushChanged != null)
            {
                AMPMMouseOverButtonsBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnAMPMMouseOverButtonsBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAMPMMouseOverButtonsBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnAMPMMouseOverButtonsBorderBrushChanged(e);
        }

        /// <summary>
        /// Raises AMPMMouseOverButtonsArrowBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnAMPMMouseOverButtonsArrowBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AMPMMouseOverButtonsArrowBrushChanged != null)
            {
                AMPMMouseOverButtonsArrowBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnAMPMMouseOverButtonsArrowBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAMPMMouseOverButtonsArrowBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnAMPMMouseOverButtonsArrowBrushChanged(e);
        }

        /// <summary>
        /// Raises AMPMMouseOverButtonsBackgroundChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnAMPMMouseOverButtonsBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AMPMMouseOverButtonsBackgroundChanged != null)
            {
                AMPMMouseOverButtonsBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnAMPMMouseOverButtonsBackgroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAMPMMouseOverButtonsBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnAMPMMouseOverButtonsBackgroundChanged(e);
        }

        /// <summary>
        /// Raises ClockPointBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnClockPointBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ClockPointBrushChanged != null)
            {
                ClockPointBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnClockPointBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnClockPointBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnClockPointBrushChanged(e);
        }

        /// <summary>
        /// Raises CenterCircleBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnCenterCircleBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CenterCircleBrushChanged != null)
            {
                CenterCircleBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnCenterCircleBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCenterCircleBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnCenterCircleBrushChanged(e);
        }

        /// <summary>
        /// Raises SecondHandBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnSecondHandBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SecondHandBrushChanged != null)
            {
                SecondHandBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnSecondHandBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSecondHandBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnSecondHandBrushChanged(e);
        }

        /// <summary>
        /// Raises SecondHandMouseOverBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnSecondHandMouseOverBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SecondHandMouseOverBrushChanged != null)
            {
                SecondHandMouseOverBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnSecondHandMouseOverBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSecondHandMouseOverBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;           
            instance.OnSecondHandMouseOverBrushChanged(e);
        }

        /// <summary>
        /// Raises MinuteHandBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnMinuteHandBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (MinuteHandBrushChanged != null)
            {
                MinuteHandBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnMinuteHandBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMinuteHandBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnMinuteHandBrushChanged(e);
        }

        /// <summary>
        /// Raises MinuteHandBorderBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnMinuteHandBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (MinuteHandBorderBrushChanged != null)
            {
                MinuteHandBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnMinuteHandBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMinuteHandBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnMinuteHandBorderBrushChanged(e);
        }

        /// <summary>
        /// Raises MinuteHandMouseOverBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnMinuteHandMouseOverBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (MinuteHandMouseOverBrushChanged != null)
            {
                MinuteHandMouseOverBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnMinuteHandMouseOverBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMinuteHandMouseOverBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnMinuteHandMouseOverBrushChanged(e);
        }

        /// <summary>
        /// Raises MinuteHandMouseOverBorderBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnMinuteHandMouseOverBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (MinuteHandMouseOverBorderBrushChanged != null)
            {
                MinuteHandMouseOverBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnMinuteHandMouseOverBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMinuteHandMouseOverBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnMinuteHandMouseOverBorderBrushChanged(e);
        }

        /// <summary>
        /// Raises HourHandBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnHourHandBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HourHandBrushChanged != null)
            {
                HourHandBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnHourHandBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnHourHandBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnHourHandBrushChanged(e);
        }

        /// <summary>
        /// Raises HourHandBorderBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnHourHandBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HourHandBorderBrushChanged != null)
            {
                HourHandBorderBrushChanged(this, e);          
            }
        }

        /// <summary>
        /// Calls OnHourHandBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnHourHandBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnHourHandBorderBrushChanged(e);
        }

        /// <summary>
        /// Raises HourHandMouseOverBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnHourHandMouseOverBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HourHandMouseOverBrushChanged != null)
            {
                HourHandMouseOverBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnHourHandMouseOverBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnHourHandMouseOverBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnHourHandMouseOverBrushChanged(e);
        }

        /// <summary>
        /// Raises HourHandMouseOverBorderBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnHourHandMouseOverBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HourHandMouseOverBorderBrushChanged != null)
            {
                HourHandMouseOverBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnHourHandMouseOverBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnHourHandMouseOverBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnHourHandMouseOverBorderBrushChanged(e);
        }

        /// <summary>
        /// Raises HourHandPressedBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnHourHandPressedBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HourHandPressedBrushChanged != null)
            {
                HourHandPressedBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnHourHandPressedBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnHourHandPressedBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnHourHandPressedBrushChanged(e);
        }

        /// <summary>
        /// Raises MinuteHandPressedBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnMinuteHandPressedBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (MinuteHandPressedBrushChanged != null)
            {
                MinuteHandPressedBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnMinuteHandPressedBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMinuteHandPressedBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnMinuteHandPressedBrushChanged(e);
        }

        /// <summary>
        /// Raises SecondHandPressedBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnSecondHandPressedBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SecondHandPressedBrushChanged != null)
            {
                SecondHandPressedBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnSecondHandPressedBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSecondHandPressedBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnSecondHandPressedBrushChanged(e);
        }

        /// <summary>
        /// Raises IsInsideAmPmVisibleChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnIsInsideAmPmVisibleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsInsideAmPmVisibleChanged != null)
            {
                IsInsideAmPmVisibleChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnIsInsideAmPmVisibleChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsInsideAmPmVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnIsInsideAmPmVisibleChanged(e);
        }

        /// <summary>
        /// Raises IsDigitalAmPmVisibleChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnIsDigitalAmPmVisibleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsDigitalAmPmVisibleChanged != null)
            {
                IsDigitalAmPmVisibleChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnIsDigitalAmPmVisibleChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsDigitalAmPmVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;
            instance.OnIsDigitalAmPmVisibleChanged(e);
        }
        #endregion

        #region Dependency Properties
        /// <summary>
        /// Identifies DateTime dependency property.
        /// </summary>
        public static DependencyProperty DateTimeProperty = DependencyProperty.Register(
               "DateTime",
               typeof(DateTime),
               typeof(Clock),
               new PropertyMetadata(DateTime.Now, new PropertyChangedCallback(OnDateTimeChanged)));

        /// <summary>
        /// This property defines ClockCornerRadius of the Clock.
        /// </summary>
        public static readonly DependencyProperty ClockCornerRadiusProperty =
            DependencyProperty.Register(
            "ClockCornerRadius",
            typeof(CornerRadius),
            typeof(Clock),
            new FrameworkPropertyMetadata(new CornerRadius(90), new PropertyChangedCallback(OnClockCornerRadiusChanged)));

        /// <summary>
        /// This property defines BorderThickness of the Clock.
        /// </summary>
        public new static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register(
            "BorderThickness",
            typeof(Thickness),
            typeof(Clock),
            new FrameworkPropertyMetadata(new Thickness(2), new PropertyChangedCallback(OnBorderThicknessChanged)));

        /// <summary>
        /// This property defines SecondHandThickness of the Clock second hand.
        /// </summary>
        public static readonly DependencyProperty SecondHandThicknessProperty =
            DependencyProperty.Register(
            "SecondHandThickness",
            typeof(double),
            typeof(Clock),
            new FrameworkPropertyMetadata(2d, new PropertyChangedCallback(OnSecondHandThicknessChanged)));

        /// <summary>
        /// This property defines InnerBorderThickness of the Clock.
        /// </summary>
        public static readonly DependencyProperty InnerBorderThicknessProperty =
            DependencyProperty.Register(
            "InnerBorderThickness",
            typeof(Thickness),
            typeof(Clock),
            new FrameworkPropertyMetadata(new Thickness(2), new PropertyChangedCallback(OnInnerBorderThicknessChanged)));

        /// <summary>
        /// This property defines DialBorderThickness of the Clock.
        /// </summary>
        public static readonly DependencyProperty DialBorderThicknessProperty =
            DependencyProperty.Register(
            "DialBorderThickness", 
            typeof(Thickness),
            typeof(Clock),
            new FrameworkPropertyMetadata(new Thickness(25), new PropertyChangedCallback(OnDialBorderThicknessChanged)));

        /// <summary>
        /// This property defines AMPMSelectorPosition of the Clock.
        /// </summary>
        public static readonly DependencyProperty AMPMSelectorPositionProperty =
            DependencyProperty.Register(
            "AMPMSelectorPosition",
            typeof(Position),
            typeof(Clock),
            new FrameworkPropertyMetadata(Position.Bottom, new PropertyChangedCallback(OnAMPMSelectorPositionChanged)));

        /// <summary>
        /// This property defines Brush of InnerBorder.
        /// </summary>
        public static readonly DependencyProperty InnerBorderBrushProperty =
            DependencyProperty.Register(
            "InnerBorderBrush",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnInnerBorderBrushChanged)));

        /// <summary>
        /// This property defines Brush of DialBorder.
        /// </summary>
        public static readonly DependencyProperty DialBackgroundProperty =
            DependencyProperty.Register(
            "DialBackground",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnDialBackgroundChanged)));

        /// <summary>
        /// This property defines Brush of the center DialBorder.
        /// </summary>
        public static readonly DependencyProperty DialCenterBackgroundProperty =
            DependencyProperty.Register(
            "DialCenterBackground",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnDialCenterBackgroundChanged)));

        /// <summary>
        /// This property defines BorderBrush of the Clock.
        /// </summary>
        public new static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register(
            "BorderBrush",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnBorderBrushChanged)));

        /// <summary>
        /// This property defines FrameBrush of the Clock.
        /// </summary>
        public static readonly DependencyProperty ClockFrameBrushProperty =
            DependencyProperty.Register(
            "ClockFrameBrush",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnClockFrameBrushChanged)));

        /// <summary>
        /// This property defines FrameBorderThickness of the Clock.
        /// </summary>
        public static readonly DependencyProperty FrameBorderThicknessProperty =
            DependencyProperty.Register(
            "FrameBorderThickness",
            typeof(Thickness),
            typeof(Clock),
            new FrameworkPropertyMetadata(new Thickness(1), new PropertyChangedCallback(OnFrameBorderThicknessChanged)));

        /// <summary>
        /// This property defines FrameInnerBorderThickness of the Clock.
        /// </summary>
        public static readonly DependencyProperty FrameInnerBorderThicknessProperty =
            DependencyProperty.Register(
            "FrameInnerBorderThickness",
            typeof(Thickness),
            typeof(Clock),
            new FrameworkPropertyMetadata(new Thickness(1, 1, 1, 0), new PropertyChangedCallback(OnFrameInnerBorderThicknessChanged)));

        /// <summary>
        /// This property defines FrameBorderBrush of the Clock.
        /// </summary>
        public static readonly DependencyProperty FrameBorderBrushProperty =
            DependencyProperty.Register(
            "FrameBorderBrush",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnFrameBorderBrushChanged)));

        /// <summary>
        /// This property defines FrameInnerBorderBrush of the Clock.
        /// </summary>
        public static readonly DependencyProperty FrameInnerBorderBrushProperty =
            DependencyProperty.Register(
            "FrameInnerBorderBrush",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnFrameInnerBorderBrushChanged)));

        /// <summary>
        /// This property defines FrameBackground of the Clock.
        /// </summary>
        public static readonly DependencyProperty FrameBackgroundProperty =
            DependencyProperty.Register(
            "FrameBackground",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnFrameBackgroundChanged)));

        /// <summary>
        /// This property defines FrameCornerRadius of the Clock.
        /// </summary>
        public static readonly DependencyProperty FrameCornerRadiusProperty =
            DependencyProperty.Register(
            "FrameCornerRadius",
            typeof(CornerRadius),
            typeof(Clock),
            new FrameworkPropertyMetadata(new CornerRadius(0), new PropertyChangedCallback(OnFrameCornerRadiusChanged)));

        /// <summary>
        /// This property defines AMPMSelectorBorderThickness of the Clock.
        /// </summary>
        public static readonly DependencyProperty AMPMSelectorBorderThicknessProperty =
            DependencyProperty.Register(
            "AMPMSelectorBorderThickness",
            typeof(Thickness),
            typeof(Clock),
            new FrameworkPropertyMetadata(new Thickness(1), new PropertyChangedCallback(OnAMPMSelectorBorderThicknessChanged)));

        /// <summary>
        /// This property defines AMPMSelectorCornerRadius of the Clock.
        /// </summary>
        public static readonly DependencyProperty AMPMSelectorCornerRadiusProperty =
            DependencyProperty.Register(
            "AMPMSelectorCornerRadius",
            typeof(CornerRadius),
            typeof(Clock),
            new FrameworkPropertyMetadata(new CornerRadius(0), new PropertyChangedCallback(OnAMPMSelectorCornerRadiusChanged)));

        /// <summary>
        /// This property defines AMPMSelectorBorderBrush of the Clock.
        /// </summary>
        public static readonly DependencyProperty AMPMSelectorBorderBrushProperty =
            DependencyProperty.Register(
            "AMPMSelectorBorderBrush",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnAMPMSelectorBorderBrushChanged)));

        /// <summary>
        /// This property defines AMPMSelectorBackground of the Clock.
        /// </summary>
        public static readonly DependencyProperty AMPMSelectorBackgroundProperty =
            DependencyProperty.Register(
            "AMPMSelectorBackground",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnAMPMSelectorBackgroundChanged)));

        /// <summary>
        /// This property defines AMPMSelectorForeground of the Clock.
        /// </summary>
        public static readonly DependencyProperty AMPMSelectorForegroundProperty =
            DependencyProperty.Register(
            "AMPMSelectorForeground",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnAMPMSelectorForegroundChanged)));

        /// <summary>
        /// This property defines AMPMSelectorButtonsArrowBrush of the Clock.
        /// </summary>
        public static readonly DependencyProperty AMPMSelectorButtonsArrowBrushProperty =
            DependencyProperty.Register(
            "AMPMSelectorButtonsArrowBrush",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnAMPMSelectorButtonsArrowBrushChanged)));

        /// <summary>
        /// This property defines AMPMSelectorButtonsBackground of the Clock.
        /// </summary>
        public static readonly DependencyProperty AMPMSelectorButtonsBackgroundProperty =
            DependencyProperty.Register(
            "AMPMSelectorButtonsBackground",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnAMPMSelectorButtonsBackgroundChanged)));

        /// <summary>
        /// This property defines AMPMSelectorButtonsBorderBrush of the Clock.
        /// </summary>
        public static readonly DependencyProperty AMPMSelectorButtonsBorderBrushProperty =
            DependencyProperty.Register(
            "AMPMSelectorButtonsBorderBrush",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnAMPMSelectorButtonsBorderBrushChanged)));

        /// <summary>
        /// This property defines AMPMMouseOverButtonsBorderBrush of the Clock.
        /// </summary>
        public static readonly DependencyProperty AMPMMouseOverButtonsBorderBrushProperty =
            DependencyProperty.Register(
            "AMPMMouseOverButtonsBorderBrush",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnAMPMMouseOverButtonsBorderBrushChanged)));

        /// <summary>
        /// This property defines AMPMSelectorButtonsArrowBrush of the Clock.
        /// </summary>
        public static readonly DependencyProperty AMPMMouseOverButtonsArrowBrushProperty =
            DependencyProperty.Register(
            "AMPMMouseOverButtonsArrowBrush",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnAMPMMouseOverButtonsArrowBrushChanged)));

        /// <summary>
        /// This property defines AMPMMouseOverButtonsBackground of the Clock.
        /// </summary>
        public static readonly DependencyProperty AMPMMouseOverButtonsBackgroundProperty =
            DependencyProperty.Register(
            "AMPMMouseOverButtonsBackground",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnAMPMMouseOverButtonsBackgroundChanged)));

        /// <summary>
        /// This property defines ClockPointBrush of the Clock.
        /// </summary>
        public static readonly DependencyProperty ClockPointBrushProperty =
            DependencyProperty.Register(
            "ClockPointBrush",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnClockPointBrushChanged)));

        /// <summary>
        /// This property defines CenterCircleBrush of the Clock.
        /// </summary>
        public static readonly DependencyProperty CenterCircleBrushProperty =
            DependencyProperty.Register(
            "CenterCircleBrush",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnCenterCircleBrushChanged)));

        /// <summary>
        /// This property defines SecondHandBrush of the Clock.
        /// </summary>
        public static readonly DependencyProperty SecondHandBrushProperty =
            DependencyProperty.Register(
            "SecondHandBrush",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnSecondHandBrushChanged)));

        /// <summary>
        /// This property defines SecondHandMouseOverBrush of the Clock.
        /// </summary>
        public static readonly DependencyProperty SecondHandMouseOverBrushProperty =
            DependencyProperty.Register(
            "SecondHandMouseOverBrush",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnSecondHandMouseOverBrushChanged)));

        /// <summary>
        /// This property defines MinuteHandBrush of the Clock.
        /// </summary>
        public static readonly DependencyProperty MinuteHandBrushProperty =
            DependencyProperty.Register(
            "MinuteHandBrush",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnMinuteHandBrushChanged)));

        /// <summary>
        /// This property defines MinuteHandBorderBrush of the Clock.
        /// </summary>
        public static readonly DependencyProperty MinuteHandBorderBrushProperty =
            DependencyProperty.Register(
            "MinuteHandBorderBrush",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnMinuteHandBorderBrushChanged)));

        /// <summary>
        /// This property defines MinuteHandMouseOverBrush of the Clock.
        /// </summary>
        public static readonly DependencyProperty MinuteHandMouseOverBrushProperty =
            DependencyProperty.Register(
            "MinuteHandMouseOverBrush",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnMinuteHandMouseOverBrushChanged)));

        /// <summary>
        /// This property defines MinuteHandMouseOverBorderBrush of the Clock.
        /// </summary>
        public static readonly DependencyProperty MinuteHandMouseOverBorderBrushProperty =
            DependencyProperty.Register(
            "MinuteHandMouseOverBorderBrush",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnMinuteHandMouseOverBorderBrushChanged)));

        /// <summary>
        /// This property defines HourHandBrush of the Clock.
        /// </summary>
        public static readonly DependencyProperty HourHandBrushProperty =
            DependencyProperty.Register(
            "HourHandBrush",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnHourHandBrushChanged)));

        /// <summary>
        /// This property defines HourHandShadowBrush of the Clock.
        /// </summary>
        public static readonly DependencyProperty HourHandBorderBrushProperty =
            DependencyProperty.Register(
            "HourHandBorderBrush",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnHourHandBorderBrushChanged)));

        /// <summary>
        /// This property defines HourHandMouseOverBrush of the Clock.
        /// </summary>
        public static readonly DependencyProperty HourHandMouseOverBrushProperty =
            DependencyProperty.Register(
            "HourHandMouseOverBrush",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnHourHandMouseOverBrushChanged)));

        /// <summary>
        /// This property defines HourHandMouseOverBorderBrush of the Clock.
        /// </summary>
        public static readonly DependencyProperty HourHandMouseOverBorderBrushProperty =
            DependencyProperty.Register(
            "HourHandMouseOverBorderBrush",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnHourHandMouseOverBorderBrushChanged)));

        /// <summary>
        /// This property defines HourHandPressedBrush of the Clock.
        /// </summary>
        public static readonly DependencyProperty HourHandPressedBrushProperty =
            DependencyProperty.Register(
            "HourHandPressedBrush",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnHourHandPressedBrushChanged)));

        /// <summary>
        /// This property defines MinuteHandPressedBrush of the Clock.
        /// </summary>
        public static readonly DependencyProperty MinuteHandPressedBrushProperty =
            DependencyProperty.Register(
            "MinuteHandPressedBrush",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnMinuteHandPressedBrushChanged)));

        /// <summary>
        /// This property defines SecondHandPressedBrush of the Clock.
        /// </summary>
        public static readonly DependencyProperty SecondHandPressedBrushProperty =
            DependencyProperty.Register(
            "SecondHandPressedBrush",
            typeof(Brush),
            typeof(Clock),
            new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnSecondHandPressedBrushChanged)));

        /// <summary>
        /// This property defines IsInsideAmPmVisible of the Clock.
        /// </summary>
        public static readonly DependencyProperty IsInsideAmPmVisibleProperty =
            DependencyProperty.Register(
            "IsInsideAmPmVisible",
            typeof(bool),
            typeof(Clock),
           new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsInsideAmPmVisibleChanged)));

        /// <summary>
        /// This property defines IsDigitalAmPmVisible of the Clock.
        /// </summary>
        public static readonly DependencyProperty IsDigitalAmPmVisibleProperty =
            DependencyProperty.Register(
            "IsDigitalAmPmVisible",
            typeof(bool),
            typeof(Clock),
           new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsDigitalAmPmVisibleChanged)));
        #endregion
    }
}
