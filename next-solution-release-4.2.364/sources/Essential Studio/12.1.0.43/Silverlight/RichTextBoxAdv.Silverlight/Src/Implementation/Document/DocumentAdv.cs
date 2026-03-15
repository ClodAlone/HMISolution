#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Windows.Markup;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Syncfusion.Windows.Tools.Controls
{
    [ContentProperty("Sections")]
    public class DocumentAdv : DependencyObject
    {

        #region Members

        private RichTextBoxAdv ownerControl;
        private BlockCollection<SectionAdv> sections;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public DocumentAdv()
        {
            sections = new BlockCollection<SectionAdv>();
        }

        #region Properties

        /// <summary>
        /// It gets / sets the RichTextBoxControl
        /// </summary>
        internal RichTextBoxAdv OwnerControl
        {
            get
            {
                return ownerControl;
            }
            set
            {
                ownerControl = value;
            }
        }

        /// <summary>
        /// It returns the sections collection.
        /// </summary>
        public BlockCollection<SectionAdv> Sections
        {
            get
            {
                return sections;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Document is empty
        /// </summary>
        internal bool IsEmpty
        {
            get
            {
                if (Sections.Count == 0)
                {
                    return true;
                }
                else
                {
                    foreach (SectionAdv section in Sections)
                    {
                        if (section.Blocks.Count > 0)
                            return false;
                    }

                    return true;
                }
            }
        }

        /// <summary>
        /// Gets or Sets the page content margin
        /// </summary>
        public Thickness PageContentMargin
        {
            get
            {
                return (Thickness)GetValue(PageContentMarginProperty);
            }
            set
            {
                SetValue(PageContentMarginProperty, value);
            }
        }

        /// <summary>
        /// Registers page content margin dependency property
        /// </summary>
        public static readonly DependencyProperty PageContentMarginProperty = DependencyProperty.Register("PageContentMargin", typeof(Thickness), typeof(RichTextBoxAdv), new PropertyMetadata(new Thickness(0)));


        #endregion

        /// <summary>
        /// Returns the Section from Paragraph
        /// </summary>
        /// <param name="paragraph"></param>
        /// <returns></returns>
        internal SectionAdv GetSection(BlockAdv paragraph)
        {
            if (paragraph != null)
            {
                foreach (SectionAdv section in sections)
                {
                    if (section.Blocks.Contains(paragraph))
                    {
                        return section;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the Section from the TextPosition
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        internal SectionAdv GetSection(TextPosition pos)
        {
            if (pos != null)
            {
                if (Sections.Count > pos.SectionIndex)
                {
                    return Sections[(int)pos.SectionIndex];
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the section from the index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal SectionAdv GetSection(double index)
        {
            if (Sections.Count > index)
            {
                return Sections[(int)index];
            }

            return null;
        }

        /// <summary>
        /// Returns the index of the Paragraph
        /// </summary>
        /// <param name="paragraph"></param>
        /// <returns></returns>
        internal int GetIndexoFParagraph(BlockAdv paragraph)
        {
            foreach (SectionAdv section in sections)
            {
                if (section.Blocks.Contains(paragraph))
                {
                    return section.Blocks.IndexOf(paragraph);
                }
            }

            return -1;
        }

        /// <summary>
        /// Removes all the lines
        /// </summary>
        internal void ClearLines()
        {
            foreach (SectionAdv section in Sections)
            {
                foreach (BlockAdv block in section.Blocks)
                {
                    block.ClearLines();
                }
            }
        }

        /// <summary>
        /// It gets the Block from the TextPosition
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        internal BlockAdv GetBlock(TextPosition pos)
        {
            if (pos != null)
            {
                if (Sections.Count > 0 && pos.SectionIndex < Sections.Count)
                {
                    SectionAdv section = Sections[(int)pos.SectionIndex];

                    if (pos.IsInsideTable)
                    {
                        TableCellAdv cell = null;
                        BlockAdv block = null;
                        TableRowAdv row = null;
                        if (!IsElementAtPosition(pos, out cell, out row, out block))
                        {
                            if (cell == null && row == null && block != null)
                            {
                                if (block.IsInsideTable)
                                {
                                    TableCellAdv associate = block.AssociatedCell;
                                    if (associate.Blocks.Contains(block))
                                    {
                                        int index = associate.Blocks.IndexOf(block);
                                        return associate.Blocks[index + 1];
                                    }
                                }
                                else
                                {
                                    int index = section.Blocks.IndexOf(block);
                                    return section.Blocks[index + 1];
                                }
                            }
                            else if (cell == null && row == null && block == null)
                            {
                                if (pos.IsPositionAtTableStart)
                                {
                                    int index = 0;
                                    TableCellAdv tablecell = GetAssociatedCellWithIndex(pos.VirtualPosition, ref index);
                                    if (tablecell != null)
                                    {
                                        return tablecell.Blocks[index];
                                    }
                                    else
                                    {
                                        return section.Blocks[index];
                                    }
                                }
                            }
                        }
                        else
                        {
                            return GetBlockFromVirtualPosition(pos.Index, ref row, ref cell); ;
                        }
                    }
                    else if (section.Blocks.Count > 0 && pos.BlockIndex < section.Blocks.Count)
                    {
                        if (pos.BlockIndex >= 0)
                        {
                            return section.Blocks[(int)pos.BlockIndex];
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Check Is Element at given text position
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="outcell"></param>
        /// <param name="outrow"></param>
        /// <param name="blk"></param>
        /// <returns></returns>
        private bool IsElementAtPosition(TextPosition pos,out TableCellAdv outcell,out TableRowAdv outrow ,out BlockAdv blk)
        {
            TableRowAdv row = null;
            TableCellAdv cell = null;

            outcell = null;
            blk = null;
            outrow = null;
            if (!string.IsNullOrEmpty(pos.VirtualPosition))
            {
                blk = GetBlockFromVirtualPosition(pos.VirtualPosition, ref row, ref cell);
                outcell = cell;
                outrow = row;
                if (blk != null)
                {
                    return row != null && cell != null;
                }
                else
                    return false;
            }
            return true;
        }

        /// <summary>
        /// It returns the End text position.
        /// </summary>
        /// <returns></returns>
        internal TextPosition DocumentEnd()
        {
            TextPosition pos = null;
            BlockAdv block = null;
            if (Sections.Count > 0 && Sections.Last().Blocks.Count > 0)
            {
                pos = new TextPosition(this);
                pos.Index = Sections.Last().Blocks.Last().Length();
                if (Sections.Last().Blocks.Last().IsTable)
                {
                    block = (Sections.Last().Blocks.Last() as TableAdv).GetLastBlockInLastCell();
                    while (block.IsTable)
                    {
                        block = (block as TableAdv).GetLastBlockInLastCell();
                    }
                }
                else if (Sections.Last().Blocks.Last().IsParagraph)
                {
                    block = Sections.Last().Blocks.Last();
                }
                pos.Paragraph = block as ParagraphAdv;
            }

            return pos;
        }

        /// <summary>
        /// It returns the Start of the TextPosition
        /// </summary>
        /// <returns></returns>
        internal TextPosition DocumentStart()
        {
            TextPosition pos = new TextPosition(this);
            BlockAdv block;
            if (Sections.First().Blocks.First().IsTable)
            {
                block = (Sections.First().Blocks.First() as TableAdv).GetFirstBlockInFirstCell();

                while (block.IsTable)
                {
                    block = (block as TableAdv).GetFirstBlockInFirstCell();
                }
                pos.Paragraph = block as ParagraphAdv;
                pos.SetIndex("0");
            }
            else if (Sections.First().Blocks.First().IsParagraph)
            {
                pos.Paragraph = Sections[0].Blocks[0] as ParagraphAdv;
                pos.SetIndex("0");
            }

            return pos;
        }

        /// <summary>
        /// It returns the Block from the VirtualPosition
        /// </summary>
        /// <param name="position"></param>
        /// <param name="inputrow"></param>
        /// <param name="inputcell"></param>
        /// <returns></returns>
        internal BlockAdv GetBlockFromVirtualPosition(string position,ref TableRowAdv inputrow,ref TableCellAdv inputcell)
        {
            List<string> ids = position.Split(new string[] { "=>" }, StringSplitOptions.None).ToList();
            TableRowAdv row = null;
            BlockAdv blk = null;
            TableCellAdv cell = null;
            try
            {
                for (int j = 0; j < ids.Count; j++)
                {
                    string s = ids[j];
                    if (!string.IsNullOrEmpty(s))
                    {
                        if (j == 0)
                        {
                            int blkvalue = 0;

                            if (int.TryParse(ids[j].Substring(1), out blkvalue))
                            {
                                if (blkvalue < Sections[0].Blocks.Count)
                                {
                                    blk = Sections[0].Blocks[blkvalue];
                                }
                                else
                                    blk = null;
                            }
                            continue;
                        }
                        else if (s.Contains("r"))
                        {
                            int rowvalue = 0;

                            if (int.TryParse(ids[j].Substring(1), out rowvalue))
                            {
                                if (blk.IsTable)
                                {
                                    if (rowvalue < (blk as TableAdv).Rows.Count)
                                    {
                                        row = (blk as TableAdv).Rows[rowvalue];
                                    }
                                    else
                                        row = null;
                                }
                                else
                                {
                                    row = null;
                                    blk = null;
                                }
                            }
                        }
                        else if (s.Contains("c"))
                        {
                            int cellvalue = 0;

                            if (int.TryParse(ids[j].Substring(1), out cellvalue))
                            {
                                if (row != null)
                                {
                                    if (cellvalue < row.Cells.Count)
                                    {
                                        cell = row.Cells[cellvalue];
                                    }
                                    else
                                        cell = null;
                                }
                                else
                                    cell = null;
                            }
                        }
                        else if (s.Contains("b"))
                        {
                            int blkvalue = 0;

                            if (int.TryParse(ids[j].Substring(1), out blkvalue))
                            {
                                if (cell != null)
                                {
                                    if (blkvalue < cell.Blocks.Count)
                                    {
                                        blk = cell.Blocks[blkvalue];
                                    }
                                    else
                                        blk = null;
                                }
                                else
                                    blk = null;
                            }
                        }
                    }
                }
                inputrow = row;
                inputcell = cell;
                return blk;
            }
            catch 
            {

            }
            return null;
        }

        /// <summary>
        /// It returns the AssociateCell from the index
        /// </summary>
        /// <param name="position"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        internal TableCellAdv GetAssociatedCellWithIndex(string position, ref int index)
        {
            List<string> ids = position.Split(new string[] { "=>" }, StringSplitOptions.None).ToList();
            TableRowAdv row = null;
            BlockAdv blk = null;
            TableCellAdv cell = null;

            try
            {
                for (int j = 0; j < ids.Count; j++)
                {
                    string s = ids[j];
                    if (!string.IsNullOrEmpty(s))
                    {
                        if (j == 0)
                        {
                            int blkvalue = 0;

                            if (int.TryParse(ids[j].Substring(1), out blkvalue))
                            {
                                if (Sections[0].Blocks.Contains(Sections[0].Blocks[blkvalue]))
                                {
                                    blk = Sections[0].Blocks[blkvalue];
                                    index = int.Parse(ids[j].Substring(1));
                                }
                            }
                            continue;
                        }
                        else if (s.Contains("r"))
                        {
                            int rowvalue = 0;

                            if (int.TryParse(ids[j].Substring(1), out rowvalue))
                            {
                                if (blk.IsTable)
                                {
                                    if ((blk as TableAdv).Rows.Contains((blk as TableAdv).Rows[rowvalue]))
                                    {
                                        row = (blk as TableAdv).Rows[rowvalue];
                                    }
                                }
                            }
                        }
                        else if (s.Contains("c"))
                        {
                            int cellvalue = 0;

                            if (int.TryParse(ids[j].Substring(1), out cellvalue))
                            {
                                if (blk.IsTable)
                                {
                                    if (row.Cells.Contains(row.Cells[cellvalue]))
                                    {
                                        cell = row.Cells[cellvalue];
                                    }
                                }
                            }
                        }
                        else if (s.Contains("b"))
                        {
                            int blkvalue = 0;

                            if (int.TryParse(ids[j].Substring(1), out blkvalue))
                            {
                                if (cell.Blocks.Contains(cell.Blocks[blkvalue]))
                                {
                                    blk = cell.Blocks[blkvalue];
                                    index = int.Parse(ids[j].Substring(1));
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }

            return cell;
                
        }
    }
}
