using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentManager.ComponentService;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using UFUAEditor.ComponentService;
using System.Windows.Controls;
using UFUAEditor.Document;
using Utilities;
using System.ComponentModel;
using WPFUtilities;

namespace UFUAEditor.TreeDocumentManagers
{
    class TreeAlarmPrototypesDocManager : TreeChangedDocument
    {
        #region Declarations
        UFUAModel.UFUAArea Area;
        UFUAModel.UFUAAlarmSource Source;
        UFUAModel.UFUAAlarmDefinition Definition;
        UFUAModel.UFUAAlarmThreshold Threshold;
        readonly IDocumentManager Parent;
        #endregion

        #region Constructors
        internal TreeAlarmPrototypesDocManager(UFUAEditorManagerComponent c, UFUAServerDocument doc) 
            : base(c, doc)
        { }


        TreeAlarmPrototypesDocManager(UFUAEditorManagerComponent c, UFUAServerDocument doc,
            UFUAModel.UFUAArea area, IDocumentManager parent) 
            : base(c, doc)
        {
            Area = area;
            Parent = parent;
        }

        TreeAlarmPrototypesDocManager(UFUAEditorManagerComponent c, UFUAServerDocument doc,
            UFUAModel.UFUAAlarmSource source, IDocumentManager parent) 
            : base(c, doc)
        {
            Source = source;
            Parent = parent;
        }

        TreeAlarmPrototypesDocManager(UFUAEditorManagerComponent c, UFUAServerDocument doc,
            UFUAModel.UFUAAlarmDefinition definition, IDocumentManager parent) 
            : base(c, doc)
        {
            Definition = definition;
            Parent = parent;
        }

        TreeAlarmPrototypesDocManager(UFUAEditorManagerComponent c, UFUAServerDocument doc,
            UFUAModel.UFUAAlarmThreshold threshold, IDocumentManager parent) 
            : base(c, doc)
        {
            Threshold = threshold;
            Parent = parent;
        }
        #endregion

        #region Overrides
        protected override void FillChilds(bool bRefresh, bool bReload)
        {
            if (bReload)
            {
                if (Area != null)
                    Area = Document.FindAlarmAreaByNodeId(Area.NodeId);
                else if (Source != null)
                    Source = Document.FindAlarmSourceAreaByNodeId(Source.NodeId);
                else if (Definition != null)
                    Definition = Document.FindAlarmDefinitionByNodeId(Definition.NodeId);
                else if (Threshold != null)
                {
                    var definition = Document.FindAlarmDefinitionByNodeId(Threshold.UFUAAlarmDefinitionNodeIdRef);
                    Threshold = (from c in definition.UFUAAlarmThresholds
                                 where c.Name == Threshold.Name
                                 select c).FirstOrDefault();
                }
            }

            if (list == null || bRefresh)
            {
                if (list == null)
                    list = new ObservableUpdateCollection<IDocumentManager>(bCheckDisposableElement: true);
                else
                    list.Clear();

                if (Area != null)
                {
                    foreach (var name in Area.UFUAAreas)
                    {
                        list.Add(new TreeDocumentManagers.TreeAlarmPrototypesDocManager(editorManagerComponent,
                                                    Document, name, this));
                    }
                    foreach (var name in Area.UFUAAlarmSources)
                    {
                        list.Add(new TreeDocumentManagers.TreeAlarmPrototypesDocManager(editorManagerComponent,
                                                    Document, name, this));
                    }
                }
                else if (Source != null)
                {
                    foreach (var name in Source.UFUAAlarmDefinitions)
                    {
                        list.Add(new TreeDocumentManagers.TreeAlarmPrototypesDocManager(editorManagerComponent,
                                                    Document, name, this));
                    }
                }
                else if (Definition != null)
                {
                    var listthresholds = (from t in Definition.UFUAAlarmThresholds where t.IsValid select t).ToList();
                    foreach (var name in listthresholds)
                    {
                        list.Add(new TreeDocumentManagers.TreeAlarmPrototypesDocManager(editorManagerComponent,
                                                    Document, name, this));
                    }
                }
                else if (Threshold == null)
                {
                    foreach (var name in Document.GetAlarmAreas())
                    {
                        list.Add(new TreeDocumentManagers.TreeAlarmPrototypesDocManager(editorManagerComponent,
                                                    Document, name, this));
                    }
                }
            }
        }

        protected override bool IsRootTreeDocManager
        {
            get
            {
                return Area == null && Source == null && Threshold == null;
            }
        }

        internal override bool OnChangedDocument(Object sender, ChangedType type, object changedObject)
        {
            var area = changedObject as UFUAModel.UFUAArea;
            var source = changedObject as UFUAModel.UFUAAlarmSource;
            var definition = changedObject as UFUAModel.UFUAAlarmDefinition;
            var threshold = changedObject as UFUAModel.UFUAAlarmThreshold;

            switch (type)
            {
                case ChangedType.changed:
                    if (area != null)
                    {
                        if (area == Area)
                        {
                            OnPropertyChanged();
                            return true;
                        }
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreeAlarmPrototypesDocManager>())
                                {
                                    if (item.OnChangedDocument(sender, type, changedObject))
                                        return true;
                                }
                            }
                        }
                    }
                    else if (source != null)
                    {
                        if (source == Source)
                        {
                            OnPropertyChanged();
                            return true;
                        }
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreeAlarmPrototypesDocManager>())
                                {
                                    if (item.OnChangedDocument(sender, type, changedObject))
                                        return true;
                                }
                            }
                        }
                    }
                    else if (definition != null)
                    {
                        if (definition == Definition)
                        {
                            OnPropertyChanged();
                            return true;
                        }
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreeAlarmPrototypesDocManager>())
                                {
                                    if (item.OnChangedDocument(sender, type, changedObject))
                                        return true;
                                }
                            }
                        }
                    }
                    else if (threshold != null)
                    {
                        if (threshold == Threshold)
                        {
                            OnPropertyChanged();
                            return true;
                        }
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreeAlarmPrototypesDocManager>())
                                {
                                    if (item.OnChangedDocument(sender, type, changedObject))
                                        return true;
                                }
                            }
                        }
                    }
                    break;

                case ChangedType.added:
                    if (area != null)
                    {
                        if (area.UFUAAreaAss == null || area.UFUAAreaAss == Area)
                        {
                            if (list != null)
                                list.Add(new TreeDocumentManagers.TreeAlarmPrototypesDocManager(editorManagerComponent,
                                                            Document, area, this));
                            return true;
                        }
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreeAlarmPrototypesDocManager>())
                                {
                                    if (item.OnChangedDocument(sender, type, changedObject))
                                        return true;
                                }
                            }
                        }
                    }
                    else if (source != null)
                    {
                        if (source.UFUAArea == null || source.UFUAArea == Area)
                        {
                            if (list != null)
                                list.Add(new TreeDocumentManagers.TreeAlarmPrototypesDocManager(editorManagerComponent,
                                                            Document, source, this));
                            return true;
                        }
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreeAlarmPrototypesDocManager>())
                                {
                                    if (item.OnChangedDocument(sender, type, changedObject))
                                        return true;
                                }
                            }
                        }
                    }
                    else if (definition != null)
                    {
                        if (definition.UFUAAlarmDefinitions == null || definition.UFUAAlarmDefinitions == Source)
                        {
                            if (list != null)
                                list.Add(new TreeDocumentManagers.TreeAlarmPrototypesDocManager(editorManagerComponent,
                                                            Document, definition, this));
                            return true;
                        }
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreeAlarmPrototypesDocManager>())
                                {
                                    if (item.OnChangedDocument(sender, type, changedObject))
                                        return true;
                                }
                            }
                        }
                    }
                    else if (threshold != null)
                    {
                        if (threshold.UFUAAlarmDefinitionRef == null || threshold.UFUAAlarmDefinitionRef == Definition)
                        {
                            if (list != null)
                                list.Add(new TreeDocumentManagers.TreeAlarmPrototypesDocManager(editorManagerComponent,
                                                            Document, threshold, this));
                            return true;
                        }
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreeAlarmPrototypesDocManager>())
                                {
                                    if (item.OnChangedDocument(sender, type, changedObject))
                                        return true;
                                }
                            }
                        }
                    }
                    break;

                case ChangedType.removed:
                    if (area != null)
                    {
                        if (area.UFUAAreaAss == null || area.UFUAAreaAss == Area)
                        {
                            if (list != null)
                            {
                                var found = (from c in list.OfType<TreeAlarmPrototypesDocManager>() where c.Area == area select c).ToList();
                                found.ForEach(item => list.Remove(item));
                            }
                            return true;
                        }
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreeAlarmPrototypesDocManager>())
                                {
                                    if (item.OnChangedDocument(sender, type, changedObject))
                                        return true;
                                }
                            }
                        }
                    }
                    else if (source != null)
                    {
                        if (source.UFUAArea == null || source.UFUAArea == Area)
                        {
                            if (list != null)
                            {
                                var found = (from c in list.OfType<TreeAlarmPrototypesDocManager>() where c.Source == source select c).ToList();
                                found.ForEach(item => list.Remove(item));
                            }
                            return true;
                        }
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreeAlarmPrototypesDocManager>())
                                {
                                    if (item.OnChangedDocument(sender, type, changedObject))
                                        return true;
                                }
                            }
                        }
                    }
                    else if (definition != null)
                    {
                        if (definition.UFUAAlarmDefinitions == null || definition.UFUAAlarmDefinitions == Source)
                        {
                            if (list != null)
                            {
                                var found = (from c in list.OfType<TreeAlarmPrototypesDocManager>() where c.Definition == definition select c).ToList();
                                found.ForEach(item => list.Remove(item));
                            }
                            return true;
                        }
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreeAlarmPrototypesDocManager>())
                                {
                                    if (item.OnChangedDocument(sender, type, changedObject))
                                        return true;
                                }
                            }
                        }
                    }
                    else if (threshold != null)
                    {
                        if (threshold.UFUAAlarmDefinitionRef == null || threshold.UFUAAlarmDefinitionRef == Definition)
                        {
                            if (list != null)
                            {
                                var found = (from c in list.OfType<TreeAlarmPrototypesDocManager>() where c.Threshold == threshold select c).ToList();
                                found.ForEach(item => list.Remove(item));
                            }
                            return true;
                        }
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreeAlarmPrototypesDocManager>())
                                {
                                    if (item.OnChangedDocument(sender, type, changedObject))
                                        return true;
                                }
                            }
                        }
                    }
                    break;
            }

            return false;
        }

        public override void Edit(Uri uri, IDocument parent)
        {
            editorManagerComponent.Edit(uri, parent);
            editorManagerComponent.GetViewFromUri(uri).tabAlarmPrototypes.IsSelected = true;
        }

        public override string TypeLabel
        {
            get 
            {
                if (Area != null)
                    return Area.Name;
                else if (Source != null)
                    return Source.Name;
                else if (Definition != null)
                    return Definition.Name;
                else if (Threshold != null)
                    return Threshold.Name;
                else
                    return Properties.Resources.AlarmRibbonTitle; 
            }
        }

        public override BitmapImage TypeIcon
        {
            get 
            {
                if (Area != null)
                    return UFUAEditorManagerComponent.GetBitmapImage("UFUASAlarmAreaSmall");
                else if (Source != null)
                    return UFUAEditorManagerComponent.GetBitmapImage("UFUASAlarmSourceSmall");
                else if (Definition != null)
                {
                    return Definition.Severity > 0 ? 
                        UFUAEditorManagerComponent.GetBitmapImage("UFUASAlarmPrototypeSmall") : 
                        UFUAEditorManagerComponent.GetBitmapImage("UFUASMessagePrototypeSmall");
                }
                else if (Threshold != null)
                {
                    return Threshold.UFUAAlarmDefinitionRef == null || Threshold.UFUAAlarmDefinitionRef.Severity > 0 ?
                        UFUAEditorManagerComponent.GetBitmapImage("UFUASAlarmPrototypeSmall") :
                        UFUAEditorManagerComponent.GetBitmapImage("UFUASMessagePrototypeSmall");
                }
                else
                    return UFUAEditorManagerComponent.GetBitmapImage("UFUASAlarmPrototypeSmall"); 
            }
        }

        public override BitmapImage TypeIconLarge
        {
            get 
            {
                if (Area != null)
                    return UFUAEditorManagerComponent.GetBitmapImage("UFUASAlarmArea");
                else if (Source != null)
                    return UFUAEditorManagerComponent.GetBitmapImage("UFUASAlarmSource");
                else if (Definition != null || Threshold != null)
                    return UFUAEditorManagerComponent.GetBitmapImage("UFUASAlarmPrototype");
                else
                    return UFUAEditorManagerComponent.GetBitmapImage("UFUASAlarmPrototype"); 
            }
        }

        public override string TypeScheme
        {
            get
            {
                return Properties.Settings.Default.AlarmsTypeScheme;
            }
        }

        public override bool IsResourceExpandable(IDocument document = null)
        {
            if (IsRootTreeDocManager)
                CreateDocumentIfDisposed();
            if (Document == null || Document.IsDisposed)
                return IsRootTreeDocManager;
                
            if (Area != null)
                return Area.UFUAAreas.Count > 0 || Area.UFUAAlarmSources.Count > 0;
            else if (Source != null)
                return Source.UFUAAlarmDefinitions.Count > 0;
            else if (Definition != null)
            {
                var listthresholds = (from t in Definition.UFUAAlarmThresholds where t.IsValid select t).ToList();
                return listthresholds.Count > 0;
            }
            else if (Threshold != null)
                return false;
            else
            {
                if (Properties.Settings.Default.FolderAlwaysExpandible)
                    return true;
                
                var list = Document.GetAlarmAreas();
                return list.Count > 0;
            }
        }

        public override Object BrowsableContent
        {
            get
            {
                if (Document == null || Document.IsDisposed)
                    return null;

                if (Area != null)
                    return Area;
                else if (Source != null)
                    return Source;
                else if (Definition != null)
                    return Definition;
                else if (Threshold != null)
                    return Threshold;

                return null;
            }
        }
        #endregion
    }
}
