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
using System.Collections;
using Syncfusion.DocIO.DLS.XML;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a collection of <see cref="Syncfusion.DocIO.DLS.Tab"/> objects 
    /// for paragraph or paragraph format.
    /// </summary>
    public class TabCollection :
#if !SILVERLIGHT && !WP
     XDLSSerializableCollection
#else
      CollectionImpl
      , IEnumerable
#endif
    {
        #region Members
        private bool m_cancelOnChange;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.Tab"/> at the specified index.
        /// </summary>
        /// <value></value>
        public Tab this[int index]
        {
            get
            {
                return (Tab)InnerList[index];
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether to cancel OnChange event.
        /// </summary>
        /// <value>
        /// 	If it specifies to cancel on change event, set to <c>true</c>.
        /// </value>
        internal bool CancelOnChangeEvent
        {
            get
            {
                return m_cancelOnChange;
            }
            set
            {
                m_cancelOnChange = value;
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TabCollection"/> class.
        /// </summary>
        internal TabCollection(WordDocument document)
            : base(document, null)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        /// <param name="owner"></param>
        internal TabCollection(WordDocument document, FormatBase owner)
            : this(document)
        {
            SetOwner(owner);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds the tab.
        /// </summary>
        /// <returns></returns>
        public Tab AddTab()
        {
            return AddTab(0, TabJustification.Left, TabLeader.NoLeader);
        }
        /// <summary>
        /// Adds the tab.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="justification">The justification.</param>
        /// <param name="leader">The leader.</param>
        /// <returns></returns>
        public Tab AddTab(float position, TabJustification justification, TabLeader leader)
        {
            Tab tab = new Tab(Document, position, justification, leader);
            InnerList.Add(tab);
            tab.SetOwner(this);
            OnChange();
            return tab;
        }
        /// <summary>
        /// Adds the tab.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public Tab AddTab(float position)
        {
            return AddTab(position, TabJustification.Left, TabLeader.NoLeader);
        }
        /// <summary>
        /// Removes all the tabs from the tab collection.
        /// </summary>
        public void Clear()
        {
            InnerList.Clear();
            OnChange();
        }
        /// <summary>
        /// Removes the tab at the specified index from the tab collection
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            InnerList.RemoveAt(index);
            OnChange();
        }
        /// <summary>
        /// Remove the tabs at the specified tab position from the tab collection.
        /// </summary>
        /// <param name="position"></param>
        public void RemoveByTabPosition(double position)
        {
            for(int i =0;i<this.Count;)
            {
                if (this[i].Position == position)
                {
                    InnerList.Remove(this[i]);
                    continue;
                }
                i++;
            }
            OnChange();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds the tab.
        /// </summary>
        /// <param name="tab">The tab.</param>
        internal void AddTab(Tab tab)
        {
            InnerList.Add(tab);
            tab.SetOwner(this);
            OnChange();
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Creates the item.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        protected override OwnerHolder CreateItem(IXDLSContentReader reader)
        {
            return new Tab(Document);
        }
        /// <summary>
        /// Gets name of xml tag
        /// </summary>
        /// <returns></returns>
        /// <value></value>
        protected override string GetTagItemName()
        {
            return XDLSConstants.TabTag;
        }
#endif
        /// <summary>
        /// 
        /// </summary>
        internal void OnChange()
        {
            if (m_cancelOnChange)
                return;

            if (OwnerBase != null && OwnerBase is WParagraphFormat)
            {
                (OwnerBase as WParagraphFormat).ChangeTabs(this);
            }
        }
        #endregion
    }
}
