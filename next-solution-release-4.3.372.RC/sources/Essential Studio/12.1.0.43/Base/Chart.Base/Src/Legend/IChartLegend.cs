#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing;
using System.Text;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    ///  Specifies the default properties of legend.
    /// </summary>  
    public interface IChartLegend
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        ChartDock Position
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        ChartOrientation Orientation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the items text aligment.
        /// </summary>
        /// <value>The items text aligment.</value>
        VerticalAlignment ItemsTextAligment
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the items alignment.
        /// </summary>
        /// <value>The items alignment.</value>
        StringAlignment ItemsAlignment
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the representation.
        /// </summary>
        /// <value>The type of the representation.</value>
        ChartLegendRepresentationType RepresentationType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the alignment.
        /// </summary>
        /// <value>The alignment.</value>
        ChartAlignment Alignment
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size of the items.
        /// </summary>
        /// <value>The size of the items.</value>
        Size ItemsSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the items shadow offset.
        /// </summary>
        /// <value>The items shadow offset.</value>
        Size ItemsShadowOffset
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color of the items shadow.
        /// </summary>
        /// <value>The color of the items shadow.</value>
        Color ItemsShadowColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the spacing.
        /// </summary>
        /// <value>The spacing.</value>
        int Spacing
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the rows count.
        /// </summary>
        /// <value>The rows count.</value>
        int RowsCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the columns count.
        /// </summary>
        /// <value>The columns count.</value>
        int ColumnsCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show symbol].
        /// </summary>
        /// <value><c>true</c> if [show symbol]; otherwise, <c>false</c>.</value>
        bool ShowSymbol
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [only columns for floating].
        /// </summary>
        /// <value>
        ///    <c>true</c> if [only columns for floating]; otherwise, <c>false</c>.
        /// </value>
        bool OnlyColumnsForFloating
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [floating auto size].
        /// </summary>
        /// <value><c>true</c> if [floating auto size]; otherwise, <c>false</c>.</value>
        bool FloatingAutoSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [visible check box].
        /// </summary>
        /// <value><c>true</c> if [visible check box]; otherwise, <c>false</c>.</value>
        bool VisibleCheckBox
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show items shadow].
        /// </summary>
        /// <value><c>true</c> if [show items shadow]; otherwise, <c>false</c>.</value>
        bool ShowItemsShadow
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [set def size for custom].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [set def size for custom]; otherwise, <c>false</c>.
        /// </value>
        bool SetDefSizeForCustom
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>The font.</value>
        Font Font { get; set; }

        /// <summary>
        /// Gets or sets the background color of the legend.
        /// </summary>
        Color BackColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the foreground color of the legend.
        /// </summary>
        Color ForeColor
        {
            get;
            set;
        }

        /// <summary>
        /// Fired when the legend items need to be filtered. Handle this event to change the collection of ChartLegendItems that the legend contains.
        /// </summary>
        event LegendFilterItemsEventHandler FilterItems;
    }
}
