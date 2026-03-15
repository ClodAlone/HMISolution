#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region File using directives
using System;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.Documentation;
#if !SILVERLIGHT && !WP
using Syncfusion.Layouting;
#endif
using System.Windows;
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for WDropDownFormField.
    /// </summary>
    public class WDropDownFormField : WFormField
#if !SILVERLIGHT && !WP
, ILeafWidget
#endif
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private short m_defaultDropDownValue;
        private WDropDownCollection m_dropDownItems;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WDropDownFormField"/> class.
        /// </summary>
        /// <param name="doc"></param>
        public WDropDownFormField(IWordDocument doc)
            : base(doc)
        {
            m_curFormFieldType = FormFieldType.DropDown;
            m_paraItemType = ParagraphItemType.DropDownFormField;
            FieldType = FieldType.FieldFormDropDown;
            Params = 32998;
            m_dropDownItems = new WDropDownCollection(Document);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>The type of the entity.</value>
        public override EntityType EntityType
        {
            get
            {
                return EntityType.DropDownFormField;
            }
        }
        /// <summary>
        /// Gets/sets selected drop down index.
        /// </summary>
        public int DropDownSelectedIndex
        {
            get
            {
                return (Value == DEF_VALUE) ? m_defaultDropDownValue : Value;
            }
            set
            {
                Value = (value);
            }
        }
        /// <summary>
        /// Gets drop down items.
        /// </summary>
        public WDropDownCollection DropDownItems
        {
            get
            {
                return m_dropDownItems;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int DefaultDropDownValue
        {
            get
            {
                return m_defaultDropDownValue;
            }
            set
            {
                m_defaultDropDownValue = (short)value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal string DropDownValue
        {
            get
            {
                if (m_dropDownItems.Count > 0)
                {
                    int index = DropDownSelectedIndex > 0 ? DropDownSelectedIndex : DefaultDropDownValue;
                    if (index < 0 || index > m_dropDownItems.Count)
                        return m_dropDownItems[0].Text;
                    else if (index < m_dropDownItems.Count)
                        return m_dropDownItems[DropDownSelectedIndex].Text;
                }
                return WTextFormField.DEF_TEXT;
            }
            set
            {
                for (int i = 0; i < m_dropDownItems.Count; i++)
                {
#if SILVERLIGHT || WP
          if( string.Compare( m_dropDownItems[ i ].Text, value ) == 0 )
#else
                    if (string.Compare(m_dropDownItems[i].Text, value, true) == 0)
#endif
                    {
                        DropDownSelectedIndex = i;
                        return;
                    }
                }
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// </summary>
        /// <returns></returns>
        protected override object CloneImpl()
        {
            WDropDownFormField dropDown = (WDropDownFormField)base.CloneImpl();
            dropDown.m_dropDownItems = new WDropDownCollection(Document);
            m_dropDownItems.CloneTo(dropDown.m_dropDownItems);

            return dropDown;
        }
        #endregion

        #region Implementation / xml
        //#if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            if (reader.HasAttribute(XDLSConstants.FormFieldDefaultDropDownValueAttr))
            {
                m_defaultDropDownValue = reader.ReadShort(XDLSConstants.FormFieldDefaultDropDownValueAttr);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(Syncfusion.DocIO.DLS.XML.IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);

            writer.WriteValue(XDLSConstants.FormFieldDefaultDropDownValueAttr, m_defaultDropDownValue);
        }
        /// <summary>
        /// 
        /// </summary>
        [DocumentationExclude()]
        protected override void InitXDLSHolder()
        {
            base.InitXDLSHolder();

            XDLSHolder.AddElement(XDLSConstants.FormFieldDropDownItemsTag, m_dropDownItems);
        }
        //#endif

        #endregion

        #region Implementation / layout
#if !SILVERLIGHT && !WP
        /// <summary>
        /// </summary>
        protected override void CreateLayoutInfo()
        {
            m_layoutInfo = new LayoutInfo(ChildrenLayoutDirection.Horizontal);
        }
#endif
        #endregion

        #region ILeafWidget Members
#if !SILVERLIGHT && !WP

        SizeF ILeafWidget.Measure(Syncfusion.DocIO.Rendering.DrawingContext dc)
        {
            return dc.MeasureString(DropDownValue, CharacterFormat.Font, null, CharacterFormat,false);
        }
#endif

        #endregion

        #region IWidget Members
#if !SILVERLIGHT && !WP
        ILayoutInfo IWidget.LayoutInfo
        {
            get
            {
                if (m_layoutInfo == null)
                    CreateLayoutInfo();

                return m_layoutInfo;
            }
        }

        void IWidget.Draw(Syncfusion.DocIO.Rendering.DrawingContext dc, LayoutedWidget ltWidget)
        {
            dc.DrawString(DropDownValue, CharacterFormat, null, ltWidget.Bounds, ltWidget.Bounds.Width, ltWidget);
        }
        /// <summary>
        /// Initializing LayoutInfo value to null
        /// </summary>
        void IWidget.InitLayoutInfo()
        {
            m_layoutInfo = null;
        }
#endif

        #endregion
    }
}
