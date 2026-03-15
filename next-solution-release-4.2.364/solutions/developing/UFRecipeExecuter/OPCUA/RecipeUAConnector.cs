using Opc.Ua;
using System;
using DocumentManager.ComponentService;
using DocumentManager.ComponentService.Helpers;
using UFRecipeSettings.Documents;
using UFRecipeExecutionContext;
using System.Collections.Generic;
using OPCUAViewModel;
using Utilities;
using UFRecipeEditor.ComponentService;

namespace UFRecipeExecuter.OPCUA
{
    public class RecipeUAConnector
    {
        #region Declarations
        readonly UFRecipeDocument recipeDocument;
        readonly ConnectorType connectorType;

        Dictionary<RecipeCommandType, RecipeMethodCall> recipeMethods;
        RecipeMethodCall recipeUpdateTagsMethod;
        RecipeMethodCall recipeReadDataMethod;
        RecipeMethodCall recipeWriteDataMethod;
        RecipeMethodCall recipeGetInDataServerValuesMethod;
        RecipeMethodCall recipeGetOutDataServerValuesMethod;

        internal RecipeUAViewModel recipeUAViewModel;
        bool bAuditTraceFetched;

        static Dictionary<RecipeCommandType, String> recipeMethodNames = new Dictionary<RecipeCommandType, string>
        {
            { RecipeCommandType.Load, RecipeUAServerInfo.MethodNames.RecipeLoadFromDBName },
            { RecipeCommandType.Save, RecipeUAServerInfo.MethodNames.RecipeSaveToDBName },
            { RecipeCommandType.Remove, RecipeUAServerInfo.MethodNames.RecipeDeleteFromDBName},
            { RecipeCommandType.Activate, RecipeUAServerInfo.MethodNames.RecipeWriteToPLCName },
            { RecipeCommandType.Read, RecipeUAServerInfo.MethodNames.RecipeReadFromPLCName },
            { RecipeCommandType.Export, RecipeUAServerInfo.MethodNames.RecipeExportToFileName },
            { RecipeCommandType.Import, RecipeUAServerInfo.MethodNames.RecipeImportFromFileName }
        };
        #endregion

        #region Constructors
        public RecipeUAConnector(UFRecipeDocument document, ConnectorType connectorType)
        {
            this.recipeDocument = document;
            this.connectorType = connectorType;
        }
        #endregion

        #region Methods
        public void PrepareExecution(String sessionName)
        {
            if (recipeUAViewModel == null)
            {
                recipeUAViewModel = new RecipeUAViewModel(recipeDocument);
                recipeUAViewModel.AuditPropertiesFetched += OnAuditFetched;
                recipeUAViewModel.PrepareExecution(sessionName);
            }

            if ((connectorType & ConnectorType.UseStandardMethods) != 0)
            {
                if (recipeMethods == null)
                {
                    recipeMethods = new Dictionary<RecipeCommandType, RecipeMethodCall>();
                    foreach (RecipeCommandType commandType in Enum.GetValues(typeof(RecipeCommandType)))
                    {
                        if (!recipeMethodNames.ContainsKey(commandType))
                            continue;

                        recipeMethods.Add(commandType, new RecipeMethodCall(recipeDocument, recipeMethodNames[commandType]));
                        recipeMethods[commandType].PrepareExecution(sessionName);
                    }
                }
            }

            if ((connectorType & ConnectorType.UseExtendedMethods) != 0)
            {
                if (recipeUpdateTagsMethod == null)
                {
                    recipeUpdateTagsMethod = new RecipeMethodCall(recipeDocument, RecipeUAServerInfo.MethodNames.RecipeUpdateRecipeTagsName);
                    recipeUpdateTagsMethod.PrepareExecution(sessionName);
                }

                if (recipeReadDataMethod == null)
                {
                    recipeReadDataMethod = new RecipeMethodCall(recipeDocument, RecipeUAServerInfo.MethodNames.RecipeReadDataName);
                    recipeReadDataMethod.PrepareExecution(sessionName);
                }

                if (recipeWriteDataMethod == null)
                {
                    recipeWriteDataMethod = new RecipeMethodCall(recipeDocument, RecipeUAServerInfo.MethodNames.RecipeWriteDataName);
                    recipeWriteDataMethod.PrepareExecution(sessionName);
                }

                if (recipeGetInDataServerValuesMethod == null)
                {
                    recipeGetInDataServerValuesMethod = new RecipeMethodCall(recipeDocument, RecipeUAServerInfo.MethodNames.RecipeGetInDataServerValuesName);
                    recipeGetInDataServerValuesMethod.PrepareExecution(sessionName);
                }

                if (recipeGetOutDataServerValuesMethod == null)
                {
                    recipeGetOutDataServerValuesMethod = new RecipeMethodCall(recipeDocument, RecipeUAServerInfo.MethodNames.RecipeGetOutDataServerValuesName);
                    recipeGetOutDataServerValuesMethod.PrepareExecution(sessionName);
                }
            }
        }

        public void TerminateExecution()
        {
            if (recipeUAViewModel != null)
            {
                recipeUAViewModel.AuditPropertiesFetched -= OnAuditFetched;
                recipeUAViewModel.Dispose();
                recipeUAViewModel = null;
            }

            if (recipeMethods != null)
            {
                foreach (var commandType in recipeMethods.Keys)
                    recipeMethods[commandType].TerminateExecution();
                recipeMethods.Clear();
                recipeMethods = null;
            }

            if (recipeUpdateTagsMethod != null)
            {
                recipeUpdateTagsMethod.TerminateExecution();
                recipeUpdateTagsMethod = null;
            }

            if (recipeReadDataMethod != null)
            {
                recipeReadDataMethod.TerminateExecution();
                recipeReadDataMethod = null;
            }

            if (recipeWriteDataMethod != null)
            {
                recipeWriteDataMethod.TerminateExecution();
                recipeWriteDataMethod = null;
            }

            if (recipeGetInDataServerValuesMethod != null)
            {
                recipeGetInDataServerValuesMethod.TerminateExecution();
                recipeGetInDataServerValuesMethod = null;
            }

            if (recipeGetOutDataServerValuesMethod != null)
            {
                recipeGetOutDataServerValuesMethod.TerminateExecution();
                recipeGetOutDataServerValuesMethod = null;
            }
        }

        public bool CanExecute(RecipeCommandType commandType)
        {
            if (commandType == RecipeCommandType.Show)
                return recipeUAViewModel != null && recipeUAViewModel.IsConnected;
            else if (recipeMethods == null || !recipeMethods.ContainsKey(commandType))
                return false;
            else if ((commandType == RecipeCommandType.Save || commandType == RecipeCommandType.Remove || commandType == RecipeCommandType.Activate) && !bAuditTraceFetched)
                return false;
            
            return recipeMethods[commandType].CanExecute();
        }

        public VariantCollection Execute(ExecutionMode mode, RecipeExecutionContext context)
        {
            if (recipeMethods == null || !recipeMethods.ContainsKey(context.CommandType))
                throw new Opc.Ua.ServiceResultException(Opc.Ua.StatusCodes.BadInvalidState);

            VariantCollection inParameters = new VariantCollection();
            if (context.CommandType == RecipeCommandType.Load)
            {
                inParameters.Add(new Variant(context.Index));
                inParameters.Add(new Variant(context.IsSynchro));
            }
            else if (context.CommandType == RecipeCommandType.Remove)
            {
                inParameters.Add(new Variant(context.Index));
                inParameters.Add(new Variant(context.IsSynchro));
                inParameters.Add(new Variant(context.UserComment ?? String.Empty));
            }
            else if (context.CommandType == RecipeCommandType.Save)
            {
                UuidCollection guids = new UuidCollection();
                foreach (var guid in context.Values.Keys)
                    guids.Add(new Uuid(guid));

                VariantCollection values = new VariantCollection();
                foreach (var datavalue in context.Values.Values)
                    values.Add(datavalue);

                inParameters.Add(new Variant(context.Index));
                inParameters.Add(new Variant(guids));
                inParameters.Add(new Variant(values));
                inParameters.Add(new Variant(context.IsSynchro));
                inParameters.Add(new Variant(context.UserComment ?? String.Empty));
            }
            else if (context.CommandType == RecipeCommandType.Export)
            {
                inParameters.Add(new Variant(context.Index));
            }
            else if (context.CommandType == RecipeCommandType.Import)
            {
                UuidCollection guids = new UuidCollection();
                foreach (var guid in context.Values.Keys)
                    guids.Add(new Uuid(guid));

                VariantCollection values = new VariantCollection();
                foreach (var datavalue in context.Values.Values)
                    values.Add(datavalue);

                inParameters.Add(new Variant(context.Index));
                inParameters.Add(new Variant(guids));
                inParameters.Add(new Variant(values));
                inParameters.Add(new Variant(context.IsSynchro));
            }
            else if (context.CommandType == RecipeCommandType.Activate)
            {
                UuidCollection guids = null;
                VariantCollection values = null;

                inParameters.Add(new Variant(context.Index));
                inParameters.Add(new Variant(guids));
                inParameters.Add(new Variant(values));
                inParameters.Add(new Variant(context.IsSynchro));
                inParameters.Add(new Variant(context.Timeout));
                inParameters.Add(new Variant(context.UserComment ?? String.Empty));
            }
            else if (context.CommandType == RecipeCommandType.Read)
            {
                inParameters.Add(new Variant(context.Index));
                inParameters.Add(new Variant(context.IsSynchro));
                inParameters.Add(new Variant(context.Timeout));
            }

            return recipeMethods[context.CommandType].Execute(inParameters);
        }

        public void UpdateRecipeTags()
        {
            if (recipeUpdateTagsMethod == null)
                throw new Opc.Ua.ServiceResultException(Opc.Ua.StatusCodes.BadInvalidState);

            recipeUpdateTagsMethod.Execute();
        }

        public bool CanReadXmlDataSet()
        {
            if (/*!bAuditTraceFetched || */recipeReadDataMethod == null)
                return false;

            return recipeReadDataMethod.CanExecute();
        }

        public VariantCollection ReadXmlDataSet(System.Data.DataRowState rowState, bool bContinueOnError)
        {
            if (recipeReadDataMethod == null)
                throw new Opc.Ua.ServiceResultException(Opc.Ua.StatusCodes.BadInvalidState);

            VariantCollection inParameters = new VariantCollection();
            inParameters.Add(new Variant(rowState.ToString()));
            inParameters.Add(new Variant(bContinueOnError));

            return recipeReadDataMethod.Execute(inParameters);
        }

        public bool CanWriteXmlDataSet()
        {
            if (!bAuditTraceFetched || recipeWriteDataMethod == null)
                return false;

            return recipeWriteDataMethod.CanExecute();
        }

        public void WriteXmlDataSet(String xmlDataSet, System.Data.DataRowState rowState, Guid recipeId, String userComment = null)
        {
            if (recipeWriteDataMethod == null)
                throw new Opc.Ua.ServiceResultException(Opc.Ua.StatusCodes.BadInvalidState);

            VariantCollection inParameters = new VariantCollection();
            inParameters.Add(new Variant(xmlDataSet));
            inParameters.Add(new Variant(rowState.ToString()));
            inParameters.Add(new Uuid(recipeId));
            inParameters.Add(new Variant(userComment ?? String.Empty));

            recipeWriteDataMethod.Execute(inParameters);
        }

        public bool CanGetInDataServerValues()
        {
            if (/*!bAuditTraceFetched || */recipeGetInDataServerValuesMethod == null)
                return false;

            return recipeGetInDataServerValuesMethod.CanExecute();
        }

        public VariantCollection GetInDataServerValues(String xmlDataSet, Guid recipeId, int timeout)
        {
            if (recipeGetInDataServerValuesMethod == null)
                throw new Opc.Ua.ServiceResultException(Opc.Ua.StatusCodes.BadInvalidState);

            VariantCollection inParameters = new VariantCollection();
            inParameters.Add(new Variant(xmlDataSet));
            inParameters.Add(new Uuid(recipeId));
            inParameters.Add(new Variant(timeout));

            return recipeGetInDataServerValuesMethod.Execute(inParameters);
        }

        public bool CanGetOutDataServerValues()
        {
            if (!bAuditTraceFetched || recipeGetOutDataServerValuesMethod == null)
                return false;

            return recipeGetOutDataServerValuesMethod.CanExecute();
        }

        public void GetOutDataServerValues(String xmlDataSet, Guid recipeId, int timeout, String userComment = null)
        {
            if (recipeGetOutDataServerValuesMethod == null)
                throw new Opc.Ua.ServiceResultException(Opc.Ua.StatusCodes.BadInvalidState);

            VariantCollection inParameters = new VariantCollection();
            inParameters.Add(new Variant(xmlDataSet));
            inParameters.Add(new Uuid(recipeId));
            inParameters.Add(new Variant(timeout));
            inParameters.Add(new Variant(userComment ?? String.Empty));

            recipeGetOutDataServerValuesMethod.Execute(inParameters);
        }

        void OnAuditFetched(object s, EventArgs ev)
        {
            bAuditTraceFetched = true;
        }
        #endregion

        #region Static Methods
        internal static String GetNodeId(Uri recipeUri, IDocument parent, String subIndentifier = null)
        {
            if (subIndentifier != null)
                return String.Format("{0}?{1}/{2}", RecipeUAServerInfo.Guids.RecipesRootGuid, GetRecipePath(recipeUri, parent), subIndentifier);
            else
                return String.Format("{0}?{1}", RecipeUAServerInfo.Guids.RecipesRootGuid, GetRecipePath(recipeUri, parent));
        }

        internal static String GetRelativePath(Uri recipeUri, IDocument parent, String subIndentifier = null)
        {
            if (subIndentifier != null)
                return String.Format("{0}/{1}/{2}", RecipeUAServerInfo.RecipeUAServerInfo.GetRecipesRootName(), GetRecipePath(recipeUri, parent), subIndentifier);
            else
                return String.Format("{0}/{1}", RecipeUAServerInfo.RecipeUAServerInfo.GetRecipesRootName(), GetRecipePath(recipeUri, parent));
        }

        internal static String GetRecipePath(Uri recipeUri, IDocument parent)
        {
            string relativePath;
            var rootParent = DocumentHelper.GetRootParent(parent, traverse: false);
#if !NET_STANDARD
            var relativeUri = rootParent.MakeRelativeUri(recipeUri);
            if (relativeUri.IsAbsoluteUri)
            {
                relativePath = new Uri(rootParent.FilePath, UriKind.RelativeOrAbsolute).MakeRelativeUri(relativeUri).GetPathString().Replace('\\', '/');
                relativePath = relativePath.Substring(rootParent.Title.Length + 1);
            }
            else
                relativePath = relativeUri.GetPathString().Replace('\\', '/');
#else
            var projectPath = String.Format("{0}{1}{2}{1}",
                System.IO.Path.GetDirectoryName(rootParent.FilePath),
                System.IO.Path.DirectorySeparatorChar,
                rootParent.Title);
            relativePath = recipeUri.GetPathString().Replace(projectPath, String.Empty).Replace('\\', '/');
#endif

            var manager = parent.GetService(typeof(IRecipeEditorManager)) as IDocumentManager;
            if (manager != null)
            {
                var managerFolder = String.Format("{0}/", manager.TypeLabel);
                if (relativePath.StartsWith(managerFolder))
                    relativePath = relativePath.Substring(managerFolder.Length);
                var managerExtension = manager.FileType;
                if (relativePath.EndsWith(managerExtension))
                    relativePath = relativePath.Substring(0, relativePath.Length - managerExtension.Length);
            }

            return relativePath;
        }
#endregion
    }
}
