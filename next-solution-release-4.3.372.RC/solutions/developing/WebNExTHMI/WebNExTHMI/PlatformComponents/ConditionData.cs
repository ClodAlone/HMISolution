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
            timeticks = model.TimeOn != null ? ((DateTime)model.TimeOn).Ticks.ToString() : "";
            activeEffectiveTransitionTimeTicks = model.ActiveEffectiveTransitionTime != null ? ((DateTime)model.ActiveEffectiveTransitionTime).Ticks.ToString() : "";
            enabledTransitionTimeTicks = model.EnabledTransitionTime != null ? ((DateTime)model.EnabledTransitionTime).Ticks.ToString() : "";
            activeTransitionTimeTicks = model.ActiveTransitionTime != null ? ((DateTime)model.ActiveTransitionTime).Ticks.ToString() : "";
            ackedTransitionTimeTicks = model.AckedTransitionTime != null ? ((DateTime)model.AckedTransitionTime).Ticks.ToString() : "";
            confirmedTransitionTimeTicks = model.ConfirmedTransitionTime != null ? ((DateTime)model.ConfirmedTransitionTime).Ticks.ToString() : "";
            suppressedTransitionTimeTicks = model.SuppressedTransitionTime != null ? ((DateTime)model.SuppressedTransitionTime).Ticks.ToString() : "";
            shelvingTransitionTimeTicks = model.ShelvingTransitionTime != null ? ((DateTime)model.ShelvingTransitionTime).Ticks.ToString() : "";
            receiveTimeTicks = model.ReceiveTime != null ? ((DateTime)model.ReceiveTime).Ticks.ToString() : "";
            localTime = String.Format("{0}", model.LocalTime);
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
        public String timeticks;
        public String activeEffectiveTransitionTimeTicks;
        public String enabledTransitionTimeTicks;
        public String activeTransitionTimeTicks;
        public String ackedTransitionTimeTicks;
        public String confirmedTransitionTimeTicks;
        public String suppressedTransitionTimeTicks;
        public String shelvingTransitionTimeTicks;
        public String receiveTimeTicks;
        public String localTime;
        public String enabledState;
        public String message;
        public String comment;
        public bool retain;
        public bool isAcknowledgeableCondition;
        public bool needsAcknoledge;
        public bool needsConfirm;
    }
}
