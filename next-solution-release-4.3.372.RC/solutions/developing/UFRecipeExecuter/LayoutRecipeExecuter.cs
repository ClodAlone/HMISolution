using System;
using System.Data;
using DocumentManager.ComponentService;
using UFRecipeLayout;
using UFRecipeLayout.Helpers;
using UFRecipeSettings.Helpers;
using Utilities;

namespace UFRecipeExecuter
{
    public class LayoutRecipeExecuter : IDisposable
    {
        #region Declarations

        readonly LayoutRecipeEditor layoutRecipeEditor;
        readonly UFRecipeExecuter recipeExecuter;

        #endregion

        #region Constructors

        public LayoutRecipeExecuter(UFRecipeExecuter executer, DataSet dataSet, IDocument parent, bool runtime = false, bool bRunningOnServer = false)
        {
            recipeExecuter = executer;
            layoutRecipeEditor = new LayoutRecipeEditor(dataSet, recipeExecuter.RecipeDocument, runtime);
            if (runtime)
            {
                layoutRecipeEditor.ReadEntityHelper = new RecipeEntityHelper(parent, recipeExecuter.UfuaEditorService, recipeExecuter.RecipeDocument.RecipeEntity, false)
                {
                    DefaultTimeout = Properties.Settings.Default.defaultSyncTimeout
                };

                layoutRecipeEditor.WriteEntityHelper = new RecipeEntityHelper(parent, recipeExecuter.UfuaEditorService, recipeExecuter.RecipeDocument.RecipeEntity, true)
                {
                    DefaultTimeout = Properties.Settings.Default.defaultSyncTimeout
                };

                // event fire from the editor when the user click on recipe command button
                layoutRecipeEditor.ExecutedCommand += (s, e) =>
                {
                    if (/*e.CommandType == EditCommandType.New ||*/
                        e.CommandType == EditCommandType.Delete)
                    {
                        try
                        {
                            recipeExecuter.AcceptUpdateData(dataSet, e.RecipeID);
                        }
                        catch (Exception ex)
                        {
                            e.exception = ex;
                        }

                        layoutRecipeEditor.Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                        {
                            recipeExecuter.ForceUpdateRecipeTags();
                        });
                    }
                    else if (e.CommandType == EditCommandType.Save)
                    {
                        try
                        {
                            recipeExecuter.AcceptUpdateData(dataSet, e.RecipeID);
                        }
                        catch (Exception ex)
                        {
                            e.exception = ex;
                        }

                        layoutRecipeEditor.Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                        {
                            recipeExecuter.ForceUpdateRecipeTags();
                        });
                    }
                    else if (e.CommandType == EditCommandType.Write)
                    {
                        try
                        {
                            recipeExecuter.UpdateActivationTime(dataSet, e.RecipeID);
                        }
                        catch (Exception ex)
                        {
                            e.exception = ex;
                        }
                    }
                    else if (e.CommandType == EditCommandType.Reject)
                    {
                        try
                        {
                            recipeExecuter.RejectUpdateData(dataSet, e.RecipeID);
                        }
                        catch (Exception ex)
                        {
                            e.exception = ex;
                        }
                    }
                };
            }

            layoutRecipeEditor.AddAvailableItems(LayoutControlHelper.CompileLayoutItems(recipeExecuter.RecipeDocument.RecipeEntity, layoutRecipeEditor, bRunningOnServer));
            if (recipeExecuter.RecipeDocument.LabelItems != null)
                layoutRecipeEditor.LayoutItems.AddLayoutItemLabelControls(recipeExecuter.RecipeDocument.LabelItems);
            LayoutControlHelper.LoadLayout(recipeExecuter.RecipeDocument, layoutRecipeEditor.LayoutItems, runtime);
        }

        #endregion

        #region Public Methods

        public void PrepareExecution()
        {
            // preparing read/write commands
            if (layoutRecipeEditor.ReadEntityHelper != null)
                layoutRecipeEditor.ReadEntityHelper.PrepareExecution(recipeExecuter.currentSessionName);
            if (layoutRecipeEditor.WriteEntityHelper != null)
                layoutRecipeEditor.WriteEntityHelper.PrepareExecution(recipeExecuter.currentSessionName);
        }

        public void TerminateExecution()
        {
            // terminating read/write commands
            if (layoutRecipeEditor.ReadEntityHelper != null)
                layoutRecipeEditor.ReadEntityHelper.TerminateExecution();
            if (layoutRecipeEditor.WriteEntityHelper != null)
                layoutRecipeEditor.WriteEntityHelper.TerminateExecution();
        }

        #endregion

        #region Properties

        public LayoutRecipeEditor LayoutControl
        {
            get
            {
                return layoutRecipeEditor;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // terminating and dispose read/write commands
            if (layoutRecipeEditor.ReadEntityHelper != null)
            {
                layoutRecipeEditor.ReadEntityHelper.TerminateExecution();
                if (layoutRecipeEditor.ReadEntityHelper is IDisposable)
                    (layoutRecipeEditor.ReadEntityHelper as IDisposable).Dispose();
                layoutRecipeEditor.ReadEntityHelper = null;
            }
            if (layoutRecipeEditor.WriteEntityHelper != null)
            {
                layoutRecipeEditor.WriteEntityHelper.TerminateExecution();
                if (layoutRecipeEditor.WriteEntityHelper is IDisposable)
                    (layoutRecipeEditor.WriteEntityHelper as IDisposable).Dispose();
                layoutRecipeEditor.WriteEntityHelper = null;
            }

            if (layoutRecipeEditor is IDisposable)
                (layoutRecipeEditor as IDisposable).Dispose();
        }

        #endregion
    }
}
