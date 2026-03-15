#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;

#if !WinRT
using Syncfusion.Windows.Diagnostics;

namespace Syncfusion.Windows.Controls.Scroll
#else
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Syncfusion.WinRT.Controls.Scroll
#endif
{
    /// <summary>
    /// Implements a child frame in a <see cref="ScrollControl"/> that can be placed at the top, bottom,
    /// left and right side of the control so that contents do scroll similiar to the Internet Explorer
    /// frames concept. Each frame remembers its placement.<para/>
    /// Adding and removing elements from the children collection does not trigger
    /// calls to InvalidateMeasure. This allows adding and removing elements
    /// on the fly. A derived control is responsible to call Measure and Arrange
    /// on child elements since this base class will not do this by itsself.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class ScrollControlChildFrame : VisualContainer, IDisposable
    {
        #region Fields
        int rowSection;
        int columnSection;
        private bool isAtTop;
        private bool isAtLeftSide;
        private bool isAtBottom;
        private bool isAtRightSide;
        private bool isScrollFrame;
        public bool IsArrangeDirty { get; internal set; }
        #endregion

        public ScrollControlChildFrame()
        {
        }

        public override string ToString()
        {
            if (Name == "")
                return String.Format("{0} (RowSection={1}, ColumnSection={2})", GetType().Name, RowSection, ColumnSection);
            return base.ToString();
        }

        #region Section: Header/Footer-, Row/Column
        /// <summary>
        /// Sets the state.
        /// </summary>
        /// <param name="isAtLeftSide">if set to <c>true</c> frame is at left side.</param>
        /// <param name="isAtTop">if set to <c>true</c> frame is at top.</param>
        /// <param name="isAtRightSide">if set to <c>true</c> is at right side.</param>
        /// <param name="isAtBottom">if set to <c>true</c> frame is at bottom.</param>
        public void SetState(bool isAtLeftSide, bool isAtTop, bool isAtRightSide, bool isAtBottom)
        {
            //? SetWantsMouseInput(this, true);
            this.isScrollFrame = true;
            this.isAtTop = isAtTop;
            this.isAtLeftSide = isAtLeftSide;
            this.isAtBottom = isAtBottom;
            this.isAtRightSide = isAtRightSide;

            columnSection = isAtLeftSide && IsAtRightSide ? 3 : isAtLeftSide ? 0 : IsAtRightSide ? 2 : 1;
            rowSection = isAtTop && isAtBottom ? 3 : isAtTop ? 0 : IsAtBottom ? 2 : 1;
        }

        /// <summary>
        /// Determines if this child frame that matches the given parameters.
        /// </summary>
        /// <param name="isAtLeftSide">if set to <c>true</c> frame is at left side.</param>
        /// <param name="isAtTop">if set to <c>true</c> frame is at top.</param>
        /// <param name="isAtRightSide">if set to <c>true</c> is at right side.</param>
        /// <param name="isAtBottom">if set to <c>true</c> frame is at bottom.</param>
        /// <returns>true if state matches; fakse otherwise.</returns>
        public bool CompareState(bool isAtLeftSide, bool isAtTop, bool isAtRightSide, bool isAtBottom)
        {
            return this.isScrollFrame &&
                this.isAtTop == isAtTop &&
                this.isAtLeftSide == isAtLeftSide &&
                this.isAtBottom == isAtBottom &&
                this.isAtRightSide == isAtRightSide;
        }

        /// <summary>
        /// The row section with 0 being at the left side, 1 being the center, 2 being at the right side and 3 spanning across multiple sections.
        /// </summary>
        public int RowSection
        {
            get
            {
                return rowSection;
            }
        }

        /// <summary>
        /// The column section with 0 being at the top side, 1 being the center, 2 being at the bottom and 3 spanning across multiple sections.
        /// </summary>
        public int ColumnSection
        {
            get
            {
                return columnSection;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is at top.
        /// </summary>
        /// <value><c>true</c> if this instance is at top; otherwise, <c>false</c>.</value>
        public bool IsAtTop
        {
            get
            {
                return this.isAtTop;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is at left side.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is at left side; otherwise, <c>false</c>.
        /// </value>
        public bool IsAtLeftSide
        {
            get
            {
                return this.isAtLeftSide;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is at bottom.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is at bottom; otherwise, <c>false</c>.
        /// </value>
        public bool IsAtBottom
        {
            get
            {
                return this.isAtBottom;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is at right side.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is at right side; otherwise, <c>false</c>.
        /// </value>
        public bool IsAtRightSide
        {
            get
            {
                return this.isAtRightSide;
            }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="SetState"/> has been called.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this <see cref="SetState"/> was called; otherwise, <c>false</c>.
        /// </value>
        public bool IsScrollFrame
        {
            get { return isScrollFrame; }
        }
        
        #endregion

        protected override Size ArrangeOverride(Size finalSize)
        {
#if (SILVERLIGHT || WinRT)
            if (IsArrangeDirty)
            {
                //Trace.Write("ArrangeOverride: " + ToString());
                foreach (UIElement el in Children)
                {
                    Rect rect = VisualContainer.GetRenderBounds(el);
                    if (rect.IsEmpty)
                        continue;

                    //#if TRACE
                    //                string c = "";
                    //                TextBox cc = el as TextBox;
                    //                if (cc != null) c = cc.Text;
                    //                Trace.Write(String.Format("Arrange {0}: {1} - {2}", el.ToString(), rect, c));
                    //                TranslateTransform translate = el.RenderTransform as TranslateTransform;
                    //                if (translate != null)
                    //                    Trace.Write(String.Format("     Translate {0},{1}", translate.X, translate.Y));
                    //#endif

                    el.Arrange(rect);
                }
                IsArrangeDirty = false;
            }
#endif
            return finalSize;
        }


        public void Dispose()
        {
            this.Children.Clear();
        }
    }

}
