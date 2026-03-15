#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if SILVERLIGHT || NETFX_CORE || WP
namespace System.Drawing.Drawing2D
{
    #region Enums
    public enum PathPointType
    {
        Start = 0,
        Line = 1,
        Bezier3 = 3,
        Bezier = 3,
        PathTypeMask = 7,
        DashMode = 16,
        PathMarker = 32,
        CloseSubpath = 128,
    }
    #endregion
}
#endif