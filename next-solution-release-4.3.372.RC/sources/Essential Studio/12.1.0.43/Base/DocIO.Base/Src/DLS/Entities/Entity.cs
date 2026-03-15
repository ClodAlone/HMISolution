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

#region file using directives
using System;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// The base class for DLS entities.
    /// </summary>
    public abstract class Entity
      : XDLSSerializableBase
      , IEntity
    {
        #region Properties
        /// <summary>
        /// Gets owner of this entity.
        /// </summary>
        /// <value></value>
        public Entity Owner
        {
            get
            {
                return (Entity)OwnerBase;
            }
        }
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>The type of the entity.</value>
        public abstract EntityType EntityType
        {
            get;
        }
        /// <summary>
        /// Gets the next sibling.
        /// </summary>
        /// <value>The next sibling.</value>
        public IEntity NextSibling
        {
            get
            {
                ICompositeEntity ce = Owner as ICompositeEntity;
                return ce != null ? ce.ChildEntities.NextSibling(this) : (Owner is SDTInlineContent) ? (Owner as SDTInlineContent).ParagraphItems.NextSibling(this) : null;
            }
        }
        /// <summary>
        /// Gets the previous sibling.
        /// </summary>
        /// <value>The previous sibling.</value>
        public IEntity PreviousSibling
        {
            get
            {
                ICompositeEntity ce = Owner as ICompositeEntity;
                return ce != null ? ce.ChildEntities.PreviousSibling(this) : (Owner is SDTInlineContent) ? (Owner as SDTInlineContent).ParagraphItems.PreviousSibling(this) : null;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is composite.
        /// </summary>
        /// <value>
        /// 	If this instance is composite, set to <c>true</c>.
        /// </value>
        public bool IsComposite
        {
            get
            {
                return (this is ICompositeEntity);
            }
        }
        /// <summary>
        /// Gets a value indicating whether this <see cref="OwnerHolder"/> is detached.
        /// </summary>
        /// <value>if detached, set to <c>true</c>.</value>
        internal bool DeepDetached
        {
            get
            {
                if (EntityType == EntityType.WordDocument)
                {
                    return false;
                }
                else
                {
                    if (Owner != null)
                    {
                        return Owner.DeepDetached;
                    }

                    return true;
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="owner">The owner.</param>
        protected Entity(WordDocument doc, Entity owner)
            : base(doc, owner)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Creates a duplicate of the entity.
        /// </summary>
        /// <returns></returns>
        public Entity Clone()
        {
            return (Entity)CloneImpl();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds the self.
        /// </summary>
        internal virtual void AddSelf()
        {
        }
        /// <summary>
        /// Clones the relations.
        /// </summary>
        internal virtual void CloneCommit()
        {
            ICompositeEntity composite = this as ICompositeEntity;

            if (composite != null)
            {
                EntityCollection children = composite.ChildEntities;

                for (int i = 0, len = children.Count; i < len; i++)
                {
                    Entity en = children[i];
                    en.CloneCommit();

                    if (children.Count < len)
                    {
                        i--;
                        len--;
                    }
                }
            }
        }
        /// <summary>
        /// Removes the self.
        /// </summary>
        internal virtual void RemoveSelf()
        {
            ICompositeEntity composite = Owner as ICompositeEntity;

            if (composite != null)
            {
                composite.ChildEntities.Remove(this);
            }
        }
        /// <summary>
        /// Gets the index in parent collection.
        /// </summary>
        /// <returns></returns>
        internal int GetIndexInOwnerCollection()
        {
            ICompositeEntity composite = Owner as ICompositeEntity;

            if (composite != null)
            {
                return composite.ChildEntities.IndexOf(this);
            }

            return -1;
        }
        /// <summary>
        /// Determines whether is parent of the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// 	If it is parent of the specified entity, set to <c>true</c>.
        /// </returns>
        internal bool IsParentOf(Entity entity)
        {
            bool isParent = false;
            OwnerHolder oh = entity.OwnerBase;

            while (oh != null)
            {
                if (oh == this)
                {
                    isParent = true;
                    break;
                }

                oh = oh.OwnerBase;
            }

            return isParent;
        }
        #endregion
    }
}