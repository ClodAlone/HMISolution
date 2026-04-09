import os

path = r'C:\Users\cfior\source\repos\Server\CameraStreamService.cs'
with open(path, 'r', encoding='utf-8-sig') as f:
    content = f.read()

old = '''            var handler = new HttpClientHandler();
            if (!string.IsNullOrEmpty(config.Username))
            {
                handler.Credentials = new System.Net.NetworkCredential(config.Username, config.Password);
            }
            _httpClient = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(30) };'''

new = '''            var handler = new HttpClientHandler();
            string? authUser = config.Username;
            string? authPass = config.Password;

            // Extract embedded credentials from URL (e.g. http://user:pass@host/path)
            if (string.IsNullOrEmpty(authUser) && !string.IsNullOrEmpty(config.Url))
            {
                try
                {
                    var uri = new Uri(config.Url);
                    if (!string.IsNullOrEmpty(uri.UserInfo))
                    {
                        var parts = uri.UserInfo.Split(':', 2);
                        authUser = Uri.UnescapeDataString(parts[0]);
                        authPass = parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : "";
                    }
                }
                catch { }
            }

            if (!string.IsNullOrEmpty(authUser))
            {
                handler.Credentials = new System.Net.NetworkCredential(authUser, authPass);
            }
            _httpClient = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(30) };'''

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
