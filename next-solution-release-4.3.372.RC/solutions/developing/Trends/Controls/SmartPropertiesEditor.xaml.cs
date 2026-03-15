using PropertyControl.ComponentService;
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
using UFInterfaces;
using Utilities;
using Utilities.WPF;
using DocumentManager.ComponentService;

namespace Trends.Controls
{
    /// <summary>
    /// Interaction logic for SmartPropertiesEditor.xaml
    /// </summary>
    public partial class SmartPropertiesEditor : UserControl
    {
        #region Dependency Properties
        #region Document
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(IDocument), typeof(SmartPropertiesEditor), new UIPropertyMetadata(null));
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

        #region Constructors
        public SmartPropertiesEditor()
        {
            InitializeComponent();
        }
        #endregion

        #region Properties
        IWorkspace workspace;
        IWorkspace Workspace
        {
            get
            {
                if (workspace == null && Document != null)
                    workspace = Document.GetService(typeof(IWorkspace)) as IWorkspace;
                return workspace;
            }
        }
        #endregion

        #region Methods
        object GetContextObject()
        {
            var contextObject = Workspace.ContextObject;
            if (contextObject == null && Workspace.ContextObjects != null && Workspace.ContextObjects.Count > 0)
                contextObject = Workspace.ContextObjects[0];
            if (contextObject is IEntityReference)
                contextObject = (contextObject as IEntityReference).ContainedObject;
            return contextObject;
        }
        #endregion

        #region Commands
        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            var button = (Button)sender;
            if (Workspace != null)
            {
                var contextObject = GetContextObject();
                if (contextObject is Chart)
                {
                    var obj = contextObject as Chart;
                    var control = obj.SmartControl;
                    var dialog = new GeneralDialogContent(control)
                    {
                        Owner = this.FindParent<Window>(),
                        Title = Properties.Resources.SmartPropertiesPopupTitle,
                        HelpLink = "ChartSmartProperties"
                    };

                    dialog.ShowDialog();
                }
                else if (contextObject is ChartXY)
                {
                    var obj = contextObject as ChartXY;
                    var control = obj.SmartControl;
                    var dialog = new GeneralDialogContent(control)
                    {
                        Owner = this.FindParent<Window>(),
                        Title = Properties.Resources.SmartPropertiesPopupTitle,
                        HelpLink = "ChartXYSmartProperties"
                    };

                    dialog.ShowDialog();
                }
            }
        }
        #endregion
    }
}
