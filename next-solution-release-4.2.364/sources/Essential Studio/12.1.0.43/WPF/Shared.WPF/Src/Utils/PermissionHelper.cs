// <copyright file="PermissionHelper.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using Syncfusion.Licensing;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Permission Helper class to check permission in XBAP partial supported environment. 
    /// </summary>
    public class PermissionHelper
    {
        #region Private members
        /// <summary>
        /// Unmanaged code permission.
        /// </summary>
        private static bool? m_unmanagedCodePermission;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether this instance has unmanaged code permission.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has unmanaged code permission; otherwise, <c>false</c>.
        /// </value>
        public static bool HasUnmanagedCodePermission
        {
            get
            {
                if (!m_unmanagedCodePermission.HasValue)
                {
                    m_unmanagedCodePermission = HasSecurityPermissionFlag(SecurityPermissionFlag.UnmanagedCode);
                }

                return m_unmanagedCodePermission.Value;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the safe point to screen.
        /// </summary>
        /// <param name="visual">The visual.</param>
        /// <param name="point">The point.</param>
        /// <returns>Point from point to screen method.</returns>
        public static Point GetSafePointToScreen(Visual visual, Point point)
        {
            if (EnvironmentTest.IsSecurityGranted)
            {
                try
                {
                    return (visual is HwndHost) ? visual != null && PresentationSource.FromVisual(visual) != null ? visual.PointToScreen(point) : point 
                                                : visual.PointToScreen(point);                    
                }
                catch
                {
                    try
                    {
                        return visual != null && PresentationSource.FromVisual(visual) != null ? visual.PointToScreen(point) : point;
                    }
                    catch
                    {
                        return (visual as FrameworkElement).Parent != null && PresentationSource.FromVisual((visual as FrameworkElement).Parent as Visual) != null ?
                            ((visual as FrameworkElement).Parent as Visual).PointToScreen(point) : point;
                    }
                }
            }
            else
                return point;

        }
        #endregion

        #region Implementation
        /// <summary>
        /// Determines whether [has security permission flag] [the specified flag].
        /// </summary>
        /// <param name="flag">The flag value to check whether permission is granted or not.</param>
        /// <returns>bool value.</returns>
        private static bool? HasSecurityPermissionFlag(SecurityPermissionFlag flag)
        {
            bool? result = true;

            try
            {
                SecurityPermission sp = new SecurityPermission(flag);
                sp.Demand();
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        #endregion
    }
}
