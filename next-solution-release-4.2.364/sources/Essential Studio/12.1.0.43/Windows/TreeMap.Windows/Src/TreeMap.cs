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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.TreeMap
{
    [ToolboxItem(true)]
    [System.Drawing.ToolboxBitmap(typeof(TreeMap), "ToolboxIcons.TreeMap.png")]

    public partial class TreeMap : Control, INotifyPropertyChanged
    {

        #region Private Variables

        Graphics graphics;
        Rectangle AvailableArea;
        TreeMapEngine engine;
        TreeMapItem RootTreeMapItem;
        double leftItemPosition;
        double topItemPosition;
        int treeMapHeight;
        int treeMapWidth;
        string toolTipString;
        TreeMapItem currentItem = new TreeMapItem();
        Point tooltipPostition = new Point();
        ToolTip mapToolTip = new ToolTip();
        int pointerX;
        int pointerY;
        List<TreeMapLeafNode> wholeItems = new List<TreeMapLeafNode>();
        int count = 0;
        private double ShorterSideLength
        {
            get
            {
                return Math.Min(AvailableArea.Width, AvailableArea.Height);
            }
        }
        bool muteDrawing;

        #endregion

        #region Properties

        #region ItemsSource
        private object itemsSource;

        public object ItemsSource
        {
            get
            {
                return itemsSource;
            }
            set
            {
                itemsSource = value;
                this.Refresh();
                OnPropertyChanged("ItemsSource");
            }
        }
        #endregion

        #region WeightValuePath
        private string weightValuePath;

        public string WeightValuePath
        {
            get
            {
                return weightValuePath;
            }
            set
            {
                weightValuePath = value;
                this.Refresh();
                OnPropertyChanged("WeightValuePath");
            }
        }
        #endregion



        #region ColorValuePath
        private string colorValuePath;
        public string ColorValuePath
        {
            get
            {
                return colorValuePath;
            }
            set
            {
                colorValuePath = value;
                this.Refresh();
                OnPropertyChanged("ColorValuePath");
            }
        }
        #endregion

        #region LeafLabelPath
        private string leafLabelPath;
        internal string LeafLabelPath
        {
            get
            {
                return leafLabelPath;
            }
            set
            {
                leafLabelPath = value;
                this.Refresh();
                OnPropertyChanged("LeafLabelPath");
            }
        }
        #endregion

        #region HeaderValuePath
        private string headerValuePath;

        internal string HeaderValuePath
        {
            get
            {
                return headerValuePath;
            }
            set
            {
                headerValuePath = value;
                this.Refresh();
                OnPropertyChanged("HeaderValuePath");
            }
        }
        #endregion

        #region Label
        private Font labelFont;

        public Font LabelFont
        {
            get
            {
                return labelFont;
            }
            set
            {
                labelFont = value;
                OnPropertyChanged("Label");
            }
        }

        #endregion

        #region LabelBrush
        private Brush labelBrush;

        public Brush LabelBrush
        {
            get
            {
                return labelBrush;
            }
            set
            {
                labelBrush = value;
                OnPropertyChanged("LabelBrush");
            }
        }
        #endregion

        #region LabelBackgroundBrush
        private Brush labelBackgroundBrush;

        public Brush LabelBackgroundBrush
        {
            get
            {
                return labelBackgroundBrush;
            }
            set
            {
                labelBackgroundBrush = value;
                OnPropertyChanged("LabelBackgroundBrush");
            }
        }
        #endregion

        #region LabelBorderBrush
        private Brush labelBorderBrush;

        public Brush LabelBorderBrush
        {
            get
            {
                return labelBorderBrush;
            }
            set
            {
                labelBorderBrush = value;
                OnPropertyChanged("LabelBorderBrush");
            }
        }
        #endregion

        #region LabelBorderThickness
        private float labelBorderThickness;

        public float LabelBorderThickness
        {
            get
            {
                return labelBorderThickness;
            }
            set
            {
                labelBorderThickness = value;
                OnPropertyChanged("LabelBorderThickness");
            }
        }
        #endregion


        #region Header
        private Font headerFont;

        public Font HeaderFont
        {
            get
            {
                return headerFont;
            }
            set
            {
                headerFont = value;
                OnPropertyChanged("Header");
            }
        }

        #endregion

        #region HeaderBrush
        private Brush headerBrush;

        public Brush HeaderBrush
        {
            get
            {
                return headerBrush;
            }
            set
            {
                headerBrush = value;
                OnPropertyChanged("HeaderBrush");
            }
        }
        #endregion

        #region HeaderBackgroundBrush
        private Brush headerBackgroundBrush;

        public Brush HeaderBackgroundBrush
        {
            get
            {
                return headerBackgroundBrush;
            }
            set
            {
                headerBackgroundBrush = value;
                OnPropertyChanged("HeaderBackgroundBrush");
            }
        }
        #endregion

        #region HeaderBorderBrush
        private Brush headerBorderBrush;

        public Brush HeaderBorderBrush
        {
            get
            {
                return headerBorderBrush;
            }
            set
            {
                headerBorderBrush = value;
                OnPropertyChanged("HeaderBorderBrush");
            }
        }
        #endregion

        #region HeaderBorderThickness
        private float headerBorderThickness;

        public float HeaderBorderThickness
        {
            get
            {
                return headerBorderThickness;
            }
            set
            {
                headerBorderThickness = value;
                OnPropertyChanged("HeaderBorderThickness");
            }
        }
        #endregion

        #region ItemsLayoutMode

        private ItemsLayoutModes itemsLayoutMode;
        public ItemsLayoutModes ItemsLayoutMode
        {
            get
            {
                return itemsLayoutMode;
            }
            set
            {
                itemsLayoutMode = value;
                this.Refresh();
                OnPropertyChanged("ItemsLayoutMode");
            }
        }
        #endregion

        #region ChildTreeMapItems

        internal List<TreeMapItem> ChildTreeMapItems
        {
            get;
            set;
        }
        #endregion

        #region LeafColorMapping

        private ColorMapping leafColorMapping;

        [Browsable(false)]
        public ColorMapping LeafColorMapping
        {
            get { return leafColorMapping; }
            set
            {
                leafColorMapping = value;
                this.Refresh();
                OnPropertyChanged("LeafColorMapping");
            }
        }
        #endregion

        #region Levels

        private List<TreeMapLevel> levels;

        [Browsable(false)]
        public List<TreeMapLevel> Levels
        {
            get { return levels; }
            set { levels = value; }
        }
        #endregion

        #region LeafNodes

        internal List<TreeMapLeafNode> LeafNodes
        {
            get;
            set;
        }

        #endregion

        #region LegendType
        private LegendTypes legendType;
        public LegendTypes LegendType
        {
            get { return legendType; }
            set { legendType = value; }
        }
        #endregion

        #region LegendGap
        private int legendGap;
        public int LegendGap
        {
            get { return legendGap; }
            set 
            {
                legendGap = value;
                this.Refresh();
                OnPropertyChanged("LegendGap");
            }
        }
        #endregion

        #region LegendPosition
        private LegendPositions legendPosition;
        public LegendPositions LegendPosition
        {
            get { return legendPosition; }
            set
            {
                legendPosition = value;
                this.Refresh();
                OnPropertyChanged("legendPosition");
            }
        }
        #endregion


        #region IsLegendVisible
        private bool isLegendVisible;
        public bool IsLegendVisible
        {
            get { return isLegendVisible; }
            set
            {
                isLegendVisible = value;
                this.Refresh();
                OnPropertyChanged("IsLegendVisible");
            }
        }
        #endregion


        #region IsTootTipVisible
        private bool isTootTipVisible;
        public bool IsTootTipVisible
        {
            get { return isTootTipVisible; }
            set
            {
                isTootTipVisible = value;
                this.Refresh();
                OnPropertyChanged("IsTootTipVisible");
            }
        }
        #endregion

        #region HeaderToolTipInfo

        private ToolTipInfo headerToolTipInfo;
        public ToolTipInfo HeaderToolTipInfo
        {
            get { return headerToolTipInfo; }
            set
            {
                headerToolTipInfo = value;
                this.Refresh();
                OnPropertyChanged("HeaderToolTipInfo");
            }
        }
        
        #endregion
        
        #region ItemToolTipInfo

        private ToolTipInfo itemToolTipInfo;
        public ToolTipInfo ItemToolTipInfo
        {
            get { return itemToolTipInfo; }
            set
            {
                itemToolTipInfo = value;
                this.Refresh();
                OnPropertyChanged("ItemToolTipInfo");
            }
        }
        #endregion

        #endregion

        #region Constructor

        public TreeMap()
        {
            Levels = new List<TreeMapLevel>();
            LeafColorMapping = new UniColorMapping();
            InitializeComponent();
            this.SizeChanged += TreeMap_SizeChanged;
            LabelFont = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            labelBrush = new SolidBrush(Color.White);
            labelBackgroundBrush = new SolidBrush(Color.Transparent);
            labelBorderBrush = new SolidBrush(Color.Transparent);
            labelBorderThickness = 2f;
            HeaderFont = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            HeaderBrush = new SolidBrush(Color.Gray);
            IsLegendVisible = true;
            HeaderBackgroundBrush = new SolidBrush(Color.White);
            HeaderBorderBrush = new SolidBrush(Color.Transparent);
            HeaderBorderThickness = 2f;
            this.Margin = new Padding(20);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            this.Dock = DockStyle.Fill;
            IsTootTipVisible = true;
        }

        void TreeMap_LeafItemDrawing(object sender, LeafItemDrawingEventArgs e)
        {
            muteDrawing = e.Cancel;
        }

        #endregion

        #region Override Methods

        protected override void OnMouseLeave(EventArgs e)
        {
            mapToolTip.Active = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            bool showToolTip = false;
            bool toolTipPositionChange = false;
            int xDiff, yDiff;
            if (e.X != pointerX && e.Y != pointerY && IsTootTipVisible && this.Levels.Count>0)
            {
                pointerX = e.X;
                pointerY = e.Y;
                xDiff = Math.Abs(e.X - tooltipPostition.X);
                yDiff = Math.Abs(e.Y - tooltipPostition.Y);
                if (RootTreeMapItem != null)
                {
                    List<TreeMapItem> sortedItems = new List<TreeMapItem>((RootTreeMapItem.ChildTreeMapItems).OrderByDescending(x => x.AreaByWeight));
                    for (int j = 0; j < sortedItems.Count; j++)
                    {
                        double gapWidth = this.Levels[0].GroupingGap;
                        if (sortedItems[j].ChildTreeMapItems != null && sortedItems[j].ChildTreeMapItems.Count > 0)
                        {
                            if ((pointerX >= sortedItems[j].LeftPosition && pointerX <= (sortedItems[j].LeftPosition + sortedItems[j].ItemWidth))
                                && ((pointerY >= sortedItems[j].TopPosition) && (pointerY <= (sortedItems[j].TopPosition + sortedItems[j].ChildTreeMapItems[0].HeaderSize))))
                            {
                                toolTipString = "";
                                if (currentItem != sortedItems[j] || (xDiff >50 || yDiff >50))
                                {
                                    currentItem = sortedItems[j];
                                    toolTipPositionChange = true;
                                    tooltipPostition = new Point(pointerX - 10, pointerY + 10);
                                }
                                if (HeaderToolTipInfo != null)
                                {
                                    #region HeaderToolTipPattern

                                    string headerPattern = HeaderToolTipInfo.ToolTipHeaderPattern;
                                    bool isBoldHeader = headerPattern.Contains("<b>");
                                    headerPattern = RemoveBoldTag(headerPattern);
                                    string headerTipString = GetHeaderToolTipString(headerPattern, sortedItems[j]);
                                    if (headerTipString == "" && headerTipString == string.Empty)
                                        mapToolTip.ToolTipTitle = null;
                                    if (isBoldHeader)
                                        mapToolTip.ToolTipTitle = headerTipString;
                                    else
                                    {
                                        mapToolTip.ToolTipTitle = null;
                                        toolTipString = headerTipString;
                                    }
                                    #endregion
                                    #region ItemToolTipPattern
                                    string contentPattern = HeaderToolTipInfo.ToolTipContentPattern;
                                    contentPattern = RemoveBoldTag(contentPattern);
                                    string contentTipString = GetHeaderToolTipString(contentPattern, sortedItems[j]);
                                    toolTipString = toolTipString + "\n" + contentTipString;
                                    showToolTip = true;
                                    #endregion

                                }
                                else
                                {
                                    toolTipString = "";
                                    mapToolTip.ToolTipTitle = sortedItems[j].Label;
                                    GetDefaultToolTipString(sortedItems[j]);
                                    showToolTip = true;
                                }
                            }
                            foreach (TreeMapItem item in sortedItems[j].ChildTreeMapItems)
                            {
                                
                                if ((pointerX >= item.LeftPosition && pointerX <= (item.LeftPosition + item.ItemWidth)) && ((pointerY >= item.TopPosition) && (pointerY <= (item.TopPosition + item.ItemHeight))))
                                {
                                    if (currentItem != item || (xDiff > 50 || yDiff > 50))
                                    {
                                        currentItem = item;
                                        toolTipPositionChange = true;
                                        tooltipPostition = new Point(pointerX - 10, pointerY + 10);
                                    }
                                    if (ItemToolTipInfo != null)
                                    {
                                        toolTipString = "";
                                        #region HeaderToolTipPattern

                                        string headerPattern = ItemToolTipInfo.ToolTipHeaderPattern;
                                        bool isBoldHeader = headerPattern.Contains("<b>");
                                        headerPattern = RemoveBoldTag(headerPattern);
                                        string headerTipString = GetItemToolTipString(headerPattern, item.Data);
                                        if (headerTipString == "" && headerTipString == string.Empty)
                                            mapToolTip.ToolTipTitle = null;
                                        if (isBoldHeader)
                                            mapToolTip.ToolTipTitle = headerTipString;
                                        else
                                        {
                                            mapToolTip.ToolTipTitle = null;
                                            toolTipString = headerTipString;
                                        }

                                        #endregion

                                        #region ItemToolTipPattern
                                        string contentPattern = ItemToolTipInfo.ToolTipContentPattern;
                                        contentPattern = RemoveBoldTag(contentPattern);
                                        string contentTipString = GetItemToolTipString(contentPattern, item.Data);
                                        toolTipString = toolTipString + "\n" + contentTipString;
                                        #endregion
                                        
                                        showToolTip = true;
                                    }
                                    else
                                    {
                                        toolTipString = "";
                                        mapToolTip.ToolTipTitle = item.Label;
                                        GetDefaultToolTipString(item);
                                        showToolTip = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var item = sortedItems[j] as TreeMapItem;
                            if ((pointerX >= item.LeftPosition && pointerX <= (item.LeftPosition + item.ItemWidth)) && ((pointerY >= item.TopPosition) && (pointerY <= (item.TopPosition + item.ItemHeight))))
                            {
                                if (currentItem != item || (xDiff >50 || yDiff >50))
                                {
                                    currentItem = item;
                                    toolTipPositionChange = true;
                                    tooltipPostition = new Point(pointerX - 10, pointerY + 10);
                                }
                                
                                if (ItemToolTipInfo != null)
                                {
                                    #region HeaderToolTipPattern
                                    toolTipString = "";
                                    string headerPattern = ItemToolTipInfo.ToolTipHeaderPattern;
                                    bool isBoldHeader = headerPattern.Contains("<b>");
                                    headerPattern = RemoveBoldTag(headerPattern);
                                    string headerTipString = GetItemToolTipString(headerPattern, item.Data);
                                    if (headerTipString == "" && headerTipString == string.Empty)
                                        mapToolTip.ToolTipTitle = null;
                                    if (isBoldHeader)
                                        mapToolTip.ToolTipTitle = headerTipString;
                                    else
                                    {
                                        mapToolTip.ToolTipTitle = null;
                                        toolTipString = headerTipString;
                                    }
                                    #endregion
                                    #region ItemToolTipPattern
                                    string contentPattern = ItemToolTipInfo.ToolTipContentPattern;
                                    contentPattern = RemoveBoldTag(contentPattern);
                                    string contentTipString = GetItemToolTipString(contentPattern, item.Data);
                                    toolTipString = toolTipString + "\n" + contentTipString;
                                    showToolTip = true;
                                    #endregion

                                }
                                else 
                                {
                                    toolTipString = "";
                                    mapToolTip.ToolTipTitle = item.Label;
                                    GetDefaultToolTipString(item);
                                    showToolTip = true;
                                }
                            }
                        }
                    }
                }
                if (toolTipString != null && toolTipString != string.Empty && showToolTip)
                {
                    mapToolTip.Active = true;
                    mapToolTip.BackColor = Color.Red;
                    mapToolTip.IsBalloon = false;
                    mapToolTip.ForeColor = Color.Black;
                    mapToolTip.ReshowDelay = 1500;
                    //mapToolTip.SetToolTip(this, toolTipString);
                    if (toolTipPositionChange)
                    {
                        mapToolTip.Show(toolTipString, this, tooltipPostition);
                        toolTipPositionChange = false;
                    }
                    toolTipString = string.Empty;
                }
                else if(!showToolTip)
                {
                    mapToolTip.Active = false;
                }
            }
        }
        void TreeMap_SizeChanged(object sender, EventArgs e)
        {
            treeMapHeight = this.Height;
            treeMapWidth = this.Width;
        }


        
        int tempHeight;
        int tempWidth;
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
			if(LeafItemDrawing != null)
            	LeafItemDrawing += TreeMap_LeafItemDrawing;

            graphics = e.Graphics;

            Rectangle r = new Rectangle(0, 0, treeMapWidth, treeMapHeight);
            Brush brush1 = new SolidBrush(Color.Transparent);
            e.Graphics.FillRectangle(new SolidBrush(Color.White), r);
            var itemSource = (IEnumerable)ItemsSource;
            tempHeight = treeMapHeight;
            tempWidth = treeMapWidth;
            if (itemSource != null && this.Levels.Count>0)
            {

                GenerateTreeMapItems(itemSource);
                if(this.WeightValuePath!=null)
                {
                    if (this.LeafColorMapping is RangeBrushColorMapping && this.IsLegendVisible)
                    {
                        DrawLegends(e.Graphics);
                    }
                    else
                    {
                        topItemPosition = this.Margin.Top;
                        leftItemPosition = this.Margin.Left;
                        treeMapHeight = this.Height - (this.Margin.Bottom + this.Margin.Top);
                        treeMapWidth = this.Width - (this.Margin.Left + this.Margin.Right);
                    }

                    if (this.ItemsLayoutMode == ItemsLayoutModes.Squarified)
                    {
                        if (RootTreeMapItem.ChildTreeMapItems != null && RootTreeMapItem.ChildTreeMapItems.Count > 0)
                        {
                            List<TreeMapItem> items = RootTreeMapItem.ChildTreeMapItems;
                            if (items.Count > 0)
                            {
                                DrawTreeMapItemInSquarifiedMode(items, treeMapWidth, treeMapHeight, e.Graphics, brush1);
                            }
                        }

                    }
                    else
                    {
                        if (RootTreeMapItem.ChildTreeMapItems != null && RootTreeMapItem.ChildTreeMapItems.Count > 0)
                        {
                            List<TreeMapItem> items = RootTreeMapItem.ChildTreeMapItems;
                            if (items.Count > 0)
                            {
                                DrawTreeMapItemInSliceAndDiceMode(items, treeMapWidth, treeMapHeight, e.Graphics, brush1);
                            }
                        }
                    }

                }



            }
        }


        #endregion


        #region Methods

        #region BoldTag
        private string RemoveBoldTag(string toolTipString)
        {
            toolTipString = toolTipString.Replace("<b>", "");
            toolTipString = toolTipString.Replace("</b>", "");
            return toolTipString;
        }
        #endregion

        #region DefaultToolTipString

        private void GetDefaultToolTipString(TreeMapItem item)
        {
            int lenghtDiff = Math.Abs(ColorValuePath.Length - weightValuePath.Length);
            if (lenghtDiff > 2)
            {
                if (ColorValuePath.Length < weightValuePath.Length)
                {
                    toolTipString = "\n" + ColorValuePath + "\t\t" + item.ColorWeight + "\n" + weightValuePath + "\t" + item.Weight;
                }
                else
                {
                    toolTipString = "\n" + ColorValuePath + "\t" + item.ColorWeight + "\n" + weightValuePath + "\t\t" + item.Weight;
                }
            }
            else
            {
                toolTipString = "\n" + ColorValuePath + "\t" + item.ColorWeight + "\n" + weightValuePath + "\t" + item.Weight;
            }
        }

        #endregion

        #region GetHeaderToolTipString
        private string GetHeaderToolTipString(string headerPattern, TreeMapItem data)
        {
            string toolTipStr = headerPattern;
            List<string> propertyNames = GetPropertyNames(headerPattern);
            int i = 0;
            string check ="";
            foreach (string word in propertyNames)
            {

                if (ColorValuePath.ToString() == word)
                {
                    check = data.ColorWeight.ToString();
                }
                else if (WeightValuePath.ToString() == word)
                {
                    check = data.Weight.ToString();
                }
                else if(data.GetType() != null && data.GetType().GetProperty(propertyNames[i]) != null)
                {
#if SyncfusionFramework3_5
                    check = data.GetType().GetProperty(propertyNames[i]).GetValue(data, null).ToString();
#else
                    check = data.GetType().GetProperty(propertyNames[i]).GetValue(data).ToString()
#endif
                }
                toolTipStr = toolTipStr.Replace("{" + propertyNames[i] + "}", check);
                i++;
            }
            return toolTipStr;
        }
        #endregion

        #region GetItemToolTipString

        private string GetItemToolTipString(string headerPattern,object data)
        {
            string toolTipStr = headerPattern;
            List<string> propertyNames = GetPropertyNames(headerPattern);
            
            int i = 0;
            foreach (string word in propertyNames)
            {
                if ((data as List<Object>)[0].GetType() != null &&  (data as List<Object>)[0].GetType().GetProperty(propertyNames[i]) != null)
                {
#if SyncfusionFramework3_5
                    string check = (data as List<Object>)[0].GetType().GetProperty(propertyNames[i]).GetValue((data as List<Object>)[0], null).ToString();
#else
                    string check = (data as List<Object>)[0].GetType().GetProperty(propertyNames[i]).GetValue((data as List<Object>)[0]).ToString();
#endif
                    toolTipStr = toolTipStr.Replace("{" + propertyNames[i] + "}", check);
                }
                i++;
            }
            return toolTipStr;
        }

        #endregion

        #region GetPropertyNames

        private List<string> GetPropertyNames(string pattern)
        {
            List<string> propertyNames = new List<string>();
            while (pattern.Contains("{"))
            {
                propertyNames.Add(pattern.Between("{", "}"));
                pattern = pattern.Remove(0, pattern.IndexOf("}") + 1);
            }
            return propertyNames;
        }

        #endregion

        private void GenerateTreeMapItems(IEnumerable itemS)
        {
            RootTreeMapItem = new TreeMapItem();
                RootTreeMapItem.ChildTreeMapItems = new List<TreeMapItem>();

            foreach (var item in itemS)
            {
                if (item != null)
                {
                    if (WeightValuePath != null)
                    {
                        PropertyInfo _weightValuePath = item.GetType().GetTypeInfo().GetDeclaredProperty(WeightValuePath);
                        if (_weightValuePath != null)
                        {
                            RootTreeMapItem.Weight = (double)_weightValuePath.GetValue(item, null);
                        }
                    }
                    if (HeaderValuePath != null)
                    {
                        PropertyInfo _headerValuePath = item.GetType().GetTypeInfo().GetDeclaredProperty(HeaderValuePath);

                        if (_headerValuePath != null)
                        {
                            if (_headerValuePath is string)
                            {
                                RootTreeMapItem.Header = (string)_headerValuePath.GetValue(item, null);
                            }
                        }
                    }
                    RootTreeMapItem.isRootTreeMapItem = true;
                    RootTreeMapItem.TreeMap = this;
                    RootTreeMapItem.ChildTreeMapItems.Add(RootTreeMapItem);
                }
            }



            //RootTreeMapItem = new TreeMapItem { isRootTreeMapItem = true, TreeMap = this, GroupingLevel = Levels[0] };
            wholeItems = new List<TreeMapLeafNode>();
            var valueField = new TreeMapValueField
            {
                Name = WeightValuePath,
                FieldName = WeightValuePath
            };

            var colorField = new TreeMapValueField
            {
                Name = ColorValuePath,
                FieldName = ColorValuePath
            };

            engine = new TreeMapEngine
            {
                ValueField = valueField,
                ColorField = colorField,
                DataSource = ItemsSource
            };

            List<TreeMapItem> rootItems = null, subItems = null;
            int levelsCount = Levels.Count;

            #region Nested Data Collection TreeMapItems
            if (Levels[0].GetType() == typeof(TreeMapHierarchicalLevel))
            {
                for (int i = 0; i < levelsCount; i++)
                {
                    if (i == 0)
                    {
                        RootTreeMapItem.GroupingLevel = Levels[i];
                        int itemsCount;
                        if (Levels[i].GroupingPath == null && i == levelsCount - 1)
                        {
                            wholeItems = engine.GetTreeMapLeafNodes(WeightValuePath, ColorValuePath, LeafLabelPath);
                            RootTreeMapItem.LeafNodes = wholeItems;
                            if (rootItems != null)
                            {
                                itemsCount = rootItems.Count;
                                for (int j = itemsCount - 1; j >= 0; j--)
                                {
                                    rootItems[j].TreeMap = this;
                                    rootItems[j].HeaderSize = Levels[i].HeaderHeight;

                                }
                            }
                        }
                        else
                        {
                            rootItems = engine.GetTreeMapItems(WeightValuePath, ColorValuePath, Levels[i].LevelLabelPath, Levels[i].LevelHeaderPath);
                            RootTreeMapItem.ChildTreeMapItems = rootItems;
                            subItems = rootItems.ToList();
                            itemsCount = rootItems.Count;
                            for (int j = itemsCount - 1; j >= 0; j--)
                            {
                                rootItems[j].TreeMap = this;
                            }
                        }
                    }
                    if (subItems != null)
                    {
                        var cloneItems = subItems.ToList();
                        subItems.Clear();
                        var levelEngine = new TreeMapEngine
                        {
                            ValueField = valueField,
                            ColorField = colorField,
                        };
                        if (i > 0 && Levels[i - 1].DataSource.Count > 0)
                        {
                            engine.DataSource = Levels[i - 1].DataSource;
                        }

                        int cloneItemsCount = cloneItems.Count;
                        for (int j = 0; j < cloneItemsCount; j++)
                        {
                            var levelItemsSource = engine.GetValue(Levels[i].GroupingPath, cloneItems[j].Data);
                            levelEngine.DataSource = levelItemsSource;
                            IEnumerator iterator = levelEngine.DataSourceList.GetEnumerator();
                            int levelItemsSourceCount = 0;
                            while (iterator.MoveNext())
                            {
                                Levels[i].DataSource.Add(iterator.Current);
                                levelItemsSourceCount++;
                            }

                            if (i != levelsCount - 1)
                            {
                                var treeItems = levelEngine.GetTreeMapItems(WeightValuePath, ColorValuePath, Levels[i + 1].LevelLabelPath, Levels[i + 1].LevelHeaderPath);
                                int treeItemsCount = treeItems.Count;
                                cloneItems[j].ChildTreeMapItems = treeItems;
                                cloneItems[j].GroupingLevel = Levels[i + 1];
                                cloneItems[j].HeaderSize = Levels[i].HeaderHeight;
                                for (int k = treeItemsCount - 1; k >= 0; k--)
                                {
                                    treeItems[k].TreeMap = this;
                                }
                                subItems.AddRange(treeItems);
                            }
                            else
                            {
                                List<TreeMapLeafNode> leafNodes;
                                if (levelItemsSourceCount == 0)
                                {
                                    levelEngine.DataSource = new List<object> { cloneItems[j].Data };
                                    leafNodes = levelEngine.GetTreeMapLeafNodes(WeightValuePath, ColorValuePath, LeafLabelPath);
                                }
                                else
                                    leafNodes = levelEngine.GetTreeMapLeafNodes(WeightValuePath, ColorValuePath, LeafLabelPath);
                                cloneItems[j].LeafNodes = leafNodes;
                                cloneItems[j].HeaderSize = Levels[i].HeaderHeight;
                                wholeItems.AddRange(leafNodes);
                            }
                        }
                        Levels[i].TreeMapItems = cloneItems;
                    }
                }
                LeafColorMapping.EvaluateColorMapping(wholeItems);
            }
            #endregion

            #region Normal Data Collection TreeMapItems
            else
            {
                #region Grouping tree map items
                for (int i = 0; i < levelsCount; i++)
                {
                    if (Levels[i].GroupingPath != null)
                    {
                        if (subItems != null)
                        {
                            var cloneItems = subItems.ToList();
                            subItems.Clear();

                            int cloneItemsCount = cloneItems.Count;
                            for (int j = 0; j < cloneItemsCount; j++)
                            {
                                engine = new TreeMapEngine
                                {
                                    ValueField = valueField,
                                    ColorField = colorField,
                                    DataSource = cloneItems[j].SubItemsList
                                };
                                var treeItems = engine.GetGroupItem(Levels[i].GroupingPath);
                                cloneItems[j].ChildTreeMapItems = treeItems;
                                cloneItems[j].GroupingLevel = Levels[i];
                                LeafColorMapping.EvaluateColorMapping(treeItems);
                                int treeItemsCount = treeItems.Count;
                                for (int k = treeItemsCount - 1; k >= 0; k--)
                                {
                                    treeItems[k].TreeMap = this;
                                    treeItems[k].HeaderSize = Levels[i].HeaderHeight;
                                }
                                subItems.AddRange(treeItems);
                            }
                            Levels[i].TreeMapItems = subItems.ToList();
                        }
                        else
                        {
                            rootItems = engine.GetGroupItem(Levels[i].GroupingPath);
                            RootTreeMapItem.GroupingLevel = Levels[i];
                            int rootItemsCount = rootItems.Count;
                            for (int j = rootItemsCount - 1; j >= 0; j--)
                            {
                                rootItems[j].TreeMap = this;
                                rootItems[j].HeaderSize = Levels[i].HeaderHeight;
                            }
                            subItems = rootItems.ToList();
                            Levels[i].TreeMapItems = rootItems;
                        }
                    }
                }
                #endregion

                if (subItems != null)
                {
                    RootTreeMapItem.ChildTreeMapItems = rootItems;
                    int subItemsCount = subItems.Count;
                    for (int i = 0; i < subItemsCount; i++)
                    {
                        engine = new TreeMapEngine
                        {
                            ValueField = valueField,
                            ColorField = colorField,
                            DataSource = subItems[i].SubItemsList,
                        };
                        var leafNodes = engine.GetTreeMapLeafNodes(WeightValuePath, ColorValuePath, LeafLabelPath);
                        subItems[i].LeafNodes = leafNodes;
                        subItems[i].GroupingLevel = Levels[levelsCount - 1];
                        wholeItems.AddRange(leafNodes);
                    }
                    LeafColorMapping.EvaluateColorMapping(wholeItems);
                }
                else
                {
                    wholeItems = engine.GetTreeMapLeafNodes(WeightValuePath, ColorValuePath, LeafLabelPath);
                    LeafColorMapping.EvaluateColorMapping(wholeItems);
                    RootTreeMapItem.LeafNodes = wholeItems;
                }
            }
            #endregion
            if (wholeItems != null)
                LeafColorMapping.EvaluateColorMapping(wholeItems);
            if (rootItems != null)
                LeafColorMapping.EvaluateColorMapping(rootItems);
            if (subItems != null)
                this.ChildTreeMapItems = subItems;
            if (wholeItems != null)
                LeafNodes = wholeItems;


        }




        void DrawTreeMapItemInSliceAndDiceMode(List<TreeMapItem> TreeMapItemList, double parentWidth, double parentHeight, Graphics graphics, Brush brush)
        {

            List<TreeMapItem> sortedItems = new List<TreeMapItem>((TreeMapItemList).OrderByDescending(x => x.AreaByWeight));
            //leftItemPosition = leftItemPosition == 0 ? leftItemPosition : this.Margin.Left;
            //topItemPosition = topItemPosition == 0 ? topItemPosition : this.Margin.Top;
            if (sortedItems.Count > 0)
            {
                int level = 0;
                double gapWidth = this.Levels[level].GroupingGap;
                if (parentWidth > 0 && parentHeight > 0)
                    CalculateSliceAndDiceItemsSize(sortedItems, leftItemPosition, topItemPosition, new Size(((int)parentWidth), (int)parentHeight), gapWidth, RootTreeMapItem.HeaderSize, graphics, brush);
                
                sortedItems = new List<TreeMapItem>(TreeMapItemList.OrderByDescending(x => x.AreaByWeight));
                double temptopItemPosition = topItemPosition;
                double templeftItemPosition = leftItemPosition;
                for (int j = 0; j < sortedItems.Count; j++)
                {
                    gapWidth = this.Levels[level].ItemsGap; ;

                    if (sortedItems[j].ChildTreeMapItems != null && sortedItems[j].ChildTreeMapItems.Count > 0)
                    {
                        double tempWidth = sortedItems[j].ItemWidth;
                        double tempHeight = sortedItems[j].ItemHeight;
                        temptopItemPosition = (sortedItems[j].TopPosition + gapWidth);
                        templeftItemPosition = (sortedItems[j].LeftPosition + gapWidth);
                        if (sortedItems[j].ChildTreeMapItems != null && sortedItems[j].ChildTreeMapItems.Count > 0)
                        {
                            List<TreeMapItem> items = new List<TreeMapItem>((sortedItems[j].ChildTreeMapItems).OrderByDescending(x => x.AreaByWeight));
                            CalculateSliceAndDiceItemsSize(items, templeftItemPosition, temptopItemPosition, new Size((int)tempWidth, (int)tempHeight), gapWidth, RootTreeMapItem.HeaderSize, graphics, brush);
                            DrawHierarchicalSliceAndDiceItems(items, templeftItemPosition, temptopItemPosition, new Size((int)tempWidth, (int)tempHeight), gapWidth, RootTreeMapItem.HeaderSize, graphics, brush);
                        }
                    }
                }
                level++;
            }

        }

        void DrawTreeMapItemInSquarifiedMode(List<TreeMapItem> TreeMapItemList, double parentWidth, double parentHeight, Graphics graphics, Brush brush)
        {
            List<TreeMapItem> sortedItems = new List<TreeMapItem>((TreeMapItemList).OrderByDescending(x => x.AreaByWeight));
            if (sortedItems.Count > 0)
            {
                int level = 0;
                //leftItemPosition = leftItemPosition > 0 ? leftItemPosition : this.Margin.Left;
                //topItemPosition = topItemPosition > 0 ? topItemPosition : this.Margin.Top;
                double gapWidth = this.Levels[level].GroupingGap;
                if (parentWidth > 0 && parentHeight > 0)
                    CalculateSquarfiedItemsSize(sortedItems, leftItemPosition, topItemPosition, new Size((int)parentWidth, (int)parentHeight), gapWidth, RootTreeMapItem.HeaderSize, graphics, brush);

                sortedItems = new List<TreeMapItem>(TreeMapItemList.OrderByDescending(x => x.AreaByWeight));
                double temptopItemPosition = topItemPosition;
                double templeftItemPosition = leftItemPosition;
                for (int j = 0; j < sortedItems.Count; j++)
                {
                    gapWidth = this.Levels[level].ItemsGap; ;
                    if (sortedItems[j].ChildTreeMapItems != null && sortedItems[j].ChildTreeMapItems.Count > 0)
                    {
                        double tempWidth = sortedItems[j].ItemWidth;
                        double tempHeight = sortedItems[j].ItemHeight;
                        temptopItemPosition = (sortedItems[j].TopPosition + gapWidth);
                        templeftItemPosition = (sortedItems[j].LeftPosition + gapWidth);
                        if (sortedItems[j].ChildTreeMapItems != null && sortedItems[j].ChildTreeMapItems.Count > 0)
                        {
                            List<TreeMapItem> items = new List<TreeMapItem>((sortedItems[j].ChildTreeMapItems).OrderByDescending(x => x.AreaByWeight));
                            CalculateSquarfiedItemsSize(items, templeftItemPosition, temptopItemPosition, new Size((int)tempWidth, (int)tempHeight), gapWidth, RootTreeMapItem.HeaderSize, graphics, brush);
                            DrawHierarchicalSquarfiedItems(items, templeftItemPosition, temptopItemPosition, new Size((int)tempWidth, (int)tempHeight), gapWidth, RootTreeMapItem.HeaderSize, graphics, brush);
                        }
                    }
                }
            }

        }


        void CalculateSliceAndDiceItemsSize(List<TreeMapItem> TreeMapItemList, double prevItemWidth, double prevItemHeight, Size AvailableSize, double Gap, double HeaderHeight, Graphics graphics, Brush brush)
        {           
            //  TreeMapItemList[0].HeaderSize = HeaderHeight < AvailableSize.Height ? HeaderHeight : 0;
            AvailableSize = new Size((int)AvailableSize.Width, (int)(AvailableSize.Height - (int)TreeMapItemList[0].HeaderSize));
            AvailableArea = new Rectangle((int)prevItemWidth, ((int)prevItemHeight + (int)TreeMapItemList[0].HeaderSize), AvailableSize.Width, AvailableSize.Height);
            graphics.FillRectangle(new SolidBrush(Color.White), AvailableArea);
            graphics.DrawRectangle(new Pen(new SolidBrush(Color.White), (int)Gap), AvailableArea);
            double totalWeight = TreeMapItemList.Sum(x => x.Weight);
            double parentArea = AvailableSize.Height * AvailableSize.Width;
            int itemsCount = TreeMapItemList.Count;
            double gap;

            Orientation orientation = GetOrientation();

            if (orientation == Orientation.Horizontal)
            {
                double parentHeight = AvailableSize.Height;
                double allottedWidth = prevItemWidth;
                for (int i = 0; i < itemsCount; i++)
                {
                    double childarea = (parentArea / totalWeight) * TreeMapItemList[i].Weight;
                    double childWidth = childarea / parentHeight;
                    gap = (childWidth > Gap) ? Gap : 0;
                    TreeMapItemList[i].ItemWidth = (i != itemsCount - 1) ? childWidth - gap : childWidth;
                    TreeMapItemList[i].ItemHeight = parentHeight;
                    TreeMapItemList[i].LeftPosition = allottedWidth;
                    TreeMapItemList[i].TopPosition = AvailableArea.Y;
                    Rectangle tempRectangle = new Rectangle((int)allottedWidth, AvailableArea.Y, (int)childWidth, (int)parentHeight);



                    if (TreeMapItemList[i].ChildTreeMapItems != null && TreeMapItemList[i].ChildTreeMapItems.Count > 0)
                    {
                        Rectangle headerRect = new Rectangle(tempRectangle.X, tempRectangle.Y, tempRectangle.Width, (int)TreeMapItemList[0].HeaderSize);

                        graphics.FillRectangle(HeaderBackgroundBrush, headerRect);
                        graphics.DrawRectangle(new Pen(HeaderBorderBrush, HeaderBorderThickness), headerRect);
                        graphics.DrawString(TreeMapItemList[i].Label, LabelFont, HeaderBrush, headerRect);
                    }
                    else
                    {
                        graphics.FillRectangle(new SolidBrush(Color.White), tempRectangle.X, tempRectangle.Y, tempRectangle.Width, tempRectangle.Height);
                        if (TreeMapItemList[i].HeaderColor != null)
                        {
                            graphics.FillRectangle(TreeMapItemList[i].HeaderColor, tempRectangle.X, tempRectangle.Y, tempRectangle.Width, tempRectangle.Height);
                            graphics.DrawRectangle(new Pen(new SolidBrush(Color.White), 5), tempRectangle.X, tempRectangle.Y, tempRectangle.Width, tempRectangle.Height);
                        }
                        int tempHeaderHeight = tempRectangle.Height < (int)LabelFont.Height ? tempRectangle.Height : (int)LabelFont.Height;
                        Rectangle headerRect = new Rectangle(tempRectangle.X, tempRectangle.Y, tempRectangle.Width, tempHeaderHeight);
                        graphics.FillRectangle(labelBackgroundBrush, tempRectangle);
                        graphics.DrawRectangle(new Pen(labelBorderBrush, labelBorderThickness), headerRect);
                        graphics.DrawString(TreeMapItemList[i].Label, LabelFont, labelBrush, tempRectangle);
                    }


                    allottedWidth += childWidth;
                }
            }
            else
            {
                double parentWidth = AvailableArea.Width;
                double allottedHeight = AvailableArea.Y;

                for (int i = 0; i < itemsCount; i++)
                {
                    double childarea = (parentArea / totalWeight) * TreeMapItemList[i].Weight;
                    double childHeight = childarea / parentWidth;
                    gap = (childHeight > Gap) ? Gap : 0;
                    TreeMapItemList[i].ItemWidth = parentWidth;
                    TreeMapItemList[i].ItemHeight = (i != itemsCount - 1) ? childHeight - gap : childHeight;
                    TreeMapItemList[i].TopPosition = allottedHeight;
                    TreeMapItemList[i].LeftPosition = AvailableArea.X;
                    Rectangle tempRectangle = new Rectangle(AvailableArea.X, (int)allottedHeight, (int)parentWidth, (int)childHeight);
                    LeafItemDrawingEventArgs leafEventArgs = new LeafItemDrawingEventArgs();
                    leafEventArgs.Graphics = graphics;
                    leafEventArgs.RectSize = tempRectangle;
                    leafEventArgs.Color = TreeMapItemList[i].HeaderColor;
                    leafEventArgs.Label = TreeMapItemList[i].Label;
                    if (TreeMapItemList[i].Data is IEnumerable<Object>)
                    {
                        var enumerator = (TreeMapItemList[i].Data as IEnumerable<Object>).GetEnumerator();
                        enumerator.MoveNext();
                        leafEventArgs.Data = enumerator.Current;
                    }
                    else
                        leafEventArgs.Data = TreeMapItemList[i].Data;

                    if (TreeMapItemList[i].ChildTreeMapItems != null && TreeMapItemList[i].ChildTreeMapItems.Count > 0)
                    {
                        Rectangle headerRect = new Rectangle(tempRectangle.X, tempRectangle.Y, tempRectangle.Width, (int)TreeMapItemList[0].HeaderSize);
                        graphics.FillRectangle(HeaderBackgroundBrush, headerRect);
                        graphics.DrawRectangle(new Pen(HeaderBorderBrush, HeaderBorderThickness), headerRect);
                        graphics.DrawString(TreeMapItemList[i].Label, LabelFont, HeaderBrush, headerRect);
                    }
                    else
                    {
                        RaiseLeafItemDrawingEvent(leafEventArgs);
                        if (!muteDrawing)
                        {
                            graphics.FillRectangle(new SolidBrush(Color.White), tempRectangle.X, tempRectangle.Y, tempRectangle.Width, tempRectangle.Height);
                            if (TreeMapItemList[i].HeaderColor != null)
                            {
                                graphics.FillRectangle(TreeMapItemList[i].HeaderColor, tempRectangle.X, tempRectangle.Y, tempRectangle.Width, tempRectangle.Height);
                                graphics.DrawRectangle(new Pen(new SolidBrush(Color.White), 5), tempRectangle.X, tempRectangle.Y, tempRectangle.Width, tempRectangle.Height);
                            }
                            int tempHeaderHeight = tempRectangle.Height < (int)LabelFont.Height ? tempRectangle.Height : (int)LabelFont.Height;
                            Rectangle headerRect = new Rectangle(tempRectangle.X, tempRectangle.Y, tempRectangle.Width, tempHeaderHeight);
                            graphics.FillRectangle(labelBackgroundBrush, headerRect);
                            graphics.DrawRectangle(new Pen(labelBorderBrush, labelBorderThickness), headerRect);
                            graphics.DrawString(TreeMapItemList[i].Label, LabelFont, labelBrush, tempRectangle);
                        }
                    }
                    allottedHeight += childHeight;
                }
            }
        }

        void CalculateSquarfiedItemsSize(List<TreeMapItem> TreeMapItemList, double prevItemWidth, double prevItemHeight, Size AvailableSize, double Gap, double HeaderHeight, Graphics graphics, Brush brush)
        {
            // TreeMapItemList[0].HeaderSize = HeaderHeight < AvailableSize.Height ? HeaderHeight : 0;
            double totalweight = TreeMapItemList.Sum(x => x.Weight);
            AvailableSize = new Size((int)(AvailableSize.Width), (int)(AvailableSize.Height - (int)TreeMapItemList[0].HeaderSize));
            AvailableArea = new Rectangle((int)prevItemWidth, ((int)prevItemHeight + (int)TreeMapItemList[0].HeaderSize), AvailableSize.Width, AvailableSize.Height);
            graphics.FillRectangle(new SolidBrush(Color.White), AvailableArea);
            graphics.DrawRectangle(new Pen(new SolidBrush(Color.White), (float)Gap), AvailableArea);
            int itemsCount = TreeMapItemList.Count;

            for (int i = itemsCount - 1; i >= 0; i--)
            {
                TreeMapItemList[i].AreaByWeight = (AvailableSize.Height * AvailableSize.Width) * TreeMapItemList[i].Weight / totalweight;
            }

            var OrderedTreeMapItemList = new List<TreeMapItem>(TreeMapItemList.OrderByDescending(x => x.AreaByWeight));

            double GroupMaxAspectRatio = 0d, curX = 0, curY = 0;
            int j;

            for (int i = 0; i < itemsCount; i = j)
            {
                var firstTreemapItem = OrderedTreeMapItemList[i];
                double GroupTotalWeight = 0d;
                j = i;

                for (; j < itemsCount; j++)
                {
                    var lastTreemapItem = OrderedTreeMapItemList[j];
                    GroupTotalWeight += lastTreemapItem.AreaByWeight;
                    double GroupWidth = GroupTotalWeight / ShorterSideLength;
                    double firstitemheight = firstTreemapItem.AreaByWeight / GroupWidth;
                    double lastitemheight = lastTreemapItem.AreaByWeight / GroupWidth;
                    if (j == 0)
                        GroupMaxAspectRatio = AspectRatio(GroupWidth, ShorterSideLength);
                    double TempAspectRatio = Math.Max(AspectRatio(firstitemheight, GroupWidth), AspectRatio(lastitemheight, GroupWidth));
                    if (GroupTotalWeight.Equals(lastTreemapItem.AreaByWeight) || TempAspectRatio < GroupMaxAspectRatio)
                    {
                        GroupMaxAspectRatio = TempAspectRatio;
                    }
                    else
                    {
                        GroupTotalWeight -= lastTreemapItem.AreaByWeight;
                        GroupWidth = GroupTotalWeight / ShorterSideLength;
                        GroupMaxAspectRatio = Math.Max(AspectRatio(firstitemheight, GroupWidth), AspectRatio(lastitemheight, GroupWidth));
                        break;
                    }
                }

                Orientation orientation = GetOrientation();
                var currentRect = new Rectangle();

                for (int k = i; k < j; k++)
                {
                    var item = OrderedTreeMapItemList[k];
                    double areaSum = GroupTotalWeight;
                    if (k == i)
                    {
                        currentRect = (orientation == Orientation.Horizontal) ? new Rectangle((int)AvailableArea.X, (int)AvailableArea.Y, (int)(areaSum / AvailableArea.Height), (int)AvailableArea.Height) :
                                                                                new Rectangle((int)AvailableArea.X, (int)AvailableArea.Y, (int)AvailableArea.Width, (int)(areaSum / AvailableArea.Width));

                        AvailableArea = (orientation == Orientation.Horizontal) ? new Rectangle(AvailableArea.X + currentRect.Width, AvailableArea.Y, Math.Max(0, AvailableArea.Width - currentRect.Width), AvailableArea.Height) :
                                                                                  new Rectangle(AvailableArea.X, AvailableArea.Y + currentRect.Height, AvailableArea.Width, Math.Max(0, AvailableArea.Height - currentRect.Height));

                        curX = currentRect.X;
                        curY = currentRect.Y;


                        //graphics.FillRectangle(brush, AvailableArea);
                    }

                    Rectangle rect;
                    if (OrderedTreeMapItemList.IndexOf(item) != itemsCount - 1)
                    {
                        rect = (orientation == Orientation.Horizontal) ? new Rectangle(0
                            , 0, (int)(currentRect.Width - Gap), (int)(item.AreaByWeight / currentRect.Width)) :

                            new Rectangle(0, 0, (int)(item.AreaByWeight / currentRect.Height), (int)(currentRect.Height - Gap));
                        if (j - k != 1)
                            rect = (orientation == Orientation.Horizontal) ? new Rectangle(0, 0, (int)rect.Width, (int)(rect.Height - Gap)) :
                                                                             new Rectangle(0, 0, (int)(rect.Width - Gap), (int)rect.Height);
                    }
                    else
                    {
                        rect = (orientation == Orientation.Horizontal) ? new Rectangle(0, 0, (int)currentRect.Width, (int)(item.AreaByWeight / currentRect.Width)) :
                                                                         new Rectangle(0, 0, (int)(item.AreaByWeight / currentRect.Height), currentRect.Height);

                    }

                    if (i == itemsCount - 1)
                    {
                        if (orientation == Orientation.Horizontal && rect.Width != AvailableArea.Width)
                            rect.Width += AvailableArea.Width;
                        if (orientation == Orientation.Vertical && rect.Height != AvailableArea.Height)
                            rect.Height += AvailableArea.Height;
                    }

                    item.ItemWidth = rect.Width;
                    item.ItemHeight = rect.Height;
                    item.LeftPosition = curX;
                    item.TopPosition = curY;
                    Pen pen1 = new Pen(new SolidBrush(Color.Red));

                    Rectangle tempRect = new Rectangle((int)curX, (int)curY, (int)rect.Width, (int)rect.Height);

                    LeafItemDrawingEventArgs leafEventArgs = new LeafItemDrawingEventArgs();
                    leafEventArgs.Graphics = graphics;
                    leafEventArgs.RectSize = tempRect;
                    leafEventArgs.Color = item.HeaderColor;
                    leafEventArgs.Label = item.Label;
                    if (item.Data is IEnumerable<Object>)
                    {
                        var enumerator = (item.Data as IEnumerable<Object>).GetEnumerator();
                        enumerator.MoveNext();
                        leafEventArgs.Data = enumerator.Current;
                    }
                    else
                        leafEventArgs.Data = item.Data;

                    Rectangle headerRect = new Rectangle((int)tempRect.X, (int)tempRect.Y, (int)tempRect.Width, (int)tempRect.Height);
                    if (item.ChildTreeMapItems != null && item.ChildTreeMapItems.Count > 0)
                    {
                        graphics.FillRectangle(HeaderBackgroundBrush, headerRect);
                        graphics.DrawRectangle(new Pen(HeaderBorderBrush, HeaderBorderThickness), headerRect);
                        graphics.DrawString(item.Label, LabelFont, HeaderBrush, headerRect);
                    }
                    else
                    {
                        RaiseLeafItemDrawingEvent(leafEventArgs);
                        if (!muteDrawing)
                        {
                            graphics.FillRectangle(new SolidBrush(Color.White), tempRect);
                            if (item != null && item.HeaderColor != null)
                                graphics.FillRectangle(item.HeaderColor, tempRect);
                            graphics.DrawRectangle(new Pen(new SolidBrush(Color.White), 2), tempRect);
                            int tempHeaderHeight = tempRect.Height < (int)LabelFont.Height ? tempRect.Height : (int)LabelFont.Height;
                            Rectangle TempHeaderRect = new Rectangle((int)tempRect.X, (int)tempRect.Y, (int)tempRect.Width, tempHeaderHeight);
                            graphics.FillRectangle(labelBackgroundBrush, TempHeaderRect);
                            graphics.DrawRectangle(new Pen(labelBorderBrush, labelBorderThickness), TempHeaderRect);
                            graphics.DrawString(item.Label, LabelFont, labelBrush, TempHeaderRect);
                        }
                    }
                    if (orientation == Orientation.Horizontal)
                    {
                        if (j - k != 1)
                            curY = curY + rect.Height + Gap;
                        else
                            curY += rect.Height;
                    }
                    else
                    {
                        if (j - k != 1)
                            curX = curX + rect.Width + Gap;
                        else
                            curX += rect.Width;
                    }
                }
            }
        }

        void DrawHierarchicalSliceAndDiceItems(List<TreeMapItem> TreeMapItemList, double prevItemWidth, double prevItemHeight, Size AvailableSize, double Gap, double HeaderHeight, Graphics graphics, Brush brush)
        {
            List<TreeMapItem> sortedItems = new List<TreeMapItem>((TreeMapItemList).OrderByDescending(x => x.AreaByWeight));
            double temptopItemPosition = topItemPosition;
            double templeftItemPosition = leftItemPosition;
            for (int j = 0; j < sortedItems.Count; j++)
            {
                double gapWidth = this.Levels[0].GroupingGap; ;
                if (sortedItems[j].ChildTreeMapItems != null && sortedItems[j].ChildTreeMapItems.Count > 0)
                {
                    double tempWidth = sortedItems[j].ItemWidth;
                    double tempHeight = sortedItems[j].ItemHeight;
                    temptopItemPosition = (sortedItems[j].TopPosition + gapWidth);
                    templeftItemPosition = (sortedItems[j].LeftPosition + gapWidth);
                    if (sortedItems[j].ChildTreeMapItems != null && sortedItems[j].ChildTreeMapItems.Count > 0)
                    {
                        List<TreeMapItem> items = new List<TreeMapItem>((sortedItems[j].ChildTreeMapItems).OrderByDescending(x => x.AreaByWeight));
                        CalculateSliceAndDiceItemsSize(items, templeftItemPosition, temptopItemPosition, new Size((int)tempWidth, (int)tempHeight), gapWidth, RootTreeMapItem.HeaderSize, graphics, brush);
                        DrawHierarchicalSliceAndDiceItems(items, templeftItemPosition, temptopItemPosition, new Size((int)tempWidth, (int)tempHeight), gapWidth, RootTreeMapItem.HeaderSize, graphics, brush);
                    }
                }
            }
        }

        void DrawHierarchicalSquarfiedItems(List<TreeMapItem> TreeMapItemList, double prevItemWidth, double prevItemHeight, Size AvailableSize, double Gap, double HeaderHeight, Graphics graphics, Brush brush)
        {
            List<TreeMapItem> sortedItems = new List<TreeMapItem>((TreeMapItemList).OrderByDescending(x => x.AreaByWeight));
            sortedItems = new List<TreeMapItem>(TreeMapItemList.OrderByDescending(x => x.AreaByWeight));
            double temptopItemPosition = topItemPosition;
            double templeftItemPosition = leftItemPosition;
            for (int j = 0; j < sortedItems.Count; j++)
            {
                double gapWidth = this.Levels[0].GroupingGap;
                if (sortedItems[j].ChildTreeMapItems != null && sortedItems[j].ChildTreeMapItems.Count > 0)
                {
                    double tempWidth = sortedItems[j].ItemWidth;
                    double tempHeight = sortedItems[j].ItemHeight;
                    temptopItemPosition = (sortedItems[j].TopPosition + gapWidth);
                    templeftItemPosition = (sortedItems[j].LeftPosition + gapWidth);
                    if (sortedItems[j].ChildTreeMapItems != null && sortedItems[j].ChildTreeMapItems.Count > 0)
                    {
                        List<TreeMapItem> items = new List<TreeMapItem>((sortedItems[j].ChildTreeMapItems).OrderByDescending(x => x.AreaByWeight));
                        CalculateSquarfiedItemsSize(items, templeftItemPosition, temptopItemPosition, new Size((int)tempWidth, (int)tempHeight), gapWidth, RootTreeMapItem.HeaderSize, graphics, brush);
                        DrawHierarchicalSquarfiedItems(items, templeftItemPosition, temptopItemPosition, new Size((int)tempWidth, (int)tempHeight), gapWidth, RootTreeMapItem.HeaderSize, graphics, brush);
                    }
                }
            }
        }

        void DrawLegends(Graphics graphics)
        {
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            int LeftMargin;
            int TopMargin;
            tempHeight = (this.Height - (this.Margin.Top + this.Margin.Bottom));
            tempWidth = (this.Width - (this.Margin.Left + this.Margin.Right));
            double legendSize = 10;
            switch (this.LegendPosition)
            {
                case LegendPositions.Left:
                    {
                        LeftMargin = this.Margin.Left;
                        TopMargin = this.Margin.Top;
                        for (int i = 0; i < (this.LeafColorMapping as RangeBrushColorMapping).Brushes.Count; i++)
                        {
                            var tempBrush = (this.LeafColorMapping as RangeBrushColorMapping).Brushes[i].Color;
                            string legendString = (this.LeafColorMapping as RangeBrushColorMapping).Brushes[i].LegendLabel;
                            Brush legendBrush = new SolidBrush(Color.FromArgb(tempBrush.A, tempBrush.R, tempBrush.G, tempBrush.B));
                            if (this.LegendType == LegendTypes.Ellipse)
                            {
                                graphics.DrawEllipse(new Pen(legendBrush, (float)legendSize), new Rectangle(LeftMargin + 10, TopMargin + 10, 10, 10));
                            }
                            else if (this.LegendType == LegendTypes.None)
                            {

                            }
                            else
                            {
                                graphics.DrawRectangle(new Pen(legendBrush, (float)legendSize), new Rectangle(LeftMargin + 10, TopMargin + 10, 10, 10));
                            }
                            graphics.DrawString(legendString, new System.Drawing.Font("Cooper", 12, FontStyle.Regular), legendBrush, new Rectangle(LeftMargin + 30, TopMargin + 6, 400, 100));
                            TopMargin += this.LegendGap;
                        }

                        topItemPosition = this.Margin.Top;
                        leftItemPosition = (this.Margin.Left + 200);
                        treeMapHeight = (this.Height - (this.Margin.Top + this.Margin.Bottom));
                        treeMapWidth = (this.Width - ((int)leftItemPosition + this.Margin.Right));
                        count++;
                        break;
                    }
                case LegendPositions.Right:
                    {
                        LeftMargin = this.Width - 200;
                        TopMargin = this.Margin.Top;
                        for (int i = 0; i < (this.LeafColorMapping as RangeBrushColorMapping).Brushes.Count; i++)
                        {
                            var tempBrush = (this.LeafColorMapping as RangeBrushColorMapping).Brushes[i].Color;
                            string legendString = (this.LeafColorMapping as RangeBrushColorMapping).Brushes[i].LegendLabel;
                            Brush legendBrush = new SolidBrush(Color.FromArgb(tempBrush.A, tempBrush.R, tempBrush.G, tempBrush.B));
                            if (this.LegendType == LegendTypes.Ellipse)
                            {
                                graphics.DrawEllipse(new Pen(legendBrush, (float)legendSize), new Rectangle(LeftMargin + 30, TopMargin, 10, 10));
                            }
                            else if (this.LegendType == LegendTypes.None)
                            {

                            }
                            else
                            {
                                graphics.DrawRectangle(new Pen(legendBrush, (float)legendSize), new Rectangle(LeftMargin + 30, TopMargin, 10, 10));
                            }
                            graphics.DrawString(legendString, new System.Drawing.Font("Cooper", 12, FontStyle.Regular), legendBrush, new Rectangle(LeftMargin + 50, TopMargin - 6, 400, 100));

                            TopMargin += this.LegendGap;
                        }

                        topItemPosition = this.Margin.Top;
                        leftItemPosition = this.Margin.Left;
                        treeMapHeight = this.Height - (this.Margin.Top + this.Margin.Bottom);
                        treeMapWidth = LeftMargin;
                        count++;
                        break;
                    }
                case LegendPositions.Bottom:
                    {
                        LeftMargin = this.Margin.Left;
                        TopMargin = this.Height - (this.Margin.Bottom + this.Margin.Top) - 40; // 40 specifies legend height.
                        for (int i = 0; i < (this.LeafColorMapping as RangeBrushColorMapping).Brushes.Count; i++)
                        {
                            var tempBrush = (this.LeafColorMapping as RangeBrushColorMapping).Brushes[i].Color;
                            string legendString = (this.LeafColorMapping as RangeBrushColorMapping).Brushes[i].LegendLabel;
                            Brush legendBrush = new SolidBrush(Color.FromArgb(tempBrush.A, tempBrush.R, tempBrush.G, tempBrush.B));
                            if (this.LegendType == LegendTypes.Ellipse)
                            {
                                graphics.DrawEllipse(new Pen(legendBrush, (float)legendSize), new Rectangle(LeftMargin + 10, TopMargin + 40, 10, 10));
                            }
                            else if (this.LegendType == LegendTypes.None)
                            {

                            }
                            else
                            {
                                graphics.DrawRectangle(new Pen(legendBrush, (float)legendSize), new Rectangle(LeftMargin + 10, TopMargin + 40, 10, 10));
                            }
                            graphics.DrawString(legendString, new System.Drawing.Font("Cooper", 12, FontStyle.Regular), legendBrush, new Rectangle(LeftMargin + 30, TopMargin + 35, 400, 100));

                            LeftMargin += this.LegendGap;
                            topItemPosition = this.Margin.Top;
                        }

                        topItemPosition = this.Margin.Top;
                        leftItemPosition = this.Margin.Left;
                        treeMapHeight = TopMargin;
                        treeMapWidth = (this.Width - (this.Margin.Left + this.Margin.Right));
                        count++;
                        break;
                    }
                default:
                    {
                        LeftMargin = this.Margin.Left;
                        TopMargin = this.Margin.Top;
                        for (int i = 0; i < (this.LeafColorMapping as RangeBrushColorMapping).Brushes.Count; i++)
                        {
                            var tempBrush = (this.LeafColorMapping as RangeBrushColorMapping).Brushes[i].Color;
                            string legendString = (this.LeafColorMapping as RangeBrushColorMapping).Brushes[i].LegendLabel;
                            Brush legendBrush = new SolidBrush(Color.FromArgb(tempBrush.A, tempBrush.R, tempBrush.G, tempBrush.B));
                            if (this.LegendType == LegendTypes.Ellipse)
                            {
                                graphics.DrawEllipse(new Pen(legendBrush, (float)legendSize), new Rectangle(LeftMargin + 10, TopMargin + 10, 10, 10));
                            }
                            else if (this.LegendType == LegendTypes.None)
                            {

                            }
                            else
                            {
                                graphics.DrawRectangle(new Pen(legendBrush, (float)legendSize), new Rectangle(LeftMargin + 10, TopMargin + 10, 10, 10));
                            }
                            graphics.DrawString(legendString, new System.Drawing.Font("Cooper", 12, FontStyle.Regular), legendBrush, new Rectangle(LeftMargin + 30, TopMargin + 6, 400, 100));

                            LeftMargin += this.LegendGap;
                        }
                        topItemPosition = this.Margin.Top + 40; // 40 specifies Legend Height
                        leftItemPosition = this.Margin.Left;
                        treeMapHeight = (this.Height - ((int)topItemPosition + this.Margin.Bottom));
                        treeMapWidth = (this.Width - (this.Margin.Left + this.Margin.Right));
                        count++;
                        break;
                    }
            }
        }


        #endregion

        #region Helper Method

        double AspectRatio(double x, double y)
        {
            return (x > y) ? (x / y) : (y / x);
        }

        Orientation GetOrientation()
        {
            if (this.ItemsLayoutMode == ItemsLayoutModes.SliceAndDiceHorizontal)
                return Orientation.Horizontal;
            if (this.ItemsLayoutMode == ItemsLayoutModes.SliceAndDiceVertical)
                return Orientation.Vertical;

            return AvailableArea.Width > AvailableArea.Height ? Orientation.Horizontal : Orientation.Vertical;
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region LeafItemDrawing Event

        public delegate void LeafItemDrawingEventHandler(object sender, LeafItemDrawingEventArgs e);

        public event LeafItemDrawingEventHandler LeafItemDrawing;

        void RaiseLeafItemDrawingEvent(LeafItemDrawingEventArgs e)
        {
            if (LeafItemDrawing != null)
                LeafItemDrawing(this, e);
        }

        #endregion

        #endregion

    }

    public class LeafItemDrawingEventArgs : CancelEventArgs
    {
        #region CLR Properties

        #region RectSize

        public Rectangle RectSize { get; set; }

        #endregion

        #region Color

        public Brush Color { get; set; }

        #endregion

        #region Graphics

        public Graphics Graphics { get; set; }

        #endregion

        #region Label

        public string Label { get; set; }

        #endregion

        #region Data

        public object Data { get; set; }

        #endregion

        #endregion
    }

    public class ToolTipInfo 
    {
        public string ToolTipHeaderPattern { get; set; }

        public string ToolTipContentPattern { get; set; }
    }

    static class SubstringExtensions
    {
        /// <summary>
        /// Get string value between [first] a and [last] b.
        /// </summary>
        public static string Between(this string value, string a, string b)
        {
            
            int posA = value.IndexOf(a);
            int posB = value.IndexOf(b);
            if (posA == -1)
            {
                return "";
            }
            if (posB == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= posB)
            {
                return "";
            }
            return value.Substring(adjustedPosA, posB - adjustedPosA);
        }

        /// <summary>
        /// Get string value after [first] a.
        /// </summary>
        public static string Before(this string value, string a)
        {
            int posA = value.IndexOf(a);
            if (posA == -1)
            {
                return "";
            }
            return value.Substring(0, posA);
        }

        /// <summary>
        /// Get string value after [last] a.
        /// </summary>
        public static string After(this string value, string a)
        {
            int posA = value.LastIndexOf(a);
            if (posA == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= value.Length)
            {
                return "";
            }
            return value.Substring(adjustedPosA);
        }
    }
}