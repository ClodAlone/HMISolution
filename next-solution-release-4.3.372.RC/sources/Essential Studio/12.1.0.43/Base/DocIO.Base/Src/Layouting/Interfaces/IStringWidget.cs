#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#if !SILVERLIGHT

using System.Drawing;
using Syncfusion.DocIO.Rendering;


namespace Syncfusion.Layouting
{
    /// <summary>
    /// Represnts an widget which can be measure itself.
    /// </summary>
    internal interface ITextMeasurable
    {
        /// <summary>
        /// Measures size of specified string.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        SizeF Measure(string text);
        /// <summary>
        /// Measures size of specified string.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        SizeF Measure(DrawingContext dc, string text);
    }

    /// <summary>
    /// Represents a Widget with string-like layout.
    /// </summary>
    internal interface IStringWidget : ISplitLeafWidget, ITextMeasurable
    {
        /// <summary>
        /// Gets text string.
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Draw specified string to custom graphics.
        /// </summary>
        /// <param name="cg"></param>
        /// <param name="ltWidget"></param>
        /// <param name="text"></param>
        void Draw(DrawingContext dc, LayoutedWidget ltWidget, string text);

        /// <summary>
        /// Offsets to index.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="text">The text.</param>
        /// <param name="clientWidth">The clientWidth.</param>
        /// <returns></returns>
        int OffsetToIndex(DrawingContext dc, double offset, string text, float clientWidth,float clientActiveAreaWidth);

        /// <summary>
        /// Gets text ascent.
        /// </summary>
        /// <param name="graphics"></param>
        /// <returns></returns>
        double GetTextAscent(DrawingContext dc, ref float exceededLineAscent);
    }
}

#endif