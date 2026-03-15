// <copyright file="DockPreviewManagerVS2003.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents docking manager's preview base for VS2003.
    /// </summary>
    /// <exclude/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DockPreviewManagerVS2003 : DockPreviewManagerBase
    {
        #region Internal class
        /// <summary>
        /// Helps to hold the distance record of docking.
        /// </summary>
        internal sealed class DistanceRecord : IComparable
        {
            /// <summary>
            /// Indicates the dockside.
            /// </summary>
            public readonly DockSide Side;

            /// <summary>
            /// Indicates distance.
            /// </summary>
            public readonly double Distance;

            /// <summary>
            /// Initializes a new instance of the <see cref="DistanceRecord"/> class.
            /// </summary>
            /// <param name="side">The DockSide side.</param>
            /// <param name="distance">Docking distance.</param>
            public DistanceRecord(DockSide side, double distance)
            {
                Distance = distance;
                Side = side;
            }

            /// <summary>
            /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
            /// </summary>
            /// <param name="obj">An object to compare with this instance.</param>
            /// <returns>
            /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings:
            /// Value
            /// Meaning
            /// Less than zero
            /// This instance is less than <paramref name="obj"/>.
            /// Zero
            /// This instance is equal to <paramref name="obj"/>.
            /// Greater than zero
            /// This instance is greater than <paramref name="obj"/>.
            /// </returns>
            /// <exception cref="T:System.ArgumentException">
            /// <paramref name="obj"/> is not the same type as this instance.
            /// </exception>
            public int CompareTo(object obj)
            {
                DistanceRecord recordTarget = (DistanceRecord)obj;

                return Distance.CompareTo(recordTarget.Distance);
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="DockPreviewManagerVS2003"/> class.
        /// </summary>
        /// <param name="manager">Specifies docking manager, the
        /// instance is bound to.</param>
        /// <property name="flag" value="Finished"/>
        public DockPreviewManagerVS2003(DockingManager manager)
            : base(manager)
        {
        }
        #endregion

        #region Propeties
        /// <summary>
        /// Gets value of the IsDockPreviewMainButtonVisible property.
        /// </summary>
        public override bool IsDockPreviewMainButtonVisible
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Finds the place in docking window where an element can be docked to.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="host">DockedElementTabbedHost where an element can be docked to</param>
        /// <param name="point">the point where an element can be docked to</param>
        /// <returns>the place, an element can be docked to</returns>
        protected override DockPreviewRecord? FindDockingPlaceInternal(FrameworkElement element, DockedElementTabbedHost host, Point point)
        {
            DockPreviewRecord result = new DockPreviewRecord
            {
                State = host.State,
                Element = element
            };
            bool hasResult = false;

            if (Keyboard.Modifiers != ModifierKeys.Control)
            {
                hasResult |= CheckIfFeets(host, point, ref result);
            }
            else if (host.State == DockState.Dock)
            {
                GeneralTransform transform = host.TransformToVisual(DockingManager);
                Point pointDocking = transform.Transform(point);
                hasResult = CheckIfFeets(DockingManager, pointDocking, ref result);

                if (hasResult)
                {
                    result.TargetElement = null;
                }
            }

            return hasResult ? (DockPreviewRecord?)result : null;
        }

        /// <summary>
        /// Creates dock preview where an element can be docked to.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="host">FrameworkElement where adorner can be created</param>
        /// <param name="size">The element Size.</param>
        /// <returns>adorner with templates support</returns>
        protected override TemplatedAdornerBase CreateDockPreviewAdorner(DockPreviewRecord record, FrameworkElement host, Size size)
        {
            HostAdornerVS2003 result = new HostAdornerVS2003(host);
            result.SetDockSide(record.Side);

            return result;
        }

        /// <summary>
        /// Creates main button in internal dock preview.
        /// </summary>
        /// <param name="host">the host to create preview button in</param>
        /// <param name="bShowSideButtons">true if side buttons must be shown</param>
        protected override void CreateDockPreviewMainButtonInternal(FrameworkElement host, bool bShowSideButtons)
        {
            return;
        }

        /// <summary>
        /// Hides main button in internal dock preview.
        /// </summary>
        /// <param name="canSwitchPerform">true if perform can be switched</param>
        protected override void HideDockPreviewMainButtonInternal(bool canSwitchPerform)
        {
            return;
        }

        /// <summary>
        /// Checks if feet's
        /// </summary>
        /// <param name="elementHost">The element host.</param>
        /// <param name="point">The point.</param>
        /// <param name="result">The result.</param>
        /// <returns>return bool value</returns>
        private static bool CheckIfFeets(FrameworkElement elementHost, Point point, ref DockPreviewRecord result)
        {
            DockedElementTabbedHost host = elementHost as DockedElementTabbedHost;

            if (elementHost != null && elementHost.Visibility != Visibility.Collapsed)
            {
                Rect rectHost = new Rect(0, 0, elementHost.ActualWidth, elementHost.ActualHeight);
                Point pointInHost = point;

                if (rectHost.Contains(pointInHost))
                {
                    ArrayList listSorting = new ArrayList() 
                    {
                        new DistanceRecord(DockSide.Top, pointInHost.Y), 
                        new DistanceRecord(DockSide.Bottom, rectHost.Bottom - pointInHost.Y), 
                        new DistanceRecord(DockSide.Left, pointInHost.X), 
                        new DistanceRecord(DockSide.Right, rectHost.Right - pointInHost.X) 
                    };
                    listSorting.Sort();

                    DistanceRecord closestRecord = (DistanceRecord)listSorting[0];
                    result.TargetElement = (host != null) ? host.HostedElement : null;
                    result.Side = closestRecord.Side;
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}
