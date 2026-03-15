//-------------------------------------------------------------------------------------------------
// <copyright file="GridProgressBarCell.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Security.Permissions;

using System.Diagnostics;
using System.Globalization;

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms.Tools;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Adds grid cell-specific keyboard logic to a <see cref="ProgressBarAdv"/>.
    /// </summary>
    [ToolboxItem(false)]
    public class GridProgressBar : ProgressBarAdv
    {
        GridProgressBarCellRenderer parent;

        /// <summary>
        /// Initializes a new <see cref="GridProgressBar"/> and attaches it to a <see cref="GridProgressBarCellRenderer"/>.
        /// </summary>
        /// <param name="parent">Parent cell renderer.</param>
        public GridProgressBar(GridProgressBarCellRenderer parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Gets the associated cell renderer for the text box.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridProgressBarCellRenderer ParentCell
        {
            get
            {
                return this.parent;
            }
        }
    }

    /// <summary>
    /// Implements the data / model part for a progress bar cell.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridProgressBarCellModel"/> can serve as a model for several <see cref="GridProgressBarCellRenderer"/>
    /// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridProgressBarCellRenderer"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class GridProgressBarCellModel : GridStaticCellModel
    {
        /// <overload>
        /// Initializes a new <see cref="GridProgressBarCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridProgressBarCellModel"/> object 
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>    
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridProgressBarCellModel(GridModel grid)
            : base(grid)
        {
            AllowFloating = true;
        }

        /// <summary>
        /// Initializes a new <see cref="GridProgressBarCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridProgressBarCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <override/>
        /// <summary>Creates a renderer for this cell model.</summary>
        /// <param name="control">The grid control.</param>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridProgressBarCellRenderer(control, this);
        }

        /// <summary>
        /// Initializes a <see cref="ProgressBarAdv"/> with information supplied by a <see cref="GridStyleInfo"/>
        /// </summary>
        /// <param name="pb">The control to be initialized.</param>
        /// <param name="style">The style with settings to be applied.</param>
        public static void InitProgressBarProperties(ProgressBarAdv pb, GridStyleInfo style)
        {
            GridProgressBarInfo info = style.ProgressBar;

            pb.BeginInit();
            pb.Minimum = info.Minimum;
            pb.Maximum = info.Maximum;
            pb.Value = info.ProgressValue;
            pb.Step = info.Step;
            ////pb.WaitingGradientWidth = info.WaitingGradientWidth;
            ////pb.WaitingGradientEnabled = info.WaitingGradientEnabled;
            ////pb.WaitingGradientInterval = info.WaitingGradientInterval;
            pb.ForeSegments = info.ForeSegments;
            pb.StretchMultGrad = info.StretchMultGrad;
            pb.MultipleColors = info.MultipleColors;
            pb.GradientStartColor = info.GradientStartColor;
            pb.GradientEndColor = info.GradientEndColor;
            pb.TubeStartColor = info.TubeStartColor;
            pb.TubeEndColor = info.TubeEndColor;
            pb.BackSegments = info.BackSegments;
            pb.BackMultipleColors = info.BackMultipleColors;
            pb.BackGradientStartColor = info.BackGradientStartColor;
            pb.BackGradientEndColor = info.BackGradientEndColor;
            pb.BackTubeStartColor = info.BackTubeStartColor;
            pb.BackTubeEndColor = info.BackTubeEndColor;
            pb.StretchImage = info.StretchImage;
            pb.BackgroundImage = info.BackgroundImage;
            pb.ForegroundImage = info.ForegroundImage;
            pb.SegmentWidth = info.SegmentWidth;
            pb.FontColor = info.FontColor;
            pb.ForeColor = info.ForeColor;
            pb.TextVisible = info.TextVisible;
            pb.TextStyle = info.TextStyle;
            pb.ProgressOrientation = info.ProgressOrientation;
            pb.TextShadow = info.TextShadow;
            pb.ProgressStyle = info.ProgressStyle;
            pb.ProgressFallbackStyle = info.ProgressFallbackStyle;
            pb.BackgroundStyle = info.BackgroundStyle;
            pb.BackgroundFallbackStyle = info.BackgroundFallbackStyle;
            pb.EndInit();

            //// Not used:
            ////pb.BackColor

            //// Assign text end backcolor outside BeginInit / EndInit block.
            ////mb.Text = info.Text;
        }
    }

    /// <summary>
    /// Implements the renderer part of a progress bar cell.
    /// </summary>
    /// <remarks>
    /// Use the <see cref="GridStyleInfo.ProgressBar"/> (<see cref="GridProgressBarInfo"/>) property
    /// of a <see cref="GridStyleInfo"/> to change progress bar properties for a cell.
    /// <para/>
    /// The ProgressBar control has background, border, and foreground styles.
    /// The background styles are <see cref="ProgressBarBackgroundStyles"/>.
    /// The border styles are <see cref="BorderStyle"/>.
    /// The foreground styles are <see cref="ProgressBarStyles"/>.
    /// <para/>
    /// The following table lists some characteristics about the ProgressBar cell type:
    /// <para/>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Item</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term>CellType</term>
    ///         <description>ProgressBar</description>
    ///     </item>
    ///     <item>
    ///         <term>Renderer</term>
    ///         <description><see cref="GridProgressBarCellRenderer"/></description>
    ///     </item>
    ///     <item>
    ///         <term>Model</term>
    ///         <description><see cref="GridProgressBarCellModel"/></description>
    ///     </item>
    ///     <item>
    ///         <term>XP Themes Support</term>
    ///         <description>Yes</description>
    ///     </item>
    ///     <item>
    ///         <term>Interactive</term>
    ///         <description>Click Only</description>
    ///     </item>
    ///     <item>
    ///         <term>Control</term>
    ///         <description><see cref="ProgressBarAdv"/></description>
    ///     </item>
    ///     <item>
    ///         <term>Floating Support</term>
    ///         <description>No</description>
    ///     </item>
    ///     <item>
    ///         <term>Base Type</term>
    ///         <description><see cref="GridStaticCellRenderer"/></description>
    ///     </item>
    /// </list>
    /// <para/>
    /// <para/>
    /// The cells behavior and appearance can be customized with the following properties of the <see cref="GridStyleInfo"/> class:
    /// <para/>
    /// <list type="table">
    ///     <listheader>
    ///         <term>PropertyName</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.BaseStyle"/> (<see cref="System.String"/>)</term>
    ///         <description>The base style for this style instance with default values for properties that are not initialized for this style object. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Borders"/> (<see cref="GridBordersInfo"/>)</term>
    ///         <description>Top, left, bottom, and right border settings. (Default: GridBordersInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellAppearance"/> (<see cref="GridCellAppearance"/>)</term>
    ///         <description>Specifies if cell edges shall be drawn raised, sunken, or flat (default). (Default: GridCellAppearance.Flat)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellTipText"/> (<see cref="System.String"/>)</term>
    ///         <description>ToolTip text to be displayed when user hovers mouse over cell. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellType"/> (<see cref="System.String"/>)</term>
    ///         <description>ProgressBar (Default: Text Box)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Enabled"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if the cell can be activated as current cell or if the cell should be skipped when moving the current cell. (Default: True)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Error"/> (<see cref="System.String"/>)</term>
    ///         <description>Holds error information if a value could not be converted to the <see cref="System.Type"/> specified with <see cref="GridStyleInfo.CellValueType"/>. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Font"/> (<see cref="GridFontInfo"/>)</term>
    ///         <description>The font for drawing text. (Default: GridFontInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Interior"/> (<see cref="Syncfusion.Drawing.BrushInfo"/>)</term>
    ///         <description> Lets you specify a solid backcolor, gradient, or pattern style with both back and forecolor for a cell's
    ///  background. (Default: SystemColors.Window)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ProgressBar"/> (<see cref="GridProgressBarInfo"/>)</term>
    ///         <description>A nested object with ProgressBar properties for a cell.  (Default: GridProgressBarInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ReadOnly"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if cell contents can be modified by the user. You can programmatically change Read-only cells by setting <see cref="GridModel.DiscardReadOnly"/> to True. (Default: False)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Themed"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if cell should be drawn using Windows XP themes when <see cref="GridControlBase.ThemesEnabled"/> has been set.  (Default: True)</description>
    ///     </item>
    /// </list>
    /// <para/>
    /// </remarks>
    public class GridProgressBarCellRenderer : GridStaticCellRenderer
    {
        private GridProgressBar progressBar;

        /// <summary>
        /// Initializes a new GridProgressBarCellRenderer object for the given GridControlBase
        /// and GridCellModelBase.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that display this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridCellModelBase"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase 
        /// and GridCellModelBase will be saved.</remarks>
        public GridProgressBarCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            progressBar = new GridProgressBar(this);
        }

        void FixParent(GridProgressBar progressBar)
        {
            if (this.progressBar.Parent != Grid)
            {
                this.progressBar.Visible = true;
                this.progressBar.Location = new Point(10000, 10000);
                this.progressBar.CausesValidation = false;
                Grid.Controls.Add(progressBar);
                this.progressBar.Visible = false;
                this.progressBar.ThemesEnabled = XPThemes.IsAppThemed;
            }
        }

        /// <override/>
        protected override void OnDraw(System.Drawing.Graphics g, System.Drawing.Rectangle clientRectangle, int rowIndex, int colIndex, Syncfusion.Windows.Forms.Grid.GridStyleInfo style)
        {
            // Get the client bounds taking floated cells into consideration.
            Rectangle bounds = GetCellClientRectangle(rowIndex, colIndex, style, true);

            GridProgressBarCellModel.InitProgressBarProperties(this.progressBar, style);
            FixParent(this.progressBar);
            progressBar.BorderStyle = BorderStyle.None;
            progressBar.BackColor = Color.FromArgb(255, Grid.GetBackColor(style.Interior.BackColor));
            bool isTextRightToLeft = (style.RightToLeft == RightToLeft.Inherit && Grid.IsRightToLeft()) || style.RightToLeft == RightToLeft.Yes;
            ////progressBar.RightToLeft = isTextRightToLeft ? RightToLeft.Yes : RightToLeft.No;
            ////progressBar.ForeColor = style.TextColor;

            ////TraceUtil.TraceCurrentMethodInfo(progressBar.Minimum, progressBar.Maximum, progressBar.Value);
            progressBar.Draw(g, clientRectangle, isTextRightToLeft);
        }
    }
}
