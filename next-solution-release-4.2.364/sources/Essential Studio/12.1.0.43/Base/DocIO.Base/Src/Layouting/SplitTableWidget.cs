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

#if !SILVERLIGHT

using System;
using Syncfusion.DocIO.Rendering;

namespace Syncfusion.Layouting
{
    /// <summary>
    /// Summary description for SplitTableWidget.
    /// </summary>
    internal class SplitTableWidget : IWidget
    {
        #region Fields
        private ITableWidget m_tableWidget;
        private int m_rowNumber = 0;
        private int m_colNumber = 0;
        private SplitWidgetContainer[] m_splittedCells = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SplitTableWidget"/> class.
        /// </summary>
        /// <param name="tableWidget">The table widget.</param>
        /// <param name="rowNumber">The row number.</param>
        public SplitTableWidget(ITableWidget tableWidget, int rowNumber)
            : this(tableWidget, rowNumber, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SplitTableWidget"/> class.
        /// </summary>
        /// <param name="tableWidget">The table widget.</param>
        /// <param name="rowNumber">The row number.</param>
        /// <param name="splittedCells">The row number.</param>
        public SplitTableWidget(ITableWidget tableWidget, int rowNumber, SplitWidgetContainer[] splittedCells)
            : this(tableWidget, rowNumber, 0)
        {
            m_splittedCells = splittedCells;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SplitTableWidget"/> class.
        /// </summary>
        /// <param name="tableWidget">The table widget.</param>
        /// <param name="rowNumber">The row number.</param>
        /// <param name="colNumber">The column number.</param>
        public SplitTableWidget(ITableWidget tableWidget, int rowNumber, int colNumber)
        {
            m_tableWidget = tableWidget;
            m_rowNumber = rowNumber;
            m_colNumber = colNumber;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the table widget.
        /// </summary>
        /// <value>The table widget.</value>
        public ITableWidget TableWidget
        {
            get
            {
                return m_tableWidget;
            }
        }

        /// <summary>
        /// Gets the start row number.
        /// </summary>
        /// <value>The start row number.</value>
        public int StartRowNumber
        {
            get
            {
                return m_rowNumber;
            }
        }

        /// <summary>
        /// Gets the start column number.
        /// </summary>
        /// <value>The start column number.</value>
        public int StartColumnNumber
        {
            get
            {
                return m_colNumber;
            }
        }

        /// <summary>
        /// Gets the splitted cells.
        /// </summary>
        /// <value>The splitted cells.</value>
        public SplitWidgetContainer[] SplittedCells
        {
            get
            {
                return m_splittedCells;
            }
        }
        #endregion
      
        #region IWidget members
        /// <summary>
        /// Gets layout info.
        /// </summary>
        /// <value></value>
        public ILayoutInfo LayoutInfo
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Draw range to graphics.
        /// </summary>
        public void Draw(DrawingContext dc, LayoutedWidget layoutedWidget)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Initializing LayoutInfo value to null
        /// </summary>
        public void InitLayoutInfo()
        {
           
        }
        #endregion
    }
}

#endif