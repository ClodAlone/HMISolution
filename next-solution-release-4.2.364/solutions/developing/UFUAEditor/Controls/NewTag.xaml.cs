using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpo;
using DriverBaseInterfaces;
using Opc.Ua;
using SmartTagsControl.ComponentService;
using UFInterfaces.Editors;
using UFUAEditor.ComponentService;
using UFUAEditor.Document;
using UFUAEditor.PropertyDataTemplate;
using Utilities;
using Utilities.WPF;
using CommonControls;

namespace UFUAEditor.Controls
{
    /// <summary>
    /// Interaction logic for NewTag.xaml
    /// </summary>
    public partial class NewTag : UserControl
    {
        #region Declarations
        readonly UFUAServerDocument Document;
        readonly bool IsPrototype;

        UFUAModel.UFUATag tag = null;
        bool cancel = false;
        string oldDynSettings = null;
        #endregion

        public NewTag(UFUAServerDocument doc, bool isPrototype = false)
        {
            InitializeComponent();
            Document = doc;

            comboModelType.ItemsSource = Enum.GetValues(typeof(UFUAModel.ModelType));
            comboDataType.ItemsSource = Enum.GetValues(typeof(UFUAModel.DataType));

            IsPrototype = isPrototype;

            Loaded += (p, q) =>
            {
                tag = DataContext as UFUAModel.UFUATag;
                if (tag != null)
                {
                    var wrapper = new StringWrapperObject(tag.DynamicSettings);
                    wrapper.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "Value")
                            tag.DynamicSettings = wrapper.Value;
                    };

                    gridDynamic.Workspace = UFUAEditorManagerComponent.ufuaEditorManagerComponent.Workspace;
                    gridDynamic.DynSettingsEditObject = tag;
                    gridDynamic.DataContext = wrapper;
                }
            };
        }        

        private void comboModelType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
            var tag = DataContext as UFUAModel.UFUATag;
            if (tag.ModelType == UFUAModel.ModelType.ObjectType)
            {
                if (comboPrototype.ItemsSource == null)
                {
                    using (new WaitCursor())
                    {
                        comboPrototype.ItemsSource = Document.GetPrototypesNames();
                    }
                }

                textEnumStrings.Visibility = Visibility.Collapsed;
                //textEditEnumStrings.Visibility = Visibility.Collapsed;
                dockEnumStrings.Visibility = Visibility.Collapsed;

                comboDataType.Visibility = Visibility.Collapsed;
                textDataType.Visibility = Visibility.Collapsed;

                comboPrototype.Visibility = Visibility.Visible;
                textPrototype.Visibility = Visibility.Visible;

                textBlockArrayDimension.Visibility = Visibility.Collapsed;
                textArrayDimension.Visibility = Visibility.Collapsed;

                textBlockInitialValue.Visibility = Visibility.Collapsed;
                textInitialValue.Visibility = Visibility.Collapsed;

                if (IsPrototype)
                {
                    textDynamicSettings.Visibility = Visibility.Collapsed;
                    contentDynamic.Visibility = Visibility.Collapsed;

                    checkBoxRetentive.Visibility = Visibility.Collapsed;
                    textBlockRetentive.Visibility = Visibility.Collapsed;
                }
                else 
                {
                    textDynamicSettings.Visibility = Visibility.Visible;
                    contentDynamic.Visibility = Visibility.Visible;

                    if (!tag.IsSubPrototypeMember)
                    {
                        checkBoxRetentive.Visibility = Visibility.Visible;
                        textBlockRetentive.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        checkBoxRetentive.Visibility = Visibility.Collapsed;
                        textBlockRetentive.Visibility = Visibility.Collapsed;
                    }
                }
            }
            else if (tag.ModelType == UFUAModel.ModelType.Variable ||
                tag.ModelType == UFUAModel.ModelType.Analog)
            {
                textEnumStrings.Visibility = Visibility.Collapsed;
                //textEditEnumStrings.Visibility = Visibility.Collapsed;
                dockEnumStrings.Visibility = Visibility.Collapsed;

                comboDataType.Visibility = Visibility.Visible;
                textDataType.Visibility = Visibility.Visible;

                comboPrototype.Visibility = Visibility.Collapsed;
                textPrototype.Visibility = Visibility.Collapsed;

                textBlockArrayDimension.Visibility = Visibility.Visible;
                checkBoxRetentive.Visibility = Visibility.Visible;
                textBlockRetentive.Visibility = Visibility.Visible;
                textArrayDimension.Visibility = Visibility.Visible;

                if (IsPrototype)
                {
                    textBlockInitialValue.Visibility = Visibility.Collapsed;
                    textInitialValue.Visibility = Visibility.Collapsed;

                    checkBoxRetentive.Visibility = Visibility.Collapsed;
                    textBlockRetentive.Visibility = Visibility.Collapsed;

                    textDynamicSettings.Visibility = Visibility.Collapsed;
                    contentDynamic.Visibility = Visibility.Collapsed;
                }
                else
                { 
                    textBlockInitialValue.Visibility = Visibility.Visible;
                    textInitialValue.Visibility = Visibility.Visible;

                    if (!tag.IsSubPrototypeMember)
                    {
                        checkBoxRetentive.Visibility = Visibility.Visible;
                        textBlockRetentive.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        checkBoxRetentive.Visibility = Visibility.Collapsed;
                        textBlockRetentive.Visibility = Visibility.Collapsed;
                    }

                    textDynamicSettings.Visibility = Visibility.Visible;
                    contentDynamic.Visibility = Visibility.Visible;
                }

                if (comboEngineeringUnit.ItemsSource == null)
                {
                    using (new WaitCursor())
                    {
                        var eunits = new List<String>();
                        eunits.Add(String.Empty);
                        eunits.AddRange(Document.GetEngineeringUnitNames());
                        comboEngineeringUnit.ItemsSource = eunits;
                    }
                }
            }
            else if (tag.ModelType == UFUAModel.ModelType.Digital)
            {
                textEnumStrings.Visibility = Visibility.Visible;
                //textEditEnumStrings.Visibility = Visibility.Visible;
                dockEnumStrings.Visibility = Visibility.Visible;

                comboDataType.Visibility = Visibility.Collapsed;
                textDataType.Visibility = Visibility.Collapsed;

                comboPrototype.Visibility = Visibility.Collapsed;
                textPrototype.Visibility = Visibility.Collapsed;

                textBlockArrayDimension.Visibility = Visibility.Visible;
                checkBoxRetentive.Visibility = Visibility.Visible;
                textBlockRetentive.Visibility = Visibility.Visible;
                textArrayDimension.Visibility = Visibility.Visible;

                if (IsPrototype)
                {
                    textBlockInitialValue.Visibility = Visibility.Collapsed;
                    textInitialValue.Visibility = Visibility.Collapsed;

                    checkBoxRetentive.Visibility = Visibility.Collapsed;
                    textBlockRetentive.Visibility = Visibility.Collapsed;

                    textDynamicSettings.Visibility = Visibility.Collapsed;
                    contentDynamic.Visibility = Visibility.Collapsed;
                }
                else
                {
                    textBlockInitialValue.Visibility = Visibility.Visible;
                    textInitialValue.Visibility = Visibility.Visible;

                    if (!tag.IsSubPrototypeMember)
                    {
                        checkBoxRetentive.Visibility = Visibility.Visible;
                        textBlockRetentive.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        checkBoxRetentive.Visibility = Visibility.Collapsed;
                        textBlockRetentive.Visibility = Visibility.Collapsed;
                    }

                    textDynamicSettings.Visibility = Visibility.Visible;
                    contentDynamic.Visibility = Visibility.Visible;
                }

                if (comboEngineeringUnit.ItemsSource == null)
                {
                    using (new WaitCursor())
                    {
                        var eunits = new List<String>();
                        eunits.Add(String.Empty);
                        eunits.AddRange(Document.GetEngineeringUnitNames());
                        comboEngineeringUnit.ItemsSource = eunits;
                    }
                }
            }
            else if (tag.ModelType == UFUAModel.ModelType.Method)
            {
                textEnumStrings.Visibility = Visibility.Collapsed;
                //textEditEnumStrings.Visibility = Visibility.Collapsed;
                dockEnumStrings.Visibility = Visibility.Collapsed;

                textBlockArrayDimension.Visibility = Visibility.Collapsed;
                checkBoxRetentive.Visibility = Visibility.Collapsed;
                textBlockRetentive.Visibility = Visibility.Collapsed;
                textArrayDimension.Visibility = Visibility.Collapsed;

                comboDataType.Visibility = Visibility.Collapsed;
                textDataType.Visibility = Visibility.Collapsed;

                comboPrototype.Visibility = Visibility.Collapsed;
                textPrototype.Visibility = Visibility.Collapsed;

                textBlockInitialValue.Visibility = Visibility.Collapsed;
                textInitialValue.Visibility = Visibility.Collapsed;

                textDynamicSettings.Visibility = Visibility.Visible;
                contentDynamic.Visibility = Visibility.Visible;
            }
            else if (tag.ModelType == UFUAModel.ModelType.Enumerated)
            {
                textEnumStrings.Visibility = Visibility.Visible;
                //textEditEnumStrings.Visibility = Visibility.Visible;
                dockEnumStrings.Visibility = Visibility.Visible;

                textBlockArrayDimension.Visibility = Visibility.Visible;
                textArrayDimension.Visibility = Visibility.Visible;

                comboDataType.Visibility = Visibility.Collapsed;
                textDataType.Visibility = Visibility.Collapsed;

                comboPrototype.Visibility = Visibility.Collapsed;
                textPrototype.Visibility = Visibility.Collapsed;

                if (IsPrototype)
                {
                    textBlockInitialValue.Visibility = Visibility.Collapsed;
                    textInitialValue.Visibility = Visibility.Collapsed;

                    checkBoxRetentive.Visibility = Visibility.Collapsed;
                    textBlockRetentive.Visibility = Visibility.Collapsed;

                    textDynamicSettings.Visibility = Visibility.Collapsed;
                    contentDynamic.Visibility = Visibility.Collapsed;
                }
                else
                {
                    textBlockInitialValue.Visibility = Visibility.Visible;
                    textInitialValue.Visibility = Visibility.Visible;

                    if (!tag.IsSubPrototypeMember)
                    {
                        checkBoxRetentive.Visibility = Visibility.Visible;
                        textBlockRetentive.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        checkBoxRetentive.Visibility = Visibility.Collapsed;
                        textBlockRetentive.Visibility = Visibility.Collapsed;
                    }


                    textDynamicSettings.Visibility = Visibility.Visible;
                    contentDynamic.Visibility = Visibility.Visible;
                }
            }
            else
            {
                textEnumStrings.Visibility = Visibility.Collapsed;
                //textEditEnumStrings.Visibility = Visibility.Collapsed;
                dockEnumStrings.Visibility = Visibility.Collapsed;

                textBlockArrayDimension.Visibility = Visibility.Collapsed;
                checkBoxRetentive.Visibility = Visibility.Collapsed;
                textBlockRetentive.Visibility = Visibility.Collapsed;
                textArrayDimension.Visibility = Visibility.Collapsed;

                comboDataType.Visibility = Visibility.Collapsed;
                textDataType.Visibility = Visibility.Collapsed;

                comboPrototype.Visibility = Visibility.Collapsed;
                textPrototype.Visibility = Visibility.Collapsed;

                textBlockInitialValue.Visibility = Visibility.Collapsed;
                textInitialValue.Visibility = Visibility.Collapsed;

                textDynamicSettings.Visibility = Visibility.Collapsed;
                contentDynamic.Visibility = Visibility.Collapsed;
            }

            textEngineeringUnit.Visibility = tag.IsEngineeringUnitSupported ? Visibility.Visible : Visibility.Collapsed;
            comboEngineeringUnit.Visibility = tag.IsEngineeringUnitSupported ? Visibility.Visible : Visibility.Collapsed;
        }

        private void comboDataType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
            var tag = DataContext as UFUAModel.UFUATag;
            textEngineeringUnit.Visibility = tag.IsEngineeringUnitSupported ? Visibility.Visible : Visibility.Collapsed;
            comboEngineeringUnit.Visibility = tag.IsEngineeringUnitSupported ? Visibility.Visible : Visibility.Collapsed;
        }
        
        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            var enumStrings = button.Tag as XPCollection<UFUAModel.UFUAEnumString>;
            if (enumStrings != null)
            {
                var listEnum = (from c in enumStrings orderby c.Oid select c.Data).ToList();
                var control = new ListEnumStringsEditor(listEnum);
                var Dialog = new GeneralDialogContent(control)
                {
                    DialogKeepContent = true,
                    Title = Properties.Resources.EnumStringsTitle,
                    Owner = this.FindParent<Window>(),
                    HelpLink = "EnumStringsEditor"
                };
                if (Dialog.ShowDialog() == true)
                {
                    bool update = (listEnum.Count != control.CurrentEnums.Length);

                    var doc = UFUAEditorManagerComponent.ufuaEditorManagerComponent.Workspace.ContextDocument as UFUAServerDocument;
                    if (doc != null)
                    {
                        var session = enumStrings.Session;
                        var tagref = (from tag in new XPQuery<UFUAModel.UFUATag>(session, true).AsParallel()
                                      where tag.EnumStrings == enumStrings
                                      select tag).SingleOrDefault();

                        while (enumStrings.Count > 0)
                            enumStrings[0].Delete();

                        foreach (var item in control.CurrentEnums)
                            tagref.EnumStrings.Add(new UFUAModel.UFUAEnumString(session) { Data = item });

                        //if (update)
                        textEditEnumStrings.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                    }
                }
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            var enumStrings = button.Tag as XPCollection<UFUAModel.UFUAEnumString>;
            if (enumStrings != null && enumStrings.Count > 0)
            {
                while (enumStrings.Count > 0)
                    enumStrings[0].Delete();

                //uriLabel.GetBindingExpression(Label.ContentProperty).UpdateTarget();
                textEditEnumStrings.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            }
        }

    }
}
