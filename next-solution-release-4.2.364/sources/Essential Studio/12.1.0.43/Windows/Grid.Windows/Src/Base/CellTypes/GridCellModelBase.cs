//-------------------------------------------------------------------------------------------------
// <copyright file="GridCellModelBase.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;

using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Defines the data / model part of a cell type. Settings can be serialized out to a file together with a <see cref="GridModel"/>.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridCellModelBase"/> can serve as model for several <see cref="GridCellRendererBase"/>
    /// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// </remarks>
    [Serializable]
    public class GridCellModelBase : Disposable, ISerializable, IDisposable
    {
        // Fields
        [NonSerialized] GridModel grid;
        Size buttonBarSize = Size.Empty;
        [NonSerialized] internal GridCellContextValue activeTextValue = new GridCellContextValue(null);
        [NonSerialized] GridCellContextValue activeTextChanged = new GridCellContextValue(false);

        /// <overload>
        /// Releases the all resources used by the component.
        /// </overload>
        /// <summary>
        /// Releases the all resources used by the component.
        /// </summary>
        public new void Dispose()
        {
            this.inDispose = true;
            this.Dispose(true);
            GC.SuppressFinalize(this);
            this.inDispose = false;
            this.isDisposed = true;
        }

        bool inDispose = false;
        bool isDisposed = false;

        /// <summary>
        /// Gets a value indicating whether the object is executing <see cref="Dispose()"/> method call.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsDisposing
        {
            get
            {
                return this.inDispose;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the object has been disposed.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsDisposed
        {
            get
            {
                return this.isDisposed;
            }
        }

        internal bool coveredCellFullRect = true;

        /// <summary>
        /// Gets or sets a value indicating whether covered cells need to be drawn passing in the complete
        /// coordinates of the covered cell even if parts of the covered cell are above the
        /// current view.
        /// </summary>
        /// <remarks>
        /// If you expect your cell type to be used in a scenario where a cell can span
        /// over 100s of rows, you should set this property to true and provide an optimized
        /// draw routine in the cell renderer that accepts a clipped rectangle. <para/>
        /// The GridGroupingControls GridNestedTableControl uses this flag to optimize drawing
        /// of nested tables. Nested tables are all drawn in a child cell and can span thousands
        /// of rows. Instead of calculating the whole rectangle for the nested cell, the grid
        /// can pass in just the visible bounds. This improves performance of these nested
        /// cells a lot.
        /// </remarks>
        [DefaultValue(true)]
        public bool ForceCoveredCellFullBounds
        {
            get
            {
                return this.coveredCellFullRect;
            }

            set
            {
                this.coveredCellFullRect = value;
            }
        }

        /// <summary>
        /// Gets the name of the cell type. May later be used as key for looking up the cell type.
        /// </summary>
        internal string Name
        {
            get
            {
                return GetType().Name;
            }
        }

        /// <summary>
        /// Gets the display name of the cell type. May later be used to display cell in a selection dialog.
        /// or property grid.
        /// </summary>
        internal string DisplayName
        {
            get
            {
                return this.GetLocalizedString(Name);
            }
        }

        BindingContext bindingContext = null;

        /// <summary>
        /// Gets or sets the <see cref="BindingContext"/> for this object. You can assign a <see cref="BindingContext"/>
        /// form a parent form to this property.
        /// </summary>
        public BindingContext BindingContext
        {
            get
            {
                return this.bindingContext;
            }

            set
            {
                if (this.bindingContext != value)
                {
                    this.bindingContext = value;
                    this.OnBindingContextChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Called when the BindingContext property was changed.
        /// </summary>
        /// <param name="e">An empty EventArgs</param>
        protected virtual void OnBindingContextChanged(EventArgs e)
        {
        }

        /// <summary>
        /// Gets a description of the cell type.
        /// </summary>
        public virtual string Description
        {
            get
            {
                return this.GetLocalizedString(Name + "Desc");
            }
        }

        /// <summary>
        /// Returns a localized string from the SR.txt resource file.
        /// </summary>
        /// <param name="value">The key to look up.</param>
        /// <returns>The localized string.</returns>
        protected virtual string GetLocalizedString(string value)
        {
            return SR.GetString(value);
        }

        /// <overload>
        /// Initializes a new GridCellModelBase object and stores a reference to the GridModel this cell belongs to.
        /// </overload>
        /// <summary>
        /// Initializes a new GridCellModelBase object and stores a reference to the GridModel this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> that owns this model.</param>
        public GridCellModelBase(GridModel grid)
        {
            this.grid = grid;
        }

        /// <summary>
        /// Called from GridModel implementation of IDeserializationCallback.OnDeserialization.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="model">The <see cref="GridModel"/> that was deserialized.</param>
        protected virtual void OnModelDeserialization(object sender, GridModel model)
        {
            this.grid = model;
        }

        internal void RaiseModelDeserialization(object sender, GridModel model)
        {
            this.OnModelDeserialization(sender, model);
        }

        internal int serializeSchemeVersion = 0;

        /// <summary>
        /// Gets the version information when cell model is deserialized.
        /// </summary>
        public int SerializeSchemeVersion
        {
            get
            {
                return this.serializeSchemeVersion;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="GridCellModelBase"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridCellModelBase(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif

            this.activeTextValue = new GridCellContextValue(null);
            this.activeTextChanged = new GridCellContextValue(false);

            foreach (SerializationEntry entry in info)
            {
                if (entry.Name == "Version")
                {
                    this.serializeSchemeVersion = Convert.ToInt32(entry.Value);
                    break;
                }
            }

            this.buttonBarSize = (Size) info.GetValue("ButtonBarSize", typeof(Size));
        }

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the cell model.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the cell model.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter=true)]
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, SerializationFormatter=true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            info.AddValue("ButtonBarSize", this.buttonBarSize); // Size
            info.AddValue("Version", 1); // Version
        }

        /// <summary>
        /// Gets a reference to the GridModel.
        /// </summary>
        public GridModel Grid
        {
            get { return this.grid; }
        }

        /// <summary>
        /// Creates a renderer for this cell model that is specific to the GridControlBase.
        /// </summary>
        /// <param name="control">The <see cref="GridControlBase"/> the cell renderer is created for.</param>
        /// <returns>A new <see cref="GridCellRendererBase"/> specific for a <see cref="GridControlBase"/>.</returns>
        /// <remarks>You must override this method in your implementation of GridCellModelBase.</remarks>
        public virtual GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            throw new NotImplementedException("GridCellRenderer is not implemented in derived class");
            ////return null;
        }

        /// <summary>
        /// Creates a copy of this cell model for another GridModel.
        /// </summary>
        /// <param name="gridModel">The <see cref="GridModel"/> for the new control.</param>
        /// <returns>Returns the instance of the grid model</returns>
        public virtual GridCellModelBase CreateCopy(GridModel gridModel)
        {
            return (GridCellModelBase) Activator.CreateInstance(GetType(), new object[] { gridModel });
        }

        /// <summary>
        /// Gets or sets the total size of any buttons.
        /// </summary>
        /// <remarks>Set size.Height to int.MaxValue if button should fill cell and not be
        /// vertically aligned with text.
        /// </remarks>
        public Size ButtonBarSize
        {
            get
            {
                return this.buttonBarSize;
            }

            set
            {
                this.buttonBarSize = value;
            }
        }

        /// <summary>
        /// Adds border margins to given cell client area size. The borders are determined from a specified style with cell content information.
        /// </summary>
        /// <param name="size">The <see cref="System.Drawing.Size"/> with the cell size.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <returns>The <see cref="System.Drawing.Size"/> with the cell size including its borders.</returns>
        /// <remarks>
        /// calls  GridMargins.AddMargins(size, StyleInfoBordersToMargins(style));
        /// </remarks>
        internal Size AddBorders(Size size, GridStyleInfo style)
        {
            // Override this method if you have overridden GetCellRect.
            return this.Grid.AddBorders(size, style);
        }

        /// <overload>
        /// Removes border margins from a given cell rectangle. The borders are determined from a specified style with cell content information.
        /// </overload>
        /// <summary>
        /// Removes border margins from a given cell rectangle. The borders are determined from a specified style with cell content information.
        /// </summary>
        /// <param name="cellBounds">The <see cref="System.Drawing.Rectangle"/> with the cell bounds.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="isRightToLeft">Indicates if grid is in RightToLeft mode.</param>
        /// <returns>The <see cref="System.Drawing.Rectangle"/> with the cell bounds excluding its borders.</returns>
        /// <remarks>
        /// Calls GridMargins.RemoveMargins(cellBounds, StyleInfoBordersToMargins(style));
        /// </remarks>
        public Rectangle SubtractBorders(Rectangle cellBounds, GridStyleInfo style, bool isRightToLeft)
        {
            return this.Grid.SubtractBorders(cellBounds, style, isRightToLeft);
        }

        /// <summary>
        /// Removes border margins from a given cell rectangle. The borders are determined from a specified style with cell content information.
        /// </summary>
        /// <param name="cellBounds">The <see cref="System.Drawing.Rectangle"/> with the cell bounds.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <returns>The <see cref="System.Drawing.Rectangle"/> with the cell bounds excluding its borders.</returns>
        /// <remarks>
        /// calls GridMargins.RemoveMargins(cellBounds, StyleInfoBordersToMargins(style));
        /// </remarks>
        [Obsolete("It is recommended to specify isRightToLeft parameter, default for isRightToLeft is false.")]
        public Rectangle SubtractBorders(Rectangle cellBounds, GridStyleInfo style)
        {
            return this.Grid.SubtractBorders(cellBounds, style, false);
        }

        /// <overload>
        /// Calculates the preferred size of the cell based on its contents, including margins and any buttons.
        /// </overload>
        /// <summary>
        /// Calculates the preferred size of the cell based on its contents, including margins and any buttons.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="queryBounds">Vertical or horizontal</param>
        /// <returns>The optimal size of the cell.</returns>
        public virtual Size CalculatePreferredCellSize(Graphics g, int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            Size clientSize = this.OnQueryPrefferedClientSize(g, rowIndex, colIndex, style, queryBounds);
            Size cellSize = GridMargins.AddMargins(clientSize, this.Grid.StyleInfoBordersToMargins(style));
            bool isMenu = false;
            if (this.grid.ActiveGridView != null && this.Grid.ActiveGridView.GetType().ToString().Equals("Syncfusion.Windows.Forms.Tools.XPMenus.MenuGrid"))
            {
                isMenu = true;
            }

            if (!isMenu)
            {
                cellSize.Width += this.ButtonBarSize.Width;
                cellSize.Height += 1;
            }
            int buttonHeight = this.ButtonBarSize.Height;
            if (buttonHeight > 0 && buttonHeight < int.MaxValue)
            {
                cellSize.Height = Math.Max(cellSize.Height, buttonHeight);
            }

            return cellSize;
        }

        /// <summary>
        /// Calculates the preferred size of the cell based on its contents without margins and any buttons.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="queryBounds">grsphical bounds</param>
        /// <returns>The optimal size of the cell.</returns>
        protected virtual Size OnQueryPrefferedClientSize(Graphics g, int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            return WinFormsUtils.MeasureSampleWString(g, style.GdipFont);
        }

        /// <summary>
        /// Gets the preferred size to be used for an empty cell.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="font">The <see cref="Font"/> to be used.</param>
        /// <returns>The <see cref="Size"/> of the string "Wg;".</returns>
        public static Size MeasureSampleWString(Graphics g, Font font)
        {
            return g.MeasureString(MeasureEmptyCellString, font).ToSize();
        }

        /// <summary>
        /// Gets or sets the string which is used when doing a resize to fit for cells with empty text.
        /// </summary>
        public static string MeasureEmptyCellString
        {
            get
            {
                return WinFormsUtils.MeasureEmptyCellString;
            }

            set
            {
                WinFormsUtils.MeasureEmptyCellString = value;
            }
        }

        /// <summary>
        /// Gets the actual size of the cell including margins and cell buttons.
        /// Spanned cells will return the size of the whole cell
        /// covering neighboring cells.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>The actual <see cref="Size"/> of the cell including margins and cell buttons. </returns>
        public Size GetCellSize(int rowIndex, int colIndex)
        {
            GridRangeInfo range;
            this.Grid.GetSpannedRangeInfo(rowIndex, colIndex, out range);
            Size size = new Size((int) this.Grid.ColWidths.GetTotal(range.Left, range.Right), (int) this.Grid.RowHeights.GetTotal(range.Top, range.Bottom));
            return size;
        }

        /// <summary>
        /// Gets the actual size of the cell without margins and without cell buttons.
        /// Spanned cells will return the size of the whole cell
        /// covering neighboring cells.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <returns>The actual <see cref="Size"/> of the cell without margins or cell buttons.</returns>
        public Size GetCellClientSize(int rowIndex, int colIndex, GridStyleInfo style)
        {
            Size size = this.GetCellSize(rowIndex, colIndex);
            size = GridMargins.RemoveMargins(size, this.Grid.StyleInfoBordersToMargins(style));
            size.Width -= this.ButtonBarSize.Width;
            return size;
        }

        /// <summary>
        /// Determines whether the cell supports floating over a neighboring cell or can be
        /// flooded by a previous cell.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="query">A <see cref="GridQueryFloatCell"/> value that specifies whether a cell is asked
        /// about support for floating over another cell or being flooded by a previous cell.</param>
        /// <returns>True if floating is possible; False otherwise.</returns>
        public virtual bool OnQueryCanFloatCell(int rowIndex, int colIndex, GridStyleInfo style, GridQueryFloatCell query)
        {
            return false;
        }

        /// <summary>
        /// Determines whether the cell supports merging of neighboring cells.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="mergeCellDirection">Specifies if rows or columns should be merged.</param>
        /// <returns>True if merging is possible; False otherwise.</returns>
        public virtual bool OnQueryCanMergeCell(int rowIndex, int colIndex, GridStyleInfo style, GridMergeCellDirection mergeCellDirection)
        {
            return false;
        }

        /// <summary>
        /// Returns the display text of the specified cell. If it is the current cell the active text is returned.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <returns>A <see cref="String"/> with the display text of the specified cell.</returns>
        public string GetFormattedOrActiveTextAt(int rowIndex, int colIndex, GridStyleInfo style)
        {
            string text = this.GetActiveText(rowIndex, colIndex);
            if (text != null)
            {
                return text;
            }
            else
            {
                return this.GetFormattedText(style, style.CellValue, GridCellBaseTextInfo.DisplayText);
            }
        }

        /// <summary>
        /// This is called from GridStyleInfo.GetFormattedText.
        /// GridStyleInfo.CultureInfo is used for conversion to string.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value to format.</param>
        /// <param name="textInfo">TextInfo is a hint of who is calling, default is GridCellBaseTextInfo.DisplayText.</param>
        /// <returns>The formatted text for the given value.</returns>
        public virtual string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            GridCellTextEventArgs ea = new GridCellTextEventArgs(string.Empty, style, value, textInfo);
            this.Grid.RaiseQueryCellFormattedText(ea);
            if (ea.Handled)
            {
                return ea.Text;
            }
            else
            {
                CultureInfo ci = style.CultureInfo;
                NumberFormatInfo nfi = ci != null ? ci.NumberFormat : null;
                ////GridNumberFormatInfoStyleProperty ognfi = null;
                // TODO: the following code costs immense performance !
                //            ognfi = ((GridNumberFormatInfoStyleProperty) CustomStyleProperties[typeof(GridNumberFormatInfoStyleProperty)]);
                //            if (ognfi != null)
                //                nfi = (NumberFormatInfo) ognfi.GetFormat(typeof(NumberFormatInfo));
                return GridCellValueConvert.FormatValue(value, style.CellValueType, style.Format, ci, nfi);
            }
        }

        /// <summary>
        /// Parses the display text and converts it into a cell value to be stored in the style object.
        /// GridStyleInfo.CultureInfo is used for parsing the string.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="text">The input text to be parsed.</param>
        /// <param name="textInfo">TextInfo is a hint of who is calling, default is GridCellBaseTextInfo.DisplayText</param>
        /// <returns>True if value was parsed correctly and saved in style object as <see cref="GridStyleInfo.CellValue"/>; False otherwise.</returns>
        public virtual bool ApplyFormattedText(GridStyleInfo style, string text, int textInfo)
        {
            GridCellTextEventArgs ea = new GridCellTextEventArgs(text, style, null, textInfo);
            this.Grid.RaiseSaveCellFormattedText(ea);

            if (!ea.Handled)
            {
                this.Grid.RaiseParseCommonFormats(ea);
            }

            if (!ea.Handled)
            {
                CultureInfo ci = style.GetCulture(true);
                IFormatProvider nfi = style.CellValueType == typeof(DateTime)
                    ? (IFormatProvider)ci.DateTimeFormat
                    : (IFormatProvider)ci.NumberFormat;

                try
                {
                    style.BeginUpdate();
                    if ((style.CellType == "StandardValuesCell" || style.CellType == "ComboBox") && !string.IsNullOrEmpty(text) && this.GetTypeConverter(style).CanConvertFrom(typeof(string)))
                    {
                        style.CellValue = this.GetTypeConverter(style).ConvertFrom(text);
                    }
                    else
                    {
                        style.CellValue = style.ParseFormats == null
                                          ? GridCellValueConvert.Parse(text, style.CellValueType, nfi, style.Format)
                                          : GridCellValueConvert.Parse(text, style.CellValueType, nfi, style.ParseFormats, false);
                    }
                    style.ResetError();
                }
                catch (Exception ex)
                {
                    style.Error = ex.Message;
                    if (style.StrictValueType)
                    {
                        throw;
                    }
                    else if (ex is FormatException || ex.InnerException is FormatException)
                    {
                        style.CellValue = text;
                        // Possibly could also change CellValueType here based on input string.
                        // e.Style.CellValueType = typeof(string);
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    style.EndUpdate();
                }
            }

            return true;
        }

        /// <summary>
        /// This is called from GridStyleInfo.GetText (ignoring any <see cref="GridStyleInfo.Format"/> settings).
        /// CultureInfo.CurrentText is used for conversion to string.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value to convert to a string.</param>
        /// <returns>The string that represents the given value.</returns>
        public virtual string GetText(GridStyleInfo style, object value)
        {
            GridCellTextEventArgs ea = new GridCellTextEventArgs(string.Empty, style, value, -1);
            this.Grid.RaiseQueryCellText(ea);
            if (ea.Handled)
            {
                return ea.Text;
            }

            if (value == null)
            {
                return string.Empty;
            }

            return (string) GridCellValueConvert.ChangeType(value, typeof(string), CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Parses the text and converts it into a cell value to be stored in the style object (ignoring any <see cref="GridStyleInfo.Format"/> settings).
        /// CultureInfo.CurrentText is used for parsing the string.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="text">The input text to be parsed.</param>
        /// <returns>True if value was parsed correctly and saved in style object as <see cref="GridStyleInfo.CellValue"/>; False otherwise.</returns>
        public virtual bool ApplyText(GridStyleInfo style, string text)
        {
            GridCellTextEventArgs ea = new GridCellTextEventArgs(text, style, null, -1);
            this.Grid.RaiseSaveCellText(ea);
            if (!ea.Handled)
            {
                CultureInfo ci = style.GetCulture(true);
                IFormatProvider nfi = style.CellValueType == typeof(DateTime)
                    ? (IFormatProvider)ci.DateTimeFormat
                    : (IFormatProvider)ci.NumberFormat;

                try
                {
                    style.BeginUpdate();
                    style.CellValue = GridCellValueConvert.Parse(text, style.CellValueType, nfi, string.Empty);
                    style.ResetError();
                }
                catch (Exception ex)
                {
                    style.Error = ex.Message;
                    if (style.StrictValueType)
                    {
                        throw;
                    }
                    else if (ex is FormatException || ex.InnerException is FormatException)
                    {
                        style.CellValue = text;
                        // Possibly could also change CellValueType here based on input string.
                        // e.Style.CellValueType = typeof(string);
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    style.EndUpdate();
                }
            }

            return true;
        }

        /// <summary>
        /// Changes the active text for the cell model.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="text">The new text for the cell.</param>
        public void SetActiveText(int rowIndex, int colIndex, string text)
        {
#if DEBUG
            Trace.WriteLineIf(Switches.General.TraceVerbose, String.Format("SetActiveText({0}, {1}, {2})", rowIndex, colIndex, text));
#endif

            if (this.activeTextValue.SetValue(rowIndex, colIndex, text))
            {
                this.activeTextChanged.SetValue(rowIndex, colIndex, true);
                this.OnActiveTextChanged(new GridCellEventArgs(rowIndex, colIndex));
            }
        }

        /// <summary>
        /// Returns active text of current cell or null if rowIndex, colIndex is not current cell.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>The active text for the current cell.</returns>
        public string GetActiveText(int rowIndex, int colIndex)
        {
            GridCurrentCellInfo cc = this.Grid.CurrentCellInfo;
            if (cc == null || cc.RowIndex != rowIndex || cc.ColIndex != colIndex
                || this.Grid.ActiveGridView == null || !Grid.ActiveGridView.CurrentCell.HasCurrentCell
                || this.Grid.ActiveGridView.CurrentCell.StaticDrawing)
            {
                return null;
            }

            return (string) this.activeTextValue.GetValue(rowIndex, colIndex);
        }

        /// <summary>
        /// Occurs when active text has been changed.
        /// </summary>
        public event GridCellEventHandler ActiveTextChanged;

        /// <summary>
        /// Recalculates floating cell state and raises the ActiveTextChanged event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellEventArgs"/> with event data.</param>
        protected virtual void OnActiveTextChanged(GridCellEventArgs e)
        {
            GridRangeInfo r = GridRangeInfo.Cells(e.RowIndex, e.ColIndex, e.RowIndex, e.ColIndex+1);
            this.Grid.FloatingCells.DelayFloatCells(r);
            this.Grid.FloatingCells.EvaluateFloatingCells(r);

////            Grid.MergeCells.DelayMergeCells(r);
////            Grid.MergeCells.EvaluateMergeCells(r);
            if (this.ActiveTextChanged != null && this.Grid.ActiveGridView != null)
            {
                this.Grid.ActiveGridView.CurrentCell.ResetError();
                this.ActiveTextChanged(this, e);
            }
        }

        /// <summary>
        /// Reset the active text to its original state.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        public void ResetActiveText(int rowIndex, int colIndex)
        {
            if ((bool) this.activeTextChanged.GetValue(rowIndex, colIndex))
            {
                this.activeTextValue.ResetValue();
                this.OnActiveTextChanged(new GridCellEventArgs(rowIndex, colIndex));
            }
        }

        /// <summary>
        /// Returns GridStyleInfo.PropertyDescriptor.
        /// </summary>
        /// <param name="style">The style object</param>
        /// <returns>A PropertyDescriptor</returns>
        public virtual PropertyDescriptor GetPropertyDescriptor(GridStyleInfo style)
        {
            return style.PropertyDescriptor;
        }

        /// <summary>
        /// Returns a TypeConverter with type information about the style.CellValue.
        /// </summary>
        /// <param name="style">The style object</param>
        /// <returns>A TypeConverter</returns>
        public virtual TypeConverter GetTypeConverter(GridStyleInfo style)
        {
            PropertyDescriptor pd = this.GetPropertyDescriptor(style);
            if (pd != null)
            {
                return pd.Converter;
            }

            Type type = style.CellValueType;
            if (type != null)
            {
                return TypeDescriptor.GetConverter(type);
            }
            else
            {
                return TypeDescriptor.GetConverter(typeof(string));
            }
        }
    }
}
