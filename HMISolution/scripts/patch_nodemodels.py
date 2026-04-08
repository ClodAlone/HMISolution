import sys

file_path = "C:\\Users\\cfior\\source\\repos\\SharedModels\\NodeModels.cs"

with open(file_path, "rb") as f:
    raw = f.read()

content = raw.decode("utf-8")
print(f"Read {len(content)} chars")

marker = "public List<BatchSequenceConfig> BatchSequences { get; set; } = new();"
idx = content.find(marker)
if idx < 0:
    print("ERROR: marker not found")
    sys.exit(1)

eol = content.index("\n", idx) + 1
insert_line = "        public List<EventConfig> Events { get; set; } = new();\r\n"
content = content[:eol] + insert_line + content[eol:]
print("Added Events to NodeModel")

event_class = '''
    // --- Events ---------------------------------------------------------

    /// <summary>
    /// Configuration for a condition-triggered event.
    /// When the condition is met, the associated commands are executed.
    /// The server evaluates event conditions periodically and fires commands on rising edge.
    /// </summary>
    public class EventConfig
    {
        /// <summary>Unique event name.</summary>
        public string Name { get; set; } = "";

        /// <summary>Whether this event is actively evaluated at runtime.</summary>
        public bool Enabled { get; set; } = true;

        /// <summary>Optional description / notes.</summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// OPC variable path whose value is evaluated as the trigger condition.
        /// </summary>
        public string ConditionVariablePath { get; set; } = "";

        /// <summary>
        /// Comparison operator: "True", "False", "==", "!=", ">", "<", ">=", "<=".
        /// Default "True" (fires when variable is truthy / non-zero).
        /// </summary>
        public string ConditionOperator { get; set; } = "True";

        /// <summary>
        /// Comparison value (used with ==, !=, >, <, >=, <=).
        /// Ignored for True/False operators.
        /// </summary>
        public string ConditionValue { get; set; } = "";

        /// <summary>
        /// Trigger mode:
        /// "RisingEdge"  - commands fire once when condition transitions from false to true.
        /// "Continuous"   - commands fire every evaluation cycle while condition is true.
        /// "FallingEdge"  - commands fire once when condition transitions from true to false.
        /// </summary>
        public string TriggerMode { get; set; } = "RisingEdge";

        /// <summary>
        /// Minimum hold time in seconds: the condition must remain true for this duration
        /// before the event fires. 0 = immediate.
        /// </summary>
        public int HoldTimeSeconds { get; set; }

        /// <summary>Commands executed when the event fires (condition met).</summary>
        public List<SymbolCommand> Commands { get; set; } = new();

        /// <summary>Optional commands executed when the condition clears (rising edge resets).</summary>
        public List<SymbolCommand> ClearCommands { get; set; } = new();

        /// <summary>Optional folder path for editor organization.</summary>
        public string Group { get; set; } = "";
    }
'''

last_brace = content.rfind("}")
content = content[:last_brace] + event_class + "\r\n}\r\n"
print("Added EventConfig class")

with open(file_path, "w", encoding="utf-8", newline="") as f:
    f.write(content)

print(f"Written {len(content)} chars - Done")
