#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using Syncfusion.Windows.Forms.HTMLUI;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class which represents check box in the document.
    /// </summary>
    public class CHECKBOXControlImpl
      : UserControlImpl
    {
        #region Class constants
        /// <summary>
        /// Default size of the control.
        /// </summary>
        private const int DEF_SIZE = 15;

        /// <summary>
        /// Default name of the button.
        /// </summary>
        private const string DEF_TEXT = " ";

        /// <summary>
        /// Additional events.
        /// </summary>
        internal static string[] SupportedEvents;
        #endregion

        #region Class members
        /// <summary>
        /// Instance on the button.
        /// </summary>
        private CheckBox m_check;

        /// <summary>
        ///  Holds all events which this class supports.
        /// </summary>
        private static Hashtable m_eventHash;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes static members of the CHECKBOXControlImpl class 
        /// </summary>
        static CHECKBOXControlImpl()
        {
            Type type = typeof(CHECKBOXControlImpl);
            SupportedEvents = BaseElement.FillEventsInfo(type, out m_eventHash);
        }

        /// <summary>
        /// Prevents a default instance of the CHECKBOXControlImpl class from being created
        /// </summary>
        private CHECKBOXControlImpl()
        {
        }

        /// <summary>
        /// Initializes a new instance of the CHECKBOXControlImpl class
        /// </summary>
        /// <param name="parent">IUserControlHolder instance</param>
        public CHECKBOXControlImpl(IUserControlHolder parent)
            : base(parent)
        {
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Overridden. Initializes the control.
        /// </summary>
        public override void InitializeControl()
        {
            DisposeControl();

            m_check = new CheckButtonEx();

            SetControl(m_check);

            ((INPUTElementImpl)this.Parent).OnCreateEvent += new EnhanceEventsEventHandler(Parent_OnCreateEvent);
            ((INPUTElementImpl)this.Parent).MergeSupportedEvents(SupportedEvents);

            ConfigureControl();
        }

        /// <summary>
        /// Overridden. Disposes of control object.
        /// </summary>
        public override void DisposeControl()
        {
            base.DisposeControl();

            if (m_check != null)
            {
                if (!m_check.IsDisposed)
                {
                    DetachEvents();
                    m_check.Dispose();
                }

                m_check = null;
            }
        }

        /// <summary>
        /// Overridden. Configures the button.
        /// </summary>
        public override void ConfigureControl()
        {
            base.ConfigureControl();

            if (!ParentElement.IsStyleHeight)
            {
                IHTMLFormat parentFormat = ParentElement.Parent.Format;
                m_check.Height = (int)parentFormat.Font.Height * 4 / 3;
            }

            if (!ParentElement.IsStyleWidth)
            {
                m_check.Width = DEF_SIZE;
            }

            m_check.TextAlign = ContentAlignment.MiddleCenter;

            ConfigureBgColor();

            // set text on button.
            ConfigureValue();

            ConfigureState();

            OnInitEndCallback();
        }

        /// <summary>
        /// Overridden. Attaches events to the control.
        /// </summary>
        public override void AttachEvents()
        {
            if (this.CustomControl != null && !this.EventsAttached)
            {
                base.AttachEvents();
            }
        }

        /// <summary>
        /// Overridden. Detaches events to the control.
        /// </summary>
        public override void DetachEvents()
        {
            if (this.CustomControl != null && this.EventsAttached)
            {
                base.DetachEvents();
            }
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Configures the text on the button.
        /// </summary>
        private void ConfigureValue()
        {
            if (ParentElement.Attributes.Contains(AttributeName.Value))
            {
                HTMLAttributeImpl attr = (HTMLAttributeImpl)ParentElement.Attributes[AttributeName.Value];
                if (attr != null)
                {
                    m_check.Tag = attr.Value;
                }
            }
        }

        /// <summary>
        /// Configures the state of the control.
        /// </summary>
        private void ConfigureState()
        {
            if (ParentElement.Attributes.Contains(AttributeName.Checked))
            {
                m_check.Checked = true;
            }
        }

        /// <summary>
        /// Sets the background color of the control.
        /// </summary>
        private void ConfigureBgColor()
        {
            INPUTElementImpl inputParent = this.ParentElement as INPUTElementImpl;
            Color parentColor = inputParent.GetBgColorForControl();
            m_check.BackColor = parentColor;
        }

        /// <summary>
        /// Raised when event is needed but is not created yet.
        /// </summary>
        /// <param name="sender">Sender of events.</param>
        /// <param name="e">Event arguments.</param>
        private void Parent_OnCreateEvent(object sender, EnhanceEventsEventArgs e)
        {
            if (m_eventHash != null && m_eventHash.Contains(e.Name))
            {
                e.Event = new HashElementEvents(this.ParentElement, m_eventHash, e.Name);
            }
        }
        #endregion
    }
}
