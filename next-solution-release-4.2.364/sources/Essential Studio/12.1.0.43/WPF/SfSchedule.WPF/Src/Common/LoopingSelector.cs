#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Syncfusion.UI.Xaml.Schedule
{  
    public class LoopItemsPanel : Panel
    {
        // hack to animate a value easy
        private Slider sliderVertical;
        private TimeSpan animationDuration = TimeSpan.FromMilliseconds(400);
        // separating offset
        private double offsetSeparator;
        
        private double finaloffset;
        private double appendedvalue=0;
        private double inertiavalue;
        double Maxwidth;
        // item height. must be 1d to fire first arrangeoverride
        private double itemHeight = 1d;

        // item height. must be 1d to fire first arrangeoverride
        private double itemWidth = 1d;

        // true when arrange override is ok
        private bool templateApplied;
        private SfSchedule schedule;
        DoubleAnimation animationSnap;
        Storyboard storyboard;
        bool IsPrevUpdate, IsNeedUpdate;
        int _selecteditem = 1;
        internal int selecteditem
        {
            get { return _selecteditem; }
            set { _selecteditem = value; }
        }

        public LoopItemsPanel()
        {
            animationSnap = new DoubleAnimation
            {                
                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
            };
            storyboard = new Storyboard();
            storyboard.Children.Add(animationSnap);
            this.ManipulationDelta += OnManipulationDelta;
            this.ManipulationCompleted += LoopItemsPanel_ManipulationCompleted;
            this.ManipulationStarted += LoopItemsPanel_ManipulationStarted;
            Loaded += LoopItemsPanel_Loaded;
            

            sliderVertical = new Slider
            {
                SmallChange = 0.0000000001,
                Minimum = double.MinValue,
                Maximum = double.MaxValue,
                TickFrequency = 0.0000000001
            };

            Storyboard.SetTarget(animationSnap, sliderVertical);
            Storyboard.SetTargetProperty(animationSnap, new PropertyPath("Value"));
            storyboard.Completed += storyboard_Completed;

        }

        void storyboard_Completed(object sender, EventArgs e)
        {
            if (IsNeedUpdate)
            {
                if (IsPrevUpdate)
                {
                    if (schedule.exceedsMinDate)
                        schedule.exceedsMinDate = false;
                    if (!schedule.exceedsMaxDate)
                        schedule.UpdateFirstItem(selecteditem);
                }
                else
                {
                    if (schedule.exceedsMaxDate)
                        schedule.exceedsMaxDate = false;
                    if (!schedule.exceedsMinDate)
                        schedule.UpdateLastItem(selecteditem);
                }
            }
        }        

        private void OnVerticalOffsetChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.UpdatePositions(e.NewValue - e.OldValue);
        }

        private void LoopItemsPanel_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            schedule.ScrollManipulationCompleted = true;
        }

        private void LoopItemsPanel_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            appendedvalue = 0;
            if ((e == null || !schedule.ScrollManipulationCompleted) || !schedule.EnableTouch || e.TotalManipulation.Translation.X==0)
            {               
                return;                
            }
            var totalTranslation= e.TotalManipulation.Translation;
            if ((totalTranslation.X > 0 && schedule.exceedsMinDate) || (totalTranslation.X < 0 && schedule.exceedsMaxDate))
            {
                return;
            }

            var translation = e.TotalManipulation.Translation;
           
            double newoffset = (itemWidth) - (Math.Abs(finaloffset));
            if (newoffset != 0)
                if (Math.Abs(finaloffset) > (itemWidth) / 2)
                {
                    IsNeedUpdate = true;
                    if (finaloffset < 0)
                    {


                        if (newoffset != 0)
                        {
                            if (selecteditem == 2)
                            {
                                selecteditem = 0;
                            }
                            else
                            {
                                selecteditem++;
                            }                           
                            schedule.currentSelectedItem = (Children[selecteditem] as Grid).Children[1] as ContentControl;
                            IsPrevUpdate = true;
                        }
                        var child = Children[selecteditem];
                        var rect = child.TransformToVisual(this).TransformBounds(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));
                        ScrollToSelectedIndex(child, rect);
                        schedule.currentSelectedItem = (Children[selecteditem] as Grid).Children[1] as ContentControl;
                    }
                    else
                    {
                        if (newoffset != 0)
                        {
                            if (selecteditem == 0)
                            {
                                selecteditem = 2;
                            }
                            else
                            {
                                selecteditem--;
                            }
                            schedule.currentSelectedItem = (Children[selecteditem] as Grid).Children[1] as ContentControl;
                            IsPrevUpdate = false;
                        }
                        var child = Children[selecteditem];
                        var rect = child.TransformToVisual(this).TransformBounds(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));
                        ScrollToSelectedIndex(child, rect);
                        schedule.currentSelectedItem = (Children[selecteditem] as Grid).Children[1] as ContentControl;
                    }
                }
                else
                {
                    IsNeedUpdate = false;
                    animationDuration = TimeSpan.FromMilliseconds(400);
                    UIElement child = Children[selecteditem];
                    var rect = child.TransformToVisual(this).TransformBounds(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));
                    ScrollToSelectedIndex(child, rect);
                    schedule.currentSelectedItem = (Children[selecteditem] as Grid).Children[1] as ContentControl;
                    
                }
            e.Handled = true;
            schedule.ScrollManipulationCompleted = false;
            
        }

        

        void LoopItemsPanel_Loaded(object sender, RoutedEventArgs e)
        {
            Binding manipulationBinding = new Binding();
            manipulationBinding.Source = schedule;
            manipulationBinding.Path = new PropertyPath("EnableTouch");
            manipulationBinding.Mode = BindingMode.OneWay;
            BindingOperations.SetBinding(this, UIElement.IsManipulationEnabledProperty, manipulationBinding);
            schedule.Prev_Button.Click -= Prev_Button_Click;
            schedule.Next_Button.Click -= Next_Button_Click;
            schedule.Prev_Button.Click += Prev_Button_Click;
            schedule.Next_Button.Click += Next_Button_Click;
        }

       

        void Next_Button_Click(object sender, RoutedEventArgs e)
        {
            MoveToNext();
           
        }

        internal void MoveToNext()
        {
            if (schedule.exceedsMinDate)
                schedule.exceedsMinDate = false;
            if (!schedule.exceedsMaxDate)
            {
                if (selecteditem == 2)
                {
                    selecteditem = 0;
                }
                else
                {
                    selecteditem++;
                }

                schedule.ClearContextmenu();
                IsNeedUpdate = true;
                if (schedule.EnableTouch)
                    IsPrevUpdate = true;
                else
                    schedule.UpdateNextItem();
                var child = Children[selecteditem];
                var rect = child.TransformToVisual(this).TransformBounds(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));
                ScrollToSelectedIndex(child, rect, -itemWidth);
                schedule.currentSelectedItem = (Children[selecteditem] as Grid).Children[1] as ContentControl;
            }
        }

        void Prev_Button_Click(object sender, RoutedEventArgs e)
        {
            MoveToPrev();
            
        }

        internal void MoveToPrev()
        {   
            if (schedule.exceedsMaxDate)
                schedule.exceedsMaxDate = false;
            if (!schedule.exceedsMinDate)
            {
                if (selecteditem == 0)
                {
                    selecteditem = 2;
                }
                else
                {
                    selecteditem--;
                }

                schedule.ClearContextmenu();
                IsNeedUpdate = true;
                if (schedule.EnableTouch)
                    IsPrevUpdate = false;
                else
                    schedule.UpdatePrevItem();
                var child = Children[selecteditem];
                var rect = child.TransformToVisual(this).TransformBounds(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));
                ScrollToSelectedIndex(child, rect, itemWidth);
                schedule.currentSelectedItem = (Children[selecteditem] as Grid).Children[1] as ContentControl;
            }
        }
        
        /// <summary>
        /// Get Items Source count items
        /// </summary>
        /// <returns></returns>
        private int GetItemsCount()
        {
            return this.Children != null ? this.Children.Count : 0;
        }



        /// <summary>
        /// On manipulation delta
        /// </summary>
        private void OnManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            if ((e == null || !schedule.ScrollManipulationCompleted) || !schedule.EnableTouch || e.Velocities.LinearVelocity.X==0)
            {
                return;
            }
            var translation = e.DeltaManipulation.Translation;
            inertiavalue = e.DeltaManipulation.Translation.X;

            if ((translation.X > 0 && schedule.exceedsMinDate) || (translation.X < 0 && schedule.exceedsMaxDate))
                return;

            if (e.IsInertial)
            {
                #region oldImplementaion
                if (Math.Abs(e.Velocities.LinearVelocity.X) > 1 && Math.Abs(appendedvalue) < itemWidth)
                {
                    animationDuration = TimeSpan.FromMilliseconds((itemWidth / 2) / Math.Abs(e.Velocities.LinearVelocity.X));
                    IsNeedUpdate = true;
                    if (appendedvalue < 0)
                    {
                        if (selecteditem == 2)
                        {
                            selecteditem = 0;
                        }
                        else
                        {
                            selecteditem++;
                        }
                        schedule.currentSelectedItem = (Children[selecteditem] as Grid).Children[1] as ContentControl;
                        IsPrevUpdate = true;
                        var child = Children[selecteditem];
                        var rect = child.TransformToVisual(this).TransformBounds(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));
                        ScrollToSelectedIndex(child, rect);                      
                        schedule.ScrollManipulationCompleted = false;
                        appendedvalue = itemWidth;
                        e.Complete();
                    }
                    else
                    {
                        if (selecteditem == 0)
                        {
                            selecteditem = 2;
                        }
                        else
                        {
                            selecteditem--;
                        }
                        schedule.currentSelectedItem = (Children[selecteditem] as Grid).Children[1] as ContentControl;
                        IsPrevUpdate = false;
                        var child = Children[selecteditem];
                        var rect = child.TransformToVisual(this).TransformBounds(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));
                        ScrollToSelectedIndex(child, rect);                           
                        appendedvalue = itemWidth;
                        schedule.ScrollManipulationCompleted = false;
                        e.Complete();
                    }
                }
                else
                {
                    e.Complete();
                }
                #endregion
            }

            if (!e.IsInertial && Math.Abs(appendedvalue) < itemWidth)
            {
                if (schedule.ScheduleType == ScheduleType.Month)
                {
                    appendedvalue = appendedvalue + translation.X;
                    this.UpdatePositions(translation.X);
                }
                else
                {
                    appendedvalue = appendedvalue + translation.X/2;
                    this.UpdatePositions(translation.X/2);
                }
            }
            else if (Math.Abs(appendedvalue) > itemWidth)
            {
                schedule.ScrollManipulationCompleted = false;
            }
        }

        protected override void OnManipulationInertiaStarting(System.Windows.Input.ManipulationInertiaStartingEventArgs e)
        {
            if (e.InitialVelocities.LinearVelocity.X == 0)
            {
                schedule.currentSelectedItem = (Children[selecteditem] as Grid).Children[1] as ContentControl;
                var child = Children[selecteditem];
                var rect = child.TransformToVisual(this).TransformBounds(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));
                IsNeedUpdate = false;
                ScrollToSelectedIndex(child, rect);
                e.Cancel();
            }
            e.TranslationBehavior.DesiredDeceleration = 40.0 * 96.0 / (1000.0 * 1000.0);
            base.OnManipulationInertiaStarting(e);
        }
      
        private void ScrollToSelectedIndex(UIElement selectedItem, Rect rect)
        {
            if (!templateApplied)
                return;

            // Apply Transform
            TranslateTransform compositeTransform = (TranslateTransform)selectedItem.RenderTransform;

            if (compositeTransform == null)
                return;

            var centerTopOffset = (this.ActualWidth / 2d) - (itemWidth) / 2d;
            var deltaOffset = centerTopOffset - rect.X; ;

            this.UpdatePositionsWithAnimation(compositeTransform.X, compositeTransform.X + deltaOffset);

        }

        private void ScrollToSelectedIndex(UIElement selectedItem, Rect rect, double offset)
        {
            if (!templateApplied)
                return;

            // Apply Transform
            TranslateTransform compositeTransform = (TranslateTransform)selectedItem.RenderTransform;

            if (compositeTransform == null)
                return;


            this.UpdatePositionsWithAnimation(compositeTransform.X, compositeTransform.X + offset);

        }

        /// <summary>
        /// Updating with an animation (after a tap)
        /// </summary>
        private void UpdatePositionsWithAnimation(Double fromOffset, Double toOffset)
        {
            storyboard.Children.Clear();
            sliderVertical = new Slider
            {
                SmallChange = 0.0000000001,
                Minimum = double.MinValue,
                Maximum = double.MaxValue,
                TickFrequency = 0.0000000001
            };
            var animationSnap = new DoubleAnimation
            {
                From = fromOffset,
                To = toOffset,
                Duration = animationDuration,
                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseInOut }
            };            
            storyboard.Children.Add(animationSnap);
            sliderVertical.ValueChanged -= OnVerticalOffsetChanged;
            sliderVertical.Value = fromOffset;            
            sliderVertical.ValueChanged += OnVerticalOffsetChanged;
            Storyboard.SetTarget(animationSnap, sliderVertical);
            Storyboard.SetTargetProperty(animationSnap, new PropertyPath("Value"));            
            storyboard.Begin();
        }       

        /// <summary>
        /// Updating position
        /// </summary>
        private void UpdatePositions(Double offsetDelta)
        {
            Double maxLogicalWidth = this.GetItemsCount() * itemWidth;

            // Reaffect correct offsetSeparator
            this.offsetSeparator = (this.offsetSeparator + offsetDelta) % maxLogicalWidth;
            if (Math.Abs(finaloffset) < itemWidth)
            {
                this.finaloffset = finaloffset + offsetDelta;
            }
            else
            {
                this.finaloffset = offsetDelta;
            }           
            
            Int32 itemNumberSeparator = (Int32)(Math.Abs(this.offsetSeparator) / itemWidth);

            Int32 itemIndexChanging;
            Double offsetAfter;
            Double offsetBefore;

            if (this.offsetSeparator > 0)
            {
                itemIndexChanging = this.GetItemsCount() - itemNumberSeparator - 1;
                offsetAfter = this.offsetSeparator;

                if (this.offsetSeparator % maxLogicalWidth == 0)
                    itemIndexChanging++;

                offsetBefore = offsetAfter - maxLogicalWidth;
            }
            else
            {
                itemIndexChanging = itemNumberSeparator;
                offsetBefore = this.offsetSeparator;
                offsetAfter = maxLogicalWidth + offsetBefore;
            }

            // items that must be before
            this.UpdatePosition(itemIndexChanging, this.GetItemsCount(), offsetBefore);

            // items that must be after
            this.UpdatePosition(0, itemIndexChanging, offsetAfter);
        }

        /// <summary>
        /// Translate items to a new offset
        /// </summary>
        private void UpdatePosition(Int32 startIndex, Int32 endIndex, Double offset)
        {
            if (this.Children.Count > 0)
            {
                for (Int32 i = startIndex; i < endIndex; i++)
                {
                    UIElement loopListItem = this.Children[i];

                    // Apply Transform
                    TranslateTransform compositeTransform = (TranslateTransform)loopListItem.RenderTransform;

                    if (compositeTransform == null)
                        continue;
                    compositeTransform.X = offset;


                }
            }
        }

        /// <summary>
        /// Arrange all items
        /// </summary>
        protected override Size ArrangeOverride(Size finalSize)
        {
            // Clip to ensure items dont override container
            this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, finalSize.Width, finalSize.Height) };
            
            Double positionLeft = -Maxwidth;
           
            
            // Must Create looping items count
            foreach (UIElement item in Children)
            {
                if (item == null)
                    continue;

                Size desiredSize = item.DesiredSize;

                if (double.IsNaN(desiredSize.Width) || double.IsNaN(desiredSize.Height)) continue;

                // Get rect position
                var rect = new Rect(positionLeft, 0, Maxwidth, desiredSize.Height);
                item.Arrange(rect);

                // set internal CompositeTransform to handle movement
                TranslateTransform compositeTransform = new TranslateTransform();
                item.RenderTransform = compositeTransform;

                positionLeft += Maxwidth;
            }

            templateApplied = true;
            if (selecteditem != 1)
            {
                if (selecteditem == 0)
                {
                    UpdatePositions(Maxwidth - offsetSeparator);
                }
                else
                {
                    UpdatePositions(-Maxwidth - offsetSeparator);
                }
            }
            
            return finalSize;
        }

        /// <summary>
        /// Measure items 
        /// </summary>
        protected override Size MeasureOverride(Size availableSize)
        {

            Size s = base.MeasureOverride(availableSize);
            if (double.IsInfinity(availableSize.Height) || availableSize.Height < MinHeight)
            {
                Height = MinHeight;
                availableSize.Height = MinHeight;
            }
            if (double.IsInfinity(availableSize.Width) || availableSize.Width < MinWidth)
            {
                Width = MinWidth;
                availableSize.Width = MinWidth;
            }
            schedule = this.FindParentElementOfType<SfSchedule>();
           
            // set good clipping
            this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, availableSize.Width, availableSize.Height) };
             Maxwidth = 0;
            // Measure all items
            foreach (UIElement container in this.Children)
            {
                container.Measure(availableSize);

                if (itemHeight != container.DesiredSize.Height)
                    itemHeight = container.DesiredSize.Height;

                Maxwidth = Maxwidth > container.DesiredSize.Width ? Maxwidth : container.DesiredSize.Width;
            }
            if (!(Maxwidth < availableSize.Width))
            {
                availableSize.Width = Maxwidth;
                itemWidth = Maxwidth;
            }
            else
            {
                itemWidth = availableSize.Width;
                Maxwidth = availableSize.Width;
            }
            if (double.IsInfinity(availableSize.Height))
            {
                availableSize.Height = itemHeight;
            }
            return (availableSize);
        }



    }
}
