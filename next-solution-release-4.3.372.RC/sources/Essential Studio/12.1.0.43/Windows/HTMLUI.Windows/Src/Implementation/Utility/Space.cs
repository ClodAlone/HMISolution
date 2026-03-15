#region Copyright Syncfusion Inc. 2001 - 2014
////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;

using Syncfusion.Windows.Forms.HTMLUI.Implementation;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Structure which contains space around the tag elements such as paddings, borders, etc...
    /// </summary>
    public struct Space
    {
        #region Class members
        /// <summary>
        /// Parent element for this structure.
        /// </summary>
        private BaseElement m_parent;
        #endregion

        #region Class Properties
        /// <summary>
        /// Gets the left indent space for the element.
        /// </summary>
        public int Left
        {
            get
            {
                int result = 0;

                if (m_parent.Format != null)
                {
                    HTMLFormat format = m_parent.Format as HTMLFormat;
                    result = format.Left.Width + format.PaddingLeft;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the top indent space for the element.
        /// </summary>
        public int Top
        {
            get
            {
                int result = 0;

                if (m_parent.Format != null)
                {
                    HTMLFormat format = m_parent.Format as HTMLFormat;
                    result = format.Top.Width + format.PaddingTop;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the right indent space for the element.
        /// </summary>
        public int Right
        {
            get
            {
                int result = 0;

                if (m_parent.Format != null)
                {
                    HTMLFormat format = m_parent.Format as HTMLFormat;
                    result = format.Right.Width + format.PaddingRight;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the bottom indent space for the element.
        /// </summary>
        public int Bottom
        {
            get
            {
                int result = 0;

                if (m_parent.Format != null)
                {
                    HTMLFormat format = m_parent.Format as HTMLFormat;
                    result = format.Bottom.Width + format.PaddingBottom;
                }

                return result;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the Space struct
        /// </summary>
        /// <param name="element">Parent element for this object.</param>
        public Space(BaseElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            m_parent = element;
        }
        #endregion
    }
}
