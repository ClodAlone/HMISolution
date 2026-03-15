using System;
using UFInterfaces.CoreHostComponents;
using UFInterfaces;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.IO;
using System.Windows.Media;
using System.Collections.Generic;
using ActiproSoftware.Windows.Controls.SyntaxEditor;
using ActiproSoftware.Text;
using Utilities;
using Utilities.WPF;
using System.ComponentModel;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using IWorkspace = UFInterfaces.IWorkspace;
using ActiproSoftware.Text.Implementation;

namespace XMLExplorer.ComponentService
{
    public class XMLExplorerComponent : ComponentBase<IXMLExplorer>, IXMLExplorer, IDisposable
    {
        #region Declaration
        Object lockObject = new Object();
        IWorkspace workspace;
        XMLExplorerUI xmlExplorerUI;
        DispatcherOperation IdleExecutionPending;
        bool bLoaded;
        bool bEnableIdleCode;
        bool bPendingIdleCode;
        #endregion

        #region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            GetComponentInterfaces();
        }
        #endregion

        void workspace_WorkspaceLoading(object sender, EventArgs e)
        {
            CreateXMLExplorer();
        }

        private static BitmapImage GetControlImage()
        {
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage("XMLExplorer", "XMLEEditorSmall");
            return bm;
        }

        private void GetComponentInterfaces()
        {
            if (workspace == null)
                workspace = GetService(typeof(IWorkspace)) as IWorkspace;
            if (workspace == null)
                throw new NotImplementedException("Expecting the missing IWorkpsace Interface");

            workspace.WorkspaceLoading += workspace_WorkspaceLoading;
        }

        ContentControl emptyControl;
        public void CreateXMLExplorer()
        {
            lock (lockObject)
            {
                if (xmlExplorerUI != null)
                    return;

                if (emptyControl == null)
                {
                    emptyControl = new ContentControl();

                    workspace.AutoHideAnimationStart += workspace_AutoHideAnimationStart;
                    workspace.AutoHideAnimationStop += workspace_AutoHideAnimationStop;
                    workspace.ContextContentChanging += workspace_ContextContentChanging;
                    workspace.ContextContentChanged += workspace_ContextContentChanged;
                    workspace.DockStateChanged += workspace_DockStateChanged;
                    workspace.ContentRendered += workspace_ContentRendered;
                    workspace.ActiveWindowChanged += workspace_ActiveWindowChanged;

                    if (workspace.HasContentRendered())
                        bEnableIdleCode = true;

                    BitmapImage bm = GetControlImage();

                    workspace.AddDockingChildren(emptyControl, Properties.Resources.XMLExplorer_Title, UFInterfaces.DockState.AutoHidden, UFInterfaces.DockSide.Bottom, false, itemID: nameof(XMLExplorerComponent));
                    workspace.SetDockedElementIcon(emptyControl, new ImageBrush(bm));
                    lastDockSide = workspace.GetElementDockSide(emptyControl);
                }
                else
                {
                    using (var cursor = new WaitCursor())
                    {
                        xmlExplorerUI = new XMLExplorerUI();

                        xmlExplorerUI.Loaded += (o, e) =>
                        {
                            if (bEnableIdleCode)
                            {
                                if (!bLoaded && xmlExplorerUI != null && xmlExplorerUI.IsVisible)
                                {
                                    bLoaded = true;
                                    if (workspace.GetElementDockState(emptyControl) == UFInterfaces.DockState.Document)
                                        PromoteCodeToIdle(true);
                                }
                            }
                        };
                        xmlExplorerUI.Unloaded += (o, e) =>
                        {
                            if (bEnableIdleCode)
                                bLoaded = false;
                        };

                        xmlExplorerUI.TabControl.SelectionChanged += (o, e) =>
                        {
                            if (e.NewSelectedItem is DXTabItem)
                            {
                                var tabitem = e.NewSelectedItem as DXTabItem;
                                if (!(tabitem.Content is Control))
                                {
                                    var fe = tabitem.Content as FrameworkElement;
                                    if (fe != null && fe.Tag is Object)
                                    {
                                        var obj = fe.Tag as Object;
                                        Control editControl = null;
                                        try
                                        {
                                            String title = obj.GetType().ToString();
                                            String str = null;
                                            if (obj is UIElement)
                                            {
                                                UIElement ui = obj as UIElement;
                                                try
                                                {
                                                    if (!ContainsProblematic(ui))
                                                    {
                                                        str = ui.XamlWriterFormatted();
                                                        if (ui is FrameworkElement)
                                                            title = ui is FrameworkElement && !(String.IsNullOrEmpty((ui as FrameworkElement).Name)) ? (ui as FrameworkElement).Name : ui.DependencyObjectType.Name;
                                                    }
                                                    else
                                                        str = obj.ToXml();
                                                }
                                                catch
                                                {
                                                }
                                            }
                                            else
                                                str = obj.ToXml();
                                            if (!String.IsNullOrEmpty(str))
                                            {
                                                if (str.Length > 10240)
                                                {
                                                    editControl = new TextBox
                                                    {
                                                        Text = str,
                                                        DataContext = obj,
                                                        FontFamily = new FontFamily("Verdana"),
                                                        FontSize = 13,
                                                        IsReadOnly = true
                                                    };
                                                }
                                                else
                                                {
                                                    editControl = new SyntaxEditor
                                                    {
                                                        Background = Brushes.White,
                                                        Text = str,
                                                        FontFamily = new FontFamily("Verdana"),
                                                        FontSize = 13,
                                                        IsWordWrapEnabled = true,
                                                        IsVirtualSpaceAtLineEndEnabled = true,
                                                        IsLineNumberMarginVisible = true,
                                                    };
                                                    (editControl as SyntaxEditor).Document.IsReadOnly = true;
                                                    (editControl as SyntaxEditor).Document.Language = obj is UIElement ? LoadLanguageDefinitionFromResourceStream("Xaml.langdef") :
                                                                                                                         LoadLanguageDefinitionFromResourceStream("Xml.langdef");
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                        }

                                        if (editControl == null)
                                        {
                                            editControl = new ContentControl()
                                            {
                                                Content = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Text = Properties.Resources.NoXMLCode }
                                            };
                                        }
                                        tabitem.Content = editControl;
                                    }
                                }
                            }
                        };

                        workspace.SetDesiredHeightAndWidthInDockedMode(emptyControl, xmlExplorerUI.Height, xmlExplorerUI.Width);
                        xmlExplorerUI.ClearValue(FrameworkElement.WidthProperty);
                        xmlExplorerUI.ClearValue(FrameworkElement.HeightProperty);

                        emptyControl.Content = xmlExplorerUI;
                    }
                }
            }
        }

        /// <summary>
		/// Loads a language definition (.langdef file) from a resource stream.
		/// </summary>
		/// <param name="filename">The filename.</param>
		/// <returns>The <see cref="ISyntaxLanguage"/> that was loaded.</returns>
		public static ISyntaxLanguage LoadLanguageDefinitionFromResourceStream(string basename)
        {
            using (Stream stream = Utilities.ExtractFileFromResource.Extract(System.Reflection.Assembly.GetExecutingAssembly(),
                                                          String.Format("{0}.Languages.{1}", typeof(XMLExplorerUI).Namespace, basename)))
            {
                if (stream != null)
                {
                    SyntaxLanguageDefinitionSerializer serializer = new SyntaxLanguageDefinitionSerializer();
                    return serializer.LoadFromStream(stream);
                }
                else
                    return SyntaxLanguage.PlainText;
            }
        }

        void workspace_ContentRendered(object sender, EventArgs e)
        {
            bEnableIdleCode = true;
            if (bPendingIdleCode)
            {
                bPendingIdleCode = false;
                PromoteCodeToIdle(false);
            }
        }

        void workspace_DockStateChanged(FrameworkElement sender, UFInterfaces.DockStateEventArgs e)
        {
            if (sender == emptyControl && e.OldState == UFInterfaces.DockState.AutoHidden)
            {
                if (emptyControl != null)
                    emptyControl.Visibility = Visibility.Visible;
                PromoteCodeToIdle(true);
            }
            else if (sender == emptyControl && (e.NewState == UFInterfaces.DockState.Hidden || e.NewState == UFInterfaces.DockState.AutoHidden))
            {
                if (emptyControl != null)
                {
                    emptyControl.Visibility = Visibility.Collapsed;
                    bAutoHideVisible = false;
                }
            }
        }

        bool bContextObjectChanged;
        void workspace_ContextContentChanged(object sender, EventArgs e)
        {
            if (emptyControl != null)
                PromoteCodeToIdle(false);
            bContextObjectChanged = true;
        }

        UFInterfaces.DockSide lastDockSide;
        void workspace_ActiveWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == emptyControl)
            {
                if (lastDockSide == UFInterfaces.DockSide.Tabbed && workspace.GetElementIsSelectedTab(emptyControl))
                {
                    if (emptyControl != null)
                        emptyControl.Visibility = Visibility.Visible;
                    PromoteCodeToIdle(true);
                }
            }
            if (e.OldValue == emptyControl)
            {
                lastDockSide = workspace.GetElementDockSide(emptyControl);
                if (lastDockSide == UFInterfaces.DockSide.Tabbed && !workspace.GetElementIsSelectedTab(emptyControl) &&
                    workspace.GetElementDockState(emptyControl) != UFInterfaces.DockState.Float)
                {
                    if (emptyControl != null)
                        emptyControl.Visibility = Visibility.Collapsed;
                }
            }

            if (emptyControl != null && (e.OldValue == emptyControl || e.NewValue == emptyControl))
            {
                var dockstate = workspace.GetElementDockState(emptyControl);
                if (dockstate == UFInterfaces.DockState.Document)
                {
                    if (e.NewValue == emptyControl && bLoaded)
                        emptyControl.Visibility = Visibility.Visible;
                    //else if (e.OldValue == emptyControl)
                    //    emptyControl.Visibility = Visibility.Collapsed;
                }
            }
        }

        void workspace_ContextContentChanging(object sender, CancelEventArgs e)
        {
        }

        bool bAutoHideVisible = false;
        bool bWasAutoHideVisible = false;
        void workspace_AutoHideAnimationStart(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource == emptyControl)
            {
                CreateXMLExplorer();

                bAutoHideVisible = true;
                if (idleOperation != null)
                {
                    idleOperation.Abort();
                    idleOperation = null;
                }
                // if (bAutoHideVisible)
                {
                    if (emptyControl != null)
                        emptyControl.Visibility = Visibility.Visible;
                }
            }
        }

        DispatcherOperation idleOperation;
        void workspace_AutoHideAnimationStop(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource == emptyControl)
            {
                if (emptyControl != null/* && workspace.ActiveWindow != emptyControl*/)
                {
                    //var dockstate = workspace.GetElementDockState(emptyControl);
                    //if (dockstate == DockState.AutoHidden)
                    {
                        if (bWasAutoHideVisible)
                        {
                            bWasAutoHideVisible = false;
                            if (idleOperation == null)
                            {
                                idleOperation = emptyControl.Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
                                {
                                    if (!Utilities.WPF.DockingHelper.GetLayoutItemVisible(emptyControl))
                                        emptyControl.Visibility = Visibility.Collapsed;
                                    else
                                        bAutoHideVisible = bWasAutoHideVisible = true;
                                });

                                idleOperation.Completed += (o, ev) =>
                                {
                                    idleOperation = null;
                                };
                            }
                            bAutoHideVisible = false;
                        }
                        else
                        {
                            bWasAutoHideVisible = true;
                            PromoteCodeToIdle(false);
                        }
                    }
                }
                else
                    PromoteCodeToIdle(true);
            }
        }

        private void PromoteCodeToIdle(bool bSynchro)
        {
            if (!bEnableIdleCode)
            {
                bPendingIdleCode = true;
                return;
            }

            var dockstate = workspace.GetElementDockState(emptyControl);
            bool bVisible = emptyControl.Visibility == Visibility.Visible &&
                (dockstate == UFInterfaces.DockState.Document && bLoaded || dockstate != UFInterfaces.DockState.Document && dockstate != UFInterfaces.DockState.AutoHidden ||
                 bAutoHideVisible && dockstate == UFInterfaces.DockState.AutoHidden);

            if (IdleExecutionPending == null && (bSynchro || bVisible))
            {
                Action action = () => IdleExecution();
                Dispatcher disp = emptyControl != null ? emptyControl.Dispatcher : Dispatcher.CurrentDispatcher;
                IdleExecutionPending = disp.BeginInvoke(action, bSynchro ? DispatcherPriority.Send : DispatcherPriority.Background);
                IdleExecutionPending.Completed += (sender, e) => IdleExecutionPending = null;
            }
        }

        static bool ContainsProblematic(UIElement element)
        {
            var viewbox = element as Viewbox;
            if (viewbox != null)
            {
                return ContainsProblematic(viewbox.Child);
            }
            var contentControl = element as ContentControl;
            if (contentControl != null)
            {
                return ContainsProblematic(contentControl.Content as UIElement);
            }
            var panel = element as Panel;
            if (panel == null)
                return false;
            foreach(UIElement el in panel.Children)
            {
                if (Utilities.WPF.XmlHelper.IsProblematicXamlWriter(el))
                    return true;

                if (ContainsProblematic(el))
                    return true;
            }

            return false;
        }

        private void IdleExecution()
        {
            using (new WaitCursor())
            {
                CreateXMLExplorer();
                if (!bContextObjectChanged)
                    return;
                bContextObjectChanged = false;

                xmlExplorerUI.TabControl.Visibility = Visibility.Collapsed;
                xmlExplorerUI.TabControl.Items.Clear();

                List<Object> list = new List<Object>();
                if (workspace.ContextObject != null)
                {
                    list.Add(workspace.ContextObject is IEntityReference && (workspace.ContextObject as IEntityReference).ContainedObject != null ? (workspace.ContextObject as IEntityReference).ContainedObject : workspace.ContextObject);
                }
                else if (workspace.ContextObjects != null)
                {
                    foreach (Object o in workspace.ContextObjects)
                    {
                        list.Add(o is IEntityReference && (o as IEntityReference).ContainedObject != null ? (o as IEntityReference).ContainedObject : o);
                    }
                }

                xmlExplorerUI.TabControl.BeginInit();
                list.ForEach(obj =>
                {
                    try
                    {
                        DXTabItem tabitem = new DXTabItem
                        {
                            Header = obj.GetType().ToString(),
                            Content = new FrameworkElement() { Tag = obj },
                            IsSelected = false
                        };
                        tabitem.InitItemTemplate();
                        xmlExplorerUI.TabControl.Items.Add(tabitem);
                    }
                    catch (Exception ex)
                    { }
                });
                xmlExplorerUI.TabControl.EndInit();

                if (xmlExplorerUI.TabControl.Items.Count > 0)
                {
                    xmlExplorerUI.TabControl.SelectedIndex = 0;
                    xmlExplorerUI.TabControl.Visibility = Visibility.Visible;
                    xmlExplorerUI.TextNoItems.Visibility = Visibility.Collapsed;
                }
                else
                    xmlExplorerUI.TextNoItems.Visibility = Visibility.Visible;
            }
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            xmlExplorerUI = null;

            if (workspace != null)
            {
                workspace.AutoHideAnimationStop -= workspace_AutoHideAnimationStop;
                workspace.AutoHideAnimationStart -= workspace_AutoHideAnimationStart;
                workspace.ContextContentChanging -= workspace_ContextContentChanging;
                workspace.ContextContentChanged -= workspace_ContextContentChanged;
                workspace.DockStateChanged -= workspace_DockStateChanged;
                workspace.ContentRendered -= workspace_ContentRendered;
                workspace.ActiveWindowChanged -= workspace_ActiveWindowChanged;
            }

            if (IdleExecutionPending != null)
            {
                IdleExecutionPending.Abort();
                IdleExecutionPending = null;
            }

            lockObject = null;

            if (xmlExplorerUI != null)
            {
                foreach (DXTabItem item in xmlExplorerUI.TabControl.Items)
                {
                    if (item.Content is IDisposable)
                        (item.Content as IDisposable).Dispose();
                }
                xmlExplorerUI.TabControl.Items.Clear();
                xmlExplorerUI.TabControl.Dispose();
            }
        }

        #endregion
    }
}
