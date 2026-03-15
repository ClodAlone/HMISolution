//-------------------------------------------------------------------------------------------------
// <copyright file="GridModelOptions.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Provides properties that allow you to adjust behavior and appearance of the grid.
    /// </summary>
    [Serializable]
    public class GridModelOptions : IDisposable, ISerializable, IDeserializationCallback
    {
        internal GridModel gridModel = null;
        GridControllerOptions controllerOptions = GridControllerOptions.All;
        GridDataObjectConsumerOptions dataObjectConsumerOptions = GridDataObjectConsumerOptions.All;

        ////GridBaseStylesMap styleInfoMap = null;
        GridBorderStyle defaultGridBorderStyle = GridBorderStyle.Dotted;
        GridResizeCellsBehavior resizeColWidthOptions = GridResizeCellsBehavior.ResizeSingle | GridResizeCellsBehavior.OutlineHeaders | GridResizeCellsBehavior.OutlineBounds;
        GridResizeCellsBehavior resizeRowHeightOptions = GridResizeCellsBehavior.ResizeSingle | GridResizeCellsBehavior.OutlineHeaders | GridResizeCellsBehavior.OutlineBounds;
        bool allowDragMoveCols = true;
        bool allowDragMoveRows = true;
        GridSelectionFlags allowSelection = GridSelectionFlags.Any;
        internal int dragDropDropTargetFlags;
        GridFloatCellsMode floatCellsMode;
        GridMergeCellsMode mergeCellsMode;
        GridMergeCellsLayout mergeCellsLayout;
        MouseButtons selectCellsMouseButtonsMask = MouseButtons.Left | MouseButtons.Middle | MouseButtons.Right;
        MouseButtons dragSelectCellsMouseButtonsMask = MouseButtons.Left;
        GridRefreshCurrentCellBehavior refreshCurrentCellBehavior = GridRefreshCurrentCellBehavior.RefreshCell;
        GridWrapCellBehavior wrapCellBehavior = GridWrapCellBehavior.None;
        bool mulitExtendedArrowKeySelect = true;
        GridScrollCurrentCellReason allowScrollCurrentCellInView = GridScrollCurrentCellReason.Any;
        bool useRightToLeftCompatibleTextBox = false;
        bool highlightFrozenLine = true;

        bool office2007ScrollBars = false;
        Office2007ColorScheme office2007ScrollBarsColorScheme = Office2007ColorScheme.Blue;
        OfficeScrollBars gridOfficeScrollBars = OfficeScrollBars.None;
        Office2010ColorScheme office2010ScrollBarsColorScheme = Office2010ColorScheme.Blue;
        GridMetroColors MetroColors = null;
        IVisualStylesDrawing gridVisualStylesDrawing = null;
        GridVisualStyles gridVisualStyles = GridVisualStyles.SystemTheme;

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            gridModel = null;
            if (gridVisualStylesDrawing != null)
            {
                if (gridVisualStylesDrawing is IDisposable)
                {
                    ((IDisposable)gridVisualStylesDrawing).Dispose();
                }

                gridVisualStylesDrawing = null;
            }
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Occurs when <see cref="ControllerOptions"/> have changed.
        /// </summary>
        public event EventHandler ControllerOptionsChanged;

        /// <summary>
        /// Occurs when <see cref="DataObjectConsumerOptions"/> have changed.
        /// </summary>
        public event EventHandler DataObjectConsumerOptionsChanged;

        /// <summary>
        /// Initializes a new instance of <see cref="GridModelOptions"/>.
        /// </summary>
        public GridModelOptions()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridModelOptions"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridModelOptions(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            int version = 0;
            try
            {
                version = info.GetInt32("Version");
            }
            catch (SerializationException ex)
            {
                //// This exception is expected for older file versions where "Version" was not
                //// yet implemented.
                //// Should be: "Additional information: Member Version was not found."

                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }

            if (version == 0)
            {
                defaultGridBorderStyle = (GridBorderStyle)info.GetValue("DefaultGridBorderStyle", typeof(GridBorderStyle));
                excelLikeCurrentCell = info.GetBoolean("ExcelLikeCurrentCell");
                excelLikeSelectionFrame = info.GetBoolean("ExcelLikeSelectionFrame");
                enterKeyBehavior = (GridDirectionType)info.GetValue("EnterKeyBehavior", typeof(GridDirectionType));
                numberedRowHeaders = info.GetBoolean("NumberedRowHeaders");
                numberedColHeaders = info.GetBoolean("NumberedColHeaders");
                transparentBackground = info.GetBoolean("TransparentBackground");
                syncCurrentCell = info.GetBoolean("SyncCurrentCell");
                floatCellsMode = (GridFloatCellsMode)info.GetValue("FloatCellsMode", typeof(GridFloatCellsMode));
                alphaBlendSelectionColor = (Color)info.GetValue("AlphaBlendSelectionColor", typeof(Color));
            }
            else
            {
                ////        public GridFloatCellsMode FloatCellsMode
                floatCellsMode = (GridFloatCellsMode)info.GetValue("FloatCellsMode", typeof(GridFloatCellsMode));
                ////        public GridBorderStyle DefaultGridBorderStyle
                defaultGridBorderStyle = (GridBorderStyle)info.GetValue("DefaultGridBorderStyle", typeof(GridBorderStyle));
                ////        public int DragDropDropTargetFlags
                dragDropDropTargetFlags = info.GetInt32("DragDropDropTargetFlags");
                ////        public GridControllerOptions ControllerOptions
                controllerOptions = (GridControllerOptions)info.GetValue("ControllerOptions", typeof(GridControllerOptions));
                ////        public GridDataObjectConsumerOptions DataObjectConsumerOptions 
                dataObjectConsumerOptions = (GridDataObjectConsumerOptions)info.GetValue("DataObjectConsumerOptions", typeof(GridDataObjectConsumerOptions));
                ////        public GridResizeCellsBehavior ResizeRowsBehavior
                resizeRowHeightOptions = (GridResizeCellsBehavior)info.GetValue("ResizeRowsBehavior", typeof(GridResizeCellsBehavior));
                ////        public GridResizeCellsBehavior ResizeColsBehavior  
                resizeColWidthOptions = (GridResizeCellsBehavior)info.GetValue("ResizeColsBehavior", typeof(GridResizeCellsBehavior));
                ////        public bool AllowDragSelectedCols
                allowDragMoveCols = info.GetBoolean("AllowDragSelectedCols");
                ////        public bool AllowDragSelectedRows
                allowDragMoveRows = info.GetBoolean("AllowDragSelectedRows");
                ////        public MouseButtons DragSelectedCellsMouseButtonsMask 
                dragSelectCellsMouseButtonsMask = (MouseButtons)info.GetValue("DragSelectedCellsMouseButtonsMask", typeof(MouseButtons));
                ////        public GridSelectionFlags AllowSelection 
                allowSelection = (GridSelectionFlags)info.GetValue("AllowSelection", typeof(GridSelectionFlags));
                ////        public MouseButtons SelectCellsMouseButtonsMask 
                selectCellsMouseButtonsMask = (MouseButtons)info.GetValue("SelectCellsMouseButtonsMask", typeof(MouseButtons));
                ////        public Color AlphaBlendSelectionColor
                alphaBlendSelectionColor = (Color)info.GetValue("AlphaBlendSelectionColor", typeof(Color));
                ////        public SelectionMode ListBoxSelectionMode
                listboxMode = (SelectionMode)info.GetValue("ListBoxSelectionMode", typeof(SelectionMode));
                ////        public bool ExcelLikeCurrentCell
                excelLikeCurrentCell = info.GetBoolean("ExcelLikeCurrentCell");
                ////        public bool ExcelLikeSelectionFrame
                excelLikeSelectionFrame = info.GetBoolean("ExcelLikeSelectionFrame");
                ////        public GridCellActivateAction ActivateCurrentCellBehavior
                activateCurrentCellBehavior = (GridCellActivateAction)info.GetValue("ActivateCurrentCellBehavior", typeof(GridCellActivateAction));
                ////        public GridDirectionType EnterKeyBehavior
                enterKeyBehavior = (GridDirectionType)info.GetValue("EnterKeyBehavior", typeof(GridDirectionType));
                ////        public bool TransparentBackground
                transparentBackground = info.GetBoolean("TransparentBackground");
                ////        public bool NumberedRowHeaders
                numberedRowHeaders = info.GetBoolean("NumberedRowHeaders");
                ////        public bool NumberedColHeaders
                numberedColHeaders = info.GetBoolean("NumberedColHeaders");
                ////        public bool HorizontalThumbTrack
                horizontalThumbTrack = info.GetBoolean("HorizontalThumbTrack");
                ////        public bool VerticalThumbTrack
                verticalThumbTrack = info.GetBoolean("VerticalThumbTrack");
                ////        public bool HorizontalScrollTips
                horizontalScrollTips = info.GetBoolean("HorizontalScrollTips");
                ////        public bool VerticalScrollTips
                verticalScrollTips = info.GetBoolean("VerticalScrollTips");
                ////        public int MinResizeRowSize
                minResizeRowSize = info.GetInt32("MinResizeRowSize");
                ////        public int MinResizeColSize
                minResizeColSize = info.GetInt32("MinResizeColSize");
                ////        public bool SmoothControlResize
                smoothControlResize = info.GetBoolean("SmoothControlResize");
                ////        public bool ScrollFrozen
                scrollFrozenLikeExcel = info.GetBoolean("ScrollFrozen");
                ////        public GridDrawOrder DrawOrder
                drawOrder = (GridDrawOrder)info.GetValue("DrawOrder", typeof(GridDrawOrder));
                ////        public GridShowCurrentCellBorder ShowCurrentCellBorderBehavior
                hideCurrentCell = (GridShowCurrentCellBorder)info.GetValue("ShowCurrentCellBorderBehavior", typeof(GridShowCurrentCellBorder));
                ////        public bool ShouldSynchronizeCurrentCell
                syncCurrentCell = info.GetBoolean("SyncCurrentCell");

                if (version >= 2)
                {
                    //// public bool DisplayEmptyRows
                    displayEmptyRows = info.GetBoolean("DisplayEmptyRows");
                    //// public bool DisplayEmptyColumns
                    displayEmptyColumns = info.GetBoolean("DisplayEmptyColumns");
                    if (version < 4)
                    {
                        //// public bool WrapCell
                        WrapCell = info.GetBoolean("WrapCell");
                    }
                }

                if (version >= 3)
                {
                    ////        public GridRefreshCurrentCellBehavior RefreshCurrentCellBehavior
                    refreshCurrentCellBehavior = (GridRefreshCurrentCellBehavior)info.GetValue("RefreshCurrentCellBehavior", typeof(GridRefreshCurrentCellBehavior));
                }

                if (version >= 4)
                {
                    ////        public GridWrapCellBehavior wrapCellBehavior
                    wrapCellBehavior = (GridWrapCellBehavior)info.GetValue("WrapCellBehavior", typeof(GridWrapCellBehavior));
                }

                if (version >= 5)
                {
                    ////        public GridMergeCellsMode MergeCellsMode
                    mergeCellsMode = (GridMergeCellsMode)info.GetValue("MergeCellsMode", typeof(GridMergeCellsMode));
                }

                if (version >= 6)
                {
                    ////        public int AlphaBlendValue 
                    alphaBlendValue = info.GetInt32("AlphaBlendValue");
                }

                if (version >= 7)
                {
                    ////        public int AlphaBlendValue 
                    mulitExtendedArrowKeySelect = info.GetBoolean("MulitExtendedArrowKeySelect");
                }

                if (version >= 8)
                {
                    ////        public GridScrollCurrentCellReason AllowScrollCurrentCellInView 
                    allowScrollCurrentCellInView = (GridScrollCurrentCellReason)info.GetValue("AllowScrollCurrentCellInView", typeof(GridScrollCurrentCellReason));
                }

                if (version >= 9)
                {
                    useRightToLeftCompatibleTextBox = info.GetBoolean("UseRightToLeftCompatibleTextBox");
                }

                if (version >= 10)
                {
                    clickedOnDisabledCellBehavior = (GridClickedOnDisabledCellBehavior)info.GetValue("ClickedOnDisabledCellBehavior", typeof(GridClickedOnDisabledCellBehavior));
                }

                if (version >= 11)
                {
                    highlightFrozenLine = info.GetBoolean("HighlightFrozenLine");
                }

                if (version >= 12)
                {
                    gridVisualStyles = (GridVisualStyles)info.GetValue("GridVisualStyles", typeof(GridVisualStyles));
                }

                if (version >= 13)
                {
                    office2007ScrollBars = (bool)info.GetValue("Office2007ScrollBars", typeof(bool));
                    office2007ScrollBarsColorScheme = (Office2007ColorScheme)info.GetValue("office2007ScrollBarsColorScheme", typeof(Office2007ColorScheme));
                }
            }
        }

        void IDeserializationCallback.OnDeserialization(object sender)
        {
        }

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the <see cref="GridModelOptions"/>.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            info.AddValue("Version", 13);

            ////        public GridMergeCellsMode MergeCellsMode
            info.AddValue("MergeCellsMode", mergeCellsMode); //// GridMergeCellsMode
            ////        public GridFloatCellsMode FloatCellsMode
            info.AddValue("FloatCellsMode", floatCellsMode); //// GridFloatCellsMode
            ////        public GridBorderStyle DefaultGridBorderStyle
            info.AddValue("DefaultGridBorderStyle", defaultGridBorderStyle); //// GridBorderStyle
            ////        public int DragDropDropTargetFlags
            info.AddValue("DragDropDropTargetFlags", dragDropDropTargetFlags); //// Int32
            ////        public GridControllerOptions ControllerOptions
            info.AddValue("ControllerOptions", controllerOptions); //// GridControllerOptions
            ////        public GridDataObjectConsumerOptions DataObjectConsumerOptions 
            info.AddValue("DataObjectConsumerOptions", dataObjectConsumerOptions); //// GridDataObjectConsumerOptions
            ////        public GridResizeCellsBehavior ResizeRowsBehavior
            info.AddValue("ResizeRowsBehavior", resizeRowHeightOptions);
            ////        public GridResizeCellsBehavior ResizeColsBehavior  
            info.AddValue("ResizeColsBehavior", resizeColWidthOptions); //// GridResizeCellsBehavior
            ////        public bool AllowDragSelectedCols
            info.AddValue("AllowDragSelectedCols", allowDragMoveCols); //// Boolean
            ////        public bool AllowDragSelectedRows
            info.AddValue("AllowDragSelectedRows", allowDragMoveRows); //// Boolean
            ////        public MouseButtons DragSelectedCellsMouseButtonsMask 
            info.AddValue("DragSelectedCellsMouseButtonsMask", this.dragSelectCellsMouseButtonsMask); //// MouseButtons 
            ////        public GridSelectionFlags AllowSelection 
            info.AddValue("AllowSelection", allowSelection); //// GridSelectionFlags 
            ////        public MouseButtons SelectCellsMouseButtonsMask 
            info.AddValue("SelectCellsMouseButtonsMask", selectCellsMouseButtonsMask); //// MouseButtons 
            ////        public Color AlphaBlendSelectionColor
            info.AddValue("AlphaBlendSelectionColor", alphaBlendSelectionColor); //// Color
            ////        public SelectionMode ListBoxSelectionMode
            info.AddValue("ListBoxSelectionMode", listboxMode); //// SelectionMode
            ////        public bool ExcelLikeCurrentCell
            info.AddValue("ExcelLikeCurrentCell", excelLikeCurrentCell); //// Boolean
            ////        public bool ExcelLikeSelectionFrame
            info.AddValue("ExcelLikeSelectionFrame", excelLikeSelectionFrame); //// Boolean
            ////        public GridCellActivateAction ActivateCurrentCellBehavior
            info.AddValue("ActivateCurrentCellBehavior", activateCurrentCellBehavior); //// GridCellActivateAction
            ////        public GridDirectionType EnterKeyBehavior
            info.AddValue("EnterKeyBehavior", enterKeyBehavior); //// GridDirectionType
            ////        public bool TransparentBackground
            info.AddValue("TransparentBackground", transparentBackground); //// Boolean
            ////        public bool NumberedRowHeaders
            info.AddValue("NumberedRowHeaders", numberedRowHeaders); //// Boolean
            ////        public bool NumberedColHeaders
            info.AddValue("NumberedColHeaders", numberedColHeaders); //// Boolean
            ////        public bool HorizontalThumbTrack
            info.AddValue("HorizontalThumbTrack", this.horizontalThumbTrack); //// Boolean
            ////        public bool VerticalThumbTrack
            info.AddValue("VerticalThumbTrack", this.verticalThumbTrack); //// Boolean
            ////        public bool HorizontalScrollTips
            info.AddValue("HorizontalScrollTips", this.horizontalScrollTips); //// Boolean
            ////        public bool VerticalScrollTips
            info.AddValue("VerticalScrollTips", this.verticalScrollTips); //// Boolean
            ////        public int MinResizeRowSize
            info.AddValue("MinResizeRowSize", this.minResizeRowSize); //// Int32
            ////        public int MinResizeColSize
            info.AddValue("MinResizeColSize", this.minResizeColSize); //// Int32
            ////        public bool SmoothControlResize
            info.AddValue("SmoothControlResize", this.smoothControlResize); //// Boolean
            ////        public bool ScrollFrozen
            info.AddValue("ScrollFrozen", this.scrollFrozenLikeExcel); //// Boolean
            ////        public GridDrawOrder DrawOrder
            info.AddValue("DrawOrder", drawOrder); //// GridDrawOrder
            ////        public GridShowCurrentCellBorder ShowCurrentCellBorderBehavior
            info.AddValue("ShowCurrentCellBorderBehavior", hideCurrentCell); //// GridShowCurrentCellBorder
            ////        public bool ShouldSynchronizeCurrentCell
            info.AddValue("SyncCurrentCell", syncCurrentCell); //// Boolean
            //// public bool DisplayEmptyRows
            info.AddValue("DisplayEmptyRows", displayEmptyRows); //// Boolean
            //// public bool DisplayEmptyColumns
            info.AddValue("DisplayEmptyColumns", displayEmptyColumns); //// Boolean
            //// public bool WrapCell
            info.AddValue("WrapCellBehavior", wrapCellBehavior); //// GridWrapCellBehavior
            ////        public GridRefreshCurrentCellBehavior RefreshCurrentCellBehavior
            info.AddValue("RefreshCurrentCellBehavior", refreshCurrentCellBehavior); //// Int32
            ////    public int AlphaBlendValue 
            info.AddValue("AlphaBlendValue", alphaBlendValue); //// Int32
            //// public bool MulitExtendedArrowKeySelect
            info.AddValue("MulitExtendedArrowKeySelect", mulitExtendedArrowKeySelect); //// bool
            ////        public GridScrollCurrentCellReason AllowScrollCurrentCellInView 
            info.AddValue("AllowScrollCurrentCellInView", allowScrollCurrentCellInView); //// GridScrollCurrentCellReason 
            info.AddValue("UseRightToLeftCompatibleTextBox", useRightToLeftCompatibleTextBox);
            info.AddValue("ClickedOnDisabledCellBehavior", clickedOnDisabledCellBehavior);
            info.AddValue("HighlightFrozenLine", highlightFrozenLine);
         //   info.AddValue("ColorStyles", colorStyles);
            info.AddValue("GridVisualStyles", gridVisualStyles);

            info.AddValue("Office2007ScrollBars", office2007ScrollBars);
            info.AddValue("office2007ScrollBarsColorScheme", office2007ScrollBarsColorScheme);
        }

        int alphaBlendValue = -1;

        /// <summary>
        /// Gets or sets an alphablend that should be used whenever a style or color setting in the grid has
        /// no alpha value specified. Ideal to change transparency for the whole grid in one place instead
        /// of changing colors for cells individually. Set to -1 if no value is specified.
        /// </summary>
        [Browsable(true)]
        [Description("Specifies an alphablend that should be used whenever a style or color setting in the grid has no alpha value specified.")]
        [Category("Appearance")]
        [DefaultValue(-1)]
        public int AlphaBlendValue
        {
            get
            {
                return alphaBlendValue;
            }

            set
            {
                if (alphaBlendValue != value)
                {
                    alphaBlendValue = value;
                    OnOptionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="GridFloatCellsMode"/> enumeration that specifies floating cells' behavior in a <see cref="GridModel"/>.
        /// </summary>
        [Browsable(true),
        DefaultValue(GridFloatCellsMode.None)]
        [Description("Enables and specifies floating cells behavior for the grid.")]
        [Category("Behavior")]
        public GridFloatCellsMode FloatCellsMode
        {
            get
            {
                return floatCellsMode;
            }

            set
            {
                if (value != FloatCellsMode)
                {
                    floatCellsMode = value;
                    OnFloatCellsModeChanged(EventArgs.Empty);
                    OnOptionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="GridMergeCellsMode"/> enumeration that specifies merge cells behavior in a <see cref="GridModel"/>.
        /// </summary>
        [Browsable(true),
        DefaultValue(GridMergeCellsMode.None)]
        [Description("Enables and specifies merge cells behavior for the grid.")]
        [Category("Behavior")]
        public GridMergeCellsMode MergeCellsMode
        {
            get
            {
                return mergeCellsMode;
            }

            set
            {
                if (value != MergeCellsMode)
                {
                    mergeCellsMode = value;
                    OnMergeCellsModeChanged(EventArgs.Empty);
                    OnOptionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="GridMergeCellsMode"/> enumeration that specifies merge cells behavior in a <see cref="GridModel"/>.
        /// </summary>
        [Browsable(true),
        DefaultValue(GridMergeCellsLayout.VisibleRange)]
        [Description("Enables and specifies merge cells behavior for the grid.")]
        [Category("Behavior")]
        public GridMergeCellsLayout MergeCellsLayout
        {
            get
            {
                return mergeCellsLayout;
            }

            set
            {
                if (value != MergeCellsLayout)
                {
                    mergeCellsLayout = value;
                    OnMergeCellsModeChanged(EventArgs.Empty);
                    OnOptionsChanged(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Occurs when the <see cref="GridModelOptions.MergeCellsMode"/> is changed.
        /// </summary>
        [Description(" Occurs when the MergeCellsMode setting is changed."),
        Category("Behavior")]
        public event EventHandler MergeCellsModeChanged;

        /// <summary>
        /// Raises the <see cref="MergeCellsModeChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
        protected internal virtual void OnMergeCellsModeChanged(EventArgs e)
        {
#if DEBUG
            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(MergeCellsMode);
            }
#else
            ;
#endif

            if (MergeCellsModeChanged != null)
            {
                MergeCellsModeChanged(this, e);
            }
        }

        /// <summary>
        /// Occurs when the <see cref="GridModelOptions.FloatCellsMode"/> is changed.
        /// </summary>
        [Description(" Occurs when the FloatCellsMode setting is changed."),
        Category("Behavior")]
        public event EventHandler FloatCellsModeChanged;

        /// <summary>
        /// Raises the <see cref="FloatCellsModeChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
        protected internal virtual void OnFloatCellsModeChanged(EventArgs e)
        {
#if DEBUG
            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(FloatCellsMode);
            }
#else
            ;
#endif

            if (FloatCellsModeChanged != null)
            {
                FloatCellsModeChanged(this, e);
            }
        }

        /// <summary>
        /// Occurs when settings in <see cref="GridModelOptions"/> have changed.
        /// </summary>
        [Description("Occurs when settings in any options have changed."),
        Category("Behavior")]
        public event EventHandler OptionsChanged;

        /// <summary>
        /// Raises the <see cref="OptionsChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
        protected internal virtual void OnOptionsChanged(EventArgs e)
        {
#if DEBUG
            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            this.gridModel.gridLineBorder = null;
            this.gridModel.fixedLineBorder = null;
            if (OptionsChanged != null)
            {
                OptionsChanged(this, e);
            }
        }
        
        /// <summary>
        /// Gets or sets the <see cref="GridBorderStyle"/> value to be used as default for cell borders.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Specifies the border style to be used as default for cell borders.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Appearance")]
        public GridBorderStyle DefaultGridBorderStyle
        {
            get
            {
                return defaultGridBorderStyle;
            }

            set
            {
                defaultGridBorderStyle = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the frozen line should be highlighted with <see cref="GridProperties"/>.<see cref="GridProperties.FixedLinesColor"/> the <see cref="GridBorderStyle"/>.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Specifies the border style to be used as default for cell borders.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool HighlightFrozenLine
        {
            get
            {
                return highlightFrozenLine;
            }

            set
            {
                highlightFrozenLine = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets or disables various options for using the grid as an OLE drop target.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Enables or disables various options for using the grid as an OLE Drop target.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Behavior")]
        public int DragDropDropTargetFlags
        {
            get
            {
                return dragDropDropTargetFlags;
            }

            set
            {
                dragDropDropTargetFlags = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets which mouse controllers should be enabled for the grid.
        /// <para/>
        /// This enumeration has a <see cref="System.FlagsAttribute"/> attribute that allows a bitwise combination of its member values.
        /// </summary>
        /// <remarks>
        /// When you assign this enumeration value to <see cref="GridModelOptions.ControllerOptions"/>,
        /// the grid will create or disable specified mouse controllers for the grid. Each of these 
        /// mouse controllers implements the <see cref="Syncfusion.Windows.Forms.IMouseController"/> interface and
        /// gets registered with <see cref="ScrollControl.MouseControllerDispatcher"/>.
        /// </remarks>
        [Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)),
        DefaultValue(GridControllerOptions.All)]
        [Description("Specifies which mouse controllers should be enabled for the grid.")]
        [RefreshProperties(RefreshProperties.All)]
        [Category("Behavior-Mouse")]
        public GridControllerOptions ControllerOptions
        {
            get
            {
                return controllerOptions;
            }

            set
            {
                if (controllerOptions != value)
                {
                    controllerOptions = value;
                    OnControllerOptionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Toggles the enabled state of a specific controller.
        /// </summary>
        /// <param name="coption">A <see cref="GridControllerOptions"/> that identifies the controller.</param>
        /// <param name="value">The new enabled state.</param>
        public void SetControllerOption(GridControllerOptions coption, bool value)
        {
            if (value)
            {
                ControllerOptions = ControllerOptions | coption;
            }
            else
            {
                ControllerOptions = ControllerOptions & ~coption;
            }
        }

        /// <summary>
        /// Returns the enabled state of a specific controller.
        /// </summary>
        /// <param name="coption">A <see cref="GridControllerOptions"/> that identifies the controller.</param>
        /// <returns>A <see cref="System.Boolean"/> that indicates if the controller is enabled.</returns>
        public bool GetControllerOption(GridControllerOptions coption)
        {
            return (ControllerOptions & coption) != GridControllerOptions.None;
        }
        
        /// <summary>
        /// Raises the <see cref="ControllerOptionsChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
        protected internal virtual void OnControllerOptionsChanged(EventArgs e)
        {
#if DEBUG
            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.ControllerOptions);
            }
#else
            ;
#endif

            if (ControllerOptionsChanged != null)
            {
                ControllerOptionsChanged(this, e);
            }
        }

        /// <summary>
        /// Gets or sets which default data consumers should be enabled for the grid.
        /// </summary>
        /// <value>
        /// A <see cref="GridDataObjectConsumerOptions"/> that holds the options to be applied.
        /// </value>
        /// <seealso cref="GridDataObjectConsumerOptions"/> 
        [Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)),
        DefaultValue(GridDataObjectConsumerOptions.All)]
        [Category("Clipboard")]
        [Description("Controls clipboard interchange format. Can be plain text and / or fully formatted with styles.")]
        public GridDataObjectConsumerOptions DataObjectConsumerOptions
        {
            get
            {
                return dataObjectConsumerOptions;
            }

            set
            {
                if (dataObjectConsumerOptions != value)
                {
                    dataObjectConsumerOptions = value;
                    OnDataObjectConsumerOptionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Toggles the enabled state for specific data consumers.
        /// </summary>
        /// <param name="coption">A <see cref="GridDataObjectConsumerOptions"/> that identifies the dataobject consumer.</param>
        /// <param name="value">A <see cref="System.Boolean"/> that indicates if the controller is enabled.</param>
        public void SetDataObjectConsumerOption(GridDataObjectConsumerOptions coption, bool value)
        {
            if (value)
            {
                DataObjectConsumerOptions = DataObjectConsumerOptions | coption;
            }
            else
            {
                DataObjectConsumerOptions = DataObjectConsumerOptions & ~coption;
            }
        }

        /// <summary>
        /// Returns the enabled state for specific data consumers.
        /// </summary>
        /// <param name="doption">A <see cref="GridDataObjectConsumerOptions"/> that identifies the dataobject consumer.</param>
        /// <returns>A <see cref="System.Boolean"/> that indicates if the controller is enabled.</returns>
        public bool GetDataObjectConsumerOption(GridDataObjectConsumerOptions doption)
        {
            return (DataObjectConsumerOptions & doption) != GridDataObjectConsumerOptions.None;
        }

        /// <summary>
        /// Raises the <see cref="DataObjectConsumerOptionsChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
        protected internal virtual void OnDataObjectConsumerOptionsChanged(EventArgs e)
        {
#if DEBUG
            if (Switches.GridModelEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.DataObjectConsumerOptions);
            }
#else

            ;
#endif
            if (DataObjectConsumerOptionsChanged != null)
            {
                DataObjectConsumerOptionsChanged(this, e);
            }
        }

        /// <summary>
        /// Gets or sets behavior for resizing rows.
        /// </summary>
        /// <value>
        /// A <see cref="GridResizeCellsBehavior"/> enumeration with options.
        /// </value>
        /// <remarks>
        /// You can customize the mouse controller's behavior at run-time while the user is performing the action 
        /// by subscribing to the <see cref="GridControlBase.ResizingColumns"/>
        /// and <see cref="GridControlBase.ResizingRows"/> events.
        /// </remarks>
        [Browsable(true),
        Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)),
        DefaultValue(GridResizeCellsBehavior.ResizeSingle | GridResizeCellsBehavior.OutlineHeaders | GridResizeCellsBehavior.OutlineBounds)]
        [Description("Defines behavior for resizing rows.")]
        [Category("Behavior-Resizing")]
        public GridResizeCellsBehavior ResizeRowsBehavior
        {
            get
            {
                return resizeRowHeightOptions;
            }

            set
            {
                resizeRowHeightOptions = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets behavior for resizing columns.
        /// </summary>
        /// <value>
        /// A <see cref="GridResizeCellsBehavior"/> enumeration with options.
        /// </value>
        /// <remarks>
        /// You can customize the mouse controller's behavior at run-time while the user is performing the action 
        /// by subscribing to the <see cref="GridControlBase.ResizingColumns"/>
        /// and <see cref="GridControlBase.ResizingRows"/> events.
        /// </remarks>
        [Browsable(true),
        Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)),
        DefaultValue(GridResizeCellsBehavior.ResizeSingle | GridResizeCellsBehavior.OutlineHeaders | GridResizeCellsBehavior.OutlineBounds)]
        [Description("Defines behavior for resizing columns.")]
        [Category("Behavior-Resizing")]
        public GridResizeCellsBehavior ResizeColsBehavior
        {
            get
            {
                return resizeColWidthOptions;
            }

            set
            {
                resizeColWidthOptions = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow the user to drag selected columns by clicking on the column header.
        /// </summary>
        /// <remarks>
        /// You can customize the mouse controller's behavior at run-time while the user is performing the action 
        /// by subscribing to the <see cref="GridControlBase.SelectionDragging"/>
        /// and  <see cref="GridControlBase.SelectionDragged"/> events.
        /// </remarks>
        [Browsable(true),
        DefaultValue(false)]
        [Description("Allow the user to drag selected columns by clicking on the column header.")]
        [Category("Behavior-DragDrop")]
        public bool AllowDragSelectedCols
        {
            get
            {
                return allowDragMoveCols;
            }

            set
            {
                allowDragMoveCols = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow the user to drag selected rows by clicking on the row header.
        /// </summary>
        /// <remarks>
        /// You can customize the mouse controller's behavior at run-time while the user is performing the action 
        /// by subscribing to the <see cref="GridControlBase.SelectionDragging"/>
        /// and  <see cref="GridControlBase.SelectionDragged"/> events.
        /// </remarks>
        [Browsable(true),
        DefaultValue(false)]
        [Description("Controls allowing the user to drag selected rows by clicking on the row header.")]
        [Category("Behavior-DragDrop")]
        public bool AllowDragSelectedRows
        {
            get
            {
                return allowDragMoveRows;
            }

            set
            {
                allowDragMoveRows = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets which mouse buttons can be used for dragging selected rows or columns.
        /// </summary>
        /// <value>
        /// A <see cref="GridSelectionFlags"/> that specifies options to be applied.
        /// </value>
        /// <remarks>
        /// You can customize the mouse controller's behavior at run-time while the user is performing the action 
        /// by subscribing to the <see cref="GridModel.SelectionChanging"/> and 
        /// <see cref="GridModel.SelectionChanged"/> events.<para/>
        /// The <see cref="AllowSelection"/> property lets you further customize selection behavior.
        /// </remarks>
        [Browsable(true),
        DefaultValue(MouseButtons.Left)]
        [Description("Defines which mouse buttons can be used for dragging selected rows or columns.")]
        [Category("Behavior-DragDrop")]
        public MouseButtons DragSelectedCellsMouseButtonsMask
        {
            get
            {
                return dragSelectCellsMouseButtonsMask;
            }

            set
            {
                dragSelectCellsMouseButtonsMask = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets selection behavior of the grid.
        /// </summary>
        /// <value>
        /// A <see cref="GridSelectionFlags"/> that specifies options to be applied.
        /// </value>
        /// <remarks>
        /// You can customize the mouse controller's behavior at run-time while the user is performing the action 
        /// by subscribing to the <see cref="GridModel.SelectionChanging"/> and 
        /// <see cref="GridModel.SelectionChanged"/> events.<para/>
        /// The <see cref="SelectCellsMouseButtonsMask"/> property lets you decide which mouse buttons 
        /// can be used for selecting cells.
        /// </remarks>
        [Browsable(true),
        Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)),
        DefaultValue(GridSelectionFlags.Any)]
        [Description("Defines selection behavior of the grid.")]
        [Category("Behavior-Selection")]
        public GridSelectionFlags AllowSelection
        {
            get
            {
                return allowSelection;
            }

            set
            {
                allowSelection = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets ScrollCurrentCellInView behavior of the grid.
        /// </summary>
        /// <value>
        /// A <see cref="GridScrollCurrentCellReason"/> that specifies options to be applied.
        /// </value>
        /// <remarks>
        /// You can customize the current cell's scroll behavior at run-time 
        /// by subscribing to the <see cref="GridControlBase.QueryScrollCellInView"/> event.<para/>
        /// The <see cref="AllowScrollCurrentCellInView"/> property lets you specify the mask for
        /// which reason the scrolling should happen.
        /// </remarks>
        [Browsable(true),
        Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)),
        DefaultValue(GridScrollCurrentCellReason.Any)]
        [Description("Defines ScrollCurrentCellInView behavior of the grid.")]
        [Category("Behavior-ScrollCurrentCellInView")]
        public GridScrollCurrentCellReason AllowScrollCurrentCellInView
        {
            get
            {
                return allowScrollCurrentCellInView;
            }

            set
            {
                allowScrollCurrentCellInView = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets which mouse buttons can be used for selecting cells.
        /// </summary>
        /// <value>
        /// A <see cref="GridSelectionFlags"/> that specifies options to be applied.
        /// </value>
        /// <remarks>
        /// You can customize the mouse controller's behavior at run-time while the user is performing the action 
        /// by subscribing to the <see cref="GridModel.SelectionChanging"/> and 
        /// <see cref="GridModel.SelectionChanged"/> events.<para/>
        /// The <see cref="AllowSelection"/> property lets you further customize selection behavior.
        /// </remarks>
        [Browsable(true),
        Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)),
        DefaultValue(MouseButtons.Left | MouseButtons.Middle | MouseButtons.Right)]
        [Description("Defines which mouse buttons can be used for selecting cells.")]
        [Category("Behavior-Selection")]
        public MouseButtons SelectCellsMouseButtonsMask
        {
            get
            {
                return selectCellsMouseButtonsMask;
            }

            set
            {
                selectCellsMouseButtonsMask = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Drawing.Color"/> for alpha blended cell selections.
        /// </summary>
        /// <value>
        /// A <see cref="System.Drawing.Color"/> for alpha blended cell selections. It is important to set the alpha value to be less
        /// than 255 when calling <see cref="System.Drawing.Color.FromArgb(int)"/>.
        /// </value>
        /// <remarks>
        /// This setting has no effect if alpha blended selections have been disabled with <see cref="GridModelOptions.AllowSelection"/>.
        /// </remarks>
        [Browsable(true)]
        [Description("Specifies the color for alpha blended cell selections.")]
        [Category("Appearance")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public Color AlphaBlendSelectionColor
        {
            get
            {
                if (alphaBlendSelectionColor.A == 255)
                {
                    if (this.GridVisualStyles == Forms.GridVisualStyles.Metro)
                        return Color.FromArgb(64, visualHoverColor);
                    else
                        return Color.FromArgb(64, alphaBlendSelectionColor);
                }

                return alphaBlendSelectionColor;
            }

            set
            {
                alphaBlendSelectionColor = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        Color alphaBlendSelectionColor = SystemColors.Highlight;
        SelectionMode listboxMode = SelectionMode.None;
        bool verticalThumbTrack = false;
        bool horizontalThumbTrack = false;
        bool verticalScrollTips = false;
        bool horizontalScrollTips = false;
        ////bool sortColsOnDoubleClick = false;
        ////bool sortRowsOnDoubleClick = false;
        bool excelLikeCurrentCell = true;
        GridClickedOnDisabledCellBehavior clickedOnDisabledCellBehavior = GridClickedOnDisabledCellBehavior.Default;
        bool excelLikeSelectionFrame = true;
        GridDirectionType enterKeyBehavior = GridDirectionType.Right;
        bool numberedRowHeaders = true;
        bool numberedColHeaders = true;
        bool transparentBackground = false;
        ////int removeRowsBehavior = GridRemoveUndoOption.RangeStyles|GridRemoveUndoOption.CellStyles;
        ////int removeColsBehavior = GridRemoveUndoOption.RangeStyles|GridRemoveUndoOption.CellStyles;
        int/*float*/ minResizeRowSize = 0;
        int/*float*/ minResizeColSize = 0;
        internal GridCellActivateAction activateCurrentCellBehavior = GridCellActivateAction.ClickOnCell;
        internal bool activateSendKey=true;
        bool smoothControlResize = true;
        bool scrollFrozenLikeExcel = true;
        bool syncCurrentCell = false;
        GridDrawOrder drawOrder = GridDrawOrder.Rows;
        GridShowCurrentCellBorder hideCurrentCell = GridShowCurrentCellBorder.WhenGridActive;

        /// <summary>
        /// Gets or sets list box-like selection behavior for the grid when the user moves the current cell.
        /// </summary>
        /// <value>
        /// A <see cref="SelectionMode"/> that defines the list box-like selection behavior of the grid.
        /// </value>
        [Category(@"Behavior"),
        DefaultValue(SelectionMode.None),
        Description(@"Grid can emulated list boxes. In such mode, indicates if the list box is to be single-select, multi-select, or unselectable.")]
        public SelectionMode ListBoxSelectionMode
        {
            get
            {
                return listboxMode;
            }

            set
            {
                if (listboxMode != value)
                {
                    listboxMode = value;
                    if (gridModel != null)
                    {
                        gridModel.SelectedRanges.Clear();
                        if (listboxMode != SelectionMode.None
                            && gridModel.ActiveGridView != null
                            && gridModel.ActiveGridView.CurrentCell.HasCurrentCell)
                        {
                            gridModel.Selections.Add(GridRangeInfo.Row(gridModel.ActiveGridView.CurrentCell.RowIndex));
                        }
                    }

                    OnOptionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether when you select SelectionMode.MultiExtended, this flag defines if the rows selection
        /// should be cleared and moved with the new current cell or if only the current cell
        /// should be moved without clearing selections.
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool MulitExtendedArrowKeySelect
        {
            get
            {
                return mulitExtendedArrowKeySelect;
            }

            set
            {
                mulitExtendedArrowKeySelect = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Excel-like current cell behavior. When the user moves the current cell out of a selected
        /// range, the range will be cleared. If the user moves the current cell inside a selected range, the
        /// range will stay.
        /// </summary>
        [Browsable(true),
        DefaultValue(false)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("Defines Excel-like current cell behavior. When the user moves the current cell out of a selected range, the range will be cleared.")]
        [Category("Excel-Emulation")]
        public bool ExcelLikeCurrentCell
        {
            get
            {
                return excelLikeCurrentCell;
            }

            set
            {
                excelLikeCurrentCell = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets Excel-like current cell behavior. When the user clicks on a cell out of a selected range for which .Enabled has been set to false.
        /// </summary>
        [Browsable(true),
        DefaultValue(GridClickedOnDisabledCellBehavior.Default)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("Defines Excel-like current cell behavior. When the user clicks on a cell out of a selected range for which .Enabled has been set to false.")]
        [Category("Excel-Emulation")]
        public GridClickedOnDisabledCellBehavior ClickedOnDisabledCellBehavior
        {
            get
            {
                return clickedOnDisabledCellBehavior;
            }

            set
            {
                clickedOnDisabledCellBehavior = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the active selection should be outline with a selection frame.
        /// </summary>
        [Browsable(true),
        DefaultValue(false)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("Specifies whether the active selection should be outlined with a selection frame.")]
        [Category("Excel-Emulation")]
        public bool ExcelLikeSelectionFrame
        {
            get
            {
                return excelLikeSelectionFrame;
            }

            set
            {
                excelLikeSelectionFrame = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets current cell activation behavior when moving the current cell or clicking inside a cell.
        /// </summary>
        /// <value>
        /// A <see cref="GridCellActivateAction"/> enumeration that defines when to set the focus / toggle edit mode for the current cell.
        /// </value>
        [Browsable(true),
        DefaultValue(GridCellActivateAction.ClickOnCell)]
        [Description("Specifies current cell activation behavior when moving the current cell or clicking inside a cell.")]
        [Category("Behavior")]
        public GridCellActivateAction ActivateCurrentCellBehavior
        {
            get
            {
                return activateCurrentCellBehavior;
            }

            set
            {
                activateCurrentCellBehavior = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }
		/// <summary>
        /// Gets or sets Activate the SendKey when pressing the negative sign key.
        /// </summary>
        [Browsable(true),
        DefaultValue(true)]
        [Description("Activate/Deactivate the SendKey method by using ActivateSendKey.")]
        [Category("Behavior")]
        public bool ActivateSendKey
        {
            get
            {
                return activateSendKey;
            }
            set
            {
                activateSendKey = value;               
            }
        }

        /// <summary>
        /// Gets or sets movement of current cell when pressing the Enter key.
        /// </summary>
        [Browsable(true),
        DefaultValue(GridDirectionType.Right)]
        [Description("Controls what the grid does with the 'Enter' key.")]
        [Category("Behavior-Keyboard")]
        public GridDirectionType EnterKeyBehavior
        {
            get
            {
                return enterKeyBehavior;
            }

            set
            {
                enterKeyBehavior = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether grid should erase and fill background of cells or only draw cell text.
        /// </summary>
        /// <value>
        /// True if only text should be drawn; False if cell background should be erased and filled.</value>
        [Browsable(false),
        DefaultValue(false)]
        [Description("Defines whether grid should erase and fill background of cells or only draw cell text.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public bool TransparentBackground
        {
            get
            {
                return transparentBackground;
            }

            set
            {
                transparentBackground = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to toggle display of row numbers in row headers.
        /// </summary>
        [Browsable(true),
        DefaultValue(true)]
        [Description("Toggle display of row numbers in row headers.")]
        [Category("Appearance")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public bool NumberedRowHeaders
        {
            get
            {
                return numberedRowHeaders;
            }

            set
            {
                numberedRowHeaders = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to toggle display of column ids (A, B, C, ...) in column headers.
        /// </summary>
        [Browsable(true),
        DefaultValue(true)]
        [Description("Toggle display of column ids (A, B, C, ...) in column headers.")]
        [Category("Appearance")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public bool NumberedColHeaders
        {
            get
            {
                return numberedColHeaders;
            }

            set
            {
                numberedColHeaders = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <internalonly/>
        [Obsolete("Use HorizontalThumbTrack and VerticalThumbTrack properties instead.")]
        [Syncfusion.Documentation.DocumentationExclude()]
        [Browsable(false)]
        public bool AllowThumbTrack
        {
            get
            {
                return verticalThumbTrack;
            }

            set
            {
                verticalThumbTrack = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the grid should scroll immediately when the user grabs a horizontal scrollbar thumb
        /// and drags it.
        /// </summary>
        [Browsable(true),
        Category("Scrolling")]
        [Description("Defines whether the grid should scroll immediately when the user grabs a horizontal scrollbar thumb and drags it.")]
        public bool HorizontalThumbTrack
        {
            get
            {
                return horizontalThumbTrack;
            }

            set
            {
                horizontalThumbTrack = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the grid should scroll immediately when the user grabs a vertical scrollbar thumb
        /// and drags it.
        /// </summary>
        [Browsable(true),
        Category("Scrolling")]
        [Description("Specifies if the control should scroll while the user is dragging a vertical scrollbars thumb.")]
        public bool VerticalThumbTrack
        {
            get
            {
                return verticalThumbTrack;
            }

            set
            {
                verticalThumbTrack = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the grid should display Scroll Tips when the user grabs a horizontal scrollbar thumb
        /// and drags it.
        /// </summary>
        [Browsable(true),
        Category("Scrolling")]
        [Description("Defines whether the grid should display Scroll Tips when the user grabs a horizontal scrollbar thumb and drags it.")]
        public bool HorizontalScrollTips
        {
            get
            {
                return horizontalScrollTips;
            }

            set
            {
                horizontalScrollTips = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the grid should display Scroll Tips when the user grabs a vertical scrollbar thumb
        /// and drags it.
        /// </summary>
        [Browsable(true),
        Category("Scrolling")]
        [Description("Defines whether the grid should display Scroll Tips when the user grabs a vertical scrollbar thumb and drags it.")]
        public bool VerticalScrollTips
        {
            get
            {
                return verticalScrollTips;
            }

            set
            {
                verticalScrollTips = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        ////            public bool SortColsOnDoubleClick
        ////            {
        ////                get
        ////                {
        ////                    return sortColsOnDoubleClick;
        ////                }
        ////                set
        ////                {
        ////                    sortColsOnDoubleClick = value;
        ////                    OnOptionsChanged(EventArgs.Empty);
        ////                }
        ////            }
        ////            public bool SortRowsOnDoubleClick
        ////            {
        ////                get
        ////                {
        ////                    return sortRowsOnDoubleClick;
        ////                }
        ////                set
        ////                {
        ////                    sortRowsOnDoubleClick = value;
        ////                    OnOptionsChanged(EventArgs.Empty);
        ////                }
        ////            }

        /// <summary>
        /// Gets or sets the minimum row height when the user resizes a row with the mouse.
        /// </summary>
        [Browsable(true),
        DefaultValue(0)]
        [Description("Defines the minimum row height when the user resizes a row with the mouse.")]
        [Category("Behavior-Resizing")]
        public int/*float*/ MinResizeRowSize
        {
            get
            {
                return minResizeRowSize;
            }

            set
            {
                minResizeRowSize = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the minimum column width when the user resizes a column with the mouse.
        /// </summary>
        [Browsable(true),
        DefaultValue(0)]
        [Description("Defines the minimum column width when the user resizes a column with the mouse.")]
        [Category("Behavior-Resizing")]
        public int/*float*/ MinResizeColSize
        {
            get
            {
                return minResizeColSize;
            }

            set
            {
                minResizeColSize = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a grid should be completely refreshed when the user resizes the window
        /// or if only newly visible rows or columns should be redrawn.
        /// </summary>
        [Browsable(true),
        DefaultValue(true)]
        [Description("Defines whether a grid should be completely refreshed when the user resizes the window or if only newly visible rows or columns should be redrawn.")]
        [Category("Behavior-Resizing")]
        public bool SmoothControlResize
        {
            get
            {
                return smoothControlResize;
            }

            set
            {
                smoothControlResize = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the scroll behavior when user moves current cell with arrow keys into the frozen cells area.
        /// </summary>
        /// <remarks>
        /// True, if current cell is at the topmost nonfrozen row, scroll the view.
        /// False, move current cell into frozen cells. If current cell is at the top row, 
        /// scroll the view.
        /// </remarks>
        [Browsable(true),
        DefaultValue(true)]
        [Description("Defines scroll behavior when user moves current cell with arrow keys into frozen cells area.")]
        [Category("Scrolling")]
        public bool ScrollFrozen
        {
            get
            {
                return scrollFrozenLikeExcel;
            }

            set
            {
                scrollFrozenLikeExcel = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the order how cells are loaded before the grid is displayed. This is of use when
        /// using the virtual grid and it is more extensive to move from column to column than to 
        /// move from row to row in your custom data source.
        /// </summary>
        [Browsable(true),
        DefaultValue(GridDrawOrder.Rows)]
        [Description("Defines the order how cells are loaded before the grid is displayed.")]
        [Category("Behavior-Render")]
        public GridDrawOrder DrawOrder
        {
            get
            {
                return drawOrder;
            }

            set
            {
                drawOrder = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets when to show current cell frame or border.
        /// </summary>
        [Browsable(true),
        DefaultValue(GridShowCurrentCellBorder.WhenGridActive)]
        [Description("Defines when to show current cell frame or border.")]
        [Category("Appearance")]
        public GridShowCurrentCellBorder ShowCurrentCellBorderBehavior
        {
            get
            {
                return hideCurrentCell;
            }

            set
            {
                hideCurrentCell = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets which cells to refresh when moving the current cell. If a cell's appearance is changed if
        /// cells are moved to a new row (e.g. when GridShowButtons.ShowCurrentRow is used), you should specify
        /// <see cref="GridRefreshCurrentCellBehavior.RefreshRow"/>.
        /// </summary>
        [Browsable(true),
        DefaultValue(GridRefreshCurrentCellBehavior.RefreshCell)]
        [Description("Which cells to refresh when moving the current cell.")]
        [Category("Appearance")]
        public GridRefreshCurrentCellBehavior RefreshCurrentCellBehavior
        {
            get
            {
                return refreshCurrentCellBehavior;
            }

            set
            {
                refreshCurrentCellBehavior = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether current cell movements should be synchronized among <see cref="GridControlBase"/> attached to the same model.
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShouldSynchronizeCurrentCell
        {
            get
            {
                return excelLikeCurrentCell || syncCurrentCell;
            }

            set
            {
                syncCurrentCell = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        bool displayEmptyRows = false;
        bool displayEmptyColumns = false;

        /// <internalonly/>
        /// <summary>Gets or sets a value indicating whether to Display Empty Rows. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        [Browsable(true),
        DefaultValue(false)]
        [Description("Show empty rows after the last row.")]
        [Category("Appearance")]
        public bool DisplayEmptyRows
        {
            get
            {
                return displayEmptyRows;
            }

            set
            {
                displayEmptyRows = value;
            }
        }

        /// <internalonly/>
        /// <summary>Gets or sets a value indicating whether to Display Empty Columns. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        [Browsable(true),
        DefaultValue(false)]
        [Description("Show empty columns after the last column.")]
        [Category("Appearance")]
        public bool DisplayEmptyColumns
        {
            get
            {
                return displayEmptyColumns;
            }

            set
            {
                displayEmptyColumns = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to go to first column in next row or last column in previous row when
        /// at end or beginning of a row and moving left or right. For more options, 
        /// use <see cref="WrapCellBehavior"/> instead.
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        DefaultValue(false)]
        [Description("Controls what the grid does when at last column in a row.")]
        [Category("Behavior-Keyboard")]
        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        public bool WrapCell
        {
            get
            {
                return wrapCellBehavior != GridWrapCellBehavior.None;
            }

            set
            {
                if (value)
                {
                    wrapCellBehavior = GridWrapCellBehavior.WrapRow;
                }
                else
                {
                    wrapCellBehavior = GridWrapCellBehavior.None;
                }
            }
        }

        /// <summary>
        /// Gets or sets go to first column in next row or last column in previous row when
        /// at end or beginning of a row and moving left or right.
        /// </summary>
        [Browsable(true),
        DefaultValue(GridWrapCellBehavior.None)]
        [Description("Controls what the grid does when at last column in a row.")]
        [Category("Behavior-Keyboard")]
        public GridWrapCellBehavior WrapCellBehavior
        {
            get
            {
                return wrapCellBehavior;
            }

            set
            {
                wrapCellBehavior = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether the controls the kind of textbox control that is created for TextBox cells. 
        /// In general the original text box behaves better than the richtext box with Hebrew and arabic languages.
        /// By default the grid uses the RichTextBox control for cell editing, but if you set
        /// UseRightToLeftCompatibleTextBox to true then the grid will do editing with original TextBox controls
        /// instead.
        /// </summary>
        [Browsable(true),
        DefaultValue(false)]
        [Description("Controls the kind of textbox control that is created for TextBox cells. In general the original text box behaves better than the default richtext box with Hebrew and arabic languages")]
        [Category("Behavior-Keyboard")]
        public bool UseRightToLeftCompatibleTextBox
        {
            get
            {
                return useRightToLeftCompatibleTextBox;
            }

            set
            {
                useRightToLeftCompatibleTextBox = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }
        

        /// <summary>
        /// Gets or sets the VisualStyles (skins) like Office2010, Office2007, Office2003
        /// </summary>
        /// <remarks> Each of the components that is incorporated into the grid control is being affected with Visual Styles.
        /// Choosing one of the options will change the look and feel of the individual grid elements.</remarks>
        /// <example>The VisualStyles can be set by assigning a <see cref="GridVisualStyles"/> enumeration value to the GridVisualStyles property
        /// <code lang="C#">
        ///     this.gridControl1.GridVisualStyles = GridVisualStyles.Office2007Blue;
        /// </code>
        /// <code lang="VB">
        ///     Me.GridControl1.GridVisualStyles = GridVisualStyles.Office2007Blue
        /// </code>
        /// </example>
        [Browsable(true),
        DefaultValue(GridVisualStyles.SystemTheme)]
        [Description("Specifies look and feel skins for the Grid")]
        [Category("Appearance")]
        public GridVisualStyles GridVisualStyles
        {
            get
            {
                return gridVisualStyles;
            }

            set
            {
                if (this.gridVisualStyles != value)
                {
                    this.gridVisualStyles = value;
                    if (gridVisualStyles == GridVisualStyles.Metro)
                    {
                        this.GridOfficeScrollBars = OfficeScrollBars.Metro;
                    }
                    OnOptionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to toggle between standard and Office2007 scrollbars.
        /// </summary>
        [Category("Appearance"),
        Description("Toggle between standard and Office2007 scrollbars."),
        DefaultValue(false)]
        public bool Office2007ScrollBars
        {
            get
            {
                return this.office2007ScrollBars;
            }

            set
            {
                if (this.office2007ScrollBars != value)
                {
                    this.office2007ScrollBars = value;
                    OnOptionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the style of Office2007 scroll bars
        /// </summary>
        [Category("Appearance"),
        Browsable(true),
        Description("Office 2007 style scrollbars."),
        DefaultValue(Office2007ColorScheme.Blue)]
        public Office2007ColorScheme Office2007ScrollBarsColorScheme
        {
            get
            {
                return this.office2007ScrollBarsColorScheme;
            }

            set
            {
                if (this.office2007ScrollBarsColorScheme != value)
                {
                    this.office2007ScrollBarsColorScheme = value;
                    OnOptionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to toggle between standard and Office2007 scrollbars.
        /// </summary>
        [Category("Appearance"),
        Description("Gets or Set MS-Office like scrollbars."),
        DefaultValue(OfficeScrollBars.None)
        ]
        public OfficeScrollBars GridOfficeScrollBars
        {
            get
            {
                return this.gridOfficeScrollBars;
            }

            set
            {
                if (this.gridOfficeScrollBars != value)
                {
                    this.gridOfficeScrollBars = value;
                    OnOptionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the style of Office2010 scroll bars
        /// </summary>
        [Category("Appearance"),
        Browsable(true),
        Description("MS-Office 2010 style scrollbars."),
        DefaultValue(Office2010ColorScheme.Blue)]
        public Office2010ColorScheme Office2010ScrollBarsColorScheme
        {
            get
            {
                return this.office2010ScrollBarsColorScheme;
            }

            set
            {
                if (this.office2010ScrollBarsColorScheme != value)
                {
                    this.office2010ScrollBarsColorScheme = value;
                    OnOptionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the VisualStylesDrawing object
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets or sets the VisualStylesDrawing object")]
        [Category("Appearance")]
        public IVisualStylesDrawing GridVisualStylesDrawing
        {
            get
            {
                if (this.gridVisualStylesDrawing == null || this.gridVisualStylesDrawing.VisualStyle != GridVisualStyles)
                {
                    switch (GridVisualStyles)
                    {
                        case GridVisualStyles.SystemTheme:
                            this.gridVisualStylesDrawing = new GridVisualStylesSystemTheme(GridVisualStyles);
                            break;
                        case GridVisualStyles.Office2003:
                            this.gridVisualStylesDrawing = new GridVisualStylesOffice2003(GridVisualStyles);
                            break;
                        case GridVisualStyles.Office2007Blue:
                            this.gridVisualStylesDrawing = new GridVisualStylesOffice2007Blue(GridVisualStyles, gridModel.EnableLegacyStyle);
                            break;
                        case GridVisualStyles.Office2007Black:
                            this.gridVisualStylesDrawing = new GridVisualStylesOffice2007Black(GridVisualStyles, gridModel.EnableLegacyStyle);
                            break;
                        case GridVisualStyles.Office2007Silver:
                            this.gridVisualStylesDrawing = new GridVisualStylesOffice2007Silver(GridVisualStyles, gridModel.EnableLegacyStyle);
                            break;
                        case GridVisualStyles.Office2010Blue:
                        case GridVisualStyles.Office2010Black:
                        case GridVisualStyles.Office2010Silver:
                            this.gridVisualStylesDrawing = new GridVisualStylesOffice2010(GridVisualStyles,gridModel.EnableLegacyStyle);
                            break;
                        case GridVisualStyles.Metro:
                            this.GridVisualStylesDrawing = new GridMetroStyle(GridVisualStyles, visualMetroColors);
                            break;
                        //// If no custom skin set (Grid.GridVisualStylesDrawing), use the default System theme
                        case GridVisualStyles.Custom:
                            this.gridVisualStyles = GridVisualStyles.SystemTheme;
                            this.gridVisualStylesDrawing = new GridVisualStylesSystemTheme(GridVisualStyles.SystemTheme);
                            break;
                    }
                }

                return this.gridVisualStylesDrawing;
            }

            set
            {
                if (gridVisualStylesDrawing != null)
                {
                    if (gridVisualStylesDrawing is IDisposable)
                    {
                        ((IDisposable)gridVisualStylesDrawing).Dispose();
                    }

                    gridVisualStylesDrawing = null;
                }

                this.gridVisualStylesDrawing = value;
            }
        }
        private Color visualColor = Color.White;
        private Color visualHoverColor = Color.FromArgb(94, 171, 222);
        private Color visualColorPressed = Color.FromArgb(35, 130, 195);
        private Color visualGroupBarColor = Color.FromArgb(94, 171, 222);
        private Color visualHeaderBorderColor = Color.FromArgb(94, 171, 222);
        private Color visualNormalSortIconColor = Color.FromArgb(94, 171, 222);
        private Color visualHoverSortIconColor = Color.White;
        private GridMetroColors visualMetroColors;
        /// <summary>
        /// Sets the custom Metro colors to the Grid.
        /// </summary>
        /// <param name="metroColor">Custom Metro Color.</param>
        /// <param name="metroHoverColor">Custom MouseHover color.</param>
        /// <param name="metroColorPressed">Custom PushButtonPress color.</param>
        public void SetMetroStyles(object metroColor, object metroHoverColor, object metroColorPressed)
        {
            if (metroColor != null)
                visualColor = (Color)metroColor;
            if (metroHoverColor != null)
                visualHoverColor = (Color)metroHoverColor;
            if (metroColorPressed != null)
                visualColorPressed = (Color)metroColorPressed;
        }

        /// <summary>
        /// set the metro color for Grid 
        /// </summary>
        /// <param name="metroColors">Collection of metro color</param>
        public void SetMetroStyles(GridMetroColors metroColors)
        {
            if (metroColors != null)
                visualMetroColors = metroColors;
        }
        /// <summary>
        /// Sets the custom metro colors to the Grid.
        /// </summary>
        /// <param name="metroColor">custom Metro Color.</param>
        /// <param name="metroHoverColor">custom MouseHover color.</param>
        /// <param name="metroColorPressed">custom MouseClick color.</param>
        /// <param name="metroGroupBarColor">custom GroupBar color.</param>
        public void SetMetroStyles(object metroColor, object metroHoverColor, object metroColorPressed, object metroGroupBarColor)
        {
            if (metroColor != null)
                visualColor = (Color)metroColor;
            if (metroHoverColor != null)
                visualHoverColor = (Color)metroHoverColor;
            if (metroColorPressed != null)
                visualColorPressed = (Color)metroColorPressed;
            if (metroGroupBarColor != null)
                visualGroupBarColor = (Color)metroGroupBarColor;
        }

    }

    /// <summary>
    /// Defines behavior when ExcelLikeCurrentCell was specified and user clicked on a cell with
    /// GridStyleInfo.Enabled = false. In versions prior to 3.0 the grid would deactivate the current cell (DeactivateCurrentCell).
    /// With 3.x we changed this behavior to leave the current cell untouched (LeaveCurrentCell).
    /// </summary>
    public enum GridClickedOnDisabledCellBehavior
    {
        /// <summary>
        /// Same as LeaveCurrentCell
        /// </summary>
        Default,

        /// <summary>
        /// Do not deactivate current cell when ExcelLikeCurrentCell was specified and user clicked on a cell with
        /// GridStyleInfo.Enabled = false. 
        /// </summary>
        LeaveCurrentCell,

        /// <summary>
        /// Deactivate the current cell when ExcelLikeCurrentCell was specified and user clicked on a cell with
        /// GridStyleInfo.Enabled = false. 
        /// </summary>
        DeactivateCurrentCell
    }
}
