//-------------------------------------------------------------------------------------------------
// <copyright file="GridSyncProperties.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections;
using System.Security;
using System.Security.Permissions;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Xml.Serialization;
using Syncfusion.ComponentModel;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Diagnostics;
using Syncfusion.Styles;

namespace Syncfusion.Windows.Forms.Grid.Design
{
    #region GridSyncProperties
    /// <exclude/>
    /// <summary>
    ///     Contains all data that is saved out to Xml
    /// </summary>
    [Serializable, TypeConverter(typeof(GridStyleInfoConverter))]
    public class GridSyncProperties : StyleInfoBase, ISerializable
    {
        #region Members
        internal bool bIsModified = false;
        private static GridSyncProperties defaultStyle = null;
        internal bool Locked = false;

        /// <summary>
        /// An empty style object.
        /// </summary>
        ////public static readonly GridSyncProperties Empty = new GridSyncProperties();
        #endregion

        #region Constructors
        static GridSyncProperties()
        {
        }

        /// <summary>
        ///     Constructs a new <see cref="GridSyncProperties"/> object with default values.
        /// </summary>
        public GridSyncProperties()
            : base(new GridSyncPropertiesStore())
        {
            InitProperties();
        }

        void InitProperties()
        {
            GridProperties prop = new GridProperties();
            SetValue(GridSyncPropertiesStore.PropertiesProperty, prop);
            Properties.Changed += new EventHandler(Properties_Changed);
        }

        GridSyncProperties(bool isDefault)
            : base(new GridSyncPropertiesStore())
        {
        }

        /// <summary>
        ///     Constructs a new <see cref="GridSyncProperties"/> object based on an existing value.
        /// </summary>
        /// <param name="style" type="Syncfusion.Windows.Forms.Grid.Design.GridSyncProperties">
        ///     <para>
        ///         The <see cref="GridSyncProperties"/> value to create the object from.
        ///     </para>
        /// </param>
        public GridSyncProperties(GridSyncProperties style)
            : base(style.Store)
        {
            InitProperties();
        }

        /// <summary>
        /// Initializes a new <see cref="GridSyncProperties"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridSyncProperties(SerializationInfo info, StreamingContext context)
            : base(new GridSyncPropertiesStore())
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            ////appearance
            BackColor = (Color)info.GetValue("BackColor", typeof(Color));
            BackgroundImage = (Image)info.GetValue("BackgroundImage", typeof(Image));
            ForeColor = (Color)info.GetValue("ForeColor", typeof(Color));
            BorderStyle = (BorderStyle)info.GetValue("BorderStyle", typeof(BorderStyle));
            HighlightFrozenLine = (bool)info.GetValue("HighlightFrozenLine", typeof(bool));
            RightToLeft = (RightToLeft)info.GetValue("RightToLeft", typeof(RightToLeft));
            ThemesEnabled = (bool)info.GetValue("ThemesEnabled", typeof(bool));
            ////end appearance

            ////grid
            SerializeCellsBehavior = (GridSerializeCellsBehavior)info.GetValue("SerializeCellsBehavior", typeof(GridSerializeCellsBehavior));
            ActivateCurrentCellBehavior = (GridCellActivateAction)info.GetValue("ActivateCurrentCellBehavior", typeof(GridCellActivateAction));
            AllowColumnResizeUsingCellBoundaries = (bool)info.GetValue("AllowColumnResizeUsingCellBoundaries", typeof(bool));
            AllowDragSelectedCols = (bool)info.GetValue("AllowDragSelectedCols", typeof(bool));
            AllowDragSelectedRows = (bool)info.GetValue("AllowDragSelectedRows", typeof(bool));
            AllowRowResizeUsingCellBoundaries = (bool)info.GetValue("AllowRowResizeUsingCellBoundaries", typeof(bool));
            AllowSelection = (GridSelectionFlags)info.GetValue("AllowSelection", typeof(GridSelectionFlags));
            AlphaBlendSelectionColor = (Color)info.GetValue("AlphaBlendSelectionColor", typeof(Color));
            ClickedOnDisabledCellBehavior = (GridClickedOnDisabledCellBehavior)info.GetValue("ClickedOnDisabledCellBehavior", typeof(GridClickedOnDisabledCellBehavior));
            ControllerOptions = (GridControllerOptions)info.GetValue("ControllerOptions", typeof(GridControllerOptions));
            DataObjectConsumerOptions = (GridDataObjectConsumerOptions)info.GetValue("DataObjectConsumerOptions", typeof(GridDataObjectConsumerOptions));
            DefaultGridBorderStyle = (GridBorderStyle)info.GetValue("DefaultGridBorderStyle", typeof(GridBorderStyle));
            DragSelectedCellsMouseButtonsMask = (MouseButtons)info.GetValue("DragSelectedCellsMouseButtonsMask", typeof(MouseButtons));
            DrawOrder = (GridDrawOrder)info.GetValue("DrawOrder", typeof(GridDrawOrder));
            EnterKeyBehavior = (GridDirectionType)info.GetValue("EnterKeyBehavior", typeof(GridDirectionType));
            ExcelLikeCurrentCell = (bool)info.GetValue("ExcelLikeCurrentCell", typeof(bool));
            ExcelLikeSelectionFrame = (bool)info.GetValue("ExcelLikeSelectionFrame", typeof(bool));
            FloatCellsMode = (GridFloatCellsMode)info.GetValue("FloatCellsMode", typeof(GridFloatCellsMode));
            ListBoxSelectionMode = (SelectionMode)info.GetValue("ListBoxSelectionMode", typeof(SelectionMode));
            MinResizeColSize = (int)info.GetValue("MinResizeColSize", typeof(int));
            MinResizeRowSize = (int)info.GetValue("MinResizeRowSize", typeof(int));
            NumberedColHeaders = (bool)info.GetValue("NumberedColHeaders", typeof(bool));
            NumberedRowHeaders = (bool)info.GetValue("NumberedRowHeaders", typeof(bool));
            OptimizeDrawBackground = (bool)info.GetValue("OptimizeDrawBackground", typeof(bool));
            OptimizeInsertRemoveCells = (bool)info.GetValue("OptimizeInsertRemoveCells", typeof(bool));
            RefreshCurrentCellBehavior = (GridRefreshCurrentCellBehavior)info.GetValue("RefreshCurrentCellBehavior", typeof(GridRefreshCurrentCellBehavior));
            ResizeColsBehavior = (GridResizeCellsBehavior)info.GetValue("ResizeColsBehavior", typeof(GridResizeCellsBehavior));
            ResizeRowsBehavior = (GridResizeCellsBehavior)info.GetValue("ResizeRowsBehavior", typeof(GridResizeCellsBehavior));
            SelectCellsMouseButtonsMask = (MouseButtons)info.GetValue("SelectCellsMouseButtonsMask", typeof(MouseButtons));
            ShowCurrentCellBorderBehavior = (GridShowCurrentCellBorder)info.GetValue("ShowCurrentCellBorderBehavior", typeof(GridShowCurrentCellBorder));
            SmoothControlResize = (bool)info.GetValue("SmoothControlResize", typeof(bool));
            ////end grid

            ////grid contents
            ColCount = (int)info.GetValue("ColCount", typeof(int));
            DefaultColWidth = (int)info.GetValue("DefaultColWidth", typeof(int));
            DefaultRowHeight = (int)info.GetValue("DefaultRowHeight", typeof(int));
            Properties = (GridProperties)info.GetValue("Properties", typeof(GridProperties));
            RowCount = (int)info.GetValue("RowCount", typeof(int));
            Cells = (GridCellsMemento)info.GetValue("Cells", typeof(GridCellsMemento));

            try
            {
                Office2007ScrollBars = (bool)info.GetValue("Office2007ScrollBars", typeof(bool));
                Office2007ScrollBarsColorScheme = (Office2007ColorScheme)info.GetValue("Office2007ScrollBarsColorScheme", typeof(Office2007ColorScheme));
                GridVisualStyles = (GridVisualStyles)info.GetValue("GridVisualStyles", typeof(GridVisualStyles));
                UseRightToLeftCompatibleTextBox = (bool)info.GetValue("UseRightToLeftCompatibleTextBox", typeof(bool));
                FrozenRowCount = (int)info.GetValue("FrozenRowCount", typeof(int));
                FrozenColCount = (int)info.GetValue("FrozenColCount", typeof(int));
            }
            catch
            {
                // document was created with earlier version that didn't have these properties.
                GridVisualStyles = GridVisualStyles.SystemTheme;
                UseRightToLeftCompatibleTextBox = false;
                FrozenRowCount = 0;
                FrozenColCount = 0;
            }

            Properties.Changed += new EventHandler(Properties_Changed);
        }
        /// <override/>
        public override void Dispose()
        {
            if (Properties != null)
            {
                Properties.Changed -= new EventHandler(Properties_Changed);
            }

            base.Dispose();
        }

        void Properties_Changed(object sender, EventArgs e)
        {
            this.OnChanged("Properties");
        }
        
        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the <see cref="GridSyncProperties"/>.
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
            GetObjectData(info, context);
        }

        /// <summary>
        /// Returns the data needed to serialize the <see cref="GridSyncProperties"/>.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ////appearance
            info.AddValue("BackColor", BackColor);
            info.AddValue("BackgroundImage", BackgroundImage);
            info.AddValue("ForeColor", ForeColor);
            info.AddValue("BorderStyle", BorderStyle);
            info.AddValue("HighlightFrozenLine", HighlightFrozenLine);
            info.AddValue("RightToLeft", RightToLeft);
            info.AddValue("ThemesEnabled", ThemesEnabled);
            ////end appearance

            ////grid
            info.AddValue("SerializeCellsBehavior", SerializeCellsBehavior);
            info.AddValue("ActivateCurrentCellBehavior", ActivateCurrentCellBehavior);
            info.AddValue("AllowColumnResizeUsingCellBoundaries", AllowColumnResizeUsingCellBoundaries);
            info.AddValue("AllowDragSelectedCols", AllowDragSelectedCols);
            info.AddValue("AllowDragSelectedRows", AllowDragSelectedRows);
            info.AddValue("AllowRowResizeUsingCellBoundaries", AllowRowResizeUsingCellBoundaries);
            info.AddValue("AllowSelection", AllowSelection);
            info.AddValue("AlphaBlendSelectionColor", AlphaBlendSelectionColor);
            info.AddValue("ClickedOnDisabledCellBehavior", ClickedOnDisabledCellBehavior);
            info.AddValue("ControllerOptions", ControllerOptions);
            info.AddValue("DataObjectConsumerOptions", DataObjectConsumerOptions);
            info.AddValue("DefaultGridBorderStyle", DefaultGridBorderStyle);
            info.AddValue("DragSelectedCellsMouseButtonsMask", DragSelectedCellsMouseButtonsMask);
            info.AddValue("DrawOrder", DrawOrder);
            info.AddValue("EnterKeyBehavior", EnterKeyBehavior);
            info.AddValue("ExcelLikeCurrentCell", ExcelLikeCurrentCell);
            info.AddValue("ExcelLikeSelectionFrame", ExcelLikeSelectionFrame);
            info.AddValue("FloatCellsMode", FloatCellsMode);
            info.AddValue("ListBoxSelectionMode", ListBoxSelectionMode);
            info.AddValue("MinResizeColSize", MinResizeColSize);
            info.AddValue("MinResizeRowSize", MinResizeRowSize);
            info.AddValue("NumberedColHeaders", NumberedColHeaders);
            info.AddValue("NumberedRowHeaders", NumberedRowHeaders);
            info.AddValue("OptimizeDrawBackground", OptimizeDrawBackground);
            info.AddValue("OptimizeInsertRemoveCells", OptimizeInsertRemoveCells);
            info.AddValue("RefreshCurrentCellBehavior", RefreshCurrentCellBehavior);
            info.AddValue("ResizeColsBehavior", ResizeColsBehavior);
            info.AddValue("ResizeRowsBehavior", ResizeRowsBehavior);
            info.AddValue("SelectCellsMouseButtonsMask", SelectCellsMouseButtonsMask);
            info.AddValue("ShowCurrentCellBorderBehavior", ShowCurrentCellBorderBehavior);
            info.AddValue("SmoothControlResize", SmoothControlResize);
            ////end grid

            ////grid contents
            info.AddValue("ColCount", ColCount);
            info.AddValue("DefaultColWidth", DefaultColWidth);
            info.AddValue("DefaultRowHeight", DefaultRowHeight);
            info.AddValue("Properties", Properties);
            info.AddValue("RowCount", RowCount);
            info.AddValue("Cells", Cells);
            ////end grid contents

            info.AddValue("Office2007ScrollBars", Office2007ScrollBars);
            info.AddValue("Office2007ScrollBarsColorScheme", Office2007ScrollBarsColorScheme);
            info.AddValue("GridVisualStyles", GridVisualStyles);
            info.AddValue("UseRightToLeftCompatibleTextBox", UseRightToLeftCompatibleTextBox);
            info.AddValue("FrozenRowCount", FrozenRowCount);
            info.AddValue("FrozenColCount", FrozenColCount);
        }

        #endregion

        #region Properties
        /// <exclude/>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XmlIgnore()]
        internal new GridSyncPropertiesStore Store
        {
            get { return (GridSyncPropertiesStore)base.Store; }
        }

        /// <summary>
        /// Gets and initializes a new GridProperties object.
        /// </summary>
        public static GridSyncProperties Default
        {
            get
            {
                if (GridSyncProperties.defaultStyle == null)
                {
                    ////appearance
                    defaultStyle = new GridSyncProperties(true);
                    defaultStyle.BackColor = SystemColors.Control; ////Color.FromKnownColor(KnownColor.Control);//Color.FromName("Control");
                    defaultStyle.BackgroundImage = null;
                    
                    ////Cursor Cursor ;
                    defaultStyle.Font = GridFontInfo.Default.GdipFont; ////new Font("Verdana",10);
                    defaultStyle.ForeColor = Color.FromKnownColor(KnownColor.WindowText);
                    defaultStyle.HighlightFrozenLine = true;
                    defaultStyle.RightToLeft = RightToLeft.Inherit;
                    defaultStyle.ThemesEnabled = false;
                    ////end appearance

                    ////grid
                    defaultStyle.ActivateCurrentCellBehavior = GridCellActivateAction.ClickOnCell;
                    defaultStyle.AllowColumnResizeUsingCellBoundaries = false;
                    defaultStyle.AllowDragSelectedCols = false;
                    defaultStyle.AllowDragSelectedRows = false;
                    defaultStyle.AllowRowResizeUsingCellBoundaries = false;
                    defaultStyle.AllowSelection = GridSelectionFlags.Any;
                    defaultStyle.AlphaBlendSelectionColor = Color.Empty;
                    defaultStyle.ClickedOnDisabledCellBehavior = GridClickedOnDisabledCellBehavior.Default;
                    defaultStyle.ControllerOptions = GridControllerOptions.All;
                    defaultStyle.DataObjectConsumerOptions = GridDataObjectConsumerOptions.All;
                    defaultStyle.DefaultGridBorderStyle = GridBorderStyle.Dotted;
                    defaultStyle.DragSelectedCellsMouseButtonsMask = MouseButtons.Left;
                    defaultStyle.DrawOrder = GridDrawOrder.Rows;
                    defaultStyle.EnterKeyBehavior = GridDirectionType.Right;
                    defaultStyle.ExcelLikeCurrentCell = false;
                    defaultStyle.ExcelLikeSelectionFrame = false;
                    defaultStyle.FloatCellsMode = GridFloatCellsMode.None;
                    defaultStyle.Office2007ScrollBars = false;
                    defaultStyle.Office2007ScrollBarsColorScheme = Office2007ColorScheme.Blue;
                    defaultStyle.GridVisualStyles = GridVisualStyles.SystemTheme;
                    defaultStyle.UseRightToLeftCompatibleTextBox = false;
                    defaultStyle.FrozenRowCount = 0;
                    defaultStyle.FrozenColCount = 0;
                    defaultStyle.ListBoxSelectionMode = SelectionMode.None;
                    defaultStyle.MinResizeColSize = 0;
                    defaultStyle.MinResizeRowSize = 0;
                    defaultStyle.NumberedColHeaders = true;
                    defaultStyle.NumberedRowHeaders = true;
                    defaultStyle.OptimizeDrawBackground = true;
                    defaultStyle.OptimizeInsertRemoveCells = false;
                    defaultStyle.RefreshCurrentCellBehavior = GridRefreshCurrentCellBehavior.RefreshCell;
                    defaultStyle.ResizeColsBehavior = GridResizeCellsBehavior.ResizeSingle | GridResizeCellsBehavior.OutlineHeaders;
                    defaultStyle.ResizeRowsBehavior = GridResizeCellsBehavior.ResizeSingle | GridResizeCellsBehavior.OutlineHeaders;
                    defaultStyle.SelectCellsMouseButtonsMask = MouseButtons.Left | MouseButtons.Middle | MouseButtons.Right;
                    defaultStyle.ShowCurrentCellBorderBehavior = GridShowCurrentCellBorder.WhenGridActive;
                    defaultStyle.SmoothControlResize = true;
                    ////end grid

                    ////grid contents
                    defaultStyle.ColCount = 10;
                    defaultStyle.DefaultColWidth = 65;
                    defaultStyle.DefaultRowHeight = 17;
                    defaultStyle.Properties = null; //// new GridProperties();
                    defaultStyle.RowCount = 10;
                    ////end grid contents
                }

                return GridSyncProperties.defaultStyle;
            }
        }

        /// <summary>
        /// Override this method to return a default style object for your derived class.
        /// </summary>
        /// <returns>A default style object.</returns>
        /// <override/>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }

        /// <summary>
        ///     Gets a value indicating whether properties have been modified
        /// </summary>
        [Browsable(false), XmlIgnore, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Modified
        {
            get
            {
                return this.bIsModified;
            }
        }

        /// <summary>
        /// Resets the <see cref="Modified"/> flag.
        /// </summary>
        public void ResetModified()
        {
            this.bIsModified = false;
        }

        /// <summary>
        /// Represents the method that handles the <see cref="GridSyncProperties.PropertyChanged"/> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="PropertyChangedEventArgs"/> that contains the event data.</param>
        public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);
        
        /// <summary>
        /// Occurs when any property in this object is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Fires the PropertyChanged event
        /// </summary>
        void OnChanged(string propName)
        {
            if (shouldRaiseChanged && PropertyChanged != null)
            {
                this.bIsModified = true;
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        internal bool shouldRaiseChanged = false;

        /// <summary>
        ///    Gets or sets collection of Image and ID values for the <see cref="GridControl"/>
        /// </summary>
        [Browsable(false)]
        public GridNamespaceGroupItemCollection StoredImages
        {
            get
            {
                object val = GetValue(GridSyncPropertiesStore.StoredImagesProperty);
                if (val == null)
                {
                    val = new GridNamespaceGroupItemCollection();
                    SetValue(GridSyncPropertiesStore.StoredImagesProperty, val);
                }

                return (GridNamespaceGroupItemCollection)val;
            }

            set
            {
                SetValue(GridSyncPropertiesStore.StoredImagesProperty, value);
            }
        }

        /// <summary>
        ///    Gets or sets collection of Font and ID values for the <see cref="GridControl"/>
        /// </summary>
        [Browsable(false)]
        public GridNamespaceGroupItemCollection StoredFonts
        {
            get
            {
                object val = GetValue(GridSyncPropertiesStore.StoredFontsProperty);
                if (val == null)
                {
                    val = new GridNamespaceGroupItemCollection();
                    SetValue(GridSyncPropertiesStore.StoredFontsProperty, val);
                }

                return (GridNamespaceGroupItemCollection)val;
            }

            set
            {
                SetValue(GridSyncPropertiesStore.StoredFontsProperty, value);
            }
        }

        /// <summary>
        ///     Resets the <see cref="StoredFonts"/> and <see cref="StoredImages"/> collections
        /// </summary>
        public void ResetStyleGroups()
        {
            StoredFonts.Clear();
            StoredImages.Clear();
        }

        ////appearance

        /// <summary>
        /// Gets or sets the background color for the grid control.
        /// </summary>
        [Description("Background Color for the GridControl")]
        [Category("Appearance")]
        public Color BackColor
        {
            get
            {
                return (Color)GetValue(GridSyncPropertiesStore.BackColorProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.BackColorProperty, value);
                OnChanged("BackColor");
            }
        }

        /// <copyfrom cref="GridControlBase.BackgroundImageID"/>
        /// <summary>Gets or sets the Namespace ID that contains the grids's background image information id.</summary>
        [Browsable(false)]
        [DefaultValue("")]
        public string BackgroundImageID
        {
            get
            {
                return (string)GetValue(GridSyncPropertiesStore.BackgroundImageIDProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.BackgroundImageIDProperty, value);
            }
        }

        /// <summary>
        ///     Conversion method that takes a byte[] and returns a Bitmap
        /// </summary>
        /// <param name="bytes" type="byte[]">
        ///     <para>
        ///            byte[] to convert         
        ///     </para>
        /// </param>
        /// <returns>
        ///     A System.Drawing.Bitmap value...
        /// </returns>
        public static Bitmap GetImageFromBytes(byte[] bytes)
        {
            Bitmap bmp = null;
            try
            {
                bmp = new Bitmap(new MemoryStream(bytes));
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                bmp = null;
            }

            return bmp;
        }

        /// <summary>
        ///     Creates a byte[] from an existing Bitmap
        /// </summary>
        /// <param name="bmp" type="System.Drawing.Bitmap">
        ///     <para>
        ///         Bitmap to retrieve values from
        ///     </para>
        /// </param>
        /// <returns>
        ///     A byte[] value...
        /// </returns>
        public static byte[] GetImageBytes(Bitmap bmp)
        {
            MemoryStream memStream = new MemoryStream();
            bmp.Save(memStream, System.Drawing.Imaging.ImageFormat.Bmp);
            int ImageSize = Convert.ToInt32(memStream.Length);
            byte[] ImageBytes = new byte[ImageSize];
            memStream.Position = 0;
            memStream.Read(ImageBytes, 0, ImageSize);
            memStream.Close();
            return ImageBytes;
        }

        /// <copyfrom cref="GridControl.BackgroundImage"/>
#if SyncfusionFramework2_0
        [Browsable(false)]
#endif
        [Localizable(true),
        DefaultValue(null),
        Description(@"The background image used for the grid."),
        Category(@"Appearance"),
        RefreshProperties(RefreshProperties.Repaint)]
        public Image BackgroundImage
        {
            get
            {
                return (Image)GetValue(GridSyncPropertiesStore.BackgroundImageProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.BackgroundImageProperty, value);
                OnChanged("BackgroundImage");
            }
        }

        /// <summary>
        ///    Gets or sets cell, row, and column specific data used for synchronization and serialization
        /// </summary>
        [Browsable(false)]
        public GridCellsMemento Cells
        {
            get
            {
                GridCellsMemento dw = (GridCellsMemento)GetValue(GridSyncPropertiesStore.DataProperty);
                if (dw == null)
                {
                    dw = new GridCellsMemento();
                    SetValue(GridSyncPropertiesStore.DataProperty, dw);
                }

                return dw;
            }

            set
            {
                SetValue(GridSyncPropertiesStore.DataProperty, value);
                OnChanged("Data");
            }
        }

        /// <copyfrom cref="BorderStyle"/>
        /// <summary>Gets or sets the border style of the grid control.</summary>
        [Description("Border Style of the grid control")]
        [Category("Appearance")]
        [DefaultValue(BorderStyle.None)]
        public BorderStyle BorderStyle
        {
            get
            {
                return (BorderStyle)GetValue(GridSyncPropertiesStore.BorderStyleProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.BorderStyleProperty, value);
                OnChanged("BorderStyle");
            }
        }

        /// <copyfrom cref="GridControl.Font"/>
        /// <summary>Gets or sets the font to display in the grid.</summary>
        [Description("Font to display in the grid")]
        [Category("Appearance")]
        [XmlIgnore()]
        public Font Font
        {
            get
            {
                return (Font)GetValue(GridSyncPropertiesStore.FontProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.FontProperty, value);
                OnChanged("Font");
            }
        }

        /// <copyfrom cref="Control.ForeColor"/>
        /// <summary>Gets or sets the text color.</summary>
        [Description("Color of the text")]
        [Category("Appearance")]
        ////[XmlIgnore]
        public Color ForeColor
        {
            get
            {
                return (Color)GetValue(GridSyncPropertiesStore.ForeColorProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.ForeColorProperty, value);
                OnChanged("ForeColor");
            }
        }

        /// <copyfrom cref="GridControl.HighlightFrozenLine"/>
        /// <summary>Gets or sets a value indicating whether border style to be used as default for cell borders.</summary>
        [Description("")]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool HighlightFrozenLine
        {
            get
            {
                return (bool)GetValue(GridSyncPropertiesStore.HighlightFrozenLineProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.HighlightFrozenLineProperty, value);
                OnChanged("HighlightFrozenLine");
            }
        }

        /// <copyfrom cref="Control.RightToLeft"/>
        /// <summary>Gets or sets whether the text appears from right to left.</summary>
        [Description("")]
        [Category("Appearance")]
        [DefaultValue(RightToLeft.No)]
        public RightToLeft RightToLeft
        {
            get
            {
                return (RightToLeft)GetValue(GridSyncPropertiesStore.RightToLeftProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.RightToLeftProperty, value);
                OnChanged("RightToLeft");
            }
        }

        /// <copyfrom cref="ThemesEnabled"/>
        /// <summary>Gets or sets a value indicating whether the themes for the grid are enabled.</summary>
        [Description("")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool ThemesEnabled
        {
            get
            {
                return (bool)GetValue(GridSyncPropertiesStore.ThemesEnabledProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.ThemesEnabledProperty, value);
                OnChanged("ThemesEnabled");
            }
        }

        ////end appearance

        ////grid

        /// <copyfrom cref="GridControl.ActivateCurrentCellBehavior"/>
        /// <summary>Gets or sets current cell activation behavior.</summary>
        [Description("")]
        [Category("Grid")]
        [DefaultValue(GridCellActivateAction.ClickOnCell)]
        public GridCellActivateAction ActivateCurrentCellBehavior
        {
            get
            {
                return (GridCellActivateAction)GetValue(GridSyncPropertiesStore.ActivateCurrentCellBehaviorProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.ActivateCurrentCellBehaviorProperty, value);
                OnChanged("ActivateCurrentCellBehavior");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow resizing of columns through standard cell boundaries. For GridListControl, it is true by default.
        /// </summary>
        [Description("Enables resizing of columns through standard cell boundaries.")]
        [Category("Grid")]
        [DefaultValue(false)]
        public virtual bool AllowColumnResizeUsingCellBoundaries
        {
            get
            {
                return (bool)GetValue(GridSyncPropertiesStore.AllowColumnResizeUsingCellBoundaries);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.AllowColumnResizeUsingCellBoundaries, value);
                OnChanged("AllowColumnResizeUsingCellBoundaries");
            }
        }

        /// <copyfrom cref="GridControl.AllowDragSelectedCols"/>
        /// <summary>Gets or sets a value indicating whether the control allows the user to drag selected columns by clicking on the column header.</summary>
        [Description("")]
        [Category("Grid")]
        [DefaultValue(false)]
        public bool AllowDragSelectedCols
        {
            get
            {
                return (bool)GetValue(GridSyncPropertiesStore.AllowDragSelectedColsProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.AllowDragSelectedColsProperty, value);
                OnChanged("AllowDragSelectedCols");
            }
        }

        /// <copyfrom cref="GridControl.AllowDragSelectedRows"/>
        /// <summary>Gets or sets a value indicating whether the control allows the user to drag selected rows by clicking on the row header.</summary>
        [Description("")]
        [Category("Grid")]
        [DefaultValue(false)]
        public bool AllowDragSelectedRows
        {
            get
            {
                return (bool)GetValue(GridSyncPropertiesStore.AllowDragSelectedRowsProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.AllowDragSelectedRowsProperty, value);
                OnChanged("AllowDragSelectedRows");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow resizing of rows through standard cell boundaries. No support for GridListControl.
        /// </summary>
        [Description("Enables resizing of rows through standard cell boundaries.")]
        [Category("Grid")]
        [DefaultValue(false)]
        public virtual bool AllowRowResizeUsingCellBoundaries
        {
            get
            {
                return (bool)GetValue(GridSyncPropertiesStore.AllowRowResizeUsingCellBoundaries);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.AllowRowResizeUsingCellBoundaries, value);
                OnChanged("AllowRowResizeUsingCellBoundaries");
            }
        }

        /// <copyfrom cref="GridControl.AllowSelection"/>
        /// <summary>Gets or sets selection behavior of the grid.</summary>
        [Description("")]
        [Category("Grid")]
        [Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)), DefaultValue(GridSelectionFlags.Any)]
        public GridSelectionFlags AllowSelection
        {
            get
            {
                return (GridSelectionFlags)GetValue(GridSyncPropertiesStore.AllowSelectionProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.AllowSelectionProperty, value);
                OnChanged("AllowSelection");
            }
        }

        /// <copyfrom cref="GridControl.AlphaBlendSelectionColor"/>
        /// <summary>Gets or sets the color for alpha blended cell selections.</summary>
        [Description("")]
        [Category("Grid")]
        public Color AlphaBlendSelectionColor
        {
            get
            {
                return (Color)GetValue(GridSyncPropertiesStore.AlphaBlendSelectionColorProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.AlphaBlendSelectionColorProperty, value);
                OnChanged("AlphaBlendSelectionColor");
            }
        }

        /// <copyfrom cref="GridControl.ClickedOnDisabledCellBehavior"/>
        /// <summary>Gets or sets Excel-like current cell behavior. When the user clicks on a cell out of a selected range for which .Enabled has been set to false.</summary>
        [Description("")]
        [Category("Grid")]
        [DefaultValue(GridClickedOnDisabledCellBehavior.Default)]
        public GridClickedOnDisabledCellBehavior ClickedOnDisabledCellBehavior
        {
            get
            {
                return (GridClickedOnDisabledCellBehavior)GetValue(GridSyncPropertiesStore.ClickedOnDisabledCellBehaviorProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.ClickedOnDisabledCellBehaviorProperty, value);
                OnChanged("ClickedOnDisabledCellBehavior");
            }
        }

        /// <copyfrom cref="GridControl.ControllerOptions"/>
        /// <summary>Gets or sets which mouse controllers should be enabled for the grid.</summary>
        [Description("")]
        [Category("Grid")]
        [Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)), DefaultValue(GridControllerOptions.All)]
        public GridControllerOptions ControllerOptions
        {
            get
            {
                return (GridControllerOptions)GetValue(GridSyncPropertiesStore.ControllerOptionsProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.ControllerOptionsProperty, value);
                OnChanged("ControllerOptions");
            }
        }

        /// <copyfrom cref="GridControl.DataObjectConsumerOptions"/>
        /// <summary>Gets or sets controls clipboard interchange format. Can be plain text and/or fully formatted with styles.</summary>
        [Description("")]
        [Category("Grid")]
        [Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)), DefaultValue(GridDataObjectConsumerOptions.All)]
        public GridDataObjectConsumerOptions DataObjectConsumerOptions
        {
            get
            {
                return (GridDataObjectConsumerOptions)GetValue(GridSyncPropertiesStore.DataObjectConsumerOptionsProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.DataObjectConsumerOptionsProperty, value);
                OnChanged("DataObjectConsumerOptions");
            }
        }

        /// <copyfrom cref="GridControl.DefaultGridBorderStyle"/>
        /// <summary>Gets or sets the border style to be used as default for cell borders.</summary>
        [Description("")]
        [Category("Grid")]
        [DefaultValue(GridBorderStyle.Dotted)]
        public GridBorderStyle DefaultGridBorderStyle
        {
            get
            {
                return (GridBorderStyle)GetValue(GridSyncPropertiesStore.DefaultGridBorderStyleProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.DefaultGridBorderStyleProperty, value);
                OnChanged("DefaultGridBorderStyle");
            }
        }

        /// <copyfrom cref="GridControl.DragSelectedCellsMouseButtonsMask"/>
        /// <summary>Gets or sets which mouse buttons can be used for dragging selected rows or columns.</summary>
        [Description("")]
        [Category("Grid")]
        [Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)), DefaultValue(MouseButtons.Left)]
        public MouseButtons DragSelectedCellsMouseButtonsMask
        {
            get
            {
                return (MouseButtons)GetValue(GridSyncPropertiesStore.DragSelectedCellsMouseButtonsMaskProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.DragSelectedCellsMouseButtonsMaskProperty, value);
                OnChanged("DragSelectedCellsMouseButtonsMask");
            }
        }

        /// <copyfrom cref="GridControl.DrawOrder"/>
        /// <summary>Gets or sets the order how cells are loaded before the grid is displayed.</summary>
        [Description("")]
        [Category("Grid")]
        [DefaultValue(GridDrawOrder.Rows)]
        public GridDrawOrder DrawOrder
        {
            get
            {
                return (GridDrawOrder)GetValue(GridSyncPropertiesStore.DrawOrderProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.DrawOrderProperty, value);
                OnChanged("DrawOrder");
            }
        }

        /// <copyfrom cref="GridControl.EnterKeyBehavior"/>
        /// <summary>Gets or sets movement of current cell when enter key is pressed.</summary>
        [Description("")]
        [Category("Grid")]
        [DefaultValue(GridDirectionType.Right)]
        public GridDirectionType EnterKeyBehavior
        {
            get
            {
                return (GridDirectionType)GetValue(GridSyncPropertiesStore.EnterKeyBehaviorProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.EnterKeyBehaviorProperty, value);
                OnChanged("EnterKeyBehavior");
            }
        }

        /// <copyfrom cref="GridControl.ExcelLikeCurrentCell"/>
        /// <summary>Gets or sets a value indicating whether Excel-like current cell behavior. When the user moves the current cell out of a selected range, the range will be cleared.</summary>
        [Description("")]
        [Category("Grid")]
        [DefaultValue(false)]
        public bool ExcelLikeCurrentCell
        {
            get
            {
                return (bool)GetValue(GridSyncPropertiesStore.ExcelLikeCurrentCellProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.ExcelLikeCurrentCellProperty, value);
                OnChanged("ExcelLikeCurrentCell");
            }
        }

        /// <copyfrom cref="GridControl.ExcelLikeSelectionFrame"/>
        /// <summary>Gets or sets a value indicating whether the active selection should be outlined with a selection frame.</summary>
        [Description("")]
        [Category("Grid")]
        [DefaultValue(false)]
        public bool ExcelLikeSelectionFrame
        {
            get
            {
                return (bool)GetValue(GridSyncPropertiesStore.ExcelLikeSelectionFrameProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.ExcelLikeSelectionFrameProperty, value);
                OnChanged("ExcelLikeSelectionFrame");
            }
        }

        /// <copyfrom cref="GridControl.FloatCellsMode"/>
        /// <summary>Gets or sets floating cells behavior for the grid.</summary>
        [Description("")]
        [Category("Grid")]
        [DefaultValue(GridFloatCellsMode.None)]
        public GridFloatCellsMode FloatCellsMode
        {
            get
            {
                return (GridFloatCellsMode)GetValue(GridSyncPropertiesStore.FloatCellsModeProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.FloatCellsModeProperty, value);
                OnChanged("FloatCellsMode");
            }
        }

        /// <copyfrom cref="GridControl.GridVisualStyles"/>
        /// <summary>Gets or sets look and feel skins for the Grid.</summary>
        [Description("")]
        [Category("Grid")]
        [DefaultValue(GridVisualStyles.SystemTheme)]
        public GridVisualStyles GridVisualStyles
        {
            get
            {
                return (GridVisualStyles)GetValue(GridSyncPropertiesStore.GridVisualStylesProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.GridVisualStylesProperty, value);
                OnChanged("GridVisualStyles");
            }
        }

        /// <copyfrom cref="GridControlBase.Office2007ScrollBars"/>
        /// <summary>Gets or sets a value indicating whether to toggle between standard and Office2007 scrollbars.</summary>
        [Description("")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool Office2007ScrollBars
        {
            get
            {
                return (bool)GetValue(GridSyncPropertiesStore.Office2007ScrollBarsProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.Office2007ScrollBarsProperty, value);
                OnChanged("Office2007ScrollBars");
            }
        }

        /// <copyfrom cref="GridControlBase.Office2007ScrollBarsColorScheme"/>
        /// <summary>
        /// Gets or sets the style of Office2007 scroll bars
        /// </summary>
        [Description("")]
        [Category("Appearance")]
        [DefaultValue(Office2007ColorScheme.Blue)]
        public Office2007ColorScheme Office2007ScrollBarsColorScheme
        {
            get
            {
                return (Office2007ColorScheme)GetValue(GridSyncPropertiesStore.Office2007ScrollBarsColorSchemeProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.Office2007ScrollBarsColorSchemeProperty, value);
                OnChanged("Office2007ScrollBarsColorScheme");
            }
        }

        /// <copyfrom cref="GridControlBase.GridOfficeScrollBars"/>
        /// <summary>
        /// Gets or sets a value indicating whether to toggle among standard, Office2007 and Office2010 scrollbars.
        /// </summary>
        [Description("Gets or sets a value indicating whether to toggle among standard, Office2007 and Office2010 scrollbars.")]
        [Category("Appearance")]
        [DefaultValue(OfficeScrollBars.None)]
        public OfficeScrollBars GridOfficeScrollBars
        {
            get
            {
                return (OfficeScrollBars)GetValue(GridSyncPropertiesStore.GridOfficeScrollBarsProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.GridOfficeScrollBarsProperty, value);
                OnChanged("GridOfficeScrollBars");
            }
        }

        /// <copyfrom cref="GridControlBase.Office2007ScrollBarsColorScheme"/>
        /// <summary>
        /// Gets or sets the style of Office2010 scroll bars.
        /// </summary>
        [Description("Gets or sets the style of Office2010 scroll bars.")]
        [Category("Appearance")]
        [DefaultValue(Office2010ColorScheme.Blue)]
        public Office2010ColorScheme Office2010ScrollBarsColorScheme
        {
            get
            {
                return (Office2010ColorScheme)GetValue(GridSyncPropertiesStore.Office2010ScrollBarsColorSchemeProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.Office2010ScrollBarsColorSchemeProperty, value);
                OnChanged("Office2010ScrollBarsColorScheme");
            }
        }


        /// <copyfrom cref="GridControl.UseRightToLeftCompatibleTextBox"/>
        /// <summary>Gets or sets a value indicating whether to controls the kind of textbox control that is created for TextBox cells. In general the original text box behaves better than the default richtext box with Hebrew and arabic languages.</summary>
        [Description("")]
        [Category("Grid")]
        [DefaultValue(false)]
        public bool UseRightToLeftCompatibleTextBox
        {
            get
            {
                return (bool)GetValue(GridSyncPropertiesStore.UseRightToLeftCompatibleTextBoxProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.UseRightToLeftCompatibleTextBoxProperty, value);
                OnChanged("UseRightToLeftCompatibleTextBox");
            }
        }

        /// <summary>
        /// Gets or sets number of frozen rows.
        /// </summary>
        [Description("")]
        [Category("Grid")]
        [DefaultValue(0)]
        public int FrozenRowCount
        {
            get
            {
                return (int)GetValue(GridSyncPropertiesStore.FrozenRowCountProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.FrozenRowCountProperty, value);
                OnChanged("FrozenRowCount");
            }
        }

        /// <summary>
        /// Gets or sets number of frozen columns.
        /// </summary>
        [Description("")]
        [Category("Grid")]
        [DefaultValue(0)]
        public int FrozenColCount
        {
            get
            {
                return (int)GetValue(GridSyncPropertiesStore.FrozenColCountProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.FrozenColCountProperty, value);
                OnChanged("FrozenColCount");
            }
        }

        /// <copyfrom cref="GridControl.ListBoxSelectionMode"/>
        /// <summary>
        ///   <para> Gets or sets the method in which items are selected in
        /// the <see cref="GridControl" /> when it is being used in listbox mode
        /// .</para>
        /// </summary>
        [Description("")]
        [Category("Grid")]
        [DefaultValue(SelectionMode.None)]
        public SelectionMode ListBoxSelectionMode
        {
            get
            {
                return (SelectionMode)GetValue(GridSyncPropertiesStore.ListBoxSelectionModeProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.ListBoxSelectionModeProperty, value);
                OnChanged("ListBoxSelectionMode");
            }
        }

        /// <copyfrom cref="GridControl.MinResizeColSize"/>
        /// <summary>Gets or sets the minimum column width when the user resizes a column with the mouse.</summary>
        [Description("")]
        [Category("Grid")]
        [DefaultValue(0)]
        public int MinResizeColSize
        {
            get
            {
                return (int)GetValue(GridSyncPropertiesStore.MinResizeColSizeProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.MinResizeColSizeProperty, value);
                OnChanged("MinResizeColSize");
            }
        }

        /// <copyfrom cref="GridControl.MinResizeRowSize"/>
        /// <summary>Gets or sets the minimum row height when the user resizes a row with the mouse.</summary>
        [Description("")]
        [Category("Grid")]
        [DefaultValue(0)]
        public int MinResizeRowSize
        {
            get
            {
                return (int)GetValue(GridSyncPropertiesStore.MinResizeRowSizeProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.MinResizeRowSizeProperty, value);
                OnChanged("MinResizeRowSize");
            }
        }

        /// <copyfrom cref="GridControl.NumberedColHeaders"/>
        /// <summary>Gets or sets a value indicating whether to toggle display of column ids (A, B, C, ...) in column headers.</summary>
        [Description("")]
        [Category("Grid")]
        [DefaultValue(true)]
        public bool NumberedColHeaders
        {
            get
            {
                return (bool)GetValue(GridSyncPropertiesStore.NumberedColHeadersProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.NumberedColHeadersProperty, value);
                OnChanged("NumberedColHeaders");
            }
        }

        /// <copyfrom cref="GridControl.NumberedRowHeaders"/>
        /// <summary>Gets or sets a value indicating whether to toggle display of row numbers in row headers.</summary>
        [Description("")]
        [Category("Grid")]
        [DefaultValue(true)]
        public bool NumberedRowHeaders
        {
            get
            {
                return (bool)GetValue(GridSyncPropertiesStore.NumberedRowHeadersProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.NumberedRowHeadersProperty, value);
                OnChanged("NumberedRowHeaders");
            }
        }

        /// <copyfrom cref="GridControlBase.OptimizeDrawBackground"/>
        /// <summary>Gets or sets a value indicating whether enable built-in optimization that allows grid to combine background drawing for cells that have the same background.</summary>
        [Description("")]
        [Category("Grid")]
        [DefaultValue(true)]
        public bool OptimizeDrawBackground
        {
            get
            {
                return (bool)GetValue(GridSyncPropertiesStore.OptimizeDrawBackgroundProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.OptimizeDrawBackgroundProperty, value);
                OnChanged("OptimizeDrawBackground");
            }
        }

        /// <copyfrom cref="GridControlBase.OptimizeInsertRemoveCells"/>
        /// ///<summary>Gets or sets a value indicating whether to enable optimization for inserting and removing cells by scrolling window contents and only invalidating new cells.</summary>
        [Description("")]
        [Category("Grid")]
        [DefaultValue(true)]
        public bool OptimizeInsertRemoveCells
        {
            get
            {
                return (bool)GetValue(GridSyncPropertiesStore.OptimizeInsertRemoveCellsProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.OptimizeInsertRemoveCellsProperty, value);
                OnChanged("OptimizeInsertRemoveCells");
            }
        }

        /// <copyfrom cref="GridControl.RefreshCurrentCellBehavior"/>
        /// <summary>Gets or sets which cells to refresh when moving the current cell.</summary>
        [Description("")]
        [Category("Grid")]
        [DefaultValue(GridRefreshCurrentCellBehavior.RefreshCell)]
        public GridRefreshCurrentCellBehavior RefreshCurrentCellBehavior
        {
            get
            {
                return (GridRefreshCurrentCellBehavior)GetValue(GridSyncPropertiesStore.RefreshCurrentCellBehaviorProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.RefreshCurrentCellBehaviorProperty, value);
                OnChanged("RefreshCurrentCellBehavior");
            }
        }

        /// <copyfrom cref="GridControl.ResizeColsBehavior"/>
        /// <summary>Gets or sets behavior for resizing columns.</summary>
        [Description("")]
        [Category("Grid")]
        [Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)), DefaultValue(GridResizeCellsBehavior.ResizeSingle | GridResizeCellsBehavior.OutlineHeaders | GridResizeCellsBehavior.OutlineBounds)]
        public GridResizeCellsBehavior ResizeColsBehavior
        {
            get
            {
                return (GridResizeCellsBehavior)GetValue(GridSyncPropertiesStore.ResizeColsBehaviorProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.ResizeColsBehaviorProperty, value);
                OnChanged("ResizeColsBehavior");
            }
        }

        /// <copyfrom cref="GridControl.ResizeRowsBehavior"/>
        /// <summary>Gets or sets behavior for resizing rows.</summary>
        [Description("")]
        [Category("Grid")]
        [Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)), DefaultValue(GridResizeCellsBehavior.ResizeSingle | GridResizeCellsBehavior.OutlineHeaders | GridResizeCellsBehavior.OutlineBounds)]
        public GridResizeCellsBehavior ResizeRowsBehavior
        {
            get
            {
                return (GridResizeCellsBehavior)GetValue(GridSyncPropertiesStore.ResizeRowsBehaviorProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.ResizeRowsBehaviorProperty, value);
                OnChanged("ResizeRowsBehavior");
            }
        }

        /// <copyfrom cref="GridControl.SelectCellsMouseButtonsMask"/>
        /// <summary>Gets or sets which mouse buttons can be used for selecting cells.</summary>
        [Description("")]
        [Category("Grid")]
        [Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)), DefaultValue(MouseButtons.Left | MouseButtons.Middle | MouseButtons.Right)]
        public MouseButtons SelectCellsMouseButtonsMask
        {
            get
            {
                return (MouseButtons)GetValue(GridSyncPropertiesStore.SelectCellsMouseButtonsMaskProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.SelectCellsMouseButtonsMaskProperty, value);
                OnChanged("SelectCellsMouseButtonsMask");
            }
        }

        /// <copyfrom cref="GridControl.ShowCurrentCellBorderBehavior"/>
        /// <summary>Gets or sets when to show current cell frame or border.</summary>
        [Description("")]
        [Category("Grid")]
        [DefaultValue(GridShowCurrentCellBorder.WhenGridActive)]
        public GridShowCurrentCellBorder ShowCurrentCellBorderBehavior
        {
            get
            {
                return (GridShowCurrentCellBorder)GetValue(GridSyncPropertiesStore.ShowCurrentCellBorderBehaviorProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.ShowCurrentCellBorderBehaviorProperty, value);
                OnChanged("ShowCurrentCellBorderBehavior");
            }
        }

        /// <summary>Gets or sets a value indicating whether a grid should be completely refreshed when the user resizes the window or if only newly visible rows or columns should be redrawn.</summary>
        /// <copyfrom cref="GridControl.SmoothControlResize"/>
        [Description("")]
        [Category("Grid")]
        [DefaultValue(true)]
        public bool SmoothControlResize
        {
            get
            {
                return (bool)GetValue(GridSyncPropertiesStore.SmoothControlResizeProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.SmoothControlResizeProperty, value);
                OnChanged("SmoothControlResize");
            }
        }

        ////end grid

        /// <copyfrom cref="GridControl.ColCount"/>
        /// <summary>Gets or sets the number of grid columns.</summary>
        ////grid contents
        [Description("")]
        [Category("Grid Content")]
        [DefaultValue(10)]
        public int ColCount
        {
            get
            {
                return (int)GetValue(GridSyncPropertiesStore.ColCountProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.ColCountProperty, value);
                OnChanged("ColCount");
            }
        }

        /// <copyfrom cref="GridControlBase.DefaultColWidth"/>
        /// <summary>Gets or sets the default width used for grid columns.</summary>
        [Description("")]
        [Category("Grid Content")]
        [DefaultValue(65)]
        public int DefaultColWidth
        {
            get
            {
                return (int)GetValue(GridSyncPropertiesStore.DefaultColWidthProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.DefaultColWidthProperty, value);
                OnChanged("DefaultColWidth");
            }
        }

        /// <copyfrom cref="GridControlBase.DefaultRowHeight"/>
        /// <summary>Gets or sets the default height used for grid rows.</summary>
        [Description("")]
        [Category("Grid Content")]
        [DefaultValue(17)]
        public int DefaultRowHeight
        {
            get
            {
                return (int)GetValue(GridSyncPropertiesStore.DefaultRowHeightProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.DefaultRowHeightProperty, value);
                OnChanged("DefaultRowHeight");
            }
        }

        /// <copyfrom cref="GridControl.Properties"/>
        /// <summary>Gets or sets more options for the grid. Printing related. Also manages colors for grid background, grid lines, and more.</summary>
        [Description("")]
        [Category("Grid Content")]
        public GridProperties Properties
        {
            get
            {
                GridProperties prop = (GridProperties)GetValue(GridSyncPropertiesStore.PropertiesProperty);
                return prop;
            }

            set
            {
                if (!Object.ReferenceEquals(Properties, value))
                {
                    if (Properties != null)
                    {
                        Properties.Changed -= new EventHandler(Properties_Changed);
                    }

                    SetValue(GridSyncPropertiesStore.PropertiesProperty, value);
                    Properties.Changed += new EventHandler(Properties_Changed);
                    OnChanged("Properties");
                }
            }
        }

        /// <copyfrom cref="GridControl.RowCount"/>
        /// <summary>Gets or sets the number of rows in the grid.</summary>
        [Description("")]
        [Category("Grid Content")]
        [DefaultValue(10)]
        public int RowCount
        {
            get
            {
                return (int)GetValue(GridSyncPropertiesStore.RowCountProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.RowCountProperty, value);
                OnChanged("RowCount");
            }
        }

        /// <copyfrom cref="GridControl.SerializeCellsBehavior"/>
        /// <summary>Gets or sets to serialize cell contents as code or into a ResX file.</summary>
        [Category("Grid Content")]
        [DefaultValue(GridSerializeCellsBehavior.SerializeIntoCode)]
        [Description("Choose to serialize cell contents as code or into a ResX file.")]
        public GridSerializeCellsBehavior SerializeCellsBehavior
        {
            get
            {
                return (GridSerializeCellsBehavior)GetValue(GridSyncPropertiesStore.SerializeCellsBehaviorProperty);
            }

            set
            {
                SetValue(GridSyncPropertiesStore.SerializeCellsBehaviorProperty, value);
                OnChanged("SerializeCellsBehavior");
            }
        }

        ////end grid contents
        #endregion
    }
    #endregion

    #region GridSyncPropertiesStore
    /// <copyfrom cref="Syncfusion.Styles.StyleInfoStore"/>
    [Serializable,
    StaticDataField("sd")]
    [DebuggerStepThrough()]
    internal class GridSyncPropertiesStore : StyleInfoStore
    {
        static StaticData sd = new StaticData(typeof(GridSyncPropertiesStore), typeof(GridSyncProperties), false);

        static GridSyncPropertiesStore()
        {
            FontProperty.SerializeXmlBehavior = SerializeXmlBehavior.Skip;
            DataProperty.SerializeXmlBehavior = SerializeXmlBehavior.SerializeWithXmlSerializer; ////.Skip;
        }

        internal static StaticData StaticData
        {
            get
            {
                return sd;
            }
        }

        /// <override/>
        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }

        #region Store Properties
        ////appearance

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.StoredImages"/> property.
        /// </summary>
        public readonly static StyleInfoProperty StoredImagesProperty = sd.CreateStyleInfoProperty(typeof(GridNamespaceGroupItemCollection), "StoredImages");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.StoredFonts"/> property.
        /// </summary>
        public readonly static StyleInfoProperty StoredFontsProperty = sd.CreateStyleInfoProperty(typeof(GridNamespaceGroupItemCollection), "StoredFonts");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.BackColor"/> property.
        /// </summary>
        public readonly static StyleInfoProperty BackColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "BackColor");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.BackgroundImageID"/> property.
        /// </summary>
        public readonly static StyleInfoProperty BackgroundImageIDProperty = sd.CreateStyleInfoProperty(typeof(string), "BackgroundImageID");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.BackgroundImage"/> property.
        /// </summary>
        public readonly static StyleInfoProperty BackgroundImageProperty = sd.CreateStyleInfoProperty(typeof(Image), "BackgroundImage");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.BorderStyle"/> property.
        /// </summary>
        public readonly static StyleInfoProperty BorderStyleProperty = sd.CreateStyleInfoProperty(typeof(BorderStyle), "BorderStyle");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.Font"/> property.
        /// </summary>
        public readonly static StyleInfoProperty FontProperty = sd.CreateStyleInfoProperty(typeof(Font), "Font");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.ForeColor"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ForeColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "ForeColor");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.HighlightFrozenLine"/> property.
        /// </summary>
        public readonly static StyleInfoProperty HighlightFrozenLineProperty = sd.CreateStyleInfoProperty(typeof(bool), "HighlightFrozenLine");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.RightToLeft"/> property.
        /// </summary>
        public readonly static StyleInfoProperty RightToLeftProperty = sd.CreateStyleInfoProperty(typeof(RightToLeft), "RightToLeft");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.ThemesEnabled"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ThemesEnabledProperty = sd.CreateStyleInfoProperty(typeof(bool), "ThemesEnabled");
        ////end appearance
        
        ////grid

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.ActivateCurrentCellBehavior"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ActivateCurrentCellBehaviorProperty = sd.CreateStyleInfoProperty(typeof(GridCellActivateAction), "ActivateCurrentCellBehavior");

        ///<summary>
        /// Provides information about the <see cref="GridSyncProperties.AllowColumnResizeUsingCellBoundaries"/> property.
        ///</summary>
        public readonly static StyleInfoProperty AllowColumnResizeUsingCellBoundaries = sd.CreateStyleInfoProperty(typeof(bool), "AllowColumnResizeUsingCellBoundaries");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.AllowDragSelectedCols"/> property.
        /// </summary>
        public readonly static StyleInfoProperty AllowDragSelectedColsProperty = sd.CreateStyleInfoProperty(typeof(bool), "AllowDragSelectedCols");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.AllowDragSelectedRows"/> property.
        /// </summary>
        public readonly static StyleInfoProperty AllowDragSelectedRowsProperty = sd.CreateStyleInfoProperty(typeof(bool), "AllogDragSelectedRows");

        ///<summary>
        /// Provides information about the <see cref="GridSyncProperties.AllowRowResizeUsingCellBoundaries"/> property.
        ///</summary>
        public readonly static StyleInfoProperty AllowRowResizeUsingCellBoundaries = sd.CreateStyleInfoProperty(typeof(bool), "AllowRowResizeUsingCellBoundaries");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.AllowSelection"/> property.
        /// </summary>
        public readonly static StyleInfoProperty AllowSelectionProperty = sd.CreateStyleInfoProperty(typeof(GridSelectionFlags), "AllowSelection");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.AlphaBlendSelectionColor"/> property.
        /// </summary>
        public readonly static StyleInfoProperty AlphaBlendSelectionColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "AlphaBlendSelectionColor");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.ClickedOnDisabledCellBehavior"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ClickedOnDisabledCellBehaviorProperty = sd.CreateStyleInfoProperty(typeof(GridClickedOnDisabledCellBehavior), "ClickedOnDisabledCellBehavior");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.ControllerOptions"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ControllerOptionsProperty = sd.CreateStyleInfoProperty(typeof(GridControllerOptions), "ControllerOptions");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.DataObjectConsumerOptions"/> property.
        /// </summary>
        public readonly static StyleInfoProperty DataObjectConsumerOptionsProperty = sd.CreateStyleInfoProperty(typeof(GridDataObjectConsumerOptions), "DataObjectConsumerOptions");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.DefaultGridBorderStyle"/> property.
        /// </summary>
        public readonly static StyleInfoProperty DefaultGridBorderStyleProperty = sd.CreateStyleInfoProperty(typeof(GridBorderStyle), "DefaultGridBorderStyle");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.DragSelectedCellsMouseButtonsMask"/> property.
        /// </summary>
        public readonly static StyleInfoProperty DragSelectedCellsMouseButtonsMaskProperty = sd.CreateStyleInfoProperty(typeof(MouseButtons), "DragSelectedCellsMouseButtonsMask");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.DrawOrder"/> property.
        /// </summary>
        public readonly static StyleInfoProperty DrawOrderProperty = sd.CreateStyleInfoProperty(typeof(GridDrawOrder), "DrawOrder");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.EnterKeyBehavior"/> property.
        /// </summary>
        public readonly static StyleInfoProperty EnterKeyBehaviorProperty = sd.CreateStyleInfoProperty(typeof(GridDirectionType), "EnterKeyBehavior");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.ExcelLikeCurrentCell"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ExcelLikeCurrentCellProperty = sd.CreateStyleInfoProperty(typeof(bool), "ExcelLikeCurrentCell");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.ExcelLikeSelectionFrame"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ExcelLikeSelectionFrameProperty = sd.CreateStyleInfoProperty(typeof(bool), "ExcelLikeSelectionFrame");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.FloatCellsMode"/> property.
        /// </summary>
        public readonly static StyleInfoProperty FloatCellsModeProperty = sd.CreateStyleInfoProperty(typeof(GridFloatCellsMode), "FloatCellsMode");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.GridOfficeScrollBars"/> property.
        /// </summary>
        public readonly static StyleInfoProperty GridOfficeScrollBarsProperty = sd.CreateStyleInfoProperty(typeof(OfficeScrollBars), "GridOfficeScrollBars");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.Office2007ScrollBars"/> property.
        /// </summary>
        public readonly static StyleInfoProperty Office2007ScrollBarsProperty = sd.CreateStyleInfoProperty(typeof(bool), "Office2007ScrollBars");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.Office2007ScrollBarsColorScheme"/> property.
        /// </summary>
        public readonly static StyleInfoProperty Office2007ScrollBarsColorSchemeProperty = sd.CreateStyleInfoProperty(typeof(Office2007ColorScheme), "Office2007ScrollBarsColorScheme");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.Office2007ScrollBarsColorScheme"/> property.
        /// </summary>
        public readonly static StyleInfoProperty Office2010ScrollBarsColorSchemeProperty = sd.CreateStyleInfoProperty(typeof(Office2010ColorScheme), "Office2010ScrollBarsColorScheme");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.GridVisualStyles"/> property.
        /// </summary>
        public readonly static StyleInfoProperty GridVisualStylesProperty = sd.CreateStyleInfoProperty(typeof(GridVisualStyles), "GridVisualStyles");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.UseRightToLeftCompatibleTextBox"/> property.
        /// </summary>
        public readonly static StyleInfoProperty UseRightToLeftCompatibleTextBoxProperty = sd.CreateStyleInfoProperty(typeof(bool), "UseRightToLeftCompatibleTextBox");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.FrozenRowCount"/> property.
        /// </summary>
        public readonly static StyleInfoProperty FrozenRowCountProperty = sd.CreateStyleInfoProperty(typeof(int), "FrozenRowCount");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.FrozenColCount"/> property.
        /// </summary>
        public readonly static StyleInfoProperty FrozenColCountProperty = sd.CreateStyleInfoProperty(typeof(int), "FrozenColCount");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.ListBoxSelectionMode"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ListBoxSelectionModeProperty = sd.CreateStyleInfoProperty(typeof(SelectionMode), "ListBoxSelectionMode");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.MinResizeColSize"/> property.
        /// </summary>
        public readonly static StyleInfoProperty MinResizeColSizeProperty = sd.CreateStyleInfoProperty(typeof(int), "MinResizeColSize");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.MinResizeRowSize"/> property.
        /// </summary>
        public readonly static StyleInfoProperty MinResizeRowSizeProperty = sd.CreateStyleInfoProperty(typeof(int), "MinResizeRowSize");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.NumberedColHeaders"/> property.
        /// </summary>
        public readonly static StyleInfoProperty NumberedColHeadersProperty = sd.CreateStyleInfoProperty(typeof(bool), "NumberedColHeaders");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.NumberedRowHeaders"/> property.
        /// </summary>
        public readonly static StyleInfoProperty NumberedRowHeadersProperty = sd.CreateStyleInfoProperty(typeof(bool), "NumberedRowHeaders");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.OptimizeDrawBackground"/> property.
        /// </summary>
        public readonly static StyleInfoProperty OptimizeDrawBackgroundProperty = sd.CreateStyleInfoProperty(typeof(bool), "OptimizeDrawBackground");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.OptimizeInsertRemoveCells"/> property.
        /// </summary>
        public readonly static StyleInfoProperty OptimizeInsertRemoveCellsProperty = sd.CreateStyleInfoProperty(typeof(bool), "OptimizeInsertRemoveCells");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.RefreshCurrentCellBehavior"/> property.
        /// </summary>
        public readonly static StyleInfoProperty RefreshCurrentCellBehaviorProperty = sd.CreateStyleInfoProperty(typeof(GridRefreshCurrentCellBehavior), "RefreshCurrentCellBehavior");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.ResizeColsBehavior"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ResizeColsBehaviorProperty = sd.CreateStyleInfoProperty(typeof(GridResizeCellsBehavior), "ResizeColsBehavior");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.ResizeRowsBehavior"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ResizeRowsBehaviorProperty = sd.CreateStyleInfoProperty(typeof(GridResizeCellsBehavior), "ResizeRowsBehavior");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.SelectCellsMouseButtonsMask"/> property.
        /// </summary>
        public readonly static StyleInfoProperty SelectCellsMouseButtonsMaskProperty = sd.CreateStyleInfoProperty(typeof(MouseButtons), "SelectCellsMouseButtonsMask");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.ShowCurrentCellBorderBehavior"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ShowCurrentCellBorderBehaviorProperty = sd.CreateStyleInfoProperty(typeof(GridShowCurrentCellBorder), "ShowCurrentCellBorderBehavior");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.SmoothControlResize"/> property.
        /// </summary>
        public readonly static StyleInfoProperty SmoothControlResizeProperty = sd.CreateStyleInfoProperty(typeof(bool), "SmoothControlResize");
        ////end grid

        ////grid contents

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.ColCount"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ColCountProperty = sd.CreateStyleInfoProperty(typeof(int), "ColCount");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.DefaultColWidth"/> property.
        /// </summary>
        public readonly static StyleInfoProperty DefaultColWidthProperty = sd.CreateStyleInfoProperty(typeof(int), "DefaultColWidth");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.DefaultRowHeight"/> property.
        /// </summary>
        public readonly static StyleInfoProperty DefaultRowHeightProperty = sd.CreateStyleInfoProperty(typeof(int), "DefaultRowHeight");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.Properties"/> property.
        /// </summary>
        public readonly static StyleInfoProperty PropertiesProperty = sd.CreateStyleInfoProperty(typeof(GridProperties), "Properties");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.RowCount"/> property.
        /// </summary>
        public readonly static StyleInfoProperty RowCountProperty = sd.CreateStyleInfoProperty(typeof(int), "RowCount");

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.SerializeCellsBehavior"/> property.
        /// </summary>
        public readonly static StyleInfoProperty SerializeCellsBehaviorProperty = sd.CreateStyleInfoProperty(typeof(GridSerializeCellsBehavior), "SerializeCellsBehavior");
        ////end grid contents

        /// <summary>
        /// Provides information about the <see cref="GridSyncProperties.Cells"/> property.
        /// </summary>
        public readonly static StyleInfoProperty DataProperty = sd.CreateStyleInfoProperty(typeof(GridCellsMemento), "Cells");
        #endregion
    }
    #endregion

    #region "'PropertyInfoCollection' strongly typed collection class"

    /// <summary>
    ///     A collection that stores 'PropertyInfo' objects.
    /// </summary>
    [Serializable()]
    internal class PropertyInfoCollection : System.Collections.CollectionBase
    {
        /// <summary>
        ///     Initializes a new instance of 'PropertyInfoCollection'.
        /// </summary>
        public PropertyInfoCollection()
        {
        }

        /// <summary>
        ///     Initializes a new instance of 'PropertyInfoCollection' based on an already existing instance.
        /// </summary>
        /// <param name='proValue'>
        ///     A 'PropertyInfoCollection' from which the contents is copied
        /// </param>
        public PropertyInfoCollection(PropertyInfoCollection proValue)
        {
            this.AddRange(proValue);
        }

        /// <summary>
        ///     Initializes a new instance of 'PropertyInfoCollection' with an array of 'PropertyInfo' objects.
        /// </summary>
        /// <param name='proValue'>
        ///     An array of 'PropertyInfo' objects with which to initialize the collection
        /// </param>
        public PropertyInfoCollection(PropertyInfo[] proValue)
        {
            this.AddRange(proValue);
        }

        /// <summary>
        ///     Represents the 'PropertyInfo' item at the specified index position.
        /// </summary>
        /// <param name='intIndex'>
        ///     The zero-based index of the entry to locate in the collection.
        /// </param>
        /// <value>
        ///     The entry at the specified index of the collection.
        /// </value>
        public PropertyInfo this[int intIndex]
        {
            get
            {
                return (PropertyInfo)List[intIndex];
            }

            set
            {
                List[intIndex] = value;
            }
        }

        /// <summary>
        ///     Adds a 'PropertyInfo' item with the specified value to the 'PropertyInfoCollection'
        /// </summary>
        /// <param name='proValue'>
        ///     The 'PropertyInfo' to add.
        /// </param>
        /// <returns>
        ///     The index at which the new element was inserted.
        /// </returns>
        public int Add(PropertyInfo proValue)
        {
            return List.Add(proValue);
        }

        /// <summary>
        ///     Copies the elements of an array at the end of this instance of 'PropertyInfoCollection'.
        /// </summary>
        /// <param name='proValue'>
        ///     An array of 'PropertyInfo' objects to add to the collection.
        /// </param>
        public void AddRange(PropertyInfo[] proValue)
        {
            for (int intCounter = 0; intCounter < proValue.Length; intCounter = intCounter + 1)
            {
                this.Add(proValue[intCounter]);
            }
        }

        /// <summary>
        ///     Adds the contents of another 'PropertyInfoCollection' at the end of this instance.
        /// </summary>
        /// <param name='proValue'>
        ///     A 'PropertyInfoCollection' containing the objects to add to the collection.
        /// </param>
        public void AddRange(PropertyInfoCollection proValue)
        {
            for (int intCounter = 0; intCounter < proValue.Count; intCounter = intCounter + 1)
            {
                this.Add(proValue[intCounter]);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the 'PropertyInfoCollection' contains the specified value.
        /// </summary>
        /// <param name='proValue'>
        ///     The item to locate.
        /// </param>
        /// <returns>
        ///     True if the item exists in the collection; false otherwise.
        /// </returns>
        public bool Contains(PropertyInfo proValue)
        {
            return List.Contains(proValue);
        }

        /// <summary>
        ///     Copies the 'PropertyInfoCollection' values to a one-dimensional System.Array
        ///     instance starting at the specified array index.
        /// </summary>
        /// <param name='proArray'>
        ///     The one-dimensional System.Array that represents the copy destination.
        /// </param>
        /// <param name='intIndex'>
        ///     The index in the array where copying begins.
        /// </param>
        public void CopyTo(PropertyInfo[] proArray, int intIndex)
        {
            List.CopyTo(proArray, intIndex);
        }

        /// <summary>
        ///     Returns the index of a 'PropertyInfo' object in the collection.
        /// </summary>
        /// <param name='proValue'>
        ///     The 'PropertyInfo' object whose index will be retrieved.
        /// </param>
        /// <returns>
        ///     If found, the index of the value; otherwise, -1.
        /// </returns>
        public int IndexOf(PropertyInfo proValue)
        {
            return List.IndexOf(proValue);
        }

        public PropertyInfo FindByName(string propertyName)
        {
            PropertyInfoCollection.PropertyInfoEnumerator ienum = this.GetEnumerator();
            while (ienum.MoveNext())
            {
                if (ienum.Current.Name == propertyName)
                {
                    return ienum.Current;
                }
            }

            return null;
        }

        /// <summary>
        ///     Inserts an existing 'PropertyInfo' into the collection at the specified index.
        /// </summary>
        /// <param name='intIndex'>
        ///     The zero-based index where the new item should be inserted.
        /// </param>
        /// <param name='proValue'>
        ///     The item to insert.
        /// </param>
        public void Insert(int intIndex, PropertyInfo proValue)
        {
            List.Insert(intIndex, proValue);
        }

        /// <summary>
        /// Returns an enumerator that can be used to iterate through
        /// the 'PropertyInfoCollection'.
        /// </summary>
        /// <returns>returns PropertyInfoEnumerato</returns>
        public new PropertyInfoEnumerator GetEnumerator()
        {
            return new PropertyInfoEnumerator(this);
        }

        /// <summary>
        ///     Removes a specific item from the 'PropertyInfoCollection'.
        /// </summary>
        /// <param name='proValue'>
        ///     The item to remove from the 'PropertyInfoCollection'.
        /// </param>
        public void Remove(PropertyInfo proValue)
        {
            List.Remove(proValue);
        }

        /// <summary>
        ///     TODO: Describe what custom processing this method does
        ///     before setting an item in the collection
        /// </summary>
        protected override void OnSet(int intIndex, object objOldValue, object objNewValue)
        {
            ////  TODO: Add code here to handle an existing value within
            ////  the collection be replaced with a new value
        }

        /// <summary>
        ///     TODO: Describe what custom processing this method does
        ///     before inserting a new item in the collection
        /// </summary>
        protected override void OnInsert(int intIndex, object objValue)
        {
            ////  TODO: Add code here to handle inserting a new item into the collection
        }

        /// <summary>
        ///     A strongly typed enumerator for 'PropertyInfoCollection'
        /// </summary>
        internal class PropertyInfoEnumerator : object, System.Collections.IEnumerator
        {
            private System.Collections.IEnumerator iEnBase;

            private System.Collections.IEnumerable iEnLocal;

            /// <summary>
            ///     Enumerator constructor
            /// </summary>
            public PropertyInfoEnumerator(PropertyInfoCollection proMappings)
            {
                this.iEnLocal = (System.Collections.IEnumerable)proMappings;
                this.iEnBase = iEnLocal.GetEnumerator();
            }

            /// <summary>
            ///     Gets the current element from the collection (strongly typed)
            /// </summary>
            public PropertyInfo Current
            {
                get
                {
                    return (PropertyInfo)iEnBase.Current;
                }
            }

            /// <summary>
            ///     Gets the current element from the collection
            /// </summary>
            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return iEnBase.Current;
                }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
            public bool MoveNext()
            {
                return iEnBase.MoveNext();
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
            bool System.Collections.IEnumerator.MoveNext()
            {
                return iEnBase.MoveNext();
            }

            /// <summary>
            ///     Sets the enumerator to the first element in the collection
            /// </summary>
            public void Reset()
            {
                iEnBase.Reset();
            }

            /// <summary>
            ///     Sets the enumerator to the first element in the collection
            /// </summary>
            void System.Collections.IEnumerator.Reset()
            {
                iEnBase.Reset();
            }
        }
    }

    #endregion //('PropertyInfoCollection' strongly typed collection class)
}
