#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
/**************************************************************************\
    Copyright Microsoft Corporation. All Rights Reserved.
\**************************************************************************/

namespace Standard
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Media;

    internal static class DpiHelper
    {
        private static Matrix _transformToDevice;
        private static Matrix _transformToDip;

        static DpiHelper()
        {
            using (SafeDC desktop = SafeDC.GetDesktop())
            {
                int pixelsPerInchX = Standard.NativeMethods.GetDeviceCaps(desktop, DeviceCap.LOGPIXELSX);
                int pixelsPerInchY = Standard.NativeMethods.GetDeviceCaps(desktop, DeviceCap.LOGPIXELSY);
                _transformToDip = Matrix.Identity;
                _transformToDip.Scale(96.0 / ((double)pixelsPerInchX), 96.0 / ((double)pixelsPerInchY));
                _transformToDevice = Matrix.Identity;
                _transformToDevice.Scale(((double)pixelsPerInchX) / 96.0, ((double)pixelsPerInchY) / 96.0);
            }
        }

        public static Point DevicePixelsToLogical(Point devicePoint)
        {
            return _transformToDip.Transform(devicePoint);
        }

        public static Rect DeviceRectToLogical(Rect deviceRectangle)
        {
            Point topLeft = DevicePixelsToLogical(new Point(deviceRectangle.Left, deviceRectangle.Top));
            return new Rect(topLeft, DevicePixelsToLogical(new Point(deviceRectangle.Right, deviceRectangle.Bottom)));
        }

        public static Size DeviceSizeToLogical(Size deviceSize)
        {
            Point pt = DevicePixelsToLogical(new Point(deviceSize.Width, deviceSize.Height));
            return new Size(pt.X, pt.Y);
        }

        public static Point LogicalPixelsToDevice(Point logicalPoint)
        {
            return _transformToDevice.Transform(logicalPoint);
        }

        public static Rect LogicalRectToDevice(Rect logicalRectangle)
        {
            Point topLeft = LogicalPixelsToDevice(new Point(logicalRectangle.Left, logicalRectangle.Top));
            return new Rect(topLeft, LogicalPixelsToDevice(new Point(logicalRectangle.Right, logicalRectangle.Bottom)));
        }

        public static Size LogicalSizeToDevice(Size logicalSize)
        {
            Point pt = LogicalPixelsToDevice(new Point(logicalSize.Width, logicalSize.Height));
            return new Size { Width = pt.X, Height = pt.Y };
        }

        public static Thickness LogicalThicknessToDevice(Thickness logicalThickness)
        {
            Point topLeft = LogicalPixelsToDevice(new Point(logicalThickness.Left, logicalThickness.Top));
            Point bottomRight = LogicalPixelsToDevice(new Point(logicalThickness.Right, logicalThickness.Bottom));
            return new Thickness(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
        }
    }
}
