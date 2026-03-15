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
    /// The base class with holder behaviour.
    /// </summary>
    public abstract class OwnerHolder
    {
        #region Fields
        protected WordDocument m_doc;
        private OwnerHolder m_owner;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <value>The document.</value>
        public WordDocument Document
        {
            get
            {
                return m_owner != null ? m_owner.Document : m_doc;
            }
        }
        /// <summary>
        /// Gets the owner.
        /// </summary>
        /// <value>The owner.</value>
        internal OwnerHolder OwnerBase
        {
            get
            {
                return m_owner;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerHolder"/> class.
        /// </summary>
        public OwnerHolder()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerHolder"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        public OwnerHolder(WordDocument doc)
            : this(doc, null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerHolder"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="owner">The owner.</param>
        public OwnerHolder(WordDocument doc, OwnerHolder owner)
        {
            m_doc = doc;
            m_owner = owner;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Sets the owner.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal void SetOwner(OwnerHolder owner)
        {
            m_owner = owner;

            if (owner != null)
            {
                m_doc = owner.Document;
            }
        }
        /// <summary>
        /// Sets the owner.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="owner">The owner.</param>
        internal void SetOwner(WordDocument doc, OwnerHolder owner)
        {
            //Sets owner holder.
            m_owner = owner;
            //Sets owner document.
            if (owner == null)
                m_doc = doc;
            else
                m_doc = owner.Document;
        }
        /// <summary>
        /// "Listener" for state change.
        /// </summary>
        internal virtual void OnStateChange(object sender)
        {
            if (m_owner != null)
                m_owner.OnStateChange(sender);
        }
        #endregion
    }
}
