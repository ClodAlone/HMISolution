#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace Syncfusion.Windows.Forms.TreeMap
{
   [Serializable]
    public partial class TreeMapLevel :  IDisposable, INotifyPropertyChanged
    {
        #region Constructor
        public TreeMapLevel()
        {
            DataSource = new List<object>();
            TreeMapItems = new List<TreeMapItem>();
        }

        #endregion

        #region  Properties

        #region GroupingPath

        private string groupingPath;
        internal string GroupingPath
        {
            get { return groupingPath; }
            set { groupingPath = value; }
        }

        #endregion

        #region GroupingGap
        internal double groupingGap;
        internal double GroupingGap
        {
            get { return groupingGap; }
            set { groupingGap = value; }
        }

     
        #endregion

        #region ItemsGap

        internal double itemsGap;
        public double ItemsGap
        {
            get { return itemsGap; }
            set
            {
                itemsGap = value;
            }

        }

        #endregion

        #region ColorMapping
        private ColorMapping colorMapping;
        public ColorMapping ColorMapping
        {
            get { return colorMapping; }
            set { colorMapping = value; }
        }

        #endregion

        #region HeaderHeight

        private double headerHeight = 0;
        public double HeaderHeight
        {
            get { return headerHeight; }
            set { headerHeight = value; }
        }

    
      
        #endregion

        #region LevelHeaderPath

        private string levelHeaderPath;
        internal string LevelHeaderPath
        {
            get { return levelHeaderPath; }
            set { levelHeaderPath = value; }
        }

     
        #endregion

     
        #region ShowLabels
        private bool showLabels;
        public bool ShowLabels
        {
            get { return showLabels; }
            set { showLabels = value; }
        }

        #endregion

        #region DataSource

        private List<object> dataSource;
        internal List<object> DataSource
        {
            get { return dataSource; }
            set { dataSource = value; }
        }

        #endregion

        #region TreeMapItems

        internal List<TreeMapItem> TreeMapItems
        {
            get;
            set;
        }

       
        #endregion

        #region LevelLabelPath

        private string levelLabelPath;
        internal string LevelLabelPath
        {
            get { return levelLabelPath; }
            set { levelLabelPath = value; }
        }

     
        #endregion

        #endregion

        #region IDisposable Method
        public void Dispose()
        {
            if (TreeMapItems != null)
            {
                TreeMapItems.Clear();
                TreeMapItems = null;
            }
            if (DataSource != null)
            {
                DataSource.Clear();
                DataSource = null;
            }
            ColorMapping = null;
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class TreeMapFlatLevel : TreeMapLevel
    {
        #region Dependency Properties

        #region GroupPath

        internal string groupPath;
        public string GroupPath
        {
            get { return groupPath; }
            set 
            {
                groupPath = value;
                base.GroupingPath = value;
            }
        }

      
        #endregion

        #region GroupGap

        internal double groupGap;
        public double GroupGap
        {
            get { return groupGap; }
            set
            {
                groupGap = value;
                base.GroupingGap = value;
            }

        }

        #endregion


        //#region ItemsGap
        //internal double itemsGap;
        //public double ItemsGap
        //{
        //    get { return itemsGap; }
        //    set
        //    {
        //        itemsGap = value;
        //        base.ItemsGap = value;
        //    }

        //}


        //#endregion

        #endregion
    }

    public class TreeMapHierarchicalLevel : TreeMapLevel
    {
        #region Dependency Properties

        #region ChildPath
        internal string childPath;
        public string ChildPath
        {
            get { return childPath; }
            set { childPath = value; }
        }

 
        #endregion

        #region ChildGap

        internal double childGap;
        public double ChildGap
        {
            get { return childGap; }
            set { childGap = value; }
        }

        #endregion

        #region LabelPath

        internal string labelPath;
        public string LabelPath
        {
            get { return labelPath; }
            set { labelPath = value; }
        }
              
        #endregion

        #region HeaderPath
        internal string headerPath;
        public string HeaderPath
        {
            get { return headerPath; }
            set { headerPath = value; }
        }

   
        #endregion

        #endregion
    }
}
