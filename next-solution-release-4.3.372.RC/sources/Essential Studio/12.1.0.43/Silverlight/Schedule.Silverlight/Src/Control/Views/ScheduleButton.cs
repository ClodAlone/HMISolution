#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Schedule
{
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
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Class for Schedule Button.
    /// </summary>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    public class ScheduleButton : Button
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleButton"/> class.
        /// </summary>
        public ScheduleButton()
        {
            this.DefaultStyleKey = typeof(ScheduleButton);
        }
    }

    /// <summary>
    ///  class that holds toggle button for schedule
    /// </summary>
    public class ScheduleToggleButton : ToggleButton
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleToggleButton"/> class.
        /// </summary>
        public ScheduleToggleButton()
        {
            this.DefaultStyleKey = typeof(ScheduleToggleButton);
        }
    }
    /// <summary>
    ///  class that holds radio button for color button
    /// </summary>
    public class ColorButton : RadioButton
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Schedule.ColorButton">ColorButton</see> class. 
        /// </summary>
        /// <remarks></remarks>
        public ColorButton()
        { 
        }
    }
}
