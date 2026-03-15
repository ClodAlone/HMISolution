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
    class SDTDataBinding
    {
 
        # region Fields
        private string m_prefixMapping;
        private string m_XPath;
        private string m_storeItemID;
        # endregion

        # region Properties
        internal string PrefixMapping
        {
            get
            {
                return m_prefixMapping;
            }
            set
            {
                m_prefixMapping = value;
            }
        }
        internal string XPath
        {
            get
            {
                return m_XPath;
            }
            set
            {
                m_XPath = value;
            }
        }
        internal string StoreItemID
        {
            get
            {
                return m_storeItemID;
            }
            set
            {
                m_storeItemID = value;
            }
        }
        # endregion

        #region Constructor
        internal SDTDataBinding()
        { }
        #endregion
    }
}
