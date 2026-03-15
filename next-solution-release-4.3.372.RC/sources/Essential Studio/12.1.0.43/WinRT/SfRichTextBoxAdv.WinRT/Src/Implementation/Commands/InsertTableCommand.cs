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
using System.Windows.Input;
using Windows.UI.Xaml;

namespace Syncfusion.UI.Xaml.RichTextBoxAdv
{
    #region Insert Table Command
    public class InsertTableCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InsertTableCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public InsertTableCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Determines whether this instance can execute command with the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        ///   <c>true</c> if this instance can execute command with the specified parameter; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanExecuteCommand(object parameter)
        {
            if (OwnerControl != null)
                return OwnerControl.Selection.IsEmpty;
            return false;
        }
        /// <summary>
        /// Executes the insert table command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl.IsReadOnlyMode || !OwnerControl.IsDocumentLoaded)
                return;
            int rowCount = 1, columnCount = 2;
            if (parameter is string)
            {
                string[] collection = parameter.ToString().Split(new string[] { ",", " " }, StringSplitOptions.None);
                if (OwnerControl != null)
                {
                    rowCount = int.Parse(collection[0]);
                    columnCount = int.Parse(collection[1]);
                }
            }
            else if (parameter is int[])
            {
                int[] rowcolumn = (int[])parameter;
                if (OwnerControl != null)
                {
                    rowCount =rowcolumn[0];
                    columnCount = rowcolumn[1];
                }
            }
            OwnerControl.InsertTableInBlocks(rowCount, columnCount);
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion

    #region Insert Column Command
    public class InsertColumnCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InsertColumnCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public InsertColumnCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Determines whether this instance can execute command with the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        ///   <c>true</c> if this instance can execute command with the specified parameter; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanExecuteCommand(object parameter)
        {
            if (OwnerControl != null)
                return OwnerControl.Selection.IsEmpty && OwnerControl.Selection.Start.Paragraph.IsInsideTable;
            return false;
        }
        /// <summary>
        /// Executes the insert columnn command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl.IsReadOnlyMode || !OwnerControl.IsDocumentLoaded)
                return;
            ColumnPlacement placement;
            if (!Enum.TryParse(parameter.ToString(), out placement))
                placement = ColumnPlacement.Right;

            OwnerControl.InsertColumnInTable(placement);
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion

    #region Insert Row Command
    public class InsertRowCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InsertRowCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public InsertRowCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Determines whether this instance can execute command with the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        ///   <c>true</c> if this instance can execute command with the specified parameter; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanExecuteCommand(object parameter)
        {
            if (OwnerControl != null)
                return OwnerControl.Selection.IsEmpty && OwnerControl.Selection.Start.Paragraph.IsInsideTable;

            return false;
        }
        /// <summary>
        /// Executes theinsert row command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl.IsReadOnlyMode || !OwnerControl.IsDocumentLoaded)
                return;
            RowPlacement rowplacement;
            if (!Enum.TryParse(parameter.ToString(), out rowplacement))
                rowplacement = RowPlacement.Below;
            OwnerControl.InsertRowInTable(rowplacement);
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion
}
