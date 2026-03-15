#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Collections.Generic;

namespace Syncfusion.Windows.Tools.Controls
{
    public class TextPosition
    {
        private DocumentAdv documentAdv;
        private string index = "b0=>0";
        internal double blockindex = double.NaN;
        internal double sectionIndex = double.NaN;
        internal double paragraphIndex = double.NaN;
        private LineInfo lineInfo = null;
        private ParagraphAdv paragraph = null;
        private Point point;
        private bool isPositionAtParagraphEnd = false;
        private bool isPositionAtTableStart = false;
        private bool isPositionAtTableEnd = false;
        private bool tempPositiontableEnd = false;
        private bool isinsidetable = false;
        
        public TextPosition(DocumentAdv document)
        {
            Document = document;
        }

        internal LineInfo LineInfo
        {
            get
            {
                if (((lineInfo == null && paragraph != null) || (lineInfo != null && paragraph != null && !paragraph.LineInfo.Contains(lineInfo))) && paragraph.LayoutViewer != null)
                {
                    return paragraph.GetLayoutViewer().OwnerControl.PositionHandler.GetLineFromIndex(index, paragraph);
                }
                return lineInfo;
            }
            set
            {
                lineInfo = value;
            }
        }

        /// <summary>
        /// Gets the Document
        /// </summary>
        public DocumentAdv Document
        {
            get
            {
                return documentAdv;
            }
            internal set
            {
                documentAdv = value;
                if (documentAdv != null && documentAdv.Sections.Count > 0 && documentAdv.Sections[0].Blocks.Count > 0)
                {
                    if (Document.Sections[0].Blocks[0] is ParagraphAdv)
                    {
                        Paragraph = Document.Sections[0].Blocks[0] as ParagraphAdv;
                        Paragraph.Section = Document.Sections[0];
                    }
                    else if (Document.Sections[0].Blocks[0] is TableAdv)
                    {
                        BlockAdv blk = (Document.Sections[0].Blocks[0] as TableAdv).GetFirstBlockInFirstCell() as ParagraphAdv;
                        while (blk is TableAdv)
                        {
                            blk = (blk as TableAdv).GetFirstBlockInFirstCell();
                        }
                        Paragraph = blk as ParagraphAdv;
                    }
                }
            }
        }

        internal double BlockIndex
        {
            get
            {
                return blockindex;
            }
            set
            {
                blockindex = value;

            }
        }

        internal bool IsInsideTable
        {
            get
            {
                if (Index != string.Empty)
                {
                    if (Index.Contains('r'))
                        return true;
                }
                return false;
            }
            set
            {
                isinsidetable = true;
            }
        }

        internal double ParagraphIndex
        {
            get
            {
                return paragraphIndex;
            }
            set
            {
                paragraphIndex = value;
            }
        }

        internal double SectionIndex
        {
            get
            {
                return sectionIndex;
            }
            set
            {
                sectionIndex = value;
            }
        }

        private string virtualPos = string.Empty;
        internal string VirtualPosition
        {
            get
            {
                return virtualPos;
            }
            set
            {
                virtualPos = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal ParagraphAdv Paragraph
        {
            get
            {
                return paragraph;
            }
            set
            {
                paragraph = value;
                if (paragraph != null)
                {
                    if (paragraph.IsInsideTable)
                    {
                        BlockIndex = GetRootBlock().Section.Blocks.IndexOf(GetRootBlock());
                    }
                    else if (paragraph.Section != null)
                    {
                        BlockIndex = Document.Sections[0].Blocks.IndexOf(paragraph);
                    }
                    SectionIndex = Document.Sections.IndexOf(paragraph.IsInsideTable ? GetRootBlock().Section : paragraph.Section);
                }
            }
        }

        internal BlockAdv GetRootBlock()
        {
            BlockAdv block = paragraph;

            if (paragraph.AssociatedCell != null)
            {
                block = paragraph.AssociatedCell.OwnerTable;
                while (block.IsTable)
                {
                    if (block.AssociatedCell != null)
                    {
                        block = (block as TableAdv).AssociatedCell.OwnerTable;
                    }
                    else
                        break;
                }
            }
            return block;
        }

        /// <summary>
        /// Indicates whether the position is at the start of the paragraph
        /// </summary>
        internal bool IsPositionAtParagraphStart
        {
            get
            {
                return ParseIndex() == 0;
            }
        }

        /// <summary>
        /// Gets or Sets the index in the Paragraph
        /// </summary>
        public string Index
        {
            get
            {
                return index;
            }
            internal set
            {
                index = value;
                Document.OwnerControl.IsPositionInsideTable = IsInsideTable;
                if (Paragraph != null)
                {
                    if (Paragraph.IsInsideTable)
                        ParagraphIndex = GetFinalParagraphIndex(index);
                    else
                        ParagraphIndex = paragraph.Section.Blocks.IndexOf(paragraph);
                }
            }
        }

        /// <summary>
        /// Returns a value indicating whether both TextPosition points same position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool IsEqual(TextPosition pos)
        {
            if (pos != null)
            {
                return Index == pos.Index && Paragraph == pos.Paragraph;
            }

            return false;
        }

        /// <summary>
        /// Gets or Sets the approximate point of the TextPosition
        /// </summary>
        internal Point Point
        {
            get
            {
                return point;
            }
            set
            {
                point = value;
            }
        }

        /// <summary>
        /// Indicates whether the position is at the end of the paragraph 
        /// </summary>
        public bool IsPositionAtParagraphEnd
        {
            get
            {
                if (Paragraph != null && Paragraph.Inlines.Count == 0)
                    return true;
                else if (Paragraph != null && Paragraph.Length() == ParseIndex().ToString())
                    return true;
                else if (IsZeroIndex() || (Paragraph !=null && Paragraph.Length()!=index))
                    isPositionAtParagraphEnd = false;

                return isPositionAtParagraphEnd;
            }
            set
            {
                isPositionAtParagraphEnd = value;
            }
        }

        public bool IsPositionAtTableStart
        {
            get
            {
                if (IsInsideTable)
                {
                    TableRowAdv row = null;
                    TableCellAdv cell = null;

                    BlockAdv blk = Document.GetBlockFromVirtualPosition(VirtualPosition, ref row, ref cell);
                    
                    if (blk != null && blk.IsTable)
                    {
                        if (row != null && cell !=null)
                        {
                            int rowindex = (blk as TableAdv).Rows.IndexOf(row);

                            int cellindex = row.Cells.IndexOf(cell);

                            return rowindex == 0 && cellindex == 0;
                        }
                    }
                }
                return isPositionAtTableStart;
            }
            set
            {
                isPositionAtTableStart = value;
            }
        }

        public bool IsPositionAtTableEnd
        {
            get
            {
                if (IsInsideTable)
                {
                    TableRowAdv row = null;
                    TableCellAdv cell = null;

                    BlockAdv blk = Document.GetBlockFromVirtualPosition(VirtualPosition, ref row, ref cell);

                    if (blk != null && blk.IsTable)
                    {
                        if (row != null && cell != null)
                        {
                            return row == (blk as TableAdv).Rows[(blk as TableAdv).Rows.Count - 1] && cell == row.Cells[row.Cells.Count - 1];
                        }
                        else
                            return false;
                    }
                }

                return isPositionAtTableEnd;
            }
            set
            {
                isPositionAtTableEnd = value;
            }
        }

        public bool TempTableEndPosition
        {
            get
            {
                return tempPositiontableEnd;
            }
            set
            {
                tempPositiontableEnd = value;
            }
        }

        /// <summary>
        /// Returns a value indicating whether both TextPosition are in same paragraph
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool IsInSameParagraph(TextPosition pos)
        {
            if (pos != null)
            {
                return Paragraph == pos.Paragraph;
            }
            return false;
        }


        internal bool IsInSameTable(TextPosition end)
        {
            TableRowAdv startrow=null;
            TableCellAdv startcell=null;

            TableRowAdv endrow=null;
            TableCellAdv endcell=null;

            BlockAdv startblk = Document.GetBlockFromVirtualPosition(VirtualPosition, ref startrow, ref startcell);

            BlockAdv endblk = Document.GetBlockFromVirtualPosition(end.VirtualPosition, ref endrow, ref endcell);

            if (startblk != null && endblk != null)
            {
                if (startblk.IsTable && endblk.IsTable && startblk == endblk)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a value indicating whether both TextPosition are in same section
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool IsInSameSection(TextPosition pos)
        {
            if (pos != null && Paragraph != null)
            {
                return Paragraph.Section == pos.Paragraph.Section;
            }

            return false;
        }

        /// <summary>
        /// Returns a value indicating whether both TextPosition are in same Document
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool IsInSameDocument(TextPosition pos)
        {
            if (pos != null)
            {
                return Document == pos.Document;
            }

            return false;
        }

        public bool IsGreaterThan(TextPosition pos)
        {
            if (pos != null)
            {
                if (IsSectionGreater(pos.SectionIndex))
                    return true;
                else if (SectionIndex < pos.SectionIndex)
                    return false;
                if (SectionIndex == pos.SectionIndex && IsBlockIndexGreater(pos.BlockIndex))
                    return true;
                else if (BlockIndex < pos.BlockIndex)
                    return false;

                if (BlockIndex == pos.BlockIndex && IsGreaterThan(pos.index))
                    return true;
                if (ParseIndex() < pos.ParseIndex())
                    return false;
            }

            return false;
        }

        internal bool IsZeroIndex()
        {
            if (ParseIndex() == 0)
                return true;
            return false;
        }

        internal int ParseIndex()
        {
            if (Index.Contains(">"))
            {
                return int.Parse(Index.Substring(Index.LastIndexOf(">") + 1));
            }
            else
            {
                return int.Parse(Index);
            }
        }

        internal int ParseIndex(string m_index)
        {
            if (m_index.Contains(">"))
            {
                return int.Parse(m_index.Substring(m_index.LastIndexOf(">") + 1));
            }
            else
            {
                return int.Parse(m_index);
            }
        }

        internal int GetFinalParagraphIndex(string m_index)
        {
            if (m_index.Contains("b") && m_index.Contains(">"))
            {
                if (m_index.LastIndexOf("b") + 1 == m_index.LastIndexOf(">") - 2)
                {
                    return int.Parse(m_index.Substring(m_index.LastIndexOf("b") + 1, 1));
                }
                else
                {
                    return int.Parse(m_index.Substring(m_index.LastIndexOf("b") + 1, m_index.LastIndexOf(">") - 2 - m_index.LastIndexOf("b")));
                }
            }
            else
            {
                return m_index != string.Empty ? int.Parse(m_index) : 0;
            }
        }

        internal string StepUp(int value)
        {
            int num = ParseIndex();
            num = num + value;
            string removed = Index.Remove(Index.LastIndexOf(">") + 1);
            return removed + num.ToString();
        }

        internal string StepDown(int value)
        {
            int num = ParseIndex();
            num = num - value;
            string removed = Index.Remove(Index.LastIndexOf(">") + 1);
            return removed + num;
        }

        /// <summary>
        /// Returns a value whether this section is greater than specified section index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal bool IsSectionGreater(double index)
        {
            return SectionIndex > index;
        }

        /// <summary>
        /// Returns a value whether this paragraph is greater than specified paragraph index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal bool IsBlockIndexGreater(double index)
        {
            return BlockIndex > index;
        }

        /// <summary>
        /// Returns a value whether this Index is greater than specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal bool IsIndexGreater(TextPosition post)
        {
            return ParseIndex() > post.ParseIndex();
        }

        /// <summary>
        /// Returns the copy of the current position
        /// </summary>
        /// <returns></returns>
        public TextPosition Copy()
        {
            TextPosition pos = new TextPosition(Document);
            pos.Index = Index;
            pos.Paragraph = Paragraph;
            pos.Point = Point;
            pos.LineInfo = LineInfo;
            return pos;
        }

        private void UpdateParagraphIndex()
        {
            if (paragraph != null)
            {
                if (paragraph.IsInsideTable)
                {
                    BlockIndex = GetRootBlock().Section.Blocks.IndexOf(GetRootBlock());
                    SectionIndex = 0;
                }
                else
                {
                    BlockIndex = paragraph.Section.Blocks.IndexOf(paragraph);
                    SectionIndex = Document.Sections.IndexOf(paragraph.Section);
                }
            }
            else
            {
                BlockIndex = double.NaN;
                SectionIndex = double.NaN;
            }
        }

        public TextPosition CopyForHistory()
        {
            UpdateParagraphIndex();
            TextPosition pos = new TextPosition(Document);
            pos.Index = Index;
            pos.BlockIndex = BlockIndex;
            pos.SectionIndex = SectionIndex;
            pos.VirtualPosition = VirtualPosition;
            pos.IsPositionAtParagraphEnd = IsPositionAtParagraphEnd;
            pos.IsPositionAtTableStart = IsPositionAtTableStart;
            pos.TempTableEndPosition = IsPositionAtTableEnd;
            pos.IsInsideTable = IsInsideTable;
            pos.Paragraph = null;
            return pos;
        }

        internal void SetIndex(string value)
        {
            string tempstring = string.Empty;
            BlockAdv tempParagraph = null;
            int blockindex = 0;
            if (Paragraph.IsInsideTable)
            {
                tempParagraph = Paragraph;
                TableAdv table = tempParagraph.AssociatedCell.OwnerTable;
                int parentindex = 0;
                while (table != null)
                {
                    tempstring = "r" + table.Rows.IndexOf(tempParagraph.AssociatedCell.OwnerRow) + "=>c" +
                           tempParagraph.AssociatedCell.OwnerRow.Cells.IndexOf(tempParagraph.AssociatedCell) + tempstring;
                    if (table.AssociatedCell != null)
                    {
                        parentindex = table.AssociatedCell == null ? Document.Sections[0].Blocks.IndexOf(table) : table.AssociatedCell.Blocks.IndexOf(table);
                        tempParagraph = table;
                        table = table.AssociatedCell.OwnerTable;
                        tempstring = "=>b" + parentindex + "=>" + tempstring;
                    }
                    else
                    {
                        blockindex = table.Section.Blocks.IndexOf(table);
                        break;
                    }
                }
                Index = "b" + blockindex + "=>" + tempstring + "=>b" + Paragraph.AssociatedCell.Blocks.IndexOf(Paragraph) + "=>" + value;
            }
            else
            {
                blockindex = Paragraph.Section.Blocks.IndexOf(Paragraph);
                Index = "b" + blockindex + "=>" + value;
            }
        }

        public TextPosition GetPositionUsingIndex()
        {
            TextPosition pos = new TextPosition(Document);
            pos.Index = Index;
            if (Document.Sections.Count > SectionIndex)
            {
                SectionAdv section = Document.Sections[(int)SectionIndex];
                if (section.Blocks.Count > BlockIndex)
                {
                    IterateBlocksInTable(section.Blocks, ref pos);
                    return pos;
                }
            }

            return null;
        }

        private bool IsGreaterThan(string index)
        {
            List<int> list1 = Parse(this.Index);
            List<int> list2 = Parse(index);

            while (list1.Count != 0 && list2.Count != 0)
            {
                if (list1[0] > list2[0])
                    return true;
                else if (list1[0] == list2[0])
                {
                    list1.RemoveAt(0);
                    list2.RemoveAt(0);
                }
                else if (list1[0] < list2[0])
                    return false;
            }
            return false;
        }

        private List<int> Parse(string index)
        {
            List<int> list = new List<int>();
            string[] str = index.Split(new string[] { "=>" }, StringSplitOptions.None);

            if (!(str.Length == 0))
            {
                for (int i = 0; i < str.Length; i++)
                {
                    list.Add(GetInteger(str[i]));
                }
            }
            else
            {
                list.Add(GetInteger(index));
            }

            return list;
        }

        private int GetInteger(string str)
        {
            int index = 0;
            if (int.TryParse(str, out index))
            {
                return index;
            }
            else
            {
                return int.Parse(str.Substring(1));
            }
        }

        internal void IterateBlocksInTable(BlockCollection<BlockAdv> blocks, ref TextPosition Pos)
        {
            foreach (BlockAdv block in blocks)
            {
                if (block is ParagraphAdv)
                {
                    if (block == Paragraph)
                    {
                        Pos.Paragraph = block as ParagraphAdv;
                    }
                }
                else if (block is TableAdv)
                {
                    foreach (LineInfo line in block.LineInfo)
                    {
                        foreach (ElementBox box in line.ElementBoxes)
                        {
                            IterateBlocksInTable((box as TableCellElementBox).CellBlocks, ref Pos);
                        }
                    }
                }
            }
        }


        internal BlockAdv IndexParser(ref TableCellAdv associatecell)
        {
            if (IsInsideTable)
            {
                List<string> ids = Index.Split(new string[] { "=>" }, StringSplitOptions.None).ToList();
                TableRowAdv row = null;
                BlockAdv blk = null;
                for (int j = 0; j < ids.Count; j++)
                {
                    string s = ids[j];
                    if (j == 0)
                    {
                        blk = Document.Sections[0].Blocks[int.Parse(ids[j].Substring(1, 1))];
                        continue;
                    }
                    else if (s.Contains("r"))
                    {
                        row = (blk as TableAdv).Rows[int.Parse(ids[j].Substring(1, 1))];
                    }
                    else if (s.Contains("c"))
                    {
                        associatecell = row.Cells[int.Parse(ids[j].Substring(1, 1))];
                    }
                    else if (s.Contains("b"))
                    {
                        blk = associatecell.Blocks[int.Parse(ids[j].Substring(1, 1))];
                    }
                }
                return blk;
            }
            else
            {
                return Paragraph;
            }
        }
    }
}
