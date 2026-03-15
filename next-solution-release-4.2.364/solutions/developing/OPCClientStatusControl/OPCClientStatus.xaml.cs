using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DocumentManager.ComponentService;
using ScreenSettings;
using WPFUtilities;
using WPFUtilities.Extensions;
using Utilities;
using Utilities.WPF;
using OPCUAClientStatus.ComponentService;

namespace OPCClientStatusControl
{
    /// <summary>
    /// Interaction logic for clientstatus.xaml
    /// </summary>
    public partial class OPCClientStatusControl : UserControl, IDisposable
    {
        #region DP
        #region OverrideBaseProperties

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(OPCClientStatusControl));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(OPCClientStatusControl));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);

            OnForegroundChanged();
            OnBackgroundChanged();
        }

        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(OPCClientStatusControl));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(OPCClientStatusControl));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
        }


        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as OPCClientStatusControl;
            if (control != null)
            {
                control.OnForegroundChanged();
            }
        }
        protected virtual void OnForegroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                ControlForeground = Foreground;
        }
        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            var control = sender as OPCClientStatusControl;
            if (control != null)
            {
                control.OnBackgroundChanged();
            }
        }
        protected virtual void OnBackgroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && !IsManipulationEnabled)
                InitBrush();
        }
        #endregion


        #region ControlForeground
        public static readonly DependencyProperty ControlForegroundProperty = DependencyProperty.Register("ControlForeground", typeof(Brush), typeof(OPCClientStatusControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnControlForegroundChanged), new CoerceValueCallback(OnCoerceControlForeground)));

        private static object OnCoerceControlForeground(DependencyObject o, object value)
        {
            OPCClientStatusControl control = o as OPCClientStatusControl;
            if (control != null)
                return control.OnCoerceControlForeground((Brush)value);
            else
                return value;
        }

        private static void OnControlForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            OPCClientStatusControl control = o as OPCClientStatusControl;
            if (control != null)
                control.OnControlForegroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceControlForeground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnControlForegroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                InitBrush();
        }
        [Browsable(false)]
        public Brush ControlForeground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(ControlForegroundProperty);
            }
            set
            {
                SetValue(ControlForegroundProperty, value);
            }
        }

        #endregion

        #endregion

        #region Declarations
        IOPCUAClientStatus opcClientStatus;
        IDocument parent;
        bool bDesign;
        bool bLoaded;
        bool bDispose;
        bool bInit;
        #endregion

        #region Contructor
        public OPCClientStatusControl()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            Loaded += (o, e) =>
            {
                if (!bLoaded && !bDispose)
                {
                    bLoaded = true;

                    
                    OverrideBaseProperties();
                    if (parent == null)
                        parent = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                    if (parent != null)
                    {
                        if (opcClientStatus == null)
                            opcClientStatus = parent.GetService(typeof(IOPCUAClientStatus)) as IOPCUAClientStatus;
                        if (opcClientStatus != null && clientContainer.Content == null)
                        {
                            var editor = opcClientStatus.GetClientStatusControl();
                            if (editor != null)
                            {
                                editor.ClearValue(FrameworkElement.WidthProperty);
                                editor.ClearValue(FrameworkElement.HeightProperty);
                                ThemeHelper.SetTheme(editor, parent.Theme);
                                clientContainer.Content = editor;
                            }
                        }
                    }
                    if (DesignerProperties.GetIsInDesignMode(this) || bDesign)
                    {
                        bDesign = true;
                        clientContainer.IsEnabled = false;
                    }
                    InitBrush();
                    bInit = true;
                }
            };
        }

        private void InitBrush()
        {
            if (this.ReadLocalValue(BackgroundProperty) != DependencyProperty.UnsetValue)
            {
                if (clientContainer.Content != null)
                    (clientContainer.Content as UserControl).Background = Background;
            }
            else
            {
                if (ThemeImageHelper.GetTheme(parent) == ThemeType.VS2010.ToString() || ThemeImageHelper.GetTheme(parent) == ThemeType.Default.ToString())
                    view.Background = new SolidColorBrush(Color.FromArgb(255, 180, 180, 180));
                else if (ThemeImageHelper.GetTheme(parent) == ThemeType.None.ToString())
                    view.Background = new SolidColorBrush(Color.FromArgb(255, 163, 195, 236));
                else
                    view.Background = new SolidColorBrush(Color.FromArgb(255, 51, 51, 51));
            }

            if (this.ReadLocalValue(ControlForegroundProperty) != DependencyProperty.UnsetValue)
            {
                if (clientContainer.Content != null)
                {
                    if (clientContainer.Content != null)
                    {
                        (clientContainer.Content as UserControl).Foreground = ControlForeground;

                        (from c in ((clientContainer.Content as UserControl).Content as UIElement).GetVisualChildrenOfType<TextBlock>()
                         select c).ToList().ForEach(child =>
                         {
                             child.Foreground = ControlForeground;
                         });
                        (from c in ((clientContainer.Content as UserControl).Content as UIElement).GetVisualChildrenOfType<TextBox>()
                         select c).ToList().ForEach(child =>
                         {
                             child.Foreground = ControlForeground;
                         });
                    }

                    //(from c in ((clientContainer.Content as UserControl).Content as UIElement).GetVisualChildrenOfType<TextBlock>()
                    // select c).ToList().ForEach(child =>
                    // {
                    //     child.Foreground = Foreground;
                    // });
                    //(from c in ((clientContainer.Content as UserControl).Content as UIElement).GetVisualChildrenOfType<TextBox>()
                    // select c).ToList().ForEach(child =>
                    // {
                    //     child.Foreground = Foreground;
                    // });
                }
            }
        }

        #endregion
        public void Dispose()
        {
            if (bDispose)
                return;
            bDispose = true;
            DetachOverrideBaseProperties();
            
            if (clientContainer.Content != null && clientContainer.Content is IDisposable)
                (clientContainer.Content as IDisposable).Dispose();
        }
    }
}
