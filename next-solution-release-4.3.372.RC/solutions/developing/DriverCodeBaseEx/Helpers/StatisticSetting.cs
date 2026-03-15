using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverBaseInterfaces;

namespace DriverCodeBaseEx.Helpers
{
    public static class StatisticSetting
    {
        public enum NodeDataNames
        {
            ByteRate,
            TotalBytesRead,
            TotalBytesWrite,
            CommunicationTaskRate,
            TimeForCommunicationTask_mS,
            ElapsedTime,
			StartingDiagnosticTime,
			EndDiagnosticTime,
			TotalJobsError,
			JobRate,
			TotalJobsRead,
			TotalJobsWrite,
			LastError,
			LastErrorTime,
			TagRate,
			TotalTagsRead,
			TotalTagsWrite,
			TotalRxBytes,
			TotalTags,
			TotalTagsInUse,
			PeakOfTagsInUse,
			TotalTasks,
			TotalTxBytes,
            TotalJobs,
            TotalJobsInUse,
            InErrorState,
        }
        //static readonly Array NodeNames = Enum.GetValues(typeof(NodeDataNames));

        public static readonly Dictionary<NodeDataNames, UFUAModel.DataType> NodeDataTypes = new Dictionary<NodeDataNames, UFUAModel.DataType>()
        {
            { NodeDataNames.ByteRate, UFUAModel.DataType.Int64 },
            { NodeDataNames.TotalBytesRead, UFUAModel.DataType.Int64 },
            { NodeDataNames.TotalBytesWrite, UFUAModel.DataType.Int64 },
            { NodeDataNames.CommunicationTaskRate, UFUAModel.DataType.Int64 },
            { NodeDataNames.TimeForCommunicationTask_mS, UFUAModel.DataType.String },
            { NodeDataNames.ElapsedTime, UFUAModel.DataType.String },
            { NodeDataNames.StartingDiagnosticTime, UFUAModel.DataType.Int64 },
            { NodeDataNames.EndDiagnosticTime, UFUAModel.DataType.Int64 },
            { NodeDataNames.TotalJobsError, UFUAModel.DataType.Int64 },
            { NodeDataNames.JobRate, UFUAModel.DataType.Int64 },
            { NodeDataNames.TotalJobsRead, UFUAModel.DataType.Int64 },
            { NodeDataNames.TotalJobsWrite, UFUAModel.DataType.Int64 },
            { NodeDataNames.LastError, UFUAModel.DataType.String },
            { NodeDataNames.LastErrorTime, UFUAModel.DataType.String },
            { NodeDataNames.TagRate, UFUAModel.DataType.Int64 },
            { NodeDataNames.TotalTagsRead, UFUAModel.DataType.Int64 },
            { NodeDataNames.TotalTagsWrite, UFUAModel.DataType.Int64 },
            { NodeDataNames.TotalRxBytes, UFUAModel.DataType.Int64 },
            { NodeDataNames.TotalTags, UFUAModel.DataType.Int64 },
            { NodeDataNames.TotalTagsInUse, UFUAModel.DataType.Int64 },
            { NodeDataNames.PeakOfTagsInUse, UFUAModel.DataType.Int64 },
            { NodeDataNames.TotalTasks, UFUAModel.DataType.Int64 },
            { NodeDataNames.TotalTxBytes, UFUAModel.DataType.Int64 },
            { NodeDataNames.TotalJobs, UFUAModel.DataType.Int64 },
            { NodeDataNames.TotalJobsInUse, UFUAModel.DataType.Int64 },
            { NodeDataNames.InErrorState, UFUAModel.DataType.Boolean },
        };
        public static readonly Dictionary<NodeDataNames, string> NodeDataDecriptions = new Dictionary<NodeDataNames, string>
        {
            { NodeDataNames.ByteRate, "Byte per sec"},
            { NodeDataNames.TotalBytesRead, "Number of read bytes , since last run."},
            { NodeDataNames.TotalBytesWrite, "Number of write bytes , since last run."},
            { NodeDataNames.CommunicationTaskRate, "Communication task per sec"},
            { NodeDataNames.TimeForCommunicationTask_mS, "Time for communication task  (mS)"},
            { NodeDataNames.ElapsedTime, "Elapsed time of the diagnostic driver."},
            { NodeDataNames.StartingDiagnosticTime, "Starting time of diagnostic of the driver."},
            { NodeDataNames.EndDiagnosticTime, "End time of diagnostic of the driver."},
            { NodeDataNames.TotalJobsError, "Number of jobs executed with error, since last run."},
            { NodeDataNames.JobRate, "Jobs per sec"},
            { NodeDataNames.TotalJobsRead, "Number of read jobs , since last run."},
            { NodeDataNames.TotalJobsWrite, "Number of write jobs , since last run."},
            { NodeDataNames.LastError, "Last error."},
            { NodeDataNames.LastErrorTime, "Time of the last error."},
            { NodeDataNames.TagRate, "Tag per sec"},
            { NodeDataNames.TotalTagsRead, "Number of read tags, since last run."},
            { NodeDataNames.TotalTagsWrite, "Number of write tags, since last run."},
            { NodeDataNames.TotalRxBytes, "Total number of bytes received, since last run."},
            { NodeDataNames.TotalTags, "Total driver tags"},
            { NodeDataNames.TotalTagsInUse, "Total tags in use of the driver"},
            { NodeDataNames.PeakOfTagsInUse, "Peak of the tags in use , since last run."},
            { NodeDataNames.TotalTasks, "Number of executed tasks , since last run."},
            { NodeDataNames.TotalTxBytes, "Total number of bytes sent , since last run."},
            { NodeDataNames.TotalJobs, "Total driver jobs"},
            { NodeDataNames.TotalJobsInUse, "Total jobs in use of the driver"},
            { NodeDataNames.InErrorState, "Error state." },
        };
        
        public static List<StatisicTag> GetStatisicTag(Array NodeArray)
        {
            List<StatisicTag> newlist = new List<StatisicTag>();

            foreach (StatisticSetting.NodeDataNames NodeDataName in NodeArray)
            {
                string NodeDescription = "";
                UFUAModel.DataType NodeDataType = UFUAModel.DataType.String;

                if (StatisticSetting.NodeDataDecriptions.ContainsKey(NodeDataName))
                    NodeDescription = StatisticSetting.NodeDataDecriptions[NodeDataName];
                if (StatisticSetting.NodeDataTypes.ContainsKey(NodeDataName))
                    NodeDataType = StatisticSetting.NodeDataTypes[NodeDataName];

                newlist.Add(new StatisicTag()
                {
                    Name = NodeDataName.ToString(),
                    Description = NodeDescription,
                    DataType = NodeDataType,
                });
            };
            return newlist;
        }

    }
}
