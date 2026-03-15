using System.Text.Json;
using SharedModels;

// ─── License Generator CLI ──────────────────────────────────────
// Usage:
//   dotnet run -- keygen                          Generate RSA key pair
//   dotnet run -- machineId                       Show current machine ID
//   dotnet run -- create <tier> <licensee> [opts] Create & sign a license
//
// Examples:
//   dotnet run -- keygen
//   dotnet run -- machineId
//   dotnet run -- create Trial "John Doe" --machine auto --days 30
//   dotnet run -- create Professional "Acme Corp" --machine abc123 --days 365 --project "Plant-A"
//   dotnet run -- create Enterprise "Acme Corp" --output license.json

if (args.Length == 0)
{
    PrintUsage();
    return;
}

var command = args[0].ToLowerInvariant();

switch (command)
{
    case "keygen":
        GenerateKeys();
        break;
    case "machineid":
        Console.WriteLine($"Machine ID: {LicenseManager.GetMachineId()}");
        break;
    case "create":
        CreateLicense(args[1..]);
        break;
    default:
        PrintUsage();
        break;
}

void PrintUsage()
{
    Console.WriteLine("""
        License Generator Tool
        ══════════════════════════════════════════

        Commands:
          keygen                             Generate new RSA key pair (private.pem + public.txt)
          machineId                          Show this machine's hardware fingerprint
          create <tier> <licensee> [options]  Create and sign a license file

        Tiers: Trial, Starter, Professional, Enterprise

        Options for 'create':
          --machine <id|auto>   Lock to machine ID ("auto" = this machine, empty = no lock)
          --days <n>            Validity period in days (default: 30 for Trial, 365 for others)
          --project <name>      Project/site name
          --output <path>       Output file path (default: license.json)
          --key <path>          Private key file (default: private.pem)

        Examples:
          dotnet run -- keygen
          dotnet run -- create Trial "John Doe" --machine auto --days 14
          dotnet run -- create Professional "Acme Corp" --machine abc123 --days 365
          dotnet run -- create Enterprise "BigCorp" --project "Factory-1" --output factory1.license.json
        """);
}

void GenerateKeys()
{
    var (privateKey, publicKey) = LicenseManager.GenerateKeyPair();
    File.WriteAllText("private.pem", privateKey);
    File.WriteAllText("public.txt", publicKey);
    Console.WriteLine("Generated key pair:");
    Console.WriteLine($"  Private key: private.pem (KEEP SECRET — never ship this)");
    Console.WriteLine($"  Public key:  public.txt  (embed this in LicenseManager.EmbeddedPublicKey)");
    Console.WriteLine();
    Console.WriteLine($"Public key (Base64):");
    Console.WriteLine(publicKey);
}

void CreateLicense(string[] createArgs)
{
    if (createArgs.Length < 2)
    {
        Console.WriteLine("Error: 'create' requires <tier> and <licensee>.");
        return;
    }

    var tier = createArgs[0];
    var licensee = createArgs[1];
    var machineId = "";
    var days = tier.Equals("Trial", StringComparison.OrdinalIgnoreCase) ? 30 : 365;
    var project = "";
    var output = "license.json";
    var keyPath = "private.pem";

    // Parse optional arguments
    for (int i = 2; i < createArgs.Length; i++)
    {
        switch (createArgs[i].ToLowerInvariant())
        {
            case "--machine" when i + 1 < createArgs.Length:
                var val = createArgs[++i];
                machineId = val.Equals("auto", StringComparison.OrdinalIgnoreCase)
                    ? LicenseManager.GetMachineId()
                    : val;
                break;
            case "--days" when i + 1 < createArgs.Length:
                days = int.Parse(createArgs[++i]);
                break;
            case "--project" when i + 1 < createArgs.Length:
                project = createArgs[++i];
                break;
            case "--output" when i + 1 < createArgs.Length:
                output = createArgs[++i];
                break;
            case "--key" when i + 1 < createArgs.Length:
                keyPath = createArgs[++i];
                break;
        }
    }

    // Create license based on tier
    License license = tier.ToLowerInvariant() switch
    {
        "trial" => LicenseTiers.CreateTrial(licensee, machineId),
        "starter" => LicenseTiers.CreateStarter(licensee, machineId, days),
        "professional" => LicenseTiers.CreateProfessional(licensee, machineId, days),
        "enterprise" => LicenseTiers.CreateEnterprise(licensee, machineId),
        _ => throw new ArgumentException($"Unknown tier: {tier}")
    };

    if (!string.IsNullOrEmpty(project))
        license.ProjectName = project;
    if (tier.Equals("trial", StringComparison.OrdinalIgnoreCase))
        license.ExpiresUtc = DateTime.UtcNow.AddDays(days);

    // Sign the license
    if (!File.Exists(keyPath))
    {
        Console.WriteLine($"Warning: Private key '{keyPath}' not found. License will be unsigned.");
        Console.WriteLine("Run 'keygen' first to generate a key pair.");
    }
    else
    {
        var privateKey = File.ReadAllText(keyPath);
        LicenseManager.SignLicense(license, privateKey);
    }

    // Export
    LicenseManager.ExportLicense(license, output);

    Console.WriteLine($"License created: {output}");
    Console.WriteLine($"  Tier:       {license.Tier}");
    Console.WriteLine($"  Licensed to: {license.LicensedTo}");
    Console.WriteLine($"  Machine ID: {(string.IsNullOrEmpty(license.MachineId) ? "(any machine)" : license.MachineId)}");
    Console.WriteLine($"  Expires:    {(license.ExpiresUtc == DateTime.MaxValue ? "Never (perpetual)" : license.ExpiresUtc.ToString("yyyy-MM-dd"))}");
    Console.WriteLine($"  Variables:  {FormatLimit(license.MaxVariables)}");
    Console.WriteLine($"  Drivers:    {FormatLimit(license.MaxDrivers)}");
    Console.WriteLine($"  Scripts:    {FormatLimit(license.MaxScripts)}");
    Console.WriteLine($"  PLC Progs:  {FormatLimit(license.MaxPlcPrograms)}");
    Console.WriteLine($"  Screens:    {FormatLimit(license.MaxScreens)}");
    Console.WriteLine($"  Recipes:    {FormatLimit(license.MaxRecipes)}");
    Console.WriteLine($"  Logging:    {(license.AllowDataLogging ? "Yes" : "No")}");
    Console.WriteLine($"  AI:         {(license.AllowAi ? "Yes" : "No")}");
    if (!string.IsNullOrEmpty(license.Signature) && license.Signature != "(trial)")
        Console.WriteLine($"  Signed:     Yes");
}

string FormatLimit(int max) => max == 0 ? "Unlimited" : max.ToString();
