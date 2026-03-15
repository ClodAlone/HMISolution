#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System.Drawing;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Common values used across the diagram.
    /// </summary>
    public class CommonUsedValues
    {
        /// <summary>
        ///  Allowed angle offset when node rotation is locked.
        /// </summary>
        public static readonly double ALLOWED_ROTATE_ANGLE = 0.5d;

        /// <summary>
        /// Count allows box position.
        /// </summary>
        public static readonly int ALLOWED_BOX_POSTIONS_NUMBER = 8;

        /// <summary>
        /// Max number change by flipped by X.
        /// </summary>
        public static readonly int ALLOWED_BOX_POSTIONS_NUMBER_FLIPPED_X = 8;

        /// <summary>
        /// Max numeric change by flipped by Y.
        /// </summary>
        public static readonly int ALLOWED_BOX_POSTIONS_NUMBER_FLIPPED_Y = 12;

        /// <summary>
        /// Indicated angle when mouse cursor change it cursor to next;
        /// </summary>
        public static readonly float fDEF_ANGLE_CURSOR_CHANGE = 23.0f;

        /// <summary>
        /// Default distance from route path points to obstacle bounds.
        /// </summary>
        public static readonly float DEF_ROUTE_DISTANCE = 5.0f;

        /// <summary>
        /// Default control point size.
        /// </summary>
        public static readonly float DEF_CONTROL_POINT_SIZE = 7.0f;

        /// <summary>
        /// Default minimum size of the decorator.
        /// </summary>
        public static readonly float DEF_MIN_DECORATOR_SIZE = 6.0f;

        /// <summary>
        /// Default minimum decorator size.
        /// </summary>
        public static readonly int DEF_MIN_RESIZE_SIZE = 1;

        /// <summary>
        /// Default right angle.
        /// </summary>
        public static readonly int DEF_RIGHT_ANGLE = 90;

        /// <summary>
        /// Rotation handle offset value.
        /// </summary>
        public static readonly int ROTATION_HANDLE_OFFSET = 25;

        /// <summary>
        /// Rotation handle size.
        /// </summary>
        public static readonly int ROTATION_HANDLE_SIZE = 11;

        /// <summary>
        /// End point handle size.
        /// </summary>
        public static readonly int END_POINT_HANDLE_SIZE = 7;

        /// <summary>
        /// Size of the pinpoint.
        /// </summary>
        public static readonly int PIN_POINT_SIZE = 8;

        /// <summary>
        /// Resize the size of the handle.
        /// </summary>
        public static readonly int RESIZE_HANDLE_SIZE = 7;

        /// <summary>
        /// Size of the vertex handle.
        /// </summary>
        public static readonly int VERTEX_HANDLE_SIZE = 10;

        public static readonly int CIRCLE = 360;
        public static readonly int HUNDRED_PERCENT = 100;
        public static readonly float fHUNDRED_PERCENT = 100f;
        public static readonly int OPAQUE = 255;
        public static readonly int HALF_OPAQUE = 128;
        public static readonly int TRANSPARENT = 0;
        public static readonly Color BACK_COLOR = Color.White;
        public static readonly Color FORE_COLOR = Color.Black;
        public static readonly string POINT_SEPARATOR = @".";
        public static readonly float SHADOW_OFFSET = 8f;
        public static readonly Color SHADOW_COLOR = Color.DimGray;
        public static readonly float LINE_WIDTH = 1f;

        /// <summary>
        /// Maximum image size than .NET Framework 2.0 can display.
        /// </summary>
        public static readonly SizeF MAX_IMAGE_SIZE = new SizeF(10000, 10000);

        /// <summary>
        /// Maximum subpath count to create region for hit testing by decrease processor usage.
        /// </summary>
        public static readonly int MAX_REGION_SUBPATH = 1000;

        public static readonly float MAX_MAGNIFICATION = 6400f;
        public static readonly float MIN_MAGNIFICATION = 1f;
        public static readonly float MAX_SCALE_VALUE = 1000f;
        public static readonly float MIN_SCALE_VALUE = 0.001f;

        /// <summary>
        /// Rotation handle offset value for touch.
        /// </summary>
        public static readonly int ROTATION_HANDLE_TOUCH_OFFSET = 30;

        /// <summary>
        /// Rotation handle size for touch.
        /// </summary>
        public static readonly int ROTATION_HANDLE_TOUCH_SIZE = 15;

        /// <summary>
        /// End point handle size for touch.
        /// </summary>
        public static readonly int END_POINT_HANDLE_TOUCH_SIZE = 11;

        /// <summary>
        /// Size of the pinpoint for touch.
        /// </summary>
        public static readonly int PIN_POINT_TOUCH_SIZE = 11;

        /// <summary>
        /// Resize the size of the handle for touch.
        /// </summary>
        public static readonly int RESIZE_HANDLE_TOUCH_SIZE = 12;

        /// <summary>
        /// Size of the vertex handle for touch.
        /// </summary>
        public static readonly int VERTEX_HANDLE_TOUCH_SIZE = 13;

        /// <summary>
        /// Default control point size for touch.
        /// </summary>
        public static readonly float DEF_CONTROL_POINT_TOUCH_SIZE = 11.0f;

        /// <summary>
        /// Default line hit test padding for touch.
        /// </summary>
        public static readonly float TOUCH__HIT_TEST_PADDING = 15.0f;
    }
}
