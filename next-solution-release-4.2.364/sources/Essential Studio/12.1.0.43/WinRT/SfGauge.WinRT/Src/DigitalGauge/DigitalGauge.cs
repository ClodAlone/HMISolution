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
using System.Collections.Specialized;
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
    ///  Represents the control which helps the user to visualize the data in digital
    /// character
    /// </summary>
    public class SfDigitalGauge: Control
    {

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="SfDigitalGauge"/> class.
        /// </summary>
        public SfDigitalGauge()
        {
            DefaultStyleKey = typeof(SfDigitalGauge);
            dictionaryLoader = new DictionaryLoader();
            dictionaryLoader.InitializeEightMatrixDictionary(this.CharacterStroke, this.DimmedBrush);
            dictionaryLoader.InitializeSixteenDictionaryList(this.CharacterStroke, this.DimmedBrush);
            dictionaryLoader.InitializeFourteenDictionaryList(this.CharacterStroke, this.DimmedBrush);
            dictionaryLoader.InitializeSevenDictionary(this.CharacterStroke, this.DimmedBrush);
        }
        internal DictionaryLoader dictionaryLoader;
       
        #endregion Initialization

        #region Events

        /// <summary>
        /// Event that is raised when <see cref="EnableRTLFormat"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback EnableRTLFormatChanged;

        /// <summary>
        /// Event that is raised when <see cref="Value"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="CharacterType"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CharacterTypeChanged;

        /// <summary>
        /// Event that is raised when <see cref="CharacterHeight"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CharacterHeightChanged;

        /// <summary>
        /// Event that is raised when <see cref="CharacterWidth"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CharacterWidthChanged;

        /// <summary>
        /// Event that is raised when <see cref="CharactersSpacing"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CharactersSpacingChanged;

        /// <summary>
        /// Event that is raised when <see cref="CharacterStroke"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CharacterStrokeChanged;   

        /// <summary>
        /// Event that is raised when <see cref="DimmedBrush"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback DimmedBrushChanged;

        /// <summary>
        /// Event that is raised when <see cref="DimmedBrushOpacity"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback DimmedBrushOpacityChanged;

        /// <summary>
        /// Event that is raised when <see cref="SegmentThickness"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback SegmentThicknessChanged;

        /// <summary>
        /// Event that is raised when <see cref="SkewAngleX"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback SkewAngleXChanged;

        /// <summary>
        /// Event that is raised when <see cref="SkewAngleY"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback SkewAngleYChanged;

        
      
        #endregion Events

        #region  Dependency Properties




        /// <summary>
        /// Gets or sets a value indicating whether the control should enable Right To Left
        /// support or not. This is a dependency Property.
        /// </summary>
        /// <remarks>
        /// Default value for EnableRTLFormat is False
        /// </remarks>
        /// <value>
        /// <see langword="true" /> if ; otherwise, <see langword="false" />.
        /// </value>
        /// <example>
        /// using Syncfusion.UI.Xaml.Gauges;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// namespace TestSample
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             digitalGauge.Value = " SYNCFUSION";
        ///             digitalGauge.EnableRTLFormat = true;
        ///         }
        ///     }
        /// }
        /// </example>
        public bool EnableRTLFormat
        {
            get { return (bool)GetValue(EnableRTLFormatProperty); }
            set { SetValue(EnableRTLFormatProperty, value); }
        }
        public static readonly DependencyProperty EnableRTLFormatProperty =
            DependencyProperty.Register("EnableRTLFormat", typeof(bool), typeof(SfDigitalGauge), new PropertyMetadata(false, new PropertyChangedCallback(OnEnableRTLFormatChanged)));
        /// <summary>
        /// Calls OnEnableRTLFormatChanged method of the instance, notifies of the dependency property EnableRTLFormat changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnEnableRTLFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfDigitalGauge instance = (SfDigitalGauge)d;
            instance.OnEnableRTLFormatChanged(e);
        }
        /// <summary>
        /// Calls OnEnableRTLFormatChanged method of the instance, notifies of the dependency property EnableRTLFormat changes.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnEnableRTLFormatChanged(DependencyPropertyChangedEventArgs e)
        {
            if (EnableRTLFormatChanged != null)
                EnableRTLFormatChanged(this, e);
        }




        /// <summary>
        /// Gets or sets the value that digital gauge should display. This is a dependency
        /// property.
        /// </summary>
        /// <value>
        /// Type: <see cref="object" />
        /// </value>
        /// <example>
        /// using Syncfusion.UI.Xaml.Gauges;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// namespace TestSample
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             digitalGauge.Value = " SYNCFUSION";
        ///         }
        ///     }
        /// }
        /// 
        /// </example>
        public string Value
        {
            get
            {
                return (string)this.GetValue(ValueProperty);
            }

            set
            {
                this.SetValue(ValueProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(SfDigitalGauge), new PropertyMetadata("SYNCFUSION", new PropertyChangedCallback(OnValueChanged)));
        /// <summary>
        /// Updates property value cache and raises ValueChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ValueChanged != null)
                this.ValueChanged(this, e);
        }
        /// <summary>
        /// Calls OnValueChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfDigitalGauge instance = (SfDigitalGauge)d;
            instance.OnValueChanged(e);
        }





        /// <summary>
        /// Gets or sets the value indicating whether character should contain seven or
        /// fourteen segments or sixteen segment or in Eight cross Eight Matrix. This is a
        /// dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="CharacterType" /> Default value is CharacterType.SegmentSeven.
        /// </value>
        /// <example>
        /// using Syncfusion.UI.Xaml.Gauges;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// 
        /// namespace TestSample
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             digitalGauge.Value = " SYNCFUSION";
        ///             digitalGauge.CharacterType = CharacterType.EightCrossEightDotMatrix;
        ///         }
        ///     }
        /// }
        /// 
        /// </example>
        public CharacterType CharacterType
        {
            get
            {
                return (CharacterType)this.GetValue(CharacterTypeProperty);
            }

            set
            {
                this.SetValue(CharacterTypeProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="CharacterType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CharacterTypeProperty =
            DependencyProperty.Register("CharacterType", typeof(CharacterType), typeof(SfDigitalGauge), new PropertyMetadata(CharacterType.SegmentSeven, new PropertyChangedCallback(OnCharacterTypeChanged)));
        /// <summary>
        /// Updates property value cache and raises CharacterTypeChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected virtual void OnCharacterTypeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CharacterTypeChanged != null)
                this.CharacterTypeChanged(this, e);
        }
        /// <summary>
        /// Calls OnCharacterTypeChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCharacterTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfDigitalGauge instance = (SfDigitalGauge)d;
            instance.OnCharacterTypeChanged(e);
        }

        


        #region /// <summary>
        ///// <summary>
        ///// Gets or sets the count of characters.
        ///// This is a dependency property.
        ///// </summary>
        ///// <value>
        ///// Type: <see cref="int"/>
        ///// Default value is 2.
        ///// </value>       
        //public int CharacterCount
        //{
        //    get{return (int)this.GetValue(CharacterCountProperty);}
        //    set{this.SetValue(CharacterCountProperty, value);}
        //}
        ///// <summary>
        ///// Identifies the <see cref="CharacterCount"/> dependency property.
        ///// </summary>
        //public static readonly DependencyProperty CharacterCountProperty =
        //    DependencyProperty.Register("CharacterCount", typeof(int), typeof(SfDigitalGauge), new PropertyMetadata(0, new PropertyChangedCallback(OnCharacterCountChanged)));
     
        ///// <summary>
        ///// Updates property value cache and raises CharacterCountChanged event.
        ///// </summary>
        ///// <param name="e">
        ///// Property change details, such as old value and new value.</param>
        //   protected virtual void OnCharacterCountChanged(DependencyPropertyChangedEventArgs e)
        //  {
        //if (e.NewValue.ToString() != e.OldValue.ToString())
        //ResetCharacters();
        // }
        ///// <summary>
        ///// Calls OnCharacterCountChanged method of the instance, notifies of the dependency property value changes.
        ///// </summary>
        ///// <param name="d">Dependency object, the change occures on.</param>
        ///// <param name="e">Property change details, such as old value and new value.</param>
        //  private static void OnCharacterCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //  {
        //     SfDigitalGauge instance = (SfDigitalGauge)d;
        //     instance.OnCharacterCountChanged(e);
        // if (e.NewValue.ToString() != e.OldValue.ToString())
        //instance.ResetCharacters();
        //  }
        #endregion
        /// <summary>
        /// Gets or sets the height of the characters. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// The Value Should be in Positive.
        /// </remarks>
        /// <value>
        /// Type: <see cref="double" /> Default value is 30.
        /// </value>
        /// <example>
        /// using Syncfusion.UI.Xaml.Gauges;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// 
        /// namespace TestSample
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             digitalGauge.Value = " SYNCFUSION";
        ///             digitalGauge.CharacterHeight = 75;   
        ///         }
        ///     }
        /// }
        /// 
        /// </example>
        public double CharacterHeight
        {
            get{return (double)this.GetValue(CharacterHeightProperty);}

            set
            {
                this.SetValue(CharacterHeightProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="CharacterHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CharacterHeightProperty =
            DependencyProperty.Register("CharacterHeight", typeof(double), typeof(SfDigitalGauge), new PropertyMetadata(30d, new PropertyChangedCallback(OnCharacterHeightChanged)));
        /// <summary>
        /// Calls OnCharacterHeightChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCharacterHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfDigitalGauge instance = (SfDigitalGauge)d;
            instance.OnCharacterHeightChanged(e);
        }
        /// <summary>
        /// Updates property value cache and raises CharacterHeightChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnCharacterHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CharacterHeightChanged !=null)
                CharacterHeightChanged(this, e);
        }





        /// <summary>
        /// Gets or sets the width of the characters. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// The Value Should be in Positive.
        /// </remarks>
        /// <value>
        /// Type: <see cref="double" /> Default value is 30.
        /// </value>
        /// <example>
        /// using Syncfusion.UI.Xaml.Gauges;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// 
        /// namespace TestSample
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             digitalGauge.Value = " SYNCFUSION";
        ///             digitalGauge.CharacterWidth = 75;   
        ///         }
        ///     }
        /// }
        /// </example>
        public double CharacterWidth
        {
            get { return (double)GetValue(CharacterWidthProperty); }
            set { SetValue(CharacterWidthProperty, value); }
        }
        // Using a DependencyProperty as the backing store for CharacterWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CharacterWidthProperty =
            DependencyProperty.Register("CharacterWidth", typeof(double), typeof(SfDigitalGauge), new PropertyMetadata(30d, new PropertyChangedCallback(OnCharacterWidthChanged)));
        /// <summary>
        /// Calls OnCharacterWidthChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCharacterWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfDigitalGauge instance = (SfDigitalGauge)d;
            instance.OnCharacterWidthChanged(e);
        }
        /// <summary>
        /// Updates property value cache and raises CharacterWidthChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnCharacterWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CharacterWidthChanged != null)
                CharacterWidthChanged(this,e);
        }




        /// <summary>
        /// Gets or sets the distance between characters. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// The Character Spacing Should be in Positive else it act as RTLFormat
        /// </remarks>
        /// <value>
        /// Type: <see cref="double" /> Default value is 10.
        /// </value>
        /// <example>
        /// using Syncfusion.UI.Xaml.Gauges;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// 
        /// namespace TestSample
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             digitalGauge.Value = " SYNCFUSION";
        ///             digitalGauge.CharactersSpacing = 50;  
        ///         }
        ///     }
        /// }
        /// 
        /// </example>
        public double CharactersSpacing
        {
            get
            {
                return (double)this.GetValue(CharactersSpacingProperty);
            }

            set
            {
                this.SetValue(CharactersSpacingProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="CharactersSpacing"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CharactersSpacingProperty =
            DependencyProperty.Register("CharactersSpacing", typeof(double), typeof(SfDigitalGauge), new PropertyMetadata(10d, new PropertyChangedCallback(OnCharactersSpacingChanged)));
        /// <summary>
        /// Calls OnCharacterSpacingChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCharactersSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfDigitalGauge instance = (SfDigitalGauge)d;
            instance.OnCharactersSpacingChanged(e);
        }
        /// <summary>
        /// Updates property value cache and raises CharacterSpacingChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnCharactersSpacingChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CharactersSpacingChanged != null)
                CharactersSpacingChanged(this, e);
        }





        /// <summary>
        /// Gets or sets the brush used to color the  bright segments.This is a dependency
        /// property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush" /> Default value is Red
        /// </value>
        /// <example>
        /// using Syncfusion.UI.Xaml.Gauges;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// 
        /// namespace TestSample
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             digitalGauge.Value = " SYNCFUSION";
        ///             digitalGauge.CharacterStroke = new SolidColorBrush(Colors.Yellow);  
        ///         }
        ///     }
        /// }
        /// 
        /// </example>
        public Brush CharacterStroke
        {
            get { return (Brush)GetValue(CharacterStrokeProperty); }
            set { SetValue(CharacterStrokeProperty, value); }
        }
        // Using a DependencyProperty as the backing store for CharacterStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CharacterStrokeProperty =
            DependencyProperty.Register("CharacterStroke", typeof(Brush), typeof(SfDigitalGauge), new PropertyMetadata(new SolidColorBrush(Colors.Red),new  PropertyChangedCallback(OnCharacterStrokeChanged)));
        /// <summary>
        /// Updates property value cache and raises CharacterStrokeChanged event.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCharacterStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfDigitalGauge)
            {
                SfDigitalGauge obj = d as SfDigitalGauge;
                obj.dictionaryLoader.InitializeEightMatrixDictionary(obj.CharacterStroke, obj.DimmedBrush);
                obj.dictionaryLoader.InitializeSixteenDictionaryList(obj.CharacterStroke, obj.DimmedBrush);
                obj.dictionaryLoader.InitializeFourteenDictionaryList(obj.CharacterStroke, obj.DimmedBrush);
                obj.dictionaryLoader.InitializeSevenDictionary(obj.CharacterStroke, obj.DimmedBrush);
                obj.OnCharacterStrokeChanged(e);
            }

        }
        /// <summary>
        /// Updates property value cache and raises CharacterStrokeChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnCharacterStrokeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CharacterStrokeChanged != null)
                this.CharacterStrokeChanged(this, e);
        }






        /// <summary>
        /// Gets or sets the brush used to color the  dimmed segments.This is a dependency
        /// property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush" />  Default value is Transparent
        /// </value>
        /// <example>
        /// using Syncfusion.UI.Xaml.Gauges;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// 
        /// namespace TestSample
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///              this.InitializeComponent();
        ///              digitalGauge.Value = " SYNCFUSION";
        ///              digitalGauge.DimmedBrush = new SolidColorBrush(Colors.White);  
        ///         }
        ///     }
        /// }
        /// 
        /// </example>
        public Brush DimmedBrush
        {
            get
            {
                return (Brush)this.GetValue(DimmedBrushProperty);
            }

            set
            {
                this.SetValue(DimmedBrushProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="DimmedBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DimmedBrushProperty =
            DependencyProperty.Register("DimmedBrush", typeof(Brush), typeof(SfDigitalGauge), new PropertyMetadata(new SolidColorBrush(Colors.Transparent), new PropertyChangedCallback(OnDimmedBrushChanged)));
        /// <summary>
        /// Updates property value cache and raises DimmedBrushChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnDimmedBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DimmedBrushChanged != null)
                this.DimmedBrushChanged(this, e);
        }
        /// <summary>
        /// Calls OnDimmedBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDimmedBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
           if (d is SfDigitalGauge)
            {
                SfDigitalGauge obj = d as SfDigitalGauge;
                obj.dictionaryLoader.InitializeEightMatrixDictionary(obj.CharacterStroke, obj.DimmedBrush);
                obj.dictionaryLoader.InitializeSixteenDictionaryList(obj.CharacterStroke, obj.DimmedBrush);
                obj.dictionaryLoader.InitializeFourteenDictionaryList(obj.CharacterStroke, obj.DimmedBrush);
                obj.dictionaryLoader.InitializeSevenDictionary(obj.CharacterStroke, obj.DimmedBrush);
                obj.OnDimmedBrushChanged(e);
            }
        }






        /// <summary>
        /// Gets or sets the opacity for the dimmed segments. This is a dependency
        ///  property.
        /// </summary>
        /// <remarks>
        /// The property gets and sets the percentage of opacity to be set in the dimmed
        /// segments.
        /// </remarks>
        /// <value>
        /// Type : <see cref="double" /> Default value is 50
        /// </value>
        /// <example>
        /// using Syncfusion.UI.Xaml.Gauges;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// 
        /// namespace TestSample
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///              this.InitializeComponent();
        ///              digitalGauge.Value = " SYNCFUSION";
        ///              digitalGauge.DimmedBrush = new SolidColorBrush(Colors.White);
        ///              digitalGauge.DimmedBrushOpacity = 20;  
        ///         }
        ///     }
        /// }
        /// 
        /// </example>
        public double DimmedBrushOpacity
        {
            get { return (double)GetValue(DimmedBrushOpacityProperty); }
            set { SetValue(DimmedBrushOpacityProperty, value); }
        }
       // Using a DependencyProperty as the backing store for DimmedBrushOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DimmedBrushOpacityProperty =
            DependencyProperty.Register("DimmedBrushOpacity", typeof(double), typeof(SfDigitalGauge), new PropertyMetadata(50d, new PropertyChangedCallback(OnDimmedBrushOpacityChanged)));
        /// <summary>
        /// Updates property value cache and raises DimmedBrushOpacityChanged event.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDimmedBrushOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {if (d is SfDigitalGauge)
            {
                SfDigitalGauge obj = d as SfDigitalGauge;
                obj.DimmedBrush.Opacity = (obj.DimmedBrushOpacity / 100);
                obj.dictionaryLoader.InitializeEightMatrixDictionary(obj.CharacterStroke, obj.DimmedBrush);
                obj.dictionaryLoader.InitializeSixteenDictionaryList(obj.CharacterStroke, obj.DimmedBrush);
                obj.dictionaryLoader.InitializeFourteenDictionaryList(obj.CharacterStroke, obj.DimmedBrush);
                obj.dictionaryLoader.InitializeSevenDictionary(obj.CharacterStroke, obj.DimmedBrush);
                obj.OnDimmedBrushOpacityChanged(e);
            }
        }
        /// <summary>
        /// Updates property value cache and raises DimmedBrushOpacityChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnDimmedBrushOpacityChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DimmedBrushOpacityChanged != null)
                this.DimmedBrushOpacityChanged(this, e);
        }




        /// <summary>
        /// Gets or sets the thickness of character segments. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// It does not supports for Eight Cross Eight Segment. If the Segment thickness is
        /// negative then it will set the value as 2, if 0 the character is not visible. It
        /// also depends on CharacterHeight and CharacterWidth Properties.
        /// </remarks>
        /// <value>
        /// Type: <see cref="double"/> Default value is 2.
        /// </value>
        /// <example>
        /// using Syncfusion.UI.Xaml.Gauges;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// 
        /// namespace TestSample
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///              this.InitializeComponent();
        ///              digitalGauge.Value = &quot; SYNCFUSION&quot;;
        ///              digitalGauge.SegmentThickness = 3;  
        ///         }
        ///     }
        /// }
        /// </example>
        public double SegmentThickness
        {
            get
            {
                return (double)this.GetValue(SegmentThicknessProperty);
            }

            set
            {
                this.SetValue(SegmentThicknessProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="SegmentThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentThicknessProperty =
            DependencyProperty.Register("SegmentThickness", typeof(double), typeof(SfDigitalGauge), new PropertyMetadata(2d, new PropertyChangedCallback(OnSegmentThicknessChanged)));
        /// <summary>
        /// Updates property value cache and raises SegmentThicknessChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnSegmentThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SegmentThicknessChanged != null)
                this.SegmentThicknessChanged(this, e);
        }
        /// <summary>
        /// Calls OnSegmentThicknessChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSegmentThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfDigitalGauge instance = (SfDigitalGauge)d;
            instance.OnSegmentThicknessChanged(e);
        }






        /// <summary>
        /// Gets or sets the angle to skew the characters along the x-axis. This is a
        /// dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/> Default value is 0.
        /// </value>
        /// <example>
        /// using Syncfusion.UI.Xaml.Gauges;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// 
        /// namespace TestSample
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///              this.InitializeComponent();
        ///              digitalGauge.Value = " SYNCFUSION";
        ///              digitalGauge.SkewAngleX = 20;  
        ///         }
        ///     }
        /// }
        /// 
        /// </example>
        public double SkewAngleX
        {
            get
            {
                return (double)this.GetValue(SkewAngleXProperty);
            }

            set
            {
                this.SetValue(SkewAngleXProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="SkewAngleX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SkewAngleXProperty =
            DependencyProperty.Register("SkewAngleX", typeof(double), typeof(SfDigitalGauge), new PropertyMetadata(0d, new PropertyChangedCallback(OnSkewAngleXChanged)));
        /// <summary>
        /// Calls OnSkewAngleXChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSkewAngleXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfDigitalGauge instance = (SfDigitalGauge)d;
            instance.OnSkewAngleXChanged(e);
        }
        /// <summary>
        /// Updates property value cache and raises SkewAngleXChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnSkewAngleXChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SkewAngleXChanged != null)
                this.SkewAngleXChanged(this, e);
        }





        /// <summary>
        /// Gets or sets the angle to skew the characters along the y-axis. This is a
        /// dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double" /> Default value is 0.
        /// </value>
        /// <example>
        /// using Syncfusion.UI.Xaml.Gauges;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// 
        /// namespace TestSample
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///              this.InitializeComponent();
        ///              digitalGauge.Value = " SYNCFUSION";
        ///              digitalGauge.SkewAngleY = 25;  
        ///         }
        ///     }
        /// }
        /// 
        /// </example>
        public double SkewAngleY
        {
            get{ return (double)this.GetValue(SkewAngleYProperty);}
            set{ this.SetValue(SkewAngleYProperty, value);}
        }
        /// <summary>
        /// Identifies the <see cref="SkewAngleY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SkewAngleYProperty =
            DependencyProperty.Register("SkewAngleY", typeof(double), typeof(SfDigitalGauge), new PropertyMetadata(0d, new PropertyChangedCallback(OnSkewAngleYChanged)));
        /// <summary>
        /// Calls OnSkewAngleYChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSkewAngleYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfDigitalGauge instance = (SfDigitalGauge)d;
            instance.OnSkewAngleYChanged(e);
        }
        /// <summary>
        /// Updates property value cache and raises SkewAngleYChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnSkewAngleYChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SkewAngleYChanged != null)
                this.SkewAngleYChanged(this, e);
        }




        #endregion  Dependency Properties  


    }
}
