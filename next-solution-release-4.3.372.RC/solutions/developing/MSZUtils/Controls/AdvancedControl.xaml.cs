using DevExpress.Xpo;
using System.IO.IsolatedStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Input;
using System.Reflection;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml;
using System.Text;
using Utilities;
using UFInterfaces.PropertyControl;
using CommonControls.PropertyDataTemplate;
using System.Data.SqlTypes;
using StringManager.ComponentService;
using System.Globalization;
using DocumentManager.ComponentService;
using Converters;
using System.Windows.Data;
using WPFUtilities.PropertyDataTemplate;
using Utilities.WPF;
using WPFUtilities;
using GridLayout;
using UIMsgBoxAlertService.ComponentService;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Grid;
using System.Data;
using DevExpress.Xpf.Core;
using MSZUtils.Helpers;
using System.Diagnostics;
using MSZUtilsServiceHelper;

namespace MSZUtils.Controls
{
    /// <summary>
    /// Interaction logic for HistoricalEvents.xaml
    /// </summary>
    public enum Options
    {
        Range,
        List
    }
    public partial class AdvancedControl : DXWindow
    {

        #region Selection
        public static readonly DependencyProperty SelectionProperty = DependencyProperty.Register("Selection", typeof(Options), typeof(AdvancedControl), new UIPropertyMetadata(Options.Range, new PropertyChangedCallback(OnSelectionChanged), new CoerceValueCallback(OnCoerceSelection)));

        private static object OnCoerceSelection(DependencyObject o, object value)
        {
            AdvancedControl control = o as AdvancedControl;
            if (control != null)
                return control.OnCoerceSelection((Options)value);
            else
                return value;
        }

        private static void OnSelectionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            AdvancedControl control = o as AdvancedControl;
            if (control != null)
                control.OnSelectionChanged((Options)e.OldValue, (Options)e.NewValue);
        }

        protected virtual Options OnCoerceSelection(Options value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSelectionChanged(Options oldValue, Options newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public Options Selection
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Options)GetValue(SelectionProperty);
            }
            set
            {
                SetValue(SelectionProperty, value);
            }
        }

        #endregion

        #region SerialNumbers
        public static readonly DependencyProperty SerialNumbersProperty = DependencyProperty.Register("SerialNumbers", typeof(string), typeof(AdvancedControl), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnSerialNumbersChanged), new CoerceValueCallback(OnCoerceSerialNumbers)));

        private static object OnCoerceSerialNumbers(DependencyObject o, object value)
        {
            AdvancedControl control = o as AdvancedControl;
            if (control != null)
                return control.OnCoerceSerialNumbers((string)value);
            else
                return value;
        }

        private static void OnSerialNumbersChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            AdvancedControl control = o as AdvancedControl;
            if (control != null)
                control.OnSerialNumbersChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceSerialNumbers(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSerialNumbersChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public string SerialNumbers
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(SerialNumbersProperty);
            }
            set
            {
                SetValue(SerialNumbersProperty, value);
            }
        }

        #endregion

        #region FromSerial
        public static readonly DependencyProperty FromSerialProperty = DependencyProperty.Register("FromSerial", typeof(int), typeof(AdvancedControl), new UIPropertyMetadata(-1, new PropertyChangedCallback(OnFromSerialChanged), new CoerceValueCallback(OnCoerceFromSerial)));

        private static object OnCoerceFromSerial(DependencyObject o, object value)
        {
            AdvancedControl control = o as AdvancedControl;
            if (control != null)
                return control.OnCoerceFromSerial((int)value);
            else
                return value;
        }

        private static void OnFromSerialChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            AdvancedControl control = o as AdvancedControl;
            if (control != null)
                control.OnFromSerialChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceFromSerial(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnFromSerialChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public int FromSerial
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(FromSerialProperty);
            }
            set
            {
                SetValue(FromSerialProperty, value);
            }
        }

        #endregion

        #region ToSerial
        public static readonly DependencyProperty ToSerialProperty = DependencyProperty.Register("ToSerial", typeof(int), typeof(AdvancedControl), new UIPropertyMetadata(-1, new PropertyChangedCallback(OnToSerialChanged), new CoerceValueCallback(OnCoerceToSerial)));

        private static object OnCoerceToSerial(DependencyObject o, object value)
        {
            AdvancedControl control = o as AdvancedControl;
            if (control != null)
                return control.OnCoerceToSerial((int)value);
            else
                return value;
        }

        private static void OnToSerialChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            AdvancedControl control = o as AdvancedControl;
            if (control != null)
                control.OnToSerialChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceToSerial(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnToSerialChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public int ToSerial
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(ToSerialProperty);
            }
            set
            {
                SetValue(ToSerialProperty, value);
            }
        }

        #endregion

        #region Contructor
        bool bLoaded;
        internal AdvancedControl(int from, int to)
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;
                    ThemeHelper.SetTheme(this, Properties.Settings.Default.ThemeName);
                    FromSerial = from;
                    ToSerial = to;
                    DataContext = this;
                }
            };
        }
        #endregion
        #region Methods
        public bool ShowDetailsList()
        {
            if (ShowDialog() == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
