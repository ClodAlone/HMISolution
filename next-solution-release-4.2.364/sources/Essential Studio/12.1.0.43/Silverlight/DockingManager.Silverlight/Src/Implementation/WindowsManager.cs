#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Tools.Controls
{
	/// <summary>
	/// Class that handles the creation and destruction of dynamic windows
	/// </summary>
	public class WindowsManager : IWindowsManager
	{
        private Canvas _canvas = null;

		/// <summary>
		/// creates the manager and stores the canvas to which attach the windows
		/// </summary>
		/// <param name="surface"> surface know as Canvas</param>
		public WindowsManager(Canvas surface)
		{
			_canvas = surface;
		}

        /// <summary>
        /// Shows the window.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="location">The location.</param>
        /// <returns>The IWindow Object.</returns>
		public IWindow ShowWindow(FrameworkElement content, string caption, Point location)
		{
			Window w = new Window();
			w.Caption = caption;
            w.contentpresenter.Children.Add(content);
			w.Closed += new EventHandler(w_Closed);
            ////Canvas.SetLeft(w, location.X);
            ////Canvas.SetTop(w, location.Y);
			_canvas.Children.Add(w);
			return w as IWindow;
		}

		/// <summary>
		/// show a window attaching it to the canvas
		/// </summary>
		/// <param name="w"> Window from docking manager</param>
		/// <param name="location"> give the Desired Location</param>
		public void ShowWindow(Window w, Point location)
		{
			Window wtmp = w as Window;
			wtmp.Closed += new EventHandler(w_Closed);
            Canvas.SetLeft(wtmp, location.X);
            Canvas.SetTop(wtmp, location.Y);
            if (!_canvas.Children.Contains(wtmp))
            {
                _canvas.Children.Add(wtmp);
            }
		}

        /// <summary>
        /// Deattaches the window.
        /// </summary>
        /// <param name="w">The w.</param>
        public void DeattachWindow(Window w)
        {
            w.Closed -= new EventHandler(w_Closed);
            if (_canvas.Children.Contains(w))
            {
                _canvas.Children.Remove(w);
            }
        }

        /// <summary>
        /// Handles the Closed event of the w control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void w_Closed(object sender, EventArgs e)
		{
            //// remove the object from the childern colelction and dispose it dispose the object 
			Window w = (Window)sender;
            w.CommonMethodForHideClose();
		}
	}
}
