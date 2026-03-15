#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !WinRT
using Syncfusion.Windows.ComponentModel;

namespace Syncfusion.Windows.Controls.Grid
#else
using Syncfusion.WinRT.ComponentModel;

namespace Syncfusion.WinRT.Controls.Grid
#endif
{
    /// <summary>
    /// Represents a method that handles a <see cref="GridModel.QueryCellText"/>, <see cref="GridModel.SaveCellText"/>, 
    /// <see cref="GridModel.QueryCellFormattedText"/>, or <see cref="GridModel.SaveCellFormattedText"/> event.
    /// </summary>
    public delegate void GridCellTextEventHandler(object sender, GridCellTextEventArgs e);

    /// <summary>
    /// Provides event data for the <see cref="GridModel.QueryCellText"/>, <see cref="GridModel.SaveCellText"/>, 
    /// <see cref="GridModel.QueryCellFormattedText"/>, or <see cref="GridModel.SaveCellFormattedText"/> event.
    /// </summary>
    /// <remarks>
    /// If you want to customize the grid's behavior, you should set <see cref="SyncfusionHandledEventArgs.Handled"/> 
    /// to True. The grid will check this flag to see whether it should accept your modification 
    /// or use a conversion.
    /// <para/>
    /// If you need identity information about the cell such as row and column index, you can get that
    /// information by querying <see cref="GridStyleInfo.CellIdentity"/> of the <see cref="GridCellTextEventArgs.Style"/>
    /// object.
    /// <para/>
    /// The <see cref="GridModel.SaveCellFormattedText"/> and <see cref="GridModel.SaveCellText"/> events
    /// expect that you save the resulting value in <see cref="GridStyleInfo.CellValue"/> of the <see cref="GridCellTextEventArgs.Style"/>
    /// object.
    /// <para/>
    /// The <see cref="GridModel.QueryCellFormattedText"/> and <see cref="GridModel.QueryCellText"/> events
    /// expect that you save the resulting string in <see cref="GridCellTextEventArgs.Text"/>.
    /// <para/>
    /// The <see cref="TextInfo"/> is only used for  <see cref="GridModel.SaveCellFormattedText"/> and
    /// <see cref="GridModel.QueryCellFormattedText"/>.
    /// </remarks>
    /// <seealso cref="GridCellTextEventHandler"/>
    /// <seealso cref="GridCellTextEventArgs"/>
    /// <seealso cref="GridModel.SaveCellFormattedText"/>
    /// <seealso cref="GridModel.QueryCellFormattedText"/>
    /// <seealso cref="GridCellModelBase.ApplyFormattedText"/>
    /// <seealso cref="GridStyleInfo.FormattedText"/>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public sealed class GridCellTextEventArgs : SyncfusionHandledEventArgs
    {
        string text;
        GridStyleInfo style;
        object value;
        int textInfo;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="text">The string that represents the underlying cell value.</param>
        /// <param name="style">The style object.</param>
        /// <param name="value">The cell value.</param>
        /// <param name="textInfo"> textInfo is a hint where the call originated, e.g. GridCellBaseTextInfo.DisplayText.</param>
        public GridCellTextEventArgs(string text, GridStyleInfo style, object value, int textInfo)
        {
            this.text = text;
            this.style = style;
            this.value = value;
            this.textInfo = textInfo;
        }

        /// <summary>
        /// The string that represents the underlying cell value.
        /// </summary>
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
            }
        }

        /// <summary>
        /// The style object.
        /// </summary>
        public GridStyleInfo Style
        {
            get
            {
                return style;
            }
        }

        /// <summary>
        /// The cell value.
        /// </summary>
        public object Value
        {
            get
            {
                return value;
            }
        }

        /// <summary>
        /// textInfo is a hint where the call originated, e.g. GridCellBaseTextInfo.DisplayText.
        /// </summary>
        public int TextInfo
        {
            get
            {
                return textInfo;
            }
        }
    }

}
