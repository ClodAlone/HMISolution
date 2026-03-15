#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Collections.Generic;

#if !WPF
using System.Windows.Browser;
#endif

using System.Text;

namespace Syncfusion.Windows.Tools.Controls
{
    public abstract class ElementBox
    {
        private ElementBox nextElementBox = null;

        private ElementBox previousElementBox = null;

        private Rect boundingRectangle = Rect.Empty;

        private LineInfo lineInfo;

        private Point elementLocation;

        private Size elementSize = Size.Empty;

        internal double CalculatedLineSpacing = 0;

        internal double AfterSpacing = 0;

        internal double BeforeSpacing = 0;

        internal double Offset = 0;

        private FrameworkElement element;

        internal double baselineOffset = 0;

        internal bool IsAddedToLine = true;

        private string internalText = string.Empty;

        internal bool IsUIBox = false;

        internal bool IsImageBox = false;

        private Inline inline;

        /// <summary>
        /// Initializes the new instance of LayoutBox
        /// </summary>
        public ElementBox()
        {

        }

        /// <summary>
        /// Initializes the new instance of LayoutBox
        /// </summary>
        /// <param name="uiElement"></param>
        public ElementBox(FrameworkElement element)
        {

        }

        internal double BaselineOffset
        {
            get
            {
                return baselineOffset;
            }
            set
            {
                baselineOffset = value;
            }
        }

        /// <summary>
        /// Gets or Sets the UIElement
        /// </summary>
        public FrameworkElement Element
        {
            get
            {
                return element;
            }
            set
            {
                element = value;
            }
        }

        public string InternalText
        {
            get
            {
                return internalText;
            }
            set
            {
                internalText = value;

                OnTextChanged();
            }
        }

        /// <summary>
        /// Gets or Sets the next Element Box
        /// </summary>
        public ElementBox NextElementBox
        {
            get
            {
                return nextElementBox;
            }
            set
            {
                nextElementBox = value;
                if (value != null)
                {
                    value.PreviousElementBox = this;
                }
            }
        }

        internal Rect InternalRect
        {
            get;
            set;
        }

        /// <summary>
        /// Identifies whether it is a white color
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        internal bool IsWhiteColor(Color c)
        {
            return c.A == 0 && c.B == 0 && c.G == 0 && c.R == 0;
        }

        /// <summary>
        /// Gets or Sets the previous Element Box
        /// </summary>
        public ElementBox PreviousElementBox
        {
            get
            {
                return previousElementBox;
            }
            set
            {
                previousElementBox = value;
            }
        }

        /// <summary>
        /// Gets or Sets the bounding rectangle
        /// </summary>
        public Rect BoundingRectangle
        {
            get
            {
                return boundingRectangle;
            }
            set
            {
                boundingRectangle = value;
                SetElementPosition();
            }
        }

        /// <summary>
        /// Gets or Sets the inline
        /// </summary>
        public Inline Inline
        {
            get
            {
                return inline;
            }
            set
            {
                inline = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Point ElementLocation
        {
            get
            {
                return elementLocation;
            }
            set
            {
                elementLocation = value;
            }
        }

        /// <summary>
        /// Gets the location of the element box
        /// </summary>
        public Point Location
        {
            get
            {
                return new Point(boundingRectangle.X, boundingRectangle.Y);
            }
        }

        protected virtual void OnTextChanged()
        {

        }

        public virtual void ArrangeLinesInSpnnedBoxes()
        {

        }

        internal virtual ElementBox CreateElementBox()
        {
            return null;
        }


        internal double DesiredWidth
        {
            get;
            set;
        }
        /// <summary>
        /// Adds the decorative elements to the page
        /// </summary>
        /// <param name="uiElement"></param>
        internal void AddDecorationToPage(UIElement uiElement)
        {
            if (LineInfo != null)
            {
                LayoutViewer viewer = Inline.Paragraph.LayoutViewer;
                if (viewer != null)
                {
                    PageAdv page = viewer.Pages[LineInfo.PageIndex];
                    if (!page.DecorationContainer.Children.Contains(uiElement))
                    {
                        page.DecorationContainer.Children.Add(uiElement);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or Sets the line information
        /// </summary>
        public LineInfo LineInfo
        {
            get
            {
                return lineInfo;
            }
            set
            {
                lineInfo = value;
            }
        }

        /// <summary>
        /// Gets the size of the LayoutBox
        /// </summary>
        public Size Size
        {
            get
            {
                if (!boundingRectangle.IsEmpty)
                {
                    return new Size(boundingRectangle.Width, boundingRectangle.Height);
                }
                return new Size(0.0, 0.0);
            }
        }

        /// <summary>
        /// Gets the element size
        /// </summary>
        public Size ElementSize
        {
            get
            {
                if (Element != null)
                {
                    if (this is ImageElementBox || this is UIElementBox)
                        return new Size(Element.Width, Element.Height);
#if WPF
                    return new Size(Element.DesiredSize.Width, Element.DesiredSize.Height);
#endif
#if !WPF
                    return new Size(Element.ActualWidth, Element.ActualHeight);
#endif

                }
                return elementSize;
            }
            set
            {
                elementSize = value;
            }
        }

        public virtual void Render()
        {

        }

        internal void SplitLongText(double width)
        {
            if (!IsUIBox && !IsImageBox)
            {
                string text = GetWord(width, InternalText);
                string remainText = InternalText.Substring(text.Length);
                InternalText = text;
                if (!string.IsNullOrEmpty(remainText))
                {
                    ElementBox newBox = CreateElementBox();
                    newBox.InternalText = remainText;
                    newBox.Inline = Inline;
                    newBox.MeasureSize();
                    int index = Inline.ElementBoxes.IndexOf(this);
                    if (index != Inline.ElementBoxes.Count - 1)
                    {
                        Inline.ElementBoxes.Insert(index + 1, newBox);
                    }
                    else
                    {
                        Inline.ElementBoxes.Add(newBox);
                    }
                }
            }
        }

        internal string GetWord(double width, string longText)
        {
            int i = 0;
            string text = string.Empty;
            while (i < longText.Length)
            {
                text = longText.Substring(0, i + 1);
                if (Math.Floor(TextHelper.MeasureText(text, this).Width) >= Math.Floor(width))
                {
                    if (Math.Floor(TextHelper.MeasureText(text, this).Width) > Math.Floor(width))
                    {
                        text = longText.Substring(0, i);
                    }
                    break;
                }
                i++;
            }

            return text;
        }

        internal void SetText(string str)
        {
            internalText = str;
        }

        /// <summary>
        /// Sets the elements position
        /// </summary>
        internal abstract void SetElementPosition();

        internal virtual void MeasureSize()
        {

        }

        internal abstract Point GetApproxRight(int index);
    }
}
