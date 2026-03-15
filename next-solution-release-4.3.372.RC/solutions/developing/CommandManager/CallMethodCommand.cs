using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using OPCUAViewModel;
#if !NET_STANDARD
using Utilities.WPF;
using UIMsgBoxAlertService.ComponentService;
#endif
#if !WINDOWS_UWP
#if !NET_STANDARD
using System.Windows.Controls;
#endif
#else
using Windows.UI.Xaml.Controls;
using System.Threading.Tasks;
#endif
using Opc.Ua;
using UFInterfaces;
using ViewModelLib;
using DocumentManager.ComponentService;
using Utilities;
using Utilities.Commands;
using System.Windows;
using System.Threading;

namespace CommandManager
{
    [DataContract(Name = "CallMethodCommand", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class CallMethodCommand : CommandManager, ICheckUserCallable
    {
        #region Members
        PropertyObserver<OPCUAEntityReference> observer;
        #endregion

        #region Properties

        [DataMember]
        VariantCollection inputParameters;
        public VariantCollection InputParameters
        {
            get
            {
                return inputParameters;
            }
#if !WINDOWS_UWP
            set
            {
                if (inputParameters == value)
                    return;
                inputParameters = value;
#if !NET_STANDARD
                OnPropertyChanged("InputParameters");
                OnPropertyChanged("CommandSummary");
#endif
            }
#endif
        }

        [DataMember]
        VariantCollection outputParameters;
        public VariantCollection OutputParameters
        {
            get
            {
                return outputParameters;
            }
            set
            {
                if (outputParameters == value)
                    return;
                outputParameters = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("OutputParameters");
                OnPropertyChanged("CommandSummary");
#endif
            }
        }

        #endregion

        #region Overrides

#if !WINDOWS_UWP && !NET_STANDARD
        public override bool this[string propertyName]
        {
            get
            {
                if (propertyName == "CommandSettings")
                {
                    return OpcuaEntityReference != null;
                }

                return base[propertyName];
            }
        }
#endif
        public override String CommandSummary
        {
            get
            {
                if (OpcuaEntityReference == null || !OpcuaEntityReference.IsValid)
                    return String.Empty;
                if (OpcuaEntityReference.ReadablePath == null || OpcuaEntityReference.AppName == null)
                    return String.Empty;
                //return OpcuaEntityReference.HumanReadable;
                Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
                UInt16 ns = (UInt16)(n.Count + 2 - 1);
                string oldChars = string.Format("{0}:", ns);
                return string.Format("{0} ({1})", (OpcuaEntityReference.ReadablePath).Replace(oldChars, ""), OpcuaEntityReference.AppName);
            }
        }

        public override String Name
        {
            get
            {
                return Properties.Resources.CallMethodName;
            }
        }

        public override bool Init(IEntityReference entity, IDocument parent, String sessionname)
        {
            base.Init(entity, parent, sessionname);
            if (OpcuaEntityReference != null)
            {
                observer = new PropertyObserver<OPCUAEntityReference>(OpcuaEntityReference)
                    .RegisterHandler(n => n.NodeIdViewModel, n =>
                        {
                            // observer.UnregisterHandler(p => p.NodeIdViewModel);
                            // Dirty the commands registered with CommandManager,
                            // such as our Save command, so that they are queried
                            // to see if they can execute now.

                            Utilities.Commands.CheckUserCallable.GetInstance().Add(this);
                        })
                     .RegisterHandler(n => n.HumanReadable, n =>
                         {
#if !NET_STANDARD
                             if (entity.ContainedObject is ContentControl && 
                                 (entity.ContainedObject as ContentControl).Content is String)
                             {
                                 (entity.ContainedObject as ContentControl).Content = n.HumanReadable;
                             }
#endif
                         });

                OpcuaEntityReference.Resolve(SessionName, parent);
                OpcuaEntityReference.SetInUse(Entity, true);

                return true;
            }

            return false;
        }


        #region ICheckUserCallable
        public void CheckUserCallable()
        {
            try
            {
                var bCan = OpcuaEntityReference.NodeIdViewModel.IsMethod && OpcuaEntityReference.NodeIdViewModel.IsMethodExecutable;
                if (bCan != bCanExecute)
                {
                    bCanExecute = bCan;

#if !WINDOWS_UWP && !NET_STANDARD
                    if (Control != null)
                    {
                        Control.Dispatcher.BeginInvokeIfRequired(() =>
                        {
                            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                        });
                    }
#endif
                }
            }
            catch (Exception ex)
            {
                if (bCanExecute)
                {
                    bCanExecute = false;
#if !WINDOWS_UWP && !NET_STANDARD
                    if (Control != null)
                    {
                        Control.Dispatcher.BeginInvokeIfRequired(() =>
                        {
                            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                        });
                    }
#endif
                }
                else
                    bCanExecute = false;
            }
        }
        #endregion

#if !WINDOWS_UWP
        public override RemoteExecute RemoteExecute()
        {
            try
            {
                // if (InputParameters == null)
                //    InputParameters = OpcuaEntityReference.NodeIdViewModel.InputArgumentValues;

                OutputParameters = OpcuaEntityReference.NodeIdViewModel.CallMethod(InputParameters != null ? InputParameters.ToArray() : null);
            }
            catch (Exception exception)
            {
                return new RemoteExecute() { ex = exception };
            }
            return null;
        }

#if !NET_STANDARD
        public override bool IsDataCommandSettingAvailable()
        {
            return true;
        }

        public override bool EditDataCommandSetting()
        {
            NodeIdViewModel nodeIdViewModel = OpcuaEntityReference?.NodeIdViewModel;
            if (nodeIdViewModel == null)
            {
                if (OpcuaEntityReference != null)
                {
                    var entity = Entity;
                    if (entity == null)
                        entity = new Executer.EntityWeakReference();

                    var opcuaEntityReference = new OPCUAEntityReference(OpcuaEntityReference);
                    opcuaEntityReference.Resolve(SessionName);
                    opcuaEntityReference.SetInUse(entity, true);
                    using (var cursor = new WaitCursor())
                    {
                        var reach = DateTime.Now.AddSeconds(8);
                        while (opcuaEntityReference.NodeIdViewModel == null && reach > DateTime.Now)
                        {
                            Thread.Sleep(250);
                        }
                    }
                    nodeIdViewModel = opcuaEntityReference.NodeIdViewModel;
                    opcuaEntityReference.SetInUse(entity, false);
                }

                if (nodeIdViewModel == null)
                {
                    IUIMsgBoxAlertService ui = null;
                    if (Parent != null)
                        ui = Parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                    if (ui != null)
                        ui.ShowError(Properties.Resources.CouldNotConnectToMethod);
                    else
                        MessageBox.Show(Properties.Resources.CouldNotConnectToMethod);
                    return false;
                }
            }
            var inputArguments = nodeIdViewModel.InputArgument;
            if (inputArguments == null || inputArguments.Count <= 0)
            {
                InputParameters = nodeIdViewModel.InputArgumentValues;
                return true;
            }
            var userControl = new UserControls.EditMethodCallParameters();
            userControl.gridControl.DataContext = nodeIdViewModel;
            userControl.gridControl.ItemsSource = inputArguments;

            GeneralDialogContent Dialog = new GeneralDialogContent(userControl);
            if (Control == null)
                Dialog.Owner = Application.Current.MainWindow;
            else
                Dialog.Owner = Control.FindParent<Window>();
            Dialog.HelpLink = "EditMethodCallParameters";
            if (Dialog.ShowDialog() != true)
                return false;

            InputParameters = nodeIdViewModel.InputArgumentValues;

            return true;
        }
#endif
        bool bExecuting;
        public override void Execute()
        {
            if (
#if !NET_STANDARD
                IsAccessDenied() || 
#endif
                !CanExecute()
#if !NET_STANDARD
                || !CanExecuteDelayCommand()
#endif                
                )
                return;

            try
            {
#if !WINDOWS_UWP && !NET_STANDARD
                if (InputParameters == null && !EditDataCommandSetting())
                    return;
#endif
                bExecuting = true;
#if !WINDOWS_UWP
                ThreadPool.QueueUserWorkItem((o) =>
#else
                Task.Run(() =>
#endif
                    {
                        try
                        {
                            OutputParameters = OpcuaEntityReference.NodeIdViewModel.CallMethod(InputParameters != null ? InputParameters.ToArray() : null);
                        }
                        catch (Exception ex)
                        {
                            var error = String.Format(Properties.Resources.CallMethodError,
                                OpcuaEntityReference.HumanReadable, ex.Message);
#if !NET_STANDARD
                            if (Control != null)
                            {
#if !WINDOWS_UWP
                                Control.Dispatcher.BeginInvokeIfRequired(() =>
#else
                                RunOnUIThread.RunIfRequired(() =>
#endif
                                {
                                    IUIMsgBoxAlertService ui = null;
                                    if (Parent != null)
                                        ui = Parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                    if (ui != null)
                                        ui.ShowError(error);
#if !WINDOWS_UWP
                                    else
                                        System.Windows.MessageBox.Show(error, Name);
#endif
                                });
                            }
                            else
#endif
                            {
#if !NET_STANDARD
                                IUIMsgBoxAlertService ui = null;
                                if (Parent != null)
                                    ui = Parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                if (ui != null)
                                    ui.ShowError(error);
#if !WINDOWS_UWP
                                else
                                    System.Windows.MessageBox.Show(error, Name);
#endif
#else
                                logCommands.Error(error);
#endif
                            }
                        }
                        bExecuting = false;

                        CheckUserCallable();

#if !WINDOWS_UWP && !NET_STANDARD
                        if (Control != null)
                        {
                            Control.Dispatcher.BeginInvokeIfRequired(() =>
                            {
                                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                            });
                        }
#endif
                    });
            }
            catch (Exception exception)
            {
                var error = String.Format(Properties.Resources.CallMethodError,
                    OpcuaEntityReference.HumanReadable, exception.Message);
#if !NET_STANDARD
                IUIMsgBoxAlertService ui = null;
                if (Parent != null)
                    ui = Parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (ui != null)
                    ui.ShowError(error);
#if !WINDOWS_UWP
                else
                    System.Windows.MessageBox.Show(error, Name);
#endif
#else
                logCommands.Error(error);
#endif
            }
        }

        bool bCanExecute;
        public override bool CanExecute()
        {
            if (!base.CanExecute())
                return false;

            try
            {
                if (bExecuting || OpcuaEntityReference.NodeIdViewModel == null)
                    return false;

                return bCanExecute;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public override void Terminate()
        {
            Utilities.Commands.CheckUserCallable.GetInstance().Remove(this);

            if (OpcuaEntityReference != null)
                OpcuaEntityReference.SetInUse(Entity, false);
            if (observer != null)
            {
                observer.Dispose();
                observer = null;
            }

            base.Terminate();
        }
#endif
#endregion
            }
}
