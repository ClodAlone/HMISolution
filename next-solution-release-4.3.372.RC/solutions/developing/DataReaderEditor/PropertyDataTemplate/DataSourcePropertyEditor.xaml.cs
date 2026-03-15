using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DataReader;
using DataReaderEditor.Converters;
using UFInterfaces;
using Utilities;
using Utilities.WPF;

namespace DataReaderEditor.PropertyDataTemplate
{
    
    /// <summary>
    /// Interaction logic for BitMaskPropertyEditor.xaml
    /// </summary>
    public partial class DataSourcePropertyEditor : UserControl
    {
        #region Workspace

        public static readonly DependencyProperty WorkspaceProperty = DependencyProperty.Register("Workspace", typeof(IWorkspace), typeof(DataSourcePropertyEditor), new UIPropertyMetadata(null));

        public IWorkspace Workspace
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (IWorkspace)GetValue(WorkspaceProperty);
            }
            set
            {
                SetValue(WorkspaceProperty, value);
            }
        }

        #endregion

        #region UseXML
        public static readonly DependencyProperty UseXMLProperty = DependencyProperty.Register("UseXML", typeof(bool), typeof(DataSourcePropertyEditor), new UIPropertyMetadata(false));
        public bool UseXML
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(UseXMLProperty);
            }
            set
            {
                SetValue(UseXMLProperty, value);
            }
        }
        #endregion

        public DataSourcePropertyEditor()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                if (Workspace?.ContextDocument != null)
                {
                    var binding = connectionlabel.GetBindingExpression(Label.ContentProperty);
                    if (binding != null && binding.ParentBinding != null && binding.ParentBinding.Converter is DataSourceConverter)
                    {
                        var converter = binding.ParentBinding.Converter as DataSourceConverter;
                        converter.projectRoot = Workspace.ContextDocument.rootBase;
                        binding.UpdateTarget();
                    }
                }
            };
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            DataReaderModel connectionstring = null;
            if(UseXML)
                connectionstring = (button.Tag as DataReaderModelXML) != null ? (button.Tag as DataReaderModelXML).ReaderModel : new DataReaderModel();
            else
                connectionstring = (button.Tag as DataReaderModel);

            if (connectionstring == null)
                connectionstring = new DataReaderModel();

            var doc = Workspace?.ContextDocument;
            var dataReaderEditor = new DataReaderEditor(connectionstring, doc?.rootBase, UseXML);
            var Dialog = new GeneralDialogContent(dataReaderEditor)
            {
                Owner = this.FindParent<Window>(),
                Title = Properties.Resources.DataSourceSelector,
                HelpLink = "DataSourceSelector"
            };
            if (Dialog.ShowDialog() == true)
            {
                connectionlabel.GetBindingExpression(Label.ContentProperty).UpdateTarget();

                if (UseXML)
                    button.Tag = new DataReaderModelXML(new DataReaderModel(connectionstring));
                else
                    button.Tag = new DataReaderModel(connectionstring);
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            connectionlabel.GetBindingExpression(Label.ContentProperty).UpdateTarget();
            if (UseXML)
                connection.Tag = new DataReaderModelXML();
            else
                connection.Tag = null;
        }
    }
    }
