#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;

using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents loaded bookmark class.
    /// </summary>
    public class PdfLoadedBookmark : PdfBookmark
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedBookmark"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        internal PdfLoadedBookmark(PdfDictionary dictionary, PdfCrossTable crossTable)
            : base(dictionary, crossTable)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the outline destination.
        /// </summary>
        public override PdfDestination Destination
        {
            get
            {
                return GetDestination();
            }

            set
            {
                base.Destination = value;
            }
        }

        /// <summary>
        /// Gets or sets the outline title.
        /// </summary>
        /// <remarks>The outline title is the text,
        /// which appears in the outline tree as a tree node.</remarks>
        public override string Title
        {
            get
            {
                return GetTitle();
            }

            set
            {
                base.Title = value;
            }
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        public override PdfColor Color
        {
            get
            {
                return GetColor();
            }

            set
            {
                SetColor(value);
            }
        }

        /// <summary>
        /// Gets or sets the text style.
        /// </summary>
        public override PdfTextStyle TextStyle
        {
            get
            {
                return GetTextStyle();
            }

            set
            {
                SetTextStyle(value);
            }
        }

        /// <summary>
        /// Gets or sets the next outline object.
        /// </summary>
        /// <remarks>The null value means that the object is the last outline.</remarks>
        internal override PdfBookmark Next
        {
            get
            {
                return GetNext();
            }

            set
            {
                base.Next = value;
            }
        }

        /// <summary>
        /// Gets or sets the previous outline object.
        /// </summary>
        /// <remarks>The null value means that the object is the first outline.</remarks>
        internal override PdfBookmark Previous
        {
            get
            {
                return GetPrevious();
            }

            set
            {
                base.Previous = value;
            }
        }

        /// <summary>
        /// Gets the parent outline base.
        /// </summary>
        internal override PdfBookmarkBase Parent
        {
            get
            {
                return base.Parent;
            }
        }

        /// <summary>
        /// Gets the sub items.
        /// </summary>
        internal override List<PdfBookmarkBase> List
        {
            get
            {
                List<PdfBookmarkBase> list = base.List;

                if (list.Count == 0)
                {
                    ReproduceTree();
                }

                return list;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <returns>The title of the bookmark.</returns>
        private string GetTitle()
        {
            string title = String.Empty;

            if (Dictionary.ContainsKey(DictionaryProperties.Title))
            {
                PdfString str = CrossTable.GetObject(Dictionary[DictionaryProperties.Title]) as PdfString;

                title = str.Value;
            }

            return title;
        }

        /// <summary>
        /// Gets the color.
        /// </summary>
        /// <returns>The color of the bookmark.</returns>
        private PdfColor GetColor()
        {
            PdfColor color = new PdfColor(0, 0, 0);

            if (Dictionary.ContainsKey(DictionaryProperties.C))
            {
                PdfArray colours = CrossTable.GetObject(Dictionary[DictionaryProperties.C]) as PdfArray;

                float red = (colours[0] as PdfNumber).FloatValue;
                float green = (colours[1] as PdfNumber).FloatValue;
                float blue = (colours[2] as PdfNumber).FloatValue;

                color = new PdfColor(red, green, blue);
            }

            return color;
        }

        /// <summary>
        /// Gets the text style.
        /// </summary>
        /// <returns>The style of bookmark text.</returns>
        private PdfTextStyle GetTextStyle()
        {
            PdfTextStyle style = PdfTextStyle.Regular;

            if (Dictionary.ContainsKey(DictionaryProperties.F))
            {
                PdfNumber flag = CrossTable.GetObject(Dictionary[DictionaryProperties.F]) as PdfNumber;

                int flagValue = flag.IntValue;

                style |= (PdfTextStyle)flagValue;
            }

            return style;
        }

        /// <summary>
        /// Gets the next.
        /// </summary>
        /// <returns>The next bookmark to this bookmark.</returns>
        private PdfBookmark GetNext()
        {
            PdfBookmark nextBookmark = null;

            int index = Parent.List.IndexOf(this);

            ++index;

            if (index < Parent.List.Count)
            {
                nextBookmark = Parent.List[index] as PdfBookmark;
            }
            else
            {
                if (Dictionary.ContainsKey(DictionaryProperties.Next))
                {
                    PdfDictionary next = CrossTable.GetObject(Dictionary[DictionaryProperties.Next]) as PdfDictionary;

                    nextBookmark = new PdfLoadedBookmark(next, CrossTable);
                }
            }

            return nextBookmark;
        }

        /// <summary>
        /// Gets the previos.
        /// </summary>
        /// <returns>The previous bookmark to this bookmark.</returns>
        private PdfBookmark GetPrevious()
        {
            PdfBookmark prevBookmark = null;

            int index = List.IndexOf(this);

            --index;

            if (index >= 0)
            {
                prevBookmark = List[index] as PdfBookmark;
            }
            else
            {
                if (Dictionary.ContainsKey(DictionaryProperties.Prev))
                {
                    PdfDictionary prev = CrossTable.GetObject(Dictionary[DictionaryProperties.Prev]) as PdfDictionary;

                    prevBookmark = new PdfLoadedBookmark(prev, CrossTable);
                }
            }

            return prevBookmark;
        }

        /// <summary>
        /// Sets the color.
        /// </summary>
        /// <param name="color">The color.</param>
        private void SetColor(PdfColor color)
        {
            float[] rgb = new float[] { color.Red, color.Green, color.Blue };
            PdfArray colors = new PdfArray(rgb);
            Dictionary.SetProperty(DictionaryProperties.C, colors);
        }

        /// <summary>
        /// Sets the text style.
        /// </summary>
        /// <param name="value">The value.</param>
        private void SetTextStyle(PdfTextStyle value)
        {
            int style = (int)GetTextStyle();

            style |= (int)value;

            Dictionary.SetNumber(DictionaryProperties.F, style);
        }

        /// <summary>
        /// Gets the destination.
        /// </summary>
        /// <returns>The destination of bookmark.</returns>
        private PdfDestination GetDestination()
        {
            if (Dictionary.ContainsKey(DictionaryProperties.Dest) && (base.Destination == null))
            {
                IPdfPrimitive obj = CrossTable.GetObject(Dictionary[DictionaryProperties.Dest]);
                PdfArray array = obj as PdfArray;
                PdfName name = obj as PdfName;
                PdfString str = obj as PdfString;
                PdfLoadedDocument ldDoc = CrossTable.Document as PdfLoadedDocument;

                if (ldDoc != null)
                {
                    if (name != null)
                    {
                        array = ldDoc.GetNamedDestination(name);
                    }
                    else if (str != null)
                    {
                        array = ldDoc.GetNamedDestination(str);
                    }
                }

                if (array != null)
                {
                    PdfReferenceHolder holder = array[0] as PdfReferenceHolder;
                    PdfPageBase page = null;

                    if (holder == null && array[0] is PdfNumber)
                    {
                        PdfNumber pageNo = array[0] as PdfNumber;
                        page = (CrossTable.Document as PdfLoadedDocument).Pages[pageNo.IntValue];

                        PdfName mode = array[1] as PdfName;
                        if (mode != null)
                        {
                            if (mode.Value == "XYZ")
                            {
                                PdfNumber left = array[2] as PdfNumber;
                                PdfNumber top = array[3] as PdfNumber;
                                PdfNumber zoom = array[4] as PdfNumber;

                                float topValue = (top == null) ? 0 : page.Size.Height - top.FloatValue;
                                float leftValue = (left == null) ? 0 : left.FloatValue;

                                base.Destination = new PdfDestination(page, new PointF(leftValue, topValue));
                                if (zoom != null)
                                {
                                    base.Destination.Zoom = zoom.FloatValue;
                                }

                                if ((left == null) || (top == null) || (zoom == null))
                                {
                                    base.Destination.SetValidation(false);
                                }
                            }
                        }
                        else
                        {
                            if (page != null)
                            {
                                base.Destination = new PdfDestination(page);
                                base.Destination.Mode = PdfDestinationMode.FitToPage;
                            }
                        }
                    }
                    if (holder != null)
                    {
                        PdfDictionary dic = CrossTable.GetObject(holder) as PdfDictionary;
                        page = (CrossTable.Document as PdfLoadedDocument).Pages.GetPage(dic);
                        PdfName mode = array[1] as PdfName;
                        if (mode != null)
                        {
                            if (mode.Value == "XYZ")
                            {
                                PdfNumber left = array[2] as PdfNumber;
                                PdfNumber top = array[3] as PdfNumber;
                                PdfNumber zoom = array[4] as PdfNumber;

                                float topValue = (top == null) ? 0 : page.Size.Height - top.FloatValue;
                                float leftValue = (left == null) ? 0 : left.FloatValue;

                                base.Destination = new PdfDestination(page, new PointF(leftValue, topValue));
                                if (zoom != null)
                                {
                                    base.Destination.Zoom = zoom.FloatValue;
                                }

                                if ((left == null) || (top == null) || (zoom == null))
                                {
                                    base.Destination.SetValidation(false);
                                }
                            }
                            else
                            {
                                if (mode.Value == "FitR")
                                {
                                    PdfNumber left = array[2] as PdfNumber;
                                    PdfNumber bottom = array[3] as PdfNumber;
                                    PdfNumber right = array[4] as PdfNumber;
                                    PdfNumber top = array[5] as PdfNumber;
                                    base.Destination = new PdfDestination(page, new RectangleF(left.FloatValue, bottom.FloatValue, right.FloatValue, top.FloatValue));
                                    base.Destination.Mode = PdfDestinationMode.FitR;

                                }

                            }
                        }
                        else
                        {
                            if (page != null && mode.Value == "Fit")
                            {
                                base.Destination = new PdfDestination(page);
                                base.Destination.Mode = PdfDestinationMode.FitToPage;
                            }
                        }
                    }
                }
            }
            else if (Dictionary.ContainsKey(DictionaryProperties.A) && (base.Destination == null))
            {
                IPdfPrimitive obj = CrossTable.GetObject(Dictionary[DictionaryProperties.A]);
                PdfDictionary destDic = obj as PdfDictionary;
                obj = destDic[DictionaryProperties.D];
                if (obj is PdfReferenceHolder)                
                     obj = (obj as PdfReferenceHolder).Object;                
                PdfArray array = obj as PdfArray;
                PdfName name = obj as PdfName;
                PdfString str = obj as PdfString;
                PdfLoadedDocument ldDoc = CrossTable.Document as PdfLoadedDocument;

                if (ldDoc != null)
                {
                    if (name != null)
                    {
                        array = ldDoc.GetNamedDestination(name);
                    }
                    else if (str != null)
                    {
                        array = ldDoc.GetNamedDestination(str);
                    }
                }

                if (array != null)
                {
                    PdfReferenceHolder holder = array[0] as PdfReferenceHolder;

                    PdfPageBase page = null;

                    if (holder != null)
                    {
                        PdfDictionary dic = CrossTable.GetObject(holder) as PdfDictionary;
                        page = (CrossTable.Document as PdfLoadedDocument).Pages.GetPage(dic);
                    }

                    PdfName mode = array[1] as PdfName;

                    if (mode.Value == "FitBH" || mode.Value == "FitH")
                    {
                        PdfNumber top = array[2] as PdfNumber;
                        if (page != null)
                        {
                            float topValue = (top == null) ? 0 : page.Size.Height - top.FloatValue;
                            base.Destination = new PdfDestination(page, new PointF(0, topValue));
                            if (top == null)
                            {
                                base.Destination.SetValidation(false);
                            }
                        }
                    }
                    else if (mode.Value == "XYZ")
                    {
                        PdfNumber left = array[2] as PdfNumber;
                        PdfNumber top = array[3] as PdfNumber;
                        PdfNumber zoom = array[4] as PdfNumber;

                        if (page != null)
                        {
                            float topValue = (top == null) ? 0 : page.Size.Height - top.FloatValue;
                            float leftValue = (left == null) ? 0 : left.FloatValue;

                            base.Destination = new PdfDestination(page, new PointF(leftValue, topValue));
                            if (zoom != null)
                            {
                                base.Destination.Zoom = zoom.FloatValue;
                            }

                            if ((left == null) || (top == null) || (zoom == null))
                            {
                                base.Destination.SetValidation(false);
                            }
                        }
                    }
                    else
                    {
                        if (page != null && mode.Value == "Fit")
                        {
                            base.Destination = new PdfDestination(page);
                            base.Destination.Mode = PdfDestinationMode.FitToPage;
                        }
                    }
                }
            }

            return base.Destination;
        }
        #endregion
    }
}
