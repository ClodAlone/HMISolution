//-------------------------------------------------------------------------------------------------
// <copyright file="Report.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.OlapSilverlight.Base.Report
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    [DataContract]
    public class OlapReport 
    {
        #region Private Variables
        private Items _CategoricalElements;

        private string _CurrentCubeName;

        private Items _FilterElements;

        private Items _SeriesElements;

        //private bool _ShowGrandTotal;

        private Items _SlicerElements;

        private bool _TogglePivot;
        #endregion

        #region Constructor
        public OlapReport()
        {
            this.Name = string.Empty;
            this._CurrentCubeName = string.Empty;
            this.ShowEmptyRowData = false;
            this.ShowEmptyColumnData = false;
            //this.ShowGrandTotal = false;
            this.TogglePivot = false;
            this.CategoricalElements = new Items();
            this.SeriesElements = new Items();
            this.SlicerElements = new Items();
            this.FilterElements = new Items();
        }

        public OlapReport(string name)
        {
            this.Name = name;
            this._CurrentCubeName = string.Empty;
            this.ShowEmptyRowData = false;
            this.ShowEmptyColumnData = false;
            //this.ShowGrandTotal = false;
            this.TogglePivot = false;
            this.CategoricalElements = new Items();
            this.SeriesElements = new Items();
            this.SlicerElements = new Items();
            this.FilterElements = new Items();
        }
        #endregion

        #region Public Methods 
        [DataMember]
        public Items CategoricalElements
        {
            get
            {
                return _CategoricalElements;
            }

            set
            {
                _CategoricalElements = value;
            }
        }

        /// <summary>
        /// Gets or sets the engine version.
        /// </summary>
        /// <value>The QueryBuilder Engine version</value>
        [DataMember]
        public QueryBuilderEngineVersions EngineVersion { get; set; }

        [DataMember]
        public string CurrentCubeName
        {
            get
            {
                return _CurrentCubeName;
            }

            set
            {
                _CurrentCubeName = value;
            }
        }

        [DataMember]
        public Items FilterElements
        {
            get
            {
                return _FilterElements;
            }

            set
            {
                _FilterElements = value;
            }
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Items SeriesElements
        {
            get
            {
                return _SeriesElements;
            }

            set
            {
                _SeriesElements = value;
            }
        }

        [DataMember]
        public bool ShowEmptyColumnData { get; set; }

        [DataMember]
        public bool ShowEmptyRowData { get; set; }

        [DataMember]
        public bool ShowExpanders { get; set; }

        [DataMember]
        public Items SlicerElements
        {
            get
            {
                return _SlicerElements;
            }

            set
            {
                _SlicerElements = value;
            }
        }

        [DataMember]
        public bool TogglePivot
        {
            get
            {
                return _TogglePivot;
            }

            set
            {
                _TogglePivot = value;
            }
        }

        #endregion
    }
}
