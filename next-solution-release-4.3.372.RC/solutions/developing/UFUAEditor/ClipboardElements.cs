using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFUAEditor
{
    internal struct ClipboardElements
    {
        public bool ContainsDrivers { get; private set; }
        public bool ContainsFolders { get; private set; }
        public bool ContainsTags { get; private set; }
        public bool ContainsAlarmAreas { get; private set; }
        public bool ContainsAlarmDefinitions { get; private set; }
        public bool ContainsAlarmSources { get; private set; }
        public bool ContainsAlarmThresholds { get; private set; }
        public bool ContainsDataLoggerColumn { get; private set; }
        public bool ContainsDataLoggerSettings { get; private set; }
        public bool ContainsEngineeringUnits { get; private set; }
        public bool ContainsHistorians { get; private set; }
        public bool ContainsPrototypes { get; private set; }
        public bool ContainsViews { get; private set; }

        public void Clear()
        {
            ContainsDrivers = false;
            ContainsFolders = false;
            ContainsTags = false;
            ContainsAlarmAreas = false;
            ContainsAlarmDefinitions = false;
            ContainsAlarmSources = false;
            ContainsAlarmThresholds = false;
            ContainsDataLoggerColumn = false;
            ContainsDataLoggerSettings = false;
            ContainsEngineeringUnits = false;
            ContainsHistorians = false;
            ContainsPrototypes = false;
            ContainsViews = false;
        }

        public void Check(UnitOfWork uow)
        {
            ContainsDrivers = (from c in new XPQuery<UFUAModel.UFUACommunicationDriver>(uow).AsParallel() select c).ToList().Count > 0;
            ContainsFolders = (from c in new XPQuery<UFUAModel.UFUAFolder>(uow).AsParallel() select c).ToList().Count > 0 ||
                (from c in new XPQuery<TempVariablesModel.Folder>(uow).AsParallel() select c).ToList().Count > 0;
            ContainsTags = (from c in new XPQuery<UFUAModel.UFUATag>(uow).AsParallel() select c).ToList().Count > 0 ||
                (from c in new XPQuery<TempVariablesModel.Variable>(uow).AsParallel() select c).ToList().Count > 0;
            ContainsAlarmAreas = (from c in new XPQuery<UFUAModel.UFUAArea>(uow).AsParallel() select c).ToList().Count > 0;
            ContainsAlarmDefinitions = (from c in new XPQuery<UFUAModel.UFUAAlarmDefinition>(uow).AsParallel() select c).ToList().Count > 0;
            ContainsAlarmSources = (from c in new XPQuery<UFUAModel.UFUAAlarmSource>(uow).AsParallel() select c).ToList().Count > 0;
            ContainsAlarmThresholds = (from c in new XPQuery<UFUAModel.UFUAAlarmThreshold>(uow).AsParallel() select c).ToList().Count > 0;
            ContainsDataLoggerColumn = (from c in new XPQuery<DataLoggerModel.DataLoggerColumn>(uow).AsParallel() select c).ToList().Count > 0;
            ContainsDataLoggerSettings = (from c in new XPQuery<DataLoggerModel.DataLoggerSettings>(uow).AsParallel() select c).ToList().Count > 0;
            ContainsEngineeringUnits = (from c in new XPQuery<UFUAModel.UFUAEngineeringUnit>(uow).AsParallel() select c).ToList().Count > 0;
            ContainsHistorians = (from c in new XPQuery<UFUAModel.UFUAHistorianSettings>(uow).AsParallel() select c).ToList().Count > 0;
            ContainsPrototypes = (from c in new XPQuery<UFUAModel.UFUATagPrototype>(uow).AsParallel() select c).ToList().Count > 0;
            ContainsViews = (from c in new XPQuery<UFUAModel.UFUAView>(uow).AsParallel() select c).ToList().Count > 0;
        }
    }
}
