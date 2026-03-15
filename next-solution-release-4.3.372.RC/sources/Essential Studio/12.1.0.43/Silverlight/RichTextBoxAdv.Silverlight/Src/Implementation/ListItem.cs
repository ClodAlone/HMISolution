#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

namespace Syncfusion.Windows.Tools.Controls
{
    public class ListItem
    {
        private ParagraphAdv associatedParagraph;
        internal FrameworkElement Item;
        public double Size = 0d;
        public double ItemSize = 0;
        private readonly double LISTITEMPERCENT = 24.07;
        private ListType listType;
        internal Image Image;
        internal LineInfo Line = null;
        internal double FontSize = 12;
        private double baseline = 0;

        /// <summary>
        /// Gets or Sets the ListType of this item
        /// </summary>
        public ListType ListType
        {
            get
            {
                return listType;
            }
            internal set
            {
                listType = value;
            }
        }

        /// <summary>
        /// Gets or Sets the list number of the item
        /// </summary>
        public double ListNumber
        {
            get
            {
                return associatedParagraph.ListNumber;
            }
        }

        public ListItem(ParagraphAdv paragraph)
        {
            if (paragraph != null)
            {
                associatedParagraph = paragraph;
                ListType = paragraph.ListType;
                paragraph.AssociatedListItem = this;
            }
            else
            {
                throw new ArgumentNullException("paragraph");
            }

            //Image = new Image();
            //transform = new TranslateTransform();
        }

        /// <summary>
        /// Creates the bullet item
        /// </summary>
        private void CreateBulletItem()
        {
            ItemSize = Math.Round((LISTITEMPERCENT * Size) / 100);
            Ellipse ellipse = new Ellipse();
            ellipse.Width = ItemSize;
            ellipse.Height = ItemSize;
            ellipse.Fill = new SolidColorBrush(Colors.Black);
            if (Item != null && Item.Parent is Canvas)
                (Item.Parent as Canvas).Children.Remove(Item);
            Item = ellipse;
        }

        /// <summary>
        /// Creates the number item
        /// </summary>
        /// <param name="number"></param>
        private void CreateNumberItem()
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = ListNumber.ToString() + ".";
            textBlock.FontSize = GetMinFontSize();
#if WPF
            textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
#endif
            ItemSize = textBlock.ActualHeight;
            if (Item != null && Item.Parent is Canvas)
                (Item.Parent as Canvas).Children.Remove(Item);
            Item = textBlock;
            baseline = textBlock.BaselineOffset;
        }

        internal double GetMinFontSize()
        {
            double fontsize = FontSize;
            foreach (Inline inline in associatedParagraph.Inlines)
            {
                if (inline is SpanAdv)
                {
                    if (associatedParagraph.Inlines.First() == inline)
                    {
                        fontsize = (inline as SpanAdv).FontSize;
                        continue;
                    }
                    Math.Min(fontsize, (inline as SpanAdv).FontSize);

                }
                else if (inline is HyperlinkAdv)
                {
                    if (associatedParagraph.Inlines.First() == inline)
                    {
                        fontsize = (inline as HyperlinkAdv).FontSize;
                        continue;
                    }
                    Math.Min(fontsize, (inline as HyperlinkAdv).FontSize);
                }
            }
            return fontsize;
        }

        /// <summary>
        /// Creates the list item
        /// </summary>
        /// <returns></returns>
        private UIElement CreateItem()
        {
            if (ListType == ListType.Bulleted)
                CreateBulletItem();
            if (ListType == ListType.Numbered)
                CreateNumberItem();

            return Item;
        }

        /// <summary>
        /// Sets the position of the item
        /// </summary>
        /// <param name="point"></param>
        public void SetPosition(Point point)
        {
            try
            {
                double y = 0;
                double top = Size - ItemSize;
                double baseLine = Line.Height - Line.AfterSpacing - Line.CalculatedLineSpace - Line.Offset - ItemSize;
                baseLine = baseLine - 2 > 0 ? baseLine - 2 : baseLine;
                if (ListType == ListType.Bulleted)
                    //y = (point.Y + Size / 2) - (ItemSize / 2);
                    y = point.Y + baseLine;
                if (ListType == ListType.Numbered)
                    y = point.Y + (Line.Height - Line.AfterSpacing - Line.CalculatedLineSpace - Line.Offset - baseline);
                //y = point.Y + (Size - baseline);
                Canvas.SetLeft(Item, point.X);
                Canvas.SetTop(Item, y);
            }
            catch { }
        }

        public double GetBulletSize(double size)
        {
            return Math.Round((LISTITEMPERCENT * size) / 100);
        }

        public UIElement Render(Point point)
        {
            CreateItem();
            SetPosition(point);
            return Item;
        }

        public void ValidateListNumber(Point point)
        {
            if (associatedParagraph != null && associatedParagraph.PreviousBlock != null)
            {
                if (associatedParagraph.PreviousBlock is ParagraphAdv && associatedParagraph.ListNumber == (associatedParagraph.PreviousBlock as ParagraphAdv).ListNumber + 1)
                {
                    Render(point);
                }
                else
                {
                    SetPosition(point);
                }
            }
        }
    }
}
