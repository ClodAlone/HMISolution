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
    internal class DocPartItem
    {
        #region Fields
        private string m_docPartCategory;
        private string m_docPartGallery;
        private bool m_bIsDocPartUnique;
        # endregion

        # region Properties
        internal string DocPartCategory
        {
            get
            {
                return m_docPartCategory;
            }
            set
            {
                m_docPartCategory = value;
            }
        }
        internal string DocPartGallery
        {
            get
            {
                return m_docPartGallery;
            }
            set
            {
                m_docPartGallery = value;
            }
        }
        internal bool IsDocPartUnique
        {
            get
            {
                return m_bIsDocPartUnique;
            }
            set
            {
                m_bIsDocPartUnique = value;
            }
        }
        # endregion
    }
    internal class DocPartList : DocPartItem
    {
        internal DocPartList()
        {
            //Empty Constructor
        }
    }

    internal class DocPartObj : DocPartItem
    { 
        internal DocPartObj()
        {
            //Empty Constructor
        }
    }
}
