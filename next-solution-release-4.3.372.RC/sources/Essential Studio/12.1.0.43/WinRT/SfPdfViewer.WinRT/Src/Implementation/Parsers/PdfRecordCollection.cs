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
using System.Collections;

namespace Syncfusion.Pdf
{
    class PdfRecordCollection: IEnumerable
    {
        private List<PdfRecord> m_recordCollection;

        internal PdfRecordCollection()
        {
            m_recordCollection = new List<PdfRecord>();
        }

        public void Add(PdfRecord record)
        {
            m_recordCollection.Add(record);
        }

        public IEnumerator GetEnumerator()
        {
            return m_recordCollection.GetEnumerator();
        }
    }
}
