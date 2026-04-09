"""Update CameraShowcase to use real Hikvision camera URLs.
Pattern: http://admin:Ciccio$Bello91@TVCC{N}/Streaming/channels/1/picture
Cameras: TVCC1 (Entrance), TVCC2 (Warehouse), TVCC3 (Parking Lot)
Protocol: HTTP snapshot polling
"""
import json, os

BASE = r"C:\Work\samples\CameraShowcase"

# Camera URL pattern
def cam_url(n):
    return f"http://admin:Ciccio$Bello91@TVCC{n}/Streaming/channels/1/picture"

# ── Fix all screen files ──
replacements = {
    # Old URL -> New URL
    "http://192.168.1.100/mjpeg": cam_url(1),
    "rtsp://192.168.1.101:554/stream1": cam_url(2),
    "http://192.168.1.102/snapshot.jpg": cam_url(3),
    # Old protocols -> http (snapshot)
}

# Also need to fix protocols and auth fields
for screen_file in ["CameraOverview.json", "SingleCamera.json", "YoloSettings.json"]:
    path = os.path.join(BASE, "screens", screen_file)
    with open(path, "r", encoding="utf-8") as f:
        screen = json.load(f)

    changed = 0
    for sym in screen["Symbols"]:
        cam = sym.get("Camera")
        if cam is None:
            continue

        old_url = cam.get("Url", "")
        cam_id = cam.get("CameraId", "")

        if cam_id == "entrance":
            cam["Url"] = cam_url(1)
            cam["Protocol"] = "http"
            cam["Fps"] = 5
            cam["Username"] = ""   # auth is in the URL
            cam["Password"] = ""
            changed += 1
        elif cam_id == "warehouse":
            cam["Url"] = cam_url(2)
            cam["Protocol"] = "http"
            cam["Fps"] = 5
            cam["Username"] = ""
            cam["Password"] = ""
            changed += 1
        elif cam_id == "parking":
            cam["Url"] = cam_url(3)
            cam["Protocol"] = "http"
            cam["Fps"] = 2
            cam["Username"] = ""
            cam["Password"] = ""
            changed += 1

    with open(path, "w", encoding="utf-8") as f:
        json.dump(screen, f, indent=2, ensure_ascii=False)

    print(f"  {screen_file}: updated {changed} camera configs")

# ── Fix help text in YoloSettings screen ──
path = os.path.join(BASE, "screens", "YoloSettings.json")
with open(path, "r", encoding="utf-8") as f:
    screen = json.load(f)

for sym in screen["Symbols"]:
    lbl = sym.get("Label", "")
    # Update protocol labels
    if sym["Id"] == "lbl_e_proto":
        sym["Label"] = "Protocol: HTTP Snapshot"
    elif sym["Id"] == "lbl_w_proto":
        sym["Label"] = "Protocol: HTTP Snapshot"
    elif sym["Id"] == "lbl_p_proto":
        sym["Label"] = "Protocol: HTTP Snapshot"
    elif sym["Id"] == "lbl_e_fps":
        sym["Label"] = "FPS: 5"
    elif sym["Id"] == "lbl_w_fps":
        sym["Label"] = "FPS: 5"
    elif sym["Id"] == "lbl_p_fps":
        sym["Label"] = "FPS: 2"
    # Update help text
    elif sym["Id"] == "help1":
        sym["Label"] = "1. Cameras use Hikvision HTTP snapshot URLs: http://admin:***@TVCC{N}/Streaming/channels/1/picture"
    elif sym["Id"] == "help_r1":
        sym["Label"] = "\u2022 Entrance (TVCC1): HTTP snapshot, YOLO enabled, dynamic enable via variable."
    elif sym["Id"] == "help_r2":
        sym["Label"] = "\u2022 Warehouse (TVCC2): HTTP snapshot, YOLO enabled, confidence 0.40."
    elif sym["Id"] == "help_r3":
        sym["Label"] = "\u2022 Parking (TVCC3): HTTP snapshot at 2 FPS, YOLO off by default (toggle to enable)."

with open(path, "w", encoding="utf-8") as f:
    json.dump(screen, f, indent=2, ensure_ascii=False)
print("  YoloSettings.json: updated help text")

# ── Fix SingleCamera title ──
path = os.path.join(BASE, "screens", "SingleCamera.json")
with open(path, "r", encoding="utf-8") as f:
    screen = json.load(f)

for sym in screen["Symbols"]:
    if sym["Id"] == "title":
        sym["Label"] = "\U0001f50d Entrance Camera (TVCC1) \u2014 Full View"

with open(path, "w", encoding="utf-8") as f:
    json.dump(screen, f, indent=2, ensure_ascii=False)
print("  SingleCamera.json: updated title")

# ── Verify final URLs ──
print("\nFinal camera URLs:")
for screen_file in ["CameraOverview.json", "SingleCamera.json", "YoloSettings.json"]:
    path = os.path.join(BASE, "screens", screen_file)
    with open(path, "r", encoding="utf-8") as f:
        screen = json.load(f)
    for sym in screen["Symbols"]:
        cam = sym.get("Camera")
        if cam:
            cid = cam["CameraId"]
            url = cam["Url"]
            proto = cam["Protocol"]
            fps = cam["Fps"]
            user = cam.get("Username", "")
            print(f"  [{screen_file}] {cid}: {proto} @ {fps}fps -> {url} (auth in URL: {not bool(user)})")

print("\nDone!")
