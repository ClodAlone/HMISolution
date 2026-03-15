#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Diagnostics;

namespace Syncfusion.UI.Xaml.Collections
{
    /// <summary>
    /// Interface definition for a node that has counters and summaries.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public interface ITreeTableCounterNode : ITreeTableSummaryNode
    {
        /// <summary>
        /// The total of this node's counter and child nodes.
        /// </summary>
        ITreeTableCounter GetCounterTotal();

        /// <summary>
        /// The cumulative position of this node.
        /// </summary>
        ITreeTableCounter GetCounterPosition();

        /// <summary>
        /// Marks all counters dirty in this node and child nodes.
        /// </summary>
        /// <param name="notifyCounterSource">if set to <c>true</c> notify counter source.</param>
        void InvalidateCounterTopDown(bool notifyCounterSource);
    }

    /// <summary>
    /// Interface definition for an object that has counters.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public interface ITreeTableCounterSource
    {
        /// <summary>
        /// Returns the counter object with counters.
        /// </summary>
        /// <returns></returns>
        ITreeTableCounter GetCounter();
        /// <summary>
        /// Marks all counters dirty in this object and child nodes.
        /// </summary>
        /// <param name="notifyCounterSource">if set to <c>true</c> notify counter source.</param>
        void InvalidateCounterTopDown(bool notifyCounterSource);

        /// <summary>
        /// Marks all counters dirty in this object and parent nodes.
        /// </summary>
        void InvalidateCounterBottomUp();
    }

    /// <summary>
    /// Interface definition for a counter object.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public interface ITreeTableCounter
    {
        /// <summary>
        /// Combines this counter object with another counter and returns a new object. A cookie can specify
        /// a specific counter type.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <param name="cookie">The cookie.</param>
        /// <returns></returns>
        ITreeTableCounter Combine(ITreeTableCounter other, int cookie);

        /// <summary>
        /// Compares this counter with another counter. A cookie can specify
        /// a specific counter type.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <param name="cookie">The cookie.</param>
        /// <returns></returns>
        double Compare(ITreeTableCounter other, int cookie);

        /// <summary>
        /// Indicates whether the counter object is empty. A cookie can specify
        /// a specific counter type.
        /// </summary>
        /// <param name="cookie">The cookie.</param>
        /// <returns>
        /// 	<c>true</c> if the specified cookie is empty; otherwise, <c>false</c>.
        /// </returns>
        bool IsEmpty(int cookie);

        /// <summary>
        /// Returns the integer value of the counter. A cookie specifies
        /// a specific counter type.
        /// </summary>
        /// <param name="cookie">The cookie.</param>
        /// <returns></returns>
        double GetValue(int cookie);


        /// <summary>
        /// Gets the Counter Kind.
        /// </summary>
        /// <value>The kind.</value>
        int Kind { get; }
    }


    /// <summary>
    /// Default counter cookies for identifying counter types.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class TreeTableCounterCookies
    {
        /// <summary>
        /// All counters.
        /// </summary>
        public const int CountAll = 0xffff;
        /// <summary>
        /// Visible Counter.
        /// </summary>
        public const int CountVisible = 0x8000;
    }

    /// <summary>
    /// A counter that counts objects that are marked "Visible".
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class TreeTableVisibleCounter : ITreeTableCounter
    {
        double visibleCount;

        /// <summary>
        /// Returns an empty TreeTableVisibleCounter that represents zero visible elements.
        /// </summary>
        public static readonly TreeTableVisibleCounter Empty = new TreeTableVisibleCounter(0);

        /// <summary>
        /// Initializes a <see cref="TreeTableVisibleCounter"/> with a specified number of visible elements.
        /// </summary>
        /// <param name="visibleCount">The visible count.</param>
        public TreeTableVisibleCounter(double visibleCount)
        {
            this.visibleCount = visibleCount;
        }

        /// <summary>
        /// The Counter Kind.
        /// </summary>
        public virtual int Kind { get { return 0; } }

        /// <summary>
        /// Returns the visible count.
        /// </summary>
        public double GetVisibleCount()
        {
            return visibleCount;
        }

        /// <summary>
        /// Returns the integer value of the counter. A cookie specifies
        /// a specific counter type.
        /// </summary>
        /// <param name="cookie">The cookie.</param>
        /// <returns></returns>
        public virtual double GetValue(int cookie)
        {
            return visibleCount;
        }


        /// <summary>
        /// Combines one tree object with another and returns the new object.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <param name="cookie">The cookie.</param>
        /// <returns></returns>
        ITreeTableCounter ITreeTableCounter.Combine(ITreeTableCounter other, int cookie)
        {
            return Combine((TreeTableVisibleCounter)other, cookie);
        }

        /// <summary>
        /// Factory method creates a new counter object of the same type as this object.
        /// </summary>
        /// <returns></returns>
        public virtual TreeTableVisibleCounter CreateCounter()
        {
            return new TreeTableVisibleCounter(0);
        }

        /// <summary>
        /// Called to combine the values of two counter objects. Results are saved back into this counter object.
        /// A cookie can filter the operation to a limited set of counter types.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="cookie">The cookie.</param>
        protected virtual void OnCombineCounters(ITreeTableCounter x, ITreeTableCounter y, int cookie)
        {
        }

        /// <summary>
        /// Combines the counter values of this counter object with the values of another counter object
        /// and returns a new counter object.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <param name="cookie">The cookie.</param>
        /// <returns></returns>
        public TreeTableVisibleCounter Combine(TreeTableVisibleCounter other, int cookie)
        {
            if (other == null || other.IsEmpty(int.MaxValue))
                return this;

            if (this.IsEmpty(int.MaxValue))
                return other;
            TreeTableVisibleCounter counter = CreateCounter();
            counter.visibleCount = GetVisibleCount() + other.GetVisibleCount();
            counter.OnCombineCounters(this, other, cookie);
            return counter;
        }

        double ITreeTableCounter.Compare(ITreeTableCounter other, int cookie)
        {
            return Compare((TreeTableVisibleCounter)other, cookie);
        }

        /// <summary>
        /// Compares this counter with another counter. A cookie can specify
        /// a specific counter type.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <param name="cookie">The cookie.</param>
        /// <returns></returns>
        public virtual double Compare(TreeTableVisibleCounter other, int cookie)
        {
            if (other == null)
                return 0;

            if ((cookie & TreeTableCounterCookies.CountVisible) != 0)
                return GetVisibleCount() - other.GetVisibleCount();

            return 0;
        }

        /// <summary>
        /// Indicates whether the counter object is empty. A cookie can specify
        /// a specific counter type.
        /// </summary>
        /// <param name="cookie">The cookie.</param>
        /// <returns>
        /// 	<c>true</c> if the specified cookie is empty; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsEmpty(int cookie)
        {
            return GetVisibleCount() == 0;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents the current <see cref="System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return "Count = " + this.GetVisibleCount().ToString();
        }

    }

    /// <summary>
    /// A tree table branch with a counter.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class TreeTableWithCounterBranch : TreeTableWithSummaryBranch, ITreeTableCounterNode
    {
        ITreeTableCounter _counter = null;

        /// <summary>
        /// Initializes a new <see cref="TreeTableWithCounterBranch"/>.
        /// </summary>
        /// <param name="tree"></param>
        public TreeTableWithCounterBranch(TreeTable tree)
            : base(tree)
        {
        }

        /// <summary>
        /// Returns the tree this branch belongs to.
        /// </summary>
        public TreeTableWithCounter TreeTableWithCounter
        {
            get
            {
                return (TreeTableWithCounter)base.Tree;
            }
        }

        /// <summary>
        /// Gets / sets the parent branch.
        /// </summary>
        public new TreeTableWithCounterBranch Parent
        {
            get
            {
                return base.Parent as TreeTableWithCounterBranch;
            }
            set
            {
                base.Parent = value;
            }
        }

        /// <summary>
        /// Returns the cumulative counter position object of a child node with all counter values.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public ITreeTableCounter GetCounterPositionOfChild(ITreeTableNode node)
        {
            ITreeTableCounter pos = GetCounterPosition();

            if (Object.ReferenceEquals(node, Right))
                return pos.Combine(GetLeftC().GetCounterTotal(), TreeTableCounterCookies.CountAll);

            else if (Object.ReferenceEquals(node, Left))
                return pos;

            throw new ArgumentException("must be a child node", "node");
        }

        /// <summary>
        /// Returns the left branch node cast to ITreeTableCounterNode.
        /// </summary>
        /// <returns></returns>
        public new ITreeTableCounterNode GetLeftC()
        {
            return (ITreeTableCounterNode)Left;
        }

        /// <summary>
        /// Returns the right branch node cast to ITreeTableCounterNode.
        /// </summary>
        /// <returns></returns>
        public new ITreeTableCounterNode GetRightC()
        {
            return (ITreeTableCounterNode)Right;
        }

        /// <summary>
        /// Returns the total of this node's counter and child nodes (cached).
        /// </summary>
        public ITreeTableCounter GetCounterTotal()
        {
            if (Tree.IsInitializing)
                return null;
            else if (_counter == null)
            {
                ITreeTableCounter left = GetLeftC().GetCounterTotal();
                ITreeTableCounter right = GetRightC().GetCounterTotal();
                if (left != null && right != null)
                    _counter = left.Combine(right, TreeTableCounterCookies.CountAll);
            }
            return _counter;
        }


        /// <summary>
        /// Returns the cumulative position of this node.
        /// </summary>
        public ITreeTableCounter GetCounterPosition()
        {
            if (Parent == null)
                return this.TreeTableWithCounter.GetStartCounterPosition();
            return Parent.GetCounterPositionOfChild(this);
        }

        /// <summary>
        /// Invalidates the counter bottom up.
        /// </summary>
        /// <param name="notifyCounterSource">if set to <c>true</c> notify counter source.</param>
        public override void InvalidateCounterBottomUp(bool notifyCounterSource)
        {
            if (Tree.IsInitializing)
                return;

            _counter = null;
            if (Parent != null)
                Parent.InvalidateCounterBottomUp(notifyCounterSource);
            else if (notifyCounterSource)
            {
                if (Tree is TreeTableWithCounter)
                {
                    ITreeTableCounterSource tcs;
                    tcs = Tree.Tag as ITreeTableCounterSource;
                    if (tcs != null)
                        tcs.InvalidateCounterBottomUp();

                    tcs = ((TreeTableWithCounter)Tree).ParentCounterSource;
                    if (tcs != null)
                        tcs.InvalidateCounterBottomUp();
                }
            }
        }

        /// <summary>
        /// Marks all counters dirty in this node and child nodes.
        /// </summary>
        /// <param name="notifyCounterSource">if set to <c>true</c> notify counter source.</param>
        public void InvalidateCounterTopDown(bool notifyCounterSource)
        {
            if (Tree.IsInitializing)
                return;

            _counter = null;
            GetLeftC().InvalidateCounterTopDown(notifyCounterSource);
            GetRightC().InvalidateCounterTopDown(notifyCounterSource);
        }
    }


    /// <summary>
    /// A tree leaf with value, sort key and counter information.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class TreeTableWithCounterEntry : TreeTableWithSummaryEntry, ITreeTableCounterNode
    {
        static internal bool traceVisibleCount = false;
        ITreeTableCounter counter = null;

        /// <summary>
        /// Returns the tree this leaf belongs to.
        /// </summary>
        public TreeTableWithCounter TreeTableWithCounter
        {
            get
            {
                return (TreeTableWithCounter)this.Tree;
            }
        }

        /// <summary>
        /// Gets / sets the parent branch.
        /// </summary>
        public new TreeTableWithCounterBranch Parent
        {
            get
            {
                return base.Parent as TreeTableWithCounterBranch;
            }
            set
            {
                base.Parent = value;
            }
        }

        /// <summary>
        /// Creates a branch that can hold this entry when new leaves are inserted into the tree.
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public override ITreeTableBranch CreateBranch(TreeTable tree)
        {
            return new TreeTableWithCounterBranch(tree);
        }

        /// <summary>
        /// Returns the cumulative position of this node.
        /// </summary>
        public virtual ITreeTableCounter GetCounterPosition()
        {
            if (Parent == null)
            {
                if (TreeTableWithCounter == null)
                    return null;
                return TreeTableWithCounter.GetStartCounterPosition();
            }

            return Parent.GetCounterPositionOfChild(this);
        }


        /// <summary>
        /// Returns the value as <see cref="ITreeTableCounterSource"/>.
        /// </summary>
        internal virtual ITreeTableCounterSource GetCounterSource()
        {
            return Value as ITreeTableCounterSource;
        }

        /// <summary>
        /// Indicates whether the counter was set dirty.
        /// </summary>
        /// <returns>True if dirty; False otherwise.</returns>
        public bool IsCounterDirty()
        {
            return counter == null;
        }

        /// <summary>
        /// Returns the total of this node's counter and child nodes.
        /// </summary>
        public ITreeTableCounter GetCounterTotal()
        {
            if (counter == null)
            {
                ITreeTableCounterSource source = GetCounterSource();
                if (source != null)
                    counter = source.GetCounter();
            }
            return counter;
        }

        /// <summary>
        /// Reset cached counter.
        /// </summary>
        public virtual void InvalidateCounter()
        {
            this.counter = null;
        }


        /// <summary>
        /// Invalidates the counter bottom up.
        /// </summary>
        /// <param name="notifyCounterSource">if set to <c>true</c> notify counter source.</param>
        public override void InvalidateCounterBottomUp(bool notifyCounterSource)
        {
            counter = null;
            if (Parent != null)
                Parent.InvalidateCounterBottomUp(notifyCounterSource);
            else if (notifyCounterSource)
            {
                if (Tree is TreeTableWithCounter)
                {
                    var tcs = Tree.Tag as ITreeTableCounterSource;
                    if (tcs != null)
                        tcs.InvalidateCounterBottomUp();

                    tcs = ((TreeTableWithCounter)Tree).ParentCounterSource;
                    if (tcs != null)
                        tcs.InvalidateCounterBottomUp();
                }
            }
        }

        /// <summary>
        /// Marks all summaries dirty in this node and child nodes.
        /// </summary>
        /// <param name="notifyCounterSource">if set to <c>true</c> notify counter source.</param>
        public void InvalidateCounterTopDown(bool notifyCounterSource)
        {
            counter = null;
            if (notifyCounterSource)
            {
                ITreeTableCounterSource source = GetCounterSource();
                if (notifyCounterSource && source != null)
                    source.InvalidateCounterTopDown(notifyCounterSource);
            }
        }
    }

    /// <summary>
    /// A balanced tree with <see cref="TreeTableWithCounterEntry"/> entries.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class TreeTableWithCounter : TreeTableWithSummary
    {
        ITreeTableCounter _startPos;

        /// <summary>
        /// Initializes a new <see cref="TreeTableWithCounter"/>.
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="sorted"></param>
        public TreeTableWithCounter(ITreeTableCounter startPos, bool sorted)
            : base(sorted)
        {
            _startPos = startPos;
        }

        public ITreeTableCounterSource ParentCounterSource
        {
            get;
            set;
        }

        /// <summary>
        /// Ends optimization of insertion of elements when tree is initialized for the first time.
        /// </summary>
        public override void EndInit()
        {
            base.EndInit();
        }

        /// <summary>
        /// Marks all counters dirty.
        /// </summary>
        /// <param name="notifyCounterSource"></param>
        public void InvalidateCounterTopDown(bool notifyCounterSource)
        {
            if (Root != null)
                ((ITreeTableCounterNode)this.Root).InvalidateCounterTopDown(notifyCounterSource);
        }

        /// <summary>
        /// Returns the total of all counters in this tree.
        /// </summary>
        public ITreeTableCounter GetCounterTotal()
        {
            if (this.Root == null)
                return _startPos;

            return ((ITreeTableCounterNode)this.Root).GetCounterTotal();
        }

        /// <summary>
        /// Returns the starting counter for this tree.
        /// </summary>
        public ITreeTableCounter GetStartCounterPosition()
        {
            return _startPos;
        }

        /// <summary>
        /// Overloaded. Returns an entry at the specified counter position. A cookie defines the type of counter.
        /// </summary>
        /// <param name="searchPosition">The search position.</param>
        /// <param name="cookie">The cookie.</param>
        /// <returns></returns>
        public TreeTableWithCounterEntry GetEntryAtCounterPosition(ITreeTableCounter searchPosition, int cookie)
        {
            return GetEntryAtCounterPosition(GetStartCounterPosition(), searchPosition, cookie, false);
        }

        /// <summary>
        /// Returns an entry at the specified counter position. A cookie defines the type of counter.
        /// </summary>
        /// <param name="searchPosition">The search position.</param>
        /// <param name="cookie">The cookie.</param>
        /// <param name="preferLeftMost">Indicates if the leftmost entry should be returned if multiple tree elements have the
        /// same searchPosition.</param>
        /// <returns></returns>
        public TreeTableWithCounterEntry GetEntryAtCounterPosition(ITreeTableCounter searchPosition, int cookie, bool preferLeftMost)
        {
            return GetEntryAtCounterPosition(GetStartCounterPosition(), searchPosition, cookie, preferLeftMost);
        }

        /// <summary>
        /// Gets the entry at counter position.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="searchPosition">The search position.</param>
        /// <param name="cookie">The cookie.</param>
        /// <returns></returns>
        TreeTableWithCounterEntry GetEntryAtCounterPosition(ITreeTableCounter start, ITreeTableCounter searchPosition, int cookie)
        {
            return GetEntryAtCounterPosition(start, searchPosition, cookie, false);
        }

        /// <summary>
        /// Gets the entry at counter position.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="searchPosition">The search position.</param>
        /// <param name="cookie">The cookie.</param>
        /// <param name="preferLeftMost">if set to <c>true</c> prefer left most.</param>
        /// <returns></returns>
        TreeTableWithCounterEntry GetEntryAtCounterPosition(ITreeTableCounter start, ITreeTableCounter searchPosition, int cookie, bool preferLeftMost)
        {
            int treeNodeCount = GetCount();

            //if (searchPosition < 0 || searchPosition >= VisibleCount)
            if (searchPosition.Compare(GetStartCounterPosition(), cookie) < 0)
                throw new ArgumentOutOfRangeException("searchPosition");

            if (searchPosition.Compare(GetCounterTotal(), cookie) > 0)
            {
                throw new ArgumentOutOfRangeException("searchPosition", String.Format("{0} out of range {1}", searchPosition, GetCounterTotal()));
            }

            if (this.Root == null)
            {
                return null;
            }
            else
            {
                // find node
                ITreeTableNode currentNode = this.Root;
                ITreeTableCounter currentNodePosition = start;
                return GetEntryAtCounterPosition(currentNode, start, searchPosition, cookie, preferLeftMost, out currentNodePosition);
            }
        }

        TreeTableWithCounterEntry GetEntryAtCounterPosition(ITreeTableNode currentNode, ITreeTableCounter start, ITreeTableCounter searchPosition, int cookie, bool preferLeftMost, out ITreeTableCounter currentNodePosition)
        {
            TreeTableWithCounterBranch savedBranch = null;
            ITreeTableCounter savedPosition = null;

            currentNodePosition = start;
            while (!currentNode.IsEntry())
            {
                var branch = (TreeTableWithCounterBranch)currentNode;
                var leftB = (ITreeTableCounterNode)branch.Left;
                ITreeTableCounter rightNodePosition = currentNodePosition.Combine(leftB.GetCounterTotal(), cookie);

                if (searchPosition.Compare(rightNodePosition, cookie) < 0)
                    currentNode = branch.Left;
                else if (preferLeftMost && searchPosition.Compare(currentNodePosition, cookie) == 0)
                {
                    while (!currentNode.IsEntry())
                    {
                        branch = (TreeTableWithCounterBranch)currentNode;
                        currentNode = branch.Left;
                    }
                }
                else
                {
                    // When the right node matches the searchPosition, there might be entries
                    // with the same position in the left branch. For example, there might be
                    // several subsequent tokens on a line in a text editor. Each token will
                    // have the same line index. When searching for the first token in a line,
                    // the method will at that time also check the rightmost nodes in the left
                    // branch.
                    //
                    // When preferLeftMost is False, the last token in the line will be returned.
                    // When preferLeftMost is True, the first last token in the line will be returned.
                    //
                    // Note: This only works for "direct hits", that means when the search position
                    // matches the right node's position. If you search for the "greatest counter
                    // smaller or equal than searchPosition", the latest node will be returned no
                    // matter if there were nodes with the same counter before.
                    //
                    // Take the YAmountCounter in a TextEditor for example. If you search
                    // for a YAmount between lines, the last token of the line will be returned.
                    // In the TextBuffer class special consideration is taken into account for
                    // this scenario. A generic solution would be too costly in this method.
                    if (preferLeftMost && searchPosition.Compare(rightNodePosition, cookie) == 0)
                    {
                        ITreeTableCounter currentNode2Position = null;
                        ITreeTableNode currentNode2 = GetEntryAtCounterPosition(branch.Left, currentNodePosition, searchPosition, cookie, preferLeftMost, out currentNode2Position);
                        if (rightNodePosition.Compare(currentNode2Position, cookie) == 0)
                        {
                            currentNode = currentNode2;
                            currentNodePosition = currentNode2Position;
                        }
                        else
                        {
                            currentNodePosition = rightNodePosition;
                            currentNode = branch.Right;
                        }
                    }
                    else
                    {
                        if (savedBranch == null)
                        {
                            savedBranch = branch;
                            savedPosition = currentNodePosition;
                        }
                        currentNodePosition = rightNodePosition;
                        currentNode = branch.Right;
                    }
                }
            }

            //			if (preferLeftMost && savedBranch != null)
            //			{
            //				ITreeTableCounter currentNode2Position = null;
            //				ITreeTableNode currentNode2 = GetEntryAtCounterPosition(savedBranch.Left, savedPosition, searchPosition, cookie, preferLeftMost, out currentNode2Position);
            //				if (currentNodePosition.Compare(currentNode2Position, cookie) == 0)
            //					currentNode = currentNode2;
            //
            //				while (!currentNode.IsEntry())
            //				{
            //					TreeTableWithCounterBranch branch = (TreeTableWithCounterBranch) currentNode;
            //					currentNode = branch.Left;
            //				}
            //			}
            return (TreeTableWithCounterEntry)currentNode;
        }

        /// <summary>
        /// Returns the subsequent entry in the collection for which the specific counter is not empty.
        /// A cookie defines the type of counter.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="cookie">The cookie.</param>
        /// <returns></returns>
        public ITreeTableEntry GetNextNotEmptyCounterEntry(ITreeTableEntry current, int cookie)
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
                next = current;
                // walk up until we find a branch that has visible entries
                do
                {
                    if (Object.ReferenceEquals(next, parent.Left))
                        next = parent.Right;
                    else
                    {
                        ITreeTableBranch parentParent = parent.Parent;
                        Debug.Assert(parentParent != parent);
                        if (parentParent == null)
                        {
                            return null;
                        }
                        else
                        {
                            while (Object.ReferenceEquals(parentParent.Right, parent)
                                // TODO: this second statement is a workaround
                                // for something that most likely went wrong when
                                // adding the node or when doing a rotation ...
                                || Object.ReferenceEquals(parentParent.Right, next)
                                )
                            {
                                parent = parentParent;
                                parentParent = parentParent.Parent;
                                if (parentParent == null)
                                {
                                    return null;
                                }
                            }

                            //Debug.Assert(next != parentParent.Right);
                            if (next == parentParent.Right)
                            {
                                throw new Exception();
                                //return null;
                            }
                            else
                            {


                                next = parentParent.Right;
                            }
                        }
                    }
                }
                while (next != null && ((ITreeTableCounterNode)next).GetCounterTotal().IsEmpty(cookie));

                // walk down to most left leaf that has visible entries
                while (!next.IsEntry())
                {
                    var branch = (ITreeTableBranch)next;
                    next = !((ITreeTableCounterNode)branch.Left).GetCounterTotal().IsEmpty(cookie) ? branch.Left : branch.Right;
                }
            }
            return next as ITreeTableEntry;
        }


        /// <summary>
        /// Returns the previous entry in the collection for which the specific counter is not empty.
        /// A cookie defines the type of counter.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="cookie">The cookie.</param>
        /// <returns></returns>
        public ITreeTableEntry GetPreviousNotEmptyCounterEntry(ITreeTableEntry current, int cookie)
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
                next = current;
                // walk up until we find a branch that has visible entries
                do
                {
                    if (Object.ReferenceEquals(next, parent.Right))
                        next = parent.Left;
                    else
                    {
                        ITreeTableBranch parentParent = parent.Parent;
                        Debug.Assert(parentParent != parent);
                        if (parentParent == null)
                        {
                            return null;
                        }
                        else
                        {
                            while (Object.ReferenceEquals(parentParent.Left, parent)
                                // TODO: this second statement is a workaround
                                // for something that most likely went wrong when
                                // adding the node or when doing a rotation ...
                                || Object.ReferenceEquals(parentParent.Left, next)
                                )
                            {
                                parent = parentParent;
                                parentParent = parentParent.Parent;
                                if (parentParent == null)
                                {
                                    return null;
                                }
                            }

                            //Debug.Assert(next != parentParent.Right);
                            if (next == parentParent.Left)
                            {
                                throw new Exception();
                                //return null;
                            }
                            else
                            {


                                next = parentParent.Left;
                            }
                        }
                    }
                }
                while (next != null && ((ITreeTableCounterNode)next).GetCounterTotal().IsEmpty(cookie));

                // walk down to most left leaf that has visible entries
                while (!next.IsEntry())
                {
                    var branch = (ITreeTableBranch)next;
                    next = !((ITreeTableCounterNode)branch.Right).GetCounterTotal().IsEmpty(cookie) ? branch.Right : branch.Left;
                }
            }
            return next as ITreeTableEntry;
        }

        /// <summary>
        /// Returns the next entry in the collection for which CountVisible counter is not empty.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <returns></returns>
        public TreeTableWithCounterEntry GetNextVisibleEntry(TreeTableWithCounterEntry current)
        {
            return (TreeTableWithCounterEntry)GetNextNotEmptyCounterEntry((ITreeTableEntry)current, TreeTableCounterCookies.CountVisible);
        }


        /// <summary>
        /// Returns the previous entry in the collection for which CountVisible counter is not empty.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <returns></returns>
        public TreeTableWithCounterEntry GetPreviousVisibleEntry(TreeTableWithCounterEntry current)
        {
            return (TreeTableWithCounterEntry)GetPreviousNotEmptyCounterEntry((ITreeTableEntry)current, TreeTableCounterCookies.CountVisible);
        }

        /// <summary>
        /// Gets / sets a TreeTableWithCounterEntry.
        /// </summary>
        public new TreeTableWithCounterEntry this[int index]
        {
            get
            {
                return (TreeTableWithCounterEntry)base[index];
            }
            set
            {
                base[index] = value;
            }
        }


        /// <summary>
        /// Inserts a <see cref="TreeTableWithCounterEntry"/> object at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        public void Insert(int index, TreeTableWithCounterEntry value)
        {
            base.Insert(index, value);
        }

        /// <summary>
        /// Removes an object from the tree.
        /// </summary>
        /// <param name="value">The value.</param>
        public bool Remove(TreeTableWithCounterEntry value)
        {
            return base.Remove(value);
        }

        /// <summary>
        /// Indicates whether an entry belongs to the tree.
        /// </summary>
        /// <param name="value">The entry.</param>
        /// <returns>
        /// 	<c>true</c> if tree contains the specified entry; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(TreeTableWithCounterEntry value)
        {
            if (value == null)
                return false;

            return base.Contains(value);
        }

        /// <summary>
        /// Returns the position of an object in the tree.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public int IndexOf(TreeTableWithCounterEntry value)
        {
            return base.IndexOf(value);
        }

        /// <summary>
        /// Appends an object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public int Add(TreeTableWithCounterEntry value)
        {
            return base.Add(value);
        }

        /// <summary>
        /// Copies the elements of this tree to an array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="index">The index.</param>
        public void CopyTo(TreeTableWithCounterEntry[] array, int index)
        {
            base.CopyTo((ITreeTableNode[])array, index);
        }

        /// <summary>
        /// Returns a strongly typed enumerator.
        /// </summary>
        /// <returns></returns>
        public new TreeTableWithCounterEnumerator GetEnumerator()
        {
            return new TreeTableWithCounterEnumerator(this);
        }
    }

    /// <summary>
    /// A strongly typed enumerator for the <see cref="TreeTableWithCounter"/> collection.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class TreeTableWithCounterEnumerator : TreeTableEnumerator
    {
        /// <summary>
        /// Initializes a new <see cref="TreeTableWithCounterEnumerator"/>.
        /// </summary>
        /// <param name="tree"></param>
        public TreeTableWithCounterEnumerator(TreeTable tree)
            : base(tree)
        {
        }

        /// <summary>
        /// Returns the current <see cref="TreeTableWithCounter"/> object.
        /// </summary>
        public new TreeTableWithCounterEntry Current
        {
            get
            {
                return (TreeTableWithCounterEntry)base.Current;
            }
        }
    }

    /// <summary>
    /// An object that counts objects that are marked "Visible". It implements
    /// the ITreeTableCounterSource interface and creates a <see cref="TreeTableVisibleCounter"/>.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class TreeTableVisibleCounterSource : ITreeTableCounterSource
    {
        double visibleCount;

        /// <summary>
        /// Initializes the object with visible count.
        /// </summary>
        /// <param name="visibleCount">The visible count.</param>
        public TreeTableVisibleCounterSource(double visibleCount)
        {
            this.visibleCount = visibleCount;
        }

        #region ITreeTableCounterSource Members

        /// <summary>
        /// Marks all counters dirty in this object and parent nodes.
        /// </summary>
        public virtual void InvalidateCounterBottomUp()
        {
        }

        /// <summary>
        /// Returns the counter object with counters.
        /// </summary>
        /// <returns></returns>
        public virtual ITreeTableCounter GetCounter()
        {
            return new TreeTableVisibleCounter(visibleCount);
        }

        /// <summary>
        /// Marks all counters dirty in this object and child nodes.
        /// </summary>
        /// <param name="notifyCounterSource">if set to <c>true</c> notify counter source.</param>
        public virtual void InvalidateCounterTopDown(bool notifyCounterSource)
        {
        }

        #endregion
    }
}
