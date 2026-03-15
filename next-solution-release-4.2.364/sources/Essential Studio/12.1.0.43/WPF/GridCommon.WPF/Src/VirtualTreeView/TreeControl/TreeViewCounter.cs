#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Windows.Collections;

namespace Syncfusion.Windows.Controls.VirtualTreeView
{
    /// <summary>
    /// The counter kind.
    /// </summary>
    class TreeViewCounterKind
    {
        /// <summary>
        /// Both lines and height.
        /// </summary>
        public const int CountAll = 0xffff;
        /// <summary>
        /// Line count.
        /// </summary>
        public const int Lines = 0x0001;
        /// <summary>
        /// Height in pixels.
        /// </summary>
        public const int Height = 0x0002;
    }

    /// <summary>
    /// A counter for counting number of visible rows and the cumulated height 
    /// of nodes in pixels.
    /// </summary>
    class TreeViewCounter : ITreeTableCounter
    {
        double height;
        int rows;

        /// <summary>
        /// Returns an empty TreeViewCounter that represents zero visible elements.
        /// </summary>
        public static readonly TreeViewCounter Empty = new TreeViewCounter(0, 0);

        /// <summary>
        /// Initializes a <see cref="TreeViewCounter"/> with a specified number of visible elements.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="distance">The distance.</param>
        public TreeViewCounter(int rows, double distance)
        {
            this.rows = rows;
            this.height = distance;
        }

        /// <summary>
        /// Returns the visible count.
        /// </summary>
        public int Rows
        {
            get
            {
                return rows;
            }
        }

        /// <summary>
        /// Gets the cumulated height in pixels.
        /// </summary>
        /// <value>The height.</value>
        public double Height
        {
            get
            {
                return height;
            }
        }

        /// <summary>
        /// Returns the integer value of the counter. A cookie specifies
        /// a specific counter type.
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        public double GetValue(int kind)
        {
            if (kind == TreeViewCounterKind.Height)
                return height;
            return rows;
        }


        /// <summary>
        /// Combines one tree obkect with another and returns the new object.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <param name="cookie">The cookie.</param>
        /// <returns></returns>
        ITreeTableCounter ITreeTableCounter.Combine(ITreeTableCounter other, int cookie)
        {
            return Combine((TreeViewCounter)other, cookie);
        }

        /// <summary>
        /// Combines the counter values of this counter object with the values of another counter object
        /// and returns a new counter object.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <param name="cookie">The cookie.</param>
        /// <returns></returns>
        public TreeViewCounter Combine(TreeViewCounter other, int cookie)
        {
            if (other == null || other.IsEmpty(int.MaxValue))
                return this;

            if (this.IsEmpty(int.MaxValue))
                return other;

            return new TreeViewCounter(rows + other.rows, height + other.height);
        }

        double ITreeTableCounter.Compare(ITreeTableCounter other, int cookie)
        {
            return Compare((TreeViewCounter)other, cookie);
        }

        /// <summary>
        /// Compares this counter with another counter. A cookie can specify
        /// a specific counter type.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <param name="cookie">The cookie.</param>
        /// <returns></returns>
        public double Compare(TreeViewCounter other, int cookie)
        {
            if (other == null)
                return 0;

            int treeNode = 0;

            if ((cookie & TreeViewCounterKind.Height) != 0)
                treeNode = Math.Sign(height - other.height);

            if (treeNode == 0 && (cookie & TreeViewCounterKind.Lines) != 0)
                treeNode = rows - other.rows;

            return treeNode;
        }

        /// <summary>
        /// Indicates whether the counter object is empty. A cookie can specify
        /// a specific counter type.
        /// </summary>
        /// <param name="cookie">The cookie.</param>
        /// <returns>
        /// 	<c>true</c> if the counted value for the specified cookie is empty; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEmpty(int cookie)
        {
            return Compare(Empty, cookie) == 0;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents the current <see cref="System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return String.Concat("Rows = ", this.Rows.ToString(), ", Height = ", this.Height.ToString());
        }


        #region ITreeTableCounter Members


        public int Kind
        {
            get { return 0; }
        }

        #endregion
    }

}
