// <copyright file="SplitterAdorner.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <property name="flag" value="Finished" />
    /// <summary>
    /// SplitterAdorner class.
    /// </summary>
    /// <remarks>
    /// In docking SplitterAdorner is used when you resize some dock window. When you start resizing,
    /// you can see horizontal or vertical line appear when dragging any side of the dock window. 
    /// This line is splitter.
    /// You can override style or template of the Splitter.
    /// </remarks>
    /// <example>
    /// <para/>This example shows how to override the template of the SplitterAdorner in XAML.
    /// <code language="XAML">
    /// <![CDATA[
    /// <ControlTemplate x:Key="SplitterAdornerTemplate">
    /// <StackPanel x:Name="stPanel" Orientation="Horizontal">
    ///  <Border MinWidth="4" MinHeight="4">
    ///    <Border.Background>
    ///      <SolidColorBrush Color="Gray" Opacity="0.6" />
    ///    </Border.Background>
    ///  </Border>
    /// </StackPanel>
    /// <ControlTemplate.Triggers>
    ///  <Trigger Property="Syncfusion:SplitterAdorner.Orientation" Value="Horizontal">
    ///    <Setter TargetName="stPanel" Property="LayoutTransform">
    ///      <Setter.Value>
    ///        <RotateTransform Angle="-90"/>
    ///      </Setter.Value>
    ///    </Setter>
    ///  </Trigger>
    /// </ControlTemplate.Triggers>
    /// </ControlTemplate>
    /// ]]>
    /// </code>
    /// </example>
    /// <seealso cref="Splitter"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class SplitterAdorner : Adorner
    {
        #region Private Members

        /// <summary>
        /// Control that is the only child of the adorner and represents
        /// adorner.
        /// </summary>
        private readonly Control m_innerControl;
                
        /// <summary>
        /// Specifies adorner's x-offset.
        /// </summary>
        private double m_offsetX;
        
        /// <summary>
        /// Specifies adorner's y-offset.
        /// </summary>
        private double m_offsetY;
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="SplitterAdorner"/> class.
        /// </summary>
        /// <param name="splitter">The splitter.</param>
        public SplitterAdorner(UIElement splitter)
            : base(splitter)
        {
            if (splitter == null)
            {
                throw new ArgumentNullException("splitter");
            }

            m_innerControl = new Control();
            BindingUtils.SetBinding(m_innerControl, splitter, Control.TemplateProperty, Splitter.AdornerTemplateProperty);
            BindingUtils.SetBinding(m_innerControl, splitter, OrientationProperty, Splitter.OrientationProperty);
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when LeftLimitReached property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback LeftLimitReachedChanged;

        /// <summary>
        /// Event that is raised when RightLimitReached property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback RightLimitReachedChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the value of the LeftLimitReached dependency
        /// property.
        /// </summary>
        /// <value><c>true</c> if [left limit reached]; otherwise, <c>false</c>.</value>
        protected internal bool LeftLimitReached
        {
            get
            {
                return (bool)GetValue(LeftLimitReachedProperty);
            }

            set
            {
                SetValue(LeftLimitReachedPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the value of the RightLimitReached dependency
        /// property.
        /// </summary>
        /// <value><c>true</c> if [right limit reached]; otherwise, <c>false</c>.</value>
        protected internal bool RightLimitReached
        {
            get
            {
                return (bool)GetValue(RightLimitReachedProperty);
            }

            set
            {
                SetValue(RightLimitReachedPropertyKey, value);
            }
        }
        
        /// <summary>
        /// Gets splitter render size.
        /// </summary>
        /// <example>
        /// <para/>This example shows how to use SplitterSize property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Style x:Key="SplitterBaseStyle" TargetType="{x:Type Syncfusion:Splitter}">
        /// <Setter Property="MinWidth" Value="{Binding Path=SplitterSize
        /// , RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Syncfusion:DockingManager}}}"/>
        /// <Setter Property="MinHeight" Value="{Binding Path=SplitterSize
        /// , RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Syncfusion:DockingManager}}}"/>
        /// </Style>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="Size"/>
        public Size SplitterSize
        {
            get
            {
                return AdornedElement.RenderSize;
            }
        }

        /// <summary>
        /// Gets x offset.
        /// </summary>
        internal double OffsetX
        {
            get
            {
                return m_offsetX;
            }
        }
        
        /// <summary>
        /// Gets y offset.
        /// </summary>
        internal double OffsetY
        {
            get
            {
                return m_offsetY;
            }
        }

        /// <summary>
        /// Gets or sets adorner orientation.
        /// </summary>
        /// <value>The orientation.</value>
        /// <example>
        /// To set Orientation property please see <see cref="Splitter.AdornerTemplate"/> property example.
        /// </example>
        /// <seealso cref="Orientation"/>
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

        #endregion

        #region Public methods
        /// <summary>
        /// Adds translate transformation to the adorner.
        /// </summary>
        /// <param name="transform">The transform that is currently
        /// applied to the adorned element.</param>
        /// <returns>A transform to apply to the adorner.</returns>
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransform baseTransform = base.GetDesiredTransform(transform);
            TransformGroup group = new TransformGroup();

            if (m_offsetX != 0 || m_offsetY != 0)
            {
                TranslateTransform offsetting = new TranslateTransform(m_offsetX, m_offsetY);
                group.Children.Add(offsetting);
            }

            Size renderSize = AdornedElement.RenderSize;
            TranslateTransform centering = new TranslateTransform(
              (renderSize.Width - DesiredSize.Width) / 2.0,
              (renderSize.Height - DesiredSize.Height) / 2.0);

            group.Children.Add(centering);
            group.Children.Add((Transform)baseTransform);
            return group;
        }

        #endregion

        #region Implementations
        /// <summary>
        /// Changes offsets of the adorner.
        /// </summary>
        /// <param name="xOffset">New x\-offset of the adorner.</param>
        /// <param name="yOffset">New y\-offset of the adorner.</param>
        /// <returns>
        /// Specifies whether some position has been changed.
        /// </returns>
        protected internal virtual bool ChangeOffsets(double xOffset, double yOffset)
        {
            bool result = m_offsetX != xOffset || m_offsetY != yOffset;
            m_offsetX = xOffset;
            m_offsetY = yOffset;

            return result;
        }

        /// <summary>
        /// Updates property value cache and raises
        /// LeftLimitReachedChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnLeftLimitReachedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LeftLimitReachedChanged != null)
            {
                LeftLimitReachedChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises
        /// RightLimitReachedChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnRightLimitReachedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (RightLimitReachedChanged != null)
            {
                RightLimitReachedChanged(this, e);
            }
        }

        /// <summary>
        /// Measures content.
        /// </summary>
        /// <param name="constraint">The size that the image should not
        /// exceed.</param>
        /// <returns>The image's desired size.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            m_innerControl.Measure(constraint);

            Size result = new Size(Math.Max(m_innerControl.DesiredSize.Width, AdornedElement.RenderSize.Width), Math.Max(m_innerControl.DesiredSize.Height, AdornedElement.RenderSize.Height));
            return result;
        }

        /// <summary>
        /// Arranges inner control to the full size.
        /// </summary>
        /// <param name="finalSize">The final size of the window object
        /// and its child elements.</param>
        /// <returns>A Size that is the size of the window.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            m_innerControl.Arrange(new Rect(finalSize));
            return m_innerControl.RenderSize;
        }

        /// <summary>
        /// Gets visual by index.
        /// </summary>
        /// <param name="index">The index of the visual object.</param>
        /// <returns>
        /// The child in the VisualCollection at the specified index
        /// value.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            return m_innerControl;
        }

        /// <summary>
        /// Gets visual children count, always 1.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of visual child elements for this element.
        /// </returns>
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Calls OnRightLimitReachedChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnRightLimitReachedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplitterAdorner instance = (SplitterAdorner)d;
            instance.OnRightLimitReachedChanged(e);
        }

        /// <summary>
        /// Calls OnLeftLimitReachedChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnLeftLimitReachedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplitterAdorner instance = (SplitterAdorner)d;
            instance.OnLeftLimitReachedChanged(e);
        }
        #endregion

        #region Dependency Properties
        /// <summary>
        /// Left Limit Reached Dependency Property Key.
        /// </summary>
        protected static readonly DependencyPropertyKey LeftLimitReachedPropertyKey =
            DependencyProperty.RegisterReadOnly("LeftLimitReached", typeof(bool), typeof(SplitterAdorner), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnLeftLimitReachedChanged)));
        
        /// <summary>
        /// Right Limit Reached Dependency Property Key.
        /// </summary>
        protected static readonly DependencyPropertyKey RightLimitReachedPropertyKey =
            DependencyProperty.RegisterReadOnly("RightLimitReached", typeof(bool), typeof(SplitterAdorner), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnRightLimitReachedChanged)));

        /// <summary>
        /// Orientation dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(SplitterAdorner), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
        
        /// <summary>
        /// Left Limit Reached Dependency Property.
        /// </summary>
        public static readonly DependencyProperty LeftLimitReachedProperty
            = LeftLimitReachedPropertyKey.DependencyProperty;
        
        /// <summary>
        /// Right Limit Reached Dependency Property.
        /// </summary>
        public static readonly DependencyProperty RightLimitReachedProperty =
            RightLimitReachedPropertyKey.DependencyProperty;
        #endregion
    }
}
