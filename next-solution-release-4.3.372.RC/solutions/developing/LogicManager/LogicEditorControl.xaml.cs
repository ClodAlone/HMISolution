using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using LogicManager.Document;
using Northwoods.GoXam;
using Utilities;
using OPCUAViewModel;
using DocumentManager.ComponentService;
using LogicCore;
using DevExpress.Xpf.Core;
using WPFUtilities;

namespace LogicManager
{
    /// <summary>
    /// Interaction logic for LogicEditorControl.xaml
    /// </summary>
    public partial class LogicEditorControl : UserControl, IEditableObject, IDisposable
    {
        #region Declarations

        UFInterfaces.IWorkspace workspace;
        bool bRuntime;
        #endregion

        bool bLoaded;
        public bool IsLoaded
        {
            get
            {
                return bLoaded;
            }
        }

        static LogicEditorControl()
        {
            // This is a runtime license key for Northwoods.GoWPF version 2.2 (or earlier)
            // Put the following statement in your LogicManager application constructor:
            Northwoods.GoXam.Diagram.LicenseKeyForDLL("pa77Mpx3p/M/OkBn/jVl2UEInQkRG+cu6g7BQUrliTb1g7u+PKxGFIiOAo1S1gJRTSi1OrR8IUgZJIbxa1xbhw==");
        }

        bool bOpening;
        public LogicEditorControl(DocumentEditorDocument doc, bool runtime = false)
        {
            // This is a runtime license key for Northwoods.GoWPF version 2.2 (or earlier)
            // Put the following statement in your MoviconNExT application constructor:
            //Northwoods.GoXam.Diagram.LicenseKey = "23r2U93FhZbehlsLpdj4yXlF6ouXxFQYczpEB/9UwQrm9ijPNEB8rMWiIcsQx2YYZIYRmgyHgP6M1cz/HAr0xQ==";

            Loaded += (o, e) =>
            {
                bLoaded = true;
            };
            Unloaded += (o, e) =>
            {
                bLoaded = false;
            };

            InitializeComponent();
            bRuntime = runtime;
            Document = doc;

            if (!bRuntime)
                workspace = Document.GetService(typeof(UFInterfaces.IWorkspace)) as UFInterfaces.IWorkspace;

            DataContext = Document;

            var types = GateData.LoadGateDataTypes();
            var templateDictionary = new DataTemplateDictionary();
            types.ForEach(type =>
            {
                var gateData = GateData.CreateFrom(type);
                var templates = gateData.GetDataTemplates();
                if (templates != null)
                {
                    foreach (var template in templates)
                    {
                        if (!templateDictionary.ContainsKey(template.Key))
                            templateDictionary.Add(template.Key, template.Value);
                    }
                }
            });

            bOpening = true;
            Document.DisableNeedsSave = true;
            myDiagram.NodeTemplateDictionary = templateDictionary;
            myDiagram.Model = Document.Model;
            if (!bRuntime)
            {
                myDiagram.StartTransaction("Opening");
                myDiagram.AllowDrop = true;
                SetCurrentSelection();
                myDiagram.CommitTransaction("Opening");
                Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    if (Document != null)
                        Document.DisableNeedsSave = false;
                    bOpening = false;
                });
            }
            else
            {
                myDiagram.IsReadOnly = true;
                myDiagram.AllowSelect = false;
            }

            Touch.FrameReported += Touch_FrameReported;
        }

        #region Events

        private void myDiagram_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                return;
            e.Handled = true;
            if (e.Delta > 0)
                myDiagram.Panel.Scale += 0.1;
            else
                myDiagram.Panel.Scale -= 0.1;
        }

        private void Touch_FrameReported(object sender, TouchFrameEventArgs e)
        {
            var touchpoints = e.GetTouchPoints(myDiagram.Panel);
            if (touchpoints.Count > 1)
            {
                var tp0 = touchpoints[0].Position;
                var tp1 = touchpoints[1].Position;
                var newDist = Math.Sqrt((tp0.X - tp1.X) * (tp0.X - tp1.X) + (tp0.Y - tp1.Y) * (tp0.Y - tp1.Y));
                if (Double.IsNaN(startDist))
                {
                    startDist = newDist;
                    startScale = myDiagram.Panel.Scale;
                    var primary = e.GetPrimaryTouchPoint(myDiagram.Panel);
                    if (primary != null) myDiagram.Panel.ZoomPoint = primary.Position;
                }
                else
                {
                    myDiagram.Panel.Scale = startScale * newDist / startDist;
                }
            }
            else
            {
                startDist = Double.NaN;
            }
        }
        private double startDist = Double.NaN;
        private double startScale = 1;

        // handle a mouse-down on an input node: toggle its Value if it's a double-click
        private void StartNodeDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Only executes logic if you have double-clicked.
            if (DiagramPanel.IsDoubleClick(e))
            {
                myDiagram.StartTransaction("Toggle");
                // When you double-click, toggles value of Input node
                Node input = Part.FindAncestor<Node>(sender as UIElement);
                if (input != null)
                {
                    GateData d = input.Data as GateData;
                    if (d != null)
                    {
                        if (d.CanBeToggled())
                            d.Value = !d.Value;
                        else if (Document != null && !Document.InExecution)
                            d.OnDblClick();
                    }
                }
                myDiagram.CommitTransaction("Toggle");
            }
        }

        #endregion

        #region IEditableObject Members

        public void BeginEdit()
        {
            Document.DisableNeedsSave = true;
            myDiagram.StartTransaction("PropertyChanging");
            Document.DisableNeedsSave = false;
        }

        public void CancelEdit()
        {
            if (myDiagram.Model != null)
                Document.DisableNeedsSave = !myDiagram.Model.IsModified;
            myDiagram.RollbackTransaction();
            Document.DisableNeedsSave = false;
        }

        public void EndEdit()
        {
            if (myDiagram.Model != null)
                Document.DisableNeedsSave = !myDiagram.Model.IsModified;
            myDiagram.CommitTransaction("PropertyChanging");
            Document.DisableNeedsSave = false;
        }

        #endregion IEditableObject Members

        #region Properties

        DocumentEditorDocument _Document;
        [Browsable(false)]
        public DocumentEditorDocument Document
        {
            get
            {
                return _Document;
            }
            private set
            {
                _Document = value;
            }
        }

        #endregion

        #region Methods
        internal void SelectElement(String path)
        {
            if (string.IsNullOrEmpty(path))
                return;
            string[] _path = path.Split('#');

            if (Document != null && Document.Model != null)
            {
                try
                {
                   myDiagram.SelectedNode = (from Node node in myDiagram.Nodes.Reverse()
                     where (node.Data as GateData) != null &&
                     (node.Data as GateData).Key == _path[0] &&
                     (node.Data as GateData).Text == _path[1]
                     select node).FirstOrDefault();
                }
                catch (Exception)
                {
                }
            }
        }
        internal void OnActivate()
        {
            if (bOpening || bRuntime)
                return;

            Document.DisableNeedsSave = true;
            myDiagram.Model = null;
            myDiagram.Model = Document.Model;
            myDiagram.StartTransaction("Opening");
            myDiagram.AllowDrop = true;
            SetCurrentSelection();
            myDiagram.CommitTransaction("Opening");
            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                if (Document != null)
                    Document.DisableNeedsSave = false;
            });
            // SetCurrentSelection();
        }

        void SetCurrentSelection()
        {
            if (workspace == null)
                return;
            var selected = myDiagram.SelectedParts;
            if (selected.Count > 0)
            {
                var list = (from c in selected where c.Data is GateData select c.Data as GateData).ToList();
                if (list.Count > 1)
                    workspace.ContextObjects = list;
                else if (list.Count == 1)
                    workspace.ContextObject = list[0];
                else
                    workspace.ContextObject = Document;
            }
            else
                workspace.ContextObject = Document;
        }

        internal bool SaveCurrentDocument()
        {
            return Document.SaveCurrentDocument();
        }

        #endregion

        #region Commands

        private void OnCommandSave(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            SaveCurrentDocument();
        }

        private void CanCommandSave(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document.NeedsSave;
        }

        private void myDiagram_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetCurrentSelection();
        }

        private void OnStartTestCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Document.Start(true, true);
        }

        private void CanStartTestCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document != null && !Document.InExecution;
        }

        private void OnStopTestCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Document.Stop();
        }

        private void CanStopTestCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document != null && Document.InExecution;
        }

        #endregion

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;
        }
        #endregion

        private void myDiagram_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (bRuntime)
            {
                e.Handled = true;
                return;
            }

            // When you double-click, toggles value of Input node
            Node input = Part.FindAncestor<Node>(e.OriginalSource as FrameworkElement);
            if (input != null)
            {
                GateData d = input.Data as GateData;
                if (d != null)
                {
                    if (d.CanBeToggled())
                    {
                        myDiagram.StartTransaction("Toggle");
                        d.Value = !d.Value;
                        myDiagram.CommitTransaction("Toggle");
                    }
                    else if (Document != null && !Document.InExecution)
                        d.OnDblClick();
                }
            }
        }

        #region Drag Drop
        private void myDiagram_Drop(object sender, DragEventArgs e)
        {
            if (bRuntime)
            {
                e.Handled = true;
                return;
            }

            var targetElement = sender as FrameworkElement;
            var dropPosition = e.GetPosition(myDiagram.Panel);
            dropPosition.X += myDiagram.Panel.HorizontalOffset + myDiagram.Panel.DiagramBounds.Left;
            dropPosition.Y += myDiagram.Panel.VerticalOffset + myDiagram.Panel.DiagramBounds.Top;

            if (e.Data.GetDataPresent(typeof(RecordDragDropData)))
            {
                Dispatcher.InvokeIfRequired(
                (Action)delegate
                {
                    var data = e.Data.GetData(typeof(RecordDragDropData)) as RecordDragDropData;
                    if (data == null)
                        return;

                    foreach (var node in myDiagram.Nodes.Reverse())
                    {
                        var rect = new Rect(node.Location, new Size(node.ActualWidth, node.ActualHeight));
                        if (rect.Contains(dropPosition))
                        {
                            var gateData = node.Data as GateData;
                            if (gateData.CanDropTag())
                            {
                                myDiagram.StartTransaction("Drop");

                                foreach (var v in data.Records)
                                {
                                    var subitem = v as TreeItemControl;
                                    if (subitem == null || subitem.TreeItemInnerObject == null)
                                        continue;

                                    if (subitem.TreeItemInnerObject is ReferenceDescriptionViewModel)
                                    {
                                        var model = subitem.TreeItemInnerObject as ReferenceDescriptionViewModel;
                                        var tag = model.CreateEntityReference(null);
                                        gateData.DroppingTag(tag, tag.HumanReadable);
                                        break;
                                    }
                                    else if (subitem.TreeItemInnerObject is IDocumentManager)
                                    {
                                        var model = subitem.TreeItemInnerObject as IDocumentManager;
                                        var reference = model.DragContent as OPCUAEntityReference;
                                        if (reference != null)
                                        {
                                            gateData.DroppingTag(reference, reference.HumanReadable);
                                        }
                                        break;
                                    }
                                    else if (subitem.TreeItemInnerObject is OPCUAEntityReference)
                                    {
                                        var model = subitem.TreeItemInnerObject as OPCUAEntityReference;
                                        gateData.DroppingTag(model, model.HumanReadable);
                                        break;
                                    }
                                }

                                myDiagram.CommitTransaction("Drop");
                                myDiagram.Select(node);
                                break;
                            }
                        }
                    }
                });
            }
        }

        private void myDiagram_DragOver(object sender, DragEventArgs e)
        {
            if (bRuntime)
            {
                e.Handled = true;
                return;
            }

            if (e.Data.GetDataPresent(typeof(RecordDragDropData)))
            {
                var targetElement = sender as FrameworkElement;
                var dropPosition = e.GetPosition(myDiagram.Panel);
                dropPosition.X += myDiagram.Panel.HorizontalOffset + myDiagram.Panel.DiagramBounds.Left;
                dropPosition.Y += myDiagram.Panel.VerticalOffset + myDiagram.Panel.DiagramBounds.Top;

                e.Handled = true;
                foreach (var node in myDiagram.Nodes.Reverse())
                {
                    var rect = new Rect(node.Location, new Size(node.ActualWidth, node.ActualHeight));
                    if (rect.Contains(dropPosition))
                    {
                        var gateData = node.Data as GateData;
                        if (gateData.CanDropTag())
                        {
                            e.Effects = DragDropEffects.Copy;
                            return;
                        }
                    }
                }

                e.Effects = DragDropEffects.None;
            }
        }

        #endregion
    }
}
