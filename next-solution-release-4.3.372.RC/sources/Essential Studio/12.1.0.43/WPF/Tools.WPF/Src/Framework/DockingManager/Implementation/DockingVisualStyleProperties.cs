#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections;


namespace Syncfusion.Windows.Tools.Controls
{
    public partial class DockingManager
    {
        #region Events
        /// <summary>
        /// Event that is raised when FloatWindowHeaderBackground property is changed.
        /// </summary>
        public event PropertyChangedCallback FloatWindowHeaderBackgroundChanged;
        /// <summary>
        /// Event that is raised when FloatWindowBorderBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback FloatWindowBorderBrushChanged;
        /// <summary>
        /// Event that is raised when FloatWindowSelectedBorderBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback FloatWindowSelectedBorderBrushChanged;
        /// <summary>
        /// Event that is raised when FloatWindowMouseOverBorderBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback FloatWindowMouseOverBorderBrushChanged;
        /// <summary>
        /// Event that is raised when FloatWindowBorderThickness property is changed.
        /// </summary>
        public event PropertyChangedCallback FloatWindowBorderThicknessChanged;
        /// <summary>
        /// Event that is raised when FloatWindowSelectedHeaderBackground property is changed.
        /// </summary>
        public event PropertyChangedCallback FloatWindowSelectedHeaderBackgroundChanged;
        /// <summary>
        /// Event that is raised when FloatWindowMouseOverHeaderBackground property is changed.
        /// </summary>
        public event PropertyChangedCallback FloatWindowMouseOverHeaderBackgroundChanged;
        /// <summary>
        /// Event that is raised when FloatWindowHeaderForeground property is changed.
        /// </summary>
        public event PropertyChangedCallback FloatWindowHeaderForegroundChanged;
        /// <summary>
        /// Event that is raised when FloatWindowSelectedHeaderForeground property is changed.
        /// </summary>
        public event PropertyChangedCallback FloatWindowSelectedHeaderForegroundChanged;
        /// <summary>
        /// Event that is raised when FloatWindowMouseOverHeaderForeground property is changed.
        /// </summary>
        public event PropertyChangedCallback FloatWindowMouseOverHeaderForegroundChanged;
        /// <summary>
        /// Event that is raised when SplitterBackground property is changed.
        /// </summary>
        public event PropertyChangedCallback SplitterBackgroundChanged;
        /// <summary>
        /// Event that is raised when SplitterSize property is changed.
        /// </summary>
        public event PropertyChangedCallback SplitterSizeChanged;
        /// <summary>
        /// Event that is raised when TabItemsCornerRadius property is changed.
        /// </summary>
        public event PropertyChangedCallback TabItemsCornerRadiusChanged;
        /// <summary>
        /// Event that is raised when SidePanelBackground property is changed.
        /// </summary>
        public event PropertyChangedCallback SidePanelBackgroundChanged;
        /// <summary>
        /// Event that is raised when TabPanelBackground property is changed.
        /// </summary>
        public event PropertyChangedCallback TabPanelBackgroundChanged;
        /// <summary>
        /// Event that is raised when TabPanelBorderBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback TabPanelBorderBrushChanged;
        /// <summary>
        /// Event that is raised when SidePanelBorderBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback SidePanelBorderBrushChanged;
        /// <summary>
        /// Event that is raised when SidePanelBorderThickness property is changed.
        /// </summary>
        public event PropertyChangedCallback SidePanelBorderThicknessChanged;
        /// <summary>
        /// Event that is raised when TabPanelBorderThickness property is changed.
        /// </summary>
        public event PropertyChangedCallback TabPanelBorderThicknessChanged;
        /// <summary>
        /// Event that is raised when TabItemBorderThickness property is changed.
        /// </summary>
        public event PropertyChangedCallback TabItemBorderThicknessChanged;
        /// <summary>
        /// Event that is raised when TabItemsBorderThicknessSelected property is changed.
        /// </summary>
        public event PropertyChangedCallback TabItemsBorderThicknessSelectedChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the value of the FloatWindowHeaderBackground dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides FloatWindowHeaderBackground value for the <see cref="DockingManager"/>. The default value of the FloatWindowHeaderBackground property is Transparent.
        /// </value>
        /// <remarks>
        /// FloatWindowHeaderBackground dependency property defines header background of the <see cref="FloatWindow"/>. 
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set FloatWindowHeaderBackground property in C#.
        /// <code language="C#">
        ///     dockingManager.FloatWindowHeaderBackground = Brushes.Black;
        /// </code>
        /// <para/>This example shows how to set FloatWindowHeaderBackground property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        ///   <Syncfusion:DockingManager FloatWindowHeaderBackground="Black" />
        /// ]]>
        /// </code>
        /// </example>
        public Brush FloatWindowHeaderBackground
        {
            get
            {
                return ( Brush )GetValue( FloatWindowHeaderBackgroundProperty );
            }
            set
            {
                SetValue( FloatWindowHeaderBackgroundProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the FloatWindowSelectedHeaderBackground dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides FloatWindowSelectedHeaderBackground value for the <see cref="DockingManager"/>. The default value of the FloatWindowSelectedHeaderBackground property is Transparent.
        /// </value>
        /// <remarks>
        /// FloatWindowSelectedHeaderBackground dependency property defines selected header background of the <see cref="FloatWindow"/>. 
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set FloatWindowSelectedHeaderBackground property in C#.
        /// <code language="C#">
        ///     dockingManager.FloatWindowSelectedHeaderBackground = Brushes.Black;
        /// </code>
        /// <para/>This example shows how to set FloatWindowSelectedHeaderBackground property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        ///   <Syncfusion:DockingManager FloatWindowSelectedHeaderBackground="#000000" />
        /// ]]>
        /// </code>
        /// </example>
        public Brush FloatWindowSelectedHeaderBackground
        {
            get
            {
                return ( Brush )GetValue( FloatWindowSelectedHeaderBackgroundProperty );
            }
            set
            {
                SetValue( FloatWindowSelectedHeaderBackgroundProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the FloatWindowMouseOverHeaderBackground dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides FloatWindowMouseOverHeaderBackground value for the <see cref="DockingManager"/>. The default value of the FloatWindowMouseOverHeaderBackground property is Transparent.
        /// </value>
        /// <remarks>
        /// FloatWindowMouseOverHeaderBackground dependency property defines header background of the <see cref="FloatWindow"/> when mouse is over the header. 
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set FloatWindowMouseOverHeaderBackground property in C#.
        /// <code language="C#">
        ///     dockingManager.FloatWindowMouseOverHeaderBackground = Brushes.Black;
        /// </code>
        /// <para/>This example shows how to set FloatWindowMouseOverHeaderBackground property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        ///   <Syncfusion:DockingManager FloatWindowMouseOverHeaderBackground="Black" />
        /// ]]>
        /// </code>
        /// </example>
        public Brush FloatWindowMouseOverHeaderBackground
        {
            get
            {
                return ( Brush )GetValue( FloatWindowMouseOverHeaderBackgroundProperty );
            }
            set
            {
                SetValue( FloatWindowMouseOverHeaderBackgroundProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the FloatWindowHeaderForeground dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides FloatWindowHeaderForeground value for the <see cref="DockingManager"/>. The default value of the FloatWindowHeaderForeground property is Transparent.
        /// </value>
        /// <remarks>
        /// FloatWindowHeaderForeground dependency property defines header foreground of the <see cref="FloatWindow"/>. 
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set FloatWindowHeaderForeground property in C#.
        /// <code language="C#">
        ///     dockingManager.FloatWindowHeaderForeground = Brushes.Black;
        /// </code>
        /// <para/>This example shows how to set FloatWindowHeaderForeground property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        ///   <Syncfusion:DockingManager FloatWindowHeaderForeground="Black" />
        /// ]]>
        /// </code>
        /// </example>
        public Brush FloatWindowHeaderForeground
        {
            get
            {
                return ( Brush )GetValue( FloatWindowHeaderForegroundProperty );
            }
            set
            {
                SetValue( FloatWindowHeaderForegroundProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the FloatWindowSelectedHeaderForeground dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides FloatWindowSelectedHeaderForeground value for the <see cref="DockingManager"/>. The default value of the FloatWindowSelectedHeaderForeground property is Transparent.
        /// </value>
        /// <remarks>
        /// FloatWindowSelectedHeaderForeground dependency property defines selected header foreground of the <see cref="FloatWindow"/>. 
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set FloatWindowSelectedHeaderForeground property in C#.
        /// <code language="C#">
        ///     dockingManager.FloatWindowSelectedHeaderForeground = Brushes.Black;
        /// </code>
        /// <para/>This example shows how to set FloatWindowSelectedHeaderForeground property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        ///   <Syncfusion:DockingManager FloatWindowSelectedHeaderForeground="Black" />
        /// ]]>
        /// </code>
        /// </example>
        public Brush FloatWindowSelectedHeaderForeground
        {
            get
            {
                return ( Brush )GetValue( FloatWindowSelectedHeaderForegroundProperty );
            }
            set
            {
                SetValue( FloatWindowSelectedHeaderForegroundProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the FloatWindowMouseOverHeaderForeground dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides FloatWindowMouseOverHeaderForeground value for the <see cref="DockingManager"/>. The default value of the FloatWindowMouseOverHeaderForeground property is Transparent.
        /// </value>
        /// <remarks>
        /// FloatWindowMouseOverHeaderForeground dependency property defines header foreground of the <see cref="FloatWindow"/> when mouse is over the header. 
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set FloatWindowMouseOverHeaderForeground property in C#.
        /// <code language="C#">
        ///     dockingManager.FloatWindowMouseOverHeaderForeground = Brushes.Black;
        /// </code>
        /// <para/>This example shows how to set FloatWindowMouseOverHeaderForeground property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        ///   <Syncfusion:DockingManager FloatWindowMouseOverHeaderForeground="Black" />
        /// ]]>
        /// </code>
        /// </example>
        public Brush FloatWindowMouseOverHeaderForeground
        {
            get
            {
                return ( Brush )GetValue( FloatWindowMouseOverHeaderForegroundProperty );
            }
            set
            {
                SetValue( FloatWindowMouseOverHeaderForegroundProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the FloatWindowBorderBrush dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides FloatWindowBorderBrush value for the <see cref="DockingManager"/>. The default value of the FloatWindowBorderBrush property is Transparent.
        /// </value>
        /// <remarks>
        /// FloatWindowBorderBrush dependency property defines border brush of the <see cref="FloatWindow"/>. 
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set FloatWindowBorderBrush property in C#.
        /// <code language="C#">
        ///     dockingManager.FloatWindowBorderBrush = Brushes.Black;
        /// </code>
        /// <para/>This example shows how to set FloatWindowBorderBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        ///   <Syncfusion:DockingManager FloatWindowBorderBrush="Black" />
        /// ]]>
        /// </code>
        /// </example>
        public Brush FloatWindowBorderBrush
        {
            get
            {
                return ( Brush )GetValue( FloatWindowBorderBrushProperty );
            }
            set
            {
                SetValue( FloatWindowBorderBrushProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the FloatWindowSelectedBorderBrush dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides FloatWindowSelectedBorderBrush value for the <see cref="DockingManager"/>. The default value of the FloatWindowSelectedBorderBrush property is Transparent.
        /// </value>
        /// <remarks>
        /// FloatWindowSelectedBorderBrush dependency property defines selected border brush of the <see cref="FloatWindow"/>. 
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set FloatWindowSelectedBorderBrush property in C#.
        /// <code language="C#">
        ///     dockingManager.FloatWindowSelectedBorderBrush = Brushes.Black;
        /// </code>
        /// <para/>This example shows how to set FloatWindowSelectedBorderBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        ///   <Syncfusion:DockingManager FloatWindowSelectedBorderBrush="Black" />
        /// ]]>
        /// </code>
        /// </example>
        /// 
       
        public Brush FloatWindowSelectedBorderBrush
        {
            get
            {
                return ( Brush )GetValue( FloatWindowSelectedBorderBrushProperty );
            }
            set
            {
                SetValue( FloatWindowSelectedBorderBrushProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the FloatWindowMouseOverBorderBrush dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides FloatWindowMouseOverBorderBrush value for the <see cref="DockingManager"/>. The default value of the FloatWindowMouseOverBorderBrush property is Transparent.
        /// </value>
        /// <remarks>
        /// FloatWindowMouseOverBorderBrush dependency property defines border brush of the <see cref="FloatWindow"/> when mouse is over the border. 
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set FloatWindowMouseOverBorderBrush property in C#.
        /// <code language="C#">
        ///     dockingManager.FloatWindowMouseOverBorderBrush = Brushes.Black;
        /// </code>
        /// <para/>This example shows how to set FloatWindowMouseOverBorderBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        ///   <Syncfusion:DockingManager FloatWindowMouseOverBorderBrush="Black" />
        /// ]]>
        /// </code>
        /// </example>
        public Brush FloatWindowMouseOverBorderBrush
        {
            get
            {
                return ( Brush )GetValue( FloatWindowMouseOverBorderBrushProperty );
            }
            set
            {
                SetValue( FloatWindowMouseOverBorderBrushProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the LeftFloatWindowTopBorderBrush dependency property.
        /// </summary>
        [Browsable(false)]
        public Brush LeftFloatWindowTopBorderBrush
        {
            get
            {
                return ( Brush )GetValue( LeftFloatWindowTopBorderBrushProperty );
            }
            set
            {
                SetValue( LeftFloatWindowTopBorderBrushProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the LeftFloatWindowSelectedTopBorderBrush dependency property.
        /// </summary>
        [Browsable(false)]
        public Brush LeftFloatWindowSelectedTopBorderBrush
        {
            get
            {
                return ( Brush )GetValue( LeftFloatWindowSelectedTopBorderBrushProperty );
            }
            set
            {
                SetValue( LeftFloatWindowSelectedTopBorderBrushProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the LeftFloatMouseOverTopBorderBrush dependency property.
        /// </summary>
        [Browsable(false)]        
        public Brush LeftFloatMouseOverTopBorderBrush
        {
            get
            {
                return ( Brush )GetValue( LeftFloatMouseOverTopBorderBrushProperty );
            }
            set
            {
                SetValue( LeftFloatMouseOverTopBorderBrushProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the LeftFloatWindowBottomBorderBrush dependency property.
        /// </summary>
        [Browsable(false)]
        public Brush LeftFloatWindowBottomBorderBrush
        {
            get
            {
                return ( Brush )GetValue( LeftFloatWindowBottomBorderBrushProperty );
            }
            set
            {
                SetValue( LeftFloatWindowBottomBorderBrushProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the LeftFloatWindowSelectedBottomBorderBrush dependency property.
        /// </summary>
        [Browsable(false)]
        public Brush LeftFloatWindowSelectedBottomBorderBrush
        {
            get
            {
                return ( Brush )GetValue( LeftFloatWindowSelectedBottomBorderBrushProperty );
            }
            set
            {
                SetValue( LeftFloatWindowSelectedBottomBorderBrushProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the LeftFloatMouseOverBottomBorderBrush dependency property.
        /// </summary>
        [Browsable(false)]
        public Brush LeftFloatMouseOverBottomBorderBrush
        {
            get
            {
                return ( Brush )GetValue( LeftFloatMouseOverBottomBorderBrushProperty );
            }
            set
            {
                SetValue( LeftFloatMouseOverBottomBorderBrushProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the LeftFloatWindowBorderBrush dependency property.
        /// </summary>
        [Browsable(false)]
        public Brush LeftFloatWindowBorderBrush
        {
            get
            {
                return ( Brush )GetValue( LeftFloatWindowBorderBrushProperty );
            }
            set
            {
                SetValue( LeftFloatWindowBorderBrushProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the LeftFloatWindowSelectedBorderBrush dependency property.
        /// </summary>
        [Browsable(false)]
        public Brush LeftFloatWindowSelectedBorderBrush
        {
            get
            {
                return ( Brush )GetValue( LeftFloatWindowSelectedBorderBrushProperty );
            }
            set
            {
                SetValue( LeftFloatWindowSelectedBorderBrushProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the LeftFloatMouseOverBorderBrush dependency property.
        /// </summary>
        [Browsable(false)]
        public Brush LeftFloatMouseOverBorderBrush
        {
            get
            {
                return ( Brush )GetValue( LeftFloatMouseOverBorderBrushProperty );
            }
            set
            {
                SetValue( LeftFloatMouseOverBorderBrushProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the RightFloatWindowTopBorderBrush dependency property.
        /// </summary>
        [Browsable(false)]
        public Brush RightFloatWindowTopBorderBrush
        {
            get
            {
                return ( Brush )GetValue( RightFloatWindowTopBorderBrushProperty );
            }
            set
            {
                SetValue( RightFloatWindowTopBorderBrushProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the RightFloatWindowSelectedTopBorderBrush dependency property.
        /// </summary>
        [Browsable(false)]
        public Brush RightFloatWindowSelectedTopBorderBrush
        {
            get
            {
                return ( Brush )GetValue( RightFloatWindowSelectedTopBorderBrushProperty );
            }
            set
            {
                SetValue( RightFloatWindowSelectedTopBorderBrushProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the RightFloatMouseOverTopBorderBrush dependency property.
        /// </summary>
        [Browsable(false)]
        public Brush RightFloatMouseOverTopBorderBrush
        {
            get
            {
                return ( Brush )GetValue( RightFloatMouseOverTopBorderBrushProperty );
            }
            set
            {
                SetValue( RightFloatMouseOverTopBorderBrushProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the RightFloatWindowBottomBorderBrush dependency property.
        /// </summary>
        [Browsable(false)]
        public Brush RightFloatWindowBottomBorderBrush
        {
            get
            {
                return ( Brush )GetValue( RightFloatWindowBottomBorderBrushProperty );
            }
            set
            {
                SetValue( RightFloatWindowBottomBorderBrushProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the RightFloatWindowSelectedBottomBorderBrush dependency property.
        /// </summary>
        [Browsable(false)]
        public Brush RightFloatWindowSelectedBottomBorderBrush
        {
            get
            {
                return ( Brush )GetValue( RightFloatWindowSelectedBottomBorderBrushProperty );
            }
            set
            {
                SetValue( RightFloatWindowSelectedBottomBorderBrushProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the RightFloatMouseOverBottomBorderBrush dependency property.
        /// </summary>
        [Browsable(false)]
        public Brush RightFloatMouseOverBottomBorderBrush
        {
            get
            {
                return ( Brush )GetValue( RightFloatMouseOverBottomBorderBrushProperty );
            }
            set
            {
                SetValue( RightFloatMouseOverBottomBorderBrushProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the RightFloatWindowBorderBrush dependency property.
        /// </summary>
        [Browsable(false)]
        public Brush RightFloatWindowBorderBrush
        {
            get
            {
                return ( Brush )GetValue( RightFloatWindowBorderBrushProperty );
            }
            set
            {
                SetValue( RightFloatWindowBorderBrushProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the RightFloatWindowSelectedBorderBrush dependency property.
        /// </summary>
        [Browsable(false)]
        public Brush RightFloatWindowSelectedBorderBrush
        {
            get
            {
                return ( Brush )GetValue( RightFloatWindowSelectedBorderBrushProperty );
            }
            set
            {
                SetValue( RightFloatWindowSelectedBorderBrushProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the RightFloatMouseOverBorderBrush dependency property.
        /// </summary>
        [Browsable(false)]
        public Brush RightFloatMouseOverBorderBrush
        {
            get
            {
                return ( Brush )GetValue( RightFloatMouseOverBorderBrushProperty );
            }
            set
            {
                SetValue( RightFloatMouseOverBorderBrushProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the FloatWindowBorderThickness dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// Provides FloatWindowBorderThickness value for the <see cref="DockingManager"/>. The default value of the FloatWindowBorderThickness property is "22,4,4,4".
        /// </value>
        /// <remarks>
        /// FloatWindowBorderThickness dependency property defines border thickness of the floating window. 
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set FloatWindowBorderThickness property in C#.
        /// <code language="C#">
        ///     dockingManager.FloatWindowBorderThickness = new Thickness(20);
        /// </code>
        /// <para/>This example shows how to set FloatWindowBorderThickness property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        ///   <Syncfusion:DockingManager FloatWindowBorderThickness="20" />
        /// ]]>
        /// </code>
        /// </example>
        public Thickness FloatWindowBorderThickness
        {
            get
            {
                return ( Thickness )GetValue( FloatWindowBorderThicknessProperty );
            }
            set
            {
                SetValue( FloatWindowBorderThicknessProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the FloatWindowTopBorderHeight dependency property.
        /// </summary>
        [Browsable(false)]
        public double FloatWindowTopBorderHeight
        {
            get
            {
                return ( double )GetValue( FloatWindowTopBorderHeightProperty );
            }
            set
            {
                SetValue( FloatWindowTopBorderHeightProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the FloatWindowBottomBorderHeight dependency property.
        /// </summary>
        [Browsable(false)]
        public double FloatWindowBottomBorderHeight
        {
            get
            {
                return ( double )GetValue( FloatWindowBottomBorderHeightProperty );
            }
            set
            {
                SetValue( FloatWindowBottomBorderHeightProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the FloatWindowLeftBorderWidth dependency property.
        /// </summary>
        [Browsable(false)]
        public double FloatWindowLeftBorderWidth
        {
            get
            {
                return ( double )GetValue( FloatWindowLeftBorderWidthProperty );
            }
            set
            {
                SetValue( FloatWindowLeftBorderWidthProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the FloatWindowRightBorderWidth dependency property.
        /// </summary>
        [Browsable(false)]
        public double FloatWindowRightBorderWidth
        {
            get
            {
                return ( double )GetValue( FloatWindowRightBorderWidthProperty );
            }
            set
            {
                SetValue( FloatWindowRightBorderWidthProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets a brush that describes the background of a control. This is a  dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides Background value for the <see cref="DockingManager"/>. The default value of the Background property is Transparent.
        /// </value>
        /// <remarks>
        /// Background dependency property defines background of the <see cref="DockingManager"/> sides. 
        /// </remarks>
        public Brush Background
        {
            get
            {
                return ( Brush )GetValue( BackgroundProperty );
            }
            set
            {
                SetValue( BackgroundProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the style used by DockHeader when it is rendered. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Style"/>
        /// Provides DockHeaderStyle value for the <see cref="DockingManager"/>.
        /// </value>
        /// <example>
        /// <para/>This example shows how to initialize and set DockHeaderStyle property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window.Resources>
		///     <ResourceDictionary>
        ///         <Style x:Key="DockHeaderPresStyle" TargetType="{x:Type Syncfusion:DockHeaderPresenter}">
        ///             <Setter Property="MinHeight" Value="57"/>
        ///         </Style>
        ///     </ResourceDictionary>
	    /// </Window.Resources>
        /// 
        ///   <Syncfusion:DockingManager DockHeaderStyle="{StaticResource DockHeaderPresStyle}" />
        /// ]]>
        /// </code>
        /// </example>
        public Style DockHeaderStyle
        {
            get
            {
                return ( Style )GetValue( DockHeaderStyleProperty );
            }
            set
            {
                SetValue( DockHeaderStyleProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the style used by DockedElementHost when it is rendered. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Style"/>
        /// Provides DockedElementHostStyle value for the <see cref="DockingManager"/>.
        /// </value>
        /// <example>
        /// You can initialize the style of docked element host and set DockedElementHostStyle property in XAML like you set <see cref="DockHeaderStyle"/> property.
        /// </example>
        public Style DockedElementHostStyle
        {
            get
            {
                return ( Style )GetValue( DockedElementHostStyleProperty );
            }
            set
            {
                SetValue( DockedElementHostStyleProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the style used by FloatWindow when it is rendered. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Style"/>
        /// Provides FloatWindowStyle value for the <see cref="DockingManager"/>.
        /// </value>
        /// <example>
        /// You can initialize the style of float window and set FloatWindowStyle property in XAML like you set <see cref="DockHeaderStyle"/> property.
        /// </example>
        public Style FloatWindowStyle
        {
            get
            {
                return ( Style )GetValue( FloatWindowStyleProperty );
            }
            set
            {
                SetValue( FloatWindowStyleProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the style used by TabItem when it is rendered. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Style"/>
        /// Provides TabItemStyle value for the <see cref="DockingManager"/>.
        /// </value>
        /// <example>
        /// You can initialize the style of tab item and set TabItemStyle property in XAML like you set <see cref="DockHeaderStyle"/> property.
        /// </example>
        public Style TabItemStyle
        {
            get
            {
                return ( Style )GetValue( TabItemStyleProperty );
            }
            set
            {
                SetValue( TabItemStyleProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the style used by TabControl when it is rendered. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Style"/>
        /// Provides TabControlStyle value for the <see cref="DockingManager"/>.
        /// </value>
        /// <example>
        /// You can initialize the style of tab control and set TabControlStyle property in XAML like you set <see cref="DockHeaderStyle"/> property.
        /// </example>
        public Style TabControlStyle
        {
            get
            {
                return ( Style )GetValue( TabControlStyleProperty );
            }
            set
            {
                SetValue( TabControlStyleProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the style used by MainHost when it is rendered. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Style"/>
        /// Provides MainHostStyle value for the <see cref="DockingManager"/>.
        /// </value>
        /// <example>
        /// You can initialize the style of main host and set MainHostStyle property in XAML like you set <see cref="DockHeaderStyle"/> property.
        /// </example>
        public Style MainHostStyle
        {
            get
            {
                return ( Style )GetValue( MainHostStyleProperty );
            }
            set
            {
                SetValue( MainHostStyleProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the style used by SidePanel when it is rendered. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Style"/>
        /// Provides SidePanelStyle value for the <see cref="DockingManager"/>.
        /// </value>
        /// <example>
        /// You can initialize the style of the side panel and set SidePanelStyle property in XAML like you set <see cref="DockHeaderStyle"/> property.
        /// </example>
        public Style SidePanelStyle
        {
            get
            {
                return ( Style )GetValue( SidePanelStyleProperty );
            }
            set
            {
                SetValue( SidePanelStyleProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the style used by SideItem when it is rendered. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Style"/>
        /// Provides SideItemStyle value for the <see cref="DockingManager"/>.
        /// </value>
        /// <example>
        /// You can initialize the style of the side item and set SideItemStyle property in XAML like you set <see cref="DockHeaderStyle"/> property.
        /// </example>
        public Style SideItemStyle
        {
            get
            {
                return ( Style )GetValue( SideItemStyleProperty );
            }
            set
            {
                SetValue( SideItemStyleProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets a control template for close button. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="ControlTemplate"/>
        /// Provides CloseButtonTemplate value for the <see cref="DockingManager"/>.
        /// </value>
        /// <example>
        /// <para/>This example shows how to initialize and set CloseButtonTemplate property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window.Resources>
        ///     <ResourceDictionary>
        ///         <ControlTemplate x:Key="FlatKeyClose" TargetType="{x:Type ToggleButton}">
        ///         <StackPanel>
        ///             <Border x:Name="brdBack" Width="15" Height="15" Margin="0,0,1,1" BorderThickness="1" BorderBrush="Transparent" >
        ///                 <Path Name="pathButton" SnapsToDevicePixels="False" Stretch="Fill" StrokeThickness="2"
		///			        Stroke="Red" Data="M109,51 L216,142 M215,52 L109,142" HorizontalAlignment="Center" VerticalAlignment="Center" Width="9" Height="8"/>
        ///             </Border>
        ///         </StackPanel>
        ///         </ControlTemplate>
        ///     </ResourceDictionary>
        /// </Window.Resources>
        /// 
        ///   <Syncfusion:DockingManager CloseButtonTemplate="{StaticResource FlatKeyClose}" />
        /// ]]>
        /// </code>
        /// </example>
        public ControlTemplate CloseButtonTemplate
        {
            get
            {
                return ( ControlTemplate )GetValue( CloseButtonTemplateProperty );
            }
            set
            {
                SetValue( CloseButtonTemplateProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets a control template for menu button. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="ControlTemplate"/>
        /// Provides MenuButtonTemplate value for the <see cref="DockingManager"/>.
        /// </value>
        /// <example>
        /// You can initialize control template of the menu button and set MenuButtonTemplate property in XAML like you set <see cref="CloseButtonTemplate"/> property.
        /// </example>
        public ControlTemplate MenuButtonTemplate
        {
            get
            {
                return ( ControlTemplate )GetValue( MenuButtonTemplateProperty );
            }
            set
            {
                SetValue( MenuButtonTemplateProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets a control template for autohide button. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="ControlTemplate"/>
        /// Provides AwlButtonTemplate value for the <see cref="DockingManager"/>.
        /// </value>
        /// <example>
        /// You can initialize control template of the auto-hide button and set AwlButtonTemplate property in XAML like you set <see cref="CloseButtonTemplate"/> property.
        /// </example>
        public ControlTemplate AwlButtonTemplate
        {
            get
            {
                return ( ControlTemplate )GetValue( AwlButtonTemplateProperty );
            }
            set
            {
                SetValue( AwlButtonTemplateProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the SplitterBackground dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides SplitterBackground value for the <see cref="DockingManager"/>. The default value of the SplitterBackground property is Transparent.
        /// </value>
        /// <remarks>
        /// SplitterBackground dependency property defines background of the split line between windows in dock state. 
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set SplitterBackground property in C#.
        /// <code language="C#">
        ///     dockingManager.SplitterBackground = Brushes.Black;
        /// </code>
        /// <para/>This example shows how to set SplitterBackground property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        ///   <Syncfusion:DockingManager SplitterBackground="Black" />
        /// ]]>
        /// </code>
        /// </example>
        public Brush SplitterBackground
        {
            get
            {
                return ( Brush )GetValue( SplitterBackgroundProperty );
            }
            set
            {
                SetValue( SplitterBackgroundProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the SplitterSize dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Provides SplitterSize value for the <see cref="DockingManager"/>. The default value of the SplitterSize property is 4.
        /// </value>
        /// <remarks>
        /// SplitterSize dependency property defines the size of the split line between windows in dock state. 
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set SplitterSize property in C#.
        /// <code language="C#">
        ///     dockingManager.SplitterSize = 10d;
        /// </code>
        /// <para/>This example shows how to set SplitterSize property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        ///   <Syncfusion:DockingManager SplitterSize="10" />
        /// ]]>
        /// </code>
        /// </example>
        public double SplitterSize
        {
            get
            {
                return ( double )GetValue( SplitterSizeProperty );
            }
            set
            {
                SetValue( SplitterSizeProperty, value );
            }
        }
        /// <summary>
        /// HeaderBackground property is used to store background value for header of the dock window.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides HeaderBackground value for the <see cref="DockingManager"/>. The default value of the HeaderBackground property depends on selected theme or skin.
        /// </value>
        /// <remarks>
        /// HeaderBackground dependency property defines background of the window`s header. 
        /// </remarks>
        /// <example>
        /// To set HeaderBackground property please see <see cref="SplitterBackground"/> property example.
        /// </example>
        public Brush HeaderBackground
        {
            get
            {
                return ( Brush )GetValue( HeaderBackgroundProperty );
            }
            set
            {
                SetValue( HeaderBackgroundProperty, value );
            }
        }
        /// <summary>
        /// SelectedHeaderBackground property is used to store background value for header of the
        /// selected dock window.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides SelectedHeaderBackground value for the <see cref="DockingManager"/>. The default value of the SelectedHeaderBackground property depends on selected theme or skin.
        /// </value>
        /// <remarks>
        /// SelectedHeaderBackground dependency property defines background of the selected window`s header. 
        /// </remarks>
        /// <example>
        /// To set SelectedHeaderBackground property please see <see cref="SplitterBackground"/> property example.
        /// </example>
        public Brush SelectedHeaderBackground
        {
            get
            {
                return ( Brush )GetValue( SelectedHeaderBackgroundProperty );
            }
            set
            {
                SetValue( SelectedHeaderBackgroundProperty, value );
            }
        }
        /// <summary>
        /// HeaderMouseOverBackground property is used to store background value for header of the 
        /// mouse over dock window.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides HeaderMouseOverBackground value for the <see cref="DockingManager"/>. The default value of the HeaderMouseOverBackground property depends on selected theme or skin.
        /// </value>
        /// <remarks>
        /// HeaderMouseOverBackground dependency property defines background of the window`s header when mouse over. 
        /// </remarks>
        /// <example>
        /// To set HeaderMouseOverBackground property please see <see cref="SplitterBackground"/> property example.
        /// </example>
        public Brush HeaderMouseOverBackground
        {
            get
            {
                return ( Brush )GetValue( HeaderMouseOverBackgroundProperty );
            }
            set
            {
                SetValue( HeaderMouseOverBackgroundProperty, value );
            }
        }
        /// <summary>
        /// SideItemsBackground property is used to store background value for side panel items.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides SideItemsBackground value for the <see cref="DockingManager"/>. The default value of the SideItemsBackground property depends on selected theme or skin.
        /// </value>
        /// <remarks>
        /// SideItemsBackground dependency property defines background of the side items. 
        /// </remarks>
        /// <example>
        /// To set SideItemsBackground property please see <see cref="SplitterBackground"/> property example.
        /// </example>
        public Brush SideItemsBackground
        {
            get
            {
                return ( Brush )GetValue( SideItemsBackgroundProperty );
            }
            set
            {
                SetValue( SideItemsBackgroundProperty, value );
            }
        }
        /// <summary>
        /// TabItemsBackground property is used to store background value for tab control items
        /// of the dock window.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides TabItemsBackground value for the <see cref="DockingManager"/>. The default value of the TabItemsBackground property depends on selected theme or skin.
        /// </value>
        /// <remarks>
        /// TabItemsBackground dependency property defines background of the tab items. 
        /// </remarks>
        /// <example>
        /// To set TabItemsBackground property please see <see cref="SplitterBackground"/> property example.
        /// </example>
        public Brush TabItemsBackground
        {
            get
            {
                return ( Brush )GetValue( TabItemsBackgroundProperty );
            }
            set
            {
                SetValue( TabItemsBackgroundProperty, value );
            }
        }
        /// <summary>
        /// TabItemsForeground property is used to store background value for tab control items
        /// of the dock window.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides TabItemsForeground value for the <see cref="DockingManager"/>. The default value of the TabItemsForeground property depends on selected theme or skin.
        /// </value>
        /// <remarks>
        /// TabItemsForeground dependency property defines foreground of the tab items. 
        /// </remarks>
        /// <example>
        /// To set TabItemsForeground property please see <see cref="SplitterBackground"/> property example.
        /// </example>
        public Brush TabItemsForeground
        {
            get
            {
                return ( Brush )GetValue( TabItemsForegroundProperty );
            }
            set
            {
                SetValue( TabItemsForegroundProperty, value );
            }
        }
        /// <summary>
        /// TabItemBackgroundSelected property is used to store background value for selected
        /// tab control item of the dock window.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides TabItemBackgroundSelected value for the <see cref="DockingManager"/>. The default value of the TabItemBackgroundSelected property depends on selected theme or skin.
        /// </value>
        /// <remarks>
        /// TabItemBackgroundSelected dependency property defines background of the selected tab item. 
        /// </remarks>
        /// <example>
        /// To set TabItemBackgroundSelected property please see <see cref="SplitterBackground"/> property example.
        /// </example>
        public Brush TabItemBackgroundSelected
        {
            get
            {
                return ( Brush )GetValue( TabItemBackgroundSelectedProperty );
            }
            set
            {
                SetValue( TabItemBackgroundSelectedProperty, value );
            }
        }
        /// <summary>
        /// TabItemForegroundSelected property is used to store background value for selected
        /// tab control item of the dock window.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides TabItemForegroundSelected value for the <see cref="DockingManager"/>. The default value of the TabItemForegroundSelected property depends on selected theme or skin.
        /// </value>
        /// <remarks>
        /// TabItemForegroundSelected dependency property defines foreground of the selected tab item. 
        /// </remarks>
        /// <example>
        /// To set TabItemForegroundSelected property please see <see cref="SplitterBackground"/> property example.
        /// </example>
        public Brush TabItemForegroundSelected
        {
            get
            {
                return ( Brush )GetValue( TabItemForegroundSelectedProperty );
            }
            set
            {
                SetValue( TabItemForegroundSelectedProperty, value );
            }
        }
        /// <summary>
        /// HeaderBorderBrush property is used to store border brush value for header of the dock window.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides HeaderBorderBrush value for the <see cref="DockingManager"/>. The default value of the HeaderBorderBrush property depends on selected theme or skin.
        /// </value>
        /// <remarks>
        /// HeaderBorderBrush dependency property defines border brush of the window`s header. 
        /// </remarks>
        /// <example>
        /// To set HeaderBorderBrush property please see <see cref="SplitterBackground"/> property example.
        /// </example>
        public Brush HeaderBorderBrush
        {
            get
            {
                return ( Brush )GetValue( HeaderBorderBrushProperty );
            }
            set
            {
                SetValue( HeaderBorderBrushProperty, value );
            }
        }
        /// <summary>
        /// HeaderBorderThickness property is used to store border thickness value for header of the dock window.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// Provides HeaderBorderThickness value for the <see cref="DockingManager"/>. The default value of the HeaderBorderThickness property is 1.
        /// </value>
        /// <remarks>
        /// HeaderBorderThickness dependency property defines border thickness of the window`s header. 
        /// </remarks>
        /// <example>
        /// To set HeaderBorderThickness property please see <see cref="FloatWindowBorderThickness"/> property example.
        /// </example>
        public Thickness HeaderBorderThickness
        {
            get
            {
                return ( Thickness )GetValue( HeaderBorderThicknessProperty );
            }
            set
            {
                SetValue( HeaderBorderThicknessProperty, value );
            }
        }
        /// <summary>
        /// ElementBorderThickness property is used to store border thickness value for host.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// Provides ElementBorderThickness value for the <see cref="DockingManager"/>. The default value of the ElementBorderThickness property is 1.
        /// </value>
        /// <remarks>
        /// ElementBorderThickness dependency property defines border thickness of the element inside the window. 
        /// </remarks>
        /// <example>
        /// To set ElementBorderThickness property please see <see cref="FloatWindowBorderThickness"/> property example.
        /// </example>
        public Thickness ElementBorderThickness
        {
            get
            {
                return ( Thickness )GetValue( ElementBorderThicknessProperty );
            }
            set
            {
                SetValue( ElementBorderThicknessProperty, value );
            }
        }
        /// <summary>
        /// SideItemsBorderBrush property is used to store border brush value for side panel items.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides SideItemsBorderBrush value for the <see cref="DockingManager"/>. The default value of the SideItemsBorderBrush property depends on selected theme or skin.
        /// </value>
        /// <remarks>
        /// SideItemsBorderBrush dependency property defines border brush of the side items. 
        /// </remarks>
        /// <example>
        /// To set SideItemsBorderBrush property please see <see cref="SplitterBackground"/> property example.
        /// </example>
        public Brush SideItemsBorderBrush
        {
            get
            {
                return ( Brush )GetValue( SideItemsBorderBrushProperty );
            }
            set
            {
                SetValue( SideItemsBorderBrushProperty, value );
            }
        }
        /// <summary>
        /// SidePanelItemsBorderThickness property is used to store border thickness value for side panel.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// Provides SidePanelItemsBorderThickness value for the <see cref="DockingManager"/>. The default value of the SidePanelItemsBorderThickness property is 1.
        /// </value>
        /// <remarks>
        /// SidePanelItemsBorderThickness dependency property defines border thickness of the side panel items. 
        /// </remarks>
        /// <example>
        /// To set SidePanelItemsBorderThickness property please see <see cref="FloatWindowBorderThickness"/> property example.
        /// </example>
        public Thickness SidePanelItemsBorderThickness
        {
            get
            {
                return ( Thickness )GetValue( SidePanelItemsBorderThicknessProperty );
            }
            set
            {
                SetValue( SidePanelItemsBorderThicknessProperty, value );
            }
        }
        /// <summary>
        /// TabItemsBorderBrush property is used to store border brush value for tab control items.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides TabItemsBorderBrush value for the <see cref="DockingManager"/>. The default value of the TabItemsBorderBrush property depends on selected theme or skin.
        /// </value>
        /// <remarks>
        /// TabItemsBorderBrush dependency property defines border brush of the tab items. 
        /// </remarks>
        /// <example>
        /// To set TabItemsBorderBrush property please see <see cref="SplitterBackground"/> property example.
        /// </example>
        public Brush TabItemsBorderBrush
        {
            get
            {
                return ( Brush )GetValue( TabItemsBorderBrushProperty );
            }
            set
            {
                SetValue( TabItemsBorderBrushProperty, value );
            }
        }
        /// <summary>
        /// TabItemBorderThickness property is used to store border thickness value for tab control items.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// Provides TabItemBorderThickness value for the <see cref="DockingManager"/>. The default value of the TabItemBorderThickness property is 1.
        /// </value>
        /// <remarks>
        /// TabItemBorderThickness dependency property defines border thickness of the tab item. 
        /// </remarks>
        /// <example>
        /// To set TabItemBorderThickness property please see <see cref="FloatWindowBorderThickness"/> property example.
        /// </example>
        public Thickness TabItemBorderThickness
        {
            get
            {
                return ( Thickness )GetValue( TabItemBorderThicknessProperty );
            }
            set
            {
                SetValue( TabItemBorderThicknessProperty, value );
            }
        }
        /// <summary>
        /// HeaderForeground property is used to store foreground brush value for header.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides HeaderForeground value for the <see cref="DockingManager"/>. The default value of the HeaderForeground property depends on selected theme or skin.
        /// </value>
        /// <remarks>
        /// HeaderForeground dependency property defines header foreground of the dock window. 
        /// </remarks>
        /// <example>
        /// To set HeaderForeground property please see <see cref="SplitterBackground"/> property example.
        /// </example>
        public Brush HeaderForeground
        {
            get
            {
                return ( Brush )GetValue( HeaderForegroundProperty );
            }
            set
            {
                SetValue( HeaderForegroundProperty, value );
            }
        }
        /// <summary>
        /// HeaderForegroundSelected property is used to store foreground brush value for selected header.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides HeaderForegroundSelected value for the <see cref="DockingManager"/>. The default value of the HeaderForegroundSelected property depends on selected theme or skin.
        /// </value>
        /// <remarks>
        /// HeaderForegroundSelected dependency property defines header foreground of the selected dock window. 
        /// </remarks>
        /// <example>
        /// To set HeaderForegroundSelected property please see <see cref="SplitterBackground"/> property example.
        /// </example>
        public Brush HeaderForegroundSelected
        {
            get
            {
                return ( Brush )GetValue( HeaderForegroundSelectedProperty );
            }
            set
            {
                SetValue( HeaderForegroundSelectedProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the TabItemsCornerRadius dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="CornerRadius"/>
        /// Provides TabItemsCornerRadius value for the <see cref="DockingManager"/>. The default value of the TabItemsCornerRadius property is 0.
        /// </value>
        /// <remarks>
        /// TabItemsCornerRadius dependency property defines corner radius of the tab items. 
        /// </remarks>
        /// <example>
        /// To set TabItemsCornerRadius property please see <see cref="FloatWindowBorderThickness"/> property example.
        /// </example>
        public CornerRadius TabItemsCornerRadius
        {
            get
            {
                return ( CornerRadius )GetValue( TabItemsCornerRadiusProperty );
            }
            set
            {
                SetValue( TabItemsCornerRadiusProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the SidePanelBackground dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides SidePanelBackground value for the <see cref="DockingManager"/>. The default value of the SidePanelBackground property is Transparent.
        /// </value>
        /// <remarks>
        /// SidePanelBackground dependency property defines background of the <see cref="SidePanel"/>. 
        /// </remarks>
        /// <example>
        /// To set SidePanelBackground property please see <see cref="SplitterBackground"/> property example.
        /// </example>
        public Brush SidePanelBackground
        {
            get
            {
                return ( Brush )GetValue( SidePanelBackgroundProperty );
            }
            set
            {
                SetValue( SidePanelBackgroundProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the TabPanelBackground dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides TabPanelBackground value for the <see cref="DockingManager"/>. The default value of the TabPanelBackground property is Transparent.
        /// </value>
        /// <remarks>
        /// TabPanelBackground dependency property defines background of the tab panel. 
        /// </remarks>
        /// <example>
        /// To set TabPanelBackground property please see <see cref="SplitterBackground"/> property example.
        /// </example>
        public Brush TabPanelBackground
        {
            get
            {
                return ( Brush )GetValue( TabPanelBackgroundProperty );
            }
            set
            {
                SetValue( TabPanelBackgroundProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the TabPanelBorderBrush dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides TabPanelBorderBrush value for the <see cref="DockingManager"/>. The default value of the TabPanelBorderBrush property is Transparent.
        /// </value>
        /// <remarks>
        /// TabPanelBorderBrush dependency property defines border brush of the tab panel. 
        /// </remarks>
        /// <example>
        /// To set TabPanelBorderBrush property please see <see cref="SplitterBackground"/> property example.
        /// </example>
        public Brush TabPanelBorderBrush
        {
            get
            {
                return ( Brush )GetValue( TabPanelBorderBrushProperty );
            }
            set
            {
                SetValue( TabPanelBorderBrushProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the TabPanelBorderThickness dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// Provides TabPanelBorderThickness value for the <see cref="DockingManager"/>. The default value of the TabPanelBorderThickness property is 0.
        /// </value>
        /// <remarks>
        /// TabPanelBorderThickness dependency property defines border thickness of the tab panel. 
        /// </remarks>
        /// <example>
        /// To set TabPanelBorderThickness property please see <see cref="FloatWindowBorderThickness"/> property example.
        /// </example>
        public Thickness TabPanelBorderThickness
        {
            get
            {
                return ( Thickness )GetValue( TabPanelBorderThicknessProperty );
            }
            set
            {
                SetValue( TabPanelBorderThicknessProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the SidePanelBorderBrush dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides SidePanelBorderBrush value for the <see cref="DockingManager"/>. The default value of the SidePanelBorderBrush property is Transparent.
        /// </value>
        /// <remarks>
        /// SidePanelBorderBrush dependency property defines border brush of the <see cref="SidePanel"/>. 
        /// </remarks>
        /// <example>
        /// To set SidePanelBorderBrush property please see <see cref="SplitterBackground"/> property example.
        /// </example>
        public Brush SidePanelBorderBrush
        {
            get
            {
                return ( Brush )GetValue( SidePanelBorderBrushProperty );
            }
            set
            {
                SetValue( SidePanelBorderBrushProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the SidePanelBorderThickness dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// Provides SidePanelBorderThickness value for the <see cref="DockingManager"/>. The default value of the SidePanelBorderThickness property is 0.
        /// </value>
        /// <remarks>
        /// SidePanelBorderThickness dependency property defines border thickness of the <see cref="SidePanel"/>. 
        /// </remarks>
        /// <example>
        /// To set SidePanelBorderThickness property please see <see cref="FloatWindowBorderThickness"/> property example.
        /// </example>
        public Thickness SidePanelBorderThickness
        {
            get
            {
                return ( Thickness )GetValue( SidePanelBorderThicknessProperty );
            }
            set
            {
                SetValue( SidePanelBorderThicknessProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the TabItemsBorderThicknessSelected dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// Provides TabItemsBorderThicknessSelected value for the <see cref="DockingManager"/>. The default value of the TabItemsBorderThicknessSelected property is 0.
        /// </value>
        /// <remarks>
        /// TabItemsBorderThicknessSelected dependency property defines border thickness of the selected tab items. 
        /// </remarks>
        /// <example>
        /// To set TabItemsBorderThicknessSelected property please see <see cref="FloatWindowBorderThickness"/> property example.
        /// </example>
        public Thickness TabItemsBorderThicknessSelected
        {
            get
            {
                return ( Thickness )GetValue( TabItemsBorderThicknessSelectedProperty );
            }
            set
            {
                SetValue( TabItemsBorderThicknessSelectedProperty, value );
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Calls OnFloatWindowHeaderBackgroundChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFloatWindowHeaderBackgroundChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            DockingManager instance = ( DockingManager )d;
            instance.OnFloatWindowHeaderBackgroundChanged( e );
        }
        /// <summary>
        /// Calls OnFloatWindowSelectedHeaderBackgroundChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFloatWindowSelectedHeaderBackgroundChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            DockingManager instance = ( DockingManager )d;
            instance.OnFloatWindowSelectedHeaderBackgroundChanged( e );
        }
        /// <summary>
        /// Calls OnFloatWindowMouseOverHeaderBackgroundChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFloatWindowMouseOverHeaderBackgroundChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            DockingManager instance = ( DockingManager )d;
            instance.OnFloatWindowMouseOverHeaderBackgroundChanged( e );
        }
        /// <summary>
        /// Calls OnFloatWindowHeaderForegroundChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFloatWindowHeaderForegroundChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            DockingManager instance = ( DockingManager )d;
            instance.OnFloatWindowHeaderForegroundChanged( e );
        }
        /// <summary>
        /// Calls OnFloatWindowSelectedHeaderForegroundChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFloatWindowSelectedHeaderForegroundChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            DockingManager instance = ( DockingManager )d;
            instance.OnFloatWindowSelectedHeaderForegroundChanged( e );
        }
        /// <summary>
        /// Calls OnFloatWindowMouseOverHeaderForegroundChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFloatWindowMouseOverHeaderForegroundChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            DockingManager instance = ( DockingManager )d;
            instance.OnFloatWindowMouseOverHeaderForegroundChanged( e );
        }
        /// <summary>
        /// Raises FloatWindowHeaderBackgroundChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnFloatWindowHeaderBackgroundChanged( DependencyPropertyChangedEventArgs e )
        {
            if( FloatWindowHeaderBackgroundChanged != null )
            {
                FloatWindowHeaderBackgroundChanged( this, e );
            }
        }
        /// <summary>
        /// Raises FloatWindowSelectedHeaderBackgroundChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnFloatWindowSelectedHeaderBackgroundChanged( DependencyPropertyChangedEventArgs e )
        {
            if( FloatWindowSelectedHeaderBackgroundChanged != null )
            {
                FloatWindowSelectedHeaderBackgroundChanged( this, e );
            }
        }
        /// <summary>
        /// Raises FloatWindowMouseOverHeaderBackgroundChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnFloatWindowMouseOverHeaderBackgroundChanged( DependencyPropertyChangedEventArgs e )
        {
            if( FloatWindowMouseOverHeaderBackgroundChanged != null )
            {
                FloatWindowMouseOverHeaderBackgroundChanged( this, e );
            }
        }
        /// <summary>
        /// Raises FloatWindowHeaderForegroundChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnFloatWindowHeaderForegroundChanged( DependencyPropertyChangedEventArgs e )
        {
            if( FloatWindowHeaderForegroundChanged != null )
            {
                FloatWindowHeaderForegroundChanged( this, e );
            }
        }
        /// <summary>
        /// Raises FloatWindowSelectedHeaderForegroundChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnFloatWindowSelectedHeaderForegroundChanged( DependencyPropertyChangedEventArgs e )
        {
            if( FloatWindowSelectedHeaderForegroundChanged != null )
            {
                FloatWindowSelectedHeaderForegroundChanged( this, e );
            }
        }
        /// <summary>
        /// Raises FloatWindowMouseOverHeaderForegroundChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnFloatWindowMouseOverHeaderForegroundChanged( DependencyPropertyChangedEventArgs e )
        {
            if( FloatWindowMouseOverHeaderForegroundChanged != null )
            {
                FloatWindowMouseOverHeaderForegroundChanged( this, e );
            }
        }
        /// <summary>
        /// Calls OnFloatWindowBorderBrushChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFloatWindowBorderBrushChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            DockingManager instance = ( DockingManager )d;
            ImageBrush imageBrush = e.NewValue as ImageBrush;

            if( imageBrush == null )
            {
                Brush newBrush = e.NewValue as Brush;
                instance.LeftFloatWindowBorderBrush = newBrush;
                instance.RightFloatWindowBorderBrush = newBrush;
                instance.LeftFloatWindowTopBorderBrush = newBrush;
                instance.LeftFloatWindowBottomBorderBrush = newBrush;
                instance.RightFloatWindowTopBorderBrush = newBrush;
                instance.RightFloatWindowBottomBorderBrush = newBrush;
            }

            instance.OnFloatWindowBorderBrushChanged( e );
        }
        /// <summary>
        /// Raises FloatWindowBorderBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnFloatWindowBorderBrushChanged( DependencyPropertyChangedEventArgs e )
        {
            if( FloatWindowBorderBrushChanged != null )
            {
                FloatWindowBorderBrushChanged( this, e );
            }
        }
        /// <summary>
        /// Calls OnFloatWindowSelectedBorderBrushChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFloatWindowSelectedBorderBrushChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            DockingManager instance = ( DockingManager )d;
            ImageBrush imageBrush = e.NewValue as ImageBrush;

            if( imageBrush == null )
            {
                Brush newBrush = e.NewValue as Brush;
                instance.LeftFloatWindowSelectedBorderBrush = newBrush;
                instance.RightFloatWindowSelectedBorderBrush = newBrush;
                instance.LeftFloatWindowSelectedTopBorderBrush = newBrush;
                instance.LeftFloatWindowSelectedBottomBorderBrush = newBrush;
                instance.RightFloatWindowSelectedTopBorderBrush = newBrush;
                instance.RightFloatWindowSelectedBottomBorderBrush = newBrush;
            }

            instance.OnFloatWindowSelectedBorderBrushChanged( e );
        }
        /// <summary>
        /// Calls OnFloatWindowMouseOverBorderBrushChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFloatWindowMouseOverBorderBrushChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            DockingManager instance = ( DockingManager )d;
            ImageBrush imageBrush = e.NewValue as ImageBrush;

            if( imageBrush == null )
            {
                Brush newBrush = e.NewValue as Brush;
                instance.LeftFloatMouseOverBorderBrush = newBrush;
                instance.RightFloatMouseOverBorderBrush = newBrush;
                instance.LeftFloatMouseOverTopBorderBrush = newBrush;
                instance.LeftFloatMouseOverBottomBorderBrush = newBrush;
                instance.RightFloatMouseOverTopBorderBrush = newBrush;
                instance.RightFloatMouseOverBottomBorderBrush = newBrush;
            }

            instance.OnFloatWindowMouseOverBorderBrushChanged( e );
        }
        /// <summary>
        /// Raises FloatWindowSelectedBorderBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnFloatWindowSelectedBorderBrushChanged( DependencyPropertyChangedEventArgs e )
        {
            if( FloatWindowSelectedBorderBrushChanged != null )
            {
                FloatWindowSelectedBorderBrushChanged( this, e );
            }
        }
        /// <summary>
        /// Raises FloatWindowMouseOverBorderBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnFloatWindowMouseOverBorderBrushChanged( DependencyPropertyChangedEventArgs e )
        {
            if( FloatWindowMouseOverBorderBrushChanged != null )
            {
                FloatWindowMouseOverBorderBrushChanged( this, e );
            }
        }
        /// <summary>
        /// Calls OnFloatWindowBorderThicknessChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFloatWindowBorderThicknessChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            DockingManager instance = ( DockingManager )d;
            Thickness newValue = ( Thickness )e.NewValue;

            instance.FloatWindowTopBorderHeight = newValue.Top;
            instance.FloatWindowBottomBorderHeight = newValue.Bottom;
            instance.FloatWindowLeftBorderWidth = newValue.Left;
            instance.FloatWindowRightBorderWidth = newValue.Right;
			instance.OnFloatWindowBorderThicknessChanged( e );
        }
        /// <summary>
        /// Raises FloatWindowBorderThicknessChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnFloatWindowBorderThicknessChanged( DependencyPropertyChangedEventArgs e )
        {
            if( FloatWindowBorderThicknessChanged != null )
            {
                FloatWindowBorderThicknessChanged( this, e );
            }
        }
        /// <summary>
        /// Raises SidePanelBackgroundChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnSidePanelBackgroundChanged( DependencyPropertyChangedEventArgs e )
        {
            if( SidePanelBackgroundChanged != null )
            {
                SidePanelBackgroundChanged( this, e );
            }
        }
        /// <summary>
        /// Raises TabPanelBackgroundChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnTabPanelBackgroundChanged( DependencyPropertyChangedEventArgs e )
        {
            if( TabPanelBackgroundChanged != null )
            {
                TabPanelBackgroundChanged( this, e );
            }
        }
        /// <summary>
        /// Raises TabItemBorderThicknessChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnTabItemBorderThicknessChanged( DependencyPropertyChangedEventArgs e )
        {
            if( TabItemBorderThicknessChanged != null )
            {
                TabItemBorderThicknessChanged( this, e );
            }
        }
        /// <summary>
        /// Raises SidePanelBorderBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnSidePanelBorderBrushChanged( DependencyPropertyChangedEventArgs e )
        {
            if( SidePanelBorderBrushChanged != null )
            {
                SidePanelBorderBrushChanged( this, e );
            }
        }
        /// <summary>
        /// Raises SidePanelBorderThicknessChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnSidePanelBorderThicknessChanged( DependencyPropertyChangedEventArgs e )
        {
            if( SidePanelBorderThicknessChanged != null )
            {
                SidePanelBorderThicknessChanged( this, e );
            }
        }
        /// <summary>
        /// Raises TabPanelBorderBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnTabPanelBorderBrushChanged( DependencyPropertyChangedEventArgs e )
        {
            if( TabPanelBorderBrushChanged != null )
            {
                TabPanelBorderBrushChanged( this, e );
            }
        }
        /// <summary>
        /// Raises TabPanelBorderThicknessChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnTabPanelBorderThicknessChanged( DependencyPropertyChangedEventArgs e )
        {
            if( TabPanelBorderThicknessChanged != null )
            {
                TabPanelBorderThicknessChanged( this, e );
            }
        }
        /// <summary>
        /// Raises SplitterBackgroundChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnSplitterBackgroundChanged( DependencyPropertyChangedEventArgs e )
        {
            if( SplitterBackgroundChanged != null )
            {
                SplitterBackgroundChanged( this, e );
            }
        }
        /// <summary>
        /// Raises SplitterSizeChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnSplitterSizeChanged( DependencyPropertyChangedEventArgs e )
        {
            if( SplitterSizeChanged != null )
            {
                SplitterSizeChanged( this, e );
            }
        }
        /// <summary>
        /// Calls OnSplitterBackgroundChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSplitterBackgroundChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            DockingManager instance = ( DockingManager )d;
            instance.OnSplitterBackgroundChanged( e );
        }
        /// <summary>
        /// Calls OnSplitterSizeChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSplitterSizeChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            DockingManager instance = ( DockingManager )d;
            instance.OnSplitterSizeChanged( e );
        }
        /// <summary>
        /// Raises TabItemsCornerRadiusChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnTabItemsCornerRadiusChanged( DependencyPropertyChangedEventArgs e )
        {
            if( TabItemsCornerRadiusChanged != null )
            {
                TabItemsCornerRadiusChanged( this, e );
            }
        }
        /// <summary>
        /// Raises TabItemsBorderThicknessSelectedChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnTabItemsBorderThicknessSelectedChanged( DependencyPropertyChangedEventArgs e )
        {
            if( TabItemsBorderThicknessSelectedChanged != null )
            {
                TabItemsBorderThicknessSelectedChanged( this, e );
            }
        }
        /// <summary>
        /// Calls OnTabItemsCornerRadiusChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabItemsCornerRadiusChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            DockingManager instance = ( DockingManager )d;
            instance.OnTabItemsCornerRadiusChanged( e );
        }
        /// <summary>
        /// Calls OnSidePanelBackgroundChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSidePanelBackgroundChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            DockingManager instance = ( DockingManager )d;
            instance.OnSidePanelBackgroundChanged( e );
        }
        /// <summary>
        /// Calls OnTabPanelBackgroundChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabPanelBackgroundChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            DockingManager instance = ( DockingManager )d;
            instance.OnTabPanelBackgroundChanged( e );
        }
        /// <summary>
        /// Calls OnTabItemBorderThicknessChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabItemBorderThicknessChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            DockingManager instance = ( DockingManager )d;
            instance.OnTabItemBorderThicknessChanged( e );
        }
        /// <summary>
        /// Calls OnSidePanelBorderBrushChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSidePanelBorderBrushChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            DockingManager instance = ( DockingManager )d;
            instance.OnSidePanelBorderBrushChanged( e );
        }
        /// <summary>
        /// Calls OnSidePanelBorderThicknessChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSidePanelBorderThicknessChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            DockingManager instance = ( DockingManager )d;
            instance.OnSidePanelBorderThicknessChanged( e );
        }
        /// <summary>
        /// Calls OnTabPanelBorderBrushChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabPanelBorderBrushChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            DockingManager instance = ( DockingManager )d;
            instance.OnTabPanelBorderBrushChanged( e );
        }
        /// <summary>
        /// Calls OnTabPanelBorderThicknessChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabPanelBorderThicknessChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            DockingManager instance = ( DockingManager )d;
            instance.OnTabPanelBorderThicknessChanged( e );
        }
        /// <summary>
        /// Calls OnTabItemsBorderThicknessSelectedChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabItemsBorderThicknessSelectedChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            DockingManager instance = ( DockingManager )d;
            instance.OnTabItemsBorderThicknessSelectedChanged( e );
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies FloatWindowHeaderBackground dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty FloatWindowHeaderBackgroundProperty =
            DependencyProperty.Register( "FloatWindowHeaderBackground", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent, new PropertyChangedCallback( OnFloatWindowHeaderBackgroundChanged ) ) );
        /// <summary>
        /// Identifies FloatWindowSelectedHeaderBackground dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty FloatWindowSelectedHeaderBackgroundProperty =
            DependencyProperty.Register( "FloatWindowSelectedHeaderBackground", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent, new PropertyChangedCallback( OnFloatWindowSelectedHeaderBackgroundChanged ) ) );
        /// <summary>
        /// Identifies FloatWindowMouseOverHeaderBackground dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty FloatWindowMouseOverHeaderBackgroundProperty =
            DependencyProperty.Register( "FloatWindowMouseOverHeaderBackground", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent, new PropertyChangedCallback( OnFloatWindowMouseOverHeaderBackgroundChanged ) ) );
        /// <summary>
        /// Identifies FloatWindowHeaderForeground dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty FloatWindowHeaderForegroundProperty =
            DependencyProperty.Register( "FloatWindowHeaderForeground", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent, new PropertyChangedCallback( OnFloatWindowHeaderForegroundChanged ) ) );
        /// <summary>
        /// Identifies FloatWindowSelectedHeaderBackground dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty FloatWindowSelectedHeaderForegroundProperty =
            DependencyProperty.Register( "FloatWindowSelectedHeaderForeground", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent, new PropertyChangedCallback( OnFloatWindowSelectedHeaderForegroundChanged ) ) );
        /// <summary>
        /// Identifies FloatWindowMouseOverHeaderForeground dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty FloatWindowMouseOverHeaderForegroundProperty =
            DependencyProperty.Register( "FloatWindowMouseOverHeaderForeground", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent, new PropertyChangedCallback( OnFloatWindowMouseOverHeaderForegroundChanged ) ) );
        /// <summary>
        /// Identifies LeftFloatWindowTopBorderBrush dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty LeftFloatWindowTopBorderBrushProperty =
            DependencyProperty.Register( "LeftFloatWindowTopBorderBrush", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent ) );
        /// <summary>
        /// Identifies LeftFloatWindowSelectedTopBorderBrush dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty LeftFloatWindowSelectedTopBorderBrushProperty =
            DependencyProperty.Register( "LeftFloatWindowSelectedTopBorderBrush", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent ) );
        /// <summary>
        /// Identifies LeftFloatMouseOverTopBorderBrush dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty LeftFloatMouseOverTopBorderBrushProperty =
            DependencyProperty.Register( "LeftFloatMouseOverTopBorderBrush", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent ) );
        /// <summary>
        /// Identifies LeftFloatWindowBottomBorderBrush dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty LeftFloatWindowBottomBorderBrushProperty =
            DependencyProperty.Register( "LeftFloatWindowBottomBorderBrush", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent ) );
        /// <summary>
        /// Identifies LeftFloatWindowSelectedBottomBorderBrush dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty LeftFloatWindowSelectedBottomBorderBrushProperty =
            DependencyProperty.Register( "LeftFloatWindowSelectedBottomBorderBrush", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent ) );
        /// <summary>
        /// Identifies LeftFloatMouseOverBottomBorderBrush dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty LeftFloatMouseOverBottomBorderBrushProperty =
            DependencyProperty.Register( "LeftFloatMouseOverBottomBorderBrush", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent ) );
        /// <summary>
        /// Identifies LeftFloatWindowBorderBrush dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty LeftFloatWindowBorderBrushProperty =
            DependencyProperty.Register( "LeftFloatWindowBorderBrush", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent ) );
        /// <summary>
        /// Identifies LeftFloatWindowSelectedBorderBrush dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty LeftFloatWindowSelectedBorderBrushProperty =
            DependencyProperty.Register( "LeftFloatWindowSelectedBorderBrush", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent ) );
        /// <summary>
        /// Identifies LeftFloatMouseOverBorderBrush dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty LeftFloatMouseOverBorderBrushProperty =
            DependencyProperty.Register( "LeftFloatMouseOverBorderBrush", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent ) );
        /// <summary>
        /// Identifies RightFloatWindowTopBorderBrush dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty RightFloatWindowTopBorderBrushProperty =
            DependencyProperty.Register( "RightFloatWindowTopBorderBrush", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent ) );
        /// <summary>
        /// Identifies RightFloatWindowSelectedTopBorderBrush dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty RightFloatWindowSelectedTopBorderBrushProperty =
            DependencyProperty.Register( "RightFloatWindowSelectedTopBorderBrush", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent ) );
        /// <summary>
        /// Identifies RightFloatMouseOverTopBorderBrush dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty RightFloatMouseOverTopBorderBrushProperty =
            DependencyProperty.Register( "RightFloatMouseOverTopBorderBrush", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent ) );
        /// <summary>
        /// Identifies RightFloatWindowBottomBorderBrush dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty RightFloatWindowBottomBorderBrushProperty =
            DependencyProperty.Register( "RightFloatWindowBottomBorderBrush", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent ) );
        /// <summary>
        /// Identifies RightFloatWindowSelectedBottomBorderBrush dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty RightFloatWindowSelectedBottomBorderBrushProperty =
            DependencyProperty.Register( "RightFloatWindowSelectedBottomBorderBrush", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent ) );
        /// <summary>
        /// Identifies RightFloatMouseOverBottomBorderBrush dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty RightFloatMouseOverBottomBorderBrushProperty =
            DependencyProperty.Register( "RightFloatMouseOverBottomBorderBrush", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent ) );
        /// <summary>
        /// Identifies RightFloatWindowBorderBrush dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty RightFloatWindowBorderBrushProperty =
            DependencyProperty.Register( "RightFloatWindowBorderBrush", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent ) );
        /// <summary>
        /// Identifies RightFloatWindowSelectedBorderBrush dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty RightFloatWindowSelectedBorderBrushProperty =
            DependencyProperty.Register( "RightFloatWindowSelectedBorderBrush", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent ) );
        /// <summary>
        /// Identifies RightFloatMouseOverBorderBrush dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty RightFloatMouseOverBorderBrushProperty =
            DependencyProperty.Register( "RightFloatMouseOverBorderBrush", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent ) );
        /// <summary>
        /// Identifies FloatWindowBorderBrush dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty FloatWindowBorderBrushProperty =
            DependencyProperty.Register( "FloatWindowBorderBrush", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent, new PropertyChangedCallback( OnFloatWindowBorderBrushChanged ) ) );
        /// <summary>
        /// Identifies FloatWindowSelectedBorderBrush dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty FloatWindowSelectedBorderBrushProperty =
            DependencyProperty.Register( "FloatWindowSelectedBorderBrush", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent, new PropertyChangedCallback( OnFloatWindowSelectedBorderBrushChanged ) ) );
        /// <summary>
        /// Identifies FloatWindowMouseOverBorderBrush dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty FloatWindowMouseOverBorderBrushProperty =
            DependencyProperty.Register( "FloatWindowMouseOverBorderBrush", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent, new PropertyChangedCallback( OnFloatWindowMouseOverBorderBrushChanged ) ) );
        /// <summary>
        /// Identifies FloatWindowBorderThickness dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty FloatWindowBorderThicknessProperty =
            DependencyProperty.Register( "FloatWindowBorderThickness", typeof( Thickness )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( new Thickness( 22,4,4,4 ), new PropertyChangedCallback( OnFloatWindowBorderThicknessChanged ) ) );
        /// <summary>
        /// Identifies FloatWindowTopBorderHeight dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty FloatWindowTopBorderHeightProperty = DependencyProperty.Register("FloatWindowTopBorderHeight", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata(22d));
        /// <summary>
        /// Identifies FloatWindowBottomBorderHeight dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty FloatWindowBottomBorderHeightProperty = DependencyProperty.Register("FloatWindowBottomBorderHeight", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata(4d));
        /// <summary>
        /// Identifies FloatWindowLeftBorderWidth dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty FloatWindowLeftBorderWidthProperty = DependencyProperty.Register("FloatWindowLeftBorderWidth", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata(4d));
        /// <summary>
        /// Identifies FloatWindowRightBorderWidth dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty FloatWindowRightBorderWidthProperty = DependencyProperty.Register("FloatWindowRightBorderWidth", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata(4d));
        /// <summary>
        /// Identifies DockHeaderStyle dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty DockHeaderStyleProperty =
            DependencyProperty.Register( "DockHeaderStyle", typeof( Style ), typeof( DockingManager )
            , new UIPropertyMetadata( null ) );
        /// <summary>
        /// Identifies the DockedElementHostStyle dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty DockedElementHostStyleProperty =
            DependencyProperty.Register( "DockedElementHostStyle", typeof( Style ), typeof( DockingManager )
            , new UIPropertyMetadata( null ) );
        /// <summary>
        /// Identifies the FloatWindowStyle dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty FloatWindowStyleProperty =
            DependencyProperty.Register( "FloatWindowStyle", typeof( Style ), typeof( DockingManager )
            , new UIPropertyMetadata( null ) );
        /// <summary>
        /// Identifies the TabItemStyle dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty TabItemStyleProperty =
            DependencyProperty.Register( "TabItemStyle", typeof( Style ), typeof( DockingManager )
            , new FrameworkPropertyMetadata( null ) );
        /// <summary>
        /// Identifies the TabControlStyle dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty TabControlStyleProperty =
            DependencyProperty.Register( "TabControlStyle", typeof( Style ), typeof( DockingManager )
            , new UIPropertyMetadata( null ) );
        /// <summary>
        /// Identifies the MainHostStyle dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty MainHostStyleProperty =
            DependencyProperty.Register( "MainHostStyle", typeof( Style ), typeof( DockingManager )
            , new UIPropertyMetadata( null ) );
        /// <summary>
        /// Identifies the SidePanelStyle dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty SidePanelStyleProperty =
            DependencyProperty.Register( "SidePanelStyle", typeof( Style ), typeof( DockingManager )
            , new UIPropertyMetadata( null ) );
        /// <summary>
        /// Identifies the SideItemStyle dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty SideItemStyleProperty =
            DependencyProperty.Register( "SideItemStyle", typeof( Style ), typeof( DockingManager )
            , new UIPropertyMetadata( null ) );
        /// <summary>
        /// Identifies the Background dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty =
            Border.BackgroundProperty.AddOwner( typeof( DockingManager )
            , new FrameworkPropertyMetadata( Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender ) );
        /// <summary>
        /// Identifies the HeaderTemplate dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.RegisterAttached( "HeaderTemplate", typeof( DataTemplate ), typeof( DockingManager )
            , new FrameworkPropertyMetadata( null, FrameworkPropertyMetadataOptions.Inherits ) );
        /// <summary>
        /// Identifies the CloseButtonTemplate dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty CloseButtonTemplateProperty =
            DependencyProperty.Register( "CloseButtonTemplate", typeof( ControlTemplate ), typeof( DockingManager )
            , new UIPropertyMetadata( null ) );
        /// <summary>
        /// Identifies the MenuButtonTemplate dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty MenuButtonTemplateProperty =
            DependencyProperty.Register( "MenuButtonTemplate", typeof( ControlTemplate ), typeof( DockingManager )
            , new UIPropertyMetadata( null ) );
        /// <summary>
        /// Identifies the AwlButtonTemplate dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty AwlButtonTemplateProperty =
            DependencyProperty.Register( "AwlButtonTemplate", typeof( ControlTemplate ), typeof( DockingManager )
            , new UIPropertyMetadata( null ) );
        /// <summary>
        /// Identifies the SplitterBackground dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty SplitterBackgroundProperty =
            DependencyProperty.Register( "SplitterBackground", typeof( Brush ), typeof( DockingManager )
            , new FrameworkPropertyMetadata( Brushes.Transparent
            , new PropertyChangedCallback( OnSplitterBackgroundChanged ) ) );
        /// <summary>
        /// Identifies the SplitterSize dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty SplitterSizeProperty =
            DependencyProperty.Register( "SplitterSize", typeof( double )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( 4d
            , new PropertyChangedCallback( OnSplitterSizeChanged ) ) );
        /// <summary>
        /// Identifies HeaderBackground dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register( "HeaderBackground", typeof( Brush ), typeof( DockingManager ) );
        /// <summary>
        /// Identifies SelectedHeaderBackground dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty SelectedHeaderBackgroundProperty =
            DependencyProperty.Register( "SelectedHeaderBackground", typeof( Brush ), typeof( DockingManager ) );
        /// <summary>
        /// Identifies HeaderMouseOverBackground dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty HeaderMouseOverBackgroundProperty =
            DependencyProperty.Register( "HeaderMouseOverBackground", typeof( Brush ), typeof( DockingManager ) );
        /// <summary>
        /// Identifies SideItemsBackground dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty SideItemsBackgroundProperty =
            DependencyProperty.Register( "SideItemsBackground", typeof( Brush ), typeof( DockingManager ) );
        /// <summary>
        /// Identifies TabItemsBackground dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty TabItemsBackgroundProperty =
            DependencyProperty.Register( "TabItemsBackground", typeof( Brush ), typeof( DockingManager ) );
        /// <summary>
        /// Identifies TabItemsForeground dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty TabItemsForegroundProperty =
            DependencyProperty.Register( "TabItemsForeground", typeof( Brush ), typeof( DockingManager ) );
        /// <summary>
        /// Identifies TabItemBackgroundSelected dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty TabItemBackgroundSelectedProperty =
            DependencyProperty.Register( "TabItemBackgroundSelected", typeof( Brush ), typeof( DockingManager ) );
        /// <summary>
        /// Identifies TabItemForegroundSelected dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty TabItemForegroundSelectedProperty =
            DependencyProperty.Register( "TabItemForegroundSelected", typeof( Brush ), typeof( DockingManager ) );
        /// <summary>
        /// Identifies HeaderBorderBrush dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty HeaderBorderBrushProperty =
            DependencyProperty.Register( "HeaderBorderBrush", typeof( Brush ), typeof( DockingManager ) );
        /// <summary>
        /// Identifies HeaderBorderThickness dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty HeaderBorderThicknessProperty =
            DependencyProperty.Register( "HeaderBorderThickness", typeof( Thickness ), typeof( DockingManager ) );
        /// <summary>
        /// Identifies ElementBorderThickness dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty ElementBorderThicknessProperty =
            DependencyProperty.Register( "ElementBorderThickness", typeof( Thickness ), typeof( DockingManager )
            , new UIPropertyMetadata( new Thickness( 1 ) ) );
        /// <summary>
        /// Identifies SideItemsBorderBrush dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty SideItemsBorderBrushProperty =
            DependencyProperty.Register( "SideItemsBorderBrush", typeof( Brush ), typeof( DockingManager ) );
        /// <summary>
        /// Identifies SidePanelItemsBorderThickness dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty SidePanelItemsBorderThicknessProperty =
            DependencyProperty.Register( "SidePanelItemsBorderThickness", typeof( Thickness ), typeof( DockingManager ) );
        /// <summary>
        /// Identifies TabItemsBorderBrush dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty TabItemsBorderBrushProperty =
            DependencyProperty.Register( "TabItemsBorderBrush", typeof( Brush ), typeof( DockingManager ) );
        /// <summary>
        /// Identifies TabItemBorderThickness dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty TabItemBorderThicknessProperty =
            DependencyProperty.Register( "TabItemBorderThickness", typeof( Thickness ), typeof( DockingManager ), new UIPropertyMetadata( new Thickness( 1 ), new PropertyChangedCallback( OnTabItemBorderThicknessChanged ) ) );
        /// <summary>
        /// Identifies HeaderForeground dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty HeaderForegroundProperty =
            DependencyProperty.Register( "HeaderForeground", typeof( Brush ), typeof( DockingManager ) );
        /// <summary>
        /// Identifies HeaderForegroundSelected dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty HeaderForegroundSelectedProperty =
            DependencyProperty.Register( "HeaderForegroundSelected", typeof( Brush ), typeof( DockingManager ) );
        /// <summary>
        /// This property define corner radius of TabItem of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty TabItemsCornerRadiusProperty =
            DependencyProperty.Register( "TabItemsCornerRadius", typeof( CornerRadius ), typeof( DockingManager )
            , new FrameworkPropertyMetadata( new CornerRadius( 0 )
            , new PropertyChangedCallback( OnTabItemsCornerRadiusChanged ) ) );
        /// <summary>
        /// Identifies SidePanelBackground dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty SidePanelBackgroundProperty =
            DependencyProperty.Register( "SidePanelBackground", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent
            , new PropertyChangedCallback( OnSidePanelBackgroundChanged ) ) );
        /// <summary>
        /// Identifies TabPanelBackground dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty TabPanelBackgroundProperty =
            DependencyProperty.Register( "TabPanelBackground", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent
            , new PropertyChangedCallback( OnTabPanelBackgroundChanged ) ) );
        /// <summary>
        /// Identifies TabPanelBorderBrush dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty TabPanelBorderBrushProperty =
            DependencyProperty.Register( "TabPanelBorderBrush", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent
            , new PropertyChangedCallback( OnTabPanelBorderBrushChanged ) ) );
        /// <summary>
        /// Identifies SidePanelBorderBrush dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty SidePanelBorderBrushProperty =
            DependencyProperty.Register( "SidePanelBorderBrush", typeof( Brush )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( Brushes.Transparent
            , new PropertyChangedCallback( OnSidePanelBorderBrushChanged ) ) );
        /// <summary>
        /// Identifies SidePanelBorderThickness dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty SidePanelBorderThicknessProperty =
            DependencyProperty.Register( "SidePanelBorderThickness", typeof( Thickness )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( new Thickness( 0 )
            , new PropertyChangedCallback( OnSidePanelBorderThicknessChanged ) ) );
        /// <summary>
        /// Identifies TabPanelBorderThickness dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty TabPanelBorderThicknessProperty =
            DependencyProperty.Register( "TabPanelBorderThickness", typeof( Thickness )
            , typeof( DockingManager ), new FrameworkPropertyMetadata( new Thickness( 0 )
            , new PropertyChangedCallback( OnTabPanelBorderThicknessChanged ) ) );
        /// <summary>
        /// Identifies TabItemsBorderThicknessSelected dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty TabItemsBorderThicknessSelectedProperty =
            DependencyProperty.Register( "TabItemsBorderThicknessSelected"
            , typeof( Thickness ), typeof( DockingManager )
            , new FrameworkPropertyMetadata( new Thickness( 0 )
                , new PropertyChangedCallback( OnTabItemsBorderThicknessSelectedChanged ) ) );
        #endregion
    }
}