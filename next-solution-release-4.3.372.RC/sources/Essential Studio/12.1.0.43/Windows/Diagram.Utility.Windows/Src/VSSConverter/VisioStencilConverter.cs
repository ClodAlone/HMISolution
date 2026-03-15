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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Office.Interop.Visio;
using Syncfusion.Runtime.InteropServices.WinAPI;
using Syncfusion.Windows.Forms.Diagram;
using Application = System.Windows.Forms.Application;
using Color = System.Drawing.Color;
using Path = System.IO.Path;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Visio stencil converter is used to convert Visio stencils (*.vss)
    /// into diagram palette files (*edp) and load into palette GroupBar control.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class VisioStencilConvert : IDisposable
    {
        #region Fields

        #region Constants
        private const string c_strBITMAP = "Bitmap";
        private const string c_strMETAFILE = "Metafile";
        private const string c_strENHMETAFILE = "EnhMetaFile";
        private const int c_nHEXADECIMAL = 16;
        private const int c_nHTML_RGB_LENGTH = 7;
        private const short c_VISIO_SHAPE_TYPE_GROUP = 2;
        private const short c_sBBoxUprightWH = 1;
        private const string c_strCONNECTION = "connection";
        private const int c_nROW_TYPE_MOVE_TO = 138;
        private const int c_nROW_TYPE_INVALID = -1;
        private const int c_nVISIO_SECTION_FIRST_COMPONENT = 10;
        private const int c_nVISIO_ROW_VERTEX = 1;
        private const int c_nVISIO_COMP_NO_SHOW = 2;
        private const int c_nVISIO_ROW_COMPONENT = 0;
        private const string c_strFOREIGN = "foreign";

        #endregion Constants

        private string m_strConvertFile;
        private string m_strTempDir;
        private string m_strTempVsxFile;
        private string m_strVssFileName;

        private Hashtable m_hashParaStyle = null;
        private Hashtable m_hashFillStyle = null;
        private Hashtable m_hashLineStyle = null;
        private Hashtable m_hashCharStyle = null;
        private Hashtable m_hashTextBlock = null;
        private Hashtable m_hashColors = null;
        private Hashtable m_hashFaceNames = null;
        private Hashtable m_hashMasterScale = null;

        private bool m_bMainShape = false;
        private float fYCenter = 0;
        private float fXCenter = 0;

        private Node m_nodeTemp = null;

        private GraphicsPath m_gpTemp;
        private SymbolPalette m_paletteTemp = null;
        private string m_strSymbolPaletteName = "no name";
        private Group m_symbolTemp = null;
        private Hashtable m_hashVisioShapes = new Hashtable();
        private ArrayList m_filesCreated = new ArrayList();
        private bool m_bTopShape = false;
        private bool m_bIgnoreStyles;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VisioStencilConvert"/> class.
        /// </summary>
        public VisioStencilConvert()
        {
            m_gpTemp = new GraphicsPath();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisioStencilConvert"/> class.
        /// </summary>
        /// <param name="strFileName">Name of the file.</param>
        /// <param name="ignoreStyles">Ignore styles, if set to true.</param>
        public VisioStencilConvert(string strFileName, bool ignoreStyles)
            : this()
        {
            m_strConvertFile = strFileName;
            m_bIgnoreStyles = ignoreStyles;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisioStencilConvert"/> class.
        /// </summary>
        /// <param name="strFileName">Name of the file.</param>
        /// <param name="strTempDir">The temp dir.</param>
        /// <param name="ignoreStyles">Ignore styles, if set to true.</param>
        public VisioStencilConvert(string strFileName, string strTempDir, bool ignoreStyles)
            : this()
        {
            m_strConvertFile = strFileName;
            m_strTempDir = strTempDir;
            m_bIgnoreStyles = ignoreStyles;
        }

        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets the hash master scale.
        /// </summary>
        /// <value>The hash master scale.</value>
        protected Hashtable HashMasterScale
        {
            get
            {
                if (m_hashMasterScale == null)
                    m_hashMasterScale = new Hashtable();

                return m_hashMasterScale;
            }
        }
        #endregion Properties

        #region Delegates

        delegate void ProcessNodeEventHandler(XmlNode nodeLine, IBaseHelper hlpLine);

        #endregion

        #region Methods

        /// <summary>
        /// Convert internally.
        /// </summary>
        public void InternalConvert()
        {
            ConvertVssToVsx();

            ProcessXmlDocument();

            // delete temporary files
            if (m_filesCreated.Count > 0)
            {
                for (int i = 0; i < m_filesCreated.Count; i++)
                {
                    try
                    {
                        File.Delete((string)m_filesCreated[i]);
                    }
                    catch (SystemException)
                    {
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// Converts this instance.
        /// </summary>
        /// <returns>Symbol Palette.</returns>
        public SymbolPalette Convert()
        {
            InternalConvert();
            return m_paletteTemp;
        }

        #region Helper Methods
        private void ProcessShape(IVShape shape)
        {
            if (shape.Shapes.Count == 0)
            {
                GraphicsPath gp = new GraphicsPath();
                GraphicsPath gp1 = new GraphicsPath();
                int i = 0;

                foreach (IVPath path in shape.PathsLocal)
                {
                    int nNoShow = ((int)shape.get_CellsSRC((short)(i + c_nVISIO_SECTION_FIRST_COMPONENT), c_nVISIO_ROW_COMPONENT, c_nVISIO_COMP_NO_SHOW).ResultIU);
                    
                    // path's start point
                    PointF ptStart = PointF.Empty;
                    
                    // path's end point
                    PointF ptEnd = PointF.Empty;
                    bool bStartInit = false;

                    // check for NoShow value and proceed depending on its value
                    if (nNoShow != 1)
                    {
                        gp1.Reset();
                        
                        // iterate through path's curves
                        // cheking whether curve is valid
                        int nCurveCount = 1;
                        int nCurveLength = path.Count;

                        while (nCurveCount <= nCurveLength)
                        {
                            IVCurve curve = path[nCurveCount];

                            if (curve != null)
                            {
                                int nRowType;

                                if (nCurveCount == 1)
                                {
                                    // if first row type is not c_nROW_TYPE_MOVE_TO proceed parse curve else proceed to next row type
                                    nRowType = shape.get_RowType((short)(i + c_nVISIO_SECTION_FIRST_COMPONENT), 0 + c_nVISIO_ROW_VERTEX);

                                    if (nRowType == c_nROW_TYPE_MOVE_TO || nRowType == c_nROW_TYPE_INVALID)
                                    {
                                        nRowType = shape.get_RowType((short)(i + c_nVISIO_SECTION_FIRST_COMPONENT), (short)(nCurveCount + c_nVISIO_ROW_VERTEX));
                                    }
                                }
                                else
                                {
                                    nRowType = shape.get_RowType((short)(i + c_nVISIO_SECTION_FIRST_COMPONENT), (short)(nCurveCount + c_nVISIO_ROW_VERTEX));
                                }

                                // skip invalid vertexes and proceed to next Curve if RowType == MoveTo
                                if (nRowType == c_nROW_TYPE_MOVE_TO || nRowType == c_nROW_TYPE_INVALID)
                                {
                                    nCurveCount++;
                                    continue;
                                }

                                // extract current curve points
                                Array xyArray = null;

                                try
                                {
                                    curve.Points(0.001, out xyArray);
                                    PointF[] pts = new PointF[xyArray.Length / 2];

                                    for (int j = 0; j < xyArray.Length; j += 2)
                                    {
                                        double x = (double)xyArray.GetValue(j);
                                        double y = (double)xyArray.GetValue(j + 1);
                                        pts[j / 2] = new PointF(ConvertToPixels((float)Math.Round(x, 4)), ConvertToPixels((float)Math.Round(y, 4)));
                                    }

                                    // set start point
                                    if (!bStartInit)
                                    {
                                        ptStart = pts[0];
                                        bStartInit = true;
                                    }

                                    // add curve points to graphics path
                                    int k = 0;
                                    while (k < pts.Length - 1)
                                    {
                                        gp1.StartFigure();
                                        gp1.AddLine(pts[k], pts[k + 1]);
                                        gp1.CloseFigure();
                                        k++;
                                    }

                                    // set end point
                                    ptEnd = pts[pts.Length - 1];
                                }
                                catch (Exception)
                                { 
                                }

                                nCurveCount++;
                            }
                        }

                        // check wehter path is closed
                        if (ptStart == ptEnd && bStartInit)
                        {
                            GraphicsPath gpTemp = new GraphicsPath();
                            gpTemp.StartFigure();
                            gpTemp.AddLines(gp1.PathPoints);
                            gpTemp.CloseFigure();

                            gp1 = gpTemp;
                        }

                        if (gp1 != null || gp1.PathData != null)
                        {
                            try
                            {
                                gp.AddPath(gp1, false);
                            }
                            catch (Exception)
                            { 
                            }
                        }
                    }

                    if (gp.PathData != null && gp.PathData.Points != null && gp.PathData.Points.Length > 0)
                    {
                        string strHashKey = shape.get_UniqueID(1) + "-" + i.ToString();

                        if (m_hashVisioShapes.ContainsKey(strHashKey))
                        {
                            ((VisioShape)m_hashVisioShapes[strHashKey]).ShapeGraphicsPath = gp;
                        }
                        else
                        {
                            VisioShape vShape = new VisioShape();
                            vShape.ShapeGraphicsPath = gp;
                            m_hashVisioShapes.Add(strHashKey, vShape);
                        }
                    }

                    i++;
                }
            }
            else
            {
                foreach (IVShape s in shape.Shapes)
                {
                    RectangleF rectShapes = RectangleF.Empty;
                    if ((s as IVShape).Type == c_VISIO_SHAPE_TYPE_GROUP)
                    {
                        double dLeft, dTop, dRight, dBottom;
                        s.BoundingBox(c_sBBoxUprightWH, out dLeft, out dBottom, out dRight, out dTop);
                        rectShapes.X = ConvertToPixels((float)dLeft);
                        rectShapes.Y = ConvertToPixels((float)dTop);
                        rectShapes.Width = ConvertToPixels((float)(dRight - dLeft));
                        rectShapes.Height = ConvertToPixels((float)(dTop - dBottom));

                        if (m_hashVisioShapes.ContainsKey(s.get_UniqueID(1)))
                            ((VisioShape)m_hashVisioShapes[s.get_UniqueID(1)]).GroupRect = rectShapes;
                        else
                        {
                            VisioShape vShape = new VisioShape();
                            vShape.GroupRect = rectShapes;
                            m_hashVisioShapes.Add(s.get_UniqueID(1), vShape);
                        }
                    }

                    ProcessShape(s);
                }
            }
        }

        private void ExtractGeoms(IVMaster master)
        {
            foreach (IVShape s in master.Shapes)
            {
                RectangleF rectShapes = RectangleF.Empty;
                if ((s as IVShape).Type == c_VISIO_SHAPE_TYPE_GROUP)
                {
                    double dLeft, dTop, dRight, dBottom;
                    (s as IVShape).BoundingBox(c_sBBoxUprightWH, out dLeft, out dBottom, out dRight, out dTop);
                    rectShapes.X = ConvertToPixels((float)dLeft);
                    rectShapes.Y = ConvertToPixels((float)dTop);
                    rectShapes.Width = ConvertToPixels((float)(dRight - dLeft));
                    rectShapes.Height = ConvertToPixels((float)(dTop - dBottom));

                    if (m_hashVisioShapes.ContainsKey((s as IVShape).get_UniqueID(1)))
                        ((VisioShape)m_hashVisioShapes[(s as IVShape).get_UniqueID(1)]).GroupRect = rectShapes;
                    else
                    {
                        VisioShape vShape = new VisioShape();
                        vShape.GroupRect = rectShapes;
                        m_hashVisioShapes.Add((s as IVShape).get_UniqueID(1), vShape);
                    }
                }

                ProcessShape(s);
            }
        }

        private void ConvertVssToVsx()
        {
            Random rnd = new Random();
            
            // make file copy to avoid file locking problems
            // when the same file could be started converting in multiply Visio instances
            string strFileNameWithoutExt = GetTempDirectory() + @"\" + GetVssFileName() + (int)(rnd.NextDouble() * 1000);

            string strFileName = strFileNameWithoutExt + Path.GetExtension(m_strConvertFile);

            // ensure unique name
            while (File.Exists(strFileName))
            {
                strFileNameWithoutExt = GetTempDirectory() + @"\" + GetVssFileName() + (int)(rnd.NextDouble() * 1000);
                strFileName = strFileNameWithoutExt + Path.GetExtension(m_strConvertFile);
            }

            // make file copy
            File.Copy(m_strConvertFile, strFileName);

            string strTempFile = strFileNameWithoutExt + ".vsx";

            m_filesCreated.Clear();
            m_filesCreated.Add(strTempFile);
            m_filesCreated.Add(strFileName);

            InvisibleAppClass app = null;
            IVDocument doc = null;

            try
            {
                app = new InvisibleAppClass();
                doc = null;
                app.Documents.Open(strFileName);

                IEnumerator ienum1 = app.Documents.GetEnumerator();
                ienum1.MoveNext();
                doc = ienum1.Current as IVDocument;

                foreach (IVMaster m in doc.Masters)
                {
                    double pageScale = 1;
                    double drawingScale = 1;

                    Cell cellPageScale = m.PageSheet.get_Cells("PageScale");
                    Cell cellDrawingScale = m.PageSheet.get_Cells("DrawingScale");
                    short units = cellPageScale.Units;

                    if (cellPageScale != null)
                        pageScale = cellPageScale.get_Result(units);

                    if (cellDrawingScale != null)
                        drawingScale = cellDrawingScale.get_Result(units);

                    if (pageScale != 1 && drawingScale != 1)
                    {
                        HashMasterScale[m.ID] = new PageScale("Custom", (float)pageScale, (float)drawingScale);
                    }

                    try
                    {
                        ExtractGeoms(m);
                    }
                    catch (COMException)
                    {
                        continue;
                    }
                }

                try
                {
                    Array namesU;
                    doc.MasterShortcuts.GetNamesU(out namesU);

                    for (int id = 0, length = doc.MasterShortcuts.Count; id < length; id++)
                    {
                        IVMasterShortcut ms = doc.MasterShortcuts.get_ItemU(namesU.GetValue(id));

                        try
                        {
                            Document d = app.Documents.OpenEx(ms.TargetDocumentName, (short)VisOpenSaveArgs.visOpenDocked + (short)VisOpenSaveArgs.visOpenRO);
                            IVMaster m = d.Masters[ms.TargetMasterName];

                            ExtractGeoms(m);

                            // replace shortcut by master
                            string name = ms.Name;
                            
                            // add master to document
                            Master newMaster = doc.Drop(m, 0, 0);
                            
                            // remove shortcut from document
                            ms.Delete();
                            
                            // set unique master name from use shortcut
                            newMaster.Name = name;

                            d.Close();
                        }
                        catch (COMException)
                        {
                            continue;
                        }
                    }
                }
                catch (COMException)
                { 
                }

                doc.SaveAs(strTempFile);
            }
            catch (COMException e)
            {
                string strErrorMessage = "You must have Visio installed in order to use this feature.";
                if (e.Source == "Microsoft Visio")
                    strErrorMessage = e.Message;

                // we have to rethrow exception
                throw new COMException(strErrorMessage, e);
            }
            finally
            {
                if (doc != null)
                    doc.Close();
                if (app != null)
                    app.Quit();
            }

            m_strTempVsxFile = strTempFile;
        }
        private string GetTempDirectory()
        {
            string strToReturn;

            if (m_strTempDir == null || m_strTempDir == string.Empty)
            {
                strToReturn = Path.GetTempPath();
            }
            else
            {
                strToReturn = m_strTempDir;
            }

            return strToReturn;
        }
        private string GetVssFileName()
        {
            m_strVssFileName = Path.GetFileNameWithoutExtension(m_strConvertFile);
            return m_strVssFileName;
        }

        #region Palette Helper Methods

        private void CreateSymbolPalette()
        {
            m_paletteTemp = new SymbolPalette();
            
            // m_paletteTemp.BoundaryConstraintsEnabled = false;
            m_paletteTemp.Name = m_strSymbolPaletteName;
        }

        #endregion Palette Methods

        #region Xml Processing Helper Methods

        private void ProcessXmlDocument()
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(m_strTempVsxFile);
            if (doc.HasChildNodes)
            {
                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    switch (node.LocalName.ToLower())
                    {
                        case "documentproperties":
                            ProcessDocProps(node);
                            break;
                        case "masters":
                            ProcessStencils(node);
                            break;
                        case "stylesheets":
                            ProcessStyleSheets(node);
                            break;
                        case "colors":
                            ProcessColors(node);
                            break;
                        case "facenames":
                            ProcessFaceNames(node);
                            break;
                    }
                }
            }
        }

        #region Style Sheets Helper Methods

        private void ProcessFaceNames(XmlNode nodeFaceNames)
        {
            if (nodeFaceNames.HasChildNodes)
            {
                if (m_hashFaceNames == null)
                    m_hashFaceNames = new Hashtable();

                foreach (XmlNode node in nodeFaceNames)
                {
                    int nID = 0;
                    if (GetAttribute(node, ref nID, "ID"))
                    {
                        string strTemp = ((XmlElement)node).GetAttribute("Name");
                        m_hashFaceNames.Add(nID, strTemp);
                    }
                }
            }
        }

        private void ProcessColors(XmlNode nodeColors)
        {
            if (nodeColors.HasChildNodes)
            {
                if (m_hashColors == null)
                    m_hashColors = new Hashtable();
                foreach (XmlNode node in nodeColors)
                {
                    int nID = 0;
                    if (GetAttribute(node, ref nID, "IX"))
                    {
                        string strTemp = ((XmlElement)node).GetAttribute("RGB");
                        Color clr = ColorTranslator.FromHtml(strTemp);
                        m_hashColors.Add(nID, clr);
                    }
                }
            }
        }

        private void ProcessStyleSheets(XmlNode nodeStyleSheets)
        {
            if (nodeStyleSheets.HasChildNodes)
            {
                foreach (XmlNode nodeStyleSheet in nodeStyleSheets)
                {
                    ProcessStyleSheet(nodeStyleSheet);
                }
            }
        }

        private void ProcessStyleSheet(XmlNode nodeStyleSheet)
        {
            if (nodeStyleSheet.HasChildNodes)
            {
                HelperFillStyle helperFillStyle = new HelperFillStyle();
                AddToHashTable(nodeStyleSheet, helperFillStyle, "FillStyle", ref m_hashFillStyle);

                HelperLineStyle helperLineStyle = new HelperLineStyle();
                AddToHashTable(nodeStyleSheet, helperLineStyle, "LineStyle", ref m_hashLineStyle);

                HelperTextBlockStyle helperTextBlockStyle = new HelperTextBlockStyle();
                AddToHashTable(nodeStyleSheet, helperTextBlockStyle, "TextStyle", ref m_hashTextBlock);

                HelperCharStyle helperCharStyle = new HelperCharStyle();
                AddToHashTable(nodeStyleSheet, helperCharStyle, "TextStyle", ref m_hashCharStyle);

                HelperParaStyle helperParaStyle = new HelperParaStyle();
                AddToHashTable(nodeStyleSheet, helperParaStyle, "TextStyle", ref m_hashParaStyle);

                foreach (XmlNode node in nodeStyleSheet)
                {
                    switch (node.LocalName.ToLower())
                    {
                        case "fill":
                            
                            // AddToHashTable( nodeStyleSheet, helperFillStyle, "FillStyle", ref m_hashFillStyle );
                            ProcessFill(node, helperFillStyle);
                            break;
                        case "line":
                            ProcessLine(node, helperLineStyle);
                            break;                            
                        case "textblock":
                            // text styles
                            ProcessTextBlock(node, helperTextBlockStyle);
                            break;
                        case "char":
                            ProcessCharNode(node, helperCharStyle);
                            break;
                        case "para":
                            ProcessParaNode(node, helperParaStyle);
                            break;
                    }
                }
            }
        }

        private void AddToHashTable(XmlNode nodeStyleSheet, IBaseHelper hlp, string strAttrToSearch, ref Hashtable hash)
        {
            int nHelper = -1;
            if (hash == null)
                hash = new Hashtable();
            if (GetAttribute(nodeStyleSheet, ref nHelper, "ID"))
            {
                // hlp.ID = nHelper;
                hash.Add(nHelper, hlp);
                if (GetAttribute(nodeStyleSheet, ref nHelper, strAttrToSearch))
                    hlp.InheritedFrom = nHelper;
            }
        }

        private void ProcessParaNode(XmlNode nodePara, IBaseHelper helperParaStyle)
        {
            if (nodePara.HasChildNodes)
            {
                ProcessTextParaNode(nodePara, helperParaStyle);
            }
        }

        private void ProcessTextParaNode(XmlNode nodePara, IBaseHelper hlpPara)
        {
            HelperParaStyle hlpParaStyle = hlpPara as HelperParaStyle;

            foreach (XmlNode node in nodePara)
            {
                switch (node.LocalName.ToLower())
                {
                    case "indfirst":
                        hlpParaStyle.IndFirst = (int)ProcessValue(node);
                        break;
                    case "indleft":
                        hlpParaStyle.IndLeft = (int)ProcessValue(node);
                        break;
                    case "indright":
                        hlpParaStyle.IndRight = ProcessValue(node);
                        break;
                    case "spafter":
                        hlpParaStyle.SpAfter = (int)ProcessValue(node);
                        break;
                    case "spbefore":
                        hlpParaStyle.SpBefore = (int)ProcessValue(node);
                        break;
                    case "spline":
                        hlpParaStyle.SpLine = (int)ProcessValue(node);
                        break;
                    case "textposafterbullet":
                        hlpParaStyle.TextPosAfterBullet = (int)ProcessValue(node);
                        break;
                    case "horzalign":
                        hlpParaStyle.HorzAlign = (int)ProcessValue(node);
                        break;
                }
            }
        }

        private void ProcessCharNode(XmlNode nodeChar, IBaseHelper helperTextStyle)
        {
            if (nodeChar.HasChildNodes)
            {
                ProcessTextCharNode(nodeChar, helperTextStyle);
            }
        }

        private void ProcessTextBlock(XmlNode nodeTextBlock, IBaseHelper helperTextStyle)
        {
            if (nodeTextBlock.HasChildNodes)
            {
                ProcessTextBlockNode(nodeTextBlock, helperTextStyle);
            }
        }

        private void ProcessLine(XmlNode nodeLine, IBaseHelper helperLineStyle)
        {
            if (nodeLine.HasChildNodes)
            {
                ProcessLineNode(nodeLine, helperLineStyle);
            }
        }

        private void ProcessFill(XmlNode nodeFill, IBaseHelper helperFillStyle)
        {
            if (nodeFill.HasChildNodes)
            {
                ProcessFillNode(nodeFill, helperFillStyle);
            }
        }

        private void ProcessTextBlockNode(XmlNode nodeTextBlock, IBaseHelper hlpText)
        {
            HelperTextBlockStyle hlpTextBlockStyle = hlpText as HelperTextBlockStyle;

            foreach (XmlNode node in nodeTextBlock)
            {
                switch (node.LocalName.ToLower())
                {
                    case "textbkgnd":
                        hlpTextBlockStyle.TextBackground = ProcessColor(node, true);
                        break;
                }
            }
        }

        private void ProcessTextCharNode(XmlNode nodeChar, IBaseHelper hlpChar)
        {
            HelperCharStyle hlpCharStyle = hlpChar as HelperCharStyle;

            foreach (XmlNode node in nodeChar)
            {
                switch (node.LocalName.ToLower())
                {
                    case "font":
                        hlpCharStyle.FontID = (int)ProcessValue(node);
                        break;
                    case "color":
                        hlpCharStyle.TextColor = ProcessColor(node, false);
                        break;
                    case "size":
                        hlpCharStyle.TextSize = ProcessValue(node);
                        break;
                    case "langid":
                        hlpCharStyle.LangID = (int)ProcessValue(node);
                        break;
                    case "style":
                        hlpCharStyle.Style = (CharFontStyle)ProcessValue(node);
                        break;
                    case "case":
                        hlpCharStyle.Case = (int)ProcessValue(node);
                        break;
                    case "rtltext":
                        hlpCharStyle.RTL = (int)ProcessValue(node);
                        break;
                    case "strikethru":
                        hlpCharStyle.StrikeThrough = (int)ProcessValue(node);
                        break;
                    case "dblunderline":
                        hlpCharStyle.DoubleUnderline = (int)ProcessValue(node);
                        break;
                    case "usevertical":
                        hlpCharStyle.UseVertical = (int)ProcessValue(node);
                        break;
                    case "pos":
                        hlpCharStyle.Position = (int)ProcessValue(node);
                        break;
                }
            }
        }

        private void ProcessLineNode(XmlNode nodeLine, IBaseHelper hlpLine)
        {
            HelperLineStyle hlpLineStyle = hlpLine as HelperLineStyle;

            foreach (XmlNode node in nodeLine)
            {
                switch (node.LocalName.ToLower())
                {
                    case "linecolor":
                        hlpLineStyle.LineColor = ProcessColor(node, false);
                        break;
                    case "lineweight":
                        hlpLineStyle.LineWeight = ProcessValue(node);
                        break;
                    case "linepattern":
                        hlpLineStyle.LinePattern = (int)ProcessValue(node);
                        break;
                    case "linecap":
                        hlpLineStyle.LineCap = (int)ProcessValue(node);
                        break;
                }
            }
        }

        private void ProcessFillNode(XmlNode nodeFill, IBaseHelper hlpFill)
        {
            HelperFillStyle hlpFillStyle = hlpFill as HelperFillStyle;

            foreach (XmlNode node in nodeFill)
            {
                switch (node.LocalName.ToLower())
                {
                    case "fillpattern":
                        hlpFillStyle.FillPattern = (int)ProcessValue(node);
                        break;
                    case "fillforegnd":
                        hlpFillStyle.FillForegnd = ProcessColor(node, false);
                        break;
                    case "fillbkgnd":
                        hlpFillStyle.FillBkgnd = ProcessColor(node, false);
                        break;
                    case "fillforegndtrans":
                        hlpFillStyle.ForeColorTransparency = ProcessValue(node);
                        break;
                    case "fillbkgndtrans":
                        hlpFillStyle.BackColorTransparency = ProcessValue(node);
                        break;
                    case "shdwpattern":
                        hlpFillStyle.ShdwPattern = (int)ProcessValue(node);
                        break;
                    case "shdwforegnd":
                        hlpFillStyle.ShdwForegnd = ProcessColor(node, false);
                        break;
                    case "shdwbkgnd":
                        hlpFillStyle.ShdwBkgnd = ProcessColor(node, false);
                        break;
                    case "shapeshdwoffsetx":
                        hlpFillStyle.ShdwOffsetX = (int)ConvertToPixels(ProcessValue(node));
                        break;
                    case "shapeshdwoffsety":
                        hlpFillStyle.ShdwOffsetY = (int)ConvertToPixels(ProcessValue(node));
                        break;
                    case "shdwforegndtrans":
                        hlpFillStyle.ShdwForegndTrans = ProcessValue(node);
                        break;
                    case "shdwbkgndtrans":
                        hlpFillStyle.ShdwBkgndTrans = ProcessValue(node);
                        break;
                }
            }
        }

        #endregion Style Sheets Helper Methods

        private void ProcessDocProps(XmlNode nodeDocProps)
        {
            if (nodeDocProps.HasChildNodes)
            {
                XmlNode node = nodeDocProps.FirstChild;
                while (node != null)
                {
                    if (node.LocalName.ToLower() == "title")
                    {
                        m_strSymbolPaletteName = node.InnerText;
                        break;
                    }
                    node = node.NextSibling;
                }
            }
        }

        private void ProcessStencils(XmlNode nodeMasters)
        {
            if (nodeMasters.HasChildNodes)
            {
                CreateSymbolPalette();
                foreach (XmlNode nodeMaster in nodeMasters)
                {
                    ProcessCurrStencil(nodeMaster);
                }

                m_hashVisioShapes.Clear();
            }
        }

        private void ProcessCurrStencil(XmlNode nodeMaster)
        {
            if (nodeMaster.HasChildNodes)
            {
                XmlElement masterElement = (XmlElement)nodeMaster;

                string strSymbolName = masterElement.GetAttribute("Name");
                string strSymbolNameU = masterElement.GetAttribute("NameU");

                if (strSymbolName == string.Empty)
                    strSymbolName = strSymbolNameU;

                int nID = int.Parse(masterElement.GetAttribute("ID"));
                PageScale pageScale = HashMasterScale[nID] as PageScale;

                foreach (XmlNode curNode in nodeMaster.ChildNodes)
                {
                    switch (curNode.LocalName.ToLower())
                    {
                        case "shapes":
                            m_symbolTemp = new Group();
                            m_bMainShape = true;
                            NodeCollection mainShapeChildren = new NodeCollection();
                            ProcessShapes(curNode, m_symbolTemp, mainShapeChildren);
                            int start;
                            m_symbolTemp.AppendChildren(mainShapeChildren, out start);
                            m_symbolTemp.Name = strSymbolName;

                            // update EditStyle
                            EditStyle stlEdit = new EditStyle();
                            XmlNode nodeShape = null;

                            // find shape node
                            foreach (XmlElement n in curNode.ChildNodes)
                            {
                                if (n.LocalName.ToLower() == "shape")
                                {
                                    nodeShape = n;
                                    break;
                                }
                            }

                            if (nodeShape.HasChildNodes)
                            {
                                foreach (XmlElement n in nodeShape.ChildNodes)
                                {
                                    if (n.LocalName.ToLower() == "protection")
                                    {
                                        ProcessProtection(n, stlEdit);
                                        break;
                                    }
                                }
                            }

                            if (stlEdit != null)
                            {
                                // update EditStyle
                                m_symbolTemp.EditStyle.AllowChangeWidth = stlEdit.AllowChangeWidth;
                                m_symbolTemp.EditStyle.AllowChangeHeight = stlEdit.AllowChangeHeight;
                                m_symbolTemp.EditStyle.AllowMoveX = stlEdit.AllowMoveX;
                                m_symbolTemp.EditStyle.AllowMoveY = stlEdit.AllowMoveY;
                                m_symbolTemp.EditStyle.AllowRotate = stlEdit.AllowRotate;
                                m_symbolTemp.EditStyle.AllowSelect = stlEdit.AllowSelect;
                                m_symbolTemp.EditStyle.AllowDelete = stlEdit.AllowDelete;
                                m_symbolTemp.EditStyle.AspectRatio = stlEdit.AspectRatio;
                                m_symbolTemp.EditStyle.AllowVertexEdit = stlEdit.AllowVertexEdit;
                            }

                            m_symbolTemp.UpdateCompositeBounds();

                            UpdateNodeScale(m_symbolTemp, pageScale);
                            m_paletteTemp.AppendChild(m_symbolTemp);
                            break;
                    }
                }
            }
        }

        private void UpdateNodeScale(Node node, PageScale pageScale)
        {
            SizeF szMaxSize = CommonUsedValues.MAX_IMAGE_SIZE;
            SizeF szNode = node.Size;
            PageScale nodeScale = node.NodeScale;

            if (pageScale != null)
            {
                nodeScale = pageScale;
            }

            if (szNode.Width > szMaxSize.Width || szNode.Height > szMaxSize.Height)
            {
                // auto scale 
            }

            // set new value
            node.NodeScale = nodeScale;
        }

        private void ProcessShapes(XmlNode node, Group parent, NodeCollection children)
        {
            if (node.HasChildNodes)
            {
                foreach (XmlNode nodeShape in node.ChildNodes)
                {
                    switch (nodeShape.LocalName.ToLower())
                    {
                        case "shape":
                            m_bTopShape = m_bMainShape;
                            ProcessShape(nodeShape, parent, children);
                            break;
                        case "shapes":
                            ProcessShapes(nodeShape, parent, children);
                            break;
                    }
                }
            }
        }

        private void ProcessShape(XmlNode nodeShape, Group parent, NodeCollection children)
        {
            if (nodeShape.HasChildNodes)
            {
                int nGeomCount = -1;
                int nGeomsShown = -1;
                XForm xfShapeProp = new XForm();
                XForm xfTextProp = new XForm();
                EditStyle styleEdit = new EditStyle();

                ConnectionTransformation connTransform = null;
                ArrayList lstConnections = null;
                string unique = ((XmlElement)nodeShape).GetAttribute("UniqueID");
                bool bConnectionPresent = CheckForNodeExistance(nodeShape, "connection");

                if (bConnectionPresent)
                {
                    lstConnections = new ArrayList();
                    connTransform = new ConnectionTransformation();
                    GetConnectionNodes(nodeShape, lstConnections);
                }

                foreach (XmlNode node in nodeShape.ChildNodes)
                {
                    switch (node.LocalName.ToLower())
                    {
                        case "shapes":
                            NodeCollection grchildren = children;
                            Group group = parent;

                            if (!m_bTopShape)
                            {
                                group = new Group();
                                group.Name = ((XmlElement)nodeShape).GetAttribute("NameU");

                                if (group.Name == String.Empty)
                                    group.Name = "Shape" + ((XmlElement)nodeShape).GetAttribute("ID");

                                grchildren = new NodeCollection();
                            }

                            XForm xf = new XForm();
                            EditStyle stlEdit = new EditStyle();

                            foreach (XmlElement n in node.ParentNode.ChildNodes)
                            {
                                if (n.LocalName.ToLower() == "protection")
                                {
                                    ProcessProtection(n, stlEdit);
                                    break;
                                }
                            }

                            if (!group.Equals(parent))
                            {
                                foreach (XmlElement n in node.ParentNode.ChildNodes)
                                {
                                    if (n.LocalName.ToLower() == "xform")
                                    {
                                        ProcessXForm(n, ref xf);
                                        break;
                                    }
                                }

                                group.Size = new SizeF(xf.Width, xf.Height);
                                group.PinPoint = new PointF(xf.PinX, parent.Size.Height - xf.PinY);
                                group.PinPointOffset = new SizeF(xf.LocPinX, xf.Height - xf.LocPinY);
                                group.RotationAngle = xf.FlipY ? xf.RotationAngle : -xf.RotationAngle;
                                group.FlipX = xf.FlipX;
                                group.FlipY = xf.FlipY;
                            }

                            ProcessShapes(node, group, grchildren);
                            int start;

                            if (!group.Equals(parent))
                            {
                                group.AppendChildren(grchildren, out start);
                                float height = (parent.Size.Height < Math.Abs(xf.PinY)) ? Math.Abs(xf.PinY) : parent.Size.Height;
                                float pinx = xf.PinX;
                                float piny = height - xf.PinY;

                                float locpinx = xf.LocPinX;
                                float locpiny = xf.Height - xf.LocPinY;
                                if (piny == 0)
                                {
                                    pinx = xf.LocPinX;
                                    piny = parent.Size.Height + xf.LocPinY;
                                }                                
                                group.PinPoint = new PointF(pinx, piny);
                                group.PinPointOffset = new SizeF(locpinx, locpiny);                                                                         
                                group.RotationAngle = xf.FlipY ? xf.RotationAngle : -xf.RotationAngle;
                                group.FlipX = xf.FlipX;
                                group.FlipY = xf.FlipY;

                                // update EditStyle
                                group.EditStyle.AllowChangeWidth = stlEdit.AllowChangeWidth;
                                group.EditStyle.AllowChangeHeight = stlEdit.AllowChangeHeight;
                                group.EditStyle.AllowMoveX = stlEdit.AllowMoveX;
                                group.EditStyle.AllowMoveY = stlEdit.AllowMoveY;
                                group.EditStyle.AllowRotate = stlEdit.AllowRotate;
                                group.EditStyle.AllowSelect = stlEdit.AllowSelect;
                                group.EditStyle.AllowDelete = stlEdit.AllowDelete;
                                group.EditStyle.AspectRatio = stlEdit.AspectRatio;
                                group.EditStyle.AllowVertexEdit = stlEdit.AllowVertexEdit;

                                children.Add(group);
                            }

                            grchildren = null;
                            break;
                        case "protection":
                            ProcessProtection(node, styleEdit);
                            break;
                        case "xform":
                            ProcessXForm(node, ref xfShapeProp);
                            if (m_bMainShape)
                            {
                                fYCenter = xfShapeProp.LocPinY + xfShapeProp.Height;
                                fXCenter = xfShapeProp.LocPinX;
                                parent.Size = new SizeF(xfShapeProp.LocPinX + xfShapeProp.Width, xfShapeProp.Height + xfShapeProp.LocPinY);
                                parent.PinPoint = new PointF(xfShapeProp.LocPinX, parent.Size.Height - xfShapeProp.LocPinY);
                                parent.PinPointOffset = new SizeF(xfShapeProp.LocPinX, xfShapeProp.Height - xfShapeProp.LocPinY);
                                parent.RotationAngle = xfShapeProp.FlipY ? xfShapeProp.RotationAngle : -xfShapeProp.RotationAngle;
                                parent.FlipX = xfShapeProp.FlipX;
                                parent.FlipY = xfShapeProp.FlipY;
                                m_bMainShape = false;
                            }

                            // ( stencil with text only has no TextXForm node defining textblock properties )
                            // so we have to take shape (XForm) properties
                            xfTextProp = (XForm)xfShapeProp.Clone();
                            break;
                        case "textxform":
                            // changes made to shape reflects on text block ( primarily it is Flipping )
                            ProcessTextXForm(node, ref xfTextProp);
                            break;
                        case "geom":
                            bool bNoShow = false;
                            XmlNode nodeNoShow = SearchForSibling(node, "noshow", null, 0, false);
							if(nodeNoShow!=null)
                            if (int.Parse(nodeNoShow.InnerText) == 1)
                                bNoShow = true;
                            nGeomCount++;
                            if (!bNoShow) nGeomsShown++;

                            if (nGeomsShown > 0) break;

                            if (!bNoShow)
                            {
                                string strHashKey = unique + "-" + nGeomCount.ToString();

                                if (m_hashVisioShapes.ContainsKey(strHashKey))
                                {
                                    m_gpTemp = ((VisioShape)(m_hashVisioShapes[strHashKey])).ShapeGraphicsPath;
                                    if (m_gpTemp != null)
                                    {
                                        if (m_gpTemp.PointCount != 0)
                                        {
                                            PointF[] pts = m_gpTemp.PathPoints;
                                            RectangleF rect = m_gpTemp.GetBounds();
                                            RectangleF rect2 = rect;
                                            for (int i = 0, length = pts.Length; i < length; i++)
                                            {
                                                pts[i] = new PointF(pts[i].X, parent.Size.Height - pts[i].Y);
                                            }

                                            m_gpTemp = new GraphicsPath(pts, m_gpTemp.PathTypes, m_gpTemp.FillMode);
                                            rect = m_gpTemp.GetBounds();

                                            if (m_bTopShape) m_bTopShape = false;

                                            m_nodeTemp = new FilledPath((GraphicsPath)m_gpTemp.Clone());
                                            string name = ((XmlElement)nodeShape).GetAttribute("NameU");
                                            if (name == String.Empty) name = "Shape" + ((XmlElement)nodeShape).GetAttribute("ID");
                                            m_nodeTemp.Name = name;

                                            if (!m_bIgnoreStyles)
                                            {
                                                ProcessGeom(node);
                                            }

                                            float height = (parent.Size.Height < Math.Abs(xfShapeProp.PinY)) ? Math.Abs(xfShapeProp.PinY) : parent.Size.Height;
                                            float pinx = xfShapeProp.PinX;
                                            float piny = height - xfShapeProp.PinY;
                                            float locpinx = xfShapeProp.LocPinX - rect.X;
                                            float locpiny = xfShapeProp.Height - xfShapeProp.LocPinY - (xfShapeProp.Height - rect2.Height - rect2.Y);
											if (piny == 0)
                                            {
                                                pinx = xfShapeProp.LocPinX;
                                                piny = parent.Size.Height - xfShapeProp.LocPinY;
                                            }
                                            m_nodeTemp.PinPointOffset = new SizeF(0, 0);
                                            m_nodeTemp.PinPoint = new PointF(pinx, piny);
                                            m_nodeTemp.PinPointOffset = new SizeF(locpinx, locpiny);
                                            m_nodeTemp.RotationAngle = xfShapeProp.FlipY ? xfShapeProp.RotationAngle : -xfShapeProp.RotationAngle;
                                            m_nodeTemp.FlipX = xfShapeProp.FlipX;
                                            m_nodeTemp.FlipY = xfShapeProp.FlipY;

                                            // update EditStyle
                                            m_nodeTemp.EditStyle.AllowChangeWidth = styleEdit.AllowChangeWidth;
                                            m_nodeTemp.EditStyle.AllowChangeHeight = styleEdit.AllowChangeHeight;
                                            m_nodeTemp.EditStyle.AllowMoveX = styleEdit.AllowMoveX;
                                            m_nodeTemp.EditStyle.AllowMoveY = styleEdit.AllowMoveY;
                                            m_nodeTemp.EditStyle.AllowRotate = styleEdit.AllowRotate;
                                            m_nodeTemp.EditStyle.AllowSelect = styleEdit.AllowSelect;
                                            m_nodeTemp.EditStyle.AllowDelete = styleEdit.AllowDelete;
                                            m_nodeTemp.EditStyle.AspectRatio = styleEdit.AspectRatio;
                                            m_nodeTemp.EditStyle.AllowVertexEdit = styleEdit.AllowVertexEdit;

                                            children.Add(m_nodeTemp);
                                            if (lstConnections != null)
                                            {
                                                ProcessConnections(lstConnections, m_nodeTemp, parent);
                                                lstConnections = null;
                                            }

                                            m_nodeTemp = null;
                                            m_gpTemp.Reset();
                                        }
                                    }
                                }
                            }
                            break;
                        case "text":
                            ProcessText(node, xfTextProp, xfShapeProp);
                            if (m_nodeTemp is RichTextNode)
                            {
                                if (xfTextProp.Width == 0)
                                    xfTextProp.Width = xfShapeProp.Width;
                                if (xfTextProp.Height == 0)
                                    xfTextProp.Height = xfShapeProp.Height;

                                RectangleF rect = new RectangleF(0, 0, xfTextProp.Width, xfTextProp.Height);

                                float height = (parent.Size.Height < Math.Abs(xfShapeProp.PinY)) ? Math.Abs(xfShapeProp.PinY) : parent.Size.Height;
                                float pinx = xfShapeProp.PinX;
                                float piny = height - xfShapeProp.PinY;
                                float locpinx = xfShapeProp.LocPinX - rect.X;
                                float locpiny = xfShapeProp.Height - xfShapeProp.LocPinY;

                                // get node name
                                string name = ((XmlElement)nodeShape).GetAttribute("NameU");
                                if (name == String.Empty) name = "Shape" + ((XmlElement)nodeShape).GetAttribute("ID");
                                m_nodeTemp.Name = name;

                                m_nodeTemp.PinPointOffset = new SizeF(0, 0);
                                m_nodeTemp.PinPoint = new PointF(locpinx, locpiny);
                                m_nodeTemp.PinPointOffset = new SizeF(locpinx, locpiny);
                                m_nodeTemp.RotationAngle = xfShapeProp.FlipY ? xfShapeProp.RotationAngle : -xfShapeProp.RotationAngle;
                                m_nodeTemp.FlipX = xfShapeProp.FlipX;
                                m_nodeTemp.FlipY = xfShapeProp.FlipY;
								if ((children.FindNodeByName(m_nodeTemp.Name) is Node))
                                    children.Remove(children.FindNodeByName(m_nodeTemp.Name));
                                children.Add(m_nodeTemp);
                                if (lstConnections != null)
                                {
                                    ProcessConnections(lstConnections, m_nodeTemp, parent);
                                    lstConnections = null;
                                }
                                m_nodeTemp = null;
                            }
                            break;
                        case "foreigndata":
                            bool bBitmap = HasBitmap(node);
                            bool bMetafile = HasMetafile(node);
                            
                            // if foreign type is Bitmap or Metafile ->
                            // create appropriate Image and append it to fii.Image
                            if (bBitmap || bMetafile)
                            {
                                // get stream containing image data
                                byte[] byteArray = System.Convert.FromBase64String(node.InnerText);

                                // get foreign node with image properties
                                XmlNode xmlNode = GetNodeByName(c_strFOREIGN, nodeShape);
                                ForeignImageInfo fii = new ForeignImageInfo();

                                if (xmlNode != null)
                                {
                                    ProcessImageInfo(xmlNode, fii);
                                }

                                Image bmpTmp;
                                Image bmp;

                                using (MemoryStream streamMemory = new MemoryStream(byteArray))
                                {
                                    // create image of size specified in ForeignImageInfo
                                    if (bBitmap)
                                        bmpTmp = new Bitmap(streamMemory);
                                    else
                                        bmpTmp = new System.Drawing.Imaging.Metafile(streamMemory);
                                }

                                // crop image according to foreign image info settings
                                if (fii != null && bBitmap)
                                {
                                    bmpTmp = new Bitmap(bmpTmp, fii.ImageWidth, fii.ImageHeight);

                                    bmp = CropImage(fii, bmpTmp, xfShapeProp);
                                    bmpTmp.Dispose();
                                }
                                else 
                                {
                                    // use original image
                                    bmp = bmpTmp;
                                }

                                // create image node
                                if (bBitmap)
                                    CreateBitmapNode((Bitmap)bmp, parent, xfShapeProp);
                                else
                                    CreateMetafileNode((System.Drawing.Imaging.Metafile)bmp, parent, xfShapeProp);

                                children.Add(m_nodeTemp);

                                if (lstConnections != null)
                                {
                                    ProcessConnections(lstConnections, m_nodeTemp, parent);
                                    lstConnections = null;
                                }

                                m_nodeTemp = null;
                            }
                            break;
                    }
                }
            }
        }

        private void ProcessImageInfo(XmlNode nodeImgInfo, ForeignImageInfo fii)
        {
            if (nodeImgInfo.HasChildNodes)
            {
                foreach (XmlNode curNode in nodeImgInfo.ChildNodes)
                {
                    switch (curNode.LocalName.ToLower())
                    {
                        case "imgwidth":
                            fii.ImageWidth = (int)ConvertToPixels(ProcessValue(curNode));
                            break;
                        case "imgheight":
                            fii.ImageHeight = (int)ConvertToPixels(ProcessValue(curNode));
                            break;
                        case "imgoffsetx":
                            fii.OffsetXToOrigin = ConvertToPixels(ProcessValue(curNode));
                            break;
                        case "imgoffsety":
                            fii.OffsetYToOrigin = ConvertToPixels(ProcessValue(curNode));
                            break;
                    }
                }
            }
        }
        private void ProcessXForm(XmlNode nodeXForm, ref XForm xfShapeProp)
        {
            if (nodeXForm.HasChildNodes)
            {
                foreach (XmlNode curNode in nodeXForm.ChildNodes)
                {
                    switch (curNode.LocalName.ToLower())
                    {
                        case "angle":
                            xfShapeProp.RotationAngle = GetRotationAngle(curNode);
                            break;
                        case "height":
                            xfShapeProp.Height = ConvertToPixels(ProcessValue(curNode));
                            break;
                        case "width":
                            xfShapeProp.Width = ConvertToPixels(ProcessValue(curNode));
                            break;
                        case "flipx":
                            if (((int)ProcessValue(curNode)) == 1)
                                xfShapeProp.FlipX = true;
                            break;
                        case "flipy":
                            if (((int)ProcessValue(curNode)) == 1)
                                xfShapeProp.FlipY = true;
                            break;
                        case "pinx":
                            xfShapeProp.PinX = ConvertToPixels(ProcessValue(curNode));
                            break;
                        case "piny":
                            xfShapeProp.PinY = ConvertToPixels(ProcessValue(curNode));
                            break;
                        case "locpinx":
                            xfShapeProp.LocPinX = ConvertToPixels(ProcessValue(curNode));
                            break;
                        case "locpiny":
                            xfShapeProp.LocPinY = ConvertToPixels(ProcessValue(curNode));
                            break;
                    }
                }
            }
        }
        private void ProcessProtection(XmlNode nodeProtection, EditStyle editStyle)
        {
            if (nodeProtection.HasChildNodes)
            {
                foreach (XmlNode curNode in nodeProtection.ChildNodes)
                {
                    switch (curNode.LocalName.ToLower())
                    {
                        case "lockwidth":
                            editStyle.AllowChangeWidth = (int)ProcessValue(curNode) == 0 ? true : false;
                            break;
                        case "lockheight":
                            editStyle.AllowChangeHeight = (int)ProcessValue(curNode) == 0 ? true : false;
                            break;
                        case "lockmovex":
                            editStyle.AllowMoveX = (int)ProcessValue(curNode) == 0 ? true : false;
                            break;
                        case "lockmovey":
                            editStyle.AllowMoveY = (int)ProcessValue(curNode) == 0 ? true : false;
                            break;
                        case "lockaspect":
                            editStyle.AspectRatio = (int)ProcessValue(curNode) == 0 ? false : true;
                            break;
                        case "lockdelete":
                            editStyle.AllowDelete = (int)ProcessValue(curNode) == 0 ? true : false;
                            break;
                        case "lockrotate":
                            editStyle.AllowRotate = (int)ProcessValue(curNode) == 0 ? true : false;
                            break;
                        case "lockselect":
                            editStyle.AllowSelect = (int)ProcessValue(curNode) == 0 ? true : false;
                            break;
                        case "lockvtxedit":
                            editStyle.AllowVertexEdit = (int)ProcessValue(curNode) == 0 ? true : false;
                            break;
                    }
                }
            }
        }

        #region Text Processing Helpers

        private void ProcessText(XmlNode nodeText, XForm xfTextProp, XForm xfShapeProp)
        {
            if (nodeText.HasChildNodes)
            {
                RichTextBoxAdv rtfTemp;

                if ((nodeText.InnerText.Length != 0 && nodeText.InnerText != "\r\n") || (CheckForNodeExistance(nodeText, "fld")))
                {
                    // create RichTextBoxAdv to format text 
                    // after formatting we can create RichTextNode with formatted text from RichTextBoxAdv
                    rtfTemp = new RichTextBoxAdv();
                    rtfTemp.Hide();
                    rtfTemp.Text = nodeText.InnerText;

                    // create rich node with default size
                    RectangleF rect = new RectangleF(0, 0, rtfTemp.Width, rtfTemp.Height);
                    m_nodeTemp = new RichTextNode(String.Empty, rect, MeasureUnits.Pixel);

                    // we must read whole rtf string and format parts of it to prevent format changes
                    int nSelectionOffset = 0;
                    int nSelectionLength = 0;
                    foreach (XmlNode node in nodeText)
                    {
                        // !!! whitespace is not in Text node but in SignificantWhitespace node
                        if ((node.NodeType == XmlNodeType.Text) || (node.NodeType == XmlNodeType.SignificantWhitespace))
                        {
                            Match match = Regex.Match(node.InnerText, "\r\n");
                            if (match.Success)
                            {
                                nSelectionLength = node.InnerText.Length - 1;
                            }
                            else
                            {
                                nSelectionLength = node.InnerText.Length;
                            }

                            rtfTemp.SelectionStart = nSelectionOffset;
                            rtfTemp.SelectionLength = nSelectionLength;
                            nSelectionOffset += nSelectionLength;

                            SetFormattingStyles(node, rtfTemp);
                        }
                        else if ((node.NodeType == XmlNodeType.Element) && (node.LocalName.ToLower() == "fld"))
                        {
                            Match match = Regex.Match(node.InnerText, "\r\n");
                            if (match.Success)
                            {
                                nSelectionLength = node.InnerText.Length - 1;
                            }
                            else
                            {
                                nSelectionLength = node.InnerText.Length;
                            }
                            rtfTemp.SelectionStart = nSelectionOffset;
                            rtfTemp.SelectionLength = nSelectionLength;
                            nSelectionOffset += nSelectionLength;
                            
                            // we can apply styles now
                            ProcessStyle(node, new HelperCharStyle(ref rtfTemp, m_hashFaceNames), null, 0);
                            ProcessStyle(node, new HelperParaStyle(ref rtfTemp), null, 0);
                        }
                    }

                    // get node size
                    if (xfTextProp.Width == 0)
                    {
                        if (xfShapeProp.Width != 0)
                        {
                            xfTextProp.Width = xfShapeProp.Width;
                        }
                        else
                        {
                            xfTextProp.LocPinX += rtfTemp.Width / 2;
                            xfTextProp.Width = rtfTemp.Width;
                        }

                        m_nodeTemp.LineStyle.LineColor = Color.Transparent;
                    }

                    if (xfTextProp.Height == 0)
                    {
                        if (xfShapeProp.Height != 0)
                        {
                            xfTextProp.Height = xfShapeProp.Height;
                        }
                        else
                        {
                            xfShapeProp.LocPinY -= rtfTemp.Font.Height * 2;
                            xfTextProp.Height = rtfTemp.Font.Height * 2;
                        }

                        m_nodeTemp.LineStyle.LineColor = Color.Transparent;
                    }

                    // update node size
                    ((IUnitIndependent)m_nodeTemp).SetSize(new SizeF(xfTextProp.Width, xfTextProp.Height), MeasureUnits.Pixel);

                    ((RichTextNode)m_nodeTemp).RichText = rtfTemp.Rtf;
                    ProcessStyle(nodeText.FirstChild, new HelperTextBlockStyle(), null, 0);
                    rtfTemp.Dispose();
                }
            }
        }

        private bool CheckForNodeExistance(XmlNode nodeToSearchIn, string strNodeNameToSearchFor)
        {
            bool bSuccess = false;
            if (nodeToSearchIn.HasChildNodes)
            {
                XmlNode node = nodeToSearchIn.FirstChild;
                while (node != null)
                {
                    if (node.LocalName.ToLower() == strNodeNameToSearchFor.ToLower())
                    {
                        bSuccess = true;
                        break;
                    }
                    node = node.NextSibling;
                }
            }

            return bSuccess;
        }

        private void SetFormattingStyles(XmlNode nodeText, RichTextBoxAdv rtfBox)
        {
            XmlNode node = nodeText.PreviousSibling;
            if (node == null)
            {
                ProcessStyle(nodeText, new HelperCharStyle(ref rtfBox, m_hashFaceNames), null, 0);
                ProcessStyle(nodeText, new HelperParaStyle(ref rtfBox), null, 0);
            }
            else
            {
                while ((node.NodeType != XmlNodeType.Text) && (node.NodeType != XmlNodeType.SignificantWhitespace))
                {
                    int i;
                    switch (node.LocalName.ToLower())
                    {
                        case "cp":
                            i = int.Parse(((XmlElement)node).GetAttribute("IX"));
                            ProcessStyle(node, new HelperCharStyle(ref rtfBox, m_hashFaceNames), "IX", i);
                            break;
                        case "tp":
                            break;
                        case "pp":
                            int nSaveSelectionLength = rtfBox.SelectionLength;
                            rtfBox.SelectionLength = rtfBox.TextLength - rtfBox.SelectionStart;
                            i = int.Parse(((XmlElement)node).GetAttribute("IX"));
                            ProcessStyle(node, new HelperParaStyle(ref rtfBox), "IX", i);
                            rtfBox.SelectionLength = nSaveSelectionLength;
                            break;
                    }
                    node = node.PreviousSibling;
                    if (node == null)
                        break;
                }
            }
        }

        private void ProcessTextXForm(XmlNode nodeXForm, ref XForm xfShapeProp)
        {
            if (nodeXForm.HasChildNodes)
            {
                foreach (XmlNode curNode in nodeXForm.ChildNodes)
                {
                    switch (curNode.LocalName.ToLower())
                    {
                        case "txtpinx":
                            xfShapeProp.PinX = ConvertToPixels(ProcessValue(curNode));
                            break;
                        case "txtpiny":
                            xfShapeProp.PinY = ConvertToPixels(ProcessValue(curNode));
                            break;
                        case "txtlocpinx":
                            xfShapeProp.LocPinX = ConvertToPixels(ProcessValue(curNode));
                            break;
                        case "txtlocpiny":
                            xfShapeProp.LocPinY = ConvertToPixels(ProcessValue(curNode));
                            break;
                        case "txtheight":
                            xfShapeProp.Height = ConvertToPixels(ProcessValue(curNode));
                            break;
                        case "txtwidth":
                            xfShapeProp.Width = ConvertToPixels(ProcessValue(curNode));
                            break;
                        case "txtangle":
                            xfShapeProp.RotationAngle = GetRotationAngle(curNode);
                            break;
                    }
                }
            }
        }

        #endregion Text Processing Helpers

        #region Connections Processing Helper Methods

        private void ProcessConnections(ArrayList lstConnections, Node node, Group parent)
        {
            if (lstConnections.Count != 0)
            {
                IEnumerator enumConnections = lstConnections.GetEnumerator();
                while (enumConnections.MoveNext())
                {
                    ProcessConnection((XmlNode)enumConnections.Current, node, parent);
                }
            }
        }

        private void GetConnectionNodes(XmlNode nodeShape, ArrayList lstConnections)
        {
            foreach (XmlNode node in nodeShape)
            {
                if (((XmlElement)node).Name.ToLower() == c_strCONNECTION)
                {
                    lstConnections.Add(node);
                }
            }
        }

        private void ProcessConnection(XmlNode nodeConnection, Node node, Group parent)
        {
            // if current shape is of Group type we must determine ports coordinates
            // according to Group LocalPin (LocPin) not Parent (Pin)
            if (nodeConnection.HasChildNodes)
            {
                PointF ptPortLocation = new PointF();
                float fX = 0;
                
                // Calculate start position
                if (node.PinPointOffset.Width > parent.PinPointOffset.Width)
                {
                    fX = node.PinPointOffset.Width - parent.PinPointOffset.Width;
                }

                foreach (XmlNode curNode in nodeConnection.ChildNodes)
                {
                    switch (curNode.LocalName)
                    {
                        case "X":
                            ptPortLocation.X = fX + ConvertToPixels(ProcessValue(curNode));
                            break;
                        case "Y":
                            ptPortLocation.Y = node.Size.Height - ConvertToPixels(ProcessValue(curNode));
                            break;
                    }
                }
                ConnectionPoint port = new ConnectionPoint();
                port.Position = Position.Custom;
                port.OffsetX = ptPortLocation.X;
                port.OffsetY = ptPortLocation.Y;
                node.Ports.Add(port);
            }
        }

        #endregion Connections Processing Helper Methods

        #region Geom Processing Helper Methods

        private void ProcessGeom(XmlNode nodeGeom)
        {
            if (nodeGeom.HasChildNodes)
            {
                // Must do it before processing other nodes
                if (ShowGeom(nodeGeom))
                {
                    foreach (XmlNode curNode in nodeGeom.ChildNodes)
                    {
                        switch (curNode.LocalName.ToLower())
                        {
                            case "nofill":
                                ProcessStyle(curNode, new HelperFillStyle(), null, 0);
                                break;
                            case "noline":
                                // Shape node needed
                                ProcessStyle(curNode, new HelperLineStyle(), null, 0);
                                break;
                            default: break;
                        }
                    }
                }
            }
        }

        private bool ShowGeom(XmlNode nodeGeom)
        {
            bool bSuccess = true;

            XmlNode nodeNoShow = SearchForSibling(nodeGeom, "NoShow", null, 0, true);
            if (nodeNoShow != null)
            {
                if (int.Parse(nodeNoShow.InnerText) == 1)
                    bSuccess = false;
            }

            return bSuccess;
        }

        private void ProcessStyle(XmlNode node, IBaseHelper hlp, string strAttributeName, int nAttrValue)
        {
            string[] str = { "Fill", "Line", "Char", "TextBlock", "Para" };
            Hashtable[] arrHash = { m_hashFillStyle, m_hashLineStyle, m_hashCharStyle, m_hashTextBlock, m_hashParaStyle };
            Type[] types = { typeof( HelperFillStyle ), typeof( HelperLineStyle ), typeof( HelperCharStyle ), typeof( HelperTextBlockStyle ), typeof( HelperParaStyle ) };
            ProcessNodeEventHandler[] evtArr = 
                   {
                   new ProcessNodeEventHandler( ProcessFillNode ), 
                   new ProcessNodeEventHandler( ProcessLineNode ), 
                   new ProcessNodeEventHandler( ProcessCharNode ),
                   new ProcessNodeEventHandler( ProcessTextBlockNode ), 
                   new ProcessNodeEventHandler( ProcessTextParaNode ) 
                   };

            // only Fill and Line styles could be appliable or not 
            if ((hlp.GetType() == typeof(HelperFillStyle)) || (hlp.GetType() == typeof(HelperLineStyle)))
            {
                string strInnerText = node.InnerText;
                if (strInnerText.Length != 0)
                {
                    hlp.Appliable(int.Parse(strInnerText));
                }
            }

            XmlNode nodeShape = node.ParentNode.ParentNode; // Shape node

            for (int i = 0; i < types.Length; i++)
            {
                if (hlp.GetType() == types[i])
                {
                    XmlNode nodeNeeded = SearchForSibling(nodeShape, str[i], strAttributeName, nAttrValue, true);
                    if (nodeNeeded != null)
                    {
                        evtArr[i](nodeNeeded, hlp);
                    }
                    else
                    {
                        InheritFromStyleSheet(nodeShape, ref hlp, arrHash[i]);
                    }

                    hlp.ApplyStyle(m_nodeTemp);
                    break;
                }
            }
        }

        private void InheritFromStyleSheet(XmlNode nodeShape, ref IBaseHelper hlp, Hashtable hash)
        {
            int nStyleSheetIdToInheritFrom = -1;
            if (GetAttribute(nodeShape, ref nStyleSheetIdToInheritFrom, hlp.SearchFor))
            {
                Inherit(hash, nStyleSheetIdToInheritFrom, ref hlp);
            }
        }

        private void Inherit(Hashtable hashToInherit, int nID, ref IBaseHelper hlp)
        {
            do
            {
                nID = hlp.Inherit(SearchForHelperToInheritFrom(hashToInherit, nID));
            }
            while (nID >= 0);
        }

        private IBaseHelper SearchForHelperToInheritFrom(Hashtable hashSearchIn, int nID)
        {
            IBaseHelper hlpBase = null;
            if (hashSearchIn.ContainsKey(nID))
            {
                hlpBase = (IBaseHelper)hashSearchIn[nID];
            }
            else
            {
                // if this happens we return style from StyleSheet with ID = 0 cause
                // all styles are defined in StyleSheet with ID = 0, so every hashTable WILL contain Key == 0
                hlpBase = (IBaseHelper)hashSearchIn[0];
            }
            return hlpBase;
        }

        private XmlNode SearchForSibling(XmlNode nodeToSearch, string strName, string strAttributeName, int nAttrValue, bool bCaseSensitive)
        {
            bool bSuccess = false;
            XmlNode node = null;
            if (nodeToSearch.HasChildNodes)
            {
                node = nodeToSearch.FirstChild;
                while (node != null)
                {
                    string strNodeName = null;
                    if (bCaseSensitive)
                        strNodeName = node.LocalName;
                    else
                        strNodeName = node.LocalName.ToLower();

                    if (strNodeName == strName)
                    {
                        // we can search for needed sibling by attribute
                        if (strAttributeName != null)
                        {
                            if (nAttrValue == int.Parse(((XmlElement)node).GetAttribute(strAttributeName)))
                            {
                                bSuccess = true;
                                break;
                            }
                        }
                        else
                        {
                            bSuccess = true;
                            break;
                        }
                    }
                    node = node.NextSibling;
                }
            }
            if (!bSuccess)
            {
                node = null;
            }
            return node;
        }

        #endregion Geom Processing Helper Methods

        private void CreateBitmapNode(Bitmap img, Group parent, XForm xfShapeProp)
        {
            RectangleF rect = new RectangleF(0, 0, xfShapeProp.Width, xfShapeProp.Height);
            m_nodeTemp = new BitmapNode(img, rect);

            float height = (parent.Size.Height < Math.Abs(xfShapeProp.PinY)) ? Math.Abs(xfShapeProp.PinY) : parent.Size.Height;
            float pinx = xfShapeProp.PinX;
            float piny = height - xfShapeProp.PinY;
            float locpinx = xfShapeProp.LocPinX - rect.X;
            float locpiny = xfShapeProp.Height - xfShapeProp.LocPinY;

            m_nodeTemp.PinPointOffset = new SizeF(0, 0);
            m_nodeTemp.PinPoint = new PointF(pinx, piny);
            m_nodeTemp.PinPointOffset = new SizeF(locpinx, locpiny);
            m_nodeTemp.RotationAngle = xfShapeProp.FlipY ? xfShapeProp.RotationAngle : -xfShapeProp.RotationAngle;
            m_nodeTemp.FlipX = xfShapeProp.FlipX;
            m_nodeTemp.FlipY = xfShapeProp.FlipY;
        }
        private void CreateMetafileNode(System.Drawing.Imaging.Metafile metafile, Group parent, XForm xfShapeProp)
        {
            RectangleF rect = new RectangleF(0, 0, xfShapeProp.Width, xfShapeProp.Height);
            m_nodeTemp = new MetafileNode(metafile, rect);

            float height = (parent.Size.Height < Math.Abs(xfShapeProp.PinY)) ? Math.Abs(xfShapeProp.PinY) : parent.Size.Height;
            float pinx = xfShapeProp.PinX;
            float piny = height - xfShapeProp.PinY;
            float locpinx = xfShapeProp.LocPinX - rect.X;
            float locpiny = xfShapeProp.Height - xfShapeProp.LocPinY;

            m_nodeTemp.PinPointOffset = new SizeF(0, 0);
            m_nodeTemp.PinPoint = new PointF(pinx, piny);
            m_nodeTemp.PinPointOffset = new SizeF(locpinx, locpiny);
            m_nodeTemp.RotationAngle = xfShapeProp.FlipY ? xfShapeProp.RotationAngle : -xfShapeProp.RotationAngle;
            m_nodeTemp.FlipX = xfShapeProp.FlipX;
            m_nodeTemp.FlipY = xfShapeProp.FlipY;
        }
        private Bitmap CropImage(ForeignImageInfo fii, Image imgTmp, XForm xfShapeProp)
        {
            // crop original image
            Bitmap img = new Bitmap((int)xfShapeProp.Width, (int)xfShapeProp.Height);

            using (Graphics gfx = Graphics.FromImage(img))
            {
                float fYOffset = fii.ImageHeight - (Math.Abs(fii.OffsetYToOrigin) + img.Height);

                gfx.DrawImage(
                    imgTmp, 
                    new System.Drawing.Rectangle(0, 0, img.Width, img.Height),
                    Math.Abs(fii.OffsetXToOrigin), 
                    fYOffset,
                    img.Width, 
                    img.Height, 
                    GraphicsUnit.Pixel);
            }
            return img;
        }
        private XmlNode GetNodeByName(string strNodeName, XmlNode xmlNodeParent)
        {
            XmlNode xmlNode = null;

            foreach (XmlNode node in xmlNodeParent.ChildNodes)
            {
                if (node.Name.ToLower() == strNodeName)
                {
                    xmlNode = node;
                    break;
                }
            }

            return xmlNode;
        }
        private bool HasBitmap(XmlNode nodeFrgnData)
        {
            if (nodeFrgnData == null) throw new ArgumentNullException("nodeFrgnData");

            XmlNode nodeAttr = nodeFrgnData.Attributes.GetNamedItem("ForeignType");

            return nodeAttr.Value == c_strBITMAP;
        }
        private bool HasMetafile(XmlNode nodeFrgnData)
        {
            if (nodeFrgnData == null) throw new ArgumentNullException("nodeFrgnData");

            XmlNode nodeAttr = nodeFrgnData.Attributes.GetNamedItem("ForeignType");

            return nodeAttr.Value == c_strMETAFILE || nodeAttr.Value == c_strENHMETAFILE;
        }
        private float GetRotationAngle(XmlNode nodeAngle)
        {
            float fRad = ProcessValue(nodeAngle);
            return (float)(fRad * (180 / Math.PI));
        }

        private float ProcessValue(XmlNode node)
        {
            NumberStyles ns = NumberStyles.Any;
            NumberFormatInfo nfi = new NumberFormatInfo();

            nfi.NumberDecimalDigits = c_nHEXADECIMAL;
            double doubleOut = 0;
            float fReturnValue = 0;

            string strTemp = ((XmlElement)node).InnerText;

            // Search for decimal separator and set it in NumberFormatInfo.NumberDecimalSeparator
            // to avoid CurrentCultureInfo problems
            RegexOptions regexOptions = RegexOptions.IgnoreCase;

            Match match = Regex.Match(strTemp, "[.,]", regexOptions);
            if (match.Success)
            {
                nfi.NumberDecimalSeparator = match.Value;
            }

            if (double.TryParse(strTemp, ns, nfi, out doubleOut))
            {
                fReturnValue = (float)doubleOut;
            }

            return fReturnValue;
        }

        private Color ProcessColor(XmlNode node, bool bTextBlock)
        {
            string strValue = node.InnerText;
            Color clr = Color.Empty;

            // color hexadecimal representation
            if (strValue.Length == c_nHTML_RGB_LENGTH) 
            {
                clr = ColorTranslator.FromHtml(strValue);
            }
            else 
            {
                // get color from m_lstColors 
                int nIndex = (int)ProcessValue(node);
                
                // Need to subtract 1 from nIndex if we are in TextBlock
                if (bTextBlock)
                {
                    if (nIndex == 0)
                    {
                        clr = Color.Transparent;
                    }
                    else
                    {
                        nIndex--;
                        if (m_hashColors.ContainsKey(nIndex))
                            clr = (Color)m_hashColors[nIndex];
                        else
                            clr = Color.Transparent;
                    }
                }
                else
                {
                    clr = (Color)m_hashColors[nIndex];
                }
            }

            return clr;
        }

        private float ConvertToPixels(float fValue)
        {
            float fDpi = Graphics.FromHwnd(IntPtr.Zero).DpiX;

            // save Dpi current value
            float nDpiSave = MeasureUnitsConverter.DpiX;
            
            // assign new value
			if(fDpi==0)
            MeasureUnitsConverter.DpiX = fDpi;

            float fValueToReturn = MeasureUnitsConverter.Convert(fValue, MeasureUnits.Inch, MeasureUnits.Pixel);

            // restore Dpi value
            MeasureUnitsConverter.DpiX = nDpiSave;

            return fValueToReturn;
        }

        private bool GetAttribute(XmlNode node, ref int nValue, string strAttributeName)
        {
            NumberStyles ns = NumberStyles.Any;
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalDigits = c_nHEXADECIMAL;
            double dOut = 0;
            bool bSuccess = true;

            string strTemp = ((XmlElement)node).GetAttribute(strAttributeName);

            // Search for decimal separator and set it in NumberFormatInfo.NumberDecimalSeparator
            // to avoid CurrentCultureInfo problems
            RegexOptions regexOptions = RegexOptions.IgnoreCase;

            Match match = Regex.Match(strTemp, "[.,]", regexOptions);
            if (match.Success)
            {
                nfi.NumberDecimalSeparator = match.Value;
            }

            if (strTemp.Length != 0)
            {
                if (double.TryParse(strTemp, ns, nfi, out dOut))
                    nValue = (int)dOut;
                else
                    bSuccess = false;
            }
            else
                bSuccess = false;

            return bSuccess;
        }

        #endregion Xml Processing Helper Methods

        #endregion Helper Methods

        #endregion Methods

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            m_gpTemp.Dispose();
            
            // m_paletteTemp.Dispose();
            // m_symbolTemp.Dispose();
            if (m_filesCreated.Count > 0)
            {
                for (int i = 0; i < m_filesCreated.Count; i++)
                {
                    try
                    {
                        File.Delete((string)m_filesCreated[i]);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// The VisioStencilConverter is a utility class that converts Visio stencil files (*.vss files) into Essential Diagram 
    /// symbol palettes (*.edp files). Each shape in the stencil file will have a <see cref="Syncfusion.Windows.Forms.Diagram.Group"/> 
    /// equivalent in the <see cref="SymbolPalette"/>. The symbol palette 
    /// may directly be consumed by diagramming applications, and symbols created from the symbol models.
    /// </summary>
    public class VisioStencilConverter : IDisposable
    {
        #region Fields

        private string m_strConvertFile;
        private string m_strTempDir;
        private ProgressDialog m_dlgProgress;
        private Form m_frmParent;
        private SymbolPalette m_paletteConverted = null;
        private VisioStencilConvert m_stencilConverter = null;
        private Exception m_conExp;
        private bool m_bShowProgressDialog = false;
        private bool m_bIgnoreStyles = false;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VisioStencilConverter"/> class.
        /// </summary>
        /// <param name="strFileName">The Visio stencil file.</param>
        public VisioStencilConverter(string strFileName)
        {
            this.m_strConvertFile = strFileName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisioStencilConverter"/> class.
        /// </summary>
        /// <param name="strFileName">The Visio stencil file.</param>
        /// <param name="strTempDir">Directory used to store Converer temorary files.</param>
        public VisioStencilConverter(string strFileName, string strTempDir)
        {
            m_strConvertFile = strFileName;
            m_strTempDir = strTempDir;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisioStencilConverter"/> class.
        /// </summary>
        /// <param name="strFileName">The Visio stencil file.</param>
        /// <param name="parent">The form hosting the converter.</param>
        public VisioStencilConverter(string strFileName, Form parent)
        {
            this.m_strConvertFile = strFileName;
            this.m_frmParent = parent;
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indcating whether temporary directory used by Visio converter to store temp files.
        /// </summary>
        public string TemporaryDirectory
        {
            get { return m_strTempDir; }
            set { m_strTempDir = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a progress dialog should be displayed during the conversion.
        /// </summary>
        public bool ShowProgressDialog
        {
            get
            {
                return m_bShowProgressDialog;
            }
            set
            {
                m_bShowProgressDialog = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the fill styles specified for the Visio shapes 
        /// will be applied to the diagram symbols.   
        /// </summary>
        /// <remarks>
        /// When the IgnoreStyles property is set to its default value of TRUE the Visio shape fill values will 
        /// not be extended to the diagram symbolmodel, and the symbols will retain the default fill values.
        /// </remarks>
        [DefaultValue(true)]
        public bool IgnoreStyles
        {
            get
            {
                return this.m_bIgnoreStyles;
            }
            set
            {
                this.m_bIgnoreStyles = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Converts the Visio stencil into an Essential Diagram symbol palette.
        /// </summary>
        /// <returns>The <see cref="SymbolPalette"/> instance generated from the stencil.</returns>
        public SymbolPalette Convert()
        {
            if (this.m_bShowProgressDialog && (this.m_frmParent != null))
                this.ShowProgress();

            Cursor.Current = Cursors.WaitCursor;
            Thread threadWork = new Thread(new ThreadStart(Conv));
            threadWork.Start();

            do
            {
                if (threadWork.Join(50))
                    break;
                Application.DoEvents();
            }
            while (true);

            if (m_dlgProgress != null)
                HideProgress();
            Cursor.Current = Cursors.Default;

            if (m_conExp != null)
            {
                MessageBox.Show(m_conExp.Message, "Convert Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return m_paletteConverted;
        }

        #region Helper Methods

        [Syncfusion.Documentation.DocumentationExclude()]
        private void Conv()
        {
            m_conExp = null;
            try
            {
                m_stencilConverter = new VisioStencilConvert(m_strConvertFile, this.TemporaryDirectory, m_bIgnoreStyles);
                m_paletteConverted = m_stencilConverter.Convert();
            }
            catch (COMException ex)
            {
                m_conExp = ex;
            }
        }

        #region ProgressDialog Helper methods

        [Syncfusion.Documentation.DocumentationExclude()]
        private void ShowProgress()
        {
            m_dlgProgress = new ProgressDialog();
            m_dlgProgress.Owner = this.m_frmParent;
            m_dlgProgress.Show();
            m_dlgProgress.BringToFront();
            m_dlgProgress.Cursor = Cursors.WaitCursor;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        private void HideProgress()
        {
            m_dlgProgress.Owner = null;
            m_dlgProgress.Hide();
            m_dlgProgress.Dispose();
        }

        #endregion ProgressDialog Helper methods

        #endregion Methods

        #endregion Helper Methods

        #region IDisposable Members

        /// <summary>
        /// Disposes the VisioStencilConverter.
        /// </summary>
        public void Dispose()
        {
            m_stencilConverter.Dispose();
        }

        #endregion
    }

    #region Helper Classes

    /// <summary>
    /// Base helper interface.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public interface IBaseHelper
    {
        /// <summary>
        /// Gets or sets the inherited value.
        /// </summary>
        int InheritedFrom
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the search for.
        /// </summary>
        string SearchFor
        {
            get;
        }

        /// <summary>
        /// Inherits the specified base helper.
        /// </summary>
        /// <param name="hlp">The base helper.</param>
        /// <returns>The value.</returns>
        int Inherit(IBaseHelper hlp);

        /// <summary>
        /// Applies the style.
        /// </summary>
        /// <param name="node">The node.</param>
        void ApplyStyle(INode node);

        /// <summary>
        /// Applies the specified appliable one.
        /// </summary>
        /// <param name="nAppliable">Appilable one</param>
        void Appliable(int nAppliable);
    }

    /// <summary>
    /// Line Style helper
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class HelperLineStyle : IBaseHelper
    {
        #region Fields

        public int m_nInheritedFrom;

        private bool m_bNoLine = false;
        private Color m_clrLineColor;
        private float m_fLineWeight;
        private int m_nLinePattern;
        private int m_nLineCap; // 0 - Rounded ; 1 - Square ends
        private float m_fTransparency;
        private string m_strSearchFor = "LineStyle";

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HelperLineStyle"/> class.
        /// </summary>
        public HelperLineStyle()
        {
            m_nInheritedFrom = -1;

            m_clrLineColor = Color.Empty;
            m_fLineWeight = -1;
            m_nLinePattern = -1;
            m_nLineCap = -1;
            m_fTransparency = -1;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether no line should be drawn.
        /// </summary>
        /// <value><c>true</c> if no line; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public bool NoLine
        {
            get
            {
                return m_bNoLine;
            }
            set
            {
                m_bNoLine = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the line.
        /// </summary>
        /// <value>The color of the line.</value>
        public Color LineColor
        {
            get
            {
                return m_clrLineColor;
            }
            set
            {
                if (m_clrLineColor != value)
                    m_clrLineColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the line weight.
        /// </summary>
        /// <value>The line weight.</value>
        [DefaultValue(-1)]
        public float LineWeight
        {
            get
            {
                return m_fLineWeight;
            }
            set
            {
                if (m_fLineWeight != value)
                    m_fLineWeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the line pattern.
        /// </summary>
        /// <value>The line pattern.</value>
        [DefaultValue(-1)]
        public int LinePattern
        {
            get
            {
                return m_nLinePattern;
            }
            set
            {
                if (m_nLinePattern != value)
                    m_nLinePattern = value;
            }
        }

        /// <summary>
        /// Gets or sets the line cap.
        /// </summary>
        /// <value>The line cap.</value>
        [DefaultValue(-1)]
        public int LineCap
        {
            get
            {
                return m_nLineCap;
            }
            set
            {
                if (m_nLineCap != value)
                    m_nLineCap = value;
            }
        }

        /// <summary>
        /// Gets or sets the transparency.
        /// </summary>
        /// <value>The transparency.</value>
        public float Transparency
        {
            get
            {
                return m_fTransparency;
            }
            set
            {
                if (m_fTransparency != value)
                    m_fTransparency = value;
            }
        }

        #endregion Properties

        #region IBaseHelper

        #region Properties

        /// <summary>
        /// Gets the search for.
        /// </summary>
        /// <value></value>
        public string SearchFor
        {
            get
            {
                return m_strSearchFor;
            }
        }

        /// <summary>
        /// Gets or sets the inherited value.
        /// </summary>
        /// <value></value>
        public int InheritedFrom
        {
            get
            {
                return m_nInheritedFrom;
            }
            set
            {
                if (m_nInheritedFrom != value)
                    m_nInheritedFrom = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Returns StyleSheet ID to inherit from
        /// </summary>
        /// <param name="hlpFromm">The base helper.</param>
        /// <returns>The helper value.</returns>
        public int Inherit(IBaseHelper hlpFromm)
        {
            HelperLineStyle hlpFrom = hlpFromm as HelperLineStyle;

            if (m_clrLineColor == Color.Empty)
                m_clrLineColor = hlpFrom.m_clrLineColor;
            if (m_nLinePattern == -1)
                m_nLinePattern = hlpFrom.m_nLinePattern;
            if (m_fLineWeight == -1)
                m_fLineWeight = hlpFrom.m_fLineWeight;
            if (m_fTransparency == -1)
                m_fTransparency = hlpFrom.m_fTransparency;
            if (m_nLineCap == -1)
                m_nLineCap = hlpFrom.m_nLineCap;

            return hlpFrom.InheritedFrom;
        }

        /// <summary>
        /// Applies the style.
        /// </summary>
        /// <param name="node">The node.</param>
        public void ApplyStyle(INode node)
        {
            FilledPath fig = node as FilledPath;
            if (m_nLinePattern == 0)
            {
                if (NoLine)
                    fig.LineStyle.LineColor = Color.Transparent;
                else
                    fig.LineStyle.LineColor = m_clrLineColor;
            }
            else
                fig.LineStyle.LineColor = m_clrLineColor;

            float fDpi = Graphics.FromHwnd(IntPtr.Zero).DpiX;

            // save Dpi current value
            float nDpiSave = MeasureUnitsConverter.DpiX;
            
            // assign new value
			if(fDpi==0)
            MeasureUnitsConverter.DpiX = fDpi;

            fig.LineStyle.LineWidth = MeasureUnitsConverter.Convert(m_fLineWeight, MeasureUnits.Inch, MeasureUnits.Pixel);

            // restore previous value
            MeasureUnitsConverter.DpiX = nDpiSave;

            if (m_nLineCap == 0)
            {
                fig.LineStyle.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            }
            else
            {
                fig.LineStyle.EndCap = System.Drawing.Drawing2D.LineCap.Square;
            }
        }

        /// <summary>
        /// Appliables the specified appliable one.
        /// </summary>
        /// <param name="nAppliable">Appliable one.</param>
        public void Appliable(int nAppliable)
        {
            if (nAppliable == 1)
            {
                NoLine = true;
            }
        }

        #endregion Methods

        #endregion IBaseHelper
    }

    /// <summary>
    /// Fill Style Helper.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class HelperFillStyle : IBaseHelper
    {
        #region Fields

        private const int COMPLETELY_OPAQUE = 255;

        private Color m_clrFillBkgnd;
        private Color m_clrFillForegnd;
        private int m_nFillPattern;
        private float m_fFillForegndTransparency;
        private float m_fFillBkgndTransparency;

        private bool m_bNoFill;
        private Color m_clrShdwBkgnd;
        private Color m_clrShdwForegnd;
        private int m_nShdwPattern;
        private int m_nShdwOffsetX;
        private int m_nShdwOffsetY;
        private float m_nShdwForegndTrans;
        private float m_nShdwBkgndTrans;

        private int m_nInheritedFrom;
        private string m_strSearchFor = "FillStyle";

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HelperFillStyle"/> class.
        /// </summary>
        public HelperFillStyle()
        {
            m_clrFillBkgnd = Color.Empty;
            m_clrFillForegnd = Color.Empty;
            m_nFillPattern = -1;
            m_fFillForegndTransparency = -1;
            m_fFillBkgndTransparency = -1;

            m_clrShdwBkgnd = Color.Empty;
            m_clrShdwForegnd = Color.Empty;
            m_nShdwPattern = -1;
            m_nShdwOffsetX = -1;
            m_nShdwOffsetY = -1;
            m_nShdwBkgndTrans = 0;
            m_nShdwForegndTrans = 0;

            m_nInheritedFrom = -1;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether fill is needed.
        /// </summary>
        /// <value><c>true</c> if no fill; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public bool NoFill
        {
            get
            {
                return m_bNoFill;
            }
            set
            {
                m_bNoFill = value;
            }
        }

        /// <summary>
        /// Gets or sets the fill pattern.
        /// </summary>
        /// <value>The fill pattern.</value>
        [DefaultValue(-1)]
        public int FillPattern
        {
            get
            {
                return m_nFillPattern;
            }
            set
            {
                if (m_nFillPattern != value)
                    m_nFillPattern = value;
            }
        }

        /// <summary>
        /// Gets or sets the fore color transparency.
        /// </summary>
        /// <value>The fore color transparency.</value>
        [DefaultValue(-1)]
        public float ForeColorTransparency
        {
            get
            {
                return m_fFillForegndTransparency;
            }
            set
            {
                if (m_fFillForegndTransparency != value)
                    m_fFillForegndTransparency = value;
            }
        }

        /// <summary>
        /// Gets or sets the back color transparency.
        /// </summary>
        /// <value>The back color transparency.</value>
        [DefaultValue(-1)]
        public float BackColorTransparency
        {
            get
            {
                return m_fFillBkgndTransparency;
            }
            set
            {
                if (m_fFillBkgndTransparency != value)
                    m_fFillBkgndTransparency = value;
            }
        }

        /// <summary>
        /// Gets or sets the shadow x offset.
        /// </summary>
        /// <value>The shadow offset X.</value>
        [DefaultValue(-1)]
        public int ShdwOffsetX
        {
            get
            {
                return m_nShdwOffsetX;
            }
            set
            {
                if (m_nShdwOffsetX != value)
                    m_nShdwOffsetX = value;
            }
        }

        /// <summary>
        /// Gets or sets the shadow y offset.
        /// </summary>
        /// <value>The shadow offset Y.</value>
        [DefaultValue(-1)]
        public int ShdwOffsetY
        {
            get
            {
                return m_nShdwOffsetY;
            }
            set
            {
                if (m_nShdwOffsetY != value)
                    m_nShdwOffsetY = value;
            }
        }

        /// <summary>
        /// Gets or sets the shadow pattern.
        /// </summary>
        /// <value>The shadow pattern.</value>
        [DefaultValue(-1)]
        public int ShdwPattern
        {
            get
            {
                return m_nShdwPattern;
            }
            set
            {
                if (m_nShdwPattern != value)
                    m_nShdwPattern = value;
            }
        }

        /// <summary>
        /// Gets or sets the fill background.
        /// </summary>
        /// <value>The fill background.</value>
        public Color FillBkgnd
        {
            get
            {
                return m_clrFillBkgnd;
            }
            set
            {
                if (m_clrFillBkgnd != value)
                    m_clrFillBkgnd = value;
            }
        }

        /// <summary>
        /// Gets or sets the fill foreground.
        /// </summary>
        /// <value>The fill foreground.</value>
        public Color FillForegnd
        {
            get
            {
                return m_clrFillForegnd;
            }
            set
            {
                if (m_clrFillForegnd != value)
                    m_clrFillForegnd = value;
            }
        }

        /// <summary>
        /// Gets or sets the shadow background.
        /// </summary>
        /// <value>The shadow background.</value>
        public Color ShdwBkgnd
        {
            get
            {
                return m_clrShdwBkgnd;
            }
            set
            {
                if (m_clrShdwBkgnd != value)
                    m_clrShdwBkgnd = value;
            }
        }

        /// <summary>
        /// Gets or sets the shadow foreground.
        /// </summary>
        /// <value>The shadow foreground.</value>
        public Color ShdwForegnd
        {
            get
            {
                return m_clrShdwForegnd;
            }
            set
            {
                if (m_clrShdwForegnd != value)
                    m_clrShdwForegnd = value;
            }
        }

        /// <summary>
        /// Gets or sets the shadow foregnd transformation.
        /// </summary>
        /// <value>The SHDshadowW foreground transformation.</value>
        [DefaultValue(0)]
        public float ShdwForegndTrans
        {
            get
            {
                return m_nShdwForegndTrans;
            }
            set
            {
                if (m_nShdwForegndTrans != value)
                    m_nShdwForegndTrans = value;
            }
        }

        /// <summary>
        /// Gets or sets the shadow background transformation.
        /// </summary>
        [DefaultValue(0)]
        public float ShdwBkgndTrans
        {
            get
            {
                return m_nShdwBkgndTrans;
            }
            set
            {
                if (m_nShdwBkgndTrans != value)
                    m_nShdwBkgndTrans = value;
            }
        }

        #endregion Properties

        #region IBaseHelper

        #region Properties

        /// <summary>
        /// Gets the search.
        /// </summary>
        /// <value></value>
        public string SearchFor
        {
            get
            {
                return m_strSearchFor;
            }
        }

        /// <summary>
        /// Gets or sets the inherited value.
        /// </summary>
        /// <value></value>
        public int InheritedFrom
        {
            get
            {
                return m_nInheritedFrom;
            }
            set
            {
                if (m_nInheritedFrom != value)
                    m_nInheritedFrom = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Returns StyleSheet ID to inherit from
        /// </summary>
        /// <param name="hlpFromm">The base helper.</param>
        /// <returns>The helper value.</returns>
        public int Inherit(IBaseHelper hlpFromm)
        {
            HelperFillStyle hlpFrom = hlpFromm as HelperFillStyle;

            if (m_clrFillBkgnd == Color.Empty)
                m_clrFillBkgnd = hlpFrom.m_clrFillBkgnd;
            if (m_clrFillForegnd == Color.Empty)
                m_clrFillForegnd = hlpFrom.m_clrFillForegnd;
            if (m_fFillBkgndTransparency == -1)
                m_fFillBkgndTransparency = hlpFrom.m_fFillBkgndTransparency;
            if (m_fFillForegndTransparency == -1)
                m_fFillForegndTransparency = hlpFrom.m_fFillForegndTransparency;
            if (m_nFillPattern == -1)
                m_nFillPattern = hlpFrom.m_nFillPattern;
            if (m_clrShdwBkgnd == Color.Empty)
                m_clrShdwBkgnd = hlpFrom.m_clrShdwBkgnd;
            if (m_clrShdwForegnd == Color.Empty)
                m_clrShdwForegnd = hlpFrom.m_clrShdwForegnd;
            if (m_nShdwOffsetX == -1)
                m_nShdwOffsetX = hlpFrom.m_nShdwOffsetX;
            if (m_nShdwOffsetY == -1)
                m_nShdwOffsetY = hlpFrom.m_nShdwOffsetY;
            if (m_nShdwPattern == -1)
                m_nShdwPattern = hlpFrom.m_nShdwPattern;

            return hlpFrom.InheritedFrom;
        }

        /// <summary>
        /// Applies the style.
        /// </summary>
        /// <param name="node">The node.</param>
        public void ApplyStyle(INode node)
        {
            FilledPath fig = node as FilledPath;
            if (fig == null)
                throw new ArgumentException("FilledPath expected.");

            if (NoFill)
            {
                fig.FillStyle.Color = Color.Transparent;
            }
            else
            {
                switch (FillPattern)
                {
                    case 1:
                        fig.FillStyle.Color = m_clrFillForegnd;
                        fig.FillStyle.Type = FillStyleType.Solid;
                        break;
                    case 2:
                        UseHatchBrush(fig, HatchStyle.BackwardDiagonal, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 3:
                        UseHatchBrush(fig, HatchStyle.Cross, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 4:
                        UseHatchBrush(fig, HatchStyle.DiagonalCross, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 5:
                        UseHatchBrush(fig, HatchStyle.ForwardDiagonal, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 6:
                        UseHatchBrush(fig, HatchStyle.Horizontal, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 7:
                        UseHatchBrush(fig, HatchStyle.Vertical, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 8:
                        UseHatchBrush(fig, HatchStyle.Percent70, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 9:
                        UseHatchBrush(fig, HatchStyle.Percent50, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 10:
                        UseHatchBrush(fig, HatchStyle.Percent25, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 11:
                        UseHatchBrush(fig, HatchStyle.Percent20, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 12:
                        UseHatchBrush(fig, HatchStyle.Percent10, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 13:
                        UseHatchBrush(fig, HatchStyle.DarkHorizontal, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 14:
                        UseHatchBrush(fig, HatchStyle.DarkVertical, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 15:
                        UseHatchBrush(fig, HatchStyle.DarkDownwardDiagonal, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 16:
                        UseHatchBrush(fig, HatchStyle.DarkUpwardDiagonal, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 17:
                        UseHatchBrush(fig, HatchStyle.SmallCheckerBoard, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 18:
                        UseHatchBrush(fig, HatchStyle.Trellis, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 19:
                        UseHatchBrush(fig, HatchStyle.LightHorizontal, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 20:
                        UseHatchBrush(fig, HatchStyle.LightVertical, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 21:
                        UseHatchBrush(fig, HatchStyle.LightDownwardDiagonal, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 22:
                        UseHatchBrush(fig, HatchStyle.LightUpwardDiagonal, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 23:
                        UseHatchBrush(fig, HatchStyle.SmallGrid, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 24:
                        UseHatchBrush(fig, HatchStyle.Percent60, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 25:
                        UseLinearGradientBrush(fig, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency, 1, 0);
                        break;
                    case 26:
                        UseLinearGradientBrush(fig, FillBkgnd, BackColorTransparency, FillForegnd, ForeColorTransparency, 0.5f, 0);
                        break;
                    case 27:
                        UseLinearGradientBrush(fig, FillBkgnd, BackColorTransparency, FillForegnd, ForeColorTransparency, 1, 0);
                        break;
                    case 28:
                        UseLinearGradientBrush(fig, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency, 1, 90);
                        break;
                    case 29:
                        UseLinearGradientBrush(fig, FillBkgnd, BackColorTransparency, FillForegnd, ForeColorTransparency, 0.5f, 90);
                        break;
                    case 30:
                        UseLinearGradientBrush(fig, FillBkgnd, BackColorTransparency, FillForegnd, ForeColorTransparency, 1, 90);
                        break;
                    case 31:
                        UsePathGradientBrush(fig, PathGradientBrushStyle.RectangleLeftTop, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 32:
                        UsePathGradientBrush(fig, PathGradientBrushStyle.RectangleRightTop, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 33:
                        UsePathGradientBrush(fig, PathGradientBrushStyle.RectangleLeftBottom, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 34:
                        UsePathGradientBrush(fig, PathGradientBrushStyle.RectangleRightBottom, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 35:
                        UsePathGradientBrush(fig, PathGradientBrushStyle.RectangleCenter, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 36:
                        UsePathGradientBrush(fig, PathGradientBrushStyle.CircleLeftTop, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 37:
                        UsePathGradientBrush(fig, PathGradientBrushStyle.CircleRightTop, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 38:
                        UsePathGradientBrush(fig, PathGradientBrushStyle.CircleLeftBottom, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 39:
                        UsePathGradientBrush(fig, PathGradientBrushStyle.CircleRightBottom, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    case 40:
                        UsePathGradientBrush(fig, PathGradientBrushStyle.CircleCenter, FillForegnd, ForeColorTransparency, FillBkgnd, BackColorTransparency);
                        break;
                    default: fig.FillStyle.Color = Color.Transparent;
                        break;
                }
            }

            ApplyShadowStyle(node as Node);
        }

        /// <summary>
        /// Appliables the specified appliable one.
        /// </summary>
        /// <param name="nAppliable">Appliable one.</param>
        public void Appliable(int nAppliable)
        {
            if (nAppliable == 1)
            {
                NoFill = true;
            }
        }

        private void UsePathGradientBrush(INode node, PathGradientBrushStyle pathStyle, Color clrFore, float nForeTransparency, Color clrBack, float nBackTransparency)
        {
            FilledPath fig = node as FilledPath;
            if (fig == null)
                throw new ArgumentException("FilledPath expected.");

            int nForeColorTransparency = ConvertTransparencyValue(nForeTransparency);
            int nBackColorTransparency = ConvertTransparencyValue(nBackTransparency);

            fig.FillStyle.Type = FillStyleType.PathGradient;

            fig.FillStyle.PathBrushStyle = pathStyle;
            fig.FillStyle.Color = clrBack;
            fig.FillStyle.ColorAlphaFactor = nBackColorTransparency;
            fig.FillStyle.ForeColor = clrFore;
            fig.FillStyle.ForeColorAlphaFactor = nForeColorTransparency;
        }

        private void UseLinearGradientBrush(INode node, Color clrFore, float nForeTransparency, Color clrBack, float nBackTransparency, float fGradientCenter, float fAngle)
        {
            FilledPath fig = node as FilledPath;
            if (fig == null)
                throw new ArgumentException("FilledPath expected.");

            int nForeColorTransparency = ConvertTransparencyValue(nForeTransparency);
            int nBackColorTransparency = ConvertTransparencyValue(nBackTransparency);

            fig.FillStyle.Type = FillStyleType.LinearGradient;
            fig.FillStyle.Color = clrBack;
            fig.FillStyle.ColorAlphaFactor = nBackColorTransparency;
            fig.FillStyle.ForeColor = clrFore;
            fig.FillStyle.ForeColorAlphaFactor = nForeColorTransparency;
            fig.FillStyle.GradientCenter = fGradientCenter;
            fig.FillStyle.GradientAngle = fAngle;
        }

        private void UseHatchBrush(INode node, HatchStyle style, Color clrFore, float nForeTransparency, Color clrBack, float nBackTransparency)
        {
            FilledPath fig = node as FilledPath;
            if (fig == null)
                throw new ArgumentException("FilledPath expected.");

            int nForeColorTransparency = ConvertTransparencyValue(nForeTransparency);
            int nBackColorTransparency = ConvertTransparencyValue(nBackTransparency);

            fig.FillStyle.Type = FillStyleType.Hatch;
            fig.FillStyle.HatchBrushStyle = style;
            fig.FillStyle.Color = clrBack;
            fig.FillStyle.ColorAlphaFactor = nBackColorTransparency;
            fig.FillStyle.ForeColor = clrFore;
            fig.FillStyle.ForeColorAlphaFactor = nForeColorTransparency;
        }

        private int ConvertTransparencyValue(float fTransparency)
        {
            return (int)(COMPLETELY_OPAQUE * (1 - fTransparency));
        }

        #endregion Methods

        /// <summary>
        /// Update the node shadow.
        /// </summary>
        /// <param name="node">The node.</param>
        private void ApplyShadowStyle(Node node)
        {
            if (node == null)
                return;

            if (this.ShdwPattern > 0)
            {
                ShadowStyle shadow = node.ShadowStyle;

                shadow.Visible = true;
                shadow.OffsetX = this.ShdwOffsetX;
                shadow.OffsetY = -this.ShdwOffsetY;
                shadow.Color = this.ShdwBkgnd;
                shadow.ForeColor = this.ShdwForegnd;
                shadow.ColorAlphaFactor = (int)ConvertTransparencyValue(this.ShdwBkgndTrans);
                shadow.ForeColorAlphaFactor = (int)ConvertTransparencyValue(this.ShdwForegndTrans);

                // SetPatternStryle( shadow, this.ShdwPattern );
            }
        }
        #endregion IBaseHelper
    }

    /// <summary>
    /// Text Block Style Helper.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class HelperTextBlockStyle : IBaseHelper
    {
        #region Fields

        private string m_strSearchFor = "TextStyle";
        private int m_nInheritedFrom;

        // TextBlock
        private Color m_clrTextBkgnd;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HelperTextBlockStyle"/> class.
        /// </summary>
        public HelperTextBlockStyle()
        {
            m_nInheritedFrom = -1;
            m_clrTextBkgnd = Color.Empty;
        }

        #endregion Constructors

        #region TextBlock

        /// <summary>
        /// Gets or sets the text background.
        /// </summary>
        /// <value>The text background.</value>
        public Color TextBackground
        {
            get
            {
                return m_clrTextBkgnd;
            }
            set
            {
                if (m_clrTextBkgnd != value)
                    m_clrTextBkgnd = value;
            }
        }

        #endregion TextBlock

        #region IBaseHelper

        #region Properties

        /// <summary>
        /// Gets the search.
        /// </summary>
        /// <value></value>
        public string SearchFor
        {
            get
            {
                return m_strSearchFor;
            }
        }

        /// <summary>
        /// Gets or sets the inherited value.
        /// </summary>
        /// <value></value>
        public int InheritedFrom
        {
            get
            {
                return m_nInheritedFrom;
            }
            set
            {
                if (m_nInheritedFrom != value)
                    m_nInheritedFrom = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Appliables the specified appliable.
        /// </summary>
        /// <param name="Appliable">The appliable.</param>
        public void Appliable(int Appliable)
        {
        }

        /// <summary>
        /// Applies the style.
        /// </summary>
        /// <param name="node">The node.</param>
        public void ApplyStyle(INode node)
        {
            RichTextNode rtnRichText = node as RichTextNode;
            rtnRichText.BackgroundColor = m_clrTextBkgnd;
        }

        /// <summary>
        /// Inherits the specified base helper.
        /// </summary>
        /// <param name="hlpFromm">The base helper.</param>
        /// <returns>Base helper value.</returns>
        public int Inherit(IBaseHelper hlpFromm)
        {
            HelperTextBlockStyle hlpFrom = hlpFromm as HelperTextBlockStyle;

            if (m_clrTextBkgnd == Color.Empty)
                m_clrTextBkgnd = hlpFrom.m_clrTextBkgnd;

            return hlpFrom.InheritedFrom;
        }

        #endregion Methods

        #endregion IBaseHelper
    }

    /// <summary>
    /// Chart style helper
    /// </summary>
    /// <remarks>
    /// used in two cases -- first : to populate HelperCharStyle Hashtable from StyleSheets ( .ctor() )
    /// second -- to apply styles ( .ctor( ref RichTextBoxAdv, Hashtable ) )
    /// </remarks>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class HelperCharStyle : IBaseHelper
    {
        #region Constants

        [Flags]
        private enum FontWeight : short
        {
            FW_DONTCARE = 0,
            FW_THIN = 100,
            FW_EXTRALIGHT = 200,
            FW_LIGHT = 300,
            FW_NORMAL = 400,
            FW_MEDIUM = 500,
            FW_SEMIBOLD = 600,
            FW_BOLD = 700,
            FW_EXTRABOLD = 800,
            FW_HEAVY = 900,
        }

        private const int CFE_STRIKEOUT = 8; // 0x0008
        private const int CFE_BOLD = 1; // 0x0001
        private const int CFE_ITALIC = 2; // 0x0002
        private const int CFE_UNDERLINE = 4; // 0x0004
        private const int CFE_SMALLCAPS = 64; // 0x0040
        private const int CFE_SUBSCRIPT = 65536; // 0x10000 
        private const int CFE_SUPERSCRIPT = 131072; // 0x20000 
        private const int CFU_UNDERLINEDOUBLE = 3; // 0x0003
        private const int CFM_SUPERSCRIPT = 196608; // 0x30000
        private const int CFM_SUBSCRIPT = 196608; // 0x30000

        private const int EM_SETCHARFORMAT = 1092; // 0x0444 
        private const int SCF_SELECTION = 1; // 0x0001 

        #endregion Constants

        #region Fields

        private string m_strSearchFor = "TextStyle";
        private int m_nInheritedFrom;

        private RichTextBoxAdv m_rtfTemp;
        private Hashtable m_hashFaceNames;

        private int m_nFontID;
        private Color m_clrTextColor;
        private float m_fFontSize;
        private int m_nLangID;
        private CharFontStyle m_enumStyle;
        private int m_nCase;
        private int m_nRTL;
        private int m_nStrikeThrough;
        private int m_nDoubleUnderline;
        private int m_nVertical;
        private int m_nSpacing;
        private int m_nPosition;
        private float m_fFontScale;        

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HelperCharStyle"/> class.
        /// </summary>
        public HelperCharStyle()
        {
            m_nInheritedFrom = -1;

            m_rtfTemp = null;
            m_hashFaceNames = null;
            
            // Char
            m_fFontScale = -1;
            m_nPosition = -1;
            m_nSpacing = -1;
            m_nFontID = -1;
            m_clrTextColor = Color.Empty;
            m_fFontSize = -1;
            m_nLangID = -1;
            m_nRTL = -1;
            m_nStrikeThrough = -1;
            m_nDoubleUnderline = -1;
            m_nVertical = -1;
            m_enumStyle = CharFontStyle.Undefined;
            m_nCase = -1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HelperCharStyle"/> class.
        /// </summary>
        /// <param name="rtfTemp">The RTF.</param>
        /// <param name="hashFaceNames">The hash face names.</param>
        public HelperCharStyle(ref RichTextBoxAdv rtfTemp, Hashtable hashFaceNames)
            : this()
        {
            m_rtfTemp = rtfTemp;
            m_hashFaceNames = hashFaceNames;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the font ID.
        /// </summary>
        /// <value>The font ID.</value>
        [DefaultValue(-1)]
        public int FontID
        {
            get
            {
                return m_nFontID;
            }
            set
            {
                if (m_nFontID != value)
                    m_nFontID = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        /// <value>The color of the text.</value>
        public Color TextColor
        {
            get
            {
                return m_clrTextColor;
            }
            set
            {
                if (m_clrTextColor != value)
                    m_clrTextColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the text.
        /// </summary>
        /// <value>The size of the text.</value>
        [DefaultValue(-1)]
        public float TextSize
        {
            get
            {
                return m_fFontSize;
            }
            set
            {
                if (m_fFontSize != value)
                    m_fFontSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the lang ID.
        /// </summary>
        /// <value>The lang ID.</value>
        [DefaultValue(-1)]
        public int LangID
        {
            get
            {
                return m_nLangID;
            }
            set
            {
                if (m_nLangID != value)
                    m_nLangID = value;
            }
        }

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>The style.</value>
        [DefaultValue(-1)]
        public CharFontStyle Style
        {
            get
            {
                return m_enumStyle;
            }
            set
            {
                if (m_enumStyle != value)
                    m_enumStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets the case.
        /// </summary>
        /// <value>The case.</value>
        [DefaultValue(-1)]
        public int Case
        {
            get
            {
                return m_nCase;
            }
            set
            {
                if (m_nCase != value)
                    m_nCase = value;
            }
        }

        /// <summary>
        /// Gets or sets the RTL.
        /// </summary>
        /// <value>The RTL.</value>
        [DefaultValue(-1)]
        public int RTL
        {
            get
            {
                return m_nRTL;
            }
            set
            {
                if (m_nRTL != value)
                    m_nRTL = value;
            }
        }

        /// <summary>
        /// Gets or sets the strike through.
        /// </summary>
        /// <value>The strike through.</value>
        [DefaultValue(-1)]
        public int StrikeThrough
        {
            get
            {
                return m_nStrikeThrough;
            }
            set
            {
                if (m_nStrikeThrough != value)
                    m_nStrikeThrough = value;
            }
        }

        /// <summary>
        /// Gets or sets the double underline.
        /// </summary>
        /// <value>The double underline.</value>
        [DefaultValue(-1)]
        public int DoubleUnderline
        {
            get
            {
                return m_nDoubleUnderline;
            }
            set
            {
                if (m_nDoubleUnderline != value)
                    m_nDoubleUnderline = value;
            }
        }

        /// <summary>
        /// Gets or sets the use vertical.
        /// </summary>
        /// <value>The use vertical.</value>
        [DefaultValue(-1)]
        public int UseVertical
        {
            get
            {
                return m_nVertical;
            }
            set
            {
                if (m_nVertical != value)
                    m_nVertical = value;
            }
        }

        /// <summary>
        /// Gets or sets the spacing.
        /// </summary>
        /// <value>The spacing.</value>
        [DefaultValue(-1)]
        public int Spacing
        {
            get
            {
                return m_nSpacing;
            }
            set
            {
                if (m_nSpacing != value)
                    m_nSpacing = value;
            }
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        [DefaultValue(-1)]
        public int Position
        {
            get
            {
                return m_nPosition;
            }
            set
            {
                if (m_nPosition != value)
                    m_nPosition = value;
            }
        }

        /// <summary>
        /// Gets or sets the font scale.
        /// </summary>
        /// <value>The font scale.</value>
        [DefaultValue(-1)]
        public float FontScale
        {
            get
            {
                return m_fFontScale;
            }
            set
            {
                if (m_fFontScale != value)
                    m_fFontScale = value;
            }
        }

        #endregion Properties

        #region IBaseHelper

        #region Properties

        /// <summary>
        /// Gets the search.
        /// </summary>
        /// <value></value>
        public string SearchFor
        {
            get
            {
                return m_strSearchFor;
            }
        }

        /// <summary>
        /// Gets or sets the inherited value.
        /// </summary>
        /// <value></value>
        public int InheritedFrom
        {
            get
            {
                return m_nInheritedFrom;
            }
            set
            {
                if (m_nInheritedFrom != value)
                    m_nInheritedFrom = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Appliables the specified appliable.
        /// </summary>
        /// <param name="Appliable">The appliable.</param>
        public void Appliable(int Appliable)
        {
        }

        /// <summary>
        /// Applies the style.
        /// </summary>
        /// <param name="node">The node.</param>
        public void ApplyStyle(INode node)
        {
            if (m_rtfTemp == null || m_hashFaceNames == null)
                throw new MemberAccessException("You must initialize member before acccessing.");

            FontFamily fntFamily;
            RichTextNativeMethods.CHARFORMAT2 chFormat = new RichTextNativeMethods.CHARFORMAT2();
            chFormat.cbSize = Marshal.SizeOf(chFormat);

            // RichTextNativeMethods.RichEditFormat rtfMask;
            if (m_clrTextColor != Color.Empty)
            {
                chFormat.dwMask |= (uint)RichTextNativeMethods.RichEditFormat.CFM_COLOR;
                
                // transparency is not supported
                // convert in right order ( COLORREF is 0x00bbggrr )
                chFormat.crTextColor = Color.FromArgb(0, m_clrTextColor.B, m_clrTextColor.G, m_clrTextColor.R).ToArgb();
            }

            if (m_nLangID > 0)
            {
                chFormat.dwMask |= (uint)RichTextNativeMethods.RichEditFormat.CFM_LCID;
                chFormat.LCID = m_nLangID;
            }

            if (m_fFontSize > 0)
            {
                chFormat.dwMask |= (uint)RichTextNativeMethods.RichEditFormat.CFM_SIZE;
                chFormat.yHeight = (int)(m_fFontSize * 1440);
            }

            // setting font 
            // if we don't have requested font -- replace it with "Arial"
            try
            {
                fntFamily = new FontFamily((string)m_hashFaceNames[m_nFontID]);
            }
            catch (ArgumentException)
            {
                fntFamily = new FontFamily("Arial");
            }
            chFormat.dwMask |= (uint)RichTextNativeMethods.RichEditFormat.CFM_FACE;
            chFormat.szFaceName = new char[32];

            for (int i = 0; i < fntFamily.Name.Length; i++)
            {
                chFormat.szFaceName[i] = fntFamily.Name[i];
            }

            // if ( m_fFontScale > 0  )
            // {
            chFormat.dwMask |= (uint)RichTextNativeMethods.RichEditFormat.CFM_WEIGHT;
            chFormat.wWeight = (short)FontWeight.FW_NORMAL;
            
            // }
            if (m_nSpacing > 0)
            {
                chFormat.dwMask |= (uint)RichTextNativeMethods.RichEditFormat.CFM_SPACING;
                chFormat.sSpacing = (short)m_nSpacing;
            }

            // Defining DoubleUnderline
            if (m_nDoubleUnderline == 1)
            {
                chFormat.dwMask |= (uint)RichTextNativeMethods.RichEditFormat.CFM_UNDERLINETYPE;
                chFormat.bUnderlineType = CFU_UNDERLINEDOUBLE;
            }

            // CHARFORMAT2.dwEffects
            // setting font styles
            if ((m_enumStyle & CharFontStyle.Bold) == CharFontStyle.Bold)
            {
                chFormat.dwMask |= (uint)RichTextNativeMethods.RichEditFormat.CFM_BOLD;
                chFormat.dwEffects |= CFE_BOLD;
            }

            if ((m_enumStyle & CharFontStyle.Italic) == CharFontStyle.Italic)
            {
                chFormat.dwMask |= (uint)RichTextNativeMethods.RichEditFormat.CFM_ITALIC;
                chFormat.dwEffects |= CFE_ITALIC;
            }

            if ((m_enumStyle & CharFontStyle.Underline) == CharFontStyle.Underline)
            {
                chFormat.dwMask |= (uint)RichTextNativeMethods.RichEditFormat.CFM_UNDERLINE;
                chFormat.dwEffects |= CFE_UNDERLINE;
            }

            if ((m_enumStyle & CharFontStyle.SmallCaps) == CharFontStyle.SmallCaps)
            {
                chFormat.dwMask |= (uint)RichTextNativeMethods.RichEditFormat.CFM_SMALLCAPS;
                chFormat.dwEffects |= CFE_SMALLCAPS;
            }

            if (m_nStrikeThrough == 1)
            {
                chFormat.dwMask |= (uint)RichTextNativeMethods.RichEditFormat.CFM_STRIKEOUT;
                chFormat.dwEffects |= CFE_STRIKEOUT;
            }

            switch (m_nPosition)
            {
                case 1:
                    chFormat.dwMask |= CFM_SUPERSCRIPT;
                    chFormat.dwEffects |= CFE_SUPERSCRIPT;
                    break;
                case 2:
                    chFormat.dwMask |= CFM_SUBSCRIPT;
                    chFormat.dwEffects |= CFE_SUBSCRIPT;
                    break;
            }

            RichTextNativeMethods.SendMessage(m_rtfTemp.Handle, EM_SETCHARFORMAT, SCF_SELECTION, ref chFormat);
        }

        /// <summary>
        /// Inherits the specified base helper.
        /// </summary>
        /// <param name="hlpFromm">The base helper.</param>
        /// <returns>Base helper value.</returns>
        public int Inherit(IBaseHelper hlpFromm)
        {
            HelperCharStyle hlpFrom = hlpFromm as HelperCharStyle;

            // Char
            if (m_nFontID == -1)
                m_nFontID = hlpFrom.FontID;
            if (m_clrTextColor == Color.Empty)
                m_clrTextColor = hlpFrom.TextColor;
            if (m_fFontSize == -1)
                m_fFontSize = hlpFrom.TextSize;
            if (m_nLangID == -1)
                m_nLangID = hlpFrom.LangID;
            if (m_nRTL == -1)
                m_nRTL = hlpFrom.RTL;
            if (m_nStrikeThrough == -1)
                m_nStrikeThrough = hlpFrom.StrikeThrough;
            if (m_nDoubleUnderline == -1)
                m_nDoubleUnderline = hlpFrom.DoubleUnderline;
            if (m_nVertical == -1)
                m_nVertical = hlpFrom.UseVertical;
            if (m_enumStyle == CharFontStyle.Undefined)
            {
                m_enumStyle &= ~CharFontStyle.Undefined;
                m_enumStyle = hlpFrom.Style;
            }
            if (m_nCase == -1)
                m_nCase = hlpFrom.Case;
            if (m_nPosition == -1)
                m_nPosition = hlpFrom.Position;
            if (m_nSpacing == -1)
                m_nSpacing = hlpFrom.Spacing;
            if (m_fFontScale == -1)
                m_fFontScale = hlpFrom.m_fFontScale;

            return hlpFrom.InheritedFrom;
        }

        #endregion Methods

        #endregion IBaseHelper
    }

    /// <summary>
    /// Para style helper
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class HelperParaStyle : IBaseHelper
    {
        #region Constants

        private const int TO_TWIPS = 1440;
        private const int PFM_STARTINDENT = 1; // 0x0001
        private const int PFM_RIGHTINDENT = 2; // 0x0002 
        private const int PFM_ALIGNMENT = 8; // 0x0008 
        private const int PFA_CENTER = 3; // 0x0003 
        private const int PFA_JUSTIFY = 4; // 0x0004 
        private const int PFA_LEFT = 1; // 0x0001 
        private const int PFA_RIGHT = 2; // 0x0002
        private const int PFM_SPACEAFTER = 128; // 0x0080
        private const int PFM_SPACEBEFORE = 64; // 0x0040 
        private const int EM_SETPARAFORMAT = 1095; // 0x0447

        #endregion Constants

        #region Fields

        private string m_strSearchFor = "TextStyle";
        private int m_nInheritedFrom;

        private RichTextBoxAdv m_rtfTemp;

        // ParaBlock
        private float m_fIndFirst;
        private float m_fIndLeft;
        private float m_fIndRight;
        private float m_fSpLine;
        private float m_fSpBefore;
        private float m_fSpAfter;
        private int m_nHorzAlign;
        private float m_fTextPosAfterBullet;        

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HelperParaStyle"/> class.
        /// </summary>
        public HelperParaStyle()
        {
            m_rtfTemp = null;
            m_nInheritedFrom = -1;

            m_fIndFirst = -1;
            m_fIndLeft = -1;
            m_fIndRight = -1;
            m_fSpLine = -1;
            m_fSpBefore = -1;
            m_fSpAfter = -1;
            m_nHorzAlign = -1;
            m_fTextPosAfterBullet = -1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HelperParaStyle"/> class.
        /// </summary>
        /// <param name="rtfBox">The RTF box.</param>
        public HelperParaStyle(ref RichTextBoxAdv rtfBox)
            : this()
        {
            m_rtfTemp = rtfBox;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the indent first.
        /// </summary>
        /// <value>The indent first.</value>
        [DefaultValue(-1)]
        public float IndFirst
        {
            get
            {
                return m_fIndFirst;
            }
            set
            {
                if (m_fIndFirst != value)
                    m_fIndFirst = value;
            }
        }

        /// <summary>
        /// Gets or sets the indent left.
        /// </summary>
        /// <value>The indent left.</value>
        [DefaultValue(-1)]
        public float IndLeft
        {
            get
            {
                return m_fIndLeft;
            }
            set
            {
                if (m_fIndLeft != value)
                    m_fIndLeft = value;
            }
        }

        /// <summary>
        /// Gets or sets the indent right.
        /// </summary>
        /// <value>The indent right.</value>
        [DefaultValue(-1)]
        public float IndRight
        {
            get
            {
                return m_fIndRight;
            }
            set
            {
                if (m_fIndRight != value)
                    m_fIndRight = value;
            }
        }

        /// <summary>
        /// Gets or sets the sp line.
        /// </summary>
        /// <value>The sp line.</value>
        [DefaultValue(-1)]
        public float SpLine
        {
            get
            {
                return m_fSpLine;
            }
            set
            {
                if (m_fSpLine != value)
                    m_fSpLine = value;
            }
        }

        /// <summary>
        /// Gets or sets the sp before.
        /// </summary>
        /// <value>The sp before.</value>
        [DefaultValue(-1)]
        public float SpBefore
        {
            get
            {
                return m_fSpBefore;
            }
            set
            {
                if (m_fSpBefore != value)
                    m_fSpBefore = value;
            }
        }

        /// <summary>
        /// Gets or sets the sp after.
        /// </summary>
        /// <value>The sp after.</value>
        [DefaultValue(-1)]
        public float SpAfter
        {
            get
            {
                return m_fSpAfter;
            }
            set
            {
                if (m_fSpAfter != value)
                    m_fSpAfter = value;
            }
        }

        /// <summary>
        /// Gets or sets the horizontal align.
        /// </summary>
        /// <value>The horizontal align.</value>
        [DefaultValue(-1)]
        public int HorzAlign
        {
            get
            {
                return m_nHorzAlign;
            }
            set
            {
                if (m_nHorzAlign != value)
                    m_nHorzAlign = value;
            }
        }

        /// <summary>
        /// Gets or sets the text position after bullet.
        /// </summary>
        /// <value>The text position after bullet.</value>
        [DefaultValue(-1)]
        public float TextPosAfterBullet
        {
            get
            {
                return m_fTextPosAfterBullet;
            }
            set
            {
                if (m_fTextPosAfterBullet != value)
                    m_fTextPosAfterBullet = value;
            }
        }

        #endregion Properties

        #region IBaseHelper

        #region Properties

        /// <summary>
        /// Gets the search.
        /// </summary>
        /// <value></value>
        public string SearchFor
        {
            get
            {
                return m_strSearchFor;
            }
        }

        /// <summary>
        /// Gets or sets the inherited value.
        /// </summary>
        /// <value></value>
        public int InheritedFrom
        {
            get
            {
                return m_nInheritedFrom;
            }
            set
            {
                if (m_nInheritedFrom != value)
                    m_nInheritedFrom = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Appliables the specified appliable.
        /// </summary>
        /// <param name="Appliable">The appliable.</param>
        public void Appliable(int Appliable)
        {
        }

        /// <summary>
        /// Applies the style.
        /// </summary>
        /// <param name="node">The node.</param>
        public void ApplyStyle(INode node)
        {
            if (m_rtfTemp == null)
                throw new MemberAccessException("You must initialize member before acccessing.");

            RichTextNativeMethods.PARAFORMAT2 pfStruct = new RichTextNativeMethods.PARAFORMAT2();
            pfStruct.cbSize = Marshal.SizeOf(pfStruct);

            if (m_fIndFirst > 0)
            {
                pfStruct.dwMask |= PFM_STARTINDENT;
                pfStruct.dxStartIndent = (int)m_fIndFirst;
            }

            if (m_fIndRight > 0)
            {
                pfStruct.dwMask |= PFM_RIGHTINDENT;
                pfStruct.dxRightIndent = (int)m_fIndRight;
            }

            if (m_nHorzAlign != -1)
            {
                pfStruct.dwMask |= PFM_ALIGNMENT;
                switch (m_nHorzAlign)
                {
                    case 0:
                        pfStruct.wAlignment = PFA_LEFT;
                        break;
                    case 1:
                        pfStruct.wAlignment = PFA_CENTER;
                        break;
                    case 2:
                        pfStruct.wAlignment = PFA_RIGHT;
                        break;
                    case 3:
                        pfStruct.wAlignment = PFA_JUSTIFY;
                        break;
                    case 4:
                        pfStruct.wAlignment = PFA_JUSTIFY;
                        break;
                }
            }

            if (m_fSpAfter != -1)
            {
                pfStruct.dwMask |= PFM_SPACEAFTER;
                pfStruct.dySpaceAfter = (int)m_fSpAfter * TO_TWIPS;
            }

            if (m_fSpBefore != -1)
            {
                pfStruct.dwMask |= PFM_SPACEBEFORE;
                pfStruct.dySpaceBefore = (int)m_fSpBefore * TO_TWIPS;
            }

            RichTextNativeMethods.SendMessage(m_rtfTemp.Handle, EM_SETPARAFORMAT, 0, ref pfStruct);
        }

        /// <summary>
        /// Inherits the specified base helper.
        /// </summary>
        /// <param name="hlpFromm">The base helper.</param>
        /// <returns>Base helper value.</returns>
        public int Inherit(IBaseHelper hlpFromm)
        {
            HelperParaStyle hlpFrom = hlpFromm as HelperParaStyle;

            if (m_nHorzAlign == -1)
                m_nHorzAlign = hlpFrom.HorzAlign;
            if (m_fIndFirst == -1)
                m_fIndFirst = hlpFrom.IndFirst;
            if (m_fIndLeft == -1)
                m_fIndLeft = hlpFrom.IndLeft;
            if (m_fIndRight == -1)
                m_fIndRight = hlpFrom.IndRight;
            if (m_fSpAfter == -1)
                m_fSpAfter = hlpFrom.SpAfter;
            if (m_fSpBefore == -1)
                m_fSpBefore = hlpFrom.SpBefore;
            if (m_fSpLine == -1)
                m_fSpLine = hlpFrom.SpLine;
            if (m_fTextPosAfterBullet == -1)
                m_fTextPosAfterBullet = hlpFrom.TextPosAfterBullet;

            return hlpFrom.InheritedFrom;
        }

        #endregion Methods

        #endregion IBaseHelper
    }

    /// <summary>
    /// X form class.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class XForm : ICloneable
    {
        #region Fields

        private bool m_bClosedPathNode;
        private bool m_bFlipX;
        private bool m_bFlipY;
        private float m_fRotationAngle;
        private float m_fNonConvertedWidth;
        private float m_fNonConvertedHeight;
        private float m_fPinX;
        private float m_fPinY;
        private float m_fLocPinX;
        private float m_fLocPinY;
        private float m_fScratchY;
        private float m_fScratchX;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether path node is closed.
        /// </summary>
        /// <value><c>true</c> if closed path node; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public bool ClosedPathNode
        {
            get
            {
                return m_bClosedPathNode;
            }
            set
            {
                m_bClosedPathNode = value;
            }
        }

        /// <summary>
        /// Gets or sets the pin X.
        /// </summary>
        /// <value>The pin X.</value>
        [DefaultValue(0)]
        public float PinX
        {
            get
            {
                return m_fPinX;
            }
            set
            {
                if (m_fPinX != value)
                    m_fPinX = value;
            }
        }

        /// <summary>
        /// Gets or sets the pin Y.
        /// </summary>
        /// <value>The pin Y.</value>
        [DefaultValue(0)]
        public float PinY
        {
            get
            {
                return m_fPinY;
            }
            set
            {
                if (m_fPinY != value)
                    m_fPinY = value;
            }
        }

        /// <summary>
        /// Gets or sets the loc pin X.
        /// </summary>
        /// <value>The loc pin X.</value>
        [DefaultValue(0)]
        public float LocPinX
        {
            get
            {
                return m_fLocPinX;
            }
            set
            {
                if (m_fLocPinX != value)
                    m_fLocPinX = value;
            }
        }

        /// <summary>
        /// Gets or sets the loc pin Y.
        /// </summary>
        /// <value>The loc pin Y.</value>
        [DefaultValue(0)]
        public float LocPinY
        {
            get
            {
                return m_fLocPinY;
            }
            set
            {
                if (m_fLocPinY != value)
                    m_fLocPinY = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether flip X.
        /// </summary>
        /// <value><c>true</c> if flip X; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public bool FlipX
        {
            get
            {
                return m_bFlipX;
            }
            set
            {
                m_bFlipX = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether flip Y.
        /// </summary>
        /// <value><c>true</c> if flip Y; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public bool FlipY
        {
            get
            {
                return m_bFlipY;
            }
            set
            {
                m_bFlipY = value;
            }
        }

        /// <summary>
        /// Gets or sets the rotation angle.
        /// </summary>
        /// <value>The rotation angle.</value>
        [DefaultValue(0)]
        public float RotationAngle
        {
            get
            {
                return m_fRotationAngle;
            }
            set
            {
                if (m_fRotationAngle != value)
                    m_fRotationAngle = value;
            }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        [DefaultValue(0)]
        public float Width
        {
            get
            {
                return m_fNonConvertedWidth;
            }
            set
            {
                if (m_fNonConvertedWidth != value)
                    m_fNonConvertedWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        [DefaultValue(0)]
        public float Height
        {
            get
            {
                return m_fNonConvertedHeight;
            }
            set
            {
                if (m_fNonConvertedHeight != value)
                    m_fNonConvertedHeight = value;
            }
        }

        #endregion Properties

        #region ICloneable Members

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            XForm res = new XForm();
            res.m_bClosedPathNode = this.m_bClosedPathNode;
            res.m_bFlipX = this.m_bFlipX;
            res.m_bFlipY = this.m_bFlipY;
            res.m_fRotationAngle = this.m_fRotationAngle;
            res.m_fNonConvertedWidth = this.m_fNonConvertedWidth;
            res.m_fNonConvertedHeight = this.m_fNonConvertedHeight;
            res.m_fPinX = this.m_fPinX;
            res.m_fPinY = this.m_fPinY;
            res.m_fLocPinX = this.m_fLocPinX;
            res.m_fLocPinY = this.m_fLocPinY;
            res.m_fScratchX = this.m_fScratchX;
            res.m_fScratchY = this.m_fScratchY;
            return res;
        }

        /// <summary>
        /// Gets or sets the scratch X.
        /// </summary>
        /// <value>The scratch X.</value>
        [DefaultValue(0)]
        public float ScratchX
        {
            get
            {
                return m_fScratchX;
            }
            set
            {
                if (m_fScratchX != value)
                    m_fScratchX = value;
            }
        }

        /// <summary>
        /// Gets or sets the scratch Y.
        /// </summary>
        /// <value>The scratch Y.</value>
        [DefaultValue(0)]
        public float ScratchY
        {
            get
            {
                return m_fScratchY;
            }
            set
            {
                if (m_fScratchY != value)
                    m_fScratchY = value;
            }
        }

        #endregion
    }

    internal class ForeignImageInfo
    {
        #region Fields
        private Size m_szImgSize;
        private SizeF m_szOffset;
        #endregion

        #region Initialize/finalize methods
        public ForeignImageInfo()
        {
            m_szImgSize = Size.Empty;
            m_szOffset = SizeF.Empty;
        }
        #endregion

        #region Properties
        public int ImageHeight
        {
            get { return m_szImgSize.Height; }
            set { m_szImgSize.Height = value; }
        }
        public int ImageWidth
        {
            get { return m_szImgSize.Width; }
            set { m_szImgSize.Width = value; }
        }
        public float OffsetXToOrigin
        {
            get { return m_szOffset.Width; }
            set { m_szOffset.Width = value; }
        }
        public float OffsetYToOrigin
        {
            get { return m_szOffset.Height; }
            set { m_szOffset.Height = value; }
        }
        #endregion
    }

    /// <summary>
    /// Char font styles
    /// </summary>
    [
    Flags,
    Syncfusion.Documentation.DocumentationExclude()
    ]
    public enum CharFontStyle
    {
        /// <summary>
        /// Undefined font style.
        /// </summary>
        Undefined = 100,

        /// <summary>
        /// Bold font style.
        /// </summary>
        Bold = 1,

        /// <summary>
        /// Italic font style.
        /// </summary>
        Italic = 2,

        /// <summary>
        /// Underline font style.
        /// </summary>
        Underline = 4,

        /// <summary>
        /// Small caps font style.
        /// </summary>
        SmallCaps = 8
    }

    /// <summary>
    /// Visio shape class.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class VisioShape
    {
        #region Fields

        private GraphicsPath m_gpShape = null;
        private RectangleF m_rectGroup = RectangleF.Empty;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the shape graphics path.
        /// </summary>
        /// <value>The shape graphics path.</value>
        public GraphicsPath ShapeGraphicsPath
        {
            get
            {
                return m_gpShape;
            }
            set
            {
                if (m_gpShape != value)
                    m_gpShape = value;
            }
        }

        /// <summary>
        /// Gets or sets the group rect.
        /// </summary>
        /// <value>The group rect.</value>
        public RectangleF GroupRect
        {
            get
            {
                return m_rectGroup;
            }
            set
            {
                if (m_rectGroup != value)
                    m_rectGroup = value;
            }
        }
        #endregion Properties
    }

    /// <summary>
    /// Connection transformation.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class ConnectionTransformation
    {
        #region Fields

        private Matrix m_matrixShapeRotate;
        private Matrix m_matrixShapeFlip;
        private XForm m_xfConnectionParent;
        private PointF m_ptGroup;
        private PointF m_ptToFlip;
        private bool m_bGroup;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionTransformation"/> class.
        /// </summary>
        public ConnectionTransformation()
        {
            m_matrixShapeFlip = new Matrix();
            m_matrixShapeRotate = new Matrix();
            m_ptGroup = new Point();
            m_ptToFlip = new Point();
            m_xfConnectionParent = new XForm();
            m_bGroup = false;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ConnectionTransformation"/> is group.
        /// </summary>
        /// <value><c>true</c> if group; otherwise, <c>false</c>.</value>
        public bool Group
        {
            get
            {
                return m_bGroup;
            }
            set
            {
                m_bGroup = value;
            }
        }

        /// <summary>
        /// Gets or sets the shape rotation.
        /// </summary>
        /// <value>The shape rotate.</value>
        public Matrix ShapeRotate
        {
            get
            {
                return m_matrixShapeRotate;
            }
            set
            {
                if (value != m_matrixShapeRotate)
                    m_matrixShapeRotate = value;
            }
        }

        /// <summary>
        /// Gets or sets the shape flip.
        /// </summary>
        /// <value>The shape flip.</value>
        public Matrix ShapeFlip
        {
            get
            {
                return m_matrixShapeFlip;
            }
            set
            {
                if (value != m_matrixShapeFlip)
                    m_matrixShapeFlip = value;
            }
        }

        /// <summary>
        /// Gets or sets the point group.
        /// </summary>
        /// <value>The point group.</value>
        public PointF PointGroup
        {
            get
            {
                return m_ptGroup;
            }
            set
            {
                if (value != m_ptGroup)
                    m_ptGroup = value;
            }
        }

        /// <summary>
        /// Gets or sets the flip point.
        /// </summary>
        /// <value>The flip point.</value>
        public PointF FlipPoint
        {
            get
            {
                return m_ptToFlip;
            }
            set
            {
                if (value != m_ptToFlip)
                    m_ptToFlip = value;
            }
        }

        /// <summary>
        /// Gets or sets the connection parent.
        /// </summary>
        /// <value>The connection parent.</value>
        public XForm ConnectionParent
        {
            get
            {
                return m_xfConnectionParent;
            }
            set
            {
                if (value != m_xfConnectionParent)
                    m_xfConnectionParent = value;
            }
        }

        #endregion Properties
    }

    #endregion Helper Classes
}
