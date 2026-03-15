#region Copyright
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
using System.Windows.Media;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the character base class.
    /// </summary>
    /// <example>
    /// <para></para>
    /// <para>using System; </para>
    /// <para></para>
    /// <para>using System.Collections.Generic; </para>
    /// <para></para>
    /// <para>using System.Linq; </para>
    /// <para></para>
    /// <para>using System.Text; </para>
    /// <para></para>
    /// <para>using System.Windows; </para>
    /// <para></para>
    /// <para>using System.Windows.Media; </para>
    /// <para></para>
    /// <para>using System.Windows.Controls; </para>
    /// <para></para>
    /// <para>using System.Windows.Data; </para>
    /// <para></para>
    /// <para>using Syncfusion.Windows.Shared; </para>
    /// <para></para>
    /// <para>using Syncfusion.Windows.Gauge; </para>
    /// <para></para>
    /// <para></para>
    /// <para>namespace CharacterSample </para>
    /// <para></para>
    /// <para>{ </para>
    /// <para></para>
    /// <para>        public partial class Window1 : Window </para>
    /// <para></para>
    /// <para>        { </para>
    /// <para></para>
    /// <para>            private DigitalGauge digitalGauge1; </para>
    /// <para></para>
    /// <para>            public Window1() </para>
    /// <para></para>
    /// <para>            { </para>
    /// <para></para>
    /// <para>                InitializeComponent(); </para>
    /// <para></para>
    /// <para></para>
    /// <para>                digitalGauge1 = new DigitalGauge(); </para>
    /// <para></para>
    /// <para>                digitalGauge1.Height = 100; </para>
    /// <para></para>
    /// <para>                digitalGauge1.Width = 350; </para>
    /// <para></para>
    /// <para>                digitalGauge1.CharacterCount = 9; </para>
    /// <para></para>
    /// <para>                digitalGauge1.CharacterHeight = 50; </para>
    /// <para></para>
    /// <para>                digitalGauge1.CenterFrameFillColor = Colors.Gray; </para>
    /// <para></para>
    /// <para>                digitalGauge1.Foreground = new
    /// SolidColorBrush(Colors.Red); </para>
    /// <para></para>
    /// <para>                digitalGauge1.DimmedBrush = new
    /// SolidColorBrush(Colors.LightGray); </para>
    /// <para></para>
    /// <para>                digitalGauge1.SegmentWidth = 5; </para>
    /// <para></para>
    /// <para>                digitalGauge1.SegmentSpacing = 3; </para>
    /// <para></para>
    /// <para>                digitalGauge1.CharacterSpacing = 5; </para>
    /// <para></para>
    /// <para>                digitalGauge1.Value = &quot;8:50 AM&quot;; </para>
    /// <para></para>
    /// <para>                this.Content = digitalGauge1; </para>
    /// <para></para>
    /// <para>            } </para>
    /// <para></para>
    /// <para>        } </para>
    /// <para></para>
    /// <para>      }</para>
    /// </example>
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

        /// <summary>
        /// Event that is raised when <see cref="SegmentWidth"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback SegmentWidthChanged;
        #endregion Events

        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="CharacterHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CharacterHeightProperty =
            DependencyProperty.Register("CharacterHeight", typeof(double), typeof(CharacterBase), new PropertyMetadata(0d,  new PropertyChangedCallback(OnCharacterHeightChanged)));

        /// <summary>
        /// Identifies the <see cref="CharactersSpacing"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CharactersSpacingProperty =
            DependencyProperty.Register("CharactersSpacing", typeof(double), typeof(CharacterBase), new PropertyMetadata(0d, new PropertyChangedCallback(OnCharactersSpacingChanged)));

        /// <summary>
        /// Identifies the <see cref="DimmedBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DimmedBrushProperty =
            DependencyProperty.Register("DimmedBrush", typeof(Brush), typeof(CharacterBase), new PropertyMetadata(new SolidColorBrush(Colors.Transparent),  new PropertyChangedCallback(OnDimmedBrushChanged)));

        /// <summary>
        /// Identifies the <see cref="DrawColon"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DrawColonProperty =
            DependencyProperty.Register("DrawColon", typeof(bool), typeof(CharacterBase), new PropertyMetadata(false,  new PropertyChangedCallback(OnDrawColonChanged)));

        /// <summary>
        /// Identifies the <see cref="DrawDot"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DrawDotProperty =
            DependencyProperty.Register("DrawDot", typeof(bool), typeof(CharacterBase), new PropertyMetadata(false,  new PropertyChangedCallback(OnDrawDotChanged)));

        /// <summary>
        /// Identifies the <see cref="SegmentBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentBrushProperty =
            DependencyProperty.Register("SegmentBrush", typeof(Brush), typeof(CharacterBase), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));
        //new PropertyChangedCallback(OnSegmentBrushChanged)

        /// <summary>
        /// Identifies the <see cref="SegmentsSpacing"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentsSpacingProperty =
            DependencyProperty.Register("SegmentsSpacing", typeof(double), typeof(CharacterBase), new PropertyMetadata(0d, new PropertyChangedCallback(OnSegmentsSpacingChanged)));

        /// <summary>
        /// Identifies the <see cref="SegmentWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentWidthProperty =
            DependencyProperty.Register("SegmentWidth", typeof(double), typeof(CharacterBase), new PropertyMetadata(2d, new PropertyChangedCallback(OnSegmentWidthChanged)));
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
        public Brush SegmentBrush
        {
            get
            {
                return (Brush)GetValue(SegmentBrushProperty);
            }

            set
            {
                SetValue(SegmentBrushProperty, value);
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

        
        //protected virtual void OnSegmentBrushChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    if (SegmentBrushChanged != null)
        //    {
        //        this.SegmentBrushChanged(this, e);
        //    }
        //}

        
        //private static void OnSegmentBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    CharacterBase instance = (CharacterBase)d;
        //    instance.OnSegmentBrushChanged(e);
        //}

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

        /// <summary>
        /// Updates property value cache and raises <see cref="SegmentWidthChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnSegmentWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SegmentWidthChanged != null)
            {
                this.SegmentWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnSegmentWidthChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSegmentWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CharacterBase instance = (CharacterBase)d;
            instance.OnSegmentWidthChanged(e);
        }
        #endregion Implementation
    }
}
