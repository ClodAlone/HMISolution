import os

path = r'C:\Users\cfior\source\repos\Server\CameraStreamService.cs'
with open(path, 'r', encoding='utf-8-sig') as f:
    content = f.read()

# Upgrade the credential setup to use CredentialCache with both Basic and Digest
old = '''            if (!string.IsNullOrEmpty(authUser))
            {
                handler.Credentials = new System.Net.NetworkCredential(authUser, authPass);
            }'''

new = '''            if (!string.IsNullOrEmpty(authUser))
            {
                // Use CredentialCache to support both Digest and Basic auth (Hikvision uses Digest)
                try
                {
                    var credUri = new Uri(config.Url);
                    var baseUri = new Uri($"{credUri.Scheme}://{credUri.Host}{(credUri.IsDefaultPort ? "" : $":{credUri.Port}")}");
                    var credCache = new System.Net.CredentialCache();
                    var cred = new System.Net.NetworkCredential(authUser, authPass);
                    credCache.Add(baseUri, "Digest", cred);
                    credCache.Add(baseUri, "Basic", cred);
                    handler.Credentials = credCache;
                }
                catch
                {
                    handler.Credentials = new System.Net.NetworkCredential(authUser, authPass);
                }
                handler.PreAuthenticate = false;
            }'''

crlf = '\r\n' in content
if crlf:
    old = old.replace('\n', '\r\n')
    new = new.replace('\n', '\r\n')

count = content.count(old)
if count == 1:
    content = content.replace(old, new)
    with open(path, 'w', encoding='utf-8-sig') as f:
        f.write(content)
    print('Credential cache patch applied.')
else:
    print(f'ERROR: Found {count} matches')
