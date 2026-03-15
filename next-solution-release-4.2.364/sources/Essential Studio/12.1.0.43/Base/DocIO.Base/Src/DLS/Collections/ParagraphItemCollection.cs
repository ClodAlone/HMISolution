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
#if !SILVERLIGHT && !WP
using Syncfusion.Layouting;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a collection for <see cref="Syncfusion.DocIO.DLS.WParagraph"/> child items.
    /// </summary>
    public class ParagraphItemCollection : EntityCollection
    {
        #region Class constants
        private static readonly System.Type[] DEF_ELEMENT_TYPES = new System.Type[1] { typeof(ParagraphItem) };
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.ParagraphItem"/> at the specified index.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        new public ParagraphItem this[int index]
        {
            get
            {
                return InnerList[index] as ParagraphItem;
            }
        }
        /// <summary>
        /// Gets the owner paragraph.
        /// </summary>
        /// <value>The owner paragraph.</value>
        protected WParagraph OwnerParagraph
        {
            get
            {
                return Owner as WParagraph;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected override Type[] TypesOfElement
        {
            get
            {
                return DEF_ELEMENT_TYPES;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ParagraphItemCollection"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        public ParagraphItemCollection(WordDocument doc)
            : base(doc)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ParagraphItemCollection"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal ParagraphItemCollection(WParagraph owner)
            : base(owner.Document, owner)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ParagraphItemCollection"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal ParagraphItemCollection(SDTInlineContent owner)
            : base(owner.Document, owner)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Clones the items to new collection.
        /// </summary>
        /// <param name="items">The items.</param>
        internal void CloneItemsTo(ParagraphItemCollection items)
        {
            for (int i = 0, cnt = Count; i < cnt; i++)
            {
                ParagraphItem item = (ParagraphItem)this[i].Clone();
                if (item != null)
                {
                    item.SetOwner(items.Owner);
                    items.UnsafeAdd(item);
                }
            }
        }
        /// <summary>
        /// Unsafe method for removes item at index.
        /// </summary>
        /// <param name="index">The index.</param>
        internal void UnsafeRemoveAt(int index)
        {
            InnerList.RemoveAt(index);
        }
        /// <summary>
        /// Unsafe method for adds the item.
        /// </summary>
        /// <param name="item">The item.</param>
        internal void UnsafeAdd(ParagraphItem item)
        {
            InnerList.Add(item);
            if (Document != null)
            {
                if (item is WField
                    && (item as WField).FieldEnd != null)
                    Document.ClonedFields.Push(item as WField);
                else if (item is WFieldMark
                    && Document.ClonedFields.Count > 0)
                {
                    WField field = Document.ClonedFields.Peek();
                    if ((item as WFieldMark).Type == FieldMarkType.FieldSeparator)
                        field.FieldSeparator = item as WFieldMark;
                    else
                    {
                        field = Document.ClonedFields.Pop();
                        field.FieldEnd = item as WFieldMark;
                    }
                }
            }
        }
        /// <summary>
        /// Called when entity inserted.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="entity">The entity.</param>
        protected override void OnInsertComplete(int index, Entity entity)
        {
            base.OnInsertComplete(index, entity);

            if (Joined && entity.Owner != null && OwnerParagraph != null )
            {
                ParagraphItem item = (ParagraphItem)entity;
                int itemPos = 0;

                if (index > 0)
                {
                    itemPos = this[index - 1].EndPos;
                }

                item.Attach(OwnerParagraph, itemPos);
            }
            else if (Joined && entity.Owner is WMergeField)
            {
                (entity as ParagraphItem).ParaItemCharFormat.ApplyBase((entity.Owner as WMergeField).CharacterFormat.BaseFormat);
            }
        }
        /// <summary>
        /// Called when entity removes.
        /// </summary>
        /// <param name="index">The index.</param>
        protected override void OnRemove(int index)
        {
            Entity ent = this[index] as Entity;
            if (Joined)
            {
                this[index].Detach();
            }
            base.OnRemove(IndexOf(ent));
        }
        /// <summary>
        /// Called when all items removes.
        /// </summary>
        protected override void OnClear()
        {
            if (Joined)
            {
                for (int i = 0; i < Count; i++)
                {
                    this[i].Detach();
                }
            }

            base.OnClear();
        }

        #endregion
#if !SILVERLIGHT && !WP
        #region Implementation / xml
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override OwnerHolder CreateItem(IXDLSContentReader reader)
        {
            Enum itemType;
            bool parseOk = reader.ParseElementType(typeof(ParagraphItemType), out itemType);

            if (!parseOk)
                itemType = ParagraphItemType.TextRange;

            ParagraphItem item = (ParagraphItem)Document.CreateParagraphItem((ParagraphItemType)itemType);
            return item;
        }
        /// <summary>
        /// Gets name of xml tag
        /// </summary>
        /// <value></value>
        protected override string GetTagItemName()
        {
            return XDLSConstants.ItemTag;
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Get Current widget
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal IWidget GetCurrentWidget(int index)
        {
            return InnerList[index] as IWidget;
        }
#endif
        #endregion
#endif
    }
}