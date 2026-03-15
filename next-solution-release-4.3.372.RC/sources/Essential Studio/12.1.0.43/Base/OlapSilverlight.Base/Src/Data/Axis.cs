#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Syncfusion.OlapSilverlight.Data
{
    /// <summary>
    /// Represents the Axis information.
    /// </summary>
    [KnownType(typeof(Tuple))]
    [DataContract]
    public class Axis
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Axis"/> class.
        /// </summary>
        public Axis()
        {
            this.TupleSet = new TupleCollection();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Axis name.
        /// </summary>
        /// <value>The Axis name.</value>
        [Description("Gets or sets the Axis name."), DefaultValue("")]
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets the Tuple Collection
        /// </summary>
        /// <value>The tuple set.</value>
      
        [Description("Gets the Tuple Collection."), DefaultValue((string)null)]
        [DataMember]
        public TupleCollection TupleSet { get;  set; }

        [DataMember]
        public List<int> MaxLevel { get; set; }
        /// <summary>
        /// Determines the Min Level of the Current Member in the TupleSet
        /// </summary>
        [DataMember]
        public  List<int> MinLevel { get; set; }
        /// <summary>
        /// Determines whether Min Level has been already set or not.  Returns true if already set or else returns false.
        /// </summary>
        [DataMember]
        public List<bool> valueSet { get; set; }

        #endregion

        /// <summary>
        /// Normalizes drilled down property of the equal cells if the differ.
        /// </summary>
        internal void NormalizeExpandedState()
        {

        }
        internal void UpdateMemberDrillState(Member member, int i, int j, Tuple row)
        {
            if (member.HasChildMembers && !member.DrilledDown)
            {
                if (this.MaxLevel[j] == member.LevelDepth)
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
    }
}
