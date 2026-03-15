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

using System;
using System.Collections;
using System.Collections.Generic;

#if DOCIO
namespace Syncfusion.CompoundFile.DocIO.Net
#else
namespace Syncfusion.CompoundFile.XlsIO.Net
#endif
{
    /// <summary>
    /// Suitable Node colors used for 2-3-4 nodes detection.
    /// </summary>
    public enum NodeColor
    {
        /// <summary>
        /// Red color of node.
        /// </summary>
        Red,
        /// <summary>
        /// Black color of node.
        /// </summary>
        Black
    }

    /// <summary>
    /// Node class used for proper storing of data in the Map Collection.
    /// </summary>
    public class RBTreeNode
    {
        #region RBTreeNode Class members
        /// <summary>
        /// Reference on left branch.
        /// </summary>
        private RBTreeNode m_left;
        /// <summary>
        /// Reference on right branch.
        /// </summary>
        private RBTreeNode m_right;
        /// <summary>
        /// Reference on parent branch.
        /// </summary>
        private RBTreeNode m_parent;
        /// <summary>
        /// Color of node branch.
        /// </summary>
        private NodeColor m_color;
        /// <summary>
        /// Is current node Nil element or not?
        /// </summary>
        private bool m_bIsNil;
        /// <summary>
        /// Key part of stored in node data.
        /// </summary>
        private object m_key;
        /// <summary>
        /// Value part of stored in node data.
        /// </summary>
        private object m_value;
        #endregion

        #region RBTreeNode Class properties
        /// <summary>
        /// Reference on left branch.
        /// </summary>
        public RBTreeNode Left
        {
            get
            {
                return m_left;
            }
            set
            {
                m_left = value;
            }
        }
        /// <summary>
        /// Reference on right branch.
        /// </summary>
        public RBTreeNode Right
        {
            get
            {
                return m_right;
            }
            set
            {
                m_right = value;
            }
        }
        /// <summary>
        /// Reference on parent branch.
        /// </summary>
        public RBTreeNode Parent
        {
            get
            {
                return m_parent;
            }
            set
            {
                m_parent = value;
            }
        }
        /// <summary>
        /// Color of node branch.
        /// </summary>
        public NodeColor Color
        {
            get
            {
                return m_color;
            }
            set
            {
                m_color = value;
            }
        }
        /// <summary>
        /// Is current node Nil element or not?
        /// </summary>
        public bool IsNil
        {
            get
            {
                return m_bIsNil;
            }
            set
            {
                m_bIsNil = value;
            }
        }
        /// <summary>
        /// Key part of stored in node data.
        /// </summary>
        public object Key
        {
            get
            {
                return m_key;
            }
            set
            {
                m_key = value;
            }
        }
        /// <summary>
        /// Value part of stored in node data.
        /// </summary>
        public object Value
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = value;
            }
        }
        /// <summary>
        /// Is current node set to red color?
        /// </summary>
        public bool IsRed
        {
            get
            {
                return (this.Color == NodeColor.Red);
            }
        }
        /// <summary>
        /// Is current node set to black color?
        /// </summary>
        public bool IsBlack
        {
            get
            {
                return (this.Color == NodeColor.Black);
            }
        }
        #endregion

        #region RBTreeNode Class Initialize/Finalize methods
        /// <summary>
        /// Create red colored Tree node.
        /// </summary>
        /// <param name="left">Reference on left branch.</param>
        /// <param name="parent">Reference on parent branch.</param>
        /// <param name="right">Refernce on right branch.</param>
        /// <param name="key">Key value of node.</param>
        /// <param name="value">Value part of node.</param>
        public RBTreeNode(RBTreeNode left, RBTreeNode parent, RBTreeNode right,
          object key, object value)
            : this(left, parent, right, key, value, NodeColor.Red)
        {

        }
        /// <summary>
        /// Main constructor of class.
        /// </summary>
        /// <param name="left">Reference on left branch.</param>
        /// <param name="parent">Reference on parent branch.</param>
        /// <param name="right">Refernce on right branch.</param>
        /// <param name="key">Key value of node.</param>
        /// <param name="value">Value part of node.</param>
        /// <param name="color">Color of node.</param>
        public RBTreeNode(RBTreeNode left, RBTreeNode parent, RBTreeNode right,
          object key, object value, NodeColor color)
        {
            m_left = left;
            m_parent = parent;
            m_right = right;
            m_key = key;
            m_value = value;
            m_color = color;
        }
        #endregion

    }
    /// <summary></summary>
    public class MapCollection : IEnumerable
    {
        #region Class members
        /// <summary>
        /// TODO: place correct comment here
        /// </summary>
        private RBTreeNode m_MyHead;
        /// <summary>
        /// TODO: place correct comment here
        /// </summary>
        private int m_size;
        /// <summary>
        /// TODO: place correct comment here
        /// </summary>
        private IComparer m_comparer = Comparer<object>.Default;
        #endregion

        #region Class properties
        /// <summary>
        /// TODO: place correct comment here
        /// </summary>
        public RBTreeNode Empty
        {
            get
            {
                return m_MyHead;
            }
        }
        /// <summary>
        /// TODO: place correct comment here
        /// </summary>
        public int Count
        {
            get
            {
                return m_size;
            }
        }

        /// <summary>
        /// TODO: place correct comment here
        /// </summary>
        public object this[object key]
        {
            get
            {
                RBTreeNode node = LBound(key);

                if (m_comparer.Compare(node.Key, key) == 0)
                    return node.Value;

                return null;
            }
            set
            {
                RBTreeNode node = LBound(key);

                if (node.IsNil || m_comparer.Compare(node.Key, key) != 0)
                {
                    this.Add(key, value);
                }
                else
                {
                    node.Value = value;
                }
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// TODO: place correct comment here
        /// </summary>
        public MapCollection()
        {
            Initialize();
        }

        /// <summary>
        /// Create collection with specified comparer for Key values.
        /// </summary>
        /// <param name="comparer">Comparer for key values.</param>
        public MapCollection(IComparer comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            Initialize();

            m_comparer = comparer;
        }
        /// <summary>
        /// Create Empty node for collection.
        /// </summary>
        protected void Initialize()
        {
            m_MyHead = new RBTreeNode(null, null, null, null, null, NodeColor.Black);
            m_MyHead.IsNil = true;
            m_MyHead.Parent = m_MyHead.Left = m_MyHead.Right = m_MyHead;
            m_size = 0;
        }

        #endregion

        #region Class Public Methods
        /// <summary>
        /// Clear collection.
        /// </summary>
        public void Clear()
        {
            Erase(m_MyHead.Parent);
            m_MyHead.Parent = m_MyHead.Left = m_MyHead.Right = m_MyHead;
            m_size = 0;
        }

        /// <summary>
        /// Add item into collection.
        /// </summary>
        /// <param name="key">Key part.</param>
        /// <param name="value">Value.</param>
        public void Add(object key, object value)
        {
            RBTreeNode _try = m_MyHead.Parent;
            RBTreeNode _where = m_MyHead;
            bool _addLeft = true;

            while (!_try.IsNil)
            {
                _where = _try;
                _addLeft = m_comparer.Compare(key, _try.Key) < 0;
                _try = (_addLeft) ? _try.Left : _try.Right;
            }

            RBTreeNode _iter = _where;

            if (!_addLeft)
            {
                // nothing here
            }
            else if (_where == begin())
            {
                Insert(true, _where, key, value);
                return;
            }
            else
            {
                _iter = Dec(_iter);
            }

            Insert(_addLeft, _where, key, value);
        }

        /// <summary>
        /// Check whether collection contains specified key.
        /// </summary>
        /// <returns>True if node with specified key is found; otherwise False.</returns>
        /// <param name="key">Key for check.</param>
        public bool Contains(object key)
        {
            RBTreeNode node = LBound(key);
            return (node != m_MyHead && m_comparer.Compare(node.Key, key) == 0);
        }

        /// <summary>
        /// Remove from collection item with specified key.
        /// </summary>
        /// <param name="key">Key to identify item.</param>
        public void Remove(object key)
        {
            RBTreeNode _erase = LBound(key);

            // nothing to remove
            if (_erase.IsNil) return;

            RBTreeNode _fix, _fixParent;
            RBTreeNode _node = _erase;

            if (_node.Left.IsNil)
            {
                _fix = _node.Right;
            }
            else if (_node.Right.IsNil)
            {
                _fix = _node.Left;
            }
            else
            {
                _node = Inc(_erase);
                _fix = _node.Right;
            }

            if (_node == _erase)
            {
                _fixParent = _erase.Parent;
                if (!_fix.IsNil) _fix.Parent = _fixParent;

                if (m_MyHead.Parent == _erase)
                {
                    m_MyHead.Parent = _fix;
                }
                else if (_fixParent.Left == _erase)
                {
                    _fixParent.Left = _fix;
                }
                else
                {
                    _fixParent.Right = _fix;
                }

                if (m_MyHead.Left == _erase)
                    m_MyHead.Left = (_fix.IsNil) ? _fixParent : Min(_fix);

                if (m_MyHead.Right == _erase)
                    m_MyHead.Right = (_fix.IsNil) ? _fixParent : Max(_fix);
            }
            else
            {
                _erase.Left.Parent = _node;
                _node.Left = _erase.Left;

                if (_node == _erase.Right)
                {
                    _fixParent = _node;
                }
                else
                {
                    _fixParent = _node.Parent;

                    if (!_fix.IsNil) _fix.Parent = _fixParent;

                    _fixParent.Left = _fix;
                    _node.Right = _erase.Right;
                    _erase.Right.Parent = _node;
                }

                if (m_MyHead.Parent == _erase)
                {
                    m_MyHead.Parent = _node;
                }
                else if (_erase.Parent.Left == _erase)
                {
                    _erase.Parent.Left = _node;
                }
                else
                {
                    _erase.Parent.Right = _node;
                }

                _node.Parent = _erase.Parent;

                NodeColor clr = _erase.Color;
                _erase.Color = _node.Color;
                _node.Color = clr;
            }

            if (_erase.Color == NodeColor.Black)
            {
                for (; _fix != m_MyHead.Parent && _fix.Color == NodeColor.Black;
                  _fixParent = _fix.Parent)
                {
                    if (_fix == _fixParent.Left)
                    {
                        _node = _fixParent.Right;

                        if (_node.Color == NodeColor.Red)
                        {
                            _node.Color = NodeColor.Black;
                            _fixParent.Color = NodeColor.Red;
                            LRotate(_fixParent);
                            _node = _fixParent.Right;
                        }

                        if (_node.IsNil)
                        {
                            _fix = _fixParent;
                        }
                        else if (_node.Left.Color == NodeColor.Black && _node.Right.Color == NodeColor.Black)
                        {
                            _node.Color = NodeColor.Red;
                            _fix = _fixParent;
                        }
                        else
                        {
                            if (_node.Right.Color == NodeColor.Black)
                            {
                                _node.Left.Color = NodeColor.Black;
                                _node.Color = NodeColor.Red;
                                RRotate(_node);
                                _node = _fixParent.Right;
                            }

                            _node.Color = _fixParent.Color;
                            _fixParent.Color = NodeColor.Black;
                            _node.Right.Color = NodeColor.Black;
                            LRotate(_fixParent);
                            break;
                        }
                    }
                    else
                    {
                        _node = _fixParent.Left;

                        if (_node.Color == NodeColor.Red)
                        {
                            _node.Color = NodeColor.Black;
                            _fixParent.Color = NodeColor.Red;
                            RRotate(_fixParent);
                            _node = _fixParent.Left;
                        }

                        if (_node.IsNil)
                        {
                            _fix = _fixParent;
                        }
                        else if (_node.Right.Color == NodeColor.Black && _node.Left.Color == NodeColor.Black)
                        {
                            _node.Color = NodeColor.Red;
                            _fix = _fixParent;
                        }
                        else
                        {
                            if (_node.Left.Color == NodeColor.Black)
                            {
                                _node.Right.Color = NodeColor.Black;
                                _node.Color = NodeColor.Red;
                                LRotate(_node);
                                _node = _fixParent.Left;
                            }

                            _node.Color = _fixParent.Color;
                            _fixParent.Color = NodeColor.Black;
                            _node.Left.Color = NodeColor.Black;
                            RRotate(_fixParent);
                            break;
                        }
                    }
                }

                _fix.Color = NodeColor.Black;
            }

            if (0 < m_size) --m_size;
        }

        #endregion

        #region Class helper methods
        /// <summary>
        /// TODO: place correct comment here
        /// </summary>
        /// <returns>
        /// TODO: place correct comment here
        /// </returns>
        private RBTreeNode begin()
        {
            return m_MyHead.Left;
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Get minimum value for specified branch.
        /// </summary>
        /// <param name="node">Branch start node.</param>
        /// <returns>Reference on minimum value node.</returns>
        public static RBTreeNode Min(RBTreeNode node)
        {
            while (!node.Left.IsNil)
            {
                node = node.Left;
            }

            return node;
        }

        /// <summary>
        /// Get maximum value for specified branch.
        /// </summary>
        /// <param name="node">Branch start node.</param>
        /// <returns>Reference on maximum value node.</returns>
        public static RBTreeNode Max(RBTreeNode node)
        {
            while (!node.Right.IsNil)
            {
                node = node.Right;
            }

            return node;
        }

        /// <summary>
        /// Go to to next item in collection.
        /// </summary>
        /// <param name="node">Start node.</param>
        /// <returns>Reference on next item in collection or this.Empty if nothing found.</returns>
        public static RBTreeNode Inc(RBTreeNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (node.IsNil) return node;

            if (!node.Right.IsNil)
            {
                node = Min(node.Right);
            }
            else
            {
                RBTreeNode _nd;

                while (!(_nd = node.Parent).IsNil && node == _nd.Right)
                {
                    node = _nd;
                }

                node = _nd;
            }

            return node;
        }
        /// <summary>
        /// Get previous item from collection.
        /// </summary>
        /// <param name="node">Start node.</param>
        /// <returns>Rererence on previous item in collection.</returns>
        public static RBTreeNode Dec(RBTreeNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (node.IsNil)
            {
                node = node.Right;
            }
            else if (!node.Left.IsNil)
            {
                node = Max(node.Left);
            }
            else
            {
                RBTreeNode _node;
                while (!(_node = node.Parent).IsNil && _node == _node.Left)
                {
                    node = _node;
                }

                if (!_node.IsNil) node = _node;
            }

            return node;
        }
        /// <summary>
        /// Find node in collection by key value (search in lower side).
        /// </summary>
        /// <param name="key">Key of node to find.</param>
        /// <returns>Reference on found node, otherwise this.Empty value.</returns>
        protected RBTreeNode LBound(object key)
        {
            RBTreeNode _root = m_MyHead.Parent;
            RBTreeNode _where = m_MyHead;

            while (!_root.IsNil)
            {
                if (m_comparer.Compare(_root.Key, key) < 0)
                {
                    _root = _root.Right;
                }
                else
                {
                    _where = _root;
                    _root = _root.Left;
                }
            }

            return _where;
        }

        /// <summary>
        /// Find node in collection by key value (search in upper side).
        /// </summary>
        /// <param name="key">Key of node to find.</param>
        /// <returns>Reference on found node, otherwise this.Empty value.</returns>
        protected RBTreeNode UBound(object key)
        {
            RBTreeNode _root = m_MyHead.Parent;
            RBTreeNode _where = m_MyHead;

            while (!_root.IsNil)
            {
                if (m_comparer.Compare(key, _root.Key) < 0)
                {
                    _where = _root;
                    _root = _root.Left;
                }
                else
                {
                    _root = _root.Right;
                }
            }

            return _where;
        }
        /// <summary>
        /// Rotate branch into left side.
        /// </summary>
        /// <param name="_where">Branch start node.</param>
        protected void LRotate(RBTreeNode _where)
        {
            RBTreeNode _node = _where.Right;
            _where.Right = _node.Left;

            if (!_node.Left.IsNil)
            {
                _node.Left.Parent = _where;
            }

            _node.Parent = _where.Parent;

            if (_where == m_MyHead.Parent)
            {
                m_MyHead.Parent = _node;
            }
            else if (_where == _where.Parent.Left)
            {
                _where.Parent.Left = _node;
            }
            else
            {
                _where.Parent.Right = _node;
            }

            _node.Left = _where;
            _where.Parent = _node;
        }
        /// <summary>
        /// Rotate branch into right side.
        /// </summary>
        /// <param name="_where">Branch start node.</param>
        protected void RRotate(RBTreeNode _where)
        {
            RBTreeNode _node = _where.Left;
            _where.Left = _node.Right;

            if (!_node.Right.IsNil)
            {
                _node.Right.Parent = _where;
            }

            _node.Parent = _where.Parent;

            if (_where == m_MyHead.Parent)
            {
                m_MyHead.Parent = _node;
            }
            else if (_where == _where.Parent.Right)
            {
                _where.Parent.Right = _node;
            }
            else
            {
                _where.Parent.Left = _node;
            }

            _node.Right = _where;
            _where.Parent = _node;
        }
        /// <summary>
        /// Erase node from collection.
        /// </summary>
        /// <param name="_root">Item to erase.</param>
        protected void Erase(RBTreeNode _root)
        {
            for (RBTreeNode _node = _root; !_node.IsNil; _root = _node)
            {
                Erase(_node.Right);
                _node = _node.Left;
            }
        }
        /// <summary>
        /// Insert item into collection.
        /// </summary>
        /// <param name="_addLeft">Add into left side of tree or right.</param>
        /// <param name="_where">Node for placement.</param>
        /// <param name="key">Key part of node.</param>
        /// <param name="value">Value part of node.</param>
        protected void Insert(bool _addLeft, RBTreeNode _where, object key, object value)
        {
            RBTreeNode _new = new RBTreeNode(m_MyHead, _where, m_MyHead, key, value);
            m_size++;

            if (_where == m_MyHead)
            {
                m_MyHead.Parent = m_MyHead.Left = m_MyHead.Right = _new;
            }
            else if (_addLeft)
            {
                _where.Left = _new;

                if (_where == m_MyHead.Left)
                {
                    m_MyHead.Left = _new;
                }
            }
            else
            {
                _where.Right = _new;

                if (_where == m_MyHead.Right)
                {
                    m_MyHead.Right = _new;
                }
            }

            for (RBTreeNode _node = _new; _node.Parent.Color == NodeColor.Red; )
            {
                if (_node.Parent == _node.Parent.Parent.Left)
                {
                    _where = _node.Parent.Parent.Right;

                    if (_where.Color == NodeColor.Red)
                    {
                        _node.Parent.Color = NodeColor.Black;
                        _where.Color = NodeColor.Black;
                        _node.Parent.Parent.Color = NodeColor.Red;
                        _node = _node.Parent.Parent;
                    }
                    else
                    {
                        if (_node == _node.Parent.Right)
                        {
                            _node = _node.Parent;
                            LRotate(_node);
                        }

                        _node.Parent.Color = NodeColor.Black;
                        _node.Parent.Parent.Color = NodeColor.Red;
                        RRotate(_node.Parent.Parent);
                    }
                }
                else
                {
                    _where = _node.Parent.Parent.Left;

                    if (_where.Color == NodeColor.Red)
                    {
                        _node.Parent.Color = NodeColor.Black;
                        _where.Color = NodeColor.Black;
                        _node.Parent.Parent.Color = NodeColor.Red;
                        _node = _node.Parent.Parent;
                    }
                    else
                    {
                        if (_node == _node.Parent.Left)
                        {
                            _node = _node.Parent;
                            RRotate(_node);
                        }

                        _node.Parent.Color = NodeColor.Black;
                        _node.Parent.Parent.Color = NodeColor.Red;
                        LRotate(_node.Parent.Parent);
                    }
                }
            }

            m_MyHead.Parent.Color = NodeColor.Black;
        }
        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns enumerator.
        /// </summary>
        /// <returns>Returns enumerator of current interface.</returns>
        public IEnumerator GetEnumerator()
        {
            return new MapEnumerator(m_MyHead.Left);
        }

        #endregion

        internal delegate void NodeFunction(RBTreeNode node);

        internal void ForAll(NodeFunction function)
        {
            ForAll(m_MyHead, function);
        }

        private void ForAll(RBTreeNode startNode, NodeFunction function)
        {
            if (startNode == null)
                return;

            if (startNode.IsNil && startNode.Left.IsNil && startNode.Right.IsNil)
                return;

            if (!startNode.IsNil)
                function(startNode);

            ForAll(startNode.Left, function);
            ForAll(startNode.Right, function);
            //throw new Exception( "The method or operation is not implemented." );
        }
    }

    /// <summary>
    /// TODO: place correct comment here
    /// </summary>
    public class MapEnumerator : IEnumerator
    {
        #region MapEnumerator Class members
        /// <summary>
        /// TODO: place correct comment here
        /// </summary>
        private RBTreeNode m_current;
        /// <summary>
        /// TODO: place correct comment here
        /// </summary>
        private RBTreeNode m_parent;
        #endregion

        #region MapEnumerator Class properties
        /// <summary>
        /// TODO: place correct comment here
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                return m_current;
            }
        }

        /// <summary>
        /// TODO: place correct comment here
        /// </summary>
        public RBTreeNode Current
        {
            get
            {
                return m_current;
            }
        }

        #endregion

        #region MapEnumerator Class Initialize/Finalize methods
        /// <summary>
        /// TODO: place correct comment here
        /// </summary>
        /// <param name="parent"/>
        public MapEnumerator(RBTreeNode parent)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            m_parent = parent;
            //m_current = m_parent;
        }

        #endregion

        #region MapEnumerator Class Public Methods
        /// <summary></summary>
        public void Reset()
        {
            m_current = null;
        }

        /// <summary></summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            if (m_current == null)
            {
                m_current = m_parent;
            }
            else
            {
                m_current = MapCollection.Inc(m_current);
            }

            return (m_current != null && !m_current.IsNil);
        }

        #endregion
    }
}
