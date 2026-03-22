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
    public HelpContent GetHelp(string context)
    {
        return _helpTopics.TryGetValue(context, out var help) ? help : _helpTopics["welcome"];
    }

    /// <summary>
    /// Get help based on the currently selected tree node type.
    /// </summary>
    public HelpContent GetHelpForNode(TreeNode? node)
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
            ResourceFolderNode rfn => rfn.ResourceKind switch
            {
                "Script" => "scripts",
                "PlcProgram" => "plcprograms",
                "Screen" => "screens",
                _ => "folder"
            },
            _ => "welcome"
        };
        return GetHelp(key);
    }

    /// <summary>
    /// Get help for a specific panel ID.
    /// </summary>
    public HelpContent GetHelpForPanel(string panelId)
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
        return GetHelp(key);
    }

    /// <summary>
    /// Get all available help topic keys for a table of contents.
    /// </summary>
    public List<(string Key, string Title, string Icon)> GetTableOfContents()
    {
        return
        [
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
        ];
    }

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
}

/// <summary>
/// Help content with a title and markdown body.
/// </summary>
public record HelpContent(string Title, string Body);
