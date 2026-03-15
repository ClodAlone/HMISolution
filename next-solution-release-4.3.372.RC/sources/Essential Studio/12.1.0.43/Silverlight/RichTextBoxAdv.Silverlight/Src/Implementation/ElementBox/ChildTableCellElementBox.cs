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
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Tools.Controls
{
    public class ChildTableCellElementBox :TableCellElementBox
    {
        public ChildTableCellElementBox(ElementBox box)
        {
            ParentBox = box;
        }

        internal override Point GetApproxRight(int index)
        {
            //throw new NotImplementedException();
            return new Point(0, 0);
        }

        internal override void SetElementPosition()
        {
            Point point = new Point(Location.X, Location.Y);

            ElementLocation = point;

            double y = point.Y;

            foreach (LineInfo line in LineInfos)
            {
                double xPos = line.BoundingRectangle.X;
                line.BoundingRectangle = new Rect(ElementLocation.X + 5d, y, line.Width, line.Height);
                ParagraphAdv paragraph = line.Block as ParagraphAdv;
                if (paragraph != null)
                {
                    ListItem item = paragraph.AssociatedListItem;
                    if (item != null && paragraph.ListType != ListType.None)
                    {
                        line.BoundingRectangle = new Rect(line.BoundingRectangle.X + 30d , y, line.Width, line.Height);
                        item.SetPosition(new Point(ElementLocation.X + 10d, paragraph.LineInfo.First().BoundingRectangle.Top));
                    }
                }
                if (LineInfo != null)
                {
                    line.PageIndex = LineInfo.PageIndex;
                }

                if (line.Block.IsTable)
                    line.ArrangeTableElementBoxes();
                else
                    line.ArrangeElementBoxes((line.Block as ParagraphAdv).TextAlignment, BoundingRectangle.Width);

                y += line.BoundingRectangle.Height;
            }
        }

        internal ElementBox GetParentCellBox()
        {
            ElementBox box = ParentBox;
            while (box is ChildTableCellElementBox)
            {
                box = (box as ChildTableCellElementBox).ParentBox;
            }
            return box;
        }

        internal LineInfo GetLineInfoFromPoint(Point p)
        {
            LineInfo lineInfo = null;
            Point newpoint = new Point(Math.Floor(p.X), p.Y);
            foreach (var line in LineInfos)
            {
                Rect newrect = new Rect(line.BoundingRectangle.X, line.BoundingRectangle.Y, Math.Ceiling(line.BoundingRectangle.Width), line.BoundingRectangle.Height);
                if (newrect.Contains(p))
                {
                    lineInfo = line;
                    break;
                }
                else if ((newpoint.X <= line.BoundingRectangle.Left || newpoint.X >= line.BoundingRectangle.Right) &&
                    (newpoint.Y >= line.BoundingRectangle.Top && newpoint.Y <= line.BoundingRectangle.Bottom) && BoundingRectangle.Contains(newpoint))
                {
                    lineInfo = line;
                    break;
                }
            }

            if (LineInfos.Count == 1)
            {
                if (lineInfo == null)
                {
                    if (newpoint.Y >= LineInfos.Last().BoundingRectangle.Top)
                    {
                        lineInfo = LineInfos.Last();
                    }
                    else if (newpoint.Y <= LineInfos.First().BoundingRectangle.Top)
                    {
                        lineInfo = LineInfos.First();
                    }
                }
            }

            if (lineInfo != null)
                return lineInfo;

            return null;
        }

    }
}
