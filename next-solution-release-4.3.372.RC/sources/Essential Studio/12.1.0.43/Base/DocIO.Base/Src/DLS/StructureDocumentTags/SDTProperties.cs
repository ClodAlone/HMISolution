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

#region file using directives

#endregion

namespace Syncfusion.DocIO.DLS
{
    internal class SDTProperties
    {
        #region fields
        private string m_Alias = string .Empty ;
        private bool m_Bibliography;
        private bool m_Citation;
        private SDTComboBox m_SDTComboBox;
        private SDTDataBinding m_DataBinding;
        private SDTDate m_date;
        private DocPartList m_DocPartList;
        private DocPartObj m_DocPartObj;
        private SDTDropDownList m_SDTDropDownList;
        //private bool m_Equation
        //private book m_Picture
        private StructureDocumentType m_SDTType = StructureDocumentType.None;
        //ID will be auto generated private int ID;
        //Todo: Further research
        private string m_Label;
        private LockSettings m_LockSettings;
        //Todo: Place holder text will be present in glossary document. Once it is supported we need parse and update this
        //private WParagraph m_PlaceHolderPara
        //private string m_PlaceHolder;
        private WCharacterFormat m_CharacterFormat;
        private bool m_IsShowingPlaceHolder;
        private uint m_TabIndex;
        private string m_Tag;
        private bool m_IsTemporary;
        private ContentRepeatingType m_ContentRepeatingType = ContentRepeatingType.None;
        private string m_id;
        private SDTCheckBox m_sdtCheckBox;
        #endregion

        #region Properties
        internal string ID
        {
            get
            {
                return m_id;
            }
            set
            {
                m_id = value;
            }
        }
        internal SDTCheckBox SDTCheckBox
        {
            get
            {
                return m_sdtCheckBox;
            }
            set
            {
                m_sdtCheckBox = value;
            }
        }
        internal string Alias
        {
            get
            {
                return m_Alias;
            }
            set
            {
                m_Alias = value;
            }
        }
        internal bool Bibliograph
        {
            get
            {
                return m_Bibliography;
            }
            set
            {
                m_Bibliography = value;
            }
        }
        internal bool Citation
        {
            get
            {
                return m_Citation;
            }
            set
            {
                m_Citation = value;
            }
        }
        internal SDTComboBox SDTComboBox
        {
            get
            {
                return m_SDTComboBox;
            }
            set
            {
                m_SDTComboBox = value;
            }
        }
        internal SDTDataBinding DataBinding
        {
            get
            {
                return m_DataBinding;
            }
            set
            {
                m_DataBinding = value;
            }
        }
        internal SDTDate Date
        {
            get
            {
                return m_date;
            }
            set
            {
                m_date = value;
            }
        }
        internal DocPartList DocPartList
        {
            get
            {
                return m_DocPartList;
            }
            set
            {
                m_DocPartList = value;
            }
        }
        internal DocPartObj DocPartObj
        {
            get
            {
                return m_DocPartObj;
            }
            set
            {
                m_DocPartObj = value;
            }
        }
        internal SDTDropDownList SDTDropDownList
        {
            get
            {
                return m_SDTDropDownList;
            }
            set
            {
                m_SDTDropDownList = value;
            }
        }
        internal StructureDocumentType SDTType
        {
            get
            {
                return m_SDTType;
            }
            set
            {
                m_SDTType = value;
            }
        }
        internal string Label
        {
            get
            {
                return m_Label;
            }
            set
            {
                m_Label = value;
            }
        }
        internal LockSettings LockSettings
        {
            get
            {
                return m_LockSettings;
            }
            set
            {
                m_LockSettings = value;
            }
        }
        internal WCharacterFormat CharacterFormat
        {
            get
            {
                return m_CharacterFormat;
            }
            set
            {
                m_CharacterFormat = value;
            }
        }
        internal bool IsShowingPlaceHolder
        {
            get
            {
                return m_IsShowingPlaceHolder;
            }
            set
            {
                m_IsShowingPlaceHolder = value;
            }
        }
        internal uint TabIndex
        {
            get
            {
                return m_TabIndex;
            }
            set
            {
                m_TabIndex = value;
            }
        }
        internal string Tag
        {
            get
            {
                return m_Tag;
            }
            set
            {
                m_Tag = value;
            }
        }
        internal bool IsTemporary
        {
            get
            {
                return m_IsTemporary;
            }
            set
            {
                m_IsTemporary = value;
            }
        }

        internal ContentRepeatingType ContentRepeatingType
        {
            get
            {
                return m_ContentRepeatingType;
            }
            set
            {
                m_ContentRepeatingType = value;
            }
        }
        #endregion

        #region Constructor
        internal SDTProperties(WordDocument doc)
        {
            m_CharacterFormat = new WCharacterFormat(doc);
        }
        #endregion
    }
    
}
