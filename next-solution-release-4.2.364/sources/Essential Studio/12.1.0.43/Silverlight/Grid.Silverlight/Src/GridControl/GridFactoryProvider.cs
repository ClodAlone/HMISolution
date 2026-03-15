#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !WinRT
namespace Syncfusion.Windows.Controls.Grid
#else
namespace Syncfusion.WinRT.Controls.Grid
#endif
{
    /// <summary>
    /// Lets you specify a custom <see cref="IGridCellModelFactory"/> that instantiates
    /// cell models for the grid on demand using the <see cref="GridStyleInfo.CellType"/>
    /// string as identifier.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridFactoryProvider
    {
        static GridFactoryProvider()
        {
        }

        /// <summary>
        /// Lets you specify a custom <see cref="IGridCellModelFactory"/> that instantiates
        /// cell models for the grid on demand using the <see cref="GridStyleInfo.CellType"/>
        /// string as identifier.
        /// </summary>
        public static void Init(IGridCellModelFactory pFactory)
        {
            cellModelFactory = pFactory;
        }

        /// <summary>
        /// Returns the <see cref="IGridCellModelFactory"/> for this process.
        /// </summary>
        public static IGridCellModelFactory CellModelFactory
        {
            get
            {
                return cellModelFactory;
            }
        }

        // Control Factory
        static IGridCellModelFactory cellModelFactory;
    }

}
