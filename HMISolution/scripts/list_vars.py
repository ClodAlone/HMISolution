import json

f = open(r"C:\Work\samples\CameraShowcase\nodes.json", "r", encoding="utf-8")
d = json.load(f)
f.close()

def walk(folder, prefix=""):
    path = prefix + folder["Name"] if prefix else folder["Name"]
    for v in folder.get("Variables", []):
        name = v["Name"]
        print(f"  {path}.{name}")
    for sub in folder.get("Folders", []):
        walk(sub, path + ".")

walk(d["Folder"])
