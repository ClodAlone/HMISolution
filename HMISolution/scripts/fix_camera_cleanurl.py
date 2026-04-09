import os

path = r'C:\Users\cfior\source\repos\Server\CameraStreamService.cs'
with open(path, 'r', encoding='utf-8-sig') as f:
    content = f.read()

# 1. Add _cleanUrl field next to _httpClient
old1 = '''        private volatile byte[]? _latestFrame;
        private readonly HttpClient _httpClient;'''
new1 = '''        private volatile byte[]? _latestFrame;
        private readonly HttpClient _httpClient;
        private readonly string _cleanUrl;'''

# 2. After _httpClient creation, compute _cleanUrl
old2 = '''            _httpClient = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(30) };

            // Load YOLO model if detection is enabled statically OR a dynamic variable is configured'''
new2 = '''            _httpClient = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(30) };

            // Build a clean URL without embedded credentials (HttpClient uses NetworkCredential instead)
            _cleanUrl = config.Url;
            try
            {
                var parsed = new Uri(config.Url);
                if (!string.IsNullOrEmpty(parsed.UserInfo))
                {
                    _cleanUrl = $"{parsed.Scheme}://{parsed.Host}{(parsed.IsDefaultPort ? "" : $":{parsed.Port}")}{parsed.PathAndQuery}";
                }
            }
            catch { }

            // Load YOLO model if detection is enabled statically OR a dynamic variable is configured'''

# 3. In CaptureHttpSnapshotAsync, use _cleanUrl
old3 = '''                    var bytes = await _httpClient.GetByteArrayAsync(_config.Url, ct);'''
new3 = '''                    var bytes = await _httpClient.GetByteArrayAsync(_cleanUrl, ct);'''

# 4. In CaptureMjpegAsync, use _cleanUrl
old4 = '''                    using var response = await _httpClient.GetAsync(_config.Url, HttpCompletionOption.ResponseHeadersRead, ct);'''
new4 = '''                    using var response = await _httpClient.GetAsync(_cleanUrl, HttpCompletionOption.ResponseHeadersRead, ct);'''

# Detect line endings
crlf = '\r\n' in content
if crlf:
    old1 = old1.replace('\n', '\r\n')
    new1 = new1.replace('\n', '\r\n')
    old2 = old2.replace('\n', '\r\n')
    new2 = new2.replace('\n', '\r\n')
    old3 = old3.replace('\n', '\r\n')
    new3 = new3.replace('\n', '\r\n')
    old4 = old4.replace('\n', '\r\n')
    new4 = new4.replace('\n', '\r\n')
    print('Using CRLF')
else:
    print('Using LF')

replacements = [
    ('_cleanUrl field', old1, new1),
    ('_cleanUrl init', old2, new2),
    ('CaptureHttpSnapshot use _cleanUrl', old3, new3),
    ('CaptureMjpeg use _cleanUrl', old4, new4),
]

for label, old, new in replacements:
    count = content.count(old)
    if count == 1:
        content = content.replace(old, new)
        print(f'OK: {label}')
    elif count == 0:
        print(f'SKIP (not found): {label}')
    else:
        print(f'ERROR ({count} matches): {label}')

with open(path, 'w', encoding='utf-8-sig') as f:
    f.write(content)
print('\nFile saved.')
