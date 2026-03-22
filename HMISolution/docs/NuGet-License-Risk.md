# HMI Solution — NuGet Package License Risk Report

> **Generated:** July 2025 · **Applies to:** all projects in `HMISolution.slnx`

---

## Table of Contents

1. [Summary](#1-summary)
2. [High Risk — Copyleft / Source Disclosure](#2-high-risk--copyleft--source-disclosure)
3. [Medium Risk — Conditional Commercial License](#3-medium-risk--conditional-commercial-license)
4. [Low Risk — Permissive Licenses](#4-low-risk--permissive-licenses)
5. [Action Items](#5-action-items)

---

## 1. Summary

All direct and transitive NuGet dependencies across the solution were audited
for license obligations. Packages fall into three risk tiers:

| Risk | Count | Licenses |
|------|-------|----------|
| 🔴 High | 1 | MPL-2.0 |
| 🟠 Medium | 2 (+1 transitive) | Six Labors Split License v1.0 |
| 🟢 Low | ~25+ | MIT, Apache-2.0, PostgreSQL |

---

## 2. High Risk — Copyleft / Source Disclosure

### `libplctag` 1.5.2 — MPL-2.0

| | |
|---|---|
| **Project** | Drivers.EtherNetIP |
| **License** | [Mozilla Public License 2.0](https://licenses.nuget.org/MPL-2.0) |
| **SPDX** | `MPL-2.0` |
| **Risk** | 🔴 **File-level copyleft** |

**What this means:**

- Using the library **unmodified** as a NuGet binary dependency is permitted
  with **no source-disclosure obligation**.
- If you **modify any source file** originating from this library (fork,
  patch, or vendor the code), you **must publish** those modified files under
  MPL-2.0.
- MPL-2.0 is file-scoped: only the modified files must be disclosed, not your
  entire application. However, this is still the **strongest copyleft
  obligation** in the solution.

**Recommendation:** Do not vendor or modify the libplctag source. Continue
consuming it exclusively as a NuGet package reference.

---

## 3. Medium Risk — Conditional Commercial License

### `SixLabors.ImageSharp` 3.1.12 & `SixLabors.ImageSharp.Drawing` 2.1.7

| | |
|---|---|
| **Project** | Server |
| **License** | [Six Labors Split License v1.0](https://github.com/SixLabors/ImageSharp/blob/main/LICENSE) |
| **Risk** | 🟠 **Commercial license may be required** |

The Split License grants free use under Apache-2.0 **only if one** of these
conditions is met:

| Condition | Free? |
|-----------|-------|
| Your software is licensed under an Open Source or Source Available license | ✅ Yes |
| The package is a **transitive** dependency (not directly referenced) | ✅ Yes |
| Your company earns **< 1 million USD** annual gross revenue | ✅ Yes |
| You are a registered non-profit or charity | ✅ Yes |
| **None of the above** | ❌ **Commercial license required** |

If your organisation is a for-profit entity with **≥ $1M USD annual gross
revenue**, you must purchase a commercial license from
[sixlabors.com/pricing](https://sixlabors.com/pricing).

### `SixLabors.Fonts` 2.1.3 (transitive)

Pulled in as a transitive dependency of `SixLabors.ImageSharp.Drawing`.
Under the Split License, transitive dependencies are automatically covered
by Apache-2.0 — **no commercial license needed**.

---

## 4. Low Risk — Permissive Licenses

All remaining packages use permissive licenses (MIT, Apache-2.0, or
PostgreSQL) that allow unrestricted commercial use with minimal obligations
(typically: include the license/notice text in distributions).

### MIT License

| Package | Version | Project(s) |
|---------|---------|------------|
| Azure.Identity | 1.18.0 | Server |
| Microsoft.CodeAnalysis.CSharp.Scripting | 4.12.0 | Server |
| Microsoft.CodeAnalysis.VisualBasic | 4.12.0 | Server |
| Microsoft.Data.SqlClient | 6.1.4 | Server |
| Microsoft.Data.Sqlite | 9.0.6 | Server |
| Microsoft.Extensions.Configuration.Json | 9.0.0 | Server |
| Microsoft.Extensions.Hosting | 9.0.0 | Server |
| Microsoft.Extensions.Hosting.WindowsServices | 9.0.0 | Server |
| Microsoft.Identity.Client | 4.80.0 | Server |
| Microsoft.IdentityModel.JsonWebTokens | 8.0.0 | Server |
| Microsoft.ML.OnnxRuntime | 1.22.0 | Server |
| Microsoft.AspNetCore.Components.Web | 10.0.0-preview.4 | ServerEditorWeb, RuntimeViewer |
| Microsoft.AspNetCore.SignalR.Client | 10.0.0-preview.4 | CloudBridge |
| MQTTnet | 5.1.0.1559 | Drivers.Mqtt |
| NModbus | 3.0.81 | Drivers.Modbus |
| OPCFoundation.NetStandard.Opc.Ua | 1.5.378.106 | Server |
| S7netplus | 0.20.0 | Drivers.S7 |
| System.Management | 9.0.0 | Server |

### Apache-2.0 License

| Package | Version | Project(s) |
|---------|---------|------------|
| Serilog | 4.3.1 | Server |
| Serilog.Extensions.Hosting | 8.0.0 | Server |
| Serilog.Extensions.Logging | 8.0.0 | Server |
| Serilog.Settings.Configuration | 8.0.2 | Server |
| Serilog.Sinks.Console | 6.0.0 | Server |
| Serilog.Sinks.File | 6.0.0 | Server |
| Photino.NET | 4.0.16 | RuntimeViewer.Desktop |
| xunit.v3 | 1.1.0 | Tests (not shipped) |

### PostgreSQL License (BSD-like)

| Package | Version | Project(s) |
|---------|---------|------------|
| Npgsql | 8.0.3 / 9.0.3 | Server, Drivers.Sql |

---

## 5. Action Items

| # | Priority | Action |
|---|----------|--------|
| 1 | 🔴 **High** | **libplctag (MPL-2.0):** Never vendor or modify its source. Keep it as a NuGet-only dependency to avoid source-disclosure obligations. |
| 2 | 🟠 **Medium** | **SixLabors.ImageSharp (Split License):** Determine whether your organisation qualifies for free Apache-2.0 use (< $1M revenue, open source, or non-profit). If not, purchase a [commercial license](https://sixlabors.com/pricing). |
| 3 | 🟢 **Low** | **MIT / Apache-2.0 packages:** Include license texts and NOTICE files in your distribution (typically in a `THIRD-PARTY-NOTICES.txt` or `licenses/` folder shipped alongside the binaries). |
| 4 | 🟢 **Maintenance** | Re-run `dotnet list package --vulnerable` periodically (or integrate into CI) to catch new advisories early. |
