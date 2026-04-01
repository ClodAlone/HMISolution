using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Serilog;
using SharedModels;

namespace SimpleOpcFileServer;

/// <summary>
/// Sends Web Push notifications using VAPID authentication (RFC 8292)
/// and aes128gcm content encryption (RFC 8291 / RFC 8188).
/// Uses only built-in .NET cryptography — no external NuGet packages required.
/// </summary>
public sealed class WebPushSender : IDisposable
{
    private readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromSeconds(30) };
    private readonly WebPushSubscriptionStore _store;
    private readonly string _vapidSubject;
    private readonly string _vapidPublicKey;   // base64url
    private readonly byte[] _vapidPublicKeyBytes;
    private readonly ECDsa _vapidKey;

    // Cache JWT per audience (origin) — tokens are valid for up to 24 h
    private readonly ConcurrentDictionary<string, (string Token, DateTime Expiry)> _jwtCache = new();

    public WebPushSender(WebPushNotificationChannel config, string projectDirectory)
    {
        _vapidSubject = config.VapidSubject;
        _vapidPublicKey = config.VapidPublicKey;
        _vapidPublicKeyBytes = Base64UrlDecode(config.VapidPublicKey);

        var privateKeyBytes = Base64UrlDecode(config.VapidPrivateKey);
        _vapidKey = ECDsa.Create(new ECParameters
        {
            Curve = ECCurve.NamedCurves.nistP256,
            D = privateKeyBytes,
            Q = DecodeUncompressedPoint(_vapidPublicKeyBytes)
        });

        _store = new WebPushSubscriptionStore(config, projectDirectory);
    }

    // ──────────── Send push to all subscribers ────────────

    public async Task SendToAllAsync(string title, string body, string tag, string urgency = "high")
    {
        var subs = _store.Load();
        if (subs.Count == 0) return;

        var payload = JsonSerializer.Serialize(new { title, body, tag });
        var expiredEndpoints = new List<string>();

        foreach (var sub in subs)
        {
            try
            {
                var success = await SendPushAsync(sub, payload, urgency);
                if (!success)
                    expiredEndpoints.Add(sub.Endpoint);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to send Web Push to {Endpoint}", sub.Endpoint);
                expiredEndpoints.Add(sub.Endpoint);
            }
        }

        // Remove expired/invalid subscriptions
        if (expiredEndpoints.Count > 0)
        {
            var cleaned = subs.Where(s => !expiredEndpoints.Contains(s.Endpoint)).ToList();
            _store.Save(cleaned);
            Log.Information("Removed {Count} expired Web Push subscription(s)", expiredEndpoints.Count);
        }
    }

    // ──────────── Core push sending ────────────

    private async Task<bool> SendPushAsync(PushSubscriptionInfo sub, string payload, string urgency)
    {
        var endpoint = new Uri(sub.Endpoint);
        var audience = $"{endpoint.Scheme}://{endpoint.Host}";

        // Encrypt payload per RFC 8291 (aes128gcm)
        var encryptedPayload = EncryptPayload(sub, Encoding.UTF8.GetBytes(payload));

        var request = new HttpRequestMessage(HttpMethod.Post, sub.Endpoint)
        {
            Content = new ByteArrayContent(encryptedPayload)
        };
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        request.Content.Headers.TryAddWithoutValidation("Content-Encoding", "aes128gcm");
        request.Headers.TryAddWithoutValidation("TTL", "86400");
        request.Headers.TryAddWithoutValidation("Urgency", urgency);

        // VAPID Authorization header
        var jwt = GetOrCreateJwt(audience);
        request.Headers.TryAddWithoutValidation("Authorization", $"vapid t={jwt},k={_vapidPublicKey}");

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.Created)
            return true;

        if (response.StatusCode == System.Net.HttpStatusCode.Gone ||
            response.StatusCode == System.Net.HttpStatusCode.NotFound ||
            (int)response.StatusCode == 410)
        {
            Log.Information("Web Push subscription expired (HTTP {Status}): {Endpoint}",
                (int)response.StatusCode, sub.Endpoint);
            return false;
        }

        var respBody = await response.Content.ReadAsStringAsync();
        Log.Warning("Web Push failed (HTTP {Status}) for {Endpoint}: {Body}",
            (int)response.StatusCode, sub.Endpoint, respBody);
        return true; // Don't remove on transient errors
    }

    // ──────────── VAPID JWT (RFC 8292) ────────────

    private string GetOrCreateJwt(string audience)
    {
        if (_jwtCache.TryGetValue(audience, out var cached) && cached.Expiry > DateTime.UtcNow.AddMinutes(5))
            return cached.Token;

        var expiry = DateTime.UtcNow.AddHours(12);
        var exp = new DateTimeOffset(expiry).ToUnixTimeSeconds();
        var headerJson = Base64UrlEncode(Encoding.UTF8.GetBytes("{\"typ\":\"JWT\",\"alg\":\"ES256\"}"));
        var payloadJson = Base64UrlEncode(Encoding.UTF8.GetBytes(
            $"{{\"aud\":\"{audience}\",\"exp\":{exp},\"sub\":\"{_vapidSubject}\"}}"));

        var signingInput = $"{headerJson}.{payloadJson}";
        var signatureBytes = _vapidKey.SignData(
            Encoding.UTF8.GetBytes(signingInput), HashAlgorithmName.SHA256);

        // ECDsa.SignData returns DER-encoded, convert to raw r||s (64 bytes) for JWT
        var rawSig = ConvertDerToRaw(signatureBytes);
        var jwt = $"{signingInput}.{Base64UrlEncode(rawSig)}";

        _jwtCache[audience] = (jwt, expiry);
        return jwt;
    }

    // ──────────── RFC 8291 payload encryption (aes128gcm) ────────────

    private byte[] EncryptPayload(PushSubscriptionInfo sub, byte[] plaintext)
    {
        var userPublicKeyBytes = Base64UrlDecode(sub.P256dh);
        var userAuthBytes = Base64UrlDecode(sub.Auth);

        // Generate ephemeral ECDH key pair
        using var ecdh = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);

        // Extract the uncompressed point (65 bytes)
        var ephemeralPublicKeyUncompressed = ExportUncompressedPoint(ecdh);

        // Import the user's public key
        var userEcdhParams = new ECParameters
        {
            Curve = ECCurve.NamedCurves.nistP256,
            Q = DecodeUncompressedPoint(userPublicKeyBytes)
        };
        using var userEcdh = ECDiffieHellman.Create(userEcdhParams);

        // Derive shared secret (raw ECDH agreement)
        var sharedSecret = ecdh.DeriveRawSecretAgreement(userEcdh.PublicKey);

        // RFC 8291 key derivation
        // PRK_key = HKDF-Extract(auth_secret, shared_secret)
        var prkKey = HkdfExtract(userAuthBytes, sharedSecret);

        // key_info = "WebPush: info\0" || ua_public || as_public
        var keyInfo = ConcatBytes(
            Encoding.ASCII.GetBytes("WebPush: info\0"),
            userPublicKeyBytes,
            ephemeralPublicKeyUncompressed);

        // IKM = HKDF-Expand(PRK_key, key_info, 32)
        var ikm = HkdfExpand(prkKey, keyInfo, 32);

        // Generate random 16-byte salt
        var salt = RandomNumberGenerator.GetBytes(16);

        // PRK = HKDF-Extract(salt, IKM)
        var prk = HkdfExtract(salt, ikm);

        // Content Encryption Key: HKDF-Expand(PRK, "Content-Encoding: aes128gcm\0", 16)
        var cekInfo = Encoding.ASCII.GetBytes("Content-Encoding: aes128gcm\0");
        var cek = HkdfExpand(prk, cekInfo, 16);

        // Nonce: HKDF-Expand(PRK, "Content-Encoding: nonce\0", 12)
        var nonceInfo = Encoding.ASCII.GetBytes("Content-Encoding: nonce\0");
        var nonce = HkdfExpand(prk, nonceInfo, 12);

        // Pad plaintext: append delimiter \x02 (final record)
        var padded = new byte[plaintext.Length + 1];
        Buffer.BlockCopy(plaintext, 0, padded, 0, plaintext.Length);
        padded[plaintext.Length] = 0x02; // Final record delimiter

        // Encrypt with AES-128-GCM
        var ciphertext = new byte[padded.Length];
        var tag = new byte[16];
        using var aes = new AesGcm(cek, 16);
        aes.Encrypt(nonce, padded, ciphertext, tag);

        // Build aes128gcm header: salt(16) || rs(4) || idlen(1) || keyid(65) || ciphertext || tag
        var recordSize = (uint)(padded.Length + 16); // ciphertext + tag size per record
        using var ms = new MemoryStream();
        ms.Write(salt, 0, 16);
        ms.Write(BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder((int)recordSize)).AsSpan(0, 4));
        ms.WriteByte((byte)ephemeralPublicKeyUncompressed.Length);
        ms.Write(ephemeralPublicKeyUncompressed, 0, ephemeralPublicKeyUncompressed.Length);
        ms.Write(ciphertext, 0, ciphertext.Length);
        ms.Write(tag, 0, tag.Length);
        return ms.ToArray();
    }

    // ──────────── Crypto helpers ────────────

    private static ECPoint DecodeUncompressedPoint(byte[] uncompressed)
    {
        if (uncompressed.Length != 65 || uncompressed[0] != 0x04)
            throw new ArgumentException("Expected 65-byte uncompressed P-256 point (0x04 || X || Y)");

        return new ECPoint
        {
            X = uncompressed[1..33],
            Y = uncompressed[33..65]
        };
    }

    private static byte[] ExportUncompressedPoint(ECDiffieHellman ecdh)
    {
        var p = ecdh.ExportParameters(false).Q;
        var result = new byte[65];
        result[0] = 0x04;
        Buffer.BlockCopy(p.X!, 0, result, 1, 32);
        Buffer.BlockCopy(p.Y!, 0, result, 33, 32);
        return result;
    }

    private static byte[] HkdfExtract(byte[] salt, byte[] ikm)
    {
        using var hmac = new HMACSHA256(salt.Length > 0 ? salt : new byte[32]);
        return hmac.ComputeHash(ikm);
    }

    private static byte[] HkdfExpand(byte[] prk, byte[] info, int length)
    {
        using var hmac = new HMACSHA256(prk);
        var input = new byte[info.Length + 1];
        Buffer.BlockCopy(info, 0, input, 0, info.Length);
        input[^1] = 1; // Counter byte
        var output = hmac.ComputeHash(input);
        return output[..length];
    }

    private static byte[] ConcatBytes(params byte[][] arrays)
    {
        var total = arrays.Sum(a => a.Length);
        var result = new byte[total];
        var offset = 0;
        foreach (var a in arrays)
        {
            Buffer.BlockCopy(a, 0, result, offset, a.Length);
            offset += a.Length;
        }
        return result;
    }

    private static byte[] ConvertDerToRaw(byte[] der)
    {
        // DER: 0x30 len 0x02 rLen r 0x02 sLen s -> raw: r(32) || s(32)
        var raw = new byte[64];
        var offset = 2; // skip 0x30 + total-length
        if (der[offset] != 0x02) throw new InvalidOperationException("Invalid DER signature");
        offset++;
        var rLen = der[offset++];
        var rBytes = der.AsSpan(offset, rLen);
        offset += rLen;
        if (der[offset] != 0x02) throw new InvalidOperationException("Invalid DER signature");
        offset++;
        var sLen = der[offset++];
        var sBytes = der.AsSpan(offset, sLen);

        CopyRightAligned(rBytes, raw.AsSpan(0, 32));
        CopyRightAligned(sBytes, raw.AsSpan(32, 32));
        return raw;
    }

    private static void CopyRightAligned(ReadOnlySpan<byte> src, Span<byte> dest)
    {
        dest.Clear();
        var trimmed = src;
        while (trimmed.Length > dest.Length && trimmed[0] == 0) trimmed = trimmed[1..];
        trimmed.CopyTo(dest[(dest.Length - trimmed.Length)..]);
    }

    // ──────────── Base64url helpers ────────────

    public static string Base64UrlEncode(byte[] data) =>
        Convert.ToBase64String(data).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    public static byte[] Base64UrlDecode(string input)
    {
        var s = input.Replace('-', '+').Replace('_', '/');
        switch (s.Length % 4)
        {
            case 2: s += "=="; break;
            case 3: s += "="; break;
        }
        return Convert.FromBase64String(s);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        _vapidKey.Dispose();
    }
}
