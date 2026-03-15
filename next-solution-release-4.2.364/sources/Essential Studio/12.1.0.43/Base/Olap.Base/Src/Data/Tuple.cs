//-------------------------------------------------------------------------------------------------
// <copyright file="Tuple.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System.ComponentModel;

namespace Syncfusion.Olap.Data
{
    /// <summary>
    /// Represents an ordered collection of members from different hierarchies. 
    /// </summary>
    /// <remarks>
    /// A tuple consists of an ordered collection of members.
    /// <para>
    /// A tuple cannot contain more than one member from any single hierarchy.
    /// </para>
    /// <para>
    /// In ADOMD.NET, a Tuple is used to represent a tuple by encapsulating the collection of Member 
    /// objects that define the tuple. Tuple objects can be referenced only through the Tuples property of a CellSet.
    /// </para>
    /// </remarks>
    public class Tuple
    {
        #region Internal Variables
        internal Axis parentAxis;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Tuple"/> class.
        /// </summary>
        public Tuple()
        {
            this.Members = new MemberCollection();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the members.
        /// </summary>
        /// <value>The members.</value>
        [DefaultValue((string)null), Description("Gets a tuple member collection.")]
        public MemberCollection Members { get; internal set; }

        /// <summary>
        /// Gets or sets the ordinal position.
        /// </summary>
        /// <value>The ordinal position.</value>
        [DefaultValue(0), Description("Gets an ordinal position of the tuple.")]
        public int OrdinalPosition { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Tuple"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        [Description("Gets or sets a flag that indicates if the Tuple in the result CellSet is visible."), DefaultValue(true)]
        public bool Visible { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            string name = string.Empty;
            foreach (Member member in Members)
            {
                if (name.Length > 0)
                {
                    name += ".";
                }

                name += member.ToString();
            }

            return name;
        }

        /// <summary>
        /// Toes the unique string.
        /// </summary>
        /// <returns></returns>
        public string ToUniqueString()
        {
            string name = string.Empty;
            foreach (Member member in Members)
            {
                if (name.Length > 0)
                {
                    name += ".";
                }

                name += member.UniqueName;
            }

            return name;
        }
        #endregion
    }
}
