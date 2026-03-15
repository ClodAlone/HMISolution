#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Collections.ComponentModel;
using System;
using System.Collections;
using System.Diagnostics;

namespace Syncfusion.UI.Xaml.Collections
{
    /// <summary>
    /// Used by TreeTable to balance the tree with algorithm based on Red-Black tree.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public enum TreeTableNodeColor
    {
        /// <summary>
        /// Red.
        /// </summary>
        Red,
        /// <summary>
        /// Black.
        /// </summary>
        Black
    }

    /// <summary>
    /// A branch or leaf in the tree.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public interface ITreeTableNode
    {
        /// <summary>
        /// Gets / sets the parent branch.
        /// </summary>
        ITreeTableBranch Parent { get; set; }

        /// <summary>
        /// returns the position in the tree.
        /// </summary>
        int GetPosition();

        /// <summary>
        /// Returns the number of child nodes (+1 for the current node).
        /// </summary>
        int GetCount();

        /// <summary>
        /// Indicates whether leaf is empty.
        /// </summary>
        bool IsEmpty();

        /// <summary>
        /// Indicates whether this is a leaf.
        /// </summary>
        bool IsEntry();

        /// <summary>
        /// Returns the tree level of this node.
        /// </summary>
        int GetLevel();

        /// <summary>
        /// Returns the minimum value (of the leftmost leaf) of the branch in a sorted tree.
        /// </summary>
        object GetMinimum();

        /// <summary>
        /// Walk up parent branches and reset counters.
        /// </summary>
        /// <param name="notifyParentRecordSource"></param>
        void InvalidateCounterBottomUp(bool notifyParentRecordSource);

        /// <summary>
        /// Walk up parent branches and reset summaries.
        /// </summary>
        /// <param name="notifyParentRecordSource"></param>
        void InvalidateSummariesBottomUp(bool notifyParentRecordSource);
    }

    /// <summary>
    /// A branch with left and right leaves or branches.
    /// </summary>
    public interface ITreeTableBranch : ITreeTableNode
    {
        /// <summary>
        /// Gets / sets the left node.
        /// </summary>
        ITreeTableNode Left { get; set; }

        /// <summary>
        /// Sets the left node.
        /// </summary>
        /// <param name="value">The new node.</param>
        /// <param name="inAddMode">Indicates whether tree-table is in add-mode.</param>
        /// <param name="isSortedTree">Indicates whether tree-table is sorted.</param>
        /// <remarks>
        /// Call this method instead of simply setting <see cref="Left"/> property if you want
        /// to avoid the round-trip call to check whether the tree is in add-mode
        /// or if tree-table is sorted.
        /// </remarks>
        void SetLeft(ITreeTableNode value, bool inAddMode, bool isSortedTree);

        /// <summary>
        /// Gets / sets the right node.
        /// </summary>
        ITreeTableNode Right { get; set; }

        /// <summary>
        /// Sets the right node.
        /// </summary>
        /// <param name="value">The new node.</param>
        /// <param name="inAddMode">Specifies if tree-table is in add-mode.</param>
        /// <remarks>
        /// Call this method instead of simply setting <see cref="Right"/> property if you want
        /// to avoid the round-trip call to check whether the tree is in add-mode
        /// or if tree-table is sorted.
        /// </remarks>
        void SetRight(ITreeTableNode value, bool inAddMode);

        /// <summary>
        /// Returns the left branch cast to ITreeTableBranch.
        /// </summary>
        /// <returns></returns>
        ITreeTableBranch GetLeftB();

        /// <summary>
        /// Returns the right branch cast to ITreeTableBranch.
        /// </summary>
        /// <returns></returns>
        ITreeTableBranch GetRightB();

        /// <summary>
        /// Gets / sets the Red-Black tree color.
        /// </summary>
        TreeTableNodeColor Color { get; set; }

        /// <summary>
        /// Returns the position in the tree table of the specified child node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        int GetEntryPositionOfChild(ITreeTableNode node);

        /// <summary>
        /// Sets this object's child node Count dirty and
        /// marks parent nodes' child node Count dirty.
        /// </summary>
        void InvalidateCountBottomUp();

        /// <summary>
        /// Sets this object's child node Count dirty and steps
        /// through all child branches and marks their child node Count dirty.
        /// </summary>
        void InvalidateCountTopDown();

        /// <summary>
        /// Sets this object's child node Minimum dirty and
        /// marks parent nodes' child node Minimum dirty.
        /// </summary>
        void InvalidateMinimumBottomUp();

        /// <summary>
        /// Sets this object's child node Minimum dirty and steps
        /// through all child branches and marks their child node Minimum dirty.
        /// </summary>
        void InvalidateMinimumTopDown();
    }

    /// <summary>
    /// A leaf with value and optional sort key.
    /// </summary>
    public interface ITreeTableEntry : ITreeTableNode, IDisposable
    {
        /// <summary>
        /// Returns the sort key of this leaf.
        /// </summary>
        object GetSortKey();

        /// <summary>
        /// Gets / sets the value attached to this leaf.
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// Creates a branch that can hold this entry when new leaves are inserted into the tree.
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        ITreeTableBranch CreateBranch(TreeTable tree);
    }


    /// <summary>
    /// A branch or leaf in the tree.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public abstract class TreeTableNode : Disposable, ITreeTableNode
    {
        ITreeTableBranch _parent;
        internal TreeTable _tree;
        internal static readonly object emptyMin = new object();

        /// <summary>
        /// Gets / sets the tree this node belongs to.
        /// </summary>
        public TreeTable Tree
        {
            get
            {
                return _tree;
            }
            set
            {
                _tree = value;
            }
        }


        /// <summary>
        /// Gets / sets the parent branch.
        /// </summary>
        public ITreeTableBranch Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;

                Debug.Assert(_parent != this);
                Debug.Assert(Parent == null || Parent.Parent == null || Parent.Parent != Parent);
                Debug.Assert(Parent == null || Parent.Parent == null || Parent.Parent != this);
                //Debug.Assert(!(this is ITreeTableBranch) || ((ITreeTableBranch)this).Left.IsEntry || ((TreeTableBranch)(((TreeTableBranch))this).Left).Right == this);
                //Debug.Assert(Parent == null || this.Parent.Left.IsEntry || ((TreeTableBranch)this.Parent.Left).Right == this);
                Debug.Assert(!(this is ITreeTableBranch) || Parent == null || Parent.Parent == null || ((TreeTableBranch)this).Right != Parent.Parent.Right);
            }
        }

        /// <summary>
        /// Returns the position in the tree.
        /// </summary>
        public virtual int GetPosition()
        {
            if (Parent == null)
                return 0;
            return Parent.GetEntryPositionOfChild(this);
        }

        /// <summary>
        /// Returns the minimum value (of the most-left leaf) of the branch in a sorted tree.
        /// </summary>
        public virtual object GetMinimum()
        {
            return emptyMin;
        }

        /// <summary>
        /// Indicates whether leaf is empty.
        /// </summary>
        public virtual bool IsEmpty()
        {
            return GetCount() == 0;
        }

        /// <summary>
        /// Indicates whether this is a leaf.
        /// </summary>
        public abstract bool IsEntry();

        /// <summary>
        /// Returns the number of child nodes (+1 for the current node).
        /// </summary>
        public abstract int GetCount();

        /// <summary>
        /// Returns the tree level of this node.
        /// </summary>
        public int GetLevel()
        {
            int level = 0;
            if (Parent != null)
                level = Parent.GetLevel() + 1;
            return level;
        }

        /// <summary>
        /// Returns the Debug / text information about the node.
        /// </summary>
        public virtual string GetNodeInfo()
        {
            string side = "_";
            if (Parent != null)
            {
                side = Object.ReferenceEquals(Parent.Left, this) ? "L" : "R";
            }
            return GetLevel().ToString() + "," + side + "," + GetPosition().ToString() + ", " + GetCount().ToString();
        }

        /// <summary>
        /// Returns the Debug / text information about the node.
        /// </summary>
        public override string ToString()
        {
            return GetType().Name + "{" + GetNodeInfo() + "}";
        }


        /// <summary>
        /// Walks up parent branches and reset counters.
        /// </summary>
        /// <param name="notifyParentRecordSource"></param>
        public virtual void InvalidateCounterBottomUp(bool notifyParentRecordSource)
        {
        }

        /// <summary>
        /// Walks up parent branches and reset summaries.
        /// </summary>
        /// <param name="notifyParentRecordSource"></param>
        public virtual void InvalidateSummariesBottomUp(bool notifyParentRecordSource)
        {
        }
    }

    /// <summary>
    /// A branch in a tree.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class TreeTableBranch : TreeTableNode, ITreeTableBranch
    {
        ITreeTableNode _left;
        ITreeTableNode _right;
        internal int _entryCount = -1;
        object _minimum = emptyMin;

        /// <summary>
        /// Initializes a new branch.
        /// </summary>
        /// <param name="tree"></param>
        public TreeTableBranch(TreeTable tree)
        {
            _tree = tree;
        }

        /// <summary>
        /// Returns the minimum value (of the most-left leaf) of the branch in a sorted tree.
        /// </summary>
        /// <returns></returns>
        public override object GetMinimum()
        {
            if (Object.ReferenceEquals(emptyMin, _minimum))
                _minimum = _left.GetMinimum();

            return _minimum;
        }

        /// <summary>
        /// Gets / sets Red-Black tree algorithm helper.
        /// </summary>
        public TreeTableNodeColor Color { get; set; }

        /// <summary>
        /// Returns the left node cast to ITreeTableBranch.
        /// </summary>
        /// <returns></returns>
        public ITreeTableBranch GetLeftB() { return (ITreeTableBranch)Left; }

        /// <summary>
        /// Returns the right node cast to ITreeTableBranch.
        /// </summary>
        /// <returns></returns>
        public ITreeTableBranch GetRightB() { return (ITreeTableBranch)Right; }

        /// <summary>
        /// Gets / sets the left leaf or branch.
        /// </summary>
        public ITreeTableNode Left
        {
            get { return _left; }
            set
            {
                SetLeft(value, false, _tree.Sorted);
            }
        }

        /// <summary>
        /// Sets the left node.
        /// </summary>
        /// <param name="value">The new node.</param>
        /// <param name="inAddMode">Indicates whether tree-table is in add-mode.</param>
        /// <param name="isSorted">Indicates whether tree-table is sorted.</param>
        /// <remarks>
        /// Call this method instead of simply setting <see cref="Left"/> property if you want
        /// to avoid the round-trip call to check whether the tree is in add-mode
        /// or if tree-table is sorted.
        /// </remarks>
        public virtual void SetLeft(ITreeTableNode value, bool inAddMode, bool isSorted)
        {
            if (!Object.ReferenceEquals(_left, value))
            {
                if (inAddMode)
                {
                    if (_left != null && _left.Parent == this) _left.Parent = null;
                    _left = value;
                    if (_left != null) _left.Parent = this;
                }
                else
                {
                    int lc = (_left != null) ? _left.GetCount() : 0;
                    int vc = (value != null) ? value.GetCount() : 0;
                    int entryCountDelta = vc - lc;
                    if (_left != null && _left.Parent == this) _left.Parent = null;
                    _left = value;
                    if (_left != null) _left.Parent = this;
                    if (entryCountDelta != 0)
                        InvalidateCountBottomUp();
                    if (isSorted)
                        InvalidateMinimumBottomUp();
                    InvalidateCounterBottomUp(false);
                    InvalidateSummariesBottomUp(false);
                }
            }
        }

        /// <summary>
        /// Gets / sets the right tree or branch.
        /// </summary>
        public ITreeTableNode Right
        {
            get { return _right; }
            set
            {
                SetRight(value, false);
            }
        }
        /// <summary>
        /// Sets the right node.
        /// </summary>
        /// <param name="value">The new node.</param>
        /// <param name="inAddMode">Indicates whether tree-table is in add-mode.</param>
        /// <remarks>
        /// Call this method instead of simply setting <see cref="Right"/> property if you want
        /// to avoid the round-trip call to check whether the tree is in add-mode
        /// or if tree-table is sorted.
        /// </remarks>
        public virtual void SetRight(ITreeTableNode value, bool inAddMode)
        {
            if (!Object.ReferenceEquals(_right, value))
            {
                if (inAddMode)
                {
                    if (_right != null && _right.Parent == this) _right.Parent = null;
                    _right = value;
                    if (_right != null) _right.Parent = this;
                }
                else
                {
                    int lc = (_right != null) ? _right.GetCount() : 0;
                    int vc = (value != null) ? value.GetCount() : 0;
                    int entryCountDelta = vc - lc;
                    if (_right != null && _right.Parent == this) _right.Parent = null;
                    _right = value;
                    if (_right != null) _right.Parent = this;
                    if (entryCountDelta != 0)
                        InvalidateCountBottomUp();
                    InvalidateCounterBottomUp(false);
                    InvalidateSummariesBottomUp(false);
                }
            }
        }

        /// <summary>
        /// Returns the position in the tree table of the specific child node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual int GetEntryPositionOfChild(ITreeTableNode node)
        {
            int pos = GetPosition();
            if (Object.ReferenceEquals(node, Right))
                pos += Left.GetCount();
            else if (!Object.ReferenceEquals(node, Left))
                throw new ArgumentException("must be a child node", "node");
            return pos;
        }

        /// <summary>
        /// Returns the number of child nodes (+1 for the current node).
        /// </summary>
        /// <returns></returns>
        public override int GetCount()
        {
#if TREE
			if (_tree.IsInitializing)
				return 1;
			else
#endif
            if (_entryCount < 0)
                _entryCount = _left.GetCount() + _right.GetCount();
            return _entryCount;
        }

        /// <summary>
        /// Indicates whether this is a leaf.
        /// </summary>
        /// <returns></returns>
        public override bool IsEntry()
        {
            return false;
        }

        /// <summary>
        /// Sets this object's child node count dirty and
        /// walks up parent nodes and marks their child node count dirty.
        /// </summary>
        public virtual void InvalidateCountBottomUp()
        {
#if TREE
			if (_tree.IsInitializing)
				return;
#endif

            _entryCount = -1;
            if (Parent != null && Parent.Parent == Parent)
                throw new InvalidOperationException();
            if (Parent != null)
                Parent.InvalidateCountBottomUp();
        }

        /// <summary>
        /// Sets this object's child node count dirty and steps
        /// through all child branches and marks their child node count dirty.
        /// </summary>
        public virtual void InvalidateCountTopDown()
        {
#if TREE
			if (_tree.IsInitializing)
				return;
#endif

            _entryCount = -1;
            if (!Left.IsEntry())
                GetLeftB().InvalidateCountTopDown();
            if (!Right.IsEntry())
                GetRightB().InvalidateCountTopDown();
        }

        /// <summary>
        /// Sets this object's child node minimum dirty and
        /// marks parent nodes' child node minimum dirty.
        /// </summary>
        public virtual void InvalidateMinimumBottomUp()
        {
#if TREE
			if (_tree.IsInitializing)
				return;
#endif

            this._minimum = emptyMin;
            if (Parent != null)
                Parent.InvalidateMinimumBottomUp();
        }

        /// <summary>
        /// Sets this object's child node minimum dirty and steps
        /// through all child branches and marks their child node minimum dirty.
        /// </summary>
        public virtual void InvalidateMinimumTopDown()
        {
#if TREE
			if (_tree.IsInitializing)
				return;
#endif
            if (!Left.IsEntry())
                GetLeftB().InvalidateMinimumTopDown();
            if (!Right.IsEntry())
                GetRightB().InvalidateMinimumTopDown();
            this._minimum = emptyMin;
        }


    }

    /// <summary>
    /// A leaf in the tree with value and optional sort key.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class TreeTableEntry : TreeTableNode, ITreeTableEntry
    {
        object _value;

        /// <summary>
        /// Releases the unmanaged resources used by the Component and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        /// <remarks>See the documentation for the <see cref="System.ComponentModel.Component"/> class and its Dispose member.</remarks>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_value is IDisposable)
                    ((IDisposable)_value).Dispose();
                _value = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets / sets the value attached to this leaf.
        /// </summary>
        public virtual object Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        /// <summary>
        /// Returns the sort key of this leaf.
        /// </summary>
        public virtual object GetSortKey()
        {
            return Value;
        }

        /// <summary>
        /// Returns the minimum value (of the most-left leaf) of the branch in a sorted tree.
        /// </summary>
        /// <returns></returns>
        public override object GetMinimum()
        {
            return GetSortKey();
        }

        /// <summary>
        /// Returns the number of child nodes (+1 for the current node).
        /// </summary>
        /// <returns></returns>
        public override int GetCount()
        {
            return 1;
        }

        /// <summary>
        /// Creates a branch that can hold this entry when new leaves are inserted into the tree.
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public virtual ITreeTableBranch CreateBranch(TreeTable tree)
        {
            return new TreeTableBranch(tree);
        }

        /// <summary>
        /// Returns the Debug / text information about the node.
        /// </summary>
        /// <returns></returns>
        public override string GetNodeInfo()
        {
            return base.GetNodeInfo() + ", " + (Value != null ? Value.ToString() : "null");
        }

        /// <summary>
        /// Indicates whether this is a leaf.
        /// </summary>
        /// <returns></returns>
        public override bool IsEntry()
        {
            return true;
        }
    }

    /// <summary>
    /// An empty node.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    internal class TreeTableEmpty : TreeTableNode
    {
        public static readonly TreeTableEmpty Empty = new TreeTableEmpty();

        public override int GetCount()
        {
            return 0;
        }

        public override bool IsEntry()
        {

            return true;
        }

        public override string GetNodeInfo()
        {
            return "Empty";
        }
    }

    /// <summary>
    /// Tree table interface definition.
    /// </summary>
    public interface ITreeTable : IList, IDisposable
    {
        /// <summary>
        /// Indicates whether this is a sorted tree.
        /// </summary>
        bool Sorted { get; }

        /// <summary>
        /// A comparer used by sorted trees.
        /// </summary>
        IComparer Comparer { get; set; }

        /// <summary>
        /// Returns the root node.
        /// </summary>
        ITreeTableNode Root { get; }

        /// <summary>
        /// Indicates whether BeginInit was called.
        /// </summary>
        bool IsInitializing { get; }

        /// <summary>
        /// Optimizes insertion of many elements when tree is initialized for the first time.
        /// </summary>
        void BeginInit();

        /// <summary>
        /// Ends optimization of insertion of elements when tree is initialized for the first time.
        /// </summary>
        void EndInit();

        /// <summary>
        /// Optimized access to a subsequent entry.
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        ITreeTableEntry GetNextEntry(ITreeTableEntry current);

        /// <summary>
        /// Optimized access to a previous entry.
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        ITreeTableEntry GetPreviousEntry(ITreeTableEntry current);
    }

    /// <summary>
    /// This object owns a <see cref="ITreeTable"/>.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public interface ITreeTableSource
    {
        /// <summary>
        /// Returns a reference to an inner tree table.
        /// </summary>
        /// <returns></returns>
        ITreeTable GetTreeTable();
    }

    /// <summary>
    /// A tree table.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class TreeTable : Disposable, ITreeTable
    {
        ITreeTableNode _root = null;
        object _tag = null;
        IComparer comparer = null;

        bool _sorted;
        bool inAddMode;


        //		int lastInsert = -1;
        ITreeTableBranch lastAddBranch = null;
        //		ITreeTableEntry lastInsertEntry = null;

#if DEBUG
        ITreeTableEntry _lastIndexLeaf = null;
        /// <summary>
        /// Gets or sets the last index leaf.
        /// </summary>
        /// <value>The last index leaf.</value>
        public ITreeTableEntry lastIndexLeaf
        {
            get
            {
                return _lastIndexLeaf;
            }
            set
            {
                if (_lastIndexLeaf != value)
                {
                    if (_lastIndexLeaf is IDisposedEvent)
                        ((IDisposedEvent)_lastIndexLeaf).Disposed -= new EventHandler(lastIndexLeaf_Disposed);
                    _lastIndexLeaf = value;
                    if (_lastIndexLeaf is IDisposedEvent)
                        ((IDisposedEvent)_lastIndexLeaf).Disposed += new EventHandler(lastIndexLeaf_Disposed);
                }
            }
        }

        private void lastIndexLeaf_Disposed(object sender, EventArgs e)
        {
            lastIndexLeaf = null;
            lastIndex = -1;
        }
#else
		ITreeTableEntry lastIndexLeaf = null;
#endif
        int lastIndex = -1;
        //int lastCount = -1;

        /// <summary>
        /// Releases the unmanaged resources used by the Component and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        /// <remarks>See the documentation for the <see cref="System.ComponentModel.Component"/> class and its Dispose member.</remarks>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (ITreeTableEntry entry in this)
                    entry.Dispose();
                _root = null;
                _sorted = false;
                _tag = null;
                inAddMode = false;
                lastAddBranch = null;
                //lastInsertEntry = null;
                //lastInsert = -1;
                comparer = null;
                lastIndexLeaf = null;
                lastIndex = -1;
                //lastCount = -1;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets / sets the comparer used by sorted trees.
        /// </summary>
        public IComparer Comparer
        {
            get
            {
                return comparer;
            }
            set
            {
                comparer = value;
                _sorted = comparer != null;
            }
        }

        /// <summary>
        /// Gets / sets the tag that can be associated with this object.
        /// </summary>
        public object Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
            }
        }


        /// <summary>
        /// Initializes a new <see cref="TreeTable"/>.
        /// </summary>
        /// <param name="sorted"></param>
        public TreeTable(bool sorted)
        {
            _sorted = sorted;
            this.inAddMode = false;
        }

        /// <summary>
        /// Indicates whether tree is sorted.
        /// </summary>
        public bool Sorted
        {
            get
            {
                return _sorted;
            }
        }

        /// <summary>
        /// Gets / sets the root node.
        /// </summary>
        public ITreeTableNode Root
        {
            get
            {
                return _root;
            }
            set
            {
                _root = value;
            }
        }

        private void LeftRotate(ITreeTableBranch x, bool inAddMode)
        {
            var y = x.Right as ITreeTableBranch;
            if (y == null)
                return;

            ITreeTableBranch xParent = x.Parent;

            // Establish x.Right link
            ITreeTableNode yLeft = y.Left;
            y.SetLeft(TreeTableEmpty.Empty, inAddMode, this.Sorted);
            x.SetRight(yLeft, inAddMode);

            // Estachlish y.Parent link
            if (x.Parent != null)
            {
                if (Object.ReferenceEquals(x, x.Parent.Left))
                    x.Parent.SetLeft(y, inAddMode, this.Sorted);
                else
                    x.Parent.SetRight(y, inAddMode);
            }
            else
                _root = y;

            // link x and y
            y.SetLeft(x, inAddMode, this.Sorted);
        }

        private void RightRotate(ITreeTableBranch x, bool inAddMode)
        {
            var y = x.Left as ITreeTableBranch;
            if (y == null)
                return;

            // establish x.Left link
            ITreeTableNode yRight = y.Right;
            y.SetRight(TreeTableEmpty.Empty, inAddMode); // make sure Parent is not reset later
            x.SetLeft(yRight, inAddMode, this.Sorted);

            // establish y.Parent link
            if (x.Parent != null)
            {
                if (x == x.Parent.Right)
                    x.Parent.SetRight(y, inAddMode);
                else
                    x.Parent.SetLeft(y, inAddMode, this.Sorted);
            }
            else
                _root = y;

            // link x and y
            y.SetRight(x, inAddMode);
        }

        private void InsertFixup(ITreeTableBranch x, bool inAddMode)
        {
            // Check Red-Black properties
            while (!Object.ReferenceEquals(x, _root) && x.Parent.Color == TreeTableNodeColor.Red
                && x.Parent.Parent != null)
            {
                // We have a violation
                if (x.Parent == x.Parent.Parent.Left)
                {
                    var y = x.Parent.Parent.Right as ITreeTableBranch;
                    if (y != null && y.Color == TreeTableNodeColor.Red)
                    {
                        // uncle is red
                        x.Parent.Color = TreeTableNodeColor.Black;
                        y.Color = TreeTableNodeColor.Black;
                        x.Parent.Parent.Color = TreeTableNodeColor.Red;
                        x = x.Parent.Parent;
                    }
                    else
                    {
                        // uncle is black
                        if (x == x.Parent.Right)
                        {
                            // Make x a left child
                            x = x.Parent;
                            LeftRotate(x, inAddMode);
                        }

                        // Recolor and rotate
                        x.Parent.Color = TreeTableNodeColor.Black;
                        x.Parent.Parent.Color = TreeTableNodeColor.Red;
                        RightRotate(x.Parent.Parent, inAddMode);
                    }
                }
                else
                {
                    // Mirror image of above code
                    var y = x.Parent.Parent.Left as ITreeTableBranch;
                    if (y != null && y.Color == TreeTableNodeColor.Red)
                    {
                        // uncle is red
                        x.Parent.Color = TreeTableNodeColor.Black;
                        y.Color = TreeTableNodeColor.Black;
                        x.Parent.Parent.Color = TreeTableNodeColor.Red;
                        x = x.Parent.Parent;
                    }
                    else
                    {
                        // uncle is black
                        if (x == x.Parent.Left)
                        {
                            x = x.Parent;
                            RightRotate(x, inAddMode);
                        }
                        x.Parent.Color = TreeTableNodeColor.Black;
                        x.Parent.Parent.Color = TreeTableNodeColor.Red;
                        LeftRotate(x.Parent.Parent, inAddMode);
                    }
                }
            }
            ((ITreeTableBranch)Root).Color = TreeTableNodeColor.Black;
        }

        private void DeleteFixup(ITreeTableBranch x, bool isLeft)
        {
            bool inAddMode = false;
            while (!Object.ReferenceEquals(x, _root) && x.Color == TreeTableNodeColor.Black)
            {
                if (isLeft)
                {
                    var w = x.Parent.Right as ITreeTableBranch;
                    if (w != null && w.Color == TreeTableNodeColor.Red)
                    {
                        w.Color = TreeTableNodeColor.Black;
                        x.Parent.Color = TreeTableNodeColor.Black;
                        LeftRotate(x.Parent, inAddMode);
                        w = x.Parent.Right as ITreeTableBranch;
                    }

                    if (w == null)
                        return;

                    if (w.Color == TreeTableNodeColor.Black && (w.Left.IsEntry() || w.GetLeftB().Color == TreeTableNodeColor.Black) && (w.Right.IsEntry() || w.GetRightB().Color == TreeTableNodeColor.Black))
                    {
                        w.Color = TreeTableNodeColor.Red;
                        if (x.Color == TreeTableNodeColor.Red)
                        {
                            x.Color = TreeTableNodeColor.Black;
                            return;
                        }
                        else
                        {
                            isLeft = x.Parent.Left == x;
                            x = x.Parent;
                        }
                    }
                    else if (w.Color == TreeTableNodeColor.Black && !w.Right.IsEntry() && w.GetRightB().Color == TreeTableNodeColor.Red)
                    {
                        LeftRotate(x.Parent, inAddMode);
                        TreeTableNodeColor t = w.Color;
                        w.Color = x.Parent.Color;
                        x.Parent.Color = t;
                        return;
                    }
                    else if (w.Color == TreeTableNodeColor.Black && !w.Left.IsEntry() && w.GetLeftB().Color == TreeTableNodeColor.Red && (w.Right.IsEntry() || w.GetRightB().Color == TreeTableNodeColor.Black))
                    {
                        RightRotate(w, inAddMode);

                        w.Parent.Color = TreeTableNodeColor.Black;
                        w.Color = TreeTableNodeColor.Red;

                        LeftRotate(x.Parent, inAddMode);
                        TreeTableNodeColor t = w.Color;
                        w.Color = x.Parent.Color;
                        x.Parent.Color = t;
                        return;
                    }
                    else
                        return;
                }
                else
                {
                    var w = x.Parent.Left as ITreeTableBranch;
                    if (w != null && w.Color == TreeTableNodeColor.Red)
                    {
                        w.Color = TreeTableNodeColor.Black;
                        x.Parent.Color = TreeTableNodeColor.Red;
                        RightRotate(x.Parent, inAddMode);
                        w = x.Parent.Left as ITreeTableBranch;
                    }

                    if (w == null)
                        return;

                    if (w.Color == TreeTableNodeColor.Black && (w.Left.IsEntry() || w.GetLeftB().Color == TreeTableNodeColor.Black) && (w.Right.IsEntry() || w.GetRightB().Color == TreeTableNodeColor.Black))
                    {
                        w.Color = TreeTableNodeColor.Red;
                        if (x.Color == TreeTableNodeColor.Red)
                        {
                            x.Color = TreeTableNodeColor.Black;
                            return;
                        }
                        else if (x.Parent != null)
                        {
                            isLeft = x.Parent.Left == x;
                            x = x.Parent;
                        }
                    }
                    else
                    {
                        if (w.Color == TreeTableNodeColor.Black && !w.Right.IsEntry() && w.GetRightB().Color == TreeTableNodeColor.Red)
                        {
                            ITreeTableBranch xParent = x.Parent;
                            LeftRotate(xParent, inAddMode);
                            TreeTableNodeColor t = w.Color;
                            w.Color = xParent.Color;
                            xParent.Color = t;
                            return;
                        }
                        else if (w.Color == TreeTableNodeColor.Black && !w.Left.IsEntry() && w.GetLeftB().Color == TreeTableNodeColor.Red && (w.Right.IsEntry() || w.GetRightB().Color == TreeTableNodeColor.Black))
                        {
                            ITreeTableBranch wParent = w.Parent;
                            ITreeTableBranch xParent = x.Parent;
                            RightRotate(w, inAddMode);

                            wParent.Color = TreeTableNodeColor.Black;
                            w.Color = TreeTableNodeColor.Red;

                            LeftRotate(x.Parent, inAddMode);
                            TreeTableNodeColor t = w.Color;
                            w.Color = xParent.Color;
                            xParent.Color = t;
                            return;
                        }
                    }
                    //else
                    //	return;
                }
            }
            x.Color = TreeTableNodeColor.Black;
        }

        /// <summary>
        /// Gets / sets an item at the specified index.
        /// </summary>
        public ITreeTableNode this[int index]
        {
            get
            {
                return GetEntryAt(index);
            }
            set
            {
                SetNodeAt(index, value);
            }
        }

        /// <summary>
        /// Sets the node at the specified index.
        /// </summary>
        /// <param name="index">Index value where the node is to be inserted.</param>
        /// <param name="value">Value of the node that is to be inserted.</param>
        void SetNodeAt(int index, ITreeTableNode value)
        {
            ITreeTableEntry leaf = GetEntryAt(index);
            if (Object.ReferenceEquals(leaf, _root))
                _root = value;
            else
            {
                ITreeTableBranch branch = leaf.Parent;
                ReplaceNode(branch, leaf, value, false);
            }
            lastIndex = -1;
        }

        ITreeTableEntry GetEntryAt(int index)
        {
            int treeCount = GetCount();
            if (index < 0 || index >= treeCount)
                throw new ArgumentOutOfRangeException("index", index.ToString() + " must be between 0 and " + (treeCount - 1).ToString());

            if (_root == null)
            {
                // replace root
                return (ITreeTableEntry)_root;
            }
            else
            {
                if (lastIndex != -1)
                {
                    if (index == lastIndex)
                    {
                        return lastIndexLeaf;
                    }
                    else if (index == lastIndex + 1)
                    {
                        lastIndex++;
                        lastIndexLeaf = GetNextEntry(lastIndexLeaf);
                        // Debug.Assert(lastElement == ElementHelper.FindElement(_table, GroupByVisibleCounter.CreateDisplayElementCounter(index)));
                        return lastIndexLeaf;
                    }
                }

                // find node
                ITreeTableBranch branch = null;
                ITreeTableNode current = _root;
                int count = 0;
                while (!current.IsEntry())
                {
                    branch = (ITreeTableBranch)current;
                    int leftCount = branch.Left.GetCount();

                    if (index < count + leftCount)
                        current = branch.Left;
                    else
                    {
                        count += branch.Left.GetCount();
                        current = branch.Right;
                    }
                }

                lastIndexLeaf = (ITreeTableEntry)current;
                //				lastCount = treeCount;
                lastIndex = index;
                return lastIndexLeaf;
            }
        }

        /// <summary>
        /// Optimized access to the previous entry.
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public ITreeTableEntry GetPreviousEntry(ITreeTableEntry current)
        {
            ITreeTableBranch parent = current.Parent;
            ITreeTableNode prev;

            if (parent == null)
            {
                prev = null;
                return null;
            }
            else
            {
                if (Object.ReferenceEquals(current, parent.Right))
                    prev = parent.Left;
                else
                {
                    ITreeTableBranch parentParent = parent.Parent;
                    if (parentParent == null)
                    {
                        return null;
                    }
                    else
                    {
                        while (Object.ReferenceEquals(parentParent.Left, parent))
                        {
                            parent = parentParent;
                            parentParent = parentParent.Parent;
                            if (parentParent == null)
                            {
                                return null;
                            }
                        }

                        prev = parentParent.Left;
                    }
                }
                while (!prev.IsEntry())
                    prev = ((ITreeTableBranch)prev).Right;
            }
            return prev as ITreeTableEntry;
        }


        /// <summary>
        /// Optimized access to a subsequent entry.
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public ITreeTableEntry GetNextEntry(ITreeTableEntry current)
        {
            ITreeTableBranch parent = current.Parent;
            ITreeTableNode next;

            if (parent == null)
            {
                next = null;
                return null;
            }
            else
            {
                if (Object.ReferenceEquals(current, parent.Left))
                    next = parent.Right;
                else
                {
                    ITreeTableBranch parentParent = parent.Parent;
                    if (parentParent == null)
                    {
                        return null;
                    }
                    else
                    {
                        while (Object.ReferenceEquals(parentParent.Right, parent))
                        {
                            parent = parentParent;
                            parentParent = parentParent.Parent;
                            if (parentParent == null)
                            {
                                return null;
                            }
                        }

                        next = parentParent.Right;
                    }
                }
                while (!next.IsEntry())
                    next = ((ITreeTableBranch)next).Left;
            }
            return next as ITreeTableEntry;
        }

        ITreeTableEntry GetMostLeftEntry(ITreeTableBranch parent)
        {
            ITreeTableNode next;

            if (parent == null)
            {
                next = null;
                return null;
            }
            else
            {
                next = parent.Left;
                while (!next.IsEntry())
                    next = ((ITreeTableBranch)next).Left;
            }
            return next as ITreeTableEntry;
        }

        /// <summary>
        /// Inserts a node at the specified index.
        /// </summary>
        /// <param name="index">Index value where the node is to be inserted.</param>
        /// <param name="value">Value of the node to insert.</param>
        public void Insert(int index, ITreeTableNode value)
        {
            if (Sorted)
                throw new InvalidOperationException("This tree is sorted - use AddSorted instead.");

            int treeCount = GetCount();
            if (index < 0 || index > treeCount)
                throw new ArgumentOutOfRangeException("index", index.ToString() + "must be between 0 and " + treeCount.ToString());

            if (index == treeCount)
            {
                Add(value);
                return;
            }

            this.CacheLastFoundEntry(null, null, false);

            //			_dirty++;

            if (_root == null)
            {
                // replace root
                _root = value;
            }
            else
            {
                ITreeTableEntry leaf = null;
                if (lastIndex != -1)
                {
                    if (index == lastIndex)
                    {
                        leaf = lastIndexLeaf;
                    }
                    else if (index == lastIndex + 1)
                    {
                        leaf = GetNextEntry(lastIndexLeaf);
                    }
                }
                //				if (lastInsert == index)
                //				{
                //					leaf = lastInsertEntry;
                //				}
                //				else if (lastInsertEntry != null && lastInsert+1 == index)
                //				{
                //					leaf = GetNextEntry(lastInsertEntry);
                //				}

                if (leaf == null)
                    leaf = GetEntryAt(index);

                ITreeTableBranch branch = leaf.Parent;
                ITreeTableBranch newBranch = leaf.CreateBranch(this);

                newBranch.SetLeft(value, false, this.Sorted); // will set leaf.Parent ...
                newBranch.Right = leaf;

                if (branch == null)
                    _root = newBranch;
                else
                {
                    // swap out leafs parent with new node
                    ReplaceNode(branch, leaf, newBranch, false);
                }

                //Debug.Assert(value.Position == index);

                InsertFixup(newBranch, inAddMode);

                if (value.IsEntry())
                {
                    lastIndexLeaf = (ITreeTableEntry)value;
                    lastIndex = index;
                }
                else
                {
                    lastIndexLeaf = null;
                    lastIndex = -1;
                }
                //				if (value.IsEntry())
                //				{
                //					lastInsertEntry = (ITreeTableEntry) value;
                //					lastInsert = index;
                //				}
                //				else
                //					lastInsertEntry = null;
            }
        }


        /// <summary>
        /// Removes the specified node.
        /// </summary>
        /// <param name="value">Node value to look for and remove.</param>
        public bool Remove(ITreeTableNode value)
        {
            return _Remove(value, true);
        }

        /// <summary>
        /// Resets the cache.
        /// </summary>
        public void ResetCache()
        {
            lastAddBranch = null;
            lastIndex = -1;
            lastIndexLeaf = null;
        }

        internal bool _Remove(ITreeTableNode value, bool resetParent)
        {
            if (value == null)
                return false;

            if (!Contains(value))
                return false;

            this.CacheLastFoundEntry(null, null, false);

            lastAddBranch = null;
            //			lastInsertEntry = null;
            //			lastInsert = -1;
            lastIndex = -1;
            lastIndexLeaf = null;

            // root
            if (Object.ReferenceEquals(value, _root))
            {
                _root = null;
                if (resetParent)
                    value.Parent = null;
            }
            else
            {
                ITreeTableBranch leafsParent = value.Parent;

                // get the sister node
                ITreeTableNode sisterNode = GetSisterNode(leafsParent, value);

                // swap out leaves parent with sister
                if (Object.ReferenceEquals(leafsParent, _root))
                {
                    _root = sisterNode;
                    _root.Parent = null;
                }
                else
                {
                    ITreeTableBranch leafsParentParent = leafsParent.Parent;
                    bool isLeft = leafsParentParent.Left == leafsParent;
                    ReplaceNode(leafsParentParent, leafsParent, sisterNode, false);

                    if (leafsParent.Color == TreeTableNodeColor.Black)
                    {
                        leafsParent.Parent = leafsParentParent;
                        DeleteFixup(leafsParent, isLeft);
                    }
                }
                if (resetParent)
                    value.Parent = null;
            }

            return true;
        }

        ITreeTableNode GetSisterNode(ITreeTableBranch leafsParent, ITreeTableNode node)
        {
            ITreeTableNode sisterNode;
            sisterNode = Object.ReferenceEquals(leafsParent.Left, node) ? leafsParent.Right : leafsParent.Left;
            return sisterNode;
        }

        void ReplaceNode(ITreeTableBranch branch, ITreeTableNode oldNode, ITreeTableNode newNode, bool inAddMode)
        {
            // also updates node count.
            if (Object.ReferenceEquals(branch.Left, oldNode))
                branch.SetLeft(newNode, inAddMode, this.Sorted);
            else
                branch.SetRight(newNode, inAddMode);
        }

        /// <summary>
        /// Indicates whether the node belongs to this tree.
        /// </summary>
        /// <param name="value">Node value to search for.</param>
        /// <returns>True if node belongs to this tree; false otherwise.</returns>
        public bool Contains(ITreeTableNode value)
        {
            if (value == null || _root == null)
                return false;

            // search root
            while (value.Parent != null)
                value = value.Parent;

            return Object.ReferenceEquals(value, _root);
        }

        /// <summary>
        /// Returns the position of a node.
        /// </summary>
        /// <param name="value">Node value to look for.</param>
        /// <returns>Index of the node if found.</returns>
        public int IndexOf(ITreeTableNode value)
        {
            if (!Contains(value))
                return -1;
            return value.GetPosition();
        }

        /// <summary>
        /// Appends a node.
        /// </summary>
        /// <param name="value">Node value to append.</param>
        /// <returns></returns>
        public int Add(ITreeTableNode value)
        {
            this.CacheLastFoundEntry(null, null, false);
            lastIndex = -1;

            if (Sorted && !this.inAddMode)
            {
                return AddSorted(value);
                // throw new InvalidOperationException("This tree is sorted - Use AddSorted instead or call this fn only after a BeginInit()");
            }

            // empty?
            if (_root == null)
            {
                // replace root
                _root = value;
                return 0;
            }
            else
            {
                // add node to most right branch
                ITreeTableBranch branch = null;
                ITreeTableNode current = lastAddBranch ?? _root;
                int leafCount = value.GetCount();
                while (!current.IsEntry())
                {
                    branch = (ITreeTableBranch)current;
                    //branch.OnAddNodes(leafCount);
                    current = branch.Right;
                }

                var leaf = (ITreeTableEntry)current;
                //if (branch != null)
                //	branch.Right = TreeTableEmpty.Empty;
                ITreeTableBranch newBranch = leaf.CreateBranch(this);

                newBranch.SetLeft(leaf, inAddMode, this.Sorted); // will set leaf.Parent ...
                newBranch.SetRight(value, inAddMode);

                if (branch == null)
                    _root = newBranch;
                else
                {
                    // swap out leafs parent with new node
                    ReplaceNode(branch, current, newBranch, inAddMode);
#if DEBUG
                    Debug.Assert(branch.Parent == null || branch.Parent.Parent == null || branch.Parent.Parent != branch.Parent);
                    Debug.Assert(branch.Parent == null || branch.Parent.Parent == null || branch.Parent.Parent != this);
                    if (!(branch.Parent == null || branch.Parent.Parent == null || branch.Right != branch.Parent.Parent.Right))
                    {
                        throw new Exception();
                    }
                    if (!(branch.Parent == null || branch.Parent.Left.IsEntry() || ((TreeTableBranch)branch.Parent.Left).Right != branch))
                    {
                        throw new Exception();
                    }
                    Debug.Assert(branch.Parent == null || branch.Parent.Left.IsEntry() || ((TreeTableBranch)branch.Parent.Left).Right != branch);
                    Debug.Assert(branch.Parent == null || branch.Parent.Parent == null || branch.Right != branch.Parent.Parent.Right);
#endif
                }

                InsertFixup(newBranch, inAddMode);

#if DEBUG
                if (value.Parent != null && value.Parent.Parent != null)
                {
                    if (value.Parent.Parent.Right == value)
                    {
                        throw new Exception();
                        //Trace.WriteLine(next.ToString());
                    }
                }
#endif
                lastAddBranch = newBranch;

                if (inAddMode)
                    return -1;
                else
                    return _root.GetCount() - 1;
            }
        }

        /// <summary>
        /// Indicates whether BeginInit was called.
        /// </summary>
        public bool IsInitializing
        {
            get { return inAddMode; }
        }

        /// <summary>
        /// Optimizes insertion of many elements when tree is initialized for the first time.
        /// </summary>
        public virtual void BeginInit()
        {
            inAddMode = true;
        }

        /// <summary>
        /// Ends optimization of insertion of elements when tree is initialized for the first time.
        /// </summary>
        public virtual void EndInit()
        {
            inAddMode = false;

            // Fixes issues when GetCount() was called while debugging ...
            var branch = this._root as TreeTableBranch;
            if (branch != null && branch._entryCount != -1)
                branch._entryCount = -1;
        }

        /// <summary>
        /// Adds a node into a sorted tree.
        /// </summary>
        /// <param name="value">Node value to add.</param>
        /// <returns></returns>
        public int AddSorted(ITreeTableNode value)
        {
            if (!Sorted)
                throw new InvalidOperationException("This tree is not sorted.");

            if (this.inAddMode)
                return Add(value);

            this.CacheLastFoundEntry(null, null, false);

            if (_root == null)
            {
                // replace root
                _root = value;
                return 0;
            }
            else
            {
                bool inAddMode = false;
                IComparer comparer = Comparer;
                // find node
                ITreeTableBranch branch = null;
                ITreeTableNode current = _root;
                int count = 0;
                int cmp = 0;

                while (!current.IsEntry())
                {
                    branch = (ITreeTableBranch)current;
                    if (comparer != null)
                        cmp = comparer.Compare(value.GetMinimum(), branch.Right.GetMinimum());
                    else if (value.GetMinimum() is IComparable)
                        cmp = ((IComparable)value.GetMinimum()).CompareTo(branch.Right.GetMinimum());
                    else
                        throw new InvalidOperationException("No Comparer specified.");

                    if (cmp <= 0)
                        current = branch.Left;
                    else
                    {
                        count += branch.Left.GetCount();
                        current = branch.Right;
                    }
                }

                var leaf = (ITreeTableEntry)current;

                ITreeTableBranch newBranch = leaf.CreateBranch(this);

                if (comparer != null)
                    cmp = comparer.Compare(value.GetMinimum(), leaf.GetSortKey());
                else if (value.GetMinimum() is IComparable)
                    cmp = ((IComparable)value.GetMinimum()).CompareTo(leaf.GetSortKey());

                if (cmp <= 0)
                {
                    newBranch.SetLeft(value, false, this.Sorted); // will set leaf.Parent ...
                    newBranch.Right = leaf;
                }
                else
                {
                    newBranch.SetLeft(leaf, false, this.Sorted); // will set leaf.Parent ...
                    count++;
                    newBranch.Right = value;
                }

                if (branch == null)
                    _root = newBranch;
                else
                {
                    // swap out leafs parent with new node
                    ReplaceNode(branch, leaf, newBranch, inAddMode);
                }

                //Debug.Assert(value.Position == index);

                InsertFixup(newBranch, inAddMode);

                return count;
            }
        }

        /// <summary>
        /// Adds a node in a sorted tree only if no node with the same value has not been added yet.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value">Node value to add.</param>
        /// <returns></returns>
        public ITreeTableEntry AddIfNotExists(object key, ITreeTableEntry value)
        {
            if (!Sorted)
                throw new InvalidOperationException("This tree is not sorted.");

            this.CacheLastFoundEntry(null, null, false);

            if (_root == null)
            {
                // replace root
                _root = value;
                return value;
            }
            else
            {
                // find node
                ITreeTableBranch branch = null;
                ITreeTableNode current = _root;
                int count = 0;
                int cmp = 0;
                IComparer comparer = Comparer;
                bool inAddMode = false;

                while (!current.IsEntry())
                {
                    branch = (ITreeTableBranch)current;
                    if (comparer != null)
                        cmp = comparer.Compare(key, branch.Right.GetMinimum());
                    else if (key is IComparable)
                        cmp = ((IComparable)key).CompareTo(branch.Right.GetMinimum());
                    else
                        throw new InvalidOperationException("No Comparer specified.");

                    if (cmp == 0)
                    {
                        current = branch.Right;
                        while (!current.IsEntry())
                            current = ((ITreeTableBranch)current).Left;
                        return (ITreeTableEntry)current;
                    }
                    else if (cmp < 0)
                        current = branch.Left;
                    else
                    {
                        count += branch.Left.GetCount();
                        current = branch.Right;
                    }
                }

                var leaf = (ITreeTableEntry)current;

                if (comparer != null)
                    cmp = comparer.Compare(key, leaf.GetSortKey());
                else if (value.GetMinimum() is IComparable)
                    cmp = ((IComparable)key).CompareTo(leaf.GetSortKey());

                if (cmp == 0)
                {
                    return leaf;
                }

                ITreeTableBranch newBranch = leaf.CreateBranch(this);

                if (cmp < 0)
                {
                    newBranch.SetLeft(value, false, this.Sorted); // will set leaf.Parent ...
                    newBranch.Right = leaf;
                }
                else if (cmp > 0)
                {
                    newBranch.SetLeft(leaf, false, this.Sorted); // will set leaf.Parent ...
                    count++;
                    newBranch.Right = value;
                }

                if (branch == null)
                    _root = newBranch;
                else
                {
                    // swap out leafs parent with new node
                    ReplaceNode(branch, leaf, newBranch, false);
                }

                //Debug.Assert(value.Position == index);

                InsertFixup(newBranch, inAddMode);

                return value;
            }
        }

        /// <summary>
        /// Finds a node in a sorted tree.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int IndexOfKey(object key)
        {
            ITreeTableEntry entry = FindKey(key);
            if (entry == null)
                return -1;
            return entry.GetPosition();
        }

        /// <summary>
        /// Finds a node in a sorted tree that matches the specified key.
        /// </summary>
        /// <param name="key">The key to search.</param>
        /// <returns>The node; NULL if not found.</returns>
        public ITreeTableEntry FindKey(object key)
        {
            return _FindKey(key, false);
        }

        /// <summary>
        /// Finds the node in a sorted tree is just one entry ahead of the
        /// node with the specified key. It searches for the largest possible
        /// key that is smaller than the specified key.
        /// </summary>
        /// <param name="key">The key to search.</param>
        /// <returns>The node; NULL if not found.</returns>
        public ITreeTableEntry FindHighestSmallerOrEqualKey(object key)
        {
            return _FindKey(key, true);
        }

        ITreeTableEntry lastFoundEntry = null;
        object lastFoundEntryKey = null;
        bool lastFoundEntryHighestSmallerValue = false;

        ITreeTableEntry CacheLastFoundEntry(ITreeTableEntry entry, object key, bool highestSmallerValue)
        {
            lastIndex = -1;
            lastFoundEntry = entry;
            lastFoundEntryKey = key;
            lastFoundEntryHighestSmallerValue = highestSmallerValue;
            return lastFoundEntry;
        }

        ITreeTableEntry _FindKey(object key, bool highestSmallerValue)
        {
            if (!Sorted)
                throw new InvalidOperationException("This tree is not sorted.");

            if (_root == null)
            {
                // replace root
                return null;
            }
            else
            {
                IComparer comparer = Comparer;
                int cmp = 0;

                if (lastFoundEntry != null &&
                    lastFoundEntryKey != null && key != null &&
                    lastFoundEntryHighestSmallerValue == highestSmallerValue)
                {
                    if (comparer != null)
                        cmp = comparer.Compare(key, lastFoundEntry.GetMinimum());
                    else if (key is IComparable)
                        cmp = ((IComparable)key).CompareTo(lastFoundEntry.GetMinimum());
                    if (cmp == 0)
                    {
                        //Console.WriteLine("Found: " + lastFoundEntry.ToString());
                        return lastFoundEntry;
                    }
                    //Console.WriteLine("Not Found");
                }

                // find node
                ITreeTableBranch branch = null;
                ITreeTableNode current = _root;
                int count = 0;

                ITreeTableNode lastLeft = null;

                while (!current.IsEntry())
                {
                    branch = (ITreeTableBranch)current;
                    if (comparer != null)
                        cmp = comparer.Compare(key, branch.Right.GetMinimum());
                    else if (key is IComparable)
                        cmp = ((IComparable)key).CompareTo(branch.Right.GetMinimum());
                    else
                        throw new InvalidOperationException("No Comparer specified.");

                    if (cmp == 0)
                    {
                        current = branch.Right;
                        while (!current.IsEntry())
                            current = ((ITreeTableBranch)current).Left;
                        return CacheLastFoundEntry((ITreeTableEntry)current, key, highestSmallerValue);
                    }
                    else if (cmp < 0)
                    {
                        current = branch.Left;
                        lastLeft = branch.Left;
                    }
                    else
                    {
                        count += branch.Left.GetCount();
                        current = branch.Right;
                    }
                }

                var leaf = (ITreeTableEntry)current;

                if (comparer != null)
                    cmp = comparer.Compare(key, leaf.GetSortKey());
                else if (key is IComparable)
                    cmp = ((IComparable)key).CompareTo(leaf.GetSortKey());

                if (cmp == 0)
                {
                    return CacheLastFoundEntry(leaf, key, highestSmallerValue);
                }

                if (highestSmallerValue)
                {
                    if (cmp < 0)
                        return CacheLastFoundEntry(leaf, key, highestSmallerValue);
                    else if (lastLeft != null)
                    {
                        current = lastLeft;
                        while (!current.IsEntry())
                            current = ((ITreeTableBranch)current).Right;
                        return CacheLastFoundEntry((ITreeTableEntry)current, key, highestSmallerValue);
                    }
                }

                lastFoundEntry = null;
                return null;
            }
        }

        #region IList Members

        /// <summary>
        /// Indicates whether the tree is Read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets / sets the item with the specified index.
        /// </summary>
        /// <param name="index">Index value of the item.</param>
        /// <returns></returns>
        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = (ITreeTableNode)value;
            }
        }

        /// <summary>
        /// Removes a node at the specified position.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            Remove(this[index]);
        }

        /// <summary>
        /// Inserts a node at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        void IList.Insert(int index, object value)
        {
            Insert(index, (ITreeTableNode)value);
        }

        /// <summary>
        /// Removes the node with the specified value.
        /// </summary>
        /// <param name="value"></param>
        void IList.Remove(object value)
        {
            Remove((ITreeTableNode)value);
        }


        /// <summary>
        /// Indicates whether the node belongs to this tree.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool IList.Contains(object value)
        {
            return Contains((ITreeTableNode)value);
        }

        /// <summary>
        /// Clears all nodes in the tree.
        /// </summary>
        public void Clear()
        {
            _root = null;
            lastAddBranch = null;
            //lastInsertEntry = null;
            //lastInsert = -1;
            lastIndex = -1;
            lastIndexLeaf = null;

            this.CacheLastFoundEntry(null, null, false);
        }

        /// <summary>
        /// Returns the index of the specified node.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        int IList.IndexOf(object value)
        {
            return IndexOf((ITreeTableNode)value);
        }

        /// <summary>
        /// Adds the specified node to the tree.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        int IList.Add(object value)
        {
            return Add((ITreeTableNode)value);
        }

        /// <summary>
        /// Indicates whether the nodes can be added or removed.
        /// </summary>
        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region ICollection Members

        /// <summary>
        /// Not supported.
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the number of leaves.
        /// </summary>
        public int GetCount()
        {
            // I made this a function so that in debug Watch window the Count is not shown (and
            // tree gets unintentionally initialized)
            return _root == null ? 0 : _root.GetCount();
        }

        /// <summary>
        /// Returns the number of leaves.
        /// </summary>
        int ICollection.Count
        {
            get
            {
                return GetCount();
            }
        }

        /// <summary>
        /// Copies the element from this collection into an array.
        /// </summary>
        /// <param name="array">The destination array.</param>
        /// <param name="index">The starting index in thedestination array.</param>
        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((ITreeTableNode[])array, index);
        }

        /// <summary>
        /// Copies the elements from this collection into an array.
        /// </summary>
        /// <param name="array">The destination array.</param>
        /// <param name="index">The starting index in the destination array.</param>
        public void CopyTo(ITreeTableNode[] array, int index)
        {
            int count = GetCount();
            for (int i = 0; i < count; i++)
                array[i + index] = this[i];
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns a <see cref="TreeTableEnumerator"/>.
        /// </summary>
        /// <returns></returns>
        public TreeTableEnumerator GetEnumerator()
        {
            return new TreeTableEnumerator(this);
        }

        #endregion

    }

    /// <summary>
    /// Strongly typed enumerator for <see cref="TreeTable"/>.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class TreeTableEnumerator : IEnumerator, IDisposable
    {
        ITreeTableNode _cursor, _next;
        ITreeTable _tree;

        /// <summary>
        /// Initializes a new <see cref="TreeTableEnumerator"/>.
        /// </summary>
        /// <param name="tree"></param>
        public TreeTableEnumerator(ITreeTable tree)
        {
            _tree = tree;
            _cursor = null;
            if (tree.Count > 0)
                _next = (ITreeTableNode)tree[0];
        }

        #region IEnumerator Members

        /// <summary>
        /// Resets the enumerator.
        /// </summary>
        public virtual void Reset()
        {
            _cursor = null;
            if (_tree.Count > 0)
                _next = (ITreeTableNode)_tree[0];
            else
                _next = null;
        }

        /// <summary>
        /// Returns the current enumerator.
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        /// <summary>
        /// Returns the current node.
        /// </summary>
        public ITreeTableEntry Current
        {
            get
            {
                return (ITreeTableEntry)_cursor;
            }
        }

        /// <summary>
        /// Indicates whether to move to the next node.
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            if (_next == null)
                return false;

            _cursor = _next;

            ITreeTableBranch parent = _cursor.Parent;

            if (parent == null)
            {
                _next = null;
                return true;
            }
            else
            {
                if (Object.ReferenceEquals(_cursor, parent.Left))
                    _next = parent.Right;
                else
                {
                    ITreeTableBranch parentParent = parent.Parent;
                    if (parentParent == null)
                    {
                        _next = null;
                        return true;
                    }
                    else
                    {
                        while (Object.ReferenceEquals(parentParent.Right, parent))
                        {
                            parent = parentParent;
                            parentParent = parentParent.Parent;
                            if (parentParent == null)
                            {
                                _next = null;
                                return true;
                            }
                        }

                        _next = parentParent.Right;
                    }
                }
                while (!_next.IsEntry())
                    _next = ((ITreeTableBranch)_next).Left;
            }

            return _cursor != null;
        }
        #endregion



        #region IDisposable Members

        public void Dispose()
        {
            _cursor = null;
            _next = null;
            _tree = null;
        }

        #endregion
    }

    /// <summary>
    /// An object that holds an <see cref="ITreeTableEntry"/>.
    /// </summary>
    public interface ITreeTableEntrySource
    {
        /// <summary>
        /// Gets a reference to the <see cref="ITreeTableEntry"/>.
        /// </summary>
        ITreeTableEntry Entry { get; set; }
    }

    /// <summary>
    /// A collection of <see cref="ITreeTableEntrySource"/> objects
    /// that are internally using a <see cref="ITreeTable"/>.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class TreeTableEntrySourceCollection : IList
    {
        internal ITreeTable _inner;

        /// <summary>
        /// Overloaded. Initializes a new <see cref="TreeTableEntrySourceCollection"/>.
        /// </summary>
        public TreeTableEntrySourceCollection()
        {
            _inner = new TreeTable(false);
        }

        /// <summary>
        /// Initializes a new <see cref="TreeTableEntrySourceCollection"/>.
        /// </summary>
        public TreeTableEntrySourceCollection(bool sorted)
        {
            _inner = new TreeTable(sorted);
        }

        /// <summary>
        /// Initializes a new <see cref="TreeTableEntrySourceCollection"/>.
        /// </summary>
        public TreeTableEntrySourceCollection(ITreeTable inner)
        {
            _inner = inner;
        }

        /// <summary>
        /// Indicates whether BeginInit was called.
        /// </summary>
        public bool IsInitializing
        {
            get { return _inner.IsInitializing; }
        }

        /// <summary>
        /// Optimizes insertion of many elements when tree is initialized for the first time.
        /// </summary>
        public void BeginInit()
        {
            _inner.BeginInit();
        }

        /// <summary>
        /// Ends optimization of insertion of elements when tree is initialized for the first time.
        /// </summary>
        public void EndInit()
        {
            _inner.EndInit();
        }


        /// <summary>
        /// Gets / sets an <see cref="ITreeTableEntrySource"/> at a specific position.
        /// </summary>
        public ITreeTableEntrySource this[int index]
        {
            get
            {
                var entry = (ITreeTableEntry)_inner[index];
                if (entry == null)
                    return null;
                return entry.Value as ITreeTableEntrySource;
            }
            set
            {
                var entry = new TreeTableEntry {Value = value};
                value.Entry = entry;
                _inner[index] = entry;
            }
        }

        /// <summary>
        /// Indicates whether object belongs to this collection.
        /// </summary>
        /// <param name="value">The value of the object.</param>
        /// <returns>True if object belongs to the collection; false otherwise.</returns>
        public bool Contains(ITreeTableEntrySource value)
        {
            if (value == null)
                return false;

            return _inner.Contains(value.Entry);
        }

        /// <summary>
        /// Returns the position of a object in the collection.
        /// </summary>
        /// <param name="value">The value of the object.</param>
        /// <returns>The position of the object.</returns>
        public int IndexOf(ITreeTableEntrySource value)
        {
            return _inner.IndexOf(value.Entry);
        }

        /// <summary>
        /// Copies the contents of the collection to an array.
        /// </summary>
        /// <param name="array">Destination array.</param>
        /// <param name="index">Starting index of the destination array.</param>
        public void CopyTo(ITreeTableEntrySource[] array, int index)
        {
            int count = _inner.Count;
            for (int n = 0; n < count; n++)
                array[index + n] = this[n];
        }

        //		public TreeTableEntrySourceCollection SyncRoot
        //		{
        //			get
        //			{
        //				return null;
        //			}
        //		}

        /// <summary>
        /// Returns a strongly typed enumerator.
        /// </summary>
        /// <returns>A strongly types enumerator.</returns>
        public TreeTableEntrySourceCollectionEnumerator GetEnumerator()
        {
            return new TreeTableEntrySourceCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts an object at the specified index.
        /// </summary>
        /// <param name="index">Index value where the object is to be inserted.</param>
        /// <param name="value">Value of the object to insert.</param>
        public void Insert(int index, ITreeTableEntrySource value)
        {
            var entry = new TreeTableEntry {Value = value};
            value.Entry = entry;
            _inner.Insert(index, entry);
        }

        /// <summary>
        /// Appends an object.
        /// </summary>
        /// <param name="value">The value of the object to append.</param>
        /// <returns></returns>
        public int Add(ITreeTableEntrySource value)
        {
            var entry = new TreeTableEntry {Value = value};
            value.Entry = entry;
            return _inner.Add(entry);
        }

        /// <summary>
        /// Removes the object.
        /// </summary>
        /// <param name="value">The value of the object to remove.</param>
        public void Remove(ITreeTableEntrySource value)
        {
            _inner.Remove(value.Entry);
        }

        #region IList Members

        /// <summary>
        /// Indicates whether tree is Read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets / sets the item at the specified index.
        /// </summary>
        /// <param name="index">Index of the item.</param>
        /// <returns>The item at the specified index.</returns>
        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }
        }

        /// <summary>
        /// Removes a node at the specified index.
        /// </summary>
        /// <param name="index">Index value of the node to remove.</param>
        public void RemoveAt(int index)
        {
            _inner.RemoveAt(index);
        }

        /// <summary>
        /// Inserts the object at the specified index.
        /// </summary>
        /// <param name="index">Index value of the object to insert.</param>
        /// <param name="value">Value of the object to insert.</param>
        void IList.Insert(int index, object value)
        {
            Insert(index, (ITreeTableEntrySource)value);
        }

        /// <summary>
        /// Removes the specified object.
        /// </summary>
        /// <param name="value">Value of the object to remove.</param>
        void IList.Remove(object value)
        {
            Remove((ITreeTableEntrySource)value);
        }

        /// <summary>
        /// Indicate whether the specified object belongs to this collection.
        /// </summary>
        /// <param name="value">Object value to look for.</param>
        /// <returns>True if object belongs to the collection; false otherwise.</returns>
        bool IList.Contains(object value)
        {
            return Contains((ITreeTableEntrySource)value);
        }

        /// <summary>
        /// Clears all nodes in the tree.
        /// </summary>
        public void Clear()
        {
            _inner.Clear();
        }

        /// <summary>
        /// Returns the index of the specified object.
        /// </summary>
        /// <param name="value">Value of the object.</param>
        /// <returns>Index value of the object.</returns>
        int IList.IndexOf(object value)
        {
            return IndexOf((ITreeTableEntrySource)value);
        }

        /// <summary>
        /// Adds the specified object to the collection.
        /// </summary>
        /// <param name="value">Value of the object to add.</param>
        /// <returns></returns>
        int IList.Add(object value)
        {
            return Add((ITreeTableEntrySource)value);
        }

        /// <summary>
        /// Indicates whether the nodes can be added or removed.
        /// </summary>
        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region ICollection Members

        /// <summary>
        /// Not supported.
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the number of objects in this collection.
        /// </summary>
        public int Count
        {
            get
            {
                return _inner.Count;
            }
        }

        /// <summary>
        /// Copies elements to destination array.
        /// </summary>
        /// <param name="array">Destination array.</param>
        /// <param name="index">Starting index of the destination array.</param>
        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((ITreeTableEntrySource[])array, index);
        }

        object ICollection.SyncRoot
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns>Enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

    /// <summary>
    /// A strongly typed enumerator for the <see cref="TreeTableEntrySourceCollection"/>.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class TreeTableEntrySourceCollectionEnumerator : IEnumerator
    {
        TreeTableEnumerator _inner;

        /// <summary>
        /// Initializes the <see cref="TreeTableEntrySourceCollectionEnumerator"/>.
        /// </summary>
        /// <param name="collection"></param>
        public TreeTableEntrySourceCollectionEnumerator(TreeTableEntrySourceCollection collection)
        {
            _inner = new TreeTableEnumerator(collection._inner);
        }

        /// <summary>
        /// Resets the enumerator.
        /// </summary>
        public virtual void Reset()
        {
            _inner.Reset();
        }

        /// <summary>
        /// Returns the current enumerator.
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        /// <summary>
        /// Returns the current <see cref="ITreeTableEntrySource"/> object.
        /// </summary>
        public ITreeTableEntrySource Current
        {
            get
            {
                return (ITreeTableEntrySource)_inner.Current.Value;
            }
        }

        /// <summary>
        /// Indicates whether to move to the next object in the collection.
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            return _inner.MoveNext();
        }
    }
}
