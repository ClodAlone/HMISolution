#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

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
        bool IsPrevUpdate,IsNeedUpdate,IsAnimationCompleted=true;
        internal int selecteditem = 1;

        public LoopItemsPanel()
        {
            animationSnap = new DoubleAnimation
            {
                EnableDependentAnimation = true,                               
                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
            };
            storyboard = new Storyboard();
            storyboard.Children.Add(animationSnap);
             
            this.ManipulationMode = (ManipulationModes.TranslateX | ManipulationModes.TranslateInertia);
            
            this.ManipulationDelta += OnManipulationDelta;
            this.ManipulationCompleted += LoopItemsPanel_ManipulationCompleted;
            this.ManipulationStarted += LoopItemsPanel_ManipulationStarted;
            Loaded += LoopItemsPanel_Loaded;
            //this.ManipulationInertiaStarting += LoopItemsPanel_ManipulationInertiaStarting;
            // this.Tapped += OnTapped;

            sliderVertical = new Slider
            {
                SmallChange = 0.0000000001,
                Minimum = double.MinValue,
                Maximum = double.MaxValue,
                StepFrequency = 0.0000000001
            };
            Storyboard.SetTarget(animationSnap, sliderVertical);
            Storyboard.SetTargetProperty(animationSnap, "Value");
            storyboard.Completed += storyboard_Completed;
        }

        void LoopItemsPanel_Loaded(object sender, RoutedEventArgs e)
        {
            schedule.Prev_Button.Click -= Prev_Button_Click;
            schedule.Next_Button.Click -= Next_Button_Click;
            schedule.Prev_Button.Click += Prev_Button_Click;
            schedule.Next_Button.Click += Next_Button_Click;
        }

        void Next_Button_Click(object sender, RoutedEventArgs e)
        {
            if (schedule.calendarPopup != null)
                schedule.calendarPopup.IsOpen = false;
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
                this.UpdatePositions(-itemWidth);
                schedule.UpdateFirstItem(selecteditem);
            }
        }

        void Prev_Button_Click(object sender, RoutedEventArgs e)
        {
            if (schedule.calendarPopup != null)
                schedule.calendarPopup.IsOpen = false;
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
                this.UpdatePositions(itemWidth);
                schedule.UpdateLastItem(selecteditem);
            }
        }

        void LoopItemsPanel_ManipulationInertiaStarting(object sender, ManipulationInertiaStartingRoutedEventArgs e)
        {
            e.Handled = true;
        }

        void LoopItemsPanel_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            schedule.ClearContextmenu();           
        }

        void LoopItemsPanel_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            appendedvalue = 0;           
            if (e == null || !schedule.ScrollManipulationCompleted || e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
                return;

            var translation = e.Cumulative.Translation;

            if ((translation.X > 0 && schedule.exceedsMinDate) || (translation.X < 0 && schedule.exceedsMaxDate))
                return;

            double newoffset = (itemWidth) - (Math.Abs(finaloffset));
            if (newoffset != 0)
            {
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

                            IsPrevUpdate = true;
                        }
                        var child = Children[selecteditem];
                        var rect = child.TransformToVisual(this).TransformBounds(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));
                        ScrollToSelectedIndex(child, rect);
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
                            IsPrevUpdate = false;
                        }
                        var child = Children[selecteditem];
                        var rect = child.TransformToVisual(this).TransformBounds(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));
                        ScrollToSelectedIndex(child, rect);
                    }
                }
                else
                {
                    IsNeedUpdate = false;
                    animationDuration = TimeSpan.FromMilliseconds(400);
                    UIElement child = Children[selecteditem];
                    var rect = child.TransformToVisual(this).TransformBounds(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));
                    ScrollToSelectedIndex(child, rect);
                }
            }
            e.Handled = true;
            schedule.ScrollManipulationCompleted = false;
            schedule.ClearContextmenu();            
        }

        private void OnVerticalOffsetChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            this.UpdatePositions(e.NewValue - e.OldValue);
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
        private void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e == null || !schedule.ScrollManipulationCompleted || e.PointerDeviceType!= Windows.Devices.Input.PointerDeviceType.Touch)
                return;
           
            var translation = e.Delta.Translation;
            inertiavalue = e.Delta.Translation.X;

            if ((translation.X > 0 && schedule.exceedsMinDate) || (translation.X < 0 && schedule.exceedsMaxDate))
                return;

            if (e.IsInertial)
            {
                #region oldImplementaion
                if (Math.Abs(e.Velocities.Linear.X) > 1 && Math.Abs(appendedvalue) < itemWidth)
                {
                    animationDuration = TimeSpan.FromMilliseconds((itemWidth/2 ) / Math.Abs(e.Velocities.Linear.X));                   
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
                        IsPrevUpdate = true;
                        var child = Children[selecteditem];
                        var rect = child.TransformToVisual(this).TransformBounds(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));
                        IsAnimationCompleted = false;
                        ScrollToSelectedIndex(child, rect);
                        
                        appendedvalue = itemWidth;
                        schedule.ScrollManipulationCompleted = false;
                        e.Complete();
                    }
                    else if(appendedvalue >0)
                    {
                        if (selecteditem == 0)
                        {
                            selecteditem = 2;
                        }
                        else
                        {
                            selecteditem--;
                        }
                        IsPrevUpdate = false;                      
                        var child=Children[selecteditem];
                        var rect = child.TransformToVisual(this).TransformBounds(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));
                        IsAnimationCompleted = false;
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
            if (IsAnimationCompleted)
            {
                if (!e.IsInertial && Math.Abs(appendedvalue) < itemWidth)
                {
                    appendedvalue = appendedvalue + translation.X / 2;
                    this.UpdatePositions(translation.X / 2);
                }
                else if (Math.Abs(appendedvalue) > itemWidth)
                {
                    schedule.ScrollManipulationCompleted = false;
                }
            }
            else
            {
                schedule.ScrollManipulationCompleted = false;
                e.Complete();
            }
        }

        private  void TriggerNextUpdate()
        {
            if (schedule.exceedsMaxDate)
                schedule.exceedsMaxDate = false;
            if (!schedule.exceedsMinDate)
                schedule.UpdateLastItem(selecteditem);
        }

        private  void TriggerPrevUpdate()
        {
            if (schedule.exceedsMinDate)
                schedule.exceedsMinDate = false;
            if (!schedule.exceedsMaxDate)
               schedule.UpdateFirstItem(selecteditem);
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

            var deltaOffset = centerTopOffset - rect.X;

            this.UpdatePositionsWithAnimation(compositeTransform.X, compositeTransform.X + deltaOffset);

        }

        /// <summary>
        /// Updating with an animation (after a tap)
        /// </summary>
        private void UpdatePositionsWithAnimation(Double fromOffset, Double toOffset)
        {
            animationSnap.From = fromOffset;
            animationSnap.To = toOffset;
            animationSnap.Duration = animationDuration;
            sliderVertical.ValueChanged -= OnVerticalOffsetChanged;
            sliderVertical.Value = fromOffset;
            sliderVertical.ValueChanged += OnVerticalOffsetChanged;                
            storyboard.Begin();            
        }

        void storyboard_Completed(object sender, object e)
        {
            if (schedule.calendarPopup != null)
                schedule.calendarPopup.IsOpen = false;
            if (IsNeedUpdate)
            {
                if (IsPrevUpdate)
                {
                    TriggerPrevUpdate();
                }
                else
                {
                    TriggerNextUpdate();
                }
            }
            IsAnimationCompleted = true;
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

            //ObservableCollection<UIElement> childCollection = new ObservableCollection<UIElement>();
            //if (selecteditem != 1)
            //{
            //    if (selecteditem == 0)
            //    {
            //        childCollection.Add(Children[2]);
            //        childCollection.Add(Children[0]);
            //        childCollection.Add(Children[1]);
            //    }
            //    else
            //    {
            //        childCollection.Add(Children[1]);
            //        childCollection.Add(Children[2]);
            //        childCollection.Add(Children[0]);
            //    }
            //}
            //else
            //{
            //    childCollection.Add(Children[0]);
            //    childCollection.Add(Children[1]);
            //    childCollection.Add(Children[2]);
            //}
                       
            // Must Create looping items count
            //foreach (UIElement item in childCollection)
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
#if WINRT
            if (double.IsInfinity(availableSize.Height) || availableSize.Height < MinHeight)
                availableSize.Height = MinHeight;
            if (double.IsInfinity(availableSize.Width) || availableSize.Width < MinWidth)
                availableSize.Width = MinWidth; 
#endif
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
            return (availableSize);
        }



    }
}
