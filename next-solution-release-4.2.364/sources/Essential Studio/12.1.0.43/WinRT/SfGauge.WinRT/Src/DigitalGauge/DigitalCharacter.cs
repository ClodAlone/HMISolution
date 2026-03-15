#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using System.Threading.Tasks;
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
    ///  It is a control, which represents the character in the digital segments.
    /// </summary>
    public class DigitalCharacter : Control
    {
        #region Constructor

        public DigitalCharacter()
        {
            DefaultStyleKey = typeof(DigitalCharacter);
        }

        #endregion

        #region Dependency Properties

        internal List<Brush> ListSegments
        {
            get { return (List<Brush>)GetValue(ListSegmentsProperty); }
            set { SetValue(ListSegmentsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ListSegments.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ListSegmentsProperty =
            DependencyProperty.Register("ListSegments", typeof(List<Brush>), typeof(DigitalCharacter), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the value that digital gauge should display.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="object"/>
        /// </value>
        internal char Value
        {
            get { return (char)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(char), typeof(DigitalCharacter), new PropertyMetadata(new char()));

        /// <summary>
        /// Gets or sets the value indicating whether character should 
        /// contain seven or fourteen segments. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="CharacterType"/>
        /// Default value is CharacterType.SegmentSeven.
        /// </value>
        internal CharacterType CharacterType
        {
            get { return (CharacterType)GetValue(CharacterTypeProperty); }
            set { SetValue(CharacterTypeProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="CharacterType"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty CharacterTypeProperty =
            DependencyProperty.Register("CharacterType", typeof(CharacterType), typeof(DigitalCharacter), new PropertyMetadata(CharacterType.SegmentSeven));

        /// <summary>
        /// Gets or sets the width of the segments.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 2.
        /// </value>
        internal double SegmentThickness
        {
            get { return (double)GetValue(SegmentThicknessProperty); }
            set { SetValue(SegmentThicknessProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="SegmentThickness"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty SegmentThicknessProperty =
            DependencyProperty.Register("SegmentThickness", typeof(double), typeof(DigitalCharacter), new PropertyMetadata(2d));

        #endregion

        #region Implementation
#if WINRT
        protected override void OnApplyTemplate()        
#else
        public override void OnApplyTemplate()
#endif
        {
            if (CharacterType == CharacterType.SegmentSeven)
            {
                LoadSevenSegments();
            }
            else if (CharacterType == CharacterType.SegmentFourteen)
            {
                LoadFourteenSegments();
            }
            else if (CharacterType == CharacterType.SegmentSixteen)
            {
                LoadSixteenSegments();
            }
            else if (CharacterType == CharacterType.EightCrossEightDotMatrix)
            {
                LoadEightMatrix();
            }
        }

        #region Segment Methods

        internal void LoadSevenSegments()
        {
            var parentgauge = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this)) as SfDigitalGauge;
            if (parentgauge != null)
            {
                if (parentgauge.dictionaryLoader.sevenSegmentDictionary.ContainsKey(Value))
                {
                    ListSegments = parentgauge.dictionaryLoader.sevenSegmentDictionary[Value];
                }
            }

        }

        internal void LoadFourteenSegments()
        {
            var parentgauge = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this)) as SfDigitalGauge;
            if (parentgauge != null)
            {
                if (parentgauge.dictionaryLoader.fourteenSegmentDictionary.ContainsKey(Value))
                {
                    ListSegments = parentgauge.dictionaryLoader.fourteenSegmentDictionary[Value];
                }
            }
        }

        internal void LoadSixteenSegments()
        {
            var parentgauge = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this)) as SfDigitalGauge;
            if (parentgauge != null)
            {
                if (parentgauge.dictionaryLoader.sixteenSegmentDictionary.ContainsKey(Value))
                {
                    ListSegments = parentgauge.dictionaryLoader.sixteenSegmentDictionary[Value];
                }
            }
        }

        internal void LoadEightMatrix()
        {
            var parentgauge = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this)) as SfDigitalGauge;
            if (parentgauge != null)
            {
                if (parentgauge.dictionaryLoader.eightMatrixDictionary.ContainsKey(Value))
                {
                    ListSegments = parentgauge.dictionaryLoader.eightMatrixDictionary[Value];
                }
            }
        }


        #endregion

        #endregion

    }

}
