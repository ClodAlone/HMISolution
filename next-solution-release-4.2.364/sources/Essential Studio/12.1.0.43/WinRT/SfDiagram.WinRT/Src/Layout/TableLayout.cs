#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using Syncfusion.UI.Xaml.Diagram.Layout.Base;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if WINRT
using System.Threading.Tasks;
using Windows.Foundation; 
#endif
using System.Collections.ObjectModel;

namespace Syncfusion.UI.Xaml.Diagram.Layout
{
    public class TableLayout : LayoutBase
    {
        #region Fields

        private Point LayoutAnchor { get; set; }

        #region Public Properties

        public double HorizontalSpacing { get; set; }

        public double VerticalSpacing { get; set; }

        public bool EnableLayoutWithVariedSizes { get; set; }

        public ExpandMode TableExpandMode { get; set; }

        public int ColumnCount { get; set; }

        public int RowCount { get; set; }
        #endregion

        internal IInternalNode InternalLayoutRoot { get; set; }

        internal IGraphInternal Graph
        {
            get;
            set;
        }
        #endregion

        #region Methods

        public TableLayout()
        {
            HorizontalSpacing = 10;
            VerticalSpacing = 20;
        }

        public override void UpdateLayout()
        {
            this.PrepareActivity();
            this.StartNodeArrangement();
        }

        void SetToBounds()
        {
            LayoutAnchor = new Point(Graph.Viewport.Width / 2, 50);
        }

        public void PrepareActivity()
        {
            Graph = (this as IInternalLayout).Graph;
        }

        public void StartNodeArrangement()
        {
            if (Graph.InternalNodes.Count != 0)
            {
                if (this.EnableLayoutWithVariedSizes)
                {
                    this.DoLayoutWIthDifffentSizes();
                }
                else
                {
                    this.DoLayout();
                }
            }
        }

        public void DoLayoutWIthDifffentSizes()
        {
            int row = 1;
            int column = 0;
            if (this.TableExpandMode == ExpandMode.Horizontal)
            {
                if (this.ColumnCount == 0)
                {
                    this.ColumnCount = 1;
                }

                this.RowCount = (int)Math.Ceiling((Graph.InternalNodes.Count / (double)this.ColumnCount));
            }
            else
            {
                if (this.RowCount == 0)
                {
                    this.RowCount = 1;
                }

                this.ColumnCount = (int)Math.Ceiling((Graph.InternalNodes.Count / (double)this.RowCount));
                row = 0;
                column = 1;
            }

            IInternalNode[,] array = new IInternalNode[this.RowCount, this.ColumnCount];
            SetToBounds();
            int rowcount = 1;
            int columncount = 1;
            ObservableCollection<Size> rowsizecollection = new ObservableCollection<Size>();
            ObservableCollection<Size> columnsizecollection = new ObservableCollection<Size>();
            foreach (IInternalNode shape in Graph.InternalNodes)
            {
                if (this.TableExpandMode == ExpandMode.Horizontal)
                {
                    if (columncount <= this.ColumnCount)
                    {
                        column++;
                        shape.TempX = row;
                        shape.TempY = column;
                        columncount++;
                    }
                    else
                    {
                        row++;
                        columncount = 2;
                        column = 1;
                        shape.TempX = row;
                        shape.TempY = column;
                    }
                }
                else
                {
                    if (rowcount <= this.RowCount)
                    {
                        row++;
                        shape.TempX = row;
                        shape.TempY = column;
                        rowcount++;
                    }
                    else
                    {
                        column++;
                        rowcount = 2;
                        row = 1;
                        shape.TempX = row;
                        shape.TempY = column;
                    }
                }

                array[Convert.ToInt32(shape.TempX) - 1, Convert.ToInt32(shape.TempY) - 1] = shape;
            }

            this.CalculateMaximumSize(array, rowsizecollection, columnsizecollection);

            this.LayoutNodes(array, rowsizecollection, columnsizecollection);
        }

        public void DoLayout()
        {
            SetToBounds();
            int rowcount = 1;
            int columncount = 1;
            DiagramCollection rootnodecollection = new DiagramCollection();
            rootnodecollection.Add(Graph.InternalNodes.ToList<IInternalNode>().ToList()[0]);
            foreach (IInternalNode shape in Graph.InternalNodes)
            {

                if (this.TableExpandMode == ExpandMode.Horizontal)
                {
                    if (this.ColumnCount == 0)
                    {
                        this.ColumnCount = 1;
                    }

                    if (columncount <= ColumnCount)
                    {
                        this.SetBreadthSpace(shape);
                        if (PreviousShape(shape) != null)
                        {
                            this.SetY(shape, null, (PreviousShape(shape) as IInternalNode).OffsetY);
                        }
                        else
                        {
                            this.SetY(shape, null, this.LayoutAnchor.Y);
                        }

                        columncount++;
                    }
                    else
                    {
                        rowcount++;
                        double y = (rootnodecollection[rootnodecollection.Count - 1] as IInternalNode).OffsetY + (rootnodecollection[rootnodecollection.Count - 1] as IInternalNode).ActualHeight + this.VerticalSpacing;

                        this.SetY(shape, null, y);
                        this.SetX(shape, null, this.LayoutAnchor.X);
                        rootnodecollection.Add(shape);
                        columncount = 2;
                    }
                }
                else
                {
                    if (this.RowCount == 0)
                    {
                        this.RowCount = 1;
                    }

                    if (rowcount <= this.RowCount)
                    {
                        this.SetDepthSpace(shape);
                        if (PreviousShape(shape) != null)
                        {

                            this.SetX(shape, null, (PreviousShape(shape) as IInternalNode).OffsetX);
                        }
                        else
                        {
                            this.SetX(shape, null, this.LayoutAnchor.X);
                        }

                        rowcount++;
                    }
                    else
                    {
                        columncount++;
                        double x = (rootnodecollection[rootnodecollection.Count - 1] as IInternalNode).OffsetX + (rootnodecollection[rootnodecollection.Count - 1] as IInternalNode).ActualWidth + this.HorizontalSpacing;
                        this.SetX(shape, null, x);
                        this.SetY(shape, null, this.LayoutAnchor.Y);
                        rootnodecollection.Add(shape);
                        rowcount = 2;
                    }
                }
            }
        }

        void SetX(IInternalNode item, IInternalNode referrer, double x)
        {
            item.OffsetX = x;
        }

        void SetY(IInternalNode item, IInternalNode referrer, double y)
        {
            item.OffsetY = y;
        }

        private void SetBreadthSpace(IInternalNode currentShape)
        {
            double a = 0;

            if (PreviousShape(currentShape) != null)
            {

                a = (PreviousShape(currentShape) as IInternalNode).OffsetX + (PreviousShape(currentShape) as IInternalNode).ActualWidth + this.HorizontalSpacing;
            }
            else
            {
                a = this.LayoutAnchor.X;
            }

            this.SetX(currentShape, null, a);
        }

        private object PreviousShape(IInternalNode currentShape)
        {
            int ind = 0;
            if (Graph.InternalNodes != null)
            {

                ind = this.Graph.InternalNodes.ToList<IInternalNode>().IndexOf(currentShape);
            }
            if (ind == 0)
            {
                return null;
            }
            else
            {
                return Graph.InternalNodes.ToList<IInternalNode>()[ind - 1];
            }
        }

        private void SetDepthSpace(IInternalNode currentShape)
        {
            double a = 0;

            if (PreviousShape(currentShape) != null)
            {

                a = (PreviousShape(currentShape) as IInternalNode).OffsetY + (PreviousShape(currentShape) as IInternalNode).ActualHeight + this.VerticalSpacing;

            }
            else
            {
                a = this.LayoutAnchor.Y;
            }

            this.SetY(currentShape, null, a);
        }

        private void CalculateMaximumSize(IInternalNode[,] array, ObservableCollection<Size> rowsizecollection, ObservableCollection<Size> columnsizecollection)
        {
            double maxwidth = 0;
            double maxheight = 0;
            for (int i = 0; i < this.RowCount; i++)
            {
                for (int j = 0; j < this.ColumnCount; j++)
                {
                    if ((array[i, j]) != null)
                    {
                        if ((array[i, j]).ActualWidth > maxwidth)
                        {
                            maxwidth = (array[i, j]).ActualWidth;
                        }

                        if ((array[i, j]).ActualHeight > maxheight)
                        {
                            maxheight = (array[i, j]).ActualHeight;
                        }
                    }
                }

                rowsizecollection.Add(new Size(maxwidth, maxheight));
                maxwidth = maxheight = 0;
            }

            for (int i = 0; i < this.ColumnCount; i++)
            {
                for (int j = 0; j < this.RowCount; j++)
                {
                    if ((array[j, i]) != null)
                    {
                        if ((array[j, i]).ActualWidth > maxwidth)
                        {
                            maxwidth = (array[j, i]).ActualWidth;
                        }

                        if ((array[j, i]).ActualHeight > maxheight)
                        {
                            maxheight = (array[j, i]).ActualHeight;
                        }
                    }
                }

                columnsizecollection.Add(new Size(maxwidth, maxheight));
                maxwidth = maxheight = 0;
            }
        }

        private void LayoutNodes(IInternalNode[,] array, ObservableCollection<Size> rowsizecollection, ObservableCollection<Size> columnsizecollection)
        {
            for (int i = 0; i < this.ColumnCount; i++)
            {
                for (int j = 0; j < this.RowCount; j++)
                {
                    if ((array[j, i]) != null)
                    {
                        IInternalNode previousNode;
                        double previousrowHeight = 0;
                        if (j != 0)
                        {
                            previousNode = array[j - 1, i];
                            previousrowHeight = rowsizecollection[j - 1].Height;
                        }
                        else
                        {
                            previousNode = null;
                        }

                        double y = this.GetY(array[j, i], rowsizecollection[j].Height, previousNode, previousrowHeight);
                        this.SetY((array[j, i]), null, y);
                    }
                }
            }

            for (int i = 0; i < this.RowCount; i++)
            {
                for (int j = 0; j < this.ColumnCount; j++)
                {
                    if ((array[i, j]) != null)
                    {
                        IInternalNode previousNode;
                        double previouscolumnwidth = 0;
                        if (j != 0)
                        {
                            previousNode = array[i, j - 1];
                            previouscolumnwidth = columnsizecollection[j - 1].Width;
                        }
                        else
                        {
                            previousNode = null;
                        }

                        double x = this.GetX((array[i, j]), columnsizecollection[j].Width, previousNode, previouscolumnwidth);
                        this.SetX((array[i, j]), null, x);
                    }
                }
            }
        }

        private double GetX(IInternalNode node, double maxwidth, IInternalNode previousNode, double previouscolumnwidth)
        {
            double center = maxwidth / 2;
            double nodecenter = node.ActualWidth / 2;
            double diff = center - nodecenter;
            double a = 0;
            if (previousNode != null)
            {

                double posx = previousNode.OffsetX + (previouscolumnwidth / 2);
                a = posx + (previousNode.ActualWidth / 2) + this.HorizontalSpacing;
            }
            else
            {
                a = this.LayoutAnchor.X;
            }

            return a + diff;
        }

        private double GetY(IInternalNode node, double maxheight, IInternalNode previousNode, double previousrowHeight)
        {
            double center = maxheight / 2;
            double nodecenter = node.ActualHeight / 2;
            double diff = center - nodecenter;
            double a = 0;
            if (previousNode != null)
            {

                double posy = previousNode.OffsetY + (previousrowHeight / 2);
                a = posy + (previousNode.ActualHeight / 2) + this.VerticalSpacing;
            }
            else
            {
                a = this.LayoutAnchor.Y;
            }

            return a + diff;
        } 
        #endregion
    }

    /// <summary>
    /// Specifies the Expand Mode.
    /// </summary>
    public enum ExpandMode
    {
        /// <summary>
        /// Horizontal expansion.
        /// </summary>
        Horizontal,

        /// <summary>
        /// Vertical expansion.
        /// </summary>
        Vertical
    }
}
