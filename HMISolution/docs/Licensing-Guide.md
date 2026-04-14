# HMISolution — Licensing Guide

> **Version:** 1.2 · **Date:** July 2025 · **Applies to:** Server, ServerEditorWeb, RuntimeViewer

---

## Table of Contents

1. [Overview](#1-overview)
2. [Architecture Overview](#2-architecture-overview)
3. [How Licensing Works](#3-how-licensing-works)
4. [License Generator (WPF GUI Tool)](#4-license-generator-wpf-gui-tool)
5. [License Tiers & Feature Limits](#5-license-tiers--feature-limits)
6. [License File Format](#6-license-file-format)
7. [Developer Quick-Start (End-to-End Walkthrough)](#7-developer-quick-start-end-to-end-walkthrough)
8. [Step-by-Step: Initial Setup (One-Time)](#8-step-by-step-initial-setup-one-time)
9. [Step-by-Step: Issuing a License to a Customer](#9-step-by-step-issuing-a-license-to-a-customer)
10. [Step-by-Step: Installing a License on a Target Machine](#10-step-by-step-installing-a-license-on-a-target-machine)
11. [How Validation Works at Runtime](#11-how-validation-works-at-runtime)
12. [Enforcement Details](#12-enforcement-details)
13. [Hardware Fingerprint (Machine ID)](#13-hardware-fingerprint-machine-id)
14. [Trial / Unlicensed Mode](#14-trial--unlicensed-mode)
15. [License Display in the Editor](#15-license-display-in-the-editor)
16. [Security Model](#16-security-model)
17. [Troubleshooting](#17-troubleshooting)
18. [API Reference](#18-api-reference)
19. [Known Issues & Fixes](#19-known-issues--fixes)

---

## 1. Overview

HMISolution uses an **RSA-signed license file** system.

### Key Principles

- **Asymmetric cryptography**: Only the developer can create licenses (private key). All deployed applications can verify them (public key).
- **Tamper-proof**: Any modification to the license JSON invalidates the RSA signature.
- **Machine-locking** (optional): Licenses can be bound to a specific machine's hardware fingerprint.
- **Expiration**: Licenses have an expiry date (or can be perpetual).
- **Feature gating**: Each tier defines limits on variables, drivers, scripts, screens, recipes, and features.
- **Graceful degradation**: If no valid license is found, the application runs in Trial mode with restricted limits.

---

## 2. Architecture Overview

The licensing system spans multiple projects in the solution. Each component has a distinct role:

| Component | Project | Role |
|---|---|---|
| **LicenseManager** | `SharedModels/LicenseManager.cs` | RSA key generation, license signing, signature verification, hardware fingerprinting, validation logic |
| **License / LicenseTiers** | `SharedModels/LicenseModel.cs` | Data models (`License`, `LicenseStatus`, `LicenseTiers` factory methods) |
| **LicenseGenerator.Wpf** | `LicenseGenerator.Wpf/` | WPF GUI for developers — generate key pairs, create/sign/export/validate licenses |
| **Server** | `Server/Program.cs` | Calls `Validate()` at startup, runs demo-expiry timer, enforces limits in the node manager |
| **Editor** | `ServerEditorWeb/` | Displays license status/limits in the Server Panel; enforces AI feature gate |
| **RuntimeViewer** | `RuntimeViewer/` | Validates license when loading a project |

### File Flow

```
Developer machine                         Customer machine
--------------------------                --------------------------
LicenseGenerator.Wpf                      project-folder/
  |                                         +-- nodes.json
  +-- Generate Key Pair                     +-- license.json  <-- delivered
  |     +-> private.pem  (SECRET)           |
  |     +-> public.txt   (embed)           Server / Editor / RuntimeViewer
  |                                         +-- LicenseManager.Validate()
  +-- Sign & Export                              +-> signature check
        +-> license.json  ----deliver--->         +-> expiration check
                                                  +-> machine ID check
```

---

## 3. How Licensing Works

### The Big Picture

```
+------------------------------+
|  DEVELOPER (you)             |
|                              |
|  1. GenerateKeyPair()        | ---- One-time setup
|     -> private.key (SECRET)  |      (GUI or code)
|     -> public.key (EMBED)    |
|                              |
|  2. Embed public key in      |
|     LicenseManager.cs        |
|     -> Ship with binaries    |
|                              |
|  3. For each customer:       |
|     SignLicense(license,      | ---- Per-customer
|                privateKey)   |      (GUI or code)
|     ExportLicense(license,   |
|                "license.json")|
+--------------+---------------+
               | Deliver license.json
               v
+------------------------------+
|  CUSTOMER MACHINE            |
|                              |
|  project-folder/             |
|  +-- nodes.json              |
|  +-- license.json  <-------- | ---- Placed by customer
|                              |
|  At startup:                 |
|  FindLicenseFile() -> path   |
|  Validate(path)              |
|    +-- Verify RSA signature  |
|    +-- Check expiration      |
|    +-- Check machine ID      |
|                              |
|  -> LicenseStatus (Valid/Fail)|
+------------------------------+
```

### Validation Flow

```
                    +-------------+
                    | license.json|
                    |  found?     |
                    +------+------+
                           |
                    No --- + --- Yes
                    |      |
                    v      v
               +--------+  Parse JSON
               | Trial  |     |
               | mode   |     v
               +--------+  +------------------+
                           | Verify RSA       |
                           | signature against |
                           | embedded pubkey  |
                           +--------+---------+
                                    |
                             Fail --+-- Pass
                             |      |
                             v      v
                        +--------+  Check
                        |Invalid |  expiration
                        |tampered|     |
                        +--------+     |
                              Expired -+-- Valid
                              |        |
                              v        v
                         +--------+  Check
                         |Expired |  machine ID
                         +--------+    |
                              Fail --- + --- Pass (or empty)
                              |              |
                              v              v
                         +---------+   +----------+
                         | Wrong   |   | Valid    |
                         | machine |   | Licensed |
                         +---------+   +----------+
```

---

## 4. License Generator (WPF GUI Tool)

HMISolution ships with a dedicated **WPF desktop application**

### Project Location

```
HMISolution/
+-- LicenseGenerator.Wpf/
    +-- LicenseGenerator.Wpf.csproj   (.NET 10, WPF)
    +-- App.xaml / App.xaml.cs         (dark-themed application)
    +-- MainWindow.xaml / .xaml.cs     (main UI & logic)
```

### How to Launch

Open the solution in Visual Studio, set **LicenseGenerator.Wpf** as the startup project, and press **F5**. Or from the command line:

```powershell
cd LicenseGenerator.Wpf
dotnet run
```

### UI Sections

The application window is organized into five sections:

```
+------------------------------------------------------+
|  Key HMISolution License Generator                    |
+------------------------------------------------------+
|  RSA Private Key                                      |
|  [Browse...]  [Generate New Key Pair]                 |
+------------------------------------------------------+
|  License Tier                                         |
|  v Trial | Starter | Professional | Enterprise       |
|  (auto-fills default limits for selected tier)        |
+------------------------------------------------------+
|  Customer Information                                 |
|  Licensed To .  Project Name                          |
|  Machine ID [This PC]  .  Validity (days)             |
+------------------------------------------------------+
|  Feature Limits                                       |
|  Variables . Drivers . Scripts . PLC . Screens        |
|  Recipes . [x] Data Logging . [x] AI Assistant        |
+------------------------------------------------------+
|  License Preview (JSON)                               |
|  { "Id": "a1b2c3d4e5f6", ... }                       |
+------------------------------------------------------+
|  [Validate Existing...]    [Preview] [Sign & Export]  |
+------------------------------------------------------+
```

### Feature Summary

| Feature | Description |
|---|---|
| **Generate Key Pair** | Creates an RSA-2048 key pair. Saves `private.pem` to your chosen folder and `public.txt` (Base64 public key) next to it. Displays a confirmation dialog with the public key preview. |
| **Browse Private Key** | Load an existing PEM private key file for signing |
| **Tier Selection** | Dropdown pre-fills all feature limits with the tier's defaults (Trial, Starter, Professional, Enterprise) |
| **Customer Info** | Enter customer name, project name, optional machine ID, and validity period |
| **This PC** button | Auto-fills the Machine ID field with the current machine's hardware fingerprint |
| **Feature Limits** | Customize individual limits (variables, drivers, scripts, PLC programs, screens, recipes, data logging, AI) |
| **Preview** | Shows the full license JSON before signing |
| **Sign & Export** | Signs the license with the private key and saves `license.json` via a Save dialog |
| **Validate Existing** | Opens an existing `license.json` file and verifies its RSA signature and status |

> **Tip**: The WPF tool uses the same `LicenseManager` and `LicenseTiers` classes from `SharedModels` -- it produces identical output to the code-based approach described in the following sections.

---

## 5. License Tiers & Feature Limits

| Feature | Demo | Trial | Starter | Professional | Enterprise |
|---|:---:|:---:|:---:|:---:|:---:|
| Max Variables | Unlimited | 20 | 100 | 1,000 | Unlimited |
| Max Drivers | Unlimited | 1 | 2 | Unlimited | Unlimited |
| Max Scripts | Unlimited | 2 | 5 | Unlimited | Unlimited |
| Max PLC Programs | Unlimited | 1 | 2 | Unlimited | Unlimited |
| Max Screens | Unlimited | 1 | 3 | Unlimited | Unlimited |
| Max Recipes | Unlimited | 0 | 3 | Unlimited | Unlimited |
| Data Logging | Yes | No | No | Yes | Yes |
| AI Assistant | Yes | No | No | Yes | Yes |
| Duration | **10 minutes** | 30 days | Custom | Custom | Perpetual |
| Machine Lock | No | No | Optional | Optional | Optional |

> **Demo mode**: When no `license.json` file is found, the application starts in Demo mode
> with full (Enterprise-level) features for **10 minutes**. After the grace period expires,
> it automatically degrades to Trial limits. Restart the application to get another 10-minute
> demo window, or add a license file for permanent access.

> `Unlimited` means `0` in the JSON. All integer limits follow the rule: `0 = unlimited`.

---

## 6. License File Format

A signed license file (`license.json`) looks like this:

```json
{
  "Id": "a1b2c3d4e5f6",
  "Tier": "Professional",
  "LicensedTo": "Acme Manufacturing GmbH",
  "ProjectName": "Plant-Berlin-Line3",
  "MachineId": "3f8a2b1c9d4e5f6a7b8c9d0e1f2a3b4c",
  "IssuedUtc": "2025-07-14T10:00:00.0000000Z",
  "ExpiresUtc": "2026-07-14T10:00:00.0000000Z",
  "MaxVariables": 1000,
  "MaxDrivers": 0,
  "MaxScripts": 0,
  "MaxPlcPrograms": 0,
  "MaxScreens": 0,
  "MaxRecipes": 0,
  "AllowDataLogging": true,
  "AllowAi": true,
  "Signature": "Base64EncodedRSASignature..."
}
```

### Field Reference

| Field | Type | Description |
|---|---|---|
| `Id` | `string` | Unique license identifier (auto-generated GUID, first 12 chars) |
| `Tier` | `string` | `"Trial"`, `"Starter"`, `"Professional"`, or `"Enterprise"` |
| `LicensedTo` | `string` | Customer/company name |
| `ProjectName` | `string` | Optional project or site name |
| `MachineId` | `string` | Hardware fingerprint hash (empty = not machine-locked) |
| `IssuedUtc` | `DateTime` | Issue date (UTC, ISO 8601) |
| `ExpiresUtc` | `DateTime` | Expiry date (UTC). `DateTime.MaxValue` = perpetual |
| `MaxVariables` | `int` | Max OPC variables (0 = unlimited) |
| `MaxDrivers` | `int` | Max loaded drivers (0 = unlimited) |
| `MaxScripts` | `int` | Max C#/VB.NET scripts (0 = unlimited) |
| `MaxPlcPrograms` | `int` | Max PLC programs (0 = unlimited) |
| `MaxScreens` | `int` | Max HMI screens (0 = unlimited) |
| `MaxRecipes` | `int` | Max recipe definitions (0 = unlimited) |
| `AllowDataLogging` | `bool` | Enable TimescaleDB/SQLite historian |
| `AllowAi` | `bool` | Enable AI assistant feature |
| `Signature` | `string` | RSA-SHA256 signature (Base64). **Not included in the signed payload.** |

---

## 7. Developer Quick-Start (End-to-End Walkthrough)

This section is a condensed, action-oriented guide for developers who need to generate a key pair, create a license, and deploy it. For full details on each step, see the sections that follow.

### Prerequisites

- Visual Studio with the **LicenseGenerator.Wpf** project (or `dotnet run` from the command line)
- A secure location for your private key (password manager, encrypted drive, etc.)

### 1. Generate the RSA-2048 Key Pair (one-time)

1. Run **LicenseGenerator.Wpf** (set as startup project → F5, or `cd LicenseGenerator.Wpf && dotnet run`).
2. Click **"Generate New Key Pair"** in the 🔐 RSA Private Key section.
3. Choose a save location for `private.pem`.
4. The tool saves two files:
   - **`private.pem`** — your signing key. ⚠️ **KEEP SECRET — never commit to Git.**
   - **`public.txt`** — Base64-encoded public key, saved next to the private key.
5. A confirmation dialog shows the public key preview.

> **Under the hood:** `LicenseManager.GenerateKeyPair()` calls `RSA.Create(2048)`, exports the private key as PEM and the public key as Base64.

### 2. Embed the Public Key in Shipped Binaries (one-time)

1. Open `SharedModels/LicenseManager.cs`.
2. Replace the placeholder on line 27–28:

```csharp
// BEFORE:
private const string EmbeddedPublicKey =
    "REPLACE_WITH_YOUR_PUBLIC_KEY";

// AFTER (paste the contents of public.txt):
private const string EmbeddedPublicKey =
    "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCg...your_key_here...";
```

3. Build the solution. All apps (Server, Editor, RuntimeViewer) will now verify signatures using this key.

> The public key can only **verify** — it cannot sign. It is safe to ship.

### 3. Create & Sign a License for a Customer

1. **Load your private key:** Click **"Browse…"** → select `private.pem` (or it's already loaded if you just generated it).
2. **Select a Tier:** Trial / Starter / Professional / Enterprise. Limits auto-populate from `LicenseTiers`.
3. **Fill Customer Info:**
   - **Licensed To** *(required)*: Customer name or company.
   - **Project Name** *(optional)*: Their HMI project name.
   - **Machine ID** *(optional)*: For machine-locked licenses, ask the customer for their hardware fingerprint (visible in Editor → Server Panel → License). Leave empty for floating licenses. Click **"This PC"** to use the local machine.
   - **Validity (days)**: Default 365. Use 0 for perpetual (Enterprise).
4. **Adjust limits** if needed (the defaults come from `LicenseTiers` but every field is editable).
5. Click **"Preview"** to inspect the unsigned JSON.
6. Click **"Sign & Export license.json"** → choose output path → done.

### 4. Deliver & Install

1. Send `license.json` to the customer (email, portal, USB).
2. Customer places `license.json` **next to their `nodes.json`** file.
3. Customer restarts the application — license is detected automatically.

### 5. Validate an Existing License

Click **"🔍 Validate Existing…"** (bottom-left of the WPF tool), select any `license.json`, and the tool checks:
- **Signature** — RSA-SHA256 against the embedded public key
- **Expiration** — `ExpiresUtc` vs. current UTC time
- **Machine ID** — hardware fingerprint match (if set)

### Summary Cheat Sheet

| Step | Action | Output |
|---:|---|---|
| 1 | Generate Key Pair | `private.pem` (secret) + `public.txt` (embed) |
| 2 | Embed public key | Update `EmbeddedPublicKey` in `LicenseManager.cs`, rebuild |
| 3 | Create license | Fill form in WPF tool → **Sign & Export** → `license.json` |
| 4 | Deliver | Customer places `license.json` next to `nodes.json` |
| 5 | Validate | **Validate Existing…** button in WPF tool |

---

## 8. Step-by-Step: Initial Setup (One-Time)

This is done **once** by the developer before shipping the product. The goal is to generate an RSA key pair and embed the public key in the binaries.

### Step 1 -- Generate the RSA Key Pair

#### Option A -- Use the WPF License Generator (Recommended)

1. Launch **LicenseGenerator.Wpf** (see [Section 4](#4-license-generator-wpf-gui-tool))
2. In the **RSA Private Key** section, click **Generate New Key Pair**
3. Choose a save location — the tool saves `private.pem` there and `public.txt` (Base64 public key) next to it
4. Store the private key in a secure vault (password manager, encrypted drive, HSM)
5. Open `public.txt` and copy the Base64 string into `SharedModels/LicenseManager.cs` (see Step 2 below)

```
+--------------------------------------------------+
|  RSA Private Key                                  |
|  C:\secure\private.pem                 [Browse..] |
|  [Generate New Key Pair]  ✅ New key pair generated |
+--------------------------------------------------+
        |
        +-->  private.pem  saved to chosen folder
        +-->  public.txt   saved next to it
```

#### Option B -- Use Code (Console / C# REPL)

Create a small console application or use the .NET Interactive / C# REPL:

```csharp
using SharedModels;

// Generate RSA-2048 key pair
var (privateKeyPem, publicKeyBase64) = LicenseManager.GenerateKeyPair();

// Save the private key to a SECURE location -- NEVER ship this
File.WriteAllText(@"C:\secure\private.pem", privateKeyPem);

// Print the public key to embed in the source code
Console.WriteLine("=== PUBLIC KEY (embed this in LicenseManager.cs) ===");
Console.WriteLine(publicKeyBase64);
```

#### What You Get

| Output | Format | Where It Goes |
|---|---|---|
| **Private key** (`private.pem`) | PEM (`-----BEGIN RSA PRIVATE KEY-----...`) | Your secure vault. Never share. Never commit to Git. |
| **Public key** (`public.txt`) | Base64 string | Embedded in `LicenseManager.cs` → `EmbeddedPublicKey` → shipped with the binaries |

### Step 2 -- Embed the Public Key

Open `SharedModels/LicenseManager.cs` and replace the placeholder:

```csharp
// BEFORE (placeholder):
private const string EmbeddedPublicKey =
    "REPLACE_WITH_YOUR_PUBLIC_KEY";

// AFTER (your actual public key):
private const string EmbeddedPublicKey =
    "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCg...your_key_here...";
```

### Step 3 -- Build and Ship

Build the solution. The public key is now compiled into every binary (Server, Editor, RuntimeViewer). These binaries can **verify** licenses but **cannot create** them.

### Summary

```
GenerateKeyPair()
    |
    +-->  private.pem   -> Store securely (your vault)
    |                      Used to SIGN licenses
    |
    +-->  public.txt    -> Embed in LicenseManager.cs -> EmbeddedPublicKey
                           Used to VERIFY licenses
                           Safe to ship with binaries
```

> **CRITICAL**: If the private key is compromised, anyone can generate valid licenses. Store it in a password manager, encrypted USB drive, or HSM. Never commit it to version control.

---

## 9. Step-by-Step: Issuing a License to a Customer

### Prerequisites

- Your RSA private key file (`private.pem` from the initial setup)
- Customer information (name, project, machine ID if machine-locked)

### Using the WPF License Generator (Recommended)

The fastest way to issue a license is with the **LicenseGenerator.Wpf** GUI (see [Section 4](#4-license-generator-wpf-gui-tool)):

1. **Load your private key** -- click **Browse...** and select your `private.pem` file
2. **Select a tier** -- choose Trial / Starter / Professional / Enterprise from the dropdown (auto-fills limits)
3. **Enter customer info** -- fill in *Licensed To*, *Project Name*, *Machine ID* (or leave empty for floating), and *Validity (days)*
4. **Adjust feature limits** -- override any auto-filled defaults if needed
5. **Click Preview** -- verify the JSON in the preview pane
6. **Click Sign & Export** -- choose the output path and the signed `license.json` is saved

```
+--------------------------------------------------------+
|  Professional              Variables: Unlimited         |
|  Acme Manufacturing GmbH   Drivers: Unlimited           |
|  Plant-Berlin-Line3        Scripts: Unlimited            |
|  3f8a2b1c... (365 days)   Data Logging: Yes             |
+--------------------------------------------------------+
|  [Validate Existing...]  [Preview]  [Sign & Export]     |
+--------------------------------------------------------+
```

> To validate a license you previously created, click **Validate Existing...** and open the `license.json` file.

> **Note:** The WPF tool will warn you if no private key is loaded and offer to export an unsigned license. Unsigned licenses only work when the `EmbeddedPublicKey` is still set to the placeholder value (development mode).

### Using Code (Alternative)

If you prefer the programmatic approach, follow the steps below.

### Step 1 -- Get the Customer's Machine ID (Optional)

If you want to lock the license to a specific machine, the customer needs to run this on their target machine:

```csharp
using SharedModels;
Console.WriteLine("Machine ID: " + LicenseManager.GetMachineId());
```

Or they can find it in the **Editor** -> **Server Panel** -> **License** section, which displays:
```
Machine ID: 3f8a2b1c9d4e5f6a7b8c9d0e1f2a3b4c
```

If you **don't** want machine-locking, leave `MachineId` as an empty string -- the license will work on any machine.

### Step 2 -- Create the License Object

Use the built-in tier factory methods:

```csharp
using SharedModels;

// Option A: Use a predefined tier
var license = LicenseTiers.CreateProfessional(
    licensedTo: "Acme Manufacturing GmbH",
    machineId:  "3f8a2b1c9d4e5f6a7b8c9d0e1f2a3b4c",  // or "" for no lock
    daysValid:  365
);
license.ProjectName = "Plant-Berlin-Line3";

// Option B: Use a different tier
var license = LicenseTiers.CreateStarter("Small Corp", "", daysValid: 180);

// Option C: Enterprise (perpetual, unlimited)
var license = LicenseTiers.CreateEnterprise("Big Corp", "");

// Option D: Custom limits (manual creation)
var license = new License
{
    Tier = "Custom",
    LicensedTo = "Special Customer",
    MachineId = "",
    ExpiresUtc = DateTime.UtcNow.AddDays(90),
    MaxVariables = 500,
    MaxDrivers = 3,
    MaxScripts = 10,
    MaxPlcPrograms = 5,
    MaxScreens = 10,
    MaxRecipes = 5,
    AllowDataLogging = true,
    AllowAi = false
};
```

### Step 3 -- Sign the License

```csharp
// Load your private key
var privateKey = File.ReadAllText(@"C:\secure\private.pem");

// Sign -- this sets the Signature field on the license object
LicenseManager.SignLicense(license, privateKey);
```

### Step 4 -- Export to File

```csharp
// Export as a pretty-printed JSON file
LicenseManager.ExportLicense(license, @"C:\licenses\acme-license.json");
```

### Step 5 -- Deliver to Customer

Send the `license.json` file to the customer via a secure channel (email, download portal, USB). Instruct them to place it in their project folder (see Section 8).

### Complete Example (All-in-One Script)

```csharp
using SharedModels;

// --- Configuration -----------------------------------------------
var privateKey = File.ReadAllText(@"C:\secure\private.pem");
var customerName = "Acme Manufacturing GmbH";
var machineId = "3f8a2b1c9d4e5f6a7b8c9d0e1f2a3b4c"; // from customer
var outputPath = @"C:\licenses\acme-license.json";

// --- Create ------------------------------------------------------
var license = LicenseTiers.CreateProfessional(customerName, machineId, daysValid: 365);
license.ProjectName = "Plant-Berlin-Line3";

// --- Sign --------------------------------------------------------
LicenseManager.SignLicense(license, privateKey);

// --- Export ------------------------------------------------------
LicenseManager.ExportLicense(license, outputPath);

Console.WriteLine($"License created: {outputPath}");
Console.WriteLine($"  Tier:       {license.Tier}");
Console.WriteLine($"  Licensed to: {license.LicensedTo}");
Console.WriteLine($"  Expires:    {license.ExpiresUtc:yyyy-MM-dd}");
Console.WriteLine($"  Machine:    {(string.IsNullOrEmpty(license.MachineId) ? "Any" : license.MachineId[..8] + "...")}");
Console.WriteLine($"  ID:         {license.Id}");
```

---

## 10. Step-by-Step: Installing a License on a Target Machine

### For the Customer

1. **Receive** the `license.json` file from the developer
2. **Place** it in the **same folder** as your `nodes.json` project file:

```
project-folder/
+-- nodes.json         <-- your project configuration
+-- license.json       <-- place the license file here
+-- screens/
+-- scripts/
+-- ...
```

3. **Restart** the Server, Editor, or RuntimeViewer -- the license is detected automatically

### File Discovery Rules

The system searches for the license file in this order:

1. `license.json` (exact name) in the same directory as `nodes.json`
2. `License.json` (case-insensitive match)
3. `*.license.json` (any file ending in `.license.json`)

> The first match wins. Only one license file should be present.

### Verification

**In the Server console**, you'll see:
```
License: Professional -- License valid.
Licensed to: Acme Manufacturing GmbH
```

**In the Editor** -> **Server Panel** -> **License**, you'll see:
- Tier badge (green for valid, yellow for trial/expired)
- Feature limits table
- Expiration date and days remaining
- Machine ID

---

## 11. How Validation Works at Runtime

### Where Validation Happens

Validation runs at startup in **all three processes**:

| Process | Location | Code |
|---|---|---|
| **Server** | `Program.cs` line 41-45 | `LicenseManager.FindLicenseFile()` -> `Validate()` |
| **Editor** | `NodeEditorService.cs` line 125-126 | Called when a project is loaded |
| **RuntimeViewer** | `ProjectService.cs` line 50-51 | Called when the project JSON is loaded |

### Validation Steps (in order)

```
1. FindLicenseFile(configPath)
   -> Searches for license.json next to nodes.json
   -> Returns null if not found -> Trial mode

2. Validate(licensePath)
   a. Read and parse the JSON file
   b. If no file found -> Demo mode (full features for 10 minutes, then degrades to Trial)
   c. Verify RSA-SHA256 signature:
      - Build canonical payload (sorted keys, no Signature field)
      - RSA.VerifyData(payload, signature, SHA256, PKCS1)
      - If public key is still placeholder -> skip (dev mode)
   d. Check ExpiresUtc < DateTime.UtcNow -> expired
   e. Check MachineId matches GetMachineId() -> wrong machine
   f. All passed -> LicenseStatus.IsValid = true

3. Result cached in LicenseManager.Current (static)

4. Demo expiry check (Server only):
   - A background timer fires every 30 seconds
   - LicenseManager.CheckDemoExpiry() checks if 10 minutes have elapsed
   - When expired: degrades to Trial limits, re-enforces license limits on the loaded model
   - Timer stops after degradation (permanent until restart with a license file)
```

### Canonical Payload (Signed Data)

The signature covers a **deterministic JSON string** of all fields except `Signature`, with sorted keys and no whitespace:

```json
{"AllowAi":true,"AllowDataLogging":true,"ExpiresUtc":"2026-07-14T10:00:00.0000000Z","Id":"a1b2c3d4e5f6","IssuedUtc":"2025-07-14T10:00:00.0000000Z","LicensedTo":"Acme Manufacturing GmbH","MachineId":"3f8a2b...","MaxDrivers":0,"MaxPlcPrograms":0,"MaxRecipes":0,"MaxScreens":0,"MaxScripts":0,"MaxVariables":1000,"ProjectName":"Plant-Berlin-Line3","Tier":"Professional"}
```

This ensures that:
- Reordering JSON fields doesn't break the signature
- Pretty-printing or reformatting the file doesn't break the signature
- Only the `Signature` field itself is excluded (it would be circular)

---

## 12. Enforcement Details

License enforcement runs in the **Server's node manager** (`SimpleFileServerNodeManager`) during address space creation. Enforcement is **skipped in DEBUG builds** to allow unrestricted development.

### What Gets Enforced

| Limit | Enforcement Action |
|---|---|
| `MaxVariables` exceeded | Extra variables are **trimmed** from the folder tree (not created in OPC UA) |
| `MaxDrivers` exceeded | Extra drivers are **unloaded and disposed** |
| `MaxScripts` exceeded | Extra scripts are **disabled** (`Enabled = false`) |
| `MaxPlcPrograms` exceeded | Extra PLC programs are **disabled** |
| `MaxScreens` exceeded | Warning logged (screens still load but count is noted) |
| `MaxRecipes` exceeded | Extra recipes are **disabled** |
| `AllowDataLogging = false` | Database configuration is **set to null** (data logging fully disabled) |
| `AllowAi = false` | Enforced at the editor UI level |

### Enforcement Events

All limit violations are:
1. Logged to the **Serilog** console/file output as warnings
2. Recorded in the **Event Journal** (SQLite) under category `"System"`, source `"License"`

Example server log:
```
[WRN] License limit: 150 variables exceeds limit of 100 (Starter). Extra variables will not be created.
[WRN] License limit: Data logging not allowed in Trial tier. Database disabled.
```

### IsWithinLimit Helper

For custom enforcement in your code:

```csharp
var status = LicenseManager.Current;
if (!status.IsWithinLimit(currentVariableCount, status.License.MaxVariables))
{
    // Over the limit
}
```

The rule: `max == 0` means unlimited, otherwise `current <= max`.

---

## 13. Hardware Fingerprint (Machine ID)

### How It's Calculated

```csharp
MachineName | OSDescription | ProcessorCount | FirstPhysicalMACAddress
    |
SHA-256 hash -> first 32 hex characters
```

| Component | Example | Source |
|---|---|---|
| Machine name | `PROD-HMI-01` | `Environment.MachineName` |
| OS description | `Microsoft Windows 10.0.19045` | `RuntimeInformation.OSDescription` |
| Processor count | `8` | `Environment.ProcessorCount` |
| MAC address | `A0B1C2D3E4F5` | First UP, non-loopback NIC, sorted by name |

### Stability

The fingerprint is stable across:
- Reboots
- Application updates
- .NET runtime updates
- Network reconnections

The fingerprint **will change** if:
- Machine name is changed
- OS is reinstalled (changes OS description)
- Primary network adapter is replaced (changes MAC)
- CPU count changes (e.g., VM reconfiguration)

### Machine-Locked vs. Floating License

| `MachineId` value | Behavior |
|---|---|
| Empty string `""` | **Floating** -- license works on any machine |
| SHA-256 hash | **Locked** -- license only works on the machine with that fingerprint |

---

## 14. Demo & Trial Mode

When no valid license is found (missing file), the system starts in **Demo mode** with full features for **10 minutes**, then degrades to **Trial mode**:

### Demo Mode (first 10 minutes)

All features are unlocked (Enterprise-level). The server console shows:
```
License: Demo — No license file found. Running in Demo mode (full features for 10 minutes).
Demo mode: full features for 10 minutes (started 14:30:00 UTC)
```

The editor shows a countdown timer:
```
⏱️ Demo: 8:42 remaining — full features active
```

### Trial Mode (after demo expires, or expired/tampered license)

| Limit | Trial Value |
|---|---|
| Max Variables | 20 |
| Max Drivers | 1 |
| Max Scripts | 2 |
| Max PLC Programs | 1 |
| Max Screens | 1 |
| Max Recipes | 0 |
| Data Logging | Disabled |
| AI Assistant | Disabled |

When the demo expires, the server logs:
```
Demo period expired — degrading to Trial mode with limited features.
```

When a license file is expired or invalid:
```
License: Trial -- License expired on 2025-06-01.
```

> **Note**: Restarting the application grants another 10-minute demo window. Add a `license.json` file to the project folder for permanent access.

---

## 15. License Display in the Editor

The **Server Panel** in the web editor shows the current license status:

```
License
+------------------------------------------+
|  [Professional]   Acme Manufacturing     |
|  License valid.                          |
|                                          |
|  Variables     Unlimited                 |
|  Drivers       Unlimited                 |
|  Scripts       Unlimited                 |
|  PLC Programs  Unlimited                 |
|  Screens       Unlimited                 |
|  Recipes       Unlimited                 |
|  Data Logging  Yes                       |
|  AI Assistant  Yes                       |
|  Expires       2026-07-14 (365 days)     |
|                                          |
|  Machine ID: 3f8a2b1c9d4e5f6a...        |
+------------------------------------------+
```

The Machine ID displayed here is what the customer should send you if you need to issue a machine-locked license.

---

## 16. Security Model

### Cryptographic Details

| Parameter | Value |
|---|---|
| Algorithm | RSA-2048 |
| Signature | RSA-SHA256 with PKCS#1 v1.5 padding |
| Payload format | Canonical JSON (sorted keys, compact, excludes Signature field) |
| Encoding | UTF-8 -> SHA-256 -> RSA sign/verify |
| Key format (private) | PEM (`-----BEGIN RSA PRIVATE KEY-----`) |
| Key format (public) | Base64-encoded DER (raw RSA public key) |

### Trust Model

```
Private Key (SECRET)           Public Key (SHIPPED)
  - Signs licenses              - Verifies signatures
  - Only developer has it       - Embedded in all binaries
  - Can create any tier         - Cannot create licenses
  - PEM format                  - Base64 format
```

### Attack Resistance

| Attack | Protection |
|---|---|
| Modify license JSON | RSA signature verification fails |
| Create fake license | Requires private key (attacker doesn't have it) |
| Copy license to another machine | Machine ID check fails (if machine-locked) |
| Use expired license | Expiration date check fails |
| Replace embedded public key | Requires recompiling binaries (code signing prevents this in production) |
| Replay old license | Each license has a unique `Id` (logging/audit trail) |

### What Is NOT Protected

- **Decompilation**: A determined attacker with access to the .NET assemblies could remove the license check entirely. For production use, consider:
  - .NET assembly obfuscation (e.g., Babel, Dotfuscator, ConfuserEx)
  - Code signing of all assemblies
  - Server-side license validation for cloud deployments
- **Clock manipulation**: Expiration checks use `DateTime.UtcNow`. On a machine with a manually set clock, expiration can be bypassed.

---

## 17. Troubleshooting

### "No license file found. Running in Trial mode."

**Cause**: The system could not find a `license.json` file.

**Fix**: Ensure the file is placed in the **same directory** as `nodes.json`:
```
C:\projects\myplant\nodes.json     <-- project file
C:\projects\myplant\license.json   <-- license file (SAME folder)
```

---

### "License signature is invalid. The file may have been tampered with."

**Cause**: The license JSON was modified after signing, or it was signed with a different private key than the embedded public key expects.

**Fix**:
1. Do NOT edit the license JSON file manually -- any change invalidates the signature
2. Re-sign the license with your private key if changes are needed (use **LicenseGenerator.Wpf** → load `private.pem` → recreate & sign)
3. Verify you embedded the correct public key (from `public.txt`) matching your private key

---

### "License expired on YYYY-MM-DD."

**Cause**: The `ExpiresUtc` date has passed.

**Fix**: Issue a new license with an updated expiration date. Use the **LicenseGenerator.Wpf** GUI or re-sign via code and deliver to the customer.

---

### "License is locked to a different machine."

**Cause**: The `MachineId` in the license doesn't match the current machine's fingerprint.

**Fix**:
1. Get the **new** Machine ID from the editor (Server Panel -> License -> Machine ID)
2. Issue a new license with the updated `MachineId` (use **LicenseGenerator.Wpf** or code)
3. Or issue a floating license (`MachineId = ""`)

Common reasons the Machine ID changed:
- VM was migrated/cloned
- Network adapter was replaced
- Machine was renamed
- OS was reinstalled

---

### "License validation is being skipped (dev mode)"

**Cause**: The `EmbeddedPublicKey` is still set to `"REPLACE_WITH_YOUR_PUBLIC_KEY"`.

**Fix**: This is normal during development. The signature check is skipped when the public key hasn't been configured yet. Complete the Initial Setup (Section 6) before shipping.

---

### License enforcement skipped in DEBUG builds

**By design**: The `EnforceLicenseLimits` method is wrapped in `#if !DEBUG`, so feature limits are not enforced during development. Build in Release mode for production.

---

## 18. API Reference

### `LicenseManager` (static class)

```csharp
namespace SharedModels;

public static class LicenseManager
{
    // --- Validation ------------------------------------------------

    /// Find license.json next to the given config file path.
    /// Returns null if not found.
    static string? FindLicenseFile(string configPath);

    /// Validate a license file. Returns the status (valid/invalid + details).
    /// Result is cached in LicenseManager.Current.
    static LicenseStatus Validate(string? licenseFilePath);

    /// Get the cached validation result from the last Validate() call.
    static LicenseStatus Current { get; }

    // --- Hardware Fingerprint --------------------------------------

    /// Generate the SHA-256 hardware fingerprint for this machine.
    static string GetMachineId();

    // --- Key Generation (developer tool) ---------------------------

    /// Generate a new RSA-2048 key pair.
    /// Returns (privateKeyPem, publicKeyBase64).
    static (string PrivateKeyPem, string PublicKeyBase64) GenerateKeyPair();

    // --- Signing (developer tool) ----------------------------------

    /// Sign a License object with the RSA private key (PEM).
    /// Sets the license.Signature property.
    static void SignLicense(License license, string privateKeyPem);

    /// Export a signed license as a pretty-printed JSON file.
    static void ExportLicense(License license, string filePath);
}
```

### `License` (model class)

```csharp
public class License
{
    string Id;                  // Unique identifier
    string Tier;                // "Trial", "Starter", "Professional", "Enterprise"
    string LicensedTo;          // Customer name
    string ProjectName;         // Optional project/site name
    string MachineId;           // Hardware fingerprint (empty = any machine)
    DateTime IssuedUtc;         // Issue date
    DateTime ExpiresUtc;        // Expiry date (DateTime.MaxValue = perpetual)
    int MaxVariables;           // 0 = unlimited
    int MaxDrivers;             // 0 = unlimited
    int MaxScripts;             // 0 = unlimited
    int MaxPlcPrograms;         // 0 = unlimited
    int MaxScreens;             // 0 = unlimited
    int MaxRecipes;             // 0 = unlimited
    bool AllowDataLogging;      // Enable historian
    bool AllowAi;               // Enable AI assistant
    string Signature;           // RSA-SHA256 Base64 (excluded from signed payload)
}
```

### `LicenseStatus` (runtime result)

```csharp
public class LicenseStatus
{
    bool IsValid;               // Whether the license passed all checks
    string Tier;                // Tier name or "Unlicensed" / "Trial"
    string LicensedTo;          // Customer name
    string Message;             // Human-readable status message
    DateTime ExpiresUtc;        // Expiration date
    int DaysRemaining;          // Days until expiry (int.MaxValue = perpetual)
    License? License;           // The full license object (or trial defaults)

    /// Check if a count is within the license limit (0 = unlimited).
    bool IsWithinLimit(int current, int max);
}
```

### `LicenseTiers` (factory methods)

```csharp
public static class LicenseTiers
{
    static License CreateTrial(string licensedTo, string machineId);
    static License CreateStarter(string licensedTo, string machineId, int daysValid = 365);
    static License CreateProfessional(string licensedTo, string machineId, int daysValid = 365);
    static License CreateEnterprise(string licensedTo, string machineId);
}
```

---

## Quick Reference Card

```
ONE-TIME SETUP (GUI):
  1. Launch LicenseGenerator.Wpf
  2. Click "Generate New Key Pair" -> saves private.pem + public.txt
  3. Store private.pem in secure vault (NEVER commit to Git)
  4. Copy contents of public.txt -> LicenseManager.cs -> EmbeddedPublicKey
  5. Build & ship

ONE-TIME SETUP (Code):
  1. var (privKey, pubKey) = LicenseManager.GenerateKeyPair();
  2. File.WriteAllText("private.pem", privKey);  // store securely
  3. Embed pubKey -> LicenseManager.cs -> EmbeddedPublicKey
  4. Build & ship

PER CUSTOMER (GUI):
  1. Launch LicenseGenerator.Wpf
  2. Browse... -> load your private.pem
  3. Select tier, fill customer info, adjust limits
  4. Click "Sign & Export" -> saves license.json
  5. Deliver license.json to customer

PER CUSTOMER (Code):
  1. var license = LicenseTiers.CreateProfessional("Customer", machineId);
  2. var privKey = File.ReadAllText("private.pem");
  3. LicenseManager.SignLicense(license, privKey);
  4. LicenseManager.ExportLicense(license, "license.json");
  5. Deliver license.json to customer

VALIDATE (GUI):
  1. Launch LicenseGenerator.Wpf
  2. Click "Validate Existing..." -> open license.json
  3. View signature, expiration, and machine-lock status

CUSTOMER INSTALL:
  1. Place license.json next to nodes.json
  2. Restart application
  3. Done!
```

---

## 19. Known Issues & Fixes

### WPF License Generator — Dark Theme Readability (Fixed)

**Issue:** The `MutedColor` used for all labels and secondary text (`#6C7086`) had a contrast ratio of only ~2.6:1 against the dark surface background (`#2A2A3C`). This made labels like "Licensed To", "Max Variables", "Machine ID", etc. nearly unreadable.

**Fix:** Updated `MutedColor` in `LicenseGenerator.Wpf/App.xaml` from `#6C7086` to `#9399B2`, which provides ~5:1 contrast while still appearing visually muted relative to the primary text color (`#CDD6F4`).

```xml
<!-- App.xaml — before -->
<Color x:Key="MutedColor">#6C7086</Color>

<!-- App.xaml — after -->
<Color x:Key="MutedColor">#9399B2</Color>
```

**Affected elements:** All `Label` controls, tier description text, key status text, and the status bar.

---

*Document generated from HMISolution source code — `SharedModels/LicenseManager.cs`, `SharedModels/LicenseModel.cs`, and `LicenseGenerator.Wpf/`*
