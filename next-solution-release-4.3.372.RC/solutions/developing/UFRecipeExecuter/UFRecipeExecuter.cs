using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using DocumentManager.ComponentService;
using System.Windows;
#if !NET_STANDARD
using UIMsgBoxAlertService.ComponentService;
using UFRecipeLayout;
#endif
using System.Diagnostics;
using System.Threading;
using UFUAEditor.ComponentService;
using UFRecipeSettings.UFRecipeModel;
using UFRecipeExecutionContext;
using UFRecipeSettings.Helpers;
using UFRecipeSettings.Documents;
using UFUAModel.Extensions;
using StringManager.ComponentService;
using OPCUAViewModel;
using Opc.Ua;
using DataReader.Helpers;
using DataReader.Extensions;
using log4net;
using Utilities.Logger;
using UFRecipeExecuter.OPCUA;
using DevExpress.Xpo;
using UFUAServerBase;
using System.Windows.Input;

namespace UFRecipeExecuter
{
    public class UFRecipeExecuter : IDisposable
    {
#region Declarations

        string dataProvider;
        string connectionString;
        string clientSessionName;
        internal string currentSessionName;

        DataSet loadDataSet;
        DataSet writeDataSet;
        DataSet readDataSet;

        RecipeEntityHelper recipeReader;
        RecipeEntityHelper recipeWriter;

        readonly RecipeUAConnector recipeUAConnector;

        readonly List<SubscribeTagReference> subscribedRecipeTags = new List<SubscribeTagReference>();
        readonly Dictionary<RecipeExecutionContext, SubscribeDataValues> subscribedDataValues = new Dictionary<RecipeExecutionContext, SubscribeDataValues>();

        bool bExecuted;
        bool bSuspended = true;
        Timer checkSubscribedRecipeTags;

        Task taskPendingActions;
        readonly OrderedDictionary pendingActions = new OrderedDictionary();
        readonly CancellationTokenSource ctsPendingTasks = new CancellationTokenSource();
        int pendingCommands;
        RecipeExecutionContext pendingRecipeTagExecution;

        Task taskVerifyDatataBase;

        static readonly ILog logServer = Logger.GetDestinationLog(LoggerDestination.RecipeService);
        readonly static Dictionary<String, RecipeCheckingStateEnum> recipeCheckingState = new Dictionary<String, RecipeCheckingStateEnum>();
        List<Guid> InitializedDataValues = new List<Guid>();
#if !NET_STANDARD
        private UFRecipeTransactionLogger transactionLogger;
        string TransactionLogConnectionString;
        TimeSpan TransactionLogMaxAge;
        bool IsRedundancyEnabled;
#endif
#endregion

#region Constructors
#if !NET_STANDARD
        public UFRecipeExecuter(UFRecipeDocument doc, ConnectorType connectorType, string clientSessionName = null, TransactionLog.RecipeRedundancyInitializationContext context = null)
#else
        public UFRecipeExecuter(UFRecipeDocument doc, ConnectorType connectorType, string clientSessionName = null)
#endif
        {
            RecipeDocument = doc;
            if (connectorType != ConnectorType.None)
                recipeUAConnector = new RecipeUAConnector(doc, connectorType);
            this.clientSessionName = clientSessionName;
            currentSessionName = String.IsNullOrEmpty(clientSessionName) ? doc.SessionString : clientSessionName;
            CheckConnectionSource();
#if !NET_STANDARD
            if(context != null)
            {
                TransactionLogConnectionString = context.TransactionLogConnectionString;
                TransactionLogMaxAge = context.TransactionLogMaxAge;
                IsRedundancyEnabled = context.IsRedudancyEnabled;
            }
#endif
        }
#endregion

#region Public Events
        public event EventHandler<StateChangedArgs> StateChanged;

        void OnStateChanged(RecipeExecutionStateEnum oldState, RecipeExecutionStateEnum newState)
        {
            var e = StateChanged;
            if (e != null)
                e(this, new StateChangedArgs()
                {
                    recipeName = RecipeDocument.Title,
                    oldState = oldState,
                    newState = newState
                });
        }

        public event EventHandler<AuditTraceArgs> AuditTraceMessage;

        void NotifyAuditTraceMessage(String recipeIndex, String details, String userName, String userComment, AuditLogEntryType logEntryType)
        {
            var e = AuditTraceMessage;
            if (e != null)
            {
                e(this, new AuditTraceArgs()
                {
                    RecipeIndex = recipeIndex,
                    Details = details,
                    UserName = userName,
                    UserComment = userComment,
                    LogEntryType = logEntryType
                });
            }
        }
#endregion

#region Public Methods

        public static void ClearCheckedRecipes()
        {
            lock (recipeCheckingState)
            {
                recipeCheckingState.Clear();
            }
        }

        public static void ClearCheckedRecipes(UFRecipeDocument doc)
        {
            lock (recipeCheckingState)
            {
                if (recipeCheckingState.ContainsKey(doc.FullPath))
                    recipeCheckingState.Remove(doc.FullPath);
            }
        }

        public static void ChangeRowsState(DataSet dataSet, DataRowState newRowState)
        {
            dataSet.AcceptChanges();
            if (newRowState != DataRowState.Unchanged)
            {
                foreach (DataTable dataTable in dataSet.Tables)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        if (newRowState == DataRowState.Deleted)
                            row.Delete();
                        else if (newRowState == DataRowState.Added)
                            row.SetAdded();
                        else if (newRowState == DataRowState.Modified)
                            row.SetModified();
                    }
                }
            }
        }

        public void Initialize()
        {
            PrepareExecution(String.IsNullOrEmpty(clientSessionName) ? RecipeDocument.SessionString : clientSessionName);
#if !NET_STANDARD
            if (TransactionLogConnectionString != null && TransactionLogConnectionString != String.Empty && 
                transactionLogger == null && IsRedundancyEnabled)
            {
                transactionLogger = new UFRecipeTransactionLogger(TransactionLogConnectionString, TransactionLogMaxAge);
            }
#endif
        }

        public void Resume()
        {
            if (!bSuspended || recipeUAConnector != null)
                return;
            bSuspended = false;

            SubscribeRecipeTags();
        }

        public void Suspend()
        {
            if (bSuspended || recipeUAConnector != null)
                return;
            bSuspended = true;

            UnsubscribeRecipeTags();
            UnsubscribeDataValues();
        }

        public void Execute(IDocument parent, ExecutionMode mode, RecipeExecutionContext executionContext)
        {
            if (executionContext == null)
                return;

            try
            {
                if ((executionContext.IsSynchro || executionContext.IsAuditTrace) && executionContext.IsExecuted == null)
                    executionContext.IsExecuted = new ManualResetEvent(false);

                if (recipeUAConnector != null)
                {
#if !NET_STANDARD
                    using (var cursor = new WaitCursor())
#endif
                    {
                        switch (executionContext.CommandType)
                        {
                            case RecipeCommandType.Show:
#if !NET_STANDARD
                                ShowRecipe(parent, cursor);
#endif
                                if (executionContext.IsExecuted != null)
                                    executionContext.IsExecuted.Set();
                                break;
                            default:
                                if (!executionContext.IsAuditTrace || mode == ExecutionMode.Synchro)
                                    PrepareExecute(executionContext);
                                else
                                    InternalExecute(executionContext);
                                break;
                        }

                        if (executionContext.IsExecuted != null)
                            executionContext.IsExecuted.WaitOne();
                    }
                }
                else
                {
                    switch (executionContext.CommandType)
                    {
                        case RecipeCommandType.Activate:
                            WriteRecipe(executionContext);
                            break;
                        case RecipeCommandType.Load:
                            LoadRecipe(executionContext);
                            break;
                        case RecipeCommandType.Save:
                            SaveRecipe(executionContext);
                            break;
                        case RecipeCommandType.Remove:
                            DeleteRecipe(executionContext);
                            break;
                        case RecipeCommandType.Read:
                            ReadRecipe(executionContext);
                            break;
                        case RecipeCommandType.Export:
                            ExportRecipe(executionContext);
                            break;
                        case RecipeCommandType.Import:
                            ImportRecipe(executionContext);
                            break;
                        default:
                            if (executionContext.IsExecuted != null)
                                executionContext.IsExecuted.Set();
                            break;
                    }

                    if (executionContext.IsExecuted != null)
                        executionContext.IsExecuted.WaitOne();
                }
            }
            finally
            {
                if (executionContext.IsExecuted != null)
                    executionContext.IsExecuted.Dispose();
            }
        }

        public DataSet CreateDataSet(bool fill = false)
        {
            String fillError = null;
            return CreateDataSet(ctsPendingTasks.Token, fill, out fillError);
        }

        public DataSet CreateDataSet(CancellationToken token)
        {
            String fillError = null;
            return CreateDataSet(token, fill: false, out fillError);
        }

        public DataSet CreateDataSet(CancellationToken token, ref Dictionary<string, List<Tuple<string, string>>> mapWrongValues)
        {
            String fillError = null;
            var ret = CreateDataSet(token, fill: false, out fillError);
            mapWrongValues = this.mapWrongValues;
            return ret;
        }

        public DataSet CreateDataSet(CancellationToken token, bool fill)
        {
            String fillError = null;
            return CreateDataSet(token, fill, out fillError);
        }

        public DataSet CreateDataSet(bool fill, out String fillError, bool bContinueOnError = false)
        {
            fillError = null;
            return CreateDataSet(ctsPendingTasks.Token, fill, out fillError, bContinueOnError);
        }

        public DataSet CreateDataSet(bool fill, out String fillError, ref Dictionary<string, List<Tuple<string, string>>> mapWrongValues, bool bContinueOnError = false)
        {
            fillError = null;
            var ret = CreateDataSet(ctsPendingTasks.Token, fill, out fillError, bContinueOnError);
            mapWrongValues = this.mapWrongValues;
            return ret;
        }

        public DataSet CreateDataSet(CancellationToken token, bool fill, out String fillError, ref Dictionary<string, List<Tuple<string, string>>> mapWrongValues, bool bContinueOnError = false)
        {
            var ret = CreateDataSet(token, fill, out fillError, bContinueOnError);
            mapWrongValues = this.mapWrongValues;
            return ret;
        }

        public DataSet CreateDataSet(CancellationToken token, bool fill, out String fillError, bool bContinueOnError = false)
        {
            fillError = null;
            var ds = new DataSet(RecipeDocument.RecipeEntity.RecipeName);
            var table = CreateDataTable(RecipeDocument.RecipeEntity);
            ds.Tables.Add(table);
            foreach (var group in RecipeDocument.RecipeEntity.Groups)
            {
                token.ThrowIfCancellationRequested();

                var childtable = CreateDataTable(group);
                ds.Tables.Add(childtable);
                childtable.ParentRelations.Add(new DataRelation(childtable.TableName, table.PrimaryKey[0], childtable.PrimaryKey[0]));
            }

            if (fill)
                fillError = FillDataSet(ds, token, ref mapWrongValues, bContinueOnError);

            return ds;
        }

        public VariantCollection CheckAccessXmlDataSet(bool bContinueOnError = false)
        {
            return recipeUAConnector.ReadXmlDataSet(DataRowState.Unchanged, bContinueOnError);
        }

        private Dictionary<string, List<Tuple<string, string>>> mapWrongValues;
        public String FillDataSet(DataSet ds, CancellationToken token, ref Dictionary<string, List<Tuple<string, string>>> mapWrongValues,
            bool bContinueOnError = false)
        {
            mapWrongValues = new Dictionary<string, List<Tuple<string, string>>>();
            var ret = FillDataSet(ds, token, bContinueOnError);
            mapWrongValues = this.mapWrongValues;
            return ret;
        }


        public String FillDataSet(DataSet ds, CancellationToken token, bool bContinueOnError = false)
        {
            if (recipeUAConnector != null)
            {
                var maxDateTime = DateTime.UtcNow.AddMilliseconds(Properties.Settings.Default.defaultSyncTimeout);
                while (!recipeUAConnector.CanReadXmlDataSet())
                {
                    token.ThrowIfCancellationRequested();
                    if (DateTime.UtcNow > maxDateTime)
                        throw new TimeoutException(Properties.Resources.TimeoutReadingData);
                    Thread.Sleep(10);
                }

                var result = CheckAccessXmlDataSet(bContinueOnError);
                
                String xmlDataSet = null;
                if (result != null && result.Count > 0)
                    xmlDataSet = result[0].ToString();

                if (xmlDataSet != null)
                {
                    var xmlBuilder = new System.Text.StringBuilder();
                    System.Xml.XmlReaderSettings settings = new System.Xml.XmlReaderSettings()
                    {
                        ConformanceLevel = System.Xml.ConformanceLevel.Document,
                        CloseInput = true,
                        CheckCharacters = false
                    };

                    using (var stringReader = new System.IO.StringReader(xmlDataSet))
                    {
                        using (var xmlReader = System.Xml.XmlReader.Create(stringReader, settings))
                        {
                            foreach (DataTable dataTable in ds.Tables)
                                dataTable.BeginLoadData();

                            // **********************************************************
                            // Moving two node away is needed for reading all records
                            xmlReader.MoveToContent();
                            xmlReader.Read();
                            // **********************************************************
                            ds.ReadXml(xmlReader, XmlReadMode.IgnoreSchema);
                            ChangeRowsState(ds, DataRowState.Unchanged);

                            foreach (DataTable dataTable in ds.Tables)
                                dataTable.EndLoadData();
                        }
                    }
                }

                String fillError = null;
                if (result != null && result.Count > 1)
                {
                    fillError = result[1].ToString();
                    String xmlDictionary = null;
                    if (result != null && result.Count > 1)
                    {
                        xmlDictionary = result[2].ToString();
                        mapWrongValues = xmlDictionary.FromXml<Dictionary<string, List<Tuple<string, string>>>>();
                    }
                }
                return fillError;
            }
            else
            {
                String fillError = null;
                if (!String.IsNullOrEmpty(dataProvider) && !String.IsNullOrEmpty(connectionString))
                {
                    try
                    {
                        using (var dbSchemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(dataProvider, connectionString))
                        {
                            using (var connection = DataReader.DataReader.CreateDbConnection(dataProvider, connectionString))
                            {
                                connection.Open();
                                using (var dbdapater = DataReader.DataReader.CreateDbDataAdapter(dataProvider))
                                {
                                    using (var cmdBuilder = DataReader.DataReader.CreateDbCommandBuilder(dataProvider))
                                    {
                                        // preparing select command
                                        dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(dataProvider);
                                        dbdapater.SelectCommand.Connection = connection;
                                        if (bContinueOnError)
                                        {
                                            dbdapater.FillError += (s, e) =>
                                            {
                                                e.Continue = true;
                                                if (String.IsNullOrEmpty(fillError))
                                                    fillError = e.Errors.InnerException == null ? e.Errors.Message : e.Errors.InnerException.Message;

                                                var row = e.DataTable.NewRow();
	                                            row.BeginEdit();
	                                            for (int ii = 0; ii < row.Table.Columns.Count; ii++)
	                                            {
	                                                try
	                                                {
	                                                    row[ii] = e.Values[ii];
	                                                }
	                                                catch (Exception ex)
	                                                {
                                                        //Save in the map the recipe ID and the errors related to the columns
                                                        if(mapWrongValues == null)
                                                            mapWrongValues = new Dictionary<string, List<Tuple<string, string>>>();

                                                        var t = Tuple.Create(row.Table.Columns[ii].ColumnName, ex.Message);
                                                        if (!mapWrongValues.Keys.Contains(row[0].ToString()))
                                                            mapWrongValues.Add(row[0].ToString(), new List<Tuple<string, string>> { t });
                                                        if (!mapWrongValues[row[0].ToString()].Contains(t))
                                                            mapWrongValues[row[0].ToString()].Add(t);
                                                    }
	                                            }
	                                            row.EndEdit();
	
	                                            e.DataTable.Rows.Add(row);
	                                            e.DataTable.AcceptChanges();
                                            };
                                        }

                                        // filling tables in dataset
                                        for (int cc = 0; cc < ds.Tables.Count; cc++)
                                        {
                                            token.ThrowIfCancellationRequested();

                                            dbdapater.SelectCommand.CommandText = String.Format("SELECT * FROM {0}", dbSchemaInfo.WrapObjectName(ds.Tables[cc].TableName));
                                            int nRows = dbdapater.Fill(ds.Tables[cc]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        lock (recipeCheckingState)
                        {
                            recipeCheckingState[RecipeDocument.FullPath] = RecipeCheckingStateEnum.Error;
                        }

                        throw;
                    }
                }

                return fillError;
            }
        }

        public bool IsReady(RecipeCommandType commandType)
        {
            if (recipeUAConnector != null)
            {
                return recipeUAConnector.CanExecute(commandType);
            }
            else
            {
                if (CheckState == RecipeCheckingStateEnum.Error)
                {
                    CheckAndVerifyDatabaseAsync();
                }

                return CheckState == RecipeCheckingStateEnum.Checked;
            }
        }

        public bool CanReadXmlDataSet()
        {
            try
            {
                if (recipeUAConnector != null)
                    return recipeUAConnector.CanReadXmlDataSet();
            }
            catch
            { }

            return false;
        }

        public bool CanWriteXmlDataSet()
        {
            try
            {
                if (recipeUAConnector != null)
                    return recipeUAConnector.CanWriteXmlDataSet();
            }
            catch
            { }

            return false;
        }

        public bool CanGetInDataServerValues()
        {
            try
            {
                if (recipeUAConnector != null)
                    return recipeUAConnector.CanGetInDataServerValues();
                else if (recipeReader != null)
                    return recipeReader.CanExecute(0);
            }
            catch
            { }

            return false;
        }

        public bool CanGetOutDataServerValues()
        {
            try
            {
                if (recipeUAConnector != null)
                    return recipeUAConnector.CanGetOutDataServerValues();
                else if (recipeWriter != null)
                    return recipeWriter.CanExecute(0);
            }
            catch
            { }

            return false;
        }
        public void AcceptUpdateData(DataSet dataSet, bool avoidAuditTrace = false)
        {
            AcceptUpdateData(dataSet, Guid.Empty, null, null, avoidAuditTrace: avoidAuditTrace);
        }

        public void AcceptUpdateData(DataSet dataSet, bool updateTransactionLog, bool avoidAuditTrace = false)
        {
            AcceptUpdateData(dataSet, Guid.Empty, null, null, avoidAuditTrace: avoidAuditTrace, updateTransactionLog: updateTransactionLog);
        }

        public void AcceptUpdateData(DataSet dataSet, Guid guid, string userName = null, string userComment = null, bool avoidAuditTrace = false, bool updateTransactionLog = true)
        {
            if (recipeUAConnector != null)
            {
                // Write only deleted dataSet records
                var dataDeleted = dataSet.GetChanges(DataRowState.Deleted);
                if (dataDeleted != null)
                {
                    dataDeleted.RejectChanges();

                    var xmlDataSet = new System.Text.StringBuilder();
                    System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings()
                    {
                        Encoding = System.Text.Encoding.UTF8,
                        OmitXmlDeclaration = false,
                        Indent = false,
                        CloseOutput = true,
                        CheckCharacters = false
                    };

                    using (var xmlWriter = System.Xml.XmlWriter.Create(xmlDataSet, settings))
                    {
#if DEBUG
                        //dataDeleted.WriteXmlSchema(@"C:\Temp\DataSetSchema_Deleted.xml");
                        //dataDeleted.WriteXml(@"C:\Temp\DataSet_Deleted.xml", XmlWriteMode.IgnoreSchema);
#endif
                        dataDeleted.WriteXml(xmlWriter, System.Data.XmlWriteMode.IgnoreSchema);
                        recipeUAConnector.WriteXmlDataSet(xmlDataSet.ToString(), DataRowState.Deleted, guid, userComment);
                    }
                }

                // Write only added dataSet records
                var dataAdded = dataSet.GetChanges(DataRowState.Added);
                if (dataAdded != null)
                {
                    dataAdded.AcceptChanges();

                    var xmlDataSet = new System.Text.StringBuilder();
                    System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings()
                    {
                        Encoding = System.Text.Encoding.UTF8,
                        OmitXmlDeclaration = false,
                        Indent = false,
                        CloseOutput = true,
                        CheckCharacters = false
                    };

                    using (var xmlWriter = System.Xml.XmlWriter.Create(xmlDataSet, settings))
                    {
#if DEBUG
                        //dataAdded.WriteXmlSchema(@"C:\Temp\DataSetSchema_Added.xml");
                        //dataAdded.WriteXml(@"C:\Temp\DataSet_Added.xml", XmlWriteMode.IgnoreSchema);
#endif
                        dataAdded.WriteXml(xmlWriter, System.Data.XmlWriteMode.IgnoreSchema);
                        recipeUAConnector.WriteXmlDataSet(xmlDataSet.ToString(), DataRowState.Added, guid, userComment);
                    }
                }

                // Write only modified dataSet records
                var dataModified = dataSet.GetChanges(DataRowState.Modified);
                if (dataModified != null)
                {
                    dataModified.AcceptChanges();

                    var xmlDataSet = new System.Text.StringBuilder();
                    System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings()
                    {
                        Encoding = System.Text.Encoding.UTF8,
                        OmitXmlDeclaration = false,
                        Indent = false,
                        CloseOutput = true,
                        CheckCharacters = false
                    };

                    using (var xmlWriter = System.Xml.XmlWriter.Create(xmlDataSet, settings))
                    {
#if DEBUG
                        //dataModified.WriteXmlSchema(@"C:\Temp\DataSetSchema_Modified.xml");
                        //dataModified.WriteXml(@"C:\Temp\DataSet_Modified.xml", XmlWriteMode.IgnoreSchema);
#endif
                        dataModified.WriteXml(xmlWriter, System.Data.XmlWriteMode.IgnoreSchema);
                        recipeUAConnector.WriteXmlDataSet(xmlDataSet.ToString(), DataRowState.Modified, guid, userComment);
                    }
                }

                for (int cc = 0; cc < dataSet.Tables.Count; cc++)
                {
                    DataView dataView = new DataView(dataSet.Tables[cc]);

                    String rowFilter = null;
                    if (guid != Guid.Empty)
                        rowFilter = String.Format("[{0}]='{1}'", dataView.Table.PrimaryKey[0].ColumnName, guid);

                    // deleted rows
                    dataView.RowFilter = rowFilter ?? String.Empty;
                    dataView.RowStateFilter = DataViewRowState.Deleted;
                    foreach (DataRowView rowView in dataView)
                        rowView.Row.AcceptChanges();

                    // added rows
                    dataView.RowFilter = rowFilter ?? String.Empty;
                    dataView.RowStateFilter = DataViewRowState.Added;
                    foreach (DataRowView rowView in dataView)
                        rowView.Row.AcceptChanges();

                    // update rows
                    dataView.RowFilter = rowFilter ?? String.Empty;
                    dataView.RowStateFilter = DataViewRowState.ModifiedCurrent;
                    foreach (DataRowView rowView in dataView)
                        rowView.Row.AcceptChanges();
                }
            }
            else
            {
                using (var writer = new DataWriter.DataSetWriter(dataProvider, connectionString))
                {
                    var dataDeleted = dataSet.GetChanges(DataRowState.Deleted);
                    if (dataDeleted != null)
                        dataDeleted.RejectChanges();
                    var dataAdded = dataSet.GetChanges(DataRowState.Added);
                    var dataModified = dataSet.GetChanges(DataRowState.Modified);
                    var dictTransaction = new Dictionary<Guid, List<object>>();
                    for (int cc = 0; cc < dataSet.Tables.Count; cc++)
                    {
                        DataView dataView = new DataView(dataSet.Tables[cc]);

                        bool bAuditTrace = !avoidAuditTrace && RecipeDocument.RecipeEntity.AuditTraceEnabled && dataView.Table.TableName == DataSetHelper.TableName(RecipeDocument.RecipeEntity);

                        String rowFilter = null;
                        if (guid != Guid.Empty)
                            rowFilter = String.Format("[{0}]='{1}'", dataView.Table.PrimaryKey[0].ColumnName, guid);

                        // deleted rows
                        dataView.RowFilter = rowFilter ?? String.Empty;
                        dataView.RowStateFilter = DataViewRowState.Deleted;
                        writer.DeleteRows(dataView);
                        foreach (DataRowView rowView in dataView)
                        {
                            String recipeIndex = null;
                            if (bAuditTrace)
                                recipeIndex = rowView.Row[DataSetHelper.ColumnName(RecipeDocument.RecipeEntity), DataRowVersion.Original].ToString();
                            rowView.Row.AcceptChanges();
                            if (bAuditTrace)
                                NotifyAuditTraceMessage(recipeIndex, dataDeleted?.GetXml(), userName, userComment, AuditLogEntryType.Deleted);
                        }

                        // added rows
                        dataView.RowFilter = rowFilter ?? String.Empty;
                        dataView.RowStateFilter = DataViewRowState.Added;
                        writer.InsertRows(dataView);
                        foreach (DataRowView rowView in dataView)
                        {
                            rowView.Row.AcceptChanges();
                            if (bAuditTrace)
                            {
                                var recipeIndex = rowView.Row[DataSetHelper.ColumnName(RecipeDocument.RecipeEntity)].ToString();
                                NotifyAuditTraceMessage(recipeIndex, dataAdded?.GetXml(), userName, userComment, AuditLogEntryType.Added);
                            }
                        }

                        // update rows
                        dataView.RowFilter = rowFilter ?? String.Empty;
                        dataView.RowStateFilter = DataViewRowState.ModifiedCurrent;
                        writer.UpdateRows(dataView);
                        foreach (DataRowView rowView in dataView)
                        {
                            rowView.Row.AcceptChanges();
                            if (bAuditTrace)
                            {
                                var recipeIndex = rowView.Row[DataSetHelper.ColumnName(RecipeDocument.RecipeEntity)].ToString();
                                NotifyAuditTraceMessage(recipeIndex, dataModified?.GetXml(), userName, userComment, AuditLogEntryType.Modified);
                            }
                            if (dictTransaction.ContainsKey(guid))
                                dictTransaction[guid].Add(rowView.Row);
                            else
                                dictTransaction.Add(guid, new List<object> { rowView.Row });
                        }
                    }
#if !NET_STANDARD
                    if (transactionLogger != null && updateTransactionLog)
                    {
                        if(dataAdded != null)
                            transactionLogger.UpdateTransactionLog(dataAdded, RecipeDocument, TransactionLogEventType.Added);
                        if(dataDeleted != null)
                            transactionLogger.UpdateTransactionLog(dataDeleted, RecipeDocument, TransactionLogEventType.Deleted);
                        if (dataModified != null)
                            transactionLogger.UpdateTransactionLog(dataModified, RecipeDocument, TransactionLogEventType.Modified);
                    }
#endif
                }
            }
        }

        public void RejectUpdateData(DataSet dataSet)
        {
            RejectUpdateData(dataSet, Guid.Empty);
        }

        public void RejectUpdateData(DataSet dataSet, Guid guid)
        {
            for (int cc = dataSet.Tables.Count - 1; cc >= 0; cc--)
            {
                DataView dataView = new DataView(dataSet.Tables[cc]);

                String rowFilter = null;
                if (guid != Guid.Empty)
                    rowFilter = String.Format("[{0}]='{1}'", dataView.Table.PrimaryKey[0].ColumnName, guid);

                // deleted rows
                dataView.RowFilter = rowFilter ?? String.Empty;
                dataView.RowStateFilter = DataViewRowState.Deleted;
                foreach (DataRowView rowView in dataView)
                    rowView.Row.RejectChanges();

                // added rows
                dataView.RowFilter = rowFilter ?? String.Empty;
                dataView.RowStateFilter = DataViewRowState.Added;
                foreach (DataRowView rowView in dataView)
                    rowView.Row.RejectChanges();

                // update rows
                dataView.RowFilter = rowFilter ?? String.Empty;
                dataView.RowStateFilter = DataViewRowState.ModifiedCurrent;
                foreach (DataRowView rowView in dataView)
                    rowView.Row.RejectChanges();
            }
        }

        public void UpdateActivationTime(DataSet dataSet)
        {
            UpdateActivationTime(dataSet, Guid.Empty);
        }

        public void UpdateActivationTime(DataSet dataSet, Guid guid, string userName = null, string userComment = null)
        {
            if (recipeUAConnector != null)
            {
                DataView dataView = new DataView(dataSet.Tables[DataSetHelper.TableName(RecipeDocument.RecipeEntity)]);

                String rowFilter = null;
                if (guid != Guid.Empty)
                    rowFilter = String.Format("[{0}]='{1}'", dataView.Table.PrimaryKey[0].ColumnName, guid);

                var columns = new List<String>();
                var activationDateTimeColumnName = DataSetHelper.ActivationDateTimeColumnName(RecipeDocument.RecipeEntity);

                dataView.RowFilter = rowFilter ?? String.Empty;
                foreach (DataRowView rowView in dataView)
                    rowView[activationDateTimeColumnName] = DateTime.UtcNow;

                var dataModified = dataSet.GetChanges(DataRowState.Modified);
                if (dataModified != null)
                {
                    var xmlDataSet = new System.Text.StringBuilder();
                    System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings()
                    {
                        Encoding = System.Text.Encoding.UTF8,
                        OmitXmlDeclaration = false,
                        Indent = false,
                        CloseOutput = true,
                        CheckCharacters = false
                    };

                    using (var xmlWriter = System.Xml.XmlWriter.Create(xmlDataSet, settings))
                    {
                        dataModified.WriteXml(xmlWriter, System.Data.XmlWriteMode.IgnoreSchema);
                        recipeUAConnector.WriteXmlDataSet(xmlDataSet.ToString(), DataRowState.Modified, guid);
                    }
                }

                foreach (DataRowView rowView in dataView)
                    rowView.Row.AcceptChanges();
            }
            else
            {
                using (var writer = new DataWriter.DataSetWriter(dataProvider, connectionString))
                {
                    var dataModified = new Dictionary<DataRowView, String>();
                    DataView dataView = new DataView(dataSet.Tables[DataSetHelper.TableName(RecipeDocument.RecipeEntity)]);

                    String rowFilter = null;
                    if (guid != Guid.Empty)
                        rowFilter = String.Format("[{0}]='{1}'", dataView.Table.PrimaryKey[0].ColumnName, guid);

                    var columns = new List<String>();
                    var activationDateTimeColumnName = DataSetHelper.ActivationDateTimeColumnName(RecipeDocument.RecipeEntity);
                    columns.Add(activationDateTimeColumnName);

                    dataView.RowFilter = rowFilter ?? String.Empty;
                    foreach (DataRowView row in dataView)
                    {
                        row[activationDateTimeColumnName] = DateTime.UtcNow;
                        if (RecipeDocument.RecipeEntity.AuditTraceEnabled)
                            dataModified.Add(row, dataSet.GetChanges(DataRowState.Modified)?.GetXml());
                    }

                    dataView.RowStateFilter = DataViewRowState.OriginalRows;
                    if (dataView.Count > 0)
                    {
                        dataView.RowStateFilter = DataViewRowState.CurrentRows;
                        writer.UpdateRows(dataView, columns, skipOrUseColumns: false);
                        foreach (DataRowView rowView in dataView)
                        {
                            rowView.Row.AcceptChanges();
                            if (RecipeDocument.RecipeEntity.AuditTraceEnabled && dataModified.ContainsKey(rowView))
                            {
                                var recipeIndex = rowView.Row[DataSetHelper.ColumnName(RecipeDocument.RecipeEntity)].ToString();
                                NotifyAuditTraceMessage(recipeIndex, dataModified[rowView], userName, userComment, AuditLogEntryType.Activated);
                            }
                        }
                    }
                }
            }
        }

        public void GetInDataServerValues(DataSet dataSet, Guid guid, int timeout, CancellationToken token)
        {
            if (recipeUAConnector != null)
            {
                var maxDateTime = DateTime.UtcNow.AddMilliseconds(Properties.Settings.Default.defaultSyncTimeout);
                while (!recipeUAConnector.CanGetInDataServerValues())
                {
                    token.ThrowIfCancellationRequested();
                    if (DateTime.UtcNow > maxDateTime)
                        throw new TimeoutException(Properties.Resources.TimeoutReadingData);
                    Thread.Sleep(10);
                }

                var xmlDataSetBuilder = new System.Text.StringBuilder();
                System.Xml.XmlWriterSettings writerSettings = new System.Xml.XmlWriterSettings()
                {
                    Encoding = System.Text.Encoding.UTF8,
                    OmitXmlDeclaration = false,
                    Indent = false,
                    CloseOutput = true,
                    CheckCharacters = false
                };

                using (var xmlWriter = System.Xml.XmlWriter.Create(xmlDataSetBuilder, writerSettings))
                {
                    dataSet.WriteXml(xmlWriter, System.Data.XmlWriteMode.IgnoreSchema);
                }

                var result = recipeUAConnector.GetInDataServerValues(xmlDataSetBuilder.ToString(), guid, timeout);
                String xmlDataSet = null;
                if (result != null && result.Count > 0)
                    xmlDataSet = result[0].ToString();

                if (xmlDataSet != null)
                {
                    var xmlBuilder = new System.Text.StringBuilder();
                    System.Xml.XmlReaderSettings readingSettings = new System.Xml.XmlReaderSettings()
                    {
                        ConformanceLevel = System.Xml.ConformanceLevel.Document,
                        CloseInput = true,
                        CheckCharacters = false
                    };

                    using (var ds = CreateDataSet(fill: false))
                    {
                        using (var stringReader = new System.IO.StringReader(xmlDataSet))
                        {
                            using (var xmlReader = System.Xml.XmlReader.Create(stringReader, readingSettings))
                            {
                                foreach (DataTable dataTable in ds.Tables)
                                    dataTable.BeginLoadData();

                                // **********************************************************
                                // Moving two node away is needed for reading all records
                                xmlReader.MoveToContent();
                                xmlReader.Read();
                                // **********************************************************
                                ds.ReadXml(xmlReader, XmlReadMode.IgnoreSchema);

                                foreach (DataTable dataTable in ds.Tables)
                                    dataTable.EndLoadData();
                            }
                        }
                        dataSet.Merge(ds);
                    }
                }
            }
            else
            {
                var reader = recipeReader;
                if (reader != null)
                {
                    if (!reader.CanExecute(Properties.Settings.Default.defaultSyncTimeout))
                        throw new TimeoutException(Properties.Resources.TimeoutReadingData);

                    ExecutionResult result = new ExecutionResult() { ResultState = true };

                    DataView viewRecipes = new DataView(dataSet.Tables[DataSetHelper.TableName(RecipeDocument.RecipeEntity)],
                                                        String.Format("[{0}]='{1}'", DataSetHelper.PrimaryKeyName(RecipeDocument.RecipeEntity), guid),
                                                        String.Empty,
                                                        DataViewRowState.CurrentRows);

                    if (viewRecipes.Count != 0)
                    {
                        for (int cc = 0; cc < dataSet.Tables.Count; cc++)
                        {
                            if (dataSet.Tables[cc].DefaultView.RowStateFilter != DataViewRowState.CurrentRows)
                                dataSet.Tables[cc].DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
                            dataSet.Tables[cc].DefaultView.RowFilter = String.Empty;
                            dataSet.Tables[cc].DefaultView.RowFilter = String.Format("[{0}]='{1}'",
                                                                        dataSet.Tables[cc].PrimaryKey[0].ColumnName,
                                                                        guid);
                        }

                        VariantCollection values = new VariantCollection();
                        if (RecipeDocument.RecipeEntity.IsReadable())
                        {
                            // first write the datavalues without group with starting address
                            var readableedatavalues = (from c in RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                                                       where c.IsReadable() && String.IsNullOrEmpty(c.StartingAddress)
                                                       select c).ToList();

                            if (readableedatavalues.Count > 0)
                            {
                                values = DataSetHelper.GetDBValues(readableedatavalues, dataSet, guid);
                                result = reader.Execute(RecipeDocument.RecipeEntity.StartingAddress, values.ToArray(), timeout);
                                if (result.ResultState)
                                    DataSetHelper.SetDBValues(readableedatavalues, dataSet, result.OutputValues);
                            }
                        }

                        if (result.ResultState)
                        {
                            // next write the datavalues inside group with starting address
                            var validgroups = (from c in RecipeDocument.RecipeEntity.Groups
                                               where c.IsReadable()
                                               select c).ToList();

                            foreach (var recipegroup in validgroups)
                            {
                                var readableedatavalues = (from c in recipegroup.DataValues
                                                           where c.UseInCommunication && String.IsNullOrEmpty(c.StartingAddress)
                                                           orderby c.OID ascending
                                                           select c).ToList();

                                if (readableedatavalues.Count > 0)
                                {
                                    values = DataSetHelper.GetDBValues(readableedatavalues, dataSet, guid);
                                    result = reader.Execute(recipegroup.StartingAddress, values.ToArray(), timeout);
                                }

                                if (!result.ResultState)
                                    break;

                                DataSetHelper.SetDBValues(readableedatavalues, dataSet, result.OutputValues);
                            }
                        }

                        if (result.ResultState)
                        {
                            // end write the datavalues with starting address
                            var readableedatavalues = (from c in RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                                                       where c.IsReadable() && !String.IsNullOrEmpty(c.StartingAddress)
                                                       select c).ToList();

                            List<UFDataValueEntity> list = new List<UFDataValueEntity>();
                            foreach (var datavalue in readableedatavalues)
                            {
                                list.Add(datavalue);
                                values = DataSetHelper.GetDBValues(list, dataSet, guid);
                                result = reader.Execute(datavalue.StartingAddress, values.ToArray(), timeout);

                                if (!result.ResultState)
                                    break;

                                DataSetHelper.SetDBValues(list, dataSet, result.OutputValues);
                                list.Clear();
                            }
                        }

                        if (result.ResultState)
                        {
                            var readabledatavalues = (from c in RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                                                      where c.IsTagIOReferenceValid()
                                                      select c).ToList();
                            if (readabledatavalues.Count > 0)
                            {
                                values = DataSetHelper.GetDBValues(readabledatavalues, dataSet, guid, false);
                                result = reader.Execute(values.ToArray());
                                if (result.ResultState)
                                    DataSetHelper.SetDBValues(readabledatavalues, dataSet, result.OutputValues, false);
                            }
                        }
                    }

                    if (result.ExceptionInfo != null)
                    {
                        throw result.ExceptionInfo;
                    }
                    else if (!result.ResultState)
                    {
                        throw new TimeoutException(Properties.Resources.TimeoutReadingData);
                    }
                }
            }
        }

        public void GetOutDataServerValues(DataSet dataSet, Guid guid, int timeout, CancellationToken token, string userName = null, string userComment = null)
        {
            if (recipeUAConnector != null)
            {
                var maxDateTime = DateTime.UtcNow.AddMilliseconds(Properties.Settings.Default.defaultSyncTimeout);
                while (!recipeUAConnector.CanGetOutDataServerValues())
                {
                    token.ThrowIfCancellationRequested();
                    if (DateTime.UtcNow > maxDateTime)
                        throw new TimeoutException(Properties.Resources.TimeoutWritingData);
                    Thread.Sleep(10);
                }

                var xmlDataSet = new System.Text.StringBuilder();
                System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings()
                {
                    Encoding = System.Text.Encoding.UTF8,
                    OmitXmlDeclaration = false,
                    Indent = false,
                    CloseOutput = true,
                    CheckCharacters = false
                };

                using (var xmlWriter = System.Xml.XmlWriter.Create(xmlDataSet, settings))
                {
                    dataSet.WriteXml(xmlWriter, System.Data.XmlWriteMode.IgnoreSchema);
                    recipeUAConnector.GetOutDataServerValues(xmlDataSet.ToString(), guid, timeout, userComment);
                }
            }
            else
            {
                var writer = recipeWriter;
                if (writer != null)
                {
                    if (!writer.CanExecute(Properties.Settings.Default.defaultSyncTimeout))
                        throw new TimeoutException(Properties.Resources.TimeoutWritingData);

                    ExecutionResult result = new ExecutionResult() { ResultState = true };

                    DataView viewRecipes = new DataView(dataSet.Tables[DataSetHelper.TableName(RecipeDocument.RecipeEntity)],
                                                        String.Format("[{0}]='{1}'", DataSetHelper.PrimaryKeyName(RecipeDocument.RecipeEntity), guid),
                                                        String.Empty,
                                                        DataViewRowState.CurrentRows);
                    if (viewRecipes.Count > 0)
                    {
                        for (int cc = 0; cc < dataSet.Tables.Count; cc++)
                        {
                            if (dataSet.Tables[cc].DefaultView.RowStateFilter != DataViewRowState.CurrentRows)
                                dataSet.Tables[cc].DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
                            dataSet.Tables[cc].DefaultView.RowFilter = String.Empty;
                            dataSet.Tables[cc].DefaultView.RowFilter = String.Format("[{0}]='{1}'",
                                                                        dataSet.Tables[cc].PrimaryKey[0].ColumnName,
                                                                        guid);
                        }

                        VariantCollection values = new VariantCollection();
                        if (RecipeDocument.RecipeEntity.IsWritable())
                        {
                            // first write the datavalues without group with starting address
                            var writabledatavalues = (from c in RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                                                      where c.IsWritable() && String.IsNullOrEmpty(c.StartingAddress)
                                                      select c).ToList();

                            if (writabledatavalues.Count > 0)
                            {
                                values = DataSetHelper.GetDBValues(writabledatavalues, dataSet, guid);
                                result = writer.Execute(RecipeDocument.RecipeEntity.StartingAddress, values.ToArray(), timeout);
                            }
                        }

                        if (result.ResultState)
                        {
                            // next write the datavalues inside group with starting address
                            var validgroups = (from c in RecipeDocument.RecipeEntity.Groups
                                               where c.IsWritable()
                                               select c).ToList();

                            foreach (var recipegroup in validgroups)
                            {
                                var writabledatavalues = (from c in recipegroup.DataValues
                                                          where c.UseInCommunication && String.IsNullOrEmpty(c.StartingAddress)
                                                          orderby c.OID ascending
                                                          select c).ToList();

                                if (writabledatavalues.Count > 0)
                                {
                                    values = DataSetHelper.GetDBValues(writabledatavalues, dataSet, guid);
                                    result = writer.Execute(recipegroup.StartingAddress, values.ToArray(), timeout);
                                }

                                if (!result.ResultState)
                                    break;
                            }
                        }

                        if (result.ResultState)
                        {
                            // end write the datavalues with starting address
                            var writabledatavalues = (from c in RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                                                      where c.IsWritable() && !String.IsNullOrEmpty(c.StartingAddress)
                                                      select c).ToList();

                            List<UFDataValueEntity> list = new List<UFDataValueEntity>();
                            foreach (var datavalue in writabledatavalues)
                            {
                                list.Add(datavalue);
                                values = DataSetHelper.GetDBValues(list, dataSet, guid);
                                result = writer.Execute(datavalue.StartingAddress, values.ToArray(), timeout);

                                if (!result.ResultState)
                                    break;

                                list.Clear();
                            }
                        }

                        if (result.ResultState)
                        {
                            var writabledatavalues = (from c in RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                                                      where c.IsTagIOReferenceValid()
                                                      select c).ToList();
                            if (writabledatavalues.Count > 0)
                            {
                                values = DataSetHelper.GetDBValues(writabledatavalues, dataSet, guid, false);
                                result = writer.Execute(values.ToArray());
                            }
                        }
                    }

                    if (result.ExceptionInfo != null)
                    {
                        throw result.ExceptionInfo;
                    }
                    else if (!result.ResultState)
                    {
                        throw new TimeoutException(Properties.Resources.TimeoutWritingData);
                    }
                    else if (viewRecipes.Count > 0)
                    {
                        UpdateActivationTime(dataSet, guid, userName, userComment);
                    }
                }
            }
        }

        public void CheckAndVerifyDatabase(bool clear = false)
        {
            CheckAndVerifyDatabase(CancellationToken.None, clear);
        }

        public void CheckAndVerifyDatabase(CancellationToken token, bool clear = false)
        {
            if (String.IsNullOrEmpty(dataProvider) || String.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(String.Format(Properties.Resources.InvalidConnectionString, RecipeDocument.RecipeEntity.Name));
            }

            if (recipeUAConnector == null)
            {
                DataSet dataSet = null;
                try
                {
                    lock (recipeCheckingState)
                    {
                        recipeCheckingState[RecipeDocument.FullPath] = RecipeCheckingStateEnum.Checking;
                    }

                    dataSet = CreateDataSet(token);
                    using (var writer = new DataWriter.DataSetWriter(dataProvider, connectionString))
                    {
                        if (!writer.TryOpenConnection())
                        {
                            writer.TryCreateDataBase();
                            writer.OpenConnection();
                        }

                        if (clear)
                        {
                            try
                            {
                                foreach (DataTable table in dataSet.Tables)
                                    writer.RemoveTable(table.TableName);
                            }
                            catch
                            { }
                        }

                        try
                        {
                            writer.CheckTables(dataSet, token, RecipeDocument.RecipeEntity.SkipCheckColumnsType);
                        }
                        catch (Exception ex)
                        {
                            if (!(ex is DataWriter.InvalidSchemaTableException))
                            {
                                throw new InvalidOperationException(
                                    String.Format(Properties.Resources.CheckTableFailed.Replace("-newline-", Environment.NewLine),
                                    RecipeDocument.RecipeEntity.RecipeName, ex.Message));
                            }

                            var exInvalidSchema = ex as DataWriter.InvalidSchemaTableException;
                            if (exInvalidSchema.FaultOperation == DataWriter.FaultOperation.ChangeSchema ||
                                exInvalidSchema.FaultOperation == DataWriter.FaultOperation.RebuildPrimaryKeys)
                            {
                                foreach (DataTable table in dataSet.Tables)
                                {
                                    writer.TryDropConstraints(table.TableName);

                                    var newTableName = string.Format("{0}.{1}", table.TableName, Guid.NewGuid());
                                    writer.RenameTable(table.TableName, newTableName);
                                    logServer.WarnFormat(Properties.Resources.FailedToChangeRecipeSchema, exInvalidSchema.Message, table.TableName, newTableName);
                                }

                                try
                                {
                                    writer.CheckTables(dataSet, token, RecipeDocument.RecipeEntity.SkipCheckColumnsType);
                                }
                                catch (Exception exception)
                                {
                                    throw new InvalidOperationException(
                                    String.Format(Properties.Resources.CheckTableFailed.Replace("-newline-", Environment.NewLine),
                                    RecipeDocument.RecipeEntity.RecipeName, exception.Message));
                                }
                            }
                            else
                                throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    lock (recipeCheckingState)
                    {
                        recipeCheckingState[RecipeDocument.FullPath] = RecipeCheckingStateEnum.Error;
                    }

                    throw ex;
                }
                finally
                {
                    if (dataSet != null)
                        dataSet.Dispose();

                    lock (recipeCheckingState)
                    {
                        recipeCheckingState[RecipeDocument.FullPath] = RecipeCheckingStateEnum.None;
                    }
                }

                lock (recipeCheckingState)
                {
                    recipeCheckingState[RecipeDocument.FullPath] = RecipeCheckingStateEnum.Checked;
                }
            }
        }

        public void ForceUpdateRecipeTags()
        {
            if (recipeUAConnector != null)
            {
                try
                {
                    recipeUAConnector.UpdateRecipeTags();
                }
                catch (Exception ex)
                {
                    logServer.Error(ex.Message);
                }
            }
            else
            {
                ForceUpdateRecipeTagList();
                ForceUpdateRecipeTagIndex();
            }
        }

#if !NET_STANDARD
        /// <summary>
        /// Read the transaction log starting from the UTC DateTime indicated
        /// </summary>
        /// <param name="transactionLogUTCDateTime"></param>
        /// <returns>List of UFRecipeTransactionLogItem</returns>
        public List<UFRecipeTransactionLogItem> ReadTransactionLog(DateTime transactionLogUTCDateTime, string RecipeName)
        {
            try
            {
                return transactionLogger.ReadTransactionLog(transactionLogUTCDateTime, RecipeName);
            }
            catch
            {
                return new List<UFRecipeTransactionLogItem>();
            }
        }

        public void UpdateRedundancyData(List<UFRecipeTransactionLogItem> updatedItems)
        {
            if (updatedItems != null)
            {
                foreach (var item in updatedItems)
                {
                    using (var dataSet = CreateDataSet(fill: false))
                    {
                        System.Xml.XmlReaderSettings settings = new System.Xml.XmlReaderSettings()
                        {
                            ConformanceLevel = System.Xml.ConformanceLevel.Document,
                            CloseInput = true,
                            CheckCharacters = false
                        };

                        using (var stringReader = new StringReader(item.XmlDataSet))
                        {
                            using (var xmlReader = System.Xml.XmlReader.Create(stringReader, settings))
                            {
                                foreach (DataTable dataTable in dataSet.Tables)
                                    dataTable.BeginLoadData();

                                // **********************************************************
                                // Moving two node away is needed for reading all records
                                xmlReader.MoveToContent();
                                xmlReader.Read();
                                // **********************************************************
                                dataSet.ReadXml(xmlReader, XmlReadMode.IgnoreSchema);

                                foreach (DataTable dataTable in dataSet.Tables)
                                    dataTable.EndLoadData();
                            }
                        }
                        MergeAndUpdateData(dataSet, item.EventType);
                    }
                }
            }
        }

        public UFRecipeTransactionLogItem GetLastTransactionLogItem(string recipeName)
        {
            var itemsList = ReadTransactionLog(DateTime.MinValue, recipeName);
            if(itemsList.Count() > 0)
                return itemsList.OrderBy(x => x.EventDateTimeUtc).ToList().Last();
            return null;
        }
#endif
        #endregion

        #region Properties

        public UFRecipeDocument RecipeDocument { get; private set; }

        public RecipeUAViewModel RecipeUAViewModel
        {
            get
            {
                if (recipeUAConnector != null)
                    return recipeUAConnector.recipeUAViewModel;
                return null;
            }
        }

        public RecipeCheckingStateEnum CheckState
        {
            get
            {
                lock (recipeCheckingState)
                {
                    if (!recipeCheckingState.ContainsKey(RecipeDocument.FullPath))
                        return RecipeCheckingStateEnum.None;

                    return recipeCheckingState[RecipeDocument.FullPath];
                }
            }
        }

        IUFUAEditorManager ufuaEditorService;
        internal IUFUAEditorManager UfuaEditorService
        {
            get
            {
                if (ufuaEditorService == null)
                {
                    ufuaEditorService = RecipeDocument.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                }

                return ufuaEditorService;
            }
        }

#if !NET_STANDARD
        IStringEditorManager stringEditorManager;
        IStringEditorManager StringEditorManager
        {
            get
            {
                if (stringEditorManager == null)
                {
                    stringEditorManager = RecipeDocument.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                }

                return stringEditorManager;
            }
        }

        IUIMsgBoxAlertService uiinterface;
        IUIMsgBoxAlertService UIInterface
        {
            get
            {
                if (uiinterface == null)
                {
                    uiinterface = RecipeDocument.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                }

                return uiinterface;
            }
        }
#endif

        RecipeExecutionStateEnum executionState = RecipeExecutionStateEnum.None;
        public RecipeExecutionStateEnum ExecutionState
        {
            get
            {
                return executionState;
            }
            private set
            {
                if (executionState == value)
                    return;

                var oldState = executionState;
                executionState = value;
                UpdateRecipeTagState();

                OnStateChanged(oldState, value);
            }
        }

#endregion

#region Private Methods
        void PrepareExecution(String sessionname)
        {
            if (bExecuted)
                return;
            bExecuted = true;

            RecipeDocument.UpdateSessionSettings();
            currentSessionName = sessionname;

            ExecutionState = RecipeExecutionStateEnum.Initialization;

            if (recipeUAConnector == null)
            {
#if !NET_STANDARD
                //In case of redundancy the Recipe Tags subscription has to be
                //executed after this phase because otherwise the connected client
                //may show wrong data (for example in case the server will be the
                //active one and it has to update the data stored in the database)
                if(!IsRedundancyEnabled)
#endif
                    SubscribeRecipeTags();
                SubscribeRecipeReader();
                SubscribeRecipeWriter();
            }
            else
                recipeUAConnector.PrepareExecution(currentSessionName);

            ExecutionState &= ~RecipeExecutionStateEnum.Initialization;
        }

        void TerminateExecution()
        {
            if (!bExecuted)
                return;
            bExecuted = false;

            ExecutionState = RecipeExecutionStateEnum.Finalization;

            UnsubscribeRecipeReader();
            UnsubscribeRecipeWriter();
            UnsubscribeRecipeTags();
            UnsubscribeDataValues();
            recipeUAConnector?.TerminateExecution();

            ExecutionState = RecipeExecutionStateEnum.None;
        }

        void StartNewAction(RecipeAction action)
        {
            lock (pendingActions)
            {
                if (action.IsCommandAction && pendingCommands >= Properties.Settings.Default.MaxPendingActions)
                {
                    string message = String.Format(Properties.Resources.MaxPendingActionsExceeded, action.Identity);
                    throw new OverflowException(message);
                }
                else
                {
                    if (action.IsCommandAction)
                        pendingCommands++;
                    pendingActions[action.Identity] = action;
                }
                
                EnsureTaskExecution();
#if DEBUG
                Debug.WriteLine("UFRecipeExecuter - Added Action = {0}, Total Pending Actions = {1}", action.Identity, pendingActions.Count);
#endif
            }
        }

        void EnsureTaskExecution()
        {
            if (taskPendingActions == null)
            {
                taskPendingActions = Task.Factory.StartNew(() =>
                {
                    RecipeAction action = null;
                    lock (pendingActions)
                    {
                        action = pendingActions[0] as RecipeAction;
                        pendingActions.RemoveAt(0);
                        if (action.IsCommandAction)
                            pendingCommands--;
                    }
#if DEBUG
                    Debug.WriteLine("UFRecipeExecuter - Removed Action = {0}, Total Pending Actions = {1}", action.Identity, pendingActions.Count);
#endif
                    action.Execute();
                }, ctsPendingTasks.Token);

                taskPendingActions.ContinueWith((T) =>
                {
                    lock (pendingActions)
                    {
                        taskPendingActions = null;
                        if (pendingActions.Count > 0)
                            EnsureTaskExecution();
                    }
                }, ctsPendingTasks.Token);
            }
        }

        void TerminatePendingTasks()
        {
            ctsPendingTasks.Cancel();

            Task task = null;
            lock (pendingActions)
            {
                task = taskPendingActions;
            }

            if (task != null)
                task.Wait();

            ctsPendingTasks.Dispose();
        }

        void SubscribeRecipeReader()
        {
            if (recipeReader == null)
            {
                recipeReader = new RecipeEntityHelper(RecipeDocument, UfuaEditorService, RecipeDocument.RecipeEntity, false);
                recipeReader.PrepareExecution(currentSessionName);
            }
        }

        void UnsubscribeRecipeReader()
        {
            if (recipeReader != null)
            {
                recipeReader.TerminateExecution();
                recipeReader.Dispose();
                recipeReader = null;
            }
        }

        private void RecipeWriterComplete(object sender, EventArgs e)
        {
            CheckAndVerifyDatabaseAsync(logError: true);
        }

        void SubscribeRecipeWriter()
        {
            if (recipeWriter == null)
            {
                recipeWriter = new RecipeEntityHelper(RecipeDocument, UfuaEditorService, RecipeDocument.RecipeEntity, true);
                var dataValuesInherited = from x in RecipeDocument.RecipeEntity.DataValues.AsParallel()
                               where x.InheritDataTypeFromTag == true
                               select x;
                //In case there isn't any dataValue with Inherit Data Type From Tag checked, the check of database can start immediately
                //otherwise it will start when the dataValue have inherited the data type from associate I/O Tag
                if (dataValuesInherited.Count() > 0)
                    recipeWriter.OnComplete += RecipeWriterComplete;
                else
                    CheckAndVerifyDatabaseAsync(logError: true);
                recipeWriter.PrepareExecution(currentSessionName);
            }
        }

        void UnsubscribeRecipeWriter()
        {
            if (recipeWriter != null)
            {
                recipeWriter.OnComplete -= RecipeWriterComplete;
                recipeWriter.TerminateExecution();
                recipeWriter.Dispose();
                recipeWriter = null;
            }
        }

        void SubscribeRecipeTags()
        {
            List<SubscribeTagReference> values = null;
            lock (subscribedRecipeTags)
            {
                RecipeDocument.RecipeEntity.TagRecipeIndex = new OPCUAEntityReference(RecipeDocument.RecipeEntity.TagRecipeIndex);
                if (RecipeDocument.RecipeEntity.TagRecipeIndex != null && RecipeDocument.RecipeEntity.TagRecipeIndex.IsValid)
                {
                    subscribedRecipeTags.Add(new SubscribeTagReference(this,
                        RecipeDocument.RecipeEntity.TagRecipeIndex,
                        new Action<MonitoredItemViewModel>(RecipeTagIndexChanged), monitor: true));
                }

                RecipeDocument.RecipeEntity.TagRecipeLoad = new OPCUAEntityReference(RecipeDocument.RecipeEntity.TagRecipeLoad);
                if (RecipeDocument.RecipeEntity.TagRecipeLoad != null && RecipeDocument.RecipeEntity.TagRecipeLoad.IsValid)
                {
                    subscribedRecipeTags.Add(new SubscribeTagReference(this,
                        RecipeDocument.RecipeEntity.TagRecipeLoad,
                        new Action<MonitoredItemViewModel>(RecipeTagCommandChanged), monitor: true));
                }

                RecipeDocument.RecipeEntity.TagRecipeSave = new OPCUAEntityReference(RecipeDocument.RecipeEntity.TagRecipeSave);
                if (RecipeDocument.RecipeEntity.TagRecipeSave != null && RecipeDocument.RecipeEntity.TagRecipeSave.IsValid)
                {
                    subscribedRecipeTags.Add(new SubscribeTagReference(this,
                        RecipeDocument.RecipeEntity.TagRecipeSave,
                        new Action<MonitoredItemViewModel>(RecipeTagCommandChanged), monitor: true));
                }

                RecipeDocument.RecipeEntity.TagRecipeDelete = new OPCUAEntityReference(RecipeDocument.RecipeEntity.TagRecipeDelete);
                if (RecipeDocument.RecipeEntity.TagRecipeDelete != null && RecipeDocument.RecipeEntity.TagRecipeDelete.IsValid)
                {
                    subscribedRecipeTags.Add(new SubscribeTagReference(this,
                        RecipeDocument.RecipeEntity.TagRecipeDelete,
                        new Action<MonitoredItemViewModel>(RecipeTagCommandChanged), monitor: true));
                }

                RecipeDocument.RecipeEntity.TagRecipeWrite = new OPCUAEntityReference(RecipeDocument.RecipeEntity.TagRecipeWrite);
                if (RecipeDocument.RecipeEntity.TagRecipeWrite != null && RecipeDocument.RecipeEntity.TagRecipeWrite.IsValid)
                {
                    subscribedRecipeTags.Add(new SubscribeTagReference(this,
                        RecipeDocument.RecipeEntity.TagRecipeWrite,
                        new Action<MonitoredItemViewModel>(RecipeTagCommandChanged), monitor: true));
                }

                RecipeDocument.RecipeEntity.TagRecipeRead = new OPCUAEntityReference(RecipeDocument.RecipeEntity.TagRecipeRead);
                if (RecipeDocument.RecipeEntity.TagRecipeRead != null && RecipeDocument.RecipeEntity.TagRecipeRead.IsValid)
                {
                    subscribedRecipeTags.Add(new SubscribeTagReference(this,
                        RecipeDocument.RecipeEntity.TagRecipeRead,
                        new Action<MonitoredItemViewModel>(RecipeTagCommandChanged), monitor: true));
                }

                RecipeDocument.RecipeEntity.TagRecipeList = new OPCUAEntityReference(RecipeDocument.RecipeEntity.TagRecipeList);
                if (RecipeDocument.RecipeEntity.TagRecipeList != null && RecipeDocument.RecipeEntity.TagRecipeList.IsValid)
                {
                    subscribedRecipeTags.Add(new SubscribeTagReference(this,
                        RecipeDocument.RecipeEntity.TagRecipeList,
                        new Action<MonitoredItemViewModel>(UpdateRecipeTagList), monitor: false));
                }

                RecipeDocument.RecipeEntity.TagRecipeState = new OPCUAEntityReference(RecipeDocument.RecipeEntity.TagRecipeState);
                if (RecipeDocument.RecipeEntity.TagRecipeState != null && RecipeDocument.RecipeEntity.TagRecipeState.IsValid)
                {
                    subscribedRecipeTags.Add(new SubscribeTagReference(this,
                        RecipeDocument.RecipeEntity.TagRecipeState,
                        new Action<MonitoredItemViewModel>(UpdateRecipeTagState), monitor: false));
                }

                values = subscribedRecipeTags.ToList();
                if (values.Count > 0)
                {
                    if (checkSubscribedRecipeTags != null)
                        checkSubscribedRecipeTags.Dispose();
                    checkSubscribedRecipeTags = new Timer((o) =>
                    {
                        List<SubscribeTagReference> subscribed = null;
                        lock (subscribedRecipeTags)
                        {
                            subscribed = subscribedRecipeTags.ToList();
                            if (subscribed != null && subscribed.Count == 0)
                            {
                                if (checkSubscribedRecipeTags != null)
                                {
                                    checkSubscribedRecipeTags.Dispose();
                                    checkSubscribedRecipeTags = null;
                                }
                            }
                        }

                        if (subscribed != null && subscribed.Count > 0)
                        {
                            foreach (var item in subscribed)
                            {
                                if (StatusCode.IsBad(item.Quality))
                                {
                                    ExecutionState |= RecipeExecutionStateEnum.TimeoutOnConnectCommandTags;
                                    return;
                                }
                            }
                        }

                        ExecutionState &= ~RecipeExecutionStateEnum.TimeoutOnConnectCommandTags;
                    }, this, TimeSpan.FromMilliseconds(Properties.Settings.Default.defaultSyncTimeout), TimeSpan.FromMilliseconds(Properties.Settings.Default.defaultSyncTimeout));
                }
            }

            if (values != null && values.Count > 0)
            {
                values.ForEach((item) =>
                {
                    item.Subscribe();
                });
            }
        }

        void UnsubscribeRecipeTags()
        {
            List<SubscribeTagReference> values = null;
            lock (subscribedRecipeTags)
            {
                values = subscribedRecipeTags.ToList();
                subscribedRecipeTags.Clear();

                if (checkSubscribedRecipeTags != null)
                {
                    checkSubscribedRecipeTags.Dispose();
                    checkSubscribedRecipeTags = null;
                }
                pendingRecipeTagExecution = null;
            }            

            if (values != null && values.Count > 0)
            {
                values.ForEach((item) =>
                {
                    item.Dispose();
                });
            }

            ExecutionState &= ~RecipeExecutionStateEnum.TimeoutOnConnectCommandTags;
        }

        void SubscribeDataValues(RecipeExecutionContext context, IDictionary<Guid, SubscribeTagReference> subscribed)
        {
            if (subscribed.Count > 0)
            {
                lock (subscribedDataValues)
                {
                    subscribedDataValues.Add(context, new SubscribeDataValues(subscribed, context.Timeout));
                    subscribedDataValues[context].Subscribe();
                    ExecutionState &= ~RecipeExecutionStateEnum.TimeoutOnConnectValueTags;
                    subscribedDataValues[context].Timeout += (s, e) =>
                    {
                        ResetCommandTag(context);
                        ExecutionState |= RecipeExecutionStateEnum.TimeoutOnConnectValueTags;
                        context.Exception = 
                            new TimeoutException(String.Format(Properties.Resources.ExecutingCommandTimeout, 
                            context.CommandType, RecipeDocument.RecipeEntity.RecipeName));
                    };
                }
            }
        }

        void UnsubscribeDataValues(RecipeExecutionContext context)
        {
            SubscribeDataValues subscribed = null;
            lock (subscribedDataValues)
            {
                if (subscribedDataValues.ContainsKey(context))
                {
                    subscribed = subscribedDataValues[context];
                    subscribedDataValues.Remove(context);
                }
            }

            if (subscribed != null)
            {
                subscribed.Unsubscribe();
                ExecutionState &= ~RecipeExecutionStateEnum.TimeoutOnConnectValueTags;
            }
        }

        void UnsubscribeDataValues()
        {
            List<SubscribeDataValues> subscribeds = null;
            lock (subscribedDataValues)
            {
                subscribeds = subscribedDataValues.Values.ToList();
                subscribedDataValues.Clear();
            }

            if (subscribeds != null && subscribeds.Count > 0)
            {
                foreach (var item in subscribeds)
                    item.Unsubscribe();
            }
        }

        void RenewEntityReferences(UFRecipeEntity recipe)
        {
            var datavalues = recipe.GetFlatDataValuesCollection();
            datavalues.ForEach((datavalue) =>
            {
                datavalue.TagDataValue = new OPCUAEntityReference(datavalue.TagDataValue);
                datavalue.TagIODataValue = new OPCUAEntityReference(datavalue.TagIODataValue);
            });
        }

        void PrepareExecute(RecipeExecutionContext context)
        {
            var action = new RecipeAction(context, () =>
            {
                var subscribed = new Dictionary<Guid, SubscribeTagReference>();
                if (context != null)
                {
                    if (recipeUAConnector != null && String.IsNullOrEmpty(context.Index))
                    {
                        RecipeDocument.RecipeEntity.TagRecipeIndex = new OPCUAEntityReference(RecipeDocument.RecipeEntity.TagRecipeIndex);
                        if (RecipeDocument.RecipeEntity.TagRecipeIndex != null && RecipeDocument.RecipeEntity.TagRecipeIndex.IsValid)
                        {
                            if (!subscribed.ContainsKey(RecipeDocument.RecipeEntity.NodeId))
                                subscribed.Add(RecipeDocument.RecipeEntity.NodeId, new SubscribeTagReference(this, RecipeDocument.RecipeEntity.TagRecipeIndex, new Action<MonitoredItemViewModel, RecipeExecutionContext>(UpdateDataValue), context));
                        }
                    }

                    if (context.CommandType == RecipeCommandType.Save)
                    {
                        RenewEntityReferences(RecipeDocument.RecipeEntity);
                        var datavalues = (from c in RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                                          where c.IsTagReferenceValid()
                                          select c).ToList();

                        if (datavalues.Count > 0)
                        {
                            datavalues.ForEach((datavalue) =>
                            {
                                if (!subscribed.ContainsKey(datavalue.NodeId))
                                    subscribed.Add(datavalue.NodeId, new SubscribeTagReference(this, datavalue.TagDataValue, new Action<MonitoredItemViewModel, RecipeExecutionContext>(UpdateDataValue), context));
                            });
                        }
                    }

                    if (context.CommandType == RecipeCommandType.Import)
                    {
                        if (String.IsNullOrEmpty(context.FilePathName))
                        {
                            logServer.WarnFormat(Properties.Resources.ExecutingCommandWarning, context.CommandType, RecipeDocument.Title);

                            ResetCommandTag(context);
                            return;
                        }

                        try
                        {
                            var index = context.Index;
                            if (String.IsNullOrEmpty(index))
                                index = System.IO.Path.GetFileNameWithoutExtension(context.FilePathName);

                            using (var dataSet = CreateDataSet(false))
                            {
                                DataView viewRecipes = new DataView(dataSet.Tables[DataSetHelper.TableName(RecipeDocument.RecipeEntity)],
                                                                        String.Format("[{0}]='{1}'", DataSetHelper.ColumnName(RecipeDocument.RecipeEntity), index),
                                                                        String.Empty,
                                                                        DataViewRowState.CurrentRows);
                                AddNewRecipe(dataSet, index);

                                if (viewRecipes.Count > 0)
                                {
                                    var guid = Guid.Empty;
                                    var recipeId = viewRecipes[0].Row[DataSetHelper.PrimaryKeyName(RecipeDocument.RecipeEntity)].ToString();
                                    if (Guid.TryParse(recipeId, out guid))
                                    {
                                        using (System.IO.StreamReader readFile = new System.IO.StreamReader(context.FilePathName))
                                        {
                                            var recipeEntity = RecipeDocument.RecipeEntity;
                                            var recipePrimaryKeyName = DataSetHelper.PrimaryKeyName(RecipeDocument.RecipeEntity);
                                            var recipeName = RecipeDocument.RecipeEntity.Name;

                                            readFile.ReadLine();
                                            var line = readFile.ReadLine();
                                            if (!line.Contains(string.Format("{{{0}}}", recipeName)))
                                                throw new InvalidDataException(String.Format(Properties.Resources.ImportCommandMissingRecimeName, recipeName));
                                            readFile.ReadLine();

                                            StringBuilder retColumns = new StringBuilder();
                                            StringBuilder retValues = new StringBuilder();

                                            for (int cc = 0; cc < dataSet.Tables.Count; cc++)
                                            {
                                                DataView dataView = new DataView(dataSet.Tables[cc]);

                                                String rowFilter = null;
                                                rowFilter = String.Format("[{0}]='{1}'", dataView.Table.PrimaryKey[0].ColumnName, guid);

                                                dataView.RowFilter = rowFilter;
                                                dataView.RowStateFilter = DataViewRowState.CurrentRows;

                                                readFile.ReadLine();
                                                line = readFile.ReadLine();
                                                if (!line.Contains(string.Format("{{{0}}}", dataView.Table.TableName)))
                                                    throw new InvalidDataException(String.Format(Properties.Resources.ImportCommandMissingTableName, dataView.Table.TableName));
                                                readFile.ReadLine();

                                                retColumns.Append(readFile.ReadLine());
                                                retValues.Append(readFile.ReadLine());

                                                var columnlist = retColumns.ToString().Split(';');
                                                var valuelist = retValues.ToString().Split(';');
                                                foreach (DataRowView rowView in dataView)
                                                {
                                                    foreach (DataColumn column in rowView.Row.Table.Columns)
                                                    {
                                                        if (column.DataType != typeof(System.DateTime) && column.ColumnName != recipePrimaryKeyName && column.ColumnName != recipeName)
                                                        {
                                                            if (columnlist.Contains(column.ColumnName))
                                                            {
                                                                string v = valuelist.ElementAt(Array.IndexOf(columnlist, column.ColumnName));
                                                                rowView[column.ColumnName] = TypeExtensions.ChangeType(v, column.DataType, force: true);
                                                            }
                                                        }
                                                    }
                                                }

                                                retColumns.Clear();
                                                retValues.Clear();
                                            }
                                        }

                                        var datavalues = RecipeDocument.RecipeEntity.GetFlatDataValuesCollection();
                                        var values = DataSetHelper.GetDBValues(datavalues, dataSet, guid, bEncodingString: false);
                                        for (int ii = 0; ii < datavalues.Count; ii++)
                                            context.Values[datavalues[ii].NodeId] = values[ii];
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            logServer.Error(ex.Message);
                            context.Exception = ex;

                            ResetCommandTag(context);
                        }
                    }
                }

                if (subscribed.Count > 0)
                    SubscribeDataValues(context, subscribed);
                else if (!context.IsAuditTrace)
                    InternalExecute(context);
            });

            try
            {
                StartNewAction(action);
            }
            catch (Exception ex)
            {
                logServer.Error(ex.Message);
                context.Exception = ex;

                ResetCommandTag(context);
            }
        }

        void InternalExecute(RecipeExecutionContext context)
        {
            try
            {
                if (recipeUAConnector != null)
                {
                    var result = recipeUAConnector.Execute(ExecutionMode.Normal, context);
                    if (context.CommandType == RecipeCommandType.Export)
                    {
                        if (result.Count >= 2 && result[0].Value is IList<Uuid> && result[1].Value is IList<Variant>)
                        {
                            var guids = result[0].Value as IList<Uuid>;
                            var values = result[1].Value as IList<Variant>;

                            context.Values.Clear();
                            if (guids.Count == values.Count)
                            {
                                for (int ii = 0; ii < guids.Count; ii++)
                                    context.Values.Add(guids[ii], values[ii]);
                            }
                        }

                        ExportToFile(context);
                    }
                }
                else
                    Execute(RecipeDocument, ExecutionMode.Normal, context);
            }
            catch (Exception ex)
            {
                context.Exception = ex;
                if (!context.IsAuditTrace)
                {
                    logServer.Error(ex.Message);
#if !NET_STANDARD
                    if (UIInterface != null)
                    {
                        var control = context.Control as UIElement;
                        if (control != null)
                            control.Dispatcher.BeginInvokeIfRequired(() => UIInterface.ShowError(ex.Message));
                        else
                            UIInterface.ShowError(ex.Message);
                    }
#endif
                }
            }
            finally
            {
                if (context.IsExecuted != null)
                    context.IsExecuted.Set();
            }
        }

        void UpdateDataValue(MonitoredItemViewModel model, RecipeExecutionContext context)
        {
            if (bDisposed || model == null || !model.IsValid)
                return;

            var action = new RecipeAction(model.monitoredItem.ResolvedNodeId, () =>
            {
                bool bReady = false;
                var subscriptions = new List<SubscribeTagReference>();
                if (RecipeDocument.RecipeEntity.TagRecipeIndex != null &&
                    RecipeDocument.RecipeEntity.TagRecipeIndex.MonitoredItemViewModel == model)
                {
                    lock (subscribedDataValues)
                    {
                        if (subscribedDataValues.ContainsKey(context))
                        {
                            var subscribed = subscribedDataValues[context].Remove(RecipeDocument.RecipeEntity.NodeId);
                            if (subscribed != null)
                                subscriptions.Add(subscribed);
                            bReady = subscribedDataValues[context].Count == 0;
                        }
                    }

                    try
                    {
                        var dataValue = model.monitoredItem.Subscription.Session.ReadValue(model.monitoredItem.ResolvedNodeId);
                        context.Index = String.Format("{0}", dataValue.WrappedValue);
                    }
                    catch
                    {
                        context.Index = model.Value;
                    }
                }
                else
                {
                    var datavalues = (from c in RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                                      where c.TagDataValue != null && c.TagDataValue.MonitoredItemViewModel == model
                                      select c).ToList();

                    if (datavalues.Count > 0)
                    {
                        var dataValue = model.DataValue;
                        try
                        {
                            dataValue = model.monitoredItem.Subscription.Session.ReadValue(model.monitoredItem.ResolvedNodeId);
                        }
                        catch
                        { }

                        lock (subscribedDataValues)
                        {
                            if (subscribedDataValues.ContainsKey(context))
                            {
                                foreach (var datavalue in datavalues)
                                {
                                    var subscribed = subscribedDataValues[context].Remove(datavalue.NodeId);
                                    if (subscribed != null)
                                    {
                                        subscriptions.Add(subscribed);
                                        context.Values.Add(datavalue.NodeId, dataValue.WrappedValue);
                                    }
                                }

                                bReady = subscribedDataValues[context].Count == 0;
                            }
                        }
                    }
                }

                if (subscriptions.Count > 0)
                    subscriptions.ForEach((subscribed) => subscribed.Dispose());

                if (bReady)
                {
                    if (!context.IsAuditTrace)
                        InternalExecute(context);
                    else if (context.IsExecuted != null)
                        context.IsExecuted.Set();
                }
            });

            try
            {
                StartNewAction(action);
            }
            catch (Exception ex)
            {
                logServer.Error(ex.Message);
            }
        }

        void ForceUpdateRecipeTagIndex()
        {
            if (recipeUAConnector == null)
            {
                OPCUAEntityReference entityReference = RecipeDocument.RecipeEntity.TagRecipeIndex;
                if (entityReference != null && entityReference.IsValid)
                {
                    RecipeTagIndexChanged(entityReference.MonitoredItemViewModel);
                }
            }
        }

        void RecipeTagIndexChanged(MonitoredItemViewModel model)
        {
            if (bDisposed || model == null || !model.IsValid)
                return;

            var context = new RecipeExecutionContext()
            {
                CommandType = RecipeCommandType.Load,
                Index = GetRecipeIndex(),
                Timeout = Properties.Settings.Default.defaultSyncTimeout
            };

            Execute(RecipeDocument, ExecutionMode.Normal, context);
        }

        void RecipeTagCommandChanged(MonitoredItemViewModel model)
        {
            if (bDisposed || model == null || !model.IsValid)
                return;

            Double dvalue;
            Boolean bvalue;
            if (Double.TryParse(model.Value, out dvalue) && dvalue > 0.0 || 
                Boolean.TryParse(model.Value, out bvalue) && bvalue == true)
            {
                var context = new RecipeExecutionContext()
                {
                    Index = GetRecipeIndex(),
                    Timeout = Properties.Settings.Default.defaultSyncTimeout
                };

                if (RecipeDocument.RecipeEntity.TagRecipeLoad != null &&
                    RecipeDocument.RecipeEntity.TagRecipeLoad.MonitoredItemViewModel == model)
                {
                    context.CommandType = RecipeCommandType.Load;
                }
                else if (RecipeDocument.RecipeEntity.TagRecipeSave != null &&
                    RecipeDocument.RecipeEntity.TagRecipeSave.MonitoredItemViewModel == model)
                {
                    context.CommandType = RecipeCommandType.Save;
                }
                else if (RecipeDocument.RecipeEntity.TagRecipeDelete != null &&
                    RecipeDocument.RecipeEntity.TagRecipeDelete.MonitoredItemViewModel == model)
                {
                    context.CommandType = RecipeCommandType.Remove;
                }
                else if (RecipeDocument.RecipeEntity.TagRecipeWrite != null &&
                    RecipeDocument.RecipeEntity.TagRecipeWrite.MonitoredItemViewModel == model)
                {
                    context.CommandType = RecipeCommandType.Activate;
                }
                else if (RecipeDocument.RecipeEntity.TagRecipeRead != null &&
                    RecipeDocument.RecipeEntity.TagRecipeRead.MonitoredItemViewModel == model)
                {
                    context.CommandType = RecipeCommandType.Read;
                }

                bool bCanExecute = false;
                bool bIsBusy = false;
                lock (subscribedRecipeTags)
                {
                    if (pendingRecipeTagExecution == null)
                    {
                        bCanExecute = true;
                        pendingRecipeTagExecution = context;
                    }
                    else
                        bIsBusy = pendingRecipeTagExecution.CommandType != context.CommandType;
                }

                if (bCanExecute)
                {
                    PrepareExecute(context);
                }
                else if (bIsBusy)
                {
                    if (context.CommandType == RecipeCommandType.Load)
                    {
                        ExecutionState |= RecipeExecutionStateEnum.ErrorOnLoadingValues;
                    }
                    else if (context.CommandType == RecipeCommandType.Save)
                    {
                        ExecutionState |= RecipeExecutionStateEnum.ErrorOnSavingValues;
                    }
                    else if (context.CommandType == RecipeCommandType.Remove)
                    {
                        ExecutionState |= RecipeExecutionStateEnum.ErrorOnDeletingValues;
                    }
                    else if (context.CommandType == RecipeCommandType.Activate)
                    {
                        ExecutionState |= RecipeExecutionStateEnum.ErrorOnWritingValues;
                    }
                    else if (context.CommandType == RecipeCommandType.Read)
                    {
                        ExecutionState |= RecipeExecutionStateEnum.ErrorOnReadingValues;
                    }
                    else if (context.CommandType == RecipeCommandType.Import)
                    {
                        ExecutionState |= RecipeExecutionStateEnum.ErrorOnImportingValues;
                    }
                    else if (context.CommandType == RecipeCommandType.Export)
                    {
                        ExecutionState |= RecipeExecutionStateEnum.ErrorOnExportingValues;
                    }

                    logServer.Error(String.Format(Properties.Resources.ExecutingCommandBusy, context.CommandType, RecipeDocument.Title));
                    
                    ResetCommandTag(context);
                }
            }
        }

        void ResetCommandTag(RecipeExecutionContext context)
        {
            MonitoredItemViewModel model = null;
            if (context.CommandType == RecipeCommandType.Load && 
                RecipeDocument.RecipeEntity.TagRecipeLoad != null)
            {
                model = RecipeDocument.RecipeEntity.TagRecipeLoad.MonitoredItemViewModel;
            }
            else if (context.CommandType == RecipeCommandType.Save &&
                RecipeDocument.RecipeEntity.TagRecipeSave != null)
            {
                model = RecipeDocument.RecipeEntity.TagRecipeSave.MonitoredItemViewModel;
            }
            else if (context.CommandType == RecipeCommandType.Remove &&
                RecipeDocument.RecipeEntity.TagRecipeDelete != null)
            {
                model = RecipeDocument.RecipeEntity.TagRecipeDelete.MonitoredItemViewModel;
            }
            else if (context.CommandType == RecipeCommandType.Activate &&
                RecipeDocument.RecipeEntity.TagRecipeWrite != null)
            {
                model = RecipeDocument.RecipeEntity.TagRecipeWrite.MonitoredItemViewModel;
            }
            else if (context.CommandType == RecipeCommandType.Read &&
                RecipeDocument.RecipeEntity.TagRecipeRead != null)
            {
                model = RecipeDocument.RecipeEntity.TagRecipeRead.MonitoredItemViewModel;
            }

            try
            {
                if (model != null && model.IsValid)
                    model.WriteValue(0);
            }
            catch (Exception ex)
            {
                logServer.Error(ex.Message);
            }

            UnsubscribeDataValues(context);
            try
            {
                if (context.IsExecuted != null)
                    context.IsExecuted.Set();
            }
            catch
            { }

            lock (subscribedRecipeTags)
            {
                if (pendingRecipeTagExecution == context)
                    pendingRecipeTagExecution = null;
            }
        }

        public string GetRecipeIndex()
        {
            OPCUAEntityReference entityReference = RecipeDocument.RecipeEntity.TagRecipeIndex;
            if (entityReference != null && entityReference.IsValid)
                return GetRecipeIndex(entityReference.MonitoredItemViewModel);

            return null;
        }

        string GetRecipeIndex(MonitoredItemViewModel model)
        {
            if (model == null || !model.IsValid)
                return null;

            String recipeList = GetRecipeList();
            if (model.DataType == "String")
                return model.Value;
            else if (!String.IsNullOrEmpty(recipeList))
            {
                Int32 index;
                if (Int32.TryParse(model.Value, out index))
                {
                    var recipes = recipeList.Split('|');
                    if (recipes.Length > index)
                        return recipes[index];
                }
            }

            return null;
        }

        string GetRecipeList()
        {
            OPCUAEntityReference entityReference = RecipeDocument.RecipeEntity.TagRecipeList;
            if (entityReference != null && entityReference.IsValid)
                return GetRecipeList(entityReference.MonitoredItemViewModel);

            return null;
        }

        string GetRecipeList(MonitoredItemViewModel model)
        {
            if (model == null || !model.IsValid)
                return null;

            return model.Value;
        }

        void LoadRecipe(RecipeExecutionContext context)
        {
            if (String.IsNullOrEmpty(context.Index))
            {
                ExecutionState = RecipeExecutionStateEnum.None;

                logServer.WarnFormat(Properties.Resources.ExecutingCommandWarning, context.CommandType, RecipeDocument.Title);

                ResetCommandTag(context);
                return;
            }

            var action = new RecipeAction(context, () =>
            {
                ExecutionState = RecipeExecutionStateEnum.LoadingValues;

                RenewEntityReferences(RecipeDocument.RecipeEntity);
                var datavalues = (from c in RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                                  where c.IsTagReferenceValid()
                                  select c).ToList();

                if (datavalues.Count > 0)
                {
                    var subscribed = new Dictionary<Guid, SubscribeTagReference>();
                    try
                    {
                        if (loadDataSet != null)
                            loadDataSet.Dispose();
                        loadDataSet = CreateDataSet(true);

                        UnsubscribeDataValues(context);
                        datavalues.ForEach((datavalue) =>
                        {
                            if (ctsPendingTasks != null)
                                ctsPendingTasks.Token.ThrowIfCancellationRequested();

                            if (!subscribed.ContainsKey(datavalue.NodeId))
                            {
                                subscribed.Add(datavalue.NodeId,
                                    new SubscribeTagReference(this, datavalue.TagDataValue, new Action<MonitoredItemViewModel, RecipeExecutionContext>(UpdateRecipeTagValue), context));
                            }
                        });
                        SubscribeDataValues(context, subscribed);
                    }
                    catch (Exception ex)
                    {
                        ExecutionState &= ~RecipeExecutionStateEnum.LoadingValues;
                        ExecutionState |= RecipeExecutionStateEnum.ErrorOnLoadingValues;

                        if (subscribed.Count > 0)
                        {
                            foreach (var subscribedTag in subscribed.Values)
                                subscribedTag.Dispose();
                        }

                        logServer.Error(ex.Message);
                        context.Exception = ex;

                        ResetCommandTag(context);
                    }
                }
                else
                {
                    ExecutionState &= ~RecipeExecutionStateEnum.LoadingValues;

                    ResetCommandTag(context);
                }
            });

            try
            {
                StartNewAction(action);
            }
            catch (Exception ex)
            {
                ExecutionState &= ~RecipeExecutionStateEnum.LoadingValues;
                ExecutionState |= RecipeExecutionStateEnum.ErrorOnLoadingValues;

                logServer.Error(ex.Message);
                context.Exception = ex;

                ResetCommandTag(context);
            }
        }

        void UpdateRecipeTagValue(MonitoredItemViewModel model, RecipeExecutionContext context)
        {
            var recipeIndex = context.Index;
            if (bDisposed || model == null || !model.IsValid || String.IsNullOrEmpty(recipeIndex))
                return;

            bool bNotFound = false;
            var action = new RecipeAction(model.monitoredItem.ResolvedNodeId, () =>
            {
                var datavalues = (from c in RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                                 where c.TagDataValue != null && c.TagDataValue.MonitoredItemViewModel == model
                                 select c).ToList();

                if (datavalues.Count > 0)
                {
                    DataView viewRecipes = null;
                    bool bComplete = false;
                    foreach (var datavalue in datavalues)
                    {
                        try
                        {
                            var dataSet = loadDataSet;
                            if (dataSet == null)
                                throw new InvalidOperationException(String.Format(Properties.Resources.RecipeNotInizializedError, RecipeDocument.RecipeEntity.Name));

                            if (viewRecipes == null)
                            {
                                viewRecipes = new DataView(dataSet.Tables[DataSetHelper.TableName(RecipeDocument.RecipeEntity)],
                                                                    String.Format("[{0}]='{1}'", DataSetHelper.ColumnName(RecipeDocument.RecipeEntity), recipeIndex),
                                                                    String.Empty,
                                                                    DataViewRowState.CurrentRows);

                                if (viewRecipes.Count == 0)
                                    bNotFound = true;
                            }

                            var defaultValue = TypeExtensions.ChangeType(datavalue.DefaultValue, datavalue.DataType.ToNetType(), force: true, datavalue.ArrayDimension);
                            if (viewRecipes.Count > 0)
                            {
                                var guid = Guid.Empty;
                                var recipeId = viewRecipes[0].Row[DataSetHelper.PrimaryKeyName(RecipeDocument.RecipeEntity)].ToString();
                                if (Guid.TryParse(recipeId, out guid))
                                {
                                    var tablename = DataSetHelper.TableName(datavalue);
                                    DataView view = new DataView(dataSet.Tables[tablename]);
                                    view.RowFilter = String.Format("[{0}]='{1}'", view.Table.PrimaryKey[0].ColumnName, guid);

                                    object value = view.Count > 0 ? view[0].Row[DataSetHelper.ColumnName(datavalue)] : defaultValue;
                                    model.WriteValue(value != DBNull.Value ? value : defaultValue);
                                    if (!InitializedDataValues.Contains(datavalue.NodeId))
                                        InitializedDataValues.Add(datavalue.NodeId);
                                }
                            }
                            else if (!InitializedDataValues.Contains(datavalue.NodeId))
                            {
                                model.WriteValue(defaultValue);
                                InitializedDataValues.Add(datavalue.NodeId);
                            }
                        }
                        catch (Exception ex)
                        {
                            ExecutionState |= RecipeExecutionStateEnum.ErrorOnLoadingValues;

                            logServer.Error(ex.Message);
                            context.Exception = ex;
                        }

                        SubscribeTagReference subscribed = null;
                        lock (subscribedDataValues)
                        {
                            if (subscribedDataValues.ContainsKey(context))
                            {
                                subscribed = subscribedDataValues[context].Remove(datavalue.NodeId);
                                bComplete = subscribedDataValues[context].Count == 0;
                            }
                        }
                        if (subscribed != null)
                            subscribed.Dispose();
                    }

                    if (bComplete)
                    {
                        ExecutionState &= ~RecipeExecutionStateEnum.LoadingValues;
                        if (bNotFound)
                            ExecutionState |= RecipeExecutionStateEnum.NotFoundOnLoadingValues;
                        else if ((ExecutionState & RecipeExecutionStateEnum.ErrorOnLoadingValues) != RecipeExecutionStateEnum.ErrorOnLoadingValues)
                            ExecutionState |= RecipeExecutionStateEnum.SuccessfullLoadingValues;

                        ResetCommandTag(context);
                    }
                }
            });

            try
            {
                StartNewAction(action);
            }
            catch (Exception ex)
            {
                ExecutionState &= ~RecipeExecutionStateEnum.LoadingValues;
                ExecutionState |= RecipeExecutionStateEnum.ErrorOnLoadingValues;

                logServer.Error(ex.Message);
                context.Exception = ex;

                ResetCommandTag(context);
            }
        }

        void ForceUpdateRecipeTagList()
        {
            if (recipeUAConnector == null)
            {
                OPCUAEntityReference entityReference = RecipeDocument.RecipeEntity.TagRecipeList;
                if (entityReference != null && entityReference.IsValid)
                    UpdateRecipeTagList(entityReference.MonitoredItemViewModel);
            }
        }

        void UpdateRecipeTagList(MonitoredItemViewModel model)
        {
            if (bDisposed || model == null || !model.IsValid)
                return;

            var action = new RecipeAction(model.monitoredItem.ResolvedNodeId, () =>
            {
                try
                {
                    using (var dataSet = CreateDataSet(true))
                    {
                        var recipes = new List<String>();
                        var table = dataSet.Tables[DataSetHelper.TableName(RecipeDocument.RecipeEntity)];

                        foreach (DataRow row in table.Rows)
                        {
                            if (ctsPendingTasks != null)
                                ctsPendingTasks.Token.ThrowIfCancellationRequested();

                            recipes.Add(String.Format("{0}", 
                                row[DataSetHelper.ColumnName(RecipeDocument.RecipeEntity)]));
                        }

                        recipes.Sort();
                        model.WriteValue(String.Join("|", recipes));
                        ExecutionState &= ~RecipeExecutionStateEnum.ErrorOnUpdatingList;
                    }
                }
                catch (Exception ex)
                {
                    ExecutionState |= RecipeExecutionStateEnum.ErrorOnUpdatingList;

                    logServer.Error(ex.Message);
                }
            });

            try
            {
                StartNewAction(action);
            }
            catch (Exception ex)
            {
                ExecutionState |= RecipeExecutionStateEnum.ErrorOnUpdatingList;

                logServer.Error(ex.Message);
            }
        }

        void UpdateRecipeTagState()
        {
            OPCUAEntityReference entityReference = RecipeDocument.RecipeEntity.TagRecipeState;
            if (entityReference != null && entityReference.IsValid)
                UpdateRecipeTagState(entityReference.MonitoredItemViewModel);
        }

        void UpdateRecipeTagState(MonitoredItemViewModel model)
        {
            if (bDisposed || model == null || !model.IsValid)
                return;

            try
            {
                model.WriteValue(executionState);
                executionState &= ~RecipeExecutionStateEnum.ErrorOnUpdatingState;
            }
            catch (Exception ex)
            {
                executionState |= RecipeExecutionStateEnum.ErrorOnUpdatingState;

                logServer.Error(ex.Message);
            }
        }

        void ReadRecipeTagValue(MonitoredItemViewModel model, RecipeExecutionContext context)
        {
            var recipeIndex = context.Index;
            if (bDisposed || model == null || !model.IsValid || String.IsNullOrEmpty(recipeIndex))
                return;

            var action = new RecipeAction(model.monitoredItem.ResolvedNodeId, () =>
            {
                var datavalues = (from c in RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                                 where c.TagDataValue != null && c.TagDataValue.MonitoredItemViewModel == model
                                 select c).ToList();

                if (datavalues.Count > 0)
                {
                    DataView viewRecipes = null;
                    bool bComplete = false;
                    foreach (var datavalue in datavalues)
                    {
                        try
                        {
                            var dataSet = readDataSet;
                            if (dataSet == null)
                                throw new InvalidOperationException(String.Format(Properties.Resources.RecipeNotInizializedError, RecipeDocument.RecipeEntity.Name));

                            if (viewRecipes == null)
                            {
                                viewRecipes = new DataView(dataSet.Tables[DataSetHelper.TableName(RecipeDocument.RecipeEntity)],
                                                                    String.Format("[{0}]='{1}'", DataSetHelper.ColumnName(RecipeDocument.RecipeEntity), recipeIndex),
                                                                    String.Empty,
                                                                    DataViewRowState.CurrentRows);
                            }

                            var defaultValue = TypeExtensions.ChangeType(datavalue.DefaultValue, datavalue.DataType.ToNetType(), force: true, datavalue.ArrayDimension);
                            if (viewRecipes.Count > 0)
                            {
                                var guid = Guid.Empty;
                                var recipeId = viewRecipes[0].Row[DataSetHelper.PrimaryKeyName(RecipeDocument.RecipeEntity)].ToString();
                                if (Guid.TryParse(recipeId, out guid))
                                {
                                    var tablename = DataSetHelper.TableName(datavalue);
                                    DataView view = new DataView(dataSet.Tables[tablename]);
                                    view.RowFilter = String.Format("[{0}]='{1}'", view.Table.PrimaryKey[0].ColumnName, guid);

                                    object value = view.Count > 0 ? view[0].Row[DataSetHelper.ColumnName(datavalue)] : defaultValue;
                                    model.WriteValue(value != DBNull.Value ? value : defaultValue);
                                }
                            }
                            else
                                model.WriteValue(defaultValue);
                        }
                        catch (Exception ex)
                        {
                            ExecutionState |= RecipeExecutionStateEnum.ErrorOnReadingValues;

                            logServer.Error(ex.Message);
                            context.Exception = ex;
                        }

                        SubscribeTagReference subscribed = null;
                        lock (subscribedDataValues)
                        {
                            if (subscribedDataValues.ContainsKey(context))
                            {
                                subscribed = subscribedDataValues[context].Remove(datavalue.NodeId);
                                bComplete = subscribedDataValues[context].Count == 0;
                            }
                        }
                        if (subscribed != null)
                            subscribed.Dispose();
                    }

                    if (bComplete)
                    {
                        ExecutionState &= ~RecipeExecutionStateEnum.ReadingValues;
                        if ((ExecutionState & RecipeExecutionStateEnum.ErrorOnReadingValues) != RecipeExecutionStateEnum.ErrorOnReadingValues)
                            ExecutionState |= RecipeExecutionStateEnum.SuccessfullReadValues;

                        ResetCommandTag(context);
                    }
                }
            });

            try
            {
                StartNewAction(action);
            }
            catch (Exception ex)
            {
                ExecutionState &= ~RecipeExecutionStateEnum.ReadingValues;
                ExecutionState |= RecipeExecutionStateEnum.ErrorOnReadingValues;

                logServer.Error(ex.Message);
                context.Exception = ex;

                ResetCommandTag(context);
            }
        }

        void ImportRecipeTagValue(MonitoredItemViewModel model, RecipeExecutionContext context)
        {
            var recipeIndex = context.Index;
            if (bDisposed || model == null || !model.IsValid || String.IsNullOrEmpty(recipeIndex))
                return;

            var action = new RecipeAction(model.monitoredItem.ResolvedNodeId, () =>
            {
                var datavalues = (from c in RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                                  where c.TagDataValue != null && c.TagDataValue.MonitoredItemViewModel == model
                                  select c).ToList();

                if (datavalues.Count > 0)
                {
                    bool bComplete = false;
                    foreach (var datavalue in datavalues)
                    {
                        try
                        {
                            var defaultValue = TypeExtensions.ChangeType(datavalue.DefaultValue, datavalue.DataType.ToNetType(), force: true, datavalue.ArrayDimension);
                            if (context.Values != null && context.Values.ContainsKey(datavalue.NodeId))
                            {
                                model.WriteValue(context.Values[datavalue.NodeId].Value);
                            }
                            else
                                model.WriteValue(defaultValue);
                        }
                        catch (Exception ex)
                        {
                            ExecutionState |= RecipeExecutionStateEnum.ErrorOnImportingValues;

                            logServer.Error(ex.Message);
                            context.Exception = ex;
                        }

                        SubscribeTagReference subscribed = null;
                        lock (subscribedDataValues)
                        {
                            if (subscribedDataValues.ContainsKey(context))
                            {
                                subscribed = subscribedDataValues[context].Remove(datavalue.NodeId);
                                bComplete = subscribedDataValues[context].Count == 0;
                            }
                        }
                        if (subscribed != null)
                            subscribed.Dispose();
                    }

                    if (bComplete)
                    {
                        ExecutionState &= ~RecipeExecutionStateEnum.ImportingValues;
                        if ((ExecutionState & RecipeExecutionStateEnum.ErrorOnImportingValues) != RecipeExecutionStateEnum.ErrorOnImportingValues)
                            ExecutionState |= RecipeExecutionStateEnum.SuccessfullImportValues;

                        ResetCommandTag(context);
                    }
                }
            });

            try
            {
                StartNewAction(action);
            }
            catch (Exception ex)
            {
                ExecutionState &= ~RecipeExecutionStateEnum.ImportingValues;
                ExecutionState |= RecipeExecutionStateEnum.ErrorOnImportingValues;

                logServer.Error(ex.Message);
                context.Exception = ex;

                ResetCommandTag(context);
            }
        }

        void CheckAndVerifyDatabaseAsync(bool clear = false, bool logError = false)
        {
            if (taskVerifyDatataBase == null)
            {
                taskVerifyDatataBase = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        CheckAndVerifyDatabase(ctsPendingTasks.Token, clear);
                    }
                    catch (Exception ex)
                    {
                        if (logError)
                            logServer.Error(ex.Message);
                    }
                    finally
                    {
                        taskVerifyDatataBase = null;
                    }
                }, ctsPendingTasks.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
            }
        }

        void CheckConnectionSource()
        {
            dataProvider = RecipeDocument.RecipeEntity.DataProvider;
            connectionString = XpoHelpers.XpoHelper.NormalizeConnectionString(RecipeDocument.RecipeEntity.ConnectionString, RecipeDocument.rootBase);
            if (UfuaEditorService != null)
            {
                if (String.IsNullOrEmpty(dataProvider) || String.IsNullOrEmpty(connectionString))
                {
                    var conn = UfuaEditorService.GetHistorianDefaultConnection(RecipeDocument);
                    conn = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(conn, currentSessionName);
                    dataProvider = XpoConversionHelper.GetDataProviderFromXpoConnection(conn);
                    connectionString = XpoConversionHelper.GetConnectionStringFromXpoConnection(conn);
                }
            }

            lock (recipeCheckingState)
            {
                if (!recipeCheckingState.ContainsKey(RecipeDocument.FullPath))
                    recipeCheckingState.Add(RecipeDocument.FullPath, RecipeCheckingStateEnum.None);
            }
        }

#if !NET_STANDARD
        public void MergeAndUpdateData(DataSet dataSet)
        {
            MergeAndUpdateData(dataSet, TransactionLogEventType.Unknown);
        }

        private void MergeAndUpdateData(DataSet dataSet, TransactionLogEventType eventType)
        {
            using (var ds = CreateDataSet(fill: true))
            {
                for (int cc = 0; cc < ds.Tables.Count; cc++)
                {
                    DataView dataView1 = new DataView(ds.Tables[cc]);
                    DataView dataView2 = new DataView(dataSet.Tables[cc]);

                    foreach (DataRowView rowView1 in dataView1)
                    {
                        dataView2.RowFilter = String.Format("[{0}]='{1}'", dataView2.Table.PrimaryKey[0].ColumnName, rowView1[dataView2.Table.PrimaryKey[0].ColumnName]);
                        if (dataView2.Count == 0)
                            rowView1.Delete();
                    }

                    dataView2.RowFilter = String.Empty;
                    foreach (DataRowView rowView2 in dataView2)
                    {
                        dataView1.RowFilter = String.Format("[{0}]='{1}'", dataView2.Table.PrimaryKey[0].ColumnName, rowView2[dataView2.Table.PrimaryKey[0].ColumnName]);
                        if (dataView1.Count == 0 && rowView2.Row.RowState != DataRowState.Added)
                            rowView2.Row.SetAdded();
                        else
                        {
                            foreach (DataColumn column in dataView1.Table.Columns)
                            {
                                if (dataView1.Count > 0 && rowView2[column.ColumnName] != null)
                                {
                                    var v1 = Convert.ToString(dataView1[0].Row[column.ColumnName], System.Globalization.CultureInfo.InvariantCulture);
                                    var v2 = Convert.ToString(rowView2[column.ColumnName], System.Globalization.CultureInfo.InvariantCulture);
                                    if (v1 != v2)
                                    {
                                        if (rowView2.Row.RowState != DataRowState.Added)
                                        {
                                            rowView2.Row.SetModified();
                                            break;
                                        }
                                    }
                                }

                            }
                        }
                    }
                }

                ds.Merge(dataSet);
                if (eventType != TransactionLogEventType.Unknown)
                {
                    UpdateDatabase(ds, eventType);
                    if (transactionLogger != null)
                        transactionLogger.UpdateTransactionLog(ds, RecipeDocument, eventType);
                }
                else
                    AcceptUpdateData(ds, false, true); //Case when recipes data are updated copying the database
            }
        }

        private void UpdateDatabase(DataSet dataSet, TransactionLogEventType eventType)
        {
            using (var writer = new DataWriter.DataSetWriter(dataProvider, connectionString))
            {
                for (int cc = 0; cc < dataSet.Tables.Count; cc++)
                {
                    DataView dataView = new DataView(dataSet.Tables[cc]);

                    String rowFilter = null;
                    dataView.RowFilter = rowFilter ?? String.Empty;

                    try
                    {
                        if (eventType == TransactionLogEventType.Deleted)
                            writer.DeleteRows(dataView);
                        else if (eventType == TransactionLogEventType.Added)
                        {
                            dataView.RowStateFilter = DataViewRowState.Added;
                            writer.InsertRows(dataView);
                        }
                        else if (eventType == TransactionLogEventType.Modified)
                        {
                            dataView.RowStateFilter = DataViewRowState.ModifiedCurrent;
                            writer.UpdateRows(dataView);

                        }
                        foreach (DataRowView rowView in dataView)
                            rowView.Row.AcceptChanges();
                    }
                    catch (Exception e)
                    {
                        Logger.WriteToEventLog(Utilities.Properties.Resources.RecipeService, String.Format(
                            Utilities.Properties.Resources.RedundancyUpdateDBError, e.Message), EventLogEntryType.Error);
                    }
                }
            }
        }
#endif
        #endregion

        #region Recipe Commands Manager
#if !NET_STANDARD
        void ShowRecipe(IDocument parent, WaitCursor cursor = null)
        {
            try
            {
                using (var dataSet = CreateDataSet(true))
                {
                    using (var layoutExecuter = new LayoutRecipeExecuter(this, dataSet, parent, true))
                    {
                        layoutExecuter.PrepareExecution();
                        if (cursor != null)
                            cursor.Release();

                        layoutExecuter.LayoutControl.ExecutedCommand += (s, e) =>
                        {
                            if (e.CommandType == EditCommandType.Reload)
                            {
                                try
                                {
                                    dataSet.Clear();
                                    FillDataSet(dataSet, e.CancellationToken);
                                }
                                catch (Exception ex)
                                {
                                    e.exception = ex;
                                }
                            }
                        };

                        Window curwnd = null;
                        if (System.Windows.Input.Keyboard.FocusedElement is DependencyObject)
                            curwnd = Window.GetWindow(System.Windows.Input.Keyboard.FocusedElement as DependencyObject);

                        var title = String.IsNullOrEmpty(RecipeDocument.RecipeEntity.Description) ? RecipeDocument.RecipeEntity.Name : RecipeDocument.RecipeEntity.Description;
                        if (StringEditorManager != null)
                        {
                            var map = StringEditorManager.GetListStringForCulture(RecipeDocument, StringEditorManager.GetActiveCulture(RecipeDocument));
                            if (map != null && map.Count > 0 && map.ContainsKey(title))
                                title = map[title];
                        }

                        var dialog = new GeneralDialogContent(layoutExecuter.LayoutControl, GeneralDialogButtons.OkCancelButtons)
                        {
                            Owner = curwnd,
                            Title = title,
                            HelpLink = "ShowRecipe"
                        };
                        WPFUtilities.ThemeHelper.SetTheme(layoutExecuter.LayoutControl, parent.Theme);
                        bool bSave = false;
                        if (dialog.ShowDialog() == true)
                            bSave = dataSet.HasChanges();

                        if (cursor != null)
                            cursor.Aquire();

                        if (bSave)
                        {
                            AcceptUpdateData(dataSet);
                            ForceUpdateRecipeTags();
                        }
                        else
                            RejectUpdateData(dataSet);
                    }
                }
            }
            catch (Exception ex)
            {
                if (UIInterface != null)
                    UIInterface.ShowError(ex.Message);
            }
        }
#endif

        void WriteRecipe(RecipeExecutionContext context)
        {
            if (String.IsNullOrEmpty(context.Index))
            {
                ExecutionState = RecipeExecutionStateEnum.ErrorOnWritingValues;

                logServer.WarnFormat(Properties.Resources.ExecutingCommandWarning, context.CommandType, RecipeDocument.Title);

                ResetCommandTag(context);
                return;
            }

            var action = new RecipeAction(context, () =>
            {
                ExecutionState = RecipeExecutionStateEnum.WritingValues;

                RenewEntityReferences(RecipeDocument.RecipeEntity);
                try
                {
                    if (writeDataSet != null)
                        writeDataSet.Dispose();
                    writeDataSet = CreateDataSet(true);
                    var dataSet = writeDataSet;
                    {
                        ExecutionResult result = new ExecutionResult() { ResultState = true };

                        var writer = recipeWriter;
                        if (writer != null)
                        {
                            if (!writer.CanExecute(Properties.Settings.Default.defaultSyncTimeout))
                                throw new TimeoutException(Properties.Resources.TimeoutWritingData);

                            DataView viewRecipes = new DataView(dataSet.Tables[DataSetHelper.TableName(RecipeDocument.RecipeEntity)],
                                                                String.Format("[{0}]='{1}'", DataSetHelper.ColumnName(RecipeDocument.RecipeEntity), context.Index),
                                                                String.Empty,
                                                                DataViewRowState.CurrentRows);
                            if (viewRecipes.Count > 0)
                            {
                                var guid = Guid.Empty;
                                var recipeId = viewRecipes[0].Row[DataSetHelper.PrimaryKeyName(RecipeDocument.RecipeEntity)].ToString();
                                if (Guid.TryParse(recipeId, out guid))
                                {
                                    for (int cc = 0; cc < dataSet.Tables.Count; cc++)
                                    {
                                        if (dataSet.Tables[cc].DefaultView.RowStateFilter != DataViewRowState.CurrentRows)
                                            dataSet.Tables[cc].DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
                                        dataSet.Tables[cc].DefaultView.RowFilter = String.Empty;
                                        dataSet.Tables[cc].DefaultView.RowFilter = String.Format("[{0}]='{1}'",
                                                                                    dataSet.Tables[cc].PrimaryKey[0].ColumnName,
                                                                                    guid);
                                    }

                                    VariantCollection values = new VariantCollection();
                                    if (RecipeDocument.RecipeEntity.IsWritable())
                                    {
                                        // first write the datavalues without group with starting address
                                        var writabledatavalues = (from c in RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                                                                  where c.IsWritable() && String.IsNullOrEmpty(c.StartingAddress)
                                                                  select c).ToList();

                                        if (writabledatavalues.Count > 0)
                                        {
                                            values = DataSetHelper.GetDBValues(writabledatavalues, dataSet, guid);
                                            result = writer.Execute(RecipeDocument.RecipeEntity.StartingAddress, values.ToArray(), context.Timeout);
                                        }
                                    }

                                    if (result.ResultState)
                                    {
                                        // next write the datavalues inside group with starting address
                                        var validgroups = (from c in RecipeDocument.RecipeEntity.Groups
                                                           where c.IsWritable()
                                                           select c).ToList();

                                        foreach (var recipegroup in validgroups)
                                        {
                                            var writabledatavalues = (from c in recipegroup.DataValues
                                                                      where c.UseInCommunication && String.IsNullOrEmpty(c.StartingAddress)
                                                                      orderby c.OID ascending
                                                                      select c).ToList();

                                            if (writabledatavalues.Count > 0)
                                            {
                                                values = DataSetHelper.GetDBValues(writabledatavalues, dataSet, guid);
                                                result = writer.Execute(recipegroup.StartingAddress, values.ToArray(), context.Timeout);
                                            }

                                            if (!result.ResultState)
                                                break;
                                        }
                                    }

                                    if (result.ResultState)
                                    {
                                        // end write the datavalues with starting address
                                        var writabledatavalues = (from c in RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                                                                  where c.IsWritable() && !String.IsNullOrEmpty(c.StartingAddress)
                                                                  select c).ToList();

                                        List<UFDataValueEntity> list = new List<UFDataValueEntity>();
                                        foreach (var datavalue in writabledatavalues)
                                        {
                                            list.Add(datavalue);
                                            values = DataSetHelper.GetDBValues(list, dataSet, guid);
                                            result = writer.Execute(datavalue.StartingAddress, values.ToArray(), context.Timeout);

                                            if (!result.ResultState)
                                                break;

                                            list.Clear();
                                        }
                                    }

                                    if (result.ResultState)
                                    {
                                        var writabledatavalues = (from c in RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                                                                  where c.IsTagIOReferenceValid()
                                                                  select c).ToList();
                                        if (writabledatavalues.Count > 0)
                                        {
                                            values = DataSetHelper.GetDBValues(writabledatavalues, dataSet, guid, false);
                                            result = writer.Execute(values.ToArray());
                                        }

                                        UpdateActivationTime(dataSet, guid, context.UserName, context.UserComment);

                                        ExecutionState &= ~RecipeExecutionStateEnum.WritingValues;
                                        ExecutionState |= RecipeExecutionStateEnum.SuccessfullWriteValues;

                                        ResetCommandTag(context);
                                    }
                                    else
                                    {
                                        ExecutionState &= ~RecipeExecutionStateEnum.WritingValues;
                                        ExecutionState |= RecipeExecutionStateEnum.ErrorOnWritingValues;
                                        context.Exception = result.ExceptionInfo ?? new TimeoutException(String.Format(Properties.Resources.ExecutingCommandTimeout,
                                        context.CommandType, RecipeDocument.RecipeEntity.RecipeName));

                                        ResetCommandTag(context);
                                    }
                                }
                            }
                            else
                            {
                                ExecutionState &= ~RecipeExecutionStateEnum.WritingValues;
                                ExecutionState |= RecipeExecutionStateEnum.ErrorOnWritingValues;
                                context.Exception = result.ExceptionInfo ?? new InvalidOperationException(String.Format(Properties.Resources.ExecutingCommandInvalidIndex,
                                context.CommandType, RecipeDocument.RecipeEntity.RecipeName, context.Index));

                                ResetCommandTag(context);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExecutionState &= ~RecipeExecutionStateEnum.WritingValues;
                    ExecutionState |= RecipeExecutionStateEnum.ErrorOnWritingValues;

                    logServer.Error(ex.Message);
                    context.Exception = ex;

                    ResetCommandTag(context);
                }
            });

            try
            {
                StartNewAction(action);
            }
            catch (Exception ex)
            {
                ExecutionState &= ~RecipeExecutionStateEnum.WritingValues;
                ExecutionState |= RecipeExecutionStateEnum.ErrorOnWritingValues;

                logServer.Error(ex.Message);
                context.Exception = ex;

                ResetCommandTag(context);
            }
        }

        VariantCollection GetWritableValues(IList<UFDataValueEntity> writabledatavalues, IDictionary<Guid, Variant> values, bool bEncodingString = true)
        {
            VariantCollection ret = new VariantCollection();
            foreach (var datavalue in writabledatavalues)
            {
                object value = datavalue.DefaultValue;
                if (values != null && values.ContainsKey(datavalue.NodeId))
                    value = values[datavalue.NodeId].Value;
                if (bEncodingString && datavalue.ArrayDimension == 0 && datavalue.DataType == UFUAModel.DataType.String)
                {
                    if (datavalue.BytesStringValueSize > 0)
                    {
                        // special case for String who should be encoding from String to String
                        var sval = value as String;
                        if (sval == null)
                            sval = string.Empty;
                        ret.Add(new Variant(DataSetHelper.EncodingStringToBytesValue(sval, datavalue.Encoding, datavalue.BytesStringValueSize)));
                    }
                }
                else
                    ret.Add(new Variant(DataTypeExtensions.ChangeType(value, datavalue.DataType, (int)datavalue.ArrayDimension, true)));
            }

            return ret;
        }

        void ReadRecipe(RecipeExecutionContext context)
        {
            if (String.IsNullOrEmpty(context.Index))
            {
                ExecutionState = RecipeExecutionStateEnum.ErrorOnReadingValues;

                logServer.WarnFormat(Properties.Resources.ExecutingCommandWarning, context.CommandType, RecipeDocument.Title);

                ResetCommandTag(context);
                return;
            }

            var action = new RecipeAction(context, () =>
            {
                ExecutionState = RecipeExecutionStateEnum.ReadingValues;

                RenewEntityReferences(RecipeDocument.RecipeEntity);
                var writabledatavalues = (from c in RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                                          where c.IsTagReferenceValid()
                                          select c).ToList();

                ExecutionResult result = new ExecutionResult() { ResultState = true };

                if (writabledatavalues.Count > 0)
                {
                    try
                    {
                        if (readDataSet != null)
                            readDataSet.Dispose();
                        readDataSet = CreateDataSet(true);
                        var dataSet = readDataSet;

                        var reader = recipeReader;
                        if (reader != null)
                        {
                            if (!reader.CanExecute(Properties.Settings.Default.defaultSyncTimeout))
                                throw new TimeoutException(Properties.Resources.TimeoutReadingData);

                            DataView viewRecipes = new DataView(dataSet.Tables[DataSetHelper.TableName(RecipeDocument.RecipeEntity)],
                                                                String.Format("[{0}]='{1}'", DataSetHelper.ColumnName(RecipeDocument.RecipeEntity), context.Index),
                                                                String.Empty,
                                                                DataViewRowState.CurrentRows);

                            if (viewRecipes.Count == 0)
                            {
                                AddNewRecipe(dataSet, context.Index);
                            }

                            var guid = Guid.Empty;
                            var recipeId = viewRecipes[0].Row[DataSetHelper.PrimaryKeyName(RecipeDocument.RecipeEntity)].ToString();
                            if (Guid.TryParse(recipeId, out guid))
                            {
                                for (int cc = 0; cc < dataSet.Tables.Count; cc++)
                                {
                                    if (dataSet.Tables[cc].DefaultView.RowStateFilter != DataViewRowState.CurrentRows)
                                        dataSet.Tables[cc].DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
                                    dataSet.Tables[cc].DefaultView.RowFilter = String.Empty;
                                    dataSet.Tables[cc].DefaultView.RowFilter = String.Format("[{0}]='{1}'",
                                                                                dataSet.Tables[cc].PrimaryKey[0].ColumnName,
                                                                                guid);
                                }

                                VariantCollection values = new VariantCollection();
                                if (RecipeDocument.RecipeEntity.IsReadable())
                                {
                                    // first write the datavalues without group with starting address
                                    var readableedatavalues = (from c in RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                                                               where c.IsReadable() && String.IsNullOrEmpty(c.StartingAddress)
                                                               select c).ToList();

                                    if (readableedatavalues.Count > 0)
                                    {
                                        values = DataSetHelper.GetDBValues(readableedatavalues, dataSet, guid);
                                        result = reader.Execute(RecipeDocument.RecipeEntity.StartingAddress, values.ToArray(), context.Timeout);
                                        if (result.ResultState)
                                            DataSetHelper.SetDBValues(readableedatavalues, dataSet, result.OutputValues);
                                    }
                                }

                                if (result.ResultState)
                                {
                                    // next write the datavalues inside group with starting address
                                    var validgroups = (from c in RecipeDocument.RecipeEntity.Groups
                                                       where c.IsReadable()
                                                       select c).ToList();

                                    foreach (var recipegroup in validgroups)
                                    {
                                        var readableedatavalues = (from c in recipegroup.DataValues
                                                                   where c.UseInCommunication && String.IsNullOrEmpty(c.StartingAddress)
                                                                   orderby c.OID ascending
                                                                   select c).ToList();

                                        if (readableedatavalues.Count > 0)
                                        {
                                            values = DataSetHelper.GetDBValues(readableedatavalues, dataSet, guid);
                                            result = reader.Execute(recipegroup.StartingAddress, values.ToArray(), context.Timeout);
                                        }

                                        if (!result.ResultState)
                                            break;

                                        DataSetHelper.SetDBValues(readableedatavalues, dataSet, result.OutputValues);
                                    }
                                }

                                if (result.ResultState)
                                {
                                    // end write the datavalues with starting address
                                    var readableedatavalues = (from c in RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                                                               where c.IsReadable() && !String.IsNullOrEmpty(c.StartingAddress)
                                                               select c).ToList();

                                    List<UFDataValueEntity> list = new List<UFDataValueEntity>();
                                    foreach (var datavalue in readableedatavalues)
                                    {
                                        list.Add(datavalue);
                                        values = DataSetHelper.GetDBValues(list, dataSet, guid);
                                        result = reader.Execute(datavalue.StartingAddress, values.ToArray(), context.Timeout);

                                        if (!result.ResultState)
                                            break;

                                        DataSetHelper.SetDBValues(list, dataSet, result.OutputValues);
                                        list.Clear();
                                    }
                                }

                                if (result.ResultState)
                                {
                                    var readabledatavalues = (from c in RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                                                              where c.IsTagIOReferenceValid()
                                                              select c).ToList();
                                    if (readabledatavalues.Count > 0)
                                    {
                                        values = DataSetHelper.GetDBValues(readabledatavalues, dataSet, guid, false);
                                        result = reader.Execute(values.ToArray());
                                        if (result.ResultState)
                                            DataSetHelper.SetDBValues(readabledatavalues, dataSet, result.OutputValues, false);
                                    }
                                }

                                if (result.ResultState)
                                {
                                    var subscribed = new Dictionary<Guid, SubscribeTagReference>();
                                    UnsubscribeDataValues(context);
                                    writabledatavalues.ForEach((datavalue) =>
                                    {
                                        if (!subscribed.ContainsKey(datavalue.NodeId))
                                        {
                                            subscribed.Add(datavalue.NodeId,
                                                new SubscribeTagReference(this, datavalue.TagDataValue, new Action<MonitoredItemViewModel, RecipeExecutionContext>(ReadRecipeTagValue), context));
                                        }
                                    });
                                    SubscribeDataValues(context, subscribed);
                                }
                                else
                                {
                                    ExecutionState &= ~RecipeExecutionStateEnum.ReadingValues;
                                    ExecutionState |= RecipeExecutionStateEnum.ErrorOnReadingValues;
                                    context.Exception = result.ExceptionInfo ?? new TimeoutException(String.Format(Properties.Resources.ExecutingCommandTimeout,
                                        context.CommandType, RecipeDocument.RecipeEntity.RecipeName));

                                    ResetCommandTag(context);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ExecutionState &= ~RecipeExecutionStateEnum.ReadingValues;
                        ExecutionState |= RecipeExecutionStateEnum.ErrorOnReadingValues;

                        logServer.Error(ex.Message);
                        context.Exception = ex;

                        ResetCommandTag(context);
                    }
                }
                else
                {
                    ExecutionState &= ~RecipeExecutionStateEnum.ReadingValues;

                    ResetCommandTag(context);
                }
            });

            try
            {
                StartNewAction(action);
            }
            catch (Exception ex)
            {
                ExecutionState &= ~RecipeExecutionStateEnum.ReadingValues;
                ExecutionState |= RecipeExecutionStateEnum.ErrorOnReadingValues;

                logServer.Error(ex.Message);
                context.Exception = ex;

                ResetCommandTag(context);
            }
        }

        void SaveRecipe(RecipeExecutionContext context)
        {
            if (String.IsNullOrEmpty(context.Index) || context.Values == null || context.Values.Keys.Count == 0)
            {
                ExecutionState = RecipeExecutionStateEnum.ErrorOnSavingValues;

                logServer.WarnFormat(Properties.Resources.ExecutingCommandWarning, context.CommandType, RecipeDocument.Title);

                ResetCommandTag(context);
                return;
            }

            var action = new RecipeAction(context, () =>
            {
                ExecutionState = RecipeExecutionStateEnum.SavingValues;

                try
                {
                    using (var dataSet = CreateDataSet(true))
                    {
                        DataView viewRecipes = new DataView(dataSet.Tables[DataSetHelper.TableName(RecipeDocument.RecipeEntity)],
                                                                String.Format("[{0}]='{1}'", DataSetHelper.ColumnName(RecipeDocument.RecipeEntity), context.Index),
                                                                String.Empty,
                                                                DataViewRowState.CurrentRows);
                        if (viewRecipes.Count == 0)
                        {
                            AddNewRecipe(dataSet, context.Index);
                        }

                        if (viewRecipes.Count > 0)
                        {
                            var guid = Guid.Empty;
                            var recipeId = viewRecipes[0].Row[DataSetHelper.PrimaryKeyName(RecipeDocument.RecipeEntity)].ToString();
                            if (Guid.TryParse(recipeId, out guid))
                            {
                                if (viewRecipes[0].Row.RowState == DataRowState.Unchanged)
                                    viewRecipes[0].Row.SetModified();

                                foreach (var key in context.Values.Keys)
                                {
                                    var datavalue = (from c in RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                                                     where c.NodeId == key
                                                     select c).FirstOrDefault();
                                    if (datavalue != null)
                                    {
                                        var defaultValue = TypeExtensions.ChangeType(datavalue.DefaultValue, datavalue.DataType.ToNetType(), force: true, datavalue.ArrayDimension);
                                        var value = context.Values[key];
                                        var tablename = DataSetHelper.TableName(datavalue);
                                        DataView view = new DataView(dataSet.Tables[tablename]);
                                        view.RowFilter = String.Format("[{0}]='{1}'", view.Table.PrimaryKey[0].ColumnName, guid);
                                        if (datavalue.ArrayDimension > 0)
                                            view[0].Row[DataSetHelper.ColumnName(datavalue)] = value.ToString(null, System.Globalization.CultureInfo.InvariantCulture);
                                        else
                                            view[0].Row[DataSetHelper.ColumnName(datavalue)] = value.Value != null ? value.Value : datavalue.AllowNull ? DBNull.Value : defaultValue;
                                    }
                                }

                                AcceptUpdateData(dataSet, guid, context.UserName, context.UserComment);
                                
                            }
                        }
                    }

                    ExecutionState &= ~RecipeExecutionStateEnum.SavingValues;
                    ExecutionState |= RecipeExecutionStateEnum.SuccessfullSaveValues;

                    ForceUpdateRecipeTagList();
                }
                catch (Exception ex)
                {
                    ExecutionState &= ~RecipeExecutionStateEnum.SavingValues;
                    ExecutionState |= RecipeExecutionStateEnum.ErrorOnSavingValues;

                    logServer.Error(ex.Message);
                    context.Exception = ex;
                }

                ResetCommandTag(context);
            });

            try
            {
                StartNewAction(action);
            }
            catch (Exception ex)
            {
                ExecutionState &= ~RecipeExecutionStateEnum.SavingValues;
                ExecutionState |= RecipeExecutionStateEnum.ErrorOnSavingValues;

                logServer.Error(ex.Message);
                context.Exception = ex;

                ResetCommandTag(context);
            }
        }

        void DeleteRecipe(RecipeExecutionContext context)
        {
            if (String.IsNullOrEmpty(context.Index))
            {
                ExecutionState = RecipeExecutionStateEnum.ErrorOnDeletingValues;

                logServer.WarnFormat(Properties.Resources.ExecutingCommandWarning, context.CommandType, RecipeDocument.Title);

                ResetCommandTag(context);
                return;
            }

            var action = new RecipeAction(context, () =>
            {
                ExecutionState = RecipeExecutionStateEnum.DeletingValues;

                try
                {
                    using (var dataSet = CreateDataSet(true))
                    {
                        DataView viewRecipes = new DataView(dataSet.Tables[DataSetHelper.TableName(RecipeDocument.RecipeEntity)],
                                                            String.Format("[{0}]='{1}'", DataSetHelper.ColumnName(RecipeDocument.RecipeEntity), context.Index),
                                                            String.Empty,
                                                            DataViewRowState.CurrentRows);
                        if (viewRecipes.Count > 0)
                        {
                            var guid = Guid.Empty;
                            var recipeId = viewRecipes[0].Row[DataSetHelper.PrimaryKeyName(RecipeDocument.RecipeEntity)].ToString();
                            if (Guid.TryParse(recipeId, out guid))
                            {
                                var tablename = DataSetHelper.TableName(RecipeDocument.RecipeEntity);
                                DataView view = new DataView(dataSet.Tables[tablename]);
                                view.RowFilter = String.Format("[{0}]='{1}'", view.Table.PrimaryKey[0].ColumnName, guid);
                                view[0].Delete();

                                AcceptUpdateData(dataSet, guid, context.UserName, context.UserComment);

                                ForceUpdateRecipeTagList();
                            }
                        }
                    }

                    ExecutionState &= ~RecipeExecutionStateEnum.DeletingValues;
                    ExecutionState |= RecipeExecutionStateEnum.SuccessfullDeleteValues;
                }
                catch (Exception ex)
                {
                    ExecutionState |= RecipeExecutionStateEnum.ErrorOnDeletingValues;

                    logServer.Error(ex.Message);
                    context.Exception = ex;
                }

                ResetCommandTag(context);
            });

            try
            {
                StartNewAction(action);
            }
            catch (Exception ex)
            {
                ExecutionState &= ~RecipeExecutionStateEnum.DeletingValues;
                ExecutionState |= RecipeExecutionStateEnum.ErrorOnDeletingValues;

                logServer.Error(ex.Message);
                context.Exception = ex;

                ResetCommandTag(context);
            }
        }

        void AddNewRecipe(DataSet dataSet, string recipeIndex)
        {
            Guid newguid = Guid.NewGuid();
            DataRow parentRow = null;
            for (int cc = 0; cc < dataSet.Tables.Count; cc++)
            {
                DataView view = new DataView(dataSet.Tables[cc]);
                DataRowView rowView = view.AddNew();
                rowView.BeginEdit();
                if (dataSet.Tables[cc].TableName == DataSetHelper.TableName(RecipeDocument.RecipeEntity))
                {
                    parentRow = rowView.Row;
                    rowView.Row[dataSet.Tables[cc].Columns[DataSetHelper.ColumnName(RecipeDocument.RecipeEntity)]] = recipeIndex;
                    rowView.Row[dataSet.Tables[cc].Columns[DataSetHelper.CreationDateTimeColumnName(RecipeDocument.RecipeEntity)]] = DateTime.UtcNow;
                }
                else if (parentRow != null)
                    rowView.Row.SetParentRow(parentRow);
                rowView.Row[dataSet.Tables[cc].PrimaryKey[0]] = newguid;

                // copy column values
                for (int ii = 0; ii < rowView.Row.Table.Columns.Count; ii++)
                {
                    if (dataSet.Tables[cc].DefaultView.Count == 0 ||
                        rowView.Row.Table.PrimaryKey.Contains(rowView.Row.Table.Columns[ii]) ||
                        (rowView.Row.Table.TableName == DataSetHelper.TableName(RecipeDocument.RecipeEntity) &&
                        rowView.Row.Table.Columns[ii].ColumnName == DataSetHelper.ColumnName(RecipeDocument.RecipeEntity)) ||
                        rowView.Row.Table.Columns[ii].ColumnName == DataSetHelper.CreationDateTimeColumnName(RecipeDocument.RecipeEntity) ||
                        rowView.Row.Table.Columns[ii].ColumnName == DataSetHelper.ActivationDateTimeColumnName(RecipeDocument.RecipeEntity))
                        continue;

                    rowView.Row[ii] = dataSet.Tables[cc].DefaultView[0].Row[ii];
                }

                rowView.EndEdit();
            }
        }
        void ExportRecipe(RecipeExecutionContext context)
        {
            if (String.IsNullOrEmpty(context.Index))
            {
                ExecutionState = RecipeExecutionStateEnum.ErrorOnExportingValues;

                logServer.WarnFormat(Properties.Resources.ExecutingCommandWarning, context.CommandType, RecipeDocument.Title);

                ResetCommandTag(context);
                return;
            }

            var action = new RecipeAction(context, () =>
            {
                ExecutionState = RecipeExecutionStateEnum.ExportingValues;

                try
                {
                    using (var dataSet = CreateDataSet(true))
                    {
                        DataView viewRecipes = new DataView(dataSet.Tables[DataSetHelper.TableName(RecipeDocument.RecipeEntity)],
                                                                String.Format("[{0}]='{1}'", DataSetHelper.ColumnName(RecipeDocument.RecipeEntity), context.Index),
                                                                String.Empty,
                                                                DataViewRowState.CurrentRows);
                        if (viewRecipes.Count == 0)
                            throw new InvalidOperationException(String.Format(Properties.Resources.ExecutingCommandInvalidIndex,
                                context.CommandType, RecipeDocument.RecipeEntity.RecipeName, context.Index));

                        var guid = Guid.Empty;
                        var recipeId = viewRecipes[0].Row[DataSetHelper.PrimaryKeyName(RecipeDocument.RecipeEntity)].ToString();
                        if (Guid.TryParse(recipeId, out guid))
                        {
                            var datavalues = RecipeDocument.RecipeEntity.GetFlatDataValuesCollection();
                            var values = DataSetHelper.GetDBValues(datavalues, dataSet, guid, bEncodingString: false);
                            for (int ii = 0; ii < datavalues.Count; ii++)
                                context.Values[datavalues[ii].NodeId] = values[ii];
                        }
                    }

                    ExecutionState &= ~RecipeExecutionStateEnum.ExportingValues;
                    ExecutionState |= RecipeExecutionStateEnum.SuccessfullExportValues;
                }
                catch (Exception ex)
                {
                    ExecutionState &= ~RecipeExecutionStateEnum.ExportingValues;
                    ExecutionState |= RecipeExecutionStateEnum.ErrorOnExportingValues;

                    logServer.Error(ex.Message);
                    context.Exception = ex;
                }

                ResetCommandTag(context);
            });

            try
            {
                StartNewAction(action);
            }
            catch (Exception ex)
            {
                ExecutionState &= ~RecipeExecutionStateEnum.ExportingValues;
                ExecutionState |= RecipeExecutionStateEnum.ErrorOnExportingValues;

                logServer.Error(ex.Message);
                context.Exception = ex;

                ResetCommandTag(context);
            }
        }

        void ExportToFile(RecipeExecutionContext context)
        {
            if (String.IsNullOrEmpty(context.Index) || String.IsNullOrEmpty(context.FilePathName) || context.Values == null || context.Values.Keys.Count == 0)
            {
                logServer.WarnFormat(Properties.Resources.ExecutingCommandWarning, context.CommandType, RecipeDocument.Title);

                ResetCommandTag(context);
                return;
            }

            try
            {
                using (var dataSet = CreateDataSet(false))
                {
                    DataView viewRecipes = new DataView(dataSet.Tables[DataSetHelper.TableName(RecipeDocument.RecipeEntity)],
                                                            String.Format("[{0}]='{1}'", DataSetHelper.ColumnName(RecipeDocument.RecipeEntity), context.Index),
                                                            String.Empty,
                                                            DataViewRowState.CurrentRows);

                    AddNewRecipe(dataSet, context.Index);

                    var datavalues = RecipeDocument.RecipeEntity.GetFlatDataValuesCollection();
                    var values = GetWritableValues(datavalues, context.Values, bEncodingString: false);
                    DataSetHelper.SetDBValues(datavalues, dataSet, values, bEncodingString: false);

                    var guid = Guid.Empty;
                    var recipeId = viewRecipes[0].Row[DataSetHelper.PrimaryKeyName(RecipeDocument.RecipeEntity)].ToString();
                    if (Guid.TryParse(recipeId, out guid))
                    {
                        using (System.IO.StreamWriter writeFile = new System.IO.StreamWriter(context.FilePathName))
                        {
                            var recipeEntity = RecipeDocument.RecipeEntity;
                            var recipePrimaryKeyName = DataSetHelper.PrimaryKeyName(RecipeDocument.RecipeEntity);
                            var recipeName = RecipeDocument.RecipeEntity.Name;

                            writeFile.WriteLine("--------------------------------------------");
                            writeFile.WriteLine(string.Format(Properties.Resources.ExportRecipeHeader, recipeName));
                            writeFile.WriteLine(("--------------------------------------------"));

                            StringBuilder retColumns = new StringBuilder();
                            StringBuilder retValues = new StringBuilder();

                            for (int cc = 0; cc < dataSet.Tables.Count; cc++)
                            {
                                DataView dataView = new DataView(dataSet.Tables[cc]);

                                String rowFilter = null;
                                rowFilter = String.Format("[{0}]='{1}'", dataView.Table.PrimaryKey[0].ColumnName, guid);

                                dataView.RowFilter = rowFilter;
                                dataView.RowStateFilter = DataViewRowState.CurrentRows;

                                writeFile.WriteLine();
                                writeFile.WriteLine(string.Format(Properties.Resources.ExportGroupHeader, dataView.Table.TableName));
                                writeFile.WriteLine();

                                foreach (DataRowView rowView in dataView)
                                {
                                    foreach (DataColumn column in rowView.Row.Table.Columns)
                                    {
                                        if (column.DataType != typeof(System.DateTime) && column.ColumnName != recipePrimaryKeyName && column.ColumnName != recipeName)
                                        {
                                            if (retColumns.Length > 0)
                                                retColumns.Append(';');
                                            retColumns.Append(column.ColumnName);
                                            string v = Convert.ToString(rowView[column.ColumnName], System.Globalization.CultureInfo.InvariantCulture);
                                            if (retValues.Length > 0)
                                                retValues.Append(';');
                                            retValues.Append(v);
                                        }
                                    }
                                }

                                writeFile.WriteLine(retColumns.ToString());
                                writeFile.WriteLine(retValues.ToString());

                                retColumns.Clear();
                                retValues.Clear();
                            }

                            writeFile.WriteLine();
                            writeFile.WriteLine("--------------------------------------------");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logServer.Error(ex.Message);
                context.Exception = ex;
            }
        }

        void ImportRecipe(RecipeExecutionContext context)
        {
            if (String.IsNullOrEmpty(context.Index) || context.Values == null || context.Values.Keys.Count == 0)
            {
                ExecutionState = RecipeExecutionStateEnum.ErrorOnImportingValues;

                logServer.WarnFormat(Properties.Resources.ExecutingCommandWarning, context.CommandType, RecipeDocument.Title);

                ResetCommandTag(context);
                return;
            }

            var action = new RecipeAction(context, () =>
            {
                ExecutionState = RecipeExecutionStateEnum.ImportingValues;

                RenewEntityReferences(RecipeDocument.RecipeEntity);
                var writabledatavalues = (from c in RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                                          where c.IsTagReferenceValid()
                                          select c).ToList();

                if (writabledatavalues.Count > 0)
                {
                    try
                    {
                        var subscribed = new Dictionary<Guid, SubscribeTagReference>();
                        UnsubscribeDataValues(context);
                        writabledatavalues.ForEach((datavalue) =>
                        {
                            if (!subscribed.ContainsKey(datavalue.NodeId))
                            {
                                subscribed.Add(datavalue.NodeId,
                                    new SubscribeTagReference(this, datavalue.TagDataValue, new Action<MonitoredItemViewModel, RecipeExecutionContext>(ImportRecipeTagValue), context));
                            }
                        });
                        SubscribeDataValues(context, subscribed);

                    }
                    catch (Exception ex)
                    {
                        ExecutionState &= ~RecipeExecutionStateEnum.ImportingValues;
                        ExecutionState |= RecipeExecutionStateEnum.ErrorOnImportingValues;

                        logServer.Error(ex.Message);
                        context.Exception = ex;

                        ResetCommandTag(context);
                    }
                }
                else
                {
                    ExecutionState &= ~RecipeExecutionStateEnum.ImportingValues;

                    ResetCommandTag(context);
                }
            });

            try
            {
                StartNewAction(action);
            }
            catch (Exception ex)
            {
                ExecutionState &= ~RecipeExecutionStateEnum.ImportingValues;
                ExecutionState |= RecipeExecutionStateEnum.ErrorOnImportingValues;

                logServer.Error(ex.Message);
                context.Exception = ex;

                ResetCommandTag(context);
            }
        }

        #endregion

        #region Dataset

        DataTable CreateDataTable(object obj)
        {
            // add recipe index table
            DataTable table = new DataTable(DataSetHelper.TableName(obj));

            // add primary key
            DataColumn[] keys = new DataColumn[1];

            // add column
            var column = new DataColumn()
            {
                ColumnName = DataSetHelper.PrimaryKeyName(obj),
                DataType = DataSetHelper.PrimaryKeyType(obj),
                AllowDBNull = false,
                Unique = true,
                ColumnMapping = MappingType.Element
            };

            table.Columns.Add(column);
            keys[0] = column;
            table.PrimaryKey = keys;

            if (obj is UFRecipeEntity)
            {
                // add recipe index column
                table.Columns.Add(new DataColumn()
                {
                    ColumnName = DataSetHelper.ColumnName(obj as UFRecipeEntity),
                    DataType = typeof(String),
                    AllowDBNull = false,
                    Unique = true,
                    MaxLength = (obj as UFRecipeEntity).MaxLength,
                    ColumnMapping = MappingType.Element
                });

                // add recipe informations
                table.Columns.Add(new DataColumn()
                {
                    ColumnName = DataSetHelper.CreationDateTimeColumnName(obj as UFRecipeEntity),
                    DataType = typeof(DateTime),
                    AllowDBNull = true,
                    Unique = false,
                    ColumnMapping = MappingType.Element
                });

                table.Columns.Add(new DataColumn()
                {
                    ColumnName = DataSetHelper.ActivationDateTimeColumnName(obj as UFRecipeEntity),
                    DataType = typeof(DateTime),
                    AllowDBNull = true,
                    Unique = false,
                    ColumnMapping = MappingType.Element
                });
            }

            // add data value columns
            var datavalues = new List<UFDataValueEntity>();
            if (obj is UFRecipeEntity)
                datavalues.AddRange((obj as UFRecipeEntity).DataValues);
            else if (obj is UFGroupEntity)
                datavalues.AddRange((obj as UFGroupEntity).DataValues);
            foreach (var value in datavalues)
            {
                var bAllowDBNull = value.ArrayDimension == 0 ? value.AllowNull : false;
                table.Columns.Add(new DataColumn()
                {
                    ColumnName = value.DataValueName,
                    DataType = value.ArrayDimension == 0 ? value.DataType.ToNetType() : typeof(String),
                    MaxLength = value.ArrayDimension == 0 && value.DataType.IsVariableLenght() && value.UFRecipeAss != null ? value.UFRecipeAss.MaxLength : -1,
                    AllowDBNull = bAllowDBNull,
                    DefaultValue = TypeExtensions.ChangeType(value.DefaultValue, value.DataType.ToNetType(), force: !bAllowDBNull, value.ArrayDimension),
                    ColumnMapping = MappingType.Element,
                    Caption = value.Description
                });
            }

            return table;
        }

#endregion Dataset

#region IDisposable

        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            TerminateExecution();

            TerminatePendingTasks();

            ctsPendingTasks.Dispose();

            if (writeDataSet != null)
                writeDataSet.Dispose();
            writeDataSet = null;

            if (readDataSet != null)
                readDataSet.Dispose();
            readDataSet = null;

            if (loadDataSet != null)
                loadDataSet.Dispose();
            loadDataSet = null;
#if !NET_STANDARD
            if (transactionLogger != null)
                transactionLogger.Dispose();
            transactionLogger = null;
#endif
        }

#endregion
    }
}
