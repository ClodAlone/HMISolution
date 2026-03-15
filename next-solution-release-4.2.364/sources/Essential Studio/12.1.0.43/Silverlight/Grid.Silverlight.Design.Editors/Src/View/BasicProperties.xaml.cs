#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Syncfusion.Grid.WPF.VisualStudio.Design
{

    public enum GridControlLengthUnitTypeView
    {
        //// Summary:
        //// No Sizing
        None = 0,
        ////
        //// Summary:
        ////     The unit of measure is based on the size of the cells and the column header.
        Auto,
        ////
        //// Summary:
        ////     The unit of measure is based on the size of the cells and the column heder with Last column fill.
        AutoWithLastColumnFill,
        ////
        //// Summary:
        ////     The unit of measure is based on the size of the cells.
        SizeToCells,
        ////
        //// Summary:
        ////     The unit of measure is based on the size of the column header.
        SizeToHeader,
        ////
        //// Summary:
        ////     The unit of measure is a weighted proportion of the available space.
        Star,
    }

    public enum GridSelectionFlagsView
    {
        /// <summary>
        /// Disable selecting cells.
        /// </summary>
        None = 0x00,
        /// <summary>
        /// Rows can be selected.
        /// </summary>
        Row = 0x01,
        /// <summary>
        /// Columns can be selected.
        /// </summary>
        Column = 0x02,
        /// <summary>
        /// Whole table can be selected.
        /// </summary>
        Table = 0x04,
        /// <summary>
        /// Indidvidual cells can be selected.
        /// </summary>
        Cell = 0x08,
        /// <summary>
        /// Multiple ranges of cells can be selected. The user has to press Control Key to select multiple ranges.
        /// </summary>
        Multiple = 0x10,
        /// <summary>
        /// Allow extend existing selection when user holds Shift Key and clicks on a cell.
        /// </summary>
        Shift = 0x20,
        /// <summary>
        /// Allow extend existing selection when user holds Shift Key and arrow keys.
        /// </summary>
        Keyboard = 0x40,
        /// <summary>
        /// Use alpha blending to highlight selected cells.
        /// </summary>
        //AlphaBlend = 0x80,
        /// <summary>
        /// Allow both rows and columns to be selected at same time when <see cref="Multiple"/> is specified. By default, the grid does not allow having rows and column
        /// ranges be selected at the same time.
        /// </summary>
        MixRangeType = 0x100,
        /// <summary>
        /// Default behavior for selecting cells: Rows, Columns, Table, Cell, Multiple, Extends Shift Key support, and alphablending.
        /// </summary>
        
        Any = 0xff,
    };

    public enum GridSelectionModeView
    {
        /// <summary>
        /// No items can be selected.
        /// </summary>
        None = 0,

        /// <summary>
        /// Only one item can be selected.
        /// </summary>
        One = 1,
        //
        // Summary:
        //     Multiple items can be selected.
        MultiSimple = 2,

        /// <summary>
        /// Multiple items can be selected, and the user can use the SHIFT, CTRL, and
        /// arrow keys to make selections
        /// </summary>
        MultiExtended = 3,
    }

    public enum GridCellActivateActionView
    {
        /// <summary>
        /// Do not set focus to text box.
        /// </summary>
        None = 0x00,
        /// <summary>
        /// Begin editing / focus on text box after user clicked on cell.
        /// </summary>
        /// <remarks>
        /// The cancelable <see cref="GridControlBase.CurrentCellStartEditing"/> event may block editing mode for the current cell.
        /// </remarks>
        ClickOnCell = 0x01,
        /// <summary>
        /// Begin editing / focus on text box whenever a cell becomes current cell no matter if user clicked on cell or moved with arrow keys.
        /// </summary>
        /// <remarks>
        /// When GridCellActivateAction.SetCurrent is specified <see cref="GridCurrentCell.BeginEdit"/> will be called
        /// before the <see cref="GridControlBase.CurrentCellActivated"/> event is raised. <para/>
        /// See the <see cref="GridControlBase.CurrentCellActivated"/> event if you want to programmatically call <see cref="GridCurrentCell.BeginEdit"/>.<para/>
        /// The cancelable <see cref="GridControlBase.CurrentCellStartEditing"/> event may block editing mode for the current cell.
        /// </remarks>
        SetCurrent = 0x02,
        /// <summary>
        /// Begin editing / focus on text box when user double clicked on cell.
        /// </summary>
        /// <remarks>
        /// The cancelable <see cref="GridControlBase.CurrentCellStartEditing"/> event may block editing mode for the current cell.
        /// </remarks>
        DblClickOnCell = 0x04,
        /// <summary>
        /// Begin editing / focus on text box and select all text whenever a cell becomes current cell no matter if user clicked on cell or moved with arrow keys.
        /// </summary>
        SelectAll = 0x0a,   // (0x08 | SetCurrent)
        /// <summary>
        /// Forward mouse click to the text box so that the caret can be positioned at the character under the mouse pointer.
        /// </summary>
        PositionCaret = 0x11     // (0x10 | ClickOnCell)
    }

    public enum VisualStyleView
    {
        /// <summary>
        /// Default skin.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Office 2007 Blue skin.
        /// </summary>
        Office2007Blue = 1,
        /// <summary>
        /// Office 2007 Silver skin.
        /// </summary>
        Office2007Silver = 2,
        /// <summary>
        /// Office 2007 Black skin.
        /// </summary>
        Office2007Black = 3,
        /// <summary>
        /// Office 2003 skin.
        /// </summary>
        Office2003 = 4,
        /// <summary>
        /// Blend skin.
        /// </summary>
        Blend = 5,
        /// <summary>
        /// Custom skin. Implement <see cref="IGridVisualStyle"/> interface for this skin.
        /// </summary>
        Custom = 6,
        /// <summary>
        /// Glassy Green skin.
        /// </summary>
        GlassyGreen = 7,
        /// <summary>
        /// Sun Black skin.
        /// </summary>
        SunBlack = 8,
        /// <summary>
        /// Shiny Red skin.
        /// </summary>
        ShinyRed = 9,
        /// <summary>
        /// Shiny Blue skin.
        /// </summary>
        ShinyBlue = 10,
        /// <summary>
        /// Bureau Blue skin.
        /// </summary>
        BureauBlue = 11,
        /// <summary>
        /// Bureau Black skin.
        /// </summary>
        BureauBlack = 12,
        /// <summary>
        /// Default Office 2007 Blue skin.
        /// </summary>
        DefaultOffice2007Blue = 14,
        /// <summary>
        /// Default Office 2007 Silver skin.
        /// </summary>
        DefaultOffice2007Silver = 15,
        /// <summary>
        /// Default Office 2007 Black skin.
        /// </summary>
        DefaultOffice2007Black = 16,
        /// <summary>
        /// Office 14 Blue skin.
        /// </summary>
        Office14Blue = 17,
        /// <summary>
        /// Office 14 Silver skin.
        /// </summary>
        Office14Silver = 18,
        /// <summary>
        /// Office 14 Black skin.
        /// </summary>
        Office14Black = 19
    }


    /// <summary>
    /// Interaction logic for BasicProperties.xaml
    /// </summary>
    public partial class BasicProperties : UserControl
    {
        public BasicProperties()
        {
            InitializeComponent();
        }

        private Expander expander;

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            if (expander != null && expander != ((Expander)sender))
            {
                expander.IsExpanded = false;
            }

            expander = sender as Expander;
        }
    }
}
