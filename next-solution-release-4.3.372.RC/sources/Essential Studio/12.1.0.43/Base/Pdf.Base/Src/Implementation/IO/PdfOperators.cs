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

#region file using directives
using System;
using System.Diagnostics;
#endregion

namespace Syncfusion.Pdf.IO
{
    /// <summary>
    /// Class of string PDF common operators.
    /// </summary>
    internal sealed class Operators
    {
        #region Constants
        /// <summary>
        /// Operator for starting indirect object.
        /// </summary>
        public const string obj = "obj";
        /// <summary>
        /// Operator for ending indirect object.
        /// </summary>
        public const string endobj = "endobj";
        /// <summary>
        /// Operator for reference on indirect object.
        /// </summary>
        public const string R = "R";
        /// <summary>
        /// Whitespace symbol.
        /// </summary>
        public const string WhiteSpace = " ";
        /// <summary>
        /// Slash symbol.
        /// </summary>
        public const string Slash = "/";
        /// <summary>
        /// Less than symbol.
        /// </summary>
        public const string LessThan = "<";
        /// <summary>
        /// Greater than symbol.
        /// </summary>
        public const string GreaterThan = ">";
        /// <summary>
        /// New Line symbol.
        /// </summary>
        public const string NewLine = "\r\n";
        /// <summary>
        /// Regex New Line symbol.
        /// </summary>
        public const string RegexNewLine = @"\\r\\n";
        /// <summary>
        /// Operator for starting stream object.
        /// </summary>
        public const string stream = "stream";
        /// <summary>
        /// Operator for ending indirect object.
        /// </summary>
        public const string endstream = "endstream";
        /// <summary>
        /// Operator for starting cross-reference table.
        /// </summary>
        public const string xref = "xref";
        /// <summary>
        /// Operator in cross-reference table.
        /// </summary>
        public const string f = "f";
        /// <summary>
        /// Operator in cross-reference table.
        /// </summary>
        public const string n = "n";
        /// <summary>
        /// Trailer begining.
        /// </summary>
        public const string trailer = "trailer";
        /// <summary>
        /// Operator in trailer object.
        /// </summary>
        public const string startxref = "startxref";
        /// <summary>
        /// End of File (trailer) operator.
        /// </summary>
        public const string EOF = "%%EOF";
        /// <summary>
        /// Start of File (trailer) operator.
        /// </summary>
        public const string header = "%PDF-1.5";
        /// <summary>
        /// Begin text operator.
        /// </summary>
        public const string BeginText = "BT";
        /// <summary>
        /// End text operator.
        /// </summary>
        public const string EndText = "ET";
        /// <summary>
        /// Begin path operator.
        /// </summary>
        public const string BeginPath = "m";
        /// <summary>
        /// Append line segment operator.
        /// </summary>
        public const string AppendLineSegment = "l";
        /// <summary>
        /// Stroke operator.
        /// </summary>
        public const string Stroke = "S";
        /// <summary>
        /// Fill by nonzero winding rule operator.
        /// </summary>
        public const string Fill = "f";
        /// <summary>
        /// Fill by even-odd rule operator.
        /// </summary>
        public const string Fill_EvenOdd = "f*";
        /// <summary>
        /// Fill &amp;&amp; Stroke operator.
        /// </summary>
        public const string FillStroke = "B";
        /// <summary>
        /// Fill &amp;&amp; Stroke operator.
        /// </summary>
        public const string FillStroke_EvenOdd = "B*";
        /// <summary>
        /// Append a cubic Bezier curve to the current path.
        /// </summary>
        public const string AppendBezierCurve = "c";
        /// <summary>
        /// Append a rectangle to the current path as a complete subpath.
        /// </summary>
        public const string AppendRectangle = "re";
        /// <summary>
        /// Save graphics state operator.
        /// </summary>
        public const string SaveState = "q";
        /// <summary>
        /// Restore graphics state operator.
        /// </summary>
        public const string RestoreState = "Q";
        /// <summary>
        /// Paint XObject operator.
        /// </summary>
        public const string PaintXObject = "Do";
        /// <summary>
        /// Modifies CTM (current transformation matrix).
        /// </summary>
        public const string ModifyCTM = "cm";
        /// <summary>
        /// Modifies CTM (current transformation matrix).
        /// </summary>
        public const string ModifyTM = "Tm";
        /// <summary>
        /// Sets line width.
        /// </summary>
        public const string SetLineWidth = "w";
        /// <summary>
        /// Sets line cap style.
        /// </summary>
        public const string SetLineCapStyle = "J";
        /// <summary>
        /// Sets line join style.
        /// </summary>
        public const string SetLineJoinStyle = "j";
        /// <summary>
        /// Sets dash pattern.
        /// </summary>
        public const string SetDashPattern = "d";
        /// <summary>
        /// Sets flatness tolerance.
        /// </summary>
        public const string SetFlatnessTolerance = "i";
        /// <summary>
        /// Closes path.
        /// </summary>
        public const string ClosePath = "h";
        /// <summary>
        /// Closes and strokes path.
        /// </summary>
        public const string CloseStrokePath = "s";
        /// <summary>
        /// Operator for closing then filling and stroking a path.
        /// </summary>
        public const string CloseFillStrokePath = "b";
        /// <summary>
        /// Sets character space.
        /// </summary>
        public const string SetCharacterSpace = "Tc";
        /// <summary>
        /// Sets word space.
        /// </summary>
        public const string SetWordSpace = "Tw";
        /// <summary>
        /// Sets horizontal scaling.
        /// </summary>
        public const string SetHorizontalScaling = "Tz";
        /// <summary>
        /// Sets text leading.
        /// </summary>
        public const string SetTextLeading = "TL";
        /// <summary>
        /// Sets font operator.
        /// </summary>
        public const string SetFont = "Tf";
        /// <summary>
        /// Sets rendering mode.
        /// </summary>
        public const string SetRenderingMode = "Tr";
        /// <summary>
        /// Sets text rise.
        /// </summary>
        public const string SetTextRise = "Ts";
        /// <summary>
        /// Sets text horizontal scaling.
        /// </summary>
        public const string SetTextScaling = "Tz";
        /// <summary>
        /// Set coordinates operator.
        /// </summary>
        public const string SetCoords = "Td";
        /// <summary>
        /// Operator that sets the start of the new line and leading simultaneously.
        /// </summary>
        public const string SetCoordsAndLeading = "TD";
        /// <summary>
        /// Sets text pointer to next line.
        /// </summary>
        public const string GoToNextLine = "T*";
        /// <summary>
        /// Set text operator
        /// </summary>
        public const string SetText = "Tj";
        /// <summary>
        /// Operator to set text with formatting.
        /// </summary>
        public const string SetTextWithFormatting = "TJ";
        /// <summary>
        /// Operator that writes text on the new line.
        /// </summary>
        public const string SetTextOnNewLine = "\'";
        /// <summary>
        /// Operator that writes text on the new line and set spacings.
        /// </summary>
        public const string SetTextOnNewLineWithSpacings = "\"";
        /// <summary>
        /// Selects a color space for the stroking color.
        /// </summary>
        public const string SelectColorSpaceForStroking = "CS";
        /// <summary>
        /// Selects a color space for the nonstroking color.
        /// </summary>
        public const string SelectColorSpaceForNonStroking = "cs";
        /// <summary>
        /// Sets RGB color for stroking operations.
        /// </summary>
        public const string SetRGBColorForStroking = "RG";
        /// <summary>
        /// Same as RGB but for nonstroking operations.
        /// </summary>
        public const string SetRGBColorForNonStroking = "rg";
        /// <summary>
        /// Sets CMYK color for stroking operations.
        /// </summary>
        public const string SetCMYKColorForStroking = "K";
        /// <summary>
        /// Same as CMYK but for nonstroking operations.
        /// </summary>
        public const string SetCMYKColorForNonstroking = "k";
        /// <summary>
        /// Sets gray color for stroking operations.
        /// </summary>
        public const string SetGrayColorForStroking = "G";
        /// <summary>
        /// Same as RGB but for nonstroking operations.
        /// </summary>
        public const string SetGrayColorForNonstroking = "g";
        /// <summary>
        /// Set pattern operator.
        /// </summary>
        public const string Pattern = "Pattern";
        /// <summary>
        /// Same as SC, but also supports Pattern, Separation, DeviceN, and ICCBased
        /// color spaces. For non-stroking operations.
        /// </summary>
        public const string SetColorAndPattern = "scn";
        /// <summary>
        /// Same as SC, but also supports Pattern, Separation, DeviceN, and ICCBased
        /// color spaces. For stroking.
        /// </summary>
        public const string SetColorAndPatternStroking = "SCN";
        /// <summary>
        /// Modify the current clipping path by intersecting it with the current path, using the
        /// nonzero winding number rule to determine which regions lie inside the clipping path.
        /// </summary>
        public const string ClipPath = "W";
        /// <summary>
        /// Modify the current clipping path by intersecting it with the current path, using the
        /// odd-even rule to determine which regions lie inside the clipping path.
        /// </summary>
        public const string ClipPath_EvenOdd = "W*";
        /// <summary>
        /// End the path object without filling or stroking it. This operator is a "path-painting
        /// no-op," used primarily for the side effect of changing the current clipping path (see
        /// "Clipping Path Operators").
        /// </summary>
        public const string EndPath = "n";
        /// <summary>
        /// Graphics state operator.
        /// </summary>
        public const string SetGraphicsState = "gs";
        /// <summary>
        /// Symbol of commenting.
        /// </summary>
        public const string Comment = "%";
        /// <summary>
        /// Indicates any symbol (regex syntax).
        /// </summary>
        public const string AnyRegexSymbol = ".*";
        /// <summary>
        /// Begins a marked-content sequence.
        /// </summary>
        public const string BeginMarkedSequence = "BMC";
        /// <summary>
        /// Ends a marked-content sequence.
        /// </summary>
        public const string EndMarkedSequence = "EMC";
        /// <summary>
        /// Even-odd filling method marker.
        /// </summary>
        public const string EvenOdd = "*";
        /// <summary>
        /// The operator to apped bezier curve with x2 y2 x3 y3 set.
        /// </summary>
        public const string AppendBezierCurve2 = "v";
        /// <summary>
        /// The operator to apped bezier curve with x1 y1 x3 y3 set.
        /// </summary>
        public const string AppendBezierCurve1 = "y";
        /// <summary>
        /// Set miter limit operator.
        /// </summary>
        public const string SetMiterLimit = "M";
        /// <summary>
        /// Set color rendering intent operator.
        /// </summary>
        public const string SetColorRenderingIntent = "ri";
        /// <summary>
        /// Set colour of the current colour space for stroking.
        /// </summary>
        public const string SetColorStroking = "SC";
        /// <summary>
        /// Set colour of the current colour space for non-stroking operations.
        /// </summary>
        public const string SetColorNonStroking = "sc";

        public const string Para = "P";
        public const string Mcid = "MCID";
        #endregion

        #region Constructors
        /// <summary>
        /// Private constructor.
        /// </summary>
        private Operators()
        {
            throw new NotSupportedException();
        }
        #endregion
    }
}
