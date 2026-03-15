#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Class containing Update Info.
    /// </summary>
    public class UpdateInfo
    {
        #region Class members
        public System.Drawing.Rectangle m_rectUpdate;
        #endregion

        #region Class utility methods
        /// <summary>
        /// Union the given update rectangle with exist refresh rectangle.
        /// </summary>
        /// <param name="rectUpdating">The update rectangle in client coordinates.</param>
        public void UpdateRefreshRect(System.Drawing.Rectangle rectUpdating)
        {
            if (m_rectUpdate.Size.IsEmpty)
            {
                m_rectUpdate = rectUpdating;
            }
            else if (!rectUpdating.Size.IsEmpty)
            {
                m_rectUpdate = System.Drawing.Rectangle.Union(m_rectUpdate, rectUpdating);
            }
            m_rectUpdate = System.Drawing.Rectangle.Inflate(m_rectUpdate, 20, 20);
        }
        #endregion
    }
}
