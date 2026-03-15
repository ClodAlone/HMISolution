#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;

namespace Syncfusion.XlsIO.Implementation
{
    public class CustomXmlSchemaCollection:ICustomXmlSchemaCollection
    {

        #region ICustomXmlSchemaCollection Members

        private readonly List<string> m_Items = new List<string>();

        public int Count
        {
            get { return this.m_Items.Count; }
        }

        public string this[int index]
        {
            get
            {
                return this.m_Items[index];
            }
            set
            {
                this.m_Items[index] = value;
            }
        }

        public void Add(string name)
        {
            this.m_Items.Add(name);
        }

        public void Clear()
        {
            this.m_Items.Clear();
        }

        public ICustomXmlSchemaCollection Clone()
        {
            CustomXmlSchemaCollection schemas = new CustomXmlSchemaCollection();
            foreach (string str in this)
            {
                schemas.Add(str);
            }
            return schemas;
        }

        public int IndexOf(string value)
        {
            return this.m_Items.IndexOf(value);
        }

        public void Remove(string name)
        {
             this.m_Items.Remove(name);
        }

        public void RemoveAt(int index)
        {
             this.m_Items.RemoveAt(index);
        }

        #endregion

        #region IEnumerable Members

        public System.Collections.IEnumerator GetEnumerator()
        {
            return this.m_Items.GetEnumerator();
        }

        #endregion
    }
}
