using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFRecipeSettings.UFRecipeModel;

namespace UFRecipeEditor.CsvHelper
{
    public enum OperationType
    {
        Import,
        Export
    }

    public class CsvResultEventArgs : EventArgs
    {
        public CsvResultEventArgs(UFRecipeEntity recipeEntity, string csvFile, OperationType type, bool result)
        {
            NewRecipeEntity = recipeEntity;
            CsvFile = csvFile;
            Type = type;
            Result = result;
        }

        public UFRecipeEntity NewRecipeEntity { get; private set; }
        public string CsvFile { get; private set; }
        public OperationType Type { get; private set; }
        public bool Result { get; private set; }
        
    }
}
