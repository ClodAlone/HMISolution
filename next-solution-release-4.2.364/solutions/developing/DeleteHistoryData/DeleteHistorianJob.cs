using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UFUAHistorianModel.Helpers;
using XpoHelpers;

namespace DeleteHistoryData
{
    internal class DeleteHistorianJob : DeleteHystoryJob
    {
        #region Declarations
        readonly string nodeId;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialize a new instance of DeleteHistorianJob class.
        /// </summary>
        /// <param name="settings">
        /// DevExpress.Xpo connection string.
        /// </param>
        /// <param name="maxAge">
        /// Max age of data for current NodeId.
        /// </param>
        /// <param name="maxTake">
        /// Max number of records to load before delete it.
        /// </param>
        /// <param name="nodeId">
        /// The NodeId of the record tag to delete.
        /// </param>
        public DeleteHistorianJob(string settings, TimeSpan maxAge, int maxTake, string nodeId) :
            base(settings, maxAge, maxTake)
        {
            this.nodeId = nodeId;
        }
        #endregion

        #region Ovverides
        protected override int Execute(string settings, DateTime maxDateTime, int maxTake)
        {
            using (var idlDel = HistorianHelper.CreateSimpleDataLayer<UFUAHistorianModel.UFUAAuditDataItem>(settings, DevExpress.Xpo.DB.AutoCreateOption.SchemaAlreadyExists))
            {
                using (var ufwDel = new UnitOfWork(idlDel))
                {
                    try
                    {
                        ufwDel.LockingOption = LockingOption.None;
                        try
                        {
                            ufwDel.ExplicitBeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
                        }
                        catch
                        {
                            ufwDel.ExplicitBeginTransaction();
                        }

                        var info = HistorianHelper.GetAuditDataLogInfo(nodeId, ufwDel);
                        if (info == null)
                            return 0;

                        var tableName = HistorianHelper.GetTableName<UFUAHistorianModel.UFUAAuditDataItem>(settings);
                        if (String.IsNullOrEmpty(tableName))
                            tableName = "UFUAAuditDataItem";

                        if (XpoHelper.IsMSSQlDataProvider(settings))
                        {
                            var query = String.Format("DELETE TOP ({0}) FROM {3} WHERE DataLogRef = {1} AND RecordDateTimeUtc < {{ ts '{2}' }}",
                                maxTake, info.Oid, maxDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture), tableName);
                            return ufwDel.ExecuteNonQuery(query);
                        }
                        else if (XpoHelper.IsMySQlDataProvider(settings))
                        {
                            var query = String.Format("DELETE QUICK FROM {3} WHERE DataLogRef = {1} AND RecordDateTimeUtc < {{ ts '{2}' }} LIMIT {0}",
                                maxTake, info.Oid, maxDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture), tableName);
                            return ufwDel.ExecuteNonQuery(query);
                        }
                        else if (XpoHelper.IsOracleSQlDataProvider(settings))
                        {
                            var query = String.Format("DELETE FROM {3} WHERE DataLogRef = {1} AND RecordDateTimeUtc < {{ ts '{2}' }} AND ROWNUM <= {0}",
                                maxTake, info.Oid, maxDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture), tableName);
                            return ufwDel.ExecuteNonQuery(query);
                        }
                        else
                        {
                            /* I didn't use 'AsParallel()' in the LinQ expression because 
                            * it produced a no optimized query in the database, like this:
                            * 
                            * [select N0."OID",N0."Name",N0."NodeId",N0."HistorianName",N0."Value",
                            * N0."Status",N0."RecordDateTime",N0."SourceTimeStamp",N0."SourcePicoseconds",
                            * N0."ServerTimeStamp",N0."ServerPicoseconds",N0."UserName",N0."Reason",
                            * N0."ModificationTime",N0."ModificationType",N0."OptimisticLockField" from "dbo"."UFUAAuditDataItem" N0]
                            * 
                            */
                            var entriesToDelete = (from entry in new XPQuery<UFUAHistorianModel.UFUAAuditDataItem>(ufwDel)/*.AsParallel()*/
                                                   where entry.DataLogRef == info.Oid &&
                                                   (entry.RecordDateTimeUtc < maxDateTime || entry.RecordDateTimeUtc == null)
                                                   select entry).Take(maxTake).ToList();

                            ufwDel.Delete(entriesToDelete);
                            ufwDel.CommitChanges();
                            return entriesToDelete.Count;
                        }
                    }
                    finally
                    {
                        ufwDel.ExplicitCommitTransaction();
                    }
                }
            }
        }
        #endregion
    }
}
