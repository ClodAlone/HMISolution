#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if WINRT_USING
using Windows.Foundation;
#else
using System.Windows;
#endif
using Syncfusion.UI.Xaml.Diagram.Utility;

namespace Syncfusion.UI.Xaml.Diagram.Controller
{
    internal class SpatialSearching : ISharedData
    {
        private SharedData _mSharedData;

        internal long _pageLeft, _pageRight, _pageTop, _pageBottom;
        internal IInternalGroupable LeftElement;
        internal IInternalGroupable TopElement;
        internal IInternalGroupable RightElement;
        internal IInternalGroupable BottomElement;
        private const long quadSize = 100;
        //private bool modified = false;
        internal List<Quad> quads = new List<Quad>();

        public void Init(SharedData shared)
        {
            _mSharedData = shared;
            ParentQuad = new Quad(0, 0, quadSize * 2, quadSize * 2, this);
            _pageLeft = long.MaxValue;
            _pageRight = long.MinValue;
            _pageTop = long.MaxValue;
            _pageBottom = long.MinValue;
        }

        internal Quad ParentQuad;

        public void Dispose()
        {
        }

        internal void RemoveFromAQuad(IInternalGroupable node)
        {
            if (node.Quad != null)
            {
                node.Quad.objects.Remove(node);
                Update(node.Quad);
                node.Quad = null;
            }
        }

        private void Update(Quad quad)
        {
            if (quad.Parent != null && quad.objects.Count == 0 && quad.First == null && quad.Second == null && quad.Third == null && quad.Fourth == null)
            {
                Quad parent = quad.Parent;
                if (parent.First == quad)
                {
                    parent.First = null;
                }
                else if (parent.Second == quad)
                {
                    parent.Second = null;
                }
                else if (parent.Third == quad)
                {
                    parent.Third = null;
                }
                else if (parent.Fourth == quad)
                {
                    parent.Fourth = null;
                }
                Update(quad.Parent);
            }
            else
                return;
        }

        internal void UpdateQuad(IInternalGroupable node)
        {
            SetCurrentNode(node);
            if (!(node is ISelector))
            {
                Rect nodBounds = node.Bounds;
                if (!(nodBounds.X.IsValid() && nodBounds.Y.IsValid() && nodBounds.Width.IsValid() && nodBounds.Height.IsValid()))
                    return;
                //nodBounds = new Rect(nodBounds.X.Valid(), nodBounds.Y.Valid(), nodBounds.Width.Valid(), nodBounds.Height.Valid());
                if (node.Quad != null)
                {
                    if (node is IInternalConnector && _mSharedData.Graph.CanVirtualize)
                    {
                        RemoveFromAQuad(node);
                        ParentQuad.AddIntoaQuad(node);
                    }
                    else
                    {
                        if (!node.Quad.IsContained())
                        {
                            RemoveFromAQuad(node);
                            ParentQuad.AddIntoaQuad(node);
                        }
                    }
                }
                else
                {
                    ParentQuad.AddIntoaQuad(node);
                }

                if (IsWithinPageBounds(ref nodBounds) &&
                    LeftElement != node &&
                    TopElement != node &&
                    RightElement != node &&
                    BottomElement != node)
                {
                }
                else
                {
                    bool modified = false;
                    if (_pageLeft >= childLeft)
                    {
                        _pageLeft = childLeft;
                        LeftElement = node;
                        modified = true;
                    }
                    else if (node == LeftElement)
                    {
                        _pageLeft = long.MaxValue;
                        FindLeft(ParentQuad);
                        modified = true;
                    }

                    if (_pageTop >= childTop)
                    {
                        _pageTop = childTop;
                        TopElement = node;
                        modified = true;
                    }
                    else if (node == TopElement)
                    {
                        _pageTop = long.MaxValue;
                        FindTop(ParentQuad);
                        modified = true;
                    }

                    if (_pageBottom <= childBottom)
                    {
                        modified = true;
                        _pageBottom = childBottom;
                        BottomElement = node;
                    }
                    else if (node == BottomElement)
                    {
                        _pageBottom = long.MinValue;
                        FindBottom(ParentQuad);
                        modified = true;
                    }

                    if (_pageRight <= childRight)
                    {
                        _pageRight = childRight;
                        RightElement = node;
                        modified = true;
                    }
                    else if (node == RightElement)
                    {
                        _pageRight = long.MinValue;
                        FindRight(ParentQuad);
                        modified = true;
                    }
                    if (modified)
                    {
                        UpdatePageBounds();
                        if (_mSharedData.ScrollViewer != null)
                        {
                            _mSharedData.ScrollViewer.InvalidateArrange();
                        }
                    }

                }

            }
        }

        private bool IsWithinPageBounds(ref Rect node)
        {
            if (node.Left >= _pageLeft && node.Right <= _pageRight && node.Top >= _pageTop && node.Bottom <= _pageBottom)
                return true;
            else
                return false;
        }

        internal List<Quad> FindQuads(Rect _mCurrentViewPort)
        {
            if (quads == null)
            {
                quads = new List<Quad>();
            }
            quads.Clear();
            ParentQuad.FindQuads(ref _mCurrentViewPort);
            return quads;
        }

        internal void UpdateBounds(IInternalGroupable node)
        {
            bool modified = false;
            if (node == _mSharedData.SpatialSearch.TopElement)
            {
                modified = true;
                _pageTop = long.MaxValue;
                FindTop(ParentQuad);
            }
            if (node == _mSharedData.SpatialSearch.LeftElement)
            {
                modified = true;
                _pageLeft = long.MaxValue;
                FindLeft(ParentQuad);
            }
            if (node == _mSharedData.SpatialSearch.RightElement)
            {
                modified = true;
                _pageRight = long.MinValue;
                FindRight(ParentQuad);
            }
            if (node == _mSharedData.SpatialSearch.BottomElement)
            {
                modified = true;
                _pageBottom = long.MinValue;
                FindBottom(ParentQuad);
            }
            if (modified)
            {
                UpdatePageBounds();
            }

        }

        internal void UpdatePageBounds()
        {
            if (_mSharedData.PageSettingsController != null)
            {
                Rect bounds;
                if (_pageLeft != long.MaxValue)
                    bounds = new Rect(_pageLeft, _pageTop, _pageRight - _pageLeft, _pageBottom - _pageTop);
                else
                    bounds = new Rect(0, 0, 0, 0);
                if (_mSharedData.PageSettingsController != null)
                    _mSharedData.PageSettingsController.UpdatePageBounds(bounds.Left, bounds.Right, bounds.Top, bounds.Bottom);

            }
            else
            {
                if (_mSharedData.ScrollViewer != null)
                {
                    ScrollChanged current = _mSharedData.ScrollViewer._mCurrentState;
                    Rect Bounds;
                    if (_pageLeft != long.MaxValue)
                        Bounds = new Rect(_pageLeft, _pageTop, _pageRight - _pageLeft, _pageBottom - _pageTop);
                    else
                        Bounds = new Rect(0, 0, 0, 0);
                    current.ContentBounds = Bounds;
                    current.PageBounds = Bounds;
                    _mSharedData.ScrollViewer.InvokeViewportChangedEvent(current);
                }
            }
            if (_mSharedData.ScrollViewer != null)
            {
                _mSharedData.ScrollViewer.InvalidateArrange();
            }

        }

        private void FindBottom(Quad quad)
        {
            //if (quad.Quads.Count == 4)
            {
                if (quad.Third != null || quad.Fourth != null)
                {
                    if (quad.Third != null)
                    {
                        FindBottom(quad.Third);
                    }
                    if (quad.Fourth != null)
                    {
                        FindBottom(quad.Fourth);
                    }
                }
                else
                {
                    if (quad.Second != null)
                    {
                        FindBottom(quad.Second);

                    }
                    if (quad.First != null)
                    {
                        FindBottom(quad.First);
                    }
                }
            }
            foreach (IInternalGroupable node in quad.objects)
            {
                if (_pageBottom <= node.Bounds.Bottom)
                {
                    _pageBottom = (long)node.Bounds.Bottom;
                    BottomElement = node;
                }
            }

        }

        private void FindRight(Quad quad)
        {

            //if (quad.Quads.Count == 4)
            {
                if (quad.Second != null || quad.Fourth != null)
                {
                    if (quad.Second != null)
                    {
                        FindRight(quad.Second);

                    }
                    if (quad.Fourth != null)
                    {
                        FindRight(quad.Fourth);
                    }
                }
                //else
                {
                    if (quad.First != null)
                    {
                        FindRight(quad.First);

                    }
                    if (quad.Third != null)
                    {
                        FindRight(quad.Third);
                    }
                }
            }

            foreach (IInternalGroupable node in quad.objects)
            {
                if (_pageRight <= node.Bounds.Right)
                {
                    _pageRight = (long)node.Bounds.Right;
                    RightElement = node;
                }
            }
        }

        private void FindLeft(Quad quad)
        {

            //if (quad.Quads.Count == 4)
            {
                if (quad.First != null || quad.Third != null)
                {
                    if (quad.First != null)
                    {
                        FindLeft(quad.First);
                    }
                    if (quad.Third != null)
                    {
                        FindLeft(quad.Third);
                    }
                }
                else
                {
                    if (quad.Second != null)
                    {
                        FindLeft(quad.Second);
                    }
                    if (quad.Fourth != null)
                    {
                        FindLeft(quad.Fourth);
                    }
                }
            }
            foreach (IInternalGroupable node in quad.objects)
            {
                if (_pageLeft >= node.Bounds.Left)
                {
                    _pageLeft = (long)node.Bounds.Left;
                    LeftElement = node;
                }
            }
        }

        private void FindTop(Quad quad)
        {
            //if (quad.Quads.Count == 4)
            {
                if (quad.First != null || quad.Second != null)
                {
                    if (quad.First != null)
                    {
                        FindTop(quad.First);
                    }
                    if (quad.Second != null)
                    {
                        FindTop(quad.Second);
                    }
                }
                else
                {
                    if (quad.Third != null)
                    {
                        FindTop(quad.Third);
                    }
                    if (quad.Fourth != null)
                    {
                        FindTop(quad.Fourth);
                    }
                }
            }
            foreach (IInternalGroupable node in quad.objects)
            {
                if (_pageTop >= node.Bounds.Top)
                {
                    _pageTop = (long)node.Bounds.Top;
                    TopElement = node;
                }
            }
        }

        internal IInternalGroupable childnode;
        internal long childLeft;
        internal long childTop;
        internal long childRight;
        internal long childBottom;

        internal void SetCurrentNode(IInternalGroupable node)
        {
            childnode = node;
            Rect r = node.Bounds;
            childLeft = (long)r.Left.Valid();
            childTop = (long)r.Top.Valid();
            childRight = (long)r.Right.Valid();
            childBottom = (long)r.Bottom.Valid();
        }
    }

    internal class Quad
    {
        internal List<object> objects;
        //internal Rect Bounds;
        public long Left;
        public long Top;
        public long Width;
        public long Height;
        internal Quad First;
        internal Quad Second;
        internal Quad Third;
        internal Quad Fourth;
        internal Quad Parent;
        private SpatialSearching _mSpatialSearching;

        public Quad(long left, long top, long width, long height, SpatialSearching spatialSearching)
        {
            objects = new List<object>();
            Left = left;
            Top = top;
            Width = width;
            Height = height;
            //Quads = new Quad[4];
            _mSpatialSearching = spatialSearching;
        }

        internal void FindQuads(ref Rect _mCurrentViewPort)
        {
            //if (First != null)
            {
                if (First != null && First.IsIntersect(ref _mCurrentViewPort))
                {
                    First.FindQuads(ref _mCurrentViewPort);
                }

                if (Second != null && Second.IsIntersect(ref _mCurrentViewPort))
                {
                    Second.FindQuads(ref _mCurrentViewPort);
                }

                if (Third != null && Third.IsIntersect(ref _mCurrentViewPort))
                {
                    Third.FindQuads(ref _mCurrentViewPort);
                }

                if (Fourth != null && Fourth.IsIntersect(ref _mCurrentViewPort))
                {
                    Fourth.FindQuads(ref _mCurrentViewPort);
                }
            }
            if (objects.Count > 0)
                _mSpatialSearching.quads.Add(this);
        }

        private bool IsIntersect(ref Rect t)
        {
            if (Left + Width < t.Left || Top + Height < t.Top || Left > t.Right || Top > t.Bottom)
            {
                return false;
            }
            return true;
        }

        internal void SelectQuad()
        {
            Quad current = this;
            while (current != null)
            {
                current = current.GetQuad();
            }
        }

        private Quad GetQuad()
        {
            //Rect child = childnode.Bounds;
            //Rect parent = parentquad.Bounds;
            //double childWidth = childnode.Bounds.Width;
            //double childHeight = childnode.Bounds.Height;
            long halfWidth = Width / 2;
            long halfHeight = Height / 2;
            if (//_mSpatialSearching.child.Width <= halfWidth && _mSpatialSearching.child.Height <= halfHeight &&
                halfWidth >= 100 && halfHeight >= 100)
            {
                long xCenter = Left + halfWidth;
                long yCenter = Top + halfHeight;

                if (_mSpatialSearching.childRight <= xCenter)
                {
                    if (_mSpatialSearching.childBottom <= yCenter)
                    {
                        return First ??
                               (First = new Quad(Left, Top, halfWidth, halfHeight, _mSpatialSearching) { Parent = this });
                    }
                    if (_mSpatialSearching.childTop >= yCenter)
                    {
                        return Third ??
                               (Third = new Quad(Left, yCenter, halfWidth, halfHeight, _mSpatialSearching) { Parent = this });
                    }
                }
                else if (_mSpatialSearching.childLeft >= xCenter)
                {
                    if (_mSpatialSearching.childBottom <= yCenter)
                    {
                        return Second ??
                               (Second = new Quad(xCenter, Top, halfWidth, halfHeight, _mSpatialSearching) { Parent = this });
                    }
                    if (_mSpatialSearching.childTop >= yCenter)
                    {
                        return Fourth ??
                               (Fourth = new Quad(xCenter, yCenter, halfWidth, halfHeight, _mSpatialSearching) { Parent = this });
                    }
                }
            }
            objects.Add(_mSpatialSearching.childnode);
            _mSpatialSearching.childnode.Quad = this;
            return null;
        }

        internal bool IsContained()
        {
            if (_mSpatialSearching.childLeft >= Left && _mSpatialSearching.childRight <= Left + Width &&
                _mSpatialSearching.childTop >= Top && _mSpatialSearching.childBottom <= Top + Height)
            {
                return true;
            }
            return false;
        }

        internal void AddIntoaQuad(IInternalGroupable node)
        {
            bool isAdded = false;
            _mSpatialSearching.SetCurrentNode(node);

            while (!isAdded)
            {
                isAdded = _mSpatialSearching.ParentQuad.Add();
            }
        }

        private bool Add()
        {
            if (IsContained())
            {
                SelectQuad();
                return true;
            }
            else
            {
                Quad newParent;
                bool isempty = this.objects.Count == 0 && First == null && Second == null && Third == null && Fourth == null;
                if (_mSpatialSearching.childLeft < Left)
                {
                    if (_mSpatialSearching.childTop < Top)
                    {
                        newParent = new Quad(Left - Width, Top - Height,
                                             Width * 2, Height * 2,
                                             _mSpatialSearching);
                        if (!isempty)
                            newParent.Fourth = this;
                    }
                    else
                    {
                        newParent = new Quad(Left - Width, Top,
                                             Width * 2, Height * 2,
                                             _mSpatialSearching);
                        if (!isempty)
                            newParent.Second = this;
                    }
                }
                else if (_mSpatialSearching.childTop < Top)
                {
                    newParent = new Quad(Left, Top - Height,
                                         Width * 2, Height * 2,
                                         _mSpatialSearching);
                    if (!isempty)
                        newParent.Third = this;
                }
                else
                {
                    newParent = new Quad(Left, Top,
                                         Width * 2, Height * 2,
                                         _mSpatialSearching);
                    if (!isempty)
                        newParent.First = this;
                }
                this.Parent = newParent;
                _mSpatialSearching.ParentQuad = newParent;
                return false;
                //newParent.AddIntoaQuad(node);
            }
        }
    }
}

