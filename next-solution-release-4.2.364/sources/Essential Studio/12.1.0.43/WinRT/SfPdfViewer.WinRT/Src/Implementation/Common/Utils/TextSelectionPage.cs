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
using System.Threading.Tasks;
using Windows.UI.Xaml.Shapes;

namespace Syncfusion.Windows.PdfViewer
{
    internal class TextSelectionPage
    {
        internal int PageNumber;
        internal int WordCount;
        private Dictionary<Line, float> m_textSelectLineSequence;
        private Dictionary<float, Line> m_textSelectIndexSequence;
        private Dictionary<double, Line> m_textSelectionDifferentLines;
        internal Dictionary<Line, float> TextSelectLineSequence
        {
            get
            {
                return m_textSelectLineSequence;
            }
            set
            {
                m_textSelectLineSequence = new Dictionary<Line, float>(value);
            }
        }
        internal Dictionary<float, Line> TextSelectIndexSequence
        {
            get
            {
                return m_textSelectIndexSequence;
            }
            set
            {
                m_textSelectIndexSequence = new Dictionary<float, Line>(value);
            }
        }
        internal Dictionary<double, Line> TextSelectionDifferentLines
        {
            get
            {
                return m_textSelectionDifferentLines;
            }
            set
            {
                m_textSelectionDifferentLines = new Dictionary<double, Line>(value);
            }
        }
    }
}
