#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// The TableTreeLayoutManager is a laying out nodes to table style.
    /// </summary>
    /// <remarks>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.LayoutManager"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.GraphLayoutManager"/>
    /// </remarks>
    [ToolboxItem(false)]
    public class TableLayoutManager : LayoutManager
    {
        #region Constants
        private const int c_nDEFAULT_OFFSET = 10;
        private const int c_nDEFAULT_COLUMNS_COUNT = 4;
        private const int c_nDEFAULT_ROWS_COUNT = 4;
        #endregion Constants

        #region Fields
        private float m_fVerticalSpacing = c_nDEFAULT_OFFSET;
        private float m_fHorizontalSpacing = c_nDEFAULT_OFFSET;
        private MeasureUnits m_unitMeasure = MeasureUnits.Pixel;
        private SizeF m_szMaxSize;
        private Size m_szMaxCellCount;
        private Orientation m_orientation;
        private CellSizeMode m_cellSizeMode;
        protected float m_fBorder;
        /// <summary>
        /// The first node ion selection list what ignore layout.
        /// </summary>
        private Node m_helperSelectedNode;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets unit of measure.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(MeasureUnits.Pixel)]
        [Description("Specifies unit of measurement.")]
        public MeasureUnits MeasurementUnits
        {
            get
            {
                return m_unitMeasure;
            }
            set
            {
                if (value != m_unitMeasure)
                    m_unitMeasure = value;
            }
        }

        /// <summary>
        /// Gets or sets the vertical offset between adjacent nodes.
        /// </summary>
        [DefaultValue(c_nDEFAULT_OFFSET)]
        public float VerticalSpacing
        {
            get 
            { 
                return m_fVerticalSpacing; 
            }
            set
            {
                if (m_fVerticalSpacing != value)
                    m_fVerticalSpacing = value;
            }
        }

        /// <summary>
        /// Gets or sets the horizontal offset between adjacent nodes.
        /// </summary>
        [DefaultValue(c_nDEFAULT_OFFSET)]
        public float HorizontalSpacing
        {
            get 
            { 
                return m_fHorizontalSpacing; 
            }
            set
            {
                if (m_fHorizontalSpacing != value)
                    m_fHorizontalSpacing = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the cell.
        /// </summary>
        /// <value>The size of the cell.</value>
        public SizeF MaxSize
        {
            get
            {
                return m_szMaxSize;
            }
            set
            {
                m_szMaxSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the horizontal cell count in table.
        /// </summary>
        /// <value>The horizontal count.</value>
        [Browsable(true)]
        [DefaultValue(4)]
        [Description("Specifies max table columns count.")]
        public int MaxColummnCount
        {
            get
            {
                return m_szMaxCellCount.Width;
            }
            set
            {
                m_szMaxCellCount.Width = value;
            }
        }

        /// <summary>
        /// Gets or sets the vertical cell count in table.
        /// </summary>
        /// <value>The vertical count.</value>
        [Browsable(true)]
        [DefaultValue(5)]
        [Description("Specifies max table rows count.")]
        public int MaxRowsCount
        {
            get
            {
                return m_szMaxCellCount.Height;
            }
            set
            {
                m_szMaxCellCount.Height = value;
            }
        }

        /// <summary>
        /// Gets or sets the orientation way of lay out.
        /// </summary>
        /// <value>The orientation.</value>
        [Browsable(true)]
        [DefaultValue("Horizontal")]
        [Description("Specifies layout orientation.")]
        public Orientation Orientation
        {
            get { return m_orientation; }
            set { m_orientation = value; }
        }

        /// <summary>
        /// Gets or sets the cell size mode.
        /// </summary>
        /// <value>The cell size mode.</value>
        public CellSizeMode CellSizeMode
        {
            get { return m_cellSizeMode; }
            set { m_cellSizeMode = value; }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="TableLayoutManager"/> class.
        /// </summary>
        public TableLayoutManager()
            : base()
        {
            m_szMaxCellCount = new Size(c_nDEFAULT_COLUMNS_COUNT, c_nDEFAULT_ROWS_COUNT);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableLayoutManager"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="nMaxColumnCount">The max columns count.</param>
        /// <param name="nMaxRowsCount">The max rows count.</param>
        public TableLayoutManager(Model model, int nMaxColumnCount, int nMaxRowsCount)
            : base(model)
        {
            m_szMaxCellCount = new Size(nMaxColumnCount, nMaxRowsCount);
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Updates the layout of the nodes in the model.
        /// </summary>
        /// <param name="contextInfo">Provides context information to help with updating the layout.</param>
        /// <returns>
        /// True if changes were made; otherwise False.
        /// </returns>
        public override bool UpdateLayout(object contextInfo)
        {
            // get selected node list form given contextInfo
            NodeCollection selectionList = contextInfo as NodeCollection;
            m_fBorder = MeasureUnitsConverter.ConvertX(this.Model.LineStyle.LineWidth,this.Model.MeasurementUnits,MeasureUnits.Pixel);
            if (selectionList != null && selectionList.Count > 0)
            {
                // get first node from selected list
                m_helperSelectedNode = (Node)selectionList[0].Clone();
            }

            int nodesCount = this.Nodes.Count;

            if (nodesCount > 0)
            {
                // save boundary contrains state
                bool bBoundaryConstraintsEnabled = this.Model.BoundaryConstraintsEnabled;

                BeginLayout(false);
                MakeTableLayout(this.Nodes);

                EndLayout(bBoundaryConstraintsEnabled);
            }

            return false;
        }
        #endregion

        #region Class helpers methods
        private void BeginLayout(bool bBoundaryConstraintsEnabled)
        {
            this.Model.HistoryManager.StartAtomicAction("Layout");
            this.Model.BoundaryConstraintsEnabled = bBoundaryConstraintsEnabled;

            // Set updating layout flag.
            this.UpdatingLayout = true;
        }
        private void EndLayout(bool bBoundaryConstraintsEnabled)
        {
            // Reset update layouting flag.
            this.UpdatingLayout = false;

            this.Model.BoundaryConstraintsEnabled = bBoundaryConstraintsEnabled;
            this.Model.HistoryManager.EndAtomicAction();
        }

        /// <summary>
        /// Makes the table layout.
        /// </summary>
        /// <param name="lstNodes">The nodes list.</param>
        private void MakeTableLayout(NodeCollection lstNodes)
        {
            switch (m_cellSizeMode)
            {
                case CellSizeMode.EqualToMaxNode:
                    EqualToMaxLayout(lstNodes);
                    break;
                case CellSizeMode.Minimal:
                    MinimalLayout(lstNodes);
                    break;
                case CellSizeMode.MinimalTable:
                    MinimalTableLayout(lstNodes);
                    break;
            }

            PositionNodeToSelectNode();
        }
        private void PositionNodeToSelectNode()
        {
            if (m_helperSelectedNode != null)
            {
                MeasureUnits units = MeasureUnits.Pixel;

                Node node = m_helperSelectedNode as Node;
                Node originNode = this.Nodes.FindNodeByName(node.Name);

                // PointF ptPinPointOrigin = ((IUnitIndependent)originNode).GetPinPoint( units );
                PointF ptPinPointClone = ((IUnitIndependent)node).GetPinPoint(units);
                ptPinPointClone.X += m_fBorder;
                ptPinPointClone.Y += m_fBorder;

                //left and top margin
                ptPinPointClone.X += MeasureUnitsConverter.ToPixelX(this.LeftMargin, this.MeasurementUnits);
                ptPinPointClone.Y += MeasureUnitsConverter.ToPixelY(this.TopMargin, this.MeasurementUnits);

                ((IUnitIndependent)originNode).SetPinPoint(ptPinPointClone, units);

                m_helperSelectedNode = null;
            }
        }

        private SizeF GetCellSize(NodeCollection lstNodes)
        {
            SizeF szCellSize = SizeF.Empty;
            MeasureUnits units = MeasureUnits.Pixel;
            foreach (Node node in lstNodes)
            {
                if (IgnoreNode(lstNodes, node))
                    continue;
                RectangleF rcBounds = ((IUnitIndependent)node).GetBoundingRectangle(units, false);
                //szCellSize.Width = Math.Max(szCellSize.Width, node.Size.Width);
                //szCellSize.Height = Math.Max(szCellSize.Height, node.Size.Height);

                szCellSize.Width = Math.Max(szCellSize.Width, rcBounds.Size.Width);
                szCellSize.Height = Math.Max(szCellSize.Height, rcBounds.Size.Height);

            }

            return szCellSize;
        }
        private bool IgnoreNode(NodeCollection lstNodes, Node node)
        {
            bool bIgnore = false;
            IEndPointContainer endPointContainer = node as IEndPointContainer;

            if (endPointContainer != null)
            {
                if (endPointContainer.HeadEndPoint.Port != null)
                {
                    bIgnore = lstNodes.Contains(endPointContainer.HeadEndPoint.Port.Container);
                }

                if (!bIgnore && endPointContainer.TailEndPoint.Port != null)
                {
                    bIgnore = lstNodes.Contains(endPointContainer.TailEndPoint.Port.Container);
                }
            }

            return bIgnore;
        }
        private void MinimalLayout(NodeCollection lstNodes)
        {
            MeasureUnits units = MeasureUnits.Pixel;
            
            // current table index position
            PointF ptPosition = Point.Empty;
            float fMaxHeight = 0;

            foreach (Node node in lstNodes)
            {
                if (IgnoreNode(lstNodes, node))
                    continue;

                PointF ptPinPoint = ((IUnitIndependent)node).GetPinPoint(units);
                RectangleF rcBounds = ((IUnitIndependent)node).GetBoundingRectangle(units, false);
                SizeF szOffset = new SizeF(ptPinPoint.X - rcBounds.X, ptPinPoint.Y - rcBounds.Y);

                PointF ptLocation = ptPosition;
                ptLocation.X += szOffset.Width + m_fBorder;
                ptLocation.Y += szOffset.Height + m_fBorder;

                //left and top margin
                ptLocation.X += MeasureUnitsConverter.ToPixelX(this.LeftMargin, this.MeasurementUnits);
                ptLocation.Y += MeasureUnitsConverter.ToPixelY(this.TopMargin, this.MeasurementUnits);

                ((IUnitIndependent)node).SetPinPoint(ptLocation, units);

                if (m_orientation == Orientation.Horizontal)
                {
                    ptPosition.X += rcBounds.Width;
                    fMaxHeight = Math.Max(fMaxHeight, rcBounds.Height);

                    if (ptPosition.X >= this.MaxSize.Width)
                    {
                        ptPosition.X = 0;
                        ptPosition.Y += fMaxHeight;
                        fMaxHeight = 0;
                    }
                }
                else if (m_orientation == Orientation.Vertical)
                {
                    ptPosition.Y += rcBounds.Height;
                    fMaxHeight = Math.Max(fMaxHeight, rcBounds.Width);

                    if (ptPosition.Y >= this.MaxSize.Height)
                    {
                        ptPosition.Y = 0;
                        ptPosition.X += fMaxHeight;
                        fMaxHeight = 0;
                    }
                }
            }
        }
        private void MinimalTableLayout(NodeCollection lstNodes)
        {
            MeasureUnits units = MeasureUnits.Pixel;

            int nNode = 0;
            PointF ptPosition = PointF.Empty;
            PointF ptLocation;

            SizeF szCellSize = GetCellSize(lstNodes);
            Size szCellCount = GetCellCount(szCellSize);

            if (szCellCount.Width <= 0 || szCellCount.Height <= 0)
                return;

            ArrayList lstDimensionColumns = new ArrayList();
            ArrayList lstDimensionRows = new ArrayList();

            GetMaxDimension(lstNodes, lstDimensionColumns, lstDimensionRows, szCellCount);

            foreach (Node node in lstNodes)
            {
                PointF ptPinPoint = ((IUnitIndependent)node).GetPinPoint(units);
                RectangleF rcBounds = ((IUnitIndependent)node).GetBoundingRectangle(units, false);
                SizeF szOffset = new SizeF(ptPinPoint.X - rcBounds.X, ptPinPoint.Y - rcBounds.Y);
                SizeF szSize = rcBounds.Size;

                int columnIndex = (m_orientation == Orientation.Horizontal) ? nNode % szCellCount.Width : nNode / szCellCount.Height;
                int rowIndex = (m_orientation == Orientation.Horizontal) ? nNode / szCellCount.Width : nNode % szCellCount.Height;

                if (IgnoreNode(lstNodes, node))
                    szCellSize = SizeF.Empty;
                else
                    szCellSize = new SizeF((float)lstDimensionColumns[columnIndex], (float)lstDimensionRows[rowIndex]);

                ptLocation = ptPosition;
                ptLocation.X += (szCellSize.Width - szSize.Width) / 2 + szOffset.Width + m_fBorder;
                ptLocation.Y += (szCellSize.Height - szSize.Height) / 2 + szOffset.Height + m_fBorder;

                //left and top margin
                ptLocation.X += MeasureUnitsConverter.ToPixelX(this.LeftMargin, this.MeasurementUnits);
                ptLocation.Y += MeasureUnitsConverter.ToPixelY(this.TopMargin, this.MeasurementUnits);

                ((IUnitIndependent)node).SetPinPoint(ptLocation, units);

                if (m_orientation == Orientation.Horizontal)
                {
                    if (!IgnoreNode(lstNodes, node))
                    {
                        ptPosition.X += szCellSize.Width + this.HorizontalSpacing;
                        nNode++;
                        columnIndex++;
                    }

                    if (columnIndex >= szCellCount.Width)
                    {
                        ptPosition.X = 0;
                        ptPosition.Y += szCellSize.Height + this.VerticalSpacing;
                    }
                }
                else if (m_orientation == Orientation.Vertical)
                {
                    if (!IgnoreNode(lstNodes, node))
                    {
                        ptPosition.Y += szCellSize.Height + this.VerticalSpacing;
                        nNode++;
                        rowIndex++;
                    }

                    if (rowIndex >= szCellCount.Height)
                    {
                        ptPosition.Y = 0;
                        ptPosition.X += szCellSize.Width + this.HorizontalSpacing;
                    }
                }
            }
        }
        private Size GetCellCount(SizeF szCellSize)
        {
            Size szCellCount = m_szMaxCellCount;

            int cellCount = (int)(this.MaxSize.Width / (szCellSize.Width + this.HorizontalSpacing));
            if (cellCount < szCellCount.Width)
                szCellCount.Width = cellCount;

            cellCount = (int)(this.MaxSize.Height / (szCellSize.Height + this.VerticalSpacing));
            if (cellCount < szCellCount.Height)
                szCellCount.Height = cellCount;

            return szCellCount;
        }
        private void GetMaxDimension(NodeCollection lstNodes, ArrayList lstDimensionColumns, ArrayList lstDimensionRows, Size szCellCount)
        {
            int nNode = 0;
            MeasureUnits units = MeasureUnits.Pixel;

            foreach (Node node in lstNodes)
            {
                if (IgnoreNode(lstNodes, node))
                    continue;

                SizeF szSize = ((IUnitIndependent)node).GetBoundingRectangle(units, false).Size;
                int columnIndex = 0;
                int rowIndex = 0;
                float zero = 0f;

                if (m_orientation == Orientation.Horizontal)
                {
                    columnIndex = nNode % szCellCount.Width;
                    rowIndex = nNode / szCellCount.Width;
                }
                else if (m_orientation == Orientation.Vertical)
                {
                    columnIndex = nNode / szCellCount.Height;
                    rowIndex = nNode % szCellCount.Height;
                }

                while (lstDimensionColumns.Count <= columnIndex)
                {
                    lstDimensionColumns.Add(zero);
                }

                float width = (float)lstDimensionColumns[columnIndex];
                lstDimensionColumns[columnIndex] = Math.Max(width, szSize.Width);

                while (lstDimensionRows.Count <= rowIndex)
                {
                    lstDimensionRows.Add(zero);
                }

                float height = (float)lstDimensionRows[rowIndex];
                lstDimensionRows[rowIndex] = Math.Max(height, szSize.Height);

                nNode++;
            }
        }
        private void EqualToMaxLayout(NodeCollection lstNodes)
        {
            MeasureUnits units = MeasureUnits.Pixel;
            
            // current table index position
            Point ptPosition = Point.Empty;
            
            // max horizontal and vertical layout
            SizeF szCellSize = GetCellSize(lstNodes);
            Size szCellCount = GetCellCount(szCellSize);

            foreach (Node node in lstNodes)
            {
                if (IgnoreNode(lstNodes, node))
                    continue;

                PointF ptPinPoint = ((IUnitIndependent)node).GetPinPoint(units);
                RectangleF rcBounds = ((IUnitIndependent)node).GetBoundingRectangle(units, false);
                SizeF szOffset = new SizeF(ptPinPoint.X - rcBounds.X, ptPinPoint.Y - rcBounds.Y);
                SizeF szSize = rcBounds.Size;

                PointF ptLocation = new PointF(
                    ptPosition.X * (szCellSize.Width + this.HorizontalSpacing),
                    ptPosition.Y * (szCellSize.Height + this.VerticalSpacing));

                ptLocation.X += (szCellSize.Width - szSize.Width) / 2;
                ptLocation.Y += (szCellSize.Height - szSize.Height) / 2;

                if (m_orientation == Orientation.Horizontal)
                {
                    ptPosition.X++;

                    if (ptPosition.X >= szCellCount.Width)
                    {
                        ptPosition.X = 0;
                        ptPosition.Y++;
                    }
                }
                else
                {
                    ptPosition.Y++;

                    if (ptPosition.Y >= szCellCount.Height)
                    {
                        ptPosition.Y = 0;
                        ptPosition.X++;
                    }
                }

                ptLocation.X += szOffset.Width + m_fBorder;
                ptLocation.Y += szOffset.Height + m_fBorder;

                //left and top margin
                ptLocation.X += MeasureUnitsConverter.ToPixelX(this.LeftMargin, this.MeasurementUnits);
                ptLocation.Y += MeasureUnitsConverter.ToPixelY(this.TopMargin, this.MeasurementUnits);

                ((IUnitIndependent)node).SetPinPoint(ptLocation, units);
            }
        }
        #endregion
    }
}
