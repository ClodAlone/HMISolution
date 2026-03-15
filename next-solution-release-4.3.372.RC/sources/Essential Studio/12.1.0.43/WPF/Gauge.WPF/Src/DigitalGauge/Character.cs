// <copyright file="Character.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the character base class.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <Window x:Class="CharacterSample.Window1" Title="CharacterSample" Height="400"
    /// Width="400" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf">
    ///     <Grid>
    ///         <syncfusion:DigitalGauge Name="digitalGauge1" Width="350" Height="100"
    ///                                  CharacterCount="9" CharacterHeight="50" CenterFrameFillColor="Gray"
    ///                                  Foreground="Red" DimmedBrush="LightGray" SegmentWidth="5" 
    ///                                  SegmentSpacing="3"
    ///                                  CharacterSpacing="5" Value="8:50 AM" />
    ///     </Grid>
    /// </Window>
    /// </code>
    /// <code lang="C#">
    /// using System;
    /// using System.Collections.Generic;
    /// using System.Linq;
    /// using System.Text;
    /// using System.Windows;
    /// using System.Windows.Media;
    /// using System.Windows.Controls;
    /// using System.Windows.Data;
    /// using Syncfusion.Windows.Shared;
    /// using Syncfusion.Windows.Gauge;<para/>
    /// namespace CharacterSample
    /// {
    ///         public partial class Window1 : Window
    ///         {
    ///             private DigitalGauge digitalGauge1;            
    ///             public Window1()
    ///             {                
    ///                 InitializeComponent();<para/>
    ///                 digitalGauge1 = new DigitalGauge();
    ///                 digitalGauge1.Height = 100;
    ///                 digitalGauge1.Width = 350;
    ///                 digitalGauge1.CharacterCount = 9;
    ///                 digitalGauge1.CharacterHeight = 50;
    ///                 digitalGauge1.CenterFrameFillColor = Colors.Gray;
    ///                 digitalGauge1.Foreground = new SolidColorBrush(Colors.Red);
    ///                 digitalGauge1.DimmedBrush = new SolidColorBrush(Colors.LightGray);          
    ///                 digitalGauge1.SegmentWidth = 5;
    ///                 digitalGauge1.SegmentSpacing = 3;
    ///                 digitalGauge1.CharacterSpacing = 5;
    ///                 digitalGauge1.Value = "8:50 AM";
    ///                 this.Content = digitalGauge1;
    ///             }
    ///         }
    ///       }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal class CharacterBase : FrameworkElement
    {
        #region Private Members
        /// <summary>
        /// Collection of boolean values indicating what character segments should be 
        /// drawn with foreground brush.
        /// </summary>
        private List<bool> m_listSegments = new List<bool>();
        #endregion Private Members

        #region CLR Getters & Setters
        /// <summary>
        /// Gets or sets the collection of boolean values indicating what character 
        /// segments should be drawn with foreground brush.
        /// </summary>
        /// <value>
        /// Type: <see cref="List{Booliean}"/>
        /// </value>
        internal virtual List<bool> Segments
        {
            get
            {
                return m_listSegments;
            }

            set
            {
                m_listSegments = value;
            }
        }
        #endregion CLR Getters & Setters

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="CharacterHeight"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CharacterHeightChanged;

        /// <summary>
        /// Event that is raised when <see cref="CharactersSpacing"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CharactersSpacingChanged;

        /// <summary>
        /// Event that is raised when <see cref="DimmedBrush"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback DimmedBrushChanged;

        /// <summary>
        /// Event that is raised when <see cref="DrawColon"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback DrawColonChanged;

        /// <summary>
        /// Event that is raised when <see cref="DrawDot"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback DrawDotChanged;

        /// <summary>
        /// Event that is raised when <see cref="SegmentsSpacing"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback SegmentsSpacingChanged;
        #endregion Events

        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="CharacterHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CharacterHeightProperty =
            DependencyProperty.Register("CharacterHeight", typeof(double), typeof(CharacterBase), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(OnCharacterHeightChanged)));

        /// <summary>
        /// Identifies the <see cref="CharactersSpacing"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CharactersSpacingProperty =
            DependencyProperty.Register("CharactersSpacing", typeof(double), typeof(CharacterBase), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(OnCharactersSpacingChanged)));

        /// <summary>
        /// Identifies the <see cref="DimmedBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DimmedBrushProperty =
            DependencyProperty.Register("DimmedBrush", typeof(Brush), typeof(CharacterBase), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Transparent), FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnDimmedBrushChanged)));

        /// <summary>
        /// Identifies the <see cref="DrawColon"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DrawColonProperty =
            DependencyProperty.Register("DrawColon", typeof(bool), typeof(CharacterBase), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnDrawColonChanged)));

        /// <summary>
        /// Identifies the <see cref="DrawDot"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DrawDotProperty =
            DependencyProperty.Register("DrawDot", typeof(bool), typeof(CharacterBase), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnDrawDotChanged)));

        /// <summary>
        /// Identifies the <see cref="ForegroundBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ForegroundBrushProperty =
            DependencyProperty.Register("ForegroundBrush", typeof(Brush), typeof(CharacterBase), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Transparent), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies the <see cref="SegmentsSpacing"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentsSpacingProperty =
            DependencyProperty.Register("SegmentsSpacing", typeof(double), typeof(CharacterBase), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(OnSegmentsSpacingChanged)));

        /// <summary>
        /// Identifies the <see cref="SegmentWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentWidthProperty =
            DependencyProperty.Register("SegmentWidth", typeof(double), typeof(CharacterBase), new FrameworkPropertyMetadata(2d, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        #endregion Dependency Properties

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets the height of the character.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        public double CharacterHeight
        {
            get
            {
                return (double)GetValue(CharacterHeightProperty);
            }

            set
            {
                SetValue(CharacterHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the space between the characters.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        public double CharactersSpacing
        {
            get
            {
                return (double)GetValue(CharactersSpacingProperty);
            }

            set
            {
                SetValue(CharactersSpacingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the dimmed brush of the element.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        public Brush DimmedBrush
        {
            get
            {
                return (Brush)GetValue(DimmedBrushProperty);
            }

            set
            {
                SetValue(DimmedBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to draw the colon.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        public bool DrawColon
        {
            get
            {
                return (bool)GetValue(DrawColonProperty);
            }

            set
            {
                SetValue(DrawColonProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to draw the dot.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        public bool DrawDot
        {
            get
            {
                return (bool)GetValue(DrawDotProperty);
            }

            set
            {
                SetValue(DrawDotProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the foreground brush of the element.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        public Brush ForegroundBrush
        {
            get
            {
                return (Brush)GetValue(ForegroundBrushProperty);
            }

            set
            {
                SetValue(ForegroundBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the space between the segments.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        public double SegmentsSpacing
        {
            get
            {
                return (double)GetValue(SegmentsSpacingProperty);
            }

            set
            {
                SetValue(SegmentsSpacingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the segments.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 2.
        /// </value>
        public double SegmentWidth
        {
            get
            {
                return (double)GetValue(SegmentWidthProperty);
            }

            set
            {
                SetValue(SegmentWidthProperty, value);
            }
        }
        #endregion DP Getters & Setters

        #region Overrides
        /// <summary>
        /// Measures the size in layout required for child elements 
        /// and determines a size for the element.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements.</param>
        /// <returns>The size that this element determines it needs during layout.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(this.CharacterHeight / 2, this.CharacterHeight);
        }
        #endregion Overrides

        #region Implementation
        /// <summary>
        /// Calls OnCharacterHeightChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCharacterHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CharacterBase instance = (CharacterBase)d;
            instance.OnCharacterHeightChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="CharacterHeightChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnCharacterHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CharacterHeightChanged != null)
            {
                this.CharacterHeightChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnCharactersSpacingChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCharactersSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CharacterBase instance = (CharacterBase)d;
            instance.OnCharactersSpacingChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="CharactersSpacingChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnCharactersSpacingChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CharactersSpacingChanged != null)
            {
                this.CharactersSpacingChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="DimmedBrushChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnDimmedBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DimmedBrushChanged != null)
            {
                this.DimmedBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnDimmedBrushChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDimmedBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CharacterBase instance = (CharacterBase)d;
            instance.OnDimmedBrushChanged(e);
        }

        /// <summary>
        /// Calls OnDrawColonChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDrawColonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CharacterBase instance = (CharacterBase)d;
            instance.OnDrawColonChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="DrawColonChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnDrawColonChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DrawColonChanged != null)
            {
                this.DrawColonChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="DrawDotChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnDrawDotChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DrawDotChanged != null)
            {
                this.DrawDotChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnDrawDotChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDrawDotChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CharacterBase instance = (CharacterBase)d;
            instance.OnDrawDotChanged(e);
        }

        /// <summary>
        /// Calls OnSegmentsSpacingChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSegmentsSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CharacterBase instance = (CharacterBase)d;
            instance.OnSegmentsSpacingChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="SegmentsSpacingChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnSegmentsSpacingChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SegmentsSpacingChanged != null)
            {
                this.SegmentsSpacingChanged(this, e);
            }
        }

        #endregion Implementation
    }
}
