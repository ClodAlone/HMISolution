#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using Syncfusion.UI.Xaml.Diagram.Layout.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
#if WINRT_USING
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;

#endif
namespace Syncfusion.UI.Xaml.Diagram.Layout
{
    public class RadialTreeLayout : TreeLayoutBase
    {
        #region Fields

        internal TreeOrientation Orientation { get; set; }

        internal List<IInternalNode> layoutNodes = new List<IInternalNode>();

        private double leastX;

        private double leastY;

        double maxX;

        double maxY;
        internal IInternalNode InternalLayoutRoot { get; set; }

        internal IGraphInternal Graph
        {
            get;
            set;
        }

        private ObservableCollection<Level> Levels = new ObservableCollection<Level>();

        private Point leftLast = new Point(0, 0);

        public override object LayoutRoot
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public RadialTreeLayout()
        {
            Orientation = TreeOrientation.TopToBottom;
            HorizotalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalSpacing = 20;
            VerticalSpacing = 50;
            SpaceBetweenSubTrees = 10;
            Margin = new Thickness(50);
        }

        private struct Level
        {
            internal ObservableCollection<IInternalNode> Nodes;
            internal double min;
            internal double max;
            internal double Height;
            internal ObservableCollection<double> Ratio;
            internal double Circumference;
            internal double radius;
        }

        private bool IsValidLayout
        {
            get
            {
                if (InternalLayoutRoot != null)
                {
                    if (Graph.InternalNodes != null)
                    {
                        var maxParents = from IInternalNode n in Graph.InternalNodes
                                          where n.InternalInNeighbors != null
                                          select n.InternalInNeighbors.Count();
                        if (maxParents!=null && maxParents.Count()>0 && maxParents.Max()<2)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
       
        private void ClearUnwantedStages()
        {
            foreach (IInternalNode n in Graph.InternalNodes)
            {
                if (!layoutNodes.Contains(n))
                {
                    n.Stage = -1;
                }
            }
        }

        private void MoveOtherNodesAway()
        {
            double x = (from IInternalNode n in layoutNodes
                        select n.OffsetX).Min();
            double y = (from IInternalNode n in layoutNodes
                        select n.OffsetY).Max() + 100;
            foreach (IInternalNode n in Graph.InternalNodes)
            {
                if (!layoutNodes.Contains(n))
                {
                    n.OffsetX = x;
                    n.OffsetY = y;
                    x += 50 + n.ActualWidth;
                }
            }
        }

        private void SetToBounds()
        {
            //Point a = new Point(Graph.ScrollInfo.Viewport.Width / 2-InternalLayoutRoot.ActualWidth/2, 
            //    Graph.ScrollInfo.Viewport.Height/2-InternalLayoutRoot.ActualHeight/2);
            //leastX = -a.X;
            //leastY = -a.Y;
            double width = maxX - leastX;
            double height = maxY - leastY;
            Rect bounds = Bounds ?? Graph.ScrollInfo.Viewport;
            switch (HorizotalAlignment)
            {
                case HorizontalAlignment.Center:
                    leastX = bounds.Left+bounds.Width / 2;
                    break;
                case HorizontalAlignment.Left:
                    leastX = bounds.Left + width / 2 + Margin.Left;
                    break;
                case HorizontalAlignment.Right:
                    leastX = bounds.Right - width / 2-Margin.Right;
                    break;
            }
            switch (VerticalAlignment)
            {
                case VerticalAlignment.Center:
                    leastY = bounds.Top + bounds.Height / 2;
                    break;
                case VerticalAlignment.Top:
                    leastY = bounds.Top + height / 2+Margin.Top;
                    break;
                case VerticalAlignment.Bottom:
                    leastY = bounds.Bottom - height / 2 - Margin.Bottom;
                    break;
            }
            leastX += InternalLayoutRoot.ActualWidth / 2;
            leastY += InternalLayoutRoot.ActualHeight / 2;
            for (int i = 0; i < Levels.Count; i++)
            {
                for (int j = 0; j < Levels[i].Nodes.Count; j++)
                {
                    Levels[i].Nodes[j].OffsetX = Levels[i].Nodes[j].TempX + leastX;
                    Levels[i].Nodes[j].OffsetY = Levels[i].Nodes[j].TempY + leastY;
                }
            }
        }

        //private void AssignParent()
        //{
        //    foreach (IInternalNode n in Graph.InternalNodes)
        //    {
        //        if (n.InNeighbors.Count > 0)
        //        {
        //            n.ParentNode = n.InNeighbors[0] as IInternalNode;
        //        }
        //    }
        //}

        private void TransformToCircleLayout()
        {
            InternalLayoutRoot.TempX = 0;
            InternalLayoutRoot.TempY = 0;
            InternalLayoutRoot.TempX -= InternalLayoutRoot.ActualWidth / 2;
            InternalLayoutRoot.TempY -= InternalLayoutRoot.ActualHeight / 2;
            for (int i = 1; i < Levels.Count; i++)
            {
                for (int j = 0; j < Levels[i].Nodes.Count; j++)
                {
                    Levels[i].Nodes[j].TempX = Math.Cos(Levels[i].Ratio[j] * 360 * Math.PI / 180) * (Levels[i].radius
                                                        + this.VerticalSpacing * i);
                    Levels[i].Nodes[j].TempY = Math.Sin(Levels[i].Ratio[j] * 360 * Math.PI / 180) * (Levels[i].radius
                                                        + this.VerticalSpacing * i);


                    Levels[i].Nodes[j].TempX -= Levels[i].Nodes[j].ActualWidth / 2;
                    Levels[i].Nodes[j].TempY -= Levels[i].Nodes[j].ActualHeight / 2;

                    leastX = Math.Min(leastX, Levels[i].Nodes[j].TempX);
                    leastY = Math.Min(leastY, Levels[i].Nodes[j].TempY);

                    maxX = Math.Max(maxX, Levels[i].Nodes[j].TempX + Levels[i].Nodes[j].ActualWidth);
                    maxY = Math.Max(maxY, Levels[i].Nodes[j].TempY + Levels[i].Nodes[j].ActualHeight);
                }
            }
            leastX = Math.Min(leastX, InternalLayoutRoot.TempX);
            leastY = Math.Min(leastY, InternalLayoutRoot.TempY);// - InternalLayoutRoot.ActualHeight);

            maxX = Math.Max(maxX, InternalLayoutRoot.TempX+ InternalLayoutRoot.ActualWidth);
            maxY = Math.Max(maxY, InternalLayoutRoot.TempY + InternalLayoutRoot.ActualHeight);
        }

        private void DepthFirstAllignment(IInternalNode parent)
        {
            if (parent != null && parent.InternalChildren!=null)
            {
                if ((parent.InternalChildren as List<IInternalNode>).Count > 0)
                {
                    leftLast.Y = leftLast.Y + 300;
                    foreach (IInternalNode n in parent.InternalChildren)
                    {
                        DepthFirstAllignment(n);
                    }

                    double min = (from IInternalNode n in parent.InternalChildren
                                  select n.TempX).Min();
                    double max = (from IInternalNode n in parent.InternalChildren
                                  select n.TempX
                                  + n.ActualWidth
                                  ).Max();
                    parent.TempX = min + (max - min) / 2;
                    parent.SegmentOffset = max + this.HorizontalSpacing;
                    leftLast.X = max + this.HorizontalSpacing;
                    parent.TempX -= parent.ActualWidth / 2;
                    parent.TempY -= parent.ActualHeight / 2;

                    if (parent.TempX < min && parent.SubTreeVal)
                    {
                        parent.TempX = min;
                        leftLast.X = parent.TempX + parent.ActualWidth / 2 - (max - min) / 2;
                        parent.SubTreeVal = false;
                        DepthFirstAllignment(parent);
                        parent.SubTreeVal = true;
                        leftLast.X = parent.TempX
                            + parent.ActualWidth
                            + this.HorizontalSpacing;
                    }
                    max = (from IInternalNode n in parent.InternalChildren
                           select n.SegmentOffset
                                  ).Max();
                    leftLast.X = leftLast.X < max ? max : leftLast.X;
                    leftLast.Y = leftLast.Y - 300;
                    parent.TempY = leftLast.Y;
                }
                else
                {
                    parent.TempX = leftLast.X;
                    parent.TempY = leftLast.Y;
                    leftLast.X = leftLast.X
                            + parent.ActualWidth
                            + this.HorizontalSpacing;

                }
            }
        }

        private void PopulateLevels()
        {
            double min = (from IInternalNode n in layoutNodes
                          select n.TempX).Min();
            double max = (from IInternalNode n in layoutNodes
                          select n.TempX
                            + n.ActualWidth
                            + this.HorizontalSpacing).Max();

            double full = max - min;

            List<IGrouping<Int32, IInternalNode>> StageGrouped = (from IInternalNode n in layoutNodes
                                                         orderby n.Stage
                                                         where n.Stage >= 0
                                                         group n by n.Stage into grouping
                                                         select grouping).ToList();

            Levels.Clear();

            foreach (IGrouping<Int32, IInternalNode> Stages in StageGrouped)
            {
                Level newlevel = new Level();
                List<IInternalNode> ordered = (from IInternalNode n in Stages
                                      orderby n.TempX
                                      select n).ToList();
                newlevel.Nodes = new ObservableCollection<IInternalNode>();
                newlevel.Ratio = new ObservableCollection<double>();
                double x1 = (from IInternalNode n in Stages
                             select n.TempX).Min();

                double x2 = (from IInternalNode n in Stages
                             select n.TempX
                             + n.ActualWidth
                             + this.HorizontalSpacing
                             ).Max();

                newlevel.Height = (from IInternalNode n in Stages
                                   select n.ActualHeight).Max();
                newlevel.Circumference = x2 - x1;
                newlevel.min = x1;
                newlevel.max = x2;

                newlevel.radius = newlevel.Circumference / (2 * Math.PI) + newlevel.Height;
                foreach (IInternalNode Stage in Stages)
                {
                    newlevel.Nodes.Add(Stage);
                    newlevel.Ratio.Add((Math.Abs(Stage.TempX + Stage.ActualWidth / 2 - min) / full));
                }

                if (Stages.Key > 1)
                {
                    if (Levels[Stages.Key - 1].radius + Levels[Stages.Key - 1].Height >= newlevel.radius)
                    {
                        newlevel.radius = Levels[Stages.Key - 1].radius + Levels[Stages.Key - 1].Height;
                    }
                }

                Levels.Add(newlevel);
            }
        }

        private void UpdateStages(IInternalNode parent)
        {
            if (parent != null && parent.InternalChildren!=null)
            {
                layoutNodes.Add(parent);
                parent.Visited = true;
                foreach (IInternalNode child in parent.InternalChildren)
                {
                    if (!child.Visited && child.ParentNode.Equals(parent))
                    {
                        child.Stage = parent.Stage + 1;
                    }
                }
                foreach (IInternalNode child in parent.InternalChildren)
                {
                    if (!child.Visited && child.ParentNode.Equals(parent))
                    {
                        UpdateStages(child);
                    }
                }
            }
        }

        private void clearVisited()
        {
            foreach (IInternalNode n in Graph.InternalNodes)
            {
                n.Visited = false;
            }
        }

        internal void PrepareActivity()
        {
            Graph = (this as IInternalLayout).Graph;
            if (Graph.InternalNodes != null)
            {
                if (LayoutRoot == null)
                {
                    InternalLayoutRoot = Graph.InternalNodes.FirstOrDefault(node => node.InternalInNeighbors == null || node.InternalInNeighbors.Count() == 0);
                }
                else
                {
                    InternalLayoutRoot = Graph.GetNodeWrapper(LayoutRoot, false);
                }
            }
        }
        
        public override void UpdateLayout()
        {
            PrepareActivity();

            if (IsValidLayout && InternalLayoutRoot.InternalChildren!=null && InternalLayoutRoot.InternalChildren.Count()>0)
            {
                //AssignParent();
                layoutNodes.Clear();
                clearVisited();
                UpdateStages(InternalLayoutRoot);
                ClearUnwantedStages();
                leftLast = new Point(0, 0);
                leastX = double.MaxValue;
                leastY = double.MaxValue;
                maxX = 0;
                maxY = 0;
                DepthFirstAllignment(InternalLayoutRoot);

                PopulateLevels();
                TransformToCircleLayout();
                SetToBounds();
                MoveOtherNodesAway();
            }
        }

        public override void UpdateLayout(object fixedNode)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
