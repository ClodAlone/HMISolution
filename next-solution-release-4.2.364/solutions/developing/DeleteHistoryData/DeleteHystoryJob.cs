using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeleteHistoryData
{
    internal abstract class DeleteHystoryJob
    {
        #region Declarations
        readonly string settings;
        readonly TimeSpan maxAge;
        readonly int maxTake;
        #endregion

        #region Constructors
        protected DeleteHystoryJob(string settings, TimeSpan maxAge, int maxTake)
        {
            this.settings = settings;
            this.maxAge = maxAge;
            this.maxTake = maxTake;
        }
        #endregion

        #region Abstract Methods
        protected abstract int Execute(string settings, DateTime maxDateTime, int maxTake);
        #endregion

        #region Methods
        public void Execute()
        {
            try
            {
                var maxDateTime = AdjustDateTime(DateTime.UtcNow - maxAge);

                while (true)
                {
#if DEBUG
                    var watcher = new System.Diagnostics.Stopwatch();
                    watcher.Start();
#endif
                    var deletedRecords = Execute(settings, maxDateTime, maxTake);
                    
                    if (deletedRecords > 0)
                    {
                        Console.WriteLine(deletedRecords);
#if DEBUG
                        watcher.Stop();
                        System.Diagnostics.Debug.WriteLine("Deleted Records = {0}, Elapsed Time = {1}", deletedRecords, watcher.Elapsed);
#endif
                    }

                    if (deletedRecords == 0 || deletedRecords < maxTake)
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        DateTime AdjustDateTime(DateTime dateTime)
        {
            if (dateTime < System.Data.SqlTypes.SqlDateTime.MinValue.Value)
                dateTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
            else if (dateTime > System.Data.SqlTypes.SqlDateTime.MaxValue.Value)
                dateTime = System.Data.SqlTypes.SqlDateTime.MaxValue.Value;

            return dateTime;
        }
        #endregion
    }
}
