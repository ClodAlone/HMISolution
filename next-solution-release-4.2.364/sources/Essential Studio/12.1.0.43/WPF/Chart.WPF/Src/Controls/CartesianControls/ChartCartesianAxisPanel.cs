// <copyright file="ChartCartesianAxisPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Controls;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation;

    /// <summary>
    /// Represents cartesian coordinate system axis.
    /// </summary>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class ChartCartesianAxisPanel : Panel, IDisposable
    {
        private class ChartCartesianAxisPanelAutomationPeer : FrameworkElementAutomationPeer
        {
            public ChartCartesianAxisPanelAutomationPeer(ChartCartesianAxisPanel control)
                : base(control)
            {
            }

            protected override string GetClassNameCore()
            {
                return "ChartCartesianAxisPanel";
            }

            protected override AutomationControlType GetAutomationControlTypeCore()
            {
                return AutomationControlType.Custom;
            }

            public override object GetPattern(PatternInterface patternInterface)
            {                
                    return this;
                
            }

            protected override string GetAutomationIdCore()
            {
                return this.MyOwner.Axis.Name;
            }
            private ChartCartesianAxisPanel MyOwner
            {
                get
                {
                    return (ChartCartesianAxisPanel)base.Owner;
                }
            }
        }

        /// <summary>
        /// Returns class-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer"/> implementations for the Windows Presentation Foundation (WPF) infrastructure.
        /// </summary>
        /// <returns>
        /// The type-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer"/> implementation.
        /// </returns>
        protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
        {
            return new ChartCartesianAxisPanelAutomationPeer(this);
        }
        #region Dependency properties
        /// <summary>
        /// Identifies the Axis dependency property.
        /// </summary>
        /// <summary>
        /// Using a DependencyProperty as the backing store for Axis. This enables animation, styling, binding, etc...
        /// </summary>
        internal static readonly DependencyProperty AxisProperty =
            DependencyProperty.Register("Axis", typeof(ChartAxis), typeof(ChartCartesianAxisPanel), new FrameworkPropertyMetadata(null));
        /// <summary>
        /// Identifies the Orientation dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ChartCartesianAxisPanel), new UIPropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(OnOpposedPositionChanged)));
       
        /// <summary>
        /// Identifies the OpposedPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty OpposedPositionProperty =
            DependencyProperty.Register("OpposedPosition", typeof(bool), typeof(ChartCartesianAxisPanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(OnOpposedPositionChanged)));

        /// <summary>
        /// Identifies the ValueType dependency property.
        /// </summary>
        internal static readonly DependencyProperty ValueTypeProperty =
          DependencyProperty.Register("ValueType", typeof(ChartValueType), typeof(ChartCartesianAxisPanel), new PropertyMetadata(ChartValueType.Double));

        /// <summary>
        /// Identifies the HeaderPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderPositionProperty =
            DependencyProperty.Register("HeaderPosition", typeof(HeaderPositions), typeof(ChartCartesianAxisPanel), new FrameworkPropertyMetadata(HeaderPositions.Outside, FrameworkPropertyMetadataOptions.AffectsArrange));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the panel orientation. This is a dependency property.
        /// </summary>
        /// <value>The orientation.</value>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }

            set
            {
                SetValue(OrientationProperty, value);
            }
        }


        /// <summary>
        /// Gets or sets the type of the value that chart axis displays. This is a dependency property.
        /// </summary>
        /// <value>The type of the value.</value>
        /// <seealso cref="ChartValueType"/>
        public ChartValueType ValueType
        {
            get
            {
                return (ChartValueType)GetValue(ValueTypeProperty);
            }
            set
            {
                SetValue(ValueTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether panel has opposed position. This is a dependency property.
        /// </summary>
        /// <value><c>true</c> if panel has opposed position; otherwise, <c>false</c>.</value>
        public bool OpposedPosition
        {
            get
            {
                return (bool)GetValue(OpposedPositionProperty);
            }

            set
            {
                SetValue(OpposedPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the axis header position. This is a dependency property.
        /// </summary>
        /// <value>the AxisHeaderPostion. either <c>Far</c> or <c>Near</c>. <c>Far</c> is a default value. </value>
        public HeaderPositions HeaderPosition
        {
            get
            {
                return (HeaderPositions)GetValue(HeaderPositionProperty);
            }

            set
            {
                SetValue(HeaderPositionProperty, value);
            }
        }


        /// <summary>
        /// Gets or sets the axis. This is a dependency property.
        /// </summary>
        /// <value>The axis value.</value>
        public ChartAxis Axis
        {
            get
            {
                return (ChartAxis)GetValue(AxisProperty);
            }

            set
            {
                SetValue(AxisProperty, value);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartCartesianAxisPanel"/> class.
        /// </summary>
        public ChartCartesianAxisPanel()
        {
            base.ClipToBounds = false;
            AutomationProperties.SetItemStatus(this, string.Empty + ";" + string.Empty + ";" + string.Empty + ";" + string.Empty + ";" + string.Empty + ";" + string.Empty + ";" + string.Empty + ";" + string.Empty + ";" + this.Orientation.ToString() + ";" + this.OpposedPosition.ToString() + ";" + this.ValueType.ToString() + ";");
        }
        #endregion

        #region Implmentation

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.FrameworkElement"/> has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)"/>.
        /// </summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == ChartCartesianAxisPanel.OrientationProperty || e.Property == ChartCartesianAxisPanel.OpposedPositionProperty || e.Property == ChartCartesianAxisPanel.ValueTypeProperty)
            {
                AutomationProperties.SetItemStatus(this,string.Empty + ";" + string.Empty + ";" + string.Empty + ";" + string.Empty + ";" + string.Empty + ";" + string.Empty + ";" + string.Empty + ";" + string.Empty +";" + this.Orientation.ToString() + ";" + this.OpposedPosition.ToString() + ";" + this.ValueType.ToString() + ";");
            }
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"></see> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            UIElement element = null;
            bool isVertical = Orientation == Orientation.Vertical;
            bool inversed = this.OpposedPosition ^ isVertical;
            double coordinate = 0;
            //Add header below / left
            if (HeaderPosition == HeaderPositions.Outside)
            {
                for (int i = 0, ci = this.InternalChildren.Count; i < ci; i++)
                {
                    element = inversed ? this.InternalChildren[i] : this.InternalChildren[ci - i - 1];

                    if (isVertical)
                    {
                        Rect rect = new Rect(coordinate, 0, element.DesiredSize.Width, finalSize.Height);
                        element.Arrange(rect);
                        coordinate = rect.Right;
                    }
                    else
                    {
                        Rect rect = new Rect(0, coordinate, finalSize.Width, element.DesiredSize.Height == 0 ? (element as FrameworkElement).ActualHeight : element.DesiredSize.Height);
                        element.Arrange(rect);
                        coordinate = rect.Bottom;
                    }
                }
            }
            
            //Add header above / right
            else
            {
                int i = 0;
                int ci = this.InternalChildren.Count;
                int[] order = new int[ci];
                double offsetVertical = 0;
                if (inversed) // Element Arranging Order 1 2 3 ... 0
                {
                    for (int j = 0; j < ci; j++)
                    {
                        order[j] = j + 1;
                    }
                    order[ci - 1] = 0;
                }
                else // Element Arranging Order 3 2 1...0
                {
                    int a = 0;
                    for (int k = ci - 1; k >= 0; k--)
                    {
                        order[a] = k;
                        a++;
                    }
                    //order[0] = 0;
                }
                while (i < ci)
                {
                    element = this.InternalChildren[order[i]];
                    if (isVertical)
                    {
                        if (i == 0)
                        {
                            coordinate = this.InternalChildren[0].DesiredSize.Width;
                        }
                        if (order[i] != 0)
                        {
                            Rect rect = new Rect(coordinate, 0, element.DesiredSize.Width, finalSize.Height);
                            offsetVertical += element.DesiredSize.Width;
                            coordinate = rect.Right;                            
                            element.Arrange(rect);
                        }
                        else
                        {
                            element.Arrange(new Rect(coordinate+offsetVertical,0,element.DesiredSize.Width,finalSize.Height));
                        }
                    }
                    else
                    {                        
                        if (order[i] != 0)
                        {
                            Rect rect = new Rect(0, coordinate, finalSize.Width, element.DesiredSize.Height == 0 ? (element as FrameworkElement).ActualHeight : element.DesiredSize.Height);
                            coordinate = rect.Bottom;
                            element.Arrange(rect);
                        }
                        else
                        {
                            element.Arrange(new Rect(0,-coordinate-element.DesiredSize.Height,finalSize.Width,element.DesiredSize.Height));
                        }
                    }

                    i++;
                }

            }

            return finalSize;
        }

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"></see>-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size resultSize = new Size();
            Size size = availableSize;
            bool isVectical = Orientation == Orientation.Vertical;

            foreach (UIElement element in InternalChildren)
            {
                if (isVectical)
                {
                    element.Measure(size);
                    size.Width = Math.Max(size.Width - element.DesiredSize.Width, 0);
                    resultSize.Height = Math.Max(resultSize.Height, 0);
                    resultSize.Width += element.DesiredSize.Width;
                }
                else
                {
                    element.Measure(size);
                    size.Height = Math.Max(size.Height - element.DesiredSize.Height, 0);
                    resultSize.Width = Math.Max(resultSize.Width, element.DesiredSize.Width);
                    resultSize.Height += element.DesiredSize.Height;
                }
            }

            return resultSize;
        }

        /// <summary>
        /// Called when opposed position property changed.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnOpposedPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartCartesianAxisPanel parent = d as ChartCartesianAxisPanel;
            if (parent != null)
            {
                foreach (UIElement ui in parent.InternalChildren)
                {
                    if (ui is ChartCartesianAxisElement)
                    {
                        ui.InvalidateVisual();
                        break;
                    }
                }
            }
        }
        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (this.Resources != null)
            {
                this.Resources.Clear();
                this.Resources = null;
            }
        }

        #endregion
    }
}
