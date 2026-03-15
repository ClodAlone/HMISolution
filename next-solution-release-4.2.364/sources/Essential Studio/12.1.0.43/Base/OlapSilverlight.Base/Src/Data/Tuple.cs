#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Data
{
    /// <summary>
    /// Represents the Tuple information.
    /// </summary>
    [KnownType(typeof(Member))]
    [DataContract]
    public class Tuple
    {
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
        [DataMember]
        public MemberCollection Members { get;  set; }

        /// <summary>
        /// Gets or sets the ordinal position.
        /// </summary>
        /// <value>The ordinal position.</value>
        [DataMember]
        public int OrdinalPosition { get;  set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Tuple"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool Visible { get; set; }
        #endregion
    }
}
