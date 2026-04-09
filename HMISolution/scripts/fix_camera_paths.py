"""Fix CameraShowcase: remove 'CameraDemo.' prefix from all variable paths.
The root folder name is transparent in the OPC address space, so variables are
addressed as 'Cameras.Entrance.YoloEnabled' not 'CameraDemo.Cameras.Entrance.YoloEnabled'."""

import json, os, re

BASE = r"C:\Work\samples\CameraShowcase"
PREFIX = "CameraDemo."

def fix_json_file(path):
    """Replace all occurrences of 'CameraDemo.' prefix in a JSON file."""
    with open(path, "r", encoding="utf-8") as f:
        content = f.read()

    count = content.count(PREFIX)
    if count == 0:
        print(f"  {os.path.basename(path)}: no changes needed")
        return

    content = content.replace(PREFIX, "")

    with open(path, "w", encoding="utf-8") as f:
        f.write(content)

    print(f"  {os.path.basename(path)}: removed {count} occurrences of '{PREFIX}'")

# Fix all JSON files
files = [
    os.path.join(BASE, "screens", "CameraOverview.json"),
    os.path.join(BASE, "screens", "SingleCamera.json"),
    os.path.join(BASE, "screens", "YoloSettings.json"),
    os.path.join(BASE, "scripts", "SimulateDetections.json"),
]

print("Fixing variable path prefixes...")
for f in files:
    fix_json_file(f)

# Verify the script code after fix
with open(os.path.join(BASE, "scripts", "SimulateDetections.json"), "r", encoding="utf-8") as f:
    script = json.load(f)

code = script["Code"]
# Show the first few variable references
refs = re.findall(r'(?:Read\w+|Write)\("([^"]+)"', code)
unique_refs = sorted(set(refs))
print(f"\nScript variable references ({len(unique_refs)} unique):")
for r in unique_refs:
    print(f"  {r}")

# Also verify a screen file
with open(os.path.join(BASE, "screens", "CameraOverview.json"), "r", encoding="utf-8") as f:
    screen = json.load(f)

var_paths = set()
for sym in screen["Symbols"]:
    if sym.get("VariablePath"):
        var_paths.add(sym["VariablePath"])
    cam = sym.get("Camera")
    if cam:
        if cam.get("YoloEnableVariable"):
            var_paths.add(cam["YoloEnableVariable"])
        if cam.get("DetectionVariablePrefix"):
            var_paths.add(cam["DetectionVariablePrefix"] + ".*")

print(f"\nCameraOverview variable references ({len(var_paths)} unique):")
for p in sorted(var_paths):
    print(f"  {p}")

print("\nDone!")
