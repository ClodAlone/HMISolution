#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript.Shared.Serializer;

using Syncfusion.JavaScript.Shared;


namespace Syncfusion.JavaScript.Models
{
    public class GridPropertiesBase<T> where T : class
    {
        #region Fields

        private bool allowPaging = false;
        private bool allowFiltering = false;
        private bool allowSelection = true;
        private bool allowSorting = false;
        private int selectedRow = -1;
        private object dataSource = new object();
        private string query = null;
        private string cssClass = null;
        private bool enablePersist = false;
        private bool allowScrolling = false;
       
        private String load = null;
        private String create = null;
        private String destroy = null;
        private String actionbegin = null;
        private String actioncomplete = null;
        private String rowdatabound = null;
        private String rowselecting = null;
        private String rowselected = null;
        private String querycellinfo = null;

        #endregion
        public GridPropertiesBase() { }
        

        #region Properties

        [JsonProperty("allowPaging")]
        [DefaultValue(false)]
        public bool AllowPaging
        {
            get { return this.allowPaging; }
            set { this.allowPaging = value; }
        }
        [JsonProperty("allowSorting")]
        [DefaultValue(false)]
        public bool AllowSorting
        {
            get { return this.allowSorting; }
            set { this.allowSorting = value; }
        }
        [JsonProperty("allowFiltering")]
        [DefaultValue(false)]
        public bool AllowFiltering
        {
            get { return this.allowFiltering; }
            set { this.allowFiltering = value; }
        }
       
        [JsonProperty("allowSelection")]
        [DefaultValue(true)]
        public bool AllowSelection
        {
            get { return this.allowSelection; }
            set { this.allowSelection = value; }
        }
       
        [JsonProperty("selectedRow")]
        [DefaultValue(-1)]
        public int SelectedRow
        {
            get { return this.selectedRow; }
            set { this.selectedRow = value; }
        }
        [JsonProperty("dataSource")]
        [JsonConverter(typeof(DataManagerConverter))]
        public object DataSource
        {
            get { return this.dataSource; }
            set { this.dataSource = value; }
        }

        [JsonProperty("query")]
        [DefaultValue(null)]
        [JsonConverter(typeof(QueryConverter))]
        public string Query
        {
            get { return this.query; }
            set { this.query = value; }
        }

      
        [JsonProperty("cssClass")]
        [DefaultValue(null)]
        public string CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }
        [JsonProperty("persist")]
        [DefaultValue(false)]
        public bool EnablePersist
        {
            get { return this.enablePersist; }
            set { this.enablePersist = value; }
        }
       
        [JsonProperty("allowScrolling")]
        [DefaultValue(false)]
        public bool AllowScrolling
        {
            get { return this.allowScrolling; }
            set { this.allowScrolling = value; }
        }
       
        
        [JsonProperty("load")]
        [DefaultValue(null)]
        public String Load
        {
            get { return this.load; }
            set { this.load = value; }
        }
        [JsonProperty("create")]
        [DefaultValue(null)]
        public String Create
        {
            get { return this.create; }
            set { this.create = value; }
        }
        [JsonProperty("destroy")]
        [DefaultValue(null)]
        public String Destroy
        {
            get { return this.destroy; }
            set { this.destroy = value; }
        }
        [JsonProperty("actionBegin")]
        [DefaultValue(null)]
        public String ActionBegin
        {
            get { return this.actionbegin; }
            set { this.actionbegin = value; }
        }
        [JsonProperty("actionComplete")]
        [DefaultValue(null)]
        public String ActionComplete
        {
            get { return this.actioncomplete; }
            set { this.actioncomplete = value; }
        }
        [JsonProperty("rowDataBound")]
        [DefaultValue(null)]
        public String RowDataBound
        {
            get { return this.rowdatabound; }
            set { this.rowdatabound = value; }
        }
        [JsonProperty("rowSelecting")]
        [DefaultValue(null)]
        public String RowSelecting
        {
            get { return this.rowselecting; }
            set { this.rowselecting = value; }
        }
        [JsonProperty("rowSelected")]
        [DefaultValue(null)]
        public String RowSelected
        {
            get { return this.rowselected; }
            set { this.rowselected = value; }
        }
        
        [JsonProperty("queryCellInfo")]
        [DefaultValue(null)]
        public string QueryCellInfo
        {
            get { return this.querycellinfo; }
            set { this.querycellinfo = value; }
        }
        #endregion


    }
}
