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
    public class CustomGrid:Grid
    {
        #region Constructor

        public CustomGrid()
        {
            ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.TranslateInertia;
            ManipulationDelta += CustomGrid_ManipulationDelta;
        }

        void CustomGrid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
                return;
            SfSchedule schedule = this.FindParentElementOfType<SfSchedule>();
            ScheduleTimeLineView timelineview = this.FindParentElementOfType<ScheduleTimeLineView>();
            if (!schedule.ScrollManipulationCompleted)
            {
                if (Math.Abs(e.Delta.Translation.X) > Math.Abs(e.Delta.Translation.Y))
                {
                    double delta = timelineview.timelinescroll.HorizontalOffset + (e.Delta.Translation.X * -1);
                    if (delta <= timelineview.timelinescroll.ScrollableWidth)
                    {
#if SyncfusionFramework4_5_11
                        timelineview.timelinescroll.ChangeView(delta, null, null);
#else
                        timelineview.timelinescroll.ScrollToHorizontalOffset(delta);
#endif
                        if (delta <= 0)
                        {
                            if (e.IsInertial)
                            {
                                //e.Complete();
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
                           // e.Complete();
                        }
                        else
                        {
                            schedule.ScrollManipulationCompleted = true;                           
                        }
                    }
                }
            }
        }

        #endregion
    }
}
