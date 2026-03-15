#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.TreeMap
{
    public class TreeMapItem : INotifyPropertyChanged
    {
        #region Private Variable

        internal TreeMap TreeMap;
        internal bool isRootTreeMapItem;
        internal TreeMapLevel GroupingLevel;
        private TreeMapLevel treeMapLevel;
        public TreeMapLevel TreeMapLevel
        {
            get
            {
                return treeMapLevel;
            }
            internal set
            {
                treeMapLevel = value;
            }
        }

        #endregion

        #region Properties

        #region Height

        private double height;
        public double Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
                OnPropertyChanged("Height");
            }
        }

        #endregion

        #region DefaultWidth

        private double width;
        public double DefaultWidth
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
                OnPropertyChanged("DefaultWidth");
            }
        }

        #endregion

        #region AreaByWeight
        private double areaByWeight;

        public double AreaByWeight
        {
            get
            {
                return areaByWeight;
            }
            set
            {
                areaByWeight = value;
                OnPropertyChanged("AreaByWeight");
            }
        }
        #endregion

        #region Weight
        private double weight;

        public double Weight
        {
            get
            {
                return weight;
            }
            set
            {
                weight = value;
                OnPropertyChanged("Weight");
            }
        }
        #endregion

        #region ItemWidth
        private double itemWidth;

        public double ItemWidth
        {
            get
            {
                return itemWidth;
            }
            set
            {
                itemWidth = value;
                OnPropertyChanged("ItemWidth");
            }
        }
        #endregion

        #region ItemHeight
        private double itemHeight;

        public double ItemHeight
        {
            get
            {
                return itemHeight;
            }
            set
            {
                itemHeight = value;
                OnPropertyChanged("ItemHeight");
            }
        }
        #endregion

        #region LeftPosition
        private double leftPosition;

        public double LeftPosition
        {
            get
            {
                return leftPosition;
            }
            set
            {
                leftPosition = value;
                OnPropertyChanged("LeftPosition");
            }
        }
        #endregion

        #region TopPosition
        private double topPosition;

        public double TopPosition
        {
            get
            {
                return topPosition;
            }
            set
            {
                topPosition = value;
                OnPropertyChanged("TopPosition");
            }
        }
        #endregion

        #region HeaderSize
        private double headerSize;

        public double HeaderSize
        {
            get
            {
                return headerSize;
            }
            set
            {
                headerSize = value;
                OnPropertyChanged("HeaderSize");
            }
        }
        #endregion

        #region Header
        private string header;

        public string Header
        {
            get
            {
                return header;
            }
            set
            {
                header = value;
                OnPropertyChanged("Header");
            }
        }
        #endregion

        #region ColorWeight

        private double colorWeight;

        public double ColorWeight
        {
            get { return colorWeight; }
            set { colorWeight = value; }
        }

        #endregion

        #region HeaderColor

        private Brush headerColor;

        public Brush HeaderColor
        {
            get { return headerColor; }
            set { headerColor = value; }
        }

        #endregion

        #region MappedColor

        private Brush mappedColor;

        public Brush MappedColor
        {
            get { return mappedColor; }
            set { mappedColor = value; }
        }

        #endregion

        #region LeafNodes

        private List<TreeMapLeafNode> leafNodes;
        public List<TreeMapLeafNode> LeafNodes
        {
            get { return leafNodes; }
            internal set { leafNodes = value; }
        }

        #endregion

        #region ChildTreeMapItems
        private List<TreeMapItem> childTreeMapItems;
        public List<TreeMapItem> ChildTreeMapItems
        {
            get { return childTreeMapItems; }
            internal set { childTreeMapItems = value; }
        }

        #endregion

        #region SubItemsList

        private List<object> subItemsList;
        internal List<object> SubItemsList
        {
            get { return subItemsList; }
            set { subItemsList = value; }
        }

        #endregion

        #region Data
        private object data;
        public object Data
        {
            get { return data; }
            internal set { data = value; }
        }
        #endregion

        #region Label

        private string label;
        public string Label
        {
            get { return label; }
            internal set {label = value; }
        }

        #endregion


        #endregion

        #region Constructor


        public TreeMapItem()
        {

        }

        #endregion

        #region Methods


     

        #endregion


    

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
