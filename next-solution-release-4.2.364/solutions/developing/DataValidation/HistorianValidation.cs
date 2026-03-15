using DataReader.Helpers;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFUAHistorianModel;
using Utilities;

namespace DataValidation
{
    public class HistorianValidation : BaseValidation
    {
        #region Declarations
        static readonly string[] skipShowColumns;
        static readonly ObservableCollection<Column> columns;

        #endregion

        #region Constructors
        static HistorianValidation()
        {
            columns = new ObservableCollection<Column>();
            skipShowColumns = new string[]
            {
                "ModificationTime",
                "ModificationType",
                "RedundancySyncTime",
                "DataLogRef",
                "OptimisticLockField",
            };
            FillColumnsList();
        }

        public HistorianValidation(string name, string invariantName, string defaultConnection) :
            base(name, invariantName, defaultConnection)
        { }

        public HistorianValidation(string name, string defaultConnection) :
            base(name, defaultConnection)
        { }
        #endregion

        #region Overrides
        protected override bool InternalValidate(DateTime startDateTime, DateTime endDateTime, bool bContinueOnError)
        {
            bool validationResult = true;
            using (var dl = UFUAHistorianModel.Helpers.HistorianHelper.CreateSimpleDataLayer<UFUAAuditDataItem>(defaultConnection))
            {
                using (var uow = new UnitOfWork(dl))
                {
                    var mapDescriptions = GetMapDescriptions(uow);
                    var dataItems = GetData(startDateTime, endDateTime, uow);
                    totalRows = dataItems.Count;

                    for (int ii = 0; ii < dataItems.Count; ii++)
                    {
                        if (Token != null)
                            Token.ThrowIfCancellationRequested();

                        OnProcessingRow(ii + 1);

                        var oid = dataItems[ii].Oid;
                        bool isValid = CheckIsValid(oid);
                        string description = null;
                        if (mapDescriptions != null && 
                            mapDescriptions.ContainsKey(dataItems[ii].DataLogRef))
                            description = mapDescriptions[dataItems[ii].DataLogRef];
                        AddRow(dataItems[ii], isValid, description);

                        if (!isValid)
                        {
                            validationResult = false;
                            if (!bContinueOnError)
                                break;
                        }
                    }
                }
            }

            return validationResult;
        }

        protected override void ClearAllCounters()
        {
            base.ClearAllCounters();

            Rows.Clear();
        }

        public override ObservableCollection<Column> Columns
        {
            get
            {
                return columns;
            }
        }

        ObservableCollection<DynamicRow> rows;
        public override ObservableCollection<DynamicRow> Rows
        {
            get
            {
                if (rows == null)
                    rows = new SafeObservableCollection<DynamicRow>();
                return rows;
            }
        }

        protected override string ProviderName
        {
            get
            {
                return XpoConversionHelper.GetDataProviderFromXpoConnection(defaultConnection);
            }
        }


        protected override String ConnectionString
        {
            get
            {
                var conn = UFUAHistorianModel.Helpers.HistorianHelper.RemoveTableName(defaultConnection);
                return XpoConversionHelper.GetConnectionStringFromXpoConnection(conn);
            }
        }

        protected override String TableName
        {
            get
            {
                var tableName = UFUAHistorianModel.Helpers.HistorianHelper.GetTableName<UFUAAuditDataItem>(defaultConnection);
                if (!String.IsNullOrEmpty(tableName))
                    return tableName;
                else
                    return "UFUAAuditDataItem";
            }
        }

        protected override String AutoIncrementColumnName
        {
            get
            {
                return "OID";
            }
        }
        #endregion

        #region Methods
        List<UFUAAuditDataItem> GetData(DateTime startDateTime, DateTime endDateTime, UnitOfWork uow)
        {
            startDateTime = AdjustDateTime(startDateTime);
            endDateTime = AdjustDateTime(endDateTime);

            return (from entry in new XPQuery<UFUAAuditDataItem>(uow)
                    where entry.RecordDateTimeUtc >= startDateTime &&
                    entry.RecordDateTimeUtc <= endDateTime
                    orderby entry.Oid ascending select entry).ToList();
        }

        List<UFUAAuditDataLog> GetDataInfo(UnitOfWork uow)
        {
            return (from entry in new XPQuery<UFUAAuditDataLog>(uow)
                    select entry).ToList();
        }

        Dictionary<int, String> GetMapDescriptions(UnitOfWork uow)
        {
            return (from entry in new XPQuery<UFUAAuditDataLog>(uow)
                    select entry).ToDictionary(entry => entry.Oid, entry => entry.Description);
        }

        static void FillColumnsList()
        {
            var dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
            var tables = dict.GetDataStoreSchema(typeof(UFUAAuditDataItem));
            if (tables.Length > 0)
            {
                foreach (var column in tables[0].Columns)
                {
                    if (skipShowColumns.Contains(column.Name))
                        continue;

                    columns.Add(new Column()
                    {
                        HeaderName = column.Name,
                        FieldName = column.Name
                    });

                    if (column.Name == "Name")
                    {
                        columns.Add(new Column()
                        {
                            HeaderName = Properties.Resources.HistorianDescriptionHeaderName,
                            FieldName = Properties.Settings.Default.HistorianDescriptionColumnName,
                        });
                    }
                }
            }
        }

        void AddRow(UFUAAuditDataItem item, bool isValid, string description = null)
        {
            var expandoObject = new  Dictionary<string, object>();
            foreach (var column in Columns)
            {
                if (column.FieldName == "OID")
                    expandoObject.Add(column.FieldName, item.Oid);
                else if (column.FieldName == Properties.Settings.Default.HistorianDescriptionColumnName)
                    expandoObject.Add(column.FieldName, description ?? String.Empty);
                else
                    expandoObject.Add(column.FieldName, item.GetMemberValue(column.FieldName));
            }

            Rows.Add(new DynamicRow(expandoObject, isValid));
        }
        #endregion
    }
}
