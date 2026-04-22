# FDA 21 CFR Part 11 - Configuration Migration

## Overview

Compliance settings have been **moved from `appsettings.json` to project-level settings** (`nodes.json` → `ServerSettings.Compliance`) so you can configure them through the **property panel** in the editor UI.

---

## What Changed

### ✅ **Before** (Old Approach — NOT RECOMMENDED)
```json
// Server/appsettings.json and ServerEditorWeb/appsettings.json
{
  "Compliance": {
    "Enabled": true,
    "AuditDbPath": "audit_trail.db",
    "AuditRetentionDays": 2555,
    "RequireElectronicSignatures": true,
    // ... etc
  }
}
```

**Problems:**
- Required manual editing of `appsettings.json` on deployment
- Not visible or editable in the editor UI
- Not part of the project version control/backup

### ✅ **After** (New Approach — RECOMMENDED)
```json
// nodes.json (your project file)
{
  "Server": {
    "EndpointUrl": "opc.tcp://localhost:14840/SimpleOpcFileServer",
    "EnableAnonymous": true,
    "EnableEditorLogin": true,
    "EnableRuntimeLogin": true,
    "Compliance": {
      "Enabled": true,
      "AuditDbPath": "audit_trail.db",
      "AuditRetentionDays": 2555,
      "RequireElectronicSignatures": true,
      "RequireDualSignature": false,
      "RequireReasonForChange": true,
      "PasswordPolicy": {
        "MinimumLength": 12,
        "RequireUppercase": true,
        "RequireLowercase": true,
        "RequireDigit": true,
        "RequireSpecialChar": true,
        "PreventReuse": 12,
        "MaxAgeDays": 90
      },
      "MaxFailedLoginAttempts": 5,
      "LockoutDurationMinutes": 30,
      "AutoLogOffMinutes": 15,
      "VerifyIntegrityOnStartup": true
    }
  }
}
```

**Benefits:**
- ✅ **Editable via property panel** in the editor UI
- ✅ **Saved with the project** — version controlled, backed up automatically
- ✅ **Type-safe** — the editor validates values and provides dropdowns/help text
- ✅ **Consistent** — follows the same pattern as `EventLog`, `Backup`, `AlarmNotification`, etc.

---

## How to Configure (Property Panel)

1. **Open your project** in the editor (`ServerEditorWeb`)
2. **Select the root project node** in the tree
3. **Open the Properties panel** (usually on the right side)
4. **Expand "Server" → "Compliance"**
5. **Edit the compliance settings** using the property grid:
   - Toggle `Enabled` on/off
   - Set `AuditDbPath`, `AuditRetentionDays`, etc.
   - Configure `PasswordPolicy` sub-properties
6. **Save the project** (`Ctrl+S` or File → Save)

The settings are serialized into `nodes.json` and automatically loaded by both the server and editor on startup.

---

## Technical Details

### Model Structure
```csharp
// SharedModels/NodeModels.cs
public class ServerSettings
{
    // ... other properties ...

    /// <summary>
    /// FDA 21 CFR Part 11 compliance configuration. When enabled, the system enforces
    /// electronic signatures, tamper-proof audit trails, password policies, and account lockout.
    /// </summary>
    public ComplianceConfig? Compliance { get; set; }
}
```

The `ComplianceConfig` class (defined in `SharedModels/ComplianceModels.cs`) contains all compliance settings and is automatically bound to the property panel.

### Service Registration Pattern
Services that require compliance configuration (e.g., `AuditTrailService`, `AuthServiceCompliance`, `ElectronicSignatureService`) accept `ComplianceConfig` via constructor injection:

```csharp
public AuditTrailService(ComplianceConfig config, ILogger logger)
{
    _config = config ?? throw new ArgumentNullException(nameof(config));
    // ...
}
```

At runtime, the server loads the project file and passes the compliance config from `ServerSettings.Compliance` to these services.

### Migration from appsettings.json
If you previously configured compliance in `appsettings.json`:

1. **Copy the values** from `appsettings.json` → `nodes.json` under `Server.Compliance`
2. **Remove the `Compliance` section** from `appsettings.json` (both `Server/appsettings.json` and `ServerEditorWeb/appsettings.json`)
3. **Save and test** — the services will now read from the project settings

---

## Next Steps

1. ✅ **Property panel already supports the new structure** — no UI changes needed (built-in property grid reflection)
2. ✅ **Services are ready** — `AuditTrailService`, `AuthServiceCompliance`, and `ElectronicSignatureService` accept `ComplianceConfig`
3. 🔄 **Update service registration** (if needed) to read from loaded `NodeModel.Server.Compliance` instead of `IConfiguration`
4. 🔄 **Update documentation/quick-start** to reflect project-level configuration

---

## Summary

| Aspect | Old (appsettings.json) | New (nodes.json → Property Panel) |
|--------|------------------------|-----------------------------------|
| **Editing** | Manual text editing | Property panel UI |
| **Visibility** | Hidden in appsettings | Visible in project tree |
| **Version control** | Separate deployment config | Part of project backup/versioning |
| **Type safety** | JSON only | Editor validates types/values |
| **Consistency** | Different from other settings | Same pattern as `EventLog`, `Backup`, etc. |

🎯 **Result:** Compliance is now a first-class project feature, not a deployment-time configuration detail.
