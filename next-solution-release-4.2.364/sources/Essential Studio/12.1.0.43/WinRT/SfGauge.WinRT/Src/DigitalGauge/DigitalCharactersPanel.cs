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
using System.Linq;
using System.Text;

#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using System.Threading.Tasks;
using Windows.UI;
#else
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
#endif

namespace Syncfusion.UI.Xaml.Gauges
{
    /// <summary>
    ///  The digital characters are arranged in this panel
    /// </summary>
    /// <remarks>
    /// It also supports right-to-left format.
    /// </remarks>
    public class DigitalCharactersPanel : Panel
    {

        #region ResourceDictionary Declaration
      
        internal static ResourceDictionary resourcedictionary;
        internal static ControlTemplate sevensegment;
        internal static ControlTemplate fourteensegment;
        internal static ControlTemplate sixteensegment;
        internal static ControlTemplate eightmatrix;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalCharactersPanel"/> class.
        /// </summary>
        public DigitalCharactersPanel()
        {
            #if WINRT
            resourcedictionary = new ResourceDictionary() { Source = new Uri("ms-appx:///Syncfusion.SfGauge.WinRT/DigitalGauge/Themes/DigitalGauge.xaml", UriKind.RelativeOrAbsolute) };
            #elif WINDOWSPHONE_8
            resourcedictionary = new ResourceDictionary() { Source = new Uri("/Syncfusion.SfGauge.WP8;component/DigitalGauge/Themes/DigitalGauge.xaml", UriKind.Relative) };
            #elif WINDOWSPHONE_7
            resourcedictionary = new ResourceDictionary() { Source = new Uri("/Syncfusion.SfGauge.WP7;component/DigitalGauge/Themes/DigitalGauge.xaml", UriKind.Relative) };
            #elif SILVERLIGHT
            resourcedictionary = new ResourceDictionary() { Source = new Uri("/Syncfusion.SfGauge.Silverlight;component/DigitalGauge/Themes/DigitalGauge.xaml", UriKind.Relative) };
            #else
            resourcedictionary = new ResourceDictionary() { Source = new Uri("/Syncfusion.SfGauge.WPF;component/DigitalGauge/Themes/DigitalGauge.xaml", UriKind.Relative) };
            #endif
            sevensegment = resourcedictionary["SevenSegment"] as ControlTemplate;
            fourteensegment = resourcedictionary["FourteenSegment"] as ControlTemplate;
            sixteensegment = resourcedictionary["SixteenSegment"] as ControlTemplate;
            eightmatrix = resourcedictionary["EightMatrix"] as ControlTemplate;

        }

        #endregion

        #region Dependency Properties


        public bool EnableRTLFormat
        {
            get { return (bool)GetValue(EnableRTLFormatProperty); }
            internal set { SetValue(EnableRTLFormatProperty, value); }
        }
        // Using a DependencyProperty as the backing store for EnableRTLFormat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableRTLFormatProperty =
            DependencyProperty.Register("EnableRTLFormat", typeof(bool), typeof(DigitalCharactersPanel), new PropertyMetadata(false, OnEnableRTLFormatdChanged));
        
        private static void OnEnableRTLFormatdChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is DigitalCharactersPanel)
            {
                DigitalCharactersPanel panel = obj as DigitalCharactersPanel;
                panel.InvalidateMeasure();
                panel.InvalidateArrange();
            }
        }


        private string OldValue { get; set; }

        private string NewValue { get; set; }


        public string Values
        {
            get { return (string)GetValue(ValuesProperty); }
            internal set { SetValue(ValuesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Values.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValuesProperty =
            DependencyProperty.Register("Values", typeof(string), typeof(DigitalCharactersPanel), new PropertyMetadata(string.Empty, OnValuesChanged));

        private static void OnValuesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is DigitalCharactersPanel)
            {               
                DigitalCharactersPanel panel = obj as DigitalCharactersPanel;

                panel.OldValue = args.OldValue.ToString();

                panel.NewValue = args.NewValue.ToString();

                panel.InvalidateMeasure();
                panel.InvalidateArrange();
            }
        }





        public CharacterType CharacterType
        {
            get { return (CharacterType)GetValue(CharacterTypeProperty); }
            internal set { SetValue(CharacterTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CharacterType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CharacterTypeProperty =
            DependencyProperty.Register("CharacterType", typeof(CharacterType), typeof(DigitalCharactersPanel), new PropertyMetadata(CharacterType.SegmentSeven, OnCharacterTypeChanged));

        private static void OnCharacterTypeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is DigitalCharactersPanel)
            {
                DigitalCharactersPanel panel = obj as DigitalCharactersPanel;
                panel.InvalidateMeasure();
                panel.InvalidateArrange();
            }
        }







        //internal int CharacterCount
        //{
        //    get { return (int)GetValue(CharacterCountProperty); }
        //    set { SetValue(CharacterCountProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for CharacterCount.  This enables animation, styling, binding, etc...
        //internal static readonly DependencyProperty CharacterCountProperty =
        //    DependencyProperty.Register("CharacterCount", typeof(int), typeof(DigitalCharactersPanel), new PropertyMetadata(0));






        public double CharacterHeight
        {
            get
            {
                  return (double)GetValue(CharacterHeightProperty);
                
            }
            internal set { SetValue(CharacterHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CharacterHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CharacterHeightProperty =
            DependencyProperty.Register("CharacterHeight", typeof(double), typeof(DigitalCharactersPanel), new PropertyMetadata(30d,OnCharacterHeightChanged));

        private static void OnCharacterHeightChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is DigitalCharactersPanel)
            {
                DigitalCharactersPanel panel = obj as DigitalCharactersPanel;
                    panel.InvalidateMeasure();
                    panel.InvalidateArrange();
            }
        }





        public double CharacterWidth
        {
            get { return (double)GetValue(CharacterWidthProperty); }
            internal set { SetValue(CharacterWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CharacterWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CharacterWidthProperty =
            DependencyProperty.Register("CharacterWidth", typeof(double), typeof(DigitalCharactersPanel), new PropertyMetadata(30d,OnCharacterWidthChanged));

        private static void OnCharacterWidthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is DigitalCharactersPanel)
            {
                DigitalCharactersPanel panel = obj as DigitalCharactersPanel;

                panel.InvalidateMeasure();
                panel.InvalidateArrange();
            }
        }






        public double CharacterSpacing
        {
            get { return (double)GetValue(CharacterSpacingProperty); }
            internal set { SetValue(CharacterSpacingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CharacterSpacing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CharacterSpacingProperty =
            DependencyProperty.Register("CharacterSpacing", typeof(double), typeof(DigitalCharactersPanel), new PropertyMetadata(10d, OnCharacterSpacingChanged));

        private static void OnCharacterSpacingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is DigitalCharactersPanel)
            {
                DigitalCharactersPanel panel = obj as DigitalCharactersPanel;
                    panel.InvalidateMeasure();
                    panel.InvalidateArrange();
                
            }
        }





        public Brush CharacterStroke
        {
            get { return (Brush)GetValue(CharacterStrokeProperty); }
            internal set { SetValue(CharacterStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CharacterStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CharacterStrokeProperty =
            DependencyProperty.Register("CharacterStroke", typeof(Brush), typeof(DigitalCharactersPanel), new PropertyMetadata(new SolidColorBrush(Colors.Red), OnCharacterStrokeChanged));

        private static void OnCharacterStrokeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is DigitalCharactersPanel)
            {
                DigitalCharactersPanel panel = obj as DigitalCharactersPanel;
                panel.InvalidateMeasure();
                panel.InvalidateArrange();
            }
        }







        public Brush DimmedBrush
        {
            get { return (Brush)GetValue(DimmedBrushProperty); }
            internal set { SetValue(DimmedBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DimmedBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DimmedBrushProperty =
            DependencyProperty.Register("DimmedBrush", typeof(Brush), typeof(DigitalCharactersPanel), new PropertyMetadata(new SolidColorBrush(Colors.Transparent), OnDimmedBrushChanged));

        private static void OnDimmedBrushChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is DigitalCharactersPanel)
            {
                DigitalCharactersPanel panel = obj as DigitalCharactersPanel;
                panel.InvalidateMeasure();
                panel.InvalidateArrange();
            }
        }







        public double DimmedBrushOpacity
        {
            get { return (double)GetValue(DimmedBrushOpacityProperty); }
            internal set { SetValue(DimmedBrushOpacityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DimmedBrushOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DimmedBrushOpacityProperty =
            DependencyProperty.Register("DimmedBrushOpacity", typeof(double), typeof(DigitalCharactersPanel), new PropertyMetadata(50d, OnDimmedBrushOpacityChanged));

        private static void OnDimmedBrushOpacityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is DigitalCharactersPanel)
            {
                DigitalCharactersPanel panel = obj as DigitalCharactersPanel;
                panel.InvalidateMeasure();
                panel.InvalidateArrange();
            }

        }






        public double SegmentThickness
        {
            get { return (double)GetValue(SegmentThicknessProperty); }
            internal set { SetValue(SegmentThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SegmentThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SegmentThicknessProperty =
            DependencyProperty.Register("SegmentThickness", typeof(double), typeof(DigitalCharactersPanel), new PropertyMetadata(2d,OnSegmentThicknessChanged));

        private static void OnSegmentThicknessChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is DigitalCharactersPanel)
            {
                DigitalCharactersPanel panel = obj as DigitalCharactersPanel;
                panel.InvalidateMeasure();
                panel.InvalidateArrange();
            }
        }






        public double SkewAngleX
        {
            get { return (double)GetValue(SkewAngleXProperty); }
            internal set { SetValue(SkewAngleXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SkewAngleX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SkewAngleXProperty = 
            DependencyProperty.Register("SkewAngleX", typeof(double), typeof(DigitalCharactersPanel), new PropertyMetadata(0d,OnSkewAngleXChanged));

        private static void OnSkewAngleXChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is DigitalCharactersPanel)
            {
                DigitalCharactersPanel panel = obj as DigitalCharactersPanel;
                panel.InvalidateMeasure();
                panel.InvalidateArrange();
            }
        }





        public double SkewAngleY
        {
            get { return (double)GetValue(SkewAngleYProperty); }
            internal set { SetValue(SkewAngleYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SkewAngleY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SkewAngleYProperty = 
            DependencyProperty.Register("SkewAngleY", typeof(double), typeof(DigitalCharactersPanel), new PropertyMetadata(0d,OnSkewAngleYChanged));

        private static void OnSkewAngleYChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is DigitalCharactersPanel)
            {
                DigitalCharactersPanel panel = obj as DigitalCharactersPanel;
                panel.InvalidateMeasure();
                panel.InvalidateArrange();
            }
        }

     



        #endregion

        #region Measure Override

        protected override Size MeasureOverride(Size availableSize)
        {

       
            #region Template Changed Baased on CharacterType
            DigitalCharacter DigiChar = new DigitalCharacter();
            if (this.CharacterType == Gauges.CharacterType.EightCrossEightDotMatrix)
            {
                DigiChar.Template = eightmatrix;
            }
            else if (this.CharacterType == Gauges.CharacterType.SegmentSixteen)
            {
                DigiChar.Template = sixteensegment;
            }
            else if (this.CharacterType == Gauges.CharacterType.SegmentFourteen)
            {
                DigiChar.Template = fourteensegment;
            }
            else
            {
                DigiChar.Template = sevensegment;
            }
            #endregion

            if (Values != string.Empty )
            {
                if (this.CharacterHeight >= 0 && this.CharacterWidth >= 0 && this.SegmentThickness >= 0)
                {
                    #region Validation

                    this.NewValue = this.Values;
                    if (OldValue == NewValue)
                    {
                        #region OldValue and New Value  same.

                        this.Children.Clear();
                        foreach (char item in NewValue)
                        {
                            UIElement child = new DigitalCharacter()
                            {
                                Value = item,
                                Height = this.CharacterHeight,
                                Width = this.CharacterWidth,
                                CharacterType = this.CharacterType,
                                SegmentThickness = this.SegmentThickness,
                                RenderTransform = new SkewTransform() { AngleX = this.SkewAngleX, AngleY = this.SkewAngleY },
                                Template = DigiChar.Template,
                            };

                            this.Children.Add(child);
                            child.Measure(availableSize);

                        }


                        #endregion
                    }
                    else
                    {
                        #region OldValue and New Value are not same
                        if ((OldValue.Length == (NewValue.Length + 1)) || OldValue.Length == (NewValue.Length - 1))
                        {
                            if (OldValue.Contains(NewValue) || NewValue.Contains(OldValue))
                            {
                                #region Either added or deleting character from the old value.

                                if (NewValue.Length > OldValue.Length)
                                {
                                    #region Adding a character


                                    if (OldValue != string.Empty)
                                    {
                                        #region OldValue Not equal to null

                                        if (NewValue.Contains(OldValue))
                                        {

                                            int startindexofoldvalue = NewValue.IndexOf(OldValue);
                                            int endindexofoldvalue = startindexofoldvalue + OldValue.Length;


                                            if (NewValue.Length > endindexofoldvalue)
                                            {
                                                #region String added at the end of the old value.

                                                for (int i = endindexofoldvalue; i < NewValue.Length; i++)
                                                {

                                                    UIElement child = new DigitalCharacter()
                                                    {
                                                        Value = NewValue[i],
                                                        Height = this.CharacterHeight,
                                                        Width = this.CharacterWidth,
                                                        CharacterType = this.CharacterType,
                                                        SegmentThickness = this.SegmentThickness,
                                                        RenderTransform = new SkewTransform() { AngleX = this.SkewAngleX, AngleY = this.SkewAngleY },
                                                        Template = DigiChar.Template,
                                                    };

                                                    this.Children.Insert(i, child);
                                                    child.Measure(availableSize);
                                                }
                                                #endregion
                                            }
                                            else if (startindexofoldvalue > 0)
                                            {
                                                #region String added at the begining of the old value.

                                                for (int i = 0; i < startindexofoldvalue; i++)
                                                {

                                                    UIElement child = new DigitalCharacter()
                                                    {
                                                        Value = NewValue[i],
                                                        Height = this.CharacterHeight,
                                                        Width = this.CharacterWidth,
                                                        CharacterType = this.CharacterType,
                                                        SegmentThickness = this.SegmentThickness,
                                                        RenderTransform = new SkewTransform() { AngleX = this.SkewAngleX, AngleY = this.SkewAngleY },
                                                        Template = DigiChar.Template,
                                                    };

                                                    this.Children.Insert(i, child);
                                                    child.Measure(availableSize);
                                                }
                                                #endregion
                                            }
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region OldValue  equal to null
                                        this.Children.Clear();
                                        foreach (char item in NewValue)
                                        {
                                            UIElement child = new DigitalCharacter()
                                            {
                                                Value = item,
                                                Height = this.CharacterHeight,
                                                Width = this.CharacterWidth,
                                                CharacterType = this.CharacterType,
                                                SegmentThickness = this.SegmentThickness,
                                                RenderTransform = new SkewTransform() { AngleX = this.SkewAngleX, AngleY = this.SkewAngleY },
                                                Template = DigiChar.Template,
                                            };

                                            this.Children.Add(child);
                                            child.Measure(availableSize);

                                        }
                                        #endregion
                                    }

                                    #endregion
                                }
                                else
                                {
                                    #region Deleting a Character

                                    if (OldValue.Contains(NewValue))
                                    {
                                        int startIndexOfNewValue = OldValue.IndexOf(NewValue);
                                        int endIndexOfOldValue = startIndexOfNewValue + NewValue.Length;

                                        if (OldValue.Length > endIndexOfOldValue)
                                        {
                                            #region Character deleted at the end of the oldvalue.

                                            for (int i = endIndexOfOldValue; i <= NewValue.Length; i++)
                                            {
                                                this.Children.RemoveAt(i);
                                            }
                                            #endregion
                                        }

                                        else if (startIndexOfNewValue > 0)
                                        {
                                            #region Character deleted at the begining of the old value.

                                            for (int i = 0; i < startIndexOfNewValue; i++)
                                            {
                                                this.Children.RemoveAt(i);
                                            }
                                            #endregion
                                        }
                                    }

                                    #endregion
                                }
                                #endregion
                            }
                            else
                            {
                                #region replace the character or edit at the middle.
                                this.Children.Clear();
                                foreach (char item in NewValue)
                                {
                                    UIElement child = new DigitalCharacter()
                                    {
                                        Value = item,
                                        Height = this.CharacterHeight,
                                        Width = this.CharacterWidth,
                                        CharacterType = this.CharacterType,
                                        SegmentThickness = this.SegmentThickness,
                                        RenderTransform = new SkewTransform() { AngleX = this.SkewAngleX, AngleY = this.SkewAngleY },
                                        Template = DigiChar.Template,
                                    };

                                    this.Children.Add(child);
                                    child.Measure(availableSize);
                                #endregion

                                }

                            }
                        }
                        else
                        {
                            this.Children.Clear();
                            foreach (char item in NewValue)
                            {
                                UIElement child = new DigitalCharacter()
                                {
                                    Value = item,
                                    Height = this.CharacterHeight,
                                    Width = this.CharacterWidth,
                                    CharacterType = this.CharacterType,
                                    SegmentThickness = this.SegmentThickness,
                                    RenderTransform = new SkewTransform() { AngleX = this.SkewAngleX, AngleY = this.SkewAngleY },
                                    Template = DigiChar.Template,
                                };

                                this.Children.Add(child);
                                child.Measure(availableSize);

                            }

                        }

                        #endregion

                    }
                    this.OldValue = this.NewValue;

                    #endregion
                }
                else
                {
                    this.Children.Clear();

                    //set 1 when segmentThichness is set in -ve

                //    DigitalCharacter Digit = new DigitalCharacter();
                //    if (this.SegmentThickness < 0)
                //    {
                //        Digit.SegmentThickness = 1;
                //    }
                //    else
                //    {
                //        Digit.SegmentThickness = this.SegmentThickness;
                //    }

                //    this.Children.Clear();
                //    if (this.CharacterHeight >= 0 && this.CharacterWidth >= 0)
                //    {
                //        foreach (char item in NewValue)
                //        {
                //            UIElement child = new DigitalCharacter()
                //            {
                //                Value = item,
                //                SegmentThickness = Digit.SegmentThickness,
                //                Height = this.CharacterHeight,
                //                Width = this.CharacterWidth,
                //                CharacterType = this.CharacterType,
                //                RenderTransform = new SkewTransform() { AngleX = this.SkewAngleX, AngleY = this.SkewAngleY },
                //                Template = DigiChar.Template,
                //            };
                //            this.Children.Add(child);
                //            child.Measure(availableSize);
                //        } 
                //    }
                }
               
            }
            else
            {
                this.Children.Clear();
            }
            return  double.IsPositiveInfinity(availableSize.Height) || double.IsPositiveInfinity(availableSize.Width) || double.IsNaN(availableSize.Height) || double.IsPositiveInfinity(availableSize.Width) ? new Size(0, 0) : availableSize;

        }

        #endregion
            
        #region Arrange Override


        protected override Size ArrangeOverride(Size finalSize)
        {
            double allottedWidth = 0d;
            double totalWidth = finalSize.Width;
            if (Values != null)
            {
                if (this.CharacterHeight >= 0 && this.CharacterWidth >= 0 )
                {
                    foreach (UIElement item in Children)
                    {
                        DigitalCharacter digitalCharacter = (DigitalCharacter)item;
                        if (digitalCharacter != null)
                        {
                            if (EnableRTLFormat == true)
                                item.Arrange(new Rect(totalWidth - allottedWidth, 0, CharacterWidth, CharacterHeight));
                            else
                                item.Arrange(new Rect(allottedWidth, 0, CharacterWidth, CharacterHeight));
                            allottedWidth += CharacterWidth;
                            if (CharacterSpacing < 0)
                                allottedWidth++;
                            else
                            allottedWidth += CharacterSpacing;
                        }
                    }
                }
            }

            return finalSize;
        }



        #endregion
              
    }
}
