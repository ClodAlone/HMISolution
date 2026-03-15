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
using System.ComponentModel;

namespace Syncfusion.Windows.Controls.Grid
{
    #region multi column sorting support
    /// <summary>
    /// Holds information like property name and sort direction for a sorted column in a GridTreeControl.
    /// </summary>
    public class SortState : IComparable
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SortState()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="property">The mappingname for the sorted column.</param>
        /// <param name="direction">The ListSortDirection of teh sorted column.</param>
        public SortState(string property, ListSortDirection direction)
        {
            this.direction = direction;
            this.property = property;
        }

        string property = "";

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        public string Property
        {
            get { return property; }
            set { property = value; }
        }
        ListSortDirection direction = ListSortDirection.Ascending;

        /// <summary>
        /// Gets or sets the ListSortDirection for the column.
        /// </summary>
        public ListSortDirection Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        private static SortState StringToSortState(string sortString)
        {
            SortState state = new SortState();
            int loc = sortString.LastIndexOf(' ') + 1;
            if (loc > 0 && (sortString.Substring(loc).ToLower() == "asc"
                            || sortString.Substring(loc).ToLower() == "desc"))
            {
                state.Property = sortString.Substring(0, loc - 1);
                state.Direction = sortString.Substring(loc).ToLower() == "asc" ? ListSortDirection.Ascending : ListSortDirection.Descending;
            }
            else
            {
                state.Property = sortString;
            }
            return state;
        }

        /// <summary>
        /// Takes a comma separated string holding one or more sorted column names along with their sort direction and returns 
        /// a collection of SortState objects matching the contents of the string.
        /// </summary>
        /// <param name="sortString">A string holding one or more column names followed by ASC or DESC separated by commas.</param>
        /// <returns>A list of SortState objects.</returns>
        /// <remarks>
        /// One example of a valid string would be "States Desc, Cities ASC, Salaries DESC". This string reflects a sorted GridTreeControl
        /// that is first sorted on States in descending order, then on Cities in ascending order, and finally on Salaries in descending
        /// order.
        /// </remarks>
        public static List<SortState> GetSortStatesFromString(string sortString)
        {
            List<SortState> sortStates = new List<SortState>();
            if (sortString.Length > 0)
            {
                string[] strings = sortString.Split(new char[] { ',' });
                foreach (string s in strings)
                {
                    sortStates.Add(StringToSortState(s));
                }
            }
            return sortStates;
        }

        /// <summary>
        /// Takes a property name and ListSortDirection objct and returns a string consisting of the property name followed by a space followed 
        /// by either ASC or DESC depending upon the ListSortDirection.
        /// </summary>
        /// <param name="property">The property name.</param>
        /// <param name="dir">The ListSortDirection.</param>
        /// <returns>The string consisting of the property name followed by a space followed 
        /// by either ASC or DESC.</returns>
        public static string GetCompositeString(string property, ListSortDirection dir)
        {
            return string.Format("{0} {1}", property, dir == ListSortDirection.Ascending ? "ASC" : "DESC");
        }

        /// <summary>
        /// Takes a collection of SortStates and returns a string the represent the collection.
        /// </summary>
        /// <param name="states">The SortStates collection.</param>
        /// <returns>A comma separated string holding one or more sorted column names along with their sort direction
        /// as determined by the content of the collection.</returns>
        public static string GetSortStringFromStates(List<SortState> states)
        {
            string s = "";
            foreach (SortState state in states)
            {
                if (s.Length > 0)
                {
                    s += ",";
                }
                s += state.ToString();
            }
            return s;
        }

        /// <exclude />
        ///<summary>
        ///Overridden to indicate 2 SortStates are equal if their Property values are equal.
        ///</summary>
        public override bool Equals(object obj)
        {
            return this.property == ((SortState)obj).property;
        }

        /// <exclude />
        ///<summary>
        ///Overridden to indicate 2 SortStates are equal if their Property values are equal.
        ///</summary>
        public override int GetHashCode()
        {
            return this.property.GetHashCode();
        }

        /// <exclude />
        public override string ToString()
        {
            return GetCompositeString(property, direction);
        }

        #region IComparable Members
        /// <exclude />
        ///<summary>
        ///Overridden to compare SortStates using their Property values.
        ///</summary>
        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            return this.property.CompareTo(((SortState)obj).Property);
        }

        #endregion
    }

    #endregion

    #region Node Comparer
    /// <exclude/>
    /// <summary>
    /// Handles comparing grid nodes for sorting.
    /// </summary>
    class NodeComparer : IComparer<GridTreeNode>
    {
        GridTreeControlImpl grid = null;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="grid">The GridTreeControl.</param>
        public NodeComparer(GridTreeControlImpl grid)
        {
            this.grid = grid;
        }
        #region IComparer<GridNode> Members

        /// <exclude/>
        /// <summary>
        /// The IComparer implement.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(GridTreeNode x, GridTreeNode y)
        {
            int c = 0;
            foreach (SortState state in grid.SortStates)
            {
                c = Compare(x, y, state);
                if (c != 0)
                    break;
                else
                    continue;
            }

            return c;
        }

        /// <exclude/>
        /// <summary>
        /// The IComparer implementation.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(GridTreeNode x, GridTreeNode y, SortState state)
        {
            IComparable xc = grid.GetValueFromNode(state.Property, x) as IComparable;
            IComparable yc = grid.GetValueFromNode(state.Property, y) as IComparable;
            int c = 0;
            if (xc != null && yc == null)
            {
                c = 1;
            }
            else if (xc == null && yc != null)
            {
                c = -1;
            }
            else if (xc != null && yc != null)
            {
                c = xc.CompareTo(yc);
                if (c == 0 && grid.SortStates.IndexOf(state) == grid.SortStates.Count - 1)
                {
                    c = x.GetHashCode().CompareTo(y.GetHashCode());
                }

                //if (c == 0 && grid.SortStates.IndexOf(state) == grid.SortStates.Count - 1)
                //{
                //    // c = x.GetHashCode().CompareTo(y.GetHashCode());//Previous code
                //    //Previously we have check the hashcode code of two TreeNode If both are same.
                //    //Hash code doesnt show the correct value.
                //    //Now we have change the implementation as follows. If both nodes are same then we have Check the child node.
                //    if (x.HasChildNodes && !y.HasChildNodes)
                //    {
                //        c = -1;
                //    }
                //    else if (!x.HasChildNodes && y.HasChildNodes)
                //    {
                //        c = 1;
                //    }
                //    else
                //    {

                //        for (int i = 0; i < x.ChildNodes.Count; ++i)
                //        {
                //            if (i == y.ChildNodes.Count)
                //            {
                //                c = -1;
                //                break;
                //            }
                //            xc = grid.GetValueFromNode(state.Property, x.ChildNodes[i]) as IComparable;
                //            yc = grid.GetValueFromNode(state.Property, y.ChildNodes[i]) as IComparable;
                //            if (xc != null && yc == null)
                //            {
                //                c = 1;
                //            }
                //            else if (xc == null && yc != null)
                //            {
                //                c = -1;
                //            }
                //            else if (xc != null && yc != null)
                //            {
                //                c = xc.CompareTo(yc);
                //            }
                //        }
                //        if (c == 0)
                //        {
                //            c = x.ChildNodes.Count.CompareTo(y.ChildNodes.Count);
                //        }
                //        if (c == 0)
                //        {
                //            for (int i = 0; i < x.ChildNodes.Count && c == 0; i++)
                //            {
                //                c = Compare(x.ChildNodes[i], y.ChildNodes[i], state);
                //            }
                //        }
                //    }
                //}
            }

            if (state.Direction == ListSortDirection.Descending)
                c = -c;
            return c;
        }

        #endregion
    }

    #endregion

}
