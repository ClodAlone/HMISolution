#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.XlsIO.Implementation
{
    public class MetaPropertyImpl:IMetaProperty
    {
        private string m_value;
        private string m_elementName;
        private string m_displayName;
        private string m_internalName;
        private string m_nameSpaceURI;
        #region IMetaProperty Members

        public string Value
        {
            get 
            {
                return m_value;
            }
            set
            {
                m_value = value;
            }
        }

        public string Name
        {
            get 
            {
                return m_displayName;
            }
            set
            {
                m_displayName = value;
            }
        }

        internal string ElementName
        {
            get
            {
                return m_elementName;
            }
            set
            {
                m_elementName = value;
            }
        }
        internal string InternalName
        {
            get
            {
                return m_internalName;
            }
            set
            {
                m_internalName = value;
            }
        }
        internal string NameSpaceURI
        {
            get
            {
                return m_nameSpaceURI;
            }
            set
            {
                m_nameSpaceURI = value;
            }
        }

        #endregion

        
    }
}
