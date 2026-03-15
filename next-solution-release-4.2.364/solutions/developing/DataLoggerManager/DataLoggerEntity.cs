using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLoggerManager
{
    /// <summary>
    /// object to store a single record to add to database's table.
    /// </summary>
    public class DataLoggerEntity : ICloneable
    {
        #region Public Data
        public string dataLoggerName;
        public DateTime recordingTime;
        public DataLoggerModel.DataLoggerRecordingType recordingType;
        public string userName;
        public IList<DataColumEntity> listDataColumnEntities;
        #endregion

        #region Constructors
        public DataLoggerEntity()
        { }

        public DataLoggerEntity(DataLoggerEntity entity) 
            : this()
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            this.dataLoggerName = entity.dataLoggerName;
            this.recordingTime = entity.recordingTime;
            this.recordingType = entity.recordingType;
            this.userName = entity.userName;

            if (entity.listDataColumnEntities != null)
            {
                listDataColumnEntities = new List<DataColumEntity>(entity.listDataColumnEntities.Count);
                foreach (var column in entity.listDataColumnEntities)
                    listDataColumnEntities.Add((DataColumEntity)column.Clone());
            }
        }
        #endregion

        #region Pulic Methods
        public DataLoggerEntity CreateSnapshot()
        {
            return Clone() as DataLoggerEntity;
        }
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Retreive the reason string linked to recording type enum value.
        /// </summary>
        /// <param name="type">
        /// The recording type enum value to use for retreive the string.
        /// </param>
        /// <returns></returns>
        public static string GetReason(DataLoggerModel.DataLoggerRecordingType type)
        {
            switch (type)
            {
                case DataLoggerModel.DataLoggerRecordingType.OnChange: return Properties.Resources.OnChangeReasonDesc;
                case DataLoggerModel.DataLoggerRecordingType.OnCommand: return Properties.Resources.OnCommandReasonDesc;
                default: return Properties.Resources.OnTimeReasonDesc;
            }
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return String.Format("DataLogger = {0}, RecordingTime = {1}", dataLoggerName, recordingTime);
        }
        #endregion

        #region ICloneable
        public object Clone()
        {
            return new DataLoggerEntity(this);
        }
        #endregion
    }
}
