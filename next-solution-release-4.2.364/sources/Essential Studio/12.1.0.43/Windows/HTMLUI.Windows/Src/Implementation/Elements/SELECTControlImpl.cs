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
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class which represents the list in the document.
    /// </summary>
    public class SELECTControlImpl
      : UserControlImpl
    {
        #region Class constants
        /// <summary>
        /// XPath for retrieving items for select control.
        /// </summary>
        private const string DEF_OPTION_XPATH = "node()[ name()='" + TagName.Option + "' ]";

        /// <summary>
        /// Width of the empty list.
        /// </summary>
        private const int DEF_EMPTY_WIDTH = 20;

        /// <summary>
        /// Default name of the button.
        /// </summary>
        private const string DEF_VALUE = " ";

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
        /// List box or combo box control instance.
        /// </summary>
        private Control m_list;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes static members of the SELECTControlImpl class 
        /// </summary>
        static SELECTControlImpl()
        {
            Type type = typeof(SELECTControlImpl);
            SupportedEvents = BaseElement.FillEventsInfo(type, out m_eventHash);
        }

        /// <summary>
        /// Prevents a default instance of the SELECTControlImpl class from being created
        /// </summary>
        private SELECTControlImpl()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SELECTControlImpl class
        /// </summary>
        /// <param name="parent">IUserControlHolder instance</param>
        public SELECTControlImpl(IUserControlHolder parent)
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
            CreateInstance();

            SetControl(m_list);

            ((SELECTElementImpl)this.Parent).OnCreateEvent += new EnhanceEventsEventHandler(Parent_OnCreateEvent);
            ((SELECTElementImpl)this.Parent).MergeSupportedEvents(SupportedEvents);

            ConfigureControl();
        }

        /// <summary>
        /// Overridden. Disposes control object.
        /// </summary>
        public override void DisposeControl()
        {
            base.DisposeControl();

            if (m_list != null)
            {
                if (!m_list.IsDisposed)
                {
                    DetachEvents();
                    m_list.Dispose();
                }

                m_list = null;
            }
        }

        /// <summary>
        /// Overridden. Configures the control.
        /// </summary>
        public override void ConfigureControl()
        {
            base.ConfigureControl();

            ConfigureContent();

            ConfigureSize();

            SelectItems();

            OnInitEndCallback();
        }

        /// <summary>
        /// Overridden. Sets the Enabled state for control.
        /// </summary>
        protected override void SetEnabledState()
        {
            ComboBox combo = m_list as ComboBox;

            if (combo != null)
            {
                bool bReadonly = ParentElement.Attributes.Contains(AttributeName.Disabled);

                if (bReadonly)
                {
                    Color color = combo.BackColor;
                    combo.Enabled = !bReadonly;
                    combo.BackColor = color;
                }
                else
                {
                    combo.Enabled = !bReadonly;
                }
            }
            else
            {
                base.SetEnabledState();
            }
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Calculates width of the control.
        /// </summary>
        private void CalculateWidth()
        {
            IList items = null;
            ComboBox combo = m_list as ComboBox;
            ListBox list = m_list as ListBox;

            if (list != null)
            {
                items = list.Items;
            }
            else if (combo != null)
            {
                items = combo.Items;
            }

            if (items != null)
            {
                int width = DEF_EMPTY_WIDTH;
                using (Graphics g = m_list.CreateGraphics())
                {
                    for (int i = 0, len = items.Count; i < len; i++)
                    {
                        string item = (string)items[i];
                        Size itemSize = Size.Ceiling(g.MeasureString(item, m_list.Font));
                        int itemWidth = itemSize.Width;
                        width = (int)Math.Max(width, DEF_EMPTY_WIDTH + itemWidth);
                    }
                }

                m_list.Width = width;
            }
        }

        /// <summary>
        /// Calculates height of the control.
        /// </summary>
        private void CalculateHeight()
        {
            if (ParentElement.Attributes.Contains(AttributeName.Size)
              && (m_list is ListBox) && !ParentElement.IsStyleHeight)
            {
                HTMLAttributeImpl attr = (HTMLAttributeImpl)ParentElement.Attributes[AttributeName.Size];
                if (attr != null)
                {
                    double result = 0;
                    if (Double.TryParse(attr.Value, NumberStyles.Integer, null, out result))
                    {
                        if (((int)result) > 0)
                        {
                            result = m_list.Font.Height * (result + 1);
                            m_list.Height = (int)result;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Configures the width of the control.
        /// </summary>
        private void ConfigureSize()
        {
            CalculateWidth();
            CalculateHeight();
        }

        /// <summary>
        /// Attaches events to the control.
        /// </summary>
        public override void AttachEvents()
        {
            if (this.CustomControl != null && !this.EventsAttached)
            {
                base.AttachEvents();
            }
        }

        /// <summary>
        /// Detaches events from the control.
        /// </summary>
        public override void DetachEvents()
        {
            if (this.CustomControl != null && this.EventsAttached)
            {
                base.DetachEvents();
            }
        }

        /// <summary>
        /// Creates combo box or list box of control depending on attributes.
        /// </summary>
        private void CreateInstance()
        {
            DisposeControl();

            HTMLAttributeImpl attrMult = (HTMLAttributeImpl)this.ParentElement.Attributes[AttributeName.Multiple];

            if (attrMult != null)
            {
                m_list = CreateListBox();
            }
            else
            {
                HTMLAttributeImpl attrSize = (HTMLAttributeImpl)this.ParentElement.Attributes[AttributeName.Size];

                if (attrSize != null && attrSize.Value.Length > 0)
                {
                    double result;

                    if (Double.TryParse(attrSize.Value, NumberStyles.Integer, null, out result) && result > 1)
                    {
                        m_list = CreateListBox();
                    }
                    else
                    {
                        m_list = CreateComboBox();
                    }
                }
                else
                {
                    m_list = CreateComboBox();
                }
            }
        }

        /// <summary>
        /// Creates list box control.
        /// </summary>
        /// <returns>List box control.</returns>
        private ListBox CreateListBox()
        {
            return new ListBox();
        }

        /// <summary>
        /// Creates combo box control.
        /// </summary>
        /// <returns>Combo box control.</returns>
        private ComboBox CreateComboBox()
        {
            return new ComboBox();
        }

        /// <summary>
        /// Adds items to the control.
        /// </summary>
        private void ConfigureContent()
        {
            ArrayList values = GetItems();
            if (m_list is ListBox)
            {
                ListBox listBox = (ListBox)m_list;
                listBox.Items.Clear();
                listBox.Items.AddRange((object[])values.ToArray(typeof(object)));
                listBox.ScrollAlwaysVisible = true;
            }
            else
            {
                ComboBox comboBox = (ComboBox)m_list;
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                comboBox.Items.Clear();
                comboBox.Items.AddRange((object[])values.ToArray(typeof(object)));
            }
        }

        /// <summary>
        /// Returns the list of items.
        /// </summary>
        /// <returns>List of items.</returns>
        private ArrayList GetItems()
        {
            ArrayList result = new ArrayList();
            XmlElement storage = this.ParentElement.Storage;
            XmlNodeList nodeList = storage.SelectNodes(DEF_OPTION_XPATH);

            if (nodeList.Count == 0) return result;

            XmlElement element = null;

            for (int i = 0, len = nodeList.Count; i < len; i++)
            {
                element = nodeList[i] as XmlElement;

                result.Add(Utilities.GetConvertedString(element.InnerText));
            }

            return result;
        }

        /// <summary>
        /// Selects the corresponding items in list.
        /// </summary>
        private void SelectItems()
        {
            ArrayList selected = GetSelectedIndexes();
            if (selected.Count == 0) return;

            if (!this.ParentElement.Attributes.Contains(AttributeName.Multiple))
            {
                if (m_list is ListBox)
                {
                    (m_list as ListBox).SelectedIndex = (int)selected[0];
                }
                else
                {
                    ((ComboBox)m_list).SelectedIndex = (int)selected[0];
                }
            }
            else
            {
                if (m_list is ListBox)
                {
                    ListBox boxTmp = m_list as ListBox;

                    boxTmp.SelectionMode = SelectionMode.MultiExtended;

                    for (int i = 0, len = selected.Count; i < len; i++)
                    {
                        int index = (int)selected[i];
                        boxTmp.SetSelected(index, true);

                        // NOTE: This line is needed for correct selection in the list box.
                        boxTmp.SelectedIndex.ToString();
                    }
                }
                else
                {
                    ((ComboBox)m_list).SelectedIndex = (int)selected[0];
                }
            }
        }

        /// <summary>
        /// Returns the indices of items which must be selected.
        /// </summary>
        /// <returns>Indices of items which must be selected.</returns>
        private ArrayList GetSelectedIndexes()
        {
            ArrayList result = new ArrayList();
            XmlElement storage = this.ParentElement.Storage;
            XmlNodeList nodeList = storage.SelectNodes(DEF_OPTION_XPATH);

            if (nodeList.Count == 0) return result;

            for (int i = 0; i < nodeList.Count; i++)
            {
                XmlElement element = (XmlElement)nodeList[i];
                XmlAttribute attr = element.Attributes[AttributeName.Selected];
                if (attr != null)
                {
                    result.Add(i);
                }
            }

            return result;
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
