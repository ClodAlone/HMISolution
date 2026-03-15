using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.Windows.Markup;
using System.Xml;
using System.Diagnostics;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Controls;
using ScreenSettings.Entities;
using System.Linq;
using Utilities.WPF;
using ScreenSettings;
using Utilities;

namespace ScreenManager.UndoRedoManager
{
    public enum UndoAction
    {
        Changed,
        Added,
        Removed,
        TotalState
    };

    public class MementoData
    {
        public double width { get; set; }
        public double height { get; set; }
        public double top { get; set; }
        public double left { get; set; }
        public int zOrder { get; set; }
        public bool manipulation { get; set; }
        public String name { get; set; }
        public String code { get; set; }

        public static MementoData FromElement(FrameworkElement element, bool bOnlyPos = false)
        {
            var ret = new MementoData()
            {
                width = element.Width,
                height = element.Height,
                name = element.Name,

                left = Canvas.GetLeft(element),
                top = Canvas.GetTop(element)
            };

            if (!bOnlyPos)
            {
                ret.code = element.XamlWriterFormatted();
                ret.manipulation = element.IsManipulationEnabled;
            }

            return ret;
        }

        public void RestorePos(UIElement ret)
        {
            var element = ret as FrameworkElement;
            if (element == null)
                return;

            element.Width = width;
            element.Height = height;
            element.Name = name;

            if (!Double.IsNaN(left))
            {
                Canvas.SetLeft(element, left);
                // InkCanvas.SetLeft(element, left);
            }
            if (!Double.IsNaN(top))
            {
                Canvas.SetTop(element, top);
                // InkCanvas.SetTop(element, top);
            }

            // Canvas.SetZIndex(element, zOrder);
        }

        public UIElement ToElement()
        {
            if (String.IsNullOrEmpty(code))
                return null;
            var ret = code.ReadUIElement();
            var element = ret as FrameworkElement;
            element.Width = width;
            element.Height = height;
            element.Name = name;
            element.IsManipulationEnabled = manipulation;
            DesignerProperties.SetIsInDesignMode(element, true);

            if (!Double.IsNaN(left))
            {
                Canvas.SetLeft(element, left);
                // InkCanvas.SetLeft(element, left);
            }
            if (!Double.IsNaN(top))
            {
                Canvas.SetTop(element, top);
                // InkCanvas.SetTop(element, top);
            }

            return ret;
        }
    }

    #region Memento
    public class Memento
    {
        List<MementoData> _ContainerState;
        List<String> _ContainerSelectionState;
        FrameworkElement _Container;
        ScreenObjectsSettingsMap _ContainerDocument;
        List<String> _ListResources;
        UndoAction _action;

        public UndoAction UndoAction
        {
            get
            {
                return _action;
            }
        }
        public List<MementoData> ContainerState
        {
            get { return _ContainerState; }
        }

        public List<String> ContainerSelectionState
        {
            get { return _ContainerSelectionState; }
        }

        public FrameworkElement Container
        {
            get { return _Container; }
        }

        public ScreenObjectsSettingsMap ContainerDocument
        {
            get { return _ContainerDocument; }
        }

        public List<String> ListResources
        {
            get { return _ListResources; }
        }

        public Memento(List<MementoData> containerState, List<String> containerSelectionState,
                       UIElement container, ScreenObjectsSettingsMap containerDocument,
                       List<String> listResources, UndoAction action, ScreenObjectsSettingsMap inners = null)
        {
            _action = action;
            _ContainerState = containerState;
            _ContainerSelectionState = new List<String>(containerSelectionState);

            if (containerSelectionState.Count == 0)
            {
                Canvas ink = new Canvas();
                _Container = ink;
                _Container.Resources = ink.Resources;
                Utilities.WPF.Cloners.CopyObjects(container, ink, false);
            }

            if (containerDocument != null)
            {
                _ContainerDocument = new ScreenObjectsSettingsMap();
                if (_action == UndoAction.TotalState)
                {
                    foreach (var v in containerDocument.Keys)
                    {
                        try
                        {
                            var str = containerDocument[v].ToXml();
                            var entity = str.FromXml<ScreenEntity>();
                            _ContainerDocument.Add(v, entity);
                        }
                        catch { }
                    }
                }
                else
                {
                    containerSelectionState.ForEach(name =>
                        {
                            if (containerDocument.ContainsKey(name))
                            {
                                try
                                {
                                    var str = containerDocument[name].ToXml();
                                    var entity = str.FromXml<ScreenEntity>();
                                    _ContainerDocument.Add(name, entity);
                                }
                                catch { }
                            }
                        });

                    if (inners != null)
                        inners.Keys.ToList().ForEach(name =>
                            {
                                if (containerDocument.ContainsKey(name))
                                {
                                    try
                                    {
                                        var str = containerDocument[name].ToXml();
                                        var entity = str.FromXml<ScreenEntity>();
                                        _ContainerDocument.Add(name, entity);
                                    }
                                    catch { }
                                }
                            });
                }
            }

            if (listResources != null)
            {
                if (_ListResources == null)
                    _ListResources = new List<string>();
                _ListResources.AddRange(listResources);
            }
        }

        internal void InvertAction()
        {
            if (_action == UndoAction.Removed)
                _action = UndoAction.Added;
            else if (_action == UndoAction.Added)
                _action = UndoAction.Removed;
        }
    }

    #endregion

    #region MementoOriginator
    public class MementoOriginator
    {
        private ScreenEditorView _Container;

        public MementoOriginator(ScreenEditorView container)
        {
            _Container = container;
        }

        public Memento getMemento(UndoAction action)
        {
            List<MementoData> _ContainerState = new List<MementoData>();
            ScreenObjectsSettingsMap inners = null;
            var items = new List<FrameworkElement>();
            if (action == UndoAction.TotalState)
            {
                _Container.Document.ResolveProblematicXamlOnCanvas(_Container.MainSurface);

                foreach (UIElement item in _Container.MainSurface.Children)
                {
                    try
                    {
                        var fe = item as FrameworkElement;

                        // _Container.Document.ResolveProblematicXamlOnCanvas(_Container.MainSurface, fe.Name);
                        if (fe != null) {
                            _Container.RestoreUntranslated(fe);
                            items.Add(fe);
                        }

                        var ret = MementoData.FromElement(fe);
                        ret.zOrder = _Container.MainSurface.Children.IndexOf(fe);

                        _ContainerState.Add(ret);

                        //_Container.Document.RestoreProblematicXamlWriter(_Container.MainSurface, fe.Name, true);
                        //_Container.Document.CleanProblematicXamlBags();

                        _Container.Document.SaveProblematicXamlWriterProperties(fe, fe.Name);
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError(ex.ToString());
                    }
                }

                _Container.Document.RestoreProblematicXamlWriter(_Container.MainSurface);
                _Container.Document.CleanProblematicXamlBags();
            }
            else
            {
                var selectionNames = _Container.MainSurface_GetCurrentSelectionNames();

                // _Container.MainSurface_ActivateDeactivateLineBaseAdorners(false);
                foreach (FrameworkElement item in _Container.MainSurface.Children)
                {
                    if (!selectionNames.Contains(item.Name))
                        continue;
                    
                    _Container.Document.ResolveProblematicXamlOnCanvas(_Container.MainSurface, item.Name);
                    _Container.RestoreUntranslated(item);
                    items.Add(item);

                    var ret = MementoData.FromElement(item/*, action == UndoAction.Changed*/);
                    ret.zOrder = _Container.MainSurface.Children.IndexOf(item);

                    _ContainerState.Add(ret);

                    _Container.Document.RestoreProblematicXamlWriter(_Container.MainSurface, item.Name, bUpdate:true);
                    _Container.Document.CleanProblematicXamlBags();
                }
                // _Container.MainSurface_ActivateDeactivateLineBaseAdorners(true);

                selectionNames.ForEach(name =>
                    {
                        var item = _Container.MainSurface.FindName(name) as UIElement;
                        var listinners = _Container.Document.GetListInners(name);
                        if (listinners.Count > 0)
                        {
                            if (inners == null)
                                inners = new ScreenObjectsSettingsMap();
                            listinners.ForEach(inner =>
                                {
                                    if (!inners.ContainsKey(inner) && _Container.Document.MapScreenEntities.ContainsKey(inner))
                                        inners.Add(inner, _Container.Document.MapScreenEntities[inner]);
                                });
                        }

                        /*
                        if (item != null)
                        {
                            try
                            {
                                var fe = item as FrameworkElement;

                                _Container.Document.ResolveProblematicXamlOnCanvas(_Container.MainSurface, fe.Name);

                                _ContainerState.Add(MementoData.FromElement(fe));

                                _Container.Document.RestoreProblematicXamlWriter(_Container.MainSurface, fe.Name, true);
                                _Container.Document.CleanProblematicXamlBags();

                                _Container.Document.SaveProblematicXamlWriterProperties(fe, fe.Name);
                            }
                            catch (Exception ex)
                            {
                                Trace.TraceError(ex.ToString());
                            }
                        }
                        */
                    });
            }

            foreach (var fe in items)
            {
                //if (_Container.Document.MapScreenEntities.ContainsKey(fe.Name) && !_Container.Document.MapScreenEntities[fe.Name].SourceSymbolLinked) //otherwise, translation will be performed on Repository Item Loaded event.
                _Container.ElementToCurrentLanguage(fe);
            }

            return new Memento(_ContainerState, _Container.MainSurface_GetCurrentSelectionNames(), 
                                _Container.MainSurface, _Container.Document.MapScreenEntities,
                                _Container.Document.ListResources, action, inners);
        }

        public void setMemento(Memento memento)
        {
            if (_Container == null || _Container.Document == null || _Container.MainSurface == null)
                return;

            _Container.MainSurface_CleanCurrentSelectedion();
            var newItems = new List<FrameworkElement>();
            switch (memento.UndoAction)
            {
                case UndoAction.TotalState:

                    _Container.Document.UnsubscribeAllChangeXamlWriterProperties();
                    //foreach (var v in _Container.Document.MapScreenEntities.Keys)
                    //{
                    //    var uie = _Container.Document.FindInnerControl(_Container.MainSurface, v);
                    //    Utilities.WPF.DependencyObjectExtensions.UnregisterName(_Container.MainSurface, v);
                    //}
                    _Container.Document.MapScreenEntities.Clear();
                    foreach (var ui in _Container.MainSurface.Children)
                    {
                        if (ui is FrameworkElement)
                            Utilities.WPF.DependencyObjectExtensions.UnregisterName(_Container.MainSurface, ui as FrameworkElement);
                        _Container.Document.ClearTranslationMaps(ui as UIElement);
                    }
                    foreach (var v in memento.ContainerDocument.Keys)
                    {
                        try
                        {
                            var str = memento.ContainerDocument[v].ToXml();
                            var entity = str.FromXml<ScreenEntity>();

                            _Container.Document.MapScreenEntities.Add(v, entity);
                            Utilities.WPF.DependencyObjectExtensions.UnregisterName(_Container.MainSurface, v);
                        }
                        catch { }
                    }
                    _Container.MainSurface.Children.Clear();
                    memento.ContainerState.ForEach(item =>
                    {
                        UIElement newItem = item.ToElement();
                        var bIsProblematic = Utilities.WPF.XmlHelper.CheckProblematicXamlWriter(newItem, item.code);
                        if (newItem as FrameworkElement != null)
                        {
                            if (!bIsProblematic)
                                _Container.RestoreUntranslated(newItem as FrameworkElement);
                            newItems.Add(newItem as FrameworkElement);
                        }
                        _Container.MainSurface.Children.Add(newItem);
                        if (newItem is FrameworkElement)
                        {
                            var fe = newItem as FrameworkElement;
                            if (bIsProblematic)
                            {
                                Utilities.WPF.DependencyObjectExtensions.RegisterName(_Container.MainSurface, newItem as FrameworkElement, true, false);
                                _Container.Document.UpdateProblematicXamlWriterProperties(newItem as FrameworkElement, (newItem as FrameworkElement).Name);
                                _Container.RestoreUntranslated(newItem as FrameworkElement);
                            }
                            else
                                Utilities.WPF.DependencyObjectExtensions.RegisterName(_Container.MainSurface, newItem as FrameworkElement);
                        }
                        _Container.Document.RestoreProblematicXamlWriter(_Container.MainSurface, bUpdate: true);
                        _Container.Document.CleanProblematicXamlBags();
                        try
                        {
                            _Container.Document.LoadRepositoryItem(_Container, _Container.MainSurface, (newItem as FrameworkElement).Name, bAnimate: false);
                        }
                        catch(Exception ex)
                        {

                        }
                    });
                    break;

                case UndoAction.Changed:
                    foreach (var v in memento.ContainerDocument.Keys)
                    {
                        var uie = _Container.Document.FindInnerControl(_Container.MainSurface, v);
                        _Container.Document.UnsubscribePropertyChangeXamlWriterProperties(uie);
                        if (_Container.Document.MapScreenEntities.ContainsKey(v))
                            _Container.Document.MapScreenEntities.Remove(v);

                        try
                        {
                            var str = memento.ContainerDocument[v].ToXml();
                            var entity = str.FromXml<ScreenEntity>();
                            _Container.Document.MapScreenEntities.Add(v, entity);
                        }
                        catch { }
                    }
                    memento.ContainerState.ForEach(item =>
                    {
                        UIElement newItem = item.ToElement();
                        if (newItem == null)
                        {
                            newItem = _Container.MainSurface.FindName(item.name) as UIElement;
                            item.RestorePos(newItem);
                        }
                        else
                        {
                            var bIsProblematic = Utilities.WPF.XmlHelper.CheckProblematicXamlWriter(newItem, item.code);
                            if (newItem is FrameworkElement)
                            {
                                var fe = newItem as FrameworkElement;
                                if (newItem as FrameworkElement != null)
                                {
                                    if (!bIsProblematic)
                                        _Container.RestoreUntranslated(newItem as FrameworkElement);
                                    newItems.Add(newItem as FrameworkElement);
                                }
                                var itemToRemove = _Container.MainSurface.FindName(fe.Name) as FrameworkElement;
                                if (itemToRemove != null)
                                {
                                    Utilities.WPF.DependencyObjectExtensions.UnregisterName(_Container.MainSurface, itemToRemove);
                                    _Container.Document.UnsubscribePropertyChangeXamlWriterProperties(itemToRemove);
                                    int n = _Container.MainSurface.Children.IndexOf(itemToRemove);
                                    if (n < 0)
                                    {
                                        var parent = itemToRemove.FindFirstParent<Panel>();
                                        if (parent != null && (itemToRemove is Canvas || itemToRemove is InkCanvas))
                                            parent = parent.FindFirstParent<Panel>();
                                        if (parent != null)
                                        {
                                            n = parent.Children.IndexOf(itemToRemove);
                                            parent.Children.Insert(n, newItem);
                                            parent.Children.Remove(itemToRemove);
                                            InkCanvas.SetTop(newItem, InkCanvas.GetTop(itemToRemove));
                                            InkCanvas.SetLeft(newItem, InkCanvas.GetLeft(itemToRemove));
                                            Canvas.SetTop(newItem, Canvas.GetTop(itemToRemove));
                                            Canvas.SetLeft(newItem, Canvas.GetLeft(itemToRemove));
                                        }
                                        else
                                        {
                                            var decorator = itemToRemove.FindFirstParent<Decorator>();
                                            if (decorator != null)
                                            {
                                                decorator.Child = newItem;
                                            }
                                            else
                                            {
                                                var content = itemToRemove.FindFirstParent<ContentControl>();
                                                if (content != null)
                                                    content.Content = newItem;
                                            }
                                        }
                                        _Container.Document.ClearTranslationMaps(itemToRemove);
                                    }
                                    else
                                    {
                                        _Container.Document.ClearTranslationMaps(itemToRemove);
                                        _Container.MainSurface.Children.Remove(itemToRemove);
                                        _Container.MainSurface.Children.Insert(n, newItem);
                                    }
                                    _Container.Document.ClearTranslationMaps(itemToRemove);
                                    itemToRemove.Dispose();
                                }

                                if (newItem is FrameworkElement)
                                {
                                    if (bIsProblematic)
                                    {
                                        Utilities.WPF.DependencyObjectExtensions.RegisterName(_Container.MainSurface, newItem as FrameworkElement, true, false);
                                        _Container.Document.UpdateProblematicXamlWriterProperties(newItem as FrameworkElement, (newItem as FrameworkElement).Name);
                                        _Container.RestoreUntranslated(newItem as FrameworkElement);
                                    }
                                    else
                                        Utilities.WPF.DependencyObjectExtensions.RegisterName(_Container.MainSurface, newItem as FrameworkElement);
                                }

                                _Container.Document.RestoreProblematicXamlWriter(_Container.MainSurface, fe.Name, bUpdate: true);
                                _Container.Document.CleanProblematicXamlBags();

                                try
                                {
                                    var ret = _Container.Document.LoadRepositoryItem(_Container, _Container.MainSurface, fe.Name, bAnimate: false);
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                            item.RestorePos(newItem);
                        }

                        if (item.zOrder >= 0 && item.zOrder < _Container.MainSurface.Children.Count)
                        {
                            var zOrder = _Container.MainSurface.Children.IndexOf(newItem);
                            if (zOrder != item.zOrder)
                            {
                                _Container.MainSurface.Children.Remove(newItem);
                                _Container.MainSurface.Children.Insert(item.zOrder, newItem);
                            }
                        }
                    });
                    break;
                case UndoAction.Added:
                    foreach (var v in memento.ContainerDocument.Keys)
                    {
                        var uie = _Container.Document.FindInnerControl(_Container.MainSurface, v);
                        _Container.Document.UnsubscribePropertyChangeXamlWriterProperties(uie);
                        if (_Container.Document.MapScreenEntities.ContainsKey(v))
                            _Container.Document.MapScreenEntities.Remove(v);
                    }
                    memento.ContainerState.ForEach(item =>
                    {
                        UIElement newItem = item.ToElement();
                        if (newItem == null)
                        {
                            newItem = _Container.MainSurface.FindName(item.name) as UIElement;
                            item.RestorePos(newItem);
                        }
                        else
                        {
                            if (newItem is FrameworkElement)
                            {
                                var fe = newItem as FrameworkElement;
                                var itemToRemove = _Container.MainSurface.FindName(fe.Name) as FrameworkElement;
                                Utilities.WPF.DependencyObjectExtensions.UnregisterName(_Container.MainSurface, itemToRemove);
                                _Container.Document.ClearTranslationMaps(itemToRemove);
                                _Container.MainSurface.Children.Remove(itemToRemove);
                                itemToRemove.Dispose();
                            }
                        }
                    });
                    break;
                case UndoAction.Removed:
                    foreach (var v in memento.ContainerDocument.Keys)
                    {
                        if (_Container.Document.MapScreenEntities.ContainsKey(v))
                            _Container.Document.MapScreenEntities.Remove(v);

                        try
                        {
                            var str = memento.ContainerDocument[v].ToXml();
                            var entity = str.FromXml<ScreenEntity>();
                            _Container.Document.MapScreenEntities.Add(v, entity);
                        }
                        catch { }
                    }
                    memento.ContainerState.ForEach(item =>
                    {
                        var newItem = item.ToElement();
                        if (newItem == null)
                        {
                            newItem = _Container.MainSurface.FindName(item.name) as UIElement;
                            item.RestorePos(newItem);
                        }
                        else 
                        {
                            var bIsProblematic = Utilities.WPF.XmlHelper.CheckProblematicXamlWriter(newItem, item.code);
                            if (newItem as FrameworkElement != null)
                            {
                                if (!bIsProblematic)
                                    _Container.RestoreUntranslated(newItem as FrameworkElement);
                                newItems.Add(newItem as FrameworkElement);
                            }
                            if (item.zOrder >= 0 && item.zOrder < _Container.MainSurface.Children.Count)
                                _Container.MainSurface.Children.Insert(item.zOrder, newItem);
                            else
                                _Container.MainSurface.Children.Add(newItem);
                            if (newItem is FrameworkElement)
                            {
                                if (bIsProblematic)
                                {
                                    Utilities.WPF.DependencyObjectExtensions.RegisterName(_Container.MainSurface, newItem as FrameworkElement, true, false);
                                    _Container.Document.UpdateProblematicXamlWriterProperties(newItem as FrameworkElement, (newItem as FrameworkElement).Name);
                                    _Container.RestoreUntranslated(newItem as FrameworkElement);
                                }
                                else
                                    Utilities.WPF.DependencyObjectExtensions.RegisterName(_Container.MainSurface, newItem as FrameworkElement);
                            }

                            var fe = newItem as FrameworkElement;
                            _Container.Document.RestoreProblematicXamlWriter(_Container.MainSurface, fe.Name, bUpdate: true);
                            _Container.Document.CleanProblematicXamlBags();

                            try
                            {
                                _Container.Document.LoadRepositoryItem(_Container, _Container.MainSurface, fe.Name, bAnimate: false);
                            }
                            catch(Exception ex)
                            {

                            }
                        }
                    });
                    break;
            }

            if (memento.Container != null)
            {
                Utilities.WPF.Cloners.CopyObjects(memento.Container, _Container.MainSurface, false);
                _Container.MainSurface.Resources = memento.Container.Resources;
            }

            if (memento.ListResources != null)
            {
                _Container.Document.ListResources.Clear();
                _Container.Document.ListResources.AddRange(memento.ListResources);
            }
            else
                _Container.Document.ListResources = null;

            foreach (var fe in newItems)
            {
                if (_Container.Document.MapScreenEntities.ContainsKey(fe.Name) && !_Container.Document.MapScreenEntities[fe.Name].SourceSymbolLinked) //otherwise, translation will be performed on Repository Item Loaded event.
                    _Container.ElementToCurrentLanguage(fe);
            }

            _Container.MainSurface_SetCurrentSelectionNames(memento.ContainerSelectionState);
        }

        private static UIElement DeepClone(UIElement element)
        {
            return element.Clone();
            //string clon = XamlWriter.Save(element);
            //UIElement cloned = (UIElement)XamlReader.Load(
            //    XmlReader.Create(new StringReader(clon)));
            //return cloned;

            //string shapestring = XamlWriter.Save(element);
            //StringReader stringReader = new StringReader(shapestring);
            //XmlTextReader xmlTextReader = new XmlTextReader(stringReader);
            //UIElement DeepCopyobject = (UIElement)XamlReader.Load(xmlTextReader);
            //return DeepCopyobject;
        }

    }
    #endregion

    #region Caretaker

    class Caretaker : IDisposable
    {
        private List<Memento> UndoStack = new List<Memento>();
        private List<Memento> RedoStack = new List<Memento>();

        public Memento getUndoMemento()
        {
            if (UndoStack.Count >= 1)
            {
                Memento undoStackPop = UndoStack[UndoStack.Count - 1];
                UndoStack.RemoveAt(UndoStack.Count - 1);
                // RedoStack.Add(undoStackPop);
                return undoStackPop;
            }
            else
                return null;
        }
        public Memento getRedoMemento()
        {
            if (RedoStack.Count >= 1)
            {
                //Memento m = RedoStack[RedoStack.Count - 2];
                //UndoStack.Add(RedoStack[RedoStack.Count - 1]);
                //RedoStack.RemoveAt(RedoStack.Count - 1);
                Memento m = RedoStack[RedoStack.Count - 1];
                RedoStack.RemoveAt(RedoStack.Count - 1);
                // UndoStack.Add(m);
                return m;
            }
            else
                return null;
        }

        public void InsertMementoForUndoRedo(Memento memento)
        {
            if (memento == null)
                return;

            UndoStack.Add(memento);
            RedoStack.Clear();

            while (UndoStack.Count > maxUndo)
                UndoStack.RemoveAt(0);

            Debug.WriteLine(String.Format("new Screen Memento. Total Memento {0}", UndoStack.Count));
        }

        public void InsertMementoForUndo(Memento memento)
        {
            if (memento == null)
                return;

            UndoStack.Add(memento);

            Debug.WriteLine(String.Format("new Screen Memento. Total Memento {0}", UndoStack.Count));
        }

        public void InsertMementoForRedo(Memento memento)
        {
            if (memento == null)
                return;

            RedoStack.Add(memento);

            Debug.WriteLine(String.Format("new Screen Memento. Total Memento {0}", RedoStack.Count));
        }

        public bool IsUndoPossible()
        {
            return UndoStack.Count >= 1 ? true : false;

        }
        public bool IsRedoPossible()
        {
            return RedoStack.Count >= 1 ? true : false;
        }
        public bool IsRedoEmpty()
        {
            return RedoStack.Count == 0 ? true : false;
        }

        const int maxUndo = 10;

        #region IDisposable Members

        public void Dispose()
        {
            if (UndoStack != null)
            {
                UndoStack.Clear();
                UndoStack = null;
            }

            if (RedoStack != null)
            {
                RedoStack.Clear();
                RedoStack = null;
            }
        }

        #endregion

        internal void UpdateRedoBuffer(Memento back)
        {
            RedoStack.RemoveAt(RedoStack.Count - 1);
            RedoStack.Add(back);
        }

        internal void UpdateUndoBuffer(Memento back)
        {
            UndoStack.RemoveAt(UndoStack.Count - 1);
            UndoStack.Add(back);
        }
    }

    #endregion

    #region UndoRedo
    public class UndoRedo : IUndoRedo, IEditableObject, IDisposable
    {


        Caretaker _Caretaker = new Caretaker();
        MementoOriginator _MementoOriginator = null;
        public event EventHandler EnableDisableUndoRedoFeature;

        bool isEnabled = true;
        public bool IsEnabled { 
            get
            {
                return isEnabled;
            }
            set
            {
                isEnabled = value;
            }
        }

        public UndoRedo(ScreenEditorView container)
        {
            _MementoOriginator = new MementoOriginator(container);

        }
        public void Undo(int level)
        {
            if (_MementoOriginator == null || _Caretaker == null)
                return;

            Memento memento = null;
            // bool isRedoPossible = _Caretaker.IsRedoPossible();
            for (int i = 1; i <= level; i++)
            {
                // if (_Caretaker.IsRedoEmpty())
                {
                    Memento redo = _MementoOriginator.getMemento(UndoAction.TotalState);
                    _Caretaker.InsertMementoForRedo(redo);
                }
                memento = _Caretaker.getUndoMemento();
            }
            if (memento != null)
            {
                //if (!isRedoPossible && memento.UndoAction == UndoAction.Changed)
                //{
                //    var back = _MementoOriginator.getMemento(UndoAction.Changed);
                //    _Caretaker.UpdateRedoBuffer(back);
                //}

                _MementoOriginator.setMemento(memento);
                memento.InvertAction();
            }

            OnEnableUndoRedoFeatures();
        }

        void OnEnableUndoRedoFeatures()
        {
            EventHandler temp = EnableDisableUndoRedoFeature;
            if (temp != null)
            {
                temp(null, null);
            }

            // Dirty the commands registered with CommandManager,
            // such as our Save command, so that they are queried
            // to see if they can execute now.
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }

        public void Redo(int level)
        {
            if (_MementoOriginator == null || _Caretaker == null)
                return;

            Memento memento = null;
            // bool isUndoPossible = _Caretaker.IsUndoPossible();
            for (int i = 1; i <= level; i++)
            {
                // if (_Caretaker.IsUndoEmpty())
                {
                    Memento undo = _MementoOriginator.getMemento(UndoAction.TotalState);
                    _Caretaker.InsertMementoForUndo(undo);
                }
                memento = _Caretaker.getRedoMemento();
            }
            if (memento != null)
            {
                //if (!isUndoPossible && memento.UndoAction == UndoAction.Changed)
                //{
                //    var back = _MementoOriginator.getMemento(UndoAction.Changed);
                //    _Caretaker.UpdateUndoBuffer(back);
                //}

                _MementoOriginator.setMemento(memento);
                memento.InvertAction();
            }

            OnEnableUndoRedoFeatures();
        }

        public void SetStateForUndoRedo(UndoAction action)
        {
            if (_MementoOriginator == null || _Caretaker == null)
                return;

            if (!IsEnabled)
                return;

            Memento memento = _MementoOriginator.getMemento(action);
            _Caretaker.InsertMementoForUndoRedo(memento);

            OnEnableUndoRedoFeatures();
        }

        public bool IsUndoPossible()
        {
            return IsEnabled && _Caretaker != null && _Caretaker.IsUndoPossible();

        }
        public bool IsRedoPossible()
        {
            return IsEnabled && _Caretaker != null && _Caretaker.IsRedoPossible();
        }



        #region IEditableObject Members

        Memento BegingEditMemento;
        public void BeginEdit()
        {
            if (_MementoOriginator == null)
                return;

            if (IsEnabled && BegingEditMemento == null && _Caretaker != null)
                BegingEditMemento = _MementoOriginator.getMemento(UndoAction.Changed);
        }

        public void CancelEdit()
        {
            if (_MementoOriginator == null)
                return;

            if (IsEnabled && BegingEditMemento != null && _Caretaker != null)
            {
                _MementoOriginator.setMemento(BegingEditMemento);
                BegingEditMemento = null;
            }
        }

        public void EndEdit()
        {
            if (_MementoOriginator == null)
                return;

            if (IsEnabled && BegingEditMemento != null && _Caretaker != null)
            {
                _Caretaker.InsertMementoForUndoRedo(BegingEditMemento);
                OnEnableUndoRedoFeatures();
            }

            BegingEditMemento = null;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_Caretaker != null)
            {
                _Caretaker.Dispose();
                _Caretaker = null;
            }
            _MementoOriginator = null;
        }

        #endregion
    }



    #endregion

    #region Interface
    interface IUndoRedo
    {
        void Undo(int level);
        void Redo(int level);
        void SetStateForUndoRedo(UndoAction action);

    }
    #endregion

}
