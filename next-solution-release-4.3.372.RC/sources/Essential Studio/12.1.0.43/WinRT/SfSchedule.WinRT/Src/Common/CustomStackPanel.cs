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
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Syncfusion.UI.Xaml.Schedule
{
    public class CustomStackPanel : StackPanel
    {

        #region Constructor
        public CustomStackPanel()
        {
            ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.TranslateInertia;
            ManipulationDelta += CustomStackPanel_ManipulationDelta;
        }

        void CustomStackPanel_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
                return;
            SfSchedule schedule = this.FindParentElementOfType<SfSchedule>();
            ScheduleDaysView dayview = this.FindParentElementOfType<ScheduleDaysView>();
            AllDayAppointmentItemscontrol allday = this.FindParentElementOfType<AllDayAppointmentItemscontrol>();
            if (!schedule.ScrollManipulationCompleted)
            {
                if (Math.Abs(e.Delta.Translation.X) > Math.Abs(e.Delta.Translation.Y))
                {
                    double delta = dayview.scrollviewer.HorizontalOffset + (e.Delta.Translation.X * -1);
                    if (delta <= dayview.scrollviewer.ScrollableWidth)
                    {
#if SyncfusionFramework4_5_11
                        dayview.scrollviewer.ChangeView(delta, null, null);
#else
                        dayview.scrollviewer.ScrollToHorizontalOffset(delta);
#endif
                        if (delta <= 0)
                        {
                            if (e.IsInertial)
                            {
                                e.Complete();
                            }
                            else
                            {
                                schedule.ScrollManipulationCompleted = true;
                            }
                        }
                    }
                    else
                    {
                        if (e.IsInertial)
                        {
                            e.Complete();
                        }
                        else
                        {
                            schedule.ScrollManipulationCompleted = true;
                        }
                    }

                }
                else
                {
                    if (allday.AllDayScrollView.VerticalOffset + (e.Delta.Translation.Y * -1) <= allday.AllDayScrollView.ScrollableHeight && allday.AllDayScrollView.VerticalOffset + (e.Delta.Translation.Y * -1) >= 0)
                    {
#if SyncfusionFramework4_5_11
                        allday.AllDayScrollView.ChangeView(null, allday.AllDayScrollView.VerticalOffset + (e.Delta.Translation.Y * -1), null);
#else
                        allday.AllDayScrollView.ScrollToVerticalOffset(allday.AllDayScrollView.VerticalOffset + (e.Delta.Translation.Y * -1));
#endif
                    }
                }
            }
        }
        #endregion

    }
}
