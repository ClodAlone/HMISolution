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
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class which represents the radio button in the document.
    /// </summary>
    public class RADIOControlImpl
      : UserControlImpl
    {
        #region Class constants
        /// <summary>
        /// Width and height of the radio button.
        /// </summary>
        private const int DEF_SIZE = 15;
        #endregion

        #region Class members
        /// <summary>
        /// Instance of the radio button.
        /// </summary>
        private RadioButtonEx m_radio;

        /// <summary>
        /// Additional events.
        /// </summary>
        internal static string[] SupportedEvents;

        /// <summary>
        ///  Holds all events which this class supports.
        /// </summary>
        private static Hashtable m_eventHash;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes static members of the RADIOControlImpl class 
        /// </summary>
        static RADIOControlImpl()
        {
            Type type = typeof(RADIOControlImpl);
            SupportedEvents = BaseElement.FillEventsInfo(type, out m_eventHash);
        }

        /// <summary>
        /// Prevents a default instance of the RADIOControlImpl class from being created
        /// </summary>
        private RADIOControlImpl()
        {
        }

        /// <summary>
        /// Initializes a new instance of the RADIOControlImpl class
        /// </summary>
        /// <param name="parent">IUserControlHolder instance</param>
        public RADIOControlImpl(IUserControlHolder parent)
            : base(parent)
        {
        }
        #endregion

        #region Class Event Handlers
        /// <summary>
        /// Raised when user clicks on RadioButton control element.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments.</param>
        protected void Radio_Click(object sender, EventArgs e)
        {
            if (!m_radio.Checked)
            {
                ArrayList buttons = GetBtnsInSameGroup(m_radio.GroupName);

                object item = null;
                RadioButtonEx button = null;

                for (int i = 0, len = buttons.Count; i < len; i++)
                {
                    item = buttons[i];
                    if (item is RadioButtonEx == false) continue;

                    button = item as RadioButtonEx;
                    button.Checked = false;
                }

                m_radio.Checked = true;
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Overridden. Initializes a project.
        /// </summary>
        public override void InitializeControl()
        {
            CreateInstance();

            SetControl(m_radio);

            ((INPUTElementImpl)this.Parent).OnCreateEvent += new EnhanceEventsEventHandler(Parent_OnCreateEvent);
            ((INPUTElementImpl)this.Parent).MergeSupportedEvents(SupportedEvents);

            ConfigureControl();
        }

        /// <summary>
        /// Overridden. Disposes control object.
        /// </summary>
        public override void DisposeControl()
        {
            base.DisposeControl();

            if (m_radio != null)
            {
                if (!m_radio.IsDisposed)
                {
                    DetachEvents();
                    m_radio.Dispose();
                }

                m_radio = null;
            }
        }

        /// <summary>
        /// Overridden. Configures the button.
        /// </summary>
        public override void ConfigureControl()
        {
            base.ConfigureControl();

            m_radio.Text = string.Empty;

            if (!ParentElement.IsStyleWidth)
            {
                m_radio.Width = DEF_SIZE;
            }

            if (!ParentElement.IsStyleHeight)
            {
                IHTMLFormat parentFormat = ParentElement.Parent.Format;
                m_radio.Height = (int)parentFormat.Font.Height * 4 / 3;
            }

            ConfigureBgColor();

            // Set state on button.
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

                m_radio.Click += new EventHandler(Radio_Click);
            }
        }

        /// <summary>
        /// Overridden. Detaches events from the control.
        /// </summary>
        public override void DetachEvents()
        {
            if (this.CustomControl != null && this.EventsAttached)
            {
                base.DetachEvents();

                m_radio.Click -= new EventHandler(Radio_Click);
            }
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Configures the text on the button.
        /// </summary>
        private void ConfigureState()
        {
            if (ParentElement.Attributes.Contains(AttributeName.Checked))
            {
                m_radio.Checked = true;
            }

            // Add to all controls in the page.
            if (!ParentElement.Attributes.Contains(AttributeName.Name))
            {
                m_radio.AutoCheck = false;
            }
        }

        /// <summary>
        /// Sets the background color of the radio control.
        /// </summary>
        private void ConfigureBgColor()
        {
            Color parentColor = (this.ParentElement as INPUTElementImpl).GetBgColorForControl();
            m_radio.BackColor = parentColor;

            /*
            Color parentColor = this.ParentElement.Parent.Format.BackgroundColor;
            Color defColor = this.ParentElement.Control.DefaultFormat.BackgroundColor;

            if( parentColor != defColor )
            {
              m_radio.BackColor = parentColor;
            }
            else
            {
              m_radio.BackColor = defColor;
            }*/
        }

        /// <summary>
        /// Creates an instance of the RadioButtonEx class.
        /// </summary>
        private void CreateInstance()
        {
            DisposeControl();

            string groupName = string.Empty;

            if (this.ParentElement.Attributes.Contains(AttributeName.Name))
            {
                groupName = this.ParentElement.Attributes[AttributeName.Name].Value;
            }

            m_radio = new RadioButtonEx(this, groupName);
            m_radio.AutoCheck = false;
        }

        /// <summary>
        /// Returns a list of all RadioButtons which have the same group name.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <returns>Array of controls in the group.</returns>
        private ArrayList GetBtnsInSameGroup(string groupName)
        {
            if (groupName == null)
                throw new ArgumentNullException("groupName");

            ArrayList result = new ArrayList();

            for (int index = 0; index < DocumentUserControls.Count; index++)
            {
                ICustomControlBase control = DocumentUserControls[index] as ICustomControlBase;

                if (control.CustomControl is RadioButtonEx)
                {
                    RadioButtonEx radio = control.CustomControl as RadioButtonEx;

                    if (string.Compare(radio.GroupName, groupName, true, CultureInfo.InvariantCulture) == 0)
                    {
                        IHTMLElement thisParent = GetContainer(m_radio);
                        IHTMLElement controlParent = GetContainer(radio);

                        if (thisParent == controlParent)
                        {
                            result.Add(control.CustomControl);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the parent for the control but only BODY or FORM element.
        /// </summary>
        /// <param name="button">Button control.</param>
        /// <returns>Parent container of the control.</returns>
        private IHTMLElement GetContainer(RadioButtonEx button)
        {
            if (button == null)
                throw new ArgumentNullException("button");

            IHTMLElement parent = button.ParentElement.ParentElement.Parent;
            while (parent != null && !(parent is BODYElementImpl) && !(parent is FORMElementImpl))
            {
                parent = parent.Parent;
            }

            return parent;
        }

        /// <summary>
        /// Creates a special event of the control.
        /// </summary>
        /// <param name="sender">Event sender</param>
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
