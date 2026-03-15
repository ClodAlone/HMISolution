#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Syncfusion.Design.Controls
{
    public class ImagePickerItem
    {
        private Uri m_uri;
        private string m_name;
        private bool m_isFolder;
        private IList<ImagePickerItem> m_items;

        public IList<ImagePickerItem> Items
        {
            get
            {
                return m_items;
            }
        }

        public bool IsFolder
        {
            get
            {
                return m_isFolder;
            }
        }

        public Uri Uri
        {
            get
            {
                return m_uri;
            }            
        }

        public string Name
        {
            get
            {
                return m_name;
            }   
        }
    }
}
