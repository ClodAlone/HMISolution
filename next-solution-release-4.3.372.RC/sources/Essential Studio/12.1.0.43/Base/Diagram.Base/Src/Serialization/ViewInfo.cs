#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Serializable class used for persisting the <see cref="Syncfusion.Windows.Forms.Diagram.View"/> data. 
    /// </summary>
    /// <remarks>
    /// When using a custom View for the diagram control that contains its own persistence data, use a 
    /// subclass of the ViewInfo type for serializing the new View data.
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.View.GetViewInfo"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.View.SetViewInfo"/>
    /// </remarks>
    [Serializable]
    public class ViewInfo : ISerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewInfo"/> class.
        /// </summary>
        /// <param name="srcview">The view.</param>
        public ViewInfo(View srcview)
        {
            this.rcBounds = srcview.Bounds;
            this.ptOrigin = srcview.Origin;
            this.backgroundColor = srcview.BackgroundColor;
            this.handleColor = srcview.HandleRenderer.HandleColor;
            this.handleOutlineColor = srcview.HandleRenderer.HandleOutlineColor;
            
            // this.handleAnchorColor = srcview.HandleAnchorColor;
            this.handleDisabledColor = srcview.HandleRenderer.HandleDisabledColor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewInfo"/> class.
        /// </summary>
        /// <param name="info">SerializationInfo object to populate.</param>
        /// <param name="context">Destination streaming context.</param>
        protected ViewInfo(SerializationInfo info, StreamingContext context)
        {
            this.rcBounds = (System.Drawing.Rectangle)info.GetValue("bounds", typeof(System.Drawing.Rectangle));
            this.ptOrigin = (PointF)info.GetValue("origin", typeof(PointF));
            this.backgroundColor = (System.Drawing.Color)info.GetValue("backgroundColor", typeof(System.Drawing.Color));
            this.handleColor = (System.Drawing.Color)info.GetValue("handleColor", typeof(System.Drawing.Color));
            this.handleOutlineColor = (System.Drawing.Color)info.GetValue("handleOutlineColor", typeof(System.Drawing.Color));
            this.handleAnchorColor = (System.Drawing.Color)info.GetValue("handleAnchorColor", typeof(System.Drawing.Color));
            this.handleDisabledColor = (System.Drawing.Color)info.GetValue("handleDisabledColor", typeof(System.Drawing.Color));
            this.szMagnification = (System.Drawing.Size)info.GetValue("magnification", typeof(System.Drawing.Size));
        }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">SerializationInfo object to populate.</param>
        /// <param name="context">Destination streaming context.</param>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            this.GetObjectData(info, context);
        }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">SerializationInfo object to populate.</param>
        /// <param name="context">Destination streaming context.</param>
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("bounds", this.rcBounds);
            info.AddValue("origin", this.ptOrigin);
            info.AddValue("backgroundColor", this.backgroundColor);
            info.AddValue("handleSize", this.handleSize);
            info.AddValue("handleColor", this.handleColor);
            info.AddValue("handleOutlineColor", this.handleOutlineColor);
            info.AddValue("handleAnchorColor", this.handleAnchorColor);
            info.AddValue("handleDisabledColor", this.handleDisabledColor);
            info.AddValue("magnification", this.szMagnification);
        }

        /// <summary>
        /// The bounding rectangle.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal System.Drawing.Rectangle rcBounds;

        /// <summary>
        /// Origin of the view.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal PointF ptOrigin;

        /// <summary>
        /// Background color.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal System.Drawing.Color backgroundColor;

        /// <summary>
        /// Size of the handle.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal int handleSize;

        /// <summary>
        /// Color of the handle.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal System.Drawing.Color handleColor;

        /// <summary>
        /// Outline color of the handle.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal System.Drawing.Color handleOutlineColor;

        /// <summary>
        /// Color of the handle anchor.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal System.Drawing.Color handleAnchorColor;

        /// <summary>
        /// Color of the disabled color.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal System.Drawing.Color handleDisabledColor;

        /// <summary>
        /// Size of the magnification.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal Size szMagnification;
    }
}
