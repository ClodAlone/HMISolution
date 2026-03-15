// <copyright file="ExtendedPopup.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// ========================================
    /// .NET Framework 3.0 Custom Control
    /// ========================================
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is
    /// to be used:
    /// xmlns:MyNamespace="clr-namespace:OfficeStyleWindowProject"
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is
    /// to be used:
    /// xmlns:MyNamespace="clr-namespace:OfficeStyleWindowProject;assembly=OfficeStyleWindowProject"
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    /// Right click on the target project in the Solution Explorer and
    /// "Add Reference"->"Projects"->[Browse to and select this project]
    /// Step 2)
    /// Go ahead and use your control in the XAML file. Note that Intellisense in the
    /// XML editor does not currently work on custom controls and its child elements.
    /// <MyNamespace:ExtendedPopup xmlns:MyNamespace="http://schemas.syncfusion.com/wpf"/>
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ExtendedPopup : Popup
    {
        /// <summary>
        /// Represents the stack
        /// </summary>
        public Stack<object> m_stack = new Stack<object>();

        /// <summary>
        /// Represents it was pressed
        /// </summary>
        private bool m_wasPressed = false;

        /// <property name="flag" value="Finished" />
        /// <unfinished />
        /// <summary>
        /// Invoked when an unhandled Mouse.LostMouseCapture attached
        /// event reaches an element in its route that is derived from
        /// this class. Implement this method to add class handling for
        /// this event.
        /// </summary>
        /// <param name="e">TheMouseEventArgs that contains event data.</param>
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            m_stack.Push(e.OriginalSource);
            Trace.WriteLine(e.OriginalSource + " - OnLostMouseCapture");
            base.OnLostMouseCapture(e);
        }

        /// <property name="flag" value="Finished" />
        /// <unfinished />
        /// <summary>
        /// Invoked when an unhandled Mouse.GotMouseCapture attached
        /// event reaches an element in its route that is derived from
        /// this class. Implement this method to add class handling for
        /// this event.
        /// </summary>
        /// <param name="e">The MouseEventArgs that contains the event
        /// data.</param>
        protected override void OnGotMouseCapture(MouseEventArgs e)
        {
            if (m_wasPressed)
            {
                m_stack.Clear();
                IsOpen = false;
                m_wasPressed = false;
            }
            else
            {
                AutomationPeer peer = UIElementAutomationPeer.CreatePeerForElement(e.OriginalSource as UIElement);
                if (m_stack.Count > 0)
                {
                    Trace.WriteLine(e.OriginalSource + " - OnGotMouseCapture - " + m_stack.Peek().ToString());
                }
                else
                {
                    Trace.WriteLine(e.OriginalSource + " - OnGotMouseCapture");
                }

                if (m_stack.Count > 0 && m_stack.Peek().ToString() == "System.Windows.Controls.Primitives.PopupRoot")
                {
                    if (peer != null && peer.IsControlElement())
                    {
                        if (peer is IInvokeProvider)
                        {
                            m_wasPressed = true;
                        }
                    }
                }
            }

            base.OnGotMouseCapture(e);
        }

        /// <summary>
        /// Hides the popup.
        /// </summary>
        private void HidePopup()
        {
            IsOpen = false;
        }

        /// <property name="flag" value="Finished" />
        /// <unfinished />
        /// <summary>
        /// Invoked whenever an unhandled Mouse.MouseDown attached routed
        /// event reaches an element derived from this class in its
        /// route. Implement this method to add class handling for this
        /// event.
        /// </summary>
        /// <param name="e">Provides data about the event.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            Trace.WriteLine(e.Source + " - OnMouseDown");
            base.OnMouseDown(e);
        }

        /// <property name="flag" value="Finished" />
        /// <unfinished />
        /// <summary>
        /// Invoked when an unhandled Mouse.MouseUp routed event reaches
        /// an element in its route that is derived from this class.
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the
        /// event data. The event data reports that the
        /// mouse button was released.</param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            Trace.WriteLine(e.Source + " - OnMouseUp");
            base.OnMouseUp(e);
        }
    }
}
