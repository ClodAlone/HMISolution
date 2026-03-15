// <copyright file="GaugeBase.cs" company="Syncfusion Software">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Contains all the basic brushes, styles and adorner used by the Gauges. 
    /// This class is derived from the <see cref="Control"/> class
    /// </summary>
    /// <remarks>
    /// This class is the base class for all the controls such as <see cref="CircularGauge"/>, <see cref="LinearGauge"/>
    /// and <see cref="DigitalGauge"/>.
    /// </remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [StyleTypedProperty(Property = "StateIndicatorStyle", StyleTargetType = typeof(StateIndicator))]

    public class GaugeBase : Control
    {
        #region Private Members
        /// <summary>
        /// Collection of visual children.
        /// </summary>
        private VisualChildrenCollection<FrameworkElement> children;

        /// <summary>
        /// Border of the Control template
        /// </summary>
        private Border partContainerBorder;

        /// <summary>
        /// Collection of custom labels.
        /// </summary>
        private CustomLabelsCollection customLabels;

        /// <summary>
        /// First frame.
        /// </summary>
        private Border firstCircleBorder;

        /// <summary>
        /// Adorner used to display child elements.
        /// </summary>
        private GaugeAdorner gaugeAdorner;

        /// <summary>
        /// Collection of images.
        /// </summary>
        private ImagesCollection images;

        /// <summary>
        /// Inner frame.
        /// </summary>
        private Border innerCircleBorder;

        /// <summary>
        /// Second frame.
        /// </summary>
        private Border secondCircleBorder;

        /// <summary>
        /// Collection of state indicators.
        /// </summary>
        private StateIndicatorsCollection stateIndicators;

        /// <summary>
        /// ZIndex to Set for LocalizableGaugeElements
        /// </summary>
        private int zindex = 0;

        /// <summary>
        /// Stack to store the Operators.
        /// </summary>
        private static Stack<string> operatorStack = new Stack<string>();

        /// <summary>
        /// Stack to store the Operands.
        /// </summary>
        private static Stack<double> varStack = new Stack<double>();

        /// <summary>
        /// Stores the calculated postfix expression.
        /// </summary>
        private static string postfix = null;
        #endregion Private Members

        #region CLR Getters & Setters

        /// <summary>
        /// Gets or sets the GaugeAdorner.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GaugeAdorner GaugeAdorner
        {
            get
            {
                return gaugeAdorner;
            }

            set
            {
                gaugeAdorner = value;
            }
        }

        /// <summary>
        /// Gets or sets the collection of visual children.
        /// </summary>
        /// <value>
        /// Type: <see cref="VisualChildrenCollection{FrameworkElement}"/>
        /// </value>
        /// <seealso cref="VisualChildrenCollection{FrameworkElement}"/>
        internal VisualChildrenCollection<FrameworkElement> ChildrenCollection
        {
            get
            {
                return children;
            }

            set
            {
                children = value;
            }
        }

        /// <summary>
        /// Gets the Border element.
        /// </summary>
        internal Border PART_ContainerBorder
        {
            get
            {
                return partContainerBorder;
            }
        }

        /// <summary>
        /// Gets or sets the collection of custom labels.
        /// </summary>
        /// <value>
        /// Type: <see cref="CustomLabelsCollection"/>
        /// </value>
        /// <seealso cref="CustomLabelsCollection"/>
        public CustomLabelsCollection CustomLabels
        {
            get
            {
                return customLabels;
            }

            set
            {
                customLabels = value;
            }
        }

        /// <summary>
        /// Gets the border of the First circle
        /// </summary>
        /// <value>
        /// Type: <see cref="Border"/>
        /// </value>
        /// <seealso cref="Border"/>
        internal Border FirstCircleBorder
        {
            get
            {
                return firstCircleBorder;
            }
        }

        /// <summary>
        /// Gets the Visual brush which of gauge visual
        /// </summary>
        internal VisualBrush GaugeVisualBrush
        {
            get
            {
                DrawingVisual drawingVisual = new DrawingVisual();
                using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                {
                    VisualBrush visualBrush = new VisualBrush(this);
                    drawingContext.DrawRectangle(visualBrush, null, new Rect(new Point(0, 0), this.RenderSize));

                    if (GaugeAdorner != null)
                    {
                        visualBrush = new VisualBrush(GaugeAdorner);
                        drawingContext.DrawRectangle(visualBrush, null, new Rect(new Point(0, 0), GaugeAdorner.RenderSize));
                    }
                }

                return new VisualBrush(drawingVisual);
            }
        }

        /// <summary>
        /// Gets or sets the collection of images.
        /// </summary>
        /// <value>
        /// Type: <see cref="ImagesCollection"/>
        /// </value>
        /// <seealso cref="ImagesCollection"/>
        public ImagesCollection Images
        {
            get
            {
                return images;
            }

            set
            {
                images = value;
            }
        }

        /// <summary>
        /// Gets the border element for the inner circle
        /// </summary>
        /// <value>
        /// Type: <see cref="Border"/>
        /// </value>
        /// <seealso cref="Border"/>
        internal Border InnerCircleBorder
        {
            get
            {
                return innerCircleBorder;
            }
        }

        /// <summary>
        /// Gets the border element for the second circle
        /// </summary>
        /// <value>
        /// Type: <see cref="Border"/>
        /// </value>
        /// <seealso cref="Border"/>
        internal Border SecondCircleBorder
        {
            get
            {
                return secondCircleBorder;
            }
        }

        /// <summary>
        /// Gets or sets the collection of state indicators.
        /// </summary>
        /// <value>
        /// Type: <see cref="StateIndicatorsCollection"/>
        /// </value>
        /// <seealso cref="StateIndicatorsCollection"/>
        public StateIndicatorsCollection StateIndicators
        {
            get
            {
                return stateIndicators;
            }

            set
            {
                stateIndicators = value;
            }
        }
        #endregion CLR Getters & Setters

        #region Events
        /// <summary>
        /// Event that is raised when AdornerStyle property is changed.
        /// </summary>
        public event PropertyChangedCallback AdornerStyleChanged;

        /// <summary>
        /// Event that is raised when ApplyFrameStyles property is changed.
        /// </summary>
        public event PropertyChangedCallback ApplyFrameStylesChanged;

        /// <summary>
        /// Event that is raised when <see cref="FirstFrameThickness"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback FirstFrameThicknessChanged;

        /// <summary>
        /// Event that is raised when <see cref="SecondFrameThickness"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback SecondFrameThicknessChanged;

        /// <summary>
        /// Event that is raised when <see cref="IsColorMergeWithBase"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IsColorMergeWithBaseChanged;

        /// <summary>
        /// Event that is raised when <see cref="EnableEffects"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback EnableEffectsChanged;
        #endregion Events

        #region Dependency Properties

        /// <summary>
        ///  Identifies the <see cref="StateIndicatorStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StateIndicatorStyleProperty =
            DependencyProperty.Register("StateIndicatorStyle", typeof(Style), typeof(CircularGauge), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="AdornerStyle"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty AdornerStyleProperty =
            DependencyProperty.Register("AdornerStyle", typeof(Style), typeof(GaugeBase), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnAdornerStyleChanged)));

        /// <summary>
        /// Identifies the <see cref="ApplyFrameStyles"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ApplyFrameStylesProperty =
            DependencyProperty.Register("ApplyFrameStyles", typeof(bool), typeof(GaugeBase), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnApplyFrameStylesChanged)));

        /// <summary>
        /// Identifies the <see cref="CenterFrameFillColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterFrameFillColorProperty =
            DependencyProperty.Register("CenterFrameFillColor", typeof(Brush), typeof(GaugeBase), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCenterFrameFillColorChanged)));

        /// <summary>
        /// Identifies the <see cref="CornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(GaugeBase), new FrameworkPropertyMetadata(new CornerRadius(0), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies the <see cref="FirstFrameBrush"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty FirstFrameBrushProperty =
            DependencyProperty.Register("FirstFrameBrush", typeof(Brush), typeof(GaugeBase), new FrameworkPropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Identifies the <see cref="CenterFrameFillColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FirstFrameFillColorProperty =
            DependencyProperty.Register("FirstFrameFillColor", typeof(Brush), typeof(GaugeBase), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnFirstFrameFillColorChanged)));

        /// <summary>
        /// Identifies the <see cref="FirstFrameStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FirstFrameStyleProperty =
            DependencyProperty.Register("FirstFrameStyle", typeof(Style), typeof(GaugeBase), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="InnerFrameBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FirstFrameThicknessProperty =
            DependencyProperty.Register("FirstFrameThickness", typeof(Thickness), typeof(GaugeBase), new FrameworkPropertyMetadata(new Thickness(17), new PropertyChangedCallback(OnFirstFrameThicknessChanged)));

        /// <summary>
        /// Identifies the <see cref="InnerFrameBrush"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty InnerFrameBrushProperty =
            DependencyProperty.Register("InnerFrameBrush", typeof(Brush), typeof(GaugeBase), new FrameworkPropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Identifies the <see cref="InnerFrameContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerFrameContentProperty =
           DependencyProperty.Register("InnerFrameContent", typeof(object), typeof(GaugeBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnInnerFrameContentChanged)));

        /// <summary>
        /// Identifies the <see cref="InnerFrameStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerFrameStyleProperty =
            DependencyProperty.Register("InnerFrameStyle", typeof(Style), typeof(GaugeBase), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="SecondFrameBrush"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty SecondFrameBrushProperty =
            DependencyProperty.Register("SecondFrameBrush", typeof(Brush), typeof(GaugeBase), new FrameworkPropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Identifies the <see cref="CenterFrameFillColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SecondFrameFillColorProperty =
            DependencyProperty.Register("SecondFrameFillColor", typeof(Brush), typeof(GaugeBase), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSecondFrameFillColorChanged)));

        /// <summary>
        /// Identifies the <see cref="SecondFrameStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SecondFrameStyleProperty =
            DependencyProperty.Register("SecondFrameStyle", typeof(Style), typeof(GaugeBase), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="InnerFrameBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SecondFrameThicknessProperty =
            DependencyProperty.Register("SecondFrameThickness", typeof(Thickness), typeof(GaugeBase), new FrameworkPropertyMetadata(new Thickness(7), new PropertyChangedCallback(OnSecondFrameThicknessChanged)));

        /// <summary>
        /// Identifies the <see cref="SizeToContainer"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SizeToContainerProperty =
            DependencyProperty.Register("SizeToContainer", typeof(bool), typeof(GaugeBase), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies the <see cref="IsColorMergeWithBase"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsColorMergeWithBaseProperty =
            DependencyProperty.Register("IsColorMergeWithBase", typeof(bool), typeof(GaugeBase), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnIsColorMergeWithBaseChanged)));
        /// <summary>
        /// Identifies the <see cref="EnableEffects"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableEffectsProperty =
            DependencyProperty.Register("EnableEffects", typeof(bool), typeof(GaugeBase), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnEnableEffectsChanged)));

        #endregion Dependency Properties

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets the adorner style.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Style"/>
        /// </value>
        internal Style AdornerStyle
        {
            get
            {
                return (Style)GetValue(AdornerStyleProperty);
            }

            set
            {
                SetValue(AdornerStyleProperty, value);
            }
        }

        /// <summary>
        /// StateIndicatorStyle
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        public Style StateIndicatorStyle
        {
            get { return (Style)GetValue(StateIndicatorStyleProperty); }
            set { SetValue(StateIndicatorStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to apply custom styles.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        [Bindable(true)]
        [Category("Appearance")]
        public bool ApplyFrameStyles
        {
            get
            {
                return (bool)GetValue(ApplyFrameStylesProperty);
            }

            set
            {
                SetValue(ApplyFrameStylesProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color used for drawing the inner frame.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Color"/>
        /// Default value is Color.FromRgb( 69, 129, 239 ).
        /// </value>
        /// <seealso cref="FirstFrameFillColor"/>
        [Bindable(true)]
        [Category("Brushes")]
        public Brush CenterFrameFillColor
        {
            get
            {
                return (Brush)GetValue(CenterFrameFillColorProperty);
            }

            set
            {
                SetValue(CenterFrameFillColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the gauge's corner radius .
        /// This is a dependency property.
        /// </summary>
        [Bindable(true)]
        [Category("Behaviors")]
        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }

            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the initial FrameType brush value for FirstFrame
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        internal Brush FirstFrameBrush
        {
            get
            {
                return (Brush)GetValue(FirstFrameBrushProperty);
            }

            set
            {
                SetValue(FirstFrameBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color used for drawing the outer frame.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Color"/>
        /// Default value is Color.FromRgb( 69, 129, 239 ).
        /// </value>
        /// <seealso cref="SecondFrameFillColor"/>
        [Bindable(true)]
        [Category("Brushes")]
        public Brush FirstFrameFillColor
        {
            get
            {
                return (Brush)GetValue(FirstFrameFillColorProperty);
            }

            set
            {
                SetValue(FirstFrameFillColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style of the first frame.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Style"/>
        /// </value>
        [Bindable(true)]
        [Category("Appearance")]
        public Style FirstFrameStyle
        {
            get
            {
                return (Style)GetValue(FirstFrameStyleProperty);
            }

            set
            {
                SetValue(FirstFrameStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the first frame thickness of the gauge.
        /// This is a dependency property.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        public Thickness FirstFrameThickness
        {
            get
            {
                return (Thickness)GetValue(FirstFrameThicknessProperty);
            }

            set
            {
                SetValue(FirstFrameThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the initial FrameType brush value for InnerFrame.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        internal Brush InnerFrameBrush
        {
            get
            {
                return (Brush)GetValue(InnerFrameBrushProperty);
            }

            set
            {
                SetValue(InnerFrameBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the the inner frame content to host any content inside the gauge. 
        /// This content will be added as inner frame content.
        /// </summary>
        /// <value>
        /// Type: <see cref="object"/>
        /// Default value is null.
        /// </value>
        [Bindable(true)]
        [Category("Content")]
        public object InnerFrameContent
        {
            get
            {
                return (object)GetValue(InnerFrameContentProperty);
            }

            set
            {
                SetValue(InnerFrameContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style of the inner frame.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Style"/>
        /// </value>
        [Bindable(true)]
        [Category("Appearance")]
        public Style InnerFrameStyle
        {
            get
            {
                return (Style)GetValue(InnerFrameStyleProperty);
            }

            set
            {
                SetValue(InnerFrameStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the initial FrameType brush value for Secondframe
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        internal Brush SecondFrameBrush
        {
            get
            {
                return (Brush)GetValue(SecondFrameBrushProperty);
            }

            set
            {
                SetValue(SecondFrameBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color used for drawing of SecondFrame color.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Color"/>
        /// Default value is Color.FromRgb( 69, 129, 239 ).
        /// </value>
        /// <seealso cref="CenterFrameFillColor"/>
        [Bindable(true)]
        [Category("Brushes")]
        public Brush SecondFrameFillColor
        {
            get
            {
                return (Brush)GetValue(SecondFrameFillColorProperty);
            }

            set
            {
                SetValue(SecondFrameFillColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style of the second frame.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Style"/>
        /// </value>
        [Bindable(true)]
        [Category("Appearance")]
        public Style SecondFrameStyle
        {
            get
            {
                return (Style)GetValue(SecondFrameStyleProperty);
            }

            set
            {
                SetValue(SecondFrameStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the second frame thickness of the gauge.
        /// This is a dependency property.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        public Thickness SecondFrameThickness
        {
            get
            {
                return (Thickness)GetValue(SecondFrameThicknessProperty);
            }

            set
            {
                SetValue(SecondFrameThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether gauge control should fit within its container.
        /// Default is false.
        /// This is a dependency property.
        /// </summary>
        [Bindable(true)]
        [Category("Behaviors")]
        public bool SizeToContainer
        {
            get
            {
                return (bool)GetValue(SizeToContainerProperty);
            }

            set
            {
                SetValue(SizeToContainerProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the colors applied to the frame fill colors should be merged.
        /// Default is true.
        /// This is a dependency property.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        public bool IsColorMergeWithBase
        {
            get
            {
                return (bool)GetValue(IsColorMergeWithBaseProperty);
            }

            set
            {
                SetValue(IsColorMergeWithBaseProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the colors applied to the frame fill colors should be merged.
        /// Default is true.
        /// This is a dependency property.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        public bool EnableEffects
        {
            get
            {
                return (bool)GetValue(EnableEffectsProperty);
            }

            set
            {
                SetValue(EnableEffectsProperty, value);
            }
        }
        #endregion DP Getters & Setters

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="GaugeBase"/> class.
        /// Overrides some dependency properties.
        /// </summary>
        static GaugeBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GaugeBase), new FrameworkPropertyMetadata(typeof(GaugeBase)));
            CursorProperty.OverrideMetadata(typeof(CircularGauge), new FrameworkPropertyMetadata(OnCursorChanged));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GaugeBase"/> class.
        /// </summary>
        public GaugeBase()
        {
            this.ChildrenCollection = new VisualChildrenCollection<FrameworkElement>(this);
            this.StateIndicators = new StateIndicatorsCollection();
            this.CustomLabels = new CustomLabelsCollection();
            this.Images = new ImagesCollection();
            this.StateIndicators.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            this.CustomLabels.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            this.Images.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
        }
        #endregion Initialization

        #region Static Methods
        /// <summary>
        /// Calculates the label value for the given formula.
        /// </summary>
        /// <param name="label">The current label value.</param>
        /// <param name="formula">The formula to be acted upon the label.</param>
        /// <returns>String value of the new label</returns>
        internal static string CalculateLabel(string label, string formula)
        {
            string newFormula = string.Empty;
            int count = 0;

            if (formula == "x")
            {
                if (label[0] == '!')
                {
                    label = label.Remove(0, 1);
                    label = "-" + label;
                }

                return label;
            }

            if (formula == "!x")
            {
                if (label[0] == '!')
                {
                    label = label.Remove(0, 1);
                    return label;
                }

                double d = Convert.ToDouble(label);
                d = -d;
                return d.ToString();
            }

            for (int i = 0; i < formula.Length; i++)
            {
                if (formula[i] == '(')
                {
                    count++;
                }
                else if (formula[i] == ')')
                {
                    count--;
                }
            }

            if (count != 0)
            {
                label = "() ERR";
                return label;
            }

            if (formula.IndexOf("x") != -1)
            {
                formula = formula.Replace("x", label);
            }
            else
            {
                label = "x !PRESENT";
                return label;
            }

            if (formula.IndexOf("!!") != -1)
            {
                formula = formula.Replace("!!", string.Empty);
            }

            string[] splitted = formula.Split(' ');

            foreach (string s in splitted)
            {
                newFormula = newFormula + s;
            }

            GaugeBase.InfixToPostfix(newFormula);
            GaugeBase.Evaluate(GaugeBase.postfix);
            return varStack.Pop().ToString();
        }

        /// <summary>
        /// Converts Infix notation to postfix notation.
        /// </summary>
        /// <param name="input">The input which is the Infix notation.</param>
        internal static void InfixToPostfix(string input)
        {
            int firstIndex;
            operatorStack.Clear();
            varStack.Clear();
            postfix = string.Empty;
            char[] operators = { '*', '/', '+', '-', '%', '(', ')', '!' };
            firstIndex = input.IndexOfAny(operators);
            while (firstIndex != -1)
            {
                string var = string.Empty;
                if (input[0] != '(' && input[0] != ')')
                {
                    var = input.Substring(0, firstIndex);
                }
                else
                {
                    if (input[0] == '(')
                    {
                        operatorStack.Push("(");
                    }
                    else
                    {
                        // if ")" is the operator
                        while (operatorStack.Count > 0)
                        {
                            if (operatorStack.Peek() == "(")
                            {
                                operatorStack.Pop();
                                break;
                            }

                            postfix = postfix + operatorStack.Pop();
                        }
                    }

                    input = input.Remove(0, 1);
                    firstIndex = input.IndexOfAny(operators);
                    continue;
                }

                // Added to postfix with a seperator string
                postfix = postfix + "|" + var;

                if (operatorStack.Count >= 0)
                {
                    string currentOp = input[firstIndex].ToString();
                    if (currentOp == ")" || currentOp == "(")
                    {
                        input = input.Remove(0, firstIndex);
                        firstIndex = input.IndexOfAny(operators);
                        continue;
                    }

                    if (operatorStack.Count == 0)
                    {
                        operatorStack.Push(currentOp);
                    }
                    else
                    {
                        while (operatorStack.Count > 0 && (currentOp == "+" || currentOp == "-") && (operatorStack.Peek() == "*" || operatorStack.Peek() == "/" || operatorStack.Peek() == "%" || operatorStack.Peek() == "!"))
                        {
                            postfix = postfix + operatorStack.Pop();
                        }

                        while (operatorStack.Count > 0 && (currentOp == "*" || currentOp == "/" || currentOp == "%") && operatorStack.Peek() == "!")
                        {
                            postfix = postfix + operatorStack.Pop();
                        }

                        operatorStack.Push(currentOp);
                    }
                }

                input = input.Remove(0, firstIndex + 1);
                firstIndex = input.IndexOfAny(operators);
            }

            postfix = postfix + "|" + input;

            while (operatorStack.Count > 0)
            {
                postfix = postfix + operatorStack.Pop();
            }
        }

        /// <summary>
        /// Evaluates the input in postfix notation.
        /// </summary>
        /// <param name="input">The input in postfix notation.</param>
        internal static void Evaluate(string input)
        {
            int firstIndex;
            char[] operators = { '|', '*', '/', '%', '+', '-', '!' };
            firstIndex = input.IndexOfAny(operators);
            while (firstIndex != -1)
            {
                if (input[0] == '|')
                {
                    input = input.Remove(0, 1);
                }
                else
                {
                    bool isNaN = false;
                    string var = input.Substring(0, firstIndex);
                    if (var == string.Empty)
                    {
                        var = input[0].ToString();
                    }

                    foreach (char ch in operators)
                    {
                        if (var[0] == ch)
                        {
                            isNaN = true;
                            break;
                        }
                    }

                    // If a number
                    if (isNaN == false)
                    {
                        varStack.Push(Convert.ToDouble(var));
                        input = input.Remove(0, firstIndex);
                    }
                    else
                    {
                        if (var != string.Empty && var[0] != '|')
                        {
                            double temp = varStack.Pop();
                            double topStack = 0;

                            if (var[0] != '!')
                            {
                                topStack = varStack.Pop();
                            }

                            // Find the operator and Evaluate
                            switch (var)
                            {
                                case "+":
                                    varStack.Push(topStack + temp);
                                    break;

                                case "-":
                                    varStack.Push(topStack - temp);
                                    break;

                                case "*":
                                    varStack.Push(topStack * temp);
                                    break;

                                case "/":
                                    varStack.Push(topStack / temp);
                                    break;

                                case "%":
                                    varStack.Push(topStack % temp);
                                    break;

                                case "!":
                                    varStack.Push(-temp);
                                    break;
                            }

                            input = input.Remove(0, 1);
                        }
                        else
                        {
                            input = input.Remove(0, 1);
                        }
                    }
                }

                firstIndex = input.IndexOfAny(operators);
            }
        }
        #endregion Static Methods

        #region Overrides
        /// <summary>
        /// Raises the <see cref="System.Windows.FrameworkElement.Initialized"/> event. 
        /// This method is invoked whenever <see cref="System.Windows.FrameworkElement.IsInitialized"/> property 
        /// is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.Loaded += new RoutedEventHandler(GaugeBaseLoaded);
        }

        /// <summary>
        /// Builds the current template's visual tree if necessary.
        /// </summary>
        public override void OnApplyTemplate()
        {
            partContainerBorder = this.GetTemplateChild("PART_ContainerBorder") as Border;

            if (partContainerBorder != null)
            {
                GaugeAdorner = new GaugeAdorner(partContainerBorder);
            }
            else
            {
                GaugeAdorner = new GaugeAdorner(this);
            }

            firstCircleBorder = this.Template.FindName("FirstCircleBorder", this) as Border;
            if (firstCircleBorder != null)
            {
                firstCircleBorder.Style = this.FirstFrameStyle;
            }

            secondCircleBorder = this.Template.FindName("SecondCircleBorder", this) as Border;
            if (secondCircleBorder != null)
            {
                secondCircleBorder.Style = this.SecondFrameStyle;
            }

            innerCircleBorder = this.Template.FindName("InnerCircleBorder", this) as Border;
            if (innerCircleBorder != null)
            {
                innerCircleBorder.Style = this.InnerFrameStyle;
            }

            base.OnApplyTemplate();
        }

        /// <summary>
        /// Called to arrange and size the content of a control.
        /// </summary>
        /// <param name="arrangeBounds">The computed size that is used to arrange the content.</param>
        /// <returns>The size of the control.</returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            this.UpdateChildrenLocation();
            return base.ArrangeOverride(arrangeBounds);
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.FrameworkElement.SizeChanged"/> event, 
        /// using the specified information as part of the eventual event data. 
        /// </summary>
        /// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (GaugeAdorner != null)
            {
                GaugeAdorner.AdornerWidth = this.ActualWidth;
                GaugeAdorner.AdornerHeight = this.ActualHeight;

                GaugeAdorner.GaugeParent = this;
            }

            int count = this.ChildrenCollection.Count;
            for (int i = 0; i < count; i++)
            {
                if (this.ChildrenCollection[i] is ScaleBase)
                {
                    ScaleBase scale = this.ChildrenCollection[i] as ScaleBase;

                    scale.InvalidateVisual();
                }
            }
            if (this is RollingGauge)
            {
                (this as RollingGauge)._Initialized = false;
                (this as RollingGauge).AddSegments();
            }
            else
            {
                this.UpdateChildrenLocation();
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseWheel"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseWheelEventArgs"/> that contains the event data.</param>
        protected override void OnMouseWheel(System.Windows.Input.MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            int count = this.ChildrenCollection.Count;
            for (int i = 0; i < count; i++)
            {
                if (this.ChildrenCollection[i] is CircularScale)
                {
                    CircularScale scale = this.ChildrenCollection[i] as CircularScale;
                    foreach (CircularPointer pointer in scale.Pointers)
                    {
                        if (pointer.IsFocused)
                        {
                            if (e.Delta > scale.Minimum)
                                pointer.Value--;

                            if (e.Delta < scale.Maximum)
                                pointer.Value++;
                        }
                    }
                }
                else if (this.ChildrenCollection[i] is LinearScale)
                {
                    LinearScale scale = this.ChildrenCollection[i] as LinearScale;
                    foreach (LinearPointer pointer in scale.Pointers)
                    {
                        if (pointer.IsFocused)
                        {
                            if (e.Delta > scale.Minimum)
                                pointer.Value--;

                            if (e.Delta < scale.Maximum)
                                pointer.Value++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (this.StateIndicatorStyle != null)
                foreach (StateIndicator stateIndicator in this.StateIndicators)
                    stateIndicator.Style = this.StateIndicatorStyle;
            base.OnRender(drawingContext);
        }

        #endregion Overrides

        #region ColorMerger
        /// <summary>
        /// Returns skined color from Color value.
        /// </summary>
        /// <param name="baseColor">BaseColor to be merged</param>
        /// <param name="skinColor">SkinColor to be merged</param>
        /// <returns>Returns merged color </returns>
        internal Color GetColor(Color baseColor, Color skinColor)
        {
            if (baseColor.A == 0)
            {
                return baseColor;
            }

            return Color.FromArgb(baseColor.A, MergeChannels(baseColor.R, skinColor.R), MergeChannels(baseColor.G, skinColor.G), MergeChannels(baseColor.B, skinColor.B));
        }

        /// <summary>
        /// Returns a merged brush from Color and brush value.
        /// </summary>
        /// <param name="oldbrush">Brush to be merged</param>
        /// <param name="newcolor">Color to be merged</param>
        /// <returns>Returns merged brush</returns>
        internal Brush GetMergedSolidandLinearBrush(Brush oldbrush, Color newcolor)
        {
            if (oldbrush is LinearGradientBrush)
            {
                LinearGradientBrush brush = oldbrush as LinearGradientBrush;
                LinearGradientBrush newBrush = new LinearGradientBrush();

                newBrush.StartPoint = brush.StartPoint;
                newBrush.EndPoint = brush.EndPoint;
                newBrush.Transform = brush.Transform;

                foreach (GradientStop stop in brush.GradientStops)
                {
                    GradientStop newStop = new GradientStop(this.GetColor(stop.Color, newcolor), stop.Offset);
                    newBrush.GradientStops.Add(newStop);
                }

                return newBrush;
            }
            else if (oldbrush is SolidColorBrush)
            {
                SolidColorBrush newBrush = new SolidColorBrush();
                newBrush.Color = this.GetColor((oldbrush as SolidColorBrush).Color, newcolor);
                return newBrush;
            }
            else if (oldbrush is RadialGradientBrush)
            {
                RadialGradientBrush brush = oldbrush as RadialGradientBrush;
                RadialGradientBrush newBrush = new RadialGradientBrush();

                newBrush.Center = brush.Center;
                newBrush.GradientOrigin = brush.GradientOrigin;
                newBrush.RadiusX = brush.RadiusX;
                newBrush.RadiusY = brush.RadiusY;

                foreach (GradientStop stop in brush.GradientStops)
                {
                    GradientStop newStop = new GradientStop(this.GetColor(stop.Color, newcolor), stop.Offset);
                    newBrush.GradientStops.Add(newStop);
                }

                return newBrush;
            }

            return oldbrush;
        }

        /// <summary>
        /// Merges two color channels.
        /// </summary>
        /// <param name="baseChannel">BaseChannel to merge.</param>
        /// <param name="blendChannel">BlendChannel to merge.</param>
        /// <returns>Merged channels.</returns>
        internal byte MergeChannels(int baseChannel, int blendChannel)
        {
            int mediana, dif, rest, max = 255;
            if (baseChannel < 24)
            {
                baseChannel = 24;
            }

            mediana = baseChannel * blendChannel / max;
            dif = ((max - baseChannel) * (max - blendChannel)) / max;
            rest = baseChannel * (max - dif - mediana);

            return (byte)(mediana + (rest / 255));
        }

        /// <summary>
        /// Merges two colors.
        /// </summary>
        /// <param name="color">Color to merge.</param>
        /// <param name="blendColor">BlendColor to merge.</param>
        /// <returns>Merged colors.</returns>
        internal Color MergeChannels(Color color, Color blendColor)
        {
            return Color.FromArgb(color.A, this.MergeChannels(color.R, blendColor.R), this.MergeChannels(color.G, blendColor.G), this.MergeChannels(color.B, blendColor.B));
        }

        /// <summary>
        /// Merges the color.
        /// </summary>
        /// <param name="brushvalue">The brushvalue.</param>
        /// <param name="newcolor">The newcolor.</param>
        /// <returns>Returns Merged Brush</returns>
        internal Brush MergeColor(Brush brushvalue, Color newcolor)
        {
            if (brushvalue is DrawingBrush)
            {
                DrawingBrush drawingBrush = brushvalue as DrawingBrush;
                DrawingBrush newdBrush = new DrawingBrush();

                newdBrush.AlignmentX = drawingBrush.AlignmentX;
                newdBrush.AlignmentY = drawingBrush.AlignmentY;
                newdBrush.Stretch = drawingBrush.Stretch;
                newdBrush.TileMode = drawingBrush.TileMode;
                newdBrush.Viewbox = drawingBrush.Viewbox;
                newdBrush.ViewboxUnits = drawingBrush.ViewboxUnits;

                if (drawingBrush.Drawing is DrawingGroup)
                {
                    DrawingGroup dgroup = drawingBrush.Drawing as DrawingGroup;
                    DrawingGroup newdGroup = new DrawingGroup();

                    foreach (Drawing drawing in dgroup.Children)
                    {
                        GeometryDrawing newgeometry = new GeometryDrawing();
                        if (drawing is GeometryDrawing)
                        {
                            newgeometry.Geometry = (drawing as GeometryDrawing).Geometry;
                            newgeometry.Brush = GetMergedSolidandLinearBrush((drawing as GeometryDrawing).Brush, newcolor);
                            newgeometry.Pen = (drawing as GeometryDrawing).Pen;
                        }

                        if (newgeometry != null)
                        {
                            newdGroup.Children.Add(newgeometry);
                        }
                    }

                    newdBrush.Drawing = newdGroup;
                }

                return newdBrush;
            }
            else
            {
                return GetMergedSolidandLinearBrush(brushvalue, newcolor);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Invoked when <see cref="LocalizableGaugeElement.LocationProperty"/> property of a child is changed.
        /// </summary>
        /// <param name="sender">The <see cref="CircularGauge"/> object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected void ChildLocationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            this.UpdateChildrenLocation();
        }

        /// <summary>
        /// Occurs when an item is added, removed, changed, moved, or the entire collection is refreshed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> that contains the event data.</param>
        protected virtual void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                // int count = e.NewItems.Count;
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        if (e.NewItems[i] is ScaleBase)
                        {
                            ScaleBase elem = e.NewItems[i] as ScaleBase;
                            if (elem != null)
                            {
                                this.ChildrenCollection.Insert(0, elem);
                                elem.LocationChanged += new PropertyChangedCallback(ChildLocationChanged);
                            }
                        }
                        else if (e.NewItems[i] is LocalizableGaugeElement)
                        {
                            zindex = e.NewItems[i] is GaugeImage ? -2 : -1;
                            LocalizableGaugeElement elem = e.NewItems[i] as LocalizableGaugeElement;
                            if (elem != null)
                            {
                                int index = this.ChildrenCollection.Count > 0 ? this.ChildrenCollection.Count : 0;
                                elem.SetValue(Panel.ZIndexProperty, zindex);
                                this.ChildrenCollection.Insert(0, elem);
                                elem.LocationChanged += new PropertyChangedCallback(ChildLocationChanged);
                            }
                        }
                        else
                        {
                            FrameworkElement elem = e.NewItems[i] as FrameworkElement;
                            if (elem != null)
                            {
                                this.ChildrenCollection.Insert(0, elem);
                            }
                        }
                    }
                }
            }
            else if (e.OldItems != null)
            {
                // int count = e.OldItems.Count;
                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                        FrameworkElement elem = e.OldItems[i] as FrameworkElement;
                        if (elem != null && this.ChildrenCollection.Contains(elem))
                        {
                            this.ChildrenCollection.Remove(elem);
                        }
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (this is DigitalGauge)
                    this.ChildrenCollection.Clear();
            }
        }

        /// <summary>
        /// Refreshes the Adorner layer
        /// </summary>
        protected virtual void RefreshAdornerLayer()
        {
            AdornerLayer adornerLayer;
            if (partContainerBorder != null)
            {
                adornerLayer = AdornerLayer.GetAdornerLayer(partContainerBorder);
                if (adornerLayer != null && GaugeAdorner != null)
                {
                    GaugeAdorner.SetStyle(this.AdornerStyle);
                    if (adornerLayer.GetAdorners(partContainerBorder) == null || (adornerLayer.GetAdorners(partContainerBorder) != null && !adornerLayer.GetAdorners(partContainerBorder).Contains(GaugeAdorner)))
                    {
                        adornerLayer.Add(GaugeAdorner);
                    }
                }
            }
            else
            {
                adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null && GaugeAdorner != null)
                {
                    GaugeAdorner.SetStyle(this.AdornerStyle);
                    if (adornerLayer.GetAdorners(this) == null || (adornerLayer.GetAdorners(this) != null && !adornerLayer.GetAdorners(this).Contains(GaugeAdorner)))
                    {
                        adornerLayer.Add(GaugeAdorner);
                    }
                }
            }

            if (GaugeAdorner != null)
            {
                GaugeAdorner.AdornerWidth = this.ActualWidth;
                GaugeAdorner.AdornerHeight = this.ActualHeight;
            }
        }

        /// <summary>
        /// Updates gauge frames.
        /// </summary>
        protected virtual void RefreshBorders()
        {
            if (!this.ApplyFrameStyles)
            {
                if (this.FirstCircleBorder != null)
                {
                    Path path = this.Template.FindName("FirstPath", this) as Path;
                    if (FirstFrameFillColor != null)
                    {
                        if (path == null)
                        {
                            if (IsColorMergeWithBase)
                            {
                                //Added for VS2010 theme                               
                                if (FirstFrameFillColor is SolidColorBrush && FirstFrameFillColor != Brushes.Transparent && SkinStorage.GetVisualStyle(this).ToString() != "VS2010")                                
                                {
                                    this.FirstCircleBorder.Background = MergeColor(FirstFrameBrush, (FirstFrameFillColor as SolidColorBrush).Color);
                                }
                                else
                                {
                                    this.FirstCircleBorder.Background = FirstFrameFillColor;
                                }
                            }
                            else
                            {
                                this.FirstCircleBorder.Background = FirstFrameFillColor;
                            }
                        }
                        else
                        {
                            if (IsColorMergeWithBase)
                            {
                                if (FirstFrameFillColor is SolidColorBrush)
                                {
                                    path.Fill = MergeColor(FirstFrameBrush, (FirstFrameFillColor as SolidColorBrush).Color);
                                }
                            }
                            else
                            {
                                path.Fill = FirstFrameFillColor;
                            }
                        }
                    }
                    else
                    {
                        if (path == null)
                        {
                            this.FirstCircleBorder.Background = FirstFrameBrush;
                        }
                    }

                    this.FirstCircleBorder.OpacityMask = this.OpacityMask;
                    this.FirstCircleBorder.Opacity = this.Opacity;
                }

                if (this.SecondCircleBorder != null)
                {
                    Path path = this.Template.FindName("SecondPath", this) as Path;
                    if (SecondFrameFillColor != null)
                    {
                        if (path == null)
                        {
                            if (IsColorMergeWithBase)
                            {
                                if (SecondFrameFillColor is SolidColorBrush && SecondFrameFillColor != Brushes.Transparent)
                                {
                                    this.SecondCircleBorder.Background = MergeColor(SecondFrameBrush, (this.SecondFrameFillColor as SolidColorBrush).Color);
                                }
                                else
                                {
                                    this.SecondCircleBorder.Background = this.SecondFrameFillColor;
                                }
                            }
                            else
                            {
                                this.SecondCircleBorder.Background = this.SecondFrameFillColor;
                            }
                        }
                        else
                        {
                            if (IsColorMergeWithBase)
                            {
                                if (SecondFrameFillColor is SolidColorBrush)
                                {
                                    path.Fill = MergeColor(SecondFrameBrush, (this.SecondFrameFillColor as SolidColorBrush).Color);
                                }
                                else
                                {
                                    path.Fill = this.SecondFrameFillColor;
                                }
                            }
                            else
                            {
                                path.Fill = SecondFrameFillColor;
                            }
                        }
                    }
                    else
                    {
                        if (path == null)
                        {
                            this.SecondCircleBorder.Background = SecondFrameBrush;
                        }
                    }

                    this.SecondCircleBorder.OpacityMask = this.OpacityMask;
                    this.SecondCircleBorder.Opacity = this.Opacity;
                }

                if (this.InnerCircleBorder != null)
                {
                    if (this.Background == null)
                    {
                        Path path = this.Template.FindName("InnerPath", this) as Path;
                        if (CenterFrameFillColor != null)
                        {
                            if (path == null)
                            {
                                if (IsColorMergeWithBase)
                                {
                                    if (CenterFrameFillColor is SolidColorBrush && CenterFrameFillColor != Brushes.Transparent)
                                    {
                                        this.InnerCircleBorder.Background = MergeColor(InnerFrameBrush, (CenterFrameFillColor as SolidColorBrush).Color);
                                    }
                                    else
                                    {
                                        this.InnerCircleBorder.Background = CenterFrameFillColor;
                                    }
                                }
                                else
                                {
                                    this.InnerCircleBorder.Background = CenterFrameFillColor;
                                }
                            }
                            else
                            {
                                if (IsColorMergeWithBase)
                                {
                                    if (CenterFrameFillColor is SolidColorBrush)
                                    {
                                        path.Fill = MergeColor(InnerFrameBrush, (CenterFrameFillColor as SolidColorBrush).Color);
                                    }
                                    else
                                    {
                                        path.Fill = CenterFrameFillColor;
                                    }
                                }
                                else
                                {
                                    path.Fill = CenterFrameFillColor;
                                }
                            }
                        }
                        else
                        {
                            if (path == null)
                            {
                                this.InnerCircleBorder.Background = InnerFrameBrush;
                            }
                        }
                    }
                    else
                    {
                        Path path = this.Template.FindName("InnerPath", this) as Path;
                        if (path == null)
                        {
                            this.InnerCircleBorder.Background = this.Background;
                        }
                        else
                        {
                            path.Fill = this.Background;
                        }
                    }

                    this.InnerCircleBorder.OpacityMask = this.OpacityMask;
                    this.InnerCircleBorder.Opacity = this.Opacity;
                }
                if (this is DigitalGauge)
                {
                    (this as DigitalGauge).RefreshAdornerLayer();
                }
                else if (this is LinearGauge)
                {
                    (this as LinearGauge).RefreshAdornerLayer();
                }
                //else if (this is RollingGauge)
                //{
                //    (this as RollingGauge).RefreshAdornerLayer();
                //}
            }
        }

        /// <summary>
        /// Converts Logical bounds of range [0 - 100](as perceived by the user),<para/>
        /// to Actual control bounds with starting position as Center of the Gauge.
        /// </summary>
        /// <param name="p">Logical Point to convert.</param>
        /// <param name="width">Width of the gauge.</param>
        /// <param name="height">Height of the gauge.</param>
        /// <returns>Converted point.</returns>
        /// <remarks>
        ///         This method needs to calculate the ratio of logical point "p" to that of 100(the maximum 
        /// logical width/height possible) and multiply the ratio to that of the ActualWidth or ActualHeight
        /// as required, and hence get the point "p" converted to Control bounds.
        ///         But since we calculate actual control bounds with the Center of Gauge as origin, we need to 
        /// calculate the ratio, with respect to that of 50(i.e half of the logical maximum width/height
        /// possible) rather than to that of 100. 
        ///         For that, we need to bring the logical point "p" to values lesser than or equal to 50 
        /// and then calculate the ratio. If logical point "p" given by the user is greater than "50", 
        /// multiplying the ratio(calculated with 50 deducted logical point) with actual width or height / 2 
        /// will suffice.If the logical point given by the user is lesser than 50, we need to subtract the 
        /// result by half of the actual width/height as required.Hence get the Logical bounds converted to
        /// Actual actual with Center of gauge as the origin.
        /// </remarks>         
        protected virtual Point ConvertLocation(Point p, double width, double height)
        {
            double x = 0;
            double y = 0;
            double ratioX = 0;
            double ratioY = 0;
            double mywidth = width;
            double myheight = height;

            if (p.X < 50)
            {
                ratioX = p.X / 50;
                x = ((mywidth / 2) * ratioX) - (mywidth / 2);
            }
            else
            {                
                ratioX = (p.X - 50) / 50;
                x += mywidth / 2 * ratioX;
            }

            if (p.Y < 50)
            {
                ratioY = p.Y / 50;
                y = (myheight / 2 * ratioY) - (myheight / 2);
            }
            else
            {
                ratioY = (p.Y - 50) / 50;
               y += myheight / 2 * ratioY;
                
            }

            return new Point(x, y);
        }

        /// <summary>
        /// Converts point from range (0,0)-(100,100) to control bounds.
        /// </summary>
        /// <param name="p">Point to convert.</param>
        /// <returns>Converted point.</returns>
        protected virtual Point ConvertLocation(Point p)
        {

         return this.ConvertLocation(p, this.ActualWidth, this.DesiredSize.Height);

        }

        /// <summary>
        /// Invoked when the control is ready for presentation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected virtual void GaugeBaseLoaded(object sender, RoutedEventArgs e)
        {
            this.RefreshAdornerLayer();
            this.RefreshBorders();
            this.UpdateChildrenLocation();
        }

        /// <summary>
        /// Updates property value cache and raises AdornerStyleChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnAdornerStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AdornerStyleChanged != null)
            {
                this.AdornerStyleChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnAdornerStyleChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAdornerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeBase instance = (GaugeBase)d;
            instance.OnAdornerStyleChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises UseFrameStylesChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnApplyFrameStylesChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ApplyFrameStylesChanged != null)
            {
                this.ApplyFrameStylesChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnApplyFrameStylesChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnApplyFrameStylesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeBase instance = (GaugeBase)d;
            instance.OnApplyFrameStylesChanged(e);
        }

        /// <summary>
        /// Calls OnCenterFrameFillColorChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCenterFrameFillColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeBase instance = (GaugeBase)d;
            instance.RefreshBorders();
        }

        /// <summary>
        /// Invoked when <see cref="FrameworkElement.CursorProperty"/> is changed.
        /// </summary>
        /// <param name="d">The <see cref="CircularGauge"/> object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnCursorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeBase owner = d as GaugeBase;

            if (owner != null && owner.GaugeAdorner != null)
            {
                owner.GaugeAdorner.Cursor = owner.Cursor;
            }
        }

        /// <summary>
        /// Calls OnFirstFrameFillColorChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFirstFrameFillColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeBase instance = (GaugeBase)d;
            instance.RefreshBorders();
        }

        /// <summary>
        /// Virtual method to raise the OnFirstFrameThickness event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnFirstFrameThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FirstFrameThicknessChanged != null)
            {
                this.FirstFrameThicknessChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnFirstFrameThicknessChanged method of the instance, notifies of the dependency property values changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFirstFrameThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeBase instance = (GaugeBase)d;
            instance.OnFirstFrameThicknessChanged(e);
        }

        /// <summary>
        /// Calls OnSecondFrameFillColorChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSecondFrameFillColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeBase instance = (GaugeBase)d;
            instance.RefreshBorders();
        }

        /// <summary>
        /// Virtual method to raise the OnSecondFrameThickness event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnSecondFrameThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SecondFrameThicknessChanged != null)
            {
                SecondFrameThicknessChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnInnerFrameContentChanged method of the instance, notifies of the dependency property value change.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnInnerFrameContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeBase instance = (GaugeBase)d;
            instance.OnInnerFrameContentChanged(e);
        }

        /// <summary>
        /// Updates property value cache.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnInnerFrameContentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                this.RemoveVisualChild(e.OldValue as Visual);
            }
            else
            {
                this.AddLogicalChild(e.NewValue as Visual);
            }
        }

        /// <summary>
        /// Calls OnSecondFrameThicknessChanged method of the instance, notifies of the dependency property values changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSecondFrameThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeBase instance = (GaugeBase)d;
            instance.OnSecondFrameThicknessChanged(e);
        }

        /// <summary>
        /// Updates the location of children elements.
        /// </summary>
        protected virtual void UpdateChildrenLocation()
        {
            int count = this.ChildrenCollection.Count;
            for (int i = 0; i < count; i++)
            {
                if (this.ChildrenCollection[i] is LocalizableGaugeElement)
                {
                    LocalizableGaugeElement elem = this.ChildrenCollection[i] as LocalizableGaugeElement;
                    Point location = this.ConvertLocation(elem.Location);
                    elem.RenderTransform = new TranslateTransform(location.X, location.Y);
                }
            }
        }

        /// <summary>
        /// Updates property value cache and raises IsColorMergeWithBaseChangedz event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnIsColorMergeWithBaseChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsColorMergeWithBaseChanged != null)
            {
                this.IsColorMergeWithBaseChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnApplyFrameStylesChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsColorMergeWithBaseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeBase instance = (GaugeBase)d;
            instance.OnIsColorMergeWithBaseChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises OnEnableEffectsChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnEnableEffectsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (EnableEffectsChanged != null)
            {
                this.EnableEffectsChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnEnableEffectsChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnEnableEffectsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeBase instance = (GaugeBase)d;
            instance.OnEnableEffectsChanged(e);
        }
        #endregion Implementation
    }
}