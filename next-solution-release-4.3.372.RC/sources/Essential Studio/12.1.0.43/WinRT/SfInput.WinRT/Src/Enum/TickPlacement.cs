// <copyright file="TickPlacement.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if WPF
namespace Syncfusion.Windows.Controls.Input
#elif WINDOWS_PHONE||WINDOWS_PHONE_7
namespace Syncfusion.WP.Controls.Input
#elif SILVERLIGHT
namespace Syncfusion.Tools.Controls.Input
#else
namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
   
    /// <summary>
    /// Specifies the Ticks placement with range slider.
    /// </summary>
    public enum TickPlacement
    {
        /// <summary>
        /// The ticks are placed bottom for horizontal orientation and  right for vertical orientation.
        /// </summary>
        BottomRight,

        /// <summary>
        /// The ticks are placed inside the range slider.
        /// </summary>
        Inline,

        /// <summary>
        /// The ticks are not showing.
        /// </summary>
        None,

        /// <summary>
        /// The ticks are placed both side for  horizontal orientation ticks are in top and bottom and for  vertical orientation ticks are in left and right.
        /// </summary>
        Outside,

        /// <summary>
        /// The ticks are placed top for horizontal orientation and  left for vertical orientation.
        /// </summary>
        TopLeft
    }

    /// <summary>
    /// Specifies the Ticks placement with range slider.
    /// </summary>
    public enum LabelPlacement
    {
        /// <summary>
        /// The labels are placed bottom for horizontal orientation and  right for vertical orientation.
        /// </summary>
        BottomRight,

        
        /// <summary>
        /// The labels are placed top for horizontal orientation and  left for vertical orientation.
        /// </summary>
        TopLeft
    }



    /// <summary>
    /// Specifies the Ticks placement with range slider.
    /// </summary>
    public enum ValuePlacement
    {
        /// <summary>
        /// The labels are placed bottom for horizontal orientation and  right for vertical orientation.
        /// </summary>
        BottomRight,


        /// <summary>
        /// The labels are placed top for horizontal orientation and  left for vertical orientation.
        /// </summary>
        TopLeft
    }


    /// <summary>
    /// Specifies the SliderSnapsTo with range slider.
    /// </summary>
    public enum SliderSnapsTo
    {
        /// <summary>
        /// Conform the indicator to the step values.
        /// </summary>
        StepValues,

        /// <summary>
        ///  Conform the indicator to the tick marks.
        /// </summary>
        Ticks
    }

    /// <summary>
    /// specifies the Thumb ToolTip placement with range slider.
    /// </summary>
    public enum ThumbToolTipPlacement
    {
        /// <summary>
        /// The tooltip placed bottom for horizontal orientation and  right for vertical orientation.
        /// </summary>
        BottomRight,
        
        /// <summary>
        /// The tooltip is not showing.
        /// </summary>
        None,       

        /// <summary>
        /// The tooltip is placed top for horizontal orientation and  left for vertical orientation.
        /// </summary>
        TopLeft
    }
}
