#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents collection of actions.
    /// </summary>
    /// <seealso cref="PdfCollection"/> Class.
    public class PdfActionCollection : PdfCollection
    {
        #region Fields
        /// <summary>
        /// Array of actions.
        /// </summary>
        private PdfArray m_actions = new PdfArray();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.Pdf.Interactive.PdfAction"/> at the specified index.
        /// </summary>
        /// <value></value>
        PdfAction this[int index]
        {
            get
            {
                return (PdfAction)List[index];
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>action</returns>
        public int Add(PdfAction action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            return DoAdd(action);
        }

        /// <summary>
        /// Inserts the action at the specified position.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="action">The action.</param>
        public void Insert(int index, PdfAction action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            DoInsert(index, action);
        }

        /// <summary>
        /// Gets the index of the action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>action</returns>
        public int IndexOf(PdfAction action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            return List.IndexOf(action);
        }

        /// <summary>
        /// Determines whether the action is contained within collection.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>
        /// Value, indicating the presents of the action in collection.
        /// </returns>
        public bool Contains(PdfAction action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            return List.Contains(action);
        }

        /// <summary>
        /// Clears this collection.
        /// </summary>
        public void Clear()
        {
            DoClear();
        }

        /// <summary>
        /// Removes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        public void Remove(PdfAction action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            DoRemove(action);
        }

        /// <summary>
        /// Removes the action at the specified position.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            DoRemoveAt(index);
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfActionCollection"/> class.
        /// </summary>
        public PdfActionCollection()
            : base()
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds the action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>Index of the inserted action.</returns>
        private int DoAdd(PdfAction action)
        {
            m_actions.Add(new PdfReferenceHolder(action));
            return List.Add(action);
        }

        /// <summary>
        /// Inserts the action.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="action">The action.</param>
        private void DoInsert(int index, PdfAction action)
        {
            m_actions.Insert(index, new PdfReferenceHolder(action));
            List.Insert(index, action);
        }

        /// <summary>
        /// Clear the collection.
        /// </summary>
        private void DoClear()
        {
            m_actions.Clear();
            List.Clear();
        }

        /// <summary>
        /// Removes the action.
        /// </summary>
        /// <param name="action">The action.</param>
        private void DoRemove(PdfAction action)
        {
            int index = List.IndexOf(action);
            m_actions.RemoveAt(index);
            List.Remove(action);
        }

        /// <summary>
        /// Removes the action at the specified position.
        /// </summary>
        /// <param name="index">The index.</param>
        private void DoRemoveAt(int index)
        {
            m_actions.RemoveAt(index);
            List.RemoveAt(index);
        }
        #endregion
    }
}
