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
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Syncfusion.Windows.Tools.Olap
{
    public class PopupDelayService
    {
        private DispatcherTimer timer = new DispatcherTimer();

        private UIElement target;
        private Popup smartTag;

        public PopupDelayService(UIElement element, Popup popup)
        {
            InitService(element, popup, TimeSpan.FromSeconds(.5));
        }

        public PopupDelayService(UIElement element, Popup popup, TimeSpan delayInternal)
        {
            InitService(element, popup, delayInternal);
        }

        private void InitService(UIElement element, Popup popup, TimeSpan delayInternal)
        {
            target = element;
            smartTag = popup;

            smartTag.MouseMove += new MouseEventHandler(MyControl_MouseMove);
            smartTag.MouseLeave += new MouseEventHandler(myPopup_MouseLeave);
            target.MouseLeave += new MouseEventHandler(MyControl_MouseLeave);
            target.MouseMove += new MouseEventHandler(MyControl_MouseMove);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = delayInternal;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            if ((!target.IsMouseOver) && (!smartTag.IsMouseOver))
                smartTag.IsOpen = false;
            else
            {
                if (!smartTag.IsOpen)
                    smartTag.IsOpen = true;
            }
        }

        private void myPopup_MouseLeave(object sender, MouseEventArgs e)
        {
            //DelayShow(false);
            smartTag.IsOpen = false;
        }
        private void MyControl_MouseLeave(object sender, MouseEventArgs e)
        {
            DelayShow(true);
        }
        private void MyControl_MouseMove(object sender, MouseEventArgs e)
        {
            if ((!smartTag.IsOpen) || (smartTag.Child.Opacity < 1.0))
                DelayShow(true);
        }

        private void DelayShow(Boolean ShowMe)
        {
            if (!ShowMe)
                smartTag.Child.Opacity = 0.5;
            else
                smartTag.Child.Opacity = 1.0;

            timer.Start();
        }

    }

}
