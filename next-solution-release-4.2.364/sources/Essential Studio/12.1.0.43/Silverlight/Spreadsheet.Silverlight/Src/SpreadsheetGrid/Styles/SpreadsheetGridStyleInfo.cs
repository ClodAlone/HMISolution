#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Styles;
using System.Windows.Media;

namespace Syncfusion.Windows.Controls.Spreadsheet
{
    public class SpreadsheetGridStyleInfo : GridStyleInfo
    {
        public SpreadsheetGridStyleInfo()
        {
        }

        public SpreadsheetGridStyleInfo(GridStyleInfo style)
            : base(style)
        {
        }

        public SpreadsheetGridStyleInfo(GridStyleInfoStore store)
            : base(store)
        {
        }

        public SpreadsheetGridStyleInfo(StyleInfoIdentityBase identity)
            : base(identity)
        {
        }

        public SpreadsheetGridStyleInfo(StyleInfoIdentityBase identity, GridStyleInfoStore store)
            : base(identity, store)
        {
        }
    }

    [StaticDataField("sd")]
    public class SpreadsheetGridStyleInfoStore : GridStyleInfoStore
    {

#if SILVERLIGHT
        private new static StaticData sd = new StaticData(typeof(SpreadsheetGridStyleInfoStore), typeof(SpreadsheetGridStyleInfo), false);
#else
        private static StaticData sd = new StaticData(typeof(SpreadsheetGridStyleInfoStore), typeof(SpreadsheetGridStyleInfo), false);
#endif

        public SpreadsheetGridStyleInfoStore()
        {
            if (sd.IsEmpty)
            {
                new SpreadsheetGridStyleInfo();
            }
        }

        internal static StaticData StaticData
        {
            get
            {
                return sd;
            }
        }

        /// <override/>
        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }

        /// <override/>
        public override object Clone()
        {
            StyleInfoStore target = new SpreadsheetGridStyleInfoStore();
            CopyTo(target);
            return target;
        }
    }

    public class ExcelGridStyleInfoIdentity:GridStyleInfoIdentity
    {
        public ExcelGridStyleInfoIdentity(GridVolatileCellStyles data, int rowIndex, int colIndex) : base(data, rowIndex, colIndex)
        {
        }

        public ExcelGridStyleInfoIdentity(GridVolatileCellStyles data, RowColumnIndex pos) : base(data, pos)
        {
        }

        public ExcelGridStyleInfoIdentity(GridVolatileCellStyles data, int rowIndex, int colIndex, bool offLine) : base(data, rowIndex, colIndex, offLine)
        {
        }

        public ExcelGridStyleInfoIdentity(GridVolatileCellStyles data, RowColumnIndex pos, bool offLine) : base(data, pos, offLine)
        {
        }

        protected ExcelGridStyleInfoIdentity(GridStyleInfoIdentity other) : base(other)
        {
        }
    }

    //public class SpreadsheetGridRenderStyleInfo : GridRenderStyleInfo
    //{
    //    public SpreadsheetGridRenderStyleInfo(GridControlBase gridControl, GridStyleInfo modelStyle)
    //        : base(gridControl, modelStyle)
    //    {
    //    }
    //}

    public interface ISpreadsheetGridVisualStyle
    {
        /// <summary>
        /// Gets the header background.
        /// </summary>
        /// <value>The header background.</value>
        Brush HeaderBackgroundBrush { get; }

        /// <summary>
        /// Gets the header foreground.
        /// </summary>
        /// <value>The header foreground.</value>
        Brush HeaderForegroundBrush { get; }

        /// <summary>
        /// Gets the highlight header background.
        /// </summary>
        /// <value>The highlight header background.</value>
        Brush SelectionHeaderBackgroundBrush { get; }

        /// <summary>
        /// Gets the row couumn highlight selection header background.
        /// </summary>
        /// <value>The highlight header background.</value>
        Brush RowColumnSelectionHeaderBackgroundBrush { get; }
    }

    public class DefaultSpreadsheetGridVisualStyle : ISpreadsheetGridVisualStyle
    {
        private Brush headerBackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 218, 231, 245));
        private Brush headerForegroundBrush = new SolidColorBrush(Colors.Black); 
        private Brush excelOrange = new SolidColorBrush() { Color = SpreadsheetGrid.StringToColor("#FFFFDC61") };
        private Brush excelRowSelectionBlue = new SolidColorBrush(Color.FromArgb(255, 199, 206, 214));

        /// <summary>
        /// Gets the header background.
        /// </summary>
        /// <value>The header background.</value>
        public Brush HeaderBackgroundBrush
        {
            get { return headerBackgroundBrush; }
        }

        /// <summary>
        /// Gets the header foreground.
        /// </summary>
        /// <value>The header foreground.</value>
        public Brush HeaderForegroundBrush
        {
            get { return headerForegroundBrush; }
        }

        /// <summary>
        /// Gets the highlight header background.
        /// </summary>
        /// <value>The highlight header background.</value>
        public Brush SelectionHeaderBackgroundBrush
        {
            get{ return excelOrange; }
        }

        /// <summary>
        /// Gets the row couumn highlight selection header background.
        /// </summary>
        /// <value>The highlight header background.</value>
        public Brush RowColumnSelectionHeaderBackgroundBrush
        {
            get { return excelRowSelectionBlue; }
        }
    }

    public class Office2010BlueSpreadsheetGridVisualStyle : ISpreadsheetGridVisualStyle
    {
        private Brush headerBackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 218, 231, 245));
        private Brush headerForegroundBrush = new SolidColorBrush(Color.FromArgb(255, 30, 57, 91)); 
        private Brush excelOrange = new SolidColorBrush() { Color = SpreadsheetGrid.StringToColor("#FFFFDC61") };
        private Brush excelRowSelectionBlue = new SolidColorBrush(Color.FromArgb(255, 199, 206, 214));

        /// <summary>
        /// Gets the header background.
        /// </summary>
        /// <value>The header background.</value>
        public Brush HeaderBackgroundBrush
        {
            get { return headerBackgroundBrush; }
        }

        /// <summary>
        /// Gets the header foreground.
        /// </summary>
        /// <value>The header foreground.</value>
        public Brush HeaderForegroundBrush
        {
            get { return headerForegroundBrush; }
        }

        /// <summary>
        /// Gets the highlight header background.
        /// </summary>
        /// <value>The highlight header background.</value>
        public Brush SelectionHeaderBackgroundBrush
        {
            get { return excelOrange; }           
        }

        /// <summary>
        /// Gets the row couumn highlight selection header background.
        /// </summary>
        /// <value>The highlight header background.</value>
        public Brush RowColumnSelectionHeaderBackgroundBrush
        {
            get { return excelRowSelectionBlue; }
        }
    }

    public class Office2010SilverSpreadsheetGridVisualStyle : ISpreadsheetGridVisualStyle
    {
        private Brush headerBackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 223, 227, 232));
        private Brush headerForegroundBrush = new SolidColorBrush(Color.FromArgb(255, 56, 56, 56));
        private Brush excelOrange = new SolidColorBrush() { Color = SpreadsheetGrid.StringToColor("#FFFFDC61") };
        private Brush excelRowSelectionSilver = new SolidColorBrush(Color.FromArgb(255, 170, 170, 170));

        /// <summary>
        /// Gets the header background.
        /// </summary>
        /// <value>The header background.</value>
        public Brush HeaderBackgroundBrush
        {
            get { return headerBackgroundBrush; }
        }

        /// <summary>
        /// Gets the header foreground.
        /// </summary>
        /// <value>The header foreground.</value>
        public Brush HeaderForegroundBrush
        {
            get { return headerForegroundBrush; }
        }

        /// <summary>
        /// Gets the highlight header background.
        /// </summary>
        /// <value>The highlight header background.</value>
        public Brush SelectionHeaderBackgroundBrush
        {
            get { return excelOrange; }
        }

        /// <summary>
        /// Gets the row couumn highlight selection header background.
        /// </summary>
        /// <value>The highlight header background.</value>
        public Brush RowColumnSelectionHeaderBackgroundBrush
        {
            get { return excelRowSelectionSilver; }
        }
    }

    public class Office2010BlackSpreadsheetGridVisualStyle : ISpreadsheetGridVisualStyle
    {
        private Brush headerBackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 106, 106, 106));
        private Brush headerForegroundBrush = new SolidColorBrush(Color.FromArgb(255, 239, 240, 240));
        private Brush excelOrange = new SolidColorBrush() { Color = SpreadsheetGrid.StringToColor("#FFFFDC61") };
        private Brush excelRowSelectionBlack = new SolidColorBrush(Color.FromArgb(255, 186, 186, 186));

        /// <summary>
        /// Gets the header background.
        /// </summary>
        /// <value>The header background.</value>
        public Brush HeaderBackgroundBrush
        {
            get { return headerBackgroundBrush; }
        }

        /// <summary>
        /// Gets the header foreground.
        /// </summary>
        /// <value>The header foreground.</value>
        public Brush HeaderForegroundBrush
        {
            get { return headerForegroundBrush; }
        }

        /// <summary>
        /// Gets the highlight header background.
        /// </summary>
        /// <value>The highlight header background.</value>
        public Brush SelectionHeaderBackgroundBrush
        {
            get { return excelOrange; }
        }

        /// <summary>
        /// Gets the row couumn highlight selection header background.
        /// </summary>
        /// <value>The highlight header background.</value>
        public Brush RowColumnSelectionHeaderBackgroundBrush
        {
            get { return excelRowSelectionBlack; }
        }
    }
}
