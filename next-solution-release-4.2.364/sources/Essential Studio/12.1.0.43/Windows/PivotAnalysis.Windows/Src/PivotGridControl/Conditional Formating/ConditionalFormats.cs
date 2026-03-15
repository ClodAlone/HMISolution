#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.PivotAnalysis.Base;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Syncfusion.Linq;
using System.Windows.Forms;
using System.Drawing;
using Syncfusion.Drawing;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Data;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Syncfusion.Windows.Forms.PivotAnalysis
{
    #region PivotGridConditionalFormat class
    [Serializable]
    public class PivotGridConditionalFormat
    {
        #region Variable
        Dictionary<string, Delegate> delegateValues = null;
        private PivotCellInfo cellType = null;
        private GridStyleInfo pivotCellStyle = null;
        private string name = "ConditionalFormat1";
        #endregion

        #region Constructor
        public PivotGridConditionalFormat()
        {
            delegateValues = new Dictionary<string, Delegate>();
            cellType = new PivotCellInfo();
            pivotCellStyle = new GridStyleInfo();
            this.Conditions = new ObservableCollection<ConditionalFormat>();
        }

        #endregion

        #region Property
        /// <summary>
        /// Get or set PivotCellInfo.
        /// </summary>
        public PivotCellInfo PivotCellInfo
        {
            get { return cellType; }
            set { cellType = value; }
        }

        /// <summary>
        /// Get or set PivotCellStyle.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridStyleInfo PivotCellStyle
        {
            get { return pivotCellStyle; }
            set { pivotCellStyle = value; }
        }

        /// <summary>
        /// Get or set Name of the collection.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }               

        /// <summary>
        /// Gets or sets the conditions.
        /// </summary>
        private ObservableCollection<ConditionalFormat> _conditions;

        /// <summary>
        /// Gets or sets the conditions collections.
        /// </summary>        
        public ObservableCollection<ConditionalFormat> Conditions
        {
            get { return _conditions; }
            set { _conditions = value; }
        }

        #region [ Helper Methods ]

        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Checks whether the current cell qualifies for conditional formatting
        /// </summary>
        /// <param name="cellInfo">The cell info.</param>
        /// <returns></returns>
        internal bool ApplyFormat(PivotCellInfo cellInfo, string associatedMeasure)
        {
            Delegate del = null;
            if (delegateValues.Count > 0)
            {
                if (delegateValues.ContainsKey(associatedMeasure))
                {
                    del = delegateValues[associatedMeasure];
                }
            }

            if (del == null)
            {
                Delegate comDelegate = this.GetCompliedDelegate(cellInfo, associatedMeasure);
                if (comDelegate != null)
                {
                    return (bool)comDelegate.DynamicInvoke(new object[] { cellInfo });
                }
                else
                    return false;
            }
            else
            {
                return (bool)del.DynamicInvoke(new object[] { cellInfo });
            }
        }

        /// <summary>
        /// Applying style for supplied GridStyleInfo
        /// </summary>
        /// <param name="styleInfo">GridStyleInfo</param>
        /// <param name="cellStyle">GridStyleInfo</param>
        private void ApplyStyle(GridStyleInfo styleInfo, GridStyleInfo cellStyle)
        {
            if (cellStyle != null)
            {
                styleInfo.BackColor = cellStyle.BackColor;
                styleInfo.Font = cellStyle.Font;
                styleInfo.TextColor = cellStyle.TextColor;
            }
        }
        /// <summary>
        /// Gets the complied delegate.
        /// </summary>
        /// <param name="cellInfo">The cell info.</param>
        /// <param name="associatedMeasure">The associated measure.</param>
        /// <returns></returns>
        private Delegate GetCompliedDelegate(PivotCellInfo cellInfo, string associatedMeasure)
        {
            if (this.Conditions.Count > 0)
            {
                bool firstLoop = false;
                System.Linq.Expressions.Expression predicate = null;
                Type recordType = cellInfo.GetType();
                ParameterExpression paramExp = recordType.Parameter();
                foreach (var condition in this.Conditions)
                {
                    if (condition.SummaryElement == associatedMeasure)
                    {
                        if (!firstLoop)
                        {
                            predicate = Predicate(paramExp, condition.ConditionType, condition);
                            firstLoop = true;
                        }
                        else
                        {
                            if (condition.PredicateType == PredicateType.And)
                            {
                                predicate = predicate.AndAlsoPredicate(Predicate(paramExp, condition.ConditionType, condition));
                            }
                            else if (condition.PredicateType == PredicateType.Or)
                            {
                                predicate = predicate.OrElsePredicate(Predicate(paramExp, condition.ConditionType, condition));
                            }
                        }
                    }
                }
                if (predicate != null)
                {
                    var lambda = System.Linq.Expressions.Expression.Lambda(predicate, paramExp);
                    delegateValues.Add(associatedMeasure, lambda.Compile());
                    return this.delegateValues[associatedMeasure];
                }
                else
                    return null;
            }
            else
                return null;
        }

        /// <summary>
        /// Returns the Predicate based on ConditionType and Parameter Expression
        /// </summary>
        /// <param name="paramExp">The param exp.</param>
        /// <param name="pivotGridDataConditionType">Type of the pivot grid data condition.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private System.Linq.Expressions.Expression Predicate(ParameterExpression paramExp, PivotGridDataConditionType pivotGridDataConditionType, ConditionalFormat Condition)
        {
            System.Linq.Expressions.Expression predicate = null;
            switch (pivotGridDataConditionType)
            {
                case PivotGridDataConditionType.Equals:
                    predicate = paramExp.Equal("DoubleValue", Condition.StartValue);
                    break;
                case PivotGridDataConditionType.GreaterThan:
                    predicate = paramExp.GreaterThan("DoubleValue", Condition.StartValue);
                    break;
                case PivotGridDataConditionType.GreaterThanOrEqual:
                    predicate = paramExp.GreaterThanOrEqual("DoubleValue", Condition.StartValue);
                    break;
                case PivotGridDataConditionType.LessThan:
                    predicate = paramExp.LessThan("DoubleValue", Condition.StartValue);
                    break;
                case PivotGridDataConditionType.LessThanOrEqual:
                    predicate = paramExp.LessThanOrEqual("DoubleValue", Condition.StartValue);
                    break;
                case PivotGridDataConditionType.NotEquals:
                    predicate = paramExp.NotEqual("DoubleValue", Condition.StartValue);
                    break;
                case PivotGridDataConditionType.Between:
                    double s_value = 0, e_value = 0;
                    if ((double.Parse(Condition.StartValue.ToString())) <= (double.Parse(Condition.EndValue.ToString())))
                    {
                        s_value = double.Parse(Condition.StartValue.ToString());
                        e_value = double.Parse(Condition.EndValue.ToString());
                    }
                    else if ((double.Parse(Condition.StartValue.ToString())) >= (double.Parse(Condition.EndValue.ToString())))
                    {
                        s_value = double.Parse(Condition.EndValue.ToString());
                        e_value = double.Parse(Condition.StartValue.ToString());
                    }
                    predicate = paramExp.GreaterThanOrEqual("DoubleValue", s_value);
                    if (Condition.PredicateType == PredicateType.And)
                        predicate = predicate.AndAlsoPredicate(paramExp.LessThanOrEqual("DoubleValue", e_value));
                    else
                        predicate = predicate.OrElsePredicate(paramExp.LessThanOrEqual("DoubleValue", e_value));
                    break;
            }
            return predicate;
        }

        #endregion
    }
    #endregion

    #endregion

    #region ConditionalFormat class
    [Serializable]
    public class ConditionalFormat
    {
        #region Variable Declaration
        private PivotGridDataConditionType conditionType;
        private PredicateType predicateType;
        private double _startValue;
        private double _endValue;
        private string summaryElement;
        #endregion

        #region constructor
        public ConditionalFormat()
        {
            this.ConditionType = PivotGridDataConditionType.Equals;
            this.PredicateType = PredicateType.Or;
        }
        #endregion

        #region Property
        /// <summary>
        /// Get or set ConditionType
        /// </summary>
        public PivotGridDataConditionType ConditionType
        {
            get { return conditionType; }
            set { conditionType = value; }
        }

        /// <summary>
        /// Get or set PredicateType
        /// </summary>
        public PredicateType PredicateType
        {
            get { return predicateType; }
            set { predicateType = value; }
        }

        /// <summary>
        /// Get or set StartValue
        /// </summary>
        public double StartValue
        {
            get { return _startValue; }
            set { _startValue = value; }
        }

        /// <summary>
        /// Get or set EndValue
        /// </summary>
        public double EndValue
        {
            get { return _endValue; }
            set { _endValue = value; }
        }

        /// <summary>
        /// Get or set SummaryElement
        /// </summary>
        public string SummaryElement
        {
            get { return summaryElement; }
            set { summaryElement = value; }
        }
        #endregion
    }
    #endregion

    #region NewRuleConditionalFormat class
    [Serializable]
    public class NewRuleConditionalFormat
    {
        #region Variable Declaration
        private RuleType _ruleType;
        private FormatStyle _formatStyle;
        private IconImageCategory _iconImage;
        private ValueType _valueType1;
        private ValueType _valueType2;
        private ValueType _valueType3;
        private ValueType _valueType4;
        private FormateValuesRankType _rank;
        private FormateSelectedAverageValueType _formateSelectedAverageValueType;
        private FormatAllType _formatAllType;
        private ArrayList _customImage = new ArrayList();
        private int _formatRankValue = 10;       
        private Color _minValueColor = Color.Orange;
        private Color _midValueColor = Color.PaleVioletRed;
        private Color _maxValueColor = Color.YellowGreen;        
        private double _value1 = 20;
        private double _value2 = 40;
        private double _value3 = 60;
        private double _value4 = 80;
        private ObservableCollection<ConditionalFormat> _conditions = null;
        private string summaryElement = string.Empty;
        private BitMapImageName _imageName;
        private GridRangeInfo selectedRanges = null;
        #endregion

        #region Constructor
        public NewRuleConditionalFormat()
        {
            this.Ranges = new GridRangeInfo();
            this._conditions = new ObservableCollection<ConditionalFormat>();
            this.CustomImageList = new ArrayList();
        }
        #endregion

        #region Property
        /// <summary>
        /// Get or Set GridRanges.
        /// </summary>
        public GridRangeInfo Ranges
        {
            get { return selectedRanges; }
            set { selectedRanges = value; }
        }

        /// <summary>
        /// Get or Set Summary Element
        /// </summary>
        public string SummaryElement
        {
            get { return summaryElement; }
            set { summaryElement = value; }
        }

        /// <summary>
        /// Get or Set RuleType
        /// </summary>
        public RuleType RuleType
        {
            get { return _ruleType; }
            set { _ruleType = value; }
        }

        /// <summary>
        /// Get or Set FormateValuesRankType.
        /// </summary>
        public FormateValuesRankType FormateValuesRankType
        {
            get { return _rank; }
            set { _rank = value; }
        }

        /// <summary>
        /// Get or Set FormateSelectedAverageValueType.
        /// </summary>
        public FormateSelectedAverageValueType FormateSelectedAverageValueType
        {
            get { return _formateSelectedAverageValueType; }
            set { _formateSelectedAverageValueType = value; }
        }

        /// <summary>
        /// Get or Set FormatAllType.
        /// </summary>
        public FormatAllType FormatAllType
        {
            get { return _formatAllType; }
            set { _formatAllType = value; }
        }

        /// <summary>
        /// Get or Set RankValue.
        /// </summary>
        public int RankValue
        {
            get { return _formatRankValue; }
            set { _formatRankValue = value; }
        }

        /// <summary>
        /// Get or Set FormatStyle.
        /// </summary>
        public FormatStyle FormatStyle
        {
            get { return _formatStyle; }
            set { _formatStyle = value; }
        }

        /// <summary>
        /// Get or Set IconImageCategory.
        /// </summary>
        [Browsable(false)]
        public IconImageCategory IconImage
        {
            get { return _iconImage; }
            set { _iconImage = value; }
        }

        /// <summary>
        /// Get or Set the customize image.
        /// </summary>       
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ArrayList CustomImageList
        {
            get { return _customImage; }
            set { _customImage = value; }
        }
        /// <summary>
        /// Get or Set ValueType.
        /// </summary>
        public ValueType ValueType1
        {
            get { return _valueType1; }
            set { _valueType1 = value; }
        }

        /// <summary>
        /// Get or Set ValueType.
        /// </summary>
        public ValueType ValueType2
        {
            get { return _valueType2; }
            set { _valueType2 = value; }
        }

        /// <summary>
        /// Get or Set ValueType.
        /// </summary>
        public ValueType ValueType3
        {
            get { return _valueType3; }
            set { _valueType3 = value; }
        }

        /// <summary>
        /// Get or Set ValueType.
        /// </summary>
        public ValueType ValueType4
        {
            get { return _valueType4; }
            set { _valueType4 = value; }
        }  

        /// <summary>
        /// Get or set Value1
        /// by default values taken here percent/number and ranked values top/bottom
        /// </summary>
        public double Value1
        {
            get { return _value1; }
            set { _value1 = value; }
        }

        /// <summary>
        /// Get or Set Value2.
        /// </summary>
        public double Value2
        {
            get { return _value2; }
            set { _value2 = value; }
        }

        /// <summary>
        /// Get or Set Value2.
        /// </summary>
        public double Value3
        {
            get { return _value3; }
            set { _value3 = value; }
        }

        /// <summary>
        /// Get or Set Value4.
        /// </summary>
        public double Value4
        {
            get { return _value4; }
            set { _value4 = value; }
        }

        /// <summary>
        /// Gets or sets the ImageName.
        /// </summary>
        /// <value>The ImageName.</value>
        public BitMapImageName  Image
        {
            get { return _imageName; }
            set { _imageName = value; }
        }

        /// <summary>
        /// Gets or sets the conditions.
        /// </summary>
        /// <value>The conditions.</value>
        public ObservableCollection<ConditionalFormat> Conditions
        {
            get { return _conditions; }
            set { _conditions = value; }
        }
        #endregion
    }
    #endregion

    #region PivotGridNewRuleConditionalFormat class    
    [Serializable]
    public class PivotGridNewRuleConditionalFormat
    {
        #region Variable Decalaration

        private PivotCellInfo cellInfo = null;
        private GridStyleInfo pivotCellStyle = null;
        internal static IconPaint iconPainter;
        private ObservableCollection<NewRuleConditionalFormat> _newRule;
        private bool ApplyOnRanges = false;
        private string name = "NewRuleConditionalFormat1";
        Dictionary<int, Delegate> delegateValues = null;
        #endregion

        #region Constructor
        public PivotGridNewRuleConditionalFormat()
        {
            this.NewRuleCollections = new ObservableCollection<NewRuleConditionalFormat>();
            this.PivotCellStyle = new GridStyleInfo();
            this.PivotCellInfo = new PivotCellInfo();
            this.delegateValues = new Dictionary<int, Delegate>();
        }
        #endregion

        #region Property
        /// <summary>
        /// Get or Set PivotCellInfo.
        /// </summary>
        public PivotCellInfo PivotCellInfo
        {
            get { return cellInfo; }
            set { cellInfo = value; }
        }

        /// <summary>
        /// Get or Set PivotCellStyle.
        /// </summary>        
        public GridStyleInfo PivotCellStyle
        {
            get
            {
                //if (ShouldSerializeStyle())
                //    pivotCellStyle = new GridStyleInfo();
                return pivotCellStyle;
            }
            set
            {
                pivotCellStyle = value;
            }
        }

        /// <summary>
        /// Get or Set New rule conditional format Collections name.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Get or Set Collection of NewRuleConditionalFormat.
        /// </summary>
        public ObservableCollection<NewRuleConditionalFormat> NewRuleCollections
        {
            get { return _newRule; }
            set { _newRule = value; }
        }

        /// <summary>
        ///  Get or Set IconPainter.
        /// </summary>
        internal static IconPaint IconPainter
        {
            get
            {
                if (iconPainter == null)
                {
                    iconPainter = new IconPaint(AssemblyInfo.RootNamespace + @".Resources.", AssemblyInfo.Assembly);
                }

                return iconPainter;
            }
        }

        /// <summary>
        /// Determines if style information has been set and should be serialized into code at design-time.
        /// </summary>
        /// <returns>True if style information has been set; False otherwise.</returns>
        public bool ShouldSerializeStyle()
        {
            return this.pivotCellStyle != null && !pivotCellStyle.IsEmpty;
        }

        /// <summary>
        /// Resets style information.
        /// </summary>
        public void ResetStyle()
        {
            if (ShouldSerializeStyle())
            {
                pivotCellStyle = null;
            }
        }

        public override string ToString()
        {
            PivotGridNewRuleConditionalFormat item = new PivotGridNewRuleConditionalFormat();
            return string.Format("{0}", item.Name);
        }

        /// <summary>
        /// Get the bitmap image
        /// </summary>
        /// <param name="name">Image Name</param>
        /// <returns>Bitmap image</returns>
        internal static Bitmap GetBitmap(string name)
        {
            return IconPainter.GetBitmap(name.ToLower() + ".png");
        }
        #endregion

        #region Method       

        /// <summary>
        /// Creates a copy of this descriptor.
        /// </summary>
        /// <returns>A copy of this descriptor.</returns>
        public PivotGridNewRuleConditionalFormat Clone()
        {
            PivotGridNewRuleConditionalFormat rd = new PivotGridNewRuleConditionalFormat();
            rd.InitializeFrom(this);  
            return rd;
        }

        /// <summary>
        /// Initializes this object and copies properties from another object. <see cref="PropertyChanging"/>
        /// and <see cref="PropertyChanged"/> events are raised for every property that is modified. If both
        /// objects are equal, no events are raised.
        /// </summary>
        /// <param name="other">The source object.</param>
        public void InitializeFrom(PivotGridNewRuleConditionalFormat other)
        {
            this.Name = other.Name;          
        }

        /// <summary>
        /// Create a image list based on the category.
        /// </summary>
        /// <param name="IconImageCategory">Image category</param>
        /// <param name="ImageName">Image name</param>
        /// <returns></returns>
        internal ImageList CreateImageList(object IconImageCategory, string ImageName)
        {
            ImageList ImageLst = new ImageList();
            switch (IconImageCategory.ToString())
            {
                case "Three":
                    ImageLst = InitImageListFromDirectory(3, ImageLst, ImageName);
                    break;
                case "Four":
                    ImageLst = InitImageListFromDirectory(4, ImageLst, ImageName);
                    break;
                case "Five":
                    ImageLst = InitImageListFromDirectory(5, ImageLst, ImageName);
                    break;
            }
            return ImageLst;
        }

        /// <summary>
        /// To bind the image from the directory.
        /// </summary>
        /// <param name="num">number of image</param>
        /// <param name="ImageList">ImageList</param>
        /// <param name="ImageName">Image name</param>
        /// <returns></returns>
        internal ImageList InitImageListFromDirectory(int num, ImageList ImageList, string ImageName)
        {
            ImageList imageList = new ImageList();
            for (int i = 0; i < num; i++)
            {
                string pDirectory = string.Format("{0}{1}.png", ImageName, i);
                Image img = IconPainter.GetBitmap(pDirectory);
                imageList.Images.Add(img);
            }
            return imageList;
        }


        /// <summary>
        /// Gets the image count.
        /// </summary>
        /// <param name="imageName"> Image name</param>
        /// <returns>Image category value</returns>
        internal int GetImageCount(string imageName)
        {
            switch (imageName)
            {
                case "FiveArrowsColored":
                case "FiveArrowsGray":
                case "FiveBoxes":
                case "FiveQuarters":
                case "FiveRatings":
                    return 5;
                case "FourArrowsColored":
                case "FourArrowsGray":
                case "FourRating":
                case "FourTrafficLight":
                case "RedToBlack":
                    return 4;
                default:
                    return 3;
            }           
        }

        /// <summary>
        /// Get the image name
        /// </summary>
        /// <param name="imageName">BitMap Image Name</param>
        /// <returns>Image name</returns>
        internal string GetImageName(BitMapImageName imageName)
        {
            string _imageName = string.Empty;
            switch (imageName)
            {
                case BitMapImageName.ThreeArrowsColored:
                    return "ThreeArrowsColored";
                case BitMapImageName.ThreeArrowsGray:
                    return "ThreeArrowsGray";
                case BitMapImageName.ThreeFlags:
                    return "ThreeFlags";
                case BitMapImageName.FiveArrowsColored:
                    return "FiveArrowsColored";
                case BitMapImageName.FiveArrowsGray:
                    return "FiveArrowsGray";
                case BitMapImageName.FiveBoxes:
                    return "FiveBoxes";
                case BitMapImageName.FiveQuarters:
                    return "FiveQuarters";
                case BitMapImageName.FiveRatings:
                    return "FiveRatings";
                case BitMapImageName.FourArrowsColored:
                    return "FourArrowsColored";
                case BitMapImageName.FourArrowsGray:
                    return "FourArrowsGray";
                case BitMapImageName.FourRating:
                    return "FourRating";
                case BitMapImageName.FourTrafficLight:
                    return "FourTrafficLight";
                case BitMapImageName.RedToBlack:
                    return "RedToBlack";
                case BitMapImageName.ThreeSigns:
                    return "ThreeSigns";
                case BitMapImageName.ThreeStars:
                    return "ThreeStars";
                case BitMapImageName.ThreeSymbols:
                    return "ThreeSymbols";
                case BitMapImageName.ThreeSymbolsCircled:
                    return "ThreeSymbolsCircled";
                case BitMapImageName.ThreeSymbolsUnCircled:
                    return "ThreeSymbolsUnCircled";
                case BitMapImageName.ThreeTrafficLightsRimmed:
                    return "ThreeTrafficLightsRimmed";
                case BitMapImageName.ThreeTrafficLightsUnrimmed:
                    return "ThreeTrafficLightsUnrimmed";
                case BitMapImageName.ThreeTriangle:
                    return "ThreeTriangle";
            }
            return _imageName;
        }

        /// <summary>
        /// Gets the image category
        /// </summary>
        /// <param name="count">Image list count</param>
        /// <returns>Image category count</returns>
        private IconImageCategory imageCount(int count)
        {
            switch (count)
            {               
                case 4:
                    return IconImageCategory.Four;
                case 5:
                    return IconImageCategory.Five;
                default :
                    return IconImageCategory.Three;
            }
        }
      /// <summary>
      /// Checks whether the current cell qualifies for conditional formatting
      /// </summary>
      /// <param name="cellInfo">GridStyleInfo</param>
      /// <param name="row">RowIndex</param>
      /// <param name="col">Column Index</param>
      /// <param name="pivotGrid">PivotGridControlBase</param>
      /// <param name="pivotCellInfo">PivotCellInfo</param>
        internal void ApplyStyle(GridStyleInfo cellInfo, int row, int col, PivotGridControlBase pivotGrid, PivotCellInfo pivotCellInfo)
        {
            if (this.NewRuleCollections.Count > 0)
            {
                ArrayList Values = new ArrayList();
                ImageList ImageLst = new ImageList();
                string associatedMeasure = GetAssociatedMeasure(pivotGrid, pivotCellInfo, row, col);
                foreach (var newRule in this.NewRuleCollections)
                {
                    if (newRule.Ranges.RangeType != GridRangeInfoType.Empty && newRule.Ranges.IntersectsWith(GridRangeInfo.Cell(row, col)) && string.IsNullOrEmpty(newRule.SummaryElement))
                    {
                        ApplyOnRanges = true;
                    }
                    else if (newRule.Ranges.RangeType == GridRangeInfoType.Empty && !string.IsNullOrEmpty(newRule.SummaryElement)
                      && associatedMeasure == newRule.SummaryElement)
                    {
                        ApplyOnRanges = false;
                    }
                    else
                        return;
                    if (newRule.FormatStyle == FormatStyle.IconSets && newRule.RuleType == RuleType.FormatAllCellsBasedOnTheirValues)
                    {
                        string imageName = GetImageName(newRule.Image);
                        if (newRule.CustomImageList == null || newRule.CustomImageList.Count == 0)
                        {
                            newRule.IconImage = imageCount(GetImageCount(imageName));
                            ImageLst = CreateImageList(newRule.IconImage, imageName);
                        }
                        else
                        {
                            ImageList imageList = new ImageList();
                            for (int imgCount = 0; imgCount < newRule.CustomImageList.Count; imgCount++)
                            {                                
                                Image img = Image.FromFile(newRule.CustomImageList[imgCount].ToString());
                                imageList.Images.Add(img);
                            }
                            ImageLst = imageList;
                            newRule.IconImage = imageCount(ImageLst.Images.Count);
                        }
                        cellInfo.ImageList = ImageLst;
                        Values = GetMaxAndMinVale(cellInfo, pivotGrid, newRule);
                        cellInfo.ImageIndex = GetImageIndex(cellInfo, Values, newRule);
                    }

                    else if (newRule.RuleType == RuleType.FormatOnlyCellsThatContain)
                    {
                        bool apply = GetPredicate(pivotCellInfo, col, row);
                        if (apply)
                            this.ApplyStyle(cellInfo, this.PivotCellStyle);
                    }
                    else if (newRule.RuleType == RuleType.FormatTopOrBottomRankedValues)
                    {
                        List<double> FilteredRankedValues = new List<double>();
                        List<double> RankedValues = new List<double>();
                        RankedValues = BindRankedList(newRule, pivotGrid);
                        if (newRule.FormateValuesRankType == FormateValuesRankType.Top)
                            FilteredRankedValues = RankedValues.Select(b => b).OrderByDescending(c => c).Take(newRule.RankValue).ToList();
                        else
                            FilteredRankedValues = RankedValues.Select(b => b).OrderBy(c => c).Take(newRule.RankValue).ToList();
                        SetFilteredRankStyle(pivotCellInfo, cellInfo, FilteredRankedValues);
                    }
                    else if (newRule.RuleType == RuleType.FormatOnlyValuesThatAreAboveOrBelowAverage)
                    {
                        List<double> FilteredRankedValues = new List<double>();
                        List<double> RankedValues = new List<double>();
                        RankedValues = BindRankedList(newRule, pivotGrid);
                        if (RankedValues != null)
                        {
                            double sum = RankedValues.Select(b => b).Sum();
                            double avg = sum / RankedValues.Count;
                            SetFilteredRankStyle(pivotCellInfo, cellInfo, newRule.FormateSelectedAverageValueType, avg);
                        }
                    }
                    else if (newRule.RuleType == RuleType.FormatOnlyUniqueOrDuplicateValues)
                    {
                        List<double> RankedValues = new List<double>();
                        RankedValues = BindRankedList(newRule, pivotGrid);
                        SetFilteredRankStyle(pivotCellInfo, cellInfo, newRule.FormatAllType, RankedValues);
                    }

                }
            }
        }

        /// <summary>
        /// Get the Associated Column name in pivot.
        /// </summary>
        /// <param name="pivotGrid">PivotGridControlBase</param>
        /// <param name="cellInfo">PivotCellInfo</param>
        /// <param name="rowIndex">RowIndex</param>
        /// <param name="columnIndex">ColumnIndex</param>
        /// <returns></returns>
        private string GetAssociatedMeasure(PivotGridControlBase pivotGrid, PivotCellInfo cellInfo, int rowIndex, int columnIndex)
        {
            if (pivotGrid.PivotCalculations.Count > 0)
            {
                if (pivotGrid.PivotCalculations.Count > 1)
                {
                    if (pivotGrid.ShowCalculationsAsColumns)
                    {
                        return pivotGrid.PivotEngine[pivotGrid.PivotColumns.Count, columnIndex - 1].FormattedText;
                    }
                    else
                    {
                        return pivotGrid.PivotEngine[pivotGrid.PivotRows.Count, columnIndex - 1].FormattedText;
                    }
                }
                else
                    return pivotGrid.PivotCalculations[0].FieldName;
            }
            else
                return string.Empty;
        }      

        /// <summary>
        /// To set the filter Rank Style based on the condition.
        /// </summary>
        /// <param name="pivotCellInfo">PivotCellInfo</param>
        /// <param name="cellInfo">GridStyleInfo</param>
        /// <param name="FormatAllType">FormatAllType</param>
        /// <param name="RankedValues">List<double></param>
        internal void SetFilteredRankStyle(PivotCellInfo pivotCellInfo, GridStyleInfo cellInfo, FormatAllType FormatAllType, List<double> RankedValues)
        {
            List<double> FilteredRankedValues = new List<double>();
            List<double> FilteredRankedValuesA = new List<double>();
            List<double> FilteredRankedValuesB = new List<double>();
            switch (FormatAllType)
            {
                case FormatAllType.Duplicate:
                    FilteredRankedValues = RankedValues.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList();                   
                    break;
                case FormatAllType.Unique:
                    FilteredRankedValuesA = RankedValues.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
                    FilteredRankedValuesB = RankedValues.Select(b => b).Distinct().ToList();
                    FilteredRankedValues = FilteredRankedValuesB.Except(FilteredRankedValuesA.ToList()).Select(a => a).ToList();
                    break;
            }
            SetFilteredRankStyle(pivotCellInfo, cellInfo, FilteredRankedValues);   
        }

        /// <summary>
        /// To set the filter Rank Style based on the condition.
        /// </summary>
        /// <param name="pivotCellInfo">PivotCellInfo</param>
        /// <param name="cellInfo">GridStyleInfo</param>
        /// <param name="FormateSelectedAverageValueType">Formate Selected ValueType</param>
        /// <param name="avgValue">Average Value</param>
        internal void SetFilteredRankStyle(PivotCellInfo pivotCellInfo, GridStyleInfo cellInfo, FormateSelectedAverageValueType FormateSelectedAverageValueType, double avgValue)
        {
            double formatValues = 0;
            if (double.TryParse(pivotCellInfo.FormattedText, out formatValues))
            {
                switch (FormateSelectedAverageValueType)
                {
                    case PivotAnalysis.FormateSelectedAverageValueType.Above:
                        if (avgValue < formatValues)
                            this.ApplyStyle(cellInfo, this.PivotCellStyle);
                        break;
                    case PivotAnalysis.FormateSelectedAverageValueType.Below:
                        if (avgValue > formatValues)
                            this.ApplyStyle(cellInfo, this.PivotCellStyle);
                        break;
                    case PivotAnalysis.FormateSelectedAverageValueType.EqualOrAbove:
                        if (avgValue <= formatValues)
                            this.ApplyStyle(cellInfo, this.PivotCellStyle);
                        break;
                    case PivotAnalysis.FormateSelectedAverageValueType.EqualOrBelow:
                        if (avgValue >= formatValues)
                            this.ApplyStyle(cellInfo, this.PivotCellStyle);
                        break;
                }
            }
        }
        /// <summary>
        /// Used to bind the Ranked list
        /// </summary>
        /// <param name="newRule">NewRuleConditionalFormat</param>
        /// <param name="pivotGrid">PivotGridControlBase</param>
        /// <returns>List</returns>
        internal List<double> BindRankedList(NewRuleConditionalFormat newRule, PivotGridControlBase pivotGrid)
        {
            List<double> RankedValues = new List<double>();
            RankedValues = FindRankValues(newRule, pivotGrid);
            return RankedValues;
        }

        /// <summary>
        /// To set the filtered rank style based on the condition.
        /// </summary>
        /// <param name="pivotCellInfo">PivotCellInfo</param>
        /// <param name="cellInfo">GridStyleInfo</param>
        /// <param name="FilteredRankedValues">List</param>
        internal void SetFilteredRankStyle(PivotCellInfo pivotCellInfo, GridStyleInfo cellInfo, List<double> FilteredRankedValues)
        {
            double formatValues = 0;
            if (double.TryParse(pivotCellInfo.FormattedText, out formatValues))
            {
                if (FilteredRankedValues.Contains(formatValues))
                    this.ApplyStyle(cellInfo, this.PivotCellStyle);
            }
        }

        /// <summary>
        /// To Find the Ranked Values.
        /// </summary>
        /// <param name="conditionalFormat">NewRuleConditionalFormat</param>
        /// <param name="pivotGrid">PivotGridControlBase</param>
        /// <returns>List</returns>
        private List<double> FindRankValues(NewRuleConditionalFormat conditionalFormat, PivotGridControlBase pivotGrid)
        {
            List<double> RankedValues = new List<double>();
            if (!ApplyOnRanges)
            {
                for (int row = 1; row < pivotGrid.PivotEngine.RowCount; row++)
                {
                    for (int col = 1; col < pivotGrid.PivotEngine.ColumnCount; col++)
                    {
                        PivotCellInfo Info = pivotGrid.PivotEngine[row - 1, col - 1];
                        if (GetAssociatedMeasure(pivotGrid, Info, row, col) == conditionalFormat.SummaryElement)
                        {
                            if (Info.CellType == PivotCellType.ValueCell)
                            {
                                double formatedValue = 0;
                                if (double.TryParse(Info.FormattedText, out formatedValue))
                                {
                                    RankedValues.Add(formatedValue);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (int row = conditionalFormat.Ranges.Top; row <= conditionalFormat.Ranges.Bottom; row++)
                {
                    for (int col = conditionalFormat.Ranges.Left; col <= conditionalFormat.Ranges.Right; col++)
                    {
                        PivotCellInfo Info = pivotGrid.PivotEngine[row - 1, col - 1];
                        if (Info.CellType == PivotCellType.ValueCell)
                        {
                            double formatedValue = 0;
                            if (double.TryParse(Info.FormattedText, out formatedValue))
                            {
                                RankedValues.Add(formatedValue);
                            }
                        }
                    }
                }
            }
            return RankedValues;
        }

        /// <summary>
        /// Get predicate status.
        /// </summary>
        /// <param name="cellInfo">PivotCellInfo</param>
        /// <param name="colIndex">Column Index</param>
        /// <param name="rowIndex">RowIndex</param>
        /// <returns></returns>
        private bool GetPredicate(PivotCellInfo cellInfo, int colIndex, int rowIndex)
        {
            Delegate del = null;
            if (delegateValues != null && delegateValues.Count > 0)
            {
                if (delegateValues.ContainsKey(colIndex))
                {
                    del = delegateValues[colIndex];
                }
            }

            if (del == null)
            {
                Delegate comDelegate = this.GetCompiledDelegate(cellInfo, colIndex, rowIndex);
                if (comDelegate != null)
                {
                    return (bool)comDelegate.DynamicInvoke(new object[] { cellInfo });
                }
                else
                    return false;
            }
            else
            {
                return (bool)del.DynamicInvoke(new object[] { cellInfo });
            }
        }

        /// <summary>
        /// Gets the complied delegate.
        /// </summary>
        /// <param name="cellInfo">PivotCellInfo</param>
        /// <param name="colIndex">Column Index</param>
        /// <param name="rowIndex">RowIndex</param>
        /// <returns>Delegate</returns>
        private Delegate GetCompiledDelegate(PivotCellInfo cellInfo, int colIndex, int rowIndex)
        {
            if (this.NewRuleCollections.Count > 0)
            {
                bool firstLoop = false;
                System.Linq.Expressions.Expression predicate = null;
                Type recordType = cellInfo.GetType();
                ParameterExpression paramExp = recordType.Parameter();
                foreach (var rulesCollections in this.NewRuleCollections)
                {
                    foreach (var condition in rulesCollections.Conditions)
                    {

                        if (!firstLoop)
                        {
                            predicate = Predicate(paramExp, condition.ConditionType, condition);
                            firstLoop = true;
                        }
                        else
                        {
                            if (condition.PredicateType == PredicateType.And)
                            {
                                predicate = predicate.AndAlsoPredicate(Predicate(paramExp, condition.ConditionType, condition));
                            }
                            else if (condition.PredicateType == PredicateType.Or)
                            {
                                predicate = predicate.OrElsePredicate(Predicate(paramExp, condition.ConditionType, condition));
                            }
                        }


                    }
                }
                if (predicate != null)
                {
                    var lambda = System.Linq.Expressions.Expression.Lambda(predicate, paramExp);
                    delegateValues.Add(colIndex, lambda.Compile());
                    return this.delegateValues[colIndex];
                }
                else
                {
                    return null;
                }

            }
            else
                return null;
        }

        /// <summary>
        /// Applying style for supplied GridStyleInfo
        /// </summary>
        /// <param name="styleInfo">GridStyleInfo</param>
        /// <param name="cellStyle">GridStyleInfo</param>
        private void ApplyStyle(GridStyleInfo styleInfo, GridStyleInfo cellStyle)
        {
            if (cellStyle != null)
            {
                styleInfo.BackColor = cellStyle.BackColor;
                styleInfo.Font = cellStyle.Font;
                styleInfo.TextColor = cellStyle.TextColor;               
            }
        }

        /// <summary>
        /// Returns the Predicate based on ConditionType and Parameter Expression
        /// </summary>
        /// <param name="paramExp">The param exp.</param>
        /// <param name="pivotGridDataConditionType">Type of the pivot grid data condition.</param>
        /// <param name="value">The value.</param>
        /// <returns>Expression</returns>
        private System.Linq.Expressions.Expression Predicate(ParameterExpression paramExp, PivotGridDataConditionType pivotGridDataConditionType, ConditionalFormat Condition)
        {
            System.Linq.Expressions.Expression predicate = null;
            switch (pivotGridDataConditionType)
            {
                case PivotGridDataConditionType.Equals:
                    predicate = paramExp.Equal("DoubleValue", Condition.StartValue);
                    break;
                case PivotGridDataConditionType.GreaterThan:
                    predicate = paramExp.GreaterThan("DoubleValue", Condition.StartValue);
                    break;
                case PivotGridDataConditionType.GreaterThanOrEqual:
                    predicate = paramExp.GreaterThanOrEqual("DoubleValue", Condition.StartValue);
                    break;
                case PivotGridDataConditionType.LessThan:
                    predicate = paramExp.LessThan("DoubleValue", Condition.StartValue);
                    break;
                case PivotGridDataConditionType.LessThanOrEqual:
                    predicate = paramExp.LessThanOrEqual("DoubleValue", Condition.StartValue);
                    break;
                case PivotGridDataConditionType.NotEquals:
                    predicate = paramExp.NotEqual("DoubleValue", Condition.StartValue);
                    break;
                case PivotGridDataConditionType.Between:
                    double s_value = 0, e_value = 0;
                    if ((double.Parse(Condition.StartValue.ToString())) <= (double.Parse(Condition.EndValue.ToString())))
                    {
                        s_value = double.Parse(Condition.StartValue.ToString());
                        e_value = double.Parse(Condition.EndValue.ToString());
                    }
                    else if ((double.Parse(Condition.StartValue.ToString())) >= (double.Parse(Condition.EndValue.ToString())))
                    {
                        s_value = double.Parse(Condition.EndValue.ToString());
                        e_value = double.Parse(Condition.StartValue.ToString());
                    }
                    predicate = paramExp.GreaterThanOrEqual("DoubleValue", s_value);
                    if (Condition.PredicateType == PredicateType.And)
                        predicate = predicate.AndAlsoPredicate(paramExp.LessThanOrEqual("DoubleValue", e_value));
                    else
                        predicate = predicate.OrElsePredicate(paramExp.LessThanOrEqual("DoubleValue", e_value));
                    break;
            }
            return predicate;
        }

        /// <summary>
        /// To get the max,min and Average value.
        /// </summary>
        /// <param name="cellInfo">GridStyleInfo</param>
        /// <param name="pivotGrid">PivotGridControlBase</param>
        /// <param name="newRule">NewRuleConditionalFormat</param>
        /// <returns>ArrayList</returns>
        internal ArrayList GetMaxAndMinVale(GridStyleInfo cellInfo, PivotGridControlBase pivotGrid, NewRuleConditionalFormat newRule)
        {
            ArrayList values = new ArrayList();
            double max = 0;
            double min = 0;           
            List<double> lt = new List<double>();
            if (!ApplyOnRanges)
            {
                for (int col = 1; col < pivotGrid.PivotEngine.ColumnCount; col++)
                {
                    for (int row = 1; row < pivotGrid.PivotEngine.RowCount; row++)
                    {
                        string associatedMeasure = GetAssociatedMeasure(pivotGrid, PivotCellInfo, row, col);
                        if (associatedMeasure == newRule.SummaryElement)
                        {
                            PivotCellInfo Info = pivotGrid.PivotEngine[row - 1, col - 1];
                            if (Info != null && Info.CellType == PivotCellType.ValueCell)
                            {
                                double outValue;
                                if (!string.IsNullOrEmpty(Info.FormattedText) && double.TryParse(Info.FormattedText, out outValue))
                                {
                                    lt.Add(outValue);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (int row = newRule.Ranges.Top; row <= newRule.Ranges.Bottom; row++)
                {
                    for (int col = newRule.Ranges.Left; col <= newRule.Ranges.Right; col++)
                    {
                        PivotCellInfo Info = pivotGrid.PivotEngine[row - 1, col - 1];
                        if (Info.CellType == PivotCellType.ValueCell)
                        {
                            double outValue;
                            if (!string.IsNullOrEmpty(Info.FormattedText) && double.TryParse(Info.FormattedText, out outValue))
                            {
                                lt.Add(outValue);
                            }
                        }
                    }
                }
            }
            max = lt.Select(a => a).OrderByDescending(o => 0).Max();
            min = lt.Select(a => a).OrderBy(o => 0).Min();
            double commonValue = double.Parse(max.ToString()) - double.Parse(min.ToString());
            values.Add(max);
            values.Add(min);
            values.Add(commonValue);
            return values;
        }

        /// <summary>
        /// To get the image index 
        /// </summary>
        /// <param name="cellInfo">GridStyleInfo</param>
        /// <param name="maxValues">ArrayList</param>
        /// <param name="newRule">NewRuleConditionalFormat</param>
        /// <returns>Integer</returns>
        internal int GetImageIndex(GridStyleInfo cellInfo, ArrayList maxValues, NewRuleConditionalFormat newRule)
        {
            int imageIndex = 0;
            double percentage1 = 0;
            double percentage2 = 0;
            double percentage3 = 0;
            double percentage4 = 0;            
            if (newRule.ValueType1 == ValueType.Percent)
                percentage1 = FindPercentage(cellInfo, maxValues);
            else if (newRule.ValueType1 == ValueType.Number)
            {
                if (!string.IsNullOrEmpty(cellInfo.Text))
                {
                    double value = 0;
                    if (double.TryParse(cellInfo.Text, out value))
                    {
                        percentage1 = value;
                    }
                }
            }

            if (newRule.ValueType2 == ValueType.Percent)
                percentage2 = FindPercentage(cellInfo, maxValues);
            else if (newRule.ValueType2 == ValueType.Number)
            {
                if (!string.IsNullOrEmpty(cellInfo.Text))
                {
                    double value = 0;
                    if (double.TryParse(cellInfo.Text, out value))
                    {
                        percentage2 = value;
                    }
                }
            }

            if (newRule.ValueType3 == ValueType.Percent)
                percentage3 = FindPercentage(cellInfo, maxValues);
            else if (newRule.ValueType1 == ValueType.Number)
            {
                if (!string.IsNullOrEmpty(cellInfo.Text))
                {
                    double value = 0;
                    if (double.TryParse(cellInfo.Text, out value))
                    {
                        percentage3 = value;
                    }
                }
            }

            if (newRule.ValueType4 == ValueType.Percent)
                percentage4 = FindPercentage(cellInfo, maxValues);
            else if (newRule.ValueType4 == ValueType.Number)
            {
                if (!string.IsNullOrEmpty(cellInfo.Text))
                {
                    double value = 0;
                    if (double.TryParse(cellInfo.Text, out value))
                    {
                        percentage4 = value;
                    }
                }
            }
            switch (newRule.IconImage.ToString())
            {
                case "Three":
                    if (percentage1 < newRule.Value1)
                        imageIndex = 0;
                    else if (percentage1 <= newRule.Value2 && percentage1 >= newRule.Value1)
                        imageIndex = 1;
                    else if (percentage2 >= newRule.Value2)
                        imageIndex = 2;
                    break;
                case "Four":
                    if (percentage1 < newRule.Value1)
                        imageIndex = 0;
                    else if (percentage1 <= newRule.Value2 && percentage1 >= newRule.Value1)
                        imageIndex = 1;
                    else if (percentage2 <= newRule.Value3 && percentage2 >= newRule.Value2)
                        imageIndex = 2;
                    else if (percentage3 >= newRule.Value3)
                        imageIndex = 3;
                    break;
                case "Five":
                    if (percentage1 < newRule.Value1)
                        imageIndex = 0;
                    else if (percentage1 <= newRule.Value2 && percentage1 >= newRule.Value1)
                        imageIndex = 1;
                    else if (percentage2 <= newRule.Value3 && percentage2 >= newRule.Value2)
                        imageIndex = 2;
                    else if (percentage3 <= newRule.Value4 && percentage3 >= newRule.Value3)
                        imageIndex = 3;
                    else if (percentage4 >= newRule.Value4)
                        imageIndex = 4;
                    break;
            }
            return imageIndex;
        }

        /// <summary>
        /// To calculate the percentage of the values in PivotGrid.
        /// </summary>
        /// <param name="cellInfo">GridStyleInfo</param>
        /// <param name="maxValues">ArrayList</param>
        /// <returns>Double Value</returns>
        internal double FindPercentage(GridStyleInfo cellInfo, ArrayList maxValues)
        {
            double percentage = 0;
            double commonValue = 0;
            double difference = double.Parse(maxValues[0].ToString()) / double.Parse(maxValues[1].ToString());
            commonValue = (double.Parse(cellInfo.Text) / double.Parse(maxValues[1].ToString()));
            percentage = commonValue / difference;
            return percentage * 100;
        }
        #endregion
    }
    #endregion

    #region Enum
    /// <summary>
    /// To apply Type of the Rule.
    /// </summary>
    public enum RuleType
    {
        FormatAllCellsBasedOnTheirValues = 0,
        FormatOnlyCellsThatContain = 1,
        FormatTopOrBottomRankedValues = 2,
        FormatOnlyValuesThatAreAboveOrBelowAverage = 3,
        FormatOnlyUniqueOrDuplicateValues = 4
    }

    /// <summary>
    /// To apply Type of the Format.
    /// </summary>
    public enum FormatAllType
    {
        Unique,
        Duplicate
    }

    /// <summary>
    /// To apply Type of the Selected Average Value.
    /// </summary>
    public enum FormateSelectedAverageValueType
    {
        Above,
        Below,
        EqualOrAbove,
        EqualOrBelow
    }

    /// <summary>
    /// To apply the Format of the style.
    /// </summary>
    public enum FormatStyle
    {
        IconSets = 0,        
    }

    /// <summary>
    /// To apply the Icon Image category
    /// </summary>
    public enum IconImageCategory
    {
        Three = 0,
        Four = 1,
        Five = 2
    }

    /// <summary>
    /// To apply the conditional based on the value type.
    /// </summary>
    public enum ValueType
    {
        Percent,        
        Number        
    }

    /// <summary>
    ///  To find the values in cell based on the Format type.
    /// </summary>
    public enum FormateValueWithCell
    {
        CellValue
    }

    /// <summary>
    /// Format of the Rank type.
    /// </summary>
    public enum FormateValuesRankType
    {
        Top,
        Bottom
    }

    public enum BitMapImageName
    {
        ThreeArrowsColored,
        ThreeArrowsGray,
        ThreeFlags,
        ThreeTrafficLightsUnrimmed,
        ThreeTrafficLightsRimmed,
        ThreeSigns,
        ThreeSymbols,
        ThreeSymbolsCircled,
        ThreeSymbolsUnCircled,
        ThreeStars,
        ThreeTriangle,
        FourArrowsColored,
        FourArrowsGray,
        RedToBlack,
        FourRating,
        FourTrafficLight,
        FiveArrowsColored,
        FiveArrowsGray,
        FiveQuarters,
        FiveRatings,
        FiveBoxes
    }

    /// <summary>
    /// Predicate type defined for Filters.
    /// </summary>        
    public enum PredicateType
    {
        // Summary:
        //     Does an AND operation on filters.
        And = 0,
        //
        // Summary:
        //     Does an OR operation on filters.
        Or = 1,
    }

    /// <summary>
    /// Conditional format type for operation.
    /// </summary>
    public enum PivotGridDataConditionType
    {
        /// <summary>
        /// Performs an Equals operation on the operands.
        /// </summary>
        Equals,
        /// <summary>
        /// Performs a NotEquals operation on the operands.
        /// </summary>
        NotEquals,
        /// <summary>
        /// Performs a LessThan operation on the operands.
        /// </summary>
        LessThan,
        /// <summary>
        /// Performs a LessThanOrEqual operation on the operands.
        /// </summary>
        LessThanOrEqual,
        /// <summary>
        /// Performs a GreatherThan operation on the operands.
        /// </summary>
        GreaterThan,
        /// <summary>
        /// Performs a GreaterThanOrEqual operation on the operands.
        /// </summary>
        GreaterThanOrEqual,
        /// <summary>
        /// Performs a Between operation on the operands.
        /// </summary>
        Between
    }
#endregion 
}
