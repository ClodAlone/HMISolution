using Opc.Ua;
using OPCUAViewModel;
using System;
using DocumentManager.ComponentService;
using UFRecipeEditor.ComponentService;
using UFRecipeSettings.Documents;
using Utilities;

namespace UFRecipeExecuter.OPCUA
{
    internal class RecipeMethodCall
    {
        #region Declarations
        readonly IDocument parent;
        readonly Uri recipeUri;
        readonly String methodName;
        readonly Object lockObject = new Object();

        UFRecipeDocument recipeDocument;
        OPCUAEntityReference serverReference;

        bool bDisposeRecipeDocument;
        bool bExecuted;
        #endregion

        #region Constructors
        public RecipeMethodCall(IDocument parent, Uri recipeUri, String methodName)
        {
            this.parent = parent;
            this.recipeUri = recipeUri;
            this.methodName = methodName;
        }

        public RecipeMethodCall(UFRecipeDocument document, String methodName)
        {
            this.recipeUri = new Uri(document.FullPath, UriKind.RelativeOrAbsolute);
            this.recipeDocument = document;
            this.methodName = methodName;
            this.parent = document.Parent;
        }
        #endregion

        #region Methods
        public void PrepareExecution(String sessionName)
        {
            if (bExecuted || RecipeManager == null)
                return;
            bExecuted = true;

            if (recipeDocument == null)
            {
                Uri uri = parent.MakeAbosoluteUri(recipeUri);
                recipeDocument = UFRecipeDocument.FromFile(uri.GetPathString(), parent);
                bDisposeRecipeDocument = true;
            }
            if (recipeDocument != null)
            {
                var stringReference = RecipeManager.GetRecipeUAServerEntityReference(parent, RecipeUAConnector.GetRelativePath(recipeUri, parent, methodName), RecipeUAConnector.GetNodeId(recipeUri, parent, methodName));
                if (stringReference != null)
                {
                    serverReference = stringReference.FromXml<OPCUAEntityReference>();
                    if (serverReference != null)
                    {
                        serverReference.Resolve(sessionName);
                        serverReference.SetInUse(recipeDocument.RecipeEntity, true);
                    }
                }
            }
        }

        public void TerminateExecution()
        {
            if (!bExecuted)
                return;
            bExecuted = false;

            if (serverReference != null)
            {
                serverReference.SetInUse(recipeDocument.RecipeEntity, false);
                serverReference = null;
            }

            if (bDisposeRecipeDocument && recipeDocument != null)
            {
                recipeDocument.Dispose();
                recipeDocument = null;
            }
        }

        public bool CanExecute()
        {
            bool executable = false;
            try
            {
                executable = serverReference.NodeIdViewModel.IsMethodExecutable;
            }
            catch { }

            return serverReference != null &&
                serverReference.NodeIdViewModel != null &&
                serverReference.NodeIdViewModel.IsMethod &&
                executable;
        }

        public VariantCollection Execute(VariantCollection inParameters)
        {
            if (!CanExecute())
                throw new Opc.Ua.ServiceResultException(Opc.Ua.StatusCodes.BadInvalidState);

            try
            {
                var diagnosticMask = DiagnosticsMasks.LocalizedText | DiagnosticsMasks.ServiceLocalizedText | DiagnosticsMasks.OperationLocalizedText;
                return serverReference.NodeIdViewModel.CallMethod(new RequestHeader() { ReturnDiagnostics = (uint)diagnosticMask }, inParameters.ToArray());
            }
            catch (ServiceResultException ex)
            {
                if (ex.LocalizedText == String.Format("{0:X8}", ex.Result.StatusCode.Code))
                    throw new Opc.Ua.ServiceResultException(ex.Result.StatusCode);

                throw;
            }
        }

        public VariantCollection Execute()
        {
            if (!CanExecute())
                throw new Opc.Ua.ServiceResultException(Opc.Ua.StatusCodes.BadInvalidState);

            return serverReference.NodeIdViewModel.CallMethod();
        }
        #endregion

        #region Properties
        IRecipeEditorManager recipeManager;
        internal IRecipeEditorManager RecipeManager
        {
            get
            {
                if (recipeManager == null)
                {
                    recipeManager = parent.GetService(typeof(IRecipeEditorManager)) as IRecipeEditorManager;
                }

                return recipeManager;
            }
        }
        #endregion
    }
}
