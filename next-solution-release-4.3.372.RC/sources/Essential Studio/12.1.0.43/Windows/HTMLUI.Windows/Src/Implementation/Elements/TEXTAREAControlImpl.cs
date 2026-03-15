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
using Syncfusion.Windows.Forms.HTMLUI;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class which represents the text area in the document.
    /// </summary>
    public class TEXTAREAControlImpl
      : UserControlImpl
    {
        #region Class constants
        /// <summary>
        /// Default name of the button.
        /// </summary>
        private const string DEF_VALUE = " ";

        /// <summary>
        /// Default width of the control.
        /// </summary>
        private const int DEF_WIDTH = 170;

        /// <summary>
        /// Default height of the control.
        /// </summary>
        private const int DEF_HEIGHT = 50;

        /// <summary>
        /// Additional events.
        /// </summary>
        private static string[] SupportedEvents;
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
        #endregion

        #region Class Events
        /// <summary>
        /// Event which is raised when the text in the text box is changed.
        /// </summary>
        [ElementEvent("RaiseTextChangedEvent")]
        public event EventHandler TextChanged;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes static members of the TEXTAREAControlImpl class 
        /// </summary>
        static TEXTAREAControlImpl()
        {
            Type type = typeof(TEXTAREAControlImpl);
            SupportedEvents = BaseElement.FillEventsInfo(type, out m_eventHash);
        }

        /// <summary>
        /// Prevents a default instance of the TEXTAREAControlImpl class from being created
        /// </summary>
        private TEXTAREAControlImpl()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TEXTAREAControlImpl class
        /// </summary>
        /// <param name="parent">IUserControlHolder instance</param>
        public TEXTAREAControlImpl(IUserControlHolder parent)
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

            m_textBox = new TextBox();

            SetControl(m_textBox);

            ((TEXTAREAElementImpl)this.Parent).OnCreateEvent += new EnhanceEventsEventHandler(Parent_OnCreateEvent);
            ((TEXTAREAElementImpl)this.Parent).MergeSupportedEvents(SupportedEvents);

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

            m_textBox.Multiline = true;
            m_textBox.ScrollBars = ScrollBars.Both;
            Font font = new Font(FontFamily.GenericMonospace, m_textBox.Font.Size);
            m_textBox.Font = font;

            int defWidth = ParentElement.IsStyleWidth ? m_textBox.Width : DEF_WIDTH;
            int defHeight = ParentElement.IsStyleHeight ? m_textBox.Height : DEF_HEIGHT;

            m_textBox.Size = new Size(defWidth, defHeight);

            // Set text in text box.
            ConfigureValue();

            // Set size of text box.
            ConfigureSizeByAttributes();

            // Set max length of text box.
            ConfigureMaxLength();
            OnInitEndCallback();
        }

        /// <summary>
        /// Overridden. Attaches events to the element.
        /// </summary>
        public override void AttachEvents()
        {
            if (this.CustomControl != null && !this.EventsAttached)
            {
                base.AttachEvents();

                m_textBox.TextChanged += new EventHandler(M_textBox_TextChanged);
            }
        }

        /// <summary>
        /// Overridden. Detaches events from the element.
        /// </summary>
        public override void DetachEvents()
        {
            if (this.CustomControl != null && this.EventsAttached)
            {
                base.DetachEvents();

                m_textBox.TextChanged -= new EventHandler(M_textBox_TextChanged);
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
        /// Raised when the text in the text box is changed.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments.</param>
        private void M_textBox_TextChanged(object sender, EventArgs e)
        {
            RaiseTextChangedEvent(e);
        }

        /// <summary>
        /// Raises the text changed event.
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
            if (ParentElement.Storage.InnerText.Length > 0)
            {
                m_textBox.Text = Utilities.GetConvertedString(ParentElement.Storage.InnerText);
            }
            else
            {
                m_textBox.Text = DEF_VALUE;
            }
        }

        /// <summary>
        /// Configures the width of the text box.
        /// </summary>
        private void ConfigureSizeByAttributes()
        {
            if (ParentElement.Attributes.Contains(AttributeName.Rows) &&
              !ParentElement.IsStyleHeight)
            {
                HTMLAttributeImpl attrWdth = (HTMLAttributeImpl)ParentElement.Attributes[AttributeName.Rows];
                if (attrWdth != null)
                {
                    double result = 0;
                    if (Double.TryParse(attrWdth.Value, NumberStyles.Integer, null, out result))
                    {
                        if (result > 0)
                        {
                            result = ParentElement.Format.Font.Height * result;
                            m_textBox.Height = (int)result;
                        }
                    }
                }
            }

            if (ParentElement.Attributes.Contains(AttributeName.Cols) &&
              !ParentElement.IsStyleWidth)
            {
                HTMLAttributeImpl attrHeight = (HTMLAttributeImpl)ParentElement.Attributes[AttributeName.Cols];
                if (attrHeight != null)
                {
                    double result = 0;
                    if (Double.TryParse(attrHeight.Value, NumberStyles.Integer, null, out result))
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
        /// Creates a new event for the element.
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
