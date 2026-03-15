using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlarmWindow
{
    internal class ConditionStateCompositeCollection : CompositeCollection<ConditionStateViewModel>
    {
        public ConditionStateCompositeCollection(object syncLock) : base(syncLock)
        {
        }

        protected override void AddItems(IEnumerable<ConditionStateViewModel> items)
        {
            try
            {
                Semaphore.Wait();
                foreach (var me in items.ToList())
                    lock (me.lockObject)
                        Add(me);
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
            }
            finally
            {
                Semaphore.Release();
            }
        }

        protected override void RemoveItems(IEnumerable<ConditionStateViewModel> items)
        {
            try
            {
                Semaphore.Wait();
                foreach (var me in items.ToList())
                    lock (me.lockObject)
                        Remove(me);
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
            }
            finally
            {
                Semaphore.Release();
            }
        }
    }
}