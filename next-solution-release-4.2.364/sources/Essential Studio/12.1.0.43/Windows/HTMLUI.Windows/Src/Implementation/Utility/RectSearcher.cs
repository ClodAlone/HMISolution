#region Copyright Syncfusion Inc. 2001 - 2014
////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility
{
    /// <summary>
    /// Class which searches element where the defined point is inside.
    /// </summary>
    internal sealed class RectSearcher
    {
        #region Class members
        /// <summary>
        /// Point which we check.
        /// </summary>
        private Point m_point;

        /// <summary>
        /// List of all blocks.
        /// </summary>
        private ArrayList m_blocksList;

        /// <summary>
        /// Default element.
        /// </summary>
        private IHTMLElement m_default;

        /// <summary>
        /// Document of this searcher.
        /// </summary>
        private InputHTML m_document;

        /// <summary>
        /// Current active block at mouse point.
        /// </summary>
        private Block m_activeBlock;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the point which we check.
        /// </summary>
        internal Point Point
        {
            get
            {
                return m_point;
            }
            set
            {
                if (m_point != value)
                {
                    m_point = value;
                }
            }
        }

        /// <summary>
        /// Gets the active block at mouse point at the current time.
        /// </summary>
        internal Block ActiveBlock
        {
            get
            {
                return m_activeBlock;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the RectSearcher class
        /// </summary>
        /// <param name="document">Document object.</param>
        public RectSearcher(InputHTML document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            if (document.RenderRoot == null)
                throw new ArgumentNullException("RenderRoot");

            m_blocksList = new ArrayList();
            m_point = Point.Empty;
            m_document = document;
            m_default = document.RenderRoot;
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Adds block to the list of blocks.
        /// </summary>
        /// <param name="block">Block object.</param>
        public void AddBlock(Block block)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            //// if( !m_blocksList.Contains( block ) )
            {
                m_blocksList.Add(block);
            }
        }

        /// <summary>
        /// Returns the tag element which contains the specified point.
        /// It returns the smallest child it contains.
        /// If mouse is not in the client area of the document, method returns NULL.
        /// </summary>
        /// <param name="point">Location for searching.</param>
        /// <returns>Element or NULL.</returns>
        public IHTMLElement GetElement(Point point)
        {
            m_point = point;

            if (!m_document.VisibleRectangle.Contains(m_point)) return null;

            return GetElementHolder(m_point);
        }

        /// <summary>
        /// Indicates whether there are no any elements for checking.
        /// </summary>
        /// <returns>True if container is empty.</returns>
        public bool IsEmpty()
        {
            return m_blocksList.Count == 0;
        }

        /// <summary>
        /// Clears all elements from inner container.
        /// </summary>
        public void Clear()
        {
            m_blocksList.Clear();
        }

        /// <summary>
        /// Resets cached found block object.
        /// </summary>
        public void ResetCache()
        {
            m_activeBlock = null;
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Searches the smallest block containing the point location.
        /// </summary>
        /// <param name="point">Point structure.</param>
        /// <returns>Smallest block containing the point location if found; Null otherwise.</returns>
        internal Block GetBlockAtPoint(Point point)
        {
            Block container = null;
            bool first = true;

            Rectangle minRect = Rectangle.Empty;
            Rectangle curRect = Rectangle.Empty;
            Block block = null;

            for (int index = 0, len = m_blocksList.Count; index < len; index++)
            {
                block = m_blocksList[index] as Block;
                curRect = block.Rectangle;

                if (curRect.Contains(point)) 
                {
                    // if block contains point
                    if (first) 
                    {
                        // first time
                        minRect = curRect;
                        container = block;
                        first = false;
                    }
                    else
                    {
                        // Current block is less than current minimum block.
                        if (minRect.Contains(curRect))
                        {
                            // bloks are equal
                            if (minRect.Equals(curRect))
                            {
                                // Current block belongs to child of current min block owner.
                                // or current block is contained in the current min block.
                                if (block.Owner.Parent == container.Owner ||
                                  container.Contains(block))
                                {
                                    minRect = curRect;
                                    container = block;
                                }
                            }
                            else 
                            {
                                // Current block is less strong than current min block.
                                minRect = curRect;
                                container = block;
                            }
                        }
                    }
                }
            }

            return container;
        }

        /// <summary>
        /// Returns the tag element which contains the points inside.
        /// </summary>
        /// <param name="point">Point value.</param>
        /// <returns>Element which holds this point.</returns>
        private IHTMLElement GetElementHolder(Point point)
        {
            Block container = GetBlockAtPoint(point);
            m_activeBlock = container;
            IHTMLElement element = (container != null) ? container.Owner : m_default;
            return element;
        }

        /// <summary>
        /// Returns the array of blocks which contains the specified point.
        /// </summary>
        /// <param name="point">Point element.</param>
        /// <returns>Array of elements containing point.</returns>
        private ArrayList GetHolders(Point point)
        {
            ArrayList array = new ArrayList();
            Rectangle rect = Rectangle.Empty;

            foreach (Block block in m_blocksList)
            {
                rect = block.Rectangle;
                if (rect.Contains(point))
                {
                    array.Add(block);
                }
            }

            return array;
        }

        /// <summary>
        /// Returns the tag element which contains the specified point.
        /// </summary>
        /// <param name="holders">Array of elements containing the point.</param>
        /// <param name="point">Point value.</param>
        /// <returns>IHTML Element reference to the element; NULL, if no element contains the point.</returns>
        private IHTMLElement GetHolderElement(ArrayList holders, Point point)
        {
            if (holders == null)
                throw new ArgumentNullException("holders");

            if (holders.Count == 0) return m_default;

            int distance = Int32.MaxValue;
            int tmpDistance = 0;
            Point blockLocation = Point.Empty;
            Block container = null;

            foreach (Block block in holders)
            {
                blockLocation.X = block.X;
                blockLocation.Y = block.Y;

                tmpDistance = GetDistance(blockLocation, point);
                if (tmpDistance <= distance)
                {
                    // If different blocks start in the same location.
                    if (tmpDistance == distance)
                    {
                        if (block.Owner.Parent == container.Owner) 
                        {
                            //// Block belongs to container child.
                            distance = tmpDistance;
                            container = block;
                        }
                    }
                    else
                    {
                        distance = tmpDistance;
                        container = block;
                    }
                }
            }

            if (container != null)
            {
                return container.Owner;
            }

            return m_default;
        }

        /// <summary>
        /// Calculates the distance between two points.
        /// Method uses Pythagoras theorem.
        /// </summary>
        /// <param name="point1">Start point.</param>
        /// <param name="point2">End point.</param>
        /// <returns>Distance between points.</returns>
        private int GetDistance(Point point1, Point point2)
        {
            int a = point1.X - point2.X;
            int b = point1.Y - point2.Y;

            return (a * a) + (b * b);
        }
        #endregion
    }
}
