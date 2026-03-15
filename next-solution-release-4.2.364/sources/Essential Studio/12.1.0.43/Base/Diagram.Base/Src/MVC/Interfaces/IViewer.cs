#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Implement default view properties of MVC architecture.
    /// </summary>
    public interface IViewer
    {
        /// <summary>
        /// Gets or sets a value indicating whether view show horizontal and vertical rulers.
        /// </summary>
        /// <value><c>true</c> if show rulers; otherwise, <c>false</c>.</value>
        /// DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        [DefaultValue(false) ]
        bool ShowRulers { get; set; }

        /// <summary>
        /// Gets the height of the rulers.
        /// </summary>
        /// <value>The height of the rulers.</value>
        int RulersHeight { get; }

        /// <summary>
        /// Gets the reference to viewer event sink.
        /// </summary>
        /// <value>The event sink.</value>
        ViewerEventSink EventSink { get; }

        /// <summary>
        /// Gets the view origin.
        /// </summary>
        /// <value>The origin.</value>
        PointF Origin { get; }

        /// <summary>
        /// Gets the magnification percent.
        /// </summary>
        /// <value>The magnification percent.</value>
        float Magnification { get; }

        /// <summary>
        /// Gets or sets the view mouse cursor.
        /// </summary>
        /// <value>The mouse cursor.</value>
        Cursor Cursor { get; set; }

        /// <summary>
        /// Gets the reference to diagram model.
        /// </summary>
        /// <value>The model.</value>
        Model Model { get; }

        /// <summary>
        /// Updates the view area.
        /// </summary>
        void UpdateView();

        /// <summary>
        /// Updates the view with the node bounds.
        /// </summary>
        void UpdateView(Node node);
    }
}
