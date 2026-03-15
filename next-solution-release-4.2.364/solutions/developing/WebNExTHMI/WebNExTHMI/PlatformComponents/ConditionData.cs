using Opc.Ua;
using OPCUAViewModel;
using System;

namespace WebNExTHMI.PlatformComponents
{
    public class ConditionData
    {
        public ConditionData(ConditionStateViewModel model)
        {
            nodeId = model.NodeIdString;
            dialogConditionPrompt = model.DialogConditionPrompt;
            shelveOneShot = model.ShelveOneShot;
            shelvingTime = model.ShelvingTime;
            dialogSelectedResponse = model.DialogSelectedResponse;
            conditionLastError = model.ConditionLastError;
            source = model.Source;
            sourceNode = model.SourceNode;
            condition = model.Condition;
            branch = model.Branch;
            branchText = model.BranchText;
            type = model.Type;
            severity = model.Severity.Value;
            quality = model.Quality;
            time = String.Format("{0}", model.Time);
            timeticks = model.Time != null ? ((DateTime)model.Time).Ticks.ToString() : "";
            enabledTransitionTime = String.Format("{0}", model.EnabledTransitionTime);
            activeTransitionTime = String.Format("{0}", model.ActiveTransitionTime);
            ackedTransitionTime = String.Format("{0}", model.AckedTransitionTime);
            confirmedTransitionTime = String.Format("{0}", model.ConfirmedTransitionTime);
            suppressedTransitionTime = String.Format("{0}", model.SuppressedTransitionTime);
            shelvingTransitionTime = String.Format("{0}", model.ShelvingTransitionTime);
            localTime = String.Format("{0}", model.LocalTime);
            receiveTime = String.Format("{0}", model.ReceiveTime);
            enabledState = model.EnabledState;
            message = model.Message;
            comment = model.Comment;
            retain = model.Retain.Value;
            isAcknowledgeableCondition = model.IsAcknowledgeableCondition;
            needsAcknoledge = model.NeedsAcknoledge;
            needsConfirm = model.NeedsConfirm;
        }

        public ConditionData()
        {
        }

        public String nodeId;
        public String dialogConditionPrompt;
        public bool shelveOneShot;
        public double shelvingTime;
        public int dialogSelectedResponse;
        public String conditionLastError;
        public String source;
        public String sourceNode;
        public String condition;
        public String branch;
        public String branchText;
        public String type;
        public ushort severity;
        public String quality;
        public String time;
        public String timeticks;
        public String enabledTransitionTime;
        public String activeTransitionTime;
        public String ackedTransitionTime;
        public String confirmedTransitionTime;
        public String suppressedTransitionTime;
        public String shelvingTransitionTime;
        public String localTime;
        public String receiveTime;
        public String enabledState;
        public String message;
        public String comment;
        public bool retain;
        public bool isAcknowledgeableCondition;
        public bool needsAcknoledge;
        public bool needsConfirm;
    }
}
