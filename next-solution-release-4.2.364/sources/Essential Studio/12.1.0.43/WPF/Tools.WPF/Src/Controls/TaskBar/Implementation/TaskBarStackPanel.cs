// <copyright file="TaskBarStackPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Overrides standard <see cref="StackPanel"/> control and extends its functional possibilities.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TaskBarStackPanel : StackPanel
    {
        internal static double prevHeight = 0.0;

        #region Event

        /// <summary>
        /// Event that is raised when <see cref="SafeHeight"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback SafeHeightChanged;

        /// <summary>
        /// Event that is raised when <see cref="AnimationInProgress"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback AnimationInProgressChanged;

        #endregion Event

        #region Properties

        /// <summary>
        /// Gets or sets the value that represents height of the expander. Used in animation.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// Value that represents height of the expander.
        /// </value>
        public double SafeHeight
        {
            get
            {
                return (double)GetValue(SafeHeightProperty);
            }

            set
            {
                SetValue(SafeHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [animation in progress].
        /// </summary>
        /// <value><c>true</c> if [animation in progress]; otherwise, <c>false</c>.</value>
        private bool AnimationInProgress
        {
            get
            {
                return (bool)GetValue(AnimationInProgressProperty);
            }

            set
            {
                SetValue(AnimationInProgressProperty, value);
            }
        }

        #endregion Properties

        #region Implementation

        /// <summary>
        /// Method is called when size of the window changed
        /// </summary>
        public new void SizeChanged()
        {
            this.InvalidateMeasure();
        }

        /// <summary>
        /// Measures the size in layout required
        /// for child elements and determines a size for the System.Windows.FrameworkElement-derived class.
        /// Changes <see cref="SafeHeight"/> for animation .
        /// </summary>
        /// <param name="constraint">The available size that this element can give to child elements.
        /// Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its
        /// calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            UIElementCollection collection = base.InternalChildren;
            Size returnSize = new Size();
            Size givenToElementSize = constraint;
            double heightval = 0.0;
            returnSize.Width = constraint.Width;
            givenToElementSize.Height = double.PositiveInfinity;
            double widthMax = 0.0;

            foreach (UIElement element in collection)
            {
                element.Measure(givenToElementSize);
                Size elementSize = element.DesiredSize;

                returnSize.Height += elementSize.Height;

                prevHeight = returnSize.Height;

                if (widthMax < elementSize.Width)
                {
                    widthMax = elementSize.Width;
                }
            }

            // Makes the last task item to fill the remaining vertical space

            #region region last task bar item height

            TaskBar parentBar = VisualUtils.FindAncestor(this, typeof(TaskBar)) as TaskBar;
            TaskBarItem lastItem = null;
            if (parentBar != null)
            {
                lastItem = parentBar.Items[parentBar.Items.Count - 1] as TaskBarItem;

                TaskBarItem currentItem = VisualUtils.FindAncestor(this, typeof(TaskBarItem)) as TaskBarItem;
                Thickness itemMargin = new Thickness(0);

                if (double.IsNaN(currentItem.Height))
                {
                    if (parentBar.LastChildFill)
                    {
                        if (!(double.IsNaN(parentBar.Height)))
                        {
                            if (parentBar.GroupMargin != null)
                            {
                                itemMargin = parentBar.GroupMargin;
                            }
                            if (lastItem == currentItem)
                            {
                                double d = 0;
                                foreach (TaskBarItem item in parentBar.Items)
                                {
                                    if (item != currentItem)
                                    {
                                        d += item.DesiredSize.Height;
                                    }
                                }

                                double height = 0;
                                if (parentBar.GroupOrientation == Orientation.Vertical)
                                {
                                    if (parentBar.ActualHeight > d && lastItem.DesiredSize.Height < parentBar.ActualHeight)
                                        height = (parentBar.ActualHeight - d);
                                }
                                else
                                {
                                    height = parentBar.ActualHeight;
                                }

                                if (height < 0)
                                {
                                    returnSize.Height = 0;
                                }
                                else
                                {
                                    if (height != 0.00)
                                    {
                                        if (height > (itemMargin.Bottom))
                                        {
                                            returnSize.Height = (height - (itemMargin.Bottom));
                                        }
                                        else
                                        {
                                            returnSize.Height = ((itemMargin.Bottom) - height);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            FrameworkElement elem = parentBar.Parent as FrameworkElement;
                            if (elem != null)
                            {
                                bool flag = true;
                                do
                                {
                                    if (!(double.IsNaN(elem.Height)))
                                    {
                                        flag = false;
                                    }
                                    else
                                    {
                                        elem = elem.Parent as FrameworkElement;
                                        if (elem == null)
                                        {
                                            flag = false;
                                        }
                                    }
                                } while (flag);

                                if (elem != null)
                                {
                                    if (!(double.IsNaN(elem.Height)))
                                    {
                                        if (parentBar.GroupMargin != null)
                                        {
                                            itemMargin = parentBar.GroupMargin;
                                        }
                                        if (lastItem == currentItem)
                                        {
                                            double d = 0;
                                            foreach (TaskBarItem item in parentBar.Items)
                                            {
                                                if (item != currentItem)
                                                {
                                                    d += item.DesiredSize.Height;
                                                }
                                            }

                                            if (parentBar.GroupOrientation == Orientation.Vertical)
                                            {
                                                if (elem.ActualHeight > d && lastItem.DesiredSize.Height < elem.ActualHeight)
                                                    heightval = elem.ActualHeight - d;
                                            }
                                            else
                                            {
                                                heightval = elem.ActualHeight;
                                            }

                                            if (heightval < 0)
                                            {
                                                returnSize.Height = 0;
                                            }
                                            else
                                            {
                                                if (heightval != 0.00)
                                                {
                                                    if (heightval > (itemMargin.Bottom))
                                                    {
                                                        returnSize.Height = (heightval - (itemMargin.Bottom));
                                                    }
                                                    else
                                                    {
                                                        returnSize.Height = ((itemMargin.Bottom) - heightval);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    returnSize.Height = currentItem.Height;
                }
            }

            #endregion region last task bar item height

            if (double.IsInfinity(constraint.Width))
            {
                returnSize.Width = widthMax;
            }

            if (parentBar.LastChildFill)
            {
                if (returnSize.Height > prevHeight)
                {
                    SafeHeight = returnSize.Height;
                }
                else
                    SafeHeight = prevHeight;
                AnimationInProgress = true;
            }

            ValueSource source = DependencyPropertyHelper.GetValueSource(this, SafeHeightProperty);

            if (source.IsAnimated)
            {
                if (this.Parent is ScrollViewer)
                {
                    ExpanderExt exp = (ExpanderExt)((ScrollViewer)this.Parent).Parent;

                    if (null != exp)
                    {
                        if (SafeHeight < returnSize.Height)
                        {
                            SafeHeight = returnSize.Height;
                        }
                    }

                    if (exp.IsExpanded)
                    {
                        if (returnSize.Height > prevHeight)
                        {
                            SafeHeight = returnSize.Height;
                        }
                        else
                            SafeHeight = prevHeight;
                    }

                    if (!exp.IsExpanded)
                    {
                        AnimationInProgress = true;
                    }
                    else if (returnSize.Height == SafeHeight)
                    {
                        AnimationInProgress = false;
                    }
                }
            }

            if (AnimationInProgress)
            {
                returnSize.Height = SafeHeight;
            }

            return returnSize;
        }

        /// <summary>
        /// This method defines <see cref="SafeHeight"/> property value.
        /// </summary>
        /// <param name="sizeInfo">Information about size changing.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            if (!AnimationInProgress)
            {
                SafeHeight = DesiredSize.Height;
            }

            base.OnRenderSizeChanged(sizeInfo);
        }

        /// <summary>
        /// Calls OnSafeHeightChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnSafeHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TaskBarStackPanel instance = (TaskBarStackPanel)d;
            instance.OnSafeHeightChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="SafeHeightChanged"/>
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnSafeHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SafeHeightChanged != null)
            {
                SafeHeightChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnAnimationInProgressChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnAnimationInProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TaskBarStackPanel instance = (TaskBarStackPanel)d;
            instance.OnAnimationInProgressChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="AnimationInProgressChanged"/>
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnAnimationInProgressChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AnimationInProgressChanged != null)
            {
                AnimationInProgressChanged(this, e);
            }
        }

        #endregion Implementation

        #region Dependency Properties

        /// <summary>
        /// Identifies <see cref="SafeHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SafeHeightProperty = DependencyProperty.Register("SafeHeight", typeof(Double), typeof(TaskBarStackPanel), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnSafeHeightChanged));

        /// <summary>
        /// Identifies <see cref="AnimationInProgress"/> dependency property.
        /// </summary>
        private static readonly DependencyProperty AnimationInProgressProperty = DependencyProperty.Register("AnimationInProgress", typeof(Boolean), typeof(TaskBarStackPanel), new UIPropertyMetadata(false, OnAnimationInProgressChanged));

        #endregion Dependency Properties
    }
}