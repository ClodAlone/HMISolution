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

#region file using directives
using Syncfusion.DocIO.Rendering;
using System.Drawing;
#endregion

namespace Syncfusion.Layouting
{
    /// <summary>
    /// Summary description for LeafEmtyWidget.
    /// </summary>
    internal class LeafEmtyWidget : ILeafWidget
    {
        #region Fields
        private SizeF m_size;
        private LayoutInfo m_layoutInfo;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="LeafEmtyWidget"/> class.
        /// </summary>
        /// <param name="size">The size.</param>
        public LeafEmtyWidget(SizeF size)
        {
            m_layoutInfo = new LayoutInfo(ChildrenLayoutDirection.Horizontal);
            m_size = size;
        }
        #endregion

        #region ILeafWidget Members
        /// <summary>
        /// Measures the specified graphics.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <returns></returns>
        public SizeF Measure(DrawingContext dc)
        {
            return m_size;
        }

        /// <summary>
        /// Gets layout info.
        /// </summary>
        public ILayoutInfo LayoutInfo
        {
            get
            {
                return m_layoutInfo;
            }
        }

        /// <summary>
        /// Draw range to graphics.
        /// </summary>
        public void Draw(DrawingContext dc, LayoutedWidget layoutedWidget)
        {
            //throw new NotImplementedException();
        }
        /// <summary>
        /// Initializing LayoutInfo value to null
        /// </summary>
        public void InitLayoutInfo()
        {
            m_layoutInfo = null;
        }
        #endregion
    }
}

#endif
