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
using System.Collections;
using Syncfusion.DocIO.DLS.XML;
using System.Collections.Generic;
#if WINRT
using Syncfusion.DocIO.WinrtHelper;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a collection of DLS entities.
    /// </summary>
    public abstract class EntityCollection :
#if !SILVERLIGHT && !WP
      XDLSSerializableCollection
#else
      CollectionImpl
      , IEnumerable
#endif
      , IEntityCollectionBase
    {
        #region Fields
        internal ChangeItemsHandlerList ChangeItemsHandlers = new ChangeItemsHandlerList();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.Entity"/> at the specified index.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        public Entity this[int index]
        {
            get
            {
                return InnerList[index] as Entity;
            }
        }
        /// <summary>
        /// Gets the first item.
        /// </summary>
        /// <value>The first item.</value>
        public Entity FirstItem
        {
            get
            {
                return Count > 0 ? this[0] : null;
            }
        }
        /// <summary>
        /// Gets the last item.
        /// </summary>
        /// <value>The last item.</value>
        public Entity LastItem
        {
            get
            {
                return Count > 0 ? this[Count - 1] : null;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this <see cref="EntityCollection"/> is joined.
        /// </summary>
        /// <value>If it is joined, set to <c>true</c>.</value>
        internal bool Joined
        {
            get
            {
                return OwnerBase != null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal Entity Owner
        {
            get
            {
                return (Entity)OwnerBase;
            }
        }
        protected abstract Type[] TypesOfElement
        {
            get;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollection"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal EntityCollection(WordDocument doc)
            : this(doc, null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollection"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="owner">The owner.</param>
        internal EntityCollection(WordDocument doc, Entity owner)
            : base(doc, owner)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public int Add(IEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            int index = Count;
            OnInsert(index, (Entity)entity);
            index = Count;
            index = OnInsertField(index, (Entity)entity);
            InnerList.Add(entity);
            OnInsertComplete(index, (Entity)entity);
            OnAddFieldComplete(index, (Entity)entity);
            return index;
        }
        /// <summary>
        /// Removes all items
        /// </summary>
        public void Clear()
        {
            OnClear();
            InnerList.Clear();
        }
        /// <summary>
        /// Determines whether a entity is in the collection.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public bool Contains(IEntity entity)
        {
            return InnerList.Contains(entity);
        }
        /// <summary>
        /// Returns the zero-based index of the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public int IndexOf(IEntity entity)
        {
            return InnerList.IndexOf(entity);
        }
        /// <summary>
        /// Inserts a entity into the collection at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="entity">The entity.</param>
        public void Insert(int index, IEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            OnInsert(index, (Entity)entity);
            index = OnInsertField(index, (Entity)entity);
            InnerList.Insert(index, entity);
            OnInsertComplete(index, (Entity)entity);
            OnInsertFieldComplete(index, (Entity)entity);
        }
        /// <summary>
        /// Removes the entity from the collection.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Remove(IEntity entity)
        {
            UpdateBookmarksCollection(entity);
            OnRemove(IndexOf(entity));
            InnerList.Remove(entity);
        }
        /// <summary>
        /// Removes the entity at the specified index from the collection.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            UpdateBookmarksCollection(this.InnerList[index] as IEntity);
            OnRemove(index);
            InnerList.RemoveAt(index);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Updates the bookmarks collection.
        /// </summary>
        /// <param name="entity">The entity.</param>
        private void UpdateBookmarksCollection(IEntity entity)
        {
            if (entity is WParagraph)
            {
                foreach (ParagraphItem item in (entity as WParagraph).Items)
                {
                    if (item is BookmarkStart)
                    {
                        Bookmark bkmk = this.Document.Bookmarks.FindByName((item as BookmarkStart).Name);

                        if (bkmk != null)
                        {
                            if (bkmk.BookmarkStart != null)
                                bkmk.BookmarkStart.m_isDetached = true;
                            if (bkmk.BookmarkEnd != null)
                                bkmk.BookmarkEnd.m_isDetached = true;
                            this.Document.Bookmarks.InnerList.Remove(bkmk);
                        }
                    }
                    else if (item is WTextBox)
                    {
                        foreach (TextBodyItem bodyItem in (item as WTextBox).TextBoxBody.Items)
                        {
                            UpdateBookmarksCollection(bodyItem);
                        }
                    }
                }
            }
            else if (entity is WTable)
            {
                foreach (WTableRow row in (entity as WTable).Rows)
                {
                    foreach (WTableCell cell in row.Cells)
                    {
                        foreach (TextBodyItem item in (cell as WTextBody).Items)
                        {
                            UpdateBookmarksCollection(item);
                        }
                    }
                }
            }
            else if (entity is BookmarkStart)
            {
                Bookmark bkmk = this.Document.Bookmarks.FindByName((entity as BookmarkStart).Name);

                if (bkmk != null)
                {
                    if (bkmk.BookmarkStart != null)
                        bkmk.BookmarkStart.m_isDetached = true;
                    if (bkmk.BookmarkEnd != null)
                        bkmk.BookmarkEnd.m_isDetached = true;
                    this.Document.Bookmarks.InnerList.Remove(bkmk);
                }
            }
        }
        /// <summary>
        /// Gets Next sibling entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        internal Entity NextSibling(Entity entity)
        {
            int index = IndexOf(entity);

            if (index < 0 || index > Count - 2)
            {
                return null;
            }

            return this[index + 1];
        }
        /// <summary>
        /// Gets Previous sibling entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        internal Entity PreviousSibling(Entity entity)
        {
            int index = IndexOf(entity);

            if (index < 1 || index > Count - 1)
            {
                return null;
            }

            return this[index - 1];
        }
        /// <summary>
        /// Gets the index of the next or previous element.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="type">The type.</param>
        /// <param name="next">if it specifies next element, set to <c>true</c>.</param>
        /// <returns></returns>
        internal int GetNextOrPrevIndex(int index, EntityType type, bool next)
        {
            while (true)
            {
                index += (next) ? 1 : -1;

                if (index > InnerList.Count - 1 || index < 0)
                {
                    return -1;
                }
                else
                {
                    Entity en = InnerList[index] as Entity;

                    if (en.EntityType == type)
                        break;
                }
            }

            return index;
        }
        /// <summary>
        /// Removes items of specified type
        /// </summary>
        /// <param name="type">The type.</param>
        internal void InternalClearBy(EntityType type)
        {
            for (int i = 0, length = Count; i < length; i++)
            {
                Entity en = this[i];

                if (en.EntityType == type)
                {
                    en.SetOwner(null);
                    InnerList.RemoveAt(i);
                    i--;
                }
            }
        }
        /// <summary>
        /// Clones all items to destination collection.
        /// </summary>
        /// <param name="destColl">The destination collection.</param>
        internal void CloneTo(EntityCollection destColl)
        {
            for (int i = 0, cnt = Count; i < cnt; i++)
            {
                destColl.Add(this[i].Clone());
            }
        }
        /// <summary>
        /// Called when all items removes.
        /// </summary>
        protected virtual void OnClear()
        {
            for (int i = 0, length = Count; i < length; i++)
            {
                this[i].SetOwner(null);
            }

            ChangeItemsHandlers.Send(ChangeItemsType.Clear, null);
        }
        /// <summary>
        /// Called when entity inserts.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="entity">The entity.</param>
        protected virtual void OnInsert(int index, Entity entity)
        {
            if (!IsCorrectElementType(entity))
            {
                string exMessage = string.Format("Cannot insert an object of type {0} into the {1}",
                  entity.EntityType, Owner.EntityType);
                throw new ArgumentException(exMessage);
            }

            if (Joined)
            {
                bool isDetachedEntity = (entity.Owner == null);
                bool isDeepDetachedCollection = Owner.DeepDetached;
                WordDocument srcDoc = entity.Document;

                if (Document != srcDoc)
                {
                    if (!isDetachedEntity)
                        throw new InvalidOperationException("You can not add no clonned entity from other document");

                    entity.CloneRelationsTo(Document, Owner);
                }

                if (!isDetachedEntity)
                {
                    entity.RemoveSelf();
                }
                else if (!Document.IsOpening && !Document.IsCloning && Document == srcDoc)
                    entity.AddSelf();

                entity.SetOwner(Owner);
            }

            ChangeItemsHandlers.Send(ChangeItemsType.Add, entity);
        }

        /// <summary>
        /// Called when entity inserted.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="entity">The entity.</param>
        protected virtual void OnInsertComplete(int index, Entity entity)
        {
            if (Joined)
            {
                if (!Owner.DeepDetached)
                {
                    if (!(entity.Owner is WParagraph
                        && (entity is BookmarkStart || entity is BookmarkEnd)))
                        entity.CloneCommit();
                }
            }
        }
        /// <summary>
        /// Called when entity removes.
        /// </summary>
        /// <param name="index">The index.</param>
        protected virtual void OnRemove(int index)
        {
            Entity en = this[index];
            en.SetOwner(null);
            ChangeItemsHandlers.Send(ChangeItemsType.Remove, en);
        }
        /// <summary>
        /// Determines whether specified element type is correct.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// 	If the element type is correct, set to <c>true</c>.
        /// </returns>
        private bool IsCorrectElementType(Entity entity)
        {
            
            bool correctElType = false;
            
            foreach (Type enType in TypesOfElement)
            {
                correctElType = enType.IsInstanceOfType(entity);
                if (correctElType)
                    break;
            }

            return correctElType;
        }
        /// <summary>
        /// Called when [insert field].
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        private int OnInsertField(int index, Entity entity)
        {
            if (m_doc != null && !m_doc.IsOpening)
            {
                if (entity is WFormField)
                    index = OnInsertFormField(index, (Entity)entity);
                else if (entity is WField
                    && (entity as WField).FieldEnd != null)
                    Document.ClonedFields.Push(entity as WField);
            }
            return index;
        }
        /// <summary>
        /// Called when [insert field complete].
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="entity">The entity.</param>
        private void OnInsertFieldComplete(int index, Entity entity)
        {
            if (m_doc != null && !m_doc.IsOpening)
            {
                if (entity is WFormField)
                    OnInsertFormFieldComplete(index, (Entity)entity);
                else if (entity is WFieldMark
                    && Document.ClonedFields.Count > 0)
                {
                    WField field = Document.ClonedFields.Peek();
                    if ((entity as WFieldMark).Type == FieldMarkType.FieldSeparator)
                        field.FieldSeparator = entity as WFieldMark;
                    else
                    {
                        field = Document.ClonedFields.Pop();
                        field.FieldEnd = entity as WFieldMark;
                    }
                }
            }
        }
        /// <summary>
        /// Called when [add field complete].
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="entity">The entity.</param>
        private void OnAddFieldComplete(int index, Entity entity)
        {
            if (m_doc != null && !m_doc.IsOpening)
            {
                if (entity is WFormField)
                    OnAddFormFieldComplete(index, (Entity)entity);
                else if (entity is WFieldMark
                    && Document.ClonedFields.Count > 0)
                {
                    WField field = Document.ClonedFields.Peek();
                    if ((entity as WFieldMark).Type == FieldMarkType.FieldSeparator)
                        field.FieldSeparator = entity as WFieldMark;
                    else
                    {
                        field = Document.ClonedFields.Pop();
                        field.FieldEnd = entity as WFieldMark;
                    }
                }
            }
        }
        /// <summary>
        /// Called when form field inserts - Performs insertion of bookmark start for the corresponding form field.
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="entity">Entity</param>
        /// <returns>Updated index</returns>
        private int OnInsertFormField(int index, Entity entity)
        {
            switch ((entity as WFormField).FormFieldType)
            {
                case FormFieldType.CheckBox:
                    {
                        WCheckBox checkBox = entity as WCheckBox;
                        if (checkBox.Name == null || checkBox.Name == string.Empty)
                        {
                            string titleName = "Check_" + (Guid.NewGuid().ToString()).Replace("-", "_");
                            checkBox.Name = titleName.Substring(0, 20);
                        }
                    }
                    break;
                case FormFieldType.DropDown:
                    {
                        WDropDownFormField dropDownField = entity as WDropDownFormField;
                        if (dropDownField.Name == null || dropDownField.Name == string.Empty)
                        {
                            string titleName = "Drop_" + (Guid.NewGuid().ToString()).Replace("-", "_");
                            dropDownField.Name = titleName.Substring(0, 20);
                        }
                    }
                    break;
                case FormFieldType.TextInput:
                    {
                        WTextFormField textFormField = entity as WTextFormField;
                        if (textFormField.Name == null || textFormField.Name == string.Empty)
                        {
                            string titleName = "Text_" + (Guid.NewGuid().ToString()).Replace("-", "_");
                            textFormField.Name = titleName.Substring(0, 20);
                        }
                        if (textFormField.DefaultText == null || textFormField.DefaultText == string.Empty)
                        {
                            textFormField.DefaultText = WTextFormField.DEF_TEXT;
                        }
                    }
                    break;
            }
            //Skip adding form field bookmark, if bookmark is copied from the source.
            if ((Owner as WParagraph).Items.Count > 0 && (((Owner as WParagraph).LastItem is BookmarkStart
                && ((Owner as WParagraph).LastItem as BookmarkStart).Name == (entity as WFormField).Name)
                || (index < (Owner as WParagraph).Items.Count && index > 0
                && (Owner as WParagraph).Items[index - 1] is BookmarkStart
                && ((Owner as WParagraph).Items[index - 1] as BookmarkStart).Name == (entity as WFormField).Name)))
                return index;
            (Owner as WParagraph).CheckFormFieldName((entity as WFormField).Name);
            (Owner as WParagraph).Items.Insert(index, new BookmarkStart(Document, (entity as WFormField).Name));
            index++;                
            return index;
        }
        /// <summary>
        /// Called after form field inserted - Inserts bookmark end for the corresponding form fields.
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="entity">Entity</param>
        private void OnInsertFormFieldComplete(int index, Entity entity)
        {
            if (entity is WTextFormField)
            {
                (Owner as WParagraph).Items.Insert(++index, new WFieldMark(m_doc, FieldMarkType.FieldSeparator));
                (Owner as WParagraph).Items.Insert(++index, new WFieldMark(m_doc, FieldMarkType.FieldEnd));
                (Owner as WParagraph).Items.Insert(++index, new BookmarkEnd(Document, (entity as WFormField).Name));
            }
            else
            {
                (Owner as WParagraph).Items.Insert(++index, new WFieldMark(m_doc, FieldMarkType.FieldEnd));
                (Owner as WParagraph).Items.Insert(++index, new BookmarkEnd(Document, (entity as WFormField).Name));
            }                
        }
        /// <summary>
        /// Called after form field Added - Adds bookmark end for the corresponding form fields.
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="entity">Entity</param>
        private void OnAddFormFieldComplete(int index, Entity entity)
        {
            WFieldMark fieldSep = new WFieldMark(m_doc, FieldMarkType.FieldSeparator);
            WFieldMark fieldEnd = new WFieldMark(m_doc, FieldMarkType.FieldEnd);
            if (entity is WTextFormField)
            {
                (entity as WTextFormField).FieldSeparator = fieldSep;
                (Owner as WParagraph).Items.Add(fieldSep);
                if ((entity as WTextFormField).TextRange.Owner == null)
                {
                    (Owner as WParagraph).Items.Add((entity as WTextFormField).TextRange);
                    (entity as WTextFormField).Range.Items.Add((entity as WTextFormField).TextRange);
                }
                (entity as WTextFormField).FieldEnd = fieldEnd;
                (Owner as WParagraph).Items.Add(fieldEnd);
                (entity as WTextFormField).Range.Items.Add(fieldEnd);
                (Owner as WParagraph).Items.Add(new BookmarkEnd(Document, (entity as WFormField).Name));
            }
            else
            {
                (entity as WFormField).FieldEnd = fieldEnd;
                (Owner as WParagraph).Items.Add(fieldEnd);
                (Owner as WParagraph).Items.Add(new BookmarkEnd(Document, (entity as WFormField).Name));
            }
        }
        #endregion

        #region Internal declaration
        /// <summary>
        /// Specifies Item Entity type.
        /// </summary>
        public enum ChangeItemsType
        {
            /// <summary>
            /// Add entity type.
            /// </summary>
            Add,
            /// <summary>
            /// Remove Entity type.
            /// </summary>
            Remove,
            /// <summary>
            /// Clear Entity type.
            /// </summary>
            Clear
        }
        /// <summary>
        /// Eventhandler for EntityCollection class.
        /// </summary>
        /// <param name="type">Entity type.</param>
        /// <param name="entity">The Entity.</param>
        public delegate void ChangeItems(ChangeItemsType type, Entity entity);
        /// <summary>
        /// 
        /// </summary>
        internal class ChangeItemsHandlerList : IEnumerable
        {
            #region Fields
            private List<ChangeItems> m_list = new List<ChangeItems>();
            #endregion

            #region Public methods
            /// <summary>
            /// Adds the specified handler.
            /// </summary>
            /// <param name="handler">The handler.</param>
            public void Add(ChangeItems handler)
            {
                if (m_list.Contains(handler))
                    throw new ArgumentException("handler already exists");

                m_list.Add(handler);
            }
            /// <summary>
            /// Removes the specified handler.
            /// </summary>
            /// <param name="handler">The handler.</param>
            public void Remove(ChangeItems handler)
            {
                if (!m_list.Contains(handler))
                    throw new ArgumentException("handler not exists");

                m_list.Remove(handler);
            }
            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>
            /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
            /// </returns>
            public IEnumerator GetEnumerator()
            {
                return m_list.GetEnumerator();
            }
            /// <summary>
            /// Sends the specified type.
            /// </summary>
            /// <param name="type">The type.</param>
            /// <param name="entity">The entity.</param>
            public void Send(ChangeItemsType type, Entity entity)
            {
                foreach (ChangeItems handler in m_list)
                {
                    handler.DynamicInvoke(new object[] { type, entity });
                }
            }
            #endregion
        }
        #endregion
    }
}
