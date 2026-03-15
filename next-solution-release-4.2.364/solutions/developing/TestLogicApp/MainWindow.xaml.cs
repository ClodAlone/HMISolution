using LogicCore;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace TestLogicApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var types = GateData.LoadGateDataTypes();
            var templateDictionary = new DataTemplateDictionary();
            types.ForEach(type =>
            {
                var gateData = GateData.CreateFrom(type);
                var templates = gateData.GetDataTemplates();
                if (templates != null)
                {
                    foreach (var template in templates)
                        templateDictionary.Add(template.Key, template.Value);
                }
            });

            myDiagram.NodeTemplateDictionary = templateDictionary;
            var model = new GraphLinksModel<GateData, String, String, WireData>();
            model.NodeCategoryPath = GateData.NodeCategoryPath;
            model.Modifiable = true;  // let the user modify the graph
            model.HasUndoManager = true;  // support undo/redo
            myDiagram.Model = model;

            var mapRibbonBars = new Dictionary<String, ContentControl>();
            var mapModels = new Dictionary<String, GraphLinksModel<GateData, String, String, WireData>>();
            var mapNodeSourceModels = new Dictionary<String, ObservableCollection<GateData>>();
            var mapDataTemplates = new Dictionary<String, DataTemplateDictionary>();
            types.ForEach(type =>
            {
                var gateData = GateData.CreateFrom(type);
                var category = gateData.GetCategory();
                if (!mapRibbonBars.ContainsKey(category))
                {
                    var ribbonBar = new ContentControl();
                    mapRibbonBars.Add(category, ribbonBar);

                    mapModels.Add(category, new GraphLinksModel<GateData, String, String, WireData>());
                    mapModels[category].NodeCategoryPath = GateData.NodeCategoryPath;

                    mapDataTemplates.Add(category, new DataTemplateDictionary());
                    mapNodeSourceModels.Add(category, new ObservableCollection<GateData>());
                    mapModels[category].NodesSource = mapNodeSourceModels[category];
                    mapModels[category].LinksSource = new List<WireData>();
                }
                var templates = gateData.GetDataTemplates();
                if (templates != null)
                {
                    foreach (var template in templates)
                        mapDataTemplates[category].Add(template.Key, template.Value);
                }
                foreach(var nodesource in gateData.GetTypes())
                    mapNodeSourceModels[category].Add(nodesource);
            });

            foreach (var key in mapRibbonBars)
            {
                var category = key.Key;
                var item = TryFindResource("paletteDataTemplate") as DataTemplate;
                var palette = item.LoadContent() as Palette;
                palette.NodeTemplateDictionary = mapDataTemplates[category];
                palette.Model = mapModels[category];
                palette.Height = 100 * mapNodeSourceModels[category].Count;
                key.Value.Content = palette;
                stackPalettes.Children.Add(key.Value);
            }
        }
    }
}
