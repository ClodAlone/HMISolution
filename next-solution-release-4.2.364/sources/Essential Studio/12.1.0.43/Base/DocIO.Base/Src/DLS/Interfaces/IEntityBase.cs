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

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a base entity for other entities of DLS.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Gets document of this entity.
        /// </summary>
        WordDocument Document
        {
            get;
        }
        /// <summary>
        /// Gets owner of this entity.
        /// </summary>
        Entity Owner
        {
            get;
        }
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>The type of the entity.</value>
        EntityType EntityType
        {
            get;
        }
        /// <summary>
        /// Gets the next sibling.
        /// </summary>
        /// <value>The next sibling.</value>
        IEntity NextSibling
        {
            get;
        }
        /// <summary>
        /// Gets the previous sibling.
        /// </summary>
        /// <value>The previous sibling.</value>
        IEntity PreviousSibling
        {
            get;
        }
        /// <summary>
        /// Gets a value indicating whether this instance is composite.
        /// </summary>
        /// <value>
        /// 	if this instance is composite, set to <c>true</c>.
        /// </value>
        bool IsComposite
        {
            get;
        }
        /// <summary>
        /// Creates a duplicate of the entity.
        /// </summary>
        /// <returns></returns>
        Entity Clone();
    }
}