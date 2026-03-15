// <copyright file="CursorLayer.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Syncfusion.Windows.Edit
{
    /// <summary>
    /// CursorLayer class is used as an adorner for cursor. The adorner gets invalidated
    /// when the caretposition property is changed.
    /// </summary>
    /// <remarks>
    /// CursorLayer class contains the Cursor properties like the cursor index, cursor
    /// line.
    /// </remarks>
    /// Syncfusion.Windows.Edit.ICursor
#if SyncfusionFramework4_0

    [DesignTimeVisible(false)]
#endif
    internal class CursorLayer : Adorner, ICursor
    {
        #region Dependency Properties

        /// <summary>
        /// DependencyProperty for CaretPosition
        /// </summary>
        private static readonly DependencyProperty CaretPositionProperty = DependencyProperty.Register("CaretPosition", typeof(Point), typeof(CursorLayer), new FrameworkPropertyMetadata(new Point(-1, 0), new PropertyChangedCallback(OnCaretPositionChanged)));

        #endregion Dependency Properties

        #region Local Variables

        /// <summary>
        /// instance for CursorIndex property.
        /// </summary>
        private int mindex;

        /// <summary>
        /// instance for LineNumber property.
        /// </summary>
        private int mline;

        /// <summary>
        /// instance for LineItem property, in which cursor is placed
        /// </summary>
        private LineItem mlineitem;

        #endregion Local Variables

        #region DP Setters and Getters

        /// <summary>
        /// Gets or sets the cursor position
        /// </summary>
        /// <remarks>CaretPosition specifies the Caret position of the cursor.</remarks>
        internal Point CaretPosition
        {
            get
            {
                return (Point)GetValue(CaretPositionProperty);
            }

            set
            {
                SetValue(CaretPositionProperty, value);
            }
        }

        #endregion DP Setters and Getters

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the  class.
        /// </summary>
        /// <remarks>
        /// <para>The Cursor Layer constructor.</para>
        /// </remarks>
        /// <param name="adornedElement">Gets the UIElement object from the reporting
        /// source.</param>
        public CursorLayer(UIElement adornedElement)
            : base(adornedElement)
        {
            this.Loaded += new RoutedEventHandler(CursorLayer_Loaded);
            this.LineItemHost = (LineItem)adornedElement;
            this.IsHitTestVisible = false;
            this.SnapsToDevicePixels = true;
        }

        #endregion Initialization

        #region CLR Properties

        /// <summary>
        /// Gets or sets the lineitem in which cursor is placed.
        /// </summary>
        /// <value>
        /// Type: <see
        /// cref="T:Syncfusion.Windows.Edit.LineItem">Syncfusion.Windows.Edit.LineItem.</see>
        /// </value>
        /// <remarks>LineItemHost specifies the Lineitem with its line properties.</remarks>
        internal LineItem LineItemHost
        {
            get
            {
                return mlineitem;
            }

            set
            {
                mlineitem = value;
            }
        }

        #endregion CLR Properties

        #region Overrides

        /// <summary>
        /// OnRender method to render the cursor
        /// </summary>
        /// <param name="drawingContext">The drawingContext with the type of DrawingContext</param>
        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            if (LineItemHost.ParentControl == null)
            {
                throw new Exception("EditControl Not Loaded");
            }

            if (LineItemHost.ParentControl.LineHeight > 0)
            {
                drawingContext.DrawRectangle(LineItemHost.ParentControl.CaretBrush, new Pen(LineItemHost.ParentControl.CaretBrush, 0.1d), new Rect(CaretPosition, new Size(1, LineItemHost.ParentControl.LineHeight - 1)));
                base.OnRender(drawingContext);
            }
        }

        #endregion Overrides

        #region Events

        /// <summary>
        /// Applying a double animation to provide the blinking effect
        /// </summary>
        /// <param name="sender">Gets the sender object from the reporting source</param>
        /// <param name="e">Gets the RoutedEventArgs object from the reporting
        /// source</param>
        private void CursorLayer_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.5d));
            animation.AutoReverse = true;
            animation.From = 1d;
            animation.To = 0d;
            animation.RepeatBehavior = RepeatBehavior.Forever;
            this.BeginAnimation(Adorner.OpacityProperty, animation);
        }

        /// <summary>
        /// HorizontalScrollbar is updated to adjust based on the cursor position
        /// </summary>
        /// <param name="d">DepdendencyObject, returns CursorLayer</param>
        /// <param name="e">DependencyPropertyChangedEventArgs, returns old and new
        /// value</param>
        private static double value = 10;

        private static void OnCaretPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CursorLayer cursorlayer = (CursorLayer)d;
            if (cursorlayer.LineItemHost.ParentControl != null && cursorlayer.LineItemHost.ParentControl.ScrollControl.Caret != null)
            {
                double tempx = cursorlayer.LineItemHost.ParentControl.ScrollControl.Caret.CaretPosition.X;

                if ((int)tempx >= (int)cursorlayer.LineItemHost.ParentControl.ScrollControl.ViewportWidth + cursorlayer.LineItemHost.ParentControl.ScrollControl.HorizontalOffset)
                {
                    var offsetValue = ((cursorlayer.LineItemHost.ParentControl.ScrollControl.Caret.CaretPosition.X - cursorlayer.LineItemHost.ParentControl.ScrollControl.ViewportWidth) / 2);
                    var horOffset = cursorlayer.LineItemHost.ParentControl.ScrollControl.HorizontalOffset;

                    if (cursorlayer.LineItemHost.ParentControl.ScrollControl.HorizontalOffset > 15)
                        value = value + 10;

                    offsetValue = offsetValue + value;
                    cursorlayer.LineItemHost.ParentControl.ScrollControl.SetHorizontalOffset(offsetValue);
                }
                else if ((int)cursorlayer.CaretPosition.X <= (int)cursorlayer.LineItemHost.ParentControl.ScrollControl.HorizontalOffset)
                {
                    cursorlayer.LineItemHost.ParentControl.ScrollControl.SetHorizontalOffset(cursorlayer.CaretPosition.X);
                }
            }

            cursorlayer.InvalidateVisual();
        }

        #endregion Events

        #region ICursor Members

        /// <summary>
        /// Gets or sets current index
        /// </summary>
        /// <remarks>
        /// <para>Specifies the cursor index.</para>
        /// </remarks>
        /// <value>
        /// <para>Type: System.Int32 </para>
        /// </value>
        public int CursorIndex
        {
            get { return mindex; }
            set { mindex = value; }
        }

        /// <summary>
        /// Gets or sets current linenumber.
        /// </summary>
        /// <remarks>
        /// <para>Specifies the current line number.</para>
        /// </remarks>
        /// <value>
        /// <para>Type: System.Int32 </para>
        /// </value>
        public int LineNumber
        {
            get { return mline; }
            set { mline = value; }
        }

        /// <summary>
        /// Method to move the cursor to specified number of characters. here it denotes the
        /// number of characters to be moved.
        /// </summary>
        /// <remarks>
        /// <para>The MoveTo method moves the cursor position into original index value,
        /// which is specified index value. And also set the caret position.</para>
        /// </remarks>
        /// <param name="i">Gets the int value from the reporting source</param>
        /// <returns>
        /// <para>Returns the CursorIndex at the specified position.</para>
        /// </returns>
        /// <example>
        /// <para>int a = MoveTo(20);</para>
        /// </example>
        public int MoveTo(int i)
        {
            if (mlineitem != null)
            {
                string text = mlineitem.Text;
                if (!mlineitem.IsExpanded)
                {
                    text = this.mlineitem.ParentControl.CurrentLanguage.GetCollapsedItemText(mlineitem);
                    BlockListener block = mlineitem.GetPreprocessorType();

                    if (block != null && block.IsPreprocessor)
                    {
                        int prefixlength = mlineitem.Text.IndexOf(block.BlockStart);
                        if (mindex == prefixlength && i > 0)
                        {
                            return this.MoveToEnd();
                        }
                        else
                        {
                            if (i < 0 && mindex == text.Length)
                            {
                                i = text.IndexOf(mlineitem.PreprocessorText);
                                double x = Utils.GetWidth(text.Substring(0, Math.Min(i, text.Length)), mlineitem.ParentControl.FontFamily, mlineitem.ParentControl.FontSize, mlineitem.ParentControl.Foreground);
                                this.CaretPosition = new Point(x, 0);
                                this.CursorIndex = i;
                            }
                            else if (text.Length > 0)
                            {
                                double x = Utils.GetWidth(text.Substring(0, Math.Min((mindex + i), text.Length)), mlineitem.ParentControl.FontFamily, mlineitem.ParentControl.FontSize, mlineitem.ParentControl.Foreground);
                                this.CaretPosition = new Point(x, 0);
                                this.CursorIndex = mindex + i;
                            }
                        }
                    }
                    else if ((block != null && !block.IsPreprocessor) || block == null)
                    {
                        LanguageBase language = mlineitem.ParentControl.CurrentLanguage;
                        string collapsedItemText = language.GetCollapsedItemText(mlineitem);
                        int ellipsisIndex = collapsedItemText.IndexOf(language.EllipsisText);
                        if (mindex == mlineitem.Text.Length && i > 0)
                        {
                            return this.MoveToEnd();
                        }
                        else if (i < 0 && mindex == ellipsisIndex + language.EllipsisText.Length)
                        {
                            double x = Utils.GetWidth(collapsedItemText.Substring(0, Math.Min(ellipsisIndex, collapsedItemText.Length)), mlineitem.ParentControl.FontFamily, mlineitem.ParentControl.FontSize, mlineitem.ParentControl.Foreground);
                            this.CaretPosition = new Point(x, 0);
                            this.CursorIndex = ellipsisIndex;
                        }
                        else if (i > 0 && mindex == ellipsisIndex)
                        {
                            double x = Utils.GetWidth(collapsedItemText.Substring(0, Math.Min((mindex + language.EllipsisText.Length), collapsedItemText.Length)), mlineitem.ParentControl.FontFamily, mlineitem.ParentControl.FontSize, mlineitem.ParentControl.Foreground);
                            this.CaretPosition = new Point(x, 0);
                            this.CursorIndex = mindex + language.EllipsisText.Length;
                        }
                        else
                        {
                            if (text.Length > 0)
                            {
                                double x = Utils.GetWidth(text.Substring(0, Math.Min((mindex + i), text.Length)), mlineitem.ParentControl.FontFamily, mlineitem.ParentControl.FontSize, mlineitem.ParentControl.Foreground);
                                this.CaretPosition = new Point(x, 0);
                                this.CursorIndex = mindex + i;
                            }
                        }
                    }
                }
                else
                {
                    if (mindex > text.Length)
                    {
                        return this.MoveToEnd();
                    }
                    else
                    {
                        if ((mindex + i) > 0 && text.Length > 0)
                        {
                            double x = Utils.GetWidth(text.Substring(0, Math.Min((mindex + i), text.Length)), mlineitem.ParentControl.FontFamily, mlineitem.ParentControl.FontSize, mlineitem.ParentControl.Foreground);
                            this.CaretPosition = new Point(x, 0);
                            this.CursorIndex = mindex + i;
                        }
                        else
                        {
                            return this.MoveToBegin();
                        }
                    }
                }
            }

            return CursorIndex;
        }

        /// <summary>
        /// Show the current visibility of Cursor.
        /// </summary>
        /// <remarks>
        /// <para>Show method Visible the current source from its hide visibility mode.  It
        /// mainly used to visible the cursor.</para>
        /// </remarks>
        /// <example>
        /// <para>Show();</para>
        /// </example>
        public void Show()
        {
            this.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Hide the current visibility of Cursor.
        /// </summary>
        /// <remarks>
        /// <para>Hide method hide the current source from its show visibility mode.  It
        /// mainly used to hide the cursor.</para>
        /// </remarks>
        /// <example>
        /// <para>Hide();</para>
        /// </example>
        public void Hide()
        {
            this.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Moves the cursor to the start of the line
        /// </summary>
        /// <remarks>
        /// The MoveToBegin method moves the cursor pointer at the beginning of the line.
        /// </remarks>
        /// <returns>
        /// Returns the CursorIndex at the beginning of the line
        /// </returns>
        /// <example>
        /// <para>int a = MoveToBegin();</para>
        /// </example>
        public int MoveToBegin()
        {
            if (mlineitem != null)
            {
                this.CaretPosition = new Point(0, 0);
                this.CursorIndex = 0;
            }

            return CursorIndex;
        }

        /// <summary>
        /// Moves the cursor to the end of line
        /// </summary>
        /// <remarks>
        /// <para>The MoveToEnd method moves the cursor position at the end of the
        /// line</para>
        /// </remarks>
        /// <returns>
        /// <para>Returns the CursorIndex at the end of the line</para>
        /// </returns>
        /// <example>
        /// <para>int a = MoveToEnd();</para>
        /// </example>
        public int MoveToEnd()
        {
            if (mlineitem != null)
            {
                string lineItemText = !mlineitem.IsExpanded ? this.mlineitem.ParentControl.CurrentLanguage.GetCollapsedItemText(mlineitem) : mlineitem.Text;
                if (lineItemText.Length > 0)
                {
                    double x = Utils.GetWidth(lineItemText, mlineitem.ParentControl.FontFamily, mlineitem.ParentControl.FontSize, mlineitem.ParentControl.Foreground);
                    this.CaretPosition = new Point(x, 0);
                    this.CursorIndex = lineItemText.Length;
                }
                else
                {
                    this.CursorIndex = MoveToBegin();
                }
            }

            return CursorIndex;
        }

        /// <summary>
        /// Helper method to move the cursor to specified index. parameter newindex is used
        /// to specify the target index.
        /// </summary>
        /// <remarks>
        /// <para>This method moves the cursor pointer at the new index position.</para>
        /// </remarks>
        /// <param name="newIndex">Gets the new index of the cursor from the reporting
        /// source</param>
        /// <returns>
        /// <para>Returns the CursorIndex at the particular loacation</para>
        /// </returns>
        /// <example>
        /// <para>int a = MoveToLocation(15);</para>
        /// </example>
        public int MoveToLocation(int newIndex)
        {
            string lineItemText = mlineitem.IsExpanded ? mlineitem.Text : mlineitem.ParentControl.CurrentLanguage.GetCollapsedItemText(mlineitem);
            if (newIndex <= 0)
            {
                this.CursorIndex = MoveToBegin();
                return CursorIndex;
            }

            if (newIndex < lineItemText.Length)
            {
                double x = Utils.GetWidth(lineItemText.Substring(0, newIndex), mlineitem.ParentControl.FontFamily, mlineitem.ParentControl.FontSize, mlineitem.ParentControl.Foreground);
                this.CaretPosition = new Point(x, 0);
                this.CursorIndex = newIndex;
            }
            else
            {
                this.CursorIndex = MoveToEnd();
            }

            return CursorIndex;
        }

        #endregion ICursor Members
    }
}