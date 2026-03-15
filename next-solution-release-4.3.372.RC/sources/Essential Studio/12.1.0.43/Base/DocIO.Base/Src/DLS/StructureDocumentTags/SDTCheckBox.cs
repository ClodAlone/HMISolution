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
    internal class SDTCheckBox
    {
        #region fields
        private bool m_isChecked;
        private CheckBoxState m_CheckedState;
        private CheckBoxState m_UnCheckedState;
        #endregion

        #region properties
        internal bool IsChecked
        {
            get
            {
                return m_isChecked;
            }
            set
            {
                m_isChecked = value;
            }
        }
        internal CheckBoxState CheckedState
        {
            get
            {
                return m_CheckedState;
            }
            set
            {
                m_CheckedState = value;
            }
        }
        internal CheckBoxState UncheckedState
        {
            get
            {
                return m_UnCheckedState;
            }
            set
            {
                m_UnCheckedState = value;
            }
        }
        #endregion

        #region Constructor
        internal SDTCheckBox()
        {
            m_CheckedState = new CheckBoxState();
            m_UnCheckedState = new CheckBoxState();
        }
        #endregion
    }
    internal class CheckBoxState
    {
        #region fields
        private string m_Font;
        private string m_Value;
        #endregion

        #region Properties
        internal string Font
        {
            get
            {
                return m_Font;
            }
            set
            {
                m_Font = value;
            }
        }
        internal string Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value;
            }
        }
        #endregion
    }
}
