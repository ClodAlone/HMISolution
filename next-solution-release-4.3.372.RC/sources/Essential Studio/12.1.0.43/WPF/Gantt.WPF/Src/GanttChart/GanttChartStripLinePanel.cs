#region Copyright Syncfusion Inc. 2001 - 2014
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
using System.Windows.Controls;
using Syncfusion.Windows.Controls.Gantt.Schedule;
using System.Windows;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Data;

namespace Syncfusion.Windows.Controls.Gantt.Chart
{
    /// <summary>
    /// Interactive code for stripline in chart region.
    /// </summary>
    public class GanttChartStripLinePanel : VirtualizingPanel
    {
        #region Private Members

        DateTime _startTime = DateTime.Now.AddDays(-15);
        DateTime _endTime = DateTime.Now.AddDays(45);
        double pos;
        TimeSpan noOfDays;
        double width;
        DateTime StripStart;
        DateTime StripEnd;
#if !SILVERLIGHT
        DataTemplate CustomTemplate;
        Style CustomStyle;
#endif

        #endregion

        #region Internal Members

        /// <summary>
        /// Gets or sets the parent control.
        /// </summary>
        /// <value>The parent control.</value>
        internal GanttChart ParentControl { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>The start time.</value>
        internal DateTime StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                if (_startTime != value)
                {
                    _startTime = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        /// <value>The end time.</value>
        internal DateTime EndTime
        {
            get
            {
                return _endTime;
            }
            set
            {
                if (_endTime != value)
                {
                    _endTime = value;
                }
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Arranges the override.
        /// </summary>
        /// <param name="arrangeSize">Size of the arrange.</param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            foreach (UIElement child in Children)
            {
                StripLine strip = child as StripLine;
                strip.Height = strip.Type == StriplineType.Regular ? arrangeSize.Height : strip.Height;
                strip.Arrange(new Rect(strip.Position, new Size(strip.Width, strip.Height)));
            }
            return base.ArrangeOverride(arrangeSize);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Resets the stripline.
        /// </summary>
        internal void ResetStripline()
        {
            this.Children.Clear();
            AddStripLine();
        }

        /// <summary>
        /// Adds the strip line.
        /// </summary>
        private void AddStripLine()
        {
            bool canAddChild;
            var parent = this.ParentControl.ParentControl as GanttControl;

            if (parent.ShowStripLines && parent.StripLines != null)
            {
                foreach (StripLineInfo stripLineInfo in parent.StripLines)
                {
                    //Whether the user defined invalid StartDate or EndDate the striplines is not drawned.
                    if ((stripLineInfo.StartDate < this.StartTime || stripLineInfo.StartDate > this.EndTime || stripLineInfo.EndDate < this.StartTime || stripLineInfo.EndDate > this.EndTime || stripLineInfo.EndDate < stripLineInfo.StartDate) && stripLineInfo.Type!= StriplineType.Absolute)
                        return;

                    //Stripline in single date without repeating is added here. And Absolute stripline also added here.
                    if (stripLineInfo.RepeatBehavior == Repeat.None || stripLineInfo.Type== StriplineType.Absolute)
                    {
                        StripLine strip = new StripLine();
                        strip = GetStripline(stripLineInfo);

                        StriplineCreated(strip);

                        this.Children.Add(strip);
                    }

                    //Repeating strip added in the following code.
                    else
                    {
                        StripStart = stripLineInfo.StartDate;
                        StripEnd = stripLineInfo.EndDate;
                        while (StripEnd <= this.EndTime && StripEnd <= stripLineInfo.RepeatUpto)
                        {
                            StripLine repeatStrip = new StripLine();
                            repeatStrip = GetStripline(stripLineInfo);
                            repeatStrip.StartDate = StripStart;
                            repeatStrip.EndDate = StripEnd;
                            canAddChild = true;
#if !SILVERLIGHT
                            // Style for stripline can be applied using StyleSelector as user defined
                            this.GetStyle(repeatStrip, stripLineInfo);

                            //ContentTemplate for Stripline can be applied using DataTemplateSelector as user defined.
                            this.GetTemplate(repeatStrip, stripLineInfo);
                            pos = this.ParentControl.GetStartPositionOfDate(StripStart, TimeUnit.Hours);
#else
                            pos = this.ParentControl.GetStartPositionOfDate(StripStart);
#endif
                            repeatStrip.Position=new Point(pos,0);

                            StriplineCreated(repeatStrip);

                            switch (stripLineInfo.RepeatBehavior.ToString())
                            {
                                case "Year":
                                    {
                                        StripStart = StripStart.AddYears(stripLineInfo.RepeatFor);
                                        StripEnd = StripEnd.AddYears(stripLineInfo.RepeatFor);
                                    }
                                    break;
                                case "Month":
                                    {
                                        StripStart = StripStart.AddMonths(stripLineInfo.RepeatFor);
                                        StripEnd = StripEnd.AddMonths(stripLineInfo.RepeatFor);
                                    }
                                    break;
                                case "Week":
                                    {
                                        if (noOfDays.Days > stripLineInfo.RepeatFor * 7)
                                            canAddChild = false;
                                        StripStart = StripStart.AddDays(stripLineInfo.RepeatFor * 7);
                                        StripEnd = StripEnd.AddDays(stripLineInfo.RepeatFor * 7);
                                    }
                                    break;
                                case "Day":
                                    {
                                        if (noOfDays.Days > stripLineInfo.RepeatFor)
                                            canAddChild = false;
                                        StripStart = StripStart.AddDays(stripLineInfo.RepeatFor);
                                        StripEnd = StripEnd.AddDays(stripLineInfo.RepeatFor);
                                    }
                                    break;
#if !SILVERLIGHT
                                case "Hour":
                                    {
                                        if (noOfDays.Hours > stripLineInfo.RepeatFor)
                                            canAddChild = false;
                                        StripStart = StripStart.AddHours(stripLineInfo.RepeatFor);
                                        StripEnd = StripEnd.AddHours(stripLineInfo.RepeatFor);
                                    }
                                    break;
                                case "Minute":
                                    {
                                        if (noOfDays.Hours > stripLineInfo.RepeatFor)
                                            canAddChild = false;
                                        StripStart = StripStart.AddMinutes(stripLineInfo.RepeatFor);
                                        StripEnd = StripEnd.AddMinutes(stripLineInfo.RepeatFor);
                                    }
                                    break;
#endif
                            }

                            //if the user defined the invalid repeatfor value for the Merged stripline means the children will be not added to the stripline panel
                            if(canAddChild)
                                this.Children.Add(repeatStrip);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the stripline.
        /// </summary>
        /// <param name="striplineInfo">The stripline info.</param>
        /// <returns></returns>
        private StripLine GetStripline(StripLineInfo striplineInfo)
        {
            StripLine strip = new StripLine();
            strip.DataContext = striplineInfo;
            strip.StartDate = striplineInfo.StartDate;
            strip.Type = striplineInfo.Type;
            strip.EndDate = striplineInfo.EndDate;

            noOfDays = striplineInfo.EndDate.Subtract(striplineInfo.StartDate);

            if (striplineInfo.Style != null)
                strip.Style = striplineInfo.Style;

            if (strip.Type == StriplineType.Regular)
            {
#if !SILVERLIGHT
                if (noOfDays.Hours == 0 && striplineInfo.StartDate.Equals(striplineInfo.EndDate))
                    noOfDays = noOfDays.Add(new TimeSpan(24, 0, 0));
                width = GetWidth(striplineInfo.StartDate, TimeUnit.Hours) * (noOfDays.TotalHours);
#else
                width = GetWidth(striplineInfo.StartDate, TimeUnit.Days) * (noOfDays.Days + 1);
#endif
                if (striplineInfo.RepeatBehavior == Repeat.None)
                {

#if !SILVERLIGHT
                    // Style for stripline can be applied using StyleSelector as user defined
                    this.GetStyle(strip, striplineInfo);

                    //ContentTemplate for Stripline can be applied using DataTemplateSelector as user defined.
                    this.GetTemplate(strip, striplineInfo);
                    pos = this.ParentControl.GetStartPositionOfDate(striplineInfo.StartDate, TimeUnit.Hours);
#else
                    pos = this.ParentControl.GetStartPositionOfDate(striplineInfo.StartDate);
#endif
                    strip.Position = new Point(pos, 0);
                }
                strip.Width = width;
            }

            //if the user defined the stripline as Absolute we need to set the Height,Width and position as user given
            if (strip.Type == StriplineType.Absolute)
            {
                strip.Height = striplineInfo.Height > 0 ? striplineInfo.Height : this.ActualHeight;
                strip.Width = striplineInfo.Width > 0 ? striplineInfo.Width : this.ActualWidth;
                strip.Position = striplineInfo.Position;
            }
#if SILVERLIGHT
            SetBinding(strip, striplineInfo);
#endif
            return strip;
        }


#if !SILVERLIGHT

        /// <summary>
        /// Gets the style.
        /// </summary>
        /// <param name="strip">The strip.</param>
        /// <param name="stripLineInfo">The strip line info.</param>
        private void GetStyle(StripLine strip, StripLineInfo stripLineInfo)
        {
            if (stripLineInfo.StyleSelector != null)
            {
                CustomStyle = stripLineInfo.StyleSelector.SelectStyle(strip, this);
                if (CustomStyle != null)
                    strip.Style = CustomStyle;
            }
        }
            

        /// <summary>
        /// Gets the template.
        /// </summary>
        /// <param name="strip">The strip.</param>
        /// <param name="stripLineInfo">The strip line info.</param>
        private void GetTemplate(StripLine strip,StripLineInfo stripLineInfo)
        {
            if (stripLineInfo.ContentTemplateSelector != null)
            {
                CustomTemplate = stripLineInfo.ContentTemplateSelector.SelectTemplate(strip, this);
                if (CustomTemplate != null)
                    strip.ContentTemplate = CustomTemplate;
            }
        }

#endif

        /// <summary>
        /// Striplines the created.
        /// </summary>
        /// <param name="strip">The strip.</param>
        private void StriplineCreated(StripLine strip)
        {
            StriplineCreatedEventArgs args = new StriplineCreatedEventArgs() 
            { 
                CurrentStripline = strip, 
#if !SILVERLIGHT
                RoutedEvent=GanttControl.StripLineCreatedevent 
#endif
            };
            this.ParentControl.ParentControl.RaiseStripLineCreated(args);
            if (args.Handled == true)
            {
               strip= args.CurrentStripline as StripLine;
            }
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="timeUnit">The time unit.</param>
        /// <returns></returns>
        internal double GetWidth(DateTime time, TimeUnit timeUnit)
        {
            return this.ParentControl.ParentControl.GanttSchedule.GetWidth(this.StartTime, time, timeUnit);
        }

#if SILVERLIGHT

        /// <summary>
        /// Sets the binding.
        /// </summary>
        /// <param name="strip">The strip.</param>
        /// <param name="stripLineInfo">The strip line info.</param>
        private void SetBinding(StripLine strip, StripLineInfo stripLineInfo)
        {
            //SL4 doesn't support the Binding in Setter values. So we externaly binding the properties.
            Binding contentBinding = new Binding("Content");
            contentBinding.Source = stripLineInfo;
            strip.SetBinding(StripLine.ContentProperty, contentBinding);

            Binding contentTemplateBinding = new Binding("ContentTemplate");
            contentTemplateBinding.Source = stripLineInfo;
            strip.SetBinding(StripLine.ContentTemplateProperty, contentTemplateBinding);

            Binding backgroundBinding = new Binding("Background");
            backgroundBinding.Source = stripLineInfo;
            strip.SetBinding(StripLine.BackgroundProperty, backgroundBinding);

            Binding verticalCntAlignmentBinding = new Binding("VerticalContentAlignment");
            verticalCntAlignmentBinding.Source = stripLineInfo;
            strip.SetBinding(StripLine.VerticalContentAlignmentProperty, verticalCntAlignmentBinding);

            Binding horizontalCntAlignmentBinding = new Binding("HorizontalContentAlignment");
            horizontalCntAlignmentBinding.Source = stripLineInfo;
            strip.SetBinding(StripLine.HorizontalContentAlignmentProperty, horizontalCntAlignmentBinding);
        }

#endif
        #endregion
    }
}
