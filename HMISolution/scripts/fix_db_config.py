import json

path = 'C:/Work/samples/CameraShowcase/nodes.json'
with open(path, 'r', encoding='utf-8') as f:
    data = json.load(f)

old_db = data.get('Database', {})
print(f'Old Database config: {json.dumps(old_db, indent=2)}')

data['Database'] = {
    "Provider": "Sqlite",
    "ConnectionString": "hda.db",
    "TableName": "variable_data"
}

print(f'\nNew Database config: {json.dumps(data["Database"], indent=2)}')

with open(path, 'w', encoding='utf-8', newline='\n') as f:
    json.dump(data, f, indent=2, ensure_ascii=False)

print('\nnodes.json updated successfully.')
