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
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Syncfusion.Windows.Tools.Controls
{
    public class HistoryInfo
    {
        private Actions action;
        private BlockCollection<BlockAdv> blocks;
        private TextPosition startPosition;
        private TextPosition endPosition;
        private string text;
        private Inline style;
        private ParagraphAdv paraStyle;
        private bool isStyleChanged = false;
        private UndoType undoType = UndoType.Normal;
        private TextPosition tempPositionStart;
        private TextPosition tempPositionEnd;
        private List<PreservedCellsInfo> preservedcells = new List<PreservedCellsInfo>();
        bool isImageselected = false;
        bool isdeletedcolumn = false;
        bool isdeletedrow = false;
        bool isMergedcells = false;
        bool isdeletedtable = false;
        bool isinsertedtable = false;
        bool isinsertedrow = false;
        bool isinsertedcolumn = false;
        bool nextistable = false;
        bool cellselected = false;

        /// <summary>
        /// Initializes the new instance of HistoryInfo class
        /// </summary>
        public HistoryInfo()
        {
            blocks = new BlockCollection<BlockAdv>();
        }

        /// <summary>
        /// Gets or Sets the action
        /// </summary>
        internal Actions Action
        {
            get
            {
                return action;
            }
            set
            {
                action = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        internal bool IsDeletedColumnHistory
        {
            get
            {
                return isdeletedcolumn;
            }
            set
            {
                isdeletedcolumn = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal bool IsDeletedRowHistory
        {
            get
            {
                return isdeletedrow;
            }
            set
            {
                isdeletedrow = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal bool IsMergedCellsHistory
        {
            get
            {
                return isMergedcells;
            }
            set
            {
                isMergedcells = value;
            }
        }

        internal bool IsDeletedTableHistory
        {
            get
            {
                return isdeletedtable;
            }
            set
            {
                isdeletedtable = value;
            }
        }

        internal bool IsImageResizerSelected
        {
            get
            {
                return isImageselected;
            }
            set
            {
                isImageselected = value;
            }
        }


        internal bool IsInsertedTableHistory
        {
            get
            {
                return isinsertedtable;
            }
            set
            {
                isinsertedtable = value;
            }
        }


        internal bool IsInsertedRowHistory
        {
            get
            {
                return isinsertedrow;
            }
            set
            {
                isinsertedrow = value;
            }
        }

        internal bool IsInsertedColumnHistory
        {
            get
            {
                return isinsertedcolumn;
            }
            set
            {
                isinsertedcolumn = value;
            }
        }

        /// <summary>
        /// Gets or Sets the inline style
        /// </summary>
        internal Inline InlineStyle
        {
            get
            {
                return style;
            }
            set
            {
                style = value;
            }
        }

        internal UndoType UndoType
        {
            get
            {
                return undoType;
            }
            set
            {
                undoType = value;
            }
        }

        /// <summary>
        /// Gets or Sets the paragraph style
        /// </summary>
        internal ParagraphAdv ParagraphStyle
        {
            get
            {
                return paraStyle;
            }
            set
            {
                paraStyle = value;
            }
        }

        internal List<PreservedCellsInfo> PreservedCells
        {
            get
            {
                return preservedcells;
            }
            set
            {
                preservedcells = value;
            }
        }

        /// <summary>
        /// Gets or sets the blocks
        /// </summary>
        internal BlockCollection<BlockAdv> Blocks
        {
            get
            {
                return blocks;
            }
            set
            {
                blocks = value;
            }
        }

        internal BlockCollection<BlockAdv> CopiedBlocks
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        internal bool IsStyleChanged
        {
            get
            {
                return isStyleChanged;
            }
            set
            {
                isStyleChanged = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal TextPosition StartPosition
        {
            get
            {
                return startPosition;
            }
            set
            {
                startPosition = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal TextPosition EndPosition
        {
            get
            {
                return endPosition;
            }
            set
            {
                endPosition = value;
            }
        }

        internal TextPosition TempPositionStart
        {
            get
            {
                return tempPositionStart;
            }
            set
            {
                tempPositionStart = value;
            }
        }

        internal bool IsCellSelected
        {
            get
            {
                return cellselected;
            }
            set
            {
                cellselected = value;
            }
        }

        internal TextPosition TempPositionEnd
        {
            get
            {
                return tempPositionEnd;
            }
            set
            {
                tempPositionEnd = value;
            }
        }

        internal bool NextIsTable
        {
            get
            {
                return nextistable;
            }
            set
            {
                nextistable = value;
            }
        }

        internal bool IsInSameTable
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the text
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
    }

}
