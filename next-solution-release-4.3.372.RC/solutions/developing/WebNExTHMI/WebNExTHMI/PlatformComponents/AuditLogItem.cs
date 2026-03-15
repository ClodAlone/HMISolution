using System;
using System.Collections.Generic;

namespace WebNExTHMI.PlatformComponents
{
    public class AuditLogItem
    {
        public DateTime EventDateTimeUtc { get; }
        public string EventType { get; }
        public string SourceName { get; }
        public string SourceNode { get; }
        public string UserName { get; }
        public DateTime UtcRecordingTime { get; }
        public ushort Severity { get; }
        public DateTime EventDateTime { get; }
        public string EventComment { get; }
        public string EventDetails { get; }
        public TimeSpan EventDuration { get; }
        public string EventMessage { get; }
        public ulong EventOccurence { get; }
        public ulong EventSequence { get; }
        public string EventState { get; }

        public AuditLogItem(DateTime eventDateTimeUtc, string eventType, string sourceName, string sourceNode, string userName, DateTime utcRecordingTime, ushort severity, DateTime eventDateTime, string eventComment, string eventDetails, TimeSpan eventDuration, string eventMessage, ulong eventOccurence, ulong eventSequence, string eventState)
        {
            EventDateTimeUtc = eventDateTimeUtc;
            EventType = eventType;
            SourceName = sourceName;
            SourceNode = sourceNode;
            UserName = userName;
            UtcRecordingTime = utcRecordingTime;
            Severity = severity;
            EventDateTime = eventDateTime;
            EventComment = eventComment;
            EventDetails = eventDetails;
            EventDuration = eventDuration;
            EventMessage = eventMessage;
            EventOccurence = eventOccurence;
            EventSequence = eventSequence;
            EventState = eventState;
        }

        public override bool Equals(object obj)
        {
            return obj is AuditLogItem other &&
                   EventDateTimeUtc == other.EventDateTimeUtc &&
                   EventType == other.EventType &&
                   SourceName == other.SourceName &&
                   SourceNode == other.SourceNode &&
                   UserName == other.UserName &&
                   UtcRecordingTime == other.UtcRecordingTime &&
                   Severity == other.Severity &&
                   EventDateTime == other.EventDateTime &&
                   EventComment == other.EventComment &&
                   EventDetails == other.EventDetails &&
                   EventDuration.Equals(other.EventDuration) &&
                   EventMessage == other.EventMessage &&
                   EventOccurence == other.EventOccurence &&
                   EventSequence == other.EventSequence &&
                   EventState == other.EventState;
        }

        public override int GetHashCode()
        {
            var hashCode = -774596092;
            hashCode = hashCode * -1521134295 + EventDateTimeUtc.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(EventType);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SourceName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SourceNode);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UserName);
            hashCode = hashCode * -1521134295 + UtcRecordingTime.GetHashCode();
            hashCode = hashCode * -1521134295 + Severity.GetHashCode();
            hashCode = hashCode * -1521134295 + EventDateTime.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(EventComment);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(EventDetails);
            hashCode = hashCode * -1521134295 + EqualityComparer<TimeSpan>.Default.GetHashCode(EventDuration);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(EventMessage);
            hashCode = hashCode * -1521134295 + EventOccurence.GetHashCode();
            hashCode = hashCode * -1521134295 + EventSequence.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(EventState);
            return hashCode;
        }
    }
}
