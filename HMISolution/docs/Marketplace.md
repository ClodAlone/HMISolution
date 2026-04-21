# AI Core HMI - Marketplace & Extensions

**Extend AI Core HMI with community and commercial drivers, widgets, and integrations**

---

## 🎯 Marketplace Overview

The AI Core HMI Marketplace is a platform for:
- **Drivers**: Protocol and device connectivity
- **Widgets**: Custom visualization components
- **Templates**: Industry-specific screen layouts
- **Integrations**: ERP, MES, CMMS connectors
- **AI Models**: Pre-trained anomaly detection and predictive models
- **Themes**: Custom UI styling

---

## 📦 Extension Types

### 1. Protocol Drivers

Connect to industrial devices and systems.

**Examples:**
- Allen-Bradley ControlLogix (EtherNet/IP)
- Siemens S7-1500 (S7 protocol)
- Mitsubishi MELSEC (MC protocol)
- BACnet (building automation)
- Custom serial protocols

**Revenue Model:**
- Free (community-contributed)
- Paid (one-time or subscription)
- Marketplace commission: 30% for paid drivers

### 2. Custom Widgets

Reusable UI components for screens.

**Examples:**
- Advanced charts (Gantt, waterfall, radar)
- 3D tank visualizations
- Custom motor controls
- Energy dashboards
- Process flow diagrams

**Revenue Model:**
- Free or paid
- Widget packs (bundle pricing)

### 3. Industry Templates

Pre-built screen collections for specific industries.

**Examples:**
- Water treatment plant
- Pharmaceutical batch control
- Food & beverage
- Oil & gas SCADA
- Building automation

**Revenue Model:**
- Typically $99-$999 one-time
- Includes screens, scripts, sample data

### 4. AI Models

Pre-trained models for specific use cases.

**Examples:**
- Pump failure prediction
- Chiller optimization
- Quality defect detection
- Energy consumption forecasting

**Revenue Model:**
- Subscription-based ($49-$299/month)
- Usage-based (API calls)

### 5. Integrations

Connectors to external systems.

**Examples:**
- SAP ERP connector
- Rockwell FactoryTalk integration
- Microsoft Teams notifications
- AWS IoT integration
- PostgreSQL/MySQL connectors

**Revenue Model:**
- Free for common open-source targets
- Paid for enterprise systems

---

## 🏪 How the Marketplace Works

### For Users (Buyers)

1. **Browse** marketplace at https://marketplace.aicorehmi.com
2. **Preview** screenshots, documentation, ratings
3. **Trial** available for paid extensions (7-14 days)
4. **Purchase** with credit card or license key
5. **Install** via editor UI or CLI:
   ```bash
   aicorehmi install extension <extension-name>
   ```
6. **Rate & review** after use

### For Developers (Publishers)

1. **Develop** your extension
2. **Package** using CLI:
   ```bash
   aicorehmi package
   ```
3. **Submit** to marketplace for review
4. **Publish** after approval
5. **Earn revenue** (70% to developer, 30% marketplace fee)
6. **Update** and support your extension

---

## 🛠️ Extension Manifest Schema

Every extension requires a `manifest.json`:

```json
{
  "schema": "1.0",
  "id": "com.yourcompany.extension-name",
  "type": "driver" | "widget" | "template" | "integration" | "ai-model" | "theme",
  "name": "Extension Display Name",
  "version": "1.2.3",
  "description": "Brief description of what this extension does",
  "longDescription": "Detailed description with features, use cases, etc.",
  "author": {
    "name": "Your Company",
    "email": "support@yourcompany.com",
    "url": "https://yourcompany.com"
  },
  "license": "MIT" | "Apache-2.0" | "GPL-3.0" | "Commercial" | "proprietary",
  "pricing": {
    "model": "free" | "one-time" | "subscription" | "usage",
    "price": 0,
    "currency": "USD",
    "trialDays": 14
  },
  "compatibility": {
    "minVersion": "1.0.0",
    "maxVersion": "2.0.0",
    "platforms": ["windows", "linux", "docker"]
  },
  "tags": ["modbus", "plc", "industrial", "automation"],
  "category": "drivers/plc" | "widgets/charts" | "templates/water-treatment",
  "icon": "icon.png",
  "screenshots": ["screenshot1.png", "screenshot2.png"],
  "documentation": "README.md",
  "changelog": "CHANGELOG.md",
  "dependencies": [
    {
      "id": "com.microsoft.dotnet",
      "version": ">=8.0"
    }
  ],
  "permissions": [
    "network.connect",
    "filesystem.read",
    "opc.server.modify"
  ],
  "entryPoint": {
    "assembly": "YourExtension.dll",
    "class": "YourCompany.YourExtension.Driver"
  },
  "configuration": {
    "schema": "config-schema.json",
    "defaults": {
      "ipAddress": "192.168.1.100",
      "port": 502
    }
  }
}
```

---

## 🔌 Driver Development

### Driver Interface

Implement `IDriverConnector`:

```csharp
public interface IDriverConnector
{
    string DriverId { get; }
    string DisplayName { get; }
    Task<bool> ConnectAsync(DriverConfig config);
    Task DisconnectAsync();
    Task<object?> ReadAsync(string address);
    Task WriteAsync(string address, object value);
    Task<List<Tag>> BrowseTagsAsync();
    event EventHandler<ValueChangedEventArgs>? ValueChanged;
}
```

**Example:**

```csharp
public class MyModbusDriver : IDriverConnector
{
    public string DriverId => "com.example.modbus-extended";
    public string DisplayName => "Extended Modbus Driver";

    public async Task<bool> ConnectAsync(DriverConfig config)
    {
        // Your connection logic
        return true;
    }

    public async Task<object?> ReadAsync(string address)
    {
        // Parse address, read from device
        return 42.5;
    }

    // ... implement other methods
}
```

---

## 🎨 Widget Development

### Widget Component

Create a Blazor component:

```razor
@using SharedModels
@implements IWidgetComponent

<div class="my-custom-widget" style="width: @Symbol.Width; height: @Symbol.Height">
    <h3>@Title</h3>
    <p>Value: @CurrentValue</p>
</div>

@code {
    [Parameter] public ScreenSymbol Symbol { get; set; }
    [Parameter] public IOpcRuntimeClient Opc { get; set; }

    private string Title => Symbol.CustomProperties.GetValueOrDefault("Title", "Widget");
    private object? CurrentValue { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await Opc.SubscribeAsync(Symbol.VariablePath, value => {
            CurrentValue = value;
            StateHasChanged();
        });
    }
}
```

**Widget Metadata:**

```json
{
  "widgetType": "mycustomwidget",
  "displayName": "My Custom Widget",
  "icon": "🎛️",
  "category": "Industrial Controls",
  "defaultSize": { "width": 200, "height": 100 },
  "properties": [
    {
      "name": "Title",
      "type": "string",
      "default": "My Widget",
      "description": "Widget title"
    },
    {
      "name": "VariablePath",
      "type": "opc-variable",
      "required": true
    }
  ]
}
```

---

## 📜 Submission Guidelines

### Review Criteria

Extensions must meet these requirements:

1. **Functionality**
   - Works as described
   - No crashes or errors
   - Performance acceptable

2. **Code Quality**
   - Well-structured and documented
   - Follows C# / .NET conventions
   - No security vulnerabilities

3. **Documentation**
   - Clear README
   - Configuration examples
   - Troubleshooting guide

4. **Licensing**
   - Clear license statement
   - No license conflicts
   - Proper attribution of dependencies

5. **User Experience**
   - Professional appearance
   - Intuitive configuration
   - Good error messages

### Prohibited Content

Extensions may NOT:
- Contain malware, spyware, or viruses
- Collect user data without disclosure
- Violate intellectual property
- Include cryptominers
- Phone home without permission
- Use deceptive practices

---

## 💰 Revenue Sharing

| Extension Price | Developer Share | Marketplace Fee |
|-----------------|-----------------|-----------------|
| Free | 100% | 0% |
| $1-$99 | 70% | 30% |
| $100-$499 | 75% | 25% |
| $500+ | 80% | 20% |

**Payment Terms:**
- Monthly payouts (minimum $50 balance)
- PayPal, bank transfer, or wire
- Tax forms required (W-9 for US, W-8 for international)

**Subscription Revenue:**
- Same revenue share
- Recurring monthly payments

---

## 🏆 Certification Program

### Certified Extensions

Extensions can earn "Certified" badge by:
- Code review by AI Core HMI team
- Security audit passed
- 4+ star average rating
- 90-day support SLA commitment

**Benefits:**
- "Certified" badge on listing
- Featured placement
- Co-marketing opportunities
- Reduced marketplace fee (25% → 20%)

---

## 📊 Analytics & Metrics

Publishers get access to:
- Install/download counts
- Active users
- Rating trends
- Revenue reports
- Crash/error analytics (opt-in)

---

## 🚀 Getting Started

### 1. Set Up Development Environment

```bash
# Clone SDK
git clone https://github.com/ClodAlone/HMISolution-SDK

# Install CLI
dotnet tool install -g aicorehmi-cli

# Create extension scaffold
aicorehmi create driver MyDriver
```

### 2. Develop Your Extension

- Implement required interfaces
- Add configuration schema
- Write documentation
- Create examples

### 3. Test Locally

```bash
# Install in dev mode
aicorehmi install --local ./MyDriver

# Run tests
dotnet test
```

### 4. Package & Submit

```bash
# Package extension
aicorehmi package

# Validate
aicorehmi validate MyDriver.zip

# Submit to marketplace
aicorehmi publish MyDriver.zip
```

---

## 📚 Resources

- **Developer Docs**: https://docs.aicorehmi.com/extensions
- **SDK Repository**: https://github.com/ClodAlone/HMISolution-SDK
- **Example Extensions**: https://github.com/ClodAlone/HMISolution-Examples
- **Developer Forum**: https://forum.aicorehmi.com/c/extensions
- **API Reference**: https://api-docs.aicorehmi.com

---

## 💬 Support for Publishers

- **Developer forum**: https://forum.aicorehmi.com/c/dev
- **Discord #marketplace channel**: https://discord.gg/aicorehmi
- **Email**: marketplace@aicorehmi.com

---

## 📧 Contact

- **General inquiries**: marketplace@aicorehmi.com
- **Technical support**: dev-support@aicorehmi.com
- **Partnership opportunities**: partnerships@aicorehmi.com

---

**Start building and monetizing your AI Core HMI extensions today!** 🚀
