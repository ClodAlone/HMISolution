using DevExpress.Xpf.Editors;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Utilities;
using ViewModelLib;
using WPFUtilities.PropertyDataTemplate;
using DocumentManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using System.Windows.Controls;

namespace CommonControls
{
    /// <summary>
    /// Interaction logic for CustomPopupColorEdit.xaml
    /// </summary>
    /// 
    public partial class CustomPopupColorEdit : UserControl
    {
        #region DP
        #region OnlySolidColors
        public static readonly DependencyProperty OnlySolidColorsProperty = DependencyProperty.Register("OnlySolidColors", typeof(bool), typeof(CustomPopupColorEdit), new UIPropertyMetadata(true, new PropertyChangedCallback(OnOnlySolidColorsChanged), new CoerceValueCallback(OnCoerceOnlySolidColors)));

        private static object OnCoerceOnlySolidColors(DependencyObject o, object value)
        {
            CustomPopupColorEdit CustomPopupColorEdit = o as CustomPopupColorEdit;
            if (CustomPopupColorEdit != null)
                return CustomPopupColorEdit.OnCoerceOnlySolidColors((bool)value);
            else
                return value;
        }

        private static void OnOnlySolidColorsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CustomPopupColorEdit CustomPopupColorEdit = o as CustomPopupColorEdit;
            if (CustomPopupColorEdit != null)
                CustomPopupColorEdit.OnOnlySolidColorsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceOnlySolidColors(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnOnlySolidColorsChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue)
            {
                if (newValue)
                {
                    if (Color == ColorEdit.EmptyColor)
                        SelectedBrush = originalBrush;
                    else
                        SelectedBrush = new SolidColorBrush(Color);
                }

                colorSelection.Visibility = newValue ? Visibility.Visible : Visibility.Collapsed;
                brushSelection.Visibility = newValue ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public bool OnlySolidColors
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(OnlySolidColorsProperty);
            }
            set
            {
                SetValue(OnlySolidColorsProperty, value);
            }
        }
        #endregion
        #region SelectedBrush
        public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register("SelectedBrush", typeof(Brush), typeof(CustomPopupColorEdit), new UIPropertyMetadata(null, new PropertyChangedCallback(OnSelectedBrushChanged), new CoerceValueCallback(OnCoerceSelectedBrush)));

        private static object OnCoerceSelectedBrush(DependencyObject o, object value)
        {
            CustomPopupColorEdit CustomPopupColorEdit = o as CustomPopupColorEdit;
            if (CustomPopupColorEdit != null)
                return CustomPopupColorEdit.OnCoerceSelectedBrush((Brush)value);
            else
                return value;
        }

        private static void OnSelectedBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CustomPopupColorEdit CustomPopupColorEdit = o as CustomPopupColorEdit;
            if (CustomPopupColorEdit != null)
                CustomPopupColorEdit.OnSelectedBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceSelectedBrush(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSelectedBrushChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            //if (newValue == null)
            //    newValue = new SolidColorBrush(Colors.Transparent);

            //newValue.Freeze();
        }

        public Brush SelectedBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(SelectedBrushProperty);
            }
            set
            {
                SetValue(SelectedBrushProperty, value);
            }
        }
        #endregion
        #region Color
        public static readonly DependencyProperty ColorProperty = 
            DependencyProperty.Register("Color", typeof(Color), typeof(CustomPopupColorEdit), new UIPropertyMetadata(ColorEdit.EmptyColor));

        public Color Color
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Color)GetValue(ColorProperty);
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }
        #endregion
        #region Document
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(IDocument), typeof(CustomPopupColorEdit), new UIPropertyMetadata(null, new PropertyChangedCallback(OnDocumentChanged), new CoerceValueCallback(OnCoerceDocument)));

        private static object OnCoerceDocument(DependencyObject o, object value)
        {
            CustomPopupColorEdit CustomPopupColorEdit = o as CustomPopupColorEdit;
            if (CustomPopupColorEdit != null)
                return CustomPopupColorEdit.OnCoerceDocument((IDocument)value);
            else
                return value;
        }

        private static void OnDocumentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CustomPopupColorEdit CustomPopupColorEdit = o as CustomPopupColorEdit;
            if (CustomPopupColorEdit != null)
                CustomPopupColorEdit.OnDocumentChanged((IDocument)e.OldValue, (IDocument)e.NewValue);
        }

        protected virtual IDocument OnCoerceDocument(IDocument value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnDocumentChanged(IDocument oldValue, IDocument newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public IDocument Document
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (IDocument)GetValue(DocumentProperty);
            }
            set
            {
                SetValue(DocumentProperty, value);
            }
        }
        #endregion
        #endregion

        #region Public Properties
        public IUIMsgBoxAlertService UIMsgBoxAlertService { get; set; }
        #endregion

        #region Declarations
        bool bLoaded;
        //bool bDisposed;
        public event EventHandler BrushSelected;

        public static readonly RoutedEvent MoreColorsClickedEvent = EventManager.RegisterRoutedEvent(
            "MoreColorsClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CustomPopupColorEdit));

        public event RoutedEventHandler MoreColorsClicked
        {
            add { AddHandler(MoreColorsClickedEvent, value); }
            remove { RemoveHandler(MoreColorsClickedEvent, value); }
        }
        #endregion
        Brush originalBrush;
        public CustomPopupColorEdit()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                bLoaded = true;

                if (OnlySolidColors)
                {
                    if (Color == ColorEdit.EmptyColor)
                        SelectedBrush = null;
                    else
                        SelectedBrush = new SolidColorBrush(Color);
                }
                
                originalBrush = SelectedBrush;

                RecentColorsHelper.SetPalettes(colorEdit);
                RecentColorsHelper.UpdateRecentColors(colorEdit.RecentColors);
                RecentColorsHelper.RecentColorsChanged += OnRecentColorsChanged;
                colorEdit.PopupClosed += OnPopupClosed;
            };

            Unloaded += (o, e) =>
            {
                bLoaded = false;

                colorEdit.PopupClosed -= OnPopupClosed;
                RecentColorsHelper.RecentColorsChanged -= OnRecentColorsChanged;
            };
        }

        #region Commands
        RelayCommand moreColorsCommand;
        public ICommand MoreColorsCommand
        {
            get
            {
                if (moreColorsCommand == null)
                {
                    moreColorsCommand = new RelayCommand(
                        param =>
                        {
                            var args = new RoutedEventArgs(MoreColorsClickedEvent, colorEdit);
                            RaiseEvent(args);
                            if (!args.Handled)
                                CreateBrushEditor();
                        },
                        param => bLoaded /*&& !bDisposed*/
                        );
                }
                return moreColorsCommand;
            }
        }
        #endregion

        void OnRecentColorsChanged(object sender, EventArgs e)
        {
            RecentColorsHelper.UpdateRecentColors(colorEdit.RecentColors);
        }

        private void OnPopupClosed(object sender, ClosePopupEventArgs e)
        {
            if (e.CloseMode == PopupCloseMode.Normal)
            {
                if (Color == ColorEdit.EmptyColor)
                {
                    SelectedBrush = originalBrush;
                    if (originalBrush is SolidColorBrush)
                        Color = (originalBrush as SolidColorBrush).Color;
                }
                else
                    SelectedBrush = new SolidColorBrush(colorEdit.Color);
                BrushSelected?.Invoke(this, EventArgs.Empty);
            }
        }

        void CreateBrushEditor()
        {
            var initialBrush = SelectedBrush?.Clone();
            var initialColor = Color;
            BrushEditor brushEditor = new BrushEditor(Document, UIMsgBoxAlertService) { Brush = SelectedBrush ?? new SolidColorBrush(Color), OnlySolidColors = OnlySolidColors };

            var Dialog = new GeneralDialogContent(brushEditor)
            {
                //Owner = this.FindParent<Window>(),
                Title = Properties.Resources.BrushEditor,
                HelpLink = "BrushEditor"
            };
            brushEditor.SelectionCompleted += (o, ev) =>
            {
                Dialog.DialogResult = true;
            };
            brushEditor.BrushChanged += (o, e) =>
            {
                SelectedBrush = brushEditor.Brush;
                if (brushEditor.Brush is SolidColorBrush)
                {
                    Color = (brushEditor.Brush as SolidColorBrush).Color;
                }
            };

            if (Dialog.ShowDialog() == true)
            {
                if (brushEditor.Brush is SolidColorBrush)
                {
                    var color = (brushEditor.Brush as SolidColorBrush).Color;
                    RecentColorsHelper.AddRecentColor(this, color);
                }
            }
            else
            {
                SelectedBrush = initialBrush; 
                Color = initialColor;
            }
            BrushSelected?.Invoke(this, EventArgs.Empty);
        }
    }
}
