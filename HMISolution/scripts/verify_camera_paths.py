"""Verify variable paths match between nodes.json definition and screen/script references."""
import json, os

BASE = r"C:\Work\samples\CameraShowcase"

# Build variable paths from nodes.json (root folder is transparent)
with open(os.path.join(BASE, "nodes.json"), "r", encoding="utf-8") as f:
    nodes = json.load(f)

def walk(folder, prefix="", is_root=False):
    """Walk folder tree. Root folder is transparent (prefix stays empty)."""
    if is_root:
        child_prefix = ""
    else:
        child_prefix = prefix + folder["Name"] if not prefix else prefix + "." + folder["Name"]

    paths = set()
    for v in folder.get("Variables", []):
        name = v["Name"]
        vpath = child_prefix + "." + name if child_prefix else name
        paths.add(vpath)
    for sub in folder.get("Folders", []):
        paths |= walk(sub, child_prefix)
    return paths

all_vars = walk(nodes["Folder"], is_root=True)
print(f"OPC variables ({len(all_vars)}):")
for v in sorted(all_vars):
    print(f"  {v}")

# Check script references
with open(os.path.join(BASE, "scripts", "SimulateDetections.json"), "r", encoding="utf-8") as f:
    script = json.load(f)

import re
script_refs = set(re.findall(r'(?:Read\w+|Write)\("([^"]+)"', script["Code"]))
missing = script_refs - all_vars
if missing:
    print(f"\n❌ Script references {len(missing)} missing variables:")
    for m in sorted(missing):
        print(f"  {m}")
else:
    print(f"\n✅ All {len(script_refs)} script variable references are valid.")

# Check screen variable references
for screen_file in os.listdir(os.path.join(BASE, "screens")):
    with open(os.path.join(BASE, "screens", screen_file), "r", encoding="utf-8") as f:
        screen = json.load(f)

    screen_refs = set()
    for sym in screen["Symbols"]:
        vp = sym.get("VariablePath")
        if vp:
            screen_refs.add(vp)
        cam = sym.get("Camera")
        if cam and cam.get("YoloEnableVariable"):
            screen_refs.add(cam["YoloEnableVariable"])

    missing_screen = screen_refs - all_vars
    if missing_screen:
        print(f"\n❌ {screen_file} references {len(missing_screen)} missing variables:")
        for m in sorted(missing_screen):
            print(f"  {m}")
    else:
        print(f"✅ {screen_file}: all {len(screen_refs)} variable references valid.")
