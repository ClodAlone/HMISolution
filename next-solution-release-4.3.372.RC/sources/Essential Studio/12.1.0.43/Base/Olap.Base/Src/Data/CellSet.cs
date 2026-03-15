//-------------------------------------------------------------------------------------------------
// <copyright file="CellSet.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using Syncfusion.Olap.Engine;
using Syncfusion.Olap.Manager;
using Syncfusion.Olap.DataProvider;
using MASAC = Microsoft.AnalysisServices.AdomdClient;

namespace Syncfusion.Olap.Data
{
    /// <summary>
    /// The CellSet encapsulates a multidimensional result set that is the result of running a 
    /// command. A multidimensional result set contains a discrete collection of data points, 
    /// or cells, that are organized along multiple dimensions, or axes.
    /// </summary>
    /// <remarks>
    /// A cellset is created when ExecuteCellSet method of AdomdDataProvider is called to run 
    /// a command.
    /// </remarks>
    [Serializable]
    public class CellSet
    {
        #region Private Variables
        [NonSerialized]
        IDataProvider _dataProvider;
        AxisCollection axisCollection;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CellSet"/> class.
        /// </summary>
        /// <param name="dataProvider">The data provider.</param>
        public CellSet(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
            this.axisCollection = new AxisCollection(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CellSet"/> class.
        /// </summary>
        public CellSet()
        {
            this.axisCollection = new AxisCollection(this);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets an instance of the AxisCollection class that contains the axes of the CellSet.
        /// </summary>
        /// <value>Contains the Collection of Axis</value>
        [Description("Getsan instance of the AxisCollection class that contains the axes of the CellSet."), DefaultValue((string)null)]
        public AxisCollection Axes
        {
            get
            {
                return this.axisCollection;
            }
        }

        /// <summary>
        /// Gets the column max level.
        /// </summary>
        /// <value>The column max level.</value>
        public int ColumnMaxLevel
        {
            get
            {
                if (this.Axes.Count > 0)
                {
                    return GetAxisLevelsDepth(this.Axes[0]);
                }
                else
                    return 0;
            }
        }

        /// <summary>
        /// Gets or sets the member cell collection.
        /// </summary>
        /// <value>The member cell collection.</value>
        public CellCollection MemberCellCollection
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the row max level.
        /// </summary>
        /// <value>The row max level.</value>
        public int RowMaxLevel
        {
            get
            {
                if (this.Axes.Count > 1)
                {
                    return GetAxisLevelsDepth(this.Axes[1]);
                }
                else
                    return 0;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Gets the axis levels depth.
        /// </summary>
        /// <param name="cubeAxis">The cube axis.</param>
        /// <returns>Returns the Axis level Depth</returns>
        private int GetAxisLevelsDepth(Axis cubeAxis)
        {
            int currentLevel = 0;
            List<int> levelsArr = new List<int>();

            foreach (Tuple tuple in cubeAxis.TupleSet)
            {
                for (int colInd = 0; colInd < tuple.Members.Count; colInd++)
                {
                    int currLevel = tuple.Members[colInd].LevelDepth;
                    if (currLevel == 0)
                    {
                        currLevel++;
                    }

                    if (levelsArr.Count >= (colInd + 1))
                    {
                        if (levelsArr[colInd] < currLevel)
                        {
                            levelsArr[colInd] = currLevel;
                        }
                    }
                    else
                    {
                        levelsArr.Insert(colInd, currLevel);
                    }
                }
            }

            foreach (int l in levelsArr)
            {
                currentLevel += l;
            }

            return currentLevel;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the cell, based on the parent index passed.
        /// </summary>
        /// <param name="indexes">list of indexes</param>
        /// <returns>specified Cell</returns>
        public Cell GetCell(params int[] indexes)
        {
            return _dataProvider.GetCell(this, indexes);
        }

        /// <summary>
        /// Determines whether [has valid cells].
        /// </summary>
        /// <returns>
        /// <c>true</c> if [has valid cells]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasValidCells()
        {
            return _dataProvider.HasValidCells();
        }
        #endregion
    }
}
