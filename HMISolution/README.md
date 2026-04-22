# 🧠 AI Core HMI

**Artificial Intelligence at the Core of Industrial Automation**

The first open-source HMI / SCADA platform with AI built into every layer — from anomaly detection and predictive alarms to natural-language data queries and AI-assisted project engineering. Built on .NET 10, OPC UA, Blazor, and Ollama.

[![GitHub Stars](https://img.shields.io/github/stars/ClodAlone/HMISolution?style=social)](https://github.com/ClodAlone/HMISolution)
[![Docker Pulls](https://img.shields.io/docker/pulls/clodprogea/aicorehmi)](https://hub.docker.com/r/clodprogea/aicorehmi)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![FDA 21 CFR Part 11](https://img.shields.io/badge/FDA%2021%20CFR%20Part%2011-Compliant-green)](docs/FDA_21_CFR_Part_11_Implementation_Summary.md)

---

## ✨ Highlights

🧠 **AI-Powered** - Anomaly detection, natural-language queries, AI-assisted engineering  
🏭 **Industrial-Grade** - OPC UA, 13+ drivers, 100K+ variables, <3s startup  
🔒 **FDA 21 CFR Part 11** - Electronic signatures, tamper-proof audit trails, validated workflows  
🐳 **Docker Ready** - Single-command deployment with embedded AI (Ollama + Mistral)  
⚡ **Modern Stack** - .NET 10, Blazor, TimescaleDB, real-time performance

---

## 🐳 Docker Quick Start

Deploy everything — including a local LLM — with a single Docker command:

### Option 1: AI Core HMI (Full Stack with AI)

```bash
# Pull the all-in-one image from Docker Hub
docker pull clodprogea/aicorehmi:latest

# Run with persistent volumes
docker run -d --name aicorehmi \
  -p 14840:14840 \
  -p 14841:14841 \
  -p 8080:8080 \
  -p 8081:8081 \
  -p 8088:8088 \
  -p 5432:5432 \
  -p 11434:11434 \
  -v hmi-data:/data \
  -v hmi-pgdata:/var/lib/postgresql/data \
  clodprogea/aicorehmi:latest
```

### Option 2: HMI All-in-One (Lightweight - No AI)

For a lighter deployment without AI features:

```bash
# Pull the lightweight all-in-one image
docker pull clodprogea/hmi-allinone:latest

# Run the container
docker run -d --name hmi-allinone \
  -p 5000:8080 \
  -p 5001:8081 \
  -p 5002:8082 \
  clodprogea/hmi-allinone:latest
```

**Quick Access (Lightweight)**:
- **HMI Server**: http://localhost:5000
- **HMI Viewer**: http://localhost:5001
- **HMI Editor**: http://localhost:5002

### 🌐 Access Points

After starting the container, access the services at:

| Service | URL | Description |
|---------|-----|-------------|
| **Web Editor** | http://localhost:8080 | Project editor with AI assistant |
| **Runtime Viewer** | http://localhost:8088 | Operator interface with NL query chat |
| **OPC UA Server** | opc.tcp://localhost:14840 | Industrial data server |
| **REST API** | http://localhost:14841 | Diagnostics and REST endpoints |
| **Ollama AI** | http://localhost:11434 | Local LLM service (Mistral) |
| **PostgreSQL** | localhost:5432 | Historical database (TimescaleDB) |

### 📦 Available Docker Images

Choose the deployment that fits your needs:

**All-in-One Images:**
```bash
# Full stack with AI (12.3GB)
docker pull clodprogea/aicorehmi:latest

# Lightweight without AI (4.1GB)
docker pull clodprogea/hmi-allinone:latest
```

**Individual Components:**
```bash
# Pull individual images
docker pull clodprogea/hmi-server:latest   # OPC UA Server (255MB)
docker pull clodprogea/hmi-editor:latest   # Web Editor (133MB)
docker pull clodprogea/hmi-viewer:latest   # Runtime Viewer (116MB)

# Or use docker-compose for orchestration
curl -O https://raw.githubusercontent.com/ClodAlone/HMISolution/master/docker-compose.allinone.yml
docker-compose -f docker-compose.allinone.yml up -d
```

## 🧠 AI Features

### 🔍 Anomaly Detection & Predictive Alarms

- Z-score statistical analysis on all monitored variables
- Automatic predictive alarms — fires before limits are breached
- Configurable thresholds and baseline windows
- Scales to 100K+ variables

### 💬 Natural-Language Data Queries

Ask questions about your process data in plain English:

- "What was the maximum temperature in AHU1 yesterday?"
- "Show me the average pressure over the last 2 hours"
- "Did any flow sensor spike this week?"

Powered by OpenAI, Gemini, Claude, or fully offline with Ollama.

### 🤖 AI-Assisted Project Engineering

- Generate variables and configurations from natural language
- Import PLC exports (Siemens TIA, KNX ETS, Modbus maps)
- Context-aware editing with project awareness
- Train local Ollama models on your conventions

### 🏠 Embedded Ollama — Fully Offline AI

- Runs entirely on your hardware
- Default model: Mistral
- Swap to Llama 3, CodeGemma, Phi-3, or any Ollama model
- Perfect for air-gapped industrial environments

### 🔔 AI-Driven Notifications

Anomaly alerts delivered across multiple channels:
- Email (SMTP)
- Telegram Bot
- WhatsApp Cloud API
- Amazon Alexa
- IP Speakers (Sonos / UPnP)

## ⚡ Platform Features

- **🖥️ Blazor Web Editor** - Full-featured browser-based project editor
- **🔗 OPC UA Server** - Built-in OPC UA server with GDS support
- **🏭 13 Industrial Drivers** - Modbus, S7, EtherNet/IP, KNX, MQTT, Sparkplug B, OPC UA Client, REST, TCP, SQL, CSV, Simulation
- **🚨 Alarm System** - Configurable alarms with multi-channel notifications
- **🏷️ Runtime Tag Browser** - Hierarchical OPC UA browser with live search
- **💾 Backup & Restore** - Automatic snapshots with retention management
- **🔐 Security** - PBKDF2-SHA256, user groups, password policies
- **📋 FDA 21 CFR Part 11** - Electronic signatures, tamper-proof audit trails, account lockout
- **🌍 Localization** - EN, DE, IT, FR, JA, ZH support
- **☁️ Cloud Bridge** - Extend to cloud with CloudBridge & CloudRelay

## 🔒 FDA 21 CFR Part 11 Compliance

Enterprise-ready regulatory compliance for pharmaceutical, biotech, and life sciences:

### Electronic Signatures
- **Re-authentication required** for critical operations (recipe changes, config modifications, user management)
- **Dual signature workflows** for high-risk operations
- **Reason-for-change enforcement** with full audit capture
- Username + full name + timestamp + meaning captured for every signature

### Tamper-Proof Audit Trail
- **Cryptographic hash chaining** - Each audit record contains SHA256 hash of previous record
- **SQLite with WAL mode** - Immutable, high-performance storage
- **Automatic integrity verification** on startup
- **7-year retention** (2555 days, configurable)
- **Complete audit log** - Login/logout, data changes, config changes, signature events, access denied

### Password & Access Controls
- **Strong password policies** - Configurable complexity, minimum length, expiration
- **Password history** - Prevents reuse of last 12 passwords
- **Account lockout** - Automatic lockout after failed login attempts (configurable)
- **Auto log-off** - Automatic session timeout for inactive users
- **User groups & permissions** - Role-based access control

### Configuration via Property Panel
- **Project-level settings** - All compliance features configurable through the editor UI
- **No manual JSON editing** - Settings saved with project and version controlled
- **Type-safe validation** - Editor prevents invalid configurations

### Compliance Reports
- User activity reports with filterable audit trail
- Signature verification and audit integrity checks
- Export to CSV for regulatory submission

**Documentation**:
- [FDA 21 CFR Part 11 Implementation Guide](docs/FDA_21_CFR_Part_11_Implementation_Summary.md)
- [Quick Start Guide](docs/FDA_21_CFR_Part_11_Quick_Start.md)
- [Configuration Migration](docs/Compliance_Configuration_Migration.md)


## 🎨 Screen Editor & Widgets

Design rich HMI screens with 21+ built-in widget types:

- Rectangle, Circle, Ellipse, Text
- Gauge, Indicator, SVG Symbol
- Alarm List, HDA Chart, HDA Grid
- Event Log, Alarm Analytics
- KPI Widget, EditBox
- IP Camera, Recipe
- Image Map, Realtime Trend
- Command Button, Animated Text
- Radio Group

## ⚡ IEC 61131-3 PLC Runtime

Built-in PLC runtime supporting:

- **ST** (Structured Text)
- **IL** (Instruction List)
- **LD** (Ladder Diagram) with visual editor

Plus Roslyn-powered scripting with **C#** and **VB.NET** — both with integrated breakpoint debuggers.

## 🚀 Performance at Scale

Real benchmarks on a 12-core workstation with .NET 10:

- **~3s** - Server startup with 100K variables
- **322K** - SQLite writes/sec (WAL mode)
- **506K** - SQLite writes/sec (in-memory)
- **335K** - TimescaleDB writes/sec (COPY)
- **275K** - TimescaleDB multi-producer writes/sec
- **100ms** - PLC cycle time

## 📡 REST API

Comprehensive REST API on port 14841:

- Read, write, and browse variables
- Manage alarms (acknowledge, confirm, shelve)
- Query historical data
- Recipe management
- Server diagnostics and health checks

Includes demo Blazor app with 7 interactive pages.

## 💎 Pricing

### Community Edition - **FREE**
- ✅ Unlimited I/O tags
- ✅ Web & desktop runtime
- ✅ 40+ widgets
- ✅ Script debugger
- ✅ OpenAI, Gemini, Ollama AI
- ✅ SQLite historian
- ✅ Basic drivers
- ✅ Full source code (MIT license)

### Professional - **$199/month**
- Everything in Community, plus:
- Anthropic Claude AI
- TimescaleDB historian (unlimited)
- Premium drivers (20+ protocols)
- Redundancy & failover
- SSO / LDAP / AD
- Predictive alarms
- Email/chat support (8x5)

### Enterprise - **Custom**
- Everything in Professional, plus:
- **FDA 21 CFR Part 11 compliance** - Electronic signatures, tamper-proof audit trails, password policies
- Unlimited instances / multi-site
- 24x7 dedicated support
- 1-hour critical SLA
- Custom AI integration
- White-labeling & OEM
- Priority feature development

## 🛠️ Building from Source

```bash
# Clone the repository
git clone https://github.com/ClodAlone/HMISolution.git
cd HMISolution

# Build and run with docker-compose
docker-compose -f docker-compose.allinone.yml build
docker-compose -f docker-compose.allinone.yml up -d
```

Or build .NET projects directly:

```bash
# Requires .NET 10 SDK
dotnet build Server/Server.csproj -c Release
dotnet build ServerEditorWeb/ServerEditorWeb.csproj -c Release
dotnet build RuntimeViewer/RuntimeViewer.csproj -c Release
```

## 📚 Documentation

- **Landing Page**: https://clodalone.github.io/HMISolution
- **Docker Guide**: [DOCKER-README.md](DOCKER-README.md)
- **Deployment**: [docs/DEPLOYMENT.md](docs/DEPLOYMENT.md)
- **Features**: [docs/FEATURES.md](docs/FEATURES.md)
- **Pricing**: [docs/Pricing.md](docs/Pricing.md)

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md) for details.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

Commercial licenses and enterprise support available - see [LICENSE.COMMERCIAL.md](LICENSE.COMMERCIAL.md).

## 🌟 Support

- ⭐ **Star this repo** if you find it useful
- 🐛 **Report issues** on [GitHub Issues](https://github.com/ClodAlone/HMISolution/issues)
- 💬 **Join discussions** on [GitHub Discussions](https://github.com/ClodAlone/HMISolution/discussions)
- 📧 **Contact**: sales@aicorehmi.com

---

**Built with ❤️ using .NET 10, OPC UA, Blazor & Ollama**

