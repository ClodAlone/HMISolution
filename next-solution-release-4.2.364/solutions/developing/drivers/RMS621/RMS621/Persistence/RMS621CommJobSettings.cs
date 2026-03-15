using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace RMS621
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class RMS621CommJobSettings : CommJobSettings
    {
        #region Constructors

        public RMS621CommJobSettings(Session session, RMS621CommJob job)
            : base(session, job)
        {
            _Command = job.Command;
            _Process = job.Process;
            _ProcessNumber = job.ProcessNumber;
        }

        public RMS621CommJobSettings(Session session)
            : base(session)
        {
        }

        public RMS621CommJobSettings()
        {
        }

        #endregion

        public void DefaultSettings()
        {
            _Command = RMS621Protocol.Command.Read;
            _Process = RMS621Protocol.Process.None;
            _ProcessNumber = 1;
        }

        #region Properties

        /// <summary>
        /// Command Code
        /// </summary>
        private byte _CommandCode;
        public byte CommandCode
        {
            get
            {
                return _CommandCode;
            }

            set
            {
                _CommandCode = value;
            }
        }

        private RMS621Protocol.Command _Command;
        public RMS621Protocol.Command Command
        {
            get { return _Command; }
            set
            {
                _Command = value;
            }
        }

        private RMS621Protocol.Process _Process;
        public RMS621Protocol.Process Process
        {
            get { return _Process; }
            set
            {
                _Process = value;
            }
        }

        private uint _ProcessNumber;
        public uint ProcessNumber
        {
            get { return _ProcessNumber; }
            set
            {
                _ProcessNumber = value;
            }
        }

        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            return null;
        }

        #endregion

    }
}
