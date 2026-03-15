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
using OPCUAViewModel;
using UFEventEditor.Document;
using UFEventModel;
using Utilities;
using Utilities.WPF;
using UFEventEditor.ComponentService;
using System.Windows.Threading;

namespace UFEventEditor.Controls
{
    /// <summary>
    /// Interaction logic for NewEvent.xaml
    /// </summary>
    public partial class NewEvent : UserControl, IDisposable
    {
        #region Declarations
        readonly EventEditorManagerComponent EditorComponent;
        OPCUAEntityReference item;
        OPCUAEntityReference enableitem;
        #endregion

        public NewEvent(EventEditorManagerComponent edcomp, EventEditorDocument doc)
        {
            EditorComponent = edcomp;

            InitializeComponent();

            Loaded += (o, e) =>
            {
                UFEventObject a = DataContext as UFEventObject;
                if (a != null)
                {
                    OPCUAXMLEntityReferenceModel Tag = new OPCUAXMLEntityReferenceModel() { Value = a.Tag};
                    OPCUAXMLEntityReferenceModel EnableTag = new OPCUAXMLEntityReferenceModel() { Value = a.EnableTag};
                    OPCUAXMLEntityReferenceModel ValueTag = new OPCUAXMLEntityReferenceModel() { Value = a.ValueTag};

                    textEditItem.DataContext = Tag;
                    textEditEnableItem.DataContext = EnableTag;
                    textEditValueItem.DataContext = ValueTag;

                    Tag.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            try
                            {
                                if (Tag.Value != null)
                                    a.Tag = Tag.Value;
                                else
                                    a.Tag = null;
                            }
                            catch (Exception)
                            {
                            }
                        }
                    };
                    EnableTag.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            try
                            {
                                if (EnableTag.Value != null)
                                    a.EnableTag = EnableTag.Value;
                                else
                                    a.EnableTag = null;
                            }
                            catch (Exception)
                            {
                            }
                        }
                    };
                    ValueTag.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            try
                            {
                                if (ValueTag.Value != null)
                                    a.ValueTag = ValueTag.Value;
                                else
                                    a.ValueTag = null;
                            }
                            catch (Exception)
                            {
                            }
                        }
                    };
                }

                if (CommandCtrl.Content == null)
                {
                    var commandor = EditorComponent.CommandExplorer.control;
                    EditorComponent.CommandExplorer.SetSync(commandor, true);
                    commandor.ClearValue(FrameworkElement.WidthProperty);
                    commandor.ClearValue(FrameworkElement.HeightProperty);

                    commandor.DataContext = DataContext;
                    CommandCtrl.Content = commandor;
                }
                else
                    (CommandCtrl.Content as FrameworkElement).DataContext = DataContext;
            };
            comboConditionType.ItemsSource = Enum.GetValues(typeof(ConditionType));
            comboType.ItemsSource = Enum.GetValues(typeof(EventType));
            comboSchedType.ItemsSource = Enum.GetValues(typeof(ScheduleType));
        }

        #region IDisposable Members
        public void Dispose()
        {
            EditorComponent.Workspace.ContextObject = null;
        }

        #endregion

    }
}
