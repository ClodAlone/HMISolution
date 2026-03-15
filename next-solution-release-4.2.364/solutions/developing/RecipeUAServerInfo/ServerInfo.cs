using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Opc.Ua;
using Opc.Ua.Configuration;

namespace RecipeUAServerInfo
{
    #region Helper Class
    public class RecipeUAServerInfo
    {
        #region Public Methods
        public static String GetProcessName()
        {
            return Properties.Settings.Default.DefaultServerExe;
        }

        public static String GetSysTrayProcessName()
        {
            return Properties.Settings.Default.SysTrayApp;
        }

        public static String GetServerFolder()
        {
            return GetAssemblyPath();
        }

        public static String GetServerName()
        {
            return System.IO.Path.GetFileNameWithoutExtension(Properties.Settings.Default.DefaultServerExe);
        }

        public static String GetServerPath()
        {
            return String.Format("{0}{1}", GetServerFolder(), Properties.Settings.Default.DefaultServerExe);
        }

        public static string GetServerConfigFile()
        {
            return String.Format("{0}{1}", GetServerFolder(),
                                                Properties.Settings.Default.DefaultServerConfig);
        }

        public static string GetRecipesRootName()
        {
            return Properties.Settings.Default.ASRootName;
        }

        public static IList<string> GetCurrentApplicationBaseAddresses()
        {
            List<string> list = new List<string>();
            ServerConfiguration conf = GetCurrentServerConfiguration();
            if (conf.BaseAddresses.Count > 0)
            {
                list.AddRange(conf.BaseAddresses);
            }
            return list;
        }

        static ServerConfiguration configuration = null;
        public static ServerConfiguration GetCurrentServerConfiguration()
        {
            if (configuration == null)
            {
                var app = Utilities.ApplicationConfigurationHelper.LoadConfiguration(GetServerConfigFile());
                if (app != null)
                    configuration = app.ServerConfiguration;
                else
                    configuration = new ServerConfiguration();
            }

            return configuration;
        }
        #endregion

        #region Private Methods

        static string GetAssemblyPath()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            string s = a.Location.ToLower();
            string name = a.GetName().Name.ToLower() + ".dll";
            int idx = s.IndexOf(name);
            if (idx != -1)
                return a.Location.Substring(0, idx);
            return string.Empty;
        }

        #endregion
    }
    #endregion

    #region Guids Declarations
    public static partial class Guids
    {
        #region Roots
        /// <summary>
        /// The Guid for the root Tags component.
        /// </summary>
        public static Guid RecipesRootGuid = new Guid("{ 0x2548e1e0, 0x755e, 0x4709, { 0xb5, 0x96, 0x4b, 0xf8, 0xe3, 0x80, 0xc2, 0xa7 } }");
        /// <summary>
        /// The Guid for the recipe prototype component.
        /// </summary>
        public static Guid RecipesPrototypeGuid = new Guid("{ 0x5f0cb96, 0xac83, 0x49dc, { 0x92, 0x3e, 0xd1, 0x2f, 0xcb, 0xe2, 0xce, 0x66 } }");
        #endregion
    }
    #endregion

    #region BrowseName Declarations
    public static partial class BrowserNames
    {
        #region BaseObjectTypeState
        /// <summary>
        /// The BrowseName for the recipe prototype component.
        /// </summary>
        public const string RecipePrototypeName = "Recipe";
        #endregion

        #region BaseVariableState
        /// <summary>
        /// The BrowseName for the recipe execution state variable component.
        /// </summary>
        public const string RecipeExecutionStatateName = "ExecutionState";
        #endregion

        #region Descriptions
        /// <summary>
        /// The Description for the recipe prototype component.
        /// </summary>
        public const string RecipePrototypeDesc = "Recipe Prototype";
        /// <summary>
        /// The Description for the recipe execution state variable component.
        /// </summary>
        public const string RecipeExecutionStatateDesc = "Allows to get the uint mask of the last execution state";
        #endregion

        #region Auditing
        /// <summary>
        /// The BrowseName for the IsAuditTraceEnabled component.
        /// </summary>
        public const string IsAuditTraceEnabled = "IsAuditTraceEnabled";
        // <summary>
        /// The BrowseName for the IsCommentRequiredOnAudit component.
        /// </summary>
        public const string IsCommentRequiredOnAudit = "IsCommentRequiredOnAudit";
        // <summary>
        /// The BrowseName for the IsPasswordRequiredOnAudit component.
        /// </summary>
        public const string IsPasswordRequiredOnAudit = "IsPasswordRequiredOnAudit";
        // <summary>
        /// The BrowseName for the MinAccessLevelRequiredOnAudit component.
        /// </summary>
        public const string MinAccessLevelRequiredOnAudit = "MinAccessLevelRequiredOnAudit";
        /// <summary>
        /// The BrowseName for the LastUserNameOnAudit component.
        /// </summary>
        public const string LastUserNameOnAudit = "LastUserNameOnAudit";
        /// <summary>
        /// The BrowseName for the LastCommentOnAudit component.
        /// </summary>
        public const string LastCommentOnAudit = "LastCommentOnAudit";
        #endregion
    }
    #endregion

    #region Recipe Methods Declaration
    public static partial class MethodNames
    {
        #region Methods
        /// <summary>
        /// The BrowseName for the LoadFromDB method.
        /// </summary>
        public const string RecipeLoadFromDBName = "LoadFromDB";
        /// <summary>
        /// The BrowseName for the SaveToDB method.
        /// </summary>
        public const string RecipeSaveToDBName = "SaveToDB";
        /// <summary>
        /// The BrowseName for the DeleteFromDB method.
        /// </summary>
        public const string RecipeDeleteFromDBName = "DeleteFromDB";
        /// <summary>
        /// The BrowseName for the WriteToPLC method.
        /// </summary>
        public const string RecipeWriteToPLCName = "WriteToPLC";
        /// <summary>
        /// The BrowseName for the ReadFromPLC method.
        /// </summary>
        public const string RecipeReadFromPLCName = "ReadFromPLC";
        /// <summary>
        /// The BrowseName for the ExportToFile method.
        /// </summary>
        public const string RecipeExportToFileName = "ExportToFile";
        /// <summary>
        /// The BrowseName for the ImportFromFile method.
        /// </summary>
        public const string RecipeImportFromFileName = "ImportFromFile";
        /// <summary>
        /// The BrowseName for the UpdateRecipeTags method.
        /// </summary>
        public const string RecipeUpdateRecipeTagsName = "UpdateRecipeTags";
        /// <summary>
        /// The BrowseName for the ReadData method.
        /// </summary>
        public const string RecipeReadDataName = "ReadData";
        /// <summary>
        /// The BrowseName for the WriteData method.
        /// </summary>
        public const string RecipeWriteDataName = "WriteData";
        /// <summary>
        /// The BrowseName for the GetInDataServerValues method.
        /// </summary>
        public const string RecipeGetInDataServerValuesName = "GetInDataServerValues";
        /// <summary>
        /// The BrowseName for the GetOutDataServerValues method.
        /// </summary>
        public const string RecipeGetOutDataServerValuesName = "GetOutDataServerValues";
        #endregion

        #region Descriptions
        /// <summary>
        /// The Description for the LoadFromDB method.
        /// </summary>
        public const string RecipeLoadFromDBDesc = "Allows to read the recipe's data from database";
        /// <summary>
        /// The Description for the SaveToDB method.
        /// </summary>
        public const string RecipeSaveToDBDesc = "Allows to save the recipe's data to database ";
        /// <summary>
        /// The Description for the DeleteFromDB method.
        /// </summary>
        public const string RecipeDeleteFromDBDesc = "Allows to delete a recipe from database";
        /// <summary>
        /// The Description for the WriteToPLC method.
        /// </summary>
        public const string RecipeWriteToPLCDesc = "Allows to write the recipe's data to PLC";
        /// <summary>
        /// The Description for the WriteToPLC method.
        /// </summary>
        public const string RecipeReadFromPLCDesc = "Allows to read the recipe's data from PLC";
        /// <summary>
        /// The Description for the ExportToFile method.
        /// </summary>
        public const string RecipeExportToFileDesc = "Allows to export the recipe's data to file";
        /// <summary>
        /// The Description for the ImportFromFile method.
        /// </summary>
        public const string RecipeImportFromFileDesc = "Allows to import the recipe's data from file";
        /// <summary>
        /// The Description for the UpdateRecipeTags method.
        /// </summary>
        public const string RecipeUpdateRecipeTagsDesc = "Allows to updates recipe's tags (list and index)";
        /// <summary>
        /// The Description for the ReadData method.
        /// </summary>
        public const string RecipeReadDataDesc = "Allows to read a data set from the database";
        /// <summary>
        /// The Description for the WriteData method.
        /// </summary>
        public const string RecipeWriteDataDesc = "Allows to write a data set to database";
        /// <summary>
        /// The Description for the GetInDataServerValues method.
        /// </summary>
        public const string RecipeGetInDataServerValuesDesc = "Allows to fill the given data set with values from IO Data Server";
        /// <summary>
        /// The Description for the GetOutDataServerValues method.
        /// </summary>
        public const string RecipeGetOutDataServerValuesDesc = "Allows to write on IO Data Server the values of the given data set";
        #endregion
    }
    #endregion
}
