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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Chart
{
    #region delegates
    /// <summary>
    /// Delegate that is to be used with ChartLegend.FilterItems event. This Event is fired when the legend items need to be filtered.
    /// Handle this event to change the collection of LegendItems that the legend contains.
    /// </summary>
    /// <param name="sender" type="object">
    ///     <para>
    ///     Sender.
    ///     </para>
    /// </param>
    /// <param name="e" type="Syncfusion.Windows.Forms.Chart.ChartLegendFilterItemsEventArgs">
    ///     <para>
    ///     Argument.
    ///     </para>
    /// </param>
    public delegate void LegendFilterItemsEventHandler(object sender, ChartLegendFilterItemsEventArgs e);

    /// <summary>
    /// Delegate that is to be used with ChartLegend.DrawItem event. This event is fired when a legend item needs to draw. Handle this event to change the drawing of items.
    /// </summary>
    /// <param name="sender" type="object">
    ///     <para>
    ///     Sender.
    ///     </para>
    /// </param>
    /// <param name="e" type="Syncfusion.Windows.Forms.Chart.ChartLegendDrawItemEventArgs">
    ///     <para>
    ///     Argument.
    ///     </para>
    /// </param>
    public delegate void LegendDrawItemEventHandler(object sender, ChartLegendDrawItemEventArgs e);

    /// <summary>
    /// Delegate that is to be used with ChartLegend.MinSize event. This event is fired when the legend's minimum size is to be fixed.
    /// </summary>
    /// <param name="sender" type="object">
    ///     <para>
    ///     Sender.
    ///     </para>
    /// </param>
    /// <param name="e" type="Syncfusion.Windows.Forms.Chart.ChartLegendMinSizeEventArgs">
    ///     <para>
    ///     Argument.
    ///     </para>
    /// </param>
    public delegate void ChartLegendMinSizeEventHandler(object sender, ChartLegendMinSizeEventArgs e);

    /// <summary>
    /// Delegate that is to be used with ChartLegend.DrawItemText event. This event is fired when a legend item text needs to draw. Handle this event to change the drawing of items text.
    /// </summary>
    /// <param name="sender" type="object">
    ///     <para>
    ///     Sender.
    ///     </para>
    /// </param>
    /// <param name="e" type="Syncfusion.Windows.Forms.Chart.ChartLegendDrawItemTextEventArgs">
    ///     <para>
    ///     Argument.
    ///     </para>
    /// </param>
    public delegate void LegendDrawItemTextEventHandler(object sender, ChartLegendDrawItemTextEventArgs e);

    #endregion
    public class ChartLegendDrawItemTextEventArgs : EventArgs
    {
        #region Members

        private Graphics graphics;
        private bool handled = false;
        private string text;
        private RectangleF m_textrect;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the graphics.
        /// </summary>
        /// <value>The graphics.</value>
        public Graphics Graphics
        {
            get
            {
                return graphics;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="LegendDrawItemTextEventArgs"/> is handled.
        /// </summary>
        /// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
        public bool Handled
        {
            get
            {
                return handled;
            }

            set
            {
                handled = value;
            }
        }

        /// <summary>
        /// Gets or sets text of Legend item
        /// </summary>
        /// <value>The graphics.</value>
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
            }
        }

        /// <summary>
        /// Gets bounds of legend item
        /// </summary>
        /// <value>The graphics.</value>
        public RectangleF TextRect
        {
            get
            {
                return m_textrect;
            }

        }
        #endregion

        #region Constructor
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="g" type="System.Drawing.Graphics">
        ///     <para>
        ///     Graphics object.
        ///     </para>
        /// </param>
        /// <param name="text" type="Syncfusion.Windows.Forms.Chart.LegendItem">
        ///     <para>
        ///     Legend item text to be rendered.
        ///     </para>
        /// </param>
        /// <param name="m_textRect" type="System.Drawing.Point">
        ///     <para>
        ///      Bounds of the legend item.
        ///     </para>
        /// </param>     
        public ChartLegendDrawItemTextEventArgs(Graphics g, string text, RectangleF m_textRect)
        {
            this.graphics = g;
            this.text = text;
            this.m_textrect = m_textRect;           
        }
        #endregion
    }

    /// <summary>
    /// Event argument that is to be used with ChartLegend.FilterItems event. This
    /// event is raised before the legend items are rendered. This can be used to remove any item conditionally.
    /// </summary>
    public class ChartLegendFilterItemsEventArgs : EventArgs
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartLegendFilterItemsEventArgs"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        public ChartLegendFilterItemsEventArgs(ChartLegendItem[] items)
        {
            m_items = new ChartLegendItemsCollection();

            for (int i = 0, end = items.Length; i < end; i++)
            {
                m_items.Add(items[i]);
            }
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="items" type="Syncfusion.Windows.Forms.Chart.LegendItem[]">
        ///     <para>
        ///     Legend items that are to be rendered.
        ///     </para>
        /// </param>
        public ChartLegendFilterItemsEventArgs(ChartLegendItemsCollection items)
        {
            m_items = items;
        }

        #endregion

        #region Properties
        /// <summary>
        ///     Gets or sets the legend items that are to be rendered.
        /// </summary>
        public ChartLegendItemsCollection Items
        {
            get
            {
                return m_items;
            }

            set
            {
                m_items = value;
            }
        }

        #endregion

        #region Members
        private ChartLegendItemsCollection m_items;
        #endregion
    }

    /// <summary>
    ///  Delegate that is to be used with ChartLegend.DrawItem event. This event is fired when a legend item needs to draw. Handle this event to change the drawing of items.
    /// </summary>
    public class ChartLegendDrawItemEventArgs : EventArgs
    {
        #region members

        private Graphics graphics;
        private bool handled = false;
        private int index = -1;
        private ChartLegendItem legendItem;
        private Rectangle m_bounds;

        #endregion

        #region properties
        
        /// <summary>
        /// Gets the graphics.
        /// </summary>
        /// <value>The graphics.</value>
        public Graphics Graphics
        {
            get
            {
                return graphics;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ChartLegendDrawItemEventArgs"/> is handled.
        /// </summary>
        /// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
        public bool Handled
        {
            get
            {
                return handled;
            }

            set
            {
                handled = value;
            }
        }

        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <value>The index.</value>
        public int Index
        {
            get
            {
                return index;
            }
        }

        /// <summary>
        /// Gets the legend item.
        /// </summary>
        /// <value>The legend item.</value>
        public ChartLegendItem LegendItem
        {
            get
            {
                return legendItem;
            }
        }

        /// <summary>
        /// Gets or sets the location of the item.
        /// </summary>
        public Point Location
        {
            get
            {
                return m_bounds.Location;
            }

            set
            {
                m_bounds.Location = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the item.
        /// </summary>
        public Size Size
        {
            get
            {
                return m_bounds.Size;
            }

            set
            {
                m_bounds.Size = value;
            }
        }

        /// <summary>
        /// Gets or sets the bounds of the item. 
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return m_bounds;
            }

            set
            {
                m_bounds = value;
            }
        }
        #endregion

        #region constructor
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="g" type="System.Drawing.Graphics">
        ///     <para>
        ///     Graphics object.
        ///     </para>
        /// </param>
        /// <param name="item" type="Syncfusion.Windows.Forms.Chart.LegendItem">
        ///     <para>
        ///     Legend item to be rendered.
        ///     </para>
        /// </param>
        /// <param name="loc" type="System.Drawing.Point">
        ///     <para>
        ///     Location of the legend item.
        ///     </para>
        /// </param>
        /// <param name="index" type="int">
        ///     <para>
        ///     Index value of the legend item being rendered.
        ///     </para>
        /// </param>
        public ChartLegendDrawItemEventArgs(Graphics g, ChartLegendItem item, Point loc, int index)
        {
            this.graphics = g;
            this.legendItem = item;
            m_bounds = new Rectangle(loc, Size.Empty);
            this.index = index;
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="g" type="System.Drawing.Graphics">
        ///     <para>
        ///     Graphics object.
        ///     </para>
        /// </param>
        /// <param name="item" type="Syncfusion.Windows.Forms.Chart.LegendItem">
        ///     <para>
        ///     Legend item to be rendered.
        ///     </para>
        /// </param>
        /// <param name="bounds" type="System.Drawing.Point">
        ///     <para>
        ///     Bounds of the legend item.
        ///     </para>
        /// </param>
        /// <param name="index" type="int">
        ///     <para>
        ///     Index value of the legend item being rendered.
        ///     </para>
        /// </param>
        public ChartLegendDrawItemEventArgs(Graphics g, ChartLegendItem item, Rectangle bounds, int index)
        {
            this.graphics = g;
            this.legendItem = item;
            m_bounds = bounds;
            this.index = index;
        }
        #endregion
    }

    /// <summary>
    /// Argument that is to be used with ChartLegend.MinSize event. This event is fired when the legend's minimum size is to be fixed.
    /// </summary>
    public class ChartLegendMinSizeEventArgs : EventArgs
    {
        #region Constructor
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="size" type="System.Drawing.Size">
        ///     <para>
        ///     Size to be used for the legend.
        ///     </para>
        /// </param>
        /// <returns>
        ///     A void value.
        /// </returns>
        public ChartLegendMinSizeEventArgs(Size size)
        {
            this.size = size;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ChartLegendMinSizeEventArgs"/> is handled.
        /// </summary>
        /// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
        public bool Handled
        {
            get
            {
                return handled;
            }

            set
            {
                handled = value;
            }
        }

        /// <summary>
        ///     Gets or sets the minimum size to be used for the legend.
        /// </summary>
        public Size Size
        {
            get
            {
                return size;
            }

            set
            {
                size = value;
            }
        }

        #endregion

        #region Members
        private bool handled = false;
        private Size size;
        #endregion
    }
}