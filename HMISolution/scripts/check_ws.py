path = r"C:\Users\cfior\source\repos\Server\SimpleFileServerNodeManager.cs"

with open(path, 'rb') as f:
    data = f.read().decode('utf-8')

# Check exact whitespace around batch block
idx = data.find('Batch / Sequence Manager')
if idx >= 0:
    snippet = data[idx-20:idx+350]
    print("BATCH BLOCK:")
    print(repr(snippet))
else:
    print('batch block not found')

print('---')
# Check CreateAssetVar
idx2 = data.find('private void CreateAssetVar<T>')
if idx2 >= 0:
    snippet2 = data[idx2:idx2+600]
    print("CREATE ASSET VAR:")
    print(repr(snippet2))
else:
    print('CreateAssetVar not found')
