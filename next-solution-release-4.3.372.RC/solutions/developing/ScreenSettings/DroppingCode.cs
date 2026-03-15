using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ScreenSettings.Entities;
using WinWrap.Basic;
using System.Reflection;
using System.Windows.Interop;
using System.Collections;
using System.Windows.Threading;
using Utilities;
using System.Windows.Controls;
using UFUAEditor.ComponentService;
using System.ComponentModel;

namespace ScreenSettings
{
    public class DroppingCode : IDisposable
    {
        #region Declarations
        BasicIdeObj basicCtl;
        bool bDontRaiseSecondError;
        #endregion

        #region Constructor
        public DroppingCode(String code, String entityName, UIElement element, ScreenEntity entity = null)
        {
            Code = code;
            EntityName = entityName;
            Element = element;
            Entity = entity;
        }
        #endregion
        
        #region Properties
        private String _Code;
        public String Code
        {
            get { return _Code; }
            internal set
            {
                _Code = value;
            }
        }

        private String _EntityName;
        public String EntityName
        {
            get { return _EntityName; }
            internal set
            {
                _EntityName = value;
            }
        }
        
        private UIElement _Element;
        public UIElement Element
        {
            get { return _Element; }
            internal set
            {
                _Element = value;
            }
        }

        private ScreenEntity _Entity;
        public ScreenEntity Entity
        {
            get { return _Entity; }
            internal set
            {
                _Entity = value;
            }
        }
        #endregion

        #region Events
        public event EventHandler<CancelEventArgs> Customizing;
        internal virtual void OnCustomizing(CancelEventArgs ea)
        {
            var e = Customizing;
            if (e != null)
            {
                e(this, ea);
            }
        }

        public event EventHandler<VariableEventArgs> CreatingVariable;
        internal virtual void OnCreatingVariable(VariableEventArgs ea)
        {
            var e = CreatingVariable;
            if (e != null)
            {
                e(this, ea);
            }
        }

        public event EventHandler<VariableEventArgs> VariableCreated;
        internal virtual void OnVariableCreated(VariableEventArgs ea)
        {
            var e = VariableCreated;
            if (e != null)
            {
                e(this, ea);
            }
        }
        #endregion

        #region Methods
        bool InExecuting;
        public void ExecuteScriptCode(CancelEventArgs cancelArgs, VariableEventArgs args = null, bool creating = false)
        {
            if (InExecuting)
                return;
            InExecuting = true;
            if (!String.IsNullOrEmpty(Code))
            {
                bDontRaiseSecondError = false;
                basicCtl = new BasicIdeObj();

#if DEBUG
                if (System.IO.File.Exists(@"C:\Program Files (x86)\Polar Engineering\WinWrap Basic\Certificates\Application-a67e0d79.htm"))
#endif
                basicCtl.Secret = new Guid(Properties.Settings.Default.SecretKey);

                basicCtl.Initialize();
                basicCtl.Caption = EntityName;
                basicCtl.LargeIcon = null;
                basicCtl.SmallIcon = null;
                basicCtl.TaskbarIconMode = WinWrap.Basic.TaskbarIconModeConstants.IconNoneSysmenuNone;
                // basicCtl.AttachToWindow(Window.GetWindow(Element), ManageConstants.All);
                basicCtl.AttachToWindow(null, ManageConstants.Disconnecting);

                basicCtl.OverrideModalWindowOwner += (o, e) =>
                {
                    var wnd = Window.GetWindow(Element);
                    if (wnd != null)
                    {
                        if (wnd != null)
                        {
                            e.OwnerHandle = new WindowInteropHelper(wnd).Handle;
                            wnd.Focus();
                        }
                    }
                };
                basicCtl.ErrorAlert += (o, e) =>
                {
                    if (!bDontRaiseSecondError)
                    {
                        bDontRaiseSecondError = true;
                        basicCtl.CreateOverlappedWindow();
                        basicCtl.WindowState = WindowState.Maximized;
                        basicCtl.Run = true;

                        MessageBox.Show(basicCtl.Error.ToString());
                    }
                };

                basicCtl.DoEvents += (o, e) =>
                {
                    WaitForPriority.DoEvents();
                };

                //Assembly wpfCoreAssembly = typeof(CommandBinding).Assembly;
                //basicCtl.AddExtension("#", wpfCoreAssembly);
                //Assembly wpfFrameworkAssembly = typeof(Window).Assembly;
                //basicCtl.AddExtension("#", wpfFrameworkAssembly);

                var opcua = typeof(Opc.Ua.DataValue).Assembly;
                basicCtl.AddExtension("#", opcua);

                var canceleventarg = typeof(CancelEventArgs).Assembly;
                basicCtl.AddExtension("#", canceleventarg);

                var variableeventarg = typeof(VariableEventArgs).Assembly;
                basicCtl.AddExtension("#", variableeventarg);

                basicCtl.AddExtension("$Feature ExtensionCache False", null);
                if (Entity != null)
                {
                    foreach (var reference in Entity.GetReferenceList())
                    {
                        var referenceGetTypeAssembly = reference.GetType().Assembly;
                        var referenceName = Entity.GetReferenceName(reference);

                        if (referenceName.StartsWith("%"))
                            basicCtl.AddExtension(referenceName, reference);
                        else
                        {
                            basicCtl.AddExtension("#", referenceGetTypeAssembly);
                            basicCtl.AddExtensionObjectWithEvents(referenceName, reference);
                        }
                    }
                }
                else
                {
                    foreach (var reference in GetReferenceList())
                    {
                        Assembly referenceGetTypeAssembly = reference.GetType().Assembly;
                        var referenceName = GetReferenceName(reference);
                        if (referenceName.StartsWith("%"))
                            basicCtl.AddExtension(referenceName, reference);
                        else
                        {
                            basicCtl.AddExtension("#", referenceGetTypeAssembly);
                            basicCtl.AddExtensionObjectWithEvents(referenceName, reference);
                        }
                    }
                }

                Assembly referenceGetTypeAssemblyThis = this.GetType().Assembly;
                var referenceNameThis = GetReferenceName(this);
                if (referenceNameThis.StartsWith("%"))
                    basicCtl.AddExtension(referenceNameThis, this);
                else
                {
                    basicCtl.AddExtension("#", referenceGetTypeAssemblyThis);
                    basicCtl.AddExtensionObjectWithEvents(referenceNameThis, this);
                }

                basicCtl.FileTools = false;
                basicCtl.EventMode = true;
                basicCtl.Code = Code;
                basicCtl.Changed = false;
                basicCtl.Run = true;

                if (args != null)
                {
                    if (creating)
                        OnCreatingVariable(args);
                    else
                        OnVariableCreated(args);
                }
                else
                {
                    if (Customizing == null && !bDontRaiseSecondError)
                        MessageBox.Show("Cannot find the dropping event in the dropping code !");
                    else
                        OnCustomizing(cancelArgs);
                }

                basicCtl.Run = false;
                TerminateScriptCode();
            }
            InExecuting = false;
        }

        DispatcherTimer delayTerminate;
        internal void TerminateScriptCode()
        {
            if (basicCtl == null)
                return;

            var ret = basicCtl.Shutdown();
            if (ret < 0)
            {
                if (delayTerminate == null)
                    delayTerminate = new DispatcherTimer();
                delayTerminate.Interval = TimeSpan.FromSeconds(1);
                delayTerminate.Tick += (o, e) =>
                {
                    delayTerminate.Stop();
                    TerminateScriptCode();
                };
                delayTerminate.Start();
                return;
            }

            try
            {
                basicCtl.Disconnect();
                basicCtl.Dispose();
            }
            catch { }
            basicCtl = null;
        }

        public IList GetQuickReferenceList()
        {
            return null;
        }

        internal IList GetReferenceList()
        {
            var list = new List<Object>();
            if (Entity != null)
                list.Add(Entity);
            if (Element != null)
                list.Add(Element);
            list.Add(this);
            return list;
        }

        internal String GetReferenceName(Object var)
        {
            if (var == this)
                return "DroppingCode";
            String title = var.GetType().ToString();
            if (var is UIElement)
            {
                var element = var as UIElement;
                if (element is FrameworkElement)
                    title = element is FrameworkElement && !(String.IsNullOrEmpty((element as FrameworkElement).Name)) ? (element as FrameworkElement).Name : element.DependencyObjectType.Name;
            }

            return title;
        }

        static readonly String baseCode = "'#Language \"WWB.NET\"";

        bool InEditing;
        public bool Edit(Window owner)
        {
            if (InEditing)
                return false;
            InEditing = true;

            bool preventClosing = false;
            var basicIdeCtl = new BasicIdeCtl();

#if DEBUG
            if (System.IO.File.Exists(@"C:\Program Files (x86)\Polar Engineering\WinWrap Basic\Certificates\Application-a67e0d79.htm"))
#endif

            basicIdeCtl.Secret = new Guid(Properties.Settings.Default.SecretKey);

            basicIdeCtl.FileMenuVisible = false;
            basicIdeCtl.FullPopupMenu = true;
            basicIdeCtl.FileTools = false;
            basicIdeCtl.FileDesc = EntityName;
            basicIdeCtl.Caption = EntityName;
            basicIdeCtl.LargeIcon = null;
            basicIdeCtl.SmallIcon = null;
            basicIdeCtl.TaskbarIconMode = TaskbarIconModeConstants.IconNoneSysmenuNone;
            basicIdeCtl.AttachToWindow(owner, ManageConstants.All);
            basicIdeCtl.Width = 400;
            basicIdeCtl.Height = 300;

            basicIdeCtl.OpenSheet += basicIdeCtl_OpenSheet;
            basicIdeCtl.CloseSheet_ += (o, e) =>
                {                    
                    if (basicIdeCtl.Changed)
                        Code = basicIdeCtl.Code;

                    e.Cancel = preventClosing ? 1 : 0;
                };

            GeneralDialogContent Dialog = new GeneralDialogContent(basicIdeCtl, bhandleEnterKey: false)
            {
                Owner = owner,
                HelpLink = "DroppingCodeEditor"
            };

            bool bret = false;            
            Dialog.Closing += (sender, e) =>
            {
                if (basicIdeCtl.Changed)
                {
                    if (Dialog.DialogResult != true)
                    {
                        if (MessageBox.Show(Properties.Resources.ConfirmClosingMessage, Properties.Resources.ConfirmOperation, MessageBoxButton.YesNo) == MessageBoxResult.No)
                        {
                            preventClosing = true;
                            e.Cancel = true;
                        }
                        else
                        {
                            preventClosing = false;
                        }
                    }
                    else
                    {
                        preventClosing = false;
                        bret = true;
                    }
                }
            };

            Dialog.ShowDialog();

            //while(basicIdeCtl.Shutdown() < 0)
            //    WaitForPriority.DoEvents();            

            basicIdeCtl.Disconnect();
            basicIdeCtl.Dispose();

            InEditing = false;
            return bret;
        }

        void basicIdeCtl_OpenSheet(object sender, WinWrap.Basic.Classic.OpenSheetEventArgs e)
        {
            if (!(sender is BasicIdeCtl))
                return;
            BasicIdeCtl basicIdeCtl = sender as BasicIdeCtl;
            basicIdeCtl.OpenSheet -= basicIdeCtl_OpenSheet;

            basicIdeCtl.Dispatcher.BeginInvoke(
            (Action)delegate
            {
                //Assembly wpfCoreAssembly = typeof(CommandBinding).Assembly;
                //basicIdeCtl.AddExtension("#", wpfCoreAssembly);
                //Assembly wpfFrameworkAssembly = typeof(Window).Assembly;
                //basicIdeCtl.AddExtension("#", wpfFrameworkAssembly);

                var opcua = typeof(Opc.Ua.DataValue).Assembly;
                basicIdeCtl.AddExtension("#", opcua);

                var canceleventarg = typeof(CancelEventArgs).Assembly;
                basicIdeCtl.AddExtension("#", canceleventarg);

                var variableeventarg = typeof(VariableEventArgs).Assembly;
                basicIdeCtl.AddExtension("#", variableeventarg);

                if (Entity != null)
                {
                    foreach (var reference in Entity.GetReferenceList())
                    {
                        var referenceGetTypeAssembly = reference.GetType().Assembly;
                        var referenceName = Entity.GetReferenceName(reference);

                        if (referenceName.StartsWith("%"))
                            basicIdeCtl.AddExtension(referenceName, reference);
                        else
                        {
                            basicIdeCtl.AddExtension("#", referenceGetTypeAssembly);
                            basicIdeCtl.AddExtensionObjectWithEvents(referenceName, reference);
                        }
                    }
                }
                else
                {
                    foreach (var reference in GetReferenceList())
                    {
                        Assembly referenceGetTypeAssembly = reference.GetType().Assembly;
                        var referenceName = GetReferenceName(reference);
                        if (referenceName.StartsWith("%"))
                            basicIdeCtl.AddExtension(referenceName, reference);
                        else
                        {
                            basicIdeCtl.AddExtension("#", referenceGetTypeAssembly);
                            basicIdeCtl.AddExtensionObjectWithEvents(referenceName, reference);
                        }
                    }
                }

                Assembly referenceGetTypeAssemblyThis = this.GetType().Assembly;
                var referenceNameThis = GetReferenceName(this);
                if (referenceNameThis.StartsWith("%"))
                    basicIdeCtl.AddExtension(referenceNameThis, this);
                else
                {
                    basicIdeCtl.AddExtension("#", referenceGetTypeAssemblyThis);
                    basicIdeCtl.AddExtensionObjectWithEvents(referenceNameThis, this);
                }

                basicIdeCtl.EventMode = true;
                if (String.IsNullOrEmpty(Code))
                    basicIdeCtl.Code = baseCode;
                else
                    basicIdeCtl.Code = Code;
                basicIdeCtl.Changed = false;
                //basicIdeCtl.AddExtension("$Feature WWB.COM False", null); Tom cat vegna un azzideint
                //basicIdeCtl.AddExtension("$Feature WWB.NET True", null);
            }, DispatcherPriority.ApplicationIdle);
        }

        #endregion

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            //if (disposing)
            //    if (basicCtl != null)
            //    {
            //        basicCtl.Dispose();
            //        basicCtl = null;
            //    }
        }
        ~DroppingCode()
        {
            Dispose(false);
        }
        #endregion
    }
}
