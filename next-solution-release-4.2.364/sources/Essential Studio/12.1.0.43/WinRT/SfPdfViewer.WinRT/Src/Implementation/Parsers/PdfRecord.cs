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
using System.Diagnostics;

namespace Syncfusion.Pdf
{
    [DebuggerDisplay("({OperatorName}, operands={Operands.Length})")]
    class PdfRecord
    {
        private int m_operatorName;
        private string[] m_operands;

        internal int OperatorName
        {
            get
            {
                return m_operatorName;
            }
            set
            {
                m_operatorName = value;
            }
        }

        internal string[] Operands
        {
            get
            {
                return m_operands;
            }
            set
            {
                m_operands = value;
            }
        }

        public PdfRecord(int name, string[] operands)
        {
            m_operatorName = name;
            m_operands = operands;
        }
    }
}