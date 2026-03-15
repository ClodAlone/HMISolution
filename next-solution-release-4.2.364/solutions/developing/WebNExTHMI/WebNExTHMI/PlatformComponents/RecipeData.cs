using DataReader.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UFRecipeEditor.ComponentService;
using UFRecipeSettings.Helpers;
using UFUAModel.Extensions;
using Utilities;

namespace WebNExTHMI.PlatformComponents
{
    public class RecipeData : IDisposable
    {
        static readonly int recipeGuidLength = Guid.Empty.ToString().Length;

        #region Public Props
        UFRecipeExecuter.UFRecipeExecuter recipeExecuter;
        UFRecipeExecuter.UFRecipeExecuter RecipeExecuter
        {
            get
            {
                if (recipeExecuter == null)
                {
                    recipeExecuter = new UFRecipeExecuter.UFRecipeExecuter(RecipeDocument, UFRecipeExecuter.OPCUA.ConnectorType.UseAllMethods);
                    recipeExecuter.Initialize();
                }
                return recipeExecuter;
            }
        }
        UFRecipeSettings.Documents.UFRecipeDocument recipeDocument;
        internal UFRecipeSettings.Documents.UFRecipeDocument RecipeDocument
        {
            get
            {
                if (recipeDocument == null)
                {
                    var projectDocument = PlatformComponents.GetProjectDocument();
                    var rUri = new Uri(recipePath, UriKind.RelativeOrAbsolute);
                    var recipeUri = projectDocument.MakeAbosoluteUri(rUri);
                    recipeDocument = UFRecipeSettings.Documents.UFRecipeDocument.FromFile(recipeUri.GetPathString(), projectDocument);
                    if (recipeDocument == null)
                        throw new FileNotFoundException(String.Format("InvalidRecipeUri|{0}", recipeUri));
                    recipeDocument.Parent = projectDocument;
                }
                return recipeDocument;
            }
        }

        bool? hasReferences = null;
        internal bool HasReferences
        {
            get 
            {
                if (hasReferences == null && RecipeDocument != null)
                {
                    hasReferences = (from c in recipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                                     where c.IsTagIOReferenceValid()
                                     select c).ToList().Count > 0;
                }

                return hasReferences != null && hasReferences.Value;
            }
        }
        internal string LastFillError
        {
            get;
            private set;
        }
        #endregion
        #region Declarations
        String recipePath;
        DataSet dataSet = new DataSet();
        bool bDataSetLoaded;
        int defaultHelpersTimeout;
        CancellationTokenSource ctsLoading;
        #endregion

        public RecipeData(string recipePath, int defaultTimeout, bool bExecutingCommand = false)
        {
            this.recipePath = recipePath;
            this.defaultHelpersTimeout = defaultTimeout;
            ctsLoading = new System.Threading.CancellationTokenSource();
            TryCreateDataSource(bExecutingCommand);
        }

        void TryCreateDataSource(bool bExecutingCommand)
        {
            string fillError = null;
            try
            {
                dataSet = RecipeExecuter.CreateDataSet(ctsLoading.Token, true, out fillError, true);
                bDataSetLoaded = true;
            }
            catch (Exception)
            {
                if (!bExecutingCommand)
                    throw;
            }
            if (!bExecutingCommand)
                LastFillError = fillError;
        }

        public void Refill()
        {
            LastFillError = null;
            if (!bDataSetLoaded)
            {
                TryCreateDataSource(false);
                return;
            }
            dataSet.Clear();
            LastFillError = recipeExecuter.FillDataSet(dataSet, ctsLoading.Token, true);
        }       

        public RecipeItemsData GetRecipeItems(string selectedSubrecipeName, CancellationToken ct)
        {
            var jobject = PlatformComponents.GetDataSetJObject(dataSet);
            var tableName = DataSetHelper.TableName(RecipeDocument.RecipeEntity);
            var columnName = DataSetHelper.ColumnName(recipeDocument.RecipeEntity);
            var idKey = DataSetHelper.PrimaryKeyName(RecipeDocument.RecipeEntity);
            String recipeId = null;
            if (!String.IsNullOrEmpty(selectedSubrecipeName) || jobject[tableName].Count() > 0)
            {
                recipeId = String.IsNullOrEmpty(selectedSubrecipeName) ? jobject[tableName][0][idKey].ToString() :
                    (from recipe in jobject[tableName] where (string)(recipe[columnName]) == selectedSubrecipeName
                     select recipe[idKey]).FirstOrDefault()?.ToString();
            }

            var dataValues = RecipeDocument.RecipeEntity.GetFlatDataValuesCollection();
            var ret = new RecipeItemsData() { items = new List<RecipeItem>() };
            foreach (var dataValue in dataValues)
            {
                ct.ThrowIfCancellationRequested();
                var recipeItem = new RecipeItem()
                {
                    OID = dataValue.OID,
                    DecimalDigits = dataValue.DecimalDigits,
                    DataType = dataValue.DataType,
                    GroupName = dataValue.UFGroupAss != null ? dataValue.UFGroupAss.Name : String.Empty,
                    Name = dataValue.Name,
                    Value = dataValue.DefaultValue,
                    Description = dataValue.Description,
                };
                ret.items.Add(recipeItem);

                var recipeValue = TypeExtensions.ChangeType(dataValue.DefaultValue, dataValue.DataType.ToNetType(), force: true, dataValue.ArrayDimension);
                if (recipeId != null)
                {
                    tableName = DataSetHelper.TableName(dataValue);
                    columnName = DataSetHelper.ColumnName(dataValue);
                    var columnValues = (from recipe in jobject[tableName] where (string)(recipe[idKey]) == recipeId select recipe[columnName]).ToList();
                    if (columnValues.Count > 0 && columnValues[0] != null)
                        recipeValue = columnValues[0];
                }

                var stringValue = recipeValue.ToString();
                if (dataValue.ArrayDimension == 0)
                {
                    try
                    {
                        if (dataValue.DataType == UFUAModel.DataType.Float || dataValue.DataType == UFUAModel.DataType.Double)
                            stringValue = DecimalNumberToInvariant(dataValue, stringValue, ((JValue)recipeValue).Value);
                        if (recipeItem.IsScalar && dataValue.EditControlType == UFRecipeSettings.UFRecipeModel.EditValueControlTypeEnum.EditDisplay && !String.IsNullOrEmpty(dataValue.UnitName))
                            stringValue = String.Format("{0} {1}", stringValue, dataValue.UnitName);
                    }
                    catch { }
                }
                recipeItem.Value = stringValue;
            }

            return ret;
        }

        string DecimalNumberToInvariant(UFRecipeSettings.UFRecipeModel.UFDataValueEntity dataValue, string stringValue, object recipeValue)
        {
            var stringFormat = "0.";
            for (var i = 0; i < dataValue.DecimalDigits; i++)
                stringFormat += "#";
            if (dataValue.DataType == UFUAModel.DataType.Float)
            {
                float dvValue = 0;
                try
                {
                    //SQL Server "float" type is mapped to .NET "Double" type
                    dvValue = Convert.ToSingle(recipeValue, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch { }
                stringValue = dvValue.ToString(stringFormat, System.Globalization.CultureInfo.InvariantCulture);
            }
            if (dataValue.DataType == UFUAModel.DataType.Double)
            {
                double dvValue = 0;
                try
                {
                    dvValue = (double)recipeValue;
                }
                catch { }
                stringValue = dvValue.ToString(stringFormat, System.Globalization.CultureInfo.InvariantCulture);
            }
            return stringValue;
        }

        public Dictionary<string, List<JToken>> GetRecipeNames()
        {
            var jobject = PlatformComponents.GetDataSetJObject(dataSet);
            var recipeName = DataSetHelper.ColumnName(RecipeDocument.RecipeEntity);
            var tableName = DataSetHelper.TableName(RecipeDocument.RecipeEntity);
            var idKey = DataSetHelper.PrimaryKeyName(RecipeDocument.RecipeEntity);
            var recipeNamesDict = (from recipe in jobject[tableName] select recipe).ToDictionary(rec => rec[idKey], rec => rec[recipeName]).OrderBy(d => d.Value.ToString());

            return new Dictionary<string, List<JToken>>()
            {
                { "RecipeNames", recipeNamesDict.Select(kp => kp.Value).ToList() },
                { "RecipeIDS", recipeNamesDict.Select(kp => kp.Key).ToList() }
            };
        }

        class DataValueData
        {
            public string dataValueName;
            public object dataValueValue;
            public DataValueData() { }
        }

        public void RecipeDBSave(object pendingChanges, string stringRecipeGuid, string newRecipePrefix, bool bDeleting)
        {
            Guid recipeGuid;
            var finalDataSet = DataSet_MergePendingChanges(recipePath, pendingChanges, stringRecipeGuid, newRecipePrefix, out recipeGuid, bDeleting);
            RecipeExecuter.AcceptUpdateData(finalDataSet, recipeGuid);
            RecipeExecuter.ForceUpdateRecipeTags();
        }

        public void RecipeDeviceSave(object pendingChanges, string stringRecipeGuid, string newRecipePrefix)
        {
            Guid guid;
            var dataSet = DataSet_MergePendingChanges(recipePath, pendingChanges, stringRecipeGuid, newRecipePrefix, out guid);
            if (dataSet.Tables[DataSetHelper.TableName(RecipeDocument.RecipeEntity)].DefaultView.Count == 0)
                return;
            
            recipeExecuter.GetOutDataServerValues(dataSet, guid, defaultHelpersTimeout, ctsLoading.Token);
        }

        public void RecipeCommandExecute(Uri recipePath, int commandType, int syncTimeout, bool bSynchronous)
        {
            var parentDoc = PlatformComponents.GetProjectDocument();
            var rootParent = DocumentManager.ComponentService.Helpers.DocumentHelper.GetRootParent(parentDoc, traverse: false);
            var uri = rootParent.MakeAbosoluteUri(recipePath);
            parentDoc = parentDoc.UpdateParentFromUri(uri);

            var context = new UFRecipeExecutionContext.RecipeExecutionContext()
            {
                CommandType = (UFRecipeExecutionContext.RecipeCommandType)commandType,
                Timeout = syncTimeout,
                Index = RecipeExecuter.GetRecipeIndex(),
                IsSynchro = bSynchronous
            };

            if (!bSynchronous)
                recipeExecuter.CheckAccessXmlDataSet();

            RecipeExecuter.Execute(parentDoc, DocumentManager.ComponentService.ExecutionMode.Normal, context);
            if (context.Exception != null)
                throw context.Exception;
        }

        public RecipeItemsData RecipeDeviceLoad(string selectedSubrecipeName, string recipeGuid, CancellationToken token)
        {
            if (dataSet.Tables[DataSetHelper.TableName(RecipeDocument.RecipeEntity)].DefaultView.Count == 0)
                throw new System.ComponentModel.WarningException("ReadCommandExecutionResultWarning");
           
            var guid = Guid.Empty;
            if (Guid.TryParse(recipeGuid, out guid))
            {
                recipeExecuter.GetInDataServerValues(dataSet, guid, defaultHelpersTimeout, token);
                return GetRecipeItems(selectedSubrecipeName, token);
            }
            else
            {
                throw new System.ComponentModel.WarningException("ReadCommandExecutionResultWarning");
            }
        }

        DataSet DataSet_MergePendingChanges(string recipePath, object pendingChanges, string stringRecipeGuid, string newRecipePrefix, out Guid recipeGuid, bool bDeleting = false)
        {
            //merging original dataset with client pending changes
            var originalDataset = dataSet;
            var recipeName = System.IO.Path.GetFileNameWithoutExtension(recipePath);
            var tableName = DataSetHelper.TableName(RecipeDocument.RecipeEntity);
            try
            {
                recipeGuid = Guid.Parse(stringRecipeGuid);
            }
            catch
            {
                recipeGuid = Guid.NewGuid();
            }
            var newRecipeGuid = recipeGuid.ToString();
            if (bDeleting)
            {
                List<DataRow> mainTableDeletingRows = new List<DataRow>();
                foreach (var row in originalDataset.Tables.Cast<DataTable>().SelectMany(tb => tb.Rows.Cast<DataRow>()).Where(row => row.RowState != DataRowState.Deleted && row.ItemArray[0].ToString() == stringRecipeGuid).ToList())
                    row.Delete();
            }
            else
            {
                var bSavingNewRecipe = originalDataset.Tables.Cast<DataTable>().SelectMany(tb => tb.Rows.Cast<DataRow>()).Where(row => row.ItemArray[0].ToString() == stringRecipeGuid).ToList().Count == 0;
                if (bSavingNewRecipe)
                {
                    var clonedRecipeGuid = stringRecipeGuid.Substring(newRecipePrefix.Length, recipeGuidLength);
                    if (clonedRecipeGuid != Guid.Empty.ToString()) //populating missing newRecipeDataValueValues records with the other recipe's data: selecting all groups and datavalues of the starting recipe
                    {
                        var recipeTable = (from DataTable t in originalDataset.Tables where t.TableName == tableName select t).FirstOrDefault();
                        if (recipeTable != null)
                        {
                            var groupRowsToClone = originalDataset.Tables.Cast<DataTable>().SelectMany(tb => tb.Rows.Cast<DataRow>()).Where(row => row.ItemArray[0].ToString() == clonedRecipeGuid).ToList();
                            foreach (var sourceRow in groupRowsToClone)
                            {
                                var sourceTable = sourceRow.Table;
                                var clonedRow = (from DataRow row in sourceTable.Rows where row[sourceTable.PrimaryKey.First().ColumnName].ToString() == newRecipeGuid select row).FirstOrDefault();
                                if (clonedRow == null)
                                {
                                    clonedRow = sourceTable.NewRow();
                                    clonedRow[sourceTable.PrimaryKey.First().ColumnName] = newRecipeGuid;
                                    if (sourceTable == recipeTable)
                                    {
                                        clonedRow[recipeName] = stringRecipeGuid.Substring(newRecipePrefix.Length + recipeGuidLength);
                                        var creationDateTimeColName = DataSetHelper.CreationDateTimeColumnName(RecipeDocument.RecipeEntity);
                                        clonedRow[creationDateTimeColName] = DateTime.UtcNow;
                                    }
                                    sourceTable.Rows.Add(clonedRow);
                                }
                                if (sourceTable == recipeTable)
                                {
                                    var dataValueColumns = (from DataColumn col in sourceTable.Columns where col.ColumnName != tableName && col.ColumnName != sourceTable.PrimaryKey.First().ColumnName && col.ColumnName != DataSetHelper.CreationDateTimeColumnName(RecipeDocument.RecipeEntity) && col.ColumnName != DataSetHelper.ActivationDateTimeColumnName(RecipeDocument.RecipeEntity) select col).ToList();
                                    foreach (var col in dataValueColumns)
                                        clonedRow[col.ColumnName] = sourceRow[col.ColumnName];
                                }
                                else
                                {
                                    var dataValueColumns = (from DataColumn col in sourceTable.Columns where col.ColumnName != sourceTable.PrimaryKey.First().ColumnName select col).ToList();
                                    foreach (var col in dataValueColumns)
                                        clonedRow[col.ColumnName] = sourceRow[col.ColumnName];
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (DataTable table in originalDataset.Tables)
                        {
                            var newRow = table.NewRow();
                            newRow[table.PrimaryKey.First().ColumnName] = newRecipeGuid;
                            if (table.TableName == tableName)
                            {
                                newRow[recipeName] = stringRecipeGuid.Substring(newRecipePrefix.Length + recipeGuidLength);
                                var creationDateTimeColName = DataSetHelper.CreationDateTimeColumnName(RecipeDocument.RecipeEntity);
                                newRow[creationDateTimeColName] = DateTime.UtcNow;
                            }
                            table.Rows.Add(newRow);
                        }
                    }
                }

                var jobject = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(pendingChanges));
                foreach (JProperty x in (JToken)jobject)
                {
                    string dataValueGroup = String.IsNullOrEmpty(x.Name) ? tableName : String.Format("{0}_{1}", tableName, x.Name);
                    foreach (JProperty y in x.Value)
                    {
                        string recipeID = y.Name;
                        if (recipeID.ToLower() != stringRecipeGuid?.ToLower())
                            continue;
                        if (bSavingNewRecipe)
                            recipeID = newRecipeGuid;
                        bool bAddingNewRecipe = false;
                        Dictionary<int, List<DataValueData>> newRecipeDataValueValues = new Dictionary<int, List<DataValueData>>();
                        foreach (JProperty z in y.Value)
                        {
                            var dataValues = String.IsNullOrEmpty(x.Name) ? RecipeDocument.RecipeEntity.DataValues : (from g in RecipeDocument.RecipeEntity.Groups where g.Name == x.Name select g).First().DataValues;
                            string dataValueName = z.Name;
                            object dataValueValue = null;
                            var dataValue = (from dv in dataValues where dv.Name == z.Name && (dv.UFGroupAss == null && dataValueGroup == tableName || dv.UFGroupAss?.Name == x.Name) select dv).FirstOrDefault();
                            var dataType = dataValue?.DataType;
                            var bIsArray = dataValue?.ArrayDimension > 0;
                            if (dataType == UFUAModel.DataType.Boolean)
                            {
                                if (Boolean.TryParse((string)z.Value, out bool b))
                                    dataValueValue = b;
                                else
                                {
                                    try
                                    {
                                        dataValueValue = Convert.ToBoolean(Convert.ToInt32((string)z.Value));
                                    }
                                    catch { }
                                }
                            }
                            dataValueValue = dataValueValue ?? (string)z.Value;
                            var query =
                            (
                                from DataTable table in originalDataset.Tables
                                where table.TableName == dataValueGroup
                                select new Tuple<int, int>(originalDataset.Tables.IndexOf(table),
                                (from DataColumn column in table.Columns where column.ColumnName == dataValueName select table.Columns.IndexOf(column)).FirstOrDefault())).FirstOrDefault();
                            if (query != null)
                            {
                                int tableIndex = query.Item1; //Tables <=> Datavalue groups
                                int rowSubIndex = query.Item2; //DataValue index inside DataRow
                                var rowIndexQuery = (from DataRow row in originalDataset.Tables[tableIndex].Rows where row.ItemArray[0].ToString() == recipeID select originalDataset.Tables[tableIndex].Rows.IndexOf(row));
                                var table = originalDataset.Tables[tableIndex];
                                
                                if (bIsArray && dataValue != null)
                                {
                                    var stringArray = dataValueValue.ToString().Trim();
                                    stringArray = stringArray.Substring(1, stringArray.Length - 2);
                                    var newDvValue = "{";
                                    foreach (var item in stringArray.Split('|'))
                                    {
                                        object curValue;
                                        if (dataType == UFUAModel.DataType.Boolean)
                                        {
                                            curValue = TypeExtensions.ChangeType(item, dataType.ToNetType());
                                            if (curValue == null)
                                            {
                                                try
                                                {
                                                    curValue = Convert.ToBoolean(Convert.ToInt16(item));
                                                }
                                                catch
                                                {
                                                    curValue = Boolean.Parse(Boolean.FalseString);
                                                }
                                            }
                                        }
                                        else
                                            curValue = TypeExtensions.ChangeType(item, dataType.ToNetType(), force: true);
                                        var stringValue = curValue.ToString();
                                        if (dataType == UFUAModel.DataType.Float || dataType == UFUAModel.DataType.Double)
                                            stringValue = DecimalNumberToInvariant(dataValue, stringValue, curValue);
                                        newDvValue = String.Format("{0}{1} |", newDvValue, stringValue);
                                    }
                                    dataValueValue = String.Format("{0}}}", newDvValue.Substring(0, newDvValue.Length - 1).Trim());
                                }
                                else if (dataValueValue != null && (dataType == UFUAModel.DataType.Float || dataType == UFUAModel.DataType.Double))
                                {
                                    if (dataType == UFUAModel.DataType.Float)
                                        dataValueValue = float.Parse(dataValueValue.ToString(), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                                    else
                                        dataValueValue = double.Parse(dataValueValue.ToString(), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                                }
                                if (rowIndexQuery.Count() > 0)
                                {
                                    if (dataValueValue != null)
                                        table.Rows[rowIndexQuery.First()][rowSubIndex] = dataValueValue;
                                    else
                                        table.Rows[rowIndexQuery.First()][rowSubIndex] = DBNull.Value;
                                }
                                else
                                {
                                    bAddingNewRecipe = true;
                                    if (!newRecipeDataValueValues.ContainsKey(tableIndex))
                                        newRecipeDataValueValues.Add(tableIndex, new List<DataValueData>());
                                    newRecipeDataValueValues[tableIndex].Add(new DataValueData() { dataValueName = dataValueName, dataValueValue = dataValueValue });
                                }
                            }
                        }
                        if (bAddingNewRecipe)
                        {
                            var recipeTable = (from DataTable t in originalDataset.Tables where t.TableName == tableName select t).FirstOrDefault();
                            if (recipeTable != null)
                            {
                                DataRow mainRow = (from DataRow row in recipeTable.Rows where row.ItemArray[0].ToString() == newRecipeGuid select row).FirstOrDefault();
                                if (mainRow == null)
                                {
                                    mainRow = recipeTable.NewRow();
                                    mainRow[recipeTable.PrimaryKey.First().ColumnName] = newRecipeGuid;
                                    mainRow[recipeName] = stringRecipeGuid.Substring(newRecipePrefix.Length + recipeGuidLength);
                                    var creationDateTimeColName = DataSetHelper.CreationDateTimeColumnName(RecipeDocument.RecipeEntity);
                                    mainRow[creationDateTimeColName] = DateTime.UtcNow;
                                    recipeTable.Rows.Add(mainRow);
                                }
                                foreach (var tableIndex in newRecipeDataValueValues.Keys)
                                {
                                    var table = originalDataset.Tables[tableIndex];
                                    DataRow newRow = mainRow;
                                    if (tableIndex > 0)
                                        newRow = table.NewRow();
                                    //var idColName = String.Format("{0}_ID", System.IO.Path.GetFileNameWithoutExtension(recipePath));
                                    newRow[table.PrimaryKey.First().ColumnName] = newRecipeGuid;
                                    foreach (var dvData in newRecipeDataValueValues[tableIndex])
                                    {
                                        if (dvData.dataValueValue != null)
                                            newRow[dvData.dataValueName] = dvData.dataValueValue;
                                        else
                                            newRow[dvData.dataValueName] = DBNull.Value;
                                    }
                                    if (newRow != mainRow)
                                        table.Rows.Add(newRow);
                                }
                            }
                        }
                    }
                }
            }
            return originalDataset;
        }

        public void Dispose()
        {
            RecipeExecuter?.Dispose();
            RecipeDocument?.Dispose();
            ctsLoading?.Dispose();
        }
    }

    public class RecipeItemsData
    {
        public RecipeItemsData()
        {
        }

        public string recipeName;
        public List<RecipeItem> items;
    }

    public class RecipeItem
    {
        public RecipeItem(){ }

        public int OID;
        public int DecimalDigits;
        public string GroupName;
        public string Name;
        public string Value;
        public string Description;
        UFUAModel.DataType dataType;
        public UFUAModel.DataType DataType
        {
            get
            {
                return dataType;
            }
            set
            {
                dataType = value;
                IsString = dataType == UFUAModel.DataType.String;
                IsUnsigned = dataType == UFUAModel.DataType.Boolean || dataType == UFUAModel.DataType.Byte || dataType == UFUAModel.DataType.UInt16 || dataType == UFUAModel.DataType.UInt32 || dataType == UFUAModel.DataType.UInt64;
                IsDecimal = dataType == UFUAModel.DataType.Float || dataType == UFUAModel.DataType.Double;
                IsScalar = dataType != UFUAModel.DataType.Boolean && dataType != UFUAModel.DataType.String;
                IsBoolean = dataType == UFUAModel.DataType.Boolean;
            }
        }

        public bool IsString;
        public bool IsUnsigned;
        public bool IsDecimal;
        public bool IsScalar;
        public bool IsBoolean;

        public bool AllowNull;
        public string DefaultValue;
        public RecipeItemControlType EditControlType;
        public string UnitName;
        public double MinValue;
        public double MaxValue;
    }

    public enum RecipeItemControlType
    {
        EditDisplay,
        CheckBox,
        RadioButton,
        ComboBox,
        ListView
    }
}
