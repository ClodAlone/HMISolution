#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.XlsIO.Drawing
{
    internal class SizeProperties
    {
        #region Static Member
        internal static float FULL_COLUMN_OFFSET = 1024;
        internal static float FULL_ROW_OFFSET = 256;
        #endregion

        #region Members
        private int m_left;
        private int m_top;
        private int m_bottom;
        private int m_right;
        private int m_leftColumn;
        private int m_rightColumn;
        private int m_topRow;
        private int m_bottomRow;
        private PlacementType m_placementType;
        #endregion

        #region Intialization
        internal SizeProperties()
        {
            // TODO: Complete member initialization
            this.m_placementType = PlacementType.MoveAndSize;
        }
        #endregion

        #region Methods
        internal void SetBottomRow(int bottomRow)
        {
            this.m_bottomRow = bottomRow;
        }

        internal int GetRightColumn()
        {
            return this.m_rightColumn;
        }

        internal void SetRightColumn(int rightColumn)
        {
            this.m_rightColumn = rightColumn;
        }

        internal PlacementType GetPlacementType()
        {
            return this.m_placementType;
        }

        internal void SetPlacementType(PlacementType placementType)
        {
            if (this.m_placementType != placementType)
            {
                this.m_placementType = placementType;
            }
        }

        internal int GetTopRow()
        {
            return this.m_topRow;
        }

        internal void SetTopRow(int topRow)
        {
            this.m_topRow = topRow;
        }

        internal int GetLeftColumn()
        {
            return this.m_leftColumn;
        }

        internal void SetLeftColumn(int leftColumn)
        {
            this.m_leftColumn = leftColumn;
        }

        internal int GetBottomRow()
        {
            return this.m_bottomRow;
        }
        #endregion

        #region Properties
        internal int Bottom
        {
            get
            {
                return this.m_bottom;
            }
            set
            {
                this.m_bottom = value;
            }
        }

        internal int Left
        {
            get
            {
                return this.m_left;
            }
            set
            {
                this.m_left = value;
            }
        }

        internal int Right
        {
            get
            {
                return this.m_right;
            }
            set
            {
                this.m_right = value;
            }
        }

        internal int Top
        {
            get
            {
                return this.m_top;
            }
            set
            {
                this.m_top = value;
            }
        }
        #endregion
    }
   
}
