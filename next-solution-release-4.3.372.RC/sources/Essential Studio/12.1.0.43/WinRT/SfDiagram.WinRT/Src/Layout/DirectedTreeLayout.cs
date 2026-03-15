#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Syncfusion.UI.Xaml.Diagram.Layout.Base;
#if WINRT_USING
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;
using System.Collections.ObjectModel;
using Syncfusion.UI.Xaml.Diagram.Controls; 
#endif

namespace Syncfusion.UI.Xaml.Diagram.Layout
{
    public class DirectedTreeLayout 
        : TreeLayoutBase
    {
        public TreeOrientation Orientation { get; set; }

        public override object LayoutRoot
        {
            get;
            set;
        }

        internal IInternalNode InternalLayoutRoot { get; set; }

        internal IGraphInternal Graph
        {
            get;
            set;
        }

        private List<IInternalNode> _mLevelOneNodes = new List<IInternalNode>();

        private IInternalNode _mFixedNode;

        #region Methods
        internal void SetX(IInternalNode item, IInternalNode referrer, double x)
        {
            item.OffsetX = x;
        }

        internal void SetY(IInternalNode item, IInternalNode referrer, double y)
        {
            item.OffsetY = y;
        }
        #endregion

        #region Fields

        private double[] _mDepths;
        private int _mMaxDepth;
        private Dictionary<object, LayoutInfo> _mEqualities;
        private double _mAnchorX, _mAnchorY;
        #endregion

        #region Methods

        public DirectedTreeLayout()
        {
            Orientation = TreeOrientation.TopToBottom;
            HorizotalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Top;
            HorizontalSpacing = 20;
            VerticalSpacing = 50;
            SpaceBetweenSubTrees = 20;
            Margin = new Thickness(50);
        }

        public override void UpdateLayout()
        {
            Graph = (this as IInternalLayout).Graph;
            if (Graph.InternalNodes!=null && Graph.InternalNodes.Count > 0)
            {
                if (this.LayoutRoot == null)
                {
                    _mLevelOneNodes = Graph.InternalNodes
                                          .Where(node => (node.InternalInNeighbors == null ||
                                                         node.InternalInNeighbors.Count() == 0)).ToList();

                }
                else
                {
                    _mLevelOneNodes.Clear();
                    _mLevelOneNodes.Add(Graph.GetNodeWrapper(LayoutRoot,false));
                }

                if (_mLevelOneNodes.Count > 0)
                {
                    InternalLayoutRoot = _mLevelOneNodes[0];
                    StartNodeArrangement();
                }
            }
        }

        public override void UpdateLayout(object fixedNode)
        {
            IGraphInternal graph = (this as IInternalLayout).Graph;
            _mFixedNode = graph.GetNodeWrapper(fixedNode, false);
            UpdateLayout();
            _mFixedNode = null;
            //Point fixedPoint = new Point(internalNode.OffsetX, internalNode.OffsetY); 
            //UpdateLayout();
            //Point delta = new Point(fixedPoint.X - internalNode.OffsetX, fixedPoint.Y - internalNode.OffsetY);
            //foreach (var node in graph.InternalNodes)
            //{
            //    node.OffsetX += delta.X;
            //    node.OffsetY += delta.Y;
            //}
        }

        protected virtual INodeInfo GetNextSibling(INode child)
        {
            return (child.Info as IInternalNode).NextSibling;
        }

        protected virtual INodeInfo GetPreviousSibling(INode child)
        {
            return (child.Info as IInternalNode).PreviousSibling;
        }

        protected virtual INodeInfo GetLastChild(INode parent)
        {
            return (parent.Info as IInternalNode).LastChild;
        }

        protected virtual INodeInfo GetFirstChild(INode parent)
        {
            return (parent.Info as IInternalNode).FirstChild;
        }

        private void StartNodeArrangement()
        {
            InitializeLayout();
            DoLayout();
        }

        private void InitializeLayout()
        {
            if (Graph == null)
                throw new Exception("Diagram Model is Empty");
            //InternalLayoutRoot = InternalLayoutRoot;
            //MakeTraversing(InternalLayoutRoot);

            _mEqualities = new Dictionary<object, LayoutInfo>();

            //foreach (IInternalNode node in Graph.SharedData.GlobalNodes.Values)
            //{
            //    LayoutInfo par = new LayoutInfo();
            //    par.SetupInfo(node);
            //    _mEqualities.Add(node, par);
            //}
        }

        private double GetSpace(IInternalNode l, IInternalNode r, bool siblings)
        {
            double hspace = 0;
            double stree = 0;
            bool w = (this.Orientation == TreeOrientation.TopToBottom || this.Orientation == TreeOrientation.BottomToTop);

            hspace = this.HorizontalSpacing;
            stree = this.SpaceBetweenSubTrees;

            return (siblings ? hspace : stree) + 0.5 * (w ? l.Rectangle.Width + r.Rectangle.Width : l.Rectangle.Height + r.Rectangle.Height);
        }

        private void UpdateDepths(int depth, IInternalNode item)
        {
            bool v = (this.Orientation == TreeOrientation.TopToBottom || this.Orientation == TreeOrientation.BottomToTop);
            double d = (v ? item.Rectangle.Height : item.Rectangle.Width);
            if (this.Orientation == TreeOrientation.BottomToTop || this.Orientation == TreeOrientation.RightToLeft)
            {
                d = -d;
                if (_mDepths.Length <= depth)
                    Array.Resize(ref _mDepths, (3 * depth / 2));

                _mDepths[depth] = Math.Min(_mDepths[depth], d);
            }
            else
            {
                if (_mDepths.Length <= depth)
                    Array.Resize(ref _mDepths, (3 * depth / 2));

                _mDepths[depth] = Math.Max(_mDepths[depth], d);
            }
            _mMaxDepth = Math.Max(_mMaxDepth, depth);
        }

        private void CheckDepths()
        {
            for (int i = 1; i < _mMaxDepth; ++i)
            {
                double vspace = 0;
                vspace = VerticalSpacing;
                if (Orientation == TreeOrientation.BottomToTop || Orientation == TreeOrientation.RightToLeft)
                    vspace = -vspace;
                _mDepths[i] += _mDepths[i - 1] + vspace;
            }
        }

        public void DoLayout()
        {
            _mDepths = new double[10];
            _mDepths.Initialize();

            _mAnchorX = 0;
            _mAnchorY = 0;
            _mMaxDepth = 0;

            if (_mLevelOneNodes.Count > 1)
            {
                DoFirstWalk(_mLevelOneNodes[0], 0, 1, null, null, _mLevelOneNodes[1]);
            }
            else
            {
                DoFirstWalk(_mLevelOneNodes[0], 0, 1, null, null, null);
            }

            CheckDepths();

            if (_mFixedNode == null)
            {
                double _min=0, _max=0;
                double _width,_height;
                FindLayoutBounds(ref _min, ref _max);

                if (Orientation == TreeOrientation.LeftToRight || Orientation == TreeOrientation.RightToLeft)
                {
                    _width = Math.Abs(_mDepths[_mMaxDepth - 1] + _mDepths[_mMaxDepth]);
                    _height = _max - _min;
                }
                else
                {
                    _height = Math.Abs(_mDepths[_mMaxDepth - 1] + _mDepths[_mMaxDepth]);
                    _width = _max - _min;
                }
                UpdateAnchor(_width, _height,_min);
            }
            else
            {
                UpdateAnchor();
            }
            
            LayoutInfo rp = _mEqualities[_mLevelOneNodes[0]];
            DoSecondWalk(_mLevelOneNodes[0], null, -rp.Prelim, 0);
        }
        
        private void FindLayoutBounds(ref double min,ref double max)
        {
            IInternalNode _lnode = _mLevelOneNodes[0];
            FindMinValue(ref min,-_mEqualities[_lnode].Prelim, _lnode, 0);
            if (_mLevelOneNodes.Count == 0)
                FindMaxValue(ref max,-_mEqualities[_mLevelOneNodes[_mLevelOneNodes.Count - 1]].Prelim, _mLevelOneNodes[_mLevelOneNodes.Count - 1], 0);
            else
            {
                FindMaxValue(ref max,-_mEqualities[InternalLayoutRoot].Prelim, _mLevelOneNodes[_mLevelOneNodes.Count - 1], 0);
            }
        }

        private void FindMinValue(ref double min,double space, IInternalNode node, int depth)
        {
            double _size=(Orientation==TreeOrientation.TopToBottom||Orientation==TreeOrientation.BottomToTop? node.Rectangle.Width / 2:node.Rectangle.Height/2);
            if (min >= space + _mEqualities[node].Prelim - _size)
            {
                min = space + _mEqualities[node].Prelim - _size;
                if (depth == _mMaxDepth - 1)
                {
                    return;
                }
            }

            if (node.IsExpanded && node.InternalChildren != null && node.InternalChildren.Count() > 0)
            {
                FindMinValue(ref min,space + _mEqualities[node].Mod, node.FirstChild, depth+1);
            }

            if ( depth>0)
            {
                if (_mEqualities[node].Mod < 0)
                {
                    if ( node.IsExpanded && (node.InternalChildren == null || node.InternalChildren.Count() == 0) && node.NextSibling != null && node.NextSibling.InternalChildren != null && node.NextSibling.InternalChildren.Count() > 0)
                    {
                        FindMinValue(ref min,space, node.NextSibling, depth);
                    }
                }
            }
            else
            {
                if (_mLevelOneNodes.Count > 1 && _mLevelOneNodes.Contains(node) && _mLevelOneNodes.IndexOf(node) <_mLevelOneNodes.Count-1)
                {
                    FindMinValue(ref min,space, _mLevelOneNodes[_mLevelOneNodes.IndexOf(node) + 1], 0);
                }
            }
        }

        private void FindMaxValue(ref double max,double space, IInternalNode node, int depth)
        {
            bool w  = Orientation == TreeOrientation.TopToBottom || Orientation == TreeOrientation.BottomToTop ;
            double _size=w? node.Rectangle.Width / 2 : node.Rectangle.Height / 2;
            if (max <= space + _mEqualities[node].Prelim + _size)
            {
                max = space + _mEqualities[node].Prelim + _size;
                if (depth == _mMaxDepth - 1)
                {
                    return;
                }
            }

            if (node.IsExpanded && node.InternalChildren != null && node.InternalChildren.Count() > 0)
            {
                FindMaxValue(ref max,space + _mEqualities[node].Mod, node.LastChild, depth+1);
            }

            if (depth > 0)
            {
                if (node.IsExpanded && (node.InternalChildren == null || node.InternalChildren.Count() == 0) && node.PreviousSibling != null && node.PreviousSibling.InternalChildren != null && node.PreviousSibling.InternalChildren.Count() > 0)
                {
                    FindMaxValue(ref max,space, node.PreviousSibling, depth+1);
                }
            }
            else
            {
                if (_mLevelOneNodes.Count > 1 && _mLevelOneNodes.Contains(node) && _mLevelOneNodes.IndexOf(node)>0)
                {
                    FindMaxValue(ref max,space, _mLevelOneNodes[_mLevelOneNodes.IndexOf(node) - 1], 0);
                }
            }
        }

        private void UpdateAnchor(double width, double height, double min)
        {
            Rect _bounds = Graph.ScrollInfo.Viewport;
            if (this.Bounds != null)
            {
                _bounds = this.Bounds.Value;
            }
            
            switch (this.HorizotalAlignment)
            {
                case HorizontalAlignment.Left:
                    _mAnchorX =_bounds.Left+Margin.Left;
                    break;
                case HorizontalAlignment.Center:
                    _mAnchorX = _bounds.Left+_bounds.Width/2 - width / 2;
                    break;
                case HorizontalAlignment.Right:
                    _mAnchorX = _bounds.Right - width-Margin.Right;
                    break;
            }

            switch (this.VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    _mAnchorY = _bounds.Top + Margin.Top;
                    break;
                case VerticalAlignment.Center:
                    _mAnchorY = _bounds.Top+ _bounds.Height / 2 - height / 2;
                    break;
                case VerticalAlignment.Bottom:
                    _mAnchorY = _bounds.Bottom - height-Margin.Bottom;
                    break;
            }

            switch (Orientation)
            {
                case TreeOrientation.TopToBottom:
                    _mAnchorX = _mAnchorX - min;
                    break;
                case TreeOrientation.BottomToTop:
                    _mAnchorY += height;
                    _mAnchorX = _mAnchorX - min;
                    break;
                case TreeOrientation.LeftToRight:
                    _mAnchorY = _mAnchorY-min;
                    break;
                case TreeOrientation.RightToLeft:
                    _mAnchorY = _mAnchorY - min;
                    _mAnchorX += width;
                    break;
            }
        }
         
        private void UpdateAnchor()
        {
           double diffx = 0;
           int depth = 0;
            //if (Orientation == TreeOrientation.BottomToTop || Orientation == TreeOrientation.TopToBottom)
            //{
            //    LayoutInfo rp = _mEqualities[_mLevelOneNodes[_mLevelOneNodes.Count - 1]];
            //    LayoutInfo rp1 = _mEqualities[_mLevelOneNodes[0]];
            //    _mAnchorX -= (rp.Prelim - rp1.Prelim) / 2;
            //}
            //else
            //{
            //    LayoutInfo rp = _mEqualities[_mLevelOneNodes[_mLevelOneNodes.Count - 1]];
            //    LayoutInfo rp1 = _mEqualities[_mLevelOneNodes[0]];
            //    _mAnchorY -= (rp.Prelim - rp1.Prelim) / 2;
            //}
            if (_mFixedNode != null)
            {
                IInternalNode fixednod = _mFixedNode;
                IInternalNode nod = fixednod;
                List<IInternalNode> list = new List<IInternalNode>();
                list.Add(nod);
                while (nod != null)
                {
                    depth++;
                    nod = nod.ParentNode;
                    if (nod != null)
                    {
                        list.Insert(0, nod);
                    }
                }
                foreach (IInternalNode n in list)
                {
                    LayoutInfo rp3 = _mEqualities[n as IInternalNode];
                    if (list.IndexOf(n) == 0)
                    {
                        LayoutInfo rinfo = _mEqualities[_mLevelOneNodes[0]];
                        diffx -= rinfo.Prelim;
                        if (n == fixednod)
                            diffx += rp3.Prelim;
                        else
                            diffx += rp3.Mod;
                    }
                    else
                    {
                        if (n != fixednod)
                        {
                            diffx += rp3.Mod;
                        }
                        else
                            diffx +=rp3.Prelim;
                    }
                }

                if (Orientation == TreeOrientation.BottomToTop || Orientation == TreeOrientation.TopToBottom)
                {
                    double diffy = _mAnchorY + _mDepths[depth - 1] - _mFixedNode.OffsetY;
                    diffx = _mAnchorX + diffx - _mFixedNode.OffsetX;
                    _mAnchorX -= diffx;
                    _mAnchorY -= diffy;
                    if (Orientation == TreeOrientation.BottomToTop)
                    {
                        _mAnchorY += ((depth - 1) * (fixednod.Rectangle.Height));
                        _mAnchorY += InternalLayoutRoot.Rectangle.Height / 2;
                    }
                    else
                    {
                        _mAnchorY -= InternalLayoutRoot.Rectangle.Height / 2;
                    }
                }
                else
                {
                     double diff = _mAnchorX + _mDepths[depth - 1] - _mFixedNode.OffsetX;
                     double diffy = _mAnchorY + diffx - _mFixedNode.OffsetY;
                    _mAnchorX -= diff;
                    _mAnchorY -= diffy;
                    if (Orientation == TreeOrientation.RightToLeft)
                    {
                        _mAnchorX += ((depth - 1) * (fixednod.Rectangle.Width));
                        _mAnchorX += InternalLayoutRoot.Rectangle.Width / 2;
                    }
                    else
                    {
                        _mAnchorX -= InternalLayoutRoot.Rectangle.Width / 2;
                    }
                }
            }
        }

        private void DoFirstWalk(IInternalNode shape, int number, int depth, IInternalNode parent,IInternalNode prev,IInternalNode next)
        {
            LayoutInfo layoutInfo = new LayoutInfo();
            layoutInfo.SetupInfo(shape);
            _mEqualities.Add(shape, layoutInfo);
            layoutInfo.Number = number;
            UpdateDepths(depth, shape);
            bool isExpanded = shape.IsExpanded;
            if (!shape.HasChild || !isExpanded)
            {
                IInternalNode l = prev;
                if (l == null)
                {
                    layoutInfo.Prelim = 0;
                }
                else
                {
                    layoutInfo.Prelim = _mEqualities[l].Prelim + GetSpace(l, shape, true);
                }
            }

            else if (isExpanded)
            {
                IInternalNode leftMostShape = GetFirstChild(shape.Source as INode)as IInternalNode;
                IInternalNode rightMostShape = GetLastChild(shape.Source as INode) as IInternalNode;
                IInternalNode defaultAncestor = leftMostShape;
                IInternalNode c = leftMostShape;
                for (int i = 0; c != null; ++i,  c = GetNextSibling(c.Source as INode) as IInternalNode)
                {
                    IInternalNode p=GetPreviousSibling(c.Source as INode) as IInternalNode;
                    IInternalNode n = GetNextSibling(c.Source as INode) as IInternalNode;
                    DoFirstWalk(c, i, depth + 1, shape, p,n);
                    defaultAncestor = AllocateSpace(c, defaultAncestor, shape, p,n);
                }

                TranslateShapePosition(shape);

                double midpoint = 0.5 *
                    (_mEqualities[leftMostShape].Prelim + _mEqualities[rightMostShape].Prelim);
                
                IInternalNode left = null ;
                if (_mLevelOneNodes.Contains(shape))
                {
                    if (_mLevelOneNodes.IndexOf(shape) > 0)
                    {
                        left = _mLevelOneNodes[_mLevelOneNodes.IndexOf(shape) - 1];
                    }
                }
                else
                {
                    if (GetPreviousSibling(shape.Source as INode) != null)
                        left = GetPreviousSibling(shape.Source as INode) as IInternalNode;
                }
                if (left != null)
                {
                    layoutInfo.Prelim = _mEqualities[left].Prelim + GetSpace(left, shape, true);
                    layoutInfo.Mod = layoutInfo.Prelim - midpoint;
                }
                else
                {
                    layoutInfo.Prelim = (midpoint);
                }
                
            }
            if (_mLevelOneNodes.Contains(shape) && _mLevelOneNodes.Count > 1)
                AllocateSpace(shape, _mLevelOneNodes[0], null, prev, next);

            //finding the next root level node

            if (_mLevelOneNodes.Contains(shape) && _mLevelOneNodes.Count > 1)
            {
                IInternalNode node = null;

                if (_mLevelOneNodes.IndexOf(shape) < _mLevelOneNodes.Count - 1)
                {
                    node = _mLevelOneNodes[_mLevelOneNodes.IndexOf(shape) + 1];
                }
                if (node != null)
                {
                    IInternalNode prevRoot, nextRoot;

                    if (_mLevelOneNodes.IndexOf(node) == 0)
                    {
                        prevRoot = null;
                    }
                    else
                    {
                        prevRoot = _mLevelOneNodes[_mLevelOneNodes.IndexOf(node) - 1];
                    }

                    if (_mLevelOneNodes.IndexOf(node) == _mLevelOneNodes.Count - 1)
                    {
                        nextRoot = null;
                    }
                    else
                    {
                        nextRoot = _mLevelOneNodes[_mLevelOneNodes.IndexOf(shape) + 1];
                    }

                    DoFirstWalk(node, 0, 1, null, prevRoot, nextRoot);
                }
            }
        }

        private IInternalNode AllocateSpace(IInternalNode v, IInternalNode a,IInternalNode parent,IInternalNode prev,IInternalNode next)
        {
            IInternalNode w = prev;
            if (w != null)
            {
                IInternalNode vip, vim, vop, vom;
                double sip, sim, sop, som;

                vip = vop = v;
                vim = w;
                vom=null;
                if (parent != null)
                    vom = GetFirstChild(parent.Source as INode) as IInternalNode;
                else
                {
                    vom = _mLevelOneNodes[0];
                }
                sip = _mEqualities[vip].Mod;
                sop = _mEqualities[vop].Mod;
                sim = _mEqualities[vim].Mod;
                som = _mEqualities[vom].Mod;
                IInternalNode nr = AdjacentRight(vim);
                IInternalNode nl = AdjacentLeft(vip);
                while (nr != null && nl != null)
                {
                    vim = nr;
                    vip = nl;
                    vom = AdjacentLeft(vom);
                    vop = AdjacentRight(vop);
                    LayoutInfo parms = _mEqualities[vop];
                    parms.Ancestor = v;
                    double shift = (_mEqualities[vim].Prelim + sim) -
                        (_mEqualities[vip].Prelim + sip) + GetSpace(vim, vip, false);
                    if (shift > 0)
                    {
                        ShiftSubTree(AncestorShape(vim, v, a), v, shift);
                        sip += shift;
                        sop += shift;
                    }
                    sim += _mEqualities[vim].Mod;
                    sip += _mEqualities[vip].Mod;
                    som += _mEqualities[vom].Mod;
                    sop += _mEqualities[vop].Mod;

                    nr = AdjacentRight(vim);
                    nl = AdjacentLeft(vip);
                }
                if (nr != null && AdjacentRight(vop) == null)
                {
                    LayoutInfo vopp = _mEqualities[vop];
                    vopp.Thread = nr;
                    vopp.Mod += sim - sop;
                }
                if (nl != null && AdjacentLeft(vom) == null)
                {
                    LayoutInfo vomp = _mEqualities[vom];
                    vomp.Thread = nl;
                    vomp.Mod += sip - som;
                    a = v;
                }
            }
            return a;
        }

        private IInternalNode AdjacentLeft(IInternalNode shape)
        {
            IInternalNode tempshape = null;
            if (shape.IsExpanded) tempshape = GetFirstChild(shape.Source as INode) as IInternalNode;
         
            return (tempshape ?? _mEqualities[shape].Thread);
        }

        private IInternalNode AdjacentRight(IInternalNode shape)
        {
            IInternalNode tempShape = null;
            if (shape.IsExpanded) tempShape = GetLastChild(shape.Source as INode) as IInternalNode;
            return (tempShape ?? _mEqualities[shape].Thread);
        }

        private void ShiftSubTree(IInternalNode shape, IInternalNode shape2, double shift)
        {
            LayoutInfo wmp = _mEqualities[shape];
            LayoutInfo wpp = _mEqualities[shape2];
            double distance = wpp.Number - wmp.Number;
            wpp.Change -= shift / distance;
            wpp.Shift += shift;
            wmp.Change += shift / distance;
            wpp.Prelim += shift;
            wpp.Mod += shift;
        }

        private void TranslateShapePosition(IInternalNode shape)
        {
            double shift = 0, change = 0;
            IInternalNode c;
            int index;
            for (c = GetLastChild(shape.Source as INode) as IInternalNode, index = shape.InternalChildren.Count() - 1; c != null; c=GetPreviousSibling(c.Source as INode) as IInternalNode,index--)
            {
                LayoutInfo cp = _mEqualities[c];
                cp.Prelim += shift;
                cp.Mod += shift;
                change += cp.Change;
                shift += cp.Shift + change;
            }
            
        }
       
        private IInternalNode AncestorShape(IInternalNode shape, IInternalNode shape1, IInternalNode AdjucentShape)
        {
            IInternalNode parentshape = shape1.ParentNode;
            LayoutInfo shapeLayoutInfo = _mEqualities[shape];
            if (shapeLayoutInfo.Ancestor != null && shapeLayoutInfo.Ancestor.ParentNode == parentshape)
            {
                return shapeLayoutInfo.Ancestor;
            }
            else
            {
                return AdjucentShape;
            }
        }

        private void DoSecondWalk(IInternalNode node, IInternalNode previousShape, double space, int depth)
        {
            LayoutInfo np = _mEqualities[node];
            if (Orientation == TreeOrientation.LeftToRight || Orientation == TreeOrientation.RightToLeft)
            {
                SetBreadthSpace(node, previousShape, _mDepths[depth], depth);
                SetDepthSpace(node, previousShape, np.Prelim + space, depth);
            }
            else
            {
                SetBreadthSpace(node, previousShape, np.Prelim + space, depth);
                SetDepthSpace(node, previousShape, _mDepths[depth], depth);
            }
            if (node.IsExpanded && node.HasChild)
            {
                depth += 1;
                int i = 0;
                for (IInternalNode c = GetFirstChild(node.Source as INode) as IInternalNode; c != null; c = GetNextSibling(c.Source as INode) as IInternalNode, i++)
                {
                    DoSecondWalk(c, node, space + np.Mod, depth);
                }
            }
            if (_mLevelOneNodes.Contains(node))
            {
                IInternalNode nextnode = null;

                if (_mLevelOneNodes.IndexOf(node) < _mLevelOneNodes.Count - 1)
                {
                    nextnode = _mLevelOneNodes[_mLevelOneNodes.IndexOf(node) + 1];
                }
                if (nextnode != null)
                {
                    DoSecondWalk(nextnode, null, space, 0);
                }
            }

            np.ClearInfo();
        }

        private void SetBreadthSpace(IInternalNode shapeNext, IInternalNode previousShape, double space, int depth)
        {
            if (Orientation == TreeOrientation.RightToLeft)
            {
                SetX(shapeNext, previousShape, _mAnchorX + space - shapeNext.Rectangle.Width / 2);
            }
            else
                if (Orientation == TreeOrientation.LeftToRight)
                {
                    SetX(shapeNext, previousShape, _mAnchorX + space + shapeNext.Rectangle.Width / 2);
                }
                else
                {
                    SetX(shapeNext, previousShape, _mAnchorX + space);
                }
        }

        private void SetDepthSpace(IInternalNode shapeNext, IInternalNode previousShape, double space, int depth)
        {
            if (Orientation == TreeOrientation.BottomToTop)
            {
                SetY(shapeNext, previousShape, _mAnchorY + space - shapeNext.Rectangle.Height / 2);
            }
            else if (Orientation == TreeOrientation.TopToBottom)
            {
                SetY(shapeNext, previousShape, _mAnchorY + space + shapeNext.Rectangle.Height / 2);
            }
            else
                SetY(shapeNext, previousShape, _mAnchorY + space);
        }
        #endregion
    }

    internal class LayoutInfo
    {
        internal double Prelim;
        internal double Mod;
        internal double Shift;
        internal double Change;
        internal int Number;
        internal IInternalNode Ancestor;
        internal IInternalNode Thread;

        public void SetupInfo(IInternalNode item)
        {
            Ancestor = item;
            Number = -1;
            Ancestor = Thread = null;
        }

        public void ClearInfo()
        {
            Number = -2;
            Prelim = Mod = Shift = Change = 0;
            Ancestor = Thread = null;
        }
    }

    public enum TreeOrientation
    {
        LeftToRight,
        RightToLeft,
        TopToBottom,
        BottomToTop
    }
}
