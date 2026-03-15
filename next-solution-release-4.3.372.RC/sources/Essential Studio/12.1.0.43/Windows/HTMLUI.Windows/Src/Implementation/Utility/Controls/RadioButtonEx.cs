#region Copyright Syncfusion Inc. 2001 - 2014
////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.HTMLUI;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;

#endregion

namespace Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility
{
    /// <summary>
    /// Class which holds an instance of the RadioControlImpl class.
    /// </summary>
    [ToolboxItem(false)]
    internal class RadioButtonEx : RadioButton
    {
        #region Class members
        /// <summary>
        /// Parent control's class instance.
        /// </summary>
        private RADIOControlImpl m_parent;

        /// <summary>
        /// Name of the group of the control.
        /// </summary>
        private string m_groupName;
        #endregion

        #region Class Properties
        /// <summary>
        /// Gets or sets the parent class instance.
        /// </summary>
        public RADIOControlImpl ParentElement
        {
            get
            {
                return m_parent;
            }
            set
            {
                m_parent = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the group of the control.
        /// </summary>
        public string GroupName
        {
            get
            {
                return m_groupName;
            }
            set
            {
                m_groupName = value;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the RadioButtonEx class
        /// </summary>
        public RadioButtonEx()
            : base()
        {
            this.CheckAlign = ContentAlignment.MiddleCenter;
            this.Text = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the RadioButtonEx class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="groupName">Name of the group.</param>
        public RadioButtonEx(RADIOControlImpl parent, string groupName)
            : this()
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            if (groupName == null)
                throw new ArgumentNullException("groupName");

            m_parent = parent;
            m_groupName = groupName;
        }
        #endregion

        #region Custom drawing
        /// <summary>
        /// Overridden. Raised to repaint the control.
        /// </summary>
        /// <param name="pevent">Painting arguments.</param>
        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            if (this.Focused)
            {
                ControlPaint.DrawFocusRectangle(pevent.Graphics, this.ClientRectangle);
            }
        }
        #endregion
    }
}
