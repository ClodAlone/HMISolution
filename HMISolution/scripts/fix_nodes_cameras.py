import json

path = 'C:/Work/samples/CameraShowcase/nodes.json'
with open(path, 'r', encoding='utf-8') as f:
    data = json.load(f)

url_map = {
    'entrance':  'http://admin:Ciccio$Bello91@TVCC1/Streaming/channels/1/picture',
    'warehouse': 'http://admin:Ciccio$Bello91@TVCC2/Streaming/channels/1/picture',
    'parking':   'http://admin:Ciccio$Bello91@TVCC3/Streaming/channels/1/picture',
}

for cam in data.get('Cameras', []):
    cid = cam.get('CameraId', '')
    if cid in url_map:
        old_url = cam.get('Url', '')
        old_proto = cam.get('Protocol', '')
        cam['Url'] = url_map[cid]
        cam['Protocol'] = 'http'
        cam['Username'] = ''
        cam['Password'] = ''
        print(f'  {cid}: {old_proto} {old_url}')
        print(f'      -> http {url_map[cid]}')

with open(path, 'w', encoding='utf-8', newline='\n') as f:
    json.dump(data, f, indent=2, ensure_ascii=False)

print('\nnodes.json updated successfully.')
