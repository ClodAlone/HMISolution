import json, os

d = 'C:/Work/samples/CameraShowcase/screens'
for f in sorted(os.listdir(d)):
    if not f.endswith('.json'):
        continue
    data = json.load(open(os.path.join(d, f), encoding='utf-8'))
    for s in data.get('Symbols', []):
        cam = s.get('Camera')
        if cam and cam.get('Url'):
            print(f'{f} -> {s.get("Id","?")} : {cam["Protocol"]} {cam["Url"]}')
        # Also check for old Properties-based camera config
        props = s.get('Properties', {})
        if props.get('Url'):
            print(f'{f} -> {s.get("Id","?")} [Properties]: {props.get("Protocol","")} {props["Url"]}')
