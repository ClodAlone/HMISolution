using System;
using System.Text;
using UFRecipeExecutionContext;
using Opc.Ua;

namespace UFRecipeExecuter
{
    internal class RecipeAction
    {
        #region Declarations
        readonly NodeId nodeId;
        readonly RecipeExecutionContext executionContext;
        readonly Action action;
        #endregion

        #region Constructors
        public RecipeAction(NodeId nodeId, Action action)
        {
            this.nodeId = nodeId;
            this.action = action;

            SetIndentity();
        }

        public RecipeAction(RecipeExecutionContext executionContext, Action action)
        {
            this.executionContext = executionContext;
            this.action = action;

            SetIndentity();
        }
        #endregion

        #region Private Methods
        void SetIndentity()
        {
            if (nodeId != null)
            {
                indentity = String.Format("{0}", nodeId);
            }
            else if (executionContext != null)
            {
                if (executionContext.CommandType == RecipeCommandType.Activate ||
                    executionContext.CommandType == RecipeCommandType.Load ||
                    executionContext.CommandType == RecipeCommandType.Read ||
                    executionContext.CommandType == RecipeCommandType.Remove)
                {
                    indentity = String.Format("{0}:{1}", executionContext.CommandType, 
                        executionContext.Index);
                }
                else if (executionContext.CommandType == RecipeCommandType.Export ||
                        executionContext.CommandType == RecipeCommandType.Import)
                {
                    indentity = String.Format("{0}:{1}.{2}", executionContext.CommandType, 
                        executionContext.Index, executionContext.FilePathName);
                }
                else if (executionContext.CommandType == RecipeCommandType.Save)
                {
                    var hashCode = new StringBuilder();
                    foreach (var value in executionContext.Values.Values)
                        hashCode.Append(value);

                    indentity = String.Format("{0}:{1}.{2}", executionContext.CommandType, 
                        executionContext.Index, hashCode);
                }
                else
                    throw new ArgumentOutOfRangeException("CommandType");
            }
            else
                throw new InvalidOperationException("Cannot set a valid indentity");
        }
        #endregion

        #region Public Methods
        public void Execute()
        {
            action();
        }
        #endregion

        #region Public Properties
        string indentity;
        public string Identity
        {
            get
            {
                return indentity;
            }
        }

        public bool IsCommandAction
        {
            get
            {
                return executionContext != null;
            }
        }
        #endregion
    }
}
