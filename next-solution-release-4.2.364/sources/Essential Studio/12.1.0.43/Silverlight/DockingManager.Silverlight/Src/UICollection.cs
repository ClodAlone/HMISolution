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
using System.Collections.ObjectModel;
using System.Windows.Browser;
using System.Collections.Generic;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the UICollection class.
    /// </summary>
    public class UICollection : ObservableCollection<UIElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UICollection"/> class.
        /// </summary>
        /// <param name="myCanvas">My canvas.</param>
        public UICollection(DockingManager myCanvas)
        {
            canvas = (Canvas)myCanvas;
        }

        internal Canvas canvas = null;
        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert.</param>
        protected override void InsertItem(int index, UIElement item)
        {
            base.InsertItem(index, item);
            if (canvas != null)
            {
                if (!canvas.Children.Contains(item))
                {
                    ((DockingManager)canvas).Add(item);
                }
            }

        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index.</param>
        protected override void SetItem(int index, UIElement item)
        {
            base.SetItem(index, item);
            canvas.Children[index] = item;
        }

        /// <summary>
        /// Removes the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            
            if (canvas != null)
            {
                UIElement elem = ((DockingManager)canvas).Children.Items[index];
                base.RemoveItem(index);
                ((DockingManager)canvas).Remove(elem);
                //canvas.Children.RemoveAt(index);
            }
            
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        protected override void ClearItems()
        {
            base.ClearItems();
            //((DockingManager)canvas).Clear();
            ((DockingManager)canvas).BaseClear();
        }
    }

    /// <summary>
    /// Represents the RightMouseButton EventArgs
    /// </summary>
    public class RightMouseButtonEventArgs : EventArgs
	{
		Point Point;
        /// <summary>
        /// Initializes a new instance of the <see cref="RightMouseButtonEventArgs"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="point">The point.</param>
		internal RightMouseButtonEventArgs(UIElement sender, Point point)
		{
			OriginalSource = sender;
			this.Point = point;
		}
        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <param name="relativeTo">The relative to.</param>
        /// <returns></returns>
		public Point GetPosition(UIElement relativeTo)
		{
			return OriginalSource.TransformToVisual(relativeTo).Transform(Point);
		}
        /// <summary>
        /// Gets or sets the original source.
        /// </summary>
        /// <value>The original source.</value>
		public UIElement OriginalSource { get; internal set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RightMouseButtonEventArgs"/> is handled.
        /// </summary>
        /// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
		public bool Handled { get; set; }
	}

    /// <summary>
    /// Represents the Right Click Class.
    /// </summary>
	public static class RightClick
	{
		private static RightClickWorker worker;
		
		private static readonly DependencyProperty RightClickHandlersProperty = DependencyProperty.RegisterAttached("RightClickHandlers", typeof(IList<EventHandler<RightMouseButtonEventArgs>>), typeof(RightClick), null);
		private static UIElement currentElement;

        /// <summary>
        /// Attaches the right click.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="handler">The handler.</param>
		public static void AttachRightClick(this UIElement element, EventHandler<RightMouseButtonEventArgs> handler)
		{
			if (worker == null)
			{
				worker = new RightClickWorker();
				worker.RightClick += worker_RightClick;
			}

			element.MouseEnter += HandleMouseEnter; //Enable
			//element.MouseMove += HandleMouseEnter;  //Enable
			element.MouseLeave += HandleMouseLeave; //Disable

			IList<EventHandler<RightMouseButtonEventArgs>> handlers = element.GetValue(RightClickHandlersProperty) as IList<EventHandler<RightMouseButtonEventArgs>>;
			if (handlers == null)
			{
				handlers = new List<EventHandler<RightMouseButtonEventArgs>>();
				element.SetValue(RightClickHandlersProperty, handlers);
			}
			handlers.Add(handler);
		}

        /// <summary>
        /// Detaches the right click.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="handler">The handler.</param>
		public static void DetachRightClick(this UIElement element, EventHandler<RightMouseButtonEventArgs> handler)
		{
			element.MouseEnter -= HandleMouseEnter; //Enable			
			element.MouseLeave -= HandleMouseLeave; //Disable

			IList<EventHandler<RightMouseButtonEventArgs>> handlers = element.GetValue(RightClickHandlersProperty) as IList<EventHandler<RightMouseButtonEventArgs>>;
			if (handlers != null)
			{
				if (handlers.Contains(handler))
				{
					handlers.Remove(handler);
					if (handlers.Count == 0)
						element.ClearValue(RightClickHandlersProperty);
				}
			}
		}

        /// <summary>
        /// Handles the mouse enter.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private static void HandleMouseEnter(object sender, EventArgs e)
		{
			currentElement = (sender as UIElement);
			worker.IsEnabled = true;
		}

        /// <summary>
        /// Handles the mouse leave.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private static void HandleMouseLeave(object sender, EventArgs e)
		{
			currentElement = null;
			worker.IsEnabled = false;
		}

        /// <summary>
        /// Handles the RightClick event of the worker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Browser.HtmlEventArgs"/> instance containing the event data.</param>
		private static void worker_RightClick(object sender, HtmlEventArgs e)
		{
			if (currentElement == null) return;
			IList<EventHandler<RightMouseButtonEventArgs>> handlers = currentElement.GetValue(RightClickHandlersProperty) as IList<EventHandler<RightMouseButtonEventArgs>>;
			if (handlers == null || handlers.Count == 0) return;
			Point p = new System.Windows.Point(e.OffsetX, e.OffsetY);
			p = System.Windows.Application.Current.RootVisual.TransformToVisual(currentElement).Transform(p);
			RightMouseButtonEventArgs args = new RightMouseButtonEventArgs(currentElement, p);			
			foreach (EventHandler<RightMouseButtonEventArgs> handler in handlers)
			{
				handler(currentElement, args);
			}
			if (args.Handled)
			{
				//Prevent Silverlight Plugin from receiving this event (buggy for FireFox)
				e.PreventDefault();
				e.StopPropagation();
			}
		}

		/// <summary>
		/// Uses the HTML Dom bridge for detecting right clicks.
		/// </summary>
		private class RightClickWorker
		{
			private bool IsInternetExplorer;
			public event EventHandler<HtmlEventArgs> RightClick;
			private bool isEnabled;
			HtmlElement glassDiv;

			public bool IsEnabled
			{
				get { return isEnabled; }
				set { 
					isEnabled = value;
					if (!value) HideGlass();
				}
			}
			/// <summary>
			/// Initializes a new instance of the <see cref="RightClickWorker"/> class.
			/// </summary>
			public RightClickWorker()
			{
				if (HtmlPage.IsEnabled && Application.Current.Host.Settings.Windowless)
				{
					IsInternetExplorer = HtmlPage.BrowserInformation.UserAgent.IndexOf("MSIE") > -1;
					if (IsInternetExplorer)
					{
						HtmlPage.Document.AttachEvent("oncontextmenu", this.HandleIEContextMenu);
					}
					else
					{
						HtmlPage.Document.AttachEvent("mousedown", this.HandleGlassMouseDown);
					}
				}
			}

			/// <summary>
			/// Creates a div on top of the plugin to prevent mouse events from
			/// firing on the plugin (Mozilla browsers only).
			/// </summary>
			private void ShowGlass()
			{
				if (glassDiv == null)
				{
					glassDiv = HtmlPage.Document.CreateElement("div");
					glassDiv.SetAttribute("style", "width:100%;height:100%;position:absolute;left:0;top:0;zIndex:100");
					glassDiv.AttachEvent("mouseup", this.HandleGlassMouseUp);
					glassDiv.AttachEvent("mousemove", (object s, HtmlEventArgs e) => { HideGlass(); });
					HtmlPage.Document.Body.AppendChild(glassDiv);
				}
			}

            /// <summary>
            /// Hides the glass.
            /// </summary>
			private void HideGlass()
			{
				if (glassDiv != null)
				{
					HtmlPage.Document.Body.RemoveChild(glassDiv);
					glassDiv = null;
				}
			}

            /// <summary>
            /// Handles the glass mouse down.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="args">The <see cref="System.Windows.Browser.HtmlEventArgs"/> instance containing the event data.</param>
			private void HandleGlassMouseDown(object sender, HtmlEventArgs args)
			{
				if (IsEnabled && args.MouseButton == MouseButtons.Right)
				{
					ShowGlass();
				}
			}

            /// <summary>
            /// Handles the glass mouse up.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="args">The <see cref="System.Windows.Browser.HtmlEventArgs"/> instance containing the event data.</param>
			private void HandleGlassMouseUp(object sender, HtmlEventArgs args)
			{
				if (args.MouseButton == MouseButtons.Right)
				{
					OnRightClick(args);					
				}
			}

            /// <summary>
            /// Handles the IE context menu.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="args">The <see cref="System.Windows.Browser.HtmlEventArgs"/> instance containing the event data.</param>
			private void HandleIEContextMenu(object sender, HtmlEventArgs args)
			{
				OnRightClick(args);
			}

            /// <summary>
            /// Raises the <see cref="E:RightClick"/> event.
            /// </summary>
            /// <param name="args">The <see cref="System.Windows.Browser.HtmlEventArgs"/> instance containing the event data.</param>
			private void OnRightClick(HtmlEventArgs args)
			{
				if (RightClick != null)
				{
					RightClick(null, args);
				}
			}
		}
	}
}

