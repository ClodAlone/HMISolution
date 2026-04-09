import json, os

d = 'C:/Work/samples/CameraShowcase/screens'
for f in ['CameraOverview.json', 'SingleCamera.json', 'YoloSettings.json']:
    data = json.load(open(os.path.join(d, f), encoding='utf-8'))
    cams = [s for s in data.get('Symbols', []) if 'Camera' in s and s.get('Camera') and s['Camera'].get('Url')]
    print(f + ': ' + str(len(cams)) + ' cameras')
    for s in cams:
        c = s['Camera']
        print('  ' + c['CameraId'] + ': ' + c['Url'] + ' (proto=' + c['Protocol'] + ')')
