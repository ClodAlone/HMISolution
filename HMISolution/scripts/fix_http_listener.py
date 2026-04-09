import os

path = r'C:\Users\cfior\source\repos\Server\CameraHostedService.cs'
with open(path, 'r', encoding='utf-8-sig') as f:
    content = f.read()

old = '''        // Start HTTP listener for MJPEG streams
        try
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add($"http://+:{StreamPort}/");
            _httpListener.Start();
            _logger.LogInformation("Camera MJPEG stream server listening on port {Port}", StreamPort);'''

new = '''        // Start HTTP listener for MJPEG streams
        try
        {
            _httpListener = new HttpListener();
            try
            {
                // Try wildcard binding first (allows remote access, requires admin or URL ACL)
                _httpListener.Prefixes.Add($"http://+:{StreamPort}/");
                _httpListener.Start();
            }
            catch (HttpListenerException)
            {
                // Fall back to localhost binding (no admin required)
                _httpListener.Close();
                _httpListener = new HttpListener();
                _httpListener.Prefixes.Add($"http://localhost:{StreamPort}/");
                _httpListener.Start();
                _logger.LogInformation("Camera stream bound to localhost only (run as admin or register URL ACL for remote access)");
            }
            _logger.LogInformation("Camera MJPEG stream server listening on port {Port}", StreamPort);'''

# Detect line endings
if '\r\n' in content:
    old = old.replace('\n', '\r\n')
    new = new.replace('\n', '\r\n')
    print('Using CRLF')
else:
    print('Using LF')

count = content.count(old)
if count == 1:
    content = content.replace(old, new)
    with open(path, 'w', encoding='utf-8-sig') as f:
        f.write(content)
    print('File patched successfully.')
elif count == 0:
    print('ERROR: Old text not found')
else:
    print(f'ERROR: Found {count} matches')
