#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    ///     Delegate that is to be used with chart region related mouse events.
    /// </summary>
    /// <param name="sender" type="object">
    ///     <para>
    ///     Sender.    
    ///     </para>
    /// </param>
    /// <param name="e" type="Syncfusion.Windows.Forms.Chart.ChartRegionMouseEventArgs">
    ///     <para>
    ///     Event argument.    
    ///     </para>
    /// </param>
    public delegate void ChartRegionMouseEventHandler(object sender, ChartRegionMouseEventArgs e);

    /// <summary>
    ///     Delegate that is to be used with chart area image drawing events.
    /// </summary>
    /// <param name="sender" type="object">
    ///     <para>
    ///     Sender.    
    ///     </para>
    /// </param>
    /// <param name="e" type="Syncfusion.Windows.Forms.Chart.ChartAreaImageEventArgs">
    ///     <para>
    ///     Event argument.    
    ///     </para>
    /// </param>
    public delegate void ChartAreaImageEventHandler(object sender, ChartAreaImageEventArgs e);

    /// <summary>
    ///    Argument that is to be used with chart region related mouse events. 
    /// </summary>
    public class ChartRegionMouseEventArgs : EventArgs
    {
        #region Members
        private ChartRegion m_region;
        private Point m_clickPoint;
        private MouseButtons m_button;
        #endregion

        #region Properties

        /// <summary>
        /// Gets the region.
        /// </summary>
        /// <value>The region.</value>
        public ChartRegion Region
        {
            get
            {
                return m_region;
            }
        }
        
        /// <summary>
        /// Gets the point.
        /// </summary>
        /// <value>The point.</value>
        public Point Point
        {
            get
            {
                return m_clickPoint;
            }
        }

        /// <summary>
        /// Gets the button.
        /// </summary>
        /// <value>The button.</value>
        public MouseButtons Button
        {
            get
            {
                return m_button;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartRegionMouseEventArgs"/> class.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="clickPoint">The click point.</param>
        public ChartRegionMouseEventArgs(ChartRegion region, Point clickPoint)
        {
            m_region = region;
            m_clickPoint = clickPoint;
            m_button = MouseButtons.None;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartRegionMouseEventArgs"/> class.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="clickPoint">The click point.</param>
        /// <param name="button">The button.</param>

        public ChartRegionMouseEventArgs(ChartRegion region, Point clickPoint, MouseButtons button)
        {
            m_region = region;
            m_clickPoint = clickPoint;
            m_button = button;
        }

        #endregion
    }

    /// <summary>
    ///  Argument that is to be used with chart area image drawing events.
    /// </summary>
    public class ChartAreaImageEventArgs : EventArgs
    {
        #region Members
        private Image m_bufferImage = null;
        private bool m_handled = false;
        private Point m_location = Point.Empty;
        private Size m_size = new Size(400, 300);
        #endregion

        #region Constructors
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAreaImageEventArgs"/> class.
        /// </summary>
        /// <param name="BufferImage">The buffer image.</param>
        public ChartAreaImageEventArgs(Image BufferImage)
        {
            m_bufferImage = BufferImage;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the buffer image.
        /// </summary>
        /// <value>The buffer image.</value>
        public Image BufferImage
        {
            get
            {
                return m_bufferImage;
            }
            set
            {
                if (value != m_bufferImage)
                {
                    m_bufferImage = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ChartAreaImageEventArgs"/> is handled.
        /// </summary>
        /// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
        [Description("Gets or sets a value indicating whether this ChartAreaImageEventArgs is handled."), DefaultValue(false)]
        public bool Handled
        {
            get
            {
                return m_handled;
            }
            set
            {
                if (value != m_handled)
                {
                    m_handled = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the location of the image.
        /// </summary>
        /// <value>The location.</value>
        [Description("Gets or sets the location of the image.")]
        public Point Location
        {
            get
            {
                return m_location;
            }
            set
            {
                if (value != m_location)
                {
                    m_location = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the image.
        /// </summary>
        /// <value>The size.</value>
        [Description("Gets or sets the size of the image.")]
        public Size Size
        {
            get
            {
                return m_size;
            }
            set
            {
                if (value != m_size)
                {
                    m_size = value;
                }
            }
        }

        #endregion
    }
}