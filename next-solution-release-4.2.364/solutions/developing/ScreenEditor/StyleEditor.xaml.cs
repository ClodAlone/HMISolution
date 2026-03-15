using System;
using System.Collections.Generic;
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
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid.TreeList;
using UIMsgBoxAlertService.ComponentService;
using System.Collections.ObjectModel;
using Utilities;
using System.IO;
using WPFUtilities;

namespace ScreenManager
{
    /// <summary>
    /// Interaction logic for StyleEditor.xaml
    /// </summary>
    public partial class StyleEditor : UserControl
    {
        readonly IUIMsgBoxAlertService UIService;
        Dictionary<Object, Object> mapResourceStyles;
        readonly Dictionary<FrameworkElement, Object> mapCreatedControls = new Dictionary<FrameworkElement, Object>();
        readonly Type TargetType;
        readonly ObservableCollection<FrameworkElement> listStyledObject = new ObservableCollection<FrameworkElement>();
        bool bLoaded;

        public event EventHandler StyleChanged;
        public Style StyleEdit { get; set; }
        public String ResourceName { get; set; }
        public String ResourceFileName { get; set; }

        public StyleEditor(Type targetType, IUIMsgBoxAlertService uiService)
        {
            InitializeComponent();

            TargetType = targetType;
            UIService = uiService;

            Loaded += (o, e) =>
                {
                    if (!bLoaded)
                    {
                        bLoaded = true;
                        //using (new WaitCursor())
                        //{
                        //    String startingPath = String.Format("{0}\\Styles\\{1}",
                        //        ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"), targetType.Name);

                        //    try
                        //    {
                        //        string[] directoryGetFiles = Directory.GetFiles(startingPath, "*.xaml");
                        //        Array.ForEach(directoryGetFiles, AddTreeItem);
                        //    }
                        //    catch (Exception ex)
                        //    {
                                
                        //    }
                        //}
                    }
                };
        }

        void treeListControl_SelectionChanged(object sender, TreeListSelectionChangedEventArgs e)
        {
            e.Handled = true;
            TreeListNode item = e.Node as TreeListNode;
            if (item == null || item.Tag == null || !(item.Tag is String))
                return;

            LoadStyles(item.Tag as String);
        }

        private void AddTreeItem(String item)
        {
            FileInfo file = new FileInfo(item);
            string header = System.IO.Path.GetFileNameWithoutExtension(file.Name); 
            var ic = new TreeItemControl(item, header);
            var newitem = treeListControl.AddNode(ic, null, item);
        }

        private void LoadStyles(string file)
        {
            using (new WaitCursor())
            {
                ResourceFileName = file;
                var map = ResourceDictionaryExtensions.LoadFromFile(ResourceFileName, typeof(Style));
                FillResourceList(map);

                FillList(listStyledObject);
            }
        }

        private void FillList(ObservableCollection<FrameworkElement> list)
        {
            using (new WaitCursor())
            {
                foreach (var uie in Carousel.Children)
                {
                    var content = uie as ContentControl;
                    var btn = content.Content as Button;
                    btn.Content = null;
                }

                Carousel.Children.Clear();
                DataTemplate item = TryFindResource("ItemTemplate") as DataTemplate;
                foreach (var uie in list)
                {
                    var btn = item.LoadContent() as Button;
                    btn.Content = uie;
                    Carousel.Children.Add(new ContentControl() { Content = btn });
                }
                Carousel.ReInitialize();
                // if (list.Count > 0)
                //    Carousel.SelectElement(list[0] as FrameworkElement);
            }
        }

        private void FillResourceList(Dictionary<Object, Object> map)
        {
            var list = (from entry in map.Values.OfType<Style>()/*.AsParallel()*/ select entry).ToList();
            var listType = (from type in list where type.TargetType == TargetType select type).ToList();

            listStyledObject.Clear();
            mapCreatedControls.Clear();
            listType.ForEach(style =>
                {
                    FrameworkElement uie = Activator.CreateInstance(TargetType) as FrameworkElement;
                    if (uie != null)
                    {
                        var key = (from c in map where c.Value == style select c.Key).FirstOrDefault();
                        uie.Style = style;
                        if (uie is ContentControl)
                            (uie as ContentControl).Content = key;
                        listStyledObject.Add(uie);

                        mapCreatedControls.Add(uie, key);
                    }
                });
            mapResourceStyles = map;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String file = UIService.ShowOpenFileDialog(Properties.Settings.Default.ResourceDictionaryFileType);
            if (String.IsNullOrEmpty(file))
                return;

            try
            {
                var map = ResourceDictionaryExtensions.LoadFromFile(file, typeof(Style));
                FillResourceList(map);

                txtNoLibrary.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                UIService.ShowError(ex.Message);
            }
        }

        #region OnStyleChanged
        /// <summary>
        /// Triggers the BrushChanged event.
        /// </summary>
        public virtual void OnStyleChanged(Style style)
        {
            StyleEdit = style;

            var temp = StyleChanged;
            if (temp != null)
                temp(null/*this*/, new EventArgs());
        }
        #endregion

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var content = sender as ContentControl;
            var uie = content.Content as FrameworkElement;
            if (!mapCreatedControls.ContainsKey(uie))
                return;
            var key = mapCreatedControls[uie];
            ResourceName = key as String;
            OnStyleChanged(mapResourceStyles[key] as Style);
        }

        private void Button_Prev(object sender, RoutedEventArgs e)
        {
            Carousel.SelectPrev();
        }

        private void Button_Next(object sender, RoutedEventArgs e)
        {
            Carousel.SelectNext();
        }
    }
}
