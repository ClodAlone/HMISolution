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
using UFCrossReferenceEditor.ComponentService;
using WPFUtilities;
using WPFUtilities.Extensions;
using Utilities;
using Utilities.WPF;


namespace CrossReferenceControl
{
    /// <summary>
    /// Interaction logic for CrossReference.xaml
    /// </summary>
    [Obsolete("This control is obsolete and not supported anymore.")]
    public partial class CrossReference : UserControl, IDisposable
    {
        #region DP
        #region OverrideBaseProperties

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(CrossReference));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(CrossReference));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
        }
        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(CrossReference));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(CrossReference));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
        }


        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as CrossReference;
            if (control != null)
            {
                control.OnForegroundChanged();
            }
        }
        protected virtual void OnForegroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if(bInit)
                UpdateForeColor();
        }
        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            var control = sender as CrossReference;
            if (control != null)
            {
                control.OnBackgroundChanged();
            }
        }
        protected virtual void OnBackgroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if(bInit && !IsManipulationEnabled)
                UpdateBackColor();
        }
        #endregion
        #endregion

        #region Public Props
        [Browsable(false)]
        public bool RunningOnServer
        {
            get
            {
                return ScreenSettings.ScreenDocument.GetRunningOnServer(this);
            }
        }
        #endregion

        #region Declarations
        ICrossReferenceEditorManager crossReferenceEditorManager;
        IDocument parent;
        bool bDesign;
        bool bLoaded;
        bool bELoaded;
        bool bDispose;
        bool bInit;
        #endregion

        #region Contructor
        public CrossReference()
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
                        if (crossReferenceEditorManager == null)
                            crossReferenceEditorManager = parent.GetService(typeof(ICrossReferenceEditorManager)) as ICrossReferenceEditorManager;
                        if (crossReferenceEditorManager != null && crossreference.Content == null)
                        {
                            var editor = crossReferenceEditorManager.GetRuntimeControl(parent, RunningOnServer);
                            if (editor != null)
                            {
                                editor.Loaded += (ob, ea) =>
                                    {
                                        if (!bELoaded)
                                        {
                                            bELoaded = true;
                                            InitBrush();
                                        }
                                    };

                                editor.ClearValue(FrameworkElement.WidthProperty);
                                editor.ClearValue(FrameworkElement.HeightProperty);
                                crossreference.Content = editor;
                            }
                        }
                    }
                    if (DesignerProperties.GetIsInDesignMode(this) || bDesign)
                    {
                        bDesign = true;
                        crossreference.IsEnabled = false;
                    }
                    UpdateBackColor();
                    UpdateForeColor();
                    bInit = true;
                }
            };
        }


        void UpdateBackColor()
        {
            if (this.ReadLocalValue(BackgroundProperty) != DependencyProperty.UnsetValue)
            {
                if (crossreference.Content != null)
                {
                    (crossreference.Content as UserControl).Background = Background;
                    (from c in ((crossreference.Content as UserControl).Content as UIElement).GetVisualChildrenOfType<Control>()
                     select c).ToList().ForEach(child =>
                     {
                         child.Background = Background;
                     });
                }
            }
        }

        void UpdateForeColor()
        {
            if (this.ReadLocalValue(ForegroundProperty) != DependencyProperty.UnsetValue)
            {
                if (crossreference.Content != null)
                {
                    (crossreference.Content as UserControl).Foreground = Foreground;
                    (from c in ((crossreference.Content as UserControl).Content as UIElement).GetVisualChildrenOfType<TextBlock>()
                     select c).ToList().ForEach(child =>
                     {
                         child.Foreground = Foreground;
                     });
                    (from c in ((crossreference.Content as UserControl).Content as UIElement).GetVisualChildrenOfType<TextBox>()
                     select c).ToList().ForEach(child =>
                     {
                         child.Foreground = Foreground;
                     });
                }
            }
        }

        private void InitBrush()
        {
            UpdateBackColor();
            UpdateForeColor();
        }

        #endregion
        public void Dispose()
        {
            if (bDispose)
                return;
            bDispose = true;
            DetachOverrideBaseProperties();
            
            if (crossreference.Content != null && crossreference.Content is IDisposable)
                (crossreference.Content as IDisposable).Dispose();
        }
    }
}
