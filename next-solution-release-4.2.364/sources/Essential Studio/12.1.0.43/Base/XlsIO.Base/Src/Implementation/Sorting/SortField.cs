#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if ( WINRT )
using Windows.UI;
#endif


#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;

#endif 
namespace Syncfusion.XlsIO.Implementation.Sorting
{
    /// <summary>
    /// Represents the sort Field attributes.
    /// </summary>
    class SortField: ISortField
    {
        #region Members
        /// <summary>
        /// Represents the column to be sorted on.
        /// </summary>
        private int m_iKey;
        /// <summary>
        /// Represents the sort by in the range.
        /// </summary>
        private SortOn m_sortOn;
        /// <summary>
        /// Represents the sort order.
        /// </summary>
        OrderBy m_Order;
        /// <summary>
        /// Represents the color to sort. Throws exception when SortOn type is Values.
        /// </summary>
        Color m_color;
        /// <summary>
        /// Parent class object.
        /// </summary>
        SortFields m_parent;
        #endregion

        #region Properties
        /// <summary>
        /// Represents the column to be sorted on.
        /// </summary>
        public int Key
        {
            get
            {
                return m_iKey;
            }
            set
            {
                m_iKey = value;
            }
        }
        /// <summary>
        /// Represents the sort by in the range.
        /// </summary>
       public SortOn SortOn
        {
            get
            {
                return m_sortOn;
            }
            set
            {
                m_sortOn = value;
            }
        }

        /// <summary>
        /// Represents the sort order.
        /// </summary>
        public OrderBy Order
        {
            get
            {
                return m_Order;
            }
            set
            {
                m_Order = value;
            }
        }
        /// <summary>
        /// Represents the color to sort. Throws exception when SortOn type is Values.
        /// </summary>
        public Color Color
        {
            get
            {
                return m_color;

            }
            set
            {
                m_color = value;
            }
        }
        #endregion

        #region intialization
        /// <summary>
        /// Initialize the sort field.
        /// </summary>
        /// <param name="parent"></param>
        public SortField(SortFields parent)
        {
            m_parent = parent;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Sets sorting priority.
        /// </summary>
        /// <param name="priority">integer priority value. 0 represents high priority.</param>
        public void SetPriority(int priority)
        {
            m_parent.SetPriority(this, priority);
        }
        #endregion
    }
}
