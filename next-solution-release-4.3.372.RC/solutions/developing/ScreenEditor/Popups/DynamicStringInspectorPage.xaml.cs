using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ScreenSettings;
using Utilities.WPF;
using System.ComponentModel;
using System.Windows.Controls;
using WPFUtilities.PropertyDataTemplate;
using IWorkspace = UFInterfaces.IWorkspace;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Grid;

namespace ScreenManager.Popups
{
    /// <summary>
    /// Interaction logic for DynamicStringInspectorPage.xaml
    /// </summary>
    public partial class DynamicStringInspectorPage : UserControl /*, IContainPropertyEditors*/
    {
        bool bLoaded;

        public class StringsData : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            string desc;
            string text;
            string propName;
            string originalText;
            UIElement elem;
            public bool TextChanged
            {
                get
                {
                    return originalText != text;
                }
            }
            public string StringDescription { 
                get
                {
                    return desc;
                }
                set
                {
                    if (desc != value)
                    {
                        desc = value;
                        OnPropertyChanged("StringDescription");
                    }
                }
            }
            public string StringText
            {
                get
                {
                    return text;
                }
                set
                {
                    if (text != value)
                    {
                        text = value;
                        OnPropertyChanged("StringText");
                    }
                }
            }
            public string PropertyName
            {
                get
                {
                    return propName;
                }
                set
                {
                    if (propName != value)
                    {
                        propName = value;
                        OnPropertyChanged("PropertyName");
                    }
                }
            }
            public string EntityName { get; }
            public UIElement Element
            {
                get
                {
                    return elem;
                }
                set
                {
                    if (elem != value)
                    {
                        elem = value;
                        OnPropertyChanged("Element");
                    }
                }
            }

            public StringsData(UIElement elem, string desc, string text, string propName, string entityName)
            {
                Element = elem;
                StringDescription = desc;
                originalText = StringText = text;
                PropertyName = propName;
                EntityName = entityName;
            }
            void OnPropertyChanged(string name)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        ObservableCollection<StringsData> selection = new ObservableCollection<StringsData>();
        public DynamicStringInspectorPage(ScreenDocument document, List<String> items, Dictionary<string, Tuple<DependencyProperty, string>> textsList, Dictionary<string, UIElement> elementsList, List<StringsData> editedStringsData)
        {
            InitializeComponent();

            Loaded += (ob, ev) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;
                    var wnd = this.FindParent<Window>();
                    var selectionPreTags = new Dictionary<String, String>();
                    var selectionAfterTags = new Dictionary<String, String>();
                    var mapNodeToNames = new Dictionary<String, List<String>>();
                    var mapNameToRefences = new Dictionary<String, String>();
                    textsList.Keys.ToList().ForEach(item =>
                        {
                            if (!mapNodeToNames.ContainsKey(item))
                                mapNodeToNames.Add(item, new List<String>());
                            if (!mapNameToRefences.ContainsKey(item))
                                mapNameToRefences.Add(item, textsList[item].Item2);
                            var title = document.CleanInnerName(item);
                            if (!mapNodeToNames[item].Contains(title))
                                mapNodeToNames[item].Add(title);
                        });

                    int nCounter = 0;
                    var builder = new StringBuilder();
                    mapNodeToNames.Keys.ToList().ForEach(item =>
                        {
                            builder.Clear();
                            foreach (var i in mapNodeToNames[item])
                            {
                                if (builder.Length == 0)
                                    builder.Append(String.Format(Properties.Resources.DynPropUsed, ++nCounter));
                                else
                                    builder.Append(", ");
                                builder.Append(i);
                            }
                            var sd = new StringsData(elementsList[item], builder.ToString(), mapNameToRefences[item], textsList[item].Item1.Name, item);
                            sd.PropertyChanged += OnStringChanged;
                            selection.Add(sd);
                        });

                    (mainGrid.View as DataViewBase).ShowEmptyText = true;
                    mainGrid.ItemsSource = selection;
                    var workspace = document.GetService(typeof(IWorkspace)) as IWorkspace;

                    var dt = new DataTemplate() { DataType = typeof(string) };
                    var factory = new FrameworkElementFactory(typeof(TextPropertyEditor));
                    factory.SetValue(TextPropertyEditor.WorkspaceProperty, workspace);
                    factory.SetValue(TextPropertyEditor.BindingPathProperty, "RowData.Row.StringText");
                    factory.SetValue(TextPropertyEditor.TextUIElementProperty, "RowData.Row.Element");
                    factory.SetValue(TextPropertyEditor.PropertyNameProperty, "RowData.Row.PropertyName");
                    dt.VisualTree = factory;
                    textGridColumn.CellTemplate = dt;

                    if (wnd != null)
                    {
                        wnd.Closing += (o, e) =>
                        {
                            foreach (var sd in selection)
                            {
                                sd.PropertyChanged -= OnStringChanged;
                            }
                            editedStringsData.Clear();
                        };
                    }
                }
            };

            void OnStringChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "StringText")
                {
                    var sd = (StringsData)sender;
                    if (!editedStringsData.Contains(sd))
                        editedStringsData.Add(sd);
                }
            }
        }
    }
}
