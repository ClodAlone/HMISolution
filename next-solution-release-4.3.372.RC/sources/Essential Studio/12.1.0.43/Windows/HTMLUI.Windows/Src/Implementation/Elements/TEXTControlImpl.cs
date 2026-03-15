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
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class which represents the text box in the document.
    /// </summary>
    public class TEXTControlImpl
      : UserControlImpl
    {
        #region Class constants
        /// <summary>
        /// Default name of the button.
        /// </summary>
        private const string DEF_VALUE = " ";

        /// <summary>
        /// Additional events.
        /// </summary>
        internal static string[] DEF_EVENTS;
        #endregion

        #region Class members
        /// <summary>
        ///  Holds all events which this class supports.
        /// </summary>
        private static Hashtable m_eventHash;

        /// <summary>
        /// Text box control instance.
        /// </summary>
        private TextBox m_textBox;

        /// <summary>
        /// Indicates whether the control must be visible.
        /// </summary>
        private bool m_bIsVisible = true;

        /// <summary>
        /// Indicates whether the element has password style.
        /// </summary>
        private bool m_bIsPassword;

        /// <summary>
        /// Indicates whether the symbol should be written.
        /// </summary>
        private bool m_bPreventChar;
        #endregion

        #region Class Events
        /// <summary>
        /// Event which is raised when text in text box is changed.
        /// </summary>
        [ElementEvent("RaiseTextChangedEvent")]
        public event EventHandler TextChanged;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes static members of the TEXTControlImpl class
        /// </summary>
        static TEXTControlImpl()
        {
            Type type = typeof(TEXTControlImpl);
            DEF_EVENTS = BaseElement.FillEventsInfo(type, out m_eventHash);
        }

        /// <summary>
        /// Prevents a default instance of the TEXTControlImpl class from being created
        /// </summary>
        private TEXTControlImpl()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TEXTControlImpl class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="isPassword">Indicates whether the control has password state.</param>
        public TEXTControlImpl(IUserControlHolder parent, bool isPassword)
            : base(parent, false)
        {
            m_bIsPassword = isPassword;

            InitializeControl();
        }

        /// <summary>
        /// Initializes a new instance of the TEXTControlImpl class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="isPassword">Indicates whether the control has password state.</param>
        /// <param name="isVisible">Indicates whether the control is visible.</param>
        public TEXTControlImpl(IUserControlHolder parent, bool isPassword, bool isVisible)
            : this(parent, isPassword)
        {
            m_textBox.Visible = isVisible;
            m_bIsVisible = isVisible;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Overridden. Initializes a control.
        /// </summary>
        public override void InitializeControl()
        {
            DisposeControl();

            m_textBox = new TextBox();
            m_textBox.Visible = m_bIsVisible;
            m_textBox.Multiline = true;

            if (m_bIsPassword)
            {
                m_textBox.PasswordChar = '*';
                m_textBox.Multiline = false;
            }

            SetControl(m_textBox);

            ((INPUTElementImpl)this.Parent).OnCreateEvent += new EnhanceEventsEventHandler(Parent_OnCreateEvent);
            ((INPUTElementImpl)this.Parent).MergeSupportedEvents(DEF_EVENTS);

            m_textBox.KeyDown += new KeyEventHandler(KeyDown);
            m_textBox.KeyPress += new KeyPressEventHandler(KeyPress);

            ConfigureControl();
        }

        /// <summary>
        /// Overridden. Disposes control object.
        /// </summary>
        public override void DisposeControl()
        {
            base.DisposeControl();

            if (m_textBox != null)
            {
                if (!m_textBox.IsDisposed)
                {
                    DetachEvents();
                    m_textBox.Dispose();
                }

                m_textBox = null;
            }
        }

        /// <summary>
        /// Overridden. Configures the text box control.
        /// </summary>
        public override void ConfigureControl()
        {
            base.ConfigureControl();

            // Set text in text box.
            ConfigureValue();

            // Set size of text box.
            ConfigureSizeByAttribute();

            // Set max length of text box.
            ConfigureMaxLength();

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
                m_textBox.TextChanged += new EventHandler(HandleTextChanged);
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
                m_textBox.TextChanged -= new EventHandler(HandleTextChanged);
                m_textBox.KeyDown -= new KeyEventHandler(KeyDown);
                m_textBox.KeyPress -= new KeyPressEventHandler(KeyPress);
            }
        }

        /// <summary>
        /// Overridden. Sets the Readonly state for the control.
        /// </summary>
        protected override void SetEnabledState()
        {
            bool bReadonly = ParentElement.Attributes.Contains(AttributeName.Disabled);

            if (bReadonly)
            {
                Color color = m_textBox.BackColor;
                m_textBox.ReadOnly = bReadonly;
                m_textBox.BackColor = color;
            }
            else
            {
                m_textBox.ReadOnly = bReadonly;
            }
        }
        #endregion

        #region Class Event Handlers
        /// <summary>
        /// Raised when text in text box is changed.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments.</param>
        private void HandleTextChanged(object sender, EventArgs e)
        {
            RaiseTextChangedEvent(e);
        }

        /// <summary>
        /// Raised when key was down.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event data.</param>
        private void KeyDown(object sender, KeyEventArgs e)
        {
            Keys key = e.KeyCode & Keys.KeyCode;

            if (key == Keys.Enter)
            {
                m_bPreventChar = true;
            }
            else
            {
                m_bPreventChar = false;
            }
        }

        /// <summary>
        /// Raised when key was pressed.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event data.</param>
        private void KeyPress(object sender, KeyPressEventArgs e)
        {
            if (m_bPreventChar)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Raises text changed event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected internal virtual void RaiseTextChangedEvent(EventArgs args)
        {
            this.ParentElement.RaiseBubblingEvent(TextChanged, EventName.TextChanged, args);
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Configures the value of the text box.
        /// </summary>
        private void ConfigureValue()
        {
            if (ParentElement.Attributes.Contains(AttributeName.Value))
            {
                HTMLAttributeImpl attr = (HTMLAttributeImpl)ParentElement.Attributes[AttributeName.Value];
                if (attr != null)
                {
                    m_textBox.Text = attr.Value;
                }
            }
            else
            {
                m_textBox.Text = DEF_VALUE;
            }
        }

        /// <summary>
        /// Configures the width of the text box.
        /// </summary>
        private void ConfigureSizeByAttribute()
        {
            if (!m_textBox.Visible)
            {
                m_textBox.Width = 0;
                m_textBox.Height = 0;

                return;
            }

            if (ParentElement.Attributes.Contains(AttributeName.Size))
            {
                HTMLAttributeImpl attr = (HTMLAttributeImpl)ParentElement.Attributes[AttributeName.Size];
                if (attr != null)
                {
                    double result = 0;
                    if (Double.TryParse(attr.Value, NumberStyles.Integer, null, out result))
                    {
                        if (result > 0)
                        {
                            result = ParentElement.Format.Font.Size * result;
                            m_textBox.Width = (int)result;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Configures the maxlength property of the text box.
        /// </summary>
        private void ConfigureMaxLength()
        {
            if (ParentElement.Attributes.Contains(AttributeName.MaxLength))
            {
                HTMLAttributeImpl attr = (HTMLAttributeImpl)ParentElement.Attributes[AttributeName.MaxLength];
                if (attr != null)
                {
                    double result = 0;
                    if (Double.TryParse(attr.Value, NumberStyles.Integer, null, out result))
                    {
                        if (result >= 0)
                        {
                            m_textBox.MaxLength = (int)result;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates the special event of the control.
        /// </summary>
        /// <param name="sender">Event sender.</param>
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
