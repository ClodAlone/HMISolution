#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WINDOWS || MVC

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Syncfusion.PdfViewer.Base
{
    internal class GraphicObjectData
    {
        private Color m_strokingColorspace;
        private Color m_nonStokingColorspace;
        private Font m_font;
        private string m_currentFont;
        private float m_fontSize;
        private float m_textLeading;

        internal Color StrokingColorspace
        {
            get
            {
                return m_strokingColorspace;
            }
            set
            {
                m_strokingColorspace = value;
            }
        }

        internal Color NonStrokingColorspace
        {
            get
            {
                return m_nonStokingColorspace;
            }
            set
            {
                m_nonStokingColorspace = value;
            }
        }

        internal Font Font
        {
            get
            {
                return m_font;
            }
            set
            {
                m_font = value;
            }
        }
        internal string CurrentFont
        {
            get
            {
                return m_currentFont;
            }
            set
            {
                m_currentFont = value;
            }
        }

        internal float FontSize
        {
            get
            {
                return m_fontSize;
            }
            set
            {
                m_fontSize = value;
            }
        }

        internal float TextLeading
        {
            get
            {
                return m_textLeading;
            }
            set
            {
                m_textLeading = value;
            }
        }

        internal GraphicObjectData()
        {
        }
    }
}
#elif WPF
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using Syncfusion.Windows.PdfViewer;

namespace Syncfusion.PdfViewer.Base
{
    internal class GraphicObjectData
    {
        private Color m_strokingColorspace;
        private Color m_nonStokingColorspace;
        private PIFont m_font;
        private string m_currentFont;
        private float m_fontSize;

        internal Color StrokingColorspace
        {
            get
            {
                return m_strokingColorspace;
            }
            set
            {
                m_strokingColorspace = value;
            }
        }

        internal string CurrentFont
        {
            get
            {
                return m_currentFont;
            }
            set
            {
                m_currentFont = value;
            }
        }

        internal float FontSize
        {
            get
            {
                return m_fontSize;
            }
            set
            {
                m_fontSize = value;
            }
        }
        internal Color NonStrokingColorspace
        {
            get
            {
                return m_nonStokingColorspace;
            }
            set
            {
                m_nonStokingColorspace = value;
            }
        }

        internal PIFont Font
        {
            get
            {
                return m_font;
            }
            set
            {
                m_font = value;
            }
        }


        internal GraphicObjectData()
        {
        }
    }
}
#else
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Syncfusion.Windows.PdfViewer;

namespace Syncfusion.PdfViewer.Base
{
    internal class GraphicObjectData
    {
        private global::Windows.UI.Color m_strokingColorspace;
        private global::Windows.UI.Color m_nonStokingColorspace;
        private int m_clipCounts;
        private string m_currentFont;
        private float m_fontSize, m_textLeading;
        private int m_layerCounts;
        private PathRenderer m_clipPath = new PathRenderer();

        internal PathRenderer ClipPath
        {
            get
            {
                return m_clipPath;
            }
            set
            {
                m_clipPath = value;
            }
        }
        internal int ClipCounts
        {
            get
            {
                return m_clipCounts;
            }
            set
            {
                m_clipCounts = value;
            }
        }
        internal int LayerCounts
        {
            get
            {
                return m_layerCounts;
            }
            set
            {
                m_layerCounts = value;
            }
        }

        internal global::Windows.UI.Color StrokingColorspace
        {
            get
            {
                return m_strokingColorspace;
            }
            set
            {
                m_strokingColorspace = value;
            }
        }

        internal global::Windows.UI.Color NonStrokingColorspace
        {
            get
            {
                return m_nonStokingColorspace;
            }
            set
            {
                m_nonStokingColorspace = value;
            }
        }

        internal string CurrentFont
        {
            get
            {
                return m_currentFont;
            }
            set
            {
                m_currentFont = value;
            }
        }

        internal float FontSize
        {
            get
            {
                return m_fontSize;
            }
            set
            {
                m_fontSize = value;
            }
        }

        internal float TextLeading
        {
            get
            {
                return m_textLeading;
            }
            set
            {
                m_textLeading = value;
            }
        }

        internal GraphicObjectData()
        {
        }
    }
}
#endif
