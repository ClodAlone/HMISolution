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

using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Collections;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Abstract class that implements the basic functionality of the data binding.
    /// </summary>
    public abstract class ChartBaseDataBindList
    {
        #region Members
        private object m_dataSource;
        private string m_dataMember;
        private BindingContext m_bindingContext;

        /// <summary>
        /// The <see cref="CurrencyManager"/>.
        /// </summary>
        protected CurrencyManager m_dataManager = null;

        /// <summary>
        /// The properties collection.
        /// </summary>
        protected PropertyDescriptorCollection m_properties = null;
        #endregion

        #region Events
        /// <summary>
        /// This event is raised when the data is changed.
        /// </summary>
        public event ListChangedEventHandler Changed;
        #endregion

        #region Propeties
        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        /// <value>The data source.</value>
        [DefaultValue(null)]
        public object DataSource
        {
            get
            {
                return m_dataSource; 
            }

            set
            {
                if (m_dataSource != value)
                {
                    m_dataSource = value;
                    this.Reset();
                }
            }
        }

        /// <summary>
        /// Gets or sets the data member.
        /// </summary>
        /// <value>The data member.</value>
        [DefaultValue("")]
        public string DataMember
        {
            get
            {
                return m_dataMember; 
            }

            set
            {
                if (m_dataMember != value)
                {
                    m_dataMember = value;
                    this.Reset();
                }
            }
        }

        /// <summary>
        /// Gets or sets the binding context.
        /// </summary>
        /// <value>The binding context.</value>
        public BindingContext BindingContext
        {
            get 
            {
                return m_bindingContext; 
            }

            set
            {
                if (m_bindingContext != value)
                {
                    m_bindingContext = value;
                    this.Reset();
                }
            }
        }
        
        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public virtual int Count
        {
            get
            {
                return m_dataManager != null ? m_dataManager.Count : 0;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartBaseDataBindList"/> class.
        /// </summary>
        public ChartBaseDataBindList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartBaseDataBindList"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="dataMember">The data member.</param>
        public ChartBaseDataBindList(object dataSource, string dataMember)
        {
            m_dataSource = dataSource;
            m_dataMember = dataMember;
            this.Reset();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartBaseDataBindList"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="dataMember">The data member.</param>
        /// <param name="bindingContext">The binding context.</param>
        public ChartBaseDataBindList(object dataSource, string dataMember, BindingContext bindingContext)
        {
            m_dataSource = dataSource;
            m_dataMember = dataMember;
            m_bindingContext = bindingContext;
            this.Reset();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Refresh binding.
        /// </summary>
        public void DataBind()
        {
            this.OnListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Resets this instance.
        /// </summary>
        protected virtual void Reset()
        {
            if (m_dataSource != null)
            {
                if (m_bindingContext == null)
                {
                    m_bindingContext = new BindingContext();
                }

                CurrencyManager manager = m_bindingContext[m_dataSource, m_dataMember] as CurrencyManager;

                if (m_dataManager != manager)
                {
                    if (m_dataManager != null)
                    {
#if SyncfusionFramework2_0
                        m_dataManager.ListChanged -= new ListChangedEventHandler(OnListChanged);
#else
						if(m_dataManager.List is IBindingList)
						{			
							(m_dataManager.List as IBindingList).ListChanged -= new ListChangedEventHandler(OnListChanged);
						}
#endif
                    }

                    m_dataManager = manager;

                    if (m_dataManager != null)
                    {
                        m_properties = m_dataManager.GetItemProperties();

#if SyncfusionFramework2_0
                        m_dataManager.ListChanged += new ListChangedEventHandler(OnListChanged);
#else
						if(m_dataManager.List is IBindingList)
						{			
							(m_dataManager.List as IBindingList).ListChanged += new ListChangedEventHandler(OnListChanged);
						}
#endif
                    }
                }
            }
            else
            {
                if (m_dataManager != null)
                {
#if SyncfusionFramework2_0
                    m_dataManager.ListChanged -= new ListChangedEventHandler(OnListChanged);
#else
					if(m_dataManager.List is IBindingList)
					{			
						(m_dataManager.List as IBindingList).ListChanged -= new ListChangedEventHandler(OnListChanged);
					}
#endif
                }

                m_dataManager = null;
                m_properties = null;
            }
        }

        /// <summary>
        /// Resets the properties.
        /// </summary>
        private void ResetProperties()
        {
            if (m_dataManager != null)
            {
                m_properties = m_dataManager.GetItemProperties();
            }
        }

        /// <summary>
        /// Called when list is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.ComponentModel.ListChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnListChanged(object sender, ListChangedEventArgs args)
        {
            if (args.ListChangedType == ListChangedType.Reset)
            {
                this.Reset();
                this.RaiseChanged(args);
            }
            else if (args.ListChangedType == ListChangedType.PropertyDescriptorAdded
                || args.ListChangedType == ListChangedType.PropertyDescriptorDeleted
                || args.ListChangedType == ListChangedType.PropertyDescriptorChanged)
            {
                this.Reset();
                this.ResetProperties();
            }
            else
            {
                this.RaiseChanged(args);
            }
        }

        /// <summary>
        /// Raises the Changed event.
        /// </summary>
        /// <param name="args">The <see cref="System.ComponentModel.ListChangedEventArgs"/> instance containing the event data.</param>
        protected void RaiseChanged(ListChangedEventArgs args)
        {
            if (this.Changed != null)
            {
                this.Changed(this, args);
            }
        }
        #endregion
    }
}
