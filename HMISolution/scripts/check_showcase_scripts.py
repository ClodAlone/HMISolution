import json

f = open(r"C:\Work\samples\WidgetShowcase\nodes.json", "r", encoding="utf-8")
d = json.load(f)
f.close()

scripts = d.get("Scripts", [])
print(f"Scripts count: {len(scripts)}")
for s in scripts:
    name = s.get("Name", "?")
    enabled = s.get("Enabled", False)
    interval = s.get("IntervalMs", 0)
    code_len = len(s.get("Code", ""))
    print(f"  - {name} (enabled={enabled}, interval={interval}ms, code={code_len} chars)")
