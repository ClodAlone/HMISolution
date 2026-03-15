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
using Syncfusion.HTMLUI.Base.Utility;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class which represents the button in the document.
    /// </summary>
    public class BUTTONControlImpl
      : UserControlImpl
    {
        #region Class constants
        /// <summary>
        /// Default name of the button.
        /// </summary>
        private const string DEF_TEXT = "button";

        /// <summary>
        /// Additional events.
        /// </summary>
        internal static string[] SupportedEvents;

        /// <summary>
        /// Horizontal indent on the button.
        /// </summary>
        private const int DEF_HINDENT = 15;

        /// <summary>
        /// Vertical indent on the button.
        /// </summary>
        private const int DEF_VINDENT = 5;
        #endregion

        #region Class members
        /// <summary>
        /// Instance on the button.
        /// </summary>
        private Button m_button;

        /// <summary>
        ///  Holds all events which this class supports.
        /// </summary>
        private static Hashtable m_eventHash;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes static members of the BUTTONControlImpl class
        /// </summary>
        static BUTTONControlImpl()
        {
            Type type = typeof(BUTTONControlImpl);
            SupportedEvents = BaseElement.FillEventsInfo(type, out m_eventHash);
        }

        /// <summary>
        /// Prevents a default instance of the BUTTONControlImpl class from being created
        /// </summary>
        private BUTTONControlImpl()
        {
        }

        /// <summary>
        /// Initializes a new instance of the BUTTONControlImpl class
        /// </summary>
        /// <param name="parent">IUserControlHolder instance</param>
        public BUTTONControlImpl(IUserControlHolder parent)
            : base(parent)
        {
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Overridden. Initializes a control.
        /// </summary>
        public override void InitializeControl()
        {
            DisposeControl();

            m_button = new Button();

            SetControl(m_button);

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

            if (m_button != null)
            {
                if (!m_button.IsDisposed)
                {
                    DetachEvents();
                    m_button.Dispose();
                }

                m_button = null;
            }
        }

        /// <summary>
        /// Overridden. Configures the button.
        /// </summary>
        public override void ConfigureControl()
        {
            base.ConfigureControl();

            // set text on button.
            ConfigureValue();

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
                    CustomControl.Text = Utilities.GetConvertedString(attr.Value);
                }
            }
            else
            {
                CustomControl.Text = DEF_TEXT;
            }

            //// Define size of the button.

            SizeF sizef = BaseElement.Graphics.MeasureString(m_button.Text, m_button.Font);
            Size size = Size.Ceiling(sizef);

            int minWidth = size.Width + 2 * DEF_HINDENT;
            int minHeight = size.Height + 2 * DEF_VINDENT;

            if (!ParentElement.IsStyleWidth)
            {
                m_button.Width = minWidth;
            }

            if (!ParentElement.IsStyleHeight)
            {
                m_button.Height = minHeight;
            }
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
