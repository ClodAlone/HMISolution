//-------------------------------------------------------------------------------------------------
// <copyright file="GridDropDownMonthCalendarCellModel.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Defines the data / model part of a month calendar cell that lets users drop-down a calendar
    /// and select a date or range of dates.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridDropDownMonthCalendarCellModel"/> can serve as model for several <see cref="GridDropDownMonthCalendarCellRenderer"/>
    /// instances if a there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridDropDownMonthCalendarCellRenderer"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class GridDropDownMonthCalendarCellModel : GridDropDownCellModel
    {
        /// <overload>
        /// Initializes a new <see cref="GridDropDownMonthCalendarCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridDropDownMonthCalendarCellModel"/> object 
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>    
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridDropDownMonthCalendarCellModel(GridModel grid)
            : base(grid)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridDropDownMonthCalendarCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridDropDownMonthCalendarCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
        }

        /// <summary>
        /// Parses the text and converts it into a cell value to be stored in the style object (ignoring any <see cref="GridStyleInfo.Format"/> settings).
        /// CultureInfo.CurrentText is used for parsing the string.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="text">The input text to be parsed.</param>
        /// <returns>True if value was parsed correctly and saved in style object as <see cref="GridStyleInfo.CellValue"/>; False otherwise.</returns>
        public override bool ApplyText(GridStyleInfo style, string text)
        {
            if (style.CellValueType != null)
            {
                return base.ApplyText(style, text);
            }

            GridCellTextEventArgs ea = new GridCellTextEventArgs(text, style, null, -1);
            Grid.RaiseSaveCellText(ea);
            if (!ea.Handled)
            {
                try
                {
                    style.BeginUpdate();
                    if (text != string.Empty)
                    {
                        style.CellValue = GetValueFromStyle(style, text);
                    }
                    else
                    {
                        style.CellValue = text;
                    }

                    style.ResetError();
                }
                catch (Exception ex)
                {
                    style.Error = ex.Message;
                    if (style.StrictValueType)
                    {
                        throw;
                    }
                    else if (ex is FormatException || ex.InnerException is FormatException)
                    {
                        style.CellValue = text;
                        // Possibly could also change CellValueType here based on input string.
                        // e.Style.CellValueType = typeof(string);
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    style.EndUpdate();
                }
            }

            return true;
        }

        /// <summary>
        /// Parses the display text and converts it into a cell value to be stored in the style object.
        /// </summary>
        /// <param name="style">Cell style information.</param>
        /// <param name="text">Input text.</param>
        /// <param name="textInfo">TextInfo is a hint of who is calling, default is GridCellBaseTextInfo.DisplayText.</param>
        /// <returns>True if value was parsed correctly and saved in style object as <see cref="GridStyleInfo.CellValue"/>; False otherwise.</returns>
        public override bool ApplyFormattedText(GridStyleInfo style, string text, int textInfo)
        {
            if (style.CellValueType != null)
            {
                return base.ApplyFormattedText(style, text, textInfo);
            }

            GridCellTextEventArgs ea = new GridCellTextEventArgs(text, style, null, textInfo);
            Grid.RaiseSaveCellFormattedText(ea);

            if (!ea.Handled)
            {
                Grid.RaiseParseCommonFormats(ea);
            }

            if (!ea.Handled)
            {
                try
                {
                    style.BeginUpdate();
                    style.CellValue = GetValueFromStyle(style, text);
                    style.ResetError();
                }
                catch (Exception ex)
                {
                    style.Error = ex.Message;
                    if (style.StrictValueType)
                    {
                        throw;
                    }
                    else if (ex is FormatException || ex.InnerException is FormatException)
                    {
                        style.CellValue = text;
                        //// Possibly could also change CellValueType here based on input string.
                        //// e.Style.CellValueType = typeof(string);
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    style.EndUpdate();
                }
            }

            return true;
        }

        /// <summary>
        /// Gets formatted DateTime Value from GridStyleInfo object.
        /// </summary>
        /// <param name="style">GridStyleInfo object.</param>
        /// <returns>Formatted DateTime value.</returns>
        public static object GetValueFromStyle(GridStyleInfo style)
        {
            return GetValueFromStyle(style, style.CellValue);
        }

        private static object GetValueFromStyle(GridStyleInfo style, object value)
        {
            if (value is string)
            {
                string text = (string)value;
                if (text.Length > 0)
                {
                    CultureInfo ci = style.GetCulture(true);
                    IFormatProvider nfi = ci.DateTimeFormat;
                    value = style.ParseFormats == null
                          ? GridCellValueConvert.Parse(text, typeof(DateTime), nfi, style.Format)
                          : GridCellValueConvert.Parse(text, typeof(DateTime), nfi, style.ParseFormats, false);
                }
            }

            return value;
        }

        /// <override/>
        /// <summary>
        /// This is called from GridStyleInfo.GetFormattedText. 
        /// GridStyleInfo.CultureInfo is used for conversion to string.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value to format.</param>
        /// <param name="textInfo">TextInfo is a hint of who is calling, default is GridCellBaseTextInfo.DisplayText.</param>
        /// <returns>The formatted text for the given value.</returns>
        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            CultureInfo ci = style.CultureInfo != null ? style.CultureInfo : CultureInfo.CurrentCulture;

            object dateValue = GetValueFromStyle(style, value);

            if (dateValue is DateTime)
            {
                return ((DateTime)dateValue).ToString(style.Format, ci.DateTimeFormat);
            }

            return base.GetFormattedText(style, value, textInfo);
        }

        /// <override/>
        /// <summary>Creates a renderer for this cell model.</summary>
        /// <param name="control">The grid control.</param>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridDropDownMonthCalendarCellRenderer(control, this);
        }
    }
}
