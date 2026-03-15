#region Copyright Syncfusion Inc. 2001 - 2014
//// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.HTMLUI;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility
{
    /// <summary>
    /// Class corresponding to elements focused in the document.
    /// </summary>
    internal class FocusManager
    {
        #region Class members
        /// <summary>
        /// Comparer of elements in the collection.
        /// </summary>
        private TabStopComparer m_comparer;

        /// <summary>
        /// Index of the currently focused element in the collection.
        /// </summary>
        private int m_tabIndex = -1;

        /// <summary>
        /// Array of elements with TabIndex greater than or equal to zero.
        /// </summary>
        private ArrayList m_elements;

        /// <summary>
        /// Indicates whether focusing of an element is in process.
        /// </summary>
        private bool m_focusProcessing;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the comparer for the elements sorted by their Tab order.
        /// </summary>
        public TabStopComparer Comparer
        {
            get
            {
                if (m_comparer == null)
                {
                    m_comparer = new TabStopComparer();
                }

                return m_comparer;
            }
        }

        /// <summary>
        /// Gets the list of elements with TabIndex equal to or greater than zero.
        /// </summary>
        private ArrayList Elements
        {
            get
            {
                if (m_elements == null)
                {
                    m_elements = new ArrayList();
                }

                return m_elements;
            }
        }

        /// <summary>
        /// Gets or sets the index of the currently focused element in the document.
        /// </summary>
        public int Index
        {
            get
            {
                return m_tabIndex;
            }
            set
            {
                if (m_tabIndex != value)
                {
                    m_tabIndex = value;
                }
            }
        }

        /// <summary>
        /// Gets the focused element in the document.
        /// </summary>
        public BaseElement FocusedElement
        {
            get
            {
                BaseElement result = null;

                if (!(m_elements == null || m_tabIndex < 0 ||
                  m_tabIndex > m_elements.Count - 1))
                {
                    result = m_elements[m_tabIndex] as BaseElement;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether focusing is in process.
        /// </summary>
        public bool IsFocusing
        {
            get
            {
                return m_focusProcessing;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the FocusManager class
        /// </summary>
        public FocusManager()
        {
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Pair method. Indicates beginning of the focusing process.
        /// </summary>
        /// <remarks>After the end of focusing, EndFocus method must be invoked.</remarks>
        public void BeginFocus()
        {
            m_focusProcessing = true;
        }

        /// <summary>
        /// Pair Method. Indicates the end of focusing process.
        /// </summary>
        public void EndFocus()
        {
            m_focusProcessing = false;
        }

        /// <summary>
        /// Sets the focus for an element in the document.
        /// </summary>
        /// <param name="step">Step of changing element.</param>
        /// <returns>True if element has been changed; False otherwise.</returns>
        /// <remarks>If step &gt; 0, forward order is used; 
        /// if step &lt; 0 - backward order is used. </remarks>
        public bool SetFocus(int step)
        {
            bool result;

            // There are no any element can be focused.
            if (m_elements == null)
            {
                result = false;
            }
            else
            {
                // Step can be less, equal and greater than zero.
                int currIndex = (step < 0 && m_tabIndex < 0) ?
                  m_elements.Count + step : m_tabIndex + step;

                if (currIndex >= 0 && currIndex < m_elements.Count)
                {
                    BaseElement elm = m_elements[currIndex] as BaseElement;

                    if (elm != null)
                    {
                        result = SetFocus(elm);

                        m_tabIndex = currIndex;
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Adds an element to the collection of elements getting focused.
        /// </summary>
        /// <param name="element">Element for adding to the collection.</param>
        public void Add(IHTMLElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (element.TabIndex >= 0 && !this.Elements.Contains(element))
            {
                this.Elements.Add(element);
            }

            this.Elements.Sort(this.Comparer);
        }

        /// <summary>
        /// Removes the element from the collection.
        /// </summary>
        /// <param name="element">Element for removing from the collection.</param>
        public void Remove(IHTMLElement element)
        {
            if (element != null && m_elements != null && m_elements.Contains(element))
            {
                m_elements.Remove(element);
                m_elements.Sort(this.Comparer);
            }
        }

        /// <summary>
        /// Resets the index of the focused element.
        /// </summary>
        public void Reset()
        {
            LostFocus();
            m_tabIndex = -1;
        }

        /// <summary>
        /// Overloaded. Draws focus rectangle on the active focused element.
        /// </summary>
        /// <param name="g">Graphics object.</param>
        public void DrawFocusRect(Graphics g)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            BaseElement elm = this.FocusedElement;

            if (elm != null && !m_focusProcessing)
            {
                DrawFocusRect(elm, g);
            }
        }

        /// <summary>
        /// Draws focus rectangle on the element.
        /// </summary>
        /// <param name="element">Element for focus rectangle drawing.</param>
        /// <param name="g">Graphics object.</param>
        public void DrawFocusRect(BaseElement element, Graphics g)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (g == null)
                throw new ArgumentNullException("g");

            Rectangle elmRect;

            if (element.IsBlock)
            {
                Block main = element.MainBlock;

                if (main != null && element.Control != null)
                {
                    elmRect = element.GlobalToClient(main.Rectangle);
                    ControlPaint.DrawFocusRectangle(g, elmRect);
                }
            }
            else if (element.Control != null)
            {
                Block curBlock;

                for (int i = 0, len = element.Blocks.Count; i < len; i++)
                {
                    curBlock = element.Blocks[i] as Block;

                    if (curBlock != null)
                    {
                        elmRect = element.GlobalToClient(curBlock.Rectangle);
                        ControlPaint.DrawFocusRectangle(g, elmRect);
                    }
                }
            }
        }

        /// <summary>
        /// Draws focus rectangle on the element.
        /// </summary>
        /// <param name="element">Element for focus rectangle drawing.</param>
        public void DrawFocusRect(BaseElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (element.Control != null)
            {
                using (Graphics g = element.Control.CreateGraphics())
                {
                    DrawFocusRect(element, g);
                }
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Sets the element as focused in the document.
        /// </summary>
        /// <param name="element">Element going to be focused.</param>
        /// <returns>True if element is set as focused; false otherwise.</returns>
        protected internal bool SetFocus(IHTMLElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            bool result = false;
            int elmIndex = m_elements.IndexOf(element);

            if (elmIndex != -1)
            {
                m_tabIndex = elmIndex;
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Clears focus rectangle of the previous element.
        /// </summary>
        protected internal void ClearPrevFocusRect()
        {
            BaseElement curElement = this.FocusedElement;

            if (curElement != null)
            {
                bool buff = m_focusProcessing;
                m_focusProcessing = true;

                curElement.Control.Refresh();

                m_focusProcessing = buff;
            }
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Sets focus to the element.
        /// </summary>
        /// <param name="element">Element getting focus.</param>
        /// <returns>True if element got focused; False otherwise.</returns>
        private bool SetFocus(BaseElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            bool result;

            // Element can't be focused.
            if (element.TabIndex < 0)
            {
                result = false;
            }
            else
            {
                LostFocus();
                result = element.Focus();
            }

            return result;
        }

        /// <summary>
        /// Invokes the Leave event on element which has focus at the current time.
        /// </summary>
        private void LostFocus()
        {
            BaseElement curElement = this.FocusedElement;

            if (curElement != null && curElement.TabIndex >= 0)
            {
                curElement.LeaveFocus();
            }
        }
        #endregion
    }
}
