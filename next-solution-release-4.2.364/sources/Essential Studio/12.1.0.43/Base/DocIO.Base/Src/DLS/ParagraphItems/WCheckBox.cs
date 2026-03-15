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
using Syncfusion.DocIO.ReaderWriter.Biff_Records;

#if !SILVERLIGHT && !WP
using Syncfusion.Layouting;
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for WCheckBox.
    /// </summary>
    public class WCheckBox : WFormField
#if !SILVERLIGHT && !WP
        , ILeafWidget
#endif
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private int m_checkBoxSize;
        private bool m_defCheckBoxValue;
        private CheckBoxSizeType m_sizeType;
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
                return EntityType.CheckBox;
            }
        }
        /// <summary>
        /// Gets/sets size of checkbox.
        /// </summary>
        public int CheckBoxSize
        {
            get
            {
                return m_checkBoxSize;
            }
            set
            {
                m_checkBoxSize = value;
            }
        }
        /// <summary>
        /// Gets/sets default checkbox value.
        /// </summary>
        public bool DefaultCheckBoxValue
        {
            get
            {
                return m_defCheckBoxValue;
            }
            set
            {
                m_defCheckBoxValue = value;
            }
        }
        /// <summary>
        /// Gets/sets Checked property.
        /// </summary>
        public bool Checked
        {
            get
            {
                switch (Value)
                {
                    case 0:
                        {
                            return false;
                        }
                    case 1:
                        {
                            return true;
                        }
                    case 25:
                        {
                            return m_defCheckBoxValue;
                        }
                }
                throw new ArgumentException("Unsupported checkbox field value found.");
            }
            set
            {
                Value = (value ? 1 : 0);
            }
        }
        /// <summary>
        /// Gets/sets check box size type.
        /// </summary>
        public CheckBoxSizeType SizeType
        {
            get
            {
                return ((Params & 0x400) == 1024) ? CheckBoxSizeType.Exactly : CheckBoxSizeType.Auto;
            }
            set
            {
                m_sizeType = value;
                if (value == CheckBoxSizeType.Exactly)
                {
                    Params = (short)BaseWordRecord.SetBitsByMask(Params, 0x400, 10, 1);
                }
                else
                {
                    Params = (short)BaseWordRecord.SetBitsByMask(Params, 0x400, 10, 0);
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WCheckBox"/> class.
        /// </summary>
        /// <param name="doc"></param>
        public WCheckBox(IWordDocument doc)
            : base(doc)
        {
            m_curFormFieldType = FormFieldType.CheckBox;
            m_paraItemType = ParagraphItemType.CheckBox;
            FieldType = FieldType.FieldFormCheckBox;
            Params = 229; //101;
            m_checkBoxSize = 20;

        }
        #endregion

        #region Implementation
        /// <summary>
        /// </summary>
        /// <returns></returns>
        protected override object CloneImpl()
        {
            WCheckBox checkBox = (WCheckBox)base.CloneImpl();

            return checkBox;
        }
//#if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            if (reader.HasAttribute(XDLSConstants.FormFieldCheckBoxSizeAttr))
            {
                m_checkBoxSize = reader.ReadShort(XDLSConstants.FormFieldCheckBoxSizeAttr);
            }
            if (reader.HasAttribute(XDLSConstants.FormFieldDefaultCheckBoxValueAttr))
            {
                m_defCheckBoxValue = reader.ReadBoolean(XDLSConstants.FormFieldDefaultCheckBoxValueAttr);
            }
            if (reader.HasAttribute(XDLSConstants.FormFieldCheckBoxSizeType))
            {
                SizeType =
                  (CheckBoxSizeType)
                  reader.ReadEnum(XDLSConstants.FormFieldCheckBoxSizeType, typeof(CheckBoxSizeType));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);

            writer.WriteValue(XDLSConstants.FormFieldCheckBoxSizeAttr, m_checkBoxSize);
            writer.WriteValue(XDLSConstants.FormFieldCheckBoxSizeType, SizeType);
            writer.WriteValue(XDLSConstants.FormFieldDefaultCheckBoxValueAttr, m_defCheckBoxValue);
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
            if (m_sizeType != CheckBoxSizeType.Auto)
                return new SizeF((float)m_checkBoxSize, (float)m_checkBoxSize);
            else
            {
                //Sets the size of checkbox if checkbox size type is auto
                WTextRange textRange = this as WTextRange;
                float fontSize = textRange.CharacterFormat.FontSize;
                return new SizeF(fontSize, fontSize);
            }
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
            dc.DrawCheckBox(this, ltWidget);
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
