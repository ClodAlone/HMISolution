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

namespace Syncfusion.Windows.Collections
{
	/// <summary>
	/// Interface definition for a summary object.
	/// </summary>
	public interface ITreeTableSummary
	{
        /// <summary>
        /// Combines this summary information with another object's summary and returns a new object.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
		ITreeTableSummary Combine(ITreeTableSummary other);
	}

	/// <summary>
	/// Interface definition for a node that has one or more summaries.
	/// </summary>
	public interface ITreeTableSummaryNode : ITreeTableNode
	{
		/// <summary>
		/// Indicates whether node has summaries.
		/// </summary>
		bool HasSummaries { get; }
        /// <summary>
        /// Returns an array of summary objects.
        /// </summary>
        /// <param name="emptySummaries">The empty summaries.</param>
        /// <returns></returns>
		ITreeTableSummary[] GetSummaries(ITreeTableEmptySummaryArraySource emptySummaries);
        /// <summary>
        /// Marks all summaries dirty in this node and child nodes.
        /// </summary>
        /// <param name="notifyEntrySummary">if set to <c>true</c> notify entry summary.</param>
		void InvalidateSummariesTopDown(bool notifyEntrySummary);
	}

	/// <summary>
	/// Provides a <see cref="GetEmptySummaries"/> method.
	/// </summary>
	public interface ITreeTableEmptySummaryArraySource
	{
		/// <summary>
		/// Gets an array of summary objects.
		/// </summary>
		ITreeTableSummary[] GetEmptySummaries();
	}


	/// <summary>
	/// Interface definition for an object that has summaries.
	/// </summary>
	public interface ITreeTableSummaryArraySource
	{
        /// <summary>
        /// Returns an array of summary objects.
        /// </summary>
        /// <param name="emptySummaries">An array of empty summary objects.</param>
        /// <param name="changed">Returns True if summaries were recalculated; False if already cached.</param>
        /// <returns>An array of summary objects.</returns>
		ITreeTableSummary[] GetSummaries(ITreeTableEmptySummaryArraySource emptySummaries, out bool changed);

		/// <summary>
		/// Marks all summaries dirty in this object and child nodes.
		/// </summary>
		void InvalidateSummariesTopDown();

		/// <summary>
		/// Marks all summaries dirty in this object and parent nodes.
		/// </summary>
		void InvalidateSummariesBottomUp();

		/// <summary>
		/// Marks all summaries dirty in this object only.
		/// </summary>
		void InvalidateSummary();
	}

	/// <summary>
	/// A tree table branch with a counter.
	/// </summary>
	public class TreeTableWithSummaryBranch : TreeTableBranch, ITreeTableSummaryNode
	{
		ITreeTableSummary[] _summaries = null;

		/// <summary>
		/// Initializes a new <see cref="TreeTableWithSummaryBranch"/>.
		/// </summary>
		/// <param name="tree"></param>
		public TreeTableWithSummaryBranch(TreeTable tree)
			: base(tree)
		{
		}

		/// <summary>
		/// Returns the tree this branch belongs to.
		/// </summary>
		public TreeTableWithSummary TreeTableWithSummary
		{
			get
			{
				return (TreeTableWithSummary) base.Tree;
			}
		}

		/// <summary>
		/// Gets / sets the parent branch.
		/// </summary>
		public new TreeTableWithSummaryBranch Parent
		{
			get
			{
				return base.Parent as TreeTableWithSummaryBranch;
			}
			set
			{
				base.Parent  = value;
			}
		}

		/// <summary>
		/// Returns the left branch node cast to ITreeTableSummaryNode.
		/// </summary>
		/// <returns></returns>
		public ITreeTableSummaryNode GetLeftC()
		{
			return (ITreeTableSummaryNode) Left;
		}

		/// <summary>
		/// Returns the right branch node cast to ITreeTableSummaryNode.
		/// </summary>
		/// <returns></returns>
		public ITreeTableSummaryNode GetRightC()
		{
			return (ITreeTableSummaryNode) Right;
		}

		/// <summary>
		/// Indicates whether this node has summaries.
		/// </summary>
		public bool HasSummaries
		{
			get { return _summaries != null; }
		}

        /// <summary>
        /// Returns an array of summary objects.
        /// </summary>
        /// <param name="emptySummaries">The empty summaries.</param>
        /// <returns></returns>
		public ITreeTableSummary[] GetSummaries(ITreeTableEmptySummaryArraySource emptySummaries)
		{
			if (Tree.IsInitializing)
				return null;
			else if (_summaries == null)
			{
				ITreeTableSummary[] left = GetLeftC().GetSummaries(emptySummaries);
				ITreeTableSummary[] right = GetRightC().GetSummaries(emptySummaries);
				if (left != null && right != null)
				{
					int reuseLeft = 0;
					int reuseRight = 0;
					_summaries = new ITreeTableSummary[left.Length];
					for (int i = 0; i < _summaries.Length; i++)
					{
						_summaries[i] = left[i].Combine(right[i]);

						// preserve memory optimization
						if (reuseLeft == i || reuseRight == i)
						{
							if (Object.ReferenceEquals(_summaries[i], left[i]))
								reuseLeft++;
							else if (Object.ReferenceEquals(_summaries[i], right[i]))
								reuseRight++;
						}
					}

					// preserve memory optimization
					if (reuseLeft == _summaries.Length)
						_summaries = left;
					else if (reuseRight == _summaries.Length)
						_summaries = right;
				}
			}
			return _summaries;
		}

        /// <summary>
        /// Walks up parent branches and reset summaries.
        /// </summary>
        /// <param name="notifyParentRecordSource"></param>
		public override void InvalidateSummariesBottomUp(bool notifyParentRecordSource)
		{
			if (Tree.IsInitializing)
				return;

			_summaries = null;
			if (Parent != null)
				Parent.InvalidateSummariesBottomUp(notifyParentRecordSource);
			else if (notifyParentRecordSource)
			{
				if (Tree != null && Tree.Tag is ITreeTableSummaryArraySource)
				{
					((ITreeTableSummaryArraySource) Tree.Tag).InvalidateSummariesBottomUp();
				}
			}
		}


        /// <summary>
        /// Marks all summaries dirty in this node and child nodes.
        /// </summary>
        /// <param name="notifyCounterSource">if set to <c>true</c> notify counter source.</param>
		public void InvalidateSummariesTopDown(bool notifyCounterSource)
		{
			if (Tree.IsInitializing)
				return;

			_summaries = null;
			GetLeftC().InvalidateSummariesTopDown(notifyCounterSource);
			GetRightC().InvalidateSummariesTopDown(notifyCounterSource);
		}
	}


	/// <summary>
	/// A tree leaf with value and summary information.
	/// </summary>
	public class TreeTableWithSummaryEntry : TreeTableEntry, ITreeTableSummaryNode
	{
		static readonly ITreeTableSummary[] emptySummaryArray = new ITreeTableSummary[0];


		/// <summary>
		/// Returns the tree this leaf belongs to.
		/// </summary>
		public TreeTableWithSummary TreeTableWithSummary
		{
			get
			{
				return (TreeTableWithSummary) this.Tree;
			}
		}

		/// <summary>
		/// Gets / sets the parent branch.
		/// </summary>
		public new TreeTableWithSummaryBranch Parent
		{
			get
			{
				return base.Parent as TreeTableWithSummaryBranch;
			}
			set
			{
				base.Parent  = value;
			}
		}

        /// <summary>
        /// Creates a branch that can hold this entry when new leaves are inserted into the tree.
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
		public override ITreeTableBranch CreateBranch(TreeTable tree)
		{
			return new TreeTableWithSummaryBranch(tree);
		}

		ITreeTableSummary[] summaries = null;


		/// <summary>
		/// Indicates whether the node has summaries.
		/// </summary>
		public bool HasSummaries
		{
			get { return summaries != null; }
		}


        /// <summary>
        /// Returns an array of summary objects.
        /// </summary>
        /// <param name="emptySummaries">The empty summaries.</param>
        /// <returns></returns>
		public ITreeTableSummary[] GetSummaries(ITreeTableEmptySummaryArraySource emptySummaries)
		{
			if (summaries == null)
			{
				summaries = OnGetSummaries(emptySummaries);

				if (summaries == null)
					summaries = emptySummaryArray;

			}
			return summaries;
		}

        /// <summary>
        /// Called from <see cref="GetSummaries"/> when called the first time after summaries were invalidated.
        /// </summary>
        /// <param name="emptySummaries">The empty summaries.</param>
        /// <returns></returns>
		public virtual ITreeTableSummary[] OnGetSummaries(ITreeTableEmptySummaryArraySource emptySummaries)
		{
			ITreeTableSummary[] summaries = null;
			ITreeTableSummaryArraySource summaryArraySource = GetSummaryArraySource();
			if (summaryArraySource != null)
			{
				bool summaryChanged;
				summaries = summaryArraySource.GetSummaries(emptySummaries, out summaryChanged);
			}
			return summaries;
		}
		
		/// <summary>
		/// Returns the value as <see cref="ITreeTableSummaryArraySource"/>.
		/// </summary>
		public virtual ITreeTableSummaryArraySource GetSummaryArraySource()
		{
			return Value as ITreeTableSummaryArraySource;
		}

        /// <summary>
        /// Walks up parent branches and reset summaries.
        /// </summary>
        /// <param name="notifyParentRecordSource"></param>
		public override void InvalidateSummariesBottomUp(bool notifyParentRecordSource)
		{
			summaries = null;
			if (Value is ITreeTableSummaryArraySource && Tree != null)
				((ITreeTableSummaryArraySource) Tree.Tag).InvalidateSummary();

			if (Parent != null)
				Parent.InvalidateSummariesBottomUp(notifyParentRecordSource);
			else if (notifyParentRecordSource)
			{
				if (Tree != null && Tree.Tag is ITreeTableSummaryArraySource)
				{
					((ITreeTableSummaryArraySource) Tree.Tag).InvalidateSummariesBottomUp();
				}
			}
		}

        /// <summary>
        /// Marks all summaries dirty in this node and child nodes.
        /// </summary>
        /// <param name="notifySummaryArraySource">if set to <c>true</c> notify summary array source.</param>
		public void InvalidateSummariesTopDown(bool notifySummaryArraySource)
		{
			summaries = null;
			if (notifySummaryArraySource)
			{
				ITreeTableSummaryArraySource summaryArraySource = GetSummaryArraySource();
				if (summaryArraySource != null)
					summaryArraySource.InvalidateSummariesTopDown();
				summaries = null;
			}
		}

	}

	/// <summary>
	/// A balanced tree with <see cref="TreeTableWithSummaryEntry"/> entries.
	/// </summary>
	public class TreeTableWithSummary : TreeTable
	{
        /// <summary>
        /// Initializes a new <see cref="TreeTableWithSummary"/>.
        /// </summary>
        /// <param name="sorted"></param>
		public TreeTableWithSummary(bool sorted)
			: base(sorted)
		{
		}

        /// <summary>
        /// Marks all summaries dirty.
        /// </summary>
        /// <param name="notifySummariesSource">if set to <c>true</c> notify summaries source.</param>
		public void InvalidateSummariesTopDown(bool notifySummariesSource)
		{
			if (Root != null)
				((ITreeTableSummaryNode) this.Root).InvalidateSummariesTopDown(notifySummariesSource);
		}

		/// <summary>
		/// Indicates whether the tree has summaries.
		/// </summary>
		public bool HasSummaries
		{
			get
			{
				if (this.Root == null)
					return false;

				return ((ITreeTableSummaryNode) this.Root).HasSummaries;
			}
		}

		/// <summary>
		/// Returns an array of summary objects.
		/// </summary>
		public ITreeTableSummary[] GetSummaries(ITreeTableEmptySummaryArraySource emptySummaries)
		{
			if (this.Root == null)
				return emptySummaries.GetEmptySummaries();

			return ((ITreeTableSummaryNode) this.Root).GetSummaries(emptySummaries);
		}


		/// <summary>
		/// Gets / sets a TreeTableWithSummaryEntry.
		/// </summary>
		public new TreeTableWithSummaryEntry this[int index]
		{
			get
			{
				return (TreeTableWithSummaryEntry) base[index];
			}
			set
			{
				base[index] = value;
			}
		}


		/// <summary>
		/// Inserts a <see cref="TreeTableWithSummaryEntry"/> object at the specified index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		public void Insert(int index, TreeTableWithSummaryEntry value)
		{
			base.Insert(index, value);
		}

		/// <summary>
		/// Removes an object from the tree.
		/// </summary>
		/// <param name="value"></param>
		public bool Remove(TreeTableWithSummaryEntry value)
		{
			return base.Remove(value);
		}

		/// <summary>
		/// Indicates whether an object belongs to the tree.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool Contains(TreeTableWithSummaryEntry value)
		{
			if (value == null)
				return false;

			return base.Contains(value);
		}

		/// <summary>
		/// Returns the index of an object in the tree.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int IndexOf(TreeTableWithSummaryEntry value)
		{
			return base.IndexOf(value);
		}

		/// <summary>
		/// Appends an object.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int Add(TreeTableWithSummaryEntry value)
		{
			return base.Add(value);
		}

		/// <summary>
		/// Copies the elements of this tree to an array.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public void CopyTo(TreeTableWithSummaryEntry[] array, int index)
		{
			base.CopyTo((ITreeTableNode[]) array, index);
		}

		/// <summary>
		/// Returns a strongly typed enumerator.
		/// </summary>
		/// <returns></returns>
		public new TreeTableWithSummaryEnumerator GetEnumerator()
		{
			return new TreeTableWithSummaryEnumerator(this);
		}
	}

	/// <summary>
	/// A strongly typed enumerator for the <see cref="TreeTableWithSummary"/> collection.
	/// </summary>
	public class TreeTableWithSummaryEnumerator : TreeTableEnumerator
	{
		/// <summary>
		/// Initializes a new <see cref="TreeTableWithSummaryEnumerator"/>.
		/// </summary>
		/// <param name="tree"></param>
		public TreeTableWithSummaryEnumerator(TreeTable tree)
			: base(tree)
		{
		}

		/// <summary>
		/// Returns the current <see cref="TreeTableWithSummary"/> object.
		/// </summary>
		public new TreeTableWithSummaryEntry Current
		{
			get
			{
				return (TreeTableWithSummaryEntry) base.Current;
			}
		}
	}


}