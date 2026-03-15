using DocumentManager.ComponentService;
using Opc.Ua;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
#if !WINDOWS_UWP
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
using System.Threading.Tasks;
#endif
using UFInterfaces;
using UFUAEditor.ComponentService;
using Utilities;
using Utilities.Commands;
using ViewModelLib;

namespace AuditTrace
{
    public class AuditInputParameters
    {
        public NodeId TagNodeId { get; set; }

        public string AuditValue { get; set; }

        public string AuditComment { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public Guid TokenId { get; set; }
    }

    public class AuditOutputParameters
    {
        public string requiredUserRole { get; set; }

        public int requiredUserLevel { get; set; }
    }

    public class AuditWriteCommand : ICheckUserCallable
    {
        #region Declarations
        OPCUAEntityReference auditMethod;
        PropertyObserver<OPCUAEntityReference> observer;

        IEntityReference entity;
        #endregion

        #region Methods
        public bool Init(IEntityReference entity, IDocument parent, String sessionName)
        {
            this.entity = entity;

            var uaEditorManager = parent.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (uaEditorManager != null)
            {
                var opcString = uaEditorManager.GetNodeIdEntityReference(parent,
                        UFUAServerInfo.BrowserNames.WriteAuditValue,
                        UFUAServerInfo.Guids.RootTagsGuid.ToString());
                if (!String.IsNullOrEmpty(opcString))
                {
                    auditMethod = opcString.FromXml<OPCUAEntityReference>();
                }
            }

            if (auditMethod != null)
            {
                observer = new PropertyObserver<OPCUAEntityReference>(auditMethod)
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
                         if (entity.ContainedObject is ContentControl &&
                             (entity.ContainedObject as ContentControl).Content is String)
                         {
                             (entity.ContainedObject as ContentControl).Content = n.HumanReadable;
                         }
                     });

                auditMethod.Resolve(sessionName);
                auditMethod.SetInUse(entity, true);

                return true;
            }

            return false;
        }

        public void Terminate()
        {
            Utilities.Commands.CheckUserCallable.GetInstance().Remove(this);

            if (auditMethod != null)
                auditMethod.SetInUse(entity, false);

            if (observer != null)
            {
                observer.Dispose();
                observer = null;
            }
        }

        bool bCanExecute;
        public bool CanExecute()
        {
            if (auditMethod == null || auditMethod.NodeIdViewModel == null)
                return false;

            return bCanExecute;
        }

        public AuditOutputParameters Execute(AuditInputParameters parameters)
        {
            var inputs = new VariantCollection();
            inputs.Add(new Variant(parameters.TagNodeId));
            inputs.Add(new Variant(parameters.AuditValue));
            inputs.Add(new Variant(parameters.AuditComment));
            inputs.Add(new Variant(parameters.UserName ?? String.Empty));
            inputs.Add(new Variant(parameters.Password ?? String.Empty));
            inputs.Add(new Variant(parameters.TokenId.ToString()));

            var outputs = auditMethod.NodeIdViewModel.CallMethod(inputs.ToArray());
            if (outputs == null || outputs.Count == 0)
                return null;

            var result = new AuditOutputParameters();
            if (outputs.Count > 0)
            {
                try
                {
                    result.requiredUserRole = Convert.ToString(outputs[0].Value);
                    if (!String.IsNullOrEmpty(result.requiredUserRole))
                        result.requiredUserRole = String.Format("\\{0}", result.requiredUserRole);
                }
                catch
                { }
            }


            if (outputs.Count > 1)
            {
                try
                {
                    result.requiredUserLevel = Convert.ToInt32(outputs[1].Value);
                }
                catch
                { }
            }

            return result;
        }
        #endregion

        #region ICheckUserCallable
        public void CheckUserCallable()
        {
            try
            {
                var bCan = auditMethod.NodeIdViewModel.IsMethod && auditMethod.NodeIdViewModel.IsMethodExecutable;
                if (bCan != bCanExecute)
                {
                    bCanExecute = bCan;
                }
            }
            catch (Exception ex)
            {
                bCanExecute = false;
            }
        }
        #endregion

    }
}
