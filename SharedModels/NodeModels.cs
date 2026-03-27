using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharedModels
{
    /// <summary>
    /// Specifies the allowed string values for a property, enabling dropdown rendering in the property grid.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AllowedStringValuesAttribute : Attribute
    {
        public string[] Values { get; }
        public AllowedStringValuesAttribute(params string[] values) => Values = values;
    }

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
        public List<ImageResource> Images { get; set; } = new();
        public List<CameraConfig> Cameras { get; set; } = new();
        public List<SchedulerConfig> Schedulers { get; set; } = new();
        public List<ReportConfig> Reports { get; set; } = new();
        public List<CalculatedVariableConfig> CalculatedVariables { get; set; } = new();

        /// <summary>
        /// PBKDF2-SHA256 hash of the project protection password.
        /// When set, the project is locked and cannot be edited without entering the correct password.
        /// </summary>
        public string ProjectPasswordHash { get; set; } = "";

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

        /// <summary>Initial value applied when the variable is created. Parsed according to Type.</summary>
        public string InitialValue { get; set; } = "";

        /// <summary>When true, the variable value is saved to disk and restored on server restart.</summary>
        public bool Retentive { get; set; }

        /// <summary>Optional runtime statistics configuration (min, max, average tracking).</summary>
        public VariableStatisticsConfig? Statistics { get; set; }

        /// <summary>Optional linear scaling configuration (raw ↔ engineering conversion).</summary>
        public ScalingConfig? Scaling { get; set; }

        /// <summary>Engineering unit label (e.g. "°C", "bar", "%", "m³/h"). Exposed as OPC UA EngineeringUnits property.</summary>
        public string EngineeringUnit { get; set; } = "";

        [JsonExtensionData]
        public Dictionary<string, JsonElement>? DriverConfigs { get; set; }
    }

    /// <summary>Configuration for runtime variable statistics (min, max, average, count).</summary>
    public class VariableStatisticsConfig
    {
        /// <summary>Whether runtime statistics collection is enabled for this variable.</summary>
        public bool Enabled { get; set; }
    }

    /// <summary>Linear scaling: raw driver values ↔ engineering units (linear interpolation).</summary>
    public class ScalingConfig
    {
        /// <summary>Raw value corresponding to the engineering minimum.</summary>
        public double RawMin { get; set; }

        /// <summary>Raw value corresponding to the engineering maximum.</summary>
        public double RawMax { get; set; } = 100;

        /// <summary>Engineering value at the low end of the scale.</summary>
        public double EngMin { get; set; }

        /// <summary>Engineering value at the high end of the scale.</summary>
        public double EngMax { get; set; } = 100;

        /// <summary>When true, clamp the engineering value to [EngMin, EngMax].</summary>
        public bool ClampEnabled { get; set; }

        /// <summary>Applies forward scaling: raw → engineering.</summary>
        public double RawToEng(double raw)
        {
            double range = RawMax - RawMin;
            if (Math.Abs(range) < 1e-15) return EngMin;
            double eng = (raw - RawMin) / range * (EngMax - EngMin) + EngMin;
            if (ClampEnabled)
            {
                double lo = Math.Min(EngMin, EngMax);
                double hi = Math.Max(EngMin, EngMax);
                eng = Math.Clamp(eng, lo, hi);
            }
            return eng;
        }

        /// <summary>Applies reverse scaling: engineering → raw.</summary>
        public double EngToRaw(double eng)
        {
            double range = EngMax - EngMin;
            if (Math.Abs(range) < 1e-15) return RawMin;
            return (eng - EngMin) / range * (RawMax - RawMin) + RawMin;
        }
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

        /// <summary>
        /// When true, alarm activation triggers notifications via the configured channels
        /// (Email, Telegram, WhatsApp) in the server's AlarmNotification settings.
        /// </summary>
        public bool NotifyOnActivation { get; set; }
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

        /// <summary>0-based line numbers where breakpoints are set. Persisted with the project.</summary>
        public List<int> Breakpoints { get; set; } = new();
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

        /// <summary>
        /// When true, the runtime viewer shows the screen navigation bar even in kiosk mode.
        /// In normal mode the bar is always visible; this setting only affects kiosk.
        /// </summary>
        public bool ShowNavigationBar { get; set; }

        /// <summary>
        /// Navigation style for the runtime viewer: "tabs" (default top tabs), "sidebar" (left sidebar),
        /// "hamburger" (collapsible hamburger menu), or "bottom" (bottom tab bar).
        /// </summary>
        [AllowedStringValues("tabs", "sidebar", "hamburger", "bottom")]
        public string NavigationStyle { get; set; } = "tabs";

        /// <summary>
        /// When true, the runtime viewer shows the Tag Browser button in the top bar,
        /// allowing operators to browse, search, read, and write OPC UA tags live.
        /// Default: true.
        /// </summary>
        public bool EnableTagBrowser { get; set; } = true;

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

        /// <summary>
        /// Cloud relay configuration. When set, the RuntimeViewer connects through the
        /// cloud SignalR hub instead of directly to the OPC UA server.
        /// </summary>
        public CloudRelayConfig? CloudRelay { get; set; }

        /// <summary>
        /// Alarm notification configuration. Defines delivery channels (Email, Telegram, WhatsApp)
        /// used when an alarm with NotifyOnActivation fires.
        /// </summary>
        public AlarmNotificationConfig? AlarmNotification { get; set; }

        /// <summary>
        /// GDS (Global Discovery Server) configuration for OPC UA certificate management.
        /// When configured, the server can register with a GDS and receive CA-signed certificates.
        /// </summary>
        public GdsConfig? Gds { get; set; }

        /// <summary>
        /// Automated backup/snapshot configuration. When enabled, the editor creates
        /// ZIP snapshots of the project on save and/or on a schedule.
        /// </summary>
        public BackupConfig? Backup { get; set; }

        /// <summary>REST API configuration (exposes variables, alarms, recipes to external systems).</summary>
        public ApiConfig? Api { get; set; }

        /// <summary>Server redundancy / high-availability configuration.</summary>
        public RedundancyConfig? Redundancy { get; set; }

        /// <summary>Rate limiting / throttling configuration.</summary>
        public RateLimitConfig? RateLimit { get; set; }
    }

    /// <summary>
    /// Configuration for the cloud relay tunnel.
    /// Both the CloudBridge and RuntimeViewer use these settings.
    /// </summary>
    public class CloudRelayConfig
    {
        /// <summary>Whether cloud relay mode is enabled.</summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// URL of the cloud SignalR hub (e.g. "https://myrelay.azurewebsites.net/relay").
        /// Both the bridge and the viewer connect to this URL.
        /// </summary>
        public string HubUrl { get; set; } = "";

        /// <summary>
        /// Shared API key used to authenticate bridge and viewer connections.
        /// Sent as a query-string parameter (?apiKey=…) during the SignalR handshake.
        /// </summary>
        public string ApiKey { get; set; } = "";
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
    /// Configuration for OPC UA Global Discovery Server (GDS) integration.
    /// </summary>
    public class GdsConfig
    {
        /// <summary>OPC UA endpoint URL of the GDS (e.g. "opc.tcp://gds.example.com:58810/GlobalDiscoveryServer").</summary>
        public string EndpointUrl { get; set; } = "";

        /// <summary>Username for GDS authentication (optional — leave empty for anonymous).</summary>
        public string UserName { get; set; } = "";

        /// <summary>Password for GDS authentication.</summary>
        public string Password { get; set; } = "";
    }

    /// <summary>
    /// Configuration for automated project backup snapshots.
    /// </summary>
    public class BackupConfig
    {
        /// <summary>Whether automated backups are enabled.</summary>
        public bool Enabled { get; set; }

        /// <summary>Create a snapshot automatically each time the project is saved.</summary>
        public bool SnapshotOnSave { get; set; } = true;

        /// <summary>
        /// Interval in minutes for scheduled snapshots (0 = disabled).
        /// When > 0, a snapshot is created every N minutes while the project is open.
        /// </summary>
        public int ScheduleMinutes { get; set; }

        /// <summary>
        /// Maximum number of snapshots to keep. Oldest snapshots beyond this limit
        /// are automatically deleted. Default: 20.
        /// </summary>
        public int MaxSnapshots { get; set; } = 20;
    }

    /// <summary>
    /// Configuration for alarm notifications delivered via Email, Telegram, and/or WhatsApp
    /// when an alarm with NotifyOnActivation fires.
    /// </summary>
    public class AlarmNotificationConfig
    {
        /// <summary>Whether alarm notifications are globally enabled.</summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Minimum severity threshold (1–1000) for sending notifications.
        /// Alarms below this severity are silently ignored. Default 1 (all alarms).
        /// </summary>
        public ushort MinSeverity { get; set; } = 1;

        /// <summary>
        /// Cooldown period in seconds between repeated notifications for the same alarm.
        /// Prevents notification flooding during alarm flickering. Default 60 seconds.
        /// </summary>
        public int CooldownSeconds { get; set; } = 60;

        /// <summary>Email channel configuration.</summary>
        public EmailNotificationChannel? Email { get; set; }

        /// <summary>Telegram Bot API channel configuration.</summary>
        public TelegramNotificationChannel? Telegram { get; set; }

        /// <summary>WhatsApp Cloud API channel configuration.</summary>
        public WhatsAppNotificationChannel? WhatsApp { get; set; }
    }

    /// <summary>
    /// Email notification channel using SMTP.
    /// </summary>
    public class EmailNotificationChannel
    {
        /// <summary>Whether email notifications are enabled.</summary>
        public bool Enabled { get; set; }

        /// <summary>SMTP server host (e.g. "smtp.gmail.com").</summary>
        public string SmtpHost { get; set; } = "";

        /// <summary>SMTP port (e.g. 587 for TLS, 465 for SSL).</summary>
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
    }

    /// <summary>
    /// Telegram Bot API notification channel.
    /// Requires a bot token from &#64;BotFather and a chat ID.
    /// </summary>
    public class TelegramNotificationChannel
    {
        /// <summary>Whether Telegram notifications are enabled.</summary>
        public bool Enabled { get; set; }

        /// <summary>Telegram Bot API token (e.g. "123456:ABC-DEF1234ghIkl-zyx57W2v1u123ew11").</summary>
        public string BotToken { get; set; } = "";

        /// <summary>
        /// Target chat ID(s), comma-separated.
        /// Can be a user, group, or channel ID (e.g. "-1001234567890").
        /// </summary>
        public string ChatId { get; set; } = "";

        /// <summary>When true, sends with parse_mode=HTML for formatted messages.</summary>
        public bool UseHtml { get; set; } = true;
    }

    /// <summary>
    /// WhatsApp Cloud API notification channel.
    /// Requires a Meta/WhatsApp Business account with Cloud API access.
    /// </summary>
    public class WhatsAppNotificationChannel
    {
        /// <summary>Whether WhatsApp notifications are enabled.</summary>
        public bool Enabled { get; set; }

        /// <summary>WhatsApp Cloud API access token.</summary>
        public string AccessToken { get; set; } = "";

        /// <summary>WhatsApp Business Phone Number ID (from Meta Business Manager).</summary>
        public string PhoneNumberId { get; set; } = "";

        /// <summary>
        /// Recipient phone number(s) in E.164 format, comma-separated (e.g. "+1234567890").
        /// </summary>
        public string To { get; set; } = "";

        /// <summary>
        /// Optional pre-approved message template name. When set, sends a template message
        /// instead of a free-form text (required for initiating conversations).
        /// </summary>
        public string TemplateName { get; set; } = "";

        /// <summary>Language code for the template (e.g. "en_US"). Default "en_US".</summary>
        public string TemplateLanguage { get; set; } = "en_US";
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
        /// Optional ID referencing an ImageResource entry whose Data contains the base64 data URI.
        /// When set, the image is rendered behind all symbols, covering the entire screen canvas.
        /// </summary>
        public string BackgroundImageId { get; set; } = "";

        /// <summary>Layout mode: "svg" for fixed SVG canvas, "responsive" for responsive HTML grid.</summary>
        [AllowedStringValues("svg", "responsive")]
        public string LayoutMode { get; set; } = "svg";

        /// <summary>Number of grid columns for responsive layout.</summary>
        public int GridColumns { get; set; } = 12;

        /// <summary>Gap between grid cells in pixels (responsive layout).</summary>
        public int GridGap { get; set; } = 8;

        /// <summary>Optional folder path for editor organization (e.g. "Main/Popups"). Ignored by the server.</summary>
        public string Group { get; set; } = "";

        /// <summary>When true (default), this screen appears in the runtime navigation bar/menu.</summary>
        public bool ShowInNavigation { get; set; } = true;

        // --- Editor grid / snap settings ---
        /// <summary>Show the alignment grid in the editor canvas.</summary>
        public bool ShowGrid { get; set; }

        /// <summary>Grid cell size in pixels (editor only).</summary>
        public int EditorGridSize { get; set; } = 20;

        /// <summary>Snap moved/resized symbols to the grid.</summary>
        public bool SnapToGrid { get; set; }

        /// <summary>Enable smart-snap alignment guides between symbols.</summary>
        public bool SmartSnap { get; set; }

        public List<ScreenSymbol> Symbols { get; set; } = new();
    }

    public class ScreenSymbol
    {
        public string Id { get; set; } = "";
        public string Type { get; set; } = "rect"; // rect, circle, ellipse, text, line, gauge, indicator, svg, alarmlist, hdachart, hdagrid, eventlog, editbox, ipcamera, recipe, weeklyplanner, screenembed, reportviewer, imagemap, trend, progressbar, numericdisplay, ledarray, pipe, tank, dropdown, datatable, sparkline, motorcontrol, valve, alarmbanner, colorzone, conveyor, piechart, barchart, navbutton, heatexchanger, popup, setpointramp, flowmeter, xyplot, pdfviewer, switch, rotaryswitch, knob, hslider, vslider, button, animtext
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; } = 80;
        public double Height { get; set; } = 40;
        public string Fill { get; set; } = "#4a90d9";
        public string Stroke { get; set; } = "#333333";
        public double StrokeWidth { get; set; } = 1;
        public string Label { get; set; } = "";
        public double Rotation { get; set; }


        // --- Font properties ---
        /// <summary>Font family for labels/text. Empty = "sans-serif" (default).</summary>
        public string FontFamily { get; set; } = "";

        /// <summary>Font size in SVG units (px). 0 = use default (14 for text, 11 for labels, etc.).</summary>
        public int FontSize { get; set; }

        /// <summary>Font weight: "" (default/normal), "bold", "lighter", "100"-"900".</summary>
        public string FontWeight { get; set; } = "";

        /// <summary>Font style: "" (default/normal), "italic", "oblique".</summary>
        public string FontStyle { get; set; } = "";
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

        /// <summary>When true, the EditBox widget shows a statistics bar (Min/Max/Avg) read from the variable's .Statistics.* OPC sub-variables.</summary>
        public bool EditBoxShowStatistics { get; set; }

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

        /// <summary>Scheduler name to bind this widget to (Type == "weeklyplanner"). Must match a SchedulerConfig.Name.</summary>
        public string SchedulerName { get; set; } = "";

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

        // --- Toggle/Rotary Switch properties ---
        /// <summary>Switch visual style (Type == "switch"): "apple" (iOS pill toggle, default), "flat" (minimal).</summary>
        public string SwitchStyle { get; set; } = "apple";

        /// <summary>Color when the toggle switch is ON. Default green.</summary>
        public string SwitchOnColor { get; set; } = "#34c759";

        /// <summary>Color when the toggle switch is OFF. Default gray.</summary>
        public string SwitchOffColor { get; set; } = "#787880";

        /// <summary>Number of discrete positions for a rotary switch (Type == "rotaryswitch"). Min 2, max 12. Default 3.</summary>
        public int SwitchPositions { get; set; } = 3;

        /// <summary>Comma-separated labels for each rotary position, e.g. "Off,Low,Med,High".</summary>
        public string SwitchPositionLabels { get; set; } = "";

        /// <summary>Color of the rotary switch knob. Default "#e0e0e0".</summary>
        public string SwitchKnobColor { get; set; } = "#e0e0e0";

        /// <summary>Color of the rotary switch dial/body. Default "#2d2d2d".</summary>
        public string SwitchDialColor { get; set; } = "#2d2d2d";

        /// <summary>Color of the knob body (Type == "knob"). Default "#3a3a3a".</summary>
        public string KnobColor { get; set; } = "#3a3a3a";

        /// <summary>Color of the knob pointer/indicator line. Default "#ff6b35".</summary>
        public string KnobPointerColor { get; set; } = "#ff6b35";

        /// <summary>Whether to display the scale tick marks around the knob. Default true.</summary>
        public bool KnobShowScale { get; set; } = true;

        /// <summary>Whether to show the numeric value readout below the knob. Default true.</summary>
        public bool KnobShowValue { get; set; } = true;

        /// <summary>Unit suffix for knob value display (e.g. "%", "rpm"). Default "".</summary>
        public string KnobUnit { get; set; } = "";

        /// <summary>Color of the slider track (Type == "hslider" or "vslider"). Default "#444".</summary>
        public string SliderTrackColor { get; set; } = "#444";

        /// <summary>Color of the filled portion of the slider track. Default "#3b82f6".</summary>
        public string SliderFillColor { get; set; } = "#3b82f6";

        /// <summary>Color of the slider thumb/pointer. Default "#ffffff".</summary>
        public string SliderThumbColor { get; set; } = "#ffffff";

        /// <summary>Whether to show the numeric value on the slider thumb. Default true.</summary>
        public bool SliderShowValue { get; set; } = true;

        /// <summary>Whether to show tick marks on the slider scale. Default true.</summary>
        public bool SliderShowTicks { get; set; } = true;

        // --- Command Button properties (Type == "button") ---

        /// <summary>
        /// Button visual style. Supported values:
        /// "raised" - 3D raised button with gradient and shadow (default),
        /// "flat" - flat filled button with no shadow,
        /// "outline" - transparent with colored border,
        /// "pill" - fully rounded ends (capsule shape),
        /// "glass" - semi-transparent frosted-glass look,
        /// "danger" - red raised button for destructive actions,
        /// "success" - green raised button for confirmations,
        /// "warning" - amber raised button for caution actions,
        /// "icon" - compact square/circle button for icon-only use.
        /// </summary>
        public string ButtonStyle { get; set; } = "raised";

        /// <summary>Primary button color (background for raised/flat/pill, border for outline). Default "#3b82f6".</summary>
        public string ButtonColor { get; set; } = "#3b82f6";

        /// <summary>Button text/icon color. Default "#ffffff".</summary>
        public string ButtonTextColor { get; set; } = "#ffffff";

        /// <summary>Optional icon/emoji placed inside the button alongside the label. Default "".</summary>
        public string ButtonIcon { get; set; } = "";

        /// <summary>Icon position relative to the label: "left" (default), "right", "top", "center" (icon-only).</summary>
        public string ButtonIconPosition { get; set; } = "left";

        /// <summary>Border radius in pixels. 0 = square, 999 = fully rounded. Default 6.</summary>
        public int ButtonBorderRadius { get; set; } = 6;


        // --- Animated Text properties (Type == "animtext") ---

        /// <summary>
        /// Animation mode for the animated text widget. Supported values:
        /// "scroll" — horizontal marquee scrolling (default),
        /// "flash" — text and/or background color alternates between two colors,
        /// "pulse" — smooth opacity pulsing,
        /// "typewriter" — text appears letter-by-letter then resets,
        /// "fade" — crossfade between messages in the list.
        /// </summary>
        public string AnimTextMode { get; set; } = "scroll";

        /// <summary>Animation speed in seconds per cycle. Default 5.</summary>
        public double AnimTextSpeed { get; set; } = 5;

        /// <summary>Secondary text color used for flash animation alternation. Default "#ff4444".</summary>
        public string AnimTextColor2 { get; set; } = "#ff4444";

        /// <summary>Background color of the animated text area. Empty = transparent. Default "".</summary>
        public string AnimTextBackground { get; set; } = "";

        /// <summary>Secondary background color for flash animation. Default "".</summary>
        public string AnimTextBackground2 { get; set; } = "";

        /// <summary>
        /// Newline-separated list of messages to cycle through (for scroll/fade modes).
        /// Each message is localized via @-prefixed string IDs.
        /// Empty = use the Label property as the single message.
        /// </summary>
        public string AnimTextMessages { get; set; } = "";

        /// <summary>Border radius of the animated text background box. Default 4.</summary>
        public int AnimTextBorderRadius { get; set; } = 4;

        /// <summary>Whether to show a border around the widget. Default false.</summary>
        public bool AnimTextShowBorder { get; set; }

        // --- Report Viewer widget (Type == "reportviewer") --------
        /// <summary>Report definition name (must match a ReportConfig.Name).</summary>
        public string ReportName { get; set; } = "";

        // ── Progress Bar ──
        /// <summary>Progress bar orientation: "horizontal" or "vertical".</summary>
        public string ProgressBarOrientation { get; set; } = "horizontal";
        /// <summary>Fill color for the progress portion.</summary>
        public string ProgressBarFillColor { get; set; } = "#22c55e";
        /// <summary>Background/track color.</summary>
        public string ProgressBarTrackColor { get; set; } = "#334155";
        /// <summary>Whether to show percentage text overlay.</summary>
        public bool ProgressBarShowText { get; set; } = true;
        /// <summary>Minimum scale value (default 0).</summary>
        public double ProgressBarMin { get; set; } = 0;
        /// <summary>Maximum scale value (default 100).</summary>
        public double ProgressBarMax { get; set; } = 100;
        /// <summary>Border radius in px.</summary>
        public int ProgressBarBorderRadius { get; set; } = 4;

        // ── Numeric Display ──
        /// <summary>Number of decimal places.</summary>
        public int NumericDisplayDecimals { get; set; } = 1;
        /// <summary>Engineering unit label (e.g. "°C", "bar").</summary>
        public string NumericDisplayUnit { get; set; } = "";
        /// <summary>Text color.</summary>
        public string NumericDisplayColor { get; set; } = "#e2e8f0";
        /// <summary>Background color.</summary>
        public string NumericDisplayBackground { get; set; } = "#1e293b";
        /// <summary>Low warning threshold; value below this shows warning color.</summary>
        public double NumericDisplayLowWarn { get; set; } = -1;
        /// <summary>High warning threshold; value above this shows warning color.</summary>
        public double NumericDisplayHighWarn { get; set; } = -1;
        /// <summary>Warning-zone color.</summary>
        public string NumericDisplayWarnColor { get; set; } = "#f59e0b";

        // ── LED Array ──
        /// <summary>Number of LEDs in the array.</summary>
        public int LedArrayCount { get; set; } = 8;
        /// <summary>Layout: "row" or "column".</summary>
        public string LedArrayLayout { get; set; } = "row";
        /// <summary>LED color when bit is ON.</summary>
        public string LedArrayOnColor { get; set; } = "#22c55e";
        /// <summary>LED color when bit is OFF.</summary>
        public string LedArrayOffColor { get; set; } = "#334155";
        /// <summary>LED shape: "circle" or "square".</summary>
        public string LedArrayShape { get; set; } = "circle";

        // ── Pipe ──
        /// <summary>Flow direction: "left-right", "right-left", "top-bottom", "bottom-top".</summary>
        public string PipeFlowDirection { get; set; } = "left-right";
        /// <summary>Pipe body color.</summary>
        public string PipeColor { get; set; } = "#64748b";
        /// <summary>Fluid/flow indicator color.</summary>
        public string PipeFluidColor { get; set; } = "#3b82f6";
        /// <summary>Pipe wall thickness in px.</summary>
        public double PipeThickness { get; set; } = 6;
        /// <summary>Animate the flow when value is truthy.</summary>
        public bool PipeAnimate { get; set; } = true;

        // ── Tank ──
        /// <summary>Tank fill color.</summary>
        public string TankFillColor { get; set; } = "#3b82f6";
        /// <summary>Tank body/shell color.</summary>
        public string TankBodyColor { get; set; } = "#1e293b";
        /// <summary>Scale minimum (empty).</summary>
        public double TankMin { get; set; } = 0;
        /// <summary>Scale maximum (full).</summary>
        public double TankMax { get; set; } = 100;
        /// <summary>Show level percentage text.</summary>
        public bool TankShowLevel { get; set; } = true;
        /// <summary>Engineering unit label.</summary>
        public string TankUnit { get; set; } = "%";

        // ── Dropdown ──
        /// <summary>Semicolon-separated options: "Label=Value;Label2=Value2".</summary>
        public string DropdownOptions { get; set; } = "";
        /// <summary>Background color for the select element.</summary>
        public string DropdownBackground { get; set; } = "#1e293b";
        /// <summary>Text color.</summary>
        public string DropdownTextColor { get; set; } = "#e2e8f0";

        // ── Data Table ──
        /// <summary>Semicolon-separated column definitions: "Header=VarPath;Header2=VarPath2".</summary>
        public string DataTableColumns { get; set; } = "";
        /// <summary>Header background color.</summary>
        public string DataTableHeaderBg { get; set; } = "#1e293b";
        /// <summary>Row background color.</summary>
        public string DataTableRowBg { get; set; } = "#0f172a";
        /// <summary>Text color.</summary>
        public string DataTableTextColor { get; set; } = "#e2e8f0";
        /// <summary>Show grid borders.</summary>
        public bool DataTableShowBorders { get; set; } = true;

        // ── Sparkline ──
        /// <summary>Line color.</summary>
        public string SparklineColor { get; set; } = "#3b82f6";
        /// <summary>Number of data points to retain.</summary>
        public int SparklineMaxPoints { get; set; } = 50;
        /// <summary>Show current value text.</summary>
        public bool SparklineShowValue { get; set; } = true;
        /// <summary>Fill area under the line.</summary>
        public bool SparklineFillArea { get; set; }
        /// <summary>Line stroke width.</summary>
        public double SparklineStrokeWidth { get; set; } = 1.5;

        // ── Motor Control ──
        /// <summary>Motor symbol type: "motor", "pump", "fan".</summary>
        public string MotorControlStyle { get; set; } = "motor";
        /// <summary>Color when running.</summary>
        public string MotorControlRunColor { get; set; } = "#22c55e";
        /// <summary>Color when stopped.</summary>
        public string MotorControlStopColor { get; set; } = "#64748b";
        /// <summary>Color when faulted.</summary>
        public string MotorControlFaultColor { get; set; } = "#ef4444";
        /// <summary>Variable path for fault state.</summary>
        public string MotorControlFaultPath { get; set; } = "";
        /// <summary>Show RPM or speed value.</summary>
        public bool MotorControlShowSpeed { get; set; }
        /// <summary>Variable path for speed/RPM.</summary>
        public string MotorControlSpeedPath { get; set; } = "";

        // ── Valve ──
        /// <summary>Valve type: "gate", "ball", "butterfly".</summary>
        public string ValveStyle { get; set; } = "gate";
        /// <summary>Color when open.</summary>
        public string ValveOpenColor { get; set; } = "#22c55e";
        /// <summary>Color when closed.</summary>
        public string ValveClosedColor { get; set; } = "#ef4444";
        /// <summary>Color for intermediate/traveling state.</summary>
        public string ValveTransitColor { get; set; } = "#f59e0b";
        /// <summary>Orientation: "horizontal" or "vertical".</summary>
        public string ValveOrientation { get; set; } = "horizontal";
        /// <summary>Variable path for position feedback (0-100%).</summary>
        public string ValvePositionPath { get; set; } = "";

        // ── Alarm Banner ──
        /// <summary>Banner display style: "compact" or "detailed".</summary>
        public string AlarmBannerStyle { get; set; } = "compact";
        /// <summary>Show alarm count badge.</summary>
        public bool AlarmBannerShowCount { get; set; } = true;
        /// <summary>Flash on critical alarm.</summary>
        public bool AlarmBannerFlash { get; set; } = true;

        // ── Color Zone ──
        /// <summary>Default background color when no rule matches.</summary>
        public string ColorZoneDefault { get; set; } = "#334155";
        /// <summary>Border radius in px.</summary>
        public int ColorZoneBorderRadius { get; set; } = 4;
        /// <summary>Semicolon-separated color rules: ">80 #ef4444;>=20 #22c55e;>=0 #3b82f6".</summary>
        public string ColorZoneRules { get; set; } = "";
        /// <summary>Show value text overlay.</summary>
        public bool ColorZoneShowValue { get; set; }

        // ── Conveyor ──
        /// <summary>Conveyor orientation: "horizontal" or "vertical".</summary>
        public string ConveyorOrientation { get; set; } = "horizontal";
        /// <summary>Belt/roller color.</summary>
        public string ConveyorBeltColor { get; set; } = "#475569";
        /// <summary>Running-state indicator color.</summary>
        public string ConveyorRunColor { get; set; } = "#22c55e";
        /// <summary>Fault-state indicator color.</summary>
        public string ConveyorFaultColor { get; set; } = "#ef4444";
        /// <summary>Animation speed multiplier (1 = normal).</summary>
        public double ConveyorSpeed { get; set; } = 1;
        /// <summary>Variable path for fault state (1/true = fault).</summary>
        public string ConveyorFaultPath { get; set; } = "";

        // ── Pie Chart ──
        /// <summary>Pie chart style: "pie" or "doughnut".</summary>
        public string PieChartStyle { get; set; } = "pie";
        /// <summary>Inner radius ratio for doughnut (0-0.9).</summary>
        public double PieChartInnerRadius { get; set; } = 0.5;
        /// <summary>Show legend alongside chart.</summary>
        public bool PieChartShowLegend { get; set; } = true;
        /// <summary>Show percentage labels on slices.</summary>
        public bool PieChartShowPercentage { get; set; }
        /// <summary>Slice definitions: "Label=VarPath #color;...".</summary>
        public string PieChartSlices { get; set; } = "";

        // ── Bar Chart ──
        /// <summary>Bar orientation: "vertical" or "horizontal".</summary>
        public string BarChartOrientation { get; set; } = "vertical";
        /// <summary>Maximum scale value.</summary>
        public double BarChartMax { get; set; } = 100;
        /// <summary>Show value labels on bars.</summary>
        public bool BarChartShowValues { get; set; } = true;
        /// <summary>Show grid lines.</summary>
        public bool BarChartShowGrid { get; set; } = true;
        /// <summary>Bar definitions: "Label=VarPath #color;...".</summary>
        public string BarChartBars { get; set; } = "";

        // ── Nav Button ──
        /// <summary>Target screen name/path for navigation.</summary>
        public string NavButtonTarget { get; set; } = "";
        /// <summary>Button style: "filled", "outline", "ghost".</summary>
        public string NavButtonStyle { get; set; } = "filled";
        /// <summary>Icon character/emoji displayed on button.</summary>
        public string NavButtonIcon { get; set; } = ">";
        /// <summary>Icon position: "left", "right", "none".</summary>
        public string NavButtonIconPosition { get; set; } = "left";
        /// <summary>Button background color. Default "#3b82f6".</summary>
        public string NavButtonColor { get; set; } = "#3b82f6";
        /// <summary>Button text color. Default "#ffffff".</summary>
        public string NavButtonTextColor { get; set; } = "#ffffff";

        // ── Heat Exchanger ──
        /// <summary>Hot-side color. Default "#ef4444".</summary>
        public string HeatExchangerHotColor { get; set; } = "#ef4444";
        /// <summary>Cold-side color. Default "#3b82f6".</summary>
        public string HeatExchangerColdColor { get; set; } = "#3b82f6";
        /// <summary>Shell body color. Default "#64748b".</summary>
        public string HeatExchangerBodyColor { get; set; } = "#64748b";
        /// <summary>Show temperature labels on hot/cold sides.</summary>
        public bool HeatExchangerShowTemps { get; set; } = true;
        /// <summary>Variable path for hot-side inlet temperature.</summary>
        public string HeatExchangerHotInPath { get; set; } = "";
        /// <summary>Variable path for cold-side inlet temperature.</summary>
        public string HeatExchangerColdInPath { get; set; } = "";
        /// <summary>Orientation: "horizontal" or "vertical".</summary>
        public string HeatExchangerOrientation { get; set; } = "horizontal";

        // ── Popup ──
        /// <summary>Trigger mode: "hover", "click", or "variable".</summary>
        public string PopupTrigger { get; set; } = "hover";
        /// <summary>Content text displayed inside the popup.</summary>
        public string PopupContent { get; set; } = "";
        /// <summary>Popup width in pixels.</summary>
        public double PopupWidth { get; set; } = 200;
        /// <summary>Popup height in pixels.</summary>
        public double PopupHeight { get; set; } = 120;
        /// <summary>Popup background color.</summary>
        public string PopupBackground { get; set; } = "#1e293b";
        /// <summary>Popup border color.</summary>
        public string PopupBorderColor { get; set; } = "#475569";
        /// <summary>Position relative to parent: "top", "bottom", "left", "right".</summary>
        public string PopupPosition { get; set; } = "top";
        /// <summary>Semicolon-separated variable paths shown inside popup.</summary>
        public string PopupVariables { get; set; } = "";

        // ── Setpoint Ramp ──
        /// <summary>Current-value line color.</summary>
        public string SetpointColor { get; set; } = "#22c55e";
        /// <summary>Target-value line color.</summary>
        public string SetpointTargetColor { get; set; } = "#f97316";
        /// <summary>Variable path for ramp rate.</summary>
        public string SetpointRatePath { get; set; } = "";
        /// <summary>Variable path for target setpoint.</summary>
        public string SetpointTargetPath { get; set; } = "";
        /// <summary>Scale minimum.</summary>
        public double SetpointMin { get; set; } = 0;
        /// <summary>Scale maximum.</summary>
        public double SetpointMax { get; set; } = 100;
        /// <summary>Show ramp rate value.</summary>
        public bool SetpointShowRate { get; set; } = true;
        /// <summary>Engineering unit label.</summary>
        public string SetpointUnit { get; set; } = "";

        // ── Flow Meter ──
        /// <summary>Visual style: "circular", "digital".</summary>
        public string FlowMeterStyle { get; set; } = "circular";
        /// <summary>Body/gauge color.</summary>
        public string FlowMeterBodyColor { get; set; } = "#334155";
        /// <summary>Flow indicator color.</summary>
        public string FlowMeterFlowColor { get; set; } = "#3b82f6";
        /// <summary>Variable path for totalizer value.</summary>
        public string FlowMeterTotalizerPath { get; set; } = "";
        /// <summary>Flow rate engineering unit.</summary>
        public string FlowMeterUnit { get; set; } = "L/min";
        /// <summary>Totalizer engineering unit.</summary>
        public string FlowMeterTotalizerUnit { get; set; } = "L";
        /// <summary>Show totalizer readout.</summary>
        public bool FlowMeterShowTotalizer { get; set; } = true;
        /// <summary>Orientation: "horizontal" or "vertical".</summary>
        public string FlowMeterOrientation { get; set; } = "horizontal";

        // ── XY Plot ──
        /// <summary>Variable path for X-axis data.</summary>
        public string XYPlotXPath { get; set; } = "";
        /// <summary>Variable path for Y-axis data.</summary>
        public string XYPlotYPath { get; set; } = "";
        /// <summary>Trace line color.</summary>
        public string XYPlotColor { get; set; } = "#3b82f6";
        /// <summary>Max data points to retain.</summary>
        public int XYPlotMaxPoints { get; set; } = 200;
        /// <summary>Show grid lines.</summary>
        public bool XYPlotShowGrid { get; set; } = true;
        /// <summary>Show axis lines and labels.</summary>
        public bool XYPlotShowAxes { get; set; } = true;
        /// <summary>Plot background color.</summary>
        public string XYPlotBackground { get; set; } = "#0f172a";
        /// <summary>Trace stroke width.</summary>
        public double XYPlotStrokeWidth { get; set; } = 1.5;
        /// <summary>X-axis label text.</summary>
        public string XYPlotXLabel { get; set; } = "X";
        /// <summary>Y-axis label text.</summary>
        public string XYPlotYLabel { get; set; } = "Y";

        // ── PDF Viewer ──
        /// <summary>PDF source type: "url" or "embedded".</summary>
        public string PdfViewerSource { get; set; } = "url";
        /// <summary>URL of the PDF document.</summary>
        public string PdfViewerUrl { get; set; } = "";
        /// <summary>Show the PDF toolbar.</summary>
        public bool PdfViewerShowToolbar { get; set; } = true;
        /// <summary>Background color for the viewer frame.</summary>
        public string PdfViewerBackground { get; set; } = "#1e293b";

        // ── Additional aliases / properties used by prior widget code ──
        public bool ProgressBarShowValue { get; set; } = true;
        public string NumericDisplayFormat { get; set; } = "F1";
        public int NumericDisplayDigits { get; set; } = 5;
        public string LedArrayOrientation { get; set; } = "horizontal";
        public string LedArrayLabels { get; set; } = "";
        public string PipeFlowColor { get; set; } = "#3b82f6";
        public bool PipeShowFlow { get; set; } = true;
        public double PipeFlowSpeed { get; set; } = 1;
        public string PipeOrientation { get; set; } = "horizontal";
        public string TankLiquidColor { get; set; } = "#3b82f6";
        public string TankStyle { get; set; } = "rectangular";
        public string MotorRunColor { get; set; } = "#22c55e";
        public string MotorStopColor { get; set; } = "#64748b";
        public string MotorFaultColor { get; set; } = "#ef4444";
        public string MotorFaultPath { get; set; } = "";
        public bool MotorAnimateRotation { get; set; } = true;
        public string MotorStyle { get; set; } = "motor";
        public int SparklinePoints { get; set; } = 50;
        public string DropdownValues { get; set; } = "";
        public string DataTableBackground { get; set; } = "#0f172a";
        public bool DataTableShowHeader { get; set; } = true;
        public int DataTableMaxRows { get; set; } = 10;
        public string ConveyorFrameColor { get; set; } = "#334155";
        public bool ConveyorAnimate { get; set; } = true;
        public string PieChartBackground { get; set; } = "#0f172a";
        public bool PieChartShowLabels { get; set; } = true;
        public string BarChartBackground { get; set; } = "#0f172a";
        public double BarChartMaxValue { get; set; } = 100;

        // ─── KPI Widget properties (Type == "kpiwidget") ────────────
        public string KpiMode { get; set; } = "oee";
        public string KpiTitle { get; set; } = "";
        public string KpiUnit { get; set; } = "%";
        public string KpiBackground { get; set; } = "#0f172a";
        public string KpiGaugeColor { get; set; } = "#22c55e";
        public double KpiTargetValue { get; set; } = 100;
        public bool KpiShowTitle { get; set; } = true;
        public bool KpiShowValue { get; set; } = true;
        public bool KpiShowTarget { get; set; } = true;
        public string KpiAvailabilityPath { get; set; } = "";
        public string KpiPerformancePath { get; set; } = "";
        public string KpiQualityPath { get; set; } = "";
        public string KpiUptimePath { get; set; } = "";
        public string KpiThroughputPath { get; set; } = "";
        public string KpiThroughputTargetPath { get; set; } = "";
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
        /// ChangeLanguage,
        /// GenerateReport
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

        /// <summary>Target report name (for GenerateReport).</summary>
        public string TargetReport { get; set; } = "";
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

    /// <summary>
    /// A reusable image resource. The Id is used to reference the image from screens and widgets,
    /// and Data stores the raster image as a base64 data URI (e.g. "data:image/png;base64,...").
    /// </summary>
    public class ImageResource
    {
        public string Id { get; set; } = "";
        public string Data { get; set; } = "";
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

        /// <summary>Live debug snapshots for running PLC programs and scripts.</summary>
        public List<ProgramDebugInfo> ProgramDebug { get; set; } = new();

        /// <summary>Alarm analytics snapshot (top-N, MTTA, flood detection). Null when no alarm data is available.</summary>
        public AlarmAnalyticsSnapshot? AlarmAnalytics { get; set; }
    }

    // ─── Alarm Analytics ─────────────────────────────────────────

    /// <summary>
    /// Aggregated alarm analytics: top-N most-frequent alarms, mean-time-to-acknowledge,
    /// and alarm-flood detection over a sliding window.
    /// </summary>
    public class AlarmAnalyticsSnapshot
    {
        /// <summary>UTC timestamp when the snapshot was computed.</summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>Total alarm activations tracked in the current window.</summary>
        public int TotalActivations { get; set; }

        /// <summary>Total alarm acknowledgements tracked in the current window.</summary>
        public int TotalAcknowledgements { get; set; }

        /// <summary>Top-10 most frequently activated alarms.</summary>
        public List<AlarmFrequencyEntry> TopAlarms { get; set; } = new();

        /// <summary>
        /// Mean-time-to-acknowledge in seconds across all alarms that were acknowledged in the window.
        /// Null when no acknowledgements were recorded.
        /// </summary>
        public double? MeanTimeToAcknowledgeSeconds { get; set; }

        /// <summary>Whether an alarm flood is currently detected.</summary>
        public bool IsFloodDetected { get; set; }

        /// <summary>Number of alarm activations in the most recent flood-detection window (e.g. last 60 s).</summary>
        public int FloodWindowActivations { get; set; }

        /// <summary>Flood-detection threshold (activations per FloodWindowSeconds).</summary>
        public int FloodThreshold { get; set; }

        /// <summary>Size of the flood-detection sliding window in seconds.</summary>
        public int FloodWindowSeconds { get; set; }

        /// <summary>Sliding analytics window size in minutes.</summary>
        public int WindowMinutes { get; set; }
    }

    /// <summary>A single alarm source with its activation count in the analytics window.</summary>
    public class AlarmFrequencyEntry
    {
        /// <summary>Variable path of the alarm source.</summary>
        public string VariablePath { get; set; } = "";

        /// <summary>Alarm message template.</summary>
        public string Message { get; set; } = "";

        /// <summary>Number of activations in the window.</summary>
        public int ActivationCount { get; set; }

        /// <summary>Average time-to-acknowledge in seconds for this source. Null if never acknowledged.</summary>
        public double? AvgAcknowledgeSeconds { get; set; }
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

    /// <summary>
    /// Debug snapshot for a running PLC program or script, showing live variable values
    /// and execution flow information. Captured after each execution cycle.
    /// </summary>
    public class ProgramDebugInfo
    {
        /// <summary>Category: "PlcProgram" or "Script".</summary>
        public string Category { get; set; } = "";

        /// <summary>Name of the program or script.</summary>
        public string Name { get; set; } = "";

        /// <summary>Current local and OPC variable values: name -> string representation.</summary>
        public Dictionary<string, string> Variables { get; set; } = new();

        /// <summary>Execution flow trace: list of recently executed source line numbers.</summary>
        public List<int> ExecutedLines { get; set; } = new();

        /// <summary>Zero-based line numbers of OPC Write operations in the last cycle.</summary>
        public List<int> WriteLines { get; set; } = new();

        /// <summary>Current cycle count.</summary>
        public long CycleCount { get; set; }

        /// <summary>Status: "Running", "Idle", "Error".</summary>
        public string Status { get; set; } = "Running";

        /// <summary>Last error message, if any.</summary>
        public string? LastError { get; set; }

        /// <summary>Per-line debug annotations: 0-based line number -> display text (variable values at that line).</summary>
        public Dictionary<int, string> LineAnnotations { get; set; } = new();

        /// <summary>Active debug session state (breakpoints, pause state, watch variables). Null when not debugging.</summary>
        public ScriptDebugSession? DebugSession { get; set; }
    }

    // ─── Scheduler ───────────────────────────────────────────

    /// <summary>
    /// Configuration for a time-based scheduler that executes commands at scheduled times.
    /// Supports weekly recurring schedules with different behavior for weekdays, weekends, and holidays.
    /// </summary>
    public class SchedulerConfig
    {
        public string Name { get; set; } = "";
        public bool Enabled { get; set; } = true;

        /// <summary>When true, runtime users can edit the schedule from the Weekly Planner widget.</summary>
        public bool AllowRuntimeEdit { get; set; }

        /// <summary>Time slot granularity in minutes (15 or 60). Default 60.</summary>
        public int SlotMinutes { get; set; } = 60;

        /// <summary>Commands executed when a scheduled time slot becomes active.</summary>
        public List<SymbolCommand> Commands { get; set; } = new();

        /// <summary>Optional commands executed when leaving a scheduled time slot (slot becomes inactive).</summary>
        public List<SymbolCommand> DeactivateCommands { get; set; } = new();

        /// <summary>Weekly time slots where the scheduler is active.</summary>
        public List<WeeklyTimeSlot> WeeklySlots { get; set; } = new();

        /// <summary>Behavior on weekends: "Same" (use weekday schedule), "Off" (no execution), "Custom" (use WeekendSlots).</summary>
        public string WeekendMode { get; set; } = "Same";

        /// <summary>Custom weekend time slots (used when WeekendMode == "Custom").</summary>
        public List<WeeklyTimeSlot> WeekendSlots { get; set; } = new();

        /// <summary>Behavior on holidays: "Same" (use normal schedule), "Off" (no execution), "Weekend" (use weekend schedule).</summary>
        public string HolidayMode { get; set; } = "Same";

        /// <summary>Locale code for the built-in holiday calendar (e.g. "US", "DE", "IT", "FR", "UK", "ES", "JP"). Empty = none.</summary>
        public string HolidayLocale { get; set; } = "";

        /// <summary>Additional custom holidays (dates without year recur every year).</summary>
        public List<HolidayEntry> CustomHolidays { get; set; } = new();
    }

    /// <summary>
    /// A weekly time slot defining an active period.
    /// DayOfWeek 0=Sunday … 6=Saturday.
    /// StartMinute and EndMinute are minutes from midnight (0–1440).
    /// </summary>
    public class WeeklyTimeSlot
    {
        public int DayOfWeek { get; set; }
        public int StartMinute { get; set; }
        public int EndMinute { get; set; }
    }

    /// <summary>
    /// A holiday entry. When Year is 0, the holiday recurs every year.
    /// </summary>
    public class HolidayEntry
    {
        public string Name { get; set; } = "";
        public int Month { get; set; }
        public int Day { get; set; }
        /// <summary>Specific year. 0 = every year.</summary>
        public int Year { get; set; }
    }

    /// <summary>
    /// Provides built-in holiday lists by locale.
    /// </summary>
    public static class HolidayCalendars
    {
        public static readonly Dictionary<string, List<HolidayEntry>> BuiltIn = new(StringComparer.OrdinalIgnoreCase)
        {
            ["US"] = new()
            {
                new() { Name = "New Year's Day", Month = 1, Day = 1 },
                new() { Name = "Independence Day", Month = 7, Day = 4 },
                new() { Name = "Veterans Day", Month = 11, Day = 11 },
                new() { Name = "Christmas Day", Month = 12, Day = 25 },
            },
            ["DE"] = new()
            {
                new() { Name = "Neujahrstag", Month = 1, Day = 1 },
                new() { Name = "Tag der Arbeit", Month = 5, Day = 1 },
                new() { Name = "Tag der Deutschen Einheit", Month = 10, Day = 3 },
                new() { Name = "Weihnachtstag", Month = 12, Day = 25 },
                new() { Name = "2. Weihnachtstag", Month = 12, Day = 26 },
            },
            ["IT"] = new()
            {
                new() { Name = "Capodanno", Month = 1, Day = 1 },
                new() { Name = "Epifania", Month = 1, Day = 6 },
                new() { Name = "Festa della Liberazione", Month = 4, Day = 25 },
                new() { Name = "Festa del Lavoro", Month = 5, Day = 1 },
                new() { Name = "Festa della Repubblica", Month = 6, Day = 2 },
                new() { Name = "Ferragosto", Month = 8, Day = 15 },
                new() { Name = "Tutti i Santi", Month = 11, Day = 1 },
                new() { Name = "Immacolata Concezione", Month = 12, Day = 8 },
                new() { Name = "Natale", Month = 12, Day = 25 },
                new() { Name = "Santo Stefano", Month = 12, Day = 26 },
            },
            ["FR"] = new()
            {
                new() { Name = "Jour de l'An", Month = 1, Day = 1 },
                new() { Name = "Fête du Travail", Month = 5, Day = 1 },
                new() { Name = "Victoire 1945", Month = 5, Day = 8 },
                new() { Name = "Fête nationale", Month = 7, Day = 14 },
                new() { Name = "Assomption", Month = 8, Day = 15 },
                new() { Name = "Toussaint", Month = 11, Day = 1 },
                new() { Name = "Armistice", Month = 11, Day = 11 },
                new() { Name = "Noël", Month = 12, Day = 25 },
            },
            ["UK"] = new()
            {
                new() { Name = "New Year's Day", Month = 1, Day = 1 },
                new() { Name = "Christmas Day", Month = 12, Day = 25 },
                new() { Name = "Boxing Day", Month = 12, Day = 26 },
            },
            ["ES"] = new()
            {
                new() { Name = "Año Nuevo", Month = 1, Day = 1 },
                new() { Name = "Epifanía", Month = 1, Day = 6 },
                new() { Name = "Día del Trabajo", Month = 5, Day = 1 },
                new() { Name = "Asunción", Month = 8, Day = 15 },
                new() { Name = "Fiesta Nacional", Month = 10, Day = 12 },
                new() { Name = "Todos los Santos", Month = 11, Day = 1 },
                new() { Name = "Constitución", Month = 12, Day = 6 },
                new() { Name = "Inmaculada Concepción", Month = 12, Day = 8 },
                new() { Name = "Navidad", Month = 12, Day = 25 },
            },
            ["JP"] = new()
            {
                new() { Name = "元日", Month = 1, Day = 1 },
                new() { Name = "成人の日", Month = 1, Day = 8 },
                new() { Name = "建国記念の日", Month = 2, Day = 11 },
                new() { Name = "天皇誕生日", Month = 2, Day = 23 },
                new() { Name = "昭和の日", Month = 4, Day = 29 },
                new() { Name = "憲法記念日", Month = 5, Day = 3 },
                new() { Name = "みどりの日", Month = 5, Day = 4 },
                new() { Name = "こどもの日", Month = 5, Day = 5 },
                new() { Name = "文化の日", Month = 11, Day = 3 },
                new() { Name = "勤労感謝の日", Month = 11, Day = 23 },
            },
            ["BR"] = new()
            {
                new() { Name = "Confraternização Universal", Month = 1, Day = 1 },
                new() { Name = "Tiradentes", Month = 4, Day = 21 },
                new() { Name = "Dia do Trabalho", Month = 5, Day = 1 },
                new() { Name = "Independência", Month = 9, Day = 7 },
                new() { Name = "Nossa Sra. Aparecida", Month = 10, Day = 12 },
                new() { Name = "Finados", Month = 11, Day = 2 },
                new() { Name = "Proclamação da República", Month = 11, Day = 15 },
                new() { Name = "Natal", Month = 12, Day = 25 },
            },
            ["CA"] = new()
            {
                new() { Name = "New Year's Day", Month = 1, Day = 1 },
                new() { Name = "Canada Day", Month = 7, Day = 1 },
                new() { Name = "Remembrance Day", Month = 11, Day = 11 },
                new() { Name = "Christmas Day", Month = 12, Day = 25 },
            },
            ["AU"] = new()
            {
                new() { Name = "New Year's Day", Month = 1, Day = 1 },
                new() { Name = "Australia Day", Month = 1, Day = 26 },
                new() { Name = "Anzac Day", Month = 4, Day = 25 },
                new() { Name = "Christmas Day", Month = 12, Day = 25 },
                new() { Name = "Boxing Day", Month = 12, Day = 26 },
            },
            ["IN"] = new()
            {
                new() { Name = "Republic Day", Month = 1, Day = 26 },
                new() { Name = "Independence Day", Month = 8, Day = 15 },
                new() { Name = "Gandhi Jayanti", Month = 10, Day = 2 },
            },
            ["CN"] = new()
            {
                new() { Name = "元旦", Month = 1, Day = 1 },
                new() { Name = "劳动节", Month = 5, Day = 1 },
                new() { Name = "国庆节", Month = 10, Day = 1 },
            },
        };

        public static string[] AvailableLocales => [.. BuiltIn.Keys.Order()];
    }
    // â”€â”€â”€ Report Configuration â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    /// <summary>
    /// Configuration for a report definition that can be generated on demand or on schedule.
    /// Reports can include charts (historical data), real-time OPC values, tables, and text.
    /// </summary>
    public class ReportConfig
    {
        public string Name { get; set; } = "";
        public bool Enabled { get; set; } = true;

        /// <summary>Output format: "PDF" or "HTML". Default "HTML".</summary>
        public string Format { get; set; } = "HTML";

        /// <summary>Report title shown in the header.</summary>
        public string Title { get; set; } = "";

        /// <summary>Optional description or subtitle.</summary>
        public string Description { get; set; } = "";

        /// <summary>Page size for PDF: "A4", "Letter", "A3". Default "A4".</summary>
        public string PageSize { get; set; } = "A4";

        /// <summary>Page orientation: "Portrait" or "Landscape". Default "Portrait".</summary>
        public string Orientation { get; set; } = "Portrait";

        /// <summary>Ordered list of report sections (charts, tables, text, values, headers, page breaks).</summary>
        public List<ReportSection> Sections { get; set; } = new();

        /// <summary>Delivery configuration: email, disk, or both.</summary>
        public ReportDeliveryConfig Delivery { get; set; } = new();

        /// <summary>Optional folder path for editor organization.</summary>
        public string Group { get; set; } = "";
    }

    /// <summary>
    /// A section within a report. The Type determines which properties are relevant.
    /// </summary>
    public class ReportSection
    {
        /// <summary>Unique ID for this section.</summary>
        public string Id { get; set; } = "";

        /// <summary>
        /// Section type: "Header", "Text", "Chart", "Table", "Value", "PageBreak".
        /// </summary>
        public string Type { get; set; } = "Text";

        /// <summary>Section title (for Header, Chart, Table).</summary>
        public string Title { get; set; } = "";

        /// <summary>Text content or HTML (for Text sections).</summary>
        public string Content { get; set; } = "";

        // â”€â”€â”€ Chart properties (Type == "Chart") â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        /// <summary>Chart type: "Line", "Bar", "Area", "Pie". Default "Line".</summary>
        public string ChartType { get; set; } = "Line";

        /// <summary>Variable paths for chart data series (historical data).</summary>
        public List<string> ChartVariablePaths { get; set; } = new();

        /// <summary>Time range in minutes for historical data query. Default 60.</summary>
        public int ChartTimeRangeMinutes { get; set; } = 60;

        /// <summary>Maximum data points per series. Default 500.</summary>
        public int ChartMaxPoints { get; set; } = 500;

        /// <summary>Chart width in pixels (for HTML rendering). Default 600.</summary>
        public int ChartWidth { get; set; } = 600;

        /// <summary>Chart height in pixels. Default 300.</summary>
        public int ChartHeight { get; set; } = 300;

        /// <summary>Show chart legend. Default true.</summary>
        public bool ChartShowLegend { get; set; } = true;

        /// <summary>Show chart grid lines. Default true.</summary>
        public bool ChartShowGrid { get; set; } = true;

        // â”€â”€â”€ Table properties (Type == "Table") â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        /// <summary>Variable paths for table rows (each path becomes a row with name + current value).</summary>
        public List<string> TableVariablePaths { get; set; } = new();

        /// <summary>Whether to show historical data in table (min/max/avg). Default false.</summary>
        public bool TableShowStatistics { get; set; }

        /// <summary>Time range for table statistics in minutes. Default 60.</summary>
        public int TableTimeRangeMinutes { get; set; } = 60;

        // â”€â”€â”€ Value properties (Type == "Value") â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        /// <summary>Variable path for a single real-time value display.</summary>
        public string ValueVariablePath { get; set; } = "";

        /// <summary>Label shown next to the value.</summary>
        public string ValueLabel { get; set; } = "";

        /// <summary>Format string for the value (e.g. "F2"). Default "".</summary>
        public string ValueFormat { get; set; } = "";

        /// <summary>Engineering unit (e.g. "Â°C", "bar"). Default "".</summary>
        public string ValueUnit { get; set; } = "";
    }

    /// <summary>
    /// Delivery configuration for generated reports.
    /// </summary>
    public class ReportDeliveryConfig
    {
        /// <summary>Delivery method: "Email", "Disk", "Both". Default "Disk".</summary>
        public string Method { get; set; } = "Disk";

        /// <summary>Comma-separated email addresses for delivery.</summary>
        public string EmailRecipients { get; set; } = "";

        /// <summary>Email subject line. Supports {ReportName} and {DateTime} placeholders.</summary>
        public string EmailSubject { get; set; } = "Report: {ReportName} - {DateTime}";

        /// <summary>SMTP server address.</summary>
        public string SmtpServer { get; set; } = "";

        /// <summary>SMTP server port. Default 587.</summary>
        public int SmtpPort { get; set; } = 587;

        /// <summary>SMTP username.</summary>
        public string SmtpUser { get; set; } = "";

        /// <summary>SMTP password.</summary>
        public string SmtpPassword { get; set; } = "";

        /// <summary>Use SSL/TLS for SMTP. Default true.</summary>
        public bool SmtpUseSsl { get; set; } = true;

        /// <summary>Sender email address.</summary>
        public string SmtpFromAddress { get; set; } = "";

        /// <summary>Disk output directory path. Default "Reports".</summary>
        public string DiskPath { get; set; } = "Reports";

        /// <summary>File name pattern. Supports {ReportName}, {DateTime}, {Date}. Default "{ReportName}_{DateTime}".</summary>
        public string FileNamePattern { get; set; } = "{ReportName}_{DateTime}";
    }

    // --- Feature: REST API Configuration ---

    /// <summary>
    /// REST API configuration for exposing variables, alarms, recipes, and reports
    /// to external systems (MES, ERP, dashboards) via HTTP.
    /// </summary>
    public class ApiConfig
    {
        /// <summary>Whether the REST API is enabled.</summary>
        public bool Enabled { get; set; }

        /// <summary>HTTP port for the REST API. Default 14842.</summary>
        public int Port { get; set; } = 14842;

        /// <summary>
        /// API key for authentication. Sent as X-API-Key header or ?apiKey= query parameter.
        /// Empty = no authentication required (not recommended for production).
        /// </summary>
        public string ApiKey { get; set; } = "";
    }

    // --- Feature: Redundancy / High Availability ---

    /// <summary>
    /// Server redundancy configuration for high-availability deployments.
    /// Implements a heartbeat-based failover mechanism between primary and standby servers.
    /// </summary>
    public class RedundancyConfig
    {
        /// <summary>Whether redundancy is enabled.</summary>
        public bool Enabled { get; set; }

        /// <summary>Role of this server instance: "Primary" or "Standby".</summary>
        [AllowedStringValues("Primary", "Standby")]
        public string Role { get; set; } = "Primary";

        /// <summary>HTTP endpoint of the partner server (e.g. "http://192.168.1.100:14841").</summary>
        public string PartnerEndpoint { get; set; } = "";

        /// <summary>Heartbeat interval in seconds. Default 5.</summary>
        public int HeartbeatIntervalSeconds { get; set; } = 5;

        /// <summary>
        /// Number of consecutive missed heartbeats before triggering failover. Default 3.
        /// </summary>
        public int FailoverMissedHeartbeats { get; set; } = 3;

        /// <summary>
        /// Interval in milliseconds for syncing variable state from primary to standby.
        /// Set to 0 to disable state sync. Default 10000 (10 seconds).
        /// </summary>
        public int StateSyncIntervalMs { get; set; } = 10000;

        /// <summary>
        /// When true, standby automatically switches back to standby role when the primary recovers.
        /// </summary>
        public bool AutoSwitchback { get; set; }

        /// <summary>
        /// OPC UA endpoint URLs for both servers, exposed to clients for reconnection on failover.
        /// </summary>
        public List<string> ServerUrls { get; set; } = new();

        /// <summary>
        /// Maximum allowed replication lag in seconds before raising a warning. Default 30.
        /// </summary>
        public int MaxReplicationLagSeconds { get; set; } = 30;
    }

    // --- Feature: Rate Limiting / Throttling ---

    /// <summary>
    /// Configures per-endpoint rate limiting to protect OPC UA and web endpoints from abuse.
    /// </summary>
    public class RateLimitConfig
    {
        /// <summary>Whether rate limiting is active.</summary>
        public bool Enabled { get; set; }

        /// <summary>Maximum REST API requests per window. 0 = unlimited.</summary>
        public int ApiMaxRequestsPerWindow { get; set; } = 100;

        /// <summary>Maximum OPC UA write operations per window. 0 = unlimited.</summary>
        public int OpcUaWriteMaxPerWindow { get; set; } = 200;

        /// <summary>Maximum login attempts per window. 0 = unlimited.</summary>
        public int LoginMaxAttemptsPerWindow { get; set; } = 10;

        /// <summary>Maximum diagnostics endpoint requests per window. 0 = unlimited.</summary>
        public int DiagnosticsMaxRequestsPerWindow { get; set; } = 60;

        /// <summary>Sliding window duration in seconds. Default 60.</summary>
        public int WindowSeconds { get; set; } = 60;
    }

    // --- Feature: Calculated / Virtual Tags ---

    /// <summary>
    /// Configuration for a calculated (virtual) variable whose value is computed
    /// from an expression referencing other OPC variables.
    /// </summary>
    public class CalculatedVariableConfig
    {
        public string Name { get; set; } = "";

        /// <summary>
        /// C# expression that computes the value. Use Read("path") to reference OPC variables.
        /// Leave empty when using AggregateFunction instead.
        /// </summary>
        public string Expression { get; set; } = "";

        /// <summary>Output variable type: Double, Int32, Boolean, String. Default "Double".</summary>
        public string Type { get; set; } = "Double";

        /// <summary>Evaluation interval in milliseconds. Default 1000.</summary>
        public int IntervalMs { get; set; } = 1000;

        /// <summary>Whether this calculated variable is active.</summary>
        public bool Enabled { get; set; } = true;

        /// <summary>Folder path where the variable is created. Default "_Calculated".</summary>
        public string FolderPath { get; set; } = "_Calculated";

        /// <summary>Engineering unit label.</summary>
        public string EngineeringUnit { get; set; } = "";

        /// <summary>
        /// Built-in aggregate function instead of a custom expression.
        /// Supported: "Avg", "Sum", "Min", "Max", "Count", "RateOfChange", "Delta",
        /// "RunningAvg", "RunningMin", "RunningMax", "StdDev".
        /// </summary>
        public string AggregateFunction { get; set; } = "";

        /// <summary>Variable path(s) to aggregate. Semicolon-separated for multi-source.</summary>
        public string AggregateSourcePath { get; set; } = "";

        /// <summary>Rolling window size in seconds for time-based aggregates. Default 300.</summary>
        public int AggregateWindowSeconds { get; set; } = 300;
    }
}
