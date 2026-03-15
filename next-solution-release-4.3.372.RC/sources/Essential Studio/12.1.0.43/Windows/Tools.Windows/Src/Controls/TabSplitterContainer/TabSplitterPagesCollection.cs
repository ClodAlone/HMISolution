#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Collections;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Collection containing <see cref="TabSplitterContainer"/>'s primary or secondary nested items.
    /// </summary>
    [Editor(typeof(Design.TabSplitterPagesCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public class TabSplitterPagesCollection : CollectionBase
    {
        #region Construction

        internal TabSplitterPagesCollection(TabSplitterContainer owner)
        {
            m_owner = owner;
        }

        #endregion

        #region Properties
  
        public TabSplitterPage this[int index]
        {
            get
            { 
                return List[index] as TabSplitterPage;
            }
            set
            { 
                List[index] = value;
            }
        }

        public TabSplitterContainer Owner
        {
            get
            { 
                return m_owner; 
            }
        }

        /// <summary>
        /// Gets or sets the zero-based index of the currently selected page in collection
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedIndex
        {
            get
            {
                return m_selectedIndex;
            }
            set
            {
                if (m_selectedIndex != value)
                {
                    Select(value);
                }
            }
        }

        private int ActiveIndex
        {
            get
            {
                if (m_activeIndex < 0)
                {
                    if (this.Count > 0)
                    {
                        return 0;
                    }
                }
                return m_activeIndex;
            }
        }
        #endregion Properties

        #region Methods
   
        public int Add(TabSplitterPage value)
        {
            if (!List.Contains(value))
            {
                return List.Add(value);
            }
            return -1;
        }

        public void AddRange(TabSplitterPage[] values)
        {
            this.Owner.SuspendLayout();

            foreach (TabSplitterPage value in values)
            {
                this.Add(value);
            }

            this.Owner.ResumeLayout(true);
        }

        public int IndexOf(TabSplitterPage value)
        {
            return List.IndexOf(value);
        }

        public void Insert(int index, TabSplitterPage value)
        {
            if (!List.Contains(value))
            {
                List.Insert(index, value);
            }
        }
        
        public void Remove(TabSplitterPage value)
        {
            if (List.Contains(value))
            {
                List.Remove(value);
            }
        }

        public bool Contains(TabSplitterPage value)
        {
            return List.Contains(value);
        }

        internal void Select(int value)
        {
            if (value >= -1 && value < this.Count)
            {
                m_owner.SuspendLayout();

                if (m_selectedIndex >= 0 && m_selectedIndex < this.Count)
                {
                    TabSplitterPage page = this[m_selectedIndex];

                    page.Visible = false;
                }

                m_selectedIndex = value;

                if (m_selectedIndex >= 0 && m_selectedIndex < this.Count)
                {
                    TabSplitterPage page = this[m_selectedIndex];

                    page.BringToFront();
                    if(!page.Hide)
                    page.Visible = true;

                    m_activeIndex = m_selectedIndex;
                }

                m_owner.ResumeLayout();

                OnSelectedIndexChanged();
            }
        }
  
        internal void Select(TabSplitterPage page)
        {
            this.SelectedIndex = IndexOf(page);
        }

        internal bool Deselect(TabSplitterPage page, bool bCollapsed)
        {
            if (bCollapsed)
            {
                return m_owner.PrimaryPagesInternal.Deselect(page, false) || m_owner.SecondaryPagesInternal.Deselect(page, false);
            }
            else
            {
                for (int i = 0; i < this.InnerList.Count; i++)
                {
                    if (this.InnerList[i] != page)
                    {
                        Select(i);
                        return true;
                    }
                }
            }
            return false;
        }

        internal void SelectActiveItem()
        {
            this.SelectedIndex = this.ActiveIndex;
        }

        #endregion

        #region Overrides

        protected override void OnClear()
        {
            base.OnClear();

            m_owner.SuspendLayout();

            foreach (TabSplitterPage page in this.InnerList)
            {
                page.SetOwner(null);
                page.Parent = null;
            }

            m_owner.ResumeLayout();
        }
   
        protected override void OnClearComplete()
        {
            base.OnClearComplete();

            if (Deselect(null, m_owner.Collapsed))
            {
                this.SelectedIndex = -1;
            }

            m_owner.OnPagesChanged();
        }

        protected override void OnInsertComplete(int index, object value)
        {
            base.OnInsertComplete(index, value);

            TabSplitterPage page = value as TabSplitterPage;
            if (page != null)
            {
                m_owner.SuspendLayout();

                page.Visible = false;

                page.SetOwner(this);
                page.Parent = m_owner;

                if (m_selectedIndex < 0)
                {
                    if (!m_owner.Collapsed || (m_owner.PrimaryPages.SelectedIndex < 0 && m_owner.SecondaryPages.SelectedIndex < 0))
                    {
                        m_selectedIndex = index;
                        m_activeIndex = m_selectedIndex;

                        page.Visible = true;
                    }
                }
                else if (index <= m_selectedIndex)
                {
                    m_selectedIndex++;
                    m_activeIndex = m_selectedIndex;
                }

                m_owner.ResumeLayout();
                m_owner.OnPagesChanged();
            }
        }
    
        protected override void OnRemove(int index, object value)
        {
            base.OnRemove(index, value);

            if (!m_owner.Disposing)
            {
                if (index == m_selectedIndex)
                {
                    if (!Deselect(value as TabSplitterPage, m_owner.Collapsed))
                    {
                        this.SelectedIndex = -1;
                    }
                }
            }
        }
 
        protected override void OnRemoveComplete(int index, object value)
        {
            base.OnRemoveComplete(index, value);

            if (!m_owner.Disposing)
            {
                TabSplitterPage page = value as TabSplitterPage;
                if (page != null)
                {
                    m_owner.SuspendLayout();

                    page.SetOwner(null);
                    page.Parent = null;

                    if (index < m_selectedIndex)
                    {
                        m_selectedIndex--;
                        m_activeIndex = m_selectedIndex;
                    }

                    m_owner.ResumeLayout();

                    m_owner.OnPagesChanged();
                }
            }
        }
   
        protected override void OnSetComplete(int index, object oldValue, object newValue)
        {
            base.OnSetComplete(index, oldValue, newValue);

            m_owner.SuspendLayout();

            TabSplitterPage oldPanel = oldValue as TabSplitterPage;
            if (oldPanel != null)
            {
                oldPanel.SetOwner(null);
                oldPanel.Parent = null;
            }

            TabSplitterPage newPanel = newValue as TabSplitterPage;
            if (newPanel != null)
            {
                newPanel.Visible = index == m_selectedIndex;

                newPanel.SetOwner(this);
                newPanel.Parent = this.Owner;
            }

            m_owner.ResumeLayout();

            m_owner.OnPagesChanged();
        }
        #endregion

        #region Implementation

        private void OnSelectedIndexChanged()
        {
            m_owner.OnSelectedIndexChanged(this);

            if (this.SelectedIndexChanged != null)
            {
                this.SelectedIndexChanged(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Raised when selected page was changed.
        /// </summary>
        public event EventHandler SelectedIndexChanged;

        #endregion Events

        #region Fields

       private TabSplitterContainer m_owner;

        /// <summary>
        /// Selected page's index.
        /// </summary>
        private int m_selectedIndex = -1;

        private int m_activeIndex = -1;
        #endregion Data
    }
}
