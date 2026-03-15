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
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace Syncfusion.Windows.Forms.TreeMap
{
    #region ColorMapping

    public class ColorMapping 
    {
        #region Implementation

        public virtual void EvaluateColorMapping(List<TreeMapLeafNode> leafNodes)
        {
            ColorMapping colorMapping = this;
            int itemsCount = leafNodes.Count;

            switch (GetType().Name)
            {
                case "UniColorMapping":
                    (colorMapping as UniColorMapping).SetUniColor(leafNodes);
                    break;

                case "RangeBrushColorMapping":
                    (colorMapping as RangeBrushColorMapping).SetRangeBrushColor(leafNodes);
                    break;

                case "DesaturationColorMapping":
                    (colorMapping as DesaturationColorMapping).SetDesaturatedColor(leafNodes);
                    break;

                case "PaletteColorMapping":
                    (colorMapping as PaletteColorMapping).SetPaletteColor(leafNodes);
                    break;

                default:
                    Color defaultColor = Color.FromArgb(255, 174, 217, 96);
                    for (int i = itemsCount - 1; i >= 0; i--)
                        leafNodes[i].MappedColor = new SolidBrush(defaultColor);
                    break;
            }
        }

        public virtual void EvaluateColorMapping(List<TreeMapItem> treeMapItems)
        {
            ColorMapping colorMapping = this;
            int itemsCount = treeMapItems.Count;

            switch (GetType().Name)
            {
                case "UniColorMapping":
                    (colorMapping as UniColorMapping).SetUniColor(treeMapItems);
                    break;

                case "RangeBrushColorMapping":
                    (colorMapping as RangeBrushColorMapping).SetRangeBrushColor(treeMapItems);
                     break;

                case "DesaturationColorMapping":
                    (colorMapping as DesaturationColorMapping).SetDesaturatedColor(treeMapItems);
                    break;

                case "PaletteColorMapping":
                    (colorMapping as PaletteColorMapping).SetPaletteColor(treeMapItems);
                    break;

                default:
                    Color defaultColor = Color.FromArgb(255, 119, 216, 216);

                    for (int i = itemsCount - 1; i >= 0; i--)
                        treeMapItems[i].HeaderColor = new SolidBrush(defaultColor);
                    break;
            }
        }

        public void SetColor(TreeMapLeafNode item, Brush brush)
        {
            item.MappedColor = brush;
        }

        #endregion
    }

    #endregion

    #region RangeBrushColorMapping

    public class RangeBrushColorMapping : ColorMapping
    {
        #region Constructor

        public RangeBrushColorMapping()
        {
            Brushes = new List<RangeBrush>();
        }

        #endregion

        #region  Properties

        #region Brushes

        private List<RangeBrush> brushes;
        public List<RangeBrush> Brushes
        {
            get { return brushes; }
            set { brushes = value; }
        }

        
        #endregion

        #endregion





        #region Implementation

        internal void SetRangeBrushColor(List<TreeMapLeafNode> leafNodes)
        {
            int itemsCount = leafNodes.Count;
            

            if (Brushes != null)
                for (int i = itemsCount - 1; i >= 0; i--)
                {
                    foreach (RangeBrush rangeBrush in Brushes)
                    {
                        if (leafNodes[i].ColorWeight >= rangeBrush.From && leafNodes[i].ColorWeight <= rangeBrush.To)
                        {
                            leafNodes[i].MappedColor = new SolidBrush(rangeBrush.Color);
                            break;
                        }
                    }
                }
        }

        internal void SetRangeBrushColor(List<TreeMapItem> leafNodes)
        {
            int itemsCount = leafNodes.Count;
            if (Brushes != null)
                for (int i = itemsCount - 1; i >= 0; i--)
                {
                    foreach (RangeBrush rangeBrush in Brushes)
                    {
                        if (leafNodes[i].ColorWeight >= rangeBrush.From && leafNodes[i].ColorWeight <= rangeBrush.To)
                        {                           
                            leafNodes[i].HeaderColor = new SolidBrush(rangeBrush.Color);
                            break;
                        }                                               
                    }
                }
        }

        #endregion
    }

    #endregion

    #region UniColorMapping

    public class UniColorMapping : ColorMapping
    {
        #region  Properties

        #region Brush

        private Color color;
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }


        #endregion

        #endregion

        #region Implementation

        internal void SetUniColor(List<TreeMapLeafNode> leafNodes)
        {
            int itemsCount = leafNodes.Count;
            for (int i = itemsCount - 1; i >= 0; i--)
                leafNodes[i].MappedColor = new SolidBrush(Color);
        }

        internal void SetUniColor(List<TreeMapItem> leafNodes)
        {
            int itemsCount = leafNodes.Count;
            for (int i = itemsCount - 1; i >= 0; i--)
                leafNodes[i].HeaderColor = new SolidBrush(Color);
        }

        #endregion
    }

    #endregion

    #region PaletteColorMapping

    public class PaletteColorMapping : ColorMapping
    {
        #region Constructor

        public PaletteColorMapping()
        {
            Colors = new List<Brush>();
        }

        #endregion

        #region  Properties

        #region Colors

        private List<Brush> colors;
        public List<Brush> Colors
        {
            get { return colors; }
            set { colors = value; }
        }

        #endregion

        #endregion

        #region Implementation

        internal void SetPaletteColor(List<TreeMapLeafNode> list)
        {
            list = new List<TreeMapLeafNode>(list.OrderByDescending(x => x.ColorWeight));

            for (int i = 0; i < list.Count; i++)
            {
                if (Colors.Count > 0)
                {
                    list[i].MappedColor = Colors[i % (Colors.Count)];
                }
            }
        }

        internal void SetPaletteColor(List<TreeMapItem> list)
        {
            list = new List<TreeMapItem>(list.OrderByDescending(x => x.ColorWeight));

            for (int i = 0; i < list.Count; i++)
            {
                if (Colors.Count > 0)
                {
                    list[i].HeaderColor = Colors[i % (Colors.Count)];
                }
            }
        }

        #endregion
    }

    #endregion

    #region DesaturationColorMapping

    public class DesaturationColorMapping : ColorMapping
    {
        #region  Properties

        #region Color

        private Color color;
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        #endregion

        #region From


        private double from;
        public double From
        {
            get { return from; }
            set { from = value; }
        }
        #endregion

        #region To

        private double to;
        public double To
        {
            get { return to; ; }
            set { to = value; }
        }

        #endregion

        #region RangeMinimum

        private double rangeMinimum;
        public double RangeMinimum
        {
            get { return rangeMinimum; }
            set { rangeMinimum = value; }
        }

        #endregion

        #region RangeMaximum

        private double rangeMaximum;
        public double RangeMaximum
        {
            get { return rangeMaximum; }
            set { rangeMaximum = value; }
        }
               
        #endregion

        #endregion

        #region Implementation

        internal void SetDesaturatedColor(List<TreeMapLeafNode> list)
        {
            list = new List<TreeMapLeafNode>(list.OrderByDescending(x => x.ColorWeight));

            double from = From;
            double to = To;
            if (@from < 0 || @from > 1)
                @from = 255;
            if (to > 255 || to < 0)
                to = 0;

            double interval = (@from - to) / list.Count;
            for (int i = 0; i < list.Count; i++)
            {
                //list[i].MappedColor = new SolidBrush(Color);

                if (list[i].ColorWeight >= RangeMinimum && list[i].ColorWeight <= RangeMaximum)
                {
                    list[i].MappedColor = new SolidBrush(Color.FromArgb((int)@from,Color));
                    //list[i].MappedColor.Opacity = @from;
                    @from = @from - interval;
                }
            }
        }

        internal void SetDesaturatedColor(List<TreeMapItem> list)
        {
            list = new List<TreeMapItem>(list.OrderByDescending(x => x.ColorWeight));

            double from = From;
            double to = To;
            if (@from < 0 || @from > 255)
                @from = 255;
            if (to > 255 || to < 0)
                to = 0;

            double interval = (@from - to) / list.Count;
            
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ColorWeight >= RangeMinimum && list[i].ColorWeight <= RangeMaximum)
                {
                    list[i].HeaderColor = new SolidBrush(Color.FromArgb((int)@from, Color));
                    //list[i].HeaderColor.Opacity = @from;
                    @from = @from - interval;
                }
               
            }
        }

        #endregion
    }

    #endregion

    #region RangeBrush

    public class RangeBrush 
    {
        #region  Properties

        #region Color

        private Color color;
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        #endregion

        #region From


        private double from;
        public double From
        {
            get { return from; }
            set { from = value; }
        }
        #endregion

        #region To

        private double to;
        public double To
        {
            get { return to; ; }
            set { to = value; }
        }

        #endregion

        #region LegendLabel
        private string legendLabel;
        public string LegendLabel
        {
            get { return legendLabel; }
            set { legendLabel = value; }
        }

        #endregion

        #endregion
    }

    #endregion
}
