using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Opc.Ua;
using Utilities.Logger;

namespace UFUAServerBase.SystemTags
{
    public class SystemTagsMember<T> : BaseDataVariableState<T>
    {
        #region Declaration
        Timer samplingTimer;
        bool updatingSampling;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes the instance with its defalt attribute values.
        /// </summary>
        public SystemTagsMember(bool retentive, T defaultvalue, NodeState parent)
            : base(parent)
        {
            bRetentive = retentive;
            defaultValue = defaultvalue;
        }

        #endregion

        #region Public Events
        public event EventHandler SamplingIntervallExpired;
        #endregion

        #region Private Methods
        internal void SetSampling(double samplingIntervall)
        {
            if (samplingTimer != null)
            {
                samplingTimer.Dispose();
                samplingTimer = null;
            }

            if (samplingIntervall > 0.0 && SamplingIntervallExpired != null)
            {
                samplingIntervall = Math.Max(samplingIntervall, MinimumSamplingInterval);
                samplingTimer = new Timer(new TimerCallback(OnSamplingIntervallExpired), null, (long)samplingIntervall, (long)samplingIntervall);
            }
        }

        void OnSamplingIntervallExpired(object state)
        {
            if (updatingSampling)
                return;

            try
            {
                updatingSampling = true;
                var e = SamplingIntervallExpired;
                if (e != null)
                    e(this, EventArgs.Empty);
            }
            finally
            {
                updatingSampling = false;
            }
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// Creates a node with default values and assigns new node ids to it and all children.
        /// </summary>
        public override void Create(
            ISystemContext context,
            NodeId nodeId,
            QualifiedName browseName,
            LocalizedText displayName,
            bool assignNodeIds)
        {
            var description = Description;
            var accessLevel = AccessLevel;
            var userAccessLevel = UserAccessLevel;
            var minimumSamplingInterval = MinimumSamplingInterval;

            base.Create(context, nodeId, browseName, displayName, assignNodeIds);

            Description = description;
            AccessLevel = accessLevel;
            UserAccessLevel = userAccessLevel;
            MinimumSamplingInterval = minimumSamplingInterval;

            if (!bRetentive || !LoadRetentive())
            {
                this.WrappedValue = new Variant(defaultValue);
                this.Timestamp = DateTime.UtcNow;
            }
            
            if (bRetentive)
            {
                this.StateChanged += (context2, node, changes) =>
                {
                    if ((changes & NodeStateChangeMasks.Value) != 0)
                        SaveRetentive();
                };
            }
        }

        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (samplingTimer != null)
                {
                    samplingTimer.Dispose();
                    samplingTimer = null;
                }
            }
        }

        #endregion

        #region Isolated Storage Persistence
        static IsolatedStorageFile GetStorage()
        {
            try
            {
                return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            }
            catch
            {
                try
                {
                    return IsolatedStorageFile.GetStore(IsolatedStorageScope.Machine | IsolatedStorageScope.Assembly, null, null);
                }
                catch (Exception ex)
                {
                    Logger.WriteToEventLog(Properties.Resources.LoggerSource, ex.Message,
                                            System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                }
            }

            return null;
        }

        String GetStorageFileName()
        {
            return String.Format("{0}.{1}.dat", 
                System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location), 
                this.Parent.SymbolicName);
        }

        void SaveRetentive()
        {
            IsolatedStorageFile isoStorage = GetStorage();
            if (null == isoStorage || string.IsNullOrEmpty(GetStorageFileName()))
                return;

            using (Stream stream = new IsolatedStorageFileStream(GetStorageFileName(), FileMode.Create, isoStorage))
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = false,
                    Encoding = Encoding.UTF8
                };

                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {
                    try
                    {
                        writer.WriteElementString(SymbolicName, String.Format(CultureInfo.InvariantCulture, "{0}", this.WrappedValue.Value));
                    }
                    catch (Exception ex)
                    {
                        writer.Close();
                    }
                }
            }
        }

        bool LoadRetentive()
        {
            IsolatedStorageFile isoStorage = GetStorage();
            if (null == isoStorage || string.IsNullOrEmpty(GetStorageFileName()) ||
                isoStorage.GetFileNames(GetStorageFileName()).Length <= 0)
                return false;

            using (Stream stream = new IsolatedStorageFileStream(GetStorageFileName(), FileMode.OpenOrCreate, isoStorage))
            {
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    ConformanceLevel = ConformanceLevel.Document,
                    CloseInput = true
                };

                using (XmlReader reader = XmlReader.Create(stream, settings))
                {
                    try
                    {
                        object value = Convert.ChangeType(reader.ReadElementString(SymbolicName), typeof(T), CultureInfo.InvariantCulture);
                        this.WrappedValue = new Variant(value);
                        this.Timestamp = DateTime.UtcNow;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        reader.Close();
                    }
                }
            }

            return false;
        }
        #endregion

        #region Private Members

        readonly bool bRetentive;
        readonly T defaultValue;

        #endregion
        
    }
}
