using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharedModels
{
    public class NodeModel
    {
        public DatabaseConfig? Database { get; set; }
        public List<UserConfig> Users { get; set; } = new();
        public List<UserGroupConfig> UserGroups { get; set; } = new();
        public List<ScriptConfig> Scripts { get; set; } = new();
        public List<PlcProgramConfig> PlcPrograms { get; set; } = new();
        public List<ScreenConfig> Screens { get; set; } = new();
        public List<RecipeConfig> Recipes { get; set; } = new();
        public Folder Folder { get; set; } = new();
        public List<LocalizedStringEntry> Strings { get; set; } = new();
        public List<CameraConfig> Cameras { get; set; } = new();

        [JsonPropertyName("Server")]
        public ServerSettings Server { get; set; } = new();
    }

    public class DatabaseConfig
    {
        public string Provider { get; set; } = "TimescaleDb";
        public string ConnectionString { get; set; } = "";
        public string TableName { get; set; } = "variable_data";
    }

    public class Folder
    {
        public string Name { get; set; } = "";
        public List<Folder> Folders { get; set; } = new();
        public List<Variable> Variables { get; set; } = new();
    }

    public class Variable
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = ""; // "Double", "Int32", etc.
        public string Access { get; set; } = "ReadWrite"; // "Read", "Write", "ReadWrite"
        public object? Value { get; set; } // Can be Double, Int32, Boolean, String, JsonElement
        public TimeSpan? MaxAge { get; set; }
        public AlarmConfig? Alarm { get; set; }
        public DataLoggingConfig? DataLogging { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement>? DriverConfigs { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AlarmTriggerType
    {
        /// <summary>Trigger based on High/Low/HighHigh/LowLow numeric limit thresholds.</summary>
        Limit,
        /// <summary>Trigger based on a boolean condition expression evaluated against the variable value.</summary>
        Condition
    }

    public class AlarmConfig
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AlarmTriggerType TriggerType { get; set; } = AlarmTriggerType.Limit;

        // ── Limit mode fields ──
        public double? HighHighLimit { get; set; }
        public double HighLimit { get; set; }
        public double LowLimit { get; set; }
        public double? LowLowLimit { get; set; }

        // ── Condition mode fields ──
        /// <summary>
        /// Comparison operator for condition-based alarms.
        /// Supported: "==", "!=", "&gt;", "&gt;=", "&lt;", "&lt;=", "True", "False", "Changed".
        /// </summary>
        public string Operator { get; set; } = "==";

        /// <summary>
        /// The comparison value as a string. Parsed according to the variable type.
        /// For operators "True", "False", and "Changed" this is ignored.
        /// </summary>
        public string CompareValue { get; set; } = "";

        /// <summary>Severity (1–1000) when the condition alarm is active. Default 500.</summary>
        public ushort ConditionSeverity { get; set; } = 500;

        // ── Common fields ──
        public string Message { get; set; } = "";

        /// <summary>
        /// Hysteresis (deadband) value to prevent alarm flickering.
        /// For Limit mode: alarm deactivates only when the value returns past the limit by this amount
        /// (e.g. High alarm activates at HighLimit, deactivates at HighLimit − Hysteresis).
        /// For Condition mode with numeric operators: same deadband logic applies.
        /// A value of 0 disables hysteresis.
        /// </summary>
        public double Hysteresis { get; set; }
    }

    public class DataLoggingConfig
    {
        public bool Enabled { get; set; }
        public double Hysteresis { get; set; }
        public TimeSpan? MaxAge { get; set; }
    }

    public class UserConfig
    {
        public string Username { get; set; } = "";

        /// <summary>Legacy plain-text password. Migrated to PasswordHash on save.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Password { get; set; } = "";

        /// <summary>PBKDF2-SHA256 hashed password stored as Base64.</summary>
        public string PasswordHash { get; set; } = "";

        public string Group { get; set; } = "";

        /// <summary>Auto log-off after N seconds of inactivity. 0 = disabled.</summary>
        public int AutoLogOffSeconds { get; set; }

        /// <summary>Force password change after N days. 0 = never expires.</summary>
        public int PasswordExpiryDays { get; set; }

        /// <summary>When true, user must change password on first login.</summary>
        public bool MustChangePasswordOnFirstLogin { get; set; }

        /// <summary>UTC date when the password was last changed. Null = never set.</summary>
        public DateTime? PasswordChangedDate { get; set; }

        /// <summary>Set to true after the user has logged in at least once.</summary>
        public bool HasLoggedInBefore { get; set; }
    }

    public class UserGroupConfig
    {
        public string Name { get; set; } = "";
        public string AccessLevel { get; set; } = "Read"; // Read, Write, ReadWrite

        /// <summary>Whether users in this group can log in to the project editor.</summary>
        public bool CanAccessEditor { get; set; }

        /// <summary>Whether users in this group can log in to the runtime viewer.</summary>
        public bool CanAccessRuntime { get; set; }
    }

    public class ScriptConfig
    {
        public string Name { get; set; } = "";
        public string Code { get; set; } = "";
        public bool Enabled { get; set; } = true;
        public int IntervalMs { get; set; } = 1000;
        public string Language { get; set; } = "CSharp";

        /// <summary>Optional folder path for editor organization (e.g. "Alarms/Temperature"). Ignored by the server.</summary>
        public string Group { get; set; } = "";
    }

    /// <summary>
    /// Configuration for an IEC 61131-3 PLC program.
    /// Supports Structured Text (ST), Instruction List (IL), and Ladder Diagram (LD).
    /// Programs run cyclically at the configured interval, reading and writing OPC variables.
    /// </summary>
    public class PlcProgramConfig
    {
        public string Name { get; set; } = "";
        public string Code { get; set; } = "";
        public bool Enabled { get; set; } = true;
        public int IntervalMs { get; set; } = 100;

        /// <summary>Programming language: ST (Structured Text), IL (Instruction List), or LD (Ladder Diagram).</summary>
        public string Language { get; set; } = "ST";

        /// <summary>Optional folder path for editor organization (e.g. "Motion/Axis1"). Ignored by the server.</summary>
        public string Group { get; set; } = "";
    }

    /// <summary>
    /// Configuration for a recipe definition.
    /// A recipe stores named sets of variable values in a SQLite database.
    /// Variables are identified by index for efficient storage.
    /// At runtime, the server creates OPC variables for Load, Save, Activate, and ActiveRecipeName commands.
    /// </summary>
    public class RecipeConfig
    {
        public string Name { get; set; } = "";

        /// <summary>Path to the SQLite database file (relative to nodes.json). Default "recipes/{Name}.db".</summary>
        public string DbPath { get; set; } = "";

        /// <summary>Whether the recipe manager is active at runtime.</summary>
        public bool Enabled { get; set; } = true;

        /// <summary>The list of OPC variables included in this recipe, each with a unique index.</summary>
        public List<RecipeVariable> Variables { get; set; } = new();
    }

    /// <summary>
    /// A variable entry in a recipe definition.
    /// Index is a stable numeric identifier used as the column key in the recipe database.
    /// VariablePath is the OPC variable path (e.g. "Plant.Furnace.Temperature").
    /// </summary>
    public class RecipeVariable
    {
        /// <summary>Unique numeric index for this variable within the recipe (stable across renames).</summary>
        public int Index { get; set; }

        /// <summary>OPC variable path (e.g. "Plant.Line1.Speed").</summary>
        public string VariablePath { get; set; } = "";

        /// <summary>Optional display name for the variable in the recipe editor.</summary>
        public string DisplayName { get; set; } = "";
    }

    public class ServerSettings
    {
        public string EndpointUrl { get; set; } = "opc.tcp://localhost:14840/SimpleOpcFileServer";
        public bool EnableAnonymous { get; set; } = true;

        /// <summary>When true, the web editor requires login before use.</summary>
        public bool EnableEditorLogin { get; set; }

        /// <summary>When true, the runtime viewer requires login before displaying screens.</summary>
        public bool EnableRuntimeLogin { get; set; }

        /// <summary>
        /// When true, passwords must meet strong-password requirements:
        /// minimum 8 characters, at least one uppercase, one lowercase, one digit, and one special character.
        /// </summary>
        public bool RequireStrongPassword { get; set; }

        /// <summary>Name of the screen to display on startup in the RuntimeViewer.</summary>
        public string StartupScreen { get; set; } = "";

        /// <summary>Configuration for the server event log (alarms, auth, driver, system events).</summary>
        public EventLogConfig? EventLog { get; set; }

        /// <summary>
        /// HTTP port for the diagnostics API. Set to 0 to disable.
        /// When enabled, the server exposes GET /diag returning subsystem performance JSON.
        /// Default: 14841.
        /// </summary>
        public int DiagnosticsPort { get; set; } = 14841;

        /// <summary>
        /// SMTP configuration for crash report email notifications.
        /// When configured, unhandled exceptions are automatically emailed to the specified recipient.
        /// </summary>
        public CrashEmailConfig? CrashEmail { get; set; }
    }

    /// <summary>
    /// SMTP settings for sending crash report emails.
    /// </summary>
    public class CrashEmailConfig
    {
        /// <summary>Whether crash email notifications are enabled.</summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// When true, crash reports are emailed automatically as soon as they occur.
        /// When false, emails can still be sent manually from the Crash Reports panel.
        /// Default: true.
        /// </summary>
        public bool AutoSend { get; set; } = true;

        /// <summary>SMTP server host (e.g. "smtp.gmail.com", "smtp.office365.com").</summary>
        public string SmtpHost { get; set; } = "";

        /// <summary>SMTP port (e.g. 587 for TLS, 465 for SSL, 25 for plain).</summary>
        public int SmtpPort { get; set; } = 587;

        /// <summary>Use TLS/SSL for the SMTP connection.</summary>
        public bool UseSsl { get; set; } = true;

        /// <summary>SMTP username (often the sender email address).</summary>
        public string Username { get; set; } = "";

        /// <summary>SMTP password or app password.</summary>
        public string Password { get; set; } = "";

        /// <summary>Sender email address.</summary>
        public string From { get; set; } = "";

        /// <summary>Recipient email address(es), comma-separated.</summary>
        public string To { get; set; } = "";

        /// <summary>Optional project/site name included in the email subject.</summary>
        public string ProjectName { get; set; } = "";
    }

    /// <summary>
    /// Configuration for the server event journal stored in a SQLite database.
    /// Records alarms, user authentication, driver events, and system events.
    /// </summary>
    public class EventLogConfig
    {
        /// <summary>Whether event logging is enabled. Default true when section is present.</summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Path to the SQLite database file (relative to nodes.json).
        /// Default "events.db".
        /// </summary>
        public string DbPath { get; set; } = "events.db";

        /// <summary>
        /// Maximum age of events in days. Events older than this are periodically purged.
        /// 0 = keep forever. Default 90 days.
        /// </summary>
        public int MaxAgeDays { get; set; } = 90;
    }

    public class ScreenConfig
    {
        public string Name { get; set; } = "";
        public int Width { get; set; } = 800;
        public int Height { get; set; } = 600;
        public string Background { get; set; } = "#ffffff";

        /// <summary>
        /// Optional background raster image stored as a base64 data URI (e.g. "data:image/png;base64,...").
        /// When set, the image is rendered behind all symbols, covering the entire screen canvas.
        /// </summary>
        public string BackgroundImage { get; set; } = "";

        /// <summary>Layout mode: "svg" for fixed SVG canvas, "responsive" for responsive HTML grid.</summary>
        public string LayoutMode { get; set; } = "svg";

        /// <summary>Number of grid columns for responsive layout.</summary>
        public int GridColumns { get; set; } = 12;

        /// <summary>Gap between grid cells in pixels (responsive layout).</summary>
        public int GridGap { get; set; } = 8;

        /// <summary>Optional folder path for editor organization (e.g. "Main/Popups"). Ignored by the server.</summary>
        public string Group { get; set; } = "";

        public List<ScreenSymbol> Symbols { get; set; } = new();
    }

    public class ScreenSymbol
    {
        public string Id { get; set; } = "";
        public string Type { get; set; } = "rect"; // rect, circle, ellipse, text, line, gauge, indicator, svg, alarmlist, hdachart, hdagrid, eventlog, editbox, ipcamera, recipe, screenembed, imagemap, trend
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; } = 80;
        public double Height { get; set; } = 40;
        public string Fill { get; set; } = "#4a90d9";
        public string Stroke { get; set; } = "#333333";
        public double StrokeWidth { get; set; } = 1;
        public string Label { get; set; } = "";
        public double Rotation { get; set; }

        /// <summary>Inline SVG content for library symbols (Type == "svg").</summary>
        public string? SvgContent { get; set; }

        /// <summary>Group identifier. Symbols sharing the same non-null GroupId belong to one group.</summary>
        public string? GroupId { get; set; }

        /// <summary>Animations configured for this symbol.</summary>
        public List<SymbolAnimation> Animations { get; set; } = new();

        // OPC variable binding
        public string? VariablePath { get; set; }

        /// <summary>
        /// Multiple variable paths for HDA widgets (Type == "hdachart" or "hdagrid").
        /// Each path corresponds to one series/column.
        /// </summary>
        public List<string> HdaVariablePaths { get; set; } = new();

        /// <summary>HDA time range in minutes (0 = all). Default 60.</summary>
        public int HdaTimeRangeMinutes { get; set; } = 60;

        /// <summary>Maximum number of data points per series.</summary>
        public int HdaMaxPoints { get; set; } = 500;

        /// <summary>Maximum rows shown in the Event Log widget (Type == "eventlog"). Default 200.</summary>
        public int EventLogMaxRows { get; set; } = 200;

        /// <summary>
        /// Event log time range in minutes (Type == "eventlog"). Default 1440 (24h). 0 = all.
        /// </summary>
        public int EventLogTimeRangeMinutes { get; set; } = 1440;

        /// <summary>
        /// Comma-separated category filter for the Event Log widget.
        /// e.g. "Alarm,Auth,Driver,System". Empty = all categories.
        /// </summary>
        public string EventLogCategories { get; set; } = "";

        /// <summary>
        /// Step increment for the EditBox spin buttons (Type == "editbox").
        /// When the bound variable is numeric, the spin buttons adjust the value by this amount.
        /// Default 1.
        /// </summary>
        public double EditBoxStep { get; set; } = 1;

        /// <summary>
        /// Format string for the EditBox display (Type == "editbox").
        /// e.g. "F2" for 2 decimal places. Empty = raw value.
        /// </summary>
        public string EditBoxFormat { get; set; } = "";

        // Animation bindings — evaluated at runtime
        public string? FillBinding { get; set; }       // e.g., "value > 50 ? '#ff0000' : '#00ff00'"
        public string? VisibilityBinding { get; set; } // e.g., "value == true"
        public string? RotationBinding { get; set; }   // e.g., "value * 3.6"
        public string? LabelBinding { get; set; }      // e.g., "value.ToString('F1') + ' °C'"
        public double? MinValue { get; set; }
        public double? MaxValue { get; set; }

        // ─── Gauge style properties (Type == "gauge") ────────────
        /// <summary>
        /// Gauge visual style. Supported values:
        /// "needle" — classic dial with rotating needle (default),
        /// "arc" — arc/donut with filled progress stroke,
        /// "semicircle" — 180° half-circle arc gauge,
        /// "hbar" — horizontal bar fill,
        /// "vbar" — vertical bar fill (bottom-up),
        /// "thermometer" — vertical thermometer with bulb.
        /// </summary>
        public string GaugeStyle { get; set; } = "needle";

        /// <summary>Number of major tick marks on the gauge scale. 0 = no ticks. Default 5.</summary>
        public int GaugeTicks { get; set; } = 5;

        /// <summary>Whether to show numeric tick labels on the scale. Default true.</summary>
        public bool GaugeShowTickLabels { get; set; } = true;

        /// <summary>Whether to show the numeric value readout. Default true.</summary>
        public bool GaugeShowValue { get; set; } = true;

        /// <summary>Unit suffix shown after the value readout (e.g. "°C", "bar", "%"). Default "".</summary>
        public string GaugeUnit { get; set; } = "";

        /// <summary>Color of the gauge scale/track background. Default "#e0e0e0".</summary>
        public string GaugeTrackColor { get; set; } = "#e0e0e0";

        /// <summary>Color of the needle or secondary elements. Default "#333333".</summary>
        public string GaugeNeedleColor { get; set; } = "#333333";

        /// <summary>Value format string for the readout (e.g. "F1", "F0"). Default "F1".</summary>
        public string GaugeValueFormat { get; set; } = "F1";

        /// <summary>Commands executed at runtime when the user interacts with this symbol.</summary>
        public List<SymbolCommand> Commands { get; set; } = new();

        /// <summary>
        /// Required access level for this symbol at runtime.
        /// Empty/None = no restriction; Read = visible only with read access;
        /// Write = commands require write access; ReadWrite = both.
        /// </summary>
        public string RequiredAccess { get; set; } = "";

        /// <summary>Camera configuration for IP camera widgets (Type == "ipcamera").</summary>
        public CameraConfig? Camera { get; set; }

        /// <summary>Recipe name to bind this widget to (Type == "recipe"). Must match a RecipeConfig.Name.</summary>
        public string RecipeName { get; set; } = "";

        /// <summary>
        /// Screen names to embed inside this symbol (Type == "screenembed").
        /// If one screen is specified, it is rendered directly.
        /// If multiple screens are specified, a tab strip is shown with lazy-loaded content.
        /// </summary>
        public List<string> EmbeddedScreens { get; set; } = new();

        // ─── Responsive layout properties ────────────────────────
        /// <summary>Column span in the responsive grid (1-12).</summary>
        public int ColSpan { get; set; } = 4;

        /// <summary>Row span in the responsive grid.</summary>
        public int RowSpan { get; set; } = 1;

        /// <summary>Row height in pixels for responsive layout.</summary>
        public int RowHeight { get; set; } = 200;

        /// <summary>Display order in the responsive grid (lower = first).</summary>
        public int Order { get; set; }

        /// <summary>
        /// List of condition-to-image mappings for the Image Map widget (Type == "imagemap").
        /// </summary>
        public List<ImageMapEntry> ImageMapEntries { get; set; } = new();

        // ─── Realtime Trend properties (Type == "trend") ─────────
        /// <summary>Pen definitions for the Realtime Trend widget.</summary>
        public List<TrendPen> TrendPens { get; set; } = new();

        /// <summary>Visible time window in seconds. Default 60.</summary>
        public int TrendTimeWindowSeconds { get; set; } = 60;

        /// <summary>Whether to show horizontal grid lines. Default true.</summary>
        public bool TrendShowGrid { get; set; } = true;

        /// <summary>Whether the interactive cursor crosshair is enabled. Default true.</summary>
        public bool TrendShowCursor { get; set; } = true;

        /// <summary>Whether to show the legend bar below the chart. Default true.</summary>
        public bool TrendShowLegend { get; set; } = true;

        /// <summary>Y-axis minimum. Null = auto-scale.</summary>
        public double? TrendYMin { get; set; }

        /// <summary>Y-axis maximum. Null = auto-scale.</summary>
        public double? TrendYMax { get; set; }

        /// <summary>Background color of the trend plot area. Default "#111827".</summary>
        public string TrendBackground { get; set; } = "#111827";
    }

    /// <summary>
    /// A single pen (trace) in the Realtime Trend widget.
    /// Each pen is bound to a variable and drawn as a polyline.
    /// </summary>
    public class TrendPen
    {
        /// <summary>Variable path to sample at each tick.</summary>
        public string VariablePath { get; set; } = "";

        /// <summary>Display name shown in the legend. Empty = last segment of VariablePath.</summary>
        public string Label { get; set; } = "";

        /// <summary>Line color. Default is auto-assigned from palette.</summary>
        public string Color { get; set; } = "";

        /// <summary>Line width in pixels. Default 1.5.</summary>
        public double LineWidth { get; set; } = 1.5;
    }

    /// <summary>
    /// A single condition-to-image mapping used by the Image Map widget.
    /// Condition syntax examples: "value == 1", "value > 50", "value == true", "*" (default).
    /// ImageData stores the raster image as a base64 data URI (e.g. "data:image/png;base64,...").
    /// </summary>
    public class ImageMapEntry
    {
        /// <summary>
        /// Condition expression evaluated against the live variable value.
        /// Supports: "value == 1", "value > 50", "value &lt;= 10", "value == true", "value != 0".
        /// Use "*" or "" as a catch-all default.
        /// </summary>
        public string Condition { get; set; } = "*";

        /// <summary>Display label for this entry in the editor (optional).</summary>
        public string Label { get; set; } = "";

        /// <summary>Raster image stored as a base64 data URI.</summary>
        public string ImageData { get; set; } = "";
    }

    /// <summary>
    /// A command bound to a screen symbol, executed at runtime on user interaction.
    /// </summary>
    public class SymbolCommand
    {
        public string Id { get; set; } = "";

        /// <summary>
        /// Command type:
        /// NavigateScreen, OpenScreenPopup, OpenScreenModal,
        /// SetVariable, ResetVariable, ToggleVariable, IncrementVariable, DecrementVariable,
        /// WriteVariable,
        /// ExecuteJavaScript, ExecuteScript,
        /// Login, Logout,
        /// AcknowledgeAllAlarms, ResetAllAlarms,
        /// ChangeLanguage
        /// </summary>
        public string Action { get; set; } = "NavigateScreen";

        /// <summary>
        /// Trigger: MouseDown, MouseUp, Click, WhilePressed
        /// </summary>
        public string Trigger { get; set; } = "Click";

        /// <summary>Target screen name (for NavigateScreen/OpenScreenPopup/OpenScreenModal).</summary>
        public string TargetScreen { get; set; } = "";

        /// <summary>OPC variable path (for Set/Reset/Toggle/Increment/Decrement/WriteVariable).</summary>
        public string VariablePath { get; set; } = "";

        /// <summary>Value to write (for SetVariable/WriteVariable). For Toggle: ignored. For Increment/Decrement: step amount.</summary>
        public string Value { get; set; } = "";

        /// <summary>Value to write on release (WhilePressed trigger). If empty, ResetVariable is used.</summary>
        public string ReleaseValue { get; set; } = "";

        /// <summary>JavaScript or C# script body (for ExecuteJavaScript/ExecuteScript).</summary>
        public string Script { get; set; } = "";
    }

    /// <summary>
    /// A localized string entry. The Key is used as an ID (e.g., "btn_start"),
    /// and Translations maps language codes to translated text (e.g., "en" → "Start", "de" → "Starten").
    /// </summary>
    public class LocalizedStringEntry
    {
        public string Key { get; set; } = "";
        public Dictionary<string, string> Translations { get; set; } = new();
    }

    public class SymbolAnimation
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// The property to animate: Position, Scale, Rotation, Opacity, Fill, Stroke, StrokeWidth, Width, Height
        /// </summary>
        public string Property { get; set; } = "Position";

        /// <summary>Trigger type: Always, OnVariable, OnClick</summary>
        public string Trigger { get; set; } = "Always";

        /// <summary>OPC variable path that triggers the animation (when Trigger == OnVariable).</summary>
        public string? TriggerVariable { get; set; }

        /// <summary>Condition expression for the trigger variable, e.g. "value > 50".</summary>
        public string? TriggerCondition { get; set; }

        // ─── Keyframes ──────────────────────────────────────
        public List<AnimationKeyframe> Keyframes { get; set; } = new();

        // ─── Timing ─────────────────────────────────────────
        public double DurationMs { get; set; } = 1000;
        public double DelayMs { get; set; }

        /// <summary>Easing: Linear, EaseIn, EaseOut, EaseInOut, Bounce, Elastic</summary>
        public string Easing { get; set; } = "Linear";

        /// <summary>Number of repeat cycles. 0 = infinite.</summary>
        public int RepeatCount { get; set; } = 0;

        /// <summary>Whether to reverse on each alternate cycle.</summary>
        public bool AutoReverse { get; set; }
    }

    public class AnimationKeyframe
    {
        /// <summary>Position in the timeline 0–100 (percent).</summary>
        public double Percent { get; set; }

        /// <summary>Value at this keyframe. Interpretation depends on the property being animated.</summary>
        public string Value { get; set; } = "";

        /// <summary>Per-segment easing override (optional).</summary>
        public string? Easing { get; set; }
    }

    /// <summary>
    /// Configuration for an IP camera (Type == "ipcamera").
    /// The server captures frames, optionally runs YOLO object detection, and streams MJPEG to viewers.
    /// </summary>
    public class CameraConfig
    {
        /// <summary>Unique camera ID used for the server stream endpoint.</summary>
        public string CameraId { get; set; } = "";

        /// <summary>Camera stream URL (RTSP, MJPEG, or HTTP snapshot).</summary>
        public string Url { get; set; } = "";

        /// <summary>Protocol: "rtsp", "mjpeg", or "http" (snapshot).</summary>
        public string Protocol { get; set; } = "mjpeg";

        /// <summary>Frames per second for HTTP snapshot polling. Default 5.</summary>
        public int Fps { get; set; } = 5;

        /// <summary>Enable YOLO object detection on frames.</summary>
        public bool EnableYolo { get; set; }

        /// <summary>Path to the YOLO ONNX model file. Empty = use default bundled model.</summary>
        public string YoloModelPath { get; set; } = "";

        /// <summary>Minimum confidence threshold for detections (0.0–1.0). Default 0.5.</summary>
        public double YoloConfidence { get; set; } = 0.5;

        /// <summary>
        /// OPC variable path prefix for writing detection results.
        /// The server creates variables: {Prefix}.Label, {Prefix}.Confidence, {Prefix}.Count
        /// </summary>
        public string DetectionVariablePrefix { get; set; } = "";

        /// <summary>Draw bounding boxes on the stream output.</summary>
        public bool DrawDetections { get; set; } = true;

        /// <summary>Optional username for camera authentication.</summary>
        public string Username { get; set; } = "";

        /// <summary>Optional password for camera authentication.</summary>
        public string Password { get; set; } = "";
    }

    // ─── Diagnostics ─────────────────────────────────────────

    /// <summary>
    /// Snapshot of server subsystem diagnostics, exposed via the /diag HTTP endpoint.
    /// </summary>
    public class ServerDiagnostics
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public double ProcessCpuPercent { get; set; }
        public long MemoryMB { get; set; }
        public int ThreadCount { get; set; }
        public List<SubsystemDiagnostics> Subsystems { get; set; } = new();
    }

    /// <summary>
    /// Performance data for a single subsystem (driver, script, PLC program, recipe, logger, etc.).
    /// </summary>
    public class SubsystemDiagnostics
    {
        public string Category { get; set; } = ""; // "Driver", "Script", "PlcProgram", "Recipe", "DataLogger", "EventLogger"
        public string Name { get; set; } = "";
        public bool Enabled { get; set; } = true;
        public long CycleCount { get; set; }
        public double AvgCycleMs { get; set; }
        public double MaxCycleMs { get; set; }
        public double LastCycleMs { get; set; }
        public double TotalCpuMs { get; set; }
        public string Status { get; set; } = "Running"; // Running, Stopped, Error
        public string? LastError { get; set; }
    }
}
