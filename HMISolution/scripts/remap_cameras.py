import json, os

# Camera ID -> new TVCC mapping
remap = {
    'TVCC1': 'TVCC21',
    'TVCC3': 'TVCC22',
    # TVCC2 stays the same
}

def remap_url(url):
    for old, new in remap.items():
        url = url.replace(f'@{old}/', f'@{new}/')
    return url

# Fix nodes.json
nodes_path = 'C:/Work/samples/CameraShowcase/nodes.json'
with open(nodes_path, 'r', encoding='utf-8') as f:
    data = json.load(f)

for cam in data.get('Cameras', []):
    old_url = cam.get('Url', '')
    new_url = remap_url(old_url)
    if old_url != new_url:
        cam['Url'] = new_url
        print(f'nodes.json: {cam["CameraId"]}: {old_url} -> {new_url}')

with open(nodes_path, 'w', encoding='utf-8', newline='\n') as f:
    json.dump(data, f, indent=2, ensure_ascii=False)

# Fix screen files
screens_dir = 'C:/Work/samples/CameraShowcase/screens'
for fname in sorted(os.listdir(screens_dir)):
    if not fname.endswith('.json'):
        continue
    fpath = os.path.join(screens_dir, fname)
    with open(fpath, 'r', encoding='utf-8') as f:
        sdata = json.load(f)

    changed = False
    for sym in sdata.get('Symbols', []):
        cam = sym.get('Camera')
        if cam and cam.get('Url'):
            old_url = cam['Url']
            new_url = remap_url(old_url)
            if old_url != new_url:
                cam['Url'] = new_url
                print(f'{fname}: {sym.get("Id","?")} -> {new_url}')
                changed = True

    if changed:
        with open(fpath, 'w', encoding='utf-8', newline='\n') as f:
            json.dump(sdata, f, indent=2, ensure_ascii=False)

print('\nCamera URL remapping complete.')
