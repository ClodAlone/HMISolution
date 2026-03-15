using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XpoHelpers
{
    public class CachedUnitOfWork : IDisposable
    {
        #region Declarations
        readonly IDataLayer dataLayer;
        #endregion

        #region Events
        public EventHandler InUseEvent;
        void SetInUse()
        {
            lastTimeUsed = DateTime.UtcNow;

            var temp = InUseEvent;
            if (temp != null)
                temp(this, new EventArgs());
        }

        public EventHandler NotInUseEvent;
        void SetNotInUse()
        {
            if (!KeepUnitOfWork && unitOfWork != null)
            {
                unitOfWork.Dispose();
                unitOfWork = null;
            }

            var temp = NotInUseEvent;
            if (temp != null)
                temp(this, new EventArgs());
        }
        #endregion

        #region Constructors
        public CachedUnitOfWork(IDataLayer dataLayer)
        {
            this.dataLayer = dataLayer;
            this.owner = Thread.CurrentThread;

            RenewUnitOfWork();
        }
        #endregion

        #region Methods
        public void RenewUnitOfWork()
        {
            if (Interlocked.Increment(ref inUse) == 1)
                SetInUse();
        }

        public void CleanUnitOfWork()
        {
            if (dataLayer != null)
                dataLayer.Dispose();

            if (unitOfWork != null)
                unitOfWork.Dispose();
        }
        #endregion

        #region Properties
        long inUse;
        public bool InUse
        {
            get
            {
                return Interlocked.Read(ref inUse) > 0;
            }
        }

        bool keepUnitOfWork;
        public bool KeepUnitOfWork
        {
            get
            {
                return keepUnitOfWork;
            }
            set
            {
                if (keepUnitOfWork == value)
                    return;
                keepUnitOfWork = value;
            }
        }

        DateTime lastTimeUsed;
        public DateTime LastTimeUsed
        {
            get
            {
                return lastTimeUsed;
            }
        }

        Thread owner;
        public Thread Owner
        {
            get
            {
                return owner;
            }
        }

        UnitOfWork unitOfWork;
        public UnitOfWork UnitOfWork
        {
            get
            {
                if (unitOfWork == null)
                    unitOfWork = new UnitOfWork(dataLayer);

                return unitOfWork;
            }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            if (Interlocked.Decrement(ref inUse) == 0)
                SetNotInUse();
        }
        #endregion
    }
}
