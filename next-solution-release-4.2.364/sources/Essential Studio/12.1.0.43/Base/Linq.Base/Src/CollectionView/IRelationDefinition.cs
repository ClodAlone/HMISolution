#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    /// <summary>
    /// Specifies the relation type.
    /// </summary>
    public enum RelationType
    {
        /// <summary>
        /// Specifies Mater-Detail kind of relation.
        /// </summary>
        MasterDetails,
        /// <summary>
        /// Specifies Foreign Key reference relation.
        /// </summary>
        ForeignKeyReference
    }

    /// <summary>
    /// Specifies the relation definition for the <see cref="ICollectionViewAdv"/>.
    /// </summary>
    public interface IRelationDefinition
    {
        /// <summary>
        /// Gets or sets the relational column.
        /// </summary>
        /// <value>The relational column.</value>
        string RelationalColumn { get; set; }


        /// <summary>
        /// Gets or sets the type of the relation.
        /// </summary>
        /// <value>The type of the relation.</value>
        RelationType RelationType { get; set; }        
    }
}
