#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Shared
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Scaling Panel
    /// </summary>
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
        Type = typeof(FishEyePanel), XamlResource = "/Syncfusion.Theming.Blend;component/FishEyePanel.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(FishEyePanel), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/FishEyePanel.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(FishEyePanel), XamlResource = "/Syncfusion.Theming.Office2007Black;component/FishEyePanel.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(FishEyePanel), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/FishEyePanel.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(FishEyePanel), XamlResource = "/Syncfusion.Theming.Default;component/FishEyePanel.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
        Type = typeof(FishEyePanel), XamlResource = "/Syncfusion.Theming.Office2003;component/FishEyePanel.xaml")]
    public class FishEyePanel : Panel
    {
        #region Dependency Properties

        /// <summary>
        /// This property is used to set the Magnification ratio for the FishEyePanel's children.
        /// </summary>
        public static readonly DependencyProperty MagnificationFactorProperty = DependencyProperty.Register("MagnificationFactor", typeof(double), typeof(FishEyePanel), new PropertyMetadata(2d));

        /// <summary>
        /// This property is used to specify the animation speed.
        /// </summary>
        public static readonly DependencyProperty AnimatingDurationProperty = DependencyProperty.Register("AnimatingDuration", typeof(int), typeof(FishEyePanel), new PropertyMetadata(125));

        /// <summary>
        /// This property is used to specify whether different sized children are scaled to a uniform size.
        /// </summary>
        public static readonly DependencyProperty UniformScalingProperty = DependencyProperty.Register("UniformScaling", typeof(bool), typeof(FishEyePanel), new PropertyMetadata(true));

        /// <summary>
        /// This property stores the orientation for the panel.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(FishEyePanel), new PropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(OnOrientationChanged)));

        #endregion
        
        #region Internal Variables
        /// <summary>
        /// stores value indication whether animation is taking place.
        /// </summary>
        private bool isAnimating = false;

        /// <summary>
        /// Stores the size available for the panel
        /// </summary>
        private Size panelSize;

        /// <summary>
        /// stores sum of the widths of all children
        /// </summary>
        private double totalChildrenWidth = 0;

        /// <summary>
        /// stores sum of the heights of all children
        /// </summary>
        private double totalChildrenHeight = 0;

        /// <summary>
        /// stores value to determine if mouse remains over the panel
        /// </summary>
        private bool wasMouseOver = false;

        /// <summary>
        /// stores value indicating the presence of mouse over the panel
        /// </summary>
        private bool isMouseOver = false;

        /// <summary>
        /// Determines whether the orientation has been changed. Used in ArrangeOverride.
        /// </summary>
        private bool isOrientationChanged = false;

        /// <summary>
        /// Stores the MouseEventArgs when moves moves over the panel.
        /// </summary>
        private MouseEventArgs mouseArgs;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the FishEyePanel class
        /// </summary>
        public FishEyePanel()
        {
            this.Background = new SolidColorBrush(Colors.Transparent);
            this.MouseMove += new MouseEventHandler(this.FishEyePanel_MouseMove);
            this.MouseEnter += new MouseEventHandler(this.FishEyePanel_MouseEnter);
            this.MouseLeave += new MouseEventHandler(this.FishEyePanel_MouseLeave);
        }

        #endregion

        #region DP Wrappers
        /// <summary>
        /// Gets or sets the magnification ratio of the FishEyePanel.
        /// </summary>
        public double MagnificationFactor
        {
            get { return (double)GetValue(MagnificationFactorProperty); }
            set { SetValue(MagnificationFactorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the animation speed of the FishEyePanel.
        /// </summary>
        public int AnimatingDuration
        {
            get { return (int)GetValue(AnimatingDurationProperty); }
            set { SetValue(AnimatingDurationProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the different sized children are scaled to uniform width.
        /// </summary>
        public bool UniformScaling
        {
            get { return (bool)GetValue(UniformScalingProperty); }
            set { SetValue(UniformScalingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Orientation of the panel.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        } 
        #endregion

        #region Overrides
        /// <summary>
        /// Measure Override. 
        /// </summary>
        /// <param name="availableSize">The size available for the panel</param>
        /// <returns>the size required by the panel</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size infiniteSize = new Size(Double.PositiveInfinity, Double.PositiveInfinity);
            Size requiredSize = new Size(0, 0);
            
            if (Orientation == Orientation.Horizontal)
            {
                foreach (UIElement child in Children)
                {
                    if (child.Visibility == Visibility.Visible)
                    {
                        child.Measure(infiniteSize);
                        requiredSize.Width += child.DesiredSize.Width;
                        requiredSize.Height = Math.Max(requiredSize.Height, child.DesiredSize.Height);
                    }
                }
            }
            else
            {
                foreach (UIElement child in Children)
                {
                    if (child.Visibility == Visibility.Visible)
                    {
                        child.Measure(infiniteSize);
                        requiredSize.Height += child.DesiredSize.Height;
                        requiredSize.Width = Math.Max(requiredSize.Width, child.DesiredSize.Width);
                    }
                }
            }

            if (double.IsInfinity(availableSize.Height) || double.IsInfinity(availableSize.Width))
            {
                return requiredSize;
            }
            else
            {
                return availableSize;
            }
        }

        /// <summary>
        /// Arrange Override.
        /// </summary>
        /// <param name="arrangeSize">The size in which the children are to be arranged.</param>
        /// <returns>The size in which the children were arranged</returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if ((this.Children == null) || (this.Children.Count == 0))
            {
                return arrangeSize;
            }

            this.panelSize = arrangeSize;
            this.totalChildrenWidth = 0;
            this.totalChildrenHeight = 0;
            Point point = new Point(0, 0);
            
            foreach (UIElement child in this.Children)
            {
                if (child.Visibility == Visibility.Visible)
                {
                    if ((child.RenderTransform as TransformGroup == null) || (this.isOrientationChanged))
                    {
                        if (Orientation == Orientation.Horizontal)
                        {
                            child.RenderTransformOrigin = new Point(0, 0.5);
                        }
                        else
                        {
                            child.RenderTransformOrigin = new Point(0.5, 0);
                        }

                        TransformGroup transformGroup = new TransformGroup();
                        transformGroup.Children.Add(new ScaleTransform());
                        transformGroup.Children.Add(new TranslateTransform());
                        child.RenderTransform = transformGroup;
                    }

                    Rect bounds = new Rect(point, new Point(child.DesiredSize.Width, child.DesiredSize.Height));
                    child.Arrange(bounds);
                    this.totalChildrenHeight += child.DesiredSize.Height;
                    this.totalChildrenWidth += child.DesiredSize.Width;
                }
            }

            this.isOrientationChanged = false;
            this.ScaleChildren();
            return arrangeSize;
        } 
        #endregion

        #region Methods and Events

        /// <summary>
        /// Orientation property changed callback
        /// </summary>
        /// <param name="obj">Object(MenuAdv object) whose Orientation is changed</param>
        /// <param name="e">DP changed event args</param>
        private static void OnOrientationChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            FishEyePanel fishEyePanel = (FishEyePanel)obj;
            fishEyePanel.isOrientationChanged = true;
            fishEyePanel.InvalidateMeasure();
        }

        /// <summary>
        /// Occurs when mouse moves over the panel. Keeps updating the mousArgs to get the synchronous mouse position in other methods.
        /// </summary>
        /// <param name="sender">the FishEye Panel on which the mouse moved</param>
        /// <param name="e">mouse event args</param>
        private void FishEyePanel_MouseMove(object sender, MouseEventArgs e)
        {
            this.mouseArgs = e;
            if (!this.isAnimating)
            {
                this.InvalidateArrange();
            }
        }

        /// <summary>
        /// Occurs when mouse moves over the panel.
        /// Updates the initial value for mouseArgs.
        /// Sets boolean, indicating that the mouse is over the panel.
        /// </summary>
        /// <param name="sender">the FishEye Panel into which the mouse entered</param>
        /// <param name="e">mouse event args</param>
        private void FishEyePanel_MouseEnter(object sender, MouseEventArgs e)
        {
            this.isMouseOver = true;
            this.mouseArgs = e;
            this.InvalidateArrange();
        }

        /// <summary>
        /// Occurs when mouse moves over the panel.
        /// Sets boolean, indicating that the mouse is out of the panel.
        /// </summary>
        /// <param name="sender">the FishEye Panel from which the mouse moved out</param>
        /// <param name="e">mouse event args</param>
        private void FishEyePanel_MouseLeave(object sender, MouseEventArgs e)
        {
            this.isMouseOver = false;
            this.InvalidateArrange();
        }

        /// <summary>
        /// the core of the FishEyePanel performing all necessary calculations.
        /// </summary>
        private void ScaleChildren()
        {
            if ((this.Children == null) || (this.Children.Count == 0))
            {
                return;
            }

            int visibleChildrenCount = 0;
            double widthOfOneChild = 0;
            double heightOfOneChild = 0;
            double panelChildrenScaleRatio;
            double widthCovered = 0;
            double heightCovered = 0;
            double currentElementOffsetX = 0;
            double currentElementOffsetY = 0;
            double mouseOffsetInsideChild = 0;
            double previousElementScale = 0;
            double currentElementScale = 0;
            double nextElementScale = 0;
            double otherElementsScale = 0;
            double duration = 0;
            UIElement previousElement = null;
            UIElement currentElement = null;
            UIElement nextElement = null;
            double additionalSpace = 0;
            this.isAnimating = true;

            visibleChildrenCount = (from c in this.Children where (c.Visibility == Visibility.Visible) select c).Count();

            if (this.Orientation == Orientation.Horizontal)
            {
                widthOfOneChild = (1.0 / visibleChildrenCount) * this.panelSize.Width;
                panelChildrenScaleRatio = (1.0 / this.totalChildrenWidth) * this.panelSize.Width;

                if (this.isMouseOver)
                {
                    Point mousePosition = this.mouseArgs.GetPosition(this);
                    double horizontalMouseOffset = mousePosition.X;

                    // determines the current child and its previous and next child to be magnified
                    foreach (UIElement child in this.Children)
                    {
                        if (child.Visibility == Visibility.Visible)
                        {
                            if (currentElement == null)
                            {
                                currentElementOffsetX = widthCovered;
                            }

                            if (this.UniformScaling)
                            {
                                widthCovered += widthOfOneChild;
                            }
                            else
                            {
                                widthCovered += child.DesiredSize.Width * panelChildrenScaleRatio;
                            }

                            if ((horizontalMouseOffset < widthCovered) && (currentElement == null))
                            {
                                currentElement = child;
                            }
                            else if (currentElement == null)
                            {
                                previousElement = child;
                            }
                            else if ((nextElement == null) && (currentElement != child))
                            {
                                nextElement = child;
                                break;
                            }
                        }
                    }

                    if (currentElement != null)
                    {
                        // calculates Portion covered by mouse inside the current child. (ranges between 0 and 1).
                        if (this.UniformScaling)
                        {
                            mouseOffsetInsideChild = (horizontalMouseOffset - currentElementOffsetX) / widthOfOneChild;
                        }
                        else
                        {
                            mouseOffsetInsideChild = (horizontalMouseOffset - currentElementOffsetX) / (currentElement.DesiredSize.Width * panelChildrenScaleRatio);
                        }
                    }
                }
            }
            else
            {
                heightOfOneChild = (1.0 / visibleChildrenCount) * this.panelSize.Height;
                panelChildrenScaleRatio = (1.0 / this.totalChildrenHeight) * this.panelSize.Height;

                if (this.isMouseOver)
                {
                    Point mousePosition = this.mouseArgs.GetPosition(this);
                    double verticalMouseOffset = mousePosition.Y;

                    // determines the child and its neighbours to be magnified
                    foreach (UIElement child in this.Children)
                    {
                        if (child.Visibility == Visibility.Visible)
                        {
                            if (currentElement == null)
                            {
                                currentElementOffsetY = heightCovered;
                            }

                            if (this.UniformScaling)
                            {
                                heightCovered += heightOfOneChild;
                            }
                            else
                            {
                                heightCovered += child.DesiredSize.Height * panelChildrenScaleRatio;
                            }

                            if ((verticalMouseOffset < heightCovered) && (currentElement == null))
                            {
                                currentElement = child;
                            }
                            else if (currentElement == null)
                            {
                                previousElement = child;
                            }
                            else if ((nextElement == null) && (currentElement != child))
                            {
                                nextElement = child;
                                break;
                            }
                        }
                    }

                    if (currentElement != null)
                    {
                        // Calculates Portion covered by mouse inside the current child. (ranges between 0 and 1).
                        if (this.UniformScaling)
                        {
                            mouseOffsetInsideChild = (verticalMouseOffset - currentElementOffsetY) / heightOfOneChild;
                        }
                        else
                        {
                            mouseOffsetInsideChild = (verticalMouseOffset - currentElementOffsetY) / (currentElement.DesiredSize.Height * panelChildrenScaleRatio);
                        }
                    }
                }
            }

            if (currentElement != null)
            {
                additionalSpace += this.MagnificationFactor - 1;
            }
            else if (previousElement == null)
            {
                additionalSpace += (this.MagnificationFactor - 1) * mouseOffsetInsideChild;
            }
            else if (nextElement == null)
            {
                additionalSpace += (this.MagnificationFactor - 1) * (1 - mouseOffsetInsideChild);
            }
            else
            {
                additionalSpace += this.MagnificationFactor - 1;
            }

            previousElementScale = (1 + ((this.MagnificationFactor - 1) * (1 - mouseOffsetInsideChild))) * this.Children.Count  / (additionalSpace + this.Children.Count);
            currentElementScale = this.MagnificationFactor * this.Children.Count / (additionalSpace + this.Children.Count);
            nextElementScale = (1 + ((this.MagnificationFactor - 1) * mouseOffsetInsideChild)) * this.Children.Count / (additionalSpace + this.Children.Count);
            otherElementsScale = this.Children.Count / (additionalSpace + this.Children.Count);

            if (Orientation == Orientation.Horizontal)
            {
                if (!this.UniformScaling && this.isMouseOver)
                {
                    double scaledWidth = 0;
                    double actualWidth = 0;
                    if (previousElement != null)
                    {
                        scaledWidth += panelChildrenScaleRatio * previousElementScale * previousElement.DesiredSize.Width;
                        actualWidth += previousElement.DesiredSize.Width;
                    }

                    if (currentElement != null)
                    {
                        scaledWidth += panelChildrenScaleRatio * currentElementScale * currentElement.DesiredSize.Width;
                        actualWidth += currentElement.DesiredSize.Width;
                    }

                    if (nextElement != null)
                    {
                        actualWidth += nextElement.DesiredSize.Width;
                        scaledWidth += panelChildrenScaleRatio * nextElementScale * nextElement.DesiredSize.Width;
                    }

                    otherElementsScale = (this.panelSize.Width - scaledWidth) / ((this.totalChildrenWidth - actualWidth) * panelChildrenScaleRatio);
                }
            }
            else
            {
                if (!this.UniformScaling && this.isMouseOver)
                {
                    double scaledHeight = 0;
                    double actualHeight = 0;
                    if (previousElement != null)
                    {
                        actualHeight += previousElement.DesiredSize.Height;
                        scaledHeight += previousElementScale * previousElement.DesiredSize.Height * panelChildrenScaleRatio;
                    }

                    if (currentElement != null)
                    {
                        actualHeight += currentElement.DesiredSize.Height;
                        scaledHeight += currentElementScale * currentElement.DesiredSize.Height * panelChildrenScaleRatio;
                    }

                    if (nextElement != null)
                    {
                        actualHeight += nextElement.DesiredSize.Height;
                        scaledHeight += nextElementScale * nextElement.DesiredSize.Height * panelChildrenScaleRatio;
                    }

                    otherElementsScale = (this.panelSize.Height - scaledHeight) / ((this.totalChildrenHeight - actualHeight) * panelChildrenScaleRatio);
                }
            }

            if (this.wasMouseOver != this.isMouseOver)
            {
                duration = this.AnimatingDuration;
            }

            widthCovered = 0;
            heightCovered = 0;
            foreach (UIElement child in this.Children)
            {
                if (child.Visibility == Visibility.Visible)
                {
                    double childScale = otherElementsScale;
                    if (child == previousElement)
                    {
                        childScale = previousElementScale;
                    }
                    else if (child == currentElement)
                    {
                        childScale = currentElementScale;
                    }
                    else if (child == nextElement)
                    {
                        childScale = nextElementScale;
                    }

                    if (this.UniformScaling)
                    {
                        if (Orientation == Orientation.Horizontal)
                        {
                            if (child.DesiredSize.Width != 0)
                            {
                                childScale *= widthOfOneChild / child.DesiredSize.Width;
                            }
                        }
                        else
                        {
                            if (child.DesiredSize.Height != 0)
                            {
                                childScale *= heightOfOneChild / child.DesiredSize.Height;
                            }
                        }
                    }
                    else
                    {
                        childScale *= panelChildrenScaleRatio;
                    }

                    if (Orientation == Orientation.Horizontal)
                    {
                        this.AnimateChild(child, widthCovered, (this.panelSize.Height - child.DesiredSize.Height) / 2, childScale, duration);
                        widthCovered += child.DesiredSize.Width * childScale;
                    }
                    else
                    {
                        this.AnimateChild(child, (this.panelSize.Width - child.DesiredSize.Width) / 2, heightCovered, childScale, duration);
                        heightCovered += child.DesiredSize.Height * childScale;
                    }
                }
            }

            this.wasMouseOver = this.isMouseOver;
        }

        /// <summary>
        /// Performs animations and transforms on the child which is passed in.
        /// </summary>
        /// <param name="child">the child on which the animation/transform is to be performed.</param>
        /// <param name="x">TranslateTransform target x value</param>
        /// <param name="y">TranslateTransform target y value</param>
        /// <param name="s">ScaleTransform target value</param>
        /// <param name="duration">duration for which the animation must run</param>
        private void AnimateChild(UIElement child, double x, double y, double s, double duration)
        {
            TransformGroup transformGroup = (TransformGroup)child.RenderTransform;
            ScaleTransform scaleTransform = (ScaleTransform)transformGroup.Children[0];
            TranslateTransform translateTransform = (TranslateTransform)transformGroup.Children[1];

            if (duration == 0)
            {
                translateTransform.X = x;
                translateTransform.Y = y;
                scaleTransform.ScaleX = s;
                scaleTransform.ScaleY = s;
                this.Animation_Completed(null, null);
            }
            else
            {
                this.StartAnimation(child, "(UIElement.RenderTransform).(TransformGroup.Children)[1].(X)", this.GetAnimation(x, duration, this.Animation_Completed));
                this.StartAnimation(child, "(UIElement.RenderTransform).(TransformGroup.Children)[1].(Y)", this.GetAnimation(y, duration, this.Animation_Completed));
                this.StartAnimation(child, "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleX)", this.GetAnimation(s, duration, this.Animation_Completed));
                this.StartAnimation(child, "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleY)", this.GetAnimation(s, duration, this.Animation_Completed));
            }
        }

        /// <summary>
        /// Creates the DoubleAnimation
        /// </summary>
        /// <param name="to">To property value for the DoubleAnimation</param>
        /// <param name="duration">Duration property value for the DoubleAnimation</param>
        /// <param name="endEvent">Animation completed event</param>
        /// <returns>a DoubleAnimation</returns>
        private DoubleAnimation GetAnimation(double to, double duration, EventHandler endEvent)
        {
            DoubleAnimation dblAnimation = new DoubleAnimation();
            dblAnimation.To = to;
            dblAnimation.Duration = TimeSpan.FromMilliseconds(duration);
            if (endEvent != null)
            {
                dblAnimation.Completed += endEvent;
            }

            return dblAnimation;
        }

        /// <summary>
        /// Occurs after the DoubleAnimation is completed. 
        /// </summary>
        /// <param name="sender">the DoubleAnimation object</param>
        /// <param name="e">event args</param>
        private void Animation_Completed(object sender, EventArgs e)
        {
            this.isAnimating = false;
        }

        /// <summary>
        /// Creates the storyboard for the DoubleAnimation and begins the animation.
        /// </summary>
        /// <param name="obj">the child of the panel which is to be animated</param>
        /// <param name="propertyPath">the property on which the animation is applied.</param>
        /// <param name="animation">the DoubleAnimation object</param>
        private void StartAnimation(UIElement obj, string propertyPath, DoubleAnimation animation)
        {
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            Storyboard.SetTarget(storyboard, obj);
            Storyboard.SetTargetProperty(storyboard, new PropertyPath(propertyPath));
            storyboard.Begin();
        }

        #endregion
    }
}
