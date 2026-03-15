#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Scroll
{

    internal class MouseEventTargetCollection : List<IMouseEventsTarget>, IMouseEventsTarget
    {
        FrameworkElement host;

        public new void Add(IMouseEventsTarget target)
        {
            base.Add(target);
            if (host != null)
                target.SetHost(host);
        }

        #region IMouseEventsTarget Members
        public void SetHost(FrameworkElement host)
        {
            this.host = host;
            foreach (IMouseEventsTarget target in this)
            {
                target.SetHost(host);
            }
        }

        public void OnMouseEnter(MouseEventArgs e)
        {
            foreach (IMouseEventsTarget target in this)
            {
                if (!e.Handled)
                    target.OnMouseEnter(e);
            }
        }

        public void OnMouseLeave(MouseEventArgs e)
        {
            foreach (IMouseEventsTarget target in this)
            {
                if (!e.Handled)
                    target.OnMouseLeave(e);
            }
        }

        public void OnMouseDown(MouseButtonEventArgs e)
        {
            foreach (IMouseEventsTarget target in this)
            {
                if (!e.Handled)
                    target.OnMouseDown(e);
            }
        }

        public void OnMouseMove(MouseEventArgs e)
        {
            foreach (IMouseEventsTarget target in this)
            {
                if (!e.Handled)
                    target.OnMouseMove(e);
            }
        }

        public void OnMouseUp(MouseButtonEventArgs e)
        {
            foreach (IMouseEventsTarget target in this)
            {
                if (!e.Handled)
                    target.OnMouseUp(e);
            }
        }

        public void OnMouseWheel(MouseWheelEventArgs e)
        {
            foreach (IMouseEventsTarget target in this)
            {
                if (!e.Handled)
                    target.OnMouseWheel(e);
            }
        }

        public void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            foreach (IMouseEventsTarget target in this)
            {
                if (!e.Handled)
                    target.OnPreviewMouseWheel(e);
            }
        }

        public void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            foreach (IMouseEventsTarget target in this)
            {
                if (!e.Handled)
                    target.OnPreviewMouseDown(e);
            }
        }

        public void OnPreviewMouseMove(MouseEventArgs e)
        {
            foreach (IMouseEventsTarget target in this)
            {
                if (!e.Handled)
                    target.OnPreviewMouseMove(e);
            }
        }

        public void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            foreach (IMouseEventsTarget target in this)
            {
                if (!e.Handled)
                    target.OnPreviewMouseUp(e);
            }
        }
      
        public void OnDrop(DragEventArgs e)
        {
          
            foreach (IMouseEventsTarget target in this)
            {
                if (!e.Handled)
                    target.OnDrop(e);
            }
        }

        #endregion
    }

}
