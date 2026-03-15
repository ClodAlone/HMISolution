#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Syncfusion.XlsIO.Implementation.Collections;
#if  (SILVERLIGHT)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
#elif ( WINRT )
using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Implementation.WINRT;
#else
using System.Drawing;

#endif

#endregion


namespace Syncfusion.XlsIO.Implementation
{
    ///<exclude/>
    /// <summary>
    /// This class contains utility methods, that cannot be logically placed in any other class.
    /// </summary>
    public class OutlineWrapperUtility
    {
        #region Class Members
       
        /// <summary>
        /// Represents the outline levels dictionary collection
        /// </summary>
        private Dictionary<int, List<Point>> m_outlineLevels;
        /// <summary>
        /// Represents the outline level order
        /// </summary>
        private bool m_bIsLevelInOrder;

        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// 
        /// </summary>       
        public OutlineWrapperUtility(Dictionary<int, List<Point>> outlineLevels)
        {
            this.m_outlineLevels = outlineLevels;
        }

        #endregion

        #region Class Properties

        /// <summary>
        /// Returns or sets the outline dictionary collection
        /// </summary>
        public Dictionary<int, List<Point>> OutlineLevels
        {
            get
            {
                return m_outlineLevels;
            }
            set
            {
                m_outlineLevels = value;
            }
        }      
        #endregion

        #region Class methods
        public void UpdateOutlineRowStorage(WorksheetImpl sheet,Dictionary<int,int> levelAndIndexes)
        {
            Dictionary<int, List<Point>> groups = new Dictionary<int, List<Point>>();
            if (levelAndIndexes != null && levelAndIndexes.Count>0)
            {
                foreach(KeyValuePair<int,int> point in levelAndIndexes)
                {
                    int level = point.Value;
                        if (level == 0)
                            continue;
                        UpdateOutline(point.Key, level, groups);
                }
            }
            else
            {
                CellRecordCollection cells = sheet.CellRecords;
               

                for (int rowIndex = cells.FirstRow, iLen = cells.LastRow; rowIndex <= iLen; rowIndex++)
                {
                    if (cells.ContainsRow(rowIndex - 1))
                    {
                        RowStorage row = (RowStorage)cells.Table.Rows[rowIndex - 1];
                        int level = row.OutlineLevel;
                        if (level == 0)
                            continue;
                        UpdateOutline(rowIndex, level, groups);
                    }
                }
            }
            SortGroups(groups);
            
            {
                sheet.RowOutlineLevels = new Dictionary<int, List<Point>>();
                for(int i=1;i<=groups.Count;i++)
                {
                    sheet.RowOutlineLevels.Add(i, groups[i]);
                }
                sheet.OutlineLevelRow = (byte)groups.Count;
            }
          
        }
        public void UpdateOutlineColumn(WorksheetImpl sheet,Dictionary<int,int> indexAndLevels)
        {
            Dictionary<int, List<Point>> groups = new Dictionary<int, List<Point>>();
            if (indexAndLevels != null && indexAndLevels.Count > 0)
            {
                foreach (KeyValuePair<int, int> point in indexAndLevels)
                {
                    int level = point.Value;
                        if (level == 0)
                            continue;
                        UpdateOutline(point.Key, level, groups);
                }
            }
            SortGroups(groups);

            {
                sheet.ColumnOutlineLevels = new Dictionary<int, List<Point>>();
                for (int i = 1; i <= groups.Count; i++)
                {
                    sheet.ColumnOutlineLevels.Add(i, groups[i]);
                }
                sheet.OutlineLevelColumn =(byte) groups.Count;
            }

        }
        
        public void UpdateOutline(int index, int level,Dictionary<int,List<Point>> outlines)
        {
          List<Point> points=null;
            if (!outlines.ContainsKey(level))
            {
             
                points = new List<Point>();
                points.Add(new Point(index, index));
                outlines.Add(level, points);
                if (level > 1)
                {
                    UpdateInAllLevels(level, index, outlines);
                }
            }
            else
            {
                UpdateInAllLevels(level, index, outlines);
            }
        }
        public void UpdateInAllLevels(int level, int index, Dictionary<int, List<Point>> outlines)
        {
            List<Point> points = null;
            Point point, newPoint;
            int i = 0;
            for (int currentLevel = 1; currentLevel <= level; currentLevel++)
            {
                if (outlines.ContainsKey(currentLevel))
                {
                    points = outlines[currentLevel];
                    for (i = 0; i < points.Count; i++)
                    {
                        point = points[i];
                        if (point.Y == index - 1)
                        {
                            newPoint = new Point(point.X, index);
                            points.RemoveAt(i);
                            points.Add(newPoint);
                            break;
                        }
                        else if (point.Y == index)
                        {
                            break;
                        }
                    }
                    if (i == points.Count)
                    {
                        newPoint = new Point(index, index);
                        points.Add(newPoint);
                    }
                }
                else
                {
                    points = new List<Point>();
                    points.Add(new Point(index, index));
                    outlines.Add(currentLevel, points);
                }
            }
        }
        /// <summary>
        /// Combines the group into grouping levels
        /// </summary>
        public void AddInGroupLevels()
        {
            int lastLevel = m_outlineLevels.Count;
            SortGroups();
            if (m_outlineLevels.Count <= 1)
            {
                GroupingOneOutlineLevel();
            }
            else
            {

                int[] keyLevels = new int[m_outlineLevels.Keys.Count];
                m_outlineLevels.Keys.CopyTo(keyLevels, 0);
                if (keyLevels[0] < keyLevels[keyLevels.Length - 1])
                {
                    m_bIsLevelInOrder = true;
                }
                Array.Sort<int>(keyLevels);

                for (int level = keyLevels.Length - 1; level >= 1; level--)
                {
                    GroupingNearByRange(keyLevels[level], keyLevels[level - 1]);
                }
                
                UpdateRangeBasedOnLevel(keyLevels);
            }
        }

        /// <summary>
        /// Grouping for one grouping outline level
        /// </summary>
        private void GroupingOneOutlineLevel()
        {
            if (m_outlineLevels.Count == 1)
            {
                List<Point> points = new List<Point>();
                int[] levels = new int[m_outlineLevels.Keys.Count];
                m_outlineLevels.Keys.CopyTo(levels, 0);
                points = m_outlineLevels[levels[0]];
                int i;
                for (i = 0; i < points.Count-1; i++)
                {
                    if (points[i].Y == points[i + 1].X - 1)
                    {
                        Point point = new Point(points[i].X, points[i + 1].Y);
                        points.RemoveAt(i + 1);
                        points.RemoveAt(i);
                        points.Insert(i, point);
                        i = i - 1;
                    }
                    
                }               
            }
        }

        /// <summary>
        /// Sorts outline groups based on level and points
        /// </summary>
        private void SortGroups(Dictionary<int,List<Point>> groups)
        {

            for (int i = 1, count = groups.Count; i <= count; i++)
            {
                if (groups.ContainsKey(i))
                {
                    List<Point> points = groups[i];
                    Point[] pointArray = new Point[points.Count];
                    points.CopyTo(pointArray, 0);
                    Array.Sort<Point>(pointArray, new GroupLevelComparer());
                    points.Clear();
                    foreach (Point point in pointArray)
                        points.Add(point);
                }
            }
        }
        /// <summary>
        /// Combines the groups based on the group information
        /// </summary>
        /// <param name="outlineLevels">group collection dictionary</param>
        /// <param name="groupInfo">row group information array</param>
        internal void AddInGroupLevels(Dictionary<int, List<Point>> outlineLevels, int[][] groupInfo)
        {
            for (int i = 0; i < groupInfo.Length; i++)
            {
                Point point = new Point(groupInfo[i][1], groupInfo[i][2]);
                int level = groupInfo[i][0];
                if (outlineLevels.ContainsKey(level))
                {
                    List<Point> points = outlineLevels[level];
                    points.Add(point);
                }
                else
                {
                    List<Point> points = new List<Point>();
                    points.Add(point);
                    outlineLevels.Add(level, points);
                }
            }
        }

        /// <summary>
        /// Add each column outline level in to dictionary collection
        /// </summary>
        /// <param name="level">Outline level for the column</param>
        /// <param name="min">Min value of column</param>
        /// <param name="max">Mac vallue of column</param>
        /// <param name="outlineLevels">Outline collection dicationary</param>
        public void AddColumnLevel(int level, int min, int max, Dictionary<int, List<Point>> outlineLevels)
        {

            Point point = new Point(min, max);
            if (!outlineLevels.ContainsKey(level))
            {
                List<Point> points = new List<Point>();
                points.Add(point);
                outlineLevels.Add(level, points);
            }
            else
            {
                List<Point> points = outlineLevels[level];
                points.Add(point);
            }
        }
        /// <summary>
        /// Adds each row outline level to dictionary collection
        /// </summary>
        /// <param name="level">Level of an row</param>
        /// <param name="rowIndex">Row index value</param>
        /// <param name="outlineLevels">group dictionary collection</param>
        public void AddRowLevel(int level, int rowIndex, Dictionary<int, List<Point>> outlineLevels)
        {
            if (!outlineLevels.ContainsKey(level))
            {
                List<Point> points = new List<Point>();
                Point point = new Point(rowIndex, rowIndex);
                points.Add(point);
                outlineLevels.Add(level, points);
            }
            else
            {
                int i = 0;
                List<Point> points = outlineLevels[level];

                for ( i = 0; i < points.Count; i++)
                {
                    if (points[i].Y == rowIndex - 1)
                    {
                        Point point = new Point(points[i].X, rowIndex);
                        points.RemoveAt(i);
                        points.Insert(i, point);
                        break;
                    }
                   
                }
                if (i == points.Count)
                {
                    Point point = new Point(rowIndex, rowIndex);
                    points.Add(point);
                }
                
            }
        }
        /// <summary>
        /// Updating the grouping dictionary based on outline level
        /// </summary>
        /// <param name="levels">Array of outline levels</param>
        internal void UpdateRangeBasedOnLevel(int [] levels)
        {
            int startRange = 1;
            for (int level = levels.Length-2; level > 0; level--)
            {
                List<Point> points = m_outlineLevels[levels[level]];
                List<Point> postPoints = m_outlineLevels[levels[level + 1]];
                for (int i = 0; i < points.Count; i++)
                {
                    if (i == 0)
                    {
                        startRange = 1;
                    }
                    else
                    {
                        startRange = points[i - 1].Y;
                    }
                    Point point = points[i];
                    startRange = GroupByLimitRange(postPoints, startRange, point.Y);
                    if (point.X > startRange)
                    {
                        point.X = startRange;
                    }
                }
            }
        }

        /// <summary>
        /// Updates range based on the outline level
        /// </summary>
        internal void UpdateRangeBasedOnLevel() 
        {
            int startRange = 1;
            for (int level = m_outlineLevels.Count - 1; level > 0; level--)
            {
                List<Point> points = m_outlineLevels[level];
                List<Point> postPoints = m_outlineLevels[level + 1];
                for (int i = 0; i < points.Count; i++)
                {
                    if (i == 0)
                    {
                        startRange = 1;
                    }
                    else
                    {
                        startRange = points[i - 1].Y;
                    }
                    Point point = points[i];
                    startRange = GroupByLimitRange(postPoints, startRange, point.Y);
                    if (point.X > startRange)
                    {
                        point.X = startRange;
                    }
                }
            }
        }

        /// <summary>
        /// Sorts outline groups based on level and points
        /// </summary>
        private void SortGroups()
        {

            for (int i = 1, count = m_outlineLevels.Count; i <= count; i++)
            {
                if (m_outlineLevels.ContainsKey(i))
                {
                    List<Point> points = m_outlineLevels[i];
                    Point[] pointArray = new Point[points.Count];
                    points.CopyTo(pointArray, 0);
                    Array.Sort<Point>(pointArray, new GroupLevelComparer());
                    points.Clear();
                    foreach (Point point in pointArray)
                        points.Add(point);
                }
            }
        }

        /// <summary>
        /// Grouping the dictionary based on min and max points
        /// </summary>
        /// <param name="points">points list collection</param>
        /// <param name="min">min value</param>
        /// <param name="max">max value</param>
        /// <returns></returns>
        private int GroupByLimitRange(List<Point> points, int min, int max)
        {
            int startRange = int.MaxValue;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].Y < max)
                {
                    if (points[i].X > min)
                    {
                        if (startRange > points[i].X)
                        {
                            startRange = points[i].X;
                            break;
                        }
                    }
                    else
                        break;
                }
            }
            return startRange;
        }

        /// <summary>
        /// Grouping the ranges by nearest level
        /// </summary>
        /// <param name="level">Current outline level</param>
        /// <param name="preLevel">Previous outline level</param>
        private void GroupingNearByRange(int level, int preLevel) 
        {
            List<Point> points = m_outlineLevels[level];
            List<Point> prePoints = m_outlineLevels[preLevel];
            int startIndex = 0;
            int i = 0;
            foreach (Point point in points)
            {
                for (i = startIndex; i < prePoints.Count; i++)
                {
                    if (prePoints[i].Y == point.X - 1)
                    {
                        Point newPoint = new Point(prePoints[i].X, point.Y);                      
                        prePoints.RemoveAt(i);
                        prePoints.Insert(i, newPoint);
                        break;
                    }
                    else if (point.Y == prePoints[i].Y - 1 && !m_bIsLevelInOrder)
                    {
                        Point newPoint = new Point(point.X, prePoints[i].Y);
                        prePoints.RemoveAt(i);
                        prePoints.Insert(i, newPoint);
                        break;
                    }
                    else if (point.X == prePoints[i].Y - 1)
                    {
                        Point newPoint = new Point(point.X, prePoints[i].Y);
                        prePoints.RemoveAt(i);
                        prePoints.Insert(i, newPoint);
                    }
                    else if (point.Y == prePoints[i].X - 1 && !m_bIsLevelInOrder)
                    {
                        Point newPoint = new Point(point.X, prePoints[i].Y);
                        prePoints.RemoveAt(i);
                        prePoints.Insert(i, newPoint);
                        break;
                    }
                }
                if (i != prePoints.Count)
                {
                    startIndex = i + 1;
                   
                }
            }
            if (i != prePoints.Count)
            {
                if (i >= points.Count && i >= prePoints.Count)
                {
                    if (prePoints[i].X == points[i].X - 1 )
                    {
                        Point newPoint = new Point(points[i].X, points[i].Y);
                        points.RemoveAt(i);
                        points.Insert(i, newPoint);
                    }
                }
                
            }
            CheckAndCorrectCurrentRange(preLevel);
        }
        
        /// <summary>
        /// Check and corrects the outline range
        /// </summary>
        /// <param name="level">Outline level</param>
        private void CheckAndCorrectCurrentRange(int level)
        {
            List<Point> points = m_outlineLevels[level];
            int count = points.Count;
            if (count > 1)
            {
                for (int i = 0; i < points.Count - 1; i++)
                {                   
                    if (points[i].Y == points[i + 1].X - 1 ||
                        points[i].Y==points[i+1].X||
                        points[i].Y == points[i + 1].X + 1)
                    {
                        Point newPoint = new Point(points[i].X, points[i + 1].Y);                 
                        points.RemoveAt(i + 1);
                        points.RemoveAt(i);                        
                        points.Insert(i, newPoint);
                        
                        i = i - 1;
                    }
                    else if(points.Count>i+2 && (points[i+1].Y== points[i+2].X-1))
                    {
                        Point newPoint = new Point(points[i+1].X, points[i + 2].Y);
                        points.RemoveAt(i + 2);
                        points.RemoveAt(i + 1);
                        points.Insert(i + 1,newPoint);
                    }
                    else
                        break;
                }
            }

        }

        /// <summary>
        /// IComparer for comparing between two points
        /// </summary>
        class GroupLevelComparer : IComparer<Point>
        {
            public int Compare(Point minArray, Point maxArray)
            {
                int diff=minArray.X - maxArray.X;
                if (diff == 0)
                    diff=minArray.Y - maxArray.Y;
                return diff;
            }
        }
        #endregion
    }
}
