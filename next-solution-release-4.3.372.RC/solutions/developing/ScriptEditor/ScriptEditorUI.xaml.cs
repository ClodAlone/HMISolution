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
using System.ComponentModel;
using ScriptManager.ComponentService;
using ScriptManager.Document;
using System.Windows.Threading;
using WinWrap.Basic;
using Utilities;
using Utilities.WPF;
using System.Reflection;
using ScriptVariableValues;
using Opc.Ua;
using System.Diagnostics;
using System.Windows.Interop;
using Utilities.ProgressDialog;
using System.Threading.Tasks;
using System.Drawing;

namespace ScriptManager
{
    /// <summary>
    /// Interaction logic for ScriptEditorUI.xaml
    /// </summary>
    public partial class ScriptEditorUI : UserControl, IEditableObject, IDisposable
    {
        #region Members
        ScriptManagerComponent EditorComponent;
        Handler handler;
        bool bBasicLoaded;
        #endregion

        bool bLoaded;
        public bool IsLoaded
        {
            get
            {
                return bLoaded;
            }
        }

        bool bExecutingCode;
        bool bSettingCode;
        public ScriptEditorUI(ScriptManagerComponent editorComponent, ScriptDocument doc)
        {
            EditorComponent = editorComponent;
            Document = doc;

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;

                basicIdeCtl.AttachToWindow(null, ManageConstants.OnCaptionChange);
                basicIdeCtl.AttachToWindow(null, ManageConstants.Disconnecting);

                Dispatcher.BeginInvokeAsynchronouslyInRender(() =>
                {
                    basicIdeCtl_OpenSheet(null, null);
                    Visibility = Visibility.Visible;
                });
            };

            InitializeComponent();
            Visibility = Visibility.Hidden;

            Document.GetTagList += (o, ev) =>
            {
                ev.list = editorComponent.UFUAEditor.GetFlatListTags(Document);
            };
            Document.GetPrototypeList += (o, ev) =>
            {
                ev.mapDefinitions = editorComponent.UFUAEditor.GetListPrototypesDesc(Document);
                ev.mapPrototypes = editorComponent.UFUAEditor.GetFlatListPrototypeInstances(Document);
            };
            Document.GetChildsTagList += (o, ev) =>
            {
                ev.map = editorComponent.UFUAEditor.GetFlatListChildTags(Document);
            };
            Document.GetChildsPrototypeList += (o, ev) =>
            {
                ev.mapChildsDefinitions = editorComponent.UFUAEditor.GetListChildsPrototypesDesc(Document);
                ev.mapChildsPrototypes = editorComponent.UFUAEditor.GetFlatListChildsPrototypeInstances(Document);
            };

#if DEBUG
            if (System.IO.File.Exists(@"C:\Program Files (x86)\Polar Engineering\WinWrap Basic\Certificates\Application-a67e0d79.htm"))
#endif
            basicIdeCtl.Secret = new Guid(Properties.Settings.Default.SecretKey);

            basicIdeCtl.Caption = System.IO.Path.GetFileNameWithoutExtension(Document.FullPath);
            // basicIdeCtl.OpenSheet += basicIdeCtl_OpenSheet;

            basicIdeCtl.CloseSheet_ += (o, e) =>
            {
                if (!bSettingCode && e.Sheet == 1)
                    e.Cancel = 1;
            };

            basicIdeCtl.FontChanged += SaveFont;

            basicIdeCtl.MessageHook += basicIdeCtl_MessageHook;
            
            basicIdeCtl.OverrideMenuCommand += (o, e) =>
                {
                    if (e.CmdId == CommandConstants.MacroRun || 
                        e.CmdId == CommandConstants.DebugStepInto || 
                        e.CmdId == CommandConstants.DebugStepOver || 
                        e.CmdId == CommandConstants.DebugStepTo)
                    {
                        if (basicIdeCtl.Pause && e.CmdId == CommandConstants.MacroRun)
                        {
                            basicIdeCtl.Pause = false;
                            e.Cancel = true;
                        }
                        else
                        {
                            if (basicIdeCtl.Run == false)
                            {
                                e.Cancel = true;
                                bExecutingCode = true;
                                basicIdeCtl.Run = true;
                                if (handler.Exists)
                                {
                                    handler.StepInto = e.CmdId != CommandConstants.MacroRun;
                                    try
                                    {
                                        handler.Call();
                                    }
                                    catch (TerminatedException)
                                    {
                                        // script execution has been terminated
                                    }
                                    catch (Exception ex)
                                    {
                                        try
                                        {
                                            handler.ReportError(ex);
                                        }
                                        catch (Exception exe)
                                        {
                                            
                                        }
                                    }
                                }
                                else if(basicIdeCtl.Error != null && basicIdeCtl.Error.Number == 10203)
                                    MessageBox.Show(Properties.Resources.MissingEntryPoint);
                                basicIdeCtl.Run = false;
                                bExecutingCode = false;
                            }
                        }
                    }
                };

            basicIdeCtl.DoEvents += (o, e) =>
                {
                    WaitForPriority.Wait(DispatcherPriority.Background, null);
                };

            basicIdeCtl.ReadMacro += (o, e) =>
                {
                    if (e.FileName.StartsWith("*"))
                    {
                        var filename = e.FileName.Replace("*", "");
                        var uri = Document.MakeAbosoluteUri(new Uri(Document.FullPath, UriKind.RelativeOrAbsolute));
                        var path = System.IO.Path.GetDirectoryName(uri.GetPathString());
                        var fileToRead = String.Format("{0}\\{1}{2}", path, filename, Properties.Settings.Default.DefaultFileExt);
                        var docMacro = ScriptDocument.FromFile(fileToRead, Document);
                        if (docMacro != null)
                        {
                            e.Code = docMacro.Code;
                            docMacro.Dispose();
                            e.Changed = true;
                            e.Cancel = false;
                        };
                    }
                };
        }

        public void LoadFont()
        {
            Font fontToLoad = FontPropertiesHelper.GetProperty("ScriptEditor");
            if (fontToLoad != null)
                basicIdeCtl.Font = fontToLoad;
        }

        void SaveFont(object sender, EventArgs e)
        {
            Font newFont = (sender as BasicIdeCtl).Font;
            FontPropertiesHelper.SetProperty(newFont, "ScriptEditor");
        }

        IntPtr basicIdeCtl_MessageHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //Debug.WriteLine(String.Format("Message {0} received", msg));

            if (msg == 0x0007 /* WM_SETFOCUS */ || 
                msg == 0x0021 /* WM_MOUSEACTIVATE */)
            {
                if (EditorComponent.Workspace.ActiveWindow != this)
                {
                    EditorComponent.Workspace.ActiveWindow = this;
                    Focusable = true;
                    Focus();
                }
            }

            return IntPtr.Zero;
        }

        internal void UpdateBreakPointState()
        {
            try
            {
                if (!bBasicLoaded || basicIdeCtl == null)
                    return;
                Document.Breakpoints = basicIdeCtl.BreakPoints;
            }
            catch (Exception ex)
            {
                
            }
        }

        List<VariableValues> pendingListVariables;
        void AddPendingVariables()
        {
            if (pendingListVariables == null || pendingListVariables.Count == 0 || Document == null || Document.IsDisposed)
            {
                progressBar.Visibility = Visibility.Collapsed;
                return;
            }
            var variableValue = pendingListVariables[0];
            pendingListVariables.RemoveAt(0);
            
            var name = variableValue.GetName();
            if (String.IsNullOrEmpty(name))
                basicIdeCtl.AddExtension("%", variableValue);
            else
                basicIdeCtl.AddExtension(String.Format("%{0}.", name), variableValue);
            Dispatcher.BeginInvokeAsynchronouslyInBackground(AddPendingVariables);
        }

        bool bOpenSheet;
        void basicIdeCtl_OpenSheet(object sender, WinWrap.Basic.Classic.OpenSheetEventArgs e)
        {
            if (bOpenSheet || Document == null)
                return;
            bOpenSheet = true;
            //if (!(sender is BasicIdeCtl))
            //    return;
            //BasicIdeCtl basicIdeCtl = sender as BasicIdeCtl;
            //basicIdeCtl.OpenSheet -= basicIdeCtl_OpenSheet;

            //Dispatcher.BeginInvoke(
            //(Action)delegate
            //{
                var opcua = typeof(DataValue).Assembly;
                basicIdeCtl.AddExtension("#", opcua);

                basicIdeCtl.AddExtension("$Feature ExtensionCache False", null);
            //var dlg = new ProgressDialog()
            //{
            //    AutoShowDelay = 0,
            //    Owner = this.FindParent<Window>() ?? Application.Current.MainWindow,
            //    ProgressBarIndeterminate = true,
            //    DialogText = String.Format(Properties.Resources.AddingProjectVariables, Document.Title),
            //    IsCancellingEnabled = false
            //};

            //dlg.RunWorkerThread(null, (o1, ev1) =>
            //{
            //    var listVariables = Document.GetVariableObjectDispatcher(false);
            //    listVariables.ForEach(variableValue =>
            //    {
            //        var name = variableValue.GetName();
            //        Dispatcher.InvokeIfRequired(() =>
            //            {
            //                if (String.IsNullOrEmpty(name))
            //                    basicIdeCtl.AddExtension("%", variableValue);
            //                else
            //                    basicIdeCtl.AddExtension(String.Format("%{0}.", name), variableValue);
            //            });
            //    });
            //});
            var task1 = Task.Factory.StartNew(delegate
            {
                pendingListVariables = Document.GetVariableObjectDispatcher(false);
            });
            var task2 = task1.ContinueWith(ret =>
            {
                if (pendingListVariables != null && pendingListVariables.Count > 0)
                    Dispatcher.BeginInvokeAsynchronouslyInBackground(AddPendingVariables);
                else
                    progressBar.Visibility = Visibility.Collapsed;
            }, TaskScheduler.FromCurrentSynchronizationContext());

            var listReferences = new List<Object>();
                listReferences.Add(Document);
                listReferences.Add(Document.Parent);
                listReferences.Add(new StartupContext());

                listReferences.ForEach(reference =>
                {
                    Assembly referenceGetTypeAssembly = reference.GetType().Assembly;
                    basicIdeCtl.AddExtension("#", referenceGetTypeAssembly);
                    var type = reference.GetType();
                    var referenceGetTypeToString = type.Name;
                    basicIdeCtl.AddExtensionObjectWithEvents(referenceGetTypeToString, reference);
                });

                bSettingCode = true;
                try
                {
                    basicIdeCtl.Code = !String.IsNullOrEmpty(Document.Code) ? Document.Code : Properties.Settings.Default.DefaultScriptName;
                }
                finally
                {
                    bSettingCode = false;
                }

                basicIdeCtl.OverrideModalWindowOwner += (o, ev) =>
                {
                    ev.OwnerHandle = new WindowInteropHelper(Window.GetWindow(this)).Handle;
                };

                handler = basicIdeCtl.CreateHandler(Properties.Settings.Default.EntryPointSub);
                basicIdeCtl.BreakPoints = Document.Breakpoints;
                basicIdeCtl.SelStart = Document.SelStart;
                basicIdeCtl.SelLength = Document.SelLength;
                if (basicIdeCtl.Changed)
                    basicIdeCtl.Changed = false;

                basicIdeCtl.SortSheets = false;
                basicIdeCtl.ActiveSheetChange += (i, j) =>
                {
                    basicIdeCtl.Locked = basicIdeCtl.ActiveSheet > 1;
                };

                basicIdeCtl.Change += (i, j) =>
                {
                    if (!bExecutingCode && (basicIdeCtl.SheetCount == 1 || basicIdeCtl.ActiveSheet != basicIdeCtl.SheetCount))
                    {
                        Document.Code = basicIdeCtl.Code;
                        Document.SelStart = basicIdeCtl.SelStart;
                        Document.SelLength = basicIdeCtl.SelLength;
                        Document.Breakpoints = basicIdeCtl.BreakPoints;
                        //if (basicIdeCtl.Changed)
                        //    basicIdeCtl.Changed = false;
                    }
                };
                LoadFont();
                bBasicLoaded = true;
            //}, DispatcherPriority.Background);
        }

        #region Properties Members

        ScriptDocument _Document;
        [Browsable(false)]
        public ScriptDocument Document
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

        #region IEditableObject Members

        public void BeginEdit()
        {
        }

        public void CancelEdit()
        {
        }

        public void EndEdit()
        {
        }

        #endregion

        #region IDisposable Members

        DispatcherTimer closingTimer;
        public void Dispose()
        {
            // basicIdeCtl.Changed = false;
            Document = null;
            if (basicIdeCtl != null)
            {
                try
                {
                    basicIdeCtl.Run = false;
                    if (basicIdeCtl.Shutdown() < 0)
                    {
                        if (closingTimer == null)
                        {
                            closingTimer = new DispatcherTimer();
                            closingTimer.Interval = TimeSpan.FromMilliseconds(500);
                            closingTimer.Tick += (o, e) => { Dispose(); };
                            closingTimer.Start();
                        }
                        return;
                    }

                    if (closingTimer != null)
                    {
                        closingTimer.Stop();
                        closingTimer = null;
                    }

                    basicIdeCtl.Disconnect();
                    basicIdeCtl.Dispose();
                    basicIdeCtl = null;
                }
                catch { }
            }

            if (handler != null)
            {
                handler.Dispose();
                handler = null;
            }
        }

        #endregion

        #region Commands

        void OnCommandSave(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Document.SaveCurrentDocument();
        }

        void CanCommandSave(object sender, CanExecuteRoutedEventArgs e)
        {
            UpdateBreakPointState();
            e.CanExecute = Document != null && Document.NeedsSave;
        }

        void OnCutEvent(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            basicIdeCtl.ExecuteMenuCommand(WinWrap.Basic.CommandConstants.EditCut);
        }

        void OnCopyEvent(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            basicIdeCtl.ExecuteMenuCommand(WinWrap.Basic.CommandConstants.EditCopy);
        }

        void OnEditReferences(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            basicIdeCtl.ExecuteMenuCommand(WinWrap.Basic.CommandConstants.EditReferences);
        }

        void CanEditReferences(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document != null && bBasicLoaded && basicIdeCtl != null && basicIdeCtl.IsMenuCommandEnabled(WinWrap.Basic.CommandConstants.EditReferences);
        }

        void OnDebugStepInto(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            basicIdeCtl.ExecuteMenuCommand(WinWrap.Basic.CommandConstants.DebugStepInto);
        }

        void CanDebugStepInto(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document != null && bBasicLoaded && basicIdeCtl != null && basicIdeCtl.IsMenuCommandEnabled(WinWrap.Basic.CommandConstants.DebugStepInto);
        }
        

        void CanExecuteSelected(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document != null && bBasicLoaded && basicIdeCtl != null && basicIdeCtl.IsMenuCommandEnabled(WinWrap.Basic.CommandConstants.EditCut);
        }

        void OnPasteEvent(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            basicIdeCtl.ExecuteMenuCommand(WinWrap.Basic.CommandConstants.EditPaste);
        }

        void CanExecutePaste(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document != null && bBasicLoaded && basicIdeCtl != null && basicIdeCtl.IsMenuCommandEnabled(WinWrap.Basic.CommandConstants.EditPaste);
        }

        void OnUndoEvent(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            basicIdeCtl.ExecuteMenuCommand(WinWrap.Basic.CommandConstants.EditUndo);
        }

        void CanExecuteUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document != null && bBasicLoaded && basicIdeCtl != null && basicIdeCtl.IsMenuCommandEnabled(WinWrap.Basic.CommandConstants.EditUndo);
        }

        void OnRedoEvent(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            basicIdeCtl.ExecuteMenuCommand(WinWrap.Basic.CommandConstants.EditRedo);
        }

        void CanExecuteRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document != null && bBasicLoaded && basicIdeCtl != null && basicIdeCtl.IsMenuCommandEnabled(WinWrap.Basic.CommandConstants.EditRedo);
        }
        #endregion
    }
}
