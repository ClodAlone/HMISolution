using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
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
using ScreenSettings;
using UFInterfaces.Scriptable;
using Utilities;
using Utilities.WPF;
using PropertyControl.ComponentService;
using UFInterfaces;
using UFUAEditor.ComponentService;
using System.Text.RegularExpressions;
using System.ComponentModel;
using ExpressionManager;
using OPCUAViewModel;

namespace ScreenManager.Popups
{
    /// <summary>
    /// Interaction logic for DynamicPropertyInspectorPage.xaml
    /// </summary>
    public partial class DynamicPropertyInspectorPage : UserControl
    {
        //bool bLoaded;
        IUFUAEditorManager UFUAEditor;
        ScreenDocument document;
        Action applyChanges;
        internal bool isChanged;
        private readonly ILocalTagParser _localTagParser = new LocalTagParser();

        public DynamicPropertyInspectorPage(ScreenDocument document, String item,
            IPropertyControl editor, IDictionary<IScriptable, Grid> scriptControls)
        {
            InitializeComponent();

            this.document = document;

            SelectedPage += (ob, ev) =>
                {
                    //if (!bLoaded)
                    {
                        //bLoaded = true;
                        //var wnd = this.FindParent<Window>(); 

                        var control = editor.controlNoSelection;
                        control.ClearValue(FrameworkElement.WidthProperty);
                        control.ClearValue(FrameworkElement.HeightProperty);
                        contentControlProperty.Content = control;
                        var ret = document.MapScreenEntities[item].GetMapDynamics();
                        var selection = new Dictionary<String, Object>();
                        var selectionPreTags = new Dictionary<String, String>();
                        var selectionAfterTags = new Dictionary<String, String>();

                        foreach (var key in ret.Keys)
                            selection.Add(key, ret[key]);

                        var retExpressions = document.MapScreenEntities[item].GetMapDynamicExpressions();
                        foreach (var key in retExpressions.Keys)
                        {
                            if (retExpressions[key] == null)
                                selection.Add(key, String.Empty);
                            else
                                selection.Add(key, retExpressions[key]);
                        }

                        if (document.MapScreenEntities[item].Element is IDynamicTagAware)
                        {
                            var dyamicAware = document.MapScreenEntities[item].Element as IDynamicTagAware;
                            var dynamicMap = dyamicAware.GetMapDynamics();
                            foreach (var key in dynamicMap.Keys)
                            {
                                var entity = dynamicMap[key].FromXml<OPCUAEntityReference>();
                                selection.Add(key, entity);
                            }
                        }

                        var mapTagValues = (from key in selection
                                            where key.Value is OPCUAEntityReference
                                            select key).ToList();
                        mapTagValues.ForEach(key => selectionPreTags.Add(key.Key, key.Value.ToXml()));

                        //if (wnd != null)
                        {
                            bool bRestore = document.NeedsSave;
                            int placeHolderCounter = 0;
                            string stringId = Properties.Resources.Expression;
                            if (document.MapScreenEntities[item].Expression == null)
                                document.MapScreenEntities[item].Expression = String.Empty;
                            while(selection.ContainsKey(stringId))
                            {
                                stringId = string.Format("{0}({1})", stringId, placeHolderCounter + 1);
                                ++placeHolderCounter;
                            }
                            selection.Add(stringId, document.MapScreenEntities[item].Expression);
                            stringId = Properties.Resources.Reverse;
                            placeHolderCounter = 0;
                            if (document.MapScreenEntities[item].ReverseExpression == null)
                                document.MapScreenEntities[item].ReverseExpression = String.Empty;
                            while (selection.ContainsKey(stringId))
                            {
                                stringId = string.Format("{0}({1})", stringId, placeHolderCounter + 1);
                                ++placeHolderCounter;
                            }
                            selection.Add(stringId, document.MapScreenEntities[item].ReverseExpression);

                            document.NeedsSave = bRestore;
                        }
                        editor.SetControlSelection(control, selection);

                        //if (wnd != null)
                        {
                            //wnd.Closing += (o, e) =>
                            applyChanges = new Action(() =>
                            {
                                document.MapScreenEntities[item].Expression = selection[Properties.Resources.Expression] as String;
                                document.MapScreenEntities[item].ReverseExpression = selection[Properties.Resources.Reverse] as String;
                                document.MapScreenEntities[item].SetMapDynamicExpressions(selection);

                                var mapTagChanged = new Dictionary<String, String>();
                                mapTagValues.ForEach(key =>
                                {
                                    if (selectionAfterTags.ContainsKey(key.Key))
                                        selectionAfterTags.Remove(key.Key);
                                    selectionAfterTags.Add(key.Key, key.Value.ToXml());
                                });
                                foreach (var tag in selectionPreTags.Keys)
                                {
                                    if (!selectionAfterTags.ContainsKey(tag))
                                        continue;
                                    if (selectionPreTags[tag] != selectionAfterTags[tag])
                                    {
                                        if (mapTagChanged.ContainsKey(selectionPreTags[tag]))
                                            mapTagChanged.Remove(selectionPreTags[tag]);
                                        mapTagChanged.Add(selectionPreTags[tag], selectionAfterTags[tag]);
                                    }
                                }

                                if (document.MapScreenEntities[item].Element is IDynamicTagAware)
                                {
                                    var dyamicAware = document.MapScreenEntities[item].Element as IDynamicTagAware;

                                    if (mapTagChanged.Count > 0)
                                        dyamicAware.UpdateMapDynamics(mapTagChanged);
                                }


                                if (mapTagChanged.Count > 0)
                                    isChanged = true;

                                //if (mapTagChanged.Count > 0)
                                //{
                                //    if (wnd.DialogResult == true)
                                //        document.NeedsSave = true;
                                //}
                            });
                        }

                        if (scriptControls != null)
                        {
                            if (scriptControls.ContainsKey(document.MapScreenEntities[item]))
                            {
                                ShowCustomExpander(scriptControls[document.MapScreenEntities[item]]);
                            }
                            else
                                ShowCustomExpander(null);
                        }
                    }
                };

            UnSelectedPage += (ob, ev) =>
            {
                if (contentControlProperty.Content is IDisposable)
                    (contentControlProperty.Content as IDisposable).Dispose();
                contentControlProperty.Content = null;
                if (applyChanges != null)
                    applyChanges();
            };
        }

        String GetReferenceName(OPCUAEntityReference reference)
        {
            if (reference.ResolvedNodeId != null)
                return reference.ResolvedNodeId.ToString();
            else if (reference.StartingAddress != null)
                return String.Format("{0}-{1}", reference.StartingAddress, reference.RelativePath);

            return reference.HumanReadable;
        }

        Dictionary<string, string> GetEntityExpressions(ScreenSettings.Entities.ScreenEntity entity)
        {
            var retExp = entity.GetMapDynamicExpressions();
            if (entity.Expression != null)
                retExp.Add(nameof(ScreenSettings.Entities.ScreenEntity.Expression), entity.Expression);
            if (entity.ReverseExpression != null)
                retExp.Add(nameof(ScreenSettings.Entities.ScreenEntity.ReverseExpression), entity.ReverseExpression);
            return retExp;
        }

        public DynamicPropertyInspectorPage(ScreenDocument document, List<String> items,
            IPropertyControl editor)
        {
            InitializeComponent();

            this.document = document;

            SelectedPage += (ob, ev) =>
            {
                //if (!bLoaded)
                {
                    //bLoaded = true;
                    //var wnd = this.FindParent<Window>();

                    var control = editor.controlNoSelection;
                    control.ClearValue(FrameworkElement.WidthProperty);
                    control.ClearValue(FrameworkElement.HeightProperty);
                    contentControlProperty.Content = control;
                    var selection = new Dictionary<String, Object>();
                    var selectionPreTags = new Dictionary<String, String>();
                    var selectionAfterTags = new Dictionary<String, String>();
                    var mapNodeToNames = new Dictionary<String, List<String>>();
                    var mapNameToRefences = new Dictionary<String, OPCUAEntityReference>();
                    items.ForEach(item =>
                        {
                            var ret = document.MapScreenEntities[item].GetMapDynamics();
                            foreach (var key in ret.Keys)
                            {
                                String name = GetReferenceName(ret[key]);
                                if (String.IsNullOrEmpty(name))
                                    continue;

                                if (!mapNodeToNames.ContainsKey(name))
                                    mapNodeToNames.Add(name, new List<String>());
                                if (!mapNameToRefences.ContainsKey(name))
                                    mapNameToRefences.Add(name, ret[key]);
                                var title = document.CleanInnerName(item);
                                if (!mapNodeToNames[name].Contains(title))
                                    mapNodeToNames[name].Add(title);
                            }
                            
                            foreach (var val in GetEntityExpressions(document.MapScreenEntities[item]).Values.ToList())
                            {
                                if (string.IsNullOrEmpty(val))
                                    continue;
                                using (var expConv = new Utilities.Converters.ExpressionValueConverter(val))
                                {
                                    expConv.ParseFormula();
                                    foreach (var tag in expConv.GetAllParsedVariables())
                                    {
                                        var sanitizedTagName = NamespaceTableConverter.GetSanitizedReadableValue(tag);
                                        var opcua = GetOPCUAEntityReference(sanitizedTagName);
                                        if (opcua != null)
                                        {
                                            String name = GetReferenceName(opcua);
                                            if (String.IsNullOrEmpty(name))
                                                continue;

                                            if (!mapNodeToNames.ContainsKey(name))
                                                mapNodeToNames.Add(name, new List<String>());
                                            if (!mapNameToRefences.ContainsKey(name))
                                                mapNameToRefences.Add(name, opcua);
                                            var title = document.CleanInnerName(item);
                                            if (!mapNodeToNames[name].Contains(title))
                                                mapNodeToNames[name].Add(title);
                                        }
                                    }
                                }
                            }

                            if (document.MapScreenEntities[item].Element is IDynamicTagAware)
                            {
                                var dyamicAware = document.MapScreenEntities[item].Element as IDynamicTagAware;
                                var dynamicMap = dyamicAware.GetMapDynamics();
                                foreach (var key in dynamicMap.Keys)
                                {
                                    var entity = dynamicMap[key].FromXml<OPCUAEntityReference>();
                                    String name = GetReferenceName(entity);
                                    if (String.IsNullOrEmpty(name))
                                        continue;
                                    if (!mapNodeToNames.ContainsKey(name))
                                        mapNodeToNames.Add(name, new List<String>());
                                    if (!mapNameToRefences.ContainsKey(name))
                                        mapNameToRefences.Add(name, entity);
                                    var title = document.CleanInnerName(item);
                                    if (!mapNodeToNames[name].Contains(title))
                                        mapNodeToNames[name].Add(title);
                                }
                            }
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
                            selection.Add(builder.ToString(), mapNameToRefences[item]);

                            mapNameToRefences[item].PropertyChanged += DynamicPropertyInspectorPage_PropertyChanged;
                        });

                    editor.SetControlSelection(control, selection);

                    var mapTagValues = (from key in selection
                                        where key.Value is OPCUAEntityReference
                                        select key).ToList();
                    mapTagValues.ForEach(key => selectionPreTags.Add(key.Key, key.Value.ToXml()));

                    //if (wnd != null)
                    {
                        //wnd.Closing += (o, e) =>
                        applyChanges = new Action(() =>
                        {
                            var mapTagChanged = new Dictionary<String, String>();
                            mapTagValues.ForEach(key => selectionAfterTags.Add(key.Key, key.Value.ToXml()));
                            foreach (var tag in selectionPreTags.Keys)
                            {
                                if (!selectionAfterTags.ContainsKey(tag))
                                    continue;
                                if (selectionPreTags[tag] != selectionAfterTags[tag] && !mapTagChanged.ContainsKey(selectionPreTags[tag]))
                                    mapTagChanged.Add(selectionPreTags[tag], selectionAfterTags[tag]);
                            }

                            mapNodeToNames.Keys.ToList().ForEach(item =>
                                {
                                    mapNameToRefences[item].PropertyChanged -= DynamicPropertyInspectorPage_PropertyChanged;
                                });
                            listChanged.ForEach(ent =>
                                {
                                    var found = (from c in mapNameToRefences where Object.ReferenceEquals(c.Value, ent) select c.Key).ToList();
                                    if (found.Count > 0)
                                    {
                                        items.ForEach(item =>
                                            {
                                                var ret = document.MapScreenEntities[item].GetMapDynamics();
                                                foreach (var key in ret.Keys)
                                                {
                                                    String name = GetReferenceName(ret[key]);
                                                    if (String.IsNullOrEmpty(name))
                                                        continue;
                                                    if (name == found[0])
                                                        ret[key].UpdateValue(ent);
                                                }

                                                Dictionary<string, object> newselection = new Dictionary<string, object>();
                                                var retExp = GetEntityExpressions(document.MapScreenEntities[item]);
                                                foreach (var exprName in retExp.Keys)
                                                {
                                                    var oldExpr = retExp[exprName];
                                                    if (string.IsNullOrEmpty(oldExpr))
                                                        continue;
                                                    using (var expConv = new Utilities.Converters.ExpressionValueConverter(oldExpr))
                                                    {
                                                        expConv.ParseFormula();
                                                        foreach (var tag in expConv.GetAllParsedVariables())
                                                        {
                                                            var sanitizedTagName = NamespaceTableConverter.GetSanitizedReadableValue(tag);
                                                            var opcua = GetOPCUAEntityReference(sanitizedTagName);
                                                            if (opcua != null)
                                                            {
                                                                String name = GetReferenceName(opcua);
                                                                if (String.IsNullOrEmpty(name))
                                                                    continue;

                                                                if (name == found[0])
                                                                {
                                                                    string newExpr = oldExpr;
                                                                    var oldPathPositions = Regex.Matches(oldExpr, Regex.Escape(opcua.RelativePath), RegexOptions.IgnoreCase).Cast<Match>().Select(m => m.Index);
                                                                    foreach (var pos in oldPathPositions)
                                                                        newExpr = String.Format("{0}{1}{2}", newExpr.Substring(0, pos), ent.RelativePath, newExpr.Substring(pos + opcua.RelativePath.Length));
                                                                    if (exprName == nameof(ScreenSettings.Entities.ScreenEntity.Expression))
                                                                        document.MapScreenEntities[item].Expression = newExpr;
                                                                    else if (exprName == nameof(ScreenSettings.Entities.ScreenEntity.ReverseExpression))
                                                                        document.MapScreenEntities[item].ReverseExpression = newExpr;
                                                                    else
                                                                        newselection.Add(exprName, newExpr);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                if (newselection.Count > 0)
                                                    document.MapScreenEntities[item].SetMapDynamicExpressions(newselection);

                                                if (document.MapScreenEntities[item].Element is IDynamicTagAware)
                                                {
                                                    var dyamicAware = document.MapScreenEntities[item].Element as IDynamicTagAware;
                                                    var dynamicMap = dyamicAware.GetMapDynamics();

                                                    if (mapTagChanged.Count > 0)
                                                        dyamicAware.UpdateMapDynamics(mapTagChanged);

                                                    foreach (var key in dynamicMap.Keys)
                                                    {
                                                        var entity = dynamicMap[key].FromXml<OPCUAEntityReference>();
                                                        String name = GetReferenceName(entity);
                                                        if (String.IsNullOrEmpty(name))
                                                            continue;
                                                        if (name == found[0])
                                                            entity.UpdateValue(ent);
                                                    }
                                                }
                                            });
                                    }
                                });

                            if (listChanged.Count > 0)
                                isChanged = true;

                            //if (wnd.DialogResult == true && (listChanged.Count > 0 || bExpressionsChanged))
                            //    document.NeedsSave = true;
                        });
                    }
                }
            };

            UnSelectedPage += (ob, ev) =>
            {
                if (contentControlProperty.Content is IDisposable)
                    (contentControlProperty.Content as IDisposable).Dispose();
                contentControlProperty.Content = null;
                if (applyChanges != null)
                    applyChanges();
            };
        }

        private OPCUAEntityReference GetOPCUAEntityReference(string tag)
        {
            WPFUtilities.SmartControlHelper.GetInstanceName(tag, out var instance, out var name);

            if (UFUAEditor == null)
            {
                if (document == null)
                    return null;
                UFUAEditor = document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                if (UFUAEditor == null)
                    return null;
            }
            var xml = UFUAEditor.GetTagEntityReference(document, name, instance);
            return string.IsNullOrEmpty(xml) 
                ? _localTagParser.Parse(tag) 
                : xml.FromXml<OPCUAEntityReference>();
        }

        event EventHandler SelectedPage;
        void OnSelectedPage()
        {
            var temp = SelectedPage;
            if (temp != null)
                temp(this, EventArgs.Empty);
        }

        internal void Selected()
        {
            OnSelectedPage();
        }

        event EventHandler UnSelectedPage;
        void OnUnSelectedPage()
        {
            var temp = UnSelectedPage;
            if (temp != null)
                temp(this, EventArgs.Empty);
        }

        internal void UnSelected()
        {
            OnUnSelectedPage();
        }

        List<OPCUAEntityReference> listChanged = new List<OPCUAViewModel.OPCUAEntityReference>();
        void DynamicPropertyInspectorPage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var entity = sender as OPCUAEntityReference;
            if (entity == null || listChanged.Contains(entity))
                return;
            listChanged.Add(entity);
        }

        internal void ShowCustomExpander(Grid control)
        {
            contentControl.Content = control;
            if (control != null)
            {
                var height = ActualHeight / 2;
                control.ClearValue(FrameworkElement.WidthProperty);
                control.ClearValue(FrameworkElement.HeightProperty);

                rowSplitter.Height = new GridLength(5);
                rowContent.Height = new GridLength(height);
                contentControl.Visibility = Visibility.Visible;
            }
            else
            {
                rowSplitter.Height = new GridLength(0);
                rowContent.Height = new GridLength(0);
                contentControl.Visibility = Visibility.Collapsed;
            }
        }
    }
}
