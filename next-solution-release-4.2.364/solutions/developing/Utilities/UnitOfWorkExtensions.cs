using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;
using log4net;
using DevExpress.Xpo.Helpers;

namespace Utilities
{
    public static class UnitOfWorkExtensions
    {
        public static void CommitChangesAndFreeMemory(this UnitOfWork uow)
        {
            uow.CommitChanges();
            uow.PurgeDeletedObjects();
            uow.DropIdentityMap();
        }

        public static void CommitChangesAndDropIdentityMap(this UnitOfWork uow)
        {
            uow.CommitChanges();
            uow.DropIdentityMap();
        }

        public static bool TryCommitChanges(this UnitOfWork uow)
        {
            return TryCommitChanges(uow, null);
        }

        public static bool TryCommitChanges(this UnitOfWork uow, ILog log)
        {
            try
            {
                uow.CommitChanges();
            }
            catch (Exception ex)
            {
                if (log != null)
                    log.Debug(ex.Message, ex);
                return false;
            }

            return true;
        }

        public static int PurgeDeletedObjects(this UnitOfWork uow)
        {
            uow.ReloadChangedObjects();
            var purgeResult = uow.PurgeDeletedObjects();
            return purgeResult.Purged;
        }

        public static int TryPurgeDeletedObjects(this UnitOfWork uow, ILog log = null)
        {
            try
            {
                return PurgeDeletedObjects(uow);
            }
            catch (Exception ex)
            {
                if (log != null)
                    log.Warn(Properties.Resources.PurgeDeletedObjectsFailed);
            }

            return 0;
        }
    }
}
