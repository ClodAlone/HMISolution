using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using UFInterfaces;

namespace CommonControls.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for PasswordPropertyEditor.xaml
    /// </summary>
    public partial class PasswordPropertyEditor : UserControl
    {
        #region Dependency Properties
        #region Workspace
        public static readonly DependencyProperty WorkspaceProperty = DependencyProperty.Register("Workspace", typeof(IWorkspace), typeof(PasswordPropertyEditor), new UIPropertyMetadata(null));

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

        #region ValidationNames
        public static readonly DependencyProperty ValidationNamesProperty = DependencyProperty.Register("ValidationNames", typeof(string[]), typeof(PasswordPropertyEditor), new UIPropertyMetadata(null));

        public string[] ValidationNames
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string[])GetValue(ValidationNamesProperty);
            }
            set
            {
                SetValue(ValidationNamesProperty, value);
            }
        }
        #endregion
        #endregion

        bool bLoaded;
        public PasswordPropertyEditor()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;

                if (ValidationNames != null)
                {
                    var propertyNames = new List<string>(ValidationNames);
                    var notifyObject = GetContextObject() as INotifyPropertyChanged;
                    if (notifyObject != null)
                    {
                        bool bUpdatingSource = false;
                        notifyObject.PropertyChanged += (s1, e1) =>
                        {
                            if (!bUpdatingSource && !string.IsNullOrEmpty(e1.PropertyName) && propertyNames.Contains(e1.PropertyName))
                            {
                                try
                                {
                                    bUpdatingSource = true;
                                    pswEdit.GetBindingExpression(BindablePasswordBox.PasswordProperty).UpdateSource();
                                }
                                finally
                                {
                                    bUpdatingSource = false;
                                }
                            }
                        };
                    }
                }
            };
        }

        #region Methods
        object GetContextObject()
        {
            if (Workspace == null)
                return null;

            var contextObject = Workspace.ContextObject;
            if (contextObject == null && Workspace.ContextObjects != null && Workspace.ContextObjects.Count > 0)
                contextObject = Workspace.ContextObjects[0];
            if (contextObject is IEntityReference)
                contextObject = (contextObject as IEntityReference).ContainedObject;
            return contextObject;
        }
        #endregion
    }
}
