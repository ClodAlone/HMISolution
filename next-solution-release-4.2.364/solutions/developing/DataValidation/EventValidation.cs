using DataReader.Helpers;
using DevExpress.Xpo;
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
    public class EventValidation : BaseValidation
    {
        #region Declarations
        static readonly string[] skipShowColumns;
        static readonly ObservableCollection<Column> columns;
        #endregion

        #region Constructors
        static EventValidation()
        {
            columns = new ObservableCollection<Column>();
            skipShowColumns = new string[]
            {
                "RedundancySyncTime",
                "OptimisticLockField",
            };
            FillColumnsList();
        }

        public EventValidation(string name, string invariantName, string defaultConnection) :
           base(name, invariantName, defaultConnection)
        { }

        public EventValidation(string name, string defaultConnection) :
            base(name, defaultConnection)
        { }
        #endregion

        #region Overrides
        protected override bool InternalValidate(DateTime startDateTime, DateTime endDateTime, bool bContinueOnError)
        {
            bool validationResult = true;
            using (var dl = UFUAHistorianModel.Helpers.HistorianHelper.CreateSimpleDataLayer<UFUAAuditLogItem>(defaultConnection))
            {
                using (var uow = new UnitOfWork(dl))
                {
                    var dataItems = GetData(startDateTime, endDateTime, uow);
                    totalRows = dataItems.Count;

                    for (int ii = 0; ii < dataItems.Count; ii++)
                    {
                        if (Token != null)
                            Token.ThrowIfCancellationRequested();

                        OnProcessingRow(ii + 1);

                        var oid = dataItems[ii].Oid;
                        bool isValid = CheckIsValid(oid);
                        AddRow(dataItems[ii], isValid);

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
                return XpoConversionHelper.GetConnectionStringFromXpoConnection(defaultConnection);
            }
        }

        protected override String TableName
        {
            get
            {
                return "UFUAAuditLogItem";
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
        List<UFUAAuditLogItem> GetData(DateTime startDateTime, DateTime endDateTime, UnitOfWork uow)
        {
            startDateTime = AdjustDateTime(startDateTime);
            endDateTime = AdjustDateTime(endDateTime);

            return (from entry in new XPQuery<UFUAAuditLogItem>(uow)
                    where entry.EventDateTimeUtc >= startDateTime &&
                    entry.EventDateTimeUtc <= endDateTime
                    orderby entry.Oid ascending select entry).ToList();
        }

        static void FillColumnsList()
        {
            var dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
            var tables = dict.GetDataStoreSchema(typeof(UFUAAuditLogItem));
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
                }
            }
        }

        void AddRow(UFUAAuditLogItem item, bool isValid)
        {
            var expandoObject = new Dictionary<string, object>();
            foreach (var column in Columns)
            {
                if (column.FieldName == "OID")
                    expandoObject.Add(column.FieldName, item.Oid);
                else
                    expandoObject.Add(column.FieldName, item.GetMemberValue(column.FieldName));
            }

            Rows.Add(new DynamicRow(expandoObject, isValid));
        }
        #endregion
    }
}
