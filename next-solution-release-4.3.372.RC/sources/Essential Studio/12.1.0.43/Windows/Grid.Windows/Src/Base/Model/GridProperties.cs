//-------------------------------------------------------------------------------------------------
// <copyright file="GridProperties.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;
using System.Xml;
using System.Xml.Serialization;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Styles;

namespace Syncfusion.Windows.Forms.Grid
{
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class GridPropertyColorIndex
    {
        public const int GridLines = 0;
        public const int FixedLines = 1;
        public const int ResizingCellsLines = 2;
        public const int DraggingLines = 3;
        public const int Background = 4;
    }

    /// <summary>
    /// GridProperties holds various options that let you customize the appearance of the grid,
    /// such as window background, grid line colors, printer / page settings, and more.
    /// </summary>
    [Serializable,
    TypeConverter(typeof(ExpandableObjectConverter))]
    public class GridProperties : Disposable, ISerializable
    {
        internal bool modified = false;
        internal int version = 0;
        private bool forceImmediateRepaint = false;
        /// <summary>
        /// Initializes a new <see cref="GridProperties"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridProperties(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            //info.AddValue("Version", 14);
            m_ColorNames = new string[]
            {
                "GRID_IDS_COLOR_GRIDLINES", 
                "GRID_IDS_COLOR_FIXEDLINES", 
                "GRID_IDS_COLOR_TRACKINGLINE",
                "GRID_IDS_COLOR_DRAGGINGLINE",
                "GRID_IDS_COLOR_BACKGROUND", 
            };
            try
            {
                version = info.GetInt32("Version");
            }
            catch (SerializationException ex)
            {
                //// This exception is expected for older file versions where "Version" was not
                //// yet implemented.
                //// Should be: "Additional information: Member Version was not found."
#if DEBUG
                Trace.WriteLine("Update the serialization file. " + ex.Message);
#endif
            }

            ////            m_sSection               = szDefaultSection;

            ////            m_UserPropertyInfoMap = (Hashtable) info.GetValue("UserData", typeof(Hashtable));
            m_b3dButtons = info.GetBoolean("Display3DButtons");
            m_bDisplayVertLines = info.GetBoolean("DisplayVerticalLines");
            m_bDisplayHorzLines = info.GetBoolean("DisplayHorizontalLines");
            m_bMarkRowHeader = info.GetBoolean("MarkRowHeader");
            m_bMarkColHeader = info.GetBoolean("MarkColHeader");
            ////            m_sSection = info.GetString("Section");
            m_ColorTable = (Color[])info.GetValue("Colors", typeof(Color[]));
            m_nZoom = info.GetInt32("Zoom");
            m_nBottomMargin = info.GetInt32("BottomMargin");
            m_nLeftMargin = info.GetInt32("LeftMargin");
            m_nTopMargin = info.GetInt32("TopMargin");
            m_nRightMargin = info.GetInt32("RightMargin");
            m_bCenterHorizontal = info.GetBoolean("CenterHorizontal");
            m_bCenterVertical = info.GetBoolean("CenterVertical");
            m_bPrintRowHeader = info.GetBoolean("PrintRowHeader");
            m_bPrintColHeader = info.GetBoolean("PrintColHeader");
            m_bColHeaders = info.GetBoolean("ColHeaders");
            m_bPrintHorzLines = info.GetBoolean("PrintHorzLines");
            m_bRowHeaders = info.GetBoolean("RowHeaders");
            m_bPrintVertLines = info.GetBoolean("PrintVertLines");
            m_nPageOrder = info.GetInt32("PageOrder");
            m_bBlackWhite = info.GetBoolean("BlackWhite");
            if (version >= 14)
                m_bThemedHeader = info.GetBoolean("ThemedHeader");
            m_bPrintFrame = info.GetBoolean("PrintFrame");
            m_nDistTop = info.GetInt32("DistTop");
            m_nDistBottom = info.GetInt32("DistBottom");
            m_nFirstPage = info.GetInt32("FirstPage");
            ////            m_mapDataFooter = (GridData) info.GetValue("Footer", typeof(GridData));
            ////            m_mapDataHeader = (GridData) info.GetValue("Header", typeof(GridData));
            ////            resizingCellsLinesBorder = null;
            ////            resizingCellsBoundsBorder = null;

            bool mod = true;
            SerializationInfoEnumerator sie = info.GetEnumerator();
            while (sie.MoveNext())
            {
                if (sie.Name == "Modified")
                {
                    mod = Convert.ToBoolean(sie.Value);
                }
            }

            this.modified = mod;
        }

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the <see cref="GridProperties"/>.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, SerializationFormatter = true)]
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
            info.AddValue("Version", 14);
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
            ////            info.AddValue("ArrayList", m_params);
            ////            info.AddValue("UserData", m_UserPropertyInfoMap); // Hashtable
            info.AddValue("Display3DButtons", m_b3dButtons); // Boolean
            info.AddValue("DisplayVerticalLines", m_bDisplayVertLines); // Boolean
            info.AddValue("DisplayHorizontalLines", m_bDisplayHorzLines); // Boolean
            info.AddValue("MarkRowHeader", m_bMarkRowHeader); // Boolean
            info.AddValue("MarkColHeader", m_bMarkColHeader); // Boolean
            //            info.AddValue("Section", m_sSection); // String
            info.AddValue("Colors", m_ColorTable); // Color[]
            info.AddValue("Zoom", m_nZoom); // Int32
            info.AddValue("BottomMargin", m_nBottomMargin); // Int32
            info.AddValue("LeftMargin", m_nLeftMargin); // Int32
            info.AddValue("TopMargin", m_nTopMargin); // Int32
            info.AddValue("RightMargin", m_nRightMargin); // Int32
            info.AddValue("CenterHorizontal", m_bCenterHorizontal); // Boolean
            info.AddValue("CenterVertical", m_bCenterVertical); // Boolean
            info.AddValue("PrintRowHeader", m_bPrintRowHeader); // Boolean
            info.AddValue("PrintColHeader", m_bPrintColHeader); // Boolean
            info.AddValue("ColHeaders", m_bColHeaders); // Boolean
            info.AddValue("PrintHorzLines", m_bPrintHorzLines); // Boolean
            info.AddValue("RowHeaders", m_bRowHeaders); // Boolean
            info.AddValue("PrintVertLines", m_bPrintVertLines); // Boolean
            info.AddValue("PageOrder", m_nPageOrder); // Int32
            info.AddValue("BlackWhite", m_bBlackWhite); // Boolean
            if (version >= 14)
                info.AddValue("ThemedHeader", m_bThemedHeader); // Boolean
            info.AddValue("PrintFrame", m_bPrintFrame); // Boolean
            info.AddValue("DistTop", m_nDistTop); // Int32
            info.AddValue("DistBottom", m_nDistBottom); // Int32
            info.AddValue("FirstPage", m_nFirstPage); // Int32
            //            info.AddValue("Footer", m_mapDataFooter); // GridData
            //            info.AddValue("Header", m_mapDataHeader); // GridData
            info.AddValue("Modified", modified);
        }

        /// <summary>
        /// Initializes a new GridProperties object.
        /// </summary>
        public GridProperties()
        {
            // Default settings
            m_bDisplayVertLines = true;
            m_bDisplayHorzLines = true;
            m_b3dButtons = true;
            m_bMarkRowHeader = false;
            m_bMarkColHeader = false;
            m_bPrinting = false;
            ////            m_sSection               = szDefaultSection;
            m_nZoom = 100;

            //// Colors
            m_ColorNames = new string[]
            {
                "GRID_IDS_COLOR_GRIDLINES", 
                "GRID_IDS_COLOR_FIXEDLINES", 
                "GRID_IDS_COLOR_TRACKINGLINE",
                "GRID_IDS_COLOR_DRAGGINGLINE",
                "GRID_IDS_COLOR_BACKGROUND", 
            };

            m_ColorTable = new Color[]
            {
                /*GRID_IDS_COLOR_GRIDLINES*/ SystemColors.GrayText,
                /*GRID_IDS_COLOR_FIXEDLINES*/ SystemColors.ActiveCaption,
                /*GRID_IDS_COLOR_TRACKINGLINE*/ Color.Red,
                /*GRID_IDS_COLOR_DRAGGINGLINE*/ Color.Red,
                /*GRID_IDS_COLOR_BACKGROUND*/ SystemColors.Control
            };

            // Print settings.
            m_bPrintVertLines = true;
            m_bPrintHorzLines = true;
            m_bBlackWhite = false;
            m_bThemedHeader = false;
            m_bRowHeaders = true;
            m_bColHeaders = true;
            m_bPrintRowHeader = true;
            m_bPrintColHeader = true;
            m_bPrintFrame = true;

            m_bCenterVertical = false;
            m_bCenterHorizontal = true;
            m_nPageOrder = 0;

            m_nLeftMargin = GridFontState.INCHtoDP(1);
            m_nRightMargin = GridFontState.INCHtoDP(1);
            m_nTopMargin = GridFontState.INCHtoDP(1.5f);
            m_nBottomMargin = GridFontState.INCHtoDP(1.5f);

            // Header&Footer
            m_nDistTop = GridFontState.INCHtoDP(0.4f);
            m_nDistBottom = GridFontState.INCHtoDP(0.4f);
            m_nFirstPage = -1;
        }

        /// <summary>
        /// Gets a value indicating whether the dictionary was modified.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Modified
        {
            get
            {
                return modified;
            }
        }

        /// <summary>
        /// Resets the <see cref="Modified"/> flag.
        /// </summary>
        public void ResetModified()
        {
            modified = false;
        }
        
        /// <summary>
        /// Occurs when any property in this object is changed.
        /// </summary>
        [Description("Occurs when any property in this object is changed.")] 
        public event EventHandler Changed;

        void OnChanged()
        {
#if DEBUG
            if (Switches.General.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            modified = true;
            if (Changed != null)
            {
                Changed(this, EventArgs.Empty);
            }
        }

        ////        ///// <summary>
        ////        ///// Creates a copy of the current object.
        ////        ///// </summary>
        ////        ///// <returns>A <see cref="GridProperties"/> with same data as the current object.</returns>
        ////        public GridProperties Clone()
        ////        {
        ////            GridProperties clone = new GridProperties();
        ////            return clone;
        ////        }
        ////
        ////        object ICloneable.Clone()
        ////        {
        ////            GridTraceStack.TraceMethodInfo();
        ////            return this.Clone();
        ////        }

        internal const string szMarkRowHeader = "MarkRowHeader";
        internal const string szMarkColHeader = "MarkColumnHeader";
        internal const string szDisplay3DButtons = "3dHeader";
        internal const string szDisplayVertLines = "VerticalLines";
        internal const string szDisplayHorzLines = "HorizontalLines";
        internal const string szDisplayRowHeader = "DisplayRowHeader";
        internal const string szDisplayColHeader = "DisplayColumnHeader";

        internal const string szDefaultSection = "Grid";

        //// Printing.
        internal const string szPrintVertLines = "Print Vertical Lines";
        internal const string szPrintHorzLines = "Print Horizontal Lines";
        internal const string szBlackWhite = "Print Only Black&White";
        internal const string szThemedHeader = "Print header with theme";
        internal const string szRowHeaders = "Print Row Headers";
        internal const string szColHeaders = "Print Column Headers";
        internal const string szCenterVertical = "Center Vertical";
        internal const string szCenterHorizontal = "Center Horizontal";
        internal const string szPageOrder = "Page Order";
        internal const string szPrintFrame = "Margins";
        internal const string szMargins = "Print Frame";

        internal const string szFirstPage = "Start Page Numbering";
        internal const string szDistances = "Distances";

        //// Current cell.
        string _sInvertNormal;
        string _sInvertThick;
        string _sInvertDrawBorder;
        ////        string _sInvertThickBorder;
        string _sInvertNoBorder;

        internal string sInvertNormal
        {
            get
            {
                if (_sInvertNormal == null)
                {
                    _sInvertNormal = SR.GetString("GRID_IDS_INVERTNORMAL");
                }

                return _sInvertNormal;
            }
        }

        internal string sInvertThick
        {
            get
            {
                if (_sInvertThick == null)
                {
                    _sInvertThick = SR.GetString("GRID_IDS_INVERTTHICK");
                }

                return _sInvertThick;
            }
        }

        internal string sInvertDrawBorder
        {
            get
            {
                if (_sInvertDrawBorder == null)
                {
                    _sInvertDrawBorder = SR.GetString("GRID_IDS_INVERTDRAWBORDER");
                }

                return _sInvertDrawBorder;
            }
        }

        internal string sInvertThickBorder
        {
            get
            {
                if (_sInvertDrawBorder == null)
                {
                    _sInvertDrawBorder = SR.GetString("GRID_IDS_INVERTTHICKBORDER");
                }

                return _sInvertDrawBorder;
            }
        }

        internal string sInvertNoBorder
        {
            get
            {
                if (_sInvertNoBorder == null)
                {
                    _sInvertNoBorder = SR.GetString("GRID_IDS_INVERTNOBORDER");
                }

                return _sInvertNoBorder;
            }
        }

        //// Attributes.
        private bool m_b3dButtons;           // 3d look alike Headers
        private bool m_bDisplayVertLines;    // vertical gridlines
        private bool m_bDisplayHorzLines;    // horinzontal gridlines
        private bool m_bMarkRowHeader;       // mark row headers
        private bool m_bMarkColHeader;       // mark column headers

        private bool m_bPrinting;

        ////        private string  m_sSection;

        //// Colors.
        string[] m_ColorNames;
        private Color[] m_ColorTable;

        private int m_nZoom;

        // Print settings.
        private int m_nBottomMargin;
        private int m_nLeftMargin;
        private int m_nTopMargin;
        private int m_nRightMargin;
        private bool m_bCenterHorizontal;
        private bool m_bCenterVertical;
        private bool m_bPrintRowHeader;
        private bool m_bPrintColHeader;
        private bool m_bColHeaders;
        private bool m_bPrintHorzLines;
        private bool m_bRowHeaders;
        private bool m_bPrintVertLines;
        private int m_nPageOrder;
        private bool m_bBlackWhite;
        private bool m_bThemedHeader;
        private bool m_bPrintFrame;

        // Header&Footer.
        private int m_nDistTop;
        private int m_nDistBottom;
        private int m_nFirstPage;

        // Attributes.

        /// <summary>
        /// Gets or sets a value indicating whether row and column headers should appear raised or flat.
        /// </summary>
        [Description("Specifies if row and column headers should appear raised or flat.")]
        [Browsable(true), DefaultValue(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public bool Buttons3D
        {
            get
            {
                return m_b3dButtons;
            }

            set
            {
                if (m_b3dButtons != value)
                {
                    m_b3dButtons = value;
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether vertical lines should be displayed.
        /// </summary>
        [Description("Specifies if vertical lines should be displayed.")]
        [Browsable(true), DefaultValue(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public bool DisplayVertLines
        {
            get
            {
                return m_bDisplayVertLines;
            }

            set
            {
                if (m_bDisplayVertLines != value)
                {
                    m_bDisplayVertLines = value;
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether horizontal lines should be displayed.
        /// </summary>
        [Description("Specifies if horizontal lines should be displayed.")]
        [Browsable(true), DefaultValue(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public bool DisplayHorzLines
        {
            get
            {
                return m_bDisplayHorzLines;
            }

            set
            {
                if (m_bDisplayHorzLines != value)
                {
                    m_bDisplayHorzLines = value;
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the row header for the current cell should be highlighted.
        /// </summary>
        [Description("Specifies if the row header for the current cell should be highlighted.")]
        [Browsable(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public bool MarkRowHeader
        {
            get
            {
                return m_bMarkRowHeader;
            }

            set
            {
                if (m_bMarkRowHeader != value)
                {
                    m_bMarkRowHeader = value;
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the column header for the current cell should be highlighted.
        /// </summary>
        [Description("Specifies if the column header for the current cell should be highlighted.")]
        [Browsable(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public bool MarkColHeader
        {
            get
            {
                return m_bMarkColHeader;
            }

            set
            {
                if (m_bMarkColHeader != value)
                {
                    m_bMarkColHeader = value;
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether row headers should be printed when printing the grid.
        /// </summary>
        [Description("Specifies if row headers should be printed when printing the grid.")]
        [Category("Grid")]
        [Browsable(true), DefaultValue(true)]
        public bool PrintRowHeader
        {
            get
            {
                return m_bPrintRowHeader;
            }

            set
            {
                if (m_bPrintRowHeader != value)
                {
                    m_bPrintRowHeader = value;
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether column headers should be printed when printing the grid.
        /// </summary>
        [Description("Specifies if column headers should be printed when printing the grid.")]
        [Category("Grid")]
        [Browsable(true), DefaultValue(true)]
        public bool PrintColHeader
        {
            get
            {
                return m_bPrintColHeader;
            }

            set
            {
                if (m_bPrintColHeader != value)
                {
                    m_bPrintColHeader = value;
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether switch to printing mode for the grid.
        /// </summary>
        [Description("Switches printing mode for the grid.")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Printing
        {
            get
            {
                return m_bPrinting;
            }

            set
            {
                m_bPrinting = value;
            }
        }

        ////        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        ////        public string  Section
        ////        {
        ////            get 
        ////            {
        ////                return m_sSection;
        ////            }
        ////            set
        ////            {
        ////                m_sSection = value;
        ////            }
        ////        }

        // colors
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal Color[] Colors
        {
            get
            {
                return m_ColorTable;
            }
        }

        ////        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        ////        public int Zoom
        ////        {
        ////            get 
        ////            {
        ////                return m_nZoom;
        ////            }
        ////            set
        ////            {
        ////                m_nZoom = value;
        ////            }
        ////        }

        // print settings

        /// <summary>
        /// Gets or sets a value indicating whether the grid should be centered horizontally on the page when printing.
        /// </summary>
        [Description("Specifies if the grid should be centered horizontally on the page when printing.")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CenterHorizontal
        {
            get
            {
                return m_bCenterHorizontal;
            }

            set
            {
                if (m_bCenterHorizontal != value)
                {
                    m_bCenterHorizontal = value;
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the grid should be centered vertically on the page when printing.
        /// </summary>
        [Description("Specifies if the grid should be centered vertically on the page when printing.")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CenterVertical
        {
            get
            {
                return m_bCenterVertical;
            }

            set
            {
                if (m_bCenterVertical != value)
                {
                    m_bCenterVertical = value;
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the grid should be display column headers.
        /// </summary>
        [Description("Specifies if column headers should be displayed or hidden.")]
        [Browsable(true), DefaultValue(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public bool ColHeaders
        {
            get
            {
                return Printing ? this.m_bPrintColHeader : m_bColHeaders;
            }

            set
            {
                if (m_bColHeaders != value)
                {
                    m_bColHeaders = value;
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the grid should draw horizontal lines when printing.
        /// </summary>
        [Description("Specifies if the grid should draw horizontal lines when printing.")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool PrintHorzLines
        {
            get
            {
                return m_bPrintHorzLines;
            }

            set
            {
                if (m_bPrintHorzLines != value)
                {
                    m_bPrintHorzLines = value;
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether row headers should be displayed or hidden. (Might be better to use HideCols[0] = false) instead.
        /// </summary>
        [Description("Specifies if row headers should be displayed or hidden.")]
        [Browsable(true), DefaultValue(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public bool RowHeaders
        {
            get
            {
                return Printing ? this.m_bPrintRowHeader : m_bRowHeaders;
            }

            set
            {
                if (m_bRowHeaders != value)
                {
                    m_bRowHeaders = value;
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the grid should draw vertical lines when printing.
        /// </summary>
        [Description("Specifies if the grid should draw vertical lines when printing.")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool PrintVertLines
        {
            get
            {
                return m_bPrintVertLines;
            }

            set
            {
                if (m_bPrintVertLines != value)
                {
                    m_bPrintVertLines = value;
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Forces Immediate repaint of Client Area. 
        /// </summary>
        public bool ForceImmediateRepaint
        {
            get
            {
                return forceImmediateRepaint;
            }

            set
            {
                forceImmediateRepaint = value;
            }
        }
        /// <summary>
        /// Gets or sets the page order how the grid should be printed.
        /// </summary>
        [Description("Specifies the page order how the grid should be printed.")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int PageOrder
        {
            get
            {
                return m_nPageOrder;
            }

            set
            {
                if (m_nPageOrder != value)
                {
                    m_nPageOrder = value;
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the grid should print only in black and white.
        /// </summary>
        [Description("Specifies if the grid should print only in black and white.")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public bool BlackWhite
        {
            get
            {
                return m_bBlackWhite;
            }

            set
            {
                if (m_bBlackWhite != value)
                {
                    m_bBlackWhite = value;
                    OnChanged();
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the grid should print the header with theme
        /// </summary>
        [DefaultValue(false)]
        [Description("Specifies if the grid should print the header with theme")]
        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ThemedHeader
        {
            get
            {
                return m_bThemedHeader;
            }
            set
            {
                if (m_bThemedHeader != value)
                {
                    m_bThemedHeader = value;
                    OnChanged();
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether a frame should be drawn around the grid when printing.
        /// </summary>
        [Description("Specifies if a frame should be drawn around the grid when printing.")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public bool PrintFrame
        {
            get
            {
                return m_bPrintFrame;
            }

            set
            {
                if (m_bPrintFrame != value)
                {
                    m_bPrintFrame = value;
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of grid lines.
        /// </summary>
        [Description("The color of grid lines.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlIgnore]
        public Color GridLineColor
        {
            get
            {
                return m_ColorTable[GridPropertyColorIndex.GridLines];
            }

            set
            {
                if (m_ColorTable[GridPropertyColorIndex.GridLines] != value)
                {
                    m_ColorTable[GridPropertyColorIndex.GridLines] = value;
                    OnChanged();
                }
            }
        }

        /// <summary>
        ///   Gets or sets GridLine Color String. Internal only
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string GridLineColorString
        {
            get
            {
                return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, GridLineColor);
            }

            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                try
                {
                    GridLineColor = (Color)tc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    GridLineColor = (Color)tc.ConvertFromString(value);
                }
            }
        }

        bool ShouldSerializeGridLineColor()
        {
            return GridLineColor != SystemColors.GrayText;
        }

        /// <summary>
        /// Resets GridLineColor to its default value.
        /// </summary>
        public void ResetGridLineColor()
        {
            GridLineColor = SystemColors.GrayText;
        }

        /// <summary>
        /// Gets or sets the color of frozen grid lines.
        /// </summary>
        [Description("The color of frozen grid lines.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlIgnore]
        public Color FixedLinesColor
        {
            get
            {
                return m_ColorTable[GridPropertyColorIndex.FixedLines];
            }

            set
            {
                if (m_ColorTable[GridPropertyColorIndex.FixedLines] != value)
                {
                    m_ColorTable[GridPropertyColorIndex.FixedLines] = value;
                    OnChanged();
                }
            }
        }

        /// <summary>
        ///  Gets or sets Fixed Lines ColorString. Internal only.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FixedLinesColorString
        {
            get
            {
                return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, FixedLinesColor);
            }

            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                try
                {
                    FixedLinesColor = (Color)tc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    FixedLinesColor = (Color)tc.ConvertFromString(value);
                }
            }
        }

        bool ShouldSerializeFixedLinesColor()
        {
            return FixedLinesColor != SystemColors.ActiveCaption;
        }

        /// <summary>
        /// Resets FixedLinesColor to its default value.
        /// </summary>
        public void ResetFixedLinesColor()
        {
            FixedLinesColor = SystemColors.ActiveCaption;
        }

        /// <summary>
        /// Gets or sets the color of the grid line marker when the user is resizing rows or columns.
        /// </summary>
        [Description("Gets or sets the color of the grid line marker when the user is resizing rows or columns.")]
        [XmlIgnore]
        public Color ResizingCellsLinesColor
        {
            get
            {
                return m_ColorTable[GridPropertyColorIndex.ResizingCellsLines];
            }

            set
            {
                m_ColorTable[GridPropertyColorIndex.ResizingCellsLines] = value;
                ////resizingCellsLinesBorder = new GridBorder(GridBorderStyle.Dashed, value, GridBorderWeight.Thick);
            }
        }

        /// <summary>
        ///  Gets or sets ResizingCells Lines ColorString. Internal only.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ResizingCellsLinesColorString
        {
            get
            {
                return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, ResizingCellsLinesColor);
            }

            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                try
                {
                    ResizingCellsLinesColor = (Color)tc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    ResizingCellsLinesColor = (Color)tc.ConvertFromString(value);
                }
            }
        }

        bool ShouldSerializeResizingCellsLinesColor()
        {
            return ResizingCellsLinesColor != Color.Red;
        }

        /// <summary>
        /// Resets ResizingCellsLinesColor to its default value.
        /// </summary>
        public void ResetResizingCellsLinesColor()
        {
            ResizingCellsLinesColor = Color.Red;
        }

        //
        //        /// <summary>
        //        /// Later.
        //        /// </summary>
        //        internal Color DraggingLinesColor
        //        {
        //            get
        //            {
        //                return m_ColorTable[GridPropertyColorIndex.DraggingLines];
        //            }
        //            set
        //            {
        //                m_ColorTable[GridPropertyColorIndex.DraggingLines] = value;
        //            }
        //        }
        //        bool ShouldSerializeDraggingLinesColor()
        //        {
        //            return DraggingLinesColor != Color.Red;
        //        }
        //        /// <summary>
        //        /// Resets DraggingLinesColor to its default value.
        //        /// </summary>
        //        public void ResetDraggingLinesColor()
        //        {
        //            DraggingLinesColor = Color.Red;
        //        }
        //

        /// <summary>
        /// Gets or sets the color of the area below the last row and right of the last column inside the grid window.
        /// </summary>
        [Description("The color of the area below the last row and right of the last column inside the grid window.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlIgnore]
        public Color BackgroundColor
        {
            get
            {
                return m_ColorTable[GridPropertyColorIndex.Background];
            }

            set
            {
                if (m_ColorTable[GridPropertyColorIndex.Background] != value)
                {
                    m_ColorTable[GridPropertyColorIndex.Background] = value;
                    OnChanged();
                }
            }
        }

        private bool allowHiddenCellFloating = false;
        /// <summary>
        /// Gets / sets the values to enable floating over hidden cells
        /// </summary>
        [Category("Appearance"),
        Browsable(true),
        Description("Supports floating over hidden cells."),
        DefaultValue(false)]
        public bool AllowHiddenCellFloating
        {
            get
            {
                return allowHiddenCellFloating;
            }
            set
            {
                allowHiddenCellFloating = value;
            }
        }

        /// <summary>
        ///  Gets or sets Background ColorString. Internal only.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string BackgroundColorString
        {
            get
            {
                return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, BackgroundColor);
            }

            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                try
                {
                    BackgroundColor = (Color)tc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    BackgroundColor = (Color)tc.ConvertFromString(value);
                }
            }
        }

        bool ShouldSerializeBackgroundColor()
        {
            return BackgroundColor != SystemColors.Control;
        }

        /// <summary>
        /// Resets BackgroundColor to its default value.
        /// </summary>
        public void ResetBackgroundColor()
        {
            BackgroundColor = SystemColors.Control;
        }

        /// <summary>
        ///     Creates a deep copy of the <see cref="GridProperties"/> object
        /// </summary>
        /// <returns>A duplicate of the current object.</returns>
        /// <remarks>
        ///     This will copy the values of the <see cref="GridProperties"/> object, ignoring the <see cref="GridModel"/> references.
        /// </remarks>
        public GridProperties Clone()
        {
            GridProperties clone = new GridProperties();

            clone.CopyPropertiesFrom(this);
            return clone;
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal void CopyPropertiesFrom(GridProperties properties)
        {
            PropertyInfo[] props = GetType().GetProperties();
            foreach (PropertyInfo property in props)
            {
                AttributeCollection attribs = TypeDescriptor.GetProperties(this)[property.Name].Attributes;
                DesignerSerializationVisibilityAttribute visibleAttrib = (DesignerSerializationVisibilityAttribute)attribs[typeof(DesignerSerializationVisibilityAttribute)];
                if (visibleAttrib != null)
                {
                    if (visibleAttrib.Visibility == DesignerSerializationVisibility.Hidden)
                    {
                        continue;
                    }
                }

                object value = property.GetValue(properties, new object[0]);
                property.SetValue(this, value, new object[0]);
            }
        }
    }
}
