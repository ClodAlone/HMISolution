// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using SharedModels;

namespace LicenseGenerator.Wpf;

public partial class MainWindow : Window
{
    private string? _privateKeyPem;

    public MainWindow()
    {
        InitializeComponent();
        ApplyTierDefaults();
    }

    // ─── Private Key ────────────────────────────────────────────

    private void BrowsePrivateKey_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Title = "Select RSA Private Key",
            Filter = "PEM files (*.pem)|*.pem|Key files (*.key)|*.key|All files (*.*)|*.*",
            InitialDirectory = AppContext.BaseDirectory
        };

        if (dlg.ShowDialog() == true)
        {
            try
            {
                _privateKeyPem = File.ReadAllText(dlg.FileName);
                TxtPrivateKeyPath.Text = dlg.FileName;
                TxtKeyStatus.Text = "✅ Key loaded";
                TxtKeyStatus.Foreground = (Brush)FindResource("GreenBrush");
            }
            catch (Exception ex)
            {
                ShowError($"Failed to read key: {ex.Message}");
            }
        }
    }

    private void GenerateKeyPair_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new SaveFileDialog
        {
            Title = "Save Private Key (KEEP SECRET)",
            Filter = "PEM files (*.pem)|*.pem",
            FileName = "private.pem",
            InitialDirectory = AppContext.BaseDirectory
        };

        if (dlg.ShowDialog() != true)
            return;

        try
        {
            var (privateKeyPem, publicKeyBase64) = LicenseManager.GenerateKeyPair();

            // Save private key
            File.WriteAllText(dlg.FileName, privateKeyPem);
            _privateKeyPem = privateKeyPem;
            TxtPrivateKeyPath.Text = dlg.FileName;

            // Save public key next to it
            var pubPath = Path.Combine(Path.GetDirectoryName(dlg.FileName)!, "public.txt");
            File.WriteAllText(pubPath, publicKeyBase64);

            TxtKeyStatus.Text = "✅ New key pair generated";
            TxtKeyStatus.Foreground = (Brush)FindResource("GreenBrush");

            MessageBox.Show(
                $"RSA-2048 key pair generated successfully.\n\n" +
                $"Private key: {dlg.FileName}\n" +
                $"  ⚠ KEEP THIS SECRET — never share or commit to Git!\n\n" +
                $"Public key: {pubPath}\n" +
                $"  Embed this in LicenseManager.cs → EmbeddedPublicKey\n\n" +
                $"Public key (Base64):\n{publicKeyBase64[..60]}…",
                "Key Pair Generated",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            ShowError($"Key generation failed: {ex.Message}");
        }
    }

    // ─── Tier Selection ─────────────────────────────────────────

    private void CmbTier_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ApplyTierDefaults();
    }

    private void ApplyTierDefaults()
    {
        if (CmbTier?.SelectedItem is not ComboBoxItem item)
            return;

        var tier = item.Content?.ToString() ?? "Professional";

        // Create a temporary license with tier defaults to populate the UI
        var dummy = tier switch
        {
            "Trial" => LicenseTiers.CreateTrial("", ""),
            "Starter" => LicenseTiers.CreateStarter("", ""),
            "Professional" => LicenseTiers.CreateProfessional("", ""),
            "Enterprise" => LicenseTiers.CreateEnterprise("", ""),
            _ => LicenseTiers.CreateProfessional("", "")
        };

        if (TxtMaxVariables != null)
        {
            TxtMaxVariables.Text = dummy.MaxVariables.ToString();
            TxtMaxDrivers.Text = dummy.MaxDrivers.ToString();
            TxtMaxScripts.Text = dummy.MaxScripts.ToString();
            TxtMaxPlcPrograms.Text = dummy.MaxPlcPrograms.ToString();
            TxtMaxScreens.Text = dummy.MaxScreens.ToString();
            TxtMaxRecipes.Text = dummy.MaxRecipes.ToString();
            ChkDataLogging.IsChecked = dummy.AllowDataLogging;
            ChkAi.IsChecked = dummy.AllowAi;

            // Set default days
            TxtDays.Text = tier switch
            {
                "Trial" => "30",
                "Enterprise" => "0",
                _ => "365"
            };

            // Tier descriptions
            TxtTierDescription.Text = tier switch
            {
                "Trial" => "Limited evaluation license. 20 variables, 1 driver, 1 screen. No data logging or AI.",
                "Starter" => "Small projects. 100 variables, 2 drivers, 3 screens, 3 PLC programs. Data logging included.",
                "Professional" => "Full-featured. 1000 variables, unlimited drivers/scripts/screens. AI assistant included.",
                "Enterprise" => "Unlimited everything. Perpetual license. All features enabled.",
                _ => ""
            };
        }
    }

    // ─── Customer Info ──────────────────────────────────────────

    private void FillMachineId_Click(object sender, RoutedEventArgs e)
    {
        TxtMachineId.Text = LicenseManager.GetMachineId();
        SetStatus("Machine ID filled from this PC's hardware fingerprint");
    }

    // ─── Build License ──────────────────────────────────────────

    private License BuildLicense()
    {
        var tier = ((ComboBoxItem)CmbTier.SelectedItem).Content?.ToString() ?? "Professional";
        var licensedTo = TxtLicensedTo.Text.Trim();
        var projectName = TxtProjectName.Text.Trim();
        var machineId = TxtMachineId.Text.Trim();
        var daysText = TxtDays.Text.Trim();

        if (string.IsNullOrEmpty(licensedTo))
            throw new InvalidOperationException("Licensed To is required.");

        int days = 365;
        if (!string.IsNullOrEmpty(daysText) && daysText != "0")
            days = int.Parse(daysText);

        var license = tier switch
        {
            "Trial" => LicenseTiers.CreateTrial(licensedTo, machineId),
            "Starter" => LicenseTiers.CreateStarter(licensedTo, machineId, days),
            "Professional" => LicenseTiers.CreateProfessional(licensedTo, machineId, days),
            "Enterprise" => LicenseTiers.CreateEnterprise(licensedTo, machineId),
            _ => throw new InvalidOperationException($"Unknown tier: {tier}")
        };

        // Override with custom values from the UI
        license.ProjectName = projectName;

        if (tier == "Trial" && days != 30)
            license.ExpiresUtc = DateTime.UtcNow.AddDays(days);

        license.MaxVariables = ParseInt(TxtMaxVariables.Text);
        license.MaxDrivers = ParseInt(TxtMaxDrivers.Text);
        license.MaxScripts = ParseInt(TxtMaxScripts.Text);
        license.MaxPlcPrograms = ParseInt(TxtMaxPlcPrograms.Text);
        license.MaxScreens = ParseInt(TxtMaxScreens.Text);
        license.MaxRecipes = ParseInt(TxtMaxRecipes.Text);
        license.AllowDataLogging = ChkDataLogging.IsChecked == true;
        license.AllowAi = ChkAi.IsChecked == true;

        return license;
    }

    // ─── Preview ────────────────────────────────────────────────

    private void Preview_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var license = BuildLicense();
            var json = JsonSerializer.Serialize(license, new JsonSerializerOptions { WriteIndented = true });
            TxtPreview.Text = json;
            SetStatus("Preview updated (unsigned)");
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }

    // ─── Sign & Export ──────────────────────────────────────────

    private void SignAndExport_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var license = BuildLicense();

            // Sign
            if (!string.IsNullOrEmpty(_privateKeyPem))
            {
                LicenseManager.SignLicense(license, _privateKeyPem);
            }
            else
            {
                var result = MessageBox.Show(
                    "No private key is loaded. The license will be exported WITHOUT a signature.\n\n" +
                    "An unsigned license will only work if the embedded public key hasn't been configured yet " +
                    "(development mode).\n\nContinue without signing?",
                    "No Private Key",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                    return;
            }

            // Show save dialog
            var dlg = new SaveFileDialog
            {
                Title = "Export License File",
                Filter = "JSON files (*.json)|*.json",
                FileName = "license.json"
            };

            if (dlg.ShowDialog() != true)
                return;

            LicenseManager.ExportLicense(license, dlg.FileName);

            // Update preview with signed version
            var json = JsonSerializer.Serialize(license, new JsonSerializerOptions { WriteIndented = true });
            TxtPreview.Text = json;

            var signed = !string.IsNullOrEmpty(license.Signature) && license.Signature != "(trial)";
            SetStatus($"✅ License exported to {Path.GetFileName(dlg.FileName)}" +
                      (signed ? " (signed)" : " (UNSIGNED)"));

            MessageBox.Show(
                $"License exported successfully!\n\n" +
                $"File: {dlg.FileName}\n" +
                $"Tier: {license.Tier}\n" +
                $"Licensed to: {license.LicensedTo}\n" +
                $"Expires: {FormatExpiry(license)}\n" +
                $"Signed: {(signed ? "Yes ✅" : "No ⚠")}\n\n" +
                $"Instruct the customer to place this file next to their nodes.json.",
                "License Exported",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }

    // ─── Validate Existing ──────────────────────────────────────

    private void ValidateExisting_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Title = "Select License File to Validate",
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*"
        };

        if (dlg.ShowDialog() != true)
            return;

        try
        {
            var status = LicenseManager.Validate(dlg.FileName);

            var details = $"File: {dlg.FileName}\n\n" +
                          $"Valid: {(status.IsValid ? "✅ Yes" : "❌ No")}\n" +
                          $"Tier: {status.Tier}\n" +
                          $"Licensed to: {status.LicensedTo}\n" +
                          $"Message: {status.Message}\n";

            if (status.License != null)
            {
                details += $"\nExpires: {FormatExpiry(status.License)}\n" +
                           $"Machine ID: {(string.IsNullOrEmpty(status.License.MachineId) ? "(any)" : status.License.MachineId)}\n" +
                           $"\nLimits:\n" +
                           $"  Variables: {FormatLimit(status.License.MaxVariables)}\n" +
                           $"  Drivers: {FormatLimit(status.License.MaxDrivers)}\n" +
                           $"  Scripts: {FormatLimit(status.License.MaxScripts)}\n" +
                           $"  PLC Programs: {FormatLimit(status.License.MaxPlcPrograms)}\n" +
                           $"  Screens: {FormatLimit(status.License.MaxScreens)}\n" +
                           $"  Recipes: {FormatLimit(status.License.MaxRecipes)}\n" +
                           $"  Data Logging: {(status.License.AllowDataLogging ? "✅" : "❌")}\n" +
                           $"  AI Assistant: {(status.License.AllowAi ? "✅" : "❌")}\n";
            }

            details += $"\nThis machine's ID: {LicenseManager.GetMachineId()}";

            // Also show in preview
            var json = File.ReadAllText(dlg.FileName);
            TxtPreview.Text = json;

            MessageBox.Show(details,
                status.IsValid ? "✅ License Valid" : "❌ License Invalid",
                MessageBoxButton.OK,
                status.IsValid ? MessageBoxImage.Information : MessageBoxImage.Warning);

            SetStatus(status.IsValid
                ? $"✅ {Path.GetFileName(dlg.FileName)} is valid ({status.Tier})"
                : $"❌ {Path.GetFileName(dlg.FileName)} — {status.Message}");
        }
        catch (Exception ex)
        {
            ShowError($"Validation failed: {ex.Message}");
        }
    }

    // ─── Helpers ────────────────────────────────────────────────

    private static int ParseInt(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return 0;
        return int.TryParse(text.Trim(), out var val) ? val : 0;
    }

    private static string FormatLimit(int max) => max == 0 ? "Unlimited" : max.ToString();

    private static string FormatExpiry(License lic) =>
        lic.ExpiresUtc == DateTime.MaxValue
            ? "Never (perpetual)"
            : lic.ExpiresUtc.ToString("yyyy-MM-dd");

    private void SetStatus(string text)
    {
        TxtStatus.Text = text;
        TxtStatus.Foreground = (Brush)FindResource("MutedBrush");
    }

    private void ShowError(string message)
    {
        TxtStatus.Text = $"❌ {message}";
        TxtStatus.Foreground = (Brush)FindResource("RedBrush");
        MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
