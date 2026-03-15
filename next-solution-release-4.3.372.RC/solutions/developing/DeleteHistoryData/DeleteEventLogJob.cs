using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFUAHistorianModel.Helpers;
using XpoHelpers;

namespace DeleteHistoryData
{
    internal class DeleteEventLogJob : DeleteHystoryJob
    {
        #region Constructors
        /// <summary>
        /// Initialize a new instance of DeleteEventLogJob class.
        /// </summary>
        /// <param name="settings">
        /// DevExpress.Xpo connection string.
        /// </param>
        /// <param name="maxAge">
        /// Max age of data.
        /// </param>
        /// <param name="maxTake">
        /// Max number of records to load before delete it.
        /// </param>
        public DeleteEventLogJob(string settings, TimeSpan maxAge, int maxTake) :
            base(settings, maxAge, maxTake)
        { }
        #endregion

        #region Ovverides
        protected override int Execute(string settings, DateTime maxDateTime, int maxTake)
        {
            using (var idlDel = XpoDefault.GetDataLayer(settings, DevExpress.Xpo.DB.AutoCreateOption.SchemaAlreadyExists))
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

                        if (XpoHelper.IsMSSQlDataProvider(settings))
                        {
                            var query = String.Format("DELETE TOP ({0}) FROM UFUAAuditLogItem WHERE EventDateTimeUtc < {{ ts '{1}' }}",
                                maxTake, maxDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture));
                            return ufwDel.ExecuteNonQuery(query);
                        }
                        else if (XpoHelper.IsMySQlDataProvider(settings))
                        {
                            var query = String.Format("DELETE QUICK FROM UFUAAuditLogItem WHERE EventDateTimeUtc < {{ ts '{1}' }} LIMIT {0}",
                                maxTake, maxDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture));
                            return ufwDel.ExecuteNonQuery(query);
                        }
                        else if (XpoHelper.IsOracleSQlDataProvider(settings))
                        {
                            var query = String.Format("DELETE FROM UFUAAuditLogItem WHERE EventDateTimeUtc < {{ ts '{1}' }} AND ROWNUM <= {0}",
                                maxTake, maxDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture));
                            return ufwDel.ExecuteNonQuery(query);
                        }
                        else
                        {
                            /* I didn't use 'AsParallel()' in the LinQ expression because 
                                        * it produced a no optimized query in the database, like this:
                                        * 
                                        * [select N0."OID",N0."NodeId",N0."EventId",N0."EventDateTime",
                                        * N0."EventDateTimeUtc",N0."SourceNode",N0."EventSequence",
                                        * N0."EventOccurence",N0."EventCode",N0."EventDetailCode",N0."Message",
                                        * N0."Details",N0."UserName",N0."OptimisticLockField" from "dbo"."UFUAAuditLogItem" N0]
                                        * 
                                        */
                            var entriesToDelete = (from entry in new XPQuery<UFUAHistorianModel.UFUAAuditLogItem>(ufwDel)/*.AsParallel()*/
                                                    where (entry.EventDateTimeUtc < maxDateTime || entry.EventDateTimeUtc == null)
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
