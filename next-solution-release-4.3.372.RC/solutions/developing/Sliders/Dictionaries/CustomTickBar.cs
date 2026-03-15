using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Sliders
{
    public class CustomTickBar : TickBar
    {
        #region TickSize
        public static readonly DependencyProperty TickSizeProperty = DependencyProperty.Register("TickSize", typeof(int), typeof(CustomTickBar), new UIPropertyMetadata(8, new PropertyChangedCallback(OnTickSizeChanged), new CoerceValueCallback(OnCoerceTickSize)));

        private static object OnCoerceTickSize(DependencyObject o, object value)
        {
            CustomTickBar control = o as CustomTickBar;
            if (control != null)
                return control.OnCoerceTickSize((int)value);
            else
                return value;
        }

        private static void OnTickSizeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CustomTickBar control = o as CustomTickBar;
            if (control != null)
                control.OnTickSizeChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceTickSize(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTickSizeChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public int TickSize
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(TickSizeProperty);
            }
            set
            {
                SetValue(TickSizeProperty, value);
            }
        }

        #endregion

        #region TickMinorSize
        public static readonly DependencyProperty TickMinorSizeProperty = DependencyProperty.Register("TickMinorSize", typeof(int), typeof(CustomTickBar), new UIPropertyMetadata(4, new PropertyChangedCallback(OnTickMinorSizeChanged), new CoerceValueCallback(OnCoerceTickMinorSize)));

        private static object OnCoerceTickMinorSize(DependencyObject o, object value)
        {
            CustomTickBar control = o as CustomTickBar;
            if (control != null)
                return control.OnCoerceTickMinorSize((int)value);
            else
                return value;
        }

        private static void OnTickMinorSizeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CustomTickBar control = o as CustomTickBar;
            if (control != null)
                control.OnTickMinorSizeChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceTickMinorSize(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTickMinorSizeChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public int TickMinorSize
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(TickMinorSizeProperty);
            }
            set
            {
                SetValue(TickMinorSizeProperty, value);
            }
        }

        #endregion


        #region TickThickness
        public static readonly DependencyProperty TickThicknessProperty = DependencyProperty.Register("TickThickness", typeof(double), typeof(CustomTickBar), new UIPropertyMetadata(1.0, new PropertyChangedCallback(OnTickThicknessChanged), new CoerceValueCallback(OnCoerceTickThickness)));

        private static object OnCoerceTickThickness(DependencyObject o, object value)
        {
            CustomTickBar control = o as CustomTickBar;
            if (control != null)
                return control.OnCoerceTickThickness((double)value);
            else
                return value;
        }

        private static void OnTickThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CustomTickBar control = o as CustomTickBar;
            if (control != null)
                control.OnTickThicknessChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceTickThickness(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTickThicknessChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public double TickThickness
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(TickThicknessProperty);
            }
            set
            {
                SetValue(TickThicknessProperty, value);
            }
        }

        #endregion



        #region TickFontFamily
        public static readonly DependencyProperty TickFontFamilyProperty = DependencyProperty.Register("TickFontFamily", typeof(FontFamily), typeof(CustomTickBar), new UIPropertyMetadata(new FontFamily("Segoe UI"), new PropertyChangedCallback(OnTickFontFamilyChanged), new CoerceValueCallback(OnCoerceTickFontFamily)));

        private static object OnCoerceTickFontFamily(DependencyObject o, object value)
        {
            CustomTickBar control = o as CustomTickBar;
            if (control != null)
                return control.OnCoerceTickFontFamily((FontFamily)value);
            else
                return value;
        }

        private static void OnTickFontFamilyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CustomTickBar control = o as CustomTickBar;
            if (control != null)
                control.OnTickFontFamilyChanged((FontFamily)e.OldValue, (FontFamily)e.NewValue);
        }

        protected virtual FontFamily OnCoerceTickFontFamily(FontFamily value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTickFontFamilyChanged(FontFamily oldValue, FontFamily newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public FontFamily TickFontFamily
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (FontFamily)GetValue(TickFontFamilyProperty);
            }
            set
            {
                SetValue(TickFontFamilyProperty, value);
            }
        }

        #endregion


        #region TickFontSize
        public static readonly DependencyProperty TickFontSizeProperty = DependencyProperty.Register("TickFontSize", typeof(double), typeof(CustomTickBar), new UIPropertyMetadata(12.0, new PropertyChangedCallback(OnTickFontSizeChanged), new CoerceValueCallback(OnCoerceTickFontSize)));

        private static object OnCoerceTickFontSize(DependencyObject o, object value)
        {
            CustomTickBar control = o as CustomTickBar;
            if (control != null)
                return control.OnCoerceTickFontSize((double)value);
            else
                return value;
        }

        private static void OnTickFontSizeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CustomTickBar control = o as CustomTickBar;
            if (control != null)
                control.OnTickFontSizeChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceTickFontSize(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTickFontSizeChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public double TickFontSize
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(TickFontSizeProperty);
            }
            set
            {
                SetValue(TickFontSizeProperty, value);
            }
        }

        #endregion


        #region TickFontStyle
        public static readonly DependencyProperty TickFontStyleProperty = DependencyProperty.Register("TickFontStyle", typeof(FontStyle), typeof(CustomTickBar), new UIPropertyMetadata(FontStyles.Normal, new PropertyChangedCallback(OnTickFontStyleChanged), new CoerceValueCallback(OnCoerceTickFontStyle)));

        private static object OnCoerceTickFontStyle(DependencyObject o, object value)
        {
            CustomTickBar control = o as CustomTickBar;
            if (control != null)
                return control.OnCoerceTickFontStyle((FontStyle)value);
            else
                return value;
        }

        private static void OnTickFontStyleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CustomTickBar control = o as CustomTickBar;
            if (control != null)
                control.OnTickFontStyleChanged((FontStyle)e.OldValue, (FontStyle)e.NewValue);
        }

        protected virtual FontStyle OnCoerceTickFontStyle(FontStyle value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTickFontStyleChanged(FontStyle oldValue, FontStyle newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public FontStyle TickFontStyle
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (FontStyle)GetValue(TickFontStyleProperty);
            }
            set
            {
                SetValue(TickFontStyleProperty, value);
            }
        }

        #endregion


        #region TickFontWeight
        public static readonly DependencyProperty TickFontWeightProperty = DependencyProperty.Register("TickFontWeight", typeof(FontWeight), typeof(CustomTickBar), new UIPropertyMetadata(FontWeights.Normal, new PropertyChangedCallback(OnTickFontWeightChanged), new CoerceValueCallback(OnCoerceTickFontWeight)));

        private static object OnCoerceTickFontWeight(DependencyObject o, object value)
        {
            CustomTickBar control = o as CustomTickBar;
            if (control != null)
                return control.OnCoerceTickFontWeight((FontWeight)value);
            else
                return value;
        }

        private static void OnTickFontWeightChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CustomTickBar control = o as CustomTickBar;
            if (control != null)
                control.OnTickFontWeightChanged((FontWeight)e.OldValue, (FontWeight)e.NewValue);
        }

        protected virtual FontWeight OnCoerceTickFontWeight(FontWeight value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTickFontWeightChanged(FontWeight oldValue, FontWeight newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public FontWeight TickFontWeight
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (FontWeight)GetValue(TickFontWeightProperty);
            }
            set
            {
                SetValue(TickFontWeightProperty, value);
            }
        }

        #endregion


        #region TickMajorFrequency
        public static readonly DependencyProperty TickMajorFrequencyProperty = DependencyProperty.Register("TickMajorFrequency", typeof(double), typeof(CustomTickBar), new UIPropertyMetadata(20.0, new PropertyChangedCallback(OnTickMajorFrequencyChanged), new CoerceValueCallback(OnCoerceTickMajorFrequency)));

        private static object OnCoerceTickMajorFrequency(DependencyObject o, object value)
        {
            CustomTickBar control = o as CustomTickBar;
            if (control != null)
                return control.OnCoerceTickMajorFrequency((double)value);
            else
                return value;
        }

        private static void OnTickMajorFrequencyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CustomTickBar control = o as CustomTickBar;
            if (control != null)
                control.OnTickMajorFrequencyChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceTickMajorFrequency(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTickMajorFrequencyChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public double TickMajorFrequency
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(TickMajorFrequencyProperty);
            }
            set
            {
                SetValue(TickMajorFrequencyProperty, value);
            }
        }

        #endregion


        #region ShowLabel
        public static readonly DependencyProperty ShowLabelProperty = DependencyProperty.Register("ShowLabel", typeof(bool), typeof(CustomTickBar), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowLabelChanged), new CoerceValueCallback(OnCoerceShowLabel)));

        private static object OnCoerceShowLabel(DependencyObject o, object value)
        {
            CustomTickBar control = o as CustomTickBar;
            if (control != null)
                return control.OnCoerceShowLabel((bool)value);
            else
                return value;
        }

        private static void OnShowLabelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CustomTickBar control = o as CustomTickBar;
            if (control != null)
                control.OnShowLabelChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowLabel(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowLabelChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool ShowLabel
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowLabelProperty);
            }
            set
            {
                SetValue(ShowLabelProperty, value);
            }
        }

        #endregion


        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            try 
	        {
                double num = this.Maximum - this.Minimum;
                double y = this.ReservedSpace * 0.5;
                FormattedText formattedText = null;
                double x = 0;
                double _TickSize =  (double)TickSize;
                bool _ShowLabel = ShowLabel;
                Pen pen = new Pen(this.Fill, TickThickness);
                var points = TickFrequency > 0 ? (num / TickFrequency) : 0;
                var tickFrequency = TickFrequency;
                if (points == 0)
                    return;
                if (points > Properties.Settings.Default.MaxTickNumber)
                {
                    points = Properties.Settings.Default.MaxTickNumber - 1;
                    tickFrequency = num / points;
                }

                var majorPoints = TickMajorFrequency > 0 ? (num / TickMajorFrequency) : 0;
                if (majorPoints > points)
                    majorPoints = points;
                int delta = majorPoints > 0 ? (int)(points / majorPoints) : (int)points;
                var deltaWidth = this.ActualWidth / points;
                decimal j = 0;
                decimal value = 0;
                for (int i = 0; i <= points; i++)
                {
                    try 
	                {	        
                        if (i == 0)
                            x = 0;
                        else
                            x += deltaWidth;
                        value = (decimal)(Minimum + i * tickFrequency);
                        if (TickMajorFrequency > 0 && (i==0 || i == points || j == i))
                        {
                            _ShowLabel = ShowLabel;
                            _TickSize = (double)TickSize;
                            j += delta;
                        }
                        else
                        {
                            _ShowLabel = false;
                            _TickSize = (double)TickMinorSize;
                        }

                        formattedText = new FormattedText((value).ToString(), System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(TickFontFamily, TickFontStyle, TickFontWeight, FontStretches.Normal),
                            TickFontSize, this.Fill);

                        double dim = (double)TickFontSize / 2;
                        double dimLabel = formattedText.Width / 2;

                        if (this.Name.Equals("TopTick"))
                        {
                            dc.DrawLine(pen, new Point(x, 0), new Point(x,  _TickSize ));
                            if(_ShowLabel)
                                dc.DrawText(formattedText, new Point(x - dimLabel, -(_TickSize + 6 + dim)));
                        }
                        else
                        {
                            dc.DrawLine(pen, new Point(x, _TickSize), new Point(x, 0));
                            if (_ShowLabel)
                                dc.DrawText(formattedText, new Point(x - dimLabel, _TickSize + dim));
                        }
                    }
                    catch (Exception)
	                {
	                }
                }
	        }
	        catch (Exception)
	        {
	        }
        }
    }
}
