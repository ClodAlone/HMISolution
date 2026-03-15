//-------------------------------------------------------------------------------------------------
// <copyright file="Axis.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Syncfusion.Olap.Data
{
    /// <summary>
    /// The Axis function uses the zero-based position of an axis to return the set of tuples 
    /// on an axis. For example, Axis(0) returns the COLUMNS axis, Axis(1) returns the ROWS 
    /// axis, and so on. The Axis function cannot be used on the filter axis. This function can 
    /// be used to make calculated members aware of the context of the query that is being 
    /// run. For example, you might need a calculated member that provides the sum of only 
    /// those members selected on the Rows axis. It can also be used to make the definition 
    /// of one axis dependent on the definition of another. For example, by ordering the contents 
    /// of the Rows axis according to the value of the first item on the Columns axis.
    /// </summary>
    /// <remarks>
    /// An Axis is created in Cellset object when ExecuteCellSet method is invoked from 
    /// AdomdProvider.
    /// </remarks>
    [Serializable]
    public class Axis : IDisposable
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Axis"/> class.
        /// </summary>
        public Axis()
        {
            this.TupleSet = new TupleCollection(this);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Axis name.
        /// </summary>
        /// <value>The Axis name.</value>
        [Description("Gets or sets the Axis name."), DefaultValue("")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the Tuple Collection
        /// </summary>
        /// <value>The tuple set.</value>
        [Description("Gets the Tuple Collection."), DefaultValue((string)null)]
        public TupleCollection TupleSet { get; private set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (Tuple tuple in this.TupleSet)
            {
                tuple.Members.Clear();
                tuple.parentAxis = null;
            }

            this.TupleSet.Clear();
        }

        /// <summary>
        /// Normalizes drilleddown property of the equal cells if they differ.
        /// </summary>
        public void NormalizeExpandedState()
        {

        }
        internal void UpdateMemberDrillState(Member member, int i, int j, Tuple row)
        {
            if (member.HasChildMembers && !member.DrilledDown)
            {
                if (this.TupleSet.MaxLevel[j] == member.LevelDepth)
                    return;

                for (int rowindex = i + 1; rowindex < this.TupleSet.Count; rowindex++)
                {
                    var compareMember = this.TupleSet[rowindex].Members[j];

                    if (member.LevelDepth != compareMember.LevelDepth)
                    {
                        if (string.Equals(member.UniqueName, compareMember.ParentUniqueName))
                        {
                            for (int index = i; index < rowindex; index++)
                                this.TupleSet[index].Members[j].DrilledDown = true;
                        }
                        break;
                    }
                }
            }
        }
        #endregion
    }
}
