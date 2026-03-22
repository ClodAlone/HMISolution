using ServerEditorWeb.Models;

namespace ServerEditorWeb.Services;

/// <summary>
/// Provides context-sensitive help content based on the currently selected editor element.
/// </summary>
public class HelpService
{
    /// <summary>
    /// Get help content for the given context.
    /// </summary>
    public HelpContent GetHelp(string context, string locale = "en")
    {
        // Try fully localized help first (title + body)
        if (locale != "en" && _localizedHelp.TryGetValue(locale, out var locHelp) && locHelp.TryGetValue(context, out var locContent))
            return locContent;
        // Fall back to English
        if (!_helpTopics.TryGetValue(context, out var help))
            help = _helpTopics["welcome"];
        return help;
    }

    /// <summary>
    /// Get help based on the currently selected tree node type.
    /// </summary>
    public HelpContent GetHelpForNode(TreeNode? node, string locale = "en")
    {
        var key = node switch
        {
            ProjectNode => "project",
            VariableGroupNode => "variables",
            FolderNode => "folder",
            VariableNode => "variable",
            ScriptGroupNode => "scripts",
            ScriptNode => "script",
            PlcGroupNode => "plcprograms",
            PlcProgramNode => "plcprogram",
            ScreenGroupNode => "screens",
            ScreenNode => "screen",
            RecipeGroupNode or RecipeNode => "recipe",
            SchedulerGroupNode or SchedulerNode => "scheduler",
            ReportGroupNode or ReportNode => "report",
            UserGroupListNode or UserGroupNode => "usergroups",
            UserNode => "user",
            ServerSettingsNode => "server",
            ResourceFolderNode rfn => rfn.ResourceKind switch
            {
                "Script" => "scripts",
                "PlcProgram" => "plcprograms",
                "Screen" => "screens",
                _ => "folder"
            },
            _ => "welcome"
        };
        return GetHelp(key, locale);
    }

    /// <summary>
    /// Get help for a specific panel ID.
    /// </summary>
    public HelpContent GetHelpForPanel(string panelId, string locale = "en")
    {
        var key = panelId switch
        {
            "properties" => "properties",
            "json" => "json",
            "screen" => "screeneditor",
            "server" => "serverpanel",
            "git" => "git",
            "opc" => "opcbrowse",
            "datalogging" => "datalogging",
            "symbols" => "symbollibrary",
            "toolbox" => "toolbox",
            "scripteditor" => "scripteditor",
            "plceditor" => "plceditor",
            "ladder" => "laddereditor",
            "scheduler" => "schedulereditor",
            "report" => "reportdesigner",
            "xref" => "crossreference",
            "watch" => "watchtable",
            "problems" => "problems",
            "help" => "welcome",
            _ => "welcome"
        };
        return GetHelp(key, locale);
    }

    /// <summary>
    /// Get all available help topic keys for a table of contents.
    /// </summary>
    public List<(string Key, string Title, string Icon)> GetTableOfContents(string locale = "en")
    {
        var toc = new List<(string Key, string Title, string Icon)>
        {
            ("welcome", "Getting Started", "🏠"),
            ("project", "Project Structure", "📁"),
            ("variables", "Variables", "🏷️"),
            ("variable", "Variable Properties", "🏷️"),
            ("folder", "Folders", "📂"),
            ("scripts", "Scripts", "⚡"),
            ("script", "Script Editor", "⚡"),
            ("plcprograms", "PLC Programs", "⚙️"),
            ("plcprogram", "PLC Editor", "⚙️"),
            ("screens", "Screens", "🖼️"),
            ("screen", "Screen Editor", "🖼️"),
            ("screeneditor", "Screen Editor Panel", "🖼️"),
            ("recipe", "Recipes", "📦"),
            ("scheduler", "Schedulers", "📅"),
            ("report", "Reports", "📄"),
            ("user", "Users & Security", "👤"),
            ("usergroups", "User Groups", "👥"),
            ("server", "Server Settings", "🖥️"),
            ("serverpanel", "Server Panel", "🖥️"),
            ("strings", "Localization (Strings)", "🌐"),
            ("images", "Image Resources", "🖼️"),
            ("json", "JSON / AI Editor", "📝"),
            ("git", "Git Version Control", "🔀"),
            ("opcbrowse", "OPC Browse", "🔌"),
            ("datalogging", "Data Logging Viewer", "📊"),
            ("symbollibrary", "Symbol Library", "🏭"),
            ("toolbox", "Toolbox", "🧰"),
            ("crossreference", "Cross Reference", "🔗"),
            ("watchtable", "Watch Table", "👁️"),
            ("problems", "Problems Panel", "⚠️"),
            ("properties", "Properties Panel", "🏷️"),
            ("keyboard", "Keyboard Shortcuts", "⌨️"),
        };

        if (locale != "en" && _tocTitles.TryGetValue(locale, out var titles))
        {
            for (int i = 0; i < toc.Count; i++)
            {
                if (titles.TryGetValue(toc[i].Key, out var locTitle))
                    toc[i] = (toc[i].Key, locTitle, toc[i].Icon);
            }
        }

        return toc;
    }

    // ═══════════════════════════════════════════════════════════
    //  Localized TOC titles (help body stays in English)
    // ═══════════════════════════════════════════════════════════
    private static readonly Dictionary<string, Dictionary<string, string>> _tocTitles = new()
    {
        ["de"] = new(StringComparer.OrdinalIgnoreCase)
        {
            ["welcome"] = "Erste Schritte",
            ["project"] = "Projektstruktur",
            ["variables"] = "Variablen",
            ["variable"] = "Variableneigenschaften",
            ["folder"] = "Ordner",
            ["scripts"] = "Skripte",
            ["script"] = "Skript-Editor",
            ["plcprograms"] = "SPS-Programme",
            ["plcprogram"] = "SPS-Editor",
            ["screens"] = "Bildschirme",
            ["screen"] = "Bildschirm-Editor",
            ["screeneditor"] = "Bildschirm-Editor Panel",
            ["recipe"] = "Rezepte",
            ["scheduler"] = "Zeitplaner",
            ["report"] = "Berichte",
            ["user"] = "Benutzer & Sicherheit",
            ["usergroups"] = "Benutzergruppen",
            ["server"] = "Servereinstellungen",
            ["serverpanel"] = "Server-Panel",
            ["strings"] = "Lokalisierung (Strings)",
            ["images"] = "Bildressourcen",
            ["json"] = "JSON / KI-Editor",
            ["git"] = "Git-Versionsverwaltung",
            ["opcbrowse"] = "OPC Durchsuchen",
            ["datalogging"] = "Datenprotokoll-Viewer",
            ["symbollibrary"] = "Symbolbibliothek",
            ["toolbox"] = "Werkzeugkasten",
            ["crossreference"] = "Querverweise",
            ["watchtable"] = "Beobachtungstabelle",
            ["problems"] = "Probleme-Panel",
            ["properties"] = "Eigenschaften-Panel",
            ["keyboard"] = "Tastaturkürzel",
        },
        ["it"] = new(StringComparer.OrdinalIgnoreCase)
        {
            ["welcome"] = "Per iniziare",
            ["project"] = "Struttura progetto",
            ["variables"] = "Variabili",
            ["variable"] = "Proprietà variabile",
            ["folder"] = "Cartelle",
            ["scripts"] = "Script",
            ["script"] = "Editor script",
            ["plcprograms"] = "Programmi PLC",
            ["plcprogram"] = "Editor PLC",
            ["screens"] = "Schermate",
            ["screen"] = "Editor schermate",
            ["screeneditor"] = "Pannello Editor schermate",
            ["recipe"] = "Ricette",
            ["scheduler"] = "Pianificatori",
            ["report"] = "Report",
            ["user"] = "Utenti e sicurezza",
            ["usergroups"] = "Gruppi utente",
            ["server"] = "Impostazioni server",
            ["serverpanel"] = "Pannello server",
            ["strings"] = "Localizzazione (Stringhe)",
            ["images"] = "Risorse immagine",
            ["json"] = "Editor JSON / IA",
            ["git"] = "Controllo versione Git",
            ["opcbrowse"] = "Esplora OPC",
            ["datalogging"] = "Visualizzatore log dati",
            ["symbollibrary"] = "Libreria simboli",
            ["toolbox"] = "Casella strumenti",
            ["crossreference"] = "Riferimenti incrociati",
            ["watchtable"] = "Tabella osservazione",
            ["problems"] = "Pannello problemi",
            ["properties"] = "Pannello proprietà",
            ["keyboard"] = "Scorciatoie da tastiera",
        },
        ["fr"] = new(StringComparer.OrdinalIgnoreCase)
        {
            ["welcome"] = "Prise en main",
            ["project"] = "Structure du projet",
            ["variables"] = "Variables",
            ["variable"] = "Propriétés de variable",
            ["folder"] = "Dossiers",
            ["scripts"] = "Scripts",
            ["script"] = "Éditeur de scripts",
            ["plcprograms"] = "Programmes API",
            ["plcprogram"] = "Éditeur API",
            ["screens"] = "Écrans",
            ["screen"] = "Éditeur d'écrans",
            ["screeneditor"] = "Panneau éditeur d'écrans",
            ["recipe"] = "Recettes",
            ["scheduler"] = "Planificateurs",
            ["report"] = "Rapports",
            ["user"] = "Utilisateurs et sécurité",
            ["usergroups"] = "Groupes d'utilisateurs",
            ["server"] = "Paramètres du serveur",
            ["serverpanel"] = "Panneau serveur",
            ["strings"] = "Localisation (Chaînes)",
            ["images"] = "Ressources d'images",
            ["json"] = "Éditeur JSON / IA",
            ["git"] = "Contrôle de version Git",
            ["opcbrowse"] = "Parcourir OPC",
            ["datalogging"] = "Visionneuse de journaux",
            ["symbollibrary"] = "Bibliothèque de symboles",
            ["toolbox"] = "Boîte à outils",
            ["crossreference"] = "Références croisées",
            ["watchtable"] = "Table de surveillance",
            ["problems"] = "Panneau des problèmes",
            ["properties"] = "Panneau des propriétés",
            ["keyboard"] = "Raccourcis clavier",
        },
        ["ja"] = new(StringComparer.OrdinalIgnoreCase)
        {
            ["welcome"] = "はじめに",
            ["project"] = "プロジェクト構造",
            ["variables"] = "変数",
            ["variable"] = "変数プロパティ",
            ["folder"] = "フォルダー",
            ["scripts"] = "スクリプト",
            ["script"] = "スクリプトエディター",
            ["plcprograms"] = "PLCプログラム",
            ["plcprogram"] = "PLCエディター",
            ["screens"] = "画面",
            ["screen"] = "画面エディター",
            ["screeneditor"] = "画面エディターパネル",
            ["recipe"] = "レシピ",
            ["scheduler"] = "スケジューラー",
            ["report"] = "レポート",
            ["user"] = "ユーザーとセキュリティ",
            ["usergroups"] = "ユーザーグループ",
            ["server"] = "サーバー設定",
            ["serverpanel"] = "サーバーパネル",
            ["strings"] = "ローカライゼーション（文字列）",
            ["images"] = "画像リソース",
            ["json"] = "JSON / AIエディター",
            ["git"] = "Gitバージョン管理",
            ["opcbrowse"] = "OPCブラウズ",
            ["datalogging"] = "データログビューアー",
            ["symbollibrary"] = "シンボルライブラリ",
            ["toolbox"] = "ツールボックス",
            ["crossreference"] = "クロスリファレンス",
            ["watchtable"] = "ウォッチテーブル",
            ["problems"] = "問題パネル",
            ["properties"] = "プロパティパネル",
            ["keyboard"] = "キーボードショートカット",
        },
        ["zh"] = new(StringComparer.OrdinalIgnoreCase)
        {
            ["welcome"] = "入门指南",
            ["project"] = "项目结构",
            ["variables"] = "变量",
            ["variable"] = "变量属性",
            ["folder"] = "文件夹",
            ["scripts"] = "脚本",
            ["script"] = "脚本编辑器",
            ["plcprograms"] = "PLC程序",
            ["plcprogram"] = "PLC编辑器",
            ["screens"] = "画面",
            ["screen"] = "画面编辑器",
            ["screeneditor"] = "画面编辑器面板",
            ["recipe"] = "配方",
            ["scheduler"] = "调度器",
            ["report"] = "报告",
            ["user"] = "用户与安全",
            ["usergroups"] = "用户组",
            ["server"] = "服务器设置",
            ["serverpanel"] = "服务器面板",
            ["strings"] = "本地化（字符串）",
            ["images"] = "图像资源",
            ["json"] = "JSON / AI编辑器",
            ["git"] = "Git版本控制",
            ["opcbrowse"] = "OPC浏览",
            ["datalogging"] = "数据日志查看器",
            ["symbollibrary"] = "符号库",
            ["toolbox"] = "工具箱",
            ["crossreference"] = "交叉引用",
            ["watchtable"] = "监视表",
            ["problems"] = "问题面板",
            ["properties"] = "属性面板",
            ["keyboard"] = "键盘快捷键",
        },
    };

    private static readonly Dictionary<string, HelpContent> _helpTopics = new(StringComparer.OrdinalIgnoreCase)
    {
        ["welcome"] = new("🏠 Getting Started", """
## Welcome to the Server Editor

This is a web-based editor for configuring the **Simple OPC File Server** — an OPC UA server that reads its address space from a JSON configuration file.

### Quick Start
1. **Open a project** — File → Browse Server or Ctrl+O
2. **Edit variables** — Expand the project tree and select variables/folders
3. **Configure screens** — Double-click a screen node to open the Screen Editor
4. **Start the server** — Use the Server panel to start/stop the OPC UA server
5. **Save changes** — Ctrl+S or File → Save

### Key Concepts
- **Variables** — OPC UA address space nodes organized in folders
- **Screens** — HMI visualizations with SVG symbols and data bindings
- **Scripts** — C# or VB.NET code that runs periodically on the server
- **PLC Programs** — IEC 61131-3 Structured Text, Instruction List, or Ladder
- **Recipes** — Named sets of variable values stored in SQLite
- **Schedulers** — Weekly time-based variable automation

### Panels
Use **View** menu to show/hide panels. Drag panel tabs to reorganize the layout.
"""),

        ["project"] = new("📁 Project Structure", """
## Project Structure

A project file (`.json`) contains all configuration in a single file:

- **Variables** — Organized in folders, each with a name, type, and optional alarm/logging config
- **Scripts** — C# or VB.NET scripts that run at a configurable interval
- **PLC Programs** — Structured Text (ST), Instruction List (IL), or Ladder (LD)
- **Screens** — HMI screens with SVG/responsive layouts and interactive symbols
- **Recipes** — Named value sets that can be loaded/saved to SQLite
- **Schedulers** — Weekly time programs that write values on schedule
- **Reports** — Auto-generated PDF/HTML reports with charts, tables, and values
- **Users** — Authentication with groups, permissions, and auto-logoff
- **Server Settings** — OPC endpoint, diagnostics, cloud relay, crash email

### Multi-Project
You can open multiple project files simultaneously. Right-click a project to set it as active.
"""),

        ["variables"] = new("🏷️ Variables", """
## Variables

Variables define the OPC UA address space of the server.

### Organization
Variables are organized in **Folders**. The folder hierarchy defines the OPC UA browse path.
Example: `Plant.Furnace.Temperature` → OPC UA NodeId `"Plant.Furnace.Temperature"`.

### Adding Variables
- Select a folder or the Variables group node
- Click the 🏷️ button in the toolbar (or Edit → Add Variable)
- The new variable is created with a default `Double` type

### Variable Types
`Double`, `Int32`, `Int64`, `Boolean`, `String`, `DateTime`, `Float`, `UInt16`, `UInt32`, `Byte`

### Features
- **Alarms** — Limit-based or condition-based alarms on variable values
- **Data Logging** — Log value changes to TimescaleDB/PostgreSQL
- **Retentive** — Persist value across server restarts
- **Statistics** — Track Min/Max/Average/Count at runtime
- **Initial Value** — Set a starting value when the server starts
"""),

        ["variable"] = new("🏷️ Variable Properties", """
## Variable Properties

Select a variable and use the **Properties** panel to configure:

| Property | Description |
|----------|------------|
| **Name** | Variable name (part of the OPC path) |
| **Type** | Data type (Double, Int32, Boolean, String, etc.) |
| **Access** | Read, Write, or ReadWrite |
| **Initial Value** | Starting value on server start |
| **Retentive** | Save/restore value across restarts |
| **Alarm** | Configure alarm thresholds or conditions |
| **Data Logging** | Enable change-of-value logging |
| **Statistics** | Enable Min/Max/Avg tracking |

### Driver Bindings
Variables can be bound to external devices via drivers (Modbus, S7, OPC UA Client, etc.). Driver-specific properties appear in the JSON view.
"""),

        ["folder"] = new("📂 Folders", """
## Folders

Folders organize variables into a hierarchical structure that maps directly to the OPC UA address space.

### Creating Folders
- Select a parent folder or the Variables group
- Click 📁 in the toolbar or Edit → Add Folder

### Best Practices
- Use descriptive names: `Plant`, `Line1`, `Furnace`, `Motor`
- Keep the hierarchy shallow (3-4 levels max)
- Group related variables together
"""),

        ["scripts"] = new("⚡ Scripts", """
## Scripts

Scripts are C# or VB.NET code that runs periodically on the server.

### Script API
```csharp
Read("Plant.Temp")           // Read a variable value
ReadDouble("Plant.Temp")     // Read as double
ReadInt("Plant.Count")       // Read as int
ReadBool("Plant.Running")    // Read as bool
Write("Plant.Output", 42.0)  // Write a value
Log("message")               // Write to server log
OnChanged("Plant.Temp", e => { ... }) // React to changes
```

### Creating Scripts
1. Select the Scripts group in the tree
2. Click ⚡ in the toolbar or Edit → Add Script
3. Double-click the script to open the Script Editor

### Configuration
- **Language** — C# or VB.NET
- **Interval** — Execution interval in milliseconds
- **Enabled** — Start/stop the script
"""),

        ["script"] = new("⚡ Script Editor", """
## Script Editor

The Script Editor provides a code editing environment for C# and VB.NET scripts.

### Features
- Syntax highlighting with line numbers
- **Check Syntax** button — validates using Roslyn compiler
- Error locations shown with line numbers
- **Run Once** — Execute the script immediately (when server is running)

### Tips
- Use `OnChanged` for event-driven logic instead of polling
- Keep scripts focused — one purpose per script
- Use `Log()` for debugging
"""),

        ["plcprograms"] = new("⚙️ PLC Programs", """
## PLC Programs

IEC 61131-3 compliant programs in three languages:

### Structured Text (ST)
```
temperature := READ('Plant.Temp');
IF temperature > 80.0 THEN
    WRITE('Plant.Alarm', TRUE);
END_IF;
```

### Instruction List (IL)
```
LD 'Plant.Temp'
GT 80.0
ST 'Plant.Alarm'
```

### Ladder Diagram (LD)
Visual contact/coil logic editor with AND/OR/NOT operations.

### Configuration
- **Language** — ST, IL, or LD
- **Interval** — Scan cycle in milliseconds (default 100ms)
- **Enabled** — Active/inactive
"""),

        ["plcprogram"] = new("⚙️ PLC Editor", """
## PLC Editor

Code editor for Structured Text and Instruction List programs.

### Syntax Check
Click **Check Syntax** to validate the program before deploying.

### ST Quick Reference
- `READ('path')` / `WRITE('path', value)` — Variable access
- `IF...THEN...ELSIF...ELSE...END_IF` — Conditional
- `FOR...TO...DO...END_FOR` — Loop
- `WHILE...DO...END_WHILE` — While loop
- `CASE...OF...END_CASE` — Switch
- `:=` — Assignment
"""),

        ["screens"] = new("🖼️ Screens", """
## Screens

HMI screens with interactive visualizations.

### Layout Modes
- **SVG (Fixed)** — Fixed-size canvas with absolute positioning
- **Responsive** — CSS Grid layout that adapts to screen size

### Creating Screens
1. Select the Screens group
2. Click 🖼️ in the toolbar
3. Double-click to open the Screen Editor

### Symbol Types
Rect, Circle, Ellipse, Text, Line, Gauge, Indicator, SVG, Alarm List, HDA Chart, HDA Grid, Event Log, EditBox, IP Camera, Recipe, Weekly Planner, Screen Embed, Image Map, Trend, Switch, Rotary Switch, Knob, Slider, Button, Animated Text

### Data Binding
Symbols can be bound to variables via **VariablePath**. Bindings update in real-time at runtime.
"""),

        ["screen"] = new("🖼️ Screen Editor", """
## Screen Editor

The visual screen designer for creating HMI layouts.

### Adding Symbols
- Use the **Toolbox** panel to add shapes and widgets
- Use the **Symbol Library** for pre-built industrial SVG symbols
- Click on the canvas to place the selected tool

### Editing Symbols
- Click to select, drag to move
- Use handles to resize
- Hold Ctrl to select multiple symbols
- Properties panel shows all symbol settings

### Symbol Properties
- **Variable Path** — Bind to an OPC variable
- **Fill/Stroke/Label** — Visual appearance
- **Animations** — Dynamic color, visibility, rotation
- **Commands** — Click actions (write value, navigate, toggle)
"""),

        ["screeneditor"] = new("🖼️ Screen Editor Panel", """
## Screen Editor Panel

### Canvas Controls
- **Click** — Select symbol
- **Ctrl+Click** — Multi-select
- **Drag** — Move selected symbols
- **Drag handle** — Resize
- **Delete** — Remove selected symbols

### Alignment
Use the alignment toolbar to align or distribute multiple selected symbols.

### Groups
Select multiple symbols and use **Group** to lock them together.

### Animation Preview
The editor periodically ticks animations to preview dynamic behavior.
"""),

        ["recipe"] = new("📦 Recipes", """
## Recipes

Recipes store named sets of variable values in a SQLite database.

### How It Works
1. Define which variables are part of the recipe
2. At runtime, the server creates OPC commands: `Load`, `Save`, `Activate`, `ActiveRecipeName`
3. Use the Recipe widget on screens or write commands via scripts

### Recipe Variables
Each variable has:
- **Index** — Unique numeric key (stable across renames)
- **Variable Path** — OPC variable to read/write
- **Display Name** — Shown in the recipe editor UI
"""),

        ["scheduler"] = new("📅 Schedulers", """
## Schedulers

Weekly time programs that automatically write values to variables on a schedule.

### How It Works
- Define time slots for each day of the week
- Each slot writes a configured value to the target variable
- The server evaluates the schedule every minute

### Use Cases
- HVAC setpoint scheduling (comfort vs. economy mode)
- Lighting schedules
- Equipment start/stop times
"""),

        ["report"] = new("📄 Reports", """
## Reports

Auto-generated reports with charts, tables, and values.

### Sections
- **Chart** — Line/bar charts from HDA data
- **Table** — Tabular HDA data with timestamps
- **Value** — Current or last value of a variable

### Configuration
Each section references variable paths and has a configurable time range and title.
"""),

        ["user"] = new("👤 Users & Security", """
## Users & Security

### Authentication
When **Editor Login** or **Runtime Login** is enabled, users must authenticate.

### User Properties
- **Username/Password** — Credentials (passwords are PBKDF2-SHA256 hashed)
- **Group** — Assign to a user group for permissions
- **Auto Log-Off** — Idle timeout in seconds
- **Password Expiry** — Force change after N days
- **Must Change on First Login** — One-time password reset

### Access Levels
- **Read** — View only
- **Write** — Can modify values
- **ReadWrite** — Full access
"""),

        ["usergroups"] = new("👥 User Groups", """
## User Groups

Groups define access permissions for users.

### Properties
- **Name** — Group identifier
- **Access Level** — Read, Write, or ReadWrite
- **Can Access Editor** — Allow login to the web editor
- **Can Access Runtime** — Allow login to the runtime viewer
"""),

        ["server"] = new("🖥️ Server Settings", """
## Server Settings

### OPC UA Endpoint
The server listens on the configured endpoint URL (default: `opc.tcp://localhost:14840/SimpleOpcFileServer`).

### Key Settings
- **Enable Anonymous** — Allow unauthenticated OPC connections
- **Editor Login** / **Runtime Login** — Require user authentication
- **Startup Screen** — Default screen shown in RuntimeViewer
- **Diagnostics Port** — HTTP API for performance metrics (default 14841)
- **Cloud Relay** — Connect through a SignalR cloud hub
- **Crash Email** — SMTP configuration for crash report notifications
"""),

        ["serverpanel"] = new("🖥️ Server Panel", """
## Server Panel

Manage the OPC UA server process directly from the editor.

### Features
- **Start/Stop** the server process
- **Install as Windows Service** — Run the server as a background service
- **Performance Dashboard** — Live CPU, memory, thread count
- **Subsystem Performance** — Diagnostics from the server's `/diag` endpoint
- **Project Subsystems** — Shows which scripts, PLC programs, etc. are configured
"""),

        ["strings"] = new("🌐 Localization (Strings)", """
## Localization

The Strings editor manages translated text for multi-language HMI applications.

### How It Works
- Define string entries with a unique ID and translations per language
- Reference strings in screen labels using the `@StringId` prefix
- The RuntimeViewer resolves strings based on the active language
"""),

        ["images"] = new("🖼️ Image Resources", """
## Image Resources

Manage raster images (PNG, JPG, SVG) used in screens.

### Usage
- Upload images as base64 data URIs
- Reference images in screen backgrounds or Image Map widgets
- Images are stored inside the project JSON file
"""),

        ["json"] = new("📝 JSON / AI Editor", """
## JSON / AI Editor

Direct JSON editing of the project tree with AI assistance.

### JSON Editor
- View and edit the raw JSON of the selected node
- Click **Refresh** to sync from the tree
- Click **Apply** to write changes back

### AI Assistant
- Type a natural language request (e.g., "Add 10 temperature variables")
- The AI generates the modified JSON
- Review the diff and click Apply
"""),

        ["git"] = new("🔀 Git Version Control", """
## Git Panel

Track changes to your project files with Git.

### Features
- View file status (modified, added, deleted)
- Stage and unstage files
- Commit changes with a message
- View commit history
"""),

        ["opcbrowse"] = new("🔌 OPC Browse", """
## OPC Browse Panel

Browse the live OPC UA server address space.

### Features
- **Tree browser** — Navigate the OPC UA node hierarchy
- **Monitor** — Subscribe to node values with live updates
- **Write** — Double-click a monitored value to write a new value
- Auto-connects to the server when it's running
"""),

        ["datalogging"] = new("📊 Data Logging Viewer", """
## Data Logging Viewer

View historical data logged by the server.

### Features
- Query data by variable path and time range
- Line chart visualization
- Data grid with timestamps and values
- Export capabilities
"""),

        ["symbollibrary"] = new("🏭 Symbol Library", """
## Symbol Library

Pre-built industrial SVG symbols for HMI screens.

### Usage
1. Browse the library categories
2. Click a symbol to select it
3. The symbol is added to the active screen as an SVG element
4. Resize and position as needed

### Categories
Valves, pumps, motors, tanks, pipes, sensors, indicators, and more.
"""),

        ["toolbox"] = new("🧰 Toolbox", """
## Toolbox

Basic shapes and widgets for the Screen Editor.

### Shape Tools
- Rectangle, Circle, Ellipse, Line, Text

### Widget Tools
- Gauge, Indicator, EditBox, Button, Switch, Knob, Slider
- HDA Chart, HDA Grid, Event Log, Trend
- Recipe, Weekly Planner, IP Camera
- Screen Embed, Image Map, Animated Text
"""),

        ["crossreference"] = new("🔗 Cross Reference", """
## Cross Reference Panel

Find everywhere a variable is used across the entire project.

### Usage
1. Type or select a variable path in the search box
2. Click **Find** to scan all screens, scripts, PLC programs, alarms, etc.
3. Click a result to navigate to that location

### Also Available
Right-click a variable in the tree → **🔗 Find References**

### Prefix Match
Enable **Prefix match** to find all variables under a folder path.
"""),

        ["watchtable"] = new("👁️ Watch Table", """
## Watch Table

Monitor and write live OPC UA variable values.

### Usage
1. Add variables using the search box or right-click → **👁️ Add to Watch**
2. Values update in real-time via OPC UA subscription
3. **Double-click** a value to write a new one
4. Quality and timestamp are shown for each entry

### Watch Lists
- **Save** — Store the current variable set as a named list
- **Load** — Restore a previously saved list
- Lists are persisted per project in `%APPDATA%/SimpleOpcFileServer/watchlists/`
"""),

        ["problems"] = new("⚠️ Problems Panel", """
## Problems Panel

Validates the entire project and reports configuration errors and warnings.

### What It Checks
- **Broken bindings** — Screen symbols referencing non-existent variables
- **Script errors** — C# / VB.NET syntax errors via Roslyn
- **PLC errors** — Structured Text / Instruction List / Ladder syntax
- **Alarm issues** — Invalid thresholds, missing messages
- **Recipe/Report** — References to deleted variables
- **Server config** — Missing endpoints, login without users
- **Duplicates** — Duplicate names for screens, scripts, recipes, etc.

### Usage
Click **🔍 Validate Project** to run a full scan. Filter by severity (Error/Warning/Info) or by category.
"""),

        ["properties"] = new("🏷️ Properties Panel", """
## Properties Panel

Displays and edits properties of the selected tree node or screen symbol.

### Features
- All editable properties for the selected item
- Type-appropriate editors (text, number, checkbox, dropdown)
- Undo/Redo support for property changes
"""),

        ["keyboard"] = new("⌨️ Keyboard Shortcuts", """
## Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| **Ctrl+N** | New project |
| **Ctrl+O** | Open project file |
| **Ctrl+S** | Save |
| **Ctrl+Shift+S** | Save As |
| **Ctrl+Z** | Undo |
| **Ctrl+Y** | Redo |
| **Ctrl+C** | Copy |
| **Ctrl+V** | Paste |
| **Delete** | Delete selected |
| **Escape** | Restore layout / exit maximize |
"""),

        ["camera"] = new("📷 Cameras", """
## IP Cameras

Configure IP camera streams for live video on HMI screens.

### Properties
- **Stream URL** — MJPEG or snapshot URL
- **Refresh Interval** — Update rate for snapshot mode
- **Name** — Display name
"""),
    };

    // ═══════════════════════════════════════════════════════════
    //  Fully localized help content (title + body per language)
    // ═══════════════════════════════════════════════════════════
    private static readonly Dictionary<string, Dictionary<string, HelpContent>> _localizedHelp = new()
    {
        // ───────────────────── GERMAN ─────────────────────
        ["de"] = new(StringComparer.OrdinalIgnoreCase)
        {
            ["welcome"] = new("🏠 Erste Schritte", """
## Willkommen im Server-Editor

Dies ist ein webbasierter Editor zur Konfiguration des **Simple OPC File Server** — ein OPC-UA-Server, der seinen Adressraum aus einer JSON-Konfigurationsdatei liest.

### Schnellstart
1. **Projekt öffnen** — Datei → Server durchsuchen oder Strg+O
2. **Variablen bearbeiten** — Projektbaum erweitern und Variablen/Ordner auswählen
3. **Bildschirme konfigurieren** — Doppelklick auf einen Bildschirmknoten
4. **Server starten** — Server-Panel zum Starten/Stoppen verwenden
5. **Änderungen speichern** — Strg+S oder Datei → Speichern

### Schlüsselkonzepte
- **Variablen** — OPC-UA-Adressraumknoten in Ordnern organisiert
- **Bildschirme** — HMI-Visualisierungen mit SVG-Symbolen und Datenbindungen
- **Skripte** — C#- oder VB.NET-Code, der periodisch auf dem Server ausgeführt wird
- **SPS-Programme** — IEC 61131-3 Strukturierter Text, Anweisungsliste oder Kontaktplan
- **Rezepte** — Benannte Variablenwert-Sätze in SQLite gespeichert
- **Zeitplaner** — Wöchentliche zeitbasierte Variablenautomatisierung

### Panels
Verwenden Sie das Menü **Ansicht**, um Panels ein-/auszublenden. Ziehen Sie Panel-Tabs, um das Layout anzupassen.
"""),
            ["project"] = new("📁 Projektstruktur", """
## Projektstruktur

Eine Projektdatei (`.json`) enthält die gesamte Konfiguration in einer einzigen Datei:

- **Variablen** — In Ordnern organisiert, jeweils mit Name, Typ und optionaler Alarm-/Logging-Konfiguration
- **Skripte** — C#- oder VB.NET-Skripte mit konfigurierbarem Intervall
- **SPS-Programme** — Strukturierter Text (ST), Anweisungsliste (IL) oder Kontaktplan (LD)
- **Bildschirme** — HMI-Bildschirme mit SVG/Responsive-Layouts und interaktiven Symbolen
- **Rezepte** — Benannte Wertesätze, die in SQLite geladen/gespeichert werden können
- **Zeitplaner** — Wöchentliche Zeitprogramme, die Werte nach Zeitplan schreiben
- **Berichte** — Automatisch generierte PDF/HTML-Berichte mit Diagrammen, Tabellen und Werten
- **Benutzer** — Authentifizierung mit Gruppen, Berechtigungen und Auto-Abmeldung
- **Servereinstellungen** — OPC-Endpunkt, Diagnose, Cloud-Relay, Absturz-E-Mail

### Multi-Projekt
Sie können mehrere Projektdateien gleichzeitig öffnen. Rechtsklick auf ein Projekt, um es als aktiv zu setzen.
"""),
            ["variables"] = new("🏷️ Variablen", """
## Variablen

Variablen definieren den OPC-UA-Adressraum des Servers.

### Organisation
Variablen sind in **Ordnern** organisiert. Die Ordnerhierarchie definiert den OPC-UA-Browsepfad.
Beispiel: `Plant.Furnace.Temperature` → OPC-UA-NodeId `"Plant.Furnace.Temperature"`.

### Variablen hinzufügen
- Ordner oder Variablengruppen-Knoten auswählen
- 🏷️-Button in der Symbolleiste klicken (oder Bearbeiten → Variable hinzufügen)
- Die neue Variable wird mit dem Standardtyp `Double` erstellt

### Variablentypen
`Double`, `Int32`, `Int64`, `Boolean`, `String`, `DateTime`, `Float`, `UInt16`, `UInt32`, `Byte`

### Funktionen
- **Alarme** — Grenzwert- oder bedingungsbasierte Alarme
- **Datenprotokollierung** — Wertänderungen in TimescaleDB/PostgreSQL protokollieren
- **Retentiv** — Wert über Serverneustarts hinweg beibehalten
- **Statistik** — Min/Max/Durchschnitt/Anzahl zur Laufzeit verfolgen
- **Anfangswert** — Startwert beim Serverstart festlegen
"""),
            ["variable"] = new("🏷️ Variableneigenschaften", """
## Variableneigenschaften

Wählen Sie eine Variable aus und verwenden Sie das **Eigenschaften**-Panel zur Konfiguration:

| Eigenschaft | Beschreibung |
|-------------|-------------|
| **Name** | Variablenname (Teil des OPC-Pfads) |
| **Typ** | Datentyp (Double, Int32, Boolean, String usw.) |
| **Zugriff** | Lesen, Schreiben oder Lesen/Schreiben |
| **Anfangswert** | Startwert beim Serverstart |
| **Retentiv** | Wert über Neustarts speichern/wiederherstellen |
| **Alarm** | Alarmschwellen oder -bedingungen konfigurieren |
| **Datenprotokoll** | Wertänderungs-Protokollierung aktivieren |
| **Statistik** | Min/Max/Durchschnitt-Verfolgung aktivieren |

### Treiberbindungen
Variablen können über Treiber (Modbus, S7, OPC-UA-Client usw.) an externe Geräte gebunden werden. Treiberspezifische Eigenschaften erscheinen in der JSON-Ansicht.
"""),
            ["folder"] = new("📂 Ordner", """
## Ordner

Ordner organisieren Variablen in einer hierarchischen Struktur, die direkt dem OPC-UA-Adressraum zugeordnet ist.

### Ordner erstellen
- Übergeordneten Ordner oder die Variablengruppe auswählen
- 📁 in der Symbolleiste klicken oder Bearbeiten → Ordner hinzufügen

### Bewährte Praktiken
- Beschreibende Namen verwenden: `Anlage`, `Linie1`, `Ofen`, `Motor`
- Hierarchie flach halten (max. 3-4 Ebenen)
- Verwandte Variablen gruppieren
"""),
            ["scripts"] = new("⚡ Skripte", """
## Skripte

Skripte sind C#- oder VB.NET-Code, der periodisch auf dem Server ausgeführt wird.

### Skript-API
```csharp
Read("Plant.Temp")           // Variable lesen
ReadDouble("Plant.Temp")     // Als Double lesen
ReadInt("Plant.Count")       // Als Int lesen
ReadBool("Plant.Running")    // Als Bool lesen
Write("Plant.Output", 42.0)  // Wert schreiben
Log("Nachricht")             // In Serverprotokoll schreiben
OnChanged("Plant.Temp", e => { ... }) // Auf Änderungen reagieren
```

### Skripte erstellen
1. Skriptgruppe im Baum auswählen
2. ⚡ in der Symbolleiste klicken oder Bearbeiten → Skript hinzufügen
3. Doppelklick auf das Skript zum Öffnen des Skript-Editors

### Konfiguration
- **Sprache** — C# oder VB.NET
- **Intervall** — Ausführungsintervall in Millisekunden
- **Aktiviert** — Skript starten/stoppen
"""),
            ["script"] = new("⚡ Skript-Editor", """
## Skript-Editor

Der Skript-Editor bietet eine Code-Bearbeitungsumgebung für C#- und VB.NET-Skripte.

### Funktionen
- Syntaxhervorhebung mit Zeilennummern
- **Syntax prüfen**-Button — Validierung mit Roslyn-Compiler
- Fehlerposition mit Zeilennummern angezeigt
- **Einmal ausführen** — Skript sofort ausführen (wenn Server läuft)

### Tipps
- `OnChanged` für ereignisgesteuerte Logik statt Polling verwenden
- Skripte fokussiert halten — ein Zweck pro Skript
- `Log()` zum Debuggen verwenden
"""),
            ["plcprograms"] = new("⚙️ SPS-Programme", """
## SPS-Programme

IEC 61131-3 konforme Programme in drei Sprachen:

### Strukturierter Text (ST)
```
temperature := READ('Plant.Temp');
IF temperature > 80.0 THEN
    WRITE('Plant.Alarm', TRUE);
END_IF;
```

### Anweisungsliste (IL)
```
LD 'Plant.Temp'
GT 80.0
ST 'Plant.Alarm'
```

### Kontaktplan (LD)
Visueller Kontakt-/Spulen-Logik-Editor mit UND/ODER/NICHT-Operationen.

### Konfiguration
- **Sprache** — ST, IL oder LD
- **Intervall** — Abtastzyklus in Millisekunden (Standard 100ms)
- **Aktiviert** — Aktiv/Inaktiv
"""),
            ["plcprogram"] = new("⚙️ SPS-Editor", """
## SPS-Editor

Code-Editor für Strukturierten Text und Anweisungslisten-Programme.

### Syntaxprüfung
Klicken Sie auf **Syntax prüfen**, um das Programm vor der Bereitstellung zu validieren.

### ST-Kurzreferenz
- `READ('Pfad')` / `WRITE('Pfad', Wert)` — Variablenzugriff
- `IF...THEN...ELSIF...ELSE...END_IF` — Bedingung
- `FOR...TO...DO...END_FOR` — Schleife
- `WHILE...DO...END_WHILE` — While-Schleife
- `CASE...OF...END_CASE` — Fallunterscheidung
- `:=` — Zuweisung
"""),
            ["screens"] = new("🖼️ Bildschirme", """
## Bildschirme

HMI-Bildschirme mit interaktiven Visualisierungen.

### Layout-Modi
- **SVG (Fest)** — Feste Leinwandgröße mit absoluter Positionierung
- **Responsiv** — CSS-Grid-Layout, das sich an die Bildschirmgröße anpasst

### Bildschirme erstellen
1. Bildschirmgruppe auswählen
2. 🖼️ in der Symbolleiste klicken
3. Doppelklick zum Öffnen des Bildschirm-Editors

### Symboltypen
Rechteck, Kreis, Ellipse, Text, Linie, Anzeige, Indikator, SVG, Alarmliste, HDA-Diagramm, HDA-Tabelle, Ereignisprotokoll, Eingabefeld, IP-Kamera, Rezept, Wochenplaner, Bildschirm-Einbettung, Bildkarte, Trend, Schalter, Drehschalter, Drehknopf, Schieber, Taste, Animierter Text

### Datenbindung
Symbole können über **VariablePath** an Variablen gebunden werden. Bindungen werden zur Laufzeit in Echtzeit aktualisiert.
"""),
            ["screen"] = new("🖼️ Bildschirm-Editor", """
## Bildschirm-Editor

Der visuelle Bildschirm-Designer zum Erstellen von HMI-Layouts.

### Symbole hinzufügen
- **Werkzeugkasten**-Panel zum Hinzufügen von Formen und Widgets verwenden
- **Symbolbibliothek** für vorgefertigte industrielle SVG-Symbole verwenden
- Auf die Leinwand klicken, um das ausgewählte Werkzeug zu platzieren

### Symbole bearbeiten
- Klicken zum Auswählen, Ziehen zum Verschieben
- Griffe zum Ändern der Größe verwenden
- Strg halten für Mehrfachauswahl
- Eigenschaften-Panel zeigt alle Symboleinstellungen

### Symboleigenschaften
- **Variablenpfad** — An eine OPC-Variable binden
- **Füllung/Rand/Beschriftung** — Visuelles Erscheinungsbild
- **Animationen** — Dynamische Farbe, Sichtbarkeit, Drehung
- **Befehle** — Klickaktionen (Wert schreiben, Navigieren, Umschalten)
"""),
            ["screeneditor"] = new("🖼️ Bildschirm-Editor Panel", """
## Bildschirm-Editor Panel

### Leinwand-Steuerung
- **Klick** — Symbol auswählen
- **Strg+Klick** — Mehrfachauswahl
- **Ziehen** — Ausgewählte Symbole verschieben
- **Griff ziehen** — Größe ändern
- **Entf** — Ausgewählte Symbole entfernen

### Ausrichtung
Verwenden Sie die Ausrichtungs-Symbolleiste, um mehrere ausgewählte Symbole auszurichten oder zu verteilen.

### Gruppen
Wählen Sie mehrere Symbole aus und verwenden Sie **Gruppieren**, um sie zusammenzufassen.

### Animationsvorschau
Der Editor aktualisiert Animationen periodisch, um dynamisches Verhalten in der Vorschau anzuzeigen.
"""),
            ["recipe"] = new("📦 Rezepte", """
## Rezepte

Rezepte speichern benannte Variablenwert-Sätze in einer SQLite-Datenbank.

### Funktionsweise
1. Definieren Sie, welche Variablen Teil des Rezepts sind
2. Zur Laufzeit erstellt der Server OPC-Befehle: `Load`, `Save`, `Activate`, `ActiveRecipeName`
3. Verwenden Sie das Rezept-Widget auf Bildschirmen oder schreiben Sie Befehle über Skripte

### Rezeptvariablen
Jede Variable hat:
- **Index** — Eindeutiger numerischer Schlüssel (stabil über Umbenennungen)
- **Variablenpfad** — OPC-Variable zum Lesen/Schreiben
- **Anzeigename** — In der Rezept-Editor-Oberfläche angezeigt
"""),
            ["scheduler"] = new("📅 Zeitplaner", """
## Zeitplaner

Wöchentliche Zeitprogramme, die automatisch Werte nach Zeitplan in Variablen schreiben.

### Funktionsweise
- Zeitfenster für jeden Wochentag definieren
- Jedes Zeitfenster schreibt einen konfigurierten Wert in die Zielvariable
- Der Server wertet den Zeitplan jede Minute aus

### Anwendungsfälle
- HLK-Sollwertplanung (Komfort- vs. Sparmodus)
- Beleuchtungszeitpläne
- Start-/Stoppzeiten für Geräte
"""),
            ["report"] = new("📄 Berichte", """
## Berichte

Automatisch generierte Berichte mit Diagrammen, Tabellen und Werten.

### Abschnitte
- **Diagramm** — Linien-/Balkendiagramme aus HDA-Daten
- **Tabelle** — Tabellarische HDA-Daten mit Zeitstempeln
- **Wert** — Aktueller oder letzter Wert einer Variablen

### Konfiguration
Jeder Abschnitt referenziert Variablenpfade und hat einen konfigurierbaren Zeitbereich und Titel.
"""),
            ["user"] = new("👤 Benutzer & Sicherheit", """
## Benutzer & Sicherheit

### Authentifizierung
Wenn **Editor-Login** oder **Runtime-Login** aktiviert ist, müssen sich Benutzer authentifizieren.

### Benutzereigenschaften
- **Benutzername/Passwort** — Anmeldedaten (Passwörter werden mit PBKDF2-SHA256 gehasht)
- **Gruppe** — Einer Benutzergruppe für Berechtigungen zuweisen
- **Auto-Abmeldung** — Leerlauf-Timeout in Sekunden
- **Passwortablauf** — Änderung nach N Tagen erzwingen
- **Muss beim ersten Login ändern** — Einmalige Passwortzurücksetzung

### Zugriffsebenen
- **Lesen** — Nur anzeigen
- **Schreiben** — Werte ändern
- **Lesen/Schreiben** — Vollzugriff
"""),
            ["usergroups"] = new("👥 Benutzergruppen", """
## Benutzergruppen

Gruppen definieren Zugriffsberechtigungen für Benutzer.

### Eigenschaften
- **Name** — Gruppenbezeichner
- **Zugriffsebene** — Lesen, Schreiben oder Lesen/Schreiben
- **Editor-Zugang** — Anmeldung am Web-Editor erlauben
- **Runtime-Zugang** — Anmeldung am Runtime-Viewer erlauben
"""),
            ["server"] = new("🖥️ Servereinstellungen", """
## Servereinstellungen

### OPC-UA-Endpunkt
Der Server lauscht auf der konfigurierten Endpunkt-URL (Standard: `opc.tcp://localhost:14840/SimpleOpcFileServer`).

### Wichtige Einstellungen
- **Anonym erlauben** — Nicht authentifizierte OPC-Verbindungen zulassen
- **Editor-Login** / **Runtime-Login** — Benutzerauthentifizierung erfordern
- **Startbildschirm** — Standardbildschirm im RuntimeViewer
- **Diagnoseport** — HTTP-API für Leistungsmetriken (Standard 14841)
- **Cloud-Relay** — Verbindung über einen SignalR-Cloud-Hub
- **Absturz-E-Mail** — SMTP-Konfiguration für Absturzbenachrichtigungen
"""),
            ["serverpanel"] = new("🖥️ Server-Panel", """
## Server-Panel

Verwalten Sie den OPC-UA-Serverprozess direkt aus dem Editor.

### Funktionen
- Server-Prozess **starten/stoppen**
- **Als Windows-Dienst installieren** — Server als Hintergrunddienst ausführen
- **Leistungs-Dashboard** — Live CPU, Speicher, Thread-Anzahl
- **Subsystem-Leistung** — Diagnose vom `/diag`-Endpunkt des Servers
- **Projekt-Subsysteme** — Zeigt konfigurierte Skripte, SPS-Programme usw.
"""),
            ["strings"] = new("🌐 Lokalisierung (Strings)", """
## Lokalisierung

Der String-Editor verwaltet übersetzte Texte für mehrsprachige HMI-Anwendungen.

### Funktionsweise
- String-Einträge mit eindeutiger ID und Übersetzungen pro Sprache definieren
- Strings in Bildschirmbeschriftungen mit dem Präfix `@StringId` referenzieren
- Der RuntimeViewer löst Strings basierend auf der aktiven Sprache auf
"""),
            ["images"] = new("🖼️ Bildressourcen", """
## Bildressourcen

Rasterbilder (PNG, JPG, SVG) für Bildschirme verwalten.

### Verwendung
- Bilder als Base64-Daten-URIs hochladen
- Bilder in Bildschirmhintergründen oder Bildkarten-Widgets referenzieren
- Bilder werden in der Projekt-JSON-Datei gespeichert
"""),
            ["json"] = new("📝 JSON / KI-Editor", """
## JSON / KI-Editor

Direkte JSON-Bearbeitung des Projektbaums mit KI-Unterstützung.

### JSON-Editor
- Roh-JSON des ausgewählten Knotens anzeigen und bearbeiten
- **Aktualisieren** klicken zum Synchronisieren vom Baum
- **Anwenden** klicken zum Zurückschreiben der Änderungen

### KI-Assistent
- Natürlichsprachliche Anfrage eingeben (z.B. "10 Temperaturvariablen hinzufügen")
- Die KI generiert das geänderte JSON
- Diff überprüfen und Anwenden klicken
"""),
            ["git"] = new("🔀 Git-Versionsverwaltung", """
## Git-Panel

Änderungen an Projektdateien mit Git verfolgen.

### Funktionen
- Dateistatus anzeigen (geändert, hinzugefügt, gelöscht)
- Dateien bereitstellen und zurücknehmen
- Änderungen mit einer Nachricht committen
- Commit-Verlauf anzeigen
"""),
            ["opcbrowse"] = new("🔌 OPC Durchsuchen", """
## OPC-Durchsuchen-Panel

Den Live-OPC-UA-Server-Adressraum durchsuchen.

### Funktionen
- **Baum-Browser** — OPC-UA-Knotenhierarchie navigieren
- **Überwachen** — Knotenwerte mit Live-Updates abonnieren
- **Schreiben** — Doppelklick auf einen überwachten Wert zum Schreiben
- Automatische Verbindung zum Server, wenn dieser läuft
"""),
            ["datalogging"] = new("📊 Datenprotokoll-Viewer", """
## Datenprotokoll-Viewer

Historische Daten anzeigen, die vom Server protokolliert wurden.

### Funktionen
- Daten nach Variablenpfad und Zeitbereich abfragen
- Liniendiagramm-Visualisierung
- Datenraster mit Zeitstempeln und Werten
- Exportmöglichkeiten
"""),
            ["symbollibrary"] = new("🏭 Symbolbibliothek", """
## Symbolbibliothek

Vorgefertigte industrielle SVG-Symbole für HMI-Bildschirme.

### Verwendung
1. Bibliothekskategorien durchsuchen
2. Symbol zum Auswählen anklicken
3. Das Symbol wird dem aktiven Bildschirm als SVG-Element hinzugefügt
4. Nach Bedarf skalieren und positionieren

### Kategorien
Ventile, Pumpen, Motoren, Tanks, Rohre, Sensoren, Anzeigen und mehr.
"""),
            ["toolbox"] = new("🧰 Werkzeugkasten", """
## Werkzeugkasten

Grundformen und Widgets für den Bildschirm-Editor.

### Formwerkzeuge
- Rechteck, Kreis, Ellipse, Linie, Text

### Widget-Werkzeuge
- Anzeige, Indikator, Eingabefeld, Taste, Schalter, Drehknopf, Schieber
- HDA-Diagramm, HDA-Tabelle, Ereignisprotokoll, Trend
- Rezept, Wochenplaner, IP-Kamera
- Bildschirm-Einbettung, Bildkarte, Animierter Text
"""),
            ["crossreference"] = new("🔗 Querverweise", """
## Querverweise-Panel

Finden Sie alle Verwendungen einer Variablen im gesamten Projekt.

### Verwendung
1. Variablenpfad im Suchfeld eingeben oder auswählen
2. **Suchen** klicken, um alle Bildschirme, Skripte, SPS-Programme, Alarme usw. zu durchsuchen
3. Auf ein Ergebnis klicken, um dorthin zu navigieren

### Auch verfügbar
Rechtsklick auf eine Variable im Baum → **🔗 Referenzen finden**

### Präfix-Suche
**Präfix-Suche** aktivieren, um alle Variablen unter einem Ordnerpfad zu finden.
"""),
            ["watchtable"] = new("👁️ Beobachtungstabelle", """
## Beobachtungstabelle

Live-OPC-UA-Variablenwerte überwachen und schreiben.

### Verwendung
1. Variablen über das Suchfeld oder Rechtsklick → **👁️ Zur Beobachtung hinzufügen** hinzufügen
2. Werte werden in Echtzeit über OPC-UA-Abonnement aktualisiert
3. **Doppelklick** auf einen Wert zum Schreiben eines neuen Werts
4. Qualität und Zeitstempel werden für jeden Eintrag angezeigt

### Beobachtungslisten
- **Speichern** — Aktuelle Variablenmenge als benannte Liste speichern
- **Laden** — Zuvor gespeicherte Liste wiederherstellen
- Listen werden pro Projekt in `%APPDATA%/SimpleOpcFileServer/watchlists/` gespeichert
"""),
            ["problems"] = new("⚠️ Probleme-Panel", """
## Probleme-Panel

Validiert das gesamte Projekt und meldet Konfigurationsfehler und Warnungen.

### Was geprüft wird
- **Fehlende Bindungen** — Bildschirmsymbole, die auf nicht existierende Variablen verweisen
- **Skriptfehler** — C# / VB.NET Syntaxfehler über Roslyn
- **SPS-Fehler** — Strukturierter Text / Anweisungsliste / Kontaktplan Syntax
- **Alarm-Probleme** — Ungültige Schwellenwerte, fehlende Meldungen
- **Rezept/Bericht** — Verweise auf gelöschte Variablen
- **Serverkonfiguration** — Fehlende Endpunkte, Login ohne Benutzer
- **Duplikate** — Doppelte Namen für Bildschirme, Skripte, Rezepte usw.

### Verwendung
Klicken Sie auf **🔍 Projekt validieren** für einen vollständigen Scan. Filtern nach Schweregrad (Fehler/Warnung/Info) oder Kategorie.
"""),
            ["properties"] = new("🏷️ Eigenschaften-Panel", """
## Eigenschaften-Panel

Zeigt und bearbeitet Eigenschaften des ausgewählten Baumknotens oder Bildschirmsymbols.

### Funktionen
- Alle bearbeitbaren Eigenschaften für das ausgewählte Element
- Typgerechte Editoren (Text, Zahl, Kontrollkästchen, Dropdown)
- Rückgängig/Wiederherstellen-Unterstützung für Eigenschaftsänderungen
"""),
            ["keyboard"] = new("⌨️ Tastaturkürzel", """
## Tastaturkürzel

| Kürzel | Aktion |
|--------|--------|
| **Strg+N** | Neues Projekt |
| **Strg+O** | Projektdatei öffnen |
| **Strg+S** | Speichern |
| **Strg+Umschalt+S** | Speichern unter |
| **Strg+Z** | Rückgängig |
| **Strg+Y** | Wiederherstellen |
| **Strg+C** | Kopieren |
| **Strg+V** | Einfügen |
| **Entf** | Auswahl löschen |
| **Esc** | Layout wiederherstellen / Maximierung beenden |
"""),
            ["camera"] = new("📷 Kameras", """
## IP-Kameras

IP-Kamera-Streams für Live-Video auf HMI-Bildschirmen konfigurieren.

### Eigenschaften
- **Stream-URL** — MJPEG- oder Snapshot-URL
- **Aktualisierungsintervall** — Aktualisierungsrate für Snapshot-Modus
- **Name** — Anzeigename
"""),
        },

        // ───────────────────── ITALIAN ─────────────────────
        ["it"] = new(StringComparer.OrdinalIgnoreCase)
        {
            ["welcome"] = new("🏠 Per iniziare", """
## Benvenuto nel Server Editor

Questo è un editor web per la configurazione del **Simple OPC File Server** — un server OPC UA che legge il suo spazio degli indirizzi da un file di configurazione JSON.

### Avvio rapido
1. **Apri un progetto** — File → Sfoglia Server o Ctrl+O
2. **Modifica variabili** — Espandi l'albero del progetto e seleziona variabili/cartelle
3. **Configura schermate** — Doppio clic su un nodo schermata
4. **Avvia il server** — Usa il pannello Server per avviare/fermare
5. **Salva le modifiche** — Ctrl+S o File → Salva

### Concetti chiave
- **Variabili** — Nodi dello spazio degli indirizzi OPC UA organizzati in cartelle
- **Schermate** — Visualizzazioni HMI con simboli SVG e binding dati
- **Script** — Codice C# o VB.NET eseguito periodicamente sul server
- **Programmi PLC** — IEC 61131-3 Testo Strutturato, Lista Istruzioni o Ladder
- **Ricette** — Set di valori variabili con nome, salvati in SQLite
- **Pianificatori** — Automazione variabili basata su programma settimanale

### Pannelli
Usa il menu **Visualizza** per mostrare/nascondere i pannelli. Trascina le schede dei pannelli per riorganizzare il layout.
"""),
            ["project"] = new("📁 Struttura progetto", """
## Struttura progetto

Un file di progetto (`.json`) contiene tutta la configurazione in un unico file:

- **Variabili** — Organizzate in cartelle, ciascuna con nome, tipo e configurazione allarme/log opzionale
- **Script** — Script C# o VB.NET con intervallo configurabile
- **Programmi PLC** — Testo Strutturato (ST), Lista Istruzioni (IL) o Ladder (LD)
- **Schermate** — Schermate HMI con layout SVG/Responsive e simboli interattivi
- **Ricette** — Set di valori con nome che possono essere caricati/salvati in SQLite
- **Pianificatori** — Programmi settimanali che scrivono valori a orario
- **Report** — Report PDF/HTML generati automaticamente con grafici, tabelle e valori
- **Utenti** — Autenticazione con gruppi, permessi e disconnessione automatica
- **Impostazioni server** — Endpoint OPC, diagnostica, relay cloud, email crash

### Multi-progetto
Puoi aprire più file di progetto contemporaneamente. Clic destro su un progetto per impostarlo come attivo.
"""),
            ["variables"] = new("🏷️ Variabili", """
## Variabili

Le variabili definiscono lo spazio degli indirizzi OPC UA del server.

### Organizzazione
Le variabili sono organizzate in **Cartelle**. La gerarchia delle cartelle definisce il percorso di navigazione OPC UA.
Esempio: `Plant.Furnace.Temperature` → OPC UA NodeId `"Plant.Furnace.Temperature"`.

### Aggiungere variabili
- Seleziona una cartella o il nodo del gruppo Variabili
- Clicca il pulsante 🏷️ nella barra degli strumenti (o Modifica → Aggiungi Variabile)
- La nuova variabile viene creata con tipo `Double` predefinito

### Tipi di variabile
`Double`, `Int32`, `Int64`, `Boolean`, `String`, `DateTime`, `Float`, `UInt16`, `UInt32`, `Byte`

### Funzionalità
- **Allarmi** — Allarmi basati su limiti o condizioni
- **Log dati** — Registra modifiche dei valori in TimescaleDB/PostgreSQL
- **Retentiva** — Mantieni il valore tra i riavvii del server
- **Statistiche** — Traccia Min/Max/Media/Conteggio a runtime
- **Valore iniziale** — Imposta un valore di partenza all'avvio del server
"""),
            ["variable"] = new("🏷️ Proprietà variabile", """
## Proprietà variabile

Seleziona una variabile e usa il pannello **Proprietà** per configurare:

| Proprietà | Descrizione |
|-----------|------------|
| **Nome** | Nome della variabile (parte del percorso OPC) |
| **Tipo** | Tipo di dato (Double, Int32, Boolean, String, ecc.) |
| **Accesso** | Lettura, Scrittura o Lettura/Scrittura |
| **Valore iniziale** | Valore di partenza all'avvio del server |
| **Retentiva** | Salva/ripristina il valore tra i riavvii |
| **Allarme** | Configura soglie o condizioni di allarme |
| **Log dati** | Abilita la registrazione delle modifiche |
| **Statistiche** | Abilita il tracciamento Min/Max/Media |

### Binding driver
Le variabili possono essere collegate a dispositivi esterni tramite driver (Modbus, S7, OPC UA Client, ecc.). Le proprietà specifiche del driver appaiono nella vista JSON.
"""),
            ["folder"] = new("📂 Cartelle", """
## Cartelle

Le cartelle organizzano le variabili in una struttura gerarchica che corrisponde direttamente allo spazio degli indirizzi OPC UA.

### Creare cartelle
- Seleziona una cartella padre o il gruppo Variabili
- Clicca 📁 nella barra degli strumenti o Modifica → Aggiungi Cartella

### Buone pratiche
- Usa nomi descrittivi: `Impianto`, `Linea1`, `Forno`, `Motore`
- Mantieni la gerarchia poco profonda (max 3-4 livelli)
- Raggruppa le variabili correlate
"""),
            ["scripts"] = new("⚡ Script", """
## Script

Gli script sono codice C# o VB.NET eseguito periodicamente sul server.

### API Script
```csharp
Read("Plant.Temp")           // Leggi un valore variabile
ReadDouble("Plant.Temp")     // Leggi come double
ReadInt("Plant.Count")       // Leggi come int
ReadBool("Plant.Running")    // Leggi come bool
Write("Plant.Output", 42.0)  // Scrivi un valore
Log("messaggio")             // Scrivi nel log del server
OnChanged("Plant.Temp", e => { ... }) // Reagisci ai cambiamenti
```

### Creare script
1. Seleziona il gruppo Script nell'albero
2. Clicca ⚡ nella barra degli strumenti o Modifica → Aggiungi Script
3. Doppio clic sullo script per aprire l'Editor Script

### Configurazione
- **Linguaggio** — C# o VB.NET
- **Intervallo** — Intervallo di esecuzione in millisecondi
- **Abilitato** — Avvia/ferma lo script
"""),
            ["script"] = new("⚡ Editor script", """
## Editor script

L'Editor Script fornisce un ambiente di editing per script C# e VB.NET.

### Funzionalità
- Evidenziazione della sintassi con numeri di riga
- Pulsante **Verifica sintassi** — validazione con compilatore Roslyn
- Posizioni degli errori mostrate con numeri di riga
- **Esegui una volta** — Esegui lo script immediatamente (quando il server è in esecuzione)

### Suggerimenti
- Usa `OnChanged` per logica basata su eventi invece del polling
- Mantieni gli script focalizzati — uno scopo per script
- Usa `Log()` per il debug
"""),
            ["plcprograms"] = new("⚙️ Programmi PLC", """
## Programmi PLC

Programmi conformi IEC 61131-3 in tre linguaggi:

### Testo Strutturato (ST)
```
temperature := READ('Plant.Temp');
IF temperature > 80.0 THEN
    WRITE('Plant.Alarm', TRUE);
END_IF;
```

### Lista Istruzioni (IL)
```
LD 'Plant.Temp'
GT 80.0
ST 'Plant.Alarm'
```

### Ladder (LD)
Editor visuale di logica contatti/bobine con operazioni AND/OR/NOT.

### Configurazione
- **Linguaggio** — ST, IL o LD
- **Intervallo** — Ciclo di scansione in millisecondi (predefinito 100ms)
- **Abilitato** — Attivo/Inattivo
"""),
            ["plcprogram"] = new("⚙️ Editor PLC", """
## Editor PLC

Editor di codice per programmi in Testo Strutturato e Lista Istruzioni.

### Verifica sintassi
Clicca **Verifica sintassi** per validare il programma prima della distribuzione.

### Riferimento rapido ST
- `READ('percorso')` / `WRITE('percorso', valore)` — Accesso variabili
- `IF...THEN...ELSIF...ELSE...END_IF` — Condizionale
- `FOR...TO...DO...END_FOR` — Ciclo
- `WHILE...DO...END_WHILE` — Ciclo While
- `CASE...OF...END_CASE` — Switch
- `:=` — Assegnazione
"""),
            ["screens"] = new("🖼️ Schermate", """
## Schermate

Schermate HMI con visualizzazioni interattive.

### Modalità layout
- **SVG (Fisso)** — Canvas a dimensione fissa con posizionamento assoluto
- **Responsive** — Layout CSS Grid che si adatta alle dimensioni dello schermo

### Creare schermate
1. Seleziona il gruppo Schermate
2. Clicca 🖼️ nella barra degli strumenti
3. Doppio clic per aprire l'Editor Schermate

### Tipi di simbolo
Rettangolo, Cerchio, Ellisse, Testo, Linea, Indicatore, Indicatore, SVG, Lista Allarmi, Grafico HDA, Griglia HDA, Log Eventi, Campo di Input, Telecamera IP, Ricetta, Pianificatore Settimanale, Schermata Incorporata, Mappa Immagine, Trend, Interruttore, Selettore Rotativo, Manopola, Slider, Pulsante, Testo Animato

### Binding dati
I simboli possono essere collegati alle variabili tramite **VariablePath**. I binding si aggiornano in tempo reale a runtime.
"""),
            ["screen"] = new("🖼️ Editor schermate", """
## Editor schermate

Il designer visuale per la creazione di layout HMI.

### Aggiungere simboli
- Usa il pannello **Casella strumenti** per aggiungere forme e widget
- Usa la **Libreria simboli** per simboli SVG industriali prefabbricati
- Clicca sul canvas per posizionare lo strumento selezionato

### Modificare simboli
- Clic per selezionare, trascinamento per spostare
- Usa le maniglie per ridimensionare
- Tieni premuto Ctrl per selezione multipla
- Il pannello Proprietà mostra tutte le impostazioni del simbolo

### Proprietà simbolo
- **Percorso variabile** — Collega a una variabile OPC
- **Riempimento/Bordo/Etichetta** — Aspetto visivo
- **Animazioni** — Colore, visibilità, rotazione dinamici
- **Comandi** — Azioni al clic (scrivi valore, naviga, alterna)
"""),
            ["screeneditor"] = new("🖼️ Pannello Editor schermate", """
## Pannello Editor schermate

### Controlli canvas
- **Clic** — Seleziona simbolo
- **Ctrl+Clic** — Selezione multipla
- **Trascinamento** — Sposta simboli selezionati
- **Trascina maniglia** — Ridimensiona
- **Canc** — Rimuovi simboli selezionati

### Allineamento
Usa la barra degli strumenti di allineamento per allineare o distribuire più simboli selezionati.

### Gruppi
Seleziona più simboli e usa **Raggruppa** per bloccarli insieme.

### Anteprima animazioni
L'editor aggiorna periodicamente le animazioni per mostrare un'anteprima del comportamento dinamico.
"""),
            ["recipe"] = new("📦 Ricette", """
## Ricette

Le ricette memorizzano set di valori variabili con nome in un database SQLite.

### Come funziona
1. Definisci quali variabili fanno parte della ricetta
2. A runtime, il server crea comandi OPC: `Load`, `Save`, `Activate`, `ActiveRecipeName`
3. Usa il widget Ricetta sulle schermate o scrivi comandi tramite script

### Variabili ricetta
Ogni variabile ha:
- **Indice** — Chiave numerica univoca (stabile tra rinominazioni)
- **Percorso variabile** — Variabile OPC da leggere/scrivere
- **Nome visualizzato** — Mostrato nell'interfaccia dell'editor ricette
"""),
            ["scheduler"] = new("📅 Pianificatori", """
## Pianificatori

Programmi settimanali che scrivono automaticamente valori nelle variabili secondo un orario.

### Come funziona
- Definisci fasce orarie per ogni giorno della settimana
- Ogni fascia scrive un valore configurato nella variabile di destinazione
- Il server valuta il programma ogni minuto

### Casi d'uso
- Programmazione setpoint HVAC (modalità comfort vs. risparmio)
- Programmi di illuminazione
- Orari di avvio/arresto delle apparecchiature
"""),
            ["report"] = new("📄 Report", """
## Report

Report generati automaticamente con grafici, tabelle e valori.

### Sezioni
- **Grafico** — Grafici a linee/barre da dati HDA
- **Tabella** — Dati HDA tabulari con timestamp
- **Valore** — Valore corrente o ultimo di una variabile

### Configurazione
Ogni sezione fa riferimento a percorsi variabili e ha un intervallo temporale e un titolo configurabili.
"""),
            ["user"] = new("👤 Utenti e sicurezza", """
## Utenti e sicurezza

### Autenticazione
Quando **Login Editor** o **Login Runtime** è abilitato, gli utenti devono autenticarsi.

### Proprietà utente
- **Nome utente/Password** — Credenziali (le password sono hash PBKDF2-SHA256)
- **Gruppo** — Assegna a un gruppo utente per i permessi
- **Disconnessione automatica** — Timeout di inattività in secondi
- **Scadenza password** — Forza il cambio dopo N giorni
- **Deve cambiare al primo accesso** — Reset password una tantum

### Livelli di accesso
- **Lettura** — Solo visualizzazione
- **Scrittura** — Può modificare valori
- **Lettura/Scrittura** — Accesso completo
"""),
            ["usergroups"] = new("👥 Gruppi utente", """
## Gruppi utente

I gruppi definiscono i permessi di accesso per gli utenti.

### Proprietà
- **Nome** — Identificatore del gruppo
- **Livello di accesso** — Lettura, Scrittura o Lettura/Scrittura
- **Accesso editor** — Consenti accesso all'editor web
- **Accesso runtime** — Consenti accesso al visualizzatore runtime
"""),
            ["server"] = new("🖥️ Impostazioni server", """
## Impostazioni server

### Endpoint OPC UA
Il server ascolta sull'URL dell'endpoint configurato (predefinito: `opc.tcp://localhost:14840/SimpleOpcFileServer`).

### Impostazioni principali
- **Consenti anonimo** — Permetti connessioni OPC non autenticate
- **Login editor** / **Login runtime** — Richiedi autenticazione utente
- **Schermata di avvio** — Schermata predefinita nel RuntimeViewer
- **Porta diagnostica** — API HTTP per metriche prestazionali (predefinito 14841)
- **Relay cloud** — Connessione tramite hub cloud SignalR
- **Email crash** — Configurazione SMTP per notifiche crash
"""),
            ["serverpanel"] = new("🖥️ Pannello server", """
## Pannello server

Gestisci il processo del server OPC UA direttamente dall'editor.

### Funzionalità
- **Avvia/Ferma** il processo del server
- **Installa come servizio Windows** — Esegui il server come servizio in background
- **Dashboard prestazioni** — CPU live, memoria, conteggio thread
- **Prestazioni sottosistemi** — Diagnostica dall'endpoint `/diag` del server
- **Sottosistemi progetto** — Mostra script, programmi PLC, ecc. configurati
"""),
            ["strings"] = new("🌐 Localizzazione (Stringhe)", """
## Localizzazione

L'editor Stringhe gestisce i testi tradotti per applicazioni HMI multilingua.

### Come funziona
- Definisci voci stringa con ID univoco e traduzioni per lingua
- Fai riferimento alle stringhe nelle etichette delle schermate usando il prefisso `@StringId`
- Il RuntimeViewer risolve le stringhe in base alla lingua attiva
"""),
            ["images"] = new("🖼️ Risorse immagine", """
## Risorse immagine

Gestisci immagini raster (PNG, JPG, SVG) usate nelle schermate.

### Utilizzo
- Carica immagini come URI dati Base64
- Fai riferimento alle immagini negli sfondi delle schermate o nei widget Mappa Immagine
- Le immagini sono memorizzate nel file JSON del progetto
"""),
            ["json"] = new("📝 Editor JSON / IA", """
## Editor JSON / IA

Modifica JSON diretta dell'albero del progetto con assistenza IA.

### Editor JSON
- Visualizza e modifica il JSON grezzo del nodo selezionato
- Clicca **Aggiorna** per sincronizzare dall'albero
- Clicca **Applica** per riscrivere le modifiche

### Assistente IA
- Digita una richiesta in linguaggio naturale (es. "Aggiungi 10 variabili temperatura")
- L'IA genera il JSON modificato
- Rivedi il diff e clicca Applica
"""),
            ["git"] = new("🔀 Controllo versione Git", """
## Pannello Git

Traccia le modifiche ai file del progetto con Git.

### Funzionalità
- Visualizza lo stato dei file (modificato, aggiunto, eliminato)
- Prepara e annulla la preparazione dei file
- Esegui commit delle modifiche con un messaggio
- Visualizza la cronologia dei commit
"""),
            ["opcbrowse"] = new("🔌 Esplora OPC", """
## Pannello Esplora OPC

Sfoglia lo spazio degli indirizzi del server OPC UA live.

### Funzionalità
- **Browser ad albero** — Naviga la gerarchia dei nodi OPC UA
- **Monitora** — Sottoscrivi ai valori dei nodi con aggiornamenti live
- **Scrivi** — Doppio clic su un valore monitorato per scrivere un nuovo valore
- Connessione automatica al server quando è in esecuzione
"""),
            ["datalogging"] = new("📊 Visualizzatore log dati", """
## Visualizzatore log dati

Visualizza i dati storici registrati dal server.

### Funzionalità
- Interroga i dati per percorso variabile e intervallo temporale
- Visualizzazione grafico a linee
- Griglia dati con timestamp e valori
- Capacità di esportazione
"""),
            ["symbollibrary"] = new("🏭 Libreria simboli", """
## Libreria simboli

Simboli SVG industriali prefabbricati per schermate HMI.

### Utilizzo
1. Sfoglia le categorie della libreria
2. Clicca un simbolo per selezionarlo
3. Il simbolo viene aggiunto alla schermata attiva come elemento SVG
4. Ridimensiona e posiziona secondo necessità

### Categorie
Valvole, pompe, motori, serbatoi, tubazioni, sensori, indicatori e altro.
"""),
            ["toolbox"] = new("🧰 Casella strumenti", """
## Casella strumenti

Forme base e widget per l'Editor Schermate.

### Strumenti forma
- Rettangolo, Cerchio, Ellisse, Linea, Testo

### Strumenti widget
- Indicatore, Indicatore, Campo di Input, Pulsante, Interruttore, Manopola, Slider
- Grafico HDA, Griglia HDA, Log Eventi, Trend
- Ricetta, Pianificatore Settimanale, Telecamera IP
- Schermata Incorporata, Mappa Immagine, Testo Animato
"""),
            ["crossreference"] = new("🔗 Riferimenti incrociati", """
## Pannello Riferimenti incrociati

Trova ovunque una variabile è usata nell'intero progetto.

### Utilizzo
1. Digita o seleziona un percorso variabile nella casella di ricerca
2. Clicca **Trova** per scansionare tutte le schermate, script, programmi PLC, allarmi, ecc.
3. Clicca su un risultato per navigare a quella posizione

### Disponibile anche
Clic destro su una variabile nell'albero → **🔗 Trova riferimenti**

### Corrispondenza prefisso
Abilita **Corrispondenza prefisso** per trovare tutte le variabili sotto un percorso cartella.
"""),
            ["watchtable"] = new("👁️ Tabella osservazione", """
## Tabella osservazione

Monitora e scrivi valori variabili OPC UA live.

### Utilizzo
1. Aggiungi variabili usando la casella di ricerca o clic destro → **👁️ Aggiungi a Osservazione**
2. I valori si aggiornano in tempo reale tramite sottoscrizione OPC UA
3. **Doppio clic** su un valore per scriverne uno nuovo
4. Qualità e timestamp sono mostrati per ogni voce

### Liste di osservazione
- **Salva** — Memorizza il set corrente di variabili come lista con nome
- **Carica** — Ripristina una lista salvata in precedenza
- Le liste sono salvate per progetto in `%APPDATA%/SimpleOpcFileServer/watchlists/`
"""),
            ["problems"] = new("⚠️ Pannello problemi", """
## Pannello problemi

Valida l'intero progetto e segnala errori e avvisi di configurazione.

### Cosa controlla
- **Binding mancanti** — Simboli schermata che fanno riferimento a variabili inesistenti
- **Errori script** — Errori di sintassi C# / VB.NET tramite Roslyn
- **Errori PLC** — Sintassi Testo Strutturato / Lista Istruzioni / Ladder
- **Problemi allarmi** — Soglie non valide, messaggi mancanti
- **Ricetta/Report** — Riferimenti a variabili eliminate
- **Config server** — Endpoint mancanti, login senza utenti
- **Duplicati** — Nomi duplicati per schermate, script, ricette, ecc.

### Utilizzo
Clicca **🔍 Valida Progetto** per una scansione completa. Filtra per gravità (Errore/Avviso/Info) o per categoria.
"""),
            ["properties"] = new("🏷️ Pannello proprietà", """
## Pannello proprietà

Visualizza e modifica le proprietà del nodo dell'albero o del simbolo schermata selezionato.

### Funzionalità
- Tutte le proprietà modificabili per l'elemento selezionato
- Editor appropriati al tipo (testo, numero, checkbox, dropdown)
- Supporto Annulla/Ripristina per le modifiche alle proprietà
"""),
            ["keyboard"] = new("⌨️ Scorciatoie da tastiera", """
## Scorciatoie da tastiera

| Scorciatoia | Azione |
|-------------|--------|
| **Ctrl+N** | Nuovo progetto |
| **Ctrl+O** | Apri file progetto |
| **Ctrl+S** | Salva |
| **Ctrl+Shift+S** | Salva con nome |
| **Ctrl+Z** | Annulla |
| **Ctrl+Y** | Ripristina |
| **Ctrl+C** | Copia |
| **Ctrl+V** | Incolla |
| **Canc** | Elimina selezionato |
| **Esc** | Ripristina layout / esci dalla massimizzazione |
"""),
            ["camera"] = new("📷 Telecamere", """
## Telecamere IP

Configura stream di telecamere IP per video live sulle schermate HMI.

### Proprietà
- **URL Stream** — URL MJPEG o snapshot
- **Intervallo aggiornamento** — Frequenza di aggiornamento per modalità snapshot
- **Nome** — Nome visualizzato
"""),
        },

        // ───────────────────── FRENCH ─────────────────────
        ["fr"] = new(StringComparer.OrdinalIgnoreCase)
        {
            ["welcome"] = new("🏠 Prise en main", """
## Bienvenue dans l'éditeur de serveur

Ceci est un éditeur web pour configurer le **Simple OPC File Server** — un serveur OPC UA qui lit son espace d'adressage à partir d'un fichier de configuration JSON.

### Démarrage rapide
1. **Ouvrir un projet** — Fichier → Parcourir le serveur ou Ctrl+O
2. **Modifier les variables** — Développer l'arborescence du projet et sélectionner les variables/dossiers
3. **Configurer les écrans** — Double-clic sur un nœud d'écran
4. **Démarrer le serveur** — Utiliser le panneau Serveur pour démarrer/arrêter
5. **Enregistrer les modifications** — Ctrl+S ou Fichier → Enregistrer

### Concepts clés
- **Variables** — Nœuds de l'espace d'adressage OPC UA organisés en dossiers
- **Écrans** — Visualisations IHM avec symboles SVG et liaisons de données
- **Scripts** — Code C# ou VB.NET exécuté périodiquement sur le serveur
- **Programmes API** — IEC 61131-3 Texte Structuré, Liste d'Instructions ou Ladder
- **Recettes** — Ensembles nommés de valeurs de variables stockés dans SQLite
- **Planificateurs** — Automatisation hebdomadaire des variables basée sur le temps

### Panneaux
Utilisez le menu **Affichage** pour afficher/masquer les panneaux. Faites glisser les onglets pour réorganiser la disposition.
"""),
            ["project"] = new("📁 Structure du projet", """
## Structure du projet

Un fichier de projet (`.json`) contient toute la configuration dans un seul fichier :

- **Variables** — Organisées en dossiers, chacune avec un nom, un type et une configuration alarme/journal optionnelle
- **Scripts** — Scripts C# ou VB.NET avec intervalle configurable
- **Programmes API** — Texte Structuré (ST), Liste d'Instructions (IL) ou Ladder (LD)
- **Écrans** — Écrans IHM avec mises en page SVG/Responsive et symboles interactifs
- **Recettes** — Ensembles de valeurs nommés pouvant être chargés/sauvegardés dans SQLite
- **Planificateurs** — Programmes hebdomadaires qui écrivent des valeurs selon un horaire
- **Rapports** — Rapports PDF/HTML générés automatiquement avec graphiques, tableaux et valeurs
- **Utilisateurs** — Authentification avec groupes, permissions et déconnexion automatique
- **Paramètres du serveur** — Point de terminaison OPC, diagnostics, relais cloud, email de crash

### Multi-projet
Vous pouvez ouvrir plusieurs fichiers de projet simultanément. Clic droit sur un projet pour le définir comme actif.
"""),
            ["variables"] = new("🏷️ Variables", """
## Variables

Les variables définissent l'espace d'adressage OPC UA du serveur.

### Organisation
Les variables sont organisées en **Dossiers**. La hiérarchie des dossiers définit le chemin de navigation OPC UA.
Exemple : `Plant.Furnace.Temperature` → OPC UA NodeId `"Plant.Furnace.Temperature"`.

### Ajouter des variables
- Sélectionner un dossier ou le nœud du groupe Variables
- Cliquer sur le bouton 🏷️ dans la barre d'outils (ou Édition → Ajouter Variable)
- La nouvelle variable est créée avec un type `Double` par défaut

### Types de variables
`Double`, `Int32`, `Int64`, `Boolean`, `String`, `DateTime`, `Float`, `UInt16`, `UInt32`, `Byte`

### Fonctionnalités
- **Alarmes** — Alarmes basées sur des limites ou des conditions
- **Journalisation** — Enregistrer les changements de valeurs dans TimescaleDB/PostgreSQL
- **Rémanent** — Conserver la valeur entre les redémarrages du serveur
- **Statistiques** — Suivre Min/Max/Moyenne/Compteur en temps réel
- **Valeur initiale** — Définir une valeur de démarrage au lancement du serveur
"""),
            ["variable"] = new("🏷️ Propriétés de variable", """
## Propriétés de variable

Sélectionnez une variable et utilisez le panneau **Propriétés** pour configurer :

| Propriété | Description |
|-----------|------------|
| **Nom** | Nom de la variable (partie du chemin OPC) |
| **Type** | Type de données (Double, Int32, Boolean, String, etc.) |
| **Accès** | Lecture, Écriture ou Lecture/Écriture |
| **Valeur initiale** | Valeur de démarrage au lancement du serveur |
| **Rémanent** | Sauvegarder/restaurer la valeur entre les redémarrages |
| **Alarme** | Configurer les seuils ou conditions d'alarme |
| **Journalisation** | Activer l'enregistrement des changements de valeurs |
| **Statistiques** | Activer le suivi Min/Max/Moyenne |

### Liaisons de pilotes
Les variables peuvent être liées à des appareils externes via des pilotes (Modbus, S7, OPC UA Client, etc.). Les propriétés spécifiques au pilote apparaissent dans la vue JSON.
"""),
            ["folder"] = new("📂 Dossiers", """
## Dossiers

Les dossiers organisent les variables dans une structure hiérarchique qui correspond directement à l'espace d'adressage OPC UA.

### Créer des dossiers
- Sélectionner un dossier parent ou le groupe Variables
- Cliquer sur 📁 dans la barre d'outils ou Édition → Ajouter Dossier

### Bonnes pratiques
- Utiliser des noms descriptifs : `Usine`, `Ligne1`, `Four`, `Moteur`
- Garder la hiérarchie peu profonde (3-4 niveaux max)
- Regrouper les variables liées
"""),
            ["scripts"] = new("⚡ Scripts", """
## Scripts

Les scripts sont du code C# ou VB.NET exécuté périodiquement sur le serveur.

### API Script
```csharp
Read("Plant.Temp")           // Lire une valeur de variable
ReadDouble("Plant.Temp")     // Lire en double
ReadInt("Plant.Count")       // Lire en int
ReadBool("Plant.Running")    // Lire en bool
Write("Plant.Output", 42.0)  // Écrire une valeur
Log("message")               // Écrire dans le journal du serveur
OnChanged("Plant.Temp", e => { ... }) // Réagir aux changements
```

### Créer des scripts
1. Sélectionner le groupe Scripts dans l'arborescence
2. Cliquer sur ⚡ dans la barre d'outils ou Édition → Ajouter Script
3. Double-clic sur le script pour ouvrir l'Éditeur de Scripts

### Configuration
- **Langage** — C# ou VB.NET
- **Intervalle** — Intervalle d'exécution en millisecondes
- **Activé** — Démarrer/arrêter le script
"""),
            ["script"] = new("⚡ Éditeur de scripts", """
## Éditeur de scripts

L'Éditeur de Scripts fournit un environnement d'édition de code pour les scripts C# et VB.NET.

### Fonctionnalités
- Coloration syntaxique avec numéros de ligne
- Bouton **Vérifier la syntaxe** — validation avec le compilateur Roslyn
- Positions des erreurs affichées avec les numéros de ligne
- **Exécuter une fois** — Exécuter le script immédiatement (quand le serveur est en marche)

### Conseils
- Utiliser `OnChanged` pour la logique événementielle au lieu du polling
- Garder les scripts ciblés — un seul objectif par script
- Utiliser `Log()` pour le débogage
"""),
            ["plcprograms"] = new("⚙️ Programmes API", """
## Programmes API

Programmes conformes IEC 61131-3 en trois langages :

### Texte Structuré (ST)
```
temperature := READ('Plant.Temp');
IF temperature > 80.0 THEN
    WRITE('Plant.Alarm', TRUE);
END_IF;
```

### Liste d'Instructions (IL)
```
LD 'Plant.Temp'
GT 80.0
ST 'Plant.Alarm'
```

### Ladder (LD)
Éditeur visuel de logique contacts/bobines avec opérations ET/OU/NON.

### Configuration
- **Langage** — ST, IL ou LD
- **Intervalle** — Cycle de scrutation en millisecondes (défaut 100ms)
- **Activé** — Actif/Inactif
"""),
            ["plcprogram"] = new("⚙️ Éditeur API", """
## Éditeur API

Éditeur de code pour programmes en Texte Structuré et Liste d'Instructions.

### Vérification de syntaxe
Cliquez sur **Vérifier la syntaxe** pour valider le programme avant le déploiement.

### Référence rapide ST
- `READ('chemin')` / `WRITE('chemin', valeur)` — Accès aux variables
- `IF...THEN...ELSIF...ELSE...END_IF` — Conditionnel
- `FOR...TO...DO...END_FOR` — Boucle
- `WHILE...DO...END_WHILE` — Boucle While
- `CASE...OF...END_CASE` — Switch
- `:=` — Affectation
"""),
            ["screens"] = new("🖼️ Écrans", """
## Écrans

Écrans IHM avec visualisations interactives.

### Modes de mise en page
- **SVG (Fixe)** — Canvas à taille fixe avec positionnement absolu
- **Responsive** — Mise en page CSS Grid qui s'adapte à la taille de l'écran

### Créer des écrans
1. Sélectionner le groupe Écrans
2. Cliquer sur 🖼️ dans la barre d'outils
3. Double-clic pour ouvrir l'Éditeur d'Écrans

### Types de symboles
Rectangle, Cercle, Ellipse, Texte, Ligne, Jauge, Indicateur, SVG, Liste d'Alarmes, Graphique HDA, Grille HDA, Journal d'Événements, Champ de Saisie, Caméra IP, Recette, Planificateur Hebdomadaire, Écran Intégré, Carte Image, Tendance, Interrupteur, Sélecteur Rotatif, Bouton Rotatif, Curseur, Bouton, Texte Animé

### Liaison de données
Les symboles peuvent être liés aux variables via **VariablePath**. Les liaisons se mettent à jour en temps réel à l'exécution.
"""),
            ["screen"] = new("🖼️ Éditeur d'écrans", """
## Éditeur d'écrans

Le concepteur visuel d'écrans pour créer des mises en page IHM.

### Ajouter des symboles
- Utiliser le panneau **Boîte à outils** pour ajouter des formes et widgets
- Utiliser la **Bibliothèque de symboles** pour les symboles SVG industriels préfabriqués
- Cliquer sur le canvas pour placer l'outil sélectionné

### Modifier des symboles
- Cliquer pour sélectionner, glisser pour déplacer
- Utiliser les poignées pour redimensionner
- Maintenir Ctrl pour la sélection multiple
- Le panneau Propriétés affiche tous les paramètres du symbole

### Propriétés du symbole
- **Chemin de variable** — Lier à une variable OPC
- **Remplissage/Contour/Étiquette** — Apparence visuelle
- **Animations** — Couleur, visibilité, rotation dynamiques
- **Commandes** — Actions au clic (écrire valeur, naviguer, basculer)
"""),
            ["screeneditor"] = new("🖼️ Panneau éditeur d'écrans", """
## Panneau éditeur d'écrans

### Contrôles du canvas
- **Clic** — Sélectionner un symbole
- **Ctrl+Clic** — Sélection multiple
- **Glisser** — Déplacer les symboles sélectionnés
- **Glisser une poignée** — Redimensionner
- **Suppr** — Supprimer les symboles sélectionnés

### Alignement
Utilisez la barre d'outils d'alignement pour aligner ou distribuer plusieurs symboles sélectionnés.

### Groupes
Sélectionnez plusieurs symboles et utilisez **Grouper** pour les verrouiller ensemble.

### Aperçu des animations
L'éditeur actualise périodiquement les animations pour prévisualiser le comportement dynamique.
"""),
            ["recipe"] = new("📦 Recettes", """
## Recettes

Les recettes stockent des ensembles nommés de valeurs de variables dans une base de données SQLite.

### Fonctionnement
1. Définir quelles variables font partie de la recette
2. À l'exécution, le serveur crée des commandes OPC : `Load`, `Save`, `Activate`, `ActiveRecipeName`
3. Utiliser le widget Recette sur les écrans ou écrire des commandes via des scripts

### Variables de recette
Chaque variable a :
- **Index** — Clé numérique unique (stable après renommage)
- **Chemin de variable** — Variable OPC à lire/écrire
- **Nom d'affichage** — Affiché dans l'interface de l'éditeur de recettes
"""),
            ["scheduler"] = new("📅 Planificateurs", """
## Planificateurs

Programmes hebdomadaires qui écrivent automatiquement des valeurs dans les variables selon un horaire.

### Fonctionnement
- Définir des créneaux horaires pour chaque jour de la semaine
- Chaque créneau écrit une valeur configurée dans la variable cible
- Le serveur évalue le programme chaque minute

### Cas d'utilisation
- Programmation des consignes HVAC (mode confort vs économie)
- Programmes d'éclairage
- Horaires de démarrage/arrêt des équipements
"""),
            ["report"] = new("📄 Rapports", """
## Rapports

Rapports générés automatiquement avec graphiques, tableaux et valeurs.

### Sections
- **Graphique** — Graphiques en lignes/barres à partir de données HDA
- **Tableau** — Données HDA tabulaires avec horodatages
- **Valeur** — Valeur actuelle ou dernière d'une variable

### Configuration
Chaque section fait référence à des chemins de variables et a un intervalle de temps et un titre configurables.
"""),
            ["user"] = new("👤 Utilisateurs et sécurité", """
## Utilisateurs et sécurité

### Authentification
Lorsque **Login Éditeur** ou **Login Runtime** est activé, les utilisateurs doivent s'authentifier.

### Propriétés utilisateur
- **Nom d'utilisateur/Mot de passe** — Identifiants (les mots de passe sont hachés PBKDF2-SHA256)
- **Groupe** — Assigner à un groupe d'utilisateurs pour les permissions
- **Déconnexion automatique** — Délai d'inactivité en secondes
- **Expiration du mot de passe** — Forcer le changement après N jours
- **Doit changer à la première connexion** — Réinitialisation unique du mot de passe

### Niveaux d'accès
- **Lecture** — Consultation uniquement
- **Écriture** — Peut modifier les valeurs
- **Lecture/Écriture** — Accès complet
"""),
            ["usergroups"] = new("👥 Groupes d'utilisateurs", """
## Groupes d'utilisateurs

Les groupes définissent les permissions d'accès pour les utilisateurs.

### Propriétés
- **Nom** — Identifiant du groupe
- **Niveau d'accès** — Lecture, Écriture ou Lecture/Écriture
- **Accès éditeur** — Autoriser la connexion à l'éditeur web
- **Accès runtime** — Autoriser la connexion au visualiseur runtime
"""),
            ["server"] = new("🖥️ Paramètres du serveur", """
## Paramètres du serveur

### Point de terminaison OPC UA
Le serveur écoute sur l'URL du point de terminaison configuré (défaut : `opc.tcp://localhost:14840/SimpleOpcFileServer`).

### Paramètres clés
- **Autoriser l'anonyme** — Permettre les connexions OPC non authentifiées
- **Login éditeur** / **Login runtime** — Exiger l'authentification des utilisateurs
- **Écran de démarrage** — Écran par défaut dans le RuntimeViewer
- **Port de diagnostic** — API HTTP pour les métriques de performance (défaut 14841)
- **Relais cloud** — Connexion via un hub cloud SignalR
- **Email de crash** — Configuration SMTP pour les notifications de crash
"""),
            ["serverpanel"] = new("🖥️ Panneau serveur", """
## Panneau serveur

Gérez le processus du serveur OPC UA directement depuis l'éditeur.

### Fonctionnalités
- **Démarrer/Arrêter** le processus du serveur
- **Installer comme service Windows** — Exécuter le serveur en tant que service d'arrière-plan
- **Tableau de bord des performances** — CPU en temps réel, mémoire, nombre de threads
- **Performances des sous-systèmes** — Diagnostics depuis le point de terminaison `/diag` du serveur
- **Sous-systèmes du projet** — Affiche les scripts, programmes API, etc. configurés
"""),
            ["strings"] = new("🌐 Localisation (Chaînes)", """
## Localisation

L'éditeur de Chaînes gère les textes traduits pour les applications IHM multilingues.

### Fonctionnement
- Définir des entrées de chaînes avec un ID unique et des traductions par langue
- Référencer les chaînes dans les étiquettes d'écran avec le préfixe `@StringId`
- Le RuntimeViewer résout les chaînes en fonction de la langue active
"""),
            ["images"] = new("🖼️ Ressources d'images", """
## Ressources d'images

Gérez les images raster (PNG, JPG, SVG) utilisées dans les écrans.

### Utilisation
- Télécharger des images en tant qu'URI de données Base64
- Référencer les images dans les arrière-plans d'écran ou les widgets Carte Image
- Les images sont stockées dans le fichier JSON du projet
"""),
            ["json"] = new("📝 Éditeur JSON / IA", """
## Éditeur JSON / IA

Édition JSON directe de l'arborescence du projet avec assistance IA.

### Éditeur JSON
- Afficher et modifier le JSON brut du nœud sélectionné
- Cliquer sur **Actualiser** pour synchroniser depuis l'arborescence
- Cliquer sur **Appliquer** pour écrire les modifications

### Assistant IA
- Tapez une demande en langage naturel (ex. « Ajouter 10 variables de température »)
- L'IA génère le JSON modifié
- Examinez le diff et cliquez sur Appliquer
"""),
            ["git"] = new("🔀 Contrôle de version Git", """
## Panneau Git

Suivez les modifications de vos fichiers de projet avec Git.

### Fonctionnalités
- Afficher le statut des fichiers (modifié, ajouté, supprimé)
- Indexer et désindexer des fichiers
- Committer les modifications avec un message
- Afficher l'historique des commits
"""),
            ["opcbrowse"] = new("🔌 Parcourir OPC", """
## Panneau Parcourir OPC

Parcourez l'espace d'adressage du serveur OPC UA en direct.

### Fonctionnalités
- **Navigateur en arbre** — Parcourir la hiérarchie des nœuds OPC UA
- **Surveiller** — S'abonner aux valeurs des nœuds avec mises à jour en direct
- **Écrire** — Double-clic sur une valeur surveillée pour écrire une nouvelle valeur
- Connexion automatique au serveur lorsqu'il est en marche
"""),
            ["datalogging"] = new("📊 Visionneuse de journaux", """
## Visionneuse de journaux de données

Affichez les données historiques enregistrées par le serveur.

### Fonctionnalités
- Interroger les données par chemin de variable et plage temporelle
- Visualisation en graphique en lignes
- Grille de données avec horodatages et valeurs
- Capacités d'exportation
"""),
            ["symbollibrary"] = new("🏭 Bibliothèque de symboles", """
## Bibliothèque de symboles

Symboles SVG industriels préfabriqués pour les écrans IHM.

### Utilisation
1. Parcourir les catégories de la bibliothèque
2. Cliquer sur un symbole pour le sélectionner
3. Le symbole est ajouté à l'écran actif comme élément SVG
4. Redimensionner et positionner selon les besoins

### Catégories
Vannes, pompes, moteurs, réservoirs, tuyaux, capteurs, indicateurs et plus encore.
"""),
            ["toolbox"] = new("🧰 Boîte à outils", """
## Boîte à outils

Formes de base et widgets pour l'Éditeur d'Écrans.

### Outils de forme
- Rectangle, Cercle, Ellipse, Ligne, Texte

### Outils de widget
- Jauge, Indicateur, Champ de Saisie, Bouton, Interrupteur, Bouton Rotatif, Curseur
- Graphique HDA, Grille HDA, Journal d'Événements, Tendance
- Recette, Planificateur Hebdomadaire, Caméra IP
- Écran Intégré, Carte Image, Texte Animé
"""),
            ["crossreference"] = new("🔗 Références croisées", """
## Panneau des Références croisées

Trouvez partout où une variable est utilisée dans l'ensemble du projet.

### Utilisation
1. Tapez ou sélectionnez un chemin de variable dans la zone de recherche
2. Cliquez sur **Rechercher** pour analyser tous les écrans, scripts, programmes API, alarmes, etc.
3. Cliquez sur un résultat pour naviguer vers cet emplacement

### Également disponible
Clic droit sur une variable dans l'arborescence → **🔗 Rechercher les références**

### Correspondance par préfixe
Activez **Correspondance par préfixe** pour trouver toutes les variables sous un chemin de dossier.
"""),
            ["watchtable"] = new("👁️ Table de surveillance", """
## Table de surveillance

Surveillez et écrivez des valeurs de variables OPC UA en direct.

### Utilisation
1. Ajoutez des variables via la zone de recherche ou clic droit → **👁️ Ajouter à la surveillance**
2. Les valeurs se mettent à jour en temps réel via l'abonnement OPC UA
3. **Double-clic** sur une valeur pour en écrire une nouvelle
4. La qualité et l'horodatage sont affichés pour chaque entrée

### Listes de surveillance
- **Sauvegarder** — Stocker l'ensemble de variables actuel comme liste nommée
- **Charger** — Restaurer une liste précédemment sauvegardée
- Les listes sont persistées par projet dans `%APPDATA%/SimpleOpcFileServer/watchlists/`
"""),
            ["problems"] = new("⚠️ Panneau des problèmes", """
## Panneau des problèmes

Valide l'ensemble du projet et signale les erreurs et avertissements de configuration.

### Ce qui est vérifié
- **Liaisons cassées** — Symboles d'écran référençant des variables inexistantes
- **Erreurs de script** — Erreurs de syntaxe C# / VB.NET via Roslyn
- **Erreurs PLC** — Syntaxe Texte Structuré / Liste d'Instructions / Ladder
- **Problèmes d'alarme** — Seuils invalides, messages manquants
- **Recette/Rapport** — Références à des variables supprimées
- **Config serveur** — Points de terminaison manquants, login sans utilisateurs
- **Doublons** — Noms en double pour les écrans, scripts, recettes, etc.

### Utilisation
Cliquez sur **🔍 Valider le Projet** pour une analyse complète. Filtrez par gravité (Erreur/Avertissement/Info) ou par catégorie.
"""),
            ["properties"] = new("🏷️ Panneau des propriétés", """
## Panneau des propriétés

Affiche et modifie les propriétés du nœud d'arbre ou du symbole d'écran sélectionné.

### Fonctionnalités
- Toutes les propriétés modifiables pour l'élément sélectionné
- Éditeurs adaptés au type (texte, nombre, case à cocher, liste déroulante)
- Support Annuler/Rétablir pour les modifications de propriétés
"""),
            ["keyboard"] = new("⌨️ Raccourcis clavier", """
## Raccourcis clavier

| Raccourci | Action |
|-----------|--------|
| **Ctrl+N** | Nouveau projet |
| **Ctrl+O** | Ouvrir un fichier de projet |
| **Ctrl+S** | Enregistrer |
| **Ctrl+Shift+S** | Enregistrer sous |
| **Ctrl+Z** | Annuler |
| **Ctrl+Y** | Rétablir |
| **Ctrl+C** | Copier |
| **Ctrl+V** | Coller |
| **Suppr** | Supprimer la sélection |
| **Échap** | Restaurer la disposition / quitter la maximisation |
"""),
            ["camera"] = new("📷 Caméras", """
## Caméras IP

Configurez les flux de caméras IP pour la vidéo en direct sur les écrans IHM.

### Propriétés
- **URL du flux** — URL MJPEG ou instantané
- **Intervalle de rafraîchissement** — Taux de mise à jour pour le mode instantané
- **Nom** — Nom d'affichage
"""),
        },

        // ───────────────────── JAPANESE ─────────────────────
        ["ja"] = new(StringComparer.OrdinalIgnoreCase)
        {
            ["welcome"] = new("🏠 はじめに", """
## サーバーエディターへようこそ

これは **Simple OPC File Server** を設定するためのWebベースのエディターです。JSON設定ファイルからアドレス空間を読み取るOPC UAサーバーです。

### クイックスタート
1. **プロジェクトを開く** — ファイル → サーバーを参照 または Ctrl+O
2. **変数を編集** — プロジェクトツリーを展開し、変数/フォルダーを選択
3. **画面を設定** — 画面ノードをダブルクリック
4. **サーバーを起動** — サーバーパネルで起動/停止
5. **変更を保存** — Ctrl+S または ファイル → 保存

### 主要概念
- **変数** — フォルダーに整理されたOPC UAアドレス空間ノード
- **画面** — SVGシンボルとデータバインディングを持つHMIビジュアライゼーション
- **スクリプト** — サーバー上で定期的に実行されるC#またはVB.NETコード
- **PLCプログラム** — IEC 61131-3 構造化テキスト、命令リスト、またはラダー
- **レシピ** — SQLiteに保存された名前付き変数値セット
- **スケジューラー** — 週間時間ベースの変数自動化

### パネル
**表示**メニューでパネルの表示/非表示を切り替えます。パネルタブをドラッグしてレイアウトを変更できます。
"""),
            ["project"] = new("📁 プロジェクト構造", """
## プロジェクト構造

プロジェクトファイル（`.json`）には、すべての設定が1つのファイルに含まれています：

- **変数** — フォルダーに整理され、名前、型、オプションのアラーム/ログ設定を持つ
- **スクリプト** — 設定可能な間隔で実行されるC#またはVB.NETスクリプト
- **PLCプログラム** — 構造化テキスト（ST）、命令リスト（IL）、またはラダー（LD）
- **画面** — SVG/レスポンシブレイアウトとインタラクティブシンボルを持つHMI画面
- **レシピ** — SQLiteにロード/保存できる名前付き値セット
- **スケジューラー** — スケジュールに従って値を書き込む週間プログラム
- **レポート** — グラフ、テーブル、値を含む自動生成PDF/HTMLレポート
- **ユーザー** — グループ、権限、自動ログオフによる認証
- **サーバー設定** — OPCエンドポイント、診断、クラウドリレー、クラッシュメール

### マルチプロジェクト
複数のプロジェクトファイルを同時に開くことができます。プロジェクトを右クリックしてアクティブに設定します。
"""),
            ["variables"] = new("🏷️ 変数", """
## 変数

変数はサーバーのOPC UAアドレス空間を定義します。

### 整理
変数は**フォルダー**に整理されています。フォルダー階層がOPC UAブラウズパスを定義します。
例：`Plant.Furnace.Temperature` → OPC UA NodeId `"Plant.Furnace.Temperature"`

### 変数の追加
- フォルダーまたは変数グループノードを選択
- ツールバーの🏷️ボタンをクリック（または編集 → 変数の追加）
- 新しい変数はデフォルトの`Double`型で作成されます

### 変数の型
`Double`, `Int32`, `Int64`, `Boolean`, `String`, `DateTime`, `Float`, `UInt16`, `UInt32`, `Byte`

### 機能
- **アラーム** — 制限値または条件ベースのアラーム
- **データログ** — TimescaleDB/PostgreSQLに値の変更を記録
- **保持** — サーバー再起動時に値を保持
- **統計** — ランタイムで最小/最大/平均/カウントを追跡
- **初期値** — サーバー起動時の開始値を設定
"""),
            ["variable"] = new("🏷️ 変数プロパティ", """
## 変数プロパティ

変数を選択し、**プロパティ**パネルで設定します：

| プロパティ | 説明 |
|-----------|------|
| **名前** | 変数名（OPCパスの一部） |
| **型** | データ型（Double、Int32、Boolean、Stringなど） |
| **アクセス** | 読み取り、書き込み、または読み取り/書き込み |
| **初期値** | サーバー起動時の開始値 |
| **保持** | 再起動時に値を保存/復元 |
| **アラーム** | アラームしきい値または条件を設定 |
| **データログ** | 値変更の記録を有効化 |
| **統計** | 最小/最大/平均の追跡を有効化 |

### ドライバーバインディング
変数はドライバー（Modbus、S7、OPC UAクライアントなど）を介して外部デバイスにバインドできます。ドライバー固有のプロパティはJSONビューに表示されます。
"""),
            ["folder"] = new("📂 フォルダー", """
## フォルダー

フォルダーは変数をOPC UAアドレス空間に直接マッピングされる階層構造に整理します。

### フォルダーの作成
- 親フォルダーまたは変数グループを選択
- ツールバーの📁をクリックまたは編集 → フォルダーの追加

### ベストプラクティス
- 説明的な名前を使用：`Plant`、`Line1`、`Furnace`、`Motor`
- 階層を浅く保つ（最大3〜4レベル）
- 関連する変数をグループ化
"""),
            ["scripts"] = new("⚡ スクリプト", """
## スクリプト

スクリプトはサーバー上で定期的に実行されるC#またはVB.NETコードです。

### スクリプトAPI
```csharp
Read("Plant.Temp")           // 変数値を読み取り
ReadDouble("Plant.Temp")     // Doubleとして読み取り
ReadInt("Plant.Count")       // Intとして読み取り
ReadBool("Plant.Running")    // Boolとして読み取り
Write("Plant.Output", 42.0)  // 値を書き込み
Log("メッセージ")             // サーバーログに書き込み
OnChanged("Plant.Temp", e => { ... }) // 変更に反応
```

### スクリプトの作成
1. ツリーでスクリプトグループを選択
2. ツールバーの⚡をクリックまたは編集 → スクリプトの追加
3. スクリプトをダブルクリックしてスクリプトエディターを開く

### 設定
- **言語** — C#またはVB.NET
- **間隔** — 実行間隔（ミリ秒）
- **有効** — スクリプトの開始/停止
"""),
            ["script"] = new("⚡ スクリプトエディター", """
## スクリプトエディター

スクリプトエディターはC#およびVB.NETスクリプトのコード編集環境を提供します。

### 機能
- 行番号付きシンタックスハイライト
- **構文チェック**ボタン — Roslynコンパイラによる検証
- エラー位置が行番号で表示
- **一回実行** — スクリプトを即座に実行（サーバー実行中）

### ヒント
- ポーリングの代わりにイベント駆動ロジックには`OnChanged`を使用
- スクリプトは焦点を絞る — 1スクリプトにつき1つの目的
- デバッグには`Log()`を使用
"""),
            ["plcprograms"] = new("⚙️ PLCプログラム", """
## PLCプログラム

3つの言語に対応したIEC 61131-3準拠プログラム：

### 構造化テキスト（ST）
```
temperature := READ('Plant.Temp');
IF temperature > 80.0 THEN
    WRITE('Plant.Alarm', TRUE);
END_IF;
```

### 命令リスト（IL）
```
LD 'Plant.Temp'
GT 80.0
ST 'Plant.Alarm'
```

### ラダー（LD）
AND/OR/NOT演算を持つビジュアルコンタクト/コイルロジックエディター。

### 設定
- **言語** — ST、IL、またはLD
- **間隔** — スキャンサイクル（ミリ秒、デフォルト100ms）
- **有効** — アクティブ/非アクティブ
"""),
            ["plcprogram"] = new("⚙️ PLCエディター", """
## PLCエディター

構造化テキストおよび命令リストプログラム用のコードエディター。

### 構文チェック
デプロイ前にプログラムを検証するには**構文チェック**をクリックします。

### STクイックリファレンス
- `READ('パス')` / `WRITE('パス', 値)` — 変数アクセス
- `IF...THEN...ELSIF...ELSE...END_IF` — 条件分岐
- `FOR...TO...DO...END_FOR` — ループ
- `WHILE...DO...END_WHILE` — Whileループ
- `CASE...OF...END_CASE` — スイッチ
- `:=` — 代入
"""),
            ["screens"] = new("🖼️ 画面", """
## 画面

インタラクティブなビジュアライゼーションを持つHMI画面。

### レイアウトモード
- **SVG（固定）** — 絶対位置指定の固定サイズキャンバス
- **レスポンシブ** — 画面サイズに適応するCSSグリッドレイアウト

### 画面の作成
1. 画面グループを選択
2. ツールバーの🖼️をクリック
3. ダブルクリックで画面エディターを開く

### シンボルタイプ
矩形、円、楕円、テキスト、線、ゲージ、インジケーター、SVG、アラームリスト、HDAチャート、HDAグリッド、イベントログ、入力フィールド、IPカメラ、レシピ、週間プランナー、画面埋め込み、イメージマップ、トレンド、スイッチ、ロータリースイッチ、ノブ、スライダー、ボタン、アニメーションテキスト

### データバインディング
シンボルは**VariablePath**を介して変数にバインドできます。バインディングはランタイムでリアルタイムに更新されます。
"""),
            ["screen"] = new("🖼️ 画面エディター", """
## 画面エディター

HMIレイアウトを作成するためのビジュアル画面デザイナー。

### シンボルの追加
- **ツールボックス**パネルで図形やウィジェットを追加
- **シンボルライブラリ**で既製の産業用SVGシンボルを使用
- キャンバスをクリックして選択したツールを配置

### シンボルの編集
- クリックで選択、ドラッグで移動
- ハンドルでサイズ変更
- Ctrlキーで複数選択
- プロパティパネルにすべてのシンボル設定を表示

### シンボルプロパティ
- **変数パス** — OPC変数にバインド
- **塗りつぶし/ストローク/ラベル** — 外観
- **アニメーション** — 動的な色、表示/非表示、回転
- **コマンド** — クリックアクション（値の書き込み、ナビゲーション、切り替え）
"""),
            ["screeneditor"] = new("🖼️ 画面エディターパネル", """
## 画面エディターパネル

### キャンバスコントロール
- **クリック** — シンボルを選択
- **Ctrl+クリック** — 複数選択
- **ドラッグ** — 選択したシンボルを移動
- **ハンドルをドラッグ** — サイズ変更
- **Delete** — 選択したシンボルを削除

### 整列
整列ツールバーを使用して、複数の選択したシンボルを整列または配置します。

### グループ
複数のシンボルを選択し、**グループ化**でまとめてロックします。

### アニメーションプレビュー
エディターは定期的にアニメーションを更新し、動的な動作をプレビューします。
"""),
            ["recipe"] = new("📦 レシピ", """
## レシピ

レシピはSQLiteデータベースに名前付きの変数値セットを保存します。

### 仕組み
1. レシピに含める変数を定義
2. ランタイムで、サーバーがOPCコマンドを作成：`Load`、`Save`、`Activate`、`ActiveRecipeName`
3. 画面のレシピウィジェットを使用するか、スクリプトでコマンドを書き込み

### レシピ変数
各変数には：
- **インデックス** — 一意の数値キー（名前変更後も安定）
- **変数パス** — 読み取り/書き込み対象のOPC変数
- **表示名** — レシピエディターUIに表示
"""),
            ["scheduler"] = new("📅 スケジューラー", """
## スケジューラー

スケジュールに従って自動的に変数に値を書き込む週間プログラム。

### 仕組み
- 曜日ごとにタイムスロットを定義
- 各スロットは設定された値をターゲット変数に書き込み
- サーバーは毎分スケジュールを評価

### ユースケース
- HVAC設定値のスケジューリング（快適モード vs 省エネモード）
- 照明スケジュール
- 機器の起動/停止スケジュール
"""),
            ["report"] = new("📄 レポート", """
## レポート

グラフ、テーブル、値を含む自動生成レポート。

### セクション
- **チャート** — HDAデータからの折れ線/棒グラフ
- **テーブル** — タイムスタンプ付きの表形式HDAデータ
- **値** — 変数の現在または最後の値

### 設定
各セクションは変数パスを参照し、設定可能な時間範囲とタイトルを持ちます。
"""),
            ["user"] = new("👤 ユーザーとセキュリティ", """
## ユーザーとセキュリティ

### 認証
**エディターログイン**または**ランタイムログイン**が有効な場合、ユーザーは認証が必要です。

### ユーザープロパティ
- **ユーザー名/パスワード** — 資格情報（パスワードはPBKDF2-SHA256でハッシュ化）
- **グループ** — 権限用のユーザーグループに割り当て
- **自動ログオフ** — アイドルタイムアウト（秒）
- **パスワード有効期限** — N日後に変更を強制
- **初回ログイン時に変更必須** — ワンタイムパスワードリセット

### アクセスレベル
- **読み取り** — 閲覧のみ
- **書き込み** — 値の変更が可能
- **読み取り/書き込み** — フルアクセス
"""),
            ["usergroups"] = new("👥 ユーザーグループ", """
## ユーザーグループ

グループはユーザーのアクセス権限を定義します。

### プロパティ
- **名前** — グループ識別子
- **アクセスレベル** — 読み取り、書き込み、または読み取り/書き込み
- **エディターアクセス** — Webエディターへのログインを許可
- **ランタイムアクセス** — ランタイムビューアーへのログインを許可
"""),
            ["server"] = new("🖥️ サーバー設定", """
## サーバー設定

### OPC UAエンドポイント
サーバーは設定されたエンドポイントURL（デフォルト：`opc.tcp://localhost:14840/SimpleOpcFileServer`）でリッスンします。

### 主要設定
- **匿名を許可** — 未認証のOPC接続を許可
- **エディターログイン** / **ランタイムログイン** — ユーザー認証を要求
- **起動画面** — RuntimeViewerのデフォルト画面
- **診断ポート** — パフォーマンスメトリクスのHTTP API（デフォルト14841）
- **クラウドリレー** — SignalRクラウドハブ経由の接続
- **クラッシュメール** — クラッシュ通知のSMTP設定
"""),
            ["serverpanel"] = new("🖥️ サーバーパネル", """
## サーバーパネル

エディターから直接OPC UAサーバープロセスを管理します。

### 機能
- サーバープロセスの**起動/停止**
- **Windowsサービスとしてインストール** — バックグラウンドサービスとして実行
- **パフォーマンスダッシュボード** — リアルタイムCPU、メモリ、スレッド数
- **サブシステムパフォーマンス** — サーバーの`/diag`エンドポイントからの診断
- **プロジェクトサブシステム** — 設定済みのスクリプト、PLCプログラムなどを表示
"""),
            ["strings"] = new("🌐 ローカライゼーション（文字列）", """
## ローカライゼーション

文字列エディターは多言語HMIアプリケーション用の翻訳テキストを管理します。

### 仕組み
- 一意のIDと言語ごとの翻訳で文字列エントリを定義
- 画面ラベルで`@StringId`プレフィックスを使用して文字列を参照
- RuntimeViewerがアクティブな言語に基づいて文字列を解決
"""),
            ["images"] = new("🖼️ 画像リソース", """
## 画像リソース

画面で使用するラスター画像（PNG、JPG、SVG）を管理します。

### 使用方法
- Base64データURIとして画像をアップロード
- 画面の背景またはイメージマップウィジェットで画像を参照
- 画像はプロジェクトJSONファイル内に保存
"""),
            ["json"] = new("📝 JSON / AIエディター", """
## JSON / AIエディター

AI支援によるプロジェクトツリーの直接JSON編集。

### JSONエディター
- 選択したノードの生JSONを表示・編集
- **更新**をクリックしてツリーから同期
- **適用**をクリックして変更を書き戻し

### AIアシスタント
- 自然言語でリクエストを入力（例：「温度変数を10個追加」）
- AIが変更されたJSONを生成
- 差分を確認して適用をクリック
"""),
            ["git"] = new("🔀 Gitバージョン管理", """
## Gitパネル

Gitでプロジェクトファイルの変更を追跡します。

### 機能
- ファイルの状態を表示（変更、追加、削除）
- ファイルのステージング/アンステージング
- メッセージ付きで変更をコミット
- コミット履歴の表示
"""),
            ["opcbrowse"] = new("🔌 OPCブラウズ", """
## OPCブラウズパネル

ライブOPC UAサーバーアドレス空間を参照します。

### 機能
- **ツリーブラウザー** — OPC UAノード階層をナビゲート
- **監視** — ライブ更新でノード値をサブスクライブ
- **書き込み** — 監視中の値をダブルクリックして新しい値を書き込み
- サーバー実行時に自動接続
"""),
            ["datalogging"] = new("📊 データログビューアー", """
## データログビューアー

サーバーによって記録された履歴データを表示します。

### 機能
- 変数パスと時間範囲でデータを照会
- 折れ線グラフによる視覚化
- タイムスタンプと値を含むデータグリッド
- エクスポート機能
"""),
            ["symbollibrary"] = new("🏭 シンボルライブラリ", """
## シンボルライブラリ

HMI画面用の既製産業用SVGシンボル。

### 使用方法
1. ライブラリカテゴリを参照
2. シンボルをクリックして選択
3. シンボルがアクティブな画面にSVG要素として追加
4. 必要に応じてサイズ変更と位置調整

### カテゴリ
バルブ、ポンプ、モーター、タンク、パイプ、センサー、インジケーターなど。
"""),
            ["toolbox"] = new("🧰 ツールボックス", """
## ツールボックス

画面エディター用の基本図形とウィジェット。

### 図形ツール
- 矩形、円、楕円、線、テキスト

### ウィジェットツール
- ゲージ、インジケーター、入力フィールド、ボタン、スイッチ、ノブ、スライダー
- HDAチャート、HDAグリッド、イベントログ、トレンド
- レシピ、週間プランナー、IPカメラ
- 画面埋め込み、イメージマップ、アニメーションテキスト
"""),
            ["crossreference"] = new("🔗 クロスリファレンス", """
## クロスリファレンスパネル

プロジェクト全体で変数が使用されているすべての場所を検索します。

### 使用方法
1. 検索ボックスに変数パスを入力または選択
2. **検索**をクリックして、すべての画面、スクリプト、PLCプログラム、アラームなどをスキャン
3. 結果をクリックしてその場所に移動

### 別の方法
ツリーで変数を右クリック → **🔗 参照の検索**

### プレフィックス一致
**プレフィックス一致**を有効にして、フォルダーパス配下のすべての変数を検索します。
"""),
            ["watchtable"] = new("👁️ ウォッチテーブル", """
## ウォッチテーブル

ライブOPC UA変数値の監視と書き込み。

### 使用方法
1. 検索ボックスまたは右クリック → **👁️ ウォッチに追加**で変数を追加
2. OPC UAサブスクリプションによりリアルタイムで値が更新
3. 値を**ダブルクリック**して新しい値を書き込み
4. 各エントリの品質とタイムスタンプが表示

### ウォッチリスト
- **保存** — 現在の変数セットを名前付きリストとして保存
- **読み込み** — 以前保存したリストを復元
- リストはプロジェクトごとに`%APPDATA%/SimpleOpcFileServer/watchlists/`に保存
"""),
            ["problems"] = new("⚠️ 問題パネル", """
## 問題パネル

プロジェクト全体を検証し、設定のエラーと警告を報告します。

### チェック内容
- **壊れたバインディング** — 存在しない変数を参照する画面シンボル
- **スクリプトエラー** — Roslynによる C# / VB.NET 構文エラー
- **PLCエラー** — 構造化テキスト / 命令リスト / ラダー構文
- **アラームの問題** — 無効なしきい値、メッセージの欠落
- **レシピ/レポート** — 削除された変数への参照
- **サーバー設定** — エンドポイントの欠落、ユーザーなしのログイン
- **重複** — 画面、スクリプト、レシピなどの重複名

### 使用方法
**🔍 プロジェクトの検証**をクリックしてフルスキャンを実行。重大度（エラー/警告/情報）またはカテゴリでフィルタリングします。
"""),
            ["properties"] = new("🏷️ プロパティパネル", """
## プロパティパネル

選択したツリーノードまたは画面シンボルのプロパティを表示・編集します。

### 機能
- 選択したアイテムのすべての編集可能なプロパティ
- 型に適したエディター（テキスト、数値、チェックボックス、ドロップダウン）
- プロパティ変更の元に戻す/やり直しサポート
"""),
            ["keyboard"] = new("⌨️ キーボードショートカット", """
## キーボードショートカット

| ショートカット | アクション |
|--------------|----------|
| **Ctrl+N** | 新規プロジェクト |
| **Ctrl+O** | プロジェクトファイルを開く |
| **Ctrl+S** | 保存 |
| **Ctrl+Shift+S** | 名前を付けて保存 |
| **Ctrl+Z** | 元に戻す |
| **Ctrl+Y** | やり直し |
| **Ctrl+C** | コピー |
| **Ctrl+V** | 貼り付け |
| **Delete** | 選択を削除 |
| **Esc** | レイアウトの復元 / 最大化の終了 |
"""),
            ["camera"] = new("📷 カメラ", """
## IPカメラ

HMI画面のライブビデオ用IPカメラストリームを設定します。

### プロパティ
- **ストリームURL** — MJPEGまたはスナップショットURL
- **更新間隔** — スナップショットモードの更新レート
- **名前** — 表示名
"""),
        },

        // ───────────────────── CHINESE ─────────────────────
        ["zh"] = new(StringComparer.OrdinalIgnoreCase)
        {
            ["welcome"] = new("🏠 入门指南", """
## 欢迎使用服务器编辑器

这是一个基于Web的编辑器，用于配置 **Simple OPC File Server** — 一个从JSON配置文件读取地址空间的OPC UA服务器。

### 快速入门
1. **打开项目** — 文件 → 浏览服务器 或 Ctrl+O
2. **编辑变量** — 展开项目树并选择变量/文件夹
3. **配置画面** — 双击画面节点
4. **启动服务器** — 使用服务器面板启动/停止
5. **保存更改** — Ctrl+S 或 文件 → 保存

### 关键概念
- **变量** — 按文件夹组织的OPC UA地址空间节点
- **画面** — 带有SVG符号和数据绑定的HMI可视化
- **脚本** — 在服务器上周期运行的C#或VB.NET代码
- **PLC程序** — IEC 61131-3 结构化文本、指令表或梯形图
- **配方** — 存储在SQLite中的命名变量值集
- **调度器** — 基于时间的每周变量自动化

### 面板
使用**视图**菜单显示/隐藏面板。拖动面板选项卡重新组织布局。
"""),
            ["project"] = new("📁 项目结构", """
## 项目结构

项目文件（`.json`）在单个文件中包含所有配置：

- **变量** — 按文件夹组织，每个都有名称、类型和可选的报警/日志配置
- **脚本** — 带有可配置间隔的C#或VB.NET脚本
- **PLC程序** — 结构化文本（ST）、指令表（IL）或梯形图（LD）
- **画面** — 带有SVG/响应式布局和交互式符号的HMI画面
- **配方** — 可加载/保存到SQLite的命名值集
- **调度器** — 按计划写入值的每周程序
- **报表** — 带有图表、表格和值的自动生成PDF/HTML报表
- **用户** — 带有组、权限和自动注销的身份验证
- **服务器设置** — OPC端点、诊断、云中继、崩溃邮件

### 多项目
您可以同时打开多个项目文件。右键单击项目将其设为活动状态。
"""),
            ["variables"] = new("🏷️ 变量", """
## 变量

变量定义服务器的OPC UA地址空间。

### 组织
变量按**文件夹**组织。文件夹层次结构定义OPC UA浏览路径。
示例：`Plant.Furnace.Temperature` → OPC UA NodeId `"Plant.Furnace.Temperature"`

### 添加变量
- 选择文件夹或变量组节点
- 单击工具栏中的🏷️按钮（或编辑 → 添加变量）
- 新变量以默认`Double`类型创建

### 变量类型
`Double`、`Int32`、`Int64`、`Boolean`、`String`、`DateTime`、`Float`、`UInt16`、`UInt32`、`Byte`

### 功能
- **报警** — 基于限值或条件的报警
- **数据记录** — 将值变化记录到TimescaleDB/PostgreSQL
- **保持** — 在服务器重启时保持值
- **统计** — 在运行时跟踪最小/最大/平均/计数
- **初始值** — 设置服务器启动时的起始值
"""),
            ["variable"] = new("🏷️ 变量属性", """
## 变量属性

选择一个变量并使用**属性**面板进行配置：

| 属性 | 描述 |
|------|------|
| **名称** | 变量名称（OPC路径的一部分） |
| **类型** | 数据类型（Double、Int32、Boolean、String等） |
| **访问** | 读取、写入或读写 |
| **初始值** | 服务器启动时的起始值 |
| **保持** | 在重启时保存/恢复值 |
| **报警** | 配置报警阈值或条件 |
| **数据记录** | 启用值变化记录 |
| **统计** | 启用最小/最大/平均跟踪 |

### 驱动绑定
变量可以通过驱动程序（Modbus、S7、OPC UA客户端等）绑定到外部设备。驱动程序特定属性显示在JSON视图中。
"""),
            ["folder"] = new("📂 文件夹", """
## 文件夹

文件夹将变量组织成直接映射到OPC UA地址空间的层次结构。

### 创建文件夹
- 选择父文件夹或变量组
- 单击工具栏中的📁或编辑 → 添加文件夹

### 最佳实践
- 使用描述性名称：`Plant`、`Line1`、`Furnace`、`Motor`
- 保持层次结构浅（最多3-4层）
- 将相关变量分组在一起
"""),
            ["scripts"] = new("⚡ 脚本", """
## 脚本

脚本是在服务器上周期运行的C#或VB.NET代码。

### 脚本API
```csharp
Read("Plant.Temp")           // 读取变量值
ReadDouble("Plant.Temp")     // 读取为double
ReadInt("Plant.Count")       // 读取为int
ReadBool("Plant.Running")    // 读取为bool
Write("Plant.Output", 42.0)  // 写入值
Log("消息")                   // 写入服务器日志
OnChanged("Plant.Temp", e => { ... }) // 响应变化
```

### 创建脚本
1. 在树中选择脚本组
2. 单击工具栏中的⚡或编辑 → 添加脚本
3. 双击脚本打开脚本编辑器

### 配置
- **语言** — C#或VB.NET
- **间隔** — 执行间隔（毫秒）
- **启用** — 启动/停止脚本
"""),
            ["script"] = new("⚡ 脚本编辑器", """
## 脚本编辑器

脚本编辑器为C#和VB.NET脚本提供代码编辑环境。

### 功能
- 带行号的语法高亮
- **检查语法**按钮 — 使用Roslyn编译器验证
- 错误位置以行号显示
- **运行一次** — 立即执行脚本（服务器运行时）

### 提示
- 使用`OnChanged`进行事件驱动逻辑而非轮询
- 保持脚本专注 — 每个脚本一个目的
- 使用`Log()`进行调试
"""),
            ["plcprograms"] = new("⚙️ PLC程序", """
## PLC程序

三种语言的IEC 61131-3兼容程序：

### 结构化文本（ST）
```
temperature := READ('Plant.Temp');
IF temperature > 80.0 THEN
    WRITE('Plant.Alarm', TRUE);
END_IF;
```

### 指令表（IL）
```
LD 'Plant.Temp'
GT 80.0
ST 'Plant.Alarm'
```

### 梯形图（LD）
带有AND/OR/NOT运算的可视化触点/线圈逻辑编辑器。

### 配置
- **语言** — ST、IL或LD
- **间隔** — 扫描周期（毫秒，默认100ms）
- **启用** — 活动/非活动
"""),
            ["plcprogram"] = new("⚙️ PLC编辑器", """
## PLC编辑器

结构化文本和指令表程序的代码编辑器。

### 语法检查
单击**检查语法**在部署前验证程序。

### ST快速参考
- `READ('路径')` / `WRITE('路径', 值)` — 变量访问
- `IF...THEN...ELSIF...ELSE...END_IF` — 条件
- `FOR...TO...DO...END_FOR` — 循环
- `WHILE...DO...END_WHILE` — While循环
- `CASE...OF...END_CASE` — Switch
- `:=` — 赋值
"""),
            ["screens"] = new("🖼️ 画面", """
## 画面

带有交互式可视化的HMI画面。

### 布局模式
- **SVG（固定）** — 固定大小画布，绝对定位
- **响应式** — 适应屏幕大小的CSS网格布局

### 创建画面
1. 选择画面组
2. 单击工具栏中的🖼️
3. 双击打开画面编辑器

### 符号类型
矩形、圆形、椭圆、文本、线条、仪表、指示器、SVG、报警列表、HDA图表、HDA网格、事件日志、输入框、IP摄像头、配方、周计划、画面嵌入、图像映射、趋势、开关、旋转开关、旋钮、滑块、按钮、动画文本

### 数据绑定
符号可以通过**VariablePath**绑定到变量。绑定在运行时实时更新。
"""),
            ["screen"] = new("🖼️ 画面编辑器", """
## 画面编辑器

用于创建HMI布局的可视化画面设计器。

### 添加符号
- 使用**工具箱**面板添加形状和控件
- 使用**符号库**获取预制的工业SVG符号
- 单击画布放置选定的工具

### 编辑符号
- 单击选择，拖动移动
- 使用手柄调整大小
- 按住Ctrl进行多选
- 属性面板显示所有符号设置

### 符号属性
- **变量路径** — 绑定到OPC变量
- **填充/描边/标签** — 外观
- **动画** — 动态颜色、可见性、旋转
- **命令** — 单击操作（写入值、导航、切换）
"""),
            ["screeneditor"] = new("🖼️ 画面编辑器面板", """
## 画面编辑器面板

### 画布控制
- **单击** — 选择符号
- **Ctrl+单击** — 多选
- **拖动** — 移动选定的符号
- **拖动手柄** — 调整大小
- **Delete** — 删除选定的符号

### 对齐
使用对齐工具栏对齐或分布多个选定的符号。

### 组合
选择多个符号并使用**组合**将它们锁定在一起。

### 动画预览
编辑器定期更新动画以预览动态行为。
"""),
            ["recipe"] = new("📦 配方", """
## 配方

配方在SQLite数据库中存储命名的变量值集。

### 工作原理
1. 定义哪些变量是配方的一部分
2. 在运行时，服务器创建OPC命令：`Load`、`Save`、`Activate`、`ActiveRecipeName`
3. 在画面上使用配方控件或通过脚本写入命令

### 配方变量
每个变量具有：
- **索引** — 唯一数字键（重命名后保持稳定）
- **变量路径** — 要读取/写入的OPC变量
- **显示名称** — 在配方编辑器界面中显示
"""),
            ["scheduler"] = new("📅 调度器", """
## 调度器

按计划自动将值写入变量的每周程序。

### 工作原理
- 为每周的每一天定义时间段
- 每个时间段将配置的值写入目标变量
- 服务器每分钟评估计划

### 应用场景
- HVAC设定值调度（舒适模式与节能模式）
- 照明调度
- 设备启动/停止时间
"""),
            ["report"] = new("📄 报表", """
## 报表

带有图表、表格和值的自动生成报表。

### 区段
- **图表** — 来自HDA数据的折线/柱状图
- **表格** — 带有时间戳的表格式HDA数据
- **值** — 变量的当前或最后值

### 配置
每个区段引用变量路径，并具有可配置的时间范围和标题。
"""),
            ["user"] = new("👤 用户与安全", """
## 用户与安全

### 身份验证
当**编辑器登录**或**运行时登录**启用时，用户必须进行身份验证。

### 用户属性
- **用户名/密码** — 凭据（密码使用PBKDF2-SHA256哈希）
- **组** — 分配到用户组以获取权限
- **自动注销** — 空闲超时（秒）
- **密码过期** — N天后强制更改
- **首次登录时必须更改** — 一次性密码重置

### 访问级别
- **读取** — 仅查看
- **写入** — 可以修改值
- **读写** — 完全访问
"""),
            ["usergroups"] = new("👥 用户组", """
## 用户组

组定义用户的访问权限。

### 属性
- **名称** — 组标识符
- **访问级别** — 读取、写入或读写
- **编辑器访问** — 允许登录Web编辑器
- **运行时访问** — 允许登录运行时查看器
"""),
            ["server"] = new("🖥️ 服务器设置", """
## 服务器设置

### OPC UA端点
服务器在配置的端点URL上监听（默认：`opc.tcp://localhost:14840/SimpleOpcFileServer`）。

### 关键设置
- **允许匿名** — 允许未认证的OPC连接
- **编辑器登录** / **运行时登录** — 要求用户身份验证
- **启动画面** — RuntimeViewer中的默认画面
- **诊断端口** — 性能指标的HTTP API（默认14841）
- **云中继** — 通过SignalR云集线器连接
- **崩溃邮件** — 崩溃通知的SMTP配置
"""),
            ["serverpanel"] = new("🖥️ 服务器面板", """
## 服务器面板

从编辑器直接管理OPC UA服务器进程。

### 功能
- **启动/停止**服务器进程
- **安装为Windows服务** — 作为后台服务运行
- **性能仪表板** — 实时CPU、内存、线程数
- **子系统性能** — 来自服务器`/diag`端点的诊断
- **项目子系统** — 显示已配置的脚本、PLC程序等
"""),
            ["strings"] = new("🌐 本地化（字符串）", """
## 本地化

字符串编辑器管理多语言HMI应用程序的翻译文本。

### 工作原理
- 使用唯一ID和每种语言的翻译定义字符串条目
- 在画面标签中使用`@StringId`前缀引用字符串
- RuntimeViewer根据活动语言解析字符串
"""),
            ["images"] = new("🖼️ 图像资源", """
## 图像资源

管理画面中使用的光栅图像（PNG、JPG、SVG）。

### 用法
- 将图像作为Base64数据URI上传
- 在画面背景或图像映射控件中引用图像
- 图像存储在项目JSON文件中
"""),
            ["json"] = new("📝 JSON / AI编辑器", """
## JSON / AI编辑器

带有AI辅助的项目树直接JSON编辑。

### JSON编辑器
- 查看和编辑选定节点的原始JSON
- 单击**刷新**从树同步
- 单击**应用**写回更改

### AI助手
- 输入自然语言请求（例如"添加10个温度变量"）
- AI生成修改后的JSON
- 查看差异并单击应用
"""),
            ["git"] = new("🔀 Git版本控制", """
## Git面板

使用Git跟踪项目文件的更改。

### 功能
- 查看文件状态（已修改、已添加、已删除）
- 暂存和取消暂存文件
- 用消息提交更改
- 查看提交历史
"""),
            ["opcbrowse"] = new("🔌 OPC浏览", """
## OPC浏览面板

浏览实时OPC UA服务器地址空间。

### 功能
- **树浏览器** — 浏览OPC UA节点层次结构
- **监视** — 订阅节点值并实时更新
- **写入** — 双击监视的值写入新值
- 服务器运行时自动连接
"""),
            ["datalogging"] = new("📊 数据日志查看器", """
## 数据日志查看器

查看服务器记录的历史数据。

### 功能
- 按变量路径和时间范围查询数据
- 折线图可视化
- 带有时间戳和值的数据网格
- 导出功能
"""),
            ["symbollibrary"] = new("🏭 符号库", """
## 符号库

HMI画面的预制工业SVG符号。

### 用法
1. 浏览库类别
2. 单击符号选择
3. 符号作为SVG元素添加到活动画面
4. 根据需要调整大小和位置

### 类别
阀门、泵、电机、储罐、管道、传感器、指示器等。
"""),
            ["toolbox"] = new("🧰 工具箱", """
## 工具箱

画面编辑器的基本形状和控件。

### 形状工具
- 矩形、圆形、椭圆、线条、文本

### 控件工具
- 仪表、指示器、输入框、按钮、开关、旋钮、滑块
- HDA图表、HDA网格、事件日志、趋势
- 配方、周计划、IP摄像头
- 画面嵌入、图像映射、动画文本
"""),
            ["crossreference"] = new("🔗 交叉引用", """
## 交叉引用面板

查找变量在整个项目中使用的所有位置。

### 用法
1. 在搜索框中输入或选择变量路径
2. 单击**查找**扫描所有画面、脚本、PLC程序、报警等
3. 单击结果导航到该位置

### 另外
在树中右键单击变量 → **🔗 查找引用**

### 前缀匹配
启用**前缀匹配**以查找文件夹路径下的所有变量。
"""),
            ["watchtable"] = new("👁️ 监视表", """
## 监视表

监视和写入实时OPC UA变量值。

### 用法
1. 使用搜索框或右键单击 → **👁️ 添加到监视**来添加变量
2. 通过OPC UA订阅实时更新值
3. **双击**值写入新值
4. 每个条目显示质量和时间戳

### 监视列表
- **保存** — 将当前变量集保存为命名列表
- **加载** — 恢复之前保存的列表
- 列表按项目保存在`%APPDATA%/SimpleOpcFileServer/watchlists/`
"""),
            ["problems"] = new("⚠️ 问题面板", """
## 问题面板

验证整个项目并报告配置错误和警告。

### 检查内容
- **断开的绑定** — 引用不存在变量的画面符号
- **脚本错误** — 通过Roslyn的C# / VB.NET语法错误
- **PLC错误** — 结构化文本 / 指令表 / 梯形图语法
- **报警问题** — 无效阈值、缺少消息
- **配方/报表** — 引用已删除的变量
- **服务器配置** — 缺少端点、无用户的登录
- **重复** — 画面、脚本、配方等的重复名称

### 用法
单击**🔍 验证项目**进行完整扫描。按严重程度（错误/警告/信息）或类别过滤。
"""),
            ["properties"] = new("🏷️ 属性面板", """
## 属性面板

显示和编辑选定树节点或画面符号的属性。

### 功能
- 选定项目的所有可编辑属性
- 适合类型的编辑器（文本、数字、复选框、下拉列表）
- 属性更改的撤销/重做支持
"""),
            ["keyboard"] = new("⌨️ 键盘快捷键", """
## 键盘快捷键

| 快捷键 | 操作 |
|--------|------|
| **Ctrl+N** | 新建项目 |
| **Ctrl+O** | 打开项目文件 |
| **Ctrl+S** | 保存 |
| **Ctrl+Shift+S** | 另存为 |
| **Ctrl+Z** | 撤销 |
| **Ctrl+Y** | 重做 |
| **Ctrl+C** | 复制 |
| **Ctrl+V** | 粘贴 |
| **Delete** | 删除选定 |
| **Esc** | 恢复布局 / 退出最大化 |
"""),
            ["camera"] = new("📷 摄像头", """
## IP摄像头

为HMI画面上的实时视频配置IP摄像头流。

### 属性
- **流URL** — MJPEG或快照URL
- **刷新间隔** — 快照模式的更新率
- **名称** — 显示名称
"""),
        },
    };
}

/// <summary>
/// Help content with a title and markdown body.
/// </summary>
public record HelpContent(string Title, string Body);
