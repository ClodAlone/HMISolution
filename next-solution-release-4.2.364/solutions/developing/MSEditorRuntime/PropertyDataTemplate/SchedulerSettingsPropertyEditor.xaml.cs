using System;
using System.Collections.Generic;
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
using OPCUAViewModelService.ComponentService;
using ScreenSettings;
using UFInterfaces;
using UFUAEditor.ComponentService;
using Utilities;
using Utilities.WPF;
using MSSchedulerSettings.ComponentService;

namespace SchedulerRTControl.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for HistorianSettingsPropertyEditor.xaml
    /// </summary>
    public partial class SchedulerSettingsPropertyEditor : UserControl
    {

        #region Document
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(IDocument), typeof(SchedulerSettingsPropertyEditor), new UIPropertyMetadata(null, new PropertyChangedCallback(OnDocumentChanged), new CoerceValueCallback(OnCoerceDocument)));

        private static object OnCoerceDocument(DependencyObject o, object value)
        {
            SchedulerSettingsPropertyEditor control = o as SchedulerSettingsPropertyEditor;
            if (control != null)
                return control.OnCoerceDocument((IDocument)value);
            else
                return value;
        }

        private static void OnDocumentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SchedulerSettingsPropertyEditor control = o as SchedulerSettingsPropertyEditor;
            if (control != null)
                control.OnDocumentChanged((IDocument)e.OldValue, (IDocument)e.NewValue);
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

        public SchedulerSettingsPropertyEditor()
        {
            InitializeComponent();
        }

        private void comboPrototype_DropDownOpened(object sender, EventArgs e)
        {
            if (comboPrototype.ItemsSource == null)
            {
                var list = new List<string>() { String.Empty };
                if (Document == null)
                    return;

                ISchedulerEditorManager schedulerEditorManager = Document.GetService(typeof(ISchedulerEditorManager)) as ISchedulerEditorManager;
                if (schedulerEditorManager == null)
                    return;

                var events = schedulerEditorManager.GetFlatEventsList(Document);
                if (events != null)
                    list.AddRange(events);
                comboPrototype.ItemsSource = list.OrderBy(x => x);
            }
        }
    }
}
